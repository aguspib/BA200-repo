Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL

Namespace Biosystems.Ax00.BL
    Public Class ReagentTubeTypesDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Calculate the death volume for a Reagent Bottle according its size. The death volume is calculated based in the Bottle Section 
        ''' and the height in mm defined as not reachable with following formula: DeathVolume = (BottleSection * HEIGHT_BOTTLE_DEATH_VOLUME)/1000  
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBottleType">Code of the Reagent Bottle</param>
        ''' <param name="pBottleSection">Section area (in mm2) defined for the Reagent Bottle according its size</param>
        ''' <returns>GlobalDataTO containing a single value with the death volume (in mL) for the Reagent Bottle according its size</returns>
        ''' <remarks>
        ''' Created by:  SA 02/03/2012
        ''' </remarks>
        Public Function CalculateDeathVolByBottleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pBottleType As String, _
                                                      ByVal pBottleSection As Single) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the residual bottle height in mm (height that can not be used due to the needle cannot reach the volume in it). 
                        Dim mySwParam As New SwParametersDelegate
                        resultData = mySwParam.ReadByParameterName(dbConnection, GlobalEnumerates.SwParameters.HEIGHT_BOTTLE_DEATH_VOLUME.ToString, Nothing)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim residualHeight As Single = (DirectCast(resultData.SetDatos, ParametersDS).tfmwSwParameters.First.ValueNumeric)

                            'Calculate the death volume for the bottle and return it inside the GlobalDataTO
                            resultData.SetDatos = (pBottleSection * residualHeight) / 1000
                            resultData.HasError = False
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ReagentTubeTypesDelegate.CalculateDeathVolByBottleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get information about the Reagent Bottle positioned in the specified Rotor Cell
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pBottlePosition">Rotor Cell Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ReagentTubeTypesDS with all information about the bottle placed in 
        '''          the specified Rotor Cell</returns>
        ''' <remarks>
        ''' Created by: AG 09/06/2011
        ''' </remarks>
        Public Function GetBottleInformationByRotorPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                            ByVal pBottlePosition As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tfmwReagentTubeTypesDAO
                        resultData = myDAO.GetBottleInformationByRotorPosition(dbConnection, pAnalyzerID, pWorkSessionID, pBottlePosition)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwReagentTubeTypesDAO.GetBottleInformationByRotorPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the code of the Reagents and Additional Solutions Bottle with the informed Volume
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTubeVolume">Volume (in ml)</param>
        ''' <returns>GlobalDataTO containing the code of the Reagents and Additional Solutions Bottle with the informed Volume</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: SA 07/01/2010 - Changed the Exception Section: the error was not informed in the GlobalDataTO object; Throw ex was removed  
        '''                              Error fixed: missing AS NEW when declaring the GlobalDataTO 
        '''              SA 07/01/2010 - Return error Master Data Missing when there are not preloaded Reagents and Additional Solutions Bottles in the DB
        '''              RH 31/08/2011 - Remove NEW when declaring the GlobalDataTO. Not needed. It just get memory that will soon become unreferenced when
        '''                              you do the assigment 'bottleVolume = reagentTubeSizesData.GetBottleByVolume(Nothing, pTubeVolume)'
        '''              SA 16/02/2012 - Added parameter for the DB Connection and implemented the correspondent function template
        ''' </remarks>
        Public Function GetBottleByVolume(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTubeVolume As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim reagentTubeSizesData As New tfmwReagentTubeTypesDAO
                        resultData = reagentTubeSizesData.GetBottleByVolume(Nothing, pTubeVolume)

                        If (resultData.SetDatos Is Nothing OrElse resultData.SetDatos.ToString = "") Then
                            resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString()
                            resultData.HasError = True
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ReagentTubeTypesDelegate.GetBottleByVolume", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the maximum Reagent and Additional Solutions bottle size (excluding the one for "death volume" which is only for manual use)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing the volume (in ml) of the biggest Reagents and Additional Solutions bottle</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: SA 07/01/2010 - Changed the Exception Section: the error was not informed in the GlobalDataTO object; Throw ex was removed
        '''                              Error fixed: missing AS NEW when declaring the GlobalDataTO 
        '''              SA 07/01/2010 - Return error Master Data Missing when there are not preloaded Reagents and Additional Solutions Bottles in the DB
        '''              RH 31/08/2011 - Remove NEW when declaring the GlobalDataTO. Not needed. It just get memory that will soon become unreferenced when
        '''                              you do the assigment 'maxBottleSize = reagentTubeSizesData.GetMaximumBottleSize(Nothing)'
        '''              SA 16/02/2012 - Added parameter for the DB Connection and implemented the correspondent function template
        ''' </remarks>
        Public Function GetMaximumBottleSize(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim reagentTubeSizesData As New tfmwReagentTubeTypesDAO
                        resultData = reagentTubeSizesData.GetMaximumBottleSize(dbConnection)

                        If (resultData.SetDatos Is Nothing OrElse resultData.SetDatos.ToString = "") Then
                            resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString()
                            resultData.HasError = True
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ReagentTubeTypesDelegate.GetMaximumBottleSize", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the minimum Reagent and Additional Solutions bottle size (excluding the one for "death volume", which is only for manual use)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing the Volume (in ml) of the smallest Reagents and Additional Solutions bottle</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: SA 07/01/2010 - Changed the Exception Section: the error was not informed in the GlobalDataTO object; Throw ex was removed
        '''                              Error fixed: missing AS NEW when declaring the GlobalDataTO 
        '''              SA 07/01/2010 - Return error Master Data Missing when there are not preloaded Reagents and Additional Solutions Bottles in the DB
        '''              RH 31/08/2011 - Remove NEW when declaring the GlobalDataTO. Not needed. It just get memory that will soon become unreferenced when 
        '''                              you do the assigment 'minBottleSize = reagentTubeSizesData.GetMinimumBottleSize(Nothing)
        '''              SA 16/02/2012 - Added parameter for the DB Connection and implemented the correspondent function template
        ''' </remarks>
        Public Function GetMinimumBottleSize(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim reagentTubeSizesData As New tfmwReagentTubeTypesDAO()
                        resultData = reagentTubeSizesData.GetMinimumBottleSize(dbConnection)

                        If (resultData.SetDatos Is Nothing OrElse resultData.SetDatos.ToString = "") Then
                            resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString()
                            resultData.HasError = True
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ReagentTubeTypesDelegate.GetMinimumBottleSize", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the different sizes of Reagent Bottles currently in use, excluding the one that can be selected only manually from 
        ''' the application (bottle for death volume)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAscending">Optional parameter.  When True, it indicates the list of Bottles has to be sorted ascending by Volume; 
        '''                          when false, the list has to be sorted descending by Volume</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ReagentTubeTypesDS with the Bottle Code, the Theorical Volume and the Real 
        '''          Volume of all Reagents Bottles
        ''' </returns>         
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: BK 07/12/2009 - Returns a GlobalDataTO instead of a typed DataSet - Tested: OK
        '''              SA 07/01/2010 - Changed the Exception Section: the error was not informed in the GlobalDataTO object; Throw ex was removed.
        '''              SA 07/01/2010 - Return error Master Data Missing when there are not preloaded Reagents and Additional Solutions Bottles in the DB 
        '''              SA 02/03/2012 - Removed parameter pResidualVolume
        ''' </remarks>
        Public Function GetReagentBottles(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pAscending As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim reagentBottles As New tfmwReagentTubeTypesDAO
                        resultData = reagentBottles.GetReagentBottles(dbConnection, pAscending)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim reagentBottlesDS As ReagentTubeTypesDS = DirectCast(resultData.SetDatos, ReagentTubeTypesDS)
                            If (reagentBottlesDS.ReagentTubeTypes.Rows.Count = 0) Then
                                resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                                resultData.HasError = True
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ReagentTubeTypesDelegate.GetReagentBottles", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all data of the informed Reagents and Additional Solutions Bottle
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTubeType">Reagents and Additional Solutions Bottle Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ReagentTubeTypesDS with data of the 
        '''          specified Reagents and Additional Solutions Bottle</returns>
        ''' <remarks>
        ''' Created by:  VR 02/12/2009 - Tested: OK
        ''' Modified by: SA 07/01/2010 - Changed the way of open the DB Connection to the new template
        '''              SA 07/01/2010 - Return error Master Data Missing when there are not preloaded Reagents and Additional Solutions Bottles in the DB
        '''              SA 07/01/2010 - Name changed to GetVolumeByTubeType (instead of GetVolumeByTubeSize)
        ''' </remarks>
        Public Function GetVolumeByTubeType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTubeType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim reagentTubeSizesData As New tfmwReagentTubeTypesDAO
                        resultData = reagentTubeSizesData.ReadByTubeCode(dbConnection, pTubeType)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim reagentBottlesDS As ReagentTubeTypesDS = DirectCast(resultData.SetDatos, ReagentTubeTypesDS)

                            If (reagentBottlesDS.ReagentTubeTypes.Rows.Count = 0) Then
                                resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                                resultData.HasError = True
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ReagentTubeTypesDelegate.GetVolumeByTubeSize", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace

