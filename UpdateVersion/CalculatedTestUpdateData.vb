Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.BL.UpdateVersion
    Public Class CalculatedTestUpdateData
        Inherits UpdateElementParent

#Region " Internal Data "
#Region " DataTable "
        ''' <summary>
        ''' Gets the DataTable inside the DataSet: the result of comparation of DataBases
        ''' </summary>
        ''' <param name="pMyDataSet"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function GetDataTable(pMyDataSet As DataSet) As DataTable
            Return pMyDataSet.Tables(0)
        End Function
#End Region
        ''' <summary>
        ''' Contains all related Data in a Calculated Test
        ''' </summary>
        ''' <remarks>
        ''' Created By: JB - 30/01/2013
        ''' </remarks>
        Protected Structure DataInDB
            Public calcTestDS As CalculatedTestsDS
            Public formulaDS As FormulasDS
            Public refRangesDS As TestRefRangesDS

            ''' <summary>
            ''' Fill CalcTestDS in structure, searching by CalcTestName
            ''' </summary>
            ''' <param name="pDBConnection"></param>
            ''' <param name="pCalcTestName"></param>
            ''' <returns></returns>
            ''' <remarks>
            ''' Created By: DL - 30/01/2013
            ''' </remarks>
            Public Function SearchCalcTestByName(pDBConnection As SqlClient.SqlConnection, pCalcTestName As String, Optional factoryDB As Boolean = False) As Boolean
                Dim dbConnection As SqlClient.SqlConnection = Nothing

                calcTestDS = New CalculatedTestsDS()
                Try
                    Dim resultData As GlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                    If resultData.HasError Then Return False
                    If resultData.SetDatos Is Nothing Then Return False

                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If dbConnection Is Nothing Then Return False

                    Dim dbName As String = ""
                    If factoryDB Then dbName = GlobalBase.TemporalDBName

                    Dim myDAO As New tparCalculatedTestsDAO
                    resultData = myDAO.ReadByCalcTestName(dbConnection, pCalcTestName, pDataBaseName:=dbName)
                    If resultData.HasError Then Return False
                    If resultData.SetDatos Is Nothing Then Return False

                    calcTestDS = DirectCast(resultData.SetDatos, CalculatedTestsDS)

                    Return True

                Catch ex As Exception
                    Dim myLogAcciones As New ApplicationLogManager()
                    myLogAcciones.CreateLogActivity(ex.Message, "CalculatedTestUpdateData.DataInDB.SearchCalcTestByName", EventLogEntryType.Error, False)
                Finally
                    If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
                End Try
                Return False
            End Function

            ''' <summary>
            ''' Fill CalcTestDS in structure, searching by CalcTestLongName
            ''' </summary>
            ''' <param name="pDBConnection"></param>
            ''' <param name="pCalcTestLongName"></param>
            ''' <param name="factoryDB"></param>
            ''' <returns></returns>
            ''' <remarks>Created by: SG 15/02/2013</remarks>
            Public Function SearchCalcTestByLongName(pDBConnection As SqlClient.SqlConnection, pCalcTestLongName As String, Optional factoryDB As Boolean = False) As Boolean
                Dim dbConnection As SqlClient.SqlConnection = Nothing

                calcTestDS = New CalculatedTestsDS()
                Try
                    Dim resultData As GlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                    If resultData.HasError Then Return False
                    If resultData.SetDatos Is Nothing Then Return False

                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If dbConnection Is Nothing Then Return False

                    Dim dbName As String = ""
                    If factoryDB Then dbName = GlobalBase.TemporalDBName

                    Dim myDAO As New tparCalculatedTestsDAO
                    resultData = myDAO.ReadByCalcTestLongName(dbConnection, pCalcTestLongName, pDataBaseName:=dbName)
                    If resultData.HasError Then Return False
                    If resultData.SetDatos Is Nothing Then Return False

                    calcTestDS = DirectCast(resultData.SetDatos, CalculatedTestsDS)

                    Return True

                Catch ex As Exception
                    Dim myLogAcciones As New ApplicationLogManager()
                    myLogAcciones.CreateLogActivity(ex.Message, "CalculatedTestUpdateData.DataInDB.SearchCalcTestByLongName", EventLogEntryType.Error, False)
                Finally
                    If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
                End Try
                Return False
            End Function

            ''' <summary>
            ''' Fill FormulasDS in structure, searching by CalcTestID
            ''' </summary>
            ''' <param name="pDBConnection"></param>
            ''' <param name="pCalcTestId"></param>
            ''' <returns></returns>
            ''' <remarks>
            ''' Created By: DL - 30/01/2013
            ''' </remarks>
            Public Function SearchFormulaByCalcTestId(pDBConnection As SqlClient.SqlConnection, pCalcTestId As Integer, Optional factoryDB As Boolean = False) As Boolean
                Dim dbConnection As SqlClient.SqlConnection = Nothing

                formulaDS = New FormulasDS()
                Try
                    Dim resultData As GlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                    If resultData.HasError Then Return False
                    If resultData.SetDatos Is Nothing Then Return False

                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If dbConnection Is Nothing Then Return False

                    Dim dbName As String = ""
                    If factoryDB Then dbName = GlobalBase.TemporalDBName

                    Dim myDAO As New tparFormulasDAO
                    resultData = myDAO.GetFormulaValues(dbConnection, pCalcTestId, pDataBaseName:=dbName)
                    If resultData.HasError Then Return False
                    If resultData.SetDatos Is Nothing Then Return False

                    formulaDS = DirectCast(resultData.SetDatos, FormulasDS)
                    Return True
                Catch ex As Exception
                    Dim myLogAcciones As New ApplicationLogManager()
                    myLogAcciones.CreateLogActivity(ex.Message, "CalculatedTestUpdateData.DataInDB.SearchFormulaByCalcTestId", EventLogEntryType.Error, False)
                Finally
                    If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
                End Try
                Return False
            End Function

            ''' <summary>
            ''' Fill TestRefRangesDS in structure, searching by CalcTestID
            ''' </summary>
            ''' <param name="pDBConnection"></param>
            ''' <param name="pCalcTestId"></param>
            ''' <returns></returns>
            ''' <remarks>
            ''' Created By: JB - 30/01/2013
            ''' </remarks>
            Public Function SearchRefRangesByCalcTestId(pDBConnection As SqlClient.SqlConnection, pCalcTestId As Integer, Optional factoryDB As Boolean = False) As Boolean
                Dim dbConnection As SqlClient.SqlConnection = Nothing

                refRangesDS = New TestRefRangesDS()
                Try
                    Dim resultData As GlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                    If resultData.HasError Then Return False
                    If resultData.SetDatos Is Nothing Then Return False

                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If dbConnection Is Nothing Then Return False

                    Dim dbName As String = ""
                    If factoryDB Then dbName = GlobalBase.TemporalDBName

                    Dim myDAO As New tparTestRefRangesDAO
                    resultData = myDAO.ReadByTestID(dbConnection, pCalcTestId, , , "CALC", pDataBaseName:=dbName)
                    If resultData.HasError Then Return False
                    If resultData.SetDatos Is Nothing Then Return False

                    refRangesDS = DirectCast(resultData.SetDatos, TestRefRangesDS)
                    Return True

                Catch ex As Exception
                    Dim myLogAcciones As New ApplicationLogManager()
                    myLogAcciones.CreateLogActivity(ex.Message, "CalculatedTestUpdateData.DataInDB.SearchRefRangesByCalcTestId", EventLogEntryType.Error, False)
                Finally
                    If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
                End Try
                Return False
            End Function

            ''' <summary>
            ''' Fill CalcTestDS in structure, searching by BiosystemsID
            ''' </summary>
            ''' <param name="pDBConnection"></param>
            ''' <param name="pBiosystemsID"></param>
            ''' <returns></returns>
            ''' <remarks>
            ''' Created By: SG 19/02/2013
            ''' </remarks>
            Public Function SearchCalcTestByBiosystemsID(pDBConnection As SqlClient.SqlConnection, pBiosystemsID As Integer, Optional factoryDB As Boolean = False) As Boolean
                Dim dbConnection As SqlClient.SqlConnection = Nothing

                calcTestDS = New CalculatedTestsDS()
                Try
                    Dim resultData As GlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                    If resultData.HasError Then Return False
                    If resultData.SetDatos Is Nothing Then Return False

                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If dbConnection Is Nothing Then Return False

                    Dim dbName As String = ""
                    If factoryDB Then dbName = GlobalBase.TemporalDBName

                    Dim myDAO As New tparCalculatedTestsDAO
                    resultData = myDAO.ReadByBiosystemsID(dbConnection, pBiosystemsID, pDataBaseName:=dbName)
                    If resultData.HasError Then Return False
                    If resultData.SetDatos Is Nothing Then Return False

                    calcTestDS = DirectCast(resultData.SetDatos, CalculatedTestsDS)

                    Return True

                Catch ex As Exception
                    Dim myLogAcciones As New ApplicationLogManager()
                    myLogAcciones.CreateLogActivity(ex.Message, "CalculatedTestUpdateData.DataInDB.SearchCalcTestByBiosystemsID", EventLogEntryType.Error, False)
                Finally
                    If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
                End Try
                Return False
            End Function

            ''' <summary>
            ''' Returns if represented Data is Empty
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks>
            ''' Created by: DL - 07/02/2013
            ''' </remarks>
            Public Function IsDataEmpty() As Boolean
                Return calcTestDS Is Nothing OrElse calcTestDS.Tables.Count = 0 OrElse calcTestDS.tparCalculatedTests.Rows.Count = 0
            End Function
        End Structure

        ''' <summary>
        ''' Gets the structured data in a dataBase, from the result of comparation of DataBases
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pItemRow">Row of a result of comparation of Client and Factory DataBases</param>
        ''' <param name="pfactoryDB">Determine which databases use to get data: Factory DB or Client DB</param>
        ''' <returns>
        ''' A GlobalDataTO with a DataInDB structure inside
        ''' </returns>
        ''' <remarks>
        ''' Created by: DL 31/01/2013
        ''' Modified by: SG 15/02/2013 - find by Name or Long Name
        ''' Modified by: SG 19/02/2013 - find by new field BiosystemsID
        ''' </remarks>
        Private Function GetDataInDB(pDBConnection As SqlClient.SqlConnection,
                                             pItemRow As DataRow,
                                             pfactoryDB As Boolean) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim dataDB As New DataInDB()
                Dim bRet As Boolean

                'SGM 19/02/2013
                Dim biosystemsID As Integer
                If Not TypeOf pItemRow.Item("BiosystemsID") Is System.DBNull Then
                    biosystemsID = CInt(pItemRow.Item("BiosystemsID"))
                    'find by BiosystemsID
                    bRet = dataDB.SearchCalcTestByBiosystemsID(pDBConnection, biosystemsID, pfactoryDB) 'SGM 19/02/2013
                    If Not bRet OrElse dataDB.calcTestDS.tparCalculatedTests.Rows.Count = 0 Then Return myGlobalDataTO

                Else

                    Dim calcTestName As String = pItemRow.Item("CalcTestName").ToString()
                    If String.IsNullOrEmpty(calcTestName) Then Return myGlobalDataTO

                    Dim calcTestLongName As String = pItemRow.Item("CalcTestLongName").ToString() 'SGM 15/02/2012
                    If String.IsNullOrEmpty(calcTestLongName) Then Return myGlobalDataTO

                    'SGM 15/02/2013 - find by Name or Long Name
                    bRet = dataDB.SearchCalcTestByName(pDBConnection, calcTestName, pfactoryDB)
                    If bRet AndAlso dataDB.calcTestDS.tparCalculatedTests.Rows.Count = 0 Then
                        bRet = dataDB.SearchCalcTestByLongName(pDBConnection, calcTestLongName, pfactoryDB)
                        If Not bRet OrElse dataDB.calcTestDS.tparCalculatedTests.Rows.Count = 0 Then Return myGlobalDataTO
                    ElseIf Not bRet Then
                        Return myGlobalDataTO
                    End If
                    'end SGM 15/02/2013

                End If
                'end SGM 19/02/2013



                Dim calcTestId As Integer = DirectCast(dataDB.calcTestDS.tparCalculatedTests.Rows(0), CalculatedTestsDS.tparCalculatedTestsRow).CalcTestID

                bRet = dataDB.SearchFormulaByCalcTestId(pDBConnection, calcTestId, pfactoryDB)
                If Not bRet Then Return myGlobalDataTO

                bRet = dataDB.SearchRefRangesByCalcTestId(pDBConnection, calcTestId, pfactoryDB)
                If Not bRet Then Return myGlobalDataTO

                myGlobalDataTO.SetDatos = dataDB

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.GetDataInDB", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        Protected Overrides Function GetDataInFactoryDB(pDBConnection As SqlClient.SqlConnection, pItemRow As DataRow) As GlobalDataTO
            Return GetDataInDB(pDBConnection, pItemRow, True)
        End Function

        Protected Overrides Function GetDataInLocalDB(pDBConnection As SqlClient.SqlConnection, pItemRow As DataRow) As GlobalDataTO
            Return GetDataInDB(pDBConnection, pItemRow, False)
        End Function

        Protected Overrides Function IsDataEmpty(pDataInDB As Object) As Boolean
            Return pDataInDB Is Nothing OrElse DirectCast(pDataInDB, DataInDB).IsDataEmpty()
        End Function

        ''' <summary>
        ''' Gets if the related CalcTest is FactoryDefined (Preloaded)
        ''' </summary>
        ''' <param name="pDataInDB"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function IsFactoryDefinedItem(pDataInDB As Object, pDBConnection As SqlClient.SqlConnection) As Boolean
            Try
                If (pDataInDB Is Nothing) Then Return False
                Dim calcTestData As DataInDB = DirectCast(pDataInDB, DataInDB)
                If (calcTestData.calcTestDS Is Nothing) Then Return False
                If (calcTestData.calcTestDS.tparCalculatedTests.Rows.Count = 0) Then Return False

                Dim row As CalculatedTestsDS.tparCalculatedTestsRow
                row = DirectCast(calcTestData.calcTestDS.tparCalculatedTests.Rows(0), CalculatedTestsDS.tparCalculatedTestsRow)
                Return row.PreloadedCalculatedTest And Not row.IsBiosystemsIDNull 'SGM 19/02/2013 add BiosystemsID field not null
                'Return row.PreloadedCalculatedTest

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.IsFactoryDefinedItem", EventLogEntryType.Error, False)
            End Try

            Return False
        End Function
#End Region

#Region " Factory Updates "
        Protected Overrides Function GetAffectedItemsFromFactoryUpdates(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                myGlobalDataTO = CalculatedTestUpdateDAO.GetPreloadedCALCTestsDistinctInClient(pDBConnection)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.GetAffectedItemsFromFactoryUpdates", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

#Region " Do Actions "
        ''' <summary>
        ''' Create new element
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB"></param>
        ''' <param name="pDataInLocalDB"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: DL 07/02/2013
        ''' Modified by: XB 15/02/2013 - set ref ranges as New (Bugs tracking #1134)
        ''' Modified by: SG 20/02/2013 - rename other possible test before creting a new one in case it has already the same name or longname
        ''' </remarks>
        Protected Overrides Function DoCreateNewElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If pDataInFactoryDB Is Nothing Then Return myGlobalDataTO
                Dim dataInFactoryDB As DataInDB = DirectCast(pDataInFactoryDB, DataInDB)
                If dataInFactoryDB.IsDataEmpty Then Return myGlobalDataTO

                'Create new CALC Test like in Factory DB
                'All data to insert is inside dataInFactoryDB

                'SGM 20/02/2013
                'before create from factory find any possible user test with the same Name or ShortName
                myGlobalDataTO = MyClass.RenameCalcTestNameWhenUpdateOrCreateNew(pDBConnection, dataInFactoryDB)
                If myGlobalDataTO.HasError Then Return myGlobalDataTO

                Dim myCalTest As New CalculatedTestsDS
                myCalTest = DirectCast(dataInFactoryDB.calcTestDS, CalculatedTestsDS)

                For Each calcRow As CalculatedTestsDS.tparCalculatedTestsRow In myCalTest.tparCalculatedTests.Rows
                    calcRow.BeginEdit()
                    calcRow.PreloadedCalculatedTest = True
                    calcRow.EndEdit()
                Next

                Dim myFormula As New FormulasDS
                myFormula = DirectCast(dataInFactoryDB.formulaDS, FormulasDS)

                Dim myRefRanges As New TestRefRangesDS
                myRefRanges = DirectCast(dataInFactoryDB.refRangesDS, TestRefRangesDS)

                If (myRefRanges.tparTestRefRanges.Rows.Count > 0) Then
                    For Each refRangRow As TestRefRangesDS.tparTestRefRangesRow In myRefRanges.tparTestRefRanges
                        refRangRow.BeginEdit()
                        refRangRow.IsNew = True
                        refRangRow.EndEdit()
                    Next
                End If

                Dim calcTestDelegate As New CalculatedTestsDelegate
                myGlobalDataTO = calcTestDelegate.Add(pDBConnection,
                                                      myCalTest,
                                                      myFormula,
                                                      myRefRanges)


            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.DoCreateNewElement", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        Protected Overrides Function DoUpdateUserDefinedElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim mycalcTestDS As New CalculatedTestsDS

            Try
                If pDataInLocalDB Is Nothing Then Return myGlobalDataTO
                Dim dataInLocalDB As DataInDB = DirectCast(pDataInLocalDB, DataInDB)
                If dataInLocalDB.IsDataEmpty Then Return myGlobalDataTO

                mycalcTestDS = DirectCast(pDataInLocalDB, DataInDB).calcTestDS

                If mycalcTestDS.tparCalculatedTests.Rows.Count > 0 Then

                    'Already exists in client DataBase as User test
                    If Not mycalcTestDS.tparCalculatedTests.First.PreloadedCalculatedTest Then
                        'Rename calibrator name adding (R) at the beginning of the name validting that not exist in db


                        'SGM 15/02/2013 - bug #1134
                        myGlobalDataTO = MyClass.RenameCalcTestName(pDBConnection, mycalcTestDS.tparCalculatedTests.First)
                        If Not myGlobalDataTO.HasError Then
                            'Dim IsOK As Boolean = False
                            'Dim myNewname As String = ""
                            'Dim myCalculatedTests As New tparCalculatedTestsDAO

                            'While Not IsOK
                            '    myNewname &= "R" & mycalcTestDS.tparCalculatedTests.First.CalcTestName

                            '    myGlobalDataTO = myCalculatedTests.ReadByCalcTestName(pDBConnection, myNewname)

                            '    If Not myGlobalDataTO.HasError Then
                            '        Dim pcalcTestDS As New CalculatedTestsDS
                            '        pcalcTestDS = DirectCast(myGlobalDataTO.SetDatos, CalculatedTestsDS)

                            '        If pcalcTestDS.tparCalculatedTests.Rows.Count = 0 Then IsOK = True
                            '    End If
                            'End While

                            'For Each calcRow As CalculatedTestsDS.tparCalculatedTestsRow In mycalcTestDS.tparCalculatedTests
                            '    calcRow.BeginEdit()
                            '    calcRow.CalcTestName = myNewname
                            '    calcRow.EndEdit()
                            'Next



                            'Rename client user CALC Test in client Database and all related data. Adding (R) at the beginning of the name
                            Dim calcTestDelegate As New CalculatedTestsDelegate
                            Dim myFormulas As New FormulasDS
                            Dim myRefRanges As New TestRefRangesDS

                            myFormulas = DirectCast(pDataInLocalDB, DataInDB).formulaDS
                            myRefRanges = DirectCast(pDataInLocalDB, DataInDB).refRangesDS

                            myGlobalDataTO = calcTestDelegate.Modify(pDBConnection, mycalcTestDS, myFormulas, myRefRanges)

                            'Create preloaded CALC Test in client DataBase
                            If Not myGlobalDataTO.HasError Then
                                mycalcTestDS = DirectCast(pDataInFactoryDB, DataInDB).calcTestDS

                                For Each calcRow As CalculatedTestsDS.tparCalculatedTestsRow In mycalcTestDS.tparCalculatedTests
                                    calcRow.BeginEdit()
                                    calcRow.PreloadedCalculatedTest = True
                                    calcRow.EndEdit()
                                Next

                                myFormulas = DirectCast(pDataInFactoryDB, DataInDB).formulaDS
                                myRefRanges = DirectCast(pDataInFactoryDB, DataInDB).refRangesDS

                                myGlobalDataTO = calcTestDelegate.Add(pDBConnection, mycalcTestDS, myFormulas, myRefRanges)
                            End If
                        End If
                        'end SGM 15/02/2013 - bug #1134

                    Else
                        'Already exists in client DataBase as Preloaded test
                        Dim calcTestDelegate As New CalculatedTestsDelegate
                        Dim myFormulas As New FormulasDS
                        Dim myRefRanges As New TestRefRangesDS

                        myFormulas = DirectCast(pDataInLocalDB, DataInDB).formulaDS
                        myRefRanges = DirectCast(pDataInLocalDB, DataInDB).refRangesDS

                        myGlobalDataTO = calcTestDelegate.Modify(pDBConnection, mycalcTestDS, myFormulas, myRefRanges)

                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.DoUpdateUserDefinedElement", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get a valid  CalcTestName and CalcTestLongName.
        ''' This method is used by the Update process.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pCalcTestsRow"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: SGM 15/02/2013
        ''' </remarks>
        Private Function RenameCalcTestName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestsRow As CalculatedTestsDS.tparCalculatedTestsRow) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try

                'Validate Change the name and validate if do not exist on Client DB.
                Dim isValidNewName As Boolean = False
                Dim isValidNewLongName As Boolean = False

                Dim myValidCalcTestName As String = String.Empty
                Dim myValidCalcTestLongName As String = String.Empty
                myValidCalcTestName = pCalcTestsRow.CalcTestName
                myValidCalcTestLongName = pCalcTestsRow.CalcTestLongName

                Dim myCalcTestDAO As New tparCalculatedTestsDAO
                Dim myCalcTestDS As New CalculatedTestsDS



                While Not isValidNewName OrElse Not isValidNewLongName

                    'NAME
                    If Not isValidNewName Then
                        'Add the R at the begining
                        myValidCalcTestName = "R" & myValidCalcTestName

                        'Validate the name lenght should not be more than 8
                        If myValidCalcTestName.Length > 8 Then
                            'Remove the last letter
                            myValidCalcTestName = myValidCalcTestName.Remove(myValidCalcTestName.Length - 1)
                        End If

                        'Search on local db if name exist.
                        myGlobalDataTO = myCalcTestDAO.ExistsCalculatedTest(pDBConnection, myValidCalcTestName, "NAME")
                        If Not myGlobalDataTO.HasError AndAlso Not DirectCast(myGlobalDataTO.SetDatos, Boolean) Then
                            'If Not exist then is a valid name
                            isValidNewName = True
                        End If

                    End If

                    'LONG NAME
                    If Not isValidNewLongName Then
                        'Add the R at the begining
                        myValidCalcTestLongName = "R" & myValidCalcTestLongName

                        'Validate the name lenght should not be more than 16
                        If myValidCalcTestLongName.Length > 16 Then
                            'Remove the last letter
                            myValidCalcTestLongName = myValidCalcTestLongName.Remove(myValidCalcTestLongName.Length - 1)
                        End If

                        'Search on local db if name exist.
                        myGlobalDataTO = myCalcTestDAO.ExistsCalculatedTest(pDBConnection, myValidCalcTestLongName, "FNAME")
                        If Not myGlobalDataTO.HasError AndAlso Not DirectCast(myGlobalDataTO.SetDatos, Boolean) Then
                            'If Not exist then is a valid name
                            isValidNewLongName = True
                        End If

                    End If

                End While

                pCalcTestsRow.BeginEdit()
                pCalcTestsRow.CalcTestName = myValidCalcTestName
                pCalcTestsRow.CalcTestLongName = myValidCalcTestLongName
                pCalcTestsRow.EndEdit()

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.RenameCalcTestName", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update Calc Test as Factory definition
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB"></param>
        ''' <param name="pDataInLocalDB"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XB: 14/02/2013 -  (Bugs tracking #1134)
        ''' Modified by: SG 20/02/2013 - rename other possible test before creting a new one in case it has already the same name or longname
        ''' </remarks>
        Protected Overrides Function DoUpdateFactoryDefinedElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If pDataInFactoryDB Is Nothing Then Return myGlobalDataTO
                Dim dataInFactoryDB As DataInDB = DirectCast(pDataInFactoryDB, DataInDB)
                If dataInFactoryDB.IsDataEmpty Then Return myGlobalDataTO

                If pDataInLocalDB Is Nothing Then Return myGlobalDataTO
                Dim dataInLocalDB As DataInDB = DirectCast(pDataInLocalDB, DataInDB)
                If dataInLocalDB.IsDataEmpty Then Return myGlobalDataTO

                'SGM 20/02/2013
                'before update from factory find any possible user test with the same Name or ShortName
                myGlobalDataTO = MyClass.RenameCalcTestNameWhenUpdateOrCreateNew(pDBConnection, dataInFactoryDB)
                If myGlobalDataTO.HasError Then Return myGlobalDataTO

                ' Take Calculated Test FACTORY values 
                Dim mycalcTest As New CalculatedTestsDS
                mycalcTest = DirectCast(dataInFactoryDB.calcTestDS, CalculatedTestsDS)
                ' Take Calculated Test LOCAL values 
                Dim myInLocalCalTest As New CalculatedTestsDS
                myInLocalCalTest = DirectCast(dataInLocalDB.calcTestDS, CalculatedTestsDS)

                ' Take Formula FACTORY values 
                Dim myFormula As New FormulasDS
                myFormula = DirectCast(dataInFactoryDB.formulaDS, FormulasDS)

                ' Take Ref Ranges depending on type
                Dim myRefRanges As New TestRefRangesDS
                Dim UserChoice As Boolean = True
                ' At begining Get FACTORY Ref Ranges values
                myRefRanges = DirectCast(dataInFactoryDB.refRangesDS, TestRefRangesDS)
                If (myRefRanges.tparTestRefRanges.Rows.Count > 0) Then
                    Dim myTestRefRanges As New List(Of TestRefRangesDS.tparTestRefRangesRow)

                    myTestRefRanges = (From a In myRefRanges.tparTestRefRanges _
                                       Where a.RangeType = "GENERIC" _
                                       Select a).ToList()

                    If myTestRefRanges.Count > 0 Then
                        UserChoice = False
                    End If
                End If

                ' Check if there are Ref ranges into local DB
                Dim myInLocalRefRanges As New TestRefRangesDS
                myInLocalRefRanges = DirectCast(dataInLocalDB.refRangesDS, TestRefRangesDS)
                If (myInLocalRefRanges.tparTestRefRanges.Rows.Count > 0) Then
                    ' There are !
                    ' Take USER ActiveRangeType in case exists
                    Dim myActiveRangeType As String = ""
                    For Each calcRow As CalculatedTestsDS.tparCalculatedTestsRow In myInLocalCalTest.tparCalculatedTests.Rows
                        myActiveRangeType = calcRow.ActiveRangeType
                    Next
                    For Each calcRow As CalculatedTestsDS.tparCalculatedTestsRow In mycalcTest.tparCalculatedTests.Rows
                        calcRow.BeginEdit()
                        calcRow.ActiveRangeType = myActiveRangeType
                        calcRow.EndEdit()
                    Next
                End If


                If UserChoice Then
                    ' Get USER Ref Ranges values in case there are not GENERIC Ref Ranges from Factory
                    myRefRanges = myInLocalRefRanges
                End If

                ' Check the if ID self-generated has changed, means it has a different value than the factory
                If Not myInLocalCalTest.tparCalculatedTests.First.CalcTestID = myInLocalCalTest.tparCalculatedTests.First.BiosystemsID Then
                    ' Assign the same ID that must be used for updates

                    'Set value of Calculated Test ID in the dataset containing the Calc Test Values
                    For i As Integer = 0 To mycalcTest.tparCalculatedTests.Rows.Count - 1
                        mycalcTest.tparCalculatedTests(i).CalcTestID = myInLocalCalTest.tparCalculatedTests.First.CalcTestID
                    Next i

                    'Set value of Calculated Test ID in the dataset containing the Formula Values
                    For i As Integer = 0 To myFormula.tparFormulas.Rows.Count - 1
                        myFormula.tparFormulas(i).CalcTestID = myInLocalCalTest.tparCalculatedTests.First.CalcTestID
                    Next i

                    'Set value of Calculated Test ID in the dataset containing the Reference Ranges
                    For i As Integer = 0 To myRefRanges.tparTestRefRanges.Rows.Count - 1
                        myRefRanges.tparTestRefRanges(i).TestID = myInLocalCalTest.tparCalculatedTests.First.CalcTestID
                    Next
                End If

                Dim calcTestDelegate As New CalculatedTestsDelegate
                myGlobalDataTO = calcTestDelegate.Modify(pDBConnection, mycalcTest, myFormula, myRefRanges)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.DoUpdateFactoryDefinedElement", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        Protected Overrides Function DoIgnoreElementUpdating(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            'No action is needed
            Return New GlobalDataTO
        End Function

        ''' <summary>
        ''' Renames Calulated Test Name and LongName before updating or creting new from factory
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 20/02/2013</remarks>
        Private Function RenameCalcTestNameWhenUpdateOrCreateNew(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As DataInDB) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO

            Try

                Dim myDAO As New tparCalculatedTestsDAO

                'rename because of same NAME
                Dim calcTestName As String = pDataInFactoryDB.calcTestDS.tparCalculatedTests.First.CalcTestName
                myGlobalDataTO = myDAO.ReadByCalcTestName(pDBConnection, calcTestName)
                If Not myGlobalDataTO.HasError AndAlso myGlobalDataTO.SetDatos IsNot Nothing Then
                    Dim myDS As CalculatedTestsDS = CType(myGlobalDataTO.SetDatos, CalculatedTestsDS)
                    If myDS.tparCalculatedTests.Rows.Count > 0 Then
                        If Not myDS.tparCalculatedTests.First.PreloadedCalculatedTest Then
                            myGlobalDataTO = MyClass.RenameCalcTestName(pDBConnection, myDS.tparCalculatedTests.First)
                            If Not myGlobalDataTO.HasError Then
                                myGlobalDataTO = myDAO.Update(pDBConnection, myDS)
                            End If
                        End If
                    End If
                End If

                'rename because of same LONG NAME
                Dim calcTestLongName As String = pDataInFactoryDB.calcTestDS.tparCalculatedTests.First.CalcTestLongName
                myGlobalDataTO = myDAO.ReadByCalcTestLongName(pDBConnection, calcTestName)
                If Not myGlobalDataTO.HasError AndAlso myGlobalDataTO.SetDatos IsNot Nothing Then
                    Dim myDS As CalculatedTestsDS = CType(myGlobalDataTO.SetDatos, CalculatedTestsDS)
                    If myDS.tparCalculatedTests.Rows.Count > 0 Then
                        If Not myDS.tparCalculatedTests.First.PreloadedCalculatedTest Then
                            myGlobalDataTO = MyClass.RenameCalcTestName(pDBConnection, myDS.tparCalculatedTests.First)
                            If Not myGlobalDataTO.HasError Then
                                myGlobalDataTO = myDAO.Update(pDBConnection, myDS)
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.RenameCalcTestNameWhenUpdateOrCreateNew", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Execute required final actions
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XB 21/02/2013 - (Bugs tracking #1134)
        ''' </remarks>
        Public Overrides Function DoFinalActions(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                myGlobalDataTO = UpdateDeprecatedIdentifiers(pDBConnection)
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.DoFinalActions", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Check Calc Test Identificadors that have been deprecated
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XB 21/02/2013 - (Bugs tracking #1134)
        ''' </remarks>
        Private Function UpdateDeprecatedIdentifiers(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim calTestList As New CalculatedTestsDelegate
                Dim myFormulaDelegate As New FormulasDelegate

                myGlobalDataTO = calTestList.GetList(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim calcTestLocalDS As New CalculatedTestsDS
                    calcTestLocalDS = DirectCast(myGlobalDataTO.SetDatos, CalculatedTestsDS)
                    Dim qCalcTests As List(Of CalculatedTestsDS.tparCalculatedTestsRow)

                    qCalcTests = (From a In calcTestLocalDS.tparCalculatedTests _
                                Select a).ToList()

                    For Each calculatedTest As CalculatedTestsDS.tparCalculatedTestsRow In qCalcTests
                        ' Check items that have different identifier between Local and Factory DB
                        If calculatedTest.PreloadedCalculatedTest AndAlso _
                           Not calculatedTest.IsBiosystemsIDNull AndAlso _
                           calculatedTest.CalcTestID <> calculatedTest.BiosystemsID Then

                            ' Get the original identifier in Factory
                            Dim OriginalIDValue As Integer
                            Dim dbName As String = GlobalBase.TemporalDBName
                            Dim myDAO As New tparCalculatedTestsDAO
                            myGlobalDataTO = myDAO.ReadByBiosystemsID(pDBConnection, calculatedTest.BiosystemsID, pDataBaseName:=dbName)
                            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                                Dim calcTestFactoryDS As New CalculatedTestsDS()
                                calcTestFactoryDS = DirectCast(myGlobalDataTO.SetDatos, CalculatedTestsDS)

                                If calcTestFactoryDS.tparCalculatedTests.Rows.Count > 0 Then
                                    OriginalIDValue = calcTestFactoryDS.tparCalculatedTests.First.CalcTestID
                                Else
                                    myGlobalDataTO.HasError = True
                                End If
                            End If

                            'Get the list of specified Test included in the Formula of the Calculated Test
                            myGlobalDataTO = myFormulaDelegate.GetCalculatedTestIntoFormula(pDBConnection, OriginalIDValue)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                Dim myFormulaDS As FormulasDS = DirectCast(myGlobalDataTO.SetDatos, FormulasDS)

                                For Each testRow As FormulasDS.tparFormulasRow In myFormulaDS.tparFormulas.Rows
                                    'For each item with deprecated identifier contained into Formula, update to new identifier
                                    myFormulaDelegate.UpdateCalcTestValueAfterDBUpdate(pDBConnection, OriginalIDValue, calculatedTest.CalcTestID)
                                Next

                            End If

                        End If
                    Next

                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.UpdateDeprecatedIdentifiers", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

#End Region
#End Region

#Region " Factory Removes "
        Protected Overrides Function GetAffectedItemsFromFactoryRemoves(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                myGlobalDataTO = CalculatedTestUpdateDAO.GetPreloadedCALCTestsDistinctInFactory(pDBConnection)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.GetAffectedItemsFromFactoryRemoves", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

#Region " Do Actions "
        ''' <summary>
        ''' Remove User Element
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB"></param>
        ''' <param name="pDataInLocalDB"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: DL 08/02/2013
        ''' </remarks>
        Protected Overrides Function DoRemoveUserElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If pDataInLocalDB Is Nothing Then Return myGlobalDataTO
                Dim dataInLocalDB As DataInDB = DirectCast(pDataInLocalDB, DataInDB)
                If dataInLocalDB.IsDataEmpty Then Return myGlobalDataTO

                Dim mycalcTestDS As New CalculatedTestsDS
                mycalcTestDS = DirectCast(pDataInLocalDB, DataInDB).calcTestDS

                If mycalcTestDS.tparCalculatedTests.Rows.Count > 0 Then
                    If mycalcTestDS.tparCalculatedTests.First.PreloadedCalculatedTest Then
                        'Already exists in client DataBase as Preloaded test
                        'Rename client user CALC Test in client Database and all related data. Adding (R) at the beginning of the name
                        Dim calcTestDelegate As New CalculatedTestsDelegate
                        Dim myFormulas As New FormulasDS
                        Dim myRefRanges As New TestRefRangesDS

                        myFormulas = DirectCast(pDataInLocalDB, DataInDB).formulaDS
                        myRefRanges = DirectCast(pDataInLocalDB, DataInDB).refRangesDS

                        myGlobalDataTO = calcTestDelegate.Modify(pDBConnection, mycalcTestDS, myFormulas, myRefRanges)

                        If Not myGlobalDataTO.HasError Then
                            'Remove preloaded test in client DataBase
                            myGlobalDataTO = calcTestDelegate.Delete(pDBConnection, mycalcTestDS)
                        End If

                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.DoRemoveUserElement", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        Protected Overrides Function DoRemoveFactoryElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If pDataInLocalDB Is Nothing Then Return myGlobalDataTO
                Dim dataInLocalDB As DataInDB = DirectCast(pDataInLocalDB, DataInDB)
                If dataInLocalDB.IsDataEmpty Then Return myGlobalDataTO

                'Remove CALC Test
                Dim calcTestDelegate As New CalculatedTestsDelegate
                myGlobalDataTO = calcTestDelegate.Delete(pDBConnection, dataInLocalDB.calcTestDS)

                'Update in HTML Report?

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.DoRemoveFactoryElement", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        Protected Overrides Function DoIgnoreElementRemoving(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            'No action is needed
            Return New GlobalDataTO
        End Function

        ''' <summary>
        ''' Returns the action to do with the concrete row in local DB if not match with Factory elements
        ''' </summary>
        ''' <param name="pDataInFactoryDB">Data of the element in Factory DB</param>
        ''' <param name="pDataInLocalDB">Data of the element in Local DB</param>
        ''' <returns>GlobalDataTO with LocalActionsWithFactoryRemoves</returns>
        ''' <remarks>
        ''' Created by: SGM 18/02/2013
        ''' </remarks>
        Protected Overrides Function GetActionFromFactoryRemoves(ByVal pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            myGlobalDataTO.SetDatos = LocalActionsWithFactoryRemoves.Ignore
            Try
                ' If Not Exists in Factory DB and:
                ' If Exists in Local DB as UserTest: Action = Ignored
                ' If Exists in Local DB as PreloadedTest: Action = RemoveFactoryDefined

                If IsDataEmpty(pDataInFactoryDB) Then
                    If IsFactoryDefinedItem(pDataInLocalDB, pDBConnection) Then
                        myGlobalDataTO.SetDatos = LocalActionsWithFactoryRemoves.RemoveFactoryDefined
                    Else
                        'not to remove if its not preloaded
                        myGlobalDataTO.SetDatos = LocalActionsWithFactoryRemoves.Ignore
                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.GetActionFromFactoryRemoves", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Returns the action to do with the concrete row in local DB, if Factory data not match with local elements
        ''' </summary>
        ''' <param name="pDataInFactoryDB">Data of the element in Factory DB</param>
        ''' <param name="pDataInLocalDB">Data of the element in Local DB</param>
        ''' <returns>GlobalDataTO with LocalActionsWithFactoryUpdates</returns>
        ''' <remarks>
        ''' Created by: SGM 18/02/2013
        ''' </remarks>
        Protected Overrides Function GetActionFromFactoryUpdates(ByVal pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            myGlobalDataTO.SetDatos = LocalActionsWithFactoryUpdates.Ignore
            Try
                ' If Not Exists in Local DB: Action = CreateNew
                ' If Exists in Local DB as UserTest: Action = UpdateUserDefined
                ' If Exists in Local DB as PreloadedTest: Action = UpdateFactoryDefined

                If IsDataEmpty(pDataInLocalDB) Then
                    myGlobalDataTO.SetDatos = LocalActionsWithFactoryUpdates.CreateNew
                ElseIf IsFactoryDefinedItem(pDataInLocalDB, pDBConnection) Then 'TR 08/05/2013 send the connection
                    myGlobalDataTO.SetDatos = LocalActionsWithFactoryUpdates.UpdateFactoryDefined
                Else
                    myGlobalDataTO.SetDatos = LocalActionsWithFactoryUpdates.UpdateUserDefined
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Elememt Update Error.", "CalculatedTestUpdateData.GetActionFromFactoryUpdates", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

#End Region
#End Region

#Region "FUNCTIONS FOR NEW UPDATE VERSION PROCESS"
        ''' <summary>
        ''' Search all CALC Tests in FACTORY DB that do not exist in Customer DB and, for each one of them, execute the process of adding it to Customer DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection></param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 15/10/2014 - BA-1944 (SubTask BA-2017)
        ''' </remarks>
        Public Function CreateNEWCALCTests(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myCalcTestID As Integer = -1
                Dim myNewFormulaDS As New FormulasDS
                Dim myNewTestDS As New CalculatedTestsDS
                Dim myNewRefRanges As New TestRefRangesDS
                Dim myFormulaText As String = String.Empty
                Dim myNewTestsList As New CalculatedTestsDS
                Dim myCalcTestUpdateDAO As New CalculatedTestUpdateDAO
                Dim myCalcTestsDelegate As New CalculatedTestsDelegate
                Dim myUpdateVersionAddedElementsRow As UpdateVersionChangesDS.AddedElementsRow

                '(1) Search in Factory DB all new CALC TESTS
                myGlobalDataTO = myCalcTestUpdateDAO.GetNewFactoryTests(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myNewTestsList = DirectCast(myGlobalDataTO.SetDatos, CalculatedTestsDS)

                    '(2) Process each new CALC Test in Factory DB to add it to Customer DB
                    For Each newTest As CalculatedTestsDS.tparCalculatedTestsRow In myNewTestsList.tparCalculatedTests
                        '(2.1) Get all data in table tparCalculatedTests in Factory DB
                        myGlobalDataTO = myCalcTestUpdateDAO.GetDataInFactoryDB(pDBConnection, newTest.BiosystemsID, True)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myNewTestDS = DirectCast(myGlobalDataTO.SetDatos, CalculatedTestsDS)
                        End If

                        '(2.2) Verify if there is an User CALC Test in Customer DB with the same Name and/or ShortName of the new Factory CALC Test, and in this case, 
                        '      rename the User CALC Test 
                        If (Not myGlobalDataTO.HasError) Then
                            ' myGlobalDataTO = UpdateRenamedTest(pDBConnection, myNewTestDS.tparTests.First.TestName, myNewTestDS.tparTests.First.ShortName, pUpdateVersionChangesList)
                        End If

                        '(2.3) Get all members of the Formula of the NEW CALC Test in FACTORY DB
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myCalcTestUpdateDAO.GetFormulaInFactoryDB(pDBConnection, newTest.CalcTestID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myNewFormulaDS = DirectCast(myGlobalDataTO.SetDatos, FormulasDS)

                                For Each formulaMember As FormulasDS.tparFormulasRow In myNewFormulaDS.tparFormulas
                                    If (formulaMember.Value = "TEST") Then
                                        'Get the Test name in CUSTOMER DB
                                        myGlobalDataTO = GetTestNameForFormula(pDBConnection, formulaMember.TestType, Convert.ToInt32(formulaMember.Value), formulaMember.SampleType)
                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            myFormulaText &= Convert.ToString(myGlobalDataTO.SetDatos)
                                        Else
                                            'If an error has been raised, then the process is stopped...
                                            Exit For
                                        End If

                                        'If the Formula Member is a CALC Test, get value of CalcTestID in CUSTOMER DB
                                    Else
                                        myFormulaText &= formulaMember.ValueType.Trim
                                    End If
                                Next

                                If (Not myGlobalDataTO.HasError) Then
                                    'Update the FormulaText field for the Calculated Test (to use the name of Tests in CUSTOMER DB)
                                    myNewTestDS.tparCalculatedTests.First.FormulaText = myFormulaText
                                    myNewTestDS.tparCalculatedTests.AcceptChanges()
                                End If
                            End If
                        End If

                        '(2.4) Get Reference Ranges defined for the NEW CALC Test in FACTORY DB
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myCalcTestUpdateDAO.GetRefRangesInFactoryDB(pDBConnection, newTest.CalcTestID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myNewRefRanges = DirectCast(myGlobalDataTO.SetDatos, TestRefRangesDS)
                            End If
                        End If

                        '(2.5) Save the NEW CALC Test in CUSTOMER DB 
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myCalcTestsDelegate.Add(pDBConnection, myNewTestDS, myNewFormulaDS, myNewRefRanges)
                        End If

                        '(2.6) Add a row in the global DS containing all changes in Customer DB due to the Update Version Process (sub-table AddedElements) 
                        '      for the new Factory CALC Test
                        If (Not myGlobalDataTO.HasError) Then
                            myUpdateVersionAddedElementsRow = pUpdateVersionChangesList.AddedElements.NewAddedElementsRow
                            myUpdateVersionAddedElementsRow.ElementType = "CALC"
                            myUpdateVersionAddedElementsRow.ElementName = myNewTestDS.tparCalculatedTests.First.CalcTestLongName & " (" & myNewTestDS.tparCalculatedTests.First.CalcTestName & ")"
                            If (myNewTestDS.tparCalculatedTests.First.UniqueSampleType) Then myUpdateVersionAddedElementsRow.SampleType = myNewTestDS.tparCalculatedTests.First.SampleType
                            pUpdateVersionChangesList.AddedElements.AddAddedElementsRow(myUpdateVersionAddedElementsRow)
                            pUpdateVersionChangesList.AddedElements.AcceptChanges()
                        End If

                        'If an error has been raised, then the process is finished
                        If (myGlobalDataTO.HasError) Then Exit For
                    Next
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("CALC Test Update Error", "CalculatedTestUpdateData.CreateNEWCALCTests", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type: STD, CALC, ISE or OFFS</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type code</param>
        ''' <returns>GlobalDataTO containing an String with TestName followed by the informed SampleType code between brackets</returns>
        ''' <remarks>
        ''' Created by: SA 15/10/2014 - BA-1944 (SubTask BA-2017)
        ''' </remarks>
        Private Function GetTestNameForFormula(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, ByVal pTestID As Integer, _
                                               ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pTestType = "STD") Then
                    Dim mySTDTestDS As New TestsDS
                    Dim myTestsDelegate As New TestsDelegate

                    myGlobalDataTO = myTestsDelegate.Read(pDBConnection, pTestID)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        mySTDTestDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)

                        If (mySTDTestDS.tparTests.Rows.Count > 0) Then
                            myGlobalDataTO.SetDatos = mySTDTestDS.tparTests.First.TestName & " [" & pSampleType & "]"
                            myGlobalDataTO.HasError = False
                        Else
                            'This case is not possible...
                            myGlobalDataTO.HasError = True
                        End If
                    End If

                ElseIf (pTestType = "CALC") Then
                    Dim myCALCTestDS As New CalculatedTestsDS
                    Dim myCALCTestsDelegate As New CalculatedTestsDelegate

                    myGlobalDataTO = myCALCTestsDelegate.GetCalcTest(pDBConnection, pTestID)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myCALCTestDS = DirectCast(myGlobalDataTO.SetDatos, CalculatedTestsDS)

                        If (myCALCTestDS.tparCalculatedTests.Rows.Count > 0) Then
                            myGlobalDataTO.SetDatos = myCALCTestDS.tparCalculatedTests.First.CalcTestLongName & " [" & pSampleType & "]"
                            myGlobalDataTO.HasError = False
                        Else
                            'This case is not possible...
                            myGlobalDataTO.HasError = True
                        End If
                    End If

                ElseIf (pTestType = "ISE") Then
                    Dim myISETestDS As New ISETestsDS
                    Dim myISETestsDelegate As New ISETestsDelegate

                    myGlobalDataTO = myISETestsDelegate.Read(pDBConnection, pTestID)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myISETestDS = DirectCast(myGlobalDataTO.SetDatos, ISETestsDS)

                        If (myISETestDS.tparISETests.Rows.Count > 0) Then
                            myGlobalDataTO.SetDatos = myISETestDS.tparISETests.First.Name & " [" & pSampleType & "]"
                            myGlobalDataTO.HasError = False
                        Else
                            'This case is not possible...
                            myGlobalDataTO.HasError = True
                        End If
                    End If

                ElseIf (pTestType = "OFFS") Then
                    Dim myOFFSTestDS As New OffSystemTestsDS
                    Dim myOFFSTestsDelegate As New OffSystemTestsDelegate

                    myGlobalDataTO = myOFFSTestsDelegate.Read(pDBConnection, pTestID)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myOFFSTestDS = DirectCast(myGlobalDataTO.SetDatos, OffSystemTestsDS)

                        If (myOFFSTestDS.tparOffSystemTests.Rows.Count > 0) Then
                            myGlobalDataTO.SetDatos = myOFFSTestDS.tparOffSystemTests.First.Name & " [" & pSampleType & "]"
                            myGlobalDataTO.HasError = False
                        Else
                            'This case is not possible...
                            myGlobalDataTO.HasError = True
                        End If
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("CALC Test Update Error", "CalculatedTestUpdateData.GetTestNameForFormula", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        Private Function GetCalcTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pFactoryCalcTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myCalcTestUpdateDAO As New CalculatedTestUpdateDAO
                Dim myCalcTestsDelegate As New CalculatedTestsDelegate

                'Search in FACTORY DB the BiosystemsID for the informed CalcTestID
                myGlobalDataTO = myCalcTestUpdateDAO.GetDataInFactoryDB(pDBConnection, pFactoryCalcTestID, False)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then


                    'Search in CUSTOMER DB the CalcTestID for the specified BiosystemsID
                    ' myGlobalDataTO = myCalcTestsDelegate.GetCalcTest(pDBConnection)
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("CALC Test Update Error", "CalculatedTestUpdateData.GetCalcTestID", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region
    End Class
End Namespace

