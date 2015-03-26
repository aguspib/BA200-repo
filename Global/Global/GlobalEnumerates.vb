Option Explicit On
Option Strict On

Namespace Biosystems.Ax00.Global
    Public Class GlobalEnumerates

#Region "Database Tables Enumerates"
        ''' <summary>
        ''' Define the Enumeration for User Settings 
        ''' </summary>
        ''' <remarks>
        ''' Created by: VR 29/12/2009 
        ''' </remarks>
        Public Enum UserSettingsEnum
            AUT_RESULTS_PRINT
            AUT_RESULTS_FREQ 'AG 26/09/2013 (auto report functionality integration)
            AUT_RESULTS_TYPE 'AG 26/09/2013 (auto report functionality integration)
            AUTOMATIC_EXPORT
            AUTOMATIC_RERUN
            AUTOMATIC_RESET
            CURRENT_LANGUAGE
            CURRENT_LANGUAGE_SRV    ' XBC 12/11/2012
            DEFAULT_TUBE_BLANK
            DEFAULT_TUBE_CALIB
            DEFAULT_TUBE_CTRL
            DEFAULT_TUBE_PATIENT
            EXPORT_FREQUENCY
            BARCODE_EXTERNAL_END
            BARCODE_EXTERNAL_INI
            BARCODE_FULL_TOTAL
            BARCODE_SAMPLETYPE_END
            BARCODE_SAMPLETYPE_FLAG
            BARCODE_SAMPLEID_FLAG           'TR 07/03/2013
            BARCODE_SAMPLETYPE_INI
            BARCODE_BEFORE_START_WS         'DL 21/07/2011
            RESETWS_DOWNLOAD_ONLY_PATIENTS  'DL 04/08/2011
            'ONLINE_EXPORT                  'DL 19/10/2011 comment at 09/10/2012 by DL
            ' LIS settings (XB 05/03/2013)
            LIS_HOST_QUERY
            LIS_ACCEPT_UNSOLICITED
            LIS_DOWNLOAD_ONRUNNING
            LIS_WORKING_MODE_RERUNS
            LIS_WORKING_MODE_REFLEX_TESTS
            LIS_UPLOAD_UNSOLICITED_PAT_RES
            LIS_UPLOAD_UNSOLICITED_QC_RES
            LIS_ENABLE_COMMS
            LIS_DATA_TRANSMISSION_TYPE
            LIS_HOST_NAME
            LIS_TCP_PORT
            LIS_TCP_PORT2
            LIS_STORAGE_RECEPTION_MAX_MSG
            LIS_STORAGE_TRANS_MAX_MSG
            LIS_BAx00maxTimeToRespond
            LIS_PROTOCOL_NAME
            LIS_IHE_COMPLIANT
            LIS_CODEPAGE
            LIS_HOST_QUERY_PACKAGE
            LIS_maxTimeWaitingForResponse
            LIS_maxTimeWaitingForACK
            LIS_HOST_ID
            LIS_HOST_PROVIDER
            LIS_INSTRUMENT_ID
            LIS_INSTRUMENT_PROVIDER
            LIS_FIELD_SEPARATOR_HL7     ' XB 15/05/2013
            LIS_COMP_SEPARATOR_HL7      ' XB 15/05/2013
            LIS_REP_SEPARATOR_HL7       ' XB 15/05/2013
            LIS_SPEC_SEPARATOR_HL7      ' XB 15/05/2013
            LIS_SUBC_SEPARATOR_HL7      ' XB 15/05/2013
            LIS_FIELD_SEPARATOR_ASTM    ' XB 15/05/2013
            LIS_COMP_SEPARATOR_ASTM     ' XB 15/05/2013
            LIS_REP_SEPARATOR_ASTM      ' XB 15/05/2013
            LIS_SPEC_SEPARATOR_ASTM     ' XB 15/05/2013
            LIS_LOG_FOLDER
            LIS_LOW_LEVEL_LOG
            LIS_WITHFILES_MODE 'AG 03/04/2013
            LIS_TRACE_LEVEL 'TR 22/04/2013
            AUTO_WS_WITH_LIS_MODE 'SG 11/07/2013
            AUTO_LIS_WAIT_TIME 'SG 11/07/2013
        End Enum


        ''' <summary>
        ''' Define the Enumeration for Analyzer Settings 
        ''' </summary>
        ''' <remarks>
        ''' Created by: DL 21/07/2011
        ''' </remarks>
        Public Enum AnalyzerSettingsEnum
            COMM_PORT
            COMM_AUTO
            COMM_SPEED
            REAGENT_BARCODE_DISABLED
            SAMPLE_BARCODE_DISABLED
            WUPCOMPLETEFLAG
            WUPSTARTDATETIME
            ALARM_DISABLED
            WATER_TANK


            'AG 27/10/2011 - Commented, they are save as adjustments not in AnalyzerSettings table [Service]
            'GRL_ANALYZER_COVER
            'REACTION_ROTOR_COVER
            'SAMPLES_ROTOR_COVER
            'REAGENTS_ROTOR_COVER
            'CLOT_DETECTION

        End Enum


        ''' <summary>
        ''' Define the Enumeration for Master Data
        ''' </summary>
        ''' <remarks>
        ''' Created by: VR 29/12/2009 
        ''' </remarks>
        Public Enum MasterDataEnum
            MASTER
            QUALITATIVE_RES
            SAMPLE_TYPES
            TEST_UNITS
        End Enum

        ''' <summary>
        ''' Define the Enumeration for General Settings
        ''' </summary>
        ''' <remarks>
        ''' Created by:  VR 29/12/2009 
        ''' Modified by: SA 11/11/2014 - BA-1885 ==> Item MAX_QCRESULTS_TO_ACCUMULATE removed from the enumerated 
        ''' </remarks>
        Public Enum GeneralSettingsEnum
            ACCESS_CONTROL_ACTIVATION
            BIG_BOTTLE_MIN_VOLUME
            CAL_EXPIR_DATE_DAYNUMBER
            EXT_SAMPLEID_MAXSIZE 'AG 03/09/2014 - complete enum
            EXT_SAMPLETYPE_MAXSIZE 'AG 03/09/2014 - complete enum
            INITIAL_KEY
            INTERVAL_ABS_T
            LAST_TEST_REPORT_POS 'AG 03/09/2014 - complete enum
            MAX_CUMULATED_QCSERIES
            MAX_DAYS_PREVIOUS_BLK_CALIB
            MAX_NEW_TESTS_ALLOWED
            MAX_PATIENT_ORDER_TESTS
            SUPERVISOR_CREATED_TEST_COUNT 'AG 03/09/2014 - complete enum
            MAX_QCRESULTS_TO_ACCUMULATE
            FLIGHT_FULL_ROTOR_CADUCITY
        End Enum

        ''' <summary>
        ''' Define the Enumeration for Preloaded Master Data
        ''' </summary>
        ''' <remarks>
        ''' Created by: VR 29/12/2009 
        ''' </remarks>
        Public Enum PreloadedMasterDataEnum
            AGE_UNITS
            ALARM_SOURCES
            ALARM_TYPES
            ANALYSIS_MODES
            AUT_RESULTS_FREQ 'AG 26/09/2013 (auto report functionality integration)
            AUT_RESULTS_TYPE 'AG 26/09/2013 (auto report functionality integration)
            BARCODE_STATUS
            BLANK_MODES
            BLOOD_TYPES
            BOTTLE_STATUS
            CALIBRATION_TYPES
            CONTAMINATED_TYPES
            CONTAMINATION_TYPES
            CURVE_AXIS_TYPES
            CURVE_GROWTH_TYPES
            CURVE_TYPES
            DEMOGRAPHIC_TYPES
            DIL_SOLUTIONS 'AG 17/01/2011
            ELEMENT_STATUS
            EXECUTION_STATUS
            EXECUTION_TYPE
            EXPORT_FREQUENCY
            EXPORT_STATUS
            ICON_PATHS
            LANGUAGES
            LIS_TRACE_LEVEL 'TR 22/04/2013
            ORDER_STATUS
            ORDER_TEST_STATUS
            PATIENT_TYPES
            POSTDILUTION_TYPE
            PREDILUTION_MODES
            QC_CALC_MODES
            QC_MULTIRULES
            RANGE_SUBTYPES
            RANGE_TYPES
            REACTION_TYPES
            READING_MODES
            REAGENT_POS_STATUS
            RESULT_TYPE
            ROTOR_TYPES
            SAMPLE_CLASSES
            SAMPLE_POS_STATUS
            SEX_LIST
            SPECIAL_SOLUTIONS
            TEST_TYPES
            TREE_LEVEL1_TITLES
            TREE_LEVEL2_TITLES
            TREE_LEVEL4_TITLES
            TUBE_CONTENTS
            TUBE_TYPES_SAMPLES
            VALIDATION_STATUS
            WASHING_SOLUTIONS
            WAVELENGTHS
            WELLCONTENT
            WELLSTATUS
            WORKSESSION_STATUS
            FREEZE_MODES
            XML_MSG_STATUS 'AG 28/02/2013

            'SERVICE SOFTWARE
            SRV_SAMPLE_POSITIONS
            SRV_REAG_POSITIONS
            SRV_MIXER_POSITIONS
            SRV_ARM_MOVEMENTS
            SRV_OPTIC_CENTERING
            SRV_WASHING_STATION
            SRV_PHOTOMETRY_TESTS
            SRV_FILL_MODE
            SRV_STRESS_ARMS
            SRV_STRESS_ROTOR
            SRV_STRESS_PHOTO
            SRV_STRESS_SYRINGE
            SRV_STRESS_FLUID
            SRV_TASK_TYPES
            SRV_ACT_ADJ_ALL
            SRV_ACT_ADJ_TYPES
            SRV_ACT_TEST_TYPES
            SRV_ACT_UTIL_TYPES

            'ISE ACTIONS
            ISE_ACT_USR_INITIAL
            ISE_ACT_USR_REQUEST
            ISE_ACT_USR_GENERAL
            ISE_ACT_USR_PUMP_TUB
            ISE_ACT_USR_FLUI_TUB
            ISE_ACT_USR_CALIBR
            ISE_ACT_USR_INS_MODL
            ISE_ACT_USR_INS_REAG
            ISE_ACT_USR_INS_ELEC
            ISE_ACT_USR_DEACT
            ISE_ACT_USR_ACT
            ISE_ACT_USR_PREPOK
            ISE_ACT_USR_ALM 'SGM 26/10/2012

            ISE_ACT_SRV_INITIAL
            ISE_ACT_SRV_REQUEST
            ISE_ACT_SRV_GENERAL
            ISE_ACT_SRV_PUMP_TUB
            ISE_ACT_SRV_FLUI_TUB
            ISE_ACT_SRV_CALIBR
            ISE_ACT_SRV_INS_MODL
            ISE_ACT_SRV_INS_REAG
            ISE_ACT_SRV_INS_ELEC
            ISE_ACT_SRV_DEACT
            ISE_ACT_SRV_ACT
            ISE_ACT_SRV_PREPOK
            ISE_ACT_SRV_ALM 'SGM 26/10/2012

            ' LIS
            WS_MODE_RERUNS
            LIS_PROTOCOL
            LIS_DATA_TRANS_HL7
            LIS_DATA_TRANS_ASTM
            LIS_MAX_ORDER_DOWNLOAD_RUNNING
        End Enum

        ''' <summary>
        ''' Define the Enumeration for Field Limits
        ''' </summary>
        ''' <remarks>
        ''' Created by: VR 29/12/2009 
        ''' </remarks>
        Public Enum FieldLimitsEnum
            AGE_FROM_TO_DAYS
            AGE_FROM_TO_MONTHS
            AGE_FROM_TO_YEARS
            BARCODE_LIMIT
            BLANK_ABSORBANCE_LIMIT
            BLK_CALIB_REPLICATES
            CALNUMBER
            CONTROL_MIN_MAX_CONC
            CONTROL_MIN_NUM_SERIES
            CONTROL_REJECTION
            CONTROL_REPLICATES
            CONTROLS_NUMBER
            CTEST_NUM_DECIMALS
            DETECTION_LIMIT
            FACTOR_LIMITS
            KINETIC_BLANK_LIMIT
            LINEARITY_LIMIT
            NUM_ORDERS
            POSTDILUTED_SAMPLES_VOLUME
            POSTDILUTION_FACTOR
            PREDILUTION_FACTOR
            PREPARATION_VOLUME
            PROZONE_RATIO
            READING1_CYCLES
            READING2_CYCLES
            REAGENT1_VOLUME
            REAGENT2_VOLUME
            REF_RANGE_LIMITS
            RERUN_REF_RANGE
            SAMPLES_VOLUME
            SLOPE_FACTOR_A
            SLOPE_FACTOR_B
            SLOPE_FACTOR_A2
            SLOPE_FACTOR_B2
            SUBSTRATE_DEPLETION
            TEST_NUM_DECIMALS
            TEST_NUM_REPLICATES
            WASHING_VOLUME
            THERMO_ARMS_LIMIT
            THERMO_REACTIONS_LIMIT
            THERMO_FRIDGE_LIMIT
            THERMO_WASHSTATION_LIMIT
            HIGH_CONTAMIN_DEPOSIT_LIMIT
            WASH_SOLUTION_DEPOSIT_LIMIT
            REAGENT_LEVELCONTROL_LIMIT 'REAGENT_BOTTLE_VOLUME_LIMIT 'AG 22/02/2012 steps left until the bottle ZMAX

            'Well rejection process and base line calculations limits
            DAC_LIMIT
            MAINLIGHT_COUNTS_LIMIT
            REFLIGHT_COUNTS_LIMIT
            MAINDARK_COUNTS_LIMIT
            REFDARK_COUNTS_LIMIT
            BL_WELLREJECT_INI_RANGE_LIMIT
            BL_WELLREJECT_INI_ABS_LIMIT
            BL_WELLREJECT_RANGE_LIMIT
            BL_WELLREJECT_ABS_LIMIT
            BL_WELLREJECTED_INI_LIMIT
            WASHSTATION_BLW_LIMIT

            'CodeBar limits
            CODETEST_REAGENT_LIMITS 'BioSystems codetest fixed codification values
            CODETEST_ADDSOL_LIMITS 'BioSystems add solutions fixed codification values

            'Fixed Biosystems reagents barcode configuration
            BARCODE_CODETEST_LIMIT
            BARCODE_BOTTLETYPE_LIMIT
            BARCODE_REAGTYPE_LIMIT
            BARCODE_MONTHEXP_LIMIT
            BARCODE_YEAREXP_LIMIT
            BARCODE_LOTNUMBER_LIMIT
            BARCODE_BOTTLENUMBER_LIMIT
            BARCODE_CHKDIGIT_LIMIT

            '
            'SERVICE SOFTWARE
            '

            ' POSITION ADJUSTMENTS
            SRV_WASHING_Z_LIMIT
            SRV_SAMPLE_POLAR_LIMIT
            SRV_SAMPLE_Z_LIMIT
            SRV_REACTIONS_ROTOR_LIMIT
            SRV_SAMPLE_ROTOR_LIMIT
            SRV_SAMPLE_Z_FINE_LIMIT
            SRV_REAGENT1_POLAR_LIMIT
            SRV_REAGENT1_Z_LIMIT
            SRV_REAGENT1_Z_FINE_LIMIT
            SRV_REAGENT2_POLAR_LIMIT
            SRV_REAGENT2_Z_LIMIT
            SRV_REAGENT2_Z_FINE_LIMIT
            SRV_MIXER1_POLAR_LIMIT
            SRV_MIXER1_ROTOR_LIMIT
            SRV_MIXER1_Z_LIMIT
            SRV_MIXER1_Z_FINE_LIMIT
            SRV_MIXER2_POLAR_LIMIT
            SRV_MIXER2_ROTOR_LIMIT
            SRV_MIXER2_Z_LIMIT
            SRV_MIXER2_Z_FINE_LIMIT
            SRV_ENCODER_LIMIT

            SRV_REAGENT_ROTOR_LIMIT
            'SRV_REAGENT1_ROTOR_LIMIT
            'SRV_REAGENT2_ROTOR_LIMIT

            ' SENSORS
            SRV_WASH_SOLUTION_LIMIT
            'SRV_DISTILLED_WATER_LIMIT
            SRV_CONTAMINATED_LIMIT
            'SRV_NO_CONTAMINATED_LIMIT
            'SRV_REACTIONS_ROTOR_TEMP_LIMIT
            'SRV_FRIDGE_TEMP_LIMIT
            'SRV_WS_HEATER_TEMP_LIMIT
            'SRV_R1_PROBE_TEMP_LIMIT
            'SRV_R2_PROBE_TEMP_LIMIT
            SRV_SCALES_PERCENT_LIMIT

            'MOTORS
            SRV_INDO_DC_PUMP 'Limits for Internal Dosing DC Pumps
            SRV_WSDISP_DC_PUMP 'Limits for WS Dispensation DC Pump

            ' PHOTOMETRY TESTS
            SRV_LEDS_LIMIT
            SRV_BL_PhMAIN_LIMIT
            SRV_BL_PhREF_LIMIT

            ' THERMOS
            SRV_THERMO_PHOTOMETRY_SETPOINT
            SRV_THERMO_PROBE_R1_SETPOINT
            SRV_THERMO_PROBE_R2_SETPOINT
            SRV_THERMO_HEATER_SETPOINT

            'LEVEL DETECTION
            SRV_SAMPLE_LEVEL_DET_FREQ
            SRV_REAGENTS_LEVEL_DET_FREQ

            ' ISE
            ISE_PUMPS_CALIBR_OK
            ISE_CALIB_ACCEPTABLE_CL
            ISE_CALIB_ACCEPTABLE_K
            ISE_CALIB_ACCEPTABLE_LI
            ISE_CALIB_ACCEPTABLE_NA
            ISE_DISPAB_LIMITS

            'TR 13/03/2012 Indicate the sample barcode size limit (lenght).
            SAMPLE_BARCODE_SIZE_LIMIT

            LIS_PORTS_LIMIT
            LIS_SIZE_STORAGE_LIMIT
            LIS_TIMER_LIMIT
            LIS_TIMER_LIMIT_ASTM
            LIS_HOSTQUERY_PACK_LIMIT
            LIS_MAX_WAIT_ORDERS_LIMIT 'SGM 11/07/2013

            BL_DYNAMIC_ABS_LIMIT 'AG 26/11/2014 BA-2081

        End Enum

        ''' <summary>
        ''' Enumeration for Special Tests Settings
        ''' </summary>
        ''' <remarks>
        ''' Created by: SA 30/08/2010
        ''' </remarks>
        Public Enum SpecialTestsSettings
            CAL_POINT_USED
            FIXED_CALIBRATOR
            FIXED_NAME
            FIXED_SAMPLETYPE
            TOTAL_CAL_POINTS
            CRITICAL_PAUSEMODE 'AG 22/11/2013 - Task #1391
        End Enum

        ''' <summary>
        ''' Enumeration for DB SOFTWARE Parameters
        ''' </summary>
        ''' <remarks>
        ''' Modified by: SA 11/11/2014 - BA-1885 ==> Added new item MAX_QCRESULTS_TO_ACCUMULATE to the enumerated 
        ''' </remarks>
        Public Enum SwParameters

            MAX_FILE_LOG_SIZE
            LIMIT_ABS
            PATH_LENGHT
            KINETICS_INCREASE
            LINEAR_CORRELATION_FACTOR
            MAX_EXTRAPOLATION
            CURVE_POINTS_NUMBER
            REAGENT1_READINGS
            STEPS_UL
            SAMPLE_STEPS_UL 'AG 19/04/2011
            FIRST_USER_TESTID
            CYCLE_MACHINE
            TOTAL_READINGS
            SECOND_REAGENT
            PREDILUTION_CYCLES
            ISETEST_CYCLES_SERPLM
            ISETEST_CYCLES_URI
            WAITING_CYCLES_AFTER_R1
            WAITING_CYCLES_BEFORE_RUNNING
            ISE_EXECUTION_TIME_SER
            ISE_EXECUTION_TIME_URI
            ISE_EXECUTION_TIME
            SRV_MAX_WELLS_REACTIONS_ROTOR
            SRV_WELLS_TO_READ_BL
            SRV_MAX_STABILITY
            SRV_MAX_REPEATABILITY
            WASH_CONDITIONING_T1
            WASH_CONDITIONING_T2
            WASH_CONDITIONING_T3
            SAT_PATH
            SAT_REPORT_PREFIX
            SAT_REPORT_PREFIX_AUTO
            RESTORE_POINT_DIR
            RESTORE_POINT_PREFIX
            SYSTEM_BACKUP_DIR
            SYSTEM_BACKUP_PREFIX
            HISTORY_BACKUP_DIR
            HISTORY_BACKUP_PREFIX
            INVALID_PORT_LIST
            USB_REGISTRY_KEY
            EMAIL_SUBJECT
            EMAIL_BODY
            WRITE_SYSTEM_LOG
            EMAIL_RECIPIENT
            LIMS_IMPORT_PATH
            LIMS_IMPORT_MEMORY_PATH
            LIMS_EXPORT_PATH
            VOLUME_PREDILUTION
            CONTAMIN_REAGENT_PERSIS
            ISE_MODE
            XML_LOG_FILE_PATH
            PC_INFO_FILE_PATH
            SRV_READING_TIME
            SRV_READING_TIME_OFFSET_STABILITY
            SRV_MAX_CV
            SRV_MAX_ABS
            SRV_MAX_LEDS_WARNINGS
            MULTIPLE_ERROR_CODE
            SRV_CYCLETIME_MACHINE
            SRV_MAX_CYCLES
            SRV_TIME_REQUEST_STRESS_MODE
            SRV_NUM_CYCLES_DEMO
            REAG_VOL_VALIDATION 'TR 13/06/2012
            'Well rejection process
            BL_WELLREJECT_INI_SD 'Max SD allowed during well rejection initialization
            BL_WELLREJECT_SD 'Max SD allowed during well rejection (running)
            BL_WELLREJECT_INI_WELLNUMBER 'Well number used in well rejection initialization
            BLINE_INIT_FAILURES 'Number of failures of baseline initializations (ALIGHT)
            MAX_REJECTED_WELLS 'Max number of rejected wells in reactions rotor
            MAX_REACTROTOR_DAYS 'Max days using the same reactions rotor
            MAX_REACTROTOR_WELLS 'Max wells in reactions rotor

            REFILL_VOL_20ML 'MAx refil value for Bottles 20ml 
            REFILL_VOL_60ML 'MAx refil value for Bottles 60ml 

            SRV_WAVELENGTH_OPTICAL_CENTERING
            SRV_WASHING_STATION_OFFSET
            SRV_SAMPLE_ARM_OFFSET
            SRV_REAGENT1_ARM_OFFSET
            SRV_REAGENT2_ARM_OFFSET
            SRV_MIXER1_ARM_OFFSET
            SRV_MIXER2_ARM_OFFSET
            SRV_S_NEEDLE_PUMP_DUTY_EXT
            SRV_S_NEEDLE_PUMP_DUTY_INT

            XLS_PATH
            XLS_RESULTS_DOC

            STEPS_MM

            SRV_WELLS_TO_MEASURE_GLF_THERMO
            SRV_WELL1_FILL_GLF_THERMO
            SRV_WELL2_FILL_GLF_THERMO
            SRV_WELL3_FILL_GLF_THERMO
            SRV_WELL4_FILL_GLF_THERMO
            SRV_WELL5_FILL_GLF_THERMO
            SRV_WELL6_FILL_GLF_THERMO
            SRV_WELL7_FILL_GLF_THERMO
            SRV_WELL8_FILL_GLF_THERMO
            SRV_MIN_ROTOR_ROTATING_TIMES
            SRV_MAX_DISP_CONDITIONING_REAGENT
            SRV_WELL_SOURCE_REAGENT_THERMO
            SRV_MAX_ACT_CONDITIONING_HEATER
            SRV_MAX_PRIME_GLF_THERMO

            BL_WELLREJECT_ITEMS_NOTUSED

            LIGHT_ADJUST_TIME
            CONDITIONING_TIME

            'Special solutions barcode parameters
            DISTW_CODETEST
            SALINESOL_CODETEST
            DILSOL1_CODETEST
            DILSOL2_CODETEST
            DILSOL3_CODETEST 'AG 05/11/2012 - It exists in DataBase but not in SwParameters enumerate
            WASHSOL1_CODETEST
            WASHSOL2_CODETEST
            WASHSOL3_CODETEST
            ISE_WASHSOL_CODETEST
            EXTERNAL_REAGENT_BARCODE
            '-'-'

            WUPFULLTIME

            SRV_SAMPLE_ARM_ZREF_POLAR
            SRV_REAGENT1_ARM_ZREF_POLAR
            SRV_REAGENT2_ARM_ZREF_POLAR
            SRV_MIXER1_ARM_ZREF_POLAR
            SRV_MIXER2_ARM_ZREF_POLAR

            MAX_TIME_DEPOSIT_WARN 'AG 01/12/2011 - Max seconds while the deposit level (water or waste) can have a error value before alarm error is generated
            MAX_TIME_THERMO_ARM_REAGENTS_WARN 'AG 05/01/2012 - Max seconds while the R1 or R2 arm thermo warning remains before becomes to error alarm
            MAX_TIME_THERMO_REACTIONS_WARN 'AG 05/01/2012 - Max seconds while the reactions thermo warning remains before becomes to error alarm
            MAX_TIME_THERMO_FRIDGE_WARN 'AG 05/01/2012 - Max seconds while the fridge thermo warning remains before becomes to error alarm


            SRV_BARCODE_READ_SIGNAL_OK  ' XBC 14/12/2011

            ISE_UTIL_WASHSOL_POSITION 'AG 11/01/2012 - used for ise utilities (clean and pump calib)
            ISE_UTIL_VOLUME
            ISE_MIN_BUBBLE_CALIBR_OK    ' XBC 24/01/2012 - used for ise functionalities
            ISE_MIN_SAFETY_VOLUME_CAL_A
            ISE_MIN_SAFETY_VOLUME_CAL_B
            ISE_OUT_OF_RANGE_NA
            ISE_OUT_OF_RANGE_K
            ISE_OUT_OF_RANGE_CL
            ISE_OUT_OF_RANGE_LI
            ISE_CLEAN_REQUIRED_SAMPLES
            ISE_EXPIRATION_TIME_LI
            ISE_EXPIRATION_TIME_NA
            ISE_EXPIRATION_TIME_K
            ISE_EXPIRATION_TIME_CL
            ISE_EXPIRATION_TIME_REF
            ISE_EXPIRATION_TIME_PUMP_TUBING
            ISE_EXPIRATION_TIME_FLUID_TUBING
            ISE_MAX_CONSUME_LI
            ISE_MAX_CONSUME_NA
            ISE_MAX_CONSUME_K
            ISE_MAX_CONSUME_CL
            ISE_MAX_CONSUME_REF
            ISE_ERR_WS_BLOCK
            ISE_BIOSYSTEMS_CODE 'SGM 21/02/2012

            ' XBC 08/03/2012
            ' Ise Consumptions
            ISE_CONSUMPTION_SERUM_A
            ISE_CONSUMPTION_SERUM_B
            ISE_CONSUMPTION_URINE1_A
            ISE_CONSUMPTION_URINE1_B
            ISE_CONSUMPTION_URINE2_A
            ISE_CONSUMPTION_URINE2_B
            ISE_CONSUMPTION_CALB_A
            ISE_CONSUMPTION_CALB_B
            ISE_CONSUMPTION_PMCL_A
            ISE_CONSUMPTION_PMCL_B
            ISE_CONSUMPTION_BBCL_A
            ISE_CONSUMPTION_BBCL_B
            ISE_CONSUMPTION_CLEN_A
            ISE_CONSUMPTION_CLEN_B
            ISE_CONSUMPTION_PUGA_A
            ISE_CONSUMPTION_PUGA_B
            ISE_CONSUMPTION_PUGB_A
            ISE_CONSUMPTION_PUGB_B
            ISE_CONSUMPTION_PRMA_A
            ISE_CONSUMPTION_PRMA_B
            ISE_CONSUMPTION_PRMB_A
            ISE_CONSUMPTION_PRMB_B
            ISE_CONSUMPTION_SIPPING_A
            ISE_CONSUMPTION_SIPPING_B

            ISE_CALB_HOURS_NEEDED
            ISE_PUMPCAL_HOURS_NEEDED
            ISE_CLEAN_TEST_NEEDED

            'TR 01/02/2012 -ISE Volumes.
            ISE_SER_VOL 'Volume value for SER On ISE
            ISE_URI_VOL 'Volume value for URI On ISE
            ISE_PLM_VOL 'Volume value for PLM On ISE
            'TR 01/02/2012 -END

            HEIGHT_BOTTLE_DEATH_VOLUME

            ' XBC 27/02/2012
            ' Reagent 1 Z offsets
            SRV_REFZ_R1SV_OFFSET
            SRV_REFZ_R1WVR_OFFSET
            SRV_REFZ_R1DV_OFFSET
            SRV_REFZ_R1PI_OFFSET
            ' Reagent 2 Z offsets
            SRV_REFZ_R2SV_OFFSET
            SRV_REFZ_R2WVR_OFFSET
            SRV_REFZ_R2DV_OFFSET
            SRV_REFZ_R2PI_OFFSET
            ' Sample Z offsets
            SRV_REFZ_M1SV_OFFSET
            SRV_REFZ_M1WVR_OFFSET
            SRV_REFZ_M1DV1_OFFSET
            SRV_REFZ_M1PI_OFFSET
            SRV_REFZ_M1RPI_OFFSET
            SRV_REFZ_M1DV2_OFFSET
            ' Mixer 1 Z offsets
            SRV_REFZ_A1SV_OFFSET
            SRV_REFZ_A1WVR_OFFSET
            SRV_REFZ_A1DV_OFFSET
            ' Mixer 2 Z offsets
            SRV_REFZ_A2SV_OFFSET
            SRV_REFZ_A2WVR_OFFSET
            SRV_REFZ_A2DV_OFFSET
            ' Washing Station Z offsets
            SRV_REFZ_WSRR_OFFSET

            REAGENT_BARCODE_SIZE 'TR 13/03/2012 -Indicate the biosystems barcode size.

            'TR 14/03/2012 -ISE Slope Factor Parameters.
            ' XBC 10/05/2012 - change SLOPE_A to SLOPE and SLOPE_B to INTERCEPT
            ISE_SLOPE_SER_Li
            ISE_INTERCEPT_SER_Li
            ISE_SLOPE_SER_Na
            ISE_INTERCEPT_SER_Na
            ISE_SLOPE_SER_K
            ISE_INTERCEPT_SER_K
            ISE_SLOPE_SER_Cl
            ISE_INTERCEPT_SER_Cl
            ISE_SLOPE_URI_Li 'AG 15/09/2014 - BA-1918 new preloaded ISE slope factors for URI
            ISE_INTERCEPT_URI_Li 'AG 15/09/2014 - BA-1918 new preloaded ISE slope factors for URI
            ISE_SLOPE_URI_Na
            ISE_INTERCEPT_URI_Na
            ISE_SLOPE_URI_K
            ISE_INTERCEPT_URI_K
            ISE_SLOPE_URI_Cl
            ISE_INTERCEPT_URI_Cl
            'TR 14/03/2012 -END.

            'AG 15/09/2014 - BA-1918 new preloaded ISE slope factors for PLM
            ISE_SLOPE_PLM_Na
            ISE_INTERCEPT_PLM_Na
            ISE_SLOPE_PLM_K
            ISE_INTERCEPT_PLM_K
            ISE_SLOPE_PLM_Cl
            ISE_INTERCEPT_PLM_Cl
            'AG 15/09/2014 - BA-1918

            ISE_SECURITY_MODE 'SG 28/06/2012 Determines which kind of validation has to be performed 
            ISE_CMD_TIMEOUT 'SGM 02/07/2012 Time for wait Action 34 (ISE Instruction Start)

            ' XBC 17/07/2012
            ISE_PUGAbyFW_TIME1
            ISE_PUGAbyFW_TIME2
            ISE_SIP_INTERVAL

            SENSORUNKNOWNVALUE 'AG 30/03/2012
            'WELLOFFSET_FOR_PREDILUTION 'AG 08/06/2012 removed 'AG 31/05/2012 created
            BL_CONSECUTIVEREJECTED_WELL 'AG 04/06/2012

            SRV_SOUND_THERMO 'SGM 18/09/2012 - Seconds that takes the buzzer sound when Rotor conditioning has been finished 

            'AG 20/09/2012 recovery results errors in preparations
            RECOVERY_SAMPLE_FAILED
            RECOVERY_R1_FAILED
            RECOVERY_R2_FAILED
            RECOVERY_DILUTIONSOL_FAILED
            RECOVERY_SAMPLETODILUTE_FAILED
            RECOVERY_DILUTEDSAMPLE_FAILED
            RECOVERY_CLOT_DETECTED
            RECOVERY_CLOT_POSSIBLE
            RECOVERY_SAMPLE_COLLISION
            RECOVERY_R1_COLLISION
            RECOVERY_R2_COLLISION
            RECOVERY_R1_CONTAMINATIONRISK
            RECOVERY_R2_CONTAMINATIONRISK

            SW_READINGSOFFSET 'AG 27/09/2012 - Fw readings ID are from 1 to 68, Sw requires 3 to 70

            ' LIS
            APP_NAME_FOR_LIS                'XBC 28/02/2013
            CHANNELID_FOR_LIS               'XBC 28/02/2013
            LIS_STORAGE_TRANS_FOLDER        'XBC 18/03/2013
            LIS_STORAGE_RECEPTION_FOLDER    'XBC 18/03/2013
            LIS_RELEASECHANNEL_TIMEOUT      'XBC 25/03/2013
            MANUAL_EXPORT_FILENAME          'AG 03/04/2013
            ONLINE_EXPORT_FILENAME          'AG 03/04/2013
            LIS_NAME                        'SG 16/04/2013

            'The waiting time is got from database depending the action requested to analyzer
            WDOG_TIME_BARCODE_SCAN          'DL 16/04/2013

            LIS_LOG_MAX_DAYS 'TR 23/04/2013 -indicate the number of day to download for synapse event log.

            MAX_DAYS_IN_PREVIOUSLOG 'DL  31/05/2013 -indicate the number of days to save xml files

            AUTO_LIS_MAX_ITERA ' SGM 11/07/2013
            SS_KINETICS_INCREASE 'AG 23/10/2013 Task #1347
            R1SAMPLE1STREADINGSFORCALC 'AG 23/10/2013 Task #1347

            SRV_MONTHS_DELETE_LOG   ' XB 14/11/2013 Task #171 SERVICE

            MAX_RESULTSTOEXPORT_HIST 'AG 14/02/2014 - #1505 Max results to export from historical results
            MAX_APP_MEMORYUSAGE      'AG 24/02/2014 - #1520 Max BA400 application memory usage. When limit exceeded a message inform the user to close the app
            MAX_SQL_MEMORYUSAGE      'AG 24/02/2014 - #1520 Max SQL service memory usage. When limit exceeded a message inform the user to close the app

            BL_TYPE_FOR_CALCULATIONS
            BL_TYPE_FOR_WELLREJECT

            MAX_QCRESULTS_TO_ACCUMULATE 'BA-1885

            ' XB 09/12/2014 - BA-1872
            WAITING_TIME_OFF
            SYSTEM_TIME_OFFSET
            WAITING_TIME_DEFAULT
            WAITING_TIME_FAST
            WAITING_TIME_ISE_FAST
            WAITING_TIME_ISE_OFFSET
            BL_DYNAMIC_SD 'AG 26/11/2014 BA-2081
            FLIGHT_INIT_FAILURES 'Number of failures of baseline initializations (FLIGHT)
            RDI_CUMULATE_ALL_BASELINES 'AG 15/01/2015 BA-2212
        End Enum

        'AG 04/12/2014 BA-2236 code improvement. Comment this enum and use the Enum Alarms
        ''Calculation remarks List
        'Public Enum CaalculationRemarks
        '    'Generic absorbance remarks
        '    ABS_REMARK1 'Abs > Optical Limit
        '    ABS_REMARK2 'Sample Abs < Blank Abs
        '    ABS_REMARK3 'Sample Abs > Blank Abs
        '    ABS_REMARK4 'Non Linear Kinetics
        '    ABS_REMARK5 'Absorbance < 0
        '    ABS_REMARK6 'Absorbance increase < 0
        '    ABS_REMARK7 'Substrate depletion sample
        '    ABS_REMARK8 'Prozone sample possible (dilute manually and repeat)
        '    ABS_REMARK9 'Reactions Rotor Thermo warning (NEW 22/03/2011)
        '    ABS_REMARK10 'Possible clot (NEW 22/03/2011)
        '    ABS_REMARK11 'clot detected (NEW 31/03/2011)
        '    ABS_REMARK12 'Fluid system blocked (NEW 31/03/2011)
        '    ABS_REMARK13 'Finished with optical errors (NEW 21/10/2011)

        '    'Blank Remarks
        '    BLANK_REMARK1 'Main Abs > Blank Abs Limit
        '    BLANK_REMARK2 'Main Abs < Blank Abs Limit
        '    BLANK_REMARK3 'Abs Work Reagent > Blank Abs Limit
        '    BLANK_REMARK4 'Abs Work Reagent < Blank Abs Limit
        '    BLANK_REMARK5 'Blank Abs Initial > Blank Abs Limit
        '    BLANK_REMARK6 'Blank Abs Initial < Blank Abs Limit
        '    BLANK_REMARK7 'Kinetic blank > Kinetic Blank Limit
        '    BLANK_REMARK8 '(Abs T2 - Abs T1) * RT > Kinetic Blank Limit

        '    'Calibration Remarks
        '    CALIB_REMARK1 'Incorrect curve
        '    CALIB_REMARK2 'Calculated Factor beyond limits
        '    CALIB_REMARK3 'Calibration factor NOT calculated
        '    CALIB_REMARK4 'Calibration Lot Expired

        '    'Concentration Remarks
        '    CONC_REMARK1 'Conc NOT calculated
        '    CONC_REMARK2 'CONC out of calibration curve (HIGH)
        '    CONC_REMARK3 'CONC out of calibration curve (LOW)
        '    CONC_REMARK4 'Conc < 0
        '    CONC_REMARK5 'Conc > Linearity Limit
        '    CONC_REMARK6 'Conc < Detection Limit
        '    CONC_REMARK7 'Conc out of Normality Range
        '    CONC_REMARK8 'H
        '    CONC_REMARK9 'BH
        '    CONC_REMARK10 'B
        '    CONC_REMARK11 'Some standard tests with remarks

        '    ''ISE Remarks
        '    'ISE_REMARK1 '????
        '    'ISE_REMARK2 'mV Out n.2
        '    'ISE_REMARK3 'mV Out n.3
        '    'ISE_REMARK4 'mV Noise n.4
        '    'ISE_REMARK5 'mV Noise n.5
        '    'ISE_REMARK6 'Cal A Drift
        '    'ISE_REMARK7 'Out of Slope/Machine Ranges

        '    'ISE results remarks SGM 25/07/2012
        '    ISE_mVOutB
        '    ISE_mVOutA_SER
        '    ISE_mVOutB_URI
        '    ISE_mVNoiseB
        '    ISE_mVNoiseA_SER
        '    ISE_mVNoiseB_URI
        '    ISE_Drift_SER
        '    ISE_Drift_CAL
        '    ISE_OutSlope

        '    QC_OUT_OF_RANGE 'AG 20/07/2012

        'End Enum

        'INTERNAL CLASS FLAGS AG 25/02/2011
        Public Enum AnalyzerManagerFlags
            'Processes values: PENDING (not started), INPROCESS (started or required) , CLOSED (finished)
            CONNECTprocess 'AG 27/09/2011 - Connect + Wait until Ax00 becomes ready + PollFw + ReadAdjustments + Config Barcode + Config gral + Sound Off
            WUPprocess 'StandBy + ISECmd + Wash + Alight
            SDOWNprocess 'Wash + Sleep
            CHANGE_BOTTLES_Process 'AG 21/02/2012 wait ansinf and evaluate it
            RECOVERprocess 'Recover + 
            RUNNINGprocess 'Run + Start
            PAUSEprocess 'Pause (Analyzer is in Running and allows Scan Rotors) ' XB 15/10/2103
            ABORTprocess 'Abort + StandBy + Wash
            NEWROTORprocess 'Nrotor + Alight
            ENDprocess 'End`+ StandBy   ' XB 15/10/2103

            'Recovery results flags
            RESULTSRECOVERProcess 'AG 30/07/2012 - POLLRD (3 for prep status, 1 for biochemical readings, 2 for ise results) INPROCESS, PAUSED, CLOSED
            ResRecoverPrepProblems 'values INI, END or CANCELED
            ResRecoverReadings 'values INI, END or CANCELED
            ResRecoverISE 'values INI, END or CANCELED


            CONDITIONINGprocess 'To define
            COND_WASHSTATIONprocess 'To define
            WASHprocess 'To define
            BASELINEprocess 'To define


            'Actions used for the before processes. Values: NONE (action not started), INI (action started), END (action ended)
            StartInstrument
            SleepInstrument
            Washing 'StandBy washing
            BaseLine 'Adjust base line
            EnterRunning
            StartRunning
            StopRunning
            NewRotor
            Barcode

            'Internal Sw control flags
            ConnectedPortName
            ConnectedBauds
            TermoRotor
            WupStartDateTime
            TermoRotorStartDateTime

            'AG 19/01/2012 - Flags for auto (ise maintenance and scan barcode) processes before RUNNING
            ISEConditioningProcess 'ISE clean + ISE pump calib + ISE calib A+B
            ISEClean
            ISEPumpCalib
            ISECalibAB
            BarcodeSTARTWSProcess 'Scan Reagents + Scan Samples + RUNNINGprocess
            'AG 19/01/2012

            ISEConsumption 'AG 12/04/2012 - write ise consumptions after running if required (PAuse, End Work Session or Abort WS)
            WaitForAnalyzerReady 'AG 15/06/2012 - After connection establishment the Sw must wait the analyzer finishes current action before contine the connection process

            'JB 27/08/2012 - Already declared in this enum
            'ResRecoverPrepProblems 'AG 03/08/2012 - recover prep with problems (INI, CANCELED or END)
            'ResRecoverReadings 'AG 03/08/2012 - recover biochemical readings (INI, CANCELED or END)
            'ResRecoverISE 'AG 03/08/2012 - recover Ise results (INI, CANCELED or END)
            'JB 27/08/2012 - End

            ' XBC 01/08/2012 - When a ReportSAT is generated when RUNNING WS ' PENDING TO TEST !!!
            ReportSATonRUNNING
            SoftwareWSonRUNNING

            'AG 14/11/2014 BA-2065 new flags for perform Dynamic base line (flow, validation, results, ...)
            DynamicBL_Fill 'INI, END, CANCELED or NULL
            DynamicBL_Read 'INI, END, CANCELED or NULL
            DynamicBL_Empty 'INI, END, CANCELED or NULL
        End Enum

#End Region

#Region "Messages Enumerates"
        ''' <summary>
        ''' Define enumerates for Messages
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum Messages
            _NONE
            ADD_MORE_TESTS_TO_PATIENT
            ALLOWED_ACTION_PAUSE 'TR 11/11/2013 BT #1358
            BARCODE_OVERLAP
            BKL_CALIB_REQUIRED

            BOTTLE_CHANGE_NOT_ALLOWED
            CALIBCURVE_NOT_CALCULATED
            CHANGE_ADMIN_PWR
            CHANGE_PROZONE_PROGRAM
            CHANGE_ROUTINE_TO_STAT
            CHANGE_STAT_TO_ROUTINE
            CHECK_NOPOS_ELEMENTS
            COMMS_INITILIATION_ERROR

            CONECTION_FAIL
            CONFIRM_ABORT_WS 'AG 24/04/2012
            CONFIRM_SHUTDOWN 'DL 03/05/2012
            CONFIRM_SHUTDOWN_EXIT 'DL 04/06/2012
            CONFIRM_RESET_WS
            CONFIRM_BARCODE_WARNING 'AG 08/03/2013 'User has to decide what to do when read barcode before start WS has warnings
            CONTROL_AREADYSEL
            CONTROL_NOT_FOUND
            CUM_SERIES_LACK
            CUMULATE_QCRESULTS
            DB_CONNECTION_ERROR
            DECIMALS_ALLOW
            DELETE_CALIB_RESULTS_STORED
            DELETE_CONCURRENCE_CONFLICT
            DELETE_CONFIRMATION
            DELETE_CONTAMINATION
            DELETE_REFERENCE_RANGES
            DELETE_REPORT_SAT
            DELETE_SAMPTYPE_CONFIRMATION
            DELETE_SINGLE_SAMPLETYPE
            DELETE_TEST_RESULTS_STORED
            DELETE_TESTS
            DETAIL_REF_OVERWRITE
            DETGREATHERLIN
            DISCARD_PENDING_CHANGES
            DUPLICATED_CALIB_NAME
            DUPLICATE_CODE
            DUPLICATED_CONTROL_NAME
            DUPLICATED_ITEM_CODE
            DUPLICATED_PATIENT_ID
            DUPLICATED_TEST_NAME
            DUPLICATED_TEST_PROFILE_NAME
            DUPLICATED_TEST_SHORTNAME
            DUPLICATED_USERNAME
            DUPLICATED_VIRTUAL_ROTOR
            ELEMENT_ALREADY_POSITIONED
            EMAIL_ERROR
            EMAIL_SUCCESS
            EMPTYROTOR
            ERROR_COMM

            EXIT_PROGRAM
            FACTORY_VALUES
            FILE_EXIST
            FIRSTREAD_GREAT_SECONREAD

            FREEZE_RESET
            FREEZE_TOTAL
            FREEZE_PARTIAL
            FREEZE_AUTO

            FREEZE_GENERIC0 'Additional information (Line1) for freeze total & partial (no running)
            FREEZE_GENERIC1 'Additional information (Line1) for freeze total & partial (running)
            FREEZE_GENERIC2 'Additional information (Line2) for freeze total & partial (all states)
            FREEZE_GENERIC_RESET 'Additional infomation (reset freeze)
            FREEZE_GENERIC_AUTO 'Additional information (Line1) for freeze auto & total (no running) & partial (no running)

            FULL_KIT_DELETION
            INCOMPLETE_TESTSAMPLE
            INVALID_APPLICATION_USER
            INVALIDDATE
            ISE_CONDITION_FAILED
            ISE_MODULE_NOT_AVAILABLE
            ISE_NOT_ENOUGH_REAGENTS_VOL
            ISE_NOT_READY
            LAST_SAMPLETYPE
            LIMS_FILE_PENDING_TO_IMPORT

            'Errors for LIS implemented with FILES
            LIMS_FILE_SINTAX
            LIMS_INVALID_TESTTYPE
            LIMS_INVALID_REPLICATES
            LIMS_INVALID_TUBETYPE
            LIMS_INVALID_PATIENTID
            LIMS_TEST_WITH_NO_QC

            'Errors for LIS implemented with FILES or with ES
            LIMS_INVALID_FIELD_NUM
            LIMS_INVALID_SAMPLECLASS
            LIMS_INVALID_TEST
            LIMS_INVALID_SAMPLETYPE
            LIMS_INVALID_TEST_SAMPLETYPE

            'Errors for LIS implemented with ES
            LIS_AWOS_NOT_IN_WS          'AG  15/03/2013
            LIS_NO_INFO_AVAILABLE       'AG  15/03/2013
            LIS_NOT_ALLOWED_RERUN       'XB  21/03/2013
            LIS_DUPLICATED_REQUEST      'XB  21/03/2013
            LIS_DUPLICATED_NAME         'SGM 07/05/2013
            LIS_DUPLICATED_SPECIMEN     'SA  13/05/2013
            LIS_RERUN_WITH_DIF_SPECIMEN 'SA  13/05/2013

            MAINFILTER_DIF_REFFILTER
            MANUAL_DELETE_FOR_STATS
            MASTER_DATA_MISSING
            MAX_PATIENT_ORDERTESTS
            MAX_RESULTS_FOR_LISEXPORT
            MIN_MUST_BE_LOWER_THAN_MAX
            MINEQUALSMAX
            MINGREATHERMAX
            MISS_REACT_ROTOR 'AG 12/03/2012
            MISS_REACT_ROTOR2 'AG 12/03/2012
            NO_CALIBRATORS_FOR_SAMPLETYPE
            NO_CONTROLS_FOR_SAMPLETYPE
            NO_DATA_TO_PRINT
            NO_TESTS_IN_DB
            NO_TESTS_WITH_QC_ACTIVE
            NO_THREADS
            NO_PENDING_EXECUTIONS 'AG 09/05/2012
            NO_VIRTUAL_ROTORS
            NOCUMULATE_QCRESULT
            NOCUMULATE_QCSERIESVALUE
            NOT_CASE_FOUND
            NOT_FINISHED
            NOT_NULL_VALUE
            NUMERIC_ONLY
            ONE_LAB_CODE
            ONLY_ISE_WS_NOT_STARTED
            ONLY_ISE_WS_NOT_STARTED2
            OVERWRITE_FILE
            PASSWORD_CONFIRMATION
            PASSWORD_DUPLICATED
            PWR_NOT_ALLOW
            PATH_NOFOUND
            POSITIONING_CONFIRMATION
            PREPARATION_VOLUME
            PROZONE_TIMES
            PROZONE1MINORREADING
            'READING2GREATERT2 'AG 18/06/2012
            REMOVEBARCODE
            REPEATED_LOT_NUMBER
            REPEATED_NAME
            REPEATED_SHORTNAME
            REPLACE_SAMPLES_WITH_SAVED_WS
            REPORT_SAT_VERSION_IS_HIGHER
            REQUIRED_VALUE
            RESERVED_USERNAME
            RESET_NOTALLOW
            RESET_WS_FAILED
            RING2_FULL
            RING2_FULL_NOINUSE_CELLS
            ROTOR_FULL
            ROTOR_FULL_FOR_CALIBRATOR_KIT
            ROTOR_FULL_NOINUSE_CELLS
            ROTOR_RESET
            SAMPLETYPE_REQUIRED
            SAT_DB_RESTORE_ERROR
            SAT_DB_UPDATE_ERROR
            SAT_LOAD_REPORT_ERROR
            SAT_LOAD_REPORT_OK
            SAT_LOAD_RESTORE_POINT_ERROR
            SAT_LOAD_RESTORE_POINT_OK
            SAT_LOAD_VERSION_HIGHER
            SAT_SAVE_RESTORE_POINT_ERROR
            SAT_SQLSERVER_NOT_LOCAL
            ALIGHT_REQUIRED
            LOADRSAT_NOTALLOWED

            SAVED_RESULT_WARN
            SAVE_PENDING
            SEND_REPORT_SAT
            SETCALIBTEST
            SHOW_MESSAGE_TITLE_TEXT
            SHOW_MESSAGE_TITLE_TEXT_SRV

            SYSTEM_ERROR
            ZIP_ERROR 'DL 31/05/2013
            INVALID_DATABASE_VERSION 'AG 16/01/2013 v1.0.1 means db version > app version (Not record into tfmwMessages table)
            INVALID_DATABASE_UPDATE  'AG 16/01/2013 v1.0.1 means update db version process failed (Not record into tfmwMessages table)
            SYSTEM_USER
            TARGET_CAL_UPDATE
            TESTS_NOT_FOUND
            TESTNAME_REQUIRED
            USER_DATA_ERROR
            VALIDCONCENTRATIONVALUE
            VALUE_NOTGREATER

            VALUE_OUT_RANGE
            WRONG_AGE_LIMITS
            WRONG_CODE_SIZE
            WRONG_DATATYPE
            WRONG_POSTDILUTION_FACTOR
            WRONG_RANGE_LIMITS
            WRONG_USER_PASSWORD
            WRONG_USERNAME
            WS_ABORTED 'AG 20/03/2012
            WS_NONE_LOADED
            WS_NOT_FOUND
            WS_PARTIALLY_LOADED


            ZERO_NOTALLOW

            SAME_PARITY
            RECOVER_RECOMMENDED

            NOT_LEVEL_AVAILABLE

            MANUAL_MISSING 'DL 15/11/2012

            CHANGE_BOTTLE_RECOMMEND
            CHANGE_REACTROTOR_RECOMMEND
            CHANGE_REACTROTOR_REQUIRED 'AG 31/1/2012
            STATISTICAL_LACK
            TRY_CONNECTION

            XML_LOG_FILE_CORRUPTED

            ''''''''''''''''''
            'Status Bar Messages
            STARTING_INSTRUMENT
            STANDBY
            SLEEPING
            SHUTDOWN
            CONNECTED
            NO_CONNECTED
            CONNECTING
            LIGHT_ADJUSTMENT
            STARTING_SESSION
            RUNNING
            ABORTING_SESSION 'AG 13/04/2012
            BARCODE_READING
            RECOVERING_INSTRUMENT
            PERFORMING_ISE
            INITIALIZING_ISE
            ENDING_ISE
            PAUSE_IN_RUNNING

            ''''''''''''''''''

            ISE_CLEAN_REQUIRED
            ISE_CLEAN_WRONG
            ISE_CALB_WRONG
            ISE_PMCL_WRONG
            ISE_BMCL_WRONG

            WS_NO_MATCH1
            WS_NO_MATCH2

            'SERVICE SOFTWARE
            FWSCRIPT_EDITION
            FWSCRIPT_SYNTAX_OK
            FWSCRIPT_SYNTAX_ERROR
            FWSCRIPT_SEQUENCE_OK
            FWSCRIPT_SEQUENCE_ERROR

            FWSCRIPT_TEXT_IMPORT
            FWSCRIPT_TEXT_IMPORT_OK
            FWSCRIPT_TEXT_IMPORT_ERROR
            FWSCRIPT_TEXT_EXPORT
            FWSCRIPT_TEXT_EXPORT_OK
            FWSCRIPT_TEXT_EXPORT_ERROR

            FWSCRIPT_VERSION_ERROR
            FWSCRIPT_DATA_ERROR
            FWSCRIPT_DATA_MISSING
            FWSCRIPT_FILE_MISSING
            FWSCRIPT_RESTORE_OK
            FWSCRIPT_RESTORE_ERROR
            FWSCRIPT_RESTORE_WARNING

            FWSCRIPT_IMPORT_ALL_OK
            FWSCRIPT_IMPORT_ALL_ERROR
            FWSCRIPT_IMPORT_ALL_WARNING

            FWSCRIPT_EDIT_EMPTY

            FWSCRIPT_EDIT_FOLLOW_SEQUENCE
            FWSCRIPT_EDIT_NO_ENDER
            FWSCRIPT_EDIT_NO_PARAMETER_SEP
            FWSCRIPT_EDIT_NO_SEPARATOR
            FWSCRIPT_EDIT_NUMERIC_ONLY

            FWSCRIPT_VALIDATION_ERROR

            FWSCRIPT_GLOBAL_SAVE
            FWSCRIPT_GLOBAL_SAVE_OK
            FWSCRIPT_GLOBAL_SAVE_ERROR

            FWSCRIPT_LOCAL_SAVE
            FWSCRIPT_LOCAL_SAVE_OK
            FWSCRIPT_LOCAL_SAVE_ERROR

            SRV_SIMULATION_MODE
            SRV_ADJUSTMENTS_TESTS
            SRV_WRONG_VALUE
            SRV_HOMES_IN_PROGRESS
            SRV_HOMES_FINISHED
            SRV_ABS_REQUESTED
            SRV_READ_COUNTS
            SRV_OUTOFRANGE
            SRV_READ_ADJUSTMENTS
            SRV_ADJUSTMENTS_READED
            SRV_SAVE_ADJUSTMENTS
            SRV_ADJUSTMENTS_SAVED
            SRV_ADJUSTMENTS_LOCAL_SAVED
            SRV_TEST_REQUESTED
            SRV_TEST_COMPLETED
            SRV_COMPLETED
            SRV_NOT_COMPLETED
            RESULT_ERROR
            SRV_ADJUSTMENTS_READY
            SRV_ADJUSTMENTS_CANCELLED
            SRV_PREPARE_ADJUSTMENTS
            SRV_PREPARE_TEST
            SRV_STEP_POSITIONING
            SRV_OPTIC_ADJUSTMENT_ENTER
            SRV_OPTIC_ADJUSTMENT_SAVE
            SRV_TEST_EXIT_REQUESTED
            SRV_TEST_EXIT_COMPLETED
            SRV_CHANGE_MANDATORY

            SRV_ABSORBANCE_REQUEST
            SRV_ABSORBANCE_RECEIVED

            SRV_MIXER_TESTING
            SRV_MIXER_TESTED

            SRV_ADJUST_QUIT
            SRV_TEST_QUIT
            SRV_TEST_READY
            SRV_MIN_GREATER_MAX

            SRV_PHOTOMETRY_TESTS
            SRV_FILL_ROTOR
            SRV_BLACKNOALLOWED
            SRV_MAX_LEDS_WARNINGS

            SRV_NEW_VALUE_SET
            SRV_TEST_IN_PROCESS
            SRV_ADJUSTMENT_IN_PROCESS

            SRV_DATA_RECEIVED_ERROR

            SRV_TEST_PENDING
            SRV_PARK_REQUEST
            SRV_PARK_COMPLETED
            SRV_TEST_LC_EMPTYING
            SRV_TEST_DW_FILLING
            SRV_TEST_TANKS_TRANSFER
            SRV_TEST_TANKS_FINISHED

            SRV_COUNTS_RANGE_WRONG
            SRV_TEST_STOP_BY_USER
            SRV_TESTEND_ERROR
            SRV_TEST_ABORTING

            USED_ALTERNATIVECAL

            SRV_WORK_MODE_QUESTION

            SRV_ANALYZER_OK
            SRV_ANALYZER_KO

            SRV_ELEMENT_SELECTED
            SRV_PUMP_MUST_DEACTIVATE
            SRV_ARMS_IN_WASHING
            SRV_ARMS_IN_PARKING
            SRV_ITEMS_IN_DEFAULT
            SRV_ARMS_TO_WASHING
            SRV_ARMS_TO_PARKING
            SRV_ALL_ITEMS_TO_DEFAULT
            SRV_READING_MANIFOLD
            SRV_READING_FLUIDICS
            SRV_READING_PHOTOMETRICS
            SRV_HCTANK_FULL
            SRV_LCTANK_FULL
            SRV_PWTANK_FULL
            SRV_HCTANK_EMPTY
            SRV_MOTOR_DISABLED
            SRV_COLLISION_DETECTED
            SRV_ENABLING_FW_EVENTS
            SRV_DISABLING_FW_EVENTS
            SRV_COLLISION_TEST_READY
            SRV_ENCODER_TEST_READY
            SRV_SWITCH_REQUESTED

            SRV_CONDITIONING_SYSTEM
            SRV_WARNING_THERMO_MOVS
            SRV_ROTOR_FILLED
            SRV_CONDITIONING_THERMO_WARN
            SRV_STABILIZE_THERMO_WARN
            SRV_COND_THERMO_REAGENT

            SRV_THERMO_PRIME_WARN
            SRV_THERMO_PRIMING
            SRV_THERMO_PROBE
            SRV_THERMO_NEEDLE_DISP


            SRV_RESET_ANALYZER
            SRV_STANDBY_DOING
            SRV_STANDBY_DONE
            SRV_ANALYZER_IS_RESET
            SRV_ADJ_FILE_OPENED
            SRV_ADJ_FILE_SAVED
            SRV_ADJ_BACKUP_DONE
            SRV_BACKUPRESTORE_READY
            SRV_UPDATEFW_READY
            SRV_RESTORE_ADJUSTMENTS
            SRV_RESTORE_DEF_ADJUSTMENTS


            SRV_BACKUPADJ_SAVE_ASK

            SRV_RESTOREADJ_MUST_STANDBY
            SRV_RESTOREADJ_CANCEL
            SRV_RESTOREADJ_WRONG_FILE

            SRV_UPDATEFW_FIRMWARE
            SRV_UPDATEFW_MUST_SLEEP
            SRV_UPDATEFW_CANCEL

            SRV_MUST_RESET

            SRV_RESET_ASK
            SRV_STANDBY_ASK
            SRV_SLEEP_ASK

            SRV_SLEEP_DOING
            SRV_SLEEP_DONE

            SRV_READ_FIRMWARE
            SRV_FW_READED

            SRV_ADJ_RESTORED
            SRV_FACTORYADJ_RESTORED
            SRV_FW_UPDATED
            SRV_EDIT_ADJ_SAVE_ASK
            SRV_RESTORE_ADJ_OPEN_ASK
            SRV_WRONG_ADJ_ASK

            SRV_LC_QUICKCONNECT

            SRV_FW_INCOMPATIBLE

            SRV_READ_CYCLES
            SRV_CYCLES_READED
            SRV_WRITE_CYCLES
            SRV_CYCLES_WRITTEN

            SRV_ERR_SAVE
            SRV_SAVE_OK
            SRV_DELETE_HISTORIC
            SRV_NO_SUCCESS_TEST
            SRV_ERR_COMM
            'SRV_ERR_COMM2
            SRV_COMM_OK

            SRV_PRIMING
            SRV_PRIMED
            SRV_CONDITIONING
            SRV_CONDITIONED

            SRV_WS_TO_DOWN
            SRV_WS_TO_UP

            SRV_R1_TO_PARKING
            SRV_R2_TO_PARKING
            SRV_R1_TO_WASHING
            SRV_R2_TO_WASHING

            SRV_ROTOR_COND_CANCELLED
            SRV_WAIT_ACTION
            SRV_CANCELLING

            SRV_ADJ_WRONG_MODEL
            SRV_ADJ_WRONG_FWVER

            SRV_FREQUENCY_READING
            SRV_FREQUENCY_READED
            SRV_LEVEL_DETECTED
            SRV_LEVEL_NOT_DETECTED

            SRV_FWSCRIPTS_NOT_LOADED
            SRV_FWADJ_MASTER_NOK

            SRV_SAVE_GENERAL_CHANGES
            SRV_PLACE_REACTIONS_ROTOR

            SRV_CANCEL_ADJUSTMENT
            SRV_DISCARD_CHANGES

            SRV_MEBV_CHANGE_TAB
            SRV_MEBV_ERROR_WARN

            SRV_MEBV_WS_UP
            SRV_MEBV_WS_IS_UP

            'SGM 04/06/2012
            FW_VERSION_NOT_VALID
            FW_FILE_VERSION_NOT_VALID

            SRV_INVALID_SN

            FW_UPDATE_NEEDED

            FW_VERSION_NOT_VALID2
            DIFFERENT_SERIALNUM_1
            DIFFERENT_SERIALNUM_2
            DIFFERENT_SERIALNUM_3
            LOAD_SERIAL_NUM

            FW_UPDATE_ERROR
            FW_UPDATE ' DL 31/07/2012

            ' XBC 26/10/2012
            WATER_DEPOSIT_ERR
            WASTE_DEPOSIT_ERR

            'SGM 07/11/2012
            ROTOR_MISSING_WARN

            CREATE_RESTOREPOINT_SUCCESS 'SGM 03/12/2012
            CREATE_RESTOREPOINT_ERROR 'SGM 03/12/2012
            PENDING_ORDER_DOWNLOAD

            ' LIS
            LIS_DUPLICATE_SPECIMEN 'TR 29/04/2013
            LIS_MAPPING_WARNING 'SGM 06/05/2013
            LIS_STORAGE_LIMIT   ' XB 06/05/2013


            REMOVE_NOTINUSE 'TR 07/05/2013

            'SGM 12/07/2013 - new messages for Auto LIS
            AUTOLIS_LIS_NOT_READY
            AUTOLIS_NO_TUBES_FOUND
            AUTOLIS_BARCODE_DISABLED
            BARCODE_AUTOLIS_WARNING
            AUTOLIS_ALL_ORDERS_WRONG
            AUTOLIS_NO_TUBES_MATCHED
            AUTOLIS_WAITING_ORDERS 'DL 18/07/2013
            AUTOLIS_BARCODE_ERROR 'AG 24/07/2013
            'end SGM 12/07/2013

            CRITICAL_TESTS_PAUSEMODE 'AG + JV #1391 26/11/2013
            RES_RECOVER_INPAUSE 'AG 28/11/2013 - #1397

            READING_NOT_SAVED 'AG 21/05/2014 activate code: TR 06/05/2014 BT #1612, #1634 Indicate there was an error saving a received reading.-**UNCOMMENT Version 3.0.1**-
            RESULTS_CANNOT_BE_SENT 'AG 30/09/2014 BA-1440 new message to inform the user when some result cannot be sent to LIS

        End Enum

        Public Enum SourceScreen
            START_BUTTON
            ROTOR_POS
            SAMPLE_REQUEST
        End Enum
#End Region

#Region "Legend Enumerates"
        ''' <summary>
        ''' Define the enumeration for legend form
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum LEGEND_SOURCE
            LEGEND_PREPARATION
            LEGEND_ROTOR
        End Enum
#End Region

#Region "Calculation Project Enumerates"

        ''' <summary>
        ''' Define the Enumeration for Absorbance Errors
        ''' </summary>
        ''' <remarks>Created by: AG 08/01/2010</remarks>
        Public Enum AbsorbanceErrors
            ABSORBANCE_ERROR
            DC_HIGHER_BL  'Darkcurrent higher than base line
            SAMPLE_HIGHER_BL    'Sample counts > base line counts
            DC_HIGHER_SAMPLE    'Sample counts > base line counts
            ABS_LIMIT    'Calculated abs > higher absorbance optical system limit
            SUBSTRATE_DEPLETION
            INCORRECT_DATA
            NO_INUSE_ELEMENTS
            SATURATED_READING   'AG 03/06/2010 (Counts = 1048575)
            READING_ERROR   'AG 03/06/2010 (Counts = 0)
            SYSTEM_ERROR 'Run Time error is generated during absorbance calculation
        End Enum

        Public Enum CalibrationFactorErrors
            NON_MONOTONOUS_CURVE
            ABS_ERROR   'Blank or calibrator absorbance error
            BLANK_HIGHER_CALIB     'Sign*AbsCalibrator < Sign*AbsBlank
            SYSTEM_ERROR 'Run Time error is generated during calibration calculation
        End Enum

        Public Enum ConcentrationErrors
            CONCENTRATION_ERROR
            OUT 'Not calculated
            OUT_HIGH    'Not calculated
            OUT_LOW 'Not calculated
            SUBSTRATE_DEPLETION
            NO_INUSE_ELEMENTS
            SYSTEM_ERROR 'Run Time error is generated during concentration calculation
            DIVISION_BY_ZERO 'AG 13/09/2010
        End Enum

        ''' <summary>
        ''' Cycles uses on the ISE recived result 
        ''' </summary>
        ''' <remarks>
        ''' CREATE BY: TR 31/01/2011
        ''' </remarks>
        Public Enum ISECycles
            CALIBRATION
            SAMPLE
            URINE1
            URINE2
            SIPPING
            NONE
        End Enum

#End Region

#Region "Communications Project Enumerates"

#Region "Analyzer Manager Enumerates"
        'DIFFERENT CLASS EVENTS DEFINITION
        Public Enum AnalyzerManagerSwActionList 'AG 16/04/2010 - Action List that function ManageAnalyzer treats
            NONE    'AG 20/05/2010 - No action

            '''''''''''''''''''''''''''
            'SEND (DIRECTION SW --> FW)
            '''''''''''''''''''''''''''
            'INIT_COMMS 'AG 05/05/2010 - Ax00 physical layer dont use the INIT instruction, it use directly the CONNECT instruction

            'AG 29/04/2010 - Short instructions
            CONNECT 'connect
            SLEEP   'sleep
            STANDBY 'StandBy
            RUNNING 'running  
            START 'Start sending test preparation
            ENDRUN  'end running :AG 05/01/2011 - Ax00 instrucment uses the 'END' instruction but this string is reserved for the VS program so Sw has to define another 'ENDRUN'
            STATE 'Ask for status
            WASH 'StandBy Washings
            SKIP 'AG 03/03/2011
            INFO 'AG 07/04/2011
            CONFIG
            PAUSE ' XB 10/10/2013 - BT #1317

            RECOVER 'AG 22/02/2012

            'To implement
            NROTOR  'new rotor
            ABORT   'abort
            SOUND  'sound alarm
            ENDSOUND    'end sound alarm
            RESULTSRECOVER  'Results recovering

            'other actions
            NEXT_PREPARATION   'Next preparation
            ADJUST_LIGHT    'AG 18/05/2010 (ALIGHT instruction)
            ADJUST_FLIGHT 'IT 29/10/2014: BA-2061
            WRUN 'Washings during Running
            WASH_STATION_CTRL 'Washing Station Ctrol
            BARCODE_REQUEST 'Code bar reader request

            'SERVICE
            COMMAND         ' XBC 03/05/2010 - generic low level instruction
            ADJUST_BLIGHT   ' XBC 20/04/2011 (BLIGHT instruction)
            READADJ         ' XBC 03/05/2011 - read adjustments high level instruction
            LOADADJ         ' XBC 06/05/2011 - save adjustments high level instruction
            SDPOLL          ' XBC 27/04/2012 - request current stress status
            SDMODE          ' XBC 27/04/2012 - send specified stress test
            RESET_ANALYZER  ' SGM 01/07/2011 - Reset Analyzer
            LOADFACTORYADJ      ' SGM 04/07/2011 - Resyore factory adjustments
            UPDATEFW        'SGM 05/07/2011
            ISE_CMD 'SGM 12/12/2011
            FW_UTIL 'SGM 25/05/2012
            'SEND_SENSORS_FWSCRIPT 'SGM 01/04/11 

            UTIL    ' XBC 04/06/2012

            POLLSN  ' XBC 30/07/2012
            POLLRD 'AG 30/07/2012 - instructions for recorer results 

            ''''''''''''''''''''''''''''''''
            'RECEPTION (DIRECTION FW --> SW)
            ''''''''''''''''''''''''''''''''
            STATUS_RECEIVED '(STATUS instruction reception)
            READINGS_RECEIVED '(READ instruction reception)
            WAITING_TIME_EXPIRED
            BASELINE_RECEIVED 'AG 18/05/2010 (ANSAL, ANSBL, ANSDL instructions reception)
            ANSFBLD_RECEIVED 'AG 28/10/2014 - BA-2062 (results dynamic base line)
            ISE_RESULT_RECEIVED 'TR 03/01/2010 -ISE RESULTS.
            ARM_STATUS_RECEIVED 'AG 14/03/2011 - Arm status instruction received (R1, S or R2)
            ANSERR_RECEIVED 'AG 07/04/2011 - Alarms code details
            ANSINF_RECEIVED 'AG 07/04/2011 - General Status for real time monitoring
            ANSCBR_RECEIVED 'AG 22/06/2011 - Barcode reader answer (results or configuration)
            ANSTIN_RECEIVED 'AG 30/07/2012 - for result recover
            ANSPRD_RECEIVED 'AG 30/07/2012 - for result recover

            TRYING_CONNECTION
            START_TASK_TIMEOUT

            'SERVICE
            COMMAND_RECEIVED   ' XBC 16/11/2010
            ADJUSTMENTS_RECEIVED 'SGM 26/01/11
            TANKS_TEST          ' XBC 18/05/2011

            ' XBC 17/05/2011 - delete redundant Events replaced using AnalyzerManager.ReceptionEvent
            'SENSORS_RECEIVED 'SG 28/02/11
            'ABSORBANCESCAN_RECEIVED 'SG 14/03/11 FOR TESTING
            ANSSDM      ' XBC 27/04/2012

            'TANKTESTEMPTYLC_OK 'SGM 27/03/11- Pending to define PDT !!!
            'TANKTESTFILLDW_OK 'SGM 27/03/11- Pending to define PDT !!!
            'TANKTESTTRANSFER_OK 'SGM 27/03/11- Pending to define PDT !!!
            ' XBC 17/05/2011 - delete redundant Events replaced using AnalyzerManager.ReceptionEvent

            'POLL HW SGM 23/05/2011
            POLLHW
            'POLLHW_JE1
            'POLLHW_SF1
            'POLLHW_GLF

            ANSCPU_RECEIVED
            ANSBXX_RECEIVED
            ANSDXX_RECEIVED
            ANSRXX_RECEIVED
            ANSJEX_RECEIVED
            ANSSFX_RECEIVED
            ANSGLF_RECEIVED

            POLLFW   'XBC 25/05/2011
            ANSFCP_RECEIVED 'XBC 02/06/2011
            ANSFBX_RECEIVED 'XBC 02/06/2011
            ANSFDX_RECEIVED 'XBC 02/06/2011
            ANSFRX_RECEIVED 'XBC 02/06/2011
            ANSFGL_RECEIVED 'XBC 02/06/2011
            ANSFJE_RECEIVED 'XBC 02/06/2011
            ANSFSF_RECEIVED 'XBC 02/06/2011

            READCYCLES         ' SGM 27/07/2011 - read cycles PENDING TO SPEC high level instruction
            WRITECYCLES         ' SGM 27/07/2011 - write cycles PENDING TO SPEC high level instruction

            CYCLES_RECEIVED 'SGM 27/07/11

            NO_THREADS 'SGM 17/11/2011

            ANSFWU_RECEIVED 'SGM 25/05/2012

            ANSPSN_RECEIVED  ' XBC 30/07/2012

            ANSUTIL_RECEIVED 'SGM 04/10/2012

        End Enum

        ''''''''''''''''''''''''''''''''
        'RECEPTION (DIRECTION FW --> SW)
        ''''''''''''''''''''''''''''''''
        Public Enum AnalyzerManagerStatus 'Analyzer status list
            NONE = -1   ' AG+XBC 24/05/2012
            MONITOR = 0
            SLEEPING = 1
            STANDBY = 2
            RUNNING = 3
            'SAMPLINGSTOP = 4 'AG 21/02/2012 - this state does not exists in BAx00
        End Enum

        ''''''''''''''''''''''''''''''''
        'RECEPTION (DIRECTION FW --> SW)
        ''''''''''''''''''''''''''''''''
        Public Enum AnalyzerManagerAx00Actions 'Analyzer actions list
            'AG 13/09/2010 - Fill the actions defined by Fw
            '11/06/2010 Write actions (Pending to assign values with Fw)
            NO_ACTION = 0 'No action performed. Waiting
            CONNECTION_DONE = 1 'Connection done
            STATE_POLLING_DONE = 2 'State polling done
            SLEEP_START = 3 'Sleep instruction starts
            SLEEP_END = 4 'Sleep instruction ends
            STANDBY_START = 5 'Standby instruction starts
            STANDBY_END = 6 'Standby instruction ends
            RUNNING_START = 7 'Running instruction starts
            RUNNING_END = 8 'Running instruction ends
            END_RUN_START = 9 'AG 28/12/2010 end Running instruction starts
            END_RUN_END = 10 'AG 28/12/2010 end Running instruction ends
            START_INSTRUCTION_START = 11 'Instrument has to initialize Washing Station (start)
            START_INSTRUCTION_END = 12 'Instrument is ready for receive Test instructions
            WASHING_STDBY_START = 13 'AG 28/12/2010 (WASHING_PROCEDURE_START) Instrument has to perform probe/stirrer washing (start) during standby
            WASHING_STDBY_END = 14 'AG28/12/2010 (WASHING_PROCEDURE_END) Instrument probe/stirrer washing (end) during standby
            ABORT_START = 15 'Abort process start 
            ABORT_END = 16 'Abort process end
            TEST_PREPARATION_RECEIVED = 17    'Test accepted (the test is being performed)
            TEST_PREPARATION_END = 18  'Test preparation is near to be finished
            PREDILUTED_TEST_RECEIVED = 19    'AG 28/12/2010 - Prediluted Test accepted (the predilution is being performed)
            PREDILUTED_TEST_END = 20  'AG 28/12/2010 - Prediluted Test preparation is near to be finished
            ISE_TEST_RECEIVED = 21 'AG 28/12/2010 - ISE test accepted
            ISE_TEST_END = 22 'AG 28/12/2010 - ISE test is near to be finished
            WASHING_RUN_START = 23 'Start washing during running
            WASHING_RUN_END = 24 'End washing during running - PENDING ASSING CODE!!!
            WASHSTATION_CTRL_START = 25 'Washing Station control accepted
            WASHSTATION_CTRL_END = 26 'Washing Station control performed
            NEW_ROTOR_START = 27
            NEW_ROTOR_END = 28
            SKIP_START = 29 'Skip instruction accepted
            SKIP_END = 30 'Skip instruction performed

            CONFIG_DONE = 33 'Instrument has been configurated using the CONFIG instruction
            ISE_ACTION_START = 34 'ISECMD instruction start AG 20/02/2012

            ALIGHT_START = 40 'Adjust base line instruction accepted
            'ALIGHT_END = 41 'Adjust base line performed - NOT APPLY the end is the instruction ANSBLD

            BARCODE_ACTION_RECEIVED = 44 'Barcode request has been received and accepted by instrument

            FLIGHT_ACTION_START = 46 'FLIGHT instruction has been received and accepted by instrument
            FLIGHT_ACTION_DONE = 47

            'FW Scripts Low Level Instructions
            COMMAND_START = 50
            COMMAND_END = 51

            'restore adjustments file SGM 30/06/2011
            LOADADJ_START = 52
            LOADADJ_END = 53

            'UTIL
            UTIL_START = 54
            'UTIL_END = 55

            SOUND_DONE = 60

            RECOVER_INSTRUMENT_START = 70
            RECOVER_INSTRUMENT_END = 71

            STRESS_START = 80

            'UPDATE FW 
            FWUTIL_START = 90

            POLLRD_RECEIVED = 92 'PollRd request has been received and accepted by instrument

            ' XB 10/10/2013 - BT #1317
            PAUSE_START = 96
            PAUSE_END = 97

            'RESET (PENDING!!!!)
            RESET_START = 100
            RESET_END = 101

            'FACTORY ADJUSTMENTS (PENDING!!!!)
            FACTORYADJ_START = 102
            FACTORYADJ_END = 103

            'WRITE CYCLES (PENDING!!!!)
            WRITECYC_START = 106
            WRITECYC_END = 107



        End Enum

        Public Enum UI_RefreshEvents
            'USER SW EVENTS
            'AG 26/11/2010
            NONE 'Nothing to do
            EXECUTION_STATUS 'One or serveral executions have changes his status (A new test has been sent to Ax00 and has been accepted from it | Several executions have been LOCKED | ...)
            RESULTS_CALCULATED 'A new test (execution) has been calculated (ALSO used for results EXPORTED!!!!)
            READINGS_RECEIVED 'New readings instruction has been received
            ALARMS_RECEIVED 'New alarms has been received from Analyzer
            REACTIONS_WELL_STATUS_CHANGED 'New changes into a Well in the current reactions rotor (Status,...)
            ROTORPOSITION_CHANGED 'Changes into rotor position (status, remaining tests left,...) - Change sample tube status, change bottle status (this event is excluent with BARCODE_POSITION_READ)
            SENSORVALUE_CHANGED 'Changes into numerical sensors as temperatures,... (note when Alarms_RECEIVED event is triggered this event is also triggered)
            BARCODE_POSITION_READ 'Changes into rotor position due a new barcode data read (this event is excluent with ROTORPOSITION_CHANGED)


            'SERVICE SW EVENTS
            'SGM 10/06/2011
            FWEVENT_CHANGED

            'XBC 08/06/2011
            CPUVALUE_CHANGED
            'SGM 23/05/2011
            MANIFOLDVALUE_CHANGED
            FLUIDICSVALUE_CHANGED
            PHOTOMETRICSVALUE_CHANGED
            ' XBC 31/05/2011
            ARMSVALUE_CHANGED
            PROBESVALUE_CHANGED
            ROTORSVALUE_CHANGED
            ' XBC 02/06/2011
            FWCPUVALUE_CHANGED
            FWARMVALUE_CHANGED
            FWPROBEVALUE_CHANGED
            FWROTORVALUE_CHANGED
            FWPHOTOMETRICVALUE_CHANGED
            FWMANIFOLDVALUE_CHANGED
            FWFLUIDICSVALUE_CHANGED

            'SGM 28/07/2011
            HWCYCLES_CHANGED

            'SGM 15/12/2011 - NOT FOR V1
            SAMPLEFREQVALUE_CHANGED 'sample level detector frequency changed
            REAGENT1FREQVALUE_CHANGED 'sample level detector frequency changed
            REAGENT2FREQVALUE_CHANGED 'sample level detector frequency changed

            ' XBC 24/10/2012
            MULTIPLE_ERROR_CODE
            'JV 17/09/2013
            AUTO_REPORT
        End Enum

        ''' <summary>
        ''' BaseLine Types
        ''' </summary>
        ''' <remarks>
        ''' IT 03/11/2014 - BA-2067: Dynamic BaseLine
        ''' </remarks>
        Public Enum BaseLineType
            [STATIC] = 1
            DYNAMIC = 2
        End Enum

#End Region

#Region "Application Layer Enumerates"

        Public Enum AppLayerEventList 'Event List that function ActivateProtocol treats
            'Send Events (Sw -> Fw)
            CONNECT 'connect
            SLEEP   'sleep
            STANDBY 'StandBy
            RUNNING 'running
            START 'Start sending test preparation
            ENDRUN  'end running :AG 05/01/2011 - Ax00 instrucment uses the 'END' instruction but this string is reserved for the VS program so Sw has to define another 'ENDRUN'
            STATE 'Ask for status 'ASKSTATUS
            ALIGHT  'AG 18/05/2010 - adjustment of IT and DAC
            FLIGHT  'IT 29/10/2014: BA-2061
            SKIP 'AG 03/03/2011 - Sw rejects a well, Fw implements a Dummy
            INFO 'AG 07/04/2011
            NROTOR  'new rotor
            WSCTRL 'wash station control
            BARCODE_REQUEST 'Sw orders new code bar positions to read or configures the code bar reader
            CONFIG 'Send the general Configuration instruction
            RECOVER 'AG 22/02/2012 recover instrument instruction
            PAUSE ' XB 10/10/2013 - BT #1317

            'Complex instruction send
            SEND_PREPARATION
            SEND_ISE_PREPARATION 'AG 17/01/2011
            WASH_RUNNING 'AG 17/01/2011 (WASH in Running)
            WASH 'AG 17/01/2011 (WASH in StandBy)

            'Instructions to implement
            ABORT   'abort
            SOUND  'sound alarm
            ENDSOUND    'end sound alarm
            RESRECOVER  'Results recovering????

            READADJ         ' XBC 03/05/2011

            ISE_CMD 'Command to ISE module SGM 12/12/2011

            '
            ' SERVICE SOFTWARE
            ' 
            COMMAND         ' XBC 03/05/2011
            BLIGHT          ' XBC 20/04/2011 - BL & Darkness adjustment of IT and DAC
            'READADJ         ' XBC 03/05/2011
            LOADADJ         ' XBC 06/05/2011
            TANKSTEST       ' XBC 18/05/2011
            SDMODE          ' XBC 27/04/2012
            SDPOLL          ' XBC 27/04/2012
            STRESSSTOP      ' XBC 23/05/2011
            POLLFW          ' XBC 25/05/2011
            'DEMOTEST        ' XBC 30/05/2011
            'DEMOSTOP        ' XBC 30/05/2011

            READ_MANIFOLD   'SGM 24/05/2011
            READ_FLUIDICS  'SGM 24/05/2011

            POLLHW     ' XBC 31/05/2011

            ENABLE_EVENTS 'SGM 10/06/2011 'PDT to spec
            DISABLE_EVENTS 'SGM 10/06/2011 'PDT to spec

            'Reception Events (Fw -> Sw)
            'All instruction received trigger the same event
            RECEIVE
            ADJUSTMENTS_RECEIVE 'SGM 12/04/11

            RESET_ANALYZER 'SGM 01/07/2011

            LOADFACTORYADJ 'SGM 04/07/2011

            UPDATE_FIRMWARE 'SGM 04/07/2011

            READCYC         ' SGM 27/07/2011 PDT to Spec
            WRITECYC        ' SGM 27/07/2011 PDT to Spec

            FW_UTIL 'SGM 29/05/2012

            UTIL    ' XBC 04/06/2012

            POLLSN  ' XBC 30/07/2012
            POLLRD 'AG 30/07/2012

        End Enum

        Public Enum AppLayerInstrucionReception 'Instruction list Sw can receive from Ax00
            STATUS
            ANSPHR 'Photometrical readings (READ)
            'AG 01/03/2011 - these instructions are removed and integrated into ANSPHR
            'ANSBL    'AG 18/05/2010
            'ANSDL    'AG 18/05/2010

            ANSBLD 'AG 01/03/2011 - rename (ANSAL --> ANSBLD) 'ANSAL    'AG 18/05/2010
            ANSFBLD 'AG 28/10/2014 BA-2062
            ANSISE 'AG 03/01/2011 - ise results answer
            ANSINF 'AG 14/03/2011 - Fw sends detailed status for real time monitoring
            ANSERR 'Fw send alarms info
            'ANSFW 'AG 14/03/2011   ' XBC 02/06/2011 ANSFW has grown and now it becomes in ANSFCP, ANSFBX, ANSFDX, ANSFRX, ANSFGL, ANSFJE, ANSFSF
            ANSBR1 'Reagent1 arm status
            ANSBR2 'Reagent2 arm status
            ANSBM1 'Sample1 arm status
            ANSCBR 'Barcode reader answer (results or configuration)

            ANSCMD      ' XBC 03/05/2011
            ANSADJ      ' XBC 03/05/2011

            ' XBC 17/05/2011 - delete redundant Events replaced using AnalyzerManager.ReceptionEvent
            'SENSORS_RECEIVED 'SGM 28/02/11
            'ABSORBANCE_RECEIVED 'SGM 14/03/11 FOR TESTING
            ANSSDM      ' XBC 27/04/2012

            TANKTESTEMPTYLC_OK 'SGM 27/03/11- Pending to define PDT !!!
            TANKTESTFILLDW_OK 'SGM 27/03/11- Pending to define PDT !!!
            TANKTESTTRANSFER_OK 'SGM 27/03/11- Pending to define PDT !!!

            ANSFCP 'XBC 02/06/2011
            ANSFBX 'XBC 02/06/2011
            ANSFDX 'XBC 02/06/2011
            ANSFRX 'XBC 02/06/2011
            ANSFGL 'XBC 02/06/2011
            ANSFJE 'XBC 02/06/2011
            ANSFSF 'XBC 02/06/2011

            ANSCPU 'XBC 08/06/2011
            ANSBXX 'XBC 31/05/2011
            ANSDXX 'XBC 01/06/2011
            ANSRXX 'XBC 01/06/2011
            ANSJEX 'SGM 24/05/2011
            ANSSFX 'SGM 24/05/2011
            ANSGLF 'SGM 24/05/2011

            ANSPSN ' XBC 30/07/2012
            ANSTIN 'AG 30/07/2012
            ANSPRD 'AG 30/07/2012

            ANSFWU 'SGM 25/05/2012
            ANSUTIL 'SGM 04/10/2012

        End Enum

#End Region

#Region "Action Buttons Enumerates"
        Public Enum ActionButton
            'Instrument action buttons
            CONNECT
            START_INSTRUMENT
            SHUT_DOWN
            CHANGE_BOTTLES_CONFIRM 'REFILL_WASH_BOTTLE
            RECOVER

            'Work session action buttons
            START_WS
            PAUSE_WS
            CONTINUE_WS
            ABORT_WS

            'Utilities action buttons (V1)
            RAISE_WASH_STATION 'Change reactions rotor utility screen (1st step)
            CHANGE_REACTIONS_ROTOR 'Change reactions rotor utility screen (2on step)
            READ_BARCODE 'Sample request and rotor positions screen 
            CHECK_BOTTLE_VOLUME 'Rotor positions screen
            ISE_COMMAND 'ISE utilities screen

            'Utilities action buttons - FUTURE (V2)
            GENERAL_CONDITIONING
            WASHING_STATION_CONDITIONING
            WASH
            BASE_LINE
            CHANGE_REAGENTS_ROTOR
            CHANGE_SAMPLES_ROTOR

        End Enum

#End Region

#Region "Analyzer Sensors (Percentages, Temperatures, ...) to inform the presentation"
        'NOTE: By now 2011 april the temperatures are not shown to the user

        Public Enum AnalyzerSensors
            NONE = 0
            COVER_GENERAL = 1 '1 open, 0 close
            COVER_REACTIONS = 2 '1 open, 0 close
            COVER_FRIDGE = 3 '1 open, 0 close
            COVER_SAMPLES = 4 '1 open, 0 close

            BOTTLE_HIGHCONTAMINATION_WASTE = 5 'Level % high contamination waste bottle 
            BOTTLE_WASHSOLUTION = 6 'Level % washing solution

            WATER_DEPOSIT = 7 'Water deposit sensor (0 empty, 1 middle, 2 impossible state, 3 full)
            WASTE_DEPOSIT = 8 'Waste (low contamination) deposit sensor (0 empty, 1 middle, 2 impossible state, 3 full)

            TEMPERATURE_REACTIONS = 9 'Temperature reactions rotor
            TEMPERATURE_FRIDGE = 10 'Temperature fridge
            TEMPERATURE_R1 = 11 'Temperature Reagent1 arm
            TEMPERATURE_R2 = 12 'Temperature Reagent2 arm
            TEMPERATURE_WASHINGSTATION = 13 'Temperature washing station

            FRIDGE_STATUS = 14 '0 off, 1 on ,AG 30/03/2011: Special value -1 unknown
            ISE_STATUS = 15 ' 0 off, 1 on ,AG 30/03/2011: Special value -1 unknown

            ISE_CALIB_A_LEVEL 'ise calibrator A level
            ISE_CALIB_B_LEVEL 'ise calibrator B level
            ISE_WASTE_LEVEL 'ise waste level
            ISE_NA_ELECTRODE 'ise na electrode life time left (uses)
            ISE_K_ELECTRODE 'ise k electrode life time left (uses)
            ISE_CL_ELECTRODE 'ise cl electrode life time left (uses)
            ISE_LI_ELECTRODE 'ise li electrode life time left (uses)

            CONNECTED 'for Service Monitor Panel SGM and for UserSw monitor vertical bar 'Use the AnalyzerManager property Connected
            ISECMD_ANSWER_RECEIVED 'AG 12/01/2012  -ise has answered to an ISE_CMD 
            ISE_MONITOR_DATA_CHANGED 'SGM 09/03/2012
            ISE_READY_CHANGED 'SGM 09/03/2012
            ISE_SWITCHON_CHANGED 'SGM 09/03/2012
            ISE_CONNECTION_FINISHED 'SGM 09/03/2012
            ISE_PROCEDURE_FINISHED 'SGM 09/03/2012
            ISE_PREPARATIONS_LOCKED 'SGM 09/03/2012
            ISE_CALB_REQUIRED 'SGM 09/03/2012
            ISE_PUMPCAL_REQUIRED 'SGM 09/03/2012
            ISE_CLEAN_REQUIRED 'SGM 09/03/2012

            'Used for User Sw (monitor common vertical bar)
            REMAININGTIME 'WS remaining time
            SAMPLESROTOR_TIMETOACCESS 'Time until the samples rotor finish his work
            REAGENTSROTOR_TIMETOACCESS 'Time until the reagents rotor finish his work
            ANALYZER_SOUND_CHANGED 'Used for inform presentation that analyzer is ringing or not (=1 rings, 0= silence). NO requires clear value (=0) after his treatment
            FREEZE 'Analyzer becomes in freeze (1). Not freeze (0) - use the AnalyzerFreezeMode property to know if partial or total freeze


            ''''''''''''''''''
            'KEEP THIS CODE AT THE BOTTOM OF THE AnalyzerSensors ENUM DEFINITION
            ''''''''''''''''''

            'IMPORTANT NOTE:
            'The following are not Analyzer Sensors but we re-use the same sensor idea so business that uses them
            'can be affected by the real sensors functionality, in some cases we have to RESET these false-sensors values after treated them

            'FALSE SENSORS USED BY USER SOFTWARE:
            'Start instrument process:
            WARMUP_MANEUVERS_FINISHED '1) All maneuvers during Wup are finished (the End Wup button can be enabled if user want finish it manually). Requires clear value (=0) after his treatment
            WARMUP_STARTED '2) Ax00 starts the Standby process (starting instrument)
            ABORTED_DUE_BOTTLE_ALARMS '3) Sw aborts process (wup,...) due bottle level alarms. NO requires clear value (=0) after his treatment

            'Used for User Sw (change rotor utility): 
            WASHSTATION_CTRL_PERFORMED 'Washing station is up. Requires clear value (=0) after his treatment
            NEW_ROTOR_PERFORMED 'Change rotor process is finished. Requires clear value (=0) after his treatment
            DYNAMIC_BASELINE_ERROR 'Indicate if dynamic baseline has errors (BA-2143)
            RECOVERING_INTERRUPTED_PROCESSES 'NEWROTORprocess 'BA-2216
            NEW_ROTOR_PROCESS_STATUS_CHANGED 'INPROCESS to PAUSED and PAUSED to INPROCESS 'BA-2216

            'Enter Running process
            BARCODE_WARNINGS 'Used for User Sw:
            '_________________________________ Has to show auxiliary screen with Barcode WS alarms (samples without tests) when this sensor is triggered (= 1). Requires clear value (=0) after his treatment
            '_________________________________ After read reagents & samples barcode and apply createWSExecution method there are not available executions (=2). Requires clear value (=0) after his treatment

            ISE_WARNINGS 'AG 20/01/2012 User Sw has to inform the user in ise utilities and enter running
            BEFORE_ENTER_RUNNING 'DL 18/01/2012 -  All processes before enter in Running are finished (= 1)

            'Recover instrument process
            RECOVER_PROCESS_FINISHED 'Gets the value 0 when the recover instruction is sent. Changes to value 1 when recover instruction finished (ok or with errors)
            '                 Define this new false sensor because when the recover finishs with errors we do not know if fw will sent Action Recover End or only the recover error code


            'Used for User Sw due some screens (for instance IWSRotorPositions) has different user event enabled/disable policy depending the analyzer status
            'and the analyzer status can change when the screen is shown. Requires clear value (=0) after his treatment
            ANALYZER_STATUS_CHANGED

            ERROR_IN_STATUS_CHANGING 'AG 23/03/2012 - some errors (alarms) while sleep ini, standby ini or running ini can cause the Fw send the end action code but without change to the
            'new status, so Sw has to treat this cases and for example activate the user interface if it is disabled (start ws or continue ws button press). Requires clear value (=0) after his treatment

            AUTO_PAUSE_BY_ALARM 'AG 15/05/2012 - when some alarm appears and his treatment sends END instruction this sensor is set to 1. Requires clear value (=0) after his treatment
            RECOVERY_RESULTS_STATUS 'AG 27/08/2012 - Gets the value 1 when recovery results starts and UI changes to an special screen 
            '                                           gets the value 0 when recovery results finishes and UI load the monitor screen 

            FW_UPDATE_UTIL_RECEIVED 'SGM 29/05/2012 - when fw responses to a FWUTIL command

            TESTING_NEEDLE_COLLIDED 'SGM 10/10/2012 - SERVICE: while Needle Collision test some needle is collided
            SERIAL_NUMBER_SAVED 'SGM 10/10/2012 - SERVICE: result of the Serial Number saving operation

            SRV_MANAGEMENT_ALARM_TYPE   ' XBC 16/10/2012 - SERVICE : Received alarms Management type  (values : 1.Update Fw; 2.Fatal Error; 3.Recover; 4.Simple Error; 5.Request Info; 6.Ommit Error)
            AUTO_RERUN_ADDED 'AG 14/03/2014 - #1524: 1 rerun added automatically to WS, 0: no

            FLIGHT_ROTOR_ALREADY_FULL   'BA-2358

        End Enum
#End Region

#Region "Processes"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' Created by: IT 01/12/2014 - BA-2075
        ''' </remarks>
        Public Enum WarmUpProcessFlag
            StartInstrument
            Wash
            ProcessStaticBaseLine
            ProcessDynamicBaseLine
            ConfigureBarCode
            Finalize
        End Enum

#End Region

#Region "Instructions"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' Created by: IT 01/12/2014 - BA-2075
        ''' </remarks>
        Public Enum InstructionActions
            None
            FlightFilling
            FlightReading
            FlightEmptying
        End Enum

#End Region

#End Region

#Region "SAT Reports"

        'SG 13/10/10 used to compare Application's version to SAT Report's version
        Public Enum SATReportVersionComparison
            EqualsAPP
            LowerThanAPP
            UpperThanAPP
        End Enum

        'SG 13/10/10 used to define which report action has to be performed in the CreateSATreport method
        Public Enum SATReportActions
            SAT_REPORT
            SAT_RESTORE
            SAT_UPDATE
            SYSTEM_BACKUP
            HISTORY_BACKUP
            SAT_UPDATE_ERR
        End Enum

#End Region

#Region "Absorbance Time Graphical"
        Public Enum GraphicalAbsScreenCallMode
            NONE
            RESULTS_SINGLE 'Results screen one replicate
            RESULTS_MULTIPLE 'Results screen several replicates
            CURVE_RESULTS_SINGLE 'Curve Results screen one replicate
            CURVE_RESULTS_MULTIPLE 'Curve Results screen several replicates
            WS_STATES_SINGLE 'WorkSession states monitoring one replicate (NOT USED)
            WS_STATES_MULTIPLE 'WorkSession states monitoring several replicates (NOT USED)
        End Enum

        Public Enum ScreenCallsGraphical
            CURVEFRM
            RESULTSFRM
            WS_STATES
        End Enum

#End Region

#Region "Miscellaneous"

        'AG 28/07/2010 - Move from ManualRepetition and define as Public
        Public Enum PostDilutionTypes
            INC
            RED
            NONE
            WITHOUT
            UNDEFINED
        End Enum
        'END AG 28/07/2010

        ' dl 14/04/2011
        Public Enum GlbSourceScreen
            STANDARD
            TEST_QCTAB
            MODEL2
        End Enum
        ' end dl 14/04/2011

        'RH 08/09/2011
        Public Enum Rotors
            UNDEFINED
            SAMPLES
            REAGENTS
            REACTIONS
        End Enum
        'END RH 08/09/2011

        'RH 09/09/2011
        Public Enum TubeTypes
            UNDEFINED
            TUBE
            PEDIATRIC
        End Enum
        'END RH 09/09/2011

        'RH 05/12/2011 ReportOrientation
        Public Enum Orientation
            PORTRAIT
            LANDSCAPE
        End Enum
        'RH 05/12/2011

#End Region

#Region "Instrument Adjustments"
        Public Enum Ax00Adjustsments
            'Based on A400-Ajustes-Rev4.txt (\\Servidor\Documents\Instruments\RDI\Gestio Projectes\AX00\02 REQUISITS\03 FIRMWARE\Definicion Ajustes)
            ALL

            'AG 26/09/2011 - Copied from Database
            'SELECT CodeFw, DescriptionFw , GroupID FROM srv_tfmwAdjustments ORDER BY GroupID ASC

            RRCB    'Posición referencia Codigo de Barras - Pote 1 en C.B.	BARCODE_BOTTLE
            RMCB    'Posición referencia Codigo de Barras - Pocillo 1 en C.B.	BARCODE_WELL
            DWINPUT 'Entrada Agua Destilada 0-deposito 1-grifo	FLUIDIC_IN_OUT_SYSTEM

            GWMA    'Nivel Lleno Alta Contaminación	HIGH_CONTAMINATION
            GWMI    'Nivel Vacío Alta Contaminación	HIGH_CONTAMINATION

            ILED1   'valor de referencia intensidad Led 1	LEDS_CURRENT_REF
            ILED10  'valor de referencia intensidad Led 10	LEDS_CURRENT_REF
            ILED11  'valor de referencia intensidad Led 11	LEDS_CURRENT_REF
            ILED2   'valor de referencia intensidad Led 2	LEDS_CURRENT_REF
            ILED3   'valor de referencia intensidad Led 3	LEDS_CURRENT_REF
            ILED4   'valor de referencia intensidad Led 4	LEDS_CURRENT_REF
            ILED5   'valor de referencia intensidad Led 5	LEDS_CURRENT_REF
            ILED6   'valor de referencia intensidad Led 6	LEDS_CURRENT_REF
            ILED7   'valor de referencia intensidad Led 7	LEDS_CURRENT_REF
            ILED8   'valor de referencia intensidad Led 8	LEDS_CURRENT_REF
            ILED9   'valor de referencia intensidad Led 9	LEDS_CURRENT_REF

            A1DH    'Agitación en rotor - Horizontal	MIXER1_ARM_DISP1
            A1DV    'REF Agitación en rotor - Vertical	MIXER1_ARM_DISP1

            RA1D    'Agitación en rotor - Rotor	MIXER1_ARM_DISP1
            RA1V    'Posición Parking - Rotor	MIXER1_ARM_PARK
            A1H 'Posición Parking - Horizontal	MIXER1_ARM_PARK
            A1V 'REF Posición Parking - Vertical	MIXER1_ARM_PARK
            A1SV    'Posición Segura para movimiento Horizontal - Vertical	MIXER1_ARM_VSEC
            A1WVR   'NULL	MIXER1_ARM_VSEC2
            A1WH    'Estación de Lavado - Horizontal	MIXER1_ARM_WASH
            A1WSV   'Estación de Lavado - Vertical	MIXER1_ARM_WASH
            RA1WS   'Estación de Lavado - Rotor	MIXER1_ARM_WASH
            RA1R    'Posicion referencia -Rotor	MIXER1_ARM_ZREF
            A1RH    'Posicion referencia - Horizontal	MIXER1_ARM_ZREF
            A1RV    'Posicion referencia - Vertical	MIXER1_ARM_ZREF

            A2DH    'Agitación en rotor - Horizontal	MIXER2_ARM_DISP1
            A2DV    'REF Agitación en rotor - Vertical	MIXER2_ARM_DISP1
            RA2D    'Agitación en rotor - Rotor	MIXER2_ARM_DISP1
            RA2V    'Posición Parking - Rotor	MIXER2_ARM_PARK
            A2H 'Posición Parking - Horizontal	MIXER2_ARM_PARK
            A2V 'REF Posición Parking - Vertical	MIXER2_ARM_PARK
            A2SV    'Posición Segura para movimiento Horizontal - Vertical	MIXER2_ARM_VSEC
            A2WVR   'Estación de Lavado - Vertical Relativo a R1SV	MIXER2_ARM_VSEC2
            A2WH    'Estación de Lavado - Horizontal	MIXER2_ARM_WASH
            A2WSV   'Estación de Lavado - Vertical	MIXER2_ARM_WASH
            RA2WS   'Estación de Lavado - Rotor	MIXER2_ARM_WASH
            RA2R    'Posicion referencia -Rotor	MIXER2_ARM_ZREF
            A2RH    'Posicion referencia - Horizontal	MIXER2_ARM_ZREF
            A2RV    'Posicion referencia - Vertical	MIXER2_ARM_ZREF

            GFWR1   'Posición referencia lectura - Pocillo 1	PHOTOMETRY

            R1DH    'Dispensación en Rotor - Horizontal	REAGENT1_ARM_DISP1
            R1DV    'REF Dispensación en Rotor - Vertical	REAGENT1_ARM_DISP1
            RR1D    'REF Dispensación en Rotor - Rotor	REAGENT1_ARM_DISP1
            R1PI    'REF Punto Inicial Detección de nivel - Vertical	REAGENT1_ARM_LEVEL
            R1V 'REF Posición Parking - Vertical	REAGENT1_ARM_PARK
            RR1 'Posición Parking - Rotor	REAGENT1_ARM_PARK
            R1H 'Posición Parking - Horizontal	REAGENT1_ARM_PARK
            R1PH1   'Rotor de Reactivos Corona 1 - Horizontal	REAGENT1_ARM_RING1
            RR1P1   'Posición referencia Pipeteo - Pote 1 en Corona 1 Brazo R1	REAGENT1_ARM_RING1
            R1PV1   'Rotor de Reactivos Corona 1 Punto máximo detección - Vertical	REAGENT1_ARM_RING1
            R1PV2   'Rotor de Reactivos Corona 2 Punto máximo detección - Vertical	REAGENT1_ARM_RING2
            RR1P2   'Posición referencia Pipeteo - Pote XX en Corona 2 Brazo R1	REAGENT1_ARM_RING2
            R1PH2   'Rotor de Reactivos Corona 2 - Horizontal	REAGENT1_ARM_RING2
            R1SV    'Posición Segura para movimiento Horizontal - Vertical	REAGENT1_ARM_VSEC
            R1WVR   'Estación de Lavado - Vertical Relativo a R1SV	REAGENT1_ARM_VSEC2
            R1WH    'Estación de Lavado - Horizontal	REAGENT1_ARM_WASH
            R1WSV   'Estación de Lavado - Vertical	REAGENT1_ARM_WASH
            RR1WS   'Estación de Lavado - Rotor	REAGENT1_ARM_WASH
            RR1R    'Posicion referencia - Rotor	REAGENT1_ARM_ZREF
            R1RH    'Posicion referencia - Horizontal	REAGENT1_ARM_ZREF
            R1RV    'Posicion referencia - Vertical	REAGENT1_ARM_ZREF
            DR1S    'Sensibilidad Detección Reactivo 1	REAGENT1_LEVEL_DET

            R2DH    'Dispensación en Rotor - Horizontal	REAGENT2_ARM_DISP1
            R2DV    'REF Dispensación en Rotor - Vertical	REAGENT2_ARM_DISP1
            RR2D    'REF Dispensación en Rotor - Rotor	REAGENT2_ARM_DISP1
            R2PI    'REF Punto Inicial Detección de nivel - Vertical	REAGENT2_ARM_LEVEL
            R2H 'Posición Parking - Horizontal	REAGENT2_ARM_PARK
            R2V 'REF Posición Parking - Vertical	REAGENT2_ARM_PARK
            RR2 'Posición Parking - Rotor	REAGENT2_ARM_PARK
            RR2P1   'Posición referencia Pipeteo - Pote 1 en Corona 1 Brazo R2	REAGENT2_ARM_RING1
            R2PH1   'Rotor de Reactivos Corona 1 - Horizontal	REAGENT2_ARM_RING1
            R2PV1   'Rotor de Reactivos Corona 1 Punto máximo detección - Vertical	REAGENT2_ARM_RING1
            R2PV2   'Rotor de Reactivos Corona 2 Punto máximo detección - Vertical	REAGENT2_ARM_RING2
            R2PH2   'Rotor de Reactivos Corona 2 - Horizontal	REAGENT2_ARM_RING2
            RR2P2   'Posición referencia Pipeteo - Pote XX en Corona 2 Brazo R2	REAGENT2_ARM_RING2
            R2SV    'Posición Segura para movimiento Horizontal - Vertical	REAGENT2_ARM_VSEC
            RR2WS   'Estación de Lavado - Rotor	REAGENT2_ARM_WASH
            R2WH    'Estación de Lavado - Horizontal	REAGENT2_ARM_WASH
            R2WSV   'Estación de Lavado - Vertical	REAGENT2_ARM_WASH
            R2RH    'Posicion referencia - Horizontal	REAGENT2_ARM_ZREF
            R2RV    'Posicion referencia - Vertical	REAGENT2_ARM_ZREF
            RR2R    'Posicion referencia - Rotor	REAGENT2_ARM_ZREF
            R2WVR   'Estación de Lavado - Vertical Relativo a R1SV	REAGENT2_ARMV SEC2
            DR2S    'Sensibilidad Detección Reactivo 2	REAGENT2_LEVEL_DET

            DM1S    'Sensibilidad Detección Muestra	SAMPLE_LEVEL_DET
            M1DH1   'Dispensación en Rotor Posicion 1 - Horizontal	SAMPLES_ARM_DISP1
            M1DV1   'REF Dispensación en Rotor Posicion 1 - Vertical	SAMPLES_ARM_DISP1
            RM1D1   'Dispensación en Rotor Posicion 1 - Rotor	SAMPLES_ARM_DISP1
            RM1D2   'Dispensación en Rotor Posicion 2 - Rotor	SAMPLES_ARM_DISP2
            M1DV2   'REF Dispensación en Rotor Posicion 2 - Vertical	SAMPLES_ARM_DISP2
            M1DH2   'Dispensación en Rotor Posicion 2 - Horizontal	SAMPLES_ARM_DISP2
            M1ISEH  'Dispensación en ISE - Horizontal	SAMPLES_ARM_ISE
            M1ISEV  'Dispensación en ISE - Vertical	SAMPLES_ARM_ISE
            RM1ISE  'Dispensación en ISE - Rotor	SAMPLES_ARM_ISE
            M1PI    'REF Punto Inicial Detección de nivel - Vertical	SAMPLES_ARM_LEVEL_DET
            M1RPI   'REF Punto Inicial Detección de nivel - Vertical - En Rotor de Reacciones	SAMPLES_ARM_LEVEL_DET
            M1V 'REF Posición Parking - Vertical	SAMPLES_ARM_PARK
            M1H 'Posición Parking - Horizontal	SAMPLES_ARM_PARK
            RM1 'REF Posición Parking - Rotor	SAMPLES_ARM_PARK
            RM1P1   'Posición referencia Pipeteo - Pocillo  1 en Corona 1	SAMPLES_ARM_RING1
            M1PH1   'Rotor de Muestras Corona 1 - Horizontal	SAMPLES_ARM_RING1
            M1PV1p  'Rotor de Muestras Corona 1 Punto máximo detección pediátrico - Vertical	SAMPLES_ARM_RING1
            M1PV2p  'Rotor de Muestras Corona 2 Punto máximo detección pediátrico - Vertical	SAMPLES_ARM_RING2
            M1PH2   'Rotor de Muestras Corona 2 - Horizontal	SAMPLES_ARM_RING2
            RM1P2   'Posición referencia Pipeteo - Pocillo xx en Corona 2	SAMPLES_ARM_RING2
            RM1P3   'Posición referencia Pipeteo - Pocillo xx en Corona 3	SAMPLES_ARM_RING3
            M1PH3   'Rotor de Muestras Corona 3 - Horizontal	SAMPLES_ARM_RING3
            M1PV3p  'Rotor de Muestras Corona 3 Punto máximo detección pediátrico - Vertical	SAMPLES_ARM_RING3
            M1SV    'Posición Segura para movimiento Horizontal - Vertical	SAMPLES_ARM_VSEC
            M1WVR   'Estación de Lavado - Vertical Relativo a R1SV	SAMPLES_ARM_VSECWS
            RM1WS   'Estación de Lavado - Rotor	SAMPLES_ARM_WASH
            M1WH    'Estación de Lavado - Horizontal	SAMPLES_ARM_WASH
            M1WSV   'Estación de Lavado - Vertical	SAMPLES_ARM_WASH
            M1RH    'Posicion referencia - Horizontal	SAMPLES_ARM_ZREF
            M1RV    'Posicion referencia - Vertical	SAMPLES_ARM_ZREF
            RM1R    'Posicion referencia - Rotor	SAMPLES_ARM_ZREF
            M1PV1t  'Rotor de Muestras Corona 1 Punto máximo detección tubo - Vertical	SAMPLES_ARM_ZTUBE1
            M1PH1t  'Rotor de Muestras Corona 1 Punto máximo detección tubo - Horizontal	SAMPLES_ARM_ZTUBE1
            M1PH2t  'Rotor de Muestras Corona 2 Punto máximo detección tubo - Horizontal	SAMPLES_ARM_ZTUBE2
            M1PV2t  'Rotor de Muestras Corona 2 Punto máximo detección tubo - Vertical	SAMPLES_ARM_ZTUBE2
            M1PV3t  'Rotor de Muestras Corona 3 Punto máximo detección tubo - Vertical	SAMPLES_ARM_ZTUBE3
            M1PH3t  'Rotor de Muestras Corona 3 Punto máximo detección tubo - Horizontal	SAMPLES_ARM_ZTUBE3

            GTN1    'Consigna Termo Nevera 1	THERMOS_FRIDGE
            GTON1   'Objetivo Termo Nevera 1	THERMOS_FRIDGE
            GTOGF   'Objetivo Termo Fotometría	THERMOS_PHOTOMETRY
            GTGF    'Consigna Termo Fotometría	THERMOS_PHOTOMETRY
            GTOR1   'Objetivo Termo Punta Reactivo 1	THERMOS_REAGENT1
            GTR1    'Consigna Termo Punta Reactivo 1	THERMOS_REAGENT1
            GTR2    'Consigna Termo Punta Reactivo 2	THERMOS_REAGENT2
            GTOR2   'Objetivo Termo Punta Reactivo 2	THERMOS_REAGENT2
            GTOH1   'Objetivo Heater Estación de Lavado	THERMOS_WS_HEATER
            GTH1    'Consigna Heater Estación de Lavado	THERMOS_WS_HEATER

            GSMA    'Nivel Lleno Solución de Lavado	WASHING_SOLUTION
            GSMI    'Nivel Vacío Solución de Lavado	WASHING_SOLUTION
            WSEV    'REF Posicion final ciclo lavado - Vertical	WASHING_STATION
            WSRR    'Posicion ready relativa a Posicion referencia - Vertical	WASHING_STATION
            WSRV    'Posicion referencia - Vertical	WASHING_STATION
            WSSV    'REF Posicion de inicio ciclo lavado - Vertical	WASHING_STATION


            MCOV    'Tapa analizador activada (1), desactivada (0)
            RCOV    'Tapa reactivos activada (1), desactivada (0)
            SCOV    'Tapa muestras activada (1), desactivada (0)
            PHCOV   'Tapa reacciones activada (1), desactivada (0)
            CLOT   'Detección de clot activada (1), desactivada (0)

            SEXC    'Parámetro interno FW (Sample Excess)
            ISEINS  'ISE Instalado (0: NO INSTALADO, 1: INSTALADO)

        End Enum

#End Region

#Region "ISE Module"

#Region "Settings"

        Public Enum ISEModuleSettings
            NONE

            ISE_MODE '0-Simple, 1-Debug1 or 2-Debug 2 'PDT to Database

            AVAILABLE_VOL_CAL_A 'Avalaible Calibrator A volume
            AVAILABLE_VOL_CAL_B 'Avalaible Calibrator B volume
            REAGENTS_INSTALL_DATE   'Reagents Pack’s Installation Date
            LI_INSTALL_DATE 'Lithium electrode Installation Date 
            NA_INSTALL_DATE 'Sodium electrode Installation Date
            K_INSTALL_DATE  'Potassium electrode Installation Date
            CL_INSTALL_DATE 'Clorine electrode Installation Date
            REF_INSTALL_DATE    'Reference electrode Installation Date
            PUMP_TUBING_INSTALL_DATE    'Pump tubing electrode Installation Date
            FLUID_TUBING_INSTALL_DATE   'Fluidic tubing electrode Installation Date
            LI_CONSUMPTION  'Lithium electrode used times counter
            NA_CONSUMPTION  'Sodium electrode used times counter
            K_CONSUMPTION   'Potassium electrode used times counter
            CL_CONSUMPTION  'Clorine electrode used times counter
            REF_CONSUMPTION 'Reference electrode used times counter
            'LI_CONSUMPTION_SERUM    'Lithium electrode used times counter with Serum
            'NA_CONSUMPTION_SERUM    'Sodium electrode used times counter with Serum
            'K_CONSUMPTION_SERUM 'Potassium electrode used times counter with Serum
            'CL_CONSUMPTION_SERUM    'Clorine electrode used times counter with Serum
            'REF_CONSUMPTION_SERUM   'Reference electrode used times counter with Serum
            'LI_CONSUMPTION_URINE    'Lithium electrode used times counter with Urine
            'NA_CONSUMPTION_URINE    'Sodium electrode used times counter with Urine
            'K_CONSUMPTION_URINE 'Potassium electrode used times counter with Urine
            'CL_CONSUMPTION_URINE    'Clorine electrode used times counter with Urine
            'REF_CONSUMPTION_URINE   'Reference electrode used times counter with Urine
            LI_ENABLED 'LI_INSTALLED    'Lithium electrode installed
            LONG_TERM_DEACTIVATED   'Long term deactivation is active
            'AUTO_COND_REQUIRED  'Automatic conditioning is required
            'AUTO_COND_DONE  'Automatic conditioning is done
            LAST_CALB_DATE 'Last Electrodes calibration date
            LAST_PUMP_CAL_DATE 'Last pumps Calibration date
            LAST_BUBBLE_CAL_DATE 'Last bubble Calibration date
            LAST_CLEAN_DATE 'Last clean Date
            SAMPLES_SINCE_LAST_CLEAN 'ISE samples done since last clean cycle
            CLEANING_PACK_INSTALLED 'Reagents pack Dummy for cleaning Installed
            CALB_PENDING 'CALB operation pending after a CLEN operation
            PUMP_CAL_PENDING
            BUBBLE_CAL_PENDING 'SGM 25/07/2012
            CLEAN_PENDING
            LAST_CALB_RESULT1 'Last Electrodes calibration result 1
            LAST_CALB_RESULT2 'Last Electrodes calibration result 2
            LAST_CALB_ERROR 'Last Electrodes calibration error 'JB 02/08/2012
            LAST_PUMPCAL_RESULT 'Last pumps Calibration result
            LAST_PUMPCAL_ERROR 'Last Pumps Calibration error 'JB 02/08/2012
            LAST_BUBBLECAL_RESULT 'Last bubble Calibration result
            LAST_BUBBLECAL_ERROR 'Last Bubble Calibration error 'JB 02/08/2012
            LAST_CLEAN_ERROR 'Last Clean error 'JB 02/08/2012
            DISTRIBUTOR_CODE 'Reagents Pack’s Distributor code
            REAGENTS_EXPIRE_DATE 'Reagents Pack’s Expiration Date
            INITIAL_VOL_CAL_A 'Initial Calibrator A volume
            INITIAL_VOL_CAL_B 'Initial Calibrator B volume
            REAGENTS_SERIAL_NUM ' Identifier of the Reagents Pack
            LAST_OPERATION_DATE ' Date of the Last operation completed
            LAST_OPERATION_WS_DATE ' Date of the Last operation completed while Work Session is Running
        End Enum

#End Region

#Region "Mode"
        Public Enum ISEModes
            None = 0
            Low_Level_Control = 1 '(Use CMD)
            Cleaning_Cycle = 2 '(A400 Needs Washing Solution position in Sample Rotor)
            Pump_Calibration = 3 '(A400 Needs Calibration/Water Solution position in Sample Rotor)

            'SGM 02/07/2012
            Move_R2_To_WashStation = 4 'Moves the R2 arm away from the Parking in order to perform maintenance operations during ISE Utilities is open
            Move_R2_To_Parking = 5 'Moves the R2 arm back to the Parking after maintenance operations during ISE Utilities is open
        End Enum
#End Region

#Region "Commands"
        'defined in the Excel document located at \\servidor\Documents\Instruments\RDI\Gestio Projectes\AX00\02 REQUISITS\03 FIRMWARE
        Public Enum ISECommands
            NONE = -1
            POLL = 0 'expects <ISE!>
            CALB = 1 'expects <BMV.....>
            SAMP = 2 'expects <ISE!>
            URINE_ONE = 3 'expects <ISE!>
            URINE_TWO = 4 'expects <ISE!>
            CLEAN = 5 'expects <ISE!>
            PUMP_CAL = 6
            START = 7 'expects Results of the pre-defined cycle
            PURGEA = 8 'expects <ISE!>
            PURGEB = 9 'expects <ISE!>
            BUBBLE_CAL = 10 'expects <BBC....>
            SHOW_BUBBLE_CAL = 11 'expects <BBC...>
            SHOW_PUMP_CAL = 12 'expects <PMC....>
            DSPA = 13 ' (Use P1)'expects <ISE!>
            DSPB = 14 ' (Use P1)'expects <ISE!>
            VERSION_CHECKSUM = 15
            READ_mV = 16 'expects <AMV.....>
            LAST_SLOPES = 17 'expects <BMV....>
            DEBUG_mV_ONE = 18 'expects <ISE!>
            DEBUG_mV_TWO = 19 'expects <ISE!>
            DEBUG_mV_OFF = 20 'expects <ISE!>
            MAINTENANCE = 21 'expects <ISE!>
            PRIME_CALA = 22 'expects <ISE!>
            PRIME_CALB = 23 'expects <ISE!>
            READ_PAGE_0_DALLAS = 24 'expects <DSN.....>
            READ_PAGE_1_DALLAS = 25 'expects <DSN.....>
            WRITE_DALLAS = 26 ' (use P1, P2 and P3)'expects <ISE!>

            SHOW_CALB = 29
            SAMP_START = 30
            URINE_ONE_START = 31 'expects <UMV...><URN...>
            URINE_TWO_START = 32 'expects <UMV...><URN...>
            CLEAN_START = 33 'expects <ISE!>
            PUMP_CAL_START = 34 'expects <PMC....>

            ' XBC 19/03/2012
            WRITE_DAY_INSTALL = 40
            WRITE_MONTH_INSTALL = 41
            WRITE_YEAR_INSTALL = 42

            ' XBC 10/04/2012
            WRITE_CALA_CONSUMPTION = 43
            WRITE_CALB_CONSUMPTION = 44

            'SGM R2 Arm 05/07/2012
            R2_TO_WASHING = 45
            R2_TO_PARKING = 46

        End Enum






#End Region

#Region "Sample Tube Type"

        Public Enum ISESampleTubeTypes
            No_Tube '(Blank reagent only)
            Paediatric
            T13
            T15
            BOTTLE1 '(death volume)
            BOTTLE2 '(30ml)
            BOTTLE3 '(50ml)
            BOTTLE4 '(75ml) -Obsolete
            BOTTLE5 '(100ml)
        End Enum

#End Region

#Region "Reagent Pack"
        Public Enum WrongReasons
            None = 0
            Empty = 1
            Lapsed_at_load = 2
            Lapsed_when_using = 4
        End Enum

        Public Enum DallasXipPage1
            ''New specification 05/06/2012
            'InstallMonth = 0
            'InstallYear = 1
            'InstallDay = 2
            'FirstPositionConsumptionCalA = 3
            'FirstPositionConsumptionCalB = 16
            'NoGoodByte = 29

            ''OLD specification
            FirstPositionConsumptionCalA = 0
            LastPositionConsumptionCalA = 12
            InstallDay = 13
            InstallMonth = 14
            InstallYear = 15
            FirstPositionConsumptionCalB = 16
            LastPositionConsumptionCalB = 28
            NoGoodByte = 29
        End Enum
#End Region

#Region "Electrodes"
        Public Enum ISE_Electrodes
            None = 0
            Li = 1
            Na = 2
            K = 3
            Cl = 4
            Ref = 5
        End Enum
#End Region

#Region "ISE Calibrations"
        'JB 25/07/2012 - Adapted to DB content
        Public Enum ISEConditioningTypes
            CALB 'ISE Electrode calibration
            PMCL 'ISE Pump calibration
            BBCL 'ISE Bubble detection
            CLEN 'ISE Cleaning
        End Enum

        'JB 25/07/2012 Deleted ISE ActionType 
        'Public Enum ISECalibrationActionTypes
        '    NONE = 0
        '    AUTO = 1
        '    USER = 2
        'End Enum

#End Region

#Region "Peristaltic Pumps"
        Public Enum ISE_Pumps
            None
            A
            B
            W
        End Enum
#End Region

#Region "Bubble Detector"
        Public Enum ISE_Bubble_Detector
            None
            A
            M
            L
        End Enum

#End Region

#Region "Tests"
        ' XB 05/09/2014 - BA-1902
        Public Enum ISE_Tests
            Na = 1
            K = 2
            Cl = 3
            Li = 4
        End Enum
#End Region

#End Region

#Region "SERVICE SOFTWARE"
        '''' <summary>
        '''' Define the Enumeration for Types of Scripts
        '''' </summary>
        '''' <remarks>
        '''' Created by: XBC 22/09/2010
        '''' </remarks>
        'Public Enum SCRIPT_TYPES
        '    FACTORY
        '    USER
        'End Enum

        '''' <summary>
        '''' Define the possible response types returned by the  Analyzer after invoking and Action
        '''' </summary>
        '''' <remarks>Created by SG 23/09/2010</remarks>
        'Public Enum RESPONSE_TYPES
        '    OKNG
        '    DATA
        'End Enum

        ''' <summary>
        ''' Defines the possible results of the scripts validation
        ''' </summary>
        ''' <remarks>Created by SG 27/09/2010</remarks>
        Public Enum CHECK_RESULTS
            OK
            SYNTAX_ERROR
            SEQUENCE_ERROR
        End Enum

        Public Enum ADJUSTMENT_MODES
            _NONE

            ' status couple to read adjustments 
            ADJUSTMENTS_READING
            ADJUSTMENTS_READED
            'FORM_LOADING   in use ¿?
            ' status couple to load screens 
            LOADING
            LOADED
            ' status couple to prepare adjust phase
            ADJUST_PREPARING
            ADJUST_PREPARED
            ' status couple to make an adjustment
            ADJUSTING
            ADJUSTED

            TEST_PREPARING
            TEST_PREPARED

            ' status couple to make a test
            TESTING
            TESTED
            ' status to close screens
            CLOSING
            ' status couple to make a save operation
            SAVING
            SAVED

            SAVED_LOCAL 'SGM 29/02/2012

            ' status couple to make a parking operation
            PARKING
            PARKED
            ' status to get out to adjustment
            ADJUST_EXITING
            ' status group used for processes similar to intermediate Tanks 1/3
            TEST_START      ' 1/3
            'TEST_STARTED    ' 2/3
            'TEST_ENDED      ' 3/3

            TANKS_EMPTY_LC_REQUEST '1/3 
            TANKS_FILL_DW_REQUEST '2/3 
            TANKS_TRANSFER_DW_LC_REQUEST '3/3

            TANKS_EMPTY_LC_REQUEST_OK '1/3 
            TANKS_FILL_DW_REQUEST_OK '2/3 
            TANKS_TRANSFER_DW_LC_REQUEST_OK '3/3

            TANKS_EMPTY_LC_ENDED '1/3 
            TANKS_FILL_DW_ENDED '2/3 
            TANKS_TRANSFER_DW_LC_ENDED '3/3

            ' status couple to exit of a test
            TEST_EXITING
            TEST_EXITED
            ' status couple to scan absorbance
            ABSORBANCE_SCANNING
            ABSORBANCE_SCANNED
            ABSORBANCE_PREPARING
            ABSORBANCE_PREPARED
            ' error mode status 
            ERROR_MODE

            'SGM 02/02/2011
            ' status couple to make homes
            HOME_PREPARING
            HOME_FINISHED

            'XBC 25/02/2011
            ' status couple to read Leds intensities
            LEDS_READING
            LEDS_READED

            'XBC 22/03/2011
            ' status couple to read Stress status
            STRESS_READING
            STRESS_READED

            'XBC 25/05/2011
            ' status couple to read Fw Information 
            FW_READING
            FW_READED
            'SGM 28/06/2011
            FW_UPDATING
            FW_UTIL_RECEIVED
            FW_UPDATED

            'XBC 25/05/2011
            ' status couple to STANDBY operation
            STANDBY_DOING
            STANDBY_DONE

            'SGM 05/07/2011
            SLEEP_DOING
            SLEEP_DONE

            'SGM 31/05/2011 (motors, pumps, valves)
            MBEV_ALL_SWITCHING_OFF
            MBEV_ALL_SWITCHED_OFF

            MBEV_ALL_ARMS_TO_WASHING
            MBEV_ALL_ARMS_IN_WASHING

            MBEV_WASHING_STATION_TO_NROTOR              ' XB 15/10/2014 - BA-2004
            MBEV_WASHING_STATION_IS_NROTOR_PERFORMED    ' XB 15/10/2014 - BA-2004
            MBEV_WASHING_STATION_TO_DOWN
            MBEV_WASHING_STATION_IS_DOWN

            MBEV_WASHING_STATION_TO_UP
            MBEV_WASHING_STATION_IS_UP

            MBEV_ALL_ARMS_TO_PARKING
            MBEV_ALL_ARMS_IN_PARKING

            MBEV_ARM_TO_WASHING
            MBEV_ARM_IN_WASHING

            'XBC 31/05/2011
            ' status couple to read Instrument Information 
            ANALYZER_INFO_READING
            ANALYZER_INFO_READED

            CYCLES_READING
            CYCLES_READED
            CYCLES_WRITTING
            CYCLES_WRITTEN

            'reset analyzer
            ANALYZER_RESETING
            ANALYZER_IS_RESET

            'SGM 10/10/2012
            COLLISION_TEST_ENABLING
            COLLISION_TEST_ENABLED
            COLLISION_TEST_DISABLING

            STIRRER_TEST
            STIRRER_TESTING
            STIRRER_TESTED

            'SGM 23/09/2011
            WASH_DOING
            WASH_DONE

            'SGM 01/12/2011
            THERMO_ARM_TO_PARKING
            THERMO_ARM_IN_PARKING
            THERMO_ARM_TO_WASHING
            THERMO_ARM_IN_WASHING

            'SGM 14/12/2011
            FREQUENCY_READING
            FREQUENCY_READED
            LEVEL_DETECTING
            LEVEL_DETECTED

            'SGM 15/10/2012
            SN_SAVING
            SN_SAVED

            NEW_ROTOR_START
            NEW_ROTOR_END

            ' XB 15/10/2014 - BA-2004
            FINE_OPTICAL_CENTERING_PERFORMING
            FINE_OPTICAL_CENTERING_DONE

        End Enum



        'Scripts Queue
        Public Enum EVALUATE_TYPES
            NONE
            NUM_VALUE
            SENSOR_VALUE
            TEXT_VALUE
            EQUAL
            HIGHER
            LOWER
            EQUAL_HIGHER
            EQUAL_LOWER
        End Enum

        Public Enum RESPONSE_TYPES
            OK
            NG
            TIMEOUT
            EXCEPTION
            START
        End Enum

        Public Enum ADJUSTMENT_GROUPS

            _NONE

            'positions arms
            SAMPLES_ARM_DISP1
            SAMPLES_ARM_DISP2
            SAMPLES_ARM_ZREF
            SAMPLES_ARM_WASH
            SAMPLES_ARM_RING1
            SAMPLES_ARM_RING2
            SAMPLES_ARM_RING3
            SAMPLES_ARM_ZTUBE1
            SAMPLES_ARM_ZTUBE2
            SAMPLES_ARM_ZTUBE3
            SAMPLES_ARM_ISE
            SAMPLES_ARM_PARK
            SAMPLES_ARM_VSEC
            'SAMPLES_ARM_ZREFR
            SAMPLES_ARM_VSECWS
            SAMPLES_ARM_LEVEL_DET
            SAMPLES_ARM_EXCESS

            REAGENT1_ARM_DISP1
            REAGENT1_ARM_ZREF
            REAGENT1_ARM_WASH
            REAGENT1_ARM_RING1
            REAGENT1_ARM_RING2
            REAGENT1_ARM_PARK
            REAGENT1_ARM_VSEC
            REAGENT1_ARM_LEVEL
            REAGENT1_ARM_VSECWS

            REAGENT2_ARM_DISP1
            REAGENT2_ARM_ZREF
            REAGENT2_ARM_WASH
            REAGENT2_ARM_RING1
            REAGENT2_ARM_RING2
            REAGENT2_ARM_PARK
            REAGENT2_ARM_VSEC
            REAGENT2_ARM_LEVEL
            REAGENT2_ARM_VSECWS

            MIXER1_ARM_DISP1
            MIXER1_ARM_ZREF
            MIXER1_ARM_WASH
            MIXER1_ARM_PARK
            MIXER1_ARM_VSEC
            MIXER1_ARM_LEVEL
            MIXER1_ARM_VSECWS

            MIXER2_ARM_DISP1
            MIXER2_ARM_ZREF
            MIXER2_ARM_WASH
            MIXER2_ARM_PARK
            MIXER2_ARM_VSEC
            MIXER2_ARM_LEVEL
            MIXER2_ARM_VSECWS

            'thermos
            THERMOS_REAGENT1
            THERMOS_REAGENT2
            THERMOS_FRIDGE
            THERMOS_PHOTOMETRY
            THERMOS_WS_HEATER

            'various
            'OPTIC_CENTERING
            WASHING_STATION
            WASHING_STATION_PARK
            'BARCODE_WELL
            'BARCODE_BOTTLE

            'tanks
            'BALANCE_TANKS
            WASHING_SOLUTION
            HIGH_CONTAMINATION

            'INTERMEDIATE TANKS
            INTERNAL_TANKS
            TANKS_EMPTY_LC '1/3 TANKTESTEMPTYLC
            TANKS_FILL_DW '2/3 TANKTESTFILLDW
            TANKS_TRANSFER_DW_LC '3/3 TANKTESTTRANSFER

            'photometry
            PHOTOMETRY
            'BASELINE
            'DARKNESS_COUNTS
            IT_EDITION
            REPEATABILITY
            STABILITY
            ABSORBANCE_MEASUREMENT
            CHECK_ROTOR
            LEDS_CURRENT_REF

            'Fluidic System
            FLUIDIC_IN_OUT_SYSTEM

            'Level detection
            SAMPLE_LEVEL_DET
            REAGENT1_LEVEL_DET
            REAGENT2_LEVEL_DET

            'motors
            MOTORS_PUMPS_VALVES

            'configuration
            ALARMS_CONFIG

            'ise
            ISE_MODULE

            'rotors
            SAMPLES_ROTOR_BC
            REAGENTS_ROTOR_BC

        End Enum

        'SGM 25/10/2011
        Public Enum APPLICATION_PAGES
            POS_PHOTOMETRY
            POS_WASHING_STATION
            POS_ARMS
            THERMOS_WS
            THERMOS_ROTOR
            THERMOS_NEEDLES
            TANKS_SCALES
            TANKS_INTERMEDIATE
            SCADA_DOSING
            SCADA_EX_WASH
            SCADA_WS_ASP
            SCADA_WS_DISP
            SCADA_ENCODER
            SCADA_INOUT
            UPDATE_ADJ
            UPDATE_FW
            DEMO
            STRESS
            LEVEL_DETECTION
            COLISION
            PHOTO_BASELINE
            PHOTO_METROLOGY
            PHOTO_ROTOR
            BARCODE
            ISE_UTIL
        End Enum

        Public Enum ARMS_ELEMENTS
            ID      ' identifier
            TMP    ' board temperature
            MH      ' Horizontal Motor
            MHH     ' Home Horizontal Motor
            MHA     ' Horizontal Position
            MV      ' Vertical Motor
            MVH     ' Home Vertical Motor
            MVA     ' Vertical Position
            CYC     'Cycle counts PDT SPEC
        End Enum

        Public Enum PROBES_ELEMENTS
            ID      ' identifier
            TMP     ' board temperature
            DST     ' Detection status
            DFQ     ' Detection Base Frequency
            D       ' Detection
            DCV     ' Internal Rate of Change in last detection
            PTH     ' Probe Thermistor Value
            PTHD    ' Probe Thermistor Diagnostic
            PH      ' Probe Heater state
            PHD     ' Probe Heater Diagnostic
            CD      ' Collision Detector
            CYC     'Cycle counts PDT SPEC

        End Enum

        Public Enum ROTORS_ELEMENTS
            ID      ' identifier
            TMP     ' board temperature
            MR      ' Rotor Motor
            MRH     ' Home Motor Rotor
            MRA     ' Position
            FTH     ' Fridge Thermistor Value
            FTHD    ' Fridge Thermistor Diagnostic
            FH      ' Fridge Peltiers state
            FHD     ' Fridge Peltiers Diagnostic
            PF1     ' Peltier Fan 1 Speed
            PF1D    ' Peltier Fan 1 Diagnostic
            PF2     ' Peltier Fan 2 Speed
            PF2D    ' Peltier Fan 2 Diagnostic
            PF3     ' Peltier Fan 3 Speed
            PF3D    ' Peltier Fan 3 Diagnostic
            PF4     ' Peltier Fan 4 Speed
            PF4D    ' Peltier Fan 4 Diagnostic
            FF1     ' Frame Fan 1 Speed
            FF1D    ' Frame Fan 1 Diagnostic
            FF2     ' Frame Fan 2 Speed
            FF2D    ' Frame Fan 2 Diagnostic
            RC      ' Rotor Cover
            CB      ' Codebar Reader state
            CBE     ' Codebar Reader error
            CYC     'Cycle counts PDT SPEC
        End Enum

        Public Enum HOMES
            NONE
            ALL_ARMS 'SGM 10/03/11
            ALL_ROTORS 'SGM 10/03/11
            SAMPLES_ARM_POLAR
            SAMPLES_ARM_Z
            REAGENT1_ARM_POLAR
            REAGENT1_ARM_Z
            REAGENT2_ARM_POLAR
            REAGENT2_ARM_Z
            MIXER1_ARM_POLAR
            MIXER1_ARM_Z
            MIXER2_ARM_POLAR
            MIXER2_ARM_Z
            SAMPLES_ROTOR
            REAGENTS_ROTOR
            REACTIONS_ROTOR
            WASHING_STATION_Z
        End Enum

        Public Enum AXIS
            _NONE
            FULL
            EMPTY
            MIDDLE '??
            POLAR
            Z
            ROTOR
            REL_Z
            SETPOINT
            TARGET
            ENCODER
        End Enum

        Public Enum MOVEMENT
            _NONE
            HOME
            RELATIVE
            ABSOLUTE
        End Enum

        Public Enum FwSCRIPTS_IDS
            ' Read Adjustments
            READ_ADJUSTMENTS
            ' Read Sensors
            READ_SENSORS
            ' Photometry
            READ_LEDS
            SAVE_LEDS
            'READ_BASELINE --> BLIGHT
            ' Stress Mode
            READ_STRESS_STATUS
            START_STRESS_MODE
            ' Common
            PRIME_SYSTEM
            FILL_WELLS
            TEST_EXIT
            ' Save Adjustments
            SAVE_ADJUSTMENTS
            ' Homes
            WASHING_STATION_HOME_Z
            SAMPLE_ARM_HOME_POLAR
            SAMPLE_ARM_HOME_Z
            REAGENT1_ARM_HOME_POLAR
            REAGENT1_ARM_HOME_Z
            REAGENT2_ARM_HOME_POLAR
            REAGENT2_ARM_HOME_Z
            MIXER1_ARM_HOME_POLAR
            MIXER1_ARM_HOME_Z
            MIXER2_ARM_HOME_POLAR
            MIXER2_ARM_HOME_Z
            SAMPLES_HOME_ROTOR
            REAGENTS_HOME_ROTOR
            REACTIONS_HOME_ROTOR
            REACTIONS_ROTOR_HOME_WELL1
            REACTIONS_ROTOR_AUTO_CENTERING
            ' Absolute Movements
            WASHING_STATION_ABS_Z
            WASHING_STATION_PARK
            SAMPLE_ARM_ABS_POLAR
            SAMPLE_ARM_ABS_Z
            REAGENT1_ARM_ABS_POLAR
            REAGENT1_ARM_ABS_Z
            REAGENT2_ARM_ABS_POLAR
            REAGENT2_ARM_ABS_Z
            MIXER1_ARM_ABS_POLAR
            MIXER1_ARM_ABS_Z
            MIXER2_ARM_ABS_POLAR
            MIXER2_ARM_ABS_Z
            SAMPLES_ABS_ROTOR
            SAMPLES_ZTUBE_ABS_ROTOR 'SGM 02/03/2012 absolute movement for adjusting Z Tube
            REAGENTS_ABS_ROTOR
            REACTIONS_ABS_ROTOR
            SAMPLE_ARM_ABS_UP
            SAMPLE_ARM_ABS_DOWN
            REAGENT1_ARM_ABS_UP
            REAGENT1_ARM_ABS_DOWN
            REAGENT2_ARM_ABS_UP
            REAGENT2_ARM_ABS_DOWN
            MIXER1_ARM_ABS_UP
            MIXER1_ARM_ABS_DOWN
            MIXER2_ARM_ABS_UP
            MIXER2_ARM_ABS_DOWN
            ' Relative Movements
            WASHING_STATION_REL_Z
            SAMPLE_ARM_REL_POLAR
            SAMPLE_ARM_REL_Z
            REAGENT1_ARM_REL_POLAR
            REAGENT1_ARM_REL_Z
            REAGENT2_ARM_REL_POLAR
            REAGENT2_ARM_REL_Z
            MIXER1_ARM_REL_POLAR
            MIXER1_ARM_REL_Z
            MIXER2_ARM_REL_POLAR
            MIXER2_ARM_REL_Z
            SAMPLES_REL_ROTOR
            REAGENTS_REL_ROTOR
            REACTIONS_REL_ROTOR
            REACTIONS_REL_ROTOR_2SECONDS
            ' Arm Wash
            'SAMPLE_ARM_WASH    Not used !
            'REAGENT1_ARM_WASH    Not used !
            'REAGENT2_ARM_WASH    Not used !
            'MIXER1_ARM_WASH    Not used !
            'MIXER2_ARM_WASH    Not used !

            'Arm Park PDT
            'SAMPLE_PARK_WASH    Not used !
            'REAGENT1_PARK_WASH    Not used !
            'REAGENT2_PARK_WASH    Not used !
            'MIXER1_PARK_WASH    Not used !
            'MIXER2_PARK_WASH    Not used !

            MIXER1_ON
            MIXER1_OFF
            MIXER2_ON
            MIXER2_OFF

            ' Reactions Rotor
            PLACE_WELL1_GLF
            FILL_WELL_GLF
            'Optic Centering
            READ_COUNTS
            READ_COUNTS_ENCODER
            ' Specified movements
            SAMPLE_ARM_DISP
            REAGENT1_ARM_DISP
            REAGENT2_ARM_DISP
            MIXER1_ARM_DISP
            MIXER2_ARM_DISP

            '''''''''''''''''''''''''''''''''''
            'THERMOS ADJUSTMENTS SGM 14/10/2011
            'WS HEATER:
            WS_HEATER_TEST
            WS_HEATER_TEST_OLD 'old prototype

            'ROTOR
            ROTOR_PRIME_INI
            ROTOR_PRIME_STEP
            ROTOR_PRIME_END
            ROTOR_FILL_STEP
            ROTOR_ROTATE

            'NEEDLES
            NEEDLES_PRIME_R1
            NEEDLES_PRIME_R2
            NEEDLES_DISPENSE_R1
            NEEDLES_DISPENSE_R2

            '''''''''''''''''''''''''''''''''''

            'Homes
            HOME_ALL
            HOME_ALL_ROTORS
            HOME_ALL_ARMS

            '*****************MOTORS, PUMPS, VALVES******************

            ALL_ARMS_TO_WASH
            ALL_ARMS_TO_PARK
            ALL_ITEMS_TO_DEFAULT
            R1_ARM_TO_WASH
            R2_ARM_TO_WASH
            A1_ARM_TO_WASH
            A2_ARM_TO_WASH

            TEST_ENCODER

            'WS Motor
            GLF_MP_HOME 'Motor
            GLF_MP_REL
            GLF_MP_ABS

            'INTERNAL DOSING
            INDO_EXIT_TEST
            JE1_MS_HOME 'Samples syringe
            JE1_MS_REL
            JE1_MS_ABS
            JE1_MR1_HOME 'Reagent1 syringe
            JE1_MR1_REL
            JE1_MR1_ABS
            JE1_MR2_HOME 'Reagent2 syringe
            JE1_MR2_REL
            JE1_MR2_ABS



            JE1_B1_ON 'Samples Dosing pump
            JE1_B1_OFF
            JE1_B2_ON 'Reagent1 Dosing pump
            JE1_B2_OFF
            JE1_B3_ON 'Reagent2 Dosing pump
            JE1_B3_OFF

            JE1_EV1_ON 'Samples Dosing valve
            JE1_EV1_OFF
            JE1_EV2_ON 'Reagent1 Dosing valve
            JE1_EV2_OFF
            JE1_EV3_ON 'Reagent2 Dosing valve
            JE1_EV3_OFF

            JE1_EV4_ON 'Air OR Washing Solution Valve
            JE1_EV4_OFF
            JE1_EV5_ON 'Air/Washing OR Purified Water Solution Valve
            JE1_EV5_OFF

            'EXTERNAL WASHING
            SF1_B1_ON 'Samples Arm
            SF1_B1_OFF
            SF1_B2_ON 'Reagent1/Mixer2 Arm
            SF1_B2_OFF
            SF1_B3_ON 'Reagent2/Mixer1 Arm
            SF1_B3_OFF

            'WASHING STATION (aspiration)
            SF1_B6_ON 'Washing Station needle 2,3
            SF1_B6_OFF
            SF1_B7_ON 'Washing Station needle 4,5
            SF1_B7_OFF
            SF1_B8_ON 'Washing Station needle 6
            SF1_B8_OFF
            SF1_B9_ON 'Washing Station needle 7
            SF1_B9_OFF
            SF1_B10_ON 'Washing Station needle 1
            SF1_B10_OFF

            'WASHING STATION (dispensation)
            SF1_MS_HOME 'Washing Station Dispensation Motor
            SF1_MS_REL
            SF1_MS_ABS
            SF1_GE1_ON 'Washing Station Dispensation Electrovalves
            SF1_GE1_OFF


            'IN/OUT
            SF1_EV1_ON 'Purified Water (from external source)
            SF1_EV1_OFF
            SF1_EV2_ON 'Purified Water (from external tank)
            SF1_EV2_OFF
            SF1_EV3_ON 'reserva
            SF1_EV3_OFF
            SF1_B4_ON 'Purified Water (from external tank)
            SF1_B4_OFF
            SF1_B5_ON 'Low Contamination
            SF1_B5_OFF

            'COLLISION DETECTION

            ' XBC 10/11/2011 - repeated !
            ''Photometrics
            'GLF_MR_HOME 'Reactions Rotor Motor
            'GLF_MR_REL
            'GLF_MR_ABS
            ' XBC 10/11/2011 - repeated !

            GLF_MW_HOME 'Washing Station Motor (pinta)
            GLF_MW_REL
            GLF_MW_ABS

            '*****************THERMOS******************

            SET_TCO_REACTIONS_ROTOR
            SET_TCO_REAGENT1
            SET_TCO_REAGENT2
            SET_TCO_HEATER

            TEMPER_REAGENT1_NEEDLE
            TEMPER_REAGENT2_NEEDLE
            TAKE_TEMP_REAGENT1_NEEDLE
            TAKE_TEMP_REAGENT2_NEEDLE

            TEMPER_HEATER

            '***********LEVEL DETECTION*********************
            SAMPLE_LEVEL_DET
            REAGENT1_LEVEL_DET
            REAGENT2_LEVEL_DET

            SAMPLE_TEST_END
            REAGENT1_TEST_END
            REAGENT2_TEST_END


            '*************BAR CODE**************************
            SWITCH_ON_BARCODE
            SWITCH_OFF_BARCODE

        End Enum

        Public Enum SENSOR
            DISTILLED_WATER_FULL
            DISTILLED_WATER_EMPTY
            LOW_CONTAMINATION_FULL
            LOW_CONTAMINATION_EMPTY


            'MONITOR
            WASHING_SOLUTION_LEVEL
            HIGH_CONTAMINATION_LEVEL

            REACTIONS_ROTOR_TEMP
            FRIDGE_TEMP
            REAGENTS_ROTOR1_TEMP
            REAGENTS_ROTOR2_TEMP
            WASHING_STATION_HEATER_TEMP

            MAIN_COVER
            REAGENTS_COVER
            SAMPLES_COVER
            REACTIONS_COVER
            'CONNECTED
        End Enum


        Public Enum STRESS_STATUS
            NOT_STARTED
            UNFINISHED
            FINISHED_OK
            FINISHED_ERR
        End Enum

        Public Enum STRESS_TYPE
            ERASE_DATA = 0
            STOP_SDMODE = 1
            COMPLETE = 2
            SAMPLE_ARM_MH = 3
            SAMPLE_ARM_MV = 4
            REAGENT1_ARM_MH = 5
            REAGENT1_ARM_MV = 6
            REAGENT2_ARM_MH = 7
            REAGENT2_ARM_MV = 8
            MIXER1_ARM_MH = 9
            MIXER1_ARM_MV = 10
            MIXER2_ARM_MH = 11
            MIXER2_ARM_MV = 12
            SAMPLES_ROTOR = 13
            REACTIONS_ROTOR = 14
            REAGENTS_ROTOR = 15
            WASHING_STATION = 16
            SAMPLE_SYRINGE = 17
            REAGENT1_SYRINGE = 18
            REAGENT2_SYRINGE = 19
        End Enum

        Public Enum STRESS_ERRORS ' PDT !!! TO SPEC
            NONE
            ERROR1
            ERROR2
            ERROR3
        End Enum

        Public Enum FILL_MODE
            AUTOMATIC = 1
            MANUAL = 0
        End Enum

        Public Enum SWITCH_ACTIONS

            _NONE

            'boolean
            SET_TO_ON
            SET_TO_OFF

            '3 way valves
            SET_TO_0_1
            SET_TO_0_2
            SET_TO_1_2
        End Enum

        Public Enum FW_INFO
            ID      'identifier
            SMC     'board serial num
            RV      'Repository version
            CRC     'Repository CRC32 result (OK,NOK)
            CRCV    'Repository CRC32 value 
            CRCS    'Repository CRC32 size
            FWV     ' Fw version (CPU)
            FWCRC   'CPU CRC32 result (OK,NOK)
            FWCRCV  'CPU CRC32 value
            FWCRCS  'CPU CRC32 size
            HWV     'Hw version
            ASN     'Serial number

        End Enum


        Public Enum POLL_IDs
            CPU = 1
            BR1 = 2
            BR2 = 3
            BM1 = 4
            AG1 = 5
            AG2 = 6
            DR1 = 7
            DR2 = 8
            DM1 = 9
            RR1 = 10
            RM1 = 11
            GLF = 12
            JE1 = 13
            SF1 = 14
        End Enum

        Public Enum SUBSYSTEMS
            MANIFOLD
            FLUIDICS
            PHOTOMETRICS
        End Enum


        Public Enum CYCLE_UNITS 'PDT to SPEC
            x100STEPS
            x1000STEPS
            x10000STEPS
            SWITCHES
            HOURS
            MINUTES
            CONTAMINATIONS
        End Enum

#Region "POLLHW Elements"

        Public Enum HW_BOOL_STATES
            _OFF = 0
            _ON = 1
        End Enum

        Public Enum HW_DC_STATES
            DI = 0 'disabled (OFF)
            EN = 1 'enabled (ON)
        End Enum

        Public Enum HW_MOTOR_HOME_STATES
            C = 0 'closed
            O = 1 'open
            KO = 2 'error
        End Enum

        Public Enum HW_TANK_LEVEL_SENSOR_STATES
            EMPTY = 0 'Low sensor Down - High Sensor Down (Empty)
            MIDDLE = 1    'Low sensor High - High Sensor Down (Middle)
            SPECIAL = 2   'Low Sensor Down - High sensor High (En Running es un estado imposible - error) (En servicio es posible con manipulación)
            FULL = 3  'Both sensors High (Full)

        End Enum


        Public Enum HW_GENERIC_DIAGNOSIS
            OK = 0
            KO = 1
        End Enum

        Public Enum FW_GENERIC_RESULT
            OK = 0
            KO = 1
        End Enum

        Public Enum HW_CLOT_DIAGNOSIS
            OK = 0    'Clot free
            BS = 1    'Fluid system Blocked. 
            CD = 2    'Clot Detected
            CP = 3    'Clot Possible
            KO = 4    'Out of Scale - Impossible Value - Error

        End Enum

        Public Enum HW_THERMISTROR_DIAGNOSIS
            OK = 0    'Ok
            OPEN = 1  'Open circuit
            CLOSED = 2    'Closed circuit
            NO_RISE = 3   'Temperature doesn't Rise
            NO_DECREASE = 4   'Temperature doesn't decrease

        End Enum

        Public Enum HW_COLLISION_STATES
            O = 0 'OK
            C = 1 'collision
        End Enum


        Public Enum HW_SWITCH_STATES
            C = 0 'closed
            O = 1 'open
        End Enum

        Public Enum HW_PHOTOMETRY_STATES
            OK = 0    'OK
            CONNECTION_FAIL = 1   'Fail connection
            ERROR_DDC112 = 2  'DDC112 Error

        End Enum

        Public Enum HW_PELTIER_DIAGNOSIS
            OK = 0    'Ok
            OPEN = 1  'Open circuit
            CLOSED = 2    'Closed circuit
            OVERTEMP = 3  'Over temperature
            OUTCONTROL = 4    'Out of control
        End Enum

        Public Enum MANIFOLD_ELEMENTS
            JE1_TEMP = 1

            'Reagent1 motor
            JE1_MR1 = 2 'state (dc)
            JE1_MR1H = 3 'home
            JE1_MR1A = 4 'position
            'Reagent2 motor
            JE1_MR2 = 5 'state (dc)
            JE1_MR2H = 6 'home
            JE1_MR2A = 7 'position
            'Samples motor
            JE1_MS = 8 'state (dc)
            JE1_MSH = 9 'home
            JE1_MSA = 10 'position
            'Samples Dosing pump
            JE1_B1 = 11 'state (dc)
            JE1_B1D = 12 'diagnosis
            'Reagent1 Dosing pump
            JE1_B2 = 13 'state (dc)
            JE1_B2D = 14 'diagnosis
            'Reagent2 Dosing pump
            JE1_B3 = 15 'state (dc)
            JE1_B3D = 16 'diagnosis
            'Samples Dosing valve
            JE1_EV1 = 17 'state (bool)
            JE1_EV1D = 18 'diagnosis
            'Reagent1 Dosing valve
            JE1_EV2 = 19 'state (bool)
            JE1_EV2D = 20 'diagnosis
            'Reagent2 Dosing valve
            JE1_EV3 = 21 'state (bool)
            JE1_EV3D = 22 'diagnosis
            'Air OR Washing Solution Valve
            JE1_EV4 = 23 'state (bool)
            JE1_EV4D = 24 'diagnosis
            'Air/Washing OR Purified Water Solution Valve
            JE1_EV5 = 25 'state (bool)
            JE1_EV5D = 26 'diagnosis

            'not used ***********************
            JE1_EV6 = 27 'state (bool)
            JE1_EV6D = 28 'diagnosis
            JE1_GE1 = 29
            JE1_GE1D = 30
            JE1_GE2 = 31
            JE1_GE2D = 32
            JE1_GE3 = 33
            JE1_GE3D = 34
            '***********************************
            'Clot detection
            JE1_CLT = 35 'ascii value
            JE1_CLTD = 36 'diagnosis


        End Enum

        Public Enum FLUIDICS_ELEMENTS
            SF1_TEMP = 1

            SF1_MS = 2 'state (dc)
            SF1_MSH = 3 'home
            SF1_MSA = 4 'position

            'Samples Arm
            SF1_B1 = 5 'state (dc)
            SF1_B1D = 6 'diagnosis
            'Reagent1/Mixer2 Arm
            SF1_B2 = 7 'state (dc)
            SF1_B2D = 8 'diagnosis
            'Reagent2/Mixer1 Arm
            SF1_B3 = 9 'state (dc)
            SF1_B3D = 10 'diagnosis

            'Purified Water (from external tank)
            SF1_B4 = 11 'state (dc)
            SF1_B4D = 12 'diagnosis
            'Low Contamination
            SF1_B5 = 13 'state (dc)
            SF1_B5D = 14 'diagnosis

            'Washing Station needle 2,3
            SF1_B6 = 15 'state (dc)
            SF1_B6D = 16 'diagnosis
            'Washing Station needle 4,5
            SF1_B7 = 17 'state (dc)
            SF1_B7D = 18 'diagnosis
            'Washing Station needle 6
            SF1_B8 = 19 'state (dc)
            SF1_B8D = 20 'diagnosis
            'Washing Station needle 7
            SF1_B9 = 21 'state (dc)
            SF1_B9D = 22 'diagnosis
            'Washing Station needle 1
            SF1_B10 = 23 'state (dc)
            SF1_B10D = 24 'diagnosis

            'Electrovalves pump
            SF1_GE1 = 25 'state (dc)
            SF1_GE1D = 26 'diagnosis

            'Purified Water (from external source)
            SF1_EV1 = 27 'state (bool)
            SF1_EV1D = 28 'diagnosis
            'Purified Water (from external tank)
            SF1_EV2 = 29 'state (bool)
            SF1_EV2D = 30 'diagnosis
            'reserva
            SF1_EV3 = 31 'state (bool)
            SF1_EV3D = 32 'diagnosis

            'Washing Station Thermistor
            SF1_WSTH = 33 'value (0-50)
            SF1_WSTHD = 34 'diagnosis

            'Washing Station Heater
            SF1_WSH = 35 'state (dc)
            SF1_WSHD = 36 'diagnosis

            'Washing Solution Weight
            SF1_WSW = 37 'ascii value
            SF1_WSWD = 38 'diagnosis

            'High Contamination Weight
            SF1_HCW = 39 'ascii value
            SF1_HCWD = 40 'diagnosis

            'Waste sensor (boyas)
            SF1_WAS = 41 'state
            SF1_WTS = 42 'operation state (dc)  EN=Emptying

            'System liquid sensor (boyas)
            SF1_SLS = 43 'state
            SF1_STS = 44 'operation state (dc)  EN=Filling

            'stirrer 1
            SF1_ST1 = 45 'state (dc)

            'stirrer 2
            SF1_ST2 = 46 'state (dc)


        End Enum

        Public Enum PHOTOMETRICS_ELEMENTS
            GLF_TEMP = 1
            'Reactions Rotor Motor
            GLF_MR = 2 'state (dc)
            GLF_MRH = 3 'home
            GLF_MRA = 4 'position
            GLF_MRE = 5 'encoder position
            GLF_MRED = 6 'encoder diagnosis

            'Washing Station Vertical Motor (Pinta)
            GLF_MW = 7 'state (dc)
            GLF_MWH = 8 'home
            GLF_MWA = 9 'position

            'Washing Station Collision Detector
            GLF_CD = 10 'collision

            'Rotor Thermistor
            GLF_PTH = 11 'value
            GLF_PTHD = 12 'diagnosis

            'Rotor Peltier
            GLF_PH = 13 'state(dc)
            GLF_PHD = 14 'diagnosis

            'Peltier Fan 1
            GLF_PF1 = 15 'speed
            GLF_PF1D = 16 'diagnosis

            'Peltier Fan 2
            GLF_PF2 = 17 'speed
            GLF_PF2D = 18 'diagnosis

            'Peltier Fan 3
            GLF_PF3 = 19 'speed
            GLF_PF3D = 20 'diagnosis

            'Peltier Fan 4
            GLF_PF4 = 21 'speed
            GLF_PF4D = 22 'diagnosis

            'Rotor Cover
            GLF_RC = 23 'closed or open

            'Photometry
            GLF_PHT = 24 'state

            'Photometry Flash Memory State
            GLF_PHFM = 25 'diagnois



        End Enum

        Public Enum CYCLE_ELEMENTS

            'MANIFOLD******************************************************
            'Reagent1 motor
            JE1_MR1_CYC = 1 'Cycles PDT

            'Reagent2 motor
            JE1_MR2_CYC = 2 'Cycles PDT

            'Samples motor
            JE1_MS_CYC = 3 'Cycles PDT

            'Samples Dosing pump
            JE1_B1_CYC = 4 'Cycles PDT

            'Reagent1 Dosing pump
            JE1_B2_CYC = 5 'Cycles PDT

            'Reagent2 Dosing pump
            JE1_B3_CYC = 6 'Cycles PDT

            'Samples Dosing valve
            JE1_EV1_CYC = 7 'Cycles PDT

            'Reagent1 Dosing valve
            JE1_EV2_CYC = 8 'Cycles PDT

            'Reagent2 Dosing valve
            JE1_EV3_CYC = 9 'Cycles PDT

            'Air OR Washing Solution Valve
            JE1_EV4_CYC = 10 'Cycles PDT

            'Air/Washing OR Purified Water Solution Valve
            JE1_EV5_CYC = 11 'Cycles PDT

            'not used ***********************
            JE1_EV6_CYC = 12 'Cycles PDT
            JE1_GE1_CYC = 13 'Cycles PDT
            JE1_GE2_CYC = 14 'Cycles PDT
            JE1_GE3_CYC = 15 'Cycles PDT

            '***********************************
            'Clot detection
            JE1_CLT_CYC = 16 'Cycles PDT


            'FLUIDICS**************************************************************
            'Cycles PDT
            SF1_MS_CYC = 21 'Cycles PDT

            'Samples Arm
            SF1_B1_CYC = 22 'Cycles PDT

            'Reagent1/Mixer2 Arm
            SF1_B2_CYC = 23 'Cycles PDT

            'Reagent2/Mixer1 Arm
            SF1_B3_CYC = 24 'Cycles PDT

            'Purified Water (from external tank)
            SF1_B4_CYC = 25 'Cycles PDT

            'Low Contamination
            SF1_B5_CYC = 26 'Cycles PDT

            'Washing Station needle 2,3
            SF1_B6_CYC = 27 'Cycles PDT

            'Washing Station needle 4,5
            SF1_B7_CYC = 28 'Cycles PDT

            'Washing Station needle 6
            SF1_B8_CYC = 29 'Cycles PDT

            'Washing Station needle 7
            SF1_B9_CYC = 30 'Cycles PDT

            'Washing Station needle 1
            SF1_B10_CYC = 31 'Cycles PDT

            'Electrovalves pump
            SF1_GE1_CYC = 32 'Cycles PDT

            'Purified Water (from external source)
            SF1_EV1_CYC = 33 'Cycles PDT

            'Purified Water (from external tank)
            SF1_EV2_CYC = 34 'Cycles PDT

            'reserva
            SF1_EV3_CYC = 35 'Cycles PDT

            'Washing Station Thermistor
            SF1_WSTH_CYC = 36 'Cycles PDT

            'Washing Station Heater
            SF1_WSH_CYC = 37 'Cycles PDT

            'Washing Solution Weight
            SF1_WSW_CYC = 38 'Cycles PDT

            'High Contamination Weight
            SF1_HCW_CYC = 39 'Cycles PDT

            'Waste sensor (boyas)
            SF1_WAS_CYC = 40 'Cycles PDT

            'System liquid sensor (boyas)
            SF1_SLS_CYC = 41 'Cycles PDT

            'stirrer 1
            SF1_ST1_CYC = 42 'Cycles PDT

            'stirrer 2
            SF1_ST2_CYC = 43 'Cycles PDT


            'PHOTOMETRICS*********************************************
            'Cycles PDT COUNTS

            'Reactions Rotor Motor
            GLF_MR_CYC = 51 'Cycles PDT

            'Washing Station Vertical Motor (Pinta)
            GLF_MW_CYC = 52 'Cycles PDT

            'Washing Station Collision Detector
            GLF_CD_CYC = 53 'Cycles PDT

            'Rotor Thermistor
            GLF_PTH_CYC = 54 'Cycles PDT

            'Rotor Peltier
            GLF_PH_CYC = 55 'Cycles PDT

            'Peltier Fan 1
            GLF_PF1_CYC = 56 'Cycles PDT

            'Peltier Fan 2
            GLF_PF2_CYC = 57 'Cycles PDT

            'Peltier Fan 3
            GLF_PF3_CYC = 58 'Cycles PDT

            'Peltier Fan 4
            GLF_PF4_CYC = 59 'Cycles PDT

            'Rotor Cover
            GLF_RC_CYC = 60 'Cycles PDT

            'Photometry
            GLF_PHT_CYC = 61 'Cycles PDT

            'Photometry Flash Memory State
            GLF_PHFM_CYC = 62 'Cycles PDT

        End Enum




        Public Enum CPU_ELEMENTS
            CPU_TEMP = 1

            CPU_CAN = 2   ' CAN BUS State
            ' CAN BUS Diagnostic ARMS
            CPU_CAN_BM1 = 3
            CPU_CAN_BR1 = 4
            CPU_CAN_BR2 = 5
            CPU_CAN_AG1 = 6
            CPU_CAN_AG2 = 7
            ' CAN BUS Diagnostic PROBES
            CPU_CAN_DM1 = 8
            CPU_CAN_DR1 = 9
            CPU_CAN_DR2 = 10
            ' CAN BUS Diagnostic ROTORS
            CPU_CAN_RR1 = 11
            CPU_CAN_RM1 = 12
            ' CAN BUS Diagnostic PHOTOMETRIC
            CPU_CAN_GLF = 13
            ' CAN BUS Diagnostic FLUIDICS
            CPU_CAN_SF1 = 14
            ' CAN BUS Diagnostic MANIFOLD
            CPU_CAN_JE1 = 15

            ' Main Cover
            CPU_MC = 16
            ' Buzzer State
            CPU_BUZ = 17
            ' Firmware Repository Flash Memory
            CPU_FWFM = 18
            ' Black Box Flash Memory
            CPU_BBFM = 19
            ' ISE State
            CPU_ISE = 20
            ' ISE Error
            CPU_ISEE = 21

            '*********************************************
            'CYCLES

            ' Main Cover
            CPU_MCCYC = 51 'Cycles PDT
            ' Buzzer State
            CPU_BUZCYC = 52 'Cycles PDT
            ' Firmware Repository Flash Memory
            CPU_FWFMCYC = 53 'Cycles PDT
            ' Black Box Flash Memory
            CPU_BBFMCYC = 54 'Cycles PDT
            ' ISE State
            CPU_ISECYC = 55 'Cycles PDT
            ' ISE Error
            CPU_ISEECYC = 56 'Cycles PDT
        End Enum

#End Region



        'Created by SGM 01/08/2011
        'Public Enum HISTORY_TASK_TYPES
        '    _NONE = 0
        '    ADJUSTMENT = 1
        '    TEST = 2
        '    UTILITY = 3
        'End Enum

        Public Enum HISTORY_RECOMMENDATIONS
            _NONE = 0
            ERR_COMM = 1
            ERR_BASELINE = 2
            WARNING_BLCOUNTS = 3
            WARNING_LEDS = 4
            ERR_METROLOGY = 5
            ERR_CONDITIONING_GLF_THERMO = 6
            ERR_MEASURING_GLF_THERMO = 7
            ERR_CONDITIONING_REA_THERMO = 8
            ERR_MEASURING_REA_THERMO = 9
            ERR_CONDITIONING_HEA_THERMO = 10
            WARNING_STRESS_RESETS = 11
            ERR_STRESS = 12
            STRESS_STOP_BY_USER = 13
            ADJ_CANCELLED_BY_USER = 14
        End Enum

        Public Enum PrimingArms
            NONE = 0
            SAMPLE = 1
            REAGENT1 = 2
            REAGENT2 = 3
        End Enum

        'SGM 04/10/2012
        Public Enum UTILInstructionTypes
            None = 0
            IntermediateTanks = 1
            NeedleCollisionTest = 2
            SaveSerialNumber = 3
        End Enum

        'SGM 04/10/2012
        Public Enum UTILIntermediateTanksTestActions
            NothingToDo = 0
            EmptyLowWaste = 1
            FillDistilledWater = 2
            TransferDistilledWater_To_LowWaste = 3
            StopAnyOperation = 4
        End Enum

        'SGM 04/10/2012
        Public Enum UTILCollisionTestActions
            Disable = 0
            Enable = 1
        End Enum

        'SGM 04/10/2012
        Public Enum UTILCollidedNeedles
            None = 0
            DR1 = 1
            DR2 = 2
            DM1 = 3
        End Enum

        'SGM 04/10/2012
        Public Enum UTILSaveSerialNumberActions
            NothingToDo = 0
            SaveSerialNumber = 1
        End Enum

        'SGM 04/10/2012
        Public Enum UTILSNSavedResults
            None = 0
            OK = 1
            KO = 2
        End Enum

        ' XBC 17/10/2012
        Public Enum ManagementAlarmTypes
            NONE = 0
            UPDATE_FW = 1
            FATAL_ERROR = 2
            RECOVER_ERROR = 3
            SIMPLE_ERROR = 4
            REQUEST_INFO = 5
            OMMIT_ERROR = 6
        End Enum

#End Region

#Region "Users"
        ' XBC 27/05/2011
        Public Enum USER_LEVEL
            lOPERATOR = 1
            lSUPERVISOR = 2
            lADMINISTRATOR = 3
            lBIOSYSTEMS = 4
        End Enum
#End Region

#Region "Firmware Instructions Tags"

        Public Enum Ax00InfoInstructionModes
            INF = 1 'Ask for details for real time monitoring
            ALR = 2 'Ask for error codes
            STR = 3 'START Refreshing general Values for Real Time Monitoring (every 2 seconds)
            STP = 4 'STOP Refreshing general Values for Real Time Monitoring 
        End Enum

        Public Enum Ax00BaseLineAutoFillInstructionModes
            USER = 0 'Well filled by user
            AUTO = 1 'Well filled automatically by analyzer (washing station)
        End Enum

        Public Enum Ax00WashStationControlModes
            DOWN = 1 'Washing Station Down
            UP = 2 'Washing Station Up
        End Enum

        Public Enum Ax00ArmWellStatusValues
            ID 'Idle
            ND 'Nothing to do
            DU 'Dummy
            R1 'Reagent1 dispensed
            S1 'Sample dispensed
            R2 'Reagent2 dispensed
            DI 'Ise dispensed
            WS 'WashSol dispensed
            DS 'Skip dispensed
            PD 'Predilution dispensed (from samples to photometric rotor)
            PS 'Diluted sample dispensed (from photometric to photometric rotor)
            LD 'Fail level detection (in samples rotor)
            LP 'Fail level detection predilution (in photometric rotor)
            KO 'Collision
        End Enum

        Public Enum Ax00ArmClotDetectionValues
            OK 'Clot free
            BS 'Fluid system blocked
            CD 'Clot detected
            CP 'Clot possible
        End Enum

        Public Enum Ax00CodeBarReader
            REAGENTS = 1 'Reagents barcode
            SAMPLES = 2 'Samples barcode
        End Enum

        Public Enum Ax00CodeBarAction
            CONFIG = 1 'Configurate
            FULL_ROTOR = 2 'Read full rotor
            SINGLE_POS = 3 'Read single position
            START_TEST_MODE = 4 'Start Test mode for adjustment
            END_TEST_MODE = 5 'Stop Test mode for adjustment
        End Enum

        Public Enum Ax00AnsCodeBarStatus
            CONFIG_DONE = 1 'Configuration done OK
            FULL_ROTOR_DONE = 2 'Full rotor read OK
            SINGLE_POS_DONE = 3 'Single position read OK

            ' XBC 19/04/2012 - specification's change to match with Ax00CodeBarAction
            'CODEBAR_ERROR = 4 'Code bar reader error
            'TEST_MODE_ANSWER = 5 'Test Mode Answer
            'TEST_MODE_ENDED = 6 'Test Mode Ended
            TEST_MODE_ANSWER = 4 'Test Mode Answer
            TEST_MODE_ENDED = 5 'Test Mode Ended
            CODEBAR_ERROR = 6 'Code bar reader error
            ' XBC 19/04/2012
        End Enum

        Public Enum Ax00AnsCodeBarDiagnosis
            OK = 1
            BAD_CODE = 2
            NO_READ = 3
        End Enum

        Public Enum Ax00CodificationsCodeBar
            Code128 = 1
            Code39 = 2
            CodeBar = 3
            PharmaCode = 4
            UpcEan = 5
            Code93 = 6
            Interleaved25 = 7
        End Enum

        Public Enum Ax00PollRDAction
            Biochemical = 1
            ISE = 2
            PreparationsWithProblem = 3
        End Enum

        'SGM 29/05/2012 FWUTIL parameters
        Public Enum FwUpdateActions
            None = 0
            StartUpdate = 1   'Inici Càrrega de repositori (en campo N, numero de bloques a enviar de 2048 Bytes, aunque el ultimo (resto) no sea completo)
            SendRepository = 2    'Càrrega de repositori (N=indice de bloque)
            QueryCRC32 = 3    'Query CRC32 calculated by Analyzer
            QueryNeeded = 4 'Queries for elements that are needed to be updated
            UpdateCPU = 5 'Actualització de CPU
            UpdatePER = 6    'Updates peripherals
            'UpdateAllPeripherals = 7  'Actualització forçada de TOTES les Perifèriques.
            UpdateMAN = 8   'Actualització de Maniobres i fitxer de Inicialitzacions. En un futur s'actualitzarà també ajustos sempre i quan respectant l'estructura actual.
            UpdateADJ = 9  'Updates default adjustments (fw adjustments) 
            'SaveCurrentAsFactory = 10 'Asignar valors de fàbrica (copia desde preprocesats a Fabrica)
            'SetCurrentFromFactory = 11    'Carregar valors de fàbrica (copia de Fabrica a preprocesats)

        End Enum

        'SGM 01/06/2012
        Public Enum FwUpdateAreas
            None = 0
            CPU = 1
            Peripherals = 2
            Maneuvers = 3
            Adjustments = 4
        End Enum

        ''' <summary>
        ''' Actions for FLIGHT
        ''' </summary>
        ''' <remarks>
        ''' IT 03/11/2014 - BA-2060: Instruction FLIGHT
        ''' </remarks>
        Public Enum Ax00FlightAction
            FillRotor = 1
            EmptyRotor = 2
            Perform = 3
            FillRotor_Perform = 4
            FillRotor_Perform_EmptyRotor = 5
        End Enum

#End Region

#Region "HelpFileType"

        Public Enum HELP_FILE_TYPE
            QUICK_GUIDE
            MANUAL
            WHATS_NEW
            MANUAL_SRV
            MANUAL_USR
        End Enum

#End Region

#Region "Reports"
        ''' <summary>
        ''' Define the Enumeration the graph type in QC Reports
        ''' </summary>
        ''' <remarks>
        ''' Created by: JB 16/07/2012 
        ''' </remarks>
        Public Enum REPORT_QC_GRAPH_TYPE
            NO_GRAPH
            LEVEY_JENNINGS_GRAPH
            YOUDEN_GRAPH
        End Enum
#End Region

#Region "LIS Project Enumerates"

        ''' <summary>
        ''' Available status from ES library
        ''' Values based on Systelab documentation
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum LISStatus
            unknown
            noconnectionDisabled
            noconnectionEnabled
            connectionDisabled
            connectionEnabled
            connectionRejected
            connectionAccepted
            released            ' XB 24/04/2013
        End Enum


        ''' <summary>
        ''' This enumerate is used for decode the xml notifications received from Embedded Synapse library
        ''' messages: ControlInformation and QueryResponse
        ''' 
        ''' The variable where the decodification is loaded is defined as Dictionary(Of GlobalEnumerates.LISNotificationSensors, List(Of String))
        ''' Example:
        ''' Private LISNotificationSensors As New Dictionary(Of GlobalEnumerates.LISNotificationSensors, List(Of String))
        ''' 
        ''' Private Sub ExampleCode()
        '''   'How to Save (add new dictionary key or update existing dictionary key) - DECODE METHODS
        '''    If LISNotificationSensors.ContainsKey(GlobalEnumerates.LISNotificationSensors.PENDINGMESSAGES) Then
        '''        Dim updateValue As New List(Of String)
        '''        updateValue = LISNotificationSensors(GlobalEnumerates.LISNotificationSensors.PENDINGMESSAGES)
        ''' 
        '''        'Update the current value and update the structure
        '''        updateValue.Add("Update values to existing key")
        '''        LISNotificationSensors(GlobalEnumerates.LISNotificationSensors.PENDINGMESSAGES) = saveValue
        ''' 
        '''    Else
        '''        Dim newSaveValue As New List(Of String)
        '''        newSaveValue.Add("Create key and assign values")
        '''        LISNotificationSensors.Add(GlobalEnumerates.LISNotificationSensors.PENDINGMESSAGES, newSaveValue)
        '''    End If
        '''
        ''' 
        '''    'How to Read - PRESENTATION METHODS
        '''    If LISNotificationSensors.ContainsKey(GlobalEnumerates.LISNotificationSensors.PENDINGMESSAGES) Then
        '''        'Read all values (list)
        '''        Dim readAllValues As New List(Of String)
        '''        readAllValues = LISNotificationSensors(GlobalEnumerates.LISNotificationSensors.PENDINGMESSAGES)
        ''' 
        '''        'Read single values
        '''        Dim readValue As String = String.Empty
        '''        For i As Integer = 0 To LISNotificationSensors(GlobalEnumerates.LISNotificationSensors.PENDINGMESSAGES).Count
        '''            readValue = LISNotificationSensors(GlobalEnumerates.LISNotificationSensors.PENDINGMESSAGES).Item(i)
        '''        Next
        '''    End If
        ''' End Sub
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum LISNotificationSensors

            '1) CONTROL INFORMATION MESSAGES: xmlHelper.QueryStringValue(xmlNotification, "@type") = "controlInformation"
            '============================================================================================================
            '1.1) STATUS: xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:facility") = "channel"
            STATUS 'List of String only with 1 item. 
            ' Values based on Systelab documentation: Value = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:item/udc:status")
            ' Enum LISStatus: noconnectionDisabled, noconnectionEnabled, connectionDisabled, connectionEnabled, connectionRejected, connectionAccepted


            '1.2) STORAGE: xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:facility") = "storage"
            STORAGE 'List of String only with 1 item
            ' Values based on Systelab documentation: Value = 0 or 75 or 80 or 85 or 90 or 95 or 100
            ' If "full" = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:item/udc:id") then
            '    Value = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:item/udc:value") '75 or 80 or 85 or 90 or 95 or 100
            ' If "normal" = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:item/udc:normal") then
            '    Value = 0
            ' If "overloaded" = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:item/udc:id") then
            '    Value = 100
            ' End If


            '1.3) OTHERS NOTIFICATIONS: xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:facility") = "message"
            DELIVERED 'List of String can contain several items
            ' Values based on Systelab documentation: Value = MessageID & SEPARATOR & "delivered"   (NOTE: as separator we can use the already used ADDITIONALINFO_ALARM_SEPARATOR)
            ' MessageID = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:item/udc:id") 
            ' Information status = "success" xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:status") 
            ' Message status = "delivered" = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:item/udc:status")

            UNDELIVERED 'List of String can contain several items
            ' Values based on Systelab documentation: Value = MessageID & SEPARATOR & "undeliverable"   (NOTE: as separator we can use the already used ADDITIONALINFO_ALARM_SEPARATOR)
            ' MessageID = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:item/udc:id") 
            ' Information status = "failure" xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:status") 
            ' Message status = "undeliverable" = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:item/udc:status")

            UNRESPONDED 'List of String can contain several items
            ' Values based on Systelab documentation: Value = MessageID & SEPARATOR & "unresponded"   (NOTE: as separator we can use the already used ADDITIONALINFO_ALARM_SEPARATOR)
            ' MessageID = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:item/udc:id") 
            ' Information status = "failure" xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:status") 
            ' Message status = "unresponded" = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:item/udc:status")

            PENDINGMESSAGES 'List of String can contain several items
            ' Values based on Systelab documentation: Value = MessageID & SEPARATOR & "delivering" or "undelivered"   (NOTE: as separator we can use the already used ADDITIONALINFO_ALARM_SEPARATOR)
            ' MessageID = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:item/udc:id") 
            ' Message status = ("delivering" or "undelivered") = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:item/udc:status")

            DELETED 'List of String can contain several items
            ' Values based on Systelab documentation: Value = MessageID & SEPARATOR & "deleted"   (NOTE: as separator we can use the already used ADDITIONALINFO_ALARM_SEPARATOR)
            ' MessageID = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:item/udc:id") 
            ' Message status = "deleted" = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:information/udc:item/udc:status")

            '2) QUERY RESPONSE MESSAGES: xmlHelper.QueryStringValue(xmlNotification, "@type") = "message" 
            '                            AND
            ' xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:header/udc:metadata/udc:container/udc:action") = "program"
            '==================================================================================================================
            INVALID 'Informs the xml generated by Sw was wrong. List of String only with 1 item
            ' Values based on Systelab documentation: Value = TriggerMessage & SEPARATOR & ErrorType (NOTE: as separator we can use the already used ADDITIONALINFO_ALARM_SEPARATOR)
            ' TriggerMessage = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:header/udc:metadata/udc:container/udc:triggermessage/udc:id") 
            ' ErrorType = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:error/udc:id") 'invalidMessage, invalidSyntax, invalidValue, missingMandatoryValue"
            ' Or Else
            ' ErrorType = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:body/udc:errortype") "

            QUERYALL 'List of String only with 1 item
            ' Values based on Systelab documentation: Value = TriggerMessage & SEPARATOR &  Description & SEPARATOR & "all" & SEPARATOR & ErrorType   (NOTE: as separator we can use the already used ADDITIONALINFO_ALARM_SEPARATOR)
            ' TriggerMessage = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:header/udc:metadata/udc:container/udc:triggermessage/udc:id")
            ' Description = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:body/udc:message/ci:service/ci:status") 'done, cannotBeDone
            ' SpecimenID = "all" = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:body/udc:message/ci:service/ci:specimen")
            ' ErroType (only when description is cannotBeDone) = = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:body/udc:message/ci:service/ci:error")
            ' (Note that when description is done ErrorType = "")

            HOSTQUERY 'List of String can contain several items
            ' Values based on Systelab documentation: Value = TriggerMessage & SEPARATOR & Description & SEPARATOR & SpecimenID & SEPARATOR & ErrorType   (NOTE: as separator we can use the already used ADDITIONALINFO_ALARM_SEPARATOR)
            ' TriggerMessage = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:header/udc:metadata/udc:container/udc:triggermessage/udc:id")
            ' Description = "" empty for hostquery response
            ' SpecimenID = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:body/udc:message/ci:service/ci:specimen")
            ' ErroType (only when description is cannotBeDone) = = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:body/udc:message/ci:service/ci:error")
            ' (Note that when description is done ErrorType = "")
            '
            'When the field ErrorType <> "" then the tube status is became to REJECTED!!	


        End Enum

        Public Enum LISActions
            ConnectWitlhLIS 'Try connection with LIS
            QueryAll_MDIButton 'Query All button in MDI screen (send QueryAll message to ES)
            HostQuery_MDIButton 'HostQuery button in MDI screen (open the auxiliary screen but do not send any message to ES)
            OrdersDownload_MDIButton 'Orders download button in MDI screen (process xml messages received)
            HostQuery 'HostQuery button in Host query monitor screen (send HostQuery message to ES)
            UploadResults 'Upload results to ES (send results messages to ES)
            LISUtilities_Menu 'Menu LIS utilities (access to screen)
            LISUtilities_DeleteLISSavedWS 'LIS utilities screen, option delete LIS work orders saved (send results cancelations to ES)
            LISUtilities_ClearQueue 'LIS utilities screen, option clear internal queue
            LISUtilities_TracesEnabling 'LIS utilities screen, option enable traces
        End Enum

        Public Enum LISautomateProcessSteps
            'Initial status
            notStarted
            initialStatus

            'SubProcesses
            subProcessReadBarcode
            subProcessAskBySpecimen
            subProcessDownloadOrders
            subProcessCreatinsWS 'Used for avoid repeated calls to CreateAutomaticWSWithLIS from ManageReceptionEvent
            subProcessCreateExecutions
            subProcessEnterRunning

            'Abnormal exits
            ExitNoLISConnectionButGoToRunning 'No connection with LIS but exists WS and user decides continue running
            ExitAutomaticProcessPaused 'User pauses the process. It will be deleted just before going running
            ExitHostQueryNotAvailableButGoToRunning 'No tube found but exists WS and user decides continue running
            ExitNoWorkOrders 'No LIS workorders responded. User still has not notified and is pending to him to decide what to do (continue or stop)
            ExitAutomaticProcesAndStop 'User decides stop the automatic process once it has been started (after barcode read, order downloading,...)
            ExitBarcodeErrorAndStop 'Barcode does not respond in the maximum time expected!! Process stops
        End Enum

#End Region

    End Class

End Namespace
