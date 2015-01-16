'Option Strict On
'Option Explicit On

'Imports Biosystems.Ax00.Global
'Imports Biosystems.Ax00.Global.GlobalEnumerates
'Imports Biosystems.Ax00.Types
'Imports Biosystems.Ax00.DAL.DAO
'Imports Biosystems.Ax00.DAL
'Imports Biosystems.Ax00.BL

'Namespace Biosystems.Ax00.InfoAnalyzer
'    Public Class ISEInfoDelegate


'#Region "Constructor"
'        Public Sub New()

'        End Sub

'        Public Sub New(ByVal pAnalyzerID As String)
'            MyClass.AnalyzerIDAttr = pAnalyzerID
'        End Sub

'        Public Sub New(ByVal pAnalyzerID As String, ByVal pAnalyzerModel As String)
'            MyClass.AnalyzerIDAttr = pAnalyzerID
'            MyClass.AnalyzerModelAttr = pAnalyzerModel

'            MyClass.LoadISEParameters()

'        End Sub
'#End Region

'#Region "Declarations"

'        Private myISESwParametersDS As ParametersDS
'        Private myISEInformationDS As ISEInformationDS

'        Private myISEModuleManager As ISEModuleManager
'#End Region

'#Region "Attributes"
'        Private AnalyzerIDAttr As String = ""
'        Private AnalyzerModelAttr As String = ""

'        'Private IsISEPowerONAttr As Boolean = False '(ansinfo)
'        'Private IsISECommsOkAttr As Boolean = False ' (ise? o comunicaciones)
'        'Private IsReplacingElectrodesAttr As Boolean = False ' (Presentacion)
'        'Private IsReplacingReagentsAttr As Boolean = False ' (Presentacion)
'        'Private IsElectrodesReadyAttr As Boolean = False ' (Na, K, Cl Li*)
'        'Private IsReagentsReadyAttr As Boolean = False ' (DLWR)

'        'Private IsCalibrationNeededAttr As Boolean = False ' (thisCalibISE)
'        'Private IsPumpCalibrationNeededAttr As Boolean = False ' (thisCalibISE)

'        Private TodaysTestsWithoutCleanCountAttr As Integer = -1
'        'Private IsAutoConditioningDoneAttr As Boolean = False

'#End Region

'#Region "Private Enumerates"

'        Public Enum ExpirationItems
'            None
'            ReferenceElectrode
'            LithiumElectrode
'            SodiumElectrode
'            PotassiumElectrode
'            ClorineElectrode
'            CalibratorA
'            CalibratorB
'            PumpTubing
'            FluidicTubing
'        End Enum

'        Public Enum ValidationItems
'            None
'            LithiumElectrode
'            SodiumElectrode
'            PotassiumElectrode
'            ClorineElectrode
'            PumpCalibrationDiffAB
'            BubbleDetCalibrationDiffAL
'        End Enum
'#End Region

'#Region "Properties"



'        'This property is updated from presentation while Activating/Restoring Long Term Deactivation
'        Public ReadOnly Property IsLongTermDeactivation() As Boolean
'            Get
'                Dim myGlobal As New GlobalDataTO
'                myGlobal = MyClass.GetISEInfoFlagValue(GlobalEnumerates.ISEModuleSettings.LONG_TERM_DEACTIVATED)
'                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                    Return CBool(myGlobal.SetDatos)
'                Else
'                    Return Nothing
'                End If
'            End Get
'        End Property

'        'This property is updated from presentation
'        Public ReadOnly Property IsLithiumInstalled() As Boolean
'            Get
'                Dim myGlobal As New GlobalDataTO
'                myGlobal = MyClass.GetISEInfoFlagValue(GlobalEnumerates.ISEModuleSettings.LI_INSTALLED)
'                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                    Return CBool(myGlobal.SetDatos)
'                Else
'                    Return Nothing
'                End If
'            End Get
'        End Property



'        'This property is updated every test completed
'        Public Property TodaysTestsWithoutCleanCount() As Integer
'            Get
'                Return TodaysTestsWithoutCleanCountAttr
'            End Get
'            Set(ByVal value As Integer)
'                TodaysTestsWithoutCleanCountAttr = value
'            End Set
'        End Property

'#End Region

'#Region "Database Management"

'        ''' <summary>
'        ''' Creates a new ISE info master data with the informed dataset 
'        ''' </summary>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 24/01/2012</remarks>
'        Public Function CreateNewMasterData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISEInfoDS As ISEInformationDS) As GlobalDataTO
'            Dim resultData As New GlobalDataTO
'            Dim dbConnection As New SqlClient.SqlConnection
'            Try
'                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
'                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
'                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
'                    If (Not dbConnection Is Nothing) Then
'                        Dim myISESettingsDAO As New tinfoISEDAO
'                        resultData = myISESettingsDAO.CreateNewMasterData(dbConnection, pISEInfoDS)

'                        If (Not resultData.HasError) Then
'                            'When the Database Connection was opened locally, then the Commit is executed
'                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
'                        Else
'                            'When the Database Connection was opened locally, then the Rollback is executed
'                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
'                        End If
'                    End If
'                End If

'            Catch ex As Exception
'                resultData.HasError = True
'                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                resultData.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity(ex.Message, "ISEInfoDelegate.CreateNewMasterData", EventLogEntryType.Error, False)
'            Finally
'                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
'            End Try
'            Return resultData
'        End Function


'        ''' <summary>
'        ''' Gets all ISE Information (settings) by Analyzer Identifier
'        ''' </summary>
'        ''' <param name="pDBConnection"></param>
'        ''' <param name="pAnalyzerID"></param>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 24/01/2012</remarks>
'        Public Function ReadAllInfo(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
'            Dim resultData As New GlobalDataTO
'            Dim dbConnection As New SqlClient.SqlConnection
'            Try
'                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
'                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
'                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
'                    If (Not dbConnection Is Nothing) Then
'                        'Get data fields from the specified Analyzer
'                        Dim myISESettingsDAO As New tinfoISEDAO
'                        resultData = myISESettingsDAO.ReadAll(dbConnection, pAnalyzerID)
'                    End If
'                End If
'            Catch ex As Exception
'                resultData.HasError = True
'                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                resultData.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity(ex.Message, "ISEInfoDelegate.ReadAllInfo", EventLogEntryType.Error, False)
'            Finally
'                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
'            End Try
'            Return resultData
'        End Function

'        ''' <summary>
'        ''' Gets a specific ISE Information setting by Analyzer Identifier
'        ''' </summary>
'        ''' <param name="pDBConnection"></param>
'        ''' <param name="pAnalyzerID"></param>
'        ''' <param name="pISESettingID"></param>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 24/01/2012</remarks>
'        Public Function ReadInfoItemValue(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pISESettingID As GlobalEnumerates.ISEModuleSettings) As GlobalDataTO
'            Dim resultData As New GlobalDataTO
'            Dim dbConnection As New SqlClient.SqlConnection
'            Try
'                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
'                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
'                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
'                    If (Not dbConnection Is Nothing) Then
'                        'Get data fields from the specified Analyzer
'                        Dim myISESettingsDAO As New tinfoISEDAO
'                        resultData = myISESettingsDAO.ReadISESetting(dbConnection, pAnalyzerID, pISESettingID.ToString)
'                    End If
'                End If
'            Catch ex As Exception
'                resultData.HasError = True
'                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                resultData.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity(ex.Message, "ISEInfoDelegate.ReadInfoItemValue", EventLogEntryType.Error, False)
'            Finally
'                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
'            End Try
'            Return resultData
'        End Function



'        ''' <summary>
'        ''' Updates ISE information related to the informed Analyzer
'        ''' </summary>
'        ''' <param name="pDBConnection"></param>
'        ''' <param name="pAnalyzerID"></param>
'        ''' <param name="pISEInfoDS"></param>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 24/01/2012</remarks>
'        Public Function UpdateISEInfo(ByVal pDBConnection As SqlClient.SqlConnection, _
'                                          ByVal pAnalyzerID As String, _
'                                          ByVal pISEInfoDS As ISEInformationDS) As GlobalDataTO

'            Dim resultData As New GlobalDataTO
'            Dim dbConnection As New SqlClient.SqlConnection

'            Try
'                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
'                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
'                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
'                    If (Not dbConnection Is Nothing) Then
'                        Dim myISESettingsDAO As New tinfoISEDAO
'                        resultData = myISESettingsDAO.Update(dbConnection, pAnalyzerID, pISEInfoDS)

'                        If (Not resultData.HasError) Then
'                            'When the Database Connection was opened locally, then the Commit is executed
'                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
'                        Else
'                            'When the Database Connection was opened locally, then the Rollback is executed
'                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
'                        End If
'                    End If
'                End If
'            Catch ex As Exception
'                resultData.HasError = True
'                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                resultData.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity(ex.Message, "ISEInfoDelegate.UpdateISEInfo", EventLogEntryType.Error, False)
'            Finally
'                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
'            End Try

'            Return resultData

'        End Function


'#End Region


'#Region "Analyzer Information Management" 'QUITAR

'#Region "Public"


'        Public Function UpdateISEModuleManagerData(ByVal pISEModuleManager As ISEModuleManager) As GlobalDataTO
'            Dim myGlobal As New GlobalDataTO
'            Try

'            Catch ex As Exception
'                myGlobal.HasError = True
'                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobal.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity(ex.Message, "ISEInfoDelegate.UpdateISEModuleManagerData", EventLogEntryType.Error, False)
'            End Try
'            Return myGlobal
'        End Function

'        'VALIDATIONS
'        '****************************************************************************************************************************
'        Public Function ValidateDataLimits(ByVal pLimitsID As GlobalEnumerates.FieldLimitsEnum, ByVal pValue As Single) As GlobalDataTO
'            Dim myGlobal As New GlobalDataTO
'            Try
'                Dim myResult As Boolean = False
'                Dim myValidateMin As Single
'                Dim myValidateMax As Single
'                Dim myFieldLimitsDS As New FieldLimitsDS
'                Dim myFieldLimitsDelegate As New FieldLimitsDelegate()
'                'Load the specified limits
'                myGlobal = myFieldLimitsDelegate.GetList(Nothing, pLimitsID)
'                If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
'                    myFieldLimitsDS = CType(myGlobal.SetDatos, FieldLimitsDS)
'                    If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
'                        myValidateMin = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Single)
'                        myValidateMax = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Single)
'                        myResult = (pValue >= myValidateMin And pValue <= myValidateMax)
'                    End If
'                End If

'                myGlobal.SetDatos = myResult

'            Catch ex As Exception
'                myGlobal.HasError = True
'                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobal.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity(ex.Message, "ISEModuleModuleManager.GetValidationLimits", EventLogEntryType.Error, False)
'            End Try
'            Return myGlobal
'        End Function

'        ''' <summary>
'        ''' Returns if is needed to replace the informed expireable ISE element
'        ''' </summary>
'        ''' <param name="pItem"></param>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 06/02/2012</remarks>
'        Public Function CheckReplaceIsNeeded(ByVal pItem As ExpirationItems) As Boolean

'            Dim myResult As Boolean = False

'            Try
'                Dim myInstallationDate As DateTime
'                Dim myExpirationDate As DateTime

'                Dim myCurrentTestCount As Integer = -1
'                Dim myExpirationTestCount As Integer

'                Dim myGlobal As New GlobalDataTO


'                Dim myInfoInstallDate As GlobalEnumerates.ISEModuleSettings = GlobalEnumerates.ISEModuleSettings.NONE
'                Dim myInfoCurrentConsumption As GlobalEnumerates.ISEModuleSettings = GlobalEnumerates.ISEModuleSettings.NONE
'                Dim myParamExpireMonthCount As GlobalEnumerates.SwParameters
'                Dim myParamExpireTestCount As GlobalEnumerates.SwParameters

'                'determine the items for querying
'                Select Case pItem
'                    Case ExpirationItems.ReferenceElectrode
'                        myInfoInstallDate = GlobalEnumerates.ISEModuleSettings.REF_INSTALL_DATE
'                        myInfoCurrentConsumption = GlobalEnumerates.ISEModuleSettings.REF_CONSUMPTION
'                        myParamExpireMonthCount = GlobalEnumerates.SwParameters.ISE_EXPIRATION_TIME_REF
'                        myParamExpireTestCount = GlobalEnumerates.SwParameters.ISE_MAX_CONSUME_REF

'                    Case ExpirationItems.LithiumElectrode
'                        myInfoInstallDate = GlobalEnumerates.ISEModuleSettings.LI_INSTALL_DATE
'                        myInfoCurrentConsumption = GlobalEnumerates.ISEModuleSettings.LI_CONSUMPTION
'                        myParamExpireMonthCount = GlobalEnumerates.SwParameters.ISE_EXPIRATION_TIME_LI
'                        myParamExpireTestCount = GlobalEnumerates.SwParameters.ISE_MAX_CONSUME_LI

'                    Case ExpirationItems.SodiumElectrode
'                        myInfoInstallDate = GlobalEnumerates.ISEModuleSettings.NA_INSTALL_DATE
'                        myInfoCurrentConsumption = GlobalEnumerates.ISEModuleSettings.NA_CONSUMPTION
'                        myParamExpireMonthCount = GlobalEnumerates.SwParameters.ISE_EXPIRATION_TIME_NA
'                        myParamExpireTestCount = GlobalEnumerates.SwParameters.ISE_MAX_CONSUME_NA

'                    Case ExpirationItems.PotassiumElectrode
'                        myInfoInstallDate = GlobalEnumerates.ISEModuleSettings.K_INSTALL_DATE
'                        myInfoCurrentConsumption = GlobalEnumerates.ISEModuleSettings.K_CONSUMPTION
'                        myParamExpireMonthCount = GlobalEnumerates.SwParameters.ISE_EXPIRATION_TIME_K
'                        myParamExpireTestCount = GlobalEnumerates.SwParameters.ISE_MAX_CONSUME_K

'                    Case ExpirationItems.ClorineElectrode
'                        myInfoInstallDate = GlobalEnumerates.ISEModuleSettings.CL_INSTALL_DATE
'                        myInfoCurrentConsumption = GlobalEnumerates.ISEModuleSettings.CL_CONSUMPTION
'                        myParamExpireMonthCount = GlobalEnumerates.SwParameters.ISE_EXPIRATION_TIME_CL
'                        myParamExpireTestCount = GlobalEnumerates.SwParameters.ISE_MAX_CONSUME_CL

'                    Case ExpirationItems.PumpTubing
'                        myInfoInstallDate = GlobalEnumerates.ISEModuleSettings.PUMP_TUBING_INSTALL_DATE
'                        myParamExpireMonthCount = GlobalEnumerates.SwParameters.ISE_EXPIRATION_TIME_PUMP_TUBING

'                    Case ExpirationItems.FluidicTubing
'                        myInfoInstallDate = GlobalEnumerates.ISEModuleSettings.FLUID_TUBING_INSTALL_DATE
'                        myParamExpireMonthCount = GlobalEnumerates.SwParameters.ISE_EXPIRATION_TIME_FLUID_TUBING

'                    Case ExpirationItems.CalibratorA, ExpirationItems.CalibratorB

'                    Case Else
'                        Return False

'                End Select

'                Dim DontCheckConsumption As Boolean = (pItem = ExpirationItems.PumpTubing OrElse pItem = ExpirationItems.FluidicTubing)

'                'get Installation date
'                myGlobal = MyClass.GetISEInfoFlagValue(myInfoInstallDate)
'                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                    myInstallationDate = CDate(myGlobal.SetDatos)
'                End If

'                If Not DontCheckConsumption Then
'                    'get current test count
'                    myGlobal = MyClass.GetISEInfoFlagValue(myInfoCurrentConsumption)
'                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                        myCurrentTestCount = CInt(myGlobal.SetDatos)
'                    End If
'                End If

'                If myInstallationDate <> Nothing And myCurrentTestCount >= 0 Then
'                    'calculate expiration date
'                    myGlobal = MyClass.GetISEParameterValue(myParamExpireMonthCount)
'                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                        Dim myUsableMonths As Integer = CInt(myGlobal.SetDatos)
'                        myExpirationDate = myInstallationDate.AddMonths(myUsableMonths)
'                    End If

'                    If Not DontCheckConsumption And myCurrentTestCount >= 0 Then
'                        'get usable test count
'                        myGlobal = MyClass.GetISEParameterValue(myParamExpireTestCount)
'                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                            myExpirationTestCount = CInt(myGlobal.SetDatos)
'                        End If
'                    End If

'                    If (Now >= myExpirationDate) Or (myCurrentTestCount >= myExpirationTestCount) Then
'                        myResult = False
'                    Else
'                        myResult = True
'                    End If

'                Else
'                    myResult = False
'                End If

'                Return myResult

'            Catch ex As Exception
'                myResult = Nothing
'            End Try

'            Return myResult

'        End Function

'        ''' <summary>
'        ''' Returns if is needed to replace the Reagents Pack
'        ''' </summary>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 06/02/2012</remarks>
'        Public Function CheckReplaceReagentsPackIsNeeded() As Boolean
'            Dim myResult As Boolean = False
'            Dim myGlobal As New GlobalDataTO
'            Try


'            Catch ex As Exception
'                myResult = Nothing
'            End Try
'            Return myResult
'        End Function


'        ''' <summary>
'        ''' Returns if it is requested to perform a cleaning operation
'        ''' </summary>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 06/02/2012</remarks>
'        Public Function CheckCleaningIsNeeded() As Boolean
'            Dim myResult As Boolean = False
'            Dim myGlobal As New GlobalDataTO
'            Try
'                Dim myLastCleanDate As DateTime
'                Dim myMaximumTestsWithoutClean As Integer = -1

'                'get Lst Clean flag
'                myGlobal = MyClass.GetISEInfoFlagValue(GlobalEnumerates.ISEModuleSettings.LAST_CLEAN_DATE)
'                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                    myLastCleanDate = CDate(myGlobal.SetDatos)
'                End If

'                If myLastCleanDate >= Now.Date Then
'                    'get Maximum Test for requiring Sample
'                    myGlobal = MyClass.GetISEParameterValue(GlobalEnumerates.SwParameters.ISE_CLEAN_REQUIRED_SAMPLES)
'                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                        myMaximumTestsWithoutClean = CInt(myGlobal.SetDatos)
'                        If MyClass.TodaysTestsWithoutCleanCount < myMaximumTestsWithoutClean Then
'                            myResult = True
'                        End If
'                    End If
'                Else
'                    myResult = True
'                End If

'            Catch ex As Exception
'                myResult = Nothing
'            End Try
'            Return myResult
'        End Function

'        ''' <summary>
'        ''' 
'        ''' </summary>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 06/02/2012</remarks>
'        Public Function CheckPumpCalibrationIsNeeded() As Boolean
'            Dim myResult As Boolean = False
'            Dim myGlobal As New GlobalDataTO
'            Try
'                Dim myLastPumpCalibrationDate As DateTime
'                Dim myISECalibHisDelegate As New ISECalibHistoryDelegate

'                myGlobal = myISECalibHisDelegate.ReadAll(Nothing, MyClass.AnalyzerIDAttr)
'                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                    Dim myCalibHisDS As HistoryISECalibrationsDS = CType(myGlobal.SetDatos, HistoryISECalibrationsDS)
'                    Dim myCalibHisRows As New List(Of HistoryISECalibrationsDS.thisCalibISERow)
'                    myCalibHisRows = (From a As HistoryISECalibrationsDS.thisCalibISERow _
'                                        In myCalibHisDS.thisCalibISE _
'                                        Where a.AnalyzerID = MyClass.AnalyzerIDAttr _
'                                        And a.CalibrationType = GlobalEnumerates.ISECalibrationTypes.Pumps.ToString _
'                                        Select a Order By a.CalibrationDate).ToList()

'                    If myCalibHisRows.Count > 0 Then
'                        myLastPumpCalibrationDate = myCalibHisRows.Last.CalibrationDate
'                    End If
'                End If

'                'THE LAST CALIBRATION MUST BE VALIDATED OK!!!!!!!!!!!
'                If myLastPumpCalibrationDate < Now.Date Then
'                    myResult = True
'                End If

'            Catch ex As Exception
'                myResult = Nothing
'            End Try
'            Return myResult
'        End Function




'#End Region

'#Region "Private"


'#End Region



'        ''' <summary>
'        ''' Gets the informed Analyzer's information Setting' value
'        ''' </summary>
'        ''' <param name="pISESettingID"></param>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 06/02/2012</remarks>
'        Private Function GetISEInfoFlagValue(ByVal pISESettingID As GlobalEnumerates.ISEModuleSettings) As GlobalDataTO
'            Dim myGlobal As New GlobalDataTO
'            Try
'                If MyClass.AnalyzerIDAttr.Length > 0 AndAlso MyClass.myISEInformationDS IsNot Nothing Then

'                    Dim myInfoRows As New List(Of ISEInformationDS.tinfoISERow)
'                    myInfoRows = (From a As ISEInformationDS.tinfoISERow _
'                                        In MyClass.myISEInformationDS.tinfoISE _
'                                        Where a.AnalyzerID = MyClass.AnalyzerIDAttr _
'                                        And a.ISESettingID = pISESettingID.ToString _
'                                        Select a).ToList()

'                    If myInfoRows.Count > 0 Then
'                        Dim myValue As String = myInfoRows.First.Value
'                    Else
'                        myGlobal.SetDatos = Nothing
'                    End If
'                Else
'                    myGlobal.SetDatos = Nothing
'                End If

'            Catch ex As Exception
'                myGlobal.HasError = True
'                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobal.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity(ex.Message, "ISEModuleModuleManager.GetISEInfoFlagValue", EventLogEntryType.Error, False)
'            End Try
'            Return myGlobal
'        End Function

'        ''' <summary>
'        ''' Gets the value of the informed ISE related parameter
'        ''' </summary>
'        ''' <param name="pISEParameterName"></param>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 06/02/2012</remarks>
'        Private Function GetISEParameterValue(ByVal pISEParameterName As GlobalEnumerates.SwParameters) As GlobalDataTO
'            Dim myGlobal As New GlobalDataTO
'            Try
'                If MyClass.AnalyzerModelAttr.Length > 0 AndAlso MyClass.myISESwParametersDS IsNot Nothing Then

'                    Dim myParamsRows As New List(Of ParametersDS.tfmwSwParametersRow)
'                    myParamsRows = (From a As ParametersDS.tfmwSwParametersRow _
'                                        In MyClass.myISESwParametersDS.tfmwSwParameters _
'                                        Where a.AnalyzerModel = MyClass.AnalyzerModelAttr _
'                                        And a.ParameterName.Trim.ToUpper = pISEParameterName.ToString.ToUpper _
'                                        Select a).ToList()

'                    If myParamsRows.Count > 0 Then
'                        Dim myValue As Single = myParamsRows.First.ValueNumeric
'                    Else
'                        myGlobal.SetDatos = Nothing
'                    End If
'                Else
'                    myGlobal.SetDatos = Nothing
'                End If

'            Catch ex As Exception
'                myGlobal.HasError = True
'                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobal.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity(ex.Message, "ISEModuleModuleManager.GetISEParameterValue", EventLogEntryType.Error, False)
'            End Try
'            Return myGlobal
'        End Function


'        ''' <summary>
'        ''' Loads all ISE Parameters
'        ''' </summary>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 06/02/2012</remarks>
'        Private Function LoadISEParameters() As GlobalDataTO
'            Dim myGlobal As New GlobalDataTO
'            Dim myParams As New SwParametersDelegate
'            Dim myAllParametersDS As New ParametersDS
'            Dim myGlobalbase As New GlobalBase
'            Try
'                If MyClass.AnalyzerModelAttr.Length > 0 Then
'                    myGlobal = myParams.GetAllISEList(Nothing, MyClass.AnalyzerModelAttr)
'                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
'                        MyClass.myISESwParametersDS = CType(myGlobal.SetDatos, ParametersDS)
'                    End If
'                Else
'                    myGlobal.SetDatos = Nothing
'                End If

'            Catch ex As Exception
'                myGlobal.HasError = True
'                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobal.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                GlobalBase.CreateLogActivity(ex.Message, "ISEModuleModuleManager.LoadISEParameters", EventLogEntryType.Error, False)
'            End Try
'            Return myGlobal
'        End Function

















'#End Region




'    End Class




'End Namespace