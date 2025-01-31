Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL

Namespace Biosystems.Ax00.BL
    Public Class MasterDataDelegate

#Region "Public Methods"

        ''' <summary>
        ''' Get all values of the specified Sub Table in MasterData table
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSubTableID">Unique identifier of the Sub Table of the Master Data from which the data will be obtained</param>
        ''' <returns>GlobalDataTO containing a typed DataSet MasterDataDS with all items in the specified SubTable</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 25/02/2010 - Return error Master Data Missing when the informed SubTable does not exist
        '''              SA 26/01/2012 - Changed the function template
        ''' </remarks>
        Public Function GetList(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSubTableID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim masterDataDAO As New tcfgMasterDataDAO

                        resultData = masterDataDAO.ReadBySubTableID(dbConnection, pSubTableID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim masterDataItemDS As MasterDataDS = DirectCast(resultData.SetDatos, MasterDataDS)

                            If (masterDataItemDS.tcfgMasterData.Rows.Count = 0) Then
                                resultData.HasError = True
                                resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MasterDataDelegate.GetList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        Public Function UpdateLISValueByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSubtableID As String, _
                                                                        pItemID As String, pLISValue As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Update the User data...
                        Dim myMasterDataDAO As New tcfgMasterDataDAO
                        resultData = myMasterDataDAO.UpdateLISValueBySubTableIDAndItemID(dbConnection, pSubtableID, pItemID, pLISValue)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MasterDataDelegate.UpdateLISValueByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get a sample type list separated by commas
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 27/05/2013
        ''' MODIFIED BY: MI Made thread safe.
        ''' </remarks>
        Public Shared Function GetSampleTypes(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    Dim myMasteDataDS As MasterDataDS
                    Dim myMasterDataDelegate As New MasterDataDelegate()
                    'Dim qSampleType As New List(Of MasterDataDS.tcfgMasterDataRow)

                    resultData = myMasterDataDelegate.GetList(Nothing, "SAMPLE_TYPES")

                    If Not resultData.HasError Then
                        myMasteDataDS = CType(resultData.SetDatos, MasterDataDS)

                        <ThreadStatic()> Static sampleTypeBuilder As New Text.StringBuilder(1024)
                        sampleTypeBuilder.Clear()
                        Dim first As Boolean = True
                        For Each masterDataRow As MasterDataDS.tcfgMasterDataRow In myMasteDataDS.tcfgMasterData.Rows
                            If first Then
                                first = False
                            Else
                                SampleTypeBuilder.Append(","c)
                            End If
                            SampleTypeBuilder.Append(masterDataRow.ItemID)
                        Next
                        resultData.SetDatos = SampleTypeBuilder.ToString
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

        End Function


        Public Shared Function GetSampleTypesDataTable(ByVal pDBConnection As SqlClient.SqlConnection) As TypedGlobalDataTo(Of MasterDataDS.tcfgMasterDataDataTable)
            Dim resultData As New TypedGlobalDataTo(Of MasterDataDS.tcfgMasterDataDataTable)
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                Dim connection = DAOBase.GetSafeOpenDBConnection(pDBConnection)
                If (Not connection.HasError AndAlso Not connection.SetDatos Is Nothing) Then
                    dbConnection = connection.SetDatos

                    Dim myMasterDataDelegate As New MasterDataDelegate()

                    Dim untypedData = myMasterDataDelegate.GetList(Nothing, "SAMPLE_TYPES")

                    If Not untypedData.HasError Then
                        Dim intermediate = CType(untypedData.SetDatos, MasterDataDS)
                        resultData.SetDatos = intermediate.tcfgMasterData
                    Else
                        Throw New Exception("Data was not found")
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function



#End Region
    End Class
End Namespace
