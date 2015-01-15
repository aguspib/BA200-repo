Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tfmwAdjustmentsDAO
        Inherits DAOBase

#Region "CRUD Methods"

        ''' <summary>
        ''' Creates a new FW adjustmenst
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 24/01/2012</remarks>
        Public Function CreateMasterData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAdjDS As SRVAdjustmentsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Dim cmdText As String = ""
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else

                    Dim myAdjustmentsDS As SRVAdjustmentsDS
                    'Dim myAdjustmentsDAO As New tfmwAdjustmentsDAO
                    
                    myAdjustmentsDS = pAdjDS

                    Dim i As Integer = 0
                    Dim recordOK As Boolean = True
                    For Each D As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjustmentsDS.srv_tfmwAdjustments.Rows

                        cmdText = ""
                        cmdText &= "INSERT INTO srv_tfmwAdjustments (AnalyzerID, FwVersion, CodeFw, Value, AreaFw, DescriptionFw, GroupID, AxisID, CanSave, CanMove, InFile) " & vbCrLf
                        cmdText &= "VALUES ( N'" & D.AnalyzerID.ToString.Replace("'", "''") & "'" & vbCrLf
                        cmdText &= " , N'" & D.FwVersion.ToString.Replace("'", "''") & "'" & vbCrLf
                        cmdText &= " , N'" & D.CodeFw.ToString.Replace("'", "''") & "'" & vbCrLf
                        cmdText &= " , N'" & D.Value.ToString.Replace("'", "''") & "'" & vbCrLf
                        cmdText &= " , N'" & D.AreaFw.ToString.Replace("'", "''") & "'" & vbCrLf
                        cmdText &= " , N'" & D.DescriptionFw.ToString.Replace("'", "''") & "'" & vbCrLf
                        cmdText &= " , N'" & D.GroupID.ToString.Replace("'", "''") & "'" & vbCrLf
                        cmdText &= " , N'" & D.AxisID.ToString.Replace("'", "''") & "'" & vbCrLf
                        If D.CanSave Then
                            cmdText &= " , 1 " & vbCrLf
                        Else
                            cmdText &= " , 0 " & vbCrLf
                        End If
                        If D.CanMove Then
                            cmdText &= " , 1 " & vbCrLf
                        Else
                            cmdText &= " , 0 " & vbCrLf
                        End If
                        If D.InFile Then
                            cmdText &= " , 1 )" & vbCrLf
                        Else
                            cmdText &= " , 0 )" & vbCrLf
                        End If

                        'Execute the SQL Sentence
                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = cmdText

                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        recordOK = (resultData.AffectedRecords = 1)
                        i += 1

                        If (Not recordOK) Then Exit For
                    Next


                    If (recordOK) Then
                        resultData.HasError = False
                        resultData.AffectedRecords = i
                        resultData.SetDatos = myAdjustmentsDS
                    Else
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                        resultData.AffectedRecords = 0
                    End If

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tfmwAdjustmentsDAO.CreateMasterData", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the specified Analyzer
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>Modified by XBC : 30/09/2011 - Add AnalyzerID received from the Instrument</remarks>
        Public Function GetMasterData(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                 ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String
                        cmdText = " SELECT * " & _
                                  " FROM   srv_tfmwAdjustments " & vbCrLf & _
                                  " WHERE AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "'"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim Adjs As New SRVAdjustmentsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(Adjs.srv_tfmwAdjustments)

                        resultData.SetDatos = Adjs
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tfmwAdjustmentsDAO.GetMasterData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Updates the informed Adjustments values
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAdjustmentsDS">Typed DataSet SRVadjustmentsDS with data of the changed adjustments</param>
        ''' <returns>GlobalDataTO containing a typed DataSet SRVadjustmentsDS with all data of the modified adjustments</returns>
        ''' <remarks>
        ''' Created by:  SGM 20/09/2011
        ''' Modified by XBC : 30/09/2011 - Add AnalyzerID received from the Instrument
        ''' Modified by SGM : 28/11/2011 - Add Firmware Version received from the Instrument
        ''' </remarks>
        Public Function UpdateAdjustmentsDB(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAdjustmentsDS As SRVAdjustmentsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim cmdText As String = ""
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else

                    For Each A As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In pAdjustmentsDS.srv_tfmwAdjustments.Rows

                        cmdText = " UPDATE srv_tfmwAdjustments " & vbCrLf & _
                                           " SET    Value = '" & A.Value & "'"

                        cmdText &= " WHERE CodeFw = '" & A.CodeFw & "'" & vbCrLf

                        cmdText &= " AND AnalyzerID = '" & A.AnalyzerID.Trim.Replace("'", "''") & "'"

                        ' XBC 23/12/2011 - A revisar ! Funciona ?
                        cmdText &= " AND FwVersion = '" & A.FwVersion.Trim.Replace("'", "''") & "'" 'SGM 28/11/2011
                        ' XBC 23/12/2011 - A revisar ! Funciona ?

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = cmdText

                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()

                    Next

                    resultData.SetDatos = pAdjustmentsDS
                    resultData.HasError = False

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tfmwAdjustmentsDAO.UpdateAdjustmentsTable", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Create a new Adjustments
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID">Analyzer identifier</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 30/09/2011
        ''' Modified by SGM : 28/11/2011 - Add Firmware Version
        ''' </remarks>
        Public Function CreateMasterData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pFwVersion As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else

                    Dim myAdjustmentsDS As SRVAdjustmentsDS
                    'Dim myAdjustmentsDAO As New tfmwAdjustmentsDAO
                    resultData = MyClass.GetMasterData(Nothing, "MasterData")
                    If Not resultData.HasError And resultData.SetDatos IsNot Nothing Then
                        myAdjustmentsDS = CType(resultData.SetDatos, SRVAdjustmentsDS)

                        Dim i As Integer = 0
                        Dim recordOK As Boolean = True
                        For Each D As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjustmentsDS.srv_tfmwAdjustments.Rows

                            D.AnalyzerID = pAnalyzerID
                            D.FwVersion = pFwVersion

                            Dim cmdText As String = ""
                            cmdText &= "INSERT INTO srv_tfmwAdjustments (AnalyzerID, FwVersion, CodeFw, Value, AreaFw, DescriptionFw, GroupID, AxisID, CanSave, CanMove, InFile) " & vbCrLf
                            cmdText &= "VALUES ( N'" & D.AnalyzerID.ToString.Replace("'", "''") & "'" & vbCrLf
                            cmdText &= " , N'" & D.FwVersion.ToString.Replace("'", "''") & "'" & vbCrLf
                            cmdText &= " , N'" & D.CodeFw.ToString.Replace("'", "''") & "'" & vbCrLf
                            cmdText &= " , N'" & D.Value.ToString.Replace("'", "''") & "'" & vbCrLf
                            cmdText &= " , N'" & D.AreaFw.ToString.Replace("'", "''") & "'" & vbCrLf
                            cmdText &= " , N'" & D.DescriptionFw.ToString.Replace("'", "''") & "'" & vbCrLf
                            cmdText &= " , N'" & D.GroupID.ToString.Replace("'", "''") & "'" & vbCrLf
                            cmdText &= " , N'" & D.AxisID.ToString.Replace("'", "''") & "'" & vbCrLf
                            If D.CanSave Then
                                cmdText &= " , 1 " & vbCrLf
                            Else
                                cmdText &= " , 0 " & vbCrLf
                            End If
                            If D.CanMove Then
                                cmdText &= " , 1 " & vbCrLf
                            Else
                                cmdText &= " , 0 " & vbCrLf
                            End If
                            If D.InFile Then
                                cmdText &= " , 1 )" & vbCrLf
                            Else
                                cmdText &= " , 0 )" & vbCrLf
                            End If

                            'Execute the SQL Sentence
                            Dim dbCmd As New SqlClient.SqlCommand
                            dbCmd.Connection = pDBConnection
                            dbCmd.CommandText = cmdText

                            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                            recordOK = (resultData.AffectedRecords = 1)
                            i += 1

                            If (Not recordOK) Then Exit For
                        Next


                        If (recordOK) Then
                            resultData.HasError = False
                            resultData.AffectedRecords = i
                            resultData.SetDatos = myAdjustmentsDS
                        Else
                            resultData.HasError = True
                            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                            resultData.AffectedRecords = 0
                        End If

                    Else
                        resultData.HasError = True
                    End If

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tfmwAdjustmentsDAO.CreateMasterData", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete Adjustments
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID">Analyzer identifier</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 17/10/2011
        '''</remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    Dim cmd As New SqlCommand

                    cmdText &= " DELETE FROM  srv_tfmwAdjustments "
                    cmdText &= " WHERE AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "'"

                    cmd.CommandText = cmdText
                    cmd.Connection = pDBConnection

                    resultData.AffectedRecords = cmd.ExecuteNonQuery()

                    If resultData.AffectedRecords > 0 Then
                        resultData.HasError = False
                    Else
                        resultData.HasError = True
                    End If

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tfmwAdjustmentsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        '''' <summary>
        '''' Get all the adjustments from the database
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <returns>GlobalDataTO containing a typed DataSet SRVAdjustmentsDS with all Adjustments data</returns>
        '''' <remarks>Created by:  SGM 20/09/2011</remarks>
        'Public Function ReadAllAdjustmentsDB(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String
        '                cmdText = " SELECT * " & vbCrLf & _
        '                          " FROM   srv_tfmwAdjustments "

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                'Fill the DataSet to return 
        '                Dim myAdjustmentsDS As New SRVAdjustmentsDS
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                dbDataAdapter.Fill(myAdjustmentsDS.srv_tfmwAdjustments)

        '                resultData.SetDatos = myAdjustmentsDS
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tfmwAdjustmentsDAO.ReadAllAdjustmentsDB", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function
#End Region

    End Class

End Namespace