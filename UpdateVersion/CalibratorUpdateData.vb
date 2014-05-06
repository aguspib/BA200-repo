Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.BL.UpdateVersion

    Public Class CalibratorUpdateData
        Inherits UpdateElementParent


#Region " Internal Data "
#Region " DataTable "
        Protected Overrides Function GetDataTable(pMyDataSet As DataSet) As DataTable
            Return DirectCast(pMyDataSet, CalibratorsDS).tparCalibrators
        End Function
#End Region

        ''' <summary>
        ''' Get data from Factory DB and fill Row 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pItemRow"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: DL - 06/02/2013
        ''' </remarks>
        Protected Overrides Function GetDataInFactoryDB(pDBConnection As SqlClient.SqlConnection,
                                                        pItemRow As DataRow) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim calibRow As CalibratorsDS.tparCalibratorsRow = DirectCast(pItemRow, CalibratorsDS.tparCalibratorsRow)
                If calibRow Is Nothing Then
                    Return myGlobalDataTO
                Else
                    Dim myCalibrators As New tparCalibratorsDAO
                    myGlobalDataTO = myCalibrators.ReadByCalibratorName(pDBConnection, calibRow.CalibratorName, True)
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Calibrator Update Error.", "CalibratorUpdateData.GetDataInFactoryDB", EventLogEntryType.Error, False)
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
        ''' Created by: DL - 06/02/2013
        ''' </remarks>
        Protected Overrides Function GetDataInLocalDB(pDBConnection As SqlClient.SqlConnection,
                                                      pItemRow As DataRow) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim calibRow As CalibratorsDS.tparCalibratorsRow = DirectCast(pItemRow, CalibratorsDS.tparCalibratorsRow)
                If calibRow Is Nothing Then
                    Return myGlobalDataTO
                Else
                    Dim myCalibrators As New tparCalibratorsDAO
                    myGlobalDataTO = myCalibrators.ReadByCalibratorName(pDBConnection, calibRow.CalibratorName, False)
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Calibrator Update Error.", "CalibratorUpdateData.GetDataInLocalDB", EventLogEntryType.Error, False)
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
        ''' Created by: DL - 06/02/2013
        ''' modified by: TR 08/05/2013 add the connection.
        ''' </remarks>
        Protected Overrides Function IsFactoryDefinedItem(pDataInDB As Object, pDBConnection As SqlClient.SqlConnection) As Boolean
            ' Try
            'Dim calibratorRow As CalibratorsDS.tparCalibratorsRow = DirectCast(pDataInDB, CalibratorsDS.tparCalibratorsRow)
            'If calibratorRow Is Nothing Then Return False

            'Return False 

            'Catch ex As Exception
            'Dim myLogAcciones As New ApplicationLogManager()
            'myLogAcciones.CreateLogActivity("Calibrators Update Error.", "CalibratorUpdateData.IsFactoryDefinedItem", EventLogEntryType.Error, False)
            'End Try

            Return False
        End Function
#End Region

#Region " Factory Updates "

        ''' <summary>
        ''' Performs update actions related to modified data in Factory DB: inserts and updates in local DB
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: DL - 29/01/2013
        ''' </remarks>
        Protected Overrides Function GetAffectedItemsFromFactoryUpdates(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myLogAcciones As New ApplicationLogManager()
            Dim myGlobalDataTO As New GlobalDataTO

            Try

                myGlobalDataTO = CalibratorUpdateDAO.GetCalibratorsDistinctInClient(pDBConnection)

            Catch ex As Exception
                myLogAcciones.CreateLogActivity("Calibrators Update Error.", "CalibratorUpdateData.GetAffectedItemsFromFactoryUpdates", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO

        End Function


#Region " Do Actions "
        ''' <summary>
        ''' Create new calibator like in Factory DB
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB"></param>
        ''' <param name="pDataInLocalDB"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: DL - 29/01/2013
        ''' </remarks>
        Protected Overrides Function DoCreateNewElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myCalibratorsDS As CalibratorsDS = DirectCast(pDataInFactoryDB, CalibratorsDS)
                If myCalibratorsDS Is Nothing Then Return myGlobalDataTO

                For Each calibRow As CalibratorsDS.tparCalibratorsRow In myCalibratorsDS.tparCalibrators
                    calibRow.BeginEdit()
                    calibRow.IsNew = True
                    calibRow.EndEdit()
                Next

                Dim myCalibratorsDelegate As New CalibratorsDelegate
                myGlobalDataTO = myCalibratorsDelegate.Save(pDBConnection,
                                                            myCalibratorsDS,
                                                            New TestCalibratorsDS,
                                                            New TestCalibratorValuesDS,
                                                            Nothing,
                                                            False,
                                                            False,
                                                            False,
                                                            "",
                                                            "")

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Calibrators Update Error.", "CalibratorUpdateData.DoCreateNewElement", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' DoUpdateUserDefinedElement
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB"></param>
        ''' <param name="pDataInLocalDB"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  DL - 29/01/2013
        ''' Modified by: XB - 15/02/2013 - Correction : implement a new function to obtain a valid calibrator renaming also watching database size field limitations (Bugs tracking #1134)
        ''' </remarks>
        Protected Overrides Function DoUpdateUserDefinedElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If pDataInLocalDB Is Nothing Then Return myGlobalDataTO

                'Dim calibratorsRowLocal As CalibratorsDS.tparCalibratorsRow = DirectCast(DirectCast(pDataInLocalDB, CalibratorsDS).tparCalibrators.First, CalibratorsDS.tparCalibratorsRow)
                'If calibratorsRowLocal Is Nothing Then Return myGlobalDataTO

                ''Rename calibrator name adding (R) at the beginning of the name validting that not exist in db
                'Dim IsOK As Boolean = False
                'Dim myNewname As String = ""
                'Dim myCalibrators As New tparCalibratorsDAO

                'While Not IsOK
                '    myNewname &= "R" & calibratorsRowLocal.CalibratorName

                '    myGlobalDataTO = myCalibrators.ReadByCalibratorName(pDBConnection, myNewname)

                '    If Not myGlobalDataTO.HasError Then
                '        Dim myCalibratorDS As New CalibratorsDS
                '        myCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, CalibratorsDS)

                '        If myCalibratorDS.tparCalibrators.Rows.Count = 0 Then IsOK = True
                '    End If

                'End While

                ''Rename calibrator in client database. Adding (R) at the beginning of the name
                Dim myCalibratorsDS As CalibratorsDS = DirectCast(pDataInLocalDB, CalibratorsDS)
                If myCalibratorsDS Is Nothing Then Return myGlobalDataTO

                'For Each calibRow As CalibratorsDS.tparCalibratorsRow In myCalibratorsDS.tparCalibrators
                '    calibRow.BeginEdit()
                '    calibRow.CalibratorName = myNewname
                '    calibRow.IsNew = False
                '    calibRow.EndEdit()
                'Next

                myGlobalDataTO = MyClass.RenameCalibratorName(pDBConnection, myCalibratorsDS.tparCalibrators.First)
                If Not myGlobalDataTO.HasError Then

                    Dim myCalibratorsDelegate As New CalibratorsDelegate
                    myGlobalDataTO = myCalibratorsDelegate.Save(pDBConnection,
                                                                myCalibratorsDS,
                                                                New TestCalibratorsDS,
                                                                New TestCalibratorValuesDS, _
                                                                Nothing,
                                                                False,
                                                                False,
                                                                False,
                                                                "",
                                                                "")

                    'Create factory calibrator 
                    If Not myGlobalDataTO.HasError Then
                        myCalibratorsDS = DirectCast(pDataInFactoryDB, CalibratorsDS)
                        If myCalibratorsDS Is Nothing Then Return myGlobalDataTO

                        For Each calibRow As CalibratorsDS.tparCalibratorsRow In myCalibratorsDS.tparCalibrators
                            calibRow.BeginEdit()
                            calibRow.IsNew = True
                            calibRow.EndEdit()
                        Next

                        myGlobalDataTO = myCalibratorsDelegate.Save(pDBConnection,
                                                                    myCalibratorsDS,
                                                                    New TestCalibratorsDS,
                                                                    New TestCalibratorValuesDS, _
                                                                    Nothing,
                                                                    False,
                                                                    False,
                                                                    False, _
                                                                    "",
                                                                    "")
                    End If

                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Calibrators Update Error.", "CalibratorUpdateData.DoUpdateUserDefinedElement", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get a valid  CalibratorName
        ''' This method is used by the Update process.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pCalibratorsRow"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: XB 19/02/2013 - Correction : implement a new function to obtain a valid calibrator renaming also watching database size field limitations (Bugs tracking #1134)
        ''' </remarks>
        Private Function RenameCalibratorName(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pCalibratorsRow As CalibratorsDS.tparCalibratorsRow) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try

                'Validate Change the name and validate if do not exist on Client DB.
                Dim isValidNewName As Boolean = False
                Dim isValidNewLongName As Boolean = False

                Dim myValidCalibratorName As String = String.Empty
                myValidCalibratorName = pCalibratorsRow.CalibratorName

                Dim myCalibratorDAO As New tparCalibratorsDAO
                Dim myCalibratorDS As New CalibratorsDS

                While Not isValidNewName

                    If Not isValidNewName Then
                        'Add the R at the begining
                        myValidCalibratorName = "R" & myValidCalibratorName

                        'Validate the name lenght should not be more than 16
                        If myValidCalibratorName.Length > 16 Then
                            'Remove the last letter
                            myValidCalibratorName = myValidCalibratorName.Remove(myValidCalibratorName.Length - 1)
                        End If

                        'Search on local db if name exist.
                        Dim myCalibrators As New tparCalibratorsDAO
                        Dim myCalibratorQueryDS As New CalibratorsDS
                        ' Check if exist the name into Factory DB
                        myGlobalDataTO = myCalibrators.ReadByCalibratorName(pDBConnection, myValidCalibratorName, True)
                        If Not myGlobalDataTO.HasError Then
                            myCalibratorQueryDS = DirectCast(myGlobalDataTO.SetDatos, CalibratorsDS)
                            If myCalibratorQueryDS.tparCalibrators.Rows.Count = 0 Then
                                ' Check if exist the name into Local DB
                                myGlobalDataTO = myCalibrators.ReadByCalibratorName(pDBConnection, myValidCalibratorName, False)
                                If Not myGlobalDataTO.HasError Then
                                    myCalibratorQueryDS = DirectCast(myGlobalDataTO.SetDatos, CalibratorsDS)
                                    If myCalibratorQueryDS.tparCalibrators.Rows.Count = 0 Then
                                        'If Not exist then is a valid name
                                        isValidNewName = True
                                    End If
                                End If

                            End If
                        End If

                    End If

                End While

                pCalibratorsRow.BeginEdit()
                pCalibratorsRow.CalibratorName = myValidCalibratorName
                pCalibratorsRow.EndEdit()

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Calibrators Test Update Error.", "CalibratorUpdateData.RenameCalibratorName", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO
        End Function

        Protected Overrides Function DoUpdateFactoryDefinedElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            'No action is needed
            Return New GlobalDataTO
        End Function

        Protected Overrides Function DoIgnoreElementUpdating(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            'No action is needed
            Return New GlobalDataTO
        End Function
#End Region
#End Region

#Region " Factory Removes "
        Protected Overrides Function GetAffectedItemsFromFactoryRemoves(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                myGlobalDataTO = CalibratorUpdateDAO.GetCalibratorsDistinctInFactory(pDBConnection)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Calibrators Update Error.", "CalibratorUpdateData.GetAffectedItemsFromFactoryRemoves", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

#Region " Do Actions "
        Protected Overrides Function DoRemoveUserElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            'No action is needed
            Return New GlobalDataTO
        End Function

        Protected Overrides Function DoRemoveFactoryElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim calibratorRowLocal As CalibratorsDS.tparCalibratorsRow = DirectCast(pDataInLocalDB, CalibratorsDS.tparCalibratorsRow)
                If calibratorRowLocal Is Nothing Then Return myGlobalDataTO

                'Close calibrator in Historical module
                'Remove test in Local DB

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Calibrators Update Error.", "CalibratorUpdateData.DoRemoveFactoryElement", EventLogEntryType.Error, False)
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
#End Region
#End Region

        Protected Overrides Function IsDataEmpty(pDataInDB As Object) As Boolean
            Dim myGlobalDataTO As New GlobalDataTO
            Dim returndata As Boolean = True

            Try
                If Not pDataInDB Is Nothing Then
                    Dim myCalibrators As CalibratorsDS = DirectCast(pDataInDB, CalibratorsDS)
                    If myCalibrators.tparCalibrators.Count > 0 Then returndata = False
                End If


            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Calibrators Update Error.", "CalibratorUpdateData.IsDataEmpty", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return returndata

        End Function
    End Class

End Namespace
