Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL

Public Class ReagentsOnBoardDelegate

#Region "Business Methods"

    ''' <summary>
    ''' Calculates the real volume and number of remaining tests for a bottle position using the level control field sent by Analyzer
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pAnalyzerID" >Analyzer Identifier</param>
    ''' <param name="pWorkSessionID" >WorkSession Identifier</param>
    ''' <param name="pBottlePosition">Cell Number in Reagents Rotor</param>
    ''' <param name="pLevelControl">Level Control of the Reagent Bottle sent by the Analyzer</param>
    ''' <param name="pRealVolume">To return the current volume of the Reagent Bottle</param>
    ''' <param name="pTestLeft">To return the number of remaining Tests for the current volume of the Reagent Bottle</param>
    ''' <returns>GlobalDataTO containing success/error information. Value of parameters pRealVolume and pTestLeft</returns>
    ''' <remarks>
    ''' Created by:  AG 09/06/2011
    ''' Modified by: AG 22/02/2012 - Removed the bottle % calculation
    '''              SA 02/03/2012 - Informed parameter for the BottleType with calling function CalculateRemainingTests  
    '''              SA 07/03/2012 - Changed the function template
    '''              SA 29/06/2012 - Open a DBConnection instead a DBTransaction
    ''' </remarks>
    Public Function CalculateBottleVolumeTestLeft(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                  ByVal pBottlePosition As Integer, ByVal pLevelControl As Integer, ByRef pRealVolume As Single, _
                                                  ByRef pTestLeft As Integer) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing
        Try
            resultData = DAOBase.GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    'Search data of the type of Bottle positioned in the specified Rotor Cell (pBottlePosition)
                    Dim bottleTypesDelg As New ReagentTubeTypesDelegate
                    resultData = bottleTypesDelg.GetBottleInformationByRotorPosition(dbConnection, pAnalyzerID, pWorkSessionID, pBottlePosition)

                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        Dim volumeCalculated As Boolean = False

                        Dim bottleCfgDS As ReagentTubeTypesDS = DirectCast(resultData.SetDatos, ReagentTubeTypesDS)
                        If (bottleCfgDS.ReagentTubeTypes.Rows.Count > 0) Then
                            'Get the Bottle Code
                            Dim bottleCode As String = ""
                            If (Not bottleCfgDS.ReagentTubeTypes(0).IsTubeCodeNull) Then bottleCode = bottleCfgDS.ReagentTubeTypes(0).TubeCode

                            'Get the Bottle Section
                            Dim bottleSection As Single = 0
                            If (Not bottleCfgDS.ReagentTubeTypes(0).IsSectionNull) Then bottleSection = CSng(bottleCfgDS.ReagentTubeTypes(0).Section)

                            'Get the Bottle Volume when it is full
                            Dim bottleFullVolume As Single = 0
                            If (Not bottleCfgDS.ReagentTubeTypes(0).IsTubeVolumeNull) Then
                                pRealVolume = CSng(bottleCfgDS.ReagentTubeTypes(0).TubeVolume)
                                bottleFullVolume = pRealVolume
                            End If

                            'Get value of parameter for the vertical arm steps/mm relationship    
                            Dim swParameters As New SwParametersDelegate
                            resultData = swParameters.ReadByParameterName(dbConnection, GlobalEnumerates.SwParameters.STEPS_MM.ToString, Nothing)

                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim paramDS As ParametersDS = DirectCast(resultData.SetDatos, ParametersDS)
                                If (paramDS.tfmwSwParameters.Rows.Count > 0) Then
                                    Dim stepsMM As Single = 1
                                    If (Not paramDS.tfmwSwParameters(0).IsValueNumericNull) Then stepsMM = paramDS.tfmwSwParameters(0).ValueNumeric

                                    'Calculate the current bottle volume 
                                    pRealVolume = (bottleSection * pLevelControl * stepsMM) / 1000
                                    volumeCalculated = True

                                    ''Calculate bottle percentage
                                    'If bottleFullVolume > 0 Then
                                    '    'AG 22/02/2012 do not calculate the bottle %
                                    '    'pPercentage = 100 * pRealVolume / bottleFullVolume
                                    'End If
                                End If
                            End If
                        End If

                        'Finally, calculate the number of tests that can be executed with the current bottle volume
                        If (volumeCalculated) Then
                            'Search the ElementID of the Reagent positioned on the specified Rotor Cell
                            Dim rcpDelegate As New WSRotorContentByPositionDelegate
                            resultData = rcpDelegate.ReadByCellNumber(dbConnection, pAnalyzerID, pWorkSessionID, "REAGENTS", pBottlePosition)

                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim rcpDS As WSRotorContentByPositionDS = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)
                                If (rcpDS.twksWSRotorContentByPosition.Rows.Count > 0) Then
                                    'Get the ElementID
                                    Dim elemID As Integer = -1
                                    If (Not rcpDS.twksWSRotorContentByPosition(0).IsElementIDNull) Then elemID = rcpDS.twksWSRotorContentByPosition(0).ElementID

                                    'Get the TubeType 
                                    Dim tubeType As String = ""
                                    If (Not rcpDS.twksWSRotorContentByPosition(0).IsTubeTypeNull) Then tubeType = rcpDS.twksWSRotorContentByPosition(0).TubeType

                                    If (elemID <> -1 AndAlso tubeType <> String.Empty) Then
                                        Dim reqElementsDelegate As New WSRequiredElementsDelegate
                                        resultData = reqElementsDelegate.CalculateRemainingTests(dbConnection, pWorkSessionID, elemID, pRealVolume, tubeType)

                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            pTestLeft = CType(resultData.SetDatos, Integer)
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "ReagentsOnBoardDelegate.CalculateBottleVolumeTestLeft", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function


    ''' <summary>
    ''' check if the Volume validation is enable
    ''' Compares if the Real Volume detected by the BA400 is the same as the 
    ''' values Saved into thisReagentsBottles.BottleVolume.Get the ReagentsValue 
    ''' on the thisReagentsBottles and load saved Values.
    ''' in case volume validation is not correct then the bottle status change to LOCK.
    ''' After this validation on superior method the elemen on Rotor content by position status 
    ''' must be change to LOCKED STATUS.
    ''' In case reagent bottle do not exist on the thisReagentsBottles table then create it.
    ''' if exist the update the Botlle volume.
    ''' </summary>
    ''' <param name="pBarCodeID">Barcode ID</param>
    ''' <param name="pRealVolume">Recived Real Volume</param>
    ''' <returns></returns>
    ''' <remarks>CREATED BY: TR 12/06/2012
    '''          UPDATED BY: JV 09/01/2014 - BT #1443 </remarks>
    Public Function ReagentBottleManagement(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                            ByVal pBottlePos As Integer, ByVal pBarCodeID As String, ByVal pStatus As String, ByRef pRealVolume As Single) As GlobalDataTO
        Dim myGlobalDataTO As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing
        Try
            myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim myHisReagentsBottlesDelegate As New HisReagentBottlesDelegate
                    Dim myHisReagentsBottlesDS As HisReagentsBottlesDS
                    'Get the bottle information on the thisReagentBottles table.
                    myGlobalDataTO = myHisReagentsBottlesDelegate.ReadByBarCode(dbConnection, pBarCodeID)
                    If Not myGlobalDataTO.HasError Then
                        myHisReagentsBottlesDS = DirectCast(myGlobalDataTO.SetDatos, HisReagentsBottlesDS)
                        'Validate if Bottle exist on table.
                        If myHisReagentsBottlesDS.thisReagentsBottles.Count > 0 Then
                            Dim mySWParametersDelegate As New SwParametersDelegate
                            Dim ValidateReagentVolume As Boolean = False

                            'Validate if  volume validation  is enable.
                            myGlobalDataTO = mySWParametersDelegate.ReadTextValueByParameterName(dbConnection, _
                                                            GlobalEnumerates.SwParameters.REAG_VOL_VALIDATION.ToString(), Nothing)
                            If Not myGlobalDataTO.HasError Then
                                ValidateReagentVolume = CBool(myGlobalDataTO.SetDatos)
                            End If
                            If ValidateReagentVolume Then
                                'TR 27/09/2012 -Completed Requierement.

                                Dim myBarcodeWSDelegate As New BarcodeWSDelegate
                                'Decode Barcode to get bottle information
                                myGlobalDataTO = myBarcodeWSDelegate.DecodeReagentsBarCode(dbConnection, pBarCodeID, pAnalyzerID, pBottlePos)
                                If Not myGlobalDataTO.HasError Then
                                    Dim myDataSet As New BarCodesDS
                                    myDataSet = DirectCast(myGlobalDataTO.SetDatos, BarCodesDS)
                                    If myDataSet.DecodedReagentsFields.Count > 0 Then

                                        'Get the bottle size to get the REFILL ALLOWED LIMIT 
                                        Select Case myDataSet.DecodedReagentsFields(0).BottleType
                                            Case "BOTTLE3" '60ml
                                                'Get the Max Refill for 60 ml
                                                myGlobalDataTO = SwParametersDelegate.ReadNumValueByParameterName(dbConnection, _
                                                                                        GlobalEnumerates.SwParameters.REFILL_VOL_60ML.ToString(), Nothing)
                                                Exit Select
                                            Case "BOTTLE2" '20ml
                                                'Get the Max Refill for 20 ml
                                                myGlobalDataTO = SwParametersDelegate.ReadNumValueByParameterName(dbConnection, _
                                                                                        GlobalEnumerates.SwParameters.REFILL_VOL_20ML.ToString(), Nothing)
                                                Exit Select
                                        End Select

                                        Dim MaxRefillVolume As Integer = 0
                                        'Validate if not error then get variable value 
                                        If Not myGlobalDataTO.HasError Then
                                            MaxRefillVolume = CInt(myGlobalDataTO.SetDatos)
                                        End If

                                        'Validate if volume is correct Real Vol <= Historic Volumen + Refill Volume Allowed
                                        If (pRealVolume <= myHisReagentsBottlesDS.thisReagentsBottles(0).BottleVolume + MaxRefillVolume) Then
                                            'ACTIVE
                                            myHisReagentsBottlesDS.thisReagentsBottles(0).BottleStatus = "ACTIVE"
                                            'Update the real volume value only if active 
                                            myHisReagentsBottlesDS.thisReagentsBottles(0).BottleVolume = pRealVolume

                                        Else
                                            'LOCKED
                                            myHisReagentsBottlesDS.thisReagentsBottles(0).BottleStatus = "LOCKED"
                                            'Do not update the recive volume leave the last volume saved.
                                        End If
                                        myHisReagentsBottlesDS.thisReagentsBottles(0).Status = pStatus 'JV 09/01/2014 #1443
                                        'Update value on DB
                                        myGlobalDataTO = myHisReagentsBottlesDelegate.Update(dbConnection, myHisReagentsBottlesDS)
                                        'Set the bottle status to return. 
                                        myGlobalDataTO.SetDatos = myHisReagentsBottlesDS.thisReagentsBottles(0).BottleStatus

                                    End If
                                End If
                            Else
                                'Validate if status is not locked
                                If Not myHisReagentsBottlesDS.thisReagentsBottles(0).BottleStatus = "LOCKED" Then
                                    'Update the volume 
                                    myHisReagentsBottlesDS.thisReagentsBottles(0).BeginEdit()
                                    myHisReagentsBottlesDS.thisReagentsBottles(0).BottleVolume = pRealVolume
                                    myHisReagentsBottlesDS.thisReagentsBottles(0).BottleStatus = "ACTIVE"
                                    myHisReagentsBottlesDS.thisReagentsBottles(0).Status = pStatus 'JV 09/01/2014 #1443
                                    myHisReagentsBottlesDS.thisReagentsBottles(0).EndEdit()

                                    myGlobalDataTO = myHisReagentsBottlesDelegate.Update(dbConnection, myHisReagentsBottlesDS)
                                    'Set the bottle status to return. 
                                    myGlobalDataTO.SetDatos = myHisReagentsBottlesDS.thisReagentsBottles(0).BottleStatus
                                End If
                            End If
                        Else
                            'Create reagent bottle on hisReagentsBottlesDelegate
                            Dim myhisReagentsBottlesRow As HisReagentsBottlesDS.thisReagentsBottlesRow
                            myhisReagentsBottlesRow = myHisReagentsBottlesDS.thisReagentsBottles.NewthisReagentsBottlesRow
                            myhisReagentsBottlesRow.Barcode = pBarCodeID
                            myhisReagentsBottlesRow.BottleVolume = pRealVolume
                            myhisReagentsBottlesRow.BottleStatus = "ACTIVE"
                            myhisReagentsBottlesRow.Status = pStatus 'JV 09/01/2014 #1443
                            myHisReagentsBottlesDS.thisReagentsBottles.AddthisReagentsBottlesRow(myhisReagentsBottlesRow)
                            myGlobalDataTO = myHisReagentsBottlesDelegate.CreateReagentsBottle(dbConnection, myHisReagentsBottlesDS)

                            'Set the bottle status to return. 
                            myGlobalDataTO.SetDatos = myhisReagentsBottlesRow.BottleStatus

                        End If
                    End If
                End If
            End If

            If (Not myGlobalDataTO.HasError) Then
                'When the Database Connection was opened locally, then the Commit is executed
                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
            Else
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
            End If

        Catch ex As Exception
            'When the Database Connection was opened locally, then the Rollback is executed
            If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

            myGlobalDataTO = New GlobalDataTO()
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobalDataTO.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "ReagentsOnBoardDelegate.ValidateReagentsVolume", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return myGlobalDataTO
    End Function





#End Region

End Class
