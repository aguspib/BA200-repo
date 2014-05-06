Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL

    Partial Public Class PreparationTestDataDelegate
        'AG 31/05/2012 - Creation

#Region "CRUD"

#End Region


#Region "Public Methods"

        ''' <summary>
        ''' Gets all std test programming information required to build the TEST or PTEST instructions
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pExecutionID"></param>
        ''' <returns>GlobalDataTo (PreparationsTestDataDS)</returns>
        ''' <remarks>AG 31/05/2012</remarks>
        Public Function GetPreparationSTDTestData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'get the execution data 
                        Dim myExecutionDelegate As New ExecutionsDelegate
                        resultData = myExecutionDelegate.GetExecution(dbConnection, pExecutionID)
                        If Not resultData.HasError Then
                            Dim myWSExecutionDS As New ExecutionsDS
                            myWSExecutionDS = DirectCast(resultData.SetDatos, ExecutionsDS)
                            If myWSExecutionDS.twksWSExecutions.Rows.Count > 0 Then
                                'set the order test id to a local variable.
                                Dim myOrderTestID As Integer = myWSExecutionDS.twksWSExecutions(0).OrderTestID

                                'Get the preparation test data.
                                Dim myPreparationTestDataDAO As New vwksPreparationsTestDataDAO
                                resultData = myPreparationTestDataDAO.ReadByOrderTestID(dbConnection, myOrderTestID)

                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PreparationTestDataDelegate.GetPreparationSTDTestData", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Checks if the execution in parameter will be perform using the PTEST instruction or not
        ''' YES: ByRef pisPTESTInstruction parameter value is TRUE
        ''' NO: ByRef pisPTESTInstruction parameter value is FALSE
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pExecutionID"></param>
        ''' <param name="pisPTESTInstruction"></param>
        ''' <returns>GlobalDataTo with errors, if no error pisPTESTInstruction parameter informs if the instruction to be send is TEST (False) or PTEST (True)</returns>
        ''' <remarks>AG 31/05/2012</remarks>
        Public Function isPTESTinstruction(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionID As Integer, ByRef pisPTESTInstruction As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                pisPTESTInstruction = False
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the preparation test data information
                        resultData = GetPreparationSTDTestData(dbConnection, pExecutionID)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim myPreparationTestDataDS As New PreparationsTestDataDS
                            myPreparationTestDataDS = DirectCast(resultData.SetDatos, PreparationsTestDataDS)

                            If myPreparationTestDataDS.vwksPreparationsTestData.Count > 0 Then
                                Dim myExecutionDelegate As New ExecutionsDelegate
                                resultData = myExecutionDelegate.GetExecution(dbConnection, pExecutionID)
                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    Dim myWSExecutionDS As New ExecutionsDS
                                    myWSExecutionDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                                    If myWSExecutionDS.twksWSExecutions.Rows.Count > 0 Then
                                        'Validate if it's an automatic predilution, Patient and Control.
                                        If myPreparationTestDataDS.vwksPreparationsTestData(0).PredilutionMode = "INST" AndAlso _
                                                                      myWSExecutionDS.twksWSExecutions(0).SampleClass = "PATIENT" Then
                                            'Generate the parameterlist PTEST
                                            pisPTESTInstruction = True
                                        End If

                                    End If
                                End If

                            End If

                        End If
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PreparationTestDataDelegate.isPTESTinstruction", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

#End Region


#Region "Private Methods"

#End Region

    End Class

End Namespace
