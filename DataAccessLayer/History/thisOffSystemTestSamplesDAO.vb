Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports System.Text
Imports System.Data.SqlClient

Namespace Biosystems.Ax00.DAL.DAO
    Public Class thisOffSystemTestSamplesDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Add a list of OFFSTest/SampleTypes to the corresponding table in Historics Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisOFFSTestsDS">Typed DataSet HisOFFSTestSamplesDS  containing all OFFTest/SampleTypes to add</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisOFFSTestSamplesDS with all created OFFTest/SampleTypes with the generated HistOffSystemTestID</returns>
        ''' <remarks>
        ''' Created by:  SA 24/02/2012
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisOFFSTestsDS As HisOFFSTestSamplesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder
                    Dim newOFFSTestID As Integer = -1

                    For Each hisOFFTestRow As HisOFFSTestSamplesDS.thisOffSystemTestSamplesRow In pHisOFFSTestsDS.thisOffSystemTestSamples
                        newOFFSTestID = -1
                        cmdText.Append(" INSERT INTO thisOffSystemTestSamples (OffSystemTestID, SampleType, OffSystemTestName, ResultType, MeasureUnit, DecimalsAllowed) ")
                        cmdText.Append(" VALUES (")

                        cmdText.AppendFormat("{0}, '{1}', N'{2}', '{3}', '{4}', {5}", hisOFFTestRow.OffSystemTestID, hisOFFTestRow.SampleType, _
                                             hisOFFTestRow.OffSystemTestName.Replace("'", "''"), hisOFFTestRow.ResultType, hisOFFTestRow.MeasureUnit, _
                                             hisOFFTestRow.DecimalsAllowed)

                        'Add the last parenthesis and the sentence needed to get the ID automatically generated
                        cmdText.Append(") ")
                        cmdText.Append(" SELECT SCOPE_IDENTITY() ")

                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                            newOFFSTestID = CType(dbCmd.ExecuteScalar(), Integer)
                            If (newOFFSTestID > 0) Then
                                hisOFFTestRow.HistOffSystemTestID = newOFFSTestID
                            End If

                            cmdText.Length = 0 'Instead of using Remove use the Lenght = 0 
                        End Using
                    Next

                    myGlobalDataTO.SetDatos = pHisOFFSTestsDS
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisOffSystemTestSamplesDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' When an OffSystem Test/Sample is deleted in OFF-SYSTEM Tests Programming Screen, if it exists in the corresponding table in Historics Module, then it is 
        ''' marked as deleted by updating field ClosedOffSystemTest to True
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisOFFSTestID">Identifier of the OFF-SYSTEM Test in Historics Module</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 24/02/2012
        ''' </remarks>
        Public Function CloseOFFSTestSample(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisOFFSTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " UPDATE thisOffSystemTestSamples SET ClosedOffSystemTest = 1 " & vbCrLf & _
                                            " WHERE  HistOffSystemTestID = " & pHisOFFSTestID.ToString & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisOffSystemTestSamplesDAO.CloseOFFSTestSample", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all not in use closed Off System Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 01/07/2013
        ''' </remarks>
        Public Function DeleteClosedNotInUse(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " DELETE FROM thisOffSystemTestSamples " & vbCrLf & _
                                            " WHERE  ClosedOffSystemTest  = 1 " & vbCrLf & _
                                            " AND    HistOffSystemTestID  NOT IN (SELECT HistTestID  FROM thisWSOrderTests " & vbCrLf & _
                                                                                " WHERE  TestType = 'OFFS') " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisOffSystemTestSamplesDAO.DeleteClosedNotInUse", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if the informed OFFSTestID already exists in Historics Module and in this case, get its data
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOFFSTestID">Identifier of the OFF-SYSTEM Test in Parameters Programming Module</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisOFFSTestSamplesDS with all data saved for the OFFSTest/SampleType 
        '''          in Historics Module</returns>
        ''' <remarks>
        ''' Created by:  TR 04/09/2012
        ''' </remarks>
        Public Function ReadByOFFSTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOFFSTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    Dim myHisOFFSTestsDS As New HisOFFSTestSamplesDS
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT HOTS.* FROM thisOffSystemTestSamples HOTS " & vbCrLf & _
                                                " WHERE  HOTS.OffSystemTestID = " & pOFFSTestID.ToString & vbCrLf & _
                                                " AND    HOTS.ClosedOffSystemTest = 0 " & vbCrLf

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHisOFFSTestsDS.thisOffSystemTestSamples)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myHisOFFSTestsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisOffSystemTestSamplesDAO.ReadByOFFSTestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Verify if the informed OFFSTestID/SampleType already exists in Historics Module and in this case, get its data
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOFFSTestID">Identifier of the OFF-SYSTEM Test in Parameters Programming Module</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisOFFSTestSamplesDS with all data saved for the OFFSTest/SampleType in Historics Module</returns>
        ''' <remarks>
        ''' Created by:  SA 24/02/2012
        ''' </remarks>
        Public Function ReadByOFFSTestIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOFFSTestID As Integer, _
                                                      ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    Dim myHisOFFSTestsDS As New HisOFFSTestSamplesDS
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT HOTS.* FROM thisOffSystemTestSamples HOTS " & vbCrLf & _
                                                " WHERE  HOTS.OffSystemTestID = " & pOFFSTestID.ToString & vbCrLf & _
                                                " AND    HOTS.SampleType = '" & pSampleType.Trim & "' " & vbCrLf & _
                                                " AND    HOTS.ClosedOffSystemTest = 0 " & vbCrLf

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHisOFFSTestsDS.thisOffSystemTestSamples)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myHisOFFSTestsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisOffSystemTestSamplesDAO.ReadByOFFSTestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update fields OffSystemTestName, MeasureUnit and DecimalsAllowed for an specific OFF-SYSTEM Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisOFFSTestsDS">Typed DataSet HisOFFSTestSamplesDS containing data of the OFFTest/SampleType to update</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 24/02/2012
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisOFFSTestsDS As HisOFFSTestSamplesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " UPDATE thisOffSystemTestSamples " & vbCrLf & _
                                            " SET    OffSystemTestName = N'" & pHisOFFSTestsDS.thisOffSystemTestSamples(0).OffSystemTestName.Replace("'", "''") & "', " & vbCrLf & _
                                                   " MeasureUnit = '" & pHisOFFSTestsDS.thisOffSystemTestSamples(0).MeasureUnit & "', " & vbCrLf & _
                                                   " DecimalsAllowed = " & pHisOFFSTestsDS.thisOffSystemTestSamples(0).DecimalsAllowed.ToString & vbCrLf & _
                                            " WHERE  HistOffSystemTestID = " & pHisOFFSTestsDS.thisOffSystemTestSamples(0).HistOffSystemTestID.ToString & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisOffSystemTestSamplesDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

    End Class
End Namespace

