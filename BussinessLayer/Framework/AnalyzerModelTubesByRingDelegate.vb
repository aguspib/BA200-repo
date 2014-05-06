Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL

Namespace Biosystems.Ax00.BL
    Public Class AnalyzerModelTubesByRingDelegate

#Region "Other Methods"
        ''' <summary>
        ''' Get the list of all available Reagents and Additional Solutions Bottles for each RotorType 
        ''' and Ring, in the specified Analyzer Model
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerModel">Analyzer Model</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AnalyzerModelTubesByRingDS with all data of all
        '''          available Reagents and Additional Solutions Bottles by RotorType and RingNumber</returns>
        ''' <remarks>
        ''' Created by:  AG 23/11/2009 - Tested: OK 26/11/2009
        ''' Modified by: SA 07/01/2010 - Changed the way of open the DB Connection to the new template
        '''              SA 07/01/2010 - Return error Master Data Missing when there are not preloaded
        '''                              Reagents and Additional Solutions Bottles in the DB
        '''              SA 12/01/2010 - Added parameter for Analyzer Model; call function GetAllRotorRingBottles 
        '''                              instead of ReadAll. Moved from ReagentTubeTypesDelegate to this class and
        '''                              changes derived from this class change. 
        ''' </remarks>
        Public Function GetAllBottles(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerModel As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim reagentBottlesDS As New AnalyzerModelTubesByRingDS
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim reagentTubesData As New tfmwAnalyzerModelTubesByRingDAO
                        resultData = reagentTubesData.GetAllRotorRingBottles(dbConnection, pAnalyzerModel)

                        If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                            reagentBottlesDS = CType(resultData.SetDatos, AnalyzerModelTubesByRingDS)
                            If (reagentBottlesDS.tfmwAnalyzerModelTubesByRing.Rows.Count = 0) Then
                                resultData.HasError = True
                                resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerModelTubesByRingDelegate.GetAllBottles", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all available Tubes for Patient Samples, Calibrators and Control for  
        ''' each RotorType and Ring, in the specified Analyzer Model
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerModel">Analyzer Model</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AnalyzerModelTubesByRingDS with all data 
        '''          of all Tubes for Patient Samples, Calibrators and Controls by RotorType and RingNumber</returns>
        ''' <remarks>
        ''' Created by: SA 12/01/2010 
        ''' </remarks>
        Public Function GetAllTubes(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerModel As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim sampleTubesDS As New AnalyzerModelTubesByRingDS
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim sampleTubesData As New tfmwAnalyzerModelTubesByRingDAO
                        resultData = sampleTubesData.GetAllRotorRingTubes(dbConnection, pAnalyzerModel)

                        If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                            sampleTubesDS = CType(resultData.SetDatos, AnalyzerModelTubesByRingDS)
                            If (sampleTubesDS.tfmwAnalyzerModelTubesByRing.Rows.Count = 0) Then
                                resultData.HasError = True
                                resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerModelTubesByRingDelegate.GetAllTubes", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

    End Class
End Namespace
