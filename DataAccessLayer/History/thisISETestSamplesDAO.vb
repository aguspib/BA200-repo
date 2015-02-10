Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports System.Text

Namespace Biosystems.Ax00.DAL.DAO
    Public Class thisISETestSamplesDAO
          

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
        '''              WE 25/08/2014 - SlopeFactorA2/B2 added (#1865).
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
                        cmdText.Append(" INSERT INTO thisISETestSamples (ISETestID, SampleType, ISETestName, MeasureUnit, DecimalsAllowed, TestLongName, SlopeFactorA2, SlopeFactorB2) ")
                        cmdText.Append(" VALUES (")

                        cmdText.AppendFormat("{0}, '{1}', N'{2}', '{3}', {4}, N'{5}'", hisISETestRow.ISETestID, hisISETestRow.SampleType, _
                                             hisISETestRow.ISETestName.Replace("'", "''"), hisISETestRow.MeasureUnit, hisISETestRow.DecimalsAllowed, hisISETestRow.TestLongName.Replace("'", "''"))

                        If (Not hisISETestRow.IsSlopeFactorA2Null) Then
                            cmdText.Append(", " & ReplaceNumericString(hisISETestRow.SlopeFactorA2))
                        Else
                            cmdText.Append(", NULL")
                        End If

                        If (Not hisISETestRow.IsSlopeFactorB2Null) Then
                            cmdText.Append(", " & ReplaceNumericString(hisISETestRow.SlopeFactorB2))
                        Else
                            cmdText.Append(", NULL")
                        End If

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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisISETestSamplesDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Verify if the informed ISETestID (with whatever SampleType) already exists in Historics Module and in this case, get the saved data
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pISETestID">Identifier of the ISE Test in Parameters Programming Module</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisISETestsDS with data saved for the ISETest (all SampleTypes) in Historics Module</returns>
        ''' <remarks>
        ''' Created by:  SA 04/09/2014 - BA-1865
        ''' </remarks>
        Public Function ReadByISETestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISETestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM thisISETestSamples " & vbCrLf & _
                                                " WHERE  ISETestID = " & pISETestID.ToString & vbCrLf

                        Dim myHisISETestsDS As New HisISETestSamplesDS
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisISETestSamplesDAO.ReadByISETestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisISETestSamplesDAO.ReadByISETestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' When an ISE Test/SampleType is updated in Parameters Programming Module, if it already exists in the corresponding table in Historics 
        ''' Module, then data is also updated in this module. If field Name has been changed, it is updated not only for the informed SampleType, 
        ''' but all the Sample Types that exist in Historic Module for the informed ISETestID (in this case the entry DS can contain several rows)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisISETestSamplesDS">Typed DataSet HisISETestSamplesDS containing all data to update for the ISETestIDs/SampleTypes in 
        '''                                    Historics Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 04/09/2014 - BA-1865
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisISETestSamplesDS As HisISETestSamplesDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = ""
                    For Each row As HisISETestSamplesDS.thisISETestSamplesRow In pHisISETestSamplesDS.thisISETestSamples.Rows
                        If (Not row.IsHistISETestIDNull) Then
                            cmdText &= " UPDATE thisISETestSamples " & vbCrLf & _
                                       " SET    ISETestName     = N'" & row.ISETestName.Replace("'", "''").Trim & "', " & vbCrLf & _
                                              " MeasureUnit     = '" & row.MeasureUnit.Trim & "', " & vbCrLf & _
                                              " DecimalsAllowed = " & row.DecimalsAllowed.ToString & ", " & vbCrLf

                            If (row.IsTestLongNameNull) Then
                                cmdText &= " TestLongName = NULL, " & vbCrLf
                            Else
                                cmdText &= " TestLongName = N'" & row.TestLongName.Replace("'", "''").Trim & "', " & vbCrLf
                            End If

                            If (row.IsSlopeFactorA2Null) Then
                                cmdText &= " SlopeFactorA2 = NULL, " & vbCrLf
                            Else
                                cmdText &= " SlopeFactorA2 = " & ReplaceNumericString(row.SlopeFactorA2) & ", " & vbCrLf
                            End If
                            If (row.IsSlopeFactorB2Null) Then
                                cmdText &= " SlopeFactorB2 = NULL " & vbCrLf
                            Else
                                cmdText &= " SlopeFactorB2 = " & ReplaceNumericString(row.SlopeFactorB2) & vbCrLf
                            End If

                            cmdText &= " WHERE  HistISETestID = " & row.HistISETestID.ToString & vbCrLf & _
                                       " AND    SampleType    = '" & row.SampleType & "' " & vbCrLf

                            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                                resultData.SetDatos = dbCmd.ExecuteScalar()
                                resultData.HasError = False
                            End Using
                        End If
                    Next
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisISETestSamplesDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace
