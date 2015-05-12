Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.BL.UpdateVersion
    Public Class CalculatedTestUpdateData

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
        Public Function CREATENewCALCTests(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myCalcTestID As Integer = -1
                Dim myNewFormulaDS As New FormulasDS
                Dim myNewCalcTestDS As New CalculatedTestsDS
                Dim myNewRefRanges As New TestRefRangesDS
                Dim myFormulaText As String = String.Empty
                Dim myNewCalcTestsList As New CalculatedTestsDS
                Dim myCalcTestUpdateDAO As New CalculatedTestUpdateDAO
                Dim myCalcTestsDelegate As New CalculatedTestsDelegate
                Dim myUpdateVersionAddedElementsRow As UpdateVersionChangesDS.AddedElementsRow

                '(1) Search in Factory DB all new CALC TESTS
                myGlobalDataTO = myCalcTestUpdateDAO.GetNewFactoryTests(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myNewCalcTestsList = DirectCast(myGlobalDataTO.SetDatos, CalculatedTestsDS)

                    '(2) Process each new CALC Test in Factory DB to add it to Customer DB
                    For Each newTest As CalculatedTestsDS.tparCalculatedTestsRow In myNewCalcTestsList.tparCalculatedTests
                        '(2.1) Get all data in table tparCalculatedTests in Factory DB
                        myGlobalDataTO = myCalcTestUpdateDAO.GetDataInFactoryDB(pDBConnection, newTest.BiosystemsID, True)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myNewCalcTestDS = DirectCast(myGlobalDataTO.SetDatos, CalculatedTestsDS)
                        End If

                        '(2.2) Verify if there is an User CALC Test in Customer DB with the same Name and/or ShortName of the new Factory CALC Test, and in this case, 
                        '      rename the User CALC Test 
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = UpdateRenamedTest(pDBConnection, myNewCalcTestDS.tparCalculatedTests.First.CalcTestLongName, _
                                                               myNewCalcTestDS.tparCalculatedTests.First.CalcTestName, pUpdateVersionChangesList)
                        End If

                        '(2.3) Get the Formula of the NEW CALC Test in FACTORY DB; rebuild field FormulaText using Test Names from CUSTOMER DB
                        If (Not myGlobalDataTO.HasError) Then
                            myNewFormulaDS.Clear()
                            myGlobalDataTO = UpdateCalcTestFormula(pDBConnection, myNewCalcTestDS.tparCalculatedTests.First, myNewFormulaDS)
                        End If

                        '(2.4) Get Reference Ranges defined for the NEW CALC Test in FACTORY DB
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myCalcTestUpdateDAO.GetRefRangesInFactoryDB(pDBConnection, myNewCalcTestDS.tparCalculatedTests.First.CalcTestID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myNewRefRanges = DirectCast(myGlobalDataTO.SetDatos, TestRefRangesDS)
                            End If
                        End If

                        '(2.5) Save the NEW CALC Test in CUSTOMER DB 
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myCalcTestsDelegate.Add(pDBConnection, myNewCalcTestDS, myNewFormulaDS, myNewRefRanges)
                        End If

                        '(2.6) Add a row in the global DS containing all changes in Customer DB due to the Update Version Process (sub-table AddedElements) 
                        '      for the new Factory CALC Test
                        If (Not myGlobalDataTO.HasError) Then
                            myUpdateVersionAddedElementsRow = pUpdateVersionChangesList.AddedElements.NewAddedElementsRow
                            myUpdateVersionAddedElementsRow.ElementType = "CALC"
                            myUpdateVersionAddedElementsRow.ElementName = myNewCalcTestDS.tparCalculatedTests.First.CalcTestLongName & " (" & myNewCalcTestDS.tparCalculatedTests.First.CalcTestName & ")"
                            If (myNewCalcTestDS.tparCalculatedTests.First.UniqueSampleType) Then myUpdateVersionAddedElementsRow.SampleType = myNewCalcTestDS.tparCalculatedTests.First.SampleType
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity("CALC Test Update Error", "CalculatedTestUpdateData.CREATENewCALCTests", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the Formula of a CALC Test in FACTORY DB, and update field FormulaText by using Test Names from CUSTOMER DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pNewCalcTestRow">Row of CalculatedTestsDS containing all data of the Calculated Test in FACTORY DB</param>
        ''' <param name="pNewFormulaDS">FormulasDS to return the Formula of the Calculated Test in FACTORY DB</param>
        ''' <param name="pCustomerCalcTestID">Identifier of the Calculated Test in CUSTOMER DB; optional parameter, informed when this
        '''                                   function is called to update with values in FACTORY DB the Formula of a Calculated Test that
        '''                                   exists in CUSTOMER DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 16/10/2014 - BA-1944 (SubTask BA-2017)
        ''' Modified by: SA 21/11/2014 - BA-2105 ==> Added changes needed to manage OFFS Tests as member of the Formula of Calculated Tests
        ''' </remarks>
        Private Function UpdateCalcTestFormula(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pNewCalcTestRow As CalculatedTestsDS.tparCalculatedTestsRow, _
                                               ByRef pNewFormulaDS As FormulasDS, Optional ByVal pCustomerCalcTestID As Integer = 0) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myTestID As Integer = 0
                Dim myFormulaText As String = String.Empty
                Dim myCalcTestUpdateDAO As New CalculatedTestUpdateDAO

                'Get all members of the Formula of the CALC Test in FACTORY DB
                myGlobalDataTO = myCalcTestUpdateDAO.GetFormulaInFactoryDB(pDBConnection, pNewCalcTestRow.CalcTestID)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    pNewFormulaDS = DirectCast(myGlobalDataTO.SetDatos, FormulasDS)

                    For Each formulaMember As FormulasDS.tparFormulasRow In pNewFormulaDS.tparFormulas
                        If (pCustomerCalcTestID > 0) Then formulaMember.CalcTestID = pCustomerCalcTestID

                        If (formulaMember.ValueType = "TEST") Then
                            'Get the Test Identifier
                            myTestID = Convert.ToInt32(formulaMember.Value)

                            'If the Formula Member is a CALC Test or an OFFS Test, get value of CalcTestID or OffSystemTestID in CUSTOMER DB
                            If (formulaMember.TestType = "CALC" OrElse formulaMember.TestType = "OFFS") Then
                                myGlobalDataTO = GetCustomerTestID(pDBConnection, formulaMember.TestType, myTestID)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myTestID = Convert.ToInt32(myGlobalDataTO.SetDatos)

                                    'Update the Formula by informing the ID of the Calculated or OffSystem Test in CUSTOMER DB
                                    formulaMember.Value = myTestID.ToString
                                End If
                            End If

                            'Get the Test name in CUSTOMER DB and rebuild field FormulaText
                            If (Not myGlobalDataTO.HasError) Then
                                myGlobalDataTO = GetTestNameForFormula(pDBConnection, formulaMember.TestType, myTestID, formulaMember.SampleType)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myFormulaText &= Convert.ToString(myGlobalDataTO.SetDatos)
                                End If
                            End If
                        Else
                            myFormulaText &= formulaMember.Value.Trim
                        End If

                        'If an error has been raised, then the process is stopped...
                        If (myGlobalDataTO.HasError) Then Exit For
                    Next
                    pNewFormulaDS.AcceptChanges()

                    If (Not myGlobalDataTO.HasError) Then
                        'Update the FormulaText field for the Calculated Test (to use the name of Tests in CUSTOMER DB)
                        pNewCalcTestRow.BeginEdit()
                        pNewCalcTestRow.FormulaText = myFormulaText
                        pNewCalcTestRow.AcceptChanges()
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity("CALC Test Update Error", "CalculatedTestUpdateData.UpdateCalcTestFormula", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the name of the informed Test in CUSTOMER DB (according its Test Type), and return the TestName followed by the informed SampleType
        ''' between brackets (used to rebuild the FormulaText field of a Calculated Test in which Formula the informed Test is included)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity("CALC Test Update Error", "CalculatedTestUpdateData.GetTestNameForFormula", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Search the Identifier of a Calculated Test or of an OffSystem Test in CUSTOMER DB, searching it by BiosystemsID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code: CALC or OFFS</param>
        ''' <param name="pFactoryTestID">Identifier of the Calculated Test or the OffSystem Test in FACTORY DB</param>
        ''' <returns>GlobalDataTO containing an integer value with the Identifier of the Calculated Test or OffSystem Test in CUSTOMER DB</returns>
        ''' <remarks>
        ''' Created by:  SA 15/10/2014 - BA-1944 (SubTask BA-2017)
        ''' Modified by: SA 21/11/2014 - BA-2105 ==> Added parameter pTestType to allow use the function also for OFFS Tests
        ''' </remarks>
        Private Function GetCustomerTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, ByVal pFactoryTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim customerTestID As Integer = 0
                Dim factoryBiosystemsID As Integer = 0

                If (pTestType = "CALC") Then
                    Dim myCalcTestUpdateDAO As New CalculatedTestUpdateDAO
                    Dim myCalcTestsDelegate As New CalculatedTestsDelegate

                    'Search in FACTORY DB the BiosystemsID for the informed CalcTestID
                    myGlobalDataTO = myCalcTestUpdateDAO.GetDataInFactoryDB(pDBConnection, pFactoryTestID, False)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        Dim myCalcTestDS As CalculatedTestsDS = DirectCast(myGlobalDataTO.SetDatos, CalculatedTestsDS)

                        If (myCalcTestDS.tparCalculatedTests.Rows.Count > 0) Then
                            factoryBiosystemsID = myCalcTestDS.tparCalculatedTests.First.BiosystemsID

                            'Search in CUSTOMER DB the CalcTestID for the specified BiosystemsID
                            myGlobalDataTO = myCalcTestsDelegate.GetCalcTest(pDBConnection, factoryBiosystemsID, True)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myCalcTestDS = DirectCast(myGlobalDataTO.SetDatos, CalculatedTestsDS)
                                If (myCalcTestDS.tparCalculatedTests.Rows.Count > 0) Then customerTestID = myCalcTestDS.tparCalculatedTests.First.CalcTestID
                            End If
                        End If
                    Else
                        'This case is not possible...
                        myGlobalDataTO.HasError = True
                    End If

                ElseIf (pTestType = "OFFS") Then
                    Dim myOFFSTestUpdateDAO As New OffSystemTestUpdateDAO
                    Dim myOFFSTestsDelegate As New OffSystemTestsDelegate

                    'Search in FACTORY DB the BiosystemsID for the informed OffSystemTestID
                    myGlobalDataTO = myOFFSTestUpdateDAO.GetDataInFactoryDB(pDBConnection, pFactoryTestID, False)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        Dim myOFFSTestDS As OffSystemTestsDS = DirectCast(myGlobalDataTO.SetDatos, OffSystemTestsDS)

                        If (myOFFSTestDS.tparOffSystemTests.Rows.Count > 0) Then
                            factoryBiosystemsID = myOFFSTestDS.tparOffSystemTests.First.BiosystemsID

                            'Search in CUSTOMER DB the OffSystemTestID for the specified BiosystemsID
                            myGlobalDataTO = myOFFSTestsDelegate.Read(pDBConnection, factoryBiosystemsID, True)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myOFFSTestDS = DirectCast(myGlobalDataTO.SetDatos, OffSystemTestsDS)
                                If (myOFFSTestDS.tparOffSystemTests.Rows.Count > 0) Then customerTestID = myOFFSTestDS.tparOffSystemTests.First.OffSystemTestID
                            End If
                        End If
                    End If
                End If

                If (Not myGlobalDataTO.HasError) Then
                    myGlobalDataTO.SetDatos = customerTestID
                    myGlobalDataTO.HasError = False
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity("CALC Test Update Error", "CalculatedTestUpdateData.GetCustomerTestID", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Verify if there is an User CALC Test in Customer DB with the same Name and/or ShortName of a new Factory CALC Test, and in this case, rename
        ''' the User Test by adding as many "R" letters at the beginning of the Name and ShortName as needed until get an unique Name and ShortName
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestName">Name of the new Factory CALC Test to verify</param>
        ''' <param name="pShortName">ShortName of the new Factory CALC Test to verify</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 16/10/2014 - BA-1944 (SubTask BA-2017)
        ''' </remarks>
        Private Function UpdateRenamedTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestName As String, ByVal pShortName As String, _
                                           ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myDuplicatedNameCalcTestID As Integer = 0
                Dim myCalcTestsDS As New CalculatedTestsDS
                Dim myAuxCalcTestsDS As New CalculatedTestsDS
                Dim myCalcTestsDelegate As New CalculatedTestsDelegate
                Dim myHistCalcTestsDAO As New thisCalculatedTestsDAO
                Dim myUpdateVersionRenamedElementsRow As UpdateVersionChangesDS.RenamedElementsRow

                '(1) Search if there is an User Test with the same Name...
                myGlobalDataTO = myCalcTestsDelegate.ExistsCalculatedTest(pDBConnection, pTestName, "FNAME", 0, False)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myCalcTestsDS = DirectCast(myGlobalDataTO.SetDatos, CalculatedTestsDS)

                    If (myCalcTestsDS.tparCalculatedTests.Rows.Count > 0) Then
                        myDuplicatedNameCalcTestID = myCalcTestsDS.tparCalculatedTests.First.CalcTestID
                    End If
                End If

                '(2) Search if there is an User Test with the same ShortName (excluding the Calculated Test with the same Name 
                '    found in the previous step)
                If (Not myGlobalDataTO.HasError) Then
                    myGlobalDataTO = myCalcTestsDelegate.ExistsCalculatedTest(pDBConnection, pShortName, "NAME", myDuplicatedNameCalcTestID, False)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myAuxCalcTestsDS = DirectCast(myGlobalDataTO.SetDatos, CalculatedTestsDS)
                    End If
                End If

                '(3) Merge results of both searches
                If (Not myGlobalDataTO.HasError) Then
                    If (myCalcTestsDS.tparCalculatedTests.Rows.Count > 0 AndAlso myAuxCalcTestsDS.tparCalculatedTests.Rows.Count > 0) Then
                        If (myCalcTestsDS.tparCalculatedTests.First.CalcTestID <> myAuxCalcTestsDS.tparCalculatedTests.First.CalcTestID) Then
                            myCalcTestsDS.tparCalculatedTests.ImportRow(myAuxCalcTestsDS.tparCalculatedTests.First)
                        End If
                    ElseIf (myCalcTestsDS.tparCalculatedTests.Rows.Count = 0 AndAlso myAuxCalcTestsDS.tparCalculatedTests.Rows.Count > 0) Then
                        myCalcTestsDS.tparCalculatedTests.ImportRow(myAuxCalcTestsDS.tparCalculatedTests.First)
                    End If
                End If

                '(4) Process each one of the CALC Tests found
                If (Not myGlobalDataTO.HasError) Then
                    For Each calcTestRow As CalculatedTestsDS.tparCalculatedTestsRow In myCalcTestsDS.tparCalculatedTests
                        'Rename the CALC Test (add as many "R" letters as needed at the beginning of Name and ShortName)
                        myGlobalDataTO = RenameCalcTestName(pDBConnection, calcTestRow)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            If (Convert.ToBoolean(myGlobalDataTO.SetDatos)) Then
                                '(4.1) Update the renamed CALC Test in Customer DB
                                myAuxCalcTestsDS.Clear()
                                myAuxCalcTestsDS.tparCalculatedTests.ImportRow(calcTestRow)
                                myAuxCalcTestsDS.AcceptChanges()

                                myGlobalDataTO = myCalcTestsDelegate.Modify(pDBConnection, myAuxCalcTestsDS, New FormulasDS, New TestRefRangesDS, False, True)

                                '(4.2) Add a row in the global DS containing all changes in Customer DB due to the Update Version Process (sub-table RenamedElements)
                                If (Not myGlobalDataTO.HasError) Then
                                    myUpdateVersionRenamedElementsRow = pUpdateVersionChangesList.RenamedElements.NewRenamedElementsRow
                                    myUpdateVersionRenamedElementsRow.ElementType = "CALC"
                                    myUpdateVersionRenamedElementsRow.PreviousName = pTestName & " (" & pShortName & ")"
                                    myUpdateVersionRenamedElementsRow.UpdatedName = calcTestRow.CalcTestLongName & " (" & calcTestRow.CalcTestName & ")"
                                    pUpdateVersionChangesList.RenamedElements.AddRenamedElementsRow(myUpdateVersionRenamedElementsRow)
                                    pUpdateVersionChangesList.RenamedElements.AcceptChanges()
                                End If
                            Else
                                'If it was not possible to rename the Test (really unlikely case), it is considered an error
                                myGlobalDataTO.HasError = True
                            End If
                        End If

                        'If an error has been raised, then the process is finished
                        If (myGlobalDataTO.HasError) Then Exit For
                    Next
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity("CALC Test Update Error", "CalculatedTestUpdateDate.UpdateRenamedTest", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Add "R" letters at the beginning of a CALC Test Name and ShortName until get an unique Name/ShortName.
        ''' Used during Update Version process when a new Factory CALC Test is added and there is an User Test in 
        ''' Customer DB with the same Test Name and/or ShortName of the one to add
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTestsRow">Row of CalculatedTestsDS containing the basic data of the CALC Test to rename</param>
        ''' <returns>GlobalDataTO containing a Boolean value indicating if the CALC Test has been renamed (when TRUE)</returns>
        ''' <remarks>
        ''' Created by:  SG 15/02/2013
        ''' Modified by: SA 16/10/2014 - BA-1944 (SubTask BA-2017) ==> Return a Boolean value to indicate if the CALC Test has been renamed
        ''' </remarks>
        Private Function RenameCalcTestName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestsRow As CalculatedTestsDS.tparCalculatedTestsRow) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myCalcTestDS As New CalculatedTestsDS
                Dim myCalcTestsDelegate As New CalculatedTestsDelegate

                Dim isValidNewName As Boolean = False
                Dim isValidTestName As Boolean = False
                Dim isValidShortName As Boolean = False

                Dim numOfRs As Integer = 1
                Dim errorFound As Boolean = False
                Dim myValidTestName As String = pCalcTestsRow.CalcTestLongName
                Dim myValidShortName As String = pCalcTestsRow.CalcTestName

                While (Not isValidNewName AndAlso numOfRs < 16 AndAlso Not errorFound)
                    If (Not isValidTestName) Then
                        'Add an "R" at the beginning of the Test Name
                        myValidTestName = "R" & myValidTestName

                        'If the length of the new Test Name is greater than 16, remove the last character
                        If (myValidTestName.Length > 16) Then myValidTestName = myValidTestName.Remove(myValidTestName.Length - 1)

                        'Verify if the new Test Name is unique in Customer DB (there is not another Test with the same Name)
                        myGlobalDataTO = myCalcTestsDelegate.ExistsCalculatedTest(pDBConnection, myValidTestName, "FNAME")
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            isValidTestName = (Not Convert.ToBoolean(myGlobalDataTO.SetDatos))
                        Else
                            errorFound = True
                        End If
                    End If

                    If (Not isValidShortName) Then
                        'Add an "R" at the beginning of the Test Short Name
                        myValidShortName = "R" & myValidShortName

                        'If the length of the new Test Short Name is greater than 8, remove the last character
                        If (myValidShortName.Length > 8) Then myValidShortName = myValidShortName.Remove(myValidShortName.Length - 1)

                        'Verify if the new Test Short Name is unique in Customer DB (there is not another Test with the same Short Name)
                        myGlobalDataTO = myCalcTestsDelegate.ExistsCalculatedTest(pDBConnection, myValidShortName, "NAME")
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            isValidShortName = (Not Convert.ToBoolean(myGlobalDataTO.SetDatos))
                        Else
                            errorFound = True
                        End If
                    End If

                    'The rename is accepted with the rename of both fields Name and ShortName is accepted
                    isValidNewName = (isValidShortName AndAlso isValidTestName)

                    'Just to avoid the very unlikely probability of endless loop 
                    numOfRs += 1
                End While

                'Finally, update the name in the CalculatedTestsDS row received as entry parameter
                If (isValidNewName) Then
                    pCalcTestsRow.BeginEdit()
                    pCalcTestsRow.CalcTestLongName = myValidTestName
                    pCalcTestsRow.CalcTestName = myValidShortName
                    pCalcTestsRow.EndEdit()
                End If

                myGlobalDataTO.SetDatos = isValidNewName
                myGlobalDataTO.HasError = errorFound
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity("CALC Test Update Error", "CalculatedTestUpdateData.RenameTestName", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Execute the process to search all CALC Tests that should be deleted from CUSTOMER DB (those preloaded CALC Tests that exist in CUSTOMER DB but not in FACTORY DB) and 
        ''' remove them from CUSTOMER DB 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 16/10/2014 - BA-1944 (SubTask BA-2017)
        ''' </remarks>
        Public Function DELETERemovedCALCTests(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myDeletedCalcTestDS As New CalculatedTestsDS
                Dim myRemovedCalcTestsDS As New CalculatedTestsDS
                Dim myAffectedCalcTestsDS As New CalculatedTestsDS
                Dim myCalcTestsDelegate As New CalculatedTestsDelegate
                Dim myCalculatedTestUpdateDAO As New CalculatedTestUpdateDAO
                Dim myUpdateVersionDeletedElementsRow As UpdateVersionChangesDS.DeletedElementsRow

                '(1) Search in Customer DB all Preloaded CALC TESTS that do not exist in Factory DB:
                myGlobalDataTO = myCalculatedTestUpdateDAO.GetDeletedPreloadedCALCTests(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myRemovedCalcTestsDS = DirectCast(myGlobalDataTO.SetDatos, CalculatedTestsDS)

                    '(2) Process each returned CALC Test to delete it from CUSTOMER DB
                    For Each removedCalcTest As CalculatedTestsDS.tparCalculatedTestsRow In myRemovedCalcTestsDS.tparCalculatedTests
                        '(2.1) In CUSTOMER DB, search data of the CALC Test to delete 
                        myGlobalDataTO = myCalcTestsDelegate.GetCalcTest(pDBConnection, removedCalcTest.BiosystemsID, True)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myDeletedCalcTestDS = DirectCast(myGlobalDataTO.SetDatos, CalculatedTestsDS)

                            If (myDeletedCalcTestDS.tparCalculatedTests.Rows.Count = 0) Then
                                'This case is not possible due to the query is sorted by BiosystemsID descending and component CALC Tests are 
                                'processed after parent CALC Tests
                                myGlobalDataTO.HasError = True
                            End If
                        End If

                        '(2.2) Delete the CALC Test from CUSTOMER DB
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myCalcTestsDelegate.Delete(pDBConnection, myDeletedCalcTestDS)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myAffectedCalcTestsDS = DirectCast(myGlobalDataTO.SetDatos, CalculatedTestsDS)
                            End If
                        End If

                        '(2.3) Add a row in the global DS containing all changes in Customer DB due to the Update Version Process (sub-table DeletedElements) 
                        '      for the deleted CALC Test
                        If (Not myGlobalDataTO.HasError) Then
                            myUpdateVersionDeletedElementsRow = pUpdateVersionChangesList.DeletedElements.NewDeletedElementsRow
                            myUpdateVersionDeletedElementsRow.ElementType = "CALC"
                            myUpdateVersionDeletedElementsRow.ElementName = myDeletedCalcTestDS.tparCalculatedTests.First.CalcTestLongName & " (" & myDeletedCalcTestDS.tparCalculatedTests.First.CalcTestName & ")"
                            If (myDeletedCalcTestDS.tparCalculatedTests.First.UniqueSampleType) Then myUpdateVersionDeletedElementsRow.SampleType = myDeletedCalcTestDS.tparCalculatedTests.First.SampleType
                            pUpdateVersionChangesList.DeletedElements.AddDeletedElementsRow(myUpdateVersionDeletedElementsRow)
                            pUpdateVersionChangesList.DeletedElements.AcceptChanges()

                            'For each Calculated Test also removed (due to the deleted one was part of its formula), add a row in the global DS containing all changes in 
                            'Customer DB due to the Update Version Process (sub-table DeletedElements) 
                            For Each affectedCalcTest As CalculatedTestsDS.tparCalculatedTestsRow In myAffectedCalcTestsDS.tparCalculatedTests
                                myUpdateVersionDeletedElementsRow = pUpdateVersionChangesList.DeletedElements.NewDeletedElementsRow
                                myUpdateVersionDeletedElementsRow.ElementType = "CALC"
                                myUpdateVersionDeletedElementsRow.ElementName = affectedCalcTest.CalcTestLongName & " (" & affectedCalcTest.CalcTestName & ")"
                                If (affectedCalcTest.UniqueSampleType) Then myUpdateVersionDeletedElementsRow.SampleType = affectedCalcTest.SampleType
                                pUpdateVersionChangesList.DeletedElements.AddDeletedElementsRow(myUpdateVersionDeletedElementsRow)
                                pUpdateVersionChangesList.DeletedElements.AcceptChanges()
                            Next
                        End If

                        'If an error has been raised, then the process is stopped
                        If (myGlobalDataTO.HasError) Then Exit For
                    Next
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity("CALC Test Update Error", "CalculatedTestUpdateData.DELETERemovedCALCTests", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Execute the process to search in FACTORY DB all CALC TESTS that exists in CUSTOMER DB but for which at least one of the relevant Test fields 
        ''' have changed and modify data in CUSTOMER DB (tables tparCalculatedTests, tparFormulas and/or tparTestRefRanges
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 16/10/2014 - BA-1944 (SubTask BA-2017)
        ''' </remarks>
        Public Function UPDATEModifiedCALCTests(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim formulaChanged As Boolean
                Dim refRangesChanged As Boolean

                Dim myCalcTestsDS As New CalculatedTestsDS
                Dim myFormulaDS As New FormulasDS
                Dim myRefRanges As New TestRefRangesDS
                Dim myCustomerCalcTestsDS As New CalculatedTestsDS
                Dim myFactoryCalcTestDS As New CalculatedTestsDS
                Dim myFactoryCalcTestRow As CalculatedTestsDS.tparCalculatedTestsRow = Nothing
                Dim myPreloadedCALCTestList As New List(Of CalculatedTestsDS.tparCalculatedTestsRow)

                Dim myFormulasDelegate As New FormulasDelegate
                Dim myRefRangesDelegate As New TestRefRangesDelegate
                Dim myCalcTestsDelegate As New CalculatedTestsDelegate
                Dim myCalculatedTestUpdateDAO As New CalculatedTestUpdateDAO

                '(1) Get from CUSTOMER DB all preloaded CALC Tests 
                myGlobalDataTO = myCalcTestsDelegate.GetList(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myCustomerCalcTestsDS = DirectCast(myGlobalDataTO.SetDatos, CalculatedTestsDS)

                    'Get only the preloaded ones...
                    myPreloadedCALCTestList = (From a As CalculatedTestsDS.tparCalculatedTestsRow In myCustomerCalcTestsDS.tparCalculatedTests _
                                              Where a.PreloadedCalculatedTest = True _
                                        AndAlso Not a.IsBiosystemsIDNull() _
                                           Order By a.BiosystemsID _
                                             Select a).ToList()
                End If

                '(2) For each preloaded CALC Test in CUSTOMER DB, verify if there are changes in FACTORY DB
                For Each preloadedCustomerCalcTest As CalculatedTestsDS.tparCalculatedTestsRow In myPreloadedCALCTestList
                    '(2.1) Get data of the CALC Test in Factory DB (search by BiosystemsID)
                    myGlobalDataTO = myCalculatedTestUpdateDAO.GetDataInFactoryDB(pDBConnection, preloadedCustomerCalcTest.BiosystemsID, True)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myFactoryCalcTestDS = DirectCast(myGlobalDataTO.SetDatos, CalculatedTestsDS)

                        If (myFactoryCalcTestDS.tparCalculatedTests.Rows.Count > 0) Then
                            myFactoryCalcTestRow = myFactoryCalcTestDS.tparCalculatedTests.First
                        Else
                            'This case is not possible due to CALC Test deleted in FACTORY DB were processed before
                            myGlobalDataTO.HasError = True
                        End If
                    End If

                    '(2.2) Get the Formula of the CALC Test in Factory DB but with following changes: 
                    '      ** In FormulaDS, all IDs of Calculated Tests have been replaced for the ones in Customer DB
                    '      ** Field FormulaText for the Calculated Test has been rebuilt using the name of the Tests in Custormer DB
                    If (Not myGlobalDataTO.HasError) Then
                        myFormulaDS.Clear()
                        myGlobalDataTO = UpdateCalcTestFormula(pDBConnection, myFactoryCalcTestRow, myFormulaDS, preloadedCustomerCalcTest.CalcTestID)
                    End If

                    '(2.3) Verify if the Formula of the CALC Test has been changed 
                    If (Not myGlobalDataTO.HasError) Then
                        myGlobalDataTO = UpdateCustomerTest(myFactoryCalcTestRow, preloadedCustomerCalcTest, pUpdateVersionChangesList)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            formulaChanged = Convert.ToBoolean(myGlobalDataTO.SetDatos)
                        End If
                    End If

                    '(2.4) Verify if the Reference Ranges defined for the CALC Test have been changed
                    If (Not myGlobalDataTO.HasError) Then
                        myRefRanges.Clear()
                        myGlobalDataTO = UpdateCalcTestRefRanges(pDBConnection, myFactoryCalcTestRow, preloadedCustomerCalcTest, myRefRanges, pUpdateVersionChangesList)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            refRangesChanged = Convert.ToBoolean(myGlobalDataTO.SetDatos)
                        End If
                    End If

                    '(2.5) 'Finally, if the Formula and/or the Reference Ranges have been changed, then the CALC Test is updated in CUSTOMER DB
                    If (Not myGlobalDataTO.HasError) Then
                        If (formulaChanged OrElse refRangesChanged) Then
                            myCalcTestsDS.Clear()
                            myCalcTestsDS.tparCalculatedTests.ImportRow(preloadedCustomerCalcTest)
                            myCalcTestsDS.AcceptChanges()

                            myGlobalDataTO = myCalcTestsDelegate.Modify(pDBConnection, myCalcTestsDS, myFormulaDS, myRefRanges, formulaChanged, formulaChanged)
                        End If
                    End If

                    'If an error has been raised, then the process is stopped
                    If (myGlobalDataTO.HasError) Then Exit For
                Next
                myPreloadedCALCTestList = Nothing

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity("CALC Test Update Error", "CalculatedTestUpdateData.UPDATEModifiedCALCTests", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For a CALC Test, compare value of relevant fields in table tparCalculatedTests in CUSTOMER DB with value of the same fields in FACTORY DB, 
        ''' and update in CUSTOMER DB all modified fields.
        ''' </summary>
        ''' <param name="pFactoryCalcTestRow">Row of CalculatedTestsDS containing data of the CALC Test in FACTORY DB (although with field FormulaText
        '''                                   updated to use the name of the component Tests from CUSTOMER DB)</param>
        ''' <param name="pCustomerCalcTestRow">Row of CalculatedTestsDS containing data of the CALC Test in CUSTOMER DB</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing a Boolean value: when TRUE, it means the Formula of the CALC Test has changed</returns>
        ''' <remarks>
        ''' Created by:  SA 16/10/2014 - BA-1944 (SubTask BA-2017)
        ''' </remarks>
        Private Function UpdateCustomerTest(ByVal pFactoryCalcTestRow As CalculatedTestsDS.tparCalculatedTestsRow, _
                                            ByVal pCustomerCalcTestRow As CalculatedTestsDS.tparCalculatedTestsRow, _
                                            ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                'Build the TestName to report changes...
                Dim myCalcTestName As String = pCustomerCalcTestRow.CalcTestLongName & " (" & pCustomerCalcTestRow.CalcTestName & ") "

                'Verify if field SampleType has changed
                Dim mySampleType As String = "--"
                Dim formulaUpdated As Boolean = False

                If (Not pFactoryCalcTestRow.IsSampleTypeNull) Then
                    If (pCustomerCalcTestRow.IsSampleTypeNull OrElse pCustomerCalcTestRow.SampleType <> pFactoryCalcTestRow.SampleType) Then
                        If (Not pCustomerCalcTestRow.IsSampleTypeNull AndAlso pCustomerCalcTestRow.SampleType <> String.Empty) Then mySampleType = pCustomerCalcTestRow.SampleType

                        'Add a row for field SampleType in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "CALC", myCalcTestName, pFactoryCalcTestRow.SampleType, "SampleType", mySampleType, _
                                                            pFactoryCalcTestRow.SampleType)

                        'Update field in the Customer DataSet (update also field UniqueSampleType)
                        pCustomerCalcTestRow.SampleType = pFactoryCalcTestRow.SampleType
                        pCustomerCalcTestRow.UniqueSampleType = pFactoryCalcTestRow.UniqueSampleType
                    End If
                Else
                    If (Not pCustomerCalcTestRow.IsSampleTypeNull) Then
                        'Add a row for field SampleType in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "CALC", myCalcTestName, String.Empty, "SampleType", pCustomerCalcTestRow.SampleType, _
                                                            mySampleType)

                        'Update field in the Customer DataSet (update also field UniqueSampleType)
                        pCustomerCalcTestRow.SetSampleTypeNull()
                        pCustomerCalcTestRow.UniqueSampleType = pFactoryCalcTestRow.UniqueSampleType
                    End If
                End If

                'If the SampleType is the same, verify if the Formula has changed
                If (pCustomerCalcTestRow.FormulaText <> pFactoryCalcTestRow.FormulaText) Then
                    formulaUpdated = True

                    'Add a row for field FormulaText in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                    AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "CALC", myCalcTestName, pFactoryCalcTestRow.SampleType, "FormulaText", pCustomerCalcTestRow.FormulaText, _
                                                        pFactoryCalcTestRow.FormulaText)

                    'Update field in the Customer DataSet
                    pCustomerCalcTestRow.FormulaText = pFactoryCalcTestRow.FormulaText

                    'If the Formula has changed, verify also if field MeasureUnit has changed
                    If (pCustomerCalcTestRow.MeasureUnit <> pFactoryCalcTestRow.MeasureUnit) Then
                        'Add a row for field MeasureUnit in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "CALC", myCalcTestName, pFactoryCalcTestRow.SampleType, "MeasureUnit", pCustomerCalcTestRow.MeasureUnit, _
                                                            pFactoryCalcTestRow.MeasureUnit)

                        'Update field in the Customer DataSet
                        pCustomerCalcTestRow.MeasureUnit = pFactoryCalcTestRow.MeasureUnit
                    End If

                    If (pCustomerCalcTestRow.Decimals <> pFactoryCalcTestRow.Decimals) Then
                        'Add a row for field Decimals in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "CALC", myCalcTestName, pFactoryCalcTestRow.SampleType, "MeasureUnit", pCustomerCalcTestRow.Decimals.ToString, _
                                                            pFactoryCalcTestRow.Decimals.ToString)

                        'Update field in the Customer DataSet
                        pCustomerCalcTestRow.Decimals = pFactoryCalcTestRow.Decimals
                    End If
                End If

                myGlobalDataTO.SetDatos = formulaUpdated
                myGlobalDataTO.HasError = False
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity("CALC Test Update Error", "CalculatedTestUpdateData.UpdateCustomerTest", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For a CALC Test, verify if there are changes in the Reference Ranges in FACTORY DB and if these changes have to be applied in 
        ''' CUSTOMER DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pFactoryCalcTestRow">Row of CalculatedTestsDS containing data of the CALC Test in FACTORY DB</param>
        ''' <param name="pCustomerCalcTestRow">Row of CalculatedTestsDS containing data of the CALC Test in CUSTOMER DB</param>
        ''' <param name="pNewRefRangesDS">TestRefRangesDS to return the Reference Ranges defined for the Calculated Test in FACTORY DB</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing a Boolean value: when TRUE, it means the Reference Ranges of the CALC Test have changed</returns>
        '''  <remarks>
        ''' Created by:  SA 16/10/2014 - BA-1944 (SubTask BA-2017)
        ''' Modified by: SA 25/11/2014 - BA-2105 ==> Added some changes in the process of update Reference Ranges for a preloaded Calculated Test due 
        '''                                          to field ActiveRangeType can be NULL while there are GENERIC and/or DETAILED Ranges defined for the
        '''                                          Calculated Test
        ''' </remarks>
        Private Function UpdateCalcTestRefRanges(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pFactoryCalcTestRow As CalculatedTestsDS.tparCalculatedTestsRow, _
                                                 ByVal pCustomerCalcTestRow As CalculatedTestsDS.tparCalculatedTestsRow, ByRef pNewRefRangesDS As TestRefRangesDS, _
                                                 ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim updateRefRanges As Boolean = False
                Dim customerRefRangesDS As New TestRefRangesDS
                Dim myCalcTestUpdateDAO As New CalculatedTestUpdateDAO
                Dim myTestRefRangesDelegate As New TestRefRangesDelegate
                Dim lstCustomerRangesByType As New List(Of TestRefRangesDS.tparTestRefRangesRow)

                'Build the TestName to report changes...
                Dim myCalcTestName As String = pCustomerCalcTestRow.CalcTestLongName & " (" & pCustomerCalcTestRow.CalcTestName & ") "

                'Get the Reference Ranges defined for the CALC Test in FACTORY DB - CURRENTLY, BIOSYSTEMS DISTRIBUTES ONLY GENERIC REFERENCE RANGES FOR CALCULATED TESTS
                If (Not pFactoryCalcTestRow.IsActiveRangeTypeNull) Then
                    myGlobalDataTO = myCalcTestUpdateDAO.GetRefRangesInFactoryDB(pDBConnection, pFactoryCalcTestRow.CalcTestID)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        pNewRefRangesDS = DirectCast(myGlobalDataTO.SetDatos, TestRefRangesDS)
                    End If
                End If

                'There are Reference Ranges for the Calculated Test in FACTORY DB
                If (pNewRefRangesDS.tparTestRefRanges.Rows.Count > 0) Then
                    'In the TestRefRangesDS from FACTORY DB, update field TestID with value of CalcTestID from CUSTOMER DB
                    pNewRefRangesDS.BeginInit()
                    For Each refRangeRow As TestRefRangesDS.tparTestRefRangesRow In pNewRefRangesDS.tparTestRefRanges
                        refRangeRow.TestID = pCustomerCalcTestRow.CalcTestID
                    Next
                    pNewRefRangesDS.AcceptChanges()

                    'Get the Reference Ranges defined for the Calculated Test in CUSTOMER DB
                    myGlobalDataTO = myTestRefRangesDelegate.ReadByTestID(pDBConnection, pCustomerCalcTestRow.CalcTestID, String.Empty, String.Empty, "CALC")
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        customerRefRangesDS = DirectCast(myGlobalDataTO.SetDatos, TestRefRangesDS)

                        Dim myActiveRangeType As String = "--"
                        If (customerRefRangesDS.tparTestRefRanges.Rows.Count = 0) Then
                            '(A) There are not Reference Ranges defined for the Calculated Test in CUSTOMER DB; the Generic Range is added and ActiveRangeType is also updated
                            updateRefRanges = True

                            'Add a row for field ActiveRangeType in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                            AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "CALC", myCalcTestName, pCustomerCalcTestRow.SampleType, "ActiveRangeType", myActiveRangeType, _
                                                                pFactoryCalcTestRow.ActiveRangeType)

                            'Update field ActiveRangeType in Customer DB
                            pCustomerCalcTestRow.ActiveRangeType = pFactoryCalcTestRow.ActiveRangeType
                            pCustomerCalcTestRow.AcceptChanges()
                        Else
                            '(B) There are Reference Ranges defined for the Calculated Test in CUSTOMER DB

                            'Verify if there is a GENERIC Reference Range for the CALC TEST in CUSTOMER DB 
                            lstCustomerRangesByType = (From a As TestRefRangesDS.tparTestRefRangesRow In customerRefRangesDS.tparTestRefRanges _
                                                      Where a.RangeType = "GENERIC" _
                                                     Select a).ToList()

                            If (lstCustomerRangesByType.Count = 0) Then
                                '(C) There is not a GENERIC Range in CUSTOMER DB but there are DETAILED ones 
                                updateRefRanges = True

                                'The GENERIC Range is added but the ActiveRangeType remains without changes 
                                'Nothing to do, the Range to add is already prepared in pNewRefRangesDS
                            Else
                                '(D) There is a GENERIC Range in CUSTOMER DB; verify if the limits have been changed; the ActiveRangeType remains without changes
                                updateRefRanges = (lstCustomerRangesByType.First.NormalLowerLimit.ToString <> pNewRefRangesDS.tparTestRefRanges.First.NormalLowerLimit.ToString) OrElse _
                                                  (lstCustomerRangesByType.First.NormalUpperLimit.ToString <> pNewRefRangesDS.tparTestRefRanges.First.NormalUpperLimit.ToString)

                                If (updateRefRanges) Then
                                    'Values of the GENERIC RANGE defined for the CALC Test in CUSTOMER DB are replaced by values in FACTORY DB
                                    'Prepare the TestRefRangesDS with values needed for the update
                                    pNewRefRangesDS.tparTestRefRanges.BeginInit()
                                    pNewRefRangesDS.tparTestRefRanges.First.RangeID = lstCustomerRangesByType.First.RangeID
                                    pNewRefRangesDS.tparTestRefRanges.First.TestID = lstCustomerRangesByType.First.TestID
                                    pNewRefRangesDS.tparTestRefRanges.First.IsNew = False
                                    pNewRefRangesDS.tparTestRefRanges.First.IsDeleted = False
                                    pNewRefRangesDS.tparTestRefRanges.AcceptChanges()

                                    'If LowerLimit has been changed, add a row for field NormalLowerLimit in the DS containing all changes in Customer DB due to 
                                    'the Update Version Process (sub-table UpdatedElements) 
                                    If (lstCustomerRangesByType.First.NormalLowerLimit.ToString <> pNewRefRangesDS.tparTestRefRanges.First.NormalLowerLimit.ToString) Then
                                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "CALC", myCalcTestName, pCustomerCalcTestRow.SampleType, "NormalLowerLimit", _
                                                                            lstCustomerRangesByType.First.NormalLowerLimit.ToString, _
                                                                            pNewRefRangesDS.tparTestRefRanges.First.NormalLowerLimit.ToString)
                                    End If

                                    'If the UpperLimit has been changed, add a row for field NormalUpperLimit in the DS containing all changes in Customer DB due to 
                                    'the Update Version Process (sub-table UpdatedElements) 
                                    If (lstCustomerRangesByType.First.NormalUpperLimit.ToString <> pNewRefRangesDS.tparTestRefRanges.First.NormalUpperLimit.ToString) Then
                                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "CALC", myCalcTestName, pCustomerCalcTestRow.SampleType, "NormalUpperLimit", _
                                                                            lstCustomerRangesByType.First.NormalUpperLimit.ToString, _
                                                                            pNewRefRangesDS.tparTestRefRanges.First.NormalUpperLimit.ToString)
                                    End If
                                End If
                            End If
                        End If
                    End If
                Else
                    'No changes in the Reference Ranges defined for the Calculated Test in CUSTOMER DB
                End If
                lstCustomerRangesByType = Nothing

                ''There are Reference Ranges for the Calculated Test in FACTORY DB
                'If (pNewRefRangesDS.tparTestRefRanges.Rows.Count > 0) Then
                '    Dim myActiveRangeType As String = "--"

                '    If (pCustomerCalcTestRow.IsActiveRangeTypeNull) Then
                '        updateRefRanges = True

                '        'In the TestRefRangesDS from FACTORY DB, update field TestID with value of CalcTestID from CUSTOMER DB
                '        pNewRefRangesDS.BeginInit()
                '        For Each refRangeRow As TestRefRangesDS.tparTestRefRangesRow In pNewRefRangesDS.tparTestRefRanges
                '            refRangeRow.TestID = pCustomerCalcTestRow.CalcTestID
                '        Next
                '        pNewRefRangesDS.AcceptChanges()

                '        'Add a row for field ActiveRangeType in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                '        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "CALC", myCalcTestName, pCustomerCalcTestRow.SampleType, "ActiveRangeType", myActiveRangeType, _
                '                                            pFactoryCalcTestRow.ActiveRangeType)

                '        'Update field ActiveRangeType in Customer DB
                '        pCustomerCalcTestRow.ActiveRangeType = pFactoryCalcTestRow.ActiveRangeType
                '        pCustomerCalcTestRow.AcceptChanges()
                '    Else
                '        'Get the Reference Ranges defined for the Calculated Test in CUSTOMER DB
                '        myGlobalDataTO = myTestRefRangesDelegate.ReadByTestID(pDBConnection, pCustomerCalcTestRow.CalcTestID, String.Empty, String.Empty, "CALC")
                '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                '            customerRefRangesDS = DirectCast(myGlobalDataTO.SetDatos, TestRefRangesDS)
                '        End If

                '        If (pFactoryCalcTestRow.ActiveRangeType = "GENERIC") Then
                '            'Verify if there is a GENERIC Reference Range for the CALC TEST in CUSTOMER DB 
                '            lstCustomerRangesByType = (From a As TestRefRangesDS.tparTestRefRangesRow In customerRefRangesDS.tparTestRefRanges _
                '                                      Where a.RangeType = "GENERIC" _
                '                                     Select a).ToList()

                '            If (lstCustomerRangesByType.Count > 0) Then
                '                'There is a GENERIC Range in CUSTOMER DB; verify if the limits have been changed
                '                updateRefRanges = (lstCustomerRangesByType.First.NormalLowerLimit.ToString <> pNewRefRangesDS.tparTestRefRanges.First.NormalLowerLimit.ToString) OrElse _
                '                                  (lstCustomerRangesByType.First.NormalUpperLimit.ToString <> pNewRefRangesDS.tparTestRefRanges.First.NormalUpperLimit.ToString)

                '                If (updateRefRanges) Then
                '                    'Values of the GENERIC RANGE defined for the CALC Test in CUSTOMER DB are replaced by values in FACTORY DB
                '                    'Prepare the TestRefRangesDS with values needed for the update
                '                    pNewRefRangesDS.tparTestRefRanges.BeginInit()
                '                    pNewRefRangesDS.tparTestRefRanges.First.RangeID = lstCustomerRangesByType.First.RangeID
                '                    pNewRefRangesDS.tparTestRefRanges.First.TestID = lstCustomerRangesByType.First.TestID
                '                    pNewRefRangesDS.tparTestRefRanges.First.IsNew = False
                '                    pNewRefRangesDS.tparTestRefRanges.First.IsDeleted = False
                '                    pNewRefRangesDS.tparTestRefRanges.AcceptChanges()

                '                    'If LowerLimit has been changed, add a row for field NormalLowerLimit in the DS containing all changes in Customer DB due to 
                '                    'the Update Version Process (sub-table UpdatedElements) 
                '                    If (lstCustomerRangesByType.First.NormalLowerLimit.ToString <> pNewRefRangesDS.tparTestRefRanges.First.NormalLowerLimit.ToString) Then
                '                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "CALC", myCalcTestName, pCustomerCalcTestRow.SampleType, "NormalLowerLimit", _
                '                                                            lstCustomerRangesByType.First.NormalLowerLimit.ToString, _
                '                                                            pNewRefRangesDS.tparTestRefRanges.First.NormalLowerLimit.ToString)
                '                    End If

                '                    'If the UpperLimit has been changed, add a row for field NormalUpperLimit in the DS containing all changes in Customer DB due to 
                '                    'the Update Version Process (sub-table UpdatedElements) 
                '                    If (lstCustomerRangesByType.First.NormalUpperLimit.ToString <> pNewRefRangesDS.tparTestRefRanges.First.NormalUpperLimit.ToString) Then
                '                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "CALC", myCalcTestName, pCustomerCalcTestRow.SampleType, "NormalUpperLimit", _
                '                                                            lstCustomerRangesByType.First.NormalUpperLimit.ToString, _
                '                                                            pNewRefRangesDS.tparTestRefRanges.First.NormalUpperLimit.ToString)
                '                    End If
                '                End If
                '            Else
                '                If (pCustomerCalcTestRow.ActiveRangeType = "GENERIC") Then
                '                    'This case is not possible...
                '                    myGlobalDataTO.HasError = True
                '                ElseIf (pCustomerCalcTestRow.ActiveRangeType = "DETAILED") Then
                '                    'New Reference Range to add: in the TestRefRangesDS from FACTORY DB, update field TestID with value of CalcTestID from CUSTOMER DB
                '                    pNewRefRangesDS.tparTestRefRanges.BeginInit()
                '                    pNewRefRangesDS.tparTestRefRanges.First.TestID = customerRefRangesDS.tparTestRefRanges.First.TestID
                '                    pNewRefRangesDS.tparTestRefRanges.AcceptChanges()

                '                    updateRefRanges = True
                '                End If
                '            End If

                '        ElseIf (pFactoryCalcTestRow.ActiveRangeType = "DETAILED") Then
                '            'NOT IMPLEMENTED; CURRENTLY, BIOSYSTEMS DISTRIBUTES ONLY GENERIC REFERENCE RANGES FOR CALCULATED TESTS
                '        End If
                '    End If
                'Else
                '    'No changes in the Reference Ranges defined for the Calculated Test in CUSTOMER DB
                'End If
                'lstCustomerRangesByType = Nothing

                myGlobalDataTO.SetDatos = updateRefRanges
                myGlobalDataTO.HasError = False
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity("CALC Test Update Error", "CalculatedTestUpdateData.UpdateCalcTestRefRanges", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Add a new row with data of an updated element in the DS containing all changes in CUSTOMER DB due to the Update Version Process.
        ''' Data is added in sub-table UpdatedElements
        ''' </summary>
        ''' <param name="pUpdateVersionChangesList">DataSet containing all changes made in CUSTOMER DB for the Update Version Process</param>
        ''' <param name="pElementType">Type of affected Element: CALC</param>
        ''' <param name="pElementName">Name of the affected Element</param>
        ''' <param name="pSampleType">Sample Type of the affected Element</param>
        ''' <param name="pUpdatedField">Name of the affected field</param>
        ''' <param name="pPreviousValue">Previous value of the field in CUSTOMER DB</param>
        ''' <param name="pNewValue">New value of the field in CUSTOMER DB (from FACTORY DB)</param>
        ''' <remarks>
        ''' Created by:  SA 16/10/2014 - BA-1944 (SubTask BA-2017)
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
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity("CALC Test Update Error", "CalculatedTestUpdateData.AddUpdatedElementToChangesStructure", EventLogEntryType.Error, False)
                Throw
            End Try
        End Sub
#End Region
    End Class
End Namespace

#Region "FUNCTIONS FOR OLD UPDATE VERSION PROCESS (NOT USED)"
'Inherits UpdateElementParent

'#Region " Internal Data "
'        ''' <summary>
'        ''' Gets the DataTable inside the DataSet: the result of comparation of DataBases
'        ''' </summary>
'        ''' <param name="pMyDataSet"></param>
'        ''' <returns></returns>
'        ''' <remarks></remarks>
'        Protected Overrides Function GetDataTable(pMyDataSet As DataSet) As DataTable
'            Return pMyDataSet.Tables(0)
'        End Function

'        ''' <summary>
'        ''' Contains all related Data in a Calculated Test
'        ''' </summary>
'        ''' <remarks>
'        ''' Created By: JB - 30/01/2013
'        ''' </remarks>
'        Protected Structure DataInDB
'            Public calcTestDS As CalculatedTestsDS
'            Public formulaDS As FormulasDS
'            Public refRangesDS As TestRefRangesDS

'            ''' <summary>
'            ''' Fill CalcTestDS in structure, searching by CalcTestName
'            ''' </summary>
'            ''' <param name="pDBConnection"></param>
'            ''' <param name="pCalcTestName"></param>
'            ''' <returns></returns>
'            ''' <remarks>
'            ''' Created By: DL - 30/01/2013
'            ''' </remarks>
'            Public Function SearchCalcTestByName(pDBConnection As SqlClient.SqlConnection, pCalcTestName As String, Optional factoryDB As Boolean = False) As Boolean
'                Dim dbConnection As SqlClient.SqlConnection = Nothing

'                calcTestDS = New CalculatedTestsDS()
'                Try
'                    Dim resultData As GlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
'                    If resultData.HasError Then Return False
'                    If resultData.SetDatos Is Nothing Then Return False

'                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
'                    If dbConnection Is Nothing Then Return False

'                    Dim dbName As String = ""
'                    If factoryDB Then dbName = GlobalBase.TemporalDBName

'                    Dim myDAO As New tparCalculatedTestsDAO
'                    resultData = myDAO.ReadByCalcTestName(dbConnection, pCalcTestName, pDataBaseName:=dbName)
'                    If resultData.HasError Then Return False
'                    If resultData.SetDatos Is Nothing Then Return False

'                    calcTestDS = DirectCast(resultData.SetDatos, CalculatedTestsDS)

'                    Return True

'                Catch ex As Exception
'                    'Dim myLogAcciones As New ApplicationLogManager()
'                    GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestUpdateData.DataInDB.SearchCalcTestByName", EventLogEntryType.Error, False)
'                Finally
'                    If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
'                End Try
'                Return False
'            End Function

'            ''' <summary>
'            ''' Fill CalcTestDS in structure, searching by CalcTestLongName
'            ''' </summary>
'            ''' <param name="pDBConnection"></param>
'            ''' <param name="pCalcTestLongName"></param>
'            ''' <param name="factoryDB"></param>
'            ''' <returns></returns>
'            ''' <remarks>Created by: SG 15/02/2013</remarks>
'            Public Function SearchCalcTestByLongName(pDBConnection As SqlClient.SqlConnection, pCalcTestLongName As String, Optional factoryDB As Boolean = False) As Boolean
'                Dim dbConnection As SqlClient.SqlConnection = Nothing

'                calcTestDS = New CalculatedTestsDS()
'                Try
'                    Dim resultData As GlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
'                    If resultData.HasError Then Return False
'                    If resultData.SetDatos Is Nothing Then Return False

'                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
'                    If dbConnection Is Nothing Then Return False

'                    Dim dbName As String = ""
'                    If factoryDB Then dbName = GlobalBase.TemporalDBName

'                    Dim myDAO As New tparCalculatedTestsDAO
'                    resultData = myDAO.ReadByCalcTestLongName(dbConnection, pCalcTestLongName, pDataBaseName:=dbName)
'                    If resultData.HasError Then Return False
'                    If resultData.SetDatos Is Nothing Then Return False

'                    calcTestDS = DirectCast(resultData.SetDatos, CalculatedTestsDS)

'                    Return True

'                Catch ex As Exception
'                    'Dim myLogAcciones As New ApplicationLogManager()
'                    GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestUpdateData.DataInDB.SearchCalcTestByLongName", EventLogEntryType.Error, False)
'                Finally
'                    If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
'                End Try
'                Return False
'            End Function

'            ''' <summary>
'            ''' Fill FormulasDS in structure, searching by CalcTestID
'            ''' </summary>
'            ''' <param name="pDBConnection"></param>
'            ''' <param name="pCalcTestId"></param>
'            ''' <returns></returns>
'            ''' <remarks>
'            ''' Created By: DL - 30/01/2013
'            ''' </remarks>
'            Public Function SearchFormulaByCalcTestId(pDBConnection As SqlClient.SqlConnection, pCalcTestId As Integer, Optional factoryDB As Boolean = False) As Boolean
'                Dim dbConnection As SqlClient.SqlConnection = Nothing

'                formulaDS = New FormulasDS()
'                Try
'                    Dim resultData As GlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
'                    If resultData.HasError Then Return False
'                    If resultData.SetDatos Is Nothing Then Return False

'                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
'                    If dbConnection Is Nothing Then Return False

'                    Dim dbName As String = ""
'                    If factoryDB Then dbName = GlobalBase.TemporalDBName

'                    Dim myDAO As New tparFormulasDAO
'                    resultData = myDAO.GetFormulaValues(dbConnection, pCalcTestId, pDataBaseName:=dbName)
'                    If resultData.HasError Then Return False
'                    If resultData.SetDatos Is Nothing Then Return False

'                    formulaDS = DirectCast(resultData.SetDatos, FormulasDS)
'                    Return True
'                Catch ex As Exception
'                    'Dim myLogAcciones As New ApplicationLogManager()
'                    GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestUpdateData.DataInDB.SearchFormulaByCalcTestId", EventLogEntryType.Error, False)
'                Finally
'                    If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
'                End Try
'                Return False
'            End Function

'            ''' <summary>
'            ''' Fill TestRefRangesDS in structure, searching by CalcTestID
'            ''' </summary>
'            ''' <param name="pDBConnection"></param>
'            ''' <param name="pCalcTestId"></param>
'            ''' <returns></returns>
'            ''' <remarks>
'            ''' Created By: JB - 30/01/2013
'            ''' </remarks>
'            Public Function SearchRefRangesByCalcTestId(pDBConnection As SqlClient.SqlConnection, pCalcTestId As Integer, Optional factoryDB As Boolean = False) As Boolean
'                Dim dbConnection As SqlClient.SqlConnection = Nothing

'                refRangesDS = New TestRefRangesDS()
'                Try
'                    Dim resultData As GlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
'                    If resultData.HasError Then Return False
'                    If resultData.SetDatos Is Nothing Then Return False

'                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
'                    If dbConnection Is Nothing Then Return False

'                    Dim dbName As String = ""
'                    If factoryDB Then dbName = GlobalBase.TemporalDBName

'                    Dim myDAO As New tparTestRefRangesDAO
'                    resultData = myDAO.ReadByTestID(dbConnection, pCalcTestId, , , "CALC", pDataBaseName:=dbName)
'                    If resultData.HasError Then Return False
'                    If resultData.SetDatos Is Nothing Then Return False

'                    refRangesDS = DirectCast(resultData.SetDatos, TestRefRangesDS)
'                    Return True

'                Catch ex As Exception
'                    'Dim myLogAcciones As New ApplicationLogManager()
'                    GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestUpdateData.DataInDB.SearchRefRangesByCalcTestId", EventLogEntryType.Error, False)
'                Finally
'                    If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
'                End Try
'                Return False
'            End Function

'            ''' <summary>
'            ''' Fill CalcTestDS in structure, searching by BiosystemsID
'            ''' </summary>
'            ''' <param name="pDBConnection"></param>
'            ''' <param name="pBiosystemsID"></param>
'            ''' <returns></returns>
'            ''' <remarks>
'            ''' Created By: SG 19/02/2013
'            ''' </remarks>
'            Public Function SearchCalcTestByBiosystemsID(pDBConnection As SqlClient.SqlConnection, pBiosystemsID As Integer, Optional factoryDB As Boolean = False) As Boolean
'                Dim dbConnection As SqlClient.SqlConnection = Nothing

'                calcTestDS = New CalculatedTestsDS()
'                Try
'                    Dim resultData As GlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
'                    If resultData.HasError Then Return False
'                    If resultData.SetDatos Is Nothing Then Return False

'                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
'                    If dbConnection Is Nothing Then Return False

'                    Dim dbName As String = ""
'                    If factoryDB Then dbName = GlobalBase.TemporalDBName

'                    Dim myDAO As New tparCalculatedTestsDAO
'                    resultData = myDAO.ReadByBiosystemsID(dbConnection, pBiosystemsID, pDataBaseName:=dbName)
'                    If resultData.HasError Then Return False
'                    If resultData.SetDatos Is Nothing Then Return False

'                    calcTestDS = DirectCast(resultData.SetDatos, CalculatedTestsDS)

'                    Return True

'                Catch ex As Exception
'                    'Dim myLogAcciones As New ApplicationLogManager()
'                    GlobalBase.CreateLogActivity(ex.Message, "CalculatedTestUpdateData.DataInDB.SearchCalcTestByBiosystemsID", EventLogEntryType.Error, False)
'                Finally
'                    If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
'                End Try
'                Return False
'            End Function

'            ''' <summary>
'            ''' Returns if represented Data is Empty
'            ''' </summary>
'            ''' <returns></returns>
'            ''' <remarks>
'            ''' Created by: DL - 07/02/2013
'            ''' </remarks>
'            Public Function IsDataEmpty() As Boolean
'                Return calcTestDS Is Nothing OrElse calcTestDS.Tables.Count = 0 OrElse calcTestDS.tparCalculatedTests.Rows.Count = 0
'            End Function
'        End Structure

'        ''' <summary>
'        ''' Gets the structured data in a dataBase, from the result of comparation of DataBases
'        ''' </summary>
'        ''' <param name="pDBConnection"></param>
'        ''' <param name="pItemRow">Row of a result of comparation of Client and Factory DataBases</param>
'        ''' <param name="pfactoryDB">Determine which databases use to get data: Factory DB or Client DB</param>
'        ''' <returns>
'        ''' A GlobalDataTO with a DataInDB structure inside
'        ''' </returns>
'        ''' <remarks>
'        ''' Created by: DL 31/01/2013
'        ''' Modified by: SG 15/02/2013 - find by Name or Long Name
'        ''' Modified by: SG 19/02/2013 - find by new field BiosystemsID
'        ''' </remarks>
'        Private Function GetDataInDB(pDBConnection As SqlClient.SqlConnection,
'                                             pItemRow As DataRow,
'                                             pfactoryDB As Boolean) As GlobalDataTO

'            Dim myGlobalDataTO As New GlobalDataTO

'            Try
'                Dim dataDB As New DataInDB()
'                Dim bRet As Boolean

'                'SGM 19/02/2013
'                Dim biosystemsID As Integer
'                If Not TypeOf pItemRow.Item("BiosystemsID") Is System.DBNull Then
'                    biosystemsID = CInt(pItemRow.Item("BiosystemsID"))
'                    'find by BiosystemsID
'                    bRet = dataDB.SearchCalcTestByBiosystemsID(pDBConnection, biosystemsID, pfactoryDB) 'SGM 19/02/2013
'                    If Not bRet OrElse dataDB.calcTestDS.tparCalculatedTests.Rows.Count = 0 Then Return myGlobalDataTO

'                Else

'                    Dim calcTestName As String = pItemRow.Item("CalcTestName").ToString()
'                    If String.IsNullOrEmpty(calcTestName) Then Return myGlobalDataTO

'                    Dim calcTestLongName As String = pItemRow.Item("CalcTestLongName").ToString() 'SGM 15/02/2012
'                    If String.IsNullOrEmpty(calcTestLongName) Then Return myGlobalDataTO

'                    'SGM 15/02/2013 - find by Name or Long Name
'                    bRet = dataDB.SearchCalcTestByName(pDBConnection, calcTestName, pfactoryDB)
'                    If bRet AndAlso dataDB.calcTestDS.tparCalculatedTests.Rows.Count = 0 Then
'                        bRet = dataDB.SearchCalcTestByLongName(pDBConnection, calcTestLongName, pfactoryDB)
'                        If Not bRet OrElse dataDB.calcTestDS.tparCalculatedTests.Rows.Count = 0 Then Return myGlobalDataTO
'                    ElseIf Not bRet Then
'                        Return myGlobalDataTO
'                    End If
'                    'end SGM 15/02/2013

'                End If
'                'end SGM 19/02/2013



'                Dim calcTestId As Integer = DirectCast(dataDB.calcTestDS.tparCalculatedTests.Rows(0), CalculatedTestsDS.tparCalculatedTestsRow).CalcTestID

'                bRet = dataDB.SearchFormulaByCalcTestId(pDBConnection, calcTestId, pfactoryDB)
'                If Not bRet Then Return myGlobalDataTO

'                bRet = dataDB.SearchRefRangesByCalcTestId(pDBConnection, calcTestId, pfactoryDB)
'                If Not bRet Then Return myGlobalDataTO

'                myGlobalDataTO.SetDatos = dataDB

'            Catch ex As Exception
'                'Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.GetDataInDB", EventLogEntryType.Error, False)
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobalDataTO.ErrorMessage = ex.Message
'            End Try

'            Return myGlobalDataTO
'        End Function

'        Protected Overrides Function GetDataInFactoryDB(pDBConnection As SqlClient.SqlConnection, pItemRow As DataRow) As GlobalDataTO
'            Return GetDataInDB(pDBConnection, pItemRow, True)
'        End Function

'        Protected Overrides Function GetDataInLocalDB(pDBConnection As SqlClient.SqlConnection, pItemRow As DataRow) As GlobalDataTO
'            Return GetDataInDB(pDBConnection, pItemRow, False)
'        End Function

'        Protected Overrides Function IsDataEmpty(pDataInDB As Object) As Boolean
'            Return pDataInDB Is Nothing OrElse DirectCast(pDataInDB, DataInDB).IsDataEmpty()
'        End Function

'        ''' <summary>
'        ''' Gets if the related CalcTest is FactoryDefined (Preloaded)
'        ''' </summary>
'        ''' <param name="pDataInDB"></param>
'        ''' <returns></returns>
'        ''' <remarks></remarks>
'        Protected Overrides Function IsFactoryDefinedItem(pDataInDB As Object, pDBConnection As SqlClient.SqlConnection) As Boolean
'            Try
'                If (pDataInDB Is Nothing) Then Return False
'                Dim calcTestData As DataInDB = DirectCast(pDataInDB, DataInDB)
'                If (calcTestData.calcTestDS Is Nothing) Then Return False
'                If (calcTestData.calcTestDS.tparCalculatedTests.Rows.Count = 0) Then Return False

'                Dim row As CalculatedTestsDS.tparCalculatedTestsRow
'                row = DirectCast(calcTestData.calcTestDS.tparCalculatedTests.Rows(0), CalculatedTestsDS.tparCalculatedTestsRow)
'                Return row.PreloadedCalculatedTest And Not row.IsBiosystemsIDNull 'SGM 19/02/2013 add BiosystemsID field not null
'                'Return row.PreloadedCalculatedTest

'            Catch ex As Exception
'                'Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.IsFactoryDefinedItem", EventLogEntryType.Error, False)
'            End Try

'            Return False
'        End Function
'#End Region

'#Region " Factory Updates "
'        Protected Overrides Function GetAffectedItemsFromFactoryUpdates(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
'            Dim myGlobalDataTO As New GlobalDataTO
'            Try
'                myGlobalDataTO = CalculatedTestUpdateDAO.GetPreloadedCALCTestsDistinctInClient(pDBConnection)

'            Catch ex As Exception
'                'Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.GetAffectedItemsFromFactoryUpdates", EventLogEntryType.Error, False)
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobalDataTO.ErrorMessage = ex.Message
'            End Try

'            Return myGlobalDataTO
'        End Function

'        ''' <summary>
'        ''' Create new element
'        ''' </summary>
'        ''' <param name="pDBConnection"></param>
'        ''' <param name="pDataInFactoryDB"></param>
'        ''' <param name="pDataInLocalDB"></param>
'        ''' <returns></returns>
'        ''' <remarks>
'        ''' Created by: DL 07/02/2013
'        ''' Modified by: XB 15/02/2013 - set ref ranges as New (Bugs tracking #1134)
'        ''' Modified by: SG 20/02/2013 - rename other possible test before creting a new one in case it has already the same name or longname
'        ''' </remarks>
'        Protected Overrides Function DoCreateNewElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
'            Dim myGlobalDataTO As New GlobalDataTO
'            Try
'                If pDataInFactoryDB Is Nothing Then Return myGlobalDataTO
'                Dim dataInFactoryDB As DataInDB = DirectCast(pDataInFactoryDB, DataInDB)
'                If dataInFactoryDB.IsDataEmpty Then Return myGlobalDataTO

'                'Create new CALC Test like in Factory DB
'                'All data to insert is inside dataInFactoryDB

'                'SGM 20/02/2013
'                'before create from factory find any possible user test with the same Name or ShortName
'                myGlobalDataTO = MyClass.RenameCalcTestNameWhenUpdateOrCreateNew(pDBConnection, dataInFactoryDB)
'                If myGlobalDataTO.HasError Then Return myGlobalDataTO

'                Dim myCalTest As New CalculatedTestsDS
'                myCalTest = DirectCast(dataInFactoryDB.calcTestDS, CalculatedTestsDS)

'                For Each calcRow As CalculatedTestsDS.tparCalculatedTestsRow In myCalTest.tparCalculatedTests.Rows
'                    calcRow.BeginEdit()
'                    calcRow.PreloadedCalculatedTest = True
'                    calcRow.EndEdit()
'                Next

'                Dim myFormula As New FormulasDS
'                myFormula = DirectCast(dataInFactoryDB.formulaDS, FormulasDS)

'                Dim myRefRanges As New TestRefRangesDS
'                myRefRanges = DirectCast(dataInFactoryDB.refRangesDS, TestRefRangesDS)

'                If (myRefRanges.tparTestRefRanges.Rows.Count > 0) Then
'                    For Each refRangRow As TestRefRangesDS.tparTestRefRangesRow In myRefRanges.tparTestRefRanges
'                        refRangRow.BeginEdit()
'                        refRangRow.IsNew = True
'                        refRangRow.EndEdit()
'                    Next
'                End If

'                Dim calcTestDelegate As New CalculatedTestsDelegate
'                myGlobalDataTO = calcTestDelegate.Add(pDBConnection,
'                                                      myCalTest,
'                                                      myFormula,
'                                                      myRefRanges)


'            Catch ex As Exception
'                'Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.DoCreateNewElement", EventLogEntryType.Error, False)
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobalDataTO.ErrorMessage = ex.Message
'            End Try

'            Return myGlobalDataTO
'        End Function

'        Protected Overrides Function DoUpdateUserDefinedElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
'            Dim myGlobalDataTO As New GlobalDataTO
'            Dim mycalcTestDS As New CalculatedTestsDS

'            Try
'                If pDataInLocalDB Is Nothing Then Return myGlobalDataTO
'                Dim dataInLocalDB As DataInDB = DirectCast(pDataInLocalDB, DataInDB)
'                If dataInLocalDB.IsDataEmpty Then Return myGlobalDataTO

'                mycalcTestDS = DirectCast(pDataInLocalDB, DataInDB).calcTestDS

'                If mycalcTestDS.tparCalculatedTests.Rows.Count > 0 Then

'                    'Already exists in client DataBase as User test
'                    If Not mycalcTestDS.tparCalculatedTests.First.PreloadedCalculatedTest Then
'                        'Rename calibrator name adding (R) at the beginning of the name validting that not exist in db


'                        'SGM 15/02/2013 - bug #1134
'                        myGlobalDataTO = MyClass.RenameCalcTestName(pDBConnection, mycalcTestDS.tparCalculatedTests.First)
'                        If Not myGlobalDataTO.HasError Then
'                            'Dim IsOK As Boolean = False
'                            'Dim myNewname As String = ""
'                            'Dim myCalculatedTests As New tparCalculatedTestsDAO

'                            'While Not IsOK
'                            '    myNewname &= "R" & mycalcTestDS.tparCalculatedTests.First.CalcTestName

'                            '    myGlobalDataTO = myCalculatedTests.ReadByCalcTestName(pDBConnection, myNewname)

'                            '    If Not myGlobalDataTO.HasError Then
'                            '        Dim pcalcTestDS As New CalculatedTestsDS
'                            '        pcalcTestDS = DirectCast(myGlobalDataTO.SetDatos, CalculatedTestsDS)

'                            '        If pcalcTestDS.tparCalculatedTests.Rows.Count = 0 Then IsOK = True
'                            '    End If
'                            'End While

'                            'For Each calcRow As CalculatedTestsDS.tparCalculatedTestsRow In mycalcTestDS.tparCalculatedTests
'                            '    calcRow.BeginEdit()
'                            '    calcRow.CalcTestName = myNewname
'                            '    calcRow.EndEdit()
'                            'Next



'                            'Rename client user CALC Test in client Database and all related data. Adding (R) at the beginning of the name
'                            Dim calcTestDelegate As New CalculatedTestsDelegate
'                            Dim myFormulas As New FormulasDS
'                            Dim myRefRanges As New TestRefRangesDS

'                            myFormulas = DirectCast(pDataInLocalDB, DataInDB).formulaDS
'                            myRefRanges = DirectCast(pDataInLocalDB, DataInDB).refRangesDS

'                            myGlobalDataTO = calcTestDelegate.Modify(pDBConnection, mycalcTestDS, myFormulas, myRefRanges)

'                            'Create preloaded CALC Test in client DataBase
'                            If Not myGlobalDataTO.HasError Then
'                                mycalcTestDS = DirectCast(pDataInFactoryDB, DataInDB).calcTestDS

'                                For Each calcRow As CalculatedTestsDS.tparCalculatedTestsRow In mycalcTestDS.tparCalculatedTests
'                                    calcRow.BeginEdit()
'                                    calcRow.PreloadedCalculatedTest = True
'                                    calcRow.EndEdit()
'                                Next

'                                myFormulas = DirectCast(pDataInFactoryDB, DataInDB).formulaDS
'                                myRefRanges = DirectCast(pDataInFactoryDB, DataInDB).refRangesDS

'                                myGlobalDataTO = calcTestDelegate.Add(pDBConnection, mycalcTestDS, myFormulas, myRefRanges)
'                            End If
'                        End If
'                        'end SGM 15/02/2013 - bug #1134

'                    Else
'                        'Already exists in client DataBase as Preloaded test
'                        Dim calcTestDelegate As New CalculatedTestsDelegate
'                        Dim myFormulas As New FormulasDS
'                        Dim myRefRanges As New TestRefRangesDS

'                        myFormulas = DirectCast(pDataInLocalDB, DataInDB).formulaDS
'                        myRefRanges = DirectCast(pDataInLocalDB, DataInDB).refRangesDS

'                        myGlobalDataTO = calcTestDelegate.Modify(pDBConnection, mycalcTestDS, myFormulas, myRefRanges)

'                    End If
'                End If

'            Catch ex As Exception
'                'Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.DoUpdateUserDefinedElement", EventLogEntryType.Error, False)
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobalDataTO.ErrorMessage = ex.Message
'            End Try

'            Return myGlobalDataTO
'        End Function

'        ''' <summary>
'        ''' Update Calc Test as Factory definition
'        ''' </summary>
'        ''' <param name="pDBConnection"></param>
'        ''' <param name="pDataInFactoryDB"></param>
'        ''' <param name="pDataInLocalDB"></param>
'        ''' <returns></returns>
'        ''' <remarks>
'        ''' Created by XB: 14/02/2013 -  (Bugs tracking #1134)
'        ''' Modified by: SG 20/02/2013 - rename other possible test before creting a new one in case it has already the same name or longname
'        ''' </remarks>
'        Protected Overrides Function DoUpdateFactoryDefinedElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
'            Dim myGlobalDataTO As New GlobalDataTO

'            Try
'                If pDataInFactoryDB Is Nothing Then Return myGlobalDataTO
'                Dim dataInFactoryDB As DataInDB = DirectCast(pDataInFactoryDB, DataInDB)
'                If dataInFactoryDB.IsDataEmpty Then Return myGlobalDataTO

'                If pDataInLocalDB Is Nothing Then Return myGlobalDataTO
'                Dim dataInLocalDB As DataInDB = DirectCast(pDataInLocalDB, DataInDB)
'                If dataInLocalDB.IsDataEmpty Then Return myGlobalDataTO

'                'SGM 20/02/2013
'                'before update from factory find any possible user test with the same Name or ShortName
'                myGlobalDataTO = MyClass.RenameCalcTestNameWhenUpdateOrCreateNew(pDBConnection, dataInFactoryDB)
'                If myGlobalDataTO.HasError Then Return myGlobalDataTO

'                ' Take Calculated Test FACTORY values 
'                Dim mycalcTest As New CalculatedTestsDS
'                mycalcTest = DirectCast(dataInFactoryDB.calcTestDS, CalculatedTestsDS)
'                ' Take Calculated Test LOCAL values 
'                Dim myInLocalCalTest As New CalculatedTestsDS
'                myInLocalCalTest = DirectCast(dataInLocalDB.calcTestDS, CalculatedTestsDS)

'                ' Take Formula FACTORY values 
'                Dim myFormula As New FormulasDS
'                myFormula = DirectCast(dataInFactoryDB.formulaDS, FormulasDS)

'                ' Take Ref Ranges depending on type
'                Dim myRefRanges As New TestRefRangesDS
'                Dim UserChoice As Boolean = True
'                ' At begining Get FACTORY Ref Ranges values
'                myRefRanges = DirectCast(dataInFactoryDB.refRangesDS, TestRefRangesDS)
'                If (myRefRanges.tparTestRefRanges.Rows.Count > 0) Then
'                    Dim myTestRefRanges As New List(Of TestRefRangesDS.tparTestRefRangesRow)

'                    myTestRefRanges = (From a In myRefRanges.tparTestRefRanges _
'                                       Where a.RangeType = "GENERIC" _
'                                       Select a).ToList()

'                    If myTestRefRanges.Count > 0 Then
'                        UserChoice = False
'                    End If
'                End If

'                ' Check if there are Ref ranges into local DB
'                Dim myInLocalRefRanges As New TestRefRangesDS
'                myInLocalRefRanges = DirectCast(dataInLocalDB.refRangesDS, TestRefRangesDS)
'                If (myInLocalRefRanges.tparTestRefRanges.Rows.Count > 0) Then
'                    ' There are !
'                    ' Take USER ActiveRangeType in case exists
'                    Dim myActiveRangeType As String = ""
'                    For Each calcRow As CalculatedTestsDS.tparCalculatedTestsRow In myInLocalCalTest.tparCalculatedTests.Rows
'                        myActiveRangeType = calcRow.ActiveRangeType
'                    Next
'                    For Each calcRow As CalculatedTestsDS.tparCalculatedTestsRow In mycalcTest.tparCalculatedTests.Rows
'                        calcRow.BeginEdit()
'                        calcRow.ActiveRangeType = myActiveRangeType
'                        calcRow.EndEdit()
'                    Next
'                End If


'                If UserChoice Then
'                    ' Get USER Ref Ranges values in case there are not GENERIC Ref Ranges from Factory
'                    myRefRanges = myInLocalRefRanges
'                End If

'                ' Check the if ID self-generated has changed, means it has a different value than the factory
'                If Not myInLocalCalTest.tparCalculatedTests.First.CalcTestID = myInLocalCalTest.tparCalculatedTests.First.BiosystemsID Then
'                    ' Assign the same ID that must be used for updates

'                    'Set value of Calculated Test ID in the dataset containing the Calc Test Values
'                    For i As Integer = 0 To mycalcTest.tparCalculatedTests.Rows.Count - 1
'                        mycalcTest.tparCalculatedTests(i).CalcTestID = myInLocalCalTest.tparCalculatedTests.First.CalcTestID
'                    Next i

'                    'Set value of Calculated Test ID in the dataset containing the Formula Values
'                    For i As Integer = 0 To myFormula.tparFormulas.Rows.Count - 1
'                        myFormula.tparFormulas(i).CalcTestID = myInLocalCalTest.tparCalculatedTests.First.CalcTestID
'                    Next i

'                    'Set value of Calculated Test ID in the dataset containing the Reference Ranges
'                    For i As Integer = 0 To myRefRanges.tparTestRefRanges.Rows.Count - 1
'                        myRefRanges.tparTestRefRanges(i).TestID = myInLocalCalTest.tparCalculatedTests.First.CalcTestID
'                    Next
'                End If

'                Dim calcTestDelegate As New CalculatedTestsDelegate
'                myGlobalDataTO = calcTestDelegate.Modify(pDBConnection, mycalcTest, myFormula, myRefRanges)

'            Catch ex As Exception
'                'Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.DoUpdateFactoryDefinedElement", EventLogEntryType.Error, False)
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobalDataTO.ErrorMessage = ex.Message
'            End Try

'            Return myGlobalDataTO
'        End Function

'        Protected Overrides Function DoIgnoreElementUpdating(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
'            'No action is needed
'            Return New GlobalDataTO
'        End Function

'        ''' <summary>
'        ''' Renames Calulated Test Name and LongName before updating or creting new from factory
'        ''' </summary>
'        ''' <param name="pDBConnection"></param>
'        ''' <param name="pDataInFactoryDB"></param>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 20/02/2013</remarks>
'        Private Function RenameCalcTestNameWhenUpdateOrCreateNew(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As DataInDB) As GlobalDataTO

'            Dim myGlobalDataTO As New GlobalDataTO

'            Try

'                Dim myDAO As New tparCalculatedTestsDAO

'                'rename because of same NAME
'                Dim calcTestName As String = pDataInFactoryDB.calcTestDS.tparCalculatedTests.First.CalcTestName
'                myGlobalDataTO = myDAO.ReadByCalcTestName(pDBConnection, calcTestName)
'                If Not myGlobalDataTO.HasError AndAlso myGlobalDataTO.SetDatos IsNot Nothing Then
'                    Dim myDS As CalculatedTestsDS = CType(myGlobalDataTO.SetDatos, CalculatedTestsDS)
'                    If myDS.tparCalculatedTests.Rows.Count > 0 Then
'                        If Not myDS.tparCalculatedTests.First.PreloadedCalculatedTest Then
'                            myGlobalDataTO = MyClass.RenameCalcTestName(pDBConnection, myDS.tparCalculatedTests.First)
'                            If Not myGlobalDataTO.HasError Then
'                                myGlobalDataTO = myDAO.Update(pDBConnection, myDS)
'                            End If
'                        End If
'                    End If
'                End If

'                'rename because of same LONG NAME
'                Dim calcTestLongName As String = pDataInFactoryDB.calcTestDS.tparCalculatedTests.First.CalcTestLongName
'                myGlobalDataTO = myDAO.ReadByCalcTestLongName(pDBConnection, calcTestName)
'                If Not myGlobalDataTO.HasError AndAlso myGlobalDataTO.SetDatos IsNot Nothing Then
'                    Dim myDS As CalculatedTestsDS = CType(myGlobalDataTO.SetDatos, CalculatedTestsDS)
'                    If myDS.tparCalculatedTests.Rows.Count > 0 Then
'                        If Not myDS.tparCalculatedTests.First.PreloadedCalculatedTest Then
'                            myGlobalDataTO = MyClass.RenameCalcTestName(pDBConnection, myDS.tparCalculatedTests.First)
'                            If Not myGlobalDataTO.HasError Then
'                                myGlobalDataTO = myDAO.Update(pDBConnection, myDS)
'                            End If
'                        End If
'                    End If
'                End If

'            Catch ex As Exception
'                'Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.RenameCalcTestNameWhenUpdateOrCreateNew", EventLogEntryType.Error, False)
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobalDataTO.ErrorMessage = ex.Message
'            End Try

'            Return myGlobalDataTO
'        End Function

'        ''' <summary>
'        ''' Execute required final actions
'        ''' </summary>
'        ''' <param name="pDBConnection"></param>
'        ''' <returns></returns>
'        ''' <remarks>
'        ''' Created by XB 21/02/2013 - (Bugs tracking #1134)
'        ''' </remarks>
'        Public Overrides Function DoFinalActions(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
'            Dim myGlobalDataTO As New GlobalDataTO
'            Try
'                myGlobalDataTO = UpdateDeprecatedIdentifiers(pDBConnection)
'            Catch ex As Exception
'                'Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.DoFinalActions", EventLogEntryType.Error, False)
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobalDataTO.ErrorMessage = ex.Message
'            End Try
'            Return myGlobalDataTO
'        End Function

'        ''' <summary>
'        ''' Check Calc Test Identificadors that have been deprecated
'        ''' </summary>
'        ''' <param name="pDBConnection"></param>
'        ''' <returns></returns>
'        ''' <remarks>
'        ''' Created by XB 21/02/2013 - (Bugs tracking #1134)
'        ''' </remarks>
'        Private Function UpdateDeprecatedIdentifiers(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
'            Dim myGlobalDataTO As New GlobalDataTO
'            Try
'                Dim calTestList As New CalculatedTestsDelegate
'                Dim myFormulaDelegate As New FormulasDelegate

'                myGlobalDataTO = calTestList.GetList(pDBConnection)
'                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
'                    Dim calcTestLocalDS As New CalculatedTestsDS
'                    calcTestLocalDS = DirectCast(myGlobalDataTO.SetDatos, CalculatedTestsDS)
'                    Dim qCalcTests As List(Of CalculatedTestsDS.tparCalculatedTestsRow)

'                    qCalcTests = (From a In calcTestLocalDS.tparCalculatedTests _
'                                Select a).ToList()

'                    For Each calculatedTest As CalculatedTestsDS.tparCalculatedTestsRow In qCalcTests
'                        ' Check items that have different identifier between Local and Factory DB
'                        If calculatedTest.PreloadedCalculatedTest AndAlso _
'                           Not calculatedTest.IsBiosystemsIDNull AndAlso _
'                           calculatedTest.CalcTestID <> calculatedTest.BiosystemsID Then

'                            ' Get the original identifier in Factory
'                            Dim OriginalIDValue As Integer
'                            Dim dbName As String = GlobalBase.TemporalDBName
'                            Dim myDAO As New tparCalculatedTestsDAO
'                            myGlobalDataTO = myDAO.ReadByBiosystemsID(pDBConnection, calculatedTest.BiosystemsID, pDataBaseName:=dbName)
'                            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
'                                Dim calcTestFactoryDS As New CalculatedTestsDS()
'                                calcTestFactoryDS = DirectCast(myGlobalDataTO.SetDatos, CalculatedTestsDS)

'                                If calcTestFactoryDS.tparCalculatedTests.Rows.Count > 0 Then
'                                    OriginalIDValue = calcTestFactoryDS.tparCalculatedTests.First.CalcTestID
'                                Else
'                                    myGlobalDataTO.HasError = True
'                                End If
'                            End If

'                            'Get the list of specified Test included in the Formula of the Calculated Test
'                            myGlobalDataTO = myFormulaDelegate.GetCalculatedTestIntoFormula(pDBConnection, OriginalIDValue)
'                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
'                                Dim myFormulaDS As FormulasDS = DirectCast(myGlobalDataTO.SetDatos, FormulasDS)

'                                For Each testRow As FormulasDS.tparFormulasRow In myFormulaDS.tparFormulas.Rows
'                                    'For each item with deprecated identifier contained into Formula, update to new identifier
'                                    myFormulaDelegate.UpdateCalcTestValueAfterDBUpdate(pDBConnection, OriginalIDValue, calculatedTest.CalcTestID)
'                                Next

'                            End If

'                        End If
'                    Next

'                End If

'            Catch ex As Exception
'                'Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.UpdateDeprecatedIdentifiers", EventLogEntryType.Error, False)
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobalDataTO.ErrorMessage = ex.Message
'            End Try

'            Return myGlobalDataTO
'        End Function
'#End Region

'#Region " Factory Removes "
'        Protected Overrides Function GetAffectedItemsFromFactoryRemoves(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
'            Dim myGlobalDataTO As New GlobalDataTO
'            Try
'                myGlobalDataTO = CalculatedTestUpdateDAO.GetPreloadedCALCTestsDistinctInFactory(pDBConnection)

'            Catch ex As Exception
'                'Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.GetAffectedItemsFromFactoryRemoves", EventLogEntryType.Error, False)
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobalDataTO.ErrorMessage = ex.Message
'            End Try

'            Return myGlobalDataTO
'        End Function

'        ''' <summary>
'        ''' Remove User Element
'        ''' </summary>
'        ''' <param name="pDBConnection"></param>
'        ''' <param name="pDataInFactoryDB"></param>
'        ''' <param name="pDataInLocalDB"></param>
'        ''' <returns></returns>
'        ''' <remarks>
'        ''' Created by: DL 08/02/2013
'        ''' </remarks>
'        Protected Overrides Function DoRemoveUserElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
'            Dim myGlobalDataTO As New GlobalDataTO

'            Try
'                If pDataInLocalDB Is Nothing Then Return myGlobalDataTO
'                Dim dataInLocalDB As DataInDB = DirectCast(pDataInLocalDB, DataInDB)
'                If dataInLocalDB.IsDataEmpty Then Return myGlobalDataTO

'                Dim mycalcTestDS As New CalculatedTestsDS
'                mycalcTestDS = DirectCast(pDataInLocalDB, DataInDB).calcTestDS

'                If mycalcTestDS.tparCalculatedTests.Rows.Count > 0 Then
'                    If mycalcTestDS.tparCalculatedTests.First.PreloadedCalculatedTest Then
'                        'Already exists in client DataBase as Preloaded test
'                        'Rename client user CALC Test in client Database and all related data. Adding (R) at the beginning of the name
'                        Dim calcTestDelegate As New CalculatedTestsDelegate
'                        Dim myFormulas As New FormulasDS
'                        Dim myRefRanges As New TestRefRangesDS

'                        myFormulas = DirectCast(pDataInLocalDB, DataInDB).formulaDS
'                        myRefRanges = DirectCast(pDataInLocalDB, DataInDB).refRangesDS

'                        myGlobalDataTO = calcTestDelegate.Modify(pDBConnection, mycalcTestDS, myFormulas, myRefRanges)

'                        If Not myGlobalDataTO.HasError Then
'                            'Remove preloaded test in client DataBase
'                            myGlobalDataTO = calcTestDelegate.Delete(pDBConnection, mycalcTestDS)
'                        End If

'                    End If
'                End If

'            Catch ex As Exception
'                'Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.DoRemoveUserElement", EventLogEntryType.Error, False)
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobalDataTO.ErrorMessage = ex.Message
'            End Try

'            Return myGlobalDataTO
'        End Function

'        Protected Overrides Function DoRemoveFactoryElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
'            Dim myGlobalDataTO As New GlobalDataTO
'            Try
'                If pDataInLocalDB Is Nothing Then Return myGlobalDataTO
'                Dim dataInLocalDB As DataInDB = DirectCast(pDataInLocalDB, DataInDB)
'                If dataInLocalDB.IsDataEmpty Then Return myGlobalDataTO

'                'Remove CALC Test
'                Dim calcTestDelegate As New CalculatedTestsDelegate
'                myGlobalDataTO = calcTestDelegate.Delete(pDBConnection, dataInLocalDB.calcTestDS)

'                'Update in HTML Report?

'            Catch ex As Exception
'                'Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.DoRemoveFactoryElement", EventLogEntryType.Error, False)
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

'        ''' <summary>
'        ''' Returns the action to do with the concrete row in local DB if not match with Factory elements
'        ''' </summary>
'        ''' <param name="pDataInFactoryDB">Data of the element in Factory DB</param>
'        ''' <param name="pDataInLocalDB">Data of the element in Local DB</param>
'        ''' <returns>GlobalDataTO with LocalActionsWithFactoryRemoves</returns>
'        ''' <remarks>
'        ''' Created by: SGM 18/02/2013
'        ''' </remarks>
'        Protected Overrides Function GetActionFromFactoryRemoves(ByVal pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
'            Dim myGlobalDataTO As New GlobalDataTO
'            myGlobalDataTO.SetDatos = LocalActionsWithFactoryRemoves.Ignore
'            Try
'                ' If Not Exists in Factory DB and:
'                ' If Exists in Local DB as UserTest: Action = Ignored
'                ' If Exists in Local DB as PreloadedTest: Action = RemoveFactoryDefined

'                If IsDataEmpty(pDataInFactoryDB) Then
'                    If IsFactoryDefinedItem(pDataInLocalDB, pDBConnection) Then
'                        myGlobalDataTO.SetDatos = LocalActionsWithFactoryRemoves.RemoveFactoryDefined
'                    Else
'                        'not to remove if its not preloaded
'                        myGlobalDataTO.SetDatos = LocalActionsWithFactoryRemoves.Ignore
'                    End If
'                End If

'            Catch ex As Exception
'                'Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity("CALC Test Update Error.", "CalculatedTestUpdateData.GetActionFromFactoryRemoves", EventLogEntryType.Error, False)
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobalDataTO.ErrorMessage = ex.Message
'            End Try

'            Return myGlobalDataTO
'        End Function

'        ''' <summary>
'        ''' Returns the action to do with the concrete row in local DB, if Factory data not match with local elements
'        ''' </summary>
'        ''' <param name="pDataInFactoryDB">Data of the element in Factory DB</param>
'        ''' <param name="pDataInLocalDB">Data of the element in Local DB</param>
'        ''' <returns>GlobalDataTO with LocalActionsWithFactoryUpdates</returns>
'        ''' <remarks>
'        ''' Created by: SGM 18/02/2013
'        ''' </remarks>
'        Protected Overrides Function GetActionFromFactoryUpdates(ByVal pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
'            Dim myGlobalDataTO As New GlobalDataTO
'            myGlobalDataTO.SetDatos = LocalActionsWithFactoryUpdates.Ignore
'            Try
'                ' If Not Exists in Local DB: Action = CreateNew
'                ' If Exists in Local DB as UserTest: Action = UpdateUserDefined
'                ' If Exists in Local DB as PreloadedTest: Action = UpdateFactoryDefined

'                If IsDataEmpty(pDataInLocalDB) Then
'                    myGlobalDataTO.SetDatos = LocalActionsWithFactoryUpdates.CreateNew
'                ElseIf IsFactoryDefinedItem(pDataInLocalDB, pDBConnection) Then 'TR 08/05/2013 send the connection
'                    myGlobalDataTO.SetDatos = LocalActionsWithFactoryUpdates.UpdateFactoryDefined
'                Else
'                    myGlobalDataTO.SetDatos = LocalActionsWithFactoryUpdates.UpdateUserDefined
'                End If

'            Catch ex As Exception
'                'Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity("Elememt Update Error.", "CalculatedTestUpdateData.GetActionFromFactoryUpdates", EventLogEntryType.Error, False)
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobalDataTO.ErrorMessage = ex.Message
'            End Try

'            Return myGlobalDataTO
'        End Function
'#End Region
#End Region


