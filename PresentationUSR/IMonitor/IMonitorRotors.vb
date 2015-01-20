Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports System.Globalization

'Put here your common business code for the Rotors tabs inside Monitor Form
Partial Public Class IMonitor

#Region "Declarations"
    Dim myRotorTypeForm As String = ""
    Private setControlPosToNothing As Boolean
    Private PreviousSelect As String

    'Screen structure containing information of all positions in all the available Rotors
    Dim myRotorContentByPositionDSForm As WSRotorContentByPositionDS
    Dim mySelectedElementInfo As WSRotorContentByPositionDS

    Dim AllTubeSizeList As List(Of TubeSizeTO)
    Dim myPosControlList As List(Of BSRImage)

    '*****************************************************************************
    '* GLOBAL VARIABLES FOR NAMES OF ICONS USED IN SAMPLES ROTOR AND LEGEND AREA *
    '*****************************************************************************
    '** Variables for Cell Status (used as Background Image) - CELL IS NOT SELECTED
    Private NOTINUSE_IconName As String = String.Empty
    Private DEPLETED_IconName As String = String.Empty
    Private PENDING_IconName As String = String.Empty
    Private INPROGRESS_IconName As String = String.Empty
    Private FINISHED_IconName As String = String.Empty
    Private BTLSAMPLEBCERR_IconName As String = String.Empty

    '** Variables for Cell Status (used as Background Image) - CELL IS SELECTED
    Private EMPTYCELL_IconName As String = String.Empty
    Private BTLSAMPLENOINUSE_SEL_IconName As String = String.Empty
    Private BTLSAMPLEDEPLETED_SEL_IconName As String = String.Empty
    Private BTLSAMPLEPENDING_SEL_IconName As String = String.Empty
    Private BTLSAMPLEINPROCES_SEL_IconName As String = String.Empty
    Private BTLSAMPLEFINISHED_SEL_IconName As String = String.Empty
    Private BTLSAMPLEBCERR_SEL_IconName As String = String.Empty

    '** Variables for Sample Class Icons(used as Image in the Rotor)
    Private CALIB_IconName As String = String.Empty
    Private CTRL_IconName As String = String.Empty
    Private STATS_IconName As String = String.Empty
    Private ROUTINES_IconName As String = String.Empty
    Private DILUTIONS_IconName As String = String.Empty
    Private ADDSAMPLESOL_IconName As String = String.Empty

    '** Variables for Samples Legend Area
    Private LEGSAMPLESELECT_Iconname As String = String.Empty
    Private LEGSAMPLENOTINUSE_IconName As String = String.Empty
    Private LEGSAMPLEDEPLETED_IconName As String = String.Empty
    Private LEGSAMPLEPENDING_IconName As String = String.Empty
    Private LEGSAMPLEINPROGR_IconName As String = String.Empty
    Private LEGSAMPLEFINISHD_IconName As String = String.Empty
    Private LEGSAMPLEBCERROR_IconName As String = String.Empty

    '*****************************************************************************
    '* GLOBAL VARIABLES FOR NAMES OF ICONS USED IN REAGENTS ROTOR AND LEGEND AREA *
    '*****************************************************************************
    '** Variables for Cell Status in Ring 1 (SMALL Bottles) - CELL IS NOT SELECTED           	
    Private BOTTLE2_NOTHING_IconName As String = String.Empty    'Transparent image to improve the screen refresh
    Private BTLNUSESMALLR1_IconName As String = String.Empty
    Private BTLDEPLETSMALLR1_IconName As String = String.Empty
    Private BTLFEWSMALLR1_IconName As String = String.Empty
    Private BTLBCUKNSMALLR1_IconName As String = String.Empty
    Private BTLBCERRSMALLR1_IconName As String = String.Empty
    Private BTLREAGSMALLD1_IconName As String = String.Empty
    Private BTLADDSOLSMALLR1_IconName As String = String.Empty
    '**** BT #1359 => New icons for In Process Reagents and Washing Solutions (InUse, Depleted and Low Volume)
    Private BTL_IPRGSML1_IconName As String = String.Empty
    Private BTL_IPASSML1_IconName As String = String.Empty
    Private BTL_IPDPSML1_IconName As String = String.Empty
    Private BTL_IPFWSML1_IconName As String = String.Empty

    '** Variables for Cell Status in Ring 1 (SMALL Bottles) - CELL IS SELECTED           	
    Private BOTTLE2_EMPTYCELL_IconName As String = String.Empty   'Free position in Ring 1 (external ring)
    Private BTLNUSESMALLR1_SEL_IconName As String = String.Empty
    Private BTLDEPLETSMALLR1_SEL_IconName As String = String.Empty
    Private BTLFEWSMALLR1_SEL_IconName As String = String.Empty
    Private BTLBCUKNSMALLR1_SEL_IconName As String = String.Empty
    Private BTLBCERRSMALLR1_SEL_IconName As String = String.Empty
    Private BTLREAGSMALLD1_SEL_IconName As String = String.Empty
    Private BTLADDSOLSMALLR1_SEL_IconName As String = String.Empty
    '**** BT #1359 => New icons for In Process Reagents and Washing Solutions (InUse, Depleted and Low Volume)
    Private BTL_IPRGSML1_SEL_IconName As String = String.Empty
    Private BTL_IPASSML1_SEL_IconName As String = String.Empty
    Private BTL_IPDPSML1_SEL_IconName As String = String.Empty
    Private BTL_IPFWSML1_SEL_IconName As String = String.Empty

    '** Variables for Cell Status in Ring 2 (SMALL Bottles) - CELL IS NOT SELECTED           		
    Private BTLNUSESMALLR2_IconName As String = String.Empty
    Private BTLDEPLETSMALLR2_IconName As String = String.Empty
    Private BTLFEWSMALLR2_IconName As String = String.Empty
    Private BTLBCUKNSMALLR2_IconName As String = String.Empty
    Private BTLBCERRSMALLR2_IconName As String = String.Empty
    Private BTLREAGSMALLD2_IconName As String = String.Empty
    Private BTLADDSOLSMALLR2_IconName As String = String.Empty
    '**** BT #1359 => New icons for In Process Reagents and Washing Solutions (InUse, Depleted and Low Volume)
    Private BTL_IPRGSML2_IconName As String = String.Empty
    Private BTL_IPASSML2_IconName As String = String.Empty
    Private BTL_IPDPSML2_IconName As String = String.Empty
    Private BTL_IPFWSML2_IconName As String = String.Empty

    '** Variables for Cell Status in Ring 2 (SMALL Bottles) - CELL IS SELECTED           		
    Private BTLNUSESMALLR2_SEL_IconName As String = String.Empty
    Private BTLDEPLETSMALLR2_SEL_IconName As String = String.Empty
    Private BTLFEWSMALLR2_SEL_IconName As String = String.Empty
    Private BTLBCUKNSMALLR2_SEL_IconName As String = String.Empty
    Private BTLBCERRSMALLR2_SEL_IconName As String = String.Empty
    Private BTLREAGSMALLD2_SEL_IconName As String = String.Empty
    Private BTLADDSOLSMALLR2_SEL_IconName As String = String.Empty
    '**** BT #1359 => New icons for In Process Reagents and Washing Solutions (InUse, Depleted and Low Volume)
    Private BTL_IPRGSML2_SEL_IconName As String = String.Empty
    Private BTL_IPASSML2_SEL_IconName As String = String.Empty
    Private BTL_IPDPSML2_SEL_IconName As String = String.Empty
    Private BTL_IPFWSML2_SEL_IconName As String = String.Empty

    '** Variables for Cell Status in Ring 2 (BIG Bottles) - CELL IS NOT SELECTED           		
    Private BOTTLE3_NOTHING_IconName As String = String.Empty       'Transparent image to improve the screen refresh
    Private BTLNUSEBIGR2_IconName As String = String.Empty
    Private BTLDEPLBIGR2_IconName As String = String.Empty
    Private BTLFEWBIGR2_IconName As String = String.Empty
    Private BTLBCUKNBIGR2_IconName As String = String.Empty
    Private BTLBCERRBIGR2_IconName As String = String.Empty
    Private BTLREAGBIG2_IconName As String = String.Empty
    Private BTLADDSOLBIGR2_IconName As String = String.Empty
    '**** BT #1359 => New icons for In Process Reagents and Washing Solutions (InUse, Depleted and Low Volume)
    Private BTL_IPRGBIG2_IconName As String = String.Empty
    Private BTL_IPASBIG2_IconName As String = String.Empty
    Private BTL_IPDPBIG2_IconName As String = String.Empty
    Private BTL_IPFWBIG2_IconName As String = String.Empty

    '** Variables for Cell Status in Ring 2 (SMALL Bottles) - CELL IS SELECTED           			
    Private BOTTLE3_EMPTYCELL_IconName As String = String.Empty     'Free position in Ring 2 (internal ring)
    Private BTLNUSEBIGR2_SEL_IconName As String = String.Empty
    Private BTLDEPLBIGR2_SEL_IconName As String = String.Empty
    Private BTLFEWBIGR2_SEL_IconName As String = String.Empty
    Private BTLBCUKNBIGR2_SEL_IconName As String = String.Empty
    Private BTLBCERRBIGR2_SEL_IconName As String = String.Empty
    Private BTLREAGBIG2_SEL_IconName As String = String.Empty
    Private BTLADSLBIGR2SEL_IconName As String = String.Empty
    '**** BT #1359 => New icons for In Process Reagents and Washing Solutions (InUse, Depleted and Low Volume)
    Private BTL_IPRGBIG2_SEL_IconName As String = String.Empty
    Private BTL_IPASBIG2_SEL_IconName As String = String.Empty
    Private BTL_IPDPBIG2_SEL_IconName As String = String.Empty
    Private BTL_IPFWBIG2_SEL_IconName As String = String.Empty

    '** Variables for Reagents Rotor Legend Area
    Private LEGREAGENTREAGENT_IconName As String = String.Empty
    Private LEGREAGENTADDSOL_IconName As String = String.Empty
    Private LEGREAGENTDEPLETED_IconName As String = String.Empty
    Private LEGREAGENTFEWVOL_IconName As String = String.Empty
    Private LEGREAGENTNOTINUSE_IconName As String = String.Empty
    Private LEGREAGENTBCERRORRG_IconName As String = String.Empty
    Private LEGBCREAGENTUNKNOWN_IconName As String = String.Empty
    '**** BT #1359 => New icon for In Process Reagents and Washing Solutions; Icon for Selected Position removed  
    'Private LEGREAGENTSELECTED_IconName As String = String.Empty
    Private LEGREAGENTINPROCESS_IconName As String = String.Empty

    '** Variables for the type of Element to shown in Reagents Rotor Cells
    Private REAGENTS_IconName As String = String.Empty
    Private ADDSOL_IconName As String = String.Empty

    '*******************************************************************************
    '* GLOBAL VARIABLES FOR NAMES OF ICONS USED IN REACTIONS ROTOR AND LEGEND AREA *
    '*******************************************************************************   
    '** Variables for Well Status (used as Background Image) - WELL IS NOT SELECTED
    Private REACPOSCONT_IconName As String = String.Empty      'Contaminated (RED).   Content: C. Status: -
    Private REACPOSFINI_IconName As String = String.Empty      'Finish (GREEN).       Content: T. Status: F
    Private REACPOSNIUS_IconName As String = String.Empty      'Not in Use	(WHITE).  Content: E. Status: R
    Private REACPOSOPTI_IconName As String = String.Empty      'Rejected (BLACK).     Content:	E. Status: X
    Private REACPOSR1_IconName As String = String.Empty        'R1 (LIGHT ORANGE).    Content: T. Status: R1
    Private REACPOSR1SL_IconName As String = String.Empty      'R1 + S (ORANGE).      Content: T. Status: S
    Private REACPOSR1R2_IconName As String = String.Empty      'R1 + S + R2 (BROWN).  Content: T. Status: R2
    Private REACPOSWASH_IconName As String = String.Empty      'Washing (LIGHT BLUE). Content: W. Status: -
    Private REACPOSDIL_IconName As String = String.Empty       'Dilution (LILAC).     Content: P. Status: P
    Private REACPOSELEC_IconName As String = String.Empty

    '** Variables for Well Status (used as Background Image) - WELL IS SELECTED
    Private REACPOSCONTS_IconName As String = String.Empty     'Contaminated Selected (RED).   Content: C. Status: -
    Private REACPOSFINIS_IconName As String = String.Empty     'Finish Selected (GREEN).       Content: T. Status: F
    Private REACPOSNIUSS_IconName As String = String.Empty     'Not in Use	Selected (WHITE).  Content: E. Status: R
    Private REACPOSOPTIS_IconName As String = String.Empty     'Rejected Selected (BLACK).     Content:	E. Status: X
    Private REACPOSR1S_IconName As String = String.Empty       'R1 Selected (LIGHT ORANGE).    Content: T. Status: R1
    Private REACPOSR1SLS_IconName As String = String.Empty     'R1 + S Selected (ORANGE).      Content: T. Status: S
    Private REACPOSR1R2S_IconName As String = String.Empty     'R1 + S + R2 Selected (BROWN).  Content: T. Status: R2
    Private REACPOSWASHS_IconName As String = String.Empty     'Washing Selected (LIGHT BLUE). Content: W. Status: -
    Private REACPOSDILS_IconName As String = String.Empty      'Dilution (LILAC).              Content: P. Status: P

    '** Variables for Reactions Rotor Legend Area
    Private LEGREACTIONWASHING_IconName As String = String.Empty
    Private LEGREACTIONNOTINUSE_IconName As String = String.Empty
    Private LEGREACTIONR1_IconName As String = String.Empty
    Private LEGREACTIONR1SAMPLE_IconName As String = String.Empty
    Private LEGREACTIONR1SAMPLER2_IconName As String = String.Empty
    Private LEGREACTIONDILUTION_IconName As String = String.Empty
    Private LEGREACTIONFINISH_IconName As String = String.Empty
    Private LEGREACTIONCONTAM_IconName As String = String.Empty
    Private LEGREACTIONOPTICAL_IconName As String = String.Empty


    'Private FEWVOL_IconName As String = ""
    'Private SELECTED_IconName As String = ""
    'Private BARCODEERROR_IconName As String = ""
    'Private BOTTLE3_SEL_IconName As String = "" 'Selected bottle (60ml) (Reagents or additional solution) image inside rotor

    
#End Region

#Region "Methods"

    Private Sub ChangeControlPositionImage(ByRef pControl As BSRImage, ByVal pImagePath As String, Optional ByVal pTransparentImage As Boolean = False)
        Try
            If (pControl.ImagePath <> pImagePath) Then
                pControl.ImagePath = pImagePath
                pControl.IsTransparentImage = pTransparentImage

                If (pImagePath <> Nothing) Then
                    pControl.Image = Image.FromFile(pImagePath)
                    pControl.BringToFront()
                Else
                    pControl.Image = Nothing
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ChangeControlPositionImage ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ChangeControlPositionImage", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    Private Sub CleanInfoArea(ByVal pAfterReset As Boolean)
        Try
            'Info Area for Samples Rotor
            If (Not pAfterReset OrElse myRotorTypeForm = "SAMPLES") Then
                bsSampleCellTextBox.Clear()
                bsSampleDiskNameTextBox.Clear()
                bsSampleContentTextBox.Clear()
                bsSamplesBarcodeTextBox.Clear()
                bsSampleNumberTextBox.Clear()
                bsSampleIDTextBox.Clear()
                bsSampleTypeTextBox.Clear()
                bsDiluteStatusTextBox.Clear()
                bsTubeSizeComboBox.SelectedIndex = -1
            End If

            'Info Area for Reagents Rotor
            If (Not pAfterReset OrElse myRotorTypeForm = "REAGENTS") Then
                bsReagentsCellTextBox.Clear()
                bsReagentsDiskNameTextBox.Clear()
                bsReagentsContentTextBox.Clear()
                bsReagentsBarCodeTextBox.Clear()
                bsReagentNameTextBox.Clear()
                bsReagentsNumberTextBox.Clear()
                bsTestNameTextBox.Clear()
                bsExpirationDateTextBox.Clear()
                bsBottleSizeComboBox.SelectedIndex = -1
                bsCurrentVolTextBox.Clear()
                bsTeststLeftTextBox.Clear()
            End If

            'Info Area for Reactions Rotor
            If (Not pAfterReset OrElse myRotorTypeForm = "REACTIONS") Then
                bsWellNrTextBox.Clear()
                bsSampleClassTextBox.Clear()
                bsCalibNrTextBox.Clear()
                bsPatientIDTextBox.Clear()
                bsReacSampleTypeTextBox.Clear()
                bsDilutionTextBox.Clear()
                bsReplicateTextBox.Clear()
                bsRerunTextBox.Clear()
                bsReacTestTextBox.Clear()
                bsReacStatusTextBox.Clear()
                BsExecutionIDTextBox.Clear()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CleanInfoArea", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CleanInfoArea", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    Private Function CreatePosControlList(ByVal MyControlCollection As Control.ControlCollection) As List(Of BSRImage)
        Dim myPosControlList As New List(Of BSRImage)

        Try
            'Get all the Position Controls (Pos PictureBox) and create a list
            For Each myPosControl As Control In MyControlCollection
                If (myPosControl.Controls.Count > 0) Then
                    If Not TypeOf myPosControl Is SplitContainer Then
                        myPosControlList.AddRange(CreatePosControlList(myPosControl.Controls))
                    Else
                        myPosControlList = CreatePosControlList(myPosControl.Controls)
                    End If
                Else
                    'Get the control type to validate if it will go to our list (only for PictureBox)
                    If (TypeOf myPosControl Is BSRImage) Then
                        myPosControlList.Add(CType(myPosControl, BSRImage))
                    End If
                End If
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CreatePosControlList", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CreatePosControlList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
        Return myPosControlList
    End Function

    Private Sub FilterAllTubesbyRingNumberRotorType(ByVal pRingNumber As Integer)
        Dim result As List(Of TubeSizeTO)
        Try
            'Filter the AllTubeSizeList by RotorType and RingNumber
            result = (From a In AllTubeSizeList _
                     Where a.RingNumber = pRingNumber _
                   AndAlso a.RotorType = myRotorTypeForm _
                    Select a).ToList()

            If (result.Count > 0) Then
                'Validate which Rotor is active to set the list of values in the corresponding ComboBox
                If (myRotorTypeForm = "SAMPLES") Then
                    bsTubeSizeComboBox.DataSource = result
                    bsTubeSizeComboBox.DisplayMember = "FixedTubeName"
                    bsTubeSizeComboBox.ValueMember = "TubeCode"
                    bsTubeSizeComboBox.Refresh()
                Else
                    bsBottleSizeComboBox.DataSource = result
                    bsBottleSizeComboBox.DisplayMember = "FixedTubeName"
                    bsBottleSizeComboBox.ValueMember = "TubeCode"
                    bsTubeSizeComboBox.Refresh()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FilterAllTubesbyRingNumberRotorType ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FilterAllTubesbyRingNumberRotorType", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        Finally
            result = Nothing
        End Try
    End Sub

    Private Sub GetAllTubeSizes()
        Try
            Dim myGlobalDataTO As GlobalDataTO
            Dim myRCPDelegate As New WSRotorContentByPositionDelegate

            'Get all available Tubes/Bottles 
            myGlobalDataTO = myRCPDelegate.GetAllTubeSizes(Nothing, AnalyzerModel)
            If (Not myGlobalDataTO.HasError) Then
                AllTubeSizeList = CType(myGlobalDataTO.SetDatos, List(Of TubeSizeTO))
            Else
                ShowMessage(Me.Name & ".GetAllTubeSizes", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, MsgParent)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetAllTubeSizes", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetAllTubeSizes", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' Get the name of the Icon for the specified TubeContent 
    ''' </summary>
    ''' <param name="pTubeContent">Tube/Bottle Content</param>
    ''' <param name="pTubeSize">Tube/Bottle Size </param>
    ''' <param name="pRingNumber">Rotor Ring Number</param>
    ''' <param name="pInProcessBottle">When TRUE, it means the Reagent or Washing Solution is still needed in the active
    '''                                Work Session, and an special icon have to be shown (depending also on Bottle Size and Ring Number)</param>
    ''' <returns>The icon Name</returns>
    ''' <remarks>
    ''' Created by:  TR 15/01/2010 - Tested: OK.
    ''' Modified by: SA 18/11/2013 - Removed parameter pBarCodeStatus due to it is not used
    '''                              BT #1359 ==> Added new optional parameter pInProcessBottle. For TubeContent = REAGENT and/or WASH_SOL, 
    '''                                           previous code is valid only when pInProcessBottle is FALSE; otherwise, new icons showing 
    '''                                           In Process Reagents and/or Washing Solutions have to be loaded and shown
    ''' </remarks>
    Private Function GetIconNameByTubeContent(ByVal pTubeContent As String, ByVal pTubeSize As String, ByVal pRingNumber As Integer, _
                                              Optional ByVal pInProcessBottle As Boolean = False) As String
        Dim myIconPath As String = ""

        Try
            Select Case (pTubeContent)
                Case "CALIB"
                    myIconPath = CALIB_IconName
                    Exit Select

                Case "CTRL"
                    myIconPath = CTRL_IconName
                    Exit Select

                Case "PATIENT"
                    myIconPath = ROUTINES_IconName
                    Exit Select

                Case "TUBE_SPEC_SOL", "TUBE_WASH_SOL"
                    myIconPath = ADDSAMPLESOL_IconName
                    Exit Select

                Case "REAGENT"
                    If (Not pInProcessBottle) Then
                        If (pTubeSize = "BOTTLE2" OrElse pTubeSize = "BOTTLE1") Then
                            If (pRingNumber = 1) Then
                                myIconPath = BTLREAGSMALLD1_IconName
                            Else
                                myIconPath = BTLREAGSMALLD2_IconName
                            End If
                        ElseIf (pTubeSize = "BOTTLE3") Then
                            myIconPath = BTLREAGBIG2_IconName
                        End If
                    Else
                        'Load new icons for Second Reagents still in process in the active Work Session
                        If (pTubeSize = "BOTTLE2" OrElse pTubeSize = "BOTTLE1") Then
                            If (pRingNumber = 1) Then
                                myIconPath = BTL_IPRGSML1_IconName
                            Else
                                myIconPath = BTL_IPRGSML2_IconName
                            End If
                        ElseIf (pTubeSize = "BOTTLE3") Then
                            myIconPath = BTL_IPRGBIG2_IconName
                        End If
                    End If
                    Exit Select

                Case "SPEC_SOL"
                    If (pTubeSize = "BOTTLE2" OrElse pTubeSize = "BOTTLE1") Then
                        If (pRingNumber = 1) Then
                            myIconPath = BTLADDSOLSMALLR1_IconName
                        Else
                            myIconPath = BTLADDSOLSMALLR2_IconName
                        End If

                    ElseIf (pTubeSize = "BOTTLE3") Then
                        myIconPath = BTLADDSOLBIGR2_IconName
                    End If
                    Exit Select

                Case "WASH_SOL"
                    If (Not pInProcessBottle) Then
                        If (pTubeSize = "BOTTLE2" OrElse pTubeSize = "BOTTLE1") Then
                            If (pRingNumber = 1) Then
                                myIconPath = BTLADDSOLSMALLR1_IconName
                            Else
                                myIconPath = BTLADDSOLSMALLR2_IconName
                            End If

                        ElseIf (pTubeSize = "BOTTLE3") Then
                            myIconPath = BTLADDSOLBIGR2_IconName
                        End If
                    Else
                        'Load new icons for Washing Solutions needed to avoid Second Reagents contaminations in Reactions Rotor and still in process in the active Work Session
                        If (pTubeSize = "BOTTLE2" OrElse pTubeSize = "BOTTLE1") Then
                            If (pRingNumber = 1) Then
                                myIconPath = BTL_IPASSML1_IconName
                            Else
                                myIconPath = BTL_IPASSML2_IconName
                            End If
                        ElseIf (pTubeSize = "BOTTLE3") Then
                            myIconPath = BTL_IPASBIG2_IconName
                        End If
                    End If
                    Exit Select
            End Select
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetIconNameByTubeContent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetIconNameByTubeContent", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myIconPath
    End Function

    Private Function GetLocalPositionInfo(ByVal pRingNumber As Integer, ByVal pCellNumber As Integer, ByVal pMultiSelection As Boolean, _
                                          Optional ByVal isReaction As Boolean = False) As WSRotorContentByPositionDS
        Dim myRotorContentByPositionDS As New WSRotorContentByPositionDS

        Try
            'To get the information of the selected Rotor Position, filter the myRotorContentByPositionDSForm 
            'by the RotorType, RingNumber and CellNumber and store it in a DataSet
            Dim mySelectedElement As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)

            'If not multiselect, prepare to change the Select status of all others selected elements
            If (Not pMultiSelection) Then
                mySelectedElement = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                                    Where a.Selected = True _
                                  AndAlso a.RotorType = myRotorTypeForm _
                                 Order By a.RingNumber Ascending, a.CellNumber Ascending _
                                   Select a).ToList()

                'Change the selected status of all previous elements to false
                For Each myRCPRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In mySelectedElement
                    myRCPRow.Selected = False
                    MarkSelectedPosition(myRCPRow.RingNumber, myRCPRow.CellNumber, False)
                Next
            End If

            'Get the current selected element
            If (Not isReaction) Then
                mySelectedElement = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myRotorContentByPositionDSForm.twksWSRotorContentByPosition.AsEnumerable _
                                    Where a.RotorType = myRotorTypeForm _
                                  AndAlso a.RingNumber = pRingNumber _
                                  AndAlso a.CellNumber = pCellNumber _
                                   Select a).ToList()
            Else
                mySelectedElement = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myRotorContentByPositionDSForm.twksWSRotorContentByPosition.AsEnumerable _
                                    Where a.RotorType = myRotorTypeForm _
                                  AndAlso a.CellNumber = pCellNumber _
                                   Select a).ToList()
            End If

            'If current element found, then set Selected status to True
            If (mySelectedElement.Count > 0) Then
                mySelectedElement.First().Selected = Not mySelectedElement.First().Selected
            End If

            'Get all selected elements to return on the final Dataset
            mySelectedElement = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                                Where a.Selected = True _
                              AndAlso a.RotorType = myRotorTypeForm _
                             Order By a.RingNumber Ascending, a.CellNumber Ascending _
                               Select a).ToList()

            'Change select status to true and mark the position on the rotor area.
            For Each myRCPRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In mySelectedElement
                myRotorContentByPositionDS.twksWSRotorContentByPosition.ImportRow(myRCPRow)

                'Mark the selected position on the rotor changing the backgroun image
                MarkSelectedPosition(myRCPRow.RingNumber, myRCPRow.CellNumber, myRCPRow.Selected)
            Next
            mySelectedElement = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetLocalPositionInfo", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetLocalPositionInfo", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
        Return myRotorContentByPositionDS
    End Function

    ''' <summary>
    ''' Get the multilanguage ToolTips for all Buttons in the three Rotors Tabs
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 02/05/2011
    ''' </remarks>
    Private Sub GetScreenToolTip()
        Try
            Dim bsScreenToolTips As New ToolTip()

            'Samples Rotor Tab
            bsScreenToolTips.SetToolTip(bsSamplesMoveFirstPositionButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToFirst", LanguageID))
            bsScreenToolTips.SetToolTip(bsSamplesIncreaseButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToNext", LanguageID))
            bsScreenToolTips.SetToolTip(bsSamplesDecreaseButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToPrevious", LanguageID))
            bsScreenToolTips.SetToolTip(bsSamplesMoveLastPositionButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToLast", LanguageID))

            'Reagents Rotor Tab
            bsScreenToolTips.SetToolTip(bsReagentsMoveFirstPositionButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToFirst", LanguageID))
            bsScreenToolTips.SetToolTip(bsReagentsIncreaseButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToNext", LanguageID))
            bsScreenToolTips.SetToolTip(bsReagentsMoveLastPositionButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToLast", LanguageID))
            bsScreenToolTips.SetToolTip(bsReagentsDecreaseButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToPrevious", LanguageID))

            'Reactions Rotor Tab 
            bsScreenToolTips.SetToolTip(bsReactionsMoveFirstPositionButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToFirst", LanguageID))
            bsScreenToolTips.SetToolTip(bsReactionsIncreaseButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToNext", LanguageID))
            bsScreenToolTips.SetToolTip(bsReactionsMoveLastPositionButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToLast", LanguageID))
            bsScreenToolTips.SetToolTip(bsReactionsDecreaseButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToPrevious", LanguageID))
            bsScreenToolTips.SetToolTip(bsReactionsOpenGraph, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ABSORBANCE_CURVE", LanguageID))
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenToolTip ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenToolTip ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' Get the multilanguage description of the Status of the selected Rotor Cell
    ''' </summary>
    ''' <param name="pItemID">Status Code to search</param>
    ''' <returns>Status Description in the selected Language</returns>
    ''' <remarks>
    ''' Created by: TR 29/04/2011
    ''' </remarks>
    Private Function GetStatusDescOnCurrentLanguage(ByVal pItemID As String) As String
        Dim myResult As String = ""
        Try
            Dim mySubTableID As GlobalEnumerates.PreloadedMasterDataEnum
            Select Case myRotorTypeForm
                Case "SAMPLES"
                    mySubTableID = GlobalEnumerates.PreloadedMasterDataEnum.SAMPLE_POS_STATUS
                    Exit Select
                Case "REAGENTS"
                    mySubTableID = GlobalEnumerates.PreloadedMasterDataEnum.REAGENT_POS_STATUS
                    Exit Select
                Case "REACTIONS" 'AG 12/12/2011
                    mySubTableID = GlobalEnumerates.PreloadedMasterDataEnum.EXECUTION_STATUS
                    Exit Select
            End Select

            Dim myGlobalDataTO As New GlobalDataTO
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

            'Search the multilanguage description in table of Preloaded Master Data
            myGlobalDataTO = myPreloadedMasterDataDelegate.GetSubTableItem(Nothing, mySubTableID, pItemID)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreloMasterDS As PreloadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                If (myPreloMasterDS.tfmwPreloadedMasterData.Count > 0) Then myResult = myPreloMasterDS.tfmwPreloadedMasterData(0).FixedItemDesc
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetStatusDescOnCurrentLanguage ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetStatusDescOnCurrentLanguage ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
        Return myResult
    End Function

    Private Sub InitializeRotors()
        AnalyzerModel = "A400"

        PrepareButtons()
        PrepareIconNames()
        PreparePositionsControls()
        GetAllTubeSizes()
        LoadRotorAreaInfo(False, "")

        'Disable all controls in the Info Area
        StatusInfoAreaControls("SAMPLES", False)
        StatusInfoAreaControls("REAGENTS", False)
        StatusInfoAreaControls("REACTIONS", False)
    End Sub

    Private Sub LoadRotorAreaInfo(ByVal pReset As Boolean, ByVal pResetRotorType As String)
        Try
            'Dim StartTime As DateTime = Now

            Dim result As GlobalDataTO
            Dim myObj As New WSRotorContentByPositionDelegate

            'JV 03/12/2013: #1384 show the reagent rotor status without session
            'result = myObj.GetRotorContentPositions(Nothing, ActiveWorkSession, ActiveAnalyzer)
            If WorkSessionStatusField = "EMPTY" OrElse WorkSessionStatusField = "OPEN" Then
                result = myObj.GetRotorContentPositionsResetDone(Nothing, ActiveWorkSession, ActiveAnalyzer)
            Else
                result = myObj.GetRotorContentPositions(Nothing, ActiveWorkSession, ActiveAnalyzer)
            End If
            'JV 03/12/2013: #1384 show the reagent rotor status without session

            If (Not result.HasError AndAlso Not result.SetDatos Is Nothing) Then
                myRotorContentByPositionDSForm = DirectCast(result.SetDatos, WSRotorContentByPositionDS)

                'Update the Samples Rotor Area
                If (pResetRotorType = "SAMPLES") Then setControlPosToNothing = True Else setControlPosToNothing = False
                UpdateRotorTreeViewArea(myRotorContentByPositionDSForm, "SAMPLES")

                'Update the Reagents Rotor Area
                If (pResetRotorType = "REAGENTS") Then setControlPosToNothing = True Else setControlPosToNothing = False
                UpdateRotorTreeViewArea(myRotorContentByPositionDSForm, "REAGENTS")

                'Update the Reactions Rotor Area
                Dim myReactionsRotorDS As New ReactionsRotorDS
                Dim myReactionsRotor As New ReactionsRotorDelegate
                Dim RotorContentByPositionRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow

                result = myReactionsRotor.GetAllWellsLastTurn(Nothing, ActiveAnalyzer)
                If (Not result.HasError AndAlso Not result.SetDatos Is Nothing) Then myReactionsRotorDS = DirectCast(result.SetDatos, ReactionsRotorDS)

                Dim query As List(Of ReactionsRotorDS.twksWSReactionsRotorRow)
                For i As Integer = 1 To 120
                    Dim auxI = i
                    RotorContentByPositionRow = myRotorContentByPositionDSForm.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow()
                    RotorContentByPositionRow.RotorType = "REACTIONS"
                    RotorContentByPositionRow.AnalyzerID = ActiveAnalyzer
                    RotorContentByPositionRow.WorkSessionID = ActiveWorkSession
                    RotorContentByPositionRow.CellNumber = auxI

                    If Not myReactionsRotorDS Is Nothing AndAlso myReactionsRotorDS.twksWSReactionsRotor.Rows.Count > 0 Then
                        query = (From a In myReactionsRotorDS.twksWSReactionsRotor _
                                Where a.WellNumber = auxI _
                              AndAlso a.AnalyzerID = ActiveAnalyzer _
                               Select a).ToList()

                        If (query.Count > 0) Then
                            RotorContentByPositionRow.RingNumber = query(0).RotorTurn
                            RotorContentByPositionRow.TubeContent = query(0).WellContent
                            RotorContentByPositionRow.ElementStatus = query(0).WellStatus
                        Else
                            RotorContentByPositionRow.RingNumber = 0
                            RotorContentByPositionRow.TubeContent = "E"
                            RotorContentByPositionRow.ElementStatus = "R"
                        End If
                    Else
                        RotorContentByPositionRow.RingNumber = 0
                        RotorContentByPositionRow.TubeContent = "E"
                        RotorContentByPositionRow.ElementStatus = "R"
                    End If
                    myRotorContentByPositionDSForm.twksWSRotorContentByPosition.Rows.Add(RotorContentByPositionRow)
                Next i

                If String.Equals(pResetRotorType, "REACTIONS") Then setControlPosToNothing = True Else setControlPosToNothing = False
                UpdateRotorTreeViewArea(myRotorContentByPositionDSForm, "REACTIONS")

                setControlPosToNothing = False
            Else
                ShowMessage(Me.Name & ".LoadRotorAreaInfo", result.ErrorCode, result.ErrorMessage, MsgParent)
            End If

            'Dim ElapsedTime As Double = Now.Subtract(StartTime).TotalMilliseconds
            'Console.Text &= String.Format("LoadRotorAreaInfo: {0}{1}", ElapsedTime.ToStringWithDecimals(0), vbCrLf)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadRotorAreaInfo", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadRotorAreaInfo", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' Mark the selected position on the rotor area.
    ''' Activate the Background color in case the position control is selected.
    ''' </summary>
    ''' <param name="pRingNumber">Ring Number</param>
    ''' <param name="pCellNumber">Cell Number</param>
    ''' <param name="pMarkPosition">True when the Cell has been selected, and False when it has been unselected</param>
    ''' <remarks>
    ''' Created by:  
    ''' Modified by: SA 20/11/2013 - BT #1359 => Code changed to call a different function depending on the active Rotor Type
    ''' </remarks>
    Private Sub MarkSelectedPosition(ByVal pRingNumber As Integer, ByVal pCellNumber As Integer, ByVal pMarkPosition As Boolean)
        Try
            If (myRotorTypeForm = "SAMPLES") Then
                MarkSelectedPositionInSAMPLESRotor(pRingNumber, pCellNumber, pMarkPosition)
            ElseIf (myRotorTypeForm = "REAGENTS") Then
                MarkSelectedPositionInREAGENTSRotor(pRingNumber, pCellNumber, pMarkPosition)
            Else
                MarkSelectedPositionInREACTIONSRotor(pRingNumber, pCellNumber, pMarkPosition)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".MarkSelectedPosition", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".MarkSelectedPosition", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manage selection/unselection of Rotor Wells in Reactions Rotor, setting the proper BackgroundImage according the Well Status.
    ''' Also manage the availability of Well displacement buttons in Info Area Frame. 
    ''' </summary>
    ''' <param name="pRingNumber">Rotor Ring in which the selected Well is placed (always 1 for Reactions Rotor)</param>
    ''' <param name="pCellNumber">Selected Well Number</param>
    ''' <param name="pMarkPosition">True when the Well has been selected, and False when it has been unselected</param>
    ''' <remarks>
    ''' Created by:  SA 20/11/2013 - Code for REACTIONS Rotor moved from function MarkSelectedPosition   
    ''' </remarks>
    Private Sub MarkSelectedPositionInREACTIONSRotor(ByVal pRingNumber As Integer, ByVal pCellNumber As Integer, ByVal pMarkPosition As Boolean)
        Try
            'Get the screen Control that has been selected/unselected
            Dim FilterName As String = "Reac" & pCellNumber
            Dim controlQuery As List(Of BSRImage) = (From b In myPosControlList.AsEnumerable _
                                                    Where b.Name = FilterName _
                                                   Select b).ToList()

            If (controlQuery.Count > 0) Then
                Dim auxIconPath As String = String.Empty

                If (pMarkPosition) Then
                    Dim myImage() As String = controlQuery.First.ImagePath.ToString.Split("\"c)
                    'TODO: This is wrong parsing, we're splitting by a ., p, n and g. Not by ".png"
                    Dim myImageName() As String = myImage(UBound(myImage)).ToString.Split(".png".ToCharArray)

                    'Set cell to selected and check previous status
                    PreviousSelect = myImageName(0)
                    Select Case PreviousSelect
                        Case "REACPOSR1"
                            auxIconPath = REACPOSR1S_IconName       'Previous R1 to R1 selected
                        Case "REACPOSDIL"
                            auxIconPath = REACPOSDILS_IconName      'Previous dilution to dilution selected 
                        Case "REACPOSNIUS"
                            auxIconPath = REACPOSNIUSS_IconName     'Previous No in use to no in use selected
                        Case "REACPOSR1SL"
                            auxIconPath = REACPOSR1SLS_IconName     'Previous R1SL to R1SL selected
                        Case "REACPOSCONT"
                            auxIconPath = REACPOSCONTS_IconName     'Previous Contaminated to contaminated selected
                        Case "REACPOSFINI"
                            auxIconPath = REACPOSFINIS_IconName     'Previous finished to finished selected
                        Case "REACPOSR1R2"
                            auxIconPath = REACPOSR1R2S_IconName     'Previous R1R2 to R1R2 selected
                        Case "REACPOSWASH"
                            auxIconPath = REACPOSWASHS_IconName     'Previous washing to washing selected
                        Case "REACPOSOPTI"
                            auxIconPath = REACPOSOPTIS_IconName     'Previous optical to òptical selected 
                        Case "REACPOSR1S"
                            auxIconPath = REACPOSR1S_IconName       'Previous R1 selected to R1 selected
                        Case "REACPOSDILS"
                            auxIconPath = REACPOSDILS_IconName      'Previous dilution selected to dilution selected 
                        Case "REACPOSNIUSS"
                            auxIconPath = REACPOSNIUSS_IconName     'Previous No In use selected to no in use selected 
                        Case "REACPOSR1SLS"
                            auxIconPath = REACPOSR1SLS_IconName     'Previous R1SL selected to R1SL selected
                        Case "REACPOSCONTS"
                            auxIconPath = REACPOSCONTS_IconName     'Previous contaminated selected to contaminated selected
                        Case "REACPOSFINIS"
                            auxIconPath = REACPOSFINIS_IconName     'Previous finish selected to finish seleted
                        Case "REACPOSR1R2S"
                            auxIconPath = REACPOSR1R2S_IconName     'Previous R1R2 selected to R1R2 selected
                        Case "REACPOSWASHS"
                            auxIconPath = REACPOSWASHS_IconName     'Previous washing selected to washing selected 
                        Case "REACPOSOPTIS"
                            auxIconPath = REACPOSOPTIS_IconName     'Previous Optical selected to optical selected
                        Case Else
                            auxIconPath = REACPOSELEC_IconName
                    End Select

                    controlQuery.First.Image = Image.FromFile(MyBase.IconsPath & auxIconPath)
                    controlQuery.First.BringToFront()
                    controlQuery.First.Refresh()
                Else
                    Select Case PreviousSelect
                        Case "REACPOSR1"
                            auxIconPath = REACPOSR1_IconName        'Previous R1 to R1
                        Case "REACPOSDIL"
                            auxIconPath = REACPOSDIL_IconName       'Previous dilution to dilution
                        Case "REACPOSNIUS"
                            auxIconPath = REACPOSNIUS_IconName      'Previous No in use to No in use
                        Case "REACPOSR1SL"
                            auxIconPath = REACPOSR1SL_IconName      'Previous R1SL to R1SL
                        Case "REACPOSCONT"
                            auxIconPath = REACPOSCONT_IconName      'Previous contaminated to contaminated
                        Case "REACPOSFINI"
                            auxIconPath = REACPOSFINI_IconName      'Previous finish to finish
                        Case "REACPOSR1R2"
                            auxIconPath = REACPOSR1R2_IconName      'previous R1R2 to R1R2
                        Case "REACPOSWASH"
                            auxIconPath = REACPOSWASH_IconName      'previous washing to washing
                        Case "REACPOSOPTI"
                            auxIconPath = REACPOSOPTI_IconName      'previous optical to optical
                        Case "REACPOSR1S"
                            auxIconPath = REACPOSR1_IconName        'Previous R1 selected to R1
                        Case "REACPOSDILS"
                            auxIconPath = REACPOSDIL_IconName       'Previous Dilution selected to Dilution
                        Case "REACPOSNIUSS"
                            auxIconPath = REACPOSNIUS_IconName      'Previous No in use selected to no in use
                        Case "REACPOSR1SLS"
                            auxIconPath = REACPOSR1SL_IconName      'Previous R1SL selected to R1SL
                        Case "REACPOSCONTS"
                            auxIconPath = REACPOSCONT_IconName      'Previous contaminated selected to contaminated selected 
                        Case "REACPOSFINIS"
                            auxIconPath = REACPOSFINI_IconName      'Previous finished selected to finish
                        Case "REACPOSR1R2S"
                            auxIconPath = REACPOSR1R2_IconName      'Previous R1R2 selected to R1R2
                        Case "REACPOSWASHS"
                            auxIconPath = REACPOSWASH_IconName      'previous washing selected to washing
                        Case "REACPOSOPTIS"
                            auxIconPath = REACPOSOPTI_IconName      'Previous optical selected to optical
                    End Select

                    If (PreviousSelect <> String.Empty) Then
                        controlQuery.First.Image = Image.FromFile(MyBase.IconsPath & auxIconPath)
                        controlQuery.First.BringToFront()
                        controlQuery.First.Refresh()
                    Else
                        controlQuery.First.Image = Nothing
                    End If
                End If
            End If

            'Control availability of displacement buttons in Info Area frame
            Dim hasSelect As EnumerableRowCollection(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
            hasSelect = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                        Where a.RotorType = myRotorTypeForm _
                      AndAlso a.Selected = True _
                       Select a)

            Dim enableDisplacementButtons As Boolean = (hasSelect.Count > 0)

            bsReactionsDecreaseButton.Enabled = enableDisplacementButtons
            bsReactionsIncreaseButton.Enabled = enableDisplacementButtons
            bsReactionsMoveFirstPositionButton.Enabled = enableDisplacementButtons
            bsReactionsMoveLastPositionButton.Enabled = enableDisplacementButtons

            controlQuery = Nothing
            hasSelect = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".MarkSelectedPositionInREACTIONSRotor", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".MarkSelectedPositionInREACTIONSRotor", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manage selection/unselection of Rotor Cells in Reagents Rotor, setting the proper Icon Image according Status and Barcode Status of the Cell,
    ''' and also depending on if the Element placed in the cell is still needed for the execution of the active Work Session.
    ''' Also manage the availability of the TextBox for Reagents Barcode and Cell displacement buttons in Info Area Frame. 
    ''' </summary>
    ''' <param name="pRingNumber">Rotor Ring in which the selected Cell is placed</param>
    ''' <param name="pCellNumber">Selected Cell</param>
    ''' <param name="pMarkPosition">True when the Cell has been selected, and False when it has been unselected</param>
    ''' <remarks>
    ''' Created by:  SA 20/11/2013 - Code for REAGENTS Rotor moved from function MarkSelectedPosition. Additionally, added changes needed for:
    '''                              BT #1359 ==> When Status is DEPLETED, LOCKED or FEW, previous code is valid 
    '''                                           only when pInProcessBottle is FALSE; otherwise, new icons showing Reagents and/or Washing Solutions still needed
    '''                                           for the execution of the active Work Session have to be loaded and shown 
    ''' Modified by: SA 04/12/2013 - Include BarCodeStatus = EMPTY in the list of conditions for Bottle without Barcode or with a correct one
    ''' </remarks>
    Private Sub MarkSelectedPositionInREAGENTSRotor(ByVal pRingNumber As Integer, ByVal pCellNumber As Integer, ByVal pMarkPosition As Boolean)
        Try
            'Get the screen Control that has been selected/unselected
            Dim rotorPrefix As String = "Reag"
            Dim FilterName As String = String.Format("{0}{1}{2}", rotorPrefix, pRingNumber, pCellNumber)
            Dim controlQuery As List(Of BSRImage) = (From b In myPosControlList.AsEnumerable _
                                                    Where b.Name = FilterName _
                                                   Select b).ToList()

            'Get the content of the selected/unselected position
            Dim posContent As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
            posContent = (From c As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                         Where c.RotorType = myRotorTypeForm _
                       AndAlso c.RingNumber = pRingNumber _
                       AndAlso c.CellNumber = pCellNumber _
                        Select c).ToList()

            If (controlQuery.Count > 0) AndAlso (posContent.Count > 0) Then
                'POSITION HAS BEEN SELECTED
                If (pMarkPosition) Then
                    If (String.IsNullOrEmpty(posContent.First.BarcodeStatus) OrElse posContent.First.BarcodeStatus = "OK" OrElse posContent.First.BarcodeStatus = "EMPTY") Then
                        'Special treatment for FREE positions....
                        If (posContent.First.Status = "FREE") Then
                            If (pRingNumber = 1) Then
                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BOTTLE2_EMPTYCELL_IconName)
                            Else
                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BOTTLE3_EMPTYCELL_IconName)
                            End If
                        End If

                        'Small Bottles are allowed in both Rings => The Icon of Small Bottle to display will depend on the Cell Status 
                        'and also on whether the Element placed in the cell is still needed for the execution of the active Work Session.
                        If (posContent.First.TubeType = "BOTTLE2") Then
                            If (pRingNumber = 1) Then
                                'Small Bottle is 20mL, but the Icon used for Ring 1 is different of the Icon of Small Bottle for Ring 2
                                Select Case (posContent.First.Status)
                                    Case "NO_INUSE"
                                        ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLNUSESMALLR1_SEL_IconName)
                                    Case "INUSE"
                                        If (Not posContent.First.InProcessElement) Then
                                            If (posContent.First.TubeContent = "REAGENT") Then
                                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLREAGSMALLD1_SEL_IconName)
                                            Else
                                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLADDSOLSMALLR1_SEL_IconName)
                                            End If
                                        Else
                                            'Load new icon for selected Reagents and/or Washing Solution small bottles in Ring 1 that are still in process in the active Work Session
                                            If (posContent.First.TubeContent = "REAGENT") Then
                                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTL_IPRGSML1_SEL_IconName)
                                            Else
                                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTL_IPASSML1_SEL_IconName)
                                            End If
                                        End If
                                    Case "DEPLETED", "LOCKED"
                                        If (Not posContent.First.InProcessElement) Then
                                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLDEPLETSMALLR1_SEL_IconName)
                                        Else
                                            'Load new icon for selected Depleted/Locked small bottles in Ring 1 that are still in process in the active Work Session
                                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTL_IPDPSML1_SEL_IconName)
                                        End If
                                    Case "FEW"
                                        If (Not posContent.First.InProcessElement) Then
                                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLFEWSMALLR1_SEL_IconName)
                                        Else
                                            'Load new icon for selected Low Volume small bottles in Ring 1 that are still in process in the active Work Session
                                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTL_IPFWSML1_SEL_IconName)
                                        End If
                                End Select
                            Else
                                'Small Bottle is 20mL, but the Icon used for Ring 2 is different of the Icon of Small Bottle for Ring 1
                                Select Case (posContent.First.Status)
                                    Case "NO_INUSE"
                                        ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLNUSESMALLR2_SEL_IconName)
                                    Case "INUSE", "IN_USE"
                                        If (Not posContent.First.InProcessElement) Then
                                            If (posContent.First.TubeContent = "REAGENT") Then
                                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLREAGSMALLD2_SEL_IconName)
                                            Else
                                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLADDSOLSMALLR2_SEL_IconName)
                                            End If
                                        Else
                                            'Load new icon for selected Reagents and/or Washing Solution small bottles in Ring 2 that are still in process in the active Work Session
                                            If (posContent.First.TubeContent = "REAGENT") Then
                                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTL_IPRGSML2_SEL_IconName)
                                            Else
                                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTL_IPASSML2_SEL_IconName)
                                            End If
                                        End If
                                    Case "DEPLETED", "LOCKED"
                                        If (Not posContent.First.InProcessElement) Then
                                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLDEPLETSMALLR2_SEL_IconName)
                                        Else
                                            'Load new icon for selected Depleted/Locked small bottles in Ring 2 that are still in process in the active Work Session
                                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTL_IPDPSML2_SEL_IconName)
                                        End If
                                    Case "FEW"
                                        If (Not posContent.First.InProcessElement) Then
                                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLFEWSMALLR2_SEL_IconName)
                                        Else
                                            'Load new icon for selected Low Volume small bottles in Ring 2 that are still in process in the active Work Session
                                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTL_IPFWSML2_SEL_IconName)
                                        End If
                                End Select
                            End If
                        ElseIf (posContent.First.TubeType = "BOTTLE3") Then
                            'Big Bottles are allowed only in the internal Ring (Ring 2)
                            Select Case (posContent.First.Status)
                                Case "NO_INUSE"
                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLNUSEBIGR2_SEL_IconName)
                                Case "INUSE", "IN_USE"
                                    If (Not posContent.First.InProcessElement) Then
                                        If (posContent.First.TubeContent = "REAGENT") Then
                                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLREAGBIG2_SEL_IconName)
                                        Else
                                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLADSLBIGR2SEL_IconName)
                                        End If
                                    Else
                                        'Load new icon for selected Reagents and/or Washing Solution big bottles in Ring 2 that are still in process in the active Work Session
                                        If (posContent.First.TubeContent = "REAGENT") Then
                                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTL_IPRGBIG2_SEL_IconName)
                                        Else
                                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTL_IPASBIG2_SEL_IconName)
                                        End If
                                    End If
                                Case "DEPLETED", "LOCKED"
                                    If (Not posContent.First.InProcessElement) Then
                                        ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLDEPLBIGR2_SEL_IconName)
                                    Else
                                        'Load new icon for selected Depleted/Locked big bottles in Ring 2 that are still in process in the active Work Session
                                        ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTL_IPDPBIG2_SEL_IconName)
                                    End If
                                Case "FEW"
                                    If (Not posContent.First.InProcessElement) Then
                                        ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLFEWBIGR2_SEL_IconName)
                                    Else
                                        'Load new icon for selected Low Volume big bottles in Ring 2 that are still in process in the active Work Session
                                        ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTL_IPFWBIG2_SEL_IconName)
                                    End If
                            End Select
                        End If
                    Else
                        'When BarcodeStatus is ERROR or UNKNOWN, the Icon to shown will depend on Cell Status, Ring Number and Bottle Size
                        If (pRingNumber = 1) Then
                            'In first Ring only small Bottles are allowed 
                            If (posContent.First.BarcodeStatus = "ERROR") Then
                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCERRSMALLR1_SEL_IconName)
                            ElseIf (posContent.First.BarcodeStatus = "UNKNOWN") Then
                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCUKNSMALLR1_SEL_IconName)
                            Else
                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BOTTLE2_EMPTYCELL_IconName)
                            End If
                        Else
                            'In the second Ring, both Bottle Sizes are allowed
                            If (posContent.First.BarcodeStatus = "ERROR") Then
                                If (posContent.First.TubeType = "BOTTLE2") Then
                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCERRSMALLR2_SEL_IconName)

                                ElseIf (posContent.First.TubeType = "BOTTLE3") Then
                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCERRBIGR2_SEL_IconName)
                                End If
                            ElseIf (posContent.First.BarcodeStatus = "UNKNOWN") Then
                                If (posContent.First.TubeType = "BOTTLE2") Then
                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCUKNSMALLR2_SEL_IconName)

                                ElseIf (posContent.First.TubeType = "BOTTLE3") Then
                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCUKNBIGR2_SEL_IconName)
                                End If
                            Else
                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BOTTLE3_EMPTYCELL_IconName)
                            End If
                        End If
                    End If

                Else
                    'THE ROTOR CELL HAS BEEN UNSELECTED
                    If (posContent.First.Status = "FREE") Then
                        ChangeControlPositionImage(controlQuery.First, Nothing, True)

                    ElseIf (posContent.First.TubeContent = "REAGENT") Then
                        If (Not posContent.First.InProcessElement) Then
                            If (posContent.First.TubeType = "BOTTLE2") Then
                                If (pRingNumber = 1) Then
                                    Me.ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLREAGSMALLD1_IconName)
                                Else
                                    Me.ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLREAGSMALLD2_IconName)
                                End If
                            ElseIf (posContent.First.TubeType = "BOTTLE3") Then
                                Me.ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLREAGBIG2_IconName)
                            End If
                        Else
                            'Load new icons for unselected Reagent bottles (both Rings) that are still in process in the active Work Session
                            If (posContent.First.TubeType = "BOTTLE2") Then
                                If (pRingNumber = 1) Then
                                    Me.ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTL_IPRGSML1_IconName)
                                Else
                                    Me.ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTL_IPRGSML2_IconName)
                                End If
                            ElseIf (posContent.First.TubeType = "BOTTLE3") Then
                                Me.ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTL_IPRGBIG2_IconName)
                            End If
                        End If

                    Else 'Additional Solutions (Special and Washing)
                        If (Not posContent.First.InProcessElement) Then
                            If (posContent.First.TubeType = "BOTTLE2") Then
                                If (pRingNumber = 1) Then
                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLADDSOLSMALLR1_IconName)
                                Else
                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLADDSOLSMALLR2_IconName)
                                End If

                            ElseIf (posContent.First.TubeType = "BOTTLE3") Then
                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLADDSOLBIGR2_IconName)
                            End If
                        Else
                            'Load new icons for unselected Washing Solution bottles (both Rings) that are still in process in the active Work Session
                            If (posContent.First.TubeType = "BOTTLE2") Then
                                If (pRingNumber = 1) Then
                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTL_IPASSML1_IconName)
                                Else
                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTL_IPASSML2_IconName)
                                End If

                            ElseIf (posContent.First.TubeType = "BOTTLE3") Then
                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTL_IPASBIG2_IconName)
                            End If
                        End If
                    End If

                    SetPosControlBackGroundForREAGENTSRotor(controlQuery.First(), posContent.First.Status, posContent.First.TubeType, _
                                                            posContent.First.RingNumber, posContent.First.BarcodeStatus, posContent.First.InProcessElement)
                End If
            ElseIf (controlQuery.Count > 0) Then
                controlQuery.First.BackgroundImage = Nothing
            End If

            'Control availability of displacement buttons in Info Area frame
            Dim hasSelect As EnumerableRowCollection(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
            hasSelect = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                        Where a.RotorType = myRotorTypeForm _
                      AndAlso a.Selected = True _
                       Select a)

            Dim enableDisplacementButtons As Boolean = (hasSelect.Count > 0)

            bsReagentsMoveFirstPositionButton.Enabled = enableDisplacementButtons
            bsReagentsMoveLastPositionButton.Enabled = enableDisplacementButtons
            bsReagentsDecreaseButton.Enabled = enableDisplacementButtons
            bsReagentsIncreaseButton.Enabled = enableDisplacementButtons

            controlQuery = Nothing
            posContent = Nothing
            hasSelect = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".MarkSelectedPositionInREAGENTSRotor", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".MarkSelectedPositionInREAGENTSRotor", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manage selection/unselection of Rotor Cells in Samples Rotor, setting the proper BackgroundImage according Status and Barcode Status of the Cell.
    ''' Also manage the availability of the TextBox for Samples Barcode and Cell displacement buttons in Info Area Frame. 
    ''' </summary>
    ''' <param name="pRingNumber">Rotor Ring in which the selected Cell is placed</param>
    ''' <param name="pCellNumber">Selected Cell</param>
    ''' <param name="pMarkPosition">True when the Cell has been selected, and False when it has been unselected</param>
    ''' <remarks>
    ''' Created by:  SA 20/11/2013 - Code for SAMPLES Rotor moved from function MarkSelectedPosition   
    ''' </remarks>
    Private Sub MarkSelectedPositionInSAMPLESRotor(ByVal pRingNumber As Integer, ByVal pCellNumber As Integer, ByVal pMarkPosition As Boolean)
        Try
            'Get the screen Control that has been selected/unselected
            Dim rotorPrefix As String = "Sam"
            Dim FilterName As String = String.Format("{0}{1}{2}", rotorPrefix, pRingNumber, pCellNumber)
            Dim controlQuery As List(Of BSRImage) = (From b In myPosControlList.AsEnumerable _
                                                    Where b.Name = FilterName _
                                                   Select b).ToList()

            'Get the content of the selected position
            Dim posContent As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
            posContent = (From c As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                         Where c.RotorType = myRotorTypeForm _
                       AndAlso c.RingNumber = pRingNumber _
                       AndAlso c.CellNumber = pCellNumber _
                        Select c).ToList()

            If (controlQuery.Count > 0) AndAlso (posContent.Count > 0) Then
                If (pMarkPosition) Then
                    'Load the proper Cell Background depending on both, the Tube Status and the BarcodeStatus
                    Dim myTubeContent As String = posContent.First.TubeContent
                    If (myTubeContent = "PATIENT" OrElse myTubeContent = "CTRL" OrElse myTubeContent = "CALIB" OrElse _
                        myTubeContent = "SPEC_SOL" OrElse myTubeContent = "TUBE_SPEC_SOL" OrElse _
                        myTubeContent = "WASH_SOL" OrElse myTubeContent = "TUBE_WASH_SOL") Then
                        If (posContent.First.BarcodeStatus = "ERROR") Then
                            controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLEBCERR_SEL_IconName)
                        Else
                            Select Case (posContent.First.Status)
                                Case "NO_INUSE"
                                    controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLENOINUSE_SEL_IconName)
                                Case "DEPLETED"
                                    controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLEDEPLETED_SEL_IconName)
                                Case "PENDING"
                                    controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLEPENDING_SEL_IconName)
                                Case "INPROCESS"
                                    controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLEINPROCES_SEL_IconName)
                                Case "FINISHED"
                                    controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLEFINISHED_SEL_IconName)
                            End Select
                        End If
                    Else
                        If (posContent.First.BarcodeStatus = "ERROR") Then
                            controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLEBCERR_SEL_IconName)
                        Else
                            controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & EMPTYCELL_IconName)
                        End If
                    End If
                Else
                    'Set the proper Cell Background when Cell Rotor has been unselected
                    SetPosControlBackGroundForSAMPLESRotor(controlQuery.First(), posContent.First.Status, posContent.First.BarcodeStatus)
                End If
            ElseIf (controlQuery.Count > 0) Then
                controlQuery.First.BackgroundImage = Nothing
            End If

            'Control availability of displacement buttons in Info Area frame
            Dim hasSelect As EnumerableRowCollection(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
            hasSelect = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                        Where a.RotorType = myRotorTypeForm _
                      AndAlso a.Selected = True _
                       Select a)

            Dim enableDisplacementButtons As Boolean = (hasSelect.Count > 0)

            bsSamplesMoveFirstPositionButton.Enabled = enableDisplacementButtons
            bsSamplesMoveLastPositionButton.Enabled = enableDisplacementButtons
            bsSamplesDecreaseButton.Enabled = enableDisplacementButtons
            bsSamplesIncreaseButton.Enabled = enableDisplacementButtons

            controlQuery = Nothing
            posContent = Nothing
            hasSelect = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".MarkSelectedPositionInSAMPLESRotor", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".MarkSelectedPositionInSAMPLESRotor", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Method in charge to load the button images
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 05/05/2010
    ''' Modified by: RH 26/04/2011
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = String.Empty
            Dim iconPath As String = MyBase.IconsPath

            'MOVE TO FIRST ROTOR POSITION Buttons
            auxIconName = GetIconName("BACKWARDL")
            If (Not String.IsNullOrEmpty(auxIconName)) Then
                bsSamplesMoveFirstPositionButton.Image = Image.FromFile(iconPath & auxIconName)
                bsReagentsMoveFirstPositionButton.Image = Image.FromFile(iconPath & auxIconName)
                bsReactionsMoveFirstPositionButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'MOVE TO PREVIOUS ROTOR POSITION Buttons
            auxIconName = GetIconName("LEFT")
            If (Not String.IsNullOrEmpty(auxIconName)) Then
                bsSamplesDecreaseButton.Image = Image.FromFile(iconPath & auxIconName)
                bsReagentsDecreaseButton.Image = Image.FromFile(iconPath & auxIconName)
                bsReactionsDecreaseButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'MOVE TO NEXT ROTOR POSITION Buttons
            auxIconName = GetIconName("RIGHT")
            If (Not String.IsNullOrEmpty(auxIconName)) Then
                bsSamplesIncreaseButton.Image = Image.FromFile(iconPath & auxIconName)
                bsReagentsIncreaseButton.Image = Image.FromFile(iconPath & auxIconName)
                bsReactionsIncreaseButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'MOVE TO LAST ROTO POSITION Buttons
            auxIconName = GetIconName("FORWARDL")
            If Not String.IsNullOrEmpty(auxIconName) Then
                bsSamplesMoveLastPositionButton.Image = Image.FromFile(iconPath & auxIconName)
                bsReagentsMoveLastPositionButton.Image = Image.FromFile(iconPath & auxIconName)
                bsReactionsMoveLastPositionButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'OPEN CURVE GRAPH Button
            auxIconName = GetIconName("ABS_GRAPH")
            If (Not String.IsNullOrEmpty(auxIconName)) Then
                bsReactionsOpenGraph.Image = Image.FromFile(iconPath & auxIconName)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' Get names of all Icons needed for the Rotor Area and set Background Images of all PictureBox
    ''' in Legend frames
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 
    ''' Modified by: SA 20/11/2013 - BT #1359 => Load Icons for InProcess Reagents and/or Washing Solutions
    ''' </remarks>
    Private Sub PrepareIconNames()
        Try
            '*****************************
            '** ICONS FOR SAMPLES ROTOR **
            '*****************************

            'Icons for Cell Status (used as Background Image) - CELL IS NOT SELECTED
            NOTINUSE_IconName = GetIconName("NOT_INUSE")
            DEPLETED_IconName = GetIconName("DEPLETED")
            PENDING_IconName = GetIconName("LEG_PENDING")
            INPROGRESS_IconName = GetIconName("LEG_INPROGRESS")
            FINISHED_IconName = GetIconName("LEG_FINISH")
            BTLSAMPLEBCERR_IconName = GetIconName("SPLBCERR")

            'Icons for Cell Status (used as Background Image) - CELL IS SELECTED
            EMPTYCELL_IconName = GetIconName("EMPTYCELL")
            BTLSAMPLENOINUSE_SEL_IconName = GetIconName("SPLNUSE_SEL")
            BTLSAMPLEDEPLETED_SEL_IconName = GetIconName("SPLDEPL_SEL")
            BTLSAMPLEPENDING_SEL_IconName = GetIconName("SPLPEND_SEL")
            BTLSAMPLEINPROCES_SEL_IconName = GetIconName("SPLPROC_SEL")
            BTLSAMPLEFINISHED_SEL_IconName = GetIconName("SPLFINI_SEL")
            BTLSAMPLEBCERR_SEL_IconName = GetIconName("SPLBCERR_SEL")

            'Icons for Sample Class (used as Image in the Rotor)
            CALIB_IconName = GetIconName("CALIB")
            CTRL_IconName = GetIconName("CTRL")
            STATS_IconName = GetIconName("STATS")
            ROUTINES_IconName = GetIconName("ROUTINES")
            DILUTIONS_IconName = GetIconName("DILUTIONS")
            ADDSAMPLESOL_IconName = GetIconName("ADD_SAMPLE_SOL")

            'Load Icons for Samples Rotor Legend Area 
            LEGSAMPLESELECT_Iconname = GetIconName("MON_LEG_SELECT")
            LEGSAMPLENOTINUSE_IconName = GetIconName("MON_LEG_NOTINU")
            LEGSAMPLEDEPLETED_IconName = GetIconName("MON_LEG_DEPLET")
            LEGSAMPLEPENDING_IconName = GetIconName("MON_LEG_PENDIN")
            LEGSAMPLEINPROGR_IconName = GetIconName("MON_LEG_INPROG")
            LEGSAMPLEFINISHD_IconName = GetIconName("MON_LEG_FINISH")
            LEGSAMPLEBCERROR_IconName = GetIconName("MON_LEG_BCERR")

            '******************************
            '** ICONS FOR REAGENTS ROTOR **
            '******************************

            'Icons for Cell Status in Ring 1 (SMALL Bottles) - CELL IS NOT SELECTED           
            BOTTLE2_NOTHING_IconName = GetIconName("BOTTLE2_NOTHING")       'Empty 
            BTLNUSESMALLR1_IconName = GetIconName("BTLNUSESMLR1")           'Not In Use  
            BTLDEPLETSMALLR1_IconName = GetIconName("BTLDEPLETSMLR1")       'Depleted 
            BTLFEWSMALLR1_IconName = GetIconName("BTLFEWSMLR1")             'Low Volume (Few)
            BTLBCUKNSMALLR1_IconName = GetIconName("BTLUKWNSMLR1")          'Unknown
            BTLBCERRSMALLR1_IconName = GetIconName("BTLBCERRSMLR1")         'Barcode Error
            BTLREAGSMALLD1_IconName = GetIconName("BTLREAGSMLD1")           'In Use Reagent
            BTLADDSOLSMALLR1_IconName = GetIconName("BTLADDSOLSMLR1")       'In Use Additional Solution
            '** Icons for In Process Reagents and/or Washing Solutions
            BTL_IPRGSML1_IconName = GetIconName("BTL_IPRGSML1")
            BTL_IPASSML1_IconName = GetIconName("BTL_IPASSML1")
            BTL_IPDPSML1_IconName = GetIconName("BTL_IPDPSML1")
            BTL_IPFWSML1_IconName = GetIconName("BTL_IPFWSML1")

            'Icons for Cell Status in Ring 1 (SMALL Bottles) - CELL IS SELECTED           
            BOTTLE2_EMPTYCELL_IconName = GetIconName("BOTTLE2_EMPTY")
            BTLNUSESMALLR1_SEL_IconName = GetIconName("BTLNUSESMLR1SEL")
            BTLDEPLETSMALLR1_SEL_IconName = GetIconName("BTLDEPSMLR1SEL")
            BTLFEWSMALLR1_SEL_IconName = GetIconName("BTLFEWSMLR1SEL")
            BTLBCUKNSMALLR1_SEL_IconName = GetIconName("BTLUKWNSMLR1SEL")
            BTLBCERRSMALLR1_SEL_IconName = GetIconName("BTLBCERSMLR1SEL")
            BTLREAGSMALLD1_SEL_IconName = GetIconName("BTLREAGSMLD1SEL")
            BTLADDSOLSMALLR1_SEL_IconName = GetIconName("BTLADSLSMLR1SEL")
            '** Icons for In Process Reagents and/or Washing Solutions
            BTL_IPRGSML1_SEL_IconName = GetIconName("BTL_IPRGSML1SEL")
            BTL_IPASSML1_SEL_IconName = GetIconName("BTL_IPASSML1SEL")
            BTL_IPDPSML1_SEL_IconName = GetIconName("BTL_IPDPSML1SEL")
            BTL_IPFWSML1_SEL_IconName = GetIconName("BTL_IPFWSML1SEL")

            'Icons for Cell Status in Ring 2 (SMALL Bottles) - CELL IS NOT SELECTED          
            BTLNUSESMALLR2_IconName = GetIconName("BTLNUSESMLR2")
            BTLDEPLETSMALLR2_IconName = GetIconName("BTLDEPLETSMLR2")
            BTLFEWSMALLR2_IconName = GetIconName("BTLFEWSMLR2")
            BTLBCUKNSMALLR2_IconName = GetIconName("BTLUKWNSMLR2")
            BTLBCERRSMALLR2_IconName = GetIconName("BTLBCERRSMLR2")
            BTLREAGSMALLD2_IconName = GetIconName("BTLREAGSMLD2")
            BTLADDSOLSMALLR2_IconName = GetIconName("BTLADDSOLSMLR2")
            '** Icons for In Process Reagents and/or Washing Solutions
            BTL_IPRGSML2_IconName = GetIconName("BTL_IPRGSML2")
            BTL_IPASSML2_IconName = GetIconName("BTL_IPASSML2")
            BTL_IPDPSML2_IconName = GetIconName("BTL_IPDPSML2")
            BTL_IPFWSML2_IconName = GetIconName("BTL_IPFWSML2")

            'Icons for Cell Status in Ring 2 (SMALL Bottles) - CELL IS SELECTED        
            BTLNUSESMALLR2_SEL_IconName = GetIconName("BTLNUSESMLR2SEL")
            BTLDEPLETSMALLR2_SEL_IconName = GetIconName("BTLDEPSMLR2SEL")
            BTLFEWSMALLR2_SEL_IconName = GetIconName("BTLFEWSMLR2SEL")
            BTLBCUKNSMALLR2_SEL_IconName = GetIconName("BTLUKWNSMLR2SEL")
            BTLBCERRSMALLR2_SEL_IconName = GetIconName("BTLBCERSMLR2SEL")
            BTLREAGSMALLD2_SEL_IconName = GetIconName("BTLREAGSMLD2SEL")
            BTLADDSOLSMALLR2_SEL_IconName = GetIconName("BTLADSLSMLR2SEL")
            '** Icons for In Process Reagents and/or Washing Solutions
            BTL_IPRGSML2_SEL_IconName = GetIconName("BTL_IPRGSML2SEL")
            BTL_IPASSML2_SEL_IconName = GetIconName("BTL_IPASSML2SEL")
            BTL_IPDPSML2_SEL_IconName = GetIconName("BTL_IPDPSML2SEL")
            BTL_IPFWSML2_SEL_IconName = GetIconName("BTL_IPFWSML2SEL")

            'Icons for Cell Status in Ring 2 (BIG Bottles) - CELL IS NOT SELECTED          
            BOTTLE3_NOTHING_IconName = GetIconName("BOTTLE3_NOTHING")
            BTLNUSEBIGR2_IconName = GetIconName("BTLNUSEBIGR2")
            BTLDEPLBIGR2_IconName = GetIconName("BTLDEPLETBIGR2")
            BTLFEWBIGR2_IconName = GetIconName("BTLFEWBIGR2")
            BTLBCUKNBIGR2_IconName = GetIconName("BTLUKWNBIGR2")
            BTLBCERRBIGR2_IconName = GetIconName("BTLBCERRBIGR2")
            BTLREAGBIG2_IconName = GetIconName("BTLREAGBIGD2")
            BTLADDSOLBIGR2_IconName = GetIconName("BTLADDSOLBIGR2")
            '** Icons for In Process Reagents and/or Washing Solutions
            BTL_IPRGBIG2_IconName = GetIconName("BTL_IPRGBIG2")
            BTL_IPASBIG2_IconName = GetIconName("BTL_IPASBIG2")
            BTL_IPDPBIG2_IconName = GetIconName("BTL_IPDPBIG2")
            BTL_IPFWBIG2_IconName = GetIconName("BTL_IPFWBIG2")

            'Icons for Cell Status in Ring 2 (BIG Bottles) - CELL IS SELECTED          
            BOTTLE3_EMPTYCELL_IconName = GetIconName("BOTTLE3_EMPTY")
            BTLNUSEBIGR2_SEL_IconName = GetIconName("BTLNUSEBIGR2SEL")
            BTLDEPLBIGR2_SEL_IconName = GetIconName("BTLDEPBIGR2SEL")
            BTLFEWBIGR2_SEL_IconName = GetIconName("BTLFEWBIGR2SEL")
            BTLBCUKNBIGR2_SEL_IconName = GetIconName("BTLUKWNBIGR2SEL")
            BTLBCERRBIGR2_SEL_IconName = GetIconName("BTLBCERBIGR2SEL")
            BTLREAGBIG2_SEL_IconName = GetIconName("BTLREAGBIGD2SEL")
            BTLADSLBIGR2SEL_IconName = GetIconName("BTLADSLBIGR2SEL")
            '** Icons for In Process Reagents and/or Washing Solutions
            BTL_IPRGBIG2_SEL_IconName = GetIconName("BTL_IPRGBIG2SEL")
            BTL_IPASBIG2_SEL_IconName = GetIconName("BTL_IPASBIG2SEL")
            BTL_IPDPBIG2_SEL_IconName = GetIconName("BTL_IPDPBIG2SEL")
            BTL_IPFWBIG2_SEL_IconName = GetIconName("BTL_IPFWBIG2SEL")

            'Icons for Reagents Rotor Legend Area
            LEGREAGENTREAGENT_IconName = GetIconName("LEG_BREAGENT")
            LEGREAGENTADDSOL_IconName = GetIconName("LEG_BWASH")
            LEGREAGENTDEPLETED_IconName = GetIconName("LEG_BDEPLETED")
            LEGREAGENTFEWVOL_IconName = GetIconName("LEG_BLOWER")
            LEGREAGENTNOTINUSE_IconName = GetIconName("LEG_BNOINUSE")
            LEGREAGENTBCERRORRG_IconName = GetIconName("LEG_BC_ERR")
            LEGBCREAGENTUNKNOWN_IconName = GetIconName("LEG_BC_UKN")
            '** Icon for In Process Reagents and/or Washing Solutions
            LEGREAGENTINPROCESS_IconName = GetIconName("LEG_BINPROCESS")

            'Icons for the type of element loaded in Reagents Rotor Cells
            REAGENTS_IconName = GetIconName("REAGENTS")
            ADDSOL_IconName = GetIconName("ADD_SOL")

            '*******************************
            '** ICONS FOR REACTIONS ROTOR **
            '*******************************

            'Icons for Well Status - WELL IS NOT SELECTED
            REACPOSCONT_IconName = GetIconName("REACPOSCONT")
            REACPOSFINI_IconName = GetIconName("REACPOSFINI")
            REACPOSNIUS_IconName = GetIconName("REACPOSNIUS")
            REACPOSOPTI_IconName = GetIconName("REACPOSOPTI")
            REACPOSR1_IconName = GetIconName("REACPOSR1")
            REACPOSR1SL_IconName = GetIconName("REACPOSR1SL")
            REACPOSR1R2_IconName = GetIconName("REACPOSR1R2")
            REACPOSWASH_IconName = GetIconName("REACPOSWASH")
            REACPOSDIL_IconName = GetIconName("REACPOSDIL")
            REACPOSELEC_IconName = GetIconName("REACPOSELEC")

            'Icons for Well Status - WELL IS SELECTED
            REACPOSCONTS_IconName = GetIconName("REASPOSCONT")
            REACPOSFINIS_IconName = GetIconName("REASPOSFINI")
            REACPOSNIUSS_IconName = GetIconName("REASPOSNIUS")
            REACPOSOPTIS_IconName = GetIconName("REASPOSOPTI")
            REACPOSR1S_IconName = GetIconName("REASPOSR1")
            REACPOSR1SLS_IconName = GetIconName("REASPOSR1SL")
            REACPOSR1R2S_IconName = GetIconName("REASPOSR1R2")
            REACPOSWASHS_IconName = GetIconName("REASPOSWASH")
            REACPOSDILS_IconName = GetIconName("REASPOSDIL")

            'Icons for Reactions Rotor Legend Area
            LEGREACTIONWASHING_IconName = GetIconName("REACWAHSING")
            LEGREACTIONNOTINUSE_IconName = GetIconName("REACNOTINUSE")
            LEGREACTIONR1_IconName = GetIconName("REACR1")
            LEGREACTIONR1SAMPLE_IconName = GetIconName("REACR1SAMPLE")
            LEGREACTIONR1SAMPLER2_IconName = GetIconName("REACR1SAMPLER2")
            LEGREACTIONDILUTION_IconName = GetIconName("REACDILUTION")
            LEGREACTIONFINISH_IconName = GetIconName("REACFINISH")
            LEGREACTIONCONTAM_IconName = GetIconName("REACCONTAM")
            LEGREACTIONOPTICAL_IconName = GetIconName("REACOPTIC")
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareIconNames ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareIconNames ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub PreparePositionsControls()
        Try
            'Get the list of all Positioning controls (BSRImage)
            myPosControlList = CreatePosControlList(SamplesTab.Controls)
            myPosControlList.AddRange(CreatePosControlList(ReagentsTab.Controls))
            myPosControlList.AddRange(CreatePosControlList(ReactionsTab.Controls))

            'Prepare all the Position controls 
            For Each myControl As BSRImage In myPosControlList
                If (myControl.Name.Contains("Sam")) Then
                    myControl.Tag = myControl.Name.Replace("Sam", String.Empty).Insert(1, ",")

                ElseIf (myControl.Name.Contains("Reag")) Then
                    myControl.Tag = myControl.Name.Replace("Reag", String.Empty).Insert(1, ",")

                ElseIf (myControl.Name.Contains("Reac")) Then
                    myControl.Tag = myControl.Name.Replace("Reac", String.Empty)

                End If

                myControl.Image = Nothing
                myControl.BackgroundImage = Nothing
                myControl.BackgroundImageLayout = ImageLayout.None
                myControl.InitialImage = Nothing

                myControl.BackColor = Color.Transparent
                myControl.SizeMode = PictureBoxSizeMode.CenterImage
                myControl.AllowDrop = False
            Next myControl
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PreparePositionsControls", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PreparePositionsControls", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' Select a new Cell Rotor and unselect the previously selected one when whatever of the Rotor displacement buttons is clicked
    ''' </summary>
    ''' <param name="pButtonPress">Integer value indicating which of the Rotor displacement buttons has been clicked. Possible values are the 
    '''                            following ones: 1-Move to First Cell, 2-Move to Previous Cell, 3-Move to Next Cell, 4-Move to Last Cell</param>
    ''' <remarks>
    ''' Created by:  AG
    ''' </remarks>
    Private Sub ScrollButtonsInfoArea(ByVal pButtonPress As Integer)
        Try
            'Check how many Cells are selected -> only one position selected is allowed in order to show InfoArea
            Dim query As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
            query = (From e As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                    Where e.Selected _
                  AndAlso e.RotorType = myRotorTypeForm _
                   Select e).ToList

            If (query.Count = 1) Then
                'Get the selected Cell and unselect it
                Dim CurrentCell As Integer = query.First().CellNumber
                query.First().Selected = False
                MarkSelectedPosition(query.First.RingNumber, query.First.CellNumber, query.First.Selected)  'UnMark the selected position

                'Get the maximum Cell number for the current Analyzer and RotorType
                Dim maxCellRotor As Integer = 1
                Dim myResultData As GlobalDataTO
                Dim AnalyzerConfg As New AnalyzerModelRotorsConfigDelegate

                myResultData = AnalyzerConfg.GetRotorMaxCellNumber(Nothing, AnalyzerIDField, myRotorTypeForm)
                If (Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing) Then
                    maxCellRotor = CType(myResultData.SetDatos, Integer)
                End If

                'Set the new Cell Number that has to be selected according the displacement button clicked
                Select Case pButtonPress
                    Case 1  'FIRST
                        CurrentCell = 1
                    Case 2  'PREVIOUS
                        If (CurrentCell > 1) Then CurrentCell = CurrentCell - 1
                    Case 3  'NEXT
                        If (CurrentCell < maxCellRotor) Then CurrentCell = CurrentCell + 1
                    Case 4  'LAST
                        CurrentCell = maxCellRotor
                End Select

                'Search the new Cell Number in the local DS and update Selected flag to TRUE for it
                query = (From e As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                        Where e.CellNumber = CurrentCell _
                      AndAlso e.RotorType = myRotorTypeForm _
                       Select e).ToList
                If (query.Count > 0) Then query.First.Selected = True

                'Get all information of the content of the new selected Cell Number and show this information in Info Area frame 
                mySelectedElementInfo = GetLocalPositionInfo(query.First.RingNumber, query.First.CellNumber, False)
                If (myRotorTypeForm = "REACTIONS") Then
                    ShowPositionInfoReactionsArea(ActiveAnalyzer, query.First.RingNumber, query.First.CellNumber)
                Else
                    ShowPositionInfoArea(myRotorTypeForm, query.First.RingNumber, query.First.CellNumber)
                End If

                'Finally, mark the new Cell Number as Selected
                MarkSelectedPosition(query.First.RingNumber, query.First.CellNumber, query.First.Selected)
            Else
                'When multiple selection NO info area is shown
                CleanInfoArea(False)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ScrollButtonsInfoArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ScrollButtonsInfoArea ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' Set the Position Control BackGround depending on the Status of the Element placed in the Rotor Cell.
    ''' </summary>
    ''' <param name="pPositionControl">Control in which the BackGround or Image has to be changed (PictureBox)</param>
    ''' <param name="pStatus">Current Cell Status</param>
    ''' <param name="pRotorType">Active Rotor Type: Samples or Reagents</param>
    ''' <param name="pTubeType">Size of the Tube/Bottle placed in the Rotor Cell</param>
    ''' <param name="pRingNumber">Rotor Ring Number</param>
    ''' <param name="pBarCodeStatus">Barcode Status for the Rotor Cell</param>
    ''' <param name="pInProcessElement">When True, it indicates that the Element placed in the Rotor Cell is still needed for the execution of the active 
    '''                                 Work Session. Optional parameter with default value FALSE. It is needed only when RotorType is REAGENTS</param>
    ''' <remarks>
    ''' Created by:  TR 28/01/2010 
    ''' Modified by: SA 18/11/2013 - BT #1359 => Added new optional parameter pInProcessBottle. Code changed to call a different function depending on the 
    '''                                          informed Rotor Type
    ''' </remarks>
    Private Sub SetPosControlBackGround(ByVal pPositionControl As BSRImage, ByVal pStatus As String, ByVal pRotorType As String, _
                                        ByVal pTubeType As String, ByVal pRingNumber As Integer, ByVal pBarCodeStatus As String, _
                                        Optional ByVal pInProcessElement As Boolean = False)
        Try
            If (pRotorType = "SAMPLES") Then
                SetPosControlBackGroundForSAMPLESRotor(pPositionControl, pStatus, pBarCodeStatus)

            ElseIf (pRotorType = "REAGENTS") Then
                SetPosControlBackGroundForREAGENTSRotor(pPositionControl, pStatus, pTubeType, pRingNumber, pBarCodeStatus, pInProcessElement)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SetPosControlBackGroung ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SetPosControlBackGroung", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Change the Icon to load in positions of REAGENTS Rotor depending on: Ring Number, Bottle Size, Cell Status, Barcode Status and also if the Element placed 
    ''' in the Rotor Cell is still needed for the execution of the active Work Session (pInProcessElement = TRUE). 
    ''' </summary>
    ''' <param name="pPositionControl">Control in which the BackGround or Image has to be changed (PictureBox)</param>
    ''' <param name="pStatus">Current Cell Status</param>
    ''' <param name="pTubeType">Size of the Bottle placed in the Rotor Cell</param>
    ''' <param name="pRingNumber">Rotor Ring Number</param>
    ''' <param name="pBarCodeStatus">Barcode Status for the Rotor Cell</param>
    ''' <param name="pInProcessElement">When True, it indicates that the Element placed in the Rotor Cell is still needed for the execution of the active 
    '''                                 Work Session. Optional parameter with default value FALSE</param>
    ''' <remarks>
    ''' Created by:  SA 18/11/2013 - Code extracted from function SetPosControlBackGroung to divide processing of positions in REAGENTS Rotor from processing of 
    '''                              positions in SAMPLES Rotor.  Additionally, added changes needed for:
    '''                              BT #1359 ==> Added new optional parameter pInProcessBottle. When Status is DEPLETED, LOCKED or FEW, previous code is valid 
    '''                                           only when pInProcessBottle is FALSE; otherwise, new icons showing Reagents and/or Washing Solutions still needed
    '''                                           for the execution of the active Work Session have to be loaded and shown
    ''' Modified by: SA 04/12/2013 - Include BarCodeStatus = EMPTY in the list of conditions for Bottle without Barcode or with a correct one 
    ''' </remarks>
    Private Sub SetPosControlBackGroundForREAGENTSRotor(ByVal pPositionControl As BSRImage, ByVal pStatus As String, ByVal pTubeType As String, ByVal pRingNumber As Integer, _
                                                        ByVal pBarCodeStatus As String, Optional ByVal pInProcessElement As Boolean = False)
        Try
            'When Bottle has not Barcode or it has a correct one, the Icon to shown will depend on Bottle Status (besides RingNumber and BottleSize): 
            '(1) Status = NO_INUSE           => Gray Bottle
            '(2) Status = DEPLETED or LOCKED => Red Bottle
            '(3) Status = FEW                => Lilac Bottle
            'However, when Status = DEPLETED, LOCKED or FEW, if pInProcessElement = TRUE, the bottle to show is Red or Lilac but showing also the colour that 
            'indicates it is still needed for the execution of the active WorkSession
            If (pBarCodeStatus = String.Empty OrElse pBarCodeStatus = "OK" OrElse pBarCodeStatus = "EMPTY") Then
                Select Case (pStatus)
                    Case "NO_INUSE"
                        If (pTubeType = "BOTTLE2" OrElse pTubeType = "BOTTLE1") Then
                            If (pRingNumber = 1) Then
                                ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLNUSESMALLR1_IconName)
                            Else
                                ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLNUSESMALLR2_IconName)
                            End If

                        ElseIf (pTubeType = "BOTTLE3") Then
                            ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLNUSEBIGR2_IconName)
                        End If
                        Exit Select

                    Case "DEPLETED", "LOCKED"
                        If (Not pInProcessElement) Then
                            If (pTubeType = "BOTTLE2" OrElse pTubeType = "BOTTLE1") Then
                                If (pRingNumber = 1) Then
                                    ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLDEPLETSMALLR1_IconName)
                                Else
                                    ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLDEPLETSMALLR2_IconName)
                                End If

                            ElseIf (pTubeType = "BOTTLE3") Then
                                ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLDEPLBIGR2_IconName)
                            End If
                        Else
                            'Load new icons for Depleted/Locked bottles that are still in process in the active Work Session
                            If (pTubeType = "BOTTLE2" OrElse pTubeType = "BOTTLE1") Then
                                If (pRingNumber = 1) Then
                                    ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTL_IPDPSML1_IconName)
                                Else
                                    ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTL_IPDPSML2_IconName)
                                End If

                            ElseIf (pTubeType = "BOTTLE3") Then
                                ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTL_IPDPBIG2_IconName)
                            End If
                        End If
                        Exit Select

                    Case "FEW"
                        If (Not pInProcessElement) Then
                            If (pTubeType = "BOTTLE2" OrElse pTubeType = "BOTTLE1") Then
                                If (pRingNumber = 1) Then
                                    ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLFEWSMALLR1_IconName)
                                Else
                                    ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLFEWSMALLR2_IconName)
                                End If

                            ElseIf (pTubeType = "BOTTLE3") Then
                                ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLFEWBIGR2_IconName)
                            End If
                        Else
                            'Load new icons for Low Volume bottles that are still in process in the active Work Session
                            If (pTubeType = "BOTTLE2" OrElse pTubeType = "BOTTLE1") Then
                                If (pRingNumber = 1) Then
                                    ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTL_IPFWSML1_IconName)
                                Else
                                    ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTL_IPFWSML2_IconName)
                                End If

                            ElseIf (pTubeType = "BOTTLE3") Then
                                ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTL_IPFWBIG2_IconName)
                            End If
                        End If
                        Exit Select

                    Case Else
                        pPositionControl.BackgroundImage = Nothing
                End Select
            Else
                'When Bottle has a wrong Barcode, the Icon to shown will depend on the BarcodeStatus (besides RingNumber and BottleSize):  
                '(1) BarcodeStatus = ERROR   => Bottle with X symbol in red
                '(2) BarcodeStatus = UNKNOWN => Streaked Bottle
                Select Case (pBarCodeStatus)
                    Case "ERROR"
                        If (pTubeType = "BOTTLE2" OrElse pTubeType = "BOTTLE1") Then
                            If (pRingNumber = 1) Then
                                ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCERRSMALLR1_IconName)
                            Else
                                ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCERRSMALLR2_IconName)
                            End If

                        ElseIf (pTubeType = "BOTTLE3") Then
                            ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCERRBIGR2_IconName)
                        End If
                        Exit Select

                    Case "UNKNOWN"
                        If (pTubeType = "BOTTLE2" OrElse pTubeType = "BOTTLE1") Then
                            If (pRingNumber = 1) Then
                                ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCUKNSMALLR1_IconName)
                            Else
                                ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCUKNSMALLR2_IconName)
                            End If

                        ElseIf (pTubeType = "BOTTLE3") Then
                            ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCUKNBIGR2_IconName)
                        End If
                        Exit Select
                End Select
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SetPosControlBackGroundForREAGENTSRotor ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SetPosControlBackGroundForREAGENTSRotor", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Change the Icon to load as ImageBackground in positions of SAMPLES Rotor depending on: Cell Status
    ''' </summary>
    ''' <param name="pPositionControl">Control in which the BackGround or Image has to be changed (PictureBox)</param>
    ''' <param name="pStatus">Current Cell Status</param>
    ''' <param name="pBarCodeStatus">Barcode Status for the Rotor Cell</param>
    ''' <remarks>
    ''' Created by:  SA 18/11/2013 - Code extracted from function SetPosControlBackGroung to divide processing of positions in SAMPLES Rotor from processing of 
    '''                              positions in REAGENTS Rotor.  
    ''' Modified by: SA 04/12/2013 - Include BarCodeStatus = EMPTY in the list of conditions for Tube without Barcode or with a correct one
    ''' </remarks>
    Private Sub SetPosControlBackGroundForSAMPLESRotor(ByVal pPositionControl As BSRImage, ByVal pStatus As String, ByVal pBarCodeStatus As String)
        Try
            'When Tube has not Barcode or it has a correct one, the Background to shown will depend only on Tube Status: 
            '(1) Status = NO_INUSE  => Gray Background
            '(2) Status = DEPLETED  => Red Background
            '(3) Status = PENDING   => Yellow Background 
            '(4) Status = INPROCESS => Orange Background  
            '(5) Status = FINISHED  => Green Background 
            '(6) Status = FEW       => NOT USED FOR SAMPLES
            '(7) Status = BARERROR  => Icon with Red X symbol
            If (pBarCodeStatus = String.Empty OrElse pBarCodeStatus = "OK" OrElse pBarCodeStatus = "EMPTY") Then
                Select Case (pStatus)
                    Case "NO_INUSE"
                        pPositionControl.BackgroundImage = Image.FromFile(MyBase.IconsPath & NOTINUSE_IconName)
                        Exit Select

                    Case "DEPLETED"
                        pPositionControl.BackgroundImage = Image.FromFile(MyBase.IconsPath & DEPLETED_IconName)
                        Exit Select

                    Case "PENDING"
                        pPositionControl.BackgroundImage = Image.FromFile(MyBase.IconsPath & PENDING_IconName)
                        pPositionControl.BackgroundImageLayout = ImageLayout.Stretch
                        Exit Select

                    Case "INPROCESS"
                        pPositionControl.BackgroundImage = Image.FromFile(MyBase.IconsPath & INPROGRESS_IconName)
                        pPositionControl.BackgroundImageLayout = ImageLayout.Stretch

                    Case "FINISHED"
                        pPositionControl.BackgroundImage = Image.FromFile(MyBase.IconsPath & FINISHED_IconName)
                        pPositionControl.BackgroundImageLayout = ImageLayout.Stretch

                    Case "BARERROR"
                        pPositionControl.Image = Image.FromFile(MyBase.IconsPath & BTLSAMPLEBCERR_IconName)
                        pPositionControl.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLEBCERR_IconName)
                        pPositionControl.BackgroundImageLayout = ImageLayout.Stretch
                        Exit Select
                    Case Else
                        pPositionControl.BackgroundImage = Nothing
                End Select
            Else
                'When Tube has a wrong Barcode, the Icon to shown will depend on the BarcodeStatus (only when ERROR; there is not UNKNOWN status for Samples):  
                If (pBarCodeStatus = "ERROR") Then
                    pPositionControl.BackgroundImage = Nothing
                    ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLSAMPLEBCERR_IconName)
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SetPosControlBackGroundForSAMPLESRotor ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SetPosControlBackGroundForSAMPLESRotor", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Show the information related to the selected Cell in Info Area frame (for SAMPLES and REAGENTS Rotors)
    ''' </summary>
    ''' <param name="pRotorType">Rotor Type: SAMPLES or REAGENTS</param>
    ''' <param name="pRingNumber">Ring Number of the selected Rotor Position</param>
    ''' <param name="pCellNumber">Cell Number of the selected Rotor Position</param>
    ''' <remarks>
    ''' Created by: 
    ''' Modified by: TR 15/11/2013 - BA-1383 ==> Send information required to calculate the remaining test for Reagents with 
    '''                                          status NOT INUSE.
    '''              TR 28/03/2014 - BA-1562 ==> Show the expiration date on screen. get the value from the reagent barcode if exist.
    '''              WE 07/10/2014 - BA-1965 ==> Only get Exp.date for Reagents, not for Special Solutions (they don´t have Exp.date in Barcodes).
    '''              SA 09/01/2015 - BA-1999 ==> When position Status is FREE, if BarcodeStatus is UNKNOWN, the Barcode has to be shown in the 
    '''                                          corresponding field in Info Area 
    ''' </remarks>
    Private Sub ShowPositionInfoArea(ByVal pRotorType As String, ByVal pRingNumber As Integer, ByVal pCellNumber As Integer)
        Try
            If (myRotorContentByPositionDSForm.twksWSRotorContentByPosition.Count > 0) Then
                'Verify current Status of the Position 
                Dim currentStatus As String = String.Empty
                Dim barcodeStatus As String = String.Empty

                Dim mySelectedCell As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                mySelectedCell = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition.AsEnumerable _
                                 Where a.RotorType = pRotorType _
                               AndAlso a.RingNumber = pRingNumber _
                               AndAlso a.CellNumber = pCellNumber _
                                Select a).ToList()

                If (mySelectedCell.Count > 0) Then
                    currentStatus = mySelectedCell.First.Status
                    If (Not mySelectedCell.First.IsBarcodeStatusNull) Then barcodeStatus = mySelectedCell.First.BarcodeStatus
                End If

                'If the current Position Status is FREE , it is not needed look additional data in the DataBase
                'BA-1999: exclude from the area cleaning positions with Status FREE but BarcodeStatus = UNKNOWN
                If (currentStatus = "FREE" AndAlso barcodeStatus <> "ERROR" AndAlso barcodeStatus <> "UNKNOWN") Then
                    CleanInfoArea(False)

                    If (pRotorType = "SAMPLES") Then
                        bsSampleCellTextBox.Text = pCellNumber
                        bsSampleCellTextBox.Refresh()
                        bsSampleDiskNameTextBox.Text = pRingNumber
                        bsSampleDiskNameTextBox.Refresh()
                        bsSampleStatusTextBox.Text = String.Empty

                    ElseIf (pRotorType = "REAGENTS") Then
                        bsReagentsCellTextBox.Text = pCellNumber
                        bsReagentsCellTextBox.Refresh()
                        bsReagentsDiskNameTextBox.Text = pRingNumber
                        bsReagentsDiskNameTextBox.Refresh()
                        bsReagentsStatusTextBox.Text = String.Empty
                    End If
                Else
                    'Create a DataSet to fill the known Rotor Position information and create a Row for it
                    Dim currentCellDS As New WSRotorContentByPositionDS
                    Dim currentCellDSRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow

                    'Fill the Row and insert it in the DataSet
                    currentCellDSRow = currentCellDS.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow()
                    currentCellDSRow.AnalyzerID = ActiveAnalyzer
                    currentCellDSRow.RotorType = pRotorType
                    currentCellDSRow.RingNumber = pRingNumber
                    currentCellDSRow.CellNumber = pCellNumber
                    currentCellDSRow.WorkSessionID = ActiveWorkSession

                    'TR 14/11/2013 BT #1383
                    If (mySelectedCell.Count > 0 AndAlso Not mySelectedCell.First().IsReagentIDNull) Then
                        currentCellDSRow.ReagentID = mySelectedCell.First().ReagentID
                    End If

                    If (mySelectedCell.Count > 0 AndAlso Not mySelectedCell.First().IsRealVolumeNull) Then
                        currentCellDSRow.RealVolume = mySelectedCell.First().RealVolume
                    End If
                    'TR 14/11/2013 BT #1383 END.

                    currentCellDS.twksWSRotorContentByPosition.Rows.Add(currentCellDSRow)

                    'Get all the information of the Rotor Position from the Database
                    Dim myGlobalDataTO As GlobalDataTO
                    Dim positionInformation As New WSRotorContentByPositionDelegate

                    myGlobalDataTO = positionInformation.GetPositionInfo(Nothing, currentCellDS)
                    If (Not myGlobalDataTO.HasError) Then
                        'Get the returned information
                        Dim myCellPosInfoDS As CellPositionInformationDS = DirectCast(myGlobalDataTO.SetDatos, CellPositionInformationDS)

                        If (pRotorType = "SAMPLES") Then
                            'Get information from PositionInformation table in the returned DataSet and 
                            'fill Info Area fields for Controls, Calibrators and Patient Samples
                            If (myCellPosInfoDS.PositionInformation.Rows.Count > 0) Then
                                'Fill controls in Info Area Section for positions in Samples Rotor
                                bsSampleCellTextBox.Clear()
                                If (Not myCellPosInfoDS.PositionInformation(0).IsCellNumberNull) Then
                                    bsSampleCellTextBox.Text = myCellPosInfoDS.PositionInformation(0).CellNumber.ToString()
                                End If

                                bsSampleDiskNameTextBox.Clear()
                                If (Not myCellPosInfoDS.PositionInformation(0).IsRingNumberNull) Then
                                    bsSampleDiskNameTextBox.Text = myCellPosInfoDS.PositionInformation(0).RingNumber.ToString()
                                End If

                                bsSampleContentTextBox.Clear()
                                If (Not myCellPosInfoDS.PositionInformation(0).IsContentNull) Then
                                    Dim myPreloadedMDDelegate As New PreloadedMasterDataDelegate

                                    myGlobalDataTO = myPreloadedMDDelegate.GetSubTableItem(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.TUBE_CONTENTS, _
                                                                                           myCellPosInfoDS.PositionInformation(0).Content)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        Dim myPreloadedMasterDS As PreloadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                                        If (myPreloadedMasterDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                            bsSampleContentTextBox.Text = myPreloadedMasterDS.tfmwPreloadedMasterData(0).FixedItemDesc.ToString
                                        End If
                                    End If
                                End If

                                bsSamplesBarcodeTextBox.Clear()
                                If (Not myCellPosInfoDS.PositionInformation(0).IsBarcodeInfoNull) Then
                                    bsSamplesBarcodeTextBox.Text = myCellPosInfoDS.PositionInformation(0).BarcodeInfo
                                End If
                            End If

                            'Get information specific for Samples in the returned DataSet (Samples Table)
                            If (myCellPosInfoDS.Samples.Rows.Count > 0) Then
                                bsSampleNumberTextBox.Clear()
                                If (Not myCellPosInfoDS.Samples(0).IsMultiItemNumberNull) Then
                                    bsSampleNumberTextBox.Text = myCellPosInfoDS.Samples(0).MultiItemNumber.ToString()
                                End If

                                bsSampleIDTextBox.Clear()
                                If (Not myCellPosInfoDS.Samples(0).IsSampleIDNull) Then
                                    bsSampleIDTextBox.Text = myCellPosInfoDS.Samples(0).SampleID
                                End If

                                bsSampleTypeTextBox.Clear()
                                If (Not myCellPosInfoDS.Samples(0).IsSampleTypeNull) Then
                                    bsSampleTypeTextBox.Text = myCellPosInfoDS.Samples(0).SampleType
                                End If

                                bsDiluteStatusTextBox.Clear()
                                If (Not myCellPosInfoDS.Samples(0).IsDilutedNull) Then
                                    If (Not myCellPosInfoDS.Samples(0).IsPredilutionFactorNull AndAlso myCellPosInfoDS.Samples(0).PredilutionFactor > 0) Then
                                        'Set the value of predilution factor
                                        bsDiluteStatusTextBox.Text = "1/" & myCellPosInfoDS.Samples(0).PredilutionFactor
                                    End If
                                End If

                                bsTubeSizeComboBox.SelectedIndex = -1
                                If (Not myCellPosInfoDS.Samples(0).IsTubeTypeNull) Then
                                    'Filter the Tube Sizes by the selected RingNumber
                                    FilterAllTubesbyRingNumberRotorType(pRingNumber)
                                    bsTubeSizeComboBox.SelectedValue = myCellPosInfoDS.Samples(0).TubeType
                                End If
                            Else
                                'Clear not informed controls
                                bsSampleNumberTextBox.Clear()
                                bsSampleIDTextBox.Clear()
                                bsSampleTypeTextBox.Clear()
                                bsDiluteStatusTextBox.Clear()
                                bsTubeSizeComboBox.SelectedIndex = -1
                                bsTubeSizeComboBox.Enabled = False
                            End If

                            'TR 29/04/2011 - Get the value for the status on the corresponding language.
                            bsSampleStatusTextBox.Text = GetStatusDescOnCurrentLanguage(currentStatus, barcodeStatus)

                            'Refresh controls in Info Area for Samples
                            bsSampleCellTextBox.Refresh()
                            bsSampleDiskNameTextBox.Refresh()
                            bsSampleContentTextBox.Refresh()
                            bsSampleNumberTextBox.Refresh()
                            bsSampleIDTextBox.Refresh()
                            bsSampleTypeTextBox.Refresh()
                            bsDiluteStatusTextBox.Refresh()
                            bsSamplesBarcodeTextBox.Refresh()
                            bsTubeSizeComboBox.Refresh()

                        ElseIf String.Equals(pRotorType, "REAGENTS") Then
                            'Get information from PositionInformation table in the returned DataSet and 
                            'fill Info Area fields for Reagents and Additional Solutions
                            If (myCellPosInfoDS.PositionInformation.Rows.Count > 0) Then
                                'Fill controls in Info Area Section for positions in Samples Rotor
                                bsReagentsCellTextBox.Clear()
                                If (Not myCellPosInfoDS.PositionInformation(0).IsCellNumberNull) Then bsReagentsCellTextBox.Text = myCellPosInfoDS.PositionInformation(0).CellNumber.ToString()

                                bsReagentsDiskNameTextBox.Clear()
                                If (Not myCellPosInfoDS.PositionInformation(0).IsRingNumberNull) Then bsReagentsDiskNameTextBox.Text = myCellPosInfoDS.PositionInformation(0).RingNumber.ToString()

                                bsReagentsContentTextBox.Clear()
                                If (Not myCellPosInfoDS.PositionInformation(0).IsContentNull) Then
                                    Dim myPreloadedMDDelegate As New PreloadedMasterDataDelegate

                                    myGlobalDataTO = myPreloadedMDDelegate.GetSubTableItem(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.TUBE_CONTENTS, _
                                                                                           myCellPosInfoDS.PositionInformation(0).Content)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        Dim myPreloadedMasterDS As PreloadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                                        If (myPreloadedMasterDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                            bsReagentsContentTextBox.Text = myPreloadedMasterDS.tfmwPreloadedMasterData(0).FixedItemDesc.ToString
                                        End If
                                    End If
                                End If

                                bsReagentsBarCodeTextBox.Clear()
                                If (Not myCellPosInfoDS.PositionInformation(0).IsBarcodeInfoNull) Then
                                    bsReagentsBarCodeTextBox.Text = myCellPosInfoDS.PositionInformation(0).BarcodeInfo

                                    'TR 28/03/2014 -Set the expiration date 
                                    'If Not myCellPosInfoDS.PositionInformation(0).BarcodeInfo = "" AndAlso _
                                    '              myCellPosInfoDS.PositionInformation(0).BarcodeStatus = "OK" Then

                                    ' WE 07/10/2014 BA-1965 - Only get Exp.date for Reagents, not for Special Solutions (they don´t have Exp.date in Barcodes).
                                    If Not myCellPosInfoDS.PositionInformation(0).BarcodeInfo = "" AndAlso myCellPosInfoDS.PositionInformation(0).BarcodeStatus = "OK" _
                                            AndAlso myCellPosInfoDS.PositionInformation(0).Content = "REAGENT" Then
                                        myCellPosInfoDS.Reagents(0).ExpirationDate = GetReagentExpDateFromBarCode(myCellPosInfoDS.PositionInformation(0).BarcodeInfo)
                                    End If
                                    'TR 28/03/2014 -END.
                                End If
                            End If

                            'Get information specific for Reagents in the returned DataSet (Reagents Table)
                            If (myCellPosInfoDS.Reagents.Rows.Count > 0) Then
                                bsReagentNameTextBox.Clear()
                                If (Not myCellPosInfoDS.Reagents(0).IsReagentNameNull) Then
                                    bsReagentNameTextBox.Text = myCellPosInfoDS.Reagents(0).ReagentName
                                End If

                                bsReagentsNumberTextBox.Clear()
                                If (Not myCellPosInfoDS.Reagents(0).IsMultiItemNumberNull) Then
                                    bsReagentsNumberTextBox.Text = myCellPosInfoDS.Reagents(0).MultiItemNumber.ToString()
                                End If

                                bsTestNameTextBox.Clear()
                                If (Not myCellPosInfoDS.Reagents(0).IsTestListNull) Then
                                    bsTestNameTextBox.Text = myCellPosInfoDS.Reagents(0).TestList
                                End If

                                bsExpirationDateTextBox.Clear()
                                'TR 10/04/2014 bt#1583 -Validate if the expiration date is > than min Date value.
                                If (Not myCellPosInfoDS.Reagents(0).IsExpirationDateNull AndAlso _
                                    myCellPosInfoDS.Reagents(0).ExpirationDate > Date.MinValue) Then
                                    'TR 28/03/2014 -Set the expiration date in the correct format Month (MM) And Year (yyyy)
                                    bsExpirationDateTextBox.Text = myCellPosInfoDS.Reagents(0).ExpirationDate.ToString("MM" & SystemInfoManager.OSDateSeparator & "yyyy")
                                    'TR 28/03/2014 -END
                                End If

                                bsBottleSizeComboBox.SelectedIndex = -1
                                If (Not myCellPosInfoDS.Reagents(0).IsBottleCodeNull) Then
                                    FilterAllTubesbyRingNumberRotorType(pRingNumber)
                                    bsBottleSizeComboBox.SelectedValue = myCellPosInfoDS.Reagents(0).BottleCode
                                End If

                                bsCurrentVolTextBox.Clear()
                                If (Not myCellPosInfoDS.Reagents(0).IsRealVolumeNull) Then
                                    'AG 02/01/2011 - Show Real Volume with 2 decimals
                                    bsCurrentVolTextBox.Text = myCellPosInfoDS.Reagents(0).RealVolume.ToStringWithDecimals(2, True)
                                End If

                                bsTeststLeftTextBox.Clear()
                                If (Not myCellPosInfoDS.Reagents(0).IsRemainingTestsNull) Then
                                    bsTeststLeftTextBox.Text = myCellPosInfoDS.Reagents(0).RemainingTests.ToString()
                                End If
                            Else
                                'Clear not informed controls
                                bsReagentNameTextBox.Clear()
                                bsReagentsNumberTextBox.Clear()
                                bsTestNameTextBox.Clear()
                                bsExpirationDateTextBox.Clear()
                                bsBottleSizeComboBox.SelectedIndex = -1
                                bsCurrentVolTextBox.Clear()
                                bsTeststLeftTextBox.Clear()
                                bsBottleSizeComboBox.Enabled = False
                            End If

                            'TR 29/04/2011 - Get the value for the status on the corresponding language.
                            bsReagentsStatusTextBox.Text = GetStatusDescOnCurrentLanguage(currentStatus, barcodeStatus)

                            'Refresh controls in Info Area for Samples
                            bsReagentsCellTextBox.Refresh()
                            bsReagentsDiskNameTextBox.Refresh()
                            bsReagentsContentTextBox.Refresh()
                            bsReagentsNumberTextBox.Refresh()
                            bsReagentNameTextBox.Refresh()
                            bsTestNameTextBox.Refresh()
                            bsReagentsBarCodeTextBox.Refresh()
                            bsExpirationDateTextBox.Refresh()
                            bsCurrentVolTextBox.Refresh()
                            bsTeststLeftTextBox.Refresh()
                            bsBottleSizeComboBox.Refresh()
                        End If
                    Else
                        ShowMessage(Me.Name & ".ShowPositionInfoArea", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, MsgParent)
                    End If
                End If
                mySelectedCell = Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ShowPositionInfoArea", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ShowPositionInfoArea", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub


    ''' <summary>
    ''' Get the Expiration date from the reagent barcode information.
    ''' </summary>
    ''' <param name="pReagentBarcode"></param>
    ''' <returns>Valid datepart in pReagentBarcode ==> Expiration Date.
    '''          Datepart in pReagentBarcode represents invalid date ==> Date.MinValue</returns>
    ''' <remarks>
    ''' Created by:  TR 28/03/2014
    ''' Modified by: TR 10/04/2014 bt #1583-Initialize the ExpirationDate variable to min Date value.
    '''              XB 10/07/2014 - DateTime to Invariant Format (MM dd yyyy) - Bug #1673
    '''              WE 07/10/2014 - Extend code with check on Month field to prevent String to Date Conversion Error shown on screen (BA-1965).
    ''' </remarks>
    Private Function GetReagentExpDateFromBarCode(pReagentBarcode As String) As Date
        Dim ExpirationDate As Date = Date.MinValue
        Try
            Dim myMonth As String = ""
            Dim myYear As String = ""
            If pReagentBarcode <> "" Then
                'The month start on position 6 to 7 (2pos)
                myMonth = pReagentBarcode.Substring(5, 2)
                'The year start on position 8 to 9 (2pos)
                myYear = pReagentBarcode.Substring(7, 2)
                'Add to year expiration the 2000 to avoid error of 1900
                myYear = "20" & myYear

                'Set the result value.
                'If myMonth <> "" OrElse myYear <> "" Then
                If myMonth <> "" AndAlso myYear <> "" AndAlso CInt(myMonth) >= 1 AndAlso CInt(myMonth) <= 12 Then
                    ' XB 10/07/2014 - DateTime to Invariant Format - Bug #1673
                    'Date.TryParse("01" & "-" & myMonth & "-" & myYear, ExpirationDate)
                    ExpirationDate = New DateTime(CInt(myYear), CInt(myMonth), 1)
                    'This is wrong:
                    'ExpirationDate = CDate(myMonth & "-" & "01" & "-" & myYear).ToString(CultureInfo.InvariantCulture)
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetReagentExpDateFromBarCode", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetReagentExpDateFromBarCode", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return ExpirationDate
    End Function


    ''' <summary>
    ''' Get the information that has to be shown in the Info Area section for the selected REACTIONS Rotor Position
    ''' </summary>
    ''' <param name="pAnalyzerID">Analyzer Identifier</param>
    ''' <param name="pRingNumber">Ring Number of the selected Rotor Position</param>
    ''' <param name="pCellNumber">Cell Number of the selected Rotor Position</param>
    ''' <remarks>
    ''' Created by:  DL 02/06/2011
    ''' Modified by: RH 19/04/2012 - BT #499
    '''              JV 27/01/2014 - BT #1310
    ''' </remarks>
    Private Sub ShowPositionInfoReactionsArea(ByVal pAnalyzerID As String, ByVal pRingNumber As Integer, ByVal pCellNumber As Integer)
        Try
            If (myRotorContentByPositionDSForm.twksWSRotorContentByPosition.Count > 0) Then
                Dim ShowGraph As Boolean = True 'RH 19/04/2012

                'Verify current Status of the Position 
                Dim currentStatus As String = String.Empty
                Dim mySelectedCell As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                mySelectedCell = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                                 Where a.RotorType = "REACTIONS" _
                               AndAlso a.RingNumber = pRingNumber _
                               AndAlso a.CellNumber = pCellNumber _
                                Select a).ToList()

                If (mySelectedCell.Count > 0) Then
                    Dim cell As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow
                    cell = mySelectedCell.First()
                    currentStatus = cell.Status
                    ShowGraph = Not (cell.ElementStatus = "P" And cell.TubeContent = "P")
                End If

                'If the current Position Status is FREE, it is not needed look additional data in the DataBase
                If (currentStatus = "FREE") Then
                    CleanInfoArea(False)

                    bsWellNrTextBox.Text = pCellNumber
                    bsWellNrTextBox.Refresh()

                    bsReacStatusTextBox.Text = String.Empty
                Else
                    'Create a DataSet to fill the known Rotor Position information and create a Row for it
                    Dim currentCellDS As New WSRotorContentByPositionDS
                    Dim currentCellDSRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow

                    'Fill the Row and insert it in the DataSet
                    currentCellDSRow = currentCellDS.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow()
                    currentCellDSRow.AnalyzerID = ActiveAnalyzer
                    currentCellDSRow.RotorType = "REACTIONS"
                    currentCellDSRow.RingNumber = pRingNumber
                    currentCellDSRow.CellNumber = pCellNumber
                    currentCellDSRow.WorkSessionID = ActiveWorkSession
                    currentCellDS.twksWSRotorContentByPosition.Rows.Add(currentCellDSRow)

                    bsReactionsOpenGraph.Enabled = False

                    'Get all the information of the Rotor Position from the Database
                    Dim myGlobalDataTO As GlobalDataTO
                    Dim positionInformation As New ReactionsRotorDelegate

                    myGlobalDataTO = positionInformation.GetPositionReactionsInfo(Nothing, currentCellDS)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        Dim myReactionRotorDetails As ReactionRotorDetailsDS = DirectCast(myGlobalDataTO.SetDatos, ReactionRotorDetailsDS)

                        If (myReactionRotorDetails.ReactionsRotorDetails.Rows.Count > 0) Then
                            bsWellNrTextBox.Text = pCellNumber
                            bsWellNrTextBox.Refresh()

                            Dim mySampleClass As String = ""
                            bsSampleClassTextBox.Clear()
                            If (Not myReactionRotorDetails.ReactionsRotorDetails.First.IsSampleClassNull) Then
                                mySampleClass = myReactionRotorDetails.ReactionsRotorDetails.First.SampleClass

                                'Search the multilanguage description for the SampleClass
                                Dim myPreloadedMDDelegate As New PreloadedMasterDataDelegate
                                myGlobalDataTO = myPreloadedMDDelegate.GetSubTableItem(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.SAMPLE_CLASSES, mySampleClass)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    Dim myPreloadedMasterDS As PreloadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                                    If (myPreloadedMasterDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                        bsSampleClassTextBox.Text = myPreloadedMasterDS.tfmwPreloadedMasterData(0).FixedItemDesc.ToString
                                    End If
                                End If
                            End If

                            bsPatientIDTextBox.Clear()
                            If (Not myReactionRotorDetails.ReactionsRotorDetails.First.IsSampleIDNull) Then
                                bsPatientIDTextBox.Text = myReactionRotorDetails.ReactionsRotorDetails.First.SampleID
                            End If

                            bsReacSampleTypeTextBox.Clear()
                            If (Not myReactionRotorDetails.ReactionsRotorDetails.First.IsSampleTypeNull) Then
                                bsReacSampleTypeTextBox.Text = myReactionRotorDetails.ReactionsRotorDetails.First.SampleType
                            End If

                            bsReacTestTextBox.Clear()
                            If (Not myReactionRotorDetails.ReactionsRotorDetails.First.IsTestNameNull) Then
                                bsReacTestTextBox.Text = myReactionRotorDetails.ReactionsRotorDetails.First.TestName
                                'bsReactionsOpenGraph.Enabled = ShowGraph
                                bsReactionsOpenGraph.Enabled = ShowGraph And myReactionRotorDetails.ReactionsRotorDetails.First.ExecutionStatus <> "LOCKED" 'JV 27/01/2014 #1310
                            End If

                            BsExecutionIDTextBox.Clear()
                            If (Not myReactionRotorDetails.ReactionsRotorDetails.First.IsExecutionIDNull) Then
                                BsExecutionIDTextBox.Text = myReactionRotorDetails.ReactionsRotorDetails.First.ExecutionID
                            End If

                            bsCalibNrTextBox.Clear()
                            If (mySampleClass = "CALIB" AndAlso Not myReactionRotorDetails.ReactionsRotorDetails.First.IsMultiItemNumberNull) Then
                                bsCalibNrTextBox.Text = myReactionRotorDetails.ReactionsRotorDetails.First.MultiItemNumber
                            End If

                            bsOrderTestIDTextBox.Clear()
                            If (Not myReactionRotorDetails.ReactionsRotorDetails.First.IsOrderTestIDNull) Then
                                bsOrderTestIDTextBox.Text = myReactionRotorDetails.ReactionsRotorDetails.First.OrderTestID
                            End If

                            bsDilutionTextBox.Clear()
                            If (mySampleClass = "PATIENT" AndAlso Not myReactionRotorDetails.ReactionsRotorDetails.First.IsDilutionFactorNull) Then
                                bsDilutionTextBox.Text = "1/" & myReactionRotorDetails.ReactionsRotorDetails.First.DilutionFactor
                            End If

                            bsReplicateTextBox.Clear()
                            If (Not myReactionRotorDetails.ReactionsRotorDetails.First.IsReplicateNumberNull) Then
                                bsReplicateTextBox.Text = myReactionRotorDetails.ReactionsRotorDetails.First.ReplicateNumber
                            End If

                            bsRerunTextBox.Clear()
                            If (Not myReactionRotorDetails.ReactionsRotorDetails.First.IsRerunNumberNull) Then
                                bsRerunTextBox.Text = myReactionRotorDetails.ReactionsRotorDetails.First.RerunNumber
                            End If

                            bsReacStatusTextBox.Clear()
                            If (Not myReactionRotorDetails.ReactionsRotorDetails.First.IsExecutionStatusNull) Then
                                bsReacStatusTextBox.Text = GetStatusDescOnCurrentLanguage(myReactionRotorDetails.ReactionsRotorDetails.First.ExecutionStatus)
                            End If
                        Else
                            CleanInfoArea(False)

                            bsWellNrTextBox.Text = pCellNumber
                            bsWellNrTextBox.Refresh()
                        End If
                    Else
                        ShowMessage(Me.Name & ".ShowPositionInfoArea", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, MsgParent)
                    End If
                End If
                mySelectedCell = Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ShowPositionInfoArea", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ShowPositionInfoArea", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    Private Sub StatusInfoAreaControls(ByVal pRotorType As String, ByVal pActivateControls As Boolean)
        Try
            Dim backColor As System.Drawing.Color = Color.Gainsboro
            If (Not pActivateControls) Then backColor = SystemColors.MenuBar

            If (pRotorType = "SAMPLES") Then
                bsSampleCellTextBox.Enabled = pActivateControls
                bsSampleCellTextBox.BackColor = backColor
                bsSampleDiskNameTextBox.Enabled = pActivateControls
                bsSampleDiskNameTextBox.BackColor = backColor
                bsSampleContentTextBox.Enabled = pActivateControls
                bsSampleContentTextBox.BackColor = backColor
                bsSampleNumberTextBox.Enabled = pActivateControls
                bsSampleNumberTextBox.BackColor = backColor
                bsSampleIDTextBox.Enabled = pActivateControls
                bsSampleIDTextBox.BackColor = backColor
                bsSampleTypeTextBox.Enabled = pActivateControls
                bsSampleTypeTextBox.BackColor = backColor
                bsDiluteStatusTextBox.Enabled = pActivateControls
                bsDiluteStatusTextBox.BackColor = backColor
                bsSamplesBarcodeTextBox.BackColor = backColor
                bsSamplesBarcodeTextBox.Enabled = pActivateControls
                bsSampleStatusTextBox.BackColor = backColor
                bsSampleStatusTextBox.Enabled = pActivateControls

                If (pActivateControls) Then
                    bsTubeSizeComboBox.Enabled = False
                Else
                    bsTubeSizeComboBox.Enabled = pActivateControls
                End If

            ElseIf (pRotorType = "REAGENTS") Then
                bsReagentsCellTextBox.Enabled = pActivateControls
                bsReagentsCellTextBox.BackColor = backColor
                bsReagentsDiskNameTextBox.Enabled = pActivateControls
                bsReagentsDiskNameTextBox.BackColor = backColor
                bsReagentsContentTextBox.Enabled = pActivateControls
                bsReagentsContentTextBox.BackColor = backColor
                bsReagentsNumberTextBox.Enabled = pActivateControls
                bsReagentsNumberTextBox.BackColor = backColor
                bsReagentNameTextBox.Enabled = pActivateControls
                bsReagentNameTextBox.BackColor = backColor
                bsTestNameTextBox.Enabled = pActivateControls
                bsTestNameTextBox.BackColor = backColor
                bsExpirationDateTextBox.Enabled = pActivateControls
                bsExpirationDateTextBox.BackColor = backColor
                bsCurrentVolTextBox.Enabled = pActivateControls
                bsCurrentVolTextBox.BackColor = backColor
                bsTeststLeftTextBox.Enabled = pActivateControls
                bsTeststLeftTextBox.BackColor = backColor
                bsBottleSizeComboBox.Enabled = pActivateControls
                bsReagentsStatusTextBox.BackColor = backColor
                bsReagentsStatusTextBox.Enabled = pActivateControls
                bsReagentsBarCodeTextBox.BackColor = backColor
                bsReagentsBarCodeTextBox.Enabled = pActivateControls

            ElseIf (pRotorType = "REACTIONS") Then
                bsWellNrTextBox.Enabled = pActivateControls
                bsWellNrTextBox.BackColor = backColor
                bsSampleClassTextBox.Enabled = pActivateControls
                bsSampleClassTextBox.BackColor = backColor
                bsCalibNrTextBox.Enabled = pActivateControls
                bsCalibNrTextBox.BackColor = backColor
                bsPatientIDTextBox.Enabled = pActivateControls
                bsPatientIDTextBox.BackColor = backColor
                bsReacSampleTypeTextBox.Enabled = pActivateControls
                bsReacSampleTypeTextBox.BackColor = backColor
                bsDilutionTextBox.Enabled = pActivateControls
                bsDilutionTextBox.BackColor = backColor
                bsReplicateTextBox.Enabled = pActivateControls
                bsReplicateTextBox.BackColor = backColor
                bsRerunTextBox.Enabled = pActivateControls
                bsRerunTextBox.BackColor = backColor
                bsReacTestTextBox.Enabled = pActivateControls
                bsReacTestTextBox.BackColor = backColor
                bsReacStatusTextBox.Enabled = pActivateControls
                bsReacStatusTextBox.BackColor = backColor
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".StatusInfoAreaControls", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".StatusInfoAreaControls", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary></summary>
    ''' <remarks>
    ''' Created by:  TR
    ''' Modified by: SA 18/11/2013 - BT #1359 ==> Changed call to function GetIconNameByTubeContent: new parameter InProcessElement (obtained from the current
    '''                                           row in WSRotorContentByPositionDS) is informed
    ''' AG 25/11/2013 - #1359 - Inform parameter InProcessElement on call method SetPosControlBackGround
    ''' IT 04/06/2014 - #1644 - Modified the type of last parameter.
    ''' </remarks>
    Private Sub UpdateRotorArea(ByVal pRotorContenByPosRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow, _
                                ByVal pIconName As String, ByVal pRotorControl As Control)
        Try

            If (pRotorControl Is Nothing) Then Exit Sub

            Dim rotorControlCollection As Control.ControlCollection
            rotorControlCollection = pRotorControl.Controls

            'ChangeIcon name for the Reagents and Additional solutions
            Dim myIconName As String = pIconName
            If (myIconName = REAGENTS_IconName OrElse myIconName = ADDSOL_IconName) Then
                myIconName = GetIconNameByTubeContent(pRotorContenByPosRow.TubeContent, _
                                                      pRotorContenByPosRow.TubeType, _
                                                      pRotorContenByPosRow.RingNumber, _
                                                      pRotorContenByPosRow.InProcessElement)
            End If

            Dim myControlName As String = String.Empty
            Dim myControls As Control.ControlCollection
            If (Not rotorControlCollection Is Nothing) Then
                myControls = rotorControlCollection

                If (pRotorContenByPosRow.RotorType = "SAMPLES") Then
                    myControlName = String.Format("Sam{0}{1}", pRotorContenByPosRow.RingNumber, pRotorContenByPosRow.CellNumber)
                ElseIf (pRotorContenByPosRow.RotorType = "REAGENTS") Then
                    myControlName = String.Format("Reag{0}{1}", pRotorContenByPosRow.RingNumber, pRotorContenByPosRow.CellNumber)
                ElseIf (pRotorContenByPosRow.RotorType = "REACTIONS") Then
                    myControlName = "Reac" & pRotorContenByPosRow.CellNumber
                End If

                Dim lstRotorControl As List(Of Control) = (From a As Control In myControls _
                                                          Where a.Name = myControlName _
                                                         Select a).ToList()
                If (lstRotorControl.Count = 1) Then
                    Dim myBSRImage As BSRImage = CType(lstRotorControl(0), BSRImage)

                    If (myIconName <> String.Empty) Then
                        Me.ChangeControlPositionImage(myBSRImage, MyBase.IconsPath & myIconName)
                        SetPosControlBackGround(myBSRImage, pRotorContenByPosRow.Status, pRotorContenByPosRow.RotorType, pRotorContenByPosRow.TubeType, _
                                                pRotorContenByPosRow.RingNumber, pRotorContenByPosRow.BarcodeStatus)
                    Else
                        If (pRotorContenByPosRow.RotorType = "SAMPLES") Then
                            Me.ChangeControlPositionImage(myBSRImage, Nothing, True)

                        ElseIf (pRotorContenByPosRow.RotorType = "REAGENTS") Then
                            If (pRotorContenByPosRow.BarcodeStatus = "ERROR") Then
                                'TR 08/09/2011 - Validate if status is free to set by default the bottle depending the ring number
                                'if Rin 1 20 ml if ring 2 then 60 ml.
                                If (pRotorContenByPosRow.Status = "FREE") Then
                                    If (pRotorContenByPosRow.RingNumber = 1) Then
                                        ChangeControlPositionImage(myBSRImage, MyBase.IconsPath & BTLBCERRSMALLR1_IconName)
                                    Else
                                        ChangeControlPositionImage(myBSRImage, MyBase.IconsPath & BTLBCERRSMALLR2_IconName)
                                    End If
                                Else
                                    If (pRotorContenByPosRow.TubeType = "BOTTLE2" OrElse pRotorContenByPosRow.TubeType = "BOTTLE1") Then
                                        If (pRotorContenByPosRow.RingNumber = 1) Then
                                            ChangeControlPositionImage(myBSRImage, MyBase.IconsPath & BTLBCERRSMALLR1_IconName)
                                        Else
                                            ChangeControlPositionImage(myBSRImage, MyBase.IconsPath & BTLBCERRSMALLR2_IconName)
                                        End If

                                    ElseIf (pRotorContenByPosRow.TubeType = "BOTTLE3") Then
                                        ChangeControlPositionImage(myBSRImage, MyBase.IconsPath & BTLBCERRBIGR2_IconName)
                                    End If
                                End If

                            ElseIf (pRotorContenByPosRow.BarcodeStatus = "UNKNOWN") Then
                                If (pRotorContenByPosRow.TubeType = "BOTTLE2" OrElse pRotorContenByPosRow.TubeType = "BOTTLE1") Then
                                    If (pRotorContenByPosRow.RingNumber = 1) Then
                                        ChangeControlPositionImage(myBSRImage, MyBase.IconsPath & BTLBCUKNSMALLR1_IconName)
                                    Else
                                        ChangeControlPositionImage(myBSRImage, MyBase.IconsPath & BTLBCUKNSMALLR2_IconName)
                                    End If

                                ElseIf (pRotorContenByPosRow.TubeType = "BOTTLE3") Then
                                    ChangeControlPositionImage(myBSRImage, MyBase.IconsPath & BTLBCUKNBIGR2_IconName)
                                End If
                            End If
                        End If
                    End If

                    'AG 25/11/2013 - #1359 Inform the field InProcessElement
                    SetPosControlBackGround(myBSRImage, pRotorContenByPosRow.Status, pRotorContenByPosRow.RotorType, pRotorContenByPosRow.TubeType, _
                                            pRotorContenByPosRow.RingNumber, pRotorContenByPosRow.BarcodeStatus, pRotorContenByPosRow.InProcessElement)

                End If
                Application.DoEvents()

                myControls = Nothing
                lstRotorControl = Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdateRotorArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UpdateRotorArea", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    Private Function UpdateRotorContentByPositionDSForm(ByVal pRotorContentByPosRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow) As Boolean
        Dim result As Boolean = False
        Try
            'Implement LINQ to get the position to update 
            Dim query As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
            query = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                    Where a.RotorType = pRotorContentByPosRow.RotorType _
                  AndAlso a.RingNumber = pRotorContentByPosRow.RingNumber _
                  AndAlso a.CellNumber = pRotorContentByPosRow.CellNumber _
                   Select a).ToList()

            For Each myrow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In query
                myrow.ItemArray = pRotorContentByPosRow.ItemArray.Clone()
            Next
            result = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdateRotorContentByPositionDSForm ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UpdateRotorContentByPositionDSForm", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
            result = False
        End Try
        Return result
    End Function

    ''' <summary></summary>
    ''' <remarks>
    ''' Created by:
    ''' Modified by: RH 17/06/2011 - Add TUBE_SPEC_SOL and TUBE_WASH_SOL
    '''              AG 02/01/2012 - For SAMPLES Rotor: do not use the ElementStatus "POS" condition; there are other status, for example: NOPOS (tube depleted)
    '''                              For REAGENTS Rotor: do not use the ElementStatus "POS" condition; there are other status, for example: INCOMPLETE (few volume) or NOPOS (bottle depleted)
    '''                              ('Simulate receiving instructions: A400;ANSBR1;ID:2;W:58;S:R1;P:1;L:5007; (inuse) || A400;ANSBR1;ID:2;W:58;S:R1;P:1;L:507; (few) || A400;ANSBR1;ID:2;W:58;S:R1;P:1;L:57; (depleted))
    '''                              Remove the ElementStatus condition, the Free position TubeContent is not REAGENT
    '''              SA 09/02/2012 - When calling function GetRequiredPatientSamplesDetails, inform optional parameter pGetAllNoPos=TRUE to get 
    '''                              also Patient Samples with Status NOPOS but positioned in Rotor but in a tube marked as DEPLETED or FEW 
    '''              SA 20/11/2013 - BT #1359 ==> Changed call to function GetIconNameByTubeContent: new parameter InProcessElement (obtained from the current
    '''                                           row in WSRotorContentByPositionDS) is informed 
    '''              IT 04/06/2014 - BT #1644 ==> Changed call to function UpdateRotorArea: new type of the last parameter
    ''' </remarks>
    Private Sub UpdateRotorTreeViewArea(ByVal pWSRotorContentByPositionDS As WSRotorContentByPositionDS, Optional ByVal pRotorType As String = "")
        Try
            If (IsDisposed) Then Return 'IT 03/06/2014 - #1644 No refresh if screen is disposed

            Dim myGlobalDataTO As GlobalDataTO
            Dim myNotInUseRPDelegate As New WSNotInUseRotorPositionsDelegate()

            'Get positions in the specified Rotor Type
            If (String.IsNullOrEmpty(pRotorType)) Then pRotorType = myRotorTypeForm

            'RH 26/04/2011 - Just in case RotorType is not SAMPLES NOR REAGENTS NOR REACTIONS
            If (pRotorType <> "SAMPLES" AndAlso pRotorType <> "REAGENTS" AndAlso pRotorType <> "REACTIONS") Then Return

            Dim lstPosInActiveRotor = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In pWSRotorContentByPositionDS.twksWSRotorContentByPosition _
                                      Where a.RotorType = pRotorType _
                                     Select a).ToList()

            If (pWSRotorContentByPositionDS.twksWSRotorContentByPosition.Rows.Count > 0) Then
                Dim myRotorPicture As Object
                Dim auxIconPath As String = String.Empty
                Dim myVirtualRotorPosititionsDS As VirtualRotorPosititionsDS
                Dim myWSRequiredElementsDelegate As New WSRequiredElementsDelegate

                'Go through each updated Rotor Position
                For Each rotorPosition As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In lstPosInActiveRotor
                    If (UpdateRotorContentByPositionDSForm(rotorPosition)) Then
                        'Validate if there is a Required Element in the Rotor Position
                        If (Not rotorPosition.IsElementIDNull) Then
                            If (rotorPosition.TubeContent = "CALIB") Then
                                'Set the Calibrator Icon in the Rotor Position
                                UpdateRotorArea(rotorPosition, CALIB_IconName, SamplesTab)

                            ElseIf (rotorPosition.TubeContent = "TUBE_SPEC_SOL" OrElse rotorPosition.TubeContent = "TUBE_WASH_SOL") Then
                                'Set the Tube Additional Solution Icon in the Rotor Position
                                UpdateRotorArea(rotorPosition, ADDSAMPLESOL_IconName, SamplesTab)

                            ElseIf (rotorPosition.TubeContent = "PATIENT") Then
                                'Validate if the routine is Urgent
                                'JV 18/12/2013 #1056 INI - Never appears the red bitmap urgent patient
                                'myGlobalDataTO = myWSRequiredElementsDelegate.GetRequiredPatientSamplesDetails(Nothing, rotorPosition.WorkSessionID, rotorPosition.ElementID, _
                                '                                                                               rotorPosition.ElementStatus, True)
                                'If (Not myGlobalDataTO.HasError) Then
                                '    If (DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsTreeDS).PatientSamples.Count > 0) Then
                                '        If (Not DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsTreeDS).PatientSamples(0).IsPredilutionFactorNull) AndAlso _
                                '           (DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsTreeDS).PatientSamples(0).PredilutionFactor > 0) Then
                                '            UpdateRotorArea(rotorPosition, DILUTIONS_IconName, SamplesTab.Controls)
                                '        Else
                                '            If (DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsTreeDS).PatientSamples(0).StatFlag) Then
                                '                UpdateRotorArea(rotorPosition, STATS_IconName, SamplesTab.Controls)
                                '            Else
                                '                UpdateRotorArea(rotorPosition, ROUTINES_IconName, SamplesTab.Controls)
                                '            End If
                                '        End If
                                '    End If
                                'End If
                                myGlobalDataTO = myWSRequiredElementsDelegate.GetRequiredElementData(Nothing, rotorPosition.ElementID)
                                If (Not myGlobalDataTO.HasError) Then
                                    If (DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsDS).twksWSRequiredElements.Rows.Count > 0) Then
                                        If (Not DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsDS).twksWSRequiredElements(0).IsPredilutionFactorNull) AndAlso _
                                           (DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsDS).twksWSRequiredElements(0).PredilutionFactor > 0) Then
                                            UpdateRotorArea(rotorPosition, DILUTIONS_IconName, SamplesTab)
                                        Else
                                            Dim myWSRequiredElementsByOTDelegate As New WSRequiredElemByOrderTestDelegate
                                            myGlobalDataTO = myWSRequiredElementsByOTDelegate.ReadOrderTestByElementIDAndSampleClass(Nothing, rotorPosition.ElementID, rotorPosition.TubeContent)
                                            If (Not myGlobalDataTO.HasError) Then
                                                Dim myDS As WSRequiredElemByOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElemByOrderTestDS)
                                                Dim stat As Integer = (From x As WSRequiredElemByOrderTestDS.twksWSRequiredElemByOrderTestRow In myDS.twksWSRequiredElemByOrderTest _
                                                                      Where x.StatFlag = True Select x).Count
                                                If (stat > 0) Then
                                                    UpdateRotorArea(rotorPosition, STATS_IconName, SamplesTab)
                                                Else
                                                    UpdateRotorArea(rotorPosition, ROUTINES_IconName, SamplesTab)
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                                'JV 18/12/2013 #1056 END
                            ElseIf String.Equals(rotorPosition.TubeContent, "CTRL") Then
                                'Set the Calibrator Icon in the Rotor Position
                                UpdateRotorArea(rotorPosition, CTRL_IconName, SamplesTab)

                            ElseIf String.Equals(rotorPosition.TubeContent, "REAGENT") Then
                                UpdateRotorArea(rotorPosition, REAGENTS_IconName, ReagentsTab)

                            ElseIf (String.Equals(rotorPosition.TubeContent, "WASH_SOL") OrElse String.Equals(rotorPosition.TubeContent, "SPEC_SOL")) Then
                                UpdateRotorArea(rotorPosition, ADDSOL_IconName, ReagentsTab)
                            End If

                            If (rotorPosition.Selected) Then
                                'Reload the info area with the new information 
                                mySelectedElementInfo = GetLocalPositionInfo(rotorPosition.RingNumber, rotorPosition.CellNumber, False)
                                ShowPositionInfoArea(myRotorTypeForm, rotorPosition.RingNumber, rotorPosition.CellNumber)
                            End If
                        Else
                            auxIconPath = ""
                            myRotorPicture = Nothing

                            If (pRotorType = "SAMPLES") Then
                                myRotorPicture = Me.SamplesTab
                            ElseIf (pRotorType = "REAGENTS") Then
                                myRotorPicture = Me.ReagentsTab
                            ElseIf (pRotorType = "REACTIONS") Then
                                myRotorPicture = Me.ReactionsTab
                            End If

                            If (pRotorType = "SAMPLES") OrElse (pRotorType = "REAGENTS") Then
                                'If Rotor Position is not FREE but it does not contain a pòsitioned Element, then validate the tube content
                                If (Not rotorPosition.IsTubeContentNull AndAlso Not rotorPosition.TubeContent = String.Empty) Then
                                    'TR 26/01/2010 - If the TubeContent is Patient it is needed to validate if it's a dilution
                                    If (rotorPosition.TubeContent = "PATIENT") Then
                                        'Get the information for Not in use elements
                                        myGlobalDataTO = myNotInUseRPDelegate.GetPositionContent(Nothing, ActiveAnalyzer, rotorPosition.RotorType, _
                                                                                                 rotorPosition.RingNumber, rotorPosition.CellNumber, ActiveWorkSession)
                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            myVirtualRotorPosititionsDS = DirectCast(myGlobalDataTO.SetDatos, VirtualRotorPosititionsDS)

                                            If (myVirtualRotorPosititionsDS.tparVirtualRotorPosititions.Rows.Count > 0) Then
                                                'Validate if there is a Predilution Sample
                                                If (Not myVirtualRotorPosititionsDS.tparVirtualRotorPosititions(0).IsPredilutionFactorNull AndAlso _
                                                    myVirtualRotorPosititionsDS.tparVirtualRotorPosititions(0).PredilutionFactor > 0) Then
                                                    auxIconPath = DILUTIONS_IconName
                                                Else
                                                    '...then it's a full Patient Sample
                                                    auxIconPath = ROUTINES_IconName
                                                End If
                                            End If
                                        Else
                                            'Error getting the content of the Rotor Position, show it
                                            ShowMessage(Me.Name, myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, MsgParent)
                                        End If
                                    Else
                                        'Get the needed Icon when the TubeContent is not a Patient Sample
                                        auxIconPath = GetIconNameByTubeContent(rotorPosition.TubeContent, rotorPosition.TubeType, rotorPosition.RingNumber, _
                                                                               rotorPosition.InProcessElement)
                                    End If

                                    'Update the rotor area with the new icon path          
                                    UpdateRotorArea(rotorPosition, auxIconPath, myRotorPicture)
                                Else
                                    'Position is empty...icon is also empty, clean the cell
                                    UpdateRotorArea(rotorPosition, auxIconPath, myRotorPicture)
                                End If

                                If (rotorPosition.Selected) Then
                                    'Reload the info area with the new information 
                                    mySelectedElementInfo = GetLocalPositionInfo(rotorPosition.RingNumber, rotorPosition.CellNumber, False)
                                    ShowPositionInfoArea(myRotorTypeForm, rotorPosition.RingNumber, rotorPosition.CellNumber)
                                End If
                            Else
                                'Process for REACTIONS Rotor
                                If (rotorPosition.Selected) Then
                                    Select Case (rotorPosition.TubeContent)
                                        Case "W"   'Washing
                                            PreviousSelect = "REACPOSWASH"
                                            auxIconPath = REACPOSWASHS_IconName

                                        Case "E"   'Not in use
                                            If (rotorPosition.ElementStatus = "R") Then
                                                PreviousSelect = "REACPOSNIUS"
                                                auxIconPath = REACPOSNIUSS_IconName
                                            ElseIf (rotorPosition.ElementStatus = "X") Then
                                                PreviousSelect = "REACPOSOPTI"
                                                auxIconPath = REACPOSOPTIS_IconName
                                            End If

                                        Case "T"   'R1 + Sample + R2
                                            If (rotorPosition.ElementStatus = "R1") Then
                                                PreviousSelect = "REACPOSR1"
                                                auxIconPath = REACPOSR1S_IconName
                                            ElseIf (rotorPosition.ElementStatus = "S") Then
                                                PreviousSelect = "REACPOSR1SL"
                                                auxIconPath = REACPOSR1SLS_IconName
                                            ElseIf (rotorPosition.ElementStatus = "R2") Then
                                                PreviousSelect = "REACPOSR1R2"
                                                auxIconPath = REACPOSR1R2S_IconName
                                            ElseIf (rotorPosition.ElementStatus = "F") Then
                                                PreviousSelect = "REACPOSFINI"
                                                auxIconPath = REACPOSFINIS_IconName
                                            End If

                                        Case "C"  'Contaminated
                                            PreviousSelect = "REACPOSCONT"
                                            auxIconPath = REACPOSCONTS_IconName

                                        Case "P" 'Dilution
                                            PreviousSelect = "REACPOSDIL"
                                            auxIconPath = REACPOSDILS_IconName
                                    End Select
                                Else
                                    Select Case (rotorPosition.TubeContent)
                                        Case "W"   'Washing
                                            auxIconPath = REACPOSWASH_IconName

                                        Case "E"   'Not in use
                                            If (rotorPosition.ElementStatus = "R") Then
                                                auxIconPath = REACPOSNIUS_IconName
                                            ElseIf (rotorPosition.ElementStatus = "X") Then
                                                auxIconPath = REACPOSOPTI_IconName
                                            End If

                                        Case "T"   'R1 + Sample + R2
                                            If (rotorPosition.ElementStatus = "R1") Then
                                                auxIconPath = REACPOSR1_IconName
                                            ElseIf (rotorPosition.ElementStatus = "S") Then
                                                auxIconPath = REACPOSR1SL_IconName
                                            ElseIf (rotorPosition.ElementStatus = "R2") Then
                                                auxIconPath = REACPOSR1R2_IconName
                                            ElseIf (rotorPosition.ElementStatus = "F") Then
                                                auxIconPath = REACPOSFINI_IconName
                                            End If

                                        Case "C"  'Contaminated
                                            auxIconPath = REACPOSCONT_IconName

                                        Case "P" 'Dilution
                                            auxIconPath = REACPOSDIL_IconName
                                    End Select
                                End If

                                UpdateRotorArea(rotorPosition, auxIconPath, myRotorPicture)
                                If (rotorPosition.Selected) Then
                                    'Reload the info area with the new information 
                                    mySelectedElementInfo = GetLocalPositionInfo(rotorPosition.RingNumber, rotorPosition.CellNumber, False)
                                    ShowPositionInfoReactionsArea(ActiveAnalyzer, rotorPosition.RingNumber, rotorPosition.CellNumber)
                                End If
                            End If
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdateRotorTreeViewArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UpdateRotorTreeViewArea", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try

    End Sub

    ''' <summary>
    ''' Update the global variable for current RotorType when a different Tab is selected
    ''' </summary>
    Private Sub UpdateRotorType()
        Select Case MonitorTabs.SelectedTabPage.Name
            Case SamplesTab.Name
                myRotorTypeForm = "SAMPLES"
            Case ReagentsTab.Name
                myRotorTypeForm = "REAGENTS"
            Case ReactionsTab.Name
                myRotorTypeForm = "REACTIONS"
            Case Else
                myRotorTypeForm = String.Empty
        End Select
    End Sub
#End Region

#Region "Events"

    Private Sub bsSamplesMoveFirstPositionButton_Click(ByVal sender As System.Object, _
                                                           ByVal e As System.EventArgs) Handles bsSamplesMoveFirstPositionButton.Click, _
                                                                                                bsReagentsMoveFirstPositionButton.Click, _
                                                                                                bsReactionsMoveFirstPositionButton.Click
        ScrollButtonsInfoArea(1)
    End Sub

    Private Sub bsSamplesDecreaseButton_Click(ByVal sender As System.Object, _
                                              ByVal e As System.EventArgs) Handles bsSamplesDecreaseButton.Click, _
                                                                                    bsReagentsDecreaseButton.Click, _
                                                                                    bsReactionsDecreaseButton.Click
        ScrollButtonsInfoArea(2)
    End Sub

    Private Sub bsSamplesIncreaseButton_Click(ByVal sender As System.Object, _
                                              ByVal e As System.EventArgs) Handles bsSamplesIncreaseButton.Click, _
                                                                                    bsReagentsIncreaseButton.Click, _
                                                                                    bsReactionsIncreaseButton.Click
        ScrollButtonsInfoArea(3)
    End Sub

    Private Sub bsSamplesMoveLastPositionButton_Click(ByVal sender As System.Object, _
                                             ByVal e As System.EventArgs) Handles bsSamplesMoveLastPositionButton.Click, _
                                                                                  bsReagentsMoveLastPositionButton.Click, _
                                                                                  bsReactionsMoveLastPositionButton.Click
        ScrollButtonsInfoArea(4)
    End Sub

#End Region

#Region "TO DELETE - OLD"
    Private Sub MarkSelectedPositionOLD(ByVal pRingNumber As Integer, ByVal pCellNumber As Integer, ByVal pMarkPosition As Boolean)
        Try
            Dim rotorPrefix As String = Nothing
            Dim controlQuery As List(Of BSRImage)
            Dim FilterName As String

            Select Case (myRotorTypeForm.ToString)
                Case "SAMPLES"
                    rotorPrefix = "Sam"
                Case "REAGENTS"
                    rotorPrefix = "Reag"
                Case "REACTIONS"                                ' DL 02/06/2011
                    rotorPrefix = "Reac"                        ' DL 02/06/2011
            End Select

            If Not String.Equals(rotorPrefix, "Reac") Then
                FilterName = String.Format("{0}{1}{2}", rotorPrefix, pRingNumber, pCellNumber)
                controlQuery = (From b In myPosControlList.AsEnumerable _
                                Where String.Equals(b.Name, FilterName) _
                                Select b).ToList()

                If (controlQuery.Count > 0) Then
                    If (pMarkPosition) Then

                        Dim query As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                        query = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                                 Where String.Equals(a.RotorType, myRotorTypeForm) _
                                 AndAlso String.Equals(a.RingNumber, pRingNumber) _
                                 AndAlso String.Equals(a.CellNumber, pCellNumber) _
                                 Select a).ToList()

                        If String.Equals(rotorPrefix, "Sam") Then 'No changes for samples rotor
                            'DL 27/09/2011
                            'controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & EMPTYCELL_IconName)

                            If String.Equals(query.First.TubeContent, "PATIENT") OrElse String.Equals(query.First.TubeContent, "TUBE_SPEC_SOL") OrElse _
                               String.Equals(query.First.TubeContent, "TUBE_WASH_SOL") OrElse String.Equals(query.First.TubeContent, "WASH_SOL") OrElse _
                               String.Equals(query.First.TubeContent, "SPEC_SOL") Or String.Equals(query.First.TubeContent, "CALIB") OrElse _
                               String.Equals(query.First.TubeContent, "CTRL") Then
                                'query.First.TubeContent = "REAGENT" Then

                                If String.Equals(query.First.BarcodeStatus, "ERROR") Then
                                    controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLEBCERR_SEL_IconName)
                                Else
                                    Select Case query.First.Status
                                        Case "PENDING"
                                            controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLEPENDING_SEL_IconName)
                                        Case "DEPLETED"
                                            controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLEDEPLETED_SEL_IconName)
                                        Case "NO_INUSE"
                                            controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLENOINUSE_SEL_IconName)
                                        Case "FINISHED"
                                            controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLEFINISHED_SEL_IconName)
                                        Case "INPROCESS"
                                            controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLEINPROCES_SEL_IconName)
                                            'Case "BARERROR"
                                            'Case "FREE"
                                            'Case "FEW"
                                    End Select
                                End If
                            Else
                                If String.Equals(query.First.BarcodeStatus, "ERROR") Then
                                    controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLEBCERR_SEL_IconName)
                                Else
                                    controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & EMPTYCELL_IconName)
                                End If

                            End If
                            'DL 27/09/2011

                        Else 'Changes for reagents rotor
                            'Dim query As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                            'query = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                            '            Where a.RotorType = myRotorTypeForm _
                            '            AndAlso a.RingNumber = pRingNumber _
                            '            AndAlso a.CellNumber = pCellNumber _
                            '            Select a).ToList()

                            'DL 20/09/2011
                            If query.Count > 0 Then
                                If String.Equals(query.First.Status, "FREE") Then
                                    If pRingNumber = 1 Then
                                        Select Case query(0).BarcodeStatus
                                            Case "ERROR"
                                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCERRSMALLR1_SEL_IconName)
                                            Case "UNKNOWN"
                                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCUKNSMALLR1_SEL_IconName)
                                            Case Else
                                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BOTTLE2_EMPTYCELL_IconName)
                                        End Select
                                    Else
                                        Select Case query(0).BarcodeStatus
                                            Case "ERROR"
                                                If String.Equals(query.First.TubeType, "BOTTLE2") Then
                                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCERRSMALLR2_SEL_IconName)
                                                ElseIf String.Equals(query.First.TubeType, "BOTTLE3") Then
                                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCERRBIGR2_SEL_IconName)
                                                End If

                                            Case "UNKNOWN"
                                                If String.Equals(query.First.TubeType, "BOTTLE2") Then
                                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCUKNSMALLR2_SEL_IconName) '  BTLBCUKNSMALLR1_SEL_IconName)
                                                ElseIf String.Equals(query.First.TubeType, "BOTTLE3") Then
                                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCUKNBIGR2_SEL_IconName) '  BTLBCUKNSMALLR1_SEL_IconName)
                                                End If

                                            Case Else
                                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BOTTLE3_EMPTYCELL_IconName)
                                        End Select
                                    End If

                                ElseIf String.Equals(query.First.TubeType, "BOTTLE2") OrElse String.Equals(query.First.TubeType, "BOTTLE1") Then
                                    If pRingNumber = 1 Then
                                        Select Case query(0).BarcodeStatus
                                            Case "ERROR"
                                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCERRSMALLR1_SEL_IconName)

                                            Case "UNKNOWN"
                                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCUKNSMALLR1_SEL_IconName)

                                            Case Else
                                                Select Case query.First.Status
                                                    Case "INUSE"
                                                        Select Case query.First.TubeContent
                                                            Case "WASH_SOL", "SPEC_SOL", "TUBE_SPEC_SOL"
                                                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLADDSOLSMALLR1_SEL_IconName)

                                                            Case "REAGENT"
                                                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLREAGSMALLD1_SEL_IconName)

                                                        End Select

                                                    Case "DEPLETED", "LOCKED" 'TR 28/09/2012 add the locked status to do the same as deplete 
                                                        ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLDEPLETSMALLR1_SEL_IconName)

                                                    Case "FEW"
                                                        ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLFEWSMALLR1_SEL_IconName)

                                                    Case "NO_INUSE"
                                                        ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLNUSESMALLR1_SEL_IconName)
                                                End Select
                                        End Select

                                    Else
                                        Select Case query(0).BarcodeStatus
                                            Case "ERROR"
                                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCERRSMALLR2_SEL_IconName)

                                            Case "UNKNOWN"
                                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCUKNSMALLR2_SEL_IconName)

                                            Case Else
                                                Select Case query.First.Status
                                                    Case "INUSE"
                                                        Select Case query.First.TubeContent
                                                            Case "WASH_SOL", "SPEC_SOL", "TUBE_SPEC_SOL"
                                                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLADDSOLSMALLR2_SEL_IconName)
                                                            Case "REAGENT"
                                                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLREAGSMALLD2_SEL_IconName)
                                                        End Select
                                                    Case "DEPLETED", "LOCKED" 'TR 28/09/2012 add the locked status to do the same as deplete 
                                                        ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLDEPLETSMALLR2_SEL_IconName)
                                                    Case "FEW"
                                                        ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLFEWSMALLR2_SEL_IconName)
                                                    Case "NO_INUSE"
                                                        ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLNUSESMALLR2_SEL_IconName)
                                                End Select
                                        End Select
                                    End If

                                ElseIf String.Equals(query.First.TubeType, "BOTTLE3") Then
                                    Select Case query(0).BarcodeStatus
                                        Case "ERROR"
                                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCERRBIGR2_SEL_IconName)
                                        Case "UNKNOWN"
                                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCUKNBIGR2_SEL_IconName)
                                        Case Else
                                            Select Case query.First.Status
                                                Case "INUSE"
                                                    'Select Case query.First.TubeContent
                                                    '    Case "WASH_SOL", "SPEC_SOL", "TUBE_SPEC_SOL"
                                                    '        ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLADSLBIGR2SEL_IconName)

                                                    '    Case "REAGENT"
                                                    '        ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLREAGBIG2_SEL_IconName)
                                                    'End Select
                                                    If String.Equals(query.First.TubeContent, "REAGENT") Then
                                                        ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLREAGBIG2_SEL_IconName)
                                                    ElseIf String.Equals(query.First.TubeContent, "WASH_SOL") _
                                                    OrElse String.Equals(query.First.TubeContent, "SPEC_SOL") _
                                                    OrElse String.Equals(query.First.TubeContent, "TUBE_SPEC_SOL") Then
                                                        ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLADSLBIGR2SEL_IconName)
                                                    End If

                                                Case "DEPLETED", "LOCKED" 'TR 28/09/2012 add the locked status to do the same as deplete 
                                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLDEPLBIGR2_SEL_IconName)
                                                Case "FEW"
                                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLFEWBIGR2_SEL_IconName)
                                                Case "NO_INUSE"
                                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLNUSEBIGR2_SEL_IconName)
                                            End Select
                                    End Select
                                End If
                            End If
                            'END DL 20/09/2011
                        End If

                        'TR 25/09/2013 #memory
                        query = Nothing
                        'TR 25/09/2013 -END

                    Else ' Discarted position!
                        'Validate if the element is a NO_INUSE element Status to set the correct back ground.
                        Dim qNoInUse As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                        qNoInUse = (From c In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                                   Where c.RotorType = myRotorTypeForm _
                                     AndAlso c.RingNumber = pRingNumber _
                                     AndAlso c.CellNumber = pCellNumber _
                                   Select c).ToList()

                        If (qNoInUse.Count > 0) Then
                            'For reagent restore the original image
                            If rotorPrefix = "Reag" Then
                                If qNoInUse.First.Status = "FREE" Then
                                    ChangeControlPositionImage(controlQuery.First, Nothing, True)
                                ElseIf qNoInUse.First.TubeContent = "REAGENT" Then
                                    If qNoInUse.First.TubeType = "BOTTLE2" OrElse qNoInUse.First.TubeType = "BOTTLE1" Then
                                        If pRingNumber = 1 Then
                                            Me.ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLREAGSMALLD1_IconName) 'BOTTLE2_IconName)
                                        Else
                                            Me.ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLREAGSMALLD2_IconName) 'BOTTLE2_R2_IconName)
                                        End If
                                    ElseIf qNoInUse.First.TubeType = "BOTTLE3" Then
                                        ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLREAGBIG2_IconName)
                                    End If

                                Else 'Additional Solution
                                    If qNoInUse.First.TubeType = "BOTTLE2" OrElse qNoInUse.First.TubeType = "BOTTLE1" Then
                                        If pRingNumber = 1 Then
                                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLADDSOLSMALLR1_IconName)
                                        Else
                                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLADDSOLSMALLR2_IconName)
                                        End If
                                    ElseIf qNoInUse.First.TubeType = "BOTTLE3" Then
                                        ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLADDSOLBIGR2_IconName)
                                    End If
                                End If
                            End If

                            SetPosControlBackGround(controlQuery.First(), _
                                                    qNoInUse.First.Status, _
                                                    qNoInUse.First.RotorType, _
                                                    qNoInUse.First.TubeType, _
                                                    qNoInUse.First.RingNumber, _
                                                    qNoInUse.First.BarcodeStatus)
                        Else
                            controlQuery.First.BackgroundImage = Nothing
                        End If

                        'TR 25/09/2013 #memory
                        qNoInUse = Nothing
                        'TR 25/09/2013 -END.

                    End If

                End If

            Else
                Dim auxIconPath As String = ""

                FilterName = "Reac" & pCellNumber
                controlQuery = (From b In myPosControlList.AsEnumerable _
                                Where b.Name = FilterName _
                                Select b).ToList()

                If (controlQuery.Count > 0) Then
                    If (pMarkPosition) Then
                        Dim myImage() As String = controlQuery.First.ImagePath.ToString.Split("\"c)
                        Dim myImageName() As String = myImage(UBound(myImage)).ToString.Split(".png".ToCharArray)

                        PreviousSelect = myImageName(0)

                        'DL 10/06/2011
                        'Set cell to selected check previous status 
                        Select Case PreviousSelect
                            Case "REACPOSR1"
                                auxIconPath = REACPOSR1S_IconName       'Previous R1 to R1 selected
                            Case "REACPOSDIL"
                                auxIconPath = REACPOSDILS_IconName      'Previous dilution to dilution selected 
                            Case "REACPOSNIUS"
                                auxIconPath = REACPOSNIUSS_IconName     'Previous No in use to no in use selected
                            Case "REACPOSR1SL"
                                auxIconPath = REACPOSR1SLS_IconName     'Previous R1SL to R1SL selected
                            Case "REACPOSCONT"
                                auxIconPath = REACPOSCONTS_IconName     'Previous Contaminated to contaminated selected
                            Case "REACPOSFINI"
                                auxIconPath = REACPOSFINIS_IconName     'Previous finished to finished selected
                            Case "REACPOSR1R2"
                                auxIconPath = REACPOSR1R2S_IconName     'Previous R1R2 to R1R2 selected
                            Case "REACPOSWASH"
                                auxIconPath = REACPOSWASHS_IconName     'Previous washing to washing selected
                            Case "REACPOSOPTI"
                                auxIconPath = REACPOSOPTIS_IconName     'Previous optical to òptical selected 
                            Case "REACPOSR1S"
                                auxIconPath = REACPOSR1S_IconName       'Previous R1 selected to R1 selected
                            Case "REACPOSDILS"
                                auxIconPath = REACPOSDILS_IconName      'Previous dilution selected to dilution selected 
                            Case "REACPOSNIUSS"
                                auxIconPath = REACPOSNIUSS_IconName     'Previous No In use selected to no in use selected 
                            Case "REACPOSR1SLS"
                                auxIconPath = REACPOSR1SLS_IconName     'Previous R1SL selected to R1SL selected
                            Case "REACPOSCONTS"
                                auxIconPath = REACPOSCONTS_IconName     'Previous contaminated selected to contaminated selected
                            Case "REACPOSFINIS" 'DL 30/01/2012
                                auxIconPath = REACPOSFINIS_IconName     'Previous finish selected to finish seleted
                            Case "REACPOSR1R2S"
                                auxIconPath = REACPOSR1R2S_IconName     'Previous R1R2 selected to R1R2 selected
                            Case "REACPOSWASHS"
                                auxIconPath = REACPOSWASHS_IconName     'previous washing selected to washing selected 
                            Case "REACPOSOPTIS"
                                auxIconPath = REACPOSOPTIS_IconName     'Previous Optical selected to optical selected
                            Case Else
                                auxIconPath = REACPOSELEC_IconName
                        End Select

                        controlQuery.First.Image = Image.FromFile(MyBase.IconsPath & auxIconPath)
                        controlQuery.First.BringToFront()
                        controlQuery.First.Refresh()

                    Else

                        Select Case PreviousSelect
                            'DL 10/06/2011
                            Case "REACPOSR1"
                                auxIconPath = REACPOSR1_IconName        'Previous R1 to R1
                            Case "REACPOSDIL"
                                'AG 14/12/2011
                                auxIconPath = REACPOSDIL_IconName       'Previous dilution to dilution
                            Case "REACPOSNIUS"
                                auxIconPath = REACPOSNIUS_IconName      'Previous No in use to No in use
                            Case "REACPOSR1SL"
                                auxIconPath = REACPOSR1SL_IconName      'Previous R1SL to R1SL
                            Case "REACPOSCONT"
                                auxIconPath = REACPOSCONT_IconName      'Previous contaminated to contaminated
                            Case "REACPOSFINI"
                                auxIconPath = REACPOSFINI_IconName      'Previous finish to finish
                            Case "REACPOSR1R2"
                                auxIconPath = REACPOSR1R2_IconName      'previous R1R2 to R1R2
                            Case "REACPOSWASH"
                                auxIconPath = REACPOSWASH_IconName      'previous washing to washing
                            Case "REACPOSOPTI"
                                auxIconPath = REACPOSOPTI_IconName      'previous optical to optical
                            Case "REACPOSR1S"
                                auxIconPath = REACPOSR1_IconName        'Previous R1 selected to R1
                            Case "REACPOSDILS"
                                auxIconPath = REACPOSDIL_IconName       'Previous Dilution selected to Dilution
                            Case "REACPOSNIUSS"
                                auxIconPath = REACPOSNIUS_IconName      'Previous No in use selected to no in use
                            Case "REACPOSR1SLS"
                                auxIconPath = REACPOSR1SL_IconName      'Previous R1SL selected to R1SL
                            Case "REACPOSCONTS"
                                auxIconPath = REACPOSCONT_IconName      'Previous contaminated selected to contaminated selected 
                            Case "REACPOSFINIS"
                                auxIconPath = REACPOSFINI_IconName      'Previous finished selected to finish
                            Case "REACPOSR1R2S"
                                auxIconPath = REACPOSR1R2_IconName      'Previous R1R2 selected to R1R2
                            Case "REACPOSWASHS"
                                auxIconPath = REACPOSWASH_IconName      'previous washing selected to washing
                            Case "REACPOSOPTIS"
                                auxIconPath = REACPOSOPTI_IconName      'Previous optical selected to optical
                        End Select

                        If PreviousSelect <> "" Then
                            controlQuery.First.Image = Image.FromFile(MyBase.IconsPath & auxIconPath)
                            controlQuery.First.BringToFront()
                            controlQuery.First.Refresh()
                        Else
                            controlQuery.First.Image = Nothing
                        End If
                    End If
                End If
            End If

            'DL 05/10/2011
            Dim hasSelect As EnumerableRowCollection(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
            hasSelect = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                         Where String.Equals(a.RotorType, myRotorTypeForm) _
                         AndAlso a.Selected = True _
                         Select a)

            Select Case myRotorTypeForm

                Case "SAMPLES"
                    If hasSelect.Count > 0 Then
                        bsSamplesMoveFirstPositionButton.Enabled = True
                        bsSamplesMoveLastPositionButton.Enabled = True
                        bsSamplesDecreaseButton.Enabled = True
                        bsSamplesIncreaseButton.Enabled = True
                    Else
                        bsSamplesMoveFirstPositionButton.Enabled = False
                        bsSamplesMoveLastPositionButton.Enabled = False
                        bsSamplesDecreaseButton.Enabled = False
                        bsSamplesIncreaseButton.Enabled = False
                    End If

                Case "REAGENTS"
                    If hasSelect.Count > 0 Then
                        bsReagentsMoveFirstPositionButton.Enabled = True
                        bsReagentsMoveLastPositionButton.Enabled = True
                        bsReagentsDecreaseButton.Enabled = True
                        bsReagentsIncreaseButton.Enabled = True
                    Else
                        bsReagentsMoveFirstPositionButton.Enabled = False
                        bsReagentsMoveLastPositionButton.Enabled = False
                        bsReagentsDecreaseButton.Enabled = False
                        bsReagentsIncreaseButton.Enabled = False
                    End If

                Case "REACTIONS"
                    If hasSelect.Count > 0 Then
                        bsReactionsDecreaseButton.Enabled = True
                        bsReactionsIncreaseButton.Enabled = True
                        bsReactionsMoveFirstPositionButton.Enabled = True
                        bsReactionsMoveLastPositionButton.Enabled = True

                    Else
                        bsReactionsDecreaseButton.Enabled = False
                        bsReactionsIncreaseButton.Enabled = False
                        bsReactionsMoveFirstPositionButton.Enabled = False
                        bsReactionsMoveLastPositionButton.Enabled = False
                    End If
            End Select
            'DL 05/10/2011

            'TR 25/09/2013 #memory
            controlQuery = Nothing
            'TR 25/09/2013

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))" & " [Ring Nr: " & pRingNumber & ", Cell Nr: " & pCellNumber & ", Mark: " & pMarkPosition & "]", _
                              Me.Name & ".MarkSelectedPosition", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".MarkSelectedPosition", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' Get the name of the Icon for the specified TubeContent 
    ''' </summary>
    ''' <param name="pTubeContent">Tube Content</param>
    ''' <param name="pTubeSize">Tube Content</param>
    ''' <param name="pRingNumber">Tube Content</param>
    ''' <param name="pBarCodeStatus"></param> 
    ''' <returns>The icon Name</returns>
    ''' <remarks>
    ''' Created by:  TR 15/01/2010 - Tested: OK.
    ''' Modified by: TR 18/01/2010 - Added the case for TubeContent=REAGENT (Tested: OK)
    '''              SA 14/07/2010 - Use the global variables with Icon Names instead of get then every time from the DB 
    '''              AG 13/10/2010 - add new parameters TubeSize used for return different icon name for reagents rotor
    '''              RH 17/06/2011 - Add TUBE_SPEC_SOL and TUBE_WASH_SOL.
    '''              DL 07/09/2011 - Check barcode status
    ''' </remarks>
    Private Function GetIconNameByTubeContentOLD(ByVal pTubeContent As String, _
                                                 ByVal pTubeSize As String, _
                                              ByVal pRingNumber As Integer, _
                                              ByVal pBarCodeStatus As String) As String
        Dim myIconPath As String = ""

        Try

            'If pTubeContent <> "REAGENT" And pTubeContent <> "CALIB" Then MsgBox(pTubeContent)
            'If pRingNumber = 1 Then MsgBox(pTubeContent)

            Select Case pTubeContent
                Case "CALIB"
                    myIconPath = CALIB_IconName
                    Exit Select

                Case "CTRL"
                    myIconPath = CTRL_IconName
                    Exit Select

                Case "PATIENT"
                    myIconPath = ROUTINES_IconName
                    'myIconPath = SAMPLE_BC_ERR_IconName
                    Exit Select

                Case "TUBE_SPEC_SOL", "TUBE_WASH_SOL"
                    myIconPath = ADDSAMPLESOL_IconName
                    Exit Select

                Case "REAGENT"
                    'AG 13/10/2010 - Different image depending TubeSize
                    'myIconPath = REAGENTS_IconName
                    If String.Equals(pTubeSize, "BOTTLE2") OrElse String.Equals(pTubeSize, "BOTTLE1") Then
                        myIconPath = If(pRingNumber = 1, BTLREAGSMALLD1_IconName, BTLREAGSMALLD2_IconName)

                    ElseIf pTubeSize = "BOTTLE3" Then
                        myIconPath = BTLREAGBIG2_IconName
                    End If
                    'END AG 13/10/2010
                    Exit Select

                Case "SPEC_SOL", "WASH_SOL"
                    'AG 13/10/2010 - Different image depending TubeSize
                    'myIconPath = ADDSOL_IconName
                    If String.Equals(pTubeSize, "BOTTLE2") OrElse String.Equals(pTubeSize, "BOTTLE1") Then
                        myIconPath = If(pRingNumber = 1, BTLADDSOLSMALLR1_IconName, BTLADDSOLSMALLR2_IconName)

                    ElseIf String.Equals(pTubeSize, "BOTTLE3") Then
                        myIconPath = BTLADDSOLBIGR2_IconName 'BOTTLE3_ADDSOL_IconName
                    End If
                    'END AG 13/10/2010
                    Exit Select
            End Select


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetIconNameByTubeContent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetIconNameByTubeContent", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
        Return myIconPath
    End Function

    ''' <summary>
    ''' Get names of all Icons needed for the Rotor Area and set Background Images of all PictureBox
    ''' in Legend frames
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 
    ''' </remarks>
    Private Sub PrepareIconNamesOLD()
        Try
            'Get Icon Names...
            CALIB_IconName = GetIconName("CALIB")
            CTRL_IconName = GetIconName("CTRL")
            STATS_IconName = GetIconName("STATS")
            ROUTINES_IconName = GetIconName("ROUTINES")
            DILUTIONS_IconName = GetIconName("DILUTIONS")
            'REAGENTS_IconName = GetIconName("REAGENTS")
            'SAMPLES_IconName = GetIconName("SAMPLESNODE")
            'ADDSOL_IconName = GetIconName("ADD_SOL")
            ADDSAMPLESOL_IconName = GetIconName("ADD_SAMPLE_SOL") 'RH 17/06/2011
            EMPTYCELL_IconName = GetIconName("EMPTYCELL")
            'EMPTYCELL_IconName = GetIconName("LEG_SELECT")

            'Satus Images
            DEPLETED_IconName = GetIconName("LEG_DEPLETED")
            'FEWVOL_IconName = GetIconName("FEWVOLUME")
            NOTINUSE_IconName = GetIconName("NOT_INUSE")

            'Load Legend icons
            'Reagents
            LEGREAGENTDEPLETED_IconName = GetIconName("LEG_BDEPLETED")
            LEGREAGENTNOTINUSE_IconName = GetIconName("LEG_BNOINUSE")
            'LEGREAGENTSELECTED_IconName = GetIconName("LEG_BSELECT")
            LEGREAGENTBCERRORRG_IconName = GetIconName("LEG_BC_ERR")
            LEGBCREAGENTUNKNOWN_IconName = GetIconName("LEG_BC_UKN")
            LEGREAGENTADDSOL_IconName = GetIconName("LEG_BWASH")
            LEGREAGENTREAGENT_IconName = GetIconName("LEG_BREAGENT")
            LEGREAGENTFEWVOL_IconName = GetIconName("LEG_BLOWER")

            'Samples
            LEGSAMPLESELECT_Iconname = GetIconName("MON_LEG_SELECT")
            LEGSAMPLENOTINUSE_IconName = GetIconName("MON_LEG_NOTINU")
            LEGSAMPLEDEPLETED_IconName = GetIconName("MON_LEG_DEPLET")
            LEGSAMPLEPENDING_IconName = GetIconName("MON_LEG_PENDIN")
            LEGSAMPLEINPROGR_IconName = GetIconName("MON_LEG_INPROG") '25
            LEGSAMPLEFINISHD_IconName = GetIconName("MON_LEG_FINISH")
            LEGSAMPLEBCERROR_IconName = GetIconName("MON_LEG_BCERR")

            'Reactions
            LEGREACTIONWASHING_IconName = GetIconName("REACWAHSING")
            LEGREACTIONNOTINUSE_IconName = GetIconName("REACNOTINUSE")
            LEGREACTIONR1_IconName = GetIconName("REACR1")
            LEGREACTIONR1SAMPLE_IconName = GetIconName("REACR1SAMPLE")
            LEGREACTIONR1SAMPLER2_IconName = GetIconName("REACR1SAMPLER2")
            LEGREACTIONDILUTION_IconName = GetIconName("REACDILUTION")
            LEGREACTIONFINISH_IconName = GetIconName("REACFINISH")
            LEGREACTIONCONTAM_IconName = GetIconName("REACCONTAM")
            LEGREACTIONOPTICAL_IconName = GetIconName("REACOPTIC")
            'End load legend icons

            'SELECTED_IconName = GetIconName("LEG_SELECT")
            PENDING_IconName = GetIconName("LEG_PENDING")
            INPROGRESS_IconName = GetIconName("LEG_INPROGRESS")
            FINISHED_IconName = GetIconName("LEG_FINISH")
            'BARCODEERROR_IconName = GetIconName("LEG_BARCODERR")

            BTLSAMPLEBCERR_SEL_IconName = GetIconName("SPLBCERR_SEL")
            BTLSAMPLEBCERR_IconName = GetIconName("SPLBCERR")

            'DL 02/09/2011 -Load the icons
            'SAMPLE_BC_ERR_IconName = GetIconName("SAMPLE_BC_ERR")
            BTLBCUKNSMALLR1_IconName = GetIconName("BTLUKWNSMLR1") 'BOTTLE2_BC_UKN_IconName = GetIconName("BOTTLE2_BC_WNG")
            BTLBCUKNSMALLR1_SEL_IconName = GetIconName("BTLUKWNSMLR1SEL")
            BTLBCERRSMALLR1_IconName = GetIconName("BTLBCERRSMLR1") 'BOTTLE2_BC_ERR_IconName = GetIconName("BOTTLE2_BC_ERR")
            BTLBCERRSMALLR1_SEL_IconName = GetIconName("BTLBCERSMLR1SEL")
            BTLBCUKNSMALLR2_IconName = GetIconName("BTLUKWNSMLR2")
            BTLBCUKNSMALLR2_SEL_IconName = GetIconName("BTLUKWNSMLR2SEL")
            BTLBCERRSMALLR2_IconName = GetIconName("BTLBCERRSMLR2") 'BOTTLE2_R2_BC_ERR_IconName = GetIconName("BOTTLE21_BC_ERR") 50
            BTLBCERRSMALLR2_SEL_IconName = GetIconName("BTLBCERSMLR2SEL")

            BTLSAMPLEINPROCES_SEL_IconName = GetIconName("SPLPROC_SEL")
            BTLSAMPLEDEPLETED_SEL_IconName = GetIconName("SPLDEPL_SEL")
            BTLSAMPLEFINISHED_SEL_IconName = GetIconName("SPLFINI_SEL")
            BTLSAMPLENOINUSE_SEL_IconName = GetIconName("SPLNUSE_SEL")
            BTLSAMPLEPENDING_SEL_IconName = GetIconName("SPLPEND_SEL")
            BTLBCERRBIGR2_IconName = GetIconName("BTLBCERRBIGR2") 'BOTTLE3_BC_ERR_IconName = GetIconName("BOTTLE3_BC_ERR")
            BTLBCERRBIGR2_SEL_IconName = GetIconName("BTLBCERBIGR2SEL")
            BTLBCUKNBIGR2_IconName = GetIconName("BTLUKWNBIGR2")
            BTLBCUKNBIGR2_SEL_IconName = GetIconName("BTLUKWNBIGR2SEL")

            '
            'Images for reagents rotors
            'EXTERNAL RING (20 mL)
            BTLREAGSMALLD1_IconName = GetIconName("BTLREAGSMLD1") ' BOTTLE2_IconName = GetIconName("BOTTLE2")DL 19/09/2011
            BTLREAGSMALLD1_SEL_IconName = GetIconName("BTLREAGSMLD1SEL")
            BTLREAGSMALLD2_SEL_IconName = GetIconName("BTLREAGSMLD2SEL")
            BTLREAGBIG2_SEL_IconName = GetIconName("BTLREAGBIGD2SEL")

            'BOTTLE2_SEL_IconName = GetIconName("BOTTLE2_SEL")  'DL 06/03/2012
            BTLDEPLETSMALLR1_IconName = GetIconName("BTLDEPLETSMLR1") 'BOTTLE2_DEP_IconName = GetIconName("BOTTLE2_DEP")
            BTLDEPLETSMALLR1_SEL_IconName = GetIconName("BTLDEPSMLR1SEL")
            BTLFEWSMALLR1_IconName = GetIconName("BTLFEWSMLR1") 'BOTTLE2_FEW_IconName = GetIconName("BOTTLE2_FEW")
            BTLFEWSMALLR1_SEL_IconName = GetIconName("BTLFEWSMLR1SEL")
            BTLNUSESMALLR1_IconName = GetIconName("BTLNUSESMLR1") 'BOTTLE2_NOINUSE_IconName = GetIconName("BOTTLE2_NOINUSE")
            BTLNUSESMALLR1_SEL_IconName = GetIconName("BTLNUSESMLR1SEL")
            BTLADDSOLSMALLR1_IconName = GetIconName("BTLADDSOLSMLR1") 'BOTTLE2_ADDSOL_IconName
            BTLADDSOLSMALLR1_SEL_IconName = GetIconName("BTLADSLSMLR1SEL")
            BOTTLE2_EMPTYCELL_IconName = GetIconName("BOTTLE2_EMPTY")
            BOTTLE2_NOTHING_IconName = GetIconName("BOTTLE2_NOTHING")

            'INTERNAL RING (20 & 60 mL)
            BTLREAGSMALLD2_IconName = GetIconName("BTLREAGSMLD2") 'BOTTLE2_R2_IconName = GetIconName("BOTTLE2(R2)") DL 19/09/2011 75
            'BOTTLE2_R2_SEL_IconName = GetIconName("BOTTLE2(R2)_SEL")
            BTLDEPLETSMALLR2_IconName = GetIconName("BTLDEPLETSMLR2") 'BOTTLE2_R2_DEP_IconName = GetIconName("BOTTLE2(R2)_DEP")
            BTLDEPLETSMALLR2_SEL_IconName = GetIconName("BTLDEPSMLR2SEL")
            BTLFEWSMALLR2_IconName = GetIconName("BTLFEWSMLR2") 'BOTTLE2_R2_FEW_IconName = GetIconName("BOTTLE2(R2)_FEW")
            BTLFEWSMALLR2_SEL_IconName = GetIconName("BTLFEWSMLR2SEL")
            BTLNUSESMALLR2_IconName = GetIconName("BTLNUSESMLR2") 'BOTTLE2_R2_NOINUSE_IconName = GetIconName("BOTTLE2(R2)_NI")
            BTLNUSESMALLR2_SEL_IconName = GetIconName("BTLNUSESMLR2SEL")
            BTLADDSOLSMALLR2_IconName = GetIconName("BTLADDSOLSMLR2") 'BOTTLE2_R2_ADDSOL_IconName = GetIconName("BOTTLE2(R2)_AS"))
            BTLADDSOLSMALLR2_SEL_IconName = GetIconName("BTLADSLSMLR2SEL")
            BTLREAGBIG2_IconName = GetIconName("BTLREAGBIGD2") 'GetIconName("BOTTLE3")
            '
            'BOTTLE3_SEL_IconName = GetIconName("BOTTLE3_SEL")
            BTLDEPLBIGR2_IconName = GetIconName("BTLDEPLETBIGR2")    'BOTTLE3_DEP_IconName = GetIconName("BOTTLE3_DEP")
            BTLDEPLBIGR2_SEL_IconName = GetIconName("BTLDEPBIGR2SEL")
            BTLFEWBIGR2_IconName = GetIconName("BTLFEWBIGR2")        'BOTTLE3_FEW_IconName = GetIconName("BOTTLE3_FEW")
            BTLFEWBIGR2_SEL_IconName = GetIconName("BTLFEWBIGR2SEL")
            BTLNUSEBIGR2_IconName = GetIconName("BTLNUSEBIGR2")      'BOTTLE3_NOINUSE_IconName = GetIconName("BOTTLE3_NOINUSE")
            BTLNUSEBIGR2_SEL_IconName = GetIconName("BTLNUSEBIGR2SEL")
            BTLADDSOLBIGR2_IconName = GetIconName("BTLADDSOLBIGR2")  'BOTTLE3_ADDSOL_IconName = GetIconName("BOTTLE3_ADDSOL")
            BTLADSLBIGR2SEL_IconName = GetIconName("BTLADSLBIGR2SEL")
            BOTTLE3_EMPTYCELL_IconName = GetIconName("BOTTLE3_EMPTY")
            BOTTLE3_NOTHING_IconName = GetIconName("BOTTLE3_NOTHING")
            ' Reactions Images
            REACPOSCONT_IconName = GetIconName("REACPOSCONT")
            REACPOSFINI_IconName = GetIconName("REACPOSFINI")
            REACPOSNIUS_IconName = GetIconName("REACPOSNIUS")
            REACPOSOPTI_IconName = GetIconName("REACPOSOPTI")
            REACPOSR1_IconName = GetIconName("REACPOSR1") '100
            REACPOSR1SL_IconName = GetIconName("REACPOSR1SL")
            REACPOSR1R2_IconName = GetIconName("REACPOSR1R2")
            REACPOSWASH_IconName = GetIconName("REACPOSWASH")
            REACPOSDIL_IconName = GetIconName("REACPOSDIL") 'Dilution
            REACPOSELEC_IconName = GetIconName("REACPOSELEC")
            '
            REACPOSCONTS_IconName = GetIconName("REASPOSCONT")
            REACPOSFINIS_IconName = GetIconName("REASPOSFINI") 'REACPOSFINI")
            REACPOSNIUSS_IconName = GetIconName("REASPOSNIUS")
            REACPOSOPTIS_IconName = GetIconName("REASPOSOPTI")
            REACPOSR1S_IconName = GetIconName("REASPOSR1")
            REACPOSR1SLS_IconName = GetIconName("REASPOSR1SL")
            REACPOSR1R2S_IconName = GetIconName("REASPOSR1R2")
            REACPOSWASHS_IconName = GetIconName("REASPOSWASH")
            REACPOSDILS_IconName = GetIconName("REASPOSDIL") 'Dilution 114
            '            
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareIconNames ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareIconNames ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' Set the Position Control BackGround depending on the Elemtent Status.
    ''' </summary>
    ''' <param name="pPositionControl">Control To Change the BackGround (pictureBox)</param>
    ''' <param name="pStatus">Indicates if the cell background has to be set to show a Not InUse
    '''                       position or a Depleted one</param>
    ''' <param name="pRotorType"></param>
    ''' <param name="pTubeType"></param>
    ''' <param name="pRingNumber"></param>
    ''' <param name="pBarCodeStatus"></param>
    ''' <remarks>
    ''' Created by:  TR 28/01/2010 - Tested: OK
    ''' Modified by: SA 14/07/2010 - Use the global variables for Icon Names instead the hardcode name
    ''' Modified by AG 05/10/2010 - change picturebox for BSRImage
    ''' Modified by AG 13/10/2010 - add parameter pRotorType and TubeType. When reagent dont change background ... change image!!!
    ''' Modified by DL 07/09/2011 - add parameter pRotorType and TubeType. When reagent dont change background ... change image!!!
    ''' </remarks>
    Private Sub SetPosControlBackGroundOLD(ByVal pPositionControl As BSRImage, _
                                           ByVal pStatus As String, _
                                           ByVal pRotorType As String, _
                                        ByVal pTubeType As String, _
                                        ByVal pRingNumber As Integer, _
                                        ByVal pBarCodeStatus As String)
        Try
            'TR 28/01/2010 - Add functionality for deplete status. and change the if for Select Case.

            Select Case pStatus
                Case "NO_INUSE"
                    If pRotorType <> "REAGENTS" Then
                        pPositionControl.BackgroundImage = Image.FromFile(MyBase.IconsPath & NOTINUSE_IconName)

                    ElseIf pTubeType = "BOTTLE2" OrElse pTubeType = "BOTTLE1" Then
                        If pRingNumber = 1 Then
                            ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLNUSESMALLR1_IconName) ' BOTTLE2_NOINUSE_IconName)
                        Else
                            ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLNUSESMALLR2_IconName) 'BOTTLE2_R2_NOINUSE_IconName)
                        End If

                    ElseIf pTubeType = "BOTTLE3" Then
                        ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLNUSEBIGR2_IconName) 'BOTTLE3_NOINUSE_IconName)
                    End If

                Case "DEPLETED", "LOCKED" 'TR 28/09/2012 add the locked status to do the same as deplete 
                    If pRotorType <> "REAGENTS" Then
                        pPositionControl.BackgroundImage = Image.FromFile(MyBase.IconsPath & DEPLETED_IconName)

                    ElseIf pTubeType = "BOTTLE2" OrElse pTubeType = "BOTTLE1" Then
                        If pRingNumber = 1 Then
                            ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLDEPLETSMALLR1_IconName) 'BOTTLE2_DEP_IconName)
                        Else
                            ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLDEPLETSMALLR2_IconName) 'BOTTLE2_R2_DEP_IconName)
                        End If

                    ElseIf pTubeType = "BOTTLE3" Then
                        ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLDEPLBIGR2_IconName) 'BOTTLE3_DEP_IconName)
                    End If

                Case "PENDING"
                    If pRotorType = "SAMPLES" Then
                        pPositionControl.BackgroundImage = Image.FromFile(MyBase.IconsPath & PENDING_IconName)
                        pPositionControl.BackgroundImageLayout = ImageLayout.Stretch
                    End If

                    Exit Select
                Case "INPROCESS"
                    If pRotorType = "SAMPLES" Then
                        pPositionControl.BackgroundImage = Image.FromFile(MyBase.IconsPath & INPROGRESS_IconName)
                        pPositionControl.BackgroundImageLayout = ImageLayout.Stretch
                    End If

                    Exit Select
                Case "FINISHED"
                    If pRotorType = "SAMPLES" Then
                        pPositionControl.BackgroundImage = Image.FromFile(MyBase.IconsPath & FINISHED_IconName)
                        pPositionControl.BackgroundImageLayout = ImageLayout.Stretch
                    End If
                    Exit Select
                Case "BARERROR"
                    If pRotorType = "SAMPLES" Then
                        'pPositionControl.Image = Image.FromFile(MyBase.IconsPath & BARCODEERROR_IconName)
                        'pPositionControl.BackgroundImage = Image.FromFile(MyBase.IconsPath & BARCODEERROR_IconName)
                        pPositionControl.Image = Image.FromFile(MyBase.IconsPath & BTLSAMPLEBCERR_IconName)
                        pPositionControl.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLEBCERR_IconName)
                        pPositionControl.BackgroundImageLayout = ImageLayout.Stretch
                    End If
                    Exit Select

                Case "FEW"
                    'AG 12/04/2011 - Nes status case (Few volume)
                    If pRotorType <> "REAGENTS" Then
                        'pPositionControl.BackgroundImage = Image.FromFile(MyBase.IconsPath & FEWVOL_IconName)

                    ElseIf pTubeType = "BOTTLE2" OrElse pTubeType = "BOTTLE1" Then
                        If pRingNumber = 1 Then
                            ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLFEWSMALLR1_IconName) 'BOTTLE2_FEW_IconName)
                        Else
                            ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLFEWSMALLR2_IconName) 'BOTTLE2_R2_FEW_IconName)
                        End If

                    ElseIf pTubeType = "BOTTLE3" Then
                        ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLFEWBIGR2_IconName) 'BOTTLE3_FEW_IconName)

                    End If

                Case Else
                    pPositionControl.BackgroundImage = Nothing

            End Select

            'DL 07/09/2011

            If Not pBarCodeStatus Is String.Empty Then

                Select Case pRotorType
                    Case "REAGENTS"
                        If pBarCodeStatus = "ERROR" Then
                            'TR 08/09/2011 -Validate if status is free to set by default the bottle depending the ring number
                            'if Ring 1 20 ml if ring 2 then 60 ml.
                            If pStatus = "FREE" Then
                                If pRingNumber = 1 Then
                                    ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCERRSMALLR1_IconName)
                                Else
                                    'dl 20/09/2011
                                    'ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCERRBIGR2_IconName) 'BOTTLE3_BC_ERR_IconName)
                                    If pTubeType = "BOTTLE2" Then
                                        ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCERRSMALLR2_IconName)

                                    ElseIf pTubeType = "BOTTLE3" Then
                                        ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCERRBIGR2_IconName)
                                    Else
                                        pTubeType = "BOTTLE2"
                                        ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCERRSMALLR2_IconName)

                                    End If
                                    'end dl 20/09/2011
                                End If



                            Else
                                If pTubeType = "BOTTLE2" OrElse pTubeType = "BOTTLE1" Then
                                    If pRingNumber = 1 Then
                                        ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCERRSMALLR1_IconName) 'BOTTLE2_BC_ERR_IconName)
                                    Else
                                        ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCERRSMALLR2_IconName) '1BOTTLE2_R2_BC_ERR_IconName)
                                    End If

                                ElseIf pTubeType = "BOTTLE3" Then
                                    ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCERRBIGR2_IconName) 'BOTTLE3_BC_ERR_IconName)
                                End If
                            End If
                        ElseIf pBarCodeStatus = "UNKNOWN" Then
                            If pTubeType = "BOTTLE2" OrElse pTubeType = "BOTTLE1" Then
                                If pRingNumber = 1 Then
                                    ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCUKNSMALLR1_IconName) 'BOTTLE2_BC_UKN_IconName)
                                Else
                                    ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCUKNSMALLR2_IconName) 'BOTTLE2_R2_BC_UKN_IconName)
                                End If

                            ElseIf pTubeType = "BOTTLE3" Then
                                ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCUKNBIGR2_IconName) 'BOTTLE3_BC_UKN_IconName)
                            End If
                        End If

                    Case "SAMPLES"
                        If pBarCodeStatus = "ERROR" Then
                            pPositionControl.BackgroundImage = Nothing
                            ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLSAMPLEBCERR_IconName) 'SAMPLE_BC_ERR_IconName)
                        End If
                End Select

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SetPosControlBackGroung ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SetPosControlBackGroung", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

#End Region

End Class