Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.DAL

Namespace Biosystems.Ax00.Calculations
    Partial Public Class CalculationsDelegate

#Region "Constructor"
        'AG remove the analyzer model from New, it is get in InitCommon ('SGM 04/03/11)
        Public Sub New()
            Try
                'myAnalyzerModel = pAnalyzerModel
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalculationsDelegate.New", EventLogEntryType.Error, False)
            End Try
        End Sub
#End Region

#Region "Calculation Input Structures"

        Private Structure CommonData
            'Base line to be used in calculataion 
            Dim BaseLineID As Integer   'Identifier in database (used for ligth counts)
            Dim BaseLineWell As Integer     'Well where the baselineID was performed (used for ligth counts)
            Dim AdjustBaseLineID As Integer 'AG 03/11/2010 (used for dark counts)

            Dim BaseLine() As Integer    'Counts (main photodiode) - Dimension MaxFilters
            Dim DarkCurrent() As Integer     'Counts (main photodiode)  - Dimension MaxFilters
            Dim RefBaseLine() As Integer     'Counts (reference photodiode)  - Dimension MaxFilters
            Dim RefDarkCurrent() As Integer  'Counts (reference photodiode)  - Dimension MaxFilters

            'Recalculation flags
            Dim RecalculationFlag As Boolean
            Dim SampleClassRecalculated As String   'If not recalculation ... = '' // Else ... 'BLANK' or 'CALIB' or 'CTRL' or 'PATIENT'            

            'General information (Ax00 and others)
            Dim LimitAbs As Single
            Dim PathLength As Single

            Dim KineticsIncrease As Single  'Time between 2 readings
            Dim CycleMachine As Single 'AG 11/06/2010 - Analyzer cycle machine
            Dim LinearCorrelationFactor As Single 'Linear correlation factor -constant
            Dim Reagent1Readings As Integer
            Dim ExtrapolationMaximum As Single

            Dim KineticsIncreaseInPause As Single  'AG 23/10/2013 Task #1347. Time between 2 readings in pause mode
            Dim UseFirstR1SampleReadings As Integer  'AG 23/10/2013 Task #1347. Value = 1 means that calculations will use the first 33 readings R1+S. Value = 0 will use the 33 lasts

            Dim DefaultReadNumberR2Added As Integer 'AG 24/10/2013 Task #1347. Default cycle where R2 is added to preparation and fisrt read ('normal' running mode)
            Dim NumberOfReadingsWithR2 As Integer 'AG 24/10/2013 Task #1347. Number of readings after R2 added ('normal' running mode)
            Dim SwReadingsOffset As Integer 'AG 24/10/2013 Task #1347. Offset time-readings (1st reading starts 2 cycle later Sample is added)

        End Structure

        Private Structure WaterData
            'Input fields
            Dim Readings() As Integer    '// Water n. of counts (main photodiode) - Dimension MaxFilters
            Dim RefReadings() As Integer  '// Water n. of counts (reference photodiode) - Dimension MaxFilters

            'Result fields            
            Dim WaterAbs() As Single
            Dim WaterAlarms() As Integer
            Dim ErrorAbs() As String
            Dim WaterDate() As Date

        End Structure

        Private Structure PreparationData
            Dim PreparationID As Integer

            Dim SampleClass As String
            Dim SampleType As String
            Dim MultiItemNumber As Integer  'Current multiitem number
            Dim MaxReplicates As Integer
            Dim ReplicateID As Integer  'Current replicate
            Dim ReRun As Integer        'Rerun number
            Dim WellUsed As Integer     'Well where the water or preparation to calculate was performed
            'Dim RotorTurn As Integer    'Rotor turn without changing rotor (AG 03/05/2010 - Field is deleted from database)
            Dim PostDilutionType As String     ''NONE', 'INC' or 'RED' - tfmwPreloadedMasterData (subItemID = POSTDILUTION_tYPE)

            Dim TestID As Integer       'Test to calculate 
            'Dim CalibratorID As Integer    'AG 25/02/2010 - We dont use this field
            Dim ControlID As Integer

            Dim Readings() As Integer
            Dim RefReadings() As Integer
            Dim PausedReadings() As Boolean   'AG 23/10/2013 Task #1347. Used for kinetics because the paused readings will use the kineticIntervalInPause

            '''''''''
            'Results fields (average and previous sample replicates results)
            '''''''''

            'Replicate results dimensions (ReplicateID) or (ReplicateID, Partial)
            Dim PartialReplicateAbs(,) As Single 'Partial Abs
            Dim InitialReplicateAbs() As Single 'Only applies in kinetics and fixed time blanks
            Dim MainFilterReplicateAbs() As Single  'only for bichromatic end point blanks 
            Dim ReplicateAbs() As Single    'Final Abs
            Dim Reagent1ReplicateAbs() As Single     'only R1 absorbance (reading cycles 1 and 2)
            Dim ReplicateConc() As Single
            Dim rKineticsReplicate() As Single 'Kinetics correlation factor
            Dim KineticsCurve(,) As Single
            Dim SubstrateDepletionReplicate() As Boolean
            Dim ErrorReplicateAbs() As String
            Dim ErrorReplicateConc() As String
            Dim ReplicateDate() As Date
            Dim ErrorCurveReplicate() As Single    '%error between calculated concentration and theorical one by (Replicate)

            Dim RepInUse() As Boolean
            Dim ReplicateAlarms() As Integer
            Dim ReplicateExecutionID() As Integer 'AG 14/08/2010 - Needed and used for multicalibrators save executions and during calibrator (executions) remarks


            'Average results            
            Dim InitialAbs As Single 'Only applies in kinetics and fixed time blanks
            Dim MainFilterAbs As Single  'only for bichromatic end point blanks 
            Dim Abs As Single    'Final Abs             
            Dim Conc As Single
            Dim KineticsLinear As Boolean
            Dim SubstrateDepletion As Integer    '0 - Any replicate / 1 - Some replicates / 2 - All replicates
            Dim ErrorAbs As String
            Dim ErrorConc As String
            Dim AverageDate As Date

            Dim AverageAlarms As Integer

            'AG 12/07/2010
            Dim WorkingReagentAbs As Single
            Dim WorkingReagentReplicateAbs() As Single
            'AG 12/07/2010

            'AG 22/03/2011
            Dim ThermoWarningFlag As Boolean
            Dim PossibleClot As String
            'AG 22/03/2011

        End Structure

        Private Structure CalibratorData
            Dim CalibratorID As Integer
            Dim ExpirationDate As Date  'AG 08/11/2010
            Dim TestID As Integer
            Dim SampleType As String

            'Fields to calculate new calibrator factor or curve
            Dim CalibrationType As String   '// FACT (factor or EXP (experimental))
            Dim NumberOfCalibrators As Integer
            Dim TheoreticalConcentration() As Single
            Dim AlternativeSampleType As String 'Value is "" when not in use

            Dim CalibratorAbs As Single 'Last average abs calculated            
            Dim Factor As Single
            Dim ManualFactorFlag As Boolean 'AG 09/11/2010
            Dim ManualFactor As Single 'AG 09/11/2010
            Dim Curve As CalibrationCurve
            Dim CalibratorDate As Date
            Dim BlankAbsUsed As Single  'Blank absorbance value used for calculate current factor or curve
            '                            Needed for calculate BlankDifferences in new calibrator calculations

            Dim ErrorCalibration As String  'Error during calibration calculation (factor or curve)
            Dim ErrorAbs As String

        End Structure

        Private Structure CalibrationCurve
            Dim Coefficient(,) As Double   '(3,15)  idem Ax5

            Dim CalibratorAbs() As Single
            Dim ErrorAbs() As String
            Dim CurveGrowthType As String
            Dim CurveType As String
            Dim CurveAxisXType As String
            Dim CurveAxisYType As String
            Dim CurvePointsNumber As Integer  'Curve has this curvepoints number
            Dim Points(,) As Single '(1 <Abs or Conc>, CurvePoints <Point value>)
            Dim ConcCurve() As Single    '%error between calculated concentration and theorical one by (Calib points, Replicate)
            Dim ErrorCurve() As Single    '%error between calculated concentration and theorical one by (Calib points) - average
            Dim CorrelationFactor As Single 'AG 01/07/2011 - Curve correlation factor

            '...
        End Structure

        'Internal calculation curve structure
        Private Structure CurveCalculated
            Dim n As Integer
            Dim X() As Single   'Theoretical concentrations to calculate curve
            Dim Y() As Single   'Experimental Absorbances for each calibrator item
            Dim Xmax As Single
            Dim Xmin As Single
            Dim Ymax As Single
            Dim Ymin As Single
            Dim Conversion As Integer   '1: x/y, 2: x/logy, 3: logx/y, 4: logx/logy
            Dim Control As Integer  '1: Correct data
            Dim Points(,) As Single '(1 <Abs or Conc>, CurvePoints <Point value>)

        End Structure


        Private Structure TestOptions
            Dim InUse As Boolean
            Dim Value As Single
        End Structure

        Private Structure TestProzone
            Dim InUse As Boolean
            Dim Ratio As Single
            Dim Cycle1 As Integer
            Dim Cycle2 As Integer
        End Structure

        Private Structure TestSlope
            Dim InUse As Boolean
            Dim A As Single
            Dim B As Single
        End Structure

        Private Structure TestOptionsRange
            Dim InUse As Boolean
            Dim Maximum As Single
            Dim Minimum As Single
        End Structure

        Private Structure TestGenderRange
            Dim InUse As Boolean
            Dim Gender As String
        End Structure

        Private Structure TestData
            Dim TestID As Integer
            Dim SampleType As String
            Dim TestVersion As Integer

            Dim BlankAbs As Single  'Last blank absorbance calculated
            Dim InitialAbs As Single    'Last initial blank abs calculated (only in kinetics and fixed time blanks)
            Dim MainFilterAbs As Single     'Last main filter abs calculated (only in bicrhomatic end point blanks)
            Dim ErrorBlankAbs As String
            Dim BlankDate As Date

            Dim AnalysisMode As String
            Dim AbsorbanceFlag As Boolean
            Dim ReactionType As String  'DEC or INC
            Dim ReadingType As String 'MONO or BIC

            Dim MainWaveLength As Integer   'Calculation class use the position, not the label
            Dim ReferenceWaveLength As Integer 'Calculation class use the position, not the label
            Dim SampleVolume As Single
            Dim ReagentsNumber As Integer
            Dim ReagentVolume() As Single   'Dimension is ReagentsNumber

            ' PENDING: Maybe when ReagentsNumber > 2 these fields has to be arrays
            Dim InitialReadingCycle As Integer
            Dim FinalReadingCycle As Integer
            'Dim ReagentDispensingCycle() As Integer 'AG 25/02/2010 (Dont use this field)

            Dim DilutionFactor As Single
            Dim PredilutionFactor As Single
            Dim PreDilutionFlag As Boolean
            Dim PostDilutionFactor As Single    'Post dilution factor depends on Preparation.PostDilutionType            
            Dim PostSampleVolume As Single  'Post dilution volumes depends on Preparation.PostDilutionType
            Dim PostReagentVolume() As Single   'Post dilution volumes depends on Preparation.PostDilutionType 

            Dim BlankMode As String
            Dim BlankAbsorbanceLimit As TestOptions
            Dim KineticBlankLimit As TestOptions
            Dim LinearityLimit As TestOptions
            Dim DetectionLimit As TestOptions
            Dim SubstrateDepletion As TestOptions
            Dim Prozone As TestProzone
            Dim Slope As TestSlope
            Dim FactorLimit As TestOptionsRange

            'AG 04/06/2010 - Reference values
            Dim ReferenceRange As TestOptionsRange
            Dim BorderLineRange As TestOptionsRange

            Dim WorkingReagentAbs As Single 'AG 12/07/2010


        End Structure

#End Region

#Region "Private variables"
        'Input variables
        Private common() As CommonData  'Array by MultiItemID - always 0 except in calibrator multiitem
        Private water() As WaterData  'Array by MultiItemID - always 0 except in calibrator curves
        Private preparation() As PreparationData    'Array by MultiItemID - always 0 except in calibrator curves

        Private calibrator As CalibratorData    'No array, 1 prep only belongs to a calibrator
        Private test As TestData    'No array, 1 prep only belongs to a test

        'Internal variables
        Private myClassGlobalResult As New GlobalDataTO 'Used only for HasError flag
        Private myBlankDifference As Single = 0 'Blank difference to recalibrate when new blank but not new calibrator
        Private myLocalCurve As CurveCalculated

        Private myExecutionID() As Integer    'Execution to calculate (Array by MultiItemID - always 0 except in calibrator multiitem)
        Private myAnalyzerID As String      'Analyzer who has performed the execution
        Private myWorkSessionID As String   'WorkSession owner of the execution
        Private myOrderTestID As Integer    'OrderTestID to calculate
        Private myRerunNumber As Integer    'Rerun number to calculate
        Private myManualRecalculationsFlag As Boolean = False    'AG 10/09/2010
        Private myAnalyzerModel As String = "" 'SGM 04/03/11

        'Internal pseudo constants        
        Private SYSTEM_ERROR As String = GlobalEnumerates.AbsorbanceErrors.SYSTEM_ERROR.ToString
        Private INCORRECT_DATA As String = GlobalEnumerates.AbsorbanceErrors.INCORRECT_DATA.ToString
        Private ERROR_VALUE As Single = GlobalConstants.CALCULATION_ERROR_VALUE  'AG 13/09/2010 -1
        Private ABS_ID As Integer = 0  'Index for absorbances in calibration curve table
        Private CONC_ID As Integer = 1 'Index for concentrations in calibration curve table
        Private BUGDECIMALS As Single = 0

#End Region

#Region "Properties"
        Public WriteOnly Property AnalyzerModel() As String 'AG 03/07/2012
            Set(ByVal value As String)
                myAnalyzerModel = value
            End Set
        End Property

#End Region

#Region "Public methods"

        ''' <summary>
        ''' Implements the absorbance calculation formula
        ''' </summary>
        ''' <param name="pMainSample"></param>
        ''' <param name="pRefSample"></param>
        ''' <param name="pMainBL"></param>
        ''' <param name="pRefBL"></param>
        ''' <param name="pMainDC"></param>
        ''' <param name="pRefDC"></param>        
        ''' <param name="pPathLenght"></param>        
        ''' <param name="pAbsLimit"></param>
        ''' <param name="pAbsorbanceTestsFlag"></param>
        ''' <param name="pDilutionFactor"></param>
        ''' <returns>GlobalDataTo with Dataset as Calculated absorbance with water correction</returns>
        ''' <remarks>
        ''' Created by: AG 08/02/2010 (Tested OK)
        ''' Modified by: AG 19/05/2010 simplify formula using Sergi Tortosa document (tested OK - same result!!!)
        ''' </remarks>
        Public Function CalculateAbsorbance(ByVal pMainSample As Integer, ByVal pRefSample As Integer, ByVal pMainBL As Integer, _
                                            ByVal pRefBL As Integer, ByVal pMainDC As Integer, ByVal pRefDC As Integer, _
                                            ByVal pWaterAbs As Single, ByVal pPathLenght As Single, ByVal pAbsLimit As Single, _
                                            ByVal pAbsorbanceTestsFlag As Boolean, Optional ByVal pDilutionFactor As Single = 1) As GlobalDataTO

            Dim myLocalGlobalData As New GlobalDataTO

            Try
                Dim absorbance As Single = ERROR_VALUE

                'Validate counts (main and reference diodes)
                myLocalGlobalData = Me.ValidateCounts(pMainBL, pMainDC, pMainSample)
                If Not myLocalGlobalData.HasError Then myLocalGlobalData = Me.ValidateCounts(pRefBL, pRefDC, pRefSample)

                If Not myLocalGlobalData.HasError Then
                    'AG 19/05/2010 - Simplify formula
                    'Dim correctedCounts As Single = 0
                    'correctedCounts = CSng((pMainSample - pMainDC) * ((pRefBL - pRefDC) / (pRefSample - pRefDC)) + pMainDC)
                    'absorbance = CSng((10 / pPathLenght) * Math.Log10((pMainBL - pMainDC) / (correctedCounts - pMainDC)))

                    Dim operation1 As Single = 0
                    Dim operation2 As Single = 0
                    operation1 = CSng((pMainBL - pMainDC) / (pMainSample - pMainDC))
                    operation2 = CSng((pRefSample - pRefDC) / (pRefBL - pRefDC))
                    absorbance = CSng((10 / pPathLenght) * Math.Log10(operation1 * operation2))
                    'END AG 19/05/2010

                    'AG 18/02/2010 - Apply the water absorbance correction (POSTPOSED February 2010 -use ZERO value)
                    pWaterAbs = 0

                    absorbance += -pWaterAbs

                    If absorbance > pAbsLimit Then
                        myLocalGlobalData.HasError = True
                        myLocalGlobalData.ErrorCode = GlobalEnumerates.AbsorbanceErrors.ABS_LIMIT.ToString
                        absorbance = -pAbsLimit
                    End If

                    'Apply optional DilutionFactor (only apply on AbsorbanceTests)
                    If pAbsorbanceTestsFlag Then
                        'TODO
                        'absorbance = absorbance * pDilutionFactor
                    End If

                End If

                'Return calculated absorbance
                myLocalGlobalData.SetDatos = absorbance
                myClassGlobalResult = myLocalGlobalData

            Catch ex As Exception
                Me.CatchLaunched("CalculateAbsorbance", ex)
                myLocalGlobalData = myClassGlobalResult
            End Try

            Return myLocalGlobalData
        End Function

#Region "SERVICE SW"
        ''' <summary>
        ''' Calculate the standard deviation of an array of Single values
        ''' </summary>
        ''' <param name="pValues">array which will calculate the standard deviation </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 09/03/2011
        ''' </remarks>
        Public Function CalculateStdDeviation(ByVal pValues As List(Of Single)) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Dim dblDataAverage As Double = 0
                Dim TotalVariance As Double = 0
                If pValues.Count = 0 Then
                    myResultData.SetDatos = 0
                Else
                    dblDataAverage = pValues.Average
                    For i As Integer = 0 To pValues.Count - 1
                        TotalVariance += Math.Pow(pValues(i) - dblDataAverage, 2)
                    Next
                    myResultData.SetDatos = Math.Sqrt(SafeDivide(TotalVariance, pValues.Count))
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalculationsDelegate.CalculateStdDeviation", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Calculate the variation coefficient of 
        ''' </summary>
        ''' <param name="pStdDesviation">standard deviation </param>
        ''' <param name="pMean">Average </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 31/03/2011
        ''' </remarks>
        Public Function CalculateVariationCoefficient(ByVal pStdDesviation As Single, ByVal pMean As Single) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Dim result As Single
                result = (pStdDesviation / pMean) * 100
                If result < 10 Then
                    myResultData.SetDatos = result
                Else
                    myResultData.HasError = True
                End If
            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalculationsDelegate.CalculateVariationCoefficient", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Divides two numbers in a way that protects against dividing by zero
        ''' </summary>
        ''' <param name="dbl1"></param>
        ''' <param name="dbl2"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 09/03/2011
        ''' </remarks>
        Private Function SafeDivide(ByVal dbl1 As Double, ByVal dbl2 As Double) As Double
            Try
                If (dbl1 = 0) Or (dbl2 = 0) Then Return 0 Else Return dbl1 / dbl2
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalculationsDelegate.SafeDivide", EventLogEntryType.Error, False)
            End Try
        End Function

        ''' <summary>
        ''' Calculate the difference between Setpoint Temperature, which is registered by the sensor, with
        ''' Target Temperature that is wish to reach, and so, to monitorize too
        ''' </summary>
        ''' <param name="pSetPointTemp"></param>
        ''' <param name="pTargetTemp"></param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 21/06/2011</remarks>
        Public Function CalculateThermoCorrection(ByVal pSetPointTemp As Single, ByVal pTargetTemp As Single) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try

                myResultData.SetDatos = pTargetTemp - pSetPointTemp

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalculationsDelegate.CalculateThermoCorrection", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' We obtain the new SetPoint Temperature to be stored in the Instrument
        ''' </summary>
        ''' <param name="pSetPointTemp"></param>
        ''' <param name="pTargetTemp"></param>
        ''' <param name="pMeasuredTemp"></param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 21/06/2011</remarks>
        Public Function CalculateThermoSetPoint(ByVal pSetPointTemp As Single, ByVal pTargetTemp As Single, ByVal pMeasuredTemp As Single) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Dim resultValue As Single

                resultValue = pSetPointTemp - (pMeasuredTemp - pTargetTemp)

                myResultData.SetDatos = resultValue

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalculationsDelegate.CalculateThermoSetPoint", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' We obtain the Temperature that is wish to monitorize from the value registered into the Sensor
        ''' </summary>
        ''' <param name="pSetPointTemp"></param>
        ''' <param name="pTargetTemp"></param>
        ''' <param name="pSensorMeasuredTemp"></param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 21/06/2011</remarks>
        Public Function CalculateTemperatureToMonitor(ByVal pSetPointTemp As Single, ByVal pTargetTemp As Single, ByVal pSensorMeasuredTemp As Single) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try

                ' Option A : Temperature To Monitor as a result of a specific calculation betwwen Setpoint and measured temps (by now CANCELED)
                'myResultData = Me.CalculateThermoCorrection(pSetPointTemp, pTargetTemp)
                'If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                '    Dim myCorrection As Single
                '    Dim resultValue As Single

                '    myCorrection = CType(myResultData.SetDatos, Single)

                '    resultValue = pSensorMeasuredTemp + myCorrection

                '    myResultData.SetDatos = resultValue
                'End If

                ' Option B : Temperature To Monitor is the Measured Temp ! (Meeting 16-06-2011)
                myResultData.SetDatos = pSensorMeasuredTemp

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalculationsDelegate.CalculateTemperatureToMonitor", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Correction of number counts received from photodiodes main and ref to optical centering adjust
        ''' </summary>
        ''' <param name="pNumCountsCurrentMain"></param>
        ''' <param name="pNumCountsCurrentRef"></param>
        ''' <param name="pNumCountsFirstRef"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 22/09/2011
        ''' </remarks>
        Public Function CalculateOpticalCorrection(ByVal pNumCountsCurrentMain As Single, ByVal pNumCountsCurrentRef As Single, ByVal pNumCountsFirstRef As Single) As Single
            Try
                Dim returnValue As Single

                If pNumCountsCurrentMain = 0 And pNumCountsFirstRef = 0 And pNumCountsCurrentRef = 0 Then
                    returnValue = 0
                Else
                    returnValue = pNumCountsCurrentMain * pNumCountsFirstRef / pNumCountsCurrentRef
                End If

                Return returnValue

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalculationsDelegate.CalculateOpticalCorrection", EventLogEntryType.Error, False)
            End Try
        End Function

#End Region


#End Region

#Region "Main calculations methods (private)"

        '''' <summary>
        '''' Save the water results for an executionID (preparationId in Ax5) into database
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>
        '''' TODO
        '''' Modified by: GDS 29/04/2010 (Added pDBConnection parameter)
        '''' </remarks>
        'Private Function SaveWaterResults(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO

        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBTransaction(pDBConnection)

        '        If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

        '            If (Not dbConnection Is Nothing) Then
        '                If Not water Is Nothing Then
        '                    'AnalyzerID and WorkSesionID are used for read and update from database
        '                    'myAnalyzerID 
        '                    'myWorkSessionID 

        '                    For item As Integer = 0 To UBound(water)
        '                        'myExecutionID(item)    'ExecutionID
        '                        'common(item).Well      'Well

        '                        For filterid As Integer = 0 To UBound(water(item).Readings)
        '                            'Water abs and date by filter
        '                            'water(item).WaterAbs(filterid)
        '                            'water(item).WaterDate(filterid)

        '                            'Water abs error by filter
        '                            'water(item).ErrorAbs(filterid)

        '                            'Water abs alarms by filter
        '                            'water(item).WaterAlarms(filterid)
        '                        Next
        '                    Next
        '                End If

        '                If (Not resultData.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If

        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing) And Not (dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        resultData.HasError = True
        '        resultData.ErrorCode = "SYSTEM_ERROR"
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "CalculationsDelegate.SaveWaterResults", EventLogEntryType.Error, False)

        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try

        '    Return resultData

        'End Function


        ''' <summary>
        ''' Save the results for an executionID (preparationID in Ax5)
        ''' into database depending the sample class
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:   AG 02/03/2010 (Tested Pending)
        ''' Modified by: GDS 29/04/2010 (Added pDBConnection parameter)
        ''' </remarks>
        Private Function SaveExecutionResults(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO

            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myClassGlobalResult = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not myClassGlobalResult.HasError) And (Not myClassGlobalResult.SetDatos Is Nothing) Then
                    dbConnection = CType(myClassGlobalResult.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        'ExecutionID, AnalyzerID and WorkSesionID are used for read and update from database

                        Dim myCurveID As Integer = -1   'AG 12/03/2010

                        For item As Integer = 0 To UBound(preparation)
                            'SAVE VALUES IN TABLE tcalcExecutionsPartialResults the partial absorbance results: 
                            'Me.SavePartialAbsorbanceResults(dbConnection, item)
                            'If myClassGlobalResult.HasError Then Exit for

                            'SAVE VALUES IN TABLE twksWSExecutions the replicate results final abs, date, error, alarms                    
                            Me.SaveReplicateResults(dbConnection, item)
                            If myClassGlobalResult.HasError Then Exit For

                            'SAVE VALUES IN TABLE twksResults the orderTestID average results: final abs, date, error, alarms                    
                            Me.SaveAverageResults(dbConnection, item, myCurveID)
                            If myClassGlobalResult.HasError Then Exit For

                            'SAVE VALUES IN TABLE tcalcExecutionsR1Results the reagent1 results: 
                            'Me.SaveReagent1Results(dbConnection, item)
                            'If myClassGlobalResult.HasError Then Exit for
                        Next

                        If (Not myClassGlobalResult.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And Not (dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myClassGlobalResult.HasError = True
                myClassGlobalResult.ErrorCode = "SYSTEM_ERROR"
                myClassGlobalResult.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalculationsDelegate.SaveExecutionResults", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myClassGlobalResult

        End Function




        ''' <summary>
        ''' Main calculation routine. Execute calculation with the data filled in the structures
        ''' 
        ''' After initialize calculation structures this function execute calculations
        ''' - Water absorbance
        ''' - Absorbance calculation
        ''' - If sampleclass = CALIB -> Calibrator factor or curve
        ''' - If sampleclass = CTRL or PATIENT -> Concentration
        ''' </summary>
        ''' <returns>GlobalDataTo with data set as boolean that tell us if calculation succeed or failed</returns>
        ''' <remarks>
        ''' Created by: AG 08/02/2010 (Tested OK)
        ''' </remarks>
        Private Function ExecuteCalculations() As GlobalDataTO
            Dim myLocalGlobalData As New GlobalDataTO
            Try
                'Calculate absorbance (water and sample only when no recalculations)
                If Not common(0).RecalculationFlag Then
                    'AG 18/02/2010 - Water absorbance postposed (February 2010)
                    'First calculate the water absorbance in the current well
                    'Me.CalculateWaterAbsorbance()
                    'If myClassGlobalResult.HasError Then Exit Try 'Error validations
                    'END AG 18/02/2010 

                    'Error validations
                    If myClassGlobalResult.HasError Then Exit Try
                    If preparation(0).ReplicateID > preparation(0).MaxReplicates Then Exit Try

                    'AG 18/02/2010 - Reagent1 only absorbance postposed (February 2010)
                    'If no error calculate the reagent1 and the sample replicate absorbances
                    'Me.CalculateReagent1Absorbance()
                    'If myClassGlobalResult.HasError Then Exit Try 'Error validations
                    'END AG 18/02/2010 

                    Me.CalculateReplicateAbsorbance()   'Sample absorbance
                    If myClassGlobalResult.HasError Then Exit Try 'Error validations
                End If

                'Calculate absorbance average only when no recalculations or recalculating the current sample
                'AG 26/07/2010 - Always calculate avg otherwise in recalculations the Average result arent updated in the affected results
                'If Not common(0).RecalculationFlag Or (preparation(0).SampleClass = common(0).SampleClassRecalculated) Then
                Me.CalculateAbsAverage()
                If myClassGlobalResult.HasError Then Exit Try 'Error validations
                'End If

                Select Case preparation(0).SampleClass
                    Case "BLANK"
                        Me.CheckBlankResults() 'ItemID is always 0 for blanks
                        If myClassGlobalResult.HasError Then Exit Try

                    Case "CALIB"
                        Me.ApplyBlankDifference()
                        If myClassGlobalResult.HasError Then Exit Try

                        Me.CalculateCalibration()
                        If myClassGlobalResult.HasError Then Exit Try

                    Case "CTRL", "PATIENT"
                        Me.CalculateReplicateConcentration()
                        If myClassGlobalResult.HasError Then Exit Try

                        Me.CalculateConcAverage()
                        If myClassGlobalResult.HasError Then Exit Try

                End Select

            Catch ex As Exception
                Me.CatchLaunched("ExecuteCalculations", ex)
            End Try
            myLocalGlobalData = myClassGlobalResult
            Return myLocalGlobalData

        End Function


#End Region

#Region "Initialization methods"
        ''' <summary>
        ''' Initializes related fields between structures
        ''' </summary>
        ''' <param name="pItemsNumberCount"></param>
        ''' <remarks>
        ''' PRE Connection is openned
        ''' Created by AG 25/02/2010 (Tested OK)
        '''                           Pending test: GetLastBlankResults / GetLastCalibResults with data
        ''' Modified by AG 26/07/2010: In recalculations mode read all CLOSED replicates results
        '''                            Initialize PartialAbsorbances
        ''' Modified by AG 30/08/2010: Fix conditions in order to re-calibration works ok during recalculations
        ''' Modified by AG 03/07/2012: Read all previous replicates in one query (previous code executed one query for each previous replicate)
        ''' </remarks>
        Private Sub InitFieldsRelatedWithSeveralStructures(ByVal pdbConnection As SqlClient.SqlConnection, ByVal pItemsNumberCount As Integer)
            Try

                'Test initialization
                ''''''''''''''''''''
                If preparation(0).PostDilutionType Is Nothing Then preparation(0).PostDilutionType = "NONE"

                '1) On calculating a repetition (postdiluted execution) update the sample and reagents volumes
                If preparation(0).PostDilutionType <> "NONE" And test.PostDilutionFactor <> 1 Then
                    test.SampleVolume = test.PostSampleVolume
                    For i As Integer = 0 To UBound(test.ReagentVolume)
                        test.ReagentVolume(i) = test.PostReagentVolume(i)
                    Next
                End If

                'Find the last result for the blank test and fill the fields: BlankAbs, InitialAbs, MainFilterAbs, ErrorBlankAbs, BlankDate
                If preparation(0).SampleClass <> "BLANK" Then
                    Me.GetLastBlankResults(pdbConnection)
                End If


                'Calibration initialization
                ''''''''''''''''''''''''''''
                'AG 30/08/2010 - Initialize last calib results during recalculations
                'If preparation(0).SampleClass = "CTRL" Or preparation(0).SampleClass = "PATIENT" Then
                '    If calibrator.CalibrationType <> "FACTOR" Then
                '        Me.GetLastCalibResults(pdbConnection)
                '    End If
                'End If
                If calibrator.CalibrationType <> "FACTOR" Then
                    Dim GetLastCalibrationResults As Boolean = False
                    Dim LastAcceptedResult As Boolean = True

                    'Search the last accepted result for calibrate this TestID - SampleType
                    If preparation(0).SampleClass = "CTRL" Or preparation(0).SampleClass = "PATIENT" Then
                        GetLastCalibrationResults = True
                        LastAcceptedResult = True

                        'Search the calibration result by OrderTest - RerunNumber
                    ElseIf preparation(0).SampleClass = "CALIB" And common(0).RecalculationFlag Then
                        GetLastCalibrationResults = True
                        LastAcceptedResult = False
                    End If

                    If GetLastCalibrationResults Then
                        Me.GetLastCalibResults(pdbConnection, LastAcceptedResult)
                    End If
                End If
                'END AG 30/08/2010

                'Preparation initialization
                '''''''''''''''''''''''''''
                Dim ex_delegate As New ExecutionsDelegate 'AG 03/07/2012
                Dim myDataTO As New GlobalDataTO 'AG 03/07/2012
                Dim MaxReplicates As Integer = -1 'AG 03/07/2012
                Dim lastReplicateID As Integer = -1 'AG 03/07/2012
                Dim ex_DS As New ExecutionsDS 'AG 03/07/2012

                For pdimension As Integer = 0 To pItemsNumberCount - 1
                    MaxReplicates = preparation(pdimension).MaxReplicates
                    ReDim preparation(pdimension).PartialReplicateAbs(MaxReplicates - 1, Me.GetPartialAbsorbancesNumber(test.AnalysisMode, test.ReadingType) - 1)

                    'AG 14/08/2010 - initialize new field for the current replicate
                    preparation(pdimension).ReplicateExecutionID(preparation(pdimension).ReplicateID - 1) = myExecutionID(pdimension)

                    'AG 03/07/2012
                    lastReplicateID = preparation(pdimension).ReplicateID - 2
                    If common(pdimension).RecalculationFlag Then lastReplicateID = preparation(pdimension).ReplicateID - 1
                    myDataTO = ex_delegate.GetResultsOrderTestReplicate(pdbConnection, myAnalyzerID, myWorkSessionID, myOrderTestID, _
                                                              preparation(pdimension).MultiItemNumber, lastReplicateID + 1, myRerunNumber)
                    If Not myDataTO.HasError And Not myDataTO.SetDatos Is Nothing Then
                        ex_DS = CType(myDataTO.SetDatos, ExecutionsDS)

                        'AG 26/07/2010
                        'Find result for previous replicates for this OrderTestID - MultiItemNumber
                        'If preparation(pdimension).ReplicateID > 1 Then
                        '   For i As Integer = 0 To preparation(pdimension).ReplicateID - 2
                        If preparation(pdimension).ReplicateID > 1 Or common(pdimension).RecalculationFlag Then
                            'Dim lastReplicateID As Integer = preparation(pdimension).ReplicateID - 2 'AG 03/07/2012
                            'If common(pdimension).RecalculationFlag Then lastReplicateID = preparation(pdimension).ReplicateID - 1 'AG 03/07/2012
                            For i As Integer = 0 To lastReplicateID
                                'END AG 26/07/2010

                                'AG 03/07/2012
                                'Dim ex_delegate As New ExecutionsDelegate
                                'Dim myDataTO As New GlobalDataTO

                                ''AG 04/08/2010 (add parameter myRerunNumber) 'Read results for previous replicates
                                'myDataTO = ex_delegate.GetResultsOrderTestReplicate(pdbConnection, myAnalyzerID, myWorkSessionID, myOrderTestID, _
                                '                                          preparation(pdimension).MultiItemNumber, i + 1, myRerunNumber)

                                If Not myDataTO.HasError And Not myDataTO.SetDatos Is Nothing Then
                                    'Dim ex_DS As New ExecutionsDS
                                    'ex_DS = CType(myDataTO.SetDatos, ExecutionsDS)

                                    With preparation(pdimension)
                                        'Read common data for all sample classes
                                        .ReplicateAbs(i) = 0
                                        If Not ex_DS.twksWSExecutions(i).IsABS_ValueNull Then .ReplicateAbs(i) = ex_DS.twksWSExecutions(i).ABS_Value

                                        .ErrorReplicateAbs(i) = ""
                                        If Not ex_DS.twksWSExecutions(i).IsABS_ErrorNull Then .ErrorReplicateAbs(i) = ex_DS.twksWSExecutions(i).ABS_Error

                                        .RepInUse(i) = False
                                        If Not ex_DS.twksWSExecutions(i).IsInUseNull Then .RepInUse(i) = ex_DS.twksWSExecutions(i).InUse

                                        .SubstrateDepletionReplicate(i) = False
                                        If Not ex_DS.twksWSExecutions(i).IsSubstrateDepletionNull Then .SubstrateDepletionReplicate(i) = ex_DS.twksWSExecutions(i).SubstrateDepletion

                                        .ReplicateDate(i) = DateTime.MinValue
                                        If Not ex_DS.twksWSExecutions(i).IsResultDateNull Then .ReplicateDate(i) = ex_DS.twksWSExecutions(i).ResultDate

                                        'AG 14/08/2010
                                        .ReplicateExecutionID(i) = myExecutionID(pdimension)
                                        If Not ex_DS.twksWSExecutions(i).IsExecutionIDNull Then .ReplicateExecutionID(i) = ex_DS.twksWSExecutions(i).ExecutionID
                                        'END AG 14/08/2010

                                        'Read replicate concentration (Ctrl & patient)
                                        If preparation(pdimension).SampleClass = "CTRL" Or preparation(pdimension).SampleClass = "PATIENT" Then
                                            .ReplicateConc(i) = 0
                                            If Not ex_DS.twksWSExecutions(i).IsCONC_ValueNull Then .ReplicateConc(i) = ex_DS.twksWSExecutions(i).CONC_Value

                                            .ErrorReplicateConc(i) = ""
                                            If Not ex_DS.twksWSExecutions(i).IsCONC_ErrorNull Then .ErrorReplicateConc(i) = ex_DS.twksWSExecutions(i).CONC_Error
                                        End If

                                        'Read information for blanks
                                        If preparation(pdimension).SampleClass = "BLANK" Then
                                            .InitialReplicateAbs(i) = 0
                                            If Not ex_DS.twksWSExecutions(i).IsABS_InitialNull Then .InitialReplicateAbs(i) = ex_DS.twksWSExecutions(i).ABS_Initial

                                            '.MainFilterReplicateAbs(i) = ...
                                            .MainFilterReplicateAbs(i) = 0
                                            If Not ex_DS.twksWSExecutions(i).IsABS_MainFilterNull Then .MainFilterReplicateAbs(i) = ex_DS.twksWSExecutions(i).ABS_MainFilter

                                            'AG 12/07/2010 : WorkingReagentReplicateAbs(i) = ...
                                            .WorkingReagentReplicateAbs(i) = 0
                                            If Not ex_DS.twksWSExecutions(i).IsAbs_WorkReagentNull Then .WorkingReagentReplicateAbs(i) = ex_DS.twksWSExecutions(i).Abs_WorkReagent
                                            'END AG 12/07/2010
                                        End If

                                        'AG 26/07/2010 - Initialize all replicates partial absorbances values in recalculations mode
                                        If common(pdimension).RecalculationFlag Then
                                            Select Case test.AnalysisMode
                                                Case "MRFT", "BRFT" 'Fixed time
                                                    .PartialReplicateAbs(i, 0) = .InitialReplicateAbs(i)

                                                Case "MRK", "BRK"   'kinetics
                                                    .KineticsCurve(i, 0) = .InitialReplicateAbs(i)


                                                Case "MREP", "BREP" 'End Point
                                                    If test.ReadingType = "BIC" Then    'Only bicromatic
                                                        .PartialReplicateAbs(i, 1) = .MainFilterReplicateAbs(i)
                                                    End If

                                                Case "BRDIF"    'Bireagent differential
                                                    .PartialReplicateAbs(i, 1) = .WorkingReagentReplicateAbs(i)
                                            End Select
                                        End If
                                        'END AG 26/07/2010

                                    End With

                                Else
                                    myClassGlobalResult = myDataTO
                                    Exit Try
                                End If

                            Next
                        End If 'If preparation(pdimension).ReplicateID > 1 Then

                    Else
                        myClassGlobalResult = myDataTO
                        Exit Try
                    End If
                Next


            Catch ex As Exception
                Me.CatchLaunched("InitFieldsRelatedWithSeveralStructures", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Initializes Test information
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="myExecutionsDS">Typed DataSet ExecutionsDS containing data of the Execution to calculate</param>
        ''' <remarks>
        ''' Created by : DL 22/02/2010
        ''' Modified by: AG 25/02/2010 (Tested OK)
        '''              SA 12/07/2012 - Data obtained for the Test/SampleType also includes field ActiveRangeType; if value of that field
        '''                              is not NULL, then call the function for searching the Reference Ranges that will be used to validate the result
        '''                            - Data obtained for the Test/SampleType also includes field CalibratorType, CalibrationFactor and 
        '''                              SampleTypeAlternative: the corresponding fields are informed in the Calibrator Structure
        ''' AG 19/11/2014 BA-2096 adapt calculations for BA200 use parameter common(pDimension).SwReadingsOffset instead of - 2 fixed!!!!
        ''' </remarks>
        Private Sub InitTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal myExecutionsDS As ExecutionsDS)
            Dim resultData As New GlobalDataTO

            Try
                'Get TestID and SampleType
                test.TestID = preparation(0).TestID
                test.SampleType = preparation(0).SampleType

                'Get all needed TEST data
                Dim myTestsData As New TestsDelegate
                Dim myTestsDS As New TestsDS

                resultData = myTestsData.ReadForCalculations(pdbConnection, test.TestID, test.SampleType)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    myTestsDS = DirectCast(resultData.SetDatos, TestsDS)

                    test.TestVersion = myTestsDS.testCalculations(0).TestVersionNumber
                    test.AnalysisMode = Trim$(myTestsDS.testCalculations.Item(0).AnalysisMode)
                    test.AbsorbanceFlag = myTestsDS.testCalculations.Item(0).AbsorbanceFlag
                    test.ReactionType = Trim$(myTestsDS.testCalculations.Item(0).ReactionType)
                    test.ReadingType = Trim$(myTestsDS.testCalculations.Item(0).ReadingMode)
                    test.ReagentsNumber = CInt(myTestsDS.testCalculations.Item(0).ReagentsNumber)

                    'AG 09/05/2011 - Test programming starts at time 18 (cycle 3): 0s - 1, 9s - 2, 18s - 3, ...
                    'For apply calculations InitialReadingCycle must be 1 so apply -2 offset
                    'AG 19/11/2014 the SwReading offset depends by model, do not fix it in code to 2
                    'test.InitialReadingCycle = myTestsDS.testCalculations(0).FirstReadingCycle - 2
                    test.InitialReadingCycle = myTestsDS.testCalculations(0).FirstReadingCycle - common(0).SwReadingsOffset

                    'AG 09/05/2011 - Test programming starts at time 18 (cycle 3): 0s - 1, 9s - 2, 18s - 3, ...
                    'For apply calculations FinalReadingCycle must be 1 so apply -2 offset
                    test.FinalReadingCycle = test.InitialReadingCycle

                    'AG 19/11/2014 the SwReading offset depends by model, do not fix it in code to 2
                    'If Not myTestsDS.testCalculations(0).IsSecondReadingCycleNull Then test.FinalReadingCycle = myTestsDS.testCalculations(0).SecondReadingCycle - 2
                    If Not myTestsDS.testCalculations(0).IsSecondReadingCycleNull Then test.FinalReadingCycle = myTestsDS.testCalculations(0).SecondReadingCycle - common(0).SwReadingsOffset

                    'Find position for Main and Reference WaveLenght
                    Dim myAnalyzerLedPositionsDS As New AnalyzerLedPositionsDS
                    Dim myAnalyzerLedPositions As New AnalyzerLedPositionsDelegate

                    Dim myMainWaveLength As String = myTestsDS.testCalculations.Item(0).MainWavelength.ToString()
                    If (myMainWaveLength <> "") Then
                        resultData = myAnalyzerLedPositions.GetByWaveLength(pdbConnection, myExecutionsDS.twksWSExecutions.Item(0).AnalyzerID, myMainWaveLength)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            myAnalyzerLedPositionsDS = DirectCast(resultData.SetDatos, AnalyzerLedPositionsDS)
                            test.MainWaveLength = myAnalyzerLedPositionsDS.tcfgAnalyzerLedPositions.Item(0).LedPosition
                        Else
                            myClassGlobalResult = resultData
                            Exit Try
                        End If
                    End If

                    Dim myReferenceWaveLength As String = ""
                    If (test.ReadingType = "BIC") Then
                        myReferenceWaveLength = myTestsDS.testCalculations.Item(0).ReferenceWavelength.ToString()

                        If (myReferenceWaveLength <> "") Then
                            resultData = myAnalyzerLedPositions.GetByWaveLength(pDBConnection, myExecutionsDS.twksWSExecutions.Item(0).AnalyzerID, myReferenceWaveLength)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                myAnalyzerLedPositionsDS = CType(resultData.SetDatos, AnalyzerLedPositionsDS)
                                test.ReferenceWaveLength = myAnalyzerLedPositionsDS.tcfgAnalyzerLedPositions.Item(0).LedPosition
                            Else
                                myClassGlobalResult = resultData
                                Exit Try
                            End If
                        End If
                    End If

                    'Get Test Reagents data
                    ReDim test.ReagentVolume(myTestsDS.testCalculations.Rows.Count - 1)
                    ReDim test.PostReagentVolume(myTestsDS.testCalculations.Rows.Count - 1)    'AG 11/03/2010                    

                    Dim myIndex As Integer = 0
                    For Each myTestReagentsVolumeRow As TestsDS.testCalculationsRow In myTestsDS.testCalculations.Rows
                        test.ReagentVolume(myIndex) = myTestReagentsVolumeRow.ReagentVolume
                        test.PostReagentVolume(myIndex) = test.ReagentVolume(myIndex)   'AG 11/03/2010 -Protection: init post volumes

                        Select Case preparation(0).PostDilutionType
                            Case "INC"
                                If Not myTestReagentsVolumeRow.IsIncPostReagentVolumeNull Then
                                    test.PostReagentVolume(myIndex) = myTestReagentsVolumeRow.IncPostReagentVolume
                                End If

                            Case "RED"
                                If Not myTestReagentsVolumeRow.IsRedPostReagentVolumeNull Then
                                    test.PostReagentVolume(myIndex) = myTestReagentsVolumeRow.RedPostReagentVolume
                                End If

                            Case "NONE"
                                test.PostDilutionFactor = 1
                        End Select

                        myIndex = myIndex + 1
                    Next myTestReagentsVolumeRow


                    'Get Test/SampleType data
                    If Not myTestsDS.testCalculations.Item(0).IsSampleVolumeNull Then
                        test.SampleVolume = myTestsDS.testCalculations.Item(0).SampleVolume
                    End If

                    test.DilutionFactor = 1
                    If Not myTestsDS.testCalculations.Item(0).IsAbsorbanceDilutionFactorNull Then
                        test.DilutionFactor = myTestsDS.testCalculations.Item(0).AbsorbanceDilutionFactor
                    End If

                    test.PreDilutionFlag = False
                    test.PredilutionFactor = 1   'AG 11/03/2010 - Init value
                    If Not myTestsDS.testCalculations.Item(0).IsPredilutionUseFlagNull Then
                        test.PreDilutionFlag = myTestsDS.testCalculations.Item(0).PredilutionUseFlag
                    End If

                    If Not myTestsDS.testCalculations.Item(0).IsPredilutionFactorNull Then
                        test.PredilutionFactor = myTestsDS.testCalculations.Item(0).PredilutionFactor
                    End If

                    If Not myTestsDS.testCalculations.Item(0).IsBlankAbsorbanceLimitNull Then
                        test.BlankAbsorbanceLimit.Value = myTestsDS.testCalculations.Item(0).BlankAbsorbanceLimit
                        test.BlankAbsorbanceLimit.InUse = True
                    End If

                    test.PostDilutionFactor = 1
                    test.PostSampleVolume = test.SampleVolume 'AG 11/03/2010 Initialization
                    Select Case preparation(0).PostDilutionType
                        Case "INC"
                            If Not myTestsDS.testCalculations.Item(0).IsIncPostdilutionFactorNull AndAlso myTestsDS.testCalculations.Item(0).IncPostdilutionFactor > 0 Then
                                'AG 12/01/2012 - case increase: factor 1/F
                                'test.PostDilutionFactor = myTestsDS.testCalculations.Item(0).IncPostdilutionFactor
                                test.PostDilutionFactor = 1 / myTestsDS.testCalculations.Item(0).IncPostdilutionFactor
                                If Not myTestsDS.testCalculations.Item(0).IsIncPostSampleVolumeNull Then test.PostSampleVolume = myTestsDS.testCalculations.Item(0).IncPostSampleVolume 'AG 11/03/2010

                            End If

                        Case "RED"
                            If Not myTestsDS.testCalculations.Item(0).IsRedPostdilutionFactorNull Then
                                If myTestsDS.testCalculations.Item(0).RedPostdilutionFactor > 0 Then
                                    'AG 12/01/2012 - case reduced
                                    'test.PostDilutionFactor = 1 / myTestsDS.testCalculations.Item(0).RedPostdilutionFactor
                                    test.PostDilutionFactor = myTestsDS.testCalculations.Item(0).RedPostdilutionFactor
                                    If Not myTestsDS.testCalculations.Item(0).IsRedPostSampleVolumeNull Then test.PostSampleVolume = myTestsDS.testCalculations.Item(0).RedPostSampleVolume 'AG 11/03/2010
                                End If
                            End If

                        Case "NONE"
                            test.PostDilutionFactor = 1
                    End Select

                    If Not myTestsDS.testCalculations.Item(0).IsKineticBlankLimitNull Then
                        test.KineticBlankLimit.InUse = True
                        test.KineticBlankLimit.Value = myTestsDS.testCalculations.Item(0).KineticBlankLimit
                    Else
                        test.KineticBlankLimit.InUse = False
                    End If

                    test.Prozone.InUse = False
                    test.LinearityLimit.InUse = False
                    test.DetectionLimit.InUse = False
                    test.SubstrateDepletion.InUse = False
                    test.Slope.InUse = False

                    If common(0).CycleMachine > 0 Then  'AG 11/06/2010
                        If Not myTestsDS.testCalculations.Item(0).IsProzoneTime1Null Then
                            'test.Prozone.Cycle1 = myTestsDS.testCalculations.Item(0).ProzoneTime1 
                            test.Prozone.Cycle1 = CInt(myTestsDS.testCalculations.Item(0).ProzoneTime1 / common(0).CycleMachine)

                            If Not myTestsDS.testCalculations.Item(0).IsProzoneTime2Null Then
                                'test.Prozone.Cycle2 = myTestsDS.testCalculations.Item(0).ProzoneTime2
                                test.Prozone.Cycle2 = CInt(myTestsDS.testCalculations.Item(0).ProzoneTime2 / common(0).CycleMachine)
                            End If

                            If Not myTestsDS.testCalculations.Item(0).IsProzoneRatioNull Then
                                'test.Prozone.Ratio = CInt(myTestsDS.testCalculations.Item(0).ProzoneRatio)
                                test.Prozone.Ratio = myTestsDS.testCalculations.Item(0).ProzoneRatio
                                test.Prozone.InUse = True
                            End If
                        End If
                    End If

                    test.BlankMode = "DISTW"
                    If Not myTestsDS.testCalculations.Item(0).IsBlankModeNull Then
                        test.BlankMode = myTestsDS.testCalculations.Item(0).BlankMode
                    End If

                    If Not myTestsDS.testCalculations.Item(0).IsLinearityLimitNull Then
                        test.LinearityLimit.InUse = True
                        test.LinearityLimit.Value = myTestsDS.testCalculations.Item(0).LinearityLimit
                    End If

                    If Not myTestsDS.testCalculations.Item(0).IsDetectionLimitNull Then
                        test.DetectionLimit.Value = myTestsDS.testCalculations.Item(0).DetectionLimit
                        test.DetectionLimit.InUse = True
                    End If

                    If Not myTestsDS.testCalculations.Item(0).IsSubstrateDepletionValueNull Then
                        test.SubstrateDepletion.Value = myTestsDS.testCalculations.Item(0).SubstrateDepletionValue
                        test.SubstrateDepletion.InUse = True
                    End If

                    If Not myTestsDS.testCalculations.Item(0).IsSlopeFactorANull Then
                        test.Slope.A = myTestsDS.testCalculations.Item(0).SlopeFactorA

                        If Not myTestsDS.testCalculations.Item(0).IsSlopeFactorBNull Then
                            test.Slope.B = myTestsDS.testCalculations.Item(0).SlopeFactorB
                            test.Slope.InUse = True
                        End If
                    End If

                    test.FactorLimit.InUse = False
                    If Not myTestsDS.testCalculations.Item(0).IsFactorUpperLimitNull Then
                        test.FactorLimit.Maximum = myTestsDS.testCalculations.Item(0).FactorUpperLimit

                        If Not myTestsDS.testCalculations.Item(0).IsFactorLowerLimitNull Then
                            test.FactorLimit.Minimum = myTestsDS.testCalculations.Item(0).FactorLowerLimit
                            test.FactorLimit.InUse = True
                        End If
                    End If
                    'END AG 11/06/2010

                    'Get Test Calibration data
                    calibrator.TestID = test.TestID
                    calibrator.SampleType = test.SampleType

                    calibrator.Factor = 1
                    calibrator.AlternativeSampleType = ""
                    calibrator.CalibrationType = myTestsDS.testCalculations.Item(0).CalibratorType
                    If (myTestsDS.testCalculations.Item(0).CalibratorType = "FACTOR") Then
                        calibrator.Factor = myTestsDS.testCalculations.Item(0).CalibrationFactor

                    ElseIf (myTestsDS.testCalculations.Item(0).CalibratorType = "ALTERNATIV") Then
                        calibrator.AlternativeSampleType = myTestsDS.testCalculations.Item(0).SampleTypeAlternative
                    End If

                    'DL 29/06/2010
                    Dim myActiveRangeType As String = String.Empty
                    'If (Not myTestsDS.testCalculations.Item(0).IsActiveRangeTypeNull) Then myActiveRangeType = myTestsDS.testCalculations.Item(0).ActiveRangeType

                    'If (preparation(0).SampleClass = "CTRL") OrElse (preparation(0).SampleClass = "PATIENT" AndAlso myActiveRangeType <> String.Empty) Then
                    InitReferenceRange(pDBConnection, myExecutionsDS, test.TestID, test.SampleType, "STD", myActiveRangeType)
                    'End If
                Else
                    myClassGlobalResult = resultData
                End If

            Catch ex As Exception
                'AG 24/02/2010
                'resultData.HasError = True
                'resultData.ErrorCode = "SYSTEM_ERROR"
                'resultData.ErrorMessage = ex.Message
                '
                'Dim myLogAcciones As New ApplicationLogManager()
                'myLogAcciones.CreateLogActivity(ex.Message, "CalculationsDelegate.InitCommon", EventLogEntryType.Error, False)
                Me.CatchLaunched("InitTest", ex)
            End Try

        End Sub

        ''' <summary>
        ''' Initializes reference range
        ''' (PRE: Connection is also opened!!!)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExecutionsDS"></param>
        ''' <param name="pTestID"></param> 
        ''' <param name="pSampleType"></param> 
        ''' <param name="pTestType"></param>
        ''' <remarks>
        ''' </remarks>
        ''' Created by : DL 23/08/2010
        ''' Modified by: AG 26/08/2010 - Prepare to use also with Controls
        '''              DL 08/09/2010 - Prepare to use with different TestTypes
        '''              AG 14/09/2010 - Verify if field ActiveRangeType is informed before get the value to avoid uncontrolled
        '''                              exceptions (for STD and CALC Test Types)
        '''              DL 03/11/2010 - Get ActiveRangeType for ISE Tests
        '''              SA 25/01/2011 - Changed the way of getting the Reference Range Interval (for Patient Samples)
        '''              SA 12/07/2012 - Added optional parameter for the type of Reference Range defined for the Test/SampleType; if it 
        '''                              is informed it is not needed to search it in DB
        Public Sub InitReferenceRange(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionsDS As ExecutionsDS, ByVal pTestID As Integer, _
                                      ByVal pSampleType As String, Optional ByVal pTestType As String = "STD", Optional ByVal pRangeType As String = "")
            Dim resultData As New GlobalDataTO

            Try
                'Initialize InUse
                test.ReferenceRange.InUse = False
                test.BorderLineRange.InUse = False

                If (preparation(0).SampleClass = "PATIENT") Then
                    If (pRangeType = String.Empty) Then
                        If (pTestType = "STD") Then
                            Dim myTestSamplesDelegate As New TestSamplesDelegate
                            resultData = myTestSamplesDelegate.GetDefinition(pDBConnection, pTestID, pSampleType)

                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim myTestSamplesDS As TestSamplesDS = DirectCast(resultData.SetDatos, TestSamplesDS)
                                If (myTestSamplesDS.tparTestSamples.Rows.Count > 0) Then
                                    If (Not myTestSamplesDS.tparTestSamples.First.IsActiveRangeTypeNull) Then pRangeType = myTestSamplesDS.tparTestSamples.First.ActiveRangeType.Trim
                                End If
                            End If

                        ElseIf (pTestType = "CALC") Then
                            pSampleType = ""

                            Dim myCalculatedTestsDelegate As New CalculatedTestsDelegate
                            resultData = myCalculatedTestsDelegate.GetCalcTest(pDBConnection, pTestID)

                            If (Not resultData.HasError) Then
                                Dim myCalculatedTestsDS As CalculatedTestsDS = DirectCast(resultData.SetDatos, CalculatedTestsDS)
                                If (myCalculatedTestsDS.tparCalculatedTests.Rows.Count > 0) Then
                                    If (Not myCalculatedTestsDS.tparCalculatedTests.First.IsActiveRangeTypeNull) Then pRangeType = myCalculatedTestsDS.tparCalculatedTests.First.ActiveRangeType.Trim
                                End If
                            End If

                        ElseIf (pTestType = "ISE") Then
                            Dim myISETestSamplesDelegate As New ISETestSamplesDelegate
                            resultData = myISETestSamplesDelegate.GetListByISETestID(pDBConnection, pTestID, pSampleType)

                            If (Not resultData.HasError) Then
                                Dim myISETestSamplesDS As ISETestSamplesDS = DirectCast(resultData.SetDatos, ISETestSamplesDS)
                                If (myISETestSamplesDS.tparISETestSamples.Rows.Count > 0) Then
                                    If (Not myISETestSamplesDS.tparISETestSamples.First.IsActiveRangeTypeNull) Then pRangeType = myISETestSamplesDS.tparISETestSamples.First.ActiveRangeType.Trim
                                End If
                            End If
                        End If
                    End If

                    If (pRangeType <> String.Empty) Then
                        'Get the Reference Range for the Test/SampleType according the TestType and the Type of Range
                        Dim myOrderTestsDelegate As New OrderTestsDelegate
                        resultData = myOrderTestsDelegate.GetReferenceRangeInterval(pDBConnection, pExecutionsDS.twksWSExecutions(0).OrderTestID, pTestType, _
                                                                                    pTestID, pSampleType, pRangeType)
                        If (Not resultData.HasError AndAlso Not resultData.HasError) Then
                            Dim myTestRefRangesDS As TestRefRangesDS= DirectCast(resultData.SetDatos, TestRefRangesDS)

                            If (myTestRefRangesDS.tparTestRefRanges.Rows.Count = 0) Then
                                test.ReferenceRange.Minimum = -1
                                test.ReferenceRange.Maximum = -1

                                test.BorderLineRange.Minimum = -1
                                test.BorderLineRange.Maximum = -1
                            Else
                                test.ReferenceRange.Minimum = myTestRefRangesDS.tparTestRefRanges(0).NormalLowerLimit
                                test.ReferenceRange.Maximum = myTestRefRangesDS.tparTestRefRanges(0).NormalUpperLimit

                                test.BorderLineRange.Minimum = myTestRefRangesDS.tparTestRefRanges(0).BorderLineLowerLimit
                                test.BorderLineRange.Maximum = myTestRefRangesDS.tparTestRefRanges(0).BorderLineUpperLimit
                            End If
                        End If

                        If (test.ReferenceRange.Minimum <> -1 AndAlso test.ReferenceRange.Maximum <> -1) Then test.ReferenceRange.InUse = True
                        If (test.BorderLineRange.Minimum <> -1 AndAlso test.BorderLineRange.Maximum <> -1) Then test.BorderLineRange.InUse = True
                    End If

                ElseIf (preparation(0).SampleClass = "CTRL") Then
                    InitReferenceControlRanges(pDBConnection)
                End If
            Catch ex As Exception
                Me.CatchLaunched("InitReferenceRange", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Initializes the reference ranges for controls!!!
        ''' PRE: Connection is opened!!!
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <remarks>
        ''' Created by:  AG 26/08/2010
        ''' Modified by: SA 12/07/2012 - Inform parameter ControlID when calling function GetControlsNEW; removed the searching of the Control
        '''                              using LinQ and the Not NULL validations: fields Min/Max Concentration do not allow NULL values
        ''' </remarks>
        Private Sub InitReferenceControlRanges(ByVal pDBConnection As SqlClient.SqlConnection)
            Try
                'Read Test-Sample controls definition
                Dim myGlobal As New GlobalDataTO
                Dim myTestCtrl As New TestControlsDelegate

                myGlobal = myTestCtrl.GetControlsNEW(pDBConnection, "STD", preparation(0).TestID, preparation(0).SampleType, preparation(0).ControlID)
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    Dim myResultDS As TestControlsDS = DirectCast(myGlobal.SetDatos, TestControlsDS)

                    If (myResultDS.tparTestControls.Rows.Count > 0) Then
                        test.ReferenceRange.Minimum = myResultDS.tparTestControls(0).MinConcentration
                        test.ReferenceRange.Maximum = myResultDS.tparTestControls(0).MaxConcentration
                        test.ReferenceRange.InUse = True

                        ''Search by ControlID (LinQ)
                        'Dim ctrlDefinition As List(Of TestControlsDS.tparTestControlsRow)
                        'ctrlDefinition = (From a In myResultDS.tparTestControls _
                        '                 Where a.ControlID = preparation(0).ControlID _
                        '                Select a).ToList

                        ''If found, initialize reference ranges
                        'If (ctrlDefinition.Count > 0) Then
                        '    With ctrlDefinition(0)
                        '        If (Not ctrlDefinition(0).IsMinConcentrationNull And Not ctrlDefinition(0).IsMaxConcentrationNull) Then
                        '            test.ReferenceRange.Minimum = ctrlDefinition(0).MinConcentration
                        '            test.ReferenceRange.Maximum = ctrlDefinition(0).MaxConcentration
                        '            test.ReferenceRange.InUse = True
                        '        End If
                        '    End With
                        'End If
                    End If
                End If
            Catch ex As Exception
                Me.CatchLaunched("InitReferenceControlRanges", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Find and get the last blank results for the current test
        ''' </summary>
        ''' <remarks>
        ''' Created by AG 26/02/2010 (Tested OK)
        ''' PRE: connection is opened
        ''' </remarks>
        Private Sub GetLastBlankResults(ByVal pDBConnection As SqlClient.SqlConnection)
            Try
                'Initiate values
                test.BlankAbs = 0
                test.InitialAbs = 0
                test.MainFilterAbs = 0
                test.WorkingReagentAbs = 0 'AG 12/07/2010
                test.ErrorBlankAbs = ""
                test.BlankDate = DateTime.MinValue

                Dim dbConnection As New SqlClient.SqlConnection
                Dim Res_delegate As New ResultsDelegate
                Dim localres As New GlobalDataTO

                localres = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not localres.HasError) And (Not localres.SetDatos Is Nothing) Then
                    dbConnection = CType(localres.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'Find the last OrderTestID belongs to the blank testId in analyzer (AG 19/11/2012 - inform as TRUE the parameter ignoreOrderTestStatus)
                        localres = Res_delegate.GetLastExecutedBlank(dbConnection, test.TestID, test.TestVersion, myAnalyzerID, "", False, True)
                        If Not localres.HasError And Not localres.SetDatos Is Nothing Then
                            Dim AdditionalElements As New WSAdditionalElementsDS
                            AdditionalElements = CType(localres.SetDatos, WSAdditionalElementsDS)

                            If AdditionalElements.WSAdditionalElementsTable.Rows.Count > 0 Then
                                Dim foundOrderTestId As Integer = AdditionalElements.WSAdditionalElementsTable(0).PreviousOrderTestID

                                'Get blank results
                                localres = Res_delegate.GetAcceptedResults(dbConnection, foundOrderTestId, False, False)
                                If Not localres.HasError And Not localres.SetDatos Is Nothing Then
                                    Dim lastResults As New ResultsDS
                                    lastResults = CType(localres.SetDatos, ResultsDS)

                                    If Not lastResults.twksResults(0).IsABSValueNull Then test.BlankAbs = lastResults.twksResults(0).ABSValue
                                    If Not lastResults.twksResults(0).IsABS_InitialNull Then test.InitialAbs = lastResults.twksResults(0).ABS_Initial
                                    If Not lastResults.twksResults(0).IsABS_MainFilterNull Then test.MainFilterAbs = lastResults.twksResults(0).ABS_MainFilter
                                    If Not lastResults.twksResults(0).IsABS_ErrorNull Then test.ErrorBlankAbs = lastResults.twksResults(0).ABS_Error
                                    If Not lastResults.twksResults(0).IsResultDateTimeNull Then test.BlankDate = lastResults.twksResults(0).ResultDateTime
                                    If Not lastResults.twksResults(0).IsAbs_WorkReagentNull Then test.WorkingReagentAbs = lastResults.twksResults(0).Abs_WorkReagent 'AG 12/07/2010

                                End If
                            End If 'If AdditionalElements.WSAdditionalElementsTable.Rows.Count > 0 Then
                        End If

                    End If
                End If

            Catch ex As Exception
                Me.CatchLaunched("GetLastBlankResults", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Find and get the last calibrator results for the current test
        ''' </summary>
        ''' <remarks>
        ''' Created by AG 26/02/2010 (Tested OK)
        ''' Modified by AG 30/08/2010 - Add parameter LastAcceptedResult
        ''' TRUE -> original function ... get the accepted calibration result (NORMAL)
        ''' FALSE -> new ... get the current (OrderTestID, RerunNumber) calibration result (CALIB RECALCULATIONS)
        ''' 
        ''' Modified by AG 04/09/2010 - Dont update the curve definition (curve type, axis values and curve growth type) if are already informed
        ''' </remarks>
        Private Sub GetLastCalibResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pLastAcceptedResult As Boolean)
            Try
                'Initiate values
                If calibrator.NumberOfCalibrators = 1 Then
                    calibrator.Factor = 1
                    calibrator.BlankAbsUsed = 0
                    calibrator.ErrorCalibration = ""
                    calibrator.CalibratorDate = DateTime.MinValue
                Else
                    ReDim calibrator.Curve.Points(1, calibrator.Curve.CurvePointsNumber)
                    calibrator.BlankAbsUsed = 0
                    calibrator.ErrorCalibration = ""
                    calibrator.CalibratorDate = DateTime.MinValue
                End If


                Dim dbConnection As New SqlClient.SqlConnection
                Dim res_delegate As New ResultsDelegate
                Dim localres As New GlobalDataTO

                localres = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not localres.HasError) And (Not localres.SetDatos Is Nothing) Then
                    dbConnection = CType(localres.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim someError As Boolean = True

                        'ORIGINAL: Search the last ACCEPTED calibrator for the current test-sampletype
                        If pLastAcceptedResult Then
                            'Find the last OrderTestID belongs to the calibrator: testId-sampletype in analyzer (AG 19/11/2012 - inform as TRUE the parameter ignoreOrderTestStatus)                       
                            localres = res_delegate.GetLastExecutedCalibrator(dbConnection, test.TestID, calibrator.SampleType, test.TestVersion, myAnalyzerID, "", False, True)
                            If Not localres.HasError And Not localres.SetDatos Is Nothing Then
                                Dim AdditionalElements As New WSAdditionalElementsDS
                                AdditionalElements = CType(localres.SetDatos, WSAdditionalElementsDS)

                                If AdditionalElements.WSAdditionalElementsTable.Rows.Count > 0 Then
                                    Dim foundOrderTestId As Integer = AdditionalElements.WSAdditionalElementsTable(0).PreviousOrderTestID
                                    'AG 15/05/202 -Ignore validation status to get the calibrator results
                                    'localres = res_delegate.GetAcceptedResults(dbConnection, foundOrderTestId, False, False)
                                    localres = res_delegate.GetAcceptedResults(dbConnection, foundOrderTestId, True, False)
                                    someError = False
                                Else
                                    calibrator.ErrorCalibration = GlobalEnumerates.CalibrationFactorErrors.ABS_ERROR.ToString
                                End If 'If AdditionalElements.WSAdditionalElementsTable.Rows.Count > 0 Then
                            End If
                            'RECALCULATE CALIBRATOR: Search the last RESULT for the current OrderTestID - RerunNumber
                        Else
                            localres = res_delegate.ReadByOrderTestIDandRerunNumber(pDBConnection, myOrderTestID, myRerunNumber)
                            someError = False
                        End If 'If pLastAcceptedResult Then

                        If Not someError Then

                            If Not localres.HasError And Not localres.SetDatos Is Nothing Then
                                Dim lastResults As New ResultsDS
                                lastResults = CType(localres.SetDatos, ResultsDS)
                                If lastResults.twksResults.Rows.Count > 0 Then
                                    If Not lastResults.twksResults(0).IsCalibratorBlankAbsUsedNull Then calibrator.BlankAbsUsed = lastResults.twksResults(0).CalibratorBlankAbsUsed
                                    If Not lastResults.twksResults(0).IsCalibrationErrorNull Then calibrator.ErrorCalibration = lastResults.twksResults(0).CalibrationError
                                    If Not lastResults.twksResults(0).IsManualResultFlagNull Then calibrator.ManualFactorFlag = lastResults.twksResults(0).ManualResultFlag 'AG 09/11/2010
                                    If calibrator.ManualFactorFlag Then calibrator.CalibrationType = "FACTOR" 'AG 16/05/2012
                                    If Not lastResults.twksResults(0).IsManualResultNull Then calibrator.ManualFactor = lastResults.twksResults(0).ManualResult 'AG 09/11/2010

                                    'AG 04/09/2010 - Add new condition
                                    'If Not lastResults.twksResults(0).IsCurveGrowthTypeNull Then calibrator.Curve.CurveGrowthType = lastResults.twksResults(0).CurveGrowthType
                                    'If Not lastResults.twksResults(0).IsCurveTypeNull Then calibrator.Curve.CurveType = lastResults.twksResults(0).CurveType
                                    'If Not lastResults.twksResults(0).IsCurveAxisXTypeNull Then calibrator.Curve.CurveAxisXType = lastResults.twksResults(0).CurveAxisXType
                                    'If Not lastResults.twksResults(0).IsCurveAxisYTypeNull Then calibrator.Curve.CurveAxisXType = lastResults.twksResults(0).CurveAxisYType
                                    If Not (common(0).RecalculationFlag And preparation(0).SampleClass = "CALIB") Then
                                        If Not lastResults.twksResults(0).IsCurveGrowthTypeNull Then calibrator.Curve.CurveGrowthType = lastResults.twksResults(0).CurveGrowthType
                                        If Not lastResults.twksResults(0).IsCurveTypeNull Then calibrator.Curve.CurveType = lastResults.twksResults(0).CurveType
                                        If Not lastResults.twksResults(0).IsCurveAxisXTypeNull Then calibrator.Curve.CurveAxisXType = lastResults.twksResults(0).CurveAxisXType
                                        If Not lastResults.twksResults(0).IsCurveAxisYTypeNull Then calibrator.Curve.CurveAxisXType = lastResults.twksResults(0).CurveAxisYType
                                        If Not lastResults.twksResults(0).IsCurveCorrelationNull Then calibrator.Curve.CorrelationFactor = lastResults.twksResults(0).CurveCorrelation 'AG 01/07/2011
                                    End If
                                    'END AG 04/09/2010

                                    If Not lastResults.twksResults(0).IsResultDateTimeNull Then calibrator.CalibratorDate = lastResults.twksResults(0).ResultDateTime 'AG 11/03/2010

                                    'if calibrator NOT is multitiem then we already get the results. Otherwise we need to get the curve results
                                    If calibrator.NumberOfCalibrators = 1 Then
                                        If Not lastResults.twksResults(0).IsCalibratorFactorNull Then calibrator.Factor = lastResults.twksResults(0).CalibratorFactor
                                        If Not lastResults.twksResults(0).IsABSValueNull Then calibrator.CalibratorAbs = lastResults.twksResults(0).ABSValue
                                        If Not lastResults.twksResults(0).IsABS_ErrorNull Then calibrator.ErrorAbs = lastResults.twksResults(0).ABS_Error

                                    Else
                                        For i As Integer = 0 To lastResults.twksResults.Rows.Count - 1
                                            If Not lastResults.twksResults(i).IsABSValueNull Then calibrator.Curve.CalibratorAbs(i) = lastResults.twksResults(i).ABSValue
                                            If Not lastResults.twksResults(i).IsABS_ErrorNull Then calibrator.Curve.ErrorAbs(i) = lastResults.twksResults(i).ABS_Error
                                            If Not lastResults.twksResults(i).IsCONC_ValueNull Then calibrator.Curve.ConcCurve(i) = lastResults.twksResults(i).CONC_Value
                                            If Not lastResults.twksResults(i).IsRelativeErrorCurveNull Then calibrator.Curve.ErrorCurve(i) = lastResults.twksResults(i).RelativeErrorCurve
                                        Next i

                                        'AG 04/09/2010 - Add new condition (read curve only if no calib recalculations)
                                        If Not (common(0).RecalculationFlag And preparation(0).SampleClass = "CALIB") Then
                                            'Read the curve points CurveResultID
                                            'Dim Points(,) As Single '(1 <Abs or Conc>, CurvePoints <Point value>)
                                            If Not lastResults.twksResults(0).IsCurveResultsIDNull Then
                                                Dim curve_del As New CurveResultsDelegate

                                                localres = curve_del.GetResults(dbConnection, lastResults.twksResults(0).CurveResultsID)
                                                If Not localres.HasError And Not localres.SetDatos Is Nothing Then
                                                    Dim curveDS As New CurveResultsDS
                                                    curveDS = CType(localres.SetDatos, CurveResultsDS)

                                                    ReDim calibrator.Curve.Points(1, curveDS.twksCurveResults.Rows.Count - 1)
                                                    For i As Integer = 0 To curveDS.twksCurveResults.Rows.Count - 1
                                                        calibrator.Curve.Points(ABS_ID, i) = curveDS.twksCurveResults(i).ABSValue
                                                        calibrator.Curve.Points(CONC_ID, i) = curveDS.twksCurveResults(i).CONCValue
                                                    Next
                                                End If

                                            Else
                                                myClassGlobalResult.HasError = True
                                                myClassGlobalResult.ErrorCode = GlobalEnumerates.AbsorbanceErrors.INCORRECT_DATA.ToString
                                            End If
                                        End If 'AG 04/09/2010

                                    End If 'If calibrator.NumberOfCalibrators = 1 Then
                                End If 'If lastResults.twksResults.Rows.Count > 0 Then
                            Else
                                calibrator.ErrorCalibration = GlobalEnumerates.CalibrationFactorErrors.ABS_ERROR.ToString
                            End If 'If Not localres.HasError And Not localres.SetDatos Is Nothing Then

                        End If 'If Not someError Then

                    End If 'If (Not dbConnection Is Nothing) Then
                End If 'If (Not localres.HasError) And (Not localres.SetDatos Is Nothing) Then

            Catch ex As Exception
                Me.CatchLaunched("GetLastCalibResults", ex)
            End Try
        End Sub



#End Region

#Region "Private calculation methods"
        ''' <summary>
        ''' NOT USED!!!!!
        ''' Calculate the water absorbance with the data contained in waterData and commonData (base line)
        ''' Updates waterresults internal variable
        ''' </summary>
        ''' <remarks>        
        ''' Created by: AG 08/02/2010 (Tested OK)
        ''' </remarks>
        Private Sub CalculateWaterAbsorbance()
            Try
                'Validate Data (water, base line and dark current)

                Dim Validated As Boolean = False

                For itemID As Integer = 0 To UBound(water)
                    Dim myGlobal As New GlobalDataTO
                    myGlobal = Me.ValidateWaterData(water(itemID), itemID)
                    If Not myGlobal.HasError Then
                        Validated = CType(myGlobal.SetDatos, Boolean)
                    End If

                    'If valid data calculate water absorbance
                    If Validated Then

                        'Redim result fields
                        ReDim water(itemID).WaterAbs(UBound(water(itemID).Readings))
                        ReDim water(itemID).WaterAlarms(UBound(water(itemID).Readings))
                        ReDim water(itemID).ErrorAbs(UBound(water(itemID).Readings))
                        ReDim water(itemID).WaterDate(UBound(water(itemID).Readings))

                        For i As Integer = 0 To UBound(water(itemID).Readings)
                            'For each wavelength calculate the water absorbance (no absorbance test flag for water)
                            'For calculate the water abs the parameter pWaterAbs = 0
                            myGlobal = Me.CalculateAbsorbance(water(itemID).Readings(i), water(itemID).RefReadings(i), common(itemID).BaseLine(i), common(itemID).RefBaseLine(i), _
                                                     common(itemID).DarkCurrent(i), common(itemID).RefDarkCurrent(i), 0, common(itemID).PathLength, common(itemID).LimitAbs, False)

                            If Not myGlobal.HasError Then
                                water(itemID).WaterAbs(i) = CType(myGlobal.SetDatos, Single)
                                water(itemID).WaterDate(i) = Now
                            Else
                                myClassGlobalResult = myGlobal
                                water(itemID).WaterAbs(i) = ERROR_VALUE
                                water(itemID).ErrorAbs(i) = myGlobal.ErrorCode
                                Exit For
                            End If
                        Next

                    End If
                Next

            Catch ex As Exception
                Me.CatchLaunched("CalculateWaterAbsorbance", ex)

                For item As Integer = 0 To UBound(water)
                    For i As Integer = 0 To UBound(water(item).Readings)
                        water(item).WaterAbs(i) = ERROR_VALUE
                        water(item).ErrorAbs(i) = GlobalEnumerates.AbsorbanceErrors.SYSTEM_ERROR.ToString
                    Next
                Next

            End Try
        End Sub


        ''' <summary>
        ''' Calculate the sample absorbance for all analysis mode
        ''' ''' Updates preparationresults internal variable
        ''' </summary>
        ''' <remarks>
        ''' Created by: AG 09/02/2010 (Tested OK)
        ''' </remarks>
        Private Sub CalculateReplicateAbsorbance()
            Try

                'Calculate the max item number
                Dim maxItemNumber As Integer = 1
                If preparation(0).SampleClass = "CALIB" Then maxItemNumber = calibrator.NumberOfCalibrators
                'Current replicate
                Dim ReplID As Integer = preparation(0).ReplicateID - 1
                Dim filterID As Integer = 1

                For itemID As Integer = 0 To maxItemNumber - 1
                    Dim isKinetics As Boolean = False
                    If test.AnalysisMode = "BRK" Or test.AnalysisMode = "MRK" Then isKinetics = True

                    'If analysismode is not kinetics (monoreagent or bireagent)
                    If Not isKinetics Then
                        'Calculate the partial abs to calculate depending on reading type monochromatic or bichromatic)
                        Dim maxPartial As Integer = 1
                        Dim readingCycle As Integer = 1

                        maxPartial = Me.GetPartialAbsorbancesNumber(test.AnalysisMode, test.ReadingType)
                        For partialIndex As Integer = 0 To maxPartial - 1
                            Select Case partialIndex
                                Case 0
                                    readingCycle = test.InitialReadingCycle - 1

                                    If maxPartial = 1 Then
                                        'End point monochromatic
                                        filterID = test.MainWaveLength - 1

                                    ElseIf maxPartial = 2 Then
                                        'Bireagent differential or Fixed Time (Monochromatic) // End Point (Bichromatic)
                                        If String.Equals(test.AnalysisMode, "MREP") Or String.Equals(test.AnalysisMode, "BREP") Then    'The only bicrhomatic analysis mode are MREP & BREP
                                            filterID = test.ReferenceWaveLength - 1
                                            'AG 11/03/2010 - For EndPoint bicrhomatic use the reading previous OF the final cycle with referencewavelenght
                                            readingCycle = test.InitialReadingCycle - 2

                                        Else
                                            filterID = test.MainWaveLength - 1
                                        End If

                                    ElseIf maxPartial = 4 Then
                                        '(Bireagent Differential or Fixed Time) Bichromatic (Reference wave lenght & InitialTime - 1 cycle)
                                        filterID = test.ReferenceWaveLength - 1
                                        'readingCycle = test.InitialReadingCycle - 2
                                        If test.InitialReadingCycle > 1 Then 'Normal case
                                            readingCycle = test.InitialReadingCycle - 2
                                        Else 'Protection case agaisnt system errors 
                                            readingCycle = test.InitialReadingCycle
                                        End If

                                    End If 'If maxPartial = 1 Then

                                Case 1
                                    If maxPartial = 2 Then
                                        readingCycle = test.FinalReadingCycle - 1

                                        'AG 11/03/2010 - For EndPoint bicrhomatic use the reading OF the final cycle with mainwavelenght
                                        If test.AnalysisMode = "MREP" Or test.AnalysisMode = "BREP" Then
                                            readingCycle = test.InitialReadingCycle - 1
                                        End If

                                        filterID = test.MainWaveLength - 1

                                    ElseIf maxPartial = 4 Then
                                        '(Bireagent Differential or Fixed Time) Bichromatic (Main wave lenght & InitialTime)
                                        filterID = test.MainWaveLength - 1
                                        readingCycle = test.InitialReadingCycle - 1
                                    End If

                                Case 2
                                    If maxPartial = 4 Then
                                        '(Bireagent Differential or Fixed Time) Bichromatic (Reference wave lenght & FinalTime - 1 cycle)
                                        filterID = test.ReferenceWaveLength - 1
                                        readingCycle = test.FinalReadingCycle - 2
                                    End If

                                Case 3
                                    If maxPartial = 4 Then
                                        '(Bireagent Differential or Fixed Time) Bichromatic (Main wave lenght & FinalTime)
                                        filterID = test.MainWaveLength - 1
                                        readingCycle = test.FinalReadingCycle - 1
                                    End If

                                Case Else

                            End Select

                            Dim auxABS As Single = 0
                            Dim auxGlobal As New GlobalDataTO

                            'AG 18/02/2010 - Instead of water(itemID).WaterAbs(filterID) use 0 for water absorbance (Water absorbance is postposed (February 2010)
                            auxGlobal = Me.CalculateAbsorbance(preparation(itemID).Readings(readingCycle), preparation(itemID).RefReadings(readingCycle), _
                                                   common(itemID).BaseLine(filterID), common(itemID).RefBaseLine(filterID), _
                                                   common(itemID).DarkCurrent(filterID), common(itemID).RefDarkCurrent(filterID), 0, _
                                                   common(itemID).PathLength, common(itemID).LimitAbs, test.AbsorbanceFlag, test.DilutionFactor)

                            'Check if errors
                            If auxGlobal.HasError Then

                                preparation(itemID).PartialReplicateAbs(ReplID, partialIndex) = ERROR_VALUE 'AG 07/06/2010
                                For Index As Integer = 0 To maxItemNumber - 1
                                    preparation(Index).ReplicateAbs(ReplID) = ERROR_VALUE
                                    preparation(Index).RepInUse(ReplID) = False
                                    preparation(Index).ErrorReplicateAbs(ReplID) = auxGlobal.ErrorCode
                                    preparation(Index).ReplicateDate(ReplID) = Now
                                Next

                                'AG 09/06/2010 - If absLimit delete error from myClassGlobalResult. If system error keep the error flag
                                'Exit Try
                                If auxGlobal.ErrorCode <> GlobalEnumerates.AbsorbanceErrors.SYSTEM_ERROR.ToString Then
                                    myClassGlobalResult.HasError = False
                                    myClassGlobalResult.ErrorCode = ""
                                End If
                                Exit Try
                                'AG 09/06/2010

                            Else
                                auxABS = CType(auxGlobal.SetDatos, Single)
                                preparation(itemID).PartialReplicateAbs(ReplID, partialIndex) = auxABS
                                preparation(itemID).RepInUse(ReplID) = True
                                preparation(itemID).ErrorReplicateAbs(ReplID) = ""

                            End If

                        Next    'For partialIndex As Integer = 0 To maxPartial - 1

                        'Calculate replicate abs depending on the partial absorbances
                        Select Case maxPartial
                            Case 2
                                'Monochromatic Bireagent differential or Fixed Time
                                'Bichromatic End Point (mono or bireagent)

                                Dim sign As Integer = 1
                                If test.ReactionType = "DEC" Then sign = -1 'Decreasing tests

                                'Bireagent Differential (monochromatic): ABS = sign* (PartialAbs(1) - KDIF*PartialAbs(0) 
                                'Fixed time (monochromatic) or End point (Bichromatic): ABS = sign* (PartialAbs(1) - PartialAbs(0) // KDIF = 1 //
                                Dim KDIF As Single = 1
                                If test.AnalysisMode = "BRDIF" Then
                                    With test
                                        'KDIF = (sample volume + reagent1 volume) / (sample volume + reagent1 volume + + reagent2 volume)
                                        KDIF = (.SampleVolume + .ReagentVolume(0)) / (.SampleVolume + .ReagentVolume(0) + +.ReagentVolume(1))
                                    End With

                                End If
                                preparation(itemID).ReplicateAbs(ReplID) = sign * (preparation(itemID).PartialReplicateAbs(ReplID, 1) - (KDIF * preparation(itemID).PartialReplicateAbs(ReplID, 0)))

                            Case 1
                                'End Point (Monochromatic)
                                preparation(itemID).ReplicateAbs(ReplID) = preparation(itemID).PartialReplicateAbs(ReplID, 0)


                            Case 4 'AG 09/02/2011
                                Dim sign As Integer = 1
                                If test.ReactionType = "DEC" Then sign = -1 'Decreasing tests

                                'Bireagent Differential (bichromatic): Abs = sign * [(Abs(T2) - Abs(T2-1)) - KDIF*(Abs(T1) - Abs(T1-1))]       NOTE: T2 & T1 uses the main filter 
                                'Fixed time (Bichromatic): ABS = sign* (PartialAbs(1) - PartialAbs(0) // KDIF = 1 //
                                Dim KDIF As Single = 1
                                If test.AnalysisMode = "BRDIF" Then
                                    With test
                                        'KDIF = (sample volume + reagent1 volume) / (sample volume + reagent1 volume + + reagent2 volume)
                                        KDIF = (.SampleVolume + .ReagentVolume(0)) / (.SampleVolume + .ReagentVolume(0) + +.ReagentVolume(1))
                                    End With
                                End If

                                preparation(itemID).ReplicateAbs(ReplID) = sign * ( _
                                                                                    (preparation(itemID).PartialReplicateAbs(ReplID, 3) - preparation(itemID).PartialReplicateAbs(ReplID, 2)) _
                                                                                        - KDIF * (preparation(itemID).PartialReplicateAbs(ReplID, 1) - preparation(itemID).PartialReplicateAbs(ReplID, 0) _
                                                                                    ) _
                                                                                )


                        End Select

                    Else    'Analysis mode kinetics
                        Me.CalculateKineticsAbsorbance(itemID)
                        If preparation(itemID).ReplicateAbs(ReplID) = ERROR_VALUE Then Exit Try
                    End If

                    preparation(itemID).ReplicateDate(ReplID) = Now

                Next    'For itemID As Integer = 0 To maxItemNumber - 1

            Catch ex As Exception
                Me.CatchLaunched("CalculateReplicateAbsorbance", ex)
                Me.MarkReplicateAbsSytemError()
            End Try
        End Sub


        ''' <summary>
        ''' Gets the number of partial absorbances depending analysis mode and reading type
        ''' </summary>
        ''' <param name="pAnalysisMode"></param>
        ''' <param name="pReadingType"></param>
        ''' <returns>Number of partial abs (integer)</returns>
        ''' <remarks>
        ''' Created by: AG 09/02/2010 (Tested OK)
        ''' AG 09/02/2011 - add fixed time bichromatic
        ''' </remarks>
        Private Function GetPartialAbsorbancesNumber(ByVal pAnalysisMode As String, ByVal pReadingType As String) As Integer
            Dim PartialAbs As Integer = 1
            Try
                If pReadingType = "BIC" Then 'Bichromatic
                    PartialAbs = 2

                    'AG 09/02/2011
                    Select Case pAnalysisMode
                        Case "MRFT", "BRFT", "BRDIF" 'Bichromatic (fixed time or bireagent differential)
                            PartialAbs = 4
                    End Select
                    'AG 09/02/2011

                Else 'Monochromatic
                    Select Case pAnalysisMode
                        Case "BRDIF"    'Bireagent differential
                            PartialAbs = 2

                        Case "MRFT" 'Monoreagent fixed time
                            PartialAbs = 2

                        Case "BRFT" 'Bireagent fixed time
                            PartialAbs = 2
                        Case Else
                            PartialAbs = 1
                    End Select
                End If

            Catch ex As Exception
                Me.CatchLaunched("GetPartialAbsorbancesNumber", ex)
                PartialAbs = 1
            End Try
            Return PartialAbs
        End Function


        ''' <summary>
        ''' Calculate the absorbance for kinetic tests
        ''' </summary>
        ''' <remarks>
        ''' Created by: AG 09/02/2010 (Tested OK)
        ''' Modify AG 23/10/2013 - define and initialize new table PauseTable that will be use to apply KineticsIncrease or KineticsIncreaseInPause
        ''' </remarks>
        Private Sub CalculateKineticsAbsorbance(ByVal ItemId As Integer)
            Try
                Dim numberPoints As Integer = test.FinalReadingCycle - test.InitialReadingCycle + 1
                Dim AbsErrorFound As Integer = 0 '0 ok, 1 some reading with error
                Dim AbsTable() As Single
                Dim PauseTable() As Boolean 'AG 23/10/2013 Task #1347 - This table will be use to apply KineticsIncrease or KineticsIncreaseInPause
                ReDim AbsTable(numberPoints - 1)
                ReDim PauseTable(numberPoints - 1) 'AG 23/10/2013 Task #1347

                Dim filterID As Integer = test.MainWaveLength - 1   'Kinetics are monochromatic
                Dim auxGlobal As New GlobalDataTO

                For indexRead As Integer = test.InitialReadingCycle - 1 To test.FinalReadingCycle - 1
                    Dim AbsDelta As Single = 0
                    'AG 18/02/2010 - Instead of water(itemID).WaterAbs(filterID) use 0 for water absorbance (Water absorbance is postposed (February 2010)
                    auxGlobal = Me.CalculateAbsorbance(preparation(ItemId).Readings(indexRead), preparation(ItemId).RefReadings(indexRead), common(ItemId).BaseLine(filterID), _
                                                       common(ItemId).RefBaseLine(filterID), common(ItemId).DarkCurrent(filterID), common(ItemId).RefDarkCurrent(filterID), 0, _
                                                       common(ItemId).PathLength, common(ItemId).LimitAbs, test.AbsorbanceFlag, test.DilutionFactor)

                    'Check if errors
                    If auxGlobal.HasError Then
                        AbsDelta = ERROR_VALUE
                        AbsErrorFound = 1
                        Exit For
                    Else
                        AbsDelta = CType(auxGlobal.SetDatos, Single)
                        AbsTable(indexRead - test.InitialReadingCycle + 1) = AbsDelta
                        PauseTable(indexRead - test.InitialReadingCycle + 1) = preparation(ItemId).PausedReadings(indexRead) 'AG 23/10/2013 Task #1347
                    End If

                Next    'For indexRead As Integer = test.InitialReadingCycle - 1 To test.FinalReadingCycle - 1

                'Check for errors
                If AbsErrorFound > 0 Then
                    'Current replicate
                    Dim ReplID As Integer = preparation(ItemId).ReplicateID - 1

                    'Calculate the max item number
                    Dim maxItemNumber As Integer = 1
                    If preparation(ItemId).SampleClass = "CALIB" Then maxItemNumber = calibrator.NumberOfCalibrators

                    For Index As Integer = 0 To maxItemNumber - 1
                        preparation(Index).ReplicateAbs(ReplID) = ERROR_VALUE
                        preparation(Index).RepInUse(ReplID) = False
                        preparation(Index).ErrorReplicateAbs(ReplID) = auxGlobal.ErrorCode
                        preparation(Index).ReplicateDate(ReplID) = Now
                    Next

                    'AG 09/06/2010 - If absLimit delete error from myClassGlobalResult. If system error keep the error flag
                    'Exit Try
                    If auxGlobal.ErrorCode <> GlobalEnumerates.AbsorbanceErrors.SYSTEM_ERROR.ToString Then
                        myClassGlobalResult.HasError = False
                        myClassGlobalResult.ErrorCode = ""
                    End If
                    'AG 09/06/2010

                    'No errors found
                Else
                    Me.CalculateKineticSlope(ItemId, AbsTable, PauseTable) 'AG 23/10/2013 Task #1347 (new parameter PauseTable added to method)
                    If myClassGlobalResult.HasError Then Exit Try

                    Me.CalculateReplicateSubstrateDepletion(ItemId, AbsTable)
                    If myClassGlobalResult.HasError Then Exit Try 'AG 11/03/2010

                End If

            Catch ex As Exception
                Me.CatchLaunched("CalculateKineticsAbsorbance", ex)
                Me.MarkReplicateAbsSytemError()
            End Try
        End Sub


        ''' <summary>
        ''' Calculates the kinetics slope with a linear regression
        ''' y = a0 + a1 x (y= Absorbance, x= time [minutes], Slope= Abs/minute)
        ''' 
        ''' Also calculates the correlation linear coefficient and chi in order to evaluate the linearity
        ''' 
        ''' </summary>
        ''' <param name="ItemID"></param>
        ''' <param name="AbsTable"></param>
        ''' <param name="pauseTable"></param>
        ''' <remarks>
        ''' Created by: AG 09/02/2010 (Tested OK)
        ''' Modify AG 23/10/2013 - new parameter PauseTable that will be use to apply KineticsIncrease or KineticsIncreaseInPause
        ''' Modify AG 06/11/2013 - task #1377 (fix issue calculate kinetics slope with pause mode)
        ''' </remarks>
        Private Sub CalculateKineticSlope(ByVal ItemID As Integer, ByVal AbsTable() As Single, ByVal pauseTable() As Boolean)
            Try

                Dim pointsNumber As Integer = test.FinalReadingCycle - test.InitialReadingCycle + 1
                Dim X As Double = 0
                Dim SumX As Double = 0
                Dim SumY As Double = 0
                Dim SumX2 As Double = 0
                Dim SumY2 As Double = 0
                Dim SumXY As Double = 0
                Dim a0 As Double = 0
                Dim a1 As Double = 0

                For cycle As Integer = 0 To pointsNumber - 1
                    '// X has the reading cycle in minutes
                    '// 1st read in 0 seconds, next readings every kinetics increase (minutes)
                    ''X = (test.InitialReadingCycle + cycle - 1) / (common(ItemID).KineticsIncrease / CDbl(60))

                    'AG 06/11/2013 - Task #1377 (with STortosa support) - Time variable (X) must be accumulative
                    'Until now there was an unique timecycle interval so the  TotalTime = NumberCycles * TimeCycle
                    'But new there was 2 timecycle intervals so TotalTime <> NumberCycles * TimeCycle (instead of this it can be calculated accumuative)
                    'The offset (test.InitialReadingCycle + cycle - 1) is not important because we need the slope

                    'AG 23/10/2013 Task #1347. Apply the proper time interval in normal or pause mode
                    ''X = (test.InitialReadingCycle + cycle - 1) * (common(ItemID).KineticsIncrease) / CDbl(60)
                    'If pauseTable(cycle) = False Then 'Normal running mode
                    '    X = (test.InitialReadingCycle + cycle - 1) * (common(ItemID).KineticsIncrease) / CDbl(60)
                    'Else 'Paused running mode
                    '    X = (test.InitialReadingCycle + cycle - 1) * (common(ItemID).KineticsIncreaseInPause) / CDbl(60)
                    'End If
                    If pauseTable(cycle) = False Then 'Normal running mode
                        X += (common(ItemID).KineticsIncrease) / CDbl(60)
                    Else 'Paused running mode
                        X += (common(ItemID).KineticsIncreaseInPause) / CDbl(60)
                    End If
                    'AG 06/11/2013

                    SumX += X
                    SumY += AbsTable(cycle)
                    SumX2 += X ^ 2  'Math.Pow(X, 2)
                    SumY2 += AbsTable(cycle) ^ 2
                    SumXY += X * AbsTable(cycle)
                Next

                'AG 26/04/2012
                'a0 = (SumY * SumX2 - SumX * SumXY) / (pointsNumber * SumX2 - SumX ^ 2)  'Origin of kinetics curve
                'a1 = (pointsNumber * SumXY - SumX * SumY) / (pointsNumber * SumX2 - SumX ^ 2)   'Calculate slope
                Dim divisor As Double = (pointsNumber * SumX2 - SumX ^ 2)
                If divisor <> 0 Then
                    a0 = (SumY * SumX2 - SumX * SumXY) / (pointsNumber * SumX2 - SumX ^ 2)  'Origin of kinetics curve
                    a1 = (pointsNumber * SumXY - SumX * SumY) / (pointsNumber * SumX2 - SumX ^ 2)   'Calculate slope
                End If
                'AG 26/04/2012

                Dim replID As Integer = preparation(ItemID).ReplicateID - 1

                'Partial absorbance is slope (without sign)
                preparation(ItemID).PartialReplicateAbs(replID, 0) = CSng(a1)
                If test.ReactionType = "DEC" Then preparation(ItemID).PartialReplicateAbs(replID, 0) *= -1

                'Kinetics are monochromatics (ReplicateAbs = PartialAbs
                preparation(ItemID).ReplicateAbs(replID) = preparation(ItemID).PartialReplicateAbs(replID, 0)
                preparation(ItemID).RepInUse(replID) = True
                preparation(ItemID).KineticsCurve(replID, 0) = AbsTable(0)
                preparation(ItemID).KineticsCurve(replID, 1) = CSng(a1)
                'AG 08/02/2012 - the next line is commented because in Ax5 code no sign is applied
                'If test.ReactionType = "DEC" Then preparation(ItemID).KineticsCurve(replID, 1) *= -1

                'Calculate correlation linear factor
                Dim aux As Double = 0
                aux = ((pointsNumber * SumX2 - SumX ^ 2) * (pointsNumber * SumY2 - SumY ^ 2))
                If aux > 0 Then
                    Dim aux2 As Double = 0
                    aux2 = (pointsNumber * SumXY - SumX * SumY)
                    preparation(ItemID).rKineticsReplicate(replID) = CSng(aux2 / Math.Sqrt(aux))

                    preparation(ItemID).KineticsLinear = True
                    If preparation(ItemID).SampleClass <> "BLANK" Then
                        If Math.Abs(preparation(ItemID).rKineticsReplicate(replID)) < common(ItemID).LinearCorrelationFactor Then
                            preparation(ItemID).KineticsLinear = False
                        ElseIf Math.Abs(preparation(ItemID).rKineticsReplicate(replID)) > 1 Then
                            preparation(ItemID).KineticsLinear = False
                        End If
                    End If

                Else
                    Dim maxItemNumber As Integer = 1
                    If preparation(ItemID).SampleClass = "CALIB" Then maxItemNumber = calibrator.NumberOfCalibrators

                    For Index As Integer = 0 To maxItemNumber - 1
                        preparation(Index).ReplicateAbs(replID) = common(Index).LimitAbs
                        preparation(Index).RepInUse(replID) = False
                        preparation(Index).ErrorReplicateAbs(replID) = GlobalEnumerates.AbsorbanceErrors.ABS_LIMIT.ToString
                    Next

                    If preparation(ItemID).SampleClass <> "BLANK" Then preparation(ItemID).KineticsLinear = False

                End If

            Catch ex As Exception
                Me.CatchLaunched("CalculateKineticSlope", ex)
                Me.MarkReplicateAbsSytemError()
            End Try
        End Sub


        ''' <summary>
        ''' Implements the substrate depletion algorithm
        ''' </summary>
        ''' <param name="AbsTable"></param>
        ''' <remarks>
        ''' Created by: AG 09/02/2010 (Tested OK)
        ''' </remarks>
        Private Sub CalculateReplicateSubstrateDepletion(ByVal ItemID As Integer, ByVal AbsTable() As Single)
            Try
                Dim substrateDepletion As Integer = 0   '0 No, 1 Yes

                'Initial values
                preparation(ItemID).SubstrateDepletionReplicate(preparation(ItemID).ReplicateID - 1) = False
                preparation(ItemID).SubstrateDepletion = 0

                'Substrate depletion algothim only applies for patients
                If test.SubstrateDepletion.InUse = True And preparation(ItemID).SampleClass = "PATIENT" Then
                    Dim sign As Integer = 1
                    If test.ReactionType = "DEC" Then sign = -1

                    For i As Integer = 0 To UBound(AbsTable)
                        'AG 08/02/2012 - apply the sign in both sides, else the formula was wrong for decreasing tests 
                        'If AbsTable(i) > sign * test.SubstrateDepletion.Value Then substrateDepletion = 1
                        If sign * AbsTable(i) > sign * test.SubstrateDepletion.Value Then substrateDepletion = 1

                        If substrateDepletion = 1 Then Exit For
                    Next

                    Dim replID As Integer = preparation(ItemID).ReplicateID - 1
                    If substrateDepletion = 1 Then
                        preparation(ItemID).SubstrateDepletionReplicate(replID) = True
                        preparation(ItemID).ErrorReplicateAbs(replID) = GlobalEnumerates.AbsorbanceErrors.SUBSTRATE_DEPLETION.ToString  'AG 11/03/2010

                    Else
                        preparation(ItemID).SubstrateDepletionReplicate(replID) = False
                    End If

                    'Update the sample average substrate depletion value
                    ' 0- No replicates with substrate depletion, 1- some but not all replicates with SD, 2- all replicates with SD

                    Dim replWithSD As Integer = 0
                    For i As Integer = 0 To replID 'replID = current replicate
                        If preparation(ItemID).SubstrateDepletionReplicate(i) = True Then replWithSD += 1
                    Next

                    If replWithSD = 0 Then
                        preparation(ItemID).SubstrateDepletion = 0  'No replicates with SD
                    ElseIf replWithSD < replID Then
                        preparation(ItemID).SubstrateDepletion = 1  'Some but not all replicates with SD
                    Else
                        preparation(ItemID).SubstrateDepletion = 2  'All replicates with SD
                        preparation(ItemID).ErrorAbs = GlobalEnumerates.AbsorbanceErrors.SUBSTRATE_DEPLETION.ToString  'AG 11/03/2010
                    End If


                End If 'If test.SubstrateDepletion.InUse = True And preparation(ItemID).SampleClass = "PATIENT" Then

            Catch ex As Exception
                Me.CatchLaunched("CalculateReplicateSubstrateDepletion", ex)
            End Try
        End Sub


        ''' <summary>
        ''' NOT USED!!!! Finally BA400 can not perform the blank reagent (readings starts after sample is dispensed)
        ''' Calculates the absorbance for the reagent1 cycles of then preparation
        ''' </summary>
        ''' <remarks>
        ''' Created by: AG 10/02/2010 (Tested OK)
        ''' </remarks>
        Private Sub CalculateReagent1Absorbance()
            Try
                'Dim myLocal As New GlobalDataTO
                Dim filterID As Integer = test.MainWaveLength - 1
                Dim maxItemNumber As Integer = 1
                If preparation(0).SampleClass = "CALIB" Then maxItemNumber = calibrator.NumberOfCalibrators

                For itemID As Integer = 0 To maxItemNumber - 1
                    Dim replID As Integer = preparation(itemID).ReplicateID - 1

                    For r1Read As Integer = 0 To common(itemID).Reagent1Readings - 1
                        Dim auxABS As Single = 0
                        Dim auxGlobal As New GlobalDataTO

                        'AG 18/02/2010 - Instead of water(itemID).WaterAbs(filterID) use 0 for water absorbance (Water absorbance is postposed (February 2010)
                        'Reagent1 dont use the dilution factor (the test Absorbanceflag is False)
                        auxGlobal = CalculateAbsorbance(preparation(itemID).Readings(r1Read), preparation(itemID).RefReadings(r1Read), _
                                                        common(itemID).BaseLine(filterID), common(itemID).RefBaseLine(filterID), _
                                                        common(itemID).DarkCurrent(filterID), common(itemID).RefDarkCurrent(filterID), 0, _
                                                        common(itemID).PathLength, common(itemID).LimitAbs, False)

                        'Check if errors
                        If auxGlobal.HasError Then
                            preparation(itemID).Reagent1ReplicateAbs(r1Read) = ERROR_VALUE
                            preparation(itemID).RepInUse(replID) = False
                            preparation(itemID).ErrorReplicateAbs(replID) = auxGlobal.ErrorCode

                            If auxGlobal.ErrorCode <> GlobalEnumerates.AbsorbanceErrors.SYSTEM_ERROR.ToString Then
                                myClassGlobalResult.HasError = False
                            End If

                            Exit Try
                        Else
                            auxABS = CType(auxGlobal.SetDatos, Single)
                            preparation(itemID).Reagent1ReplicateAbs(r1Read) = auxABS
                            preparation(itemID).RepInUse(replID) = True
                            preparation(itemID).ErrorReplicateAbs(replID) = ""
                        End If

                    Next
                Next

            Catch ex As Exception
                Me.CatchLaunched("CalculateReagent1Absorbance", ex)

                For i As Integer = 0 To UBound(preparation)
                    Dim repl As Integer = preparation(i).ReplicateID - 1
                    For j As Integer = 0 To common(i).Reagent1Readings - 1
                        preparation(i).Reagent1ReplicateAbs(j) = ERROR_VALUE
                        preparation(i).ErrorReplicateAbs(repl) = GlobalEnumerates.AbsorbanceErrors.SYSTEM_ERROR.ToString
                    Next
                Next

            End Try
        End Sub

        ''' <summary>
        ''' Calculate the average absorbance (with replicates from 1 to current)
        ''' </summary>
        ''' <remarks>
        ''' Created AG 10/02/2010 (Tested OK)
        ''' Modified by AG 15/09/2010: add "And And preparation(itemID).ErrorAbs = "" "
        ''' </remarks>
        Private Sub CalculateAbsAverage()
            Try
                Dim itemMaxNumber As Integer = 1
                If preparation(0).SampleClass = "CALIB" Then itemMaxNumber = calibrator.NumberOfCalibrators

                'For all multiitemID
                For itemID As Integer = 0 To itemMaxNumber - 1
                    Dim inUse As Integer = 0
                    Dim Average As Single = 0
                    Dim currentRepl As Integer = preparation(itemID).ReplicateID

                    For i As Integer = 0 To currentRepl - 1
                        'AG 19/03/2012 - in condition change ErrorAbs by ErrorReplicateAbs ('AG 15/09/2010)
                        If preparation(itemID).RepInUse(i) = True And preparation(itemID).SubstrateDepletionReplicate(i) = False _
                        And preparation(itemID).ErrorReplicateAbs(i) = "" Then
                            Average += preparation(itemID).ReplicateAbs(i)
                            inUse += 1
                        End If
                    Next

                    If inUse = 0 Then
                        preparation(itemID).Abs = ERROR_VALUE
                        preparation(itemID).ErrorAbs = GlobalEnumerates.AbsorbanceErrors.NO_INUSE_ELEMENTS.ToString

                        'AG 09/06/2010
                        'myClassGlobalResult.HasError = True
                        'myClassGlobalResult.ErrorCode = GlobalEnumerates.AbsorbanceErrors.NO_INUSE_ELEMENTS.ToString
                    Else
                        Average = Average / CSng(inUse)
                        preparation(itemID).Abs = Average
                        preparation(itemID).ErrorAbs = ""

                        'AG 11/03/2010 - Average date is the last replicate date
                        ''preparation(itemID).AverageDate = Now
                        'preparation(itemID).AverageDate = preparation(itemID).ReplicateDate(currentRepl - 1)

                    End If

                    'AG 11/03/2010 - Average date is the last replicate date
                    preparation(itemID).AverageDate = preparation(itemID).ReplicateDate(currentRepl - 1)

                Next

            Catch ex As Exception
                Me.CatchLaunched("CalculateAbsAverage", ex)
                Me.MarkAverageAbsSystemError()
            End Try
        End Sub


        ''' <summary>
        ''' Check if blank absorbance has error and calculates the initial absorbance 
        ''' if analysis mode is kinetics or fixed time
        ''' 
        ''' Also the main filter absorbance when analysis mode is bichromatic end point
        ''' </summary>
        ''' <remarks>
        ''' Created AG 10/02/2010 (Tested OK)
        ''' </remarks>
        Private Sub CheckBlankResults()
            Try

                test.BlankAbs = preparation(0).Abs
                test.BlankDate = preparation(0).AverageDate
                test.ErrorBlankAbs = preparation(0).ErrorAbs

                'If no error calculate the:
                ' - initial absorbance (only in kinetics or fixed time tests)
                ' - main filter absorbance (only in bichromatic end point tests)
                If Trim$(test.ErrorBlankAbs) = "" Then
                    Dim replID As Integer = preparation(0).ReplicateID - 1

                    preparation(0).InitialReplicateAbs(replID) = 0
                    preparation(0).MainFilterReplicateAbs(replID) = 0
                    preparation(0).WorkingReagentReplicateAbs(replID) = 0 'AG 12/07/2010
                    preparation(0).InitialAbs = 0
                    preparation(0).MainFilterAbs = 0
                    preparation(0).WorkingReagentAbs = 0    'AG 12/07/2010
                    test.InitialAbs = 0
                    test.MainFilterAbs = 0
                    test.WorkingReagentAbs = 0 'AG 12/07/2010

                    Select Case test.AnalysisMode
                        Case "MRFT", "BRFT" 'Fixed time
                            If String.Equals(test.ReadingType, "MONO") Then
                                preparation(0).InitialReplicateAbs(replID) = preparation(0).PartialReplicateAbs(replID, 0)
                            Else
                                preparation(0).InitialReplicateAbs(replID) = preparation(0).PartialReplicateAbs(replID, 1) - preparation(0).PartialReplicateAbs(replID, 0)
                            End If
                            preparation(0).InitialAbs = Me.CalculateAverage(preparation(0).InitialReplicateAbs, preparation(0).RepInUse, preparation(0).ReplicateID)
                            test.InitialAbs = preparation(0).InitialAbs


                        Case "MRK", "BRK"   'kinetics
                            preparation(0).InitialReplicateAbs(replID) = preparation(0).KineticsCurve(replID, 0)
                            preparation(0).InitialAbs = Me.CalculateAverage(preparation(0).InitialReplicateAbs, preparation(0).RepInUse, preparation(0).ReplicateID)
                            test.InitialAbs = preparation(0).InitialAbs


                        Case "MREP", "BREP" 'End Point
                            If String.Equals(test.ReadingType, "BIC") Then    'Only bicromatic
                                preparation(0).MainFilterReplicateAbs(replID) = preparation(0).PartialReplicateAbs(replID, 1)
                                preparation(0).MainFilterAbs = Me.CalculateAverage(preparation(0).MainFilterReplicateAbs, preparation(0).RepInUse, preparation(0).ReplicateID)
                                test.MainFilterAbs = preparation(0).MainFilterAbs
                            End If

                            'AG 12/07/2010
                        Case "BRDIF"    'Bireagent differential
                            If String.Equals(test.ReadingType, "MONO") Then
                                preparation(0).WorkingReagentReplicateAbs(replID) = preparation(0).PartialReplicateAbs(replID, 1)
                            Else
                                preparation(0).WorkingReagentReplicateAbs(replID) = preparation(0).PartialReplicateAbs(replID, 3) - preparation(0).PartialReplicateAbs(replID, 2)
                            End If
                            preparation(0).WorkingReagentAbs = Me.CalculateAverage(preparation(0).WorkingReagentReplicateAbs, preparation(0).RepInUse, preparation(0).ReplicateID)
                            test.WorkingReagentAbs = preparation(0).WorkingReagentAbs
                            'END AG 12/07/2010

                    End Select

                End If

            Catch ex As Exception
                Me.CatchLaunched("CheckBlankResults", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Recalibrate old calibrator with newer blanks
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' Created by: AG 10/02/2010 (Tested OK)
        ''' </remarks>
        Private Sub ApplyBlankDifference()
            Try
                'If exists calibration
                myBlankDifference = 0
                If calibrator.CalibratorDate > DateTime.MinValue Then
                    'Recalibrate only when newer blank result exits
                    'AG 16/11/2012 - add new condition (blank difference will apply only when exists a valid blank abs used)
                    'Activate the 2on if when change validated
                    'If calibrator.BlankAbsUsed <> test.BlankAbs And calibrator.CalibratorDate < test.BlankDate Then
                    If calibrator.BlankAbsUsed <> 0 AndAlso calibrator.BlankAbsUsed <> test.BlankAbs AndAlso calibrator.CalibratorDate < test.BlankDate Then
                        myBlankDifference = test.BlankAbs - calibrator.BlankAbsUsed
                    End If
                End If

            Catch ex As Exception
                Me.CatchLaunched("ApplyBlankDifference", ex)
            End Try
        End Sub



        ''' <summary>
        ''' Calculates a new calibration factor or curve
        ''' </summary>
        ''' <remarks>
        ''' Created by: AG 10/02/2010 (Tested OK)
        ''' Modified by AG 02/09/2010 - In calibration curves the absorbance follow the next rule
        '''                             (prep(0).Abs > prep(1).Abs > prep(2).Abs > ... > prep(n).Abs), in original code was reverse
        '''                             This change applies to Abs - ErrorAbs - %Error
        ''' Modified by AG 01/02/2011 - Return to code before 02/09/2011 due the calibration curve has changed the way it save values
        ''' 
        ''' !!!!!!!!!!!!!!!!!!
        ''' IMPORTANT NOTE
        ''' Code 01/02/2011 implements the new calibration curve programming (MultiItem1 is the lower concentrate)
        '''                 code 02/09/2011 - has to be commented and return to the original one
        ''' 
        ''' Code 02/09/2010 implies this: preparation and calibration structures arrays are sorted in reverse way!!!
        ''' Executions preparation structure: preparation(0).Abs higher than preparation(1).Abs ... higher than preparation(N).Abs
        '''                                   preparation(0).Conc higher than preparation(1).Conc ... higher than preparation(N).Conc
        ''' 
        ''' Calibrator structure: calibrator.Abs(0) lower than calibrator.Abs(1) lower than ... calibrator.Abs(N)
        '''                       calibrator.Conc(0) lower than calibrator.Conc(1) lower than ... calibrator.Conc(N)
        ''' !!!!!!!!!!!!!!!!!!
        ''' 
        ''' </remarks>
        Private Sub CalculateCalibration()
            Try
                Dim itemNumber As Integer = calibrator.NumberOfCalibrators
                Dim curveHasAbsError As Boolean = False

                calibrator.ErrorCalibration = "" 'AG 26/07/2010 - Initialize flag

                If calibrator.CalibrationType = "EXPERIMENT" Then
                    'Save current results
                    'AG 16/11/2012 - calibrator.BlankAbsUsed is the blank abs that will be used for calculate calibration when no blank difference applies
                    'Activate commented code when change validated
                    'calibrator.BlankAbsUsed = test.BlankAbs
                    If myBlankDifference = 0 Then
                        calibrator.BlankAbsUsed = test.BlankAbs
                    End If


                    If itemNumber = 1 Then
                        calibrator.CalibratorAbs = preparation(0).Abs
                        calibrator.ErrorAbs = preparation(0).ErrorAbs

                    ElseIf itemNumber > 1 Then
                        For i As Integer = 0 To itemNumber - 1
                            'AG 01/02/2011
                            calibrator.Curve.CalibratorAbs(i) = preparation(i).Abs
                            calibrator.Curve.ErrorAbs(i) = preparation(i).ErrorAbs
                            'Code (AG 02/09/2010)Commented 01/02/2011
                            'calibrator.Curve.CalibratorAbs(i) = preparation(itemNumber - 1 - i).Abs
                            'calibrator.Curve.ErrorAbs(i) = preparation(itemNumber - 1 - i).ErrorAbs
                            'END AG 01/02/2011

                            If Trim$(calibrator.Curve.ErrorAbs(i)) <> "" Then
                                curveHasAbsError = True
                                calibrator.ErrorAbs = calibrator.Curve.ErrorAbs(i)
                                Exit For
                            End If

                        Next
                    End If     'If itemNumber = 1 Then


                    'Calculate new calibrator factor is not error neither blank neither current sample
                    If Trim$(test.ErrorBlankAbs) <> "" Or Trim$(calibrator.ErrorAbs) <> "" Then
                        calibrator.Factor = ERROR_VALUE
                        calibrator.ErrorCalibration = GlobalEnumerates.CalibrationFactorErrors.ABS_ERROR.ToString

                        'Activate commented code when change validated
                        calibrator.BlankAbsUsed = 0 'AG 16/11/2012 - if calibration error then remove the blank used absorbance
                    Else
                        If itemNumber = 1 Then
                            'If NO error then calculate calibration factor = TheoreticalConc /(AbsCal - AbsBlank + BlankDifference)
                            'If predilution then: Factor = Factor/PredilutionFactor
                            Dim aux As Single = 0
                            aux = calibrator.CalibratorAbs - test.BlankAbs + myBlankDifference
                            If aux > 0 Then
                                calibrator.Factor = calibrator.TheoreticalConcentration(0) / aux

                                'AG 11/03/2010 - Dont apply!! In Ax5 no predilution in calibrator executions
                                'If test.PreDilutionFlag And test.PredilutionFactor > 0 Then
                                'calibrator.Factor = calibrator.Factor / test.PredilutionFactor
                                'End If

                                calibrator.CalibratorDate = preparation(0).AverageDate
                                calibrator.ErrorCalibration = ""

                            Else    'Blank > Calibrator
                                calibrator.Factor = ERROR_VALUE
                                calibrator.ErrorCalibration = GlobalEnumerates.CalibrationFactorErrors.BLANK_HIGHER_CALIB.ToString

                                'Activate commented code when change validated
                                calibrator.BlankAbsUsed = 0 'AG 16/11/2012 - if calibration error then remove the blank used absorbance
                            End If 'If aux > 0 Then


                        ElseIf itemNumber > 1 Then
                            Me.CalculateCalibrationCurve()
                            If myClassGlobalResult.HasError Then Exit Try 'Errors validation
                            calibrator.CalibratorDate = preparation(UBound(preparation)).AverageDate

                            'If curve if properly generated then calculate the relative error
                            If Trim$(calibrator.ErrorCalibration) = "" Then

                                For ItemId As Integer = 0 To calibrator.NumberOfCalibrators - 1
                                    For idRepl As Integer = 0 To preparation(ItemId).ReplicateID - 1
                                        Dim errorType As String = ""

                                        'Calculate the concentration for the calibrator point with the curve for each calibrator replicate
                                        'AG 01/02/2011
                                        preparation(ItemId).ReplicateConc(idRepl) = Me.CalculateConcetrationWithCurve(preparation(ItemId).ReplicateAbs(idRepl), calibrator.Curve.CurveGrowthType, errorType)
                                        'Code (AG 02/09/2010)Commented 01/02/2011
                                        'preparation(itemNumber - 1 - ItemId).ReplicateConc(idRepl) = Me.CalculateConcetrationWithCurve(preparation(itemNumber - 1 - ItemId).ReplicateAbs(idRepl), test.ReactionType, errorType)
                                        'END AG 01/02/2011

                                        If myClassGlobalResult.HasError Then Exit Try 'Errors validation

                                        'Calculate the error percentage for each calibrator replicate
                                        If Trim$(errorType) = "" Then
                                            'AG 01/02/2011
                                            preparation(ItemId).ErrorReplicateConc(idRepl) = "" 'AG 11/08/2010
                                            preparation(ItemId).ErrorCurveReplicate(idRepl) = Me.CalculateCurveCalibrationError(preparation(ItemId).ReplicateConc(idRepl), calibrator.TheoreticalConcentration(ItemId))
                                            'Code (AG 02/09/2010)Commented 01/02/2011
                                            'preparation(itemNumber - 1 - ItemId).ErrorReplicateConc(idRepl) = "" 'AG 11/08/2010
                                            'preparation(itemNumber - 1 - ItemId).ErrorCurveReplicate(idRepl) = Me.CalculateCurveCalibrationError(preparation(itemNumber - 1 - ItemId).ReplicateConc(idRepl), calibrator.TheoreticalConcentration(ItemId))
                                            'END AG 01/02/2011

                                            If myClassGlobalResult.HasError Then Exit Try 'Errors validation
                                        Else
                                            'AG 01/02/2011
                                            preparation(ItemId).ErrorReplicateConc(idRepl) = errorType
                                            'Code (AG 02/09/2010)Commented 01/02/2011
                                            'preparation(itemNumber - 1 - ItemId).ErrorReplicateConc(idRepl) = errorType
                                            'END AG 01/02/2011
                                        End If
                                    Next

                                    'Calculate the concentration for the calibrator point with the curve for each calibrator point (average)
                                    Dim avgError As String = ""
                                    'AG 01/02/2011
                                    preparation(ItemId).Conc = Me.CalculateConcetrationWithCurve(preparation(ItemId).Abs, calibrator.Curve.CurveGrowthType, avgError)
                                    'Code (AG 02/09/2010)Commented 01/02/2011
                                    'preparation(itemNumber - 1 - ItemId).Conc = Me.CalculateConcetrationWithCurve(preparation(itemNumber - 1 - ItemId).Abs, test.ReactionType, avgError)
                                    'END AG 01/02/2011

                                    If myClassGlobalResult.HasError Then Exit Try 'Errors validation

                                    'AG 01/02/2011
                                    calibrator.Curve.ConcCurve(ItemId) = preparation(ItemId).Conc
                                    'Code (AG 02/09/2010)Commented 01/02/2011
                                    'calibrator.Curve.ConcCurve(ItemId) = preparation(itemNumber - 1 - ItemId).Conc
                                    'END AG 01/02/2011

                                    'Calculate the error percentage for each calibrator point (average)
                                    If Trim$(avgError) = "" Then

                                        'AG 01/02/2011
                                        preparation(ItemId).ErrorConc = "" 'AG 11/08/2010
                                        calibrator.Curve.ErrorCurve(ItemId) = Me.CalculateCurveCalibrationError(preparation(ItemId).Conc, calibrator.TheoreticalConcentration(ItemId))
                                        'Code (AG 02/09/2010)Commented 01/02/2011
                                        'preparation(itemNumber - 1 - ItemId).ErrorConc = "" 'AG 11/08/2010
                                        'calibrator.Curve.ErrorCurve(ItemId) = Me.CalculateCurveCalibrationError(preparation(itemNumber - 1 - ItemId).Conc, calibrator.TheoreticalConcentration(ItemId))
                                        'END AG 01/02/2011

                                        If myClassGlobalResult.HasError Then Exit Try 'Errors validation
                                    Else
                                        'AG 06/11/2012 - The active code indexes are wrong. See in the replicates Loop
                                        ''AG 01/02/2011
                                        ''preparation(ItemId).ErrorConc = avgError
                                        'preparation(itemNumber - 1 - ItemId).ErrorConc = avgError
                                        ''END AG 01/02/2011
                                        preparation(ItemId).ErrorConc = avgError
                                        'AG 06/11/2012

                                    End If
                                Next

                            Else    'Curve calibration calculation NOT calculated
                                'AG 01/02/2011
                                For ItemId As Integer = 0 To calibrator.NumberOfCalibrators - 1
                                    'Code (AG 02/09/2010)Commented 01/02/2011
                                    'For ItemId As Integer = calibrator.NumberOfCalibrators - 1 To 0 Step -1
                                    'END AG 01/02/2011

                                    For idRepl As Integer = 0 To preparation(ItemId).ReplicateID - 1
                                        preparation(ItemId).ReplicateConc(idRepl) = ERROR_VALUE
                                        preparation(ItemId).ErrorCurveReplicate(idRepl) = 0
                                        preparation(ItemId).ErrorReplicateConc(idRepl) = Trim$(calibrator.ErrorCalibration)
                                    Next

                                    preparation(ItemId).Conc = ERROR_VALUE
                                    calibrator.Curve.ErrorCurve(ItemId) = ERROR_VALUE 'AG 12/03/2010
                                    calibrator.Curve.ConcCurve(ItemId) = ERROR_VALUE 'AG 12/03/2010
                                    preparation(ItemId).ErrorConc = Trim$(calibrator.ErrorCalibration)
                                Next

                                'Activate commented code when change validated
                                calibrator.BlankAbsUsed = 0 'AG 16/11/2012 - if calibration error then remove the blank used absorbance

                            End If

                        End If 'If itemNumber = 1 Then
                    End If 'If Trim$(test.ErrorBlankAbs) <> "" Or Trim$(calibrator.ErrorAbs) <> "" Then

                End If    'If calibrator.CalibrationType = "EXPERIMENT" Then

            Catch ex As Exception
                Me.CatchLaunched("CalculateCalibration", ex)
                calibrator.Factor = ERROR_VALUE
                calibrator.ErrorCalibration = GlobalEnumerates.CalibrationFactorErrors.SYSTEM_ERROR.ToString

            End Try
        End Sub


        ''' <summary>
        ''' Calculates a new concentration with sample and current blank absorbance and the calibration (factor or curve) value 
        ''' </summary>
        ''' <remarks>
        ''' Created by: AG 11/02/2010 (Tested OK)
        ''' AG 11/11/2010 - Change ERRORVALUE conditions depending the ManualFactorFlag value
        ''' </remarks>
        Private Sub CalculateReplicateConcentration()
            Try
                'Calculate only concentration if test isnt an absorbance test
                If test.AbsorbanceFlag = False Then

                    Dim recalculateAvgAbsFlag As Boolean = False 'AG 07/11/2012

                    For item As Integer = 0 To UBound(preparation)
                        Dim replID As Integer = preparation(item).ReplicateID - 1

                        preparation(item).ErrorReplicateConc(replID) = ""   'AG 26/07/2010 - Initialize flag

                        'Dont calculate concentration is some error in (sample, factor, calibrator abs or blank abs)
                        'Dont calculate concentration is sample with substrate depletion
                        'AG 11/11/2010
                        'If Trim$(preparation(item).ErrorReplicateAbs(replID)) <> "" Or Trim$(test.ErrorBlankAbs) <> "" Or Trim$(calibrator.ErrorAbs) <> "" Or Trim$(calibrator.ErrorCalibration) <> "" Then
                        Dim existError As Boolean = False

                        If Trim$(preparation(item).ErrorReplicateAbs(replID)) <> "" Or Trim$(test.ErrorBlankAbs) <> "" Then existError = True

                        If Not existError And Not calibrator.ManualFactorFlag Then
                            If Trim$(calibrator.ErrorAbs) <> "" Or Trim$(calibrator.ErrorCalibration) <> "" Then existError = True
                        End If

                        If existError Then
                            'END AG 11/11/2010

                            preparation(item).ReplicateConc(replID) = ERROR_VALUE

                            'END AG 11/03/2010
                            'preparation(item).ErrorReplicateConc(replID) = GlobalEnumerates.ConcentrationErrors.OUT.ToString
                            If preparation(item).ErrorReplicateAbs(replID) = GlobalEnumerates.AbsorbanceErrors.SUBSTRATE_DEPLETION.ToString Then
                                preparation(item).ErrorReplicateConc(replID) = GlobalEnumerates.ConcentrationErrors.SUBSTRATE_DEPLETION.ToString
                            Else
                                preparation(item).ErrorReplicateConc(replID) = GlobalEnumerates.ConcentrationErrors.OUT.ToString
                            End If
                            'END AG 11/03/2010

                        Else

                            Dim aux As Single = 0
                            If calibrator.CalibrationType = "FACTOR" Or calibrator.NumberOfCalibrators = 1 Then  'Factor (fixed or experimental)
                                'AG 10/11/2010 - adapt calculations module to use Manual Factor
                                'aux = CSng(calibrator.Factor * (preparation(item).ReplicateAbs(replID) - test.BlankAbs))
                                If Not calibrator.ManualFactorFlag Then
                                    aux = CSng(calibrator.Factor * (preparation(item).ReplicateAbs(replID) - test.BlankAbs))
                                Else
                                    aux = CSng(calibrator.ManualFactor * (preparation(item).ReplicateAbs(replID) - test.BlankAbs))
                                End If
                                'END AG 10/11/2010

                            Else    'Curve

                                Dim errorRepl As String = ""

                                'Calculate concentration with curve
                                aux = Me.CalculateConcetrationWithCurve(preparation(item).ReplicateAbs(replID), calibrator.Curve.CurveGrowthType, errorRepl)
                                If myClassGlobalResult.HasError Then Exit Try
                                preparation(item).ErrorReplicateConc(replID) = errorRepl

                            End If 'If calibrator.NumberOfCalibrators = 1 Then 

                            'If no error during concentration calculation (only in curves) apply several additional corrections when necessary
                            If Trim$(preparation(item).ErrorReplicateConc(replID)) = "" Then
                                'TR-AG 05/09/2012 -Validate the sample class 'cause predilution factor apply only to Patients not Controls
                                'Apply the predilutions when necessary
                                If String.Equals(preparation(0).SampleClass, "PATIENT") AndAlso test.PreDilutionFlag = True Then aux *= test.PredilutionFactor

                                'AG 23/02/2010
                                'If Trim$(test.PostDilutionType) <> "" Then aux *= test.PostDilutionFactor
                                If Trim$(preparation(item).PostDilutionType) <> "NONE" Then aux *= test.PostDilutionFactor

                                'Appy the slope function if programmed
                                If test.Slope.InUse Then aux = test.Slope.A * aux + test.Slope.B

                                preparation(item).ReplicateConc(replID) = aux
                                preparation(item).ErrorReplicateConc(replID) = ""

                                'AG 07/11/2012 - When Curve Calib all replicates all high are market as not inuse in order to no affect the average (same as Ax5)
                                '                but if curve is recalculated and the error conc disapear for these replicates they have to be marked as inused
                                '
                                'TODO
                                'v2: InUse requires a new field to know when the USER has reject the replicate and when has automatically the Sw
                                '    Only reactivate automatically (InUse = True) those replicates that user has not previously rejected
                                If preparation(item).RepInUse(replID) = False AndAlso calibrator.NumberOfCalibrators > 1 _
                                   AndAlso (String.Equals(preparation(0).SampleClass, "PATIENT") OrElse String.Equals(preparation(0).SampleClass, "CTRL")) Then

                                    preparation(item).RepInUse(replID) = True
                                    If preparation(item).ErrorAbs = GlobalEnumerates.AbsorbanceErrors.NO_INUSE_ELEMENTS.ToString Then
                                        preparation(item).ErrorAbs = ""
                                    End If
                                    recalculateAvgAbsFlag = True  'Recalculate the abs average with the new active replicates
                                End If
                                'AG 07/11/2012

                                'Replicate concentration date is the same field that replicate abs date

                            Else
                                preparation(item).RepInUse(replID) = False
                            End If 'If Trim$(preparation(item).ErrorReplicateConc(replID)) = "" Then


                        End If 'If Trim$(preparation(item).ErrorReplicateAbs(replID)) <> "" Or Trim$(test.ErrorBlankAbs) <> "" Or Trim$(calibrator.ErrorAbs) <> "" Then

                    Next

                    If recalculateAvgAbsFlag Then 'AG 07/11/2012 - When some replicate changes automatically from InUse = False -> True recalculate Abs Avg before calculate the new Conc Avg
                        CalculateAbsAverage()
                    End If

                End If 'If test.AbsorbanceFlag = False Then

            Catch ex As Exception
                Me.CatchLaunched("CalculateCalibration", ex)

                For i As Integer = 0 To UBound(preparation)
                    preparation(i).ReplicateConc(preparation(i).ReplicateID - 1) = ERROR_VALUE
                    preparation(i).ErrorReplicateConc(preparation(i).ReplicateID - 1) = GlobalEnumerates.ConcentrationErrors.SYSTEM_ERROR.ToString
                Next

            End Try
        End Sub


        ''' <summary>
        ''' Calculate the average concentration (with replicates from 1 to current)
        ''' </summary>
        ''' <remarks>
        ''' Created by: AG 11/02/2010 (Tested OK)
        ''' </remarks>
        Private Sub CalculateConcAverage()
            Try
                Dim itemMaxNumber As Integer = UBound(preparation) + 1

                'For all multiitemID
                For itemID As Integer = 0 To itemMaxNumber - 1
                    Dim inUse As Integer = 0
                    Dim Average As Single = 0
                    Dim currentRepl As Integer = preparation(itemID).ReplicateID

                    If preparation(itemID).SubstrateDepletion = 2 Then
                        preparation(itemID).Conc = ERROR_VALUE
                        preparation(itemID).ErrorConc = GlobalEnumerates.ConcentrationErrors.SUBSTRATE_DEPLETION.ToString

                    Else
                        Dim calibratorPoints As Integer = calibrator.NumberOfCalibrators

                        'AG 15/09/2010
                        'If calibratorPoints = 1 Then 'Calibrator with 1 item (factor)
                        If String.Equals(calibrator.CalibrationType, "FACTOR") Or calibratorPoints = 1 Then  'Factor (fixed or experimental)

                            For i As Integer = 0 To currentRepl - 1
                                'AG 02/08/2010 - Calculated average with no replicate concentration error
                                'If preparation(itemID).RepInUse(i) = True And preparation(itemID).SubstrateDepletionReplicate(i) = False Then
                                If preparation(itemID).RepInUse(i) = True And preparation(itemID).SubstrateDepletionReplicate(i) = False _
                                And preparation(itemID).ErrorReplicateConc(i) = "" Then
                                    'END AG 02/08/2010
                                    Average += preparation(itemID).ReplicateConc(i)
                                    inUse += 1
                                End If
                            Next

                            If inUse = 0 Then
                                preparation(itemID).Conc = ERROR_VALUE
                                preparation(itemID).ErrorConc = GlobalEnumerates.ConcentrationErrors.NO_INUSE_ELEMENTS.ToString
                                'myClassGlobalResult.HasError = True 'AG 02/08/2010
                                myClassGlobalResult.ErrorCode = GlobalEnumerates.ConcentrationErrors.NO_INUSE_ELEMENTS.ToString
                            Else
                                Average = Average / CSng(inUse)
                                preparation(itemID).Conc = Average
                                preparation(itemID).ErrorConc = ""
                                'Average date is the same field as average absorbance
                            End If 'If inUse = 0 Then

                        Else 'Calibrator with N items (curve)

                            'AG 11/03/2010                            
                            If Trim$(preparation(itemID).ErrorAbs) <> "" Or Trim$(test.ErrorBlankAbs) <> "" Or Trim$(calibrator.ErrorAbs) <> "" Or Trim$(calibrator.ErrorCalibration) <> "" Then

                                preparation(itemID).Conc = ERROR_VALUE
                                If preparation(itemID).ErrorAbs = GlobalEnumerates.AbsorbanceErrors.SUBSTRATE_DEPLETION.ToString Then
                                    preparation(itemID).ErrorConc = GlobalEnumerates.ConcentrationErrors.SUBSTRATE_DEPLETION.ToString
                                Else
                                    preparation(itemID).ErrorConc = GlobalEnumerates.ConcentrationErrors.OUT.ToString
                                End If
                                'END AG 11/03/2010

                            Else
                                Dim errorAvg As String = ""

                                'calculate average concentration applying curve to the average absorbance
                                preparation(itemID).Conc = Me.CalculateConcetrationWithCurve(preparation(itemID).Abs, calibrator.Curve.CurveGrowthType, errorAvg)
                                If myClassGlobalResult.HasError Then Exit Try
                                preparation(itemID).ErrorConc = errorAvg


                                'If no error during average concentration calculation (only in curves) apply several additional corrections when necessary
                                If Trim$(preparation(itemID).ErrorConc) = "" Then
                                    Dim aux As Single = preparation(itemID).Conc

                                    'TR-AG 05/09/2012 -Validate the sample class 'cause predilution factor apply only to Patients not Controls
                                    'Apply the pre and postdilutions when necessary
                                    If String.Equals(preparation(0).SampleClass, "PATIENT") AndAlso test.PreDilutionFlag = True Then aux *= test.PredilutionFactor

                                    'AG 23/02/2010
                                    'If Trim$(test.PostDilutionType) <> "" Then aux *= test.PostDilutionFactor
                                    If Trim$(preparation(itemID).PostDilutionType) <> "NONE" Then aux *= test.PostDilutionFactor

                                    'Appy the slope function if programmed
                                    If test.Slope.InUse Then aux = test.Slope.A * aux + test.Slope.B

                                    preparation(itemID).Conc = aux
                                    preparation(itemID).ErrorConc = ""
                                    'Replicate concentration date is the same field that replicate abs date
                                Else
                                End If 'If Trim$(preparation(itemID).ErrorConc) = "" Then
                            End If 'If preparation(itemID).ErrorAbs <> "" Then


                        End If 'If itemMaxNumber = 1 Then
                    End If 'If preparation(itemID).SubstrateDepletion = 2 Then

                Next

            Catch ex As Exception
                Me.CatchLaunched("CalculateConcAverage", ex)
                For i As Integer = 0 To UBound(preparation)
                    preparation(i).Conc = ERROR_VALUE
                    preparation(i).ErrorConc = GlobalEnumerates.ConcentrationErrors.SYSTEM_ERROR.ToString
                Next

            End Try
        End Sub


        ''' <summary>
        ''' Calculates the calibration curve with the current average absorbance for all multi item elements
        ''' </summary>
        ''' <remarks>
        ''' Created by: AG 11/02/2010
        '''             Poligonal (Tested OK)
        '''             LinearRegression (Tested OK)
        '''             Regression parabola (Tested OK)
        '''             Spline (Tested OK)
        ''' </remarks>
        Private Sub CalculateCalibrationCurve()
            Try
                'Prepare data to calculate curve
                Me.FillCalibrationCurveData()
                If myClassGlobalResult.HasError Then Exit Try 'Errors validation

                'Get the calibrator curve polynomial
                Select Case calibrator.Curve.CurveType
                    Case "POLYGONAL"
                        Me.CalculatePolygonalFunction()

                    Case "LINEAR" 'AG 04/09/2010 - Preloaded item isnot "LINEARREG"
                        Me.CalculateLinearRegressionFunction()

                    Case "PARABOLA" 'AG 04/09/2010 - Preloaded item isnot "PARABOLICREG"
                        Me.CalculateParabolicRegressionFunction()

                    Case "SPLINE"
                        Me.CalculateSplineFunction()
                End Select
                If myClassGlobalResult.HasError Then Exit Try 'Errors validation

                Me.GenerateCurveGraphicalTable()
                If myClassGlobalResult.HasError Then Exit Try 'Errors validation

                Me.CheckCurveMonotony()
                If myClassGlobalResult.HasError Then Exit Try 'Errors validation

            Catch ex As Exception
                Me.CatchLaunched("CalculateCalibrationCurve", ex)
                calibrator.ErrorAbs = GlobalEnumerates.CalibrationFactorErrors.SYSTEM_ERROR.ToString
            End Try
        End Sub


        ''' <summary>
        ''' Get the calibrator points concentration using the curve and then calculate the %error with the theorical concentration value
        ''' </summary>
        ''' <remarks>
        ''' Created by AG 15/02/2010 (Tested OK)
        ''' </remarks>
        Private Function CalculateCurveCalibrationError(ByVal pCaluclatedConc As Single, ByVal pTheoricalConc As Single) As Single
            Dim ErrorToReturn As Single = 0
            Try
                If Trim$(calibrator.ErrorCalibration) = "" Then
                    'calculate the error percentage                    
                    Dim myError As Single = 0

                    If pTheoricalConc > 0 Then
                        'AG 27/07/2012 - return value with sign (Sergi Tortosa 24/07/2012, revisio Fisica n 165)
                        'Note: BAx00 previous versions were adapted from Ax5 who returns the absolute value
                        'myError = 100 * Math.Abs((pTheoricalConc - pCaluclatedConc) / pTheoricalConc)
                        myError = 100 * ((pTheoricalConc - pCaluclatedConc) / pTheoricalConc)
                    Else
                        myError = 0
                    End If
                    ErrorToReturn = myError

                End If

            Catch ex As Exception
                Me.CatchLaunched("CalculateCurveCalibrationError", ex)
                calibrator.ErrorAbs = GlobalEnumerates.CalibrationFactorErrors.SYSTEM_ERROR.ToString
            End Try
            Return ErrorToReturn
        End Function



        ''' <summary>
        ''' Use the calibration curve points in order to calculate concentration
        ''' Adapt from Ax5 function (CalculosCurvaCalibracion.CalcularConcentraconConCurvaCalibracion)
        ''' </summary>
        ''' <returns>Double</returns>
        ''' <remarks>
        ''' Created by AG 15/02/2010 (Tested OK)
        ''' Modified by AG 07/09/2010
        ''' </remarks>
        Private Function CalculateConcetrationWithCurve(ByVal pAbsValue As Single, ByVal pCurveGrowthType As String, ByRef pErrorType As String) As Single
            Dim ConcToReturn As Single = 0
            Try
                Dim pointsNumber As Integer = calibrator.Curve.CurvePointsNumber + 1
                pErrorType = ""

                'Calculate only when monotonous curve
                If calibrator.ErrorCalibration = GlobalEnumerates.CalibrationFactorErrors.NON_MONOTONOUS_CURVE.ToString Then
                    ConcToReturn = ERROR_VALUE
                    pErrorType = GlobalEnumerates.ConcentrationErrors.OUT.ToString
                    Exit Try
                End If

                'Find the range (the correct point)
                Dim index As Integer = 0
                Dim signus As Integer = 1
                If pCurveGrowthType = "DEC" Then signus = -1

                Dim Found As Boolean = False
                For i As Integer = 0 To pointsNumber - 1
                    If signus * pAbsValue < signus * calibrator.Curve.Points(ABS_ID, i) Then
                        index = i
                        Found = True
                        Exit For
                    End If
                Next

                'Calculate concentration
                Dim Slope As Single = 0


                If Found And index = 0 Then   'pAbsValue < calibrator.Curve.Points(ABS_ID, 0) (blank absorbance) -Found in initial point
                    If calibrator.Curve.Points(ABS_ID, 1) <> calibrator.Curve.Points(ABS_ID, 0) Then
                        Slope = (calibrator.Curve.Points(CONC_ID, 1) - calibrator.Curve.Points(CONC_ID, 0)) / (calibrator.Curve.Points(ABS_ID, 1) - calibrator.Curve.Points(ABS_ID, 0))
                        'ConcToReturn = Me.CalculateConcentrationExtrapolated(Slope, pAbsValue, calibrator.Curve.Points(ABS_ID, 0), calibrator.Curve.Points(CONC_ID, 0))
                    Else
                        'ConcToReturn = 0
                        Slope = 0
                    End If
                    'AG 26/01/2012 - call calculateconcen... out of IF and comment the previous 2 lines
                    ConcToReturn = Me.CalculateConcentrationExtrapolated(Slope, pAbsValue, calibrator.Curve.Points(ABS_ID, 0), calibrator.Curve.Points(CONC_ID, 0))


                ElseIf Found = False Then   'pAbsValue > calibrator.Curve.Points(ABS_ID, MaxPoints) - High - Not found SampleAbs > CalibratorAbs
                    If calibrator.Curve.Points(ABS_ID, pointsNumber - 1) <> calibrator.Curve.Points(ABS_ID, pointsNumber - 2) Then
                        Slope = (calibrator.Curve.Points(CONC_ID, pointsNumber - 1) - calibrator.Curve.Points(CONC_ID, pointsNumber - 2)) / (calibrator.Curve.Points(ABS_ID, pointsNumber - 1) - calibrator.Curve.Points(ABS_ID, pointsNumber - 2))
                        ConcToReturn = Me.CalculateConcentrationExtrapolated(Slope, pAbsValue, calibrator.Curve.Points(ABS_ID, pointsNumber - 1), calibrator.Curve.Points(CONC_ID, pointsNumber - 1))

                        'Check if the extrapolated concentration is higher than the extrapolation maximum value
                        'AG 07/09/2010
                        'Dim outRangeValue As Integer = Me.CheckIfOutOfRange(ConcToReturn)
                        'If outRangeValue < 0 Then
                        '    ConcToReturn = ERROR_VALUE
                        '    Select Case outRangeValue
                        '        Case -1
                        '            pErrorType = GlobalEnumerates.ConcentrationErrors.OUT.ToString
                        '        Case -2
                        '            pErrorType = GlobalEnumerates.ConcentrationErrors.OUT_HIGH.ToString
                        '    End Select
                        'End If
                        Dim outRangeValue As Integer = Me.CheckIfOutOfRange(ConcToReturn, pErrorType)
                        If outRangeValue < 0 Then
                            ConcToReturn = ERROR_VALUE
                        End If
                        'END AG 07/09/2010
                    Else
                        ConcToReturn = ERROR_VALUE
                        'AG 07/09/2010
                        'pErrorType = SYSTEM_ERROR
                        pErrorType = GlobalEnumerates.ConcentrationErrors.OUT.ToString
                    End If


                Else    'Found in intermediate point
                    Dim auxConc As Double = calibrator.Curve.Points(CONC_ID, index) - calibrator.Curve.Points(CONC_ID, index - 1)
                    Dim auxAbs As Double = calibrator.Curve.Points(ABS_ID, index) - calibrator.Curve.Points(ABS_ID, index - 1)

                    If auxAbs <> 0 Then
                        ConcToReturn = CSng((auxConc / auxAbs) * (pAbsValue - calibrator.Curve.Points(ABS_ID, index - 1)) + calibrator.Curve.Points(CONC_ID, index - 1))
                    Else
                        ConcToReturn = 0
                    End If

                End If

            Catch ex As Exception
                Me.CatchLaunched("CalculateConcetrationWithCurve", ex)
                calibrator.ErrorAbs = GlobalEnumerates.CalibrationFactorErrors.SYSTEM_ERROR.ToString
                ConcToReturn = ERROR_VALUE
                pErrorType = SYSTEM_ERROR
            End Try

            Return ConcToReturn
        End Function

#End Region

#Region "Calibrator curve functions"


        ''' <summary>
        ''' Prepare data to calculate curve
        ''' Adapted from Ax5 function (CalculosCurvaCalibracion.RellenarDatosCurvaCalibracion)
        ''' </summary>
        ''' <remarks>
        ''' Created by: AG 11/02/2010 (Tested OK)
        ''' </remarks>
        Private Sub FillCalibrationCurveData()
            Try

                Dim init As Integer = 0
                Dim numCalib As Integer = calibrator.NumberOfCalibrators

                'Linear relationship between axis (one more point)
                If calibrator.Curve.CurveAxisXType = "LINEAR" And calibrator.Curve.CurveAxisYType = "LINEAR" Then
                    myLocalCurve.n = numCalib + 1

                    ReDim myLocalCurve.X(myLocalCurve.n)
                    ReDim myLocalCurve.Y(myLocalCurve.n)
                    ReDim myLocalCurve.Points(1, calibrator.Curve.CurvePointsNumber)    '(0..CurvePointsNumber) ... CurvePointsNumber + 1

                    'Origin
                    myLocalCurve.X(0) = 0 'Blank concentration is 0
                    myLocalCurve.Y(0) = test.BlankAbs   'Blank absorbance

                    init = 1
                    For i As Integer = 1 To numCalib
                        myLocalCurve.X(init) = calibrator.TheoreticalConcentration(i - 1)
                        myLocalCurve.Y(init) = calibrator.Curve.CalibratorAbs(i - 1) + myBlankDifference
                        init += 1
                    Next
                    myLocalCurve.Xmin = myLocalCurve.X(1)
                    myLocalCurve.Xmax = myLocalCurve.X(numCalib)

                    'Non Linear relationship between axis
                Else
                    myLocalCurve.n = numCalib

                    ReDim myLocalCurve.X(myLocalCurve.n)
                    ReDim myLocalCurve.Y(myLocalCurve.n)
                    ReDim myLocalCurve.Points(1, calibrator.Curve.CurvePointsNumber)    '    '(0..CurvePointsNumber) ... CurvePointsNumber + 1

                    init = 0
                    For i As Integer = 1 To numCalib
                        myLocalCurve.X(init) = calibrator.TheoreticalConcentration(i - 1)
                        myLocalCurve.Y(init) = calibrator.Curve.CalibratorAbs(i - 1) + myBlankDifference
                        init += 1
                    Next
                    myLocalCurve.Xmin = myLocalCurve.X(0)
                    myLocalCurve.Xmax = myLocalCurve.X(numCalib - 1)
                End If

                If test.ReactionType = "INC" Then
                    myLocalCurve.Ymin = calibrator.Curve.CalibratorAbs(0)
                    myLocalCurve.Ymax = calibrator.Curve.CalibratorAbs(numCalib - 1)
                Else
                    myLocalCurve.Ymin = calibrator.Curve.CalibratorAbs(numCalib - 1)
                    myLocalCurve.Ymax = calibrator.Curve.CalibratorAbs(0)
                End If

                'Conversion type depending the axis relationships
                If calibrator.Curve.CurveAxisXType = "LINEAR" And calibrator.Curve.CurveAxisYType = "LINEAR" Then
                    myLocalCurve.Conversion = 1
                    myLocalCurve.Control = 1    'OK

                ElseIf calibrator.Curve.CurveAxisXType = "LINEAR" And calibrator.Curve.CurveAxisYType = "LOG" Then
                    myLocalCurve.Conversion = 2

                ElseIf calibrator.Curve.CurveAxisXType = "LOG" And calibrator.Curve.CurveAxisYType = "LINEAR" Then
                    myLocalCurve.Conversion = 3

                ElseIf calibrator.Curve.CurveAxisXType = "LOG" And calibrator.Curve.CurveAxisYType = "LOG" Then
                    myLocalCurve.Conversion = 4
                End If

                Me.ConvertDataAxis(myLocalCurve.Conversion)

            Catch ex As Exception
                Me.CatchLaunched("FillCalibrationCurveData", ex)
                calibrator.ErrorAbs = GlobalEnumerates.CalibrationFactorErrors.SYSTEM_ERROR.ToString
            End Try
        End Sub


        ''' <summary>
        ''' Convert data axis X or Y in calibration curve depending the axis relationships
        ''' Adapted from Ax5 functions (CalculosCurvaCalibracion.Conversio2, Converio3 and Conversio4)
        ''' </summary>
        ''' <param name="pType"></param>
        ''' <remarks>
        ''' Created by: AG 11/02/2010 (Tested OK)
        ''' </remarks>
        Private Sub ConvertDataAxis(ByVal pType As Integer)
            Try
                Select Case pType
                    Case 1  'X linear, Y linear
                        'No conversion
                        myLocalCurve.Control = 1 'OK

                    Case 2 'X linear, Y log
                        For i As Integer = 0 To myLocalCurve.n
                            If myLocalCurve.Y(i) <= 0 Then
                                myLocalCurve.Y(i) = CSng(Math.Log(0.01))
                            Else
                                myLocalCurve.Y(i) = CSng(Math.Log(myLocalCurve.Y(i)))
                            End If
                        Next
                        myLocalCurve.Control = 1 'OK

                    Case 3 'X log, Y linear
                        For i As Integer = 0 To myLocalCurve.n
                            If myLocalCurve.X(i) <= 0 Then
                                myLocalCurve.X(i) = CSng(Math.Log(0.01))
                            Else
                                myLocalCurve.X(i) = CSng(Math.Log(myLocalCurve.X(i)))
                            End If
                        Next
                        myLocalCurve.Control = 1 'OK

                    Case 4 'X log, Y log
                        For i As Integer = 0 To myLocalCurve.n
                            If myLocalCurve.Y(i) <= 0 Then
                                myLocalCurve.Y(i) = CSng(Math.Log(0.01))
                            Else
                                myLocalCurve.Y(i) = CSng(Math.Log(myLocalCurve.Y(i)))
                            End If

                            If myLocalCurve.X(i) <= 0 Then
                                myLocalCurve.X(i) = CSng(Math.Log(0.01))
                            Else
                                myLocalCurve.X(i) = CSng(Math.Log(myLocalCurve.X(i)))
                            End If
                        Next
                        myLocalCurve.Control = 1 'OK

                End Select

            Catch ex As Exception
                Me.CatchLaunched("ConvertDataAxis", ex)
                myLocalCurve.Control = 0 'NOK
                calibrator.ErrorAbs = GlobalEnumerates.CalibrationFactorErrors.SYSTEM_ERROR.ToString
            End Try
        End Sub


        ''' <summary>
        ''' Calculate the calibration curve using a polygonal function between points
        ''' Adapted from Ax5 function (CalculosCurvaCalibracion.CalcularFuncionPoligonal)
        ''' </summary>
        ''' <remarks>
        ''' Created by: AG 12/02/2010 (Tested OK)
        ''' </remarks>
        Private Sub CalculatePolygonalFunction()
            Try
                With myLocalCurve
                    For i As Integer = 0 To .n - 2  'NOTE: n - 2!
                        Dim slope As Double = 0

                        If .X(i + 1) <> .X(i) Then
                            slope = (.Y(i + 1) - .Y(i)) / (.X(i + 1) - .X(i))
                        Else
                            slope = 0
                        End If

                        If UBound(calibrator.Curve.Coefficient, 2) >= i Then
                            calibrator.Curve.Coefficient(0, i) = .Y(i)
                            calibrator.Curve.Coefficient(1, i) = slope
                            calibrator.Curve.Coefficient(2, i) = 0
                            calibrator.Curve.Coefficient(3, i) = 0
                        End If
                    Next
                End With

            Catch ex As Exception
                Me.CatchLaunched("CalculatePolygonalFunction", ex)
                myLocalCurve.Control = 0 'NOK
                calibrator.ErrorAbs = GlobalEnumerates.CalibrationFactorErrors.SYSTEM_ERROR.ToString
            End Try
        End Sub


        ''' <summary>
        ''' Calculate the calibration curve using a linear regression function between points
        ''' Adapted from Ax5 function (CalculosCurvaCalibracion.CalcularFuncionRegresionParabolica)
        ''' </summary>
        ''' <remarks>
        ''' Created by: AG 12/02/2010 (Tested OK)
        ''' </remarks>
        Private Sub CalculateLinearRegressionFunction()
            Try
                Dim sx As Double = 0
                Dim sy As Double = 0
                Dim sx2 As Double = 0
                Dim sy2 As Double = 0
                Dim sxy As Double = 0
                Dim a1 As Double = 0
                Dim a0 As Double = 0
                Dim E As Double = 0
                Dim n As Integer = 0

                With myLocalCurve
                    n = .n
                    'Calculate sum
                    sx = 0
                    sy = 0
                    sx2 = 0
                    sxy = 0
                    For i As Integer = 0 To n - 1
                        sx = sx + .X(i)
                        sy = sy + .Y(i)
                        sx2 = sx2 + (.X(i) * .X(i))
                        sy2 = sy2 + (.Y(i) * .Y(i)) 'AG 01/07/2011
                        sxy = sxy + (.X(i) * .Y(i))
                    Next i

                    If (n * sx2 - sx * sx) <> 0 Then
                        a1 = (n * sxy - sx * sy) / (n * sx2 - sx * sx)
                        a0 = (sy * sx2 - sx * sxy) / (n * sx2 - sx ^ 2)
                    Else
                        a1 = 0
                        a0 = 0
                    End If

                    For i As Integer = 0 To n - 2   'NOTE: n - 2!
                        E = a0 + a1 * .X(i)
                        calibrator.Curve.Coefficient(0, i) = E
                        calibrator.Curve.Coefficient(1, i) = a1
                        calibrator.Curve.Coefficient(2, i) = 0
                        calibrator.Curve.Coefficient(3, i) = 0
                    Next

                    'AG 01/07/2011 - Calculate correlation factor
                    'Correlation Factor = (n * sxy - sx * sy) / sqrt ((n * sx2 - sx ^ 2) * (n * sy2 - sy ^ 2))
                    calibrator.Curve.CorrelationFactor = 0
                    a0 = (n * sx2 - sx ^ 2)
                    a1 = (n * sy2 - sy ^ 2)
                    If a0 > 0 And a1 > 0 Then
                        calibrator.Curve.CorrelationFactor = CSng((n * sxy - sx * sy) / CSng(Math.Sqrt(a0 * a1)))
                    End If
                    'AG 01/07/2011

                End With

            Catch ex As Exception
                Me.CatchLaunched("CalculateLinearRegressionFunction", ex)
                myLocalCurve.Control = 0 'NOK
                calibrator.ErrorAbs = GlobalEnumerates.CalibrationFactorErrors.SYSTEM_ERROR.ToString
            End Try
        End Sub


        ''' <summary>
        ''' Calculate the calibration curve using a parabolic regression function between points
        ''' Adapted from Ax5 function (CalculosCurvaCalibracion.CalcularFuncionRegresionParabolica)
        ''' </summary>
        ''' <remarks>
        ''' Created by: AG 12/02/2010 (Tested OK)
        ''' </remarks>
        Private Sub CalculateParabolicRegressionFunction()
            Try
                Dim n As Integer = 0
                Dim sx As Double = 0
                Dim sy As Double = 0
                Dim sx2 As Double = 0
                Dim sxy As Double = 0
                Dim sx2y As Double = 0
                Dim sx3 As Double = 0
                Dim sx4 As Double = 0
                Dim a2 As Double = 0
                Dim a1 As Double = 0
                Dim a0 As Double = 0
                Dim xi2 As Double = 0
                Dim xi3 As Double = 0
                Dim c2 As Double = 0
                Dim c3 As Double = 0
                Dim c4 As Double = 0
                Dim cxy As Double = 0
                Dim cx2y As Double = 0
                'Dim E As Double = 0

                With myLocalCurve
                    n = .n
                    sx = 0
                    sy = 0
                    sx2 = 0
                    sxy = 0

                    For i As Integer = 0 To n - 1
                        sx = sx + .X(i)
                        sy = sy + .Y(i)
                        sxy = sxy + .X(i) * .Y(i)
                        xi2 = .X(i) * .X(i)
                        sx2 = sx2 + xi2
                        sx2y = sx2y + xi2 * .Y(i)
                        xi3 = xi2 * .X(i)
                        sx3 = sx3 + xi3
                        sx4 = sx4 + xi3 * .X(i)
                    Next i

                    c2 = sx2 - sx * sx / n
                    c3 = sx3 - sx2 * sx / n
                    c4 = sx4 - sx2 * sx2 / n
                    cxy = sxy - sx * sy / n
                    cx2y = sx2y - sx2 * sy / n

                    If (c3 * c3 - c4 * c2) <> 0 Then
                        a2 = (c3 * cxy - cx2y * c2) / (c3 * c3 - c4 * c2)
                    Else
                        a2 = 0
                    End If

                    If c2 <> 0 Then
                        a1 = (cxy - a2 * c3) / c2
                    Else
                        a1 = 0
                    End If

                    a0 = (sy - a1 * sx - a2 * sx2) / n

                    For i As Integer = 0 To n - 2   'NOTE: n-2!
                        calibrator.Curve.Coefficient(0, i) = a0 + a1 * .X(i) + a2 * .X(i) * .X(i)
                        calibrator.Curve.Coefficient(1, i) = a1 + 2 * a2 * .X(i)
                        calibrator.Curve.Coefficient(2, i) = a2
                        calibrator.Curve.Coefficient(3, i) = 0
                    Next i

                End With

            Catch ex As Exception
                Me.CatchLaunched("CalculateLinearRegressionFunction", ex)
                myLocalCurve.Control = 0 'NOK
                calibrator.ErrorAbs = GlobalEnumerates.CalibrationFactorErrors.SYSTEM_ERROR.ToString
            End Try
        End Sub


        ''' <summary>
        ''' Calculate the calibration curve using a spline function between points
        ''' Adapted from Ax5 function (CalculosCurvaCalibracion.CalcularFuncionSpline)
        ''' </summary>
        ''' <remarks>
        ''' Created by: AG 12/02/2010 (Tested OK)
        ''' </remarks>
        Private Sub CalculateSplineFunction()
            Try
                Dim n As Integer = 0
                Dim reserva As Double = 0
                Dim DeltaX(15) As Double
                Dim DeltaY(15) As Double
                Dim Delta(15) As Double
                Dim Lines(15) As String
                Dim coeff(15) As Double

                With myLocalCurve
                    n = .n

                    For i As Integer = 1 To .n - 1
                        DeltaX(i) = .X(i) - .X(i - 1)
                        DeltaY(i) = .Y(i) - .Y(i - 1)

                        If DeltaX(i) <> 0 Then
                            Delta(i) = DeltaY(i) / DeltaX(i)
                        Else
                            Delta(i) = 0
                        End If
                    Next i

                    'Me.CalculateCurveSlope()    '(Ax5 CalculPendents) Dont needed because dont update any variable use for calculate the spline function
                    'If myClassGlobalResult.HasError then Exit Try 

                    Me.CheckForMonotonyAndModify(Delta, coeff, Lines)
                    If myClassGlobalResult.HasError Then Exit Try

                    For i As Integer = 0 To .n - 2  'NOTE: n-2!
                        If Lines(i) = "s" Then
                            reserva = coeff(i)
                            coeff(i) = Delta(i + 1)
                        End If
                        calibrator.Curve.Coefficient(0, i) = .Y(i)
                        calibrator.Curve.Coefficient(1, i) = coeff(i)

                        If DeltaX(i + 1) <> 0 Then
                            calibrator.Curve.Coefficient(2, i) = (3 * Delta(i + 1) - 2 * coeff(i) - coeff(i + 1)) / DeltaX(i + 1)
                            calibrator.Curve.Coefficient(3, i) = (coeff(i) + coeff(i + 1) - 2 * Delta(i + 1)) / DeltaX(i + 1) / DeltaX(i + 1)
                        Else
                            calibrator.Curve.Coefficient(2, i) = 0
                            calibrator.Curve.Coefficient(3, i) = 0
                        End If

                        If Lines(i) = "s" Then
                            coeff(i) = reserva
                        End If
                    Next i
                End With

            Catch ex As Exception
                Me.CatchLaunched("CalculateSplineFunction", ex)
                myLocalCurve.Control = 0 'NOK
                calibrator.ErrorAbs = GlobalEnumerates.CalibrationFactorErrors.SYSTEM_ERROR.ToString
            End Try
        End Sub



        ''' <summary>
        ''' Calculate the slope’s curve
        ''' Adapted from Ax5 (CalculosCurvaCalibracion.CalculPendents) function
        ''' 
        ''' NOTE: Dont use in Ax00 -> Ax5 function dont update any variable use in calculation spline curve
        ''' </summary>
        ''' <remarks>
        ''' Created by: AG 12/02/2010 (Tested OK ... No code inside)
        ''' </remarks>
        Private Sub CalculateCurveSlope()
            Try
                'Pending adapt from Ax5. We dont adapt in Ax00 because dont change any variable used in spline curve calculation

            Catch ex As Exception
                Me.CatchLaunched("CalculateCurveSlope", ex)
                myLocalCurve.Control = 0 'NOK
                calibrator.ErrorAbs = GlobalEnumerates.CalibrationFactorErrors.SYSTEM_ERROR.ToString
            End Try
        End Sub


        ''' <summary>
        ''' Check for curve monotony and modify coefficients
        ''' Adapted from Ax5 function (MonotoniaIModificacio)
        ''' </summary>
        ''' <remarks>
        ''' Created by: AG 12/02/2010 (Test OK)
        ''' </remarks>
        Private Sub CheckForMonotonyAndModify(ByVal pDelta() As Double, ByRef pCoeff() As Double, ByRef pLines() As String)
            Try
                Dim myType As String = calibrator.Curve.CurveGrowthType

                For i As Integer = 0 To myLocalCurve.n - 2  'NOTE: n-2!
                    Dim myLine As String = pLines(i)
                    If Me.Sign(pCoeff(i + 1)) <> myType Then
                        Me.Modify(i, myType, pDelta, pCoeff, myLine)

                    Else
                        Dim aux As Double = 0
                        aux = pCoeff(i) + pCoeff(i + 1) - 3 * pDelta(i + 1)
                        If ((pCoeff(i) * pCoeff(i + 1)) < (aux * aux)) Then
                            If ((Sign(pCoeff(i) + 2 * pCoeff(i + 1) - 3 * pDelta(i + 1))) = myType) And ((Sign(2 * pCoeff(i) + pCoeff(i + 1) - 3 * pDelta(i + 1))) = myType) Then
                                Me.Modify(i, myType, pDelta, pCoeff, myLine)
                            End If

                        End If
                    End If 'If Me.Sign(pCoeff(i + 1)) <> myType Then

                    If myClassGlobalResult.HasError Then Exit For
                    pLines(i) = myLine

                Next
                If myClassGlobalResult.HasError Then Exit Try


            Catch ex As Exception
                Me.CatchLaunched("CalculateCurveSlope", ex)
                myLocalCurve.Control = 0 'NOK
                calibrator.ErrorAbs = GlobalEnumerates.CalibrationFactorErrors.SYSTEM_ERROR.ToString
            End Try
        End Sub


        ''' <summary>
        ''' Adapted from Ax5 function (CalculosCurvaCalibracion.Signe)
        ''' </summary>
        ''' <param name="pValue"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: AG 12/02/2010 (Tested OK)
        ''' </remarks>
        Private Function Sign(ByVal pValue As Double) As String
            Dim myReturn As String = ""
            Try
                myReturn = ""
                If pValue > 0 Then
                    myReturn = "INC"
                ElseIf pValue < 0 Then
                    myReturn = "DEC"
                End If

            Catch ex As Exception
                Me.CatchLaunched("Sign", ex)
                myLocalCurve.Control = 0 'NOK
                calibrator.ErrorAbs = GlobalEnumerates.CalibrationFactorErrors.SYSTEM_ERROR.ToString
            End Try
            Return myReturn
        End Function


        ''' <summary>
        ''' Adapted from Ax5 function (CalculosCurvaCalibracion.Modifica)
        ''' </summary>
        ''' <param name="pIndex"></param>
        ''' <param name="pType"></param>
        ''' <param name="pDelta"></param>
        ''' <param name="pCoeff"></param>
        ''' <param name="pLine"></param>
        ''' <remarks>
        ''' Created by: AG 12/02/2010 (Tested OK)
        ''' </remarks>
        Private Sub Modify(ByVal pIndex As Integer, ByVal pType As String, ByVal pDelta() As Double, ByRef pCoeff() As Double, ByRef pLine As String)
            Try
                Dim aux As Double = 0

                pLine = "n"
                If pType = "INC" Then
                    If pCoeff(pIndex) < pDelta(pIndex + 1) Then
                        pCoeff(pIndex + 1) = (9 * pDelta(pIndex + 1) - 5 * pCoeff(pIndex)) / 4
                    Else
                        If pCoeff(pIndex) < (3 * pDelta(pIndex + 1)) Then
                            aux = 3 * pDelta(pIndex + 1) - 2 * pCoeff(pIndex)
                            If aux <= 0 Then aux = 0
                            pCoeff(pIndex + 1) = (aux + 3 * pDelta(pIndex + 1) - pCoeff(pIndex)) / 2
                        Else
                            If pCoeff(pIndex) >= (4 * pDelta(pIndex + 1)) Then
                                Call ModifyStraight(pIndex, pType, pDelta, pCoeff, pLine)
                            Else
                                pCoeff(pIndex + 1) = (6 * pDelta(pIndex + 1) - pCoeff(pIndex)) / 2
                            End If
                        End If
                    End If
                Else
                    If pCoeff(pIndex) > pDelta(pIndex + 1) Then
                        pCoeff(pIndex + 1) = (9 * pDelta(pIndex + 1) - 5 * pCoeff(pIndex)) / 4
                    Else
                        If pCoeff(pIndex) > (3 * pDelta(pIndex + 1)) Then
                            aux = 3 * pDelta(pIndex + 1) - 2 * pCoeff(pIndex)
                            If aux >= 0 Then aux = 0
                            pCoeff(pIndex + 1) = (aux + 3 * pDelta(pIndex + 1) - pCoeff(pIndex)) / 2
                        Else
                            If pCoeff(pIndex) <= (4 * pDelta(pIndex + 1)) Then
                                Call ModifyStraight(pIndex, pType, pDelta, pCoeff, pLine)
                            Else
                                pCoeff(pIndex + 1) = (6 * pDelta(pIndex + 1) - pCoeff(pIndex)) / 2
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                Me.CatchLaunched("Modify", ex)
                myLocalCurve.Control = 0 'NOK
                calibrator.ErrorAbs = GlobalEnumerates.CalibrationFactorErrors.SYSTEM_ERROR.ToString
            End Try
        End Sub


        ''' <summary>
        ''' Adapted from Ax5 function (CalculosCurvaCalibracion.ModificaRecta)
        ''' </summary>
        ''' <param name="pIndex"></param>
        ''' <param name="pType"></param>
        ''' <param name="pDelta"></param>
        ''' <param name="pCoeff"></param>
        ''' <param name="pLine"></param>
        ''' <remarks>
        ''' Created by: AG 12/02/2010 (Tested Pending - never is called)
        ''' </remarks>
        Private Sub ModifyStraight(ByVal pIndex As Integer, ByVal pType As String, ByVal pDelta() As Double, ByRef pCoeff() As Double, ByRef pLine As String)
            Try
                Dim poss As Integer = 0
                Dim reserva As Double = 0
                Dim aux As Double = 0
                Dim aux2 As Double = 0

                pCoeff(pIndex + 1) = pDelta(pIndex + 1)
                reserva = pCoeff(pIndex)
                pCoeff(pIndex) = pDelta(pIndex + 1)
                If pIndex > 0 Then aux2 = pCoeff(pIndex - 1) Else aux2 = 0

                aux = aux2 + pCoeff(pIndex) - 3 * pDelta(pIndex)
                aux = aux * aux

                'AG 07/09/2010 - Fix system error (index error)
                'If (pCoeff(pIndex) >= aux) And (pCoeff(pIndex - 1) >= aux) Then
                Dim auxCoeff As Double = 0
                If pIndex > 1 Then auxCoeff = pCoeff(pIndex - 1)
                If (pCoeff(pIndex) >= aux) And (auxCoeff >= aux) Then
                    'END AG 07/09/2010
                    poss = 1
                Else
                    If (Me.Sign(aux2 + 2 * pCoeff(pIndex) - 3 * pDelta(pIndex)) = pType) And (Me.Sign(2 * aux2 + pCoeff(pIndex) - 3 * pDelta(pIndex)) = pType) Then
                        poss = -1
                    Else
                        poss = 1
                    End If
                End If
                If poss = -1 Then
                    pCoeff(pIndex) = reserva
                    pLine = "s"
                End If

            Catch ex As Exception
                Me.CatchLaunched("ModifyStraight", ex)
                myLocalCurve.Control = 0 'NOK
                calibrator.ErrorAbs = GlobalEnumerates.CalibrationFactorErrors.SYSTEM_ERROR.ToString
            End Try
        End Sub


        ''' <summary>
        ''' Calculate the calibration curve points depending on the curve type and axis relationships
        ''' 
        ''' Adapted from Ax5 function (CalculosCurvaCalibracion.GenerarTablaGraficaCurva)
        ''' </summary>
        ''' <remarks>
        ''' Created by AG 15/02/2010 (Tested OK)
        ''' </remarks>
        Private Sub GenerateCurveGraphicalTable()
            Try
                Dim curvePoints As Integer = calibrator.Curve.CurvePointsNumber + 1
                If curvePoints <= 1 Then Exit Try

                Dim calibratorNumber As Integer = calibrator.NumberOfCalibrators

                'Generate the concentration points in table
                Me.GenerateConcentrationPoints()
                If myClassGlobalResult.HasError Then Exit Try

                'Generate the absorbance points in table
                For i As Integer = 0 To curvePoints - 1
                    Dim X As Double = 0

                    If calibrator.Curve.CurveAxisXType = "LINEAR" Then
                        X = myLocalCurve.Points(CONC_ID, i)
                    ElseIf calibrator.Curve.CurveAxisXType = "LOG" Then
                        X = Math.Log(myLocalCurve.Points(CONC_ID, i))
                    End If

                    'Find the interval where the current concentration is
                    Dim TempX As Double = 0
                    Dim Index As Integer = 0
                    Dim EndjLoop As Boolean = False
                    For j As Integer = 0 To calibratorNumber - 1
                        If X <= myLocalCurve.X(j) Then   'X < myLocalCurve.X(0)
                            TempX = myLocalCurve.X(j)
                            Index = j   'j = 0
                            EndjLoop = True

                            'X(j) <= X <= X(j+1) - If concentration value is between 2 values: use the lower limit (interpolation)
                        ElseIf myLocalCurve.X(j) <= X And X <= myLocalCurve.X(j + 1) Then
                            TempX = myLocalCurve.X(j)
                            Index = j
                            EndjLoop = True

                            'X > Xmax - If value > maximum concnetration: use the upper limit
                            'ElseIf X >= myLocalCurve.Xmax Then
                            '    TempX = myLocalCurve.Xmax
                            '    Index = calibratorNumber
                            '    EndjLoop = True
                        End If

                        If EndjLoop Then Exit For
                    Next

                    myLocalCurve.Points(ABS_ID, i) = CSng(Me.Polynom(Index, X - TempX, calibrator.Curve.Coefficient))

                    If calibrator.Curve.CurveAxisYType = "LOG" Then
                        myLocalCurve.Points(ABS_ID, i) = CSng(Math.Exp(myLocalCurve.Points(ABS_ID, i)))
                    End If

                Next    'For i As Integer = 0 To curvePoints - 1


            Catch ex As Exception
                Me.CatchLaunched("GenerateCurveGraphicalTable", ex)
                myLocalCurve.Control = 0 'NOK
                calibrator.ErrorAbs = GlobalEnumerates.CalibrationFactorErrors.SYSTEM_ERROR.ToString
            End Try
        End Sub



        ''' <summary>
        ''' Fill the concentration points in curve table
        ''' </summary>
        ''' <remarks>
        ''' Created by AG 15/02/2010 (Tested OK)
        ''' </remarks>
        Private Sub GenerateConcentrationPoints()
            Try
                Dim increase As Single
                Dim curvePoints As Integer = calibrator.Curve.CurvePointsNumber + 1
                If curvePoints <= 1 Then Exit Try

                'Linear axis (X and Y) (myLocalCurve.Conversion =1)
                If calibrator.Curve.CurveAxisXType = "LINEAR" And calibrator.Curve.CurveAxisYType = "LINEAR" Then
                    increase = myLocalCurve.Xmax / (curvePoints - 1)
                    For i As Integer = 0 To curvePoints - 1
                        myLocalCurve.Points(CONC_ID, i) = CSng(i) * increase
                    Next
                    myLocalCurve.Points(CONC_ID, curvePoints - 1) = myLocalCurve.Xmax

                    'calibrator.Curve.CurveAxisXType = "LOG"
                ElseIf calibrator.Curve.CurveAxisXType = "LOG" Then
                    increase = (myLocalCurve.Xmax - myLocalCurve.Xmin) / (curvePoints - 1)
                    For i As Integer = 0 To curvePoints - 1
                        myLocalCurve.Points(CONC_ID, i) = CSng(i) * increase
                        myLocalCurve.Points(CONC_ID, i) += myLocalCurve.Xmin
                    Next
                    myLocalCurve.Points(CONC_ID, curvePoints - 1) = myLocalCurve.Xmax

                    'X linear and Y log
                ElseIf calibrator.Curve.CurveAxisYType = "LOG" Then
                    Dim s1 As Double = 0
                    Dim s2 As Double = 0
                    Dim inc1 As Double = 0

                    s1 = myLocalCurve.Xmax
                    s2 = myLocalCurve.Xmin

                    If (s2 <> 0 And s1 <> 0) Then
                        increase = CSng(s1 / s2)
                        increase = CSng(Math.Log(increase))
                        inc1 = Math.Log(s2)
                    Else
                        increase = 0
                        inc1 = 0
                    End If

                    For i As Integer = 0 To curvePoints - 1
                        s2 = i
                        s1 = inc1 + s2 * increase / 200
                        myLocalCurve.Points(CONC_ID, i) = CSng(Math.Exp(s1))
                    Next i

                End If

            Catch ex As Exception
                Me.CatchLaunched("GenerateConcentrationPoints", ex)
                myLocalCurve.Control = 0 'NOK
                calibrator.ErrorAbs = GlobalEnumerates.CalibrationFactorErrors.SYSTEM_ERROR.ToString
            End Try

        End Sub



        ''' <summary>
        ''' y = f(X) - Returns the Absorbances (Y) for a specific concentration (X) in the current curve
        ''' Adapted from Ax5 function (CalculosCurvaCalibracion.Polinom)
        ''' </summary>
        ''' <param name="pIndex"></param>
        ''' <param name="X"></param>
        ''' <param name="pCoeff"></param>
        ''' <returns>Double</returns>
        ''' <remarks>
        ''' Created by AG 15/02/2010 (Tested OK)
        ''' </remarks>
        Private Function Polynom(ByVal pIndex As Integer, ByVal X As Double, ByVal pCoeff(,) As Double) As Double
            Dim Y As Double = 0
            Try
                'AG 07/09/2010 - Fix formula (bad copied from Ax5)
                'Y = pCoeff(0, pIndex) + X * pCoeff(1, pIndex) + X * pCoeff(2, pIndex) + X * pCoeff(3, pIndex)
                Y = pCoeff(0, pIndex) + X * (pCoeff(1, pIndex) + X * (pCoeff(2, pIndex) + X * (pCoeff(3, pIndex))))

            Catch ex As Exception
                Me.CatchLaunched("Polynom", ex)
                myLocalCurve.Control = 0 'NOK
                calibrator.ErrorAbs = GlobalEnumerates.CalibrationFactorErrors.SYSTEM_ERROR.ToString
            End Try
            Return Y
        End Function


        ''' <summary>
        ''' Check if curve is monotonous or not
        ''' Adapted from Ax5 function (CalculosCurvaCalibracion.ComprobarMonotoniaCurvaCalib)
        ''' </summary>
        ''' <remarks>
        ''' Created by AG 15/02/2010 (Tested OK)
        ''' </remarks>
        Private Sub CheckCurveMonotony()
            Try
                Dim Signus As Integer = 1
                Dim curvePoints As Integer = calibrator.Curve.CurvePointsNumber
                Dim Monotonous As Boolean = True

                If calibrator.Curve.CurveGrowthType = "INC" Then
                    Signus = 1
                Else
                    Signus = -1
                End If

                For i As Integer = 1 To curvePoints - 1
                    If Signus * myLocalCurve.Points(ABS_ID, i) <= Signus * myLocalCurve.Points(ABS_ID, i - 1) Then
                        Monotonous = False
                        Exit For
                    End If
                Next

                If Not Monotonous Then
                    calibrator.ErrorCalibration = GlobalEnumerates.CalibrationFactorErrors.NON_MONOTONOUS_CURVE.ToString
                End If
                calibrator.Curve.Points = myLocalCurve.Points


            Catch ex As Exception
                Me.CatchLaunched("CheckCurveMonotony", ex)
                myLocalCurve.Control = 0 'NOK
                calibrator.ErrorAbs = GlobalEnumerates.CalibrationFactorErrors.SYSTEM_ERROR.ToString
            End Try
        End Sub


        ''' <summary>
        ''' Extrapolated curve in order to calculate concentration out of range
        ''' Adapted from Ax5 function (CalculoscurvaCalibracion.CalcularConcentrationExtrapolada)
        ''' </summary>
        ''' <param name="pSampleAbs"></param>
        ''' <param name="pSlope"></param>
        ''' <param name="pAbsLimit"></param>
        ''' <param name="pConcLimit"></param>
        ''' <returns>Double</returns>
        ''' <remarks>
        ''' Created by AG 15/02/2010 (Tested OK)
        ''' </remarks>
        Private Function CalculateConcentrationExtrapolated(ByVal pSlope As Single, ByVal pSampleAbs As Single, _
                                                            ByVal pAbsLimit As Single, ByVal pConcLimit As Single) As Single
            Dim ReturnValue As Single = 0
            Try
                ReturnValue = pSlope * (pSampleAbs - pAbsLimit) + pConcLimit

            Catch ex As Exception
                Me.CatchLaunched("CalculateConcentrationExtrapolated", ex)
                myLocalCurve.Control = 0 'NOK
                calibrator.ErrorAbs = GlobalEnumerates.CalibrationFactorErrors.SYSTEM_ERROR.ToString
                ReturnValue = ERROR_VALUE
            End Try
            Return ReturnValue

        End Function


        ''' <summary>
        ''' Check if the concentration is out of range:
        ''' Returns:
        '''         -2 Extrapolation NOK (Out High) 
        '''         -1 Non monotonous curve
        '''          0 OK inside the curve
        '''          1 OK extrapolation low correct
        '''          2 OK extrapolation high correct
        ''' 
        ''' NOTE: It doesn’t return “Out low” because isn’t and error, is an warning
        ''' </summary>
        ''' <param name="pCalculatedConc"></param>
        ''' <param name="pErrorType"></param>
        ''' <returns>integer with the result code</returns>
        ''' <remarks>
        ''' Created by AG 16/02/2010 (Tested OK)
        ''' Modified by AG 07/09/2010 (add pErrorType parameter)
        ''' </remarks>
        Private Function CheckIfOutOfRange(ByVal pCalculatedConc As Single, ByRef pErrorType As String) As Integer
            Dim toReturn As Integer = 0
            Try
                Dim maxCalibConc As Double = calibrator.TheoreticalConcentration(calibrator.NumberOfCalibrators - 1)
                Dim maxExtrapolationFactor As Double = common(0).ExtrapolationMaximum

                If calibrator.ErrorCalibration = GlobalEnumerates.CalibrationFactorErrors.NON_MONOTONOUS_CURVE.ToString Then
                    toReturn = -1   'Non monotonous curve
                    pErrorType = GlobalEnumerates.ConcentrationErrors.OUT.ToString  'AG 07/09/2010

                ElseIf pCalculatedConc <= 0 Then
                    toReturn = 1   'OK (extrapolation low)
                    pErrorType = GlobalEnumerates.ConcentrationErrors.OUT_LOW.ToString  'AG 07/09/2010

                ElseIf pCalculatedConc >= 0 And pCalculatedConc < maxCalibConc Then
                    toReturn = 0    'OK (inside curve)
                    pErrorType = ""  'AG 07/09/2010

                ElseIf pCalculatedConc > maxCalibConc Then
                    toReturn = 2 'OK (extrapolation high)           

                    Dim factor As Double = (CDbl(100) + maxExtrapolationFactor) / CDbl(100)
                    'AG 25/01/2012 - Out high only when toReturn = -2
                    'If pCalculatedConc > factor * maxCalibConc Then toReturn = -2 'NOK (Out high)
                    'pErrorType = GlobalEnumerates.ConcentrationErrors.OUT_HIGH.ToString  'AG 07/09/2010
                    If pCalculatedConc > factor * maxCalibConc Then
                        toReturn = -2 'NOK (Out high)
                        pErrorType = GlobalEnumerates.ConcentrationErrors.OUT_HIGH.ToString  'AG 07/09/2010
                    End If
                    'AG 25/01/2012

                End If

            Catch ex As Exception
                Me.CatchLaunched("CheckIfOutOfRange", ex)
                toReturn = 0
            End Try
            Return toReturn
        End Function

#End Region

#Region "Private saving results methods"


        ''' <summary>
        ''' SAVE VALUES IN TABLE twksWSExecutions the replicate results final abs, date, error, alarms
        ''' </summary>
        ''' <param name="pItem"></param>
        ''' <remarks>
        ''' Created by AG 02/03/2010 (Tested OK)
        ''' Modified by AG 11/08/2010 if calib multiitem save replicateid and also previous replicates!!
        ''' PRE: Connection is open
        ''' </remarks>
        Private Sub SaveReplicateResults(ByVal pDBConnection As SqlClient.SqlConnection, _
                                         ByVal pItem As Integer)

            Try
                If Not pDBConnection Is Nothing Then
                    'SAVE VALUES IN TABLE twksWSExecutions the replicate results final abs, date, error, alarms
                    '''''''''''''''
                    '1) Prepare DS                    
                    With preparation(pItem)

                        Dim ex_delegate As New ExecutionsDelegate
                        Dim ex_results As New ExecutionsDS
                        Dim ex_Row As ExecutionsDS.twksWSExecutionsRow

                        'AG 11/08/2010 - when calib multi item we have to save
                        'the current replicate and also the previous
                        Dim startReplicateLoopValue As Integer = .ReplicateID - 1
                        If preparation(pItem).SampleClass = "CALIB" And calibrator.NumberOfCalibrators > 1 Then
                            startReplicateLoopValue = 0
                        End If

                        'Add loop and replace ".ReplicateID - 1" for "myReplNum"
                        For myReplNum As Integer = startReplicateLoopValue To .ReplicateID - 1
                            ex_Row = ex_results.twksWSExecutions.NewtwksWSExecutionsRow
                            ex_Row.ExecutionID = myExecutionID(pItem)

                            'AG 11/08/2010
                            If myReplNum <> .ReplicateID - 1 Then
                                'ex_Row.ExecutionID -= calibrator.NumberOfCalibrators * (.ReplicateID - 1 - myReplNum)
                                ex_Row.ExecutionID = .ReplicateExecutionID(myReplNum) 'AG 14/08/2010
                            End If
                            'END AG 11/08/2010

                            ex_Row.ABS_Value = .ReplicateAbs(myReplNum) + BUGDECIMALS
                            If Trim$(.ErrorReplicateAbs(myReplNum)) <> "" Then ex_Row.ABS_Error = .ErrorReplicateAbs(myReplNum)
                            ex_Row.InUse = .RepInUse(myReplNum)
                            ex_Row.ResultDate = .ReplicateDate(myReplNum)
                            ex_Row.ExecutionStatus = "CLOSED" '"FINISHED"

                            'Additional information for kinetics
                            If String.Equals(test.AnalysisMode, "MRK") Or String.Equals(test.AnalysisMode, "BRK") Then
                                ex_Row.rkinetics = .rKineticsReplicate(myReplNum) + BUGDECIMALS
                                ex_Row.KineticsInitialValue = .KineticsCurve(myReplNum, 0) + BUGDECIMALS
                                ex_Row.KineticsSlope = .KineticsCurve(myReplNum, 1) + BUGDECIMALS
                                ex_Row.KineticsLinear = .KineticsLinear
                                ex_Row.SubstrateDepletion = .SubstrateDepletionReplicate(myReplNum)
                            Else
                                ex_Row.SetrkineticsNull()
                                ex_Row.SetKineticsInitialValueNull()
                                ex_Row.SetKineticsSlopeNull()
                                ex_Row.SetKineticsLinearNull()
                                ex_Row.SetSubstrateDepletionNull()
                            End If

                            Select Case .SampleClass
                                Case "BLANK"
                                    ex_Row.SetABS_InitialNull()
                                    ex_Row.SetABS_MainFilterNull()
                                    ex_Row.SetAbs_WorkReagentNull() 'AG 12/07/2010

                                    Select Case test.AnalysisMode
                                        Case "MRFT", "BRFT", "MRK", "BRK" 'Fixed time and kinetics (InitialReplicateAbs)
                                            ex_Row.ABS_Initial = .InitialReplicateAbs(myReplNum) + BUGDECIMALS

                                        Case "MREP", "BREP" 'End Point bicrhomatic (MainFilterReplicateAbs)
                                            If String.Equals(test.ReadingType, "BIC") Then    'Only bicromatic
                                                ex_Row.ABS_MainFilter = .MainFilterReplicateAbs(myReplNum) + BUGDECIMALS
                                            End If

                                            'AG 12/0/2010
                                        Case "BRDIF"
                                            ex_Row.Abs_WorkReagent = .WorkingReagentReplicateAbs(myReplNum)
                                            'END AG 12/0/2010

                                    End Select

                                Case "CALIB"
                                    If calibrator.NumberOfCalibrators = 1 Then
                                        'Nothing for replicate level
                                    Else    'NumberOfCalibrators > 1                                    
                                        'AG 11/08/2010 - use the proper data!!
                                        'ex_Row.CONC_CurveError = calibrator.Curve.ErrorCurve(pItem) + BUGDECIMALS
                                        ex_Row.CONC_CurveError = .ErrorCurveReplicate(myReplNum) + BUGDECIMALS

                                        ex_Row.CONC_Value = .ReplicateConc(myReplNum)
                                        If Trim$(.ErrorReplicateConc(myReplNum)) <> "" Then ex_Row.CONC_Error = .ErrorReplicateConc(myReplNum)
                                    End If


                                Case "CTRL", "PATIENT"
                                    ex_Row.CONC_Value = .ReplicateConc(myReplNum) + BUGDECIMALS
                                    If Trim$(.ErrorReplicateConc(myReplNum)) <> "" Then ex_Row.CONC_Error = .ErrorReplicateConc(myReplNum)

                            End Select

                            '2) Save into Database                    
                            ex_results.twksWSExecutions.AddtwksWSExecutionsRow(ex_Row)
                            'ex_results.twksWSExecutions.ImportRow(ex_Row)

                            'AG 11/08/2010
                            'myClassGlobalResult = ex_delegate.SaveExecutionsResults(pDBConnection, ex_results)
                        Next
                        myClassGlobalResult = ex_delegate.SaveExecutionsResults(pDBConnection, ex_results)
                        'END AG 11/08/2010

                    End With
                End If

            Catch ex As Exception
                Me.CatchLaunched("SaveReplicateResults", ex)
            End Try

        End Sub



        ''' <summary>
        ''' Save in Results table (twksResults) the average values (final ABS, final CONC, datetime, errors and alarms) for 
        ''' and OrderTest (whatever SampleClass) requested in the active WS
        ''' </summary>
        ''' <param name="pItem">For SampleClass BLANK, CTRL and PATIENT: always zero
        '''                     For SampleClass CALIB: zero for Single Point Calibrator (and Calibrator Number -1) for Multiple Point Calibrators</param>
        ''' <remarks>
        ''' Created by:  AG 02/03/2010 - Prerequisite: Connection is opened - (Tested OK)
        ''' Modified by: AG 12/03/2010 - Added new ByRef parameter curveResultID
        '''              AG 12/05/2010 - The status of the OrderTest changes to CLOSED when the result corresponds to the last Replicate
        '''                              requested for the OrderTest (current ReplicateNumber = MAX(ReplicateNumber) for the OrderTest)
        '''              AG 10/09/2010 - Changed the rules related with ExportStatus field
        '''              SA 20/07/2011 - When the saved Result is a rerun of a requested Control, the reset of Accepted Flag for  
        '''                              previous runs is not executed. All Results ejecuted for a Control have to be marked as 
        '''                              accepted and validated in order to sent them to QC Module when the active WS is reset 
        '''              AG 25/06/2012 - ResultsDS also informs AnalyzerID and WorkSessionID
        '''              TR 19/07/2012 - Inform field SampleClass for the result
        ''' </remarks>
        Private Sub SaveAverageResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pItem As Integer, ByRef curveResultID As Integer)
            Try
                If (Not pDBConnection Is Nothing) Then
                    'SAVE VALUES IN TABLE twksResults the orderTestID average results: final abs, date, error, alarms
                    '''''''''''''''
                    '1) Prepare DS
                    With preparation(pItem)
                        Dim myGlobal As New GlobalDataTO    'AG 10/09/2010
                        Dim res_delegate As New ResultsDelegate
                        Dim res_DS As New ResultsDS
                        Dim res_row As ResultsDS.twksResultsRow
                        res_row = res_DS.twksResults.NewtwksResultsRow

                        'Add the PK
                        res_row.OrderTestID = myOrderTestID
                        res_row.RerunNumber = myRerunNumber
                        res_row.MultiPointNumber = pItem + 1

                        'TR 19/07/2012 -Inform the sampleClass
                        res_row.SampleClass = .SampleClass

                        'Add the other fields
                        res_row.TestVersion = test.TestVersion
                        res_row.ABSValue = .Abs
                        res_row.ResultDateTime = .AverageDate
                        If Trim$(.ErrorAbs) <> "" Then res_row.ABS_Error = .ErrorAbs
                        res_row.SubstrateDepletion = .SubstrateDepletion

                        'AG 21/07/2010 - In recalculation mode do not change the accepted results
                        'res_row.AcceptedResultFlag = True
                        If Not common(pItem).RecalculationFlag Then
                            res_row.AcceptedResultFlag = True
                        Else
                            res_row.IsAcceptedResultFlagNull()
                        End If
                        'END AG 21/07/2010

                        If .ErrorAbs = "" AndAlso .ErrorConc = "" AndAlso (calibrator.ErrorCalibration = "" OrElse calibrator.ManualFactorFlag) Then
                            res_row.ValidationStatus = "OK"
                        Else
                            res_row.ValidationStatus = "NOTCALC"

                            'AG 06/11/2012 - When CALIB multipoint set the validationStatus NOTCALC only if ErrorCalibrations <> ""
                            'A possible situation could be: Curva calculated but one point with ErrorConc (out or out_high) but we have to mark ValidationStatus = OK
                            'because the curve exists
                            If .SampleClass = "CALIB" AndAlso calibrator.NumberOfCalibrators > 1 AndAlso calibrator.ErrorCalibration = "" Then
                                res_row.ValidationStatus = "OK"
                            End If
                            'AG 06/11/2012
                        End If

                        'AG 10/09/2010 - new rules
                        'AG 19/05/2010 - export and printed fields
                        res_row.ExportStatus = "NOTSENT"
                        res_row.Printed = False
                        'END AG 19/05/2010

                        'AG 10/11/2010 - Init new values
                        res_row.ManualResultFlag = False
                        res_row.SetManualResultNull()
                        res_row.SetManualResultTextNull()
                        'END AG 10/11/2010

                        'AG 10/09/2010 - new rules for ExportStatus
                        If common(0).RecalculationFlag And myManualRecalculationsFlag And preparation(0).SampleClass = "PATIENT" Then
                            myGlobal = res_delegate.RecalculateExportStatusValue(pDBConnection, myOrderTestID, myRerunNumber)
                            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                res_row.ExportStatus = CType(myGlobal.SetDatos, String)
                            End If
                        End If
                        'END AG 10/09/2010 - new rules


                        res_row.SetCurveSlopeNull()
                        res_row.SetCurveOffsetNull()
                        res_row.SetCurveCorrelationNull()

                        Select Case .SampleClass
                            Case "BLANK"
                                res_row.SetABS_InitialNull()
                                res_row.SetABS_MainFilterNull()
                                res_row.SetAbs_WorkReagentNull()    'AG 12/07/2010

                                Select Case test.AnalysisMode
                                    Case "MRFT", "BRFT", "MRK", "BRK" 'Fixed time and kinetics (InitialReplicateAbs)
                                        res_row.ABS_Initial = .InitialAbs

                                    Case "MREP", "BREP" 'End Point bicrhomatic (MainFilterReplicateAbs)
                                        If test.ReadingType = "BIC" Then    'Only bicromatic
                                            res_row.ABS_MainFilter = .MainFilterAbs
                                        End If

                                        'AG 12/0/2010
                                    Case "BRDIF"
                                        res_row.Abs_WorkReagent = .WorkingReagentAbs
                                        'END AG 12/0/2010

                                End Select

                            Case "CALIB"
                                res_row.CalibratorBlankAbsUsed = calibrator.BlankAbsUsed
                                res_row.CalibrationError = calibrator.ErrorCalibration

                                If calibrator.NumberOfCalibrators > 1 Then
                                    res_row.CurveGrowthType = calibrator.Curve.CurveGrowthType
                                    res_row.CurveType = calibrator.Curve.CurveType

                                    'AG 01/07/2011 - For LINEAR regression curves save: formula coefficients and correlation factor
                                    If String.Equals(calibrator.Curve.CurveType, "LINEAR") AndAlso String.Equals(calibrator.ErrorCalibration, String.Empty) Then
                                        res_row.CurveCorrelation = calibrator.Curve.CorrelationFactor
                                        res_row.CurveSlope = CSng(calibrator.Curve.Coefficient(1, 0))
                                        res_row.CurveOffset = calibrator.Curve.Points(ABS_ID, 0) 'AG 13/07/2011 - CSng(calibrator.Curve.Coefficient(0, 0)) -NO, the Offset is got from the curve (first point so Concentration = 0)
                                    End If
                                    'EG 01/07/2011

                                    res_row.CurveAxisXType = calibrator.Curve.CurveAxisXType
                                    res_row.CurveAxisYType = calibrator.Curve.CurveAxisYType

                                    'Concentration information about current calibrator point (item variable)
                                    res_row.CONC_Value = calibrator.Curve.ConcCurve(pItem)
                                    If Trim$(.ErrorConc) <> "" Then res_row.CONC_Error = .ErrorConc 'Preparation(Item)
                                    res_row.RelativeErrorCurve = calibrator.Curve.ErrorCurve(pItem)

                                    Dim curve_res As New CurveResultsDelegate
                                    Dim myGlobal2 As New GlobalDataTO

                                    'AG 12/03/2010
                                    'Dim curveResultID As Integer = -1  
                                    If pItem = 0 And curveResultID = -1 Then
                                        'AG 12/03/2010

                                        'Check if exists results for the ordertestid belongs to the calibration curve -NOTE: IGNORE THE VALIDATION STATUS
                                        'Find the curveresultID
                                        Dim ignoreValidationStatus As Boolean = True
                                        myGlobal2 = res_delegate.GetAcceptedResults(pDBConnection, myOrderTestID, ignoreValidationStatus, False)
                                        If Not myGlobal2.HasError And Not myGlobal2.SetDatos Is Nothing Then
                                            Dim curve2_DS As New ResultsDS
                                            curve2_DS = CType(myGlobal2.SetDatos, ResultsDS)
                                            If curve2_DS.twksResults.Rows.Count > 0 Then
                                                If Not curve2_DS.twksResults(0).IsCurveResultsIDNull Then curveResultID = curve2_DS.twksResults(0).CurveResultsID
                                            End If
                                        End If
                                    End If  'AG 12/03/2010

                                    'AG 09/06/2010 - If ABS_ERROR or SYSTEM_ERROR then delete curve
                                    If calibrator.ErrorCalibration = GlobalEnumerates.CalibrationFactorErrors.ABS_ERROR.ToString Or calibrator.ErrorCalibration = GlobalEnumerates.CalibrationFactorErrors.SYSTEM_ERROR.ToString Then
                                        If curveResultID <> -1 Then
                                            'Mark CurveResultID as NULL for OrderTestId, RerunNumber
                                            myGlobal2 = res_delegate.ClearCurveResultsID(pDBConnection, myOrderTestID, myRerunNumber)

                                            'Delete Curve
                                            If Not myGlobal2.HasError Then
                                                myGlobal2 = curve_res.DeleteCurve(pDBConnection, curveResultID)
                                            End If
                                        End If

                                    Else 'No error in curve
                                        'END AG 09/06/2010

                                        'If no curve found ... then find next curveID
                                        If pItem = 0 And curveResultID = -1 Then
                                            myGlobal2 = curve_res.FindNextID(pDBConnection)
                                            If Not myGlobal2.HasError And Not myGlobal2.SetDatos Is Nothing Then
                                                curveResultID = CType(myGlobal2.SetDatos, Integer)
                                            Else
                                                myClassGlobalResult = myGlobal2
                                                Exit Try
                                            End If
                                            If curveResultID = -1 Then Exit Try 'Protection case
                                        End If

                                        'Save the curve in twksCurveResults before saving the average results in twksResults only one time
                                        If pItem = 0 Then
                                            'Save the Calibrator curve (points)
                                            Dim curveDS As New CurveResultsDS
                                            For i As Integer = 0 To UBound(calibrator.Curve.Points, 2)
                                                Dim curvepointrow As CurveResultsDS.twksCurveResultsRow
                                                curvepointrow = curveDS.twksCurveResults.NewtwksCurveResultsRow
                                                curvepointrow.CurveResultsID = curveResultID
                                                curvepointrow.CurvePoint = i
                                                curvepointrow.ABSValue = calibrator.Curve.Points(ABS_ID, i)
                                                curvepointrow.CONCValue = calibrator.Curve.Points(CONC_ID, i)
                                                curveDS.twksCurveResults.AddtwksCurveResultsRow(curvepointrow)
                                            Next i
                                            myGlobal2 = curve_res.SaveResults(pDBConnection, curveDS)
                                            If myGlobal2.HasError Then
                                                myClassGlobalResult = myGlobal2
                                                Exit Try
                                            End If
                                        End If 'If pItem = 0 Then

                                    End If 'AG 09/06/2010

                                    res_row.CurveResultsID = curveResultID
                                    'END save the curve

                                ElseIf calibrator.NumberOfCalibrators = 1 Then
                                    'Calibrator average results information 
                                    res_row.CalibratorFactor = calibrator.Factor
                                    If Trim$(calibrator.ErrorCalibration) <> "" Then res_row.CalibrationError = calibrator.ErrorCalibration
                                End If

                                'AG 10/09/2010 - update ManualFactorFlag & ManualFactor only when calibrator
                                If calibrator.ManualFactorFlag Then
                                    res_row.ManualResultFlag = True
                                    res_row.ManualResult = calibrator.ManualFactor
                                End If
                                'END AG 10/09/2010


                            Case "CTRL", "PATIENT"
                                res_row.CONC_Value = .Conc
                                If Trim$(.ErrorConc) <> "" Then res_row.CONC_Error = .ErrorConc
                        End Select

                        '2) Save into Database
                        res_row.TS_DateTime = Now
                        res_row.AnalyzerID = myAnalyzerID
                        res_row.WorkSessionID = myWorkSessionID
                        res_DS.twksResults.AddtwksResultsRow(res_row)

                        'Dim myGlobal As New GlobalDataTO    'AG 10/09/2010
                        myGlobal = res_delegate.SaveResults(pDBConnection, res_DS)
                        If myGlobal.HasError Then
                            myClassGlobalResult = myGlobal
                            Exit Try
                        End If

                        '3) Mark all other rerun as NOT ACCEPTED

                        'AG 21/07/2010 - In recalculation mode do not change the accepted results
                        'myGlobal = res_delegate.ResetAcceptedResultFlag(pDBConnection, myOrderTestID, myRerunNumber)
                        If Not common(pItem).RecalculationFlag Then
                            'For CONTROLS, results for all requested Reruns are accepted
                            If (preparation(pItem).SampleClass <> "CTRL") Then
                                myGlobal = res_delegate.ResetAcceptedResultFlag(pDBConnection, myOrderTestID, myRerunNumber)
                                If myGlobal.HasError Then
                                    myClassGlobalResult = myGlobal
                                    Exit Try
                                End If
                            End If

                            ''4) Update OrderTestStatus to CLOSED
                            Dim ot_delegate As New OrderTestsDelegate

                            'AG 12/05/2010 - When the last ordertest replicate is calculated change ordertest status to CLOSED
                            'For multiitems ordertest update the status only one time
                            'myGlobal = ot_delegate.UpdateStatusByOrderTestId(pDBConnection, myOrderTestID, "ACCEPTED")
                            If .ReplicateID = .MaxReplicates And pItem = 0 Then
                                myGlobal = ot_delegate.UpdateStatusByOrderTestID(pDBConnection, myOrderTestID, "CLOSED")
                                If myGlobal.HasError Then
                                    myClassGlobalResult = myGlobal
                                    Exit Try
                                End If
                            End If
                            'END AG 12/05/2010
                        End If
                        'END AG 21/07/2010
                    End With
                End If
            Catch ex As Exception
                Me.CatchLaunched("SaveAverageResults", ex)
            End Try
        End Sub


        ''' <summary>
        ''' SAVE VALUES IN TABLE tcalcExecutionsPartialResults the partial absorbance results: 
        ''' </summary>
        ''' <param name="pItem"></param>
        ''' <remarks>
        ''' Created by AG 02/03/2010 (TODO, by now this method isnt used, maybe in future)
        ''' </remarks>
        Private Sub SavePartialAbsorbanceResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pItem As Integer)
            Try
                If Not pDBConnection Is Nothing Then

                    'SAVE VALUES IN TABLE tcalcExecutionsPartialResults the partial absorbance results: 
                    '(Partial Absorbances, 2on dimension is only 0 or 0..1 depending on analysis mode
                    '                     Two partial absorbances only when ReadingType = Bicrhomatic or AnalysisMode= Fixed Time or Bireagent differential)
                    '''''''''''''
                    With preparation(pItem)
                        '1) Prepare DS
                        'Define delegate, DS and fill DS rows
                        'For i As Integer = 0 To Me.GetPartialAbsorbancesNumber(test.AnalysisMode, test.ReadingType) - 1
                        '    'DataBase PartialABS_Value = .PartialReplicateAbs(.ReplicateID - 1, i)
                        '    'DataBase PartialNumber = i + 1
                        'Next

                        '2) Save into Database
                    End With
                End If


            Catch ex As Exception
                Me.CatchLaunched("SavePartialAbsorbanceResults", ex)
            End Try

        End Sub


        ''' <summary>
        ''' SAVE VALUES IN TABLE tcalcExecutionsR1Results the reagent1 results: 
        ''' </summary>
        ''' <param name="pItem"></param>
        ''' <remarks>
        ''' Created by AG 02/03/2010 (TODO, by now this method isnt used, maybe in future)
        ''' </remarks>
        Private Sub SaveReagent1Results(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pItem As Integer)
            Try
                If Not pDBConnection Is Nothing Then

                    'SAVE VALUES IN TABLE tcalcExecutionsR1Results the reagent1 results: 
                    '''''''''''''''
                    '1) Prepare DS
                    With preparation(pItem)
                        '.Reagent1ReplicateAbs(0 Or 1)
                        'the reading number is the array index 0 (1) or 1 (2)
                        '.ReplicateDate(.ReplicateID - 1)

                        '2) Save into Database
                    End With
                End If

            Catch ex As Exception
                Me.CatchLaunched("SaveReagent1Results", ex)
            End Try

        End Sub


        ''' <summary>
        ''' Get the base line values
        ''' 
        ''' May2010: Develop code for use or WSBLinesDelegate or WSBLinesByWellDelegate (by now use WSBLinesDelegate)
        ''' </summary>
        ''' <param name="pdbConnection"></param>
        ''' <param name="pBaseLineID"></param>
        ''' <param name="pWellUsed"></param>
        ''' <param name="pAdjustBaseLineID"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created AG 18/05/2010 (Tested ok)
        ''' Modified AG 03/11/2011 - add pAdjustBaseLineID
        ''' AG 29/10/2014 BA-2064 adapt for static or dynamic base lines (new parameter pType)
        ''' </remarks>
        Public Function GetBaseLineValues(ByVal pdbConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                           ByVal pWorkSessionID As String, ByVal pBaseLineID As Integer, _
                                           ByVal pWellUsed As Integer, ByVal pAdjustBaseLineID As Integer, ByVal pType As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO

            Try
                'PRE: The connection is opened!!!

                ''Option1: Using table WSBLinesDelegate (adjustment base line)
                'Dim mydelegate As New WSBLinesDelegate
                'myGlobal = mydelegate.Read(pdbConnection, pAnalyzerID, pWorkSessionID, pBaseLineID)

                ''Option2: Using table WSBLinesByWellDelegate (1 base line in every well during washing cycles)
                'Dim mydelegate2 As New WSBLinesByWellDelegate
                'myGlobal = mydelegate2.Read(pdbConnection, pAnalyzerID, pWorkSessionID, pBaseLineID, pWellUsed)

                'AG 04/01/2011 - BA-2064
                'STATIC base line: Read ligth values from twksWSBLinesByWell, read dark & adjust (TI, DAC) values from twksWSBLines
                'DYNAMIC base line: Read ligth values from twksWSBLines (DYNAMIC), read dark & adjust (TI, DAC) values from twksWSBLines (STATIC with adjust)
                Dim mydelegate As New WSBLinesDelegate
                myGlobal = mydelegate.ReadValuesForCalculations(pdbConnection, pAnalyzerID, pWorkSessionID, pBaseLineID, pWellUsed, pAdjustBaseLineID, pType)


            Catch ex As Exception
                Me.CatchLaunched("GetBaseLineValues", ex)
            End Try

            Return myGlobal
        End Function

#End Region

#Region "Validate and Auxiliary methods"

        ''' <summary>
        ''' Standard average calculation
        ''' </summary>
        ''' <param name="pValues"></param>
        ''' <param name="InUse"></param>
        ''' <param name="MaxItem"></param>
        ''' <returns>Double</returns>
        ''' <remarks>
        ''' Created by: AG 10/02/2010 (Tested OK)
        ''' </remarks>
        Private Function CalculateAverage(ByVal pValues() As Single, ByVal InUse() As Boolean, ByVal MaxItem As Integer) As Single
            Dim myReturn As Single = ERROR_VALUE
            Try
                Dim inUseCount As Integer = 0
                Dim Average As Single = 0
                For i As Integer = 0 To MaxItem - 1
                    If pValues(i) <> ERROR_VALUE And InUse(i) = True Then
                        Average += pValues(i)
                        inUseCount += 1
                    End If
                Next

                If inUseCount > 0 Then
                    myReturn = Average / CSng(inUseCount)
                End If

            Catch ex As Exception
                Me.CatchLaunched("CalculateAverage", ex)
            End Try
            Return myReturn
        End Function


        ''' <summary>
        ''' Validate the number of counts (BL > Sample > DC)
        ''' </summary>
        ''' <returns>Global data to with dataset as boolean depending the data is or not correct</returns>
        ''' <remarks>
        ''' Created by AG 08/02/2010  (Tested: OK)
        ''' Modified by AG 03/06/2010 (Tested OK) - Add checks Saturated and Error reading
        ''' </remarks>
        Private Function ValidateCounts(ByVal pBL_Counts As Integer, ByVal pDC_Counts As Integer, ByVal pSample_Counts As Integer) As GlobalDataTO
            Dim myLocal As New GlobalDataTO

            Try
                'Dim hasError As Boolean = False
                'Protections
                If pBL_Counts <= pDC_Counts Then
                    myLocal.HasError = True
                    myLocal.ErrorCode = GlobalEnumerates.AbsorbanceErrors.DC_HIGHER_BL.ToString

                    ' XBC 01/04/2011 - Real posible case, but is pending to specificate limits allowed 
                    'ElseIf pSample_Counts >= pBL_Counts Then
                    '    myLocal.HasError = True
                    '    myLocal.ErrorCode = GlobalEnumerates.AbsorbanceErrors.SAMPLE_HIGHER_BL.ToString
                    ' XBC 01/04/2011 - Real posible case, but is pending to specificate limits allowed 

                ElseIf pSample_Counts <= pDC_Counts Then
                    myLocal.HasError = True
                    myLocal.ErrorCode = GlobalEnumerates.AbsorbanceErrors.DC_HIGHER_SAMPLE.ToString
                End If

                'AG 03/06/2010
                If pSample_Counts = GlobalConstants.SATURATED_READING Or pBL_Counts = GlobalConstants.SATURATED_READING Or pDC_Counts = GlobalConstants.SATURATED_READING Then
                    myLocal.HasError = True
                    myLocal.ErrorCode = GlobalEnumerates.AbsorbanceErrors.SATURATED_READING.ToString
                End If

                If pSample_Counts = GlobalConstants.READING_ERROR Or pBL_Counts = GlobalConstants.READING_ERROR Or pDC_Counts = GlobalConstants.READING_ERROR Then
                    myLocal.HasError = True
                    myLocal.ErrorCode = GlobalEnumerates.AbsorbanceErrors.READING_ERROR.ToString
                End If
                'AG 03/06/2010

                'Update class atributte
                If myLocal.HasError Then
                    'If Not myClassGlobalResult.HasError Then myClassGlobalResult = myLocal
                    myLocal.SetDatos = False    'Incorrect
                Else
                    myLocal.SetDatos = True 'Correct
                End If


            Catch ex As Exception
                Me.CatchLaunched("ValidateCounts", ex)
                myLocal = myClassGlobalResult
            End Try
            Return myLocal
        End Function

        ''' <summary>
        ''' After init method this function validates the input water data
        ''' </summary>
        ''' <returns>Global data to with dataset as boolean depending the data is or not correct</returns>
        ''' <remarks>
        ''' Created by AG 08/02/2010  (Tested: OK)
        ''' </remarks>
        Private Function ValidateWaterData(ByVal pWater As WaterData, ByVal itemID As Integer) As GlobalDataTO
            Dim myLocal As New GlobalDataTO
            Try
                Dim hasError As Boolean = False
                Dim dataCorrect As Boolean = False
                'Protections

                dataCorrect = CType(Me.ValidateCommonData(common(itemID)).SetDatos, Boolean)
                If dataCorrect Then
                    If UBound(pWater.Readings) <> UBound(pWater.RefReadings) Then
                        hasError = True
                    End If
                End If

                If hasError Then
                    myLocal.HasError = hasError
                    myLocal.ErrorCode = GlobalEnumerates.AbsorbanceErrors.INCORRECT_DATA.ToString
                End If

                'Update class atributte
                If myLocal.HasError Then
                    If Not myClassGlobalResult.HasError Then myClassGlobalResult = myLocal
                    myLocal.SetDatos = False    'Incorrect
                Else
                    myLocal.SetDatos = True 'Correct
                End If

            Catch ex As Exception
                Me.CatchLaunched("ValidateWaterData", ex)
                myLocal = myClassGlobalResult
            End Try
            Return myLocal
        End Function


        ''' <summary>
        ''' After init method this function validates the input data
        ''' </summary>
        ''' <returns>Global data to with dataset as boolean depending the data is or not correct</returns>
        ''' <remarks>
        ''' Created by AG 08/02/2010  (Tested: OK)
        ''' </remarks>
        Private Function ValidateCommonData(ByVal pCommon As CommonData) As GlobalDataTO
            Dim myLocal As New GlobalDataTO
            Try
                Dim hasError As Boolean = False
                'Protections
                If UBound(pCommon.BaseLine) <> UBound(pCommon.RefBaseLine) Then
                    hasError = True
                ElseIf UBound(pCommon.DarkCurrent) <> UBound(pCommon.RefDarkCurrent) Then
                    hasError = True
                ElseIf UBound(pCommon.BaseLine) <> UBound(pCommon.DarkCurrent) Then
                    hasError = True
                End If

                If hasError Then
                    myLocal.HasError = hasError
                    myLocal.ErrorCode = GlobalEnumerates.AbsorbanceErrors.INCORRECT_DATA.ToString
                End If

                'Update class atributte
                If myLocal.HasError Then
                    If Not myClassGlobalResult.HasError Then myClassGlobalResult = myLocal
                    myLocal.SetDatos = False    'Incorrect
                Else
                    myLocal.SetDatos = True 'Correct
                End If

            Catch ex As Exception
                Me.CatchLaunched("ValidateCommonData", ex)
                myLocal = myClassGlobalResult
            End Try
            Return myLocal
        End Function


        ''' <summary>
        ''' Mark the current replicate absorbance as system_error
        ''' </summary>
        ''' <remarks>
        ''' Created by: AG 11/02/2010 (Tested OK)
        ''' </remarks>
        Private Sub MarkReplicateAbsSytemError()
            Try
                For i As Integer = 0 To UBound(preparation)
                    Dim repl As Integer = preparation(i).ReplicateID - 1

                    preparation(i).ReplicateAbs(repl) = ERROR_VALUE
                    preparation(i).RepInUse(repl) = False
                    preparation(i).ErrorReplicateAbs(repl) = GlobalEnumerates.AbsorbanceErrors.SYSTEM_ERROR.ToString
                    preparation(i).ReplicateDate(repl) = Now    'AG 11/03/2010
                Next

            Catch ex As Exception
                Me.CatchLaunched("MarkReplicateAbsSytemError", ex)
            End Try
        End Sub


        ''' <summary>
        ''' Mark the current sample average absorbance as system_error
        ''' </summary>
        ''' <remarks>
        ''' Created by: AG 11/02/2010 (Tested OK)
        ''' </remarks>
        Private Sub MarkAverageAbsSystemError()
            Try
                For i As Integer = 0 To UBound(preparation)
                    preparation(i).Abs = ERROR_VALUE
                    preparation(i).ErrorAbs = GlobalEnumerates.AbsorbanceErrors.SYSTEM_ERROR.ToString
                    preparation(i).AverageDate = Now    'AG 11/03/2010
                Next

            Catch ex As Exception
                Me.CatchLaunched("MarkAverageAbsSystemError", ex)
            End Try
        End Sub


        ''' 
        ''' <summary>
        ''' Log activity report
        ''' </summary>
        ''' <param name="pFunctionName"></param>
        ''' <param name="pmyEx"></param>
        ''' <remarks>
        ''' Created by: AG 08/02/2010 (Tested OK)
        ''' </remarks>
        Private Sub CatchLaunched(ByVal pFunctionName As String, ByVal pmyEx As Exception)
            Try
                myClassGlobalResult.HasError = True
                myClassGlobalResult.ErrorCode = GlobalEnumerates.AbsorbanceErrors.SYSTEM_ERROR.ToString
                myClassGlobalResult.ErrorMessage = pmyEx.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(pmyEx.Message, "Calculations." & pFunctionName.ToString, EventLogEntryType.Error, False)

            Catch ex As Exception
                myClassGlobalResult.HasError = True
                myClassGlobalResult.ErrorCode = GlobalEnumerates.AbsorbanceErrors.SYSTEM_ERROR.ToString

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Calculations.CatchLaunched", EventLogEntryType.Error, False)
            End Try
        End Sub

#End Region

#Region "Remarks generation & save"

        ''' <summary>
        ''' (Adapted from Ax5 ObtenerAvisosResultados)
        ''' Evaluate the execution calculated has results remarks
        ''' -	Evaluate for the current execution (replicate)
        ''' -	Evaluate for the order test result owner of the current execution (average)
        ''' 
        ''' Evaluation can do:
        ''' -	Nothing or Add/Delete new remarks
        ''' 
        ''' The tables twksWSExecutionAlarms and twksResultAlarms are updated
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by AG 03/06/2010 (tested OK)
        ''' </remarks>
        Private Function GenerateResultsRemarks(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobal = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobal.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        'Business Method
                        Dim myExecutionRemarksDS As New WSExecutionAlarmsDS
                        Dim myAverageRemarksDS As New ResultAlarmsDS

                        myGlobal = Me.GenerateAbsorbanceRemarks(myExecutionRemarksDS, myAverageRemarksDS)
                        If Not myGlobal.HasError Then
                            Select Case preparation(0).SampleClass
                                Case "BLANK"
                                    myGlobal = Me.GenerateBlankRemarks(myExecutionRemarksDS, myAverageRemarksDS)

                                Case "CALIB"
                                    myGlobal = Me.GenerateCalibrationRemarks(myExecutionRemarksDS, myAverageRemarksDS)

                                Case "CTRL", "PATIENT"
                                    myGlobal = Me.GenerateConcentrationRemarks(dbConnection, myExecutionRemarksDS, myAverageRemarksDS)

                                Case Else
                            End Select
                        End If

                        'Finally if no error save results Remarks
                        If Not myGlobal.HasError Then
                            myGlobal = Me.SaveResultsRemarks(dbConnection, myExecutionRemarksDS, myAverageRemarksDS)
                        End If

                        If (Not myGlobal.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                Me.CatchLaunched("GenerateResultsRemarks", ex)
                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection) 'AG 10/05/2010
            End Try
            Return (myGlobal)
        End Function


        ''' <summary>
        ''' (Adapted from Ax5 ObtenerAvisosResultadosAbsorbancia)
        ''' Evaluate the remarks for absorbances results (replicates and average)
        ''' 
        ''' Evaluation can do:
        ''' -	Nothing or Add/Delete new remarks
        ''' 
        ''' The ExecutionAlarmsDS and ResultAvgAlarmsDS are updated with new alarms
        ''' </summary>
        ''' <param name="pExecutionAlarmsDS"></param>
        ''' <param name="pAverageAlarmsDS"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by AG 03/06/2010 (basic testing OK / complete testing Pending)
        ''' Modified by AG 22/03/2011 - add Thermo warning and Possible clot remarks
        ''' </remarks>
        Private Function GenerateAbsorbanceRemarks(ByRef pExecutionAlarmsDS As WSExecutionAlarmsDS, ByRef pAverageAlarmsDS As ResultAlarmsDS) As GlobalDataTO
            'Dim myGlobal As New GlobalDataTO

            Try
                Dim ReplID As Integer = 0

                '1.- Check if absorbance is higher than absorbance optical limit [(Replicate remark]
                Dim exitFor As Boolean = False

                'AG 08/02/2012 - define signus
                Dim sign As Integer = 1
                If test.ReactionType = "DEC" Then sign = -1
                'AG 08/02/2012 

                For item As Integer = 0 To UBound(preparation)
                    ReplID = preparation(item).ReplicateID - 1

                    If preparation(item).ReplicateAbs(ReplID) = ERROR_VALUE And preparation(item).ErrorReplicateAbs(ReplID) = GlobalEnumerates.AbsorbanceErrors.ABS_LIMIT.ToString Then
                        exitFor = True

                        'Abs > Optical Limit
                        Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.ABS_REMARK1.ToString)
                    End If
                    If exitFor Then Exit For

                    '2.- For End Point bichromatic check if partial absorbances are higher than optical limit
                    '(Replicate remark)
                    If (test.AnalysisMode = "MREP" Or test.AnalysisMode = "BREP") And test.ReadingType = "BIC" Then
                        For j As Integer = 0 To 1
                            If preparation(item).PartialReplicateAbs(ReplID, j) = ERROR_VALUE And preparation(item).ErrorReplicateAbs(ReplID) = GlobalEnumerates.AbsorbanceErrors.ABS_LIMIT.ToString Then
                                exitFor = True
                                'Abs > Optical Limit
                                Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.ABS_REMARK1.ToString)
                            End If
                            If exitFor Then Exit For
                        Next
                        If exitFor Then Exit For

                    End If

                    '3.- Only for TESTs that arent absorbance test and not for blanks (Sample Abs > Blank Abs) - [(Replicate & average remarks]
                    If Not test.AbsorbanceFlag And preparation(item).SampleClass <> "BLANK" Then
                        If calibrator.NumberOfCalibrators > 1 And calibrator.Curve.CurveGrowthType = "DEC" Then
                            If preparation(item).ReplicateAbs(ReplID) > test.BlankAbs Then
                                'Sample Abs > Blank Abs (when curve is decrease)
                                Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.ABS_REMARK3.ToString)
                            End If

                            'If preparation(item).Abs > test.BlankAbs Then
                            If preparation(item).Abs > test.BlankAbs AndAlso preparation(item).Abs <> ERROR_VALUE Then 'AG 07/11/2012 - when error value not inform this remark
                                'Sample Abs > Blank Abs (when curve is decrease)
                                Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.ABS_REMARK3.ToString)
                            End If

                        Else
                            If preparation(item).ReplicateAbs(ReplID) < test.BlankAbs Then
                                'Sample Abs < Blank Abs
                                Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.ABS_REMARK2.ToString)
                            End If

                            'If preparation(item).Abs < test.BlankAbs   Then
                            If preparation(item).Abs < test.BlankAbs AndAlso preparation(item).Abs <> ERROR_VALUE Then 'AG 07/11/2012 when error value not inform this remark
                                'Sample Abs < Blank Abs
                                Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.ABS_REMARK2.ToString)
                            End If

                        End If
                    End If

                    '4. (and 6.-) Only for kinetics (Kinetics non linear [Average remark]), (Absorbance increase < 0 [Replicate remark])
                    If test.AnalysisMode = "MRK" Or test.AnalysisMode = "BRK" Then
                        If Not preparation(item).KineticsLinear Then
                            'Non Linear kinetics
                            Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.ABS_REMARK4.ToString)
                        End If

                        'AG 08/02/2012 - apply sign because it has been removed when KineticsCurve(ReplID, 1) value is asigned
                        'If preparation(item).KineticsCurve(ReplID, 1) < 0 Then
                        If sign * preparation(item).KineticsCurve(ReplID, 1) < 0 Then
                            'Absorbance increase < 0
                            Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.ABS_REMARK6.ToString)
                        End If
                    End If

                    '5.- Only for differential or fixed time [Replicate remark]
                    If test.AnalysisMode = "BRDIF" Or test.AnalysisMode = "MRFT" Or test.AnalysisMode = "BRFT" Then
                        If preparation(item).ReplicateAbs(ReplID) < 0 Then
                            'Absorbance < 0
                            Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.ABS_REMARK5.ToString)
                        End If
                    End If

                    '7.- Substrate depletion [replicate and average remark]
                    If preparation(item).SubstrateDepletionReplicate(ReplID) = True Then
                        'Substrate depletion sample
                        Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.ABS_REMARK7.ToString)
                    End If

                    If preparation(item).SubstrateDepletion > 0 Then
                        'Substrate depletion sample
                        Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.ABS_REMARK7.ToString)
                    End If

                    '8.- Prozone effect
                    If preparation(item).SampleClass = "PATIENT" Then
                        'Prozone effect remarks calculation
                        Me.GenerateProzoneRemarks(pExecutionAlarmsDS, pAverageAlarmsDS)
                    End If

                    '9.- Thermo Warning (AG 22/03/2011)
                    If preparation(item).ThermoWarningFlag Then
                        'Replicate level remark
                        Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.ABS_REMARK9.ToString)

                        'Average level remark
                        Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.ABS_REMARK9.ToString)
                    End If

                    '10.- Possible clot(AG 22/03/2011)
                    If preparation(item).PossibleClot = "CP" Then 'Possible clot
                        'Replicate level remark
                        Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.ABS_REMARK10.ToString)

                        'Average level remark
                        Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.ABS_REMARK10.ToString)

                    ElseIf preparation(item).PossibleClot = "CD" Then 'Clot detected
                        'Replicate level remark
                        Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.ABS_REMARK11.ToString)

                        'Average level remark
                        Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.ABS_REMARK11.ToString)

                    ElseIf preparation(item).PossibleClot = "BS" Then 'Possible clot
                        'Replicate level remark
                        Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.ABS_REMARK12.ToString)

                        'Average level remark
                        Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.ABS_REMARK12.ToString)

                    End If


                Next

            Catch ex As Exception
                Me.CatchLaunched("GenerateAbsorbanceRemarks", ex)
            End Try
            Return (myClassGlobalResult)
        End Function


        ''' <summary>
        ''' (Adapted from Ax5 ObtenerAvisosResultadosBlanco)
        ''' Evaluate the remarks for blank results (replicates and average)
        ''' 
        ''' Evaluation can do:
        ''' -	Nothing or Add/Delete new remarks
        ''' 
        ''' The ExecutionAlarmsDS and ResultAvgAlarmsDS are updated with new alarms
        ''' </summary>
        ''' <param name="pExecutionAlarmsDS"></param>
        ''' <param name="pAverageAlarmsDS"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' ''' Created by AG 03/06/2010 (basic testing OK / complete testing Pending)
        ''' </remarks>
        Private Function GenerateBlankRemarks(ByRef pExecutionAlarmsDS As WSExecutionAlarmsDS, ByRef pAverageAlarmsDS As ResultAlarmsDS) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO

            Try
                Dim item As Integer = 0
                Dim ReplID As Integer = 0
                ReplID = preparation(item).ReplicateID - 1

                '1.- Check if the main absorbance is higher than blank absorbance limit 
                If test.BlankAbsorbanceLimit.InUse Then
                    '1.1.- (AnalysisMode is end point and monochromatic) [(Replicate remark]
                    If (test.AnalysisMode = "MREP" Or test.AnalysisMode = "BREP") And test.ReadingType = "MONO" Then
                        'Reaction increase
                        If test.ReactionType = "INC" Then
                            If preparation(item).PartialReplicateAbs(ReplID, 0) > test.BlankAbsorbanceLimit.Value Then
                                'Main Abs > Blank Abs Limit
                                Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.BLANK_REMARK1.ToString)
                            End If

                        Else 'Reaction decrease
                            If preparation(item).PartialReplicateAbs(ReplID, 0) < test.BlankAbsorbanceLimit.Value Then
                                'Main Abs < Blank Abs Limit
                                Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.BLANK_REMARK2.ToString)
                            End If

                        End If
                    End If

                    '1.2.- (AnalysisMode is end point and bichromatic) [(Replicate remark]
                    If (test.AnalysisMode = "MREP" Or test.AnalysisMode = "BREP") And test.ReadingType = "BIC" Then
                        'Reaction increase
                        If test.ReactionType = "INC" Then
                            If preparation(item).PartialReplicateAbs(ReplID, 1) > test.BlankAbsorbanceLimit.Value Then
                                'Main Abs > Blank Abs Limit
                                Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.BLANK_REMARK1.ToString)
                            End If

                        Else 'Reaction decrease
                            If preparation(item).PartialReplicateAbs(ReplID, 1) < test.BlankAbsorbanceLimit.Value Then
                                'Main Abs < Blank Abs Limit
                                Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.BLANK_REMARK2.ToString)
                            End If

                        End If
                    End If

                    '1.3.- (AnalysisMde is bireagent differential and reaction type is increase) [(Replicate and average remark]
                    If test.AnalysisMode = "BRDIF" And test.ReactionType = "INC" Then
                        'AG 12/07/2010
                        'If preparation(item).PartialReplicateAbs(ReplID, 1) > test.BlankAbsorbanceLimit.Value Then
                        If preparation(item).WorkingReagentReplicateAbs(ReplID) > test.BlankAbsorbanceLimit.Value Then
                            'END AG 12/07/2010
                            'Abs Work Reagent > Blank Abs Limit
                            Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.BLANK_REMARK3.ToString)
                        End If

                        'AG 26/07/2010
                        If preparation(item).WorkingReagentAbs > test.BlankAbsorbanceLimit.Value Then
                            'Abs Work Reagent > Blank Abs Limit
                            Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.BLANK_REMARK3.ToString)
                        End If
                        'END AG 26/07/2010
                    End If

                    '1.4.- (AnalysisMde is bireagent differential and reaction type is decrease) [(Replicate and average remark]
                    If test.AnalysisMode = "BRDIF" And test.ReactionType = "DEC" Then
                        'AG 12/07/2010
                        'If preparation(item).PartialReplicateAbs(ReplID, 1) < test.BlankAbsorbanceLimit.Value Then
                        If preparation(item).WorkingReagentReplicateAbs(ReplID) < test.BlankAbsorbanceLimit.Value Then
                            'END AG 12/07/2010
                            'Abs Work Reagent < Blank Abs Limit
                            Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.BLANK_REMARK4.ToString)
                        End If

                        'AG 26/07/2010
                        If preparation(item).WorkingReagentAbs < test.BlankAbsorbanceLimit.Value Then
                            'Abs Work Reagent < Blank Abs Limit
                            Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.BLANK_REMARK4.ToString)
                        End If
                        'END AG 26/07/2010
                    End If

                    '1.5.- (Analysismode is kinetics or fixed time) [Replicate and average remarks]
                    If test.AnalysisMode = "MRK" Or test.AnalysisMode = "BRK" Or test.AnalysisMode = "MRFT" Or test.AnalysisMode = "BRFT" Then
                        If test.ReactionType = "INC" Then   'Increase
                            If preparation(item).InitialReplicateAbs(ReplID) > test.BlankAbsorbanceLimit.Value Then
                                'Blank Abs Initial > Blank Abs Limit
                                Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.BLANK_REMARK5.ToString)
                            End If

                            If preparation(item).InitialAbs > test.BlankAbsorbanceLimit.Value Then
                                'Blank Abs Initial > Blank Abs Limit
                                Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.BLANK_REMARK5.ToString)
                            End If

                        Else    'Decrease
                            If preparation(item).InitialReplicateAbs(ReplID) < test.BlankAbsorbanceLimit.Value Then
                                'Blank Abs Initial < Blank Abs Limit
                                Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.BLANK_REMARK6.ToString)
                            End If

                            If preparation(item).InitialAbs < test.BlankAbsorbanceLimit.Value Then
                                'Blank Abs Initial < Blank Abs Limit
                                Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.BLANK_REMARK6.ToString)
                            End If
                        End If
                    End If

                End If 'If test.BlankAbsorbanceLimit.InUse Then


                '2.- Check if the kinetic blank is higher than kinetic blank limit [Replicate remark]
                If test.KineticBlankLimit.InUse Then
                    If test.AnalysisMode = "MRK" Or test.AnalysisMode = "BRK" Then
                        If preparation(item).KineticsCurve(ReplID, 1) > test.KineticBlankLimit.Value Then
                            'Kinetic Blank > Linetic blank Limit
                            Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.BLANK_REMARK7.ToString)
                        End If
                        Dim sign As Integer = 1
                        If test.ReactionType = "DEC" Then sign = -1
                        If sign * preparation(item).ReplicateAbs(ReplID) > test.KineticBlankLimit.Value Then
                            '(Abs T2 - Abs T1)*RT > Kinetic Blank Limit
                            Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.BLANK_REMARK8.ToString)
                        End If
                    End If

                    'AG 04/11/2011 - Forgotten remark - only for replicates
                    If test.AnalysisMode = "MRFT" Then
                        Dim sign As Integer = 1
                        If test.ReactionType = "DEC" Then sign = -1
                        Dim partialAbs1 As Single = preparation(item).PartialReplicateAbs(ReplID, 0)
                        Dim partialAbs2 As Single = preparation(item).PartialReplicateAbs(ReplID, 1)
                        If test.ReadingType = "BIC" Then
                            partialAbs2 = preparation(item).PartialReplicateAbs(ReplID, 3) - preparation(item).PartialReplicateAbs(ReplID, 2)
                            partialAbs1 = preparation(item).PartialReplicateAbs(ReplID, 1) - preparation(item).PartialReplicateAbs(ReplID, 0)
                        End If

                        If (partialAbs2 - partialAbs1) * sign > test.KineticBlankLimit.Value Then
                            '(Abs T2 - Abs T1)*RT > Kinetic Blank Limit
                            Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.BLANK_REMARK8.ToString)
                        End If
                    End If
                    'AG 04/11/2011
                End If


            Catch ex As Exception
                Me.CatchLaunched("GenerateBlankRemarks", ex)
            End Try
            Return (myClassGlobalResult)
        End Function


        ''' <summary>
        ''' (Adapted from Ax5 ObtenerAvisosResultadosCalibrador)
        ''' Evaluate the remarks for calibration results (replicates and average)
        ''' 
        ''' Evaluation can do:
        ''' -	Nothing or Add/Delete new remarks
        ''' 
        ''' The ExecutionAlarmsDS and ResultAvgAlarmsDS are updated with new alarms
        ''' </summary>
        ''' <param name="pExecutionAlarmsDS"></param>
        ''' <param name="pAverageAlarmsDS"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by AG 03/06/2010 (basic testing OK / complete testing Pending)
        ''' Modified by AG 08/11/2010 new remark Calibrator lot expired
        ''' </remarks>
        Private Function GenerateCalibrationRemarks(ByRef pExecutionAlarmsDS As WSExecutionAlarmsDS, ByRef pAverageAlarmsDS As ResultAlarmsDS) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO

            Try
                Dim item As Integer = 0
                '1.- Incorrect curve [Average remark]
                If calibrator.NumberOfCalibrators > 1 Then
                    If calibrator.ErrorCalibration <> "" Then
                        For item = 0 To UBound(preparation)
                            'Incorrect curve
                            Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.CALIB_REMARK1.ToString)
                        Next

                        'AG 14/08/2010 - Show replicate remarks when multi calibrator
                    Else
                        'AG 31/08/2010
                        Dim sign As Integer = 1
                        If Not calibrator.Curve.CurveGrowthType = "INC" Then
                            sign = -1
                        End If

                        For calibID As Integer = 0 To calibrator.NumberOfCalibrators - 1
                            For replID As Integer = 0 To preparation(calibID).ReplicateID - 1
                                If preparation(calibID).ErrorReplicateConc(replID) = GlobalEnumerates.ConcentrationErrors.OUT_HIGH.ToString Then
                                    'Conc out of calibration curve (HIGH)
                                    Me.AddExecutionAlarm(pExecutionAlarmsDS, preparation(calibID).ReplicateExecutionID(replID), GlobalEnumerates.CalculationRemarks.CONC_REMARK2.ToString)
                                ElseIf sign * preparation(calibID).ReplicateAbs(replID) < sign * test.BlankAbs Then
                                    'Conc out of calibration curve (LOW)
                                    Me.AddExecutionAlarm(pExecutionAlarmsDS, preparation(calibID).ReplicateExecutionID(replID), GlobalEnumerates.CalculationRemarks.CONC_REMARK3.ToString)
                                End If
                            Next
                        Next
                        'END AG 14/08/2010

                    End If

                Else    'Calibrator is single item

                    '2.- Calculated factor is beyond limits [Average remark]
                    If test.FactorLimit.InUse And calibrator.Factor <> ERROR_VALUE Then
                        If calibrator.Factor < test.FactorLimit.Minimum Or calibrator.Factor > test.FactorLimit.Maximum Then
                            'Calculated factor is beyond limits
                            Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.CALIB_REMARK2.ToString)
                        End If
                    End If

                    '3.- Calculated factor NOT calculated [Average remark]
                    If calibrator.Factor = ERROR_VALUE Then
                        'Calculated factor NOT calculated
                        Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.CALIB_REMARK3.ToString)
                    End If

                End If

                'AG 08/11/2010 - Calibration expiration date [Average remark]
                If Not calibrator.ManualFactorFlag And calibrator.ExpirationDate < Now Then
                    Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.CALIB_REMARK4.ToString)
                End If
                'END AG 08/11/2010

            Catch ex As Exception
                Me.CatchLaunched("GenerateCalibrationRemarks", ex)
            End Try
            Return (myClassGlobalResult)
        End Function

        ''' <summary>
        ''' (Adapted from Ax5 ObtenerAvisosResultadosCONC)
        ''' Evaluate the remarks for concentration results (replicates and average)
        ''' 
        ''' Evaluation can do:
        ''' -	Nothing or Add/Delete new remarks
        ''' 
        ''' The ExecutionAlarmsDS and ResultAvgAlarmsDS are updated with new alarms
        ''' </summary>
        ''' <param name="pExecutionAlarmsDS"></param>
        ''' <param name="pAverageAlarmsDS"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by AG 03/06/2010 (basic testing OK / complete testing Pending)
        ''' Modified by AG 13/09/2010 (if conc not calculated dont add the 'Conc lower than 0' remark)
        ''' Modified by AG 08/11/2010 - new remark Calibrator lot expired
        ''' </remarks>
        Private Function GenerateConcentrationRemarks(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                      ByRef pExecutionAlarmsDS As WSExecutionAlarmsDS, ByRef pAverageAlarmsDS As ResultAlarmsDS) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                Dim item As Integer = 0
                Dim ReplID As Integer = 0
                ReplID = preparation(item).ReplicateID - 1

                myGlobal = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobal.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        '1.- Conc NOT calculated [Replicate & Average remark]
                        '(if test calibrates with Curve use Incorrect Curve)
                        If test.ErrorBlankAbs <> "" Or calibrator.ErrorAbs <> "" Then
                            If calibrator.NumberOfCalibrators = 1 Then
                                'Conc NOT calculated
                                Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.CONC_REMARK1.ToString)
                                Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.CONC_REMARK1.ToString)

                                'AG 15/09/2010
                                'Else
                            ElseIf calibrator.NumberOfCalibrators > 1 Then
                                'END AG 15/09/2010
                                If test.ErrorBlankAbs = "" And calibrator.ErrorCalibration <> "" Then
                                    'Incorrect curve
                                    Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.CALIB_REMARK1.ToString)
                                    Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.CALIB_REMARK1.ToString)
                                End If
                            End If

                            'AG 02/08/2010 - Check if conc has been calculated
                        Else
                            If preparation(item).ErrorReplicateConc(ReplID) <> "" Then
                                'Conc not Calculated
                                Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.CONC_REMARK1.ToString)
                            End If
                            If preparation(item).ErrorConc <> "" Then
                                'Conc not Calculated
                                Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.CONC_REMARK1.ToString)
                            End If
                            'END AG 02/08/2010

                        End If

                        '2.- Conc out of calibration curve (HIGH) [Replicate & Average remark]
                        If calibrator.NumberOfCalibrators > 1 Then
                            If preparation(item).ErrorReplicateConc(ReplID) = GlobalEnumerates.ConcentrationErrors.OUT_HIGH.ToString Then
                                'Conc out of calibration curve (HIGH)
                                Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.CONC_REMARK2.ToString)
                            End If

                            If preparation(item).ErrorConc = GlobalEnumerates.ConcentrationErrors.OUT_HIGH.ToString Then
                                'Conc out of calibration curve (HIGH)
                                Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.CONC_REMARK2.ToString)
                            End If
                        End If

                        '3 & 4.- 'Conc out of calibration curve (LOW)' or 'Conc < 0' [Replicate & Average remark]
                        'AG 31/08/2010
                        Dim sign As Integer = 1
                        If calibrator.NumberOfCalibrators > 1 Then
                            If Not calibrator.Curve.CurveGrowthType = "INC" Then
                                sign = -1
                            End If
                        End If

                        If sign * preparation(item).ReplicateAbs(ReplID) < sign * test.BlankAbs Then
                            If calibrator.NumberOfCalibrators > 1 Then
                                'Conc out of calibration curve (LOW)
                                Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.CONC_REMARK3.ToString)

                                'AG 13/09/2010 - Conc < 0 remark only if conc has been calcualted
                                'Else
                            ElseIf preparation(item).ErrorReplicateConc(ReplID) = "" Then
                                'END AG 13/09/2010

                                'Conc < 0
                                Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.CONC_REMARK4.ToString)
                            End If

                            'AG 31/08/2010
                            If sign * preparation(item).Abs < sign * test.BlankAbs Then
                                If calibrator.NumberOfCalibrators > 1 Then
                                    'Conc out of calibration curve (LOW)
                                    Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.CONC_REMARK3.ToString)

                                    'AG 13/09/2010 - Conc < 0 remark only if conc has been calcualted
                                    'Else
                                ElseIf preparation(item).ErrorConc = "" Then
                                    'END AG 13/09/2010

                                    'Conc < 0
                                    Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.CONC_REMARK4.ToString)
                                End If
                            End If
                        End If

                        '5.- 'Conc > Linearity Limit [Replicate & Average remark]
                        If test.LinearityLimit.InUse Then
                            If preparation(item).ReplicateConc(ReplID) <> ERROR_VALUE Then
                                If preparation(item).ReplicateConc(ReplID) > test.LinearityLimit.Value Then
                                    'Conc > Linearity Limit
                                    Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.CONC_REMARK5.ToString)
                                End If
                            End If
                            If preparation(item).Conc <> ERROR_VALUE Then
                                If preparation(item).Conc > test.LinearityLimit.Value Then
                                    'Conc > Linearity Limit
                                    Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.CONC_REMARK5.ToString)
                                End If
                            End If
                        End If

                        '6.- 'Conc < Detection Limit [Replicate & Average remark]
                        If test.DetectionLimit.InUse Then
                            If preparation(item).ReplicateConc(ReplID) <> ERROR_VALUE Then
                                If preparation(item).ReplicateConc(ReplID) < test.DetectionLimit.Value Then
                                    'Conc > Detection Limit
                                    Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.CONC_REMARK6.ToString)

                                    'NOTE: If exist alarm detection limit then delete Non Linear Kinetics remark!!
                                    '(Dont needed because the Non Linear Kinetics alarms is an average alarm!)¡
                                    Dim linqRemarkToDelete As New List(Of WSExecutionAlarmsDS.twksWSExecutionAlarmsRow)
                                    linqRemarkToDelete = (From a In pExecutionAlarmsDS.twksWSExecutionAlarms _
                                                               Where a.AlarmID = GlobalEnumerates.CalculationRemarks.ABS_REMARK4.ToString _
                                                               Select a).ToList()

                                    If (linqRemarkToDelete.Count > 0) Then
                                        linqRemarkToDelete.First().Delete()
                                        linqRemarkToDelete.First().AcceptChanges()
                                    End If
                                    linqRemarkToDelete = Nothing 'AG 25/02/2014 - #1521

                                End If
                            End If

                            If preparation(item).Conc <> ERROR_VALUE Then
                                If preparation(item).Conc < test.DetectionLimit.Value Then
                                    'Conc > Detection Limit
                                    Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.CONC_REMARK6.ToString)

                                    'NOTE: If exist alarm detection limit then delete Non Linear Kinetics remark!!
                                    'Delete from local DS (pAverageAlarmsDS)
                                    Dim linqRemarkToDelete As New List(Of ResultAlarmsDS.twksResultAlarmsRow)
                                    linqRemarkToDelete = (From a In pAverageAlarmsDS.twksResultAlarms _
                                                          Where String.Equals(a.AlarmID, GlobalEnumerates.CalculationRemarks.ABS_REMARK4.ToString) _
                                                          Select a).ToList()

                                    If (linqRemarkToDelete.Count > 0) Then
                                        linqRemarkToDelete.First().Delete()
                                        linqRemarkToDelete.First().AcceptChanges()
                                    End If
                                    linqRemarkToDelete = Nothing 'AG 25/02/2014 - #1521
                                End If
                            End If
                        End If

                        '7.- 'Conc out of normality range
                        If test.ReferenceRange.InUse Then
                            'Validate Normality Ranges for Replicate.
                            If preparation(item).ReplicateConc(ReplID) <> ERROR_VALUE Then
                                'If preparation(item).ReplicateConc(ReplID) < test.ReferenceRange.Minimum Or _
                                '    preparation(item).ReplicateConc(ReplID) > test.ReferenceRange.Maximum Then
                                '    'Conc out of normality range
                                '    Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.CONC_REMARK7.ToString)
                                'End If

                                'TR 06/06/2012 -New Implementation.
                                'Rep < Panic Min
                                If preparation(item).ReplicateConc(ReplID) < test.ReferenceRange.Minimum Then
                                    'Validate if panic is in use.
                                    If test.BorderLineRange.InUse Then
                                        If preparation(item).ReplicateConc(ReplID) > test.BorderLineRange.Minimum Then
                                            'Conc. Lower than Normality Min And Greater than than panic Min.
                                            Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), _
                                                                 GlobalEnumerates.CalculationRemarks.CONC_REMARK7.ToString())
                                        End If
                                    Else
                                        'Conc. Lower than Normality Min
                                        'If (preparation(item).SampleClass = "CTRL") Then  - For BT #1054
                                        '    Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), _
                                        '                     GlobalEnumerates.CalculationRemarks.CONC_CTRL_REMARK7.ToString())
                                        'Else
                                        Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), _
                                                         GlobalEnumerates.CalculationRemarks.CONC_REMARK7.ToString())
                                        'End If
                                        
                                    End If
                                    'Rep > Panic Max
                                ElseIf preparation(item).ReplicateConc(ReplID) > test.ReferenceRange.Maximum Then
                                    'Validate if panic is in use.
                                    If test.BorderLineRange.InUse Then
                                        If preparation(item).ReplicateConc(ReplID) < test.BorderLineRange.Maximum Then
                                            'Conc. Lower than Normality Max And lower than than panic Min.
                                            Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), _
                                                                 GlobalEnumerates.CalculationRemarks.CONC_REMARK8.ToString())
                                        End If
                                    Else
                                        'Conc. greater than Normality Max
                                        'If (preparation(item).SampleClass = "CTRL") Then  - For BT #1054
                                        '    Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), _
                                        '                     GlobalEnumerates.CalculationRemarks.CONC_CTRL_REMARK8.ToString())
                                        'Else
                                        Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), _
                                                             GlobalEnumerates.CalculationRemarks.CONC_REMARK8.ToString)
                                        'End If
                                    End If
                                End If
                                'TR 06/06/2012 -END.
                            End If
                            'Validate Normality Ranges for Concentration.
                            If preparation(item).Conc <> ERROR_VALUE Then
                                'If preparation(item).Conc < test.ReferenceRange.Minimum Or preparation(item).Conc > test.ReferenceRange.Maximum Then
                                '    'Conc out of normality range
                                '    Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.CONC_REMARK7.ToString)
                                'End If

                                'TR 06/06/2012 -New Implementation.
                                If preparation(item).Conc < test.ReferenceRange.Minimum Then
                                    'Validate if panic is in use
                                    If test.BorderLineRange.InUse Then
                                        If preparation(item).Conc > test.BorderLineRange.Minimum Then
                                            'Conc out of normality range
                                            Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, _
                                                              item + 1, GlobalEnumerates.CalculationRemarks.CONC_REMARK7.ToString)
                                        End If
                                    Else
                                        'Conc out of normality range
                                        Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, _
                                                          item + 1, GlobalEnumerates.CalculationRemarks.CONC_REMARK7.ToString)
                                    End If

                                ElseIf preparation(item).Conc > test.ReferenceRange.Maximum Then
                                    If test.BorderLineRange.InUse Then
                                        If preparation(item).Conc < test.BorderLineRange.Minimum Then
                                            Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, _
                                                      item + 1, GlobalEnumerates.CalculationRemarks.CONC_REMARK8.ToString)
                                        End If
                                    Else
                                        Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, _
                                                      item + 1, GlobalEnumerates.CalculationRemarks.CONC_REMARK8.ToString)
                                    End If
                                End If
                                'TR 06/06/2012 -END.

                            End If
                        End If

                        '8.- 'BorderLine (H)
                        'Validate the Panic Ranges if in use
                        If test.ReferenceRange.InUse And test.BorderLineRange.InUse Then
                            If preparation(item).ReplicateConc(ReplID) <> ERROR_VALUE Then
                                'Validate Max Panic Ranges for Repl.
                                If preparation(item).ReplicateConc(ReplID) > test.BorderLineRange.Maximum Then
                                    ''H
                                    'Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.CONC_REMARK8.ToString)

                                    'TR 06/06/2012 -New Implementation.
                                    'Conc. higher than panic Max
                                    Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), _
                                                         GlobalEnumerates.CalculationRemarks.CONC_REMARK10.ToString)
                                    'TR 06/06/2012 -END.
                                End If
                            End If

                            If preparation(item).Conc <> ERROR_VALUE Then
                                'Validate Max Panic Ranges for Conc.
                                If preparation(item).Conc > test.BorderLineRange.Maximum Then
                                    'H
                                    'Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.CONC_REMARK8.ToString)
                                    'TR 06/06/2012 -New Implementation.
                                    Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, _
                                                      item + 1, GlobalEnumerates.CalculationRemarks.CONC_REMARK10.ToString)
                                    'TR 06/06/2012 -END.
                                End If
                            End If
                        End If

                        '9.- 'BorderLine (L)
                        If test.ReferenceRange.InUse And test.BorderLineRange.InUse Then
                            'Validate Min Panic Ranges for Repl.
                            If preparation(item).ReplicateConc(ReplID) <> ERROR_VALUE Then
                                'If preparation(item).ReplicateConc(ReplID) > test.ReferenceRange.Maximum And _
                                '    preparation(item).ReplicateConc(ReplID) < test.BorderLineRange.Maximum Then
                                '    'BH
                                '    Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.CONC_REMARK9.ToString)
                                'End If

                                'TR 06/06/2012 -New Implementation.
                                If preparation(item).ReplicateConc(ReplID) < test.BorderLineRange.Minimum Then
                                    Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), _
                                                         GlobalEnumerates.CalculationRemarks.CONC_REMARK9.ToString)
                                End If
                                'TR 06/06/2012 -END.
                            End If
                            'Validate Max Panic Ranges for Conc.
                            If preparation(item).Conc <> ERROR_VALUE Then
                                'If preparation(item).Conc > test.ReferenceRange.Maximum And preparation(item).Conc < test.BorderLineRange.Maximum Then
                                '    'BH
                                '    Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.CONC_REMARK9.ToString)
                                'End If

                                'TR 06/06/2012 -New Implementation.
                                If preparation(item).Conc < test.BorderLineRange.Minimum Then
                                    Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, _
                                                      item + 1, GlobalEnumerates.CalculationRemarks.CONC_REMARK9.ToString)
                                End If
                                'TR 06/06/2012 -END.
                            End If
                        End If

                        '10.- 'BorderLine (L)
                        'If test.ReferenceRange.InUse And test.BorderLineRange.InUse Then
                        '    If preparation(item).ReplicateConc(ReplID) <> ERROR_VALUE Then
                        '        If preparation(item).ReplicateConc(ReplID) < test.ReferenceRange.Minimum And _
                        '            preparation(item).ReplicateConc(ReplID) > test.BorderLineRange.Minimum Then
                        '            'L
                        '            Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.CONC_REMARK10.ToString)
                        '        End If
                        '    End If

                        '    If preparation(item).Conc <> ERROR_VALUE Then
                        '        If preparation(item).Conc < test.ReferenceRange.Minimum And preparation(item).Conc > test.BorderLineRange.Minimum Then
                        '            'L
                        '            Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.CONC_REMARK10.ToString)
                        '        End If
                        '    End If
                        'End If

                        '11.- Calibration expiration date [Average remark] AG 08/11/2010
                        'No manual result and the expiration date < calib calculation date
                        If Not calibrator.ManualFactorFlag And calibrator.ExpirationDate < calibrator.CalibratorDate Then
                            Me.AddResultAlarm(pAverageAlarmsDS, myOrderTestID, myRerunNumber, item + 1, GlobalEnumerates.CalculationRemarks.CALIB_REMARK4.ToString)
                        End If

                        'Close connection if needed
                        If (Not myGlobal.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If


            Catch ex As Exception
                Me.CatchLaunched("GenerateConcentrationRemarks", ex)
                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection) 'AG 10/05/2010
            End Try
            Return (myClassGlobalResult)
        End Function


        ''' <summary>
        ''' Generate prozone replicates remarks
        ''' 
        ''' Based on method Ax5 (Calculos.CalcularProzonaPorReplicado)
        ''' </summary>
        ''' <param name="pExecutionAlarmsDS"></param>
        ''' <param name="pAverageAlarmsDS"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by AG 11/06/2010 (basic testing OK / complete testing Pending)
        ''' </remarks>
        Private Function GenerateProzoneRemarks(ByRef pExecutionAlarmsDS As WSExecutionAlarmsDS, ByRef pAverageAlarmsDS As ResultAlarmsDS) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO

            Try
                Dim item As Integer = 0
                Dim ReplID As Integer = 0
                Dim applyProzoneAlgorithm As Boolean = True
                ReplID = preparation(item).ReplicateID - 1

                If test.Prozone.InUse Then

                    'Only apply for turbidimetry tests (Dont needed, this filter is applied in test programming screen)
                    'applyProzoneAlgorithm = False

                    'Dont apply if Replicate Concentration > Linearity Limit (we can linq the pExecutionAlarmsDS 
                    'looking for this remark because it hasnt been calculated yet!!
                    'the GenerateConcentrationRemarks is called later)
                    If test.LinearityLimit.InUse Then
                        If preparation(item).ReplicateConc(ReplID) <> ERROR_VALUE Then
                            If preparation(item).ReplicateConc(ReplID) > test.LinearityLimit.Value Then
                                'Conc > Linearity Limit
                                applyProzoneAlgorithm = False
                            End If
                        End If
                    End If

                    If applyProzoneAlgorithm Then
                        Dim prozoneCycle(1, 1) As Integer
                        Dim prozoneAbsorbance(1, 1) As Single
                        '1)CALCULATE PROZONE ABSOBANCES
                        '1.1) Get the programmed prozone times Time1 & Time2 and convert to cycles dividing by the Analyzer Cycle Machine
                        'We add 1 unity because the Ax5 method do it
                        prozoneCycle(0, 1) = test.Prozone.Cycle1 + 1
                        prozoneCycle(1, 1) = test.Prozone.Cycle2 + 1

                        '1.2) Adapt depending the reading type (monochromatic or bichromatic)
                        If String.Equals(test.ReadingType, "MONO") Then
                            prozoneCycle(0, 0) = prozoneCycle(0, 1) - 1
                            prozoneCycle(1, 0) = prozoneCycle(1, 1) - 1
                        Else
                            prozoneCycle(0, 0) = prozoneCycle(0, 1) - 2
                            prozoneCycle(1, 0) = prozoneCycle(1, 1) - 2
                        End If

                        '1.3) Calculate prozone absorbances
                        If prozoneCycle(0, 0) > 0 And prozoneCycle(1, 0) > 0 Then
                            prozoneAbsorbance(0, 0) = Me.CalculateAbsorbanceByCycleNumber(item, prozoneCycle(0, 0)) ' prozoneAbsorbance(0,0) in prozoneCycle(0,0)
                            prozoneAbsorbance(0, 1) = Me.CalculateAbsorbanceByCycleNumber(item, prozoneCycle(0, 1)) ' prozoneAbsorbance(0,1) in prozoneCycle(0,1)
                            prozoneAbsorbance(1, 0) = Me.CalculateAbsorbanceByCycleNumber(item, prozoneCycle(1, 0)) ' prozoneAbsorbance(1,0) in prozoneCycle(1,0)
                            prozoneAbsorbance(1, 1) = Me.CalculateAbsorbanceByCycleNumber(item, prozoneCycle(1, 1)) ' prozoneAbsorbance(1,1) in prozoneCycle(1,1)

                            If prozoneAbsorbance(0, 0) <> ERROR_VALUE And prozoneAbsorbance(0, 1) <> ERROR_VALUE And _
                                prozoneAbsorbance(1, 0) <> ERROR_VALUE And prozoneAbsorbance(1, 1) <> ERROR_VALUE Then

                                '2) PROZONE ALGORITHM
                                '2.1) Calculate prozone slope values
                                Dim slopeValues(1) As Single
                                slopeValues(0) = prozoneAbsorbance(0, 1) - prozoneAbsorbance(0, 0)
                                slopeValues(1) = prozoneAbsorbance(1, 1) - prozoneAbsorbance(1, 0)

                                Dim existProzone As Boolean = False
                                'If final slope <= 0 no prozone
                                If slopeValues(1) > 0 Then
                                    'If initial slope is negative change signus
                                    If slopeValues(0) < 0 Then slopeValues(0) *= -1

                                    'If (dblPendiente(0) / dblPendiente(1)) < CDbl(defTecnicaFichero.Tecnica(idTecFich).Opciones.Prozona.Porcentaje / 100) Then
                                    If CSng(slopeValues(0) / slopeValues(1)) < CSng(test.Prozone.Ratio / 100) Then
                                        existProzone = True
                                    End If
                                End If

                                If existProzone Then
                                    'Add Remark: Prozone sample possible (dilute manually and repeat)
                                    Me.AddExecutionAlarm(pExecutionAlarmsDS, myExecutionID(item), GlobalEnumerates.CalculationRemarks.ABS_REMARK8.ToString)
                                End If

                            End If 'If prozoneAbsorbance(0, 0) <> ERROR_VALUE And ...

                        End If 'If prozoneCycle(0, 0) <= 0 Or prozoneCycle(1, 0) <= 0 Then

                    End If 'If applyProzoneAlgorithm Then

                End If 'If test.Prozone.InUse Then


            Catch ex As Exception
                Me.CatchLaunched("GenerateProzoneRemarks", ex)
            End Try
            Return (myClassGlobalResult)
        End Function


        ''' <summary>
        ''' Calculate the absorbance for the reading (pReadNumber)
        ''' </summary>
        ''' <param name="pItem"></param>
        ''' <param name="pCycleNumber"></param>
        ''' <returns>Single</returns>
        ''' <remarks>Created by AG 11/06/2010 (Tested OK)</remarks>
        Private Function CalculateAbsorbanceByCycleNumber(ByVal pItem As Integer, ByVal pCycleNumber As Integer) As Single
            Dim returnValue As Single = ERROR_VALUE
            Try
                Dim mainSampleCounts As Integer = 0
                Dim refSampleCounts As Integer = 0
                Dim mainLightCounts As Integer = 0
                Dim refLightCounts As Integer = 0
                Dim mainDarkCounts As Integer = 0
                Dim refDarkCounts As Integer = 0
                Dim filterID As Integer = test.MainWaveLength - 1

                mainSampleCounts = preparation(pItem).Readings(pCycleNumber)
                refSampleCounts = preparation(pItem).RefReadings(pCycleNumber)
                mainLightCounts = common(pItem).BaseLine(filterID)
                refLightCounts = common(pItem).RefBaseLine(filterID)
                mainDarkCounts = common(pItem).DarkCurrent(filterID)
                refDarkCounts = common(pItem).RefDarkCurrent(filterID)

                Dim myGlobal As New GlobalDataTO
                myGlobal = Me.CalculateAbsorbance(mainSampleCounts, refSampleCounts, mainLightCounts, refLightCounts, mainDarkCounts, _
                                                  refDarkCounts, 0, common(pItem).PathLength, common(pItem).LimitAbs, test.AbsorbanceFlag, test.DilutionFactor)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    returnValue = DirectCast(myGlobal.SetDatos, Single)
                End If

            Catch ex As Exception
                Me.CatchLaunched("CalculateAbsorbanceByCycleNumber", ex)
            End Try
            Return (returnValue)
        End Function


        ''' <summary>
        ''' The ExecutionAlarmsDS and ResultAvgAlarmsDS are updated with new alarms
        ''' into their respective tables
        ''' 
        ''' Before add new alarms delete the current ones (replicates and average)
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pExecutionAlarmsDS"></param>
        ''' <param name="pAverageAlarmsDS"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by AG 03/06/2010 (Tested OK)
        ''' </remarks>
        Private Function SaveResultsRemarks(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pExecutionAlarmsDS As WSExecutionAlarmsDS, ByRef pAverageAlarmsDS As ResultAlarmsDS) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobal = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobal.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim myExecAlarms As New WSExecutionAlarmsDelegate
                        Dim myResultAlarms As New ResultAlarmsDelegate

                        'Delete all alarms for current Execution and for current OrderTestId-Rerun-MultiItem
                        For item As Integer = 0 To UBound(preparation)
                            'AG 14/08/2010 - Use myExecutionId is different than (CALIB and MultiItemNumber > 1)
                            'In this case use preparation(item).ReplicateExecutionID(replID)
                            'myGlobal = myExecAlarms.DeleteAll(dbConnection, myExecutionID(item))
                            If UBound(preparation) = 0 Then 'Single item
                                myGlobal = myExecAlarms.DeleteAll(dbConnection, myExecutionID(item))
                            Else 'MultiItem
                                For myReplicate As Integer = 0 To preparation(item).ReplicateID - 1
                                    myGlobal = myExecAlarms.DeleteAll(dbConnection, preparation(item).ReplicateExecutionID(myReplicate))
                                Next
                            End If
                            'END AG 14/08/2010

                            myGlobal = myResultAlarms.DeleteAll(dbConnection, myOrderTestID, myRerunNumber, item + 1)
                            If myGlobal.HasError Then Exit For
                        Next

                        'Insert the new alarms
                        If Not myGlobal.HasError Then
                            myGlobal = myExecAlarms.Add(dbConnection, pExecutionAlarmsDS)

                            If Not myGlobal.HasError Then
                                myGlobal = myResultAlarms.Add(dbConnection, pAverageAlarmsDS)
                            End If
                        End If

                        'Close connection if needed
                        If (Not myGlobal.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                Me.CatchLaunched("SaveResultsRemarks", ex)
                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection) 'AG 10/05/2010
            End Try
            Return (myGlobal)
        End Function


        ''' <summary>
        ''' Generical method: Add row into pExecutionAlarmsDS (ByRef)
        ''' </summary>
        ''' <param name="pExecutionAlarmsDS"></param>
        ''' <param name="pExecutionID"></param>
        ''' <param name="pAlarmID"></param>
        ''' <remarks>
        ''' Created by AG 07/06/2010 (Tested ok)
        ''' </remarks>
        Private Sub AddExecutionAlarm(ByRef pExecutionAlarmsDS As WSExecutionAlarmsDS, ByVal pExecutionID As Integer, ByVal pAlarmID As String)
            Try
                'Add alarm row into Dataset
                Dim execAlarmRow As WSExecutionAlarmsDS.twksWSExecutionAlarmsRow
                execAlarmRow = pExecutionAlarmsDS.twksWSExecutionAlarms.NewtwksWSExecutionAlarmsRow
                pExecutionAlarmsDS.twksWSExecutionAlarms.AddtwksWSExecutionAlarmsRow(execAlarmRow)
                With execAlarmRow
                    .BeginEdit()
                    .ExecutionID = pExecutionID
                    .AlarmID = pAlarmID
                    .AlarmDateTime = Now
                    .AcceptChanges()
                End With

            Catch ex As Exception
                Me.CatchLaunched("AddExecutionAlarm", ex)
            End Try
        End Sub


        ''' <summary>
        ''' Generical method: Add row into pResultsAlarmsDS (ByRef)
        ''' </summary>
        ''' <param name="pResultAlarmsDS"></param>
        ''' <param name="pOrderTestID"></param>
        ''' <param name="pRerunNumber"></param>
        ''' <param name="pMultiPointNumber"></param>
        ''' <param name="pAlarmID"></param>
        ''' <remarks>
        ''' Created by AG 07/06/2010 (Tested ok)
        ''' </remarks>
        Private Sub AddResultAlarm(ByRef pResultAlarmsDS As ResultAlarmsDS, ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer, ByVal pMultiPointNumber As Integer, ByVal pAlarmID As String)
            Try
                'Add alarm row into Dataset
                Dim resultAlarmRow As ResultAlarmsDS.twksResultAlarmsRow
                resultAlarmRow = pResultAlarmsDS.twksResultAlarms.NewtwksResultAlarmsRow
                pResultAlarmsDS.twksResultAlarms.AddtwksResultAlarmsRow(resultAlarmRow)
                With resultAlarmRow
                    .BeginEdit()
                    .OrderTestID = pOrderTestID
                    .RerunNumber = pRerunNumber
                    .MultiPointNumber = pMultiPointNumber
                    .AlarmID = pAlarmID
                    .AlarmDateTime = Now
                    .AcceptChanges()
                End With


            Catch ex As Exception
                Me.CatchLaunched("AddResultAlarm", ex)
            End Try
        End Sub

#End Region

#Region "Funtions TO_DELETE"

        'Private Sub InitCommonDataTEMP()
        '    Try
        '        'Initialize common data
        '        ReDim common(0)
        '        With common(0)
        '            .BaseLineID = 1
        '            .BaseLineWell = 1

        '            Dim mainBL() As Integer = {930000, 929000, 928000, 927000}
        '            Dim refBL() As Integer = {950000, 949000, 948000, 947000}
        '            Dim mainDC() As Integer = {5000, 5001, 5002, 5003}
        '            Dim refDC() As Integer = {4500, 4501, 4502, 4503}
        '            .BaseLine = mainBL
        '            .RefBaseLine = refBL
        '            .DarkCurrent = mainDC
        '            .RefDarkCurrent = refDC

        '            .RecalculationFlag = False
        '            .SampleClassRecalculated = ""

        '            .PathLength = 6.11
        '            .LimitAbs = 3.3
        '            .KineticsIncrease = 10.1
        '            .LinearCorrelationFactor = 0.95
        '            .Reagent1Readings = 0   '2
        '            .ExtrapolationMaximum = 20  '20%

        '        End With

        '    Catch ex As Exception

        '    End Try
        'End Sub

        Private Sub InitWaterDataTEMP()
            Try
                'Initialize water readings
                ReDim water(0)
                With water(0)
                    'For testing supose only 4 filters
                    Dim mainWater() As Integer = {900000, 901000, 902000, 903000}
                    Dim refWater() As Integer = {800000, 801000, 802000, 803000}

                    .Readings = mainWater
                    .RefReadings = refWater
                End With

            Catch ex As Exception

            End Try
        End Sub

        Private Sub InitTestDataTEMP()
            Try
                'Initialize test data
                With test
                    'Read the test definition and initializes fields
                    .TestID = 1
                    .SampleType = "SER"
                    .AnalysisMode = "MREP"  'BRDIF, BREP, BRFT,MREP, MRFT (Kinetics: BRK, MRK)
                    .AbsorbanceFlag = False
                    .ReactionType = "INC"
                    .ReadingType = "BIC"
                    .MainWaveLength = 1
                    .ReferenceWaveLength = 4
                    .SampleVolume = 30

                    'Reagents number depends on analysis mode
                    .ReagentsNumber = 1
                    Select Case .AnalysisMode
                        Case "BRDIF", "BREP", "BRFT", "BRK" 'Bireagent
                            .ReagentsNumber = 2
                        Case "MREP", "MRFT", "MRK"  'Monoreagent
                            .ReagentsNumber = 1
                    End Select
                    ReDim .ReagentVolume(.ReagentsNumber - 1)
                    .ReagentVolume(0) = 300
                    If .ReagentsNumber = 2 Then .ReagentVolume(1) = 50

                    .DilutionFactor = 1
                    .InitialReadingCycle = 3
                    .FinalReadingCycle = 7
                    'ReDim .ReagentDispensingCycle(.ReagentsNumber - 1)
                    '.ReagentDispensingCycle(0) = 0   'Reagent1
                    'If .ReagentsNumber = 2 Then .ReagentDispensingCycle(1) = 4 'Reagent2


                    .PredilutionFactor = 1
                    .PreDilutionFlag = False
                    .PostDilutionFactor = 1
                    '.PostDilutionType = ""  '"" or "INC" or "RED"
                    .PostSampleVolume = 15
                    ReDim .PostReagentVolume(.ReagentsNumber - 1)
                    .PostReagentVolume(0) = 300
                    If .ReagentsNumber = 2 Then .PostReagentVolume(1) = 50

                    'For 

                    'Options and remarks
                    .SubstrateDepletion.InUse = True
                    .SubstrateDepletion.Value = 0.0001

                    .Slope.InUse = True
                    .Slope.A = 1
                    .Slope.B = 5


                    'Business (internal fields modifications)
                    '''''''''
                    '1) After read the reading cycle from test programming we have to add the Reagent1Readings offset
                    '.InitialReadingCycle += common(0).Reagent1Readings
                    '.FinalReadingCycle += common(0).Reagent1Readings
                    '.ReagentDispensingCycle(0) += common(0).Reagent1Readings
                    'If .ReagentsNumber = 2 Then .ReagentDispensingCycle(1) += common(0).Reagent1Readings

                    '2) When we are calculating a CALIB or CTRL or PATIENT we need the blank results
                    .BlankAbs = 0.003
                    .InitialAbs = 0.001
                    .BlankDate = DateTime.MinValue

                    '...
                End With

            Catch ex As Exception

            End Try
        End Sub

        Private Sub InitCalibratorDataTEMP()
            Try
                'Read the test-calibrator definition and initializes fields
                'Initialize calibrator data
                With calibrator
                    .CalibratorID = 1
                    .TestID = test.TestID
                    .SampleType = "SER"
                    .NumberOfCalibrators = 1
                    .AlternativeSampleType = ""
                    .CalibrationType = "EXPERIMENT"  '"FACTOR", "EXPERIMENT"
                    .CalibratorDate = DateTime.MinValue

                    If String.Equals(.CalibrationType, "FACTOR") Then .Factor = 1997.5

                    ReDim .TheoreticalConcentration(.NumberOfCalibrators - 1)
                    For i As Integer = 0 To .NumberOfCalibrators - 1
                        .TheoreticalConcentration(i) = 100 + 50 * i
                    Next

                    If .NumberOfCalibrators > 1 Then
                        ReDim .Curve.CalibratorAbs(.NumberOfCalibrators - 1)
                    End If

                    'Business (internal fields modifications)
                    '1) When we are calculating a CTRL or PATIENT we need the calibrator results
                    ''''''''
                    .CalibratorAbs = 0.5
                    .Factor = 78

                End With

            Catch ex As Exception

            End Try
        End Sub

        Private Sub InitCalibratorDataCurveTEMP()
            Try
                'Read the test-calibrator definition and initializes fields
                'Initialize calibrator data
                With calibrator
                    .CalibratorID = 1
                    .TestID = test.TestID
                    .SampleType = "SER"
                    .NumberOfCalibrators = 3
                    .AlternativeSampleType = ""
                    .CalibrationType = "EXPERIMENT"  '"FACTOR", "EXPERIMENT"
                    .CalibratorDate = DateTime.MinValue

                    ReDim .TheoreticalConcentration(.NumberOfCalibrators - 1)
                    For i As Integer = 0 To .NumberOfCalibrators - 1
                        .TheoreticalConcentration(i) = 100 + 50 * i
                    Next

                    If .NumberOfCalibrators > 1 Then
                        ReDim .Curve.CalibratorAbs(.NumberOfCalibrators - 1)
                        ReDim .Curve.ErrorAbs(.NumberOfCalibrators - 1)
                    End If

                    ReDim .Curve.Coefficient(3, 15) 'Idem Ax5
                    .Curve.CurveType = "PARABOLREG"  'LINEARREG, PARABOLREG, POLYGONAL, SPLINE
                    .Curve.CurveGrowthType = "INC"  'INC, DEC
                    .Curve.CurveAxisXType = "LINEAR"  'LINEAR, LOG
                    .Curve.CurveAxisYType = "LINEAR"  'LINEAR, LOG
                    .Curve.CurvePointsNumber = 200
                    ReDim .Curve.Points(1, .Curve.CurvePointsNumber - 1)

                    'Business (internal fields modifications)
                    '1) When we are calculating a CTRL or PATIENT we need the calibrator results
                    'TODO for curves

                End With

            Catch ex As Exception

            End Try
        End Sub

        Private Sub InitSampleDataTEMP()
            Try
                'Read the executionID, orderTestID and OrderID information and initializes fields
                'Initialize preparation data
                ReDim preparation(0)
                With preparation(0)
                    .PreparationID = 1

                    .SampleClass = "PATIENT"
                    .SampleType = "SER"
                    .MultiItemNumber = 1
                    .MaxReplicates = 3
                    .ReplicateID = 3
                    .ReRun = 1
                    .WellUsed = 2
                    '.RotorTurn = 1 'AG 03/05/2010
                    .PostDilutionType = "NONE"  '"" or "INC" or "RED"

                    .TestID = 1
                    .ControlID = 0

                    'AG 18/02/2010 - Reagent1 readings postposed (Febraury 2010)
                    'For testing supose only 7 readings (NOTE there are 9 read because the 2 first readings are R1 only)
                    'Dim mainRead() As Integer = {120000, 140000, 100000, 101000, 105000, 106000, 108000, 110000, 120000}
                    'Dim refRead() As Integer = {80000, 87000, 90000, 91000, 95000, 96000, 98000, 99000, 98700}
                    Dim mainRead() As Integer = {100000, 101000, 105000, 106000, 108000, 110000, 120000}
                    Dim refRead() As Integer = {90000, 91000, 95000, 96000, 98000, 99000, 98700}

                    .Readings = mainRead
                    .RefReadings = refRead

                    'Redim result arrays using the max replicates
                    ReDim .PartialReplicateAbs(.MaxReplicates - 1, Me.GetPartialAbsorbancesNumber(test.AnalysisMode, test.ReadingType) - 1)

                    ReDim .InitialReplicateAbs(.MaxReplicates - 1)
                    ReDim .MainFilterReplicateAbs(.MaxReplicates - 1)
                    ReDim .ReplicateAbs(.MaxReplicates - 1)
                    ReDim .Reagent1ReplicateAbs(common(0).Reagent1Readings - 1)
                    ReDim .ReplicateConc(.MaxReplicates - 1)
                    ReDim .rKineticsReplicate(.MaxReplicates - 1)
                    ReDim .KineticsCurve(.MaxReplicates - 1, 1)
                    ReDim .SubstrateDepletionReplicate(.MaxReplicates - 1)
                    ReDim .ErrorReplicateAbs(.MaxReplicates - 1)
                    ReDim .ErrorReplicateConc(.MaxReplicates - 1)
                    ReDim .ReplicateDate(.MaxReplicates - 1)
                    ReDim .RepInUse(.MaxReplicates - 1)
                    ReDim .ReplicateAlarms(.MaxReplicates - 1)
                    ReDim .WorkingReagentReplicateAbs(.MaxReplicates - 1) 'AG 12/07/2010
                    ReDim .ReplicateExecutionID(.MaxReplicates - 1) 'AG 14/08/2010


                    'Business (internal fields modifications)
                    'Get value for previous replicates for the same OrderTestID in order to calculate averages
                    If .PostDilutionType Is Nothing Then .PostDilutionType = "NONE"

                    '1) On calculating a repetition (postdiluted execution) update the sample and reagents volumes
                    If String.Equals(.PostDilutionType, String.Empty) And test.PostDilutionFactor <> 1 Then
                        test.SampleVolume = test.PostSampleVolume
                        For i As Integer = 0 To UBound(test.ReagentVolume)
                            test.ReagentVolume(i) = test.PostReagentVolume(i)
                        Next
                    End If


                    For i = 0 To .ReplicateID - 1
                        .ReplicateAbs(i) = CSng(0.1) * (i + 1)
                        .ErrorReplicateAbs(i) = ""
                        .RepInUse(i) = True
                        .SubstrateDepletionReplicate(i) = False

                        If String.Equals(.SampleClass, "CTRL") Or String.Equals(.SampleClass, "PATIENT") Then
                            .ReplicateConc(i) = 50 + 2 * i
                            .ErrorReplicateConc(i) = ""
                        End If

                        If String.Equals(.SampleClass, "BLANK") Then
                            .InitialReplicateAbs(i) = CSng(0.002) * (i + 1)
                            .MainFilterReplicateAbs(i) = CSng(0.004) * (i + 1)
                        End If
                    Next
                End With

            Catch ex As Exception

            End Try
        End Sub

        Private Sub InitSampleDataCurveTEMP()
            Try
                'Read the executionID, orderTestID and OrderID information and initializes fields
                'Initialize preparation data
                ReDim preparation(calibrator.NumberOfCalibrators - 1)
                For i As Integer = 0 To calibrator.NumberOfCalibrators - 1
                    With preparation(i)
                        .PreparationID = 1

                        .SampleClass = "CALIB"
                        .SampleType = "SER"
                        .MultiItemNumber = i + 1
                        .MaxReplicates = 2
                        .ReplicateID = 2
                        .ReRun = 1
                        .WellUsed = 2
                        '.RotorTurn = 1  'AG 03/05/2010
                        .PostDilutionType = "NONE"  '"" or "INC" or "RED"


                        .TestID = 1
                        .ControlID = 0

                        'AG 18/02/2010 - Reagent1 readings postposed (Febraury 2010)
                        ''For testing supose only 7 readings (NOTE there are 9 read because the 2 first readings are R1 only)
                        'Dim mainRead() As Integer = {120000 - 1000 * i, 140000 - 1000 * i, 100000 - 1000 * i, 101000 - 1000 * i, 105000 - 1000 * i, 106000 - 1000 * i, 108000 - 1000 * i, 110000 - 1000 * i, 120000 - 1000 * i}
                        'Dim refRead() As Integer = {80000 - 1000 * i, 87000 - 1000 * i, 90000 - 1000 * i, 91000 - 1000 * i, 95000 - 1000 * i, 96000 - 1000 * i, 98000 - 1000 * i, 99000 - 1000 * i, 98700 - 1000 * i}
                        Dim mainRead() As Integer = {100000 - 1000 * i, 101000 - 1000 * i, 105000 - 1000 * i, 106000 - 1000 * i, 108000 - 1000 * i, 110000 - 1000 * i, 120000 - 1000 * i}
                        Dim refRead() As Integer = {90000 - 1000 * i, 91000 - 1000 * i, 95000 - 1000 * i, 96000 - 1000 * i, 98000 - 1000 * i, 99000 - 1000 * i, 98700 - 1000 * i}

                        .Readings = mainRead
                        .RefReadings = refRead

                        'Redim result arrays
                        ReDim .PartialReplicateAbs(.MaxReplicates - 1, Me.GetPartialAbsorbancesNumber(test.AnalysisMode, test.ReadingType) - 1)

                        ReDim .InitialReplicateAbs(.MaxReplicates - 1)
                        ReDim .MainFilterReplicateAbs(.MaxReplicates - 1)
                        ReDim .ReplicateAbs(.MaxReplicates - 1)
                        ReDim .Reagent1ReplicateAbs(common(0).Reagent1Readings - 1)
                        ReDim .ReplicateConc(.MaxReplicates - 1)
                        ReDim .rKineticsReplicate(.MaxReplicates - 1)
                        ReDim .KineticsCurve(.MaxReplicates - 1, 1)
                        ReDim .SubstrateDepletionReplicate(.MaxReplicates - 1)
                        ReDim .ErrorReplicateAbs(.MaxReplicates - 1)
                        ReDim .ErrorReplicateConc(.MaxReplicates - 1)
                        ReDim .ReplicateDate(.MaxReplicates - 1)
                        ReDim .RepInUse(.MaxReplicates - 1)
                        ReDim .ReplicateAlarms(.MaxReplicates - 1)
                        ReDim .ErrorCurveReplicate(.MaxReplicates - 1)
                        ReDim .WorkingReagentReplicateAbs(.MaxReplicates - 1) 'AG 12/07/2010
                        ReDim .ReplicateExecutionID(.MaxReplicates - 1) 'AG 14/08/2010

                        'Business (internal fields modifications)
                        If .PostDilutionType Is Nothing Then .PostDilutionType = "NONE"

                        '1) On calculating a repetition (postdiluted execution) update the sample and reagents volumes
                        If .PostDilutionType = "" And test.PostDilutionFactor <> 1 Then
                            test.SampleVolume = test.PostSampleVolume
                            For j As Integer = 0 To UBound(test.ReagentVolume)
                                test.ReagentVolume(j) = test.PostReagentVolume(j)
                            Next
                        End If


                        'Get value for previous replicates for the same OrderTestID in order to calculate averages
                        For j = 0 To .ReplicateID - 1
                            .ReplicateAbs(j) = CSng(0.1) * (i + 1) + CSng(0.1) * j
                            .ErrorReplicateAbs(j) = ""
                            .RepInUse(j) = True
                            .SubstrateDepletionReplicate(j) = False

                            If .SampleClass = "CTRL" Or .SampleClass = "PATIENT" Then
                                .ReplicateConc(j) = 50 + 2 * i + j
                                .ErrorReplicateConc(j) = ""
                            End If

                            If .SampleClass = "BLANK" Then
                                .InitialReplicateAbs(j) = CSng(0.002) * (i + 1)
                                .MainFilterReplicateAbs(j) = CSng(0.004) * (i + 1)
                            End If
                        Next
                    End With
                Next

                ReDim Preserve common(calibrator.NumberOfCalibrators - 1)
                ReDim Preserve water(calibrator.NumberOfCalibrators - 1)
                For i As Integer = 1 To calibrator.NumberOfCalibrators - 1
                    common(i) = common(0)
                    water(i) = water(0)
                Next

                'Redim errorcurve replicate and average
                With calibrator.Curve
                    ReDim .ConcCurve(calibrator.NumberOfCalibrators - 1)
                    ReDim .ErrorCurve(calibrator.NumberOfCalibrators - 1)
                End With

            Catch ex As Exception

            End Try
        End Sub

        Private Sub InitTEMP()
            'TO DELETE
            'Initializes sample information, test information and general information needed for ExecuteCalculation
            Try
                'Me.InitCommonDataTEMP()
                Me.InitWaterDataTEMP()
                'Me.InitTestDataTEMP()

                'CALIBRATOR 1 item
                Me.InitCalibratorDataTEMP()
                Me.InitSampleDataTEMP()

                'CALIBRATOR N items
                'Me.InitCalibratorDataCurveTEMP()
                'Me.InitSampleDataCurveTEMP()


            Catch ex As Exception
                Me.CatchLaunched("InitTEMP", ex)
            End Try

        End Sub

        ''' <summary>
        ''' Initializes test information
        ''' </summary>
        ''' <param name="pdbConnection"></param>
        ''' <param name="myExecutionsDS"></param>
        ''' <remarks>
        ''' </remarks>
        ''' Created by : DL 22/02/2010
        ''' Modified by: AG 25/02/2010 (Tested OK)
        Private Sub InitTestOLD03072012(ByVal pdbConnection As SqlClient.SqlConnection, _
                             ByVal myExecutionsDS As ExecutionsDS)

            Dim resultData As New GlobalDataTO

            Try
                ' Get TestID and SampleType
                test.TestID = preparation(0).TestID
                test.SampleType = preparation(0).SampleType

                ' Get test data
                Dim myTestsData As New TestsDelegate
                Dim myTestsDS As New TestsDS

                resultData = myTestsData.Read(pdbConnection, test.TestID)
                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                    myTestsDS = CType(resultData.SetDatos, TestsDS)
                    test.TestVersion = myTestsDS.tparTests(0).TestVersionNumber
                    test.AnalysisMode = Trim$(myTestsDS.tparTests.Item(0).AnalysisMode)
                    test.AbsorbanceFlag = myTestsDS.tparTests.Item(0).AbsorbanceFlag
                    test.ReactionType = Trim$(myTestsDS.tparTests.Item(0).ReactionType)
                    test.ReadingType = Trim$(myTestsDS.tparTests.Item(0).ReadingMode)
                    test.ReagentsNumber = CInt(myTestsDS.tparTests.Item(0).ReagentsNumber)
                    test.InitialReadingCycle = myTestsDS.tparTests(0).FirstReadingCycle - 2 'AG 09/05/2011 - Test programming starts at time 18 (cycle 3): 0s - 1, 9s - 2, 18s - 3
                    '                                                                        for apply calculations InitialReadingCycle must be 1 so apply -2 offset
                    test.FinalReadingCycle = test.InitialReadingCycle
                    If Not myTestsDS.tparTests(0).IsSecondReadingCycleNull Then test.FinalReadingCycle = myTestsDS.tparTests(0).SecondReadingCycle - 2 'AG 09/05/2011 - Test programming starts at time 18 (cycle 3): 0s - 1, 9s - 2, 18s - 3
                    '                                                                                                                                    for apply calculations InitialReadingCycle must be 1 so apply -2 offset

                    ' Find position for main and reference wave lenght
                    Dim myMainWaveLength As String = myTestsDS.tparTests.Item(0).MainWavelength.ToString()
                    Dim myReferenceWaveLength As String = ""
                    If test.ReadingType = "BIC" Then myReferenceWaveLength = myTestsDS.tparTests.Item(0).ReferenceWavelength.ToString()

                    If myMainWaveLength <> "" Then
                        Dim myAnalyzerLedPositions As New AnalyzerLedPositionsDelegate
                        resultData = myAnalyzerLedPositions.GetByWaveLength(pdbConnection, myExecutionsDS.twksWSExecutions.Item(0).AnalyzerID, myMainWaveLength)
                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            Dim myAnalyzerLedPositionsDS As New AnalyzerLedPositionsDS
                            myAnalyzerLedPositionsDS = CType(resultData.SetDatos, AnalyzerLedPositionsDS)
                            test.MainWaveLength = myAnalyzerLedPositionsDS.tcfgAnalyzerLedPositions.Item(0).LedPosition
                        Else
                            myClassGlobalResult = resultData
                            Exit Try
                        End If
                        'Dim myMasterDataDS As New PreloadedMasterDataDS
                        'myMasterDataDS = myPreloadedMaster.GetSubTableItem("WAVELENGTHS", myMainWaveLength)
                        'test.MainWaveLength = myMasterDataDS.tfmwPreloadedMasterData.Item(0).Position
                    End If

                    If myReferenceWaveLength <> "" Then
                        Dim myAnalyzerLedPositions As New AnalyzerLedPositionsDelegate
                        resultData = myAnalyzerLedPositions.GetByWaveLength(pdbConnection, myExecutionsDS.twksWSExecutions.Item(0).AnalyzerID, myReferenceWaveLength)
                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            Dim myAnalyzerLedPositionsDS As New AnalyzerLedPositionsDS
                            myAnalyzerLedPositionsDS = CType(resultData.SetDatos, AnalyzerLedPositionsDS)
                            test.ReferenceWaveLength = myAnalyzerLedPositionsDS.tcfgAnalyzerLedPositions.Item(0).LedPosition
                        Else
                            myClassGlobalResult = resultData
                            Exit Try
                        End If
                    End If
                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If

                ' Get test reagent data
                Dim TestReagentsVolumeData As New TestReagentsVolumeDelegate
                resultData = TestReagentsVolumeData.GetReagentsVolumesByTestID(pdbConnection, test.TestID, test.SampleType)
                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                    Dim myTestReagentsVolumesDS As New TestReagentsVolumesDS
                    myTestReagentsVolumesDS = CType(resultData.SetDatos, TestReagentsVolumesDS)

                    ReDim test.ReagentVolume(myTestReagentsVolumesDS.tparTestReagentsVolumes.Rows.Count - 1)
                    ReDim test.PostReagentVolume(myTestReagentsVolumesDS.tparTestReagentsVolumes.Rows.Count - 1)    'AG 11/03/2010                    

                    Dim myIndex As Integer = 0

                    For Each myTestReagentsVolumeRow As TestReagentsVolumesDS.tparTestReagentsVolumesRow In myTestReagentsVolumesDS.tparTestReagentsVolumes.Rows
                        test.ReagentVolume(myIndex) = myTestReagentsVolumeRow.ReagentVolume
                        test.PostReagentVolume(myIndex) = test.ReagentVolume(myIndex)   'AG 11/03/2010 -Protection: init post volumes

                        Select Case preparation(0).PostDilutionType
                            Case "INC"
                                If Not myTestReagentsVolumeRow.IsIncPostReagentVolumeNull Then
                                    test.PostReagentVolume(myIndex) = myTestReagentsVolumeRow.IncPostReagentVolume
                                End If

                            Case "RED"
                                If Not myTestReagentsVolumeRow.IsRedPostReagentVolumeNull Then
                                    test.PostReagentVolume(myIndex) = myTestReagentsVolumeRow.RedPostReagentVolume
                                End If

                            Case "NONE"
                                test.PostDilutionFactor = 1
                        End Select

                        myIndex = myIndex + 1
                    Next myTestReagentsVolumeRow
                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If

                ' Get test sample data
                Dim myTestSamplesDelegate As New TestSamplesDelegate
                Dim myTestSample As TestSamplesDS

                'AG 11/06/2010 - Read by TestID & SampleType
                'resultData = myTestSamplesDelegate.GetSampleDataByTestID(pdbConnection, test.TestID)
                resultData = myTestSamplesDelegate.GetDefinition(pdbConnection, test.TestID, test.SampleType)

                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                    myTestSample = CType(resultData.SetDatos, TestSamplesDS)

                    If Not myTestSample.tparTestSamples.Item(0).IsSampleVolumeNull Then
                        test.SampleVolume = myTestSample.tparTestSamples.Item(0).SampleVolume
                    End If

                    test.DilutionFactor = 1
                    If Not myTestSample.tparTestSamples.Item(0).IsAbsorbanceDilutionFactorNull Then
                        test.DilutionFactor = myTestSample.tparTestSamples.Item(0).AbsorbanceDilutionFactor
                    End If

                    test.PreDilutionFlag = False
                    test.PredilutionFactor = 1   'AG 11/03/2010 - Init value
                    If Not myTestSample.tparTestSamples.Item(0).IsPredilutionUseFlagNull Then
                        test.PreDilutionFlag = myTestSample.tparTestSamples.Item(0).PredilutionUseFlag
                    End If

                    If Not myTestSample.tparTestSamples.Item(0).IsPredilutionFactorNull Then
                        test.PredilutionFactor = myTestSample.tparTestSamples.Item(0).PredilutionFactor
                    End If

                    If Not myTestSample.tparTestSamples.Item(0).IsBlankAbsorbanceLimitNull Then
                        test.BlankAbsorbanceLimit.Value = myTestSample.tparTestSamples.Item(0).BlankAbsorbanceLimit
                        test.BlankAbsorbanceLimit.InUse = True
                    End If

                    test.PostDilutionFactor = 1
                    test.PostSampleVolume = test.SampleVolume 'AG 11/03/2010 Initialization
                    Select Case preparation(0).PostDilutionType
                        Case "INC"
                            If Not myTestSample.tparTestSamples.Item(0).IsIncPostdilutionFactorNull AndAlso myTestSample.tparTestSamples.Item(0).IncPostdilutionFactor > 0 Then
                                'AG 12/01/2012 - case increase: factor 1/F
                                'test.PostDilutionFactor = myTestSample.tparTestSamples.Item(0).IncPostdilutionFactor
                                test.PostDilutionFactor = 1 / myTestSample.tparTestSamples.Item(0).IncPostdilutionFactor
                                If Not myTestSample.tparTestSamples.Item(0).IsIncPostSampleVolumeNull Then test.PostSampleVolume = myTestSample.tparTestSamples.Item(0).IncPostSampleVolume 'AG 11/03/2010

                            End If

                        Case "RED"
                            If Not myTestSample.tparTestSamples.Item(0).IsRedPostdilutionFactorNull Then
                                If myTestSample.tparTestSamples.Item(0).RedPostdilutionFactor > 0 Then
                                    ''AG 12/01/2012 - case increase: factor 1/F
                                    'test.PostDilutionFactor = 1 / myTestSample.tparTestSamples.Item(0).RedPostdilutionFactor
                                    test.PostDilutionFactor = myTestSample.tparTestSamples.Item(0).RedPostdilutionFactor
                                    If Not myTestSample.tparTestSamples.Item(0).IsRedPostSampleVolumeNull Then test.PostSampleVolume = myTestSample.tparTestSamples.Item(0).RedPostSampleVolume 'AG 11/03/2010
                                End If
                            End If

                        Case "NONE"
                            test.PostDilutionFactor = 1
                    End Select

                    If Not myTestsDS.tparTests.Item(0).IsKineticBlankLimitNull Then
                        test.KineticBlankLimit.InUse = True
                        test.KineticBlankLimit.Value = myTestsDS.tparTests.Item(0).KineticBlankLimit
                    Else
                        test.KineticBlankLimit.InUse = False
                    End If

                    test.Prozone.InUse = False
                    test.LinearityLimit.InUse = False
                    test.DetectionLimit.InUse = False
                    test.SubstrateDepletion.InUse = False
                    test.Slope.InUse = False
                    If common(0).CycleMachine > 0 Then  'AG 11/06/2010
                        If Not myTestsDS.tparTests.Item(0).IsProzoneTime1Null Then
                            'test.Prozone.Cycle1 = myTestsDS.tparTests.Item(0).ProzoneTime1 
                            test.Prozone.Cycle1 = CInt(myTestsDS.tparTests.Item(0).ProzoneTime1 / common(0).CycleMachine)

                            If Not myTestsDS.tparTests.Item(0).IsProzoneTime2Null Then
                                'test.Prozone.Cycle2 = myTestsDS.tparTests.Item(0).ProzoneTime2
                                test.Prozone.Cycle2 = CInt(myTestsDS.tparTests.Item(0).ProzoneTime2 / common(0).CycleMachine)
                            End If

                            If Not myTestsDS.tparTests.Item(0).IsProzoneRatioNull Then
                                'test.Prozone.Ratio = CInt(myTestsDS.tparTests.Item(0).ProzoneRatio)
                                test.Prozone.Ratio = myTestsDS.tparTests.Item(0).ProzoneRatio
                                test.Prozone.InUse = True
                            End If
                        End If
                    End If

                    test.BlankMode = "DISTW"
                    If Not myTestsDS.tparTests.Item(0).IsBlankModeNull Then
                        test.BlankMode = myTestsDS.tparTests.Item(0).BlankMode
                    End If

                    If Not myTestSample.tparTestSamples.Item(0).IsLinearityLimitNull Then
                        test.LinearityLimit.InUse = True
                        test.LinearityLimit.Value = myTestSample.tparTestSamples.Item(0).LinearityLimit
                    End If

                    If Not myTestSample.tparTestSamples.Item(0).IsDetectionLimitNull Then
                        test.DetectionLimit.Value = myTestSample.tparTestSamples.Item(0).DetectionLimit
                        test.DetectionLimit.InUse = True
                    End If

                    If Not myTestSample.tparTestSamples.Item(0).IsSubstrateDepletionValueNull Then
                        test.SubstrateDepletion.Value = myTestSample.tparTestSamples.Item(0).SubstrateDepletionValue
                        test.SubstrateDepletion.InUse = True
                    End If

                    If Not myTestSample.tparTestSamples.Item(0).IsSlopeFactorANull Then
                        test.Slope.A = myTestSample.tparTestSamples.Item(0).SlopeFactorA

                        If Not myTestSample.tparTestSamples.Item(0).IsSlopeFactorBNull Then
                            test.Slope.B = myTestSample.tparTestSamples.Item(0).SlopeFactorB
                            test.Slope.InUse = True
                        End If
                    End If
                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If


                test.FactorLimit.InUse = False
                If Not myTestSample.tparTestSamples.Item(0).IsFactorUpperLimitNull Then
                    test.FactorLimit.Maximum = myTestSample.tparTestSamples.Item(0).FactorUpperLimit

                    If Not myTestSample.tparTestSamples.Item(0).IsFactorLowerLimitNull Then
                        test.FactorLimit.Minimum = myTestSample.tparTestSamples.Item(0).FactorLowerLimit
                        test.FactorLimit.InUse = True
                    End If
                End If
                'END AG 11/06/2010

                ' DL 29/06/2010
                InitReferenceRange(pdbConnection, myExecutionsDS, test.TestID, test.SampleType)

            Catch ex As Exception
                'AG 24/02/2010
                'resultData.HasError = True
                'resultData.ErrorCode = "SYSTEM_ERROR"
                'resultData.ErrorMessage = ex.Message
                '
                'Dim myLogAcciones As New ApplicationLogManager()
                'myLogAcciones.CreateLogActivity(ex.Message, "CalculationsDelegate.InitCommon", EventLogEntryType.Error, False)
                Me.CatchLaunched("InitTest", ex)
            End Try

        End Sub

#End Region

#Region "To_Delete"
        '''' <summary>
        '''' Find and get the last calibrator results for the current test
        '''' </summary>
        '''' <remarks>
        '''' Created by AG 26/02/2010 (Tested ...)
        '''' </remarks>
        'Private Sub GetLastCalibResultsOLD(ByVal pDBConnection As SqlClient.SqlConnection)
        '    Try
        '        'Initiate values
        '        If calibrator.NumberOfCalibrators = 1 Then
        '            calibrator.Factor = 1
        '            calibrator.BlankAbsUsed = 0
        '            calibrator.ErrorCalibration = ""
        '            calibrator.CalibratorDate = DateTime.MinValue
        '        Else
        '            ReDim calibrator.Curve.Points(1, calibrator.Curve.CurvePointsNumber)
        '            calibrator.BlankAbsUsed = 0
        '            calibrator.ErrorCalibration = ""
        '            calibrator.CalibratorDate = DateTime.MinValue
        '        End If


        '        Dim dbConnection As New SqlClient.SqlConnection
        '        Dim res_delegate As New ResultsDelegate
        '        Dim localres As New GlobalDataTO

        '        localres = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not localres.HasError) And (Not localres.SetDatos Is Nothing) Then
        '            dbConnection = CType(localres.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then

        '                'Find the last OrderTestID belongs to the calibrator: testId-sampletype in analyzer                        
        '                localres = res_delegate.GetLastExecutedCalibrator(dbConnection, test.TestID, calibrator.SampleType, test.TestVersion, myAnalyzerID, "", False)
        '                If Not localres.HasError And Not localres.SetDatos Is Nothing Then
        '                    Dim AdditionalElements As New WSAdditionalElementsDS
        '                    AdditionalElements = CType(localres.SetDatos, WSAdditionalElementsDS)

        '                    If AdditionalElements.WSAdditionalElementsTable.Rows.Count > 0 Then
        '                        Dim foundOrderTestId As Integer = AdditionalElements.WSAdditionalElementsTable(0).PreviousOrderTestID


        '                        'Get the calibrator results
        '                        localres = res_delegate.GetAcceptedResults(dbConnection, foundOrderTestId, False, False)
        '                        If Not localres.HasError And Not localres.SetDatos Is Nothing Then
        '                            Dim lastResults As New ResultsDS
        '                            lastResults = CType(localres.SetDatos, ResultsDS)
        '                            If lastResults.twksResults.Rows.Count > 0 Then
        '                                If Not lastResults.twksResults(0).IsCalibratorBlankAbsUsedNull Then calibrator.BlankAbsUsed = lastResults.twksResults(0).CalibratorBlankAbsUsed
        '                                If Not lastResults.twksResults(0).IsCalibrationErrorNull Then calibrator.ErrorCalibration = lastResults.twksResults(0).CalibrationError
        '                                If Not lastResults.twksResults(0).IsCurveGrowthTypeNull Then calibrator.Curve.CurveGrowthType = lastResults.twksResults(0).CurveGrowthType
        '                                If Not lastResults.twksResults(0).IsCurveTypeNull Then calibrator.Curve.CurveType = lastResults.twksResults(0).CurveType
        '                                If Not lastResults.twksResults(0).IsCurveAxisXTypeNull Then calibrator.Curve.CurveAxisXType = lastResults.twksResults(0).CurveAxisXType
        '                                If Not lastResults.twksResults(0).IsCurveAxisYTypeNull Then calibrator.Curve.CurveAxisXType = lastResults.twksResults(0).CurveAxisYType
        '                                If Not lastResults.twksResults(0).IsResultDateTimeNull Then calibrator.CalibratorDate = lastResults.twksResults(0).ResultDateTime 'AG 11/03/2010


        '                                'if calibrator NOT is multitiem then we already get the results. Otherwise we need to get the curve results
        '                                If calibrator.NumberOfCalibrators = 1 Then
        '                                    If Not lastResults.twksResults(0).IsCalibratorFactorNull Then calibrator.Factor = lastResults.twksResults(0).CalibratorFactor
        '                                    If Not lastResults.twksResults(0).IsABSValueNull Then calibrator.CalibratorAbs = lastResults.twksResults(0).ABSValue
        '                                    If Not lastResults.twksResults(0).IsABS_ErrorNull Then calibrator.ErrorAbs = lastResults.twksResults(0).ABS_Error

        '                                Else
        '                                    For i As Integer = 0 To lastResults.twksResults.Rows.Count - 1
        '                                        If Not lastResults.twksResults(i).IsABSValueNull Then calibrator.Curve.CalibratorAbs(i) = lastResults.twksResults(i).ABSValue
        '                                        If Not lastResults.twksResults(i).IsABS_ErrorNull Then calibrator.Curve.ErrorAbs(i) = lastResults.twksResults(i).ABS_Error
        '                                        If Not lastResults.twksResults(i).IsCONC_ValueNull Then calibrator.Curve.ConcCurve(i) = lastResults.twksResults(i).CONC_Value
        '                                        If Not lastResults.twksResults(i).IsRelativeErrorCurveNull Then calibrator.Curve.ErrorCurve(i) = lastResults.twksResults(i).RelativeErrorCurve
        '                                    Next i

        '                                    'Read the curve points CurveResultID
        '                                    'Dim Points(,) As Single '(1 <Abs or Conc>, CurvePoints <Point value>)
        '                                    If Not lastResults.twksResults(0).IsCurveResultsIDNull Then
        '                                        Dim curve_del As New CurveResultsDelegate

        '                                        localres = curve_del.GetResults(dbConnection, lastResults.twksResults(0).CurveResultsID)
        '                                        If Not localres.HasError And Not localres.SetDatos Is Nothing Then
        '                                            Dim curveDS As New CurveResultsDS
        '                                            curveDS = CType(localres.SetDatos, CurveResultsDS)

        '                                            ReDim calibrator.Curve.Points(1, curveDS.twksCurveResults.Rows.Count - 1)
        '                                            For i As Integer = 0 To curveDS.twksCurveResults.Rows.Count - 1
        '                                                calibrator.Curve.Points(ABS_ID, i) = curveDS.twksCurveResults(i).ABSValue
        '                                                calibrator.Curve.Points(CONC_ID, i) = curveDS.twksCurveResults(i).CONCValue
        '                                            Next
        '                                        End If

        '                                    Else
        '                                        myClassGlobalResult.HasError = True
        '                                        myClassGlobalResult.ErrorCode = GlobalEnumerates.AbsorbanceErrors.INCORRECT_DATA.ToString
        '                                    End If
        '                                End If 'If calibrator.NumberOfCalibrators = 1 Then
        '                            End If 'If lastResults.twksResults.Rows.Count > 0 Then
        '                        Else
        '                            calibrator.ErrorCalibration = GlobalEnumerates.CalibrationFactorErrors.NON_MONOTONOUS_CURVE.ToString
        '                        End If 'If Not localres.HasError And Not localres.SetDatos Is Nothing Then

        '                    Else
        '                        calibrator.ErrorCalibration = GlobalEnumerates.CalibrationFactorErrors.ABS_ERROR.ToString
        '                    End If 'If AdditionalElements.WSAdditionalElementsTable.Rows.Count > 0 Then

        '                End If
        '            End If
        '        End If

        '    Catch ex As Exception
        '        Me.CatchLaunched("GetLastCalibResults", ex)
        '    End Try
        'End Sub

        '''' <summary>
        '''' Get minimun ref range
        '''' </summary>
        '''' <param name="pdbConnection"></param>
        '''' <param name="pTestID"></param>
        '''' <param name="pSampleType"></param>
        '''' <param name="pRangeType"></param>
        '''' <param name="pExecutionsDS"></param>
        '''' <returns>single</returns>
        '''' <remarks>Created by: DL 23/08/2010</remarks>
        'Public Function GetMinReferenceRange(ByVal pdbConnection As SqlClient.SqlConnection, _
        '                                     ByVal pTestID As Integer, _
        '                                     ByVal pSampleType As String, _
        '                                     ByVal pRangeType As String, _
        '                                     ByVal pExecutionsDS As ExecutionsDS, _
        '                                     ByVal pTesType As String) As Single

        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Dim resultData As New GlobalDataTO
        '    Dim myMinimunValue As Single = -1

        '    Try
        '        ' DL 08/09/2010 {
        '        ' Dim mySampleClass As String = pExecutionsDS.twksWSExecutions.Item(0).SampleClass.ToString.Trim

        '        ' If mySampleClass = "PATIENT" Then
        '        ' DL 08/09/2010 }

        '        'RH: There is no need here to use a transaction, because we do not make
        '        'any change or atomic operation over the database. We only read data.
        '        'resultData = DAOBase.GetOpenDBTransaction(pdbConnection)
        '        resultData = DAOBase.GetOpenDBConnection(pdbConnection)

        '        If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

        '            If (Not dbConnection Is Nothing) Then
        '                Dim myOrderTestID As Integer = pExecutionsDS.twksWSExecutions.Item(0).OrderTestID
        '                Dim myTestRefRangesDelegate As New TestRefRangesDelegate()

        '                resultData = myTestRefRangesDelegate.ReadByTestID(pdbConnection, pTestID, pSampleType, pRangeType, pTesType)

        '                If Not resultData.HasError Then
        '                    Dim myTestRefRanges As New TestRefRangesDS
        '                    myTestRefRanges = CType(resultData.SetDatos, TestRefRangesDS)

        '                    Select Case pRangeType

        '                        Case "GENERIC"

        '                            If Not myTestRefRanges.tparTestRefRanges.Item(0).IsNormalLowerLimitNull Then
        '                                myMinimunValue = myTestRefRanges.tparTestRefRanges.Item(0).NormalLowerLimit
        '                            End If

        '                        Case "DETAILED"

        '                            Dim myOrderTestData As New OrderTestsDelegate
        '                            resultData = myOrderTestData.GetTestID(pdbConnection, myOrderTestID)

        '                            If Not resultData.HasError Then
        '                                Dim myOrdersDelegate As New OrdersDelegate
        '                                Dim myOrderTestDS As New OrderTestsDS
        '                                Dim myOrderID As String
        '                                Dim myPatientID As String = ""

        '                                myOrderTestDS = CType(resultData.SetDatos, OrderTestsDS)
        '                                myOrderID = myOrderTestDS.twksOrderTests.Item(0).OrderID
        '                                resultData = myOrdersDelegate.ReadOrders(pdbConnection, myOrderID)

        '                                If Not resultData.HasError Then
        '                                    Dim myOrdersDS As New OrdersDS
        '                                    myOrdersDS = CType(resultData.SetDatos, OrdersDS)
        '                                    myPatientID = myOrdersDS.twksOrders.Item(0).PatientID
        '                                End If

        '                                If myPatientID <> "" Then
        '                                    Dim myPatientDelegate As New PatientDelegate
        '                                    resultData = myPatientDelegate.GetPatientData(pdbConnection, myPatientID)

        '                                    If Not resultData.HasError Then
        '                                        Dim myPatientDS As New PatientsDS
        '                                        myPatientDS = CType(resultData.SetDatos, PatientsDS)

        '                                        Dim bAge As Boolean = False
        '                                        Dim bGender As Boolean = False

        '                                        Dim qAgeRanges As New List(Of String)
        '                                        qAgeRanges = (From a In myTestRefRanges.tparTestRefRanges _
        '                                                      Order By a.AgeUnit _
        '                                                      Group a By Age_ID = a.AgeUnit Into Group _
        '                                                      Where Age_ID <> "" _
        '                                                      Select Age_ID).ToList

        '                                        If qAgeRanges.Count > 0 Then bAge = True

        '                                        Dim qGenderRanges As New List(Of String)
        '                                        qGenderRanges = (From a In myTestRefRanges.tparTestRefRanges _
        '                                                         Order By a.Gender _
        '                                                         Group a By Gender_ID = a.Gender Into Group _
        '                                                         Where Gender_ID <> "" _
        '                                                         Select Gender_ID).ToList

        '                                        If qGenderRanges.Count > 0 Then bGender = True

        '                                        If bGender And Not bAge Then
        '                                            ' Gender
        '                                            resultData = myTestRefRangesDelegate.GetDetailedByGender(pdbConnection, _
        '                                                                                                     pTestID, _
        '                                                                                                     pSampleType, _
        '                                                                                                     myPatientDS.tparPatients.Item(0).Gender)
        '                                        ElseIf bAge And Not bGender Then
        '                                            ' Age
        '                                            resultData = myTestRefRangesDelegate.GetDetailedByAge(pdbConnection, _
        '                                                                                                  pTestID, _
        '                                                                                                  pSampleType, _
        '                                                                                                  myPatientDS.tparPatients.Item(0).Age)

        '                                        ElseIf bAge And bGender Then
        '                                            ' Gender + Age
        '                                            resultData = myTestRefRangesDelegate.GetDetailedByGenderAge(pdbConnection, _
        '                                                                                                        pTestID, _
        '                                                                                                        pSampleType, _
        '                                                                                                        myPatientDS.tparPatients.Item(0).Gender, _
        '                                                                                                        myPatientDS.tparPatients.Item(0).Age)
        '                                        End If

        '                                        If Not resultData.HasError Then
        '                                            myTestRefRanges = CType(resultData.SetDatos, TestRefRangesDS)

        '                                            If myTestRefRanges.tparTestRefRanges.Count > 0 Then 'AG - add this condition to fix system error
        '                                                If Not myTestRefRanges.tparTestRefRanges.Item(0).IsNormalLowerLimitNull Then
        '                                                    myMinimunValue = myTestRefRanges.tparTestRefRanges.Item(0).NormalLowerLimit
        '                                                End If
        '                                            End If

        '                                        End If
        '                                    End If
        '                                End If
        '                            End If
        '                    End Select
        '                End If

        '                ''Close connection if needed
        '                'If (Not resultData.HasError) Then
        '                '    'When the Database Connection was opened locally, then the Commit is executed
        '                '    If (pdbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                'Else
        '                '    'When the Database Connection was opened locally, then the Rollback is executed
        '                '    If (pdbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                'End If

        '            End If
        '        End If

        '        ' DL 08/09/2010 {
        '        'End If
        '        ' DL 08/09/2010 }

        '    Catch ex As Exception
        '        Me.CatchLaunched("GetMinReferenceRange", ex)

        '    Finally
        '        If (pdbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()

        '    End Try

        '    Return myMinimunValue

        'End Function

        '''' <summary>
        '''' Get minimun ref border
        '''' </summary>
        '''' <param name="pdbConnection"></param>
        '''' <param name="pTestID"></param>
        '''' <param name="pSampleType"></param>
        '''' <param name="pRangeType"></param>
        '''' <param name="pExecutionsDS"></param>
        '''' <returns>single</returns>
        '''' <remarks>Created by: DL 23/08/2010</remarks>
        'Private Function GetMinReferenceBorder(ByVal pdbConnection As SqlClient.SqlConnection, _
        '                                       ByVal pTestID As Integer, _
        '                                       ByVal pSampleType As String, _
        '                                       ByVal pRangeType As String, _
        '                                       ByVal pExecutionsDS As ExecutionsDS) As Single


        '    Dim resultData As New GlobalDataTO
        '    Dim myMinimunValue As Single = -1

        '    Try
        '        'If pExecutionsDS.twksWSExecutions.Item(0).SampleClass.ToString.Trim = "PATIENT" And pRangeType = "GENERIC" Then
        '        If pRangeType = "GENERIC" Then ' DL 09/09/2010
        '            Dim myTestRefRangesDelegate As New TestRefRangesDelegate()

        '            resultData = myTestRefRangesDelegate.ReadByTestID(pdbConnection, pTestID, pSampleType, pRangeType)

        '            If Not resultData.HasError Then
        '                Dim myTestRefRanges As New TestRefRangesDS
        '                myTestRefRanges = CType(resultData.SetDatos, TestRefRangesDS)

        '                If myTestRefRanges.tparTestRefRanges.Count > 0 Then 'AG
        '                    If Not myTestRefRanges.tparTestRefRanges.Item(0).IsBorderLineLowerLimitNull Then
        '                        myMinimunValue = myTestRefRanges.tparTestRefRanges.Item(0).BorderLineLowerLimit
        '                    End If
        '                End If

        '            End If
        '        End If

        '    Catch ex As Exception
        '        Me.CatchLaunched("MinRefBorder", ex)
        '    End Try

        '    Return myMinimunValue

        'End Function


        '''' <summary>
        '''' Get minimun ref border
        '''' </summary>
        '''' <param name="pdbConnection"></param>
        '''' <param name="pTestID"></param>
        '''' <param name="pSampleType"></param>
        '''' <param name="pRangeType"></param>
        '''' <param name="pExecutionsDS"></param>
        '''' <returns>single</returns>
        '''' <remarks>Created by: DL 23/08/2010</remarks>
        'Private Function GetMaxReferenceBorder(ByVal pdbConnection As SqlClient.SqlConnection, _
        '                                       ByVal pTestID As Integer, _
        '                                       ByVal pSampleType As String, _
        '                                       ByVal pRangeType As String, _
        '                                       ByVal pExecutionsDS As ExecutionsDS) As Single


        '    Dim resultData As New GlobalDataTO
        '    Dim myMaximunValue As Single = -1

        '    Try
        '        'If pExecutionsDS.twksWSExecutions.Item(0).SampleClass.ToString.Trim = "PATIENT" And pRangeType = "GENERIC" Then 
        '        If pRangeType = "GENERIC" Then ' DL 09/09/2010

        '            Dim myTestRefRangesDelegate As New TestRefRangesDelegate()

        '            resultData = myTestRefRangesDelegate.ReadByTestID(pdbConnection, pTestID, pSampleType, pRangeType)

        '            If Not resultData.HasError Then
        '                Dim myTestRefRanges As New TestRefRangesDS
        '                myTestRefRanges = CType(resultData.SetDatos, TestRefRangesDS)

        '                If myTestRefRanges.tparTestRefRanges.Count > 0 Then 'AG
        '                    If Not myTestRefRanges.tparTestRefRanges.Item(0).IsBorderLineUpperLimitNull Then
        '                        myMaximunValue = myTestRefRanges.tparTestRefRanges.Item(0).BorderLineUpperLimit
        '                    End If
        '                End If

        '            End If
        '        End If

        '    Catch ex As Exception
        '        Me.CatchLaunched("MaxRefBorder", ex)
        '    End Try

        '    Return myMaximunValue

        'End Function


        '''' <summary>
        '''' Get maximun ref range
        '''' </summary>
        '''' <param name="pdbConnection"></param>
        '''' <param name="pTestID"></param>
        '''' <param name="pSampleType"></param>
        '''' <param name="pRangeType"></param>
        '''' <param name="pExecutionsDS"></param>
        '''' <returns>single</returns>
        '''' <remarks>Created by: DL 23/08/2010</remarks>
        'Public Function GetMaxReferenceRange(ByVal pdbConnection As SqlClient.SqlConnection, _
        '                                     ByVal pTestID As Integer, _
        '                                     ByVal pSampleType As String, _
        '                                     ByVal pRangeType As String, _
        '                                     ByVal pExecutionsDS As ExecutionsDS, _
        '                                     ByVal pTestType As String) As Single

        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Dim resultData As New GlobalDataTO
        '    Dim myMaximunValue As Single = -1

        '    Try
        '        Dim mySampleClass As String = pExecutionsDS.twksWSExecutions.Item(0).SampleClass.ToString.Trim

        '        'If mySampleClass = "PATIENT" Then

        '        'RH: There is no need here to use a transaction, because we do not make
        '        'any change or atomic operation over the database. We only read data.
        '        'resultData = DAOBase.GetOpenDBTransaction(pdbConnection)
        '        resultData = DAOBase.GetOpenDBConnection(pdbConnection)

        '        If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

        '            If (Not dbConnection Is Nothing) Then
        '                Dim myOrderTestID As Integer = pExecutionsDS.twksWSExecutions.Item(0).OrderTestID
        '                Dim myTestRefRangesDelegate As New TestRefRangesDelegate()

        '                resultData = myTestRefRangesDelegate.ReadByTestID(pdbConnection, pTestID, pSampleType, pRangeType, pTestType)

        '                If Not resultData.HasError Then
        '                    Dim myTestRefRanges As New TestRefRangesDS
        '                    myTestRefRanges = CType(resultData.SetDatos, TestRefRangesDS)

        '                    Select Case pRangeType

        '                        Case "GENERIC"

        '                            If Not myTestRefRanges.tparTestRefRanges.Item(0).IsNormalUpperLimitNull Then
        '                                myMaximunValue = myTestRefRanges.tparTestRefRanges.Item(0).NormalUpperLimit
        '                            End If

        '                        Case "DETAILED"

        '                            Dim myOrderTestData As New OrderTestsDelegate
        '                            resultData = myOrderTestData.GetTestID(pdbConnection, myOrderTestID)

        '                            If Not resultData.HasError Then
        '                                Dim myOrdersDelegate As New OrdersDelegate
        '                                Dim myOrderTestDS As New OrderTestsDS
        '                                Dim myOrderID As String
        '                                Dim myPatientID As String = ""

        '                                myOrderTestDS = CType(resultData.SetDatos, OrderTestsDS)
        '                                myOrderID = myOrderTestDS.twksOrderTests.Item(0).OrderID
        '                                resultData = myOrdersDelegate.ReadOrders(pdbConnection, myOrderID)

        '                                If Not resultData.HasError Then
        '                                    Dim myOrdersDS As New OrdersDS
        '                                    myOrdersDS = CType(resultData.SetDatos, OrdersDS)
        '                                    myPatientID = myOrdersDS.twksOrders.Item(0).PatientID
        '                                End If

        '                                If myPatientID <> "" Then
        '                                    Dim myPatientDelegate As New PatientDelegate
        '                                    resultData = myPatientDelegate.GetPatientData(pdbConnection, myPatientID)

        '                                    If Not resultData.HasError Then
        '                                        Dim myPatientDS As New PatientsDS
        '                                        myPatientDS = CType(resultData.SetDatos, PatientsDS)

        '                                        Dim bAge As Boolean = False
        '                                        Dim bGender As Boolean = False

        '                                        Dim qAgeRanges As New List(Of String)
        '                                        qAgeRanges = (From a In myTestRefRanges.tparTestRefRanges _
        '                                                      Order By a.AgeUnit _
        '                                                      Group a By Age_ID = a.AgeUnit Into Group _
        '                                                      Where Age_ID <> "" _
        '                                                      Select Age_ID).ToList

        '                                        If qAgeRanges.Count > 0 Then bAge = True

        '                                        Dim qGenderRanges As New List(Of String)
        '                                        qGenderRanges = (From a In myTestRefRanges.tparTestRefRanges _
        '                                                         Order By a.Gender _
        '                                                         Group a By Gender_ID = a.Gender Into Group _
        '                                                         Where Gender_ID <> "" _
        '                                                         Select Gender_ID).ToList

        '                                        If qGenderRanges.Count > 0 Then bGender = True

        '                                        If bGender And Not bAge Then
        '                                            ' Gender
        '                                            resultData = myTestRefRangesDelegate.GetDetailedByGender(pdbConnection, _
        '                                                                                                     pTestID, _
        '                                                                                                     pSampleType, _
        '                                                                                                     myPatientDS.tparPatients.Item(0).Gender)
        '                                        ElseIf bAge And Not bGender Then
        '                                            ' Age
        '                                            resultData = myTestRefRangesDelegate.GetDetailedByAge(pdbConnection, _
        '                                                                                                  pTestID, _
        '                                                                                                  test.SampleType, _
        '                                                                                                  myPatientDS.tparPatients.Item(0).Age)

        '                                        ElseIf bAge And bGender Then
        '                                            ' Gender + Age
        '                                            resultData = myTestRefRangesDelegate.GetDetailedByGenderAge(pdbConnection, _
        '                                                                                                        pTestID, _
        '                                                                                                        pSampleType, _
        '                                                                                                        myPatientDS.tparPatients.Item(0).Gender, _
        '                                                                                                        myPatientDS.tparPatients.Item(0).Age)
        '                                        End If

        '                                        If Not resultData.HasError Then
        '                                            myTestRefRanges = CType(resultData.SetDatos, TestRefRangesDS)

        '                                            If myTestRefRanges.tparTestRefRanges.Count > 0 Then 'AG - add this condition to fix system error
        '                                                If Not myTestRefRanges.tparTestRefRanges.Item(0).IsNormalUpperLimitNull Then
        '                                                    myMaximunValue = myTestRefRanges.tparTestRefRanges.Item(0).NormalUpperLimit
        '                                                End If
        '                                            End If

        '                                        End If
        '                                    End If
        '                                End If
        '                            End If
        '                    End Select

        '                End If

        '                ''Close connection if needed
        '                'If (Not resultData.HasError) Then
        '                '    'When the Database Connection was opened locally, then the Commit is executed
        '                '    If (pdbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                'Else
        '                '    'When the Database Connection was opened locally, then the Rollback is executed
        '                '    If (pdbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                'End If


        '            End If
        '        End If
        '        'End If

        '    Catch ex As Exception
        '        Me.CatchLaunched("GetMaxReferenceRange", ex)

        '    Finally
        '        If (pdbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()

        '    End Try

        '    Return myMaximunValue

        'End Function

        '''' <summary>
        '''' Get ref range, both values in only one method.
        '''' Returns range in output parameters pMinimunValue and pMaximunValue
        '''' </summary>
        '''' <param name="pdbConnection"></param>
        '''' <param name="pTestID"></param>
        '''' <param name="pSampleType"></param>
        '''' <param name="pRangeType"></param>
        '''' <param name="pOrderTestID"></param>
        '''' <param name="pMinimunValue"></param>
        '''' <param name="pMaximunValue"></param>
        '''' <remarks>
        '''' Created by: RH 09/16/2010 based on DL GetMinReferenceRange()
        ''''             Merges GetMinReferenceRange() and GetMaxReferenceRange() for doing only one call per range
        '''' </remarks>
        'Public Sub GetReferenceRange(ByVal pdbConnection As SqlClient.SqlConnection, _
        '                                     ByVal pTestID As Integer, _
        '                                     ByVal pSampleType As String, _
        '                                     ByVal pRangeType As String, _
        '                                     ByVal pOrderTestID As Integer, _
        '                                     ByVal pTesType As String, _
        '                                     ByRef pMinimunValue As Nullable(Of Single), _
        '                                     ByRef pMaximunValue As Nullable(Of Single))

        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Dim resultData As New GlobalDataTO

        '    Try

        '        'RH: There is no need here to use a transaction, because we do not make
        '        'any change or atomic operation over the database. We only read data.
        '        'resultData = DAOBase.GetOpenDBTransaction(pdbConnection)
        '        resultData = DAOBase.GetOpenDBConnection(pdbConnection)

        '        If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

        '            If (Not dbConnection Is Nothing) Then
        '                Dim myTestRefRangesDelegate As New TestRefRangesDelegate()

        '                resultData = myTestRefRangesDelegate.ReadByTestID(pdbConnection, pTestID, pSampleType, pRangeType, pTesType)

        '                If Not resultData.HasError Then
        '                    Dim myTestRefRanges As New TestRefRangesDS
        '                    myTestRefRanges = CType(resultData.SetDatos, TestRefRangesDS)

        '                    pMinimunValue = Nothing
        '                    pMaximunValue = Nothing

        '                    If myTestRefRanges.tparTestRefRanges.Rows.Count = 0 Then 'Row not found                               
        '                        Return
        '                    End If

        '                    Select Case pRangeType

        '                        Case "GENERIC"

        '                            If Not myTestRefRanges.tparTestRefRanges.Item(0).IsNormalLowerLimitNull Then
        '                                pMinimunValue = myTestRefRanges.tparTestRefRanges.Item(0).NormalLowerLimit
        '                            End If

        '                            If Not myTestRefRanges.tparTestRefRanges.Item(0).IsNormalUpperLimitNull Then
        '                                pMaximunValue = myTestRefRanges.tparTestRefRanges.Item(0).NormalUpperLimit
        '                            End If

        '                        Case "DETAILED"

        '                            Dim myOrderTestData As New OrderTestsDelegate
        '                            resultData = myOrderTestData.GetTestID(pdbConnection, pOrderTestID)

        '                            If Not resultData.HasError Then
        '                                Dim myOrdersDelegate As New OrdersDelegate
        '                                Dim myOrderTestDS As New OrderTestsDS
        '                                Dim myOrderID As String
        '                                Dim myPatientID As String = ""

        '                                myOrderTestDS = CType(resultData.SetDatos, OrderTestsDS)

        '                                If myOrderTestDS.twksOrderTests.Rows.Count = 0 Then 'Row not found
        '                                    Return
        '                                End If

        '                                myOrderID = myOrderTestDS.twksOrderTests.Item(0).OrderID

        '                                resultData = myOrdersDelegate.ReadOrders(pdbConnection, myOrderID)

        '                                If Not resultData.HasError Then
        '                                    Dim myOrdersDS As New OrdersDS
        '                                    myOrdersDS = CType(resultData.SetDatos, OrdersDS)
        '                                    myPatientID = myOrdersDS.twksOrders.Item(0).PatientID
        '                                End If

        '                                If myPatientID <> "" Then
        '                                    Dim myPatientDelegate As New PatientDelegate
        '                                    resultData = myPatientDelegate.GetPatientData(pdbConnection, myPatientID)

        '                                    If Not resultData.HasError Then
        '                                        Dim myPatientDS As New PatientsDS
        '                                        myPatientDS = CType(resultData.SetDatos, PatientsDS)

        '                                        Dim bAge As Boolean = False
        '                                        Dim bGender As Boolean = False

        '                                        Dim qAgeRanges As New List(Of String)
        '                                        qAgeRanges = (From a In myTestRefRanges.tparTestRefRanges _
        '                                                      Order By a.AgeUnit _
        '                                                      Group a By Age_ID = a.AgeUnit Into Group _
        '                                                      Where Age_ID <> "" _
        '                                                      Select Age_ID).ToList

        '                                        If qAgeRanges.Count > 0 Then bAge = True

        '                                        Dim qGenderRanges As New List(Of String)
        '                                        qGenderRanges = (From a In myTestRefRanges.tparTestRefRanges _
        '                                                         Order By a.Gender _
        '                                                         Group a By Gender_ID = a.Gender Into Group _
        '                                                         Where Gender_ID <> "" _
        '                                                         Select Gender_ID).ToList

        '                                        'If qGenderRanges.Count > 0 Then bGender = True
        '                                        bGender = qGenderRanges.Count > 0

        '                                        If bGender And Not bAge Then
        '                                            ' Gender
        '                                            resultData = myTestRefRangesDelegate.GetDetailedByGender( _
        '                                                            pdbConnection, pTestID, pSampleType, _
        '                                                            myPatientDS.tparPatients.Item(0).Gender, pTesType)
        '                                        ElseIf bAge And Not bGender Then
        '                                            ' Age
        '                                            resultData = myTestRefRangesDelegate.GetDetailedByAge( _
        '                                                            pdbConnection, pTestID, pSampleType, _
        '                                                            myPatientDS.tparPatients.Item(0).Age, pTesType)

        '                                        ElseIf bAge And bGender Then
        '                                            ' Gender + Age
        '                                            resultData = myTestRefRangesDelegate.GetDetailedByGenderAge( _
        '                                                            pdbConnection, pTestID, pSampleType, _
        '                                                            myPatientDS.tparPatients.Item(0).Gender, _
        '                                                            myPatientDS.tparPatients.Item(0).Age, pTesType)
        '                                        End If

        '                                        If Not resultData.HasError Then
        '                                            myTestRefRanges = CType(resultData.SetDatos, TestRefRangesDS)

        '                                            If Not myTestRefRanges.tparTestRefRanges.Item(0).IsNormalLowerLimitNull Then
        '                                                pMinimunValue = myTestRefRanges.tparTestRefRanges.Item(0).NormalLowerLimit
        '                                            End If

        '                                            If Not myTestRefRanges.tparTestRefRanges.Item(0).IsNormalUpperLimitNull Then
        '                                                pMaximunValue = myTestRefRanges.tparTestRefRanges.Item(0).NormalUpperLimit
        '                                            End If

        '                                        End If
        '                                    End If
        '                                End If
        '                            End If
        '                    End Select
        '                End If

        '                ''Close connection if needed
        '                'If (Not resultData.HasError) Then
        '                '    'When the Database Connection was opened locally, then the Commit is executed
        '                '    If (pdbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                'Else
        '                '    'When the Database Connection was opened locally, then the Rollback is executed
        '                '    If (pdbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                'End If

        '            End If
        '        End If

        '    Catch ex As Exception
        '        Me.CatchLaunched("GetMinReferenceRange", ex)

        '    Finally
        '        If (pdbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()

        '    End Try

        'End Sub
#End Region

#Region "METHODS REPLACED FOR NEW ONES DUE TO PERFORMANCE ISSUES - TO DELETE"
        ''' <summary>
        ''' Calculation methods who initializes structures, execute calculation and finally write into database
        ''' </summary>
        ''' <param name="pExecutionID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pRecalculusFlag"></param>
        ''' <param name="pSampleClassRecalculated"></param>
        ''' <param name="pManualRecalculationsFlag"></param>
        ''' <returns>GlobalDataTo with set indicating if process successed or not</returns>
        ''' <remarks>
        ''' Created by AG 02/03/2010 (tested OK)
        ''' Modified by AG 10/09/2010 - add optional parameter ManualRecalculusFlag (default value FALSE)
        ''' </remarks>
        Public Function CalculateExecution(ByVal pDBConnection As SqlClient.SqlConnection, _
                                           ByVal pExecutionID As Integer, _
                                           ByVal pAnalyzerID As String, _
                                           ByVal pWorkSessionID As String, _
                                           ByVal pRecalculusFlag As Boolean, _
                                           ByVal pSampleClassRecalculated As String, _
                                           Optional ByVal pManualRecalculationsFlag As Boolean = False) As GlobalDataTO

            Dim globalData As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                'AG 29/06/2012 - Running Cycles lost - Solution!
                'globalData = DAOBase.GetOpenDBTransaction(pDBConnection)
                'If (Not globalData.HasError) And (Not globalData.SetDatos Is Nothing) Then
                '    dbConnection = CType(globalData.SetDatos, SqlClient.SqlConnection)
                '    If (Not dbConnection Is Nothing) Then

                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                'Dim StartTime As DateTime = Now
                'Dim myLogAcciones As New ApplicationLogManager()
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                'AG 10/09/2010
                If pRecalculusFlag Then
                    myManualRecalculationsFlag = pManualRecalculationsFlag
                End If
                'END AG 10/09/2010

                'Init structures
                globalData = Me.Init(dbConnection, pExecutionID, pAnalyzerID, pWorkSessionID, pRecalculusFlag, pSampleClassRecalculated)
                If globalData.HasError Then
                    globalData = myClassGlobalResult
                    Exit Try
                End If

                globalData = Me.ExecuteCalculations()
                If globalData.HasError Then
                    globalData = myClassGlobalResult
                    Exit Try
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                'myLogAcciones.CreateLogActivity("Calculate Execution Results: Id [" & pExecutionID.ToString & "] " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "CalculationsDelegate.CalculateExecution", EventLogEntryType.Information, False)
                'StartTime = Now

                'Postposed!!!
                'globalData = Me.SaveWaterResults()
                'If globalData.HasError Then
                '    globalData = myClassGlobalResult
                '    Exit Try
                'End If

                'Save results
                globalData = Me.SaveExecutionResults(dbConnection)
                If globalData.HasError Then
                    globalData = myClassGlobalResult
                    Exit Try
                End If

                '' XBC 03/07/2012 - time estimation
                'myLogAcciones.CreateLogActivity("Save Execution Results: Id [" & pExecutionID.ToString & "] " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "CalculationsDelegate.CalculateExecution", EventLogEntryType.Information, False)
                'StartTime = Now

                'AG 03/06/2010 - Calculate & save results remarks
                globalData = Me.GenerateResultsRemarks(dbConnection)
                If globalData.HasError Then
                    globalData = myClassGlobalResult
                    Exit Try
                End If
                ''AG 03/06/2010
                'myLogAcciones.CreateLogActivity("Calculate & Save remarks: Id [" & pExecutionID.ToString & "] " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "CalculationsDelegate.CalculateExecution", EventLogEntryType.Information, False)

                'AG 12/07/2010 - Calculated Tests operations (if execution OrderTestID is related with aa calculated test orderTestID)
                If preparation(0).SampleClass = "PATIENT" Then  'Calculated test only for patients
                    Dim myCalcTestsDelegate As New OperateCalculatedTestDelegate
                    myCalcTestsDelegate.AnalyzerID = myAnalyzerID
                    myCalcTestsDelegate.WorkSessionID = myWorkSessionID
                    globalData = myCalcTestsDelegate.ExecuteCalculatedTest(dbConnection, myOrderTestID, myManualRecalculationsFlag)
                    If globalData.HasError Then
                        Exit Try
                    End If
                End If
                'END AG 12/07/2010

                'AG 22/07/2010 - Call recalculations mode if new blank or calibrator is received
                If Not common(0).RecalculationFlag Then
                    If preparation(0).SampleClass = "BLANK" Or preparation(0).SampleClass = "CALIB" Then
                        Dim myRecalculations As New RecalculateResultsDelegate
                        myRecalculations.AnalyzerModel = myAnalyzerModel

                        'AG 23/07/2010 - Dont use different init code, use private Init method (Initialize recalculations structures)
                        'With myRecalculations
                        '    .AnalyzerID = myAnalyzerID
                        '    .WorkSessionID = myWorkSessionID
                        '    .OrderTestID = myOrderTestID
                        '    .RerunNumber = myRerunNumber
                        '    .SampleClass = preparation(0).SampleClass
                        '    .TestID = test.TestID
                        '    .SampleType = test.SampleType
                        '    .ExecutionID = myExecutionID(UBound(myExecutionID))
                        '    .MaxItemsNumber = UBound(myExecutionID) + 1
                        '    .MaxReplicates = preparation(0).MaxReplicates
                        '    .InitializedFlag = True
                        'End With

                        'Last parameter as FALSE
                        globalData = myRecalculations.RecalculateResults(dbConnection, pAnalyzerID, pWorkSessionID, pExecutionID, False)
                    End If
                End If
                'END AG 22/07/2010

                'Debug.Print("CalculationsDelegate.CalculateExecution: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 05/06/2012 - time estimation

                '    End If 'AG 29/06/2012 - Running Cycles lost - Solution!
                'End If 'AG 29/06/2012 - Running Cycles lost - Solution!

                '' XBC 03/07/2012 - time estimation

                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                'myLogAcciones.CreateLogActivity("End Function: Id [" & pExecutionID.ToString & "] " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                '                                "CalculationsDelegate.CalculateExecution", EventLogEntryType.Information, False)
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            Catch ex As Exception
                Me.CatchLaunched("CalculateExecution", ex)
                'If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection) 'AG 29/06/2012 - Running Cycles lost - Solution!('AG 10/05/2010)
            End Try
            'AG 29/06/2012 - Running Cycles lost - Solution!
            ''TR 18/10/2011 -place here to make sure on exit try to commit or rollback transaction.
            'If (Not globalData.HasError) Then
            '    'When the Database Connection was opened locally, then the Commit is executed
            '    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

            'Else
            '    'When the Database Connection was opened locally, then the Rollback is executed
            '    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
            'End If

            Return globalData
        End Function

        ''' <summary>
        ''' Initializes sample information, test information and general information needed for ExecuteCalculation
        ''' </summary>
        ''' <param name="pExecutionID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pRecalculusFlag"></param>
        ''' <param name="pSampleClassRecalculated"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:   AG ../02/2010 (Tested OK)
        ''' Modified by:  DL 19/02/2010 
        ''' Modified by:  AG 25/02/2010
        ''' Modified by: GDS 29/04/2010 (Added pDBConnection parameter)
        ''' Modified by AG 26/04/2011: improve speed reducing number of queries
        ''' </remarks>
        Private Function Init(ByVal pDBConnection As SqlClient.SqlConnection, _
                              ByVal pExecutionID As Integer, _
                              ByVal pAnalyzerID As String, _
                              ByVal pWorkSessionID As String, _
                              ByVal pRecalculusFlag As Boolean, _
                              ByVal pSampleClassRecalculated As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                'Initializar global class variables
                myAnalyzerID = pAnalyzerID
                myWorkSessionID = pWorkSessionID
                ReDim myExecutionID(0)
                myExecutionID(0) = pExecutionID

                'AG 29/06/2012 - Running Cycles lost - Solution!
                'resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                Dim myExecutionWell As Integer = 1

                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myExecutionsDS As ExecutionsDS
                        Dim myExecutionDelegate As New ExecutionsDelegate

                        Dim SampleClass As String = ""
                        Dim NumberOfCalibrators As Integer = 1

                        'AG 26/04/2011
                        ''Get the sampleclass related to the ExecutionID
                        '' DL 24/02/2010
                        'Dim myOrdersDelegate As New OrdersDelegate

                        'resultData = myOrdersDelegate.GetSampleClass(dbConnection, _
                        '                                             pExecutionID, _
                        '                                             pAnalyzerID, _
                        '                                             pWorkSessionID)

                        'If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                        '    Dim myOrderDetailsDS As New OrderDetailsDS
                        '    myOrderDetailsDS = CType(resultData.SetDatos, OrderDetailsDS)
                        '    SampleClass = myOrderDetailsDS.OrderDetails.Item(0).SampleClass
                        'Else
                        '    myClassGlobalResult = resultData
                        '    Exit Try
                        'End If

                        'find the multiitem number & sample class                        
                        'If SampleClass = "CALIB" Then
                        resultData = myExecutionDelegate.GetNumberOfMultititem(dbConnection, _
                                                                               pExecutionID)

                        ' DL 24/02/2010
                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            Dim myExecutionDS As ExecutionsDS
                            myExecutionDS = CType(resultData.SetDatos, ExecutionsDS)
                            NumberOfCalibrators = myExecutionDS.twksWSExecutions(0).MultiItemNumber
                            SampleClass = myExecutionDS.twksWSExecutions(0).SampleClass
                        Else
                            myClassGlobalResult = resultData
                            Exit Try
                        End If
                        'End If

                        If SampleClass = "CALIB" And NumberOfCalibrators > 1 Then
                            'DL 24/02/2010
                            resultData = myExecutionDelegate.GetExecutionMultititem(dbConnection, _
                                                                                    pExecutionID)

                            'AG 24/02/2010
                            If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                myExecutionsDS = CType(resultData.SetDatos, ExecutionsDS)
                            Else
                                myClassGlobalResult = resultData
                                Exit Try
                            End If
                            'AG 24/02/2010

                            Dim myIndex As Integer = 0
                            myOrderTestID = 0 'AG 10/08/2010

                            For Each myExecutionRow As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions.Rows
                                ReDim Preserve myExecutionID(myIndex)
                                myExecutionID(myIndex) = myExecutionRow.ExecutionID

                                ' Read execution data
                                ' DL 24/02/2010
                                resultData = myExecutionDelegate.GetExecution(dbConnection, _
                                                                              myExecutionRow.ExecutionID, _
                                                                              pAnalyzerID, _
                                                                              pWorkSessionID)

                                If myOrderTestID = 0 Then
                                    myOrderTestID = myExecutionsDS.twksWSExecutions(0).OrderTestID  'Save into global class variable the ordertestid to calculate
                                End If

                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                    myExecutionsDS = CType(resultData.SetDatos, ExecutionsDS)
                                    myRerunNumber = myExecutionsDS.twksWSExecutions(0).RerunNumber

                                    'AG 20/05/2010
                                    If Not myExecutionsDS.twksWSExecutions(0).IsWellUsedNull Then myExecutionWell = myExecutionsDS.twksWSExecutions(0).WellUsed

                                    ' Get common structure
                                    InitCommon(dbConnection, _
                                               myExecutionsDS, _
                                               myIndex, _
                                               pRecalculusFlag, _
                                               pSampleClassRecalculated, myExecutionWell)

                                    If myClassGlobalResult.HasError Then Exit Try

                                    ' Get preparation structure
                                    InitPreparation(dbConnection, _
                                                    myExecutionsDS, _
                                                    myIndex, _
                                                    SampleClass)

                                    If myClassGlobalResult.HasError Then Exit Try

                                    myIndex = myIndex + 1
                                Else
                                    myClassGlobalResult = resultData
                                    Exit Try
                                End If
                            Next myExecutionRow

                        Else
                            ' read execution data
                            resultData = myExecutionDelegate.GetExecution(dbConnection, _
                                                                          pExecutionID, _
                                                                          pAnalyzerID, _
                                                                          pWorkSessionID)

                            If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                myExecutionsDS = CType(resultData.SetDatos, ExecutionsDS)
                                ' Get common structure                                
                                myOrderTestID = myExecutionsDS.twksWSExecutions(0).OrderTestID  'Save into global class variable the ordertestid to calculate
                                myRerunNumber = myExecutionsDS.twksWSExecutions(0).RerunNumber

                                'AG 20/05/2010
                                If Not myExecutionsDS.twksWSExecutions(0).IsWellUsedNull Then myExecutionWell = myExecutionsDS.twksWSExecutions(0).WellUsed

                                InitCommon(dbConnection, _
                                           myExecutionsDS, _
                                           0, _
                                           pRecalculusFlag, _
                                           pSampleClassRecalculated, myExecutionWell)

                                If myClassGlobalResult.HasError Then Exit Try

                                ' Get preparation structure
                                InitPreparation(dbConnection, _
                                                myExecutionsDS, _
                                                0, _
                                                SampleClass)

                                If myClassGlobalResult.HasError Then Exit Try

                            Else
                                myClassGlobalResult = resultData
                                Exit Try
                            End If
                        End If

                        ' Get test structure
                        InitTest(dbConnection, myExecutionsDS)
                        If myClassGlobalResult.HasError Then Exit Try

                        ' get calibrator structure                        
                        'AG 01/09/2010
                        'InitCalibration(dbConnection, myExecutionsDS)
                        'If myClassGlobalResult.HasError Then Exit Try
                        If Not String.Equals(preparation(0).SampleClass, "BLANK") Then
                            InitCalibration(dbConnection, myExecutionsDS)
                            If myClassGlobalResult.HasError Then Exit Try
                        End If
                        'END AG 01/09/2010

                        ' Initialize data that needs several structures
                        InitFieldsRelatedWithSeveralStructures(dbConnection, NumberOfCalibrators)

                        If myClassGlobalResult.HasError Then Exit Try

                    End If

                    'TR 18/10/2011 -Commented and implemented outside the try
                    'If (Not resultData.HasError) Then
                    '    'When the Database Connection was opened locally, then the Commit is executed
                    '    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                    'Else
                    '    'When the Database Connection was opened locally, then the Rollback is executed
                    '    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                    'End If
                    'TR 18/10/2011 -END.

                Else
                    myClassGlobalResult = resultData
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalculationsDelegate.Init", EventLogEntryType.Error, False)
                'TR 18/10/2011 -Commented and implemented outside the try
                'Finally
                '    If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            'TR 18/10/2011 -implementation outside the try
            If Not resultData.HasError AndAlso Not myClassGlobalResult.HasError Then
                'When the Database Connection was opened locally, then the Commit is executed
                'If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection) 'AG 29/06/2012 - Running Cycles lost - Solution!
            Else
                'When the Database Connection was opened locally, then the Rollback is executed
                'If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)'AG 29/06/2012 - Running Cycles lost - Solution!
            End If
            'TR 18/10/2011 -Validate to close connection.
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            'TR 18/10/2011 -END.

            Return myClassGlobalResult
        End Function

        ''' <summary>
        ''' Initializes common information
        ''' </summary>
        ''' <param name="pdbConnection"></param>
        ''' <param name="pExecutionDS"></param>
        ''' <param name="pDimension"></param>
        ''' <remarks>
        ''' Created by : DL 19/02/2010 - Tested AG 25/02/2010 (OK)
        ''' Modified: AG 14/05/2010 -read the parameters using the Swparameterdelegate instead the baseline parameters!!! 
        '''                         -change field names in BaseLinesDS
        ''' Modified AG 17/05/2010 (use method GetBaseLineValues) - tested pending
        '''          AG 20/05/2010 - add pExecutionWell parameter
        '''          AG 26/04/2011 - improve speed
        '''          AG 23/10/2013 - Initialize new properties in common structure (kineticsIncreaseInPause, useFirstR1SampleReadings)
        ''' </remarks>
        Private Sub InitCommon(ByVal pdbConnection As SqlClient.SqlConnection, _
                               ByVal pExecutionDS As ExecutionsDS, _
                               ByVal pDimension As Integer, ByVal pRecalculusFlag As Boolean, _
                               ByVal pSampleClassRecalculated As String, ByVal pExecutionWell As Integer)

            Dim myParametersDS As New ParametersDS
            Dim resultData As New GlobalDataTO

            Try
                ReDim Preserve common(pDimension)

                ' Get Execution Values & parameters
                common(pDimension).RecalculationFlag = pRecalculusFlag
                common(pDimension).SampleClassRecalculated = pSampleClassRecalculated
                If Not pExecutionDS.twksWSExecutions.Item(0).IsBaseLineIDNull Then common(pDimension).BaseLineID = pExecutionDS.twksWSExecutions.Item(0).BaseLineID
                If Not pExecutionDS.twksWSExecutions.Item(0).IsAdjustBaseLineIDNull Then common(pDimension).AdjustBaseLineID = pExecutionDS.twksWSExecutions.Item(0).AdjustBaseLineID 'AG 03/11/2011

                'Get analyzer model (if not informed)
                If myAnalyzerModel = "" Then
                    Dim myAnalyzersDelegate As New AnalyzersDelegate
                    resultData = myAnalyzersDelegate.GetAnalyzerModel(pdbConnection, pExecutionDS.twksWSExecutions.Item(0).AnalyzerID)
                    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                        Dim myAnalizerDS As New AnalyzersDS
                        myAnalizerDS = CType(resultData.SetDatos, AnalyzersDS)
                        myAnalyzerModel = myAnalizerDS.tcfgAnalyzers.Item(0).AnalyzerModel
                    Else
                        myClassGlobalResult = resultData
                        Exit Try
                    End If
                End If

                ' Get Parameters Values from tfmwswparameters
                'AG 14/05/2010 - To read the parameter use the swParameterDelegate not the BaseLinesdelegate!!!
                'Dim myWSBaseLinesDelegate As New WSBaseLinesDelegate
                '...
                '...
                Dim myParams As New SwParametersDelegate
                Dim myLinq As New List(Of ParametersDS.tfmwSwParametersRow)

                'Read all parameters
                resultData = myParams.ReadByAnalyzerModel(pdbConnection, myAnalyzerModel)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myParametersDS = CType(resultData.SetDatos, ParametersDS)
                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If

                'AG 24/10/2013 Task #1347
                Dim limitsDlg As New FieldLimitsDelegate
                Dim limitsDS As New FieldLimitsDS
                Dim myLimitsLinq As New List(Of FieldLimitsDS.tfmwFieldLimitsRow)
                resultData = limitsDlg.GetAllList(pdbConnection, myAnalyzerModel)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    limitsDS = CType(resultData.SetDatos, FieldLimitsDS)
                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If
                'AG 24/10/2013

                ' LimitAbs
                myLinq = (From a As ParametersDS.tfmwSwParametersRow In myParametersDS.tfmwSwParameters _
                          Where a.ParameterName = GlobalEnumerates.SwParameters.LIMIT_ABS.ToString _
                          Select a).ToList
                If myLinq.Count > 0 Then
                    common(pDimension).LimitAbs = myLinq(0).ValueNumeric
                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If

                ' PathLength
                myLinq = (From a As ParametersDS.tfmwSwParametersRow In myParametersDS.tfmwSwParameters _
                          Where a.ParameterName = GlobalEnumerates.SwParameters.PATH_LENGHT.ToString _
                          Select a).ToList
                If myLinq.Count > 0 Then
                    common(pDimension).PathLength = myLinq(0).ValueNumeric
                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If


                ' KineticsIncrease
                'AG 10/11/2014 BA-2082 filter by current analyzer model
                myLinq = (From a As ParametersDS.tfmwSwParametersRow In myParametersDS.tfmwSwParameters _
                          Where a.ParameterName = GlobalEnumerates.SwParameters.KINETICS_INCREASE.ToString AndAlso a.AnalyzerModel = myAnalyzerModel _
                          Select a).ToList
                If myLinq.Count > 0 Then
                    common(pDimension).KineticsIncrease = myLinq(0).ValueNumeric
                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If

                ' AG 11/06/2010 - AnalyzerCycleMachine
                'AG 10/11/2014 BA-2082 filter by current analyzer model
                myLinq = (From a As ParametersDS.tfmwSwParametersRow In myParametersDS.tfmwSwParameters _
                          Where a.ParameterName = GlobalEnumerates.SwParameters.CYCLE_MACHINE.ToString AndAlso a.AnalyzerModel = myAnalyzerModel _
                          Select a).ToList
                If myLinq.Count > 0 Then
                    common(pDimension).CycleMachine = myLinq(0).ValueNumeric
                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If
                'END AG 11/06/2010

                ' LinearCorrelationFactor
                myLinq = (From a As ParametersDS.tfmwSwParametersRow In myParametersDS.tfmwSwParameters _
                          Where a.ParameterName = GlobalEnumerates.SwParameters.LINEAR_CORRELATION_FACTOR.ToString _
                          Select a).ToList
                If myLinq.Count > 0 Then
                    common(pDimension).LinearCorrelationFactor = myLinq(0).ValueNumeric
                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If

                ' Reagent1Readings
                myLinq = (From a As ParametersDS.tfmwSwParametersRow In myParametersDS.tfmwSwParameters _
                          Where a.ParameterName = GlobalEnumerates.SwParameters.REAGENT1_READINGS.ToString _
                          Select a).ToList
                If myLinq.Count > 0 Then
                    common(pDimension).Reagent1Readings = CInt(myLinq(0).ValueNumeric)
                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If

                ' ExtrapolationMaximum
                myLinq = (From a As ParametersDS.tfmwSwParametersRow In myParametersDS.tfmwSwParameters _
                          Where a.ParameterName = GlobalEnumerates.SwParameters.MAX_EXTRAPOLATION.ToString _
                          Select a).ToList
                If myLinq.Count > 0 Then
                    common(pDimension).ExtrapolationMaximum = CInt(myLinq(0).ValueNumeric)
                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If

                ' Curve points number
                If calibrator.Curve.CurvePointsNumber <= 0 Then
                    myLinq = (From a As ParametersDS.tfmwSwParametersRow In myParametersDS.tfmwSwParameters _
                              Where a.ParameterName = GlobalEnumerates.SwParameters.CURVE_POINTS_NUMBER.ToString _
                              Select a).ToList
                    If myLinq.Count > 0 Then
                        calibrator.Curve.CurvePointsNumber = CInt(myLinq(0).ValueNumeric)
                    Else
                        myClassGlobalResult = resultData
                        Exit Try
                    End If
                End If

                'AG 23/10/2013 Task #1347
                'AG 10/11/2014 BA-2082 filter by analyzer model
                'KineticsIncreaseInPause
                myLinq = (From a As ParametersDS.tfmwSwParametersRow In myParametersDS.tfmwSwParameters _
                          Where a.ParameterName = GlobalEnumerates.SwParameters.SS_KINETICS_INCREASE.ToString AndAlso a.AnalyzerModel = myAnalyzerModel _
                          Select a).ToList
                If myLinq.Count > 0 Then
                    common(pDimension).KineticsIncreaseInPause = myLinq(0).ValueNumeric
                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If

                'UseFirstR1SampleReadings
                myLinq = (From a As ParametersDS.tfmwSwParametersRow In myParametersDS.tfmwSwParameters _
                          Where a.ParameterName = GlobalEnumerates.SwParameters.R1SAMPLE1STREADINGSFORCALC.ToString _
                          Select a).ToList
                If myLinq.Count > 0 Then
                    common(pDimension).UseFirstR1SampleReadings = CInt(myLinq(0).ValueNumeric)
                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If

                'SwReadingsOffset 
                'AG 10/11/2014 BA-2082 filter by model
                myLinq = (From a As ParametersDS.tfmwSwParametersRow In myParametersDS.tfmwSwParameters _
                          Where a.ParameterName = GlobalEnumerates.SwParameters.SW_READINGSOFFSET.ToString AndAlso a.AnalyzerModel = myAnalyzerModel _
                          Select a).ToList
                If myLinq.Count > 0 Then
                    common(pDimension).SwReadingsOffset = CInt(myLinq(0).ValueNumeric)
                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If

                'DefaultReadNumberR2Added, NumberOfReadingsReadingsWithR2
                'AG 10/11/2014 BA-2082 filter by model
                myLimitsLinq = (From a As FieldLimitsDS.tfmwFieldLimitsRow In limitsDS.tfmwFieldLimits _
                                Where a.LimitID = GlobalEnumerates.FieldLimitsEnum.READING2_CYCLES.ToString AndAlso a.AnalyzerModel = myAnalyzerModel _
                                Select a).ToList
                If myLimitsLinq.Count > 0 Then
                    common(pDimension).DefaultReadNumberR2Added = CInt(myLimitsLinq(0).MinValue) - common(pDimension).SwReadingsOffset
                    common(pDimension).NumberOfReadingsWithR2 = CInt(myLimitsLinq(0).MaxValue) - CInt(myLimitsLinq(0).MinValue) + 1
                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If
                'AG 23/10/2013 Task #1347

                ' Get baseline values
                'Dark values are get from twksWSBLines
                'Light values are get from twksWSBLinesByWell

                'AG 11/11/2014 BA-2064 read the parameter that informs how works the model
                'resultData = GetBaseLineValues(pdbConnection, myAnalyzerID, myWorkSessionID, common(pDimension).BaseLineID, pExecutionWell, common(pDimension).AdjustBaseLineID, GlobalEnumerates.BaseLineType.DYNAMIC.ToString)
                myLinq = (From a As ParametersDS.tfmwSwParametersRow In myParametersDS.tfmwSwParameters _
                          Where a.ParameterName = GlobalEnumerates.SwParameters.BL_TYPE_FOR_CALCULATIONS.ToString AndAlso a.AnalyzerModel = myAnalyzerModel Select a).ToList
                If myLinq.Count > 0 Then
                    resultData = GetBaseLineValues(pdbConnection, myAnalyzerID, myWorkSessionID, common(pDimension).BaseLineID, pExecutionWell, common(pDimension).AdjustBaseLineID, myLinq(0).ValueText.ToString())
                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If
                'AG 11/11/2014

                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                    Dim myBaseLineDS As New BaseLinesDS
                    myBaseLineDS = CType(resultData.SetDatos, BaseLinesDS)

                    'common(pDimension).BaseLineWell = myBaseLineDS.twksWSBaseLines(0).WellUsed

                    ' Redim all properties of baseline
                    ReDim common(pDimension).BaseLine(myBaseLineDS.twksWSBaseLines.Rows.Count - 1)
                    ReDim common(pDimension).DarkCurrent(myBaseLineDS.twksWSBaseLines.Rows.Count - 1)
                    ReDim common(pDimension).RefDarkCurrent(myBaseLineDS.twksWSBaseLines.Rows.Count - 1)
                    ReDim common(pDimension).RefBaseLine(myBaseLineDS.twksWSBaseLines.Rows.Count - 1)

                    Dim myIndex As Integer = 0

                    For Each myBaseLineRow As BaseLinesDS.twksWSBaseLinesRow In myBaseLineDS.twksWSBaseLines.Rows
                        If myIndex = 0 Then common(pDimension).BaseLineWell = myBaseLineDS.twksWSBaseLines(myIndex).WellUsed

                        'AG 14/05/2010 - Use the new field names in DS
                        common(pDimension).BaseLine(myIndex) = myBaseLineRow.MainLight
                        common(pDimension).DarkCurrent(myIndex) = myBaseLineRow.MainDark
                        common(pDimension).RefBaseLine(myIndex) = myBaseLineRow.RefLight
                        common(pDimension).RefDarkCurrent(myIndex) = myBaseLineRow.RefDark
                        myIndex = myIndex + 1
                    Next myBaseLineRow
                Else
                    myClassGlobalResult = resultData
                End If

                myLinq = Nothing 'AG 25/02/2014 - #1521
                myLimitsLinq = Nothing 'AG 24/10/2013

            Catch ex As Exception
                'AG 24/02/2010
                'resultData.HasError = True
                'resultData.ErrorCode = "SYSTEM_ERROR"
                'resultData.ErrorMessage = ex.Message
                '
                'Dim myLogAcciones As New ApplicationLogManager()
                'myLogAcciones.CreateLogActivity(ex.Message, "CalculationsDelegate.InitCommon", EventLogEntryType.Error, False)
                Me.CatchLaunched("InitCommon", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Initializes Calibrator structure for the specified Execution
        ''' </summary>
        ''' <param name="pdbConnection"></param>
        ''' <param name="pExecutionsDS"></param>
        ''' <remarks>
        ''' </remarks>
        ''' Created by : DL 23/02/2010
        ''' Modified by: AG 26/02/2010 (Tested OK)
        '''              SA 30/08/2010 - Call function GetCalibratorData (instead of calling GetCalibratorID plus
        '''                              GetCalibrationData) to get CalibratorID and NumberOfCalibrators for the
        '''                              specified Execution
        '''              AG 02/09/2010 - Fix problems using alternative calibrator (the CalibratorID and NumberOfCalibrators arent initialized, keep 0 value)
        Private Sub InitCalibration(ByVal pdbConnection As SqlClient.SqlConnection, ByVal pExecutionsDS As ExecutionsDS)
            Dim resultData As New GlobalDataTO

            Try
                calibrator.ErrorAbs = "" 'AG 27/08/2010
                calibrator.ErrorCalibration = "" 'AG 27/08/2010
                calibrator.ManualFactor = 1 'AG 09/11/2010
                calibrator.ManualFactorFlag = False 'AG 09/11/2010

                'SA 30/08/2010
                'Get Execution data
                Dim ExecutionsDelegate As New ExecutionsDelegate
                'resultData = ExecutionsDelegate.GetCalibratorID(pdbConnection, pExecutionsDS.twksWSExecutions.Item(0).ExecutionID)
                resultData = ExecutionsDelegate.GetCalibratorData(pdbConnection, pExecutionsDS.twksWSExecutions.Item(0).ExecutionID)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    Dim calibratorDataDS As TestSampleCalibratorDS = DirectCast(resultData.SetDatos, TestSampleCalibratorDS)

                    If (calibratorDataDS.tparTestCalibrators.Rows.Count = 1) Then
                        calibrator.CalibratorID = calibratorDataDS.tparTestCalibrators(0).CalibratorID
                        calibrator.NumberOfCalibrators = calibratorDataDS.tparTestCalibrators(0).NumberOfCalibrators
                        calibrator.ExpirationDate = calibratorDataDS.tparTestCalibrators(0).ExpirationDate 'AG 08/11/2010
                    End If

                    'Dim mywsRequiredElementsDS As New WSRequiredElementsDS
                    'mywsRequiredElementsDS = CType(resultData.SetDatos, WSRequiredElementsDS)

                    'If mywsRequiredElementsDS.twksWSRequiredElements.Rows.Count > 0 Then
                    '    calibrator.CalibratorID = mywsRequiredElementsDS.twksWSRequiredElements.Item(0).CalibratorID

                    '    'AG 05/03/2010
                    '    Dim cal_delegate As New CalibratorsDelegate
                    '    resultData = cal_delegate.GetCalibratorData(pdbConnection, calibrator.CalibratorID)
                    '    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                    '        Dim calibDS As New CalibratorsDS
                    '        calibDS = CType(resultData.SetDatos, CalibratorsDS)
                    '        calibrator.NumberOfCalibrators = calibDS.tparCalibrators(0).NumberOfCalibrators
                    '    Else
                    '        myClassGlobalResult = resultData
                    '        Exit Try
                    '    End If
                    '    'END AG 05/03/2010
                    'End If
                    'END SA 30/08/2010
                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If

                ' Get Order Tests values from TwksOrderTests                
                calibrator.TestID = test.TestID
                calibrator.SampleType = test.SampleType

                Dim myTestSamplesDelegate As New TestSamplesDelegate
                Dim myTestSample As TestSamplesDS

                'AG 11/06/2010
                'resultData = myTestSamplesDelegate.GetSampleDataByTestID(pdbConnection, calibrator.TestID)
                resultData = myTestSamplesDelegate.GetDefinition(pdbConnection, calibrator.TestID, calibrator.SampleType)
                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                    myTestSample = CType(resultData.SetDatos, TestSamplesDS)

                    calibrator.CalibrationType = myTestSample.tparTestSamples.Item(0).CalibratorType
                    calibrator.AlternativeSampleType = ""
                    calibrator.Factor = 1

                    Select Case calibrator.CalibrationType
                        Case "FACTOR"
                            If Not myTestSample.tparTestSamples.Item(0).IsCalibrationFactorNull Then
                                calibrator.Factor = myTestSample.tparTestSamples.Item(0).CalibrationFactor
                            Else
                                myClassGlobalResult.HasError = True
                                myClassGlobalResult.ErrorCode = GlobalEnumerates.AbsorbanceErrors.INCORRECT_DATA.ToString
                            End If

                        Case "EXPERIMENT"

                        Case "ALTERNATIV"
                            If Not myTestSample.tparTestSamples.Item(0).IsSampleTypeAlternativeNull Then
                                calibrator.AlternativeSampleType = myTestSample.tparTestSamples.Item(0).SampleTypeAlternative
                                calibrator.SampleType = calibrator.AlternativeSampleType

                                'AG 02/09/2010 - Initialize CalibratorID and NumberOfCalibrators using alternative calibrator
                                resultData = ExecutionsDelegate.GetCalibratorData(pdbConnection, pExecutionsDS.twksWSExecutions.Item(0).ExecutionID, calibrator.TestID, calibrator.SampleType)
                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                    Dim calibratorDataDS As New TestSampleCalibratorDS
                                    calibratorDataDS = DirectCast(resultData.SetDatos, TestSampleCalibratorDS)

                                    If (calibratorDataDS.tparTestCalibrators.Rows.Count = 1) Then
                                        calibrator.CalibratorID = calibratorDataDS.tparTestCalibrators(0).CalibratorID
                                        calibrator.NumberOfCalibrators = calibratorDataDS.tparTestCalibrators(0).NumberOfCalibrators
                                        calibrator.ExpirationDate = calibratorDataDS.tparTestCalibrators(0).ExpirationDate  'AG 08/11/2010

                                        'AG 22/11/2010 - Maybe the alternative is a FACTOR
                                    ElseIf (calibratorDataDS.tparTestCalibrators.Rows.Count = 0) Then
                                        resultData = myTestSamplesDelegate.GetDefinition(pdbConnection, calibrator.TestID, calibrator.SampleType)
                                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                            myTestSample = CType(resultData.SetDatos, TestSamplesDS)
                                            calibrator.CalibrationType = myTestSample.tparTestSamples.Item(0).CalibratorType
                                            If calibrator.CalibrationType = "FACTOR" Then
                                                If Not myTestSample.tparTestSamples.Item(0).IsCalibrationFactorNull Then
                                                    calibrator.Factor = myTestSample.tparTestSamples.Item(0).CalibrationFactor
                                                Else
                                                    myClassGlobalResult.HasError = True
                                                    myClassGlobalResult.ErrorCode = GlobalEnumerates.AbsorbanceErrors.INCORRECT_DATA.ToString
                                                End If
                                            End If
                                        End If
                                        'END AG 22/11/2010

                                    End If


                                Else
                                    myClassGlobalResult = resultData
                                    Exit Try
                                End If
                                'END AG 02/09/201
                            Else
                                myClassGlobalResult.HasError = True
                                myClassGlobalResult.ErrorCode = GlobalEnumerates.AbsorbanceErrors.INCORRECT_DATA.ToString
                            End If
                    End Select
                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If

                If calibrator.CalibrationType <> "FACTOR" Then 'AG 27/08/2010
                    Dim testCalibratorData As New TestCalibratorsDelegate
                    If calibrator.NumberOfCalibrators > 1 Then
                        ReDim calibrator.Curve.CalibratorAbs(calibrator.NumberOfCalibrators - 1)
                        ReDim calibrator.Curve.ErrorAbs(calibrator.NumberOfCalibrators - 1)
                        ReDim calibrator.Curve.ConcCurve(calibrator.NumberOfCalibrators - 1)
                        ReDim calibrator.Curve.ErrorCurve(calibrator.NumberOfCalibrators - 1)
                        ReDim calibrator.Curve.Coefficient(3, 15) 'Idem Ax5

                        resultData = testCalibratorData.GetTestCalibratorData(pdbConnection, calibrator.TestID, calibrator.SampleType)

                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            Dim TestSampleCalib As New TestSampleCalibratorDS
                            TestSampleCalib = CType(resultData.SetDatos, TestSampleCalibratorDS)

                            If Not TestSampleCalib.tparTestCalibrators(0).IsCurveGrowthTypeNull Then calibrator.Curve.CurveGrowthType = TestSampleCalib.tparTestCalibrators(0).CurveGrowthType
                            If Not TestSampleCalib.tparTestCalibrators(0).IsCurveTypeNull Then calibrator.Curve.CurveType = TestSampleCalib.tparTestCalibrators(0).CurveType
                            If Not TestSampleCalib.tparTestCalibrators(0).IsCurveAxisXTypeNull Then calibrator.Curve.CurveAxisXType = TestSampleCalib.tparTestCalibrators(0).CurveAxisXType
                            If Not TestSampleCalib.tparTestCalibrators(0).IsCurveAxisYTypeNull Then calibrator.Curve.CurveAxisYType = TestSampleCalib.tparTestCalibrators(0).CurveAxisYType
                        Else
                            myClassGlobalResult = resultData
                            Exit Try
                        End If

                    End If

                    'AG 01/09/2010
                    'resultData = testCalibratorData.GetTestCalibratorValues(pdbConnection, calibrator.TestID)
                    resultData = testCalibratorData.GetTestCalibratorValues(pdbConnection, calibrator.TestID, calibrator.SampleType)

                    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                        Dim TestCalibratorValuesData As New TestCalibratorValuesDS
                        Dim myIndex As Integer = 0

                        TestCalibratorValuesData = CType(resultData.SetDatos, TestCalibratorValuesDS)

                        ReDim calibrator.TheoreticalConcentration(TestCalibratorValuesData.tparTestCalibratorValues.Rows.Count - 1)
                        For Each TestCalibratorValuesDataRow As TestCalibratorValuesDS.tparTestCalibratorValuesRow In TestCalibratorValuesData.tparTestCalibratorValues.Rows
                            calibrator.TheoreticalConcentration(myIndex) = TestCalibratorValuesDataRow.TheoricalConcentration

                            myIndex = myIndex + 1
                        Next TestCalibratorValuesDataRow
                    Else
                        myClassGlobalResult = resultData
                        Exit Try
                    End If
                End If 'AG 27/08/2010

            Catch ex As Exception
                'AG 24/02/2010
                'resultData.HasError = True
                'resultData.ErrorCode = "SYSTEM_ERROR"
                'resultData.ErrorMessage = ex.Message
                '
                'Dim myLogAcciones As New ApplicationLogManager()
                'myLogAcciones.CreateLogActivity(ex.Message, "CalculationsDelegate.InitCommon", EventLogEntryType.Error, False)
                Me.CatchLaunched("InitCalibration", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Initializes preparation information
        ''' </summary>
        ''' <param name="pdbConnection"></param>
        ''' <param name="pExecutionDS"></param>
        ''' <param name="pDimension"></param>
        ''' <remarks>
        ''' </remarks>
        ''' Created by : DL 23/02/2010
        ''' Modified by: AG 25/02/2010 (Tested OK)
        ''' Modified by AG 22/03/2011 - add ThermoWarningFlag, ClotValue
        ''' Modified    AG 23/10/2013 - initialize new properties in preparation structure (PausedReadings())
        Private Sub InitPreparation(ByVal pdbConnection As SqlClient.SqlConnection, _
                                    ByVal pExecutionDS As ExecutionsDS, _
                                    ByVal pDimension As Integer, _
                                    ByVal pSampleClass As String)

            Dim resultData As New GlobalDataTO

            Try
                ReDim Preserve preparation(pDimension)

                ' Get Execution values from twksWSExecutions
                preparation(pDimension).PreparationID = pExecutionDS.twksWSExecutions.Item(0).ExecutionID
                preparation(pDimension).SampleClass = pSampleClass

                ' Always 1 except in Calibrator curve (only for some SampleClass = 2)
                preparation(pDimension).MultiItemNumber = 1
                If Not pExecutionDS.twksWSExecutions.Item(0).IsMultiItemNumberNull Then
                    preparation(pDimension).MultiItemNumber = pExecutionDS.twksWSExecutions.Item(0).MultiItemNumber
                End If

                If Not pExecutionDS.twksWSExecutions.Item(0).IsReplicateNumberNull Then
                    preparation(pDimension).ReplicateID = pExecutionDS.twksWSExecutions.Item(0).ReplicateNumber
                End If

                If Not pExecutionDS.twksWSExecutions.Item(0).IsRerunNumberNull Then
                    preparation(pDimension).ReRun = pExecutionDS.twksWSExecutions.Item(0).RerunNumber
                End If

                preparation(pDimension).PostDilutionType = "NONE"
                If Not pExecutionDS.twksWSExecutions.Item(0).IsPostDilutionTypeNull Then
                    preparation(pDimension).PostDilutionType = pExecutionDS.twksWSExecutions.Item(0).PostDilutionType
                End If

                If Not pExecutionDS.twksWSExecutions.Item(0).IsWellUsedNull Then
                    preparation(pDimension).WellUsed = pExecutionDS.twksWSExecutions.Item(0).WellUsed
                End If

                'AG 22/03/2011 - ThermoWarningFlag, ClotValue
                If Not pExecutionDS.twksWSExecutions.Item(0).IsThermoWarningFlagNull Then
                    preparation(pDimension).ThermoWarningFlag = pExecutionDS.twksWSExecutions.Item(0).ThermoWarningFlag
                End If

                If Not pExecutionDS.twksWSExecutions.Item(0).IsClotValueNull Then
                    preparation(pDimension).PossibleClot = pExecutionDS.twksWSExecutions.Item(0).ClotValue
                End If
                'AG 22/03/2011

                'AG 03/05/2010 - Database field deleted
                'If Not pExecutionDS.twksWSExecutions.Item(0).IsRotorTurnNumberNull Then
                '    preparation(pDimension).RotorTurn = pExecutionDS.twksWSExecutions.Item(0).RotorTurnNumber
                'End If


                ' Get Order Tests values from TwksOrderTests
                Dim myOrderTestData As New OrderTestsDelegate

                resultData = myOrderTestData.GetTestID(pdbConnection, pExecutionDS.twksWSExecutions.Item(0).OrderTestID)
                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                    Dim myOrderTestDS As New OrderTestsDS
                    myOrderTestDS = CType(resultData.SetDatos, OrderTestsDS)
                    preparation(pDimension).TestID = myOrderTestDS.twksOrderTests.Item(0).TestID

                    If Not myOrderTestDS.twksOrderTests.Item(0).IsSampleTypeNull Then
                        preparation(pDimension).SampleType = myOrderTestDS.twksOrderTests.Item(0).SampleType
                    End If

                    If Not myOrderTestDS.twksOrderTests.Item(0).IsReplicatesNumberNull Then
                        preparation(pDimension).MaxReplicates = myOrderTestDS.twksOrderTests.Item(0).ReplicatesNumber
                    End If

                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If

                'Get data from TwksOrders                                
                Select Case pSampleClass
                    'Case "CALIB"
                    '    ' calibrator id
                    '    Dim mywsRequiredElementsDelegate As New WSRequiredElementsDelegate
                    '    resultData = mywsRequiredElementsDelegate.GetCalibratorID(pdbConnection, pExecutionDS.twksWSExecutions.Item(0).ExecutionID)
                    '    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                    '        Dim mywsRequiredElementsDS As New WSRequiredElementsDS
                    '        mywsRequiredElementsDS = CType(resultData.SetDatos, WSRequiredElementsDS)

                    '        preparation(pDimension).CalibratorID = mywsRequiredElementsDS.twksWSRequiredElements.Item(0).CalibratorID
                    'Else
                    '    myClassGlobalResult = resultData
                    '    Exit Try
                    'End If

                    Case "CTRL"
                        Dim mywsRequiredElementsDelegate As New WSRequiredElementsDelegate
                        resultData = mywsRequiredElementsDelegate.GetControlID(pdbConnection, pExecutionDS.twksWSExecutions.Item(0).ExecutionID)
                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            Dim mywsRequiredElementsDS As New WSRequiredElementsDS
                            mywsRequiredElementsDS = CType(resultData.SetDatos, WSRequiredElementsDS)
                            preparation(pDimension).ControlID = mywsRequiredElementsDS.twksWSRequiredElements.Item(0).ControlID
                        Else
                            myClassGlobalResult = resultData
                            Exit Try
                        End If
                End Select

                'Get Readings
                Dim ReadingsData As New WSReadingsDelegate
                Dim myIndex As Integer = 0

                'Read the reaction complete readings
                'AG 23/10/2013 Task #1347 (add last 6 parameters)
                resultData = ReadingsData.GetReadingsByExecutionID(pdbConnection, myAnalyzerID, myWorkSessionID, pExecutionDS.twksWSExecutions.Item(0).ExecutionID, True, _
                                                                   myAnalyzerModel, preparation(pDimension).TestID, common(pDimension).UseFirstR1SampleReadings, common(pDimension).DefaultReadNumberR2Added, _
                                                                   common(pDimension).NumberOfReadingsWithR2, common(pDimension).SwReadingsOffset)
                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                    Dim ReadingsDS As New twksWSReadingsDS
                    ReadingsDS = CType(resultData.SetDatos, twksWSReadingsDS)

                    For Each ReadingsRow As twksWSReadingsDS.twksWSReadingsRow In ReadingsDS.twksWSReadings.Rows
                        If ReadingsRow.ReactionComplete = True Then
                            ReDim Preserve preparation(pDimension).Readings(myIndex)
                            ReDim Preserve preparation(pDimension).RefReadings(myIndex)
                            ReDim Preserve preparation(pDimension).PausedReadings(myIndex) 'AG 23/10/2013 Task #1347
                            preparation(pDimension).Readings(myIndex) = ReadingsRow.MainCounts
                            preparation(pDimension).RefReadings(myIndex) = ReadingsRow.RefCounts
                            preparation(pDimension).PausedReadings(myIndex) = ReadingsRow.Pause 'AG 23/10/2013 Task #1347
                            myIndex = myIndex + 1
                        End If
                    Next ReadingsRow

                    If ReadingsDS.twksWSReadings.Rows.Count = 0 Then
                        myClassGlobalResult.HasError = True
                        myClassGlobalResult.ErrorCode = GlobalEnumerates.AbsorbanceErrors.INCORRECT_DATA.ToString
                    End If

                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If

                'AG 24/02/2010 - Redim result arrays using the max replicates
                Dim MaxReplicates As Integer = preparation(pDimension).MaxReplicates

                ReDim preparation(pDimension).InitialReplicateAbs(MaxReplicates - 1)
                ReDim preparation(pDimension).MainFilterReplicateAbs(MaxReplicates - 1)
                ReDim preparation(pDimension).ReplicateAbs(MaxReplicates - 1)
                ReDim preparation(pDimension).Reagent1ReplicateAbs(common(0).Reagent1Readings - 1)
                ReDim preparation(pDimension).ReplicateConc(MaxReplicates - 1)
                ReDim preparation(pDimension).rKineticsReplicate(MaxReplicates - 1)
                ReDim preparation(pDimension).KineticsCurve(MaxReplicates - 1, 1)
                ReDim preparation(pDimension).SubstrateDepletionReplicate(MaxReplicates - 1)
                ReDim preparation(pDimension).ErrorReplicateAbs(MaxReplicates - 1)
                ReDim preparation(pDimension).ErrorReplicateConc(MaxReplicates - 1)
                ReDim preparation(pDimension).ReplicateDate(MaxReplicates - 1)
                ReDim preparation(pDimension).RepInUse(MaxReplicates - 1)
                ReDim preparation(pDimension).ReplicateAlarms(MaxReplicates - 1)
                ReDim preparation(pDimension).ErrorCurveReplicate(MaxReplicates - 1)
                ReDim preparation(pDimension).WorkingReagentReplicateAbs(MaxReplicates - 1) 'AG 12/07/2010
                ReDim preparation(pDimension).ReplicateExecutionID(MaxReplicates - 1) 'AG 14/08/2010

            Catch ex As Exception
                'AG 24/02/2010
                'resultData.HasError = True
                'resultData.ErrorCode = "SYSTEM_ERROR"
                'resultData.ErrorMessage = ex.Message
                '
                'Dim myLogAcciones As New ApplicationLogManager()
                'myLogAcciones.CreateLogActivity(ex.Message, "CalculationsDelegate.InitCommon", EventLogEntryType.Error, False)
                Me.CatchLaunched("InitPreparation", ex)
            End Try

        End Sub
#End Region

#Region "NEW METHODS FOR PERFORMANCE IMPROVEMENTS"
        ''' <summary>
        ''' For an specific Execution, call methods to get all needed data, initialize structures, execute calculations and finally 
        ''' write into database
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pExecutionID">Execution Identifier</param>
        ''' <param name="pRecalculusFlag"></param>
        ''' <param name="pSampleClassRecalculated"></param>
        ''' <param name="pManualRecalculationsFlag"></param>
        ''' <param name="pExecutionRow">Row of typed DataSet ExecutionsDS (subtable twksWSExecutionsRow) containing all data of the Execution
        '''                             to calculate. Optional parameter; when informed, it is not needed to calling function GetExecution to
        '''                             get the data of the informed ExecutionID</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 09/07/2012 - Optimization of function CalculateExecution
        ''' Modified by: SA 16/11/2012 - After calling function SaveExecutionResults, update ExecutionStatus = CLOSED for the replicate in process
        ''' </remarks>
        Public Function CalculateExecutionNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                              ByVal pExecutionID As Integer, ByVal pRecalculusFlag As Boolean, ByVal pSampleClassRecalculated As String, _
                                              Optional ByVal pManualRecalculationsFlag As Boolean = False, _
                                              Optional ByVal pExecutionRow As ExecutionsDS.twksWSExecutionsRow = Nothing) As GlobalDataTO
            Dim globalData As New GlobalDataTO

            Try
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                'Dim StartTime As DateTime = Now
                'Dim myLogAcciones As New ApplicationLogManager()
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                Dim myExecutionsDS As New ExecutionsDS
                Dim myExecutionsDelegate As New ExecutionsDelegate

                If (Not pExecutionRow Is Nothing) Then
                    myExecutionsDS.twksWSExecutions.ImportRow(pExecutionRow)
                    globalData.HasError = False
                Else
                    'Get data of the Execution to calculate
                    globalData = myExecutionsDelegate.GetExecution(Nothing, pExecutionID, pAnalyzerID, pWorkSessionID)
                    If (Not globalData.HasError AndAlso Not globalData.SetDatos Is Nothing) Then
                        myExecutionsDS = DirectCast(globalData.SetDatos, ExecutionsDS)
                    End If
                End If

                If (Not globalData.HasError) Then
                    If (pRecalculusFlag) Then myManualRecalculationsFlag = pManualRecalculationsFlag

                    'Initialize internal structures
                    globalData = InitNEW(Nothing, myExecutionsDS, pRecalculusFlag, pSampleClassRecalculated)
                    If (globalData.HasError) Then
                        globalData = myClassGlobalResult
                        Exit Try
                    End If

                    'Execute Calculations
                    globalData = Me.ExecuteCalculations()
                    If (globalData.HasError) Then
                        globalData = myClassGlobalResult
                        Exit Try
                    End If

                    'Postposed!!!
                    'globalData = Me.SaveWaterResults()
                    'If globalData.HasError Then
                    '    globalData = myClassGlobalResult
                    '    Exit Try
                    'End If

                    'Save results
                    globalData = Me.SaveExecutionResults(Nothing)
                    If (Not globalData.HasError) Then
                        'After saving the result for the replicate, update the Execution Status in the local DS
                        myExecutionsDS.twksWSExecutions.First.ExecutionStatus = "CLOSED"
                    Else
                        globalData = myClassGlobalResult
                        Exit Try
                    End If

                    'Calculate & save results remarks
                    globalData = Me.GenerateResultsRemarks(Nothing)
                    If (globalData.HasError) Then
                        globalData = myClassGlobalResult
                        Exit Try
                    End If

                    'Calculated Tests operations (if execution OrderTestID is related with a calculated test orderTestID)
                    If (preparation(0).SampleClass = "PATIENT") Then  'Calculated test only for patients
                        Dim myCalcTestsDelegate As New OperateCalculatedTestDelegate
                        myCalcTestsDelegate.AnalyzerID = myAnalyzerID
                        myCalcTestsDelegate.WorkSessionID = myWorkSessionID

                        globalData = myCalcTestsDelegate.ExecuteCalculatedTest(Nothing, myOrderTestID, myManualRecalculationsFlag)
                        If (globalData.HasError) Then
                            Exit Try
                        End If
                    End If

                    'Call recalculations mode if new blank or calibrator is received
                    If (Not common(0).RecalculationFlag) Then
                        If (preparation(0).SampleClass = "BLANK" OrElse preparation(0).SampleClass = "CALIB") Then
                            Dim myRecalculations As New RecalculateResultsDelegate
                            myRecalculations.AnalyzerModel = myAnalyzerModel

                            'Get all affected Executions that have to be recalculated
                            globalData = myExecutionsDelegate.GetClosedExecutionsRelatedNEW(Nothing, myExecutionsDS.twksWSExecutions.First.AnalyzerID, _
                                                                                            myExecutionsDS.twksWSExecutions.First.WorkSessionID, _
                                                                                            myExecutionsDS.twksWSExecutions.First.OrderTestID, _
                                                                                            myExecutionsDS.twksWSExecutions.First.RerunNumber)
                            If (Not globalData.HasError AndAlso Not globalData.SetDatos Is Nothing) Then
                                Dim myExecsToRecalculateDS As ExecutionsDS = DirectCast(globalData.SetDatos, ExecutionsDS)

                                Dim myExecRow As ExecutionsDS.vwksWSExecutionsResultsRow = MoveSubTableInExecutionsDS(myExecutionsDS.twksWSExecutions.First)
                                globalData = myRecalculations.RecalculateResultsNEW(Nothing, myExecRow, myExecsToRecalculateDS.vwksWSExecutionsResults.First, _
                                                                                    False, False)
                            End If

                            'globalData = myRecalculations.RecalculateResults(Nothing, pAnalyzerID, pWorkSessionID, pExecutionID, False)
                        End If
                    End If
                End If

                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                'myLogAcciones.CreateLogActivity("Calculate Execution NEW: Id [" & pExecutionID.ToString & "] " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "CalculationsDelegate.CalculateExecutionNEW", EventLogEntryType.Information, False)
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Catch ex As Exception
                Me.CatchLaunched("CalculateExecutionNEW", ex)
            End Try
            Return globalData
        End Function

        ''' <summary>
        ''' Initializes structures needed to calculate Executions: Sample information, Test information, Calibration information and 
        ''' the rest of needed general information
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExecutionsDS">Typed DataSet ExecutionsDS containing data of the Execution to be calculated</param>
        ''' <param name="pRecalculusFlag"></param>
        ''' <param name="pSampleClassRecalculated"></param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 09/07/2012 - Optimization of function Init
        ''' </remarks>
        Private Function InitNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionsDS As ExecutionsDS, _
                                 ByVal pRecalculusFlag As Boolean, ByVal pSampleClassRecalculated As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                Dim myExecToCalculateRow As ExecutionsDS.twksWSExecutionsRow = pExecutionsDS.twksWSExecutions.First

                'Initialize GLOBAL CLASS Variables
                ReDim myExecutionID(0)
                myAnalyzerID = myExecToCalculateRow.AnalyzerID
                myWorkSessionID = myExecToCalculateRow.WorkSessionID
                myExecutionID(0) = myExecToCalculateRow.ExecutionID
                myOrderTestID = myExecToCalculateRow.OrderTestID
                myRerunNumber = myExecToCalculateRow.RerunNumber

                'Verify if the Execution is for a multipoint Calibrator
                Dim myExecutionsDelegate As New ExecutionsDelegate
                Dim mySampleClass As String = myExecToCalculateRow.SampleClass
                Dim myNumOfCalibrators As Integer = 1
                If (mySampleClass = "CALIB") Then
                    resultData = myExecutionsDelegate.GetNumberOfMultitItemNEW(Nothing, myAnalyzerID, myWorkSessionID, myOrderTestID, _
                                                                               myRerunNumber)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        myNumOfCalibrators = CType(resultData.SetDatos, Integer)
                    Else
                        myClassGlobalResult = resultData
                        Exit Try
                    End If
                End If

                If (Not resultData.HasError) Then
                    If (mySampleClass = "CALIB" AndAlso myNumOfCalibrators > 1) Then
                        'Process for multi-points Calibrators

                        '...Get Executions for all Calibrator points
                        resultData = myExecutionsDelegate.GetExecutionMultiItemsNEW(Nothing, myAnalyzerID, myWorkSessionID, myOrderTestID, _
                                                                                    myRerunNumber, myExecToCalculateRow.ReplicateNumber)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myMultiItemExecDS As ExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                            Dim myIndex As Integer = 0
                            For Each myExecutionRow As ExecutionsDS.twksWSExecutionsRow In myMultiItemExecDS.twksWSExecutions.Rows
                                ReDim Preserve myExecutionID(myIndex)
                                myExecutionID(myIndex) = myExecutionRow.ExecutionID

                                '...Initialize Common Structures
                                InitCommonNEW(myExecutionRow, myIndex, pRecalculusFlag, pSampleClassRecalculated)
                                If (myClassGlobalResult.HasError) Then Exit Try

                                '...Initialize Preparation Structure
                                InitPreparationNEW(myExecutionRow, myIndex)
                                If (myClassGlobalResult.HasError) Then Exit Try

                                myIndex = myIndex + 1
                            Next
                        Else
                            myClassGlobalResult = resultData
                            Exit Try
                        End If
                    Else
                        'Process for Blanks, Controls, Patients and single-point Calibrators

                        '...Initialize Common Structures
                        InitCommonNEW(myExecToCalculateRow, 0, pRecalculusFlag, pSampleClassRecalculated)
                        If (myClassGlobalResult.HasError) Then Exit Try

                        '...Initialize Preparation Structure
                        InitPreparationNEW(myExecToCalculateRow, 0)
                        If (myClassGlobalResult.HasError) Then Exit Try
                    End If

                    'Initialize Test/SampleType Structure
                    InitTest(Nothing, pExecutionsDS)
                    If (myClassGlobalResult.HasError) Then Exit Try

                    If (mySampleClass <> "BLANK") Then
                        'Initialize Calibration Structure
                        InitCalibrationNEW(myExecToCalculateRow)
                        If (myClassGlobalResult.HasError) Then Exit Try
                    End If

                    'Initialize data that needs several structures
                    InitFieldsRelatedWithSeveralStructures(Nothing, myNumOfCalibrators)
                    If (myClassGlobalResult.HasError) Then Exit Try
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalculationsDelegate.InitNEW", EventLogEntryType.Error, False)
            End Try
            Return myClassGlobalResult
        End Function

        ''' <summary>
        ''' Initializes the structure containing common information needed to calculate an Execution: value of SW Parameter and 
        ''' Base Lines for the Well used 
        ''' </summary>
        ''' <param name="pExecToCalculateRow">Row of typed DataSet ExecutionsDS (subtable twksWSExecutions) containing data of the 
        '''                                   Execution to be calculated</param>
        ''' <param name="pDimension">Array index to add</param>
        ''' <param name="pRecalculusFlag"></param>
        ''' <param name="pSampleClassRecalculated"></param>
        ''' <remarks>
        ''' Created by:  SA 09/07/2012 - Optimization of function InitCommon
        ''' Modified     AG 23/10/2013 - Initialize new properties in common structure (kineticsIncreaseInPause, useFirstR1SampleReadings)
        ''' </remarks>
        Private Sub InitCommonNEW(ByVal pExecToCalculateRow As ExecutionsDS.twksWSExecutionsRow, ByVal pDimension As Integer, _
                                  ByVal pRecalculusFlag As Boolean, ByVal pSampleClassRecalculated As String)
            Dim resultData As New GlobalDataTO

            Try
                ReDim Preserve common(pDimension)

                common(pDimension).RecalculationFlag = pRecalculusFlag
                common(pDimension).SampleClassRecalculated = pSampleClassRecalculated

                'AG 23/10/2013 Get analyzer model (if not informed) Task #1347
                If myAnalyzerModel = "" Then
                    Dim myAnalyzersDelegate As New AnalyzersDelegate
                    resultData = myAnalyzersDelegate.GetAnalyzerModel(Nothing, myAnalyzerID)
                    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                        Dim myAnalizerDS As New AnalyzersDS
                        myAnalizerDS = CType(resultData.SetDatos, AnalyzersDS)
                        myAnalyzerModel = myAnalizerDS.tcfgAnalyzers.Item(0).AnalyzerModel
                    Else
                        myClassGlobalResult = resultData
                        Exit Try
                    End If
                End If
                'AG 23/10/2013

                'Get all Parameters defined for the Analyzer according its model
                Dim myParams As New SwParametersDelegate
                resultData = myParams.ReadByAnalyzerModel(Nothing, myAnalyzerModel, myAnalyzerID)

                Dim baseline_Type_Calc As String = GlobalEnumerates.BaseLineType.STATIC.ToString() 'AG 11/11/2014 BA-2064
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    Dim myParametersDS As ParametersDS = DirectCast(resultData.SetDatos, ParametersDS)

                    If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then
                        'HIGHER LIMIT ABSORBANCE
                        If (myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.LIMIT_ABS.ToString).Count > 0) Then
                            common(pDimension).LimitAbs = myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.LIMIT_ABS.ToString).First.ValueNumeric
                        End If

                        'REACTION ROTOR PATH LENGTH
                        If (myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.PATH_LENGHT.ToString).Count > 0) Then
                            common(pDimension).PathLength = myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.PATH_LENGHT.ToString).First.ValueNumeric
                        End If

                        'KINETICS INCREASE (Time interval between two readings)
                        'AG 10/11/2014 BA-2082 filter by current analyzer model
                        If (myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.KINETICS_INCREASE.ToString AndAlso a.AnalyzerModel = myAnalyzerModel).Count > 0) Then
                            common(pDimension).KineticsIncrease = myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.KINETICS_INCREASE.ToString AndAlso a.AnalyzerModel = myAnalyzerModel).First.ValueNumeric
                        End If

                        'ANALYZER CYCLE MACHINE (Time interval between two preparations)
                        'AG 10/11/2014 BA-2082 filter by current analyzer model
                        If (myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.CYCLE_MACHINE.ToString AndAlso a.AnalyzerModel = myAnalyzerModel).Count > 0) Then
                            common(pDimension).CycleMachine = myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.CYCLE_MACHINE.ToString AndAlso a.AnalyzerModel = myAnalyzerModel).First.ValueNumeric
                        End If

                        'LINEAR CORRELATION FACTOR
                        If (myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.LINEAR_CORRELATION_FACTOR.ToString).Count > 0) Then
                            common(pDimension).LinearCorrelationFactor = myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.LINEAR_CORRELATION_FACTOR.ToString).First.ValueNumeric
                        End If

                        'REAGENT1 READINGS
                        If (myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.REAGENT1_READINGS.ToString).Count > 0) Then
                            common(pDimension).Reagent1Readings = CInt(myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.REAGENT1_READINGS.ToString).First.ValueNumeric)
                        End If

                        '% OF MAXIMUM EXTRAPOLATION ALLOWED
                        If (myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.MAX_EXTRAPOLATION.ToString).Count > 0) Then
                            common(pDimension).ExtrapolationMaximum = CInt(myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.MAX_EXTRAPOLATION.ToString).First.ValueNumeric)
                        End If

                        'CALIBRATION CURVE NUMBER OF POINTS
                        If (myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.CURVE_POINTS_NUMBER.ToString).Count > 0) Then
                            calibrator.Curve.CurvePointsNumber = CInt(myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.CURVE_POINTS_NUMBER.ToString).First.ValueNumeric)
                        End If

                        'AG 23/10/2013 Task #1347
                        'AG 10/11/2014 BA-2082 filter by analyzer model
                        'KINETICS INCREASE in pause (Time interval between two readings in pause mode)
                        If (myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.SS_KINETICS_INCREASE.ToString AndAlso a.AnalyzerModel = myAnalyzerModel).Count > 0) Then
                            common(pDimension).KineticsIncreaseInPause = myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.SS_KINETICS_INCREASE.ToString AndAlso a.AnalyzerModel = myAnalyzerModel).First.ValueNumeric
                        End If

                        'R1SAMPLE1STREADINGSFORCALC (Determine the valid readings for section R1 + S affected for pause mode)
                        '= 1 means the 33 first
                        '= 0 means the 33 last
                        If (myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.R1SAMPLE1STREADINGSFORCALC.ToString).Count > 0) Then
                            common(pDimension).UseFirstR1SampleReadings = CInt(myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.R1SAMPLE1STREADINGSFORCALC.ToString).First.ValueNumeric)
                        End If

                        'SwReadingsOffset
                        'AG 10/11/2014 BA-2082 filter by model
                        If (myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.SW_READINGSOFFSET.ToString AndAlso a.AnalyzerModel = myAnalyzerModel).Count > 0) Then
                            common(pDimension).SwReadingsOffset = CInt(myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.SW_READINGSOFFSET.ToString AndAlso a.AnalyzerModel = myAnalyzerModel).First.ValueNumeric)
                        End If
                        'AG 23/10/2013

                        'AG 11/11/2014 BA-2064 get the base line type for calculations
                        If (myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.BL_TYPE_FOR_CALCULATIONS.ToString AndAlso a.AnalyzerModel = myAnalyzerModel).Count > 0) Then
                            baseline_Type_Calc = myParametersDS.tfmwSwParameters.ToList.Where(Function(a) a.ParameterName = GlobalEnumerates.SwParameters.BL_TYPE_FOR_CALCULATIONS.ToString AndAlso a.AnalyzerModel = myAnalyzerModel).First.ValueText.ToString
                        End If

                    Else
                        myClassGlobalResult = resultData
                        Exit Try
                    End If
                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If

                'AG 24/10/2013 Task #1347
                Dim myLimits As New FieldLimitsDelegate
                resultData = myLimits.GetAllList(Nothing, myAnalyzerModel)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    Dim myLimitsDS As FieldLimitsDS = DirectCast(resultData.SetDatos, FieldLimitsDS)
                    If (myLimitsDS.tfmwFieldLimits.Rows.Count > 0) Then
                        'DefaultReadNumberR2Added, NumberOfReadingsReadingsWithR2
                        'AG 10/11/2014 BA-2082 filter by model
                        If (myLimitsDS.tfmwFieldLimits.ToList.Where(Function(a) a.LimitID = GlobalEnumerates.FieldLimitsEnum.READING2_CYCLES.ToString AndAlso a.AnalyzerModel = myAnalyzerModel).Count > 0) Then
                            common(pDimension).DefaultReadNumberR2Added = CInt(myLimitsDS.tfmwFieldLimits.ToList.Where(Function(a) a.LimitID = GlobalEnumerates.FieldLimitsEnum.READING2_CYCLES.ToString AndAlso a.AnalyzerModel = myAnalyzerModel).First.MinValue) - common(pDimension).SwReadingsOffset
                            common(pDimension).NumberOfReadingsWithR2 = CInt(myLimitsDS.tfmwFieldLimits.ToList.Where(Function(a) a.LimitID = GlobalEnumerates.FieldLimitsEnum.READING2_CYCLES.ToString AndAlso a.AnalyzerModel = myAnalyzerModel).First.MaxValue) _
                                                                        - CInt(myLimitsDS.tfmwFieldLimits.ToList.Where(Function(a) a.LimitID = GlobalEnumerates.FieldLimitsEnum.READING2_CYCLES.ToString AndAlso a.AnalyzerModel = myAnalyzerModel).First.MinValue) _
                                                                        + 1
                        End If
                    End If
                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If
                'AG 24/10/2013

                'Get BaseLine values
                '** Dark values are get from twksWSBLines
                '** Light values are get from twksWSBLinesByWell
                Dim myExecutionWell As Integer = 1
                If (Not pExecToCalculateRow.IsWellUsedNull) Then myExecutionWell = pExecToCalculateRow.WellUsed

                If (Not pExecToCalculateRow.IsBaseLineIDNull) Then common(pDimension).BaseLineID = pExecToCalculateRow.BaseLineID
                If (Not pExecToCalculateRow.IsAdjustBaseLineIDNull) Then common(pDimension).AdjustBaseLineID = pExecToCalculateRow.AdjustBaseLineID

                If (Not pExecToCalculateRow.IsBaseLineIDNull AndAlso Not pExecToCalculateRow.IsAdjustBaseLineIDNull) Then

                    'AG 11/11/2014 BA-2064 read the parameter that informs how works the model
                    'resultData = GetBaseLineValues(Nothing, myAnalyzerID, myWorkSessionID, common(pDimension).BaseLineID, myExecutionWell, common(pDimension).AdjustBaseLineID, GlobalEnumerates.BaseLineType.DYNAMIC.ToString)
                    resultData = GetBaseLineValues(Nothing, myAnalyzerID, myWorkSessionID, common(pDimension).BaseLineID, myExecutionWell, common(pDimension).AdjustBaseLineID, baseline_Type_Calc.ToString)

                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        Dim myBaseLineDS As BaseLinesDS = DirectCast(resultData.SetDatos, BaseLinesDS)

                        ReDim common(pDimension).BaseLine(myBaseLineDS.twksWSBaseLines.Rows.Count - 1)
                        ReDim common(pDimension).DarkCurrent(myBaseLineDS.twksWSBaseLines.Rows.Count - 1)
                        ReDim common(pDimension).RefDarkCurrent(myBaseLineDS.twksWSBaseLines.Rows.Count - 1)
                        ReDim common(pDimension).RefBaseLine(myBaseLineDS.twksWSBaseLines.Rows.Count - 1)

                        Dim myIndex As Integer = 0
                        For Each myBaseLineRow As BaseLinesDS.twksWSBaseLinesRow In myBaseLineDS.twksWSBaseLines.Rows
                            If (myIndex = 0) Then common(pDimension).BaseLineWell = myBaseLineDS.twksWSBaseLines(myIndex).WellUsed

                            common(pDimension).BaseLine(myIndex) = myBaseLineRow.MainLight
                            common(pDimension).DarkCurrent(myIndex) = myBaseLineRow.MainDark
                            common(pDimension).RefBaseLine(myIndex) = myBaseLineRow.RefLight
                            common(pDimension).RefDarkCurrent(myIndex) = myBaseLineRow.RefDark

                            myIndex = myIndex + 1
                        Next myBaseLineRow
                    Else
                        myClassGlobalResult = resultData
                    End If
                End If
            Catch ex As Exception
                Me.CatchLaunched("InitCommonNEW", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Initializes the structure containing preparation information needed to calculate an Execution
        ''' </summary>
        ''' <param name="pExecToCalculateRow">Row of typed DataSet ExecutionsDS (subtable twksWSExecutions) containing data of the 
        '''                                   Execution to be calculated</param>
        ''' <param name="pDimension">Array index to add</param>
        ''' <remarks>
        ''' Created by:  SA 09/07/2012 - Optimization of function InitPreparation
        ''' Modified     AG 23/10/2013 - initialize new properties in preparation structure (PausedReadings())
        ''' </remarks>
        Private Sub InitPreparationNEW(ByVal pExecToCalculateRow As ExecutionsDS.twksWSExecutionsRow, ByVal pDimension As Integer)
            Dim resultData As New GlobalDataTO
            Try
                ReDim Preserve preparation(pDimension)

                preparation(pDimension).PreparationID = pExecToCalculateRow.ExecutionID
                preparation(pDimension).SampleClass = pExecToCalculateRow.SampleClass

                preparation(pDimension).MultiItemNumber = pExecToCalculateRow.MultiItemNumber
                preparation(pDimension).ReRun = pExecToCalculateRow.RerunNumber

                If (Not pExecToCalculateRow.IsReplicateNumberNull) Then preparation(pDimension).ReplicateID = pExecToCalculateRow.ReplicateNumber
                If (Not pExecToCalculateRow.IsWellUsedNull) Then preparation(pDimension).WellUsed = pExecToCalculateRow.WellUsed
                If (Not pExecToCalculateRow.IsThermoWarningFlagNull) Then preparation(pDimension).ThermoWarningFlag = pExecToCalculateRow.ThermoWarningFlag
                If (Not pExecToCalculateRow.IsClotValueNull) Then preparation(pDimension).PossibleClot = pExecToCalculateRow.ClotValue

                preparation(pDimension).PostDilutionType = "NONE"
                If (Not pExecToCalculateRow.IsPostDilutionTypeNull) Then preparation(pDimension).PostDilutionType = pExecToCalculateRow.PostDilutionType

                preparation(pDimension).TestID = pExecToCalculateRow.TestID
                If (Not pExecToCalculateRow.IsSampleTypeNull) Then preparation(pDimension).SampleType = pExecToCalculateRow.SampleType
                If (Not pExecToCalculateRow.IsReplicatesTotalNumNull) Then preparation(pDimension).MaxReplicates = pExecToCalculateRow.ReplicatesTotalNum
                If (pExecToCalculateRow.SampleClass = "CTRL") Then preparation(pDimension).ControlID = pExecToCalculateRow.ControlID

                'Read the reaction complete readings
                Dim ReadingsData As New WSReadingsDelegate
                'AG 23/10/2013 Task #1347 (add the last 6 parameters)
                resultData = ReadingsData.GetReadingsByExecutionID(Nothing, myAnalyzerID, myWorkSessionID, pExecToCalculateRow.ExecutionID, True, _
                                                                   myAnalyzerModel, pExecToCalculateRow.TestID, common(pDimension).UseFirstR1SampleReadings, _
                                                                   common(pDimension).DefaultReadNumberR2Added, common(pDimension).NumberOfReadingsWithR2, common(pDimension).SwReadingsOffset)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    Dim myReadingsDS As twksWSReadingsDS = DirectCast(resultData.SetDatos, twksWSReadingsDS)

                    If (myReadingsDS.twksWSReadings.Rows.Count > 0) Then
                        Dim myIndex As Integer = 0
                        For Each ReadingsRow As twksWSReadingsDS.twksWSReadingsRow In myReadingsDS.twksWSReadings.Rows
                            ReDim Preserve preparation(pDimension).Readings(myIndex)
                            ReDim Preserve preparation(pDimension).RefReadings(myIndex)
                            ReDim Preserve preparation(pDimension).PausedReadings(myIndex) 'AG 23/10/2013 Task #1347

                            preparation(pDimension).Readings(myIndex) = ReadingsRow.MainCounts
                            preparation(pDimension).RefReadings(myIndex) = ReadingsRow.RefCounts
                            preparation(pDimension).PausedReadings(myIndex) = ReadingsRow.Pause 'AG 23/10/2013 Task #1347

                            myIndex = myIndex + 1
                        Next ReadingsRow
                    Else
                        myClassGlobalResult.HasError = True
                        myClassGlobalResult.ErrorCode = GlobalEnumerates.AbsorbanceErrors.INCORRECT_DATA.ToString
                    End If
                Else
                    myClassGlobalResult = resultData
                    Exit Try
                End If

                'Redim result arrays using the MaxReplicates value
                Dim MaxReplicates As Integer = preparation(pDimension).MaxReplicates

                ReDim preparation(pDimension).InitialReplicateAbs(MaxReplicates - 1)
                ReDim preparation(pDimension).MainFilterReplicateAbs(MaxReplicates - 1)
                ReDim preparation(pDimension).ReplicateAbs(MaxReplicates - 1)
                ReDim preparation(pDimension).Reagent1ReplicateAbs(common(0).Reagent1Readings - 1)
                ReDim preparation(pDimension).ReplicateConc(MaxReplicates - 1)
                ReDim preparation(pDimension).rKineticsReplicate(MaxReplicates - 1)
                ReDim preparation(pDimension).KineticsCurve(MaxReplicates - 1, 1)
                ReDim preparation(pDimension).SubstrateDepletionReplicate(MaxReplicates - 1)
                ReDim preparation(pDimension).ErrorReplicateAbs(MaxReplicates - 1)
                ReDim preparation(pDimension).ErrorReplicateConc(MaxReplicates - 1)
                ReDim preparation(pDimension).ReplicateDate(MaxReplicates - 1)
                ReDim preparation(pDimension).RepInUse(MaxReplicates - 1)
                ReDim preparation(pDimension).ReplicateAlarms(MaxReplicates - 1)
                ReDim preparation(pDimension).ErrorCurveReplicate(MaxReplicates - 1)
                ReDim preparation(pDimension).WorkingReagentReplicateAbs(MaxReplicates - 1) 'AG 12/07/2010
                ReDim preparation(pDimension).ReplicateExecutionID(MaxReplicates - 1) 'AG 14/08/2010
            Catch ex As Exception
                Me.CatchLaunched("InitPreparationNEW", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Initializes the structure containing calibration information needed to calculate an Execution
        ''' </summary>
        ''' <param name="pExecToCalculateRow">Row of typed DataSet ExecutionsDS (subtable twksWSExecutions) containing data of the 
        '''                                   Execution to be calculated</param>
        ''' <remarks>
        ''' Created by:  SA 09/07/2012 - Optimization of function InitCalibration
        ''' </remarks>
        Private Sub InitCalibrationNEW(ByVal pExecToCalculateRow As ExecutionsDS.twksWSExecutionsRow)
            Dim resultData As New GlobalDataTO

            Try
                calibrator.ErrorAbs = ""
                calibrator.ErrorCalibration = ""
                calibrator.ManualFactor = 1
                calibrator.ManualFactorFlag = False

                Dim myExecutionsDelegate As New ExecutionsDelegate
                If (calibrator.CalibrationType = "EXPERIMENT") Then
                    'Get data for EXPERIMENTAL CALIBRATOR
                    resultData = myExecutionsDelegate.GetCalibratorData(Nothing, pExecToCalculateRow.ExecutionID)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        Dim calibratorDataDS As TestSampleCalibratorDS = DirectCast(resultData.SetDatos, TestSampleCalibratorDS)

                        If (calibratorDataDS.tparTestCalibrators.Rows.Count = 1) Then
                            calibrator.CalibratorID = calibratorDataDS.tparTestCalibrators(0).CalibratorID
                            calibrator.NumberOfCalibrators = calibratorDataDS.tparTestCalibrators(0).NumberOfCalibrators
                            calibrator.ExpirationDate = calibratorDataDS.tparTestCalibrators(0).ExpirationDate
                        End If
                    Else
                        myClassGlobalResult = resultData
                        Exit Try
                    End If
                Else
                    'Get data for ALTERNATIVE CALIBRATOR
                    calibrator.SampleType = calibrator.AlternativeSampleType
                    resultData = myExecutionsDelegate.GetCalibratorData(Nothing, pExecToCalculateRow.ExecutionID, _
                                                                        calibrator.TestID, calibrator.SampleType)

                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        Dim calibratorDataDS As TestSampleCalibratorDS = DirectCast(resultData.SetDatos, TestSampleCalibratorDS)

                        If (calibratorDataDS.tparTestCalibrators.Rows.Count = 1) Then
                            If (calibratorDataDS.tparTestCalibrators.First.IsCalibratorFactorNull) Then
                                calibrator.CalibratorID = calibratorDataDS.tparTestCalibrators(0).CalibratorID
                                calibrator.NumberOfCalibrators = calibratorDataDS.tparTestCalibrators(0).NumberOfCalibrators
                                calibrator.ExpirationDate = calibratorDataDS.tparTestCalibrators(0).ExpirationDate  'AG 08/11/2010
                                'SA-TR 03/09/2012 -Set the calibrator type.
                                calibrator.CalibrationType = "EXPERIMENT"
                            Else
                                calibrator.Factor = calibratorDataDS.tparTestCalibrators(0).CalibratorFactor
                                'SA-TR 03/09/2012 -Set the calibrator type.
                                calibrator.CalibrationType = "FACTOR"
                            End If
                        End If
                    Else
                        myClassGlobalResult = resultData
                        Exit Try
                    End If
                End If

                If (calibrator.CalibrationType <> "FACTOR") Then 'AG 27/08/2010
                    Dim testCalibratorData As New TestCalibratorsDelegate
                    If (calibrator.NumberOfCalibrators > 1) Then
                        ReDim calibrator.Curve.CalibratorAbs(calibrator.NumberOfCalibrators - 1)
                        ReDim calibrator.Curve.ErrorAbs(calibrator.NumberOfCalibrators - 1)
                        ReDim calibrator.Curve.ConcCurve(calibrator.NumberOfCalibrators - 1)
                        ReDim calibrator.Curve.ErrorCurve(calibrator.NumberOfCalibrators - 1)
                        ReDim calibrator.Curve.Coefficient(3, 15) 'Idem Ax5
                    End If

                    resultData = testCalibratorData.GetTestCalibratorValuesNEW(Nothing, calibrator.TestID, calibrator.SampleType)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        Dim myTestCalibratorValuesData As TestCalibratorValuesDS = DirectCast(resultData.SetDatos, TestCalibratorValuesDS)

                        'Data of Calibration Curve is get from the first row in the returned DS (this fields have the same value for all rows)
                        If (Not myTestCalibratorValuesData.tparTestCalibratorValues(0).IsCurveGrowthTypeNull) Then calibrator.Curve.CurveGrowthType = myTestCalibratorValuesData.tparTestCalibratorValues(0).CurveGrowthType
                        If (Not myTestCalibratorValuesData.tparTestCalibratorValues(0).IsCurveTypeNull) Then calibrator.Curve.CurveType = myTestCalibratorValuesData.tparTestCalibratorValues(0).CurveType
                        If (Not myTestCalibratorValuesData.tparTestCalibratorValues(0).IsCurveAxisXTypeNull) Then calibrator.Curve.CurveAxisXType = myTestCalibratorValuesData.tparTestCalibratorValues(0).CurveAxisXType
                        If (Not myTestCalibratorValuesData.tparTestCalibratorValues(0).IsCurveAxisYTypeNull) Then calibrator.Curve.CurveAxisYType = myTestCalibratorValuesData.tparTestCalibratorValues(0).CurveAxisYType

                        Dim myIndex As Integer = 0
                        ReDim calibrator.TheoreticalConcentration(myTestCalibratorValuesData.tparTestCalibratorValues.Rows.Count - 1)
                        For Each row As TestCalibratorValuesDS.tparTestCalibratorValuesRow In myTestCalibratorValuesData.tparTestCalibratorValues
                            calibrator.TheoreticalConcentration(myIndex) = row.TheoricalConcentration

                            myIndex = myIndex + 1
                        Next
                    Else
                        myClassGlobalResult = resultData
                        Exit Try
                    End If
                End If

            Catch ex As Exception
                Me.CatchLaunched("InitCalibrationNEW", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Move data of an specific Execution from subtable twksWSExecutions in a typed DataSet ExecutionsDS to a subtable vwksWSExecutionsResultsRow
        ''' in the same typed DataSet - Needed when calling function RecalculateResultsDelegate.RecalculateResultsNEW 
        ''' </summary>
        ''' <param name="ptwksWSExecutionsRow">Row of typed DataSet ExecutionsDS (subtable twksWSExecutions)</param>
        ''' <returns>Row of typed DataSet ExecutionsDS (subtable vwksWSExecutionsResultsRow)</returns>
        ''' <remarks>
        ''' Created by:  SA 09/07/2012 
        ''' </remarks>
        Private Function MoveSubTableInExecutionsDS(ByVal ptwksWSExecutionsRow As ExecutionsDS.twksWSExecutionsRow) As ExecutionsDS.vwksWSExecutionsResultsRow
            Dim myExecutionsDS As New ExecutionsDS
            Dim myWSExecResultsRow As ExecutionsDS.vwksWSExecutionsResultsRow = Nothing

            Try
                myWSExecResultsRow = myExecutionsDS.vwksWSExecutionsResults.NewvwksWSExecutionsResultsRow
                With ptwksWSExecutionsRow
                    myWSExecResultsRow.ExecutionID = .ExecutionID
                    myWSExecResultsRow.AnalyzerID = .AnalyzerID
                    myWSExecResultsRow.WorkSessionID = .WorkSessionID
                    myWSExecResultsRow.OrderTestID = .OrderTestID
                    myWSExecResultsRow.MultiItemNumber = .MultiItemNumber
                    myWSExecResultsRow.ReplicateNumber = .ReplicateNumber
                    myWSExecResultsRow.RerunNumber = .RerunNumber
                    myWSExecResultsRow.ExecutionType = .ExecutionType
                    myWSExecResultsRow.ExecutionStatus = .ExecutionStatus
                    myWSExecResultsRow.WellUsed = .WellUsed
                    myWSExecResultsRow.BaseLineID = .BaseLineID
                    myWSExecResultsRow.AdjustBaseLineID = .AdjustBaseLineID
                    myWSExecResultsRow.TestType = .TestType
                    myWSExecResultsRow.TestID = .TestID
                    myWSExecResultsRow.TestName = .TestName
                    myWSExecResultsRow.SampleType = .SampleType
                    myWSExecResultsRow.SampleClass = .SampleClass
                    myWSExecResultsRow.OrderID = .OrderID

                    If (Not myWSExecResultsRow.IsInUseNull) Then myWSExecResultsRow.InUse = .InUse
                    If (Not myWSExecResultsRow.IsResultDateNull) Then myWSExecResultsRow.ResultDate = .ResultDate
                    If (Not myWSExecResultsRow.IsStatFlagNull) Then myWSExecResultsRow.StatFlag = .StatFlag
                    If (Not myWSExecResultsRow.IsPostDilutionTypeNull) Then myWSExecResultsRow.PostDilutionType = .PostDilutionType
                    If (Not myWSExecResultsRow.IsABS_ValueNull) Then myWSExecResultsRow.ABS_Value = .ABS_Value
                    If (Not myWSExecResultsRow.IsABS_ErrorNull) Then myWSExecResultsRow.ABS_Error = .ABS_Error
                    If (Not myWSExecResultsRow.IsABS_InitialNull) Then myWSExecResultsRow.ABS_Initial = .ABS_Initial
                    If (Not myWSExecResultsRow.IsABS_MainFilterNull) Then myWSExecResultsRow.ABS_MainFilter = .ABS_MainFilter
                    If (Not myWSExecResultsRow.IsCONC_ValueNull) Then myWSExecResultsRow.CONC_Value = .CONC_Value
                    If (Not myWSExecResultsRow.IsCONC_ErrorNull) Then myWSExecResultsRow.CONC_Error = .CONC_Error
                    If (Not myWSExecResultsRow.IsCONC_CurveErrorNull) Then myWSExecResultsRow.CONC_CurveError = .CONC_CurveError
                    If (Not myWSExecResultsRow.IsSubstrateDepletionNull) Then myWSExecResultsRow.SubstrateDepletion = .SubstrateDepletion
                    If (Not myWSExecResultsRow.IsAbs_WorkReagentNull) Then myWSExecResultsRow.Abs_WorkReagent = .Abs_WorkReagent
                    If (Not myWSExecResultsRow.IsSentNewRerunPostdilutionNull) Then myWSExecResultsRow.SentNewRerunPostdilution = .SentNewRerunPostdilution
                End With
            Catch ex As Exception
                Me.CatchLaunched("MoveSubTableInExecutionsDS", ex)
            End Try
            Return myWSExecResultsRow
        End Function
#End Region
    End Class

End Namespace
