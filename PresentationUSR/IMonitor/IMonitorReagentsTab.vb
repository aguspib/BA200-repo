Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL

'Put here your business code for the tab ReagentsTab inside Monitor Form
Partial Public Class IMonitor

#Region "Methods"
    ''' <summary>
    ''' Get texts in the current application language for all screen controls in REAGENTS Tab
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 28/04/2011
    ''' Modified by: EF 29/08/2013 - BT #1272 => Label for Barcode TextBox changed to "Barcode" 
    '''              SA 19/11/2013 - BT #1359 => In the Label used to show the multilanguage description for Selected Position it is loaded now the 
    '''                                          multilanguage description for In Process Position (Second Reagents and Washing Solution needed for second 
    '''                                          Reagent contaminations in Reactions Rotor)
    ''' </remarks>
    Private Sub GetReagentTabLabels()
        Try
            'Labels for Position Information Area 
            bsReagentsPositionInfoLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_RotorPos_InfoPos", LanguageID)
            bsReagentsDiskNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_RingNum", LanguageID) 'TR 22/03/2012
            bsReagentsCellLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorCell", LanguageID)
            bsReagentsContentLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorCellContent", LanguageID)
            bsReagentsNumberLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Number_Short", LanguageID)
            bsReagentNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_ReagentName", LanguageID)
            bsTestNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestName", LanguageID)
            'EF 29/08/2013 - Bugtracking 1272 - Change label text by 'Barcode' (smaller=visible) in v2.1.1
            'bsReagentsBarCodeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_ReagentBarcode", LanguageID)
            bsReagentsBarCodeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_BARCODE", LanguageID)
            'EF 29/08/2013
            bsExpirationDateLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ExpDate_Short", LanguageID)
            bsBottleSizeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_BottleSize", LanguageID)
            bsCurrentVolLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_CurrentVol", LanguageID)
            bsTestsLeftLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_TestLeft", LanguageID)
            bsReagentsStatusLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MainMDI_Status", LanguageID)

            'Legend Area
            bsReagentsLegendLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Legend", LanguageID)
            LegReagentLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Reagents", LanguageID)
            LegReagAdditionalSol.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LGD_RotBWash", LanguageID)
            LegReagDepleteLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LGD_ROTEmptyLocked", LanguageID)
            LegReagLowVolLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LGD_RotBLower", LanguageID)
            LegReagNoInUseLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LGD_RotNoInUse", LanguageID)
            BarcodeErrorRGLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LGD_RotBarErr", LanguageID)
            UnknownLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_UNKNOWN", LanguageID)
            'LegReagentSelLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LGD_RotBSelect", pLanguageID)
            LegReagentSelLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LGD_RotBInProcess", LanguageID)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetReagentTabLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetReagentTabLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    Private Sub InitializeReagentTab()
        PrepareReagentLegendArea()
        GetReagentTabLabels()

        myRotorTypeForm = "REAGENTS"
        For I As Integer = 45 To 88 '88 To 45 Step -1
            MarkSelectedPosition(2, I, True)
            MarkSelectedPosition(2, I, False)
        Next
    End Sub

    ''' <summary>
    ''' Load the icons on the Legend Area.
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 28/04/2011
    ''' Modified by: SA 19/11/2013 - BT #1359 => In the PictureBox used to show the icon for Selected Position it is loaded now the 
    '''                                          new icon for In Process Position (Second Reagents and Washing Solution needed for second Reagent
    '''                                          contaminations in Reactions Rotor)
    ''' </remarks>
    Private Sub PrepareReagentLegendArea()
        Try
            'Icons
            ReagentPictureBox.ImageLocation = MyBase.IconsPath & LEGREAGENTREAGENT_IconName
            AdditionalSolPictureBox.ImageLocation = MyBase.IconsPath & LEGREAGENTADDSOL_IconName
            bsDepletedPictureBox.ImageLocation = MyBase.IconsPath & LEGREAGENTDEPLETED_IconName
            LowVolPictureBox.ImageLocation = MyBase.IconsPath & LEGREAGENTFEWVOL_IconName
            NoInUsePictureBox.ImageLocation = MyBase.IconsPath & LEGREAGENTNOTINUSE_IconName
            LegendBarCodeErrorRGImage.ImageLocation = MyBase.IconsPath & LEGREAGENTBCERRORRG_IconName
            LegendUnknownImage.ImageLocation = MyBase.IconsPath & LEGBCREAGENTUNKNOWN_IconName
            SelectedPictureBox.ImageLocation = MyBase.IconsPath & LEGREAGENTINPROCESS_IconName 'LEGREAGENTSELECTED_IconName
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareReagentLegendArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareReagentLegendArea", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub
#End Region

#Region "Events"
    Private Sub GenericReagent_MouseDown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Reag19.MouseDown, Reag18.MouseDown, Reag17.MouseDown, _
                                                                                                             Reag16.MouseDown, Reag15.MouseDown, Reag119.MouseDown, _
                                                                                                             Reag118.MouseDown, Reag117.MouseDown, Reag116.MouseDown, _
                                                                                                             Reag115.MouseDown, Reag114.MouseDown, Reag113.MouseDown, _
                                                                                                             Reag112.MouseDown, Reag111.MouseDown, Reag110.MouseDown, _
                                                                                                             Reag130.MouseDown, Reag129.MouseDown, Reag128.MouseDown, _
                                                                                                             Reag127.MouseDown, Reag126.MouseDown, Reag125.MouseDown, _
                                                                                                             Reag124.MouseDown, Reag123.MouseDown, Reag122.MouseDown, _
                                                                                                             Reag121.MouseDown, Reag120.MouseDown, Reag140.MouseDown, _
                                                                                                             Reag139.MouseDown, Reag138.MouseDown, Reag137.MouseDown, _
                                                                                                             Reag136.MouseDown, Reag135.MouseDown, Reag134.MouseDown, _
                                                                                                             Reag133.MouseDown, Reag132.MouseDown, Reag131.MouseDown, _
                                                                                                             Reag144.MouseDown, Reag143.MouseDown, Reag142.MouseDown, _
                                                                                                             Reag141.MouseDown, Reag255.MouseDown, Reag254.MouseDown, _
                                                                                                             Reag253.MouseDown, Reag252.MouseDown, Reag251.MouseDown, _
                                                                                                             Reag250.MouseDown, Reag249.MouseDown, Reag248.MouseDown, _
                                                                                                             Reag247.MouseDown, Reag246.MouseDown, Reag245.MouseDown, _
                                                                                                             Reag270.MouseDown, Reag269.MouseDown, Reag268.MouseDown, _
                                                                                                             Reag267.MouseDown, Reag266.MouseDown, Reag265.MouseDown, _
                                                                                                             Reag264.MouseDown, Reag263.MouseDown, Reag262.MouseDown, _
                                                                                                             Reag261.MouseDown, Reag260.MouseDown, Reag259.MouseDown, _
                                                                                                             Reag258.MouseDown, Reag257.MouseDown, Reag256.MouseDown, _
                                                                                                             Reag280.MouseDown, Reag279.MouseDown, Reag278.MouseDown, _
                                                                                                             Reag277.MouseDown, Reag276.MouseDown, Reag275.MouseDown, _
                                                                                                             Reag274.MouseDown, Reag273.MouseDown, Reag272.MouseDown, _
                                                                                                             Reag271.MouseDown, Reag283.MouseDown, Reag282.MouseDown, _
                                                                                                             Reag281.MouseDown, Reag288.MouseDown, Reag287.MouseDown, _
                                                                                                             Reag286.MouseDown, Reag285.MouseDown, Reag284.MouseDown, _
                                                                                                             Reag14.MouseDown, Reag13.MouseDown, Reag12.MouseDown, Reag11.MouseDown


        Generic_MouseDown(sender, e)
    End Sub
#End Region
End Class