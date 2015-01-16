Option Strict On
Option Explicit On



Namespace Biosystems.Ax00.DAL.DAO

    Public Class tcfgCustomDemographicsDAO
        Inherits DAOBase

#Region "Other Methods"

        '''' <summary>
        '''' Get data of the specified Custom Demographic
        '''' </summary>
        '''' <param name="pDBConnection"></param> 
        '''' <param name="pDemographicID">Unique Identifier of the Demographic</param>
        '''' <returns>Dataset with structure of table tcfgCustomDemographics</returns>
        '''' <remarks>
        '''' Modified by: SA 14/09/2010 - Removed field LangDescription from the SQL due to it was 
        ''''                              removed from the table
        '''' </remarks>
        'Public Function ReadByDemographicID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pDemographicID As String) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim CustomDemoData As New CustomDemographicsDS
        '    Dim dbConnection As New SqlClient.SqlConnection


        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = ""

        '                'SQL Sentence to get data
        '                cmdText = " SELECT DemographicID, DemographicType, DemographicDataType, DemographicMaxLength, " & _
        '                                 " DemographicValueList, Status,TS_User,TS_DateTime " & _
        '                          " FROM   tcfgCustomDemographics " & _
        '                          " WHERE  DemographicID = '" & pDemographicID & "' "

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                'Fill the DataSet to return 
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                CustomDemoData.EnforceConstraints = False
        '                dbDataAdapter.Fill(CustomDemoData.tcfgCustomDemographics)

        '                If CustomDemoData.tcfgCustomDemographics.Rows.Count > 0 Then
        '                    If CustomDemoData.tcfgCustomDemographics.Rows(0).Item("DemographicDataType").ToString = "FIXED_LIST" Then
        '                        'Dim PreloadedMasterConfig As New PreloadedMasterDataDelegate
        '                        'returnData = New PreloadedMasterDataDS
        '                        'returnData = PreloadedMasterConfig.GetListNew(CustomDemoData.tcfgCustomDemographics.Rows(0).Item("DemographicValueList").ToString)
        '                    ElseIf CustomDemoData.tcfgCustomDemographics.Rows(0).Item("DemographicDataType").ToString = "VAR_LIST" Then
        '                        'Dim MasterConfig As New MasterData
        '                        'returnData = New MasterDataDS
        '                        'returnData = MasterConfig.GetList(CustomDemoData.tcfgCustomDemographics.Rows(0).Item("DemographicValueList").ToString)
        '                    End If
        '                End If

        '                resultData.SetDatos = CustomDemoData
        '                resultData.HasError = False
        '            End If
        '        End If

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = "SYSTEM_ERROR"
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tcfgCustomDemographicsDAO.ReadNyDemographicID", EventLogEntryType.Error, False)

        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try

        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get Active or Inactive Custom Demographics of an specific Type (Patient or Order)
        '''' </summary>
        '''' <param name="pDBConnection"></param> 
        '''' <param name="pDemographicType">Demographic Type: Patient (PAT) or Order (ORDER)</param>
        '''' <param name="pStatus">Demographic Status: Active or Inactive</param>
        '''' <returns>Dataset with structure of table tcfgCustomDemographics</returns>
        '''' <remarks></remarks>
        'Public Function ReadByTypeAndStatus(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                                    ByVal pDemographicType As String, _
        '                                    ByVal pStatus As Boolean) As GlobalDataTO

        '    Dim resultData As New GlobalDataTO
        '    Dim CustomDemographicsData As New CustomDemographicsDS
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = ""
        '                'SQL Sentence to get data
        '                cmdText = " SELECT DemographicID, DemographicDataType, FixedItemDesc, DemographicMaxLength, DemographicValueList " & _
        '                          "        Status, TS_User, TS_DateTime " & _
        '                          " FROM   tcfgCustomDemographics " & _
        '                          " WHERE  UPPER(DemographicType) = UPPER('" & pDemographicType & "') " & _
        '                          " AND    Status = " & pStatus

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                'Fill the DataSet to return 
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                dbDataAdapter.Fill(CustomDemographicsData.tcfgCustomDemographics)

        '                resultData.SetDatos = CustomDemographicsData
        '                resultData.HasError = False
        '            End If
        '        End If

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = "SYSTEM_ERROR"
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tcfgCustomDemographicsDAO.ReadByTypeAndStatus", EventLogEntryType.Error, False)

        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try

        '    Return resultData

        'End Function

        ''''' <summary>
        ''''' Get the list of selected Custom Demographics for an speciifc demographic type.
        ''''' </summary>
        ''''' <param name="pDemogTypeCode">Unique identifier of the Demographic Type</param>
        ''''' <param name="pStatus">Status of Dempgraphic Type.Active or Not </param>
        ''''' <returns>Dataset with structure of table tcfgCustomDemographics</returns>
        ''''' <remarks></remarks>
        ''Public Function ReadDemographicsByStatus(ByVal pDBConnection As SqlClient.SqlConnection, _
        ''                                         ByVal pDemogTypeCode As String, _
        ''                                         ByVal pStatus As Integer) As GlobalDataTO

        ''    Dim resultData As New GlobalDataTO
        ''    Dim CustomDemographicsData As New CustomDemographicsDS
        ''    Dim dbConnection As New SqlClient.SqlConnection

        ''    Try
        ''        resultData = GetOpenDBConnection(pDBConnection)
        ''        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        ''            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        ''            If (Not dbConnection Is Nothing) Then
        ''                Dim cmdText As String = ""
        ''                'SQL Sentence to get data
        ''                'SQL Sentence to get data
        ''                cmdText = " SELECT DemographicID, LanguageID, Description " & _
        ''                          " FROM   tcfgCustomDemographicsLANG " & _
        ''                          " WHERE  DemographicID IN (SELECT DemographicID " & _
        ''                                                   " FROM   tcfgCustomDemographics " & _
        ''                                                   " WHERE  DemographicType = '" & pDemogTypeCode & "' " & _
        ''                                                   " AND    Status = " & pStatus & ") " & _
        ''                          " AND    LanguageID = 'ENG'"

        ''                Dim dbCmd As New SqlClient.SqlCommand
        ''                dbCmd.Connection = dbConnection
        ''                dbCmd.CommandText = cmdText

        ''                'Fill the DataSet to return 
        ''                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        ''                dbDataAdapter.Fill(CustomDemographicsData.tcfgCustomDemographics)

        ''                resultData.SetDatos = CustomDemographicsData
        ''                resultData.HasError = False
        ''            End If
        ''        End If

        ''    Catch ex As Exception
        ''        resultData.HasError = True
        ''        resultData.ErrorCode = "SYSTEM_ERROR"
        ''        resultData.ErrorMessage = ex.Message

        ''        'Dim myLogAcciones As New ApplicationLogManager()
        ''        GlobalBase.CreateLogActivity(ex.Message, "tcfgCustomDemographicsDAO.ReadByTypeAndStatus", EventLogEntryType.Error, False)

        ''    Finally
        ''        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        ''    End Try

        ''    Return resultData

        ''End Function

        '''' <summary>
        '''' Get the middle name status
        '''' </summary>
        '''' <returns>Dataset with structure of table tcfgCustomDemographics</returns>
        '''' <remarks></remarks>
        'Public Function MiddleNameStatus(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim CustomDemoData As New CustomDemographicsDS
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = ""
        '                'SQL Sentence to get data
        '                'SQL Sentence to get data
        '                cmdText = " SELECT DemographicID, DemographicType, DemographicDataType, DemographicMaxLength, " & _
        '                                 " DemographicValueList, Status,TS_User,TS_DateTime " & _
        '                          " FROM   tcfgCustomDemographics " & _
        '                          " WHERE  FixedItemDesc LIKE '%MiddleName%' "

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                'Fill the DataSet to return 
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                CustomDemoData.EnforceConstraints = False
        '                dbDataAdapter.Fill(CustomDemoData.tcfgCustomDemographics)

        '                resultData.SetDatos = CustomDemoData
        '                resultData.HasError = False
        '            End If
        '        End If

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = "SYSTEM_ERROR"
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tcfgCustomDemographicsDAO.MiddleNameStatus", EventLogEntryType.Error, False)

        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try


        '    Return resultData


        'End Function

#End Region
    End Class
End Namespace

