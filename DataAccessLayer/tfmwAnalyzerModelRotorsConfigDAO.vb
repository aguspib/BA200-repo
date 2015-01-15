Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO
    Partial Public Class tfmwAnalyzerModelRotorsConfigDAO

#Region "Other Methods"
        ''' <summary>
        ''' Get the maximum Ring number in the specified Analyzer Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Analyzer Rotor Type</param>
        ''' <returns>GlobalDataTo containing the maximum Ring number</returns>
        ''' <remarks>
        ''' Created by:  BK 08/12/2009 - Tested: OK
        ''' Modified by: TR 11/12/2009 - The return value should be an integer not a string 
        '''              SA 04/01/2010 - Changed the way of open the DB Connection to the new template 
        '''              SA 04/01/2010 - Error fixed: query was not filtered by AnalyzerID
        ''' </remarks>
        Public Function GetMaxRingNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                         ByVal pRotorType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'SQL Sentence to get data
                        Dim cmdText As String = ""
                        cmdText = " SELECT MAX(RingNumber) AS RingNumber " & _
                                  " FROM   tfmwAnalyzerModelRotorsConfig AC, tcfgAnalyzers C " & _
                                  " WHERE  AC.AnalyzerModel = C.AnalyzerModel " & _
                                  " AND    AC.RotorType = '" & pRotorType & "' " & _
                                  " AND    C.AnalyzerID = '" & pAnalyzerID & "' "

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim dbDataReader As SqlClient.SqlDataReader
                        dbDataReader = dbCmd.ExecuteReader()
                        If (dbDataReader.HasRows) Then
                            dbDataReader.Read()
                            If (Not dbDataReader.IsDBNull(0)) Then
                                resultData.SetDatos = CType(dbDataReader.Item("RingNumber"), Integer)
                            End If
                        End If
                        dbDataReader.Close()    '05/01/2010 AG
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tfmwAnalyzerModelRotorsConfigDAO.GetMaxRingNumber", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the characteristics of each one of the Rotors of the specified Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param> 
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTo containing typed DataSet AnalyzerModelRotorsConfigDS with 
        '''          configuration of all Rotors (excepting the REACTIONS one) in the informed Analyzer
        ''' </returns>
        ''' <remarks>
        ''' Created by:   BK
        ''' Modified by:  AG 27/11/2009 - Return an AnalyzerModelRotorsConfigDS) -Tested OK
        '''               AG 30/11/2009 - Discard RotorType = REACTIONS) -Tested OK
        '''               SA 04/01/2010 - Changed the way of open the DB Connection to the new template 
        '''               SA 04/01/2010 - Return also the AnalyzerID 
        '''               DL 14/07/2011 - Return also the barcodeflag
        ''' </remarks >
        Public Function GetAnalyzerRotorsConfiguration(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError) Then
                    dbConnection = CType(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        Dim resultData As New AnalyzerModelRotorsConfigDS

                        'SQL Sentence to get data
                        cmdText = " SELECT  C.AnalyzerID, AC.AnalyzerModel, AC.RotorType, AC.RingNumber, AC.FirstCellNumber, AC.LastCellNumber, AC.BarCodeReaderFlag " & _
                                  " FROM    tfmwAnalyzerModelRotorsConfig AC, tcfgAnalyzers C " & _
                                  " WHERE   AC.AnalyzerModel = C.AnalyzerModel " & _
                                  " AND     AC.RotorType <> 'REACTIONS' " & _
                                  " AND     C.AnalyzerID = '" & pAnalyzerID & "'"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(resultData.tfmwAnalyzerModelRotorsConfig)

                        dataToReturn.SetDatos = resultData
                    End If
                End If

            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tfmwAnalyzerModelRotorsConfigDAO.GetAnalyzerRotorsConfiguration", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get the characteristics of each one Ring of the Rotors of the specified Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param> 
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType ">Rotor Identifier</param>
        ''' <param name="pRingNumber">Ring Number</param>
        ''' <returns>GlobalDataTo containing typed DataSet AnalyzerModelRotorsConfigDS with 
        '''          configuration of all Rotors (excepting the REACTIONS one) in the informed Analyzer
        ''' </returns>
        ''' <remarks>
        ''' Created by:   AG 05/01/2010 (Tested: OK for Reagents and Samples)
        ''' </remarks >
        Public Function GetAnalyzerRotorsRingConfiguration(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                           ByVal pRotorType As String, ByVal pRingNumber As Integer) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError) Then
                    dbConnection = CType(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        Dim resultData As New AnalyzerModelRotorsConfigDS

                        'SQL Sentence to get data
                        cmdText = " SELECT  C.AnalyzerID, AC.AnalyzerModel, AC.RotorType, AC.RingNumber, AC.FirstCellNumber, AC.LastCellNumber " & _
                                  " FROM    tfmwAnalyzerModelRotorsConfig AC, tcfgAnalyzers C " & _
                                  " WHERE   AC.AnalyzerModel = C.AnalyzerModel " & _
                                  " AND     C.AnalyzerID = '" & pAnalyzerID & "'" & _
                                  " AND     AC.RotorType = '" & pRotorType & "'" & _
                                  " AND     AC.RingNumber = '" & pRingNumber & "'"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(resultData.tfmwAnalyzerModelRotorsConfig)                        

                        dataToReturn.SetDatos = resultData
                    End If
                End If

            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tfmwAnalyzerModelRotorsConfigDAO.GetAnalyzerRotorsRingConfiguration", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function


        ''' <summary>
        ''' Get the different Rotor Types available for the specified Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer ID</param>
        ''' <returns>GlobalDataTo containing a typed DataSet AnalyzerModelRotorsConfigDS with the all different Rotor Types 
        '''          (excepting the REACTIONS one) available in the specified Analyzer </returns>
        ''' <remarks>
        ''' Created by:  VR 25/01/2010 - (Tested : OK)
        ''' </remarks>
        Public Function GetAnalyzerRotorTypes(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError) Then
                    dbConnection = CType(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        Dim resultData As New AnalyzerModelRotorsConfigDS

                        'SQL Sentence to get data
                        cmdText = " SELECT  DISTINCT C.AnalyzerID, AC.RotorType  " & _
                                  " FROM    tfmwAnalyzerModelRotorsConfig AC, tcfgAnalyzers C " & _
                                  " WHERE   AC.AnalyzerModel = C.AnalyzerModel " & _
                                  " AND     C.AnalyzerID = '" & pAnalyzerID & "'" & _
                                  " AND     AC.RotorType <> 'REACTIONS'"


                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(resultData.tfmwAnalyzerModelRotorsConfig)

                        dataToReturn.SetDatos = resultData
                    End If
                End If

            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tfmwAnalyzerModelRotorsConfigDAO.GetAnalyzerRotorTypes", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function


        ''' <summary>
        ''' Get the Analyzer Models
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param> 
        ''' <returns>GlobalDataTo containing typed DataSet AnalyzerModelDS 
        ''' </returns>
        ''' <remarks>
        ''' Created by: DL 12/07/2011
        ''' </remarks >
        Public Function GetAnalyzerModel(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError) Then
                    dbConnection = CType(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        Dim resultData As New AnalyzerModelDS

                        'SQL Sentence to get data
                        cmdText &= "SELECT  AnalyzerModel, NumberOfRotors, NonCooledRackFlag, ISEModuleFlag " & vbCrLf
                        cmdText &= "  FROM  tfmwAnalyzerModels " & vbCrLf

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(resultData.tfmwAnalyzerModels)

                        dataToReturn.SetDatos = resultData
                    End If
                End If

            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tfmwAnalyzerModelRotorsConfigDAO.GetAnalyzerModel", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function
#End Region
    End Class
End Namespace

