Option Strict On
Option Infer On
Option Explicit On
Imports Biosystems.Ax00.Global

'Put here your business code for the tab SamplesTab inside Monitor Form
Partial Public Class IMonitor

#Region "Declarations"

#End Region

#Region "Methods"
    Private Sub InitializeSamplesTab()
        'Put your initialization code here. It will be executed in the Monitor OnLoad event
        PrepareSampleLegendArea()
        GetSampleTabLabels()
        GetScreenToolTip()
    End Sub

    ''' <summary>
    ''' Load the icons on the Legend Area.
    ''' </summary>
    ''' <remarks>
    ''' CREATED BY: TR 27/04/2011
    ''' MODIFIED BY: DL 23/09/2011
    ''' </remarks>
    Private Sub PrepareSampleLegendArea()
        Try
            'Icons
            LegendSelectedImage.ImageLocation = MyBase.IconsPath & LEGSAMPLESELECT_Iconname
            LegendNotInUseImage.ImageLocation = MyBase.IconsPath & LEGSAMPLENOTINUSE_IconName
            LegendDepletedImage.ImageLocation = MyBase.IconsPath & LEGSAMPLEDEPLETED_IconName
            LegendPendingImage.ImageLocation = MyBase.IconsPath & LEGSAMPLEPENDING_IconName
            LegendInProgressImage.ImageLocation = MyBase.IconsPath & LEGSAMPLEINPROGR_IconName
            LegendFinishedImage.ImageLocation = MyBase.IconsPath & LEGSAMPLEFINISHD_IconName
            LegendBarCodeErrorImage.ImageLocation = MyBase.IconsPath & LEGSAMPLEBCERROR_IconName


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareSampleLegendArea ", EventLogEntryType.Error, _
                                                                          GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareSampleLegendArea", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' Get all label text for Sample Tab
    ''' </summary>
    ''' <remarks>
    ''' CREATE BY: TR 28/04/2011
    ''' </remarks>
    Private Sub GetSampleTabLabels()
        Try
            'Info Area
            bsSamplesPositionInfoLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_RotorPos_InfoPos", LanguageID)
            bsSamplesDiskNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_RingNum", LanguageID) 'TR 22/03/2012
            bsSamplesCellLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorCell", LanguageID)
            bsSamplesContentLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorCellContent", LanguageID)
            bsSamplesNumberLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Number_Short", LanguageID)
            'EF 29/08/2013 - Change label text by 'Sample' (more clear) and 'Barcode' (smaller size) in v2.1.1
            'bsSampleIDLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_SampleID", LanguageID)
            bsSampleIDLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_SampleVolume", LanguageID)
            'bsSamplesBarcodeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_Barcode", LanguageID)
            bsSamplesBarcodeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_BARCODE", LanguageID)
            'EF 29/08/2013
            bsSampleTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", LanguageID)
            bsDiluteStatusLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_Diluted", LanguageID)
            bsTubeSizeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_TubeSize", LanguageID)
            bsSamplesStatusLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MainMDI_Status", LanguageID)

            'Legend Area
            bsSamplesLegendLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Legend", LanguageID)
            SelectedLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSTests_Selected", LanguageID)
            NotInUseLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LGD_RotNoInUse", LanguageID)
            DepletedLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LGD_RotBDeplet", LanguageID)
            SampleStatusLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Sample_Status", LanguageID)
            PendingLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_POS_PENDING_OR_LOCKED", LanguageID)
            InProgressLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_POS_STATUS_INPROGRESS", LanguageID)
            FinishedLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_POS_STATUS_FINISHED", LanguageID)
            BarcodeErrorLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LGD_RotBarErr", LanguageID)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetSampleTabLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetSampleTabLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub
#End Region

    'Private Sub Sam11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Sam11.Click, Sam12.Click, _
    '                                                              Sam19.Click, Sam18.Click, Sam17.Click, _
    '                                                              Sam16.Click, Sam15.Click, Sam145.Click, Sam144.Click, _
    '                                                              Sam143.Click, Sam142.Click, Sam141.Click, Sam140.Click, _
    '                                                              Sam14.Click, Sam139.Click, Sam138.Click, Sam137.Click, _
    '                                                              Sam136.Click, Sam135.Click, Sam134.Click, Sam133.Click, _
    '                                                              Sam132.Click, Sam131.Click, Sam130.Click, Sam13.Click, _
    '                                                              Sam129.Click, Sam128.Click, Sam127.Click, Sam126.Click, _
    '                                                              Sam125.Click, Sam124.Click, Sam123.Click, Sam122.Click, _
    '                                                              Sam121.Click, Sam120.Click, Sam12.Click, Sam119.Click, _
    '                                                              Sam118.Click, Sam117.Click, Sam116.Click, Sam115.Click, _
    '                                                              Sam114.Click, Sam113.Click, Sam112.Click, Sam111.Click, _
    '                                                              Sam110.Click, Sam11.Click, Sam399.Click, Sam398.Click, _
    '                                                              Sam397.Click, Sam396.Click, Sam395.Click, Sam394.Click, _
    '                                                              Sam393.Click, Sam392.Click, Sam391.Click, Sam3135.Click, _
    '                                                              Sam3134.Click, Sam3133.Click, Sam3132.Click, Sam3131.Click, _
    '                                                              Sam3130.Click, Sam3129.Click, Sam3128.Click, Sam3127.Click, _
    '                                                              Sam3126.Click, Sam3125.Click, Sam3124.Click, Sam3123.Click, _
    '                                                              Sam3122.Click, Sam3121.Click, Sam3120.Click, Sam3119.Click, _
    '                                                              Sam3118.Click, Sam3117.Click, Sam3116.Click, Sam3115.Click, _
    '                                                              Sam3114.Click, Sam3113.Click, Sam3112.Click, Sam3111.Click, _
    '                                                              Sam3110.Click, Sam3109.Click, Sam3108.Click, Sam3107.Click, _
    '                                                              Sam3106.Click, Sam3105.Click, Sam3104.Click, Sam3103.Click, _
    '                                                              Sam3102.Click, Sam3101.Click, Sam3100.Click, _
    '                                                              Sam260.Click, Sam259.Click, Sam258.Click, Sam257.Click, _
    '                                                              Sam256.Click, Sam255.Click, Sam254.Click, Sam253.Click, _
    '                                                              Sam252.Click, Sam251.Click, Sam250.Click, Sam249.Click, _
    '                                                              Sam248.Click, Sam247.Click, Sam246.Click, Sam280.Click, _
    '                                                              Sam279.Click, Sam278.Click, Sam277.Click, Sam276.Click, _
    '                                                              Sam275.Click, Sam274.Click, Sam273.Click, Sam272.Click, _
    '                                                              Sam271.Click, Sam270.Click, Sam269.Click, Sam268.Click, _
    '                                                              Sam267.Click, Sam266.Click, Sam265.Click, Sam264.Click, _
    '                                                              Sam263.Click, Sam262.Click, Sam261.Click, Sam289.Click, _
    '                                                              Sam288.Click, Sam287.Click, Sam286.Click, Sam285.Click, _
    '                                                              Sam284.Click, Sam283.Click, Sam282.Click, Sam281.Click, _
    '                                                              Sam290.Click

    '    If TypeOf sender Is BSRImage Then
    '        Dim myPictureBox As BSRImage = CType(sender, BSRImage)

    '        'Validate that the tag property is not empty to get the information.
    '        If Not myPictureBox.Tag Is Nothing Then
    '            'get the selected ring and cell number.
    '            Dim myRingNumber As Integer = CType(myPictureBox.Tag.ToString().Split(",")(0), Integer)
    '            Dim myCellNumber As Integer = CType(myPictureBox.Tag.ToString().Split(",")(1), Integer)
    '            mySelectedElementInfo = GetLocalPositionInfo(myRingNumber, myCellNumber, False)

    '            ShowPositionInfoArea(myRotorTypeForm, myRingNumber, myCellNumber)
    '            'MarkSelectedPosition(myRingNumber, myCellNumber, True)

    '        End If
    '    End If
    'End Sub

    ''' <summary>
    ''' </summary>
    ''' <remarks>
    ''' CREATE BY: DL 29/09/2011
    ''' </remarks>
    Private Sub GenericSample_MouseDown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Sam11.MouseDown, Sam12.MouseDown, _
                                                                                                            Sam19.MouseDown, Sam18.MouseDown, Sam17.MouseDown, _
                                                                                                            Sam16.MouseDown, Sam15.MouseDown, Sam145.MouseDown, Sam144.MouseDown, _
                                                                                                            Sam143.MouseDown, Sam142.MouseDown, Sam141.MouseDown, Sam140.MouseDown, _
                                                                                                            Sam14.MouseDown, Sam139.MouseDown, Sam138.MouseDown, Sam137.MouseDown, _
                                                                                                            Sam136.MouseDown, Sam135.MouseDown, Sam134.MouseDown, Sam133.MouseDown, _
                                                                                                            Sam132.MouseDown, Sam131.MouseDown, Sam130.MouseDown, Sam13.MouseDown, _
                                                                                                            Sam129.MouseDown, Sam128.MouseDown, Sam127.MouseDown, Sam126.MouseDown, _
                                                                                                            Sam125.MouseDown, Sam124.MouseDown, Sam123.MouseDown, Sam122.MouseDown, _
                                                                                                            Sam121.MouseDown, Sam120.MouseDown, Sam12.MouseDown, Sam119.MouseDown, _
                                                                                                            Sam118.MouseDown, Sam117.MouseDown, Sam116.MouseDown, Sam115.MouseDown, _
                                                                                                            Sam114.MouseDown, Sam113.MouseDown, Sam112.MouseDown, Sam111.MouseDown, _
                                                                                                             Sam110.MouseDown, Sam11.MouseDown, Sam399.MouseDown, Sam398.MouseDown, _
                                                                                                             Sam397.MouseDown, Sam396.MouseDown, Sam395.MouseDown, Sam394.MouseDown, _
                                                                                                             Sam393.MouseDown, Sam392.MouseDown, Sam391.MouseDown, Sam3135.MouseDown, _
                                                                                                             Sam3134.MouseDown, Sam3133.MouseDown, Sam3132.MouseDown, Sam3131.MouseDown, _
                                                                                                             Sam3130.MouseDown, Sam3129.MouseDown, Sam3128.MouseDown, Sam3127.MouseDown, _
                                                                                                             Sam3126.MouseDown, Sam3125.MouseDown, Sam3124.MouseDown, Sam3123.MouseDown, _
                                                                                                             Sam3122.MouseDown, Sam3121.MouseDown, Sam3120.MouseDown, Sam3119.MouseDown, _
                                                                                                             Sam3118.MouseDown, Sam3117.MouseDown, Sam3116.MouseDown, Sam3115.MouseDown, _
                                                                                                             Sam3114.MouseDown, Sam3113.MouseDown, Sam3112.MouseDown, Sam3111.MouseDown, _
                                                                                                             Sam3110.MouseDown, Sam3109.MouseDown, Sam3108.MouseDown, Sam3107.MouseDown, _
                                                                                                             Sam3106.MouseDown, Sam3105.MouseDown, Sam3104.MouseDown, Sam3103.MouseDown, _
                                                                                                             Sam3102.MouseDown, Sam3101.MouseDown, Sam3100.MouseDown, _
                                                                                                             Sam260.MouseDown, Sam259.MouseDown, Sam258.MouseDown, Sam257.MouseDown, _
                                                                                                             Sam256.MouseDown, Sam255.MouseDown, Sam254.MouseDown, Sam253.MouseDown, _
                                                                                                             Sam252.MouseDown, Sam251.MouseDown, Sam250.MouseDown, Sam249.MouseDown, _
                                                                                                             Sam248.MouseDown, Sam247.MouseDown, Sam246.MouseDown, Sam280.MouseDown, _
                                                                                                             Sam279.MouseDown, Sam278.MouseDown, Sam277.MouseDown, Sam276.MouseDown, _
                                                                                                             Sam275.MouseDown, Sam274.MouseDown, Sam273.MouseDown, Sam272.MouseDown, _
                                                                                                             Sam271.MouseDown, Sam270.MouseDown, Sam269.MouseDown, Sam268.MouseDown, _
                                                                                                             Sam267.MouseDown, Sam266.MouseDown, Sam265.MouseDown, Sam264.MouseDown, _
                                                                                                             Sam263.MouseDown, Sam262.MouseDown, Sam261.MouseDown, Sam289.MouseDown, _
                                                                                                             Sam288.MouseDown, Sam287.MouseDown, Sam286.MouseDown, Sam285.MouseDown, _
                                                                                                             Sam284.MouseDown, Sam283.MouseDown, Sam282.MouseDown, Sam281.MouseDown, _
                                                                                                             Sam290.MouseDown

        Generic_MouseDown(sender, e)

    End Sub



End Class