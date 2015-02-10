Option Explicit On
Option Strict On
Option Infer On

Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.BL

'Put here your business code for the tab ISETab inside Monitor Form
Partial Public Class UiMonitor



    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 08/03/2012</remarks>
    Private Sub InitializeISETab()
        Try
            MyClass.PrepareISEMonitorIcons()
            MyClass.GetISEMonitorLabels(MyClass.LanguageID)
            Me.BsIseMonitor.RefreshFieldsData(Nothing)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " InitializeISETab ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try

    End Sub



#Region "Public Methods"


#End Region

#Region "Private Methods"

    Private Sub PrepareISEMonitorIcons()
        Dim auxIconName As String = ""
        Dim iconPath As String = MyBase.IconsPath
        'Dim myGlobal As New GlobalDataTO

        Dim myImageList As New ImageList

        Try

            'OK Icon
            auxIconName = GetIconName("STUS_FINISH")
            If System.IO.File.Exists(iconPath & auxIconName) Then
                Dim myImage As Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                If myImage IsNot Nothing Then
                    myImageList.Images.Add(BSISEMonitorPanel.IconImages.Ok.ToString, myImage)
                End If
            End If

            'Warning Icon
            auxIconName = GetIconName("STUS_WITHERRS")
            If System.IO.File.Exists(iconPath & auxIconName) Then
                Dim myImage As Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                If myImage IsNot Nothing Then
                    myImageList.Images.Add(BSISEMonitorPanel.IconImages.Warning.ToString, myImage)
                End If
            End If

            'Lock Icon
            auxIconName = GetIconName("STUS_LOCKED")
            If System.IO.File.Exists(iconPath & auxIconName) Then
                Dim myImage As Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                If myImage IsNot Nothing Then
                    myImageList.Images.Add(BSISEMonitorPanel.IconImages.Locked.ToString, myImage)
                End If
            End If

            'Error Icon
            auxIconName = GetIconName("WARNINGSMALL")
            If System.IO.File.Exists(iconPath & auxIconName) Then
                Dim myImage As Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                If myImage IsNot Nothing Then
                    myImageList.Images.Add(BSISEMonitorPanel.IconImages.Error_.ToString, myImage)
                End If
            End If

            Me.BsIseMonitor.ImagesData = myImageList

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " PrepareISEMonitorIcons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pLanguageID"></param>
    ''' <remarks>
    ''' Modified by: RH 30/05/2012 No need to use a real connection in GetResourceText(). Pass Nothing instead.
    '''                            On the other hand, Presentation Layer should never use a direct connection to DB.
    '''              XB 05/09/2014 - Take the ISE test names from the Name field on tparISETests table  instead of a multilanguage label - BA-1902
    ''' </remarks>
    Private Sub GetISEMonitorLabels(ByVal pLanguageID As String)

        'Dim myGlobal As New GlobalDataTO
        Dim MLRD As New MultilanguageResourcesDelegate
        Dim myLabelsData As New Dictionary(Of Biosystems.Ax00.Controls.UserControls.BSISEMonitorPanel.LabelElements, String)
        Dim myWarningsData As New Dictionary(Of Biosystems.Ax00.Controls.UserControls.BSISEMonitorPanel.WarningElements, String)
        'Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try

            'myGlobal = DAOBase.GetOpenDBConnection(Nothing)

            'If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
            '    dbConnection = DirectCast(myGlobal.SetDatos, SqlClient.SqlConnection)
            'End If

            'If (Not dbConnection Is Nothing) Then

            myLabelsData.Add(BSISEMonitorPanel.LabelElements.RP_Title, MLRD.GetResourceText(Nothing, "LBL_ISE_REAGENTS_PACK", pLanguageID))
            myLabelsData.Add(BSISEMonitorPanel.LabelElements.RP_InstallDate, MLRD.GetResourceText(Nothing, "LBL_InstallDate", pLanguageID))
            myLabelsData.Add(BSISEMonitorPanel.LabelElements.RP_ExpireDate, MLRD.GetResourceText(Nothing, "LBL_ExpDate_Full", pLanguageID))
            myLabelsData.Add(BSISEMonitorPanel.LabelElements.RP_RemainingVolume, MLRD.GetResourceText(Nothing, "LBL_Remaining", pLanguageID))
            myLabelsData.Add(BSISEMonitorPanel.LabelElements.RP_InitialVolume, MLRD.GetResourceText(Nothing, "LBL_InitialVolume", pLanguageID))
            myLabelsData.Add(BSISEMonitorPanel.LabelElements.RP_Calibrator, MLRD.GetResourceText(Nothing, "LBL_WSPrep_Calibrator", pLanguageID))
            myLabelsData.Add(BSISEMonitorPanel.LabelElements.RP_Volume, MultilanguageResourcesDelegate.GetResourceText("LBL_Volumen") & " [mL]") 'TR 26/03/2012 -Add new element.

            myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_Title, MLRD.GetResourceText(Nothing, "LBL_ISE_Electrodes", pLanguageID))
            myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_InstallDate, MLRD.GetResourceText(Nothing, "LBL_InstallDate", pLanguageID))
            myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_TestsCompleted, MLRD.GetResourceText(Nothing, "LBL_PREPARATIONS", pLanguageID))
            myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_Reference, MLRD.GetResourceText(Nothing, "LBL_Reference", pLanguageID))


            ' XB 05/09/2014 - BA-1902
            'myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_Sodium, MLRD.GetResourceText(Nothing, "LBL_Sodium", pLanguageID))
            'myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_Potassium, MLRD.GetResourceText(Nothing, "LBL_Potassium", pLanguageID))
            'myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_Chlorine, MLRD.GetResourceText(Nothing, "LBL_Chlorine", pLanguageID))
            'myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_Lithium, MLRD.GetResourceText(Nothing, "LBL_Lithium", pLanguageID))
            Dim ISETestList As New ISETestsDelegate
            myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_Sodium, ISETestList.GetName(Nothing, ISE_Tests.Na)) ' ID sodium = 1
            myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_Potassium, ISETestList.GetName(Nothing, ISE_Tests.K)) ' ID potassium = 2
            myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_Chlorine, ISETestList.GetName(Nothing, ISE_Tests.Cl)) ' ID chlorine = 3
            myLabelsData.Add(BSISEMonitorPanel.LabelElements.EL_Lithium, ISETestList.GetName(Nothing, ISE_Tests.Li)) ' ID lithium = 4
            ' XB 05/09/2014 - BA-1902


            myLabelsData.Add(BSISEMonitorPanel.LabelElements.CAL_Title, MLRD.GetResourceText(Nothing, "LBL_ISE_LastCalibrations", pLanguageID))
            myLabelsData.Add(BSISEMonitorPanel.LabelElements.CAL_LastDate, MLRD.GetResourceText(Nothing, "LBL_Date", pLanguageID))
            myLabelsData.Add(BSISEMonitorPanel.LabelElements.CAL_Results, MLRD.GetResourceText(Nothing, "LBL_Result", pLanguageID))
            myLabelsData.Add(BSISEMonitorPanel.LabelElements.CAL_Electrodes, MLRD.GetResourceText(Nothing, "LBL_ISE_Electrodes", pLanguageID))
            myLabelsData.Add(BSISEMonitorPanel.LabelElements.CAL_Pumps, MLRD.GetResourceText(Nothing, "LBL_ISE_Pumps", pLanguageID))
            myLabelsData.Add(BSISEMonitorPanel.LabelElements.CAL_Clean, MLRD.GetResourceText(Nothing, "LBL_ISE_CLEANING", pLanguageID))

            myLabelsData.Add(BSISEMonitorPanel.LabelElements.TUB_Pump, MLRD.GetResourceText(Nothing, "LBL_ISE_PUMP_TUBE", pLanguageID))
            myLabelsData.Add(BSISEMonitorPanel.LabelElements.TUB_Fluid, MLRD.GetResourceText(Nothing, "LBL_ISE_TUBING_TUBE", pLanguageID))

            myWarningsData.Add(BSISEMonitorPanel.WarningElements.RP_NotInstalled, MLRD.GetResourceText(Nothing, "LBL_ISE_RP_NOT_INSTALLED", pLanguageID))
            myWarningsData.Add(BSISEMonitorPanel.WarningElements.RP_Expired, MLRD.GetResourceText(Nothing, "LBL_ISE_RP_EXPIRED", pLanguageID))
            myWarningsData.Add(BSISEMonitorPanel.WarningElements.RP_Depleted, MLRD.GetResourceText(Nothing, "LGD_RotDeplet", pLanguageID))
            myWarningsData.Add(BSISEMonitorPanel.WarningElements.RP_WrongBiosystemsCode, MLRD.GetResourceText(Nothing, "LBL_ISE_WRONG_BIO_CODE", pLanguageID))
            myWarningsData.Add(BSISEMonitorPanel.WarningElements.EL_NotInstalled, MLRD.GetResourceText(Nothing, "LBL_ISE_EL_NOT_INSTALLED", pLanguageID))
            myWarningsData.Add(BSISEMonitorPanel.WarningElements.EL_ReplaceRecommended, MLRD.GetResourceText(Nothing, "LBL_ISE_REPLACE_RECOMMENDED", pLanguageID))
            myWarningsData.Add(BSISEMonitorPanel.WarningElements.CAL_Recommended, MLRD.GetResourceText(Nothing, "LBL_ISE_CALIB_RECOMMENDED", pLanguageID))
            myWarningsData.Add(BSISEMonitorPanel.WarningElements.CAL_Error, MLRD.GetResourceText(Nothing, "LBL_ISE_CALIB_ERROR", pLanguageID))
            myWarningsData.Add(BSISEMonitorPanel.WarningElements.CAL_CleanRecommended, MLRD.GetResourceText(Nothing, "LBL_ISE_CLEAN_RECOMMENDED", pLanguageID))
            myWarningsData.Add(BSISEMonitorPanel.WarningElements.RP_CleanPackInstalled, MLRD.GetResourceText(Nothing, "LBL_ISE_CLEAN_PACK_INSTALLED", pLanguageID))
            myWarningsData.Add(BSISEMonitorPanel.WarningElements.TUB_NotInstalled, MLRD.GetResourceText(Nothing, "LBL_NOT_INSTALLED", pLanguageID))
            'End If

            ' XBC 26/07/2012 - Add Bubble Calibration
            myLabelsData.Add(BSISEMonitorPanel.LabelElements.CAL_Bubble, MLRD.GetResourceText(Nothing, "LBL_ISE_Bubble", pLanguageID))

            Me.BsIseMonitor.LabelsData = myLabelsData
            Me.BsIseMonitor.WarnigsData = myWarningsData


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

            'Finally
            '    If Not dbConnection Is Nothing Then dbConnection.Close()
        End Try
    End Sub


#End Region

#Region "Event handlers"

#End Region

End Class