Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.AlarmEnumerates
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports System.Threading.Tasks

Namespace Biosystems.Ax00.BL

    Public Class WSStatusRecalculatorForCtrlPatientsSTD : Inherits WSStatusRecalculatorForNotDeletedExecutions
        Public Sub New(ByVal pMyExecutionsDAO As twksWSExecutionsDAO, ByVal pMyExecutionsDS As ExecutionsDS, ByVal pDbConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String)
            MyBase.New(pMyExecutionsDAO, pMyExecutionsDS, pDbConnection, pAnalyzerID, pWorkSessionID)
        End Sub

        Protected Overrides Function GetExecutions() As List(Of ExecutionsDS.twksWSExecutionsRow)
            Return (From a As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions _
                    Where (a.SampleClass = "CTRL" OrElse a.SampleClass = "PATIENT") _
                    AndAlso a.ExecutionType = "PREP_STD" Select a).ToList()
        End Function

        Protected Overrides Function GetNumberElements() As Integer
            Return (From b As WSOrderTestsForExecutionsDS.WSOrderTestsForExecutionsRow In reqElementsDS.WSOrderTestsForExecutions _
                                                            Where b.ElementStatus = "NOPOS" _
                                                           Select b).ToList.Count()
        End Function

        Protected Overrides Function FinalStatus(ByVal numElem As Integer, ByVal elem As ExecutionsDS.twksWSExecutionsRow) As String
            Dim output = "PENDING"
            If elem.IsLockedByLISNull AndAlso elem.LockedByLIS Then output = "LOCKED"
            If (numElem > 0) Then
                output = "LOCKED"
            ElseIf output = "PENDING" Then
                Dim resultData = VerifyLockedCalibrator(dbConnection, WorkSessionID, AnalyzerID, reqElementsDS.WSOrderTestsForExecutions.First.TestID,
                                                        reqElementsDS.WSOrderTestsForExecutions.First.SampleType)
                If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                    If (DirectCast(resultData.SetDatos, Boolean)) Then
                        output = "LOCKED"
                    Else
                        resultData = VerifyLockedBlank(dbConnection, WorkSessionID, AnalyzerID, reqElementsDS.WSOrderTestsForExecutions.First.TestID,
                                                        reqElementsDS.WSOrderTestsForExecutions.First.SampleType)
                        If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                            If (DirectCast(resultData.SetDatos, Boolean)) Then output = "LOCKED"
                        Else
                            output = "EXIT"
                        End If
                    End If
                Else
                    output = "EXIT"
                End If
            End If

            Return output
        End Function
    End Class
End Namespace

