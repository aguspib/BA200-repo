Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports System.Data.SqlClient

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tcfgBarCodeSampleTypesMappingDAO
        Inherits DAOBase

#Region "CRUD"
        ''' <summary>
        ''' Read all Barcode mappings defined for the available Sample Types
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BarCodeSampleTypesMappingDS with all Barcode mappings defined for 
        '''          the available Sample Types</returns>
        ''' <remarks>
        ''' Created by:  TR 30/08/2011
        ''' Modified by: XB 04/06/2013 - Update query to return also tparTestSamples without multilanguage translation
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Dim var As New GlobalBase

                        ' XB  04/06/2013
                        'Dim cmdText As String = " SELECT BSTM.SampleType, BSTM.ActiveSampleType, BSTM.ExternalSampleType, " & vbCrLf & _
                        '                              " (BSTM.SampleType + ' - ' + MR.ResourceText) as CodeDesc " & vbCrLf & _
                        '                        " FROM tcfgBarCodeSampleTypesMapping BSTM INNER JOIN tcfgMasterData MD ON BSTM.SampleType = MD.ItemID " & vbCrLf & _
                        '                                                                " INNER JOIN tfmwMultiLanguageResources MR ON MD.ResourceID = MR.ResourceID " & vbCrLf & _
                        '                        " WHERE MD.SubTableID = 'SAMPLE_TYPES' " & vbCrLf & _
                        '                        " AND   MD.Status = 1 " & vbCrLf & _
                        '                        " AND   MR.LanguageID = '" & var.GetSessionInfo.ApplicationLanguage & "' " & vbCrLf & _
                        '                        " ORDER BY MD.Position "
                        Dim cmdText As String
                        cmdText = " SELECT BSTM.SampleType, BSTM.ActiveSampleType, BSTM.ExternalSampleType, " & vbCrLf & _
                               " (BSTM.SampleType + ' - ' + MR.ResourceText) as CodeDesc, MD.Position " & vbCrLf & _
                         " FROM tcfgBarCodeSampleTypesMapping BSTM INNER JOIN tcfgMasterData MD ON BSTM.SampleType = MD.ItemID " & vbCrLf & _
                                                                 " INNER JOIN tfmwMultiLanguageResources MR ON MD.ResourceID = MR.ResourceID " & vbCrLf & _
                         " WHERE MD.SubTableID = 'SAMPLE_TYPES' " & vbCrLf & _
                         " AND   MD.Status = 1 " & vbCrLf & _
                         " AND   MR.LanguageID = '" & GlobalBase.GetSessionInfo.ApplicationLanguage & "' "

                        cmdText &= " UNION " & vbCrLf

                        cmdText &= " SELECT BSTM.SampleType, BSTM.ActiveSampleType, BSTM.ExternalSampleType, " & vbCrLf & _
                               " (BSTM.SampleType + ' - ' + MD.FixedItemDesc) as CodeDesc, MD.Position " & vbCrLf & _
                         " FROM tcfgBarCodeSampleTypesMapping BSTM INNER JOIN tcfgMasterData MD ON BSTM.SampleType = MD.ItemID " & vbCrLf & _
                         " WHERE MD.SubTableID = 'SAMPLE_TYPES' " & vbCrLf & _
                         " AND   MD.Status = 1 " & vbCrLf & _
                         " AND   MD.MultiLanguageFlag = 0 "

                        cmdText &= " ORDER BY MD.Position "
                        ' XB  04/06/2013

                        Dim myBarCodeSampleTypesMappingDS As New BarCodeSampleTypesMappingDS
                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myBarCodeSampleTypesMappingDS.tcfgBarCodeSampleTypesMapping)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myBarCodeSampleTypesMappingDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgBarCodeSampleTypesMappingDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the the internal Sample Type code mapped to the specified External SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExternalSampleType">External Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BarCodeSampleTypesMappingDS containing the internal Sample Type code  
        '''          mapped to the specified External SampleType</returns>
        ''' <remarks>
        ''' Created by:  AG 30/08/2011
        ''' Modified by: SA 18/04/2012 - Added preffix N in the filter by ExternalSampleType due to it can contain non-ASCII characters
        ''' </remarks>
        Public Function ReadByExternalSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExternalSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT BSTM.SampleType, BSTM.ActiveSampleType, BSTM.ExternalSampleType " & vbCrLf & _
                                                " FROM   tcfgBarCodeSampleTypesMapping BSTM  " & vbCrLf & _
                                                " WHERE  ActiveSampleType   = 1 " & vbCrLf & _
                                                " AND    ExternalSampleType = N'" & pExternalSampleType.Trim & "' " & vbCrLf

                        Dim myBarCodeSampleTypesMappingDS As New BarCodeSampleTypesMappingDS
                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myBarCodeSampleTypesMappingDS.tcfgBarCodeSampleTypesMapping)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myBarCodeSampleTypesMappingDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgBarCodeSampleTypesMappingDAO.ReadByExternalSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update values of mapped SampleTypes in Barcode Configuration 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBarCodeSampleTypesMappingDS">Typed DataSet BarCodeSampleTypesMappingDS containing all Sample Types
        '''                                            mapping to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 31/08/2011
        ''' Modified by: SA 18/04/2012 - Changed the function template for the one used for Insert/Update/Delete in DAO Classes
        '''                              Added preffix N in the filter by ExternalSampleType due to it can contain non-ASCII charactersdd
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pBarCodeSampleTypesMappingDS As BarCodeSampleTypesMappingDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                Else
                    Dim cmdText As String = String.Empty
                    Dim affectedRecords As Integer = 0

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        Using dbDataAdapter As New SqlDataAdapter(dbCmd)
                            For Each sampleTypeMappingRow As BarCodeSampleTypesMappingDS.tcfgBarCodeSampleTypesMappingRow _
                                                          In pBarCodeSampleTypesMappingDS.tcfgBarCodeSampleTypesMapping.Rows
                                cmdText = " UPDATE tcfgBarCodeSampleTypesMapping " & vbCrLf & _
                                          " SET    ExternalSampleType = N'" & sampleTypeMappingRow.ExternalSampleType & "' " & vbCrLf & _
                                          " WHERE  SampleType         =  '" & sampleTypeMappingRow.SampleType & "' " & vbCrLf

                                dbCmd.CommandText = cmdText
                                affectedRecords += dbCmd.ExecuteNonQuery()
                            Next
                        End Using
                    End Using

                    resultData.SetDatos = affectedRecords
                    resultData.HasError = False
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgBarCodeSampleTypesMappingDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace
