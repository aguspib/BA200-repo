Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.AlarmEnumerates
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports System.Threading.Tasks

Namespace Biosystems.Ax00.BL

    Public MustInherit Class WSStatusRecalculatorForNotDeletedExecutions
        Protected myExecutionsDS As ExecutionsDS
        Protected reqElementsDS As WSOrderTestsForExecutionsDS
        Private dbConnection As SqlConnection
        Private AnalyzerID As String
        Private WorkSessionID As String
        Private myExecutionsDAO As twksWSExecutionsDAO

        Public Sub New(ByVal pMyExecutionsDAO As twksWSExecutionsDAO, ByVal pMyExecutionsDS As ExecutionsDS, ByVal pDbConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String)
            myExecutionsDS = pMyExecutionsDS
            dbConnection = pDbConnection
            AnalyzerID = pAnalyzerID
            WorkSessionID = pWorkSessionID
            myExecutionsDAO = pMyExecutionsDAO
        End Sub

        Public Function Recalculate() As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim noPOSElements As Integer
            Dim myWSOrderTests As New WSOrderTestsDelegate            
            Dim lstSampleClassExecutions As List(Of ExecutionsDS.twksWSExecutionsRow)
            Dim lstToPENDING As List(Of ExecutionsDS.twksWSExecutionsRow) 'AG 19/02/2014 - #1514
            Dim lstToLOCKED As List(Of ExecutionsDS.twksWSExecutionsRow) 'AG 19/02/2014 - #1514


            'Get all BLANK Order Tests having Pending and/or Locked Executions
            lstSampleClassExecutions = GetExecutions()

            For Each element As ExecutionsDS.twksWSExecutionsRow In lstSampleClassExecutions
                'Get the list of Elements required for the Blank Order Test 
                resultData = myWSOrderTests.GetOrderTestsForExecutions(dbConnection, AnalyzerID, WorkSessionID, element.SampleClass, element.OrderTestID)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    reqElementsDS = DirectCast(resultData.SetDatos, WSOrderTestsForExecutionsDS)

                    'Verify if at least one of the required Elements is not positioned
                    noPOSElements = GetNumberElements()

                    'The Executions for the Blank Order Test will be marked as LOCKED if there are not positioned elements
                    element.ExecutionStatus = FinalStatus(noPOSElements)
                Else
                    Exit For
                End If
            Next

            'Finally, update the status of the Executions for each Blank OrderTest
            lstToPENDING = (From a As ExecutionsDS.twksWSExecutionsRow In lstSampleClassExecutions Where a.ExecutionStatus = "PENDING" Select a).ToList
            lstToLOCKED = (From a As ExecutionsDS.twksWSExecutionsRow In lstSampleClassExecutions Where a.ExecutionStatus = "LOCKED" Select a).ToList
            If (Not resultData.HasError) Then resultData = myExecutionsDAO.UpdateStatusByOTAndRerunNumber(dbConnection, lstToPENDING, lstToLOCKED)

            Return resultData
        End Function

        Protected MustOverride Function GetExecutions() As List(Of ExecutionsDS.twksWSExecutionsRow)
        Protected MustOverride Function GetNumberElements() As Integer
        Protected MustOverride Function FinalStatus(ByVal numElem As Integer) As String

    End Class
End Namespace