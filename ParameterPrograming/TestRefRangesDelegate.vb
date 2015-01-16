Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL

    Public Class TestRefRangesDelegate

#Region "CRUD"

        ''' <summary>
        ''' Create new Test Reference Ranges
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestRefRanges">Typed DataSet TestRefRangesDS  with data of the Reference Ranges to add</param>
        ''' <param name="pTestType">Standard, Calculated or ISE</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestRefRangesDS with data of the added Reference Range</returns>
        ''' <remarks>
        ''' Created by:  SG 16/06/2010 
        ''' Modified by: SG 01/09/2010 - Added parameter pTestType 
        '''              AG 26/10/2010 - Call the DAO function for each row in the DS
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestRefRanges As TestRefRangesDS, _
                               Optional ByVal pTestType As String = "STD") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (pTestRefRanges.tparTestRefRanges.Rows.Count > 0) Then
                            Dim myTestRefRangesDAO As New tparTestRefRangesDAO()
                            For Each myTestRefRangesRow As TestRefRangesDS.tparTestRefRangesRow In pTestRefRanges.tparTestRefRanges.Rows
                                myGlobalDataTO = myTestRefRangesDAO.Create(dbConnection, myTestRefRangesRow, pTestType)
                                If myGlobalDataTO.HasError Then Exit For
                            Next

                            If (Not myGlobalDataTO.HasError) Then
                                'When the Database Connection was opened locally, then the Commit is executed
                                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                                myGlobalDataTO.SetDatos = pTestRefRanges
                            Else
                                'When the Database Connection was opened locally, then the Rollback is executed
                                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TestRefRangesDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete the specified Reference Ranges
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestRefRanges">Typed DataSet TestRefRangesDS with the Test Reference Ranges to delete</param>
        ''' <param name="pTestType">Standard, Calculated or ISE</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 16/06/2010 
        ''' Modified by: SG 01/09/2010 - Added parameter pTestType 
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestRefRanges As TestRefRangesDS, _
                               Optional ByVal pTestType As String = "STD") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestRefRangesDAO As New tparTestRefRangesDAO
                        resultData = myTestRefRangesDAO.Delete(dbConnection, pTestRefRanges, pTestType)

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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TestRefRangesDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete Reference Ranges defined for a Test and optionally, a SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pTestType">Standard, Calculated or ISE</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 16/06/2010
        ''' Modified by: SG 01/09/2010 - Added parameter pTestType 
        ''' </remarks>
        Public Function DeleteByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, Optional ByVal pSampleType As String = "", _
                                       Optional ByVal pTestType As String = "STD") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestRefRangesDAO As New tparTestRefRangesDAO
                        myGlobalDataTO = myTestRefRangesDAO.DeleteByTestID(dbConnection, pTestID, pSampleType, pTestType)

                        If (Not myGlobalDataTO.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TestRefRangesDelegate.DeleteByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get Reference Ranges defined by the specified Test, and optionally by an specific SampleType and/or RangeType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pTestType">Standard, Calculated or ISE</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pRangeType">Type of Range: Generic or Detailed</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestRefRangesDS with the Reference Ranges defined for the Test</returns>
        ''' <remarks>
        ''' Created by:   SG 16/06/2010 
        ''' Modified by:  SG 01/09/2010 - Added parameter pTestType
        '''               SA 17/11/2010 - Get the multilanguage description of fields Gender and AgeUnit for detailed ReferenceRanges
        ''' </remarks>
        Public Function ReadByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, Optional ByVal pSampleType As String = "", _
                                     Optional ByVal pRangeType As String = "", Optional ByVal pTestType As String = "STD") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestRefRangesDAO As New tparTestRefRangesDAO
                        resultData = myTestRefRangesDAO.ReadByTestID(dbConnection, pTestID, pSampleType, pRangeType, pTestType)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myTestRefRanges As TestRefRangesDS = DirectCast(resultData.SetDatos, TestRefRangesDS)

                            'Get the current application language
                            'Dim myGlobalbase As New GlobalBase
                            Dim currentLang As String = GlobalBase.GetSessionInfo.ApplicationLanguage

                            'Get gender descriptions in the current application language
                            Dim lstDetailedByGender As List(Of TestRefRangesDS.tparTestRefRangesRow)
                            lstDetailedByGender = (From a As TestRefRangesDS.tparTestRefRangesRow In myTestRefRanges.tparTestRefRanges _
                                                  Where a.RangeType = "DETAILED" _
                                            AndAlso Not a.IsGenderNull _
                                               Order By a.Gender _
                                                 Select a).ToList

                            Dim genderDesc As String = ""
                            Dim currentGender As String = ""
                            Dim myPreloadedDS As New PreloadedMasterDataDS
                            Dim myPreloadedMDDelegate As New PreloadedMasterDataDelegate
                            For Each diffGenderRange As TestRefRangesDS.tparTestRefRangesRow In lstDetailedByGender
                                If (currentGender = "") OrElse (currentGender <> "" And currentGender <> diffGenderRange.Gender) Then
                                    currentGender = diffGenderRange.Gender

                                    'Get the Gender description
                                    resultData = myPreloadedMDDelegate.GetSubTableItem(dbConnection, GlobalEnumerates.PreloadedMasterDataEnum.SEX_LIST, _
                                                                                       currentGender)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myPreloadedDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                        If (myPreloadedDS.tfmwPreloadedMasterData.Rows.Count = 1) Then
                                            genderDesc = myPreloadedDS.tfmwPreloadedMasterData(0).FixedItemDesc

                                            diffGenderRange.GenderDesc = genderDesc
                                        End If
                                    Else
                                        Exit For
                                    End If
                                Else
                                    diffGenderRange.GenderDesc = genderDesc
                                End If
                            Next

                            If (Not resultData.HasError) Then
                                'Get age unit descriptions in the current application language
                                Dim lstDetailedByAge As List(Of TestRefRangesDS.tparTestRefRangesRow)
                                lstDetailedByAge = (From a As TestRefRangesDS.tparTestRefRangesRow In myTestRefRanges.tparTestRefRanges _
                                                   Where a.RangeType = "DETAILED" _
                                             AndAlso Not a.IsAgeUnitNull _
                                                Order By a.AgeUnit _
                                                  Select a).ToList

                                Dim ageUnitDesc As String = ""
                                Dim currentAgeUnit As String = ""
                                For Each diffAgeRange As TestRefRangesDS.tparTestRefRangesRow In lstDetailedByAge
                                    If (currentAgeUnit = "") OrElse (currentGender <> "" And currentAgeUnit <> diffAgeRange.AgeUnit) Then
                                        currentAgeUnit = diffAgeRange.AgeUnit

                                        'Get the Age Unit description
                                        resultData = myPreloadedMDDelegate.GetSubTableItem(dbConnection, GlobalEnumerates.PreloadedMasterDataEnum.AGE_UNITS, _
                                                                                           currentAgeUnit)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            myPreloadedDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                            If (myPreloadedDS.tfmwPreloadedMasterData.Rows.Count = 1) Then
                                                ageUnitDesc = myPreloadedDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                                diffAgeRange.AgeUnitDesc = ageUnitDesc
                                            End If
                                        Else
                                            Exit For
                                        End If
                                    Else
                                        diffAgeRange.AgeUnitDesc = ageUnitDesc
                                    End If
                                Next
                            End If

                            'If no error in the process, the final DS with the Reference Ranges is returned
                            If (Not resultData.HasError) Then
                                resultData.SetDatos = myTestRefRanges
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
                GlobalBase.CreateLogActivity(ex.Message, "TestRefRangesDelegate.ReadByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Modify a group of Test Reference Ranges
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestRefRanges">Typed DataSet TestRefRanges with data of the Reference Ranges to update</param>
        ''' <param name="pTestType">Standard, Calculated or ISE</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestRefRangesDS with the updated Reference Ranges</returns>
        ''' <remarks>
        ''' Created by:  SG 16/06/2010 
        ''' Modified by: SG 01/09/2010 - Added parameter pTestType 
        '''              AG 26/10/2010 - Call the DAO function for each row in the DS
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestRefRanges As TestRefRangesDS, _
                               Optional ByVal pTestType As String = "STD") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If (pTestRefRanges.tparTestRefRanges.Rows.Count > 0) Then
                            Dim myTestRefRangesDAO As New tparTestRefRangesDAO
                            For Each TestRefRangesRow As TestRefRangesDS.tparTestRefRangesRow In pTestRefRanges.tparTestRefRanges.Rows
                                resultData = myTestRefRangesDAO.Update(dbConnection, TestRefRangesRow, pTestType)
                                If (resultData.HasError) Then Exit For
                            Next
                            
                            If (Not resultData.HasError) Then
                                'When the Database Connection was opened locally, then the Commit is executed
                                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                                resultData.SetDatos = pTestRefRanges
                            Else
                                'When the Database Connection was opened locally, then the Rollback is executed
                                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TestRefRangesDelegate.Update", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "TO DELETE"
        '''' <summary>
        '''' Get detailed Reference Range defined for the specified Gender for a Test/SampleType
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code</param>
        '''' <param name="pGender">Gender Code to filter the detailed Reference Ranges</param>
        '''' <param name="pTestType">Standard, Calculated or ISE</param>
        '''' <returns>GlobalDataTO containing a typed DataSet TestRefRangesDS with data of the Test Reference Range</returns>
        '''' <remarks>
        '''' Created by:  DL 29/06/2010 
        '''' Modified by: SG 01/09/2010 - Added parameter pTestType
        '''' </remarks>
        'Public Function GetDetailedByGender(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String, _
        '                                    ByVal pGender As String, Optional ByVal pTestType As String = "STD") As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myTestRefRangesDAO As New tparTestRefRangesDAO
        '                resultData = myTestRefRangesDAO.GetDetailedByGender(dbConnection, pTestID, pSampleType, pGender, pTestType)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "TestRefRangesDelegate.GetDetailedByGender", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get detailed Reference Range defined for the specified Age for a Test/SampleType
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code</param>
        '''' <param name="pAge">Age to filter the detailed Reference Ranges</param>
        '''' <param name="pTestType">Standard, Calculated or ISE</param>
        '''' <returns>GlobalDataTO containing a typed DataSet TestRefRangesDS with data of the Test Reference Range</returns>
        '''' <remarks>
        '''' Created by:  DL 29/06/2010 
        '''' Modified by: SG 01/09/2010 - Added parameter pTestType
        '''' </remarks>
        'Public Function GetDetailedByAge(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String, _
        '                                 ByVal pAge As Single, Optional ByVal pTestType As String = "STD") As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myTestRefRangesDAO As New tparTestRefRangesDAO
        '                resultData = myTestRefRangesDAO.GetDetailedByAge(dbConnection, pTestID, pSampleType, pAge, pTestType)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "TestRefRangesDelegate.GetDetailedByAge", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get detailed Reference Range defined for the specified Gender and Age for a Test/SampleType
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code</param>
        '''' <param name="pGender">Gender Code to filter the detailed Reference Ranges</param>
        '''' <param name="pAge">Age to filter the detailed Reference Ranges</param>
        '''' <param name="pTestType">Standard, Calculated or ISE</param>
        '''' <returns>GlobalDataTO containing a typed DataSet TestRefRangesDS with data of the Test Reference Range</returns>
        '''' <remarks>
        '''' Created by:  DL 29/06/2010 
        '''' Modified by: SG 01/09/2010 - Added parameter pTestType
        '''' </remarks>
        'Public Function GetDetailedByGenderAge(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String, _
        '                                       ByVal pGender As String, ByVal pAge As Single, Optional ByVal pTestType As String = "STD") As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myTestRefRangesDAO As New tparTestRefRangesDAO
        '                resultData = myTestRefRangesDAO.GetDetailedByGenderAge(dbConnection, pTestID, pSampleType, pGender, pAge, pTestType)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "TestRefRangesDelegate.GetDetailedByAge", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function
#End Region
    End Class

End Namespace

