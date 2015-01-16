Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports System.Data
Imports System.Data.SqlClient
Imports System.Text

Namespace Biosystems.Ax00.CommunicationsSwFw

    Partial Public Class AnalyzerManager

#Region "Level1 Private Methods"


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 02/03/2011</remarks>
        Private Function ProcessBaseLineReceived_SRV(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            ' XBC 25/10/2011 - dbConnection is not used
            'Dim dbConnection As New SqlClient.SqlConnection
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                'InitializeTimerControl(WAITING_TIME_OFF) 'AG 13/02/2012 - the waiting timer is disabled on every reception process ('AG 18/07/2011)

                Dim query As New List(Of InstructionParameterTO)
                Dim myOffset As Integer = -1
                Dim myUtility As New Utilities()
                Dim myInstructionType As String = ""
                Dim myWell As Integer = 0
                Dim myTotalResults As Integer = 0
                Dim baseLineWithAdjust As Boolean = False
                '' Initialize PhotometryDataTO
                'AppLayer.PhotometryData = New PhotometryDataTO

                ' XBC 30/10/2012 - is need not clear queue because is posible there are any pending operation (INFO;Q:2) to execute due Alarms (E:99)
                '' Set Waiting Timer Current Instruction OFF
                'If myApplicationName.ToUpper.Contains("SERVICE") Then ClearQueueToSend()
                ' XBC 30/10/2012

                'AG 03/01/2011 - Get the instruction type value. to set the offset.
                myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 2)

                If Not myGlobalDataTO.HasError Then
                    myInstructionType = DirectCast(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue
                    If myInstructionType = AppLayerInstrucionReception.ANSBLD.ToString Then baseLineWithAdjust = True
                Else
                    Exit Try
                End If
                'AG 03/01/2011

                'Get the well used from pInstructionReceived and initialize myWell variable
                query = (From a In pInstructionReceived Where a.ParameterIndex = 3 Select a).ToList()
                If query.Count > 0 Then
                    myWell = CType(query.First().ParameterValue, Integer)
                End If

                'filter to get the results only from the instruction recived.
                query = (From a In pInstructionReceived Where a.ParameterIndex > 3 Select a).ToList()

                'Set the offset value depending on the instruction type
                myOffset = 7

                If myOffset > 0 And query.Count > 0 Then

                    myTotalResults = CInt(query.Count / myOffset)
                    Dim myIteration As Integer

                    For i As Integer = 1 To myTotalResults Step 1
                        myIteration = i

                        'Get the Position Led
                        Dim PositionLed As Integer
                        myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 4 + (myIteration - 1) * myOffset)
                        If Not myGlobalDataTO.HasError Then
                            PositionLed = CInt(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue)
                        Else
                            Exit For
                        End If

                        For j As Integer = 0 To AppLayer.PhotometryData.PositionLed.Count - 1
                            If AppLayer.PhotometryData.PositionLed(j) = PositionLed Then

                                'MainLine
                                myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 5 + (myIteration - 1) * myOffset)
                                If Not myGlobalDataTO.HasError Then
                                    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                                        AppLayer.PhotometryData.CountsMainBaseline(j) = CInt(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue)
                                    End If
                                Else
                                    Exit For
                                End If

                                'RefLight
                                myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 6 + (myIteration - 1) * myOffset)
                                If Not myGlobalDataTO.HasError Then
                                    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                                        AppLayer.PhotometryData.CountsRefBaseline(j) = CInt(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue)
                                    End If
                                Else
                                    Exit For
                                End If

                                'MainDark
                                myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 7 + (myIteration - 1) * myOffset)
                                If Not myGlobalDataTO.HasError Then
                                    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                                        AppLayer.PhotometryData.CountsMainDarkness(j) = CInt(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue)
                                    End If
                                Else
                                    Exit For
                                End If

                                'RefDark
                                myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 8 + (myIteration - 1) * myOffset)
                                If Not myGlobalDataTO.HasError Then
                                    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                                        AppLayer.PhotometryData.CountsRefDarkness(j) = CInt(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue)
                                    End If
                                Else
                                    Exit For
                                End If

                                'IT
                                myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 9 + (myIteration - 1) * myOffset)
                                If Not myGlobalDataTO.HasError Then
                                    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                                        AppLayer.PhotometryData.IntegrationTimes(j) = CType(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue, Single)
                                    End If

                                Else
                                    Exit For
                                End If
                                'DAC
                                myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 10 + (myIteration - 1) * myOffset)
                                If Not myGlobalDataTO.HasError Then
                                    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                                        AppLayer.PhotometryData.LEDsIntensities(j) = CType(DirectCast(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue, Single)
                                    End If
                                Else
                                    Exit For
                                End If

                                ' Matched !
                                Exit For
                            End If
                        Next

                        ''Get the Position Led
                        'myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 4 + (myIteration - 1) * myOffset)
                        'If Not myGlobalDataTO.HasError Then
                        '    AppLayer.PhotometryData.PositionLed.Add(CInt(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue))
                        'Else
                        '    Exit For
                        'End If

                        ''MainLine
                        'myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 5 + (myIteration - 1) * myOffset)
                        'If Not myGlobalDataTO.HasError Then
                        '    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                        '        AppLayer.PhotometryData.CountsMainBaseline.Add(CInt(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue))
                        '    End If
                        'Else
                        '    Exit For
                        'End If

                        ''RefLight
                        'myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 6 + (myIteration - 1) * myOffset)
                        'If Not myGlobalDataTO.HasError Then
                        '    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                        '        AppLayer.PhotometryData.CountsRefBaseline.Add(CInt(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue))
                        '    End If
                        'Else
                        '    Exit For
                        'End If

                        ''MainDark
                        'myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 7 + (myIteration - 1) * myOffset)
                        'If Not myGlobalDataTO.HasError Then
                        '    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                        '        AppLayer.PhotometryData.CountsMainDarkness.Add(CInt(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue))
                        '    End If
                        'Else
                        '    Exit For
                        'End If

                        ''RefDark
                        'myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 8 + (myIteration - 1) * myOffset)
                        'If Not myGlobalDataTO.HasError Then
                        '    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                        '        AppLayer.PhotometryData.CountsRefDarkness.Add(CInt(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue))
                        '    End If
                        'Else
                        '    Exit For
                        'End If

                        ''IT
                        'myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 9 + (myIteration - 1) * myOffset)
                        'If Not myGlobalDataTO.HasError Then
                        '    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                        '        AppLayer.PhotometryData.IntegrationTimes.Add(CType(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue, Single))
                        '    End If

                        'Else
                        '    Exit For
                        'End If
                        ''DAC
                        'myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 10 + (myIteration - 1) * myOffset)
                        'If Not myGlobalDataTO.HasError Then
                        '    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                        '        AppLayer.PhotometryData.LEDsIntensities.Add(CType(DirectCast(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue, Single))
                        '    End If
                        'Else
                        '    Exit For
                        'End If
                    Next

                End If
                query = Nothing 'AG 02/08/2012 - free memory

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessBaseLineReceived_SRV", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Software has received a new script answer instruction
        ''' This function threats this instruction and decides next action to perform
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns> GlobalDataTo indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Created by:  XBC 16/11/2010
        ''' Modified by: XBC 04/05/2011 - Command instruction already defined
        '''              SA  19/12/2014 - Replaced sentences DirectCast(CInt(myInstParamTO.ParameterValue), Integer) by CInt(myInstParamTO.ParameterValue)
        '''                               due to the first one is redundant and produces building warnings
        ''' </remarks>
        Private Function ProcessFwCommandAnswerReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                ' XBC 28/10/2011 - timeout limit repetitions for Start Tasks
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then
                    InitializeTimerStartTaskControl(WAITING_TIME_OFF)
                    ClearStartTaskQueueToSend()
                End If
                ' XBC 28/10/2011 - timeout limit repetitions for Start Tasks

                Dim myUtilities As New Utilities
                Dim myInstParamTO As New InstructionParameterTO
                Dim myActionValue As AnalyzerManagerAx00Actions = AnalyzerManagerAx00Actions.COMMAND_END
                Me.OpticCenterResultsAttr = New OpticCenterDataTO

                ' XBC 25/10/2012 - is need not clear queue because is posible there are any pending operation (INFO;Q:2) to execute due Alarms (E:99)
                '' Set Waiting Timer Current Instruction OFF
                'If myApplicationName.ToUpper.Contains("SERVICE") Then ClearQueueToSend()
                ' XBC 25/10/2012

                ' Get Status fields (parameter index 3)
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 3)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                Dim myResultValue As Integer
                If IsNumeric(myInstParamTO.ParameterValue) Then
                    myResultValue = CInt(myInstParamTO.ParameterValue)
                End If

                ' Get Ph Main Read Counts values in case its exists
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 4)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If
                If IsNumeric(myInstParamTO.ParameterValue) Then
                    Me.OpticCenterResultsAttr.CountsMain = CInt(myInstParamTO.ParameterValue)
                End If

                ' Get Ph Ref Read Counts values in case its exists
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 5)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If
                If IsNumeric(myInstParamTO.ParameterValue) Then
                    Me.OpticCenterResultsAttr.CountsRef = CInt(myInstParamTO.ParameterValue)
                End If

                ' XBC 21/12/2011 - Add Encoder functionality
                'Encoder
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 6)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If
                If IsNumeric(myInstParamTO.ParameterValue) Then
                    Me.OpticCenterResultsAttr.Encoder = CInt(myInstParamTO.ParameterValue)
                End If
                ' XBC 21/12/2011 - Add Encoder functionality

                'PENDING TO SPEC
                'Level Detection
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 7)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If
                If IsNumeric(myInstParamTO.ParameterValue) Then
                    LevelDetectedAttr = CInt(myInstParamTO.ParameterValue)
                End If



                ManageFwCommandAnswer(myActionValue, myResultValue.ToString)

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessFwCommandAnswerReceived", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' SERVICE - Processes the received Adjustments data
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Modified by XBC : 03/05/2011 - ANSADJ just defined
        ''' Modified by XBC : 30/09/2011 - Save as AnalyzerID received from the Instrument
        ''' </remarks>
        Private Function ProcessFwAdjustmentsReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            ' XBC 25/10/2011 - place here to make sure on exit try to commit or rollback transaction.
            Dim dbConnection As New SqlConnection
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myUtilities As New Utilities
                Dim myInstParamTO As New InstructionParameterTO

                ' Set Waiting Timer Current Instruction OFF
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then ClearQueueToSend()

                'obtain Master Data Adjustments DS
                Dim readAdjustmentsDS As New SRVAdjustmentsDS
                myGlobal = ReadFwAdjustmentsDS()
                If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                    readAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                    If readAdjustmentsDS Is Nothing Then
                        readAdjustmentsDS = New SRVAdjustmentsDS
                    End If

                    'instantiate the delegate
                    Dim myAdjustmentDelegate As New FwAdjustmentsDelegate(readAdjustmentsDS)



                    'convert the data reeived to the DS
                    For i As Integer = 2 To pInstructionReceived.Count - 1
                        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, i + 1)
                        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)

                            myGlobal = myAdjustmentDelegate.ConvertReceivedDataToDS(myInstParamTO)
                            If myGlobal.HasError Then
                                Exit Try
                            End If
                        Else
                            Exit Try
                        End If
                    Next

                    If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                        readAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                        If readAdjustmentsDS IsNot Nothing Then
                            'MyClass.UpdateFwAdjustmentsDS(readAdjustmentsDS) 'SGM 05/12/2011 Update Application main Adjustments Dataset
                            ' XBC 04/10/2011 - Update database
                            Dim myAdjustmentsDelegate As New DBAdjustmentsDelegate
                            myGlobal = myAdjustmentsDelegate.UpdateAdjustmentsDB(Nothing, readAdjustmentsDS)
                        Else
                            Exit Try
                        End If
                    End If

                    'TODO
                    'AG 30/11/2011
                    'By now this attribute is set using a fixed serial number in property ActiveAnalyzer (who implements more business related)
                    '       but when the serial number will be received in adjustments be have to update variable at this point
                    '       calling the ActiveAnalyzer (set) with the new value
                    'ActiveAnalyzer = Received serial number

                    ' XBC 30/09/2011 - insert/update adjustments information into DB
                    If readAdjustmentsDS.srv_tfmwAdjustments.Rows(0).Item("AnalyzerID").ToString = "MasterData" Then   ' XBC 21/06/2012

                        myGlobal = DAOBase.GetOpenDBTransaction(Nothing)

                        If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                            dbConnection = CType(myGlobal.SetDatos, SqlConnection)
                            If (Not dbConnection Is Nothing) Then

                                Dim myAdjustmentsDelegate As New DBAdjustmentsDelegate
                                myGlobal = myAdjustmentsDelegate.ReadAdjustmentsFromDB(dbConnection, AnalyzerIDAttribute)
                                If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                                    readAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                                    If readAdjustmentsDS.srv_tfmwAdjustments.Count > 0 Then
                                        ' 1st step : delete previous adj. readings
                                        myGlobal = myAdjustmentsDelegate.DeleteAdjustmentsDB(dbConnection, AnalyzerIDAttribute)

                                        If myGlobal.HasError Then
                                            Exit Try
                                        End If
                                    End If

                                    ' 2on step : insert adjustment values with its AnalyzerID
                                    myGlobal = myAdjustmentsDelegate.InsertAdjustmentsDB(dbConnection, AnalyzerIDAttribute, FwVersionAttribute)
                                    If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                                        readAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                                    End If
                                End If

                                ' XBC 25/10/2011 - placed on final of the Function to make sure on exit try to commit or rollback transaction.
                                'If (Not myGlobal.HasError) Then
                                '    'When the Database Connection was opened locally, then the Commit is executed
                                '    DAOBase.CommitTransaction(dbConnection)
                                'Else
                                '    'When the Database Connection was opened locally, then the Rollback is executed
                                '    DAOBase.RollbackTransaction(dbConnection)
                                'End If
                            End If

                            ' XBC 25/10/2011 - placed on final of the Function to make sure on exit try to commit or rollback transaction.
                            'If (Not dbConnection Is Nothing) Then dbConnection.Close()
                        End If ' XBC 30/09/2011

                        'AG 07/12/2011 - close transaction
                        If (Not myGlobal.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            DAOBase.RollbackTransaction(dbConnection)
                        End If
                        If (Not dbConnection Is Nothing) Then dbConnection.Close()
                        'AG 07/12/2011

                    End If

                    'updates the system's adjustments dataset
                    myGlobal = UpdateFwAdjustmentsDS(readAdjustmentsDS)
                    If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                        myAdjustmentDelegate = New FwAdjustmentsDelegate(readAdjustmentsDS)
                        myGlobal = myAdjustmentDelegate.ExportDSToFile(AnalyzerIDAttribute)
                        If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                            AdjustmentsFilePath = CStr(myGlobal.SetDatos)
                        End If
                    End If

                    If myGlobal.HasError Then
                        Exit Try
                    End If


                    'SGM 17/02/2012 Initialization of ISE Manager
                    If ISE_Manager IsNot Nothing AndAlso ISE_Manager.IsAnalyzerDisconnected Then
                        'ISE_Manager.Dispose() 'SGM 13/09/2012 clear all resources 
                        ISE_Manager = Nothing 'kill instance of when disconnected mode
                    End If

                    If ISE_Manager Is Nothing Then
                        ISE_Manager = New ISEManager(Me, AnalyzerIDAttribute, myAnalyzerModel)
                    End If

                    'update ISE is installed
                    If ISE_Manager IsNot Nothing Then
                        Dim myISEInstalled As Boolean = False
                        myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByCode(Ax00Adjustsments.ISEINS.ToString)
                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                            Dim res As String = CStr(myGlobal.SetDatos)
                            If IsNumeric(res) Then
                                myISEInstalled = (CInt(res) > 0)
                            End If
                            ISE_Manager.IsISEModuleInstalled = myISEInstalled
                        Else
                            Exit Try
                        End If
                    End If
                    'end SGM 17/02/2012

                End If

                'AG 02/05/2011 - On receive the adjustments inform them into base line calculations class
                If Not baselineCalcs Is Nothing And Not readAdjustmentsDS Is Nothing Then
                    baselineCalcs.Adjustments = readAdjustmentsDS
                End If
                'AG 02/05/2011

                'AG 27/09/2011 - In User Sw send the config instructions after receive the adjustments
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then
                    'Service Sw
                    ' Management is treated in Ax00ServiceMainMDI (ManageReceptionEvent)

                    ' XBC 13/02/2012 - CODEBR Configuration instruction
                    Dim BarCodeDS As New AnalyzerManagerDS
                    Dim rowBarCode As AnalyzerManagerDS.barCodeRequestsRow
                    rowBarCode = BarCodeDS.barCodeRequests.NewbarCodeRequestsRow
                    With rowBarCode
                        .RotorType = "SAMPLES"
                        .Action = Ax00CodeBarAction.CONFIG
                        .Position = 0
                    End With
                    BarCodeDS.barCodeRequests.AddbarCodeRequestsRow(rowBarCode)
                    BarCodeDS.AcceptChanges()
                    myGlobal = ManageAnalyzer(AnalyzerManagerSwActionList.BARCODE_REQUEST, True, Nothing, BarCodeDS)
                    ' XBC 13/02/2012 - CODEBR Configuration instruction
                Else

                    '' XBC 04/10/2011 - changed through a delegate
                    'Dim myAdjustmentsDelegate As New AdjustmentsDelegate
                    'myGlobal = myAdjustmentsDelegate.UpdateAdjustmentsDB(Nothing, readAdjustmentsDS)

                    'User Sw
                    If mySessionFlags(AnalyzerManagerFlags.CONNECTprocess.ToString) = "INPROCESS" Then

                        '' XB 17/09/2013 - Additional protection - commented by now
                        '' - Additional protection to ensure ANSFCP reach to Presentation layer
                        'While WaitingGUI
                        '    System.Threading.Thread.Sleep(1)
                        'End While
                        '' XB 17/09/2013 - Additional protection

                        AnalyzerIsReadyAttribute = True

                        ' XBC 13/02/2012 - CODEBR Configuration instruction
                        'Read the current code bar configuration and send the CODEBR instruction in configuration mode
                        'when ANSCBR will be received in connect process then general CONFIG instruction will be sent
                        'and finally when the status with Action CONFIG_DONE is received the connection process is finished

                        ''AG 23/11/2011 - CODE TO BE COMMENTED!!!
                        ''When the CODEBR configuration instruction will be sent then comment this line
                        'myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.CONFIG, True)

                        Dim BarCodeDS As New AnalyzerManagerDS
                        Dim rowBarCode As AnalyzerManagerDS.barCodeRequestsRow
                        rowBarCode = BarCodeDS.barCodeRequests.NewbarCodeRequestsRow
                        With rowBarCode
                            .RotorType = "SAMPLES"
                            .Action = Ax00CodeBarAction.CONFIG
                            .Position = 0
                        End With
                        BarCodeDS.barCodeRequests.AddbarCodeRequestsRow(rowBarCode)
                        BarCodeDS.AcceptChanges()
                        myGlobal = ManageAnalyzer(AnalyzerManagerSwActionList.BARCODE_REQUEST, True, Nothing, BarCodeDS)
                        ' XBC 13/02/2012 - CODEBR Configuration instruction

                        If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                            SetAnalyzerNotReady()
                        End If


                        ''AG 27/09/2011 - CODE TO BE COMMENTED!!!
                        ''START temporal code to be commented once the TODO code becomes enabled
                        'Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
                        'myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATE, True, Nothing, Nothing)

                        'If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                        '    'Update analyzer session flags into DataBase
                        '    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess, "CLOSED")
                        '    If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                        '        Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                        '        myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                        '    End If

                        '    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.CONNECTED, CSng(IIf(ConnectedAttribute, 1, 0)), True)
                        'End If
                        ''END temporal code to be commented
                        ''AG 27/09/2011 - END CODE TO BE COMMENTED!!!

                    End If
                End If
                'AG 27/09/2011

                ' XBC 03/10/2012
                AdjustmentsReadAttr = True

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessFwAdjustmentsReceived", EventLogEntryType.Error, False)
            End Try

            ' XBC 25/10/2011 - place here to make sure on exit try to commit or rollback transaction.
            If Not dbConnection Is Nothing AndAlso Not dbConnection.State = ConnectionState.Closed Then
                If (Not myGlobal.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Database Connection was opened locally, then the Rollback is executed
                    DAOBase.RollbackTransaction(dbConnection)
                End If
                If (Not dbConnection Is Nothing) Then dbConnection.Close()
            End If

            Return myGlobal
        End Function

        ''' <summary>
        ''' SERVICE - Processes the received Cycles data
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM : 27/07/2011 - ANSCYC just defined</remarks>
        Private Function PDT_ProcessHwCyclesReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO

            Try
                'PENDING TO SPEC!!!!!!!!!!!


                'myGlobal = UpdateHwCycles(pInstructionReceived, GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_TEMP)





            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessHwCyclesReceived", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' Answer Stress / Demo Mode
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 22/03/2011</remarks>
        Private Function ProcessStressModeReceived_SRV(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myUtilities As New Utilities
                Dim myInstParamTO As New InstructionParameterTO
                Dim myActionValue As AnalyzerManagerAx00Actions = AnalyzerManagerAx00Actions.NO_ACTION

                ' Set Waiting Timer Current Instruction OFF
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then ClearQueueToSend()


                ' Status
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 5)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    If IsNumeric(myInstParamTO.ParameterValue) Then
                        AppLayer.StressModeData.Status = DirectCast(CInt(myInstParamTO.ParameterValue), STRESS_STATUS)

                        ' XBC 16/05/2012 - reactivate sensors AnsInf when initate into Stressing mode
                        If AppLayer.StressModeData.Status = STRESS_STATUS.FINISHED_OK Or _
                           AppLayer.StressModeData.Status = STRESS_STATUS.FINISHED_ERR Then
                            SensorValuesAttribute.Clear()
                            InfoRefreshFirstTime = True
                        End If
                        ' XBC 16/05/2012

                    End If
                Else
                    Exit Try
                End If
                ' Num Cycles
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 6)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    If IsNumeric(myInstParamTO.ParameterValue) Then
                        AppLayer.StressModeData.NumCyclesCompleted = CInt(myInstParamTO.ParameterValue)
                    End If
                Else
                    Exit Try
                End If
                ' Num Cycles completed
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 7)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    If IsNumeric(myInstParamTO.ParameterValue) Then
                        AppLayer.StressModeData.NumCycles = CInt(myInstParamTO.ParameterValue)
                    End If
                Else
                    Exit Try
                End If
                ' Start Hour
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 8)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    If IsNumeric(myInstParamTO.ParameterValue) Then
                        AppLayer.StressModeData.StartHour = CInt(myInstParamTO.ParameterValue)
                    End If
                Else
                    Exit Try
                End If
                ' Start Minute
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 9)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    If IsNumeric(myInstParamTO.ParameterValue) Then
                        AppLayer.StressModeData.StartMinute = CInt(myInstParamTO.ParameterValue)
                    End If
                Else
                    Exit Try
                End If
                ' Start Second
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 10)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    If IsNumeric(myInstParamTO.ParameterValue) Then
                        AppLayer.StressModeData.StartSecond = CInt(myInstParamTO.ParameterValue)
                    End If
                Else
                    Exit Try
                End If


                ' maybe is no used ?
                AppLayer.StressModeData.StartDatetime = New DateTime(Now.Date.Year, _
                                                                     Now.Date.Month, _
                                                                     Now.Date.Day, _
                                                                     Now.Date.Hour, _
                                                                     Now.Date.Minute, _
                                                                     Now.Date.Second)

                'AppLayer.StressModeData.StartDatetime.AddHours(-pStressStartHour)
                'AppLayer.StressModeData.StartDatetime.AddMinutes(-pStressStartMinute)
                'AppLayer.StressModeData.StartDatetime.AddSeconds(-pStressStartSecond)

                ' Stress Type Element
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 11)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    If IsNumeric(myInstParamTO.ParameterValue) Then
                        AppLayer.StressModeData.Type = DirectCast(CInt(myInstParamTO.ParameterValue), STRESS_TYPE)
                    End If
                Else
                    Exit Try
                End If
                ' Num Resets
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 12)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    If IsNumeric(myInstParamTO.ParameterValue) Then
                        AppLayer.StressModeData.NumResets = CInt(myInstParamTO.ParameterValue)
                    End If
                Else
                    Exit Try
                End If
                ' Cycles Resets  PDT !!! (list of...)
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 13)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    AppLayer.StressModeData.CyclesResets = New List(Of Long)
                    If IsNumeric(myInstParamTO.ParameterValue) Then
                        AppLayer.StressModeData.CyclesResets.Add(CInt(myInstParamTO.ParameterValue))
                    End If
                Else
                    Exit Try
                End If
                ' Num Errors
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 14)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    If IsNumeric(myInstParamTO.ParameterValue) Then
                        AppLayer.StressModeData.NumErrors = CInt(myInstParamTO.ParameterValue)
                    End If
                Else
                    Exit Try
                End If
                ' Codes Errors  PDT !!! (list of...)
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 15)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    AppLayer.StressModeData.CodeErrors = New List(Of STRESS_ERRORS)
                    If IsNumeric(myInstParamTO.ParameterValue) Then
                        AppLayer.StressModeData.CodeErrors.Add(DirectCast(CInt(myInstParamTO.ParameterValue), STRESS_ERRORS))
                    End If
                Else
                    Exit Try
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessStressModeReceived_SRV", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        Private Function UpdateHwElement(ByVal pInstructionReceived As List(Of InstructionParameterTO), _
                                               ByVal pElementID As Object) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Dim myUtilities As New Utilities
            Dim myCpuItem As CPU_ELEMENTS
            Dim myManifoldItem As MANIFOLD_ELEMENTS
            Dim myFluidicsItem As FLUIDICS_ELEMENTS
            Dim myPhotometricsItem As PHOTOMETRICS_ELEMENTS
            Dim myInstParamTO As New InstructionParameterTO

            Try
                Dim myValue As String = ""
                Dim myParIndex As Integer = 0

                'ONLY FOR SERVICE
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then


                    'CPU
                    If TypeOf pElementID Is CPU_ELEMENTS Then
                        myCpuItem = CType(pElementID, CPU_ELEMENTS)
                        If myCpuItem <> Nothing Then
                            myParIndex = 3 + CInt(myCpuItem)
                            myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, myParIndex)
                            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                            Else
                                Exit Try
                            End If
                            myValue = myInstParamTO.ParameterValue.Replace(".", ",")
                            'update dataset
                            UpdateCpuValuesAttribute(myCpuItem, myValue, True)
                            myGlobal.SetDatos = myValue
                        End If
                    End If

                    'MANIFOLD
                    If TypeOf pElementID Is MANIFOLD_ELEMENTS Then
                        myManifoldItem = CType(pElementID, MANIFOLD_ELEMENTS)
                        If myManifoldItem <> Nothing Then
                            myParIndex = 3 + CInt(myManifoldItem)
                            myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, myParIndex)
                            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                            Else
                                Exit Try
                            End If
                            myValue = myInstParamTO.ParameterValue.Replace(".", ",")
                            'update dataset
                            UpdateManifoldValuesAttribute(myManifoldItem, myValue, True)
                            myGlobal.SetDatos = myValue
                        End If
                    End If

                    'FLUIDICS
                    If TypeOf pElementID Is FLUIDICS_ELEMENTS Then
                        myFluidicsItem = CType(pElementID, FLUIDICS_ELEMENTS)
                        If myFluidicsItem <> Nothing Then
                            myParIndex = 3 + CInt(myFluidicsItem)
                            myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, myParIndex)
                            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                            Else
                                Exit Try
                            End If
                            myValue = myInstParamTO.ParameterValue.Replace(".", ",")
                            'update dataset
                            UpdateFluidicsValuesAttribute(myFluidicsItem, myValue, True)
                            myGlobal.SetDatos = myValue
                        End If
                    End If

                    'PHOTOMETRICS
                    If TypeOf pElementID Is PHOTOMETRICS_ELEMENTS Then
                        myPhotometricsItem = CType(pElementID, PHOTOMETRICS_ELEMENTS)
                        If myPhotometricsItem <> Nothing Then
                            myParIndex = 3 + CInt(myPhotometricsItem)
                            myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, myParIndex)
                            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                            Else
                                Exit Try
                            End If
                            myValue = myInstParamTO.ParameterValue.Replace(".", ",")
                            'update dataset
                            UpdatePhotometricsValuesAttribute(myPhotometricsItem, myValue, True)
                            myGlobal.SetDatos = myValue
                        End If
                    End If

                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.UpdateHwElement", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        Private Function PDT_UpdateHwCycles(ByVal pInstructionReceived As List(Of InstructionParameterTO), _
                                              ByVal pElementID As Object) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim myUtilities As New Utilities
            Dim myCycleItem As CYCLE_ELEMENTS
            Dim myInstParamTO As New InstructionParameterTO

            Try
                'PENDING TO SPEC!!!!!!!!!!!
                Dim myValue As String = ""
                Dim myParIndex As Integer = 0

                'ONLY FOR SERVICE
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then

                    'CPU
                    If TypeOf pElementID Is CYCLE_ELEMENTS Then
                        myCycleItem = CType(pElementID, CYCLE_ELEMENTS)
                        If myCycleItem <> Nothing Then
                            myParIndex = 3 + CInt(myCycleItem)
                            myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, myParIndex)
                            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                            Else
                                Exit Try
                            End If
                            myValue = myInstParamTO.ParameterValue.Replace(".", ",")
                            'update dataset
                            'UpdateHwCyclesValuesAttribute(myCycleItem, myValue, True)
                            myGlobal.SetDatos = myValue
                        End If

                    End If

                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.UpdateHwCycles", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function





        ''' <summary>
        ''' SW has received and ANSCPU instruction for real time monitoring
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>XBC 08/06/2011 - creation</remarks>
        Private Function ProcessHwCpuStatusReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try
                'Dim myUtilities As New Utilities
                'Dim myInstParamTO As New InstructionParameterTO
                'Dim myElements As New Dictionary(Of GlobalEnumerates.CPU_ELEMENTS, String) 'Local structure

                ' Set Waiting Timer Current Instruction OFF
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then ClearQueueToSend()

                Dim myValue As String = ""
                Dim myParIndex As Integer = 0

                'Board temperature------------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, CPU_ELEMENTS.CPU_TEMP)

                'CAN Status ------------------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, CPU_ELEMENTS.CPU_CAN)

                'CAN Diagnostic ARMS ---------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, CPU_ELEMENTS.CPU_CAN_BM1)
                myGlobal = UpdateHwElement(pInstructionReceived, CPU_ELEMENTS.CPU_CAN_BR1)
                myGlobal = UpdateHwElement(pInstructionReceived, CPU_ELEMENTS.CPU_CAN_BR2)
                myGlobal = UpdateHwElement(pInstructionReceived, CPU_ELEMENTS.CPU_CAN_AG1)
                myGlobal = UpdateHwElement(pInstructionReceived, CPU_ELEMENTS.CPU_CAN_AG2)

                'CAN Diagnostic PROBES ---------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, CPU_ELEMENTS.CPU_CAN_DM1)
                myGlobal = UpdateHwElement(pInstructionReceived, CPU_ELEMENTS.CPU_CAN_DR1)
                myGlobal = UpdateHwElement(pInstructionReceived, CPU_ELEMENTS.CPU_CAN_DR2)

                'CAN Diagnostic ROTORS ---------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, CPU_ELEMENTS.CPU_CAN_RR1)
                myGlobal = UpdateHwElement(pInstructionReceived, CPU_ELEMENTS.CPU_CAN_RM1)

                'CAN Diagnostic PHOTOMETRIC-----------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, CPU_ELEMENTS.CPU_CAN_GLF)

                'CAN Diagnostic MANIFOLD -------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, CPU_ELEMENTS.CPU_CAN_JE1)

                'CAN Diagnostic FLUIDICS -------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, CPU_ELEMENTS.CPU_CAN_SF1)


                ' MAIN COVER -------------------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, CPU_ELEMENTS.CPU_MC)

                ' BUZZ STATUS ------------------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, CPU_ELEMENTS.CPU_BUZ)

                ' FW FLASH MEMORY --------------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, CPU_ELEMENTS.CPU_FWFM)

                ' BLACK BOX --------------------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, CPU_ELEMENTS.CPU_BBFM)

                ' ISE STATUS -------------------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, CPU_ELEMENTS.CPU_ISE)

                ' ISE ERROR --------------------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, CPU_ELEMENTS.CPU_ISEE)


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessHwCpuUStatusReceived", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' SW has received and ANSJEX instruction for real time monitoring
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>SGM 23/05/2011 - creation</remarks>
        Private Function ProcessHwManifoldStatusReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try
                'Dim myUtilities As New Utilities
                'Dim myInstParamTO As New InstructionParameterTO
                'Dim myElements As New Dictionary(Of GlobalEnumerates.MANIFOLD_ELEMENTS, String) 'Local structure

                ' Set Waiting Timer Current Instruction OFF
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then ClearQueueToSend()

                Dim myValue As String = ""
                Dim myParIndex As Integer = 0

                'Board temperature------------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_TEMP)

                'Reagent1 Motor------------------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_MR1)
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_MR1H)
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_MR1A)

                'Reagent2  Motor---------------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_MR2)
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_MR2H)
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_MR2A)

                'Samples  Motor-------------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_MS)
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_MSH)
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_MSA)


                'Samples Dosing Pump-------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_B1)
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_B1D)

                'Reagent 1 Dosing Pump-------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_B2)
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_B2D)


                'Reagent 2 Dosing Pump-------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_B3)
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_B3D)


                'Samples Dosing Valve-------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_EV1)
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_EV1D)



                'Reagent 1 Dosing Valve--------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_EV2)
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_EV2D)




                'Reagent 2 Dosing Valve-------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_EV3)
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_EV3D)


                'Air OR Washing Solution Valve------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_EV4)
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_EV4D)



                'Air/Washing OR Purified Water Solution Valve-----------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_EV5)
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_EV5D)


                'EV6-----------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_EV6)
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_EV6D)


                'GE1-----------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_GE1)
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_GE1D)


                'GE2-----------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_GE2)
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_GE2D)


                'GE3-----------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_GE3)
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_GE3D)


                'Clot detection---------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_CLT)
                myGlobal = UpdateHwElement(pInstructionReceived, MANIFOLD_ELEMENTS.JE1_CLTD)



            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessHwManifoldStatusReceived", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' SW has received and ANSSFX instruction for real time monitoring
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>SGM 23/05/2011 - creation</remarks>
        Private Function ProcessHwFluidicsStatusReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try
                'Dim myUtilities As New Utilities
                'Dim myInstParamTO As New InstructionParameterTO
                'Dim myElements As New Dictionary(Of GlobalEnumerates.FLUIDICS_ELEMENTS, String) 'Local structure

                ' Set Waiting Timer Current Instruction OFF
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then ClearQueueToSend()

                Dim myValue As String = ""
                Dim myParIndex As Integer = 0


                'Board temperature------------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_TEMP)

                'Washing Station Motor---------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_MS)
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_MSH)
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_MSA)


                'Samples Arm Pump-----------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_B1)
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_B1D)


                'Reagent1/Mixer2 Arm Pump------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_B2)
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_B2D)

                'Reagent2/Mixer1 Arm Pump------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_B3)
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_B3D)

                'Purified Water (from external tank) Pump-----------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_B4)
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_B4D)

                'Low Contamination Pump---------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_B5)
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_B5D)

                'Washing Station needle 2,3 Pump-----------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_B6)
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_B6D)

                'Washing Station needle 4,5 Pump--------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_B7)
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_B7D)


                'Washing Station needle 6 Pump---------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_B8)
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_B8D)

                'Washing Station needle 7 Pump--------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_B9)
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_B9D)

                'Washing Station needle 1 Pump-----------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_B10)
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_B10D)


                ''Dispensation ElectroValves Group-----------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_GE1)
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_GE1D)


                'Purified Water (from external source) Valve---------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_EV1)
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_EV1D)


                'Purified Water (from external tank) Valve----------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_EV2)
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_EV2D)

                'EV3----------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_EV3)
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_EV3D)

                ''Washing Station Thermistor-------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_WSTH)
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_WSTHD)

                'Washing Station Heater---------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_WSH)
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_WSHD)

                'Washing Solution Weight----------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_WSW)
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_WSWD)

                'High Contamination Weight------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_HCW)
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_HCWD)


                'Waste sensor (boyas)----------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_WAS)

                'operation state
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_WTS)

                'System liquid sensor (boyas)-----------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_SLS)

                'operation state
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_STS)

                'stirrer 1-------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_ST1)

                'stirrer 2-------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, FLUIDICS_ELEMENTS.SF1_ST2)




            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessHwFluidicsStatusReceived", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' SW has received and ANSGLF instruction for real time monitoring
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>SGM 23/05/2011 - creation</remarks>
        Private Function ProcessHwPhotometricsStatusReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try
                'Dim myUtilities As New Utilities
                'Dim myInstParamTO As New InstructionParameterTO
                'Dim myElements As New Dictionary(Of GlobalEnumerates.PHOTOMETRICS_ELEMENTS, String) 'Local structure

                ' Set Waiting Timer Current Instruction OFF
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then ClearQueueToSend()

                'PENDING TO SPEC DATA FORMAT
                'Get General cover (parameter index 3)
                Dim myValue As String = ""

                'Board temperature------------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_TEMP)


                'Reactions Rotor Motor------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_MR)
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_MRH)
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_MRA)
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_MRE)
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_MRED)



                'Washing Station Vertical Motor (Pinta)---------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_MW)
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_MWH)
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_MWA)



                'Washing Station Collision Detector-----------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_CD)



                'Rotor Thermistor--------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_PTH)
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_PTHD)


                'Rotor Peltier------------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_PH)
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_PHD)


                'Peltier Fan 1--------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_PF1)
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_PF1D)


                'Peltier Fan 2--------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_PF2)
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_PF2D)


                'Peltier Fan 3--------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_PF3)
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_PF3D)


                'Peltier Fan 4--------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_PF4)
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_PF4D)

                'Rotor Cover---------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_RC)


                'Photometry------------------------------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_PHT)


                'Photometry Flash Memory State----------------------------------------
                myGlobal = UpdateHwElement(pInstructionReceived, PHOTOMETRICS_ELEMENTS.GLF_PHFM)


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessHwPhotometricsStatusReceived", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        Private Function GetFWInfoData(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myUtilities As New Utilities
                Dim myInstParamTO As New InstructionParameterTO
                Dim myElements As New Dictionary(Of FW_INFO, String) 'Local structure
                Dim FwVersion As String = ""
                Dim AnalyzerID As String = ""

                'Get Ientificator value (parameter index 3)
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 3)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(FW_INFO.ID, DirectCast(myInstParamTO.ParameterValue, String))
                Else
                    Exit Try
                End If

                'Get Board serial number (parameter index 4)
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 4)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(FW_INFO.SMC, DirectCast(myInstParamTO.ParameterValue, String))
                Else
                    Exit Try
                End If

                'Get Firmware Version (parameter index 5)
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 5)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(FW_INFO.FWV, DirectCast(myInstParamTO.ParameterValue, String))
                Else
                    Exit Try
                End If

                'Get Repository CRC32 result (OK,NOK) (parameter index 6)
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 6)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(FW_INFO.FWCRC, DirectCast(myInstParamTO.ParameterValue, String))
                Else
                    Exit Try
                End If

                'Get Repository CRC32 value  (parameter index 7)
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 7)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(FW_INFO.FWCRCV, DirectCast(myInstParamTO.ParameterValue, String))
                Else
                    Exit Try
                End If

                'Get Repository CRC32 size (parameter index 8)
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 8)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(FW_INFO.FWCRCS, DirectCast(myInstParamTO.ParameterValue, String))
                Else
                    Exit Try
                End If

                'Get Hardware version(parameter index 13)
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 9)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(FW_INFO.HWV, DirectCast(myInstParamTO.ParameterValue, String))
                Else
                    Exit Try
                End If

                myGlobal.SetDatos = myElements

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.GetFWInfoData", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Gets all trhe data refered to Firmware CPU like versions
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 25/05/2012</remarks>
        Private Function GetFWCPUInfoData(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myUtilities As New Utilities
                Dim myInstParamTO As New InstructionParameterTO
                Dim myElements As New Dictionary(Of FW_INFO, String) 'Local structure
                Dim FwVersion As String = ""
                Dim AnalyzerID As String = ""

                'Get Ientificator value (parameter index 3) 
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 3)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(FW_INFO.ID, DirectCast(myInstParamTO.ParameterValue, String))
                Else
                    Exit Try
                End If

                'Get Board serial number (parameter index 4)'Hexadecimal
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 4)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(FW_INFO.SMC, DirectCast(myInstParamTO.ParameterValue, String))
                Else
                    Exit Try
                End If

                'FIRMWARE VERSION FOR VALIDATING SW-FW COMPATIBILITY
                'Get Repository Version (parameter index 5) 'Float
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 5)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(FW_INFO.RV, DirectCast(myInstParamTO.ParameterValue, String))
                Else
                    Exit Try
                End If

                'Get Repository CRC32 result (OK,NOK) (parameter index 6)'string
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 6)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(FW_INFO.CRC, DirectCast(myInstParamTO.ParameterValue, String))
                Else
                    Exit Try
                End If

                'Get Repository CRC32 value  (parameter index 7)'Hexadecimal
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 7)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(FW_INFO.CRCV, DirectCast(myInstParamTO.ParameterValue, String))
                Else
                    Exit Try
                End If

                'Get Repository CRC32 size (parameter index 8)'int32
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 8)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(FW_INFO.CRCS, DirectCast(myInstParamTO.ParameterValue, String))
                Else
                    Exit Try
                End If

                'Get Fw version (CPU Board) (parameter index 9) float
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 9)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(FW_INFO.FWV, DirectCast(myInstParamTO.ParameterValue, String))
                Else
                    Exit Try
                End If

                'Get CPU CRC32 result (OK,NOK) (parameter index 10) string
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 10)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(FW_INFO.FWCRC, DirectCast(myInstParamTO.ParameterValue, String))
                Else
                    Exit Try
                End If

                'Get CPU CRC32 value (parameter index 11) hexadecimal
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 11)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(FW_INFO.FWCRCV, DirectCast(myInstParamTO.ParameterValue, String))
                Else
                    Exit Try
                End If

                'Get CPU CRC32 size (parameter index 12) int32
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 12)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(FW_INFO.FWCRCS, DirectCast(myInstParamTO.ParameterValue, String))
                Else
                    Exit Try
                End If

                'Get Hardware version(parameter index 13) float
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 13)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(FW_INFO.HWV, DirectCast(myInstParamTO.ParameterValue, String))
                Else
                    Exit Try
                End If


                'Analyzer Serial number (parameter index 14)string
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 14)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    AnalyzerID = DirectCast(myInstParamTO.ParameterValue, String)
                    myElements.Add(FW_INFO.ASN, AnalyzerID)
                Else
                    Exit Try
                End If

                myGlobal.SetDatos = myElements

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.GetFWCPUInfoData", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' SW has received and ANSFCP instruction
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 25/05/2011
        ''' Modified by XBC 02/06/2011 - ANSFW becomes in ANSFCP, ANSFBX, ANSFDX, ANSFRX, ANSFGL, ANSFJE, ANSFSF
        ''' </remarks>
        Private Function ProcessFwCPUDetailsReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            ' XBC 25/10/2011 - place here to make sure on exit try to commit or rollback transaction.
            Dim dbConnection As New SqlConnection
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myUtilities As New Utilities
                Dim myElements As New Dictionary(Of FW_INFO, String) 'Local structure
                'Dim myInstParamTO As New InstructionParameterTO
                Dim SwVersion As String = ""
                Dim FwVersion As String = ""
                Dim AnalyzerID As String = ""

                ' Set Waiting Timer Current Instruction OFF
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then ClearQueueToSend()

                myGlobal = GetFWCPUInfoData(pInstructionReceived)
                If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then

                    myElements = CType(myGlobal.SetDatos, Dictionary(Of FW_INFO, String))

                    AnalyzerID = myElements(FW_INFO.ASN)
                    FwVersion = myElements(FW_INFO.RV)

                    ReadedFwVersion = FwVersion

                    ' XBC 06/06/2012
                    Dim confAnalyzers As New AnalyzersDelegate
                    Dim myAnalyzersRow As AnalyzersDS.tcfgAnalyzersRow
                    Dim myAnalyzersDS As New AnalyzersDS
                    myAnalyzersRow = myAnalyzersDS.tcfgAnalyzers.NewtcfgAnalyzersRow

                    myAnalyzersRow.AnalyzerID = AnalyzerID
                    myAnalyzersRow.AnalyzerModel = GetModelValue(AnalyzerID)
                    myAnalyzersRow.FirmwareVersion = FwVersion

                    IsFwReaded = True 'SGM 21/06/2012

                    'Validate the SW-FW Compatibility
                    myGlobal = myUtilities.GetSoftwareVersion()
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then

                        SwVersion = myGlobal.SetDatos.ToString
                        myGlobal = ValidateFwSwCompatibility(FwVersion, SwVersion)
                        If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then

                            IsFwSwCompatible = CBool(myGlobal.SetDatos)

                            myGlobal = DAOBase.GetOpenDBTransaction(Nothing)
                            If (Not myGlobal.HasError) Then
                                dbConnection = CType(myGlobal.SetDatos, SqlConnection)
                                If (Not dbConnection Is Nothing) Then

                                    If IsFwSwCompatible Then

                                        Dim myLogAccionesAux As New ApplicationLogManager()
                                        myLogAccionesAux.CreateLogActivity("(Analyzer Change) Check Analyzer ", "AnalyzerManager.ProcessFwCPUDetailsReceived", EventLogEntryType.Information, False)

                                        myGlobal = confAnalyzers.CheckAnalyzer(dbConnection, myAnalyzersRow)

                                        If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                                            myTmpConnectedAnalyzerDS = CType(myGlobal.SetDatos, AnalyzersDS)

                                            If Not myTmpConnectedAnalyzerDS Is Nothing Then
                                                If myTmpConnectedAnalyzerDS.tcfgAnalyzers.Count > 0 Then
                                                    myLogAccionesAux.CreateLogActivity("(Analyzer Change) Temp Connected Analyzer [" & myTmpConnectedAnalyzerDS.tcfgAnalyzers(0).AnalyzerID & "] ", "AnalyzerManager.ProcessFwCPUDetailsReceived", EventLogEntryType.Information, False)
                                                End If
                                            End If

                                            TemporalAnalyzerConnectedAttr = AnalyzerID
                                            ActiveFwVersion = FwVersion

                                            'SGM 01/02/2012 - Check if it is User Assembly - Bug #1112
                                            'If Not My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                            If Not GlobalBase.IsServiceAssembly Then
                                                'User Sw
                                                IsAnalyzerIDNotLikeWSAttr = False
                                                Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate()
                                                Dim myWSStatus As String = ""
                                                Dim myWSAnalyzerID As String = ""
                                                myGlobal = myWSAnalyzersDelegate.GetActiveWSByAnalyzer(dbConnection)
                                                If Not myGlobal.HasError Then
                                                    Dim myWSAnalyzersDS As New WSAnalyzersDS
                                                    myWSAnalyzersDS = DirectCast(myGlobal.SetDatos, WSAnalyzersDS)
                                                    If myWSAnalyzersDS.twksWSAnalyzers.Count > 0 Then
                                                        myWSStatus = myWSAnalyzersDS.twksWSAnalyzers(0).WSStatus
                                                        myWSAnalyzerID = myWSAnalyzersDS.twksWSAnalyzers(0).AnalyzerID
                                                        If myWSStatus = "EMPTY" Or myWSStatus = "OPEN" Then
                                                            ' Nothing to do
                                                        Else
                                                            If myWSAnalyzerID = TemporalAnalyzerConnected Then
                                                                ' Nothing to do
                                                            Else
                                                                IsAnalyzerIDNotLikeWSAttr = True
                                                            End If
                                                        End If
                                                    End If
                                                End If
                                            End If

                                        Else
                                            myLogAccionesAux.CreateLogActivity("(Analyzer Change) Error into Check Analyzer method ! ", "AnalyzerManager.ProcessFwCPUDetailsReceived", EventLogEntryType.Error, False)
                                        End If
                                    Else
                                        ' FW Not compatible with SW !!!
                                        ' This way is treated on MDI into ANSFCP event reception
                                    End If
                                End If

                            End If

                        End If
                    End If

                End If

                'SGM 20/06/2012
                If (Not myGlobal.HasError) Then

                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        ServiceFWInfoTreatment(myElements)
                    Else
                        'Nothing by now
                    End If

                    'Generate UI_Refresh event FWCPUVALUE_CHANGED
                    If Not myUI_RefreshEvent.Contains(UI_RefreshEvents.FWCPUVALUE_CHANGED) Then myUI_RefreshEvent.Add(UI_RefreshEvents.FWCPUVALUE_CHANGED)


                    If IsFwSwCompatible Then
                        'SEND READADJ
                        ' XBC 05/06/2012
                        'SGM 01/02/2012 - Check if it is User Assembly - Bug #1112
                        'If Not My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                        If Not GlobalBase.IsServiceAssembly Then
                            'User Sw
                            If Not IsAnalyzerIDNotLikeWSAttr Then
                                If mySessionFlags(AnalyzerManagerFlags.CONNECTprocess.ToString) = "INPROCESS" Then
                                    myGlobal = ManageAnalyzer(AnalyzerManagerSwActionList.READADJ, True, Nothing, Ax00Adjustsments.ALL)
                                    If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                                        SetAnalyzerNotReady()
                                    End If
                                End If
                            End If
                        End If
                        ' XBC 05/06/2012
                    End If

                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessFwCPUDetailsReceived", EventLogEntryType.Error, False)
            End Try

            ' XBC 25/10/2011 - place here to make sure on exit try to commit or rollback transaction.
            If (Not myGlobal.HasError) Then
                'When the Database Connection was opened locally, then the Commit is executed
                DAOBase.CommitTransaction(dbConnection)
            Else
                'When the Database Connection was opened locally, then the Rollback is executed
                DAOBase.RollbackTransaction(dbConnection)
            End If
            If (Not dbConnection Is Nothing) Then dbConnection.Close()

            Return myGlobal
        End Function

        ''' <summary>
        ''' SW has received and ANSFBX instruction
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 02/06/2011
        ''' </remarks>
        Private Function ProcessFwARMDetailsReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'Dim myUtilities As New Utilities
                'Dim myInstParamTO As New InstructionParameterTO
                Dim myElements As New Dictionary(Of FW_INFO, String) 'Local structure

                ' Set Waiting Timer Current Instruction OFF
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then ClearQueueToSend()

                myGlobal = GetFWInfoData(pInstructionReceived)
                If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                    myElements = CType(myGlobal.SetDatos, Dictionary(Of FW_INFO, String))
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        ServiceFWInfoTreatment(myElements)
                    Else
                        'Nothing by now
                    End If

                    'Generate UI_Refresh event FWARMVALUE_CHANGED
                    If Not myUI_RefreshEvent.Contains(UI_RefreshEvents.FWARMVALUE_CHANGED) Then myUI_RefreshEvent.Add(UI_RefreshEvents.FWARMVALUE_CHANGED)

                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessFwARMDetailsReceived", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' SW has received and ANSFDX instruction
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 02/06/2011
        ''' </remarks>
        Private Function ProcessFwPROBEDetailsReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'Dim myUtilities As New Utilities
                'Dim myInstParamTO As New InstructionParameterTO
                Dim myElements As New Dictionary(Of FW_INFO, String) 'Local structure

                ' Set Waiting Timer Current Instruction OFF
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then ClearQueueToSend()

                myGlobal = GetFWInfoData(pInstructionReceived)
                If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                    myElements = CType(myGlobal.SetDatos, Dictionary(Of FW_INFO, String))
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then

                        ServiceFWInfoTreatment(myElements)

                    Else
                        'Nothing by now
                    End If

                    'Generate UI_Refresh event FWPROBEVALUE_CHANGED
                    If Not myUI_RefreshEvent.Contains(UI_RefreshEvents.FWPROBEVALUE_CHANGED) Then myUI_RefreshEvent.Add(UI_RefreshEvents.FWPROBEVALUE_CHANGED)

                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessFwPROBEDetailsReceived", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' SW has received and ANSFRX instruction
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 02/06/2011
        ''' </remarks>
        Private Function ProcessFwROTORDetailsReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'Dim myUtilities As New Utilities
                'Dim myInstParamTO As New InstructionParameterTO
                Dim myElements As New Dictionary(Of FW_INFO, String) 'Local structure

                ' Set Waiting Timer Current Instruction OFF
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then ClearQueueToSend()

                myGlobal = GetFWInfoData(pInstructionReceived)
                If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                    myElements = CType(myGlobal.SetDatos, Dictionary(Of FW_INFO, String))
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then

                        ServiceFWInfoTreatment(myElements)

                    Else
                        'Nothing by now
                    End If

                    'Generate UI_Refresh event FWROTORVALUE_CHANGED
                    If Not myUI_RefreshEvent.Contains(UI_RefreshEvents.FWROTORVALUE_CHANGED) Then myUI_RefreshEvent.Add(UI_RefreshEvents.FWROTORVALUE_CHANGED)

                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessFwROTORDetailsReceived", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' SW has received and ANSFGL instruction
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 02/06/2011
        ''' </remarks>
        Private Function ProcessFwPHOTOMETRICDetailsReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'Dim myUtilities As New Utilities
                'Dim myInstParamTO As New InstructionParameterTO
                Dim myElements As New Dictionary(Of FW_INFO, String) 'Local structure

                ' Set Waiting Timer Current Instruction OFF
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then ClearQueueToSend()

                myGlobal = GetFWInfoData(pInstructionReceived)
                If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                    myElements = CType(myGlobal.SetDatos, Dictionary(Of FW_INFO, String))
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then

                        ServiceFWInfoTreatment(myElements)

                    Else
                        'Nothing by now
                    End If

                    'Generate UI_Refresh event FWPHOTOMETRICVALUE_CHANGED
                    If Not myUI_RefreshEvent.Contains(UI_RefreshEvents.FWPHOTOMETRICVALUE_CHANGED) Then myUI_RefreshEvent.Add(UI_RefreshEvents.FWPHOTOMETRICVALUE_CHANGED)

                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessFwPHOTOMETRICDetailsReceived", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' SW has received and ANSFJE instruction
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 02/06/2011
        ''' </remarks>
        Private Function ProcessFwMANIFOLDDetailsReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'Dim myUtilities As New Utilities
                'Dim myInstParamTO As New InstructionParameterTO
                Dim myElements As New Dictionary(Of FW_INFO, String) 'Local structure

                ' Set Waiting Timer Current Instruction OFF
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then ClearQueueToSend()

                myGlobal = GetFWInfoData(pInstructionReceived)
                If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                    myElements = CType(myGlobal.SetDatos, Dictionary(Of FW_INFO, String))
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then

                        ServiceFWInfoTreatment(myElements)

                    Else
                        'Nothing by now
                    End If

                    'Generate UI_Refresh event FWMANIFOLDVALUE_CHANGED
                    If Not myUI_RefreshEvent.Contains(UI_RefreshEvents.FWMANIFOLDVALUE_CHANGED) Then myUI_RefreshEvent.Add(UI_RefreshEvents.FWMANIFOLDVALUE_CHANGED)

                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessFwMANIFOLDDetailsReceived", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' SW has received and ANSFSF instruction
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 02/06/2011
        ''' </remarks>
        Private Function ProcessFwFLUIDICSDetailsReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'Dim myUtilities As New Utilities
                'Dim myInstParamTO As New InstructionParameterTO
                Dim myElements As New Dictionary(Of FW_INFO, String) 'Local structure

                ' Set Waiting Timer Current Instruction OFF
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then ClearQueueToSend()

                myGlobal = GetFWInfoData(pInstructionReceived)
                If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                    myElements = CType(myGlobal.SetDatos, Dictionary(Of FW_INFO, String))
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then

                        ServiceFWInfoTreatment(myElements)

                    Else
                        'Nothing by now
                    End If

                    'Generate UI_Refresh event FWFLUIDICSVALUE_CHANGED
                    If Not myUI_RefreshEvent.Contains(UI_RefreshEvents.FWFLUIDICSVALUE_CHANGED) Then myUI_RefreshEvent.Add(UI_RefreshEvents.FWFLUIDICSVALUE_CHANGED)

                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessFwFLUIDICSDetailsReceived", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' SW has received and ANSBXX instruction for real time monitoring
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>XBC 31/05/2011 - creation</remarks>
        Private Function ProcessHwARMDetailsReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myUtilities As New Utilities
                Dim myInstParamTO As New InstructionParameterTO
                Dim myElements As New Dictionary(Of ARMS_ELEMENTS, String) 'Local structure

                ' Set Waiting Timer Current Instruction OFF
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then ClearQueueToSend()

                'Arm Identifier
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 3)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ARMS_ELEMENTS.ID, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Board temperature
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 4)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ARMS_ELEMENTS.TMP, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Horizontal Motor
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 5)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ARMS_ELEMENTS.MH, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Home Horizontal Motor
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 6)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ARMS_ELEMENTS.MHH, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Horizontal current Position
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 7)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ARMS_ELEMENTS.MHA, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Vertical Motor
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 8)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ARMS_ELEMENTS.MV, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Home Vertical Motor
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 9)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ARMS_ELEMENTS.MVH, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Vertical current Position
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 10)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ARMS_ELEMENTS.MVA, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If


                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then

                    ServiceSwArmsInfoTreatment(myElements)

                Else
                    'Nothing by now
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessHwARMDetailsReceived", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' SW has received and ANSDXX instruction for real time monitoring
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>XBC 01/06/2011 - creation</remarks>
        Private Function ProcessHwPROBEDetailsReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myUtilities As New Utilities
                Dim myInstParamTO As New InstructionParameterTO
                Dim myElements As New Dictionary(Of PROBES_ELEMENTS, String) 'Local structure

                ' Set Waiting Timer Current Instruction OFF
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then ClearQueueToSend()

                'Identifier
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 3)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(PROBES_ELEMENTS.ID, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Board temperature
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 4)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(PROBES_ELEMENTS.TMP, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Detection status
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 5)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(PROBES_ELEMENTS.DST, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Detection Base Frequency
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 6)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(PROBES_ELEMENTS.DFQ, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Detection
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 7)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(PROBES_ELEMENTS.D, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Internal Rate of Change in last detection
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 8)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(PROBES_ELEMENTS.DCV, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Probe Thermistor Value
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 9)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(PROBES_ELEMENTS.PTH, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Probe Thermistor Diagnostic
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 10)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(PROBES_ELEMENTS.PTHD, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Probe Heater state
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 11)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(PROBES_ELEMENTS.PH, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Probe Heater Diagnostic
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 12)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(PROBES_ELEMENTS.PHD, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Collision Detector
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 13)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(PROBES_ELEMENTS.CD, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If


                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then
                    ServiceSwProbesInfoTreatment(myElements)
                Else
                    'Nothing by now
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessHwPROBEDetailsReceived", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' SW has received and ANSRXX instruction for real time monitoring
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>XBC 01/06/2011 - creation</remarks>
        Private Function ProcessHwROTORDetailsReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myUtilities As New Utilities
                Dim myInstParamTO As New InstructionParameterTO
                Dim myElements As New Dictionary(Of ROTORS_ELEMENTS, String) 'Local structure

                ' Set Waiting Timer Current Instruction OFF
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then ClearQueueToSend()

                'Identifier
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 3)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.ID, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Board temperature
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 4)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.TMP, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Rotor Motor
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 5)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.MR, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Home Motor Rotor
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 6)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.MRH, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Motor Position
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 7)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.MRA, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Fridge Thermistor Value
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 8)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.FTH, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Fridge Thermistor Diagnostic
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 9)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.FTHD, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Fridge Peltiers state
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 10)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.FH, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Fridge Peltiers Diagnostic
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 11)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.FHD, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Peltier Fan 1 Speed
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 12)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.PF1, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Peltier Fan 1 Diagnostic
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 13)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.PF1D, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Peltier Fan 2 Speed
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 14)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.PF2, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Peltier Fan 2 Diagnostic
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 15)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.PF2D, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Peltier Fan 3 Speed
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 16)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.PF3, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Peltier Fan 3 Diagnostic
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 17)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.PF3D, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Peltier Fan 4 Speed
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 18)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.PF4, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Peltier Fan 4 Diagnostic
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 19)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.PF4D, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Frame Fan 1 Speed
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 20)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.FF1, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Frame Fan 1 Diagnostic
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 21)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.FF1D, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Frame Fan 2 Speed
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 22)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.FF2, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Frame Fan 2 Diagnostic
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 23)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.FF2D, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Rotor Cover
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 24)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.RC, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Codebar Reader state
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 25)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.CB, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If

                'Codebar Reader error
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 26)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myElements.Add(ROTORS_ELEMENTS.CBE, myInstParamTO.ParameterValue.ToString)
                Else
                    Exit Try
                End If


                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then
                    ServiceSwRotorsInfoTreatment(myElements)
                Else
                    'Nothing by now
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessHwROTORDetailsReceived", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function



        ''' <summary>
        ''' SW has received ANSUTIL
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>SGM 04/10/2012</remarks>
        Public Function ProcessANSUTILReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try
                Dim myUtilities As New Utilities
                Dim myInstParamTO As New InstructionParameterTO

                Dim myUTILType As UTILInstructionTypes = UTILInstructionTypes.None
                Dim myTanksTestResult As Integer = 0 'NOT V1
                Dim myCollisionTestResult As UTILCollidedNeedles = UTILCollidedNeedles.None
                Dim mySNSavingResult As UTILSNSavedResults = UTILSNSavedResults.None

                'Action
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 3)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myUTILType = CType(CInt(myInstParamTO.ParameterValue), UTILInstructionTypes)
                Else
                    Exit Try
                End If

                'Tanks Test result
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 4)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myTanksTestResult = CType(CInt(myInstParamTO.ParameterValue), Integer)
                    If myTanksTestResult > 0 Then
                        'Activate sensor NOT V1
                        'UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.TANKS_TEST_STEP, 1, True)
                    End If
                Else
                    Exit Try
                End If

                'Collision Test result
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 5)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myCollisionTestResult = CType(CInt(myInstParamTO.ParameterValue), UTILCollidedNeedles)
                    TestingCollidedNeedleAttribute = myCollisionTestResult
                    If myCollisionTestResult <> UTILCollidedNeedles.None Then
                        'Activate sensor
                        UpdateSensorValuesAttribute(AnalyzerSensors.TESTING_NEEDLE_COLLIDED, 1, True)
                    End If
                Else
                    Exit Try
                End If

                'Save Serial number result
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 6)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    mySNSavingResult = CType(CInt(myInstParamTO.ParameterValue), UTILSNSavedResults)
                    SaveSNResultAttribute = mySNSavingResult
                    If mySNSavingResult <> UTILSNSavedResults.None Then
                        'Activate sensor
                        UpdateSensorValuesAttribute(AnalyzerSensors.SERIAL_NUMBER_SAVED, 1, True)
                    End If
                Else
                    Exit Try
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessFirmwareEventsReceived", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' Process Firmware Update Utility Instruction ANSFWU
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 25/05/2012</remarks>
        Private Function ProcessFirmwareUtilReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try
                Dim myUtilities As New Utilities
                Dim myInstParamTO As New InstructionParameterTO

                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then ClearQueueToSend()

                Dim myAction As FwUpdateActions = FwUpdateActions.None
                Dim myResult As FW_GENERIC_RESULT = FW_GENERIC_RESULT.OK
                Dim myCRC As String = ""
                Dim myCPU As FW_GENERIC_RESULT = FW_GENERIC_RESULT.OK
                Dim myPER As FW_GENERIC_RESULT = FW_GENERIC_RESULT.OK
                Dim myMAN As FW_GENERIC_RESULT = FW_GENERIC_RESULT.OK

                'Action
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 3)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myAction = CType(CInt(myInstParamTO.ParameterValue), FwUpdateActions)
                Else
                    Exit Try
                End If

                'Result
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 4)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    Dim myResultStr As String = myInstParamTO.ParameterValue
                    Select Case myResultStr
                        Case "OK" : myResult = FW_GENERIC_RESULT.OK
                        Case Else : myResult = FW_GENERIC_RESULT.KO
                    End Select
                Else
                    Exit Try
                End If

                'CRC
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 5)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    myCRC = CStr(myInstParamTO.ParameterValue)
                Else
                    Exit Try
                End If

                'CPU
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 6)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    Dim myCPUStr As String = myInstParamTO.ParameterValue
                    Select Case myCPUStr
                        Case "OK" : myCPU = FW_GENERIC_RESULT.OK
                        Case Else : myCPU = FW_GENERIC_RESULT.KO
                    End Select
                Else
                    Exit Try
                End If

                'Peripherals file
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 7)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    Dim myPERStr As String = myInstParamTO.ParameterValue
                    Select Case myPERStr
                        Case "OK" : myPER = FW_GENERIC_RESULT.OK
                        Case Else : myPER = FW_GENERIC_RESULT.KO
                    End Select
                Else
                    Exit Try
                End If

                'Maneuver file
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 8)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    Dim myMANStr As String = myInstParamTO.ParameterValue
                    Select Case myMANStr
                        Case "OK" : myMAN = FW_GENERIC_RESULT.OK
                        Case Else : myMAN = FW_GENERIC_RESULT.KO
                    End Select
                Else
                    Exit Try
                End If

                Dim myFWUpdateResponse As New FWUpdateResponseTO(myAction)
                With myFWUpdateResponse
                    .ActionResult = myResult
                    .FirmwareCRC = myCRC
                    .IsUpdatedCPU = myCPU
                    .IsUpdatedPER = myPER
                    .IsUpdatedMAN = myMAN
                End With

                myGlobal.SetDatos = myFWUpdateResponse

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessFirmwareUtilReceived", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' SGM 29/05/2012
        ''' </summary>
        ''' <param name="pFileStringData"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CalculateFwFileCRC32(ByVal pFileStringData As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'Dim myUtil As New Utilities
                Dim myFileBytes As Byte()

                myFileBytes = ASCIIEncoding.ASCII.GetBytes(pFileStringData)

                myGlobal = CalculateFwFileCRC32(myFileBytes)


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.CalculateFwFileCRC32", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' SGM 29/05/2012
        ''' </summary>
        ''' <param name="pFileBytes"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CalculateFwFileCRC32(ByVal pFileBytes As Byte()) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myUtil As New Utilities
                Dim myFileBytes As Byte()
                'Dim FWFileCRC32Hex As String

                myFileBytes = pFileBytes

                myGlobal = myUtil.CalculateCRC32(myFileBytes)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then

                    Dim myCRCResult As UInt32 = CType(myGlobal.SetDatos, UInt32)

                    If myCRCResult <> &HFFFFFFFFUI Then
                        myGlobal = myUtil.ConvertUint32ToHex(myCRCResult)
                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                            myGlobal.SetDatos = "0x" & CStr(myGlobal.SetDatos)
                        End If
                    Else

                        myGlobal.SetDatos = "0x00000000"
                    End If

                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.CalculateFwFileCRC32", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Validates compatibility between Software and Firmware
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SGM 30/05/2012</remarks>
        Public Function ValidateFwSwCompatibility(ByVal pFWVersion As String, ByVal pSWVersion As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try
                'ActiveFwVersion

                ' PENDING : is necesary the compatibility check
                ' to implement when version package table's design has been created !
                Dim isCompatible As Boolean
                Dim myVersionsDelegate As New VersionsDelegate
                Dim myVersionsDS As VersionsDS
                myGlobal = myVersionsDelegate.GetVersionsData(Nothing)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    myVersionsDS = CType(myGlobal.SetDatos, VersionsDS)
                    Dim myCompatibles As List(Of VersionsDS.tfmwVersionsRow)
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        myCompatibles = (From a In myVersionsDS.tfmwVersions Where a.Firmware = pFWVersion And a.ServiceSoftware = pSWVersion Select a).ToList()
                    Else
                        myCompatibles = (From a In myVersionsDS.tfmwVersions Where a.Firmware = pFWVersion And a.UserSoftware = pSWVersion Select a).ToList()
                    End If

                    If myCompatibles.Count > 0 Then
                        isCompatible = True
                    End If
                    myCompatibles = Nothing 'AG 02/08/2012 - free memory
                End If

                myGlobal.SetDatos = isCompatible

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ValidateFwSwCompatibility", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pSWVersion"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 22/06/2012</remarks>
        Public Function GetFwVersionNeeded(ByVal pSWVersion As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myVersionsDS As VersionsDS
                Dim myVersionsDelegate As New VersionsDelegate
                myGlobal = myVersionsDelegate.GetVersionsData(Nothing)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    myVersionsDS = CType(myGlobal.SetDatos, VersionsDS)
                    Dim myCompatibles As List(Of VersionsDS.tfmwVersionsRow)

                    ' XBC 18/09/2012 - fix : difference between Service and User Sw
                    'myCompatibles = (From a In myVersionsDS.tfmwVersions Where a.UserSoftware = pSWVersion Select a).ToList()
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        myCompatibles = (From a In myVersionsDS.tfmwVersions Where a.ServiceSoftware = pSWVersion Select a).ToList()
                    Else
                        myCompatibles = (From a In myVersionsDS.tfmwVersions Where a.UserSoftware = pSWVersion Select a).ToList()
                    End If
                    ' XBC 18/09/2012

                    If myCompatibles.Count > 0 Then
                        myGlobal.SetDatos = myCompatibles(0).Firmware
                    Else
                        myGlobal.SetDatos = "?"
                    End If
                    myCompatibles = Nothing 'AG 02/08/2012 - free memory
                End If
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.GetFwVersionNeeded", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Process Serial Number Answer Instruction ANSPSN
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by  XB 30/07/2012
        ''' Modified by AG 28/11/2013 - BT #1397 in pause mode do not activate RECOVERY_RESULTS_STATUS sensor for UI refresh at this point. Do it once the running connection process finishes
        '''             XB 09/01/2014 - Complete Functionality when Sw is connecting on Running Analyzer with not contemplated new cases ABORTED, CLOSED – Task #1445</remarks>
        Private Function ProcessSerialNumberReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myUtilities As New Utilities
                Dim myInstParamTO As New InstructionParameterTO
                Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then ClearQueueToSend()

                Dim AnalyzerID As String = ""
                Dim myWSStatus As String = ""
                Dim myLastWSAnalyzerID As String = ""

                'Serial Number
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 3)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    AnalyzerID = CStr(myInstParamTO.ParameterValue)
                Else
                    Exit Try
                End If


                ' The functionality must works either User or Service software
                Dim confAnalyzers As New AnalyzersDelegate
                Dim myAnalyzersRow As AnalyzersDS.tcfgAnalyzersRow
                Dim myAnalyzersDS As New AnalyzersDS
                myAnalyzersRow = myAnalyzersDS.tcfgAnalyzers.NewtcfgAnalyzersRow

                myAnalyzersRow.AnalyzerID = AnalyzerID
                myAnalyzersRow.AnalyzerModel = GetModelValue(AnalyzerID)
                myAnalyzersRow.FirmwareVersion = ""

                Dim myConnectedAnalyzerDS As New AnalyzersDS

                ' Get the last Analyzer ID connected with Software

                Dim myLogAccionesAux As New ApplicationLogManager()
                myLogAccionesAux.CreateLogActivity("(Analyzer Change) Check Analyzer ", "AnalyzerManager.ProcessSerialNumberReceived", EventLogEntryType.Information, False)

                myGlobal = confAnalyzers.CheckAnalyzer(Nothing, myAnalyzersRow)
                If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                    myConnectedAnalyzerDS = CType(myGlobal.SetDatos, AnalyzersDS)
                    Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate()
                    ' Get current Work Session
                    myGlobal = myWSAnalyzersDelegate.GetActiveWSByAnalyzer(Nothing)
                    If Not myGlobal.HasError Then
                        Dim myWSAnalyzersDS As New WSAnalyzersDS
                        myWSAnalyzersDS = DirectCast(myGlobal.SetDatos, WSAnalyzersDS)
                        If myWSAnalyzersDS.twksWSAnalyzers.Count > 0 Then
                            ' Get current Work Session status
                            myWSStatus = myWSAnalyzersDS.twksWSAnalyzers(0).WSStatus
                            myLastWSAnalyzerID = myWSAnalyzersDS.twksWSAnalyzers(0).AnalyzerID

                            ' Check current Analyzer ID connected with the last Analyzer ID connected
                            If myConnectedAnalyzerDS.tcfgAnalyzers.Count > 0 Then

                                Dim AbortWS As Boolean = False
                                If AnalyzerID <> myLastWSAnalyzerID Then
                                    ' Different Analyzer ID than Last connected
                                    myLogAccionesAux.CreateLogActivity("Different Analyzer ID than Last connected ! (" & AnalyzerID & ") vs (" & myLastWSAnalyzerID & ")", "AnalyzerManager.ProcessSerialNumberReceived", EventLogEntryType.Information, False)
                                    AbortWS = True
                                    DifferentWSTypeAtrr = "1"
                                Else
                                    ' Same Analyzer ID than Last connected

                                    If SessionFlag(AnalyzerManagerFlags.ReportSATonRUNNING) = "INPROCESS" Then
                                        ' Comming from ReportSAT
                                        myLogAccionesAux.CreateLogActivity("Comming from ReportSAT !", "AnalyzerManager.ProcessSerialNumberReceived", EventLogEntryType.Information, False)
                                        AbortWS = True
                                        DifferentWSTypeAtrr = "2"
                                        UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.ReportSATonRUNNING, "CLOSED")

                                        ' XB 09/01/2014 - Task #1445
                                        'ElseIf myWSStatus = "INPROCESS" AndAlso SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SoftwareWSonRUNNING) = "INPROCESS" Then
                                    ElseIf (myWSStatus = "INPROCESS" OrElse myWSStatus = "ABORTED" OrElse myWSStatus = "CLOSED") AndAlso _
                                           (SessionFlag(AnalyzerManagerFlags.SoftwareWSonRUNNING) = "INPROCESS") Then
                                        ' XB 09/01/2014

                                        myLogAccionesAux.CreateLogActivity("RECOVERY RESULTS is Comming !", "AnalyzerManager.ProcessSerialNumberReceived", EventLogEntryType.Information, False)

                                        'AG 03/09/2012 correction, this flag must be updated to closed when the recovery results starts
                                        '' current Work Session in Software data is RUNNING
                                        'UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.SoftwareWSonRUNNING, "CLOSED")

                                        ' XBC 30/07/2012: RECOVERY RESULTS !!!
                                        UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.RESULTSRECOVERProcess, "INPROCESS")
                                        endRunAlreadySentFlagAttribute = True 'AG 25/09/2012 - When resultsrecover process starts the Fw implements an automatic END, so add protection in order Sw do not sent this instruction again

                                        'AG 07/01/2013 - BT #1436 - If running connection while analyzer is entering in pause mode ... do not execute the IF code
                                        If (mySessionFlags(AnalyzerManagerFlags.PAUSEprocess.ToString) <> "INPROCESS") Then
                                            'AG 28/11/2013 - BT #1397 - Add this If. Open the recover results screen only if no pause mode
                                            If AnalyzerCurrentAction = AnalyzerManagerAx00Actions.PAUSE_END AndAlso Not AllowScanInRunningAttribute Then
                                                SetAllowScanInRunningValue(True)
                                            End If
                                            If Not AllowScanInRunningAttribute Then
                                                'AG 17/09/2012 - Activate final code for v052. Temporally commented for setup 051
                                                UpdateSensorValuesAttribute(AnalyzerSensors.RECOVERY_RESULTS_STATUS, 1, True) 'AG 27/08/2012 - Update sensors for UI refresh - recovery results starts
                                            End If

                                        Else
                                            'Means current action is 96 (PAUSE_START), Software has nothing to do until the action 97 is achieved

                                            'Exception: User has clicked the PAUSE button but the comms are lost before the PAUSE instruction has been sent
                                            'Flag indicates PAUSE is in process but the action won't be never indicate pause mode
                                            If AnalyzerCurrentAction <> AnalyzerManagerAx00Actions.PAUSE_START AndAlso AnalyzerCurrentAction <> AnalyzerManagerAx00Actions.PAUSE_END Then
                                                UpdateSensorValuesAttribute(AnalyzerSensors.RECOVERY_RESULTS_STATUS, 1, True) 'AG 27/08/2012 - Update sensors for UI refresh - recovery results starts
                                            End If
                                        End If

                                    Else
                                        ' Current Work Session in Software data is NO RUNNING (by Reset or by Load ReportSAT)
                                        myLogAccionesAux.CreateLogActivity("Current WS in Software data is NO RUNNING ! - myWSStatus = " & myWSStatus & "; Flags.ReportSATonRUNNING = " & AnalyzerManagerFlags.ReportSATonRUNNING, "AnalyzerManager.ProcessSerialNumberReceived", EventLogEntryType.Information, False)
                                        AbortWS = True
                                        DifferentWSTypeAtrr = "2"
                                    End If
                                End If


                                If AbortWS Then
                                    ForceAbortSessionAttr = True
                                End If


                            End If

                        End If
                    End If
                End If

                'Update analyzer session flags into DataBase
                If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessSerialNumberReceived", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function



#End Region

#Region "Level2 Private Methods"


        ''' <summary>
        ''' Prepare the data for the UI refreh due a cpu instruction reception
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pUI_EventType"></param>
        ''' <param name="pCpuId"></param>
        ''' <param name="pCpuValue"></param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 08/06/2011
        ''' AG 22/05/2014 - #1637 Remove old commented code + use exclusive lock (multithread protection) + AcceptChanges in the datatable with changes, not in the whole dataset
        ''' </remarks>
        Private Function PrepareUIRefreshEventCpu(ByVal pDBConnection As SqlConnection, ByVal pUI_EventType As UI_RefreshEvents, _
                                              ByVal pCpuId As CPU_ELEMENTS, ByVal pCpuValue As String) As GlobalDataTO
            Dim myglobal As New GlobalDataTO
            Dim dbConnection As New SqlConnection

            Try
                myglobal = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myglobal.HasError And Not myglobal.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myglobal.SetDatos, SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        eventDataPendingToTriggerFlag = True 'AG 07/10/2011 - exists information in UI_RefreshDS pending to be send to the event
                        If Not myUI_RefreshEvent.Contains(pUI_EventType) Then myUI_RefreshEvent.Add(pUI_EventType)

                        If pUI_EventType = UI_RefreshEvents.CPUVALUE_CHANGED Then
                            'This case prepare DS 
                            'NOTE it's not necessary due the values are available using method AnalyzerManager.GetSensorValue

                            If pCpuId <> Nothing Then
                                Dim myNewCpuChangeRow As UIRefreshDS.CPUValueChangedRow
                                SyncLock myUI_RefreshDS.CPUValueChanged 'AG 22/05/2014 #1637 - Use exclusive lock over myUI_RefreshDS variables
                                    myNewCpuChangeRow = myUI_RefreshDS.CPUValueChanged.NewCPUValueChangedRow
                                    With myNewCpuChangeRow
                                        .BeginEdit()
                                        .ElementID = pCpuId.ToString
                                        .Value = pCpuValue.ToString()
                                        .EndEdit()
                                    End With
                                    myUI_RefreshDS.CPUValueChanged.AddCPUValueChangedRow(myNewCpuChangeRow)
                                    myUI_RefreshDS.CPUValueChanged.AcceptChanges() 'AG 22/05/2014 #1637 - AcceptChanges in datatable layer instead of dataset layer
                                End SyncLock
                            End If
                        End If
                        'myUI_RefreshDS.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer

                    End If

                End If

            Catch ex As Exception
                myglobal.HasError = True
                myglobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.PrepareUIRefreshEventCpu", EventLogEntryType.Error, False)
            Finally

            End Try

            'We have used Exit Try so we have to sure the connection becomes properly closed here
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            Return myglobal
        End Function

        ''' <summary>
        ''' Prepare the data for the UI refreh due a manifold instruction reception
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pUI_EventType"></param>
        ''' <param name="pManifoldId"></param>
        ''' <param name="pManifoldValue"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 24/05/2011
        ''' AG 22/05/2014 - #1637 Remove old commented code + use exclusive lock (multithread protection) + AcceptChanges in the datatable with changes, not in the whole dataset
        ''' </remarks>
        Private Function PrepareUIRefreshEventManifold(ByVal pDBConnection As SqlConnection, ByVal pUI_EventType As UI_RefreshEvents, _
                                              ByVal pManifoldId As MANIFOLD_ELEMENTS, ByVal pManifoldValue As String) As GlobalDataTO
            Dim myglobal As New GlobalDataTO
            Dim dbConnection As New SqlConnection

            Try
                myglobal = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myglobal.HasError And Not myglobal.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myglobal.SetDatos, SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        eventDataPendingToTriggerFlag = True 'AG 07/10/2011 - exists information in UI_RefreshDS pending to be send to the event
                        If Not myUI_RefreshEvent.Contains(pUI_EventType) Then myUI_RefreshEvent.Add(pUI_EventType)

                        If pUI_EventType = UI_RefreshEvents.MANIFOLDVALUE_CHANGED Then
                            'This case prepare DS 
                            'NOTE it's not necessary due the values are available using method AnalyzerManager.GetSensorValue

                            If pManifoldId <> Nothing Then
                                Dim myNewManifoldChangeRow As UIRefreshDS.ManifoldValueChangedRow
                                'AG 22/05/2014 #1637 - Use exclusive lock over myUI_RefreshDS variables
                                SyncLock myUI_RefreshDS.ManifoldValueChanged
                                    myNewManifoldChangeRow = myUI_RefreshDS.ManifoldValueChanged.NewManifoldValueChangedRow
                                    With myNewManifoldChangeRow
                                        .BeginEdit()
                                        .ElementID = pManifoldId.ToString
                                        .Value = pManifoldValue.ToString()
                                        .EndEdit()
                                    End With
                                    myUI_RefreshDS.ManifoldValueChanged.AddManifoldValueChangedRow(myNewManifoldChangeRow)
                                    myUI_RefreshDS.ManifoldValueChanged.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer
                                End SyncLock
                            End If
                        End If
                        'myUI_RefreshDS.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer

                    End If
                End If

            Catch ex As Exception
                myglobal.HasError = True
                myglobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.PrepareUIRefreshEventManifold", EventLogEntryType.Error, False)
            Finally

            End Try

            'We have used Exit Try so we have to sure the connection becomes properly closed here
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            Return myglobal
        End Function

        ''' <summary>
        ''' Prepare the data for the UI refreh due a Fluidics instruction reception
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pUI_EventType"></param>
        ''' <param name="pFluidicsId"></param>
        ''' <param name="pFluidicsValue"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 24/05/2011
        ''' AG 22/05/2014 - #1637 Remove old commented code + use exclusive lock (multithread protection) + AcceptChanges in the datatable with changes, not in the whole dataset
        ''' </remarks>
        Private Function PrepareUIRefreshEventFluidics(ByVal pDBConnection As SqlConnection, ByVal pUI_EventType As UI_RefreshEvents, _
                                                        ByVal pFluidicsId As FLUIDICS_ELEMENTS, ByVal pFluidicsValue As String) As GlobalDataTO
            Dim myglobal As New GlobalDataTO
            Dim dbConnection As New SqlConnection

            Try
                myglobal = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myglobal.HasError And Not myglobal.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myglobal.SetDatos, SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        eventDataPendingToTriggerFlag = True 'AG 07/10/2011 - exists information in UI_RefreshDS pending to be send to the event
                        If Not myUI_RefreshEvent.Contains(pUI_EventType) Then myUI_RefreshEvent.Add(pUI_EventType)

                        If pUI_EventType = UI_RefreshEvents.FLUIDICSVALUE_CHANGED Then
                            'This case prepare DS 
                            'NOTE it's not necessary due the values are available using method AnalyzerManager.GetSensorValue

                            If pFluidicsId <> Nothing Then
                                Dim myNewFluidicsChangeRow As UIRefreshDS.FluidicsValueChangedRow
                                'AG 22/05/2014 #1637 - Use exclusive lock over myUI_RefreshDS variables
                                SyncLock myUI_RefreshDS.FluidicsValueChanged
                                    myNewFluidicsChangeRow = myUI_RefreshDS.FluidicsValueChanged.NewFluidicsValueChangedRow
                                    With myNewFluidicsChangeRow
                                        .BeginEdit()
                                        .ElementID = pFluidicsId.ToString
                                        .Value = pFluidicsValue.ToString
                                        .EndEdit()
                                    End With
                                    myUI_RefreshDS.FluidicsValueChanged.AddFluidicsValueChangedRow(myNewFluidicsChangeRow)
                                    myUI_RefreshDS.FluidicsValueChanged.AcceptChanges() 'AG 22/05/2014 #1637 - AcceptChanges in datatable layer instead of dataset layer
                                End SyncLock
                            End If
                        End If

                        'myUI_RefreshDS.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer

                    End If

                End If

            Catch ex As Exception
                myglobal.HasError = True
                myglobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.PrepareUIRefreshEventFluidics", EventLogEntryType.Error, False)
            Finally

            End Try

            'We have used Exit Try so we have to sure the connection becomes properly closed here
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            Return myglobal
        End Function

        ''' <summary>
        ''' Prepare the data for the UI refreh due a Photometrics instruction reception
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pUI_EventType"></param>
        ''' <param name="pPhotometricsId"></param>
        ''' <param name="pPhotometricsValue"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 24/05/2011
        ''' AG 22/05/2014 - #1637 Remove old commented code + use exclusive lock (multithread protection) + AcceptChanges in the datatable with changes, not in the whole dataset
        ''' </remarks>
        Private Function PrepareUIRefreshEventPhotometrics(ByVal pDBConnection As SqlConnection, ByVal pUI_EventType As UI_RefreshEvents, _
                                                        ByVal pPhotometricsId As PHOTOMETRICS_ELEMENTS, ByVal pPhotometricsValue As String) As GlobalDataTO
            Dim myglobal As New GlobalDataTO
            Dim dbConnection As New SqlConnection

            Try
                myglobal = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myglobal.HasError And Not myglobal.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myglobal.SetDatos, SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        eventDataPendingToTriggerFlag = True 'AG 07/10/2011 - exists information in UI_RefreshDS pending to be send to the event
                        If Not myUI_RefreshEvent.Contains(pUI_EventType) Then myUI_RefreshEvent.Add(pUI_EventType)

                        If pUI_EventType = UI_RefreshEvents.PHOTOMETRICSVALUE_CHANGED Then
                            'This case prepare DS 
                            'NOTE it's not necessary due the values are available using method AnalyzerManager.GetSensorValue

                            If pPhotometricsId <> Nothing Then
                                Dim myNewPhotometricsChangeRow As UIRefreshDS.PhotometricsValueChangedRow
                                'AG 22/05/2014 #1637 - Use exclusive lock over myUI_RefreshDS variables
                                SyncLock myUI_RefreshDS.PhotometricsValueChanged
                                    myNewPhotometricsChangeRow = myUI_RefreshDS.PhotometricsValueChanged.NewPhotometricsValueChangedRow
                                    With myNewPhotometricsChangeRow
                                        .BeginEdit()
                                        .ElementID = pPhotometricsId.ToString
                                        .Value = pPhotometricsValue.ToString
                                        .EndEdit()
                                    End With
                                    myUI_RefreshDS.PhotometricsValueChanged.AddPhotometricsValueChangedRow(myNewPhotometricsChangeRow)
                                    myUI_RefreshDS.PhotometricsValueChanged.AcceptChanges() 'AG 22/05/2014 #1637 - AcceptChanges in datatable layer instead of dataset layer
                                End SyncLock
                            End If
                        End If

                        'myUI_RefreshDS.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer

                    End If

                End If

            Catch ex As Exception
                myglobal.HasError = True
                myglobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.PrepareUIRefreshEventPhotometrics", EventLogEntryType.Error, False)
            Finally

            End Try

            'We have used Exit Try so we have to sure the connection becomes properly closed here
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            Return myglobal
        End Function

        ''' <summary>
        ''' Prepare the data for the UI refreh due a cycle instruction reception
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pUI_EventType"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 24/05/2011
        ''' AG 22/05/2014 - #1637 Remove old commented code + use exclusive lock (multithread protection) + AcceptChanges in the datatable with changes, not in the whole dataset
        ''' </remarks>
        Private Function PrepareUIRefreshEventCycles(ByVal pDBConnection As SqlConnection, _
                                                     ByVal pUI_EventType As UI_RefreshEvents, _
                                              ByVal pCycleItemId As CYCLE_ELEMENTS, _
                                              ByVal pCycleSubSystemID As SUBSYSTEMS, _
                                              ByVal pCycleUnits As CYCLE_UNITS, _
                                              ByVal pCyclesValue As String) As GlobalDataTO
            Dim myglobal As New GlobalDataTO
            Dim dbConnection As New SqlConnection

            Try
                myglobal = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myglobal.HasError And Not myglobal.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myglobal.SetDatos, SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        eventDataPendingToTriggerFlag = True 'AG 07/10/2011 - exists information in UI_RefreshDS pending to be send to the event
                        If Not myUI_RefreshEvent.Contains(pUI_EventType) Then myUI_RefreshEvent.Add(pUI_EventType)

                        If pUI_EventType = UI_RefreshEvents.HWCYCLES_CHANGED Then
                            'This case prepare DS 
                            'NOTE it's not necessary due the values are available using method AnalyzerManager.GetSensorValue

                            If pCycleItemId <> Nothing And pCycleSubSystemID <> Nothing And pCycleUnits <> Nothing And IsNumeric(pCyclesValue) Then
                                Dim myNewCyclesChangeRow As UIRefreshDS.CyclesValuesChangedRow
                                'AG 22/05/2014 #1637 - Use exclusive lock over myUI_RefreshDS variables
                                SyncLock myUI_RefreshDS.CyclesValuesChanged
                                    myNewCyclesChangeRow = myUI_RefreshDS.CyclesValuesChanged.NewCyclesValuesChangedRow
                                    With myNewCyclesChangeRow
                                        .BeginEdit()
                                        .ItemID = pCycleItemId.ToString
                                        .SubSystemID = pCycleSubSystemID.ToString
                                        .CycleUnits = pCycleUnits.ToString
                                        .CyclesCount = CLng(pCyclesValue)
                                        .EndEdit()
                                    End With
                                    myUI_RefreshDS.CyclesValuesChanged.AddCyclesValuesChangedRow(myNewCyclesChangeRow)
                                    myUI_RefreshDS.CyclesValuesChanged.AcceptChanges() 'AG 22/05/2014 #1637 - AcceptChanges in datatable layer instead of dataset layer
                                End SyncLock
                            End If
                        End If

                        'myUI_RefreshDS.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer

                    End If

                End If

            Catch ex As Exception
                myglobal.HasError = True
                myglobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.PrepareUIRefreshEventCycles", EventLogEntryType.Error, False)
            Finally

            End Try

            'We have used Exit Try so we have to sure the connection becomes properly closed here
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            Return myglobal
        End Function

#End Region

    End Class

End Namespace

