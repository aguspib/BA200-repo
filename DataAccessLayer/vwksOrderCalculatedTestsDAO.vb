Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class vwksOrderCalculatedTestsDAO
        Inherits DAOBase

        ''' <summary>
        ''' Get data (ViewOrderCalculatedTestsDS) with all the OrderTestID (CALC) that used the entry parameter OrderTestID (STD)
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pOrderTestID">Identifier of the Calculated Test</param>
        ''' <returns>ViewOrderCalculatedTestsDS with all the OrderTestID (CALC) that used the entry parameter OrderTestID (STD)
        ''' </returns>
        ''' <remarks>
        ''' Created by:  RH 05/17/2010
        ''' </remarks>
        Public Function ReadByOrderTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try

                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else

                    Dim cmdText As String = _
                        String.Format( _
                        " SELECT *" & _
                        " FROM vwksOrderCalculatedTests" & _
                        " WHERE  OrderTestID = {0}", pOrderTestID)

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    'Fill the DataSet to return 
                    Dim viewOrderCalculatedTests As New ViewOrderCalculatedTestsDS
                    Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                    dbDataAdapter.Fill(viewOrderCalculatedTests.vwksOrderCalculatedTests)

                    resultData.SetDatos = viewOrderCalculatedTests
                    resultData.HasError = False
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "vwksOrderCalculatedTestsDAO.ReadByStdOrderTest", EventLogEntryType.Error, False)
            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Get data (ViewOrderCalculatedTestsDS) with all the OrderTestID (STD) that are part of the entry parameter OrderTestID (CALC)
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pCalcOrderTestID">Identifier of the Calculated Test</param>
        ''' <returns>ViewOrderCalculatedTestsDS with all the OrderTestID (STD) that are part of the entry parameter OrderTestID (CALC)
        ''' </returns>
        ''' <remarks>
        ''' Created by:  RH 05/17/2010
        ''' </remarks>
        Public Function ReadByCalcOrderTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcOrderTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try

                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else

                    Dim cmdText As String = _
                        String.Format( _
                        " SELECT *" & _
                        " FROM vwksOrderCalculatedTests" & _
                        " WHERE  CalcOrderTestID = {0}", pCalcOrderTestID)

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    'Fill the DataSet to return 
                    Dim orderCalculatedTests As New ViewOrderCalculatedTestsDS
                    Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                    dbDataAdapter.Fill(orderCalculatedTests.vwksOrderCalculatedTests)

                    resultData.SetDatos = orderCalculatedTests
                    resultData.HasError = False
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "vwksOrderCalculatedTestsDAO.ReadByCalcOrderTest", EventLogEntryType.Error, False)
            End Try

            Return resultData

        End Function

    End Class

End Namespace
