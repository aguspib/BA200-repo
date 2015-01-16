Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.BL.UpdateVersion
    Public Class CalibratorUpdateData

#Region "FUNCTIONS FOR NEW UPDATE VERSION PROCESS (NEW AND UPDATED FUNCTIONS)"
        ''' <summary>
        ''' Search all in FACTORY DB all Experimental Calibrators that do not exist in Customer DB or that exists but with different Number of Calibrators or SpecialCalib
        ''' flag. The Calibrator in FACTORY DB is always added to the Customer DB; the Calibrator with the same Name in Customer DB is renamed previously.  
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection></param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 08/10/2014 - BA-1944 (SubTask BA-1982)
        ''' </remarks>
        Public Function ProcessForCALIBRATORS(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myCalibratorsDS As New CalibratorsDS
                Dim myFactoryCalibratorDS As New CalibratorsDS
                Dim myCalibratorsDelegate As New CalibratorsDelegate
                Dim myCalibratorUpdateDAO As New CalibratorUpdateDAO
                Dim myUpdateVersionAddedElementsRow As UpdateVersionChangesDS.AddedElementsRow

                '(1) Search in Factory DB all new or updated CALIBRATORS
                myGlobalDataTO = CalibratorUpdateDAO.GetCalibratorsDistinctInClient(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myCalibratorsDS = DirectCast(myGlobalDataTO.SetDatos, CalibratorsDS)

                    '(2) Process each new/updated CALIBRATOR in Factory DB to add it to Customer DB
                    For Each newCalib As CalibratorsDS.tparCalibratorsRow In myCalibratorsDS.tparCalibrators
                        '(2.1) Get all data for the Calibrator in table tparCalibrators in Factory DB
                        myGlobalDataTO = myCalibratorUpdateDAO.GetDataInFactoryDB(pDBConnection, newCalib.CalibratorName)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myFactoryCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, CalibratorsDS)
                        End If

                        '(2.2) Verify if there is a Calibrator in Customer DB with the same Name, and in this case, rename it
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = UpdateRenamedCalibrator(pDBConnection, newCalib.CalibratorName, _
                                                                     pUpdateVersionChangesList)
                        End If

                        '(2.3) Save the FACTORY Calibrator in CUSTOMER DB 
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myCalibratorsDelegate.Save(pDBConnection, myFactoryCalibratorDS, New TestCalibratorsDS, _
                                                                        New TestCalibratorValuesDS, Nothing)
                        End If

                        '(2.4) Add a row in the global DS containing all changes in Customer DB due to the Update Version Process (sub-table AddedElements) 
                        '      for the added Calibrator
                        If (Not myGlobalDataTO.HasError) Then
                            myUpdateVersionAddedElementsRow = pUpdateVersionChangesList.AddedElements.NewAddedElementsRow
                            myUpdateVersionAddedElementsRow.ElementType = "CALIB"
                            myUpdateVersionAddedElementsRow.ElementName = myFactoryCalibratorDS.tparCalibrators.First.CalibratorName
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
                GlobalBase.CreateLogActivity("CALIBRATORS Update Version Error", "CalibratorUpdateData.ProcessForCALIBRATORS", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Verify if there is a Calibrator in Customer DB with the same Name of a Factory Calibrator (new or updated), and in this case, rename
        ''' the Calibrator in Customer BD by adding as many "R" letters as needed at the beginning of the Name until get an unique Name
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalibratorName">Name of the Factory Calibrator to verify</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 08/10/2014 - BA-1944 (SubTask BA-1982)
        ''' </remarks>
        Private Function UpdateRenamedCalibrator(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibratorName As String, _
                                                 ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myCustomerCalibratorDS As New CalibratorsDS
                Dim myCalibratorsDelegate As New CalibratorsDelegate
                Dim myUpdateVersionRenamedElementsRow As UpdateVersionChangesDS.RenamedElementsRow

                'Search if there is Calibrator with the same Name in Customer DB...
                myGlobalDataTO = myCalibratorsDelegate.ReadByCalibratorName(pDBConnection, pCalibratorName)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myCustomerCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, CalibratorsDS)

                    If (myCustomerCalibratorDS.tparCalibrators.Rows.Count > 0) Then
                        'Rename the Calibrator (add as many "R" letters as needed at the beginning of Name)
                        myGlobalDataTO = RenameCalibratorName(pDBConnection, myCustomerCalibratorDS.tparCalibrators.First)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            If (Convert.ToBoolean(myGlobalDataTO.SetDatos)) Then
                                'Update the renamed Calibrator in Customer DB
                                myGlobalDataTO = myCalibratorsDelegate.Save(pDBConnection, myCustomerCalibratorDS, New TestCalibratorsDS, _
                                                                            New TestCalibratorValuesDS, Nothing)

                                If (Not myGlobalDataTO.HasError) Then
                                    'Add a row in the global DS containing all changes in Customer DB due to the Update Version Process (sub-table RenamedElements)
                                    myUpdateVersionRenamedElementsRow = pUpdateVersionChangesList.RenamedElements.NewRenamedElementsRow
                                    myUpdateVersionRenamedElementsRow.ElementType = "CALIB"
                                    myUpdateVersionRenamedElementsRow.PreviousName = pCalibratorName
                                    myUpdateVersionRenamedElementsRow.UpdatedName = myCustomerCalibratorDS.tparCalibrators.First.CalibratorName
                                    pUpdateVersionChangesList.RenamedElements.AddRenamedElementsRow(myUpdateVersionRenamedElementsRow)
                                    pUpdateVersionChangesList.RenamedElements.AcceptChanges()
                                End If
                            Else
                                'If it was not possible to rename the Calibrator (really unlikely case), it is considered an error
                                myGlobalDataTO.HasError = True
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity("CALIBRATORS Update Version Error", "CalibratorUpdateData.UpdateRenamedCalibrator", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Add "R" letters at the beginning of a Calibrator Name until get an unique value. Used during Update Version process when 
        ''' an experimental Calibrator has been added or updated in FACTORY DB and there is a Calibrator with the same Name in Customer DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalibratorsRow">Row of CalibratorsDS containing the data of the Calibrator to rename</param>
        ''' <returns>GlobalDataTO containing a Boolean value indicating if the Calibrator has been renamed (when TRUE)</returns>
        ''' <remarks>
        ''' Created by:  XB 19/02/2013 - BT #1134 ==> Implement a new function to obtain a valid CalibratorName and considering DB size field limitations 
        ''' Modified by: SA 08/10/2014 - BA-1944 (SubTask BA-1982) ==> Return a Boolean value to indicate if the Calibrator has been renamed. Changes to
        '''                                                            improve the code
        ''' </remarks>
        Private Function RenameCalibratorName(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pCalibratorsRow As CalibratorsDS.tparCalibratorsRow) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myCalibratorDS As New CalibratorsDS
                Dim myCalibratorsDelegate As New CalibratorsDelegate

                Dim numOfRs As Integer = 1
                Dim errorFound As Boolean = False
                Dim isValidNewName As Boolean = False
                Dim myValidCalibName As String = pCalibratorsRow.CalibratorName

                While (Not isValidNewName AndAlso numOfRs < 16 AndAlso Not errorFound)
                    'Add an "R" at the beginning of the Calibrator Name
                    myValidCalibName = "R" & myValidCalibName

                    'If the length of the new Calibrator Name is greater than 16, remove the last character
                    If (myValidCalibName.Length > 16) Then myValidCalibName = myValidCalibName.Remove(myValidCalibName.Length - 1)

                    'Verify if the new Calibrator Name is unique in Customer DB (there is not another Calibrator with the same Name)
                    myGlobalDataTO = myCalibratorsDelegate.ReadByCalibratorName(pDBConnection, myValidCalibName)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        isValidNewName = (DirectCast(myGlobalDataTO.SetDatos, CalibratorsDS).tparCalibrators.Count = 0)
                    Else
                        errorFound = True
                    End If

                    'Just to avoid the very unlikely probability of endless loop 
                    numOfRs += 1
                End While

                'Finally, update the name in the CalibratorsDS row received as entry parameter
                If (isValidNewName) Then
                    pCalibratorsRow.BeginEdit()
                    pCalibratorsRow.CalibratorName = myValidCalibName
                    pCalibratorsRow.EndEdit()
                End If

                myGlobalDataTO.SetDatos = isValidNewName
                myGlobalDataTO.HasError = errorFound

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity("CALIBRATORS Update Version Error", "CalibratorUpdateData.RenameCalibratorName", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region
    End Class
End Namespace

#Region "FUNCTIONS FOR OLD UPDATE VERSION PROCESS (NOT USED)"
'        Inherits UpdateElementParent

'#Region " Internal Data "
'        Protected Overrides Function GetDataTable(pMyDataSet As DataSet) As DataTable
'            Return DirectCast(pMyDataSet, CalibratorsDS).tparCalibrators
'        End Function

'        ''' <summary>
'        ''' Get data from Factory DB and fill Row 
'        ''' </summary>
'        ''' <param name="pDBConnection"></param>
'        ''' <param name="pItemRow"></param>
'        ''' <returns></returns>
'        ''' <remarks>
'        ''' Created by: DL - 06/02/2013
'        ''' </remarks>
'        Protected Overrides Function GetDataInFactoryDB(pDBConnection As SqlClient.SqlConnection,
'                                                        pItemRow As DataRow) As GlobalDataTO

'            Dim myGlobalDataTO As New GlobalDataTO

'            Try
'                Dim calibRow As CalibratorsDS.tparCalibratorsRow = DirectCast(pItemRow, CalibratorsDS.tparCalibratorsRow)
'                If calibRow Is Nothing Then
'                    Return myGlobalDataTO
'                Else
'                    Dim myCalibrators As New tparCalibratorsDAO
'                    myGlobalDataTO = myCalibrators.ReadByCalibratorName(pDBConnection, calibRow.CalibratorName, True)
'                End If

'            Catch ex As Exception
'                Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity("Calibrator Update Error.", "CalibratorUpdateData.GetDataInFactoryDB", EventLogEntryType.Error, False)
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobalDataTO.ErrorMessage = ex.Message
'            End Try

'            Return myGlobalDataTO
'        End Function

'        ''' <summary>
'        ''' Get data from LocalDB and fill Row 
'        ''' </summary>
'        ''' <param name="pDBConnection"></param>
'        ''' <param name="pItemRow"></param>
'        ''' <returns></returns>
'        ''' <remarks>
'        ''' Created by: DL - 06/02/2013
'        ''' </remarks>
'        Protected Overrides Function GetDataInLocalDB(pDBConnection As SqlClient.SqlConnection,
'                                                      pItemRow As DataRow) As GlobalDataTO

'            Dim myGlobalDataTO As New GlobalDataTO

'            Try
'                Dim calibRow As CalibratorsDS.tparCalibratorsRow = DirectCast(pItemRow, CalibratorsDS.tparCalibratorsRow)
'                If calibRow Is Nothing Then
'                    Return myGlobalDataTO
'                Else
'                    Dim myCalibrators As New tparCalibratorsDAO
'                    myGlobalDataTO = myCalibrators.ReadByCalibratorName(pDBConnection, calibRow.CalibratorName, False)
'                End If

'            Catch ex As Exception
'                Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity("Calibrator Update Error.", "CalibratorUpdateData.GetDataInLocalDB", EventLogEntryType.Error, False)
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobalDataTO.ErrorMessage = ex.Message
'            End Try

'            Return myGlobalDataTO
'        End Function

'        ''' <summary>
'        ''' Is item defined by user?
'        ''' </summary>
'        ''' <param name="pDataInDB"></param>
'        ''' <returns></returns>
'        ''' <remarks>
'        ''' Created by: DL - 06/02/2013
'        ''' modified by: TR 08/05/2013 add the connection.
'        ''' </remarks>
'        Protected Overrides Function IsFactoryDefinedItem(pDataInDB As Object, pDBConnection As SqlClient.SqlConnection) As Boolean
'            ' Try
'            'Dim calibratorRow As CalibratorsDS.tparCalibratorsRow = DirectCast(pDataInDB, CalibratorsDS.tparCalibratorsRow)
'            'If calibratorRow Is Nothing Then Return False

'            'Return False 

'            'Catch ex As Exception
'            'Dim myLogAcciones As New ApplicationLogManager()
'            'GlobalBase.CreateLogActivity("Calibrators Update Error.", "CalibratorUpdateData.IsFactoryDefinedItem", EventLogEntryType.Error, False)
'            'End Try

'            Return False
'        End Function
'#End Region

'#Region " Factory Updates "
'        ''' <summary>
'        ''' Performs update actions related to modified data in Factory DB: inserts and updates in local DB
'        ''' </summary>
'        ''' <param name="pDBConnection"></param>
'        ''' <returns></returns>
'        ''' <remarks>
'        ''' Created by: DL - 29/01/2013
'        ''' </remarks>
'        Protected Overrides Function GetAffectedItemsFromFactoryUpdates(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
'            Dim myLogAcciones As New ApplicationLogManager()
'            Dim myGlobalDataTO As New GlobalDataTO

'            Try

'                myGlobalDataTO = CalibratorUpdateDAO.GetCalibratorsDistinctInClient(pDBConnection)

'            Catch ex As Exception
'                GlobalBase.CreateLogActivity("Calibrators Update Error.", "CalibratorUpdateData.GetAffectedItemsFromFactoryUpdates", EventLogEntryType.Error, False)
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobalDataTO.ErrorMessage = ex.Message
'            End Try

'            Return myGlobalDataTO

'        End Function

'        ''' <summary>
'        ''' Create new calibator like in Factory DB
'        ''' </summary>
'        ''' <param name="pDBConnection"></param>
'        ''' <param name="pDataInFactoryDB"></param>
'        ''' <param name="pDataInLocalDB"></param>
'        ''' <returns></returns>
'        ''' <remarks>
'        ''' Created by: DL - 29/01/2013
'        ''' </remarks>
'        Protected Overrides Function DoCreateNewElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
'            Dim myGlobalDataTO As New GlobalDataTO
'            Try
'                Dim myCalibratorsDS As CalibratorsDS = DirectCast(pDataInFactoryDB, CalibratorsDS)
'                If myCalibratorsDS Is Nothing Then Return myGlobalDataTO

'                For Each calibRow As CalibratorsDS.tparCalibratorsRow In myCalibratorsDS.tparCalibrators
'                    calibRow.BeginEdit()
'                    calibRow.IsNew = True
'                    calibRow.EndEdit()
'                Next

'                Dim myCalibratorsDelegate As New CalibratorsDelegate
'                myGlobalDataTO = myCalibratorsDelegate.Save(pDBConnection,
'                                                            myCalibratorsDS,
'                                                            New TestCalibratorsDS,
'                                                            New TestCalibratorValuesDS,
'                                                            Nothing,
'                                                            False,
'                                                            False,
'                                                            False,
'                                                            "",
'                                                            "")

'            Catch ex As Exception
'                Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity("Calibrators Update Error.", "CalibratorUpdateData.DoCreateNewElement", EventLogEntryType.Error, False)
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobalDataTO.ErrorMessage = ex.Message
'            End Try

'            Return myGlobalDataTO
'        End Function

'        ''' <summary>
'        ''' DoUpdateUserDefinedElement
'        ''' </summary>
'        ''' <param name="pDBConnection"></param>
'        ''' <param name="pDataInFactoryDB"></param>
'        ''' <param name="pDataInLocalDB"></param>
'        ''' <returns></returns>
'        ''' <remarks>
'        ''' Created by:  DL - 29/01/2013
'        ''' Modified by: XB - 15/02/2013 - Correction : implement a new function to obtain a valid calibrator renaming also watching database size field limitations (Bugs tracking #1134)
'        ''' </remarks>
'        Protected Overrides Function DoUpdateUserDefinedElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
'            Dim myGlobalDataTO As New GlobalDataTO
'            Try
'                If pDataInLocalDB Is Nothing Then Return myGlobalDataTO

'                'Dim calibratorsRowLocal As CalibratorsDS.tparCalibratorsRow = DirectCast(DirectCast(pDataInLocalDB, CalibratorsDS).tparCalibrators.First, CalibratorsDS.tparCalibratorsRow)
'                'If calibratorsRowLocal Is Nothing Then Return myGlobalDataTO

'                ''Rename calibrator name adding (R) at the beginning of the name validting that not exist in db
'                'Dim IsOK As Boolean = False
'                'Dim myNewname As String = ""
'                'Dim myCalibrators As New tparCalibratorsDAO

'                'While Not IsOK
'                '    myNewname &= "R" & calibratorsRowLocal.CalibratorName

'                '    myGlobalDataTO = myCalibrators.ReadByCalibratorName(pDBConnection, myNewname)

'                '    If Not myGlobalDataTO.HasError Then
'                '        Dim myCalibratorDS As New CalibratorsDS
'                '        myCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, CalibratorsDS)

'                '        If myCalibratorDS.tparCalibrators.Rows.Count = 0 Then IsOK = True
'                '    End If

'                'End While

'                ''Rename calibrator in client database. Adding (R) at the beginning of the name
'                Dim myCalibratorsDS As CalibratorsDS = DirectCast(pDataInLocalDB, CalibratorsDS)
'                If myCalibratorsDS Is Nothing Then Return myGlobalDataTO

'                'For Each calibRow As CalibratorsDS.tparCalibratorsRow In myCalibratorsDS.tparCalibrators
'                '    calibRow.BeginEdit()
'                '    calibRow.CalibratorName = myNewname
'                '    calibRow.IsNew = False
'                '    calibRow.EndEdit()
'                'Next

'                myGlobalDataTO = MyClass.RenameCalibratorName(pDBConnection, myCalibratorsDS.tparCalibrators.First)
'                If Not myGlobalDataTO.HasError Then

'                    Dim myCalibratorsDelegate As New CalibratorsDelegate
'                    myGlobalDataTO = myCalibratorsDelegate.Save(pDBConnection,
'                                                                myCalibratorsDS,
'                                                                New TestCalibratorsDS,
'                                                                New TestCalibratorValuesDS, _
'                                                                Nothing,
'                                                                False,
'                                                                False,
'                                                                False,
'                                                                "",
'                                                                "")

'                    'Create factory calibrator 
'                    If Not myGlobalDataTO.HasError Then
'                        myCalibratorsDS = DirectCast(pDataInFactoryDB, CalibratorsDS)
'                        If myCalibratorsDS Is Nothing Then Return myGlobalDataTO

'                        For Each calibRow As CalibratorsDS.tparCalibratorsRow In myCalibratorsDS.tparCalibrators
'                            calibRow.BeginEdit()
'                            calibRow.IsNew = True
'                            calibRow.EndEdit()
'                        Next

'                        myGlobalDataTO = myCalibratorsDelegate.Save(pDBConnection,
'                                                                    myCalibratorsDS,
'                                                                    New TestCalibratorsDS,
'                                                                    New TestCalibratorValuesDS, _
'                                                                    Nothing,
'                                                                    False,
'                                                                    False,
'                                                                    False, _
'                                                                    "",
'                                                                    "")
'                    End If

'                End If

'            Catch ex As Exception
'                Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity("Calibrators Update Error.", "CalibratorUpdateData.DoUpdateUserDefinedElement", EventLogEntryType.Error, False)
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobalDataTO.ErrorMessage = ex.Message
'            End Try

'            Return myGlobalDataTO
'        End Function

'        Protected Overrides Function DoUpdateFactoryDefinedElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
'            'No action is needed
'            Return New GlobalDataTO
'        End Function

'        Protected Overrides Function DoIgnoreElementUpdating(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
'            'No action is needed
'            Return New GlobalDataTO
'        End Function
'#End Region

'#Region " Factory Removes "
'        Protected Overrides Function GetAffectedItemsFromFactoryRemoves(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
'            Dim myGlobalDataTO As New GlobalDataTO
'            Try
'                myGlobalDataTO = CalibratorUpdateDAO.GetCalibratorsDistinctInFactory(pDBConnection)

'            Catch ex As Exception
'                Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity("Calibrators Update Error.", "CalibratorUpdateData.GetAffectedItemsFromFactoryRemoves", EventLogEntryType.Error, False)
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobalDataTO.ErrorMessage = ex.Message
'            End Try

'            Return myGlobalDataTO
'        End Function

'        Protected Overrides Function DoRemoveUserElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
'            'No action is needed
'            Return New GlobalDataTO
'        End Function

'        Protected Overrides Function DoRemoveFactoryElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
'            Dim myGlobalDataTO As New GlobalDataTO
'            Try
'                Dim calibratorRowLocal As CalibratorsDS.tparCalibratorsRow = DirectCast(pDataInLocalDB, CalibratorsDS.tparCalibratorsRow)
'                If calibratorRowLocal Is Nothing Then Return myGlobalDataTO

'                'Close calibrator in Historical module
'                'Remove test in Local DB

'            Catch ex As Exception
'                Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity("Calibrators Update Error.", "CalibratorUpdateData.DoRemoveFactoryElement", EventLogEntryType.Error, False)
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobalDataTO.ErrorMessage = ex.Message
'            End Try

'            Return myGlobalDataTO
'        End Function

'        Protected Overrides Function DoIgnoreElementRemoving(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
'            'No action is needed
'            Return New GlobalDataTO
'        End Function

'        Protected Overrides Function IsDataEmpty(pDataInDB As Object) As Boolean
'            Dim myGlobalDataTO As New GlobalDataTO
'            Dim returndata As Boolean = True

'            Try
'                If Not pDataInDB Is Nothing Then
'                    Dim myCalibrators As CalibratorsDS = DirectCast(pDataInDB, CalibratorsDS)
'                    If myCalibrators.tparCalibrators.Count > 0 Then returndata = False
'                End If


'            Catch ex As Exception
'                Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity("Calibrators Update Error.", "CalibratorUpdateData.IsDataEmpty", EventLogEntryType.Error, False)
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobalDataTO.ErrorMessage = ex.Message
'            End Try

'            Return returndata
'        End Function
'#End Region
#End Region

