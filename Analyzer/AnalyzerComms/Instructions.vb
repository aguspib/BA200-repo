Option Strict On
Option Explicit On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global.TO
'Imports Biosystems.Ax00.CommunicationsSwFw.ApplicationLayer
'Imports System.Drawing

Namespace Biosystems.Ax00.CommunicationsSwFw

    <ComClass()> Public Class Instructions

#Region "Declaration"
        Public ParameterList As New List(Of ParametersTO)

        'List with all the labels 
        Public InstructionList As New List(Of InstructionParameterTO)
#End Region

#Region "Constructor"
        Public Sub New()
            'fill the instruction list
            Dim myInstructionList As New InstructionTypesList
            InstructionList = myInstructionList.GetInstructionParameterList()
        End Sub
#End Region

#Region "Public Methods (utils & generate instructions to be sent)"

        ''' <summary>
        ''' Add a parameter to the instruction list
        ''' </summary>
        ''' <param name="pParameterID">Parameter identification </param>
        ''' <param name="pParameterValue">Parameter value (optional)</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  TR 13/04/2010
        ''' Modified by: AG 30/04/2010 - All instructions has the 2 firsts parameters: ParameterID = ParameterValue
        ''' </remarks>
        Public Function Add(ByVal pParameterID As String, Optional ByVal pParameterValue As String = "") As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myInstruccionTO As New ParametersTO
                'AG 30/04/2010 - If parameterID not empty inform the parameterID
                'myInstruccionTO.ParameterID = pParameterID

                ''validate if value is not empty.
                'If pParameterValue <> "" Then
                '    myInstruccionTO.ParameterValues = pParameterValue
                'End If

                ''add in to the list.
                'ParameterList.Add(myInstruccionTO)

                If pParameterID <> "" Then
                    myInstruccionTO.ParameterID = pParameterID

                    'validate if value is not empty.
                    If pParameterValue <> "" Then
                        myInstruccionTO.ParameterValues = pParameterValue
                    End If
                Else
                    If pParameterValue <> "" Then
                        myInstruccionTO.ParameterID = pParameterValue
                    End If
                End If

                If pParameterID <> "" Or pParameterValue <> "" Then
                    ParameterList.Add(myInstruccionTO)
                Else
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = "NOT_NULL_VALUE"
                End If
                'END AG 30/04/2010
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.Add", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Return the parameterlist
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  TR 13/04/2010
        ''' </remarks>
        Public Function GetParameterList() As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                myGlobalDataTO.SetDatos = ParameterList
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GetParameterList", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Remove a parameter or a parameter value depending the entry,
        ''' if the pParameter value is informed then this means then value 
        ''' has to be deleted. other wise the all parameter is delete from the list.
        ''' </summary>
        ''' <param name="pParameterID"></param>
        ''' <param name="pParameterValue"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  TR 13/04/2010
        ''' </remarks>
        Public Function Remove(ByVal pParameterID As String, Optional ByVal pParameterValue As String = "") As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myQueryList As New List(Of ParametersTO)
            Try
                If ParameterList.Count > 0 Then
                    'Validate if the pParameterValue is informe to delete the specific value

                    myQueryList = (From a In ParameterList _
                                   Where a.ParameterID = pParameterID _
                                   Select a).ToList()
                    If myQueryList.Count > 0 Then
                        If pParameterValue <> "" Then
                            'Multivalue elimination no implemented.
                        Else
                            'Remove the parameter from the list.
                            ParameterList.Remove(myQueryList.First())
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.Remove", EventLogEntryType.Error, False)
            End Try
            myQueryList = Nothing 'AG 02/08/2012 - free memory
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Clear the Parameter list
        ''' </summary>
        ''' <remarks>
        ''' Created by:  TR 13/04/2010
        ''' </remarks>
        Public Sub Clear()
            ParameterList.Clear()
        End Sub

        ''' <summary>
        ''' Genereate the ISE preparation Parameters with the corresponding values.
        ''' </summary>
        ''' <param name="pExecutionID">Execution ID</param>
        ''' <returns>Return a preparation parammeter list fill with the data</returns>
        ''' <remarks>
        ''' Created by:  TR 20/01/2011
        ''' Modified by: TR 21/05/2012 - Return Error when instruction incomplete
        '''              AG 25/05/2012 - Complete cases for return error when instruction incomplete: vm1
        '''              SA 10/07/2012 - Inform the SampleClass parameter when calling function GetAffectedISEExecutions in ExecutionsDelegate
        '''              XB 04/06/2013 - change sample types comparisons in order to prepare the code in front of DB changes
        ''' </remarks>
        Public Function GenerateISEPreparation(ByVal pExecutionID As Integer) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myTestID As Integer = 0
                Dim mySampleType As String = String.Empty
                Dim myAnalyzerID As String = String.Empty
                Dim mySampleVolume As Integer = 0
                Dim myOrderTestID As Integer = 0
                Dim mySampleClass As String = String.Empty
                Dim myOrderTestDS As New OrderTestsDS
                Dim MyOrderTestDelegate As New OrderTestsDelegate
                Dim myWSExecutionDS As New ExecutionsDS
                Dim myExecutionDelegate As New ExecutionsDelegate
                Dim myISETestSampleDS As New ISETestSamplesDS
                Dim mytwksWSPreparationsDAO As New twksWSPreparationsDAO
                Dim myISETestSampleDelegate As New ISETestSamplesDelegate
                Dim myPreparationPositionDS As New PreparationsPositionDataDS
                Dim myPreparationPositionDataDAO As New vwksPreparationsPositionDataDAO
                Dim myPreparationParameterList As New List(Of InstructionParameterTO)
                Dim qPositionData As New List(Of PreparationsPositionDataDS.vwksPreparationsPositionDataRow)

                'Get the execution data 
                myGlobalDataTO = myExecutionDelegate.GetExecution(Nothing, pExecutionID)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myWSExecutionDS = DirectCast(myGlobalDataTO.SetDatos, ExecutionsDS)

                    If (myWSExecutionDS.twksWSExecutions.Rows.Count > 0) Then
                        'Get OrderTestID, Analyzer and SampleClass from the Execution being processed
                        myOrderTestID = myWSExecutionDS.twksWSExecutions(0).OrderTestID
                        myAnalyzerID = myWSExecutionDS.twksWSExecutions(0).AnalyzerID
                        mySampleClass = myWSExecutionDS.twksWSExecutions(0).SampleClass

                        'Get data needed to prepare the instruction for the OrderTestID
                        myGlobalDataTO = myPreparationPositionDataDAO.ReadByOrderTestID(Nothing, myOrderTestID)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myPreparationPositionDS = DirectCast(myGlobalDataTO.SetDatos, PreparationsPositionDataDS)
                        Else
                            Exit Try
                        End If

                        'Get all data of the OrderTestID
                        myGlobalDataTO = MyOrderTestDelegate.GetOrderTest(Nothing, myOrderTestID)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

                            If (myOrderTestDS.twksOrderTests.Rows.Count > 0) Then
                                myTestID = myOrderTestDS.twksOrderTests(0).TestID
                                mySampleType = myOrderTestDS.twksOrderTests(0).SampleType

                                'Get data of the ISE Test/SampleType
                                myGlobalDataTO = myISETestSampleDelegate.GetListByISETestID(Nothing, myTestID, mySampleType)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myISETestSampleDS = DirectCast(myGlobalDataTO.SetDatos, ISETestSamplesDS)
                                Else
                                    Exit Try
                                End If

                                'If the OrderTestID is for an ISE Control, then inform the SampleType in PreparationDataDS
                                If (mySampleClass = "CTRL") Then
                                    myPreparationPositionDS.vwksPreparationsPositionData(0).SampleType = mySampleType
                                End If
                            End If
                        Else
                            Exit Try
                        End If

                        'Get the instruction for ISETEST
                        myPreparationParameterList = GetInstructionParameter("ISETEST")
                        If myPreparationParameterList.Count > 0 Then
                            'Go through each intruction and fill with te corresponding value.
                            For Each myInstructionTO As InstructionParameterTO In myPreparationParameterList

                                Select Case myInstructionTO.ParameterIndex
                                    Case 1 'Axxx Analyzer Description
                                        myInstructionTO.ParameterValue = "A400" '1st & 2on characters: model, 3th & 4th characters: analyzer number
                                        Exit Select

                                    Case 2 'Instruction Code
                                        myInstructionTO.ParameterValue = myInstructionTO.InstructionType
                                        Exit Select

                                    Case 3 'TI -Test/Preparation Type
                                        Dim myPreparationType As String = ""

                                        ' XB 04/06/2013
                                        'If myPreparationPositionDS.vwksPreparationsPositionData(0).SampleType.Trim() = "SER" OrElse _
                                        '    myPreparationPositionDS.vwksPreparationsPositionData(0).SampleType.Trim() = "PLM" Then
                                        '    myPreparationType = "1"

                                        'ElseIf myPreparationPositionDS.vwksPreparationsPositionData(0).SampleType.Trim() = "URI" Then
                                        '    myPreparationType = "2"

                                        'Else
                                        '    'RH 27/06/2012 -If no value then return error.
                                        '    myGlobalDataTO.HasError = True
                                        '    Exit For

                                        'End If
                                        If myPreparationPositionDS.vwksPreparationsPositionData(0).SampleType.Contains("URI") Then
                                            myPreparationType = "2"
                                        Else
                                            myPreparationType = "1"
                                        End If
                                        ' XB 04/06/2013

                                        myInstructionTO.ParameterValue = myPreparationType

                                        Exit Select

                                    Case 4 'ID -Preparation Identifier
                                        'Get the id
                                        myGlobalDataTO = mytwksWSPreparationsDAO.GeneratePreparationID(Nothing)

                                        If Not myGlobalDataTO.HasError Then
                                            myInstructionTO.ParameterValue = myGlobalDataTO.SetDatos.ToString()
                                            ''AG 28/09/2012 - auxiliary code for test incomplete instruction
                                            'If myInstructionTO.ParameterValue = "7" Then
                                            '    myGlobalDataTO.HasError = True
                                            '    Exit For
                                            'End If
                                            ''AG 28/09/2012
                                        Else
                                            Exit For 'AG 27/06/2012
                                        End If

                                        Exit Select

                                    Case 5 'M1 -Sample Tube Position
                                        'AG 03/08/2011 - Add condition Status <> LOCKED AND (BarcodeStatus IS NULL OR BarcodeStatus <> ‘ERROR’ )
                                        qPositionData = (From a In myPreparationPositionDS.vwksPreparationsPositionData _
                                                         Where a.TubeContent.Trim = myWSExecutionDS.twksWSExecutions(0).SampleClass.Trim _
                                                         AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso _
                                                         (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") Select a).ToList()

                                        If qPositionData.Count > 0 Then
                                            myInstructionTO.ParameterValue = qPositionData.First().CellNumber.ToString()
                                        Else
                                            'myInstructionTO.ParameterValue = "0"
                                            'TR 21/05/2012 -If not value then return error.
                                            myGlobalDataTO.HasError = True
                                            Exit For
                                        End If
                                        Exit Select

                                    Case 6 'TM1 -Sample Tube type
                                        'AG 03/08/2011 - Add condition Status <> LOCKED AND (BarcodeStatus IS NULL OR BarcodeStatus <> ‘ERROR’ )
                                        qPositionData = (From a In myPreparationPositionDS.vwksPreparationsPositionData _
                                                         Where (a.TubeContent = "PATIENT" OrElse a.TubeContent = "CTRL") _
                                                         AndAlso a.Status.Trim <> "DEPLETED" _
                                                         AndAlso a.Status.Trim <> "LOCKED" _
                                                         AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                                         Select a).ToList()

                                        If qPositionData.Count > 0 Then
                                            myInstructionTO.ParameterValue = GetBottleCode(qPositionData.First().TubeType)
                                        Else
                                            'myInstructionTO.ParameterValue = "0"
                                            'TR 21/05/2012 -If not value then return error.
                                            myGlobalDataTO.HasError = True
                                            Exit For
                                        End If
                                        Exit Select

                                    Case 7 'RM1 -Sample Rotor Type.
                                        myInstructionTO.ParameterValue = "0"
                                        Exit Select

                                    Case 8 'VM1 -Sample Volume.
                                        'Need to do the convertion into steps.
                                        If myISETestSampleDS.tparISETestSamples.Count > 0 Then
                                            'mySampleVolume = CInt(CalculateISeVolumeSteps(myISETestSampleDS.tparISETestSamples(0).ISE_Volume))
                                            'TR 01/02/2012 -Load the ISE_Volume Value from Software parammeter depending the sample type.
                                            myGlobalDataTO = GetISEVolumeByType(myAnalyzerID, myISETestSampleDS.tparISETestSamples(0).SampleType)
                                            If Not myGlobalDataTO.HasError Then
                                                Dim myISE_VOL As Single = Convert.ToSingle(myGlobalDataTO.SetDatos)
                                                mySampleVolume = CInt(CalculateISeVolumeSteps(myISE_VOL))
                                            End If
                                            'TR 01/02/2012 -END.
                                        End If

                                        myInstructionTO.ParameterValue = mySampleVolume.ToString()

                                        'AG 25/05/2012 - If sample volume = 0 return error
                                        If mySampleVolume = 0 Then
                                            myGlobalDataTO.HasError = True 'Invalid value
                                            Exit For
                                        End If
                                        'AG 25/05/2012

                                        Exit Select

                                    Case Else
                                        Exit Select
                                End Select
                            Next

                        Else
                            myGlobalDataTO.HasError = True
                            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.NOT_CASE_FOUND.ToString

                        End If

                        If Not myGlobalDataTO.HasError Then
                            myGlobalDataTO.SetDatos = myPreparationParameterList
                        Else
                            'TR 22/05/2012 -If error the Lock all execution related to ISE
                            myGlobalDataTO = myExecutionDelegate.GetAffectedISEExecutions(Nothing, myWSExecutionDS.twksWSExecutions(0).AnalyzerID, _
                                                                                          myWSExecutionDS.twksWSExecutions(0).WorkSessionID, pExecutionID, _
                                                                                          myWSExecutionDS.twksWSExecutions(0).SampleClass)
                            If Not myGlobalDataTO.HasError Then
                                myWSExecutionDS = DirectCast(myGlobalDataTO.SetDatos, ExecutionsDS)
                                'Go throug each related executioin and  Set the ExecutionStatus to LOCKED.

                                'Dim myLogAcciones As New ApplicationLogManager()

                                For Each myWSExec As ExecutionsDS.twksWSExecutionsRow In myWSExecutionDS.twksWSExecutions.Rows
                                    myWSExec.BeginEdit()
                                    myWSExec.ExecutionStatus = "LOCKED"
                                    myWSExec.EndEdit()
                                    GlobalBase.CreateLogActivity("Instruction with empty fields. Sw locks execution: " & myWSExec.ExecutionID.ToString, "Instructions.GenerateISEPreparation", EventLogEntryType.Information, False) 'AG 30/05/2012
                                Next
                                'Update Status on Database.
                                myGlobalDataTO = myExecutionDelegate.UpdateStatus(Nothing, myWSExecutionDS)
                            End If
                            'After locking make sure send error to next level
                            myGlobalDataTO.HasError = True
                            myGlobalDataTO.ErrorCode = "EMPTY_FIELDS" 'AG 27/09/2012 - inform not system error, it is a protection
                            'TR 22/05/2012 -END.
                        End If
                    End If
                End If
                qPositionData = Nothing 'AG 02/08/2012 - free memory
                myPreparationParameterList = Nothing 'AG 02/08/2012 - free memory

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateISEPreparation", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the ISE Volume on the SwPArammeter table by the SampleType.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY:  TR 01/02/2012 -Get the ISE Volume defined on Parameters programming table 
        ''' MODIFIED BY: XB 04/06/2013 - change sample types comparisons in order to prepare the code in front of DB changes
        ''' </remarks>
        Private Function GetISEVolumeByType(ByVal pAnalyzerID As String, ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim paramsDlg As New SwParametersDelegate
                Dim paramsDS As New ParametersDS
                Dim myParammeter As String = ""

                ' XB 04/06/2013
                'Select Case pSampleType
                '    Case "SER"
                '        myParammeter = GlobalEnumerates.SwParameters.ISE_SER_VOL.ToString()
                '        Exit Select
                '    Case "URI"
                '        myParammeter = GlobalEnumerates.SwParameters.ISE_URI_VOL.ToString()
                '        Exit Select
                '    Case "PLM"
                '        myParammeter = GlobalEnumerates.SwParameters.ISE_PLM_VOL.ToString()
                '        Exit Select
                'End Select
                If pSampleType.Contains("URI") Then
                    myParammeter = GlobalEnumerates.SwParameters.ISE_URI_VOL.ToString()
                Else
                    myParammeter = GlobalEnumerates.SwParameters.ISE_SER_VOL.ToString()
                End If
                ' XB 04/06/2013

                myGlobalDataTO = paramsDlg.GetParameterByAnalyzer(Nothing, pAnalyzerID, myParammeter, True)
                If Not myGlobalDataTO.HasError Then
                    paramsDS = CType(myGlobalDataTO.SetDatos, ParametersDS)
                    myGlobalDataTO.SetDatos = Convert.ToSingle(paramsDS.tfmwSwParameters(0).ValueNumeric)
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GetISEVolumeByType", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate the Peration pameters with the corresponding values.
        ''' </summary>
        ''' <param name="pExecutionID"></param>
        ''' <param name="pAnalyzerModel"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  TR 13/04/2010
        ''' Modified AG 30/12/2010 - get NOT DEPLETED positions!!!
        '''          TR 19/01/2011 - Prepare the Generation creating the preparation depending on the predilution mode, 
        '''                          SampleClass =(PATIENT, CTRL) to get the instruction parammeter (TEST or PTEST)
        '''          TR 11/05/2012 - On the prepare Generation creating on the SampleClass validation removed the CTRL the 
        '''                          preparation generation is for patien only.
        '''          AG 13/11/2014 BA-2118 add new parameter pAnalyzerModel
        ''' </remarks>
        Public Function GeneratePreparation(ByVal pExecutionID As Integer, ByVal pAnalyzerModel As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myWSExecutionDS As New ExecutionsDS
                Dim myExecutionDelegate As New ExecutionsDelegate
                Dim myPreparationTestDataDS As New PreparationsTestDataDS
                Dim myPreparationPositionDS As New PreparationsPositionDataDS
                Dim myPreparationPositionDataDAO As New vwksPreparationsPositionDataDAO

                Dim myPreparationParameterList As New List(Of InstructionParameterTO)

                'get the execution data 
                myGlobalDataTO = myExecutionDelegate.GetExecution(Nothing, pExecutionID)
                If Not myGlobalDataTO.HasError Then
                    myWSExecutionDS = DirectCast(myGlobalDataTO.SetDatos, ExecutionsDS)
                    If myWSExecutionDS.twksWSExecutions.Rows.Count > 0 Then
                        'set the order test id to a local variable.
                        Dim myOrderTestID As Integer = myWSExecutionDS.twksWSExecutions(0).OrderTestID

                        'Get the preparation test data.
                        myGlobalDataTO = vwksPreparationsTestDataDAO.ReadByOrderTestID(Nothing, myOrderTestID)
                        If Not myGlobalDataTO.HasError Then
                            'TODO: currently this function only works for STANDARD Tests (data in the view is filtered
                            '      by TestType = 'STD')
                            myPreparationTestDataDS = DirectCast(myGlobalDataTO.SetDatos, PreparationsTestDataDS)
                        Else
                            Exit Try
                        End If

                        'Get the preparation positions Data
                        myGlobalDataTO = myPreparationPositionDataDAO.ReadByOrderTestID(Nothing, myOrderTestID)
                        If Not myGlobalDataTO.HasError Then
                            myPreparationPositionDS = DirectCast(myGlobalDataTO.SetDatos, PreparationsPositionDataDS)
                        Else
                            Exit Try
                        End If

                        'AG 13/11/2014 BA-2118 Get parameter SW_READINGSOFFSET
                        Dim mySwParamsDelg As New SwParametersDelegate
                        myGlobalDataTO = mySwParamsDelg.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SW_READINGSOFFSET.ToString, pAnalyzerModel)

                        'Cycles between sample dispensation and the 1st reading
                        Dim readingsOffsetParam As Integer = 0
                        If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing AndAlso DirectCast(myGlobalDataTO.SetDatos, ParametersDS).tfmwSwParameters.Rows.Count > 0 Then
                            readingsOffsetParam = CInt(DirectCast(myGlobalDataTO.SetDatos, ParametersDS).tfmwSwParameters(0).ValueNumeric)
                        End If
                        'AG 13/11/2014

                        'TR -19/01/2011 -Validate the predilution mode to Get the instruction parameter.
                        If myPreparationTestDataDS.vwksPreparationsTestData.Count > 0 Then
                            'Valiadte if it's an automatic predilution, Patient and Control.
                            If myPreparationTestDataDS.vwksPreparationsTestData(0).PredilutionMode = "INST" AndAlso _
                                                          myWSExecutionDS.twksWSExecutions(0).SampleClass = "PATIENT" Then
                                'Generate the parameterlist PTEST
                                myPreparationParameterList = GetInstructionParameter("PTEST")
                                myGlobalDataTO = FillPTestPreparation(myPreparationParameterList, myWSExecutionDS, _
                                                                         myPreparationTestDataDS, myPreparationPositionDS, readingsOffsetParam)

                            Else
                                myPreparationParameterList = GetInstructionParameter("TEST")
                                'Create the test preparation for test
                                myGlobalDataTO = FillTestPreparation(myPreparationParameterList, _
                                                 myWSExecutionDS, myPreparationTestDataDS, myPreparationPositionDS, readingsOffsetParam)
                            End If
                            'TR 21/05/2012 -In case of error the LOCK execution.
                            If myGlobalDataTO.HasError Then
                                myWSExecutionDS.twksWSExecutions(0).BeginEdit()
                                myWSExecutionDS.twksWSExecutions(0).ExecutionStatus = "LOCKED"
                                myWSExecutionDS.twksWSExecutions(0).EndEdit()
                                myGlobalDataTO = myExecutionDelegate.UpdateStatus(Nothing, myWSExecutionDS)
                                'make sure send error.
                                If Not myGlobalDataTO.HasError Then
                                    'Dim myLogAcciones As New ApplicationLogManager()
                                    GlobalBase.CreateLogActivity("Instruction with empty fields. Sw locks execution: " & pExecutionID.ToString, "Instructions.GeneratePreparation", EventLogEntryType.Information, False) 'AG 30/05/2012

                                    myGlobalDataTO.HasError = True
                                    myGlobalDataTO.ErrorCode = "EMPTY_FIELDS" 'AG 27/09/2012 - inform not system error, it is a protection
                                End If
                            End If
                            'TR 21/05/2012 -END.
                        End If

                    Else 'no execution found
                        myGlobalDataTO.HasError = True
                        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.NOT_CASE_FOUND.ToString
                    End If

                Else '(1)
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.WRONG_DATATYPE.ToString
                End If '(1)

                'End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GeneratePreparation", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO

        End Function

        ''' <summary>
        ''' Generate a Washing reRunning parameters with the corresponding values.
        ''' </summary>
        ''' <param name="pWashingDataDS">Washing Data</param>
        ''' <returns></returns>
        ''' <remarks>CREATE BY: TR 28/01/2011
        '''          Modify by: DL 26/06/2012 
        ''' </remarks>
        Public Function GenerateWRunInstruction(ByVal pWashingDataDS As AnalyzerManagerDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myPreparationParameterList As New List(Of InstructionParameterTO)
                Dim myRotorContentByPositionDelegate As New WSRotorContentByPositionDelegate
                Dim qRCPList As New List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)

                If pWashingDataDS.nextPreparation.Count > 0 Then

                    myPreparationParameterList = GetInstructionParameter("WRUN")
                    If myPreparationParameterList.Count > 0 Then
                        For Each myInstructionTO As InstructionParameterTO In myPreparationParameterList
                            Select Case myInstructionTO.ParameterIndex
                                Case 1  'Axxx -Analyzer Description
                                    'myInstructionTO.Parameter = "A400"
                                    myInstructionTO.ParameterValue = "A400" '1st & 2on characters: model, 3th & 4th characters: analyzer number
                                    Exit Select
                                Case 2 'WRUN -Instruction Code
                                    myInstructionTO.ParameterValue = myInstructionTO.InstructionType
                                    Exit Select

                                Case 3 'M -Washing Mode.
                                    If Not pWashingDataDS.nextPreparation(0).IsReagentContaminationFlagNull AndAlso pWashingDataDS.nextPreparation(0).ReagentContaminationFlag Then
                                        myInstructionTO.ParameterValue = "1" 'Running Mode (System Water R1,R2 - WS from a Bottle R1,R2).
                                    ElseIf Not pWashingDataDS.nextPreparation(0).IsCuvetteContaminationFlagNull AndAlso pWashingDataDS.nextPreparation(0).CuvetteContaminationFlag Then
                                        myInstructionTO.ParameterValue = "2" 'Running Mode - Rotor Well Wash.

                                    Else 'DL 26/06/2012
                                        myGlobalDataTO.HasError = True  'DL 26/06/2012
                                        Exit For   'DL 26/06/2012
                                    End If

                                    Exit Select

                                Case 4 ' S -Source.
                                    If pWashingDataDS.nextPreparation(0).IsWashSolution1Null And pWashingDataDS.nextPreparation(0).IsWashSolution2Null Then
                                        myInstructionTO.ParameterValue = "1" 'System Water.

                                    ElseIf Not pWashingDataDS.nextPreparation(0).IsReagentContaminationFlagNull AndAlso _
                                                     pWashingDataDS.nextPreparation(0).ReagentContaminationFlag AndAlso _
                                                                    Not pWashingDataDS.nextPreparation(0).IsWashSolution1Null Then
                                        myInstructionTO.ParameterValue = "2" 'External WS

                                    ElseIf Not pWashingDataDS.nextPreparation(0).IsCuvetteContaminationFlagNull AndAlso _
                                                      pWashingDataDS.nextPreparation(0).CuvetteContaminationFlag AndAlso _
                                                      Not pWashingDataDS.nextPreparation(0).IsWashSolution1Null AndAlso _
                                                      Not pWashingDataDS.nextPreparation(0).IsWashSolution2Null Then
                                        myInstructionTO.ParameterValue = "2" 'External WS

                                    ElseIf Not pWashingDataDS.nextPreparation(0).IsCuvetteContaminationFlagNull AndAlso _
                                                     pWashingDataDS.nextPreparation(0).CuvetteContaminationFlag AndAlso _
                                                     Not pWashingDataDS.nextPreparation(0).IsWashSolution1Null AndAlso _
                                                     pWashingDataDS.nextPreparation(0).IsWashSolution2Null Then
                                        myInstructionTO.ParameterValue = "3" 'R1 with External WS + R2 with Internal (Mode 2 Only)

                                    Else 'DL 26/06/2012
                                        myGlobalDataTO.HasError = True  'DL 26/06/2012
                                        Exit For   'DL 26/06/2012
                                    End If
                                    Exit Select

                                Case 5
                                    myInstructionTO.ParameterValue = GetValueExecutionID(pWashingDataDS)

                                Case 6 ' BP1 -Bottle Position R1.
                                    myInstructionTO.ParameterValue = "0" 'AG 09/02/2012
                                    If Not pWashingDataDS.nextPreparation(0).IsWashSolution1Null Then

                                        'Get the solution 1 information by solution code.
                                        myGlobalDataTO = myRotorContentByPositionDelegate.GetWashingSolutionPosInfoBySolutionCode(Nothing, _
                                                                                                                                  pWashingDataDS.nextPreparation(0).WashSolution1)
                                        If Not myGlobalDataTO.HasError Then
                                            Dim myWSRotorContentPosDS As New WSRotorContentByPositionDS
                                            myWSRotorContentPosDS = DirectCast(myGlobalDataTO.SetDatos, WSRotorContentByPositionDS)
                                            'Filter deplete position.
                                            'AG 03/08/2011 - Add condition Status <> LOCKED AND (BarcodeStatus IS NULL OR BarcodeStatus <> ‘ERROR’ )
                                            qRCPList = (From a In myWSRotorContentPosDS.twksWSRotorContentByPosition _
                                                        Where a.Status <> "DEPLETED" AndAlso a.Status <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                                        Select a).ToList()
                                            If qRCPList.Count > 0 Then
                                                myInstructionTO.ParameterValue = qRCPList.First().CellNumber.ToString()
                                            Else                                    'DL 26/06/2012
                                                myGlobalDataTO.HasError = True      'DL 26/06/2012
                                                Exit For                            'DL 26/06/2012
                                            End If
                                        Else
                                            Exit For
                                        End If
                                    End If
                                    Exit Select

                                Case 7 ' BT1 -Bottle Type.
                                    'With the previous values on the qRCPList get the BOTTLE Type.
                                    If qRCPList.Count > 0 Then
                                        myInstructionTO.ParameterValue = GetBottleCode(qRCPList.First().TubeType)
                                    Else
                                        myInstructionTO.ParameterValue = "0" 'AG 09/02/2012
                                    End If
                                    Exit Select

                                Case 8 'BRT1 -Bottle Rack Type.
                                    myInstructionTO.ParameterValue = "1" 'Type 1 (Original)
                                    Exit Select

                                Case 9 ' BP2 -Bottle Position R2.
                                    myInstructionTO.ParameterValue = "0" 'AG 09/02/2012
                                    If Not pWashingDataDS.nextPreparation(0).IsWashSolution2Null Then
                                        'Get the solution 1 information by solution code.
                                        myGlobalDataTO = myRotorContentByPositionDelegate.GetWashingSolutionPosInfoBySolutionCode(Nothing, _
                                                                                             pWashingDataDS.nextPreparation(0).WashSolution2)
                                        If Not myGlobalDataTO.HasError Then
                                            Dim myWSRotorContentPosDS As New WSRotorContentByPositionDS
                                            myWSRotorContentPosDS = DirectCast(myGlobalDataTO.SetDatos, WSRotorContentByPositionDS)
                                            'Filter deplete position
                                            'AG 03/08/2011 - Add condition Status <> LOCKED AND (BarcodeStatus IS NULL OR BarcodeStatus <> ‘ERROR’ )
                                            qRCPList = (From a In myWSRotorContentPosDS.twksWSRotorContentByPosition _
                                                        Where a.Status <> "DEPLETED" AndAlso a.Status <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                                        Select a).ToList()

                                            If qRCPList.Count > 0 Then
                                                myInstructionTO.ParameterValue = qRCPList.First().CellNumber.ToString()
                                            Else                                    'DL 26/06/2012
                                                myGlobalDataTO.HasError = True      'DL 26/06/2012
                                                Exit For                            'DL 26/06/2012
                                            End If
                                        Else
                                            Exit For
                                        End If
                                    End If
                                    Exit Select

                                Case 10 'BT2 -Bottle Type.
                                    'With the previous values on the qRCPList get the BOTTLE Type.
                                    If qRCPList.Count > 0 Then
                                        myInstructionTO.ParameterValue = GetBottleCode(qRCPList.First().TubeType)
                                    Else
                                        myInstructionTO.ParameterValue = "0" 'AG 09/02/2012
                                    End If
                                    Exit Select

                                Case 11 'BRT2 -Bottle Rack Type.
                                    myInstructionTO.ParameterValue = "1" 'Type 1 Original.
                                    Exit Select

                                Case Else 'exit select
                                    Exit Select
                            End Select
                        Next

                        If Not myGlobalDataTO.HasError Then 'DL 26/06/2012
                            myGlobalDataTO.SetDatos = myPreparationParameterList
                        Else
                            'DL 26/06/2012
                            'Dim myLogAcciones As New ApplicationLogManager()
                            GlobalBase.CreateLogActivity("Instruction with empty fields.", "Instructions.GenerateWSRunInstruction", EventLogEntryType.Information, False)
                            'DL 26/06/2012
                            myGlobalDataTO.ErrorCode = "EMPTY_FIELDS" 'AG 27/09/2012 - inform not system error, it is a protection
                        End If

                    End If
                End If
                qRCPList = Nothing 'AG 02/08/2012 - free memory

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateWRunInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate washing parameters with corresponding values 
        ''' ONLY IMPLEMENTED: case ARM 8. Need more information for other cases
        ''' PENDING.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 03/03/2011
        ''' </remarks>
        Public Function GenerateWASHInstruction() As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim mySWParametersDelegate As New SwParametersDelegate
                Dim mySWParametersDS As New ParametersDS
                Dim T1 As Integer = 0 'Air conditioning variable.
                Dim T2 As Integer = 0 'Washing Sol. conditioning.
                Dim T3 As Integer = 0 ' System water conditioning.

                'Get the Air conditioning Value no
                myGlobalDataTO = mySWParametersDelegate.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.WASH_CONDITIONING_T1.ToString, Nothing)
                If Not myGlobalDataTO.HasError Then
                    mySWParametersDS = DirectCast(myGlobalDataTO.SetDatos, ParametersDS)
                    If mySWParametersDS.tfmwSwParameters.Count > 0 Then
                        T1 = CInt(mySWParametersDS.tfmwSwParameters(0).ValueNumeric)
                    End If
                Else
                    Exit Try
                End If

                'Get the washing solution
                myGlobalDataTO = mySWParametersDelegate.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.WASH_CONDITIONING_T2.ToString, Nothing)
                If Not myGlobalDataTO.HasError Then
                    mySWParametersDS = DirectCast(myGlobalDataTO.SetDatos, ParametersDS)
                    If mySWParametersDS.tfmwSwParameters.Count > 0 Then
                        T2 = CInt(mySWParametersDS.tfmwSwParameters(0).ValueNumeric)
                    End If
                Else
                    Exit Try
                End If

                'Get the System water for conditioning.
                myGlobalDataTO = mySWParametersDelegate.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.WASH_CONDITIONING_T3.ToString, Nothing)
                If Not myGlobalDataTO.HasError Then
                    mySWParametersDS = DirectCast(myGlobalDataTO.SetDatos, ParametersDS)
                    If mySWParametersDS.tfmwSwParameters.Count > 0 Then
                        T3 = CInt(mySWParametersDS.tfmwSwParameters(0).ValueNumeric)
                    End If
                Else
                    Exit Try
                End If

                'Get the instruction on the Instruction list.
                Dim myWashInstruction As New List(Of InstructionParameterTO)
                myWashInstruction = GetInstructionParameter("WASH")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myWashInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code
                            myInstructionTO.ParameterValue = "WASH"
                            Exit Select
                        Case 3 'ARM by default 8 until define the oteher cases
                            myInstructionTO.ParameterValue = "8"
                            Exit Select
                        Case 4 'Source
                            myInstructionTO.ParameterValue = "1"
                            Exit Select
                        Case 5 'Time Air.
                            myInstructionTO.ParameterValue = T1.ToString()
                            Exit Select

                        Case 6  'Time Washing Solution.
                            myInstructionTO.ParameterValue = T2.ToString()
                            Exit Select

                        Case 7 ' Time Water.
                            myInstructionTO.ParameterValue = T3.ToString()
                            Exit Select

                        Case 8 'Bottle position.
                            myInstructionTO.ParameterValue = "0"
                            Exit Select

                        Case 9 'Bottle Type.
                            myInstructionTO.ParameterValue = "0"
                            Exit Select

                        Case 10 'Bottle Rack Type.
                            myInstructionTO.ParameterValue = "0"
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next

                myGlobalDataTO.SetDatos = myWashInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateWASHInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate a ALIGHT parameter with corresponding values.
        ''' </summary>
        ''' <param name="pWell">Well To perform the adjusment.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 03/03/2011
        ''' AG 27/05/2011 - add field AF (AutoFill)
        ''' </remarks>
        Public Function GenerateALIGHTInstruction(ByVal pWell As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim myALIGHTInstruction As New List(Of InstructionParameterTO)
                myALIGHTInstruction = GetInstructionParameter("ALIGHT")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myALIGHTInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "ALIGHT"
                            Exit Select
                        Case 3 'Well to perform the adjustment.
                            myInstructionTO.ParameterValue = pWell.ToString()
                            Exit Select
                        Case 4 'AutoFill
                            myInstructionTO.ParameterValue = CStr(GlobalEnumerates.Ax00BaseLineAutoFillInstructionModes.AUTO)  'alwasys Auto for user Sw
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = myALIGHTInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateALIGHTInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate a WSCTRL parameter with corresponding values.
        ''' </summary>
        ''' <param name="pWashStationMode">WashStation up or down</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: AG 31/05/2011
        ''' </remarks>
        Public Function GenerateWSCTRLInstruction(ByVal pWashStationMode As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim myWSCTRLInstruction As New List(Of InstructionParameterTO)
                myWSCTRLInstruction = GetInstructionParameter("WSCTRL")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myWSCTRLInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "WSCTRL"
                            Exit Select
                        Case 3 'WStation mode
                            myInstructionTO.ParameterValue = pWashStationMode.ToString()
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = myWSCTRLInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateWSCTRLInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Method that generate a Instruction depending on parameter value
        ''' </summary>
        ''' <param name="pParameterList"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  TR 21/05/2010
        ''' AG 29/10/2014 BA-2062 new instruction ANSFBLD
        ''' </remarks>
        Public Function GenerateReception(ByVal pParameterList As List(Of ParametersTO)) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myParameterValue As String = ""
                'Get the instruction type (Always the 2on parameter in list)                
                myParameterValue = pParameterList(1).ParameterID

                'Dim myID As String = ""
                'Dim iteration As Integer = 0 'AG 11/05/2010
                Dim myIndexedList As New List(Of InstructionParameterTO)
                'Get the instruction parameters to load values.
                myIndexedList = GetInstructionParameter(myParameterValue)

                If myIndexedList.Count > 0 Then '(1) AG 28/10/2010
                    Select Case myParameterValue
                        Case GlobalEnumerates.AppLayerInstrucionReception.STATUS.ToString, _
                                GlobalEnumerates.AppLayerInstrucionReception.ANSINF.ToString, _
                                GlobalEnumerates.AppLayerInstrucionReception.ANSPRD.ToString
                            GenerateFixedReception(pParameterList)
                            Exit Select

                        Case GlobalEnumerates.AppLayerInstrucionReception.ANSPHR.ToString
                            GenerateANSPHRInstruction(myParameterValue, pParameterList, myIndexedList)
                            Exit Select

                        Case GlobalEnumerates.AppLayerInstrucionReception.ANSBLD.ToString, _
                            GlobalEnumerates.AppLayerInstrucionReception.ANSISE.ToString, _
                            GlobalEnumerates.AppLayerInstrucionReception.ANSERR.ToString, _
                            GlobalEnumerates.AppLayerInstrucionReception.ANSFWU.ToString
                            'GenerateBaseLineTypeInstruction(myParameterValue, pParameterList, myIndexedList)
                            GenerateGenericalDinamicInstruction(myParameterValue, pParameterList, myIndexedList)
                            Exit Select

                            'AG 29/10/2014 BA-2062
                        Case GlobalEnumerates.AppLayerInstrucionReception.ANSFBLD.ToString
                            'GenerateBaseLineTypeInstruction(myParameterValue, pParameterList, myIndexedList)
                            GenerateANSFBLDInstructionReception(myParameterValue, pParameterList, myIndexedList)
                            Exit Select


                        Case GlobalEnumerates.AppLayerInstrucionReception.ANSBR1.ToString, _
                             GlobalEnumerates.AppLayerInstrucionReception.ANSBR2.ToString, _
                             GlobalEnumerates.AppLayerInstrucionReception.ANSBM1.ToString

                            GenerateANSARMsInstruction(myParameterValue, pParameterList, myIndexedList)
                            Exit Select

                        Case GlobalEnumerates.AppLayerInstrucionReception.ANSCBR.ToString
                            GenerateANSCBRInstruction(myParameterValue, pParameterList, myIndexedList)
                            Exit Select

                            ' XBC 03/05/2011
                        Case GlobalEnumerates.AppLayerInstrucionReception.ANSADJ.ToString

                            GenerateANSADJInstruction(pParameterList)
                            Exit Select

                            ' XBC 16/11/2010 - Pending to define PDT !!!
                        Case GlobalEnumerates.AppLayerInstrucionReception.ANSCMD.ToString, _
                             GlobalEnumerates.AppLayerInstrucionReception.ANSSDM.ToString, _
                             GlobalEnumerates.AppLayerInstrucionReception.TANKTESTEMPTYLC_OK.ToString, _
                             GlobalEnumerates.AppLayerInstrucionReception.TANKTESTFILLDW_OK.ToString, _
                             GlobalEnumerates.AppLayerInstrucionReception.TANKTESTTRANSFER_OK.ToString

                            ' XBC 17/05/2011 - delete redundant Events replaced using AnalyzerManager.ReceptionEvent
                            ' GlobalEnumerates.AppLayerInstrucionReception.SENSORS_RECEIVED.ToString, _
                            ' GlobalEnumerates.AppLayerInstrucionReception.ABSORBANCE_RECEIVED.ToString, _

                            GenerateFixedReception(pParameterList)
                            Exit Select
                            ' XBC 16/11/2010 - Pending to define PDT !!!

                            'SGM 24/05/2011
                        Case GlobalEnumerates.AppLayerInstrucionReception.ANSCPU.ToString, _
                             GlobalEnumerates.AppLayerInstrucionReception.ANSBXX.ToString, _
                             GlobalEnumerates.AppLayerInstrucionReception.ANSDXX.ToString, _
                             GlobalEnumerates.AppLayerInstrucionReception.ANSRXX.ToString, _
                             GlobalEnumerates.AppLayerInstrucionReception.ANSGLF.ToString, _
                             GlobalEnumerates.AppLayerInstrucionReception.ANSSFX.ToString, _
                             GlobalEnumerates.AppLayerInstrucionReception.ANSJEX.ToString, _
                             GlobalEnumerates.AppLayerInstrucionReception.ANSPSN.ToString
                            GenerateFixedReception(pParameterList)
                            Exit Select

                            'XBC 25/05/2011
                        Case GlobalEnumerates.AppLayerInstrucionReception.ANSFCP.ToString, _
                             GlobalEnumerates.AppLayerInstrucionReception.ANSFBX.ToString, _
                             GlobalEnumerates.AppLayerInstrucionReception.ANSFDX.ToString, _
                             GlobalEnumerates.AppLayerInstrucionReception.ANSFRX.ToString, _
                             GlobalEnumerates.AppLayerInstrucionReception.ANSFGL.ToString, _
                             GlobalEnumerates.AppLayerInstrucionReception.ANSFSF.ToString, _
                             GlobalEnumerates.AppLayerInstrucionReception.ANSFJE.ToString, _
                             GlobalEnumerates.AppLayerInstrucionReception.ANSUTIL.ToString
                            GenerateFixedReception(pParameterList)
                            Exit Select

                            '    'XBC 25/05/2011
                            'Case GlobalEnumerates.AppLayerInstrucionReception.ANSEVENTS.ToString
                            '    GenerateFixedReception(pParameterList)

                        Case GlobalEnumerates.AppLayerInstrucionReception.ANSTIN.ToString 'AG 30/07/2012
                            GenerateANSTINInstruction(myParameterValue, pParameterList, myIndexedList)

                            '    'SGM 11/10/2012
                            'Case GlobalEnumerates.AppLayerInstrucionReception.ANSUTIL.ToString
                            '    GenerateANSUTILInstruction(pParameterList)

                            Exit Select

                    End Select

                    myGlobalDataTO.SetDatos = myIndexedList

                Else '(1)
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.WRONG_DATATYPE.ToString
                End If '(1)

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateReception", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate a short instruction with the corresponding values.
        ''' </summary>
        ''' <param name="pInstructionID">Instrucion ID</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  AG 03/05/2010 (adapted from GeneratePreparation)
        ''' </remarks>
        Public Function GenerateShortInstruction(ByVal pInstructionID As GlobalEnumerates.AppLayerEventList) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try

                Dim myShortParameterList As New List(Of InstructionParameterTO)
                'Get the instruction parameter list corresponding to the preparations.
                myShortParameterList = GetInstructionParameter(pInstructionID.ToString)

                If myShortParameterList.Count > 0 Then '(1) AG 28/10/2010
                    For Each myInstructionTO As InstructionParameterTO In myShortParameterList
                        Select Case myInstructionTO.ParameterIndex
                            Case 1 'Axxx Analyzer Description
                                'myInstructionTO.Parameter = "A400"
                                myInstructionTO.ParameterValue = "A400" '1st & 2on characters: model, 3th & 4th characters: analyzer number
                                Exit Select
                            Case 2 'Instruction Type
                                myInstructionTO.ParameterValue = myInstructionTO.InstructionType
                                'AG 28/12/2010
                                If pInstructionID = GlobalEnumerates.AppLayerEventList.ENDRUN Then
                                    myInstructionTO.ParameterValue = "END" 'AG - Vstudio dont allow define enum as END, so we continue using ENDRUN
                                End If
                                'AG 28/12/2010

                                Exit Select

                            Case Else
                                Exit Select
                        End Select
                    Next
                    myGlobalDataTO.SetDatos = myShortParameterList

                Else '(1)
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.WRONG_DATATYPE.ToString
                End If '(1)


            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateShortInstruction", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Generate a INFO parameter with corresponding values.
        ''' </summary>
        ''' <param name="pQueryMode "></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' AG 07/04/2011
        ''' </remarks>
        Public Function GenerateINFOInstruction(ByVal pQueryMode As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim myINFOInstruction As New List(Of InstructionParameterTO)
                myINFOInstruction = GetInstructionParameter("INFO")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myINFOInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "INFO"
                            Exit Select
                        Case 3 'Well to perform the adjustment.
                            myInstructionTO.ParameterValue = pQueryMode
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = myINFOInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateINFOInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate a ALIGHT parameter with corresponding values.
        ''' </summary>
        ''' <param name="pWell">Well To perform the adjusment.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: XBC 20/02/2012 - overloaded to allow fillmode parameter
        ''' </remarks>
        Public Function GenerateALIGHTInstruction(ByVal pWell As Integer, ByVal pFillMode As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim myALIGHTInstruction As New List(Of InstructionParameterTO)
                myALIGHTInstruction = GetInstructionParameter("ALIGHT")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myALIGHTInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "ALIGHT"
                            Exit Select
                        Case 3 'Well to perform the adjustment.
                            myInstructionTO.ParameterValue = pWell.ToString()
                            Exit Select
                        Case 4 'Fill mode.
                            myInstructionTO.ParameterValue = pFillMode.ToString()
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = myALIGHTInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateALIGHTInstruction overload 2", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate a BLIGHT parameter with corresponding values.
        ''' </summary>
        ''' <param name="pWell">Well To perform the adjusment.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: XBC 20/04/2011
        ''' </remarks>
        Public Function GenerateBLIGHTInstruction(ByVal pWell As Integer, ByVal pFillMode As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim myBLIGHTInstruction As New List(Of InstructionParameterTO)
                myBLIGHTInstruction = GetInstructionParameter("BLIGHT")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myBLIGHTInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "BLIGHT"
                            Exit Select
                        Case 3 'Well to perform the adjustment.
                            myInstructionTO.ParameterValue = pWell.ToString()
                            Exit Select
                        Case 4 'Fill mode.
                            myInstructionTO.ParameterValue = pFillMode.ToString()
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = myBLIGHTInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateBLIGHTInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        '''  Generate a FLIGHT parameter with corresponding values.
        ''' </summary>
        ''' <param name="action"></param>
        ''' <param name="led"></param>
        ''' <returns></returns>
        ''' <remarks> Created by:  IT 29/10/2014: BA-2061</remarks>
        Public Function GenerateFLIGHTInstruction(ByVal action As Integer, ByVal led As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim myFLIGHTInstruction As New List(Of InstructionParameterTO)
                myFLIGHTInstruction = GetInstructionParameter("FLIGHT")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myFLIGHTInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "FLIGHT"
                            Exit Select
                        Case 3 'Action
                            myInstructionTO.ParameterValue = action.ToString()
                            Exit Select
                        Case 4 'Led number
                            myInstructionTO.ParameterValue = led.ToString()
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = myFLIGHTInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateFLIGHTInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate a READADJ parameter with corresponding values.
        ''' </summary>
        ''' <param name="pQueryMode "></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' XBC 03/05/2011
        ''' </remarks>
        Public Function GenerateREADADJInstruction(ByVal pQueryMode As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim myREADADJInstruction As New List(Of InstructionParameterTO)
                myREADADJInstruction = GetInstructionParameter("READADJ")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myREADADJInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "READADJ"
                            Exit Select
                        Case 3 'Well to perform the adjustment.
                            myInstructionTO.ParameterValue = pQueryMode
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = myREADADJInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateREADADJInstruction", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate a ISECMD parameter with corresponding values.
        ''' </summary>
        ''' <param name="pISECommand"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' SGM 12/12/2011
        ''' </remarks>
        Public Function GenerateISECMDInstruction(ByVal pISECommand As ISECommandTO) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim myISECMDInstruction As New List(Of InstructionParameterTO)
                myISECMDInstruction = GetInstructionParameter("ISECMD")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myISECMDInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "ISECMD"
                            Exit Select
                        Case 3 'ISE mode
                            myInstructionTO.ParameterValue = CInt(pISECommand.ISEMode).ToString
                            Exit Select
                        Case 4 'ISE command
                            myInstructionTO.ParameterValue = CInt(pISECommand.ISECommandID).ToString
                            Exit Select
                        Case 5 'P1
                            myInstructionTO.ParameterValue = pISECommand.P1.ToString
                            Exit Select
                        Case 6 'P1
                            myInstructionTO.ParameterValue = pISECommand.P2.ToString
                            Exit Select
                        Case 7 'P1
                            myInstructionTO.ParameterValue = pISECommand.P3.ToString
                            Exit Select
                        Case 8 'M1
                            myInstructionTO.ParameterValue = pISECommand.SampleTubePos.ToString
                            Exit Select
                        Case 9 'Sample tube type
                            myInstructionTO.ParameterValue = GetBottleCode(pISECommand.SampleTubeType) 'AG 11/01/2012 CInt(pISECommand.SampleTubeType).ToString
                            Exit Select
                        Case 10 'Sample Rotor Type
                            myInstructionTO.ParameterValue = pISECommand.SampleRotorType.ToString
                            Exit Select
                        Case 11 'Sample Volume
                            myInstructionTO.ParameterValue = pISECommand.SampleVolume.ToString
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = myISECMDInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateISECMDInstruction", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate a FWUTIL parameter with corresponding values.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' SGM 29/05/2012
        ''' </remarks>
        Public Function GenerateFWUTILInstruction(ByVal pFWAction As FWUpdateRequestTO) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim myFWUTILInstruction As New List(Of InstructionParameterTO)
                myFWUTILInstruction = GetInstructionParameter("FWUTIL")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myFWUTILInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "FWUTIL"
                            Exit Select
                        Case 3 'Action
                            myInstructionTO.ParameterValue = CInt(pFWAction.ActionType).ToString
                            Exit Select
                        Case 4 'Block Index
                            myInstructionTO.ParameterValue = pFWAction.DataBlockIndex.ToString
                            Exit Select
                        Case 5 'Block Size
                            myInstructionTO.ParameterValue = pFWAction.DataBlockSize.ToString
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = myFWUTILInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateFWUTILInstruction", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate the LOADADJ instruction
        ''' </summary>
        ''' <param name="pParamsToSave">Parameters List</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: XBC 06/05/2011
        ''' </remarks>
        Public Function GenerateLOADADJInstruction(ByVal pParamsToSave As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myParametersToSave As String
            Try
                'Get the instruction on the Instruction list.
                Dim myLOADADJInstruction As New List(Of InstructionParameterTO)
                myLOADADJInstruction = GetInstructionParameter("LOADADJ")

                ' Delete comments
                myGlobalDataTO = CleanLOADADJComments(pParamsToSave)
                If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then
                    myParametersToSave = CType(myGlobalDataTO.SetDatos, String)
                    myParametersToSave = myParametersToSave.Trim
                    ' Eliminate last character ";"
                    myParametersToSave = myParametersToSave.Substring(0, myParametersToSave.Length - 1)

                    'Fill all the instruccion.
                    For Each myInstructionTO As InstructionParameterTO In myLOADADJInstruction
                        Select Case myInstructionTO.ParameterIndex
                            Case 1 'Analyzer Model and Number.
                                myInstructionTO.ParameterValue = "A400"
                                Exit Select
                            Case 2 'Instruction Code.
                                myInstructionTO.ParameterValue = "LOADADJ"
                                Exit Select
                            Case 3 'Well to perform the adjustment.
                                myInstructionTO.ParameterValue = myParametersToSave
                                Exit Select

                            Case Else
                                Exit Select

                        End Select
                    Next
                    myGlobalDataTO.SetDatos = myLOADADJInstruction

                Else
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.WRONG_DATATYPE.ToString
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateLOADADJInstruction", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate a RESET ANALYZER Instruction
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' SGM 01/07/2011
        ''' </remarks>
        Public Function GenerateRESETInstruction() As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim myRESETInstruction As New List(Of InstructionParameterTO)
                myRESETInstruction = GetInstructionParameter("RESET")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myRESETInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "RESET"
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = myRESETInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateRESETInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate a LOAD FACTORY ADJUSTMENTS Instruction
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' SGM 01/07/2011
        ''' </remarks>
        Public Function GenerateLOADFACTORYADJInstruction() As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim myLoadFactoryInstruction As New List(Of InstructionParameterTO)
                myLoadFactoryInstruction = GetInstructionParameter("LOADFACTORYADJ")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myLoadFactoryInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "LOADFACTORYADJ"
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = myLoadFactoryInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateLOADFACTORYADJInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate a UPDATE FIRMWAREInstruction
        ''' </summary>
        ''' <param name="pFirmware  "></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' SGM 01/07/2011
        ''' </remarks>
        Public Function GenerateUPDATEFWInstruction(ByVal pFirmware() As Byte) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myParametersToSave As String = ""
            Try
                'Get the instruction on the Instruction list.
                Dim myUPDATEFWInstruction As New List(Of InstructionParameterTO)
                myUPDATEFWInstruction = GetInstructionParameter("UPDATEFW")

                'convert Byte to string PENDING!!!!!!!!!!!
                myParametersToSave &= Convert.ToBase64String(pFirmware)
                myParametersToSave = "FIRMWAREBYTESTOSEND"

                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myUPDATEFWInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "UPDATEFW"
                            Exit Select
                        Case 3 'Well to perform the adjustment.
                            myInstructionTO.ParameterValue = myParametersToSave
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = myUPDATEFWInstruction


            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateUPDATEFWInstruction", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate a TANKSTEST parameter with corresponding values.    PDT TO SPECIFIED !!!
        ''' </summary>
        ''' <param name="pQueryMode "></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' XBC 18/05/2011
        ''' </remarks>
        Public Function GenerateTANKSTESTInstruction(ByVal pQueryMode As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim myINFOInstruction As New List(Of InstructionParameterTO)
                myINFOInstruction = GetInstructionParameter("TANKSTEST")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myINFOInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "TANKSTEST"
                            Exit Select
                        Case 3 'Well to perform the adjustment.
                            myInstructionTO.ParameterValue = pQueryMode
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = myINFOInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateTANKSTESTInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate a STRESS TEST parameter with corresponding values.
        ''' </summary>
        ''' <param name="pQueryMode "></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' XBC 23/05/2011
        ''' </remarks>
        Public Function GenerateSDMODEInstruction(ByVal pQueryMode As String, _
                                                  ByVal pCycles As String, _
                                                  ByVal pHour As String, _
                                                  ByVal pMinute As String, _
                                                  ByVal pSecond As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim mySTRESSInstruction As New List(Of InstructionParameterTO)
                mySTRESSInstruction = GetInstructionParameter("SDMODE")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In mySTRESSInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "SDMODE"
                            Exit Select
                        Case 3 'Query.
                            myInstructionTO.ParameterValue = pQueryMode
                            Exit Select
                        Case 4 'Cycles.
                            myInstructionTO.ParameterValue = pCycles
                            Exit Select
                        Case 5 'Hour.
                            myInstructionTO.ParameterValue = pHour
                            Exit Select
                        Case 6 'Minute.
                            myInstructionTO.ParameterValue = pMinute
                            Exit Select
                        Case 7 'Second.
                            myInstructionTO.ParameterValue = pSecond
                            Exit Select


                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = mySTRESSInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateSDMODEInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate a STRESS REQUEST parameter with corresponding values.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' XBC 23/05/2011
        ''' </remarks>
        Public Function GenerateSDPOLLInstruction() As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim mySTRESSInstruction As New List(Of InstructionParameterTO)
                mySTRESSInstruction = GetInstructionParameter("SDPOLL")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In mySTRESSInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "SDPOLL"
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = mySTRESSInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateSDPOLLInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ' ''' <summary>
        ' ''' Generate a STRESS STOP parameter with corresponding values.    PDT TO SPECIFIED !!!
        ' ''' </summary>
        ' ''' <returns></returns>
        ' ''' <remarks>
        ' ''' XBC 23/05/2011
        ' ''' </remarks>
        'Public Function GenerateSTRESSSTOPInstruction() As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Try
        '        'Get the instruction on the Instruction list.
        '        Dim mySTRESSInstruction As New List(Of InstructionParameterTO)
        '        mySTRESSInstruction = GetInstructionParameter("STRESSSTOP")
        '        'Fill all the instruccion.
        '        For Each myInstructionTO As InstructionParameterTO In mySTRESSInstruction
        '            Select Case myInstructionTO.ParameterIndex
        '                Case 1 'Analyzer Model and Number.
        '                    myInstructionTO.ParameterValue = "A400"
        '                    Exit Select
        '                Case 2 'Instruction Code.
        '                    myInstructionTO.ParameterValue = "STRESSSTOP"
        '                    Exit Select

        '                Case Else
        '                    Exit Select

        '            End Select
        '        Next
        '        myGlobalDataTO.SetDatos = mySTRESSInstruction
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateSTRESSSTOPInstruction", EventLogEntryType.Error, False)
        '    End Try

        '    Return myGlobalDataTO
        'End Function

        ''' <summary>
        ''' Generate a POLLHW parameter with corresponding values.   
        ''' </summary>
        ''' <param name="pQueryMode "></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' XBC 31/05/2011
        ''' </remarks>
        Public Function GeneratePOLLHWInstruction(ByVal pQueryMode As GlobalEnumerates.POLL_IDs) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim myPOLLHWInstruction As New List(Of InstructionParameterTO)
                myPOLLHWInstruction = GetInstructionParameter("POLLHW")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myPOLLHWInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "POLLHW"
                            Exit Select
                        Case 3 'Identifier requested.
                            myInstructionTO.ParameterValue = CInt(pQueryMode).ToString
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = myPOLLHWInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GeneratePOLLHWInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate ENABLE EVENTS  
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' SGM 10/06/2011
        ''' </remarks>
        Public Function GenerateENABLEEVENTSInstruction() As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim myENABLEEVENTSInstruction As New List(Of InstructionParameterTO)
                myENABLEEVENTSInstruction = GetInstructionParameter("ENABLE_EVENTS")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myENABLEEVENTSInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "ENABLE_EVENTS"
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = myENABLEEVENTSInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateENABLEEVENTSInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate ENABLE EVENTS  
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' SGM 10/06/2011
        ''' </remarks>
        Public Function GenerateDISABLEEVENTSInstruction() As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim myDISABLEEVENTSInstruction As New List(Of InstructionParameterTO)
                myDISABLEEVENTSInstruction = GetInstructionParameter("DISABLE_EVENTS")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myDISABLEEVENTSInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "DISABLE_EVENTS"
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = myDISABLEEVENTSInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateDISABLEEVENTSInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate a POLLFW parameter with corresponding values.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created XBC 25/05/2011
        ''' </remarks>
        Public Function GeneratePOLLFWInstruction(ByVal pQueryMode As GlobalEnumerates.POLL_IDs) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim myPOLLFWInstruction As New List(Of InstructionParameterTO)
                myPOLLFWInstruction = GetInstructionParameter("POLLFW")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myPOLLFWInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2
                            myInstructionTO.ParameterValue = "POLLFW"
                            Exit Select
                        Case 3 'Identifier requested.
                            myInstructionTO.ParameterValue = CInt(pQueryMode).ToString
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = myPOLLFWInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GeneratePOLLFWInstruction", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate the CODEBR instruction
        ''' Use only the 1st record in DataSet
        ''' </summary>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pBarCodeDS" ></param>
        ''' <returns></returns>
        ''' <remarks>Modify by XBC 10/02/2012 - Add new parameters already specified
        '''                     AG 20/03/2012 - Different NumChars depending the rotor type + add pAnalyzerID parameter
        '''                     TR 12/04/2013 - Commented the area for barcoderequest for Samples Rotor, The NumCharsValue
        '''                                     is allways 0. this is to allow the flexible barcode size for samples.
        ''' </remarks>
        Public Function GenerateCODEBRInstruction(ByVal pAnalyzerID As String, ByVal pBarCodeDS As AnalyzerManagerDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If Not pBarCodeDS Is Nothing Then
                    If pBarCodeDS.barCodeRequests.Rows.Count > 0 Then

                        ' XBC 13/02/2012
                        Dim myBarCodesDelegate As New BarCodeConfigDelegate()
                        Dim myUserSettingsDelegate As New UserSettingsDelegate()
                        Dim SamplesBarCodesConfigurationDS As New BarCodesDS
                        Dim BarCodesUserSettingDS As New UserSettingDS
                        Dim myLinqRes As New List(Of BarCodesDS.vcfgSamplesBarCodesConfigurationRow)
                        Dim NumCharsValue As Integer = 0

                        If pBarCodeDS.barCodeRequests(0).Action = 1 Then

                            ' Get Barcode num chars value 
                            If pBarCodeDS.barCodeRequests(0).RotorType = "REAGENTS" Then
                                Dim mySwParametersDelegate As New SwParametersDelegate
                                myGlobalDataTO = mySwParametersDelegate.GetParameterByAnalyzer(Nothing, pAnalyzerID, _
                                                                            GlobalEnumerates.SwParameters.REAGENT_BARCODE_SIZE.ToString(), True)

                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    Dim mySwParameters As ParametersDS = DirectCast(myGlobalDataTO.SetDatos, ParametersDS)

                                    If (mySwParameters.tfmwSwParameters.Rows.Count > 0) Then
                                        NumCharsValue = CInt(mySwParameters.tfmwSwParameters(0).ValueNumeric)
                                    End If
                                End If

                            Else 'SAMPLES
                                NumCharsValue = 0 'Set value = to 0 for the Flexible barcode size.
                                myGlobalDataTO = myUserSettingsDelegate.ReadBarcodeSettings(Nothing)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    BarCodesUserSettingDS = DirectCast(myGlobalDataTO.SetDatos, UserSettingDS)

                                    Dim linqRes As New List(Of UserSettingDS.tcfgUserSettingsRow)

                                    linqRes = (From a As UserSettingDS.tcfgUserSettingsRow _
                                               In BarCodesUserSettingDS.tcfgUserSettings _
                                               Where a.SettingID = GlobalEnumerates.UserSettingsEnum.BARCODE_FULL_TOTAL.ToString() _
                                               Select a).ToList

                                    If linqRes.Count > 0 Then
                                        If IsNumeric(linqRes(0).CurrentValue) Then
                                            'Commented Return allways 0 for the flexible barcode size
                                            'NumCharsValue = CInt(linqRes(0).CurrentValue) 
                                        End If
                                    End If
                                    linqRes = Nothing 'AG 02/08/2012 - free memory
                                End If

                            End If

                            ' Get Barcode settings for Samples
                            If Not myGlobalDataTO.HasError Then

                                myGlobalDataTO = myBarCodesDelegate.GetSamplesBarCodesConfiguration(Nothing)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    SamplesBarCodesConfigurationDS = CType(myGlobalDataTO.SetDatos, BarCodesDS)
                                End If

                            End If

                        End If
                        ' XBC 13/02/2012 

                        'Get the instruction on the Instruction list.
                        Dim myCODEBRInstruction As New List(Of InstructionParameterTO)
                        myCODEBRInstruction = GetInstructionParameter("CODEBR")
                        'Fill all the instruccion.
                        For Each myInstructionTO As InstructionParameterTO In myCODEBRInstruction
                            Select Case myInstructionTO.ParameterIndex
                                Case 1 'Analyzer Model and Number.
                                    myInstructionTO.ParameterValue = "A400"
                                    Exit Select
                                Case 2 'Instruction Code.
                                    myInstructionTO.ParameterValue = "CODEBR"
                                    Exit Select
                                Case 3 'Code bar reader selector
                                    If pBarCodeDS.barCodeRequests(0).RotorType = "REAGENTS" Then
                                        myInstructionTO.ParameterValue = CStr(GlobalEnumerates.Ax00CodeBarReader.REAGENTS) 'Get the value, not the name
                                    Else
                                        myInstructionTO.ParameterValue = CStr(GlobalEnumerates.Ax00CodeBarReader.SAMPLES) 'Get the value, not the name
                                    End If
                                    Exit Select
                                Case 4 'Action
                                    myInstructionTO.ParameterValue = pBarCodeDS.barCodeRequests(0).Action.ToString
                                    Exit Select
                                Case 5 'Position
                                    If pBarCodeDS.barCodeRequests(0).Action = 3 Then
                                        myInstructionTO.ParameterValue = pBarCodeDS.barCodeRequests(0).Position.ToString
                                    Else
                                        myInstructionTO.ParameterValue = "0"
                                    End If

                                    Exit Select

                                    ' XBC 10/02/2012
                                    'Case 6 'Configuration command
                                    '    If pBarCodeDS.barCodeRequests(0).Action = 1 Then
                                    '        myInstructionTO.ParameterValue = pBarCodeDS.barCodeRequests(0).CommangConfig
                                    '    Else
                                    '        myInstructionTO.ParameterValue = "0"
                                    '    End If
                                    '    Exit Select

                                Case 6 'Configuration command C128
                                    'BarCode Enabled (Fw requirements: 1 On, 2 Off)
                                    If pBarCodeDS.barCodeRequests(0).RotorType = "REAGENTS" Then
                                        ' Espec : Reagents CodeBar Code is fixed to Code128
                                        pBarCodeDS.barCodeRequests(0).Code128 = 1
                                    Else
                                        Dim SampleBarCode As Integer = 2
                                        If pBarCodeDS.barCodeRequests(0).Action = 1 Then
                                            myLinqRes = (From a As BarCodesDS.vcfgSamplesBarCodesConfigurationRow In SamplesBarCodesConfigurationDS.vcfgSamplesBarCodesConfiguration _
                                                          Where a.CodeID = GlobalEnumerates.Ax00CodificationsCodeBar.Code128 Select a).ToList
                                            If myLinqRes.Count > 0 AndAlso myLinqRes(0).Status Then
                                                SampleBarCode = 1
                                            End If
                                        End If
                                        pBarCodeDS.barCodeRequests(0).Code128 = SampleBarCode
                                    End If
                                    myInstructionTO.ParameterValue = pBarCodeDS.barCodeRequests(0).Code128.ToString
                                    Exit Select
                                Case 7 'Number of Chars expected for the Reader C128
                                    If pBarCodeDS.barCodeRequests(0).Action = 1 Then
                                        Dim linqRes As New List(Of UserSettingDS.tcfgUserSettingsRow)
                                        'TR 08/03/2013 -Get the Barcode Sample ID configuration value
                                        linqRes = (From a As UserSettingDS.tcfgUserSettingsRow _
                                               In BarCodesUserSettingDS.tcfgUserSettings _
                                               Where a.SettingID = GlobalEnumerates.UserSettingsEnum.BARCODE_SAMPLEID_FLAG.ToString()
                                               Select a).ToList
                                        If linqRes.Count > 0 Then
                                            'if current value = 1 then set the NumCharsValues
                                            If linqRes.First().CurrentValue = "1" Then
                                                pBarCodeDS.barCodeRequests(0).NCode128 = NumCharsValue
                                            Else
                                                'If not configure the Sample ID Flag then set value = 0.
                                                pBarCodeDS.barCodeRequests(0).NCode128 = 0

                                            End If
                                        Else
                                            pBarCodeDS.barCodeRequests(0).NCode128 = NumCharsValue
                                        End If
                                        linqRes = Nothing
                                    Else
                                        pBarCodeDS.barCodeRequests(0).NCode128 = 0
                                    End If

                                    myInstructionTO.ParameterValue = pBarCodeDS.barCodeRequests(0).NCode128.ToString

                                    Exit Select
                                Case 8 'Configuration command C39
                                    If pBarCodeDS.barCodeRequests(0).RotorType = "REAGENTS" Then
                                        ' Espec : Reagents CodeBar Code is fixed to Code128
                                        pBarCodeDS.barCodeRequests(0).Code39 = 2
                                    Else
                                        Dim SampleBarCode As Integer = 2
                                        If pBarCodeDS.barCodeRequests(0).Action = 1 Then
                                            myLinqRes = (From a As BarCodesDS.vcfgSamplesBarCodesConfigurationRow In SamplesBarCodesConfigurationDS.vcfgSamplesBarCodesConfiguration _
                                                          Where a.CodeID = GlobalEnumerates.Ax00CodificationsCodeBar.Code39 Select a).ToList
                                            If myLinqRes.Count > 0 AndAlso myLinqRes(0).Status Then
                                                SampleBarCode = 1
                                            End If
                                        End If
                                        pBarCodeDS.barCodeRequests(0).Code39 = SampleBarCode
                                    End If
                                    myInstructionTO.ParameterValue = pBarCodeDS.barCodeRequests(0).Code39.ToString
                                    Exit Select
                                Case 9 'Number of Chars expected for the Reader C39
                                    If pBarCodeDS.barCodeRequests(0).Action = 1 Then
                                        pBarCodeDS.barCodeRequests(0).NCode39 = NumCharsValue
                                    Else
                                        pBarCodeDS.barCodeRequests(0).NCode39 = 0
                                    End If
                                    myInstructionTO.ParameterValue = pBarCodeDS.barCodeRequests(0).NCode39.ToString
                                    Exit Select
                                Case 10 'Configuration command CODEBAR
                                    If pBarCodeDS.barCodeRequests(0).RotorType = "REAGENTS" Then
                                        ' Espec : Reagents CodeBar Code is fixed to Code128
                                        pBarCodeDS.barCodeRequests(0).CodeBar = 2
                                    Else
                                        Dim SampleBarCode As Integer = 2
                                        If pBarCodeDS.barCodeRequests(0).Action = 1 Then
                                            myLinqRes = (From a As BarCodesDS.vcfgSamplesBarCodesConfigurationRow In SamplesBarCodesConfigurationDS.vcfgSamplesBarCodesConfiguration _
                                                          Where a.CodeID = GlobalEnumerates.Ax00CodificationsCodeBar.CodeBar Select a).ToList
                                            If myLinqRes.Count > 0 AndAlso myLinqRes(0).Status Then
                                                SampleBarCode = 1
                                            End If
                                        End If
                                        pBarCodeDS.barCodeRequests(0).CodeBar = SampleBarCode
                                    End If
                                    myInstructionTO.ParameterValue = pBarCodeDS.barCodeRequests(0).CodeBar.ToString
                                    Exit Select
                                Case 11 'Number of Chars expected for the Reader CODEBAR
                                    If pBarCodeDS.barCodeRequests(0).Action = 1 Then
                                        pBarCodeDS.barCodeRequests(0).NCodeBar = NumCharsValue
                                    Else
                                        pBarCodeDS.barCodeRequests(0).NCodeBar = 0
                                    End If
                                    myInstructionTO.ParameterValue = pBarCodeDS.barCodeRequests(0).NCodeBar.ToString
                                    Exit Select
                                Case 12 'Configuration command PHCD
                                    If pBarCodeDS.barCodeRequests(0).RotorType = "REAGENTS" Then
                                        ' Espec : Reagents CodeBar Code is fixed to Code128
                                        pBarCodeDS.barCodeRequests(0).PharmaCode = 2
                                    Else
                                        Dim SampleBarCode As Integer = 2
                                        If pBarCodeDS.barCodeRequests(0).Action = 1 Then
                                            myLinqRes = (From a As BarCodesDS.vcfgSamplesBarCodesConfigurationRow In SamplesBarCodesConfigurationDS.vcfgSamplesBarCodesConfiguration _
                                                          Where a.CodeID = GlobalEnumerates.Ax00CodificationsCodeBar.PharmaCode Select a).ToList
                                            If myLinqRes.Count > 0 AndAlso myLinqRes(0).Status Then
                                                SampleBarCode = 1
                                            End If
                                        End If
                                        pBarCodeDS.barCodeRequests(0).PharmaCode = SampleBarCode
                                    End If
                                    myInstructionTO.ParameterValue = pBarCodeDS.barCodeRequests(0).PharmaCode.ToString
                                    Exit Select
                                Case 13 'Number of Chars expected for the Reader PHCD
                                    If pBarCodeDS.barCodeRequests(0).Action = 1 Then
                                        pBarCodeDS.barCodeRequests(0).NPharmaCode = NumCharsValue
                                    Else
                                        pBarCodeDS.barCodeRequests(0).NPharmaCode = 0
                                    End If
                                    myInstructionTO.ParameterValue = pBarCodeDS.barCodeRequests(0).NPharmaCode.ToString
                                    Exit Select
                                Case 14 'Configuration command UPCE
                                    If pBarCodeDS.barCodeRequests(0).RotorType = "REAGENTS" Then
                                        ' Espec : Reagents CodeBar Code is fixed to Code128
                                        pBarCodeDS.barCodeRequests(0).UpcEan = 2
                                    Else
                                        Dim SampleBarCode As Integer = 2
                                        If pBarCodeDS.barCodeRequests(0).Action = 1 Then
                                            myLinqRes = (From a As BarCodesDS.vcfgSamplesBarCodesConfigurationRow In SamplesBarCodesConfigurationDS.vcfgSamplesBarCodesConfiguration _
                                                          Where a.CodeID = GlobalEnumerates.Ax00CodificationsCodeBar.UpcEan Select a).ToList
                                            If myLinqRes.Count > 0 AndAlso myLinqRes(0).Status Then
                                                SampleBarCode = 1
                                            End If
                                        End If
                                        pBarCodeDS.barCodeRequests(0).UpcEan = SampleBarCode
                                    End If
                                    myInstructionTO.ParameterValue = pBarCodeDS.barCodeRequests(0).UpcEan.ToString
                                    Exit Select
                                Case 15 'Number of Chars expected for the Reader UPCE
                                    If pBarCodeDS.barCodeRequests(0).Action = 1 Then
                                        pBarCodeDS.barCodeRequests(0).NUpcEan = NumCharsValue
                                    Else
                                        pBarCodeDS.barCodeRequests(0).NUpcEan = 0
                                    End If
                                    myInstructionTO.ParameterValue = pBarCodeDS.barCodeRequests(0).NUpcEan.ToString
                                    Exit Select
                                Case 16 'Configuration command C93
                                    If pBarCodeDS.barCodeRequests(0).RotorType = "REAGENTS" Then
                                        ' Espec : Reagents CodeBar Code is fixed to Code128
                                        pBarCodeDS.barCodeRequests(0).Code93 = 2
                                    Else
                                        Dim SampleBarCode As Integer = 2
                                        If pBarCodeDS.barCodeRequests(0).Action = 1 Then
                                            myLinqRes = (From a As BarCodesDS.vcfgSamplesBarCodesConfigurationRow In SamplesBarCodesConfigurationDS.vcfgSamplesBarCodesConfiguration _
                                                          Where a.CodeID = GlobalEnumerates.Ax00CodificationsCodeBar.Code93 Select a).ToList
                                            If myLinqRes.Count > 0 AndAlso myLinqRes(0).Status Then
                                                SampleBarCode = 1
                                            End If
                                        End If
                                        pBarCodeDS.barCodeRequests(0).Code93 = SampleBarCode
                                    End If
                                    myInstructionTO.ParameterValue = pBarCodeDS.barCodeRequests(0).Code93.ToString
                                    Exit Select
                                Case 17 'Number of Chars expected for the Reader C93
                                    If pBarCodeDS.barCodeRequests(0).Action = 1 Then
                                        pBarCodeDS.barCodeRequests(0).NCode93 = NumCharsValue
                                    Else
                                        pBarCodeDS.barCodeRequests(0).NCode93 = 0
                                    End If
                                    myInstructionTO.ParameterValue = pBarCodeDS.barCodeRequests(0).NCode93.ToString
                                    Exit Select
                                    ' XBC 10/02/2012

                                    ' XBC + AG 20/03/2012
                                Case 18 'Configuration command I25
                                    If pBarCodeDS.barCodeRequests(0).RotorType = "REAGENTS" Then
                                        ' Espec : Reagents CodeBar Code is fixed to Code128
                                        pBarCodeDS.barCodeRequests(0).Interleaved25 = 2
                                    Else
                                        Dim SampleBarCode As Integer = 2
                                        If pBarCodeDS.barCodeRequests(0).Action = 1 Then
                                            myLinqRes = (From a As BarCodesDS.vcfgSamplesBarCodesConfigurationRow In SamplesBarCodesConfigurationDS.vcfgSamplesBarCodesConfiguration _
                                                          Where a.CodeID = GlobalEnumerates.Ax00CodificationsCodeBar.Interleaved25 Select a).ToList
                                            If myLinqRes.Count > 0 AndAlso myLinqRes(0).Status Then
                                                SampleBarCode = 1
                                            End If
                                        End If
                                        pBarCodeDS.barCodeRequests(0).Interleaved25 = SampleBarCode
                                    End If
                                    myInstructionTO.ParameterValue = pBarCodeDS.barCodeRequests(0).Interleaved25.ToString
                                    Exit Select
                                Case 19 'Number of Chars expected for the Reader I25
                                    If pBarCodeDS.barCodeRequests(0).Action = 1 Then
                                        pBarCodeDS.barCodeRequests(0).NInterleaved25 = NumCharsValue
                                    Else
                                        pBarCodeDS.barCodeRequests(0).NInterleaved25 = 0
                                    End If
                                    myInstructionTO.ParameterValue = pBarCodeDS.barCodeRequests(0).NInterleaved25.ToString
                                    Exit Select
                                    ' XBC + AG 20/03/2012

                                Case Else
                                    Exit Select

                            End Select
                        Next
                        myLinqRes = Nothing 'AG 02/08/2012 - free memory
                        myGlobalDataTO.SetDatos = myCODEBRInstruction

                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateALIGHTInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate a SOUND Instruction
        ''' </summary>
        ''' <param name="pStatus "></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' XBC 18/10/2011
        ''' </remarks>
        Public Function GenerateSOUNDInstruction(ByVal pStatus As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim mySOUNDInstruction As New List(Of InstructionParameterTO)
                mySOUNDInstruction = GetInstructionParameter("SOUND")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In mySOUNDInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "SOUND"
                            Exit Select
                        Case 3 'Status.
                            myInstructionTO.ParameterValue = pStatus
                            Exit Select
                        Case 4 ' Mode
                            myInstructionTO.ParameterValue = "1" ' by now
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = mySOUNDInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateSOUNDInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate a INFO parameter with corresponding values.
        ''' </summary>
        ''' <param name="pConfigSettingsDS "></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' AG 23/11/2011
        ''' </remarks>
        Public Function GenerateCONFIGInstruction(ByVal pConfigSettingsDS As AnalyzerSettingsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Using Linq SEARCH  the proper analyzer settings and translate into Fw value required
                Dim myLinqRes As List(Of AnalyzerSettingsDS.tcfgAnalyzerSettingsRow)

                'Get the instruction on the Instruction list.
                Dim myCONFIGInstruction As New List(Of InstructionParameterTO)
                myCONFIGInstruction = GetInstructionParameter("CONFIG")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myCONFIGInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "CONFIG"
                            Exit Select
                        Case 3 'Reagents barcode activation/deactivation value
                            'Reagents BarCode Enabled (Fw requirements: 1 On, 2 Off)
                            Dim reagentsBarCode As String = "1"
                            myLinqRes = (From a As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow In pConfigSettingsDS.tcfgAnalyzerSettings _
                                         Where a.SettingID = GlobalEnumerates.AnalyzerSettingsEnum.REAGENT_BARCODE_DISABLED.ToString Select a).ToList
                            If myLinqRes.Count > 0 Then
                                If CType(myLinqRes(0).CurrentValue, Boolean) Then reagentsBarCode = "2"
                            End If

                            myInstructionTO.ParameterValue = reagentsBarCode
                            Exit Select
                        Case 4 'Samples barcode activation/deactivation value
                            'Samples BarCode Enabled (Fw requirements: 1 On, 2 Off)
                            Dim samplesBarCode As String = "1"
                            myLinqRes = (From a As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow In pConfigSettingsDS.tcfgAnalyzerSettings _
                                         Where a.SettingID = GlobalEnumerates.AnalyzerSettingsEnum.SAMPLE_BARCODE_DISABLED.ToString Select a).ToList
                            If myLinqRes.Count > 0 Then
                                If CType(myLinqRes(0).CurrentValue, Boolean) Then samplesBarCode = "2"
                            End If

                            myInstructionTO.ParameterValue = samplesBarCode
                            Exit Select
                        Case 5 'Water supple
                            'Water Suppy (Fw requirements: 1 External Tank, 2 Installation water)
                            Dim waterSupply As String = "1"
                            myLinqRes = (From a As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow In pConfigSettingsDS.tcfgAnalyzerSettings _
                                         Where a.SettingID = GlobalEnumerates.AnalyzerSettingsEnum.WATER_TANK.ToString Select a).ToList
                            If myLinqRes.Count > 0 Then
                                If Not CType(myLinqRes(0).CurrentValue, Boolean) Then waterSupply = "2"
                            End If

                            myInstructionTO.ParameterValue = waterSupply
                            Exit Select
                        Case Else
                            Exit Select

                    End Select
                Next
                myLinqRes = Nothing 'AG 02/08/2012 - free memory

                myGlobalDataTO.SetDatos = myCONFIGInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateINFOInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate a UTIL parameter with corresponding values.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' XBC 04/06/2012
        ''' </remarks>
        Public Function GenerateUTILInstruction(ByVal pTankTest As String, _
                                                ByVal pCollisionTest As String, _
                                                ByVal pAction As String, _
                                                ByVal pSerialNumber As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim myUTILInstruction As New List(Of InstructionParameterTO)
                myUTILInstruction = GetInstructionParameter("UTIL")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myUTILInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "UTIL"
                            Exit Select
                        Case 3 'Tank Test type.
                            myInstructionTO.ParameterValue = pTankTest
                            Exit Select
                        Case 4 'Collision Test type.
                            myInstructionTO.ParameterValue = pCollisionTest
                            Exit Select
                        Case 5 'Action type.
                            myInstructionTO.ParameterValue = pAction
                            Exit Select
                        Case 6 'Serial Number.
                            myInstructionTO.ParameterValue = pSerialNumber
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = myUTILInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateUTILInstruction", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate a POLLSN REQUEST parameter with corresponding values.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' XBC 30/07/2012
        ''' </remarks>
        Public Function GeneratePOLLSNInstruction() As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim myPOLLSNInstruction As New List(Of InstructionParameterTO)
                myPOLLSNInstruction = GetInstructionParameter("POLLSN")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myPOLLSNInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "POLLSN"
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = myPOLLSNInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GeneratePOLLSNInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function


#End Region

#Region "Public Methods for decode"

        Public Function DecodeANSFBLDReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                Dim myInstParamTO As New InstructionParameterTO
                Dim myResults As New DynamicBaseLineTO

                'Get LED field (parameter index 3)
                Dim myInst As String = String.Empty
                Dim index As Integer = 3

                resultData = Utilities.GetItemByParameterIndex(pInstructionReceived, index)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    myInstParamTO = DirectCast(resultData.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If
                myResults.Wavelength = CInt(myInstParamTO.ParameterValue)

                'Loop: Get wellused, mainlight, reflight
                Do Until index >= pInstructionReceived.Count - 6 'do not take into account the last 6 items (maindark, refdark, IT, DAC)
                    index += 1
                    resultData = Utilities.GetItemByParameterIndex(pInstructionReceived, index)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        myInstParamTO = DirectCast(resultData.SetDatos, InstructionParameterTO)
                    Else
                        Exit Do
                    End If
                    myResults.WellUsed.Add(CInt(myInstParamTO.ParameterValue))

                    index += 1
                    resultData = Utilities.GetItemByParameterIndex(pInstructionReceived, index)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        myInstParamTO = DirectCast(resultData.SetDatos, InstructionParameterTO)
                    Else
                        Exit Do
                    End If
                    myResults.MainLight.Add(CInt(myInstParamTO.ParameterValue))

                    index += 1
                    resultData = Utilities.GetItemByParameterIndex(pInstructionReceived, index)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        myInstParamTO = DirectCast(resultData.SetDatos, InstructionParameterTO)
                    Else
                        Exit Do
                    End If
                    myResults.RefLight.Add(CInt(myInstParamTO.ParameterValue))
                Loop

                If Not resultData.HasError Then
                    'Get maindark
                    index += 1
                    resultData = Utilities.GetItemByParameterIndex(pInstructionReceived, index)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        myInstParamTO = DirectCast(resultData.SetDatos, InstructionParameterTO)
                    Else
                        Exit Try
                    End If
                    myResults.MainDark = CInt(myInstParamTO.ParameterValue)

                    'Get refdark
                    index += 1
                    resultData = Utilities.GetItemByParameterIndex(pInstructionReceived, index)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        myInstParamTO = DirectCast(resultData.SetDatos, InstructionParameterTO)
                    Else
                        Exit Try
                    End If
                    myResults.RefDark = CInt(myInstParamTO.ParameterValue)

                    'Get mainbaseline
                    index += 1
                    resultData = Utilities.GetItemByParameterIndex(pInstructionReceived, index)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        myInstParamTO = DirectCast(resultData.SetDatos, InstructionParameterTO)
                    Else
                        Exit Try
                    End If
                    myResults.MainBaseLine = CInt(myInstParamTO.ParameterValue)

                    'Get refbaseline
                    index += 1
                    resultData = Utilities.GetItemByParameterIndex(pInstructionReceived, index)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        myInstParamTO = DirectCast(resultData.SetDatos, InstructionParameterTO)
                    Else
                        Exit Try
                    End If
                    myResults.RefBaseLine = CInt(myInstParamTO.ParameterValue)

                    'Get IT
                    index += 1
                    resultData = Utilities.GetItemByParameterIndex(pInstructionReceived, index)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        myInstParamTO = DirectCast(resultData.SetDatos, InstructionParameterTO)
                    Else
                        Exit Try
                    End If
                    myResults.IntegrationTime = CSng(myInstParamTO.ParameterValue)

                    'Get DAC
                    index += 1
                    resultData = Utilities.GetItemByParameterIndex(pInstructionReceived, index)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        myInstParamTO = DirectCast(resultData.SetDatos, InstructionParameterTO)
                    Else
                        Exit Try
                    End If
                    myResults.DAC = CSng(myInstParamTO.ParameterValue)

                    resultData.SetDatos = myResults
                End If


            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.DecodeANSFBLDReceived", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "Private Methods"

        ''' <summary>
        ''' Conver the ISE Test Volume to Steps
        ''' </summary>
        ''' <param name="pVolume"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATE BY: TR 24/01/2011
        ''' </remarks>
        Private Function CalculateISeVolumeSteps(ByVal pVolume As Single) As Single
            Dim myResult As Single = 0
            Try
                Dim SwParametersDS As New ParametersDS
                Dim myGlobalDataTO As New GlobalDataTO()
                Dim myAnalyzerModel As String = ""
                Dim myAnalyzerData As New AnalyzersDS
                Dim myAnalyzerDelegate As New AnalyzersDelegate
                Dim mySwParametersDelegate As New SwParametersDelegate()

                'Get the Analyzer model
                myGlobalDataTO = myAnalyzerDelegate.GetAnalyzer(Nothing)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myAnalyzerData = DirectCast(myGlobalDataTO.SetDatos, AnalyzersDS)
                    If (myAnalyzerData.tcfgAnalyzers.Rows.Count > 0) Then
                        'Inform properties AnalyzerID.
                        'AJG 24/02/2015 Changed AnalyzerID by AnalyzerModel
                        myAnalyzerModel = myAnalyzerData.tcfgAnalyzers(0).AnalyzerModel
                    End If
                End If
                ' Get the Analyzer parameters to get the Steps values.
                myGlobalDataTO = mySwParametersDelegate.ReadByAnalyzerModel(Nothing, myAnalyzerModel)
                If Not myGlobalDataTO.HasError Then
                    SwParametersDS = CType(myGlobalDataTO.SetDatos, ParametersDS)
                End If

                'Get the machine Cicle
                Dim qswParameter As New List(Of ParametersDS.tfmwSwParametersRow)
                'AG 19/04/2011 - Use the SAMPLE_STEPS_UL instead of STEPS_UL
                'AG 10/11/2014 BA-2082 filter also by analyzer model
                qswParameter = (From a In SwParametersDS.tfmwSwParameters _
                               Where a.ParameterName = GlobalEnumerates.SwParameters.SAMPLE_STEPS_UL.ToString AndAlso a.AnalyzerModel = myAnalyzerModel _
                               Select a).ToList()

                If qswParameter.Count > 0 Then
                    myResult = pVolume * qswParameter.First().ValueNumeric
                End If
                qswParameter = Nothing 'AG 02/08/2012 - free memory

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.CalculateISeVolumeSteps", EventLogEntryType.Error, False)
            End Try

            Return myResult
        End Function

        ''' <summary>
        ''' Create the test preparation for instruction type TEST.
        ''' </summary>
        ''' <param name="pPreparationParameterList"></param>
        ''' <param name="pWSExecutionDS"></param>
        ''' <param name="pPreparationTestDataDS"></param>
        ''' <param name="pPreparationPositionDS"></param>
        ''' <param name="pSwReadingOffset">Cycles between sample dispensation and the 1st reading</param>
        ''' <returns></returns>
        ''' <remarks>CREATE BY: TR 19/01/2010
        ''' TR 21/05/2012 - Return Error when instruction incomplete
        ''' AG 25/05/2012 - complete cases for return error when instruction incomplete: tm1, r2, vm1, vr1, vr2,  mw, rw, rn
        ''' AG 13/11/2014 BA-2118 add parameter pSwReadingOffset</remarks>
        Private Function FillTestPreparation(ByVal pPreparationParameterList As List(Of InstructionParameterTO), ByVal pWSExecutionDS As ExecutionsDS, _
                                                                                ByVal pPreparationTestDataDS As PreparationsTestDataDS, _
                                                                                ByVal pPreparationPositionDS As PreparationsPositionDataDS, _
                                                                                ByVal pSwReadingOffset As Integer) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim mytwksWSPreparationsDAO As New twksWSPreparationsDAO
                Dim myAnalyzerLedPositionsDelegate As New AnalyzerLedPositionsDelegate
                Dim qTestData As New List(Of PreparationsTestDataDS.vwksPreparationsTestDataRow)
                Dim qPositionData As New List(Of PreparationsPositionDataDS.vwksPreparationsPositionDataRow)
                Dim myOrderTestID As Integer = 0

                If pWSExecutionDS.twksWSExecutions.Count > 0 Then
                    myOrderTestID = pWSExecutionDS.twksWSExecutions(0).OrderTestID
                End If

                'AG 18/04/2012 - check if is a special test (calib HbTotal)
                Dim myMultiItemNumber As Integer = pWSExecutionDS.twksWSExecutions(0).MultiItemNumber
                If pWSExecutionDS.twksWSExecutions(0).SampleClass.Trim = "CALIB" Then
                    Dim mySettingsDelegate As New SpecialTestsSettingsDelegate
                    myGlobalDataTO = mySettingsDelegate.Read(Nothing, pWSExecutionDS.twksWSExecutions(0).TestID, pWSExecutionDS.twksWSExecutions(0).SampleType, GlobalEnumerates.SpecialTestsSettings.CAL_POINT_USED.ToString)

                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        Dim mySettingsDS As New SpecialTestsSettingsDS
                        mySettingsDS = DirectCast(myGlobalDataTO.SetDatos, SpecialTestsSettingsDS)
                        If mySettingsDS.tfmwSpecialTestsSettings.Rows.Count > 0 Then
                            myMultiItemNumber = CInt(mySettingsDS.tfmwSpecialTestsSettings(0).SettingValue)
                        End If
                    End If
                End If
                'AG 18/04/2012

                For Each myInstructionTO As InstructionParameterTO In pPreparationParameterList
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Axxx Analyzer Description
                            'myInstructionTO.Parameter = "A400"
                            myInstructionTO.ParameterValue = "A400" '1st & 2on characters: model, 3th & 4th characters: analyzer number
                            Exit Select
                        Case 2 'PREP Instruction Type
                            'myInstructionTO.Parameter = "PREP"
                            'myInstructionTO.ParameterValue = "TEST"
                            myInstructionTO.ParameterValue = myInstructionTO.InstructionType   'AG 06/05/2010

                            Exit Select
                        Case 3 'TI Preparation Type
                            'Get the Preparation Type
                            If pPreparationTestDataDS.vwksPreparationsTestData.Rows.Count > 0 Then
                                If pPreparationTestDataDS.vwksPreparationsTestData(0).ReagentsNumber > 2 Then
                                    myInstructionTO.ParameterValue = (pPreparationTestDataDS.vwksPreparationsTestData(0).ReagentsNumber + 3).ToString()
                                Else
                                    myInstructionTO.ParameterValue = pPreparationTestDataDS.vwksPreparationsTestData(0).ReagentsNumber.ToString()
                                End If
                            Else
                                myGlobalDataTO.HasError = True
                                Exit For
                            End If
                            Exit Select

                        Case 4 'ID 	Preparation identifier
                            'Get the id
                            myGlobalDataTO = mytwksWSPreparationsDAO.GeneratePreparationID(Nothing)
                            If Not myGlobalDataTO.HasError Then
                                myInstructionTO.ParameterValue = myGlobalDataTO.SetDatos.ToString()

                                ''AG 28/09/2012 - auxiliary code for test incomplete instruction
                                'If myInstructionTO.ParameterValue = "7" Then
                                '    myGlobalDataTO.HasError = True
                                '    Exit For
                                'End If
                                ''AG 28/09/2012

                            Else
                                Exit For
                            End If
                            Exit Select

                        Case 5 'M1 Sample position

                            If Not pWSExecutionDS.twksWSExecutions(0).IsSampleClassNull Then
                                If pWSExecutionDS.twksWSExecutions(0).SampleClass.Trim = "BLANK" Then
                                    'filter by the Special solution
                                    'AG 20/06/2011 - blank in tubes
                                    'qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                    '                 Where a.TubeContent.Trim = "SPEC_SOL" And a.Status.Trim <> "DEPLETED" _
                                    '                 Select a).ToList()

                                    'AG 03/08/2011 - Add condition: AndAlso a.Status.Trim <> "DEPLETED" And a.Status.Trim <> "LOCKED" And (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR")
                                    qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                                     Where a.TubeContent.Trim = "TUBE_SPEC_SOL" AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" _
                                                     AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                                     Select a).ToList()
                                    'AG 20/06/2011

                                    If qPositionData.Count > 0 Then
                                        myInstructionTO.ParameterValue = qPositionData.First().CellNumber.ToString()
                                    Else
                                        'if not special solution found set 0 as value (PENDING DEFINITION REAGENT)
                                        myInstructionTO.ParameterValue = "0"
                                    End If

                                Else
                                    qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                                     Where Not a.IsMultiItemNumberNull AndAlso _
                                                     a.MultiItemNumber = pWSExecutionDS.twksWSExecutions(0).MultiItemNumber _
                                                     Select a).ToList()

                                    If qPositionData.Count > 0 Then
                                        If pWSExecutionDS.twksWSExecutions(0).SampleClass.Trim = "CALIB" Then
                                            'Filter By TubeContent
                                            'AG 03/08/2011 - Add condition: AndAlso a.Status.Trim <> "DEPLETED" And a.Status.Trim <> "LOCKED" And (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR")
                                            'AG 18/04/2012 - change pWSExecutionDS.twksWSExecutions(0).MultiItemNumber for myMultiItemNumber
                                            qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                                     Where Not a.IsMultiItemNumberNull AndAlso _
                                                     a.MultiItemNumber = myMultiItemNumber _
                                                     And a.TubeContent = pWSExecutionDS.twksWSExecutions(0).SampleClass.Trim _
                                                     AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                                     Select a).ToList()
                                            If qPositionData.Count > 0 Then
                                                myInstructionTO.ParameterValue = qPositionData.First().CellNumber.ToString()
                                            Else
                                                myGlobalDataTO.HasError = True 'TR 21/05/2012 -If not value then return error.
                                                Exit For
                                            End If

                                        ElseIf pWSExecutionDS.twksWSExecutions(0).SampleClass.Trim = "CTRL" Then
                                            'Filter By TubeContent
                                            'AG 03/08/2011 - Add condition: AndAlso a.Status.Trim <> "DEPLETED" And a.Status.Trim <> "LOCKED" And (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR")
                                            qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                                     Where a.TubeContent = pWSExecutionDS.twksWSExecutions(0).SampleClass.Trim _
                                                     AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                                     Select a).ToList()

                                            If qPositionData.Count > 0 Then
                                                myInstructionTO.ParameterValue = qPositionData.First().CellNumber.ToString()
                                            Else
                                                'TR 21/05/2012 -If not value then return error.
                                                myGlobalDataTO.HasError = True
                                                Exit For
                                            End If

                                        ElseIf pWSExecutionDS.twksWSExecutions(0).SampleClass.Trim = "PATIENT" Then
                                            'Filter By TubeContent
                                            'AG 03/08/2011 - Add condition: AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                            qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                                     Where a.TubeContent = pWSExecutionDS.twksWSExecutions(0).SampleClass.Trim _
                                                     AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                                     Select a).ToList()

                                            If qPositionData.Count > 0 Then
                                                myInstructionTO.ParameterValue = qPositionData.First().CellNumber.ToString()
                                            Else
                                                'TR 21/05/2012 -If not value then return error.
                                                myGlobalDataTO.HasError = True
                                                Exit For
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                            Exit Select

                        Case 6 'TM1 Sample Tube Type

                            If Not pWSExecutionDS.twksWSExecutions(0).IsSampleClassNull Then
                                If pWSExecutionDS.twksWSExecutions(0).SampleClass.Trim = "BLANK" Then
                                    'validate if tube content is Special solution
                                    'AG 20/06/2011 - blank in tubes
                                    'qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                    '                  Where a.TubeContent = "SPEC_SOL" And a.Status.Trim <> "DEPLETED" _
                                    '                  Select a).ToList()
                                    'AG 03/08/2011 - Add condition: AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                    qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                                     Where a.TubeContent = "TUBE_SPEC_SOL" AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" _
                                                     AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                                     Select a).ToList()

                                    'AG 20/06/2011

                                    If qPositionData.Count > 0 Then
                                        myInstructionTO.ParameterValue = GetBottleCode(qPositionData.First().TubeType)
                                    Else
                                        'if not special solution found set 0 as value (PENDING DEFINITION REAGENT)
                                        myInstructionTO.ParameterValue = "0" 'OK, this value is valid only for blanks
                                    End If
                                ElseIf pWSExecutionDS.twksWSExecutions(0).SampleClass.Trim = "PATIENT" Then
                                    'AG 03/08/2011 - Add condition: AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                    qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                                    Where a.TubeContent = "PATIENT" AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" _
                                                    AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                                    Select a).ToList()

                                    If qPositionData.Count > 0 Then
                                        myInstructionTO.ParameterValue = GetBottleCode(qPositionData.First().TubeType)
                                    Else
                                        myInstructionTO.ParameterValue = "0"
                                        myGlobalDataTO.HasError = True 'AG 25/05/2012 - This is an invalid value for patients
                                        Exit For
                                    End If
                                ElseIf pWSExecutionDS.twksWSExecutions(0).SampleClass.Trim = "CALIB" Then
                                    'pWSExecutionDS.twksWSExecutions(0).SampleClass.Trim = "CTRL" Then 'AG 06/02/2012 remove ctrl case because multiitemnumber is null the linq return no items

                                    'AG 03/08/2011 - Add condition: AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                    'AG 18/04/2012 - change pWSExecutionDS.twksWSExecutions(0).MultiItemNumber for myMultiItemNumber
                                    qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                                    Where a.TubeContent = pWSExecutionDS.twksWSExecutions(0).SampleClass.Trim AndAlso _
                                                    Not a.IsMultiItemNumberNull AndAlso _
                                                    a.MultiItemNumber = myMultiItemNumber _
                                                    AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" _
                                                    AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                                    Select a).ToList()

                                    If qPositionData.Count > 0 Then
                                        myInstructionTO.ParameterValue = GetBottleCode(qPositionData.First().TubeType)
                                    Else
                                        myInstructionTO.ParameterValue = "0"
                                        myGlobalDataTO.HasError = True 'AG 25/05/2012 - This is an invalid value for calibrators
                                        Exit For
                                    End If

                                ElseIf pWSExecutionDS.twksWSExecutions(0).SampleClass.Trim = "CTRL" Then 'AG 06/02/2012
                                    'AG 03/08/2011 - Add condition: AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                    qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                                    Where a.TubeContent = pWSExecutionDS.twksWSExecutions(0).SampleClass.Trim _
                                                    AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" _
                                                    AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                                    Select a).ToList()

                                    If qPositionData.Count > 0 Then
                                        myInstructionTO.ParameterValue = GetBottleCode(qPositionData.First().TubeType)
                                    Else
                                        myInstructionTO.ParameterValue = "0"
                                        myGlobalDataTO.HasError = True 'AG 25/05/2012 - This is an invalid value for controls
                                        Exit For
                                    End If

                                End If
                            End If

                            Exit Select

                        Case 7 'RM1 Sample rotor type
                            myInstructionTO.ParameterValue = "0"
                            Exit Select

                        Case 8 'R1 Reagent1 position
                            'AG 03/08/2011 - Add condition: AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                            qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                            Where a.TubeContent = "REAGENT" AndAlso (Not a.IsMultiItemNumberNull AndAlso a.MultiItemNumber = 1) _
                                            AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" _
                                            AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                            Select a).ToList()

                            If qPositionData.Count > 0 Then
                                'if there are more than one bottler positioned, select the first one.
                                myInstructionTO.ParameterValue = qPositionData.First().CellNumber.ToString()
                            Else
                                'TR 21/05/2012 -If not value then return error.
                                myGlobalDataTO.HasError = True
                                Exit For
                            End If
                            Exit Select
                        Case 9 'R1 Reagent1 Bottle Type
                            'AG 03/08/2011 - Add condition: AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                            qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                            Where a.TubeContent = "REAGENT" AndAlso (Not a.IsMultiItemNumberNull AndAlso a.MultiItemNumber = 1) _
                                            AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" _
                                            AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                            Select a).ToList()

                            If qPositionData.Count > 0 Then
                                myInstructionTO.ParameterValue = GetBottleCode(qPositionData.First().TubeType)
                            Else
                                'TR 21/05/2012 -If not value then return error.
                                myGlobalDataTO.HasError = True
                                Exit For
                            End If

                            Exit Select

                        Case 10 'RR1 Reagent1 Rotor Type 
                            myInstructionTO.ParameterValue = "0"
                            Exit Select

                        Case 11 'R2 Reagent2 position
                            'AG 03/08/2011 - Add condition: AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                            qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                            Where a.TubeContent = "REAGENT" AndAlso (Not a.IsMultiItemNumberNull AndAlso a.MultiItemNumber = 2) _
                                            AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" _
                                            AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                            Select a).ToList()

                            If qPositionData.Count > 0 Then
                                myInstructionTO.ParameterValue = qPositionData.First().CellNumber.ToString()
                            Else
                                myInstructionTO.ParameterValue = "0"

                                'AG 25/05/2012 - Check if preparations belongs to a bireagent test. In this case position 0 is an invalid value and Sw must return error
                                qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                                 Where a.TubeContent = "REAGENT" AndAlso (Not a.IsMultiItemNumberNull AndAlso a.MultiItemNumber = 2) _
                                                 Select a).ToList()
                                If qPositionData.Count > 0 Then
                                    myGlobalDataTO.HasError = True 'AG 25/05/2012 - 0 is an invalid value 
                                    Exit For
                                End If
                                'AG 25/05/2012
                            End If
                            Exit Select

                        Case 12 'TR2 Reagent2 bottle type.
                            'AG 03/08/2011 - Add condition: AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                            qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                           Where a.TubeContent = "REAGENT" AndAlso (Not a.IsMultiItemNumberNull AndAlso a.MultiItemNumber = 2) _
                                           AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" _
                                           AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                           Select a).ToList()

                            If qPositionData.Count > 0 Then
                                myInstructionTO.ParameterValue = GetBottleCode(qPositionData.First().TubeType)
                            Else
                                myInstructionTO.ParameterValue = "0"
                            End If
                            Exit Select

                        Case 13 'RR2 Reagent2 rotor type
                            myInstructionTO.ParameterValue = "0"
                            Exit Select

                        Case 14 'VM1 Sample volume

                            'AG 31/12/2010
                            'qTestData = (From a In myPreparationTestDataDS.vwksPreparationsTestData _
                            '            Where a.OrderTestID = myOrderTestID _
                            '            Select a).ToList()
                            qTestData.Clear()
                            If pWSExecutionDS.twksWSExecutions(0).SampleClass.Trim = "BLANK" And _
                               pPreparationTestDataDS.vwksPreparationsTestData(0).BlankMode = "REAGENT" Then
                            Else
                                qTestData = (From a In pPreparationTestDataDS.vwksPreparationsTestData _
                                            Where a.OrderTestID = myOrderTestID _
                                            Select a).ToList()
                            End If
                            'AG 31/12/2010

                            If qTestData.Count > 0 Then
                                If Not qTestData.First().IsSampleVolumeStepsNull Then
                                    'AG 12/01/2012 - Use the correct post volumes
                                    'myInstructionTO.ParameterValue = CType(qTestData.First().SampleVolumeSteps, Integer).ToString()
                                    If pWSExecutionDS.twksWSExecutions(0).IsPostDilutionTypeNull Then
                                        myInstructionTO.ParameterValue = CType(qTestData.First().SampleVolumeSteps, Integer).ToString()
                                    Else
                                        Select Case pWSExecutionDS.twksWSExecutions(0).PostDilutionType
                                            Case "NONE"
                                                myInstructionTO.ParameterValue = CType(qTestData.First().SampleVolumeSteps, Integer).ToString()
                                            Case "INC"
                                                myInstructionTO.ParameterValue = CType(qTestData.First().IncPostSampleVolumeSteps, Integer).ToString()
                                            Case "RED"
                                                myInstructionTO.ParameterValue = CType(qTestData.First().RedPostSampleVolumeSteps, Integer).ToString()
                                            Case Else
                                                myGlobalDataTO.HasError = True 'AG 25/05/2012 - This is an invalid value 
                                                Exit For
                                        End Select
                                    End If
                                    'AG 12/01/2012

                                Else
                                    myInstructionTO.ParameterValue = "0"
                                End If
                            Else
                                myInstructionTO.ParameterValue = "0" 'AG 31/12/2010
                            End If
                            Exit Select

                        Case 15 'VR1 Reagent1 volume
                            qTestData = (From a In pPreparationTestDataDS.vwksPreparationsTestData _
                                         Where a.OrderTestID = myOrderTestID And a.ReagentNumber = 1 _
                                         Select a).ToList()

                            If qTestData.Count > 0 Then
                                myInstructionTO.ParameterValue = CType(qTestData.First().ReagentVolumeSteps, Integer).ToString()
                                'AG 12/01/2012 - Use the correct post volumes
                                'myInstructionTO.ParameterValue = CType(qTestData.First().ReagentVolumeSteps, Integer).ToString()
                                If pWSExecutionDS.twksWSExecutions(0).IsPostDilutionTypeNull Then
                                    myInstructionTO.ParameterValue = CType(qTestData.First().ReagentVolumeSteps, Integer).ToString()
                                Else
                                    Select Case pWSExecutionDS.twksWSExecutions(0).PostDilutionType
                                        Case "NONE"
                                            myInstructionTO.ParameterValue = CType(qTestData.First().ReagentVolumeSteps, Integer).ToString()
                                        Case "INC"
                                            myInstructionTO.ParameterValue = CType(qTestData.First().IncPostReagentVolumeSteps, Integer).ToString()
                                        Case "RED"
                                            myInstructionTO.ParameterValue = CType(qTestData.First().RedPostReagentVolumeSteps, Integer).ToString()
                                        Case Else
                                            myGlobalDataTO.HasError = True 'AG 25/05/2012 - This is an invalid value 
                                            Exit For
                                    End Select
                                End If
                                'AG 12/01/2012
                            Else
                                myInstructionTO.ParameterValue = "0"
                            End If
                            Exit Select

                        Case 16 'VR2 Reagent2 volume
                            qTestData = (From a In pPreparationTestDataDS.vwksPreparationsTestData _
                                        Where a.OrderTestID = myOrderTestID And a.ReagentNumber = 2 _
                                        Select a).ToList()

                            If qTestData.Count > 0 Then
                                'AG 12/01/2012 - Use the correct post volumes
                                'myInstructionTO.ParameterValue = CType(qTestData.First().ReagentVolumeSteps, Integer).ToString()
                                If pWSExecutionDS.twksWSExecutions(0).IsPostDilutionTypeNull Then
                                    myInstructionTO.ParameterValue = CType(qTestData.First().ReagentVolumeSteps, Integer).ToString()
                                Else
                                    Select Case pWSExecutionDS.twksWSExecutions(0).PostDilutionType
                                        Case "NONE"
                                            myInstructionTO.ParameterValue = CType(qTestData.First().ReagentVolumeSteps, Integer).ToString()
                                        Case "INC"
                                            myInstructionTO.ParameterValue = CType(qTestData.First().IncPostReagentVolumeSteps, Integer).ToString()
                                        Case "RED"
                                            myInstructionTO.ParameterValue = CType(qTestData.First().RedPostReagentVolumeSteps, Integer).ToString()
                                        Case Else
                                            myGlobalDataTO.HasError = True 'AG 25/05/2012 - This is an invalid value 
                                            Exit For
                                    End Select
                                End If
                                'AG 12/01/2012
                            Else
                                myInstructionTO.ParameterValue = "0"
                            End If
                            Exit Select

                        Case 17 'MW Main wavelength
                            qTestData = (From a In pPreparationTestDataDS.vwksPreparationsTestData _
                                       Where a.OrderTestID = myOrderTestID _
                                       Select a).ToList()

                            If qTestData.Count > 0 Then
                                'Get the let position from the MainWaveLength value
                                myGlobalDataTO = myAnalyzerLedPositionsDelegate.GetByWaveLength(Nothing, _
                                                            pWSExecutionDS.twksWSExecutions(0).AnalyzerID, _
                                                            qTestData.First().MainWavelength.Trim())

                                If Not myGlobalDataTO.HasError Then
                                    Dim myAnalyLedPosDS As New AnalyzerLedPositionsDS
                                    myAnalyLedPosDS = DirectCast(myGlobalDataTO.SetDatos, AnalyzerLedPositionsDS)
                                    If myAnalyLedPosDS.tcfgAnalyzerLedPositions.Rows.Count > 0 Then
                                        myInstructionTO.ParameterValue = myAnalyLedPosDS.tcfgAnalyzerLedPositions(0).LedPosition.ToString()
                                    Else
                                        myGlobalDataTO.HasError = True 'AG 25/05/2012 - This is an invalid value 
                                        Exit For
                                    End If
                                Else
                                    Exit Try
                                End If
                            End If
                            Exit Select

                        Case 18 'RW Reference wavelength. 
                            qTestData = (From a In pPreparationTestDataDS.vwksPreparationsTestData _
                                       Where a.OrderTestID = myOrderTestID _
                                       Select a).ToList()

                            If qTestData.Count > 0 Then
                                Dim myLength As String
                                If Not qTestData.First().IsReferenceWavelengthNull Then
                                    myLength = qTestData.First().ReferenceWavelength.Trim
                                Else
                                    myLength = qTestData.First().MainWavelength.Trim
                                End If

                                'Get the let position from the MainWaveLength value
                                myGlobalDataTO = myAnalyzerLedPositionsDelegate.GetByWaveLength(Nothing, _
                                                            pWSExecutionDS.twksWSExecutions(0).AnalyzerID, _
                                                            myLength)
                                If Not myGlobalDataTO.HasError Then
                                    Dim myAnalyLedPosDS As New AnalyzerLedPositionsDS
                                    myAnalyLedPosDS = DirectCast(myGlobalDataTO.SetDatos, AnalyzerLedPositionsDS)
                                    If myAnalyLedPosDS.tcfgAnalyzerLedPositions.Rows.Count > 0 Then
                                        myInstructionTO.ParameterValue = myAnalyLedPosDS.tcfgAnalyzerLedPositions(0).LedPosition.ToString()
                                    Else
                                        myGlobalDataTO.HasError = True 'AG 25/05/2012 - This is an invalid value 
                                        Exit For
                                    End If
                                Else
                                    Exit Try
                                End If

                            End If
                            Exit Select
                        Case 19 'RN Readings number
                            qTestData = (From a In pPreparationTestDataDS.vwksPreparationsTestData _
                                       Where a.OrderTestID = myOrderTestID _
                                       Select a).ToList()

                            If qTestData.Count > 0 Then
                                If Not qTestData.First().IsSecondReadingCycleNull Then
                                    myInstructionTO.ParameterValue = qTestData.First().SecondReadingCycle.ToString()
                                Else
                                    myInstructionTO.ParameterValue = qTestData.First().FirstReadingCycle.ToString()
                                End If
                            Else
                                myGlobalDataTO.HasError = True 'AG 25/05/2012 - This is an invalid value 
                                Exit For
                            End If
                            Exit Select

                        Case Else
                            Exit Select
                    End Select
                Next
                'TR 21/05/2012 -Validate there're no error.
                If Not myGlobalDataTO.HasError Then
                    'AG 11/03/2011 - business if RN (18) is odd: instrucion is OK || if even: changes OW (16) and EW (17) in order to use the main filter
                    'in the last reading before calculations
                    'AG 13/11/2014 BA-2118
                    'Dim myCycle As Integer = CInt(pPreparationParameterList(18).ParameterValue)
                    Dim myCycle As Integer = CInt(pPreparationParameterList(18).ParameterValue) - pSwReadingOffset

                    If myCycle Mod 2 = 0 Then
                        Dim auxValue As String = pPreparationParameterList(16).ParameterValue
                        pPreparationParameterList(16).ParameterValue = pPreparationParameterList(17).ParameterValue
                        pPreparationParameterList(17).ParameterValue = auxValue
                    End If
                    'END AG 11/03/2011 

                    myGlobalDataTO.SetDatos = pPreparationParameterList
                End If
                'TR 21/05/2012 -END.

                qTestData = Nothing 'AG 02/08/2012 - free memory
                qPositionData = Nothing 'AG 02/08/2012 - free memory

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.CreateTestPreparation", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO

        End Function

        ''' <summary>
        ''' Create the test preparation for instruction type PTEST.
        ''' </summary>
        ''' <param name="pPreparationParameterList"></param>
        ''' <param name="pWSExecutionDS"></param>
        ''' <param name="pPreparationTestDataDS"></param>
        ''' <param name="pPreparationPositionDS"></param>
        ''' <param name="pSwReadingOffset">Cycles between sample dispensation and the 1st reading</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  TR 18/01/2011
        ''' Modified by: SA 27/01/2011 - When getting the type and position of the bottle of the Special Solution needed to 
        '''                              execute an automatic dilution (Instruction Index 21 and 20), filter for the DiluentSolution 
        '''                              defined for the Test/SampleType
        ''' TR 21/05/2012 - Return Error when instruction incomplete
        ''' AG 25/05/2012 - complete cases for return error when instruction incomplete: r2, vm1, vr1, vr2, mw, rw, rn, pm1, ptm
        ''' AG 13/11/2014 BA-2118 add parameter pSwReadingOffset</remarks>
        Private Function FillPTestPreparation(ByVal pPreparationParameterList As List(Of InstructionParameterTO), _
                                              ByVal pWSExecutionDS As ExecutionsDS, _
                                              ByVal pPreparationTestDataDS As PreparationsTestDataDS, _
                                              ByVal pPreparationPositionDS As PreparationsPositionDataDS, _
                                              ByVal pSwReadingOffset As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim mytwksWSPreparationsDAO As New twksWSPreparationsDAO
                Dim myAnalyzerLedPositionsDelegate As New AnalyzerLedPositionsDelegate
                Dim qTestData As New List(Of PreparationsTestDataDS.vwksPreparationsTestDataRow)
                Dim qPositionData As New List(Of PreparationsPositionDataDS.vwksPreparationsPositionDataRow)
                Dim myOrderTestID As Integer = 0

                If pWSExecutionDS.twksWSExecutions.Count > 0 Then
                    myOrderTestID = pWSExecutionDS.twksWSExecutions(0).OrderTestID
                End If


                For Each myInstructionTO As InstructionParameterTO In pPreparationParameterList
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Axxx Analyzer Description
                            myInstructionTO.ParameterValue = "A400" '1st & 2on characters: model, 3th & 4th characters: analyzer number
                            Exit Select

                        Case 2 'PREP Instruction Type
                            myInstructionTO.ParameterValue = myInstructionTO.InstructionType   'AG 06/05/2010
                            Exit Select

                        Case 3 'TI Preparation Type
                            'Get the Preparation Type
                            If pPreparationTestDataDS.vwksPreparationsTestData.Rows.Count > 0 Then
                                If pPreparationTestDataDS.vwksPreparationsTestData(0).ReagentsNumber > 2 Then
                                    myInstructionTO.ParameterValue = (pPreparationTestDataDS.vwksPreparationsTestData(0).ReagentsNumber + 3).ToString()
                                Else
                                    myInstructionTO.ParameterValue = pPreparationTestDataDS.vwksPreparationsTestData(0).ReagentsNumber.ToString()
                                End If
                            Else
                                myGlobalDataTO.HasError = True
                                Exit For
                            End If
                            Exit Select

                        Case 4 'ID 	Preparation identifier
                            'Get the id
                            myGlobalDataTO = mytwksWSPreparationsDAO.GeneratePreparationID(Nothing)
                            If Not myGlobalDataTO.HasError Then
                                myInstructionTO.ParameterValue = myGlobalDataTO.SetDatos.ToString()

                                ''AG 28/09/2012 - auxiliary code for test incomplete instruction
                                'If myInstructionTO.ParameterValue = "7" Then
                                '    myGlobalDataTO.HasError = True
                                '    Exit For
                                'End If
                                ''AG 28/09/2012
                            Else
                                Exit For
                            End If
                            Exit Select

                        Case 5 'R1 Reagent1 position
                            'AG 04/08/2011 - Add condition: AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR")
                            qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                            Where a.TubeContent = "REAGENT" AndAlso (Not a.IsMultiItemNumberNull AndAlso a.MultiItemNumber = 1) _
                                            AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                            Select a).ToList()

                            If qPositionData.Count > 0 Then
                                'if there are more than one bottler positioned, select the first one.
                                myInstructionTO.ParameterValue = qPositionData.First().CellNumber.ToString()
                            Else
                                'TR 21/05/2012 -If not value then return error.
                                myGlobalDataTO.HasError = True
                                Exit For
                            End If
                            Exit Select

                        Case 6 'R1 Reagent1 Bottle Type
                            'AG 04/08/2011 - Add condition: AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR")
                            qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                            Where a.TubeContent = "REAGENT" AndAlso (Not a.IsMultiItemNumberNull AndAlso a.MultiItemNumber = 1) _
                                            AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                            Select a).ToList()

                            If qPositionData.Count > 0 Then
                                myInstructionTO.ParameterValue = GetBottleCode(qPositionData.First().TubeType)
                            Else
                                'TR 21/05/2012 -If not value then return error.
                                myGlobalDataTO.HasError = True
                                Exit For
                            End If
                            Exit Select

                        Case 7 'RR1 Reagent1 Rotor Type 
                            myInstructionTO.ParameterValue = "0"
                            Exit Select

                        Case 8 'R2 Reagent2 position
                            'AG 04/08/2011 - Add condition: AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR")
                            qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                            Where a.TubeContent = "REAGENT" AndAlso (Not a.IsMultiItemNumberNull AndAlso a.MultiItemNumber = 2) _
                                            AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                            Select a).ToList()

                            If qPositionData.Count > 0 Then
                                myInstructionTO.ParameterValue = qPositionData.First().CellNumber.ToString()
                            Else
                                myInstructionTO.ParameterValue = "0"

                                'AG 25/05/2012 - Check if preparations belongs to a bireagent test. In this case position 0 is an invalid value and Sw must return error
                                qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                                 Where a.TubeContent = "REAGENT" AndAlso (Not a.IsMultiItemNumberNull AndAlso a.MultiItemNumber = 2) _
                                                 Select a).ToList()
                                If qPositionData.Count > 0 Then
                                    myGlobalDataTO.HasError = True 'AG 25/05/2012 - 0 is an invalid value 
                                    Exit For
                                End If
                                'AG 25/05/2012
                            End If
                            Exit Select

                        Case 9 'TR2 Reagent2 bottle type.
                            'AG 04/08/2011 - Add condition: AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR")
                            qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                           Where a.TubeContent = "REAGENT" AndAlso (Not a.IsMultiItemNumberNull AndAlso a.MultiItemNumber = 2) _
                                           AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                           Select a).ToList()

                            If qPositionData.Count > 0 Then
                                myInstructionTO.ParameterValue = GetBottleCode(qPositionData.First().TubeType)
                            Else
                                myInstructionTO.ParameterValue = "0"
                            End If
                            Exit Select

                        Case 10 'RR2 Reagent2 rotor type
                            myInstructionTO.ParameterValue = "0"
                            Exit Select

                        Case 11 'VM1 Sample volume

                            qTestData.Clear()
                            If pWSExecutionDS.twksWSExecutions(0).SampleClass.Trim = "BLANK" And _
                               pPreparationTestDataDS.vwksPreparationsTestData(0).BlankMode = "REAGENT" Then
                            Else
                                qTestData = (From a In pPreparationTestDataDS.vwksPreparationsTestData _
                                            Where a.OrderTestID = myOrderTestID Select a).ToList()
                            End If

                            If qTestData.Count > 0 Then
                                If Not qTestData.First().IsSampleVolumeStepsNull Then
                                    'AG 12/01/2012 - Use the correct post volumes
                                    'myInstructionTO.ParameterValue = CType(qTestData.First().SampleVolumeSteps, Integer).ToString()
                                    If pWSExecutionDS.twksWSExecutions(0).IsPostDilutionTypeNull Then
                                        myInstructionTO.ParameterValue = CType(qTestData.First().SampleVolumeSteps, Integer).ToString()
                                    Else
                                        Select Case pWSExecutionDS.twksWSExecutions(0).PostDilutionType
                                            Case "NONE"
                                                myInstructionTO.ParameterValue = CType(qTestData.First().SampleVolumeSteps, Integer).ToString()
                                            Case "INC"
                                                myInstructionTO.ParameterValue = CType(qTestData.First().IncPostSampleVolumeSteps, Integer).ToString()
                                            Case "RED"
                                                myInstructionTO.ParameterValue = CType(qTestData.First().RedPostSampleVolumeSteps, Integer).ToString()
                                            Case Else
                                                myGlobalDataTO.HasError = True 'AG 25/05/2012 - This is an invalid value 
                                                Exit For
                                        End Select
                                    End If
                                    'AG 12/01/2012

                                Else
                                    myInstructionTO.ParameterValue = "0"
                                End If
                            Else
                                myInstructionTO.ParameterValue = "0"
                            End If
                            Exit Select

                        Case 12 'VR1 Reagent1 volume
                            qTestData = (From a In pPreparationTestDataDS.vwksPreparationsTestData _
                                         Where a.OrderTestID = myOrderTestID And a.ReagentNumber = 1 _
                                         Select a).ToList()

                            If qTestData.Count > 0 Then
                                'AG 12/01/2012 - Use the correct post volumes
                                'myInstructionTO.ParameterValue = CType(qTestData.First().ReagentVolumeSteps, Integer).ToString()
                                If pWSExecutionDS.twksWSExecutions(0).IsPostDilutionTypeNull Then
                                    myInstructionTO.ParameterValue = CType(qTestData.First().ReagentVolumeSteps, Integer).ToString()
                                Else
                                    Select Case pWSExecutionDS.twksWSExecutions(0).PostDilutionType
                                        Case "NONE"
                                            myInstructionTO.ParameterValue = CType(qTestData.First().ReagentVolumeSteps, Integer).ToString()
                                        Case "INC"
                                            myInstructionTO.ParameterValue = CType(qTestData.First().IncPostReagentVolumeSteps, Integer).ToString()
                                        Case "RED"
                                            myInstructionTO.ParameterValue = CType(qTestData.First().RedPostReagentVolumeSteps, Integer).ToString()
                                        Case Else
                                            myGlobalDataTO.HasError = True 'AG 25/05/2012 - This is an invalid value 
                                            Exit For
                                    End Select
                                End If
                                'AG 12/01/2012

                            Else
                                myInstructionTO.ParameterValue = "0"
                            End If
                            Exit Select

                        Case 13 'VR2 Reagent2 volume
                            qTestData = (From a In pPreparationTestDataDS.vwksPreparationsTestData _
                                        Where a.OrderTestID = myOrderTestID And a.ReagentNumber = 2 _
                                        Select a).ToList()

                            If qTestData.Count > 0 Then
                                'AG 12/01/2012 - Use the correct post volumes
                                'myInstructionTO.ParameterValue = CType(qTestData.First().ReagentVolumeSteps, Integer).ToString()
                                If pWSExecutionDS.twksWSExecutions(0).IsPostDilutionTypeNull Then
                                    myInstructionTO.ParameterValue = CType(qTestData.First().ReagentVolumeSteps, Integer).ToString()
                                Else
                                    Select Case pWSExecutionDS.twksWSExecutions(0).PostDilutionType
                                        Case "NONE"
                                            myInstructionTO.ParameterValue = CType(qTestData.First().ReagentVolumeSteps, Integer).ToString()
                                        Case "INC"
                                            myInstructionTO.ParameterValue = CType(qTestData.First().IncPostReagentVolumeSteps, Integer).ToString()
                                        Case "RED"
                                            myInstructionTO.ParameterValue = CType(qTestData.First().RedPostReagentVolumeSteps, Integer).ToString()
                                        Case Else
                                            myGlobalDataTO.HasError = True 'AG 25/05/2012 - This is an invalid value 
                                            Exit For
                                    End Select
                                End If
                                'AG 12/01/2012

                            Else
                                myInstructionTO.ParameterValue = "0"
                            End If
                            Exit Select

                        Case 14 'MW Main wavelength
                            qTestData = (From a In pPreparationTestDataDS.vwksPreparationsTestData _
                                       Where a.OrderTestID = myOrderTestID _
                                       Select a).ToList()

                            If qTestData.Count > 0 Then
                                'Get the let position from the MainWaveLength value
                                myGlobalDataTO = myAnalyzerLedPositionsDelegate.GetByWaveLength(Nothing, _
                                                            pWSExecutionDS.twksWSExecutions(0).AnalyzerID, _
                                                            qTestData.First().MainWavelength.Trim())

                                If Not myGlobalDataTO.HasError Then
                                    Dim myAnalyLedPosDS As New AnalyzerLedPositionsDS
                                    myAnalyLedPosDS = DirectCast(myGlobalDataTO.SetDatos, AnalyzerLedPositionsDS)
                                    If myAnalyLedPosDS.tcfgAnalyzerLedPositions.Rows.Count > 0 Then
                                        myInstructionTO.ParameterValue = myAnalyLedPosDS.tcfgAnalyzerLedPositions(0).LedPosition.ToString()
                                    Else
                                        myGlobalDataTO.HasError = True 'AG 25/05/2012 - This is an invalid value 
                                        Exit For
                                    End If
                                Else
                                    Exit Try
                                End If
                            End If
                            Exit Select

                        Case 15 'RW Reference wavelength. 
                            qTestData = (From a In pPreparationTestDataDS.vwksPreparationsTestData _
                                       Where a.OrderTestID = myOrderTestID Select a).ToList()

                            If qTestData.Count > 0 Then
                                Dim myLength As String
                                If Not qTestData.First().IsReferenceWavelengthNull Then
                                    myLength = qTestData.First().ReferenceWavelength.Trim
                                Else
                                    myLength = qTestData.First().MainWavelength.Trim
                                End If

                                'Get the let position from the MainWaveLength value
                                myGlobalDataTO = myAnalyzerLedPositionsDelegate.GetByWaveLength(Nothing, _
                                                            pWSExecutionDS.twksWSExecutions(0).AnalyzerID, myLength)
                                If Not myGlobalDataTO.HasError Then
                                    Dim myAnalyLedPosDS As New AnalyzerLedPositionsDS
                                    myAnalyLedPosDS = DirectCast(myGlobalDataTO.SetDatos, AnalyzerLedPositionsDS)
                                    If myAnalyLedPosDS.tcfgAnalyzerLedPositions.Rows.Count > 0 Then
                                        myInstructionTO.ParameterValue = myAnalyLedPosDS.tcfgAnalyzerLedPositions(0).LedPosition.ToString()
                                    Else
                                        myGlobalDataTO.HasError = True 'AG 25/05/2012 - This is an invalid value 
                                        Exit For
                                    End If
                                Else
                                    Exit Try
                                End If
                            End If
                            Exit Select

                        Case 16 'RN Readings number
                            qTestData = (From a In pPreparationTestDataDS.vwksPreparationsTestData _
                                       Where a.OrderTestID = myOrderTestID Select a).ToList()

                            If qTestData.Count > 0 Then
                                If Not qTestData.First().IsSecondReadingCycleNull Then
                                    myInstructionTO.ParameterValue = qTestData.First().SecondReadingCycle.ToString()
                                Else
                                    myInstructionTO.ParameterValue = qTestData.First().FirstReadingCycle.ToString()
                                End If
                            Else
                                myGlobalDataTO.HasError = True 'AG 25/05/2012 - This is an invalid value 
                                Exit For
                            End If
                            Exit Select

                        Case 17 'Sample Tube Position for predilution 

                            'Validate sample class is not null value to get the tube position by the tube content.
                            If Not pWSExecutionDS.twksWSExecutions(0).IsSampleClassNull Then
                                'Filter by the Control
                                'AG 04/08/2011 - Add condition: AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR")
                                qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                                 Where a.TubeContent.Trim = pWSExecutionDS.twksWSExecutions(0).SampleClass.Trim _
                                                 AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                                 Select a).ToList()
                            End If

                            'Set the value 
                            If qPositionData.Count > 0 Then
                                myInstructionTO.ParameterValue = qPositionData.First().CellNumber.ToString()
                            Else
                                myInstructionTO.ParameterValue = "0"
                                myGlobalDataTO.HasError = True 'AG 25/05/2012 - This is an invalid value 
                                Exit For
                            End If

                            Exit Select

                        Case 18 'Sample Tube type
                            If pWSExecutionDS.twksWSExecutions(0).SampleClass.Trim = "PATIENT" OrElse _
                                                        pWSExecutionDS.twksWSExecutions(0).SampleClass.Trim = "CTRL" Then

                                'AG 04/08/2011 - Add condition: AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR")
                                qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                                Where a.TubeContent = pWSExecutionDS.twksWSExecutions(0).SampleClass.Trim _
                                                AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                                Select a).ToList()

                                If qPositionData.Count > 0 Then
                                    myInstructionTO.ParameterValue = GetBottleCode(qPositionData.First().TubeType)
                                Else
                                    myInstructionTO.ParameterValue = "0"
                                    myGlobalDataTO.HasError = True 'AG 25/05/2012 - This is an invalid value 
                                    Exit For
                                End If
                            End If
                            Exit Select
                        Case 19 'Sample Rotor Type

                            myInstructionTO.ParameterValue = "0"
                            Exit Select

                        Case 20 'Special Solution Bottle Position.
                            'AG 05/07/2011 - special solutions for predilutions are in the reagents rotor not in the samples rotor
                            'AG 04/08/2011 - Add condition: AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR")
                            qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                            Where a.TubeContent = "SPEC_SOL" _
                                            AndAlso a.SolutionCode = pPreparationTestDataDS.vwksPreparationsTestData(0).DiluentSolution _
                                            AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                            Select a).ToList()

                            If qPositionData.Count > 0 Then
                                'If there are more than one bottler positioned, select the first one.
                                myInstructionTO.ParameterValue = qPositionData.First().CellNumber.ToString()
                            Else
                                'TR 21/05/2012 -If not value then return error.
                                myGlobalDataTO.HasError = True
                                Exit For
                            End If
                            Exit Select

                        Case 21 'Special Solution Bottle type
                            'AG 05/07/2011 - special solutions for predilutions are in the reagents rotor not in the samples rotor
                            'AG 04/08/2011 - Add condition: AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR")
                            qPositionData = (From a In pPreparationPositionDS.vwksPreparationsPositionData _
                                            Where a.TubeContent = "SPEC_SOL" _
                                            AndAlso a.SolutionCode = pPreparationTestDataDS.vwksPreparationsTestData(0).DiluentSolution _
                                            AndAlso a.Status.Trim <> "DEPLETED" AndAlso a.Status.Trim <> "LOCKED" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus <> "ERROR") _
                                            Select a).ToList()

                            If qPositionData.Count > 0 Then
                                myInstructionTO.ParameterValue = GetBottleCode(qPositionData.First().TubeType)
                            Else
                                'TR 21/05/2012 -If not value then return error.
                                myGlobalDataTO.HasError = True
                                Exit For
                            End If
                            Exit Select

                        Case 22 'Special Solution Rotor Type
                            myInstructionTO.ParameterValue = "0"
                            Exit Select

                        Case 23 'Sample Volume For Predilution.
                            myInstructionTO.ParameterValue = CInt(pPreparationTestDataDS.vwksPreparationsTestData(0).PredilutedSampleVolSteps).ToString()
                            Exit Select

                        Case 24 'Reagent Volume for Predilution
                            myInstructionTO.ParameterValue = CInt(pPreparationTestDataDS.vwksPreparationsTestData(0).PreDiluentVolSteps).ToString()
                            Exit Select

                        Case Else
                            Exit Select
                    End Select
                Next

                'TR 21/05/2012 -Validate there was no error.
                If Not myGlobalDataTO.HasError Then
                    'AG 11/03/2011 - business if RN (15) is odd instrucion is OK, if even changes OW (13) and EW (14) in order to used the main filter
                    'in the last reading before calculations
                    'AG 13/11/2014 BA-2118
                    'Dim myCycle As Integer = CInt(pPreparationParameterList(15).ParameterValue)
                    Dim myCycle As Integer = CInt(pPreparationParameterList(15).ParameterValue) - pSwReadingOffset

                    If myCycle Mod 2 = 0 Then
                        Dim auxValue As String = pPreparationParameterList(13).ParameterValue
                        pPreparationParameterList(13).ParameterValue = pPreparationParameterList(14).ParameterValue
                        pPreparationParameterList(14).ParameterValue = auxValue
                    End If
                    'END AG 11/03/2011 
                    myGlobalDataTO.SetDatos = pPreparationParameterList
                End If
                qTestData = Nothing 'AG 02/08/2012 - free memory
                qPositionData = Nothing 'AG 02/08/2012 - free memory

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.CreatePTestPreparation", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO

        End Function

        ''' <summary>
        ''' Generate a InstructionParameterList from the ParametersTO received from Analyzer
        ''' TODO: TR review
        ''' </summary>
        ''' <param name="pParameterList"></param>
        ''' <returns>GlobalDataTo with data as InstructionParameterList</returns>
        ''' <remarks>
        ''' Created by:  AG 22/04/2010 (Tested OK)
        ''' </remarks>
        Private Function GenerateFixedReception(ByVal pParameterList As List(Of ParametersTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myParameterValue As String = ""

                'Get the instruction type (Always the 2on parameter in list)                
                myParameterValue = pParameterList(1).ParameterID

                Dim myIndexedList As New List(Of InstructionParameterTO)
                myIndexedList = GetInstructionParameter(myParameterValue)

                If myIndexedList.Count > 0 Then '(1) AG 28/10/2010
                    Dim query As New List(Of ParametersTO)
                    Dim iteration As Integer = 0    'AG 11/05/2010
                    For Each myIndexTO As InstructionParameterTO In myIndexedList
                        'Linq the ParameterID and when found assign the ParameterValue
                        Dim myID As String = myIndexTO.Parameter
                        query = (From a In pParameterList Where a.ParameterID = myID Select a).ToList

                        'If found ... assign value
                        'AG 11/05/2010 - The 2 first parameters hasnt ParameterID (A400;instructiontype;...)
                        If iteration <= 1 Then
                            myIndexTO.Parameter = pParameterList(iteration).ParameterID
                            myIndexTO.ParameterValue = pParameterList(iteration).ParameterID
                            'END AG 11/05/2010 

                        ElseIf query.Count > 0 Then
                            myIndexTO.ParameterValue = query(0).ParameterValues
                        End If
                        iteration = iteration + 1

                    Next
                    query = Nothing 'AG 02/08/2012 - free memory

                    myGlobal.SetDatos = myIndexedList

                Else '(1)
                    myGlobal.HasError = True
                    myGlobal.ErrorCode = GlobalEnumerates.Messages.WRONG_DATATYPE.ToString
                End If '(1)

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateFixedReception", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Generate the Read type Instruction.
        ''' </summary>
        ''' <param name="pParameterValue"></param>
        ''' <param name="pParameterList"></param>
        ''' <param name="pIndexedList"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  TR 21/05/2010
        ''' Modified by: AG 20/04/2011 - Changes in instruction definition Excel Instruction (Rev23) - No changes
        '''              TR 10/10/2013 - BT #1319 ==> Changes to get new field ST (Status of photometric Readings: 0=Normal Reading cycle / 1=Pause reading cycle) 
        '''                              from the list of ANSPHR Parameters to return (ParameterIndex = 47).
        ''' </remarks>
        Public Function GenerateANSPHRInstruction(ByVal pParameterValue As String, ByVal pParameterList As List(Of ParametersTO), _
                                                 ByVal pIndexedList As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myID As String = ""
                Dim myIndexNumber As Integer = -1
                Dim iteration As Integer = 0 'AG 11/05/2010
                Dim myResultCounts As Integer = 0
                Dim setFirstParam As Boolean = False
                Dim myFirstResultParam As String = ""
                Dim query As New List(Of ParametersTO)
                'Dim myIndexedList As New List(Of InstructionParameterTO)
                Dim myTemIndexedList As New List(Of InstructionParameterTO)

                For Each myIndexTO As InstructionParameterTO In pIndexedList
                    'Linq the ParameterID and when found assign the ParameterValue
                    myID = myIndexTO.Parameter
                    'set the index number
                    myIndexNumber = myIndexTO.ParameterIndex

                    query = (From a In pParameterList Where a.ParameterID = myID Select a).ToList

                    'TR 19/05/2010 'set the value of the position not the label.

                    If myIndexNumber = 46 And query.Count > 0 Then 'TR 02/03/2011 -the result index change from 3 to 46
                        'myResultCounts = CType(query.First().ParameterValues, Integer)
                        myResultCounts = 68 'AG  05/05/2011 
                        setFirstParam = True
                    ElseIf setFirstParam Then
                        myFirstResultParam = myID
                        setFirstParam = False
                    End If
                    'TR 19/05/2010 END

                    'If found ... assign value
                    'AG 11/05/2010 - The 2 first parameters hasnt ParameterID (A400;instructiontype;...)
                    If iteration <= 1 Then
                        myIndexTO.Parameter = pParameterList(iteration).ParameterID
                        myIndexTO.ParameterValue = pParameterList(iteration).ParameterID
                    ElseIf query.Count > 0 Then
                        myIndexTO.ParameterValue = query.First().ParameterValues
                        'remove from parameters
                        pParameterList.Remove(query.First())
                    End If
                    myIndexTO.ParameterIndex = iteration + 1 ' TR 25/05/2010 -Set the secuential index value.
                    iteration = iteration + 1
                Next

                'validate the result numbers
                If myResultCounts - 1 > 0 Then
                    myTemIndexedList = GetInstructionParameter(pParameterValue)

                    If myTemIndexedList.Count > 0 Then '(1) AG 28/10/2010
                        Dim qResultStructure As New List(Of InstructionParameterTO)
                        'for each result create a new result structure and fill it with their values.
                        For res As Integer = 1 To myResultCounts - 1
                            'Remove the first three parameter and get the result structures.
                            'TR 10/10/2013 -increase the Parameterindex > value from 46 to 47 because new field is added on position 47 ST
                            qResultStructure = (From a In myTemIndexedList Where a.ParameterIndex > 47 Select a).ToList()
                            Dim myID2 As String = ""
                            'Declare a new intruction parameter to assigned the new results values.
                            Dim myIntructionParameterTO As New InstructionParameterTO()
                            'go throught each intstruction on the result structure and fill it.
                            For Each myIntrucRow As InstructionParameterTO In qResultStructure
                                'fill the intruccion with the required parameters
                                myIntructionParameterTO.InstructionType = myIntrucRow.InstructionType
                                myIntructionParameterTO.Parameter = myIntrucRow.Parameter
                                myIntructionParameterTO.ParameterIndex = iteration + 1 ' TR 25/05/2010 -Set the secuential index value.

                                myID2 = myIntrucRow.Parameter
                                'Get the parameter value onthe ParameterList Structure.
                                query = (From a In pParameterList Where a.ParameterID.Trim = myID2.Trim _
                                         Select a).ToList

                                If query.Count > 0 Then
                                    myIntructionParameterTO.ParameterValue = query.First().ParameterValues
                                    ' after adding value remove for the parameter list.
                                    pParameterList.Remove(query.First())
                                End If
                                'Add result to my intructions list.
                                pIndexedList.Add(myIntructionParameterTO)

                                myIntructionParameterTO = New InstructionParameterTO()
                                iteration += 1
                            Next
                        Next
                        qResultStructure = Nothing 'AG 02/08/2012 - free memory
                        'myGlobalDataTO.SetDatos = pIndexedList

                    Else '(1)
                        myGlobalDataTO.HasError = True
                        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.WRONG_DATATYPE.ToString
                    End If '(1)

                    'myGlobalDataTO.SetDatos = pIndexedList 'AG 28/10/2010
                End If
                query = Nothing 'AG 02/08/2012 - free memory
                myTemIndexedList = Nothing 'AG 02/08/2012 - free memory

                myGlobalDataTO.SetDatos = pIndexedList
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateANSPHRInstruction", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate the ANSBLD, ANSISE instruction
        ''' </summary>
        ''' <param name="pParameterValue"></param>
        ''' <param name="pParameterList"></param>
        ''' <param name="pIndexedList"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  TR 21/05/2010
        ''' AG 01/03/2011 - Rename as GenerateGenericalDinamicInstruction
        ''' </remarks>
        Private Function GenerateGenericalDinamicInstruction(ByVal pParameterValue As String, ByVal pParameterList As List(Of ParametersTO), _
                                                         ByRef pIndexedList As List(Of InstructionParameterTO)) As GlobalDataTO
            'Old method name was GenerateBaseLineTypeInstruction
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myID As String = ""
                Dim myIndexNumber As Integer = -1
                Dim iteration As Integer = 0
                Dim myResultCounts As Integer = 0
                Dim query As New List(Of ParametersTO)
                Dim myTemIndexedList As New List(Of InstructionParameterTO)

                'Get the instruction parameters to load values.
                pIndexedList = GetInstructionParameter(pParameterValue)

                For Each myIndexTO As InstructionParameterTO In pIndexedList
                    'Linq the ParameterID and when found assign the ParameterValue
                    myID = myIndexTO.Parameter
                    'TR 26/05/2010 ' get the index value to work with 
                    myIndexNumber = myIndexTO.ParameterIndex
                    query = (From a In pParameterList Where a.ParameterID = myID Select a).ToList
                    'TR 26/05/2010 -change to work with the index number and not use the label.
                    'To get the label to know how mani diodes are as result use the index number 4.
                    If myIndexNumber = 4 Then
                        myResultCounts = (From a In pParameterList Where a.ParameterID = myID Select a).ToList().Count
                    End If

                    'If found ... assign value
                    If iteration <= 1 Then
                        myIndexTO.Parameter = pParameterList(iteration).ParameterID
                        myIndexTO.ParameterValue = pParameterList(iteration).ParameterID
                    ElseIf query.Count > 0 Then
                        myIndexTO.ParameterValue = query.First().ParameterValues
                        'remove from parameters
                        pParameterList.Remove(query.First())
                    End If

                    iteration = iteration + 1
                Next

                'validate the result numbers
                If myResultCounts > 0 Then
                    myTemIndexedList = GetInstructionParameter(pParameterValue)
                    Dim qResultStructure As New List(Of InstructionParameterTO)
                    'for each result create a new result structure and fill it with their values.
                    For res As Integer = 1 To myResultCounts - 1 Step 1
                        'Remove the first three parameter and get the result structures.
                        qResultStructure = (From a In myTemIndexedList Where a.ParameterIndex > 3 Select a).ToList()
                        Dim myID2 As String = ""
                        'Declare a new intruction parameter to assigned the new results values.
                        Dim myIntructionParameterTO As New InstructionParameterTO()
                        'go throught each intstruction on the result structure and fill it.
                        For Each myIntrucRow As InstructionParameterTO In qResultStructure
                            'fill the intruccion with the required parameters
                            myIntructionParameterTO.InstructionType = myIntrucRow.InstructionType
                            myIntructionParameterTO.Parameter = myIntrucRow.Parameter
                            myIntructionParameterTO.ParameterIndex = myIntrucRow.ParameterIndex

                            myID2 = myIntrucRow.Parameter
                            'Get the parameter value onthe ParameterList Structure.
                            query = (From a In pParameterList Where a.ParameterID.Trim = myID2.Trim _
                                     Select a).ToList

                            If query.Count > 0 Then
                                myIntructionParameterTO.ParameterValue = query.First().ParameterValues
                                myIntructionParameterTO.ParameterIndex = iteration + 1
                                ' after adding value remove for the parameter list.
                                pParameterList.Remove(query.First())
                            End If
                            'Add result to my intructions list.
                            pIndexedList.Add(myIntructionParameterTO)

                            myIntructionParameterTO = New InstructionParameterTO()
                            iteration += 1
                        Next
                    Next
                    qResultStructure = Nothing 'AG 02/08/2012 - free memory
                End If

                query = Nothing 'AG 02/08/2012 - free memory
                myTemIndexedList = Nothing 'AG 02/08/2012 - free memory

                myGlobalDataTO.SetDatos = pIndexedList

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateGenericalDinamicInstruction", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate the ANSBR1, ANSBR2, ANSBM1 instructionS
        ''' </summary>
        ''' <param name="pParameterValue">Parameters Value</param>
        ''' <param name="pParameterList">Parameters List</param>
        ''' <param name="pIndexedList">Index List</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 16/03/2011
        ''' Modified by: AG 21/10/2011
        ''' </remarks>
        Private Function GenerateANSARMsInstruction(ByVal pParameterValue As String, ByVal pParameterList As List(Of ParametersTO), _
                                                                ByRef pIndexedList As List(Of InstructionParameterTO)) As GlobalDataTO
            'Old method name was GenerateBaseLineTypeInstruction
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction parameters to load values.
                pIndexedList = GetInstructionParameter(pParameterValue)

                Dim query As New List(Of ParametersTO)
                For Each myIndexTO As InstructionParameterTO In pIndexedList

                    Select Case myIndexTO.ParameterIndex
                        Case 1
                            'AG 21/10/2011
                            'myIndexTO.ParameterValue = "AX400"
                            myIndexTO.Parameter = pParameterList(myIndexTO.ParameterIndex - 1).ParameterID
                            myIndexTO.ParameterValue = pParameterList(myIndexTO.ParameterIndex - 1).ParameterID
                            Exit Select
                        Case 2
                            myIndexTO.ParameterValue = myIndexTO.InstructionType
                            myIndexTO.Parameter = myIndexTO.InstructionType
                            Exit Select

                        Case Else
                            query = (From a In pParameterList Where a.ParameterID = myIndexTO.Parameter Select a).ToList()
                            If query.Count > 0 Then
                                myIndexTO.ParameterValue = query.First().ParameterValues
                            End If
                            Exit Select
                    End Select

                Next
                query = Nothing 'AG 02/08/2012 - free memory

                myGlobalDataTO.SetDatos = pIndexedList

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateANSArmsInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO

        End Function

        ''' <summary>
        ''' Generate the ANSADJ instruction
        ''' </summary>
        ''' <param name="pParameterList">Parameters List</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: XBC 03/05/2011
        ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
        ''' </remarks>
        Private Function GenerateANSADJInstruction(ByVal pParameterList As List(Of ParametersTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myParameterValue As String = ""
                Dim myParameterList As New List(Of ParametersTO)

                ' Delete comments
                myGlobal = CleanANSADJComments(pParameterList)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParameterList = CType(myGlobal.SetDatos, List(Of ParametersTO))

                    'Get the instruction type (Always the 2on parameter in list)                
                    myParameterValue = myParameterList(1).ParameterID

                    Dim myIndexedList As New List(Of InstructionParameterTO)
                    myIndexedList = GetInstructionParameter(myParameterValue)

                    If myIndexedList.Count > 0 Then '(1) AG 28/10/2010
                        Dim query As New List(Of ParametersTO)
                        Dim iteration As Integer = 0    'AG 11/05/2010
                        For Each myIndexTO As InstructionParameterTO In myIndexedList
                            'Linq the ParameterID and when found assign the ParameterValue
                            Dim myID As String = myIndexTO.Parameter
                            query = (From a In myParameterList Where a.ParameterID.Trim = myID.Trim Select a).ToList
                            'query = (From a In myParameterList Where a.ParameterID.ToUpper.Trim = myID.ToUpper.Trim Select a).ToList

                            'If found ... assign value
                            'AG 11/05/2010 - The 2 first parameters hasnt ParameterID (A400;instructiontype;...)
                            If iteration <= 1 Then
                                myIndexTO.Parameter = myParameterList(iteration).ParameterID
                                myIndexTO.ParameterValue = myParameterList(iteration).ParameterID
                                'END AG 11/05/2010 

                            ElseIf query.Count > 0 Then
                                myIndexTO.ParameterValue = query(0).ParameterValues
                            End If
                            iteration = iteration + 1

                        Next
                        query = Nothing 'AG 02/08/2012 - free memory

                        myGlobal.SetDatos = myIndexedList

                    Else '(1)
                        myGlobal.HasError = True
                        myGlobal.ErrorCode = GlobalEnumerates.Messages.WRONG_DATATYPE.ToString
                    End If '(1)

                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateANSADJInstruction", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' Generate the Code bar Reader type Answer Instruction.
        ''' </summary>
        ''' <param name="pParameterValue"></param>
        ''' <param name="pParameterList"></param>
        ''' <param name="pIndexedList"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: AG 22/06/2011 - based on GenerateANSPHRInstruction
        ''' </remarks>
        Public Function GenerateANSCBRInstruction(ByVal pParameterValue As String, ByVal pParameterList As List(Of ParametersTO), _
                                                 ByVal pIndexedList As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myID As String = ""
                Dim myIndexNumber As Integer = -1
                Dim iteration As Integer = 0
                Dim myResultCounts As Integer = 0
                Dim setFirstParam As Boolean = False
                Dim myFirstResultParam As String = ""
                Dim query As New List(Of ParametersTO)
                Dim myTemIndexedList As New List(Of InstructionParameterTO)

                For Each myIndexTO As InstructionParameterTO In pIndexedList
                    'Linq the ParameterID and when found assign the ParameterValue
                    myID = myIndexTO.Parameter
                    myIndexNumber = myIndexTO.ParameterIndex 'set the index number
                    query = (From a In pParameterList Where a.ParameterID = myID Select a).ToList

                    If myIndexNumber = 5 And query.Count > 0 Then
                        myResultCounts = CType(query.First().ParameterValues, Integer)
                        setFirstParam = True
                    ElseIf setFirstParam Then
                        myFirstResultParam = myID
                        setFirstParam = False
                    End If

                    'If found ... assign value
                    'AG 11/05/2010 - The 2 first parameters hasnt ParameterID (A400;instructiontype;...)
                    If iteration <= 1 Then
                        myIndexTO.Parameter = pParameterList(iteration).ParameterID
                        myIndexTO.ParameterValue = pParameterList(iteration).ParameterID
                    ElseIf query.Count > 0 Then
                        myIndexTO.ParameterValue = query.First().ParameterValues
                        pParameterList.Remove(query.First()) 'remove from parameters
                    End If
                    myIndexTO.ParameterIndex = iteration + 1 ' TR 25/05/2010 -Set the secuential index value.
                    iteration = iteration + 1
                Next

                'validate the result numbers
                If myResultCounts - 1 > 0 Then
                    myTemIndexedList = GetInstructionParameter(pParameterValue)

                    If myTemIndexedList.Count > 0 Then '(1) AG 28/10/2010
                        Dim qResultStructure As New List(Of InstructionParameterTO)
                        'for each result create a new result structure and fill it with their values.
                        For res As Integer = 1 To myResultCounts - 1
                            'Remove the first three parameter and get the result structures.
                            qResultStructure = (From a In myTemIndexedList Where a.ParameterIndex > 5 Select a).ToList()
                            Dim myID2 As String = ""
                            'Declare a new intruction parameter to assigned the new results values.
                            Dim myIntructionParameterTO As New InstructionParameterTO()
                            'go throught each intstruction on the result structure and fill it.
                            For Each myIntrucRow As InstructionParameterTO In qResultStructure
                                'fill the intruccion with the required parameters
                                myIntructionParameterTO.InstructionType = myIntrucRow.InstructionType
                                myIntructionParameterTO.Parameter = myIntrucRow.Parameter
                                myIntructionParameterTO.ParameterIndex = iteration + 1 ' TR 25/05/2010 -Set the secuential index value.

                                myID2 = myIntrucRow.Parameter
                                'Get the parameter value onthe ParameterList Structure.
                                query = (From a In pParameterList Where a.ParameterID.Trim = myID2.Trim _
                                         Select a).ToList

                                If query.Count > 0 Then
                                    myIntructionParameterTO.ParameterValue = query.First().ParameterValues
                                    ' after adding value remove for the parameter list.
                                    pParameterList.Remove(query.First())
                                End If
                                'Add result to my intructions list.
                                pIndexedList.Add(myIntructionParameterTO)

                                myIntructionParameterTO = New InstructionParameterTO()
                                iteration += 1
                            Next
                        Next
                        qResultStructure = Nothing 'AG 02/08/2012 - free memory
                    Else '(1)
                        myGlobalDataTO.HasError = True
                        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.WRONG_DATATYPE.ToString
                    End If '(1)

                End If
                query = Nothing 'AG 02/08/2012 - free memory
                myTemIndexedList = Nothing 'AG 02/08/2012 - free memory

                myGlobalDataTO.SetDatos = pIndexedList
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateANSCBRInstruction", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate a READCYCLES parameter with corresponding values. PENDING TO SPEC!!!!!!!!!!!!!
        ''' </summary>
        ''' <param name="pQueryMode "></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' SGM 27/07/2011
        ''' </remarks>
        Public Function PDT_GenerateREADCYCLESInstruction(ByVal pQueryMode As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim myREADCYCLESInstruction As New List(Of InstructionParameterTO)
                myREADCYCLESInstruction = GetInstructionParameter("READCYCLES")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myREADCYCLESInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "READCYCLES"
                            Exit Select
                        Case 3 'Well to perform the adjustment.
                            myInstructionTO.ParameterValue = pQueryMode
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = myREADCYCLESInstruction
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateREADCYCLESInstruction", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate the WRITECYCLES instruction PENDING TO SPEC!!!!!!!!!!!!!
        ''' </summary>
        ''' <param name="pParamsToSave">Parameters List</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: SGM 27/07/2011
        ''' </remarks>
        Public Function PDT_GenerateWRITECYCLESInstruction(ByVal pParamsToSave As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myParametersToSave As String
            Try
                'Get the instruction on the Instruction list.
                Dim myWRITECYCLESInstruction As New List(Of InstructionParameterTO)
                myWRITECYCLESInstruction = GetInstructionParameter("WRITECYCLES")

                ' Delete comments
                myGlobalDataTO = CleanLOADADJComments(pParamsToSave)
                If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then
                    myParametersToSave = CType(myGlobalDataTO.SetDatos, String)
                    myParametersToSave = myParametersToSave.Trim
                    ' Eliminate last character ";"
                    myParametersToSave = myParametersToSave.Substring(0, myParametersToSave.Length - 1)

                    'Fill all the instruccion.
                    For Each myInstructionTO As InstructionParameterTO In myWRITECYCLESInstruction
                        Select Case myInstructionTO.ParameterIndex
                            Case 1 'Analyzer Model and Number.
                                myInstructionTO.ParameterValue = "A400"
                                Exit Select
                            Case 2 'Instruction Code.
                                myInstructionTO.ParameterValue = "WRITECYCLES"
                                Exit Select
                            Case 3 'Well to perform the adjustment.
                                myInstructionTO.ParameterValue = myParametersToSave
                                Exit Select

                            Case Else
                                Exit Select

                        End Select
                    Next
                    myGlobalDataTO.SetDatos = myWRITECYCLESInstruction

                Else
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.WRONG_DATATYPE.ToString
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateWRITECYCLESInstruction", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get for a specific instruction all the required parameters
        ''' </summary>
        ''' <param name="pInstructionType">Intruction Type</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: TR 15/04/2010
        ''' </remarks>
        Private Function GetInstructionParameter(ByVal pInstructionType As String) As List(Of InstructionParameterTO)
            Dim InstructionParameterList As New List(Of InstructionParameterTO)
            Try
                If pInstructionType <> "" Then
                    'filter the parameter list by the intruction type.
                    InstructionParameterList = (From a In InstructionList _
                                               Where a.InstructionType = pInstructionType _
                                               Select a).ToList()
                End If

                If InstructionParameterList.Count = 0 Then
                    'ERROR
                End If

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GetInstructionParameter", EventLogEntryType.Error, False)
            End Try
            Return InstructionParameterList
        End Function

        ''' <summary>
        ''' Table with the bottle codes
        ''' </summary>
        ''' <param name="pBottleID">Tube/Bottle Identifier</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  TR 16/04/2010</remarks>
        Private Function GetBottleCode(ByVal pBottleID As String) As String
            Dim myResult As String = ""
            Try
                Select Case pBottleID
                    Case ""
                        myResult = "0"
                        Exit Select
                    Case "PED"
                        myResult = "1"
                        Exit Select
                    Case "T13"
                        myResult = "2"
                        Exit Select
                    Case "T15"
                        myResult = "3"
                        Exit Select
                    Case "BOTTLE1"
                        myResult = "4"
                        Exit Select
                    Case "BOTTLE2"
                        myResult = "5"
                        Exit Select
                    Case "BOTTLE3"
                        myResult = "6"
                        Exit Select
                    Case "BOTTLE4"
                        myResult = "7"
                        Exit Select
                    Case "BOTTLE5"
                        myResult = "8"
                        Exit Select

                    Case Else
                        Exit Select
                End Select

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GetBottleCode", EventLogEntryType.Error, False)
            End Try
            Return myResult
        End Function

        ''' <summary>
        ''' function to delete posible comments existing in ANSADJ instruction received from Instrument
        ''' </summary>
        ''' <param name="pParameterList"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: XBC 03/05/2011</remarks>
        Private Function CleanANSADJComments(ByVal pParameterList As List(Of ParametersTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim myParameterList As New List(Of ParametersTO)
            Dim strWithoutComments As String
            Try
                For i As Integer = 0 To pParameterList.Count - 1
                    strWithoutComments = pParameterList(i).ParameterID
                    Dim myAdjustmentDescription As String = ""

                    If strWithoutComments.Contains("}") Then
                        myAdjustmentDescription = strWithoutComments.Substring(strWithoutComments.IndexOf("{"))
                        myAdjustmentDescription = myAdjustmentDescription.Substring(0, myAdjustmentDescription.LastIndexOf("}") + 1)
                        strWithoutComments = strWithoutComments.Replace(myAdjustmentDescription, "").Trim
                        pParameterList(i).ParameterID = strWithoutComments
                    End If
                    myParameterList.Add(pParameterList(i))
                Next
                myGlobal.SetDatos = myParameterList
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.CleanANSADJComments", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' function to delete posible comments existing in LOADADJ instruction to send to the Instrument
        ''' </summary>
        ''' <param name="pParameter"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: XBC 06/05/2011</remarks>
        Private Function CleanLOADADJComments(ByVal pParameter As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim strWithoutComments As String
            Try
                strWithoutComments = pParameter
                Dim myAdjustmentDescription As String = ""

                If strWithoutComments.Contains("}") Then
                    myAdjustmentDescription = strWithoutComments.Substring(strWithoutComments.IndexOf("{"))
                    myAdjustmentDescription = myAdjustmentDescription.Substring(0, myAdjustmentDescription.IndexOf("}") + 1)
                    strWithoutComments = strWithoutComments.Replace(myAdjustmentDescription, "").Trim
                End If

                If strWithoutComments.Contains("{") Then
                    ' recursive call until eliminate all comments
                    myGlobal = CleanLOADADJComments(strWithoutComments)
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        strWithoutComments = CType(myGlobal.SetDatos, String)
                    End If
                End If

                ' delete invalid chars to send to the Instrument
                strWithoutComments = strWithoutComments.Replace(Chr(13), "")
                strWithoutComments = strWithoutComments.Replace(Chr(10), "")

                myGlobal.SetDatos = strWithoutComments
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.CleanLOADADJComments", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' Generate the ANSTIN instruction
        ''' </summary>
        ''' <param name="pParameterValue"></param>
        ''' <param name="pParameterList"></param>
        ''' <param name="pIndexedList"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  AG 31/07/2012 based on GenerateGenericalDinamicInstruction
        ''' </remarks>
        Private Function GenerateANSTINInstruction(ByVal pParameterValue As String, ByVal pParameterList As List(Of ParametersTO), _
                                                         ByRef pIndexedList As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myID As String = ""
                Dim myIndexNumber As Integer = -1
                Dim iteration As Integer = 0
                Dim myResultCounts As Integer = 0
                Dim query As New List(Of ParametersTO)
                Dim myTemIndexedList As New List(Of InstructionParameterTO)

                'Get the instruction parameters to load values.
                pIndexedList = GetInstructionParameter(pParameterValue)

                For Each myIndexTO As InstructionParameterTO In pIndexedList
                    'Linq the ParameterID and when found assign the ParameterValue
                    myID = myIndexTO.Parameter
                    'TR 26/05/2010 ' get the index value to work with 
                    myIndexNumber = myIndexTO.ParameterIndex
                    query = (From a In pParameterList Where a.ParameterID = myID Select a).ToList
                    If myIndexNumber = 3 AndAlso query.Count > 0 Then
                        myResultCounts = CInt(query.First().ParameterValues)
                    End If

                    'If found ... assign value
                    If iteration <= 1 Then
                        myIndexTO.Parameter = pParameterList(iteration).ParameterID
                        myIndexTO.ParameterValue = pParameterList(iteration).ParameterID
                    ElseIf query.Count > 0 Then
                        myIndexTO.ParameterValue = query.First().ParameterValues
                        'remove from parameters
                        pParameterList.Remove(query.First())
                    End If

                    iteration = iteration + 1
                Next

                'validate the result numbers
                If myResultCounts > 0 Then
                    myTemIndexedList = GetInstructionParameter(pParameterValue)
                    Dim qResultStructure As New List(Of InstructionParameterTO)
                    'for each result create a new result structure and fill it with their values.
                    For res As Integer = 1 To myResultCounts - 1 Step 1
                        'Remove the first three parameter and get the result structures.
                        qResultStructure = (From a In myTemIndexedList Where a.ParameterIndex > 3 Select a).ToList()
                        Dim myID2 As String = ""
                        'Declare a new intruction parameter to assigned the new results values.
                        Dim myIntructionParameterTO As New InstructionParameterTO()
                        'go throught each intstruction on the result structure and fill it.
                        For Each myIntrucRow As InstructionParameterTO In qResultStructure
                            'fill the intruccion with the required parameters
                            myIntructionParameterTO.InstructionType = myIntrucRow.InstructionType
                            myIntructionParameterTO.Parameter = myIntrucRow.Parameter
                            myIntructionParameterTO.ParameterIndex = myIntrucRow.ParameterIndex

                            myID2 = myIntrucRow.Parameter
                            'Get the parameter value onthe ParameterList Structure.
                            query = (From a In pParameterList Where a.ParameterID.Trim = myID2.Trim _
                                     Select a).ToList

                            If query.Count > 0 Then
                                myIntructionParameterTO.ParameterValue = query.First().ParameterValues
                                myIntructionParameterTO.ParameterIndex = iteration + 1
                                ' after adding value remove for the parameter list.
                                pParameterList.Remove(query.First())
                            End If
                            'Add result to my intructions list.
                            pIndexedList.Add(myIntructionParameterTO)

                            myIntructionParameterTO = New InstructionParameterTO()
                            iteration += 1
                        Next
                    Next
                    qResultStructure = Nothing 'AG 02/08/2012 - free memory
                End If
                query = Nothing 'AG 02/08/2012 - free memory
                myTemIndexedList = Nothing 'AG 02/08/2012 - free memory

                myGlobalDataTO.SetDatos = pIndexedList

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateANSTINInstruction", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate a ALIGHT parameter with corresponding values.
        ''' </summary>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pPollRdAction">Recover action mode</param>
        ''' <param name="pRecoveryResultsInPause"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: AG 31/07/2012
        ''' Modify by AG 27/11/2013 - Task #1397 in normal scenario ask for the last well baseline + 1 but in pause mode ask for the last
        ''' </remarks>
        Public Function GeneratePOLLRDInstruction(ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pPollRdAction As Integer, ByVal pRecoveryResultsInPause As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the instruction on the Instruction list.
                Dim myPOLLRDInstruction As New List(Of InstructionParameterTO)
                myPOLLRDInstruction = GetInstructionParameter("POLLRD")
                'Fill all the instruccion.
                For Each myInstructionTO As InstructionParameterTO In myPOLLRDInstruction
                    Select Case myInstructionTO.ParameterIndex
                        Case 1 'Analyzer Model and Number.
                            myInstructionTO.ParameterValue = "A400"
                            Exit Select
                        Case 2 'Instruction Code.
                            myInstructionTO.ParameterValue = "POLLRD"
                            Exit Select
                        Case 3 'Recover action mode
                            myInstructionTO.ParameterValue = pPollRdAction.ToString()
                            Exit Select
                        Case 4 'starting well (only when recovery biochemical)
                            myInstructionTO.ParameterValue = "1" 'Default value
                            If pPollRdAction = GlobalEnumerates.Ax00PollRDAction.Biochemical Then
                                'Search the last well rejection received.
                                Dim wellBaseLineDelg As New WSBLinesByWellDelegate
                                myGlobalDataTO = wellBaseLineDelg.GetLastWellBaseLineReceived(Nothing, pAnalyzerID, pWorkSessionID)
                                If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then


                                    Dim wellFound As Integer = CType(myGlobalDataTO.SetDatos, Integer) + 1 'Ask for next well
                                    'AG 27/11/2013 - Task #1397 - One less in pause mode
                                    If pRecoveryResultsInPause Then
                                        wellFound -= 1
                                    End If
                                    'AG 27/11/2013

                                    Dim maxWells As Integer = 120
                                    Dim paramsDelg As New SwParametersDelegate
                                    myGlobalDataTO = paramsDelg.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.MAX_REACTROTOR_WELLS.ToString, Nothing)
                                    If Not myGlobalDataTO.HasError Then
                                        Dim paramsDS As New ParametersDS
                                        paramsDS = CType(myGlobalDataTO.SetDatos, ParametersDS)
                                        If paramsDS.tfmwSwParameters.Rows.Count > 0 Then
                                            If Not paramsDS.tfmwSwParameters(0).IsValueNumericNull Then maxWells = CInt(paramsDS.tfmwSwParameters(0).ValueNumeric)
                                        End If
                                    End If

                                    Dim reactRotorDlg As New ReactionsRotorDelegate
                                    wellFound = reactRotorDlg.GetRealWellNumber(wellFound, maxWells)
                                    If wellFound > 0 Then
                                        myInstructionTO.ParameterValue = wellFound.ToString
                                    End If
                                End If
                            End If
                            Exit Select

                        Case Else
                            Exit Select

                    End Select
                Next
                myGlobalDataTO.SetDatos = myPOLLRDInstruction

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GeneratePOLLRDInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Generate the ANSFBLD (dynamic base line results) type Instruction.
        ''' </summary>
        ''' <param name="pParameterValue"></param>
        ''' <param name="pParameterList"></param>
        ''' <param name="pIndexedList"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  AG 30/10/2014 BA-2062 (based on GenerateANSPHRInstruction)
        ''' </remarks>
        Private Function GenerateANSFBLDInstructionReception(ByVal pParameterValue As String, ByVal pParameterList As List(Of ParametersTO), _
                                                 ByVal pIndexedList As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myID As String = ""
                Dim myIndexNumber As Integer = -1
                Dim iteration As Integer = 0 'AG 11/05/2010
                Dim numberOfWells As Integer = 0
                Dim insertIndex As Integer = 0
                Dim query As New List(Of ParametersTO)
                Dim myTemIndexedList As New List(Of InstructionParameterTO)

                'A400;ANSFBLD;P:<diode position>;BLW:<well read>;MP:<Mainlight>;RP:<Reflight>; repeat these tags 120 times ...;MD:<mainDark>;RD:<refdark>;TI:<intTime>;DAC:<value>;

                For Each myIndexTO As InstructionParameterTO In pIndexedList
                    'Linq the ParameterID and when found assign the ParameterValue
                    myID = myIndexTO.Parameter 'for the 2 first items it is not informed

                    'set the index number
                    myIndexNumber = myIndexTO.ParameterIndex
                    query = (From a In pParameterList Where a.ParameterID = myID Select a).ToList

                    'Index 4 belongs BLW tag, query count inform us the number of wells read in instruction
                    If myIndexNumber = 4 And query.Count > 0 Then
                        numberOfWells = query.Count
                    End If

                    'If found ... assign value
                    'AG 11/05/2010 - The 2 first parameters hasnt ParameterID (A400;instructiontype;...)
                    If iteration <= 1 Then
                        myIndexTO.Parameter = pParameterList(iteration).ParameterID
                        myIndexTO.ParameterValue = pParameterList(iteration).ParameterID
                    ElseIf query.Count > 0 Then
                        myIndexTO.ParameterValue = query.First().ParameterValues
                        'remove from parameters the 1st values of: BLW, MP, RP
                        pParameterList.Remove(query.First())
                    End If
                    myIndexTO.ParameterIndex = iteration + 1 ' TR 25/05/2010 -Set the secuential index value.
                    iteration = iteration + 1

                Next


                'Search and INSERT the rest of wells read (insert fields BLW, MP, RP)
                If numberOfWells - 1 > 0 Then
                    insertIndex = 6
                    myTemIndexedList = GetInstructionParameter(pParameterValue)

                    If myTemIndexedList.Count > 0 Then '(1) AG 28/10/2010
                        Dim qResultStructure As New List(Of InstructionParameterTO)

                        For res As Integer = 1 To numberOfWells - 1
                            'The fields in loop are BLW, MP and BP with parameter index 4,5,6
                            qResultStructure = (From a In myTemIndexedList Where a.ParameterIndex >= 4 AndAlso a.ParameterIndex <= 6 Select a).ToList()
                            Dim myID2 As String = ""

                            Dim myIntructionParameterTO As New InstructionParameterTO()
                            'go throught each intstruction on the result structure and fill it.
                            For Each myIntrucRow As InstructionParameterTO In qResultStructure
                                'fill the intruccion with the required parameters
                                myIntructionParameterTO.InstructionType = myIntrucRow.InstructionType
                                myIntructionParameterTO.Parameter = myIntrucRow.Parameter
                                myIntructionParameterTO.ParameterIndex = iteration + 1 ' TR 25/05/2010 -Set the secuential index value.

                                myID2 = myIntrucRow.Parameter
                                'Get the parameter value onthe ParameterList Structure.
                                query = (From a In pParameterList Where a.ParameterID.Trim = myID2.Trim _
                                         Select a).ToList

                                If query.Count > 0 Then
                                    myIntructionParameterTO.ParameterValue = query.First().ParameterValues
                                    ' after adding value remove for the parameter list.
                                    pParameterList.Remove(query.First())

                                    'Insert values in my intructions list.
                                    pIndexedList.Insert(insertIndex, myIntructionParameterTO)
                                    insertIndex += 1
                                    myIntructionParameterTO = New InstructionParameterTO()
                                End If
                                iteration += 1
                            Next

                        Next
                        qResultStructure = Nothing 'AG 02/08/2012 - free memory

                        'Finally renumerate properly the ParameterIndex value
                        iteration = 1
                        For Each itemList As InstructionParameterTO In pIndexedList
                            itemList.ParameterIndex = iteration
                            iteration += 1
                        Next

                    Else '(1)
                        myGlobalDataTO.HasError = True
                        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.WRONG_DATATYPE.ToString
                    End If '(1)

                    'myGlobalDataTO.SetDatos = pIndexedList 'AG 28/10/2010
                End If
                query = Nothing 'AG 02/08/2012 - free memory
                myTemIndexedList = Nothing 'AG 02/08/2012 - free memory

                myGlobalDataTO.SetDatos = pIndexedList
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateANSFBLDInstructionReception", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        '''' <summary>
        '''' Received ANSUTIL
        '''' </summary>
        '''' <param name="pParameterValue"></param>
        '''' <param name="pParameterList"></param>
        '''' <param name="pIndexedList"></param>
        '''' <returns></returns>
        '''' <remarks>Created by SGM 04/10/2012</remarks>
        'Private Function GenerateANSUTILInstruction(ByVal pParameterList As List(Of ParametersTO)) As GlobalDataTO
        '    'Old method name was GenerateBaseLineTypeInstruction
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Try
        '        'Get the instruction parameters to load values.
        '        pIndexedList = GetInstructionParameter(pParameterValue)

        '        Dim query As New List(Of ParametersTO)
        '        For Each myIndexTO As InstructionParameterTO In pIndexedList

        '            Select Case myIndexTO.ParameterIndex
        '                Case 1
        '                    'AG 21/10/2011
        '                    'myIndexTO.ParameterValue = "AX400"
        '                    myIndexTO.Parameter = pParameterList(myIndexTO.ParameterIndex - 1).ParameterID
        '                    myIndexTO.ParameterValue = pParameterList(myIndexTO.ParameterIndex - 1).ParameterID
        '                    Exit Select
        '                Case 2
        '                    myIndexTO.ParameterValue = myIndexTO.InstructionType
        '                    myIndexTO.Parameter = myIndexTO.InstructionType
        '                    Exit Select

        '                Case Else
        '                    query = (From a In pParameterList Where a.ParameterID = myIndexTO.Parameter Select a).ToList()
        '                    If query.Count > 0 Then
        '                        myIndexTO.ParameterValue = query.First().ParameterValues
        '                    End If
        '                    Exit Select
        '            End Select

        '        Next
        '        query = Nothing 'AG 02/08/2012 - free memory

        '        myGlobalDataTO.SetDatos = pIndexedList

        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "Instructions.GenerateANSArmsInstruction", EventLogEntryType.Error, False)
        '    End Try

        '    Return myGlobalDataTO

        'End Function
#End Region


#Region "OLD methods ... for old instrucion definition"

#End Region

        Private Function GetValueExecutionID(pWashingDataDS As AnalyzerManagerDS) As String
            If (pWashingDataDS.nextPreparation.Any() AndAlso Not pWashingDataDS.nextPreparation(0).IsExecutionIDNull) Then
                Return pWashingDataDS.nextPreparation(0).ExecutionID.ToString()
            End If
            Return "0"
        End Function

    End Class

End Namespace
