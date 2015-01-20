Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tparOffSystemTestSamplesDAO
        Inherits DAOBase

#Region "CRUD"
        ''' <summary>
        ''' Add a new SampleType (and related data) for an specific OFF-SYSTEM Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestSampleTypesRow">Row of a typed DataSet OffSystemTestSamplesDS containing the data of the SampleType to 
        '''                                   link to the specified OFF-SYSTEM Test</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 29/11/2010
        ''' Modified by: SA 03/11/2010
        '''</remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestSampleTypesRow As OffSystemTestSamplesDS.tparOffSystemTestSamplesRow) _
                               As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText &= " INSERT INTO tparOffSystemTestSamples (OffSystemTestID, SampleType, DefaultValue, ActiveRangeType, TS_User, TS_DateTime) " & vbCrLf
                    cmdText &= " VALUES (" & pTestSampleTypesRow.OffSystemTestID & ", " & vbCrLf
                    cmdText &= "        '" & pTestSampleTypesRow.SampleType.ToString().Replace("'", "''") & "', " & vbCrLf

                    If (pTestSampleTypesRow.IsDefaultValueNull OrElse pTestSampleTypesRow.DefaultValue.ToString = "") Then
                        cmdText &= " NULL, " & vbCrLf
                    Else
                        cmdText &= " N'" & pTestSampleTypesRow.DefaultValue.ToString().Replace("'", "''") & "', " & vbCrLf
                    End If

                    If (pTestSampleTypesRow.IsActiveRangeTypeNull OrElse pTestSampleTypesRow.ActiveRangeType.ToString = "") Then
                        cmdText &= " NULL, " & vbCrLf
                    Else
                        cmdText &= " '" & pTestSampleTypesRow.ActiveRangeType.ToString().Replace("'", "''") & "', " & vbCrLf
                    End If

                    If (pTestSampleTypesRow.IsTS_UserNull) Then
                        'Dim currentSession As New GlobalBase
                        cmdText &= " N'" & GlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', " & vbCrLf
                    Else
                        cmdText &= " N'" & pTestSampleTypesRow.TS_User.Trim.Replace("'", "''") & "', " & vbCrLf
                    End If

                    If (pTestSampleTypesRow.IsTS_DateTimeNull) Then
                        cmdText &= " '" & DateTime.Now.ToString("yyyyMMdd HH:mm:ss") & "') "
                    Else
                        cmdText &= " '" & CType(pTestSampleTypesRow.TS_DateTime.ToString(), DateTime).ToString("yyyyMMdd HH:mm:ss") & "') "
                    End If

                    'Execute the SQL Sentence
                    Dim dbCmd As New SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery
                    If (resultData.AffectedRecords = 1) Then
                        resultData.HasError = False
                        resultData.SetDatos = pTestSampleTypesRow
                    Else
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparOffSystemTestSamplesDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update SampleType data for the specified OFF-SYSTEM Test 
        ''' </summary>
        ''' <param name="pTestSampleTypesRow">Row of a typed DataSet OffSystemTestSamplesDS containing the data of the SampleType linked 
        '''                                   to the specified OFF-SYSTEM Test</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: DL 29/11/2010
        ''' Modified by: SA 03/11/2010
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestSampleTypesRow As OffSystemTestSamplesDS.tparOffSystemTestSamplesRow) _
                               As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText &= " UPDATE tparOffSystemTestSamples " & vbCrLf
                    cmdText &= " SET    SampleType = '" & pTestSampleTypesRow.SampleType.ToString.Replace("'", "''") & "', " & vbCrLf

                    If (pTestSampleTypesRow.IsDefaultValueNull OrElse pTestSampleTypesRow.DefaultValue = "") Then
                        cmdText &= " DefaultValue = NULL, " & vbCrLf
                    Else
                        cmdText &= " DefaultValue = N'" & pTestSampleTypesRow.DefaultValue.ToString().Replace("'", "''") & "', " & vbCrLf
                    End If

                    If (pTestSampleTypesRow.IsActiveRangeTypeNull OrElse pTestSampleTypesRow.ActiveRangeType = "") Then
                        cmdText &= " ActiveRangeType = NULL, " & vbCrLf
                    Else
                        cmdText &= " ActiveRangeType = N'" & pTestSampleTypesRow.ActiveRangeType.ToString().Replace("'", "''") & "', " & vbCrLf
                    End If

                    If (pTestSampleTypesRow.IsTS_UserNull) Then
                        'Dim currentSession As New GlobalBase
                        cmdText &= " TS_User = N'" & GlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', " & vbCrLf
                    Else
                        cmdText &= " TS_User = N'" & pTestSampleTypesRow.TS_User.Trim.Replace("'", "''") & "', " & vbCrLf
                    End If

                    If (pTestSampleTypesRow.IsTS_DateTimeNull) Then
                        cmdText &= " TS_DateTime = '" & DateTime.Now.ToString("yyyyMMdd HH:mm:ss") & "' " & vbCrLf
                    Else
                        cmdText &= " TS_DateTime = '" & CType(pTestSampleTypesRow.TS_DateTime.ToString(), DateTime).ToString("yyyyMMdd HH:mm:ss") & "' " & vbCrLf
                    End If

                    cmdText &= " WHERE OffSystemTestID = " & pTestSampleTypesRow.OffSystemTestID & vbCrLf

                    'AG 10/12/2010 - By now the offsystem test can not define several SampleTypes
                    'cmdText &= "   AND UPPER(SampleType) = UPPER('" & pTestSampleTypesRow.SampleType.ToString.Replace("'", "''") & "') "
                    'END AG 10/12/2010

                    'Execute the SQL Sentence
                    Dim dbCmd As New SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery
                    If (resultData.AffectedRecords = 1) Then
                        resultData.HasError = False
                        resultData.SetDatos = pTestSampleTypesRow
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparOffSystemTestSamplesDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all SampleTypes  linked to the specified OFF-SYSTEM Test (currently only a SampleType is allowed)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOffSystemTestID">OFF-SYSTEM Test Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: DL 01/12/2010
        ''' </remarks>
        Public Function DeleteByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOffSystemTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = " DELETE FROM tparOffSystemTestSamples " & _
                              " WHERE  OffSystemTestID = " & pOffSystemTestID

                    'Execute the SQL Sentence
                    Dim dbCmd As New SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery
                    If (resultData.AffectedRecords > 0) Then
                        resultData.HasError = False
                    Else
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparOffSystemTestSamplesDAO.DeleteByTestID", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all defined SampleTypes for the specified OFF-SYSTEM Test. Besides, if a SampleType is informed,
        ''' then verify if the specified SampleType exists for the OFF-SYSTEM Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOffSystemTestID">OffSystemTest Identifier</param>
        ''' <param name="pSampleType">Sample Type code. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OffSystemTestSamplesDS with the list of Sample Types
        '''          linked to the specified OFF-SYSTEM Test</returns>
        ''' <remarks>
        ''' Created by:  DL 25/11/2010
        ''' Modified by: XB 04/06/2013 - Update query to return also tparTestSamples without multilanguage translation
        ''' </remarks>
        Public Function GetByOffSystemTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOffSystemTestID As Integer, _
                                             Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        'Dim myGlobalbase As New GlobalBase

                        ' XB 04/06/2013
                        'cmdText &= " SELECT ITS.*, ITS.SampleType + '-' + MR.ResourceText AS SampleTypeDesc " & vbCrLf
                        'cmdText &= " FROM   tparOffSystemTestSamples ITS INNER JOIN tcfgMasterData MD ON ITS.SampleType = MD.ItemID " & vbCrLf
                        'cmdText &= "                                     INNER JOIN tfmwMultiLanguageResources MR ON MD.ResourceID = MR.ResourceID " & vbCrLf
                        'cmdText &= " WHERE  MD.SubTableID = '" & GlobalEnumerates.MasterDataEnum.SAMPLE_TYPES.ToString & "' " & vbCrLf
                        'cmdText &= " AND    ITS.OffSystemTestID = " & pOffSystemTestID & vbCrLf
                        'cmdText &= " AND    MR.LanguageID = '" & GlobalBase.GetSessionInfo.ApplicationLanguage.Trim & "' " & vbCrLf

                        'If (pSampleType <> "") Then cmdText &= " AND ITS.SampleType = '" & pSampleType & "'" & vbCrLf
                        'cmdText &= " ORDER BY MD.Position "
                        cmdText &= " SELECT ITS.*, ITS.SampleType + '-' + MR.ResourceText AS SampleTypeDesc, MD.Position " & vbCrLf
                        cmdText &= " FROM   tparOffSystemTestSamples ITS INNER JOIN tcfgMasterData MD ON ITS.SampleType = MD.ItemID " & vbCrLf
                        cmdText &= "                                     INNER JOIN tfmwMultiLanguageResources MR ON MD.ResourceID = MR.ResourceID " & vbCrLf
                        cmdText &= " WHERE  MD.SubTableID = '" & GlobalEnumerates.MasterDataEnum.SAMPLE_TYPES.ToString & "' " & vbCrLf
                        cmdText &= " AND    ITS.OffSystemTestID = " & pOffSystemTestID & vbCrLf
                        cmdText &= " AND    MR.LanguageID = '" & GlobalBase.GetSessionInfo.ApplicationLanguage.Trim & "' " & vbCrLf
                        If (pSampleType <> "") Then cmdText &= " AND ITS.SampleType = '" & pSampleType & "'" & vbCrLf

                        cmdText &= " UNION " & vbCrLf

                        cmdText &= " SELECT ITS.*, ITS.SampleType + '-' + MD.FixedItemDesc AS SampleTypeDesc, MD.Position " & vbCrLf
                        cmdText &= " FROM   tparOffSystemTestSamples ITS INNER JOIN tcfgMasterData MD ON ITS.SampleType = MD.ItemID " & vbCrLf
                        cmdText &= " WHERE  MD.SubTableID = '" & GlobalEnumerates.MasterDataEnum.SAMPLE_TYPES.ToString & "' " & vbCrLf
                        cmdText &= " AND    ITS.OffSystemTestID = " & pOffSystemTestID & vbCrLf
                        cmdText &= " AND   MD.MultiLanguageFlag = 0 " & vbCrLf
                        If (pSampleType <> "") Then cmdText &= " AND ITS.SampleType = '" & pSampleType & "'" & vbCrLf

                        cmdText &= " ORDER BY Position "
                        ' XB 04/06/2013

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim myOffSystemTestSamplesDS As New OffSystemTestSamplesDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)

                        dbDataAdapter.Fill(myOffSystemTestSamplesDS.tparOffSystemTestSamples)
                        resultData.SetDatos = myOffSystemTestSamplesDS
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparOffSystemTestSamplesDAO.GetByOffSystemTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class

End Namespace
