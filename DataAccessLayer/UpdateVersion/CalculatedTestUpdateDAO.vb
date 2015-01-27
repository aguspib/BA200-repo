Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO
    Public Class CalculatedTestUpdateDAO
          

#Region "FUNCTIONS FOR NEW UPDATE VERSION PROCESS"
        ''' <summary>
        ''' Search in FACTORY DB all preloaded Calculated Tests that do not exists in CUSTOMER DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a CalculatedTestsDS with the list of BiosystemsIDs of Calculated Tests added in FACTORY DB</returns>
        ''' <remarks>
        ''' Created by:  SA 15/10/2014 - BA-1944 (SubTask BA-2014)
        ''' </remarks>
        Public Function GetNewFactoryTests(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT BiosystemsID FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparCalculatedTests] " & vbCrLf & _
                                                " EXCEPT " & vbCrLf & _
                                                " SELECT BiosystemsID FROM [Ax00].[dbo].[tparCalculatedTests] " & vbCrLf & _
                                                " WHERE  PreloadedCalculatedTest = 1 " & vbCrLf

                        Dim newCalcTestsDS As New CalculatedTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(newCalcTestsDS.tparCalculatedTests)
                            End Using
                        End Using

                        resultData.SetDatos = newCalcTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsUpdateDAO.GetNewFactoryTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search in FACTORY DB the basic definition data of the specified preloaded Calculated Test (BiosystemsID is used as identifier of the
        ''' Calculated Test)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pID">Unique Calculated Test Identifier for preloaded Calculated Tests</param>
        ''' <param name="pSearchByBiosystemsID">When TRUE, the search is executed by field BiosystemsID instead of by field CalcTestID</param>
        ''' <returns>GlobalDataTO containing a CalculatedTestsDS with data of the specified preloaded Calculated Test in FACTORY DB</returns>
        ''' <remarks>
        ''' Created by:  SA 15/10/2014 - BA-1944 (SubTask BA-2014)
        ''' </remarks>
        Public Function GetDataInFactoryDB(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pID As Integer, ByVal pSearchByBiosystemsID As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparCalculatedTests] " & vbCrLf

                        If (pSearchByBiosystemsID) Then
                            cmdText &= " WHERE BiosystemsID = " & pID.ToString & vbCrLf
                        Else
                            cmdText &= " WHERE CalcTestID = " & pID.ToString & vbCrLf
                        End If

                        Dim factoryCalcTestDS As New CalculatedTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(factoryCalcTestDS.tparCalculatedTests)
                            End Using
                        End Using

                        resultData.SetDatos = factoryCalcTestDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsUpdateDAO.GetDataInFactoryDB", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search in FACTORY DB all values contained in the Formula of the specified preloaded Calculated Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTestID">Calculated Test Identifier in FACTORY DB</param>
        ''' <returns>GlobalDataTO containing a FormulaDS with all data of the Formula of the specified preloaded Calculated Test in FACTORY DB</returns>
        ''' <remarks>
        ''' Created by:  SA 15/10/2014 - BA-1944 (SubTask BA-2014)
        ''' </remarks>
        Public Function GetFormulaInFactoryDB(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparFormulas] " & vbCrLf & _
                                                " WHERE  CalcTestID = " & pCalcTestID.ToString & vbCrLf

                        Dim factoryFormulaDS As New FormulasDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(factoryFormulaDS.tparFormulas)
                            End Using
                        End Using

                        resultData.SetDatos = factoryFormulaDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsUpdateDAO.GetFormulaInFactoryDB", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search in FACTORY DB all Reference Ranges defined for the specified preloaded Calculated Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTestID">Calculated Test Identifier in FACTORY DB</param>
        ''' <returns>GlobalDataTO containing a TestRefRangesDS with all Reference Ranges defined for the specified preloaded Calculated Test in FACTORY DB</returns>
        ''' <remarks>
        ''' Created by:  SA 15/10/2014 - BA-1944 (SubTask BA-2014)
        ''' </remarks>
        Public Function GetRefRangesInFactoryDB(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT *, 1 AS IsNew FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparTestRefRanges] " & vbCrLf & _
                                                " WHERE  TestType = 'CALC' " & vbCrLf & _
                                                " AND    TestID = " & pCalcTestID.ToString & vbCrLf

                        Dim factoryRefRangesDS As New TestRefRangesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(factoryRefRangesDS.tparTestRefRanges)
                            End Using
                        End Using

                        resultData.SetDatos = factoryRefRangesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsUpdateDAO.GetRefRangesInFactoryDB", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Execute the query to search all CALC Tests that should be deleted (those preloaded CALC Tests that exist in CUSTOMER DB but not in FACTORY DB)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a CalculatedTestsDS with the list of identifiers of CALC Tests that have to be deleted from CUSTOMER DB</returns>
        ''' <remarks>
        ''' Created by: SA 16/10/2014 - BA-1944 (SubTask BA-2017)
        ''' </remarks>
        Public Function GetDeletedPreloadedCALCTests(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT BiosystemsID FROM [Ax00].[dbo].[tparCalculatedTests] " & vbCrLf & _
                                                " WHERE  PreloadedCalculatedTest = 1 " & vbCrLf & _
                                                " EXCEPT " & vbCrLf & _
                                                " SELECT BiosystemsID FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparCalculatedTests] " & vbCrLf & _
                                                " ORDER BY BiosystemsID DESC "

                        Dim customerCalcTestsDS As New CalculatedTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(customerCalcTestsDS.tparCalculatedTests)
                            End Using
                        End Using

                        resultData.SetDatos = customerCalcTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestUpdateDAO.GetDeletedPreloadedCALCTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search in FACTORY DB all CALC Tests that exist in CUSTOMER DB but for which at least one of the relevant fields have been changed
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a CalculatedTestsDS with value of the relevant fields in FACTORY DB for modified CALC Tests</returns>
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
                        Dim cmdText As String = " SELECT BiosystemsID, UniqueSampleType, SampleType, ActiveRangeType " & vbCrLf & _
                                                " FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparCalculatedTests] " & vbCrLf & _
                                                " EXCEPT " & vbCrLf & _
                                                " SELECT BiosystemsID, UniqueSampleType, SampleType, ActiveRangeType " & vbCrLf & _
                                                " FROM   [Ax00].[dbo].[tparCalculatedTests] " & vbCrLf

                        Dim factoryCalcTestsDS As New CalculatedTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(factoryCalcTestsDS.tparCalculatedTests)
                            End Using
                        End Using

                        resultData.SetDatos = factoryCalcTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestUpdateDAO.GetUpdatedFactoryTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace

#Region "FUNCTIONS FOR OLD UPDATE VERSION PROCESS (NOT USED)"
' ''' <summary>
' ''' Gets the list of all calculated Tests that in Factory DB appears as Preloaded and data don't match with client data
' ''' </summary>
' ''' <param name="pDBConnection">Open DB Connection</param>
' ''' <returns>
' ''' All the results are the calc test that can be updated or created
' ''' </returns>
' ''' <remarks>
' ''' Created by: DL - 28/01/2013
' ''' Modified by: XB - 15/02/2013 - Force order by CALC tests inside Formula (Bugs tracking #1134)  !!! CANCELED !!! --> Sorting is commented by now
' ''' Modified by: SG - 19/02/2013 - Add new field BiosystemsID (Bugs tracking #1134)
' ''' </remarks>
'Public Shared Function GetPreloadedCALCTestsDistinctInClient(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
'    Dim resultData As GlobalDataTO = Nothing
'    Dim dbConnection As SqlClient.SqlConnection = Nothing

'    Try
'        resultData = GetOpenDBConnection(pDBConnection)
'        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
'            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
'            If (Not dbConnection Is Nothing) Then
'                'CALC Tests
'                'All data in Ax00_TEM DataBase different from data in Ax00
'                Dim cmdText As String = ""
'                cmdText &= "Select CT.CalcTestID" & vbCrLf
'                cmdText &= "   	  ,CT.CalcTestName " & vbCrLf
'                cmdText &= "   	  ,CT.CalcTestLongName " & vbCrLf
'                cmdText &= "   	  ,CT.MeasureUnit " & vbCrLf
'                cmdText &= " 	  ,CT.UniqueSampleType " & vbCrLf
'                cmdText &= " 	  ,CT.SampleType " & vbCrLf
'                cmdText &= " 	  ,CT.Decimals " & vbCrLf
'                cmdText &= " 	  ,CT.FormulaText " & vbCrLf
'                cmdText &= " 	  ,CT.ActiveRangeType " & vbCrLf
'                cmdText &= " 	  ,CT.EnableStatus " & vbCrLf
'                cmdText &= " 	  ,CT.PreloadedCalculatedTest " & vbCrLf
'                cmdText &= " 	  ,CT.BiosystemsID " & vbCrLf 'SGM 19/02/2013
'                cmdText &= " 	  ,TRR.RangeType " & vbCrLf
'                cmdText &= " 	  ,TRR.Gender " & vbCrLf
'                cmdText &= " 	  ,TRR.AgeUnit " & vbCrLf
'                cmdText &= " 	  ,TRR.AgeRangeFrom " & vbCrLf
'                cmdText &= " 	  ,TRR.AgeRangeTo " & vbCrLf
'                cmdText &= " 	  ,TRR.NormalLowerLimit " & vbCrLf
'                cmdText &= " 	  ,TRR.NormalUpperLimit " & vbCrLf
'                cmdText &= " 	  ,TRR.BorderLineLowerLimit " & vbCrLf
'                cmdText &= " 	  ,TRR.BorderLineUpperLimit " & vbCrLf
'                'cmdText &= " 	  ,FF.TestType " & vbCrLf
'                cmdText &= "  FROM " & GlobalBase.TemporalDBName & ".dbo.tparCalculatedTests ct " & vbCrLf
'                'cmdText &= "       LEFT JOIN  " & GlobalBase.TemporalDBName & ".dbo.tparFormulas FF on (FF.TestType = 'CALC' and FF.CalcTestID = CT.CalcTestID) " & vbCrLf
'                cmdText &= "       LEFT JOIN  " & GlobalBase.TemporalDBName & ".dbo.tparTestRefRanges TRR on (TRR.TestType = 'CALC' and TRR.TestID = CT.CalcTestID) " & vbCrLf
'                'cmdText &= " WHERE CT.PreloadedCalculatedTest = 1 " & vbCrLf
'                cmdText &= "EXCEPT " & vbCrLf
'                cmdText &= "Select CT.CalcTestID " & vbCrLf
'                cmdText &= "   	  ,CT.CalcTestName " & vbCrLf
'                cmdText &= "   	  ,CT.CalcTestLongName " & vbCrLf
'                cmdText &= " 	  ,CT.MeasureUnit " & vbCrLf
'                cmdText &= " 	  ,CT.UniqueSampleType " & vbCrLf
'                cmdText &= " 	  ,CT.SampleType " & vbCrLf
'                cmdText &= " 	  ,CT.Decimals " & vbCrLf
'                cmdText &= " 	  ,CT.FormulaText " & vbCrLf
'                cmdText &= " 	  ,CT.ActiveRangeType " & vbCrLf
'                cmdText &= " 	  ,CT.EnableStatus " & vbCrLf
'                cmdText &= " 	  ,CT.PreloadedCalculatedTest " & vbCrLf
'                cmdText &= " 	  ,CT.BiosystemsID " & vbCrLf 'SGM 19/02/2013
'                cmdText &= " 	  ,TRR.RangeType " & vbCrLf
'                cmdText &= " 	  ,TRR.Gender " & vbCrLf
'                cmdText &= " 	  ,TRR.AgeUnit " & vbCrLf
'                cmdText &= " 	  ,TRR.AgeRangeFrom " & vbCrLf
'                cmdText &= " 	  ,TRR.AgeRangeTo " & vbCrLf
'                cmdText &= " 	  ,TRR.NormalLowerLimit " & vbCrLf
'                cmdText &= " 	  ,TRR.NormalUpperLimit " & vbCrLf
'                cmdText &= " 	  ,TRR.BorderLineLowerLimit " & vbCrLf
'                cmdText &= " 	  ,TRR.BorderLineUpperLimit " & vbCrLf
'                'cmdText &= " 	  ,FF.TestType " & vbCrLf
'                cmdText &= " FROM dbo.tparCalculatedTests ct " & vbCrLf
'                'cmdText &= "      LEFT JOIN  " & GlobalBase.TemporalDBName & ".dbo.tparFormulas FF on (FF.TestType = 'CALC' and FF.CalcTestID = CT.CalcTestID) " & vbCrLf
'                cmdText &= "      LEFT JOIN dbo.tparTestRefRanges TRR on (TRR.TestType = 'CALC' and TRR.TestID = ct.CalcTestID) " & vbCrLf

'                'cmdText &= " ORDER BY FF.TestType "

'                Dim compCalcTests As New DataSet
'                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
'                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
'                        dbDataAdapter.Fill(compCalcTests)
'                    End Using
'                End Using

'                resultData.SetDatos = compCalcTests
'                resultData.HasError = False
'            End If
'        End If

'    Catch ex As Exception
'        resultData = New GlobalDataTO()
'        resultData.HasError = True
'        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
'        resultData.ErrorMessage = ex.Message

'        'Dim myLogAcciones As New ApplicationLogManager()
'        GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsUpdateDAO.GetPreloadedCALCTestsDistinctInClient", EventLogEntryType.Error, False)
'    Finally
'        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
'    End Try
'    Return resultData
'End Function

' ''' <summary>
' ''' Gets the list of all calculated Tests that in local DB appears as Preloaded and data don't match with Factory DB
' ''' </summary>
' ''' <param name="pDBConnection">Open DB Connection</param>
' ''' <returns>
' ''' All the results are the calc test that can be removed or ignored
' ''' </returns>
' ''' <remarks>
' ''' Created by: JB - 29/01/2013
' ''' Modified by XB - 15/02/2013 - delete Ref Ranges into comparasion because would be usual have differences between Local and Factory DB (Bugs tracking #1134)
' ''' Modified by: SG - 19/02/2013 - Add new field BiosystemsID (Bugs tracking #1134)
' ''' </remarks>
'Public Shared Function GetPreloadedCALCTestsDistinctInFactory(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
'    Dim resultData As GlobalDataTO = Nothing
'    Dim dbConnection As SqlClient.SqlConnection = Nothing

'    Try
'        resultData = GetOpenDBConnection(pDBConnection)
'        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
'            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
'            If (Not dbConnection Is Nothing) Then
'                'CALC Tests
'                'All data in Ax00 DataBase different from data in Ax00_TEM
'                Dim cmdText As String = ""
'                cmdText &= "Select ct.CalcTestID " & vbCrLf
'                cmdText &= "   	  ,CT.CalcTestName " & vbCrLf
'                cmdText &= "   	  ,CT.CalcTestLongName " & vbCrLf
'                cmdText &= "   	  ,ct.MeasureUnit " & vbCrLf
'                cmdText &= " 	  ,ct.UniqueSampleType " & vbCrLf
'                cmdText &= " 	  ,ct.SampleType " & vbCrLf
'                cmdText &= " 	  ,ct.Decimals " & vbCrLf
'                cmdText &= " 	  ,ct.FormulaText " & vbCrLf
'                'cmdText &= " 	  ,ct.ActiveRangeType " & vbCrLf
'                cmdText &= " 	  ,ct.EnableStatus " & vbCrLf
'                cmdText &= " 	  ,ct.PreloadedCalculatedTest " & vbCrLf
'                cmdText &= " 	  ,CT.BiosystemsID " & vbCrLf 'SGM 19/02/2013
'                'cmdText &= " 	  ,trr.RangeType " & vbCrLf
'                'cmdText &= " 	  ,trr.Gender " & vbCrLf
'                'cmdText &= " 	  ,trr.AgeUnit " & vbCrLf
'                'cmdText &= " 	  ,trr.AgeRangeFrom " & vbCrLf
'                'cmdText &= " 	  ,trr.AgeRangeTo " & vbCrLf
'                'cmdText &= " 	  ,trr.NormalLowerLimit " & vbCrLf
'                'cmdText &= " 	  ,trr.NormalUpperLimit " & vbCrLf
'                'cmdText &= " 	  ,trr.BorderLineLowerLimit " & vbCrLf
'                'cmdText &= " 	  ,trr.BorderLineUpperLimit " & vbCrLf
'                cmdText &= "  FROM dbo.tparCalculatedTests ct " & vbCrLf
'                'cmdText &= "        LEFT JOIN dbo.tparTestRefRanges trr on trr.TestType = 'CALC' and trr.TestID=ct.CalcTestID " & vbCrLf
'                ''cmdText &= " WHERE ct.PreloadedCalculatedTest = 1 " & vbCrLf
'                cmdText &= "EXCEPT " & vbCrLf
'                cmdText &= "Select ct.CalcTestID " & vbCrLf
'                cmdText &= "   	  ,CT.CalcTestName " & vbCrLf
'                cmdText &= "   	  ,CT.CalcTestLongName " & vbCrLf
'                cmdText &= "   	  ,ct.MeasureUnit " & vbCrLf
'                cmdText &= " 	  ,ct.UniqueSampleType " & vbCrLf
'                cmdText &= " 	  ,ct.SampleType " & vbCrLf
'                cmdText &= " 	  ,ct.Decimals " & vbCrLf
'                cmdText &= " 	  ,ct.FormulaText " & vbCrLf
'                'cmdText &= " 	  ,ct.ActiveRangeType " & vbCrLf
'                cmdText &= " 	  ,ct.EnableStatus " & vbCrLf
'                cmdText &= " 	  ,ct.PreloadedCalculatedTest " & vbCrLf
'                cmdText &= " 	  ,CT.BiosystemsID " & vbCrLf 'SGM 19/02/2013
'                'cmdText &= " 	  ,trr.RangeType " & vbCrLf
'                'cmdText &= " 	  ,trr.Gender " & vbCrLf
'                'cmdText &= " 	  ,trr.AgeUnit " & vbCrLf
'                'cmdText &= " 	  ,trr.AgeRangeFrom " & vbCrLf
'                'cmdText &= " 	  ,trr.AgeRangeTo " & vbCrLf
'                'cmdText &= " 	  ,trr.NormalLowerLimit " & vbCrLf
'                'cmdText &= " 	  ,trr.NormalUpperLimit " & vbCrLf
'                'cmdText &= " 	  ,trr.BorderLineLowerLimit " & vbCrLf
'                'cmdText &= " 	  ,trr.BorderLineUpperLimit " & vbCrLf
'                cmdText &= " FROM " & GlobalBase.TemporalDBName & ".dbo.tparCalculatedTests ct " & vbCrLf
'                'cmdText &= "    LEFT JOIN " & GlobalBase.TemporalDBName & ".dbo.tparTestRefRanges trr on trr.TestType = 'CALC' and trr.TestID=ct.CalcTestID " & vbCrLf

'                Dim compCalcTests As New DataSet
'                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
'                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
'                        dbDataAdapter.Fill(compCalcTests)
'                    End Using
'                End Using

'                resultData.SetDatos = compCalcTests
'                resultData.HasError = False
'            End If
'        End If
'    Catch ex As Exception
'        resultData = New GlobalDataTO()
'        resultData.HasError = True
'        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
'        resultData.ErrorMessage = ex.Message

'        'Dim myLogAcciones As New ApplicationLogManager()
'        GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestsUpdateDAO.GetPreloadedCALCTestsDistinctInClient", EventLogEntryType.Error, False)
'    Finally
'        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
'    End Try
'    Return resultData
'End Function
#End Region