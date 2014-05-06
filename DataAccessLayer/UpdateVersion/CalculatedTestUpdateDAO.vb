Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class CalculatedTestUpdateDAO
        Inherits DAOBase

        ''' <summary>
        ''' Gets the list of all calculated Tests that in Factory DB appears as Preloaded and data don't match with client data
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>
        ''' All the results are the calc test that can be updated or created
        ''' </returns>
        ''' <remarks>
        ''' Created by: DL - 28/01/2013
        ''' Modified by: XB - 15/02/2013 - Force order by CALC tests inside Formula (Bugs tracking #1134)  !!! CANCELED !!! --> Sorting is commented by now
        ''' Modified by: SG - 19/02/2013 - Add new field BiosystemsID (Bugs tracking #1134)
        ''' </remarks>
        Public Shared Function GetPreloadedCALCTestsDistinctInClient(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'CALC Tests
                        'All data in Ax00_TEM DataBase different from data in Ax00
                        Dim cmdText As String = ""
                        cmdText &= "Select CT.CalcTestID" & vbCrLf
                        cmdText &= "   	  ,CT.CalcTestName " & vbCrLf
                        cmdText &= "   	  ,CT.CalcTestLongName " & vbCrLf
                        cmdText &= "   	  ,CT.MeasureUnit " & vbCrLf
                        cmdText &= " 	  ,CT.UniqueSampleType " & vbCrLf
                        cmdText &= " 	  ,CT.SampleType " & vbCrLf
                        cmdText &= " 	  ,CT.Decimals " & vbCrLf
                        cmdText &= " 	  ,CT.FormulaText " & vbCrLf
                        cmdText &= " 	  ,CT.ActiveRangeType " & vbCrLf
                        cmdText &= " 	  ,CT.EnableStatus " & vbCrLf
                        cmdText &= " 	  ,CT.PreloadedCalculatedTest " & vbCrLf
                        cmdText &= " 	  ,CT.BiosystemsID " & vbCrLf 'SGM 19/02/2013
                        cmdText &= " 	  ,TRR.RangeType " & vbCrLf
                        cmdText &= " 	  ,TRR.Gender " & vbCrLf
                        cmdText &= " 	  ,TRR.AgeUnit " & vbCrLf
                        cmdText &= " 	  ,TRR.AgeRangeFrom " & vbCrLf
                        cmdText &= " 	  ,TRR.AgeRangeTo " & vbCrLf
                        cmdText &= " 	  ,TRR.NormalLowerLimit " & vbCrLf
                        cmdText &= " 	  ,TRR.NormalUpperLimit " & vbCrLf
                        cmdText &= " 	  ,TRR.BorderLineLowerLimit " & vbCrLf
                        cmdText &= " 	  ,TRR.BorderLineUpperLimit " & vbCrLf
                        'cmdText &= " 	  ,FF.TestType " & vbCrLf
                        cmdText &= "  FROM " & GlobalBase.TemporalDBName & ".dbo.tparCalculatedTests ct " & vbCrLf
                        'cmdText &= "       LEFT JOIN  " & GlobalBase.TemporalDBName & ".dbo.tparFormulas FF on (FF.TestType = 'CALC' and FF.CalcTestID = CT.CalcTestID) " & vbCrLf
                        cmdText &= "       LEFT JOIN  " & GlobalBase.TemporalDBName & ".dbo.tparTestRefRanges TRR on (TRR.TestType = 'CALC' and TRR.TestID = CT.CalcTestID) " & vbCrLf
                        'cmdText &= " WHERE CT.PreloadedCalculatedTest = 1 " & vbCrLf
                        cmdText &= "EXCEPT " & vbCrLf
                        cmdText &= "Select CT.CalcTestID " & vbCrLf
                        cmdText &= "   	  ,CT.CalcTestName " & vbCrLf
                        cmdText &= "   	  ,CT.CalcTestLongName " & vbCrLf
                        cmdText &= " 	  ,CT.MeasureUnit " & vbCrLf
                        cmdText &= " 	  ,CT.UniqueSampleType " & vbCrLf
                        cmdText &= " 	  ,CT.SampleType " & vbCrLf
                        cmdText &= " 	  ,CT.Decimals " & vbCrLf
                        cmdText &= " 	  ,CT.FormulaText " & vbCrLf
                        cmdText &= " 	  ,CT.ActiveRangeType " & vbCrLf
                        cmdText &= " 	  ,CT.EnableStatus " & vbCrLf
                        cmdText &= " 	  ,CT.PreloadedCalculatedTest " & vbCrLf
                        cmdText &= " 	  ,CT.BiosystemsID " & vbCrLf 'SGM 19/02/2013
                        cmdText &= " 	  ,TRR.RangeType " & vbCrLf
                        cmdText &= " 	  ,TRR.Gender " & vbCrLf
                        cmdText &= " 	  ,TRR.AgeUnit " & vbCrLf
                        cmdText &= " 	  ,TRR.AgeRangeFrom " & vbCrLf
                        cmdText &= " 	  ,TRR.AgeRangeTo " & vbCrLf
                        cmdText &= " 	  ,TRR.NormalLowerLimit " & vbCrLf
                        cmdText &= " 	  ,TRR.NormalUpperLimit " & vbCrLf
                        cmdText &= " 	  ,TRR.BorderLineLowerLimit " & vbCrLf
                        cmdText &= " 	  ,TRR.BorderLineUpperLimit " & vbCrLf
                        'cmdText &= " 	  ,FF.TestType " & vbCrLf
                        cmdText &= " FROM dbo.tparCalculatedTests ct " & vbCrLf
                        'cmdText &= "      LEFT JOIN  " & GlobalBase.TemporalDBName & ".dbo.tparFormulas FF on (FF.TestType = 'CALC' and FF.CalcTestID = CT.CalcTestID) " & vbCrLf
                        cmdText &= "      LEFT JOIN dbo.tparTestRefRanges TRR on (TRR.TestType = 'CALC' and TRR.TestID = ct.CalcTestID) " & vbCrLf

                        'cmdText &= " ORDER BY FF.TestType "

                        Dim compCalcTests As New DataSet
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(compCalcTests)
                            End Using
                        End Using

                        resultData.SetDatos = compCalcTests
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalculatedTestsUpdateDAO.GetPreloadedCALCTestsDistinctInClient", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets the list of all calculated Tests that in local DB appears as Preloaded and data don't match with Factory DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>
        ''' All the results are the calc test that can be removed or ignored
        ''' </returns>
        ''' <remarks>
        ''' Created by: JB - 29/01/2013
        ''' Modified by XB - 15/02/2013 - delete Ref Ranges into comparasion because would be usual have differences between Local and Factory DB (Bugs tracking #1134)
        ''' Modified by: SG - 19/02/2013 - Add new field BiosystemsID (Bugs tracking #1134)
        ''' </remarks>
        Public Shared Function GetPreloadedCALCTestsDistinctInFactory(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'CALC Tests
                        'All data in Ax00 DataBase different from data in Ax00_TEM
                        Dim cmdText As String = ""
                        cmdText &= "Select ct.CalcTestID " & vbCrLf
                        cmdText &= "   	  ,CT.CalcTestName " & vbCrLf
                        cmdText &= "   	  ,CT.CalcTestLongName " & vbCrLf
                        cmdText &= "   	  ,ct.MeasureUnit " & vbCrLf
                        cmdText &= " 	  ,ct.UniqueSampleType " & vbCrLf
                        cmdText &= " 	  ,ct.SampleType " & vbCrLf
                        cmdText &= " 	  ,ct.Decimals " & vbCrLf
                        cmdText &= " 	  ,ct.FormulaText " & vbCrLf
                        'cmdText &= " 	  ,ct.ActiveRangeType " & vbCrLf
                        cmdText &= " 	  ,ct.EnableStatus " & vbCrLf
                        cmdText &= " 	  ,ct.PreloadedCalculatedTest " & vbCrLf
                        cmdText &= " 	  ,CT.BiosystemsID " & vbCrLf 'SGM 19/02/2013
                        'cmdText &= " 	  ,trr.RangeType " & vbCrLf
                        'cmdText &= " 	  ,trr.Gender " & vbCrLf
                        'cmdText &= " 	  ,trr.AgeUnit " & vbCrLf
                        'cmdText &= " 	  ,trr.AgeRangeFrom " & vbCrLf
                        'cmdText &= " 	  ,trr.AgeRangeTo " & vbCrLf
                        'cmdText &= " 	  ,trr.NormalLowerLimit " & vbCrLf
                        'cmdText &= " 	  ,trr.NormalUpperLimit " & vbCrLf
                        'cmdText &= " 	  ,trr.BorderLineLowerLimit " & vbCrLf
                        'cmdText &= " 	  ,trr.BorderLineUpperLimit " & vbCrLf
                        cmdText &= "  FROM dbo.tparCalculatedTests ct " & vbCrLf
                        'cmdText &= "        LEFT JOIN dbo.tparTestRefRanges trr on trr.TestType = 'CALC' and trr.TestID=ct.CalcTestID " & vbCrLf
                        ''cmdText &= " WHERE ct.PreloadedCalculatedTest = 1 " & vbCrLf
                        cmdText &= "EXCEPT " & vbCrLf
                        cmdText &= "Select ct.CalcTestID " & vbCrLf
                        cmdText &= "   	  ,CT.CalcTestName " & vbCrLf
                        cmdText &= "   	  ,CT.CalcTestLongName " & vbCrLf
                        cmdText &= "   	  ,ct.MeasureUnit " & vbCrLf
                        cmdText &= " 	  ,ct.UniqueSampleType " & vbCrLf
                        cmdText &= " 	  ,ct.SampleType " & vbCrLf
                        cmdText &= " 	  ,ct.Decimals " & vbCrLf
                        cmdText &= " 	  ,ct.FormulaText " & vbCrLf
                        'cmdText &= " 	  ,ct.ActiveRangeType " & vbCrLf
                        cmdText &= " 	  ,ct.EnableStatus " & vbCrLf
                        cmdText &= " 	  ,ct.PreloadedCalculatedTest " & vbCrLf
                        cmdText &= " 	  ,CT.BiosystemsID " & vbCrLf 'SGM 19/02/2013
                        'cmdText &= " 	  ,trr.RangeType " & vbCrLf
                        'cmdText &= " 	  ,trr.Gender " & vbCrLf
                        'cmdText &= " 	  ,trr.AgeUnit " & vbCrLf
                        'cmdText &= " 	  ,trr.AgeRangeFrom " & vbCrLf
                        'cmdText &= " 	  ,trr.AgeRangeTo " & vbCrLf
                        'cmdText &= " 	  ,trr.NormalLowerLimit " & vbCrLf
                        'cmdText &= " 	  ,trr.NormalUpperLimit " & vbCrLf
                        'cmdText &= " 	  ,trr.BorderLineLowerLimit " & vbCrLf
                        'cmdText &= " 	  ,trr.BorderLineUpperLimit " & vbCrLf
                        cmdText &= " FROM " & GlobalBase.TemporalDBName & ".dbo.tparCalculatedTests ct " & vbCrLf
                        'cmdText &= "    LEFT JOIN " & GlobalBase.TemporalDBName & ".dbo.tparTestRefRanges trr on trr.TestType = 'CALC' and trr.TestID=ct.CalcTestID " & vbCrLf

                        Dim compCalcTests As New DataSet
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(compCalcTests)
                            End Using
                        End Using

                        resultData.SetDatos = compCalcTests
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalculatedTestsUpdateDAO.GetPreloadedCALCTestsDistinctInClient", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

    End Class

End Namespace
