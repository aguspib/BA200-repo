Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL

Namespace Biosystems.Ax00.BL.UpdateVersion

    ''' <summary>
    ''' Abstract class: inherits from in order to implement the Update Process
    ''' FactoryUpdates => The process to update local DB with new data in Factory DB (factory new, factory update, client remove or client update)
    ''' FactoryRemoves => The process to update local DB with removed data in Factory DB (factory remove)
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 29/01/2013
    ''' </remarks>
    Public MustInherit Class UpdateElementParent

#Region " Enumerations "
        ''' <summary>
        ''' Available actions to perform in Local DB with an elements in Factory Updates verification
        ''' </summary>
        ''' <remarks>
        ''' Created by: JB - 29/01/2013
        ''' </remarks>
        Protected Enum LocalActionsWithFactoryUpdates As Byte
            ''' <summary>
            ''' Need to Create a new element in Local DB
            ''' </summary>
            ''' <remarks></remarks>
            CreateNew = 1

            ''' <summary>
            ''' Need to Update the element in Local DB: exist and is User-defined
            ''' </summary>
            ''' <remarks></remarks>
            UpdateUserDefined = 2

            ''' <summary>
            ''' Need to Update the element in Local DB: exists and is Factory-defined
            ''' </summary>
            ''' <remarks></remarks>
            ''' 
            UpdateFactoryDefined = 3

            ''' <summary>
            ''' Need to ignore the element in Local DB
            ''' </summary>
            ''' <remarks></remarks>
            Ignore = 0
        End Enum

        ''' <summary>
        ''' Available actions to perform in Local DB with an elements in Factory Removes verification
        ''' </summary>
        ''' <remarks>
        ''' Created by: JB - 29/01/2013
        ''' </remarks>
        Protected Enum LocalActionsWithFactoryRemoves As Byte
            ''' <summary>
            ''' Need to Remove the element in Local DB: exists and is User-defined
            ''' </summary>
            ''' <remarks></remarks>
            RemoveUserDefined = 1

            ''' <summary>
            ''' Need to Remove the element in Local DB: exists and is Factory-defined
            ''' </summary>
            ''' <remarks></remarks>
            RemoveFactoryDefined = 2

            ''' <summary>
            ''' Need to ignore the element in Local DB
            ''' </summary>
            ''' <remarks></remarks>
            Ignore = 0
        End Enum
#End Region

#Region " Public Methods "
        ''' <summary>
        ''' Performs update actions related to modified data in Factory DB: inserts and updates in local DB
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: JB - 28/01/2013
        ''' AG 31/01/2013 - use the proper template
        ''' </remarks>
        Public Overridable Function UpdateFromFactoryUpdates(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myLogAcciones As New ApplicationLogManager()
            Dim myDataSet As DataSet = Nothing
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Search elems affected
                        resultData = GetAffectedItemsFromFactoryUpdates(dbConnection)
                        If Not resultData.HasError Then

                            myDataSet = DirectCast(resultData.SetDatos, DataSet)
                            If Not myDataSet Is Nothing Then
                                For Each itemRow As DataRow In GetDataTable(myDataSet).Rows
                                    resultData = DoItemActionFromFactoryUpdates(dbConnection, itemRow)

                                    'SGM 19/02/2013 - abort loop in case of error
                                    If resultData.HasError Then
                                        Exit For
                                    End If

                                Next
                            End If

                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message
                myLogAcciones.CreateLogActivity(ex.Message, "UpdateElementParent.UpdateFromFactoryUpdates", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

        End Function

        ''' <summary>
        ''' Performs update actions related to removed data in Factory DB: removes in local DB
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: JB - 29/01/2013
        ''' AG 31/01/2013 - use the proper template
        ''' </remarks>
        Public Overridable Function UpdateFromFactoryRemoves(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myLogAcciones As New ApplicationLogManager()
            Dim myDataSet As DataSet = Nothing
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'Search elems affected
                        resultData = GetAffectedItemsFromFactoryRemoves(dbConnection)
                        If Not resultData.HasError Then

                            myDataSet = DirectCast(resultData.SetDatos, DataSet)
                            If Not myDataSet Is Nothing Then
                                'For each elem, perform action
                                For Each itemRow As DataRow In GetDataTable(myDataSet).Rows
                                    resultData = DoItemActionFromFactoryRemoves(dbConnection, itemRow)

                                    'SGM 19/02/2013 - abort loop in case of error
                                    If resultData.HasError Then
                                        Exit For
                                    End If

                                Next
                            End If

                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message
                myLogAcciones.CreateLogActivity(ex.Message, "UpdateElementParent.UpdateFromFactoryRemoves", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

        End Function

        ''' <summary>
        ''' Performs any required final validation
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XB 21/02/2013 - (Bugs tracking #1134)
        ''' </remarks>
        Public Overridable Function DoFinalActions(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Return New GlobalDataTO
        End Function
#End Region

#Region " Processing Elements "
        ''' <summary>
        ''' Performs correct action related to Factory Updates 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pItemRow"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: JB 29/01/2013
        ''' AG 31/01/2013 - use the proper template
        ''' </remarks>
        Protected Overridable Function DoItemActionFromFactoryUpdates(ByVal pDBConnection As SqlClient.SqlConnection, pItemRow As DataRow) As GlobalDataTO
            Dim myLogAcciones As New ApplicationLogManager()
            Dim myDataSet As DataSet = Nothing
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        resultData = GetDataInFactoryDB(dbConnection, pItemRow)
                        If Not resultData.HasError Then
                            Dim myDataInFactoryDB As Object = resultData.SetDatos
                            resultData = GetDataInLocalDB(dbConnection, pItemRow)
                            If Not resultData.HasError Then
                                Dim myDataInLocalDB As Object = resultData.SetDatos

                                resultData = GetActionFromFactoryUpdates(dbConnection, myDataInFactoryDB, myDataInLocalDB)
                                If Not resultData.HasError Then
                                    Dim action As LocalActionsWithFactoryUpdates = DirectCast(resultData.SetDatos, LocalActionsWithFactoryUpdates)
                                    Select Case action
                                        Case LocalActionsWithFactoryUpdates.CreateNew
                                            resultData = DoCreateNewElement(dbConnection, myDataInFactoryDB, myDataInLocalDB)
                                        Case LocalActionsWithFactoryUpdates.UpdateUserDefined
                                            resultData = DoUpdateUserDefinedElement(dbConnection, myDataInFactoryDB, myDataInLocalDB)
                                        Case LocalActionsWithFactoryUpdates.UpdateFactoryDefined
                                            resultData = DoUpdateFactoryDefinedElement(pDBConnection, myDataInFactoryDB, myDataInLocalDB)
                                        Case LocalActionsWithFactoryUpdates.Ignore
                                            resultData = DoIgnoreElementUpdating(dbConnection, myDataInFactoryDB, myDataInLocalDB)
                                    End Select
                                End If
                            End If
                        End If
                    End If

                    If (Not resultData.HasError) Then
                        'When the Database Connection was opened locally, then the Commit is executed
                        If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        'resultData.SetDatos = <value to return; if any>
                    Else
                        'When the Database Connection was opened locally, then the Rollback is executed
                        If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message
                myLogAcciones.CreateLogActivity(ex.Message, "UpdateElementParent.DoItemActionFromFactoryUpdates", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

        End Function

        ''' <summary>
        ''' Performs correct action related to Factory Removes
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pItemRow"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: JB 29/01/2013
        ''' AG 31/01/2013 - use the proper template
        ''' </remarks>
        Protected Overridable Function DoItemActionFromFactoryRemoves(ByVal pDBConnection As SqlClient.SqlConnection, pItemRow As DataRow) As GlobalDataTO
            Dim myLogAcciones As New ApplicationLogManager()
            Dim myDataSet As DataSet = Nothing
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        resultData = GetDataInFactoryDB(dbConnection, pItemRow)
                        If Not resultData.HasError Then
                            Dim myDataInFactoryDB As Object = resultData.SetDatos

                            resultData = GetDataInLocalDB(dbConnection, pItemRow)
                            If Not resultData.HasError Then
                                Dim myDataInLocalDB As Object = resultData.SetDatos

                                resultData = GetActionFromFactoryRemoves(dbConnection, myDataInFactoryDB, myDataInLocalDB)
                                If Not resultData.HasError Then

                                    Dim action As LocalActionsWithFactoryRemoves = DirectCast(resultData.SetDatos, LocalActionsWithFactoryRemoves)
                                    Select Case action
                                        Case LocalActionsWithFactoryRemoves.RemoveUserDefined
                                            resultData = DoRemoveUserElement(dbConnection, myDataInFactoryDB, myDataInLocalDB)
                                        Case LocalActionsWithFactoryRemoves.RemoveFactoryDefined
                                            resultData = DoRemoveFactoryElement(dbConnection, myDataInFactoryDB, myDataInLocalDB)
                                        Case LocalActionsWithFactoryRemoves.Ignore
                                            resultData = DoIgnoreElementRemoving(dbConnection, myDataInFactoryDB, myDataInLocalDB)
                                    End Select
                                End If
                            End If
                        End If

                    End If

                    If (Not resultData.HasError) Then
                        'When the Database Connection was opened locally, then the Commit is executed
                        If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        'resultData.SetDatos = <value to return; if any>
                    Else
                        'When the Database Connection was opened locally, then the Rollback is executed
                        If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message
                myLogAcciones.CreateLogActivity(ex.Message, "UpdateElementParent.DoItemActionFromFactoryRemoves", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

        End Function

        ''' <summary>
        ''' Returns the action to do with the concrete row in local DB, if Factory data not match with local elements
        ''' </summary>
        ''' <param name="pDataInFactoryDB">Data of the element in Factory DB</param>
        ''' <param name="pDataInLocalDB">Data of the element in Local DB</param>
        ''' <returns>GlobalDataTO with LocalActionsWithFactoryUpdates</returns>
        ''' <remarks>
        ''' Created by: JB - 28/01/2013
        ''' </remarks>
        Protected Overridable Function GetActionFromFactoryUpdates(ByVal pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            myGlobalDataTO.SetDatos = LocalActionsWithFactoryUpdates.Ignore
            Try
                ' If Not Exists in Local DB: Action = CreateNew
                ' If Exists in Local DB as UserTest: Action = UpdateUserDefined
                ' If Exists in Local DB as PreloadedTest: Action = UpdateFactoryDefined

                If IsDataEmpty(pDataInLocalDB) Then
                    myGlobalDataTO.SetDatos = LocalActionsWithFactoryUpdates.CreateNew
                ElseIf IsFactoryDefinedItem(pDataInLocalDB, pDBConnection) Then
                    myGlobalDataTO.SetDatos = LocalActionsWithFactoryUpdates.UpdateFactoryDefined
                Else
                    myGlobalDataTO.SetDatos = LocalActionsWithFactoryUpdates.UpdateUserDefined
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Elememt Update Error.", "UpdateElementParent.GetActionFromFactoryUpdates", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Returns the action to do with the concrete row in local DB if not match with Factory elements
        ''' </summary>
        ''' <param name="pDataInFactoryDB">Data of the element in Factory DB</param>
        ''' <param name="pDataInLocalDB">Data of the element in Local DB</param>
        ''' <returns>GlobalDataTO with LocalActionsWithFactoryRemoves</returns>
        ''' <remarks>
        ''' Created by: JB - 29/01/2013
        ''' </remarks>
        Protected Overridable Function GetActionFromFactoryRemoves(ByVal pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            myGlobalDataTO.SetDatos = LocalActionsWithFactoryRemoves.Ignore
            Try
                ' If Not Exists in Factory DB and:
                ' If Exists in Local DB as UserTest: Action = RemoveUserDefined
                ' If Exists in Local DB as PreloadedTest: Action = RemoveFactoryDefined

                If IsDataEmpty(pDataInFactoryDB) Then
                    If IsFactoryDefinedItem(pDataInLocalDB, pDBConnection) Then
                        myGlobalDataTO.SetDatos = LocalActionsWithFactoryRemoves.RemoveFactoryDefined
                    Else
                        myGlobalDataTO.SetDatos = LocalActionsWithFactoryRemoves.RemoveUserDefined
                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("CALC Test Update Error.", "UpdateElementParent.GetActionFromFactoryRemoves", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function
#End Region

#Region " MustOverride Methods"
#Region " Data Methods "
        ''' <summary>
        ''' Returns the DataTable to work with
        ''' </summary>
        ''' <param name="pMyDataSet"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: JB - 29/01/2013
        ''' </remarks>
        Protected MustOverride Function GetDataTable(pMyDataSet As DataSet) As DataTable

        ''' <summary>
        ''' Returns if the Data in parameter is Empty
        ''' </summary>
        ''' <param name="pDataInDB">Data to process</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: JB - 30/01/2013
        ''' </remarks>
        Protected MustOverride Function IsDataEmpty(pDataInDB As Object) As Boolean

        ''' <summary>
        ''' Returns Data of the element stored in Local DB
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pItemRow">The row to identify the element</param>
        ''' <returns>Return the Data in GlobalDataTO.SetDatos. If Data don't exists in Local DB, store Nothing</returns>
        ''' <remarks>
        ''' Created by: JB 29/01/2013
        ''' </remarks>
        Protected MustOverride Function GetDataInLocalDB(ByVal pDBConnection As SqlClient.SqlConnection, pItemRow As DataRow) As GlobalDataTO

        ''' <summary>
        ''' Returns Data of the element stored in Factory DB
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pItemRow">The row to identify the element</param>
        ''' <returns>Return the Data in GlobalDataTO.SetDatos. If Data don't exists in Factory DB, store Nothing</returns>
        ''' <remarks>
        ''' Created by: JB 29/01/2013
        ''' </remarks>
        Protected MustOverride Function GetDataInFactoryDB(ByVal pDBConnection As SqlClient.SqlConnection, pItemRow As DataRow) As GlobalDataTO

        ''' <summary>
        ''' Returns if an element (with all data in parameter) is "Factory defined"
        ''' </summary>
        ''' <param name="pDataInDB"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: JB - 29/01/2013
        ''' </remarks>
        Protected MustOverride Function IsFactoryDefinedItem(pDataInDB As Object, ByVal pDBConnection As SqlClient.SqlConnection) As Boolean
#End Region

#Region " Process Methods "
        ''' <summary>
        ''' Gets all the Elements from local DB that differs from Factory Preloaded tests
        ''' These elements will be created or updated in local DB to match Factory data
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: JB - 28/01/2013
        ''' Modified by: TR -08/05/2013 -Implement the connection because is required for consulting database info.
        ''' </remarks>
        Protected MustOverride Function GetAffectedItemsFromFactoryUpdates(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO

        ''' <summary>
        ''' Gets all the Elements from Factory DB that differs from local DB
        ''' These elements will be removed (or ignored) in local DB to match Factory data
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: JB - 28/01/2013
        ''' </remarks>
        Protected MustOverride Function GetAffectedItemsFromFactoryRemoves(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO


        ''' <summary>
        ''' Create a new element in local DB
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB">Data of the element in Factory DB</param>
        ''' <param name="pDataInLocalDB">Data of the element in Local DB</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: JB - 29/01/2013
        ''' </remarks>
        Protected MustOverride Function DoCreateNewElement(ByVal pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO

        ''' <summary>
        ''' Update an element tagged as User-defined in local DB
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB">Data of the element in Factory DB</param>
        ''' <param name="pDataInLocalDB">Data of the element in Local DB</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: JB - 29/01/2013
        ''' </remarks>
        Protected MustOverride Function DoUpdateUserDefinedElement(ByVal pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO

        ''' <summary>
        ''' Update an element tagged as Factory-defined in local DB
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB">Data of the element in Factory DB</param>
        ''' <param name="pDataInLocalDB">Data of the element in Local DB</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: JB - 29/01/2013
        ''' </remarks>
        Protected MustOverride Function DoUpdateFactoryDefinedElement(ByVal pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO

        ''' <summary>
        ''' Ignore an element in the Updating process
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB">Data of the element in Factory DB</param>
        ''' <param name="pDataInLocalDB">Data of the element in Local DB</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: JB - 29/01/2013
        ''' </remarks>
        Protected MustOverride Function DoIgnoreElementUpdating(ByVal pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO


        ''' <summary>
        ''' Remove an element in local DB tagged as User-defined
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB">Data of the element in Factory DB</param>
        ''' <param name="pDataInLocalDB">Data of the element in Local DB</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: JB - 29/01/2013
        ''' </remarks>
        Protected MustOverride Function DoRemoveUserElement(ByVal pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO

        ''' <summary>
        ''' Remove an element in local DB tagged as Factory-defined
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB">Data of the element in Factory DB</param>
        ''' <param name="pDataInLocalDB">Data of the element in Local DB</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: JB - 29/01/2013
        ''' </remarks>
        Protected MustOverride Function DoRemoveFactoryElement(ByVal pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO

        ''' <summary>
        ''' Ignore an element in the Removing process
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB">Data of the element in Factory DB</param>
        ''' <param name="pDataInLocalDB">Data of the element in Local DB</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: JB - 29/01/2013
        ''' </remarks>
        Protected MustOverride Function DoIgnoreElementRemoving(ByVal pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
#End Region
#End Region
    End Class

End Namespace
