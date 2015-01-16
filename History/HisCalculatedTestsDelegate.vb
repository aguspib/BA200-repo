Option Explicit On
Option Strict On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.BL
    Public Class HisCalculatedTestsDelegate

#Region "Public Methods"

        ''' <summary>
        ''' Receive a list of Calculated Test Identifiers and for each one of them, verify if it already exists in Historics Module and when it does not exist, 
        ''' get the needed data and create the new Calculated Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisCalculatedTestsDS">Typed DataSet HisCalculatedTestsDS containing the list of Calculated Tests to verify if they already exist 
        '''                                     in Historics Module and create them when not</param>
        ''' <returns>GlobalDataTO containing a typed Dataset HisCalculatedTestsDS with the identifier in Historics Module for each Calculated Test in it</returns>
        ''' <remarks>
        ''' Created by:  SA 24/02/2012
        ''' </remarks>
        Public Function CheckCalculatedTestsInHistorics(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisCalculatedTestsDS As HisCalculatedTestsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myCalcTestDS As New CalculatedTestsDS
                        Dim myCalcTestDelegate As New CalculatedTestsDelegate

                        Dim myDAO As New thisCalculatedTestsDAO
                        Dim auxiliaryDS As New HisCalculatedTestsDS
                        Dim calcTestsToAddDS As New HisCalculatedTestsDS
                        
                        For Each calcTestRow As HisCalculatedTestsDS.thisCalculatedTestsRow In pHisCalculatedTestsDS.thisCalculatedTests
                            resultData = myDAO.ReadByCalcTestID(dbConnection, calcTestRow.CalcTestID)

                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                auxiliaryDS = DirectCast(resultData.SetDatos, HisCalculatedTestsDS)

                                If (auxiliaryDS.thisCalculatedTests.Rows.Count = 0) Then
                                    'New Calculated Test; get basic data from tables tparCalculatedTests
                                    resultData = myCalcTestDelegate.GetCalcTest(dbConnection, calcTestRow.CalcTestID)

                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myCalcTestDS = DirectCast(resultData.SetDatos, CalculatedTestsDS)

                                        If (myCalcTestDS.tparCalculatedTests.Rows.Count > 0) Then
                                            calcTestRow.BeginEdit()
                                            calcTestRow.CalcTestLongName = myCalcTestDS.tparCalculatedTests.First.CalcTestLongName
                                            calcTestRow.MeasureUnit = myCalcTestDS.tparCalculatedTests.First.MeasureUnit
                                            calcTestRow.DecimalsAllowed = myCalcTestDS.tparCalculatedTests.First.Decimals
                                            calcTestRow.FormulaText = myCalcTestDS.tparCalculatedTests.First.FormulaText
                                            calcTestRow.EndEdit()
                                        End If
                                    Else
                                        'Error getting data of the Calculated Test
                                        Exit For
                                    End If

                                    'Copy the Calculated Test data to the auxiliary DS of elements to add
                                    calcTestsToAddDS.thisCalculatedTests.ImportRow(calcTestRow)
                                Else
                                    'The Calculated Test  already exists in Historics Module; inform all fields in the DS
                                    calcTestRow.BeginEdit()
                                    calcTestRow.HistCalcTestID = auxiliaryDS.thisCalculatedTests.First.HistCalcTestID
                                    calcTestRow.CalcTestLongName = auxiliaryDS.thisCalculatedTests.First.CalcTestLongName
                                    calcTestRow.MeasureUnit = auxiliaryDS.thisCalculatedTests.First.MeasureUnit
                                    calcTestRow.DecimalsAllowed = auxiliaryDS.thisCalculatedTests.First.DecimalsAllowed
                                    calcTestRow.FormulaText = auxiliaryDS.thisCalculatedTests.First.FormulaText
                                    calcTestRow.EndEdit()
                                End If
                            Else
                                'Error verifying if the Calculate Test exists in Historics Module
                                Exit For
                            End If
                        Next

                        'Add to Historics Module all new Calculated Tests
                        If (Not resultData.HasError AndAlso calcTestsToAddDS.thisCalculatedTests.Rows.Count > 0) Then
                            resultData = myDAO.Create(dbConnection, calcTestsToAddDS)

                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                calcTestsToAddDS = DirectCast(resultData.SetDatos, HisCalculatedTestsDS)

                                'Search the added Calculated Tests in the entry DS and inform the CalcTestID in Historics Module
                                Dim lstCalcTestToUpdate As List(Of HisCalculatedTestsDS.thisCalculatedTestsRow)
                                For Each calcTestRow As HisCalculatedTestsDS.thisCalculatedTestsRow In calcTestsToAddDS.thisCalculatedTests
                                    lstCalcTestToUpdate = (From a As HisCalculatedTestsDS.thisCalculatedTestsRow In pHisCalculatedTestsDS.thisCalculatedTests _
                                                          Where a.CalcTestID = calcTestRow.CalcTestID _
                                                        AndAlso a.IsHistCalcTestIDNull).ToList

                                    If (lstCalcTestToUpdate.Count = 1) Then
                                        lstCalcTestToUpdate.First.BeginEdit()
                                        lstCalcTestToUpdate.First.HistCalcTestID = calcTestRow.HistCalcTestID
                                        lstCalcTestToUpdate.First.EndEdit()
                                    End If
                                Next
                                lstCalcTestToUpdate = Nothing
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            resultData.SetDatos = pHisCalculatedTestsDS
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If

                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisCalculatedTestsDelegate.CheckCalculatedTestsInHistorics", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all not in use closed Calculated Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 01/07/2013
        ''' </remarks>
        Public Function DeleteClosedNotInUse(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisCalculatedTestsDAO
                        resultData = myDAO.DeleteClosedNotInUse(dbConnection)

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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisCalculatedTestsDelegate.DeleteClosedNotInUse", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace

