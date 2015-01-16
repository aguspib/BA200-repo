Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL

Namespace Biosystems.Ax00.BL

    Public Class FormulasDelegate

#Region "Public Methods"

        ''' <summary>
        ''' Get all values included in the Formula defined for a Calculated Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTestID">Identifier of the Calculated Test</param>
        ''' <returns>GlobalDataTO containing a typed DataSet FormulasDS with all components of the Calculated Test Formula</returns>
        ''' <remarks></remarks>
        Public Function GetFormulaValues(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim resultDAO As New tparFormulasDAO
                        resultData = resultDAO.GetFormulaValues(dbConnection, pCalcTestID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FormulasDelegate.GetFormulaValues", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Tests included in the Formula defined for a specified Calculated Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTestID">Identifier of the Calculated Test</param>
        ''' <param name="pVerifyCalcInFormula">Optional parameter used to indicate if when in the Formula of the
        '''                                    specified Calculated Test there are Calculated Tests, the Tests included
        '''                                    in the Formula of each one of them have to be also added to the final DataSet</param>
        ''' <returns>GlobalDataTO containing a typed DataSet FormulasDS with the list of Tests (standard 
        '''          and calculated) included in the Formula defined for the specified Calculated Test</returns>
        ''' <remarks>
        ''' Created by:  GDS 03/05/2010
        ''' Modified by: SA  04/05/2010 - Use LINQ to verify if there are calculated tests in the Formula
        '''              SA  26/05/2010 - Added optional parameter pVerifyCalcInFormula 
        ''' </remarks>
        Public Function GetTestsInFormula(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestID As Integer, _
                                          Optional ByVal pVerifyCalcInFormula As Boolean = True) As GlobalDataTO
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim resultDAO As New tparFormulasDAO
                        resultData = resultDAO.ReadTestsInFormula(dbConnection, pCalcTestID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myFormulas As FormulasDS = DirectCast(resultData.SetDatos, FormulasDS)

                            If (pVerifyCalcInFormula) Then
                                'Verify if there are Calculated Tests in the Formula
                                Dim lstTestList As List(Of FormulasDS.tparFormulasRow)
                                lstTestList = (From a In myFormulas.tparFormulas _
                                              Where a.TestType = "CALC" _
                                             Select a).ToList

                                Dim myFormulaAux As New FormulasDS
                                For Each rowFormulas As FormulasDS.tparFormulasRow In lstTestList
                                    'Get Standard Tests included in the Calculated Test
                                    resultData = resultDAO.ReadTestsInFormula(dbConnection, CType(rowFormulas.Value, Integer))
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myFormulaAux = DirectCast(resultData.SetDatos, FormulasDS)

                                        For Each testRow As FormulasDS.tparFormulasRow In myFormulaAux.tparFormulas.Rows
                                            myFormulas.tparFormulas.ImportRow(testRow)
                                        Next
                                    Else
                                        'Error getting the list of tests in the Calculated Test Formula
                                        Exit For
                                    End If
                                Next
                                lstTestList = Nothing
                            End If

                            resultData.SetDatos = myFormulas
                            resultData.HasError = False
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FormulasDelegate.GetTestsInFormula", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if the specified Test (Standard, ISE, Off-System or Calculated Test) is included in the formula of a Calculated Test,
        ''' filtering data by Sample Type if this value has been informed.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter.</param>
        ''' <param name="pTestType">Type of Test (STD,ISE,OFFS,CALC). Optional parameter.</param>
        ''' <param name="pExcludeSampleTypes">When True, it indicates the Test will be searched in Formulas but using a
        '''                                   SampleType different of the specified ones. Optional parameter.</param>
        ''' <returns>GlobalDataTO containing a typed DataSet FormulasDS with the Identifier and Name of the Calculated
        '''          Test(s) in which formula the specified Test is included.</returns>
        ''' <remarks>
        ''' Created by:  TR 17/05/2010
        ''' Modified by: SA 14/01/2010 - Added new optional parameter to allow search a Test in a Formula but using a SampleType 
        '''                              different of the specified ones
        '''              WE 21/11/2014 - RQ00035C (BA-1867): change Summary and Parameter description.
        ''' </remarks>
        Public Function ReadFormulaByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, Optional ByVal pSampleType As String = "", _
                                            Optional ByVal pTestType As String = "STD", Optional ByVal pExcludeSampleTypes As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim resultDAO As New tparFormulasDAO
                        resultData = resultDAO.ReadFormulaByTestID(dbConnection, pTestID, pSampleType, pTestType, pExcludeSampleTypes)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FormulasDelegate.ReadFormulaByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all values included in the Formula defined for the specified Calculated Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTestID">Identifier of the Calculated Test</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 22/06/2010 
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim resultDAO As New tparFormulasDAO
                        resultData = resultDAO.Delete(dbConnection, pCalcTestID)

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
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FormulasDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Add values included in the Formula defined for the specified Calculated Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pFormulaValues">Typed DataSet FormulasDS containing all values in the Formula</param>
        ''' <param name="pNewCalcTest">When True, it indicates the Formula is added for a new Calculated Test</param>
        ''' <returns>GlobalDataTO containing a typed DataSet FormulasDS with all the added records and/or error information</returns>
        ''' <remarks>
        ''' Created by:  SA 22/06/2010 
        ''' </remarks>
        Public Function AddFormula(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pFormulaValues As FormulasDS, _
                                   ByVal pNewCalcTest As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim resultDAO As New tparFormulasDAO

                        'When an existing Calculated Test was modified, its current Formula members are deleted
                        'The ID is obtained from the first row in the entry DS
                        If (Not pNewCalcTest) Then
                            resultData = resultDAO.Delete(dbConnection, pFormulaValues.tparFormulas(0).CalcTestID)
                        End If

                        'Formula members are created
                        If (Not resultData.HasError) Then
                            resultData = resultDAO.Create(dbConnection, pFormulaValues)
                        End If

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
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FormulasDelegate.AddFormula", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' auxiliar method for updating Value field for formulas that includes Calculated tests after Database version update
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pOldValue"></param>
        ''' <param name="pNewValue"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 21/02/2013</remarks>
        Public Function UpdateCalcTestValueAfterDBUpdate(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOldValue As Integer, ByVal pNewValue As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim resultDAO As New tparFormulasDAO

                        resultData = resultDAO.UpdateCalcTestValueAfterDBUpdate(dbConnection, pOldValue, pNewValue)

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
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FormulasDelegate.UpdateCalcTestValueAfterDBUpdate", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        '''  Get all Calculated Tests included in the Formula defined for the specified Calculated Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTestID">Identifier of the Calculated Test</param>
        ''' <returns>GlobalDataTO containing a typed DataSet FormulasDS with all components of the Calculated Test Formula</returns>
        ''' <remarks>
        ''' Created by: XB 21/02/2013
        ''' </remarks>
        Public Function GetCalculatedTestIntoFormula(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestID As Integer, Optional pDataBaseName As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim resultDAO As New tparFormulasDAO
                        resultData = resultDAO.GetCalculatedTestIntoFormula(dbConnection, pCalcTestID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FormulasDelegate.GetCalculatedTestIntoFormula", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region
    End Class
End Namespace