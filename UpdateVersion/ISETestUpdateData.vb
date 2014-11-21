Option Explicit On
Option Strict On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.BL.UpdateVersion
    Public Class ISETestUpdateData

#Region "FUNCTIONS FOR NEW UPDATE VERSION PROCESS"
        ''' <summary>
        ''' Execute the process to search in FACTORY DB all ISE TESTS that exists in CUSTOMER DB but for which at least one of the relevant Test fields 
        ''' have changed and modify data in CUSTOMER DB (tables tparISETests and tparISETestSamples)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 15/10/2014 - BA-1944 (SubTask BA-2013)
        ''' </remarks>
        Public Function UPDATEModifiedISETestSamples(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myISETestUpdateDAO As New ISETestUpdateDAO
                Dim myFactoryISETestSamplesDS As New ISETestSamplesDS

                '(1) Search in Factory DB all ISE TESTS/SAMPLE TYPES for which at least one of the relevant fields have changed
                myGlobalDataTO = myISETestUpdateDAO.GetUpdatedFactoryISETestSamples(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myFactoryISETestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, ISETestSamplesDS)

                    '(1.1) Search changes in fields of tparISETests table and update the affected ISE Tests in CUSTOMER DB
                    myGlobalDataTO = UpdateISETests(pDBConnection, myFactoryISETestSamplesDS, pUpdateVersionChangesList)

                    '(1.2) Search changes in fields of tparISETestSamples table and update the affected ISE Test/Sample Types in CUSTOMER DB
                    If (Not myGlobalDataTO.HasError) Then
                        myGlobalDataTO = UpdateISETestSamples(pDBConnection, myFactoryISETestSamplesDS, pUpdateVersionChangesList)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("ISE Test Update Error", "ISETestUpdateData.UPDATEModifiedISETestSamples", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update modified ISETests in CUSTOMER DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pFactoryISETestSamplesDS">ISETestSamplesDS containing all ISETest/SampleTypes with changes in FACTORY DB</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 15/10/2014 - BA-1944 (SubTask BA-2013)
        ''' </remarks>
        Private Function UpdateISETests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pFactoryISETestSamplesDS As ISETestSamplesDS, _
                                        ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim executeUpdate As Boolean = False
                Dim myUnits As String = String.Empty
                Dim myISEUnits As String = String.Empty
                Dim myISEResultID As String = String.Empty
                Dim myISETestName As String = String.Empty

                Dim myCustomerISETestDS As New ISETestsDS
                Dim myISETestsDelegate As New ISETestsDelegate

                'Get the list of different ISETestIDs in the DS containing all changes in FACTORY DB
                Dim affectedISETestsList As List(Of Integer) = (From a In pFactoryISETestSamplesDS.tparISETestSamples _
                                                              Select a.ISETestID Distinct).ToList

                'For each different ISETestID, verify if fields ISE_Units and ISE_ResultID have changed, and in this case, update them in CUSTOMER DB
                For Each myISETestID As Integer In affectedISETestsList
                    executeUpdate = False

                    'Get all data of the ISE Test in CUSTOMER DB
                    myGlobalDataTO = myISETestsDelegate.Read(pDBConnection, myISETestID)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myCustomerISETestDS = DirectCast(myGlobalDataTO.SetDatos, ISETestsDS)

                        If (myCustomerISETestDS.tparISETests.Rows.Count > 0) Then
                            'Build the Test Name for the update version changes structure
                            myISETestName = myCustomerISETestDS.tparISETests.First.Name & " (" & myCustomerISETestDS.tparISETests.First.ShortName & ")"

                            'Get value of fields ISE_Units, Units and ISE_ResultID for the ISE Test in FACTORY DB
                            myUnits = pFactoryISETestSamplesDS.tparISETestSamples.ToList.Where(Function(a) a.ISETestID = myISETestID).First.MeasureUnit
                            myISEUnits = pFactoryISETestSamplesDS.tparISETestSamples.ToList.Where(Function(a) a.ISETestID = myISETestID).First.ISE_Units
                            myISEResultID = pFactoryISETestSamplesDS.tparISETestSamples.ToList.Where(Function(a) a.ISETestID = myISETestID).First.ISE_ResultID

                            'Verify if field Units has changed 
                            If (myCustomerISETestDS.tparISETests.First.Units <> myUnits) Then
                                'Add a row for field Units in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                                AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "ISE", myISETestName, String.Empty, "Units", _
                                                                    myCustomerISETestDS.tparISETests.First.Units, myUnits)

                                'Update the field in the Customer DS
                                executeUpdate = True
                                myCustomerISETestDS.tparISETests.First.Units = myUnits
                            End If

                            'Verify if field ISE_Units has changed 
                            If (myCustomerISETestDS.tparISETests.First.ISE_Units <> myISEUnits) Then
                                'Add a row for field ISE_Units in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                                AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "ISE", myISETestName, String.Empty, "ISE_Units", _
                                                                    myCustomerISETestDS.tparISETests.First.ISE_Units, myISEUnits)

                                'Update the field in the Customer DS
                                executeUpdate = True
                                myCustomerISETestDS.tparISETests.First.ISE_Units = myISEUnits
                            End If

                            'Verify if field ISE_ResultID has changed 
                            If (myCustomerISETestDS.tparISETests.First.ISE_ResultID <> myISEResultID) Then
                                'Add a row for field ISE_ResultID in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                                AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "ISE", myISETestName, String.Empty, "ISE_ResultID", _
                                                                    myCustomerISETestDS.tparISETests.First.ISE_ResultID, myISEResultID)

                                'Update the field in the Customer DS
                                executeUpdate = True
                                myCustomerISETestDS.tparISETests.First.ISE_ResultID = myISEResultID
                            End If

                            'If field LIS Value is empty in CUSTOMER DB, set its value to NULL (due to the NULL value read from DB is changed to 
                            'an empty string when it is loaded in the DataSet)
                            If (Not myCustomerISETestDS.tparISETests.First.IsLISValueNull AndAlso myCustomerISETestDS.tparISETests.First.LISValue = String.Empty) Then
                                myCustomerISETestDS.tparISETests.First.SetLISValueNull()
                            End If

                            'If at least one of the relevant fields have changed, then update the ISETest in CUSTOMER DB
                            If (executeUpdate) Then
                                myCustomerISETestDS.tparISETests.AcceptChanges()
                                myGlobalDataTO = myISETestsDelegate.Modify(pDBConnection, myCustomerISETestDS)
                            End If
                        Else
                            'This case is not possible...
                            myGlobalDataTO.HasError = True
                            Exit For
                        End If
                    End If

                    'If an error has been raised, the update process is stopped
                    If (myGlobalDataTO.HasError) Then Exit For
                Next
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("ISE Test Update Error", "ISETestUpdateData.UpdateISETests", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update modified ISETest/SampleTypes in CUSTOMER DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pFactoryISETestSamplesDS">ISETestSamplesDS containing all ISETest/SampleTypes with changes in FACTORY DB</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 15/10/2014 - BA-1944 (SubTask BA-2013)
        ''' </remarks>
        Private Function UpdateISETestSamples(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pFactoryISETestSamplesDS As ISETestSamplesDS, _
                                              ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim executeUpdate As Boolean = False
                Dim myISEVolume As String = String.Empty
                Dim myISEDilFactor As String = String.Empty
                Dim myISETestName As String = String.Empty

                Dim myCustomerISETestSampleDS As New ISETestSamplesDS
                Dim myISETestSamplesDelegate As New ISETestSamplesDelegate

                'For each different ISE Test/SampleType, verify changed fields and execute the update in CUSTOMER DB
                For Each updatedISETestSample As ISETestSamplesDS.tparISETestSamplesRow In pFactoryISETestSamplesDS.tparISETestSamples
                    executeUpdate = False

                    'Get data of the ISETest/SampleType in CUSTOMER DB
                    myGlobalDataTO = myISETestSamplesDelegate.Read(pDBConnection, updatedISETestSample.ISETestID, updatedISETestSample.SampleType)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myCustomerISETestSampleDS = DirectCast(myGlobalDataTO.SetDatos, ISETestSamplesDS)

                        If (myCustomerISETestSampleDS.tparISETestSamples.Rows.Count > 0) Then
                            'Build the Test Name for the update version changes structure
                            myISETestName = myCustomerISETestSampleDS.tparISETestSamples.First.ISETestName & " (" & myCustomerISETestSampleDS.tparISETestSamples.First.ISETestShortName & ")"

                            'Verify if field SampleType_ResultID has changed
                            If (myCustomerISETestSampleDS.tparISETestSamples.First.SampleType_ResultID <> updatedISETestSample.SampleType_ResultID) Then
                                'Add a row for field SampleType_ResultID in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                                AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "ISE", myISETestName, updatedISETestSample.SampleType, "SampleType_ResultID", _
                                                                    myCustomerISETestSampleDS.tparISETestSamples.First.SampleType_ResultID, updatedISETestSample.SampleType_ResultID)

                                'Update the field in the Customer DS
                                executeUpdate = True
                                myCustomerISETestSampleDS.tparISETestSamples.First.SampleType_ResultID = updatedISETestSample.SampleType_ResultID
                            End If

                            'Verify if field ISE_Volume has changed
                            myISEVolume = "--"
                            If (Not updatedISETestSample.IsISE_VolumeNull) Then
                                If (Not myCustomerISETestSampleDS.tparISETestSamples.First.IsISE_VolumeNull) Then myISEVolume = myCustomerISETestSampleDS.tparISETestSamples.First.ISE_Volume.ToString

                                If (myCustomerISETestSampleDS.tparISETestSamples.First.IsISE_VolumeNull) OrElse _
                                   (myCustomerISETestSampleDS.tparISETestSamples.First.ISE_Volume <> updatedISETestSample.ISE_Volume) Then
                                    'Add a row for field ISE_Volume in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                                    AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "ISE", myISETestName, updatedISETestSample.SampleType, "ISE_Volume", _
                                                                        myISEVolume, updatedISETestSample.ISE_Volume.ToString)

                                    'Update the field in the Customer DS
                                    executeUpdate = True
                                    myCustomerISETestSampleDS.tparISETestSamples.First.ISE_Volume = updatedISETestSample.ISE_Volume
                                End If
                            Else
                                If (Not myCustomerISETestSampleDS.tparISETestSamples.First.IsISE_VolumeNull) Then
                                    'Add a row for field ISE_Volume in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                                    AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "ISE", myISETestName, updatedISETestSample.SampleType, "ISE_Volume", _
                                                                        myCustomerISETestSampleDS.tparISETestSamples.First.ISE_Volume.ToString, myISEVolume)

                                    'Update the field in the Customer DS
                                    executeUpdate = True
                                    myCustomerISETestSampleDS.tparISETestSamples.First.SetISE_VolumeNull()
                                End If
                            End If

                            'Verify if field ISE_DilutionFactor has changed
                            myISEDilFactor = "--"
                            If (Not updatedISETestSample.IsISE_DilutionFactorNull) Then
                                If (Not myCustomerISETestSampleDS.tparISETestSamples.First.IsISE_DilutionFactorNull) Then myISEDilFactor = myCustomerISETestSampleDS.tparISETestSamples.First.ISE_DilutionFactor.ToString

                                If (myCustomerISETestSampleDS.tparISETestSamples.First.IsISE_DilutionFactorNull) OrElse _
                                   (myCustomerISETestSampleDS.tparISETestSamples.First.ISE_DilutionFactor <> updatedISETestSample.ISE_DilutionFactor) Then
                                    'Add a row for field ISE_Volume in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                                    AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "ISE", myISETestName, updatedISETestSample.SampleType, "ISE_DilutionFactor", _
                                                                        myISEDilFactor, updatedISETestSample.ISE_DilutionFactor.ToString)

                                    'Update the field in the Customer DS
                                    executeUpdate = True
                                    myCustomerISETestSampleDS.tparISETestSamples.First.ISE_DilutionFactor = updatedISETestSample.ISE_DilutionFactor
                                End If
                            Else
                                If (Not myCustomerISETestSampleDS.tparISETestSamples.First.IsISE_DilutionFactorNull) Then
                                    'Add a row for field ISE_Volume in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                                    AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "ISE", myISETestName, updatedISETestSample.SampleType, "ISE_DilutionFactor", _
                                                                        myCustomerISETestSampleDS.tparISETestSamples.First.ISE_DilutionFactor.ToString, myISEDilFactor)

                                    'Update the field in the Customer DS
                                    executeUpdate = True
                                    myCustomerISETestSampleDS.tparISETestSamples.First.SetISE_DilutionFactorNull()
                                End If
                            End If

                            'If field ActiveRangeType is empty in CUSTOMER DB, set its value to NULL (due to the NULL value read from DB is changed to 
                            'an empty string when it is loaded in the DataSet)
                            If (Not myCustomerISETestSampleDS.tparISETestSamples.First.IsActiveRangeTypeNull AndAlso _
                                myCustomerISETestSampleDS.tparISETestSamples.First.ActiveRangeType = String.Empty) Then
                                myCustomerISETestSampleDS.tparISETestSamples.First.SetActiveRangeTypeNull()
                            End If

                            'If field CalculationMode is empty in CUSTOMER DB, set its value to NULL (due to the NULL value read from DB is changed to 
                            'an empty string when it is loaded in the DataSet)
                            If (Not myCustomerISETestSampleDS.tparISETestSamples.First.IsCalculationModeNull AndAlso _
                                myCustomerISETestSampleDS.tparISETestSamples.First.CalculationMode = String.Empty) Then
                                myCustomerISETestSampleDS.tparISETestSamples.First.SetCalculationModeNull()
                            End If

                            'If field TestLongName is empty in CUSTOMER DB, set its value to NULL (due to the NULL value read from DB is changed to 
                            'an empty string when it is loaded in the DataSet)
                            If (Not myCustomerISETestSampleDS.tparISETestSamples.First.IsTestLongNameNull AndAlso _
                                myCustomerISETestSampleDS.tparISETestSamples.First.TestLongName = String.Empty) Then
                                myCustomerISETestSampleDS.tparISETestSamples.First.SetTestLongNameNull()
                            End If

                            'If at least one of the relevant fields have changed, then update the ISETest/SampleType in CUSTOMER DB
                            If (executeUpdate) Then
                                myCustomerISETestSampleDS.tparISETestSamples.AcceptChanges()
                                myGlobalDataTO = myISETestSamplesDelegate.Modify(pDBConnection, myCustomerISETestSampleDS)
                            End If
                        Else
                            'This case is not possible...
                            myGlobalDataTO.HasError = True
                            Exit For
                        End If
                    End If

                    'If an error has been raised, the update process is stopped
                    If (myGlobalDataTO.HasError) Then Exit For
                Next
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("ISE Test Update Error", "ISETestUpdateData.UpdateISETestSamples", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Add a new row with data of an updated element in the DS containing all changes in CUSTOMER DB due to the Update Version Process.
        ''' Data is added in sub-table UpdatedElements
        ''' </summary>
        ''' <param name="pUpdateVersionChangesList">DataSet containing all changes made in CUSTOMER DB for the Update Version Process</param>
        ''' <param name="pElementType">Type of affected Element: ISE</param>
        ''' <param name="pElementName">Name of the affected Element</param>
        ''' <param name="pUpdatedField">Name of the affected field</param>
        ''' <param name="pPreviousValue">Previous value of the field in CUSTOMER DB</param>
        ''' <param name="pNewValue">New value of the field in CUSTOMER DB (from FACTORY DB)</param>
        ''' <remarks>
        ''' Created by:  SA 15/10/2014 - BA-1944 (SubTask BA-2013)
        ''' </remarks>
        Private Sub AddUpdatedElementToChangesStructure(ByRef pUpdateVersionChangesList As UpdateVersionChangesDS, ByVal pElementType As String, _
                                                        ByVal pElementName As String, ByVal pSampleType As String, ByVal pUpdatedField As String, ByVal pPreviousValue As String, _
                                                        ByVal pNewValue As String)
            Try
                Dim myUpdateVersionChangedElementsRow As UpdateVersionChangesDS.UpdatedElementsRow
                myUpdateVersionChangedElementsRow = pUpdateVersionChangesList.UpdatedElements.NewUpdatedElementsRow
                myUpdateVersionChangedElementsRow.ElementType = pElementType
                myUpdateVersionChangedElementsRow.ElementName = pElementName
                myUpdateVersionChangedElementsRow.SampleType = pSampleType
                myUpdateVersionChangedElementsRow.UpdatedField = pUpdatedField
                myUpdateVersionChangedElementsRow.PreviousValue = pPreviousValue
                myUpdateVersionChangedElementsRow.NewValue = pNewValue
                pUpdateVersionChangesList.UpdatedElements.AddUpdatedElementsRow(myUpdateVersionChangedElementsRow)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("ISE Test Update Error", "ISETestUpdateData.AddUpdatedElementToChangesStructure", EventLogEntryType.Error, False)
                Throw
            End Try
        End Sub
#End Region
    End Class
End Namespace

#Region "FUNCTIONS FOR OLD UPDATE VERSION PROCESS (NOT USED)"
'Inherits UpdateElementParent

' ''' <summary>
' ''' Gets the DataTable inside the DataSet: the result of comparation of DataBases
' ''' </summary>
' ''' <param name="pMyDataSet"></param>
' ''' <returns></returns>
' ''' <remarks></remarks>
'Protected Overrides Function GetDataTable(pMyDataSet As DataSet) As DataTable
'    'Return DirectCast(pMyDataSet, ISETestsDS).tparISETests
'    Return pMyDataSet.Tables(0)
'End Function

' ''' <summary>
' ''' Contains all related Data in a ISE Test
' ''' </summary>
' ''' <remarks>
' ''' Created By: DL - 05/02/2013
' ''' </remarks>
'Protected Structure DataInDB
'    Public myISETestsDS As ISETestsDS
'    Public myRefRangsDS As TestRefRangesDS

'    ''' <summary>
'    ''' Fill sie Test Data Set in structure, searching by ise Test Name
'    ''' </summary>
'    ''' <param name="pDBConnection"></param>
'    ''' <param name="pISETestId"></param>
'    ''' <returns></returns>
'    ''' <remarks>
'    ''' Created By: SG 14/02/2013 - Bug #1134
'    ''' </remarks>
'    Public Function SearchISETestById(pDBConnection As SqlClient.SqlConnection,
'                                        pISETestId As Integer,
'                                        Optional factoryDB As Boolean = False) As Boolean

'        Dim dbConnection As SqlClient.SqlConnection = Nothing
'        myISETestsDS = New ISETestsDS

'        Try
'            Dim resultData As GlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
'            If resultData.HasError Then Return False
'            If resultData.SetDatos Is Nothing Then Return False

'            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
'            If dbConnection Is Nothing Then Return False

'            Dim dbName As String = ""
'            If factoryDB Then dbName = GlobalBase.TemporalDBName

'            Dim mytparISETests As New tparISETestsDAO
'            resultData = mytparISETests.Read(dbConnection, pISETestId, factoryDB)

'            If resultData.HasError Then Return False
'            If resultData.SetDatos Is Nothing Then Return False

'            'load data to structure member
'            MyClass.myISETestsDS = DirectCast(resultData.SetDatos, ISETestsDS)
'            Return True

'        Catch ex As Exception
'            Dim myLogAcciones As New ApplicationLogManager()
'            myLogAcciones.CreateLogActivity(ex.Message, "ISETestUpdateData.SearchISETestById", EventLogEntryType.Error, False)
'        Finally
'            If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
'        End Try
'        Return False
'    End Function

'    ''' <summary>
'    ''' Fill TestRefRangesDS in structure, searching by ise Test identifier
'    ''' </summary>
'    ''' <param name="pDBConnection"></param>
'    ''' <param name="pISETestId"></param>
'    ''' <returns></returns>
'    ''' <remarks>
'    ''' Created By: DL - 06/02/2013
'    ''' </remarks>
'    Public Function SearchRefRangesByISETestId(pDBConnection As SqlClient.SqlConnection,
'                                                pISETestId As Integer,
'                                                Optional factoryDB As Boolean = False) As Boolean

'        Dim dbConnection As SqlClient.SqlConnection = Nothing

'        myRefRangsDS = New TestRefRangesDS()
'        Try
'            Dim resultData As GlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
'            If resultData.HasError Then Return False
'            If resultData.SetDatos Is Nothing Then Return False

'            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
'            If dbConnection Is Nothing Then Return False

'            Dim dbName As String = ""
'            If factoryDB Then dbName = GlobalBase.TemporalDBName

'            Dim myDAO As New tparTestRefRangesDAO
'            resultData = myDAO.ReadByTestID(dbConnection, pISETestId, , , "ISE", pDataBaseName:=dbName)
'            If resultData.HasError Then Return False
'            If resultData.SetDatos Is Nothing Then Return False

'            myRefRangsDS = DirectCast(resultData.SetDatos, TestRefRangesDS)
'            Return True

'        Catch ex As Exception
'            Dim myLogAcciones As New ApplicationLogManager()
'            myLogAcciones.CreateLogActivity(ex.Message, "ISETestUpdateData.SearchRefRangesByISETestId", EventLogEntryType.Error, False)
'        Finally
'            If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
'        End Try
'        Return False
'    End Function

'    ''' <summary>
'    ''' Returns if represented Data is Empty
'    ''' </summary>
'    ''' <returns></returns>
'    ''' <remarks>
'    ''' Created by: JB - 30/01/2013
'    ''' </remarks>
'    Public Function IsDataEmpty() As Boolean
'        Return myISETestsDS Is Nothing OrElse myISETestsDS.Tables.Count = 0 OrElse myISETestsDS.tparISETests.Rows.Count = 0
'    End Function
'End Structure

' ''' <summary>
' ''' Gets the structured data in a dataBase, from the result of comparation of DataBases
' ''' </summary>
' ''' <param name="pDBConnection"></param>
' ''' <param name="pItemRow">Row of a result of comparation of Client and Factory DataBases</param>
' ''' <param name="factoryDB">Determine which databases use to get data: Factory DB or Client DB</param>
' ''' <returns>
' ''' A GlobalDataTO with a DataInDB structure inside
' ''' </returns>
' ''' <remarks>
' ''' Created by: DL 06/02/2013
' ''' Modified by: SG 14/02/2013 Bug #1134
' ''' </remarks>
'Private Function GetDataInDB(pDBConnection As SqlClient.SqlConnection,
'                             pItemRow As DataRow,
'                             factoryDB As Boolean) As GlobalDataTO

'    Dim myGlobalDataTO As New GlobalDataTO

'    Try
'        Dim dataDB As New DataInDB()
'        Dim bRet As Boolean

'        'SG 14/02/2013 Bug #1134
'        'Dim iseTestName As String = pItemRow(0).ToString()
'        'If String.IsNullOrEmpty(iseTestName) Then Return myGlobalDataTO


'        Dim iseTestId As Integer = CInt(pItemRow(0))
'        If iseTestId <= 0 Then Return myGlobalDataTO


'        'bRet = dataDB.SearchISETestByName(pDBConnection, iseTestName, factoryDB)SG 14/02/2013 Bug #1134
'        bRet = dataDB.SearchISETestById(pDBConnection, iseTestId, factoryDB)
'        If Not bRet OrElse dataDB.myISETestsDS.tparISETests.Rows.Count = 0 Then Return myGlobalDataTO

'        'Dim iseTestId As Integer = DirectCast(dataDB.myISETestsDS.tparISETests.Rows(0), ISETestsDS.tparISETestsRow).ISETestID

'        bRet = dataDB.SearchRefRangesByISETestId(pDBConnection, iseTestId, factoryDB)
'        If Not bRet Then Return myGlobalDataTO

'        myGlobalDataTO.SetDatos = dataDB

'    Catch ex As Exception
'        Dim myLogAcciones As New ApplicationLogManager()
'        myLogAcciones.CreateLogActivity("ISE Test Update Error.", "ISETestUpdateData.GetDataInDB", EventLogEntryType.Error, False)
'        myGlobalDataTO.HasError = True
'        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'        myGlobalDataTO.ErrorMessage = ex.Message
'    End Try

'    Return myGlobalDataTO
'End Function

'Protected Overrides Function DoCreateNewElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
'    'No action is needed
'    Return New GlobalDataTO
'End Function

'Protected Overrides Function DoIgnoreElementRemoving(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
'    'No action is needed
'    Return New GlobalDataTO
'End Function

'Protected Overrides Function DoIgnoreElementUpdating(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
'    'No action is needed
'    Return New GlobalDataTO
'End Function

'Protected Overrides Function DoRemoveFactoryElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
'    'No action is needed
'    Return New GlobalDataTO
'End Function

'Protected Overrides Function DoRemoveUserElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
'    'No action is needed
'    Return New GlobalDataTO
'End Function

' ''' <summary>
' ''' Update preloaded ISE Test
' ''' </summary>
' ''' <param name="pDBConnection"></param>
' ''' <param name="pDataInFactoryDB"></param>
' ''' <param name="pDataInLocalDB"></param>
' ''' <returns></returns>
' ''' <remarks>
' ''' Created by:   DL 07/02/2013
' ''' Modified by: SGM 14/02/2013 Bug #1134
' '''               XB 19/09/2014 - Preserve the now editable by User field Test ISE Name - BA-1865
' ''' </remarks>
'Protected Overrides Function DoUpdateFactoryDefinedElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
'    Dim myGlobalDataTO As New GlobalDataTO
'    Try
'        'SGM 14/02/2013 Bug #1134
'        Dim myIseTestClient As New ISETestsDS
'        myIseTestClient = DirectCast(pDataInLocalDB, DataInDB).myISETestsDS

'        Dim myIseTestFactory As New ISETestsDS
'        myIseTestFactory = DirectCast(pDataInFactoryDB, DataInDB).myISETestsDS

'        'Create ISETestSamples DataSets needed for the Save method
'        Dim myISETestSampleDelegate As New ISETestSamplesDelegate
'        Dim ISETestsSamplesDS As New ISETestSamplesDS

'        'TR 18/02/2013 -Send the connection object
'        myGlobalDataTO = myISETestSampleDelegate.GetListByISETestID(pDBConnection, myIseTestFactory.tparISETests.First.ISETestID, myIseTestFactory.tparISETests.First.SampleType)
'        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
'            ISETestsSamplesDS = CType(myGlobalDataTO.SetDatos, ISETestSamplesDS)
'        End If

'        'Create refranges Datasets needed for the Save method
'        Dim newRefRangesDS As New TestRefRangesDS
'        Dim updatedRefRangesDS As New TestRefRangesDS
'        Dim deletedRefRangesDS As New TestRefRangesDS

'        Dim myFactoryRefRanges As New TestRefRangesDS
'        myFactoryRefRanges = DirectCast(pDataInFactoryDB, DataInDB).myRefRangsDS

'        Dim myLocalRefRanges As New TestRefRangesDS
'        myLocalRefRanges = DirectCast(pDataInLocalDB, DataInDB).myRefRangsDS

'        If myFactoryRefRanges.tparTestRefRanges.Count > myLocalRefRanges.tparTestRefRanges.Count Then
'            newRefRangesDS = myFactoryRefRanges

'        ElseIf myFactoryRefRanges.tparTestRefRanges.Count = myLocalRefRanges.tparTestRefRanges.Count AndAlso myLocalRefRanges.tparTestRefRanges.Count > 0 Then
'            updatedRefRangesDS = myFactoryRefRanges
'        End If

'        ' XB 19/09/2014 - BA-1865
'        ''preserve "Enabled" field's value for Lithium
'        ''SGM 14/02/2013 Bug #1134
'        'Dim LiEnabled As Boolean
'        'If myIseTestClient.tparISETests.First.ISETestID = 4 Then
'        '    LiEnabled = myIseTestClient.tparISETests.First.Enabled
'        '    myIseTestFactory.BeginInit()
'        '    Dim myRow As ISETestsDS.tparISETestsRow = CType(myIseTestFactory.tparISETests.Rows(0), ISETestsDS.tparISETestsRow)
'        '    myRow.Enabled = LiEnabled
'        '    myIseTestFactory.EndInit()
'        '    myIseTestFactory.AcceptChanges()

'        'End If
'        ''end  SGM 14/02/2013 Bug #1134

'        'preserve field's value defined by User
'        Dim LiEnabled As Boolean
'        Dim TestName As String
'        myIseTestFactory.BeginInit()
'        Dim myRow As ISETestsDS.tparISETestsRow = CType(myIseTestFactory.tparISETests.Rows(0), ISETestsDS.tparISETestsRow)
'        'preserve "Name" field's value
'        TestName = myIseTestClient.tparISETests.First.Name
'        myRow.Name = TestName
'        If myIseTestClient.tparISETests.First.ISETestID = 4 Then
'            'preserve "Enabled" field's value for Lithium
'            LiEnabled = myIseTestClient.tparISETests.First.Enabled
'            myRow.Enabled = LiEnabled
'        End If
'        myIseTestFactory.EndInit()
'        myIseTestFactory.AcceptChanges()
'        ' XB 19/09/2014 - BA-1865

'        Dim myISETestsDelegate As New ISETestsDelegate()
'        myGlobalDataTO = myISETestsDelegate.SaveISETestNEW(pDBConnection,
'                                                           myIseTestFactory,
'                                                           ISETestsSamplesDS,
'                                                           newRefRangesDS,
'                                                           updatedRefRangesDS,
'                                                           deletedRefRangesDS,
'                                                           New TestSamplesMultirulesDS,
'                                                           New TestControlsDS,
'                                                           Nothing)


'    Catch ex As Exception
'        Dim myLogAcciones As New ApplicationLogManager()
'        myLogAcciones.CreateLogActivity("ISE Tests Update Error.", "ISETestUpdateData.DoUpdateFactoryDefinedElement", EventLogEntryType.Error, False)
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
' ''' <param name="pDataInFactoryDB"></param>
' ''' <param name="pDataInLocalDB"></param>
' ''' <returns></returns>
' ''' <remarks>
' ''' Created by: DL 07/02/2013
' ''' </remarks>
'Protected Overrides Function DoUpdateUserDefinedElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
'    'No action is needed
'    Return New GlobalDataTO
'End Function

'Protected Overrides Function GetAffectedItemsFromFactoryRemoves(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
'    Return New GlobalDataTO
'End Function

' ''' <summary>
' ''' Get Ise Test distinct between Client and Factory
' ''' </summary>
' ''' <param name="pDBConnection"></param>
' ''' <returns></returns>
' ''' <remarks></remarks>
'Protected Overrides Function GetAffectedItemsFromFactoryUpdates(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
'    Dim myLogAcciones As New ApplicationLogManager()
'    Dim myGlobalDataTO As New GlobalDataTO

'    Try

'        myGlobalDataTO = ISETestUpdateDAO.GetISETestDistinctInClient(pDBConnection)

'    Catch ex As Exception
'        myLogAcciones.CreateLogActivity("ISE Tests Update Error.", "ISETestUpdateData.GetAffectedItemsFromFactoryUpdates", EventLogEntryType.Error, False)
'        myGlobalDataTO.HasError = True
'        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'        myGlobalDataTO.ErrorMessage = ex.Message
'    End Try

'    Return myGlobalDataTO
'End Function

' ''' <summary>
' ''' Get data from Factory DB and fill Row 
' ''' </summary>
' ''' <param name="pDBConnection"></param>
' ''' <param name="pItemRow"></param>
' ''' <returns></returns>
' ''' <remarks>
' ''' Created by: DL 07/02/2013
' ''' </remarks>
'Protected Overrides Function GetDataInFactoryDB(pDBConnection As SqlClient.SqlConnection, pItemRow As DataRow) As GlobalDataTO
'    Dim myGlobalDataTO As New GlobalDataTO
'    Try
'        Return GetDataInDB(pDBConnection, pItemRow, True)

'    Catch ex As Exception
'        Dim myLogAcciones As New ApplicationLogManager()
'        myLogAcciones.CreateLogActivity("ISE Tests Update Error.", "ISETestUpdateData.GetDataInFactoryDB", EventLogEntryType.Error, False)
'        myGlobalDataTO.HasError = True
'        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'        myGlobalDataTO.ErrorMessage = ex.Message
'    End Try

'    Return myGlobalDataTO
'End Function

' ''' <summary>
' ''' Get data from LocalDB and fill Row
' ''' </summary>
' ''' <param name="pDBConnection"></param>
' ''' <param name="pItemRow"></param>
' ''' <returns></returns>
' ''' <remarks>
' ''' Created by: DL 07/02/2013
' ''' </remarks>
'Protected Overrides Function GetDataInLocalDB(pDBConnection As SqlClient.SqlConnection, pItemRow As DataRow) As GlobalDataTO
'    Dim myGlobalDataTO As New GlobalDataTO
'    Try
'        Return GetDataInDB(pDBConnection, pItemRow, False)

'    Catch ex As Exception
'        Dim myLogAcciones As New ApplicationLogManager()
'        myLogAcciones.CreateLogActivity("ISE Test Update Error.", "ISETestUpdateData.GetDataInLocalDB", EventLogEntryType.Error, False)
'        myGlobalDataTO.HasError = True
'        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'        myGlobalDataTO.ErrorMessage = ex.Message
'    End Try

'    Return myGlobalDataTO
'End Function

'Protected Overrides Function IsDataEmpty(pDataInDB As Object) As Boolean

'    Return pDataInDB Is Nothing OrElse DirectCast(pDataInDB, DataInDB).IsDataEmpty()
'End Function

'Protected Overrides Function IsFactoryDefinedItem(pDataInDB As Object, pDBConnection As SqlClient.SqlConnection) As Boolean
'    Return True
'End Function
#End Region