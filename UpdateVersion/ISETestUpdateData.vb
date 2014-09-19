Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.BL.UpdateVersion
    Public Class ISETestUpdateData
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
            'Return DirectCast(pMyDataSet, ISETestsDS).tparISETests
            Return pMyDataSet.Tables(0)
        End Function

#End Region
        ''' <summary>
        ''' Contains all related Data in a ISE Test
        ''' </summary>
        ''' <remarks>
        ''' Created By: DL - 05/02/2013
        ''' </remarks>
        Protected Structure DataInDB
            Public myISETestsDS As ISETestsDS
            Public myRefRangsDS As TestRefRangesDS

            ' ''' <summary>
            ' ''' Fill sie Test Data Set in structure, searching by ise Test Name
            ' ''' </summary>
            ' ''' <param name="pDBConnection"></param>
            ' ''' <param name="pISETestName"></param>
            ' ''' <returns></returns>
            ' ''' <remarks>
            ' ''' Created By: DL - 06/02/2013
            ' ''' </remarks>
            'Public Function SearchISETestByName(pDBConnection As SqlClient.SqlConnection,
            '                                    pISETestName As String,
            '                                    Optional factoryDB As Boolean = False) As Boolean

            '    Dim dbConnection As SqlClient.SqlConnection = Nothing
            '    myISETestsDS = New ISETestsDS

            '    Try
            '        Dim resultData As GlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
            '        If resultData.HasError Then Return False
            '        If resultData.SetDatos Is Nothing Then Return False

            '        dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
            '        If dbConnection Is Nothing Then Return False

            '        Dim dbName As String = ""
            '        If factoryDB Then dbName = GlobalBase.TemporalDBName

            '        Dim mytparISETests As New tparISETestsDAO
            '        resultData = mytparISETests.ReadByName(dbConnection, pISETestName, True, pDataBaseName:=dbName)

            '        If resultData.HasError Then Return False
            '        If resultData.SetDatos Is Nothing Then Return False

            '        'load data to structure member
            '        MyClass.myISETestsDS = DirectCast(resultData.SetDatos, ISETestsDS)
            '        Return True

            '    Catch ex As Exception
            '        Dim myLogAcciones As New ApplicationLogManager()
            '        myLogAcciones.CreateLogActivity(ex.Message, "ISETestUpdateData.SearchISETestByName", EventLogEntryType.Error, False)
            '    Finally
            '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            '    End Try
            '    Return False
            'End Function

            ''' <summary>
            ''' Fill sie Test Data Set in structure, searching by ise Test Name
            ''' </summary>
            ''' <param name="pDBConnection"></param>
            ''' <param name="pISETestId"></param>
            ''' <returns></returns>
            ''' <remarks>
            ''' Created By: SG 14/02/2013 - Bug #1134
            ''' </remarks>
            Public Function SearchISETestById(pDBConnection As SqlClient.SqlConnection,
                                                pISETestId As Integer,
                                                Optional factoryDB As Boolean = False) As Boolean

                Dim dbConnection As SqlClient.SqlConnection = Nothing
                myISETestsDS = New ISETestsDS

                Try
                    Dim resultData As GlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                    If resultData.HasError Then Return False
                    If resultData.SetDatos Is Nothing Then Return False

                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If dbConnection Is Nothing Then Return False

                    Dim dbName As String = ""
                    If factoryDB Then dbName = GlobalBase.TemporalDBName

                    Dim mytparISETests As New tparISETestsDAO
                    resultData = mytparISETests.Read(dbConnection, pISETestId, factoryDB)

                    If resultData.HasError Then Return False
                    If resultData.SetDatos Is Nothing Then Return False

                    'load data to structure member
                    MyClass.myISETestsDS = DirectCast(resultData.SetDatos, ISETestsDS)
                    Return True

                Catch ex As Exception
                    Dim myLogAcciones As New ApplicationLogManager()
                    myLogAcciones.CreateLogActivity(ex.Message, "ISETestUpdateData.SearchISETestById", EventLogEntryType.Error, False)
                Finally
                    If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
                End Try
                Return False
            End Function

            ''' <summary>
            ''' Fill TestRefRangesDS in structure, searching by ise Test identifier
            ''' </summary>
            ''' <param name="pDBConnection"></param>
            ''' <param name="pISETestId"></param>
            ''' <returns></returns>
            ''' <remarks>
            ''' Created By: DL - 06/02/2013
            ''' </remarks>
            Public Function SearchRefRangesByISETestId(pDBConnection As SqlClient.SqlConnection,
                                                        pISETestId As Integer,
                                                        Optional factoryDB As Boolean = False) As Boolean

                Dim dbConnection As SqlClient.SqlConnection = Nothing

                myRefRangsDS = New TestRefRangesDS()
                Try
                    Dim resultData As GlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                    If resultData.HasError Then Return False
                    If resultData.SetDatos Is Nothing Then Return False

                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If dbConnection Is Nothing Then Return False

                    Dim dbName As String = ""
                    If factoryDB Then dbName = GlobalBase.TemporalDBName

                    Dim myDAO As New tparTestRefRangesDAO
                    resultData = myDAO.ReadByTestID(dbConnection, pISETestId, , , "ISE", pDataBaseName:=dbName)
                    If resultData.HasError Then Return False
                    If resultData.SetDatos Is Nothing Then Return False

                    myRefRangsDS = DirectCast(resultData.SetDatos, TestRefRangesDS)
                    Return True

                Catch ex As Exception
                    Dim myLogAcciones As New ApplicationLogManager()
                    myLogAcciones.CreateLogActivity(ex.Message, "ISETestUpdateData.SearchRefRangesByISETestId", EventLogEntryType.Error, False)
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
            ''' Created by: JB - 30/01/2013
            ''' </remarks>
            Public Function IsDataEmpty() As Boolean
                Return myISETestsDS Is Nothing OrElse myISETestsDS.Tables.Count = 0 OrElse myISETestsDS.tparISETests.Rows.Count = 0
            End Function

        End Structure

        ''' <summary>
        ''' Gets the structured data in a dataBase, from the result of comparation of DataBases
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pItemRow">Row of a result of comparation of Client and Factory DataBases</param>
        ''' <param name="factoryDB">Determine which databases use to get data: Factory DB or Client DB</param>
        ''' <returns>
        ''' A GlobalDataTO with a DataInDB structure inside
        ''' </returns>
        ''' <remarks>
        ''' Created by: DL 06/02/2013
        ''' Modified by: SG 14/02/2013 Bug #1134
        ''' </remarks>
        Private Function GetDataInDB(pDBConnection As SqlClient.SqlConnection,
                                     pItemRow As DataRow,
                                     factoryDB As Boolean) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim dataDB As New DataInDB()
                Dim bRet As Boolean

                'SG 14/02/2013 Bug #1134
                'Dim iseTestName As String = pItemRow(0).ToString()
                'If String.IsNullOrEmpty(iseTestName) Then Return myGlobalDataTO

                
                Dim iseTestId As Integer = CInt(pItemRow(0))
                If iseTestId <= 0 Then Return myGlobalDataTO


                'bRet = dataDB.SearchISETestByName(pDBConnection, iseTestName, factoryDB)SG 14/02/2013 Bug #1134
                bRet = dataDB.SearchISETestById(pDBConnection, iseTestId, factoryDB)
                If Not bRet OrElse dataDB.myISETestsDS.tparISETests.Rows.Count = 0 Then Return myGlobalDataTO

                'Dim iseTestId As Integer = DirectCast(dataDB.myISETestsDS.tparISETests.Rows(0), ISETestsDS.tparISETestsRow).ISETestID

                bRet = dataDB.SearchRefRangesByISETestId(pDBConnection, iseTestId, factoryDB)
                If Not bRet Then Return myGlobalDataTO

                myGlobalDataTO.SetDatos = dataDB

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("ISE Test Update Error.", "ISETestUpdateData.GetDataInDB", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function


#End Region

        Protected Overrides Function DoCreateNewElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            'No action is needed
            Return New GlobalDataTO
        End Function

        Protected Overrides Function DoIgnoreElementRemoving(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            'No action is needed
            Return New GlobalDataTO
        End Function

        Protected Overrides Function DoIgnoreElementUpdating(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            'No action is needed
            Return New GlobalDataTO
        End Function

        Protected Overrides Function DoRemoveFactoryElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            'No action is needed
            Return New GlobalDataTO
        End Function

        Protected Overrides Function DoRemoveUserElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            'No action is needed
            Return New GlobalDataTO
        End Function

        ''' <summary>
        ''' Update preloaded ISE Test
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB"></param>
        ''' <param name="pDataInLocalDB"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:   DL 07/02/2013
        ''' Modified by: SGM 14/02/2013 Bug #1134
        '''               XB 19/09/2014 - Preserve the now editable by User field Test ISE Name - BA-1865
        ''' </remarks>
        Protected Overrides Function DoUpdateFactoryDefinedElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'SGM 14/02/2013 Bug #1134
                Dim myIseTestClient As New ISETestsDS
                myIseTestClient = DirectCast(pDataInLocalDB, DataInDB).myISETestsDS

                Dim myIseTestFactory As New ISETestsDS
                myIseTestFactory = DirectCast(pDataInFactoryDB, DataInDB).myISETestsDS

                'Create ISETestSamples DataSets needed for the Save method
                Dim myISETestSampleDelegate As New ISETestSamplesDelegate
                Dim ISETestsSamplesDS As New ISETestSamplesDS

                'TR 18/02/2013 -Send the connection object
                myGlobalDataTO = myISETestSampleDelegate.GetListByISETestID(pDBConnection, myIseTestFactory.tparISETests.First.ISETestID, myIseTestFactory.tparISETests.First.SampleType)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    ISETestsSamplesDS = CType(myGlobalDataTO.SetDatos, ISETestSamplesDS)
                End If

                'Create refranges Datasets needed for the Save method
                Dim newRefRangesDS As New TestRefRangesDS
                Dim updatedRefRangesDS As New TestRefRangesDS
                Dim deletedRefRangesDS As New TestRefRangesDS

                Dim myFactoryRefRanges As New TestRefRangesDS
                myFactoryRefRanges = DirectCast(pDataInFactoryDB, DataInDB).myRefRangsDS

                Dim myLocalRefRanges As New TestRefRangesDS
                myLocalRefRanges = DirectCast(pDataInLocalDB, DataInDB).myRefRangsDS

                If myFactoryRefRanges.tparTestRefRanges.Count > myLocalRefRanges.tparTestRefRanges.Count Then
                    newRefRangesDS = myFactoryRefRanges

                ElseIf myFactoryRefRanges.tparTestRefRanges.Count = myLocalRefRanges.tparTestRefRanges.Count AndAlso myLocalRefRanges.tparTestRefRanges.Count > 0 Then
                    updatedRefRangesDS = myFactoryRefRanges
                End If

                ' XB 19/09/2014 - BA-1865
                ''preserve "Enabled" field's value for Lithium
                ''SGM 14/02/2013 Bug #1134
                'Dim LiEnabled As Boolean
                'If myIseTestClient.tparISETests.First.ISETestID = 4 Then
                '    LiEnabled = myIseTestClient.tparISETests.First.Enabled
                '    myIseTestFactory.BeginInit()
                '    Dim myRow As ISETestsDS.tparISETestsRow = CType(myIseTestFactory.tparISETests.Rows(0), ISETestsDS.tparISETestsRow)
                '    myRow.Enabled = LiEnabled
                '    myIseTestFactory.EndInit()
                '    myIseTestFactory.AcceptChanges()

                'End If
                ''end  SGM 14/02/2013 Bug #1134

                'preserve field's value defined by User
                Dim LiEnabled As Boolean
                Dim TestName As String
                myIseTestFactory.BeginInit()
                Dim myRow As ISETestsDS.tparISETestsRow = CType(myIseTestFactory.tparISETests.Rows(0), ISETestsDS.tparISETestsRow)
                'preserve "Name" field's value
                TestName = myIseTestClient.tparISETests.First.Name
                myRow.Name = TestName
                If myIseTestClient.tparISETests.First.ISETestID = 4 Then
                    'preserve "Enabled" field's value for Lithium
                    LiEnabled = myIseTestClient.tparISETests.First.Enabled
                    myRow.Enabled = LiEnabled
                End If
                myIseTestFactory.EndInit()
                myIseTestFactory.AcceptChanges()
                ' XB 19/09/2014 - BA-1865

                Dim myISETestsDelegate As New ISETestsDelegate()
                myGlobalDataTO = myISETestsDelegate.SaveISETestNEW(pDBConnection,
                                                                   myIseTestFactory,
                                                                   ISETestsSamplesDS,
                                                                   newRefRangesDS,
                                                                   updatedRefRangesDS,
                                                                   deletedRefRangesDS,
                                                                   New TestSamplesMultirulesDS,
                                                                   New TestControlsDS,
                                                                   Nothing)


            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("ISE Tests Update Error.", "ISETestUpdateData.DoUpdateFactoryDefinedElement", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB"></param>
        ''' <param name="pDataInLocalDB"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: DL 07/02/2013
        ''' </remarks>
        Protected Overrides Function DoUpdateUserDefinedElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            'No action is needed
            Return New GlobalDataTO
        End Function

        Protected Overrides Function GetAffectedItemsFromFactoryRemoves(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Return New GlobalDataTO
        End Function

        ''' <summary>
        ''' Get Ise Test distinct between Client and Factory
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function GetAffectedItemsFromFactoryUpdates(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myLogAcciones As New ApplicationLogManager()
            Dim myGlobalDataTO As New GlobalDataTO

            Try

                myGlobalDataTO = ISETestUpdateDAO.GetISETestDistinctInClient(pDBConnection)

            Catch ex As Exception
                myLogAcciones.CreateLogActivity("ISE Tests Update Error.", "ISETestUpdateData.GetAffectedItemsFromFactoryUpdates", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Get data from Factory DB and fill Row 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pItemRow"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: DL 07/02/2013
        ''' </remarks>
        Protected Overrides Function GetDataInFactoryDB(pDBConnection As SqlClient.SqlConnection, pItemRow As DataRow) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Return GetDataInDB(pDBConnection, pItemRow, True)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("ISE Tests Update Error.", "ISETestUpdateData.GetDataInFactoryDB", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get data from LocalDB and fill Row
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pItemRow"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: DL 07/02/2013
        ''' </remarks>
        Protected Overrides Function GetDataInLocalDB(pDBConnection As SqlClient.SqlConnection, pItemRow As DataRow) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Return GetDataInDB(pDBConnection, pItemRow, False)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("ISE Test Update Error.", "ISETestUpdateData.GetDataInLocalDB", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        Protected Overrides Function IsDataEmpty(pDataInDB As Object) As Boolean

            Return pDataInDB Is Nothing OrElse DirectCast(pDataInDB, DataInDB).IsDataEmpty()
        End Function


        Protected Overrides Function IsFactoryDefinedItem(pDataInDB As Object, pDBConnection As SqlClient.SqlConnection) As Boolean
            'Try
            '    If (pDataInDB Is Nothing) Then Return False
            '    Dim iseTestData As DataInDB = DirectCast(pDataInDB, DataInDB)
            '    If (iseTestData.myISETestsDS Is Nothing) Then Return False
            '    If (iseTestData.myISETestsDS.tparISETests.Rows.Count = 0) Then Return False

            '    'Dim row As ISETestsDS.tparISETestsRow
            '    'row = DirectCast(iseTestData.myISETestsDS.tparISETests.Rows(0), ISETestsDS.tparISETestsRow)
            '    'Return False

            'Catch ex As Exception
            '    Dim myLogAcciones As New ApplicationLogManager()
            '    myLogAcciones.CreateLogActivity("ISE Test Update Error.", "ISETestUpdateData.IsFactoryDefinedItem", EventLogEntryType.Error, False)
            'End Try

            Return True

        End Function



    End Class

End Namespace