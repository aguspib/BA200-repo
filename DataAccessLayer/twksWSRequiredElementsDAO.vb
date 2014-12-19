Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class twksWSRequiredElementsDAO
        Inherits DAOBase

#Region "CRUD Methods"

        ''' <summary>
        ''' Create one or more new Required Elements in a specific WorkSession.  Required Elements are 
        ''' automatically created every time a Blank, Calibrator, Control and/or Patient Sample is included 
        ''' in a WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pElement">Dataset with structure of table twksWSRequiredElements</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 23/11/2009 - Tested: PENDING
        ''' Modified by: SA 09/03/2010 - Changes to include new field SampleID in the INSERT sentence
        '''              SA 17/03/2010 - Changes to include new field TubeType in the INSERT sentence
        '''              SA 22/10/2010 - Changes to include new field OnlyForISE in the INSERT sentence
        '''              SA 04/11/2010 - Add N preffix for multilanguage of field SampleID
        '''              SA 11/01/2012 - Removed code to set HasError=True depending on value of AffectedRecords
        '''              SG 29/04/2013 - Insert SpecimenList value when indormed
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pElement As WSRequiredElementsDS, Optional pSpecimenIDList As String = "") As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an opened Database Connection
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " INSERT INTO twksWSRequiredElements (ElementID, WorkSessionID, TubeContent, ReagentID, " & vbCrLf & _
                                                                                " SolutionCode, RequiredVolume, CalibratorID, ControlID, " & vbCrLf & _
                                                                                " MultiItemNumber, SampleType, OrderID, PatientID, SampleID, " & vbCrLf & _
                                                                                " PredilutionFactor, OnlyForISE, ElementStatus, TubeType, SpecimenIDList) " & vbCrLf & _
                                            " VALUES (" & pElement.twksWSRequiredElements(0).ElementID & ", " & vbCrLf & _
                                                    "'" & pElement.twksWSRequiredElements(0).WorkSessionID.Trim & "', " & vbCrLf & _
                                                    "'" & pElement.twksWSRequiredElements(0).TubeContent.Trim & "', " & vbCrLf

                    'ReagentID is informed only when the Element is a Reagent 
                    If (pElement.twksWSRequiredElements(0).IsReagentIDNull) Then
                        cmdText += " NULL, " & vbCrLf
                    Else
                        cmdText += pElement.twksWSRequiredElements(0).ReagentID & ", " & vbCrLf
                    End If

                    'SolutionCode is informed only when the Element is an Special Solution
                    '(Distilled Water or Saline Solution) or a Washing Solution
                    If (pElement.twksWSRequiredElements(0).IsSolutionCodeNull) Then
                        cmdText += " NULL, " & vbCrLf
                    Else
                        cmdText += "'" & pElement.twksWSRequiredElements(0).SolutionCode & "', " & vbCrLf
                    End If

                    'Volume is informed only when the Element is a Reagent or an Additional Solution
                    If (pElement.twksWSRequiredElements(0).IsReagentIDNull AndAlso pElement.twksWSRequiredElements(0).IsSolutionCodeNull) Then
                        cmdText += " NULL, " & vbCrLf
                    Else
                        cmdText += ReplaceNumericString(pElement.twksWSRequiredElements(0).RequiredVolume) & ", " & vbCrLf
                    End If

                    'CalibratorID is informed only when the Element is a Calibrator
                    If (pElement.twksWSRequiredElements(0).IsCalibratorIDNull) Then
                        cmdText += " NULL, " & vbCrLf
                    Else
                        cmdText += pElement.twksWSRequiredElements(0).CalibratorID & ", " & vbCrLf
                    End If

                    'ControlID is informed only when the Element is a Control
                    If (pElement.twksWSRequiredElements(0).IsControlIDNull) Then
                        cmdText += " NULL, " & vbCrLf
                    Else
                        cmdText += pElement.twksWSRequiredElements(0).ControlID & ", " & vbCrLf
                    End If

                    'MultiItemNumber is informed only when the Element is Reagent or Calibrator
                    'It will be the Reagent Number or the Calibrator Number respectively
                    If (pElement.twksWSRequiredElements(0).IsReagentNumberNull AndAlso pElement.twksWSRequiredElements(0).IsMultiItemNumberNull) Then
                        cmdText += " NULL, " & vbCrLf
                    Else
                        If (Not pElement.twksWSRequiredElements(0).IsReagentNumberNull) Then
                            cmdText += pElement.twksWSRequiredElements(0).ReagentNumber & ", " & vbCrLf
                        ElseIf (Not pElement.twksWSRequiredElements(0).IsMultiItemNumberNull) Then
                            cmdText += pElement.twksWSRequiredElements(0).MultiItemNumber & ", " & vbCrLf
                        End If
                    End If

                    'SampleType is informed only when the Element is a Patient Sample
                    If (pElement.twksWSRequiredElements(0).IsSampleTypeNull) Then
                        cmdText += " NULL, " & vbCrLf
                    Else
                        cmdText += "'" & pElement.twksWSRequiredElements(0).SampleType.Trim & "', " & vbCrLf
                    End If

                    'OrderID is informed only when the Element is a Patient Sample
                    If (pElement.twksWSRequiredElements(0).IsOrderIDNull) Then
                        cmdText += " NULL, " & vbCrLf
                    Else
                        cmdText += "'" & pElement.twksWSRequiredElements(0).OrderID.Trim & "', " & vbCrLf
                    End If

                    'PatientID is informed only when the Element is a Patient Sample
                    If (pElement.twksWSRequiredElements(0).IsPatientIDNull) Then
                        cmdText += " NULL, " & vbCrLf
                    Else
                        cmdText += "N'" & pElement.twksWSRequiredElements(0).PatientID.Trim & "', " & vbCrLf
                    End If

                    'SampleID is informed only when the Element is a Patient Sample
                    If (pElement.twksWSRequiredElements(0).IsSampleIDNull) Then
                        cmdText += " NULL, " & vbCrLf
                    Else
                        cmdText += "N'" & pElement.twksWSRequiredElements(0).SampleID.Trim.Replace("'", "''") & "', " & vbCrLf
                    End If

                    'Predilution Factor will be informed only when the Element corresponds a diluted SampleType
                    'for a Patient Sample
                    If (pElement.twksWSRequiredElements(0).IsPredilutionFactorNull) Then
                        cmdText += " NULL, " & vbCrLf
                    Else
                        cmdText += pElement.twksWSRequiredElements(0).PredilutionFactor & ", " & vbCrLf
                    End If

                    'OnlyForISE is set to False when it is not informed
                    If (pElement.twksWSRequiredElements(0).IsOnlyForISENull) Then
                        cmdText += " 0, " & vbCrLf
                    Else
                        cmdText += Convert.ToInt32(pElement.twksWSRequiredElements(0).OnlyForISE).ToString & ", " & vbCrLf
                    End If

                    'ElementStatus is always informed 
                    cmdText += " '" & pElement.twksWSRequiredElements(0).ElementStatus.Trim & "', " & vbCrLf

                    'TubeType is informed only for Calibrators, Controls and Patient Samples
                    If (pElement.twksWSRequiredElements(0).IsTubeTypeNull) Then
                        cmdText += " NULL, " & vbCrLf
                    Else
                        cmdText += "'" & pElement.twksWSRequiredElements(0).TubeType.Trim & "', " & vbCrLf
                    End If

                    'SGM 29/04/2013 - insert SpecimenList value when indormed
                    If pSpecimenIDList.Length = 0 Then
                        cmdText += " NULL) " & vbCrLf
                    Else
                        cmdText += "'" & pSpecimenIDList.Trim & "') " & vbCrLf
                    End If
                    'end SGM 29/04/2013

                    'Execute the SQL sentence 
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Delete the specified Required Element
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  BK 08/12/2009 - Tested: OK
        ''' Modified by: SA 11/01/2012 - Removed code to set HasError=True depending on value of AffectedRecords
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pElementID As Integer) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM twksWSRequiredElements " & vbCrLf & _
                                            " WHERE  ElementID = " & pElementID

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get the details of the specified Required Element 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementID">Required Element Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS with all data of the specified Required Element</returns>
        ''' <remarks>
        ''' Created by:  BK 08/12/2009 - Tested: OK
        ''' Modified by: SA 11/01/2010 - Changed the way of open the DB Connection to fulfill the new template. 
        '''                              Query changed for a Select *
        '''              SA 09/01/2012 - Changed the function template
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pElementID As Integer) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM   twksWSRequiredElements " & vbCrLf & _
                                                " WHERE  ElementID = " & pElementID

                        Dim resultData As New WSRequiredElementsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRequiredElements)
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
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get all Patient Samples for manual predilutions having field SpecimenIDList informed
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS with all data obtained</returns>
        ''' <remarks>
        ''' Created by:  SA 04/04/2014 - BT #1524
        ''' </remarks>
        Public Function ReadAllPatientDilutions(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT ISNULL(PatientID, SampleID) AS PatientID, SampleType, REPLACE(SpecimenIDList, CHAR(13), ', ') AS SpecimenIDList, ElementFinished " & vbCrLf & _
                                                " FROM   twksWSRequiredElements " & vbCrLf & _
                                                " WHERE  TubeContent = 'PATIENT' " & vbCrLf & _
                                                " AND    PredilutionFactor IS NOT NULL " & vbCrLf & _
                                                " AND    SpecimenIDList IS NOT NULL " & vbCrLf

                        Dim resultData As New WSRequiredElementsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRequiredElements)
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
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.ReadAllPatientDilutions", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Update data of the specified ElementID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>Global object containing the error information</returns>
        ''' <remarks>
        ''' Created by:  BK 08/12/2009 - Tested: OK
        ''' Modified by: SA 09/03/2010 - Changes to include new field SampleID in the UPDATE sentence
        '''              SA 04/11/2010 - Add N preffix for multilanguage of field SampleID
        '''              SA 11/01/2012 - Removed code to set HasError=True depending on value of AffectedRecords
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSRequiredElementsDS As WSRequiredElementsDS) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksWSRequiredElements " & vbCrLf & _
                                            " SET    WorkSessionID = '" & pWSRequiredElementsDS.twksWSRequiredElements(0).WorkSessionID & "', " & vbCrLf & _
                                                   " TubeContent = '" & pWSRequiredElementsDS.twksWSRequiredElements(0).TubeContent & "', " & vbCrLf

                    'ReagentID is informed only when the Element is a Reagent
                    If (pWSRequiredElementsDS.twksWSRequiredElements(0).IsReagentIDNull) Then
                        cmdText += " ReagentID = NULL, " & vbCrLf
                    Else
                        cmdText += " ReagentID = " & pWSRequiredElementsDS.twksWSRequiredElements(0).ReagentID & ", " & vbCrLf
                    End If

                    'SolutionCode is informed only when the Element is an Special Solution
                    '(Distilled Water or Saline Solution) or a Washing Solution
                    If (pWSRequiredElementsDS.twksWSRequiredElements(0).IsSolutionCodeNull) Then
                        cmdText += " SolutionCode = NULL, " & vbCrLf
                    Else
                        cmdText += " SolutionCode = '" & pWSRequiredElementsDS.twksWSRequiredElements(0).SolutionCode & "', " & vbCrLf
                    End If

                    'Volume is informed only when the Element is a Reagent or an Additional Solution
                    If (pWSRequiredElementsDS.twksWSRequiredElements(0).IsReagentIDNull AndAlso pWSRequiredElementsDS.twksWSRequiredElements(0).IsSolutionCodeNull) Then
                        cmdText += " RequiredVolume = NULL, " & vbCrLf
                    Else
                        cmdText += " RequiredVolume = " & ReplaceNumericString(pWSRequiredElementsDS.twksWSRequiredElements(0).RequiredVolume) & ", " & vbCrLf
                    End If

                    'CalibratorID is informed only when the Element is a Calibrator
                    If (pWSRequiredElementsDS.twksWSRequiredElements(0).IsCalibratorIDNull) Then
                        cmdText += " CalibratorID = NULL, " & vbCrLf
                    Else
                        cmdText += " CalibratorID = " & pWSRequiredElementsDS.twksWSRequiredElements(0).CalibratorID & ", " & vbCrLf
                    End If

                    'ControlID is informed only when the Element is a Control
                    If (pWSRequiredElementsDS.twksWSRequiredElements(0).IsControlIDNull) Then
                        cmdText += " ControlID = NULL, " & vbCrLf
                    Else
                        cmdText += " ControlID = " & pWSRequiredElementsDS.twksWSRequiredElements(0).ControlID & ", " & vbCrLf
                    End If

                    'MultiItemNumber is informed only when the Element is Reagent or Calibrator
                    'It will be the Reagent Number or the Calibrator Number respectively
                    If (pWSRequiredElementsDS.twksWSRequiredElements(0).TubeContent = "REAGENT" OrElse _
                        pWSRequiredElementsDS.twksWSRequiredElements(0).TubeContent = "CALIB") Then
                        If (Not pWSRequiredElementsDS.twksWSRequiredElements(0).IsReagentNumberNull) Then
                            cmdText += " MultiItemNumber = " & pWSRequiredElementsDS.twksWSRequiredElements(0).ReagentNumber & ", "

                        ElseIf (Not pWSRequiredElementsDS.twksWSRequiredElements(0).IsMultiItemNumberNull) Then
                            cmdText += " MultiItemNumber = " & pWSRequiredElementsDS.twksWSRequiredElements(0).MultiItemNumber & ", "
                        End If
                    Else
                        cmdText += " MultiItemNumber = NULL, " & vbCrLf
                    End If

                    'SampleType is informed only when the Element is a Patient Sample
                    If (pWSRequiredElementsDS.twksWSRequiredElements(0).IsSampleTypeNull) Then
                        cmdText += " SampleType = NULL, " & vbCrLf
                    Else
                        cmdText += " SampleType = '" & pWSRequiredElementsDS.twksWSRequiredElements(0).SampleType.Trim & "', " & vbCrLf
                    End If

                    'OrderID is informed only when the Element is a Patient Sample
                    If (pWSRequiredElementsDS.twksWSRequiredElements(0).IsOrderIDNull) Then
                        cmdText += " OrderID = NULL, " & vbCrLf
                    Else
                        cmdText += " OrderID = '" & pWSRequiredElementsDS.twksWSRequiredElements(0).OrderID.Trim & "', " & vbCrLf
                    End If

                    'PatientID is informed only when the Element is a Patient Sample
                    If (pWSRequiredElementsDS.twksWSRequiredElements(0).IsPatientIDNull) Then
                        cmdText += " PatientID = NULL, " & vbCrLf
                    Else
                        cmdText += " PatientID = N'" & pWSRequiredElementsDS.twksWSRequiredElements(0).PatientID.Trim & "', " & vbCrLf
                    End If

                    'SampleID is informed only when the Element is a Patient Sample
                    If (pWSRequiredElementsDS.twksWSRequiredElements(0).IsSampleIDNull) Then
                        cmdText += " SampleID = NULL, " & vbCrLf
                    Else
                        cmdText += " SampleID = N'" & pWSRequiredElementsDS.twksWSRequiredElements(0).SampleID.Trim.Replace("'", "''") & "', " & vbCrLf
                    End If

                    'Predilution Factor will be informed only when the Element corresponds a diluted SampleType
                    'for a Patient Sample
                    If (pWSRequiredElementsDS.twksWSRequiredElements(0).IsPredilutionFactorNull) Then
                        cmdText += " PredilutionFactor = NULL, " & vbCrLf
                    Else
                        cmdText += " PredilutionFactor = " & pWSRequiredElementsDS.twksWSRequiredElements(0).PredilutionFactor & ", " & vbCrLf
                    End If

                    'Element Status is always informed
                    cmdText += " ElementStatus = '" & pWSRequiredElementsDS.twksWSRequiredElements(0).ElementStatus & "' " & vbCrLf

                    'Set the Where by ElementID
                    cmdText += " WHERE ElementID = " & pWSRequiredElementsDS.twksWSRequiredElements(0).ElementID & vbCrLf

                    'Execute the SQL sentence 
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                        dataToReturn.HasError = False
                    End Using
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 08/03/2012</remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = " SELECT * FROM   twksWSRequiredElements " & vbCrLf & _
                                  " WHERE  WorkSessionID = '" & pWorkSessionID & "' "

                        Dim myDataSet As New WSRequiredElementsDS

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksWSRequiredElements)
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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.ReadAll", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


#End Region

#Region "Other Methods"

        ''' <summary>
        ''' When LIS sends demographics for a Patient previously added as SampleID to the active WS, the field PatientID is updated in the Order,
        ''' but if the SampleID already exists as a required Element, the PatientID has to be updated also in table twksWSRequiredElements  
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPatientID">Patient Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: SA 03/05/2013 
        ''' </remarks>
        Public Function ChangeSampleIDToPatientID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPatientID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksWSRequiredElements " & vbCrLf & _
                                            " SET    PatientID = N'" & pPatientID.Trim.Replace("'", "''") & "', " & vbCrLf & _
                                                   " SampleID  = NULL " & vbCrLf & _
                                            " WHERE  SampleID  = N'" & pPatientID.Trim.Replace("'", "''") & "' " & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.ChangeSampleIDToPatientID", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the number of Elements belonging to the specified Work Session that have been still non positioned 
        ''' in an Analyzer Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pRotorType">Analyzer Rotor Type. Optional parameter; when not informed, then all non positioned
        '''                          Elements (excepting Washing Solution Tubes) will be count, without filtering them by TubeContent</param>
        ''' <param name="pStatusDiffOfPOS">Optional parameter to indicate that all Elements with Status different of POS have 
        '''                                to be count</param>
        ''' <param name="pExcludePatients">TRUE only when called from automatic WS creation with LIS process</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of non positioned Work Session Elements</returns>
        ''' <remarks>
        ''' Created by:  TR 29/01/2010
        ''' Modified by: SA 27/07/2010 - Parameter Rotor Type is now optional. Added parameters to indicate the filter by ElementStatus
        '''                              should be different of POS.
        '''              RH 14/02/2011 - Code optimization.
        '''              RH 16/06/2011 - Added Tubes of Additional Solutions.
        '''              RH 31/08/2011 - Count INCOMPLETE elements also as NOPOS
        '''              SA 09/01/2012 - Filter not positioned Elements by field ElementFinished=FALSE to avoid autopositioning (or alert
        '''                              of not positioned Element) an element that is not required anymore; changed the function template.
        '''              TR 13/02/2012 - Removed Tube of Washing Solution to avoid count as not positioned the ISE Washing Solutions.
        '''              AG 16/07/2013 - Added new optional parameter pExcludePatients. When this parameter is TRUE (only when this function is called
        '''                              during process of Automatic WS Creation with LIS), not positioned Patient Samples Tubes are not included in 
        '''                              the total number of not positioned required Elements.
        '''              AG 04/06/2014 - #1519 - Ignore the WASHING SOLUTIONS that are not positioned (they do not shown warning 
        ''' </remarks>
        Public Function CountNotPositionedElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                   Optional ByVal pRotorType As String = "", Optional ByVal pStatusDiffOfPOS As Boolean = False, _
                                                   Optional ByVal pExcludePatients As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) FROM twksWSRequiredElements " & vbCrLf & _
                                                " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    ElementFinished = 0 " & vbCrLf

                        If (Not pStatusDiffOfPOS) Then
                            cmdText &= " AND (ElementStatus = 'NOPOS' OR ElementStatus = 'INCOMPLETE') " & vbCrLf
                        Else
                            cmdText &= " AND ElementStatus <> 'POS' " & vbCrLf
                        End If

                        If (pRotorType = "SAMPLES") Then
                            If (Not pExcludePatients) Then
                                'Normal case: all not positioned Elements in Samples Rotor are included in the total 
                                cmdText += " AND TubeContent IN ('CALIB', 'PATIENT','CTRL', 'TUBE_SPEC_SOL') " & vbCrLf
                            Else
                                'Case of Automatic WS Creation with LIS: not positioned Patient Samples Tubes are excluded from the total
                                cmdText += " AND TubeContent IN ('CALIB', 'CTRL', 'TUBE_SPEC_SOL') " & vbCrLf
                            End If

                        ElseIf (pRotorType = "REAGENTS") Then
                            'In Reagents Rotor, all not positioned Elements are included in the total
                            'cmdText += " AND TubeContent IN ('SPEC_SOL','REAGENT') " & vbCrLf 'AG 04/06/2014 - #1519 - Ignore the WASH SOL that are not positioned (they do not shown warning)

                            'AG 15/10/2014 BA-1519 error in previous change. the WASH_SOL has not to be excluded here because  when RotorType = REAGENTS during reagents autopositioning process (See bug number BA-1968)
                            cmdText += " AND TubeContent IN ('SPEC_SOL','REAGENT', 'WASH_SOL') " & vbCrLf

                        Else
                            If (Not pExcludePatients) Then
                                'Normal case: only Washing Solutions Tubes are excluded from the total
                                cmdText += " AND TubeContent NOT IN ('TUBE_WASH_SOL')"
                            Else
                                'Case of Automatic WS Creation with LIS: Washing Solutions and Patient Samples Tubes are excluded from the total
                                cmdText += " AND TubeContent NOT IN ('TUBE_WASH_SOL', 'PATIENT')"
                            End If
                        End If

                        'AG 04/06/2014 - #1519 - Ignore the WASHING SOLUTIONS that are not positioned (they do not shown warning
                        If pRotorType = String.Empty Then
                            cmdText += " AND TubeContent NOT IN ('WASH_SOL')"
                        End If

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar()
                            myGlobalDataTO.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.CountNotPositionedElements", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate the next ElementID for an specific Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing the generated ElementID</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 09/03/2010 - Changed the way of open the DB Connection to fulfill the new template
        '''                              Change the returned type to a GlobalDataTO 
        ''' </remarks>
        Public Function GenerateElementID(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO 'Integer
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Search the last ElementID for the informed WorkSession
                        Dim cmdText As String = " SELECT MAX(ElementID) AS NextElementID " & vbCrLf & _
                                                " FROM   twksWSRequiredElements "

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader
                            dbDataReader = dbCmd.ExecuteReader()

                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (dbDataReader.IsDBNull(0)) Then
                                    resultData.SetDatos = 1
                                Else
                                    resultData.SetDatos = CInt(dbDataReader.Item("NextElementID")) + 1
                                End If
                            End If
                            dbDataReader.Close()
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.GenerateElementID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if the informed Additional Solution exists as a Required Element in the specified Work Session and in this case, gets all 
        ''' data of the Element
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pTubeContent">Type of Additional Solution: Special, Washing, ...</param>
        ''' <param name="pSolutionCode">Additional Solution Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS with data of the Required Element when the informed Additional 
        '''          Solution already exists as an Element in the specified Work Session</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: VR 09/12/2009 - In cmdText, remove the ElementID with * - Tested: OK
        '''              VR 21/12/2009 - Changed the returned WSRequiredElementsTreeDS to GlobalDataTO - Tested: OK
        '''              SA 05/01/2010 - Changed the way of open the DB Connection to the new template
        '''              SA 05/01/2010 - DB Connection was bad informed (it used parameter instead of the local variable)
        '''              SA 09/01/2012 - Changed the function template
        ''' </remarks>
        Public Function GetAddittionalSolutionElementID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                        ByVal pTubeContent As String, ByVal pSolutionCode As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSRequiredElements " & vbCrLf & _
                                                " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    TubeContent = '" & pTubeContent & "' " & vbCrLf & _
                                                " AND    SolutionCode = '" & pSolutionCode.Trim & "' " & vbCrLf

                        Dim resultData As New WSRequiredElementsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRequiredElements)
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
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.GetAddittionalSolutionElementID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Verify if there is a required Element in the WorkSession for the specified PatientID/SampleID and optionally, an specific Sample Type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pPatientID">Patient or Sample Identifier</param>
        ''' <param name="pSampleType">Sample Type code. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS </returns>
        ''' <remarks>
        ''' Created by:  SA 10/04/2013
        ''' Modified by: SA 17/04/2013 - Deleted parameter pPatientExists. Changed the SQL Query to filter data by PatientID OR SampleID equal
        '''                              to the value specified by parameter pPatientID
        '''              TR 06/05/2013 - Changed the query by adding a filter by PredilutionFactor = NULL, to exclude required Elements created 
        '''                              for manual predilutions (due to manual predilutions should not be linked to positioned tubes after scanning)
        ''' </remarks>
        Public Function GetByPatientAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                  ByVal pPatientID As String, Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSRequiredElements " & vbCrLf & _
                                                " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    TubeContent   = 'PATIENT' " & vbCrLf & _
                                                " AND    PredilutionFactor IS NULL " & vbCrLf & _
                                                " AND   (PatientID     = N'" & pPatientID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " OR     SampleID      = N'" & pPatientID.Trim.Replace("'", "''") & "') " & vbCrLf

                        'Add a filter by SampleType if the optional parameter has been informed
                        If (pSampleType <> String.Empty) Then cmdText &= " AND SampleType = '" & pSampleType.Trim & "' "

                        Dim resultData As New WSRequiredElementsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRequiredElements)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.GetByPatientAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Verify if the informed Calibrator (or the specified Calibrator Point) exists as a Required Element in the specified Work Session, and 
        ''' in this case, gets all data of the correspondent Elements
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pCalibratorID">Calibrator Identifier</param>
        ''' <param name="pMultiItemNumber">Calibrator Point (for single point Calibrators: 1)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS. If a Calibrator Point has been specified (parameter pMultiItemNumber 
        '''          is informed), the DataSet will contain data of the correspondent Required Element; otherwise, the DataSet will contain data of all 
        '''          the Required Elements for the Calibrator (one for each Calibrator Point).  If the Calibrator has not been still added as a Required 
        '''          Element for the informed WorkSession, the DataSet is returned empty</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: VR 09/12/2009 - In cmdText, remove the ElementID with *
        '''              VR 10/12/2009 - Tested: OK
        '''              VR 21/12/2009 - Changed the returned WSRequiredElementsTreeDS to GlobalDataTO - Tested: OK
        '''              SA 05/01/2010 - Changed the way of open the DB Connection to the new template
        '''              SA 05/01/2010 - DB Connection was bad informed (it used parameter instead of the local variable)
        '''              SA 09/01/2012 - Changed the function template. Parameter pMultiItemNumber changed to optional: when it is not informed, then the 
        '''                              ElementID of all Calibrator points are returned, sorted by MultiItemNumber
        ''' </remarks>
        Public Function GetCalibratorElementID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                               ByVal pCalibratorID As Integer, Optional ByVal pMultiItemNumber As Integer = -1) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSRequiredElements " & vbCrLf & _
                                                " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    TubeContent = 'CALIB' " & vbCrLf & _
                                                " AND    CalibratorID = " & pCalibratorID.ToString() & vbCrLf

                        If (pMultiItemNumber <> -1) Then cmdText &= " AND MultiItemNumber = " & pMultiItemNumber.ToString() & vbCrLf
                        cmdText &= " ORDER BY MultiItemNumber " & vbCrLf

                        Dim resultData As New WSRequiredElementsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRequiredElements)
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
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.GetCalibratorElementID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of Required Elements for Calibrators for the specified TestID/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestDetailsRow">Row with structure of DataSet OrderTestsDetailsDS containing TestID and SampleType</param>
        ''' <param name="pPointNumber">When informed, indicates the query has to be filtered by the specified Multipoint Number instead of 
        '''                            get all Calibrator Points. Optional parameter that will be informed only for Special Test HbTotal</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSElementsByOrderTestDS with the list of Required Calibrator Elements for the 
        '''          specified TestID/SampleType</returns>
        ''' <remarks>
        ''' Created by:  SA
        ''' Modified by: SA 05/01/2010 - Changed the way of open the DB Connection to the new template; changes to return a GlobalDataTO instead 
        '''                              a typed DataSet WSElementsByOrderTestDS
        '''              SA 30/08/2010 - Added new optional parameter pPointNumber to allow filter the query for an specific MultiPointNumber  
        '''                              (needed for management of Tests HbA1C and HbTotal in the WS)
        '''              SA 10/01/2012 - Changed the function template
        ''' </remarks>
        Public Function GetCalibratorElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                              ByVal pOrderTestDetailsRow As OrderTestsDetailsDS.OrderTestsDetailsRow, _
                                              Optional ByVal pPointNumber As Integer = -1) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT RE.ElementID " & vbCrLf & _
                                                " FROM   twksWSRequiredElements RE INNER JOIN tparTestCalibrators TC ON RE.CalibratorID = TC.CalibratorID " & vbCrLf & _
                                                " WHERE  RE.WorkSessionID   = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    RE.TubeContent     = 'CALIB' " & vbCrLf & _
                                                " AND    TC.TestID          = " & pOrderTestDetailsRow.TestID & vbCrLf & _
                                                " AND    TC.SampleType      = '" & pOrderTestDetailsRow.SampleType.Trim & "' " & vbCrLf

                        If (pPointNumber <> -1) Then cmdText &= " AND RE.MultiItemNumber = " & pPointNumber & vbCrLf

                        Dim resultData As New WSElementsByOrderTestDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRequiredElemByOrderTest)
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
                myLogAcciones.CreateLogActivity(ex.Message, "WSRequiredElementsDelegate.GetCalibratorElements", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Verify if the informed Control exists as a Required Element in the specified Work Session and in this case, gets all data of the Element
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS with data of the Required Element when the informed Control 
        '''          already exists as an Element in the specified Work Session</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: VR 09/12/2009 - In cmdText, change the ElementID with * 
        '''              VR 10/12/2009 - Tested: OK 
        '''              VR 21/12/2009 - Changed the returned WSRequiredElementsTreeDS to GlobalDataTO - Testing: PENDING
        '''              SA 05/01/2010 - Changed the way of open the DB Connection to the new template
        '''              SA 05/01/2010 - DB Connection was bad informed (it used parameter instead of the local variable)
        '''              SA 09/01/2012 - Changed the function template
        ''' </remarks>
        Public Function GetControlElementID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pControlID As Integer) _
                                            As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSRequiredElements " & vbCrLf & _
                                                " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    TubeContent = 'CTRL' " & vbCrLf & _
                                                " AND    ControlID = " & pControlID.ToString() & vbCrLf

                        Dim resultData As New WSRequiredElementsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRequiredElements)
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
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.GetControlElementID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the Element Identifier for each point of an Experimental Calibrator
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAlternativeOrderTestID"></param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS with the list of ElementIDs for the Experimental
        '''          Calibrator for the TestID/SampleType of the specified Order Test</returns>
        ''' <remarks>
        ''' Created by:  SA 17/02/2011
        ''' Modified by: SA 10/01/2012 - Changed the function template
        '''              SA 19/04/2012 - Changed the query by adding a filter by Standard Tests
        ''' </remarks>
        Public Function GetElemIDForAlternativeCalibrator(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                          ByVal pAlternativeOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT RE.ElementID " & vbCrLf & _
                                                " FROM   twksWSRequiredElements RE INNER JOIN tparTestCalibrators TC ON RE.CalibratorID = TC.CalibratorID " & vbCrLf & _
                                                                                 " INNER JOIN vwksWSOrderTests WSOT ON TC.TestID = WSOT.TestID AND TC.SampleType = WSOT.SampleType " & vbCrLf & _
                                                " WHERE  RE.TubeContent = 'CALIB' " & vbCrLf & _
                                                " AND    WSOT.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    WSOT.OrderTestID   = " & pAlternativeOrderTestID & vbCrLf & _
                                                " AND    WSOT.ToSendFlag    = 1 " & vbCrLf & _
                                                " AND    WSOT.OpenOTFlag    = 0 " & vbCrLf & _
                                                " AND    WSOT.TestType      = 'STD' " & vbCrLf

                        Dim myWSRequiredElementsDS As New WSRequiredElementsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myWSRequiredElementsDS.twksWSRequiredElements)
                            End Using
                        End Using

                        resultData.SetDatos = myWSRequiredElementsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.GetElemIDForAlternativeCalibrator", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get required elements for a patient where specimen identifier has value
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS</returns>
        ''' <remarks>
        ''' Created by:  DL 14/06/2013
        ''' Modified by: SG 17/07/2013 - Changed the SQL by adding a DISTINCT clause to avoid get duplicated values of SpecimenIDList field when 
        '''                              there are manual predilutions for the same tube
        ''' </remarks>
        Public Function GetLISPatientElements(ByVal pDBConnection As SqlClient.SqlConnection,
                                              ByVal pWorkSessionID As String) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT (CASE WHEN PatientID IS NULL THEN SampleID ELSE PatientID END) AS PatientID, " & vbCrLf & _
                                                                " SpecimenIDList, SampleType" & vbCrLf & _
                                                " FROM   twksWSRequiredElements " & vbCrLf & _
                                                " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    TubeContent   = 'PATIENT' " & vbCrLf & _
                                                " AND    SpecimenIDList IS NOT NULL " & vbCrLf

                        Dim resultData As New WSRequiredElementsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRequiredElements)
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
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.GetLISPatientElements", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' For a required Element corresponding to a Calibrator, search if there are other related required Elements (case of 
        ''' MultiPoint Calibrators)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Identifier of the required Calibrator Element</param>
        ''' <param name="pCalibratorID">Identifier of the Calibrator</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS with the Element Identifier of all points 
        '''          of the informed Calibrator</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: SA 23/12/2009 - Throw ex when an exception happens was removed (error is returned inside the GlobalDataTO)
        '''              SA 11/01/2010 - Changed the way of open the DB Connection to fulfill the new template 
        '''              AG 10/10/2011 - Changed the query to get also field ElementStatus
        '''              SA 09/01/2012 - Changed the query to get also fields MultiItemNumber and ElementFinished; changed the function template
        ''' </remarks>
        Public Function GetMultipointCalibratorElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                        ByVal pElementID As Integer, ByVal pCalibratorID As Integer) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT ElementID, MultiItemNumber, ElementStatus, ElementFinished " & vbCrLf & _
                                                " FROM   twksWSRequiredElements " & vbCrLf & _
                                                " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    CalibratorID  = " & pCalibratorID & vbCrLf & _
                                                " AND    ElementID    <> " & pElementID & vbCrLf

                        Dim resultData As New WSRequiredElementsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRequiredElements)
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
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.GetMultipointCalibratorElements", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get the list of all required Elements of the specified Work Session that have not been still positioned 
        ''' in the correspondent Analyzer Rotor (of all Tube Contents excepting Patient Samples)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSNoPosRequiredElements with the list of required Elements of the specified
        '''          Work Session that have not been still positioned in the correspondent Analyzer Rotor</returns>
        ''' <remarks>
        ''' Created by:  SA 26/07/2010
        ''' Modified by: PG 19/10/2010 - Modified query because it must show the ResourceText
        '''              RH 16/06/2011 - Introduce TUBE_SPEC_SOL and TUBE_WASH_SOL, the Using statement and some code optimizations
        '''              SA 09/01/2012 - Changed subqueries to get only elements not marked as finished; changed the function template
        '''              TR 13/03/2012 - In subquery for Washing Solutions (in both, Reagents and Samples Rotors), exclude the ISE
        '''                              Washing Solution (code WASHSOL3)
        '''              SA 03/05/2012 - Changed subquery for Special Solutions: the description can be also in SubTable DIL_SOLUTIONS in 
        '''                              Preloaded Master Data, not only in SubTable SPECIAL_SOLUTIONS
        '''              TR 21/11/2013 - BT #1388 ==> Changed all sub-queries to return also field ElementID
        '''              SA 27/05/2014 - BT #1519 ==> Changed the sub-queries used to get the not positioned REAGENTS and the not positioned DILUTION SOLUTIONS 
        '''                                           to return only elements still needed in the active Work Session (those needed for NOT CLOSED Order Tests). 
        '''                                           Created a new sub-query to get the not positioned SPECIAL SOLUTIONS needed for Blanks (previouly they were 
        '''                                           obtained in the same sub-query than DILUTION SOLUTIONS). Removed the getting of not positioned WASHING 
        '''                                           SOLUTIONS from this function (they are obtained in a different function called from the function with the 
        '''                                           same name in the Delegate Class) 
        ''' </remarks>
        Public Function GetNotPositionedElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim var As New GlobalBase

                        '(1) BT #1519 - Subquery to get REAGENTS that are not positioned (or without enough volume) but that are still needed in the active Work Session
                        Dim cmdText As String = " SELECT RE.TubeContent AS SampleClass, R.ReagentName AS SampleName, NULL AS SampleType, 1 AS Position, RE.ElementID " & vbCrLf & _
                                                " FROM   twksWSRequiredElements RE INNER JOIN tparReagents R ON RE.ReagentID = R.ReagentID " & vbCrLf & _
                                                                                 " INNER JOIN twksWSRequiredElemByOrderTest REOT ON RE.ElementID = REOT.ElementID " & vbCrLf & _
                                                                                 " INNER JOIN twksOrderTests OT ON REOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                " WHERE  RE.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    RE.ElementStatus <> 'POS'" & vbCrLf & _
                                                " AND    RE.TubeContent = 'REAGENT' " & vbCrLf & _
                                                " AND    OT.OrderTestStatus <> 'CLOSED' " & vbCrLf

                        '(2) BT #1519 - Subquery to get DILUTION SOLUTIONS that are not positioned (or without enough volume) but that are still needed in the active 
                        '               Work Session (for automatic predilutions)
                        cmdText &= " UNION " & vbCrLf & _
                                   " SELECT DISTINCT RE.TubeContent AS SampleClass, MR.ResourceText AS SampleName, NULL AS SampleType, 2 AS Position, RE.ElementID " & vbCrLf & _
                                   " FROM   twksWSRequiredElements RE INNER JOIN tfmwPreloadedMasterData PMD ON RE.SolutionCode = PMD.ItemID " & vbCrLf & _
                                                                    " INNER JOIN tfmwMultiLanguageResources MR ON MR.ResourceID = PMD.ResourceID " & vbCrLf & _
                                                                    " INNER JOIN twksWSRequiredElemByOrderTest REOT ON RE.ElementID = REOT.ElementID " & vbCrLf & _
                                                                    " INNER JOIN twksOrderTests OT ON REOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                   " WHERE RE.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                   " AND   RE.ElementStatus <> 'POS' " & vbCrLf & _
                                   " AND   RE.ElementFinished = 0 " & vbCrLf & _
                                   " AND   RE.TubeContent = 'SPEC_SOL' " & vbCrLf & _
                                   " AND   PMD.SubTableID = '" & GlobalEnumerates.PreloadedMasterDataEnum.DIL_SOLUTIONS.ToString & "' " & vbCrLf & _
                                   " AND   MR.LanguageID = '" & var.GetSessionInfo.ApplicationLanguage & "' " & vbCrLf & _
                                   " AND   OT.OrderTestStatus <> 'CLOSED' " & vbCrLf

                        '(3) BT #1519 - Subquery to get SPECIAL SOLUTIONS (needed for Blanks) that are not positioned (or without enough volume) but that are still needed 
                        '               in the active Work Session
                        cmdText &= " UNION " & vbCrLf & _
                                   " SELECT RE.TubeContent AS SampleClass, MR.ResourceText AS SampleName, NULL AS SampleType, 2 AS Position, RE.ElementID " & vbCrLf & _
                                   " FROM   twksWSRequiredElements RE INNER JOIN tfmwPreloadedMasterData PMD ON RE.SolutionCode = PMD.ItemID  " & vbCrLf & _
                                                                    " INNER JOIN tfmwMultiLanguageResources MR ON MR.ResourceID = PMD.ResourceID " & vbCrLf & _
                                   " WHERE RE.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                   " AND   RE.ElementStatus <> 'POS' " & vbCrLf & _
                                   " AND   RE.ElementFinished = 0 " & vbCrLf & _
                                   " AND   RE.TubeContent = 'TUBE_SPEC_SOL' " & vbCrLf & _
                                   " AND   PMD.SubTableID = '" & GlobalEnumerates.PreloadedMasterDataEnum.SPECIAL_SOLUTIONS.ToString & "' " & vbCrLf & _
                                   " AND   MR.LanguageID = '" & var.GetSessionInfo.ApplicationLanguage & "' " & vbCrLf

                        ''(4) BT #1519 - Subquery to get WASHING SOLUTIONS that are not positioned (or without enough volume) but that are still needed in the active Work Session
                        ''               (to avoid Contaminations between Reagents)
                        'cmdText &= " UNION " & vbCrLf & _
                        '           " SELECT DISTINCT RE.TubeContent AS SampleClass, MR.ResourceText AS SampleName, NULL AS SampleType, 3 AS Position, RE.ElementID " & vbCrLf & _
                        '           " FROM   twksWSRequiredElements RE INNER JOIN tfmwPreloadedMasterData PMD ON RE.SolutionCode = PMD.ItemID " & vbCrLf & _
                        '                                            " INNER JOIN tfmwMultiLanguageResources MR ON MR.ResourceID = PMD.ResourceID " & vbCrLf & _
                        '                                            " INNER JOIN twksWSRequiredElemByOrderTest REOT ON RE.ElementID = REOT.ElementID " & vbCrLf & _
                        '                                            " INNER JOIN twksOrderTests OT ON REOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                        '           " WHERE  RE.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                        '           " AND    RE.ElementStatus <> 'POS' " & vbCrLf & _
                        '           " AND    RE.ElementFinished = 0 " & vbCrLf & _
                        '           " AND    RE.TubeContent = 'WASH_SOL' " & vbCrLf & _
                        '           " AND    PMD.SubTableID = '" & GlobalEnumerates.PreloadedMasterDataEnum.WASHING_SOLUTIONS.ToString & "' " & vbCrLf & _
                        '           " AND    MR.LanguageID = '" & var.GetSessionInfo.ApplicationLanguage & "'" & vbCrLf & _
                        '           " AND    OT.OrderTestStatus <> 'CLOSED' " & vbCrLf

                        '(5) Subquery to get CALIBRATORS that are not positioned (or without enough volume) but that are still needed in the active Work Session
                        cmdText &= " UNION " & vbCrLf & _
                                   " SELECT DISTINCT RE.TubeContent AS SampleClass, C.CalibratorName + ' (' + CONVERT(NVARCHAR(1), C.NumberOfCalibrators) + ')' AS SampleName, " & vbCrLf & _
                                                   " NULL AS SampleType, 4 AS Position, NULL As ElementID " & vbCrLf & _
                                   " FROM   twksWSRequiredElements RE INNER JOIN tparCalibrators C ON RE.CalibratorID = C.CalibratorID " & vbCrLf & _
                                   " WHERE  RE.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                   " AND    RE.ElementStatus <> 'POS' " & vbCrLf & _
                                   " AND    RE.ElementFinished = 0 " & vbCrLf & _
                                   " AND    RE.TubeContent = 'CALIB' " & vbCrLf & _
                                   " AND    RE.ElementFinished = 0 " & vbCrLf

                        '(5) Subquery to get CONTROLS that are not positioned (or without enough volume) but that are still needed in the active Work Session
                        cmdText &= " UNION " & vbCrLf & _
                                   " SELECT DISTINCT RE.TubeContent AS SampleClass, C.ControlName AS SampleName, NULL AS SampleType, 5 AS Position, NULL As ElementID " & vbCrLf & _
                                   " FROM   twksWSRequiredElements RE INNER JOIN tparControls C ON RE.ControlID = C.ControlID " & vbCrLf & _
                                   " WHERE  RE.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                   " AND    RE.ElementStatus <> 'POS' " & vbCrLf & _
                                   " AND    RE.ElementFinished = 0 " & vbCrLf & _
                                   " AND    RE.TubeContent = 'CTRL' " & vbCrLf & _
                                   " AND    RE.ElementFinished = 0 " & vbCrLf & _
                                   " ORDER BY Position, SampleName "

                        Dim myNoPosElements As New WSNoPosRequiredElementsDS()
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myNoPosElements.twksWSRequiredElements)
                            End Using
                        End Using

                        resultData.SetDatos = myNoPosElements
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.GetNotPositionedElements", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all not positioned tubes for manual predilution of Patient Samples needed for Tests requested by LIS. Used in the process of 
        ''' Automatic WS Creation with LIS to stop the process before enter in Running to allow the final User to prepare and place in Samples
        ''' Rotor all tubes with manual predilutions needed in the Work Session (for instance, all prediluted Urine tubes for all ISE Tests
        ''' requested by LIS) 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS with all information of not positioned tubes for manual predilution 
        '''          of Patient Samples needed for Tests requested by LIS</returns>
        ''' <remarks>
        ''' Created by:  SA 29/01/2014 - BT #1474
        ''' </remarks>
        Public Function GetNotPositionedPredilutionSamples(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT RE.* " & vbCrLf & _
                                                " FROM   twksWSRequiredElements RE INNER JOIN twksWSRequiredElemByOrderTest REOT ON RE.ElementID = REOT.ElementID " & vbCrLf & _
                                                                                 " INNER JOIN twksOrderTests OT ON REOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                " WHERE  RE.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    RE.TubeContent   = 'PATIENT' " & vbCrLf & _
                                                " AND    RE.PredilutionFactor IS NOT NULL " & vbCrLf & _
                                                " AND    RE.ElementFinished = 0 " & vbCrLf & _
                                                " AND    RE.ElementStatus <> 'POS' " & vbCrLf & _
                                                " AND    OT.LISRequest = 1 " & vbCrLf

                        Dim resultData As New WSRequiredElementsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRequiredElements)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.GetNotPositionedPredilutionSamples", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of Required Elements for Patient's Samples for the specified SampleType and PatientID (or OrderID, for Patient's 
        ''' Orders in which the Patient is not informed)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestDetailsRow">Row with structure of DataSet OrderTestsDetailsDS containing
        '''                                    SampleType and PatientID/OrderID</param>
        ''' <param name="pOnlyForISE">When informed (value is different of -1) then data is filtered also by value 
        '''                           of field OnlyForISE</param>
        ''' <param name="pIgnorePredilutionFactor">When informed, filters that depend on value of field PredilutionFactor are not applied</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDSwith the list of Required Patient  
        '''          Sample Elements for the specified SampleType/PatientID (or OrderID)</returns>
        ''' <remarks>
        ''' Created by: 
        ''' Modified by: VR 09/12/2009 - Changed the returned WSElementsByOrderTestDS to GlobalDataTO
        '''              VR 10/12/2009 - Change ElementID to * - Tested: OK
        '''              VR 21/12/2009 - Tested: OK
        '''              SA 05/01/2010 - Changed the way of open the DB Connection to the new template 
        '''              SA 05/01/2010 - DB Connection was bad informed (it used parameter instead of the local variable); 
        '''                              verification of PatientID or OrderID not informed was bad (!= "" instead of Is..Null)      
        '''              AG 14/01/2010 - In where SELECT include always predilution factor (Tested OK)
        '''              SA 09/03/2010 - Changes to add the filter by SampleID when this field is informed
        '''              SA 14/10/2010 - When field PredilutionFactor is informed, apply the filter only for manual dilutions 
        '''                              (that means, if value of field PredilutionMode is different of INST) 
        '''              SA 26/10/2010 - Changes to allow having two manual dilutions with the same factor for the same Patient Sample
        '''                              when one of them is used exclusively for ISE Tests
        '''              SA 04/11/2010 - Add N preffix for multilanguage of fields PatientID and SampleID
        '''              SA 27/01/2011 - When field PredilutionFactor is informed, but PredilutionMode is INST (automatic dilution)
        '''                              filter the query to get the Patient Sample Element with PredilutionFactor NULL
        '''              SA 01/09/2011 - Added optional parameter pIgnorePredilutionFactor
        '''              SA 10/01/2012 - Changed the function template
        ''' </remarks>
        Public Function GetPatientElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                           ByVal pOrderTestDetailsRow As OrderTestsDetailsDS.OrderTestsDetailsRow, _
                                           Optional ByVal pOnlyForISE As Integer = -1, Optional ByVal pIgnorePredilutionFactor As Boolean = False) _
                                           As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSRequiredElements " & vbCrLf & _
                                                " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    TubeContent   = 'PATIENT' " & vbCrLf & _
                                                " AND    SampleType    = '" & pOrderTestDetailsRow.SampleType.Trim & "' " & vbCrLf

                        'Add filter by PatientID, SampleID or OrderID
                        If (Not pOrderTestDetailsRow.IsPatientIDNull AndAlso pOrderTestDetailsRow.PatientID <> "") Then
                            cmdText = cmdText & " AND PatientID = N'" & pOrderTestDetailsRow.PatientID.Trim & "' " & vbCrLf

                        ElseIf (Not pOrderTestDetailsRow.IsSampleIDNull AndAlso pOrderTestDetailsRow.SampleID <> "") Then
                            cmdText = cmdText & " AND SampleID = N'" & pOrderTestDetailsRow.SampleID.Trim.Replace("'", "''") & "' " & vbCrLf
                        Else
                            cmdText = cmdText & " AND OrderID = '" & pOrderTestDetailsRow.OrderID.Trim & "' " & vbCrLf
                        End If

                        'Add filter by Predilution Factor when informed and the Predilution is MANUAL
                        If (Not pIgnorePredilutionFactor) Then
                            If (Not pOrderTestDetailsRow.IsPredilutionFactorNull()) Then
                                If (pOrderTestDetailsRow.PredilutionMode <> "INST") Then
                                    cmdText &= " AND PredilutionFactor = " & ReplaceNumericString(pOrderTestDetailsRow.PredilutionFactor) & vbCrLf
                                Else
                                    cmdText &= " AND PredilutionFactor is NULL " & vbCrLf
                                End If
                            Else
                                cmdText &= " AND PredilutionFactor is NULL " & vbCrLf
                            End If
                        End If

                        'Add filter by field OnlyForISE (True for URI dilutions needed for ISE Tests)
                        If (pOnlyForISE <> -1) Then
                            cmdText &= " AND OnlyForISE = " & pOnlyForISE & vbCrLf
                        End If

                        Dim resultData As New WSRequiredElementsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRequiredElements)
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
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.GetPatientElements", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For a Required Element of type Reagent, get the volume programmed for each Test/SampleType using it
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Identifier of the required Reagent Element</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS with the Volume of the informed Reagent
        '''          required for every different TestID/SampleType in the Work Session</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: SA 23/12/2009 - Throw ex when an exception happens was removed (error is returned inside the GlobalDataTO)
        '''              SA 11/01/2010 - Changed the way of open the DB Connection to fulfill the new template 
        '''              SA 09/01/2012 - Changed the function template
        '''              SA 15/02/2012 - Query format changed to ANSI
        '''              SA 27/02/2012 - Changed the query to get also the currently saved Required Elements
        '''              SA 19/04/2012 - Changed the query by adding a filter by Standard Tests
        '''              XB 20/03/2014 - Add parameter HResult into Try Catch section - #1548
        ''' </remarks>
        Public Function GetProgrammedReagentVol(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pElementID As Integer) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT RE.RequiredVolume, RV.ReagentVolume, OT.TestID, OT.SampleType " & vbCrLf & _
                                                " FROM   twksWSRequiredElements RE INNER JOIN twksWSRequiredElemByOrderTest RO ON RE.ElementID = RO.ElementID " & vbCrLf & _
                                                                                 " INNER JOIN tparTestReagentsVolumes RV ON RE.ReagentID = RV.ReagentID " & vbCrLf & _
                                                                                                                      " AND RE.MultiItemNumber = RV.ReagentNumber " & vbCrLf & _
                                                                                 " INNER JOIN twksOrderTests OT ON RO.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                                                             " AND RV.TestID = OT.TestID AND RV.SampleType = OT.SampleType " & vbCrLf & _
                                                " WHERE  RE.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    RE.ElementID     = " & pElementID.ToString & vbCrLf & _
                                                " AND    OT.TestType      = 'STD' " & vbCrLf

                        Dim resultData As New WSRequiredElementsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRequiredElements)
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
                dataToReturn.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "twksWSRequiredElementsDAO.GetProgrammedReagentVol", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Verify if the informed Reagent exists as a Required Element in the specified Work Session
        ''' and in this case, gets all data of the Element
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pReagentID">Reagent Identifier</param>
        ''' <param name="pMultiItemNumber">Reagent Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS with data of the
        '''          Required Element when the informed Reagent already exists as an Element in the
        '''          specified Work Session</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: BK 09/12/2009 - In cmdText, remove the ElementID with *
        '''              VR 10/12/2009 - Tested: OK 
        '''              VR 21/12/2009 - Changed the returned WSRequiredElementsTreeDS to GlobalDataTO - Tested: OK
        '''              SA 05/01/2010 - Changed the way of open the DB Connection to the new template
        '''              SA 05/01/2010 - Query was bad written; DB Connection was bad informed (it used parameter instead of the local variable)
        '''              SA 09/01/2012 - Changed the function template
        ''' </remarks>
        Public Function GetReagentElementID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                            ByVal pReagentID As Integer, ByVal pMultiItemNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSRequiredElements " & vbCrLf & _
                                                " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    TubeContent = 'REAGENT' " & vbCrLf & _
                                                " AND    ReagentID = " & pReagentID.ToString() & vbCrLf & _
                                                " AND    MultiItemNumber = " & pMultiItemNumber.ToString() & vbCrLf

                        Dim resultData As New WSRequiredElementsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRequiredElements)
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
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.GetReagentElementID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of Required Elements for Reagents for the specified TestID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestDetailsRow">Row with structure of DataSet OrderTestsDetailsDS containing
        '''                                    TestID</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSElementsByOrderTestDS with the list of Required Reagent 
        '''          Elements for the specified TestID</returns>
        ''' <remarks>
        ''' Created by:  SA
        ''' Modified by: SA 05/01/2010 - Changed the way of open the DB Connection to the new template; changes to return a 
        '''                              GlobalDataTO instead a typed DataSet WSElementsByOrderTestDS   
        '''              SA 09/01/2012 - Changed the function template
        ''' </remarks>
        Public Function GetReagentElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                           ByVal pOrderTestDetailsRow As OrderTestsDetailsDS.OrderTestsDetailsRow) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT RP.ElementID " & vbCrLf & _
                                                " FROM   twksWSRequiredElements RP INNER JOIN tparTestReagents TR ON RP.ReagentID = TR.ReagentID " & vbCrLf & _
                                                                                                               " AND RP.MultiItemNumber = TR.ReagentNumber " & vbCrLf & _
                                                " WHERE  RP.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    RP.TubeContent   = 'REAGENT' " & vbCrLf & _
                                                " AND    TR.TestID        = " & pOrderTestDetailsRow.TestID & vbCrLf

                        Dim resultData As New WSElementsByOrderTestDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRequiredElemByOrderTest)
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
                myLogAcciones.CreateLogActivity(ex.Message, "WSRequiredElementsDelegate.GetReagentElements", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get details of all Calibrators needed for a Work Session.  Optionally, this function can get details of just one Calibrator, 
        ''' or get details of all Calibrators that have an specified Status (positioned or non positioned). Multipoint Calibrators will 
        ''' be returned as an unique row in the DataSet indicating the total number of points
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Optional parameter. When informed, it allows get only the data of the specified Calibrator</param>
        ''' <param name="pElementStatus">Optional parameter. When informed, it allows get only the data of required Calibrator having the specified 
        '''                              Status (POS or NOPOS for Calibrators)</param>
        ''' <param name="pOnlyNotFinished">Optional parameter. When value of this parameter is TRUE, only Calibrators not marked as finished are returned</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsTreeDS with information obtained stored in Calibrators table</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: SA 23/12/2009 - Throw ex when an exception happens was removed (error is returned inside the GlobalDataTO)
        '''                            - Changed the Query to include field MultiItemNumber in the ORDER BY 
        '''              SA 11/01/2010 - Changed the way of open the DB Connection to fulfill the new template
        '''              SA 18/03/2010 - Get also value of field TubeType from table of Required Elements
        '''              SA 09/01/2012 - Added optional parameter to allow get only not finished Calibrators; changed the function template
        '''              SA 07/02/2012 - When filter by ElementStatus is informed with value NOPOS, verify the Calibrator is not positioned
        '''                              in Samples Rotor (the Calibrator can have a NOPOS status having the tubes placed in the Rotor when
        '''                              at least one of the tubes is marked as DEPLETED or with FEW volume)
        ''' </remarks>
        Public Function GetRequiredCalibratorsDetails(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, Optional ByVal pElementID As Integer = 0, _
                                                      Optional ByVal pElementStatus As String = "", Optional ByVal pOnlyNotFinished As Boolean = False) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT RE.TubeContent, RE.ElementID, RE.CalibratorID, C.CalibratorName, RE.MultiItemNumber AS CalibratorNumber, " & vbCrLf & _
                                                       " RE.ElementStatus, RE.TubeType, C.LotNumber, C.ExpirationDate " & vbCrLf & _
                                                " FROM   twksWSRequiredElements RE INNER JOIN tparCalibrators C ON C.CalibratorID  = RE.CalibratorID " & vbCrLf & _
                                                " WHERE  RE.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    RE.TubeContent   = 'CALIB' " & vbCrLf

                        'Link to the Query the optional parameters when they are informed 
                        If (pElementID > 0) Then cmdText &= " AND RE.ElementID = " & pElementID & vbCrLf
                        If (pElementStatus <> "") Then
                            cmdText &= " AND RE.ElementStatus = '" & pElementStatus & "' " & vbCrLf
                            If (pElementStatus = "NOPOS") Then
                                cmdText &= " AND RE.ElementID NOT IN (SELECT ElementID FROM twksWSRotorContentByPosition " & vbCrLf & _
                                                                    " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                                    " AND    RotorType     = 'SAMPLES' " & vbCrLf & _
                                                                    " AND    ElementID IS NOT NULL) " & vbCrLf
                            End If
                        End If

                        If (pOnlyNotFinished) Then cmdText &= " AND RE.ElementFinished = 0 " & vbCrLf
                        cmdText &= " ORDER BY C.CalibratorName, RE.MultiItemNumber "

                        Dim resultData As New WSRequiredElementsTreeDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.Calibrators)
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
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.GetRequiredCalibratorsDetails", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get details of all the Controls included in the specified Work Session or, optionally, get details of an specific Control or of 
        ''' all Controls that have the specified status
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Optional parameter. When informed, it allows get only the data of the specified Control</param>
        ''' <param name="pElementStatus">Optional parameter. When informed, it allows get only the data of required Controls having the specified 
        '''                              Status (POS or NOPOS for Controls)</param>
        ''' <param name="pOnlyNotFinished">Optional parameter. When value of this parameter is TRUE, only Controls not marked as finished are returned</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsTreeDS with information obtained stored in Controls table</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: SA 23/12/2009 - Throw ex when an exception happens was removed (error is returned inside the GlobalDataTO)
        '''              SA 11/01/2010 - Changed the way of open the DB Connection to fulfill the new template 
        '''              SA 18/03/2010 - Get also value of field TubeType from table of Required Elements
        '''              SA 09/01/2012 - Added optional parameter to allow get only not finished Controls; changed the function template 
        '''              SA 07/02/2012 - When filter by ElementStatus is informed with value NOPOS, verify the Control is not positioned
        '''                              in Samples Rotor (the Control can have a NOPOS status having a tube placed in the Rotor when
        '''                              it is marked as DEPLETED or with FEW volume)
        ''' </remarks>
        Public Function GetRequiredControlsDetails(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, Optional ByVal pElementID As Integer = 0, _
                                                   Optional ByVal pElementStatus As String = "", Optional ByVal pOnlyNotFinished As Boolean = False) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT RE.TubeContent, RE.ElementID, RE.ControlID, QC.ControlName," & vbCrLf & _
                                                       " RE.ElementStatus, RE.TubeType, QC.LotNumber, QC.ExpirationDate " & vbCrLf & _
                                                " FROM   twksWSRequiredElements RE INNER JOIN tparControls QC ON RE.ControlID  = QC.ControlID " & vbCrLf & _
                                                " WHERE  RE.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    RE.TubeContent   = 'CTRL' " & vbCrLf

                        'Link to the Query the optional parameters when they are informed 
                        If (pElementID <> 0) Then cmdText &= " AND RE.ElementID = " & pElementID & vbCrLf
                        If (pElementStatus <> "") Then
                            cmdText &= " AND RE.ElementStatus = '" & pElementStatus & "' " & vbCrLf
                            If (pElementStatus = "NOPOS") Then
                                cmdText &= " AND RE.ElementID NOT IN (SELECT ElementID FROM twksWSRotorContentByPosition " & vbCrLf & _
                                                                    " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                                    " AND    RotorType     = 'SAMPLES' " & vbCrLf & _
                                                                    " AND    ElementID IS NOT NULL) "
                            End If
                        End If
                        If (pOnlyNotFinished) Then cmdText &= " AND RE.ElementFinished = 0 " & vbCrLf
                        cmdText &= " ORDER BY QC.ControlName "

                        Dim resultData As New WSRequiredElementsTreeDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.Controls)
                            End Using
                        End Using

                        dataToReturn.SetDatos = resultData
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.GetRequiredControlsDetails", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get details of all Patient Samples needed for a Work Session.  Optionally, this function can get details of just one Patient Sample, 
        ''' or get detaills of all Patient Samples that have an specified Status (positioned or non positioned). This function also verify if 
        ''' there is at least one Stat Order for each different PatientID/OrderID and SampleType; if there is at least one, then the Patient Sample 
        ''' will be marked as required for Stat in the WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Optional parameter. When informed, it allows get only the data of the specified Patient Sample</param>
        ''' <param name="pElementStatus">Optional parameter. When informed, it allows get only the data of required Patient Samples having the specified 
        '''                              Status (POS or NOPOS for Patient Samples)</param>
        ''' <param name="pOnlyNotFinished">Optional parameter. When value of this parameter is TRUE, only Patient Samples not marked as finished are returned</param>
        ''' <param name="pGetAllNoPos">Optional parameter. When value of this parameter is TRUE, Patient Samples marked as Not Positioned and having DEPLETED or FEW
        '''                            tubes positioned in the Rotor are also returned; otherwise, they are not returned (this is needed in positioning function)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsTreeDS with information obtained stored in Patients table</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: SA 23/12/2009 - Throw ex when an exception happens was removed (error is returned inside the GlobalDataTO)
        '''              SA 11/01/2010 - Changed the way of open the DB Connection to fulfill the new template 
        '''              SA 09/03/2010 - Changes needed to return also the field SampleID
        '''              SA 18/03/2010 - Get also value of field TubeType from table of Required Elements
        '''              SA 22/10/2010 - Get also value of field OnlyForISE from table of Required Elements
        '''              TR 22/09/2011 - Added filters when parameter pExcludedDepleted has been set to True
        '''              SA 09/11/2011 - Patient Samples are sorted by ElementID (to return them according the order in which they
        '''                              were requested in the WorkSession); changed the function template
        '''              SA 09/01/2012 - Added optional parameter to allow get only not finished Patient Samples; changed the function template 
        '''              SA 07/02/2012 - Removed optional parameter pExcludedDepleted. When filter by ElementStatus is informed with value NOPOS, verify the Patient 
        '''                              Sample is not positioned in Samples Rotor (the Patient Sample can have a NOPOS status having a tube placed in the Rotor when
        '''                              it is marked as DEPLETED or with FEW volume)
        '''              SA 09/02/2012 - Added optional parameter pGetAllNoPos to allow get also Patient Samples positioned in Rotor but marked as DEPLETED or FEW
        '''                              This parameter is used only when pElementStatus is informed as NOPOS
        '''              SG 30/04/2013 - Add 'SpecimenIDList' in the query
        ''' </remarks>
        Public Function GetRequiredPatientSamplesElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, Optional ByVal pElementID As Integer = 0, _
                                                          Optional ByVal pElementStatus As String = "", Optional ByVal pOnlyNotFinished As Boolean = False, _
                                                          Optional ByVal pGetAllNoPos As Boolean = False) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT RE.TubeContent, RE.ElementID, RE.PatientID, RE.OrderID, RE.SampleID, RE.SampleType, " & vbCrLf & _
                                                       " RE.PredilutionFactor, RE.ElementStatus, RE.TubeType, RE.OnlyForISE, RE.SpecimenIDList " & vbCrLf & _
                                                " FROM   twksWSRequiredElements RE " & vbCrLf & _
                                                " WHERE  RE.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    RE.TubeContent   = 'PATIENT' " & vbCrLf

                        'Link to the Query the optional parameters when they are informed 
                        If (pElementID > 0) Then cmdText &= " AND RE.ElementID = " & pElementID & vbCrLf
                        If (pElementStatus <> "") Then
                            cmdText &= " AND RE.ElementStatus = '" & pElementStatus & "' " & vbCrLf
                            If (pElementStatus = "NOPOS" AndAlso Not pGetAllNoPos) Then
                                cmdText &= " AND RE.ElementID NOT IN (SELECT ElementID FROM twksWSRotorContentByPosition " & vbCrLf & _
                                                                    " WHERE  WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                                    " AND    RotorType     = 'SAMPLES' " & vbCrLf & _
                                                                    " AND    ElementID IS NOT NULL) " & vbCrLf
                            End If
                        End If
                        If (pOnlyNotFinished) Then cmdText &= " AND RE.ElementFinished = 0 " & vbCrLf
                        cmdText += " ORDER BY RE.ElementID "

                        Dim resultData As New WSRequiredElementsTreeDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.PatientSamples)
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
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.GetRequiredPatientSamplesElements", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get information of Required Work Session Elements of Reagent type 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Optional parameter. When informed, it allows get only the data of the specified Reagent</param>
        ''' <param name="pElementStatus">Optional parameter. When informed, it allows get only the data of required Reagent
        '''                              having the specified Status (POS, NOPOS or INCOMPLETE for Reagents)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsTreeDS with information 
        '''          obtained stored in Reagents table</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: SA 23/12/2009 - Throw ex when an exception happens was removed (error is returned inside the GlobalDataTO)
        '''              SA 11/01/2010 - Changed the way of open the DB Connection to fulfill the new template
        '''              SA 09/03/2010 - Filter the query by TestID in the WorkSession (due to problems with Reagents used for several Tests)
        '''              SA 09/01/2012 - Changed the function template; changed the ORDER BY, it should be by Reagent and Test, not the opposite
        '''                              (to allow management of shared Reagents)
        '''              SA 19/04/2012 - Changed the query to use INNER JOINs 
        ''' TODO:
        ''' AG 02/08/2011 - LotNumber and Expiration date are get from table tparHistoryReagentBottles instead of from tparReagents
        ''' </remarks>
        Public Function GetRequiredReagentsDetails(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                   Optional ByVal pElementID As Integer = 0, Optional ByVal pElementStatus As String = "") _
                                                   As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError) AndAlso (Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'TODO
                        'AG 02/08/2011 - LotNumber and ExpirationDate are get from tparHistoryReagentBottles
                        'OLD 2on line: " RE.MultiItemNumber AS ReagentNumber, RE.RequiredVolume, RE.ElementStatus, R.LotNumber, R.ExpirationDate " & _

                        Dim cmdText As String = " SELECT RE.TubeContent, RE.ElementID, TR.TestID, T.TestName, RE.ReagentID, R.ReagentName, " & vbCrLf & _
                                                       " RE.MultiItemNumber AS ReagentNumber, RE.RequiredVolume, RE.ElementStatus " & vbCrLf & _
                                                " FROM   twksWSRequiredElements RE INNER JOIN tparReagents R      ON RE.ReagentID = R.ReagentID " & vbCrLf & _
                                                                                 " INNER JOIN tparTestReagents TR ON RE.ReagentID = TR.ReagentID " & vbCrLf & _
                                                                                 " INNER JOIN tparTests T         ON TR.TestID    = T.TestID " & vbCrLf & _
                                                " WHERE  RE.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    RE.TubeContent   = 'REAGENT' " & vbCrLf & _
                                                " AND    TR.TestID IN (SELECT OT.TestID " & vbCrLf & _
                                                                     " FROM   twksOrderTests OT INNER JOIN twksWSOrderTests WSOT ON WSOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                     " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "') " & vbCrLf

                        'Dim cmdText As String = " SELECT RE.TubeContent, RE.ElementID, TR.TestID, T.TestName, RE.ReagentID, R.ReagentName, " & vbCrLf & _
                        '                               " RE.MultiItemNumber AS ReagentNumber, RE.RequiredVolume, RE.ElementStatus " & vbCrLf & _
                        '                        " FROM   twksWSRequiredElements RE, tparReagents R, tparTestReagents TR, tparTests T " & vbCrLf & _
                        '                        " WHERE  RE.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                        '                        " AND    RE.TubeContent = 'REAGENT' " & vbCrLf & _
                        '                        " AND    R.ReagentID  = RE.ReagentID " & vbCrLf & _
                        '                        " AND    TR.ReagentID = RE.ReagentID " & vbCrLf & _
                        '                        " AND    TR.TestID	= T.TestID " & vbCrLf & _
                        '                        " AND    TR.TestID IN (SELECT OT.TestID FROM twksOrderTests OT, twksWSOrderTests WSOT " & vbCrLf & _
                        '                                             " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                        '                                             " AND    WSOT.OrderTestID = OT.OrderTestID) " & vbCrLf

                        'Link to the Query the optional parameters when they are informed 
                        If (pElementID > 0) Then cmdText += " AND RE.ElementID = " & pElementID & vbCrLf
                        If (pElementStatus <> "") Then cmdText += " AND RE.ElementStatus = '" & pElementStatus & "' " & vbCrLf

                        'Sort records by ReagentName, ReagentNumber and TestID
                        cmdText += " ORDER BY R.ReagentName, RE.MultiItemNumber, TR.TestID " & vbCrLf

                        Dim resultData As New WSRequiredElementsTreeDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.Reagents)
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
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.GetRequiredReagentsDetails", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get details of all the Additional Solutions included in the specified Work Session or, optionally, get details
        ''' of an specific Additional Solution or of all Additional Solutions that have the specified status
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection </param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Element Identifier. Optional parameter; when informed, the function gets details
        '''                          of the specific Additional Solution</param>
        ''' <param name="pElementStatus">Element Status. Optional parameter; when informed, the function gets details
        '''                              of all Additional Solutions in the Work Session that have this status</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsTreeDS with details of the Additional Solutions
        '''          included in the specified Work Session</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: TR 14/12/2009 - Add the optional filters also to the first select in the UNION
        '''              SA 23/12/2009 - The optional filter by ElementID was missing in the first select
        '''                            - Throw ex when an exception happens was removed (error is returned inside the GlobalDataTO)
        '''              SA 11/01/2010 - Changed the way of open the DB Connection to fulfill the new template
        '''              DL 08/10/2010 - Added subquery to get the Washing Solutions
        '''              PG 19/10/2010 - Modified query because it must show the ResourceText (value depends the current language)
        '''              SA 26/01/2011 - Added a new subquery to get details of Elements corresponding to Diluent Solutions
        '''              SA 09/01/2012 - Changed the function template
        '''              AG 17/01/2012 - In the subquery for getting the Diluent Solutions, exclude also the Distilled Water (DISTW)
        ''' </remarks>
        Public Function GetRequiredSolutionsDetails(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                    Optional ByVal pElementID As Integer = 0, Optional ByVal pElementStatus As String = "") _
                                                    As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError) AndAlso (Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim var As New GlobalBase
                        Dim cmdText As String = String.Empty
                        cmdText &= "SELECT   RE.TubeContent, RE.ElementID, RE.SolutionCode, RE.ElementStatus, MR.ResourceText AS SolutionName, " & vbCrLf
                        cmdText &= "         RE.RequiredVolume, PMD.Position AS SortElement, 1 AS SortGroup " & vbCrLf
                        cmdText &= "FROM     twksWSRequiredElements RE INNER JOIN tfmwPreloadedMasterData PMD  " & vbCrLf
                        cmdText &= "         ON PMD.ItemID  = RE.SolutionCode INNER JOIN tfmwMultiLanguageResources MR" & vbCrLf
                        cmdText &= "         ON PMD.ResourceID = MR.ResourceID" & vbCrLf
                        cmdText &= "WHERE    RE.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf
                        cmdText &= "  AND    RE.TubeContent = 'SPEC_SOL' " & vbCrLf
                        cmdText &= "  AND    PMD.SubTableID = 'SPECIAL_SOLUTIONS' "
                        cmdText &= "  AND    MR.LanguageID = '" & var.GetSessionInfo.ApplicationLanguage & "'"

                        'Link to the Query the optional parameters when they are informed 
                        If (pElementID > 0) Then cmdText &= "  AND RE.ElementID = " & pElementID & vbCrLf
                        If (pElementStatus <> "") Then cmdText &= "  AND RE.ElementStatus = '" & pElementStatus & "' " & vbCrLf

                        cmdText &= "UNION " & vbCrLf
                        cmdText &= "SELECT   RE.TubeContent, RE.ElementID, RE.SolutionCode, RE.ElementStatus, MR.ResourceText AS SolutionName, " & vbCrLf
                        cmdText &= "         RE.RequiredVolume, PMD.Position AS SortElement, 2 AS SortGroup " & vbCrLf
                        cmdText &= "FROM     twksWSRequiredElements RE INNER JOIN tfmwPreloadedMasterData PMD" & vbCrLf
                        cmdText &= "         ON PMD.ItemID  = RE.SolutionCode INNER JOIN tfmwMultiLanguageResources MR " & vbCrLf
                        cmdText &= "         ON PMD.ResourceID = MR.ResourceID" & vbCrLf
                        cmdText &= "WHERE    RE.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf
                        cmdText &= "  AND    RE.TubeContent = 'SPEC_SOL' " & vbCrLf
                        cmdText &= "  AND    RE.SolutionCode <> 'SALINESOL' " & vbCrLf
                        cmdText &= "  AND    RE.SolutionCode <> 'DISTW' " & vbCrLf
                        cmdText &= "  AND    PMD.SubTableID = 'DIL_SOLUTIONS' " & vbCrLf
                        cmdText &= "  AND    MR.LanguageID = '" & var.GetSessionInfo.ApplicationLanguage & "'"

                        'Link to the Query the optional parameters when they are informed 
                        If (pElementID > 0) Then cmdText &= "  AND RE.ElementID = " & pElementID & vbCrLf
                        If (pElementStatus <> "") Then cmdText &= "  AND RE.ElementStatus = '" & pElementStatus & "'" & vbCrLf

                        cmdText &= "UNION " & vbCrLf
                        cmdText &= "SELECT   RE.TubeContent, RE.ElementID, RE.SolutionCode, RE.ElementStatus, MR.ResourceText AS SolutionName, " & vbCrLf
                        cmdText &= "         RE.RequiredVolume, PMD.Position AS SortElement, 3 AS SortGroup " & vbCrLf
                        cmdText &= "FROM     twksWSRequiredElements RE INNER JOIN tfmwPreloadedMasterData PMD" & vbCrLf
                        cmdText &= "         ON PMD.ItemID  = RE.SolutionCode INNER JOIN tfmwMultiLanguageResources MR " & vbCrLf
                        cmdText &= "         ON PMD.ResourceID = MR.ResourceID" & vbCrLf
                        cmdText &= "WHERE    RE.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf
                        cmdText &= "  AND    RE.TubeContent = 'WASH_SOL' " & vbCrLf
                        cmdText &= "  AND    PMD.SubTableID = 'WASHING_SOLUTIONS' " & vbCrLf
                        cmdText &= "  AND    MR.LanguageID = '" & var.GetSessionInfo.ApplicationLanguage & "'"

                        'Link to the Query the optional parameters when they are informed 
                        If (pElementID > 0) Then cmdText &= "  AND RE.ElementID = " & pElementID & vbCrLf
                        If (pElementStatus <> "") Then cmdText &= "  AND RE.ElementStatus = '" & pElementStatus & "'" & vbCrLf

                        cmdText &= "ORDER BY RE.TubeContent, SortGroup, PMD.Position"

                        Dim resultData As New WSRequiredElementsTreeDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.AdditionalSolutions)
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
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.GetRequiredSolutionsDetails", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get details of all the Tube Additional Solutions included in the specified Work Session or, optionally, get details
        ''' of an specific Tube Additional Solution or of all Tube Additional Solutions that have the specified status
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Optional parameter. When informed, it allows get only the data of the specified Element</param>
        ''' <param name="pElementStatus">Optional parameter. When informed, it allows get only the data of required Element having the specified 
        '''                              Status (POS, NOPOS or INCOMPLETE for Reagents)</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsTreeDS with information obtained stored in AdditionalSolutions table</returns>
        ''' <remarks>
        ''' Created by:  RH 10/06/2011 - Based on GetRequiredSolutionsDetails()
        ''' Modified by: SA 11/01/2012 - Added optional parameter to allow get only not finished Sample Additional Solutions
        '''              SA 20/04/2012 - Removed subquery for Diluent Solutions due to they are not loaded in Samples Rotor as Tube
        '''                              Additional Solutions, they are loaded in Reagents Rotor as Additional Solutions 
        ''' </remarks>
        Public Function GetRequiredTubeSolutionsDetails(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, Optional ByVal pElementID As Integer = 0, _
                                                        Optional ByVal pElementStatus As String = "", Optional ByVal pOnlyNotFinished As Boolean = False) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim var As New GlobalBase
                        Dim cmdText As String = String.Empty
                        cmdText &= "SELECT   RE.TubeContent, RE.ElementID, RE.SolutionCode, RE.ElementStatus, MR.ResourceText AS SolutionName, " & vbCrLf
                        cmdText &= "         RE.RequiredVolume, PMD.Position AS SortElement, 1 AS SortGroup, RE.TubeType " & vbCrLf
                        cmdText &= "FROM     twksWSRequiredElements RE INNER JOIN tfmwPreloadedMasterData PMD   ON PMD.ItemID = RE.SolutionCode " & vbCrLf
                        cmdText &= "                                   INNER JOIN tfmwMultiLanguageResources MR ON PMD.ResourceID = MR.ResourceID " & vbCrLf
                        cmdText &= "WHERE    RE.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf
                        cmdText &= "  AND    RE.TubeContent = 'TUBE_SPEC_SOL' " & vbCrLf
                        cmdText &= "  AND    PMD.SubTableID = 'SPECIAL_SOLUTIONS' "
                        cmdText &= "  AND    MR.LanguageID = '" & var.GetSessionInfo.ApplicationLanguage & "'" & vbCrLf

                        'Link to the Query the optional parameters when they are informed 
                        If (pElementID > 0) Then cmdText &= "  AND RE.ElementID = " & pElementID & vbCrLf
                        If (pElementStatus <> "") Then cmdText &= "  AND RE.ElementStatus = '" & pElementStatus & "' " & vbCrLf
                        If (pOnlyNotFinished) Then cmdText &= " AND RE.ElementFinished = 0 " & vbCrLf

                        cmdText &= "UNION " & vbCrLf
                        cmdText &= "SELECT   RE.TubeContent, RE.ElementID, RE.SolutionCode, RE.ElementStatus, MR.ResourceText AS SolutionName, " & vbCrLf
                        cmdText &= "         RE.RequiredVolume, PMD.Position AS SortElement, 3 AS SortGroup, RE.TubeType " & vbCrLf
                        cmdText &= "FROM     twksWSRequiredElements RE INNER JOIN tfmwPreloadedMasterData PMD" & vbCrLf
                        cmdText &= "         ON PMD.ItemID  = RE.SolutionCode INNER JOIN tfmwMultiLanguageResources MR " & vbCrLf
                        cmdText &= "         ON PMD.ResourceID = MR.ResourceID" & vbCrLf
                        cmdText &= "WHERE    RE.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf
                        cmdText &= "  AND    RE.TubeContent = 'TUBE_WASH_SOL' " & vbCrLf
                        cmdText &= "  AND    PMD.SubTableID = 'WASHING_SOLUTIONS' " & vbCrLf
                        cmdText &= "  AND    MR.LanguageID = '" & var.GetSessionInfo.ApplicationLanguage & "'" & vbCrLf

                        'Link to the Query the optional parameters when they are informed 
                        If (pElementID > 0) Then cmdText &= "  AND RE.ElementID = " & pElementID & vbCrLf
                        If (pElementStatus <> "") Then cmdText &= "  AND RE.ElementStatus = '" & pElementStatus & "'" & vbCrLf
                        If (pOnlyNotFinished) Then cmdText &= " AND RE.ElementFinished = 0 " & vbCrLf

                        cmdText &= "ORDER BY RE.TubeContent, SortGroup, PMD.Position"

                        Dim resultData As New WSRequiredElementsTreeDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.TubeAdditionalSolutions)
                            End Using
                        End Using

                        dataToReturn.SetDatos = resultData
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.GetRequiredTubeSolutionsDetails", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Search if there are dilutions for the Patient Sample identified for the informed Element Identifier
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pElementID">Identifier of the required Patient Sample Element</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS with data of all 
        '''          non positioned Patient Sample Dilutions (related with the informed Patient Sample Element).</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: SA 23/12/2009 - Throw ex when an exception happens was removed (error is returned inside the GlobalDataTO)
        '''              SA 11/01/2010 - Changed the way of open the DB Connection to fulfill the new template 
        '''              SA 09/03/2010 - Change to filter data also by SampleID when this field is informed
        '''              SA 09/01/2012 - Changed the function template
        ''' </remarks>
        Public Function GetSampleDilutionElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                  ByVal pElementID As Integer) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT B.ElementID, B.ElementFinished " & vbCrLf & _
                                                " FROM   twksWSRequiredElements A, twksWSRequiredElements B " & vbCrLf & _
                                                " WHERE  A.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    A.ElementID = " & pElementID & vbCrLf & _
                                                " AND    B.WorkSessionID = A.WorkSessionID " & vbCrLf & _
                                                " AND    B.SampleType = A.SampleType " & vbCrLf & _
                                                " AND    B.ElementID <> A.ElementID " & vbCrLf & _
                                                " AND  ((A.PatientID IS NOT NULL AND A.PatientID = B.PatientID) " & vbCrLf & _
                                                " OR    (A.SampleID IS NOT NULL AND A.SampleID = B.SampleID) " & vbCrLf & _
                                                " OR    (A.OrderID IS NOT NULL and A.OrderID = B.OrderID)) " & vbCrLf

                        Dim resultData As New WSRequiredElementsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRequiredElements)
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
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.GetSampleDilutionElements", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get the list of Required Elements for Special Solution for the specified TestID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestDetailsRow">Row with structure of DataSet OrderTestsDetailsDS containing
        '''                                    TestID</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSElementsByOrderTestDS with the list of Required Special Solution 
        '''          Elements for the specified TestID</returns>
        ''' <remarks>
        ''' Created by:  RH 13/04/2010
        ''' Modified by: SA 15/10/2010 - Changed the SQL to get always the required Element for Saline Solution when the processed
        '''                              Order Test corresponds to a Patient Sample for a Test/SampleType with a programmed automatic 
        '''                              dilution 
        '''              SA 27/01/2011 - Changed the SQL to get the required Element for the Diluent Solution when the processed 
        '''                              Order Test corresponds to a Patient Sample for a Test/SampleType with a programmed automatic
        '''                              dilution
        '''              RH 17/06/2011 - TUBE_SPEC_SOL
        '''              SA 09/01/2012 - Changed the function template
        ''' </remarks>
        Public Function GetSpecialSolutionElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                                   ByVal pOrderTestDetailsRow As OrderTestsDetailsDS.OrderTestsDetailsRow) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT RP.ElementID FROM twksWSRequiredElements RP " & vbCrLf & _
                                                " WHERE  RP.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                " AND    RP.TubeContent = 'TUBE_SPEC_SOL' " & vbCrLf & _
                                                " AND    RP.SolutionCode IN (SELECT BlankMode FROM tparTests " & vbCrLf & _
                                                                           " WHERE  TestID = " & pOrderTestDetailsRow.TestID & ") " & vbCrLf

                        If (pOrderTestDetailsRow.SampleClass = "PATIENT") Then
                            If (Not pOrderTestDetailsRow.IsPredilutionModeNull AndAlso pOrderTestDetailsRow.PredilutionMode = "INST") Then
                                'For patient samples with automatic dilution, get the needed Diluent Solution...
                                cmdText &= " UNION " & vbCrLf & _
                                           " SELECT RP.ElementID FROM twksWSRequiredElements RP " & vbCrLf & _
                                           " WHERE  RP.WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                           " AND    RP.TubeContent = 'SPEC_SOL' " & vbCrLf & _
                                           " AND    RP.SolutionCode IN (SELECT DiluentSolution FROM tparTestSamples " & vbCrLf & _
                                                                      " WHERE  TestID = " & pOrderTestDetailsRow.TestID & vbCrLf & _
                                                                      " AND    SampleType = '" & pOrderTestDetailsRow.SampleType & "') " & vbCrLf
                            End If
                        End If

                        Dim resultData As New WSElementsByOrderTestDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRequiredElemByOrderTest)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSRequiredElementsDelegate.GetSpecialSolutionElements", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Search the ElementID of Reagents by Biosystems Reagent CodeTest or the ElementID of Additonal Solutions by CodeTest and SolutionCode
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCodeTest">Biosystems Code for the Reagent or the Additional Solution</param>
        ''' <param name="pReagentNumber">Reagent Number searched. Informed only when the funtion is used for searching of a Reagent</param>
        ''' <param name="pSolutionCode">Code of the Additional Solution in the application. Informed only when the function is used for
        '''                             searching of an Additional Solution</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS</returns>
        ''' <remarks>
        ''' Created by:  AG 29/08/2011
        ''' Modified by: TR 07/09/2011 - Changed the SQL used for searching of Additional Solutions by removing use of table twksWSRequiredElemByOrderTest
        '''              TR 26/01/2012 - Changed the CodeTest Parammeter from Integer to String
        '''              SA 19/04/2012 - Changed the query (Reagents case) by adding a filter by Standard Tests
        ''' </remarks>
        Public Function ReadElemIDByCodeTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCodeTest As String, ByVal pReagentNumber As Integer, _
                                             ByVal pSolutionCode As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        If (pSolutionCode = "") Then
                            'Search if the REAGENT belongs to the current WorkSession
                            cmdText = " SELECT DISTINCT RE.ElementID, RE.TubeContent " & vbCrLf & _
                                      " FROM   twksWSRequiredElements RE INNER JOIN twksWSRequiredElemByOrderTest REOT ON RE.ElementID = REOT.ElementID AND RE.TubeContent = 'REAGENT' " & vbCrLf & _
                                                                       " INNER JOIN twksOrderTests OT ON REOT.OrderTestID = OT.OrderTestID AND OT.TestType = 'STD' " & vbCrLf & _
                                                                       " INNER JOIN tparTestReagents TR ON OT.TestID = TR.TestID " & vbCrLf & _
                                                                       " INNER JOIN tparReagents R ON TR.ReagentID = R.ReagentID and TR.ReagentNumber = R.ReagentNumber " & vbCrLf & _
                                      " WHERE R.CodeTest = '" & pCodeTest & "' " & vbCrLf & _
                                      " AND   RE.MultiItemNumber = " & pReagentNumber
                        Else
                            'Search if the AdditionalSolution belongs to the current worksession
                            cmdText = " SELECT DISTINCT RE.ElementID, RE.TubeContent " & vbCrLf & _
                                      " FROM   twksWSRequiredElements RE INNER JOIN tfmwSwParameters SW ON RE.SolutionCode = SW.ValueText " & vbCrLf & _
                                      " WHERE  SW.ValueNumeric = '" & pCodeTest & "' " & vbCrLf & _
                                      " AND   (RE.TubeContent = 'SPEC_SOL' OR RE.TubeContent = 'WASH_SOL') " & vbCrLf
                        End If

                        Dim myDataSet As New WSRequiredElementsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksWSRequiredElements)
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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.ReadElemIDByCodeTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all information for the Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created By: GDS 21/04/2010
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksWSRequiredElements" & vbCrLf & _
                                            " WHERE  WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "' "

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.ResetWS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update field ElementFinished for the specified Required Element
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementID">Required Element Identifier</param>
        ''' <param name="pElementFinished">When True, it indicates the Element is not needed anymore in the WS</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 10/01/2012
        ''' Modified by: SG 29/04/2013 - Updated field SpecimenIDList to be shown in the TreeView of Rotor Positions Screen
        ''' </remarks>
        Public Function UpdateElementFinished(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pElementID As Integer, _
                                              ByVal pElementFinished As Boolean, Optional pSpecimenIDList As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksWSRequiredElements " & vbCrLf & _
                                            " SET    ElementFinished = " & IIf(pElementFinished, 1, 0).ToString & vbCrLf
                    If pSpecimenIDList.Length > 0 Then
                        cmdText &= " , SpecimenIDList = '" & pSpecimenIDList & "'"
                    Else
                        'cmdText &= " , SpecimenIDList = NULL " SGM 27/07/2013 - not to set to NULL in case of not informed
                    End If

                    cmdText &= " WHERE  ElementID = " & pElementID & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.UpdateElementFinished", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' For all required Patient Sample Elements for the informed Order:
        ''' ** For the required Patient Sample element for the specified Sample Type, if field SpecimenIDList is NULL, update it with the current SampleID (it means the Order was manually created,
        '''    using the SpecimenID as SampleID
        ''' ** For all required Patient Sample elements (all Sample Types), update fields PatientID and SampleID, depending on value of parameter pSampleIDType:
        '''    * If MAN, update SampleID = pSampleID and PatientID = NULL
        '''    * If DB,  update PatientID = pSampleID and SampleID = NULL
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <param name="pSampleID">Sample or Patient Identifier to update</param>
        ''' <param name="pSampleIDType">Type of Sample ID: DB or MAN</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 01/08/2013 
        ''' </remarks>
        Public Function UpdatePatientSampleFields(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderID As String, ByVal pSampleID As String, ByVal pSampleIDType As String, _
                                                  ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksWSRequiredElements " & vbCrLf & _
                                            " SET    SpecimenIDList = SampleID " & vbCrLf & _
                                            " WHERE  TubeContent = 'PATIENT' " & vbCrLf & _
                                            " AND    SampleType  = '" & pSampleType.Trim & "' " & vbCrLf & _
                                            " AND    SpecimenIDList IS NULL " & vbCrLf & _
                                            " AND    ElementID IN (SELECT ElementID FROM twksWSRequiredElemByOrderTest REOT INNER JOIN twksOrderTests OT ON REOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                 " WHERE  OT.OrderID = '" & pOrderID.Trim & "') " & vbCrLf & _
                                            " UPDATE twksWSRequiredElements " & vbCrLf & _
                                            " SET    PatientID = " & IIf(pSampleIDType = "DB", "N'" & pSampleID.Trim.Replace("'", "''") & "', ", "NULL,").ToString & vbCrLf & _
                                            "        SampleID  = " & IIf(pSampleIDType = "MAN", "N'" & pSampleID.Trim.Replace("'", "''") & "' ", "NULL").ToString & vbCrLf & _
                                            " WHERE  TubeContent = 'PATIENT' " & vbCrLf & _
                                            " AND    ElementID IN (SELECT ElementID FROM twksWSRequiredElemByOrderTest REOT INNER JOIN twksOrderTests OT ON REOT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                 " WHERE  OT.OrderID = '" & pOrderID.Trim & "') " & vbCrLf


                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.UpdatePatientSampleFields", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the Status of the specified Required Element
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementID">Required Element Identifier</param>
        ''' <param name="pNewElementStatus">New Element Status</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  BK 08/12/2009 - Tested: OK
        ''' Modified by: TR 14/12/2009 - The ResultData.SetDatos do not need to insert the GlobalDataTO
        '''              SA 11/01/2012 - Removed code to set HasError=True depending on value of AffectedRecords
        '''              XB 20/03/2014 - Add parameter HResult into Try Catch section - #1548
        ''' </remarks>
        Public Function UpdateStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pElementID As Integer, _
                                     ByVal pNewElementStatus As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksWSRequiredElements " & vbCrLf & _
                                            " SET    ElementStatus = '" & pNewElementStatus & "' " & vbCrLf & _
                                            " WHERE  ElementID = " & pElementID & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "twksWSRequiredElementsDAO.UpdateStatus", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Updates the Status and Tube Type of the specified Required Element. If TubeContent = CALIB, updates all the Calibrators in the
        ''' kit but only when optional parameter pApplyToAllTubes has TRUE as value
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pElementID">Required Element Identifier</param>
        ''' <param name="pNewElementStatus">New Element Status</param>
        ''' <param name="pNewTubeType">New Tube Type for the specified Element</param>
        ''' <param name="pTubeContent">Tube Content</param>
        ''' <param name="pApplyToAllTubes">Optional parameter. When FALSE and TubeContent=CALIB, it means only the Element for the selected
        '''                                Calibrator point is updated (refill case), while, when TRUE and TubeContent=CALIB, it means the 
        '''                                Elements for all Calibrator points in the kit (tube type change case) are updated</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  RH 01/08/2011
        ''' Modified by: SA 11/01/2012 - Removed code to set HasError=True depending on value of AffectedRecords
        '''              SA 13/02/2012 - Added optional parameter pApplyToAllTubes to indicate if the Status has to be updated for all points in
        '''                              the Calibrator kit (only whe TubeContent = CALIB). Added parameter for the WorkSessionID
        ''' </remarks>
        Public Function UpdateStatusAndTubeType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pElementID As Integer, _
                                                ByVal pNewElementStatus As String, ByVal pNewTubeType As String, ByVal pTubeContent As String, _
                                                Optional ByVal pApplyToAllTubes As Boolean = False) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " UPDATE twksWSRequiredElements " & vbCrLf & _
                                            " SET    ElementStatus = '" & pNewElementStatus.Trim & "', TubeType = '" & pNewTubeType & "' " & vbCrLf & _
                                            " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf

                    If (pTubeContent <> "CALIB" OrElse (pTubeContent = "CALIB" AndAlso Not pApplyToAllTubes)) Then
                        'Update is executed only for the specified ElementID 
                        cmdText &= " AND ElementID = " & pElementID
                    Else
                        'Update is executed for all Elements in the Calibrator kit...
                        cmdText &= " AND CalibratorID = (SELECT TOP 1 CalibratorID FROM twksWSRequiredElements " & vbCrLf & _
                                                       " WHERE  ElementID = " & pElementID & ") " & vbCrLf
                    End If

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.UpdateStatusAndTubeType", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update ElementStatus = NOPOS for all Required Elements placed in an Analyzer Rotor after Reset the Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Analyzer Rotor Type</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 25/11/2009 - Tested: OK
        ''' Modified by: AG 30/11/2009 - Added parameter WorkSessionID (Tested: OK)
        '''              SA 11/01/2010 - Error fixed: function does not have to return the GlobalDataTO inside the SetDatos
        '''              SA 11/01/2012 - Removed code to set HasError=True depending on value of AffectedRecords
        ''' </remarks>
        Public Function UpdateStatusByResetRotor(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                 ByVal pRotorType As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE twksWSRequiredElements " & vbCrLf & _
                                            " SET    ElementStatus = 'NOPOS' " & vbCrLf & _
                                            " WHERE  ElementID IN (SELECT DISTINCT ElementID FROM twksWSRotorContentByPosition " & vbCrLf & _
                                                                 " WHERE  AnalyzerId = '" & pAnalyzerID & "' " & vbCrLf & _
                                                                 " AND    WorkSessionID = '" & pWorkSessionID & "' " & vbCrLf & _
                                                                 " AND    RotorType = '" & pRotorType & "') " & vbCrLf

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.UpdateStatusByResetRotor", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "REVIEW-THIS FUNCTION SHOULD BE IN twksWSExecutionsDAO"
        ''' <summary>
        ''' Get the Control Identifier of a required Element by Execution
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExecutionID">Execution Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS containing the Control Identifier</returns>
        ''' <remarks>
        ''' Created by:  DL 23/02/2010
        ''' Modified by: SA 26/07/2010 - Changed the query; it has a closing bracket without the corresponding opening bracket
        '''              DL 18/04/2011 - Changed the query
        ''' </remarks>
        Public Function GetControlID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT OT.ControlID " & vbCrLf & _
                                                " FROM   twksWSExecutions WSE INNER JOIN twksOrderTests OT ON WSE.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                " WHERE  WSE.ExecutionID = " & pExecutionID

                        Dim myWSRequiredElementsDS As New WSRequiredElementsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myWSRequiredElementsDS.twksWSRequiredElements)
                            End Using
                        End Using

                        resultData.SetDatos = myWSRequiredElementsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSRequiredElementsDAO.GetControlID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "TO REVIEW -DELETE??"
        '''' <summary>
        '''' Get the list of Required Elements for Controls for the specified TestID/SampleType
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <param name="pOrderTestDetailsRow">Row with structure of DataSet OrderTestsDetailsDS containing
        ''''                                    TestID and SampleType</param>
        '''' <returns>GlobalDataTO containing a typed DataSet WSElementsByOrderTestDS with the list of Required Control 
        ''''          Elements for the specified TestID/SampleType</returns>
        '''' <remarks>
        '''' Created by:  SA
        '''' Modified by: SA 05/01/2010 - Changed the way of open the DB Connection to the new template; changes to return a 
        ''''                              GlobalDataTO instead a typed DataSet WSElementsByOrderTestDS
        ''''              RH 14/04/2010 - ControlNum field added to query
        '''' </remarks>
        'Public Function GetControlElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
        '                                   ByVal pOrderTestDetailsRow As OrderTestsDetailsDS.OrderTestsDetailsRow) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = ""
        '                cmdText = " SELECT RP.ElementID " & _
        '                          " FROM   twksWSRequiredElements RP INNER JOIN tparTestControls TQC ON RP.ControlID = TQC.ControlID " & _
        '                                                                                          " AND RP.MultiItemNumber = TQC.ControlNum " & _
        '                          " WHERE  RP.WorkSessionID   = '" & pWorkSessionID.Trim & "' " & _
        '                          " AND    RP.TubeContent     = 'CTRL' " & _
        '                          " AND    TQC.TestID         = " & pOrderTestDetailsRow.TestID & _
        '                          " AND    TQC.SampleType     = '" & pOrderTestDetailsRow.SampleType.Trim & "' " & _
        '                          " AND    TQC.ControlNum     = " & pOrderTestDetailsRow.ControlNumber

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                'Fill the DataSet to return 
        '                Dim resultData As New WSElementsByOrderTestDS
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                dbDataAdapter.Fill(resultData.twksWSRequiredElemByOrderTest)

        '                myGlobalDataTO.SetDatos = resultData
        '                myGlobalDataTO.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "WSRequiredElementsDelegate.GetControlsElements", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

#End Region

    End Class
End Namespace
