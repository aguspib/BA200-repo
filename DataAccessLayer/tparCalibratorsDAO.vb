Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Text

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Partial Public Class tparCalibratorsDAO
      

#Region "CRUD Methods"
    ''' <summary>
    ''' Add a new Calibrator to Calibrators table
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pCalibratorsDS">Typed DataSet CalibratorsDS containing the data of the Calibrator to add</param>
    ''' <returns>GlobalDataTO containing the entry typed DataSet CalibratorsDS updated with value
    '''          of the CalibratorID generated automatically for the DB when the insert was successful; 
    '''          otherwise, the error information is returned</returns>
    ''' <remarks>
    ''' Created by:  VR 31/05/2010 - Tested: Pending
    ''' Modified by: SA 13/10/2010 - Added Replace in String fields
    '''              SA 27/10/2010 - Added N preffix for multilanguage of field TS_User. Other changes: NumberOfCalibrators
    '''                              is a mandatory field, InUse is always false when adding, if TS_User is not informed in
    '''                              the DS, get the ID of the current logged user, if TS_DateTime is not informed in the DS,
    '''                              use the current date and time
    ''' </remarks>
    Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibratorsDS As CalibratorsDS, Optional pIsFactory As Boolean = False) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                'SQL Sentence to insert data
                Dim cmdText As String = ""

                cmdText &= "INSERT INTO "

                If pIsFactory Then cmdText &= GlobalBase.TemporalDBName & "."

                cmdText &= "dbo.tparCalibrators" & vbCrLf
                cmdText &= "           (CalibratorName" & vbCrLf
                cmdText &= "           ,LotNumber" & vbCrLf
                cmdText &= "           ,NumberOfCalibrators" & vbCrLf
                cmdText &= "           ,InUse" & vbCrLf
                cmdText &= "           ,ExpirationDate" & vbCrLf
                cmdText &= "           ,TS_User" & vbCrLf
                cmdText &= "           ,TS_DateTime)" & vbCrLf
                cmdText &= "VALUES" & vbCrLf
                cmdText &= "           (N'" & pCalibratorsDS.tparCalibrators(0).CalibratorName.Replace("'", "''") & "'" & vbCrLf
                cmdText &= "           ,N'" & pCalibratorsDS.tparCalibrators(0).LotNumber.Replace("'", "''") & "'" & vbCrLf
                cmdText &= "           ," & pCalibratorsDS.tparCalibrators(0).NumberOfCalibrators & vbCrLf
                cmdText &= "           ,0" & vbCrLf

                If (pCalibratorsDS.tparCalibrators(0).IsExpirationDateNull) Then
                    cmdText &= "           ,NULL" & vbCrLf
                Else
                    cmdText &= "           ,'" & pCalibratorsDS.tparCalibrators(0).ExpirationDate.ToString("yyyyMMdd") & "'" & vbCrLf
                End If

                If (pCalibratorsDS.tparCalibrators(0).IsTS_UserNull) Then
                    'Dim myGlobalbase As New GlobalBase
                    cmdText &= "           ,N'" & GlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "'" & vbCrLf
                Else
                    cmdText &= "           ,N'" & pCalibratorsDS.tparCalibrators(0).TS_User.Trim.Replace("'", "''") & "'" & vbCrLf
                End If

                If (pCalibratorsDS.tparCalibrators(0).IsTS_DateTimeNull) Then
                    cmdText &= "           ,'" & Now.ToString("yyyyMMdd HH:mm:ss") & "') "
                Else
                    cmdText &= "           ,'" & pCalibratorsDS.tparCalibrators(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "') "
                End If

                'Add the scope identity because the ID is generated automatically by the database
                cmdText &= "SELECT SCOPE_IDENTITY()"

                'cmdText = " INSERT INTO tparCalibrators (CalibratorName, LotNumber, NumberOfCalibrators, InUse, " & _
                '                                       " ExpirationDate, TS_User,TS_DateTime) " & _
                '          " VALUES( N'" & pCalibratorsDS.tparCalibrators(0).CalibratorName.Replace("'", "''") & "', " & _
                '                  " N'" & pCalibratorsDS.tparCalibrators(0).LotNumber.Replace("'", "''") & "', " & _
                '                          pCalibratorsDS.tparCalibrators(0).NumberOfCalibrators & ", 0, "

                'If (pCalibratorsDS.tparCalibrators(0).IsExpirationDateNull) Then
                '    cmdText &= " NULL, "
                'Else
                '    cmdText &= " '" & pCalibratorsDS.tparCalibrators(0).ExpirationDate.ToString("yyyyMMdd") & "', "
                'End If

                'If (pCalibratorsDS.tparCalibrators(0).IsTS_UserNull) Then
                '    'Dim myGlobalbase As New GlobalBase
                '    cmdText &= " N'" & GlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', "
                'Else
                '    cmdText &= " N'" & pCalibratorsDS.tparCalibrators(0).TS_User.Trim.Replace("'", "''") & "', "
                'End If

                'If (pCalibratorsDS.tparCalibrators(0).IsTS_DateTimeNull) Then
                '    cmdText &= " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "') "
                'Else
                '    cmdText &= " '" & pCalibratorsDS.tparCalibrators(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "') "
                'End If

                ''Add the scope identity because the ID is generated automatically by the database
                'cmdText &= " SELECT SCOPE_IDENTITY()"

                'Execute the SQL sentence 
                Dim dbCmd As New SqlClient.SqlCommand
                dbCmd.Connection = pDBConnection
                dbCmd.CommandText = cmdText

                Dim calibratorID As Integer = 0
                calibratorID = CType(dbCmd.ExecuteScalar(), Integer)
                If (calibratorID > 0) Then
                    'Set the calibrator ID to the Recived dataset
                    pCalibratorsDS.tparCalibrators(0).CalibratorID = calibratorID
                End If
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparCalibratorsDAO.Create", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Delete the specified Calibrator from Calibrators table
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pCalibratorID">Calibrator Identifier</param>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by : VR 31/05/2010 (Tested : Pending)
    ''' Modified by: SA 13/10/2010 - Do not return error when affected records is zero
    ''' </remarks>
    Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibratorID As Integer) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                'SQL Sentence to insert data
                Dim cmdText As String = " DELETE FROM tparCalibrators WHERE CalibratorID = " & pCalibratorID

                'Execute the SQL sentence 
                Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    resultData.HasError = False
                End Using
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparCalibratorsDAO.Delete", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Get basic data of the specified Calibrator
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pCalibratorID">Calibrator Identifier</param>
    ''' <param name="pIsFactory">is factory</param> 
    ''' <returns>GlobalDataTO containing a typed DataSet CalibratorDS with data of the informed Calibrator</returns>
    ''' <remarks>
    ''' Created by:  DL 21/01/2010
    ''' Modified by: SA 09/05/2012 - Changed the function template
    ''' Modified by: DL 30/01/2013 - Add optional paramenter IsFactory
    ''' </remarks>
    Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibratorID As Integer, Optional pIsFactory As Boolean = False) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then

                    Dim cmdText As String = ""
                    cmdText &= "SELECT CalibratorID" & vbCrLf
                    cmdText &= "      ,CalibratorName" & vbCrLf
                    cmdText &= "      ,LotNumber" & vbCrLf
                    cmdText &= "      ,ExpirationDate" & vbCrLf
                    cmdText &= "      ,NumberOfCalibrators" & vbCrLf
                    cmdText &= "      ,InUse" & vbCrLf
                    cmdText &= "      ,SpecialCalib" & vbCrLf
                    cmdText &= "      ,TS_User" & vbCrLf
                    cmdText &= "      ,TS_DateTime" & vbCrLf
                    cmdText &= "  FROM "

                    If pIsFactory Then cmdText &= GlobalBase.TemporalDBName & "."

                    cmdText &= "dbo.tparCalibrators" & vbCrLf
                    cmdText &= " WHERE CalibratorID = " & pCalibratorID.ToString

                    Dim calibratorsDataDS As New CalibratorsDS
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(calibratorsDataDS.tparCalibrators)
                        End Using
                    End Using

                    resultData.SetDatos = calibratorsDataDS
                    resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparCalibratorsDAO.Read", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Get all Experimental Calibrators from the Calibrators table
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <returns>GlobalDataTO containing a typed DataSet CalibratorDS with data of all Calibrators</returns>
    ''' <remarks>
    ''' Created by:  VR 31/05/2010 
    ''' Modified by: SA 09/05/2012 - Changed the function template
    ''' </remarks>
    Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = " SELECT * FROM tparCalibrators "

                    Dim calibratorsDataDS As New CalibratorsDS
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(calibratorsDataDS.tparCalibrators)
                        End Using
                    End Using

                    resultData.SetDatos = calibratorsDataDS
                    resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparCalibratorsDAO.ReadAll", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Get basic data of the specified Calibrator (searching by Calibrator Name)
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pCalibratorName">Calibrator Name</param>
    ''' <returns>GlobalDataTO containing a typed DataSet CalibratorDS with data of the informed Calibrator</returns>
    ''' <remarks>
    ''' Created by:  TR 24/03/2011
    ''' Modified by: SA 09/05/2012 - Changed the function template; added prefix N to the filter by CalibratorName
    ''' AG  13/03/2014 - #1538 fix issue when name contains char ' (use .Replace("'", "''"))
    ''' </remarks>
    Public Function ReadByCalibratorName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibratorName As String, Optional pIsFactory As Boolean = False) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then

                    Dim cmdText As String = ""
                    cmdText &= "SELECT CalibratorID" & vbCrLf
                    cmdText &= "      ,CalibratorName" & vbCrLf
                    cmdText &= "      ,LotNumber" & vbCrLf
                    cmdText &= "      ,ExpirationDate" & vbCrLf
                    cmdText &= "      ,NumberOfCalibrators" & vbCrLf
                    cmdText &= "      ,InUse" & vbCrLf
                    cmdText &= "      ,SpecialCalib" & vbCrLf
                    cmdText &= "      ,TS_User" & vbCrLf
                    cmdText &= "      ,TS_DateTime" & vbCrLf
                    cmdText &= "  FROM "

                    If pIsFactory Then cmdText &= GlobalBase.TemporalDBName & "."

                    cmdText &= "dbo.tparCalibrators" & vbCrLf

                    'cmdText &= " WHERE  CalibratorName = N'" & pCalibratorName.Trim & "' " & vbCrLf
                    cmdText &= " WHERE  CalibratorName = N'" & pCalibratorName.Trim.Replace("'", "''") & "' " & vbCrLf

                    Dim calibratorsDataDS As New CalibratorsDS
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(calibratorsDataDS.tparCalibrators)
                        End Using
                    End Using

                    resultData.SetDatos = calibratorsDataDS
                    resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparCalibratorsDAO.ReadByCalibratorName", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Update values of the specified Calibrator
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pCalibratorsDS">Typed DataSet CalibratorsDS containing data to update</param>
    ''' <returns>GlobalDataTO containing sucess/error information</returns>
    ''' <remarks>
    ''' Created by:  VR 31/05/2010 - Tested: Pending
    ''' Modified by: TR 07/06/2010
    '''              SA 13/10/2010 - Added Replace in String fields
    '''              SA 27/10/2010 - Added N preffix for multilanguage of field TS_User. Other changes: CalibratorName, 
    '''                              LotNumber, NumberOfCalibrators and TS_User are mandatory fields, they do not allow Null 
    '''                              values; field InUse is not update (there is an specific function for it); if TS_User is 
    '''                              not informed in the DS, get the ID of the current logged user
    ''' </remarks>
    Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibratorsDS As CalibratorsDS) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim cmdText As String = ""
                cmdText = " UPDATE tparCalibrators " & _
                          " SET CalibratorName = N'" & pCalibratorsDS.tparCalibrators(0).CalibratorName.Replace("'", "''") & "', " & _
                              " LotNumber      = N'" & pCalibratorsDS.tparCalibrators(0).LotNumber.Replace("'", "''") & "', " & _
                              " NumberOfCalibrators = " & pCalibratorsDS.tparCalibrators(0).NumberOfCalibrators & ", "

                If (pCalibratorsDS.tparCalibrators(0).IsExpirationDateNull) Then
                    cmdText &= " ExpirationDate = NULL, "
                Else
                    cmdText &= " ExpirationDate = '" & pCalibratorsDS.tparCalibrators(0).ExpirationDate.ToString("yyyyMMdd") & "', "
                End If

                If (pCalibratorsDS.tparCalibrators(0).IsTS_UserNull) Then
                    'Dim myGlobalbase As New GlobalBase
                    cmdText &= " TS_User = N'" & GlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', "
                Else
                    cmdText &= " TS_User = N'" & pCalibratorsDS.tparCalibrators(0).TS_User.Trim.Replace("'", "''") & "', "
                End If

                If (pCalibratorsDS.tparCalibrators(0).IsTS_DateTimeNull) Then
                    cmdText &= " TS_DateTime = '" & DateTime.Now.ToString("yyyyMMdd HH:mm:ss") & "' "
                Else
                    cmdText &= " TS_DateTime  = '" & pCalibratorsDS.tparCalibrators(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "' "
                End If

                cmdText &= " WHERE CalibratorID = " & pCalibratorsDS.tparCalibrators(0).CalibratorID

                'Execute the SQL sentence 
                Dim dbCmd As New SqlClient.SqlCommand
                dbCmd.Connection = pDBConnection
                dbCmd.CommandText = cmdText

                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparCalibratorsDAO.Update", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function
#End Region

#Region "Other methods"
    ''' <summary>
    ''' Returns selected Calibrators info to shown in a Report
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="AppLang">Application Language</param>
    ''' <param name="SelectedCalibrators">List of selected Calibrators IDs</param>
    ''' <remarks>
    ''' Created by: RH 16/12/2011
    ''' </remarks>
    Public Function GetCalibratorsForReport(ByVal pDBConnection As SqlClient.SqlConnection, ByVal AppLang As String, _
                                            Optional ByVal SelectedCalibrators As List(Of Integer) = Nothing) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            resultData = GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim StrSelectedCalibrators As String = String.Empty

                    If (Not SelectedCalibrators Is Nothing AndAlso SelectedCalibrators.Count > 0) Then
                        Dim Calibrators As New StringBuilder()
                        For Each c As Integer In SelectedCalibrators
                            Calibrators.AppendFormat("{0},", c)
                        Next

                        StrSelectedCalibrators = String.Format(" WHERE TC.CalibratorID IN ({0})", _
                                                            Calibrators.ToString().Substring(0, Calibrators.Length - 1))
                    End If

                    Dim cmdText As String = String.Format(" SELECT CalibratorID, CalibratorName, LotNumber, NumberOfCalibrators, ExpirationDate" & _
                                                          " FROM   tparCalibrators TC{0}", StrSelectedCalibrators)

                    'Get the first table
                    Dim myCalibratorsDS As New CalibratorsDS
                    Using dbCmd As New SqlCommand(cmdText, dbConnection)
                        'Fill the DataSet to return 
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(myCalibratorsDS.tparCalibrators)
                        End Using
                    End Using

                    cmdText = String.Format( _
                            " SELECT TC.CalibratorID, T.TestName, T.MeasureUnit, T.DecimalsAllowed," & _
                            "   (TC.SampleType + '-' + MR.ResourceText) AS SampleType," & _
                            "   (CASE WHEN TC.CurveGrowthType IS NULL THEN NULL ELSE MR1.ResourceText END) AS CurveGrowthType," & _
                            "   (CASE WHEN TC.CurveType IS NULL THEN NULL ELSE MR2.ResourceText END) AS CurveType," & _
                            "   (CASE WHEN TC.CurveAxisXType IS NULL THEN NULL ELSE MR3.ResourceText END) AS CurveAxisXType," & _
                            "   (CASE WHEN TC.CurveAxisYType IS NULL THEN NULL ELSE MR4.ResourceText END) AS CurveAxisYType," & _
                            "   TCV.CalibratorNum, TCV.TheoricalConcentration" & _
                            " FROM tparTestCalibrators TC " & _
                            "   INNER JOIN tparTests T ON TC.TestID = T.TestID" & _
                            "   INNER JOIN tcfgMasterData MD ON TC.SampleType = MD.ItemID AND MD.SubTableID = 'SAMPLE_TYPES'" & _
                            "   INNER JOIN tfmwMultiLanguageResources MR ON MD.ResourceID = MR.ResourceID AND MR.LanguageID = '{0}'" & _
                            "   INNER JOIN tparTestCalibratorValues TCV ON TC.TestCalibratorID = TCV.TestCalibratorID" & _
                            "   INNER JOIN tparCalibrators C ON TC.CalibratorID = C.CalibratorID" & _
                            "   LEFT OUTER JOIN tfmwPreloadedMasterData PMD1" & _
                            "     ON TC.CurveGrowthType = PMD1.ItemID AND PMD1.SubTableID = 'CURVE_GROWTH_TYPES'" & _
                            "   LEFT OUTER JOIN tfmwMultiLanguageResources MR1" & _
                            "     ON PMD1.ResourceID = MR1.ResourceID AND MR1.LanguageID = '{0}'" & _
                            "   LEFT OUTER JOIN tfmwPreloadedMasterData PMD2" & _
                            "     ON TC.CurveType = PMD2.ItemID AND PMD2.SubTableID = 'CURVE_TYPES'" & _
                            "   LEFT OUTER JOIN tfmwMultiLanguageResources MR2" & _
                            "     ON PMD2.ResourceID = MR2.ResourceID AND MR2.LanguageID = '{0}'" & _
                            "   LEFT OUTER JOIN tfmwPreloadedMasterData PMD3" & _
                            "     ON TC.CurveAxisXType = PMD3.ItemID AND PMD3.SubTableID = 'CURVE_AXIS_TYPES'" & _
                            "   LEFT OUTER JOIN tfmwMultiLanguageResources MR3" & _
                            "     ON PMD3.ResourceID = MR3.ResourceID AND MR3.LanguageID = '{0}'" & _
                            "   LEFT OUTER JOIN tfmwPreloadedMasterData PMD4" & _
                            "     ON TC.CurveAxisYType = PMD4.ItemID AND PMD4.SubTableID = 'CURVE_AXIS_TYPES'" & _
                            "   LEFT OUTER JOIN tfmwMultiLanguageResources MR4" & _
                            "     ON PMD4.ResourceID = MR4.ResourceID AND MR4.LanguageID = '{0}'{1}" & _
                            " ORDER BY T.TestPosition, TCV.CalibratorNum DESC", AppLang, StrSelectedCalibrators)

                    'Get the second table
                    Using dbCmd As New SqlCommand(cmdText, dbConnection)
                        'Fill the DataSet to return 
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(myCalibratorsDS.tparCalibratorsTests)
                            resultData.SetDatos = myCalibratorsDS
                        End Using
                    End Using
                End If
            End If
        Catch ex As Exception
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparCalibratorsDAO.GetCalibratorsForReport", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Set value of flag InUse for all Calibrators added/removed from the Active WorkSession 
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pWorkSessionID">Work Session Identifier</param>
    ''' <param name="pAnalyzerID">Analyzer Identifier</param>
    ''' <param name="pFlag">Value of the InUse Flag to set</param>
    ''' <param name="pUpdateForExcluded">Optional parameter. When True, it means the InUse will be set to False
    '''                                  only for Calibrators that have been excluded from the active WorkSession</param>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by:  GDS 10/05/2010 
    ''' Modified by: SA  09/06/2010 - Change the Query. To set InUse=TRUE, the current query works only for positioned Calibrators, 
    '''                               and it should set both, positioned and not positioned Calibrators. Added new optional parameter
    '''                               to reuse this method to set InUse=False for Calibrators that have been excluded from
    '''                               the active WorkSession. Added the parameter for the AnalyzerID    
    ''' </remarks>
    Public Function UpdateInUseFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                    ByVal pFlag As Boolean, Optional ByVal pUpdateForExcluded As Boolean = False) As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim cmdText As String

                If (Not pUpdateForExcluded) Then
                    cmdText = " UPDATE tparCalibrators " & _
                              " SET    InUse = " & Convert.ToInt32(IIf(pFlag, 1, 0)) & _
                              " WHERE  CalibratorID IN (SELECT DISTINCT TC.CalibratorID " & _
                                                      " FROM   tparTestCalibrators TC INNER JOIN twksOrderTests OT ON TC.TestID = OT.TestID AND TC.SampleType = OT.SampleType " & _
                                                                                    " INNER JOIN twksWSOrderTests WSOT ON OT.OrderTestID = WSOT.OrderTestID " & _
                                                                                    " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & _
                                                      " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & _
                                                      " AND    OT.AnalyzerID = '" & pAnalyzerID.Trim & "' " & _
                                                      " AND    O.SampleClass = 'CALIB') "
                Else
                    cmdText = " UPDATE tparCalibrators " & _
                              " SET    InUse = " & Convert.ToInt32(IIf(pFlag, 1, 0)) & _
                              " WHERE  CalibratorID NOT IN (SELECT DISTINCT TC.CalibratorID " & _
                                                          " FROM   tparTestCalibrators TC INNER JOIN twksOrderTests OT ON TC.TestID = OT.TestID AND TC.SampleType = OT.SampleType " & _
                                                                                        " INNER JOIN twksWSOrderTests WSOT ON OT.OrderTestID = WSOT.OrderTestID " & _
                                                                                        " INNER JOIN twksOrders O ON OT.OrderID = O.OrderID " & _
                                                          " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & _
                                                          " AND    OT.AnalyzerID = '" & pAnalyzerID.Trim & "' " & _
                                                          " AND    O.SampleClass = 'CALIB') " & _
                              " AND    InUse = 1 "
                End If

                Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                    myGlobalDataTO.HasError = False
                End Using
            End If
        Catch ex As Exception
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobalDataTO.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparCalibratorsDAO.UpdateInUseFlag", EventLogEntryType.Error, False)
        End Try
        Return myGlobalDataTO
    End Function

#End Region
End Class
