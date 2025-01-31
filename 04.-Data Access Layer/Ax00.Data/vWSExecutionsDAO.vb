﻿Imports Biosystems.Ax00.Data.Interfaces
Imports vWSExecutionsDSTableAdapters
Imports System.Data.SqlClient

Namespace Biosystems.Ax00.Data
    ''' <summary>
    ''' Class which encapsulates the callings for database objects
    ''' </summary>
    ''' <remarks></remarks>
    Public Class vWSExecutionsDAO
        Implements IvWSExecutionsDAO

        ''' <summary>
        ''' Standard constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()

        End Sub

        ''' <summary>
        ''' Retrieves all the information related to the current WSExecution, 
        ''' in a way that the current context is able to deal with
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetInfoExecutions() As vWSExecutionsDS
            Dim result As New vWSExecutionsDS()
            Dim aux As New vWSExecutionsDSTableAdapters.vWSExecutionsSELECTTableAdapter()

            'AJG. If this command throws and exception, it needs to be catched up by the upper layer
            aux.Fill(result.vWSExecutionsSELECT)

            Return result
        End Function

        ''' <summary>
        ''' Retrieves all the information related to a single execution inside the current
        ''' WSExecution, in a way that the current context is able to deal with
        ''' </summary>
        ''' <param name="executionID">the execution Id to look for</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetInfoExecutionByExecutionID(executionID As Integer) As vWSExecutionsDS
            Dim result As New vWSExecutionsDS()
            Dim aux As New vWSExecutionsDSTableAdapters.vWSExecutionsSELECTTableAdapter()

            'AJG. If this command throws and exception, it needs to be catched up by the upper layer
            aux.FillByExecutionID(result.vWSExecutionsSELECT, executionID)

            Return result
        End Function

        ''' <summary>
        ''' Retrieves the required washing solution, used in a given WRUN instruction
        ''' If the washing solution is distilled water, it returns 'DISTW'
        ''' Otherwise, it returns the Washing solution code.
        ''' </summary>
        ''' <param name="ExecutionID"></param>
        ''' <param name="AnalyzerID"></param>
        ''' <param name="WorkSessionID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetWashingSolution(ExecutionID As Integer, AnalyzerID As String, WorkSessionID As String) As vWSExecutionsDS Implements IvWSExecutionsDAO.GetWashingSolution
            Dim result As New vWSExecutionsDS()
            Dim aux As New vWSExecutionsDSTableAdapters.WashingSolutionSELECTTableAdapter()

            'AJG. If this command throws and exception, it needs to be catched up by the upper layer
            aux.Fill(result.WashingSolutionSELECT, ExecutionID.ToString(), AnalyzerID, WorkSessionID)

            Return result
        End Function

        Public Function GetBireactivesContext() As vWSExecutionsDS.twksWSBbireactiveContextDataTable
            Dim result As New vWSExecutionsDS.twksWSBbireactiveContextDataTable
            Dim aux As New vWSExecutionsDSTableAdapters.twksWSBbireactiveContextTableAdapter
            aux.Fill(result)
            Return result
        End Function

        Public Sub SaveBireactivesContext(value As vWSExecutionsDS.twksWSBbireactiveContextDataTable)
            Dim aux As New vWSExecutionsDSTableAdapters.twksWSBbireactiveContextTableAdapter
            aux.Update(value)
        End Sub

        Public Sub ClearBireactivesContext()
            Dim aux As New vWSExecutionsDSTableAdapters.twksWSBbireactiveContextTableAdapter
            Try

                Dim sqlTrunc As String = "TRUNCATE TABLE twksWSBbireactiveContext"
                aux.Connection.Open()
                Dim cmd As SqlCommand = New SqlCommand(sqlTrunc, aux.Connection)
                cmd.ExecuteNonQuery()
            Catch
                Throw
            Finally
                If aux IsNot Nothing Then aux.Connection.Close()
            End Try
        End Sub

    End Class
End Namespace