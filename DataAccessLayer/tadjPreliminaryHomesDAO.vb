Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tadjPreliminaryHomesDAO
        Inherits DAOBase

#Region "CRUD Methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pNewAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 20/01/2012</remarks>
        Public Function InsertAnalyzerPreliminaryHomes(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pNewAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myPreliminaryDS As SRVPreliminaryHomesDS
                        resultData = MyClass.GetMasterDataPreliminaryHomes(dbConnection)
                        If Not resultData.HasError And resultData.SetDatos IsNot Nothing Then
                            myPreliminaryDS = CType(resultData.SetDatos, SRVPreliminaryHomesDS)

                            Dim i As Integer = 0
                            Dim recordOK As Boolean = True
                            For Each D As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myPreliminaryDS.srv_tadjPreliminaryHomes.Rows

                                D.AnalyzerID = pNewAnalyzerID

                                Dim cmdText As String = ""
                                cmdText &= "INSERT INTO srv_tadjPreliminaryHomes (AnalyzerID, AdjustmentGroupID,RequiredHomeID, ExeOrder, Done) " & vbCrLf
                                cmdText &= "VALUES ( N'" & D.AnalyzerID.ToString.Replace("'", "''") & "'"
                                cmdText &= " , N'" & D.AdjustmentGroupID.ToString.Replace("'", "''") & "'"
                                cmdText &= " , N'" & D.RequiredHomeID.ToString.Replace("'", "''") & "'"
                                cmdText &= " , " & D.ExeOrder.ToString

                                If D.Done Then
                                    cmdText &= " , 1 "
                                Else
                                    cmdText &= " , 0 "
                                End If

                                cmdText &= ")"

                                'Execute the SQL Sentence
                                Dim dbCmd As New SqlClient.SqlCommand
                                dbCmd.Connection = dbConnection
                                dbCmd.CommandText = cmdText

                                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                                recordOK = (resultData.AffectedRecords = 1)
                                i += 1

                                If (Not recordOK) Then
                                    Exit For
                                End If

                            Next


                            If (recordOK) Then
                                resultData.HasError = False
                                resultData.AffectedRecords = i
                                resultData.SetDatos = myPreliminaryDS
                            Else
                                resultData.HasError = True
                                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                                resultData.AffectedRecords = 0
                            End If

                        Else
                            resultData.HasError = True
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tadjPreliminaryHomesDAO.InsertAnalyzerPreliminaryHomes", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAdjustmentID"></param>
        ''' <returns></returns>
        ''' <remarks>SGM 25/01/2011</remarks>
        Public Function GetPreliminaryHomesByAdjID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pAdjustmentID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String
                        cmdText = " SELECT * " & _
                                  " FROM   srv_tadjPreliminaryHomes "

                        cmdText &= " WHERE AnalyzerID = '" & pAnalyzerID & "'" & _
                        " AND AdjustmentGroupID = '" & pAdjustmentID & "'"


                        cmdText &= " ORDER BY ExeOrder"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim Homes As New SRVPreliminaryHomesDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(Homes.srv_tadjPreliminaryHomes)

                        resultData.SetDatos = Homes
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tadjPreliminaryHomesDAO.GetPreliminaryHomes", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        Public Function GetAllPreliminaryHomes(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String
                        cmdText = " SELECT * " & _
                                  " FROM   srv_tadjPreliminaryHomes "

                        cmdText &= " WHERE AnalyzerID = '" & pAnalyzerID & "'"

                        cmdText &= " ORDER BY ExeOrder"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim Homes As New SRVPreliminaryHomesDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(Homes.srv_tadjPreliminaryHomes)

                        resultData.SetDatos = Homes
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tadjPreliminaryHomesDAO.GetAllPreliminaryHomes", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        'Public Function GetPreliminaryHomesByAdjustmentsDS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAdjustmentsDS As SRVAdjustmentsDS) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        If pAdjustmentsDS IsNot Nothing Then

        '            Dim myAdjustmentGroupList As New List(Of String)
        '            For Each R As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In pAdjustmentsDS.srv_tfmwAdjustments.Rows
        '                If Not myAdjustmentGroupList.Contains(R.GroupID) Then
        '                    myAdjustmentGroupList.Add(R.GroupID)
        '                End If
        '            Next

        '            resultData = GetOpenDBConnection(pDBConnection)
        '            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '                dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '                If (Not dbConnection Is Nothing) Then
        '                    Dim cmdText As String
        '                    cmdText = " SELECT * " & _
        '                              " FROM   srv_tadjPreliminaryHomes "

        '                    Dim i As Integer = 1
        '                    For Each G As String In myAdjustmentGroupList
        '                        If i = 1 Then
        '                            cmdText &= " WHERE  AdjustmentGroupID = '" & G & "'"
        '                        Else
        '                            cmdText &= " OR  AdjustmentGroupID = '" & G & "'"
        '                        End If
        '                        i = i + 1
        '                    Next

        '                    cmdText &= " ORDER BY ExeOrder"

        '                    Dim dbCmd As New SqlClient.SqlCommand
        '                    dbCmd.Connection = dbConnection
        '                    dbCmd.CommandText = cmdText

        '                    'Fill the DataSet to return 
        '                    Dim Homes As New SRVPreliminaryHomesDS
        '                    Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                    dbDataAdapter.Fill(Homes.srv_tadjPreliminaryHomes)

        '                    resultData.SetDatos = Homes
        '                    resultData.HasError = False
        '                End If
        '            End If
        '        End If

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tadjPreliminaryHomesDAO.GetPreliminaryHomes", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function
        ''' <summary>
        ''' Gets Master Data preliminary homes
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>SGM 25/01/2011</remarks>
        Private Function GetMasterDataPreliminaryHomes(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText &= "SELECT * " & vbCrLf
                        cmdText &= "FROM srv_tadjPreliminaryHomes " & vbCrLf
                        cmdText &= "WHERE AnalyzerID = '" & "MasterData" & "'"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim Homes As New SRVPreliminaryHomesDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(Homes.srv_tadjPreliminaryHomes)

                        resultData.SetDatos = Homes
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tadjPreliminaryHomesDAO.GetMasterDataPreliminaryHomes", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pHomeID"></param>
        ''' <returns></returns>
        ''' <remarks>SGM 25/01/2011</remarks>
        Public Function SetPreliminaryHomeAsDone(ByVal pDBConnection As SqlClient.SqlConnection, _
                                               ByVal pAnalyzerID As String, ByVal pHomeID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        Dim cmdText As String = ""

                        cmdText = " UPDATE srv_tadjPreliminaryHomes SET [Done] = " & vbCrLf
                        cmdText &= "'1'"
                        cmdText &= " WHERE RequiredHomeID = '" & pHomeID & "' "
                        cmdText &= " AND AnalyzerID = '" & pAnalyzerID & "' "



                        Dim dbCmd As New SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()

                        'If (resultData.AffectedRecords > 0) Then
                        '    resultData.HasError = False

                        'Else
                        '    resultData.HasError = True
                        '    resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                        'End If

                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tadjPreliminaryHomesDAO.GetAllPreliminaryHomes", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>SGM 25/01/2011</remarks>
        Public Function ResetPreliminaryHomes(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        Dim cmdText As String = ""

                        cmdText = " UPDATE srv_tadjPreliminaryHomes SET [Done] = " & vbCrLf
                        cmdText &= "'0'"
                        cmdText &= " WHERE AnalyzerID = '" & pAnalyzerID & "'"


                        Dim dbCmd As New SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()

                        'If (resultData.AffectedRecords = 1) Then
                        '    resultData.HasError = False

                        'Else
                        '    resultData.HasError = True
                        '    resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                        'End If


                    End If

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tadjPreliminaryHomesDAO.ResetPreliminaryHomes", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Reset an specified Prelimimnary Home
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>XBC 12/11/2012</remarks>
        Public Function ResetSpecifiedPreliminaryHomes(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                       ByVal pAnalyzerID As String, _
                                                       ByVal pHomeID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        Dim cmdText As String = ""

                        cmdText = " UPDATE srv_tadjPreliminaryHomes SET [Done] = " & vbCrLf
                        cmdText &= "'0'"
                        cmdText &= " WHERE RequiredHomeID = '" & pHomeID & "' "
                        cmdText &= " AND AnalyzerID = '" & pAnalyzerID & "' "

                        Dim dbCmd As New SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End If

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tadjPreliminaryHomesDAO.ResetSpecifiedPreliminaryHomes", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        '''' <summary>
        '''' 
        '''' </summary>
        '''' <param name="pDBConnection"></param>
        '''' <param name="pGroupId"></param>
        '''' <returns></returns>
        '''' <remarks>SGM 02/02/2011</remarks>
        'Public Function ResetPreliminaryHomesByGroupID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pGroupId As String) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

        '            If (Not dbConnection Is Nothing) Then

        '                Dim cmdText As String = ""

        '                cmdText = " UPDATE srv_tadjPreliminaryHomes SET [Done] = '0' " & vbCrLf
        '                cmdText &= " WHERE " & vbCrLf
        '                cmdText &= " [AdjustmentGroupID] = '" & pGroupId & "'" & vbCrLf
        '                cmdText &= " [AnalyzerID] = '" & pAnalyzerID & "' "

        '                Dim dbCmd As New SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()

        '                If (resultData.AffectedRecords > 0) Then
        '                    resultData.HasError = False

        '                Else
        '                    resultData.HasError = True
        '                    resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '                End If


        '            End If

        '        End If

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tadjPreliminaryHomesDAO.ResetPreliminaryHomesByGroupID", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function
#End Region

    End Class

End Namespace