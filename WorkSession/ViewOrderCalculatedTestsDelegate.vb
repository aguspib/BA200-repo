Option Explicit On
Option Strict On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO


Namespace Biosystems.Ax00.BL

    Public Class ViewOrderCalculatedTestsDelegate

#Region "Public Functions"

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
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New vwksOrderCalculatedTestsDAO
                        resultData = myDAO.ReadByOrderTest(pDBConnection, pOrderTestID)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ViewOrderCalculatedTestsDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
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
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New vwksOrderCalculatedTestsDAO
                        resultData = myDAO.ReadByCalcOrderTest(pDBConnection, pCalcOrderTestID)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ViewOrderCalculatedTestsDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

    End Class

End Namespace
