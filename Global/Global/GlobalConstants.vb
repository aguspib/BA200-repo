Option Explicit On
Option Strict On

Namespace Biosystems.Ax00.Global
    Public Class GlobalConstants

        '**********************
        '**** GLOBAL FLAGs ****
        '**********************

        '*** PROVISIONAL: BT #1545 ==> ONLY WHILE DEVELOPMENT OF NEW ADD WS PROCESS (USING SEVERAL DB TRANSACTIONS) ***'
        Public Shared NEWAddWorkSession As Boolean = False
        '*** END PROVISIONAL

        '*** PROVISIONAL: ONLY WHILE DEVELOPMENT OF HISTORIC MODULE IS IN PROCESS ***'
        Public Shared HISTWorkingMode As Boolean = True
        '*** END PROVISIONAL

        Public Shared AnalyzerIsRunningFlag As Boolean = False 'AG + DL 24/01/2012 - we must define this public variable due the Global project must know when the Analyzer is in Running
        '                                                to perform a different business that in other status
        '                                                The analyzer status information is in Communications project that includes Global, so Global can NOT CALL methods or properties in Communications project

        Public Shared MaxFileLogSize As Long = -1       'DL 27/01/2012

        Public Shared CreateWSExecutionsWithMultipleTransactions As Boolean = False 'AG 24/03/2014 - #1545 - Create executions using multiple transactions
        Public Shared CreateWSExecutionsWithSemaphore As Boolean = True 'AG 03/06/2014 - #1644

        'TR 25/01/2011 -Replace by corresponding value on global base.
        Public Shared REAL_DEVELOPMENT_MODE As Integer = GlobalBase.RealDevelopmentMode

        '**************************
        '**** GLOBAL CONSTANTs ****
        '**************************
        'AG 21/04/2010 - There arent preparation pending to sent the analyzer
        Public Const NO_PENDING_PREPARATION_FOUND As Integer = -1
        Public Const ADDITIONALINFO_ALARM_SEPARATOR As String = "|" 'Field twksWSAnalyzerAlarms.AdditionalInfo has several fields informed separated using this character

        'AG 03/06/2010 - Value for Saturated and for error readings
        Public Const SATURATED_READING As Integer = 1048575
        Public Const READING_ERROR As Integer = 0

        Public Const MAX_WAVELENGTHS As Integer = 11

        'AG 02/08/2010
        'Constants used during calculations
        Public Const CALCULATION_ERROR_VALUE As Single = -1

        'Constant used showing results in screen
        Public Const FACTOR_NOT_CALCULATED As String = "Out"
        Public Const CONCENTRATION_NOT_CALCULATED As String = "Out" 'Conc not calculated due no calibration factor or outside limits
        Public Const CONCENTRATION_HIGH As String = "Out High" 'Conc outside extrapolation curve limits (HIGH)
        Public Const CONC_DUE_ABS_ERROR As String = "--" 'Conc not calculated due absorbance error (For instance substrate depletion)
        Public Const ABSORBANCE_ERROR As String = "--"
        Public Const EXPORTED_RESULT As String = "Exp"
        Public Const ABSORBANCE_INVALID_VALUE As String = "Error" 'AG 15/10/2012 - Used when the abs can not be calculated (abs(t) graph in table, results excel document)
        Public Const CONC_ISE_ERROR As String = "--"    ' XB 16/01/2015 - BA-1064

        'Temporal constant
        Public Const TO_DO As String = "Pending"
        'END AG 02/08/2010

        'SA 13/09/2010 - Added constants to be used in the process of Import Samples from an external LIMS system
        'AG 21/09/2010 - Added constant for the name of the EXPORT TO LIMS file: "ONLINE (YY-MM-DD HH-MM)_1.txt"
        'SA 16/06/2011 - Allowed Sample Classes restricted to Normal and Stat Patients
        'SA 04/08/2011 - Maximum length for field ExternalPID changed from 30 to 28; prefix for PatientID assigned to Patients received 
        '                from LIMS changed from "LIMS-" to "E-"
        'SA 18/05/2012 - Allowed Sample Classes include also Controls. Defined new constant for the mandatory number of fields in every
        '                row of the import from LIMS file
        Public Const LIMS_PATIENTID_PREFIX As String = "E-"
        Public Const LIMS_IMPORT_FILE_NAME As String = "Import.txt"
        Public Const LIMS_FILE_ALLOWED_SAMPLE_CLASSES As String = "N|U|Q" '|C|B"
        Public Const LIMS_FILE_MAXLEN_EXTERNAL_PATIENTID As Integer = 28
        Public Const LIMS_NUM_FIELDS_BY_LINE As Integer = 7
        'Public Const LIMS_EXPORT_FILE_NAME As String = "ONLINE" 'Not used!! Moved to tfmwSwParameters in v2.0.0
        'Public Const RESET_EXPORT_FILENAME As String = "EXP" 'Not used!! Moved to tfmwSwParameters in v2.0.0

        'AG 30/11/2010
        Public Const ABSORBANCE_FORMAT As String = "0.0000"
        Public Const CALIBRATOR_FACTOR_FORMAT As String = "#####0.####" 'AG 22/10/2012 show decimals only when significant (limit 4) "####0.0000"

        'RH 23/12/2010
        Public Const SAT_DATETIME_FORMAT As String = " dd-MM-yyyy HH-mm"

        ' DL 29/03/2011
        Public Const LEGEND_PREPARATION As String = "LEGEND_PREPARATION"
        Public Const LEGEND_ROTOR As String = "LEGEND_ROTOR"

        'Public Shared LegendSource As GlobalEnumerates.LEGEND_SOURCE
        ' END DL 29/03/2011

        'SGM 31/03/11
        Public Const SENSORS_MINIMUM_SAMPLE_RATE As Integer = 500
        Public Const CONTROL_TARGET_FORMAT As String = "###0.##00"

        'SA 25/10/2012 - Constants for status after validation of applied ReferenceRange
        Public Const NOT_PANIC_REMARK As String = "*"   'When there is not a defined Panic Range and CONC < MinRange or CONC > MaxRange or whatever other alarm was raised
        Public Const PANIC_LOW As String = "PL"     'When there is a defined Panic Range and CONC < MinPanicRange
        Public Const LOW As String = "L"            'When there is a defined Panic Range and MinPanicRange < CONC < MinRange
        Public Const HIGH As String = "H"           'When there is a defined Panic Range and MaxRange < CONC < MaxPanicRange
        Public Const PANIC_HIGH As String = "PH"    'When there is a defined Panic Range and MaxPanicRange < CONC

        'SGM 19/03/2013 - LIS Xml Schema Url's
        Public Const UDCSchema As String = "http://www.nte.es/schema/udc-interface-v1.0"
        Public Const ClinicalInfoSchema As String = "http://www.nte.es/schema/clinical-information-v1.0"
        Public Const TraceSchema As String = "http://www.nte.es/schema/trace-v1.0"
        Public Const RawHL7Schema As String = "http://www.nte.es/schema/raw-hl7-v1.0" 'XML schema for catching hl7 errors

        'SGM 29/04/2013 - SpecimenID seperator for positioning treeview
        Public Const SPECIMENID_SEPARATOR As String = vbCrLf 'Field twksWSRequiredElements.SpecimenIDList has several fields informed separated using this character

        'AG 08/07/2013 - Special mode for work with LIS with automatic actions
        'In final code this constant will be a user setting
        Public Const AUTO_WS_WITH_LIS_MODE As Boolean = True '(User Settings)
        Public Const AUTO_LIS_WAIT_TIME As Single = 20 'seconds before process all received LIS workorders (User Settings)
        Public Const AUTO_LIS_MAX_ITERA As Integer = 3 'Max iterations when after order download process new LIS orders has been received (SwParameters)

        'AG + XB 24/02/20174 - BT #1520
        Public Const MAX_APP_MEMORY_USAGE As String = "For best performance, it's required to close and start the application before continuing."
        Public Const UNHANDLED_EXCEPTION_ERR As String = "The application has encountered an unexpected error. " & vbNewLine & "Please, try again or contact the technical service."
        Public Const CATCH_APP_MEMORY_ERR As String = "Some images hasn't been properly loaded. For best visualization, change screen and return to the present one."
        Public Const HResult_OutOfMemoryException As String = "-2147024882"
        Public Const HResult_InsufficientMemoryException As String = "-2146233027"
        Public Const HResult_TimeoutException As String = "-2146233083"
        Public Const CloseCannotBeCalledException As String = "-2146233079" ' XB 17/03/20174 - #1544
        Public Const SQLDeadLockException As String = "-2146232060" ' XB 20/03/2014 - #1548
        Public Const ObjRefException As String = "-2147467261" ' XB 26/03/2014 - #1548
        Public Const NullReferenceException As String = "-2146233069" ' XB 26/03/2014 - #1548


        'AG 02/06/2014 - #1644 constants for global createWSexecutions semaphore
        Public Const SEMAPHORE_TOUT_CREATE_EXECUTIONS As Integer = 12000 '12 sec
        Public Const SEMAPHORE_BUSY As Integer = 0

    End Class
End Namespace
