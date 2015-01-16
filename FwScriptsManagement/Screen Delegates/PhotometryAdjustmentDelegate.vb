Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Calculations
Imports System.Math

Namespace Biosystems.Ax00.FwScriptsManagement

    Public Class PhotometryAdjustmentDelegate
        Inherits BaseFwScriptDelegate

#Region "Constructor"
        Public Sub New(ByVal pAnalyzerID As String, ByVal pFwScriptsDelegate As SendFwScriptsDelegate)
            MyBase.New(pAnalyzerID)
            myFwScriptDelegate = pFwScriptsDelegate
            MyClass.CurrentTestAttr = ADJUSTMENT_GROUPS._NONE
            MyClass.PreviousTestAttr = ADJUSTMENT_GROUPS._NONE
            MyClass.CurrentOperation = OPERATIONS._NONE
            MyClass.ReportCountTimeout = 0
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub
#End Region

#Region "Enumerations"
        'Private Enum ERROR_TYPES
        '    OK
        '    NO_OK
        'End Enum

        'Private Enum WL_STATUS
        '    OK
        '    WARNING1    ' WL with warning for Num Counts > BLMax
        '    WARNING2    ' WL with warning for Num Counts < WarningBLMin
        '    ALARM1      ' WL with alarm for Num Counts > BLMax
        '    ALARM2      ' WL with alarm for Num Counts < BLMin
        'End Enum

        Private Enum OPERATIONS
            _NONE
            HOMES
            READ_COUNTS
            REAGENTS_HOME_ROTOR
            SAVE_ADJUSMENTS
            ' XBC 20/02/2012
            WASHING_STATION_UP
            WASHING_STATION_DOWN
        End Enum
#End Region

#Region "Structures"
        Private Structure CommonData
            Dim MaxWells As Integer
            Dim MaxWaveLengths As Integer
            'Dim WellsToReadBL As Single
            Dim MaxStability As Single
            Dim MaxRepeatability As Single
        End Structure

        Private Structure CommonResults
            Dim WaveLength As Integer
            Dim Absorbance As Single
            Dim CountsPhMainDK As Single
            Dim CountsPhRefDK As Single
            Dim STDDeviationPhMainDK As Single
            Dim STDDeviationPhRefDK As Single
            Dim STDDeviationAbsorbance As Single
            Dim CVAbsorbance As Single
        End Structure

        Private Structure CommonReadedCounts
            Dim LedPosition As Integer
            Dim MeasuresRepeatabilityphMainCounts As List(Of Single)
            Dim MeasuresRepeatabilityphRefCounts As List(Of Single)
            Dim MeasuresRepeatabilityphMainCountsDK As List(Of Single)
            Dim MeasuresRepeatabilityphRefCountsDK As List(Of Single)
            Dim MeasuresRepeatabilityAbsorbances As List(Of Single)
            Dim MeasuresStabilityphMainCounts As List(Of Single)
            Dim MeasuresStabilityphRefCounts As List(Of Single)
            Dim MeasuresStabilityphMainCountsDK As List(Of Single)
            Dim MeasuresStabilityphRefCountsDK As List(Of Single)
            Dim MeasuresStabilityAbsorbances As List(Of Single)
            Dim MeasureABSphMainCount As Single
            Dim MeasureABSphRefCount As Single
        End Structure
#End Region

#Region "Declarations"

        'Private myAnalyzerLedPosDS As New AnalyzerLedPositionsDS
        Dim qLeds As List(Of AnalyzerLedPositionsDS.tcfgAnalyzerLedPositionsRow)
        Private CommonParameters As CommonData
        Private CurrentOperation As OPERATIONS                  ' controls the operation which is currently executed in each time
        Private ResultsRepeatabilityTest() As CommonResults     ' Last Repeatability test results done
        Private ResultsStabilityTest() As CommonResults         ' Last Stability test results done
        Private ResultsABSTest() As CommonResults               ' Last Absorbance Measurement test results done
        'Private ResultsCheckRotorTest As New CommonResults     ' Last Verification Rotor test results done
        Private EmptyWell As List(Of Boolean)                   ' empty well (True) , fill well (False)
        Private PathLength As Single
        Private LimitAbs As Single
        Private DilutionFactor As Single
        Private ReadingTime As Single                           ' Time is left between the microInstructtion and the next microInstructtion to read counts
        Private ReadingStabilityOffsetTime As Single            ' Time additional for Stability test
        Private MaxCvToDisplay As Single                        ' Max value allowed to show Coefficient Variation
        Private MaxLEDsWarningsAttr As Single                   ' Max number warnings for display a warning message
        Private RecommendationsReport() As HISTORY_RECOMMENDATIONS
        Private ReportCountTimeout As Integer
        Private myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator 'RH 15/02/2012 Get the static version
        Private myGroupSeparator As String = SystemInfoManager.OSGroupSeparator 'RH 15/02/2012 Get the static version

        ' Simulation mode
        Private PositionLEDsxSimulation As List(Of Integer)
#End Region

#Region "Attributes"
        Private CurrentTestAttr As ADJUSTMENT_GROUPS    ' Test currently executed
        Private PreviousTestAttr As ADJUSTMENT_GROUPS   ' Test previously executed

        Private pValueAdjustAttr As String

        Private WellToUseAttr As Integer    ' WellToUse to use in the current test
        Private FillModeAttr As FILL_MODE ' indicates if the system must be filled automatically or by user

        Private TestBaseLineDoneAttr As Boolean
        'Private TestDarknessCountsDoneAttr As Boolean   ' no need because is tested at the same time with Baseline test

        Private TestRepeatabilityDoneAttr As Boolean
        Private TestStabilityDoneAttr As Boolean
        Private TestABSDoneAttr As Boolean
        'Private TestCheckRotorDoneAttr As Boolean
        Private ITsEditionDoneAttr As Boolean
        Private ITsEditionFirstPartDoneAttr As Boolean
        Private HomesDoingAttr As Boolean
        Private HomesDoneAttr As Boolean
        Private ReadLedsDoneAttr As Boolean
        Private SaveLedsIntensitiesDoneAttr As Boolean

        Private BaseLineMainCountsAttr As List(Of Single)
        Private BaseLineRefCountsAttr As List(Of Single)
        Private DarkphMainCountAttr As Single
        Private DarkphRefCountAttr As Single

        Private RepeatabilityReadingsAttr As Integer
        Private StabilityReadingsAttr As Integer
        'Private CheckRotorReadingsAttr As Integer

        Private TestReadedCountsAttr() As CommonReadedCounts

        'Private MeasuresCheckRotorCountsAttr As List(Of Single)
        'Private MeasuresCheckRotorAbsorbancesAttr As List(Of Single)

        Private CurrentLEDsIntensitiesAttr As List(Of Single)
        Private IntegrationTimesAttr As List(Of Single)
        Private PositionLEDsAttr As List(Of Single)

        Private NoneInstructionToSendAttr As Boolean

        Private AnalyzerModelAttr As String 'SGM 25/04/2011
        'Private AnalyzerIDAttr As String

        Private CurrentTimeOperationAttr As Integer                 ' Time expected for current operation

        Private LimitMinLEDsAttr As Long
        Private LimitMaxLEDsAttr As Long
        Private LimitMinPhMainAttr As Long
        Private LimitMaxPhMainAttr As Long
        Private LimitMinPhRefAttr As Long
        Private LimitMaxPhRefAttr As Long

        ' XBC 20/02/2012
        Private IsWashingStationUpAttr As Boolean

        ' XBC 01/03/2012
        Private MaxAbsToDisplayAttr As Single                        ' Max value allowed to show Absorbance calculation

#End Region

#Region "Properties"

#Region "GENERAL"
        Public Property WellToUse() As Integer
            Get
                Return WellToUseAttr
            End Get
            Set(ByVal value As Integer)
                WellToUseAttr = value
            End Set
        End Property

        Public ReadOnly Property GetEmptyWell(ByVal pWell As Integer) As Boolean
            Get
                Return MyClass.EmptyWell(pWell)
            End Get
        End Property

        Public Property FillMode() As FILL_MODE
            Get
                Return FillModeAttr
            End Get
            Set(ByVal value As FILL_MODE)
                FillModeAttr = value
            End Set
        End Property

        Public Property TestBaseLineDone() As Boolean
            Get
                Return TestBaseLineDoneAttr
            End Get
            Set(ByVal value As Boolean)
                TestBaseLineDoneAttr = value
            End Set
        End Property

        Public ReadOnly Property TestReadedCountsNum() As Integer
            Get
                Return TestReadedCountsAttr.Count
            End Get
        End Property

        Public Property CurrentLEDsIntensities() As List(Of Single)
            Get
                Return CurrentLEDsIntensitiesAttr
            End Get
            Set(ByVal value As List(Of Single))
                CurrentLEDsIntensitiesAttr = value
            End Set
        End Property

        ' XBC 29/02/2012
        Public Property IntegrationTimes() As List(Of Single)
            Get
                Return IntegrationTimesAttr
            End Get
            Set(ByVal value As List(Of Single))
                IntegrationTimesAttr = value
            End Set
        End Property

        Public Property PositionLEDs() As List(Of Single)
            Get
                Return PositionLEDsAttr
            End Get
            Set(ByVal value As List(Of Single))
                PositionLEDsAttr = value
            End Set
        End Property
        ' XBC 29/02/2012

        Public Property SaveLedsIntensitiesDone() As Boolean
            Get
                Return SaveLedsIntensitiesDoneAttr
            End Get
            Set(ByVal value As Boolean)
                SaveLedsIntensitiesDoneAttr = value
            End Set
        End Property

        Public ReadOnly Property MaxWells() As Integer
            Get
                Return CommonParameters.MaxWells
            End Get
        End Property

        Public ReadOnly Property MaxWaveLengths() As Integer
            Get
                Return CommonParameters.MaxWaveLengths
            End Get
        End Property

        Public Property HomesDoing() As Boolean
            Get
                Return HomesDoingAttr
            End Get
            Set(ByVal value As Boolean)
                HomesDoingAttr = value
            End Set
        End Property

        Public Property HomesDone() As Boolean
            Get
                Return HomesDoneAttr
            End Get
            Set(ByVal value As Boolean)
                HomesDoneAttr = value
            End Set
        End Property

        Public Property CurrentTest() As ADJUSTMENT_GROUPS
            Get
                Return CurrentTestAttr
            End Get
            Set(ByVal value As ADJUSTMENT_GROUPS)
                CurrentTestAttr = value
            End Set
        End Property

        Public Property TestRepeatabilityDone() As Boolean
            Get
                Return TestRepeatabilityDoneAttr
            End Get
            Set(ByVal value As Boolean)
                TestRepeatabilityDoneAttr = value
            End Set
        End Property

        Public Property TestStabilityDone() As Boolean
            Get
                Return TestStabilityDoneAttr
            End Get
            Set(ByVal value As Boolean)
                TestStabilityDoneAttr = value
            End Set
        End Property

        Public Property TestABSDone() As Boolean
            Get
                Return TestABSDoneAttr
            End Get
            Set(ByVal value As Boolean)
                TestABSDoneAttr = value
            End Set
        End Property

        Public Property ITsEditionDone() As Boolean
            Get
                Return ITsEditionDoneAttr
            End Get
            Set(ByVal value As Boolean)
                ITsEditionDoneAttr = value
            End Set
        End Property

        Public Property ITsEditionFirstPartDone() As Boolean
            Get
                Return ITsEditionFirstPartDoneAttr
            End Get
            Set(ByVal value As Boolean)
                ITsEditionFirstPartDoneAttr = value
            End Set
        End Property

        Public ReadOnly Property MaxLEDsWarnings() As Single
            Get
                Return MaxLEDsWarningsAttr
            End Get
        End Property

        Public Property NoneInstructionToSend() As Boolean
            Get
                Return NoneInstructionToSendAttr
            End Get
            Set(ByVal value As Boolean)
                NoneInstructionToSendAttr = value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>SGM 25/04/2011</remarks>
        Public Property AnalyzerModel() As String
            Get
                Return AnalyzerModelAttr
            End Get
            Set(ByVal value As String)
                AnalyzerModelAttr = value
            End Set
        End Property

        Public ReadOnly Property CurrentTimeOperation() As Integer
            Get
                Return CurrentTimeOperationAttr
            End Get
        End Property

        Public Property pValueAdjust() As String
            Get
                Return MyClass.pValueAdjustAttr
            End Get
            Set(ByVal value As String)
                MyClass.pValueAdjustAttr = value
            End Set
        End Property

        Public ReadOnly Property LimitMinLEDs() As Long
            Get
                Return MyClass.LimitMinLEDsAttr
            End Get
        End Property

        Public ReadOnly Property LimitMaxLEDs() As Long
            Get
                Return MyClass.LimitMaxLEDsAttr
            End Get
        End Property

        Public ReadOnly Property LimitMinPhMain() As Long
            Get
                Return MyClass.LimitMinPhMainAttr
            End Get
        End Property

        Public ReadOnly Property LimitMaxPhMain() As Long
            Get
                Return MyClass.LimitMaxPhMainAttr
            End Get
        End Property

        Public ReadOnly Property LimitMinPhRef() As Long
            Get
                Return MyClass.LimitMinPhRefAttr
            End Get
        End Property

        Public ReadOnly Property LimitMaxPhRef() As Long
            Get
                Return MyClass.LimitMaxPhRefAttr
            End Get
        End Property

        Public Property IsWashingStationUp() As Boolean
            Get
                Return MyClass.IsWashingStationUpAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.IsWashingStationUpAttr = value
            End Set
        End Property

#End Region

#Region "BASELINE"
        Public Property BaseLineMainCounts() As List(Of Single)
            Get
                Return BaseLineMainCountsAttr
            End Get
            Set(ByVal value As List(Of Single))
                BaseLineMainCountsAttr = value
            End Set
        End Property

        Public Property BaseLineRefCounts() As List(Of Single)
            Get
                Return BaseLineRefCountsAttr
            End Get
            Set(ByVal value As List(Of Single))
                BaseLineRefCountsAttr = value
            End Set
        End Property

#End Region

#Region "DARKNESS"
        Public Property DarkphMainCount() As Single
            Get
                Return DarkphMainCountAttr
            End Get
            Set(ByVal value As Single)
                DarkphMainCountAttr = value
            End Set
        End Property

        Public Property DarkphRefCount() As Single
            Get
                Return DarkphRefCountAttr
            End Get
            Set(ByVal value As Single)
                DarkphRefCountAttr = value
            End Set
        End Property

#End Region

#Region "REPEATABILITY"
        Public Property RepeatabilityReadings() As Integer
            Get
                Return RepeatabilityReadingsAttr
            End Get
            Set(ByVal value As Integer)
                RepeatabilityReadingsAttr = value
            End Set
        End Property

        Public Property MeasuresRepeatabilityphMainCountsByLed(ByVal LedPosition As Integer) As List(Of Single)
            Get
                Return TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphMainCounts
            End Get
            Set(ByVal value As List(Of Single))
                TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphMainCounts = value
            End Set
        End Property

        Public Property MeasuresRepeatabilityphRefCountsByLed(ByVal LedPosition As Integer) As List(Of Single)
            Get
                Return TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphRefCounts
            End Get
            Set(ByVal value As List(Of Single))
                TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphRefCounts = value
            End Set
        End Property

        Public Property MeasuresRepeatabilityphMainCountsDKByLed(ByVal LedPosition As Integer) As List(Of Single)
            Get
                Return TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphMainCountsDK
            End Get
            Set(ByVal value As List(Of Single))
                TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphMainCountsDK = value
            End Set
        End Property

        Public Property MeasuresRepeatabilityphRefCountsDKByLed(ByVal LedPosition As Integer) As List(Of Single)
            Get
                Return TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphRefCountsDK
            End Get
            Set(ByVal value As List(Of Single))
                TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphRefCountsDK = value
            End Set
        End Property

        Public Property MeasuresRepeatabilityAbsorbances(ByVal LedPosition As Integer) As List(Of Single)
            Get
                Return TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityAbsorbances
            End Get
            Set(ByVal value As List(Of Single))
                TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityAbsorbances = value
            End Set
        End Property

        Public ReadOnly Property GetWaveLengthRepeatabilityResult(ByVal LedPosition As Integer) As Integer
            Get
                Return ResultsRepeatabilityTest(LedPosition).WaveLength
            End Get
        End Property

        Public ReadOnly Property GetAbsorbanceRepeatabilityResult(ByVal LedPosition As Integer) As Single
            Get
                Return ResultsRepeatabilityTest(LedPosition).Absorbance
            End Get
        End Property

        Public ReadOnly Property GetPhMainCountsRepeatabilityResultDK(ByVal LedPosition As Integer) As Single
            Get
                Return ResultsRepeatabilityTest(LedPosition).CountsPhMainDK
            End Get
        End Property

        Public ReadOnly Property GetPhRefCountsRepeatabilityResultDK(ByVal LedPosition As Integer) As Single
            Get
                Return ResultsRepeatabilityTest(LedPosition).CountsPhRefDK
            End Get
        End Property

        Public ReadOnly Property GetAbsorbanceRepeatabilityResult() As List(Of Single)
            Get
                Dim myList As New List(Of Single)
                If Not ResultsRepeatabilityTest Is Nothing Then
                    If UBound(ResultsRepeatabilityTest) > 0 Then
                        For i As Integer = 0 To UBound(ResultsRepeatabilityTest)
                            myList.Add(ResultsRepeatabilityTest(i).Absorbance)

                        Next
                    End If
                End If

                Return myList
            End Get
        End Property

        Public ReadOnly Property GetAbsorbanceDeviationRepeatabilityResult(ByVal LedPosition As Integer) As Single
            Get
                Return ResultsRepeatabilityTest(LedPosition).STDDeviationAbsorbance
            End Get
        End Property

        Public ReadOnly Property GetSTDDeviationPhMainRepeatabilityResultDK(ByVal LedPosition As Integer) As Single
            Get
                Return ResultsRepeatabilityTest(LedPosition).STDDeviationPhMainDK
            End Get
        End Property

        Public ReadOnly Property GetSTDDeviationPhRefRepeatabilityResultDK(ByVal LedPosition As Integer) As Single
            Get
                Return ResultsRepeatabilityTest(LedPosition).STDDeviationPhRefDK
            End Get
        End Property

        Public ReadOnly Property GetSTDDeviationAbsorbanceRepeatabilityResult(ByVal LedPosition As Integer) As Single
            Get
                Return ResultsRepeatabilityTest(LedPosition).STDDeviationAbsorbance
            End Get
        End Property

        Public ReadOnly Property GetCVAbsorbanceRepeatabilityResults(ByVal LedPosition As Integer) As String
            Get
                Dim returnValue As String = ""
                If ResultsRepeatabilityTest(LedPosition).CVAbsorbance > MyClass.MaxCvToDisplay Then
                    returnValue = "> " & MyClass.MaxCvToDisplay.ToString & MyClass.myDecimalSeparator.ToString & "00 %"
                    'ElseIf ResultsRepeatabilityTest(LedPosition).CVAbsorbance < -100 Then
                    '   returnValue = "<-" & MyClass.MaxCvToDisplay.ToString & ".00 %"
                Else
                    returnValue = ResultsRepeatabilityTest(LedPosition).CVAbsorbance.ToString("#,##0.00") & " %"
                End If
                Return returnValue
            End Get
        End Property

        Public ReadOnly Property GetMAXPhMainRepeatabilityResultDK(ByVal LedPosition As Integer) As Single
            Get
                Dim returnValue As Single = 0
                If Not MyClass.TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphMainCountsDK Is Nothing AndAlso _
                   MyClass.TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphMainCountsDK.Count > 0 Then
                    returnValue = MyClass.TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphMainCountsDK.Max
                End If
                Return returnValue
            End Get
        End Property

        Public ReadOnly Property GetMINPhMainRepeatabilityResultDK(ByVal LedPosition As Integer) As Single
            Get
                Dim returnValue As Single = 0
                If Not MyClass.TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphMainCountsDK Is Nothing AndAlso _
                   MyClass.TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphMainCountsDK.Count > 0 Then
                    returnValue = MyClass.TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphMainCountsDK.Min
                End If
                Return returnValue
            End Get
        End Property

        Public ReadOnly Property GetMAXPhRefRepeatabilityResultDK(ByVal LedPosition As Integer) As Single
            Get
                Dim returnValue As Single = 0
                If Not MyClass.TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphRefCountsDK Is Nothing AndAlso _
                   MyClass.TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphRefCountsDK.Count > 0 Then
                    returnValue = MyClass.TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphRefCountsDK.Max
                End If
                Return returnValue
            End Get
        End Property

        Public ReadOnly Property GetMINPhRefRepeatabilityResultDK(ByVal LedPosition As Integer) As Single
            Get
                Dim returnValue As Single = 0
                If Not MyClass.TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphRefCountsDK Is Nothing AndAlso _
                   MyClass.TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphRefCountsDK.Count > 0 Then
                    returnValue = MyClass.TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphRefCountsDK.Min
                End If
                Return returnValue
            End Get
        End Property

        Public ReadOnly Property GetMAXAbsorbanceRepeatabilityResult(ByVal LedPosition As Integer) As Single
            Get
                Dim returnValue As Single = 0
                If Not MyClass.TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityAbsorbances Is Nothing AndAlso _
                   MyClass.TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityAbsorbances.Count > 0 Then
                    returnValue = MyClass.TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityAbsorbances.Max
                End If
                Return returnValue
            End Get
        End Property

        Public ReadOnly Property GetMINAbsorbanceRepeatabilityResult(ByVal LedPosition As Integer) As Single
            Get
                Dim returnValue As Single = 0
                If Not MyClass.TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityAbsorbances Is Nothing AndAlso _
                   MyClass.TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityAbsorbances.Count > 0 Then
                    returnValue = MyClass.TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityAbsorbances.Min
                End If
                Return returnValue
            End Get
        End Property

        Public ReadOnly Property GetRangePhMainRepeatabilityResultDK(ByVal LedPosition As Integer) As Single
            Get
                Dim returnValue As Single = 0
                returnValue = GetMAXPhMainRepeatabilityResultDK(LedPosition) - GetMINPhMainRepeatabilityResultDK(LedPosition)
                Return returnValue
            End Get
        End Property

        Public ReadOnly Property GetRangePhRefRepeatabilityResultDK(ByVal LedPosition As Integer) As Single
            Get
                Dim returnValue As Single = 0
                returnValue = GetMAXPhRefRepeatabilityResultDK(LedPosition) - GetMINPhRefRepeatabilityResultDK(LedPosition)
                Return returnValue
            End Get
        End Property

        Public ReadOnly Property GetRangeAbsorbanceRepeatabilityResult(ByVal LedPosition As Integer) As Single
            Get
                Dim returnValue As Single = 0
                returnValue = GetMAXAbsorbanceRepeatabilityResult(LedPosition) - GetMINAbsorbanceRepeatabilityResult(LedPosition)
                Return returnValue
            End Get
        End Property

        Public ReadOnly Property MaxRepeatability() As Single
            Get
                Return CommonParameters.MaxRepeatability
            End Get
        End Property

#End Region

#Region "STABILITY"
        Public Property StabilityReadings() As Integer
            Get
                Return StabilityReadingsAttr
            End Get
            Set(ByVal value As Integer)
                StabilityReadingsAttr = value
            End Set
        End Property

        Public Property MeasuresStabilityphMainCountsByLed(ByVal LedPosition As Integer) As List(Of Single)
            Get
                Return TestReadedCountsAttr(LedPosition).MeasuresStabilityphMainCounts
            End Get
            Set(ByVal value As List(Of Single))
                TestReadedCountsAttr(LedPosition).MeasuresStabilityphMainCounts = value
            End Set
        End Property

        Public Property MeasuresStabilityphRefCountsByLed(ByVal LedPosition As Integer) As List(Of Single)
            Get
                Return TestReadedCountsAttr(LedPosition).MeasuresStabilityphRefCounts
            End Get
            Set(ByVal value As List(Of Single))
                TestReadedCountsAttr(LedPosition).MeasuresStabilityphRefCounts = value
            End Set
        End Property

        Public Property MeasuresStabilityphMainCountsDKByLed(ByVal LedPosition As Integer) As List(Of Single)
            Get
                Return TestReadedCountsAttr(LedPosition).MeasuresStabilityphMainCountsDK
            End Get
            Set(ByVal value As List(Of Single))
                TestReadedCountsAttr(LedPosition).MeasuresStabilityphMainCountsDK = value
            End Set
        End Property

        Public Property MeasuresStabilityphRefCountsDKByLed(ByVal LedPosition As Integer) As List(Of Single)
            Get
                Return TestReadedCountsAttr(LedPosition).MeasuresStabilityphRefCountsDK
            End Get
            Set(ByVal value As List(Of Single))
                TestReadedCountsAttr(LedPosition).MeasuresStabilityphRefCountsDK = value
            End Set
        End Property

        Public Property MeasuresStabilityAbsorbances(ByVal LedPosition As Integer) As List(Of Single)
            Get
                Return TestReadedCountsAttr(LedPosition).MeasuresStabilityAbsorbances
            End Get
            Set(ByVal value As List(Of Single))
                TestReadedCountsAttr(LedPosition).MeasuresStabilityAbsorbances = value
            End Set
        End Property

        Public ReadOnly Property GetWaveLengthStabilityResult(ByVal LedPosition As Integer) As Integer
            Get
                Return ResultsStabilityTest(LedPosition).WaveLength
            End Get
        End Property

        Public ReadOnly Property GetAbsorbanceStabilityResult(ByVal LedPosition As Integer) As Single
            Get
                Return ResultsStabilityTest(LedPosition).Absorbance
            End Get
        End Property

        Public ReadOnly Property GetAbsorbanceStabilityResult() As List(Of Single)
            Get
                Dim myList As New List(Of Single)
                If Not ResultsStabilityTest Is Nothing Then
                    If UBound(ResultsStabilityTest) > 0 Then
                        For i As Integer = 0 To UBound(ResultsStabilityTest)
                            myList.Add(ResultsStabilityTest(i).Absorbance)
                        Next
                    End If
                End If

                Return myList
            End Get
        End Property

        Public ReadOnly Property GetPhMainCountsStabilityResultDK(ByVal LedPosition As Integer) As Single
            Get
                Return ResultsStabilityTest(LedPosition).CountsPhMainDK
            End Get
        End Property

        Public ReadOnly Property GetPhRefCountsStabilityResultDK(ByVal LedPosition As Integer) As Single
            Get
                Return ResultsStabilityTest(LedPosition).CountsPhRefDK
            End Get
        End Property

        Public ReadOnly Property GetAbsorbanceDeviationStabilityResult(ByVal LedPosition As Integer) As Single
            Get
                Return ResultsStabilityTest(LedPosition).STDDeviationAbsorbance
            End Get
        End Property

        Public ReadOnly Property GetSTDDeviationPhMainStabilityResultDK(ByVal LedPosition As Integer) As Single
            Get
                Return ResultsStabilityTest(LedPosition).STDDeviationPhMainDK
            End Get
        End Property

        Public ReadOnly Property GetSTDDeviationPhRefStabilityResultDK(ByVal LedPosition As Integer) As Single
            Get
                Return ResultsStabilityTest(LedPosition).STDDeviationPhRefDK
            End Get
        End Property

        Public ReadOnly Property GetSTDDeviationAbsorbanceStabilityResult(ByVal LedPosition As Integer) As Single
            Get
                Return ResultsStabilityTest(LedPosition).STDDeviationAbsorbance
            End Get
        End Property

        Public ReadOnly Property GetCVAbsorbanceStabilityResults(ByVal LedPosition As Integer) As String
            Get
                Dim returnValue As String = ""
                If ResultsStabilityTest(LedPosition).CVAbsorbance > MyClass.MaxCvToDisplay Then
                    returnValue = "> " & MyClass.MaxCvToDisplay.ToString & MyClass.myDecimalSeparator.ToString & "00 %"
                    'ElseIf ResultsStabilityTest(LedPosition).CVAbsorbance < -100 Then
                    '    returnValue = "<-" & MyClass.MaxCvToDisplay.ToString & ".00 %"
                Else
                    returnValue = ResultsStabilityTest(LedPosition).CVAbsorbance.ToString("#,##0.00") & " %"
                End If
                Return returnValue
            End Get
        End Property

        Public ReadOnly Property GetMAXPhMainStabilityResultDK(ByVal LedPosition As Integer) As Single
            Get
                Dim returnValue As Single = 0
                If Not MyClass.TestReadedCountsAttr(LedPosition).MeasuresStabilityphMainCountsDK Is Nothing AndAlso _
                   MyClass.TestReadedCountsAttr(LedPosition).MeasuresStabilityphMainCountsDK.Count > 0 Then
                    returnValue = MyClass.TestReadedCountsAttr(LedPosition).MeasuresStabilityphMainCountsDK.Max
                End If
                Return returnValue
            End Get
        End Property

        Public ReadOnly Property GetMINPhMainStabilityResultDK(ByVal LedPosition As Integer) As Single
            Get
                Dim returnValue As Single = 0
                If Not MyClass.TestReadedCountsAttr(LedPosition).MeasuresStabilityphMainCountsDK Is Nothing AndAlso _
                   MyClass.TestReadedCountsAttr(LedPosition).MeasuresStabilityphMainCountsDK.Count > 0 Then
                    returnValue = MyClass.TestReadedCountsAttr(LedPosition).MeasuresStabilityphMainCountsDK.Min
                End If
                Return returnValue
            End Get
        End Property

        Public ReadOnly Property GetMAXPhRefStabilityResultDK(ByVal LedPosition As Integer) As Single
            Get
                Dim returnValue As Single = 0
                If Not MyClass.TestReadedCountsAttr(LedPosition).MeasuresStabilityphRefCountsDK Is Nothing AndAlso _
                   MyClass.TestReadedCountsAttr(LedPosition).MeasuresStabilityphRefCountsDK.Count > 0 Then
                    returnValue = MyClass.TestReadedCountsAttr(LedPosition).MeasuresStabilityphRefCountsDK.Max
                End If
                Return returnValue
            End Get
        End Property

        Public ReadOnly Property GetMINPhRefStabilityResultDK(ByVal LedPosition As Integer) As Single
            Get
                Dim returnValue As Single = 0
                If Not MyClass.TestReadedCountsAttr(LedPosition).MeasuresStabilityphRefCountsDK Is Nothing AndAlso _
                   MyClass.TestReadedCountsAttr(LedPosition).MeasuresStabilityphRefCountsDK.Count > 0 Then
                    returnValue = MyClass.TestReadedCountsAttr(LedPosition).MeasuresStabilityphRefCountsDK.Min
                End If
                Return returnValue
            End Get
        End Property

        Public ReadOnly Property GetMAXAbsorbanceStabilityResult(ByVal LedPosition As Integer) As Single
            Get
                Dim returnValue As Single = 0
                If Not MyClass.TestReadedCountsAttr(LedPosition).MeasuresStabilityAbsorbances Is Nothing AndAlso _
                   MyClass.TestReadedCountsAttr(LedPosition).MeasuresStabilityAbsorbances.Count > 0 Then
                    returnValue = MyClass.TestReadedCountsAttr(LedPosition).MeasuresStabilityAbsorbances.Max
                End If
                Return returnValue
            End Get
        End Property

        Public ReadOnly Property GetMINAbsorbanceStabilityResult(ByVal LedPosition As Integer) As Single
            Get
                Dim returnValue As Single = 0
                If Not MyClass.TestReadedCountsAttr(LedPosition).MeasuresStabilityAbsorbances Is Nothing AndAlso _
                   MyClass.TestReadedCountsAttr(LedPosition).MeasuresStabilityAbsorbances.Count > 0 Then
                    returnValue = MyClass.TestReadedCountsAttr(LedPosition).MeasuresStabilityAbsorbances.Min
                End If
                Return returnValue
            End Get
        End Property

        Public ReadOnly Property GetRangePhMainStabilityResultDK(ByVal LedPosition As Integer) As Single
            Get
                Dim returnValue As Single = 0
                returnValue = GetMAXPhMainStabilityResultDK(LedPosition) - GetMINPhMainStabilityResultDK(LedPosition)
                Return returnValue
            End Get
        End Property

        Public ReadOnly Property GetRangePhRefStabilityResultDK(ByVal LedPosition As Integer) As Single
            Get
                Dim returnValue As Single = 0
                returnValue = GetMAXPhRefStabilityResultDK(LedPosition) - GetMINPhRefStabilityResultDK(LedPosition)
                Return returnValue
            End Get
        End Property

        Public ReadOnly Property GetRangeAbsorbanceStabilityResult(ByVal LedPosition As Integer) As Single
            Get
                Dim returnValue As Single = 0
                returnValue = GetMAXAbsorbanceStabilityResult(LedPosition) - GetMINAbsorbanceStabilityResult(LedPosition)
                Return returnValue
            End Get
        End Property

        Public ReadOnly Property MaxStability() As Single
            Get
                Return CommonParameters.MaxStability
            End Get
        End Property

#End Region

#Region "ABS"
        Public Property MeasureABSphMainCounts(ByVal LedPosition As Integer) As Single
            Get
                Return TestReadedCountsAttr(LedPosition).MeasureABSphMainCount
            End Get
            Set(ByVal value As Single)
                TestReadedCountsAttr(LedPosition).MeasureABSphMainCount = value
            End Set
        End Property

        Public Property MeasureABSphRefCounts(ByVal LedPosition As Integer) As Single
            Get
                Return TestReadedCountsAttr(LedPosition).MeasureABSphRefCount
            End Get
            Set(ByVal value As Single)
                TestReadedCountsAttr(LedPosition).MeasureABSphRefCount = value
            End Set
        End Property

        Public ReadOnly Property GetWaveLengthABSResult(ByVal LedPosition As Integer) As Integer
            Get
                Return ResultsABSTest(LedPosition).WaveLength
            End Get
        End Property

        Public ReadOnly Property GetAbsorbanceABSResult(ByVal LedPosition As Integer) As Single
            Get
                Return ResultsABSTest(LedPosition).Absorbance
            End Get
        End Property

        Public ReadOnly Property GetAbsorbanceABSResult() As List(Of Single)
            Get
                Dim myList As New List(Of Single)
                If Not ResultsABSTest Is Nothing Then
                    If UBound(ResultsABSTest) > 0 Then
                        For i As Integer = 0 To UBound(ResultsABSTest)
                            myList.Add(ResultsABSTest(i).Absorbance)
                        Next
                    End If
                End If

                Return myList
            End Get
        End Property

        ' XBC 01/03/2012
        Public ReadOnly Property MaxAbsToDisplay() As Single
            Get
                Return MyClass.MaxAbsToDisplayAttr
            End Get
        End Property
#End Region

#Region "COMENTED"
        'Public Property CheckRotorReadings() As Integer
        '    Get
        '        Return CheckRotorReadingsAttr
        '    End Get
        '    Set(ByVal value As Integer)
        '        CheckRotorReadingsAttr = value
        '    End Set
        'End Property
        'Public Property MeasuresCheckRotorCounts() As List(Of Single)
        '    Get
        '        Return MeasuresCheckRotorCountsAttr
        '    End Get
        '    Set(ByVal value As List(Of Single))
        '        MeasuresCheckRotorCountsAttr = value
        '    End Set
        'End Property

        'Public Property MeasuresCheckRotorAbsorbances() As List(Of Single)
        '    Get
        '        Return MeasuresCheckRotorAbsorbancesAttr
        '    End Get
        '    Set(ByVal value As List(Of Single))
        '        MeasuresCheckRotorAbsorbancesAttr = value
        '    End Set
        'End Property
        'Public ReadOnly Property GetWaveLengthCheckRotorResult() As Integer
        '    Get
        '        Return ResultsCheckRotorTest.WaveLength
        '    End Get
        'End Property

        'Public ReadOnly Property GetAbsorbanceCheckRotorResult() As Single
        '    Get
        '        Return ResultsCheckRotorTest.Absorbance
        '    End Get
        'End Property

        'Public ReadOnly Property GetCountsCheckRotorResult() As Single
        '    Get
        '        Return ResultsCheckRotorTest.Counts
        '    End Get
        'End Property

        'Public ReadOnly Property GetAbsorbanceDeviationCheckRotorResult() As Single
        '    Get
        '        Return ResultsCheckRotorTest.AbsorbanceDeviation
        '    End Get
        'End Property

        'Public ReadOnly Property GetCountsDeviationCheckRotorResult() As Single
        '    Get
        '        Return ResultsCheckRotorTest.CountsDeviation
        '    End Get
        'End Property

        'Public ReadOnly Property GetCVAbsorbanceCheckRotorResults() As String
        '    Get
        '        Dim resultValue As String
        '        Dim CV As Single
        '        resultValue = "< 10%"
        '        CV = MyClass.ResultsCheckRotorTest.Absorbance / MyClass.ResultsCheckRotorTest.AbsorbanceDeviation * 100
        '        If CV >= 10 Then
        '            resultValue = CV.ToString + "%"
        '        End If
        '        Return resultValue
        '    End Get
        'End Property

        'Public ReadOnly Property GetCVCountsCheckRotorResults() As String
        '    Get
        '        Dim resultValue As String
        '        Dim CV As Single
        '        resultValue = "< 10%"
        '        CV = MyClass.ResultsCheckRotorTest.Counts / MyClass.ResultsCheckRotorTest.CountsDeviation * 100
        '        If CV >= 10 Then
        '            resultValue = CV.ToString + "%"
        '        End If
        '        Return resultValue
        '    End Get
        'End Property

        'Public ReadOnly Property GetMaxValueCheckRotorResult() As Single
        '    Get
        '        Return ResultsCheckRotorTest.MaxValue
        '    End Get
        'End Property

        'Public ReadOnly Property GetMinValueCheckRotorResult() As Single
        '    Get
        '        Return ResultsCheckRotorTest.MinValue
        '    End Get
        'End Property

        'Public ReadOnly Property GetRangeValueCheckRotorResult() As Single
        '    Get
        '        Return ResultsCheckRotorTest.MaxValue - ResultsCheckRotorTest.MinValue
        '    End Get
        'End Property

        'Public Property PrimeDone() As Boolean
        '    Get
        '        Return PrimeDoneAttr
        '    End Get
        '    Set(ByVal value As Boolean)
        '        PrimeDoneAttr = value
        '    End Set
        'End Property

        'Public Property TestCheckRotorDone() As Boolean
        '    Get
        '        Return TestCheckRotorDoneAttr
        '    End Get
        '    Set(ByVal value As Boolean)
        '        TestCheckRotorDoneAttr = value
        '    End Set
        'End Property

        'Public Property TestDarknessCountsDone() As Boolean
        '    Get
        '        Return TestDarknessCountsDoneAttr
        '    End Get
        '    Set(ByVal value As Boolean)
        '        TestDarknessCountsDoneAttr = value
        '    End Set
        'End Property
        'Public ReadOnly Property MeasuresRepeatabilityphMainCountsByAll(ByVal LedPosition As Integer) As List(Of Single)
        '    Get
        '        Dim myList As New List(Of Single)
        '        If Not TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphMainCounts Is Nothing Then
        '            If TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphMainCounts.Count > 0 Then
        '                For i As Integer = 0 To TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphMainCounts.Count - 1
        '                    myList.Add(TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphMainCounts(i))
        '                Next
        '            End If
        '        End If

        '        Return myList
        '    End Get
        'End Property
        'Public ReadOnly Property MeasuresRepeatabilityphRefCountsByAll(ByVal LedPosition As Integer) As List(Of Single)
        '    Get
        '        Dim myList As New List(Of Single)
        '        If Not TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphRefCounts Is Nothing Then
        '            If TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphRefCounts.Count > 0 Then
        '                For i As Integer = 0 To TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphRefCounts.Count - 1
        '                    myList.Add(TestReadedCountsAttr(LedPosition).MeasuresRepeatabilityphRefCounts(i))
        '                Next
        '            End If
        '        End If

        '        Return myList
        '    End Get
        'End Property
        'Public ReadOnly Property MeasuresStabilityphMainCountsByAll(ByVal LedPosition As Integer) As List(Of Single)
        '    Get
        '        Dim myList As New List(Of Single)
        '        If Not TestReadedCountsAttr(LedPosition).MeasuresStabilityphMainCounts Is Nothing Then
        '            If TestReadedCountsAttr(LedPosition).MeasuresStabilityphMainCounts.Count > 0 Then
        '                For i As Integer = 0 To TestReadedCountsAttr(LedPosition).MeasuresStabilityphMainCounts.Count - 1
        '                    myList.Add(TestReadedCountsAttr(LedPosition).MeasuresStabilityphMainCounts(i))
        '                Next
        '            End If
        '        End If

        '        Return myList
        '    End Get
        'End Property
        'Public ReadOnly Property MeasuresStabilityphRefCountsByAll(ByVal LedPosition As Integer) As List(Of Single)
        '    Get
        '        Dim myList As New List(Of Single)
        '        If Not TestReadedCountsAttr(LedPosition).MeasuresStabilityphRefCounts Is Nothing Then
        '            If TestReadedCountsAttr(LedPosition).MeasuresStabilityphRefCounts.Count > 0 Then
        '                For i As Integer = 0 To TestReadedCountsAttr(LedPosition).MeasuresStabilityphRefCounts.Count - 1
        '                    myList.Add(TestReadedCountsAttr(LedPosition).MeasuresStabilityphRefCounts(i))
        '                Next
        '            End If
        '        End If

        '        Return myList
        '    End Get
        'End Property

#End Region

#End Region

#Region "Event Handlers"
        ''' <summary>
        ''' manages the responses of the Analyzer
        ''' The response can be OK, NG, Timeout or Exception
        ''' </summary>
        ''' <param name="pResponse">response type</param>
        ''' <param name="pData">data received</param>
        ''' <remarks>Created by XBC 01/03/11</remarks>
        Private Sub ScreenReceptionLastFwScriptEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) Handles MyClass.ReceivedLastFwScriptEvent
            Dim myGlobal As New GlobalDataTO
            Try
                'manage special operations according to the screen characteristics

                ' XBC 05/05/2011 - timeout limit repetitions
                If pResponse = RESPONSE_TYPES.TIMEOUT Or _
                   pResponse = RESPONSE_TYPES.EXCEPTION Then


                    If MyClass.ReportCountTimeout = 0 Then
                        MyClass.ReportCountTimeout += 1
                        ' registering the incidence in historical reports activity
                        If MyClass.RecommendationsReport Is Nothing Then
                            ReDim MyClass.RecommendationsReport(0)
                        Else
                            ReDim Preserve MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport) + 1)
                        End If
                        MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport)) = HISTORY_RECOMMENDATIONS.ERR_COMM
                    End If

                    Exit Sub
                End If
                ' XBC 05/05/2011 - timeout limit repetitions


                ' XBC 20/02/2012
                Select Case CurrentOperation
                    Case OPERATIONS.WASHING_STATION_UP
                        Select Case pResponse
                            Case RESPONSE_TYPES.START
                                ' Nothing by now
                            Case RESPONSE_TYPES.OK
                                MyClass.IsWashingStationUpAttr = True
                        End Select
                        Exit Sub

                    Case OPERATIONS.WASHING_STATION_DOWN
                        Select Case pResponse
                            Case RESPONSE_TYPES.START
                                ' Nothing by now
                            Case RESPONSE_TYPES.OK
                                MyClass.IsWashingStationUpAttr = False
                        End Select
                        Exit Sub
                End Select
                ' XBC 20/02/2012

                Select Case MyClass.CurrentTestAttr
                    Case ADJUSTMENT_GROUPS.PHOTOMETRY

                        Select Case CurrentOperation

                            Case OPERATIONS.HOMES
                                Select Case pResponse
                                    Case RESPONSE_TYPES.START
                                        ' Nothing by now
                                        'MyClass.HomesDoingAttr = True
                                        'CurrentTimeOperationAttr = myFwScriptDelegate.CurrentFwScriptItem.TimeExpected
                                    Case RESPONSE_TYPES.OK
                                        MyClass.HomesDoingAttr = False
                                        MyClass.HomesDoneAttr = True
                                End Select

                            Case OPERATIONS.READ_COUNTS
                                If pResponse = RESPONSE_TYPES.OK Then
                                    If MyClass.WellToUse > 0 Then
                                        MyClass.EmptyWell(MyClass.WellToUse - 1) = False
                                    End If

                                    ' Baseline/Darkness Counts Tests done !
                                    myGlobal = ManageResultsBL()
                                    If Not myGlobal.HasError Then
                                        MyClass.TestBaseLineDone = True
                                    End If

                                End If

                        End Select

                    Case ADJUSTMENT_GROUPS.IT_EDITION
                        Select Case CurrentOperation
                            Case OPERATIONS.SAVE_ADJUSMENTS
                                If pResponse = RESPONSE_TYPES.OK Then
                                    If Not myGlobal.HasError Then
                                        MyClass.SaveLedsIntensitiesDone = True
                                    End If
                                End If
                        End Select


                    Case ADJUSTMENT_GROUPS.REPEATABILITY

                        Select Case CurrentOperation
                            'Case OPERATIONS.HOMES
                            '    Select Case pResponse
                            '        Case RESPONSE_TYPES.START
                            '            ' Nothing by now
                            '            'MyClass.HomesDoingAttr = True
                            '            'CurrentTimeOperationAttr = myFwScriptDelegate.CurrentFwScriptItem.TimeExpected
                            '        Case RESPONSE_TYPES.OK
                            '            MyClass.HomesDoingAttr = False
                            '            MyClass.HomesDoneAttr = True
                            '    End Select

                            Case OPERATIONS.READ_COUNTS
                                If pResponse = RESPONSE_TYPES.OK Then
                                    If MyClass.WellToUse > 0 Then
                                        MyClass.EmptyWell(MyClass.WellToUse - 1) = False
                                    End If
                                    MyClass.RepeatabilityReadingsAttr += 1
                                    ' Repeatability Test done !
                                    myGlobal = MeasureRepeatabilityReadedCounts()
                                    If Not myGlobal.HasError Then
                                        Debug.Print("Num readings : " & MyClass.RepeatabilityReadingsAttr.ToString)
                                        If MyClass.RepeatabilityReadingsAttr >= MyClass.CommonParameters.MaxRepeatability Then
                                            myGlobal = ManageTestResults()
                                            If Not myGlobal.HasError Then
                                                MyClass.TestRepeatabilityDone = True
                                            End If
                                        End If
                                    End If
                                End If
                        End Select

                    Case ADJUSTMENT_GROUPS.STABILITY

                        Select Case CurrentOperation
                            'Case OPERATIONS.HOMES
                            '    Select Case pResponse
                            '        Case RESPONSE_TYPES.START
                            '            ' Nothing by now
                            '            'MyClass.HomesDoingAttr = True
                            '            'CurrentTimeOperationAttr = myFwScriptDelegate.CurrentFwScriptItem.TimeExpected
                            '        Case RESPONSE_TYPES.OK
                            '            MyClass.HomesDoingAttr = False
                            '            MyClass.HomesDoneAttr = True
                            '    End Select

                            Case OPERATIONS.READ_COUNTS
                                If pResponse = RESPONSE_TYPES.OK Then
                                    If MyClass.WellToUse > 0 Then
                                        MyClass.EmptyWell(MyClass.WellToUse - 1) = False
                                    End If
                                    MyClass.StabilityReadingsAttr += 1
                                    ' Stability Test done !
                                    myGlobal = MeasureStabilityReadedCounts()
                                    If Not myGlobal.HasError Then
                                        Debug.Print("Num readings : " & MyClass.StabilityReadingsAttr.ToString)
                                        If MyClass.StabilityReadingsAttr >= MyClass.CommonParameters.MaxStability Then
                                            myGlobal = ManageTestResults()
                                            If Not myGlobal.HasError Then
                                                MyClass.TestStabilityDone = True
                                            End If
                                        End If
                                    End If
                                End If
                        End Select

                    Case ADJUSTMENT_GROUPS.ABSORBANCE_MEASUREMENT

                        Select Case CurrentOperation
                            'Case OPERATIONS.HOMES
                            '    Select Case pResponse
                            '        Case RESPONSE_TYPES.START
                            '            ' Nothing by now
                            '            'MyClass.HomesDoingAttr = True
                            '            'CurrentTimeOperationAttr = myFwScriptDelegate.CurrentFwScriptItem.TimeExpected
                            '        Case RESPONSE_TYPES.OK
                            '            MyClass.HomesDoingAttr = False
                            '            MyClass.HomesDoneAttr = True
                            '    End Select

                            Case OPERATIONS.READ_COUNTS
                                If pResponse = RESPONSE_TYPES.OK Then
                                    If MyClass.WellToUse > 0 Then
                                        MyClass.EmptyWell(MyClass.WellToUse - 1) = False
                                    End If
                                    ' Absorbance Measurement Test done !
                                    myGlobal = ManageABSTestResults(False)
                                    If Not myGlobal.HasError Then
                                        MyClass.TestABSDone = True
                                    End If
                                End If
                        End Select


                        'Case ADJUSTMENT_GROUPS.CHECK_ROTOR
                        '    CANCELLED


                End Select

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.ScreenReceptionLastFwScriptEvent", EventLogEntryType.Error, False)
            End Try
        End Sub
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Create the corresponding Script list according to the Screen Mode
        ''' </summary>
        ''' <param name="pMode">Screen mode</param>
        ''' <param name="pAdjustmentGroup">Adjustment type group</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: XBC 23/02/11
        ''' </remarks>
        Public Function SendFwScriptsQueueList(ByVal pMode As ADJUSTMENT_MODES, _
                                               Optional ByVal pAdjustmentGroup As ADJUSTMENT_GROUPS = Nothing) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                ' Create the list of Scripts which are need to initialize this Adjustment

                Select Case pMode
                    Case ADJUSTMENT_MODES.ADJUSTMENTS_READING
                        myResultData = MyBase.SendQueueForREADINGADJUSTMENTS()

                    Case ADJUSTMENT_MODES.TESTING
                        If MyClass.CurrentTestAttr <> pAdjustmentGroup Then
                            If MyClass.CurrentTestAttr <> ADJUSTMENT_GROUPS._NONE Then
                                MyClass.PreviousTestAttr = MyClass.CurrentTestAttr
                            End If
                            MyClass.CurrentTestAttr = pAdjustmentGroup
                        End If
                        myResultData = MyClass.SendQueueForTESTING(pAdjustmentGroup)

                        'Case ADJUSTMENT_MODES.TEST_EXITING
                        '    myResultData = MyClass.SendQueueForTEST_EXITING(pAdjustmentGroup)
                        ' Pending on future requirements
                End Select

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.SendFwScriptsQueueList", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Get the list of WaveLengths
        ''' </summary>
        ''' <param name="pAnalyzerID">Identifier of the Analyzer serial number which the data will be obtained</param>
        ''' <returns>GlobalDataTO containing a Typed Dataset AnalyzerLedPositionsDS with the list of WaveLength items</returns>
        ''' <remarks>Created by : XBC 24/02/2011</remarks>
        Public Function ReadWaveLengths(ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myAnalyzerLedPositionsDelegate As New AnalyzerLedPositionsDelegate
            Dim myAnalyzerLedPosDS As New AnalyzerLedPositionsDS
            Try
                myResultData = myAnalyzerLedPositionsDelegate.GetAllWaveLengths(Nothing, pAnalyzerID)
                AnalyzerId = pAnalyzerID

                If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
                    myAnalyzerLedPosDS = DirectCast(myResultData.SetDatos, AnalyzerLedPositionsDS)
                    If myAnalyzerLedPosDS.tcfgAnalyzerLedPositions.Rows.Count > 0 Then

                        qLeds = (From a In myAnalyzerLedPosDS.tcfgAnalyzerLedPositions _
                           Order By a.LedPosition _
                           Select a Where a.Status = True _
                           Order By a.WaveLength).ToList()

                        Dim myPhotometryDataTO As New PhotometryDataTO
                        PositionLEDsxSimulation = New List(Of Integer)
                        For i As Integer = 0 To qLeds.Count - 1
                            myPhotometryDataTO.CountsMainBaseline.Add(0)
                            myPhotometryDataTO.CountsMainDarkness.Add(0)
                            myPhotometryDataTO.CountsRefBaseline.Add(0)
                            myPhotometryDataTO.CountsRefDarkness.Add(0)
                            myPhotometryDataTO.IntegrationTimes.Add(0)
                            myPhotometryDataTO.LEDsIntensities.Add(0)
                            myPhotometryDataTO.PositionLed.Add(qLeds(i).LedPosition)

                            PositionLEDsxSimulation.Add(qLeds(i).LedPosition)
                        Next
                        myFwScriptDelegate.AnalyzerManager.SetPhotometryData(myPhotometryDataTO)

                        CommonParameters.MaxWaveLengths = qLeds.Count ' myAnalyzerLedPosDS.tcfgAnalyzerLedPositions.Rows.Count

                        myResultData.SetDatos = qLeds
                    Else
                        myResultData.HasError = True
                    End If
                Else
                    myResultData.HasError = True
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.ReadWaveLengths", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Get the Photometry Parameters
        ''' </summary>
        ''' <param name="pAnalyzerModel">Identifier of the Analyzer model which the data will be obtained</param>
        ''' <returns>GlobalDataTO containing a Typed Dataset AnalyzerLedPositionsDS with the list of WaveLength items</returns>
        ''' <remarks>Created by : XBC 24/02/2011</remarks>
        Public Function GetParameters(ByVal pAnalyzerModel As String) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myParams As New SwParametersDelegate
            Dim myParametersDS As New ParametersDS
            Try
                ' Max Rotor Wells
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_MAX_WELLS_REACTIONS_ROTOR.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    CommonParameters.MaxWells = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                    'CommonParameters.MaxWells = 12 ' for test !!!
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' List of empty/filled wells
                MyClass.EmptyWell = New List(Of Boolean)
                For i As Integer = 0 To CommonParameters.MaxWells
                    ' by default all wells are empty
                    MyClass.EmptyWell.Add(True)
                Next
                '' Wells to Read for the Baseline test
                'myResultData = myParams.ReadByParameterName(Nothing, "SRV_WELLS_TO_READ_BL", pAnalyzerModel)
                'If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                '    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                '    CommonParameters.WellsToReadBL = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                '    'CommonParameters.WellsToReadBL = 12 ' for test !!!
                'Else
                '    myResultData.HasError = True
                '    Exit Try
                'End If
                ' Maximum number of measures for the Stability test
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_MAX_STABILITY.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    CommonParameters.MaxStability = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                    'CommonParameters.MaxStability = 10 ' for test !!!
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' Maximum number of measures for the Repeatability test
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_MAX_REPEATABILITY.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    CommonParameters.MaxRepeatability = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                    'CommonParameters.MaxRepeatability = 10 ' for test !!!
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                ' PathLength
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.PATH_LENGHT.ToString, Nothing)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.PathLength = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                ' LimitAbs
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.LIMIT_ABS.ToString, Nothing)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.LimitAbs = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                ' Interval Time to Reading Counts
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_READING_TIME.ToString, Nothing)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.ReadingTime = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                ' Interval Time to Reading Counts
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_READING_TIME_OFFSET_STABILITY.ToString, Nothing)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.ReadingStabilityOffsetTime = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If


                MyClass.DilutionFactor = 1   ' Per ara no es fa servir (AG)


                ' Max value allowed to show Coefficient Variation
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_MAX_CV.ToString, Nothing)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.MaxCvToDisplay = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                ' Max number warnings for display a warning message
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_MAX_LEDS_WARNINGS.ToString, Nothing)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.MaxLEDsWarningsAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                ' Max value allowed to show Absorbance calculation
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_MAX_ABS.ToString, Nothing)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.MaxAbsToDisplayAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.GetParameters", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Get Limits values from BD for every different arm
        ''' </summary>
        ''' <remarks>Created by XBC 30/03/2011</remarks>
        Public Function GetLimitValues() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myFieldLimitsDS As New FieldLimitsDS
            Try
                myResultData = GetControlsLimits(FieldLimitsEnum.SRV_LEDS_LIMIT)
                If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
                    myFieldLimitsDS = CType(myResultData.SetDatos, FieldLimitsDS)
                    If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                        MyClass.LimitMinLEDsAttr = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Long)
                        MyClass.LimitMaxLEDsAttr = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Long)
                    End If
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                myResultData = GetControlsLimits(FieldLimitsEnum.SRV_BL_PhMAIN_LIMIT)
                If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
                    myFieldLimitsDS = CType(myResultData.SetDatos, FieldLimitsDS)
                    If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                        MyClass.LimitMinPhMainAttr = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Long)
                        MyClass.LimitMaxPhMainAttr = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Long)
                    End If
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                myResultData = GetControlsLimits(FieldLimitsEnum.SRV_BL_PhREF_LIMIT)
                If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
                    myFieldLimitsDS = CType(myResultData.SetDatos, FieldLimitsDS)
                    If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                        MyClass.LimitMinPhRefAttr = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Long)
                        MyClass.LimitMaxPhRefAttr = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Long)
                    End If
                Else
                    myResultData.HasError = True
                    Exit Try
                End If


            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.GetLimitValues", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        Public Function Initialize() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myPhotometryDataTO As PhotometryDataTO
            Try
                'MyClass.TestBaseLineDoneAttr = False
                MyClass.HomesDoneAttr = False

                '' ResultsBL
                'ReDim MyClass.ResultsBL(CommonParameters.MaxWaveLengths)
                'For i As Integer = 0 To CommonParameters.MaxWaveLengths - 1
                '    MyClass.ResultsBL(i).Counts = 0
                'Next

                myResultData = myFwScriptDelegate.AnalyzerManager.ReadPhotometryData
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myPhotometryDataTO = CType(myResultData.SetDatos, PhotometryDataTO)

                    ' Initialize variables
                    ReDim MyClass.ResultsRepeatabilityTest(qLeds.Count - 1)
                    ReDim MyClass.ResultsStabilityTest(qLeds.Count - 1)
                    ReDim MyClass.ResultsABSTest(qLeds.Count - 1)

                    ReDim MyClass.TestReadedCountsAttr(qLeds.Count - 1)
                    For i As Integer = 0 To qLeds.Count - 1
                        MyClass.TestReadedCountsAttr(i).LedPosition = qLeds(i).LedPosition
                    Next
                Else
                    myResultData.HasError = True
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.Initialize", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Create Baseline / Darkness Counts test output File
        ''' </summary>
        ''' <param name="pPath">destination where save</param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 02/03/2011</remarks>
        Public Function SaveBLDCFile(ByVal pPath As String, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myPhotometryDataTO As PhotometryDataTO = Nothing
            Try
                Dim myUtility As New Utilities()

                ' Serialize PhotometryDataTO / save value of the current test executed
                myResultData = myFwScriptDelegate.AnalyzerManager.ReadPhotometryData()
                If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
                    myPhotometryDataTO = DirectCast(myResultData.SetDatos, PhotometryDataTO)
                    myPhotometryDataTO.AnalyzerID = pAnalyzerID
                    AnalyzerId = pAnalyzerID
                    myResultData = myUtility.Serialize(myPhotometryDataTO, pPath)
                End If

                If Not myResultData.HasError Then
                    myResultData = AcceptBLResults()
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.SaveBLDCFile", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Save Baseline / Darkness Counts test values
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 08/03/2011</remarks>
        Public Function AcceptBLResults() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myPhotometryDataTO As PhotometryDataTO = Nothing
            Try
                myResultData = myFwScriptDelegate.AnalyzerManager.ReadPhotometryData()
                If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
                    myPhotometryDataTO = DirectCast(myResultData.SetDatos, PhotometryDataTO)

                    ' Save current values as BLDC tests results to use with next tests
                    MyClass.BaseLineMainCountsAttr = New List(Of Single)
                    MyClass.BaseLineRefCountsAttr = New List(Of Single)
                    MyClass.IntegrationTimesAttr = New List(Of Single)
                    MyClass.PositionLEDsAttr = New List(Of Single)
                    For i As Integer = 0 To myPhotometryDataTO.CountsMainBaseline.Count - 1
                        MyClass.BaseLineMainCountsAttr.Add(myPhotometryDataTO.CountsMainBaseline(i))
                        MyClass.BaseLineRefCountsAttr.Add(myPhotometryDataTO.CountsRefBaseline(i))
                        MyClass.IntegrationTimesAttr.Add(myPhotometryDataTO.IntegrationTimes(i))
                        MyClass.PositionLEDsAttr.Add(myPhotometryDataTO.PositionLed(i))
                    Next
                    MyClass.DarkphMainCountAttr = myPhotometryDataTO.CountsMainDarkness.Average()
                    MyClass.DarkphRefCountAttr = myPhotometryDataTO.CountsRefDarkness.Average()
                End If


            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.AcceptBLResults", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Get Baseline / Darkness Counts test previously completed
        ''' </summary>
        ''' <param name="pPath">source from recoverave</param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 02/03/2011</remarks>
        Public Function GetLastBLDCCompletedTest(ByVal pPath As String) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myPhotometryDataTO As New PhotometryDataTO
            Try
                Dim myUtility As New Utilities()

                ' Serialitze PhotometryDataTO / save value of the current test executed
                myResultData = myUtility.DeSerialize(myPhotometryDataTO, pPath)
                If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
                    myPhotometryDataTO = DirectCast(myResultData.SetDatos, PhotometryDataTO)
                    myResultData = myFwScriptDelegate.AnalyzerManager.SetPhotometryData(myPhotometryDataTO)

                    If Not myResultData.HasError Then
                        myResultData.SetDatos = myPhotometryDataTO
                    End If
                Else
                    myResultData.HasError = True
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.GetLastBLDCCompletedTest", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Load Adjustments High Level Instruction to save values into the instrument
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 16/05/2011</remarks>
        Public Function SendLOAD_ADJUSTMENTS(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                MyClass.CurrentTestAttr = pAdjustment
                MyClass.CurrentOperation = OPERATIONS.SAVE_ADJUSMENTS
                myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.LOADADJ, True, Nothing, MyClass.pValueAdjustAttr)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.SendLOAD_ADJUSTMENTS", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Load Adjustments High Level Instruction to move Washing Station
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 20/02/2012</remarks>
        Public Function SendWASH_STATION_CTRL(ByVal pAction As Ax00WashStationControlModes) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Select Case pAction
                    Case Ax00WashStationControlModes.UP
                        MyClass.CurrentOperation = OPERATIONS.WASHING_STATION_UP
                    Case Ax00WashStationControlModes.DOWN
                        MyClass.CurrentOperation = OPERATIONS.WASHING_STATION_DOWN
                End Select
                myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.WASH_STATION_CTRL, True, Nothing, pAction)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.SendWASH_STATION_CTRL", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' NRotor High Level Instruction
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XB 13/10/2014 - Use NROTOR instead WSCTRL when Wash Station is down - BA-2004</remarks>
        Public Function SendNEW_ROTOR() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myParams As New List(Of String)
            Try

                CurrentOperation = OPERATIONS.WASHING_STATION_DOWN

                myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.NROTOR, True)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.SendNEW_ROTOR", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function


        ''' <summary>
        ''' Method to decode the data information of the this screen from a String format source and obtain the data information easily legible
        ''' </summary>
        ''' <param name="pTask">task identifier</param>
        ''' <param name="pAction">task's action identifier</param>
        ''' <param name="pData">content data with the information to format</param>
        ''' <param name="pcurrentLanguage">language identifier to localize contents</param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 02/08/2011</remarks>
        Public Function DecodeDataReport(ByVal pTask As String, ByVal pAction As String, ByVal pData As String, ByVal pcurrentLanguage As String) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                Dim myUtility As New Utilities()
                Dim text1 As String
                Dim text2 As String
                Dim text3 As String
                Dim text As String = ""
                Dim ReportInfo() As List(Of String) = Nothing

                'myResultData = MyClass.GetCultureInfo()
                'If myResultData.HasError Then
                '    Dim myLogAcciones As New ApplicationLogManager()
                '    myLogAcciones.CreateLogActivity(myResultData.ErrorMessage, "PhotometryAdjustmentDelegate.DecodeDataReport", EventLogEntryType.Error, False)
                '    Exit Try
                'End If

                Select Case pTask
                    Case "TEST"

                        Select Case pAction
                            Case "BL_DC"

                                Dim j As Integer = 0
                                ' Well used for the test
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_WELL_USED", pcurrentLanguage) + ": "
                                text1 += CSng(pData.Substring(j, 3)).ToString("##0")
                                text += myUtility.FormatLineHistorics(text1)
                                j += 3

                                ' Filling option used for the test
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FILL_OPTION", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_AUTOMATIC_FILL", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MANUALLY_FILL", pcurrentLanguage)
                                End If
                                text += myUtility.FormatLineHistorics(text1)
                                j += 1

                                Dim numLeds As Integer
                                numLeds = CInt(pData.Substring(j, 2))
                                j += 2

                                '' Integration Time common (x all Leds)
                                'text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INTEGRATION_TIME", pcurrentLanguage) + ": "
                                'text1 += CSng(pData.Substring(18, 5)).ToString("##,##0")
                                'text += myUtility.FormatLineHistorics(text1)

                                Dim darkPhMain As String
                                Dim darkPhRef As String
                                darkPhMain = CSng(pData.Substring(j, 7)).ToString("#,###,##0")
                                j += 7
                                darkPhRef = CSng(pData.Substring(j, 7)).ToString("#,###,##0")
                                j += 7

                                '' Leds currents of Reference changed
                                'text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_LEDS_CHANGED", pcurrentLanguage) + ": "
                                'If pData.Substring(j, 1) = "1" Then
                                '    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_YES", pcurrentLanguage)
                                'Else
                                '    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NO", pcurrentLanguage)
                                'End If
                                'text += myUtility.FormatLineHistorics(text1)
                                'j += 1

                                For i As Integer = 0 To numLeds - 1

                                    If ReportInfo Is Nothing Then
                                        ReDim ReportInfo(0)
                                    Else
                                        ReDim Preserve ReportInfo(UBound(ReportInfo) + 1)
                                    End If
                                    ReportInfo(UBound(ReportInfo)) = New List(Of String)

                                    ' BaseLine Results 
                                    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_LED", pcurrentLanguage) + " "
                                    text1 += CSng(pData.Substring(j, 3)).ToString("##0") + ":"
                                    ReportInfo(UBound(ReportInfo)).Add(text1)
                                    j += 3

                                    ' PhMain BL Counts 
                                    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode1", pcurrentLanguage) + ": "

                                    ' alignment...
                                    text1 += myUtility.SetSpaces(22 - text1.Length - 1 - CSng(pData.Substring(j, 7)).ToString("#,###,##0").Replace("+", "").Length)
                                    text1 += CSng(pData.Substring(j, 7)).ToString("#,###,##0").Replace("+", "")

                                    ReportInfo(UBound(ReportInfo)).Add(text1)
                                    j += 7

                                    ' PhRef BL Counts 
                                    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode2", pcurrentLanguage) + ": "

                                    ' alignment...
                                    text1 += myUtility.SetSpaces(22 - text1.Length - 1 - CSng(pData.Substring(j, 7)).ToString("#,###,##0").Replace("+", "").Length)
                                    text1 += CSng(pData.Substring(j, 7)).ToString("#,###,##0").Replace("+", "")

                                    ReportInfo(UBound(ReportInfo)).Add(text1)
                                    j += 7

                                    ' Current intensity 
                                    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INTENSITY", pcurrentLanguage) + ": "

                                    ' alignment...
                                    text1 += myUtility.SetSpaces(22 - text1.Length - 1 - CSng(pData.Substring(j, 5)).ToString("##,##0").Replace("+", "").Length)
                                    text1 += CSng(pData.Substring(j, 5)).ToString("##,##0").Replace("+", "")

                                    ReportInfo(UBound(ReportInfo)).Add(text1)
                                    j += 5

                                    ' Warning
                                    If pData.Substring(j, 1) = "1" Then
                                        text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REVIEW_LED", pcurrentLanguage)
                                    Else
                                        text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
                                    End If
                                    ReportInfo(UBound(ReportInfo)).Add(text1)
                                    j += 1

                                Next

                                ' Insert Results of all Leds as a columns and rows
                                Dim k As Integer
                                For i As Integer = 0 To ReportInfo.Length - 1

                                    k = i
                                    text += Environment.NewLine

                                    If k + 2 < ReportInfo.Length Then
                                        ' Insert 3 columns

                                        For t As Integer = 0 To ReportInfo(k).Count - 1
                                            text1 = ReportInfo(k)(t).ToString
                                            text2 = ReportInfo(k + 1)(t).ToString
                                            text3 = ReportInfo(k + 2)(t).ToString
                                            text += myUtility.FormatLineHistorics(text1, text2, text3)
                                        Next

                                        k += 2
                                    ElseIf k + 1 < ReportInfo.Length Then
                                        ' Insert 2 columns

                                        For t As Integer = 0 To ReportInfo(k).Count - 1
                                            text1 = ReportInfo(k)(t).ToString
                                            text2 = ReportInfo(k + 1)(t).ToString
                                            text += myUtility.FormatLineHistorics(text1, text2)
                                        Next
                                        k += 1
                                    Else
                                        ' Insert 1 column

                                        For t As Integer = 0 To ReportInfo(k).Count - 1
                                            text1 = ReportInfo(k)(t).ToString
                                            text += myUtility.FormatLineHistorics(text1)
                                        Next
                                    End If

                                    i = k
                                Next

                                text += Environment.NewLine
                                ' Darkness Results
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_DARKNESS", pcurrentLanguage) + ": "
                                text += myUtility.FormatLineHistorics(text1)

                                ' PhMain Dark Counts Mean
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode1", pcurrentLanguage) + ": "

                                ' alignment...
                                text1 += myUtility.SetSpaces(22 - text1.Length - 1 - darkPhMain.Replace("+", "").Length)
                                text1 += darkPhMain.Replace("+", "")

                                text += myUtility.FormatLineHistorics(text1)
                                ' PhRef Dark Counts Mean
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode2", pcurrentLanguage) + ": "

                                ' alignment...
                                text1 += myUtility.SetSpaces(22 - text1.Length - 1 - darkPhRef.Replace("+", "").Length)
                                text1 += darkPhRef.Replace("+", "")

                                text += myUtility.FormatLineHistorics(text1)

                            Case "REPEAT", "STAB"

                                Dim j As Integer = 0
                                ' Well used for the test
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_WELL_USED", pcurrentLanguage) + ": "
                                text1 += CSng(pData.Substring(j, 3)).ToString("##0")
                                text += myUtility.FormatLineHistorics(text1)
                                j += 3

                                ' Filling option used for the test
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FILL_OPTION", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_AUTOMATIC_FILL", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MANUALLY_FILL", pcurrentLanguage)
                                End If
                                text += myUtility.FormatLineHistorics(text1)
                                j += 1

                                Dim numLeds As Integer
                                numLeds = CInt(pData.Substring(j, 2))
                                j += 2

                                For i As Integer = 0 To numLeds - 1

                                    If ReportInfo Is Nothing Then
                                        ReDim ReportInfo(0)
                                    Else
                                        ReDim Preserve ReportInfo(UBound(ReportInfo) + 1)
                                    End If
                                    ReportInfo(UBound(ReportInfo)) = New List(Of String)

                                    ' Results wavelength i
                                    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_LED", pcurrentLanguage) + " "
                                    text1 += CSng(pData.Substring(j, 3)).ToString("##0") + ":"
                                    ReportInfo(UBound(ReportInfo)).Add(text1)
                                    j += 3

                                    ' LIGHT Results
                                    ' Mean
                                    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Mean", pcurrentLanguage) + ": " 'JB 01/10/2012 - Resource String unification

                                    ' alignment...
                                    text1 += myUtility.SetSpaces(22 - text1.Length - 1 - pData.Substring(j, 7).Replace("+", "").Length)
                                    text1 += pData.Substring(j, 7).ToString().Replace("+", "")

                                    ReportInfo(UBound(ReportInfo)).Add(text1)
                                    j += 7

                                    ' Std. Deviation
                                    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SDevShort", pcurrentLanguage) + ": "

                                    ' alignment...
                                    text1 += myUtility.SetSpaces(22 - text1.Length - 1 - pData.Substring(j, 7).Replace("+", "").Length)
                                    text1 += pData.Substring(j, 7).ToString().Replace("+", "")

                                    ReportInfo(UBound(ReportInfo)).Add(text1)
                                    j += 7

                                    ' Coef. Variation
                                    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CoefShort", pcurrentLanguage) + ": "

                                    ' alignment...
                                    text1 += myUtility.SetSpaces(22 - text1.Length - 1 - pData.Substring(j, 7).Replace("+", "").Length)
                                    text1 += pData.Substring(j, 7).ToString().Replace("+", "")

                                    ReportInfo(UBound(ReportInfo)).Add(text1)
                                    j += 7

                                    ' Max value
                                    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MaxShort", pcurrentLanguage) + ": "

                                    ' alignment...
                                    text1 += myUtility.SetSpaces(22 - text1.Length - 1 - pData.Substring(j, 7).Replace("+", "").Length)
                                    text1 += pData.Substring(j, 7).ToString().Replace("+", "")

                                    ReportInfo(UBound(ReportInfo)).Add(text1)
                                    j += 7

                                    ' Min value
                                    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MinShort", pcurrentLanguage) + ": "

                                    ' alignment...
                                    text1 += myUtility.SetSpaces(22 - text1.Length - 1 - pData.Substring(j, 7).Replace("+", "").Length)
                                    text1 += pData.Substring(j, 7).ToString().Replace("+", "")

                                    ReportInfo(UBound(ReportInfo)).Add(text1)
                                    j += 7

                                    ' Range
                                    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Range", pcurrentLanguage) + ": "

                                    ' alignment...
                                    text1 += myUtility.SetSpaces(22 - text1.Length - 1 - pData.Substring(j, 7).Replace("+", "").Length)
                                    text1 += pData.Substring(j, 7).ToString().Replace("+", "")

                                    ReportInfo(UBound(ReportInfo)).Add(text1)
                                    j += 7
                                Next

                                ' Insert Results of all Leds as a columns and rows
                                Dim k As Integer
                                For i As Integer = 0 To ReportInfo.Length - 1

                                    k = i
                                    text += Environment.NewLine

                                    If k + 2 < ReportInfo.Length Then
                                        ' Insert 3 columns

                                        For t As Integer = 0 To ReportInfo(k).Count - 1
                                            text1 = ReportInfo(k)(t).ToString
                                            text2 = ReportInfo(k + 1)(t).ToString
                                            text3 = ReportInfo(k + 2)(t).ToString
                                            text += myUtility.FormatLineHistorics(text1, text2, text3)
                                        Next

                                        k += 2
                                    ElseIf k + 1 < ReportInfo.Length Then
                                        ' Insert 2 columns

                                        For t As Integer = 0 To ReportInfo(k).Count - 1
                                            text1 = ReportInfo(k)(t).ToString
                                            text2 = ReportInfo(k + 1)(t).ToString
                                            text += myUtility.FormatLineHistorics(text1, text2)
                                        Next
                                        k += 1
                                    Else
                                        ' Insert 1 column

                                        For t As Integer = 0 To ReportInfo(k).Count - 1
                                            text1 = ReportInfo(k)(t).ToString
                                            text += myUtility.FormatLineHistorics(text1)
                                        Next
                                    End If

                                    i = k
                                Next

                                ' Darkness Results

                                text += Environment.NewLine
                                ' Darkness Results
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_DARKNESS", pcurrentLanguage) + ": "
                                text += myUtility.FormatLineHistorics(text1)

                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode1", pcurrentLanguage) + ": "
                                text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode2", pcurrentLanguage) + ": "
                                text += myUtility.FormatLineHistorics(text1, text2)

                                ' Mean dark
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Mean", pcurrentLanguage) + ": " 'JB 01/10/2012 - Resource String unification

                                ' alignment...
                                text1 += myUtility.SetSpaces(22 - text1.Length - 1 - CSng(pData.Substring(j, 7)).ToString("#,###,##0").Replace("+", "").Length)
                                text1 += CSng(pData.Substring(j, 7)).ToString("#,###,##0").Replace("+", "")

                                text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Mean", pcurrentLanguage) + ": " 'JB 01/10/2012 - Resource string unnification

                                ' alignment...
                                text2 += myUtility.SetSpaces(22 - text2.Length - 1 - CSng(pData.Substring(j + 33, 7)).ToString("#,###,##0").Replace("+", "").Length)
                                text2 += CSng(pData.Substring(j + 33, 7)).ToString("#,###,##0").Replace("+", "")

                                text += myUtility.FormatLineHistorics(text1, text2)
                                j += 7

                                ' Std. Deviation dark
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SDevResult", pcurrentLanguage) + ": "

                                ' alignment...
                                text1 += myUtility.SetSpaces(22 - text1.Length - 1 - CSng(pData.Substring(j, 5)).ToString("##,##0").Replace("+", "").Length)
                                text1 += CSng(pData.Substring(j, 5)).ToString("##,##0").Replace("+", "")

                                text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SDevResult", pcurrentLanguage) + ": "

                                ' alignment...
                                text2 += myUtility.SetSpaces(22 - text2.Length - 1 - CSng(pData.Substring(j + 33, 5)).ToString("##,##0").Replace("+", "").Length)
                                text2 += CSng(pData.Substring(j + 33, 5)).ToString("##,##0").Replace("+", "")

                                text += myUtility.FormatLineHistorics(text1, text2)
                                j += 5

                                ' Max value dark
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MaxShort", pcurrentLanguage) + ": "

                                ' alignment...
                                text1 += myUtility.SetSpaces(22 - text1.Length - 1 - CSng(pData.Substring(j, 7)).ToString("#,###,##0").Replace("+", "").Length)
                                text1 += CSng(pData.Substring(j, 7)).ToString("#,###,##0").Replace("+", "")

                                text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MaxShort", pcurrentLanguage) + ": "

                                ' alignment...
                                text2 += myUtility.SetSpaces(22 - text2.Length - 1 - CSng(pData.Substring(j + 33, 7)).ToString("#,###,##0").Replace("+", "").Length)
                                text2 += CSng(pData.Substring(j + 33, 7)).ToString("#,###,##0").Replace("+", "")

                                text += myUtility.FormatLineHistorics(text1, text2)
                                j += 7

                                ' Min value dark
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MinShort", pcurrentLanguage) + ": "

                                ' alignment...
                                text1 += myUtility.SetSpaces(22 - text1.Length - 1 - CSng(pData.Substring(j, 7)).ToString("#,###,##0").Replace("+", "").Length)
                                text1 += CSng(pData.Substring(j, 7)).ToString("#,###,##0").Replace("+", "")

                                text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MinShort", pcurrentLanguage) + ": "

                                ' alignment...
                                text2 += myUtility.SetSpaces(22 - text2.Length - 1 - CSng(pData.Substring(j + 33, 7)).ToString("#,###,##0").Replace("+", "").Length)
                                text2 += CSng(pData.Substring(j + 33, 7)).ToString("#,###,##0").Replace("+", "")

                                text += myUtility.FormatLineHistorics(text1, text2)
                                j += 7

                                ' Range dark
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Range", pcurrentLanguage) + ": "

                                ' alignment...
                                text1 += myUtility.SetSpaces(22 - text1.Length - 1 - CSng(pData.Substring(j, 7)).ToString("#,###,##0").Replace("+", "").Length)
                                text1 += CSng(pData.Substring(j, 7)).ToString("#,###,##0").Replace("+", "")

                                text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Range", pcurrentLanguage) + ": "

                                ' alignment...
                                text2 += myUtility.SetSpaces(22 - text2.Length - 1 - CSng(pData.Substring(j + 33, 7)).ToString("#,###,##0").Replace("+", "").Length)
                                text2 += CSng(pData.Substring(j + 33, 7)).ToString("#,###,##0").Replace("+", "")

                                text += myUtility.FormatLineHistorics(text1, text2)


                            Case "ABS"

                                Dim j As Integer = 0
                                ' Well used for the test
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_WELL_USED", pcurrentLanguage) + ": "
                                text1 += CSng(pData.Substring(j, 3)).ToString("##0")
                                text += myUtility.FormatLineHistorics(text1)
                                j += 3

                                ' Filling option used for the test
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FILL_OPTION", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_AUTOMATIC_FILL", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MANUALLY_FILL", pcurrentLanguage)
                                End If
                                text += myUtility.FormatLineHistorics(text1)
                                j += 1

                                Dim numLeds As Integer
                                numLeds = CInt(pData.Substring(j, 2))
                                j += 2

                                For i As Integer = 0 To numLeds - 1

                                    If ReportInfo Is Nothing Then
                                        ReDim ReportInfo(0)
                                    Else
                                        ReDim Preserve ReportInfo(UBound(ReportInfo) + 1)
                                    End If
                                    ReportInfo(UBound(ReportInfo)) = New List(Of String)

                                    ' Results wavelength i
                                    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_LED", pcurrentLanguage) + " "
                                    text1 += CSng(pData.Substring(j, 3)).ToString("##0") + ":"
                                    ReportInfo(UBound(ReportInfo)).Add(text1)
                                    j += 3

                                    ' LIGHT Results
                                    ' Mean
                                    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Mean", pcurrentLanguage) + ": " 'JB 01/10/2012 - Resource string unification

                                    ' alignment...
                                    text1 += myUtility.SetSpaces(22 - text1.Length - 1 - pData.Substring(j, 7).Replace("+", "").Length)
                                    text1 += pData.Substring(j, 7).ToString().Replace("+", "")

                                    ReportInfo(UBound(ReportInfo)).Add(text1)
                                    j += 7

                                Next

                                ' Insert Results of all Leds as a columns and rows
                                Dim k As Integer
                                For i As Integer = 0 To ReportInfo.Length - 1

                                    k = i
                                    text += Environment.NewLine

                                    If k + 2 < ReportInfo.Length Then
                                        ' Insert 3 columns

                                        For t As Integer = 0 To ReportInfo(k).Count - 1
                                            text1 = ReportInfo(k)(t).ToString
                                            text2 = ReportInfo(k + 1)(t).ToString
                                            text3 = ReportInfo(k + 2)(t).ToString
                                            text += myUtility.FormatLineHistorics(text1, text2, text3)
                                        Next

                                        k += 2
                                    ElseIf k + 1 < ReportInfo.Length Then
                                        ' Insert 2 columns

                                        For t As Integer = 0 To ReportInfo(k).Count - 1
                                            text1 = ReportInfo(k)(t).ToString
                                            text2 = ReportInfo(k + 1)(t).ToString
                                            text += myUtility.FormatLineHistorics(text1, text2)
                                        Next
                                        k += 1
                                    Else
                                        ' Insert 1 column

                                        For t As Integer = 0 To ReportInfo(k).Count - 1
                                            text1 = ReportInfo(k)(t).ToString
                                            text += myUtility.FormatLineHistorics(text1)
                                        Next
                                    End If

                                    i = k
                                Next

                        End Select
                End Select

                ' XBC 13/09/2011 - put separator as a group thousands separator
                text = text.Replace(MyClass.myGroupSeparator.ToString, " ")

                myResultData.SetDatos = text

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.DecodeDataReport", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Refresh this specified delegate with the information received from the Instrument
        ''' </summary>
        ''' <param name="pRefreshEventType"></param>
        ''' <param name="pRefreshDS"></param>
        ''' <remarks>Created by XBC 20/02/2012</remarks>
        Public Sub RefreshDelegate(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As UIRefreshDS)
            Dim myResultData As New GlobalDataTO
            Try
                MyClass.ScreenReceptionLastFwScriptEvent(RESPONSE_TYPES.OK, Nothing)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.RefreshDelegate", EventLogEntryType.Error, False)
            End Try
        End Sub

#End Region

#Region "Private Methods"

#Region "Sending Instructions"
        ''' <summary>
        ''' Creates the Script List for Screen Testing operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 28/02/2011
        ''' Modified by XB 13/10/2014 - new photometry adjustment maneuver (use REAGENTS_ABS_ROTOR (with parameter = current value of GFWR1) instead of REACTIONS_ROTOR_HOME_WELL1) - BA-1953
        '''             XB 31/10/2014 - Use FAC because Firmware makes the fine auto-adjustment - BA-2058
        ''' </remarks>
        Private Function SendQueueForTESTING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myListFwScript As New List(Of FwScriptQueueItem)
            Dim myFwScript1 As New FwScriptQueueItem
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'get the pending Homes  
                Dim myHomes As New tadjPreliminaryHomesDAO
                Dim myHomesDS As SRVPreliminaryHomesDS
                myResultData = myHomes.GetPreliminaryHomesByAdjID(Nothing, AnalyzerId, pAdjustment.ToString)
                If myResultData IsNot Nothing AndAlso Not myResultData.HasError Then
                    myHomesDS = CType(myResultData.SetDatos, SRVPreliminaryHomesDS)

                    Dim myPendingHomesList As List(Of SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow) = _
                                    (From a As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myHomesDS.srv_tadjPreliminaryHomes _
                                    Where a.Done = False Select a).ToList

                    For Each H As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myPendingHomesList
                        Dim myFwScript As New FwScriptQueueItem
                        myListFwScript.Add(myFwScript)
                    Next

                    Dim i As Integer = 0
                    For Each H As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myPendingHomesList
                        'GET EACH PENDING HOME'S FWSCRIPT FROM FWSCRIPT DATA AND ADD TO THE FWSCRIPT QUEUE
                        If i = myListFwScript.Count - 1 Then
                            'Last index
                            With myListFwScript(i)
                                .FwScriptID = H.RequiredHomeID.ToString
                                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                                .EvaluateValue = 1
                                .NextOnResultOK = myFwScript1   ' XB 13/10/2014 - BA-1953
                                .NextOnResultNG = Nothing
                                .NextOnTimeOut = myFwScript1    ' XB 13/10/2014 - BA-1953
                                .NextOnError = Nothing
                                .ParamList = Nothing
                            End With
                        Else
                            With myListFwScript(i)
                                .FwScriptID = H.RequiredHomeID.ToString
                                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                                .EvaluateValue = 1
                                .NextOnResultOK = myListFwScript(i + 1)
                                .NextOnResultNG = Nothing
                                .NextOnTimeOut = myListFwScript(i + 1)
                                .NextOnError = Nothing
                                .ParamList = Nothing
                            End With
                        End If
                        i += 1
                    Next

                End If

                Select Case pAdjustment
                    Case ADJUSTMENT_GROUPS.PHOTOMETRY

                        ' XB 13/10/2014 - BA-1953
                        'If myListFwScript.Count > 0 Then
                        '    CurrentOperation = OPERATIONS.HOMES
                        '    For i As Integer = 0 To myListFwScript.Count - 1
                        '        If i = 0 Then
                        '            ' First Script
                        '            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
                        '        Else
                        '            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
                        '        End If
                        '    Next
                        'Else
                        '    MyClass.HomesDoneAttr = True
                        '    MyClass.NoneInstructionToSend = False
                        'End If

                        ' After home move abs to the current value of adjustment GFWR1
                        With myFwScript1
                            .FwScriptID = FwSCRIPTS_IDS.REACTIONS_ROTOR_HOME_WELL1.ToString    ' FwSCRIPTS_IDS.REACTIONS_ABS_ROTOR.ToString ' XB 31/10/2014 - BA-2058
                            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                            .EvaluateValue = 1
                            .NextOnResultOK = Nothing
                            .NextOnResultNG = Nothing
                            .NextOnTimeOut = Nothing
                            .NextOnError = Nothing
                            ' XB 31/10/2014 - BA-2058
                            '' expects 1 param
                            '.ParamList = New List(Of String)
                            '.ParamList.Add(Me.pValueAdjustAttr)
                            .ParamList = Nothing
                        End With

                        ' Send Preliminary Homes
                        If myListFwScript.Count > 0 Then
                            CurrentOperation = OPERATIONS.HOMES
                            For i As Integer = 0 To myListFwScript.Count - 1
                                If i = 0 Then
                                    ' First Script
                                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
                                Else
                                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
                                End If
                            Next
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                        Else
                            MyClass.HomesDoneAttr = True
                            MyClass.NoneInstructionToSend = False
                        End If
                        ' XB 13/10/2014 - BA-1953

                        'Select Case MyClass.FillMode
                        '    'Case FILL_MODE.MANUAL
                        '    '    ' Rotor is auto filled and then ready to start BLDC test
                        '    '    myResultData = SendREAD_COUNTS()

                        '    Case FILL_MODE.AUTOMATIC
                        '        If Not MyClass.EmptyWell(MyClass.WellToUse - 1) Then  ' real wells 1-120 -> for EmptyWell (0-119) 
                        '            ' well already filled !
                        '        End If

                        '        If Not MyClass.HomesDoneAttr Then
                        '            ' selected well must be filled
                        '            CurrentOperation = OPERATIONS.HOMES
                        '            ' Fill selected well automatically  ' cóm funciona ???
                        '            With myFwScript1
                        '                .FwScriptID = FwSCRIPTS_IDS.FILL_WELLS.ToString
                        '                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                        '                .EvaluateValue = 1
                        '                .NextOnResultOK = Nothing
                        '                .NextOnResultNG = Nothing
                        '                .NextOnTimeOut = Nothing
                        '                .NextOnError = Nothing
                        '                ' expects 1 param
                        '                .ParamList = New List(Of String)
                        '                .ParamList.Add(MyClass.WellToUse.ToString)
                        '            End With

                        '            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
                        '        End If
                        'End Select

                        'Case ADJUSTMENT_GROUPS.IT_EDITION
                        '    If Not MyClass.ITsEditionFirstPartDone Then
                        '        ' Send the first operation to complete Leds intensities reading
                        '        ' Read References Leds intensities
                        '        CurrentOperation = OPERATIONS.READ_LEDS

                        '        'Script1
                        '        With myFwScript1
                        '            .FwScriptID = FwSCRIPTS_IDS.READ_LEDS.ToString
                        '            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                        '            .EvaluateValue = 1
                        '            .NextOnResultOK = Nothing
                        '            .NextOnResultNG = Nothing
                        '            .NextOnTimeOut = Nothing
                        '            .NextOnError = Nothing
                        '            .ParamList = Nothing
                        '        End With

                        '        'add to the queue list
                        '        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

                        '    ElseIf Not MyClass.ITsEditionDone Then

                        '        ' Ref Leds intesities already readed and ready to read current Leds Intensities
                        '        CurrentOperation = OPERATIONS.READ_COUNTS

                        '        ' Configure the times when a Read Counts request is sent
                        '        With myFwScript1
                        '            .FwScriptID = FwSCRIPTS_IDS.READ_BASELINE.ToString  ' Canviar per BLIGHT !!!
                        '            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                        '            .EvaluateValue = 1
                        '            .NextOnResultOK = Nothing
                        '            .NextOnResultNG = Nothing
                        '            .NextOnTimeOut = Nothing
                        '            .NextOnError = Nothing
                        '            ' expects 2 params
                        '            .ParamList = New List(Of String)
                        '            .ParamList.Add(MyClass.WellToUse.ToString)   ' WellToUse ??? 
                        '        End With

                        '        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

                        '    End If

                        'Case ADJUSTMENT_GROUPS.REPEATABILITY, _
                        '     ADJUSTMENT_GROUPS.STABILITY, _
                        '     ADJUSTMENT_GROUPS.ABSORBANCE_MEASUREMENT

                        '    If MyClass.FillMode = FILL_MODE.AUTOMATIC Then

                        '        ' Rotor is auto filled and then ready to start BLDC test
                        '        CurrentOperation = OPERATIONS.READ_COUNTS

                        '        ' Configure the times when a Read Counts request is sent
                        '        With myFwScript1
                        '            .FwScriptID = FwSCRIPTS_IDS.READ_BASELINE.ToString  '  Canviar per BLIGHT + param AutoFill !!!
                        '            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                        '            .EvaluateValue = 1
                        '            .NextOnResultOK = Nothing
                        '            .NextOnResultNG = Nothing
                        '            .NextOnTimeOut = Nothing
                        '            .NextOnError = Nothing
                        '            ' expects 2 params
                        '            .ParamList = New List(Of String)
                        '            .ParamList.Add(MyClass.WellToUse.ToString)
                        '            ' Afegir paràmetres de timings per instruccions REP i STA (ReadingTime i ReadingStabilityOffsetTime ) !!!
                        '        End With

                        '        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

                        '    Else
                        '        If Not MyClass.EmptyWell(MyClass.WellToUse - 1) Then  ' real wells 1-120 -> for EmptyWell (0-119) 
                        '            ' well already filled !
                        '            MyClass.HomesDoneAttr = True
                        '        End If

                        '        If Not MyClass.HomesDoneAttr Then
                        '            ' selected well must be filled
                        '            CurrentOperation = OPERATIONS.HOMES
                        '            ' Fill selected well automatically  '  cóm funciona ???
                        '            With myFwScript1
                        '                .FwScriptID = FwSCRIPTS_IDS.FILL_WELLS.ToString
                        '                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                        '                .EvaluateValue = 1
                        '                .NextOnResultOK = Nothing
                        '                .NextOnResultNG = Nothing
                        '                .NextOnTimeOut = Nothing
                        '                .NextOnError = Nothing
                        '                ' expects 1 param
                        '                .ParamList = New List(Of String)
                        '                .ParamList.Add(MyClass.WellToUse.ToString)
                        '            End With

                        '            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

                        '        Else

                        '            ' Repeatability/Stability Tests
                        '            ' Rotor is already filled and ready to send Read Counts
                        '            CurrentOperation = OPERATIONS.READ_COUNTS

                        '            ' Configure the times when a Read Counts request is sent
                        '            With myFwScript1
                        '                .FwScriptID = FwSCRIPTS_IDS.READ_BASELINE.ToString  '  Canviar per BLIGHT NOVA !!!
                        '                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                        '                .EvaluateValue = 1
                        '                .NextOnResultOK = Nothing
                        '                .NextOnResultNG = Nothing
                        '                .NextOnTimeOut = Nothing
                        '                .NextOnError = Nothing
                        '                ' expects 2 params
                        '                .ParamList = New List(Of String)
                        '                .ParamList.Add("0")   '  compte que no cal passar el WL ja que la nova ALIGHT per això ho testejarà/retornarà amb tots els WLs !!!
                        '                .ParamList.Add(MyClass.WellToUse.ToString)
                        '                '  Afegir paràmetres de timings per instruccions REP i STA (ReadingTime i ReadingStabilityOffsetTime ) !!!
                        '            End With

                        '            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

                        '        End If

                        '    End If

                        'Case ADJUSTMENT_GROUPS.CHECK_ROTOR
                        ' CANCELLED

                End Select

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.SendQueueForTESTING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List for Exiting operation of specified Test
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 15/03/2011
        ''' </remarks>
        Private Function SendQueueForTEST_EXITING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                ' Pending on future requirements

                'If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                '    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                'End If


                'Select Case pAdjustment                  '  operacions per sortir de cada test en concret ???
                '    Case ADJUSTMENT_GROUPS.BASELINE, _
                '         ADJUSTMENT_GROUPS.DARKNESS_COUNTS, _
                '         ADJUSTMENT_GROUPS.REPEATABILITY, _
                '         ADJUSTMENT_GROUPS.STABILITY, _
                '         ADJUSTMENT_GROUPS.ABSORBANCE_MEASUREMENT
                '        'ADJUSTMENT_GROUPS.CHECK_ROTOR

                '        Dim myFwScript1 As New FwScriptQueueItem

                '        CurrentOperation = OPERATIONS.REAGENTS_HOME_ROTOR
                '        'Script1
                '        With myFwScript1
                '            .FwScriptID = FwSCRIPTS_IDS. ??? .ToString     
                '            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                '            .EvaluateValue = 1
                '            .NextOnResultOK = Nothing
                '            .NextOnResultNG = Nothing
                '            .NextOnTimeOut = Nothing
                '            .NextOnError = Nothing
                '            .ParamList = Nothing
                '        End With

                '        'add to the queue list
                '        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

                'End Select

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.SendQueueForTEST_EXITING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Read Counts High Level Instruction
        ''' </summary>
        ''' <param name="pAdjustmentGroup"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SendREAD_COUNTS(ByVal pAdjustmentGroup As ADJUSTMENT_GROUPS, Optional ByVal pFastReading As Boolean = False) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myFillMode As Integer
            Dim myParams As New List(Of String)
            Try
                If MyClass.CurrentTestAttr <> pAdjustmentGroup Then
                    If MyClass.CurrentTestAttr <> ADJUSTMENT_GROUPS._NONE Then
                        MyClass.PreviousTestAttr = MyClass.CurrentTestAttr
                    End If
                    MyClass.CurrentTestAttr = pAdjustmentGroup
                End If

                myFillMode = MyClass.FillModeAttr
                'Select Case MyClass.FillModeAttr
                '    Case FILL_MODE.AUTOMATIC
                '        ' Weel filled by Instrument With Washing Station
                '        myFillMode = 1
                '    Case FILL_MODE.MANUAL
                '        ' Well filled by user
                '        myFillMode = 0
                'End Select


                CurrentOperation = OPERATIONS.READ_COUNTS
                ' XBC 21/02/2012 - next measures have well already placed and need to execute BLIGHT fastest
                If pFastReading Then
                    myParams.Add("0")
                    myParams.Add("0")
                Else
                    If Not MyClass.EmptyWell(MyClass.WellToUse - 1) Then  ' real wells 1-120 -> for EmptyWell (0-119) 
                        ' well already filled !
                        myFillMode = 0
                    End If
                    myParams.Add(MyClass.WellToUse.ToString)
                    myParams.Add(myFillMode.ToString)
                End If
                ' XBC 21/02/2012 - next measures have well already placed and need to execute BLIGHT fastest


                ' XBC 20/02/2012
                If pAdjustmentGroup = ADJUSTMENT_GROUPS.PHOTOMETRY Then
                    'Once the well is ready to adjust Photometric values the Sw send an ALIGHT instruction 
                    myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_LIGHT, True, Nothing, Nothing, "", myParams)
                Else
                    'Once the well is ready to read Photometric Counts the Sw send an BLIGHT instruction 
                    myResultData = myFwScriptDelegate.AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_BLIGHT, True, Nothing, Nothing, "", myParams)
                End If
                ' XBC 20/02/2012

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.SendREAD_COUNTS", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function
#End Region

#Region "Reading Counts"
        ''' <summary> 
        ''' Routine called after loading the test of LB to manage the WaveLengths obtained from the Instrument
        ''' </summary>
        ''' <returns>GlobalDataTO containing a Typed Dataset AnalyzerLedPositionsDS with the list of WaveLength items</returns>
        ''' <remarks>
        ''' Created by XBC 28/02/2011
        ''' Modified by XBC 01/08/2011 - Add registry functionality into historical activities
        ''' </remarks>
        Private Function ManageResultsBL() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                myResultData = MeasureCurrentLEDSReaded()
                If myResultData.HasError Then
                    Exit Try
                End If

                ' Insert the new activity into Historic reports
                myResultData = InsertReport("TEST", "BL_DC")

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.ManageResultsBL", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary> 
        ''' Routine called after every readed counts for the Repeatability test to store them
        ''' </summary>
        ''' <returns>GlobalDataTO containing a Typed Dataset AnalyzerLedPositionsDS with the list of WaveLength items</returns>
        ''' <remarks>Created by : XBC 08/03/2011</remarks>
        Private Function MeasureRepeatabilityReadedCounts() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myPhotometryDataTO As PhotometryDataTO
            Try
                myResultData = myFwScriptDelegate.AnalyzerManager.ReadPhotometryData
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myPhotometryDataTO = CType(myResultData.SetDatos, PhotometryDataTO)

                    For i As Integer = 0 To MyClass.TestReadedCountsAttr.Count - 1
                        MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphMainCounts.Add(myPhotometryDataTO.CountsMainBaseline(i))
                        MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphRefCounts.Add(myPhotometryDataTO.CountsRefBaseline(i))

                        MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphMainCountsDK.Add(myPhotometryDataTO.CountsMainDarkness(i))
                        MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphRefCountsDK.Add(myPhotometryDataTO.CountsRefDarkness(i))
                    Next
                Else
                    myResultData.HasError = True
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.MeasureRepeatabilityReadedCounts", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary> 
        ''' Routine called after every readed counts for the Stability test to store them
        ''' </summary>
        ''' <returns>GlobalDataTO containing a Typed Dataset AnalyzerLedPositionsDS with the list of WaveLength items</returns>
        ''' <remarks>Created by : XBC 08/03/2011</remarks>
        Private Function MeasureStabilityReadedCounts() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myPhotometryDataTO As PhotometryDataTO
            Try
                myResultData = myFwScriptDelegate.AnalyzerManager.ReadPhotometryData
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myPhotometryDataTO = CType(myResultData.SetDatos, PhotometryDataTO)

                    For i As Integer = 0 To MyClass.TestReadedCountsAttr.Count - 1
                        MyClass.TestReadedCountsAttr(i).MeasuresStabilityphMainCounts.Add(myPhotometryDataTO.CountsMainBaseline(i))
                        MyClass.TestReadedCountsAttr(i).MeasuresStabilityphRefCounts.Add(myPhotometryDataTO.CountsRefBaseline(i))

                        MyClass.TestReadedCountsAttr(i).MeasuresStabilityphMainCountsDK.Add(myPhotometryDataTO.CountsMainDarkness(i))
                        MyClass.TestReadedCountsAttr(i).MeasuresStabilityphRefCountsDK.Add(myPhotometryDataTO.CountsRefDarkness(i))
                    Next
                Else
                    myResultData.HasError = True
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.MeasureStabilityReadedCounts", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary> 
        ''' Routine called after current Leds intesities readed
        ''' </summary>
        ''' <returns>GlobalDataTO containing a Typed Dataset AnalyzerLedPositionsDS with the list of WaveLength items</returns>
        ''' <remarks>Created by : XBC 15/03/2011</remarks>
        Private Function MeasureCurrentLEDSReaded() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myPhotometryDataTO As PhotometryDataTO
            Try
                myResultData = myFwScriptDelegate.AnalyzerManager.ReadPhotometryData
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myPhotometryDataTO = CType(myResultData.SetDatos, PhotometryDataTO)

                    MyClass.CurrentLEDsIntensitiesAttr = New List(Of Single)
                    For i As Integer = 0 To myPhotometryDataTO.LEDsIntensities.Count - 1
                        MyClass.CurrentLEDsIntensitiesAttr.Add(myPhotometryDataTO.LEDsIntensities(i))
                    Next
                Else
                    myResultData.HasError = True
                End If
            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.MeasureCurrentLEDSReaded", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Method incharge to get the controls limits value. 
        ''' </summary>
        ''' <param name="pLimitsID">Limit to get</param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 30/03/2011</remarks>
        Private Function GetControlsLimits(ByVal pLimitsID As FieldLimitsEnum) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myFieldLimitsDS As New FieldLimitsDS
            Try
                Dim myFieldLimitsDelegate As New FieldLimitsDelegate()
                'Load the specified limits
                myResultData = myFieldLimitsDelegate.GetList(Nothing, pLimitsID)
            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.GetControlsLimits", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function
#End Region

#Region "Calculations"

        '''' <summary> 
        '''' Routine called after every readed counts for the Verification Rotor test to store them
        '''' </summary>
        '''' <returns>GlobalDataTO containing a Typed Dataset AnalyzerLedPositionsDS with the list of WaveLength items</returns>
        '''' <remarks>Created by : XBC 10/03/2011</remarks>
        'Private Function MeasureCheckRotorReadedCounts() As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Dim myPhotometryDataTO As PhotometryDataTO
        '    Try

        '        myResultData = myFwScriptDelegate.AnalyzerManager.ReadPhotometryData
        '        If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
        '            myPhotometryDataTO = CType(myResultData.SetDatos, PhotometryDataTO)

        '            MyClass.MeasuresCheckRotorCountsAttr.Add(myPhotometryDataTO.CountsMainBaseline(MyClass.WLSToUse))    
        '        Else
        '            myResultData.HasError = True
        '        End If

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.MeasureCheckRotorReadedCounts", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function

        ''' <summary> 
        ''' Routine called when the test of Repeatability/Stability is finished and to manage the WaveLengths obtained from the Instrument
        ''' </summary>
        ''' <returns>GlobalDataTO containing a Typed Dataset AnalyzerLedPositionsDS with the list of WaveLength items</returns>
        ''' <remarks>Created by : XBC 08/03/2011</remarks>
        Private Function ManageTestResults() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim resultValue As Single
            Dim myCalc As New CalculationsDelegate()
            Try
                ' initializations
                resultValue = 0

                '
                ' READING COUNTS
                '
                Select Case MyClass.CurrentTestAttr
                    Case ADJUSTMENT_GROUPS.REPEATABILITY
                        For i As Integer = 0 To TestReadedCountsAttr.Count - 1
                            For j As Integer = 0 To MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphMainCounts.Count - 1
                                ' Calculate absorbances

                                myResultData = myCalc.CalculateAbsorbance(CInt(MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphMainCounts(j)), _
                                                                          CInt(MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphRefCounts(j)), _
                                                                          CInt(MyClass.BaseLineMainCountsAttr(i)), _
                                                                          CInt(MyClass.BaseLineRefCountsAttr(i)), _
                                                                          CInt(MyClass.DarkphMainCountAttr), _
                                                                          CInt(MyClass.DarkphRefCountAttr), _
                                                                          0, _
                                                                          MyClass.PathLength, _
                                                                          MyClass.LimitAbs, _
                                                                          False, _
                                                                          MyClass.DilutionFactor)

                                resultValue = GlobalConstants.CALCULATION_ERROR_VALUE     ' XBC 09/11/2012
                                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                                    resultValue = CType(myResultData.SetDatos, Single)
                                End If
                                MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityAbsorbances.Add(resultValue)
                            Next
                        Next
                    Case ADJUSTMENT_GROUPS.STABILITY
                        For i As Integer = 0 To TestReadedCountsAttr.Count - 1
                            For j As Integer = 0 To MyClass.TestReadedCountsAttr(i).MeasuresStabilityphMainCounts.Count - 1
                                ' Calculate absorbances

                                myResultData = myCalc.CalculateAbsorbance(CInt(MyClass.TestReadedCountsAttr(i).MeasuresStabilityphMainCounts(j)), _
                                                                          CInt(MyClass.TestReadedCountsAttr(i).MeasuresStabilityphRefCounts(j)), _
                                                                          CInt(MyClass.BaseLineMainCountsAttr(i)), _
                                                                          CInt(MyClass.BaseLineRefCountsAttr(i)), _
                                                                          CInt(MyClass.DarkphMainCountAttr), _
                                                                          CInt(MyClass.DarkphRefCountAttr), _
                                                                          0, _
                                                                          MyClass.PathLength, _
                                                                          MyClass.LimitAbs, _
                                                                          False, _
                                                                          MyClass.DilutionFactor)

                                resultValue = GlobalConstants.CALCULATION_ERROR_VALUE     ' XBC 09/11/2012
                                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                                    resultValue = CType(myResultData.SetDatos, Single)
                                End If
                                MyClass.TestReadedCountsAttr(i).MeasuresStabilityAbsorbances.Add(resultValue)
                            Next
                        Next
                        'Case ADJUSTMENT_GROUPS.CHECK_ROTOR
                        '    For i As Integer = 0 To MyClass.MeasuresCheckRotorCountsAttr.Count - 1 
                        '        ' Calculate absorbances
                        '        myResultData = CalculateAbsorbance(MyClass.MeasuresCheckRotorCountsAttr(i), (MyClass.BaseLineCountsAttr(MyClass.WLSToUse) - MyClass.DarkphMainCountAttr), i + 1) 
                        '        If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                        '            resultValue = CType(myResultData.SetDatos, Single)
                        '            MeasuresCheckRotorAbsorbancesAttr.Add(resultValue)
                        '        End If
                        '    Next
                End Select

                'If Not myResultData.HasError Then
                '
                ' COUNTS AVERAGE
                '
                Select Case MyClass.CurrentTestAttr
                    Case ADJUSTMENT_GROUPS.REPEATABILITY
                        For i As Integer = 0 To TestReadedCountsAttr.Count - 1
                            If MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphMainCountsDK.Count > 0 Then
                                MyClass.ResultsRepeatabilityTest(i).CountsPhMainDK = MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphMainCountsDK.Average()
                            Else
                                MyClass.ResultsRepeatabilityTest(i).CountsPhMainDK = 0
                            End If
                            If MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphRefCountsDK.Count > 0 Then
                                MyClass.ResultsRepeatabilityTest(i).CountsPhRefDK = MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphRefCountsDK.Average()
                            Else
                                MyClass.ResultsRepeatabilityTest(i).CountsPhRefDK = 0
                            End If
                        Next
                    Case ADJUSTMENT_GROUPS.STABILITY
                        For i As Integer = 0 To TestReadedCountsAttr.Count - 1
                            If MyClass.TestReadedCountsAttr(i).MeasuresStabilityphMainCountsDK.Count > 0 Then
                                MyClass.ResultsStabilityTest(i).CountsPhMainDK = MyClass.TestReadedCountsAttr(i).MeasuresStabilityphMainCountsDK.Average()
                            Else
                                MyClass.ResultsStabilityTest(i).CountsPhMainDK = 0
                            End If
                            If MyClass.TestReadedCountsAttr(i).MeasuresStabilityphRefCountsDK.Count > 0 Then
                                MyClass.ResultsStabilityTest(i).CountsPhRefDK = MyClass.TestReadedCountsAttr(i).MeasuresStabilityphRefCountsDK.Average()
                            Else
                                MyClass.ResultsStabilityTest(i).CountsPhRefDK = 0
                            End If
                        Next
                        'Case ADJUSTMENT_GROUPS.CHECK_ROTOR
                        '    ' Nothing 
                End Select

                '
                ' ABSORBANCE AVERAGE
                '
                Select Case MyClass.CurrentTestAttr
                    Case ADJUSTMENT_GROUPS.REPEATABILITY
                        For i As Integer = 0 To TestReadedCountsAttr.Count - 1
                            If MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityAbsorbances.Count > 0 Then
                                MyClass.ResultsRepeatabilityTest(i).Absorbance = MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityAbsorbances.Average()
                            Else
                                MyClass.ResultsRepeatabilityTest(i).Absorbance = 0
                            End If
                        Next
                    Case ADJUSTMENT_GROUPS.STABILITY
                        For i As Integer = 0 To TestReadedCountsAttr.Count - 1
                            If MyClass.TestReadedCountsAttr(i).MeasuresStabilityAbsorbances.Count > 0 Then
                                MyClass.ResultsStabilityTest(i).Absorbance = MyClass.TestReadedCountsAttr(i).MeasuresStabilityAbsorbances.Average()
                            Else
                                MyClass.ResultsStabilityTest(i).Absorbance = 0
                            End If
                        Next
                        'Case ADJUSTMENT_GROUPS.CHECK_ROTOR
                        '    MyClass.ResultsStabilityTest.Absorbance = MeasuresCheckRotorAbsorbancesAttr.Average()
                End Select

                '
                ' STANDARD DEVIATION DARKNESS COUNTS
                '
                Select Case MyClass.CurrentTestAttr
                    Case ADJUSTMENT_GROUPS.REPEATABILITY
                        For i As Integer = 0 To TestReadedCountsAttr.Count - 1
                            ' Ph Main
                            If MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphMainCountsDK.Count > 0 Then
                                myResultData = myCalc.CalculateStdDeviation(MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphMainCountsDK)
                                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                                    MyClass.ResultsRepeatabilityTest(i).STDDeviationPhMainDK = CType(myResultData.SetDatos, Single)
                                Else
                                    MyClass.ResultsRepeatabilityTest(i).STDDeviationPhMainDK = 99999
                                    myResultData.HasError = False
                                End If
                            Else
                                MyClass.ResultsRepeatabilityTest(i).STDDeviationPhMainDK = 99999
                            End If
                            ' Ph Ref
                            If MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphRefCountsDK.Count > 0 Then
                                myResultData = myCalc.CalculateStdDeviation(MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphRefCountsDK)
                                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                                    MyClass.ResultsRepeatabilityTest(i).STDDeviationPhRefDK = CType(myResultData.SetDatos, Single)
                                Else
                                    MyClass.ResultsRepeatabilityTest(i).STDDeviationPhRefDK = 99999
                                    myResultData.HasError = False
                                End If
                            Else
                                MyClass.ResultsRepeatabilityTest(i).STDDeviationPhRefDK = 99999
                            End If
                        Next
                    Case ADJUSTMENT_GROUPS.STABILITY
                        For i As Integer = 0 To TestReadedCountsAttr.Count - 1
                            ' Ph Main
                            If MyClass.TestReadedCountsAttr(i).MeasuresStabilityphMainCountsDK.Count > 0 Then
                                myResultData = myCalc.CalculateStdDeviation(MyClass.TestReadedCountsAttr(i).MeasuresStabilityphMainCountsDK)
                                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                                    MyClass.ResultsStabilityTest(i).STDDeviationPhMainDK = CType(myResultData.SetDatos, Single)
                                Else
                                    MyClass.ResultsStabilityTest(i).STDDeviationPhMainDK = 99999
                                    myResultData.HasError = False
                                End If
                            Else
                                MyClass.ResultsStabilityTest(i).STDDeviationPhMainDK = 99999
                            End If
                            ' Ph Ref
                            If MyClass.TestReadedCountsAttr(i).MeasuresStabilityphRefCountsDK.Count > 0 Then
                                myResultData = myCalc.CalculateStdDeviation(MyClass.TestReadedCountsAttr(i).MeasuresStabilityphRefCountsDK)
                                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                                    MyClass.ResultsStabilityTest(i).STDDeviationPhRefDK = CType(myResultData.SetDatos, Single)
                                Else
                                    MyClass.ResultsStabilityTest(i).STDDeviationPhRefDK = 99999
                                    myResultData.HasError = False
                                End If
                            Else
                                MyClass.ResultsStabilityTest(i).STDDeviationPhRefDK = 99999
                            End If
                        Next
                        'Case ADJUSTMENT_GROUPS.CHECK_ROTOR
                        '    ' Nothing 
                End Select

                '
                ' STANDARD DEVIATION ABSORBANCE
                '
                Select Case MyClass.CurrentTestAttr
                    Case ADJUSTMENT_GROUPS.REPEATABILITY
                        For i As Integer = 0 To TestReadedCountsAttr.Count - 1
                            If MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityAbsorbances.Count > 0 Then
                                myResultData = myCalc.CalculateStdDeviation(MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityAbsorbances)
                                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                                    MyClass.ResultsRepeatabilityTest(i).STDDeviationAbsorbance = CType(myResultData.SetDatos, Single)
                                Else
                                    MyClass.ResultsRepeatabilityTest(i).STDDeviationAbsorbance = 9.9999
                                    myResultData.HasError = False
                                End If
                            Else
                                MyClass.ResultsRepeatabilityTest(i).STDDeviationAbsorbance = 9.9999
                            End If
                        Next
                    Case ADJUSTMENT_GROUPS.STABILITY
                        For i As Integer = 0 To TestReadedCountsAttr.Count - 1
                            If MyClass.TestReadedCountsAttr(i).MeasuresStabilityAbsorbances.Count > 0 Then
                                myResultData = myCalc.CalculateStdDeviation(MyClass.TestReadedCountsAttr(i).MeasuresStabilityAbsorbances)
                                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                                    MyClass.ResultsStabilityTest(i).STDDeviationAbsorbance = CType(myResultData.SetDatos, Single)
                                Else
                                    MyClass.ResultsStabilityTest(i).STDDeviationAbsorbance = 9.9999
                                    myResultData.HasError = False
                                End If
                            Else
                                MyClass.ResultsStabilityTest(i).STDDeviationAbsorbance = 9.9999
                            End If
                        Next
                        'Case ADJUSTMENT_GROUPS.CHECK_ROTOR
                        '    myResultData = CalculateStdDeviation(MeasuresCheckRotorAbsorbancesAttr)
                        '    If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                        '        MyClass.ResultsCheckRotorTest.AbsorbanceDeviation = CType(myResultData.SetDatos, Single)
                        '    Else
                        '        MyClass.ResultsCheckRotorTest.AbsorbanceDeviation = 9.9999
                        '    End If
                End Select

                '
                ' VARIATION COEFICIENT ABSORBANCE
                '
                Select Case MyClass.CurrentTestAttr
                    Case ADJUSTMENT_GROUPS.REPEATABILITY
                        For i As Integer = 0 To TestReadedCountsAttr.Count - 1
                            myResultData = myCalc.CalculateVariationCoefficient(MyClass.ResultsRepeatabilityTest(i).STDDeviationAbsorbance, _
                                                                                Abs(MyClass.ResultsRepeatabilityTest(i).Absorbance))
                            If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                                MyClass.ResultsRepeatabilityTest(i).CVAbsorbance = CType(myResultData.SetDatos, Single)
                            Else
                                MyClass.ResultsRepeatabilityTest(i).CVAbsorbance = 9999
                                myResultData.HasError = False
                            End If
                        Next
                    Case ADJUSTMENT_GROUPS.STABILITY
                        For i As Integer = 0 To TestReadedCountsAttr.Count - 1
                            myResultData = myCalc.CalculateVariationCoefficient(MyClass.ResultsStabilityTest(i).STDDeviationAbsorbance, _
                                                                                Abs(MyClass.ResultsStabilityTest(i).Absorbance))
                            If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                                MyClass.ResultsStabilityTest(i).CVAbsorbance = CType(myResultData.SetDatos, Single)
                            Else
                                MyClass.ResultsStabilityTest(i).CVAbsorbance = 9999
                                myResultData.HasError = False
                            End If
                        Next
                End Select


                ' Insert the new activity into Historic reports
                Select Case MyClass.CurrentTestAttr
                    Case ADJUSTMENT_GROUPS.REPEATABILITY
                        myResultData = InsertReport("TEST", "REPEAT")
                    Case ADJUSTMENT_GROUPS.STABILITY
                        myResultData = InsertReport("TEST", "STAB")
                End Select

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.ManageTestResults", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary> 
        ''' Routine called when the test of Absorbance Measurement is finished and to manage the Counts obtained from the Instrument
        ''' </summary>
        ''' <returns>GlobalDataTO containing a Typed Dataset AnalyzerLedPositionsDS with the list of WaveLength items</returns>
        ''' <remarks>Created by : XBC 10/03/2011</remarks>
        Private Function ManageABSTestResults(ByVal SimulateTest As Boolean) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myPhotometryDataTO As PhotometryDataTO
            Dim resultValue As Single
            Dim myCalc As New CalculationsDelegate()
            Try
                If Not SimulateTest Then
                    myResultData = myFwScriptDelegate.AnalyzerManager.ReadPhotometryData
                    If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                        myPhotometryDataTO = CType(myResultData.SetDatos, PhotometryDataTO)

                        For i As Integer = 0 To TestReadedCountsAttr.Count - 1
                            ' Reading Counts result by WL
                            MyClass.TestReadedCountsAttr(i).MeasureABSphMainCount = myPhotometryDataTO.CountsMainBaseline(i)
                            MyClass.TestReadedCountsAttr(i).MeasureABSphRefCount = myPhotometryDataTO.CountsRefBaseline(i)
                        Next
                    Else
                        myResultData.HasError = True
                    End If
                End If

                ' Calculate absorbances
                For i As Integer = 0 To TestReadedCountsAttr.Count - 1

                    myResultData = myCalc.CalculateAbsorbance(CInt(MyClass.TestReadedCountsAttr(i).MeasureABSphMainCount), _
                                                              CInt(MyClass.TestReadedCountsAttr(i).MeasureABSphRefCount), _
                                                              CInt(MyClass.BaseLineMainCountsAttr(i)), _
                                                              CInt(MyClass.BaseLineRefCountsAttr(i)), _
                                                              CInt(MyClass.DarkphMainCountAttr), _
                                                              CInt(MyClass.DarkphRefCountAttr), _
                                                              0, _
                                                              MyClass.PathLength, _
                                                              MyClass.LimitAbs, _
                                                              False, _
                                                              MyClass.DilutionFactor)

                    resultValue = GlobalConstants.CALCULATION_ERROR_VALUE     ' XBC 09/11/2012
                    If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                        resultValue = CType(myResultData.SetDatos, Single)
                        ' XBC 09/11/2012
                        'Else   
                        'resultValue = 99999 ' error    
                        ' XBC 09/11/2012
                    End If

                    MyClass.ResultsABSTest(i).WaveLength = i
                    MyClass.ResultsABSTest(i).Absorbance = resultValue
                Next

                ' Insert the new activity into Historic reports
                myResultData = InsertReport("TEST", "ABS")

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.ManageABSTestResults", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function
#End Region

#Region "Generic"

        'RH 15/92/2012 Not needed. Removed.
        '''' <summary>
        '''' Control system info for separators formats
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by XBC 13/09/2011</remarks>
        'Private Function GetCultureInfo() As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Try
        '        myResultData = PCInfoReader.GetOSCultureInfo()
        '        Dim OSCultureInfo As New PCInfoReader.AX00PCOSCultureInfo
        '        If Not myResultData.HasError And Not myResultData Is Nothing Then
        '            OSCultureInfo = CType(myResultData.SetDatos, PCInfoReader.AX00PCOSCultureInfo)

        '            MyClass.myDecimalSeparator = OSCultureInfo.DecimalSeparator
        '            MyClass.myGroupSeparator = OSCultureInfo.GroupSeparator
        '        End If

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.GetCultureInfo", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function

#End Region

#Region "Historical reports"
        Private Function InsertReport(ByVal pTaskID As String, ByVal pActionID As String) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                If Not MyClass.BaseLineMainCountsAttr Is Nothing Then
                    Dim myHistoricalReportsDelegate As New HistoricalReportsDelegate
                    Dim myHistoricReport As New SRVResultsServiceDS
                    Dim myHistoricReportRow As SRVResultsServiceDS.srv_thrsResultsServiceRow

                    myHistoricReportRow = myHistoricReport.srv_thrsResultsService.Newsrv_thrsResultsServiceRow
                    myHistoricReportRow.TaskID = pTaskID
                    myHistoricReportRow.ActionID = pActionID
                    myHistoricReportRow.Data = GenerateDataReport(myHistoricReportRow.TaskID, myHistoricReportRow.ActionID)
                    myHistoricReportRow.AnalyzerID = AnalyzerId

                    myResultData = myHistoricalReportsDelegate.Add(Nothing, myHistoricReportRow)
                    If (Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing) Then
                        'Get the generated ID from the dataset returned 
                        Dim generatedID As Integer = -1
                        generatedID = DirectCast(myResultData.SetDatos, SRVResultsServiceDS.srv_thrsResultsServiceRow).ResultServiceID

                        ' Insert recommendations if existing
                        If MyClass.RecommendationsReport IsNot Nothing Then
                            Dim myRecommendationsList As New SRVRecommendationsServiceDS
                            Dim myRecommendationsRow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow

                            For i As Integer = 0 To MyClass.RecommendationsReport.Length - 1
                                myRecommendationsRow = myRecommendationsList.srv_thrsRecommendationsService.Newsrv_thrsRecommendationsServiceRow
                                myRecommendationsRow.ResultServiceID = generatedID
                                myRecommendationsRow.RecommendationID = CInt(MyClass.RecommendationsReport(i))
                                myRecommendationsList.srv_thrsRecommendationsService.Rows.Add(myRecommendationsRow)
                            Next

                            myResultData = myHistoricalReportsDelegate.AddRecommendations(Nothing, myRecommendationsList)
                            If myResultData.HasError Then
                                myResultData.HasError = True
                            End If
                            MyClass.RecommendationsReport = Nothing
                        End If
                    Else
                        myResultData.HasError = True
                    End If
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.InsertReport", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Information of every test in this screen with its corresponding codification to be inserted in the common database of Historics
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 01/08/2011
        ''' 
        ''' Data Format : 
        ''' -----------
        ''' Well used for the test (3)
        ''' Filling option used for the test (1) - Automatic=0 Manual=1
        ''' PhMain Dark Counts Mean (7)
        ''' PhRef Dark Counts Mean (7)
        ''' Integration Time common (x all Leds) (5) - ANNULLED
        ''' Leds currents of Reference changed (1) - ANNULLED
        ''' - Following fields repeated for every Led : -
        ''' PhMain BL Counts (7)
        ''' PhMain BL Counts Warning (1) - Success=0 Warning=1
        ''' PhRef BL Counts (7)
        ''' PhRef BL Counts Warning (1) - Success=0 Warning=1
        ''' Current intensity  (5)
        ''' Current intensity Warning (1) - Success=0 Warning=1
        ''' </remarks>
        Private Function GenerateDataReport(ByVal pTask As String, ByVal pAction As String) As String
            Dim myResultData As New GlobalDataTO
            Dim returnValue As String = ""
            Try
                'myResultData = MyClass.GetCultureInfo()
                'If myResultData.HasError Then
                '    Dim myLogAcciones As New ApplicationLogManager()
                '    myLogAcciones.CreateLogActivity(myResultData.ErrorMessage, "PhotometryAdjustmentDelegate.GenerateDataReport", EventLogEntryType.Error, False)
                '    Exit Try
                'End If

                Select Case pTask
                    Case "TEST"

                        Select Case pAction
                            Case "BL_DC"
                                Dim IsWarningBLMainCounts As Boolean = False
                                Dim IsWarningBLRefCounts As Boolean = False
                                Dim IsWarningLeds As Boolean = False

                                returnValue = ""
                                ' Well used for the test
                                returnValue += MyClass.WellToUseAttr.ToString("000")
                                ' Filling option used for the test
                                returnValue += CInt(MyClass.FillModeAttr).ToString
                                ' Number of Leds
                                returnValue += MyClass.qLeds.Count.ToString("00")
                                ' PhMain Dark Counts Mean
                                returnValue += MyClass.DarkphMainCountAttr.ToString("0000000")
                                ' PhRef Dark Counts Mean
                                returnValue += MyClass.DarkphRefCountAttr.ToString("0000000")
                                ' Integration Time common (x all Leds)
                                ' ???
                                '' Leds currents of Reference changed
                                'If MyClass.SaveLedsIntensitiesDone Then
                                '    returnValue += "1"
                                'Else
                                '    returnValue += "0"
                                'End If


                                For i As Integer = 0 To MyClass.BaseLineMainCountsAttr.Count - 1
                                    ' Wavelength 
                                    returnValue += MyClass.qLeds(i).WaveLength.ToString("000")

                                    ' PhMain BL Counts
                                    returnValue += MyClass.BaseLineMainCountsAttr(i).ToString("0000000")

                                    ' PhRef BL Counts 
                                    returnValue += MyClass.BaseLineRefCountsAttr(i).ToString("0000000")

                                    ' Current intensity 
                                    returnValue += MyClass.CurrentLEDsIntensitiesAttr(i).ToString("00000")


                                    Dim Warning As String = "0"
                                    ' PhMain BL Counts Warning
                                    If MyClass.BaseLineMainCountsAttr(i) < MyClass.LimitMinPhMain Or MyClass.BaseLineMainCountsAttr(i) > MyClass.LimitMaxPhMain Then
                                        IsWarningBLMainCounts = True
                                        Warning = "1"
                                    End If
                                    ' PhRef BL Counts Warning
                                    If MyClass.BaseLineRefCountsAttr(i) < MyClass.LimitMinPhRef Or MyClass.BaseLineRefCountsAttr(i) > MyClass.LimitMaxPhRef Then
                                        IsWarningBLRefCounts = True
                                        Warning = "1"
                                    End If
                                    ' Current intensity Warning
                                    If MyClass.CurrentLEDsIntensitiesAttr(i) < MyClass.LimitMinLEDs Or MyClass.CurrentLEDsIntensitiesAttr(i) > MyClass.LimitMaxLEDs Then
                                        IsWarningLeds = True
                                        Warning = "1"
                                    End If

                                    returnValue += Warning

                                Next


                                If IsWarningBLMainCounts Or IsWarningBLRefCounts Then
                                    ' registering the incidence in historical reports activity
                                    If MyClass.RecommendationsReport Is Nothing Then
                                        ReDim MyClass.RecommendationsReport(0)
                                    Else
                                        ReDim Preserve MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport) + 1)
                                    End If
                                    MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport)) = HISTORY_RECOMMENDATIONS.WARNING_BLCOUNTS
                                End If
                                If IsWarningLeds Then
                                    ' registering the incidence in historical reports activity
                                    If MyClass.RecommendationsReport Is Nothing Then
                                        ReDim MyClass.RecommendationsReport(0)
                                    Else
                                        ReDim Preserve MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport) + 1)
                                    End If
                                    MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport)) = HISTORY_RECOMMENDATIONS.WARNING_LEDS
                                End If

                            Case "REPEAT"

                                returnValue = ""
                                ' Well used for the test
                                returnValue += MyClass.WellToUseAttr.ToString("000")
                                ' Filling option used for the test
                                returnValue += CInt(MyClass.FillModeAttr).ToString
                                ' Number of Leds
                                returnValue += MyClass.qLeds.Count.ToString("00")

                                ' LIGHT Results x nLEDs
                                For i As Integer = 0 To MyClass.ResultsRepeatabilityTest.Count - 1
                                    ' Wavelength 
                                    returnValue += MyClass.qLeds(i).WaveLength.ToString("000")

                                    ' Mean Abs
                                    If MyClass.ResultsRepeatabilityTest(i).Absorbance < 0 Then
                                        returnValue += MyClass.ResultsRepeatabilityTest(i).Absorbance.ToString("0.0000") ' minus sign (-) is added automatically
                                    Else
                                        returnValue += MyClass.ResultsRepeatabilityTest(i).Absorbance.ToString("+0.0000")
                                    End If
                                    ' Std. Deviation
                                    If MyClass.ResultsRepeatabilityTest(i).STDDeviationAbsorbance < 0 Then
                                        returnValue += MyClass.ResultsRepeatabilityTest(i).STDDeviationAbsorbance.ToString("0.0000") ' minus sign (-) is added automatically
                                    Else
                                        returnValue += MyClass.ResultsRepeatabilityTest(i).STDDeviationAbsorbance.ToString("+0.0000")
                                    End If
                                    ' Coef. Variation
                                    If MyClass.ResultsRepeatabilityTest(i).CVAbsorbance < 0 Then
                                        'If MyClass.ResultsRepeatabilityTest(i).CVAbsorbance < -100 Then
                                        '    returnValue += "<-10" & MyClass.myDecimalSeparator.ToString & "0"
                                        'Else
                                        returnValue += MyClass.ResultsRepeatabilityTest(i).CVAbsorbance.ToString("00.00") ' minus sign (-) is added automatically
                                        'End If
                                    Else
                                        If MyClass.ResultsRepeatabilityTest(i).CVAbsorbance > 10 Then
                                            returnValue += ">10" & MyClass.myDecimalSeparator.ToString & "00"
                                        Else
                                            returnValue += MyClass.ResultsRepeatabilityTest(i).CVAbsorbance.ToString("+00.00")
                                        End If
                                    End If
                                    returnValue += "%"
                                    ' Max value
                                    If MyClass.GetMAXAbsorbanceRepeatabilityResult(i) < 0 Then
                                        returnValue += MyClass.GetMAXAbsorbanceRepeatabilityResult(i).ToString("0.0000") ' minus sign (-) is added automatically
                                    Else
                                        returnValue += MyClass.GetMAXAbsorbanceRepeatabilityResult(i).ToString("+0.0000")
                                    End If
                                    ' Min value
                                    If MyClass.GetMINAbsorbanceRepeatabilityResult(i) < 0 Then
                                        returnValue += MyClass.GetMINAbsorbanceRepeatabilityResult(i).ToString("0.0000") ' minus sign (-) is added automatically
                                    Else
                                        returnValue += MyClass.GetMINAbsorbanceRepeatabilityResult(i).ToString("+0.0000")
                                    End If
                                    ' Range value
                                    If MyClass.GetRangeAbsorbanceRepeatabilityResult(i) < 0 Then
                                        returnValue += MyClass.GetRangeAbsorbanceRepeatabilityResult(i).ToString("0.0000") ' minus sign (-) is added automatically
                                    Else
                                        returnValue += MyClass.GetRangeAbsorbanceRepeatabilityResult(i).ToString("+0.0000")
                                    End If

                                Next


                                ' DARK Results (same for All LEDs - take the first !!!)

                                ' PH MAIN 
                                ' Counts Mean
                                returnValue += MyClass.ResultsRepeatabilityTest(0).CountsPhMainDK.ToString("0000000")
                                ' Std. Deviation
                                returnValue += MyClass.ResultsRepeatabilityTest(0).STDDeviationPhMainDK.ToString("00000")
                                ' Max Value
                                returnValue += MyClass.GetMAXPhMainRepeatabilityResultDK(0).ToString("0000000")
                                ' Min Value
                                returnValue += MyClass.GetMINPhMainRepeatabilityResultDK(0).ToString("0000000")
                                ' Range Value
                                returnValue += MyClass.GetRangePhMainRepeatabilityResultDK(0).ToString("0000000")

                                ' PH REF 
                                ' Counts Mean
                                returnValue += MyClass.ResultsRepeatabilityTest(0).CountsPhRefDK.ToString("0000000")
                                ' Std. Deviation
                                returnValue += MyClass.ResultsRepeatabilityTest(0).STDDeviationPhRefDK.ToString("00000")
                                ' Max Value
                                returnValue += MyClass.GetMAXPhRefRepeatabilityResultDK(0).ToString("0000000")
                                ' Min Value
                                returnValue += MyClass.GetMINPhRefRepeatabilityResultDK(0).ToString("0000000")
                                ' Range Value
                                returnValue += MyClass.GetRangePhRefRepeatabilityResultDK(0).ToString("0000000")

                            Case "STAB"

                                returnValue = ""
                                ' Well used for the test
                                returnValue += MyClass.WellToUseAttr.ToString("000")
                                ' Filling option used for the test
                                returnValue += CInt(MyClass.FillModeAttr).ToString
                                ' Number of Leds
                                returnValue += MyClass.qLeds.Count.ToString("00")

                                ' LIGHT Results x nLEDs
                                For i As Integer = 0 To MyClass.ResultsStabilityTest.Count - 1
                                    ' Wavelength 
                                    returnValue += MyClass.qLeds(i).WaveLength.ToString("000")

                                    ' Mean Abs
                                    If MyClass.ResultsStabilityTest(i).Absorbance < 0 Then
                                        returnValue += MyClass.ResultsStabilityTest(i).Absorbance.ToString("0.0000") ' minus sign (-) is added automatically
                                    Else
                                        returnValue += MyClass.ResultsStabilityTest(i).Absorbance.ToString("+0.0000")
                                    End If
                                    ' Std. Deviation
                                    If MyClass.ResultsStabilityTest(i).STDDeviationAbsorbance < 0 Then
                                        returnValue += MyClass.ResultsStabilityTest(i).STDDeviationAbsorbance.ToString("0.0000") ' minus sign (-) is added automatically
                                    Else
                                        returnValue += MyClass.ResultsStabilityTest(i).STDDeviationAbsorbance.ToString("+0.0000")
                                    End If
                                    ' Coef. Variation
                                    If MyClass.ResultsStabilityTest(i).CVAbsorbance < 0 Then
                                        'If MyClass.ResultsStabilityTest(i).CVAbsorbance < -100 Then
                                        '    returnValue += "<-10" & MyClass.myDecimalSeparator.ToString & "0"
                                        'Else
                                        returnValue += MyClass.ResultsStabilityTest(i).CVAbsorbance.ToString("00.00") ' minus sign (-) is added automatically
                                        'End If
                                    Else
                                        If MyClass.ResultsStabilityTest(i).CVAbsorbance > 10 Then
                                            returnValue += ">10" & MyClass.myDecimalSeparator.ToString & "00"
                                        Else
                                            returnValue += MyClass.ResultsStabilityTest(i).CVAbsorbance.ToString("+00.00")
                                        End If
                                    End If
                                    returnValue += "%"
                                    ' Max value
                                    If MyClass.GetMAXAbsorbanceStabilityResult(i) < 0 Then
                                        returnValue += MyClass.GetMAXAbsorbanceStabilityResult(i).ToString("0.0000") ' minus sign (-) is added automatically
                                    Else
                                        returnValue += MyClass.GetMAXAbsorbanceStabilityResult(i).ToString("+0.0000")
                                    End If
                                    ' Min value
                                    If MyClass.GetMINAbsorbanceStabilityResult(i) < 0 Then
                                        returnValue += MyClass.GetMINAbsorbanceStabilityResult(i).ToString("0.0000") ' minus sign (-) is added automatically
                                    Else
                                        returnValue += MyClass.GetMINAbsorbanceStabilityResult(i).ToString("+0.0000")
                                    End If
                                    ' Range value
                                    If MyClass.GetRangeAbsorbanceStabilityResult(i) < 0 Then
                                        returnValue += MyClass.GetRangeAbsorbanceStabilityResult(i).ToString("0.0000") ' minus sign (-) is added automatically
                                    Else
                                        returnValue += MyClass.GetRangeAbsorbanceStabilityResult(i).ToString("+0.0000")
                                    End If

                                Next


                                ' DARK Results (same for All LEDs - take the first !!!)

                                ' PH MAIN 
                                ' Counts Mean
                                returnValue += MyClass.ResultsStabilityTest(0).CountsPhMainDK.ToString("0000000")
                                ' Std. Deviation
                                returnValue += MyClass.ResultsStabilityTest(0).STDDeviationPhMainDK.ToString("00000")
                                ' Max Value
                                returnValue += MyClass.GetMAXPhMainStabilityResultDK(0).ToString("0000000")
                                ' Min Value
                                returnValue += MyClass.GetMINPhMainStabilityResultDK(0).ToString("0000000")
                                ' Range Value
                                returnValue += MyClass.GetRangePhMainStabilityResultDK(0).ToString("0000000")

                                ' PH REF 
                                ' Counts Mean
                                returnValue += MyClass.ResultsStabilityTest(0).CountsPhRefDK.ToString("0000000")
                                ' Std. Deviation
                                returnValue += MyClass.ResultsStabilityTest(0).STDDeviationPhRefDK.ToString("00000")
                                ' Max Value
                                returnValue += MyClass.GetMAXPhRefStabilityResultDK(0).ToString("0000000")
                                ' Min Value
                                returnValue += MyClass.GetMINPhRefStabilityResultDK(0).ToString("0000000")
                                ' Range Value
                                returnValue += MyClass.GetRangePhRefStabilityResultDK(0).ToString("0000000")


                            Case "ABS"

                                returnValue = ""
                                ' Well used for the test
                                returnValue += MyClass.WellToUseAttr.ToString("000")
                                ' Filling option used for the test
                                returnValue += CInt(MyClass.FillModeAttr).ToString
                                ' Number of Leds
                                returnValue += MyClass.qLeds.Count.ToString("00")

                                ' LIGHT Results x nLEDs
                                For i As Integer = 0 To MyClass.ResultsABSTest.Count - 1
                                    ' Wavelength 
                                    returnValue += MyClass.qLeds(i).WaveLength.ToString("000")

                                    ' Mean Abs
                                    If MyClass.ResultsABSTest(i).Absorbance < 0 Then
                                        returnValue += MyClass.ResultsABSTest(i).Absorbance.ToString("0.0000") ' minus sign (-) is added automatically
                                    Else
                                        returnValue += MyClass.ResultsABSTest(i).Absorbance.ToString("+0.0000")
                                    End If
                                Next


                        End Select
                End Select

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.GenerateDataReport", EventLogEntryType.Error, False)
            End Try
            Return returnValue
        End Function
#End Region

#End Region

#Region "Simulate Mode"
        ' TEST WITH EXCEL 
        '' Please, type here your path and the name of your excel test file :
        'Private simulatePath As String = "C:\Documents and Settings\Xavier_Badia\Escritorio\En curs\"
        'Private simulateFile As String = "Real Values for Photometry Metrology Test.xls"
        ' TEST WITH EXCEL 

        Public Sub SimulateBLTest()
            Dim PhotometryData As New PhotometryDataTO
            Dim Rnd As New Random()

            Dim myResultData As New GlobalDataTO
            Dim myAnalyzerLedPositionsDelegate As New AnalyzerLedPositionsDelegate
            Dim myAnalyzerLedPosDS As New AnalyzerLedPositionsDS

            ''For i As Integer = 0 To CommonParameters.MaxWaveLengths - 1
            ''    PhotometryData.PositionLed.Add(i + 1)
            ''Next
            For i As Integer = 0 To CommonParameters.MaxWaveLengths - 1
                PhotometryData.PositionLed.Add(CInt(MyClass.PositionLEDsxSimulation(i)))
            Next

            ' TEST WITH EXCEL 
            'Dim sProva As String
            'sProva = "Exemple Baixa Abs" ' Prova 1
            ''sProva = "Exemple Alta Abs" ' Prova 2

            'PhotometryData.CountsMainBaseline = _
            'loadRange(simulatePath & simulateFile, _
            '            sProva, "I5:I14")

            'PhotometryData.CountsRefBaseline = _
            'loadRange(simulatePath & simulateFile, _
            '            sProva, "J5:J14")

            'PhotometryData.CountsMainDarkness = _
            'loadRange(simulatePath & simulateFile, _
            '            sProva, "G5:G14")

            'PhotometryData.CountsRefDarkness = _
            'loadRange(simulatePath & simulateFile, _
            '            sProva, "H5:H14")

            'For i As Integer = 0 To CommonParameters.MaxWaveLengths - 1
            '    PhotometryData.IntegrationTimes.Add(12)
            'Next

            'PhotometryData.LEDsIntensities = _
            'loadRange(simulatePath & simulateFile, _
            '            sProva, "C5:C14")
            ' TEST WITH EXCEL 

            ' TEST WITH RANDOM 
            For i As Integer = 0 To CommonParameters.MaxWaveLengths - 1
                PhotometryData.CountsMainBaseline.Add(Rnd.Next(100000, 1100000))
            Next

            For i As Integer = 0 To CommonParameters.MaxWaveLengths - 1
                PhotometryData.CountsRefBaseline.Add(Rnd.Next(100000, 950000))
            Next

            For i As Integer = 0 To CommonParameters.MaxWaveLengths - 1
                PhotometryData.CountsMainDarkness.Add(Rnd.Next(3000, 4000))
            Next

            For i As Integer = 0 To CommonParameters.MaxWaveLengths - 1
                PhotometryData.CountsRefDarkness.Add(Rnd.Next(3000, 4000))
            Next

            For i As Integer = 0 To CommonParameters.MaxWaveLengths - 1
                PhotometryData.IntegrationTimes.Add(12)
            Next

            For i As Integer = 0 To CommonParameters.MaxWaveLengths - 1
                PhotometryData.LEDsIntensities.Add(Rnd.Next(2000, 8000))
            Next
            ' TEST WITH RANDOM 

            myFwScriptDelegate.AnalyzerManager.SetPhotometryData(PhotometryData)
            AcceptBLResults()
            ManageResultsBL()
            'MeasureCurrentLEDSReaded()
        End Sub

        Public Sub SimulateITEditionTest()
            MyClass.CurrentLEDsIntensitiesAttr = New List(Of Single)

            Dim Rnd As New Random()
            For i As Integer = 0 To CommonParameters.MaxWaveLengths - 1
                MyClass.CurrentLEDsIntensitiesAttr.Add(Rnd.Next(2000, 8000))
            Next
        End Sub

        Public Sub SimulateRepeatabilityTest()
            For i As Integer = 0 To TestReadedCountsAttr.Count - 1
                MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityAbsorbances = New List(Of Single)
                MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphMainCounts = New List(Of Single)
                MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphRefCounts = New List(Of Single)
                MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphMainCountsDK = New List(Of Single)
                MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphRefCountsDK = New List(Of Single)
            Next

            ' TEST WITH EXCEL 
            'Dim sProva As String
            'sProva = "Exemple Baixa Abs" ' Prova 1
            ''sProva = "Exemple Alta Abs" ' Prova 2

            'MyClass.TestReadedCountsAttr(0).MeasuresRepeatabilityphMainCounts = _
            'loadRange(simulatePath & simulateFile, _
            '            sProva, "D35:D335")

            'MyClass.TestReadedCountsAttr(0).MeasuresRepeatabilityphRefCounts = _
            'loadRange(simulatePath & simulateFile, _
            '            sProva, "E35:E335")

            'MyClass.TestReadedCountsAttr(0).MeasuresRepeatabilityphMainCountsDK = _
            'loadRange(simulatePath & simulateFile, _
            '            "Exemple Foscor", "D28:D328")

            'MyClass.TestReadedCountsAttr(0).MeasuresRepeatabilityphRefCountsDK = _
            'loadRange(simulatePath & simulateFile, _
            '            "Exemple Foscor", "E28:E328")
            ' TEST WITH EXCEL 


            ' TEST WITH RANDOM 
            Dim Rnd As New Random()
            Dim myMaxValueLIGHT As Integer
            Dim myMinValueLIGHT As Integer
            Dim myMaxValueDARK As Integer
            Dim myMinValueDARK As Integer
            myMaxValueDARK = Max(CInt(MyClass.DarkphMainCountAttr) - 1, CInt(MyClass.DarkphRefCountAttr) - 1)
            myMinValueDARK = Min(CInt(MyClass.DarkphMainCountAttr) + 1, CInt(MyClass.DarkphRefCountAttr) + 1)
            myMaxValueLIGHT = Max(CInt(MyClass.BaseLineMainCountsAttr.Max) - 1, CInt(MyClass.BaseLineRefCountsAttr.Max) - 1)
            myMinValueLIGHT = Min(CInt(MyClass.BaseLineMainCountsAttr.Min) + 1, CInt(MyClass.BaseLineRefCountsAttr.Min) + 1)

            For i As Integer = 0 To TestReadedCountsAttr.Count - 1
                For j As Integer = 0 To CInt(CommonParameters.MaxRepeatability) - 1
                    MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphMainCounts.Add(Rnd.Next(myMinValueLIGHT, myMaxValueLIGHT))
                    MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphRefCounts.Add(Rnd.Next(myMinValueLIGHT, myMaxValueLIGHT) - 1000)

                    MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphMainCountsDK.Add(Rnd.Next(myMinValueDARK, myMaxValueDARK))
                    MyClass.TestReadedCountsAttr(i).MeasuresRepeatabilityphRefCountsDK.Add(Rnd.Next(myMinValueDARK, myMaxValueDARK) - 100)
                Next
            Next
            ' TEST WITH RANDOM 


            MyClass.CurrentTestAttr = ADJUSTMENT_GROUPS.REPEATABILITY
            MyClass.ManageTestResults()
        End Sub

        Public Sub SimulateStabilityTest()
            For i As Integer = 0 To TestReadedCountsAttr.Count - 1
                MyClass.TestReadedCountsAttr(i).MeasuresStabilityAbsorbances = New List(Of Single)
                MyClass.TestReadedCountsAttr(i).MeasuresStabilityphMainCounts = New List(Of Single)
                MyClass.TestReadedCountsAttr(i).MeasuresStabilityphRefCounts = New List(Of Single)
                MyClass.TestReadedCountsAttr(i).MeasuresStabilityphMainCountsDK = New List(Of Single)
                MyClass.TestReadedCountsAttr(i).MeasuresStabilityphRefCountsDK = New List(Of Single)
            Next

            ' TEST WITH EXCEL 
            'Dim sProva As String
            'sProva = "Exemple Baixa Abs" ' Prova 1
            ''sProva = "Exemple Alta Abs" ' Prova 2

            'MyClass.TestReadedCountsAttr(0).MeasuresStabilityphMainCounts = _
            'loadRange(simulatePath & simulateFile, _
            '            sProva, "D35:D335")

            'MyClass.TestReadedCountsAttr(0).MeasuresStabilityphRefCounts = _
            'loadRange(simulatePath & simulateFile, _
            '            sProva, "E35:E335")

            'MyClass.TestReadedCountsAttr(0).MeasuresStabilityphMainCountsDK = _
            'loadRange(simulatePath & simulateFile, _
            '            "Exemple Foscor", "D28:D328")

            'MyClass.TestReadedCountsAttr(0).MeasuresStabilityphRefCountsDK = _
            'loadRange(simulatePath & simulateFile, _
            '            "Exemple Foscor", "E28:E328")
            ' TEST WITH EXCEL 


            ' TEST WITH RANDOM 
            Dim Rnd As New Random()
            Dim myMaxValueLIGHT As Integer
            Dim myMinValueLIGHT As Integer
            Dim myMaxValueDARK As Integer
            Dim myMinValueDARK As Integer
            myMaxValueDARK = Max(CInt(MyClass.DarkphMainCountAttr) - 1, CInt(MyClass.DarkphRefCountAttr) - 1)
            myMinValueDARK = Min(CInt(MyClass.DarkphMainCountAttr) + 1, CInt(MyClass.DarkphRefCountAttr) + 1)
            myMaxValueLIGHT = Max(CInt(MyClass.BaseLineMainCountsAttr.Max) - 1, CInt(MyClass.BaseLineRefCountsAttr.Max) - 1)
            myMinValueLIGHT = Min(CInt(MyClass.BaseLineMainCountsAttr.Min) + 1, CInt(MyClass.BaseLineRefCountsAttr.Min) + 1)

            For i As Integer = 0 To TestReadedCountsAttr.Count - 1
                For j As Integer = 0 To CInt(CommonParameters.MaxStability) - 1
                    MyClass.TestReadedCountsAttr(i).MeasuresStabilityphMainCounts.Add(Rnd.Next(myMinValueLIGHT, myMaxValueLIGHT))
                    MyClass.TestReadedCountsAttr(i).MeasuresStabilityphRefCounts.Add(Rnd.Next(myMinValueLIGHT, myMaxValueLIGHT))

                    MyClass.TestReadedCountsAttr(i).MeasuresStabilityphMainCountsDK.Add(Rnd.Next(myMinValueDARK, myMaxValueDARK))
                    MyClass.TestReadedCountsAttr(i).MeasuresStabilityphRefCountsDK.Add(Rnd.Next(myMinValueDARK, myMaxValueDARK))
                Next
            Next
            ' TEST WITH RANDOM 

            MyClass.CurrentTestAttr = ADJUSTMENT_GROUPS.STABILITY
            MyClass.ManageTestResults()
        End Sub

        Public Sub SimulateABSTest()
            For i As Integer = 0 To TestReadedCountsAttr.Count - 1
                MyClass.TestReadedCountsAttr(i).MeasureABSphMainCount = New Single
                MyClass.TestReadedCountsAttr(i).MeasureABSphRefCount = New Single
            Next

            ' TEST WITH EXCEL 
            'Dim sProva As String
            'sProva = "Exemple Baixa Abs" ' Prova 1
            ''sProva = "Exemple Alta Abs" ' Prova 2

            'Dim listReturn As New List(Of Single)

            'listReturn = _
            'loadRange(simulatePath & simulateFile, _
            '            sProva, "D35:D36")

            'MyClass.TestReadedCountsAttr(0).MeasureABSphMainCount = listReturn(0)

            'listReturn = _
            'loadRange(simulatePath & simulateFile, _
            '            sProva, "E35:E36")

            'MyClass.TestReadedCountsAttr(0).MeasureABSphRefCount = listReturn(0)
            ' TEST WITH EXCEL 


            ' TEST WITH RANDOM 
            Dim Rnd As New Random()
            Dim myMaxValueLIGHT As Integer
            Dim myMinValueLIGHT As Integer
            myMaxValueLIGHT = Max(CInt(MyClass.BaseLineMainCountsAttr.Max) - 1, CInt(MyClass.BaseLineRefCountsAttr.Max) - 1)
            myMinValueLIGHT = Min(CInt(MyClass.BaseLineMainCountsAttr.Min) + 1, CInt(MyClass.BaseLineRefCountsAttr.Min) + 1)

            For i As Integer = 0 To TestReadedCountsAttr.Count - 1
                MyClass.TestReadedCountsAttr(i).MeasureABSphMainCount = Rnd.Next(myMinValueLIGHT, myMaxValueLIGHT)
                MyClass.TestReadedCountsAttr(i).MeasureABSphRefCount = Rnd.Next(myMinValueLIGHT, myMaxValueLIGHT)
            Next
            ' TEST WITH RANDOM 

            ManageABSTestResults(True)
        End Sub

        Private Function loadRange(ByVal sFileName As String, ByVal sSheetName As String, ByVal sRange As String) As List(Of Single)
            Dim returnValue As New List(Of Single)
            Try

                ' // Comprobar que el archivo Excel existe   
                If System.IO.File.Exists(sFileName) Then

                    Dim objDataSet As System.Data.DataSet
                    Dim objDataAdapter As System.Data.OleDb.OleDbDataAdapter
                    ' // Declarar la Cadena de conexión   
                    Dim sCs As String = "provider=Microsoft.Jet.OLEDB.4.0; " & "data source=" & sFileName & "; Extended Properties=Excel 8.0;"
                    Dim objOleConnection As System.Data.OleDb.OleDbConnection
                    objOleConnection = New System.Data.OleDb.OleDbConnection(sCs)

                    ' // Declarar la consulta SQL que indica el libro y el rango de la hoja   
                    Dim sSql As String = "select * from " & "[" & sSheetName & "$" & sRange & "]"
                    ' // Obtener los datos   
                    objDataAdapter = New System.Data.OleDb.OleDbDataAdapter(sSql, objOleConnection)

                    ' // Crear DataSet y llenarlo   
                    objDataSet = New System.Data.DataSet

                    objDataAdapter.Fill(objDataSet)
                    ' // Cerrar la conexión   
                    objOleConnection.Close()

                    ' // Enlazar DataGrid al Dataset   
                    'With dv
                    '    .DataSource = objDataSet
                    '    .DataMember = objDataSet.Tables(0).TableName
                    'End With
                    For Each myRow As DataRow In objDataSet.Tables(0).Rows
                        returnValue.Add(CInt(myRow.Item(0)))
                    Next
                Else
                    MsgBox("No se ha encontrado el archivo: " & sFileName, MsgBoxStyle.Exclamation)
                End If

            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Critical)
            End Try
            Return returnValue
        End Function

        'Public Sub SimulateCheckRotorTest()
        '    MyClass.MeasuresCheckRotorCountsAttr = New List(Of Single)

        '    Dim Rnd As New Random()
        '    Dim myMaxValue As Integer
        '    Dim myMinValue As Integer
        '    If MyClass.WLSToUse = 0 Then
        '        myMaxValue = 4500
        '        myMinValue = 0
        '    Else
        '        myMaxValue = 980000
        '        myMinValue = 800000
        '    End If

        '    For i As Integer = 0 To 7
        '        MyClass.MeasuresCheckRotorCountsAttr.Add(Rnd.Next(myMinValue, myMaxValue))
        '    Next

        '    MyClass.CurrentTestAttr = ADJUSTMENT_GROUPS.CHECK_ROTOR
        '    MyClass.ManageTestResults()
        'End Sub
#End Region

#Region "TO DELETE"

        ' Case "BL_DC"

        'text += Environment.NewLine
        '' BaseLine Results wavelenghts 1, 2, 3
        'text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_LED", pcurrentLanguage) + " "
        'text2 = text1
        'text3 = text1
        'text1 += CSng(pData.Substring(19, 3)).ToString("##0") + ":"
        'text2 += CSng(pData.Substring(44, 3)).ToString("##0") + ":"
        'text3 += CSng(pData.Substring(69, 3)).ToString("##0") + ":"
        'text += myUtility.FormatLineHistorics(text1, text2, text3)

        '' PhMain BL Counts wavelenghts 1, 2, 3
        'text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode1", pcurrentLanguage) + ": "
        'text1 += CSng(pData.Substring(22, 7)).ToString("#,###,##0")
        'text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode1", pcurrentLanguage) + ": "
        'text2 += CSng(pData.Substring(47, 7)).ToString("#,###,##0")
        'text3 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode1", pcurrentLanguage) + ": "
        'text3 += CSng(pData.Substring(72, 7)).ToString("#,###,##0")
        'text += myUtility.FormatLineHistorics(text1, text2, text3)

        '' PhRef BL Counts wavelenghts 1, 2, 3
        'text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode2", pcurrentLanguage) + ": "
        'text1 += CSng(pData.Substring(30, 7)).ToString("#,###,##0")
        'text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode2", pcurrentLanguage) + ": "
        'text2 += CSng(pData.Substring(55, 7)).ToString("#,###,##0")
        'text3 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode2", pcurrentLanguage) + ": "
        'text3 += CSng(pData.Substring(80, 7)).ToString("#,###,##0")
        'text += myUtility.FormatLineHistorics(text1, text2, text3)

        '' Current intensity wavelenghts 1, 2, 3
        'text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INTENSITY", pcurrentLanguage) + ": "
        'text1 += CSng(pData.Substring(38, 5)).ToString("##,##0")
        'text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INTENSITY", pcurrentLanguage) + ": "
        'text2 += CSng(pData.Substring(63, 5)).ToString("##,##0")
        'text3 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INTENSITY", pcurrentLanguage) + ": "
        'text3 += CSng(pData.Substring(88, 5)).ToString("##,##0")
        'text += myUtility.FormatLineHistorics(text1, text2, text3)

        '' Warnings wavelenghts 1, 2, 3
        'If pData.Substring(29, 1) = "1" Or pData.Substring(37, 1) = "1" Or pData.Substring(43, 1) = "1" Then
        '    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REVIEW_LED", pcurrentLanguage)
        'Else
        '    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
        'End If
        'If pData.Substring(54, 1) = "1" Or pData.Substring(62, 1) = "1" Or pData.Substring(68, 1) = "1" Then
        '    text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REVIEW_LED", pcurrentLanguage)
        'Else
        '    text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
        'End If
        'If pData.Substring(79, 1) = "1" Or pData.Substring(87, 1) = "1" Or pData.Substring(93, 1) = "1" Then
        '    text3 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REVIEW_LED", pcurrentLanguage)
        'Else
        '    text3 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
        'End If
        'text += myUtility.FormatLineHistorics(text1, text2, text3)


        'text += Environment.NewLine
        '' BaseLine Results wavelenghts 4, 5, 6
        'text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_LED", pcurrentLanguage) + " "
        'text2 = text1
        'text3 = text1
        'text1 += CSng(pData.Substring(94, 3)).ToString("##0") + ":"
        'text2 += CSng(pData.Substring(119, 3)).ToString("##0") + ":"
        'text3 += CSng(pData.Substring(144, 3)).ToString("##0") + ":"
        'text += myUtility.FormatLineHistorics(text1, text2, text3)

        '' PhMain BL Counts wavelenghts 4, 5, 6
        'text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode1", pcurrentLanguage) + ": "
        'text1 += CSng(pData.Substring(97, 7)).ToString("#,###,##0")
        'text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode1", pcurrentLanguage) + ": "
        'text2 += CSng(pData.Substring(122, 7)).ToString("#,###,##0")
        'text3 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode1", pcurrentLanguage) + ": "
        'text3 += CSng(pData.Substring(147, 7)).ToString("#,###,##0")
        'text += myUtility.FormatLineHistorics(text1, text2, text3)

        '' PhRef BL Counts wavelenghts 4, 5, 6
        'text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode2", pcurrentLanguage) + ": "
        'text1 += CSng(pData.Substring(105, 7)).ToString("#,###,##0")
        'text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode2", pcurrentLanguage) + ": "
        'text2 += CSng(pData.Substring(130, 7)).ToString("#,###,##0")
        'text3 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode2", pcurrentLanguage) + ": "
        'text3 += CSng(pData.Substring(155, 7)).ToString("#,###,##0")
        'text += myUtility.FormatLineHistorics(text1, text2, text3)

        '' Current intensity wavelenghts 4, 5, 6
        'text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INTENSITY", pcurrentLanguage) + ": "
        'text1 += CSng(pData.Substring(113, 5)).ToString("##,##0")
        'text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INTENSITY", pcurrentLanguage) + ": "
        'text2 += CSng(pData.Substring(138, 5)).ToString("##,##0")
        'text3 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INTENSITY", pcurrentLanguage) + ": "
        'text3 += CSng(pData.Substring(163, 5)).ToString("##,##0")
        'text += myUtility.FormatLineHistorics(text1, text2, text3)

        '' Warnings wavelenghts 4, 5, 6
        'If pData.Substring(104, 1) = "1" Or pData.Substring(112, 1) = "1" Or pData.Substring(118, 1) = "1" Then
        '    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REVIEW_LED", pcurrentLanguage)
        'Else
        '    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
        'End If
        'If pData.Substring(129, 1) = "1" Or pData.Substring(137, 1) = "1" Or pData.Substring(143, 1) = "1" Then
        '    text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REVIEW_LED", pcurrentLanguage)
        'Else
        '    text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
        'End If
        'If pData.Substring(154, 1) = "1" Or pData.Substring(162, 1) = "1" Or pData.Substring(168, 1) = "1" Then
        '    text3 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REVIEW_LED", pcurrentLanguage)
        'Else
        '    text3 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
        'End If
        'text += myUtility.FormatLineHistorics(text1, text2, text3)


        'text += Environment.NewLine
        '' BaseLine Results wavelenghts 7, 8, 9
        'text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_LED", pcurrentLanguage) + " "
        'text2 = text1
        'text3 = text1
        'text1 += CSng(pData.Substring(169, 3)).ToString("##0") + ":"
        'text2 += CSng(pData.Substring(194, 3)).ToString("##0") + ":"
        'text3 += CSng(pData.Substring(219, 3)).ToString("##0") + ":"
        'text += myUtility.FormatLineHistorics(text1, text2, text3)

        '' PhMain BL Counts wavelenghts 7, 8, 9
        'text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode1", pcurrentLanguage) + ": "
        'text1 += CSng(pData.Substring(172, 7)).ToString("#,###,##0")
        'text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode1", pcurrentLanguage) + ": "
        'text2 += CSng(pData.Substring(197, 7)).ToString("#,###,##0")
        'text3 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode1", pcurrentLanguage) + ": "
        'text3 += CSng(pData.Substring(222, 7)).ToString("#,###,##0")
        'text += myUtility.FormatLineHistorics(text1, text2, text3)

        '' PhRef BL Counts wavelenghts 7, 8, 9 
        'text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode2", pcurrentLanguage) + ": "
        'text1 += CSng(pData.Substring(180, 7)).ToString("#,###,##0")
        'text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode2", pcurrentLanguage) + ": "
        'text2 += CSng(pData.Substring(205, 7)).ToString("#,###,##0")
        'text3 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode2", pcurrentLanguage) + ": "
        'text3 += CSng(pData.Substring(230, 7)).ToString("#,###,##0")
        'text += myUtility.FormatLineHistorics(text1, text2, text3)

        '' Current intensity wavelenghts 7, 8, 9
        'text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INTENSITY", pcurrentLanguage) + ": "
        'text1 += CSng(pData.Substring(188, 5)).ToString("##,##0")
        'text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INTENSITY", pcurrentLanguage) + ": "
        'text2 += CSng(pData.Substring(213, 5)).ToString("##,##0")
        'text3 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INTENSITY", pcurrentLanguage) + ": "
        'text3 += CSng(pData.Substring(238, 5)).ToString("##,##0")
        'text += myUtility.FormatLineHistorics(text1, text2, text3)

        '' Warnings wavelenghts 7, 8, 9
        'If pData.Substring(179, 1) = "1" Or pData.Substring(187, 1) = "1" Or pData.Substring(193, 1) = "1" Then
        '    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REVIEW_LED", pcurrentLanguage)
        'Else
        '    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
        'End If
        'If pData.Substring(204, 1) = "1" Or pData.Substring(212, 1) = "1" Or pData.Substring(218, 1) = "1" Then
        '    text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REVIEW_LED", pcurrentLanguage)
        'Else
        '    text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
        'End If
        'If pData.Substring(229, 1) = "1" Or pData.Substring(237, 1) = "1" Or pData.Substring(243, 1) = "1" Then
        '    text3 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REVIEW_LED", pcurrentLanguage)
        'Else
        '    text3 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
        'End If
        'text += myUtility.FormatLineHistorics(text1, text2, text3)



        ' Case "REPEAT" 
        'text += Environment.NewLine

        '' Results wavelength 1
        'text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_LED", pcurrentLanguage) + " "
        'text1 += CSng(pData.Substring(4, 3)).ToString("##0") + ":"
        '' Darkness Results
        'text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode1", pcurrentLanguage) + ": "
        'text3 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photodiode2", pcurrentLanguage) + ": "
        'text += myUtility.FormatLineHistorics(text1, text2, text3)

        '' LIGHT Results
        '' Mean
        'text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Mean", pcurrentLanguage) + ": " 'JB 01/10/2012 - Resource String unification
        'text1 += pData.Substring(7, 7).ToString()
        '' Mean dark
        'text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Mean", pcurrentLanguage) + ": " 'JB 01/10/2012 - Resource string unification
        'text2 += CSng(pData.Substring(49, 7)).ToString("#,###,##0")
        'text3 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Mean", pcurrentLanguage) + ": " 'JB 01/10/2012 - Resource String unification
        'text3 += CSng(pData.Substring(82, 7)).ToString("#,###,##0")
        'text += myUtility.FormatLineHistorics(text1, text2, text3)

        '' Std. Deviation
        'text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SDevResult", pcurrentLanguage) + ": "
        'text1 += pData.Substring(14, 7).ToString()
        '' Std. Deviation dark
        'text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SDevResult", pcurrentLanguage) + ": "
        'text2 += CSng(pData.Substring(56, 5)).ToString("##,##0")
        'text3 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SDevResult", pcurrentLanguage) + ": "
        'text3 += CSng(pData.Substring(89, 5)).ToString("##,##0")
        'text += myUtility.FormatLineHistorics(text1, text2, text3)

        '' Coef. Variation
        'text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CDevResult", pcurrentLanguage) + ": "
        'text1 += pData.Substring(21, 7).ToString()
        'text += myUtility.FormatLineHistorics(text1)

        '' Max value
        'text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MaxValue", pcurrentLanguage) + ": "
        'text1 += pData.Substring(28, 7).ToString()
        '' Max value dark
        'text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MaxValue", pcurrentLanguage) + ": "
        'text2 += CSng(pData.Substring(61, 7)).ToString("#,###,##0")
        'text3 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MaxValue", pcurrentLanguage) + ": "
        'text3 += CSng(pData.Substring(94, 7)).ToString("#,###,##0")
        'text += myUtility.FormatLineHistorics(text1, text2, text3)

        '' Min value
        'text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MinValue", pcurrentLanguage) + ": "
        'text1 += pData.Substring(35, 7).ToString()
        '' Min value dark
        'text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MinValue", pcurrentLanguage) + ": "
        'text2 += CSng(pData.Substring(68, 7)).ToString("#,###,##0")
        'text3 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MinValue", pcurrentLanguage) + ": "
        'text3 += CSng(pData.Substring(101, 7)).ToString("#,###,##0")
        'text += myUtility.FormatLineHistorics(text1, text2, text3)

        '' Range
        'text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Range", pcurrentLanguage) + ": "
        'text1 += pData.Substring(42, 7).ToString()
        '' Range dark
        'text2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Range", pcurrentLanguage) + ": "
        'text2 += CSng(pData.Substring(75, 7)).ToString("#,###,##0")
        'text3 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Range", pcurrentLanguage) + ": "
        'text3 += CSng(pData.Substring(108, 7)).ToString("#,###,##0")
        'text += myUtility.FormatLineHistorics(text1, text2, text3)

        ' ******************************************
#End Region

    End Class

End Namespace