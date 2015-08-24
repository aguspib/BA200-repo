Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class tfmwPreloadedMasterDataDAO
          

#Region "CRUD Methods"

        ''' <summary>
        ''' Get the list of all active values of the specified Sub Table in the group of Preloaded Master Data Sub Tables 
        ''' </summary>
        ''' <param name="pSubTableID">Identifier of the Sub Table of the Preloaded Master Data from which the data will be obtained</param>
        ''' <returns>Dataset with structure of table tfmwPreloadedMasterData</returns>
        ''' <remarks>
        ''' Created by: DL 28/03/2011
        ''' </remarks>
        Public Function ReadLegendBySubTableID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                               ByVal pSubTableID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText &= "SELECT  MLR.ResourceText AS [Description], PMD.ResourceID AS [Group], PMD.FixedItemDesc AS FileName, PMD.Position" & vbCrLf
                        cmdText &= "FROM tfmwPreloadedMasterData PMD LEFT JOIN tfmwMultiLanguageResources MLR ON PMD.ITEMID = MLR.RESOURCEID" & vbCrLf
                        cmdText &= "WHERE PMD.SubTableID = '" & pSubTableID & "' AND MLR.LanguageID = '" & GlobalBase.GetSessionInfo.ApplicationLanguage & "' " & vbCrLf
                        cmdText &= "ORDER BY PMD.Position"

                        Dim myLegendDS As New LegendDS
                        'TR 19/09/2011 -Implement the using stament
                        Using dbCmd As New SqlClient.SqlCommand
                            dbCmd.Connection = dbConnection
                            dbCmd.CommandText = cmdText

                            'Fill the DataSet to return 
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myLegendDS.tfmwLegend)
                            End Using
                        End Using
                        resultData.SetDatos = myLegendDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "tfmwPreloadedMasterDataDAO.ReadLegendBySubTableID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all active values of the specified Sub Table in the group of Preloaded Master Data Sub Tables 
        ''' </summary>
        ''' <param name="pSubTableID">Identifier of the Sub Table of the Preloaded Master Data from which the data will be obtained</param>
        ''' <returns>Dataset with structure of table tfmwPreloadedMasterData</returns>
        ''' <remarks>
        ''' Modified by: DL 25/05/2010
        '''              SA 14/09/2010 - Removed field LangDescription from the SQL due to it was 
        '''                              removed from the table
        '''              AG 08/10/2010 - Change query to adapt it for multilanguage
        ''' </remarks>
        Public Function ReadBySubTableID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                         ByVal pSubTableID As PreloadedMasterDataEnum) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            
            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText &= "SELECT PMD.SubTableID, PMD.ItemID, MR.ResourceText AS FixedItemDesc, PMD.Position, PMD.Status " & vbCrLf
                        cmdText &= "FROM   tfmwPreloadedMasterData PMD INNER JOIN tfmwMultiLanguageResources MR " & vbCrLf
                        cmdText &= "       ON PMD.ResourceID = MR.ResourceID " & vbCrLf
                        cmdText &= "WHERE  PMD.SubTableID = '" & pSubTableID.ToString() & "'" & vbCrLf
                        cmdText &= "  AND  PMD.MultiLanguageFlag = 1" & vbCrLf
                        cmdText &= "  AND  PMD.Status = 1 " & vbCrLf
                        cmdText &= "  AND  MR.LanguageID = '" & GlobalBase.GetSessionInfo.ApplicationLanguage & "' " & vbCrLf
                        cmdText &= "UNION " & vbCrLf
                        cmdText &= "SELECT SubTableID, ItemID, FixedItemDesc, Position, Status " & vbCrLf
                        cmdText &= " FROM  tfmwPreloadedMasterData " & vbCrLf
                        cmdText &= "WHERE  SubTableID = '" & pSubTableID.ToString() & "'" & vbCrLf
                        cmdText &= "  AND  MultiLanguageFlag = 0" & vbCrLf
                        cmdText &= "  AND  Status = 1 " & vbCrLf
                        cmdText &= "ORDER BY Position "

                        Dim myPreloadedMasterData As New PreloadedMasterDataDS
                        'TR 19/09/2011 -Implement the using stament
                        Using dbCmd As New SqlClient.SqlCommand
                            dbCmd.Connection = dbConnection
                            dbCmd.CommandText = cmdText
                            'Fill the DataSet to return 
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myPreloadedMasterData.tfmwPreloadedMasterData)
                            End Using
                        End Using
                        resultData.SetDatos = myPreloadedMasterData
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "tfmwPreloadedMasterDataDAO.ReadBySubTableID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get value of an specific Item of a Preloaded Master Data Sub Table
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSubTableID">Unique identifier of the Sub Table of the Preloaded Master Data from which the data will be obtained</param>
        ''' <param name="pItemID">Unique identifier of the specific Item which value will be obtained</param>
        ''' <returns>GlobalDataTO containing a typed DataSet PreloadedMasterDataDS with the data of the specified
        '''          subtable Item</returns>
        ''' <remarks>
        ''' Created by:  TR 25/02/2010 - As copy of the existing function (same name) but following the current template
        ''' Modified by: SA 14/09/2010 - Removed field LangDescription from the SQL due to it was 
        '''                              removed from the table
        '''              AG 08/10/2010 - Change query to adapt it for multilanguage
        ''' </remarks>
        Public Function ReadByItemID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSubTableID As PreloadedMasterDataEnum, _
                                     ByVal pItemID As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim cmdText = " SELECT PMD.SubTableID, PMD.ItemID, MR.ResourceText AS FixedItemDesc, PMD.Position, PMD.Status " & _
                                  " FROM   tfmwPreloadedMasterData PMD WITH (NOLOCK) INNER JOIN " & _
                                  "        tfmwMultiLanguageResources MR WITH (NOLOCK) ON PMD.ResourceID = MR.ResourceID " & _
                                  " WHERE  PMD.SubTableID = '" & pSubTableID.ToString & "' " & _
                                  " AND    PMD.ItemID = '" & pItemID & "' " & _
                                  " AND    PMD.MultiLanguageFlag = 1 " & _
                                  " AND    MR.LanguageID = '" & GetSessionInfo.ApplicationLanguage & "' " & _
                                  " UNION " & _
                                  " SELECT SubTableID, ItemID, FixedItemDesc, Position, Status " & _
                                  " FROM tfmwPreloadedMasterData WITH (NOLOCK) " & _
                                  " WHERE  SubTableID = '" & pSubTableID.ToString & "' " & _
                                  " AND    ItemID = '" & pItemID & "' " & _
                                  " AND    MultiLanguageFlag = 0"

                        Dim resultData As New PreloadedMasterDataDS
                        'TR 19/09/2011 -Implement the using stament
                        Using dbCmd As New SqlClient.SqlCommand
                            dbCmd.Connection = dbConnection
                            dbCmd.CommandText = cmdText

                            'Fill the DataSet to return 
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.tfmwPreloadedMasterData)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                CreateLogActivity(ex.Message, "tfmwPreloadedMasterDataDAO.ReadByItemID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get value of an specific Item of a Preloaded Master Data Sub Table (special query for ICON_PATHS), not INNER JOIN with multilanguageResources
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSubTableID">Unique identifier of the Sub Table of the Preloaded Master Data from which the data will be obtained</param>
        ''' <param name="pItemID">Unique identifier of the specific Item which value will be obtained</param>
        ''' <returns>GlobalDataTO containing a typed DataSet PreloadedMasterDataDS with the data of the specified
        '''          subtable Item</returns>
        ''' <remarks>
        ''' Created by:  AG 21/02/2014 - #1516 (based on ReadByItemID)
        ''' </remarks>
        Public Function ReadByItemIDWithOutLanguages(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSubTableID As PreloadedMasterDataEnum, _
                                     ByVal pItemID As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText = " SELECT SubTableID, ItemID, FixedItemDesc, Position, Status " & _
                                  " FROM tfmwPreloadedMasterData WITH (NOLOCK) " & _
                                  " WHERE  SubTableID = '" & pSubTableID.ToString & "' " & _
                                  " AND    ItemID = '" & pItemID & "' " & _
                                  " AND    MultiLanguageFlag = 0"

                        Dim resultData As New PreloadedMasterDataDS
                        'TR 19/09/2011 -Implement the using stament
                        Using dbCmd As New SqlClient.SqlCommand
                            dbCmd.Connection = dbConnection
                            dbCmd.CommandText = cmdText

                            'Fill the DataSet to return 
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.tfmwPreloadedMasterData)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                CreateLogActivity(ex.Message, "tfmwPreloadedMasterDataDAO.ReadByItemIDWithOutLanguages", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Get the list of all active values of the specified Sub Table in the group of Preloaded Master Data Sub Tables
        ''' (special query for ICON_PATHS), not INNER JOIN with multilanguageResources
        ''' </summary>
        ''' <param name="pSubTableID">Identifier of the Sub Table of the Preloaded Master Data from which the data will be obtained</param>
        ''' <returns>Dataset with structure of table tfmwPreloadedMasterData</returns>
        ''' <remarks>
        ''' Created by:  AG 21/02/2014 - #1516 (based on ReadBySubTableID)
        ''' </remarks>
        Public Function ReadBySubTableIDWithOutLanguages(ByVal pDBConnection As SqlClient.SqlConnection, _
                                         ByVal pSubTableID As PreloadedMasterDataEnum) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText &= "SELECT SubTableID, ItemID, FixedItemDesc, Position, Status " & vbCrLf
                        cmdText &= " FROM  tfmwPreloadedMasterData " & vbCrLf
                        cmdText &= "WHERE  SubTableID = '" & pSubTableID.ToString() & "'" & vbCrLf
                        cmdText &= "  AND  MultiLanguageFlag = 0" & vbCrLf
                        cmdText &= "  AND  Status = 1 " & vbCrLf
                        cmdText &= "ORDER BY Position "

                        Dim myPreloadedMasterData As New PreloadedMasterDataDS
                        'TR 19/09/2011 -Implement the using stament
                        Using dbCmd As New SqlClient.SqlCommand
                            dbCmd.Connection = dbConnection
                            dbCmd.CommandText = cmdText
                            'Fill the DataSet to return 
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myPreloadedMasterData.tfmwPreloadedMasterData)
                            End Using
                        End Using
                        resultData.SetDatos = myPreloadedMasterData
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                CreateLogActivity(ex.Message, "tfmwPreloadedMasterDataDAO.ReadBySubTableIDWithOutLanguages", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "TO REVIEW"
        ''' <summary>
        ''' Get value of a specific Item in the indicated Preloaded Master Data Sub Table
        ''' </summary>
        ''' <param name="pSubTableID">Identifier of the Sub Table of the Preloaded Master Data from which the data will be obtained</param>
        ''' <param name="pItemID">Identifier of the specific Item which value will be obtained</param>
        ''' <returns>Dataset with structure of table tfmwPreloadedMasterData</returns>
        ''' <remarks>
        '''              AG 08/10/2010 - adapt to multilanguage
        ''' </remarks>
        Public Function ReadByItemID(ByVal pSubTableID As String, _
                                     ByVal pItemID As String) As PreloadedMasterDataDS
            Dim cmdText As String = ""
            Dim resultData As New PreloadedMasterDataDS
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                'Open the DB Connection
                dbConnection.ConnectionString = GetConnectionString()
                dbConnection.Open()
                'SQL Sentence to get data 
                cmdText = " SELECT PMD.SubTableID, PMD.ItemID, MR.ResourceText AS FixedItemDesc, PMD.Position, PMD.Status " & _
                          " FROM   tfmwPreloadedMasterData PMD INNER JOIN tfmwMultiLanguageResources MR ON PMD.ResourceID = MR.ResourceID " & _
                          " WHERE  PMD.SubTableID = '" & pSubTableID.ToString & "' " & _
                          " AND    PMD.ItemID = '" & pItemID & "' " & _
                          " AND    PMD.MultiLanguageFlag = 1 " & _
                          " AND    MR.LanguageID = '" & GetSessionInfo.ApplicationLanguage & "' " & _
                          " UNION SELECT SubTableID, ItemID, FixedItemDesc, Position, Status " & _
                          " FROM tfmwPreloadedMasterData" & _
                          " WHERE  SubTableID = '" & pSubTableID.ToString & "' " & _
                          " AND    ItemID = '" & pItemID & "' " & _
                          " AND    MultiLanguageFlag = 0"

                Dim dbCmd As New SqlClient.SqlCommand
                dbCmd.Connection = dbConnection
                dbCmd.CommandText = cmdText

                'Fill the DataSet to return 
                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                dbDataAdapter.Fill(resultData.tfmwPreloadedMasterData)

            Catch ex As Exception
                CreateLogActivity(ex.Message, "tfmwPreloadedMasterDataDAO.ReadByItemID", EventLogEntryType.Error, False)
                Throw ex

            Finally
                'Close the DB Connection
                dbConnection.Close()
            End Try

            Return resultData
        End Function
#End Region
    End Class
End Namespace

