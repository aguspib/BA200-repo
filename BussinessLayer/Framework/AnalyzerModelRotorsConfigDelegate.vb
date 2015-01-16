Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL

Namespace Biosystems.Ax00.BL
    Public Class AnalyzerModelRotorsConfigDelegate

#Region "Other Methods"
        ''' <summary>
        ''' Get the maximum Ring number in the specified Analyzer Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param> 
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Analyzer Rotor Type</param>
        ''' <returns>GlobalDataTo containing the maximum Ring number</returns>
        ''' <remarks>
        ''' Created by:  BK 07/12/2009 - Tested: OK
        ''' Modified by: SA 04/01/2010 - Added the parameter for the DB Connection and add code to open 
        '''                              the DB Connection according the new template 
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
                        Dim rotorConfig As New tfmwAnalyzerModelRotorsConfigDAO
                        resultData = rotorConfig.GetMaxRingNumber(dbConnection, pAnalyzerID, pRotorType)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerModelRotorsConfigDelegate.GetMaxRingNumber", EventLogEntryType.Error, False)
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
        ''' Created by:   BK 07/12/2009 - Tested: OK
        ''' Modified by:  SA 04/01/2010 - Changed the way of open the DB Connection to the new template 
        ''' </remarks>
        Public Function GetAnalyzerRotorsConfiguration(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim rotorConfig As New tfmwAnalyzerModelRotorsConfigDAO
                        resultData = rotorConfig.GetAnalyzerRotorsConfiguration(dbConnection, pAnalyzerID)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerModelRotorsConfigDelegate.GetAnalyzerRotorsConfiguration", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the maximum Cell number in the specified Analyzer Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param> 
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pRotorType">Analyzer Rotor Type</param>
        ''' <returns>GlobalDataTo containing the last cell for the maximum ring</returns>
        ''' <remarks>
        ''' Created by:  AG 05/01/20109 - (Tested: OK for Reagents and Samples)
        ''' </remarks>
        Public Function GetRotorMaxCellNumber(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                         ByVal pRotorType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim rotorConfig As New tfmwAnalyzerModelRotorsConfigDAO
                        'Get the analyzer-rotor maximum ring
                        resultData = rotorConfig.GetMaxRingNumber(dbConnection, pAnalyzerID, pRotorType)

                        If Not resultData.HasError Then
                            Dim maxRing As Integer = CType(resultData.SetDatos, Integer)
                            'Get the analyzer-rotor-ring configuration
                            resultData = rotorConfig.GetAnalyzerRotorsRingConfiguration(dbConnection, pAnalyzerID, pRotorType, maxRing)

                            If Not resultData.HasError Then
                                'Get the last cell number
                                Dim MaxCell As Integer = CType(resultData.SetDatos, AnalyzerModelRotorsConfigDS).tfmwAnalyzerModelRotorsConfig(0).LastCellNumber
                                resultData.SetDatos = MaxCell
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerModelRotorsConfigDelegate.GetRotorMaxCellNumber", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the different Rotor Types available for the specified Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer ID</param>
        ''' <returns>GlobalDataTo containing a typed DataSet AnalyzerModelRotorsConfigDS with the all different Rotor Types 
        '''          (excepting the REACTIONS one) available in the specified Analyzer </returns>
        ''' <remarks>
        ''' Created BY : VR 25/01/2010 - (Tested : OK)
        ''' </remarks>
        Public Function GetAnalyzerRotorTypes(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the Analyzer Rotor Types
                        Dim analyzerConfig As New tfmwAnalyzerModelRotorsConfigDAO
                        resultData = analyzerConfig.GetAnalyzerRotorTypes(dbConnection, pAnalyzerID)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerModelRotorsConfigDelegate.GetAnalyzerRotorTypes", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function



        ''' <summary>
        ''' Get the different Rotor Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTo containing a typed DataSet AnalyzerModel</returns>
        ''' <remarks>
        ''' Created BY : DL 12/07/2011
        ''' </remarks>
        Public Function GetAnalyzerModel(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the Analyzer Rotor Types
                        Dim analyzerConfig As New tfmwAnalyzerModelRotorsConfigDAO
                        resultData = analyzerConfig.GetAnalyzerModel(dbConnection)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerModelRotorsConfigDelegate.GetAnalyzerModel", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

    End Class
End Namespace

