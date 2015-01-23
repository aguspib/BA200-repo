Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class tfmwReagentTubeTypesDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Get all data of the informed Reagents and Additional Solutions Bottle
        ''' </summary>
        ''' <param name="pDBConnection"> Open DB Connection</param>
        ''' <param name="pTubeType">Reagents and Additional Solutions Bottle Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ReagentTubeTypesDS with data of the 
        '''          specified Reagents and Additional Solutions Bottle</returns>
        ''' <remarks>
        ''' Created by:  VR 02/12/2009 - Tested: OK
        ''' Modified by: SA 07/01/2010 - Changed the way of open the DB Connection to the new template
        '''              SA 07/01/2010 - Changed the function name to ReadByTubeCode. Function moved to CRUD Region
        '''              SA 16/02/2012 - Changed the function template
        ''' </remarks>
        Public Function ReadByTubeCode(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTubeType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tfmwReagentTubeTypes " & vbCrLf & _
                                                " WHERE  TubeCode = '" & pTubeType.Trim & "' " & vbCrLf

                        Dim myReagentTubeTypesDS As New ReagentTubeTypesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myReagentTubeTypesDS.ReagentTubeTypes)
                            End Using
                        End Using

                        resultData.SetDatos = myReagentTubeTypesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwReagentTubeTypesDAO.ReadByTubeCode", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "Other Methods"

        ''' <summary>
        ''' Get the code of the Reagents and Additional Solutions Bottle with the informed Volume
        ''' </summary>
        ''' <param name="pTubeVolume">Volume (in ml)</param>
        ''' <returns>GlobalDataTO containing the code of the Reagents and Additional Solutions 
        '''          Bottle with the informed Volume</returns>
        ''' <remarks>
        ''' Created by:  BK 08/12/2009 - Tested: OK
        ''' Modified by: SA 07/01/2010 - Changed the way of open the DB Connection to the new template; changed the use
        '''                              of a DataReader for an ExecuteScalar
        '''              SA 07/01/2010 - Changed name of the function to GetBottleByVolume. Query changed from 
        '''                              "TubeCode AS TubeVolume" to "TubeCode AS BottleCode"
        '''              SA 12/01/2010 - Added filter by Status to get only active Reagent Bottles
        ''' </remarks>
        Public Function GetBottleByVolume(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTubeVolume As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TubeCode AS BottleCode " & vbCrLf & _
                                                " FROM   tfmwReagentTubeTypes " & vbCrLf & _
                                                " WHERE  TubeVolume = " & pTubeVolume & vbCrLf & _
                                                " AND    Status = 1 " & vbCrLf

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            resultData.SetDatos = dbCmd.ExecuteScalar()
                            resultData.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwReagentTubeTypesDAO.GetBottleByVolume", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get information about the type of bottle positioned in an specific Cell in REAGENTS Rotor
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pBottlePosition">Number of cell in Reagents Rotor</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ReagentTubeTypesDS with all information about the bottle
        '''          positioned in the specified Rotor Cell</returns>
        ''' <remarks>
        ''' Created by:  AG 09/06/2011
        ''' Modified by: SA 16/02/2012 - Changed the function template
        ''' </remarks>
        Public Function GetBottleInformationByRotorPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                            ByVal pWorkSessionID As String, ByVal pBottlePosition As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT RT.TubeCode, RT.TubeVolume, RT.ManualUseFlag, RT.Status, RT.Section " & vbCrLf & _
                                                " FROM   tfmwReagentTubeTypes RT INNER JOIN twksWSRotorContentByPosition RCP ON RT.TubeCode = RCP.TubeType " & vbCrLf & _
                                                " WHERE  RCP.AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    RCP.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    RCP.RotorType = 'REAGENTS' " & vbCrLf & _
                                                " AND    RCP.CellNumber = " & pBottlePosition & vbCrLf

                        Dim myDS As New ReagentTubeTypesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDS.ReagentTubeTypes)
                            End Using
                        End Using

                        resultData.HasError = False
                        resultData.SetDatos = myDS
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
        ''' Get the maximum Reagent and Additional Solutions bottle size (excluding the one for "death volume",
        ''' which is only for manual use)
        ''' </summary>
        ''' <returns>GlobalDataTO containing the volume (in ml) of the biggest Reagents and Additional Solutions bottle</returns>
        ''' <remarks>
        ''' Created by:  BK 08/12/2009 - Tested: OK
        ''' Modified by: SA 07/01/2010 - Changed the way of open the DB Connection to the new template; changed the use
        '''                              of a DataReader for an ExecuteScalar
        '''              SA 12/01/2010 - Added filter by Status to get only active Reagent Bottles
        '''              SA 16/02/2012 - Changed the function template
        ''' </remarks>
        Public Function GetMaximumBottleSize(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT MAX(TubeVolume) AS MaxBottleSize " & vbCrLf & _
                                                " FROM   tfmwReagentTubeTypes " & vbCrLf & _
                                                " WHERE  ManualUseFlag = 0 " & vbCrLf & _
                                                " AND    Status = 1 " & vbCrLf

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            resultData.SetDatos = dbCmd.ExecuteScalar()
                            resultData.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwReagentTubeTypesDAO.GetMaximumBottleSize", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the minimum Reagent and Additional Solutions bottle size (excluding the one for "death volume",
        ''' which is only for manual use)
        ''' </summary>
        ''' <returns>GlobalDataTO containing the Volume (in ml) of the smallest Reagents and Additional Solutions bottle</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: BK 08/12/2009 - Tested: OK
        '''              TR 11/12/2009 - Changed the return value type: it was a String and should be an Integer
        '''              SA 07/01/2010 - Changed the way of open the DB Connection to the new template; changed the use
        '''                              of a DataReader for an ExecuteScalar
        '''              SA 12/01/2010 - Added filter by Status to get only active Reagent Bottles
        '''              SA 16/02/2012 - Changed the function template
        ''' </remarks>
        Public Function GetMinimumBottleSize(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT MIN(TubeVolume) AS MinBottleSize " & vbCrLf & _
                                                " FROM   tfmwReagentTubeTypes " & vbCrLf & _
                                                " WHERE  ManualUseFlag = 0 " & vbCrLf & _
                                                " AND    Status = 1 " & vbCrLf

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            resultData.SetDatos = dbCmd.ExecuteScalar()
                            resultData.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwReagentTubeTypesDAO.GetMinimumBottleSize", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the different sizes of Reagent Bottles currently in use, excluding the one that can be selected only manually from 
        ''' the application (bottle for death volume)
        ''' </summary>
        ''' <param name="pAscending">Optional parameter.  When True, it indicates the list of Bottles has to be sorted ascending by Volume; 
        '''                          when false, the list has to be sorted descending by Volume</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ReagentTubeTypesDS with the Bottle Code, the Theorical Volume and the Real Volume 
        '''          of all available Reagents Bottles
        ''' </returns>   
        ''' <remarks>
        ''' Created by: 
        ''' Modified by: BK 08/12/2009 - Returns a GlobalDataTO instead of a typed DataSet - Tested: OK
        '''              SA 07/01/2010 - Changed the way of open the DB Connection to the new template 
        '''              SA 12/01/2010 - Added filter by Status to get only active Reagent Bottles
        '''              SA 16/02/2012 - Changed the function template. Changed formula to calculate the RealVolume, it has to be
        '''                              the TubeVolume - (TubeVolume*ResidualVolume) due to ResidualVolume is expressed as percentage
        '''              AG 22/02/2012 - Changed formula to calculate the RealVolume due to the Residual Volume is a volume in mL, not a 
        '''                              percentage; new formula has to be TubeVolume - ResidualVolume
        '''              SA 02/03/2012 - Removed parameter pResidualVolume. Changed the query to exclude calculation of the RealVolume
        ''' </remarks>
        Public Function GetReagentBottles(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pAscending As Boolean = False) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TubeCode, TubeVolume, 0 AS NumOfBottles, Section, 0 AS RealVolume " & vbCrLf & _
                                                " FROM   tfmwReagentTubeTypes " & vbCrLf & _
                                                " WHERE  ManualUseFlag = 0 " & vbCrLf & _
                                                " AND    Status = 1 " & vbCrLf & _
                                                " ORDER BY TubeVolume "
                        If (Not pAscending) Then cmdText &= " DESC "

                        Dim resultData As New ReagentTubeTypesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(resultData.ReagentTubeTypes)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwReagentTubeTypesDAO.GetReagentBottles", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

#End Region
    End Class
End Namespace

