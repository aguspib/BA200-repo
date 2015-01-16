Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.FwScriptsManagement

Public Class HistoricalReportsDelegate

#Region "Attributes"
    ' Language
    Private currentLanguageAttr As String
    Private AnalyzerIDAttr As String = "" 'SGM 20/01/2012
#End Region

#Region "Properties"
    Public Property currentLanguage() As String
        Get
            Return Me.currentLanguageAttr
        End Get
        Set(ByVal value As String)
            Me.currentLanguageAttr = value
        End Set
    End Property


    'SGM 20/01/2012
    Public Property AnalyzerID() As String
        Get
            Return AnalyzerIDAttr
        End Get
        Set(ByVal value As String)
            AnalyzerIDAttr = value
        End Set
    End Property
#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Get all activities contents done with the Instrument
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>XBC 27/07/2011</remarks>
    Public Function GetAllResultsService(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                         Optional ByVal pTaskID As String = "", Optional ByVal pActionID As String = "", _
                                         Optional ByVal pDateFrom As DateTime = Nothing, Optional ByVal pDateTo As DateTime = Nothing) _
                                         As GlobalDataTO
        Dim resultdataToReturn As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing
        Try
            Dim resultdata As New GlobalDataTO
            Dim myResultsDAO As New thrsResultsServiceDAO
            Dim myRecommendationsDAO As New thrsRecommendationsServiceDAO
            Dim myResultsDS As New SRVResultsServiceDS
            Dim myRecommendationsDS As New SRVRecommendationsServiceDS
            Dim myResultsDecodedDS As New SRVResultsServiceDecodedDS
            Dim myResultsRow As SRVResultsServiceDecodedDS.srv_thrsResultsServiceRow
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            resultdata = DAOBase.GetOpenDBConnection(pDBConnection)
            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                dbConnection = CType(resultdata.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then

                    Dim myResultsList As New List(Of SRVResultsServiceDS.srv_thrsResultsServiceRow)

                    resultdata = myResultsDAO.ReadAll(dbConnection, pAnalyzerID, pTaskID, pActionID, pDateFrom, pDateTo)

                    If resultdata IsNot Nothing AndAlso Not resultdata.HasError Then
                        myResultsDS = CType(resultdata.SetDatos, SRVResultsServiceDS)

                        myResultsList = (From a As SRVResultsServiceDS.srv_thrsResultsServiceRow In myResultsDS.srv_thrsResultsService Select a).ToList

                        For Each H As SRVResultsServiceDS.srv_thrsResultsServiceRow In myResultsList

                            myResultsRow = myResultsDecodedDS.srv_thrsResultsService.Newsrv_thrsResultsServiceRow

                            Dim taskDesc As String = ""
                            Dim actionDesc As String = ""
                            Dim recommendations As String = ""
                            Dim text As String = ""

                            Select Case H.TaskID
                                Case "ADJUST"

                                    Select Case H.ActionID

                                        Case "SCALES"
                                            '
                                            ' Scales Adjustment
                                            '
                                            Dim myTankLevelsDelegate As New TankLevelsAdjustmentDelegate

                                            resultdata = myTankLevelsDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJUST", currentLanguage)
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SCALES", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = MyClass.GetRecommendations(dbConnection, H)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    recommendations = CStr(resultdata.SetDatos)
                                                End If
                                                'resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                'If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                '    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                '    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                '        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                '    Next
                                                'End If

                                            End If

                                        Case "OPT"
                                            '
                                            ' Optic Centering Adjustment
                                            '
                                            Dim myPositionsDelegate As New PositionsAdjustmentDelegate

                                            resultdata = myPositionsDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJUST", currentLanguage)
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_OpticCenter", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = MyClass.GetRecommendations(dbConnection, H)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    recommendations = CStr(resultdata.SetDatos)
                                                End If
                                                'resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                'If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                '    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                '    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                '        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                '    Next
                                                'End If

                                            End If

                                        Case "WS_POS"
                                            '
                                            ' Washing Station Adjustment
                                            '
                                            Dim myPositionsDelegate As New PositionsAdjustmentDelegate

                                            resultdata = myPositionsDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJUST", currentLanguage)
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_WashStation", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = MyClass.GetRecommendations(dbConnection, H)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    recommendations = CStr(resultdata.SetDatos)
                                                End If
                                                'resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                'If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                '    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                '    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                '        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                '    Next
                                                'End If

                                            End If

                                        Case "SAM_POS", "REG1_POS", "REG2_POS", "MIX1_POS", "MIX2_POS"
                                            Dim myPositionsDelegate As New PositionsAdjustmentDelegate

                                            resultdata = myPositionsDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJUST", currentLanguage)
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ArmsPositions", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = MyClass.GetRecommendations(dbConnection, H)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    recommendations = CStr(resultdata.SetDatos)
                                                End If
                                                'resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                'If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                '    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                '    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                '        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                '    Next
                                                'End If

                                            End If

                                       

                                        Case "GLF_TER"
                                            '
                                            ' Photometry Thermo Adjustment
                                            '
                                            Dim myThermosAdjustmentsDelegate As New ThermosAdjustmentsDelegate

                                            resultdata = myThermosAdjustmentsDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJUST", currentLanguage)
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_GLF_TER", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If

                                        Case "REG1_TER"
                                            '
                                            ' Reagent1 Thermo Adjustment
                                            '
                                            Dim myThermosAdjustmentsDelegate As New ThermosAdjustmentsDelegate

                                            resultdata = myThermosAdjustmentsDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJUST", currentLanguage)
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REG1_TER", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If

                                        Case "REG2_TER"
                                            '
                                            ' Reagent2 Thermo Adjustment
                                            '
                                            Dim myThermosAdjustmentsDelegate As New ThermosAdjustmentsDelegate

                                            resultdata = myThermosAdjustmentsDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJUST", currentLanguage)
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REG2_TER", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If

                                        Case "HEAT_TER"
                                            '
                                            ' Washing Station Heater Thermo Adjustment
                                            '
                                            Dim myThermosAdjustmentsDelegate As New ThermosAdjustmentsDelegate

                                            resultdata = myThermosAdjustmentsDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJUST", currentLanguage)
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_HEAT_TER", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If

                                        Case "BARCODE"
                                            '
                                            ' Barcode Adjustment
                                            '
                                            Dim myBarCodeDelegate As New BarCodeAdjustmentDelegate

                                            resultdata = myBarCodeDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJUST", currentLanguage)
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TITLE_BARCODE", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If

                                    End Select

                               




                                Case "TEST"

                                    Select Case H.ActionID

                                        Case "BL_DC"
                                            '
                                            ' BaseLine & Darkness Tests
                                            '
                                            Dim myPhotometryDelegate As New PhotometryAdjustmentDelegate

                                            resultdata = myPhotometryDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", currentLanguage) 'JB 01/10/2012 - Resource String unification
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_BL_DC", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If

                                        Case "REPEAT"
                                            '
                                            ' Repeatability Test
                                            '
                                            Dim myPhotometryDelegate As New PhotometryAdjustmentDelegate

                                            resultdata = myPhotometryDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", currentLanguage) 'JB 01/10/2012 - Resource String unification
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REP_TEST", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If


                                        Case "STAB"
                                            '
                                            ' Stability Test
                                            '
                                            Dim myPhotometryDelegate As New PhotometryAdjustmentDelegate

                                            resultdata = myPhotometryDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", currentLanguage) 'JB 01/10/2012 - Resource string unification
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_STA_TEST", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If

                                        Case "GLF_TER"
                                            '
                                            ' Photometry Thermo Test
                                            '
                                            Dim myThermosAdjustmentsDelegate As New ThermosAdjustmentsDelegate

                                            resultdata = myThermosAdjustmentsDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", currentLanguage) 'JB 01/10/2012 - Resource string unification
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_GLF_TER", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If

                                        Case "REG1_TER"
                                            '
                                            ' Reagent1 Thermo Test
                                            '
                                            Dim myThermosAdjustmentsDelegate As New ThermosAdjustmentsDelegate

                                            resultdata = myThermosAdjustmentsDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", currentLanguage) 'JB 01/10/2012 - Resource String unification
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REG1_TER", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If

                                        Case "REG2_TER"
                                            '
                                            ' Reagent2 Thermo Test
                                            '
                                            Dim myThermosAdjustmentsDelegate As New ThermosAdjustmentsDelegate

                                            resultdata = myThermosAdjustmentsDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", currentLanguage) 'JB 01/10/2012 - Resource string unification
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REG2_TER", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If

                                        Case "HEAT_TER"
                                            '
                                            ' Washing Station Heater Thermo Test
                                            '
                                            Dim myThermosAdjustmentsDelegate As New ThermosAdjustmentsDelegate

                                            resultdata = myThermosAdjustmentsDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", currentLanguage) 'JB 01/10/2012 - Resource string unification
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_HEAT_TER", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If

                                        Case "ABS"
                                            '
                                            ' Absorbance Test
                                            '
                                            Dim myPhotometryDelegate As New PhotometryAdjustmentDelegate

                                            resultdata = myPhotometryDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", currentLanguage) 'JB 01/10/2012 - Resource String unification
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Full", currentLanguage) ' JB 01/10/2012 - Resource String unification
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If


                                        Case "SCALES"
                                            '
                                            ' Scales Test Not Applied
                                            '

                                        Case "INTERMEDIATE"
                                            '
                                            ' Intermediate Test
                                            '
                                            Dim myTankLevelsDelegate As New TankLevelsAdjustmentDelegate

                                            resultdata = myTankLevelsDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", currentLanguage) 'JB 01/10/2012 - Resource string unification
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_IntermediateTanks", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = MyClass.GetRecommendations(dbConnection, H)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    recommendations = CStr(resultdata.SetDatos)
                                                End If
                                                'resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                'If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                '    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                '    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                '        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                '    Next
                                                'End If

                                            End If

                                        

                                        Case "SAM_POS", "REG1_POS", "REG2_POS", "MIX1_POS", "MIX2_POS"
                                            Dim myPositionsDelegate As New PositionsAdjustmentDelegate

                                            resultdata = myPositionsDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TEST", currentLanguage)
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ArmsPositions", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = MyClass.GetRecommendations(dbConnection, H)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    recommendations = CStr(resultdata.SetDatos)
                                                End If
                                                'resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                'If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                '    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                '    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                '        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                '    Next
                                                'End If

                                            End If


                                        Case "MIX1_TEST", "MIX2_TEST"
                                            Dim myPositionsDelegate As New PositionsAdjustmentDelegate

                                            resultdata = myPositionsDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TEST", currentLanguage)
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_StirrerTest", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = MyClass.GetRecommendations(dbConnection, H)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    recommendations = CStr(resultdata.SetDatos)
                                                End If
                                                'resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                'If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                '    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                '    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                '        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                '    Next
                                                'End If

                                            End If


                                        Case "STRESS"
                                            '
                                            ' Stress Tests
                                            '
                                            Dim myStressModeTestDelegate As New StressModeTestDelegate

                                            resultdata = myStressModeTestDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", currentLanguage) 'JB 01/10/2012 - Resource string unification
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_STRESS", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If

                                        Case "INT_DOS", "EXT_WASH", "WS_ASP", "WS_DISP", "IN_OUT"
                                            '
                                            ' Motors, Pumps, tests
                                            '
                                            Dim myMotorsPumpsValvesTestDelegate As New MotorsPumpsValvesTestDelegate

                                            resultdata = myMotorsPumpsValvesTestDelegate.DecodeDataReport(H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", currentLanguage) 'JB 01/10/2012 - Resource string unification

                                                Select Case H.ActionID
                                                    Case "INT_DOS" : actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_InternalDosingTitle", currentLanguage)
                                                    Case "EXT_WASH" : actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ExternalWashingTitle", currentLanguage)
                                                    Case "WS_ASP" : actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_WsAspirationTitle", currentLanguage)
                                                    Case "WS_DISP" : actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_WsDispensationTitle", currentLanguage)
                                                    Case "IN_OUT" : actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_IN_OUT", currentLanguage)

                                                End Select

                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If

                                        Case "COLLISION"
                                            '
                                            ' Collision test
                                            '
                                            Dim myMotorsPumpsValvesTestDelegate As New MotorsPumpsValvesTestDelegate

                                            resultdata = myMotorsPumpsValvesTestDelegate.DecodeDataReport(H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", currentLanguage) 'JB 01/10/2012 - Resource string unification
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CollisionTest", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If

                                        Case "GLF_ENC"
                                            '
                                            ' Encoder test
                                            '
                                            Dim myMotorsPumpsValvesTestDelegate As New MotorsPumpsValvesTestDelegate

                                            resultdata = myMotorsPumpsValvesTestDelegate.DecodeDataReport(H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", currentLanguage) 'JB 01/10/2012 - Resource string unification
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_EncoderTest", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If


                                        Case "BARCODE"
                                            '
                                            ' Barcode Tests
                                            '
                                            Dim myBarCodeDelegate As New BarCodeAdjustmentDelegate

                                            resultdata = myBarCodeDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", currentLanguage) 'JB 01/10/2012 - Resource string unification
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TITLE_BARCODE", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If

                                        Case "LEVEL_FREQ_READ"
                                            '
                                            ' Level Detection Frequencies
                                            '
                                            Dim myLevelDetectionDelegate As New LevelDetectionTestDelegate

                                            resultdata = myLevelDetectionDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", currentLanguage) 'JB 01/10/2012 - Resource string unification
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FREQUENCY_READ", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If


                                        Case "LEVEL_DET_TEST"
                                            '
                                            ' Level Detection test
                                            '
                                            Dim myLevelDetectionDelegate As New LevelDetectionTestDelegate

                                            resultdata = myLevelDetectionDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", currentLanguage) 'JB 01/10/2012 - Resource string unification
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_DETECTION_TEST", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If

                                    End Select

                                Case "UTIL"

                                    Select Case H.ActionID

                                        Case "DEMO"
                                            '
                                            ' Demo Mode
                                            '
                                            Dim myDemoModeDelegate As New DemoModeDelegate

                                            resultdata = myDemoModeDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_Utilities", currentLanguage)
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_DEMO", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If

                                        Case "COMM"
                                            '
                                            ' Communications Settings
                                            '
                                            resultdata = MyClass.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_Utilities", currentLanguage)
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_COMM", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If

                                        Case "ADJ_BK", "ADJ_RES", "ADJ_FAC" 'backup / restore adjustments
                                            Dim myInstrumentUpdateUtilDelegate As New InstrumentUpdateUtilDelegate

                                            resultdata = myInstrumentUpdateUtilDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_Utilities", currentLanguage)
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJUTIL_TAB", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If

                                        Case "FW_UPT" 'Instrument Firmware Update
                                            Dim myInstrumentUpdateUtilDelegate As New InstrumentUpdateUtilDelegate

                                            resultdata = myInstrumentUpdateUtilDelegate.DecodeDataReport(H.TaskID, H.ActionID, H.Data, currentLanguage)
                                            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                Dim ContentData As String
                                                ContentData = CType(resultdata.SetDatos, String)

                                                taskDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_Utilities", currentLanguage)
                                                actionDesc = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FWUTIL_TAB", currentLanguage)
                                                text = ContentData
                                                recommendations = ""

                                                resultdata = myRecommendationsDAO.ReadByResultServiceID(dbConnection, H.ResultServiceID)
                                                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                                                    myRecommendationsDS = CType(resultdata.SetDatos, SRVRecommendationsServiceDS)

                                                    For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                                                        recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                                                    Next
                                                End If

                                            End If

                                       

                                    End Select

                                Case Else
                                    myResultsRow = Nothing

                            End Select

                            If myResultsRow IsNot Nothing Then
                                myResultsRow.ResultServiceID = H.ResultServiceID
                                myResultsRow.TaskDesc = taskDesc
                                myResultsRow.ActionDesc = actionDesc
                                myResultsRow.Data = text
                                myResultsRow.Comments = H.Comments
                                myResultsRow.AnalyzerID = H.AnalyzerID
                                myResultsRow.TS_User = H.TS_User
                                myResultsRow.TS_DateTime = H.TS_DateTime
                                myResultsRow.Recommendations = recommendations
                                myResultsRow.isModified = False

                                myResultsDecodedDS.srv_thrsResultsService.Rows.Add(myResultsRow)
                            End If
                        Next

                        resultdataToReturn = New GlobalDataTO
                        resultdataToReturn.SetDatos = myResultsDecodedDS
                    End If

                End If
            End If

        Catch ex As Exception
            resultdataToReturn = New GlobalDataTO
            resultdataToReturn.HasError = True
            resultdataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultdataToReturn.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "HistoricalReportsDelegate.GetAllResultsService", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultdataToReturn
    End Function

    ''' <summary>
    ''' Get the different analyzers that have been registered activities with this software
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>XBC 03/08/2011</remarks>
    Public Function GetAnalyzerResultsService(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        Dim resultdata As New GlobalDataTO
        Dim dbConnection As SqlClient.SqlConnection = Nothing
        Try
            Dim myResultsDAO As New thrsResultsServiceDAO
            Dim myResultsDS As New SRVResultsServiceDS

            resultdata = DAOBase.GetOpenDBConnection(pDBConnection)
            If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                dbConnection = CType(resultdata.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then

                    resultdata = myResultsDAO.ReadByAnalyzer(dbConnection)

                    If resultdata IsNot Nothing AndAlso Not resultdata.HasError Then
                        myResultsDS = CType(resultdata.SetDatos, SRVResultsServiceDS)

                        resultdata = New GlobalDataTO
                        resultdata.SetDatos = myResultsDS
                    End If

                End If
            End If

        Catch ex As Exception
            resultdata = New GlobalDataTO
            resultdata.HasError = True
            resultdata.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultdata.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "HistoricalReportsDelegate.GetAnalyzerResultsService", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultdata
    End Function

    ''' <summary>
    ''' Create a new Historic Report
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <remarks>
    ''' Created by: XBC 01/08/2011
    ''' </remarks>
    Public Function Add(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistoricReport As SRVResultsServiceDS.srv_thrsResultsServiceRow) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing
        Try
            resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
            If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    'Insert the new Historic Report
                    Dim HistoricReportToAdd As New thrsResultsServiceDAO

                    resultData = HistoricReportToAdd.Create(dbConnection, pHistoricReport)
                    If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
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
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "HistoricalReportsDelegate.Add", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Create a new Recomendation for an specific Historic Report
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <remarks>
    ''' Created by: XBC 03/08/2011
    ''' </remarks>
    Public Function AddRecommendations(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRecommendationsList As SRVRecommendationsServiceDS) As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Dim dbConnection As New SqlClient.SqlConnection
        Try
            resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    'Insert the new Recommendation
                    Dim RecommendationToAdd As New thrsRecommendationsServiceDAO

                    resultData = RecommendationToAdd.Create(dbConnection, pRecommendationsList)
                    If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
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
            GlobalBase.CreateLogActivity(ex.Message, "HistoricalReportsDelegate.AddRecommendations", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Update comments field in Historics Report
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <remarks>
    ''' Created by: XBC 04/08/2011
    ''' </remarks>
    Public Function UpdateComments(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultServiceList As SRVResultsServiceDS) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing
        Try
            resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
            If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    'Update Historic Report
                    Dim HistoricReportToModify As New thrsResultsServiceDAO

                    resultData = HistoricReportToModify.UpdateComments(dbConnection, pResultServiceList)
                    If Not resultData.HasError Then
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
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "HistoricalReportsDelegate.UpdateComments", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Delete an specified Historic Report
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <remarks>
    ''' Created by: XBC 04/08/2011
    ''' </remarks>
    Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultServiceList As SRVResultsServiceDS) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing
        Try
            resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
            If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    'Delete Historic Report
                    Dim HistoricReportToDelete As New thrsResultsServiceDAO

                    For Each rowthrsResultService As SRVResultsServiceDS.srv_thrsResultsServiceRow In pResultServiceList.srv_thrsResultsService.Rows
                        resultData = HistoricReportToDelete.Delete(dbConnection, rowthrsResultService.ResultServiceID)

                        If resultData.HasError Then
                            Exit For
                        Else
                            ' Delete recommendation records of the Historic Report deleted
                            Dim RecommendationToDelete As New thrsRecommendationsServiceDAO

                            resultData = RecommendationToDelete.Delete(dbConnection, rowthrsResultService.ResultServiceID)
                        End If
                    Next

                    If Not resultData.HasError Then
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
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "HistoricalReportsDelegate.Delete", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pResultsRow"></param>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 04/08/2011</remarks>
    Private Function GetRecommendations(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultsRow As SRVResultsServiceDS.srv_thrsResultsServiceRow) As GlobalDataTO

        Dim myResultData As New GlobalDataTO
        'Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

        Try
            Dim myRecommendationsDAO As New thrsRecommendationsServiceDAO
            Dim myRecommendationsDS As New SRVRecommendationsServiceDS

            Dim recommendations As String = ""


            myResultData = myRecommendationsDAO.ReadByResultServiceID(pDBConnection, pResultsRow.ResultServiceID)
            If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
                myRecommendationsDS = CType(myResultData.SetDatos, SRVRecommendationsServiceDS)

                For Each myrow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow In myRecommendationsDS.srv_thrsRecommendationsService.Rows
                    recommendations += Environment.NewLine + myrow.FixedItemDesc + Environment.NewLine
                Next
            End If

            myResultData.SetDatos = recommendations

        Catch ex As Exception
            myResultData = New GlobalDataTO()
            myResultData.HasError = True
            myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myResultData.ErrorMessage = ex.Message
            Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "HistoricalReportsDelegate.GetRecommendations", EventLogEntryType.Error, False)
        End Try

        Return myResultData

    End Function

    ''' <summary>
    ''' Method to decode the data information of the this screen from a String format source and obtain the data information easily legible
    ''' designed to elements from a Form which does not have its own delegate
    ''' </summary>
    ''' <param name="pTask">task identifier</param>
    ''' <param name="pAction">task's action identifier</param>
    ''' <param name="pData">content data with the information to format</param>
    ''' <param name="pcurrentLanguage">language identifier to localize contents</param>
    ''' <returns></returns>
    ''' <remarks>Created by XBC 07/09/2011</remarks>
    Public Function DecodeDataReport(ByVal pTask As String, ByVal pAction As String, ByVal pData As String, ByVal pcurrentLanguage As String) As GlobalDataTO
        Dim myResultData As New GlobalDataTO
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            Dim myUtility As New Utilities()
            Dim text1 As String
            Dim text As String = ""

             Select pTask
                Case "UTIL"

                    Select Case pAction
                        Case "COMM"

                            Dim j As Integer = 0
                            ' Final Result
                            If pData.Substring(j, 1) = "1" Then
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_SRV_COMM_OK", pcurrentLanguage)
                            Else
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_SRV_ERR_COMM", pcurrentLanguage)
                            End If
                            text += myUtility.FormatLineHistorics(text1)
                            j += 1

                            ' Communications mode
                            If pData.Substring(j, 1) = "0" Then
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Settings_Auto", pcurrentLanguage)
                            Else
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Settings_Manual", pcurrentLanguage)
                            End If
                            text += myUtility.FormatLineHistorics(text1)
                            j += 1

                            ' Speed value 
                            text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CfgAnalyzer_PortSpeed", pcurrentLanguage) + ": "
                            text1 += CSng(pData.Substring(j, 7)).ToString("#,###,##0")
                            text += myUtility.FormatLineHistorics(text1)
                            j += 7

                            ' Port value 
                            text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CfgAnalyzer_Port", pcurrentLanguage) + ": "
                            text1 += pData.Substring(j, pData.Length - j)
                            text += myUtility.FormatLineHistorics(text1)

                    End Select

            End Select

            myResultData.SetDatos = text

        Catch ex As Exception
            myResultData = New GlobalDataTO()
            myResultData.HasError = True
            myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myResultData.ErrorMessage = ex.Message
            Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "HistoricalReportsDelegate.DecodeDataReport", EventLogEntryType.Error, False)
        End Try
        Return myResultData
    End Function


#End Region

#Region "TO DELETE"

    'Select Case H.ActionID
    '    Case "1"

    '        ' Formating Information...
    '        text1 = "Dispensation Point:"
    '        text2 = "Predilution Point:"
    '        text3 = "Parking Point:"
    '        text += FormatLine(text1, text2, text3)

    '        text1 = "Polar Axis: 2.720"
    '        text2 = "Polar Axis: 2.719"
    '        text3 = "Polar Axis: 364"
    '        text += FormatLine(text1, text2, text3)

    '        text1 = "Z Axis: 1.382"
    '        text2 = "Z Axis: 1.382"
    '        text3 = "Z Axis: 1.332"
    '        text += FormatLine(text1, text2, text3)

    '        text1 = "Adjusted: YES"
    '        text2 = "Adjusted: NO"
    '        text3 = "Adjusted: NO"
    '        text += FormatLine(text1, text2, text3)

    '        text += Environment.NewLine

    '        text1 = "Z Ref Point:"
    '        text2 = "Washing Point:"
    '        text3 = "Z Tube Point:"
    '        text += FormatLine(text1, text2, text3)

    '        text1 = "Z Axis: 382"
    '        text2 = "Polar Axis: 1.292"
    '        text3 = "Z Axis: 1.950"
    '        text += FormatLine(text1, text2, text3)

    '        text1 = "Adjusted: YES"
    '        text2 = "Adjusted: YES"
    '        text3 = "Adjusted: NO"
    '        text += FormatLine(text1, text2, text3)

    '        text += Environment.NewLine

    '        text1 = "Ring1 Point:"
    '        text2 = "Ring2 Point:"
    '        text3 = "Ring3 Point:"
    '        text += FormatLine(text1, text2, text3)

    '        text1 = "Polar Axis: 1.599"
    '        text2 = "Polar Axis: 1.597"
    '        text3 = "Polar Axis: 1.578"
    '        text += FormatLine(text1, text2, text3)

    '        text1 = "Z Axis: 1.855"
    '        text2 = "Z Axis: 1.850"
    '        text3 = "Z Axis: 1.851"
    '        text += FormatLine(text1, text2, text3)

    '        text1 = "Rotor: 3.525"
    '        text2 = "Rotor: 334"
    '        text3 = "Rotor: 344"
    '        text += FormatLine(text1, text2, text3)

    '        text1 = "Adjusted: YES"
    '        text2 = "Adjusted: NO"
    '        text3 = "Adjusted: YES"
    '        text += FormatLine(text1, text2, text3)

    '        text += Environment.NewLine

    '        text1 = "Ise Point:"
    '        text += FormatLine(text1)

    '        text1 = "Polar Axis: -305"
    '        text += FormatLine(text1)

    '        text1 = "Z Axis: 2.086"
    '        text += FormatLine(text1)

    '        text1 = "Adjusted: NO"
    '        text += FormatLine(text1)
    '        ' Formating Information...

    'End Select


    'Private Const longText As Integer = 25
    'Private Const longSeparate As Integer = 1

    'Private Function FormatLine(Optional ByVal text1 As String = "", Optional ByVal text2 As String = "", Optional ByVal text3 As String = "") As String
    '    Dim returnValue As String = ""
    '    Try
    '        returnValue += text1
    '        returnValue += SetSpaces(longText - text1.Length)
    '        returnValue += SetSpaces(longSeparate)
    '        returnValue += text2
    '        returnValue += SetSpaces(longText - text2.Length)
    '        returnValue += SetSpaces(longSeparate)
    '        returnValue += text3
    '        returnValue += Environment.NewLine

    '    Catch ex As Exception
    '        Dim myLogAcciones As New ApplicationLogManager()
    '        GlobalBase.CreateLogActivity(ex.Message, "HistoricalReportsDelegate.FormatLine", EventLogEntryType.Error, False)
    '    End Try
    '    Return returnValue
    'End Function

    'Private Function SetSpaces(ByVal numSpaces As Integer) As String
    '    Dim returnValue As String = ""
    '    Try

    '        For i As Integer = 0 To numSpaces - 1
    '            returnValue += " "
    '        Next

    '    Catch ex As Exception
    '        Dim myLogAcciones As New ApplicationLogManager()
    '        GlobalBase.CreateLogActivity(ex.Message, "HistoricalReportsDelegate.SetSpaces", EventLogEntryType.Error, False)
    '    End Try
    '    Return returnValue
    'End Function

#End Region

End Class
