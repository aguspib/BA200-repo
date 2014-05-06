Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.BL

    Public Class SpecialTestsSettingsDelegate

#Region "CRUD Methods"

        ''' <summary>
        ''' Get the Settings defined for the specified Special Test/Sample Type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pSettingName">Optional parameter; when informed, only values of the
        '''                          specified Setting is returned</param>
        ''' <returns>GlobalDataTO containing a typed DataSet SpecialTest</returns>
        ''' <remarks>
        ''' Created by:  DL 27/08/2010
        ''' Modified by: SA 30/08/2010 - Removed the error return when there are not Special Test Settings
        '''                              for the specified Test/SampleType; it is not an error, it is possible
        '''                              for instance, for Special Test HbA1C. Parameter pSettingID changed to optional
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                             ByVal pSampleType As String, Optional ByVal pSettingName As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mySpecialTestsSettings As New tfmwSpecialTestsSettingsDAO

                        resultData = mySpecialTestsSettings.Read(dbConnection, pTestID, pSampleType, pSettingName)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SpecialTestsSettingsDelegate.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "Other Methods"

        ''' <summary>
        ''' Read all executions in process whose test id are defined as special test because they are critical in pause mode
        ''' SpecialTestSetting - CRITICAL_PAUSEMODE
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <returns>GlobalDataTo (ExecutionsDS)</returns>
        ''' <remarks>AG 22/11/2013 - Creation - Task #1391</remarks>
        Public Function ExistsCriticalPauseTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tfmwSpecialTestsSettingsDAO
                        resultData = myDAO.ExistsCriticalPauseTests(dbConnection, GlobalEnumerates.SpecialTestsSettings.CRITICAL_PAUSEMODE.ToString, pAnalyzerID, pWorkSessionID)

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SpecialTestsSettingsDelegate.ExistsCriticalPauseTests", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

#End Region
    End Class

End Namespace