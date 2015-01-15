Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.BL

    Public Class WSRotorPositionsInProcessDelegate

#Region "Public Methods"
        ''' <summary>
        ''' When analyzer performs the action of add Sample or R2 into preparation this method is called and decrement
        ''' the number of in process tests using this position in rotor
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pRotorType"></param>
        ''' <param name="pCellNumber"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: AG 15/11/2013 - BT #1385
        ''' </remarks>
        Public Function DecrementInProcessTestsNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pRotorType As String, ByVal pCellNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                'Read current contents
                Dim myDAO As New twksWSRotorPositionsInProcessDAO
                Dim myList As New List(Of Integer)
                myList.Add(pCellNumber)

                resultData = myDAO.Read(Nothing, pAnalyzerID, pRotorType, myList)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    Dim myDS As New RotorPositionsInProcessDS
                    myDS = DirectCast(resultData.SetDatos, RotorPositionsInProcessDS)

                    Dim counter As Integer = myDS.twksWSRotorPositionsInProcess.Rows.Count
                    If counter > 0 Then
                        counter = myDS.twksWSRotorPositionsInProcess(0).InProcessTestsNumber
                    End If

                    If counter = 0 Then
                        'Nothing
                    Else
                        resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                            If (Not dbConnection Is Nothing) Then
                                If counter = 1 Then
                                    'Delete
                                    resultData = myDAO.Delete(dbConnection, pAnalyzerID, pRotorType, pCellNumber)

                                ElseIf counter > 1 Then
                                    'Update (decrement)
                                    counter -= 1
                                    With myDS.twksWSRotorPositionsInProcess(0)
                                        .BeginEdit()
                                        .AnalyzerID = pAnalyzerID
                                        .RotorType = pRotorType
                                        .CellNumber = pCellNumber
                                        .InProcessTestsNumber = counter
                                        .EndEdit()
                                    End With

                                    resultData = myDAO.Update(dbConnection, myDS)
                                End If

                                If (Not resultData.HasError) Then
                                    'When the Database Connection was opened locally, then the Commit is executed
                                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                                    'resultData.SetDatos = <value to return; if any>
                                Else
                                    'When the Database Connection was opened locally, then the Rollback is executed
                                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                                End If

                            End If
                        End If

                    End If
                End If
                myList = Nothing

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSRotorPositionsInProcessDelegate.DecrementInProcessTestsNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When analyzer accepts the action of add Sample or R2 into preparation this method is called and increments
        ''' the number of in process tests using this position in rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pLAx00Instr">Instruction in LAX00 format</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: AG 15/11/2013 - BT #1385
        ''' </remarks>
        Public Function IncrementInProcessTestsNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                      ByVal pLAx00Instr As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                'Get the instruction type
                Dim fields As String() = pLAx00Instr.Split(CChar(";"))

                If fields.Count >= 2 Then
                    Dim myInstType As String = pLAx00Instr.Split(CChar(";"))(1)

                    Dim mySampleTubePos As Integer = 0
                    Dim myR2BottlePos As Integer = 0

                    Select Case myInstType
                        Case "TEST", "ISETEST"
                            'Search sample tube position
                            resultData = SearchPosInLAX00Instruction(pLAx00Instr, ";M1:")
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                mySampleTubePos = DirectCast(resultData.SetDatos, Integer)
                            End If

                            'Search reagent2 bottle position
                            resultData = SearchPosInLAX00Instruction(pLAx00Instr, ";R2:")
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                myR2BottlePos = DirectCast(resultData.SetDatos, Integer)
                            End If

                        Case "PTEST"
                            'Search sample tube position
                            resultData = SearchPosInLAX00Instruction(pLAx00Instr, ";PM1:")
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                mySampleTubePos = DirectCast(resultData.SetDatos, Integer)
                            End If

                            'Search reagent2 bottle position
                            resultData = SearchPosInLAX00Instruction(pLAx00Instr, ";R2:")
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                myR2BottlePos = DirectCast(resultData.SetDatos, Integer)
                            End If

                        Case "WRUN"
                            'Search reagent2 bottle position
                            resultData = SearchPosInLAX00Instruction(pLAx00Instr, ";BP2:")
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                myR2BottlePos = DirectCast(resultData.SetDatos, Integer)
                            End If

                    End Select

                    If mySampleTubePos > 0 OrElse myR2BottlePos > 0 Then
                        resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                            If (Not dbConnection Is Nothing) Then

                                Dim myDAO As New twksWSRotorPositionsInProcessDAO
                                Dim myDS As New RotorPositionsInProcessDS
                                Dim rotorType As String = ""

                                Dim myList As New List(Of Integer)
                                Dim rotorTypesGroup() As String = {"SAMPLES", "REAGENTS"}
                                Dim myPosition As Integer = 0

                                For Each rotorType In rotorTypesGroup
                                    myList.Clear()
                                    If rotorType = "SAMPLES" Then myPosition = mySampleTubePos Else myPosition = myR2BottlePos

                                    If myPosition > 0 Then
                                        myList.Add(myPosition)
                                        resultData = myDAO.Read(dbConnection, pAnalyzerID, rotorType, myList)
                                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                            myDS = DirectCast(resultData.SetDatos, RotorPositionsInProcessDS)
                                            Dim counter As Integer = 0
                                            If myDS.twksWSRotorPositionsInProcess.Rows.Count > 0 Then
                                                counter = myDS.twksWSRotorPositionsInProcess(0).InProcessTestsNumber
                                            End If

                                            If counter = 0 Then
                                                myDS.Clear()
                                                Dim newRow As RotorPositionsInProcessDS.twksWSRotorPositionsInProcessRow
                                                newRow = myDS.twksWSRotorPositionsInProcess.NewtwksWSRotorPositionsInProcessRow
                                                newRow.AnalyzerID = pAnalyzerID
                                                newRow.RotorType = rotorType
                                                newRow.CellNumber = myPosition
                                                newRow.InProcessTestsNumber = 1
                                                myDS.twksWSRotorPositionsInProcess.AddtwksWSRotorPositionsInProcessRow(newRow)
                                                myDS.twksWSRotorPositionsInProcess.AcceptChanges()
                                                resultData = myDAO.Create(dbConnection, myDS)
                                            Else
                                                'Update (increment)
                                                counter += 1

                                                With myDS.twksWSRotorPositionsInProcess(0)
                                                    .BeginEdit()
                                                    .AnalyzerID = pAnalyzerID
                                                    .RotorType = rotorType
                                                    .CellNumber = myPosition
                                                    .InProcessTestsNumber = counter
                                                    .EndEdit()
                                                End With
                                                myDS.twksWSRotorPositionsInProcess.AcceptChanges()

                                                resultData = myDAO.Update(dbConnection, myDS)
                                            End If
                                        End If
                                    End If
                                Next
                                myList = Nothing

                                If (Not resultData.HasError) Then
                                    'When the Database Connection was opened locally, then the Commit is executed
                                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                                Else
                                    'When the Database Connection was opened locally, then the Rollback is executed
                                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                                End If
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSRotorPositionsInProcessDelegate.IncrementInProcessTestsNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read contents for an Analyzer, RotorType and Position
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pRotorType"></param>
        ''' <param name="pCellNumberList"></param>
        ''' <returns>GlobalDataTO (RotorPositionsInProcessDS)</returns>
        ''' <remarks>
        ''' Created by:  AG 15/11/2013 - BT #1385
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pRotorType As String, ByVal pCellNumberList As List(Of Integer)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSRotorPositionsInProcessDAO
                        resultData = myDAO.Read(dbConnection, pAnalyzerID, pRotorType, pCellNumberList)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSRotorPositionsInProcessDelegate.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Reagents Rotor positions that are currently In Process for the specified Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet RotorPositionsInProcessDS with all pairs of RotorType/CellNumber 
        '''          that are currently In Process in Reagents Rotor</returns>
        ''' <remarks>
        ''' Created by: SA 20/11/2013
        ''' </remarks>
        Public Function ReadAllReagents(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then

                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSRotorPositionsInProcessDAO
                        resultData = myDAO.ReadAllReagents(dbConnection, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSRotorPositionsInProcessDelegate.ReadAllReagents", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all information about Rotor Positions In Process for the specified Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 15/11/2013 - BT #1385
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSRotorPositionsInProcessDAO
                        resultData = myDAO.ResetWS(dbConnection, pAnalyzerID)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSRotorPositionsInProcessDelegate.ResetWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "Private Methods"
        ''' <summary>
        ''' Search in the received LAX00 Instruction the specified group of characters, returning the Cell Number for which the group of characters was found
        ''' For instance, in LAX00 Instruction: 
        '''         "A400;TEST;TI:1;ID:12;M1:5;TM1:1;RM1:1;R1:3;TR1:6;RR1:1;R2:0;TR2:0;RR2:0;VM1:170;VR1:2400;VR2:0;MW:2;RW:6;RN:68"searching of group of characters 
        ''' ** Searching of ";M1:" (Sample Tube Position)           -> returns 5
        ''' ** Searching of ";R2:" (Second Reagent Bottle Position) -> returns 0
        ''' </summary>
        ''' <param name="pLAx00Instr">Instruction in LAX00 format</param>
        ''' <param name="pToSearch">Group of characters used in the instruction to identify the type of position to search (Sample Tube Position or Reagent Bottle Position)</param>
        ''' <returns>GlobalDataTO with an integer value containing zero when the group of characters was not found in the LAX00 Instruction 
        '''          or the position number when it was found</returns>
        ''' <remarks>
        ''' Created by:  AG 15/11/2013 - BT #1385
        ''' </remarks>
        Private Function SearchPosInLAX00Instruction(ByVal pLAx00Instr As String, ByVal pToSearch As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                resultData.SetDatos = 0

                Dim myTextPos As Integer = InStr(pLAx00Instr, pToSearch)
                If (myTextPos > 0) Then
                    Dim myText As String = pLAx00Instr.Substring((myTextPos - 1) + Len(pToSearch))

                    myTextPos = InStr(myText, ";")
                    If (myTextPos > 0) Then
                        myText = myText.Substring(0, myTextPos - 1)

                        Dim myPosition As Integer = Convert.ToInt32(myText)
                        resultData.SetDatos = myPosition
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSRotorPositionsInProcessDelegate.SearchPosInLAX00Instruction", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "Auxiliary Methods"
        Public Function AUXCreateWholeRotorInProcess(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pRotorType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSRotorPositionsInProcessDAO
                        Dim myDS As New RotorPositionsInProcessDS
                        Dim row As RotorPositionsInProcessDS.twksWSRotorPositionsInProcessRow
                        For i As Integer = 1 To 88
                            row = myDS.twksWSRotorPositionsInProcess.NewtwksWSRotorPositionsInProcessRow
                            row.AnalyzerID = pAnalyzerID
                            row.RotorType = pRotorType
                            row.CellNumber = i
                            row.InProcessTestsNumber = 1
                            myDS.twksWSRotorPositionsInProcess.AddtwksWSRotorPositionsInProcessRow(row)
                        Next
                        myDS.twksWSRotorPositionsInProcess.AcceptChanges()
                        resultData = myDAO.Create(dbConnection, myDS)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSRotorPositionsInProcessDelegate.AUXCreateWholeRotorInProcess", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

    End Class

End Namespace
