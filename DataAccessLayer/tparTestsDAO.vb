Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports System.Text

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class tparTestsDAO
          


#Region "CRUD Methods"

        ''' <summary>
        ''' Create new Standard Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestsDS">Typed DataSet TestsDS with the list of Tests to add</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 24/02/2010
        ''' Modified by: SA 28/10/2010 - Added N preffix for multilanguage of field TS_User; when TS_User is not informed in the
        '''                              DS, then get the UserName of the current loggged User. Mandatory fields have to be informed,
        '''                              they do not allow NULL values
        '''              TR 09/05/2013 - Add new column LISValue used to indicate the mapping with list values.
        '''              AG 01/09/2014 - BA-1869 new column CustomPosition is informed!!
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestsDS As TestsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    Dim values As String = " "
                    Dim keys As String = "(TestID, TestName, ShortName, TestPosition, PreloadedTest, MeasureUnit, AnalysisMode," & _
                                         " ReagentsNumber, ReactionType, ReplicatesNumber, DecimalsAllowed, TurbidimetryFlag, " & _
                                         " AbsorbanceFlag, ReadingMode, FirstReadingCycle, SecondReadingCycle, MainWavelength, " & _
                                         " ReferenceWavelength, BlankMode, BlankReplicates, KineticBlankLimit, ProzoneRatio, " & _
                                         " ProzoneTime1, ProzoneTime2, InUse, TestVersionNumber, TestVersionDateTime, TS_User, " & _
                                         " TS_DateTime, LISValue, CustomPosition ) "

                    Dim cmd As New SqlCommand
                    cmd.Connection = pDBConnection

                    For Each tpartestsDR As TestsDS.tparTestsRow In pTestsDS.tparTests
                        values = ""
                        values &= tpartestsDR.TestID.ToString() & ","
                        values &= "N'" & tpartestsDR.TestName.ToString().Replace("'", "''").TrimEnd().TrimStart() & "',"
                        values &= "N'" & tpartestsDR.ShortName.ToString().Replace("'", "''").TrimEnd().TrimStart() & "',"
                        values &= tpartestsDR.TestPosition.ToString() & ","
                        values &= "'" & tpartestsDR.PreloadedTest.ToString().Replace("'", "''") & "',"
                        values &= "'" & tpartestsDR.MeasureUnit.ToString().Replace("'", "''") & "',"
                        values &= "'" & tpartestsDR.AnalysisMode.ToString().Replace("'", "''") & "',"
                        values &= tpartestsDR.ReagentsNumber.ToString() & " ,"
                        values &= "'" & tpartestsDR.ReactionType.ToString().Replace("'", "''") & "',"
                        values &= tpartestsDR.ReplicatesNumber.ToString() & ","
                        values &= tpartestsDR.DecimalsAllowed.ToString() & ","
                        values &= "'" & tpartestsDR.TurbidimetryFlag.ToString().Replace("'", "''") & "',"

                        If (tpartestsDR.IsAbsorbanceFlagNull) Then
                            values &= "0, "
                        Else
                            values &= "'" & tpartestsDR.AbsorbanceFlag.ToString() & "',"
                        End If

                        values &= "'" & tpartestsDR.ReadingMode.ToString().Replace("'", "''") & "',"
                        values &= "" & tpartestsDR.FirstReadingCycle.ToString() & ", "

                        If (tpartestsDR.IsSecondReadingCycleNull OrElse tpartestsDR.SecondReadingCycle = 0) Then
                            values &= "NULL,"
                        Else
                            values &= "" & tpartestsDR.SecondReadingCycle.ToString() & ", "
                        End If

                        values &= "'" & tpartestsDR.MainWavelength.ToString().Replace("'", "''") & "',"
                        If (tpartestsDR.IsReferenceWavelengthNull) Then
                            values &= "NULL,"
                        Else
                            values &= "'" & tpartestsDR.ReferenceWavelength.ToString().Replace("'", "''") & "',"
                        End If

                        If tpartestsDR.IsBlankModeNull Then
                            values &= "NULL,"
                        Else
                            values &= "'" & tpartestsDR.BlankMode.ToString().Replace("'", "''") & "',"
                        End If
                        If tpartestsDR.IsBlankReplicatesNull Then
                            values &= "NULL,"
                        Else
                            values &= tpartestsDR.BlankReplicates.ToString() & ","
                        End If
                        If tpartestsDR.IsKineticBlankLimitNull Then
                            values &= "NULL,"
                        Else
                            values &= ReplaceNumericString(tpartestsDR.KineticBlankLimit) & ","
                        End If
                        If tpartestsDR.IsProzoneRatioNull Then
                            values &= "NULL,"
                        Else
                            values &= ReplaceNumericString(tpartestsDR.ProzoneRatio) & ","
                        End If
                        If tpartestsDR.IsProzoneTime1Null Then
                            values &= "NULL,"
                        Else
                            values &= tpartestsDR.ProzoneTime1.ToString() & ","
                        End If
                        If tpartestsDR.IsProzoneTime2Null Then
                            values &= "NULL,"
                        Else
                            values &= tpartestsDR.ProzoneTime2.ToString() & ","
                        End If

                        If tpartestsDR.IsInUseNull Then
                            values &= "0, "
                        Else
                            values &= "'" & tpartestsDR.InUse.ToString().Replace("'", "''") & "',"
                        End If

                        If tpartestsDR.IsTestVersionNumberNull Then
                            values &= "0, "
                        Else
                            values &= tpartestsDR.TestVersionNumber.ToString() & ","
                        End If
                        If tpartestsDR.IsTestVersionDateTimeNull Then
                            values &= " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "', "
                        Else
                            values &= " '" & tpartestsDR.TestVersionDateTime.ToString("yyyyMMdd HH:mm:ss") & "', "
                        End If

                        If (tpartestsDR.IsTS_UserNull) Then
                            'Dim myGlobalbase As New GlobalBase
                            values &= " N'" & GlobalBase.GetSessionInfo.UserName.Replace("'", "''") & "', "
                        Else
                            values &= " N'" & tpartestsDR.TS_User.ToString().Replace("'", "''") & "',"
                        End If

                        If (tpartestsDR.IsTS_DateTimeNull) Then
                            values &= " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "', "
                        Else
                            values &= " '" & tpartestsDR.TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "', "
                        End If

                        If (tpartestsDR.IsLISValueNull) Then
                            values &= "NULL"
                        Else
                            values &= " N'" & tpartestsDR.LISValue & "' "
                        End If

                        'AG 01/09/2014 - BA-1869 - Inform the customPosition column
                        values &= " , " & tpartestsDR.CustomPosition.ToString()
                        'AG 01/09/2014 - BA-1869

                        cmdText = "INSERT INTO tparTests  " & keys & " VALUES (" & values & ")"
                        cmd.CommandText = cmdText

                        myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    Next
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update Standard Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestsDS">Typed DataSet TestsDS with the list of Tests to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 24/02/2010
        ''' Modified by: SA 28/10/2010 - Added N preffix for multilanguage of field TS_User; when TS_User is not informed in the
        '''                              DS, then get the UserName of the current loggged User. Mandatory fields have to be informed,
        '''                              they do not allow NULL values.
        '''              TR 09/05/2013 - Add new column LISValue used to indicate the mapping with list values.
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestsDS As TestsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    Dim values As String = " "

                    Dim cmd As New SqlCommand
                    cmd.Connection = pDBConnection

                    For Each tpartestsDR As TestsDS.tparTestsRow In pTestsDS.tparTests
                        values = ""
                        values &= " TestID = " & tpartestsDR.TestID.ToString() & ", "
                        values &= " TestName = N'" & tpartestsDR.TestName.ToString().Replace("'", "''").TrimEnd().TrimStart() & "', "
                        values &= " ShortName = N'" & tpartestsDR.ShortName.ToString().Replace("'", "''").TrimEnd().TrimStart() & "', "
                        values &= " TestPosition = " & tpartestsDR.TestPosition.ToString() & ", "
                        values &= " MeasureUnit = '" & tpartestsDR.MeasureUnit.ToString().Replace("'", "''") & "', "
                        values &= " AnalysisMode = '" & tpartestsDR.AnalysisMode.ToString().Replace("'", "''") & "', "
                        values &= " ReagentsNumber = " & tpartestsDR.ReagentsNumber.ToString() & ", "
                        values &= " ReactionType = '" & tpartestsDR.ReactionType.ToString().Replace("'", "''") & "', "
                        values &= " ReplicatesNumber = " & tpartestsDR.ReplicatesNumber.ToString() & ", "
                        values &= " DecimalsAllowed = " & tpartestsDR.DecimalsAllowed.ToString() & ", "
                        values &= " TurbidimetryFlag = '" & tpartestsDR.TurbidimetryFlag.ToString() & "', "

                        values &= " AbsorbanceFlag = "
                        If tpartestsDR.IsAbsorbanceFlagNull Then
                            values &= " 0,"
                        Else
                            values &= " '" & tpartestsDR.AbsorbanceFlag.ToString().Replace("'", "''") & "', "
                        End If

                        values &= " ReadingMode = '" & tpartestsDR.ReadingMode.ToString().Replace("'", "''") & "', "
                        values &= " FirstReadingCycle = " & tpartestsDR.FirstReadingCycle.ToString() & ", "

                        values &= " SecondReadingCycle = "
                        If tpartestsDR.IsSecondReadingCycleNull Then
                            values &= " NULL, "
                        Else
                            values &= tpartestsDR.SecondReadingCycle.ToString() & ", "
                        End If

                        values &= " MainWavelength = '" & tpartestsDR.MainWavelength.ToString().Replace("'", "''") & "', "

                        values &= " ReferenceWavelength = "
                        If tpartestsDR.IsReferenceWavelengthNull Then
                            values &= " NULL, "
                        Else
                            values &= " '" & tpartestsDR.ReferenceWavelength.ToString().Replace("'", "''") & "', "
                        End If

                        values &= " BlankMode = "
                        If tpartestsDR.IsBlankModeNull Then
                            values &= " NULL, "
                        Else
                            values &= " '" & tpartestsDR.BlankMode.ToString().Replace("'", "''") & "', "
                        End If

                        values &= " BlankReplicates = "
                        If tpartestsDR.IsBlankReplicatesNull Then
                            values &= " NULL, "
                        Else
                            values &= tpartestsDR.BlankReplicates.ToString() & ", "
                        End If

                        values &= " KineticBlankLimit = "
                        If tpartestsDR.IsKineticBlankLimitNull Then
                            values &= " NULL, "
                        Else
                            values &= ReplaceNumericString(tpartestsDR.KineticBlankLimit) & ", "
                        End If

                        values &= " ProzoneRatio = "
                        If tpartestsDR.IsProzoneRatioNull Then
                            values &= " NULL, "
                        Else
                            values &= ReplaceNumericString(tpartestsDR.ProzoneRatio) & ", "
                        End If

                        values &= " ProzoneTime1 = "
                        If tpartestsDR.IsProzoneTime1Null Then
                            values &= " NULL, "
                        Else
                            values &= tpartestsDR.ProzoneTime1.ToString() & ", "
                        End If

                        values &= " ProzoneTime2 = "
                        If tpartestsDR.IsProzoneTime2Null Then
                            values &= " NULL, "
                        Else
                            values &= tpartestsDR.ProzoneTime2.ToString() & ", "
                        End If

                        values &= " LISValue = N'"
                        If tpartestsDR.IsLISValueNull Then
                            values &= " NULL, "
                        Else
                            values &= tpartestsDR.LISValue.ToString() & "', "
                        End If

                        If Not tpartestsDR.IsTestVersionNumberNull Then
                            values &= " TestVersionNumber = " & tpartestsDR.TestVersionNumber.ToString() & ","
                        End If
                        If Not tpartestsDR.IsTestVersionDateTimeNull Then
                            values &= " TestVersionDateTime = '" & tpartestsDR.TestVersionDateTime.ToString("yyyyMMdd HH:mm:ss") & "', "
                        End If

                        If tpartestsDR.IsTS_UserNull Then
                            'Dim myGlobalbase As New GlobalBase
                            values &= " TS_User = N'" & GlobalBase.GetSessionInfo.UserName.Replace("'", "''") & "', "
                        Else
                            values &= " TS_User = N'" & tpartestsDR.TS_User.ToString().Replace("'", "''") & "', "
                        End If

                        If tpartestsDR.IsTS_DateTimeNull Then
                            values &= " TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' "
                        Else
                            values &= " TS_DateTime = '" & tpartestsDR.TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "' "
                        End If

                        cmdText = " UPDATE tparTests SET " & values & _
                                  " WHERE  TestID = " & tpartestsDR.TestID.ToString()

                        cmd.CommandText = cmdText
                        myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                    Next
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete the specified Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 16/03/2010
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    Dim cmd As New SqlCommand

                    cmdText &= " DELETE FROM  tparTests "
                    cmdText &= " WHERE TestID = " & pTestID

                    cmd.CommandText = cmdText
                    cmd.Connection = pDBConnection

                    myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                    myGlobalDataTO.HasError = False
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all basic parameters of the specified Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestsDS containing all basic parameters 
        '''          of the informed Test</returns>
        ''' <remarks>
        ''' Created by:  SA 09/02/2010
        ''' Modified by: DL 19/02/2010
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tparTests " & vbCrLf & _
                                                " WHERE  TestID = " & pTestID

                        Dim myTestDataDS As New TestsDS()
                        Using cmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(cmd)
                                da.Fill(myTestDataDS.tparTests)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myTestDataDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read all information needed for Biochemical calculations for an Standard Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Identifier of an Standard Test</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestsDS with all data needed for Biochemical calculations</returns>
        ''' <remarks>
        ''' Created by:  AG 03/07/2012
        ''' Modified by: SA 12/07/2012 - Changed the query to get also fields ActiveRangeType, CalibratorType, CalibrationFactor and
        '''                              SampleTypeAlternative from table tparTestSamples
        ''' </remarks>
        Public Function ReadForCalculations(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT T.TestVersionNumber, T.AnalysisMode, T.AbsorbanceFlag, T.ReactionType, T.ReadingMode, T.ReagentsNumber, " & vbCrLf & _
                                                       " T.FirstReadingCycle, T.SecondReadingCycle, T.MainWavelength, T.ReferenceWavelength, T.KineticBlankLimit, " & vbCrLf & _
                                                       " T.ProzoneTime1, T.ProzoneTime2, T.ProzoneRatio, T.BlankMode, V.ReagentVolume, V.IncPostReagentVolume, " & vbCrLf & _
                                                       " V.RedPostReagentVolume, V.ReagentNumber, S.SampleVolume, S.AbsorbanceDilutionFactor, S.PredilutionUseFlag, " & vbCrLf & _
                                                       " S.PredilutionFactor, S.BlankAbsorbanceLimit, S.IncPostdilutionFactor, S.IncPostSampleVolume, S.RedPostdilutionFactor, " & vbCrLf & _
                                                       " S.RedPostSampleVolume, S.LinearityLimit, S.DetectionLimit, S.SubstrateDepletionValue, S.SlopeFactorA, S.SlopeFactorB, " & vbCrLf & _
                                                       " S.FactorUpperLimit, S.FactorLowerLimit, S.ActiveRangeType, S.CalibratorType, S.CalibrationFactor, " & vbCrLf & _
                                                       " S.SampleTypeAlternative " & vbCrLf & _
                                                " FROM   tparTests T INNER JOIN tparTestSamples S ON T.TestID = S.TestID " & vbCrLf & _
                                                                   " INNER JOIN tparTestReagentsVolumes V ON S.TestID = V.TestID AND S.SampleType = V.SampleType " & vbCrLf & _
                                                " WHERE  T.TestID = " & pTestID.ToString & vbCrLf & _
                                                " AND    S.SampleType = '" & pSampleType.Trim & "' " & vbCrLf & _
                                                " ORDER BY V.ReagentNumber "

                        Dim myTestDataDS As New TestsDS()
                        Using cmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(cmd)
                                da.Fill(myTestDataDS.testCalculations)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myTestDataDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get information of the specified Test and the list of Sample Types linked to it. Used in the Update Version process
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code (optional parameter)</param>
        ''' <returns>GlobalDataTO containing a TestSamplesDS with Test data for all Sample Types defined for the informed Test or, if parameter 
        '''          pSampleType is informed, only for the specific Sample Type</returns>
        ''' <remarks>
        ''' Created by:  TR 09/12/2010
        ''' Modified by: SA 08/10/2014 - BA-1944 (SubTask BA-1983) ==> Parameter pSampleType changed to optional. Changed the SQL to use 
        '''                                                            INNER JOIN. Implement USING to execute the query
        ''' </remarks>
        Public Function ReadByTestIDSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                               Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT T.*, TS.SampleType " & vbCrLf & _
                                                " FROM   tparTests T INNER JOIN tparTestSamples TS ON T.TestID = TS.TestID " & vbCrLf & _
                                                " WHERE  T.TestID = " & pTestID.ToString

                        'When informed, add a filter by SampleType
                        If (pSampleType.Trim <> String.Empty) Then cmdText &= " AND TS.SampleType = '" & pSampleType.Trim & "' "

                        Dim myTestsDS As New TestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTestsDS.tparTests)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myTestsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.ReadByTestIDSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get data of a Standard Test searching by Test Short Name
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pShortName">Test Short Name</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestsDS containing all basic parameters 
        '''          of the informed Test</returns>
        ''' <remarks>
        ''' Created by:  TR 16/11/2010
        ''' Modified by: SA 21/12/2011 - Added N preffix for multilanguage of filter ShortName
        '''              XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        ''' </remarks>
        Public Function ReadByShortName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pShortName As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tparTests " & vbCrLf & _
                                                " WHERE  UPPER(ShortName) = UPPER(N'" & pShortName.Replace("'", "''") & "')"
                        '" WHERE  UPPER(ShortName) = N'" & pShortName.Replace("'", "''").ToUpper & "'"

                        Dim myTestDataDS As New TestsDS()
                        Using cmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(cmd)
                                da.Fill(myTestDataDS.tparTests)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myTestDataDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, " tparTestsDAO.ReadByShortName ", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all Standard Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCustomizedTestSelection">Default FALSE same order as until v3.0.2. When TRUE the test are filtered by Available and order by CustomPosition ASC</param>
        ''' <returns>GlobalDataTO containing all Standard Tests sorted by position</returns>
        ''' <remarks>
        ''' Created by:  TR 08/02/2010
        ''' AG 29/08/2014 BA-1869 EUA can customize the test selection visibility and order in test keyboard auxiliary screen
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCustomizedTestSelection As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = String.Empty
                        If Not pCustomizedTestSelection Then 'AG 29/08/2014 BA-1869 Old query
                            cmdText = " SELECT * FROM tparTests " & vbCrLf & _
                                                    " ORDER BY TestPosition "

                        Else 'AG 29/08/2014 BA-1869 New query
                            cmdText = " SELECT * FROM tparTests WHERE Available = 1 " & vbCrLf & _
                                                    " ORDER BY CustomPosition "
                        End If

                        Dim myTestDataDS As New TestsDS()
                        Using cmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(cmd)
                                da.Fill(myTestDataDS.tparTests)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myTestDataDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Search test data for the informed Test Name
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestName">Test Name to search by</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestsDS containing data of the 
        '''          informed Test</returns>
        ''' <remarks>
        ''' Created by:  SA 13/09/2010
        ''' Modified by: SA 28/10/2010 - Added N preffix for field TestName when use it in the where clause
        '''              XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        ''' </remarks>
        Public Function ReadByTestName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestName As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tparTests " & _
                                                " WHERE  UPPER(TestName) = UPPER(N'" & pTestName.Replace("'", "''") & "') "
                        '" WHERE  UPPER(TestName) = N'" & pTestName.Replace("'", "''").ToUpper & "' "

                        Dim myTestDataDS As New TestsDS()
                        Using cmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(cmd)
                                da.Fill(myTestDataDS.tparTests)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myTestDataDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.ReadByTestName", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Special method that update only the test position.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pListTestPositionTO">List of TestID and TestPosition.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  TR 29/03/2010
        ''' </remarks>
        Public Function UpdateTestPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pListTestPositionTO As List(Of TestPositionTO)) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    Dim values As String = " "

                    Dim cmd As New SqlCommand
                    cmd.Connection = pDBConnection

                    For Each myTestPosTO As TestPositionTO In pListTestPositionTO
                        values = ""
                        values &= " TestPosition = "
                        values &= myTestPosTO.TestPosition

                        cmdText = " UPDATE tparTests SET " & values & _
                                  " WHERE  TestID = " & myTestPosTO.TestID.ToString()

                        cmd.CommandText = cmdText
                        myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                    Next
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.UpdateTestPosition", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Set value of flag InUse for all Standard Tests added/removed from the Active WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pFlag">Value of the InUse Flag to set</param>
        ''' <param name="pUpdateForExcluded">Optional parameter. When True, it means the InUse will be set to False
        '''                                  only for Standard Tests that have been excluded from the active WorkSession</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 10/05/2010 
        ''' Modified by: SA  09/06/2010 - Added new optional parameter to reuse this method to set InUse=False for Standard Tests
        '''                               that have been excluded from the active WorkSession  
        ''' </remarks>
        Public Function UpdateInUseFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                        ByVal pAnalyzerID As String, ByVal pFlag As Boolean, _
                                        Optional ByVal pUpdateForExcluded As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String
                    If (Not pUpdateForExcluded) Then
                        cmdText = " UPDATE tparTests " & vbCrLf & _
                                  " SET    InUse = " & Convert.ToInt32(IIf(pFlag, 1, 0)) & vbCrLf & _
                                  " WHERE  TestID IN (SELECT DISTINCT TestID " & vbCrLf & _
                                                    " FROM   vwksWSOrderTests " & vbCrLf & _
                                                    " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                    " AND    AnalyzerID    = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                    " AND    TestType      = 'STD')" & vbCrLf

                    Else
                        cmdText = " UPDATE tparTests " & vbCrLf & _
                                  " SET    InUse = 0 " & vbCrLf & _
                                  " WHERE  TestID NOT IN (SELECT DISTINCT TestID " & vbCrLf & _
                                                        " FROM   vwksWSOrderTests " & vbCrLf & _
                                                        " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                        " AND    AnalyzerID    = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                        " AND    TestType      = 'STD') " & vbCrLf & _
                                  " AND    InUse = 1 " & vbCrLf
                    End If

                    Dim cmd As New SqlCommand
                    cmd.Connection = pDBConnection
                    cmd.CommandText = cmdText

                    myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.UpdateInUseFlag", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the field InUse by TestID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestID"></param>
        ''' <param name="pInUseFlag"></param>
        ''' <returns></returns>
        ''' <remarks>AG 08/05/2013</remarks>
        Public Function UpdateInUseByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pInUseFlag As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = " UPDATE tparTests " & vbCrLf & _
                              " SET    InUse = " & Convert.ToInt32(IIf(pInUseFlag, 1, 0)) & vbCrLf & _
                              " WHERE  TestID = " & pTestID.ToString

                    Dim cmd As New SqlCommand
                    cmd.Connection = pDBConnection
                    cmd.CommandText = cmdText

                    myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.UpdateInUseByTestID", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the LISValue by the testID.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestID">Test ID.</param>
        ''' <param name="pLISValue">LIS Value.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 04/03/2013
        ''' </remarks>
        Public Function UpdateLISValueByTestID(ByVal pDBConnection As SqlClient.SqlConnection, pTestID As Integer, pLISValue As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String
                    cmdText = " UPDATE tparTests " & Environment.NewLine & _
                              " SET    LISValue = N'" & pLISValue & "'" & Environment.NewLine & _
                              " WHERE  TestID = " & pTestID

                    Dim cmd As New SqlCommand
                    cmd.Connection = pDBConnection
                    cmd.CommandText = cmdText

                    myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.UpdateLISValueByTestID", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Get all Standard Preloaded Tests order by TestName
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing all Preloaded Standard Tests sorted by TestName</returns>
        ''' <remarks>
        ''' Created by:  XB 04/06/2014 - BT #1646
        ''' </remarks>
        Public Function ReadPreloadedTestByTestName(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tparTests " & vbCrLf & _
                                                " WHERE PreloadedTest = 1 " & vbCrLf & _
                                                " ORDER BY TestName ASC "

                        Dim myTestDataDS As New TestsDS()
                        Using cmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(cmd)
                                da.Fill(myTestDataDS.tparTests)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myTestDataDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.ReadPreloadedTestByTestName", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Get all Standard Tests by PreloadedTest
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPreloadedTest">Preloaded Test (1) or User Test (0)</param>
        ''' <returns>GlobalDataTO containing all User Standard Tests</returns>
        ''' <remarks>
        ''' Created by:  XB 04/06/2014 - BT #1646
        ''' </remarks>
        Public Function ReadByPreloadedTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPreloadedTest As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tparTests " & vbCrLf & _
                                                " WHERE  PreloadedTest = " & pPreloadedTest

                        Dim myTestDataDS As New TestsDS()
                        Using cmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(cmd)
                                da.Fill(myTestDataDS.tparTests)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myTestDataDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.ReadByPreloadedTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Get maximum Test position Standard Preloaded Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing all Standard Tests sorted by position</returns>
        ''' <remarks>
        ''' Created by:  XB 04/06/2014 - BT #1646
        ''' </remarks>
        Public Function GetLastPreloadedTestPosition(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT MAX(TestPosition) FROM tparTests " & vbCrLf & _
                                                " WHERE PreloadedTest = 1 "


                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar()
                        End Using

                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.GetLastPreloadedTestPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Get all Standard Biosystems Tests (PreloadedTest = TRUE) or all Standard User Tests (PreloadedTest = FALSE),
        ''' depending on value of parameter pPreloadedTest
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPreloadedTest">True to get Biosystems Tests; False to get User Tests</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestsDS with the list of obtained Tests</returns>
        ''' <remarks>
        ''' Created by: TR 23/03/2011
        ''' </remarks>
        Public Function ReadByPreloadedTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPreloadedTest As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tparTests " & vbCrLf & _
                                                " WHERE PreloadedTest = " & Convert.ToInt32(IIf(pPreloadedTest, 1, 0)) & vbCrLf

                        Dim myTestDataDS As New TestsDS()
                        Using cmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(cmd)
                                da.Fill(myTestDataDS.tparTests)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myTestDataDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.ReadByPreloadedTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get information of all Standard Tests/SampleTypes using Quality Control (those with QCActive=True)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <remarks>
        ''' Created by:  DL 06/04/2011
        ''' Modified by: SA 10/05/2011 - Removed field ControlID from the SQL
        ''' </remarks>
        Public Function GetAllWithQCActive(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT T.TestID, T.ShortName, T.TestName, T.DecimalsAllowed, T.PreloadedTest, T.TestPosition, T.InUse, " & vbCrLf & _
                                                       " TS.SampleType, TS.RejectionCriteria, TS.QCActive, TS.FactoryCalib, 0 AS ActiveControl, " & vbCrLf & _
                                                       " MD.FixedItemDesc As MeasureUnit " & vbCrLf & _
                                                " FROM   tparTests T INNER JOIN tparTestSamples TS ON T.TestID = TS.TestID " & vbCrLf & _
                                                                   " INNER JOIN tcfgMasterData MD  ON T.MeasureUnit = MD.ItemID " & vbCrLf & _
                                                " WHERE  TS.QCActive = 1 " & vbCrLf & _
                                                " AND    MD.SubTableID = '" & MasterDataEnum.TEST_UNITS.ToString & "' " & vbCrLf & _
                                                " ORDER BY TS.SampleType, T.TestPosition "

                        Dim myTestDataDS As New TestsDS()
                        Using cmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(cmd)
                                da.Fill(myTestDataDS.tparTests)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myTestDataDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.GetAllWithQCActive", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all Standard Tests/SampleTypes currently linked to the specified Control 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <remarks>
        ''' Created by:  DL 06/04/2011
        ''' Modified by: SA 10/05/2011 - Removed field ControlID from the SQL
        '''              SA 18/06/2012 - Filter Test Controls by TestType = STD
        ''' </remarks>
        Public Function GetAllByControl(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT T.TestID, T.ShortName, T.TestName, T.DecimalsAllowed, T.PreloadedTest, T.TestPosition, T.InUse, " & vbCrLf & _
                                                       " TS.SampleType, TS.RejectionCriteria, TS.QCActive, TS.FactoryCalib, TC.ActiveControl, " & vbCrLf & _
                                                       " MD.FixedItemDesc As MeasureUnit " & vbCrLf & _
                                                " FROM   tparTests T INNER JOIN tparTestSamples TS  ON T.TestID = TS.TestID " & vbCrLf & _
                                                                   " INNER JOIN tparTestControls TC ON TS.TestID = TC.TestID AND TS.SampleType = TC.SampleType " & vbCrLf & _
                                                                   " INNER JOIN tcfgMasterData MD   ON T.MeasureUnit = MD.ItemID " & vbCrLf & _
                                                " WHERE  TC.ControlID  = " & pControlID & vbCrLf & _
                                                " AND    TC.TestType   = 'STD' " & vbCrLf & _
                                                " AND    MD.SubTableID = '" & MasterDataEnum.TEST_UNITS.ToString & "' " & vbCrLf & _
                                                " ORDER BY TS.SampleType, T.TestPosition "

                        Dim myTestDataDS As New TestsDS()
                        Using cmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(cmd)
                                da.Fill(myTestDataDS.tparTests)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myTestDataDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.GetAllByControl", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of all Standard Tests for the specified Sample Type. When a SampleClass is informed, then 
        ''' only Test/SampleType with Controls or Experimental Calibrators defined are returned 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleTypeCode">Sample Type to filter the Tests</param>
        ''' <param name="pSampleClass">Optional parameter. When informed, get only Test/SampleType for which Controls or Calibrators
        '''                            have been defined</param>
        ''' <param name="pCustomizedTestSelection">Default FALSE same order as until v3.0.2. When TRUE the test are filtered by Available and order by CustomPosition ASC</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestsDS with the list of obtained Tests</returns>
        ''' <remarks>
        ''' Modified by: TR 05/02/2010 - Add more functionality, to validate the controls sample type code.
        '''              TR 08/02/2010 - Change the method sing and add the pSampleClass parameter to add more functionality
        '''                              And validate the sampleClass to change the select value.
        '''              SA 16/02/2010 - Changed query filter when pSampleClass = CALIB: only Test/SampleType using Experimental
        '''                              Calibrators or Alternative Calibrators based in an Experimental one should be returned
        '''              SA 19/05/2010 - Changed query to get also field ShortName
        '''              DL 14/10/2010 - Added new optional parameter for the Test Type and modified the SQL to apply this new filter
        '''              SA 18/10/2010 - Changed the SQL for Calculated Tests, to include also the management of Calculated Tests with
        '''                              multiple Sample Types
        '''              SA 22/10/2010 - Remove the last two changes applied: this function is used only to load the available STD Tests
        '''                              filtered by SampleType for the informed SampleClass. Removed also optional parameter for TestProfileID
        '''                              due to this function is not called from the screen of Programming Test Profiles anymore. Name changed 
        '''                              to GetBySampleType
        '''              TR 10/03/2011 - Add the factory Calib on the select (TS.FactoryCalib)
        '''              SA 27/04/2011 - Changed query filter when SampleClass is CTRL
        '''              SA 21/06/2011 - Added new filter when SampleClass is CTRL: the Test/SampleType has to have at least an ActiveControl
        '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
        '''              TR 29/04/2014 - BT #1494 Adde the EnableStatus column, use to indicate if the Test is complete or incomplete programming.
        '''              AG 29/08/2014 BA-1869 EUA can customize the test selection visibility and order in test keyboard auxiliary screen (define the last 2 parameters as required)
        ''' </remarks>
        Public Function GetBySampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleTypeCode As String, _
                                         ByVal pSampleClass As String, ByVal pCustomizedTestSelection As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT T.TestID AS TestID, T.ShortName, T.TestName, T.TestPosition, T.PreloadedTest, " & vbCrLf & _
                                                       " TS.NumberOfControls, TS.FactoryCalib, TS.EnableStatus " & vbCrLf & _
                                                " FROM   tparTests T INNER JOIN tparTestSamples TS ON T.TestID = TS.TestID " & vbCrLf & _
                                                " WHERE  UPPER(TS.SampleType) = '" & pSampleTypeCode & "' " & vbCrLf

                        Select Case (pSampleClass)
                            Case "CTRL"
                                cmdText &= " AND TS.QCActive = 1 and TS.NumberOfControls > 0 " & vbCrLf & _
                                           " AND (SELECT COUNT(*) FROM tparTestControls TC " & vbCrLf & _
                                                " WHERE TC.TestID = TS.TestID AND TC.SampleType = TS.SampleType AND TC.ActiveControl = 1) > 0 " & vbCrLf
                                Exit Select
                            Case "CALIB"
                                cmdText &= " AND (TS.CalibratorType = 'EXPERIMENT' " & vbCrLf & _
                                           " OR  (TS.CalibratorType = 'ALTERNATIV' AND (SELECT CalibratorType FROM tparTestSamples TS2 " & vbCrLf & _
                                                                                      " WHERE  TS2.TestID     = TS.TestID " & vbCrLf & _
                                                                                      " AND    TS2.SampleType = TS.SampleTypeAlternative) = 'EXPERIMENT')) " & vbCrLf
                                Exit Select
                        End Select

                        'AG 29/08/2014 BA-1869
                        If Not pCustomizedTestSelection Then 'Old order
                            cmdText &= " ORDER BY T.TestPosition "
                        Else 'New conditions
                            cmdText &= " AND T.Available = 1  ORDER BY T.CustomPosition "
                        End If
                        'AG 29/08/2014 BA-1869


                        Dim myTestDataDS As New TestsDS()
                        Using cmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(cmd)
                                da.Fill(myTestDataDS.tparTests)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myTestDataDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.GetBySampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the last created Test Identifier for User's defined Standard Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pFirstTestID">First value allowed for ID of User's defined Standard Tests</param>
        ''' <param name="pIsPreloaded">Indicate if the new test is preloaded or not.</param>
        ''' <returns>GlobalDataTO containing an integer value</returns>
        ''' <remarks>
        ''' Created by: TR 03/03/2010
        ''' MODIFIED BY: TR 05/02/2013 -Add the optional parameter IsPreloaded to get the Test Id for preloaded test.
        ''' </remarks>
        Public Function GetLastTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pFirstTestID As Integer, _
                                                                Optional pIsPreloaded As Boolean = False) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO()
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT MAX(TestID) FROM tparTests WHERE TestID >= " & pFirstTestID
                        'If preloaded the filter by PreloadedTest.
                        If pIsPreloaded Then
                            cmdText &= " AND PreloadedTest = 1"
                        End If

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar()
                        End Using
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.GetLastTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the last Test Position
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing an integer value</returns>
        ''' <remarks>
        ''' Created by: TR 03/03/2010
        ''' </remarks>
        Public Function GetLastTestPosition(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT MAX(TestPosition) FROM tparTests "

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar()
                        End Using
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.GetLastTestPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the last Custom Position
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing an integer value</returns>
        ''' <remarks>
        ''' Created by: AG 01/09/2014 - BA-1869
        ''' </remarks>
        Public Function GetLastCustomPosition(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT MAX(CustomPosition) FROM tparTests "

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar()
                        End Using
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.GetLastCustomPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Get all not InUse Tests not linked to the specified Calibrator, that is, all Tests that can be
        ''' linked to it 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalibratorID">Calibrator Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestsDS with the list of Tests</returns>
        ''' <remarks>
        ''' Created by: TR 04/06/2010
        ''' </remarks>
        Public Function GetTestToSetCalibrator(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibratorID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT T.TestID, T.TestName, T.ShortName, T.SpecialTest, T.DecimalsAllowed, TS.SampleType, TS.CalibratorType " & vbCrLf & _
                                                " FROM   tparTests T INNER JOIN tparTestSamples TS ON T.TestID = TS.TestID " & vbCrLf & _
                                                " WHERE  T.InUse = 0 " & vbCrLf & _
                                                " AND    T.SpecialTest = 0 " & vbCrLf & _
                                                " AND   (TS.CalibratorType = 'FACTOR' " & vbCrLf & _
                                                " OR     T.TestID NOT IN (SELECT TestID FROM tparTestCalibrators " & vbCrLf & _
                                                                        " WHERE CalibratorID = " & pCalibratorID.ToString & ")) " & vbCrLf & _
                                                " AND   (TS.CalibratorType = 'ALTERNATIV' " & vbCrLf & _
                                                " OR     T.TestID NOT IN (SELECT TestID FROM tparTestCalibrators " & vbCrLf & _
                                                                        " WHERE CalibratorID = " & pCalibratorID.ToString & ")) " & vbCrLf

                        Dim myTestDataDS As New TestsDS()
                        Using cmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(cmd)
                                da.Fill(myTestDataDS.tparTests)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myTestDataDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.GetTestToSetCalibrator", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all Test/Sample Type using an Experimental Calibrator
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier; optional parameter</param>
        ''' <param name="pSampleType">SampleType Code; optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestsDS with the list of obtained Tests</returns>
        ''' <remarks>
        ''' Created by:  TR 15/02/2011
        ''' Modified by: TR 22/07/2011 - On the WHERE clause, added condition "AND TC.SampleType = TS.SampleType"
        '''              SA 21/12/2011 - Query changed to ANSI SQL sintax
        '''              SA 15/11/2012 - Added optional parameters to allow get data of and specific TestID/SampleType
        ''' </remarks>
        Public Function GetCalibratorTestSampleList(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pTestID As Integer = -1, _
                                                    Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT T.TestID, T.TestName, T.ShortName, T.SpecialTest, T.DecimalsAllowed, T.InUse, " & vbCrLf & _
                                                       " T.TestVersionNumber, T.TestVersionDateTime, TS.SampleType, TS.CalibratorType, " & vbCrLf & _
                                                       " TS.EnableStatus, C.CalibratorID, C.CalibratorName, TC.TestCalibratorID " & vbCrLf & _
                                                " FROM tparTests T INNER JOIN tparTestSamples TS ON T.TestID = TS.TestID " & vbCrLf & _
                                                                 " INNER JOIN tparTestCalibrators TC ON TS.TestID = TC.TestID " & vbCrLf & _
                                                                                                   " AND TS.SampleType = TC.SampleType " & vbCrLf & _
                                                                 " INNER JOIN tparCalibrators C ON TC.CalibratorID = C.CalibratorID " & vbCrLf

                        If (pTestID <> -1 AndAlso pSampleType.Trim <> String.Empty) Then cmdText &= " WHERE TS.TestID = " & pTestID.ToString & vbCrLf & _
                                                                                                    " AND   TS.SampleType = '" & pSampleType.Trim & "' " & vbCrLf
                        cmdText &= " ORDER BY C.CalibratorID "

                        Dim myTestDataDS As New TestsDS()
                        Using cmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(cmd)
                                da.Fill(myTestDataDS.tparTests)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myTestDataDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.GetCalibratorTestSampleList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read all Tests programmed as Contaminators
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestContaminationsDS with all Contaminator Tests</returns>
        ''' <remarks>
        ''' Created by:  AG 15/12/2010
        ''' </remarks>
        Public Function ReadAllContaminators(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT T.TestID, T.TestName, C.ContaminationID " & vbCrLf & _
                                                " FROM   tparTests T INNER JOIN tparTestReagents TR ON T.TestID = TR.TestID " & vbCrLf & _
                                                                   " LEFT OUTER JOIN tparContaminations C ON TR.ReagentID = C.ReagentContaminatorID " & vbCrLf & _
                                                " WHERE  C.ContaminationID IS NOT NULL " & vbCrLf & _
                                                " ORDER BY T.TestPosition "

                        Dim myTestContaminatorsDS As New TestContaminationsDS
                        Using cmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(cmd)
                                da.Fill(myTestContaminatorsDS.tparContaminations)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myTestContaminatorsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.ReadAllContaminators", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Update the sort of the Preloaded Tests as alphabetically order by the name of the test (except User tests)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  XB 04/06/2014 - BT #1646
        ''' </remarks>
        Public Function UpdatePreloadedTestSortByTestName(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    Dim myPosition As Integer = 0

                    Dim cmd As New SqlCommand
                    cmd.Connection = pDBConnection

                    Dim myTestsDS As TestsDS
                    myGlobalDataTO = ReadPreloadedTestByTestName(pDBConnection)
                    If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                        myTestsDS = CType(myGlobalDataTO.SetDatos, TestsDS)

                        For Each tpartestsDR As TestsDS.tparTestsRow In myTestsDS.tparTests
                            myPosition += 1
                            cmdText = " UPDATE tparTests SET " & _
                                      " TestPosition = " & myPosition.ToString() & _
                                      " WHERE  TestID = " & tpartestsDR.TestID.ToString()

                            cmd.CommandText = cmdText
                            myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                        Next

                    End If

                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.UpdatePreloadedTestSortByTestName", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the position of the User Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  XB 04/06/2014 - BT #1646
        ''' </remarks>
        Public Function UpdateUserTestPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pLastPreloadedTestPosition As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""

                    Dim cmd As New SqlCommand
                    cmd.Connection = pDBConnection

                    Dim myTestsDS As TestsDS
                    ' Get all User Tests
                    myGlobalDataTO = ReadByPreloadedTest(pDBConnection, 0)
                    If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                        myTestsDS = CType(myGlobalDataTO.SetDatos, TestsDS)

                        For Each tpartestsDR As TestsDS.tparTestsRow In myTestsDS.tparTests
                            pLastPreloadedTestPosition += 1
                            cmdText = " UPDATE tparTests SET " & _
                                      " TestPosition = " & pLastPreloadedTestPosition.ToString() & _
                                      " WHERE  TestID = " & tpartestsDR.TestID.ToString()

                            cmd.CommandText = cmdText
                            myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                        Next

                    End If

                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.UpdateUserTestPosition", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Gets all STD tests order by CustomPosition (return columns: TestType, TestID, CustomPosition As TestPosition, PreloadedTest, Available)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>GlobalDataTo with setDatos ReportsTestsSortingDS</returns>
        ''' <remarks>
        ''' AG 02/09/2014 - BA-1869
        ''' </remarks>
        Public Function GetCustomizedSortedTestSelectionList(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT 'STD' AS TestType, TestID, CustomPosition AS TestPosition, TestName, PreloadedTest, Available " & vbCrLf & _
                                                " FROM tparTests ORDER BY CustomPosition ASC "

                        Dim myDataSet As New ReportsTestsSortingDS

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.tcfgReportsTestsSorting)
                            End Using
                        End Using

                        resultData.SetDatos = myDataSet
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "tparTestsDAO.GetCustomizedSortedTestSelectionList", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Update (only when informed) columns CustomPosition and Available for STD tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestsSortingDS">Typed DataSet ReportsTestsSortingDS containing all tests to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: AG 03/09/2014 - BA-1869
        ''' </remarks>
        Public Function UpdateCustomPositionAndAvailable(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestsSortingDS As ReportsTestsSortingDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder
                    For Each testrow As ReportsTestsSortingDS.tcfgReportsTestsSortingRow In pTestsSortingDS.tcfgReportsTestsSorting
                        'Check there is something to update in this row
                        If Not (testrow.IsTestPositionNull AndAlso testrow.IsAvailableNull) Then
                            cmdText.Append(" UPDATE tparTests SET ")

                            'Update CustomPosition = TestPosition if informed
                            If Not testrow.IsTestPositionNull Then
                                cmdText.Append(" CustomPosition = " & testrow.TestPosition.ToString)
                            End If

                            'Update Available = Available if informed
                            If Not testrow.IsAvailableNull Then
                                'Add coma when required
                                If Not testrow.IsTestPositionNull Then
                                    cmdText.Append(" , ")
                                End If

                                cmdText.Append(" Available = " & CInt(IIf(testrow.Available, 1, 0)))
                            End If

                            cmdText.Append(" WHERE TestID  = " & testrow.TestID.ToString)
                            cmdText.Append(vbCrLf)
                        End If
                    Next

                    If cmdText.ToString.Length <> 0 Then
                        Using dbCmd As New SqlCommand(cmdText.ToString, pDBConnection)
                            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        End Using
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestsDAO.UpdateCustomPositionAndAvailable", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


#End Region

    End Class
End Namespace
