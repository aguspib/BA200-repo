Option Explicit On
Option Strict On

Namespace Biosystems.Ax00.Global
    Public Class AlarmEnumerates
        'Instrument Alarm list
        Public Enum Alarms
            'Instrument Alarms!!!
            NONE 'Null alarm. Defined for Sw requirements

            ''''''''''''''''
            'IMPORTANT NOTE: Every alarm added into this enumerate must be also added into method ConvertToAlarmIDEnumerate (in AnalyzerManager class) select case
            ''''''''''''''''

            'GLOBE ALARMS INTO UI MONITOR (MAIN TAB)!!!
            '====================
            'Analyzer CoverAlert 
            MAIN_COVER_ERR 'WARNING! Fw indicates Ax00 main cover is open while instrument working (FW ERROR CODE: 70) 'AG 01/03/2012
            MAIN_COVER_WARN 'WARNING! Fw indicates Ax00 main cover is open (ANSINF)

            'WashingSolutionAlert 
            WASH_CONTAINER_ERR 'ERROR! Sw generates this error when the washing solution container level is lower than a critical limit (ANSINF)
            WASH_CONTAINER_WARN 'WARNING! Sw generates this warning when the washing solution container level is lower than a warning limit (ANSINF)

            'ResiduesBalanceAlert 
            HIGH_CONTAMIN_ERR 'ERROR! Sw generates this error when the high contaminated container level is higher than a critical limit (ANSINF)
            HIGH_CONTAMIN_WARN 'WARNING! Sw generates this warning when the high contaminated container level is higher than a warning limit (ANSINF)

            'Reagents1ArmAlert 
            R1_TEMP_WARN 'WARNING! Sw calculates if the R1 thermo is out of limits too much time (ANSINF)

            'DL 31/07/2012. Begin
            R1_TEMP_SYSTEM_ERR 'ERROR! Fw indicates the R1 thermo system is damaged (AG 04/01/2012 NEW) (FW ERROR CODE: 201, 202)
            'Substitute R1_TEMP_SYSTEM_ERR by R1_TEMP_SYS1_ERR and R1_TEMP_SYS2_ERR
            'R1_TEMP_SYS1_ERR 'ERROR! Fw indicates the fridge thermo system is damaged  (FW ERROR CODE: 201)
            'R1_TEMP_SYS2_ERR 'ERROR! Fw indicates the fridge thermo system is damaged  (FW ERROR CODE: 202)
            'DL 31/07/2012. End

            R1_DETECT_SYSTEM_ERR 'ERROR! Fw indicates the level detection system in R1 arm does not work properly (FW ERROR CODE: 200)
            R1_NO_VOLUME_WARN 'WARNING! Fw indicates the arm R1 has not detected liquid (ANSBR1)
            R1_COLLISION_ERR 'ERROR! Fw indicates a collision with the R1 arm (FW ERROR CODE: 203) 'AG 01/03/2012
            R1_COLLISION_WARN 'WARNING! Fw indicates a collision with the R1 arm (ANSBR1)

            'ReagentsRotorAlert
            FRIDGE_COVER_ERR 'WARNING! Fw indicates Ax00 reagents cover is open while instrument working (FW ERROR CODE: 305) 'AG 01/03/2012
            FRIDGE_COVER_WARN 'WARNING! Fw indicates Ax00 reagents cover is open (ANSINF)
            FRIDGE_TEMP_WARN 'WARNING! Sw generates this warning when Tº is out of limits (ANSINF)
            FRIDGE_TEMP_ERR 'ERROR! Sw generates this error when Tº is out of limits too much time (ANSINF)

            'DL 31/07/2012. Begin
            FRIDGE_TEMP_SYS_ERR 'ERROR! Fw indicates the fridge thermo system is damaged (AG 04/01/2012 NEW) (NOTE the FRIDGE_TEMP_SYSTEM_ERR alarmID is too long) (FW ERROR CODE: 301, 302)
            'Substitute FRIDGE_TEMP_SYS_ERR by FRIDGE_TEMP_SYS1_ERR and FRIDGE_TEMP_SYS2_ERR
            'FRIDGE_TEMP_SYS1_ERR 'ERROR! Fw indicates the fridge thermo system is damaged  (FW ERROR CODE: 301)
            'FRIDGE_TEMP_SYS2_ERR 'ERROR! Fw indicates the fridge thermo system is damaged  (FW ERROR CODE: 302)
            'DL 31/07/2012. End

            FRIDGE_STATUS_WARN 'WARNING! Fw indicates the fridge is OFF (ANSINF)
            FRIDGE_STATUS_ERR 'ERROR! Fw indicates the fridge does not work properly (FW ERROR CODE: 301, 302 or 303: FRIDGE_TEMP_SYS_ERR, FRIDGE_FAN_WARN)

            'Reagents2ArmAlert
            R2_TEMP_WARN 'WARNING! Sw calculates if the R2 thermo is out of limits (ANSINF)
            'DL 31/07/2012. Begin
            R2_TEMP_SYSTEM_ERR 'ERROR! Fw indicates the R2 thermo system is damaged (AG 04/01/2012 NEW) (FW ERROR CODE: 211, 212)
            'Substitute R2_TEMP_SYSTEM_ERR by R2_TEMP_SYS1_ERR and R2_TEMP_SYS2_ERR
            'R2_TEMP_SYS1_ERR 'ERROR! Fw indicates the fridge thermo system is damaged  (FW ERROR CODE: 211)
            'R2_TEMP_SYS2_ERR 'ERROR! Fw indicates the fridge thermo system is damaged  (FW ERROR CODE: 212)
            'DL 31/07/2012. End

            R2_DETECT_SYSTEM_ERR 'ERROR! Fw indicates the level detection system in R2 arm does not work properly (FW ERROR CODE: 210)
            R2_NO_VOLUME_WARN 'WARNING! Fw indicates the arm R2 has not detected liquid (ANSBR2)
            R2_COLLISION_ERR 'ERROR! Fw indicates a collision with the R2 arm (FW ERROR CODE: 213) 'AG 01/03/2012
            R2_COLLISION_WARN 'WARNING! Fw indicates a collision with the R2 arm (ANSBR2)

            'OthersAlert
            WATER_DEPOSIT_ERR 'ERROR! Sw generates this error when a waiting time is expired and the water deposit continues empty (ANSINF)
            WATER_SYSTEM_ERR 'ERROR! Fw indicates the water deposit entrace system is damaged (AG 04/01/2012 NEW) (NOTE the WATER_DEPOSIT_SYSTEM_ERR alarmID it too long) (FW ERROR CODE: 630, 631)
            WASTE_DEPOSIT_ERR 'ERROR! Sw generates this error when a waiting time is expired and the waste deposit continues full (ANSINF)
            WASTE_SYSTEM_ERR 'ERROR! Fw indicates the water deposit entrace system is damaged (AG 04/01/2012 NEW) (NOTE the WASTE_DEPOSIT_SYSTEM_ERR alarmID it too long) (FW ERROR CODE: 620, 621)
            REACT_ROTOR_FAN_WARN 'WARNING! Fw indicates some reactions rotor's fan are not working properly (FW ERROR CODE: 512)
            FRIDGE_FAN_WARN 'WARNING! Fw indicates some fridge's fan are not working properly (FW ERROR CODE: 303)

            'ReactionsRotorAlert
            REACT_COVER_ERR 'WARNING! Fw indicates Ax00 reactions cover is open while instrument working (FW ERROR CODE: 503) 'AG 01/03/2012
            REACT_COVER_WARN 'WARNING! Fw indicates Ax00 reactions cover is open (ANSINF + in Running also with FW ERROR CODE 503)
            REACT_MISSING_ERR 'ERROR! Fw indicates not exits a methacrylate reactions rotor (FW ERROR CODE 550)
            REACT_ENCODER_ERR 'ERROR! Fw indicates an encoder error (FW ERROR CODE: 501)
            REACT_TEMP_WARN 'WARNING! Sw generates this warning when Tº is out of limits (ANSINF)
            REACT_TEMP_ERR 'ERROR! Sw generates this error when Tº is out of limits too much time or Thermo system is damaged (ANSINF)
            REACT_TEMP_SYS_ERR 'ERROR! Fw indicates the reactions thermo system is damaged (AG 04/01/2012 NEW) (NOTE the REACT_TEMP_SYSTEM_ERR alarmID it too long) (FW ERROR CODE: 510, 511)

            REACT_SAFESTOP_ERR 'ERROR! Fw indicates a reactions rotor safety stop because the washing station is down (FW ERROR CODE: 502)
            'WashingStationAlert
            WS_TEMP_WARN 'WARNING! Sw generates this warning when Tº is out of limits (ANSINF)
            WS_TEMP_SYSTEM_ERR 'ERROR! Fw indicates the washing station thermo system is damaged (AG 04/01/2012 change alarmID from WS_TEMP_ERR to WS_TEMP_SYSTEM_ERR) (FW ERROR CODE: 610, 611)

            WS_COLLISION_ERR 'ERROR! Fw indicates a collision with the washing station (FW ERROR CODE: 504)

            'SampleArmAlert
            CLOT_DETECTION_ERR 'ERROR! Fw indicates exits clot detection (ANSBM1)
            CLOT_DETECTION_WARN 'WARNING! Fw indicates possible clot detection (ANSBM1)
            CLOT_SYSTEM_ERR 'WARNING! Fw indicate possible clot error (FW ERROR CODE: 710) 'DL 13/06/2012
            S_DETECT_SYSTEM_ERR 'ERROR! Fw indicates the level detection system in S arm does not work properly (FW ERROR CODE 220)
            S_NO_VOLUME_WARN 'WARNING! Fw indicates the arm S has not detected liquid (ANSBM1)
            DS_NO_VOLUME_WARN 'WARNING! Fw indicates the arm S has not detected diluted sample liquid inside reactions rotor (ANSBM1)
            S_COLLISION_ERR 'ERROR! Fw indicates a collision with the S arm (FW ERROR CODE: 223) 'AG 01/03/2012
            S_COLLISION_WARN 'WARNING! Fw indicates a collision with the S arm (ANSBM1)
            S_OBSTRUCTED_ERR 'ERROR!  Fw indicates Sample Fluidics Obstructed (FW ERROR CODE: 711) 'AG 12/03/2012

            'SamplesRotorCoverAlert
            S_COVER_ERR 'WARNING! Fw indicates Ax00 samples cover is open while instrument working (FW ERROR CODE: 355) 'AG 01/03/2012
            S_COVER_WARN 'WARNING! Fw indicates Ax00 samples cover is open (ANSINF)

            'Homes
            R1_H_HOME_ERR 'ERROR! Fw indicates the Reagent1 arm home is damaged (horizontal) (FW ERROR CODE: 101)
            R1_V_HOME_ERR 'ERROR! Fw indicates the Reagent1 arm home is damaged (vertical) (FW ERROR CODE: 100)
            R2_H_HOME_ERR 'ERROR! Fw indicates the Reagent2 arm home is damaged (horizontal)(FW ERROR CODE: 111)
            R2_V_HOME_ERR 'ERROR! Fw indicates the Reagent2 arm home is damaged (vertical) (FW ERROR CODE: 110)
            S_H_HOME_ERR 'ERROR! Fw indicates the Sample arm home is damaged (horizontal)(FW ERROR CODE: 121)
            S_V_HOME_ERR 'ERROR! Fw indicates the Sample arm home is damaged (vertical)(FW ERROR CODE: 120)
            STIRRER1_H_HOME_ERR 'ERROR! Fw indicates the stirrer1 arm home is damaged (horizontal)(FW ERROR CODE: 131)
            STIRRER1_V_HOME_ERR 'ERROR! Fw indicates the stirrer1 arm home is damaged (vertical) (FW ERROR CODE: 130)
            STIRRER2_H_HOME_ERR 'ERROR! Fw indicates the stirrer2 arm home is damaged (horizontal)(FW ERROR CODE: 141)
            STIRRER2_V_HOME_ERR 'ERROR! Fw indicates the stirrer2 arm home is damaged (horizontal)(FW ERROR CODE: 140)
            FRIDGE_HOME_ERR 'ERROR! Fw indicates the Fridge (reagents) rotor home is damaged (FW ERROR CODE: 300)
            SAMPLES_HOME_ERR 'ERROR! Fw indicates the samples rotor home is damaged (FW ERROR CODE: 350)
            REACTIONS_HOME_ERR 'ERROR! Fw indicates the reactions rotor home is damaged (FW ERROR CODE: 500)
            WS_HOME_ERR 'ERROR! Fw indicates the washing station home is damaged (FW ERROR CODE: 520)
            WS_PUMP_HOME_ERR 'ERROR! Fw indicates the washing station PUMP home is damaged (FW ERROR CODE: 600)
            R1_PUMP_HOME_ERR 'ERROR! Fw indicates the reagent1 PUMP home is damaged (FW ERROR CODE: 701)
            R2_PUMP_HOME_ERR 'ERROR! Fw indicates the reagent2 PUMP home is damaged (FW ERROR CODE: 702)
            S_PUMP_HOME_ERR 'ERROR! Fw indicates the sample PUMP home is damaged (FW ERROR CODE: 700)

            'Internal instrument alarms
            INST_REJECTED_ERR '(OK v0.4) ERROR! Instruction Rejected by a previous Error (Permanent or Self-recover like covers) (Stand by-Running) (FW ERROR CODE: 20) 'AG 12/03/2012
            INST_ABORTED_ERR '(OK v0.4) ERROR! Fw indicates Instruction Aborted by an Error (Stand by-Running) (FW ERROR CODE: 21) 'AG 01/03/2012
            RECOVER_ERR '(OK v0.4) ERROR! Fw indicates Recover Failed (FW ERROR CODE: 22) 'AG 01/03/2012
            INST_REJECTED_WARN '(OK v0.4) WARNING! Fw indicates Impossible to perform TEST/PTEST/WRUN; Wait to next Cycle (FW ERROR CODE: 28) 'AG 01/03/2012
            '                   (OK v0.4) WARNING! Fw indicates Impossible to perform TEST/PTEST/WRUN; Already received one in the same cycle (FW ERROR CODE: 34)
            PH_BOARD_ERR '(OK v0.4)ERROR! Fw indicates not photometric board detected (FW ERROR CODE: 530) 'AG 01/03/2012
            GLF_RESET_ERR '(OK v0.4) ERROR! Fw indicates photometric board reset (FW ERROR CODE: 599) 'AG 01/03/2012
            SFX_RESET_ERR '(OK v0.4) ERROR! Fw indicates fluid system board reset (FW ERROR CODE: 699) 'AG 01/03/2012
            JEX_RESET_ERR '(OK v0.4) ERROR! Fw indicates manifold board reset (FW ERROR CODE: 799) 'AG 01/03/2012

            R1_BOARD_ERR            '(OK v0.5.0) R1 board not responding                | FW ERROR CODE: 108 | DL 13/06/2012
            R1_RESET_ERR            '(OK v0.5.0) R1 board reset                         | FW ERROR CODE: 109 | DL 13/06/2012
            R2_BOARD_ERR            '(OK v0.5.0) R2 board not responding                | FW ERROR CODE: 118 | DL 13/06/2012
            R2_RESET_ERR            '(OK v0.5.0) R2 board reset                         | FW ERROR CODE: 119 | DL 13/06/2012
            S_BOARD_ERR             '(OK v0.5.0) Samples arm board not responding       | FW ERROR CODE: 128 | DL 13/06/2012
            S_RESET_ERR             '(OK v0.5.0) Samples arm board reset                | FW ERROR CODE: 129 | DL 13/06/2012
            STIRRER1_BOARD_ERR      '(OK v0.5.0) Stirrer1 board not responding          | FW ERROR CODE: 138 | DL 13/06/2012
            STIRRER1_RESET_ERR      '(OK v0.5.0) Stirrer1 board reset                   | FW ERROR CODE: 139 | DL 13/06/2012
            STIRRER2_BOARD_ERR      '(OK v0.5.0) Stirrer2 board not responding          | FW ERROR CODE: 148 | DL 13/06/2012
            STIRRER2_RESET_ERR      '(OK v0.5.0) Stirrer2 board reset                   | FW ERROR CODE: 149 | DL 13/06/2012
            R1_DETECT_BOARD_ERR     '(OK v0.5.0) R1 detection board not responding      | FW ERROR CODE: 208 | DL 13/06/2012
            R1_DETECT_RESET_ERR     '(OK v0.5.0) R1 detection board reset               | FW ERROR CODE: 209 | DL 13/06/2012
            R2_DETECT_BOARD_ERR     '(OK v0.5.0) R2 detection board not responding      | FW ERROR CODE: 218 | DL 13/06/2012
            R2_DETECT_RESET_ERR     '(OK v0.5.0) R2 detection board reset               | FW ERROR CODE: 219 | DL 13/06/2012
            S_DETECT_BOARD_ERR      '(OK v0.5.0) Samples detection board not responding | FW ERROR CODE: 228 | DL 13/06/2012
            S_DETECT_RESET_ERR      '(OK v0.5.0) Samples detection board reset          | FW ERROR CODE: 229 | DL 13/06/2012
            FRIDGE_BOARD_ERR        '(OK v0.5.0) Reagents rotor board not responding    | FW ERROR CODE: 348 | DL 13/06/2012
            FRIDGE_RESET_ERR        '(OK v0.5.0) Reagents rotor board reset             | FW ERROR CODE: 349 | DL 13/06/2012
            S_ROTOR_BOARD_ERR       '(OK v0.5.0) Samples rotor board not responding     | FW ERROR CODE: 398 | DL 13/06/2012
            S_ROTOR_RESET_ERR       '(OK v0.5.0) Samples rotor board reset              | FW ERROR CODE: 399 | DL 13/06/2012
            GLF_BOARD_ERR           '            Photometric board not responding       | FW ERROR CODE: 560 | AC 19/03/2015
            SFX_BOARD_ERR           '(OK v0.5.0) Fluid system board not responding      | FW ERROR CODE: 698 | DL 13/06/2012
            JEX_BOARD_ERR           '(OK v0.5.0) Manifold board not responding          | FW ERROR CODE: 798 | DL 13/06/2012

            'ISE module
            'ISE_STATUS_WARN 'WARNING! Fw indicates the ISE module is OFF (ANSINF)
            'ISE_STATUS_ERR 'ERROR! Fw generates this error when ISE module is damaged (FW ERROR CODE: 60)
            ISE_OFF_ERR 'WARNING! Fw indicates the ISE module is OFF (ANSINF)
            ISE_FAILED_ERR 'ERROR! Fw generates this error when ISE module is damaged (FW ERROR CODE: 60)
            ISE_TIMEOUT_ERR 'ERROR! Fw indicates ISE timeout (FW ERROR CODE: 61) 'AG 01/03/2012

            ISE_CONNECT_PDT_ERR 'ERROR! Sw generates this warning when ISE module is switched On but it is required that user connects it from Utilities  (Software business)
            ISE_RP_INVALID_ERR 'ERROR! Sw generates this error when Reagents Pack has no valid distributor code (Software business)
            ISE_RP_DEPLETED_ERR 'ERROR! Sw generates this error when Reagents Pack has no calibrators (A or B) volume enough (Software business)
            ISE_ELEC_WRONG_ERR 'ERROR! Sw generates this error when Electrodes are wrong installed (Software business)
            ISE_CP_INSTALL_WARN 'WARNING! Sw generates this warning when Clean Pack has been installed (Software business)
            ISE_CP_WRONG_ERR 'ERROR! Sw generates this error when Clean Pack is wrong installed (Software business)
            ISE_LONG_DEACT_ERR 'ERROR! Sw generates this warning when Ise Module is Deactivated for long term (Software business)
            ISE_RP_EXPIRED_WARN 'WARNING! Sw generates this warning when Reagents Pack is expired by date (Software business)
            ISE_ELEC_CONS_WARN 'WARNING! Sw generates this warning when Electrodes are expired by consumption (Software business)
            ISE_ELEC_DATE_WARN 'WARNING! Sw generates this warning when Electrodes are expired by date (Software business)
            ISE_ACTIVATED 'WARNING! Sw generates this warning when ISE module is Activated (Software business)
            ISE_RP_NO_INST_ERR ' ERROR! Sw generates this error when Reagents Pack is wrong installed (Software business)

            ISE_CALB_PDT_WARN   ' CALIBS WARNING! Sw generates this warning when ISE Electrodes calibration is not performed or is expired - BA-1873
            ISE_PUMP_PDT_WARN   ' CALIBS WARNING! Sw generates this warning when ISE Pumps calibration is not performed or is expired - BA-1873
            ISE_CLEAN_PDT_WARN  ' CALIBS WARNING! Sw generates this warning when ISE Clean is not performed or is expired - BA-1873

            'Software business alarms
            '========================
            BASELINE_INIT_ERR 'ERROR! Sw generates this error when the base line calculations determine a Baseline error (initialization)
            BASELINE_WELL_WARN 'ERROR! Sw generates this warning when the base line calculations determine a new reactions rotor is recommended 'AG 06/06/2012 change alarmID (old alarm ID was METHACRYL_ROTOR_WARN)
            PREP_LOCKED_WARN 'WARNING! Sw indicates all executions have been locked due volume missing (same as Ax5)
            COMMS_ERR 'ERROR! Communications error (AG 17/10/2011)
            REPORTSATLOADED_WARN 'Informative alarm - a reportsat has been loaded (RH 07/02/2012)

            'LOCKED REAGENT BOTTLE ALARM 
            BOTTLE_LOCKED_WARN 'Reagent bottle is locked due incorrect refill detected TR 01/01/2012. 

            'BT #1355 ==> ANALYZER ENTERS/LEAVES PAUSE MODE
            WS_PAUSE_MODE_WARN

            'SERVICE
            ADJUST_NO_EXIST

            'ISE Independent Errors SGM 25/07/2012

            ISE_ERROR_S 'Air in Sample/Urine
            ISE_ERROR_A 'Air in Calibrant A
            ISE_ERROR_B 'Air in Calibrant B
            ISE_ERROR_C 'Air in Cleaner
            ISE_ERROR_M 'Air in Segment
            ISE_ERROR_P 'Pump Cal
            ISE_ERROR_F 'No Flow
            ISE_ERROR_D 'Bubble Detector
            ISE_ERROR_R 'Dallas Read
            ISE_ERROR_W 'Dallas Write
            ISE_ERROR_T 'Invalid Command
            ISE_ERROR_N 'Reagent Pack is missing

            ISE_CALIB_ERROR 'Result error trated as alarm in case of calibrations

            'FW and Instructions
            FW_CPU_ERR                  'CPU Firmware Error                         | FW ERROR CODE:   1 | DL 27/07/2012 
            FW_DISTRIBUTED_ERR          'Distributed Firmware Error                 | FW ERROR CODE:   2 | DL 27/07/2012 
            FW_REPOSITORY_ERR           'Repository Firmware Error                  | FW ERROR CODE:   3 | DL 27/07/2012 
            FW_CHECKSUM_ERR             'Checksum Firmware Error                    | FW ERROR CODE:   4 | DL 27/07/2012 
            FW_INTERNAL_ERR             'Internal Execution Firmware Error          | FW ERROR CODE:   5 | DL 27/07/2012 
            FW_MAN_ERR                  'Maneuvers or Adjustments Firmware Error    | FW ERROR CODE:   6 | DL 27/07/2012 
            FW_CAN_ERR                  'CAN Bus Error                              | FW ERROR CODE:  10 | DL 27/07/2012 
            INST_UNKOWN_ERR             'Unkown Instruction                         | FW ERROR CODE:  40 | DL 27/07/2012 
            INST_NOALLOW_STA_ERR        'Not Allowed Instruction (state)            | FW ERROR CODE:  41 | DL 27/07/2012 
            INST_NOALLOW_INS_ERR        'Not Allowed Instruction (inst)             | FW ERROR CODE:  42 | DL 27/07/2012 
            INST_COMMAND_WARN           'Error with Command script                  | FW ERROR CODE:  43 | DL 27/07/2012 
            INST_LOADADJ_WARN           'Error Loading Adjustments                  | FW ERROR CODE:  45 | DL 27/07/2012 

            COMMS_TIMEOUT_ERR           'XB 06/11/2014 - BA-1872
            ''''''''''''''''
            'IMPORTANT NOTE: Every alarm added into this enumerate must be also added into method ConvertToAlarmIDEnumerate (in AnalyzerManager class) select case
            ''''''''''''''''


            '***********************
            'Calculations remarks!!!
            '***********************
            'AG 05/12/2014 BA-2236 code improvement
            'Calculations remarks!!!. In previous versions there were defined in enum CaalculationRemarks but it is better to unify this enum
            'because every alarm added into this enumerate must be also added into method ConvertToAlarmIDEnumerate (in AnalyzerManager class) select case
            'Now I have improve the method ConvertToAlarmIDEnumerate that automatically converts all alarms from String to enum but it demands use all alarms defined in tfmwAlarms exists in the same enumerate

            'Generic absorbance remarks
            ABS_REMARK1 'Abs > Optical Limit
            ABS_REMARK2 'Sample Abs < Blank Abs
            ABS_REMARK3 'Sample Abs > Blank Abs
            ABS_REMARK4 'Non Linear Kinetics
            ABS_REMARK5 'Absorbance < 0
            ABS_REMARK6 'Absorbance increase < 0
            ABS_REMARK7 'Substrate depletion sample
            ABS_REMARK8 'Prozone sample possible (dilute manually and repeat)
            ABS_REMARK9 'Reactions Rotor Thermo warning (NEW 22/03/2011)
            ABS_REMARK10 'Possible clot (NEW 22/03/2011)
            ABS_REMARK11 'clot detected (NEW 31/03/2011)
            ABS_REMARK12 'Fluid system blocked (NEW 31/03/2011)
            ABS_REMARK13 'Finished with optical errors (NEW 21/10/2011)

            'Blank Remarks
            BLANK_REMARK1 'Main Abs > Blank Abs Limit
            BLANK_REMARK2 'Main Abs < Blank Abs Limit
            BLANK_REMARK3 'Abs Work Reagent > Blank Abs Limit
            BLANK_REMARK4 'Abs Work Reagent < Blank Abs Limit
            BLANK_REMARK5 'Blank Abs Initial > Blank Abs Limit
            BLANK_REMARK6 'Blank Abs Initial < Blank Abs Limit
            BLANK_REMARK7 'Kinetic blank > Kinetic Blank Limit
            BLANK_REMARK8 '(Abs T2 - Abs T1) * RT > Kinetic Blank Limit

            'Calibration Remarks
            CALIB_REMARK1 'Incorrect curve
            CALIB_REMARK2 'Calculated Factor beyond limits
            CALIB_REMARK3 'Calibration factor NOT calculated
            CALIB_REMARK4 'Calibration Lot Expired

            'Concentration Remarks
            CONC_REMARK1 'Conc NOT calculated
            CONC_REMARK2 'CONC out of calibration curve (HIGH)
            CONC_REMARK3 'CONC out of calibration curve (LOW)
            CONC_REMARK4 'Conc < 0
            CONC_REMARK5 'Conc > Linearity Limit
            CONC_REMARK6 'Conc < Detection Limit
            CONC_REMARK7 'Conc out of Normality Range
            CONC_REMARK8 'H
            CONC_REMARK9 'BH
            CONC_REMARK10 'B
            CONC_REMARK11 'Some standard tests with remarks

            ''ISE Remarks
            'ISE_REMARK1 '????
            'ISE_REMARK2 'mV Out n.2
            'ISE_REMARK3 'mV Out n.3
            'ISE_REMARK4 'mV Noise n.4
            'ISE_REMARK5 'mV Noise n.5
            'ISE_REMARK6 'Cal A Drift
            'ISE_REMARK7 'Out of Slope/Machine Ranges

            'ISE results remarks SGM 25/07/2012
            ISE_mVOutB
            ISE_mVOutA_SER
            ISE_mVOutB_URI
            ISE_mVNoiseB
            ISE_mVNoiseA_SER
            ISE_mVNoiseB_URI
            ISE_Drift_SER
            ISE_Drift_CAL
            ISE_OutSlope

            QC_OUT_OF_RANGE 'AG 20/07/2012
        End Enum
    End Class
End Namespace
