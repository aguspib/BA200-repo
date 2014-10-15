Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class ISETestUpdateDAO
        Inherits DAOBase

        ''' <summary>
        ''' Gets the list of all ise tests that in local DB don't match with Factory DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>
        ''' All the results are the ise tests that can be removed or ignored
        ''' </returns>
        ''' <remarks>
        ''' Created by: DL - 29/01/2013
        ''' </remarks>
        Public Shared Function GetISEDistinctInFactory(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
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
                        cmdText &= "SELECT IT.ShortName" & vbCrLf
                        cmdText &= "      ,IT.Units" & vbCrLf
                        cmdText &= "      ,IT.ISE_Units" & vbCrLf
                        cmdText &= "      ,TRR.RangeType" & vbCrLf
                        cmdText &= "      ,TRR.Gender " & vbCrLf
                        cmdText &= "      ,TRR.AgeUnit " & vbCrLf
                        cmdText &= "      ,TRR.AgeRangeFrom " & vbCrLf
                        cmdText &= "      ,TRR.AgeRangeTo " & vbCrLf
                        cmdText &= "      ,TRR.NormalLowerLimit " & vbCrLf
                        cmdText &= "      ,TRR.NormalUpperLimit " & vbCrLf
                        cmdText &= "      ,TRR.BorderLineLowerLimit " & vbCrLf
                        cmdText &= "      ,TRR.BorderLineUpperLimit   " & vbCrLf
                        cmdText &= "  FROM tparISETests IT LEFT JOIN tparTestRefRanges TRR ON (TRR.TestType = 'ISE' and TRR.TestID = IT.ISETestID)" & vbCrLf
                        cmdText &= "EXCEPT" & vbCrLf
                        cmdText &= "SELECT IT.ShortName" & vbCrLf
                        cmdText &= "      ,IT.Units" & vbCrLf
                        cmdText &= "      ,IT.ISE_Units" & vbCrLf
                        cmdText &= "      ,TRR.RangeType" & vbCrLf
                        cmdText &= "      ,TRR.Gender " & vbCrLf
                        cmdText &= "      ,TRR.AgeUnit " & vbCrLf
                        cmdText &= "      ,TRR.AgeRangeFrom " & vbCrLf
                        cmdText &= "      ,TRR.AgeRangeTo " & vbCrLf
                        cmdText &= "      ,TRR.NormalLowerLimit " & vbCrLf
                        cmdText &= "      ,TRR.NormalUpperLimit " & vbCrLf
                        cmdText &= "      ,TRR.BorderLineLowerLimit " & vbCrLf
                        cmdText &= "      ,TRR.BorderLineUpperLimit   " & vbCrLf
                        cmdText &= "  FROM " & GlobalBase.TemporalDBName & ".dbo.tparISETests IT LEFT JOIN " & GlobalBase.TemporalDBName & ".dbo.tparTestRefRanges TRR ON (TRR.TestType = 'ISE' and TRR.TestID = IT.ISETestID)"

                        Dim myISETests As New ISETestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myISETests.tparISETests)
                            End Using
                        End Using

                        resultData.SetDatos = myISETests
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestUpdateDAO.GetISEDistinctInFactory", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Get differents in tparISETests between local and temporal Db
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <returns>True if find element otherwise it returns False</returns>
        ''' <remarks>
        ''' Created by:  DL 29/01/2013
        ''' Modified by: SG 14/02/2013 Bug #1134 - add ISETestID, ISE_ResultID and Name for the comparison
        ''' </remarks>
        Public Shared Function GetISETestDistinctInClient(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= "SELECT IT.ISETestID" & vbCrLf
                        cmdText &= "      ,IT.ISE_ResultID" & vbCrLf
                        cmdText &= "      ,IT.Name" & vbCrLf
                        cmdText &= "      ,IT.ShortName" & vbCrLf
                        cmdText &= "      ,IT.Units" & vbCrLf
                        cmdText &= "      ,IT.ISE_Units" & vbCrLf
                        cmdText &= "      ,IT.Enabled" & vbCrLf
                        cmdText &= "      ,TRR.RangeType" & vbCrLf
                        cmdText &= "      ,TRR.Gender " & vbCrLf
                        cmdText &= "      ,TRR.AgeUnit " & vbCrLf
                        cmdText &= "      ,TRR.AgeRangeFrom " & vbCrLf
                        cmdText &= "      ,TRR.AgeRangeTo " & vbCrLf
                        cmdText &= "      ,TRR.NormalLowerLimit " & vbCrLf
                        cmdText &= "      ,TRR.NormalUpperLimit " & vbCrLf
                        cmdText &= "      ,TRR.BorderLineLowerLimit " & vbCrLf
                        cmdText &= "      ,TRR.BorderLineUpperLimit   " & vbCrLf
                        cmdText &= "  FROM " & GlobalBase.TemporalDBName & ".dbo.tparISETests IT LEFT JOIN " & GlobalBase.TemporalDBName & ".dbo.tparTestRefRanges TRR ON (TRR.TestType = 'ISE' and TRR.TestID = IT.ISETestID)" & vbCrLf
                        cmdText &= "Except" & vbCrLf
                        cmdText &= "SELECT IT.ISETestID" & vbCrLf
                        cmdText &= "      ,IT.ISE_ResultID" & vbCrLf
                        cmdText &= "      ,IT.Name" & vbCrLf
                        cmdText &= "      ,IT.ShortName" & vbCrLf
                        cmdText &= "      ,IT.Units" & vbCrLf
                        cmdText &= "      ,IT.ISE_Units" & vbCrLf
                        cmdText &= "      ,IT.Enabled" & vbCrLf
                        cmdText &= "      ,TRR.RangeType" & vbCrLf
                        cmdText &= "      ,TRR.Gender " & vbCrLf
                        cmdText &= "      ,TRR.AgeUnit " & vbCrLf
                        cmdText &= "      ,TRR.AgeRangeFrom " & vbCrLf
                        cmdText &= "      ,TRR.AgeRangeTo " & vbCrLf
                        cmdText &= "      ,TRR.NormalLowerLimit " & vbCrLf
                        cmdText &= "      ,TRR.NormalUpperLimit " & vbCrLf
                        cmdText &= "      ,TRR.BorderLineLowerLimit " & vbCrLf
                        cmdText &= "      ,TRR.BorderLineUpperLimit   " & vbCrLf
                        cmdText &= "  FROM tparISETests IT LEFT JOIN tparTestRefRanges TRR ON (TRR.TestType = 'ISE' and TRR.TestID = IT.ISETestID)" & vbCrLf

                        Dim myISETests As New DataSet 'ISETestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myISETests) '(myISETests.tparISETests)
                            End Using
                        End Using

                        dataToReturn.SetDatos = myISETests
                        dataToReturn.HasError = False
                    End If
                End If



            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestUpdateDAO.GetISETestDistinctInClient", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function


        ''' <summary>
        ''' Get differents in tparISETests between local and temporal Db
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <returns>True if find element otherwise it returns False</returns>
        ''' <remarks>
        ''' Created by:  DL 29/01/2013
        ''' Modified by: XB 19/09/2014 - Add new fields TestLongName, SlopeFactorA2 and SlopeFactorB2 - BA-1865
        ''' </remarks>
        Public Shared Function GetISETestSamplesDistinctInClient(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText &= "SELECT ISETestID" & vbCrLf
                        cmdText &= "      ,SampleType" & vbCrLf
                        cmdText &= "      ,SampleType_ResultID" & vbCrLf
                        cmdText &= "      ,Decimals" & vbCrLf
                        cmdText &= "      ,ISE_Volume" & vbCrLf
                        cmdText &= "      ,ISE_DilutionFactor" & vbCrLf
                        cmdText &= "      ,ActiveRangeType" & vbCrLf
                        cmdText &= "      ,QCActive" & vbCrLf
                        cmdText &= "      ,ControlReplicates" & vbCrLf
                        cmdText &= "      ,NumberOfControls" & vbCrLf
                        cmdText &= "      ,RejectionCriteria" & vbCrLf
                        cmdText &= "      ,CalculationMode" & vbCrLf
                        cmdText &= "      ,NumberOfSeries" & vbCrLf
                        cmdText &= "      ,TotalAllowedError" & vbCrLf
                        cmdText &= "      ,TestLongName" & vbCrLf
                        cmdText &= "      ,SlopeFactorA2" & vbCrLf
                        cmdText &= "      ,SlopeFactorB2" & vbCrLf
                        cmdText &= "  FROM " & GlobalBase.TemporalDBName & ".dbo.tparISETestSamples" & vbCrLf
                        cmdText &= "Except" & vbCrLf
                        cmdText &= "SELECT ISETestID" & vbCrLf
                        cmdText &= "      ,SampleType" & vbCrLf
                        cmdText &= "      ,SampleType_ResultID" & vbCrLf
                        cmdText &= "      ,Decimals" & vbCrLf
                        cmdText &= "      ,ISE_Volume" & vbCrLf
                        cmdText &= "      ,ISE_DilutionFactor" & vbCrLf
                        cmdText &= "      ,ActiveRangeType" & vbCrLf
                        cmdText &= "      ,QCActive" & vbCrLf
                        cmdText &= "      ,ControlReplicates" & vbCrLf
                        cmdText &= "      ,NumberOfControls" & vbCrLf
                        cmdText &= "      ,RejectionCriteria" & vbCrLf
                        cmdText &= "      ,CalculationMode" & vbCrLf
                        cmdText &= "      ,NumberOfSeries" & vbCrLf
                        cmdText &= "      ,TotalAllowedError" & vbCrLf
                        cmdText &= "      ,TestLongName" & vbCrLf
                        cmdText &= "      ,SlopeFactorA2" & vbCrLf
                        cmdText &= "      ,SlopeFactorB2" & vbCrLf
                        cmdText &= "  FROM tparISETests"

                        Dim myISETests As New ISETestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myISETests.tparISETests)
                            End Using
                        End Using

                        dataToReturn.SetDatos = myISETests
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestUpdateDAO.GetISETestDistinctInClient", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function


#Region "FUNCTIONS FOR NEW UPDATE VERSION PROCESS"
        ''' <summary>
        ''' Search in FACTORY DB all changes in relevant fields of ISE Test/SampleTypes regarding values saved in CUSTOMER DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing an ISETestSamplesDS with all ISE Test/SampleTypes with values updated in FACTORY DB
        '''  </returns>
        ''' <remarks>
        ''' Created by: SA 15/10/2014 - BA-1944 (SubTask BA-2013)
        ''' </remarks>
        Public Function GetUpdatedFactoryISETestSamples(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT IT.ISETestID, IT.ISE_ResultID, IT.ISE_Units, ITS.SampleType, ITS.SampleType_ResultID, " & vbCrLf & _
                                                       " ITS.ISE_Volume, ITS.ISE_DilutionFactor " & vbCrLf & _
                                                " FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparISETests] IT " & vbCrLf & _
                                                " INNER JOIN " & GlobalBase.TemporalDBName & ".[dbo].[tparISETestSamples] ITS ON IT.ISETestID = ITS.ISETestID " & vbCrLf & _
                                                " EXCEPT " & vbCrLf & _
                                                " SELECT IT.ISETestID, IT.ISE_ResultID, IT.ISE_Units, ITS.SampleType, ITS.SampleType_ResultID, " & vbCrLf & _
                                                       " ITS.ISE_Volume, ITS.ISE_DilutionFactor " & vbCrLf & _
                                                " FROM [Ax00].[dbo].[tparISETests] IT INNER JOIN [Ax00].[dbo].[tparISETestSamples] ITS ON IT.ISETestID = ITS.ISETestID " & vbCrLf

                        Dim factoryISETestSamplesDS As New ISETestSamplesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(factoryISETestSamplesDS.tparISETestSamples)
                            End Using
                        End Using

                        resultData.SetDatos = factoryISETestSamplesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestUpdateDAO.GetUpdatedFactoryISETestSamples", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

    End Class

End Namespace