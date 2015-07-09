Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.Core.Entities
    Public interface IAnalyzerAlarms
        ''' <summary>
        ''' If needed perform the proper business depending the alarm code and status
        ''' Note that some alarms has already perfomed previous business (alarms in instructions different than AlarmsDetails,
        ''' for instance ANSBR1, ANSBR2, ANSBM1, STATUS, ...)
        ''' Finally save alarms into Database and prepare DS for UI refresh
        ''' </summary>
        ''' <param name="pAlarmIdList">List of alarms to treat</param>
        ''' <param name="pAlarmStatusList" >For each alarm in pAlarmCode indicates his status: TRUE alarm exists, FALSE alarm solved</param>
        ''' <param name="pAdditionalInfoList">Additional info for some alams: volume missing, prep clot warnings, prep locked</param>
        ''' <returns>Global Data To indicating when an error has occurred </returns>
        ''' <remarks>
        ''' Created by:  AG 16/03/2011
        ''' Modified by: SA 29/06/2012 - Removed the OpenDBTransaction; all functions called by this method should open their own DBTransaction
        '''                              or DBConnection, depending if they update or read data - This is to avoid locks between the different
        '''                              threads in execution
        '''              AG 25/072012    pAdditionalInfoList
        '''              AG 30/11/2012 - Do not inform the attribute AnalyzerManager.EndRunInstructionSent()as TRUE when call the ManageAnalyzer with ENDRUN or when you add it to the queue
        '''                              This flag will be informed once the instruction has been really sent. Current code causes that sometimes the ENDRUN instruction is added to
        '''                              queue but how the flag is informed before send the instruction it wont never be sent!!
        '''              AG 22/05/2014 - #1637 Use exclusive lock (multithread protection)
        '''              AG 05/06/2014 - #1657 Protection (provisional solution)! (do not clear instructions queue when there are only 1 alarm and it is ISE_OFF_ERR)
        '''                            - PENDING FINAL SOLUTION: AlarmID ISE_OFF_ERR must be generated only 1 time when alarm appears, and only 1 time when alarm is fixed (now this alarm with status FALSE is generated with each ANSINF received)
        '''              XB 04/11/2014 - Add ISE_TIMEOUT_ERR alarm - BA-1872
        '''              XB 06/11/2014 - Add COMMS_TIMEOUT_ERR alarm - BA-1872
        ''' </remarks>
        ''' <code>
        ''' --------------------------------DELETED CODE------------------------------------------
        ''' If String.Equals(AnalyzerManager.AnalyzerFreezeMode(), "AUTO") Then
        ''' 
        ''' Case Alarms.R1_NO_VOLUME_WARN, Alarms.R2_NO_VOLUME_WARN, Alarms.S_NO_VOLUME_WARN
        ''' 'NOTE: Previous business is performed in method ProcessArmStatusRecived
        ''' 
        ''' Case Alarms.FRIDGE_STATUS_WARN, Alarms.FRIDGE_STATUS_ERR
        ''' FRIDGE: No business (only inform user)
        ''' ISE: Send ISE test preparations are not allowed and inform user
        ''' 
        ''' </code>
        Function Manage(ByVal pAlarmIdList As List(Of AlarmEnumerates.Alarms), _
                               ByVal pAlarmStatusList As List(Of Boolean), _
                               Optional ByVal pAdditionalInfoList As List(Of String) = Nothing) As GlobalDataTO

        Sub RemoveAlarmState(ByVal alarmName As String)
        Sub AddNewAlarmState(ByVal alarmName As String)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="activeAlarm"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function ExistsActiveAlarm(ByVal activeAlarm As String, Optional ByVal _pAnalyzerID As String = "") As Boolean

        ''' <summary>
        ''' Method, that create or delete the alarm and send a  refresh in UserInterface to see the alarm.
        ''' </summary>
        ''' <param name="typeAction"></param>
        ''' <param name="alarm"></param>
        ''' <remarks></remarks>
        Sub ActionAlarm(ByVal typeAction As Boolean, ByRef alarm As AlarmEnumerates.Alarms)
    end interface
End NameSpace