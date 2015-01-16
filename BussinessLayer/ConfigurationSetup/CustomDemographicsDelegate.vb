Option Strict On
Option Explicit On


Namespace Biosystems.Ax00.BL

    Public Class CustomDemographicsDelegate

#Region "Methods"

        '''' <summary>
        '''' Get the list of selected Custom Demographics for an speciifc demographic type.
        '''' </summary>
        '''' <param name="pDemogType">Unique identifier of the Demographic Type</param>
        '''' <returns>Dataset with structure of table tcfgCustomDemographicsLANG</returns>
        '''' <remarks></remarks>
        'Public Function GetActiveDemographics(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pDemogType As String) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        'resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        'If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '        '    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '        '    If (Not dbConnection Is Nothing) Then
        '        '        Dim tcfgCustomDemographics As New tcfgCustomDemographicsLANGDAO
        '        '        resultData = tcfgCustomDemographics.ReadDemographicsByStatus(dbConnection, pDemogType, "'False'")
        '        '    End If
        '        'End If

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = "SYSTEM_ERROR"
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ControlsDelegate.GetControlData", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData

        'End Function

        '''' <summary>
        '''' Get the list of selected Custom Demographics for an speciifc demographic type.
        '''' </summary>
        '''' <param name="pDemographicID">Unique identifier of the Demographic Type</param>
        '''' <returns>Dataset with structure of table tfmwPreloadedMasterDataLANG or tcfgMasterData</returns>
        '''' <remarks></remarks>
        'Public Function GetDemographicDetails(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pDemographicID As String) As DataSet


        '    Dim resultDS As New DataSet
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDbConnection)
        '        If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myCustomDemographicsDAO As New tcfgCustomDemographicsDAO
        '                resultData = myCustomDemographicsDAO.ReadByDemographicID(dbConnection, pDemographicID)

        '                resultDS = CType(resultData.SetDatos, DataSet)

        '            End If
        '        End If

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = "SYSTEM_ERROR"
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ScreenBlockDelegate.GetBlocksByScreen", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try

        '    Return resultDS

        'End Function
        ''Public Function GetDemographicDetails(ByVal pDemographicID As String) As DataSet
        ''    Dim resultDS As New DataSet

        ''    Try
        ''        Dim resultData As New tcfgCustomDemographicsDAO
        ''        resultDS = resultData.ReadByDemographicID(pDemographicID)

        ''    Catch ex As Exception
        ''        Throw ex
        ''    End Try

        ''    Return resultDS

        ''End Function

        '''' <summary>
        '''' Get the middle name status
        '''' </summary>
        '''' <returns>Boolean value returned stating the status of the middle name field.</returns>
        '''' <remarks></remarks>
        'Public Function GetMiddleNameStatus(ByVal pDBConnection As SqlClient.SqlConnection) As Boolean
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim tcfgCustomDemographicData As New tcfgCustomDemographicsDAO
        '                resultData = tcfgCustomDemographicData.MiddleNameStatus(dbConnection)

        '                Dim resultDS As New CustomDemographicsDS
        '                resultDS = CType(resultData.SetDatos, CustomDemographicsDS)

        '                If resultDS.tcfgCustomDemographics.Rows.Count > 0 Then
        '                    GetMiddleNameStatus = CBool(resultDS.tcfgCustomDemographics.Rows(0).Item("Status"))
        '                End If
        '            End If
        '        End If

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = "SYSTEM_ERROR"
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "CalibratorsDelegate.GetCalibratorData", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try

        '    Return GetMiddleNameStatus

        'End Function
#End Region
    End Class
End Namespace

