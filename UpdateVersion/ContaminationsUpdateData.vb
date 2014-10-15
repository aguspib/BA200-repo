Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.TO


Namespace Biosystems.Ax00.BL.UpdateVersion

    Public Class ContaminationsUpdateData
        Inherits UpdateElementParent


#Region "PRIVATE"
        ''' According the informed Contamination Type, delete all Contaminations defined (CUVETTES case) or all Contaminations
        ''' in which the specified ReagentID acts as Contaminator.  After that, create all new defined Contaminations
        ''' COPY FROM ContaminationsDelegate.SaveContaminations
        ''' Modified to work With update PROCESS. (UPDATE PROCESS ONLY)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pContaminationType">Type of Contamination</param>
        ''' <param name="pContaminationsDS">Typed DataSet ContaminationsDS containing the Contaminations to save</param>
        ''' <param name="pReagentContaminatorID">Optional parameter, needed when the Contamination Type is R1 or R2</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 12/02/2013
        ''' </remarks>
        Public Function SaveContaminations(ByVal pDBConnection As SqlClient.SqlConnection, pContaminationsDS As ContaminationsDS) _
                                           As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myContaminationsUpdateDAO As New ContaminationsUpdateDAO
                        If pContaminationsDS.tparContaminations.Count > 0 Then
                            Select Case (pContaminationsDS.tparContaminations(0).ContaminationType)
                                Case "R1", "R2"
                                    'Delete all  Contaminations defined for the informed Contaminator Reagent
                                    resultData = myContaminationsUpdateDAO.Delete(dbConnection, pContaminationsDS.tparContaminations(0).ReagentContaminatorID, _
                                                                                          pContaminationsDS.tparContaminations(0).ReagentContaminatedID, _
                                                                                          pContaminationsDS.tparContaminations(0).ContaminationType)

                                Case "CUVETTES"
                                    'New screen has to delete Cuvette Contamination by TestID
                                    resultData = myContaminationsUpdateDAO.DeleteCuvettes(dbConnection, pContaminationsDS.tparContaminations(0).TestContaminaCuvetteID, _
                                                                                    pContaminationsDS.tparContaminations(0).ContaminationType)
                            End Select

                        End If
                        
                        If (Not resultData.HasError) Then
                            'Add all the defined Contaminations on Client DB.
                            Dim myContaminationsDAO As New tparContaminationsDAO
                            For Each contaminationRow As ContaminationsDS.tparContaminationsRow In pContaminationsDS.tparContaminations.Rows
                                resultData = myContaminationsDAO.Create(dbConnection, contaminationRow)
                                If (resultData.HasError) Then Exit For
                            Next
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
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ContaminationsDelegate.SaveContaminations", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region " Internal Data "
#Region " DataTable "
        Protected Overrides Function GetDataTable(pMyDataSet As DataSet) As DataTable
            Return DirectCast(pMyDataSet, ContaminationsDS).tparContaminations
        End Function
#End Region

        ''' <summary>
        ''' Get data from Factory DB and fill Row 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pItemRow"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: TR 11/02/2013
        ''' </remarks>
        Protected Overrides Function GetDataInFactoryDB(pDBConnection As SqlClient.SqlConnection,
                                                        pItemRow As DataRow) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim contamRow As ContaminationsDS.tparContaminationsRow = DirectCast(pItemRow, ContaminationsDS.tparContaminationsRow)
                Dim myContaminationDS As New ContaminationsDS
                If Not contamRow Is Nothing Then
                    Dim myContaminationsUpdateDAO As New ContaminationsUpdateDAO
                    'Get the data from factory db
                    myGlobalDataTO = myContaminationsUpdateDAO.GetContaminationsByContaminatonID(pDBConnection, contamRow.ContaminationID)
                    If Not myGlobalDataTO.HasError Then
                        myContaminationDS = DirectCast(myGlobalDataTO.SetDatos, ContaminationsDS)
                        If myContaminationDS.tparContaminations.Count > 0 Then
                            If myContaminationDS.tparContaminations(0).ContaminationType = "CUVETTES" Then
                                myContaminationDS.tparContaminations(0).TestName = contamRow.TestName
                            Else
                                'Set the contaminator ReagentName
                                myContaminationDS.tparContaminations(0).ContaminatorName = contamRow.ContaminatorName
                            End If


                            
                        End If
                    End If
                End If
                myGlobalDataTO.SetDatos = myContaminationDS
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Contaminations Update Error.", "ContaminationsUpdateData.GetDataInFactoryDB", EventLogEntryType.Error, False)
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
        ''' Created by: TR - 11/02/2013
        ''' </remarks>
        Protected Overrides Function GetDataInLocalDB(pDBConnection As SqlClient.SqlConnection,
                                                      pItemRow As DataRow) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim ContaminationRow As ContaminationsDS.tparContaminationsRow = DirectCast(pItemRow, ContaminationsDS.tparContaminationsRow)
                If Not ContaminationRow Is Nothing Then
                    Dim myContaminationDs As New ContaminationsDS
                    Dim myContaminationsDelegate As New ContaminationsDelegate
                    If ContaminationRow.ContaminationType = "CUVETTES" Then
                        'Get the values by Test Name.
                        myGlobalDataTO = myContaminationsDelegate.ReadByTestName(pDBConnection, ContaminationRow.TestName)

                    Else
                        'Get the values by the Contaminator reagents Name
                        myGlobalDataTO = myContaminationsDelegate.ReadByReagentsName(pDBConnection, ContaminationRow.ContaminatorName, _
                                                                                     ContaminationRow.ContaminatedName)
                    End If


                    If Not myGlobalDataTO.HasError Then
                        myContaminationDs = DirectCast(myGlobalDataTO.SetDatos, ContaminationsDS)
                        For Each contaRow As ContaminationsDS.tparContaminationsRow In myContaminationDs.tparContaminations.Rows

                            If contaRow.ContaminationType = "CUVETTES" Then
                                'Set the TestName
                                contaRow.TestName = ContaminationRow.TestName
                            Else
                                If ContaminationRow.ContaminatedName = contaRow.ContaminatedName Then
                                    'Set the contaminator ReagentName
                                    contaRow.ContaminatorName = ContaminationRow.ContaminatorName
                                Else
                                    contaRow.Delete()
                                End If
                            End If


                        Next
                        myContaminationDs.AcceptChanges()
                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Contaminations Update Error.", "ContaminationsUpdateData.GetDataInLocalDB", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Is item defined by user?
        ''' </summary>
        ''' <param name="pDataInDB"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: TR 11/02/2013
        ''' modified by: TR 08/05/2013 *-Implement the connection is require to have acces to DAOS.
        ''' </remarks>
        Protected Overrides Function IsFactoryDefinedItem(pDataInDB As Object, pDBConnection As SqlClient.SqlConnection) As Boolean
            Dim myResult As Boolean = False
            Try
                Dim myLocalDB As New ContaminationsDS
                Dim myContaminationsUpdateDAO As New ContaminationsUpdateDAO
                If DirectCast(pDataInDB, ContaminationsDS).tparContaminations.Count > 0 Then
                    If DirectCast(pDataInDB, ContaminationsDS).tparContaminations(0).ContaminationType = "CUVETTES" Then
                        'Validate if the test is factiry value 
                        myResult = myContaminationsUpdateDAO.IsFactoryTest(pDBConnection, DirectCast(pDataInDB, ContaminationsDS).tparContaminations(0).TestID)
                    Else
                        'Get the Test to validate if reagent belong to factory defined test
                        myResult = myContaminationsUpdateDAO.IsFactoryReagent(pDBConnection, DirectCast(pDataInDB, ContaminationsDS).tparContaminations(0).ReagentContaminatorID)
                    End If

                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Contaminations Update Error.", "ContaminationsUpdateData.IsFactoryDefinedItem", EventLogEntryType.Error, False)
            End Try

            Return myResult
        End Function
       
#End Region


        Protected Overrides Function DoCreateNewElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Create new Factory element into ClientDB.
                Dim myContaminationsDelegate As New ContaminationsDelegate
                Dim myFactoryDataDS As New ContaminationsDS
                myFactoryDataDS = DirectCast(pDataInFactoryDB, ContaminationsDS)
                Dim myTestDelegate As New TestsDelegate


                If myFactoryDataDS.tparContaminations.Count > 0 Then
                    If myFactoryDataDS.tparContaminations(0).ContaminationType = "CUVETTES" Then
                        'Need to get the local TestID value.
                        myGlobalDataTO = myTestDelegate.ExistsTestName(pDBConnection, myFactoryDataDS.tparContaminations(0).TestName)
                        If Not myGlobalDataTO.HasError Then
                            Dim myTestDS As New TestsDS
                            myTestDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)
                            If myTestDS.tparTests.Count > 0 Then
                                'set the testid 
                                myFactoryDataDS.tparContaminations(0).TestContaminaCuvetteID = myTestDS.tparTests(0).TestID
                            End If
                        End If
                    Else
                        'Need to set the client reagents ID search by reagent name on local client and set the ID into myFactory DATA
                        'Get the locals Reagents ID.
                        Dim myReagentsDelegate As New ReagentsDelegate
                        'Get the contaminator ID
                        myGlobalDataTO = myReagentsDelegate.GetReagentByReagentName(pDBConnection, myFactoryDataDS.tparContaminations(0).ContaminatorName)
                        If Not myGlobalDataTO.HasError Then
                            If DirectCast(myGlobalDataTO.SetDatos, ReagentsDS).tparReagents.Count > 0 Then
                                'Set the local ID to contaminator
                                myFactoryDataDS.tparContaminations(0).ReagentContaminatorID = _
                                                            DirectCast(myGlobalDataTO.SetDatos, ReagentsDS).tparReagents(0).ReagentID
                            End If

                        End If

                        'Get the contaminated ID
                        If Not myGlobalDataTO.HasError Then
                            myGlobalDataTO = myReagentsDelegate.GetReagentByReagentName(pDBConnection, myFactoryDataDS.tparContaminations(0).ContaminatedName)
                            If Not myGlobalDataTO.HasError Then
                                If DirectCast(myGlobalDataTO.SetDatos, ReagentsDS).tparReagents.Count > 0 Then
                                    'Set the local ID to contaminated ID
                                    myFactoryDataDS.tparContaminations(0).ReagentContaminatedID = _
                                                                DirectCast(myGlobalDataTO.SetDatos, ReagentsDS).tparReagents(0).ReagentID
                                End If
                            End If
                        End If
                    End If

                    'Save contamination.
                    If Not myGlobalDataTO.HasError Then
                        myGlobalDataTO = SaveContaminations(pDBConnection, myFactoryDataDS)
                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Contaminations Update Error.", "ContaminationsUpdateDataDoCreateNewElement", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO
        End Function

        Protected Overrides Function DoIgnoreElementRemoving(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Return New GlobalDataTO
        End Function

        Protected Overrides Function DoIgnoreElementUpdating(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Return New GlobalDataTO
        End Function

        Protected Overrides Function DoRemoveFactoryElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Return New GlobalDataTO
        End Function

        Protected Overrides Function DoRemoveUserElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Return New GlobalDataTO
        End Function

        ''' <summary>
        ''' Update factory elements (contaminations)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB"></param>
        ''' <param name="pDataInLocalDB"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 12/02/2012
        ''' </remarks>
        Protected Overrides Function DoUpdateFactoryDefinedElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myContaminationsDelegate As New ContaminationsDelegate
                Dim myLocalDataDS As New ContaminationsDS
                Dim myFactoryDataDS As New ContaminationsDS

                myLocalDataDS = DirectCast(pDataInLocalDB, ContaminationsDS)
                myFactoryDataDS = DirectCast(pDataInFactoryDB, ContaminationsDS)

                If myFactoryDataDS.tparContaminations.Count > 0 Then
                    For Each LocalContaminationRow As ContaminationsDS.tparContaminationsRow In myLocalDataDS.tparContaminations.Rows
                        'If washing defined on Factory then Update values
                        If Not myFactoryDataDS.tparContaminations(0).IsWashingSolutionR1Null Then
                            LocalContaminationRow.WashingSolutionR1 = myFactoryDataDS.tparContaminations(0).WashingSolutionR1
                        End If
                        If Not myFactoryDataDS.tparContaminations(0).IsWashingSolutionR2Null Then
                            LocalContaminationRow.WashingSolutionR2 = myFactoryDataDS.tparContaminations(0).WashingSolutionR2
                        End If

                        If LocalContaminationRow.ContaminationType = "CUVETTES" Then
                            'Send the test ID
                            'myGlobalDataTO = myContaminationsDelegate.SaveContaminations(pDBConnection, LocalContaminationRow.ContaminationType, myLocalDataDS, LocalContaminationRow.TestContaminaCuvetteID)
                            myGlobalDataTO = SaveContaminations(pDBConnection, myLocalDataDS)
                        Else
                            'Send the reagent ID
                            'myGlobalDataTO = myContaminationsDelegate.SaveContaminations(pDBConnection, LocalContaminationRow.ContaminationType, myLocalDataDS, LocalContaminationRow.ReagentContaminatorID)
                            myGlobalDataTO = SaveContaminations(pDBConnection, myLocalDataDS)
                        End If

                        If myGlobalDataTO.HasError Then Exit For
                    Next
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "ContaminationsUpdateData.DoUpdateFactoryDefinedElement", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        Protected Overrides Function DoUpdateUserDefinedElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Return New GlobalDataTO
        End Function

        Protected Overrides Function GetAffectedItemsFromFactoryRemoves(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            'Get the contaminations defined in client but not in factory in order to remove them
            'Not apply for contaminations
            Return New GlobalDataTO
        End Function

        Protected Overrides Function GetAffectedItemsFromFactoryUpdates(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myDataSetR1R2DS As New DataSet
                Dim myDataSetCuvettesDS As New DataSet
                Dim myContaminationsUpdateDAO As New ContaminationsUpdateDAO

                'Get the contamination types R1 or R2
                myGlobalDataTO = myContaminationsUpdateDAO.GetAffectedItemsFromFactory(pDBConnection, False)

                'Set the values to my DS
                If Not myGlobalDataTO.HasError Then

                    myDataSetR1R2DS = DirectCast(myGlobalDataTO.SetDatos, DataSet)
                    'Send All data into ContaminationDS 
                    Dim myContaminationDS As New ContaminationsDS
                    Dim myContaminationsRow As ContaminationsDS.tparContaminationsRow
                    For Each myRow As DataRow In myDataSetR1R2DS.Tables(0).Rows
                        myContaminationsRow = myContaminationDS.tparContaminations.NewtparContaminationsRow()
                        myContaminationsRow.ContaminationID = CInt(myRow("ContaminationID").ToString())
                        myContaminationsRow.ContaminatorName = myRow("ContaminatorName").ToString()
                        myContaminationsRow.ContaminationType = myRow("ContaminationType").ToString()
                        myContaminationsRow.ReagentContaminatorID = CInt(myRow("ReagentContaminatorID").ToString())
                        myContaminationsRow.ContaminatedName = myRow("ContaminatedName").ToString()
                        myContaminationsRow.ReagentContaminatedID = CInt(myRow("ReagentContaminatedID").ToString())

                        If Not myRow("TestContaminaCuvetteID") Is DBNull.Value Then
                            myContaminationsRow.TestContaminaCuvetteID = CInt(myRow("TestContaminaCuvetteID").ToString())
                        End If

                        If Not myRow("WashingSolutionR1") Is DBNull.Value Then
                            myContaminationsRow.WashingSolutionR1 = myRow("WashingSolutionR1").ToString()
                        End If

                        If Not myRow("WashingSolutionR2") Is DBNull.Value Then
                            myContaminationsRow.WashingSolutionR2 = myRow("WashingSolutionR2").ToString()
                        End If
                        'Insert Row
                        myContaminationDS.tparContaminations.AddtparContaminationsRow(myContaminationsRow)
                    Next

                    If Not myGlobalDataTO.HasError Then
                        'GET cuvettes types
                        myGlobalDataTO = myContaminationsUpdateDAO.GetAffectedItemsFromFactory(pDBConnection, True)
                        If Not myGlobalDataTO.HasError Then
                            myDataSetCuvettesDS = DirectCast(myGlobalDataTO.SetDatos, DataSet)
                            For Each myRow As DataRow In myDataSetCuvettesDS.Tables(0).Rows
                                myContaminationsRow = myContaminationDS.tparContaminations.NewtparContaminationsRow()

                                myContaminationsRow.ContaminationID = CInt(myRow("ContaminationID").ToString())
                                myContaminationsRow.TestID = CInt(myRow("TestID").ToString())
                                myContaminationsRow.TestName = myRow("TestName").ToString()
                                myContaminationsRow.ContaminationType = myRow("ContaminationType").ToString()

                                If Not myRow("TestContaminaCuvetteID") Is DBNull.Value Then
                                    myContaminationsRow.TestContaminaCuvetteID = CInt(myRow("TestContaminaCuvetteID").ToString())
                                End If

                                If Not myRow("WashingSolutionR1") Is DBNull.Value Then
                                    myContaminationsRow.WashingSolutionR1 = myRow("WashingSolutionR1").ToString()
                                End If

                                If Not myRow("WashingSolutionR2") Is DBNull.Value Then
                                    myContaminationsRow.WashingSolutionR2 = myRow("WashingSolutionR2").ToString()
                                End If
                                'Insert Row
                                myContaminationDS.tparContaminations.AddtparContaminationsRow(myContaminationsRow)
                            Next
                        End If
                    End If

                    myGlobalDataTO.SetDatos = myContaminationDS
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "ContaminationsUpdateData.GetAffectedItemsFromFactoryUpdates", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO

        End Function


        ''' <summary>
        ''' Validate if table has data
        ''' </summary>
        ''' <param name="pDataInDB"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function IsDataEmpty(pDataInDB As Object) As Boolean
            Dim myResult As Boolean = False
            If DirectCast(pDataInDB, ContaminationsDS).tparContaminations.Count = 0 Then
                myResult = True
            End If
            Return myResult
        End Function


#Region "FUNCTIONS FOR NEW UPDATE VERSION PROCESS (NEW AND UPDATED FUNCTIONS)"
        ''' <summary>
        ''' Execute the process to search in FACTORY DB all changes in Test Contaminations and the update of the CUSTOMER DB with these changes
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 14/10/2014 - BA-1944 (SubTask BA-1986)
        ''' </remarks>
        Public Function ProcessCONTAMINATIONS(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                '(1) Process DELETED R1 Contaminations 
                myGlobalDataTO = ProcessDELContaminationsR1(pDBConnection, pUpdateVersionChangesList)

                '(2) Process DELETED CUVETTES Contaminations
                If (Not myGlobalDataTO.HasError) Then
                    myGlobalDataTO = ProcessDELContaminationsCUVETTES(pDBConnection, pUpdateVersionChangesList)
                End If

                '(3) Process NEW and UPDATED R1 Contaminations
                If (Not myGlobalDataTO.HasError) Then
                    myGlobalDataTO = ProcessNEWorUPDContaminationsR1(pDBConnection, pUpdateVersionChangesList)
                End If

                '(4) Process NEW and UPDATED CUVETTES Contaminations
                If (Not myGlobalDataTO.HasError) Then
                    myGlobalDataTO = ProcessNEWorUPDContaminationsCUVETTES(pDBConnection, pUpdateVersionChangesList)
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("STD Test Update Error", "ContaminationsUpdateData.ProcessCONTAMINATIONS", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Execute the process to search and delete from CUSTOMER DB all R1 Contaminations for preloaded STD Tests that do not exist in FACTORY DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 14/10/2014 BA-1944 (SubTask BA-1986)
        ''' </remarks>
        Private Function ProcessDELContaminationsR1(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myContaminatorReagentID As Integer = 0
                Dim myContaminatedReagentID As Integer = 0

                Dim myContaminatorTestName As String = String.Empty
                Dim myContaminatedTestName As String = String.Empty

                Dim myTestReagentsDS As New TestReagentsDS
                Dim myCustomerContaminationsR1 As New ContaminationsDS

                Dim myTestReagentsDelegate As New TestReagentsDelegate
                Dim myContaminationsDelegate As New ContaminationsDelegate
                Dim myContaminationsUpdateDAO As New ContaminationsUpdateDAO

                '(1) Search in CUSTOMER DB all R1 Contaminations defined for preloaded STD Tests that are not in FACTORY DB
                myGlobalDataTO = myContaminationsUpdateDAO.GetDELContaminationsR1(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myCustomerContaminationsR1 = DirectCast(myGlobalDataTO.SetDatos, ContaminationsDS)
                End If

                '(2) Process each R1 Contamination to delete 
                If (Not myGlobalDataTO.HasError) Then
                    For Each customerR1Contamination As ContaminationsDS.tparContaminationsRow In myCustomerContaminationsR1.tparContaminations
                        '(2.1) Get the identifier of the first Reagent for the Contaminator Test in CUSTOMER DB 
                        myGlobalDataTO = myTestReagentsDelegate.GetTestReagents(pDBConnection, customerR1Contamination.TestContaminatorID, 1)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myTestReagentsDS = DirectCast(myGlobalDataTO.SetDatos, TestReagentsDS)

                            If (myTestReagentsDS.tparTestReagents.Rows.Count > 0) Then
                                myContaminatorReagentID = myTestReagentsDS.tparTestReagents.First.ReagentID
                                myContaminatorTestName = myTestReagentsDS.tparTestReagents.First.TestName & " (" & myTestReagentsDS.tparTestReagents.First.ShortName & ") "
                            Else
                                'This case is not possible....
                                myGlobalDataTO.HasError = True
                            End If
                        End If

                        '(2.2) Get the identifier of the first Reagent for the Contaminated Test in CUSTOMER DB 
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myTestReagentsDelegate.GetTestReagents(pDBConnection, customerR1Contamination.TestContaminatedID, 1)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myTestReagentsDS = DirectCast(myGlobalDataTO.SetDatos, TestReagentsDS)

                                If (myTestReagentsDS.tparTestReagents.Rows.Count > 0) Then
                                    myContaminatedReagentID = myTestReagentsDS.tparTestReagents.First.ReagentID
                                    myContaminatedTestName = myTestReagentsDS.tparTestReagents.First.TestName & " (" & myTestReagentsDS.tparTestReagents.First.ShortName & ") "
                                Else
                                    'This case is not possible....
                                    myGlobalDataTO.HasError = True
                                End If
                            End If
                        End If

                        '(2.3) Delete the R1 Contamination from CUSTOMER DB
                        If (Not myGlobalDataTO.HasError) Then
                            'Add a new row in the list of Contaminations affected by Update Version Process
                            AddContaminationToChangesStructure(pUpdateVersionChangesList, "DELETE", "R1", myContaminatorTestName, myContaminatedTestName)

                            'Delete the R1 Contamination
                            myGlobalDataTO = myContaminationsDelegate.DeleteR1Contamination(pDBConnection, myContaminatorReagentID, myContaminatedReagentID)
                        End If

                        'If an error is raised, then the process finishes
                        If (myGlobalDataTO.HasError) Then Exit For
                    Next
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("STD Test Update Error", "ContaminationsUpdateData.ProcessDELContaminationsR1", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Execute the process to search and delete from CUSTOMER DB all CUVETTES Contaminations for preloaded STD Tests that do not exist in FACTORY DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 14/10/2014 BA-1944 (SubTask BA-1986)
        ''' </remarks>
        Private Function ProcessDELContaminationsCUVETTES(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myContaminatorReagentID As Integer = 0
                Dim myContaminatedReagentID As Integer = 0

                Dim myContaminatorTestName As String = String.Empty
                Dim myContaminatedTestName As String = String.Empty

                Dim myTestsDS As New TestsDS
                Dim myCustomerContaminationsCUVETTES As New ContaminationsDS

                Dim myTestsDelegate As New TestsDelegate
                Dim myContaminationsDelegate As New ContaminationsDelegate
                Dim myContaminationsUpdateDAO As New ContaminationsUpdateDAO

                '(1) Search in CUSTOMER DB all CUVETTES Contaminations defined for preloaded STD Tests that are not in FACTORY DB
                myGlobalDataTO = myContaminationsUpdateDAO.GetDELContaminationsCUVETTES(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myCustomerContaminationsCUVETTES = DirectCast(myGlobalDataTO.SetDatos, ContaminationsDS)
                End If

                '(2) Process each CUVETTES Contamination to delete 
                If (Not myGlobalDataTO.HasError) Then
                    For Each customerCUVETTESContamination As ContaminationsDS.tparContaminationsRow In myCustomerContaminationsCUVETTES.tparContaminations
                        '(2.1) Get the name of the Test Contaminator
                        myGlobalDataTO = myTestsDelegate.Read(pDBConnection, customerCUVETTESContamination.TestContaminaCuvetteID)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myTestsDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)

                            If (myTestsDS.tparTests.Rows.Count > 0) Then
                                'Get the name of the Contaminator Test
                                myContaminatorTestName = myTestsDS.tparTests.First.TestName & " (" & myTestsDS.tparTests.First.ShortName & ") "
                            Else
                                'This case is not possible
                                myGlobalDataTO.HasError = True
                            End If
                        End If

                        '(2.2) Delete the CUVETTES Contamination from CUSTOMER DB
                        If (Not myGlobalDataTO.HasError) Then
                            'Add a new row in the list of Contaminations affected by Update Version Process
                            AddContaminationToChangesStructure(pUpdateVersionChangesList, "DELETE", "CUVETTES", myContaminatorTestName, "--")

                            'Delete the CUVETTES Contamination
                            myGlobalDataTO = myContaminationsDelegate.DeleteCUVETTESContamination(pDBConnection, customerCUVETTESContamination.TestContaminaCuvetteID)
                        End If

                        'If an error is raised, then the process finishes
                        If (myGlobalDataTO.HasError) Then Exit For
                    Next
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("STD Test Update Error", "ContaminationsUpdateData.ProcessDELContaminationsR1", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Execute the process to search in FACTORY DB all R1 Contaminations that do not exist in CUSTOMER DB or that exist but with a different 
        ''' Washing Solution R1 and add/modify data in CUSTOMER DB (table tparContaminations)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 14/10/2014 BA-1944 (SubTask BA-1986)
        ''' </remarks>
        Private Function ProcessNEWorUPDContaminationsR1(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myContaminatorReagentID As Integer = 0
                Dim myContaminatedReagentID As Integer = 0

                Dim myWashSol1 As String = String.Empty
                Dim myContaminatorTestName As String = String.Empty
                Dim myContaminatedTestName As String = String.Empty

                Dim myTestReagentsDS As New TestReagentsDS
                Dim myFactoryContaminationsR1 As New ContaminationsDS
                Dim myCustomerContaminationsR1 As New ContaminationsDS

                Dim myTestReagentsDelegate As New TestReagentsDelegate
                Dim myContaminationsDelegate As New ContaminationsDelegate
                Dim myContaminationsUpdateDAO As New ContaminationsUpdateDAO

                Dim customerR1Contamination As New List(Of ContaminationsDS.tparContaminationsRow)

                '(1) Search in Factory DB all R1 Contaminations  defined for Preloaded STD TESTS that are not in the CUSTOMER DB or that 
                '    are in CUSTOMER DB but with different Washing Solution
                myGlobalDataTO = myContaminationsUpdateDAO.GetNEWorUPDContaminationsR1(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myFactoryContaminationsR1 = DirectCast(myGlobalDataTO.SetDatos, ContaminationsDS)
                End If

                '(2) Get all R1 Contaminations in CUSTOMER DB
                If (Not myGlobalDataTO.HasError) Then
                    myGlobalDataTO = myContaminationsDelegate.GetContaminationsByType(pDBConnection, "R1")
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myCustomerContaminationsR1 = DirectCast(myGlobalDataTO.SetDatos, ContaminationsDS)
                    End If
                End If

                '(3) Process each new/updated R1 Contamination in FACTORY DB
                If (Not myGlobalDataTO.HasError) Then
                    For Each factoryR1Contamination As ContaminationsDS.tparContaminationsRow In myFactoryContaminationsR1.tparContaminations
                        '(3.1) Get the identifier of the first Reagent for the Contaminator Test in CUSTOMER DB 
                        myGlobalDataTO = myTestReagentsDelegate.GetTestReagents(pDBConnection, factoryR1Contamination.TestContaminatorID, 1)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myTestReagentsDS = DirectCast(myGlobalDataTO.SetDatos, TestReagentsDS)

                            If (myTestReagentsDS.tparTestReagents.Rows.Count > 0) Then
                                myContaminatorReagentID = myTestReagentsDS.tparTestReagents.First.ReagentID
                                myContaminatorTestName = myTestReagentsDS.tparTestReagents.First.TestName & " (" & myTestReagentsDS.tparTestReagents.First.ShortName & ") "
                            Else
                                'This case is not possible....
                                myGlobalDataTO.HasError = True
                            End If
                        End If

                        '(3.2) Get the identifier of the first Reagent for the Contaminated Test in CUSTOMER DB 
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myTestReagentsDelegate.GetTestReagents(pDBConnection, factoryR1Contamination.TestContaminatedID, 1)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myTestReagentsDS = DirectCast(myGlobalDataTO.SetDatos, TestReagentsDS)

                                If (myTestReagentsDS.tparTestReagents.Rows.Count > 0) Then
                                    myContaminatedReagentID = myTestReagentsDS.tparTestReagents.First.ReagentID
                                    myContaminatedTestName = myTestReagentsDS.tparTestReagents.First.TestName & " (" & myTestReagentsDS.tparTestReagents.First.ShortName & ") "
                                Else
                                    'This case is not possible....
                                    myGlobalDataTO.HasError = True
                                End If
                            End If
                        End If

                        '(3.3) Verify in the list of R1 Contaminations from CUSTOMER DB if the pair ReagentContaminatorID/ReagentContaminatedID already exists
                        If (Not myGlobalDataTO.HasError) Then
                            customerR1Contamination = (From a As ContaminationsDS.tparContaminationsRow In myCustomerContaminationsR1.tparContaminations _
                                                      Where a.ReagentContaminatorID = myContaminatorReagentID _
                                                    AndAlso a.ReagentContaminatedID = myContaminatedReagentID _
                                                     Select a).ToList()

                            If (customerR1Contamination.Count = 0) Then
                                '(3.3.1) New R1 Contamination: inform the obtained ReagentIDs in the ContaminationsDS row as ReagentContaminatorID and ReagentContaminatedID 
                                '        respectively and create the new R1 Contamination in CUSTOMER DB
                                factoryR1Contamination.ReagentContaminatorID = myContaminatorReagentID
                                factoryR1Contamination.ReagentContaminatedID = myContaminatedReagentID
                                myGlobalDataTO = myContaminationsDelegate.Create(pDBConnection, factoryR1Contamination)

                                'Add a new row in the list of Contaminations affected by Update Version Process
                                myWashSol1 = "--"
                                If (Not factoryR1Contamination.IsWashingSolutionR1Null) Then myWashSol1 = factoryR1Contamination.WashingSolutionR1
                                AddContaminationToChangesStructure(pUpdateVersionChangesList, "NEW", "R1", myContaminatorTestName, myContaminatedTestName, "--", myWashSol1)
                            Else
                                '(3.3.2) Verify if the Washing Solution needed to avoid the Contamination has changed, and in this case, update it in CUSTOMER DB
                                If (Not factoryR1Contamination.IsWashingSolutionR1Null) Then
                                    If (customerR1Contamination.First.IsWashingSolutionR1Null OrElse _
                                        customerR1Contamination.First.WashingSolutionR1 <> factoryR1Contamination.WashingSolutionR1) Then
                                        'Add a new row in the list of Contaminations affected by Update Version Process
                                        myWashSol1 = "--"
                                        If (Not customerR1Contamination.First.IsWashingSolutionR1Null) Then myWashSol1 = customerR1Contamination.First.WashingSolutionR1
                                        AddContaminationToChangesStructure(pUpdateVersionChangesList, "UPDATE", "R1", myContaminatorTestName, myContaminatedTestName, _
                                                                           myWashSol1, factoryR1Contamination.WashingSolutionR1)

                                        'Update the Washing Solution in CUSTOMER DB
                                        customerR1Contamination.First.WashingSolutionR1 = factoryR1Contamination.WashingSolutionR1
                                        myGlobalDataTO = myContaminationsDelegate.UpdateWashingSolutions(pDBConnection, customerR1Contamination.First)
                                    End If
                                End If
                            End If
                        End If

                        'If an error is raised, then the process finishes
                        If (myGlobalDataTO.HasError) Then Exit For
                    Next
                    customerR1Contamination = Nothing
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("STD Test Update Error", "ContaminationsUpdateData.ProcessNEWorUPDContaminationsR1", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Execute the process to search in FACTORY DB all CUVETTES Contaminations that do not exist in CUSTOMER DB or that exist but with a different 
        ''' Washing Solution R1 and/or R2 and add/modify data in CUSTOMER DB (table tparContaminations)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 14/10/2014 BA-1944 (SubTask BA-1986)
        ''' </remarks>
        Private Function ProcessNEWorUPDContaminationsCUVETTES(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myWashSol1 As String = String.Empty
                Dim myWashSol2 As String = String.Empty
                Dim myContaminatorTestName As String = String.Empty

                Dim myTestsDS As New TestsDS
                Dim myFactoryContaminationsCUVETTES As New ContaminationsDS
                Dim myCustomerContaminationsCUVETTES As New ContaminationsDS

                Dim myTestsDelegate As New TestsDelegate
                Dim myContaminationsDelegate As New ContaminationsDelegate
                Dim myContaminationsUpdateDAO As New ContaminationsUpdateDAO

                Dim updateContamination As Boolean = False
                Dim customerCUVETTESContamination As New List(Of ContaminationsDS.tparContaminationsRow)

                '(1) Search in Factory DB all CUVETTES Contaminations  defined for Preloaded STD TESTS that are not in the CUSTOMER DB or that 
                '    are in CUSTOMER DB but with different Washing Solutions
                myGlobalDataTO = myContaminationsUpdateDAO.GetNEWorUPDContaminationsCUVETTES(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myFactoryContaminationsCUVETTES = DirectCast(myGlobalDataTO.SetDatos, ContaminationsDS)
                End If

                '(2) Get all CUVETTES Contaminations in CUSTOMER DB
                If (Not myGlobalDataTO.HasError) Then
                    myGlobalDataTO = myContaminationsDelegate.GetContaminationsByType(pDBConnection, "CUVETTES")
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myCustomerContaminationsCUVETTES = DirectCast(myGlobalDataTO.SetDatos, ContaminationsDS)
                    End If
                End If

                '(3) Process each new/updated CUVETTES Contamination in FACTORY DB
                If (Not myGlobalDataTO.HasError) Then
                    For Each factoryCUVETTESContamination As ContaminationsDS.tparContaminationsRow In myFactoryContaminationsCUVETTES.tparContaminations
                        '(3.1) Get the name of the Test Contaminator
                        myGlobalDataTO = myTestsDelegate.Read(pDBConnection, factoryCUVETTESContamination.TestContaminaCuvetteID)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myTestsDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)

                            If (myTestsDS.tparTests.Rows.Count > 0) Then
                                'Get the name of the Contaminator Test
                                myContaminatorTestName = myTestsDS.tparTests.First.TestName & " (" & myTestsDS.tparTests.First.ShortName & ") "
                            Else
                                'This case is not possible
                                myGlobalDataTO.HasError = True
                            End If
                        End If

                        '(3.2) Verify in the list of CUVETTES Contaminations from CUSTOMER DB if the Test Contaminator already exists
                        If (Not myGlobalDataTO.HasError) Then
                            customerCUVETTESContamination = (From a As ContaminationsDS.tparContaminationsRow In myCustomerContaminationsCUVETTES.tparContaminations _
                                                            Where a.TestContaminaCuvetteID = factoryCUVETTESContamination.TestContaminaCuvetteID _
                                                           Select a).ToList()

                            If (customerCUVETTESContamination.Count = 0) Then
                                '(3.2.1) New CUVETTES Contamination: create it in CUSTOMER DB
                                myGlobalDataTO = myContaminationsDelegate.Create(pDBConnection, factoryCUVETTESContamination)

                                'Add a new row in the list of Contaminations affected by Update Version Process
                                myWashSol1 = "--"
                                myWashSol2 = "--"
                                If (Not factoryCUVETTESContamination.IsWashingSolutionR1Null) Then myWashSol1 = factoryCUVETTESContamination.WashingSolutionR1
                                If (Not factoryCUVETTESContamination.IsWashingSolutionR2Null) Then myWashSol2 = factoryCUVETTESContamination.WashingSolutionR2
                                AddContaminationToChangesStructure(pUpdateVersionChangesList, "NEW", "CUVETTES", myContaminatorTestName, "--", "--", myWashSol1, "--", myWashSol2)
                            Else
                                '(3.2.2) Verify if the Washing Solutions needed to avoid the Contamination have changed, and in this case, update them in CUSTOMER DB
                                updateContamination = False
                                If (Not factoryCUVETTESContamination.IsWashingSolutionR1Null) Then
                                    If (customerCUVETTESContamination.First.IsWashingSolutionR1Null OrElse _
                                        customerCUVETTESContamination.First.WashingSolutionR1 <> factoryCUVETTESContamination.WashingSolutionR1) Then
                                        myWashSol1 = "--"
                                        If (Not customerCUVETTESContamination.First.IsWashingSolutionR1Null) Then myWashSol1 = customerCUVETTESContamination.First.WashingSolutionR1

                                        customerCUVETTESContamination.First.WashingSolutionR1 = factoryCUVETTESContamination.WashingSolutionR1
                                        updateContamination = True
                                    End If
                                End If

                                If (Not factoryCUVETTESContamination.IsWashingSolutionR2Null) Then
                                    If (customerCUVETTESContamination.First.IsWashingSolutionR2Null OrElse _
                                        customerCUVETTESContamination.First.WashingSolutionR2 <> factoryCUVETTESContamination.WashingSolutionR2) Then
                                        myWashSol2 = "--"
                                        If (Not customerCUVETTESContamination.First.IsWashingSolutionR2Null) Then myWashSol2 = customerCUVETTESContamination.First.WashingSolutionR2

                                        customerCUVETTESContamination.First.WashingSolutionR2 = factoryCUVETTESContamination.WashingSolutionR2
                                        updateContamination = True
                                    End If
                                End If

                                If (updateContamination) Then
                                    'Add a new row in the list of Contaminations affected by Update Version Process
                                    AddContaminationToChangesStructure(pUpdateVersionChangesList, "UPDATE", "CUVETTES", myContaminatorTestName, "--", _
                                                                       myWashSol1, factoryCUVETTESContamination.WashingSolutionR1, myWashSol2, factoryCUVETTESContamination.WashingSolutionR2)

                                    'Update the Washing Solutions in CUSTOMER DB
                                    myGlobalDataTO = myContaminationsDelegate.UpdateWashingSolutions(pDBConnection, customerCUVETTESContamination.First)
                                End If
                            End If
                        End If

                        'If an error is raised, then the process finishes
                        If (myGlobalDataTO.HasError) Then Exit For
                    Next
                    customerCUVETTESContamination = Nothing
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("STD Test Update Error", "ContaminationsUpdateData.ProcessNEWorUPDContaminationsCUVETTES", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Add a new row with data of an added, updated or deleted Test Contamination in the DS containing all changes in CUSTOMER DB due to the Update Version Process.
        ''' Data is added in sub-table TestContaminations
        ''' </summary>
        ''' <param name="pUpdateVersionChangesList">DataSet containing all changes made in CUSTOMER DB for the Update Version Process</param>
        ''' <param name="pAction">Action type: NEW, UPDATE, DELETE</param>
        ''' <param name="pContaminationType">Type of Contamination: R1 or CUVETTES</param>
        ''' <param name="pTestContaminatorName">Name of the Contaminator STD Test</param>
        ''' <param name="pTestContaminatedName">Name of the Contaminated STD Test. Informed only for Contaminations of R1 Type</param>
        ''' <param name="pPreviousWS1">For Contaminations updated: previous value of the Washing Solution R1 in CUSTOMER DB; optional parameter</param>
        ''' <param name="pNewWS1">For Contaminations updated: new value of the Washing Solution R1 (from FACTORY DB); optional parameter</param>
        ''' <param name="pPreviousWS2">For CUVETTES Contaminations updated: previous value of the Washing Solution R2 in CUSTOMER DB; optional parameter</param>
        ''' <param name="pNewWS2">For CUVETTES Contaminations updated: new value of the Washing Solution R2 (from FACTORY DB); optional parameter</param>
        ''' <remarks>
        ''' Created by:  SA 14/10/2014 - BA-1944 (SubTask BA-1986)
        ''' </remarks>
        Private Sub AddContaminationToChangesStructure(ByRef pUpdateVersionChangesList As UpdateVersionChangesDS, ByVal pAction As String, _
                                                       ByVal pContaminationType As String, ByVal pTestContaminatorName As String, ByVal pTestContaminatedName As String, _
                                                       Optional ByVal pPreviousWS1 As String = "", Optional ByVal pNewWS1 As String = "", _
                                                       Optional ByVal pPreviousWS2 As String = "", Optional ByVal pNewWS2 As String = "")
            Try
                Dim myUpdateVersionContaminationsRow As UpdateVersionChangesDS.TestContaminationsRow
                myUpdateVersionContaminationsRow = pUpdateVersionChangesList.TestContaminations.NewTestContaminationsRow
                myUpdateVersionContaminationsRow.Action = pAction
                myUpdateVersionContaminationsRow.ContaminationType = pContaminationType
                myUpdateVersionContaminationsRow.TestContaminator = pTestContaminatorName
                myUpdateVersionContaminationsRow.TestContaminated = pTestContaminatedName

                If (pPreviousWS1 <> String.Empty) Then myUpdateVersionContaminationsRow.PreviousWashingSolR1 = pPreviousWS1
                If (pNewWS1 <> String.Empty) Then myUpdateVersionContaminationsRow.NewWashingSolR1 = pNewWS1
                If (pPreviousWS2 <> String.Empty) Then myUpdateVersionContaminationsRow.PreviousWashingSolR2 = pPreviousWS2
                If (pNewWS2 <> String.Empty) Then myUpdateVersionContaminationsRow.NewWashingSolR2 = pNewWS2

                pUpdateVersionChangesList.TestContaminations.AddTestContaminationsRow(myUpdateVersionContaminationsRow)
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("SDT Test Update Error", "ContaminationsUpdateData.AddContaminationToChangesStructure", EventLogEntryType.Error, False)
                Throw
            End Try
        End Sub

#End Region

    End Class


End Namespace



#Region "OLD METHODS commented- Initial development TR 2011"

'Public Function UpdateContaminationsData(ByVal pDBConnection As SqlClient.SqlConnection, _
'                                         ByVal pDBLocalConnection As SqlClient.SqlConnection) As GlobalDataTO
'    Dim myLogAcciones As New ApplicationLogManager()
'    Dim myGlobalDataTO As New GlobalDataTO
'    Try
'        Dim MasterContaminationsDS As New ContaminationsDS
'        Dim UserContaminationsDS As New ContaminationsDS
'        Dim myContaminationsDelegate As New ContaminationsDelegate

'        Dim myReagentsDelegate As New ReagentsDelegate
'        Dim ReagentContaminatorID As Integer = 0
'        Dim ReagentContaminatedID As Integer = 0

'        Dim ContaminatorReagentName As String = ""
'        Dim ContaminatedReagentName As String = ""

'        Dim myContaminatorID As Integer = -1

'        Dim myTestDS As New TestsDS
'        Dim UserTestID As Integer
'        Dim myTestsDelegate As New TestsDelegate


'        'Get all contamination from Master
'        myGlobalDataTO = myContaminationsDelegate.GetAllContaminations(pDBConnection)

'        If Not myGlobalDataTO.HasError Then
'            MasterContaminationsDS = DirectCast(myGlobalDataTO.SetDatos, ContaminationsDS)

'            For Each contaminationRow As ContaminationsDS.tparContaminationsRow In MasterContaminationsDS.tparContaminations.Rows
'                UserTestID = 0
'                If contaminationRow.ContaminationType = "CUVETTES" Then
'                    'Get the test name on master DB
'                    myGlobalDataTO = myTestsDelegate.Read(pDBConnection, contaminationRow.TestContaminaCuvetteID)
'                    If Not myGlobalDataTO.HasError Then
'                        myTestDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)
'                        If myTestDS.tparTests.Count > 0 Then
'                            'with the test name go to User database and get the test id 
'                            myGlobalDataTO = myTestsDelegate.ExistsTestName(pDBLocalConnection, myTestDS.tparTests(0).TestName)
'                            If Not myGlobalDataTO.HasError Then
'                                If DirectCast(myGlobalDataTO.SetDatos, TestsDS).tparTests.Count > 0 Then
'                                    UserTestID = DirectCast(myGlobalDataTO.SetDatos, TestsDS).tparTests(0).TestID
'                                End If
'                            Else : Exit Try
'                            End If
'                            UserContaminationsDS.tparContaminations.Clear()
'                            UserContaminationsDS.tparContaminations.ImportRow(contaminationRow)
'                            UserContaminationsDS.tparContaminations(0).TestContaminaCuvetteID = UserTestID
'                            myGlobalDataTO = myContaminationsDelegate.SaveContaminations(pDBLocalConnection, UserContaminationsDS.tparContaminations(0).ContaminationType, _
'                                                                                                                    UserContaminationsDS, myContaminatorID)
'                            If myGlobalDataTO.HasError Then Exit Try
'                        End If
'                    End If
'                Else

'                    'Search the reagent name on master Contaminator and contaminated
'                    ContaminatorReagentName = GetReagentNameByReagentID(pDBConnection, contaminationRow.ReagentContaminatorID)
'                    ContaminatedReagentName = GetReagentNameByReagentID(pDBConnection, contaminationRow.ReagentContaminatedID)

'                    If Not ContaminatorReagentName = "" AndAlso Not ContaminatedReagentName = "" Then
'                        'Search on user data by the reagent name to get the id
'                        ReagentContaminatorID = GetReagentIDByReagentName(pDBLocalConnection, ContaminatorReagentName)
'                        ReagentContaminatedID = GetReagentIDByReagentName(pDBLocalConnection, ContaminatedReagentName)
'                    End If

'                    If ReagentContaminatorID > 0 AndAlso ReagentContaminatedID > 0 Then
'                        'Validate if contamination exist on use data
'                        myGlobalDataTO = myContaminationsDelegate.GetContaminationByContaminatorIDContaminatedID(pDBLocalConnection, _
'                                                                                            ReagentContaminatorID, ReagentContaminatedID)

'                        If Not myGlobalDataTO.HasError Then
'                            UserContaminationsDS = DirectCast(myGlobalDataTO.SetDatos, ContaminationsDS)
'                            If UserContaminationsDS.tparContaminations.Count > 0 Then
'                                'Update values
'                                If Not contaminationRow.IsTestContaminaCuvetteIDNull Then
'                                    UserContaminationsDS.tparContaminations(0).TestContaminaCuvetteID = contaminationRow.TestContaminaCuvetteID
'                                Else
'                                    UserContaminationsDS.tparContaminations(0).SetTestContaminaCuvetteIDNull()
'                                End If

'                                UserContaminationsDS.tparContaminations(0).ContaminationType = contaminationRow.ContaminationType

'                                If Not UserContaminationsDS.tparContaminations(0).IsWashingSolutionR1Null Then
'                                    UserContaminationsDS.tparContaminations(0).WashingSolutionR1 = contaminationRow.WashingSolutionR1
'                                Else
'                                    UserContaminationsDS.tparContaminations(0).SetWashingSolutionR1Null()
'                                End If

'                                If Not UserContaminationsDS.tparContaminations(0).IsWashingSolutionR2Null Then
'                                    UserContaminationsDS.tparContaminations(0).WashingSolutionR2 = contaminationRow.WashingSolutionR2
'                                Else
'                                    UserContaminationsDS.tparContaminations(0).SetWashingSolutionR2Null()
'                                End If

'                                UserContaminationsDS.tparContaminations(0).TS_User = "BIOSYSTEMS"

'                            Else
'                                'Set the reagents id for user DB
'                                contaminationRow.ReagentContaminatorID = ReagentContaminatorID
'                                contaminationRow.ReagentContaminatedID = ReagentContaminatedID
'                                contaminationRow.TS_User = "BIOSYSTEMS"
'                                contaminationRow.TS_DateTime = DateTime.Now

'                                'Insert New Contamination.
'                                UserContaminationsDS.tparContaminations.ImportRow(contaminationRow)

'                            End If
'                            myContaminatorID = -1
'                            'Validate contamination type if R1 or R2 then send the contaminator id else -1
'                            If UserContaminationsDS.tparContaminations(0).ContaminationType = "R1" OrElse _
'                                    UserContaminationsDS.tparContaminations(0).ContaminationType = "R2" Then
'                                myContaminatorID = UserContaminationsDS.tparContaminations(0).ReagentContaminatorID
'                            End If

'                            'Update Contamination
'                            myGlobalDataTO = myContaminationsDelegate.SaveContaminations(pDBLocalConnection, UserContaminationsDS.tparContaminations(0).ContaminationType, _
'                                                                                                                    UserContaminationsDS, myContaminatorID)

'                            If Not myGlobalDataTO.HasError Then
'                                myContaminatorID = -1
'                                ReagentContaminatorID = 0
'                                ReagentContaminatedID = 0
'                                ContaminatorReagentName = ""
'                                ContaminatedReagentName = ""

'                            Else : Exit Try
'                            End If

'                        Else : Exit Try

'                        End If
'                    End If
'                End If
'            Next
'        End If

'    Catch ex As Exception
'        myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.ContaminationsUpdateData", _
'                                                                                EventLogEntryType.Error, False)
'        myGlobalDataTO.HasError = True
'        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'        myGlobalDataTO.ErrorMessage = ex.Message
'    End Try

'    Return myGlobalDataTO

'End Function

' ''' <summary>
' ''' 
' ''' </summary>
' ''' <param name="pDBConnection"></param>
' ''' <param name="pReagentID"></param>
' ''' <returns></returns>
' ''' <remarks>CREATED BY: TR 30/03/2011</remarks>
'Private Function GetReagentNameByReagentID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentID As Integer) As String
'    Dim myReagentName As String = ""
'    Try
'        Dim myGlobalDataTO As New GlobalDataTO
'        Dim myReagentsDS As New ReagentsDS
'        Dim myReagentsDelegate As New ReagentsDelegate
'        myGlobalDataTO = myReagentsDelegate.GetReagentsData(pDBConnection, pReagentID)

'        If Not myGlobalDataTO.HasError Then
'            myReagentsDS = DirectCast(myGlobalDataTO.SetDatos, ReagentsDS)
'            If myReagentsDS.tparReagents.Count = 1 Then
'                myReagentName = myReagentsDS.tparReagents(0).ReagentName
'            End If
'        End If
'    Catch ex As Exception
'        Dim myLogAcciones As New ApplicationLogManager()
'        myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.GetReagentNameByReagentID", _
'                                                                                EventLogEntryType.Error, False)
'    End Try
'    Return myReagentName
'End Function

' ''' <summary>
' ''' 
' ''' </summary>
' ''' <param name="pDBConnection"></param>
' ''' <param name="pReagentName"></param>
' ''' <returns></returns>
' ''' <remarks>CREATED BY: TR 30/03/2011</remarks>
'Private Function GetReagentIDByReagentName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentName As String) As Integer
'    Dim myReagentID As Integer = 0
'    Try
'        Dim myGlobalDataTO As New GlobalDataTO
'        Dim myReagentsDS As New ReagentsDS
'        Dim myReagentsDelegate As New ReagentsDelegate
'        myGlobalDataTO = myReagentsDelegate.GetReagentByReagentName(pDBConnection, pReagentName)

'        If Not myGlobalDataTO.HasError Then
'            myReagentsDS = DirectCast(myGlobalDataTO.SetDatos, ReagentsDS)
'            If myReagentsDS.tparReagents.Count = 1 Then
'                myReagentID = myReagentsDS.tparReagents(0).ReagentID
'            End If
'        End If
'    Catch ex As Exception
'        Dim myLogAcciones As New ApplicationLogManager()
'        myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.GetReagentIDByReagentName", _
'                                                                                EventLogEntryType.Error, False)
'    End Try
'    Return myReagentID
'End Function

#End Region


