Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global.GlobalEnumerates


Namespace Biosystems.Ax00.BL.Framework


    'Class to handler all the Application Session Funtionality
    Public Class ApplicationSessionManager
        Inherits GlobalBase

#Region "Public Methods"
        ''' <summary>
        ''' Verify if the application session exists
        ''' </summary>
        ''' <returns>True if exists; otherwise, it returns false</returns>
        ''' <remarks></remarks>
        Public Function SessionExist() As Boolean
            Dim result As Boolean = False
            Try
                result = (Not AppDomain.CurrentDomain.GetData("ApplicationInfoSession") Is Nothing)
            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationSessionManager.SessionExist", EventLogEntryType.Error, False)
            End Try
            Return result
        End Function

        ''' <summary>
        ''' Initialize the Session Object with tha application information
        ''' </summary>
        ''' <remarks>
        ''' Modified by: SA 06/10/2010 - Added parameter for the Current Application Language
        ''' </remarks>
        Public Function InitializeSession(ByVal pUserName As String, ByVal pUserLevel As String, ByVal pIconsPath As String, _
                                          ByVal pLanguageID As String) As Boolean
            Dim result As Boolean = False

            Try
                'Validate if session exist int the current Application Domain
                If (Not SessionExist()) Then
                    'Add the new object with the application global information in the application domain
                    AppDomain.CurrentDomain.SetData("ApplicationInfoSession", FillApplicationInfoSessionTO(pUserName, pUserLevel, pIconsPath, pLanguageID))
                    result = True 'change the result value to true because operation was OK.
                End If
            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationSessionManager.InitializeSession", EventLogEntryType.Error, False)
            End Try
            Return result
        End Function

        ''' <summary>
        ''' Fill an ApplicationInfoSessionTO with the required information 
        ''' </summary>
        ''' <returns>ApplicationInfoSessionTO with information</returns>
        ''' <remarks>
        ''' Modified by: SA 06/10/2010 - Added parameter for the Current Application Language
        '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
        ''' </remarks>
        Private Function FillApplicationInfoSessionTO(ByVal pUserName As String, ByVal pUserLevel As String, ByVal pIconsPath As String, _
                                                      ByVal pLanguageID As String) As ApplicationInfoSessionTO
            Dim myApplicationInfoSession As New ApplicationInfoSessionTO()
            Dim myGlobalDataTO As GlobalDataTO
            Try
                myApplicationInfoSession.ApplicationName = "BA400"
                myApplicationInfoSession.ApplicationLanguage = pLanguageID
                myApplicationInfoSession.UserName = pUserName   '.ToUpper()
                myApplicationInfoSession.UserLevel = pUserLevel
                myApplicationInfoSession.ApplicationIconPath = pIconsPath
                'myApplicationInfoSession.ActivateSystemLog = CType(ConfigurationManager.AppSettings("WriteToSystemLog"), Boolean)
                'TR 25/01/2011 -Replace by the corresponding value on the GlobalBase
                myApplicationInfoSession.ActivateSystemLog = GlobalBase.WriteToSystemLog

                'Application Setting area
                'Dim myApplicationSetting As New ApplicationSettingDelegate()
                'myApplicationInfoSession.ApplicationVersion = myApplicationSetting.GetApplicationSettingCurrentValueBySettingID("ApplicationVersion")
                'myApplicationInfoSession.DatabaseVersion = myApplicationSetting.GetApplicationSettingCurrentValueBySettingID("DatabaseVersion")

                'TR 29/03/2012 Get the max tets allow for supervisor.
                If pUserLevel = "SUPERVISOR" Then
                    'TR 29/03/2012 Get user Max Test Number.
                    Dim myUserDataDAO As New tcfgUserDataDAO
                    myGlobalDataTO = myUserDataDAO.ReadUserIDPassword(Nothing, pUserName)

                    If Not myGlobalDataTO.HasError Then
                        Dim myUserDataDS As New UserDataDS
                        myUserDataDS = DirectCast(myGlobalDataTO.SetDatos, UserDataDS)
                        If myUserDataDS.tcfgUserData.Count > 0 Then
                            If Not myUserDataDS.tcfgUserData(0).IsMaxTestsNumNull Then
                                myApplicationInfoSession.MaxTestsNumber = myUserDataDS.tcfgUserData(0).MaxTestsNum
                            End If
                        End If
                    End If
                End If
                'TR 29/03/2012 -END.

                'IT 18/09/2014 #BA-1946 - INI
                If (pUserLevel <> String.Empty) Then
                    Dim usersLevel As New UsersLevelDelegate
                    myGlobalDataTO = usersLevel.GetUserNumericLevel(Nothing, pUserLevel)
                    If Not myGlobalDataTO.HasError Then
                        myApplicationInfoSession.UserLevelEnum = CType(CType(myGlobalDataTO.SetDatos, Integer), USER_LEVEL)
                    End If
                End If
                'IT 18/09/2014 #BA-1946 - FIN

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationSessionManager.FillApplicationInfoSessionTO", EventLogEntryType.Error, False)
            End Try
            Return myApplicationInfoSession
        End Function

        ''' <summary>
        ''' Reset the Session Object
        ''' </summary>
        ''' <remarks>
        ''' Created by: VR
        ''' </remarks>
        Public Function ResetSession() As Boolean
            Dim result As Boolean = False
            Try
                'Validate if session exist in the current Application Domain.
                If (SessionExist()) Then
                    AppDomain.CurrentDomain.SetData("ApplicationInfoSession", Nothing)
                    result = True
                End If
            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationSessionManager.InitializeSession", EventLogEntryType.Error, False)
            End Try
            Return result
        End Function

#End Region
    End Class

End Namespace
