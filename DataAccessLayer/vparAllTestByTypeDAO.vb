Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types


Namespace Biosystems.Ax00.DAL.DAO

    Public Class vparAllTestByTypeDAO
        Inherits DAOBase

        ''' <summary>
        ''' Read by the TestType and TestID and get the LIS Value
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns>GlobalDataTO containing an string value with the LIS Mapping for the specified TestType/TestID</returns>
        ''' <remarks>
        ''' Created by:  TR 23/04/2013
        ''' Modified by: SA 25/11/2014 - BA-2105 ==> Changed the SQL Query to avoid an error when field LISValue is NULL (an error is raised
        '''                                          when try to convert it to string)
        ''' </remarks>
        Public Function GetLISValue(ByVal pDBConnection As SqlClient.SqlConnection, pTestType As String, pTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT (CASE WHEN LISValue IS NULL THEN '' ELSE LISValue END) AS LISValue " & vbCrLf & _
                                                " FROM    vparAllTestsByType " & vbCrLf & _
                                                " WHERE   TestType = '" & pTestType.Trim & "' " & vbCrLf & _
                                                " AND     TestID   = " & pTestID.ToString

                        Dim myResult As String = String.Empty
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            myResult = CStr(dbCmd.ExecuteScalar)
                        End Using

                        resultData.SetDatos = myResult
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "vparAllTestByTypeDAO.GetLISValue", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read the full content of view vparAllTestsByType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AllTestsByTypeDS with all LIS mappings defined for all Test Types</returns>
        ''' <remarks>
        ''' Created by:  TR 04/03/2013
        ''' Modified by: DL 24/04/2013 Add clausule ORDER BY
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM vparAllTestsByType ORDER BY TestName "


                        Dim myAllTestByTypeDS As New AllTestsByTypeDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myAllTestByTypeDS.vparAllTestsByType)
                            End Using
                        End Using

                        resultData.SetDatos = myAllTestByTypeDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "vparAllTestByTypeDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

    End Class
End Namespace

