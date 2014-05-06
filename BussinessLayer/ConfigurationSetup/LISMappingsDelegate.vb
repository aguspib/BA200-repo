Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports System.Data.SqlClient
Imports Biosystems.Ax00.Global.TO


Namespace Biosystems.Ax00.BL

    Public Class LISMappingsDelegate


        ''' <summary>
        ''' Read all data on view vcfgLISMappings.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 04/03/2013
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection, Optional pLanguage As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myvcfgLISMappingsDAO As New vcfgLISMappingsDAO
                        myGlobalDataTO = myvcfgLISMappingsDAO.ReadAll(dbConnection, pLanguage)
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "LISMappingsDelegate.ReadAll", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Update the LIS Value on MasterData table.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pLISMappingsDS">LISMappingsDS.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 04/03/2013
        ''' </remarks>
        Public Function UpdateLISValues(ByVal pDBConnection As SqlClient.SqlConnection, pLISMappingsDS As LISMappingsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim myMasterDataDelegate As New MasterDataDelegate
                        'Update the MasterDelegate
                        For Each LISMapplingsRow As LISMappingsDS.vcfgLISMappingRow In pLISMappingsDS.vcfgLISMapping.Rows
                            'Update the LIS value. on masterData.
                            myGlobalDataTO = myMasterDataDelegate.UpdateLISValueByTestID(dbConnection, _
                                                                  LISMapplingsRow.ValueType, LISMapplingsRow.ValueId, LISMapplingsRow.LISValue)
                        Next

                        If (Not myGlobalDataTO.HasError) Then
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

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "LISMappingsDelegate.UpdateLISValues", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Gets the LIS Value for the informed Sample Type
        ''' </summary>
        ''' <param name="pConfigMappingDS"></param>
        ''' <param name="pBiosystemsName"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 22/03/2013</remarks>
        Public Function GetLISSampleType(ByVal pConfigMappingDS As LISMappingsDS, _
                                         ByVal pBiosystemsName As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Try

                Dim myMapConfigRows As List(Of LISMappingsDS.vcfgLISMappingRow)
                myMapConfigRows = (From r In pConfigMappingDS.vcfgLISMapping _
                                    Where r.ValueType = "SAMPLE_TYPES" _
                                    And r.ValueId.ToUpperBS = pBiosystemsName.ToUpperBS _
                                    Select r).ToList()

                If myMapConfigRows.Count > 0 Then
                    If Not myMapConfigRows(0).IsLISValueNull AndAlso myMapConfigRows(0).LISValue.Length > 0 Then
                        resultData.SetDatos = myMapConfigRows(0).LISValue
                    Else
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                    End If
                End If

                myMapConfigRows = Nothing

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "LISMappingsDelegate.GetLISSampleType", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets the LIS Value for the informed Units
        ''' </summary>
        ''' <param name="pConfigMappingDS"></param>
        ''' <param name="pBiosystemsName"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 22/03/2013
        ''' AG 24/04/2013 - 1st apply LINQ over LongName if not results matching apply over ValueID (used in process of export to history)
        ''' </remarks>
        Public Function GetLISUnits(ByVal pConfigMappingDS As LISMappingsDS, _
                                         ByVal pBiosystemsName As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Try

                Dim myMapUnitsRows As List(Of LISMappingsDS.vcfgLISMappingRow)
                myMapUnitsRows = (From r In pConfigMappingDS.vcfgLISMapping _
                                  Where r.ValueType = "TEST_UNITS" _
                                  And r.LongName.ToUpperBS = pBiosystemsName.ToUpperBS _
                                  Select r).ToList()

                resultData.SetDatos = String.Empty
                If myMapUnitsRows.Count > 0 Then
                    If Not myMapUnitsRows(0).IsLISValueNull Then
                        resultData.SetDatos = myMapUnitsRows(0).LISValue
                    End If
                Else
                    'Used in export to history process
                    myMapUnitsRows = (From r In pConfigMappingDS.vcfgLISMapping _
                                      Where r.ValueType = "TEST_UNITS" _
                                      And r.ValueId.ToUpperBS = pBiosystemsName.ToUpperBS _
                                      Select r).ToList()
                    If myMapUnitsRows.Count > 0 Then
                        If Not myMapUnitsRows(0).IsLISValueNull Then
                            resultData.SetDatos = myMapUnitsRows(0).LISValue
                        End If
                    End If
                End If

                myMapUnitsRows = Nothing

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "LISMappingsDelegate.UpdateLISValues", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function



    End Class

End Namespace
