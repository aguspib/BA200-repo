Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.CommunicationsSwFw

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by TR
    ''' Modified by AG 30/04/2010: The 2 firts parametes have only value, not tag!
    ''' Modified by AG 17/05/2010: Move each InstructionType to and specific private method!!
    ''' Modified by AG 01/03/2011: Changes in instruction definition Excel Instructions rev9 - 10
    ''' </remarks>
    Public Class InstructionTypesList

#Region "Public methods"

        Public Function GetInstructionParameterList() As List(Of InstructionParameterTO)
            Dim Instructions As New List(Of InstructionParameterTO)
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'TEST INSTRUCTION (Sw -> Fw)
                GetTESTInstruction(Instructions)

                'STATUS INSTRUCTION (Fw -> Sw)
                GetSTATUSInstruction(Instructions)

                'SHORT INSTRUCTIONS  (Sw -> Fw)
                GetSHORTInstructions(Instructions)

                'AG 17/05/2010
                'BASELINES INSTRUCTIONS (Sw -> Fw): ALIGHT, BLIGHT, DLIGHT
                GetBASELINEInstruction(Instructions)

                'ANSPHR (optical readings and base line by well) INSTRUCTION (readings results)  (Fw -> Sw)
                GetANSPHRInstruction(Instructions)

                'ANSBLD INTRUCTION (ANSWER to ALIGHT) (Fw -> Sw)
                GetANSBLDInstruction(Instructions)

                'AG 01/03/2011 - removed instructions
                ''ANSBL INTRUCTION (ANSWER to BLIGHT) (Fw -> Sw)
                'GetANSBLInstruction(Instructions)
                '
                ''ANSDL INTRUCTION (ANSWER to DLIGHT) (Fw -> Sw)
                'GetANSDLInstruction(Instructions)
                ''END AG 17/05/2010

                'TR 03/01/2011 -ISE MODULE RESULTS
                GetANSISEInstruction(Instructions)
                'TR 03/01/2011 -END 

                'AG 28/12/2010
                'ISETEST INSTRUCTION (Sw -> Fw) 
                GetISETESTInstruction(Instructions)

                'PREDILUTION TEST INSTRUCTION (Sw -> Fw) 
                GetPTESTInstruction(Instructions)

                'WASH INSTRUCTION (Sw -> Fw) (standby WASH) 
                GetWASHInstruction(Instructions)

                'WRUN INSTRUCTION (Sw -> Fw) (running WASH) 
                GetWRUNInstruction(Instructions)
                'AG 28/12/2010

                'WSCTRL INSTRUCTION (Sw -> Fw) (Washing Station Control) 
                GetWSCTRLInstruction(Instructions)

                'TR 16/03/2011 (ANSARM)
                GetANSArmsInstruction(Instructions)

                'AG 07/04/2011 - INFO, ANSINF, ANSERR
                GetDetailsINFOOrAlarmInstructions(Instructions)

                'AG 21/06/2011 - CODEBR, ANSCBR
                GetCodeBarInstructions(Instructions)

                'AG 23/11/2011 - CONFIG
                GetCONFIGInstruction(Instructions)

                '''''''''''''''''''''
                'SERVICE INSTRUCTIONS
                '''''''''''''''''''''
                ' XBC 03/05/2011
                'ANSCMD INSTRUCTION (Fw -> Sw)
                GetANSCMDInstruction(Instructions)


                ' XBC 17/05/2011 - delete redundant Events replaced using AnalyzerManager.ReceptionEvent
                '' SGM 28/02/2011 - Pending to define PDT !!!
                ''SENSORS_DATA_RECEPTION INSTRUCTION (Fw -> Sw)
                'GetSENSORS_RECEIVEDInstruction(Instructions)
                '' SGM 28/02/2011 - Pending to define PDT !!!
                ''SENSORS_DATA_RECEPTION INSTRUCTION (Fw -> Sw)
                'GetTEST_EMPTY_LC_OKInstruction(Instructions)
                'GetTEST_FILL_DW_OKInstruction(Instructions)
                'GetTEST_TRANSFER_DWLC_OKInstruction(Instructions)
                '' SGM 14/03/2011 - Pending to define PDT !!!
                'GetABSORBANCE_RECEIVEDInstruction(Instructions)
                ' XBC 17/05/2011 - delete redundant Events replaced using AnalyzerManager.ReceptionEvent

                ' XBC 03/05/2011
                GetDetailsADJInstructions(Instructions)

                ' XBC 06/05/2011
                GetDetailsPOLLHWInstruction(Instructions)

                ' XBC 18/05/2011 - TANKS TEST
                GetTANKSTESTInstructions(Instructions)

                ' XBC 23/05/2011 - STRESS/DEMO MODEs
                GetDetailsSTRESSInstructions(Instructions)

                ' XBC 25/05/2011
                GetDetailsPOLLFWInstruction(Instructions)

                'SGM 10/06/2011
                GetDetailsFWEVENTSInstructions(Instructions)

                ' SGM 01/07/2011
                GetDetailsRESETInstructions(Instructions)

                ' SGM 05/07/2011
                GetDetailsUPDATEFWInstructions(Instructions)

                ' XBC 18/10/2011
                GetSOUNDInstruction(Instructions)

                ' SGM 12/12/2011
                'ISECMD INSTRUCTION (Sw -> Fw)
                GetISECommandInstructions(Instructions)

                'SGM 29/05/2012
                GetFWUtilInstructions(Instructions)

                'XBC 04/06/2012
                GetUTILInstruction(Instructions)

                'SGM 11/10/2012
                GetANSUTILInstruction(Instructions)

                'Recovery Results instructions
                GetDetailsPOLLSNInstruction(Instructions) ' XBC 30/07/2012
                GetDetailsPOLLRDInstruction(Instructions) 'AG 31/07/2012

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetInstructionParameterList", EventLogEntryType.Error, False)
            End Try
            Return Instructions
        End Function

#End Region

#Region "Private methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by AG 17/05/2010
        ''' </remarks>
        Private Sub GetTESTInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'TEST INSTRUCTION (Sw -> Fw)
                ''''''''''''''''''''''''''''
                myInstructionTO.InstructionType = "TEST"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "TEST"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "TEST"
                myInstructionTO.Parameter = "TI"
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "TEST"
                myInstructionTO.Parameter = "ID"
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "TEST"
                myInstructionTO.Parameter = "M1"
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "TEST"
                myInstructionTO.Parameter = "TM1"
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "TEST"
                myInstructionTO.Parameter = "RM1"
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "TEST"
                myInstructionTO.Parameter = "R1"
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "TEST"
                myInstructionTO.Parameter = "TR1"
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "TEST"
                myInstructionTO.Parameter = "RR1"
                myInstructionTO.ParameterIndex = 10
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "TEST"
                myInstructionTO.Parameter = "R2"
                myInstructionTO.ParameterIndex = 11
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "TEST"
                myInstructionTO.Parameter = "TR2"
                myInstructionTO.ParameterIndex = 12
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "TEST"
                myInstructionTO.Parameter = "RR2"
                myInstructionTO.ParameterIndex = 13
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "TEST"
                myInstructionTO.Parameter = "VM1"
                myInstructionTO.ParameterIndex = 14
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "TEST"
                myInstructionTO.Parameter = "VR1"
                myInstructionTO.ParameterIndex = 15
                Instructions.Add(myInstructionTO)
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "TEST"
                myInstructionTO.Parameter = "VR2"
                myInstructionTO.ParameterIndex = 16
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "TEST"
                myInstructionTO.Parameter = "OW" 'MW changes to OW --- filter used in odd cycles 1, 3, 5, ..., 67
                myInstructionTO.ParameterIndex = 17
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "TEST"
                myInstructionTO.Parameter = "EW" 'RW changes to EW --- filter used in even cycles 2, 4, 6, ..., 68
                myInstructionTO.ParameterIndex = 18
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "TEST"
                myInstructionTO.Parameter = "RN"
                myInstructionTO.ParameterIndex = 19
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetTESTInstruction", EventLogEntryType.Error, False)

            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by AG 17/05/2010
        ''' </remarks>
        Private Sub GetSTATUSInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'STATUS INSTRUCTION (Fw -> Sw)
                ''''''''''''''''''''''''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "STATUS"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "STATUS"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "STATUS"
                myInstructionTO.Parameter = "S"
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "STATUS"
                myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "STATUS"
                myInstructionTO.Parameter = "T"
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "STATUS"
                myInstructionTO.Parameter = "C"
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "STATUS"
                myInstructionTO.Parameter = "W"
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "STATUS"
                myInstructionTO.Parameter = "R"
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "STATUS"
                myInstructionTO.Parameter = "E"
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                'AG 23/11/2011 - Ise working (0 No work, ISE ready to work, 1 ISE working, not ready) - Instructions excel Rev37
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "STATUS"
                myInstructionTO.Parameter = "I"
                myInstructionTO.ParameterIndex = 10
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetSTATUSInstruction", EventLogEntryType.Error, False)

            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by  AG 17/05/2010
        ''' Modified by XB 10/10/2013 - Add PAUSE instruction - BT #1317
        ''' </remarks>
        Private Sub GetSHORTInstructions(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'CONNECT
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CONNECT"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CONNECT"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                'SLEEP: "A"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "SLEEP"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                '"TYPE"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "SLEEP"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                'STANDBY: "A"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "STANDBY"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                '"TYPE"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "STANDBY"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                'SKIP: "A"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "SKIP"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                '"TYPE"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "SKIP"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                'RUNNING: "A"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "RUNNING"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                '"TYPE"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "RUNNING"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                'ENDRUN: "A"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ENDRUN"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                '"TYPE"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ENDRUN"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                'NROTOR: "A"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "NROTOR"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                '"TYPE"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "NROTOR"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                'ABORT: "A"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ABORT"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                '"TYPE"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ABORT"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                ' XBC 18/10/2011
                ''SOUND: "A"
                'myInstructionTO = New InstructionParameterTO
                'myInstructionTO.InstructionType = "SOUND"
                'myInstructionTO.Parameter = ""
                'myInstructionTO.ParameterIndex = 1
                'Instructions.Add(myInstructionTO)

                ''"TYPE"
                'myInstructionTO = New InstructionParameterTO
                'myInstructionTO.InstructionType = "SOUND"
                'myInstructionTO.Parameter = ""
                'myInstructionTO.ParameterIndex = 2
                'Instructions.Add(myInstructionTO)

                ''ENDSOUND: "A"
                'myInstructionTO = New InstructionParameterTO
                'myInstructionTO.InstructionType = "ENDSOUND"
                'myInstructionTO.Parameter = ""
                'myInstructionTO.ParameterIndex = 1
                'Instructions.Add(myInstructionTO)

                ''"TYPE"
                'myInstructionTO = New InstructionParameterTO
                'myInstructionTO.InstructionType = "ENDSOUND"
                'myInstructionTO.Parameter = ""
                'myInstructionTO.ParameterIndex = 2
                'Instructions.Add(myInstructionTO)

                'RESRECOVER: "A"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "RESRECOVER"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                '"TYPE"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "RESRECOVER"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                'ASKSTATUS: "A"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "STATE" 'AG 25/10/2010 "ASKSTATUS"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                '"TYPE"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "STATE" 'AG 25/10/2010 "ASKSTATUS"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)
                'END AG 06/05/2010

                'START: "A"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "START"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                '"TYPE"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "START"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                'RECOVER: "A"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "RECOVER"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                '"TYPE"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "RECOVER"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                'PAUSE: "A"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PAUSE"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                '"TYPE"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PAUSE"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetSHORTInstructions", EventLogEntryType.Error, False)

            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by AG 17/05/2010
        ''' Modified: AG 01/03/2011 - Remove BLIGHT and DLIGHT
        '''           TR 03/03/2011 - Add Well (W)
        ''' Modified : XBC 20/04/2011 - Recover BLIGHT for Service Sw
        ''' </remarks>
        Private Sub GetBASELINEInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'ALIGHT INSTRUCTION (Sw -> Fw) adjustment of IT and DAC
                ''''''''''''''''''''''''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ALIGHT"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ALIGHT"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ALIGHT"
                myInstructionTO.Parameter = "W" 'well
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ALIGHT"
                myInstructionTO.Parameter = "AF" 'Autofill (always 1 for user sw)
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                'AG 01/03/2011 - removed instructions
                'XBC 20/04/2011 - revovered 
                'BLIGHT INSTRUCTION (Sw -> Fw) light baseline by well
                ''''''''''''''''''''''''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "BLIGHT"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "BLIGHT"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "BLIGHT"
                myInstructionTO.Parameter = "W" 'well
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "BLIGHT"
                myInstructionTO.Parameter = "AF" 'Auto Fill
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)


                ''DLIGHT INSTRUCTION (Sw -> Fw) darkness baseline by well
                '''''''''''''''''''''''''''''''
                'myInstructionTO = New InstructionParameterTO
                'myInstructionTO.InstructionType = "DLIGHT"
                'myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                'myInstructionTO.ParameterIndex = 1
                'Instructions.Add(myInstructionTO)

                'myInstructionTO = New InstructionParameterTO
                'myInstructionTO.InstructionType = "DLIGHT"
                'myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
                'myInstructionTO.ParameterIndex = 2
                'Instructions.Add(myInstructionTO)

                'myInstructionTO = New InstructionParameterTO
                'myInstructionTO.InstructionType = "DLIGHT"
                'myInstructionTO.Parameter = "W" 'well
                'myInstructionTO.ParameterIndex = 3
                'Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetBASELINEInstruction", EventLogEntryType.Error, False)

            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by:  AG 17/05/2010
        ''' Modified by: AG 01/03/2011 - Changes in instruction definition Excel Instruction Rev9
        '''              AG 20/04/2011 - Changes in instruction definition Excel Instruction Rev23)
        '''              TR 10/10/2013 - BT #1319 ==> Changes to add new field ST (Status of photometric Readings: 0=Normal Reading cycle / 1=Pause reading cycle) 
        '''                              to the list of ANSPHR Parameters to return (using ParameterIndex = 47).
        ''' </remarks>
        Private Sub GetANSPHRInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'READ INSTRUCTION (Fw -> Sw)
                ''''''''''''''''''''''''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSPHR"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSPHR"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSPHR"
                myInstructionTO.Parameter = "BLW"    'NEW!!! Base line Well
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                'IMPORTANT NOTE: On receive a ANSPHR instruction the following fields are repeated 11 times
                ''''''''''''''''''''
                'TR 02/03/2011 -Add for to repeat the led position info.
                For i As Integer = 4 To 44
                    myInstructionTO = New InstructionParameterTO
                    myInstructionTO.InstructionType = "ANSPHR"
                    myInstructionTO.Parameter = "P"    'NEW!!! Led Position
                    myInstructionTO.ParameterIndex = i
                    Instructions.Add(myInstructionTO)
                    i += 1 'increase to next index
                    myInstructionTO = New InstructionParameterTO
                    myInstructionTO.InstructionType = "ANSPHR"
                    myInstructionTO.Parameter = "MBL"    'NEW!!! Main Base Line
                    myInstructionTO.ParameterIndex = i
                    Instructions.Add(myInstructionTO)
                    i += 1 'increase to next index
                    myInstructionTO = New InstructionParameterTO
                    myInstructionTO.InstructionType = "ANSPHR"
                    myInstructionTO.Parameter = "RBL"    'NEW!!! Reference Base Line
                    myInstructionTO.ParameterIndex = i
                    Instructions.Add(myInstructionTO)
                Next
                'TR 02/03/2011 -END 
                'END IMPORTANT NOTE: On receive a ANSPHR instruction the following fields are repeated 11 times
                ''''''''''''''''''''

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSPHR"
                myInstructionTO.Parameter = "TR"    'Total readings in instruction 0 to 68
                myInstructionTO.ParameterIndex = 46
                Instructions.Add(myInstructionTO)


                'TR 10/10/2013 -new parameter ST 
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSPHR"
                myInstructionTO.Parameter = "ST" 'Status of photometric Readings (0 Normal Reading cycle; 1 Pause reading cycle)
                myInstructionTO.ParameterIndex = 47
                Instructions.Add(myInstructionTO)
                'TR 10/10/2013 -END

                'IMPORTANT NOTE: On receive a ANSPHR instruction the following fields are repeated TR value times!!!!
                ''''''''''''''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSPHR"
                myInstructionTO.Parameter = "RN"    'read number
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSPHR"
                myInstructionTO.Parameter = "ID" 'Preparation identifier
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSPHR"
                myInstructionTO.Parameter = "W" 'Well used
                myInstructionTO.ParameterIndex = 10
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSPHR"
                myInstructionTO.Parameter = "N"    'NEW!!! LED position (for bichromatics)
                myInstructionTO.ParameterIndex = 11
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSPHR"
                myInstructionTO.Parameter = "MC"    'Main diode counts
                myInstructionTO.ParameterIndex = 12
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSPHR"
                myInstructionTO.Parameter = "RC"    'Reference diode counts
                myInstructionTO.ParameterIndex = 13
                Instructions.Add(myInstructionTO)
                'END IMPORTANT NOTE: On receive a ANSPHR instruction the following fields are repeated TR value times!!!!
                ''''''''''''''''''''

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetANSPHRInstruction", EventLogEntryType.Error, False)

            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by AG 17/05/2010
        ''' Ag 01/03/2011 - rename ANSAL -> ANSBLD and change fields (Excel instructions rev10)
        ''' </remarks>
        Private Sub GetANSBLDInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'ANSAL INSTRUCTION (Fw -> Sw) (answer alight <autoadjustment of DAC and TI>!!)
                ''''''''''''''''''''''''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBLD"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBLD"
                'AG 30/04/2010 - Instruction type
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBLD"
                myInstructionTO.Parameter = "BLW"    'Well where the adjustment light has been performed
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                'IMPORTANT NOTE: On receive an ANSbLD instruction the following fields are repeeat for each diode (11 items)
                ''''''''''''''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBLD"
                myInstructionTO.Parameter = "P" 'diode position
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBLD"
                myInstructionTO.Parameter = "MP"    'main diode ligth counts
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBLD"
                myInstructionTO.Parameter = "RP"    'reference diode ligth counts
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBLD"
                myInstructionTO.Parameter = "MD"    'main diode dark counts
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBLD"
                myInstructionTO.Parameter = "RD" 'reference diode dark counts
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBLD"
                myInstructionTO.Parameter = "IT" 'diode integration time
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBLD"
                myInstructionTO.Parameter = "DAC" 'diode DAC
                myInstructionTO.ParameterIndex = 10
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetANSBLDInstruction", EventLogEntryType.Error, False)

            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by AG 17/05/2010
        ''' </remarks>
        Private Sub GetANSBLInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'ANSBL INSTRUCTION (Fw -> Sw) (answer blight <light baseline without adjustment>)
                ''''''''''''''''''''''''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBL"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBL"
                'AG 30/04/2010 - Instruction type
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBL"
                myInstructionTO.Parameter = "W"    'Well where the Blight has been performed
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                'IMPORTANT NOTE: On receive an ANSAL instruction the following fields are repeeat for each diode
                ''''''''''''''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBL"
                myInstructionTO.Parameter = "P" 'diode position
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBL"
                myInstructionTO.Parameter = "ML"    'main diode ligth counts
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBL"
                myInstructionTO.Parameter = "RL"    'reference diode ligth counts
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetANSBLInstruction", EventLogEntryType.Error, False)

            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by AG 17/05/2010
        ''' </remarks>
        Private Sub GetANSDLInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'ANSDL INSTRUCTION (Fw -> Sw) (answer dlight <darkness baseline without adjustment>)
                ''''''''''''''''''''''''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSDL"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSDL"
                'AG 30/04/2010 - Instruction type
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSDL"
                myInstructionTO.Parameter = "W"    'Well where the Blight has been performed
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                'IMPORTANT NOTE: On receive an ANSAL instruction the following fields are repeeat for each diode
                ''''''''''''''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSDL"
                myInstructionTO.Parameter = "P" 'diode position
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSDL"
                myInstructionTO.Parameter = "MD"    'main diode dark counts
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSDL"
                myInstructionTO.Parameter = "RD"    'reference diode dark counts
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetANSDLInstruction", EventLogEntryType.Error, False)

            End Try
        End Sub

        ''' <summary>
        ''' Instruction for the ISE module
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' CREATE BY: TR 03/01/2010
        ''' </remarks>
        Private Sub GetANSISEInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSISE"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSISE"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSISE"
                myInstructionTO.Parameter = "P" 'Preparation ID
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSISE"
                myInstructionTO.Parameter = "R"    'ISE Module result
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetANSISEInstruction", EventLogEntryType.Error, False)

            End Try
        End Sub

        ''' <summary>
        ''' Low level Instruction recived from Analyzer
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by XBC 16/11/2010
        ''' Modified by XBC 04/05/2011 - Command Instruction already defined
        ''' </remarks>
        Private Sub GetANSCMDInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'ANSCMD INSTRUCTION (Fw -> Sw)
                ''''''''''''''''''''''''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCMD"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCMD"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCMD"
                myInstructionTO.Parameter = "END"
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCMD"
                myInstructionTO.Parameter = "MP"    ' Main Photodiode reading counts
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCMD"
                myInstructionTO.Parameter = "RP"    ' Ref Photodiode reading counts
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                'PENDING TO SPEC
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCMD"
                myInstructionTO.Parameter = "ENC"    ' Encoder
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                'PENDING TO SPEC
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCMD"
                myInstructionTO.Parameter = "LD"    ' Level detection
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetANSCMDInstruction", EventLogEntryType.Error, False)
            End Try
        End Sub

        ' XBC 17/05/2011 - delete redundant Events replaced using AnalyzerManager.ReceptionEvent
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <param name="Instructions"></param>
        '''' <remarks>
        '''' Created by SGM 14/03/11
        '''' </remarks>
        'Private Sub GetABSORBANCE_RECEIVEDInstruction(ByRef Instructions As List(Of InstructionParameterTO))
        '    Try
        '        Dim myInstructionTO As New InstructionParameterTO

        '        'STATUS INSTRUCTION (Fw -> Sw)
        '        ''''''''''''''''''''''''''''''
        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "ABSORBANCE_RECEIVED"
        '        myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
        '        myInstructionTO.ParameterIndex = 1
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "ABSORBANCE_RECEIVED"
        '        myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
        '        myInstructionTO.ParameterIndex = 2
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "ABSORBANCE_RECEIVED"
        '        myInstructionTO.Parameter = "ABSORBANCE"
        '        myInstructionTO.ParameterIndex = 3
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "ABSORBANCE_RECEIVED"
        '        myInstructionTO.Parameter = "T"
        '        myInstructionTO.ParameterIndex = 4
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "ABSORBANCE_RECEIVED"
        '        myInstructionTO.Parameter = "D"
        '        myInstructionTO.ParameterIndex = 5
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "ABSORBANCE_RECEIVED"
        '        myInstructionTO.Parameter = "E"
        '        myInstructionTO.ParameterIndex = 6
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "ABSORBANCE_RECEIVED"
        '        myInstructionTO.Parameter = "F"
        '        myInstructionTO.ParameterIndex = 7
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "ABSORBANCE_RECEIVED"
        '        myInstructionTO.Parameter = "G"
        '        myInstructionTO.ParameterIndex = 8
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "ABSORBANCE_RECEIVED"
        '        myInstructionTO.Parameter = "H"
        '        myInstructionTO.ParameterIndex = 9
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "ABSORBANCE_RECEIVED"
        '        myInstructionTO.Parameter = "AC"
        '        myInstructionTO.ParameterIndex = 10
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "ABSORBANCE_RECEIVED"
        '        myInstructionTO.Parameter = "T"
        '        myInstructionTO.ParameterIndex = 11
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "SENSORS_RECEIVED"
        '        myInstructionTO.Parameter = "R"
        '        myInstructionTO.ParameterIndex = 12
        '        Instructions.Add(myInstructionTO)


        '    Catch ex As Exception
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetABSORBANCE_RECEIVEDInstruction", EventLogEntryType.Error, False)

        '    End Try
        'End Sub

        ''' <summary>
        ''' READADJ, ANSADJ, LOADADJ instruction definition
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' XBC 03/05/2011 - Creation
        ''' Modified by XBC : 30/09/2011 - Add MasterData feature
        ''' </remarks>
        Private Sub GetDetailsADJInstructions(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                ''''''''
                'READADJ - Ask for Adjustments info - Sw -> Fw
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "READADJ"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "READADJ"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "READADJ"
                myInstructionTO.Parameter = "Q" 'Query
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                ''''''''
                'ANSADJ - Information Instrument's Adjustments response - Fw -> Sw
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSADJ"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSADJ"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                Dim myGlobal As New GlobalDataTO
                Dim myFwAdjustmentsDS As New SRVAdjustmentsDS
                Dim myAdjustmentsDelegate As New DBAdjustmentsDelegate
                myGlobal = myAdjustmentsDelegate.ReadAdjustmentsFromDB(Nothing, "MasterData") ' take struct contents of adjustments by default
                If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                    myFwAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                    If myFwAdjustmentsDS IsNot Nothing Then
                        'Dim myRes As New List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
                        Dim i As Integer = 3
                        For Each item As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myFwAdjustmentsDS.srv_tfmwAdjustments.Rows

                            myInstructionTO = New InstructionParameterTO
                            myInstructionTO.InstructionType = "ANSADJ"
                            myInstructionTO.Parameter = item.CodeFw
                            myInstructionTO.ParameterIndex = i
                            Instructions.Add(myInstructionTO)
                            i += 1
                        Next

                    End If
                End If

                ''''''''
                'LOADADJ - Load/Save Adjustments info into Instrument - Sw -> Fw
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "LOADADJ"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "LOADADJ"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "LOADADJ"
                myInstructionTO.Parameter = "" ' x NumParams
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                'GetDetailsLOADFACTORYADJInstructions INSTRUCTION (Sw -> Fw)      PDT TO SPECIFIED !!!
                ''''''''''''''''''''''''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "LOADFACTORYADJ"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "LOADFACTORYADJ"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetDetailsADJInstructions", EventLogEntryType.Error, False)

            End Try
        End Sub


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by AG 28/12/2010
        ''' </remarks>
        Private Sub GetISETESTInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'ISETEST INSTRUCTION (Sw -> Fw)
                ''''''''''''''''''''''''''''
                myInstructionTO.InstructionType = "ISETEST"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ISETEST"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ISETEST"
                myInstructionTO.Parameter = "TI"
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ISETEST"
                myInstructionTO.Parameter = "ID"
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ISETEST"
                myInstructionTO.Parameter = "M1"
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ISETEST"
                myInstructionTO.Parameter = "TM1"
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ISETEST"
                myInstructionTO.Parameter = "RM1"
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ISETEST"
                myInstructionTO.Parameter = "VM1"
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetISETESTInstruction", EventLogEntryType.Error, False)

            End Try
        End Sub

        ''' <summary>
        ''' Predilution test instruction
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by AG 30/12/2010
        ''' </remarks>
        Private Sub GetPTESTInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'PTEST INSTRUCTION (Sw -> Fw)
                ''''''''''''''''''''''''''''
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "TI"
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "ID"
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "R1"
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "TR1"
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "RR1"
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "R2"
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "TR2"
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "RR2"
                myInstructionTO.ParameterIndex = 10
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "VM1"
                myInstructionTO.ParameterIndex = 11
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "VR1"
                myInstructionTO.ParameterIndex = 12
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "VR2"
                myInstructionTO.ParameterIndex = 13
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "OW" 'MW changes to OW --- filter used in odd cycles 1, 3, 5, ..., 67
                myInstructionTO.ParameterIndex = 14
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "EW" 'RW changes to EW --- filter used in even cycles 2, 4, 6, ..., 68
                myInstructionTO.ParameterIndex = 15
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "RN"
                myInstructionTO.ParameterIndex = 16
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "PM1"
                myInstructionTO.ParameterIndex = 17
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "PTM"
                myInstructionTO.ParameterIndex = 18
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "PRM"
                myInstructionTO.ParameterIndex = 19
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "PR1"
                myInstructionTO.ParameterIndex = 20
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "PTR"
                myInstructionTO.ParameterIndex = 21
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "PR1R"
                myInstructionTO.ParameterIndex = 22
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "PVM"
                myInstructionTO.ParameterIndex = 23
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "PTEST"
                myInstructionTO.Parameter = "PVR"
                myInstructionTO.ParameterIndex = 24
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetPTESTInstruction", EventLogEntryType.Error, False)

            End Try
        End Sub

        Private Sub GetISECommandInstructions(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                ''''''''
                'ISECMD - send ISE Command - Sw -> Fw
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ISECMD"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ISECMD"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ISECMD"
                myInstructionTO.Parameter = "M"
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ISECMD"
                myInstructionTO.Parameter = "CMD"
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ISECMD"
                myInstructionTO.Parameter = "P1"
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ISECMD"
                myInstructionTO.Parameter = "P2"
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ISECMD"
                myInstructionTO.Parameter = "P3"
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ISECMD"
                myInstructionTO.Parameter = "M1"
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ISECMD"
                myInstructionTO.Parameter = "TM1"
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ISECMD"
                myInstructionTO.Parameter = "RM1"
                myInstructionTO.ParameterIndex = 10
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ISECMD"
                myInstructionTO.Parameter = "VM1"
                myInstructionTO.ParameterIndex = 11
                Instructions.Add(myInstructionTO)

                ''''''''
                'ANSISE - Information Instrument's Adjustments response - Fw -> Sw
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSISE"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSISE"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSISE"
                myInstructionTO.Parameter = "P"
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSISE"
                myInstructionTO.Parameter = "R"
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetISECommandInstructions", EventLogEntryType.Error, False)

            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>SGM 29/05/2012</remarks>
        Private Sub GetFWUtilInstructions(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                ''''''''
                'FWUTIL - send  Sw -> Fw
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "FWUTIL"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "FWUTIL"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "FWUTIL"
                myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "FWUTIL"
                myInstructionTO.Parameter = "N"
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "FWUTIL"
                myInstructionTO.Parameter = "S"
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)


                ''''''''
                'ANSFWU -  - Fw -> Sw
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFWU"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFWU"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFWU"
                myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFWU"
                myInstructionTO.Parameter = "S"
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFWU"
                myInstructionTO.Parameter = "CRC"
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFWU"
                myInstructionTO.Parameter = "CPU"
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFWU"
                myInstructionTO.Parameter = "PER"
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFWU"
                myInstructionTO.Parameter = "MAN"
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetFWUtilInstructions", EventLogEntryType.Error, False)

            End Try
        End Sub

        ''' <summary>
        ''' standby WASHING
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by AG 28/12/2010
        ''' </remarks>
        Private Sub GetWASHInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'WASH INSTRUCTION (Sw -> Fw)
                ''''''''''''''''''''''''''''
                myInstructionTO.InstructionType = "WASH"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "WASH"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "WASH"
                myInstructionTO.Parameter = "ARM"
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "WASH"
                myInstructionTO.Parameter = "S"
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "WASH"
                myInstructionTO.Parameter = "T1" 'Washing Time in Seconds General (Air for Contitioning) (Not for External WS)
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "WASH"
                myInstructionTO.Parameter = "T2" 'Washing Time in Seconds (Washing Solution for Conditioning) (Not for External WS)
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "WASH"
                myInstructionTO.Parameter = "T3" 'Washing Time in Seconds (System Water for Conditioning )(Not for External WS)
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "WASH"
                myInstructionTO.Parameter = "BP1"
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)


                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "WASH"
                myInstructionTO.Parameter = "BT1"
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "WASH"
                myInstructionTO.Parameter = "BRT1"
                myInstructionTO.ParameterIndex = 10
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetWASHInstruction", EventLogEntryType.Error, False)

            End Try
        End Sub

        ''' <summary>
        ''' running WASHING
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by AG 28/12/2010
        ''' </remarks>
        Private Sub GetWRUNInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'WRUN INSTRUCTION (Sw -> Fw)
                ''''''''''''''''''''''''''''
                myInstructionTO.InstructionType = "WRUN"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "WRUN"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "WRUN"
                myInstructionTO.Parameter = "M"
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "WRUN"
                myInstructionTO.Parameter = "S"
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "WRUN"
                myInstructionTO.Parameter = "BP1"
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "WRUN"
                myInstructionTO.Parameter = "BT1"
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "WRUN"
                myInstructionTO.Parameter = "BRT1"
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "WRUN"
                myInstructionTO.Parameter = "BP2"
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "WRUN"
                myInstructionTO.Parameter = "BT2"
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "WRUN"
                myInstructionTO.Parameter = "BRT2"
                myInstructionTO.ParameterIndex = 10
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetWRUNInstruction", EventLogEntryType.Error, False)

            End Try
        End Sub


        ''' <summary>
        ''' WASHING STATION control
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by AG 31/05/2011
        ''' </remarks>
        Private Sub GetWSCTRLInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'WSCTRL INSTRUCTION (Sw -> Fw)
                ''''''''''''''''''''''''''''
                myInstructionTO.InstructionType = "WSCTRL"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "WSCTRL"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "WSCTRL"
                myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetWSCTRLInstruction", EventLogEntryType.Error, False)

            End Try
        End Sub


        ''' <summary>
        ''' CODEBR, ANSCBR
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' AG 21/06/2011 Creation
        ''' XBC 10/02/2012 Modified - replace parameter "C" of "CODEBR" by "C128", "C39", "CODB", ...
        ''' </remarks>
        Private Sub GetCodeBarInstructions(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                ''''''''
                'CODEBR - BAR CODE ORDERS AND CONFIGURATION
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CODEBR"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CODEBR"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CODEBR"
                myInstructionTO.Parameter = "R" 'Code bar reader selector
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CODEBR"
                myInstructionTO.Parameter = "A"    'Action
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CODEBR"
                myInstructionTO.Parameter = "P"    'Position for Action
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                ' XBC 10/02/2012
                'myInstructionTO = New InstructionParameterTO
                'myInstructionTO.InstructionType = "CODEBR"
                'myInstructionTO.Parameter = "C"    'Configuration string
                'myInstructionTO.ParameterIndex = 6
                'Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CODEBR"
                myInstructionTO.Parameter = "C128"    'CODE 128
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CODEBR"
                myInstructionTO.Parameter = "NC128"    'Number of Chars expected for the Reader
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CODEBR"
                myInstructionTO.Parameter = "C39"    'CODE 39
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CODEBR"
                myInstructionTO.Parameter = "NC39"    'Number of Chars expected for the Reader
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CODEBR"
                myInstructionTO.Parameter = "CODB"    'CODEBAR
                myInstructionTO.ParameterIndex = 10
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CODEBR"
                myInstructionTO.Parameter = "NCODB"    'Number of Chars expected for the Reader
                myInstructionTO.ParameterIndex = 11
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CODEBR"
                myInstructionTO.Parameter = "PHCD"    'PHARMACODE
                myInstructionTO.ParameterIndex = 12
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CODEBR"
                myInstructionTO.Parameter = "NPHCD"    'Number of Chars expected for the Reader
                myInstructionTO.ParameterIndex = 13
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CODEBR"
                myInstructionTO.Parameter = "UPCE"    'UPC/EAN
                myInstructionTO.ParameterIndex = 14
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CODEBR"
                myInstructionTO.Parameter = "NUPCE"    'Number of Chars expected for the Reader
                myInstructionTO.ParameterIndex = 15
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CODEBR"
                myInstructionTO.Parameter = "C93"    'CODE 93
                myInstructionTO.ParameterIndex = 16
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CODEBR"
                myInstructionTO.Parameter = "NC93"    'Number of Chars expected for the Reader
                myInstructionTO.ParameterIndex = 17
                Instructions.Add(myInstructionTO)
                ' XBC 10/02/2012

                'AG 20/03/2012
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CODEBR"
                myInstructionTO.Parameter = "I25"    'Interleave25
                myInstructionTO.ParameterIndex = 18
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CODEBR"
                myInstructionTO.Parameter = "NI25"    'Number of Chars expected for the Reader
                myInstructionTO.ParameterIndex = 19
                Instructions.Add(myInstructionTO)
                'AG 20/03/2012

                ''''''''
                'ANSCBR - Bar code answer instruction
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCBR"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCBR"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCBR"
                myInstructionTO.Parameter = "R" 'Code bar reader selector
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCBR"
                myInstructionTO.Parameter = "ST"    'Status
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCBR"
                myInstructionTO.Parameter = "N"    'Number of reads
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCBR"
                myInstructionTO.Parameter = "P"    'Position read (Loop x N)
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCBR"
                myInstructionTO.Parameter = "D"    'Read diagnostic (Loop x N)
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCBR"
                myInstructionTO.Parameter = "V"    'Value read (Loop x N)
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetCodeBarInstructions", EventLogEntryType.Error, False)

            End Try
        End Sub


        ''' <summary>
        ''' INFO, ANSINF, ANSERR instruction definition
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' AG 07/04/2011 - Creation
        ''' </remarks>
        Private Sub GetDetailsINFOOrAlarmInstructions(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                ''''''''
                'INFO - Ask for alarm or status for real time monitoring
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "INFO"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "INFO"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "INFO"
                myInstructionTO.Parameter = "Q" 'Query
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                ''''''''
                'ANSERR - Multiple alarms or errors response
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSERR"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSERR"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSERR"
                myInstructionTO.Parameter = "N" 'number of errors
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSERR"
                myInstructionTO.Parameter = "E"    'Error code
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                ''''''''
                'ANSINF - Status for real time monitoring
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSINF"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSINF"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSINF"
                myInstructionTO.Parameter = "GC" 'General cover
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSINF"
                myInstructionTO.Parameter = "PC"    'Photometrics cover (reactions cover)
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSINF"
                myInstructionTO.Parameter = "RC"    'Reagents cover
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSINF"
                myInstructionTO.Parameter = "SC"    'Samples Cover
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSINF"
                myInstructionTO.Parameter = "HS"    'System liquid sensor
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSINF"
                myInstructionTO.Parameter = "WS"    'Waste sensor
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSINF"
                myInstructionTO.Parameter = "SW"    'Wash solution weight
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSINF"
                myInstructionTO.Parameter = "WW"    'High contamination waste weight
                myInstructionTO.ParameterIndex = 10
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSINF"
                myInstructionTO.Parameter = "PT"    'Photometrics Temperature
                myInstructionTO.ParameterIndex = 11
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSINF"
                myInstructionTO.Parameter = "FS"    'Fridge status
                myInstructionTO.ParameterIndex = 12
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSINF"
                myInstructionTO.Parameter = "FT"    'Fridge temperature
                myInstructionTO.ParameterIndex = 13
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSINF"
                myInstructionTO.Parameter = "HT"    'Wash station heater temperature
                myInstructionTO.ParameterIndex = 14
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSINF"
                myInstructionTO.Parameter = "R1T"    'R1 probe temperature
                myInstructionTO.ParameterIndex = 15
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSINF"
                myInstructionTO.Parameter = "R2T"    'R2 probe temperature
                myInstructionTO.ParameterIndex = 16
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSINF"
                myInstructionTO.Parameter = "IS"    'ISE status
                myInstructionTO.ParameterIndex = 17
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetDetailsInfoOrAlarmInstructions", EventLogEntryType.Error, False)

            End Try
        End Sub


        Private Sub GetANSArmsInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                ''''''''
                'ANSBR1 - Reagent1 arm status instruction
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBR1"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBR1"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBR1"
                myInstructionTO.Parameter = "ID" 'Preparation ID
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBR1"
                myInstructionTO.Parameter = "W"    'Well
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBR1"
                myInstructionTO.Parameter = "S"    'Status
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBR1"
                myInstructionTO.Parameter = "P"    'Bottle Position
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBR1"
                myInstructionTO.Parameter = "L"    'Level Control
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                ''''''''
                'ANSBM1 - Sample arm status instruction
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBM1"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBM1"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBM1"
                myInstructionTO.Parameter = "ID" 'Preparation ID
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBM1"
                myInstructionTO.Parameter = "W"    'Well
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBM1"
                myInstructionTO.Parameter = "S"    'Status
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBM1"
                myInstructionTO.Parameter = "P"    'Bottle Position
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBM1"
                myInstructionTO.Parameter = "L"    'Level Control
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBM1"
                myInstructionTO.Parameter = "C"    'Clot detection
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)


                ''''''''
                'ANSBR2 - Reagent2 arm status instruction
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBR2"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBR2"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBR2"
                myInstructionTO.Parameter = "ID" 'Preparation ID
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBR2"
                myInstructionTO.Parameter = "W"    'Well
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBR2"
                myInstructionTO.Parameter = "S"    'Status
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBR2"
                myInstructionTO.Parameter = "P"    'Bottle Position
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBR2"
                myInstructionTO.Parameter = "L"    'Level Control
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetANSArmsInstruction", EventLogEntryType.Error, False)

            End Try
        End Sub


        ''' <summary>
        ''' POLLHW, ANSCPU, ANSBXX, ANSDXX, ANSRXX, ANSJEX, ANSSFX instruction definitions
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by XBC 06/05/2011
        ''' Modified by XBC 01/06/2011 - Add ANSBXX, ANSDXX, ANSRXX
        ''' Modified by XBC 08/06/2011 - Add ANSCPU
        ''' </remarks>
        Private Sub GetDetailsPOLLHWInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                ''''''''
                'POLLHW - Ask for alarm or status HW (Sw -> Fw)
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "POLLHW"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "POLLHW"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "POLLHW"
                myInstructionTO.Parameter = "ID"
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                ''''''''
                'ANSCPU - Multiple alarms or errors response
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "ANSCPU"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "ID" 'identificator 
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "TMP"    'board Temperature
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "CAN" 'CAN BUS State
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "BM1" 'CAN BUS Diagnostic SAMPLE ARM
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "BR1" 'CAN BUS Diagnostic REAGENT1 ARM
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "BR2" 'CAN BUS Diagnostic REAGENT2 ARM
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "AG1" 'CAN BUS Diagnostic MIXER1 ARM
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "AG2" 'CAN BUS Diagnostic MIXER2 ARM
                myInstructionTO.ParameterIndex = 10
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "DM1" 'CAN BUS Diagnostic SAMPLES PROBE
                myInstructionTO.ParameterIndex = 11
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "DR1" 'CAN BUS Diagnostic REAGENT1 PROBE
                myInstructionTO.ParameterIndex = 12
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "DR2" 'CAN BUS Diagnostic REAGENT2 PROBE 
                myInstructionTO.ParameterIndex = 13
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "RR1" 'CAN BUS Diagnostic REAGENTS ROTOR 
                myInstructionTO.ParameterIndex = 14
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "RM1" 'CAN BUS Diagnostic SAMPLES ROTOR
                myInstructionTO.ParameterIndex = 15
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "GLF" 'CAN BUS Diagnostic PHOTOMETRIC
                myInstructionTO.ParameterIndex = 16
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "SF1" 'CAN BUS Diagnostic FLUIDICS
                myInstructionTO.ParameterIndex = 17
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "JE1" 'CAN BUS Diagnostic MANIFOLD
                myInstructionTO.ParameterIndex = 18
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "MC" 'MAIN COVER
                myInstructionTO.ParameterIndex = 19
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "BUZ" 'BUZZER STATE
                myInstructionTO.ParameterIndex = 20
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "FWFM" 'Firmware Repository Flash Memory
                myInstructionTO.ParameterIndex = 21
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "BBFM" 'Black Box Flash Memory
                myInstructionTO.ParameterIndex = 22
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "ISE" 'ISE State
                myInstructionTO.ParameterIndex = 23
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSCPU"
                myInstructionTO.Parameter = "ISEE" 'ISE Error
                myInstructionTO.ParameterIndex = 24
                Instructions.Add(myInstructionTO)

                ''''''''
                'ANSBXX - Multiple alarms or errors response
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBXX"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBXX"
                myInstructionTO.Parameter = "ANSBXX"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBXX"
                myInstructionTO.Parameter = "ID" 'identificator 
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBXX"
                myInstructionTO.Parameter = "TMP"    'board Temperature
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBXX"
                myInstructionTO.Parameter = "MH" 'Horizontal Motor status
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBXX"
                myInstructionTO.Parameter = "MHH" 'Horizontal Motor Home status
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBXX"
                myInstructionTO.Parameter = "MHA" 'Horizontal Position
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBXX"
                myInstructionTO.Parameter = "MV" 'Vertical Motor status
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBXX"
                myInstructionTO.Parameter = "MVH" 'Vertical Motor Home status
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSBXX"
                myInstructionTO.Parameter = "MVA" 'Vertical Position
                myInstructionTO.ParameterIndex = 10
                Instructions.Add(myInstructionTO)

                ''''''''
                'ANSDXX - Multiple alarms or errors response
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSDXX"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSDXX"
                myInstructionTO.Parameter = "ANSDXX"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSDXX"
                myInstructionTO.Parameter = "ID" 'identificator 
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSDXX"
                myInstructionTO.Parameter = "TMP"    'board Temperature
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSDXX"
                myInstructionTO.Parameter = "DST" 'Detection status
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSDXX"
                myInstructionTO.Parameter = "DFQ" 'Detection Base Frequency
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSDXX"
                myInstructionTO.Parameter = "D" 'Detection
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSDXX"
                myInstructionTO.Parameter = "DCV" 'Internal Rate of Change in last detection
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSDXX"
                myInstructionTO.Parameter = "PTH" 'Probe Thermistor Value
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSDXX"
                myInstructionTO.Parameter = "PTHD" 'Probe Thermistor Diagnostic
                myInstructionTO.ParameterIndex = 10
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSDXX"
                myInstructionTO.Parameter = "PH" 'Probe Heater state
                myInstructionTO.ParameterIndex = 11
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSDXX"
                myInstructionTO.Parameter = "PHD" 'Probe Heater Diagnostic
                myInstructionTO.ParameterIndex = 12
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSDXX"
                myInstructionTO.Parameter = "CD" 'Collision Detector
                myInstructionTO.ParameterIndex = 13
                Instructions.Add(myInstructionTO)

                ''''''''
                'ANSRXX - Multiple alarms or errors response
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "ANSRXX"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "ID" 'identificator 
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "TMP"    'board Temperature
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "MR" 'Rotor Motor
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "MRH" 'Home Motor Rotor
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "MRA" 'Motor Position
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "FTH" 'Fridge Thermistor Value
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "FTHD" 'Fridge Thermistor Diagnostic
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "FH" 'Fridge Peltiers state
                myInstructionTO.ParameterIndex = 10
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "FHD" 'Fridge Peltiers Diagnostic
                myInstructionTO.ParameterIndex = 11
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "PF1" 'Peltier Fan 1 Speed
                myInstructionTO.ParameterIndex = 12
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "PF1D" 'Peltier Fan 1 Diagnostic
                myInstructionTO.ParameterIndex = 13
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "PF2" 'Peltier Fan 2 Speed
                myInstructionTO.ParameterIndex = 14
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "PF2D" 'Peltier Fan 2 Diagnostic
                myInstructionTO.ParameterIndex = 15
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "PF3" 'Peltier Fan 3 Speed
                myInstructionTO.ParameterIndex = 16
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "PF3D" 'Peltier Fan 3 Diagnostic
                myInstructionTO.ParameterIndex = 17
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "PF4" 'Peltier Fan 4 Speed
                myInstructionTO.ParameterIndex = 18
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "PF4D" 'Peltier Fan 4 Diagnostic
                myInstructionTO.ParameterIndex = 19
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "FF1" 'Frame Fan 1 Speed
                myInstructionTO.ParameterIndex = 20
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "FF1D" 'Frame Fan 1 Diagnostic
                myInstructionTO.ParameterIndex = 21
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "FF2" 'Frame Fan 2 Speed
                myInstructionTO.ParameterIndex = 22
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "FF2D" 'Frame Fan 2 Diagnostic
                myInstructionTO.ParameterIndex = 23
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "RC" 'Rotor Cover
                myInstructionTO.ParameterIndex = 24
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "CB" 'Codebar Reader state
                myInstructionTO.ParameterIndex = 25
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSRXX"
                myInstructionTO.Parameter = "CBE" 'Codebar Reader error
                myInstructionTO.ParameterIndex = 26
                Instructions.Add(myInstructionTO)

                ''''''''
                'ANSJEX - MANIFOLD
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "ID" 'identificator (JE1)
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "TMP"    'Board Temperature
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "MR1"    'Reagent1 Motor
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "MR1H"    'Reagent1 Motor
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "MR1A"    'Reagent1 Motor
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "MR2" 'Reagent2 Motor
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "MR2H" 'Reagent2 Motor
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "MR2A" 'Reagent2 Motor
                myInstructionTO.ParameterIndex = 10
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "MS" 'Samples Motor
                myInstructionTO.ParameterIndex = 11
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "MSH" 'Samples Motor
                myInstructionTO.ParameterIndex = 12
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "MSA" 'Samples Motor
                myInstructionTO.ParameterIndex = 13
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "B1" 'Samples Pump
                myInstructionTO.ParameterIndex = 14
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "B1D" 'Samples Pump
                myInstructionTO.ParameterIndex = 15
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "B2" 'Reagent1 pump
                myInstructionTO.ParameterIndex = 16
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "B2D" 'Reagent1 pump
                myInstructionTO.ParameterIndex = 17
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "B3" 'Reagent2 Pump
                myInstructionTO.ParameterIndex = 18
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "B3D" 'Reagent2 Pump
                myInstructionTO.ParameterIndex = 19
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "E1" 'Samples Valve
                myInstructionTO.ParameterIndex = 20
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "E1D" 'Samples Valve
                myInstructionTO.ParameterIndex = 21
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "E2" 'Reagent1 Valve
                myInstructionTO.ParameterIndex = 22
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "E2D" 'Reagent1 Valve
                myInstructionTO.ParameterIndex = 23
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "E3" 'Reagent2 Valve
                myInstructionTO.ParameterIndex = 24
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "E3D" 'Reagent2 Valve
                myInstructionTO.ParameterIndex = 25
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "E4" 'Air/WS Valve
                myInstructionTO.ParameterIndex = 26
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "E4D" 'Air/WS Valve
                myInstructionTO.ParameterIndex = 27
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "E5" 'AirWS/PW Valve
                myInstructionTO.ParameterIndex = 28
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "E5D" 'AirWS/PW Valve
                myInstructionTO.ParameterIndex = 29
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "E6" 'not used
                myInstructionTO.ParameterIndex = 30
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "E6D" 'not used
                myInstructionTO.ParameterIndex = 31
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "GE1"
                myInstructionTO.ParameterIndex = 32
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "GE1D"
                myInstructionTO.ParameterIndex = 33
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "GE2"
                myInstructionTO.ParameterIndex = 34
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "GE2D"
                myInstructionTO.ParameterIndex = 35
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "GE3"
                myInstructionTO.ParameterIndex = 36
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "GE3D"
                myInstructionTO.ParameterIndex = 37
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "CLT" 'Clot detection
                myInstructionTO.ParameterIndex = 38
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSJEX"
                myInstructionTO.Parameter = "CLTD"
                myInstructionTO.ParameterIndex = 39
                Instructions.Add(myInstructionTO)

                ''''''''
                'ANSSFX - FLUIDICS
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "ID" 'identificator (JE1)
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "TMP"    'Board Temperature
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "MS" 'Washing Station Motor
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "MSH" 'Washing Station Motor
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "MSA" 'Washing Station Motor
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "B1" 'Samples Arm pump
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "B1D" 'Samples Arm pump
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "B2" 'Reagent1/Mixer2 Arm pump
                myInstructionTO.ParameterIndex = 10
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "B2D" 'Reagent1/Mixer2 Arm pump
                myInstructionTO.ParameterIndex = 11
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "B3" 'Reagent2/Mixer1 Arm pump
                myInstructionTO.ParameterIndex = 12
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "B3D" 'Reagent2/Mixer1 Arm pump
                myInstructionTO.ParameterIndex = 13
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "B4" 'Purified Water (from external tank) pump
                myInstructionTO.ParameterIndex = 14
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "B4D" 'Purified Water (from external tank) pump
                myInstructionTO.ParameterIndex = 15
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "B5" 'Low Contamination Out pump
                myInstructionTO.ParameterIndex = 16
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "B5D" 'Low Contamination Out pump
                myInstructionTO.ParameterIndex = 17
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "B6" 'Washing Station needle 2,3 pump
                myInstructionTO.ParameterIndex = 18
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "B6D" 'Washing Station needle 2,3 pump
                myInstructionTO.ParameterIndex = 19
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "B7" 'Washing Station needle 4,5 pump
                myInstructionTO.ParameterIndex = 20
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "B7D" 'Washing Station needle 4,5 pump
                myInstructionTO.ParameterIndex = 21
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "B8" 'Washing Station needle 6 pump
                myInstructionTO.ParameterIndex = 22
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "B8D" 'Washing Station needle 6 pump
                myInstructionTO.ParameterIndex = 23
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "B9" 'Washing Station needle 7 pump
                myInstructionTO.ParameterIndex = 24
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "B9D" 'Washing Station needle 7 pump
                myInstructionTO.ParameterIndex = 25
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "B10" 'Washing Station needle 1 pump
                myInstructionTO.ParameterIndex = 26
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "B10D" 'Washing Station needle 1 pump
                myInstructionTO.ParameterIndex = 27
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "GE1"
                myInstructionTO.ParameterIndex = 28
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "GE1D"
                myInstructionTO.ParameterIndex = 29
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "E1" 'Purified Water (from external source)
                myInstructionTO.ParameterIndex = 30
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "E1D" 'Purified Water (from external source)
                myInstructionTO.ParameterIndex = 31
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "E2" 'Purified Water (from external tank)
                myInstructionTO.ParameterIndex = 32
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "E2D" 'Purified Water (from external tank)
                myInstructionTO.ParameterIndex = 33
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "E3" 'not used
                myInstructionTO.ParameterIndex = 34
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "E3D" 'not used
                myInstructionTO.ParameterIndex = 35
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "WSTH"
                myInstructionTO.ParameterIndex = 36
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "WSTHD"
                myInstructionTO.ParameterIndex = 37
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "WSH"
                myInstructionTO.ParameterIndex = 38
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "WSHD"
                myInstructionTO.ParameterIndex = 39
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "WSW"
                myInstructionTO.ParameterIndex = 40
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "WSWD"
                myInstructionTO.ParameterIndex = 41
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "HCW"
                myInstructionTO.ParameterIndex = 42
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "HCWD"
                myInstructionTO.ParameterIndex = 43
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "WAS"
                myInstructionTO.ParameterIndex = 44
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "WTS"
                myInstructionTO.ParameterIndex = 45
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "SLS"
                myInstructionTO.ParameterIndex = 46
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "STS"
                myInstructionTO.ParameterIndex = 47
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "ST1"
                myInstructionTO.ParameterIndex = 48
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSFX"
                myInstructionTO.Parameter = "ST2"
                myInstructionTO.ParameterIndex = 49
                Instructions.Add(myInstructionTO)



                ''''''''
                'ANSGLF - PHOTOMETRICS
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "ID" 'identificator (JE1)
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "TMP"    'Board Temperature
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "MR"    'Reactions Rotor Motor
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "MRH"    'Reactions Rotor Motor
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "MRA"    'Reactions Rotor Motor
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "MRE"    'Reactions Rotor Motor
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "MRED"    'Reactions Rotor Motor
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "MW"    'Washing Station Motor
                myInstructionTO.ParameterIndex = 10
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "MWH"    'Washing Station Motor
                myInstructionTO.ParameterIndex = 11
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "MWA"   'Washing Station Motor
                myInstructionTO.ParameterIndex = 12
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "CD"   'Collision Detector
                myInstructionTO.ParameterIndex = 13
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "PTH"   'Rotor Thermistor
                myInstructionTO.ParameterIndex = 14
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "PTHD"   'Rotor Thermistor
                myInstructionTO.ParameterIndex = 15
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "PH"   'Rotor Peltier
                myInstructionTO.ParameterIndex = 16
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "PHD"   'Rotor Peltier
                myInstructionTO.ParameterIndex = 17
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "PF1"   'Peltier Fan 1
                myInstructionTO.ParameterIndex = 18
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "PF1D"   'Peltier Fan 1
                myInstructionTO.ParameterIndex = 19
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "PF2"   'Peltier Fan 2
                myInstructionTO.ParameterIndex = 20
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "PF2D"   'Peltier Fan 2
                myInstructionTO.ParameterIndex = 21
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "PF3"   'Peltier Fan 3
                myInstructionTO.ParameterIndex = 22
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "PF3D"   'Peltier Fan 3
                myInstructionTO.ParameterIndex = 23
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "PF4"   'Peltier Fan 4
                myInstructionTO.ParameterIndex = 24
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "PF4D"   'Peltier Fan 4
                myInstructionTO.ParameterIndex = 25
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "RC"   'Rotor Cover
                myInstructionTO.ParameterIndex = 26
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "PHT"   'Photometry state
                myInstructionTO.ParameterIndex = 27
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSGLF"
                myInstructionTO.Parameter = "PHFM"   'Photometry memory state
                myInstructionTO.ParameterIndex = 28
                Instructions.Add(myInstructionTO)
                
                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetDetailsPOLLHWInstruction", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' TANKSTEST instruction definition
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' XBC 18/05/2011 - Creation
        ''' </remarks>
        Private Sub GetTANKSTESTInstructions(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                ''''''''
                'TANKSTEST - Request for a partial tanks process test (3)
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "TANKSTEST"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "TANKSTEST"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "TANKSTEST"
                myInstructionTO.Parameter = "Q" 'Query
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetTANKSTESTInstruction", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' STRESS MODE instructions definition
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by XBC 23/05/2010
        ''' </remarks>
        Private Sub GetDetailsSTRESSInstructions(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'STRESS TEST INSTRUCTION (Sw -> Fw)
                ''''''''''''''''''''''''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "SDMODE"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "SDMODE"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "SDMODE"
                myInstructionTO.Parameter = "Q" 'Query
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "SDMODE"
                myInstructionTO.Parameter = "CYC" 'Cycles
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "SDMODE"
                myInstructionTO.Parameter = "DH" 'Hour
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "SDMODE"
                myInstructionTO.Parameter = "DM" 'Minute
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "SDMODE"
                myInstructionTO.Parameter = "DS" 'Second
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                'STRESS REQUEST INSTRUCTION (Sw -> Fw)
                ''''''''''''''''''''''''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "SDPOLL"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "SDPOLL"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

          
              
                'STRESS RECEIVED INSTRUCTION (Fw -> Sw)
                ''''''''''''''''''''''''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSDM"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSDM"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSDM"
                myInstructionTO.Parameter = "S"
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSDM"
                myInstructionTO.Parameter = "AC"
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSDM"
                myInstructionTO.Parameter = "ST"
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSDM"
                myInstructionTO.Parameter = "NC"
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSDM"
                myInstructionTO.Parameter = "NCP"
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSDM"
                myInstructionTO.Parameter = "DH"
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSDM"
                myInstructionTO.Parameter = "DM"
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSDM"
                myInstructionTO.Parameter = "DS"
                myInstructionTO.ParameterIndex = 10
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSDM"
                myInstructionTO.Parameter = "T"
                myInstructionTO.ParameterIndex = 11
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSDM"
                myInstructionTO.Parameter = "NR"
                myInstructionTO.ParameterIndex = 12
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSDM"
                myInstructionTO.Parameter = "CR"
                myInstructionTO.ParameterIndex = 13
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSDM"
                myInstructionTO.Parameter = "NE"
                myInstructionTO.ParameterIndex = 14
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSSDM"
                myInstructionTO.Parameter = "CE"
                myInstructionTO.ParameterIndex = 15
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetDetailsSTRESSInstructions", EventLogEntryType.Error, False)

            End Try
        End Sub

        ''' <summary>
        ''' POLLFW, ANSFCP instruction definitions
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by XBC 25/05/2011
        ''' Modified by XBC 02/06/2011 - add Answer instructions
        ''' Modified by SGM 25/05/2012 - new spec from Firmware (ANSFCP)
        ''' </remarks>
        Private Sub GetDetailsPOLLFWInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                ''''''''
                'POLLFW - Ask for versions info FW (Sw -> Fw)
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "POLLFW"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "POLLFW"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "POLLFW"
                myInstructionTO.Parameter = "ID"
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                ''''''''
                'ANSFCP - Firmware Versions and Checksum for CPU electronic boards  (Fw -> Sw)
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFCP"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFCP"
                myInstructionTO.Parameter = "ANSFCP"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFCP"
                myInstructionTO.Parameter = "ID" 'CPU 'identificator 
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFCP"
                myInstructionTO.Parameter = "SMC"    'board serial number (0xXXXXXXX)
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFCP"
                myInstructionTO.Parameter = "RV" 'Repository version (X.X)
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFCP"
                myInstructionTO.Parameter = "CRC" 'Whole Repository CRC32 result (OK, KO)
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFCP"
                myInstructionTO.Parameter = "CRCV" 'Whole Repository CRC32 value (0xXXXXXXXX)
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFCP"
                myInstructionTO.Parameter = "CRCS" 'Whole Repository CRC32 size (XXXXX)
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFCP"
                myInstructionTO.Parameter = "FWV" 'CPU FW version (X.X)
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFCP"
                myInstructionTO.Parameter = "FWCRC" 'CPU FW CRC32 result (OK, KO)
                myInstructionTO.ParameterIndex = 10
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFCP"
                myInstructionTO.Parameter = "FWCRCV" 'CPU FW CRC32 value (0xXXXXXXXX)
                myInstructionTO.ParameterIndex = 11
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFCP"
                myInstructionTO.Parameter = "FWCRCS" 'CPU FW CRC32 Size (XXXXX)
                myInstructionTO.ParameterIndex = 12
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFCP"
                myInstructionTO.Parameter = "HWV" 'Hardware version (0-10)
                myInstructionTO.ParameterIndex = 13
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFCP"
                myInstructionTO.Parameter = "ASN" 'Analyzer Serial Number (XXXXXXXXXXX)
                myInstructionTO.ParameterIndex = 14
                Instructions.Add(myInstructionTO)


                ''''''''
                'ANSFBX - Firmware Versions and Checksum for ARM electronic boards  (Fw -> Sw)
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFBX"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFBX"
                myInstructionTO.Parameter = "ANSFBX"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFBX"
                myInstructionTO.Parameter = "ID" 'identificator 
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFBX"
                myInstructionTO.Parameter = "SMC"    'board serial number
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFBX"
                myInstructionTO.Parameter = "FWV" 'Fw version
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFBX"
                myInstructionTO.Parameter = "CHK" 'checksum result
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFBX"
                myInstructionTO.Parameter = "CHKV" 'checksum value
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFBX"
                myInstructionTO.Parameter = "CHKS" 'checksum size
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFBX"
                myInstructionTO.Parameter = "HWV" 'Hardware version
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                ''''''''
                'ANSFDX - Firmware Versions and Checksum for PROBE electronic boards  (Fw -> Sw)
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFDX"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFDX"
                myInstructionTO.Parameter = "ANSFDX"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFDX"
                myInstructionTO.Parameter = "ID" 'identificator 
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFDX"
                myInstructionTO.Parameter = "SMC"    'board serial number
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFDX"
                myInstructionTO.Parameter = "FWV" 'Fw version
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFDX"
                myInstructionTO.Parameter = "CHK" 'checksum result
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFDX"
                myInstructionTO.Parameter = "CHKV" 'checksum value
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFDX"
                myInstructionTO.Parameter = "CHKS" 'checksum size
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFDX"
                myInstructionTO.Parameter = "HWV" 'Hardware version
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                ''''''''
                'ANSFRX - Firmware Versions and Checksum for ROTOR electronic boards  (Fw -> Sw)
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFRX"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFRX"
                myInstructionTO.Parameter = "ANSFRX"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFRX"
                myInstructionTO.Parameter = "ID" 'identificator 
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFRX"
                myInstructionTO.Parameter = "SMC"    'board serial number
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFRX"
                myInstructionTO.Parameter = "FWV" 'Fw version
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFRX"
                myInstructionTO.Parameter = "CHK" 'checksum result
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFRX"
                myInstructionTO.Parameter = "CHKV" 'checksum value
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFRX"
                myInstructionTO.Parameter = "CHKS" 'checksum size
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFRX"
                myInstructionTO.Parameter = "HWV" 'Hardware version
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                ''''''''
                'ANSFGL - Firmware Versions and Checksum for PHOTOMETRIC electronic boards  (Fw -> Sw)
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFGL"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFGL"
                myInstructionTO.Parameter = "ANSFGL"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFGL"
                myInstructionTO.Parameter = "ID" 'identificator 
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFGL"
                myInstructionTO.Parameter = "SMC"    'board serial number
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFGL"
                myInstructionTO.Parameter = "FWV" 'Fw version
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFGL"
                myInstructionTO.Parameter = "CHK" 'checksum result
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFGL"
                myInstructionTO.Parameter = "CHKV" 'checksum value
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFGL"
                myInstructionTO.Parameter = "CHKS" 'checksum size
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFGL"
                myInstructionTO.Parameter = "HWV" 'Hardware version
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                ''''''''
                'ANSFJE - Firmware Versions and Checksum for MANIFOLD electronic boards  (Fw -> Sw)
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFJE"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFJE"
                myInstructionTO.Parameter = "ANSFJE"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFJE"
                myInstructionTO.Parameter = "ID" 'identificator 
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFJE"
                myInstructionTO.Parameter = "SMC"    'board serial number
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFJE"
                myInstructionTO.Parameter = "FWV" 'Fw version
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFJE"
                myInstructionTO.Parameter = "CHK" 'checksum result
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFJE"
                myInstructionTO.Parameter = "CHKV" 'checksum value
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFJE"
                myInstructionTO.Parameter = "CHKS" 'checksum size
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFJE"
                myInstructionTO.Parameter = "HWV" 'Hardware version
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                ''''''''
                'ANSFSF - Firmware Versions and Checksum for FLUIDICS electronic boards  (Fw -> Sw)
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFSF"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFSF"
                myInstructionTO.Parameter = "ANSFSF"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFSF"
                myInstructionTO.Parameter = "ID" 'identificator 
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFSF"
                myInstructionTO.Parameter = "SMC"    'board serial number
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFSF"
                myInstructionTO.Parameter = "FWV" 'Fw version
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFSF"
                myInstructionTO.Parameter = "CHK" 'checksum result
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFSF"
                myInstructionTO.Parameter = "CHKV" 'checksum value
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFSF"
                myInstructionTO.Parameter = "CHKS" 'checksum size
                myInstructionTO.ParameterIndex = 8
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSFSF"
                myInstructionTO.Parameter = "HWV" 'Hardware version
                myInstructionTO.ParameterIndex = 9
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetDetailsPOLLFWInstruction", EventLogEntryType.Error, False)
            End Try
        End Sub


        ''' <summary>
        ''' RESET instructions definition
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by SGM 01/07/2011
        ''' </remarks>
        Private Sub GetDetailsRESETInstructions(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'RESET INSTRUCTION (Sw -> Fw)      PDT TO SPECIFIED !!!
                ''''''''''''''''''''''''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "RESET"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "RESET"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetDetailsRESETInstructions", EventLogEntryType.Error, False)

            End Try
        End Sub

        '''' <summary>
        '''' LOADFACTORYADJ instructions definition
        '''' </summary>
        '''' <param name="Instructions"></param>
        '''' <remarks>
        '''' Created by SGM 01/07/2011
        '''' </remarks>
        'Private Sub GetDetailsLOADFACTORYADJInstructions(ByRef Instructions As List(Of InstructionParameterTO))
        '    Try
        '        Dim myInstructionTO As New InstructionParameterTO

        '        'GetDetailsLOADFACTORYADJInstructions INSTRUCTION (Sw -> Fw)      PDT TO SPECIFIED !!!
        '        ''''''''''''''''''''''''''''''
        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "LOADFACTORYADJ"
        '        myInstructionTO.Parameter = ""
        '        myInstructionTO.ParameterIndex = 1
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "LOADFACTORYADJ"
        '        myInstructionTO.Parameter = ""
        '        myInstructionTO.ParameterIndex = 2
        '        Instructions.Add(myInstructionTO)



        '    Catch ex As Exception
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetDetailsLOADFACTORYADJInstructions", EventLogEntryType.Error, False)

        '    End Try
        'End Sub

        ''' <summary>
        ''' UPDATEFW instructions definition
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by SGM 05/07/2011
        ''' </remarks>
        Private Sub GetDetailsUPDATEFWInstructions(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'GetDetailsUPDATEFWInstructions INSTRUCTION (Sw -> Fw)      PDT TO SPECIFIED !!!
                ''''''''''''''''''''''''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "UPDATEFW"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "UPDATEFW"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "UPDATEFW"
                myInstructionTO.Parameter = "" ' x NumParams
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetDetailsUPDATEFWInstructions", EventLogEntryType.Error, False)

            End Try
        End Sub


        ''' <summary>
        ''' FW EVENTS instructions definition
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by SGM 10/06/2011
        ''' </remarks>
        Private Sub GetDetailsFWEVENTSInstructions(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'ENABLE_FW_EVENTS INSTRUCTION (Sw -> Fw)      PDT TO SPECIFIED !!!
                ''''''''''''''''''''''''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ENABLE_EVENTS"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ENABLE_EVENTS"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                ''''''''
                'ANSEVENTS - PDT to spec
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSEVENTS"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSEVENTS"
                myInstructionTO.Parameter = "ANSEVENTS"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSEVENTS"
                myInstructionTO.Parameter = "ID" 'identificator 
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSEVENTS"
                myInstructionTO.Parameter = "WSCD"    'Washing Station Collision
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSEVENTS"
                myInstructionTO.Parameter = "SCD"    'Samples Needle Collision
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSEVENTS"
                myInstructionTO.Parameter = "R1CD"    'Reagent 1 Needle Collision
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSEVENTS"
                myInstructionTO.Parameter = "R2CD"    'Reagent 2 Needle Collision
                myInstructionTO.ParameterIndex = 7
                Instructions.Add(myInstructionTO)


                'DISABLE_FW_EVENTS INSTRUCTION (Sw -> Fw)       PDT TO SPECIFIED !!!
                '''''''''''''''''''''''''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "DISABLE_EVENTS"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "DISABLE_EVENTS"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetDetailsFWEVENTSInstructions", EventLogEntryType.Error, False)

            End Try
        End Sub

        ''' <summary>
        ''' SOUND
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by XBC 18/10/2011
        ''' </remarks>
        Private Sub GetSOUNDInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'SOUND INSTRUCTION (Sw -> Fw)
                ''''''''''''''''''''''''''''
                myInstructionTO.InstructionType = "SOUND"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "SOUND"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "SOUND"
                myInstructionTO.Parameter = "ST"
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "SOUND"
                myInstructionTO.Parameter = "M"
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetSOUNDInstruction", EventLogEntryType.Error, False)

            End Try
        End Sub


        ''' <summary>
        ''' General configuration instruction
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by AG 23/11/2011
        ''' </remarks>
        Private Sub GetCONFIGInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'CONFIG INSTRUCTION (Sw -> Fw)
                ''''''''''''''''''''''''''''
                myInstructionTO.InstructionType = "CONFIG"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CONFIG"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CONFIG"
                myInstructionTO.Parameter = "RCB"
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CONFIG"
                myInstructionTO.Parameter = "SCB"
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "CONFIG"
                myInstructionTO.Parameter = "WSS"
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetCONFIGInstruction", EventLogEntryType.Error, False)

            End Try
        End Sub

        ''' <summary>
        ''' Util instruction
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by XBC 04/06/2012
        ''' </remarks>
        Private Sub GetUTILInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'UTIL INSTRUCTION (Sw -> Fw)
                ''''''''''''''''''''''''''''
                myInstructionTO.InstructionType = "UTIL"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "UTIL"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "UTIL"
                myInstructionTO.Parameter = "T"
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "UTIL"
                myInstructionTO.Parameter = "C"
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "UTIL"
                myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "UTIL"
                myInstructionTO.Parameter = "SN"
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetUTILInstruction", EventLogEntryType.Error, False)

            End Try
        End Sub

        ''' <summary>
        ''' ANSUTIL
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>Created by SG 10/10/2012</remarks>
        Private Sub GetANSUTILInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'ANSUTIL INSTRUCTION (Fw -> Sw)
                ''''''''''''''''''''''''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSUTIL"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSUTIL"
                myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSUTIL"
                myInstructionTO.Parameter = "A"    'Util instruction type
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSUTIL"
                myInstructionTO.Parameter = "T" 'Tanks test event
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSUTIL"
                myInstructionTO.Parameter = "C" 'collision test event
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSUTIL"
                myInstructionTO.Parameter = "SN" 'save serial number result
                myInstructionTO.ParameterIndex = 6
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetANSUTILInstruction", EventLogEntryType.Error, False)

            End Try
        End Sub

        ''' <summary>
        ''' POLLSN instructions definition (ask + answer)
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by XBC 30/07/2012
        ''' </remarks>
        Private Sub GetDetailsPOLLSNInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'POLLSN INSTRUCTION (Sw -> Fw)
                ''''''''''''''''''''''''''''''
                '"A"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "POLLSN"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                '"TYPE"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "POLLSN"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                ''''''''
                'ANSPSN - Firmware answer with Analyzer Serial Number  (Fw -> Sw)
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSPSN"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSPSN"
                myInstructionTO.Parameter = "ANSPSN"
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSPSN"
                myInstructionTO.Parameter = "SN" 'serial number
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetDetailsPOLLSNInstruction", EventLogEntryType.Error, False)

            End Try
        End Sub

        ''' <summary>
        ''' POLLRD instructions definition (ask + answer) for recovery results
        ''' </summary>
        ''' <param name="Instructions"></param>
        ''' <remarks>
        ''' Created by AG 31/07/2012
        ''' </remarks>
        Private Sub GetDetailsPOLLRDInstruction(ByRef Instructions As List(Of InstructionParameterTO))
            Try
                Dim myInstructionTO As New InstructionParameterTO

                'POLLSN INSTRUCTION (Sw -> Fw)
                ''''''''''''''''''''''''''''''
                '"A"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "POLLRD"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                '"TYPE"
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "POLLRD"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                'ACTION
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "POLLRD"
                myInstructionTO.Parameter = "A"
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                'WELL (START RECOVER BIOCHEMICAL RESULTS)
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "POLLRD"
                myInstructionTO.Parameter = "W"
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)


                ''''''''
                'ANSPRD - Firmware answers END recover data
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSPRD"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSPRD"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSPRD"
                myInstructionTO.Parameter = "A" 'Action
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSPRD"
                myInstructionTO.Parameter = "S" 'State
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                ''''''''
                'ANSTIN - Firmware answers PREPARATIONS with problems
                ''''''''
                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSTIN"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 1
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSTIN"
                myInstructionTO.Parameter = ""
                myInstructionTO.ParameterIndex = 2
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSTIN"
                myInstructionTO.Parameter = "N" 'Number
                myInstructionTO.ParameterIndex = 3
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSTIN"
                myInstructionTO.Parameter = "ID" 'Identificator
                myInstructionTO.ParameterIndex = 4
                Instructions.Add(myInstructionTO)

                myInstructionTO = New InstructionParameterTO
                myInstructionTO.InstructionType = "ANSTIN"
                myInstructionTO.Parameter = "EC" 'Error code
                myInstructionTO.ParameterIndex = 5
                Instructions.Add(myInstructionTO)

                myInstructionTO = Nothing   ' XB 19/02/2014 - release memory - task #1496

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetDetailsPOLLRDInstruction", EventLogEntryType.Error, False)

            End Try
        End Sub


#End Region


#Region "Commented code"

        ' XBC 17/05/2011 - delete redundant Events replaced using AnalyzerManager.ReceptionEvent

        '' SGM 28/02/11 - Pending to define PDT !!!
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <param name="Instructions"></param>
        '''' <remarks>
        '''' Created by SGM 28/02/11
        '''' </remarks>
        'Private Sub GetSENSORS_RECEIVEDInstruction(ByRef Instructions As List(Of InstructionParameterTO))
        '    Try
        '        Dim myInstructionTO As New InstructionParameterTO

        '        'STATUS INSTRUCTION (Fw -> Sw)
        '        ''''''''''''''''''''''''''''''
        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "SENSORS_RECEIVED"
        '        myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
        '        myInstructionTO.ParameterIndex = 1
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "SENSORS_RECEIVED"
        '        myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
        '        myInstructionTO.ParameterIndex = 2
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "SENSORS_RECEIVED"
        '        myInstructionTO.Parameter = "SENSORS"
        '        myInstructionTO.ParameterIndex = 3
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "SENSORS_RECEIVED"
        '        myInstructionTO.Parameter = "VALUES"
        '        myInstructionTO.ParameterIndex = 4
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "SENSORS_RECEIVED"
        '        myInstructionTO.Parameter = "T"
        '        myInstructionTO.ParameterIndex = 5
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "SENSORS_RECEIVED"
        '        myInstructionTO.Parameter = "D"
        '        myInstructionTO.ParameterIndex = 6
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "SENSORS_RECEIVED"
        '        myInstructionTO.Parameter = "E"
        '        myInstructionTO.ParameterIndex = 7
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "SENSORS_RECEIVED"
        '        myInstructionTO.Parameter = "F"
        '        myInstructionTO.ParameterIndex = 8
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "SENSORS_RECEIVED"
        '        myInstructionTO.Parameter = "G"
        '        myInstructionTO.ParameterIndex = 9
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "SENSORS_RECEIVED"
        '        myInstructionTO.Parameter = "H"
        '        myInstructionTO.ParameterIndex = 10
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "SENSORS_RECEIVED"
        '        myInstructionTO.Parameter = "AC"
        '        myInstructionTO.ParameterIndex = 11
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "SENSORS_RECEIVED"
        '        myInstructionTO.Parameter = "T"
        '        myInstructionTO.ParameterIndex = 12
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "SENSORS_RECEIVED"
        '        myInstructionTO.Parameter = "R"
        '        myInstructionTO.ParameterIndex = 13
        '        Instructions.Add(myInstructionTO)


        '    Catch ex As Exception
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetSENSORDATA_ANSWERInstruction", EventLogEntryType.Error, False)

        '    End Try
        'End Sub

        '' SGM 28/02/11 - Pending to define PDT !!!
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <param name="Instructions"></param>
        '''' <remarks>
        '''' Created by SGM 28/02/11
        '''' </remarks>
        'Private Sub GetTEST_EMPTY_LC_OKInstruction(ByRef Instructions As List(Of InstructionParameterTO))
        '    Try
        '        Dim myInstructionTO As New InstructionParameterTO

        '        'STATUS INSTRUCTION (Fw -> Sw)
        '        ''''''''''''''''''''''''''''''
        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTEMPTYLC_OK"
        '        myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
        '        myInstructionTO.ParameterIndex = 1
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTEMPTYLC_OK"
        '        myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
        '        myInstructionTO.ParameterIndex = 2
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTEMPTYLC_OK"
        '        myInstructionTO.Parameter = "SENSORS"
        '        myInstructionTO.ParameterIndex = 3
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTEMPTYLC_OK"
        '        myInstructionTO.Parameter = "VALUES"
        '        myInstructionTO.ParameterIndex = 4
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTEMPTYLC_OK"
        '        myInstructionTO.Parameter = "T"
        '        myInstructionTO.ParameterIndex = 5
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTEMPTYLC_OK"
        '        myInstructionTO.Parameter = "D"
        '        myInstructionTO.ParameterIndex = 6
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTEMPTYLC_OK"
        '        myInstructionTO.Parameter = "E"
        '        myInstructionTO.ParameterIndex = 7
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTEMPTYLC_OK"
        '        myInstructionTO.Parameter = "F"
        '        myInstructionTO.ParameterIndex = 8
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTEMPTYLC_OK"
        '        myInstructionTO.Parameter = "G"
        '        myInstructionTO.ParameterIndex = 9
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTEMPTYLC_OK"
        '        myInstructionTO.Parameter = "H"
        '        myInstructionTO.ParameterIndex = 10
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTEMPTYLC_OK"
        '        myInstructionTO.Parameter = "AC"
        '        myInstructionTO.ParameterIndex = 11
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTEMPTYLC_OK"
        '        myInstructionTO.Parameter = "T"
        '        myInstructionTO.ParameterIndex = 12
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTEMPTYLC_OK"
        '        myInstructionTO.Parameter = "R"
        '        myInstructionTO.ParameterIndex = 13
        '        Instructions.Add(myInstructionTO)


        '    Catch ex As Exception
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetTEST_EMPTY_LC_OKInstruction", EventLogEntryType.Error, False)

        '    End Try
        'End Sub

        '' SGM 28/02/11 - Pending to define PDT !!!
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <param name="Instructions"></param>
        '''' <remarks>
        '''' Created by SGM 28/02/11
        '''' </remarks>
        'Private Sub GetTEST_FILL_DW_OKInstruction(ByRef Instructions As List(Of InstructionParameterTO))
        '    Try
        '        Dim myInstructionTO As New InstructionParameterTO

        '        'STATUS INSTRUCTION (Fw -> Sw)
        '        ''''''''''''''''''''''''''''''
        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTFILLDW_OK"
        '        myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
        '        myInstructionTO.ParameterIndex = 1
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTFILLDW_OK"
        '        myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
        '        myInstructionTO.ParameterIndex = 2
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTFILLDW_OK"
        '        myInstructionTO.Parameter = "SENSORS"
        '        myInstructionTO.ParameterIndex = 3
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTFILLDW_OK"
        '        myInstructionTO.Parameter = "VALUES"
        '        myInstructionTO.ParameterIndex = 4
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTFILLDW_OK"
        '        myInstructionTO.Parameter = "T"
        '        myInstructionTO.ParameterIndex = 5
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTFILLDW_OK"
        '        myInstructionTO.Parameter = "D"
        '        myInstructionTO.ParameterIndex = 6
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTFILLDW_OK"
        '        myInstructionTO.Parameter = "E"
        '        myInstructionTO.ParameterIndex = 7
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTFILLDW_OK"
        '        myInstructionTO.Parameter = "F"
        '        myInstructionTO.ParameterIndex = 8
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTFILLDW_OK"
        '        myInstructionTO.Parameter = "G"
        '        myInstructionTO.ParameterIndex = 9
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTFILLDW_OK"
        '        myInstructionTO.Parameter = "H"
        '        myInstructionTO.ParameterIndex = 10
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTFILLDW_OK"
        '        myInstructionTO.Parameter = "AC"
        '        myInstructionTO.ParameterIndex = 11
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTFILLDW_OK"
        '        myInstructionTO.Parameter = "T"
        '        myInstructionTO.ParameterIndex = 12
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTFILLDW_OK"
        '        myInstructionTO.Parameter = "R"
        '        myInstructionTO.ParameterIndex = 13
        '        Instructions.Add(myInstructionTO)


        '    Catch ex As Exception
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetTEST_FILL_DW_OKInstruction", EventLogEntryType.Error, False)

        '    End Try
        'End Sub

        '' SGM 28/02/11 - Pending to define PDT !!!
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <param name="Instructions"></param>
        '''' <remarks>
        '''' Created by SGM 28/02/11
        '''' </remarks>
        'Private Sub GetTEST_TRANSFER_DWLC_OKInstruction(ByRef Instructions As List(Of InstructionParameterTO))
        '    Try
        '        Dim myInstructionTO As New InstructionParameterTO

        '        'STATUS INSTRUCTION (Fw -> Sw)
        '        ''''''''''''''''''''''''''''''
        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTTRANSFER_OK"
        '        myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "A"
        '        myInstructionTO.ParameterIndex = 1
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTTRANSFER_OK"
        '        myInstructionTO.Parameter = "" 'myInstructionTO.Parameter = "TYPE"
        '        myInstructionTO.ParameterIndex = 2
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTTRANSFER_OK"
        '        myInstructionTO.Parameter = "SENSORS"
        '        myInstructionTO.ParameterIndex = 3
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTTRANSFER_OK"
        '        myInstructionTO.Parameter = "VALUES"
        '        myInstructionTO.ParameterIndex = 4
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTTRANSFER_OK"
        '        myInstructionTO.Parameter = "T"
        '        myInstructionTO.ParameterIndex = 5
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTTRANSFER_OK"
        '        myInstructionTO.Parameter = "D"
        '        myInstructionTO.ParameterIndex = 6
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTTRANSFER_OK"
        '        myInstructionTO.Parameter = "E"
        '        myInstructionTO.ParameterIndex = 7
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTTRANSFER_OK"
        '        myInstructionTO.Parameter = "F"
        '        myInstructionTO.ParameterIndex = 8
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTTRANSFER_OK"
        '        myInstructionTO.Parameter = "G"
        '        myInstructionTO.ParameterIndex = 9
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTTRANSFER_OK"
        '        myInstructionTO.Parameter = "H"
        '        myInstructionTO.ParameterIndex = 10
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTTRANSFER_OK"
        '        myInstructionTO.Parameter = "AC"
        '        myInstructionTO.ParameterIndex = 11
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTTRANSFER_OK"
        '        myInstructionTO.Parameter = "T"
        '        myInstructionTO.ParameterIndex = 12
        '        Instructions.Add(myInstructionTO)

        '        myInstructionTO = New InstructionParameterTO
        '        myInstructionTO.InstructionType = "TANKTESTTRANSFER_OK"
        '        myInstructionTO.Parameter = "R"
        '        myInstructionTO.ParameterIndex = 13
        '        Instructions.Add(myInstructionTO)


        '    Catch ex As Exception
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "InstructionTypesList.GetTEST_TRANSFER_DWLC_OKInstruction", EventLogEntryType.Error, False)

        '    End Try
        'End Sub

        ' XBC 22/03/11 - Pending to define PDT !!!
        ' XBC 17/05/2011 - delete redundant Events replaced using AnalyzerManager.ReceptionEvent

#End Region


    End Class

End Namespace

