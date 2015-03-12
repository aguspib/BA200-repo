Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.Core.Entities
    Module ConstantParameters
        Public WAITING_TIME_OFF As Integer = -1 'Wacthdoc off
        Public SYSTEM_TIME_OFFSET As Integer = 20 'Additional time (courtesy)    XB 04/06/2014 - BT #1656
        Public WAITING_TIME_DEFAULT As Integer = 12 'SECONDS Default time before ask again (if Ax00 is not ready and do not tell us any time estimation)
        Public WAITING_TIME_FAST As Integer = 1 'SECONDS Default timeout considered
        Public WAITING_TIME_ISE_FAST As Integer = 5 ' SECONDS waiting time for ISE module timeout (E:61) - BA-1872
        Public WAITING_TIME_ISE_OFFSET As Integer = 60 ' SECONDS waiting time for ISE repetitions - BA-1872
        Public MULTIPLE_ERROR_CODE As Integer = 99 'Default value (real value will be read in the Init method)
        Public ALIGHT_INIT_FAILURES As Integer = 2 'Default initial value for MAX ALIGHT failures without warning (real value will be read in the Init method)
        Public MAX_REACTROTOR_WELLS As Integer = 120 'Max wells inside the reactions rotor
        
        Public alarmsDefintionTableDS As New AlarmsDS 'Read the Alarm definition when analyzer manager class is created not in several methods as now (SoundActivationByAlarm, TranslateErrorCodeToAlarmID, RemoveErrorCodeAlarms, ExistFreezeAlarms

        Public LockThis As New Object() 'AG 28/06/2012
    End Module
End Namespace