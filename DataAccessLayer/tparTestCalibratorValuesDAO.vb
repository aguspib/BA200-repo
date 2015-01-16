Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class tparTestCalibratorValuesDAO
        Inherits DAOBase
#Region "CRUD"

        ''' <summary>
        ''' Add values for one point of an experimental Calibrator when it is used for an specific Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestCalibValueDS">Typed DataSet TestCalibratorValuesDS containing the Test Calibrator Values to add</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  VR 28/05/2010
        ''' Modified by: SA 08/02/2012 - Changed the function template
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestCalibValueDS As TestCalibratorValuesDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " INSERT INTO tparTestCalibratorValues (TestCalibratorID, CalibratorNum, TheoricalConcentration, KitConcentrationRelation, BaseConcentration) " & vbCrLf & _
                                            " VALUES(" & pTestCalibValueDS.tparTestCalibratorValues(0).TestCalibratorID & ", " & vbCrLf & _
                                                         pTestCalibValueDS.tparTestCalibratorValues(0).CalibratorNum & ", " & vbCrLf & _
                                                         ReplaceNumericString(pTestCalibValueDS.tparTestCalibratorValues(0).TheoricalConcentration) & ", " & vbCrLf & _
                                                         ReplaceNumericString(pTestCalibValueDS.tparTestCalibratorValues(0).KitConcentrationRelation) & ", " & vbCrLf

                    If (pTestCalibValueDS.tparTestCalibratorValues(0).IsBaseConcentrationNull) Then
                        cmdText &= " 0) " & vbCrLf
                    Else
                        cmdText &= " '" & pTestCalibValueDS.tparTestCalibratorValues(0).BaseConcentration & "') " & vbCrLf
                    End If

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
                GlobalBase.CreateLogActivity(ex.Message, "tparTestCalibratorValuesDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Calibrator values for the specified TestCalibratorID 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestCalibratorID">Test Calibrator Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 17/05/2010 
        ''' Modified by: SA 08/02/2012 - Changed the function template
        ''' </remarks>
        Public Function DeleteByTestsCalibratorID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestCalibratorID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM tparTestCalibratorValues " & vbCrLf & _
                                            " WHERE TestCalibratorID = " & pTestCalibratorID & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestCalibratorValuesDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get values of all points of a Calibrator searching by TestCalibratorID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestCalibratorID">Test Calibrator Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestCalibratorValuesDS with the values of the Calibrator points</returns>
        ''' <remarks>
        ''' Created by:  TR 03/06/2010
        ''' Modified by: SA 08/02/2012 - Changed the function template
        ''' </remarks>
        Public Function ReadByTestCalibratorID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestCalibratorID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tparTestCalibratorValues " & vbCrLf & _
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
                GlobalBase.CreateLogActivity(ex.Message, " tparTestCalibratorsDAO.ReadByTestCalibratorID ", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get values of all points of a Calibrator searching by TestID/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestCalibratorValuesDS with the values of the Calibrator points</returns>
        ''' <remarks>
        ''' Created by:  TR 14/06/2010
        ''' Modified by: SA 08/02/2012 - Changed the function template
        '''              XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        ''' </remarks>
        Public Function ReadByTestIDSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TCV.* FROM tparTestCalibratorValues TCV INNER JOIN tparTestCalibrators TC ON TCV.TestCalibratorID = TC.TestCalibratorID " & vbCrLf & _
                                                " WHERE  TC.TestID            = " & pTestID.ToString & vbCrLf & _
                                                " AND    UPPER(TC.SampleType) = UPPER(N'" & pSampleType.Trim & "') " & vbCrLf & _
                                                " ORDER BY TCV.CalibratorNum DESC "
                        '" AND    UPPER(TC.SampleType) = '" & pSampleType.Trim.ToUpper & "' " & vbCrLf & _

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
                GlobalBase.CreateLogActivity(ex.Message, " tparTestCalibratorsDAO.ReadByTestIDSampleType ", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update values of one point of an experimental Calibrator when it is used for an specific Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestCalibValueDS">Typed DataSet TestCalibratorValuesDS containing the Test Calibrator Value to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  VR 28/05/2010 
        ''' Modified by: AG 31/05/2010 - Added BaseConcentration
        '''              SA 08/02/2012 - Changed the function template
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestCalibValueDS As TestCalibratorValuesDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE tparTestCalibratorValues " & vbCrLf & _
                                            " SET    TheoricalConcentration   = " & ReplaceNumericString(pTestCalibValueDS.tparTestCalibratorValues(0).TheoricalConcentration) & ", " & vbCrLf & _
                                                   " KitConcentrationRelation = " & ReplaceNumericString(pTestCalibValueDS.tparTestCalibratorValues(0).KitConcentrationRelation) & ", " & vbCrLf & _
                                                   " BaseConcentration        = '" & pTestCalibValueDS.tparTestCalibratorValues(0).BaseConcentration & "' " & vbCrLf & _
                                            " WHERE  TestCalibratorID = " & pTestCalibValueDS.tparTestCalibratorValues(0).TestCalibratorID & vbCrLf & _
                                            " AND    CalibratorNum    = " & pTestCalibValueDS.tparTestCalibratorValues(0).CalibratorNum & vbCrLf

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
                GlobalBase.CreateLogActivity(ex.Message, "tparTestCalibratorValuesDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "Other Methods"

        ''' <summary>
        ''' Get the number of points of the Experimental Calibrator used for the Test/SampleType by searching the number of points with defined values
        ''' (theorical concentration and factor). Considerations:
        ''' ** If the Test/SampleType uses a Factor or Alternative Calibrator, this function will return zero (due to the Calibrator Type is not experimental)
        ''' ** For Multipoint Calibrators, the definition for every point
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of points of the Experimental Calibrator used for the informed Test/SampleType. 
        '''          Value 0 indicates the Test/SampleType uses a Factor or Alternative Calibrator</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: BK 23/12/2009 - Change datatype Int16 to Integer
        '''              SA 20/07/2010 - Returns a GlobalDataTO instead of an Integer value
        '''              SA 08/02/2012 - Changed query to ANSI SQL; changed the function template
        ''' </remarks>
        Public Function GetNumberOfPointsDefined(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) AS NumRealPoints " & vbCrLf & _
                                                " FROM   tparTestCalibratorValues TCV INNER JOIN tparTestCalibrators TC ON  TCV.TestCalibratorID = TC.TestCalibratorID " & vbCrLf & _
                                                " WHERE  TC.TestID = " & pTestID.ToString & vbCrLf & _
                                                " AND    UPPER(TC.SampleType) = UPPER('" & pSampleType.Trim & "') " & vbCrLf

                        Dim numPoints As Integer = 0
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader
                            dbDataReader = dbCmd.ExecuteReader()
                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (Not dbDataReader.IsDBNull(0)) Then
                                    numPoints = Convert.ToInt32(dbDataReader.Item("NumRealPoints"))
                                End If
                            End If
                            dbDataReader.Close()
                        End Using

                        resultData.SetDatos = numPoints
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestCalibratorValuesDAO.GetNumberOfPointsDefined", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "TO REVIEW!!"
        ''' <summary>
        ''' Get Test Calibrator Values by  TestCalibratorID and TestID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestCalibratorID">Test Calibrator Identifier</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  DL 07/06/2010
        ''' </remarks>
        Public Function ReadByTestCalibratorIDAndTestID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                        ByVal pTestCalibratorID As Integer, _
                                                        ByVal pTestID As Integer) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Dim myTestCalibratorValueDS As New TestCalibratorValuesDS

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= " SELECT TCV.TestCalibratorID, TCV.CalibratorNum, TCV.TheoricalConcentration, TCV.KitConcentrationRelation, TCV.BaseConcentration  " & vbCrLf
                        cmdText &= " FROM	tparTestCalibratorValues TCV inner join tparTestCalibrators TC ON TCV.TestCalibratorID = TC.TestCalibratorID " & vbCrLf
                        cmdText &= " WHERE  TCV.TestCalibratorID = " & pTestCalibratorID & " AND TC.TestID = " & pTestID
                        cmdText &= " ORDER BY TCV.CalibratorNum Desc "

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(myTestCalibratorValueDS.tparTestCalibratorValues)

                        myGlobalDataTO.SetDatos = myTestCalibratorValueDS

                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, " tparTestCalibratorsDAO.ReadByTestCalibratorIDAndTestID ", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO
        End Function
#End Region

    End Class

End Namespace
