Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports System.Text
Imports System.Data.SqlClient

Namespace Biosystems.Ax00.DAL.DAO
    Public Class thisISETestSamplesDAO
        Inherits DAOBase

#Region "CRUD Methods"

        ''' <summary>
        ''' Add a list of ISETest/SampleTypes to the corresponding table in Historics Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisISETestsDS">Typed DataSet HisISETestSamplesDS  containing all ISETest/SampleTypes to add</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisISETestSamplesDS with all created ISETest/SampleTypes with the generated HistISETestID</returns>
        ''' <remarks>
        ''' Created by:  SA 22/02/2012
        ''' Modified by: WE 31/07/2014 - TestLongName added (#1865) to support new screen field Report Name in IProgISETest.
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisISETestsDS As HisISETestSamplesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder
                    Dim newISETestID As Integer = -1

                    For Each hisISETestRow As HisISETestSamplesDS.thisISETestSamplesRow In pHisISETestsDS.thisISETestSamples
                        newISETestID = -1
                        cmdText.Append(" INSERT INTO thisISETestSamples (ISETestID, SampleType, ISETestName, MeasureUnit, DecimalsAllowed, TestLongName) ")
                        cmdText.Append(" VALUES (")

                        cmdText.AppendFormat("{0}, '{1}', N'{2}', '{3}', {4}, {5}", hisISETestRow.ISETestID, hisISETestRow.SampleType, _
                                             hisISETestRow.ISETestName.Replace("'", "''"), hisISETestRow.MeasureUnit, hisISETestRow.DecimalsAllowed, hisISETestRow.TestLongName)

                        'Add the last parenthesis and the sentence needed to get the ID automatically generated
                        cmdText.Append(") ")
                        cmdText.Append(" SELECT SCOPE_IDENTITY() ")

                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                            newISETestID = CType(dbCmd.ExecuteScalar(), Integer)
                            If (newISETestID > 0) Then
                                hisISETestRow.HistISETestID = newISETestID
                            End If

                            cmdText.Length = 0 'Instead of using Remove use the Lenght = 0 
                        End Using
                    Next

                    myGlobalDataTO.SetDatos = pHisISETestsDS

                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisISETestSamplesDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Verify if the informed ISETestID/SampleType already exists in Historics Module and in this case, get the saved data
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pISETestID">Identifier of the ISE Test in Parameters Programming Module</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisISETestSamplesDS with all data saved for the ISETest/SampleType in Historics Module</returns>
        ''' <remarks>
        ''' Created by:  SA 22/02/2012
        ''' </remarks>
        Public Function ReadByISETestIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISETestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    Dim myHisISETestsDS As New HisISETestSamplesDS
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT HITS.* FROM thisISETestSamples HITS " & vbCrLf & _
                                                " WHERE  HITS.ISETestID = " & pISETestID.ToString & vbCrLf & _
                                                " AND    HITS.SampleType = '" & pSampleType.Trim & "' " & vbCrLf 

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHisISETestsDS.thisISETestSamples)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myHisISETestsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisISETestSamplesDAO.ReadByISETestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

#End Region
    End Class
End Namespace
