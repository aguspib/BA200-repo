Option Explicit On
Option Strict On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO
Imports System.Data.SqlClient

Namespace Biosystems.Ax00.BL.Framework

    ''' <summary>
    ''' Manages the Business Logic for ApplicationSetting Table.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ApplicationSettingDelegate

        ''' <summary>
        ''' Get an application setting information by the setting id
        ''' NOT USED ON V1.0.1
        ''' </summary>
        ''' <param name="SettingID">Setting ID To get</param>
        ''' <returns>An ApplicationSetting Dataset.</returns>
        ''' <remarks></remarks>
        Public Function GetApplicationSettingBySettingID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal SettingID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myApplicationSettingDAO As New ApplicationSettingDAO()
                        'fill the dataset with the result.
                        resultData = myApplicationSettingDAO.Read(dbConnection, SettingID)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ApplicationSettingDelegate.GetApplicationSettingBySettingID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

        End Function

        ''' <summary>
        ''' Get the CurrentValue of an application setting information by the setting id
        ''' NOT USED ON V1.0.1
        ''' </summary>
        ''' <param name="SettingID">Setting ID </param>
        ''' <returns>Return the currentvalue for the Setting ID</returns>
        ''' <remarks>NOT USED ON V 1.0.1</remarks>
        Public Function GetApplicationSettingCurrentValueBySettingID(ByVal SettingID As String) As String
            Dim myCurrentValue As String = ""
            Dim myApplicationSettingDS As New ApplicationSettingDS()
            Dim resultdt As GlobalDataTO

            Try
                resultdt = GetApplicationSettingBySettingID(Nothing, SettingID)
                myApplicationSettingDS = CType(resultdt.SetDatos, ApplicationSettingDS) 'GetApplicationSettingBySettingID(SettingID)
                If myApplicationSettingDS.ApplicationSetting.Rows.Count > 0 Then 'validate if there is any row
                    myCurrentValue = myApplicationSettingDS.ApplicationSetting.Rows(0)(myApplicationSettingDS.ApplicationSetting.CurrentValueColumn).ToString()
                End If
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ApplicationSettingDelegate.GetApplicationSettingCurrentValueBySettingID", EventLogEntryType.Error, False)
            End Try
            Return myCurrentValue
        End Function

        ''' <summary>
        ''' Update the currentValue for a existing SettingID in the table ApplicationSetting.
        ''' </summary>
        ''' <param name="SettingID">Setting Id to update.</param>
        ''' <param name="CurrentValue">New current Value</param>
        ''' <returns>integer value with the amount of records modify.</returns>
        ''' <remarks></remarks>
        Public Function UpdateCurrentValueBySettingID(ByVal SettingID As String, ByVal CurrentValue As String, ByVal Conn As SqlConnection) As Integer
            Dim result As Integer = 0
            Try
                Dim myApplicationSettingDS As New ApplicationSettingDS()
                Dim MyApplicationSettingDAO As New ApplicationSettingDAO()

                ' modified by : DL 03/02/2010
                'Load the record by the SettingID
                Dim resultdt As GlobalDataTO

                resultdt = MyApplicationSettingDAO.Read(Conn, SettingID)
                myApplicationSettingDS = CType(resultdt.SetDatos, ApplicationSettingDS) 'MyApplicationSettingDAO.Read(Nothing, SettingID)

                If myApplicationSettingDS.ApplicationSetting.Rows.Count > 0 Then ' validate if return any record
                    myApplicationSettingDS.ApplicationSetting.Rows(0).BeginEdit() 'prepate to update row.
                    myApplicationSettingDS.ApplicationSetting.Rows(0)("CurrentValue") = CurrentValue ' update the value 
                    myApplicationSettingDS.ApplicationSetting.Rows(0).EndEdit()
                End If
                ' Update the database value. and assigned the value to the result
                result = MyApplicationSettingDAO.Update(myApplicationSettingDS, Conn, Nothing)
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ApplicationSettingDelegate.UpdateCurrentValueBySettingID", EventLogEntryType.Error, False)
            End Try
            Return result
        End Function

    End Class

End Namespace

