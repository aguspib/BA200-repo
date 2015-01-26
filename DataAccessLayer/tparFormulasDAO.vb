Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tparFormulasDAO
          

#Region "CRUD Methods"

        ''' <summary>
        ''' Add values included in the Formula defined for the specified Calculated Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pFormulaValues">Typed DataSet FormulasDS containing all values in the Formula</param>
        ''' <returns>GlobalDataTO containing a typed DataSet FormulasDS with all the added records and/or error information</returns>
        ''' <remarks>
        ''' Modified by: SA 21/06/2010 - Query changed to control fields that allow Null values
        '''              SA 12/03/2012 - Changed the function template
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pFormulaValues As FormulasDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim i As Integer = 0
                    Dim recordOK As Boolean = True

                    Do While (recordOK And i < pFormulaValues.tparFormulas.Rows.Count)
                        Dim cmdText As String
                        cmdText = " INSERT INTO tparFormulas (CalcTestID, Position, ValueType, [Value], TestType, SampleType) " & vbCrLf & _
                                  " VALUES (" & pFormulaValues.tparFormulas(i).CalcTestID & ", " & vbCrLf & _
                                                pFormulaValues.tparFormulas(i).Position & ", " & vbCrLf & _
                                         " '" & pFormulaValues.tparFormulas(i).ValueType & "', " & vbCrLf & _
                                         " '" & pFormulaValues.tparFormulas(i).Value & "', " & vbCrLf

                        'Link fields that allow NULL values
                        If (pFormulaValues.tparFormulas(i).IsTestTypeNull) Then
                            cmdText &= "NULL, " & vbCrLf
                        Else
                            cmdText &= " '" & pFormulaValues.tparFormulas(i).TestType & "', " & vbCrLf
                        End If

                        If (pFormulaValues.tparFormulas(i).IsSampleTypeNull) Then
                            cmdText &= "NULL) " & vbCrLf
                        Else
                            cmdText &= " '" & pFormulaValues.tparFormulas(i).SampleType & "') " & vbCrLf
                        End If

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                            recordOK = (resultData.AffectedRecords = 1)
                        End Using
                        i += 1
                    Loop

                    If (recordOK) Then
                        resultData.HasError = False
                        resultData.AffectedRecords = i
                        resultData.SetDatos = pFormulaValues

                    Else
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                        resultData.AffectedRecords = 0
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparFormulasDAO.Create", EventLogEntryType.Error, False)
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
        ''' Modified by: SA 12/03/2012 - Changed the function template
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM tparFormulas " & vbCrLf & _
                                            " WHERE  CalcTestID = " & pCalcTestID.ToString & vbCrLf

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
                GlobalBase.CreateLogActivity(ex.Message, "tparFormulasDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        
#End Region

#Region "Other Methods"
        ''' <summary>
        '''  Get all values included in the Formula defined for the specified Calculated Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTestID">Identifier of the Calculated Test</param>
        ''' <returns>GlobalDataTO containing a typed DataSet FormulasDS with all components of the Calculated Test Formula</returns>
        ''' <remarks>
        ''' Modified by: SA 21/06/2010 - Function name changed from ReadAll to GetFormulaValues; changed the SQL query to get also
        '''                              the Test Name for Formula members with ValueType=TEST
        '''              SA  12/03/2012 - Changed the function template
        '''              JB  31/01/2013 - Add optional parameter DataBaseName and use it in query
        '''              WE  11/11/2014 - RQ00035C (BA-1867).
        '''</remarks>
        Public Function GetFormulaValues(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestID As Integer, Optional pDataBaseName As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim strFromLeft As String = ""
                        If (Not String.IsNullOrEmpty(pDataBaseName)) Then strFromLeft = pDataBaseName & ".dbo."

                        Dim cmdText As String = " SELECT F.*, T.TestName + ' ['+ F.SampleType + ']' AS TestName " & vbCrLf & _
                                                " FROM   " & strFromLeft & "tparFormulas F INNER JOIN " & strFromLeft & "tparTests T ON CONVERT(int, F.[Value]) = T.TestID " & vbCrLf & _
                                                " WHERE  F.CalcTestID = " & pCalcTestID.ToString & vbCrLf & _
                                                " AND    F.ValueType = 'TEST' " & vbCrLf & _
                                                " AND    F.TestType = 'STD' " & vbCrLf & _
                                                " UNION " & vbCrLf & _
                                                " SELECT F.*, CT.CalcTestLongName + ' ['+ F.SampleType + ']' AS TestName " & vbCrLf & _
                                                " FROM   " & strFromLeft & "tparFormulas F INNER JOIN " & strFromLeft & "tparCalculatedTests CT ON CONVERT(int, F.[Value]) = CT.CalcTestID " & vbCrLf & _
                                                " WHERE  F.CalcTestID = " & pCalcTestID.ToString & vbCrLf & _
                                                " AND    F.ValueType = 'TEST' " & vbCrLf & _
                                                " AND    F.TestType = 'CALC' " & vbCrLf & _
                                                " UNION " & vbCrLf & _
                                                " SELECT F.*, IT.Name + ' ['+ F.SampleType + ']' AS TestName " & vbCrLf & _
                                                " FROM   " & strFromLeft & "tparFormulas F INNER JOIN " & strFromLeft & "tparISETests IT ON CONVERT(int, F.[Value]) = IT.ISETestID " & vbCrLf & _
                                                " WHERE  F.CalcTestID = " & pCalcTestID.ToString & vbCrLf & _
                                                " AND    F.ValueType = 'TEST' " & vbCrLf & _
                                                " AND    F.TestType = 'ISE' " & vbCrLf & _
                                                " UNION " & vbCrLf & _
                                                " SELECT F.*, OT.Name + ' ['+ F.SampleType + ']' AS TestName " & vbCrLf & _
                                                " FROM   " & strFromLeft & "tparFormulas F INNER JOIN " & strFromLeft & "tparOffSystemTests OT ON CONVERT(int, F.[Value]) = OT.OffSystemTestID " & vbCrLf & _
                                                " WHERE  F.CalcTestID = " & pCalcTestID.ToString & vbCrLf & _
                                                " AND    F.ValueType = 'TEST' " & vbCrLf & _
                                                " AND    F.TestType = 'OFFS' " & vbCrLf & _
                                                " UNION " & vbCrLf & _
                                                " SELECT F.*, NULL AS TestName " & vbCrLf & _
                                                " FROM   " & strFromLeft & "tparFormulas F " & vbCrLf & _
                                                " WHERE  F.CalcTestID = " & pCalcTestID.ToString & _
                                                " AND    F.ValueType <> 'TEST' " & vbCrLf & _
                                                " ORDER BY F.Position " & vbCrLf

                        Dim myFormula As New FormulasDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myFormula.tparFormulas)
                            End Using
                        End Using

                        resultData.SetDatos = myFormula
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparFormulasDAO.GetFormulaValues", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Tests included in the Formula defined for a specified Calculated Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTestID">Identifier of the Calculated Test</param>
        ''' <returns>GlobalDataTO containing a typed DataSet FormulasDS with the list of Tests (standard 
        '''          and calculated) included in the Formula defined for the specified Calculated Test</returns>
        ''' <remarks>
        ''' Created by:  GDS 03/05/2010
        ''' Modified by: SA  04/05/2010 - Removed the TestID alias for Value column to match the DataSet Structure
        '''              SA  26/05/2010 - Query changed to get also the name of the informed Calculated Test and the Formula Text
        '''              SA  12/03/2012 - Changed the function template
        '''              SA  18/04/2012 - Changed the SQL by adding a DISTINCT due to the same Test can be more than once in a Formula Text
        ''' </remarks>
        Public Function ReadTestsInFormula(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT F.CalcTestID, TC.CalcTestLongName AS TestName, TC.FormulaText, F.TestType, " & vbCrLf & _
                                                                " F.ValueType, F.[Value], F.SampleType " & vbCrLf & _
                                                " FROM   tparFormulas F INNER JOIN tparCalculatedTests TC ON F.CalcTestID = TC.CalcTestID " & vbCrLf & _
                                                " WHERE  F.CalcTestID = " & pCalcTestID.ToString & vbCrLf & _
                                                " AND    F.ValueType  = 'TEST' " & vbCrLf & _
                                                " ORDER BY F.TestType, F.SampleType, F.[Value] " & vbCrLf

                        Dim myFormula As New FormulasDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myFormula.tparFormulas)
                            End Using
                        End Using

                        resultData.SetDatos = myFormula
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparFormulasDAO.ReadTestsInFormula", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if the specified Test (Standard, ISE, Off-System or Calculated Test) is included in the formula of a Calculated Test,
        ''' filtering data by Sample Type if this value has been informed.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pTestType">Type of Test (STD,ISE,OFFS,CALC).</param>
        ''' <param name="pExcludeSampleTypes">When True, it indicates the Test will be searched in Formulas but using a
        '''                                   SampleType different of the specified ones. Optional parameter.</param>
        ''' <returns>GlobalDataTO containing a typed DataSet FormulasDS with the Identifier and Name of the Calculated
        '''          Test(s) in which formula the specified Test is included.</returns>
        ''' <remarks>
        ''' Created by:  TR 17/05/2010
        ''' Modified by: SA 14/01/2010 - Added new optional parameter to allow search a Test in a Formula but using a SampleType 
        '''                              different of the specified ones
        '''              SA 12/03/2012 - Changed the function template
        '''              WE 21/11/2014 - RQ00035C (BA-1867): change Summary and Parameter description.
        ''' </remarks>
        Public Function ReadFormulaByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String, _
                                            ByVal pTestType As String, Optional ByVal pExcludeSampleTypes As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT F.*, CT.CalcTestLongName AS TestName, CT.FormulaText " & vbCrLf & _
                                                " FROM   tparFormulas F INNER JOIN tparCalculatedTests CT ON F.CalcTestID = CT.CalcTestID " & vbCrLf & _
                                                " WHERE  F.ValueType = 'TEST' " & vbCrLf & _
                                                " AND    F.TestType  = '" & pTestType & "' " & vbCrLf & _
                                                " AND    F.Value     = '" & pTestID.ToString & "' " & vbCrLf

                        If (pSampleType <> "") Then
                            If (Not pExcludeSampleTypes) Then
                                cmdText &= " AND F.SampleType = '" & pSampleType & "' " & vbCrLf
                            Else
                                cmdText &= " AND F.SampleType NOT IN (" & pSampleType & ") " & vbCrLf
                            End If
                        End If

                        Dim myFormula As New FormulasDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myFormula.tparFormulas)
                            End Using
                        End Using

                        resultData.SetDatos = myFormula
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparFormulasDAO.ReadFormulaByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' auxiliar method for updating Value field after Database version update
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pOldCalcTestValue"></param>
        ''' <param name="pNewCalcTestValue"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 21/02/2013</remarks>
        Public Function UpdateCalcTestValueAfterDBUpdate(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOldCalcTestValue As Integer, ByVal pNewCalcTestValue As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    'Get the connected Username from the current Application Session
                    Dim cmdText As String
                    cmdText = " UPDATE tparFormulas " & _
                              " SET  Value = '" & pNewCalcTestValue.ToString & "' "

                    cmdText &= " WHERE ValueType = 'TEST'"
                    cmdText &= " AND TestType = 'CALC'"
                    cmdText &= " AND Value = '" & pOldCalcTestValue.ToString & "' "

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
                GlobalBase.CreateLogActivity(ex.Message, "tparFormulasDAO.UpdateCalcTestValueAfterDBUpdate", EventLogEntryType.Error, False)
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
        '''</remarks>
        Public Function GetCalculatedTestIntoFormula(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestID As Integer, Optional pDataBaseName As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim strFromLeft As String = ""
                        If (Not String.IsNullOrEmpty(pDataBaseName)) Then strFromLeft = pDataBaseName & ".dbo."

                        Dim cmdText As String = " SELECT CalcTestID, Position, ValueType, TestType, Value, SampleType " & vbCrLf & _
                                                " FROM   " & strFromLeft & "tparFormulas " & vbCrLf & _
                                                " WHERE  Value = " & pCalcTestID.ToString & vbCrLf & _
                                                " AND    ValueType = 'TEST' " & vbCrLf & _
                                                " AND    TestType = 'CALC' "

                        Dim myFormula As New FormulasDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myFormula.tparFormulas)
                            End Using
                        End Using

                        resultData.SetDatos = myFormula
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparFormulasDAO.GetCalculatedTestIntoFormula", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region
    End Class
End Namespace