Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class twksWSRequiredElementsTubesDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Add the number of needed bottles of a specified size for a Required Element
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pRequiredPosTubes">DataSet containing information about the Required Bottle Identifier,
        '''                                 the Bottle Identifier and the number of needed bottles</param>
        ''' <returns>Global object containing the added record and/or error information</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 11/01/2010 - Changed parameter validations: used "param Is Nothing" instead of "IsNothing(param)"
        '''              SA 14/01/2010 - Error fixed ("Not" was missing when validating if the entry DataSet has information)
        '''              SA 08/02/2012 - Changed the function template
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRequiredPosTubes As WSRequiredElementsTubesDS) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an opened Database Connection
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pRequiredPosTubes Is Nothing) Then
                    Dim cmdText As String = " INSERT INTO twksWSRequiredElementsTubes (ElementID, TubeCode, NumTubes) " & vbCrLf & _
                                            " VALUES(" & pRequiredPosTubes.twksWSRequiredElementsTubes(0).ElementID & ", " & vbCrLf & _
                                                  " '" & pRequiredPosTubes.twksWSRequiredElementsTubes(0).TubeCode & "', " & vbCrLf & _
                                                         pRequiredPosTubes.twksWSRequiredElementsTubes(0).NumTubes & ")" & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End Using

                    dataToReturn.SetDatos = pRequiredPosTubes
                    dataToReturn.HasError = False
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRequiredElementsTubesDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Read current number of needed bottles of a specified size for the informed Required Element
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pElement">Required Element Identifier</param>
        ''' <param name="pTubeCode">Reagent Bottle Identifier</param>
        ''' <returns>DataSet with structure of table twksWSRequiredElementsTubes</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 11/01/2010 - Changed the way of opening a DB Connection to fulfill the new template
        '''                              Returned type was changed from WSRequiredElementsTubesDS to a GlobalDataTO
        '''              SA 08/02/2012 - Changed the function template
        ''' </remarks>
        Public Function ReadByElementIDAndTubeCode(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pElement As Integer, ByVal pTubeCode As String) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT ElementID, TubeCode, NumTubes " & vbCrLf & _
                                                " FROM   twksWSRequiredElementsTubes " & vbCrLf & _
                                                " WHERE  ElementID = " & pElement & vbCrLf & _
                                                " AND    TubeCode = '" & pTubeCode.Trim & "' " & vbCrLf

                        Dim resultData As New WSRequiredElementsTubesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.twksWSRequiredElementsTubes)
                            End Using
                        End Using

                        dataToReturn.SetDatos = resultData
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRequiredElementsTubes.ReadByElementIDAndTubeCode", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Modify the number of needed Bottles of the specified size for the informed Element
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pRequiredPosTubes">DataSet containing information about the Required Bottle Identifier, the Bottle Identifier and the 
        '''                                 number of needed bottles</param>
        ''' <returns>Global object containing the modified record and/or error information</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 11/01/2010 - Error fixed: the name of the table was wrong. Changed parameter validations: 
        '''                              used "param Is Nothing" instead of "IsNothing(param)"
        '''              SA 08/02/2012 - Changed the function template
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRequiredPosTubes As WSRequiredElementsTubesDS) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an opened Database Connection
                    dataToReturn.HasError = True
                    dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pRequiredPosTubes Is Nothing) Then
                    Dim cmdText As String = " UPDATE twksWSRequiredElementsTubes " & vbCrLf & _
                                            " SET    NumTubes = " & pRequiredPosTubes.twksWSRequiredElementsTubes(0).NumTubes & vbCrLf & _
                                            " WHERE  ElementID = " & pRequiredPosTubes.twksWSRequiredElementsTubes(0).ElementID & vbCrLf & _
                                            " AND    TubeCode   = '" & pRequiredPosTubes.twksWSRequiredElementsTubes(0).TubeCode & "' " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        dataToReturn.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End Using

                    dataToReturn.SetDatos = pRequiredPosTubes
                    dataToReturn.HasError = False
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRequiredElementsTubesDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return dataToReturn
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Get the list of bottles required for the specified required Reagent (those with NumTubes > 0)
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pElementId"> Required Element Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ReagentTubeTypesDS with the list of bottles needed for the  
        '''          specified required Element</returns>
        ''' <remarks>
        ''' Created by:  TR 18/09/2009
        ''' Modified by: SA 11/01/2010 - Changed the way of opening a DB Connection to fulfill the new template
        '''              SA 08/02/2012 - Changed the function template
        ''' </remarks>
        Public Function GetAllNeededBottles(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pElementID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT ET.TubeCode, ET.NumTubes AS NumOfBottles, TT.TubeVolume " & vbCrLf & _
                                                " FROM   twksWSRequiredElementsTubes ET INNER JOIN tfmwReagentTubeTypes TT ON TT.TubeCode = ET.TubeCode " & vbCrLf & _
                                                                                                                        " AND TT.ManualUseFlag = 0 " & vbCrLf & _
                                                " WHERE  ET.ElementID = " & pElementID & vbCrLf & _
                                                " AND    ET.NumTubes > 0 " & vbCrLf

                        Dim resultData As New ReagentTubeTypesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.ReagentTubeTypes)
                            End Using
                        End Using
                        
                        myGlobalDataTO.SetDatos = resultData
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRequiredElementsTubes.GetAllNeededBottles", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all information for the Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: GDS 21/04/2010
        ''' Modified by: SA 08/02/2012 - Changed the function template
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksWSRequiredElementsTubes " & vbCrLf & _
                                            " WHERE  ElementID IN (SELECT ElementID FROM twksWSRequiredElements " & vbCrLf & _
                                                                 " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "') " & vbCrLf

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
                GlobalBase.CreateLogActivity(ex.Message, "twksWSRequiredElementsTubesDAO.ResetWS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace
