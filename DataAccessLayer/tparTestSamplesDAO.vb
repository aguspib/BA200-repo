Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class tparTestSamplesDAO
          

#Region "CRUD Methods"

        ''' <summary>
        ''' Create new Tests - Sample Types
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestSampleDS">Typed DataSet TestSamplesDS containing the list of Test Samples to create</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 24/02/2010
        ''' Modified by: DL 12/03/2010 - Call function ReplaceNumericString for values of all numeric fields
        '''              SA 04/06/2010 - Fields BorderLineLowerLimit and BorderLineUpperLimit removed from the insert. Added 
        '''                              new field ActiveRangeType
        '''              SG 10/06/2010 - FactorLowerLimit and FactorUpperlimit added to Insert   
        '''              SA 28/10/2010 - Added N preffix for multilanguage of field TS_USer. Mandatory fields have to be always
        '''                              informed, they do not allow null values. 
        '''              TR 17/01/2010 - Added the new files DiluentSolution,PredilutedSampleVol, PredilutedSampleVolSteps,
        '''                              PredilutedDilluentVol, PredilutedDiluentVolSteps.
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestSampleDS As TestSamplesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmd As New SqlCommand
                    cmd.Connection = pDBConnection

                    Dim keys As String = "(TestID, SampleType, SampleVolume, WashingVolume, PredilutionUseFlag, AutomaticRerun, CalibratorType, " & _
                                         " TestLongName, SampleVolumeSteps, WashingVolumeSteps, PredilutionFactor, PredilutionMode, " & _
                                         " RedPostdilutionFactor, RedPostSampleVolume, RedPostSampleVolumeSteps, IncPostdilutionFactor, " & _
                                         " IncPostSampleVolume,IncPostSampleVolumeSteps,AbsorbanceDilutionFactor, " & _
                                         " BlankAbsorbanceLimit, LinearityLimit, DetectionLimit, SlopeFactorA, SlopeFactorB, " & _
                                         " SubstrateDepletionValue, RerunLowerLimit, RerunUpperLimit, ActiveRangeType," & _
                                         " CalibrationFactor, SampleTypeAlternative, CalibratorReplicates," & _
                                         " NumberOfControls, ControlReplicates, RejectionCriteria, CalculationMode, NumberOfSeries, " & _
                                         " DefaultSampleType, FactorLowerLimit,FactorUpperLimit, DiluentSolution,PredilutedSampleVol, " & _
                                         " PredilutedSampleVolSteps, PredilutedDiluentVol, PreDiluentVolSteps, EnableStatus, FactoryCalib, " & _
                                         " TS_User, TS_DateTime, QCActive, TotalAllowedError) "

                    Dim values As String = " "
                    For Each TestSampleRow As TestSamplesDS.tparTestSamplesRow In pTestSampleDS.tparTestSamples.Rows
                        values = ""
                        values &= TestSampleRow.TestID.ToString() & ", "
                        values &= "'" & TestSampleRow.SampleType.ToString().Replace("'", "''") & "', "
                        values &= ReplaceNumericString(TestSampleRow.SampleVolume) & ", "
                        values &= ReplaceNumericString(TestSampleRow.WashingVolume) & ", "
                        values &= "'" & TestSampleRow.PredilutionUseFlag.ToString() & "', "
                        values &= "'" & TestSampleRow.AutomaticRerun.ToString() & "', "
                        values &= "'" & TestSampleRow.CalibratorType.ToString().Replace("'", "''") & "', "

                        If (TestSampleRow.IsTestLongNameNull Or TestSampleRow.TestLongName = "") Then
                            values &= "NULL, "
                        Else
                            values &= "N'" & TestSampleRow.TestLongName.ToString().Replace("'", "''") & "', "
                        End If
                        If (TestSampleRow.IsSampleVolumeStepsNull) Then
                            values &= "NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.SampleVolumeSteps) & ", "
                        End If
                        If (TestSampleRow.IsWashingVolumeStepsNull) Then
                            values &= "NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.WashingVolumeSteps) & ", "
                        End If
                        If (TestSampleRow.IsPredilutionFactorNull) Then
                            values &= "NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.PredilutionFactor) & ", "
                        End If
                        If (TestSampleRow.IsPredilutionModeNull) Then
                            values &= "NULL, "
                        Else
                            values &= "'" & TestSampleRow.PredilutionMode.ToString().Replace("'", "''") & "', "
                        End If
                        If (TestSampleRow.IsRedPostdilutionFactorNull) Then
                            values &= "NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.RedPostdilutionFactor) & ", "
                        End If
                        If (TestSampleRow.IsRedPostSampleVolumeNull) Then
                            values &= "NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.RedPostSampleVolume) & ", "
                        End If
                        If (TestSampleRow.IsRedPostSampleVolumeStepsNull) Then
                            values &= "NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.RedPostSampleVolumeSteps) & ", "
                        End If
                        If (TestSampleRow.IsIncPostdilutionFactorNull) Then
                            values &= "NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.IncPostdilutionFactor) & ", "
                        End If
                        If (TestSampleRow.IsIncPostSampleVolumeNull) Then
                            values &= "NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.IncPostSampleVolume) & ", "
                        End If
                        If (TestSampleRow.IsIncPostSampleVolumeStepsNull) Then
                            values &= "NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.IncPostSampleVolumeSteps) & ", "
                        End If
                        If (TestSampleRow.IsAbsorbanceDilutionFactorNull) Then
                            values &= "NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.AbsorbanceDilutionFactor) & ", "
                        End If
                        If (TestSampleRow.IsBlankAbsorbanceLimitNull) Then
                            values &= "NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.BlankAbsorbanceLimit) & ", "
                        End If
                        If (TestSampleRow.IsLinearityLimitNull) Then
                            values &= " NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.LinearityLimit) & ", "
                        End If
                        If (TestSampleRow.IsDetectionLimitNull) Then
                            values &= " NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.DetectionLimit) & ", "
                        End If
                        If (TestSampleRow.IsSlopeFactorANull) Then
                            values &= "NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.SlopeFactorA) & ", "
                        End If
                        If (TestSampleRow.IsSlopeFactorBNull) Then
                            values &= " NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.SlopeFactorB) & ", "
                        End If
                        If (TestSampleRow.IsSubstrateDepletionValueNull) Then
                            values &= "NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.SubstrateDepletionValue) & ", "
                        End If
                        If (TestSampleRow.IsRerunLowerLimitNull) Then
                            values &= " NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.RerunLowerLimit) & ", "
                        End If
                        If (TestSampleRow.IsRerunUpperLimitNull) Then
                            values &= " NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.RerunUpperLimit) & ", "
                        End If
                        If (TestSampleRow.IsActiveRangeTypeNull) Then
                            values &= " NULL, "
                        Else
                            values &= " '" & TestSampleRow.ActiveRangeType.ToString.Replace("'", "''") & "', "
                        End If
                      
                        If (TestSampleRow.IsCalibrationFactorNull) Then
                            values &= "NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.CalibrationFactor) & ", "
                        End If
                        If (TestSampleRow.IsSampleTypeAlternativeNull) Then
                            values &= " NULL, "
                        Else
                            values &= " '" & TestSampleRow.SampleTypeAlternative.ToString().Replace("'", "''") & "', "
                        End If
                        If (TestSampleRow.IsCalibratorReplicatesNull) Then
                            values &= " NULL, "
                        Else
                            values &= TestSampleRow.CalibratorReplicates.ToString() & ", "
                        End If
                       
                        If (TestSampleRow.IsNumberOfControlsNull) Then
                            values &= " NULL, "
                        Else
                            values &= TestSampleRow.NumberOfControls.ToString() & ", "
                        End If
                        If (TestSampleRow.IsControlReplicatesNull) Then
                            values &= " NULL, "
                        Else
                            values &= TestSampleRow.ControlReplicates.ToString() & ", "
                        End If
                        If (TestSampleRow.IsRejectionCriteriaNull) Then
                            values &= " NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.RejectionCriteria) & ", "
                        End If
                        If (TestSampleRow.IsCalculationModeNull) Then
                            values &= " NULL, "
                        Else
                            values &= "'" & TestSampleRow.CalculationMode.ToString().Replace("'", "''") & "', "
                        End If
                        If (TestSampleRow.IsNumberOfSeriesNull) Then
                            values &= " NULL, "
                        Else
                            values &= TestSampleRow.NumberOfSeries.ToString() & ", "
                        End If
                        If (TestSampleRow.IsDefaultSampleTypeNull) Then
                            values &= " 0, "
                        Else
                            values &= " '" & TestSampleRow.DefaultSampleType.ToString().Replace("'", "''") & "', "
                        End If
                        If (TestSampleRow.IsFactorLowerLimitNull) Then
                            values &= " NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.FactorLowerLimit) & ", "
                        End If
                        If (TestSampleRow.IsFactorUpperLimitNull) Then
                            values &= " NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.FactorUpperLimit) & ", "
                        End If

                        'TR 17/01/2011 -Add file for diluent.
                        If TestSampleRow.IsDiluentSolutionNull Then
                            values &= " NULL, "
                        Else
                            values &= " '" & TestSampleRow.DiluentSolution & "', "
                        End If

                        If TestSampleRow.IsPredilutedSampleVolNull Then
                            values &= " NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.PredilutedSampleVol) & ", "
                        End If

                        If TestSampleRow.IsPredilutedSampleVolStepsNull Then
                            values &= " NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.PredilutedSampleVolSteps) & ", "
                        End If

                        If TestSampleRow.IsPredilutedDiluentVolNull Then
                            values &= " NULL, "
                        Else
                            values &= ReplaceNumericString(TestSampleRow.PredilutedDiluentVol) & ", "
                        End If

                        If TestSampleRow.IsPredilutedSampleVolStepsNull Then
                            values &= " NULL, "
                        Else
                            'AG 12/01/2012 - Use the proper field
                            'values &= ReplaceNumericString(TestSampleRow.PredilutedSampleVolSteps) & ", "
                            values &= ReplaceNumericString(TestSampleRow.PreDiluentVolSteps) & ", "
                        End If
                        'TR 17/01/2011 -END

                        'TR 17/02/2011
                        If TestSampleRow.IsEnableStatusNull Then
                            values &= " NULL, "
                        Else
                            values &= " '" & TestSampleRow.EnableStatus & "', "
                        End If
                        'TR 17/02/2011 -END.

                        'TR 08/03/2011 -Set the factory calibrator value to false this is only true on initial mode.
                        'values &= " 'False', "
                        'TR 08/03/2011 -END 
                        'TR 14/02/2013 -Set the value recived on the dataset instead of fixe value 
                        If TestSampleRow.IsFactoryCalibNull Then
                            values &= "NULL, "
                        Else
                            values &= "'" & TestSampleRow.FactoryCalib & "',"
                        End If
                        'TR 14/02/2013 -END.
                        If (TestSampleRow.IsTS_UserNull) Then
                            'Dim myGlobalbase As New GlobalBase
                            values &= " N'" & GlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', "
                        Else
                            values &= " N'" & TestSampleRow.TS_User.ToString().Replace("'", "''") & "', "
                        End If
                        If (TestSampleRow.IsTS_DateTimeNull) Then
                            values &= " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "', "
                        Else
                            values &= " '" & TestSampleRow.TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "', "
                        End If

                        'TR 06/04/2011
                        values &= " '" & TestSampleRow.QCActive & "', "

                        If TestSampleRow.IsTotalAllowedErrorNull Then
                            values &= " NULL "
                        Else
                            values &= TestSampleRow.TotalAllowedError
                        End If
                        'TR 06/04/2011 -END

                        Dim cmdText As String = ""
                        cmdText = "INSERT INTO tparTestSamples " & keys & " VALUES (" & values & ")"

                        cmd.CommandText = cmdText
                        myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                        If (myGlobalDataTO.AffectedRecords > 0) Then
                            myGlobalDataTO.HasError = False
                            TestSampleRow.IsNew = False 'TR 05/02/2013 -Set value to false 
                        Else
                            myGlobalDataTO.HasError = True
                            Exit For
                        End If
                    Next
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestSamplesDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all data defined for a Test and Sample Type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type</param>
        ''' <param name="pGetReplicatesFlag"></param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestSamplesDS with all data of the 
        '''          specified Test and Sample Type</returns>
        ''' <remarks>
        ''' Created by:  SA 23/02/2010
        ''' AG 13/03/2013 - add  parameters to get also the replicates number. Used when import from LIS (using embedded synapse xml)
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                             ByVal pSampleType As String, ByVal pGetReplicatesFlag As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        If Not pGetReplicatesFlag Then
                            'Code in v1.0.0
                            cmdText = " SELECT * FROM tparTestSamples " & _
                                      " WHERE  TestID = " & pTestID & _
                                      " AND    SampleType = '" & pSampleType & "' "

                        Else
                            'New code for v2.0.0 when work with embedded synapse xml
                            cmdText = " SELECT ts.*, t.ReplicatesNumber FROM tparTestSamples ts " & _
                                " INNER JOIN tparTests t ON ts.TestID = t.TestID " & _
                                " WHERE  ts.TestID = " & pTestID & _
                                " AND    ts.SampleType = '" & pSampleType & "' "
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestSamplesDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update data of an specific Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestSampleDS">Typed DataSet TestSamplesDS containing the data of the Test Sample to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 24/02/2010
        ''' Modified by: DL 12/03/2010 - Call function ReplaceNumericString for values of all numeric fields
        '''              SA 04/06/2010 - Fields BorderLineLowerLimit and BorderLineUpperLimit removed from the insert. Added 
        '''                              new field ActiveRangeType
        '''              SG 10/06/2010 - FactorLowerLimit and FactorUpperlimit added to Update
        '''              SA 28/10/2010 - Added N preffix for multilanguage of field TS_USer. Mandatory fields have to be always
        '''                              informed, they do not allow null values. Added function ReplaceNumericString in field
        '''                              CalibratorFactor to avoid errors when the value has decimals.
        '''              TR 17/01/2010 - Added the new files DiluentSolution,PredilutedSampleVol, PredilutedSampleVolSteps,
        '''                              PredilutedDilluentVol, PredilutedDiluentVolSteps.
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestSampleDS As TestSamplesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim values As String = " "
                    For Each TestSampleRow As TestSamplesDS.tparTestSamplesRow In pTestSampleDS.tparTestSamples.Rows
                        'First update mandatory fields
                        values = ""
                        values &= " TestID = " & TestSampleRow.TestID.ToString() & ", "
                        values &= " SampleType = '" & TestSampleRow.SampleType.ToString().Replace("'", "''") & "', "
                        values &= " SampleVolume = " & ReplaceNumericString(TestSampleRow.SampleVolume) & ", "
                        values &= " WashingVolume = " & ReplaceNumericString(TestSampleRow.WashingVolume) & ", "
                        values &= " PredilutionUseFlag = '" & TestSampleRow.PredilutionUseFlag.ToString().Replace("'", "''") & "', "
                        values &= " AutomaticRerun = '" & TestSampleRow.AutomaticRerun.ToString().Replace("'", "''") & "', "
                        values &= " CalibratorType = '" & TestSampleRow.CalibratorType.ToString().Replace("'", "''") & "', "

                        If (TestSampleRow.IsTestLongNameNull Or TestSampleRow.TestLongName = "") Then
                            values &= " TestLongName = NULL, "
                        Else
                            values &= " TestLongName = N'" & TestSampleRow.TestLongName.ToString().Replace("'", "''") & "', "
                        End If
                        If (TestSampleRow.IsSampleVolumeStepsNull) Then
                            values &= " SampleVolumeSteps = NULL, "
                        Else
                            values &= " SampleVolumeSteps = " & ReplaceNumericString(TestSampleRow.SampleVolumeSteps) & ", "
                        End If
                        If (TestSampleRow.IsWashingVolumeStepsNull) Then
                            values &= " WashingVolumeSteps = NULL, "
                        Else
                            values &= " WashingVolumeSteps = " & ReplaceNumericString(TestSampleRow.WashingVolumeSteps) & ", "
                        End If
                        If (TestSampleRow.IsPredilutionFactorNull) Then
                            values &= " PredilutionFactor = NULL, "
                        Else
                            values &= " PredilutionFactor = " & ReplaceNumericString(TestSampleRow.PredilutionFactor) & ", "
                        End If
                        If (TestSampleRow.IsPredilutionModeNull) Then
                            values &= " PredilutionMode = NULL, "
                        Else
                            values &= " PredilutionMode = '" & TestSampleRow.PredilutionMode.ToString().Replace("'", "''") & "', "
                        End If
                        If (TestSampleRow.IsRedPostdilutionFactorNull) Then
                            values &= " RedPostdilutionFactor = NULL, "
                        Else
                            values &= " RedPostdilutionFactor = " & ReplaceNumericString(TestSampleRow.RedPostdilutionFactor) & ", "
                        End If
                        If (TestSampleRow.IsRedPostSampleVolumeNull) Then
                            values &= " RedPostSampleVolume = NULL, "
                        Else
                            values &= " RedPostSampleVolume = " & ReplaceNumericString(TestSampleRow.RedPostSampleVolume) & ", "
                        End If
                        If (TestSampleRow.IsRedPostSampleVolumeStepsNull) Then
                            values &= " RedPostSampleVolumeSteps = NULL, "
                        Else
                            values &= " RedPostSampleVolumeSteps = " & ReplaceNumericString(TestSampleRow.RedPostSampleVolumeSteps) & ", "
                        End If
                        If (TestSampleRow.IsIncPostdilutionFactorNull) Then
                            values &= " IncPostdilutionFactor = NULL, "
                        Else
                            values &= " IncPostdilutionFactor = " & ReplaceNumericString(TestSampleRow.IncPostdilutionFactor) & ", "
                        End If
                        If (TestSampleRow.IsIncPostSampleVolumeNull) Then
                            values &= " IncPostSampleVolume = NULL, "
                        Else
                            values &= " IncPostSampleVolume = " & ReplaceNumericString(TestSampleRow.IncPostSampleVolume) & ", "
                        End If
                        If (TestSampleRow.IsIncPostSampleVolumeStepsNull) Then
                            values &= " IncPostSampleVolumeSteps = NULL, "
                        Else
                            values &= " IncPostSampleVolumeSteps = " & ReplaceNumericString(TestSampleRow.IncPostSampleVolumeSteps) & ", "
                        End If
                        If (TestSampleRow.IsAbsorbanceDilutionFactorNull) Then
                            values &= " AbsorbanceDilutionFactor = NULL, "
                        Else
                            values &= " AbsorbanceDilutionFactor = '" & ReplaceNumericString(TestSampleRow.AbsorbanceDilutionFactor) & ", "
                        End If
                        If (TestSampleRow.IsBlankAbsorbanceLimitNull) Then
                            values &= " BlankAbsorbanceLimit = NULL, "
                        Else
                            values &= " BlankAbsorbanceLimit = " & ReplaceNumericString(TestSampleRow.BlankAbsorbanceLimit) & ", "
                        End If
                        If (TestSampleRow.IsLinearityLimitNull) Then
                            values &= " LinearityLimit = NULL, "
                        Else
                            values &= " LinearityLimit = " & ReplaceNumericString(TestSampleRow.LinearityLimit) & ", "
                        End If
                        If (TestSampleRow.IsDetectionLimitNull) Then
                            values &= " DetectionLimit = NULL, "
                        Else
                            values &= " DetectionLimit = " & ReplaceNumericString(TestSampleRow.DetectionLimit) & ", "
                        End If
                        If (TestSampleRow.IsSlopeFactorANull) Then
                            values &= " SlopeFactorA = NULL, "
                        Else
                            values &= " SlopeFactorA = " & ReplaceNumericString(TestSampleRow.SlopeFactorA) & ", "
                        End If
                        If (TestSampleRow.IsSlopeFactorBNull) Then
                            values &= " SlopeFactorB = NULL, "
                        Else
                            values &= " SlopeFactorB = " & ReplaceNumericString(TestSampleRow.SlopeFactorB) & ", "
                        End If
                        If (TestSampleRow.IsSubstrateDepletionValueNull) Then
                            values &= " SubstrateDepletionValue = NULL, "
                        Else
                            values &= " SubstrateDepletionValue = " & ReplaceNumericString(TestSampleRow.SubstrateDepletionValue) & ", "
                        End If
                        If (TestSampleRow.IsRerunLowerLimitNull) Then
                            values &= " RerunLowerLimit = NULL, "
                        Else
                            values &= " RerunLowerLimit = " & ReplaceNumericString(TestSampleRow.RerunLowerLimit) & ", "
                        End If
                        If (TestSampleRow.IsRerunUpperLimitNull) Then
                            values &= " RerunUpperLimit = NULL, "
                        Else
                            values &= " RerunUpperLimit = " & ReplaceNumericString(TestSampleRow.RerunUpperLimit) & ", "
                        End If
                        If (TestSampleRow.IsActiveRangeTypeNull) Then
                            values &= " ActiveRangeType = NULL, "
                        Else
                            values &= " ActiveRangeType = '" & TestSampleRow.ActiveRangeType & "', "
                        End If

                        If (TestSampleRow.IsCalibrationFactorNull) Then
                            values &= " CalibrationFactor = NULL, "
                        Else
                            values &= " CalibrationFactor = " & ReplaceNumericString(TestSampleRow.CalibrationFactor) & ", "
                        End If
                        If (TestSampleRow.IsSampleTypeAlternativeNull) Then
                            values &= " SampleTypeAlternative = NULL, "
                        Else
                            values &= " SampleTypeAlternative = '" & TestSampleRow.SampleTypeAlternative.ToString().Replace("'", "''") & "', "
                        End If
                        If (TestSampleRow.IsCalibratorReplicatesNull) Then
                            values &= " CalibratorReplicates = NULL, "
                        Else
                            values &= " CalibratorReplicates = " & TestSampleRow.CalibratorReplicates.ToString() & ", "
                        End If

                        If (TestSampleRow.IsNumberOfControlsNull) Then
                            values &= " NumberOfControls = NULL, "
                        Else
                            values &= " NumberOfControls = " & TestSampleRow.NumberOfControls.ToString() & ", "
                        End If
                        If (TestSampleRow.IsControlReplicatesNull) Then
                            values &= " ControlReplicates = NULL, "
                        Else
                            values &= " ControlReplicates = " & TestSampleRow.ControlReplicates.ToString() & ", "
                        End If
                        If (TestSampleRow.IsRejectionCriteriaNull) Then
                            values &= " RejectionCriteria = NULL, "
                        Else
                            values &= " RejectionCriteria = " & ReplaceNumericString(TestSampleRow.RejectionCriteria) & ", "
                        End If
                        If (TestSampleRow.IsCalculationModeNull) Then
                            values &= " CalculationMode = NULL, "
                        Else
                            values &= " CalculationMode = '" & TestSampleRow.CalculationMode.ToString().Replace("'", "''") & "', "
                        End If
                        If (TestSampleRow.IsNumberOfSeriesNull) Then
                            values &= " NumberOfSeries = NULL, "
                        Else
                            values &= " NumberOfSeries = " & TestSampleRow.NumberOfSeries.ToString() & ", "
                        End If
                        If (TestSampleRow.IsDefaultSampleTypeNull) Then
                            values &= " DefaultSampleType = 0, "
                        Else
                            values &= " DefaultSampleType = '" & TestSampleRow.DefaultSampleType.ToString() & "', "
                        End If
                        If (TestSampleRow.IsFactorLowerLimitNull) Then
                            values &= " FactorLowerLimit = NULL, "
                        Else
                            values &= " FactorLowerLimit = " & ReplaceNumericString(TestSampleRow.FactorLowerLimit) & ", "
                        End If
                        If (TestSampleRow.IsFactorUpperLimitNull) Then
                            values &= " FactorUpperLimit = NULL, "
                        Else
                            values &= " FactorUpperLimit = " & ReplaceNumericString(TestSampleRow.FactorUpperLimit) & ", "
                        End If

                        'TR 17/01/2011 -Add file for diluent.

                        If (TestSampleRow.IsDiluentSolutionNull) Then
                            values &= " DiluentSolution = NULL, "
                        Else
                            values &= " DiluentSolution = '" & TestSampleRow.DiluentSolution & "', "
                        End If

                        If (TestSampleRow.IsPredilutedSampleVolNull) Then
                            values &= " PredilutedSampleVol = NULL, "
                        Else
                            values &= " PredilutedSampleVol = " & ReplaceNumericString(TestSampleRow.PredilutedSampleVol) & ", "
                        End If

                        If (TestSampleRow.IsPredilutedSampleVolStepsNull) Then
                            values &= " PredilutedSampleVolSteps = NULL, "
                        Else
                            values &= " PredilutedSampleVolSteps = " & ReplaceNumericString(TestSampleRow.PredilutedSampleVolSteps) & ", "
                        End If

                        If (TestSampleRow.IsPredilutedDiluentVolNull) Then
                            values &= " PredilutedDiluentVol = NULL, "
                        Else
                            values &= " PredilutedDiluentVol = " & ReplaceNumericString(TestSampleRow.PredilutedDiluentVol) & ", "
                        End If

                        If (TestSampleRow.IsPreDiluentVolStepsNull) Then
                            values &= " PreDiluentVolSteps = NULL, "
                        Else
                            values &= " PreDiluentVolSteps = " & ReplaceNumericString(TestSampleRow.PreDiluentVolSteps) & ", "
                        End If
                        'TR 17/01/2011 -END.

                        'TR 17/02/2011
                        If TestSampleRow.IsEnableStatusNull Then
                            values &= " EnableStatus = NULL, "
                        Else
                            values &= String.Format(" EnableStatus = '{0}', ", TestSampleRow.EnableStatus)
                        End If
                        'TR 17/02/2011 -END

                        'TR 08/03/2011
                        If TestSampleRow.IsFactoryCalibNull Then
                            values &= " FactoryCalib = NULL, "
                        Else
                            values &= String.Format(" FactoryCalib = '{0}', ", TestSampleRow.FactoryCalib)
                        End If
                        'TR 08/03/2011 -END

                        'TR 06/04/2011 -Add total allowed error
                        If TestSampleRow.IsTotalAllowedErrorNull Then
                            values &= " TotalAllowedError = NULL, "
                        Else
                            values &= String.Format(" TotalAllowedError = {0}, ", TestSampleRow.TotalAllowedError)
                        End If

                        values &= String.Format(" QCActive = '{0}', ", TestSampleRow.QCActive)

                        'TR 06/04/2011 -END

                        If (TestSampleRow.IsTS_UserNull) Then
                            'Dim myGlobalbase As New GlobalBase
                            values &= " TS_User = N'" & GlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', "
                        Else
                            values &= " TS_User = N'" & TestSampleRow.TS_User.ToString().Replace("'", "''") & "', "
                        End If

                        If (TestSampleRow.IsTS_DateTimeNull) Then
                            values &= " TS_DateTime ='" & Now.ToString("yyyyMMdd HH:mm:ss") & "' "
                        Else
                            values &= " TS_DateTime ='" & TestSampleRow.TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "' "
                        End If

                        Dim cmdText As String = ""
                        cmdText = " UPDATE tparTestSamples SET " & values & _
                                  " WHERE  TestID = " & TestSampleRow.TestID.ToString() & _
                                  " AND    SampleType = '" & TestSampleRow.SampleType & "' "

                        Dim cmd As New SqlCommand
                        cmd.Connection = pDBConnection
                        cmd.CommandText = cmdText

                        myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                        'If myGlobalDataTO.AffectedRecords > 0 Then
                        '    myGlobalDataTO.HasError = False
                        'Else
                        '    myGlobalDataTO.HasError = True
                        'End If
                    Next
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestSamplesDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the EnableStatus by TestID and Sample Type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test ID</param>
        ''' <param name="pSampleType">Sample Type</param>
        ''' <param name="pEnableStatus">Status</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  TR 17/02/2011
        ''' </remarks>
        Public Function UpdateTestSampleEnableStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                     ByVal pSampleType As String, ByVal pEnableStatus As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim values As String = " "
                    values = String.Format("EnableStatus = '{0}'", pEnableStatus)

                    Dim cmdText As String = ""
                    cmdText = String.Format(" UPDATE tparTestSamples SET {0} WHERE  TestID = {1} AND SampleType = '{2}' ", values, pTestID, pSampleType)

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
                GlobalBase.CreateLogActivity(ex.Message, "tparTestSamplesDAO.UpdateTestSampleEnableStatus", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the Enable Status of all Sample Types defined for the informed Test and using as Alternative Calibrator the one defined 
        ''' for the informed Sample Type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type of the ALTERNATIVE Calibrator</param>
        ''' <param name="pEnableStatus">New value for field EnableStatus</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 21/05/2014 - BT #1633
        ''' </remarks>
        Public Function UpdateEnableStatusForALTERNATIVECalib(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                              ByVal pSampleType As String, ByVal pEnableStatus As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE tparTestSamples SET EnableStatus = " & IIf(pEnableStatus, 1, 0).ToString & vbCrLf & _
                                            " WHERE  TestID = " & pTestID.ToString & vbCrLf & _
                                            " AND    CalibratorType = 'ALTERNATIV' " & vbCrLf & _
                                            " AND    SampleTypeAlternative = '" & pSampleType.Trim & "' " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestSamplesDAO.UpdateEnableStatusForALTERNATIVECalib", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the Factory Calibrator by TestID and SampleType
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestID">Test ID</param>
        ''' <param name="pSampleType">Sample Type</param>
        ''' <param name="pFactoryCalib">Value for factory Calib.</param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 08/03/2011</remarks>
        Public Function UpdateTestSampleFactoryCalibrator(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                    ByVal pSampleType As String, ByVal pFactoryCalib As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim values As String = " "
                    values = String.Format("FactoryCalib = '{0}'", pFactoryCalib)

                    Dim cmdText As String = ""
                    cmdText = String.Format(" UPDATE tparTestSamples SET {0} WHERE  TestID = {1} AND SampleType = '{2}' ", values, pTestID, pSampleType)

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
                GlobalBase.CreateLogActivity(ex.Message, "tparTestSamplesDAO.UpdateTestSampleFactoryCalibrator", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Delete the specified Test/Sample Type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 16/03/2010
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText &= " DELETE FROM  tparTestSamples "
                    cmdText &= " WHERE TestID = " & pTestID
                    cmdText &= " AND   SampleType = '" & pSampleType & "'"

                    Dim cmd As New SqlCommand
                    cmd.Connection = pDBConnection
                    cmd.CommandText = cmdText

                    myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                    If (myGlobalDataTO.AffectedRecords > 0) Then
                        myGlobalDataTO.HasError = False
                    Else
                        'AG 28/09/2010
                        'myGlobalDataTO.HasError = True
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestSamplesDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get data of all Sample Types defined for the specified Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestSamplesDS with data of all
        '''          Sample Types defined for the specified Test</returns>
        ''' <remarks>
        ''' Created by:  TR 17/02/2010
        ''' Modified by: SG 10/06/2010 - FactorLowerLimit and FactorUpperlimit added to ReadByTestID
        '''              AG 28/09/2010 - Order different SampleTypes not alphabetic but using: DefaultSampleType, MasterData.Position
        '''              XB 04/06/2013 - Update query to return also tparTestSamples without multilanguage translation
        ''' </remarks>
        Public Function ReadByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        ' XB 04/06/2013
                        'cmdText = " SELECT (MD.ItemID + '-' + MR.ResourceText  ) as ItemIDDesc, "
                        'cmdText &= " TS.* FROM tparTestSamples TS INNER JOIN tcfgMasterData MD "
                        'cmdText &= " ON TS.SampleType = MD.ItemID AND MD.SubTableID = 'SAMPLE_TYPES' "
                        'cmdText &= " INNER JOIN tfmwMultiLanguageResources MR ON (md.ResourceID = mr.ResourceID ) "
                        'cmdText &= " WHERE  TestID = " & pTestID
                        'cmdText &= " AND MR.LanguageID = '" & var.GetSessionInfo.ApplicationLanguage & "' "
                        'cmdText &= " ORDER BY TS.DefaultSampleType DESC, MD.Position"

                        cmdText = " SELECT (MD.ItemID + '-' + MR.ResourceText  ) as ItemIDDesc, "
                        cmdText &= " TS.*, MD.Position FROM tparTestSamples TS INNER JOIN tcfgMasterData MD "
                        cmdText &= " ON TS.SampleType = MD.ItemID AND MD.SubTableID = 'SAMPLE_TYPES' "
                        cmdText &= " INNER JOIN tfmwMultiLanguageResources MR ON (md.ResourceID = mr.ResourceID ) "
                        cmdText &= " WHERE  TestID = " & pTestID
                        cmdText &= " AND MR.LanguageID = '" & GlobalBase.GetSessionInfo.ApplicationLanguage & "' "
                        cmdText &= " UNION "
                        cmdText &= " SELECT (MD.ItemID + '-' + MD.FixedItemDesc  ) as ItemIDDesc, "
                        cmdText &= " TS.*, MD.Position FROM tparTestSamples TS INNER JOIN tcfgMasterData MD "
                        cmdText &= " ON TS.SampleType = MD.ItemID AND MD.SubTableID = 'SAMPLE_TYPES' "
                        cmdText &= " WHERE  TestID = " & pTestID
                        cmdText &= " AND MD.MultiLanguageFlag = 0 "
                        cmdText &= " ORDER BY DefaultSampleType DESC, Position"
                        ' XB 04/06/2013

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        Dim myTestSamplesDS As New TestSamplesDS()
                        Dim da As New SqlDataAdapter(dbCmd)
                        da.Fill(myTestSamplesDS.tparTestSamples)

                        myGlobalDataTO.SetDatos = myTestSamplesDS
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestSamplesDAO.ReadByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update value of field NumberOfControls for the specified Test/SampleType according the number
        ''' of Controls linked to it in table tparTestControls
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 16/05/2011
        ''' </remarks>
        Public Function UpdateNumOfControls(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                            ByVal pSampleType As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE tparTestSamples " & vbCrLf & _
                                            " SET    NumberOfControls = (SELECT COUNT(*) FROM tparTestControls " & vbCrLf & _
                                                                       " WHERE  TestID = " & pTestID & vbCrLf & _
                                                                       " AND    SampleType = '" & pSampleType & "' " & vbCrLf & _
                                                                       " AND    TestType = 'STD') " & vbCrLf & _
                                            " WHERE  TestID     = " & pTestID & vbCrLf & _
                                            " AND    SampleType = '" & pSampleType & "' "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = (resultData.AffectedRecords = 0)
                    End Using

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestSamplesDAO.UpdateNumOfControls", EventLogEntryType.Error, False)
            End Try

            Return resultData
        End Function


        Public Shared Function GetPredilutionModeForTest(pTestID As Integer, pSampleType As String) As String
            If pSampleType = String.Empty Then Return ""

            Dim Query As String = String.Format("SELECT a.ReagentID, b.SampleType, b.predilutionmode FROM {0}.[dbo].[tparTestReagents] a, {0}.[dbo].[tpartestsamples] b where a.TestID = b.TestID ", GlobalBase.DatabaseName) 'IT 11/06/2015 - BA-2613
            Dim connection As TypedGlobalDataTo(Of SqlConnection) = Nothing

            Dim Result As String = ""
            <ThreadStatic> Static DataTableCache As DataTable
            Try

                If DataTableCache Is Nothing Then
                    DataTableCache = New DataTable
                    connection = GetSafeOpenDBConnection()
                    If (connection.SetDatos Is Nothing) Then
                        Return ""
                    Else

                        Dim dbCmd As New SqlCommand
                        dbCmd.Connection = connection.SetDatos
                        dbCmd.CommandText = Query
                        Dim da As New SqlDataAdapter(dbCmd)
                        da.Fill(DataTableCache)

                    End If
                End If

                '2.- Parte 2 comprobar la caché:
                <ThreadStatic> Static ValuesCache As New Dictionary(Of KeyValuePair(Of Integer, String), String)
                If ValuesCache.TryGetValue(New KeyValuePair(Of Integer, String)(pTestID, pSampleType), Result) Then
                    Return Result
                Else
                    Dim filter = (From cosica In DataTableCache Where CInt(cosica("ReagentID")) = pTestID And CType(cosica("SampleType"), String) = pSampleType).First
                    If filter IsNot Nothing AndAlso filter.IsNull("predilutionmode") = False Then

                        Result = CType(filter("predilutionmode"), String)
                        ValuesCache.Add(New KeyValuePair(Of Integer, String)(pTestID, pSampleType), Result)
                    Else
                        ValuesCache.Add(New KeyValuePair(Of Integer, String)(pTestID, pSampleType), String.Empty)
                        Result = ""
                    End If
                End If
                Return Result
            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex) '.Message, "tparTestSamplesDAO.UpdateNumOfControls", EventLogEntryType.Error, False)
                Throw
                'resultData.HasError = True
                'resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                'resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                'GlobalBase.CreateLogActivity(ex.Message, "tparTestSamplesDAO.UpdateNumOfControls", EventLogEntryType.Error, False)
            Finally
                If connection IsNot Nothing AndAlso connection.SetDatos IsNot Nothing Then
                    Try
                        connection.SetDatos.Close()
                    Catch : End Try
                End If
            End Try

            Return ""

        End Function



#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Get the Sample Type marked as default for the specified Test. Get also TestName, BlankReplicates and TestVersionNumber
        ''' for the Test. The default SampleType is used to get some values when a Blank is executed for the Test
        ''' </summary>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestsDS with the information for the specified Test</returns>
        ''' <remarks>
        ''' Created by: 
        ''' Modified by: SA 18/02/2011 - Get also TestName, BlankReplicates and TestVersionNumber for the Test.
        '''                              Return a GlobalDataTO containing a typed DataSet TestsDS
        ''' </remarks>
        Public Function GetDefaultSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String
                        cmdText = " SELECT TS.SampleType, T.TestName, T.BlankReplicates, T.TestVersionNumber, T.BlankMode " & vbCrLf & _
                                  " FROM   tparTestSamples TS INNER JOIN tparTests T ON TS.TestID = T.TestID " & vbCrLf & _
                                  " WHERE  TS.TestID = " & pTestID & vbCrLf & _
                                  " AND    TS.DefaultSampleType = 1 "

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        Dim myTestsDS As New TestsDS()
                        Dim da As New SqlDataAdapter(dbCmd)
                        da.Fill(myTestsDS.tparTests)

                        resultData.SetDatos = myTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestSamplesDAO.GetDefaultSampleType", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' For the specified Test/SampleType, get all data needed to export it to QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryQCTestSamples with all data needed to export the Test/SampleType to QC Module</returns>
        ''' <remarks>
        ''' Created by:  SA 21/05/2012
        ''' Modified by: SA 27/08/2014 - Changed the SQL Query to return also field TestLongName from table tparTestSamples (added as part of BT #1865, in which
        '''                              field TestLongName is also exported to QC Module for ISE Tests; this change is to export the field also for Standard Tests)
        ''' </remarks>
        Public Function GetDefinitionForQCModule(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT T.TestName, T.ShortName AS TestShortName, T.PreloadedTest, T.MeasureUnit, T.DecimalsAllowed, " & vbCrLf & _
                                                       " TS.RejectionCriteria, TS.CalculationMode, TS.NumberOfSeries, TS.TestLongName " & vbCrLf & _
                                                " FROM   tparTests T INNER JOIN tparTestSamples TS ON T.TestID = TS.TestID " & vbCrLf & _
                                                " WHERE  TS.TestID     = " & pTestID.ToString & vbCrLf & _
                                                " AND    TS.SampleType = '" & pSampleType & "' " & vbCrLf

                        Dim myQCTestSamplesDS As New HistoryTestSamplesDS()
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dataAdapter As New SqlDataAdapter(dbCmd)
                                dataAdapter.Fill(myQCTestSamplesDS.tqcHistoryTestSamples)
                            End Using
                        End Using

                        resultData.SetDatos = myQCTestSamplesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestSamplesDAO.GetDefinitionForQCModule", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified Test, get the list of SampleTypes linked to the informed Test and using the Calibrator of the informed SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">SampleType Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestSamplesDS with the list of SampleTypes linked to the informed Test and using the Calibrator
        '''          of the specified SampleType</returns>
        ''' <remarks>
        ''' Created by: SA 22/10/2012
        ''' </remarks>
        Public Function GetSampleTypesUsingAlternativeCalib(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT SampleType FROM tparTestSamples " & vbCrLf & _
                                                " WHERE  TestID = " & pTestID.ToString & vbCrLf & _
                                                " AND    SampleTypeAlternative = '" & pSampleType.Trim & "' " & vbCrLf

                        Dim myTestSamplesDS As New TestSamplesDS()
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dataAdapter As New SqlDataAdapter(dbCmd)
                                dataAdapter.Fill(myTestSamplesDS.tparTestSamples)
                            End Using
                        End Using

                        resultData.SetDatos = myTestSamplesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestSamplesDAO.GetSampleTypesUsingAlternativeCalib", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified Test/SampleType, get value of limits that are needed to save Results in Historic Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestSamplesDS with value of limits that are needed to save 
        '''          Results in Historic Module</returns>
        ''' <remarks>
        ''' Created by:  SA 18/10/2012
        ''' </remarks>
        Public Function GetLimitsForHISTResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT T.KineticBlankLimit, TS.BlankAbsorbanceLimit, TS.FactorLowerLimit, TS.FactorUpperLimit " & vbCrLf & _
                                                " FROM   tparTests T INNER JOIN tparTestSamples TS ON T.TestID = TS.TestID " & vbCrLf & _
                                                " WHERE  TS.TestID     = " & pTestID.ToString & vbCrLf & _
                                                " AND    TS.SampleType = '" & pSampleType & "' " & vbCrLf

                        Dim myTestSamplesDS As New TestSamplesDS()
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dataAdapter As New SqlDataAdapter(dbCmd)
                                dataAdapter.Fill(myTestSamplesDS.tparTestSamples)
                            End Using
                        End Using

                        resultData.SetDatos = myTestSamplesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestSamplesDAO.GetLimitsForHISTResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "HISTORY methods"

        ''' <summary>
        ''' Fill a structure for History test samples using the current test programming
        ''' NOTE: We can not use the methods FillTestFields, FillTestSampleFields, FillReagentFields from HisTestSamplesDelegate because of the circular references
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisTestSamplesDS with data of the informed Test/SampleType in Parameters Programming</returns>
        ''' <remarks>
        ''' Created by: AG 09/10/2012
        ''' </remarks>
        Public Function HIST_FillHisTestSamplesByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT T.TestID, TS.SampleType, T.TestName, TS.TestLongName, T.MeasureUnit, T.DecimalsAllowed, " & vbCrLf & _
                                                       " T.KineticBlankLimit, T.ProzoneRatio, T.ProzoneTime1, T.ProzoneTime2, TS.BlankAbsorbanceLimit, " & vbCrLf & _
                                                       " TS.LinearityLimit, TS.DetectionLimit, TS.SlopeFactorA, TS.SlopeFactorB, TS.SubstrateDepletionValue, " & vbCrLf & _
                                                       " TS.CalibratorType, TS.CalibrationFactor AS CalibratorFactor, NULL AS CalibratorID " & vbCrLf & _
                                                " FROM   tparTests T INNER JOIN tparTestSamples TS ON T.TestID = TS.TestID " & vbCrLf & _
                                                " WHERE  T.TestID = " & pTestID.ToString & vbCrLf & _
                                                " AND    TS.SampleType = '" & pSampleType.Trim & "' " & vbCrLf

                        Dim myDataSet As New HisTestSamplesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.thisTestSamples)
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
                GlobalBase.CreateLogActivity(ex.Message, "tparTestSamplesDAO.HIST_FillHisTestSamplesByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

    End Class

End Namespace