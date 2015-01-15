Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class vwksOperateCalculatedTestDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' For the specified OrderTestID, search if there is an accepted and validated Result
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OperateCalculatedTestDS containing the TestType, TestID, SampleType
        '''          and the accepted and validated Result for the specified OrderTestID
        ''' </returns>
        ''' <remarks>
        ''' Created by:  RH 13/05/2010
        ''' Modified by: SA 20/04/2012 - Changed the function template
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = String.Format(" SELECT * FROM vwksOperateCalculatedTest " & _
                                                              " WHERE  OrderTestID = {0} ", pOrderTestID)

                        Dim operateCalculatedTest As New OperateCalculatedTestDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(operateCalculatedTest.vwksOperateCalculatedTest)
                            End Using
                        End Using

                        resultData.SetDatos = operateCalculatedTest
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OperateCalculatedTestDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Receive the Formula of a Calculate Test as an string and sent it to SQL to calculate the Result
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pFormula">Formula to evaluate in order to get the Result for a Calculated Test</param>
        ''' <returns>GlobalDataTO containing a double value with the Result calculated after applied the Formula for the Test
        ''' </returns>
        ''' <remarks>
        ''' Created by:  RH 18/05/2010
        ''' Modified by: RH 13/09/2010 - Calculations Errors Validation
        '''              SA 20/04/2012 - Changed the function template
        '''              SA 07/11/2012 - Changed the SQL Query to be sure the returned value is not bigger than a DOUBLE, due to if it happens,
        '''                              the DataReader raises an overflow error when value of Item("Result") is get (it is a problem of the
        '''                              SqlDataReader itself). Besides, when an error happens, the DataReader has to be closed due to this 
        '''                              function is called several times inside a recursive process 
        ''' </remarks>
        Public Function Evaluate(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pFormula As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbDataReader As SqlClient.SqlDataReader
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Dim cmdText As String = String.Format("SELECT {0} AS Result", pFormula)
                        Dim cmdText As String = String.Format("SELECT CAST({0} AS FLOAT) AS Result", pFormula)

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            dbDataReader = dbCmd.ExecuteReader()

                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                resultData.SetDatos = CDbl(dbDataReader.Item("Result"))
                                resultData.HasError = False
                            Else
                                resultData.SetDatos = GlobalConstants.CALCULATION_ERROR_VALUE
                                resultData.ErrorCode = GlobalEnumerates.ConcentrationErrors.OUT.ToString()
                                resultData.HasError = False
                            End If
                            dbDataReader.Close()
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OperateCalculatedTestDAO.Evaluate", EventLogEntryType.Error, False)
            Finally
                'If an error has happened and the DataReader has been opened, then it has to be closed due to this function is 
                'called from a recursive function
                If (Not dbDataReader Is Nothing AndAlso Not dbDataReader.IsClosed) Then dbDataReader.Close()
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace
