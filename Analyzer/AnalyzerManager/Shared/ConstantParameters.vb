
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.Core.Entities
    Public Module ConstantParameters
        Public WAITING_TIME_OFF As Integer = -1 'Wacthdoc off
        Public SYSTEM_TIME_OFFSET As Integer = 20 'Additional time (courtesy)    XB 04/06/2014 - BT #1656
        Public WAITING_TIME_DEFAULT As Integer = 12 'SECONDS Default time before ask again (if Ax00 is not ready and do not tell us any time estimation)
        Public WAITING_TIME_FAST As Integer = 1 'SECONDS Default timeout considered
        Public WAITING_TIME_ISE_FAST As Integer = 5 ' SECONDS waiting time for ISE module timeout (E:61) - BA-1872
        Public WAITING_TIME_ISE_OFFSET As Integer = 60 ' SECONDS waiting time for ISE repetitions - BA-1872
        Public MULTIPLE_ERROR_CODE As Integer = 99 'Default value (real value will be read in the Init method)
        Public ALIGHT_INIT_FAILURES As Integer = 2 'Default initial value for MAX ALIGHT failures without warning (real value will be read in the Init method)
        Public MAX_REACTROTOR_WELLS As Integer = 120 'Max wells inside the reactions rotor
        Public WELL_OFFSET_FOR_PREDILUTION As Integer = 4 'Default well offset until next request when a PTEST instruction is sent
        Public WELL_OFFSET_FOR_ISETEST_SERPLM As Integer = 2 'Default well offset until next request when a ISETEST (ser or plm) instruction is sent
        Public WELL_OFFSET_FOR_ISETEST_URI As Integer = 3 'Default well offset until next request when a ISETEST (uri) instruction is sent
        Public REAGENT_CONTAMINATION_PERSISTANCE As Integer = 2 'Default initial value for the contamination persistance (real value will be read in the Init method)

        Public alarmsDefintionTableDS As New AlarmsDS 'Read the Alarm definition when analyzer manager class is created not in several methods as now (SoundActivationByAlarm, TranslateErrorCodeToAlarmID, RemoveErrorCodeAlarms, ExistFreezeAlarms

        Public ReadOnly LockThis As New Object() 'AG 28/06/2012

    End Module

    Module ConstantStates
        Public InProcessState As String = "INPROCESS"
        Public IniState As String = "INI"
        Public EndState As String = "END"
        Public ClosedState As String = "CLOSED"
        Public VoidState As String = ""
    End Module
End Namespace