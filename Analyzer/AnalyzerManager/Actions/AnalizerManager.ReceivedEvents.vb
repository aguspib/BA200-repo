Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.Core.Entities
    Partial Public Class AnalyzerManager
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function StatusReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.STATUS_RECEIVED
            Dim statusRecived = New ProcessStatusReceived(Me, pInstructionReceived)
            Return statusRecived.InstructionProcessing()
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsErrReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As GlobalDataTO

            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSERR_RECEIVED
            myGlobal = ProcessHwAlarmDetailsReceived(pInstructionReceived)

            'SGM 26/10/2012 - force Auto ANSINF
            If GlobalBase.IsServiceAssembly Then
                IsAlarmInfoRequested = False
                If AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then
                    myGlobal = ManageAnalyzer(AnalyzerManagerSwActionList.INFO, True, Nothing, Ax00InfoInstructionModes.STR)
                End If
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsInfReceivedEvent(ByVal myGlobal As GlobalDataTO, ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO

            If String.Compare(mySessionFlags(AnalyzerManagerFlags.CONNECTprocess.ToString), "INPROCESS", False) <> 0 Then
                InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSINF_RECEIVED
                myGlobal = ProcessInformationStatusReceived(pInstructionReceived)
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ReadingsReceivedEvent(ByVal myGlobal As GlobalDataTO, ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO

            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.READINGS_RECEIVED
            If mySessionFlags(AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString) <> "INPROCESS" Then
                myGlobal = ProcessReadingsReceived(pInstructionReceived)
            Else
                bufferInstructionsRESULTSRECOVERYProcess.Add(pInstructionReceived)
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function BaseLineReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As GlobalDataTO

            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.BASELINE_RECEIVED
            If GlobalBase.IsServiceAssembly Then
                myGlobal = ProcessBaseLineReceived_SRV(pInstructionReceived)
            Else
                myGlobal = ProcessBaseLineReceived(pInstructionReceived)
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsFbldReceivedEvent(ByVal myGlobal As GlobalDataTO, ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO

            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSFBLD_RECEIVED
            If Not GlobalBase.IsServiceAssembly Then
                myGlobal = ProcessANSFBLDReceived(pInstructionReceived)
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsCbrReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As GlobalDataTO

            Dim myBarCodeRotorTypeRead As String = ""
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSCBR_RECEIVED
            myGlobal = ProcessCodeBarInstructionReceived(pInstructionReceived, myBarCodeRotorTypeRead)

            If Not myGlobal.HasError Then
                If Not GlobalBase.IsServiceAssembly Then
                    If BarCodeBeforeRunningProcessStatusAttribute = BarcodeWorksessionActionsEnum.NO_RUNNING_REQUEST Then 'Barcode read requests from RotorPosition Screen
                        If myBarcodeRequestDS.barCodeRequests.Rows.Count > 1 Then
                            myBarcodeRequestDS.barCodeRequests(0).Delete() 'Remove the 1st row (already sent and with results treated)
                            myBarcodeRequestDS.AcceptChanges()
                            myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.BARCODE_REQUEST, myBarcodeRequestDS)
                        Else
                            myBarcodeRequestDS.Clear()
                            BarCodeBeforeRunningProcessStatusAttribute = BarcodeWorksessionActionsEnum.BARCODE_AVAILABLE 'Barcode free for more work

                            'When barcode finish reading the sample rotor ... Sw inform if some critical warnings exists
                            If myBarCodeRotorTypeRead = "SAMPLES" Then
                                Dim dlgBarcode As New BarcodeWSDelegate
                                myGlobal = dlgBarcode.ExistBarcodeCriticalWarnings(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, "SAMPLES")
                                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                    If CType(myGlobal.SetDatos, Boolean) = True Then 'No critical errors ... Sw can send the go to running instruction
                                        UpdateSensorValuesAttribute(AnalyzerSensors.BARCODE_WARNINGS, 1, True) 'Samples barcode warnings
                                    End If
                                End If
                            End If
                        End If
                    ElseIf BarCodeBeforeRunningProcessStatusAttribute <> BarcodeWorksessionActionsEnum.ENTER_RUNNING Then 'Barcode read requests from START or CONTINUE worksession
                        myGlobal = ManageBarCodeRequestBeforeRUNNING(Nothing, BarCodeBeforeRunningProcessStatusAttribute)

                    End If
                End If
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function IseResultReceivedEvent(ByVal myGlobal As GlobalDataTO, ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO

            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ISE_RESULT_RECEIVED

            If (mySessionFlags(AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString) <> "INPROCESS") Or _
               (mySessionFlags(AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString) = "INPROCESS" AndAlso _
                mySessionFlags(AnalyzerManagerFlags.ResRecoverISE.ToString) = "END") Then

                If ISEAnalyzer.CurrentProcedure <> ISEManager.ISEProcedures.None OrElse AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING Then
                    myGlobal = ProcessRecivedISEResult(pInstructionReceived)
                    myGlobal = ProcessISEManagerProcedures()
                End If
            Else
                bufferInstructionsRESULTSRECOVERYProcess.Add(pInstructionReceived)
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ArmStatusReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ARM_STATUS_RECEIVED
            Dim statusReceived = New ProcessArmStatusRecived(Me, pInstructionReceived)
            Return statusReceived.InstructionProcessing()

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsPsnReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSPSN_RECEIVED
            Return ProcessSerialNumberReceived(pInstructionReceived)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsTinReceivedEvent(ByVal myGlobal As GlobalDataTO, ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO

            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSTIN_RECEIVED
            If String.Compare(mySessionFlags(AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString), "INPROCESS", False) <> 0 Then
                myGlobal = ProcessANSTINInstructionReceived(pInstructionReceived)
            Else
                bufferInstructionsRESULTSRECOVERYProcess.Add(pInstructionReceived)
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsPrdReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As GlobalDataTO

            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSPRD_RECEIVED
            myGlobal = ProcessANSPRDReceived(pInstructionReceived)
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CommandReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.COMMAND_RECEIVED
            Return ProcessFwCommandAnswerReceived(pInstructionReceived)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AdjustmentsReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ADJUSTMENTS_RECEIVED
            Return ProcessFwAdjustmentsReceived(pInstructionReceived)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CyclesReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.CYCLES_RECEIVED
            Return PDT_ProcessHwCyclesReceived(pInstructionReceived)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsSdmReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSSDM
            Return ProcessStressModeReceived_SRV(pInstructionReceived)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsCpuReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSCPU_RECEIVED
            Return ProcessHwCpuStatusReceived(pInstructionReceived)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsJexReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSJEX_RECEIVED
            Return ProcessHwManifoldStatusReceived(pInstructionReceived)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsSfxReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSSFX_RECEIVED
            Return ProcessHwFluidicsStatusReceived(pInstructionReceived)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsGlfReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSGLF_RECEIVED
            Return ProcessHwPhotometricsStatusReceived(pInstructionReceived)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsFwuReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSFWU_RECEIVED
            Dim myGlobal As GlobalDataTO = ProcessFirmwareUtilReceived(pInstructionReceived)

            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                FWUpdateResponseData = CType(myGlobal.SetDatos, FWUpdateResponseTO)
                UpdateSensorValuesAttribute(AnalyzerSensors.FW_UPDATE_UTIL_RECEIVED, 1, True)
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsUtilReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSUTIL_RECEIVED
            Return ProcessANSUTILReceived(pInstructionReceived)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsRxxReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSRXX_RECEIVED
            Return ProcessHwROTORDetailsReceived(pInstructionReceived)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsDxxReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSDXX_RECEIVED
            Return ProcessHwPROBEDetailsReceived(pInstructionReceived)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsBxxReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSBXX_RECEIVED
            Return ProcessHwARMDetailsReceived(pInstructionReceived)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsFsfReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSFSF_RECEIVED
            Return ProcessFwFLUIDICSDetailsReceived(pInstructionReceived)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsFjeReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSFJE_RECEIVED
            Return ProcessFwMANIFOLDDetailsReceived(pInstructionReceived)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsFglReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSFGL_RECEIVED
            Return ProcessFwPHOTOMETRICDetailsReceived(pInstructionReceived)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsFrxReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSFRX_RECEIVED
            Return ProcessFwROTORDetailsReceived(pInstructionReceived)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsFdxReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSFDX_RECEIVED
            Return ProcessFwPROBEDetailsReceived(pInstructionReceived)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsFbxReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSFBX_RECEIVED
            Return ProcessFwARMDetailsReceived(pInstructionReceived)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AnsFcpReceivedEvent(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSFCP_RECEIVED
            Return ProcessFwCPUDetailsReceived(pInstructionReceived)
        End Function
    End Class
End Namespace

