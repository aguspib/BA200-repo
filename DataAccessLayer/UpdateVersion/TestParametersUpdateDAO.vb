Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports System.Data.SqlClient

Namespace Biosystems.Ax00.DAL.DAO

    Public Class TestParametersUpdateDAO
        Inherits DAOBase

        ''' <summary>
        ''' Get all elements that are not equal or not in local db.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 31/01/2013</remarks>
        Public Function GetAffectedItemsFromFactory(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= "SELECT  tparTests.TestID, tparTests.TestName, tparTests.ShortName, tparTests.PreloadedTest, tparTests.AnalysisMode, tparTestSamples.SampleType, tparTests.MeasureUnit,  " & vbCrLf
                        cmdText &= "        tparTests.DecimalsAllowed, tparTests.ReplicatesNumber, tparTests.ReactionType, tparTests.ReadingMode, tparTests.MainWavelength, " & vbCrLf
                        cmdText &= "        tparTests.ReferenceWavelength, tparTestReagentsVolumes.ReagentNumber, tparTestReagentsVolumes.ReagentVolume, tparTestSamples.SampleVolume," & vbCrLf
                        cmdText &= "        tparTests.FirstReadingCycle, tparTests.SecondReadingCycle, tparTestSamples.PredilutionUseFlag, tparTestSamples.PredilutionMode, " & vbCrLf
                        cmdText &= "        tparTestSamples.PredilutionFactor, tparTestSamples.DiluentSolution, tparTestSamples.RedPostdilutionFactor, tparTestSamples.IncPostdilutionFactor, " & vbCrLf
                        cmdText &= "        tparTests.BlankMode, tparTestSamples.BlankAbsorbanceLimit, tparTests.KineticBlankLimit, tparTestSamples.LinearityLimit, tparTestSamples.DetectionLimit," & vbCrLf
                        cmdText &= "        tparTestSamples.FactorLowerLimit , tparTestSamples.FactorUpperLimit, tparTestSamples.SubstrateDepletionValue, " & vbCrLf
                        cmdText &= "        tparTestSamples.CalibrationFactor,tparTestSamples.SampleTypeAlternative, tparTestSamples.SlopeFactorA,tparTestSamples.SlopeFactorB, tparReagents.CodeTest "
                        cmdText &= "FROM    " & GlobalBase.TemporalDBName & ".[dbo].tparTests INNER JOIN" & vbCrLf
                        cmdText &= "               " & GlobalBase.TemporalDBName & ".[dbo].tparTestSamples ON tparTests.TestID = tparTestSamples.TestID INNER JOIN " & vbCrLf
                        cmdText &= "               " & GlobalBase.TemporalDBName & ".[dbo].tparTestReagentsVolumes ON tparTestSamples.TestID = tparTestReagentsVolumes.TestID AND " & vbCrLf
                        cmdText &= "               tparTestSamples.SampleType = tparTestReagentsVolumes.SampleType INNER JOIN" & vbCrLf
                        cmdText &= "        Ax00TEM.[dbo].tparReagents ON tparTestReagentsVolumes.ReagentID =  tparReagents.ReagentID" & vbCrLf
                        cmdText &= "EXCEPT " & vbCrLf
                        cmdText &= "SELECT  tparTests.TestID, tparTests.TestName, tparTests.ShortName, tparTests.PreloadedTest, tparTests.AnalysisMode, tparTestSamples.SampleType, tparTests.MeasureUnit,  " & vbCrLf
                        cmdText &= "        tparTests.DecimalsAllowed, tparTests.ReplicatesNumber, tparTests.ReactionType, tparTests.ReadingMode, tparTests.MainWavelength, " & vbCrLf
                        cmdText &= "        tparTests.ReferenceWavelength, tparTestReagentsVolumes.ReagentNumber, tparTestReagentsVolumes.ReagentVolume, tparTestSamples.SampleVolume," & vbCrLf
                        cmdText &= "        tparTests.FirstReadingCycle, tparTests.SecondReadingCycle, tparTestSamples.PredilutionUseFlag, tparTestSamples.PredilutionMode, " & vbCrLf
                        cmdText &= "        tparTestSamples.PredilutionFactor, tparTestSamples.DiluentSolution, tparTestSamples.RedPostdilutionFactor, tparTestSamples.IncPostdilutionFactor, " & vbCrLf
                        cmdText &= "        tparTests.BlankMode, tparTestSamples.BlankAbsorbanceLimit, tparTests.KineticBlankLimit, tparTestSamples.LinearityLimit, tparTestSamples.DetectionLimit," & vbCrLf
                        cmdText &= "        tparTestSamples.FactorLowerLimit , tparTestSamples.FactorUpperLimit, tparTestSamples.SubstrateDepletionValue, " & vbCrLf
                        cmdText &= "        tparTestSamples.CalibrationFactor,tparTestSamples.SampleTypeAlternative, tparTestSamples.SlopeFactorA,tparTestSamples.SlopeFactorB, tparReagents.CodeTest "
                        cmdText &= "FROM    [Ax00].[dbo].tparTests INNER JOIN" & vbCrLf
                        cmdText &= "               [Ax00].[dbo].tparTestSamples ON tparTests.TestID = tparTestSamples.TestID INNER JOIN " & vbCrLf
                        cmdText &= "               [Ax00].[dbo].tparTestReagentsVolumes ON tparTestSamples.TestID = tparTestReagentsVolumes.TestID AND " & vbCrLf
                        cmdText &= "               tparTestSamples.SampleType = tparTestReagentsVolumes.SampleType INNER JOIN " & vbCrLf
                        cmdText &= "        [Ax00].[dbo].tparReagents ON tparTestReagentsVolumes.ReagentID =  tparReagents.ReagentID " & vbCrLf
                        cmdText &= "" & vbCrLf
                        cmdText &= "" & vbCrLf
                        cmdText &= "" & vbCrLf

                        Dim resultData As New DataSet
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData)
                            End Using
                        End Using

                        dataToReturn.SetDatos = resultData
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestParametersUpdateDAO.GetAffectedItemsFromFactory", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get all affected Test to be remove from Client DB.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 06/02/2013</remarks>
        Public Function GetAffectedItemsFromFactoryRemoves(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText &= "SELECT  tparTests.TestID, tparTests.TestName, tparTests.ShortName, tparTests.PreloadedTest, tparTests.AnalysisMode, tparTestSamples.SampleType, tparTests.MeasureUnit,  " & vbCrLf
                        cmdText &= "        tparTests.DecimalsAllowed, tparTests.ReplicatesNumber, tparTests.ReactionType, tparTests.ReadingMode, tparTests.MainWavelength, " & vbCrLf
                        cmdText &= "        tparTests.ReferenceWavelength, tparTestReagentsVolumes.ReagentNumber, tparTestReagentsVolumes.ReagentVolume, tparTestSamples.SampleVolume," & vbCrLf
                        cmdText &= "        tparTests.FirstReadingCycle, tparTests.SecondReadingCycle, tparTestSamples.PredilutionUseFlag, tparTestSamples.PredilutionMode, " & vbCrLf
                        cmdText &= "        tparTestSamples.PredilutionFactor, tparTestSamples.DiluentSolution, tparTestSamples.RedPostdilutionFactor, tparTestSamples.IncPostdilutionFactor, " & vbCrLf
                        cmdText &= "        tparTests.BlankMode, tparTestSamples.BlankAbsorbanceLimit, tparTests.KineticBlankLimit, tparTestSamples.LinearityLimit, tparTestSamples.DetectionLimit," & vbCrLf
                        cmdText &= "        tparTestSamples.SlopeFactorA, tparTestSamples.SlopeFactorB, tparTestSamples.SubstrateDepletionValue, " & vbCrLf
                        cmdText &= "        tparTestSamples.CalibrationFactor,tparTestSamples.SampleTypeAlternative "
                        cmdText &= "FROM    [Ax00].[dbo].tparTests INNER JOIN" & vbCrLf
                        cmdText &= "               [Ax00].[dbo].tparTestSamples ON tparTests.TestID = tparTestSamples.TestID INNER JOIN " & vbCrLf
                        cmdText &= "               [Ax00].[dbo].tparTestReagentsVolumes ON tparTestSamples.TestID = tparTestReagentsVolumes.TestID AND " & vbCrLf
                        cmdText &= "               tparTestSamples.SampleType = tparTestReagentsVolumes.SampleType" & vbCrLf
                        cmdText &= "EXCEPT " & vbCrLf
                        cmdText &= "SELECT  tparTests.TestID, tparTests.TestName, tparTests.ShortName, tparTests.PreloadedTest, tparTests.AnalysisMode, tparTestSamples.SampleType, tparTests.MeasureUnit,  " & vbCrLf
                        cmdText &= "        tparTests.DecimalsAllowed, tparTests.ReplicatesNumber, tparTests.ReactionType, tparTests.ReadingMode, tparTests.MainWavelength, " & vbCrLf
                        cmdText &= "        tparTests.ReferenceWavelength, tparTestReagentsVolumes.ReagentNumber, tparTestReagentsVolumes.ReagentVolume, tparTestSamples.SampleVolume," & vbCrLf
                        cmdText &= "        tparTests.FirstReadingCycle, tparTests.SecondReadingCycle, tparTestSamples.PredilutionUseFlag, tparTestSamples.PredilutionMode, " & vbCrLf
                        cmdText &= "        tparTestSamples.PredilutionFactor, tparTestSamples.DiluentSolution, tparTestSamples.RedPostdilutionFactor, tparTestSamples.IncPostdilutionFactor, " & vbCrLf
                        cmdText &= "        tparTests.BlankMode, tparTestSamples.BlankAbsorbanceLimit, tparTests.KineticBlankLimit, tparTestSamples.LinearityLimit, tparTestSamples.DetectionLimit," & vbCrLf
                        cmdText &= "        tparTestSamples.SlopeFactorA, tparTestSamples.SlopeFactorB, tparTestSamples.SubstrateDepletionValue, " & vbCrLf
                        cmdText &= "        tparTestSamples.CalibrationFactor,tparTestSamples.SampleTypeAlternative "
                        cmdText &= "FROM    " & GlobalBase.TemporalDBName & ".[dbo].tparTests INNER JOIN" & vbCrLf
                        cmdText &= "               " & GlobalBase.TemporalDBName & ".[dbo].tparTestSamples ON tparTests.TestID = tparTestSamples.TestID INNER JOIN " & vbCrLf
                        cmdText &= "               " & GlobalBase.TemporalDBName & ".[dbo].tparTestReagentsVolumes ON tparTestSamples.TestID = tparTestReagentsVolumes.TestID AND " & vbCrLf
                        cmdText &= "               tparTestSamples.SampleType = tparTestReagentsVolumes.SampleType" & vbCrLf

                        Dim resultData As New DataSet
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData)
                            End Using
                        End Using

                        dataToReturn.SetDatos = resultData
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestParametersUpdateDAO.GetAffectedItemsFromFactory", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get the test on the factory DB.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestID">Test ID.</param>
        ''' <returns></returns>
        ''' <remarks>CREATE BY: TR 31/01/2013</remarks>
        Public Function GetDataInFactoryDB(ByVal pDBConnection As SqlClient.SqlConnection, pTestID As Integer) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= "SELECT  *"
                        cmdText &= "FROM   " & GlobalBase.TemporalDBName & ".[dbo].tparTests "
                        cmdText &= "WHERE TestID = " & pTestID

                        Dim resultData As New TestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.tparTests)
                            End Using
                        End Using

                        dataToReturn.SetDatos = resultData
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestParametersUpdateDAO.GetDataInFactoryDB", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestName"></param>
        ''' <param name="pSampleType"></param>
        ''' <returns></returns>
        ''' <remarks>Modify AG 13/03/2014 - #1538 fix issue when name contains char ' (use .Replace("'", "''"))</remarks>
        Public Function GetDataInFactoryDB(ByVal pDBConnection As SqlClient.SqlConnection, pTestName As String, Optional pSampleType As String = "") As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        If pSampleType = "" Then
                            cmdText &= "SELECT  *"
                            cmdText &= "FROM   " & GlobalBase.TemporalDBName & ".[dbo].tparTests "
                            cmdText &= "WHERE TestName = N'" & pTestName.Replace("'", "''") & "'"
                        Else
                            cmdText &= "SELECT T.*, TS.SampleType " & vbCrLf
                            cmdText &= "  FROM  " & GlobalBase.TemporalDBName & ".[dbo].tparTests T,  " & GlobalBase.TemporalDBName & ".[dbo].tparTestSamples TS " & vbCrLf
                            cmdText &= " WHERE T.TestName = N'" & pTestName.Replace("'", "''") & "' " & vbCrLf
                            cmdText &= "   AND TS.TestID = T.TestID" & vbCrLf

                            cmdText &= "   AND TS.SampleType = '" & pSampleType & "'"

                        End If

                        Dim resultData As New TestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.tparTests)
                            End Using
                        End Using

                        dataToReturn.SetDatos = resultData
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestParametersUpdateDAO.GetDataInFactoryDB", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get the TestSample information by the Test ID and the Sample Type on Factory Table.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestID">Test ID.</param>
        ''' <param name="pSampleType">Sample Type.</param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 31/01/2013</remarks>
        Public Function GetFactoryTestSampleByTestIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                                                    Optional pSampleType As String = "") As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = String.Empty

                        cmdText &= " SELECT * "
                        cmdText &= "FROM " & GlobalBase.TemporalDBName & ".[dbo].tparTestSamples "
                        cmdText &= " WHERE  TestID = " & pTestID
                        If Not pSampleType = "" Then
                            cmdText &= " AND    SampleType = '" & pSampleType & "' "
                        End If

                        Dim myTestSamplesDS As New TestSamplesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTestSamplesDS.tparTestSamples)
                            End Using
                        End Using

                        myGlobalDataTO.HasError = False
                        myGlobalDataTO.SetDatos = myTestSamplesDS
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparTestSamplesDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the Reagents volumens on the factory DB by the TestID and SampleType.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestID">Test ID</param>
        ''' <param name="pSampleType">Sample Type</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATE BY: TR 01/02/2013
        ''' </remarks>
        Public Function GetFactoryReagentsVolumesByTesIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                               ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TestID, ReagentID, ReagentNumber ,SampleType"
                        cmdText &= ", ReagentVolume, CONVERT(DECIMAL(20,12),ReagentVolumeSteps) AS ReagentVolumeSteps "
                        cmdText &= ", RedPostReagentVolume, CONVERT(DECIMAL(20,12), RedPostReagentVolumeSteps) AS RedPostReagentVolumeSteps"
                        cmdText &= ", IncPostReagentVolume, CONVERT(DECIMAL(20,12), IncPostReagentVolumeSteps) AS IncPostReagentVolumeSteps "
                        cmdText &= " FROM " & GlobalBase.TemporalDBName & ".[dbo].tparTestReagentsVolumes " & vbCrLf
                        cmdText &= " WHERE  TestID = " & pTestID.ToString & vbCrLf

                        If pSampleType <> "" Then
                            cmdText &= " AND SampleType = '" & pSampleType.ToString & "' "
                        End If

                        Dim resultData As New TestReagentsVolumesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.tparTestReagentsVolumes)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestParametersUpdateDAO.GetReagentsVolumesByTesIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Gete the TestReagents data from factory DB.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestID">Test ID</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 04/02/2013
        ''' </remarks>
        Public Function GetFactroyTestReagents(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT T.TestID, T.TestName, T.ReagentsNumber, T.BlankReplicates, " & vbCrLf & _
                                                       " TR.ReagentID, TR.ReagentNumber, R.ReagentName, R.PreloadedReagent " & vbCrLf & _
                                                " FROM " & GlobalBase.TemporalDBName & ".[dbo].tparTestReagents TR INNER JOIN " & GlobalBase.TemporalDBName & ".[dbo].tparReagents R ON TR.ReagentID = R.ReagentID " & vbCrLf & _
                                                                           " INNER JOIN " & GlobalBase.TemporalDBName & ".[dbo].tparTests T ON TR.TestID = T.TestID " & vbCrLf & _
                                                " WHERE  TR.TestID    = " & pTestID & vbCrLf & _
                                                " ORDER BY TR.ReagentNumber "

                        Dim resultData As New TestReagentsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.tparTestReagents)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparTestReagentsDAO.GetTestReagents", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the reagent data from Factory DB.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pReagentID">ReagentID</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 04/02/2013
        ''' </remarks>
        Public Function GetFactoryReagent(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim reagentsDataDS As New ReagentsDS
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= " SELECT *" & vbCrLf
                        cmdText &= " FROM  " & GlobalBase.TemporalDBName & ".[dbo].tparReagents " & vbCrLf
                        cmdText &= " WHERE  ReagentID = " & pReagentID.ToString

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(reagentsDataDS.tparReagents)

                        resultData.SetDatos = reagentsDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparReagentsDAO.GetFactoryReagent", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get Factory TestCalibrator value by TestID and sampleType
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestID">Test ID.</param>
        ''' <param name="pSampleType">Sample Type.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFactoryTestCalibratorByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                                            Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM " & GlobalBase.TemporalDBName & _
                                                ".[dbo].tparTestCalibrators TC INNER JOIN tparCalibrators C ON C.CalibratorID = TC.CalibratorID " & vbCrLf & _
                                                " WHERE  TestID = " & pTestID.ToString
                        If (pSampleType.Trim <> String.Empty) Then cmdText &= " AND SampleType ='" & pSampleType & "'"

                        Dim testCalibratorDS As New TestCalibratorsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(testCalibratorDS.tparTestCalibrators)
                            End Using
                        End Using

                        resultData.SetDatos = testCalibratorDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparTestCalibratorsDAO.GetTestCalibratorByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get Factory Calibrator data from Factory DB.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pCalibratorID">Calibrator ID.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 04/02/2013
        ''' </remarks>
        Public Function GetFactoryCalibrator(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibratorID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim cmdText As String = ""
                        cmdText &= "SELECT CalibratorID" & vbCrLf
                        cmdText &= "      ,CalibratorName" & vbCrLf
                        cmdText &= "      ,LotNumber" & vbCrLf
                        cmdText &= "      ,ExpirationDate" & vbCrLf
                        cmdText &= "      ,NumberOfCalibrators" & vbCrLf
                        cmdText &= "      ,InUse" & vbCrLf
                        cmdText &= "      ,SpecialCalib" & vbCrLf
                        cmdText &= "      ,TS_User" & vbCrLf
                        cmdText &= "      ,TS_DateTime" & vbCrLf
                        cmdText &= "  FROM " & GlobalBase.TemporalDBName & ".dbo.tparCalibrators" & vbCrLf
                        cmdText &= " WHERE CalibratorID = " & pCalibratorID.ToString

                        Dim calibratorsDataDS As New CalibratorsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(calibratorsDataDS.tparCalibrators)
                            End Using
                        End Using

                        resultData.SetDatos = calibratorsDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparCalibratorsDAO.GetFactoryCalibrator", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the factory test calibrator values.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestCalibratorID">Test Calibrator ID</param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 04/02/2013</remarks>
        Public Function GetFactoryTestCalibratorValues(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestCalibratorID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM " & GlobalBase.TemporalDBName & ".dbo.tparTestCalibratorValues " & vbCrLf & _
                                                " WHERE  TestCalibratorID = " & pTestCalibratorID & vbCrLf

                        Dim myTestCalibratorValueDS As New TestCalibratorValuesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTestCalibratorValueDS.tparTestCalibratorValues)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myTestCalibratorValueDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, " tparTestCalibratorsDAO.ReadByTestCalibratorID ", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

    End Class

End Namespace