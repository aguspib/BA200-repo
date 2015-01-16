Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types

'Put here your business code for the tab ReactionsTab inside Monitor Form
Partial Public Class IMonitor

    Private Sub InitializeReactionsTab()
        'Put your initialization code here. It will be executed in the Monitor OnLoad event
        PrepareReactionsLegendArea()
        GetReactionsTabLabels()
    End Sub


#Region "Methods"

    ''' <summary>
    ''' Load the icons on the Legend Area.
    ''' </summary>
    ''' <remarks>
    ''' CREATED BY: DL 11/05/2011
    ''' </remarks>
    Private Sub PrepareReactionsLegendArea()
        Try
            ''Icons
            bsWashingPictureBox.ImageLocation = MyBase.IconsPath & LEGREACTIONWASHING_IconName
            bsNotInUsePictureBox.ImageLocation = MyBase.IconsPath & LEGREACTIONNOTINUSE_IconName
            bsR1PictureBox.ImageLocation = MyBase.IconsPath & LEGREACTIONR1_IconName
            bsR1SamplePictureBox.ImageLocation = MyBase.IconsPath & LEGREACTIONR1SAMPLE_IconName
            bsR1SampleR2PictureBox.ImageLocation = MyBase.IconsPath & LEGREACTIONR1SAMPLER2_IconName
            bsDilutionPictureBox.ImageLocation = MyBase.IconsPath & LEGREACTIONDILUTION_IconName
            bsFinishPictureBox.ImageLocation = MyBase.IconsPath & LEGREACTIONFINISH_IconName
            bsContaminatedPictureBox.ImageLocation = MyBase.IconsPath & LEGREACTIONCONTAM_IconName
            bsOpticalPictureBox.ImageLocation = MyBase.IconsPath & LEGREACTIONOPTICAL_IconName

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareReagentLegendArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareReagentLegendArea", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    '''  Get all label text for Sample Tab
    ''' </summary>
    ''' <remarks>CREATE BY: DL 11/05/2011</remarks>
    Private Sub GetReactionsTabLabels()
        Try
            bsWellNumberLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Well_Number", LanguageID)
            bsSampleClassLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSPrep_SampleClass", LanguageID)

            'AG 12/12/2011
            'bsCalibNumLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveReplicate_CalibNo", LanguageID)
            bsCalibNumLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Number_Short", LanguageID)
            'EF 29/08/2013 - Bugtracking 1272 - Change label text by 'Sample' in v2.1.1
            'bsReacSampleIDLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_SampleID", LanguageID)
            bsReacSampleIDLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_SampleVolume", LanguageID)
            'EF 29/08/2013
            bsReacSampleTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", LanguageID)
            bsReacDilutionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Dilution", LanguageID)
            bsReplicateLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveReplicate_Rep", LanguageID)
            bsRerunLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Rerun", LanguageID)
            bsReacTestNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test_Singular", LanguageID)
            bsReacStatusLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MainMDI_Status", LanguageID)

            'Legend Area
            bsWashingLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "GRID_SRV_WASH", LanguageID)
            bsNotInUseLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LGD_RotNoInUse", LanguageID)
            bsR1Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_R1", LanguageID)
            bsR1SampleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_R1_SAMPLE", LanguageID)
            bsR1SampleR2.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_R1_SAMPLE_R2", LanguageID)
            bsDilutionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Sample_Dilution", LanguageID)
            bsFinishLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_POS_STATUS_FINISHED", LanguageID)
            bsContaminatedLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Contaminated", LanguageID)
            bsOpticalLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Optical_Rejection", LanguageID)

            'RH 13/10/2011
            bsReactionsLegendLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Legend", LanguageID)

            'RH 02/05/2012
            bsReactionsPositionInfoLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_RotorPos_InfoPos", LanguageID)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetReagentTabLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetReagentTabLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub


    ''' <summary>
    ''' Refresh the Reactions Rotor.
    ''' </summary>
    ''' <param name="pRefreshDS"></param>
    ''' <remarks>
    ''' CREATED BY: AG 08/06/2011 (based on RefreshRotors)
    ''' Modified AG 05/06/2012 - Avoid innecessary loops to execute faster
    ''' </remarks>
    Private Sub RefreshReactionsRotor(ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        Try
            CreateLogActivity("RefreshReactionsRotor (Init): " + IsDisposed.ToString(), Name & ".RefreshReactionsRotor ", EventLogEntryType.Information, GetApplicationInfoSession().ActivateSystemLog)
            'CreateLogActivity("IAx00MainMDI.ActiveMdiChild.Name : " + IAx00MainMDI.ActiveMdiChild.Name, Name & ".RefreshReactionsRotor ", EventLogEntryType.Information, GetApplicationInfoSession().ActivateSystemLog)

            If (IsDisposed) Then Exit Sub 'IT 03/06/2014 - #1644 No refresh if screen is disposed

            'Dim myLogAcciones As New ApplicationLogManager()
            Dim StartTime As DateTime = Now 'AG 04/07/2012 - time estimation

            'Call the Method incharge to update Rotor (Sample/Reagent)
            Dim RotorContentList As New List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
            Dim tmpUpdatePosition As New WSRotorContentByPositionDS
            Dim changes As Boolean = False

            'AG 05/06/2012 - NEW CODE
            tmpUpdatePosition.twksWSRotorContentByPosition.Clear()
            If Not pRefreshDS.ReactionWellStatusChanged Is Nothing AndAlso pRefreshDS.ReactionWellStatusChanged.Rows.Count > 0 Then
                For Each RefreshRow As UIRefreshDS.ReactionWellStatusChangedRow In pRefreshDS.ReactionWellStatusChanged.Rows
                    If Not RefreshRow.IsAnalyzerIDNull AndAlso Not RefreshRow.IsWellNumberNull Then
                        RotorContentList = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                                            Where a.AnalyzerID = RefreshRow.AnalyzerID _
                                            AndAlso a.RotorType = "REACTIONS" _
                                            AndAlso a.CellNumber = RefreshRow.WellNumber _
                                            Select a).ToList
                        If RotorContentList.Count > 0 Then
                            RotorContentList.First().BeginEdit()
                            If Not RefreshRow.IsRotorTurnNull Then
                                If RotorContentList.First().RingNumber <> RefreshRow.RotorTurn Then
                                    RotorContentList.First().RingNumber = RefreshRow.RotorTurn
                                End If
                            End If
                            If Not RefreshRow.IsWellContentNull Then
                                If RotorContentList.First().TubeContent <> RefreshRow.WellContent Then
                                    RotorContentList.First().TubeContent = RefreshRow.WellContent
                                    changes = True
                                End If
                            End If
                            If Not RefreshRow.IsWellStatusNull Then
                                If RotorContentList.First().ElementStatus <> RefreshRow.WellStatus Then
                                    RotorContentList.First().ElementStatus = RefreshRow.WellStatus
                                    changes = True
                                End If
                            End If
                            RotorContentList.First().EndEdit()
                        End If

                        'When changes ... update the rotor position information on the screen 
                        If changes Then
                            tmpUpdatePosition.twksWSRotorContentByPosition.ImportRow(RotorContentList.First())
                            tmpUpdatePosition.twksWSRotorContentByPosition.AcceptChanges()
                            changes = False
                        End If

                    End If
                Next RefreshRow

                If tmpUpdatePosition.twksWSRotorContentByPosition.Rows.Count > 0 Then
                    UpdateRotorTreeViewArea(tmpUpdatePosition, RotorContentList.First().RotorType)
                End If
            End If
            'TR 25/09/2013 #memory
            RotorContentList = Nothing
            'TR 25/09/2013 END

            GlobalBase.CreateLogActivity("Refresh reactions rotor: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "iMonitor.RefreshReactionsRotor", EventLogEntryType.Information, False) 'AG 04/07/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".RefreshReactionsRotor ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RefreshReactionsRotor", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

#End Region

#Region "Events"

    ''' <summary>
    ''' Click over reaction well
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by DL 01/06/2011
    ''' </remarks>
    Private Sub Reac1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Reac1.Click, Reac2.Click, Reac3.Click, Reac4.Click, Reac5.Click, _
                                                                    Reac6.Click, Reac7.Click, Reac8.Click, Reac9.Click, Reac10.Click, Reac11.Click, Reac12.Click, _
                                                                    Reac13.Click, Reac14.Click, Reac15.Click, Reac16.Click, Reac17.Click, Reac18.Click, Reac19.Click, _
                                                                    Reac20.Click, Reac21.Click, Reac22.Click, Reac23.Click, Reac24.Click, Reac25.Click, Reac26.Click, _
                                                                    Reac27.Click, Reac28.Click, Reac29.Click, Reac30.Click, Reac31.Click, Reac32.Click, Reac33.Click, _
                                                                    Reac34.Click, Reac35.Click, Reac36.Click, Reac37.Click, Reac38.Click, Reac39.Click, Reac40.Click, _
                                                                    Reac41.Click, Reac42.Click, Reac43.Click, Reac44.Click, Reac45.Click, Reac46.Click, Reac47.Click, _
                                                                    Reac48.Click, Reac49.Click, Reac50.Click, Reac51.Click, Reac52.Click, Reac53.Click, Reac54.Click, _
                                                                    Reac55.Click, Reac56.Click, Reac57.Click, Reac58.Click, Reac59.Click, Reac60.Click, Reac61.Click, _
                                                                    Reac62.Click, Reac63.Click, Reac64.Click, Reac65.Click, Reac66.Click, Reac67.Click, Reac68.Click, _
                                                                    Reac69.Click, Reac70.Click, Reac71.Click, Reac72.Click, Reac73.Click, Reac74.Click, Reac75.Click, _
                                                                    Reac76.Click, Reac77.Click, Reac78.Click, Reac79.Click, Reac80.Click, Reac81.Click, Reac82.Click, _
                                                                    Reac83.Click, Reac84.Click, Reac85.Click, Reac86.Click, Reac87.Click, Reac88.Click, Reac89.Click, _
                                                                    Reac90.Click, Reac91.Click, Reac92.Click, Reac93.Click, Reac94.Click, Reac95.Click, Reac96.Click, _
                                                                    Reac97.Click, Reac98.Click, Reac99.Click, Reac100.Click, Reac101.Click, Reac102.Click, Reac103.Click, _
                                                                    Reac104.Click, Reac105.Click, Reac106.Click, Reac107.Click, Reac108.Click, Reac109.Click, Reac110.Click, _
                                                                    Reac111.Click, Reac112.Click, Reac113.Click, Reac114.Click, Reac115.Click, Reac116.Click, Reac117.Click, _
                                                                    Reac118.Click, Reac119.Click, Reac120.Click

        If TypeOf sender Is BSRImage Then
            Dim myPictureBox As BSRImage = CType(sender, BSRImage)

            'Validate that the tag property is not empty to get the information.
            If Not myPictureBox.Tag Is Nothing Then
                'get the selected ring and cell number.

                Dim myWellNumber As Integer = CType(myPictureBox.Tag.ToString, Integer)
                'Dim myCellNumber As Integer = CType(myPictureBox.Tag.ToString().Split(",")(1), Integer)
                mySelectedElementInfo = GetLocalPositionInfo(0, myWellNumber, False, True)

                ShowPositionInfoReactionsArea(ActiveAnalyzer, mySelectedElementInfo.twksWSRotorContentByPosition.First.RingNumber, myWellNumber)
                'MarkSelectedPosition(myRingNumber, myCellNumber, True)
            End If
        End If
    End Sub


#End Region

End Class