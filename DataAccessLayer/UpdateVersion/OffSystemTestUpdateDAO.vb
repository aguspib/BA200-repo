Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO
    Public Class OffSystemTestUpdateDAO
        Inherits DAOBase

        ''' <summary>
        ''' Search in FACTORY DB all preloaded OffSystem Tests that do not exists in CUSTOMER DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing an OffSystemTestsDS with the list of BiosystemsIDs of OffSystem Tests added in FACTORY DB</returns>
        ''' <remarks>
        ''' Created by:  SA 20/11/2014 - BA-2105
        ''' </remarks>
        Public Function GetNewFactoryTests(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT BiosystemsID FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparOffSystemTests] " & vbCrLf & _
                                                " EXCEPT " & vbCrLf & _
                                                " SELECT BiosystemsID FROM [Ax00].[dbo].[tparOffSystemTests] " & vbCrLf & _
                                                " WHERE  PreloadedOffSystemTest = 1 " & vbCrLf

                        Dim newOFFSTestsDS As New OffSystemTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(newOFFSTestsDS.tparOffSystemTests)
                            End Using
                        End Using

                        resultData.SetDatos = newOFFSTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "OffSystemTestUpdateDAO.GetNewFactoryTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search in FACTORY DB the basic definition data of the specified preloaded OffSystem Test (BiosystemsID is used as identifier of the
        ''' OffSystem Test)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pID">Unique OffSystem Test Identifier for preloaded OffSystem Tests</param>
        ''' <param name="pSearchByBiosystemsID">When TRUE, the search is executed by field BiosystemsID instead of by field OffSystemTestID</param>
        ''' <returns>GlobalDataTO containing an OffSystemTestsDS with data of the specified preloaded OffSystem Test in FACTORY DB</returns>
        ''' <remarks>
        ''' Created by:  SA 20/11/2014 - BA-2105
        ''' </remarks>
        Public Function GetDataInFactoryDB(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pID As Integer, ByVal pSearchByBiosystemsID As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT OT.*, OTS.SampleType, OTS.DefaultValue, OTS.ActiveRangeType " & vbCrLf & _
                                                " FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparOffSystemTests] OT INNER JOIN " & GlobalBase.TemporalDBName & ".[dbo].[tparOffSystemTestSamples] OTS " & vbCrLf & _
                                                                                                                             " ON OT.OffSystemTestID = OTS.OffSystemTestID " & vbCrLf

                        If (pSearchByBiosystemsID) Then
                            cmdText &= " WHERE OT.BiosystemsID = " & pID.ToString & vbCrLf
                        Else
                            cmdText &= " WHERE OT.OffSystemTestID = " & pID.ToString & vbCrLf
                        End If

                        Dim factoryOFFSTestDS As New OffSystemTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(factoryOFFSTestDS.tparOffSystemTests)
                            End Using
                        End Using

                        resultData.SetDatos = factoryOFFSTestDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "OffSystemTestUpdateDAO.GetDataInFactoryDB", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Execute the query to search all OFFS Tests that should be deleted (those preloaded OFFS Tests that exist in CUSTOMER DB but not in FACTORY DB)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing an OffSystemTestsDS with the list of identifiers of OFFS Tests that have to be deleted from CUSTOMER DB</returns>
        ''' <remarks>
        ''' Created by: SA 20/11/2014 - BA-2105
        ''' </remarks>
        Public Function GetDeletedPreloadedOFFSTests(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT BiosystemsID FROM [Ax00].[dbo].[tparOffSystemTests] " & vbCrLf & _
                                                " WHERE  PreloadedOffSystemTest = 1 " & vbCrLf & _
                                                " EXCEPT " & vbCrLf & _
                                                " SELECT BiosystemsID FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparOffSystemTests] " & vbCrLf & _
                                                " ORDER BY BiosystemsID DESC "

                        Dim customerOFFSTestsDS As New OffSystemTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(customerOFFSTestsDS.tparOffSystemTests)
                            End Using
                        End Using

                        resultData.SetDatos = customerOFFSTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "OffSystemTestUpdateDAO.GetDeletedPreloadedOFFSTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search in FACTORY DB all OFFS Tests that exist in CUSTOMER DB but for which at least one of the relevant fields have been changed
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a OffSystemTestSamplesDS with value of the relevant fields in FACTORY DB for modified CALC Tests</returns>
        ''' <remarks>
        ''' Created by: SA 16/10/2014 - BA-1944 (SubTask BA-2017)
        ''' </remarks>
        Public Function GetUpdatedFactoryTests(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT OT.BiosystemsID, OT.ResultType, OT.Units, OT.Decimals, OTS.SampleType, OTS.DefaultValue, OTS.ActiveRangeType " & vbCrLf & _
                                                " FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparOffSystemTests] OT INNER JOIN " & GlobalBase.TemporalDBName & ".[dbo].[tparOffSystemTestSamples] OTS " & vbCrLf & _
                                                                                                                             " ON OT.OffSystemTestID = OTS.OffSystemTestID " & vbCrLf & _
                                                " EXCEPT " & vbCrLf & _
                                                " SELECT OT.BiosystemsID, OT.ResultType, OT.Units, OT.Decimals, OTS.SampleType, OTS.DefaultValue, OTS.ActiveRangeType " & vbCrLf & _
                                                " FROM   [Ax00].[dbo].[tparOffSystemTests] OT INNER JOIN [Ax00].[dbo].[tparOffSystemTestSamples] OTS ON OT.OffSystemTestID = OTS.OffSystemTestID " & vbCrLf & _
                                                " WHERE  OT.PreloadedOffSystemTest = 1 " & vbCrLf

                        Dim factoryOFFSTestsDS As New OffSystemTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(factoryOFFSTestsDS.tparOffSystemTests)
                            End Using
                        End Using

                        resultData.SetDatos = factoryOFFSTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "OffSystemTestUpdateDAO.GetUpdatedFactoryTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
    End Class
End Namespace
