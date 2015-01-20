Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.DAL.DAO
    Partial Public Class tfmwAnalyzerModelTubesByRingDAO

#Region "Other Methods"
        ''' <summary>
        ''' Get the list of all available Reagents and Additional Solutions Bottles for each RotorType 
        ''' and Ring, in the specified Analyzer Model
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerModel">Analyzer Model</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AnalyzerModelTubesByRingDS with all data 
        '''          of all Reagents and Additional Solutions Bottles by RotorType and RingNumber</returns>
        ''' <remarks>
        ''' Created by:  SA 12/01/2010 
        ''' Modified by: SA 08/11/2010 - Changed the SQL to get the MultiLanguage description of all bottles with
        '''                              MultiLanguageFlag = True
        ''' </remarks>
        Public Function GetAllRotorRingBottles(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerModel As String) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO
            Dim resultData As New AnalyzerModelTubesByRingDS
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError And Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = CType(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String
                        'Dim myGlobalbase As New GlobalBase

                        cmdText = " SELECT   TBR.AnalyzerModel, TBR.RotorType, TBR.RingNumber, RTT.ManualUseFlag, " & _
                                           " RTT.TubeCode, RTT.FixedTubeName AS TubeName, RTT.TubeVolume " & _
                                  " FROM     tfmwAnalyzerModelTubesByRing TBR INNER JOIN tfmwReagentTubeTypes RTT on TBR.TubeType = RTT.TubeCode " & _
                                  " WHERE    TBR.AnalyzerModel = '" & pAnalyzerModel.Trim & "' " & _
                                  " AND      RTT.Status = 1 " & _
                                  " AND      RTT.MultiLanguageFlag = 0 " & _
                                  " UNION " & _
                                  " SELECT   TBR.AnalyzerModel, TBR.RotorType, TBR.RingNumber, RTT.ManualUseFlag, " & _
                                           " RTT.TubeCode, MR.ResourceText AS TubeName, RTT.TubeVolume " & _
                                  " FROM     tfmwAnalyzerModelTubesByRing TBR INNER JOIN tfmwReagentTubeTypes RTT ON TBR.TubeType = RTT.TubeCode " & _
                                                                            " INNER JOIN tfmwMultiLanguageResources MR ON RTT.ResourceID = MR.ResourceID " & _
                                  " WHERE    TBR.AnalyzerModel = '" & pAnalyzerModel.Trim & "' " & _
                                  " AND      RTT.Status = 1 " & _
                                  " AND      RTT.MultiLanguageFlag = 1 " & _
                                  " AND      MR.LanguageID = '" & GlobalBase.GetSessionInfo.ApplicationLanguage & "' " & _
                                  " ORDER BY TBR.RotorType, TBR.RingNumber, RTT.TubeVolume  "

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(resultData.tfmwAnalyzerModelTubesByRing)

                        dataToReturn.SetDatos = resultData
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwAnalyzerModelTubesByRingDAO.GetAllRotorRingBottles", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Get the list of all available Tubes for Patient Samples, Calibrators and Controls for  
        ''' each RotorType and Ring, in the specified Analyzer Model
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerModel">Analyzer Model</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AnalyzerModelTubesByRingDS with all data 
        '''          of all Tubes for Patient Samples, Calibrators and Controls by RotorType and RingNumber</returns>
        ''' <remarks>
        ''' Created by:  SA 12/01/2010
        ''' Modified by: TR 13/01/2010 - On the select command change the enum PreloadedMasterDataEnum.TUBE_TYPES_SAMPLES to string()
        ''' </remarks>
        Public Function GetAllRotorRingTubes(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerModel As String) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO
            Dim resultData As New AnalyzerModelTubesByRingDS
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError And Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = CType(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        'Dim myGlobalbase As New GlobalBase

                        cmdText = " SELECT   TBR.AnalyzerModel, TBR.RotorType, TBR.RingNumber, PMD.ItemID AS TubeCode, " & _
                                           " MR.ResourceText AS TubeName, PMD.Position " & _
                                  " FROM     tfmwAnalyzerModelTubesByRing TBR INNER JOIN tfmwPreloadedMasterData PMD ON TBR.TubeType = PMD.ItemID " & _
                                                                           " INNER JOIN tfmwMultiLanguageResources MR ON PMD.ResourceID = MR.ResourceID " & _
                                  " WHERE    TBR.AnalyzerModel = '" & pAnalyzerModel.Trim & "' " & _
                                  " AND      PMD.SubTableID = '" & PreloadedMasterDataEnum.TUBE_TYPES_SAMPLES.ToString() & "' " & _
                                  " AND      PMD.Status = 1 " & _
                                  " AND      MR.LanguageID = '" & GlobalBase.GetSessionInfo.ApplicationLanguage & "'" & _
                                  " ORDER BY TBR.RotorType, TBR.RingNumber, PMD.Position "

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(resultData.tfmwAnalyzerModelTubesByRing)

                        dataToReturn.SetDatos = resultData
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwAnalyzerModelTubesByRingDAO.GetAllRotorRingTubes", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

#End Region

    End Class
End Namespace

