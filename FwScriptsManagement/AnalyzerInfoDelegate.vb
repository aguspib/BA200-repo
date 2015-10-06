Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.App

Namespace Biosystems.Ax00.FwScriptsManagement

    Public Class AnalyzerInfoDelegate
        Inherits BaseFwScriptDelegate

#Region "Constructor"
        Public Sub New(ByVal pAnalyzerID As String, ByVal pFwScriptsDelegate As SendFwScriptsDelegate)
            MyBase.New(pAnalyzerID) 'SGM 20/01/2012
            myFwScriptDelegate = pFwScriptsDelegate
            Me.FwCompatibleAttr = True
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub
#End Region

#Region "Attributes"
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

        Public myLocalRefreshDS As New UIRefreshDS

        Private MaxPOLLHWAttr As Integer = 14
        Private NumPOLLHWAttr As Integer

        Private SerialNumberAttr As String
#End Region

#Region "Properties"
        Public Property SerialNumber() As String
            Get
                Return MyClass.SerialNumberAttr
            End Get
            Set(ByVal value As String)
                MyClass.SerialNumberAttr = value
            End Set
        End Property

        Public ReadOnly Property FwCompatible() As Boolean
            Get
                Return AnalyzerController.Instance.Analyzer.IsFwSwCompatible '#REFACTORING
            End Get
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
                        'If myLocalRefreshDS.FirmwareValueChanged.Rows.Contains(elementID) Then
                        For Each myRow As UIRefreshDS.FirmwareValueChangedRow In myLocalRefreshDS.FirmwareValueChanged.Rows
                            If myRow.ElementID = elementID.ToString Then
                                returnValue = myRow
                                Exit For
                            End If
                        Next
                        'End If
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
                        'If myLocalRefreshDS.CPUValueChanged.Rows.Contains(elementID) Then
                        For Each myRow As UIRefreshDS.CPUValueChangedRow In myLocalRefreshDS.CPUValueChanged.Rows
                            If myRow.ElementID = elementID.ToString Then
                                returnValue = myRow
                                Exit For
                            End If
                        Next
                        'End If
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
                        'If myLocalRefreshDS.ArmValueChanged.Rows.Contains(elementID) Then
                        For Each myRow As UIRefreshDS.ArmValueChangedRow In myLocalRefreshDS.ArmValueChanged.Rows
                            If myRow.ArmID = elementID.ToString Then
                                returnValue = myRow
                                Exit For
                            End If
                        Next
                        'End If
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
                        'If myLocalRefreshDS.ProbeValueChanged.Rows.Contains(elementID) Then
                        For Each myRow As UIRefreshDS.ProbeValueChangedRow In myLocalRefreshDS.ProbeValueChanged.Rows
                            If myRow.ProbeID = elementID.ToString Then
                                returnValue = myRow
                                Exit For
                            End If
                        Next
                        'End If
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
                        'If myLocalRefreshDS.RotorValueChanged.Rows.Contains(elementID) Then
                        For Each myRow As UIRefreshDS.RotorValueChangedRow In myLocalRefreshDS.RotorValueChanged.Rows
                            If myRow.RotorID = elementID.ToString Then
                                returnValue = myRow
                                Exit For
                            End If
                        Next
                        'End If
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
                        'If myLocalRefreshDS.PhotometricsValueChanged.Rows.Contains(elementID) Then
                        For Each myRow As UIRefreshDS.PhotometricsValueChangedRow In myLocalRefreshDS.PhotometricsValueChanged.Rows
                            If myRow.ElementID = elementID.ToString Then
                                returnValue = myRow
                                Exit For
                            End If
                        Next
                        'End If
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
                        'If myLocalRefreshDS.ManifoldValueChanged.Rows.Contains(elementID) Then
                        For Each myRow As UIRefreshDS.ManifoldValueChangedRow In myLocalRefreshDS.ManifoldValueChanged.Rows
                            If myRow.ElementID = elementID.ToString Then
                                returnValue = myRow
                                Exit For
                            End If
                        Next
                        'End If
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
                        'If myLocalRefreshDS.FluidicsValueChanged.Rows.Contains(elementID) Then
                        For Each myRow As UIRefreshDS.FluidicsValueChangedRow In myLocalRefreshDS.FluidicsValueChanged.Rows
                            If myRow.ElementID = elementID.ToString Then
                                returnValue = myRow
                                Exit For
                            End If
                        Next
                        'End If
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
        ''' <summary>
        ''' Refresh this specified delegate with the information received from the Instrument
        ''' </summary>
        ''' <param name="pRefreshEventType"></param>
        ''' <param name="pRefreshDS"></param>
        ''' <remarks>Created by XBC 31/05/2011</remarks>
        Public Sub RefreshDelegate(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As UIRefreshDS)
            Dim myResultData As New GlobalDataTO
            Dim isAlreadyInserted As Boolean = False
            Try
                ' FIRMWARE INFO ***************************************************************************************************************
                If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.FWCPUVALUE_CHANGED) Or _
                   pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.FWARMVALUE_CHANGED) Or _
                   pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.FWPROBEVALUE_CHANGED) Or _
                   pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.FWROTORVALUE_CHANGED) Or _
                   pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.FWPHOTOMETRICVALUE_CHANGED) Or _
                   pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.FWMANIFOLDVALUE_CHANGED) Or _
                   pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.FWFLUIDICSVALUE_CHANGED) Then

                    'AJG Commented just because in the BA400 version (v3.2.0) it's still pending to do
                    AddOrUpdateFirmwareInfoFromInstrument(pRefreshDS)

                    For Each S As UIRefreshDS.FirmwareValueChangedRow In pRefreshDS.FirmwareValueChanged
                        Select Case S.ElementID
                            Case GlobalEnumerates.POLL_IDs.CPU.ToString
                                Me.ReadFWCPUDoneAttr = True
                                Me.ReadFWDoneAttr = True 'PROVISIONAL Until ANSF is available

                            Case GlobalEnumerates.POLL_IDs.BM1.ToString
                                Me.ReadFWBM1DoneAttr = True
                            Case GlobalEnumerates.POLL_IDs.BR1.ToString
                                Me.ReadFWBR1DoneAttr = True
                            Case GlobalEnumerates.POLL_IDs.BR2.ToString
                                Me.ReadFWBR2DoneAttr = True
                            Case GlobalEnumerates.POLL_IDs.AG1.ToString
                                Me.ReadFWAG1DoneAttr = True
                            Case GlobalEnumerates.POLL_IDs.AG2.ToString
                                Me.ReadFWAG2DoneAttr = True
                            Case GlobalEnumerates.POLL_IDs.DR1.ToString
                                Me.ReadFWDR1DoneAttr = True
                            Case GlobalEnumerates.POLL_IDs.DR2.ToString
                                Me.ReadFWDR2DoneAttr = True
                            Case GlobalEnumerates.POLL_IDs.DM1.ToString
                                Me.ReadFWDM1DoneAttr = True
                            Case GlobalEnumerates.POLL_IDs.RR1.ToString
                                Me.ReadFWRR1DoneAttr = True
                            Case GlobalEnumerates.POLL_IDs.RM1.ToString
                                Me.ReadFWRM1DoneAttr = True
                            Case GlobalEnumerates.POLL_IDs.GLF.ToString
                                Me.ReadFWGLFDoneAttr = True
                            Case GlobalEnumerates.POLL_IDs.SF1.ToString
                                Me.ReadFWSF1DoneAttr = True
                            Case GlobalEnumerates.POLL_IDs.JE1.ToString
                                Me.ReadFWJE1DoneAttr = True
                                Me.ReadFWDoneAttr = True
                        End Select
                    Next
                End If


                ' HARDWARE ANALYZER INFO ******************************************************************************************************
                If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.ARMSVALUE_CHANGED) Then
                    '
                    ' ARMS VALUE
                    '
                    ' TODO - PENDING when FW ready to manage this !!!
                    'If myLocalRefreshDS.ArmValueChanged.Rows.Count > 0 Then
                    '    If myLocalRefreshDS.ArmValueChanged.Rows.Contains(pRefreshDS.ArmValueChanged.Rows(0).Item(GlobalEnumerates.ARMS_ELEMENTS.ID)) Then
                    '        For Each myRow As UIRefreshDS.ArmValueChangedRow In myLocalRefreshDS.ArmValueChanged.Rows
                    '            If myRow.ArmID = pRefreshDS.ArmValueChanged.Rows(0).Item(GlobalEnumerates.ARMS_ELEMENTS.ID).ToString Then
                    '                ' Already exists
                    '                isAlreadyInserted = True
                    '                With myRow
                    '                    .BeginEdit()
                    '                    .BoardTemp = CType(pRefreshDS.ArmValueChanged.Rows(0).Item(GlobalEnumerates.ARMS_ELEMENTS.TMP), Single)
                    '                    .MotorHorizontal = pRefreshDS.ArmValueChanged.Rows(0).Item(GlobalEnumerates.ARMS_ELEMENTS.MH).ToString
                    '                    .MotorHorizontalHome = pRefreshDS.ArmValueChanged.Rows(0).Item(GlobalEnumerates.ARMS_ELEMENTS.MHH).ToString
                    '                    .MotorHorizontalPosition = CType(pRefreshDS.ArmValueChanged.Rows(0).Item(GlobalEnumerates.ARMS_ELEMENTS.MHA), Single)
                    '                    .MotorVertical = pRefreshDS.ArmValueChanged.Rows(0).Item(GlobalEnumerates.ARMS_ELEMENTS.MV).ToString
                    '                    .MotorVerticalHome = pRefreshDS.ArmValueChanged.Rows(0).Item(GlobalEnumerates.ARMS_ELEMENTS.MVH).ToString
                    '                    .MotorVerticalPosition = CType(pRefreshDS.ArmValueChanged.Rows(0).Item(GlobalEnumerates.ARMS_ELEMENTS.MVA), Single)
                    '                    .EndEdit()
                    '                End With
                    '                Exit For
                    '            End If
                    '        Next
                    '    End If
                    'End If

                    'If Not isAlreadyInserted Then
                    '    'Generate UI_Refresh Arms values dataset
                    '    Dim myNewArmsValuesRow As UIRefreshDS.ArmValueChangedRow
                    '    myNewArmsValuesRow = myLocalRefreshDS.ArmValueChanged.NewArmValueChangedRow
                    '    With myNewArmsValuesRow
                    '        .BeginEdit()
                    '        .ArmID = pRefreshDS.ArmValueChanged.Rows(0).Item(GlobalEnumerates.ARMS_ELEMENTS.ID).ToString
                    '        .BoardTemp = CType(pRefreshDS.ArmValueChanged.Rows(0).Item(GlobalEnumerates.ARMS_ELEMENTS.TMP), Single)
                    '        .MotorHorizontal = pRefreshDS.ArmValueChanged.Rows(0).Item(GlobalEnumerates.ARMS_ELEMENTS.MH).ToString
                    '        .MotorHorizontalHome = pRefreshDS.ArmValueChanged.Rows(0).Item(GlobalEnumerates.ARMS_ELEMENTS.MHH).ToString
                    '        .MotorHorizontalPosition = CType(pRefreshDS.ArmValueChanged.Rows(0).Item(GlobalEnumerates.ARMS_ELEMENTS.MHA), Single)
                    '        .MotorVertical = pRefreshDS.ArmValueChanged.Rows(0).Item(GlobalEnumerates.ARMS_ELEMENTS.MV).ToString
                    '        .MotorVerticalHome = pRefreshDS.ArmValueChanged.Rows(0).Item(GlobalEnumerates.ARMS_ELEMENTS.MVH).ToString
                    '        .MotorVerticalPosition = CType(pRefreshDS.ArmValueChanged.Rows(0).Item(GlobalEnumerates.ARMS_ELEMENTS.MVA), Single)
                    '        .EndEdit()
                    '    End With
                    '    myLocalRefreshDS.ArmValueChanged.AddArmValueChangedRow(myNewArmsValuesRow)
                    'End If

                    'myLocalRefreshDS.AcceptChanges()
                    ' TODO - PENDING when FW ready to manage this !!!

                    For Each S As UIRefreshDS.ArmValueChangedRow In pRefreshDS.ArmValueChanged
                        Select Case S.ArmID
                            Case GlobalEnumerates.POLL_IDs.BM1.ToString
                                Me.ReadBM1InfoDoneAttr = True
                            Case GlobalEnumerates.POLL_IDs.BR1.ToString
                                Me.ReadBR1InfoDoneAttr = True
                            Case GlobalEnumerates.POLL_IDs.BR2.ToString
                                Me.ReadBR2InfoDoneAttr = True
                            Case GlobalEnumerates.POLL_IDs.AG1.ToString
                                Me.ReadAG1InfoDoneAttr = True
                            Case GlobalEnumerates.POLL_IDs.AG2.ToString
                                Me.ReadAG2InfoDoneAttr = True
                        End Select
                    Next
                End If

                If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.PROBESVALUE_CHANGED) Then
                    '
                    ' PROBES VALUE
                    '
                    ' TODO - PENDING when FW ready to manage this !!!
                    'If myLocalRefreshDS.ProbeValueChanged.Rows.Count > 0 Then
                    '    If myLocalRefreshDS.ProbeValueChanged.Rows.Contains(pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.ID)) Then
                    '        For Each myRow As UIRefreshDS.ProbeValueChangedRow In myLocalRefreshDS.ProbeValueChanged.Rows
                    '            If myRow.ProbeID = pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.ID).ToString Then
                    '                ' Already exists
                    '                isAlreadyInserted = True
                    '                With myRow
                    '                    .BeginEdit()
                    '                    .BoardTemp = CType(pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.TMP), Single)
                    '                    .DetectionStatus = pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.DST).ToString
                    '                    .DetectionFrequency = CType(pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.DFQ), Single)
                    '                    .Detection = pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.D).ToString
                    '                    .LastInternalRate = CType(pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.DCV), Single)
                    '                    .ThermistorValue = CType(pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.PTH), Single)
                    '                    .ThermistorDiagnostic = CType(pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.PTHD), Single)
                    '                    .HeaterStatus = pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.PH).ToString
                    '                    .HeaterDiagnostic = CType(pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.PHD), Single)
                    '                    .CollisionDetector = pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.CD).ToString
                    '                    .EndEdit()
                    '                End With
                    '                Exit For
                    '            End If
                    '        Next
                    '    End If
                    'End If

                    'If Not isAlreadyInserted Then
                    '    'Generate UI_Refresh Probes values dataset
                    '    Dim myNewprobesValuesRow As UIRefreshDS.ProbeValueChangedRow
                    '    myNewprobesValuesRow = myLocalRefreshDS.ProbeValueChanged.NewProbeValueChangedRow
                    '    With myNewprobesValuesRow
                    '        .BeginEdit()
                    '        .ProbeID = pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.ID).ToString
                    '        .BoardTemp = CType(pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.TMP), Single)
                    '        .DetectionStatus = pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.DST).ToString
                    '        .DetectionFrequency = CType(pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.DFQ), Single)
                    '        .Detection = pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.D).ToString
                    '        .LastInternalRate = CType(pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.DCV), Single)
                    '        .ThermistorValue = CType(pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.PTH), Single)
                    '        .ThermistorDiagnostic = CType(pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.PTHD), Single)
                    '        .HeaterStatus = pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.PH).ToString
                    '        .HeaterDiagnostic = CType(pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.PHD), Single)
                    '        .CollisionDetector = pRefreshDS.ProbeValueChanged.Rows(0).Item(GlobalEnumerates.PROBES_ELEMENTS.CD).ToString
                    '        .EndEdit()
                    '    End With
                    '    myLocalRefreshDS.ProbeValueChanged.AddProbeValueChangedRow(myNewprobesValuesRow)
                    'End If

                    'myLocalRefreshDS.AcceptChanges()
                    ' TODO - PENDING when FW ready to manage this !!!

                    For Each S As UIRefreshDS.ProbeValueChangedRow In pRefreshDS.ProbeValueChanged
                        Select Case S.ProbeID
                            Case GlobalEnumerates.POLL_IDs.DR1.ToString
                                Me.ReadDR1InfoDoneAttr = True
                            Case GlobalEnumerates.POLL_IDs.DR2.ToString
                                Me.ReadDR2InfoDoneAttr = True
                            Case GlobalEnumerates.POLL_IDs.DM1.ToString
                                Me.ReadDM1InfoDoneAttr = True
                        End Select
                    Next
                End If

                If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.ROTORSVALUE_CHANGED) Then
                    '
                    ' ROTORS VALUE
                    '
                    ' TODO - PENDING when FW ready to manage this !!!
                    'If myLocalRefreshDS.RotorValueChanged.Rows.Count > 0 Then
                    '    If myLocalRefreshDS.RotorValueChanged.Rows.Contains(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.ID)) Then
                    '        For Each myRow As UIRefreshDS.RotorValueChangedRow In myLocalRefreshDS.RotorValueChanged.Rows
                    '            If myRow.RotorID = pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.ID).ToString Then
                    '                ' Already exists
                    '                isAlreadyInserted = True
                    '                With myRow
                    '                    .BeginEdit()
                    '                    .BoardTemp = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.TMP), Single)
                    '                    .Motor = pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.MR).ToString
                    '                    .MotorHome = pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.MRH).ToString
                    '                    .MotorPosition = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.MRA), Single)
                    '                    .ThermistorFridgeValue = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.FTH), Single)
                    '                    .ThermistorFridgeDiagnostic = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.FTHD), Single)
                    '                    .PeltiersFridgeStatus = pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.FH).ToString
                    '                    .PeltiersFridgeDiagnostic = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.FHD), Single)
                    '                    .PeltiersFan1Speed = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.PF1), Single)
                    '                    .PeltiersFan1Diagnostic = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.PF1D), Single)
                    '                    .PeltiersFan2Speed = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.PF2), Single)
                    '                    .PeltiersFan2Diagnostic = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.PF2D), Single)
                    '                    .PeltiersFan3Speed = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.PF3), Single)
                    '                    .PeltiersFan3Diagnostic = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.PF3D), Single)
                    '                    .PeltiersFan4Speed = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.PF4), Single)
                    '                    .PeltiersFan4Diagnostic = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.PF4D), Single)
                    '                    .FrameFan1Speed = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.FF1), Single)
                    '                    .FrameFan1Diagnostic = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.FF1D), Single)
                    '                    .FrameFan2Speed = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.FF2), Single)
                    '                    .FrameFan2Diagnostic = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.FF2D), Single)
                    '                    .Cover = pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.RC).ToString
                    '                    .BarCodeStatus = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.CB), Single)
                    '                    .BarcodeError = pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.CBE).ToString
                    '                    .EndEdit()
                    '                End With
                    '                Exit For
                    '            End If
                    '        Next
                    '    End If
                    'End If

                    'If Not isAlreadyInserted Then
                    '    'Generate UI_Refresh Rotors values dataset
                    '    Dim myNewrotorsValuesRow As UIRefreshDS.RotorValueChangedRow
                    '    myNewrotorsValuesRow = myLocalRefreshDS.RotorValueChanged.NewRotorValueChangedRow
                    '    With myNewrotorsValuesRow
                    '        .BeginEdit()
                    '        .RotorID = pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.ID).ToString
                    '        .BoardTemp = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.TMP), Single)
                    '        .Motor = pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.MR).ToString
                    '        .MotorHome = pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.MRH).ToString
                    '        .MotorPosition = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.MRA), Single)
                    '        .ThermistorFridgeValue = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.FTH), Single)
                    '        .ThermistorFridgeDiagnostic = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.FTHD), Single)
                    '        .PeltiersFridgeStatus = pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.FH).ToString
                    '        .PeltiersFridgeDiagnostic = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.FHD), Single)
                    '        .PeltiersFan1Speed = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.PF1), Single)
                    '        .PeltiersFan1Diagnostic = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.PF1D), Single)
                    '        .PeltiersFan2Speed = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.PF2), Single)
                    '        .PeltiersFan2Diagnostic = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.PF2D), Single)
                    '        .PeltiersFan3Speed = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.PF3), Single)
                    '        .PeltiersFan3Diagnostic = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.PF3D), Single)
                    '        .PeltiersFan4Speed = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.PF4), Single)
                    '        .PeltiersFan4Diagnostic = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.PF4D), Single)
                    '        .FrameFan1Speed = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.FF1), Single)
                    '        .FrameFan1Diagnostic = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.FF1D), Single)
                    '        .FrameFan2Speed = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.FF2), Single)
                    '        .FrameFan2Diagnostic = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.FF2D), Single)
                    '        .Cover = pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.RC).ToString
                    '        .BarCodeStatus = CType(pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.CB), Single)
                    '        .BarcodeError = pRefreshDS.RotorValueChanged.Rows(0).Item(GlobalEnumerates.ROTORS_ELEMENTS.CBE).ToString
                    '        .EndEdit()
                    '    End With
                    '    myLocalRefreshDS.RotorValueChanged.AddRotorValueChangedRow(myNewrotorsValuesRow)
                    'End If

                    'myLocalRefreshDS.AcceptChanges()
                    ' TODO - PENDING when FW ready to manage this !!!

                    For Each S As UIRefreshDS.RotorValueChangedRow In pRefreshDS.RotorValueChanged
                        Select Case S.RotorID
                            Case GlobalEnumerates.POLL_IDs.RM1.ToString
                                Me.ReadRM1InfoDoneAttr = True
                            Case GlobalEnumerates.POLL_IDs.RR1.ToString
                                Me.ReadRR1InfoDoneAttr = True
                        End Select
                    Next
                End If


                If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.PHOTOMETRICSVALUE_CHANGED) Then
                    '
                    ' PHOTOMETRICS VALUES
                    '
                    'Generate UI_Refresh Photometrics values dataset
                    ' TODO - PENDING when FW ready to manage this !!!
                    'myLocalRefreshDS.PhotometricsValueChanged.Rows.Clear()

                    'For Each myRow As UIRefreshDS.PhotometricsValueChangedRow In pRefreshDS.PhotometricsValueChanged.Rows
                    '    Dim myNewPhotometricsValuesRow As UIRefreshDS.PhotometricsValueChangedRow
                    '    myNewPhotometricsValuesRow = myLocalRefreshDS.PhotometricsValueChanged.NewPhotometricsValueChangedRow
                    '    With myNewPhotometricsValuesRow
                    '        .BeginEdit()
                    '        .ElementID = myRow.ElementID
                    '        .Value = myRow.Value
                    '        .EndEdit()
                    '    End With
                    '    myLocalRefreshDS.PhotometricsValueChanged.AddPhotometricsValueChangedRow(myNewPhotometricsValuesRow)
                    'Next
                    'myLocalRefreshDS.AcceptChanges()
                    ' TODO - PENDING when FW ready to manage this !!!

                    Me.ReadGLFInfoDoneAttr = True
                End If


                If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.MANIFOLDVALUE_CHANGED) Then
                    '
                    ' MANIFOLD VALUES
                    '
                    'Generate UI_Refresh Manifold values dataset
                    ' TODO - PENDING when FW ready to manage this !!!
                    'myLocalRefreshDS.ManifoldValueChanged.Rows.Clear()

                    'For Each myRow As UIRefreshDS.ManifoldValueChangedRow In pRefreshDS.ManifoldValueChanged.Rows
                    '    Dim myNewManifoldValuesRow As UIRefreshDS.ManifoldValueChangedRow
                    '    myNewManifoldValuesRow = myLocalRefreshDS.ManifoldValueChanged.NewManifoldValueChangedRow
                    '    With myNewManifoldValuesRow
                    '        .BeginEdit()
                    '        .ElementID = myRow.ElementID
                    '        .Value = myRow.Value
                    '        .EndEdit()
                    '    End With
                    '    myLocalRefreshDS.ManifoldValueChanged.AddManifoldValueChangedRow(myNewManifoldValuesRow)
                    'Next
                    'myLocalRefreshDS.AcceptChanges()
                    ' TODO - PENDING when FW ready to manage this !!!

                    Me.ReadJEXInfoDoneAttr = True
                End If

                If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.FLUIDICSVALUE_CHANGED) Then
                    '
                    ' FLUIDICS VALUES
                    '
                    'Generate UI_Refresh Fluidics values dataset
                    ' TODO - PENDING when FW ready to manage this !!!
                    'myLocalRefreshDS.FluidicsValueChanged.Rows.Clear()

                    'For Each myRow As UIRefreshDS.FluidicsValueChangedRow In pRefreshDS.FluidicsValueChanged.Rows
                    '    Dim myNewFLUIDICSValuesRow As UIRefreshDS.FluidicsValueChangedRow
                    '    myNewFLUIDICSValuesRow = myLocalRefreshDS.FluidicsValueChanged.NewFluidicsValueChangedRow
                    '    With myNewFLUIDICSValuesRow
                    '        .BeginEdit()
                    '        .ElementID = myRow.ElementID
                    '        .Value = myRow.Value
                    '        .EndEdit()
                    '    End With
                    '    myLocalRefreshDS.FluidicsValueChanged.AddFluidicsValueChangedRow(myNewFLUIDICSValuesRow)
                    'Next
                    'myLocalRefreshDS.AcceptChanges()
                    ' TODO - PENDING when FW ready to manage this !!!

                    Me.ReadSFXInfoDoneAttr = True
                End If

                If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.CPUVALUE_CHANGED) Then
                    '
                    ' CPU VALUES
                    '
                    ' TODO - PENDING when FW ready to manage this !!!
                    ''Generate UI_Refresh Cpu values dataset
                    'myLocalRefreshDS.CPUValueChanged.Rows.Clear()

                    'For Each myRow As UIRefreshDS.CPUValueChangedRow In pRefreshDS.CPUValueChanged.Rows
                    '    Dim myNewCpuValuesRow As UIRefreshDS.CPUValueChangedRow
                    '    myNewCpuValuesRow = myLocalRefreshDS.CPUValueChanged.NewCPUValueChangedRow
                    '    With myNewCpuValuesRow
                    '        .BeginEdit()
                    '        .ElementID = myRow.ElementID
                    '        .Value = myRow.Value
                    '        .EndEdit()
                    '    End With
                    '    myLocalRefreshDS.CPUValueChanged.AddCPUValueChangedRow(myNewCpuValuesRow)
                    'Next
                    'myLocalRefreshDS.AcceptChanges()
                    ' TODO - PENDING when FW ready to manage this !!!

                    Me.ReadCPUInfoDoneAttr = True
                End If

                ' ENDING CHECKS ***************************************************************************************************************
                If Me.ReadFWCPUDoneAttr And _
                   Me.ReadFWBM1DoneAttr And _
                   Me.ReadFWBR1DoneAttr And _
                   Me.ReadFWBR2DoneAttr And _
                   Me.ReadFWAG1DoneAttr And _
                   Me.ReadFWAG2DoneAttr And _
                   Me.ReadFWDR1DoneAttr And _
                   Me.ReadFWDR2DoneAttr And _
                   Me.ReadFWDM1DoneAttr And _
                   Me.ReadFWRR1DoneAttr And _
                   Me.ReadFWRM1DoneAttr And _
                   Me.ReadFWGLFDoneAttr And _
                   Me.ReadFWSF1DoneAttr And _
                   Me.ReadFWJE1DoneAttr Then

                    ' With all Analyzer FW information readed...
                    Me.ReadFWDoneAttr = True
                End If

                If Me.ReadBM1InfoDoneAttr And _
                   Me.ReadBR1InfoDoneAttr And _
                   Me.ReadBR2InfoDoneAttr And _
                   Me.ReadAG1InfoDoneAttr And _
                   Me.ReadAG2InfoDoneAttr And _
                   Me.ReadDR1InfoDoneAttr And _
                   Me.ReadDR2InfoDoneAttr And _
                   Me.ReadDM1InfoDoneAttr And _
                   Me.ReadRM1InfoDoneAttr And _
                   Me.ReadRR1InfoDoneAttr And _
                   Me.ReadGLFInfoDoneAttr And _
                   Me.ReadJEXInfoDoneAttr And _
                   Me.ReadSFXInfoDoneAttr And _
                   Me.ReadCPUInfoDoneAttr Then

                    ' With all Analyzer HW information readed...
                    Me.ReadAnalyzerInfoDoneAttr = True
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerInfoDelegate.RefreshDelegate", EventLogEntryType.Error, False)
            End Try
        End Sub

        Private Sub AddOrUpdateFirmwareInfoFromInstrument(pRefreshDS As UIRefreshDS)

            If myLocalRefreshDS.FirmwareValueChanged.Rows.Count > 0 AndAlso myLocalRefreshDS.FirmwareValueChanged.Rows.Contains(pRefreshDS.FirmwareValueChanged.Rows(0).Item(GlobalEnumerates.FW_INFO.ID)) Then

                Dim RowToUpdate As UIRefreshDS.FirmwareValueChangedRow = CType(myLocalRefreshDS.FirmwareValueChanged.Rows.Find(pRefreshDS.FirmwareValueChanged.Rows(0).Item(GlobalEnumerates.FW_INFO.ID)), UIRefreshDS.FirmwareValueChangedRow)
                UpdateUIRefreshDS(RowToUpdate, pRefreshDS.FirmwareValueChanged.Rows(0))
              
            Else
                'Generate UI_Refresh Firmware values dataset
                Dim NewRowToUpdate As UIRefreshDS.FirmwareValueChangedRow
                NewRowToUpdate = myLocalRefreshDS.FirmwareValueChanged.NewFirmwareValueChangedRow
                UpdateUIRefreshDS(NewRowToUpdate, pRefreshDS.FirmwareValueChanged.Rows(0))

                myLocalRefreshDS.FirmwareValueChanged.AddFirmwareValueChangedRow(NewRowToUpdate)
            End If

            myLocalRefreshDS.AcceptChanges()
          
        End Sub

        Private Sub UpdateUIRefreshDS(destinationRow As UIRefreshDS.FirmwareValueChangedRow, sourceRow As System.Data.DataRow)

            destinationRow.BeginEdit()
            destinationRow.ElementID = sourceRow.Item(GlobalEnumerates.FW_INFO.ID).ToString
            destinationRow.BoardSerialNumber = sourceRow.Item(GlobalEnumerates.FW_INFO.SMC).ToString

            destinationRow.RepositoryVersion = sourceRow.Item(GlobalEnumerates.FW_INFO.RV).ToString
            destinationRow.RepositoryCRCResult = sourceRow.Item(GlobalEnumerates.FW_INFO.CRC).ToString
            destinationRow.RepositoryCRCValue = sourceRow.Item(GlobalEnumerates.FW_INFO.CRCV).ToString
            destinationRow.RepositoryCRCSize = sourceRow.Item(GlobalEnumerates.FW_INFO.CRCS).ToString

            destinationRow.BoardFirmwareVersion = sourceRow.Item(GlobalEnumerates.FW_INFO.FWV).ToString
            destinationRow.BoardFirmwareCRCResult = sourceRow.Item(GlobalEnumerates.FW_INFO.FWCRC).ToString
            destinationRow.BoardFirmwareCRCValue = sourceRow.Item(GlobalEnumerates.FW_INFO.FWCRCV).ToString
            destinationRow.BoardFirmwareCRCSize = sourceRow.Item(GlobalEnumerates.FW_INFO.FWCRCS).ToString

            destinationRow.BoardHardwareVersion = sourceRow.Item(GlobalEnumerates.FW_INFO.HWV).ToString
            destinationRow.AnalyzerSerialNumber = sourceRow.Item(GlobalEnumerates.FW_INFO.ASN).ToString
            destinationRow.EndEdit()

        End Sub


        '''' <summary>
        '''' Save Serial Number with UTIL High Level Instruction to save values into the instrument 
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>
        '''' Created by XBC 30/05/2011
        '''' Modified by XBC 04/06/2012
        '''' </remarks>
        'Public Function SendUTIL() As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Dim myParams As New List(Of String)
        '    Try
        '        myParams.Add("0")
        '        myParams.Add("0")
        '        myParams.Add("1")
        '        myParams.Add(MyClass.SerialNumberAttr)

        '        myResultData = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.UTIL, True, Nothing, Nothing, "", myParams)

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "AnalyzerInfoDelegate.SendUTIL", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function

        ''' <summary>
        ''' High Level Instruction to read Firmare information from the instrument 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 02/06/2011</remarks>
        Public Function REQUEST_FW_INFO() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try

                If Not ReadFWCPUDoneAttr Then
                    myResultData = MyClass.SendINFO_STOP
                    System.Threading.Thread.Sleep(1000)
                    myResultData = MyBase.SendPOLLFW(POLL_IDs.CPU)

                ElseIf Not ReadFWBM1DoneAttr Then
                    myResultData = MyBase.SendPOLLFW(POLL_IDs.BM1)
                ElseIf Not ReadFWBR1DoneAttr Then
                    myResultData = MyBase.SendPOLLFW(POLL_IDs.BR1)
                ElseIf Not ReadFWBR2DoneAttr Then
                    myResultData = MyBase.SendPOLLFW(POLL_IDs.BR2)
                ElseIf Not ReadFWAG1DoneAttr Then
                    myResultData = MyBase.SendPOLLFW(POLL_IDs.AG1)
                ElseIf Not ReadFWAG2DoneAttr Then
                    myResultData = MyBase.SendPOLLFW(POLL_IDs.AG2)
                ElseIf Not ReadFWDR1DoneAttr Then
                    myResultData = MyBase.SendPOLLFW(POLL_IDs.DR1)
                ElseIf Not ReadFWDR2DoneAttr Then
                    myResultData = MyBase.SendPOLLFW(POLL_IDs.DR2)
                ElseIf Not ReadFWDM1DoneAttr Then
                    myResultData = MyBase.SendPOLLFW(POLL_IDs.DM1)
                ElseIf Not ReadFWRR1DoneAttr Then
                    myResultData = MyBase.SendPOLLFW(POLL_IDs.RR1)
                ElseIf Not ReadFWRM1DoneAttr Then
                    myResultData = MyBase.SendPOLLFW(POLL_IDs.RM1)
                ElseIf Not ReadFWGLFDoneAttr Then
                    myResultData = MyBase.SendPOLLFW(POLL_IDs.GLF)
                ElseIf Not ReadFWSF1DoneAttr Then
                    myResultData = MyBase.SendPOLLFW(POLL_IDs.SF1)
                ElseIf Not ReadFWJE1DoneAttr Then
                    myResultData = MyBase.SendPOLLFW(POLL_IDs.JE1)
                End If


            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerInfoDelegate.REQUEST_FW_INFO", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' High Level Instruction to read Hardware information from the instrument 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 02/06/2011</remarks>
        Public Function REQUEST_ANALYZER_INFO() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                ' ARMS
                If Not ReadBM1InfoDoneAttr Then
                    Me.NumPOLLHWAttr = 1
                    myResultData = MyBase.SendPOLLHW(POLL_IDs.BM1)
                ElseIf Not ReadBR1InfoDoneAttr Then
                    Me.NumPOLLHWAttr += 1
                    myResultData = MyBase.SendPOLLHW(POLL_IDs.BR1)
                ElseIf Not ReadBR2InfoDoneAttr Then
                    Me.NumPOLLHWAttr += 1
                    myResultData = MyBase.SendPOLLHW(POLL_IDs.BR2)
                ElseIf Not ReadAG1InfoDoneAttr Then
                    Me.NumPOLLHWAttr += 1
                    myResultData = MyBase.SendPOLLHW(POLL_IDs.AG1)
                ElseIf Not ReadAG2InfoDoneAttr Then
                    Me.NumPOLLHWAttr += 1
                    myResultData = MyBase.SendPOLLHW(POLL_IDs.AG2)

                    ' PROBES
                ElseIf Not ReadDR1InfoDoneAttr Then
                    Me.NumPOLLHWAttr += 1
                    myResultData = MyBase.SendPOLLHW(POLL_IDs.DR1)
                ElseIf Not ReadDR2InfoDoneAttr Then
                    Me.NumPOLLHWAttr += 1
                    myResultData = MyBase.SendPOLLHW(POLL_IDs.DR2)
                ElseIf Not ReadDM1InfoDoneAttr Then
                    Me.NumPOLLHWAttr += 1
                    myResultData = MyBase.SendPOLLHW(POLL_IDs.DM1)

                    ' ROTORS
                ElseIf Not ReadRR1InfoDoneAttr Then
                    Me.NumPOLLHWAttr += 1
                    myResultData = MyBase.SendPOLLHW(POLL_IDs.RR1)
                ElseIf Not ReadRM1InfoDoneAttr Then
                    Me.NumPOLLHWAttr += 1
                    myResultData = MyBase.SendPOLLHW(POLL_IDs.RM1)

                    ' PHOTOMETRICS
                ElseIf Not ReadGLFInfoDoneAttr Then
                    Me.NumPOLLHWAttr += 1
                    myResultData = MyBase.SendPOLLHW(POLL_IDs.GLF)

                    ' MANIFOLD
                ElseIf Not ReadJEXInfoDoneAttr Then
                    Me.NumPOLLHWAttr += 1
                    myResultData = MyBase.SendPOLLHW(POLL_IDs.JE1)

                    ' FLUIDICS
                ElseIf Not ReadSFXInfoDoneAttr Then
                    Me.NumPOLLHWAttr += 1
                    myResultData = MyBase.SendPOLLHW(POLL_IDs.SF1)

                    ' CPU
                ElseIf Not ReadCPUInfoDoneAttr Then
                    Me.NumPOLLHWAttr += 1
                    myResultData = MyBase.SendPOLLHW(POLL_IDs.CPU)

                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerInfoDelegate.REQUEST_ANALYZER_INFO", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Validation Function of CPU information received from the Instrument
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 08/06/2011</remarks>
        Public Function CheckCPU() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Dim myTempResult As String
                Dim errorDetected As Boolean = False
                Dim errorDescription As String = ""
                Dim myFirmwareValueRow As UIRefreshDS.FirmwareValueChangedRow
                Dim myCPUValueRow As UIRefreshDS.CPUValueChangedRow

                myFirmwareValueRow = InfoFirmware(POLL_IDs.CPU)
                If myFirmwareValueRow IsNot Nothing Then
                    myTempResult = myFirmwareValueRow.BoardFirmwareCRCResult.ToString
                    If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                        errorDetected = True
                        errorDescription += "Error! Incorrect Program CRC32" + vbCrLf
                    End If
                End If

                myCPUValueRow = InfoCpu(CPU_ELEMENTS.CPU_CAN)
                If myCPUValueRow IsNot Nothing Then
                    myTempResult = myCPUValueRow.Value.ToString
                    If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                        errorDetected = True
                        errorDescription += "Error detected in CAN BUS!" + vbCrLf
                    End If
                End If

                myCPUValueRow = InfoCpu(CPU_ELEMENTS.CPU_CAN_BM1)
                If myCPUValueRow IsNot Nothing Then
                    myTempResult = myCPUValueRow.Value.ToString
                    If IsNumeric(myTempResult) Then
                        If CInt(myTempResult) > 0 Then
                            errorDetected = True
                            errorDescription += "Diagnostic Fail in SAMPLE ARM CAN BUS!" + vbCrLf
                        End If
                    Else
                        errorDetected = True
                        errorDescription += "Diagnostic Fail in SAMPLE ARM CAN BUS!" + vbCrLf
                    End If
                End If

                myCPUValueRow = InfoCpu(CPU_ELEMENTS.CPU_CAN_BR1)
                If myCPUValueRow IsNot Nothing Then
                    myTempResult = myCPUValueRow.Value.ToString
                    If IsNumeric(myTempResult) Then
                        If CInt(myTempResult) > 0 Then
                            errorDetected = True
                            errorDescription += "Diagnostic Fail in REAGENT1 ARM CAN BUS!" + vbCrLf
                        End If
                    Else
                        errorDetected = True
                        errorDescription += "Diagnostic Fail in REAGENT1 ARM CAN BUS!" + vbCrLf
                    End If
                End If

                myCPUValueRow = InfoCpu(CPU_ELEMENTS.CPU_CAN_BR2)
                If myCPUValueRow IsNot Nothing Then
                    myTempResult = myCPUValueRow.Value.ToString
                    If IsNumeric(myTempResult) Then
                        If CInt(myTempResult) > 0 Then
                            errorDetected = True
                            errorDescription += "Diagnostic Fail in REAGENT2 ARM CAN BUS!" + vbCrLf
                        End If
                    Else
                        errorDetected = True
                        errorDescription += "Diagnostic Fail in REAGENT2 ARM CAN BUS!" + vbCrLf
                    End If
                End If

                myCPUValueRow = InfoCpu(CPU_ELEMENTS.CPU_CAN_AG1)
                If myCPUValueRow IsNot Nothing Then
                    myTempResult = myCPUValueRow.Value.ToString
                    If IsNumeric(myTempResult) Then
                        If CInt(myTempResult) > 0 Then
                            errorDetected = True
                            errorDescription += "Diagnostic Fail in MIXER1 ARM CAN BUS!" + vbCrLf
                        End If
                    Else
                        errorDetected = True
                        errorDescription += "Diagnostic Fail in MIXER1 ARM CAN BUS!" + vbCrLf
                    End If
                End If

                myCPUValueRow = InfoCpu(CPU_ELEMENTS.CPU_CAN_AG2)
                If myCPUValueRow IsNot Nothing Then
                    myTempResult = myCPUValueRow.Value.ToString
                    If IsNumeric(myTempResult) Then
                        If CInt(myTempResult) > 0 Then
                            errorDetected = True
                            errorDescription += "Diagnostic Fail in MIXER2 ARM CAN BUS!" + vbCrLf
                        End If
                    Else
                        errorDetected = True
                        errorDescription += "Diagnostic Fail in MIXER2 ARM CAN BUS!" + vbCrLf
                    End If
                End If

                myCPUValueRow = InfoCpu(CPU_ELEMENTS.CPU_CAN_DM1)
                If myCPUValueRow IsNot Nothing Then
                    myTempResult = myCPUValueRow.Value.ToString
                    If IsNumeric(myTempResult) Then
                        If CInt(myTempResult) > 0 Then
                            errorDetected = True
                            errorDescription += "Diagnostic Fail in SAMPLE PROBE CAN BUS!" + vbCrLf
                        End If
                    Else
                        errorDetected = True
                        errorDescription += "Diagnostic Fail in SAMPLE PROBE CAN BUS!" + vbCrLf
                    End If
                End If

                myCPUValueRow = InfoCpu(CPU_ELEMENTS.CPU_CAN_DR1)
                If myCPUValueRow IsNot Nothing Then
                    myTempResult = myCPUValueRow.Value.ToString
                    If IsNumeric(myTempResult) Then
                        If CInt(myTempResult) > 0 Then
                            errorDetected = True
                            errorDescription += "Diagnostic Fail in REAGENT1 PROBE CAN BUS!" + vbCrLf
                        End If
                    Else
                        errorDetected = True
                        errorDescription += "Diagnostic Fail in REAGENT1 PROBE CAN BUS!" + vbCrLf
                    End If
                End If

                myCPUValueRow = InfoCpu(CPU_ELEMENTS.CPU_CAN_DR2)
                If myCPUValueRow IsNot Nothing Then
                    myTempResult = myCPUValueRow.Value.ToString
                    If IsNumeric(myTempResult) Then
                        If CInt(myTempResult) > 0 Then
                            errorDetected = True
                            errorDescription += "Diagnostic Fail in REAGENT2 PROBE CAN BUS!" + vbCrLf
                        End If
                    Else
                        errorDetected = True
                        errorDescription += "Diagnostic Fail in REAGENT2 PROBE CAN BUS!" + vbCrLf
                    End If
                End If

                myCPUValueRow = InfoCpu(CPU_ELEMENTS.CPU_CAN_RR1)
                If myCPUValueRow IsNot Nothing Then
                    myTempResult = myCPUValueRow.Value.ToString
                    If IsNumeric(myTempResult) Then
                        If CInt(myTempResult) > 0 Then
                            errorDetected = True
                            errorDescription += "Diagnostic Fail in REAGENTS ROTOR CAN BUS!" + vbCrLf
                        End If
                    Else
                        errorDetected = True
                        errorDescription += "Diagnostic Fail in REAGENTS ROTOR CAN BUS!" + vbCrLf
                    End If
                End If

                myCPUValueRow = InfoCpu(CPU_ELEMENTS.CPU_CAN_RM1)
                If myCPUValueRow IsNot Nothing Then
                    myTempResult = myCPUValueRow.Value.ToString
                    If IsNumeric(myTempResult) Then
                        If CInt(myTempResult) > 0 Then
                            errorDetected = True
                            errorDescription += "Diagnostic Fail in SAMPLES ROTOR CAN BUS!" + vbCrLf
                        End If
                    Else
                        errorDetected = True
                        errorDescription += "Diagnostic Fail in SAMPLES ROTOR CAN BUS!" + vbCrLf
                    End If
                End If

                myCPUValueRow = InfoCpu(CPU_ELEMENTS.CPU_CAN_GLF)
                If myCPUValueRow IsNot Nothing Then
                    myTempResult = myCPUValueRow.Value.ToString
                    If IsNumeric(myTempResult) Then
                        If CInt(myTempResult) > 0 Then
                            errorDetected = True
                            errorDescription += "Diagnostic Fail in PHOTOMETRIC CAN BUS!" + vbCrLf
                        End If
                    Else
                        errorDetected = True
                        errorDescription += "Diagnostic Fail in PHOTOMETRIC ROTOR CAN BUS!" + vbCrLf
                    End If
                End If

                myCPUValueRow = InfoCpu(CPU_ELEMENTS.CPU_CAN_SF1)
                If myCPUValueRow IsNot Nothing Then
                    myTempResult = myCPUValueRow.Value.ToString
                    If IsNumeric(myTempResult) Then
                        If CInt(myTempResult) > 0 Then
                            errorDetected = True
                            errorDescription += "Diagnostic Fail in FLUIDICS CAN BUS!" + vbCrLf
                        End If
                    Else
                        errorDetected = True
                        errorDescription += "Diagnostic Fail in FLUIDICS CAN BUS!" + vbCrLf
                    End If
                End If

                myCPUValueRow = InfoCpu(CPU_ELEMENTS.CPU_CAN_JE1)
                If myCPUValueRow IsNot Nothing Then
                    myTempResult = myCPUValueRow.Value.ToString
                    If IsNumeric(myTempResult) Then
                        If CInt(myTempResult) > 0 Then
                            errorDetected = True
                            errorDescription += "Diagnostic Fail in MANIFOLD CAN BUS!" + vbCrLf
                        End If
                    Else
                        errorDetected = True
                        errorDescription += "Diagnostic Fail in MANIFOLD CAN BUS!" + vbCrLf
                    End If
                End If

                myCPUValueRow = InfoCpu(CPU_ELEMENTS.CPU_BUZ)
                If myCPUValueRow IsNot Nothing Then
                    myTempResult = myCPUValueRow.Value.ToString
                    If myTempResult = HW_DC_STATES.DI.ToString Then
                        errorDetected = True
                        errorDescription += "Error detected in Buzz system!" + vbCrLf
                    End If
                End If

                myCPUValueRow = InfoCpu(CPU_ELEMENTS.CPU_FWFM)
                If myCPUValueRow IsNot Nothing Then
                    myTempResult = myCPUValueRow.Value.ToString
                    If IsNumeric(myTempResult) Then
                        If CInt(myTempResult) > 0 Then
                            errorDetected = True
                            errorDescription += "Error detected in Firmware Repository Flash Memory!" + vbCrLf
                        End If
                    Else
                        errorDetected = True
                        errorDescription += "Error detected in Firmware Repository Flash Memory!" + vbCrLf
                    End If
                End If

                myCPUValueRow = InfoCpu(CPU_ELEMENTS.CPU_BBFM)
                If myCPUValueRow IsNot Nothing Then
                    myTempResult = myCPUValueRow.Value.ToString
                    If IsNumeric(myTempResult) Then
                        If CInt(myTempResult) > 0 Then
                            errorDetected = True
                            errorDescription += "Error detected in Black Box Flash Memory!" + vbCrLf
                        End If
                    Else
                        errorDetected = True
                        errorDescription += "Error detected in Black Box Flash Memory!" + vbCrLf
                    End If
                End If

                myCPUValueRow = InfoCpu(CPU_ELEMENTS.CPU_ISE)
                If myCPUValueRow IsNot Nothing Then
                    myTempResult = myCPUValueRow.Value.ToString
                    If IsNumeric(myTempResult) Then
                        If CInt(myTempResult) > 1 Then
                            errorDetected = True
                            errorDescription += "Error detected in ISE!" + vbCrLf
                            errorDescription += InfoCpu(CPU_ELEMENTS.CPU_ISE).Value.ToString + vbCrLf
                        End If
                    Else
                        errorDetected = True
                        errorDescription += "Error detected in ISE!" + vbCrLf
                        errorDescription += InfoCpu(CPU_ELEMENTS.CPU_ISE).Value.ToString + vbCrLf
                    End If
                End If


                myResultData.HasError = errorDetected
                myResultData.SetDatos = errorDescription

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerInfoDelegate.CheckCPU", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Validation Function of ARMS information received from the Instrument
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 08/06/2011</remarks>
        Public Function CheckARMS() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Dim myTempResult As String
                Dim errorDetected As Boolean = False
                Dim errorDescription As String = ""
                Dim myFirmwareValueRow As UIRefreshDS.FirmwareValueChangedRow
                Dim myArmValueRow As UIRefreshDS.ArmValueChangedRow

                myFirmwareValueRow = InfoFirmware(POLL_IDs.BM1)
                If myFirmwareValueRow IsNot Nothing Then
                    myTempResult = myFirmwareValueRow.BoardFirmwareCRCResult.ToString
                    If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                        errorDetected = True
                        errorDescription += "Error! Incorrect SAMPLE ARM CRC32" + vbCrLf
                    Else
                        myArmValueRow = InfoArm(POLL_IDs.BM1)
                        If myArmValueRow IsNot Nothing Then
                            myTempResult = myArmValueRow.MotorHorizontalHome.ToString
                            If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                                errorDetected = True
                                errorDescription += "SAMPLE Arm Horizontal HOME Error!" + vbCrLf
                            End If
                        End If

                        myArmValueRow = InfoArm(POLL_IDs.BM1)
                        If myArmValueRow IsNot Nothing Then
                            myTempResult = myArmValueRow.MotorVerticalHome.ToString
                            If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                                errorDetected = True
                                errorDescription += "SAMPLE Arm Vertical HOME Error!" + vbCrLf
                            End If
                        End If
                    End If
                End If

                myFirmwareValueRow = InfoFirmware(POLL_IDs.BR1)
                If myFirmwareValueRow IsNot Nothing Then
                    myTempResult = myFirmwareValueRow.BoardFirmwareCRCResult.ToString
                    If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                        errorDetected = True
                        errorDescription += "Error! Incorrect REAGENT1 ARM CRC32" + vbCrLf
                    Else
                        myArmValueRow = InfoArm(POLL_IDs.BR1)
                        If myArmValueRow IsNot Nothing Then
                            myTempResult = myArmValueRow.MotorHorizontalHome.ToString
                            If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                                errorDetected = True
                                errorDescription += "REAGENT1 Arm Horizontal HOME Error!" + vbCrLf
                            End If
                        End If

                        myArmValueRow = InfoArm(POLL_IDs.BR1)
                        If myArmValueRow IsNot Nothing Then
                            myTempResult = myArmValueRow.MotorVerticalHome.ToString
                            If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                                errorDetected = True
                                errorDescription += "REAGENT1 Arm Vertical HOME Error!" + vbCrLf
                            End If
                        End If
                    End If
                End If

                myFirmwareValueRow = InfoFirmware(POLL_IDs.BR2)
                If myFirmwareValueRow IsNot Nothing Then
                    myTempResult = myFirmwareValueRow.BoardFirmwareCRCResult.ToString
                    If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                        errorDetected = True
                        errorDescription += "Error! Incorrect REAGENT2 ARM CRC32" + vbCrLf
                    Else
                        myArmValueRow = InfoArm(POLL_IDs.BR2)
                        If myArmValueRow IsNot Nothing Then
                            myTempResult = myArmValueRow.MotorHorizontalHome.ToString
                            If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                                errorDetected = True
                                errorDescription += "REAGENT2 Arm Horizontal HOME Error!" + vbCrLf
                            End If
                        End If

                        myArmValueRow = InfoArm(POLL_IDs.BR2)
                        If myArmValueRow IsNot Nothing Then
                            myTempResult = myArmValueRow.MotorVerticalHome.ToString
                            If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                                errorDetected = True
                                errorDescription += "REAGENT2 Arm Vertical HOME Error!" + vbCrLf
                            End If
                        End If
                    End If
                End If

                myFirmwareValueRow = InfoFirmware(POLL_IDs.AG1)
                If myFirmwareValueRow IsNot Nothing Then
                    myTempResult = myFirmwareValueRow.BoardFirmwareCRCResult.ToString
                    If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                        errorDetected = True
                        errorDescription += "Error! Incorrect MIXER1 ARM CRC32" + vbCrLf
                    Else
                        myArmValueRow = InfoArm(POLL_IDs.AG1)
                        If myArmValueRow IsNot Nothing Then
                            myTempResult = myArmValueRow.MotorHorizontalHome.ToString
                            If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                                errorDetected = True
                                errorDescription += "MIXER1 Arm Horizontal HOME Error!" + vbCrLf
                            End If
                        End If

                        myArmValueRow = InfoArm(POLL_IDs.AG1)
                        If myArmValueRow IsNot Nothing Then
                            myTempResult = myArmValueRow.MotorVerticalHome.ToString
                            If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                                errorDetected = True
                                errorDescription += "MIXER1 Arm Vertical HOME Error!" + vbCrLf
                            End If
                        End If
                    End If
                End If

                myFirmwareValueRow = InfoFirmware(POLL_IDs.AG2)
                If myFirmwareValueRow IsNot Nothing Then
                    myTempResult = myFirmwareValueRow.BoardFirmwareCRCResult.ToString
                    If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                        errorDetected = True
                        errorDescription += "Error! Incorrect MIXER2 ARM CRC32" + vbCrLf
                    Else
                        myArmValueRow = InfoArm(POLL_IDs.AG2)
                        If myArmValueRow IsNot Nothing Then
                            myTempResult = myArmValueRow.MotorHorizontalHome.ToString
                            If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                                errorDetected = True
                                errorDescription += "MIXER2 Arm Horizontal HOME Error!" + vbCrLf
                            End If
                        End If

                        myArmValueRow = InfoArm(POLL_IDs.AG2)
                        If myArmValueRow IsNot Nothing Then
                            myTempResult = myArmValueRow.MotorVerticalHome.ToString
                            If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                                errorDetected = True
                                errorDescription += "MIXER2 Arm Vertical HOME Error!" + vbCrLf
                            End If
                        End If
                    End If
                End If

                myResultData.HasError = errorDetected
                myResultData.SetDatos = errorDescription

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerInfoDelegate.CheckARMS", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Validation Function of PROBES information received from the Instrument
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 08/06/2011</remarks>
        Public Function CheckPROBES() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Dim myTempResult As String
                Dim errorDetected As Boolean = False
                Dim errorDescription As String = ""
                Dim myFirmwareValueRow As UIRefreshDS.FirmwareValueChangedRow
                Dim myProbeValueRow As UIRefreshDS.ProbeValueChangedRow

                myFirmwareValueRow = InfoFirmware(POLL_IDs.DR1)
                If myFirmwareValueRow IsNot Nothing Then
                    myTempResult = myFirmwareValueRow.BoardFirmwareCRCResult.ToString
                    If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                        errorDetected = True
                        errorDescription += "Error! Incorrect REAGENT1 PROBE CRC32" + vbCrLf
                    Else
                        myProbeValueRow = InfoProbe(POLL_IDs.DR1)
                        If myProbeValueRow IsNot Nothing Then
                            myTempResult = myProbeValueRow.ThermistorDiagnostic.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "REAGENT1 Detection Error!" + vbCrLf
                                Else
                                    myProbeValueRow = InfoProbe(POLL_IDs.DR1)
                                    If myProbeValueRow IsNot Nothing Then
                                        myTempResult = myProbeValueRow.HeaterDiagnostic.ToString
                                        If IsNumeric(myTempResult) Then
                                            If CInt(myTempResult) > 0 Then
                                                errorDetected = True
                                                errorDescription += "REAGENT1 Detection Error!" + vbCrLf
                                            End If
                                        Else
                                            errorDetected = True
                                            errorDescription += "REAGENT1 Detection Error!" + vbCrLf
                                        End If
                                    End If
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "REAGENT1 Detection Error!" + vbCrLf
                            End If
                        End If

                    End If
                End If

                myFirmwareValueRow = InfoFirmware(POLL_IDs.DR2)
                If myFirmwareValueRow IsNot Nothing Then
                    myTempResult = myFirmwareValueRow.BoardFirmwareCRCResult.ToString
                    If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                        errorDetected = True
                        errorDescription += "Error! Incorrect REAGENT2 PROBE CRC32" + vbCrLf
                    Else
                        myProbeValueRow = InfoProbe(POLL_IDs.DR2)
                        If myProbeValueRow IsNot Nothing Then
                            myTempResult = myProbeValueRow.ThermistorDiagnostic.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "REAGENT2 Detection Error!" + vbCrLf
                                Else
                                    myProbeValueRow = InfoProbe(POLL_IDs.DR2)
                                    If myProbeValueRow IsNot Nothing Then
                                        myTempResult = myProbeValueRow.HeaterDiagnostic.ToString
                                        If IsNumeric(myTempResult) Then
                                            If CInt(myTempResult) > 0 Then
                                                errorDetected = True
                                                errorDescription += "REAGENT2 Detection Error!" + vbCrLf
                                            End If
                                        Else
                                            errorDetected = True
                                            errorDescription += "REAGENT2 Detection Error!" + vbCrLf
                                        End If
                                    End If
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "REAGENT2 Detection Error!" + vbCrLf
                            End If
                        End If

                    End If
                End If

                myFirmwareValueRow = InfoFirmware(POLL_IDs.DM1)
                If myFirmwareValueRow IsNot Nothing Then
                    myTempResult = myFirmwareValueRow.BoardFirmwareCRCResult.ToString
                    If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                        errorDetected = True
                        errorDescription += "Error! Incorrect SAMPLE PROBE CRC32" + vbCrLf
                    End If
                End If


                myResultData.HasError = errorDetected
                myResultData.SetDatos = errorDescription

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerInfoDelegate.CheckPROBES", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Validation Function of ROTORS information received from the Instrument
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 08/06/2011</remarks>
        Public Function CheckROTORS() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Dim myTempResult As String
                Dim errorDetected As Boolean = False
                Dim errorDescription As String = ""
                Dim myFirmwareValueRow As UIRefreshDS.FirmwareValueChangedRow
                Dim myRotorValueRow As UIRefreshDS.RotorValueChangedRow

                myFirmwareValueRow = InfoFirmware(POLL_IDs.RR1)
                If myFirmwareValueRow IsNot Nothing Then
                    myTempResult = myFirmwareValueRow.BoardFirmwareCRCResult.ToString
                    If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                        errorDetected = True
                        errorDescription += "Error! Incorrect REAGENTS ROTOR CRC32" + vbCrLf
                    Else
                        myRotorValueRow = InfoRotor(POLL_IDs.RR1)
                        If myRotorValueRow IsNot Nothing Then
                            myTempResult = myRotorValueRow.MotorHome.ToString
                            If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                                errorDetected = True
                                errorDescription += "REAGENTS Rotor Error Home!" + vbCrLf
                            End If
                        End If

                        myRotorValueRow = InfoRotor(POLL_IDs.RR1)
                        If myRotorValueRow IsNot Nothing Then
                            myTempResult = myRotorValueRow.ThermistorFridgeDiagnostic.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "REAGENTS ROTOR Fridge Thermistor Diagnostic Error!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "REAGENTS ROTOR Fridge Thermistor Diagnostic Error!" + vbCrLf
                            End If
                        End If

                        myRotorValueRow = InfoRotor(POLL_IDs.RR1)
                        If myRotorValueRow IsNot Nothing Then
                            myTempResult = myRotorValueRow.PeltiersFridgeDiagnostic.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "REAGENTS ROTOR Fridge Peltiers Diagnostic Error!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "REAGENTS ROTOR Fridge Peltiers Diagnostic Error!" + vbCrLf
                            End If
                        End If

                        myRotorValueRow = InfoRotor(POLL_IDs.RR1)
                        If myRotorValueRow IsNot Nothing Then
                            myTempResult = myRotorValueRow.PeltiersFan1Diagnostic.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "REAGENTS ROTOR Fan1 Peltier Diagnostic Error!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "REAGENTS ROTOR Fan1 Peltier Diagnostic Error!" + vbCrLf
                            End If
                        End If

                        myRotorValueRow = InfoRotor(POLL_IDs.RR1)
                        If myRotorValueRow IsNot Nothing Then
                            myTempResult = myRotorValueRow.PeltiersFan2Diagnostic.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "REAGENTS ROTOR Fan2 Peltier Diagnostic Error!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "REAGENTS ROTOR Fan2 Peltier Diagnostic Error!" + vbCrLf
                            End If
                        End If

                        myRotorValueRow = InfoRotor(POLL_IDs.RR1)
                        If myRotorValueRow IsNot Nothing Then
                            myTempResult = myRotorValueRow.PeltiersFan3Diagnostic.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "REAGENTS ROTOR Fan3 Peltier Diagnostic Error!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "REAGENTS ROTOR Fan3 Peltier Diagnostic Error!" + vbCrLf
                            End If
                        End If

                        myRotorValueRow = InfoRotor(POLL_IDs.RR1)
                        If myRotorValueRow IsNot Nothing Then
                            myTempResult = myRotorValueRow.PeltiersFan4Diagnostic.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "REAGENTS ROTOR Fan4 Peltier Diagnostic Error!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "REAGENTS ROTOR Fan4 Peltier Diagnostic Error!" + vbCrLf
                            End If
                        End If

                        myRotorValueRow = InfoRotor(POLL_IDs.RR1)
                        If myRotorValueRow IsNot Nothing Then
                            myTempResult = myRotorValueRow.FrameFan1Diagnostic.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "REAGENTS ROTOR Fan1 Frame Diagnostic Error!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "REAGENTS ROTOR Fan1 Frame Diagnostic Error!" + vbCrLf
                            End If
                        End If

                        myRotorValueRow = InfoRotor(POLL_IDs.RR1)
                        If myRotorValueRow IsNot Nothing Then
                            myTempResult = myRotorValueRow.FrameFan2Diagnostic.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "REAGENTS ROTOR Fan2 Frame Diagnostic Error!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "REAGENTS ROTOR Fan2 Frame Diagnostic Error!" + vbCrLf
                            End If
                        End If

                        myRotorValueRow = InfoRotor(POLL_IDs.RR1)
                        If myRotorValueRow IsNot Nothing Then
                            myTempResult = myRotorValueRow.BarCodeStatus.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "REAGENTS ROTOR BarCode Diagnostic Error!" + vbCrLf
                                    errorDescription += InfoRotor(POLL_IDs.RR1).BarcodeError.ToString + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "REAGENTS ROTOR BarCode Diagnostic Error!" + vbCrLf
                            End If
                        End If
                    End If
                End If

                myFirmwareValueRow = InfoFirmware(POLL_IDs.RM1)
                If myFirmwareValueRow IsNot Nothing Then
                    myTempResult = myFirmwareValueRow.BoardFirmwareCRCResult.ToString
                    If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                        errorDetected = True
                        errorDescription += "Error! Incorrect SAMPLES ROTOR CRC32" + vbCrLf
                    Else
                        myRotorValueRow = InfoRotor(POLL_IDs.RM1)
                        If myRotorValueRow IsNot Nothing Then
                            myTempResult = myRotorValueRow.MotorHome.ToString
                            If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                                errorDetected = True
                                errorDescription += "SAMPLES Rotor Error Home!" + vbCrLf
                            End If
                        End If

                        myRotorValueRow = InfoRotor(POLL_IDs.RM1)
                        If myRotorValueRow IsNot Nothing Then
                            myTempResult = myRotorValueRow.BarCodeStatus.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "SAMPLES Rotor BarCode Diagnostic Error!" + vbCrLf
                                    errorDescription += InfoRotor(POLL_IDs.RR1).BarcodeError.ToString + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "SAMPLES Rotor BarCode Diagnostic Error!" + vbCrLf
                            End If
                        End If
                    End If
                End If


                myResultData.HasError = errorDetected
                myResultData.SetDatos = errorDescription

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerInfoDelegate.CheckROTORS", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Validation Function of PHOTOMETRIC information received from the Instrument
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 08/06/2011</remarks>
        Public Function CheckPHOTOMETRICS() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Dim myTempResult As String
                Dim errorDetected As Boolean = False
                Dim errorDescription As String = ""
                Dim myFirmwareValueRow As UIRefreshDS.FirmwareValueChangedRow
                Dim myPhotometricsValueRow As UIRefreshDS.PhotometricsValueChangedRow

                myFirmwareValueRow = InfoFirmware(POLL_IDs.GLF)
                If myFirmwareValueRow IsNot Nothing Then
                    myTempResult = myFirmwareValueRow.BoardFirmwareCRCResult.ToString
                    If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                        errorDetected = True
                        errorDescription += "Error! Incorrect PHOTOMETRIC CRC32" + vbCrLf
                    Else
                        myPhotometricsValueRow = InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MRH)
                        If myPhotometricsValueRow IsNot Nothing Then
                            myTempResult = myPhotometricsValueRow.Value.ToString
                            If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                                errorDetected = True
                                errorDescription += "REACTIONS Rotor Error Home!" + vbCrLf
                            End If
                        End If

                        myPhotometricsValueRow = InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MRED)
                        If myPhotometricsValueRow IsNot Nothing Then
                            myTempResult = myPhotometricsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "REACTIONS Rotor Motor Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "REACTIONS Rotor Motor Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myPhotometricsValueRow = InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MWH)
                        If myPhotometricsValueRow IsNot Nothing Then
                            myTempResult = myPhotometricsValueRow.Value.ToString
                            If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                                errorDetected = True
                                errorDescription += "WASHING STATION Error Home!" + vbCrLf
                            End If
                        End If

                        myPhotometricsValueRow = InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PTHD)
                        If myPhotometricsValueRow IsNot Nothing Then
                            myTempResult = myPhotometricsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "REACTIONS Rotor Thermistor Diagnostic Error!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "REACTIONS Rotor Thermistor Diagnostic Error!" + vbCrLf
                            End If
                        End If

                        myPhotometricsValueRow = InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PHD)
                        If myPhotometricsValueRow IsNot Nothing Then
                            myTempResult = myPhotometricsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "REACTIONS Rotor Peltier Diagnostic Error!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "REACTIONS Rotor Peltier Diagnostic Error!" + vbCrLf
                            End If
                        End If

                        myPhotometricsValueRow = InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF1D)
                        If myPhotometricsValueRow IsNot Nothing Then
                            myTempResult = myPhotometricsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "Fan1 Peltier Diagnostic Error!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "Fan1 Peltier Diagnostic Error!" + vbCrLf
                            End If
                        End If

                        myPhotometricsValueRow = InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF2D)
                        If myPhotometricsValueRow IsNot Nothing Then
                            myTempResult = myPhotometricsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "Fan2 Peltier Diagnostic Error!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "Fan2 Peltier Diagnostic Error!" + vbCrLf
                            End If
                        End If

                        myPhotometricsValueRow = InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF3D)
                        If myPhotometricsValueRow IsNot Nothing Then
                            myTempResult = myPhotometricsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "Fan3 Peltier Diagnostic Error!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "Fan3 Peltier Diagnostic Error!" + vbCrLf
                            End If
                        End If

                        myPhotometricsValueRow = InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF4D)
                        If myPhotometricsValueRow IsNot Nothing Then
                            myTempResult = myPhotometricsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "Fan4 Peltier Diagnostic Error!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "Fan4 Peltier Diagnostic Error!" + vbCrLf
                            End If
                        End If

                        myPhotometricsValueRow = InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PHT)
                        If myPhotometricsValueRow IsNot Nothing Then
                            myTempResult = myPhotometricsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "PHOTOMETRY STATUS Error!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "PHOTOMETRY STATUS Error!" + vbCrLf
                            End If
                        End If

                        myPhotometricsValueRow = InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PHFM)
                        If myPhotometricsValueRow IsNot Nothing Then
                            myTempResult = myPhotometricsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "PHOTOMETRY Flash Memory Error!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "PHOTOMETRY Flash Memory Error!" + vbCrLf
                            End If
                        End If

                    End If
                End If


                myResultData.HasError = errorDetected
                myResultData.SetDatos = errorDescription

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerInfoDelegate.CheckPHOTOMETRICS", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Validation Function of MANIFOLD information received from the Instrument
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 08/06/2011</remarks>
        Public Function CheckMANIFOLD() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Dim myTempResult As String
                Dim errorDetected As Boolean = False
                Dim errorDescription As String = ""
                Dim myFirmwareValueRow As UIRefreshDS.FirmwareValueChangedRow
                Dim myManifoldValueRow As UIRefreshDS.ManifoldValueChangedRow

                myFirmwareValueRow = InfoFirmware(POLL_IDs.JE1)
                If myFirmwareValueRow IsNot Nothing Then
                    myTempResult = myFirmwareValueRow.BoardFirmwareCRCResult.ToString
                    If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                        errorDetected = True
                        errorDescription += "Error! Incorrect MANIFOLD CRC32" + vbCrLf
                    Else
                        myManifoldValueRow = InfoManifold(MANIFOLD_ELEMENTS.JE1_MSH)
                        If myManifoldValueRow IsNot Nothing Then
                            myTempResult = myManifoldValueRow.Value.ToString
                            If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                                errorDetected = True
                                errorDescription += "SAMPLE MOTOR Error Home!" + vbCrLf
                            End If
                        End If

                        myManifoldValueRow = InfoManifold(MANIFOLD_ELEMENTS.JE1_MR1H)
                        If myManifoldValueRow IsNot Nothing Then
                            myTempResult = myManifoldValueRow.Value.ToString
                            If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                                errorDetected = True
                                errorDescription += "REAGENT1 MOTOR Error Home!" + vbCrLf
                            End If
                        End If

                        myManifoldValueRow = InfoManifold(MANIFOLD_ELEMENTS.JE1_MR2H)
                        If myManifoldValueRow IsNot Nothing Then
                            myTempResult = myManifoldValueRow.Value.ToString
                            If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                                errorDetected = True
                                errorDescription += "REAGENT2 MOTOR Error Home!" + vbCrLf
                            End If
                        End If

                        myManifoldValueRow = InfoManifold(MANIFOLD_ELEMENTS.JE1_B1D)
                        If myManifoldValueRow IsNot Nothing Then
                            myTempResult = myManifoldValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "SAMPLES DOSING PUMP Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "SAMPLES DOSING PUMP Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myManifoldValueRow = InfoManifold(MANIFOLD_ELEMENTS.JE1_B2D)
                        If myManifoldValueRow IsNot Nothing Then
                            myTempResult = myManifoldValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "REAGENT1 DOSING PUMP Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "REAGENT1 DOSING PUMP Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myManifoldValueRow = InfoManifold(MANIFOLD_ELEMENTS.JE1_B3D)
                        If myManifoldValueRow IsNot Nothing Then
                            myTempResult = myManifoldValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "REAGENT2 DOSING PUMP Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "REAGENT2 DOSING PUMP Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myManifoldValueRow = InfoManifold(MANIFOLD_ELEMENTS.JE1_EV1D)
                        If myManifoldValueRow IsNot Nothing Then
                            myTempResult = myManifoldValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "SAMPLES DOSING VALV Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "SAMPLES DOSING VALV Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myManifoldValueRow = InfoManifold(MANIFOLD_ELEMENTS.JE1_EV2D)
                        If myManifoldValueRow IsNot Nothing Then
                            myTempResult = myManifoldValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "REAGENT1 DOSING VALV Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "REAGENT1 DOSING VALV Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myManifoldValueRow = InfoManifold(MANIFOLD_ELEMENTS.JE1_EV3D)
                        If myManifoldValueRow IsNot Nothing Then
                            myTempResult = myManifoldValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "REAGENT2 DOSING VALV Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "REAGENT2 DOSING VALV Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myManifoldValueRow = InfoManifold(MANIFOLD_ELEMENTS.JE1_EV4D)
                        If myManifoldValueRow IsNot Nothing Then
                            myTempResult = myManifoldValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "Air/Washing Solution VALV Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "Air/Washing Solution VALV Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myManifoldValueRow = InfoManifold(MANIFOLD_ELEMENTS.JE1_EV5D)
                        If myManifoldValueRow IsNot Nothing Then
                            myTempResult = myManifoldValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "Air-Washing Solution/Purified Water VALV Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "Air-Washing Solution/Purified Water VALV Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                    End If
                End If


                myResultData.HasError = errorDetected
                myResultData.SetDatos = errorDescription

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerInfoDelegate.CheckMANIFOLD", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Validation Function of FLUIDICS information received from the Instrument
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 08/06/2011</remarks>
        Public Function CheckFLUIDICS() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Dim myTempResult As String
                Dim errorDetected As Boolean = False
                Dim errorDescription As String = ""
                Dim myFirmwareValueRow As UIRefreshDS.FirmwareValueChangedRow
                Dim myFluidicsValueRow As UIRefreshDS.FluidicsValueChangedRow

                myFirmwareValueRow = InfoFirmware(POLL_IDs.SF1)
                If myFirmwareValueRow IsNot Nothing Then
                    myTempResult = myFirmwareValueRow.BoardFirmwareCRCResult.ToString
                    If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                        errorDetected = True
                        errorDescription += "Error! Incorrect FLUIDICS CRC32" + vbCrLf
                    Else
                        myFluidicsValueRow = InfoFluidics(FLUIDICS_ELEMENTS.SF1_MSH)
                        If myFluidicsValueRow IsNot Nothing Then
                            myTempResult = myFluidicsValueRow.Value.ToString
                            If myTempResult = HW_GENERIC_DIAGNOSIS.KO.ToString Then
                                errorDetected = True
                                errorDescription += "WASHING STATION MOTOR Error Home!" + vbCrLf
                            End If
                        End If

                        myFluidicsValueRow = InfoFluidics(FLUIDICS_ELEMENTS.SF1_B1D)
                        If myFluidicsValueRow IsNot Nothing Then
                            myTempResult = myFluidicsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "SAMPLES NEEDLE External Washing PUMP Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "SAMPLES NEEDLE External Washing Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myFluidicsValueRow = InfoFluidics(FLUIDICS_ELEMENTS.SF1_B2D)
                        If myFluidicsValueRow IsNot Nothing Then
                            myTempResult = myFluidicsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "REAGENT1/MIXER2 NEEDLE External Washing PUMP Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "REAGENT1/MIXER2 NEEDLE External Washing PUMP Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myFluidicsValueRow = InfoFluidics(FLUIDICS_ELEMENTS.SF1_B3D)
                        If myFluidicsValueRow IsNot Nothing Then
                            myTempResult = myFluidicsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "REAGENT2/MIXER1 NEEDLE External Washing PUMP Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "REAGENT2/MIXER1 NEEDLE External Washing PUMP Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myFluidicsValueRow = InfoFluidics(FLUIDICS_ELEMENTS.SF1_B4D)
                        If myFluidicsValueRow IsNot Nothing Then
                            myTempResult = myFluidicsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "PURIFIED WATER INPUT PUMP Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "PURIFIED WATER INPUT PUMP Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myFluidicsValueRow = InfoFluidics(FLUIDICS_ELEMENTS.SF1_B5D)
                        If myFluidicsValueRow IsNot Nothing Then
                            myTempResult = myFluidicsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "LOW CONTAMINATION OUTPUT PUMP Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "LOW CONTAMINATION OUTPUT Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myFluidicsValueRow = InfoFluidics(FLUIDICS_ELEMENTS.SF1_B6D)
                        If myFluidicsValueRow IsNot Nothing Then
                            myTempResult = myFluidicsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "Washing Station NEEDLES 2,3 Aspiration PUMP Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "Washing Station NEEDLES 2,3 Aspiration PUMP Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myFluidicsValueRow = InfoFluidics(FLUIDICS_ELEMENTS.SF1_B7D)
                        If myFluidicsValueRow IsNot Nothing Then
                            myTempResult = myFluidicsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "Washing Station NEEDLES 4,5 Aspiration PUMP Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "Washing Station NEEDLES 4,5 Aspiration PUMP Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myFluidicsValueRow = InfoFluidics(FLUIDICS_ELEMENTS.SF1_B8D)
                        If myFluidicsValueRow IsNot Nothing Then
                            myTempResult = myFluidicsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "Washing Station NEEDLE 6 Aspiration PUMP Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "Washing Station NEEDLE 6 Aspiration PUMP Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myFluidicsValueRow = InfoFluidics(FLUIDICS_ELEMENTS.SF1_B9D)
                        If myFluidicsValueRow IsNot Nothing Then
                            myTempResult = myFluidicsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "Washing Station NEEDLE 7 Aspiration PUMP Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "Washing Station NEEDLE 7 Aspiration PUMP Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myFluidicsValueRow = InfoFluidics(FLUIDICS_ELEMENTS.SF1_B10D)
                        If myFluidicsValueRow IsNot Nothing Then
                            myTempResult = myFluidicsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "Washing Station NEEDLE 1 Aspiration PUMP Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "Washing Station NEEDLE 1 Aspiration PUMP Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myFluidicsValueRow = InfoFluidics(FLUIDICS_ELEMENTS.SF1_GE1D)
                        If myFluidicsValueRow IsNot Nothing Then
                            myTempResult = myFluidicsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "DISPENSATION VALVES GROUP Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "DISPENSATION VALVES GROUP Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myFluidicsValueRow = InfoFluidics(FLUIDICS_ELEMENTS.SF1_EV1D)
                        If myFluidicsValueRow IsNot Nothing Then
                            myTempResult = myFluidicsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "PURIFIED WATER INPUT (source) VALV Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "PURIFIED WATER INPUT (source) VALV Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myFluidicsValueRow = InfoFluidics(FLUIDICS_ELEMENTS.SF1_EV2D)
                        If myFluidicsValueRow IsNot Nothing Then
                            myTempResult = myFluidicsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "PURIFIED WATER INPUT (tank) VALV Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "PURIFIED WATER INPUT (tank) VALV Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myFluidicsValueRow = InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSTHD)
                        If myFluidicsValueRow IsNot Nothing Then
                            myTempResult = myFluidicsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "WASHING STATION Thermistor Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "WASHING STATION Thermistor Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myFluidicsValueRow = InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSHD)
                        If myFluidicsValueRow IsNot Nothing Then
                            myTempResult = myFluidicsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "WASHING STATION Heater Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "WASHING STATION Heater Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myFluidicsValueRow = InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSWD)
                        If myFluidicsValueRow IsNot Nothing Then
                            myTempResult = myFluidicsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "WASHING SOLUTION WEIGHT Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "WASHING SOLUTION WEIGHT Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                        myFluidicsValueRow = InfoFluidics(FLUIDICS_ELEMENTS.SF1_HCWD)
                        If myFluidicsValueRow IsNot Nothing Then
                            myTempResult = myFluidicsValueRow.Value.ToString
                            If IsNumeric(myTempResult) Then
                                If CInt(myTempResult) > 0 Then
                                    errorDetected = True
                                    errorDescription += "HIGH CONTAMINATION WEIGHT Diagnostic Fail!" + vbCrLf
                                End If
                            Else
                                errorDetected = True
                                errorDescription += "HIGH CONTAMINATION WEIGHT Diagnostic Fail!" + vbCrLf
                            End If
                        End If

                    End If
                End If


                myResultData.HasError = errorDetected
                myResultData.SetDatos = errorDescription

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerInfoDelegate.CheckFLUIDICS", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

#End Region

#Region "Private Methods"

        '''' <summary>
        '''' Validates compatibility between Software and Firmware
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Private Function ValidateFwSwCompatibility() As Boolean
        '    Dim returnValue As Boolean
        '    Try

        '        ' PENDING : is necesary the compatibility check
        '        ' to implement when version package table's design has been created !

        '        ' to test : 
        '        returnValue = True

        '    Catch ex As Exception
        '        returnValue = False
        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "AnalyzerInfoDelegate.ValidateFwSwCompatibility", EventLogEntryType.Error, False)
        '    End Try
        '    Return returnValue
        'End Function
#End Region

    
    End Class

End Namespace
