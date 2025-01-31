﻿
Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports System.IO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Controls.UserControls
'WIN32
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.PresentationCOM
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.App


Public Class TestISEMonitor
    Inherits BSBaseForm

    'Private mdiAnalyzerCopy As AnalyzerManager '#REFACTORING 

    Public SimulationMode As Boolean = False


    Private Sub PrepareISEMonitorIcons()
        Dim auxIconName As String = ""
        Dim iconPath As String = MyBase.IconsPath
        Dim myGlobal As New GlobalDataTO

        Dim myImageList As New ImageList

        Try


            'OK Icon
            auxIconName = GetIconName("STUS_FINISH")
            If File.Exists(iconPath & auxIconName) Then
                Dim myImage As Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                If myImage IsNot Nothing Then
                    myImageList.Images.Add(BSISEMonitorPanel.IconImages.Ok.ToString, myImage)
                End If
            End If

            'Warning Icon
            auxIconName = GetIconName("STUS_WITHERRS")
            If File.Exists(iconPath & auxIconName) Then
                Dim myImage As Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                If myImage IsNot Nothing Then
                    myImageList.Images.Add(BSISEMonitorPanel.IconImages.Warning.ToString, myImage)
                End If
            End If

            'Lock Icon
            auxIconName = GetIconName("STUS_LOCKED")
            If File.Exists(iconPath & auxIconName) Then
                Dim myImage As Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                If myImage IsNot Nothing Then
                    myImageList.Images.Add(BSISEMonitorPanel.IconImages.Locked.ToString, myImage)
                End If
            End If

            'Error Icon
            auxIconName = GetIconName("WARNINGSMALL")
            If File.Exists(iconPath & auxIconName) Then
                Dim myImage As Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                If myImage IsNot Nothing Then
                    myImageList.Images.Add(BSISEMonitorPanel.IconImages.Error_.ToString, myImage)
                End If
            End If

            Me.BsiseMonitorPanel1.ImagesData = myImageList

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareButtons", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

  
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pLanguageID"></param>
    ''' <remarks>
    ''' Modified by: XB 05/09/2014 - Take the ISE test names from the Name field on tparISETests table  instead of a multilanguage label - BA-1902
    ''' </remarks>
    Private Sub GetISEMonitorLabels(ByVal pLanguageID As String)

        Dim myGlobal As New GlobalDataTO
        Dim MLRD As New MultilanguageResourcesDelegate
        Dim myLabelsData As New Dictionary(Of BSISEMonitorPanel.LabelElements, String)
        Dim myWarningsData As New Dictionary(Of BSISEMonitorPanel.WarningElements, String)
        Dim dbConnection As SqlConnection = Nothing

        Try

            myGlobal = GetOpenDBConnection(Nothing)

            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                dbConnection = DirectCast(myGlobal.SetDatos, SqlConnection)
            End If

            If (Not dbConnection Is Nothing) Then

                myLabelsData.Add(BSISEMonitorPanel.LabelElements.RP_Title, MLRD.GetResourceText(dbConnection, "LBL_ISE_REAGENTS_PACK", pLanguageID))
                myLabelsData.Add(BSISEMonitorPanel.LabelElements.RP_InstallDate, MLRD.GetResourceText(dbConnection, "LBL_InstallDate", pLanguageID))
                myLabelsData.Add(BSISEMonitorPanel.LabelElements.RP_ExpireDate, MLRD.GetResourceText(dbConnection, "LBL_ExpDate_Full", pLanguageID))
                myLabelsData.Add(BSISEMonitorPanel.LabelElements.RP_RemainingVolume, MLRD.GetResourceText(dbConnection, "LBL_Remaining", pLanguageID))
                myLabelsData.Add(BSISEMonitorPanel.LabelElements.RP_InitialVolume, MLRD.GetResourceText(dbConnection, "LBL_InitialVolume", pLanguageID))
                myLabelsData.Add(BSISEMonitorPanel.LabelElements.RP_Calibrator, MLRD.GetResourceText(dbConnection, "LBL_WSPrep_Calibrator", pLanguageID))

                myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_Title, MLRD.GetResourceText(dbConnection, "LBL_ISE_Electrodes", pLanguageID))
                myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_InstallDate, MLRD.GetResourceText(dbConnection, "LBL_InstallDate", pLanguageID))
                myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_TestsCompleted, MLRD.GetResourceText(dbConnection, "LBL_TestsCompleted", pLanguageID))
                myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_Reference, MLRD.GetResourceText(dbConnection, "LBL_Reference", pLanguageID))

                ' XB 05/09/2014 - BA-1902
                'myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_Sodium, MLRD.GetResourceText(dbConnection, "LBL_Sodium", pLanguageID))
                'myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_Potassium, MLRD.GetResourceText(dbConnection, "LBL_Potassium", pLanguageID))
                'myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_Chlorine, MLRD.GetResourceText(dbConnection, "LBL_Chlorine", pLanguageID))
                'myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_Lithium, MLRD.GetResourceText(dbConnection, "LBL_Lithium", pLanguageID))
                Dim ISETestList As New ISETestsDelegate
                myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_Sodium, ISETestList.GetName(dbConnection, ISE_Tests.Na)) ' ID sodium = 1
                myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_Potassium, ISETestList.GetName(dbConnection, ISE_Tests.K)) ' ID potassium = 2
                myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_Chlorine, ISETestList.GetName(dbConnection, ISE_Tests.Cl)) ' ID chlorine = 3
                myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_Lithium, ISETestList.GetName(dbConnection, ISE_Tests.Li)) ' ID lithium = 4
                ' XB 05/09/2014 - BA-1902

                myLabelsData.Add(BSISEMonitorPanel.LabelElements.CAL_Title, MLRD.GetResourceText(dbConnection, "LBL_ISE_LastCalibrations", pLanguageID))
                myLabelsData.Add(BSISEMonitorPanel.LabelElements.CAL_LastDate, MLRD.GetResourceText(dbConnection, "LBL_LastDate", pLanguageID))
                myLabelsData.Add(BSISEMonitorPanel.LabelElements.CAL_Results, MLRD.GetResourceText(dbConnection, "LBL_Result", pLanguageID))
                myLabelsData.Add(BSISEMonitorPanel.LabelElements.CAL_Electrodes, MLRD.GetResourceText(dbConnection, "LBL_ISE_Electrodes", pLanguageID))
                myLabelsData.Add(BSISEMonitorPanel.LabelElements.CAL_Pumps, MLRD.GetResourceText(dbConnection, "LBL_ISE_Pumps", pLanguageID))
                myLabelsData.Add(BSISEMonitorPanel.LabelElements.CAL_Clean, MLRD.GetResourceText(dbConnection, "LBL_ISE_CLEANING", pLanguageID))

                myLabelsData.Add(BSISEMonitorPanel.LabelElements.TUB_Pump, MLRD.GetResourceText(dbConnection, "LBL_ISE_PUMP_TUBE", pLanguageID))
                myLabelsData.Add(BSISEMonitorPanel.LabelElements.TUB_Fluid, MLRD.GetResourceText(dbConnection, "LBL_ISE_TUBING_TUBE", pLanguageID))

                myWarningsData.Add(BSISEMonitorPanel.WarningElements.RP_NotInstalled, MLRD.GetResourceText(dbConnection, "LBL_ISE_RP_NOT_INSTALLED", pLanguageID))
                myWarningsData.Add(BSISEMonitorPanel.WarningElements.RP_Expired, MLRD.GetResourceText(dbConnection, "LBL_ISE_RP_EXPIRED", pLanguageID))
                myWarningsData.Add(BSISEMonitorPanel.WarningElements.RP_Depleted, MLRD.GetResourceText(dbConnection, "LGD_RotDeplet", pLanguageID))
                myWarningsData.Add(BSISEMonitorPanel.WarningElements.RP_WrongBiosystemsCode, MLRD.GetResourceText(dbConnection, "LBL_ISE_WRONG_BIO_CODE", pLanguageID))
                myWarningsData.Add(BSISEMonitorPanel.WarningElements.EL_NotInstalled, MLRD.GetResourceText(dbConnection, "LBL_ISE_EL_NOT_INSTALLED", pLanguageID))
                myWarningsData.Add(BSISEMonitorPanel.WarningElements.EL_ReplaceRecommended, MLRD.GetResourceText(dbConnection, "LBL_ISE_REPLACE_RECOMMENDED", pLanguageID))
                myWarningsData.Add(BSISEMonitorPanel.WarningElements.CAL_Recommended, MLRD.GetResourceText(dbConnection, "LBL_ISE_CALIB_RECOMMENDED", pLanguageID))
                myWarningsData.Add(BSISEMonitorPanel.WarningElements.CAL_Error, MLRD.GetResourceText(dbConnection, "LBL_ISE_CALIB_ERROR", pLanguageID))
                myWarningsData.Add(BSISEMonitorPanel.WarningElements.CAL_CleanRecommended, MLRD.GetResourceText(dbConnection, "LBL_ISE_CLEAN_RECOMMENDED", pLanguageID))
                myWarningsData.Add(BSISEMonitorPanel.WarningElements.RP_CleanPackInstalled, MLRD.GetResourceText(dbConnection, "LBL_ISE_CLEAN_PACK_INSTALLED", pLanguageID))
                myWarningsData.Add(BSISEMonitorPanel.WarningElements.TUB_NotInstalled, MLRD.GetResourceText(dbConnection, "LBL_NOT_INSTALLED", pLanguageID))
            End If

            Me.BsiseMonitorPanel1.LabelsData = myLabelsData
            Me.BsiseMonitorPanel1.WarnigsData = myWarningsData


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".GetScreenLabels", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)

        Finally
            If Not dbConnection Is Nothing Then dbConnection.Close()
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Modified by: IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Sub TestISEMonitor_1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try
            MyClass.PrepareISEMonitorIcons()
            MyClass.GetISEMonitorLabels("SPA")

            If MyClass.SimulationMode Then
                Dim myGlobal As GlobalDataTO = MyClass.SimulateMonitorData
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    Dim myMonitorData As ISEMonitorTO = CType(myGlobal.SetDatos, ISEMonitorTO)
                    Me.BsiseMonitorPanel1.RefreshFieldsData(myMonitorData)
                End If
            Else
                '#REFACTORING
                If AnalyzerController.Instance.Analyzer.ISEAnalyzer IsNot Nothing Then
                    Me.BsiseMonitorPanel1.RefreshFieldsData(AnalyzerController.Instance.Analyzer.ISEAnalyzer.MonitorDataTO)
                End If
            End If

            If My.Application.Info.AssemblyName.Contains("SERVICE") Then
                Me.ResetBorderSRV()
                Me.BsiseMonitorPanel1.Width = Me.Width
                Me.BsiseMonitorPanel1.Height = Me.Height
            Else
                Me.ResetBorder()
            End If



        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Function SimulateMonitorData() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            'ReagentsPack:
            Dim myRPInstallDate As DateTime = DateTime.Now.AddMonths(-3)
            Dim myRPExpireDate As DateTime = myRPInstallDate.AddMonths(6)
            Dim myRPIsExpired As Boolean = False
            Dim myRPRemVolA As Single = 368
            Dim myRPRemVolB As Single = 112
            Dim myRPInitVolA As Single = 520
            Dim myRPInitVolB As Single = 190
            Dim myRPEnoughA As Boolean = True
            Dim myRPEnoughB As Boolean = True
            Dim myRPHasBioCode As Boolean = True


            'Electrodes:
            Dim myRefData As ISEMonitorTO.ElectrodeData = New ISEMonitorTO.ElectrodeData(ISE_Electrodes.Ref)
            Dim myLiData As ISEMonitorTO.ElectrodeData = New ISEMonitorTO.ElectrodeData(ISE_Electrodes.Li)
            Dim myNaData As ISEMonitorTO.ElectrodeData = New ISEMonitorTO.ElectrodeData(ISE_Electrodes.Na)
            Dim myKData As ISEMonitorTO.ElectrodeData = New ISEMonitorTO.ElectrodeData(ISE_Electrodes.K)
            Dim myClData As ISEMonitorTO.ElectrodeData = New ISEMonitorTO.ElectrodeData(ISE_Electrodes.Cl)
            Dim myLithiumEnabled As Boolean = True

            With myRefData
                .Installed = True
                .InstallationDate = DateTime.Now.AddDays(-93)
                .IsExpired = False
                .IsOverUsed = False
                .TestCount = 391
            End With

            With myLiData
                .Installed = True 'MyClass.IsLiInstalled And MyClass.IsLiEnabledByUser
                .InstallationDate = DateTime.Now.AddDays(-94)
                .IsExpired = False
                .IsOverUsed = False
                .TestCount = 392
            End With

            With myNaData
                .Installed = True
                .InstallationDate = DateTime.Now.AddDays(-95)
                .IsExpired = False
                .IsOverUsed = True
                .TestCount = 393
            End With

            With myKData
                .Installed = True
                .InstallationDate = DateTime.Now.AddDays(-96)
                .IsExpired = False
                .IsOverUsed = False
                .TestCount = 394
            End With

            With myClData
                .Installed = True
                .InstallationDate = DateTime.Now.AddDays(-97)
                .IsExpired = False
                .IsOverUsed = False
                .TestCount = 395
            End With



            'Calibrations:

            Dim myCalElectroDate As DateTime = DateTime.Now.AddHours(-7)
            Dim myCalElectroString1 As String = "CAL Li 22.33 Na 44.55 K 66.77 Cl 88.99 A4F000D"
            Dim myCalElectroString2 As String = "CAL Li 11.22 Na 33.44 K 55.66 Cl 77.88 A4F000D"
            Dim myCalElectroResult1OK As Boolean = False
            Dim myCalElectroResult2OK As Boolean = True
            Dim myCalElectroRecomm As Boolean = False

            Dim myCalPumpsdate As DateTime = DateTime.Now.AddHours(-9)
            Dim myCalPumpsString As String = "PMC A 3000 B 2000 W 1000"
            Dim myCalPumpsResultOK As Boolean = True
            Dim myCalPumpsRecomm As Boolean = True

            Dim myCalBubbledate As DateTime = DateTime.Now.AddHours(-9)
            Dim myCalBubbleString As String = "BBC A 111 M 222 L 333 000000D"
            Dim myCalBubbleResultOK As Boolean = True
            Dim myCalBubbleRecomm As Boolean = True

            Dim myCleanDate As DateTime = DateTime.Now.AddHours(-17)
            Dim myCleanRecomm As Boolean = False

            Dim myMonitorData As New ISEMonitorTO(5, 5, myRPInstallDate, myRPExpireDate, myRPIsExpired, _
                                                  myRPRemVolA, myRPRemVolB, myRPInitVolA, myRPInitVolB, myRPEnoughA, myRPEnoughB, myRPHasBioCode, _
                                                  myRefData, myLiData, myNaData, myKData, myClData, myLithiumEnabled, _
                                                  myCalElectroDate, myCalElectroString1, myCalElectroString2, myCalElectroResult1OK, myCalElectroResult2OK, myCalElectroRecomm, _
                                                  myCalPumpsdate, myCalPumpsString, myCalPumpsResultOK, myCalPumpsRecomm, _
                                                  myCalBubbledate, myCalBubbleString, myCalBubbleResultOK, myCalBubbleRecomm, _
                                                  myCleanDate, myCleanRecomm, True, False, 4, 24)


            myGlobal.SetDatos = myMonitorData


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "ISEManager.SimulateMonitorData", EventLogEntryType.Error, False)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Refresh this specified screen with the information received from the Instrument
    ''' </summary>
    ''' <param name="pRefreshEventType"></param>
    ''' <param name="pRefreshDs"></param>
    ''' <remarks>Created by SGM 09/03/2012
    ''' Modified by: IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of UI_RefreshEvents), ByVal pRefreshDs As UIRefreshDS)
        Dim myGlobal As New GlobalDataTO
        Try
            If pRefreshEventType.Contains(UI_RefreshEvents.SENSORVALUE_CHANGED) Then
                Dim sensorValue As Single = 0

                'Monitor Data changed
                sensorValue = AnalyzerController.Instance.Analyzer.GetSensorValue(AnalyzerSensors.ISE_MONITOR_DATA_CHANGED)
                If sensorValue = 1 Then
                    ScreenWorkingProcess = False

                    AnalyzerController.Instance.Analyzer.SetSensorValue(AnalyzerSensors.ISE_MONITOR_DATA_CHANGED) = 0 'Once updated UI clear sensor

                    Me.BsiseMonitorPanel1.RefreshFieldsData(AnalyzerController.Instance.Analyzer.ISEAnalyzer.MonitorDataTO)

                    If AnalyzerController.Instance.Analyzer.Connected AndAlso AnalyzerController.Instance.Analyzer.AnalyzerStatus <> AnalyzerManagerStatus.SLEEPING Then
                        myGlobal = AnalyzerController.Instance.Analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.INFO, True, Nothing, Ax00InfoInstructionModes.STR) 'Start ANSINF
                    End If
                End If

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".RefreshScreen ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RefreshScreen", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
End Class
