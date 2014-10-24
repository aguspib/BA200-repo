Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.PresentationCOM
Imports System.Timers
Imports System.Globalization
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.App

Public Class IWSRotorPositions
    'RH 14/12/2010 Substitute every "And" by "AndAlso" (Only in boolean expressions, not in bitwise expressions!)
    '              Substitute every "Or" by "OrElse"   (Only in boolean expressions, not in bitwise expressions!)
    'To evaluate the boolean expressions in short circuit and speed up mean processing velocity

    'http://msdn.microsoft.com/en-us/library/8067cy78(v=VS.90).aspx
    'In Visual Basic 2008, the And, Or, Not, and Xor operators still evaluate all expressions contributing
    'to their operands. Visual Basic 2008 also introduces two new operators, AndAlso and OrElse, that can
    'reduce execution time by short-circuiting logical evaluations. If the first operand of an AndAlso
    'operator evaluates to False, the second operand is not evaluated. Similarly, if the first operand of
    'an OrElse operator evaluates to True, the second operand is not evaluated.

    'Description of "short-circuit" evaluation in Visual Basic
    'http://support.microsoft.com/kb/817250/en-us


    'RH 11/02/2011 Remove unneeded, avoidable and useless "New" and Typecast instructions all around to speed up mean processing velocity

#Region "Declarations"
    'List that contains information of all the Required Work Session Elements to be positioned 
    Dim listReqElements As List(Of WSRequiredElementsTO)

    'Indicates which Rotor Type is shown by default
    Dim myRotorTypeForm As String = "SAMPLES"

    'Contains all available Tubes (for Controls, Calibrators and Patient Samples) and 
    'Bottles (for Reagents and Additional Solutions). For each Tube/Bottle, the list
    'contains Code, Description, Volume and Position 
    Dim AllTubeSizeList As List(Of TubeSizeTO)

    'Screen structure containing information of all positions in all the available Rotors
    Dim myRotorContentByPositionDSForm As WSRotorContentByPositionDS

    'Screen structure containing information of a single selected Position in the Rotors Area
    Dim mySelectedElementInfo As WSRotorContentByPositionDS

    'TR 07/01/2010
    Dim myPosControlList As List(Of BSRImage)

    'Variables for DragDrop (14/12/2009 AG)
    Dim sourceNode As TreeNode
    Dim isDragging As Boolean = False
    Dim isRotorDragging As Boolean = False '(04/01/2010 TR Variable use to validate the rotor Dragging elements)
    Dim ringNumberForm As Integer = 0
    Dim cellNumberForm As Integer = 0

    'AG 18/01/2010 - Variables for Load Screen Status (Right click, infoarea, rotordragdrop, treedragdrop)
    'Default value:  Not blocked
    Dim isChangeTubeSizeAllowed As Boolean = True   'AG 25/01/2010 isChangeTubeSizeAllowed
    Dim isInfoAreaAllowed As Boolean = True
    Dim isRotorDragDropAllowed As Boolean = True
    Dim isTreeDragDropAllowed As Boolean = True
    Dim isResetRotorAllowed As Boolean = True
    Dim isSingleDeletePosAllowed As Boolean = True
    Dim isCheckRotorVolumeAllowed As Boolean = False
    Dim isSingleCheckPosAllowed As Boolean = True   'AG 25/01/2010    
    Dim isSingleRefillPosAllowed As Boolean = True  'AG 25/01/2010
    Dim isManualBarcodeAllowed As Boolean = True


    'Global variables needed to manage the Load/Save of Virtual Rotors
    'SA 14/07/2010 - Previously they were implemented as properties 
    Private VirtualSampleRotorID As Integer = -1
    Private VirtualSampleRotorName As String = ""
    Private VirtualReagentRotorID As Integer = -1
    Private VirtualReagentRotorName As String = ""

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

    '** Variables for Sample Class Icons(used as Image in the Rotor and also in Legend Area)
    Private CALIB_IconName As String = String.Empty
    Private CTRL_IconName As String = String.Empty
    Private STATS_IconName As String = String.Empty
    Private ROUTINES_IconName As String = String.Empty
    Private DILUTIONS_IconName As String = String.Empty
    Private ADDSAMPLESOL_IconName As String = String.Empty

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

    '** Variables for Reagents Legend Area
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

    'Global variables for names of icons used in the TreeView of required Elements    
    Private REAGENTS_IconName As String = String.Empty
    Private SAMPLES_IconName As String = String.Empty
    Private ADDSOL_IconName As String = String.Empty

    ''' <summary>
    ''' Temporal structure definition for checked volume frame received, and for the scan barcode frame received from analyzer
    ''' </summary>
    ''' <remarks>
    ''' Created by: AG 02/12/09 (Tested: temporally ok, pending frame definition)
    ''' </remarks>
    Structure ScanOrCheckVolumeFwFrame
        Dim RotorType As String
        Dim RingNumber As Byte
        Dim CellNumber As Byte
        Dim Value As String
    End Structure

    Private setControlPosToNothing As Boolean = False 'AG 09/12/2010
    Private ErrorOnCreateWSExecutions As String = String.Empty 'RH 09/02/2011
    'Private mdiAnalyzerCopy As AnalyzerManager '#REFACTORING

    'RH 28/07/2011
    Private ReadOnly COLOR_NOPOS As Color = Color.Black
    Private ReadOnly COLOR_POS As Color = Color.Green

    Private BarCodeErrorSampleFree As Boolean
    Private BarCodeErrorReagentFree As Boolean

    Private sampleBarcodeReaderOFF As Boolean = False
    Private reagentBarcodeReaderOFF As Boolean = False

    'DL 16/04/2013
    Private watchDogTimer As New Timer()
    Private autoWSCreationTimer As New Timer() 'AG 02/01/2014 - BT #1433 (v211 patch2)

    Private ESCKeyPressed As Boolean = False  ' XB 11/03/2014 - #1523
#End Region

#Region "Attributes"
    '21/12/2009 SA - Definition of Fields for the Screen Properties
    Private WorkSessionIDAttribute As String
    Private AnalyzerIDAttribute As String
    Private AnalyzerModelAttribute As String
    Private WorkSessionStatusAttribute As String = ""             'AG 18/01/2010
    Private ShowHostQueryScreenAttribute As Boolean = False       'AG 03/04/2013 - When TRUE after open the rotorpos screen the hostquery screen is opened automatically
    Private OpenByAutomaticProcessAttribute As Boolean = False    'AG 09/07/2013
    Private AutoWSCreationWithLISModeAttribute As Boolean = False 'XB 17/07/2013 - Auto WS process
    Private HQButtonUserClickAttribute As Boolean = False         'SA 25/07/2013
#End Region

#Region "Properties"
    'SA 21/12/2009 - Definition of Screen Property to inform the active Work Session
    'TR 13/01/2010 - Set Type to property
    Public WriteOnly Property ActiveWorkSession() As String
        Set(ByVal value As String)
            WorkSessionIDAttribute = value
        End Set
    End Property

    'AG 18/01/2010 - Definition of Screen Property to inform the Work Session Status
    'AG 11/07/2011 - Added parameter pAnalyzerStatus
    Public WriteOnly Property WorkSessionStatus(ByVal pAnalyzerStatus As String) As String
        Set(ByVal value As String)
            WorkSessionStatusAttribute = value

            'Value = OPEN, PENDING, INPROCESS or ABORTED
            'Screen business has to change case INPROCESS to RUNNING, RUNNINGPAUSE, STANDBY or FINISHED
            If (WorkSessionStatusAttribute = "INPROCESS") Then
                WorkSessionStatusAttribute = pAnalyzerStatus

                'Check if WS is finished or not
                If (WorkSessionStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.STANDBY.ToString) Then
                    Dim myGlobal As New GlobalDataTO
                    Dim executionsNumber As Integer = 0
                    Dim pendingExecutionsLeft As Boolean = False
                    Dim myWSDelegate As New WorkSessionsDelegate

                    myGlobal = myWSDelegate.StartedWorkSessionFlag(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, executionsNumber, pendingExecutionsLeft)
                    If (Not pendingExecutionsLeft) Then
                        WorkSessionStatusAttribute = "FINISHED"
                    End If
                End If
            End If
        End Set
    End Property

    '21/12/2009 SA - Definition of Screen Property to inform the active Analyzer
    '13/01/2010 TR - Set Type to property
    Public WriteOnly Property ActiveAnalyzer() As String
        Set(ByVal value As String)
            AnalyzerIDAttribute = value
        End Set
    End Property

    '13/01/2010 TR - Definition of screen property to inform the Analizer Model
    Public WriteOnly Property AnalyzerModel() As String
        Set(ByVal value As String)
            AnalyzerModelAttribute = value
        End Set
    End Property

    'AG 25/11/2011 - note the internal variable is not call Attribute due this property is new and the variable already existed
    Public ReadOnly Property CurrentRotorType() As String
        Get
            Return myRotorTypeForm
        End Get
    End Property

    Public WriteOnly Property ShowHostQueryScreen() As Boolean 'AG 03/04/2013
        Set(ByVal value As Boolean)
            ShowHostQueryScreenAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' Indicates when the screen has been open by user (FALSE) or when by the automatic WS creation with LIS process (TRUE)
    ''' AG 09/07/2013
    ''' </summary>
    Public WriteOnly Property OpenByAutomaticProcess() As Boolean
        Set(ByVal value As Boolean)
            OpenByAutomaticProcessAttribute = value
        End Set
    End Property

    ' XB 17/07/2013 - Auto WS process
    Public WriteOnly Property AutoWSCreationWithLISMode() As Boolean
        Set(ByVal value As Boolean)
            AutoWSCreationWithLISModeAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' It indicates if a final User has clicked in HostQuery Button in Host Query Screen
    ''' Created by: SA 25/07/2013
    ''' </summary>
    Public WriteOnly Property HQButtonUserClick() As Boolean
        Set(value As Boolean)
            HQButtonUserClickAttribute = value
        End Set
    End Property
#End Region

#Region "Private Methods"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Modified by XB 17/01/2014 - Improve Dispose
    ''' AG 10/02/2014 - #1496 Mark screen closing when ReleaseElement is called
    ''' </remarks>
    Private Sub ReleaseElements()
        Try
            isClosingFlag = True 'AG 10/02/2014 - #1496 Mark screen closing when ReleaseElement is called

            ' XB 17/01/2014
            For Each myControl As Control In Me.SamplesTab.Controls

                If (TypeOf myControl Is BSRImage) Then
                    Dim CurrentControl As BSRImage = CType(myControl, BSRImage)

                    CurrentControl.Image = Nothing
                    CurrentControl.BackgroundImage = Nothing

                    CurrentControl.Dispose()
                    CurrentControl = Nothing
                End If

            Next
            For Each myControl As Control In Me.ReagentsTab.Controls

                If (TypeOf myControl Is BSRImage) Then
                    Dim CurrentControl As BSRImage = CType(myControl, BSRImage)

                    CurrentControl.Image = Nothing
                    CurrentControl.BackgroundImage = Nothing

                    CurrentControl.Dispose()
                    CurrentControl = Nothing
                End If

            Next
            ' XB 17/01/2014

            'TR 29/09/2011 - Set controls to nothing.to reduce memory use
            If Not myPosControlList Is Nothing Then
                For Each myControl As BSRImage In myPosControlList
                    myControl.Image = Nothing
                    myControl.BackgroundImage = Nothing

                    myControl.Dispose() ' XB 17/01/2014
                    myControl = Nothing
                Next
                myPosControlList.Clear()
            End If

            'AG 01/08/2012 - add more controls

            listReqElements = Nothing
            AllTubeSizeList = Nothing
            myRotorContentByPositionDSForm = Nothing
            mySelectedElementInfo = Nothing
            myPosControlList = Nothing
            sourceNode = Nothing
            RotorsTabs = Nothing
            'mdiAnalyzerCopy = Nothing 'not this variable

            ReagentPictureBox.Image = Nothing
            ReagentsTab.Appearance.PageClient.Image = Nothing
            SamplesTab.Appearance.PageClient.Image = Nothing
            bsCalibratorPictureBox.Image = Nothing
            bsControlPictureBox.Image = Nothing
            bsStatPictureBox.Image = Nothing
            bsRoutinePictureBox.Image = Nothing
            bsDilutedPictureBox.Image = Nothing
            bsTubeAddSolPictureBox.Image = Nothing
            AdditionalSolPictureBox.Image = Nothing
            bsDepletedPictureBox.Image = Nothing
            LowVolPictureBox.Image = Nothing
            NoInUsePictureBox.Image = Nothing
            LegendBarCodeErrorRGImage.Image = Nothing
            LegendUnknownImage.Image = Nothing
            SelectedPictureBox.Image = Nothing
            bsReagentsMoveFirstPositionButton.Image = Nothing
            bsReagentsDecreaseButton.Image = Nothing
            bsReagentsIncreaseButton.Image = Nothing
            bsReagentsMoveLastPositionButton.Image = Nothing
            bsReagentsRefillPosButton.Image = Nothing
            bsReagentsCheckVolumePosButton.Image = Nothing
            bsReagentsDeletePosButton.Image = Nothing
            bsSamplesMoveFirstPositionButton.Image = Nothing
            bsSamplesDecreaseButton.Image = Nothing
            bsSamplesIncreaseButton.Image = Nothing
            bsSamplesMoveLastPositionButton.Image = Nothing

            'AG 10/09/2013 - Do not release the buttons in the bottom bar (causes a ugly visual effect)
            'So comment these lines
            'bsSamplesAutoPosButton.Image = Nothing
            'bsSamplesAutoPosButton = Nothing
            'bsReagentAutoPosButton.Image = Nothing
            'bsReagentAutoPosButton = Nothing
            'BsForwardButton.Image = Nothing
            'BsForwardButton = Nothing
            'bsScanningButton.Image = Nothing
            'bsScanningButton = Nothing
            'BarcodeWarningButton.Image = Nothing
            'BarcodeWarningButton = Nothing
            'bsWarningsButton.Image = Nothing
            'bsWarningsButton = Nothing
            'bsPrintButton.Image = Nothing
            'bsPrintButton = Nothing
            'bsCheckRotorVolumeButton.Image = Nothing
            'bsCheckRotorVolumeButton = Nothing
            'bsSaveVRotorButton.Image = Nothing
            'bsSaveVRotorButton = Nothing
            'bsLoadVRotorButton.Image = Nothing
            'bsLoadVRotorButton = Nothing
            'bsResetRotorButton.Image = Nothing
            'bsResetRotorButton = Nothing
            'bsAcceptButton.Image = Nothing
            'bsAcceptButton = Nothing
            'AG 10/09/2013

            'DL 16/04/2013 Set timer control to nothing before call the garbage collector. BEGIN
            watchDogTimer.Enabled = False
            RemoveHandler watchDogTimer.Elapsed, AddressOf watchDogTimer_Timer
            watchDogTimer = Nothing
            'DL 16/04/2013 Set timer control to nothing before call the garbage collector. END

            'AG 02/01/2014 - BT #1433 (v211 patch2)
            autoWSCreationTimer.Enabled = False
            RemoveHandler autoWSCreationTimer.Elapsed, AddressOf autoWSCreationTimer_Timer
            autoWSCreationTimer = Nothing
            'AG 02/01/2014 - BT #1433

            '--- Detach variable defined using WithEvents ---
            bsScreenTimer = Nothing
            bsScreenToolTips = Nothing
            bsErrorProvider1 = Nothing
            FunctionalityArea = Nothing
            BarcodeWarningButton = Nothing
            bsAcceptButton = Nothing
            bsScanningButton = Nothing
            bsCheckRotorVolumeButton = Nothing
            bsWarningsButton = Nothing
            bsPrintButton = Nothing
            bsSaveVRotorButton = Nothing
            bsLoadVRotorButton = Nothing
            bsResetRotorButton = Nothing
            bsReagentAutoPosButton = Nothing
            bsSamplesAutoPosButton = Nothing
            RotorsTabs = Nothing
            SamplesTab = Nothing
            PanelControl1 = Nothing
            PanelControl2 = Nothing
            bsSamplesLegendGroupBox = Nothing
            bsTubeAddSolLabel = Nothing
            bsTubeAddSolPictureBox = Nothing
            bsLegendDilutedLabel = Nothing
            bsDilutedPictureBox = Nothing
            bsLegendRoutineLabel = Nothing
            bsLegendStatLabel = Nothing
            bsLegendControlsLabel = Nothing
            bsLegendCalibratorsLabel = Nothing
            bsRoutinePictureBox = Nothing
            bsStatPictureBox = Nothing
            bsControlPictureBox = Nothing
            bsCalibratorPictureBox = Nothing
            bsSamplesLegendLabel = Nothing
            bsSamplesPositionInfoGroupBox = Nothing
            SamplesStatusTextBox = Nothing
            bsSamplesStatusLabel = Nothing
            bsSamplesPositionInfoLabel = Nothing
            bsSamplesDeletePosButton = Nothing
            bsSamplesMoveLastPositionButton = Nothing
            bsSamplesRefillPosButton = Nothing
            bsSamplesIncreaseButton = Nothing
            bsTubeSizeComboBox = Nothing
            bsSamplesBarcodeTextBox = Nothing
            bsSamplesDecreaseButton = Nothing
            bsDiluteStatusTextBox = Nothing
            bsSampleTypeTextBox = Nothing
            bsSamplesMoveFirstPositionButton = Nothing
            bsSampleNumberTextBox = Nothing
            bsSampleIDTextBox = Nothing
            bsSampleContentTextBox = Nothing
            bsSampleRingNumTextBox = Nothing
            bsSampleCellTextBox = Nothing
            bsTubeSizeLabel = Nothing
            bsSamplesBarcodeLabel = Nothing
            bsDiluteStatusLabel = Nothing
            bsSampleTypeLabel = Nothing
            bsSamplesNumberLabel = Nothing
            bsSampleIDLabel = Nothing
            bsSamplesContentLabel = Nothing
            bsSamplesRingNumLabel = Nothing
            bsSamplesCellLabel = Nothing
            ReagentsTab = Nothing
            PanelControl6 = Nothing
            PanelControl7 = Nothing
            bsReagentsLegendGroupBox = Nothing
            LegReagentSelLabel = Nothing
            SelectedPictureBox = Nothing
            LegendUnknownImage = Nothing
            bsUnknownLabel = Nothing
            LegendBarCodeErrorRGImage = Nothing
            bsBarcodeErrorRGLabel = Nothing
            LowVolPictureBox = Nothing
            ReagentPictureBox = Nothing
            bsLegReagLowVolLabel = Nothing
            bsLegReagentLabel = Nothing
            bsLegReagAdditionalSol = Nothing
            bsLegReagNoInUseLabel = Nothing
            bsLegReagDepleteLabel = Nothing
            AdditionalSolPictureBox = Nothing
            NoInUsePictureBox = Nothing
            bsDepletedPictureBox = Nothing
            bsReagentsLegendLabel = Nothing
            bsReagentsPositionInfoGroupBox = Nothing
            bsReagStatusLabel = Nothing
            ReagStatusTextBox = Nothing
            bsReagentsCellTextBox = Nothing
            bsReagentsCellLabel = Nothing
            bsReagentsDeletePosButton = Nothing
            bsTeststLeftTextBox = Nothing
            bsReagentsRefillPosButton = Nothing
            bsCurrentVolTextBox = Nothing
            bsReagentsCheckVolumePosButton = Nothing
            bsTestsLeftLabel = Nothing
            bsCurrentVolLabel = Nothing
            bsBottleSizeComboBox = Nothing
            bsBottleSizeLabel = Nothing
            bsExpirationDateTextBox = Nothing
            bsReagentsPositionInfoLabel = Nothing
            bsReagentsMoveLastPositionButton = Nothing
            bsReagentsBarCodeTextBox = Nothing
            bsTestNameTextBox = Nothing
            bsReagentsIncreaseButton = Nothing
            bsReagentsNumberTextBox = Nothing
            bsReagentsDecreaseButton = Nothing
            bsReagentNameTextBox = Nothing
            bsReagentsMoveFirstPositionButton = Nothing
            bsReagentsContentTextBox = Nothing
            bsReagentsRingNumTextBox = Nothing
            bsExpirationDateLabel = Nothing
            bsReagentsBarCodeLabel = Nothing
            bsTestNameLabel = Nothing
            bsReagentsNumberLabel = Nothing
            bsReagentNameLabel = Nothing
            bsReagentsContentLabel = Nothing
            bsReagentsRingNumLabel = Nothing
            BsGroupBox1 = Nothing
            BsRefresh = Nothing
            BsLabel3 = Nothing
            BsLabel2 = Nothing
            BsLabel1 = Nothing
            BsRotationAngle = Nothing
            BsLeft = Nothing
            BsRotate2 = Nothing
            BsTop1 = Nothing
            BsRotate1 = Nothing
            BsLeft2 = Nothing
            BsTop2 = Nothing
            BsLeft1 = Nothing
            BsTop = Nothing
            bsElementsTreeView = Nothing
            bsRequiredElementsLabel = Nothing
            '------------------------------------------------

            'GC.Collect()

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ReleaseElement", EventLogEntryType.Error, False)
        End Try
    End Sub


    ''' <summary>
    ''' Read LISWITHFILES setting
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 10/05/2013
    ''' </remarks>
    Private Function IsLisWithFilesMode() As Boolean
        Try
            Dim lisWithFilesMode As Boolean = False
            Dim resultData As New GlobalDataTO
            Dim userSettings As New UserSettingsDelegate

            resultData = userSettings.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_WITHFILES_MODE.ToString)
            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                lisWithFilesMode = CType(resultData.SetDatos, Boolean)
            End If

            Return lisWithFilesMode

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IsLisWithFilesMode", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "IsLisWithFilesMode()", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try


    End Function

    ''' <summary>
    ''' Load icons in Reagents Legend Area.
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 12/03/2012
    ''' Modified by: SA 19/11/2013 - BT #1359 => In the PictureBox used to show the icon for Selected Position it is loaded now the 
    '''                                          new icon for In Process Position (Second Reagents and Washing Solution needed for second Reagent
    '''                                          contaminations in Reactions Rotor)
    ''' </remarks>
    Private Sub PrepareReagentLegendArea()
        Try
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

    ''' <summary>
    ''' Get names of all Icons needed for the Rotor Area and set Background Images of all PictureBox
    ''' in Legend frames
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 14/07/2010 
    ''' Modified by: AG 13/10/2010 - Added new icons 
    '''              TR 02/05/2011 - Added new icons
    '''              DL 02/09/2011 - Added new icons
    '''              DL 12/03/2012 - Added new icons
    '''              SA 19/11/2013 - BT #1359 => Load Icons for InProcess Reagents and/or Washing Solutions
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

            'Icons for Sample Class (used as Image in the Rotor and also in Legend Area)
            CALIB_IconName = GetIconName("CALIB")
            CTRL_IconName = GetIconName("CTRL")
            STATS_IconName = GetIconName("STATS")
            ROUTINES_IconName = GetIconName("ROUTINES")
            DILUTIONS_IconName = GetIconName("DILUTIONS")
            ADDSAMPLESOL_IconName = GetIconName("ADD_SAMPLE_SOL")

            'Load Icons for Legend Area 
            bsCalibratorPictureBox.ImageLocation = MyBase.IconsPath & CALIB_IconName
            bsControlPictureBox.ImageLocation = MyBase.IconsPath & CTRL_IconName
            bsStatPictureBox.ImageLocation = MyBase.IconsPath & STATS_IconName
            bsRoutinePictureBox.ImageLocation = MyBase.IconsPath & ROUTINES_IconName
            bsDilutedPictureBox.ImageLocation = MyBase.IconsPath & DILUTIONS_IconName
            bsTubeAddSolPictureBox.ImageLocation = MyBase.IconsPath & ADDSAMPLESOL_IconName

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

            'Icons for Reagents Legend Area
            LEGREAGENTREAGENT_IconName = GetIconName("LEG_BREAGENT")
            LEGREAGENTADDSOL_IconName = GetIconName("LEG_BWASH")
            LEGREAGENTDEPLETED_IconName = GetIconName("LEG_BDEPLETED")
            LEGREAGENTFEWVOL_IconName = GetIconName("LEG_BLOWER")
            LEGREAGENTNOTINUSE_IconName = GetIconName("LEG_BNOINUSE")
            LEGREAGENTBCERRORRG_IconName = GetIconName("LEG_BC_ERR")
            LEGBCREAGENTUNKNOWN_IconName = GetIconName("LEG_BC_UKN")
            '** Icon for In Process Reagents and/or Washing Solutions
            LEGREAGENTINPROCESS_IconName = GetIconName("LEG_BINPROCESS")

            '*******************************************
            '** ICONS FOR REQUIRED ELEMENTS TREE VIEW **
            '*******************************************

            'Besides CALIB_IconName, CTRL_IconName, STATS_IconName, ROUTINES_IconName, DILUTIONS_IconName, and ADDSAMPLESOL_IconName already obtained
            'for Samples Rotor and Legend Area, following Icons are needed for nodes in the Required Elements TreeView
            REAGENTS_IconName = GetIconName("REAGENTS")
            ADDSOL_IconName = GetIconName("ADD_SOL")
            SAMPLES_IconName = GetIconName("SAMPLESNODE")
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareIconNames ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareIconNames ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Method in charge to load the button images
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 05/05/2010
    ''' Modified by: DL 03/11/2010 - Changed the way the Image is loaded for Graphical Buttons
    '''              AG 25/11/2011 - Get the Barcode reader activation/deactivation status
    '''              JB 30/08/2012 - Hide Print Button
    ''' </remarks>
    Private Sub PrepareButtons(LisWithFilesMode As Boolean)
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            '*************************
            '* BUTTONS IN INFO AREA  *
            '*************************

            'MOVE TO FIRST ROTOR POSITION Buttons
            auxIconName = GetIconName("BACKWARDL")
            If (auxIconName <> "") Then
                bsSamplesMoveFirstPositionButton.Image = Image.FromFile(iconPath & auxIconName)
                bsReagentsMoveFirstPositionButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'MOVE TO PREVIOUS ROTOR POSITION Buttons
            auxIconName = GetIconName("LEFT")
            If (auxIconName <> "") Then
                bsReagentsDecreaseButton.Image = Image.FromFile(iconPath & auxIconName)
                bsSamplesDecreaseButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'MOVE TO NEXT ROTOR POSITION Buttons
            auxIconName = GetIconName("RIGHT")
            If (auxIconName <> "") Then
                bsSamplesIncreaseButton.Image = Image.FromFile(iconPath & auxIconName)
                bsReagentsIncreaseButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'MOVE TO LAST ROTO POSITION Buttons
            auxIconName = GetIconName("FORWARDL")
            If (auxIconName <> "") Then
                bsSamplesMoveLastPositionButton.Image = Image.FromFile(iconPath & auxIconName)
                bsReagentsMoveLastPositionButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'REFILL TUBE/BOTTLE Buttons
            auxIconName = GetIconName("REFILL")
            If (auxIconName <> String.Empty) Then
                bsSamplesRefillPosButton.Image = Image.FromFile(iconPath & auxIconName)
                bsReagentsRefillPosButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'CHECK ROTOR POSITION VOLUME Button (only for Reagents
            auxIconName = GetIconName("VOLUMETEST")
            If (auxIconName <> String.Empty) Then bsReagentsCheckVolumePosButton.Image = Image.FromFile(iconPath & auxIconName)

            'DELETE POSITION Buttons
            auxIconName = GetIconName("REMOVE")
            If (auxIconName <> "") Then
                bsSamplesDeletePosButton.Image = Image.FromFile(iconPath & auxIconName)
                bsReagentsDeletePosButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            '********************
            '* GENERAL BUTTONS  *
            '********************

            'SAMPLES AUTOPOSITIONING Button
            auxIconName = GetIconName("SAMPLEPOS")
            If (auxIconName <> "") Then bsSamplesAutoPosButton.Image = Image.FromFile(iconPath & auxIconName)

            'REAGENTS AUTOPOSITIONING Button
            auxIconName = GetIconName("REAGENTPOS")
            If (auxIconName <> "") Then bsReagentAutoPosButton.Image = Image.FromFile(iconPath & auxIconName)

            ' XB 25/03/2014 - Memory leaks - Improve Dispose (SplitContainer disallows dispose memory)
            ''EXPAND TREE VIEW Button
            'auxIconName = GetIconName("FORWARD")
            'If (auxIconName <> "") Then BsForwardButton.Image = Image.FromFile(iconPath & auxIconName)

            ''CONTRACT TREE VIEW Button
            'auxIconName = GetIconName("BACKWARD")
            'If (auxIconName <> "") Then BsBackwardButton.Image = Image.FromFile(iconPath & auxIconName)
            ' XB 25/03/2014

            'SCANNING Button
            auxIconName = GetIconName("BARCODE")
            If (auxIconName <> "") Then bsScanningButton.Image = Image.FromFile(iconPath & auxIconName)

            'INCOMPLETE SAMPLES Button (if LIS is implemented with FILES) or HOST QUERY Button (if LIS is implemented with ES)
            If (Not LisWithFilesMode) Then
                auxIconName = GetIconName("HQ")
            Else
                auxIconName = GetIconName("BCWARNING")
            End If
            If (auxIconName <> "") Then BarcodeWarningButton.Image = Image.FromFile(iconPath & auxIconName)

            'SHOW NOT POSITIONED ELEMENTS Button
            auxIconName = GetIconName("WARNING")
            If (auxIconName <> "") Then bsWarningsButton.Image = Image.FromFile(iconPath & auxIconName)

            'PRINT Button
            auxIconName = GetIconName("PRINT")
            If (auxIconName <> "") Then bsPrintButton.Image = Image.FromFile(iconPath & auxIconName)

            'CHECK ROTOR VOLUME Button
            auxIconName = GetIconName("VOLUME")
            If (auxIconName <> String.Empty) Then bsCheckRotorVolumeButton.Image = Image.FromFile(iconPath & auxIconName)

            'SAVE VIRTUAL ROTOR Button
            auxIconName = GetIconName("SAVE")
            If (auxIconName <> "") Then bsSaveVRotorButton.Image = Image.FromFile(iconPath & auxIconName)

            'LOAD VIRTUAL ROTOR Button
            auxIconName = GetIconName("OPEN")
            If (auxIconName <> "") Then bsLoadVRotorButton.Image = Image.FromFile(iconPath & auxIconName)

            'RESET ROTOR Button
            auxIconName = GetIconName("RESETROTOR")
            If (auxIconName <> "") Then bsResetRotorButton.Image = Image.FromFile(iconPath & auxIconName)

            'EXIT Button 
            auxIconName = GetIconName("ACCEPT1")
            If (auxIconName <> "") Then bsAcceptButton.Image = Image.FromFile(iconPath & auxIconName)

            'Get the Barcode reader activation/deactivation status
            Dim resultData As New GlobalDataTO
            Dim analyzerSettings As New AnalyzerSettingsDelegate

            '** Get Barcode Status for Samples Rotor
            resultData = analyzerSettings.GetAnalyzerSetting(Nothing, AnalyzerIDAttribute, GlobalEnumerates.AnalyzerSettingsEnum.SAMPLE_BARCODE_DISABLED.ToString)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim myDataSet As AnalyzerSettingsDS = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)
                If (myDataSet.tcfgAnalyzerSettings.Rows.Count > 0) Then
                    sampleBarcodeReaderOFF = CType(myDataSet.tcfgAnalyzerSettings(0).CurrentValue, Boolean)
                End If
            End If

            '** Get Barcode Status for Reagents Rotor
            resultData = analyzerSettings.GetAnalyzerSetting(Nothing, AnalyzerIDAttribute, GlobalEnumerates.AnalyzerSettingsEnum.REAGENT_BARCODE_DISABLED.ToString)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim myDataSet As AnalyzerSettingsDS = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)
                If (myDataSet.tcfgAnalyzerSettings.Rows.Count > 0) Then
                    reagentBarcodeReaderOFF = CType(myDataSet.tcfgAnalyzerSettings(0).CurrentValue, Boolean)
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill the structure AllTubeSizeList with information of all available Tubes and Bottles
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 21/12/2009
    ''' Modified by: SA 22/12/2009 - Calls to MessageBox.Show were replaced by calls to the generic function 
    '''                              ShowMessage
    ''' </remarks>
    Private Sub GetAllTubeSizes()
        Try
            Dim myGlobalDataTO As GlobalDataTO
            Dim myRCPDelegate As New WSRotorContentByPositionDelegate

            'Get all available Tubes/Bottles 
            myGlobalDataTO = myRCPDelegate.GetAllTubeSizes(Nothing, AnalyzerModelAttribute)
            If (Not myGlobalDataTO.HasError) Then
                AllTubeSizeList = CType(myGlobalDataTO.SetDatos, List(Of TubeSizeTO))
            Else
                ShowMessage(Me.Name & ".GetAllTubeSizes", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetAllTubeSizes", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetAllTubeSizes", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Using a Tube/Bottle as reference, gets the code of the next Tube/Bottle according the 
    ''' value of field Position (this field is sorted according the Tube/Bottle size or volume) 
    ''' </summary>
    ''' <param name="pRotorType">Rotor Type, which is used to look for Tubes and/or Bottles</param>
    ''' <param name="pRingNumber">Ring number.</param> 
    ''' <param name="pTubeCode">Tube/Bottle Code used as reference to get the next one</param>
    ''' <returns>The code of the Tube/Bottle in the next position regarding the reference Tube/Bottle.
    '''          When the reference Tube/Bottle is in the last position, then the next one will be the
    '''          one in the first position</returns>
    ''' <remarks>
    ''' Created by:  TR 21/12/2009 - Tested: OK
    ''' Modified by: SA 22/12/2009 - Calls to MessageBox.Show were replaced by calls to the generic function 
    '''                              ShowMessage
    '''              TR 13/01/2010 - Added the parameter RingNumber and the filter by the ringnumber to get the 
    '''                              currentBottleSize
    '''              TR 01/08/2012 - Set all lists to Nothing before leave the function
    ''' </remarks>
    Private Function GetNextBottleSize(ByVal pRotorType As String, ByVal pRingNumber As Integer, ByVal pTubeCode As String) As String
        Dim resultTubeCode As String = ""
        Try
            'Filter the AllTubeSizes list to get the size of the informed Tube/Bottle 
            Dim currentBottleSize As List(Of TubeSizeTO) = (From a In AllTubeSizeList _
                                                           Where a.RotorType = pRotorType _
                                                         AndAlso a.TubeCode = pTubeCode _
                                                         AndAlso a.RingNumber = pRingNumber _
                                                          Select a).ToList()

            If (currentBottleSize.Count > 0) Then
                If Not currentBottleSize.First().ManualUseFlag Then
                    'Do a second filter to get the next Tube/Bottle regarding the position of the reference one 
                    Dim nextBottleSize As List(Of TubeSizeTO) = (From b In AllTubeSizeList _
                                                                Where b.RotorType = pRotorType _
                                                              AndAlso b.Position > currentBottleSize.First().Position _
                                                              AndAlso b.ManualUseFlag = False _
                                                             Order By b.Volume _
                                                               Select b).ToList()

                    If (nextBottleSize.Count > 0) Then
                        resultTubeCode = nextBottleSize.First().TubeCode
                        nextBottleSize = Nothing
                    Else
                        'Get all the Tubes/Bottles for the informed Rotor Type to set the one in the first position
                        currentBottleSize = (From a In AllTubeSizeList _
                                             Where a.RotorType = pRotorType _
                                             AndAlso a.ManualUseFlag = False _
                                             Order By a.Volume _
                                             Select a).ToList()

                        'The Tube/Bottle in the first position is returned
                        If (currentBottleSize.Count > 0) Then
                            resultTubeCode = currentBottleSize.First().TubeCode
                        End If
                    End If
                End If
            End If
            currentBottleSize = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetNextBottleSize", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetNextBottleSize", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return resultTubeCode
    End Function

    ''' <summary>
    ''' Drops and positioning a Required Element from the TreeView to a Rotor Position (Ring-Cell)    
    ''' </summary>
    ''' <param name="NodeToDrag">Node in the Required Elements TreeView that contains the Element dragged
    '''                          and dropped in a Cell of the Rotor displayed in the Screen</param>
    ''' <param name="WorkSessionID">Work Session Identifier</param>
    ''' <param name="AnalyzerID">Analyzer Identifier</param>
    ''' <remarks>
    ''' Created by: AG 03/12/2009  - Tested: OK 14/12/2009
    ''' Modified by: SA 21/12/2009 - The way of calling function CreateLogActivity was changed
    '''                            - Calls to MessageBox.Show were replaced by calls to the generic function 
    '''                              ShowMessage
    '''              SA 10/01/2012 - Added code to managing controlled errors that can be returned by function ManualElementPositioning 
    '''                              (ROTOR_FULL or ROTOR_FULL_FOR_CALIBRATOR_KIT)
    ''' </remarks>
    Private Sub DragTreeViewElement(ByVal NodeToDrag As TreeNode, ByVal WorkSessionID As String, ByVal AnalyzerID As String)
        Try
            'From selected Tree Node convert Tag attribute to WSRequiredElementsTO
            'Dim reqElement As New WSRequiredElementsTO
            Dim reqElement As WSRequiredElementsTO
            reqElement = CType(NodeToDrag.Tag, WSRequiredElementsTO)

            'Element can be positioned?
            If (reqElement.Allow) Then
                'Element can be positioned on the selected rotor?
                Dim bottleElement As Integer = 0    '0 = Samples, 1= Bottles
                If (reqElement.TubeContent = "REAGENT") OrElse (reqElement.TubeContent = "WASH_SOL") OrElse _
                   (reqElement.TubeContent = "SPEC_SOL") Then bottleElement = 1

                Dim rotorOK As Boolean = False
                If (bottleElement = 1 AndAlso RotorsTabs.SelectedTabPage.Equals(ReagentsTab)) Then rotorOK = True
                If (bottleElement = 0 AndAlso RotorsTabs.SelectedTabPage.Equals(SamplesTab)) Then rotorOK = True

                If (rotorOK) Then
                    'Element is already positioned?
                    '(Samples: No multiple positioning allowed || Reagents & Additional Solutions: multiple positioning allowed)
                    If (bottleElement = 1) OrElse (reqElement.ElementStatus = "NOPOS" AndAlso bottleElement = 0) Then
                        'TODO: User Drag and Drop Element into a position on selected Rotor
                        'RingNumberForm & CellNumberForm contain position where user Drops

                        'Get Local Position information (Using LinQ)
                        Dim result As GlobalDataTO
                        Dim rotorContentDS As New WSRotorContentByPositionDS
                        Dim rotorContent As New WSRotorContentByPositionDelegate

                        Dim query = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                                    Where a.RotorType = myRotorTypeForm _
                                  AndAlso a.RingNumber = ringNumberForm _
                                  AndAlso a.CellNumber = cellNumberForm _
                                   Select a).First
                        rotorContentDS.twksWSRotorContentByPosition.ImportRow(query)

                        'Is position FREE?
                        'RH 13/09/2011 Introduced new conditions: Position with BarcodeStatus = NULL or EMPTY or UNKNOWN or ERROR)
                        If (rotorContentDS.twksWSRotorContentByPosition(0).Status = "FREE" OrElse _
                            String.IsNullOrEmpty(rotorContentDS.twksWSRotorContentByPosition(0).BarcodeStatus) OrElse _
                            rotorContentDS.twksWSRotorContentByPosition(0).BarcodeStatus = "EMPTY" OrElse _
                            rotorContentDS.twksWSRotorContentByPosition(0).BarcodeStatus = "UNKNOWN" OrElse _
                            rotorContentDS.twksWSRotorContentByPosition(0).BarcodeStatus = "ERROR") Then

                            result = Me.FillRotorContentByPositionRow(reqElement.ElementID, rotorContentDS.twksWSRotorContentByPosition(0))
                            If (result.HasError) Then
                                'Show Error Message on GlobalDataTO Error Message
                                ShowMessage(Me.Name & ".DragTreeViewElement", result.ErrorCode, result.ErrorMessage, Me)
                            Else
                                rotorContentDS = CType(result.SetDatos, WSRotorContentByPositionDS)

                                'AG 10/10/2011 - Inform the calibrator to position only the not positioned tubes when multicalibrator
                                If (reqElement.TubeContent = "CALIB" AndAlso rotorContentDS.twksWSRotorContentByPosition.Rows.Count > 0) Then
                                    rotorContentDS.twksWSRotorContentByPosition(0).BeginEdit()
                                    rotorContentDS.twksWSRotorContentByPosition(0).CalibratorID = reqElement.ElementCode
                                    rotorContentDS.twksWSRotorContentByPosition(0).EndEdit()
                                    rotorContentDS.twksWSRotorContentByPosition.AcceptChanges()
                                End If
                                'AG 10/10/2011

                                'RH 30/08/2011 Bug correction, because the previous code inverts the right logic.
                                'If there is an error, you should try the error, not the other way.
                                result = rotorContent.ManualElementPositioning(Nothing, rotorContentDS)
                                If (Not result.HasError) Then
                                    If (String.IsNullOrEmpty(result.ErrorCode)) Then
                                        'Convert GlobalDataTO.Dataset to a WSRotorContentByPosition Dataset
                                        rotorContentDS = CType(result.SetDatos, WSRotorContentByPositionDS)

                                        'Element was positioned?
                                        If (rotorContentDS.twksWSRotorContentByPosition.Rows.Count > 0) Then
                                            Me.UpdateRotorTreeViewArea(rotorContentDS)

                                            'TR 09/03/2012 Validate if there are more elements positioned on Rotor and validate the new status.
                                            If rotorContentDS.twksWSRotorContentByPosition(0).ElementStatus = "POS" Then
                                                SetElementsStatusByElementID(AnalyzerID, WorkSessionID, _
                                                            rotorContentDS.twksWSRotorContentByPosition(0).ElementID, "POS")


                                                ''Search by LINQ
                                                'Dim myPositionedElements As New List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                                                'myPositionedElements = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                                                '                        Where a.WorkSessionID = WorkSessionID AndAlso a.AnalyzerID = AnalyzerID _
                                                '                        AndAlso Not a.IsElementIDNull AndAlso _
                                                '                        a.ElementID = rotorContentDS.twksWSRotorContentByPosition(0).ElementID _
                                                '                        AndAlso a.ElementStatus = "INCOMPLETE" Select a).ToList

                                                'For Each myRotorContentRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myPositionedElements
                                                '    myRotorContentRow.ElementStatus = "POS"
                                                '    myRotorContentRow.AcceptChanges()
                                                'Next
                                            End If
                                            'TR 09/03/2012 -END.

                                        End If
                                    Else
                                        'SA 10/01/2012
                                        'There are not enough free position in the Rotor to place the selected Element
                                        '(this is a controlled error ---> HasError=False but the ErrorCode is informed)
                                        ShowMessage(Me.Name & ".DragTreeViewElement", result.ErrorCode, "", Me)
                                    End If
                                Else
                                    'Show Error Message on GlobalDataTO Error Message
                                    ShowMessage(Me.Name & ".DragTreeViewElement", result.ErrorCode, result.ErrorMessage, Me)
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DragTreeViewElement", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DragTreeViewElement", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Update on the local struture myRotorContentByPositionDSForm the element status
    ''' Search on the local structure the element positioned and update the element status.
    ''' This method only search for the elements with status INCOMPLETE.
    ''' </summary>
    ''' <param name="pAnalyzerID">Analyzer ID </param>
    ''' <param name="pWorkSession">WorkSession</param>
    ''' <param name="pElementID">ElementID </param>
    ''' <param name="pElementStatus">New Element Status </param>
    ''' <remarks>CREATED BY: TR 09/03/2012</remarks>
    Private Sub SetElementsStatusByElementID(ByVal pAnalyzerID As String, ByVal pWorkSession As String, _
                                             ByVal pElementID As Integer, ByVal pElementStatus As String)
        Try

            'Validate if there are more elements positioned on Rotor with status INCOMPLETE
            'Search by LINQ
            Dim myPositionedElements As New List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
            myPositionedElements = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                                    Where a.WorkSessionID = pWorkSession AndAlso a.AnalyzerID = pAnalyzerID _
                                    AndAlso Not a.IsElementIDNull AndAlso a.ElementID = pElementID _
                                     Select a).ToList

            'Set the new elemet status.
            For Each myRotorContentRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myPositionedElements
                myRotorContentRow.ElementStatus = pElementStatus
                myRotorContentRow.AcceptChanges()
            Next
            'TR 01/08/2012 -Set list to nothing 
            myPositionedElements = Nothing

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DragTreeViewElement", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DragTreeViewElement", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' Add and fill one row (in a typed DataSet WSRotorContentByPositionDS) with information of the 
    ''' Element that has been dragged 
    ''' </summary>
    ''' <param name="pElementID">Identifier of the Required Element that has been dragged</param>
    ''' <param name="RotorContentRow">Row of typed DataSet WSRotorContentByPositionDS used get
    '''                               the DataSet fields and create a new row by cloning it</param>
    ''' <returns>GlobalDataTO with DataSet = WSRotorContentByPositionDS </returns>
    ''' <remarks>
    ''' Created by: AG 03/12/2009  - Tested: OK 14/12/2009
    ''' Modified by: SA 21/12/2009 - The way of calling function CreateLogActivity was changed
    '''                            - Calls to MessageBox.Show were replaced by calls to the generic function 
    '''                              ShowMessage
    ''' </remarks>
    Private Function FillRotorContentByPositionRow(ByVal pElementID As Integer, _
                                                   ByVal RotorContentRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow) As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Try
            'Create a WSRotorContentbyPositionDS.twksWSRotorContentByPosition.ROW
            Dim newRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow

            'Clone received twksWSRotorContentByPositionRow
            newRow = RotorContentRow

            'Complete DataRow Information  with the ElementID and the TubeContent (From ListReqElement information)
            newRow.ElementID = pElementID
            newRow.TubeContent = (From a In listReqElements _
                                 Where a.ElementID = pElementID _
                                Select a.TubeContent).First

            'Set the current User and the current DateTime
            newRow.TS_User = GetApplicationInfoSession().UserName
            newRow.TS_DateTime = DateTime.Now

            'Create a RotorContentbyPositionDS and add the new row
            Dim resultDS As New WSRotorContentByPositionDS
            resultDS.twksWSRotorContentByPosition.ImportRow(newRow)

            resultData.SetDatos = resultDS
            resultData.HasError = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FillRotorContentByPositionRow", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillRotorContentByPositionRow", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

            resultData.HasError = True
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Get information of all positions in all Rotors of the specified Analyzer in the 
    ''' informed Work Session and update the TreeView and the Rotor Area 
    ''' </summary>
    ''' <param name="pWorkSessionID">Work Session Identifier</param>
    ''' <param name="pAnalizerID">Analyzer Identifier</param>
    ''' <param name="pReset">True if a reset rotor is done</param>
    ''' <param name="pResetRotorType">Reset rotor type, "" if no reset</param>
    ''' <remarks>
    ''' Created by:  VR - Tested: OK
    ''' Modified by: TR 15/12/2009 - Add the mandatory parameters pWorkSessionID and pAnalizerID
    '''                            - Add the UpdateRotorTreeViewArea to update the interface
    '''              SA 21/12/2009 - The way of calling function CreateLogActivity was changed
    '''                            - Calls to MessageBox.Show were replaced by calls to the generic function 
    '''                              ShowMessage
    '''              SA 27/10/2010 - Two calls to function UpdateRotorTreeViewArea are needed now: one for each
    '''                              RotorType
    '''              AG 11/11/2010 - Added pReset and pResetRotorType parameters
    '''              JV 03/12/2013 - BT #1384 ==> Show Status of Not InUse Bottles in Reagents Rotor when they are DEPLETED/FEW or when Barcode Status is ERROR/UNKNOWN
    ''' </remarks>
    Private Sub LoadRotorAreaInfo(ByVal pWorkSessionID As String, ByVal pAnalizerID As String, ByVal pReset As Boolean, ByVal pResetRotorType As String)
        Try
            If isClosingFlag Then Exit Sub ' XB 13/03/2014 - #1496 No refresh if screen is closing

            'Dim result As New GlobalDataTO
            Dim result As GlobalDataTO
            Dim myObj As New WSRotorContentByPositionDelegate

            If (WorkSessionStatusAttribute = "EMPTY" OrElse WorkSessionStatusAttribute = "OPEN") Then
                result = myObj.GetRotorContentPositionsResetDone(Nothing, pWorkSessionID, pAnalizerID)
            Else
                result = myObj.GetRotorContentPositions(Nothing, pWorkSessionID, pAnalizerID)
            End If

            If (Not result.HasError) Then
                myRotorContentByPositionDSForm = CType(result.SetDatos, WSRotorContentByPositionDS)

                'RH 13/10/2011 Update mySelectedElementInfo
                If Not mySelectedElementInfo Is Nothing Then
                    Dim query As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)

                    For Each row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In mySelectedElementInfo.twksWSRotorContentByPosition
                        query = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                                 Where a.RotorType = row.RotorType _
                                 AndAlso a.RingNumber = row.RingNumber _
                                 AndAlso a.CellNumber = row.CellNumber _
                                 Select a).ToList()

                        If (query.Count > 0) Then 'JV 03/12/2013: #1384 assure there are elements in the query
                            row.ItemArray = query.First().ItemArray.Clone()
                            row.Selected = True
                        End If
                    Next
                    'TR 01/08/2012 -Selet list to nothing 
                    query = Nothing
                End If

                'Update the Treeview and the Rotor Area
                If pResetRotorType = "SAMPLES" Then setControlPosToNothing = True Else setControlPosToNothing = False 'AG 09/12/2010
                UpdateRotorTreeViewArea(myRotorContentByPositionDSForm, "SAMPLES")

                If pResetRotorType = "REAGENTS" Then setControlPosToNothing = True Else setControlPosToNothing = False 'AG 09/12/2010
                UpdateRotorTreeViewArea(myRotorContentByPositionDSForm, "REAGENTS")
                setControlPosToNothing = False 'AG 09/12/2010

            Else
                ShowMessage(Me.Name & ".LoadRotorAreaInfo", result.ErrorCode, result.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.StackTrace + " - " + ex.HResult.ToString + "))", Me.Name & ".LoadRotorAreaInfo", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadRotorAreaInfo", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get the Expiration date from the reagent barcode information.
    ''' Remember to change this function on Montitor screen.
    ''' </summary>
    ''' <param name="pReagentBarcode"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' CREATED BY: TR 28/03/2014
    '''             TR 10/04/2014 -Initialize the ExpirationDate variable to min Date value.
    '''             XB 10/07/2014 - DateTime to Invariant Format (MM dd yyyy) - Bug #1673
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
                If myMonth <> "" OrElse myYear <> "" Then
                    ' XB 10/07/2014 - DateTime to Invariant Format - Bug #1673
                    'Date.TryParse("01" & "-" & myMonth & "-" & myYear, ExpirationDate)
                    ExpirationDate = CDate(myMonth & "-" & "01" & "-" & myYear).ToString(CultureInfo.InvariantCulture)
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetReagentExpDateFromBarCode", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetReagentExpDateFromBarCode", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return ExpirationDate
    End Function

    ''' <summary>
    ''' Get the Lot Number from the reagent barcode information.
    ''' </summary>
    ''' <param name="pReagentBarcode"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' CREATED BY: TR 31/03/2014
    ''' </remarks>
    Private Function GetReagentLotNumberFromBarCode(pReagentBarcode As String) As String
        Dim myLotNumber As String = ""
        Try
            If pReagentBarcode <> "" Then
                'The LotNumber  start on position 10 to 14 (5pos)
                myLotNumber = pReagentBarcode.Substring(9, 5)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetReagentLotNumberFromBarCode", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetReagentLotNumberFromBarCode", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myLotNumber
    End Function




    ''' <summary>
    ''' Fill one Node in the TreeView of Required Elements (used by function FillTreeView)
    ''' </summary>
    ''' <param name="ElementTO">Typed DataSet containing one Required Element</param>
    ''' <remarks>
    ''' Created by:  AG 30/11/2009 - Tested: OK
    ''' Modified by: SA 21/12/2009 - Added the Try/Catch and the code in the Catch
    '''              SA 22/12/2009 - Changed the Nodes.Add of the TreeView levels (different from Level 0, Fathers and Children)
    '''                              to shown the proper ICON for each one. Removed one parameter from the Nodes.Add of Level 0
    '''                              Elements (due to ElementIcon was passed twice)
    '''              TR 25/01/2010 - Add the parameter SelectedIcon settting the same value as imageIndex ont the property Nodes add.
    '''                              This is done to avoid the icon change whe the user select an element on the TreeView.
    ''' </remarks>
    Private Sub FillItem(ByVal ElementTO As WSRequiredElementsTO)
        Try
            'Dim myTreeNode As New TreeNode
            Dim myTreeNode As TreeNode

            'Check if exists Node of Level 0
            If Not String.IsNullOrEmpty(ElementTO.Father) Then
                'Find child Node for the Father (not Level 0)
                If (bsElementsTreeView.Nodes.Find(ElementTO.Father, True).Length <> 0) Then
                    'Add child Node
                    myTreeNode = CType(bsElementsTreeView.Nodes.Find(ElementTO.Father, True).First, TreeNode)

                    'TR 25/01/2010 Add the parameter for SeletedIcon = to ImageIndex
                    Dim myChildNode As TreeNode = myTreeNode.Nodes.Add(ElementTO.ElementTitle, ElementTO.ElementTitle, ElementTO.ElementIcon, ElementTO.ElementIcon)
                    myChildNode.Tag = ElementTO

                    'SGM 30-04-2013
                    If Not String.IsNullOrEmpty(ElementTO.ElementToolTip) Then
                        myChildNode.ToolTipText = ElementTO.ElementToolTip
                    End If

                Else    'Add Father Node (not Level 0)
                    'TR 25/01/2010 Add the parameter for SeletedIcon = to ImageIndex
                    bsElementsTreeView.Nodes.Add(ElementTO.ElementTitle, ElementTO.ElementTitle, ElementTO.ElementIcon, ElementTO.ElementIcon)
                    bsElementsTreeView.Nodes(ElementTO.ElementTitle).Tag = ElementTO
                End If

            Else    'Add Father Node (Level 0)
                'TR 25/01/2010 Add the parameter for SeletedIcon = to ImageIndex
                myTreeNode = bsElementsTreeView.Nodes.Add(ElementTO.ElementTitle, ElementTO.ElementTitle, ElementTO.ElementIcon, ElementTO.ElementIcon)
                bsElementsTreeView.Nodes(ElementTO.ElementTitle).Tag = ElementTO
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FillItem", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillItem", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Change the color of all elements in the TreeView positioned in the active Rotor from 
    ''' positioned (green) to not positioned (black). Also change the ElementStatus.
    ''' </summary>
    ''' <param name="MyNodes">Node collection.</param>
    ''' <remarks>
    ''' Created by:  TR 25/01/2010
    ''' Modified By: RH 11/02/2011 code optimization
    '''              RH 28/02/2011 ElementStatus update
    '''              RH 17/06/2011 Add TUBE_SPEC_SOL and TUBE_WASH_SOL
    '''              RH 28/07/2011 Change Color.Black by COLOR_NOPOS. Change Color.Green by COLOR_POS.
    '''              RH 28/07/2011 New version of the method that actually resets current node and
    '''                            it children. The previous version only works for leaves.
    ''' </remarks>
    Private Sub ResetTreeViewPositionedElements(ByVal MyNodes As TreeNodeCollection)
        Try
            'For Each MyNode As TreeNode In MyNodes
            '    If MyNode.Nodes.Count > 0 Then
            '        ResetTreeViewPositionedElements(MyNode.Nodes)
            '    Else
            '        Dim MyRequiredElementsTO As WSRequiredElementsTO = CType(MyNode.Tag, WSRequiredElementsTO)
            '        Dim myTubeContent As String = MyRequiredElementsTO.TubeContent

            '        If (myRotorTypeForm = "REAGENTS") Then
            '            If (myTubeContent = "REAGENT" OrElse myTubeContent = "SPEC_SOL" OrElse myTubeContent = "WASH_SOL") Then
            '                MyNode.ForeColor = COLOR_NOPOS
            '                MyRequiredElementsTO.ElementStatus = "NOPOS"
            '            End If
            '        ElseIf (myRotorTypeForm = "SAMPLES") Then
            '            If (myTubeContent = "CALIB" OrElse myTubeContent = "CTRL" OrElse myTubeContent = "PATIENT" OrElse myTubeContent = "TUBE_SPEC_SOL" OrElse myTubeContent = "TUBE_WASH_SOL") Then
            '                MyNode.ForeColor = COLOR_NOPOS
            '                MyRequiredElementsTO.ElementStatus = "NOPOS"
            '            End If
            '        End If
            '    End If
            'Next

            Dim MyRequiredElementsTO As WSRequiredElementsTO
            Dim myTubeContent As String

            For Each MyNode As TreeNode In MyNodes
                MyRequiredElementsTO = CType(MyNode.Tag, WSRequiredElementsTO)
                myTubeContent = MyRequiredElementsTO.TubeContent

                If Not String.IsNullOrEmpty(myTubeContent) Then
                    If (myRotorTypeForm = "REAGENTS") Then
                        If (myTubeContent = "REAGENT" OrElse myTubeContent = "SPEC_SOL" OrElse myTubeContent = "WASH_SOL") Then
                            MyNode.ForeColor = COLOR_NOPOS
                            MyRequiredElementsTO.ElementStatus = "NOPOS"
                        End If
                    ElseIf (myRotorTypeForm = "SAMPLES") Then
                        If (myTubeContent = "CALIB" OrElse myTubeContent = "CTRL" OrElse myTubeContent = "PATIENT" OrElse myTubeContent = "TUBE_SPEC_SOL" OrElse myTubeContent = "TUBE_WASH_SOL") Then
                            MyNode.ForeColor = COLOR_NOPOS
                            MyRequiredElementsTO.ElementStatus = "NOPOS"
                        End If
                    End If
                End If

                If MyNode.Nodes.Count > 0 Then
                    'Reset children nodes
                    ResetTreeViewPositionedElements(MyNode.Nodes)
                End If
            Next

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ResetTreeViewPositionedElements", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ResetTreeViewPositionedElements", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fills the ElementsRequired TreeView for a specific WorkSession
    ''' </summary>
    ''' <param name="pWorkSessionID">Work Session Identifier</param>
    ''' <param name="pResetRotorType">Different than "" on Reset Rotor or Load Virtual Rotor, equal "" on load screen</param>
    ''' <remarks>
    ''' Created by:  AG 30/11/2009 - Tested: OK
    ''' Modified by: SA 21/12/2009 - The way of calling function CreateLogActivity was changed
    '''                            - Calls to MessageBox.Show were replaced by calls to the generic function 
    '''                              ShowMessage
    '''              SA 16/07/2010 - Create an ImageList containing all Icons needed in the Required Elements
    '''                              TreeView and assign the created list to it (previously the list of Icons 
    '''                              was set by design 
    '''              SA 23/07/2010 - Load icons in ImageList FromStream instead of FromFile (new function
    '''                              AddIconToImageList was implemented)
    '''              SA 26/07/2010 - Call new function PendingPositionedElements to enable/disable the Warnings Button
    '''              AG 13/12/2010 - Add new parameter pResetRotorType and improve screen speed
    ''' </remarks>
    Private Sub FillTreeView(ByVal pWorkSessionID As String, ByVal pResetRotorType As String)
        Try
            'Dim returnData As New GlobalDataTO
            Dim returnData As GlobalDataTO
            Dim wsElements As New WSRequiredElementsDelegate

            returnData = wsElements.GetRequiredElementsDetails(Nothing, AnalyzerIDAttribute, pWorkSessionID, WorkSessionStatusAttribute)
            If (returnData.HasError) Then
                ShowMessage(Me.Name & ".FillTreeView", returnData.ErrorCode, returnData.ErrorMessage, Me)
            Else
                'Convert SetDatos to WSRequiredElementsDS
                Dim reqElementsDS As WSRequiredElementsTreeDS = DirectCast(returnData.SetDatos, WSRequiredElementsTreeDS)

                'AG 13/12/2010 - Execute only on create tree view
                If (String.IsNullOrEmpty(pResetRotorType)) Then 'RH 11/02/2011
                    'Create the Image List and assign it to the Required Elements TreeView
                    Dim ImgList As ImageList = New ImageList()

                    AddIconToImageList(ImgList, REAGENTS_IconName)
                    AddIconToImageList(ImgList, ADDSOL_IconName)
                    AddIconToImageList(ImgList, CTRL_IconName)
                    AddIconToImageList(ImgList, CALIB_IconName)
                    AddIconToImageList(ImgList, ROUTINES_IconName)
                    AddIconToImageList(ImgList, STATS_IconName)
                    AddIconToImageList(ImgList, DILUTIONS_IconName)
                    AddIconToImageList(ImgList, SAMPLES_IconName) ' dl 10/12/2010
                    AddIconToImageList(ImgList, ADDSAMPLESOL_IconName) 'RH 15/06/2011

                    bsElementsTreeView.ImageList = ImgList
                End If

                'Create the Node list
                returnData = wsElements.CreateNodeList(Nothing, reqElementsDS)

                If (returnData.HasError) Then
                    ShowMessage(Me.Name & ".FillTreeView", returnData.ErrorCode, returnData.ErrorMessage, Me)
                Else
                    listReqElements = CType(returnData.SetDatos, List(Of WSRequiredElementsTO))
                    If (listReqElements.Count = 0) Then Exit Try 'There are not Required Elements for the informed Work Session

                    'Fill the TreeView 
                    'AG 13/12/2010 - Execute only on create tree view
                    If (String.IsNullOrEmpty(pResetRotorType)) Then 'RH 11/02/2011
                        bsElementsTreeView.Nodes.Clear()
                        For Each listItems As WSRequiredElementsTO In listReqElements
                            FillItem(listItems)
                        Next
                    Else
                        ResetTreeViewPositionedElements(Me.bsElementsTreeView.Nodes)
                    End If
                    bsElementsTreeView.ExpandAll()
                End If

                ''Verify if Warnings Button has to be enabled or Disable, depending on if there are not positioned Elements 
                'PendingPositioningElements(False)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FillTreeView", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillTreeView", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Update the Rotor Area and TreeView Area. 
    ''' Update the information on the screen structure myRotorContentByPositionDSForm.
    ''' ** If the Rotor Position contains an Element that exists on the TreeView, then the color of the Element on the TreeView area is changed 
    '''    to show its current status. After this, on the Rotor Area, the corresponding Icon is set (according the TubeContent).   
    ''' ** If the Rotor Position contains an Element that does not exist on the TreeView, then it is a point of a MultiPoint Calibrator kit (different 
    '''    from the first one), and the Calibrator Icon has to be show corresponding position in the Rotor Area. 
    ''' ** If the Rotor Position does not contain a Required Work Session Element, then the Icon for IDLE Status should be shown in the corresponding 
    '''    position in the Rotor Area.     
    ''' </summary>
    ''' <param name="pWSRotorContentByPositionDS">Typed DataSet containing information of one or more Rotor positions that have been updated. If the position
    '''                                           corresponds to a Required Element of the Work Session, the DataSet also informs the Element Status</param>
    ''' <param name="pRotorType">Optional parameter for the RotorType; when not informed, value of the active Rotor Type is used</param>
    ''' <remarks>
    ''' Created by:  TR 03/12/2009 - Tested: OK
    ''' Modified by: SA 21/12/2009 - The way of calling function CreateLogActivity was changed
    '''                            - Calls to MessageBox.Show were replaced by calls to the generic function 
    '''                              ShowMessage 
    '''                            - Get the name of Calibrator Icon from the Preloaded Master Data
    '''              TR 15/01/2010 - Prepare the update in case the element is positioned on the rotor and not 
    '''                              on the treeview  
    '''              SA 14/07/2010 - Use the global variables with Icon Names instead of get then every time from the DB.
    '''                              When TubeContent is empty, call to UpdateRotorArea with IconName="" (to manage cells
    '''                              deletion)  
    '''              SA 27/10/2010 - Added optional parameter for the type of Rotor to process; when not informed, value of the
    '''                              active Rotor Type is used. Use LINQ to get only positions in the specified Rotor Type. Changes in 
    '''                              calls to function UpdateRotorArea due to the change in the parameter for the Control Collection. 
    '''                              Change in call to ChangeTreeRotorElementsStatus due to it needs a new parameter for the Rotor Type
    '''              RH 16/06/2011 - Added TUBE_SPEC_SOL and TUBE_WASH_SOL
    '''              SA 18/11/2013 - BT #1359 ==> Changed call to function GetIconNameByTubeContent: new parameter InProcessElement (obtained from the current
    '''                                           row in WSRotorContentByPositionDS) is informed
    '''              IT 04/06/2014 - BT #1644 ==> Changed call to function UpdateRotorArea: new type of the last parameter
    ''' </remarks>
    Private Sub UpdateRotorTreeViewArea(ByVal pWSRotorContentByPositionDS As WSRotorContentByPositionDS, Optional ByVal pRotorType As String = "")
        Try
            If (IsDisposed) Then Exit Sub 'IT 03/06/2014 - #1644 No refresh if screen is disposed

            Dim myGlobalDataTO As GlobalDataTO
            Dim myNotInUseRPDelegate As New WSNotInUseRotorPositionsDelegate()

            'Get positions in the specified Rotor Type
            If (String.IsNullOrEmpty(pRotorType)) Then pRotorType = myRotorTypeForm
            If (pWSRotorContentByPositionDS.twksWSRotorContentByPosition.Rows.Count > 0) Then
                If (pWSRotorContentByPositionDS.twksWSRotorContentByPosition.Rows.Count > 2) Then
                    bsElementsTreeView.Visible = False
                End If

                Dim lstPosInActiveRotor As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                lstPosInActiveRotor = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In pWSRotorContentByPositionDS.twksWSRotorContentByPosition _
                                       Where a.RotorType = pRotorType _
                                      Select a).ToList

                For Each rotorPosition As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In lstPosInActiveRotor
                    'Update information of the correspondent position in the Rotor Data Structure
                    If (UpdateRotorContentByPositionDSForm(rotorPosition)) Then
                        'Validate if there is a Required Element in the Rotor Position
                        If (Not rotorPosition.IsElementIDNull) Then
                            Dim myTreeNode As TreeNode
                            If (Not bsElementsTreeView Is Nothing) Then
                                'Update the correspondent Node in the Required Elements TreeView and get the information of the updated Node 
                                myTreeNode = ChangeTreeRotorElementsStatus(bsElementsTreeView.Nodes, True, rotorPosition, pRotorType)

                                'If the Element is a positioned Calibrator (TubeContent=CALIB; ElementStatus=POS) but it does not exist in the 
                                'Required Elements TreeView (Node.Tag=Nothing), then the Rotor Position corresponds to a point of a multipoint 
                                'Calibrator() that is not the first in the kit, and the Calibrator Icon has to be searched and placed in the 
                                'Rotor Position (the Icon Name is stored in the Node Tag, but in this case is informed only for the first point 
                                'of the Calibrator kit)
                                If (myTreeNode Is Nothing) Then
                                    'If the Element is not on the TreeView then it has to be a Calibrator or an Special or Washing Solution in Tube 
                                    If (rotorPosition.TubeContent = "CALIB") Then
                                        'Set the Calibrator Icon in the Rotor Position
                                        UpdateRotorArea(rotorPosition, CALIB_IconName, SamplesTab)

                                    ElseIf (rotorPosition.TubeContent = "TUBE_SPEC_SOL" OrElse rotorPosition.TubeContent = "TUBE_WASH_SOL") Then
                                        'Set the Tube Additional Solution Icon in the Rotor Position
                                        UpdateRotorArea(rotorPosition, ADDSAMPLESOL_IconName, SamplesTab)
                                    End If
                                End If
                            End If
                        Else
                            Dim auxIconPath As String = ""
                            Dim myRotorPicture As Object

                            If (pRotorType = "SAMPLES") Then
                                myRotorPicture = Me.SamplesTab
                            ElseIf (pRotorType = "REAGENTS") Then
                                myRotorPicture = Me.ReagentsTab
                            End If

                            'If Rotor Position is not FREE but it does not contain a positioned Element, then validate the tube content
                            If (Not rotorPosition.IsTubeContentNull AndAlso Not rotorPosition.TubeContent = "") Then
                                'If the tube content is Patient Sample, then it is needed to validate if it's a dilution
                                If (rotorPosition.TubeContent = "PATIENT") Then
                                    'Get the information for Not in use elements
                                    myGlobalDataTO = myNotInUseRPDelegate.GetPositionContent(Nothing, AnalyzerIDAttribute, rotorPosition.RotorType, _
                                                                                             rotorPosition.RingNumber, rotorPosition.CellNumber, WorkSessionIDAttribute)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        Dim myVirtualRotorPosititionsDS As VirtualRotorPosititionsDS = DirectCast(myGlobalDataTO.SetDatos, VirtualRotorPosititionsDS)

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
                                        ShowMessage(Me.Name, myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                                    End If
                                Else
                                    'Get the needed Icon when the TubeContent is not a Patient Sample
                                    auxIconPath = GetIconNameByTubeContent(rotorPosition.TubeContent, _
                                                                           rotorPosition.TubeType, _
                                                                           rotorPosition.RingNumber, _
                                                                           rotorPosition.InProcessElement)
                                End If

                                'Update the rotor area with the new icon path          
                                UpdateRotorArea(rotorPosition, auxIconPath, myRotorPicture)

                            Else
                                'Position is empty...icon is also empty, clean the cell
                                UpdateRotorArea(rotorPosition, auxIconPath, myRotorPicture)
                            End If
                        End If
                    End If
                Next

                LoadScreenStatus(WorkSessionStatusAttribute)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.StackTrace + " - " + ex.HResult.ToString + "))", Me.Name & ".UpdateRotorTreeViewArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UpdateRotorTreeViewArea", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            If (Not IsDisposed) Then
                bsElementsTreeView.Visible = True
            End If
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
    ''' Modified by: TR 18/01/2010 - Added the case for TubeContent=REAGENT (Tested: OK)
    '''              SA 14/07/2010 - Use the global variables with Icon Names instead of get then every time from the DB 
    '''              AG 13/10/2010 - Added new parameters TubeSize and RingNumber used to return a different IconName for Reagents Rotor, 
    '''                              depending on Bottle size 
    '''              RH 17/06/2011 - Added cases for TUBE_SPEC_SOL and TUBE_WASH_SOL.
    '''              DL 07/09/2011 - Added checking of Barcode Status
    '''              SA 18/11/2013 - Removed parameter pBarCodeStatus due to it is not used
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

    ''' <summary>
    ''' Configures and prepares the CellControls (BSRImage) of the specified RotorType
    ''' </summary>
    ''' <param name="pRotorType">RotorType to be prepared</param>
    ''' <remarks>
    ''' Created by: RH 04/08/2011
    ''' </remarks> 
    Private Sub PreparePositionsControls(ByVal pRotorType As String)
        Try
            Dim PosControlList As Control.ControlCollection

            If pRotorType = "SAMPLES" Then
                PosControlList = SamplesTab.Controls
            Else
                PosControlList = ReagentsTab.Controls
            End If

            For Each myControl As Control In PosControlList
                If TypeOf myControl Is BSRImage Then
                    CType(myControl, BSRImage).Image = Nothing
                    myControl.BackgroundImage = Nothing
                End If
            Next

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PreparePositionsControls", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PreparePositionsControls", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try
    End Sub

    ''' <summary>
    ''' To improve the load of images in both Rotors (avoiding recursive calls through all Controls Collections of the screen)
    ''' </summary>
    ''' <remarks>
    ''' Created by: SA 07/02/2014 - BT #1496
    ''' </remarks>
    Private Sub PreparePositionsControlsNEW()
        Try
            Dim lastIndex As Integer = 0
            myPosControlList = New List(Of BSRImage)

            'Get the list of all BSRImage Controls in Samples Rotor
            For Each myPosControl As Control In SamplesTab.Controls
                If (myPosControl.Controls.Count = 0) Then
                    'Get the control type to validate if it will go to our list (only for PictureBox)
                    myPosControlList.Add(myPosControl)

                    lastIndex = (myPosControlList.Count - 1)
                    myPosControlList(lastIndex).Tag = myPosControlList(lastIndex).Name.Replace("Sam", String.Empty).Insert(1, ",")
                    myPosControlList(lastIndex).Image = Nothing
                    myPosControlList(lastIndex).BackgroundImage = Nothing
                    myPosControlList(lastIndex).BackgroundImageLayout = ImageLayout.None
                    myPosControlList(lastIndex).InitialImage = Nothing
                    myPosControlList(lastIndex).BackColor = Color.Transparent
                    myPosControlList(lastIndex).SizeMode = PictureBoxSizeMode.CenterImage
                    myPosControlList(lastIndex).AllowDrop = True
                End If
            Next

            'Get the list of all BSRImage Controls in Reagents Rotor
            For Each myPosControl As Control In ReagentsTab.Controls
                If (myPosControl.Controls.Count = 0) Then
                    'Get the control type to validate if it will go to our list (only for PictureBox)
                    myPosControlList.Add(myPosControl)

                    lastIndex = (myPosControlList.Count - 1)
                    myPosControlList(lastIndex).Tag = myPosControlList(lastIndex).Name.Replace("Reag", String.Empty).Insert(1, ",")
                    myPosControlList(lastIndex).Image = Nothing
                    myPosControlList(lastIndex).BackgroundImage = Nothing
                    myPosControlList(lastIndex).BackgroundImageLayout = ImageLayout.None
                    myPosControlList(lastIndex).InitialImage = Nothing
                    myPosControlList(lastIndex).BackColor = Color.Transparent
                    myPosControlList(lastIndex).SizeMode = PictureBoxSizeMode.CenterImage
                    myPosControlList(lastIndex).AllowDrop = True
                End If
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PreparePositionsControlsNEW", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PreparePositionsControlsNEW", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Create a list of Position Controls: select all the BSRImage Controls used for each Rotor Position
    ''' </summary>
    ''' <param name="MyControlCollection">Split Container where the PictureBox are located</param>
    ''' <returns>List of all Rotor Position Controls (BSRImage)</returns>
    ''' <remarks>
    ''' Created by:  TR - Tested: OK
    ''' Modified by: SA 21/12/2009 - Added the Try/Catch and the code in the Catch
    ''' Modified by AG 05/10/2010 - change picturebox for BSRImage 
    ''' Modified by RH 11/02/2011 - Make some code optimizations
    '''             RH 14/02/2011 - Now Returns BSRImage instead of Control
    ''' </remarks>
    Private Function CreatePosControlList(ByVal MyControlCollection As Control.ControlCollection) As List(Of BSRImage)
        'Private Function CreatePosControlList(ByVal MyControlCollection As SplitContainer.ControlCollection) As List(Of Control)
        'Dim myPosControlList As New List(Of Control)
        Dim myPosControlList As New List(Of BSRImage)
        Try
            'Dim myControlTMP As Control.ControlCollection

            'Get all the Position Controls (Pos PictureBox) and create a list
            For Each myPosControl As Control In MyControlCollection
                If (myPosControl.Controls.Count > 0) Then
                    'If (myPosControl.GetType().Name <> "SplitContainer") Then
                    If Not TypeOf myPosControl Is SplitContainer Then 'RH 11/02/2011
                        myPosControlList.AddRange(CreatePosControlList(myPosControl.Controls))
                    Else
                        'myControlTMP = CType(myPosControl, SplitContainer).Controls
                        'RH 11/02/2011 There is no need to make the typecast
                        'myControlTMP = myPosControl.Controls

                        'myPosControlList = CreatePosControlList(CType(myControlTMP, Control.ControlCollection))
                        'RH 11/02/2011 There is no need to make the typecast. myControlTMP IS BY DEFINITION Control.ControlCollection
                        'myPosControlList = CreatePosControlList(myControlTMP)

                        'RH 11/02/2011 There is no need to use a temporal variable so, myControlTMP gets out.
                        myPosControlList = CreatePosControlList(myPosControl.Controls)

                    End If
                Else
                    'Get the control type to validate if it will go to our list (only for PictureBox)
                    'If (myPosControl.GetType().Name = "BSRImage") Then
                    If TypeOf myPosControl Is BSRImage Then 'RH 11/02/2011
                        myPosControlList.Add(myPosControl)
                    End If
                End If
            Next

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CreatePosControlList", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CreatePosControlList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myPosControlList
    End Function

    ''' <summary>
    ''' Search for a position in the Screen Structure myRotorContentByPositionDSForm to update it with the values
    ''' contained in the row received as parameter
    ''' </summary>
    ''' <param name="pRotorContentByPosRow">Row containing the values to update the local WSRotorContentByPositionDS</param>
    ''' <returns>True when update was OK; otherwise it returns False</returns>
    ''' <remarks>
    ''' Created by:  TR 26/09/2009
    ''' Modified by: SA 21/12/2009 - Calls to MessageBox.Show were replaced by calls to the generic function 
    '''                              ShowMessage 
    '''              TR 08/01/2010 - Correct the null values validation (Tested: OK)
    ''' </remarks>
    Private Function UpdateRotorContentByPositionDSForm(ByVal pRotorContentByPosRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow) As Boolean
        Dim result As Boolean = False
        Try
            If Not myRotorContentByPositionDSForm Is Nothing Then
                'Implement LINQ to get the position to update 
                Dim query As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                query = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                        Where a.RotorType = pRotorContentByPosRow.RotorType _
                      AndAlso a.RingNumber = pRotorContentByPosRow.RingNumber _
                      AndAlso a.CellNumber = pRotorContentByPosRow.CellNumber _
                       Select a).ToList()

                For Each myrow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In query
                    myrow.ItemArray = pRotorContentByPosRow.ItemArray.Clone()
                    'myrow.Selected = pRotorContentByPosRow.Selected
                Next
                result = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdateRotorContentByPositionDSForm ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UpdateRotorContentByPositionDSForm", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            result = False
        End Try
        Return result
    End Function

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
    ''' Modified by: SA 14/07/2010 - Use the global variables for Icon Names instead the hardcode name
    '''              TR 28/01/2010 - Added functionality for Tubes/Bottles with DEPLETED Status
    '''              AG 05/10/2010 - Changed all PictureBox for BSRImage
    '''              AG 13/10/2010 - Added parameters pRotorType and TubeType. When Rotor Type is REAGENTS, the Image is changed instead of the Background
    '''              AG 12/04/2011 - Added functionality for Tubes/Bottles with FEW Status
    '''              DL 07/09/2011 - Added parameter for BarcodeStatus and functionality to set the Background also according its value
    '''              TR 28/09/2012 - Added functionality for Tubes/Bottles with LOCKED Status (same behaviour used for DEPLETED Status)
    '''              SA 18/11/2013 - BT #1359 => Added new optional parameter pInProcessBottle. Code changed to call a different function depending on the 
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

            'Select Case pStatus
            '    Case "NO_INUSE"
            '        If (pRotorType <> "REAGENTS") Then
            '            pPositionControl.BackgroundImage = Image.FromFile(MyBase.IconsPath & NOTINUSE_IconName)

            '        ElseIf (pTubeType = "BOTTLE2" OrElse pTubeType = "BOTTLE1") Then
            '            If (pRingNumber = 1) Then
            '                ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLNUSESMALLR1_IconName)
            '            Else
            '                ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLNUSESMALLR2_IconName)
            '            End If

            '        ElseIf (pTubeType = "BOTTLE3") Then
            '            ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLNUSEBIGR2_IconName)
            '        End If
            '        Exit Select

            '    Case "DEPLETED", "LOCKED"
            '        If (pRotorType <> "REAGENTS") Then
            '            pPositionControl.BackgroundImage = Image.FromFile(MyBase.IconsPath & DEPLETED_IconName)

            '        ElseIf (pTubeType = "BOTTLE2" OrElse pTubeType = "BOTTLE1") Then
            '            If (pRingNumber = 1) Then
            '                ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLDEPLETSMALLR1_IconName)
            '            Else
            '                ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLDEPLETSMALLR2_IconName)
            '            End If

            '        ElseIf (pTubeType = "BOTTLE3") Then
            '            ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLDEPLBIGR2_IconName)
            '        End If
            '        Exit Select

            '    Case "PENDING"
            '        If (pRotorType = "SAMPLES") Then
            '            pPositionControl.BackgroundImage = Image.FromFile(MyBase.IconsPath & PENDING_IconName)
            '            pPositionControl.BackgroundImageLayout = ImageLayout.Stretch
            '        End If
            '        Exit Select

            '    Case "INPROCESS"
            '        If (pRotorType = "SAMPLES") Then
            '            pPositionControl.BackgroundImage = Image.FromFile(MyBase.IconsPath & INPROGRESS_IconName)
            '            pPositionControl.BackgroundImageLayout = ImageLayout.Stretch
            '        End If
            '        Exit Select

            '    Case "FINISHED"
            '        If (pRotorType = "SAMPLES") Then
            '            pPositionControl.BackgroundImage = Image.FromFile(MyBase.IconsPath & FINISHED_IconName)
            '            pPositionControl.BackgroundImageLayout = ImageLayout.Stretch
            '        End If
            '        Exit Select

            '    Case "BARERROR"
            '        If (pRotorType = "SAMPLES") Then
            '            pPositionControl.Image = Image.FromFile(MyBase.IconsPath & BTLSAMPLEBCERR_IconName)
            '            pPositionControl.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLEBCERR_IconName)
            '            pPositionControl.BackgroundImageLayout = ImageLayout.Stretch
            '        End If
            '        Exit Select

            '    Case "FEW"
            '        If (pRotorType <> "REAGENTS") Then
            '            pPositionControl.BackgroundImage = Image.FromFile(MyBase.IconsPath & FEWVOL_IconName)

            '        ElseIf (pTubeType = "BOTTLE2" OrElse pTubeType = "BOTTLE1") Then
            '            If pRingNumber = 1 Then
            '                ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLFEWSMALLR1_IconName)
            '            Else
            '                ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLFEWSMALLR2_IconName)
            '            End If

            '        ElseIf (pTubeType = "BOTTLE3") Then
            '            ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLFEWBIGR2_IconName)
            '        End If
            '        Exit Select

            '    Case Else
            '        pPositionControl.BackgroundImage = Nothing
            'End Select

            'If (Not pBarCodeStatus Is String.Empty) Then
            '    Select Case (pRotorType)
            '        Case "REAGENTS"
            '            If (pBarCodeStatus = "ERROR") Then
            '                'TR 08/09/2011 -Validate if status is free to set by default the bottle depending the ring number
            '                'if Ring 1 20 ml if ring 2 then 60 ml.
            '                If (pStatus = "FREE") Then
            '                    If (pRingNumber = 1) Then
            '                        ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCERRSMALLR1_IconName)
            '                    Else
            '                        If pTubeType = "BOTTLE2" Then
            '                            ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCERRSMALLR2_IconName)

            '                        ElseIf pTubeType = "BOTTLE3" Then
            '                            ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCERRBIGR2_IconName)
            '                        Else
            '                            pTubeType = "BOTTLE3"
            '                            ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCERRBIGR2_IconName)
            '                        End If
            '                    End If
            '                Else
            '                    If (pTubeType = "BOTTLE2" OrElse pTubeType = "BOTTLE1") Then
            '                        If (pRingNumber = 1) Then
            '                            ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCERRSMALLR1_IconName)
            '                        Else
            '                            ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCERRSMALLR2_IconName)
            '                        End If

            '                    ElseIf (pTubeType = "BOTTLE3") Then
            '                        ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCERRBIGR2_IconName)
            '                    End If
            '                End If
            '            ElseIf (pBarCodeStatus = "UNKNOWN") Then
            '                If (pTubeType = "BOTTLE2" OrElse pTubeType = "BOTTLE1") Then
            '                    If (pRingNumber = 1) Then
            '                        ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCUKNSMALLR1_IconName)
            '                    Else
            '                        ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCUKNSMALLR2_IconName)
            '                    End If

            '                ElseIf (pTubeType = "BOTTLE3") Then
            '                    ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLBCUKNBIGR2_IconName)
            '                End If
            '            End If

            '        Case "SAMPLES"
            '            If (pBarCodeStatus = "ERROR") Then
            '                pPositionControl.BackgroundImage = Nothing
            '                ChangeControlPositionImage(pPositionControl, MyBase.IconsPath & BTLSAMPLEBCERR_IconName)
            '            End If
            '    End Select
            'End If
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

                        '' XB 19/12/2013 - add log traces
                        'Dim myLogAcciones As New ApplicationLogManager()
                        'myLogAcciones.CreateLogActivity("Not expected Case [" & pStatus & "] to paint on Rotor", Me.Name & ".SetPosControlBackGroundForSAMPLESRotor", EventLogEntryType.Information, False)

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

    ''' <remarks>
    ''' Created by:  TR - Tested: OK
    ''' Modified by: SA 21/12/2009 - Calls to MessageBox.Show were replaced by calls to the generic function 
    '''                              ShowMessage 
    '''                            - Add code (commented) to get the Icon Path from the AppSettings file 
    '''              SA 23/12/2009 - Added code for updation of Reagents Rotor
    '''              TR 05/01/2010 - Validate if the iconName is Empty 
    '''              TR 25/01/2010 - Validate the NO_INUSE status to set the corresponding icon.
    '''              SA 14/07/2010 - When there is not IconName, set ImageLocation to Nothing (instead of Image);
    '''                              besides, finish the loop when the Control is found in the Controls Collection 
    '''              AG 05/10/2010 - Changed PictureBox for BSRImage    
    '''              SA 27/10/2010 - Removed parameter for the ControlCollection of the SplitContainer and added a new 
    '''                              parameter of type ControlCollection; in this way, by passing the PictureBox of the 
    '''                              active Rotor, the collection is loaded only with its BSRImages, not with all the screen
    '''                              controls as before and due to that recursivity is not needed. Additionally, LINQ is used
    '''                              to search the BSRImage for the specified Rotor Cell
    '''              SA 18/11/2013 - BT #1359 ==> Changed call to function GetIconNameByTubeContent: new parameter InProcessElement (obtained from the current
    '''                                           row in WSRotorContentByPositionDS) is informed
    '''              AG 25/11/2013 - #1359 - Inform parameter InProcessElement on call method SetPosControlBackGround
    '''              IT 04/06/2014 - #1644 - Modified the type of last parameter.
    ''' </remarks>
    Private Sub UpdateRotorArea(ByVal pRotorContenByPosRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow, _
                                ByVal pIconName As String, ByVal pRotorControl As Control)
        Try

            If (pRotorControl Is Nothing) Then Exit Sub

            Dim pRotorControlCollection As Control.ControlCollection
            pRotorControlCollection = pRotorControl.Controls

            'Change the Icon name for the Reagents and Additional solutions
            Dim myIconName As String = pIconName
            If (myIconName = REAGENTS_IconName OrElse myIconName = ADDSOL_IconName) Then
                myIconName = GetIconNameByTubeContent(pRotorContenByPosRow.TubeContent, _
                                                      pRotorContenByPosRow.TubeType, _
                                                      pRotorContenByPosRow.RingNumber, _
                                                      pRotorContenByPosRow.InProcessElement)
            End If

            Dim myControls As Control.ControlCollection
            If (Not pRotorControlCollection Is Nothing) Then
                myControls = pRotorControlCollection

                Dim myControlName As String = String.Empty
                If (pRotorContenByPosRow.RotorType = "SAMPLES") Then
                    myControlName = String.Format("Sam{0}{1}", pRotorContenByPosRow.RingNumber, pRotorContenByPosRow.CellNumber)
                Else
                    myControlName = String.Format("Reag{0}{1}", pRotorContenByPosRow.RingNumber, pRotorContenByPosRow.CellNumber)
                End If

                Dim lstRotorControl As List(Of Control) = (From a As Control In myControls _
                                                          Where a.Name = myControlName _
                                                         Select a).ToList()
                If (lstRotorControl.Count = 1) Then
                    Dim myBSRImage As BSRImage = CType(lstRotorControl(0), BSRImage)

                    If (myIconName <> String.Empty) Then
                        Me.ChangeControlPositionImage(myBSRImage, MyBase.IconsPath & myIconName)
                    Else
                        'Rotor Position is empty (there is not an element placed in it)
                        If (pRotorContenByPosRow.RotorType = "SAMPLES") Then
                            Me.ChangeControlPositionImage(myBSRImage, Nothing, True)

                        ElseIf (pRotorContenByPosRow.RotorType = "REAGENTS") Then
                            If (pRotorContenByPosRow.RingNumber = 1) Then
                                Me.ChangeControlPositionImage(myBSRImage, MyBase.IconsPath & BOTTLE2_NOTHING_IconName, True)
                            Else
                                Me.ChangeControlPositionImage(myBSRImage, MyBase.IconsPath & BOTTLE3_NOTHING_IconName, True)
                            End If
                        End If

                        Me.ChangeControlPositionImage(myBSRImage, Nothing, True)
                    End If

                    'AG 25/11/2013 - #1359 Inform the field InProcessElement
                    SetPosControlBackGround(myBSRImage, pRotorContenByPosRow.Status, pRotorContenByPosRow.RotorType, _
                                            pRotorContenByPosRow.TubeType, pRotorContenByPosRow.RingNumber, pRotorContenByPosRow.BarcodeStatus, pRotorContenByPosRow.InProcessElement)
                End If
                Application.DoEvents()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdateRotorArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UpdateRotorArea", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Update the Status of a Required Element. Node ForeColor in the TreeView is changed to Green when the Element 
    ''' is positioned on the current Rotor 
    ''' </summary>
    ''' <param name="pMyNodes">Collection containing all Nodes of the TreeView of Required Work Session Elements</param>
    ''' <param name="pPositionedStatus">indicate if the status is positoned or not.</param>
    ''' <param name="pRotorContenByPosRow">Row containing information of a Rotor Position</param>
    ''' <returns>Updated TreeView Node</returns>
    ''' <remarks>
    ''' Created by:  TR 01/12/2009 - Tested: OK
    ''' Modified by: SA 21/12/2009 - Calls to MessageBox.Show were replaced by calls to the generic function 
    '''                              ShowMessage 
    '''              TR 11/01/2010 - Added new parameter to indicate if the Element is positioned or not, to set the node 
    '''                              backcolor in the proper way
    '''              SA 14/07/2010 - Besides the Node backcolor, update also field ElementStatus in the WSRequiredElementsTO
    '''                              stored in the Node Tag 
    '''              SA 27/10/2010 - Added new parameter for the Rotor Type (needed to known which Rotor Area has to be updated)
    '''              RH 28/07/2011 - Change Color.Black by COLOR_NOPOS. Change Color.Green by COLOR_POS.
    '''              XB 11/03/2014 - Sets an additional protection when one thread disposes (exit screen) an object which another thread (refresh) wants to use - #1523
    ''' </remarks>
    Private Function ChangeTreeRotorElementsStatus(ByVal pMyNodes As TreeNodeCollection, ByVal pPositionedStatus As Boolean, _
                                                   ByVal pRotorContenByPosRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow, _
                                                   ByVal pRotorType As String) As TreeNode
        Dim result As TreeNode = Nothing

        Try
            'Go through each Required Work Session Element (each Node on the TreeNodeCollection)
            If pMyNodes Is Nothing Then Exit Try ' XB 11/03/2014 - #1523 No refresh if screen is closing
            For Each myNode As TreeNode In pMyNodes
                'Validate if this Node is the one we are looking for to update its status 
                Dim myNodeTag As WSRequiredElementsTO = CType(myNode.Tag, WSRequiredElementsTO) 'RH 11/02/2010

                'If (CType(myNode.Tag, WSRequiredElementsTO).ElementID = pRotorContenByPosRow.ElementID) Then
                If pRotorContenByPosRow Is Nothing Then Exit Try ' XB 11/03/2014 - #1523 No refresh if screen is closing
                If (myNodeTag.ElementID = pRotorContenByPosRow.ElementID) Then 'RH 11/02/2010
                    'Update the Element Status in the Element stored in the Node Tag
                    'CType(myNode.Tag, WSRequiredElementsTO).ElementStatus = pRotorContenByPosRow.ElementStatus
                    myNodeTag.ElementStatus = pRotorContenByPosRow.ElementStatus 'RH 11/02/2010

                    'Now change the Node background....
                    'If (pPositionedStatus)  Then 'AG 12/04/2011
                    If (pPositionedStatus) AndAlso myNodeTag.ElementStatus = "POS" Then
                        'AG 10/10/2011 - When calibrator multipoint check if all points are positioned. Else mark element as NOPOS
                        'myNode.ForeColor = COLOR_POS
                        If myNodeTag.TubeContent = "CALIB" Then
                            Dim myWSReqElementDelegate As New WSRequiredElementsDelegate
                            'Dim myGlobalDataTO As New GlobalDataTO
                            Dim myGlobalDataTO As GlobalDataTO
                            Dim calibratorFullPositionedFlag As Boolean = True
                            myGlobalDataTO = myWSReqElementDelegate.GetMultiPointCalibratorElements(Nothing, WorkSessionIDAttribute, _
                                                                    myNodeTag.ElementID, myNodeTag.ElementCode)
                            'If Not myGlobalDataTO.HasError Then
                            If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then ' XB 11/03/2014 - #1523 No refresh if screen is closing
                                Dim myReqElementDS As WSRequiredElementsDS
                                myReqElementDS = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsDS)
                                If myReqElementDS.twksWSRequiredElements.Count > 0 Then
                                    Dim linqRes As List(Of WSRequiredElementsDS.twksWSRequiredElementsRow)
                                    linqRes = (From a As WSRequiredElementsDS.twksWSRequiredElementsRow In myReqElementDS.twksWSRequiredElements _
                                               Where a.ElementStatus = "NOPOS" Select a).ToList()
                                    If linqRes.Count > 0 Then calibratorFullPositionedFlag = False
                                End If
                            End If

                            If calibratorFullPositionedFlag Then
                                myNode.ForeColor = COLOR_POS
                            Else
                                myNode.ForeColor = COLOR_NOPOS
                                myNodeTag.ElementStatus = "NOPOS" 'Update MyNodeTag.ElementStatus property
                            End If

                        Else
                            myNode.ForeColor = COLOR_POS
                        End If
                        'AG 10/10/2011
                    Else
                        myNode.ForeColor = COLOR_NOPOS
                        If Not pPositionedStatus Then myNodeTag.ElementStatus = "NOPOS" 'Update MyNodeTag.ElementStatus property
                    End If

                    'The updated Node will be the one returned
                    'result = myNode.Clone()

                    'RH 01/03/2011 Don't understand why return a copy of the node and not a reference to it.
                    'If return a copy, most of them (the returned copies) will soon remain unreferenced and so,
                    ' garbage collectable. So, return a reference instead.
                    result = myNode

                    'RH 04/08/2011
                    Dim myRotorPicture As Object
                    If (pRotorType = "SAMPLES") Then
                        myRotorPicture = Me.SamplesTab
                    Else
                        myRotorPicture = Me.ReagentsTab
                    End If
                    UpdateRotorArea(pRotorContenByPosRow, myNodeTag.ElementIcon, myRotorPicture)

                    'Validate if the Node corresponds to a Diluted Patient Sample
                    'If (IsDilution(CType(myNode.Tag, WSRequiredElementsTO).ElementID)) Then
                    'If (IsDilution(myNodeTag.ElementID)) Then 'RH 11/02/2010
                    '    'Validate if the Node has children
                    '    If (myNode.Nodes.Count > 0) Then
                    '        'AG 03/02/2010 - Change the ForeColor of the child Node too
                    '        'If (pPositionedStatus)  Then 'AG 12/04/2011
                    '        If (pPositionedStatus) AndAlso myNodeTag.ElementStatus = "POS" Then
                    '            myNode.Nodes(0).ForeColor = COLOR_POS
                    '        Else
                    '            myNode.Nodes(0).ForeColor = COLOR_NOPOS
                    '        End If
                    '        'AG 03/02/2010 END

                    '        'Set the Dilution Icon in the corresponding Position at the Rotor Area because it is a Manual Dilution
                    '        UpdateRotorArea(pRotorContenByPosRow, CType(myNode.Nodes(0).Tag, WSRequiredElementsTO).ElementIcon, SamplesTab.Controls)
                    '        'UpdateRotorArea(RotorContenByPosRow, CType(myNode.Nodes(0).Tag, WSRequiredElementsTO).ElementIcon, SplitContainer1.Controls)
                    '    Else
                    '        'Set the proper Icon in the correspondent Position in the Rotor Area
                    '        'UpdateRotorArea(pRotorContenByPosRow, CType(myNode.Tag, WSRequiredElementsTO).ElementIcon, SampleRotorPic.Controls)
                    '        'RH 11/02/2010
                    '        UpdateRotorArea(pRotorContenByPosRow, myNodeTag.ElementIcon, SamplesTab.Controls)

                    '        'UpdateRotorArea(RotorContenByPosRow, CType(myNode.Tag, WSRequiredElementsTO).ElementIcon, SplitContainer1.Controls)
                    '    End If
                    'Else

                    '    'RH 11/02/2011
                    '    'Remove unneeded New and CType
                    '    Dim myRotorPicture As Object
                    '    If (pRotorType = "SAMPLES") Then
                    '        myRotorPicture = Me.SamplesTab
                    '    ElseIf (pRotorType = "REAGENTS") Then
                    '        myRotorPicture = Me.ReagentsTab
                    '    End If

                    '    'RH 11/02/2011
                    '    UpdateRotorArea(pRotorContenByPosRow, myNodeTag.ElementIcon, myRotorPicture.Controls)
                    'End If
                    'Exit For
                Else
                    'Validate if it's a Node with Sub Nodes
                    If (myNode.Nodes.Count > 0) Then
                        'Search in the sub Nodes calling this same function (recursivity)
                        result = ChangeTreeRotorElementsStatus(myNode.Nodes, pPositionedStatus, pRotorContenByPosRow, pRotorType)
                    End If
                End If
            Next

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.StackTrace + " - " + ex.HResult.ToString + "))", Me.Name & ".ChangeTreeRotorElementsStatus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ChangeTreeRotorElementsStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

        Return result
    End Function

    '''' <summary>
    '''' Validate if the informed Required Element corresponds to a Patient Sample with manual Dilution
    '''' </summary>
    '''' <param name="pElementID">Identifier of a Required Work Session Element</param>
    '''' <returns>True if the Element corresponds to a Patient Sample with manual Dilution; otherwise it returns False</returns>
    '''' <remarks>
    '''' Created by:  TR 10/12/2009
    '''' Modified by: SA 21/12/2009 - Calls to MessageBox.Show were replaced by calls to the generic function 
    ''''                              ShowMessage  
    '''' </remarks>
    'Private Function IsDilution(ByVal pElementID As Integer) As Boolean
    '    Dim result As Boolean = False
    '    Try
    '        'Dim returnData As New GlobalDataTO
    '        Dim returnData As GlobalDataTO
    '        Dim wsElements As New WSRequiredElementsDelegate

    '        returnData = wsElements.GetRequiredElementData(Nothing, pElementID)
    '        If (Not returnData.HasError) Then
    '            'Dim myReqElementsDS As New WSRequiredElementsDS
    '            Dim myReqElementsDS As WSRequiredElementsDS
    '            myReqElementsDS = CType(returnData.SetDatos, WSRequiredElementsDS)

    '            If (myReqElementsDS.twksWSRequiredElements.Rows.Count > 0) Then
    '                If (Not myReqElementsDS.twksWSRequiredElements(0).IsPredilutionFactorNull) Then
    '                    result = True
    '                End If
    '            End If
    '        End If
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IsDilution", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".IsDilution", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
    '    End Try
    '    Return result
    'End Function




    ''' <summary>
    ''' Get the information that has to be shown in the Info Area section for the selected Rotor Position
    ''' </summary>
    ''' <param name="pAnalyzerID">Analyzer Identifier</param>
    ''' <param name="pRotorType">Rotor Type</param>
    ''' <param name="pRingNumber">Ring Number of the selected Rotor Position</param>
    ''' <param name="pCellNumber">Cell Number of the selected Rotor Position</param>
    ''' <remarks>
    ''' Created by:  BK 08/12/2009 - Tested: PENDING
    ''' Modified by: TR 15/12/2009 - Validate the fields and add the missing ones
    '''              SA 21/12/2009 - Some variable names changed to fulfill the defined standard coding rules
    '''                            - Calls to MessageBox.Show were replaced by calls to the generic function ShowMessage  
    '''                            - Add code to fill Info Area fields for Reagents and Additional Solutions
    '''              SA 23/12/2009 - Add code to deactivate buttons when a position without a Required WS Element is selected
    '''              AG 08/01/2010 - Modified (Tested OK)
    '''              AG 18/01/2010 - Load screen status control (isInfoAreaAllowed)
    '''              TR 20/01/2010 - Remove part of the code that duplicate the functionality of StatusInfoAreaControl function.
    '''              AG 28/01/2010 - show the tube contents description not the code
    '''              SA 26/10/2010 - Changed the way of getting the description of the Tube Content
    '''              TR 15/11/2013 - BT #1383 Complete the information required to get the Remaining test for reagents with status NOT INUSE.
    '''              JV 02/12/2013 - BT #1419 Don't show NumTestsRemaining when barcode is unknown.
    '''              TR 28/03/2014 - BT #1562 Show the expiration date on screen. get the value from the reagent barcode if exist.
    ''' </remarks>
    Private Sub ShowPositionInfoArea(ByVal pAnalyzerID As String, ByVal pRotorType As String, _
                                     ByVal pRingNumber As Integer, ByVal pCellNumber As Integer)
        Try
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
            If (currentStatus = "FREE" AndAlso barcodeStatus <> "ERROR") Then 'AG 05/10/2011 - Change condition
                CleanInfoArea(False, True) 'AG 03/10/2011 - Only in this case add optional parameter to TRUE for not disable barcode controls

                If (pRotorType = "SAMPLES") Then
                    bsSampleCellTextBox.Text = pCellNumber
                    bsSampleCellTextBox.Refresh()
                    bsSampleRingNumTextBox.Text = pRingNumber
                    bsSampleRingNumTextBox.Refresh()

                    'TODO: when Status=IDLE maybe more fields can be informed?
                ElseIf (pRotorType = "REAGENTS") Then
                    bsReagentsCellTextBox.Text = pCellNumber
                    bsReagentsCellTextBox.Refresh()
                    bsReagentsRingNumTextBox.Text = pRingNumber
                    bsReagentsRingNumTextBox.Refresh()

                    'TODO: when Status=IDLE maybe more fields can be informed?
                End If
            Else
                SetInfoAreaEnabledStatus(myRotorTypeForm, True)

                'Create a DataSet to fill the known Rotor Position information and create a Row for it
                Dim currentCellDS As New WSRotorContentByPositionDS
                Dim currentCellDSRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow

                'Fill the Row and insert it in the DataSet
                currentCellDSRow = currentCellDS.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow()
                currentCellDSRow.AnalyzerID = pAnalyzerID
                currentCellDSRow.RotorType = pRotorType
                currentCellDSRow.RingNumber = pRingNumber
                currentCellDSRow.CellNumber = pCellNumber
                currentCellDSRow.WorkSessionID = WorkSessionIDAttribute

                'TR 15/11/2013 BT #1383
                If (mySelectedCell.Count > 0 AndAlso Not mySelectedCell.First().IsReagentIDNull) Then
                    currentCellDSRow.ReagentID = mySelectedCell.First().ReagentID
                End If
                If (mySelectedCell.Count > 0 AndAlso Not mySelectedCell.First().IsRealVolumeNull) Then
                    currentCellDSRow.RealVolume = mySelectedCell.First().RealVolume
                End If
                'TR 15/11/2013 BT #1383

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

                            bsSampleRingNumTextBox.Clear()
                            If (Not myCellPosInfoDS.PositionInformation(0).IsRingNumberNull) Then
                                bsSampleRingNumTextBox.Text = myCellPosInfoDS.PositionInformation(0).RingNumber.ToString()
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
                                    'TR 13/01/2010 - Set value of Predilution Factor
                                    bsDiluteStatusTextBox.Text = "1/" & myCellPosInfoDS.Samples(0).PredilutionFactor
                                End If
                            End If

                            bsTubeSizeComboBox.SelectedIndex = -1
                            If (Not myCellPosInfoDS.Samples(0).IsTubeTypeNull) Then
                                'TR 13/01/2010 - Filter the Tube Sizes by the selected RingNumber
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
                            bsSamplesRefillPosButton.Enabled = False
                            'bsSamplesCheckVolumePosButton.Enabled = False

                            If (barcodeStatus = "ERROR") Then
                                bsSamplesDeletePosButton.Enabled = True
                            Else
                                bsSamplesDeletePosButton.Enabled = False
                            End If
                        End If

                        'TR 02/05/2011 - Get the status value in the current language
                        SamplesStatusTextBox.Text = GetStatusDescOnCurrentLanguage(currentStatus, barcodeStatus)

                        'Refresh controls in Info Area for Samples
                        'bsSampleCellTextBox.Refresh()
                        'bsSampleDiskNameTextBox.Refresh()
                        'bsSampleContentTextBox.Refresh()
                        'bsSampleNumberTextBox.Refresh()
                        'bsSampleIDTextBox.Refresh()
                        'bsSampleTypeTextBox.Refresh()
                        'bsDiluteStatusTextBox.Refresh()
                        'bsSamplesBarcodeTextBox.Refresh()
                        'bsTubeSizeComboBox.Refresh()

                    ElseIf (pRotorType = "REAGENTS") Then
                        'Get information from PositionInformation table in the returned DataSet and 
                        'fill Info Area fields for Reagents and Additional Solutions
                        If (myCellPosInfoDS.PositionInformation.Rows.Count > 0) Then
                            'Fill controls in Info Area Section for positions in Samples Rotor
                            bsReagentsCellTextBox.Clear()
                            If (Not myCellPosInfoDS.PositionInformation(0).IsCellNumberNull) Then
                                bsReagentsCellTextBox.Text = myCellPosInfoDS.PositionInformation(0).CellNumber.ToString()
                            End If

                            bsReagentsRingNumTextBox.Clear()
                            If (Not myCellPosInfoDS.PositionInformation(0).IsRingNumberNull) Then
                                bsReagentsRingNumTextBox.Text = myCellPosInfoDS.PositionInformation(0).RingNumber.ToString()
                            End If

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
                                If myCellPosInfoDS.Reagents.Count > 0 AndAlso Not myCellPosInfoDS.PositionInformation(0).BarcodeInfo = "" Then
                                    Dim myExpirationDate As Date = GetReagentExpDateFromBarCode(myCellPosInfoDS.PositionInformation(0).BarcodeInfo)
                                    'Validate if date is diferent than the minimun date value
                                    If myExpirationDate > Date.MinValue Then
                                        myCellPosInfoDS.Reagents(0).ExpirationDate = myExpirationDate
                                    End If
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
                            If (Not myCellPosInfoDS.Reagents(0).IsExpirationDateNull) Then
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
                                bsCurrentVolTextBox.Text = myCellPosInfoDS.Reagents(0).RealVolume.ToStringWithDecimals(2, True) 'AG 02/01/2011 - show real volume with 2 decimals
                            End If

                            bsTeststLeftTextBox.Clear()
                            If (Not myCellPosInfoDS.Reagents(0).IsRemainingTestsNull) Then
                                'JV 02/12/13: #1419
                                'bsTeststLeftTextBox.Text = myCellPosInfoDS.Reagents(0).RemainingTests.ToString()
                                If (barcodeStatus = "UNKNOWN") Then
                                    bsTeststLeftTextBox.Text = String.Empty
                                Else
                                    bsTeststLeftTextBox.Text = myCellPosInfoDS.Reagents(0).RemainingTests.ToString()
                                End If
                                'JV 02/12/13: #1419
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
                            bsReagentsRefillPosButton.Enabled = False
                            bsReagentsCheckVolumePosButton.Enabled = False

                            If (barcodeStatus = "ERROR") Then
                                bsReagentsDeletePosButton.Enabled = True
                            Else
                                bsReagentsDeletePosButton.Enabled = False
                            End If
                        End If

                        'TR 02/05/2011 - Get the status value in the current language
                        ReagStatusTextBox.Text = GetStatusDescOnCurrentLanguage(currentStatus, barcodeStatus)
                    End If
                Else
                    ShowMessage(Me.Name & ".ShowPositionInfoArea", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                End If
            End If

            'RH 13/09/2011 Forces to refresh the Info Area controls
            Application.DoEvents()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ShowPositionInfoArea", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ShowPositionInfoArea", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get the information that has to be shown in the Report related to the Rotor and the Info Area section
    ''' </summary>
    ''' <param name="pAnalyzerID">Analyzer identifier</param>
    ''' <param name="pRotorType">Rotor type</param>
    ''' <remarks>
    '''               Created by:  JV - 14/11/2013 - #1382: Get the info for the report using the 'ShowPositionInfoArea' method way to obtain the data.
    '''               Modified by: JV - 19/11/2013 - #1382: The status description in the report must be the same as the screen legend. I can not use the 'GetStatusDescOnCurrentLanguage' function used in ShowPositionInfoArea
    '''                            JV - 28/11/2013 - #1412: Show the used and no-used wells in the report. Unify all the status treated and their info.
    '''                                                     Include the column 'CellNumber' in the Reagents table (CellPositionInformationDS), due to it was impossible to associate rows with 'ElementID=Null' between tables in dataset
    '''                            JV - 29/11/2013 - #1419: Order correctly by well and don't show values "Type, Volume and/or Tests" depending the barcode status: error or unknown.
    '''                            JV - 03/12/2013 - #1384: Changes in ElementId when reset the session and mantain status
    '''                            TR - 28/03/2014 - #1562: Show the Expiration Date on te report.
    '''                            XB - 03/06/2014 - #1648: Change the sort of this report alphabetically by the name of the reagent
    ''' </remarks>
    Private Sub GetInfoAreaForReport(ByVal pAnalyzerID As String, ByVal pRotorType As String)
        Try
            'Select the rotor type cells and the positions with bottle
            Dim myCellsDS As WSRotorContentByPositionDS
            Dim myCellsTable As New WSRotorContentByPositionDS.twksWSRotorContentByPositionDataTable
            Dim lStatus As New List(Of String) From {"FREE"}

            Dim query As IEnumerable(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow) = _
                            (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition.AsEnumerable _
                                Where a.AnalyzerID = AnalyzerIDAttribute _
                                And a.WorkSessionID = WorkSessionIDAttribute _
                                And a.RotorType = pRotorType _
                                And Not lStatus.Contains(a.Status) _
                                OrElse (a.Status = "FREE" And a.BarcodeStatus = "ERROR") _
                                Order By a.CellNumber _
                                Select a)

            If Not (query.Count > 0) Then
                Return
            End If

            query.CopyToDataTable(myCellsTable, LoadOption.OverwriteChanges)

            'Get all the information of the Rotor Positions from the Database
            Dim myGlobalDataTO As GlobalDataTO
            Dim positionInformation As New WSRotorContentByPositionDelegate
            myCellsDS = New WSRotorContentByPositionDS()
            myCellsDS.Tables.Clear()
            myCellsDS.Tables.Add(myCellsTable)
            'Use this method similar to 'GetPositionInfo', just modified for the report usage (treat more than one row of the WSRotorContentByPositionDS)
            myGlobalDataTO = positionInformation.GetPositionInfoForReport(Nothing, myCellsDS)
            If (Not myGlobalDataTO.HasError) Then
                'Get the returned information
                Dim myCellPosInfoDS As CellPositionInformationDS
                myCellPosInfoDS = CType(myGlobalDataTO.SetDatos, CellPositionInformationDS)

                Dim myReport As New DataTable(myCellPosInfoDS.ReportTable.TableName)
                myReport = myCellPosInfoDS.ReportTable.Clone()
                myCellPosInfoDS.Tables.Remove(myCellPosInfoDS.ReportTable.TableName)
                myCellPosInfoDS.Tables.Add(myReport)
                Select Case myRotorTypeForm
                    Case "REAGENTS"
                        Dim row As DataRow
                        Dim myReagent As IEnumerable(Of CellPositionInformationDS.ReagentsRow)
                        Dim myCellsQuery As IEnumerable(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow) = _
                            From x In myCellsTable Select x 'Where Not x.IsElementIDNull Select x

                        Dim myExpirationDate As Date 'TR 10/04/2014 BT#1583

                        For Each dr As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myCellsQuery

                            row = myReport.NewRow()
                            row(0) = dr.CellNumber
                            myReagent = From x In myCellPosInfoDS.Reagents Where x.CellNumber = dr.CellNumber Select x

                            If myReagent.Count > 0 AndAlso Not myReagent.First.IsReagentNameNull Then row(1) = myReagent.First.ReagentName
                            If Not dr.IsBarCodeInfoNull Then row(2) = dr.BarCodeInfo

                            'TR 28/03/2014 #1562
                            If dr.BarcodeStatus = "OK" AndAlso dr.BarCodeInfo <> "" Then
                                If myReagent.Count > 0 Then
                                    myReagent.First().BeginEdit()

                                    'TR 10/04/2014 BT#1583
                                    myExpirationDate = Date.MinValue 'initialize variable
                                    'Get the expiration date
                                    myExpirationDate = GetReagentExpDateFromBarCode(dr.BarCodeInfo)
                                    'Validate if date is diferent than the minimun date value
                                    If myExpirationDate > Date.MinValue Then
                                        myReagent.First().ExpirationDate = myExpirationDate
                                        myReagent.First().ExpDateFormated = myReagent.First().ExpirationDate.ToString("MM/yyyy")
                                    End If
                                    'TR 10/04/2014 BT#1583
                                    'myReagent.First().ExpirationDate = GetReagentExpDateFromBarCode(dr.BarCodeInfo)
                                    'myReagent.First().ExpDateFormated = myReagent.First().ExpirationDate.ToString("MM/yyyy")
                                    myReagent.First().LotNumber = GetReagentLotNumberFromBarCode(dr.BarCodeInfo)
                                    myReagent.First().EndEdit()

                                    'TR 10/04/2014 BT#1583 Validate the date to be shown on report.
                                    If myExpirationDate > Date.MinValue Then
                                        'Set the Formated Date to be shown on Report
                                        row("ExpDateFormated") = myReagent.First().ExpDateFormated
                                    Else
                                        row("ExpDateFormated") = String.Empty
                                    End If
                                    'TR 10/04/2014 BT#1583

                                    'TR 06/05/2014 -Validate if LotNumber is a valid number to show on report
                                    If IsNumeric(myReagent.First().LotNumber) AndAlso CInt(myReagent.First().LotNumber) > 0 Then
                                        'Set the lot number to shown on report
                                        row("LotNumber") = myReagent.First().LotNumber
                                    Else
                                        row("LotNumber") = ""
                                    End If
                                    'TR 06/05/2014 -END
                                End If
                            End If
                            'TR 28/03/2014 #1562 -END

                            If myReagent.Count > 0 AndAlso Not myReagent.First.IsExpirationDateNull Then
                                row(3) = myReagent.First.ExpirationDate.ToString(SystemInfoManager.OSDateFormat)
                            End If

                            If Not dr.IsTubeTypeNull Then

                                Dim result As List(Of TubeSizeTO) = (From a In AllTubeSizeList _
                                                                     Where a.RingNumber = dr.RingNumber _
                                                                     AndAlso String.Compare(a.RotorType, myRotorTypeForm, False) = 0 _
                                                                     AndAlso a.TubeCode = dr.TubeType _
                                                                     Select a).ToList()
                                row(4) = IIf(result.Count > 0, result.First.FixedTubeName, String.Empty)
                            End If
                            If Not dr.IsRealVolumeNull Then row(5) = dr.RealVolume.ToStringWithDecimals(2, True)
                            If myReagent.Count > 0 AndAlso Not myReagent.First.IsRemainingTestsNull Then row(6) = myReagent.First.RemainingTests.ToString
                            If dr.Status = "FREE" AndAlso dr.BarcodeStatus = "ERROR" Then
                                row(4) = String.Empty
                                row(5) = String.Empty
                                row(6) = String.Empty
                                row(7) = bsBarcodeErrorRGLabel.Text
                                row(8) = dr.BarcodeStatus
                            ElseIf dr.BarcodeStatus = "UNKNOWN" Then
                                row(6) = String.Empty
                                row(7) = bsUnknownLabel.Text
                                row(8) = dr.Status
                            Else
                                Select Case dr.Status
                                    Case "FEW"
                                        row(7) = bsLegReagLowVolLabel.Text
                                    Case "DEPLETED"
                                        row(7) = bsLegReagDepleteLabel.Text
                                    Case Else
                                        row(7) = String.Empty
                                End Select
                                row(8) = dr.Status
                            End If
                            myReport.Rows.Add(row)
                        Next

                        ' XB 03/06/2014 - BT #1648
                        'myReport.DefaultView.Sort = myReport.Columns(0).ColumnName & " " & "ASC"
                        myReport.DefaultView.Sort = myReport.Columns(1).ColumnName & " " & "ASC"

                        myReport = myReport.DefaultView.ToTable()
                    Case Else
                        'Nothing to do
                        Return
                End Select
                myCellPosInfoDS.AcceptChanges()
                'Finally we launch the report
                XRManager.ShowRotorContentByPositionReport(myCellPosInfoDS)
            Else
                ShowMessage(Me.Name & ".GetInfoAreaForReport", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetInfoAreaForReport", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetInfoAreaForReport", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get the Status translation depending on the selected language.
    ''' </summary>
    ''' <param name="pItemID">Item Id</param>
    ''' <param name="pBarcodeStatus" ></param>
    ''' <returns>The status name on the application language.</returns>
    ''' <remarks>
    ''' Created by:  TR 29/04/2011
    ''' Modified by: AG 06/10/2011 - Add pBarCodeStatus parameter for show new status due Barcode Error</remarks>
    Private Function GetStatusDescOnCurrentLanguage(ByVal pItemID As String, ByVal pBarcodeStatus As String) As String
        Dim myResult As String = ""
        Try
            Dim mySubTableID As GlobalEnumerates.PreloadedMasterDataEnum
            Dim myItemID As String = pItemID

            'Get the value for the subtable id 
            Select Case myRotorTypeForm
                Case "SAMPLES"
                    mySubTableID = GlobalEnumerates.PreloadedMasterDataEnum.SAMPLE_POS_STATUS
                    Exit Select
                Case "REAGENTS"
                    mySubTableID = GlobalEnumerates.PreloadedMasterDataEnum.REAGENT_POS_STATUS
                    Exit Select
            End Select

            'AG 06/10/2011 - Add Barcode Status information (by now, if Barcode Status is ERROR, status to shown will be NOT IN USE)
            If (pBarcodeStatus = "ERROR") Then myItemID = "NO_INUSE"

            Dim myGlobalDataTO As New GlobalDataTO
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

            'Get the multilanguage description of the informed Item
            myGlobalDataTO = myPreloadedMasterDataDelegate.GetSubTableItem(Nothing, mySubTableID, myItemID)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreloMasterDS As PreloadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                If (myPreloMasterDS.tfmwPreloadedMasterData.Count > 0) Then myResult = myPreloMasterDS.tfmwPreloadedMasterData(0).FixedItemDesc
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetStatusLanguage ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetStatusLanguage ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Method that calls the function that execute the automatic positioning of Samples or Reagents and Additional Solutions
    ''' that needs to be positioned for the Work Session. After positioning, the Rotor Area and the TreeView are updated.
    ''' </summary>
    ''' <param name="SamplesPositioning">When True indicates the function was called to automatic positioning 
    '''                                  of Samples; when False, indicates it was called to automatic positioning
    '''                                  of Reagents and Additional Solutions</param>
    ''' <remarks>
    ''' Created by:  TR 15/12/2009 - Tested: OK
    ''' Modified by: SA 21/12/2009 - Calls to MessageBox.Show were replaced by calls to the generic function 
    '''                              ShowMessage 
    '''              TR 19/01/2010 - Validate the selected rotor before autopositioning
    '''              SA 26/07/2010 - Call new function PendingPositionedElements to enable/disable the Warnings Button
    ''' </remarks>
    Private Sub ReagentsSamplesAutoPositioning(ByVal SamplesPositioning As Boolean)
        Try
            Dim positioned As Boolean = False

            'Dim myGlobalTO As New GlobalDataTO
            Dim myGlobalTO As GlobalDataTO
            Dim myRotorContentByPositionDelegate As New WSRotorContentByPositionDelegate

            Me.Cursor = Cursors.WaitCursor

            'RH 01/03/2011
            If SamplesPositioning Then
                'Execute automatic positioning of Samples
                myGlobalTO = myRotorContentByPositionDelegate.SamplesAutoPositioning(Nothing, WorkSessionIDAttribute, AnalyzerIDAttribute, "SAMPLES")
                positioned = True
                myRotorTypeForm = "SAMPLES"
            Else
                'Execute automatic positioning of Reagents and Additional Solutions
                myGlobalTO = myRotorContentByPositionDelegate.ReagentsAutoPositioning(Nothing, WorkSessionIDAttribute, AnalyzerIDAttribute, "REAGENTS")
                positioned = True
                myRotorTypeForm = "REAGENTS"
            End If

            'TR 29/01/2010 Prepared for warning about rotor full or related.
            If (positioned) Then
                If (Not myGlobalTO.HasError) Then
                    If (Not myGlobalTO.SetDatos Is Nothing) Then
                        UpdateRotorTreeViewArea(CType(myGlobalTO.SetDatos, WSRotorContentByPositionDS))
                    End If

                    If (myGlobalTO.ErrorCode <> "") Then
                        'The Rotor is full or partially full....
                        ShowMessage(Me.Name & ".ReagentsSamplesAutoPositioning", myGlobalTO.ErrorCode, , Me)
                    End If
                Else
                    ShowMessage(Me.Name & ".ReagentsSamplesAutoPositioning", myGlobalTO.ErrorCode, myGlobalTO.ErrorMessage, Me)
                End If

                ''Verify if Warnings Button has to be enabled or Disable, depending on if there are not positioned Elements 
                'PendingPositioningElements(True)
            End If

            'TR 20/01/2010 - Load the control status after Autopositioning.
            SetInfoAreaEnabledStatus(myRotorTypeForm, False)

            'RH 01/03/2011
            If SamplesPositioning Then
                RotorsTabs.SelectedTabPage = SamplesTab
            Else
                RotorsTabs.SelectedTabPage = ReagentsTab
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ReagentsSamplesAutoPositioning", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReagentsSamplesAutoPositioning", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        Finally
            Me.Cursor = Cursors.Default

        End Try

    End Sub

    ''' <summary>
    ''' Returns the number of positioned elements in the especified Rotor Type
    ''' </summary>
    ''' <param name="pRotorType">Rotor Type</param>
    ''' <remarks>
    ''' Created by: RH 18/02/2011
    ''' Modified by: DL 05/10/2011 Included when status = FREE and barcodestatus = ERROR
    ''' </remarks>
    Private Function RotorPositionedElements(ByVal pRotorType As String) As Integer
        Try
            If isClosingFlag Then Exit Function ' XB 13/03/2014 - #1496 No refresh if screen is closing
            If myRotorContentByPositionDSForm Is Nothing Then Exit Function ' XB 13/03/2014 - #1523 No refresh if screen is closing

            Dim NotFreePos As EnumerableRowCollection(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow) = _
                    (From p In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                     Where p.RotorType = pRotorType _
                     AndAlso (p.Status <> "FREE" Or (p.Status = "FREE" AndAlso (p.BarcodeStatus = "ERROR" Or p.BarcodeStatus = "UNKNOWN"))) _
                     Select p)

            Return NotFreePos.Count

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".RotorPositioned", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RotorPositioned", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Function

    ''' <summary>
    ''' Set the Status of all positions of the specified Rotor to FREE in the Rotor Area, and the Status 
    ''' of all the correspondent Required Elements to No Positioned in the TreeView Area 
    ''' </summary>
    ''' <param name="pRotorType">Rotor Type</param>
    ''' <param name="pAnalizerID">Analyzer Identifier</param>
    ''' <remarks>
    ''' Created by:  TR 15/12/2009
    ''' Modified by: SA 21/12/2009 - Current code when no error was replaced for a call to new method InitializeScren 
    '''                            - Calls to MessageBox.Show were replaced by calls to the generic function 
    '''                              ShowMessage 
    '''              TR 12/01/2010 - Show the question message before rotor is reset
    '''              DL 05/10/2011 - Included when status = FREE and barcodestatus = ERROR
    ''' </remarks>
    Private Sub ResetRotor(ByVal pRotorType As String, ByVal pAnalizerID As String)
        Try
            'Do business only if the RotorType is not empty
            Dim rcpList As New List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)

            rcpList = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                       Where a.RotorType = pRotorType _
                       AndAlso (a.Status <> "FREE" Or (a.Status = "FREE" AndAlso (a.BarcodeStatus = "ERROR" Or a.BarcodeStatus = "UNKNOWN"))) _
                       Select a).ToList

            If rcpList.Count > 0 Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.ROTOR_RESET.ToString, , Me) = DialogResult.Yes) Then
                    Me.Cursor = Cursors.WaitCursor

                    Dim myGlobalTo As GlobalDataTO
                    Dim myRotorContentByPositionDelegate As New WSRotorContentByPositionDelegate

                    myGlobalTo = myRotorContentByPositionDelegate.ResetRotor(Nothing, pAnalizerID, pRotorType, WorkSessionIDAttribute)

                    'RH 15/09/2011
                    If (Not myGlobalTo.HasError) Then
                        Dim BarcodePositionsWithNoRequests As New BarcodePositionsWithNoRequestsDelegate
                        myGlobalTo = BarcodePositionsWithNoRequests.ResetRotor(Nothing, pAnalizerID, WorkSessionIDAttribute, pRotorType)
                    End If

                    If (Not myGlobalTo.HasError) Then
                        'Prepare all the position controls
                        PreparePositionsControls(pRotorType) 'RH 15/02/2010

                        InitializeScreen(True, pRotorType) 'AG 11/11/2010 - add 2on parameter
                    Else
                        'Error reseting the Rotor
                        ShowMessage(Me.Name & ".ResetRotor", myGlobalTo.ErrorCode, myGlobalTo.ErrorMessage, Me)
                    End If

                End If

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ResetRotor", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ResetRotor", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        Finally
            Me.Cursor = Cursors.Default

        End Try
    End Sub

    ''' <summary>
    ''' To Save the Virtual Rotor Position Details
    ''' </summary>
    ''' <remarks>
    ''' Created by:  BK 01/12/2009
    ''' Modified by: BK 22/12/2009
    '''              VR - Fix some errors
    '''              VR 23/12/2009 - New Rotor Name flow is Pending 
    '''              VR 05/01/2010 - Testing: OK
    '''              AG 08/01/2010 - Changes and testing (Testing: OK)
    '''              DL 08/01/2010 - Changes and testing (Testing: OK)
    '''              AG 26/01/2010 - Changes in virtual rotor selection auxiliar screen
    '''              AG 14/03/2011 - define private
    ''' </remarks>
    Private Sub SaveVirtualRotor(ByVal pRotorType As String)
        Try

            Dim myElemFree As Integer = 0
            myElemFree = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                          Where a.RotorType = pRotorType AndAlso a.Status <> "FREE" _
                          Select a.Status).Count

            If myElemFree > 0 Then

                'RH 19/10/2010 Introduce the Using statement
                Using myLoadSaveAuxScreen As New IWSLoadSaveAuxScreen()
                    myLoadSaveAuxScreen.ScreenUse = "VROTORS"
                    myLoadSaveAuxScreen.SourceButton = "SAVE"
                    myLoadSaveAuxScreen.RotorType = pRotorType

                    'If (pRotorType = "SAMPLES") Then
                    '    myLoadSaveAuxScreen.NameProperty = VirtualSampleRotorName
                    'ElseIf (pRotorType = "REAGENTS") Then
                    '    myLoadSaveAuxScreen.NameProperty = VirtualReagentRotorName
                    'End If

                    'RH 15/02/2011 Simplified code
                    If (pRotorType = "SAMPLES") Then
                        myLoadSaveAuxScreen.NameProperty = VirtualSampleRotorName
                    Else
                        myLoadSaveAuxScreen.NameProperty = VirtualReagentRotorName
                    End If

                    'Show Auxilliary Screen with all Virtual Rotors for rotor type in parameter
                    If (myLoadSaveAuxScreen.ShowDialog() = DialogResult.OK) Then
                        Me.Cursor = Cursors.WaitCursor

                        'RH 15/02/2011 Simplified code
                        If (pRotorType = "SAMPLES") Then
                            VirtualSampleRotorID = myLoadSaveAuxScreen.IDProperty
                            VirtualSampleRotorName = myLoadSaveAuxScreen.NameProperty

                        Else
                            VirtualReagentRotorID = myLoadSaveAuxScreen.IDProperty
                            VirtualReagentRotorName = myLoadSaveAuxScreen.NameProperty
                        End If

                        GenerateAndSaveVRotorPositionDS(myRotorContentByPositionDSForm, pRotorType, myLoadSaveAuxScreen.IDProperty, myLoadSaveAuxScreen.NameProperty)
                        'RH 15/02/2011 END
                    End If
                End Using
            Else
                'ShowMessage(Me.Name & ".SaveVirtualRotor", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, , Me)
                ShowMessage("AX00", GlobalEnumerates.Messages.EMPTYROTOR.ToString)
            End If


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveVirtualRotor ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveVirtualRotor", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Open the auxiliary screen that allow select a Virtual Rotor to load. Once the Virtual Rotor to load is selected,
    ''' execute the actions needed to show its content in the Rotor
    ''' </summary>
    ''' <param name="pRotorType">Type of Virtual Rotor to load</param>
    ''' <remarks> 
    ''' Created by:  VR
    ''' Modified by: VR 16/12/2009 
    '''              AG 08/01/2010 - Modify errors, delete fixed testing code and not necessary code and finally testing
    '''              DL 25/01/2010 - Modify errors, delete fixed testing code and not necessary code and finally testing 
    '''              AG 26/01/2010 - Changes in virtual rotor selection auxiliar screen
    '''              TR 05/01/2010 - Reset the rotor an initialize the screen before loading the new rotor
    '''              SA 07/06/2010 - Changed the way of opening the auxiliary screen for Virtual Rotor Selection (the ComboBox is loaded
    '''                              in the Auxiliary Screen, is not needed load it from outside).  Deleted parameter pSourceButton; it is
    '''                              not needed due to this method is called only for LOAD; replaced use of screen Properties for use of
    '''                              the correspondent screen Attributes
    '''              RH 19/10/2010 - Introduce the Using statement
    '''              SA 09/02/2012 - If there is an error while reset the Rotor, do not try to load the Virtual Rotor
    ''' </remarks>
    Private Sub LoadVirtualRotor(ByVal pRotorType As String)
        Try
            Using myLoadSaveAuxScreen As New IWSLoadSaveAuxScreen()
                'Assign the required properties of the auxiliary screen and open it as a DialogForm
                myLoadSaveAuxScreen.ScreenUse = "VROTORS"
                myLoadSaveAuxScreen.SourceButton = "LOAD"
                myLoadSaveAuxScreen.RotorType = pRotorType

                If (myLoadSaveAuxScreen.ShowDialog() = DialogResult.OK) Then
                    Me.Cursor = Cursors.WaitCursor

                    'RH 15/02/2011 Simplified code
                    If (pRotorType = "SAMPLES") Then
                        VirtualSampleRotorID = myLoadSaveAuxScreen.IDProperty
                        VirtualSampleRotorName = myLoadSaveAuxScreen.NameProperty
                    Else
                        VirtualReagentRotorID = myLoadSaveAuxScreen.IDProperty
                        VirtualReagentRotorName = myLoadSaveAuxScreen.NameProperty
                    End If

                    Dim myGlobalObj As GlobalDataTO
                    Dim myRotorContentPos As New WSRotorContentByPositionDelegate

                    'Reset the Rotor an initialize the screen before loading the selected Virtual Rotor
                    myGlobalObj = myRotorContentPos.ResetRotor(Nothing, AnalyzerIDAttribute, pRotorType, WorkSessionIDAttribute)
                    If (Not myGlobalObj.HasError) Then
                        'Prepare all the position controls
                        PreparePositionsControls(pRotorType) 'RH 15/02/2011

                        'Initialize the Screen 
                        InitializeScreen(True, pRotorType) 'AG 11/11/2010 - Add 2on parameter

                        myGlobalObj = myRotorContentPos.LoadRotor(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, pRotorType, myLoadSaveAuxScreen.IDProperty)
                        If (Not myGlobalObj.HasError) Then
                            ClearSelection()
                            'SA + JV - 12/12/2013 - #1384
                            If (WorkSessionStatusAttribute = "EMPTY" OrElse WorkSessionStatusAttribute = "OPEN") Then
                                myGlobalObj = myRotorContentPos.GetRotorContentPositionsResetDone(Nothing, WorkSessionIDAttribute, AnalyzerIDAttribute)
                            End If
                            'FIN 12/12/2013

                            'TR 25/01/2010 - Change also the ElementStatus in the TreeView
                            ResetTreeViewPositionedElements(Me.bsElementsTreeView.Nodes)
                            Me.UpdateRotorTreeViewArea(CType(myGlobalObj.SetDatos, WSRotorContentByPositionDS)) '08/01/2010 AG
                        Else
                            'Error loading the selected Virtual Rotor, shown it
                            ShowMessage(Me.Name & ".LoadVirtualRotor", myGlobalObj.ErrorCode, myGlobalObj.ErrorMessage, Me)
                        End If
                    Else
                        'Error while reset the Rotor, shown it
                        ShowMessage(Me.Name & ".LoadVirtualRotor", myGlobalObj.ErrorCode, myGlobalObj.ErrorMessage, Me)
                    End If
                End If
            End Using
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "LoadVirtualRotor " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadVirtualRotor", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Initialize the Screen: Rotor Area, TreeView Area and Screen Structures
    ''' </summary>
    ''' <param name="pAfterResetRotor">When True indicates the method is called after Reset the current Rotor;
    '''                                when False, it indicates the method is called to initial Screen Load</param>
    ''' <param name="pResetRotorType"></param> "" no reset rotor, else indicates the reset rotor type
    ''' <remarks>
    ''' Created  By: SA 21/12/2009 - Created due the same code was used in Load Event and in ResetRotor function
    ''' Modified By: TR 05/01/2010 - Correct the funtion name previous InitializeScren change for InitializeScreen.
    ''' AG 11/11/2010 - add parameter pResetRotorType ("" if no reset rotor
    ''' </remarks>
    Private Sub InitializeScreen(ByVal pAfterResetRotor As Boolean, ByVal pResetRotorType As String)
        Try
            'SGM 30/04/2013
            bsElementsTreeView.ShowNodeToolTips = True

            'Load TreeView Elements
            FillTreeView(WorkSessionIDAttribute, pResetRotorType)

            'Load Rotor Area info
            LoadRotorAreaInfo(WorkSessionIDAttribute, AnalyzerIDAttribute, pAfterResetRotor, pResetRotorType)

            'Only in screen loading, get all the available Tubes and Bottles, fill the screen structure
            'AllTubeSizeList and fill the correspondent Commbo Boxes in both Info Area Sections
            If (Not pAfterResetRotor) Then
                GetAllTubeSizes()
                FilterAllTubesbyRingNumberRotorType(0) 'TR 13/01/2010 -initialize on rin 0 
            End If

            'Clean controls in Info Area Sections for Samples and Reagents
            CleanInfoArea(pAfterResetRotor)

            'TR 14/09/2011 -Validate if Barcode with no request to enable or disable.
            ValidateBCNoRequest()



        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeScren", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeScren", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Activate/deactivate controls in the specified Info Area Section
    ''' </summary>
    ''' <param name="pRotorType">Indicates the Info Area Section in which controls have to be activate/deactivate</param>
    ''' <param name="pEnabled">When True, controls have to be activated; otherwise, controls have to be deactivated</param>
    ''' <param name="pExcludeBarCode">When TRUE the barcode controls remains enabled for manual barcode entry</param>
    ''' <remarks>
    ''' Created  by : SA 23/12/2009 (Tested: PENDING)
    ''' Modified by : AG 08/01/2010 - When select a FREE CELL not disabled scroll buttons (Tested: OK)
    '''               AG 25/01/2010 - Use the screen access status value for the buttons refill, reset, check volume and 
    '''                               change tube for the singles positions
    '''               TR 02/05/2011 - Added enable/disable and change of BackColor for new Status controls for Samples and Reagents
    '''               AG 30/09/2011 - Field ReagentBarcode is always enabled for FREE positions
    '''               DL 04/10/2011 - Control availability of Delete Position button for Samples Rotor
    '''               DL 05/10/2011 - Control availability of Delete Position button for Reagents Rotor
    '''               RH 10/10/2011 - For Reagents: bottle can be changed only when there is not Barcode info for the selected position
    '''               SA 17/10/2011 - Enable/disable and change of BackColor for control ReagStatusTextBox was inside the code for RotorType=SAMPLES
    '''                               instead of inside the code for RotorType=REAGENTS
    '''               TR 07/11/2013 - BT#1358 For each selected element validate if allow action in pause mode. to enable or disable buttons (Actions).
    '''               TR 15/11/2013 - BT #1358(2) Implement functionality for Reactions rotor.
    ''' </remarks>
    Private Sub SetInfoAreaEnabledStatus(ByVal pRotorType As String, ByVal pEnabled As Boolean, Optional ByVal pExcludeBarCode As Boolean = False)
        Try
            If isClosingFlag Then Exit Sub ' XB 13/03/2014 - #1496 No refresh if screen is closing

            Dim backColor As Color = Color.Gainsboro
            If (Not pEnabled) Then backColor = SystemColors.MenuBar

            'TR 15/11/2013 - BT #1358(2) -Change declaration level to make it visible on reagents options.
            'Variable use to indicate id the action is allowed on pause mode.
            Dim AllowedActionInPause As Boolean = True

            If (pRotorType = "SAMPLES") Then
                bsSampleCellTextBox.Enabled = pEnabled
                bsSampleCellTextBox.BackColor = backColor
                bsSampleRingNumTextBox.Enabled = pEnabled
                bsSampleRingNumTextBox.BackColor = backColor
                bsSampleContentTextBox.Enabled = pEnabled
                bsSampleContentTextBox.BackColor = backColor
                bsSampleNumberTextBox.Enabled = pEnabled
                bsSampleNumberTextBox.BackColor = backColor
                bsSampleIDTextBox.Enabled = pEnabled
                bsSampleIDTextBox.BackColor = backColor
                bsSampleTypeTextBox.Enabled = pEnabled
                bsSampleTypeTextBox.BackColor = backColor
                bsDiluteStatusTextBox.Enabled = pEnabled
                bsDiluteStatusTextBox.BackColor = backColor
                SamplesStatusTextBox.Enabled = pEnabled
                SamplesStatusTextBox.BackColor = backColor

                'TR 07/11/2013 -BT#1358 Validate if analyzer is in Pause Mode.
                'TR 26/11/2013 -BT #1404 Validate the Analyzer status Running to apply rule.
                '#REFACTORING
                If AnalyzerController.Instance.Analyzer.AllowScanInRunning OrElse _
                    AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                    AllowedActionInPause = VerifyActionsAllowedInSampleRotor(mySelectedElementInfo, False)
                End If

                If (pEnabled) Then
                    'TR 07/11/2013 BT#1358 to enable validate the AllowedActionInPause and isChangeTubeSizeAllowed
                    bsTubeSizeComboBox.Enabled = (isChangeTubeSizeAllowed AndAlso AllowedActionInPause)
                    bsSamplesDeletePosButton.Enabled = (isSingleDeletePosAllowed AndAlso AllowedActionInPause)

                    'bsSamplesBarcodeTextBox.Enabled = (pEnabled And AllowedActionInPause) ' TR 08/11/2013 BT#1358 Barcode Textbox
                    'bsSamplesBarcodeTextBox.Enabled = (pEnabled And isManualBarcodeAllowed And AllowedActionInPause) ' TR 26/11/2013 BT#1404

                    bsSamplesRefillPosButton.Enabled = isSingleRefillPosAllowed
                Else
                    bsTubeSizeComboBox.Enabled = pEnabled
                    bsSamplesRefillPosButton.Enabled = pEnabled

                    If (BarCodeErrorSampleFree) Then
                        bsSamplesDeletePosButton.Enabled = True
                    Else
                        bsSamplesDeletePosButton.Enabled = pEnabled
                    End If
                    'bsSamplesCheckVolumePosButton.Enabled = False
                    'TR 28/3/2012 TODO: Functionality enable on version 5 meanwhele disable 
                    bsReagentsCheckVolumePosButton.Enabled = False
                    bsCheckRotorVolumeButton.Enabled = False
                    'TR 28/3/2012END
                End If

            ElseIf (pRotorType = "REAGENTS") Then
                bsReagentsCellTextBox.Enabled = pEnabled
                bsReagentsCellTextBox.BackColor = backColor
                bsReagentsRingNumTextBox.Enabled = pEnabled
                bsReagentsRingNumTextBox.BackColor = backColor
                bsReagentsContentTextBox.Enabled = pEnabled
                bsReagentsContentTextBox.BackColor = backColor
                bsReagentsNumberTextBox.Enabled = pEnabled
                bsReagentsNumberTextBox.BackColor = backColor
                bsReagentNameTextBox.Enabled = pEnabled
                bsReagentNameTextBox.BackColor = backColor
                bsTestNameTextBox.Enabled = pEnabled
                bsTestNameTextBox.BackColor = backColor
                bsExpirationDateTextBox.Enabled = pEnabled
                bsExpirationDateTextBox.BackColor = backColor
                bsCurrentVolTextBox.Enabled = pEnabled
                bsCurrentVolTextBox.BackColor = backColor
                bsTeststLeftTextBox.Enabled = pEnabled
                bsTeststLeftTextBox.BackColor = backColor
                ReagStatusTextBox.Enabled = pEnabled
                ReagStatusTextBox.BackColor = backColor

                'TR 15/11/2013 -BT #1358(2) Validate if analyzer is in Pause Mode.
                'TR 26/11/2013 -BT #1404 Validate the Analyzer status Running to apply rule.
                '#REFACTORING
                If AnalyzerController.Instance.Analyzer.AllowScanInRunning OrElse _
                    AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                    'Validate if action is allowed on pause mode.
                    AllowedActionInPause = VerifyActionsAllowedInReagentsRotor()
                End If
                'TR 15/11/2013 -BT #1358(2) -END.

                'Control the availability and BackColor of the ComboBox for Bottle Size
                'TR 15/11/2013 BT#1358(2) validate the allowedActionInPause Too.
                bsBottleSizeComboBox.Enabled = (pEnabled And isChangeTubeSizeAllowed And AllowedActionInPause And bsReagentsBarCodeTextBox.Text = String.Empty)

                If (bsBottleSizeComboBox.Enabled) Then
                    bsBottleSizeComboBox.BackColor = Color.White
                Else
                    bsBottleSizeComboBox.BackColor = SystemColors.MenuBar
                End If

                'Control availability of buttons in Info area
                If (pEnabled) Then
                    bsReagentsRefillPosButton.Enabled = (isSingleRefillPosAllowed)
                    'TR 15/11/2013 -BT #1358(2) validate the AllowedAction too.
                    bsReagentsDeletePosButton.Enabled = (isSingleDeletePosAllowed AndAlso AllowedActionInPause)
                    'bsReagentsCheckVolumePosButton.Enabled = isSingleCheckPosAllowed
                    'bsSamplesCheckVolumePosButton.Enabled = False   'TODO: confirm if it will be Check Volume for Samples; meanwhile this button is always disabled
                    bsCheckRotorVolumeButton.Enabled = False 'TODO: confirm if it will be Check Volume for Samples; meanwhile this button is always disabled
                Else
                    bsReagentsRefillPosButton.Enabled = pEnabled
                    bsReagentsCheckVolumePosButton.Enabled = pEnabled
                    'TR 28/3/2012 TODO: Functionality enable on version 5 meanwhele disable 
                    bsReagentsCheckVolumePosButton.Enabled = False
                    bsCheckRotorVolumeButton.Enabled = False
                    'TR 28/3/2012END
                    If (BarCodeErrorReagentFree) Then
                        bsReagentsDeletePosButton.Enabled = True
                    Else
                        bsReagentsDeletePosButton.Enabled = pEnabled
                    End If
                End If
            End If

            'Control the availability and BackColor of field for Barcode info
            If (Not pExcludeBarCode) Then
                'TR 27/11/2013 -BT# 1404 
                If (pRotorType = "SAMPLES") Then
                    'TR BT# 1404 -Set the same functionality as reagentbarcodetextbox.
                    bsSamplesBarcodeTextBox.Enabled = (pEnabled And isManualBarcodeAllowed And AllowedActionInPause)

                    If (bsSamplesBarcodeTextBox.Enabled) Then
                        bsSamplesBarcodeTextBox.BackColor = Color.White
                    Else
                        bsSamplesBarcodeTextBox.BackColor = backColor
                    End If
                ElseIf (pRotorType = "REAGENTS") Then
                    'TR 26/03/2012 -Set the enable value to allow edition. validate the isManualBarcodeAllowed instead the pEnable
                    bsReagentsBarCodeTextBox.Enabled = (pEnabled And isManualBarcodeAllowed And AllowedActionInPause)

                    If (bsReagentsBarCodeTextBox.Enabled) Then
                        bsReagentsBarCodeTextBox.BackColor = Color.White
                    Else
                        bsReagentsBarCodeTextBox.BackColor = backColor
                    End If
                End If
            Else
                If (pRotorType = "SAMPLES") Then
                    bsSamplesBarcodeTextBox.Enabled = (isManualBarcodeAllowed And AllowedActionInPause)
                    If (bsSamplesBarcodeTextBox.Enabled) Then
                        bsSamplesBarcodeTextBox.BackColor = Color.White
                    Else
                        bsSamplesBarcodeTextBox.BackColor = backColor
                    End If
                ElseIf (pRotorType = "REAGENTS") Then
                    'Field Barcode is always available for FREE positions
                    bsReagentsBarCodeTextBox.Enabled = (isManualBarcodeAllowed And AllowedActionInPause)
                    If (bsReagentsBarCodeTextBox.Enabled) Then
                        bsReagentsBarCodeTextBox.BackColor = Color.White
                    Else
                        bsReagentsBarCodeTextBox.BackColor = backColor
                    End If
                End If
                'bsReagentsBarCodeTextBox.BackColor = Color.White
                'bsSamplesBarcodeTextBox.BackColor = Color.White
            End If
            'TR 27/11/2013 -BT# 1404 END

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.StackTrace + " - " + ex.HResult.ToString + "))", Me.Name & ".StatusInfoAreaControls", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".StatusInfoAreaControls", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate if in the selecte element list are items that allow action on Pause mode.
    ''' for each element the AllowedActionInPause is updated. 
    ''' </summary>
    ''' <param name="pSelectedElementInfo"></param>
    ''' <returns>
    ''' Return True if there's at least one element that allow action on pause, Return false if no action allow.
    ''' </returns>
    ''' <remarks>
    ''' CREATED BY: TR 07/11/2013 - BT #1358
    '''             TR 26/11/2013 - BT #1404 Validate the Running status mode to set the allowed actions.
    ''' </remarks>
    Private Function VerifyActionsAllowedInSampleRotor(ByVal pSelectedElementInfo As WSRotorContentByPositionDS, ByVal pForMovePosition As Boolean) As Boolean
        Dim myResult As Boolean = True
        Try
            'Validate DS is not null to continue
            If Not pSelectedElementInfo Is Nothing Then

                If AnalyzerController.Instance.Analyzer.AllowScanInRunning Then '#REFACTORING
                    'pause mode to validate the allowed action on pause
                    'Go throught all element and set the Allowed action property value.
                    For Each mySelectedElementInfoRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In _
                                                                  pSelectedElementInfo.twksWSRotorContentByPosition.Rows
                        'Set value to True at the begining of validation.
                        mySelectedElementInfoRow.AllowedActionInPause = True

                        If mySelectedElementInfoRow.Status = "FEW" OrElse _
                           mySelectedElementInfoRow.Status = "DEPLETED" OrElse mySelectedElementInfoRow.Status = "INPROCESS" Then

                            mySelectedElementInfoRow.AllowedActionInPause = False

                        ElseIf Not pForMovePosition AndAlso mySelectedElementInfoRow.TubeContent = "CALIB" _
                                                              AndAlso Not mySelectedElementInfoRow.IsCalibratorIDNull Then

                            Dim myCalibPosToRemove As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                            'Validate if it's multipoint calib.
                            myCalibPosToRemove = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                                                  Where Not a.IsCalibratorIDNull _
                                                  AndAlso a.WorkSessionID = mySelectedElementInfoRow.WorkSessionID _
                                                  AndAlso a.RotorType = mySelectedElementInfoRow.RotorType _
                                                  AndAlso a.CalibratorID = mySelectedElementInfoRow.CalibratorID _
                                                  AndAlso a.CellNumber <> mySelectedElementInfoRow.CellNumber Select a).ToList()

                            If myCalibPosToRemove.Count > 0 Then
                                'Validate the position status of each item
                                If (From a In myCalibPosToRemove Where a.Status = "FEW" _
                                                                 OrElse a.Status = "DEPLETED" _
                                                                 OrElse a.Status = "INPROCESS" Select a).Count > 0 Then
                                    mySelectedElementInfoRow.AllowedActionInPause = False
                                End If
                            End If
                        End If
                    Next
                    'If there are not elements with AllowedActionInPause property = true then return false.
                    If (From a In pSelectedElementInfo.twksWSRotorContentByPosition Where a.AllowedActionInPause Select a).Count = 0 Then
                        myResult = False
                    End If

                ElseIf AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                    'Status in Running mode to validate the allowed action on  running.
                    If mySelectedElementInfo.twksWSRotorContentByPosition.Where(Function(a) a.RotorType = "SAMPLES" AndAlso _
                                                                                  (a.Status = "INPROCESS" OrElse _
                                                                                   a.Status = "PENDING" OrElse _
                                                                                   a.Status = "DEPLETED")).Count > 0 Then
                        myResult = False
                    End If
                Else
                    'Set the default value define on ScreenBlockStatus.
                    myResult = isManualBarcodeAllowed
                End If
            Else

                myResult = False
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateAllowedActionInPause", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateAllowedActionInPause", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function



    ''' <summary>
    ''' Method to clean and disable all controls in Info Area Sections when the screen is loaded, or to clean 
    ''' and disable all controls in the Info Area of the current Rotor after the Reset of it
    ''' </summary>
    ''' <param name="pAfterReset">When True indicates the method is called after Reset the current Rotor or the
    '''                           current Rotor Position; when False, it indicates the method is called to 
    '''                           initial Screen Load</param>
    ''' <param name="pExcludeBarCode">When TRUE the barcode controls remains enabled for manual barcode entry</param>
    ''' <remarks>
    ''' Created by:  SA 23/12/2009
    ''' </remarks>
    Private Sub CleanInfoArea(ByVal pAfterReset As Boolean, Optional ByVal pExcludeBarCode As Boolean = False)
        Try
            'Info Area for Controls, Calibrators and Patient Samples
            If (Not pAfterReset OrElse myRotorTypeForm = "SAMPLES") Then
                bsSampleCellTextBox.Clear()
                bsSampleRingNumTextBox.Clear()
                bsSampleContentTextBox.Clear()
                bsSamplesBarcodeTextBox.Clear()
                bsSampleNumberTextBox.Clear()
                bsSampleIDTextBox.Clear()
                bsSampleTypeTextBox.Clear()
                bsDiluteStatusTextBox.Clear()
                bsTubeSizeComboBox.SelectedIndex = -1
                SamplesStatusTextBox.Clear() 'RH 01/08/2011

                'Disable all controls in the Info Area
                SetInfoAreaEnabledStatus("SAMPLES", False, pExcludeBarCode) 'AG 03/10/2011 - Only in this case add optional parameter for not disable barcode controls
            End If

            'Info Area for Reagents and Additional Solutions
            If (Not pAfterReset OrElse myRotorTypeForm = "REAGENTS") Then
                bsReagentsCellTextBox.Clear()
                bsReagentsRingNumTextBox.Clear()
                bsReagentsContentTextBox.Clear()
                bsReagentsBarCodeTextBox.Clear()
                bsReagentNameTextBox.Clear()
                bsReagentsNumberTextBox.Clear()
                bsTestNameTextBox.Clear()
                bsExpirationDateTextBox.Clear()
                bsBottleSizeComboBox.SelectedIndex = -1
                bsCurrentVolTextBox.Clear()
                bsTeststLeftTextBox.Clear()
                ReagStatusTextBox.Clear() 'RH 01/08/2011

                'Disable all controls in the Info Area
                SetInfoAreaEnabledStatus("REAGENTS", False, pExcludeBarCode) 'AG 03/10/2011 - Only in this case add optional parameter for not disable barcode controls
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CleanInfoArea", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CleanInfoArea", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get the select cell position information on the local RotorContentByPosition.
    ''' </summary>
    ''' <param name="pRingNumber">Ring Number</param>
    ''' <param name="pCellNumber">Cell Number</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by:  TR 21/12/2009
    ''' Modified by: SA 23/12/2009 - The position is marked as Selected in the Screen Structure myRotorContentByPositionDSForm
    '''              TR 07/01/2010 - Added the functionality of single select or multiselect
    '''              AG 11/01/2010 - When multiple selection clear info area (Tested OK)
    ''' </remarks>
    Private Function GetLocalPositionInfo(ByVal pRingNumber As Integer, ByVal pCellNumber As Integer, ByVal pMultiSelection As Boolean) _
                                          As WSRotorContentByPositionDS

        Dim myRotorContentByPositionDS As New WSRotorContentByPositionDS

        Try
            'To get the information of the selected Rotor Position, filter the myRotorContentByPositionDSForm 
            'by the RotorType, RingNumber and CellNumber and store it in a DataSet

            'RH 05/08/2011 New version of the method, because the previous one does not work properly
            Dim mySelectedElement As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)

            If (Not pMultiSelection) Then
                mySelectedElement = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                                     Where a.Selected = True _
                                     AndAlso a.RotorType = myRotorTypeForm _
                                     Order By a.RingNumber Ascending, a.CellNumber Ascending _
                                     Select a).ToList()

                'Change the selected status to all previous elements to false
                For Each myRCPRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In mySelectedElement
                    myRCPRow.Selected = False
                    MarkSelectedPosition(myRCPRow.RingNumber, myRCPRow.CellNumber, False)
                Next
            End If

            'Get the current selected element
            mySelectedElement = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition.AsEnumerable _
                                 Where a.RotorType = myRotorTypeForm _
                                 AndAlso a.RingNumber = pRingNumber _
                                 AndAlso a.CellNumber = pCellNumber _
                                 Select a).ToList()

            'If currente element found, change selected status
            If (mySelectedElement.Count > 0) Then
                mySelectedElement.First().Selected = Not mySelectedElement.First().Selected
                'Mark the selected position on the rotor changing the backgroun image
                MarkSelectedPosition(mySelectedElement.First().RingNumber, mySelectedElement.First().CellNumber, mySelectedElement.First().Selected)
            End If

            'Get all selected elements to return on the result Dataset
            mySelectedElement = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                                 Where a.Selected = True _
                                 AndAlso a.RotorType = myRotorTypeForm _
                                 Order By a.RingNumber Ascending, a.CellNumber Ascending _
                                 Select a).ToList()

            For Each myRCPRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In mySelectedElement
                myRotorContentByPositionDS.twksWSRotorContentByPosition.ImportRow(myRCPRow)
            Next

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetLocalPositionInfo", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetLocalPositionInfo", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try

        Return myRotorContentByPositionDS
    End Function

    ''' <summary>
    ''' Mark the selected position on the rotor area.
    ''' Activate the Background color in case the position control is selected.
    ''' </summary>
    ''' <param name="pRingNumber">Ring Number</param>
    ''' <param name="pCellNumber">Cell Number</param>
    ''' <remarks>
    ''' Created by:  TR 07/01/2010 - Tested: OK
    ''' Modified by: TR 25/01/2010 - Add the validation of NO_INUSE status to set back the corresponding BackGroundImage.
    '''                              TODO: TR Need to implement this functionality with the reagents.
    '''              SA 14/07/2010 - Use the global variables for the Icon Names instead of the hardcoded name
    '''              AG 13/10/2010 - changes for use bottle images!!
    '''              RH 14/02/2011 - Code optimization. Every unneeded New and Typecast get out.
    '''              RH 05/08/2011 - BackgroundImage depends on the Status. Code optimization.
    '''              TR 08/11/2013 - BT #1358 => Added code to validate if TextBox for Barcode edition can be enabled in Pause mode.
    '''              SA 11/08/2013 - BT #1359 => Code changed to call a different function depending on the active Rotor Type
    ''' </remarks>
    Private Sub MarkSelectedPosition(ByVal pRingNumber As Integer, ByVal pCellNumber As Integer, ByVal pMarkPosition As Boolean)
        Try
            If (myRotorTypeForm = "SAMPLES") Then
                MarkSelectedPositionInSAMPLESRotor(pRingNumber, pCellNumber, pMarkPosition)
            Else
                MarkSelectedPositionInREAGENTSRotor(pRingNumber, pCellNumber, pMarkPosition)
            End If

            'Dim rotorPrefix As String
            'If (myRotorTypeForm = "SAMPLES") Then
            '    rotorPrefix = "Sam"

            '    bsSamplesBarcodeTextBox.Enabled = False
            '    bsSamplesBarcodeTextBox.BackColor = SystemColors.MenuBar
            'Else
            '    rotorPrefix = "Reag"
            'End If

            'BarCodeErrorSampleFree = False
            'BarCodeErrorReagentFree = False

            'Dim controlQuery As List(Of BSRImage)
            'Dim FilterName As String = String.Format("{0}{1}{2}", rotorPrefix, pRingNumber, pCellNumber)
            'controlQuery = (From b In myPosControlList.AsEnumerable _
            '               Where b.Name = FilterName _
            '              Select b).ToList()

            ''Get the content of the selected position
            'Dim posContent As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
            'posContent = (From c In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
            '             Where c.RotorType = myRotorTypeForm _
            '           AndAlso c.RingNumber = pRingNumber _
            '           AndAlso c.CellNumber = pCellNumber _
            '            Select c).ToList()

            'If (controlQuery.Count > 0) AndAlso (posContent.Count > 0) Then
            '    If (pMarkPosition) Then
            '        If (myRotorTypeForm = "SAMPLES") Then
            '            Dim myTubeContent As String = posContent.First.TubeContent

            '            'BT#1358 - Check if the TextBox for Samples Barcode has to be disabled
            '            Dim actionAllowed As Boolean = True
            '            If (myTubeContent = "PATIENT") Then
            '                actionAllowed = VerifyActionsAllowedInSampleRotor(mySelectedElementInfo, False)
            '            End If

            '            bsSamplesBarcodeTextBox.Enabled = actionAllowed
            '            If (actionAllowed) Then
            '                bsSamplesBarcodeTextBox.BackColor = Color.White
            '            Else
            '                bsSamplesBarcodeTextBox.BackColor = SystemColors.MenuBar
            '            End If
            '            'TR 08/11/2013 BT#1358 -END.

            '            'Enable/Disable sample barcode 
            '            If (String.Equals(posContent.First.Status, "FREE") Or String.Equals(myTubeContent, "PATIENT")) Then
            '                bsSamplesBarcodeTextBox.Enabled = True
            '                bsSamplesBarcodeTextBox.BackColor = Color.White
            '            End If
            '            'DL 11/10/2011

            '            If (String.Equals(myTubeContent, "PATIENT") Or String.Equals(myTubeContent, "TUBE_SPEC_SOL") Or _
            '                String.Equals(myTubeContent, "TUBE_WASH_SOL") Or String.Equals(myTubeContent, "WASH_SOL") Or _
            '                String.Equals(myTubeContent, "SPEC_SOL") Or String.Equals(myTubeContent, "CALIB") Or _
            '                String.Equals(myTubeContent, "CTRL")) Then

            '                If String.Equals(posContent.First.BarcodeStatus, "ERROR") Then
            '                    controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLEBCERR_SEL_IconName)

            '                Else
            '                    Select Case posContent.First.Status
            '                        Case "PENDING"
            '                            controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLEPENDING_SEL_IconName)

            '                        Case "DEPLETED"
            '                            controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLEDEPLETED_SEL_IconName)

            '                        Case "NO_INUSE"
            '                            controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLENOINUSE_SEL_IconName)

            '                        Case "FINISHED"
            '                            controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLEFINISHED_SEL_IconName)

            '                        Case "INPROCESS"
            '                            controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLEINPROCES_SEL_IconName)

            '                            'Case "FREE", "PATIENT"
            '                            'bsSamplesDeletePosButton.Enabled = True
            '                            'Case "BARERROR"
            '                            'Case "FEW"

            '                    End Select
            '                End If

            '            Else
            '                If String.Equals(posContent.First.BarcodeStatus, "ERROR") Then
            '                    controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & BTLSAMPLEBCERR_SEL_IconName)

            '                    If String.Equals(posContent.First.Status, "FREE") Then BarCodeErrorSampleFree = True
            '                Else
            '                    controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & EMPTYCELL_IconName)
            '                End If

            '            End If
            '            'DL 27/09/2011

            '        Else 'REAGENTS Rotor
            '            If String.Equals(posContent.First.Status, "FREE") Then
            '                If pRingNumber = 1 Then
            '                    Select Case posContent.First.BarcodeStatus
            '                        Case "ERROR"
            '                            BarCodeErrorReagentFree = True
            '                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCERRSMALLR1_SEL_IconName)

            '                        Case "UNKNOWN"
            '                            BarCodeErrorReagentFree = True
            '                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCUKNSMALLR1_SEL_IconName)

            '                        Case Else
            '                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BOTTLE2_EMPTYCELL_IconName)
            '                    End Select

            '                Else
            '                    Select Case posContent.First.BarcodeStatus
            '                        Case "ERROR"
            '                            BarCodeErrorReagentFree = True
            '                            If String.Equals(posContent.First.TubeType, "BOTTLE2") Then
            '                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCERRSMALLR2_SEL_IconName)

            '                            ElseIf String.Equals(posContent.First.TubeType, "BOTTLE3") Then
            '                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCERRBIGR2_SEL_IconName)
            '                            End If

            '                        Case "UNKNOWN"
            '                            BarCodeErrorReagentFree = True
            '                            If String.Equals(posContent.First.TubeType, "BOTTLE2") Then
            '                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCUKNSMALLR2_SEL_IconName)

            '                            ElseIf String.Equals(posContent.First.TubeType, "BOTTLE3") Then
            '                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCUKNBIGR2_SEL_IconName)
            '                            End If

            '                        Case Else
            '                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BOTTLE3_EMPTYCELL_IconName)
            '                    End Select

            '                End If

            '            ElseIf String.Equals(posContent.First.TubeType, "BOTTLE2") OrElse String.Equals(posContent.First.TubeType, "BOTTLE1") Then
            '                If pRingNumber = 1 Then
            '                    Select Case posContent.First.BarcodeStatus
            '                        Case "ERROR"
            '                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCERRSMALLR1_SEL_IconName)

            '                        Case "UNKNOWN"
            '                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCUKNSMALLR1_SEL_IconName)

            '                        Case Else
            '                            Select Case posContent.First.Status
            '                                Case "INUSE", "IN_USE"
            '                                    Select Case posContent.First.TubeContent
            '                                        Case "WASH_SOL", "SPEC_SOL", "TUBE_SPEC_SOL"
            '                                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLADDSOLSMALLR1_SEL_IconName)

            '                                        Case "REAGENT"
            '                                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLREAGSMALLD1_SEL_IconName)

            '                                    End Select

            '                                Case "DEPLETED", "LOCKED" 'TR 29/09/2012 add the locked status to do the same as deplete 
            '                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLDEPLETSMALLR1_SEL_IconName)

            '                                Case "FEW"
            '                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLFEWSMALLR1_SEL_IconName)

            '                                Case "NO_INUSE"
            '                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLNUSESMALLR1_SEL_IconName)
            '                            End Select
            '                    End Select
            '                Else
            '                    Select Case posContent.First.BarcodeStatus
            '                        Case "ERROR"
            '                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCERRSMALLR2_SEL_IconName)

            '                        Case "UNKNOWN"
            '                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCUKNSMALLR2_SEL_IconName)

            '                        Case Else
            '                            Select Case posContent.First.Status
            '                                Case "INUSE", "IN_USE"
            '                                    Select Case posContent.First.TubeContent
            '                                        Case "WASH_SOL", "SPEC_SOL", "TUBE_SPEC_SOL"
            '                                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLADDSOLSMALLR2_SEL_IconName)

            '                                        Case "REAGENT"
            '                                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLREAGSMALLD2_SEL_IconName)

            '                                    End Select

            '                                Case "DEPLETED", "LOCKED" 'TR 29/09/2012 add the locked status to do the same as deplete 
            '                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLDEPLETSMALLR2_SEL_IconName)

            '                                Case "FEW"
            '                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLFEWSMALLR2_SEL_IconName)

            '                                Case "NO_INUSE"
            '                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLNUSESMALLR2_SEL_IconName)

            '                            End Select
            '                    End Select
            '                End If

            '            ElseIf posContent.First.TubeType = "BOTTLE3" Then
            '                Select Case posContent.First.BarcodeStatus
            '                    Case "ERROR"
            '                        ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCERRBIGR2_SEL_IconName)

            '                    Case "UNKNOWN"
            '                        ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCUKNBIGR2_SEL_IconName)

            '                    Case Else
            '                        Select Case posContent.First.Status
            '                            Case "INUSE", "IN_USE"
            '                                Select Case posContent.First.TubeContent
            '                                    Case "WASH_SOL", "SPEC_SOL", "TUBE_SPEC_SOL"
            '                                        ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLADSLBIGR2SEL_IconName)

            '                                    Case "REAGENT"
            '                                        ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLREAGBIG2_SEL_IconName)

            '                                End Select

            '                            Case "DEPLETED", "LOCKED" 'TR 28/09/2012 add the locked status to do the same as deplete 
            '                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLDEPLBIGR2_SEL_IconName)

            '                            Case "FEW"
            '                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLFEWBIGR2_SEL_IconName)

            '                            Case "NO_INUSE"
            '                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLNUSEBIGR2_SEL_IconName)

            '                        End Select
            '                End Select

            '            End If
            '        End If

            '        Else
            '            If String.Equals(rotorPrefix, "Reag") Then 'REAGENTS Rotor
            '                If String.Equals(rotorPrefix, "Reag") Then
            '                    If String.Equals(posContent.First.Status, "FREE") Then
            '                        If String.Equals(posContent.First.BarcodeStatus, "ERROR") Or String.Equals(posContent.First.BarcodeStatus, "UNKNOWN") Then
            '                            BarCodeErrorReagentFree = True
            '                            'Else
            '                            '    controlQuery.First.BackgroundImage = Image.FromFile(MyBase.IconsPath & EMPTYCELL_IconName)
            '                        End If

            '                        ChangeControlPositionImage(controlQuery.First, Nothing, True)

            '                    ElseIf String.Equals(posContent.First.TubeContent, "REAGENT") Then
            '                        If String.Equals(posContent.First.TubeType, "BOTTLE2") OrElse String.Equals(posContent.First.TubeType, "BOTTLE1") Then
            '                            If pRingNumber = 1 Then
            '                                Me.ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLREAGSMALLD1_IconName) 'BOTTLE2_IconName)
            '                            Else
            '                                Me.ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLREAGSMALLD2_IconName) 'BOTTLE2_R2_IconName)
            '                            End If
            '                        ElseIf String.Equals(posContent.First.TubeType, "BOTTLE3") Then
            '                            Me.ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLREAGBIG2_IconName)
            '                        End If

            '                    Else 'Additional Solution
            '                        If String.Equals(posContent.First.TubeType, "BOTTLE2") OrElse String.Equals(posContent.First.TubeType, "BOTTLE1") Then
            '                            If pRingNumber = 1 Then
            '                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLADDSOLSMALLR1_IconName) 'BOTTLE2_ADDSOL_IconName)
            '                            Else
            '                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLADDSOLSMALLR2_IconName) 'BOTTLE2_R2_ADDSOL_IconName)
            '                            End If

            '                        ElseIf String.Equals(posContent.First.TubeType, "BOTTLE3") Then
            '                            ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLADDSOLBIGR2_IconName) 'BOTTLE3_ADDSOL_IconName)
            '                        End If

            '                    End If
            '                End If
            '                'Else 'SAMPLES Rotor
            '            End If

            '            SetPosControlBackGround(controlQuery.First(), _
            '                                    posContent.First.Status, _
            '                                    posContent.First.RotorType, _
            '                                    posContent.First.TubeType, _
            '                                    posContent.First.RingNumber, _
            '                                    posContent.First.BarcodeStatus)

            '        End If
            'Else
            '    If (controlQuery.Count > 0) Then controlQuery.First.BackgroundImage = Nothing
            'End If

            ''DL 05/10/2011
            'Dim hasSelect As EnumerableRowCollection(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
            'hasSelect = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
            '             Where a.RotorType = myRotorTypeForm _
            '             AndAlso a.Selected = True _
            '             Select a)

            'Select Case myRotorTypeForm
            '    Case "SAMPLES"
            '        If hasSelect.Count > 0 Then
            '            bsSamplesMoveFirstPositionButton.Enabled = True
            '            bsSamplesMoveLastPositionButton.Enabled = True
            '            bsSamplesDecreaseButton.Enabled = True
            '            bsSamplesIncreaseButton.Enabled = True
            '        Else
            '            bsSamplesMoveFirstPositionButton.Enabled = False
            '            bsSamplesMoveLastPositionButton.Enabled = False
            '            bsSamplesDecreaseButton.Enabled = False
            '            bsSamplesIncreaseButton.Enabled = False
            '        End If

            '    Case "REAGENTS"
            '        If hasSelect.Count > 0 Then
            '            bsReagentsMoveFirstPositionButton.Enabled = True
            '            bsReagentsMoveLastPositionButton.Enabled = True
            '            bsReagentsDecreaseButton.Enabled = True
            '            bsReagentsIncreaseButton.Enabled = True
            '        Else
            '            bsReagentsMoveFirstPositionButton.Enabled = False
            '            bsReagentsMoveLastPositionButton.Enabled = False
            '            bsReagentsDecreaseButton.Enabled = False
            '            bsReagentsIncreaseButton.Enabled = False
            '        End If
            'End Select
            ''DL 05/10/2011

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".MarkSelectedPosition", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".MarkSelectedPosition", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
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
    ''' Created by:  SA 18/11/2013 - Code for SAMPLES Rotor moved from function MarkSelectedPosition   
    ''' </remarks>
    Private Sub MarkSelectedPositionInSAMPLESRotor(ByVal pRingNumber As Integer, ByVal pCellNumber As Integer, ByVal pMarkPosition As Boolean)
        Try
            BarCodeErrorSampleFree = False
            BarCodeErrorReagentFree = False

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
                    'Set the proper Cell Background when Cell Rotor has been selected
                    Dim myTubeContent As String = posContent.First.TubeContent

                    'BT#1358 - Check if the TextBox for Samples Barcode can be enabled (possible only for FREE cells or cells containing PATIENT Tubes, 
                    '          rest of Elements do not have Barcode)
                    Dim actionAllowed As Boolean = False
                    If (posContent.First.Status = "FREE") Then
                        actionAllowed = True
                    ElseIf (myTubeContent = "PATIENT") Then
                        actionAllowed = VerifyActionsAllowedInSampleRotor(mySelectedElementInfo, False)
                    End If

                    bsSamplesBarcodeTextBox.Enabled = actionAllowed
                    If (actionAllowed) Then
                        bsSamplesBarcodeTextBox.BackColor = Color.White
                    Else
                        bsSamplesBarcodeTextBox.BackColor = SystemColors.MenuBar
                    End If

                    'Load the proper Cell Background depending on both, the Tube Status and the BarcodeStatus
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
                            If (posContent.First.Status = "FREE") Then BarCodeErrorSampleFree = True
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
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".MarkSelectedPositionInSAMPLESRotor", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".MarkSelectedPositionInSAMPLESRotor", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
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
    ''' Created by:  SA 18/11/2013 - Code for REAGENTS Rotor moved from function MarkSelectedPosition. Additionally, added changes needed for:
    '''                              BT #1359 ==> When Status is DEPLETED, LOCKED or FEW, previous code is valid 
    '''                                           only when pInProcessBottle is FALSE; otherwise, new icons showing Reagents and/or Washing Solutions still needed
    '''                                           for the execution of the active Work Session have to be loaded and shown 
    ''' Modified by: SA 04/12/2013 - Include BarCodeStatus = EMPTY in the list of conditions for Bottle without Barcode or with a correct one
    ''' </remarks>
    Private Sub MarkSelectedPositionInREAGENTSRotor(ByVal pRingNumber As Integer, ByVal pCellNumber As Integer, ByVal pMarkPosition As Boolean)
        Try
            BarCodeErrorSampleFree = False
            BarCodeErrorReagentFree = False

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
                                If (posContent.First.Status = "FREE") Then BarCodeErrorReagentFree = True
                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCERRSMALLR1_SEL_IconName)
                            ElseIf (posContent.First.BarcodeStatus = "UNKNOWN") Then
                                If (posContent.First.Status = "FREE") Then BarCodeErrorReagentFree = True
                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCUKNSMALLR1_SEL_IconName)
                            Else
                                ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BOTTLE2_EMPTYCELL_IconName)
                            End If
                        Else
                            'In the second Ring, both Bottle Sizes are allowed
                            If (posContent.First.BarcodeStatus = "ERROR") Then
                                If (posContent.First.Status = "FREE") Then BarCodeErrorReagentFree = True
                                If (posContent.First.TubeType = "BOTTLE2") Then
                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCERRSMALLR2_SEL_IconName)

                                ElseIf (posContent.First.TubeType = "BOTTLE3") Then
                                    ChangeControlPositionImage(controlQuery.First, MyBase.IconsPath & BTLBCERRBIGR2_SEL_IconName)
                                End If
                            ElseIf (posContent.First.BarcodeStatus = "UNKNOWN") Then
                                If (posContent.First.Status = "FREE") Then BarCodeErrorReagentFree = True
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
                        If (posContent.First.BarcodeStatus = "ERROR") OrElse (posContent.First.BarcodeStatus = "UNKNOWN") Then
                            BarCodeErrorReagentFree = True
                        End If
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
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".MarkSelectedPositionInREAGENTSRotor", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".MarkSelectedPositionInREAGENTSRotor", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate if a position on the Rotor is free
    ''' </summary>
    ''' <param name="pRingNumber" ></param>
    ''' <param name="pCellNumber" ></param>
    ''' <param name="pCellBarcodeStatus" ></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by:  TR 05/01/2010 - Tested: OK
    ''' Modified by: TR 07/01/2010 - Change the way the position was validated implementing LINQ
    '''              RH 13/09/2011 - Added new conditions: Position with barcode status = NULL or EMPTY or UNKNOWN or ERROR
    '''              AG 06/10/2011 - Added ByRef parameter pCellStatus
    '''              AG 29/05/2012 - Linq syntax has been corrected (previous versions returned always 1 record by position-rotor type + all records with bottle UNKOWN)
    ''' </remarks>
    Private Function IsPositionFree(ByVal pRingNumber As Integer, ByVal pCellNumber As Integer, Optional ByRef pCellBarcodeStatus As String = "", _
                                    Optional ByRef pCellBarcodeInfo As String = "") As Boolean
        Try
            Dim query1 As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
            If (Not myRotorContentByPositionDSForm Is Nothing) Then
                query1 = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                         Where a.RotorType = myRotorTypeForm _
                       AndAlso a.RingNumber = pRingNumber _
                       AndAlso a.CellNumber = pCellNumber _
                       AndAlso ((a.Status = "FREE" AndAlso (a.IsBarcodeStatusNull OrElse a.BarcodeStatus = "EMPTY" OrElse a.BarcodeStatus = "ERROR")) _
                         OrElse (a.Status = "NO_INUSE" AndAlso (Not a.IsBarcodeStatusNull AndAlso a.BarcodeStatus = "UNKNOWN"))) _
                        Select a).ToList()

                If (query1.Count > 0 AndAlso Not query1.First.IsBarcodeStatusNull) Then pCellBarcodeStatus = query1.First.BarcodeStatus
                If (query1.Count > 0 AndAlso Not query1.First.IsBarCodeInfoNull) Then pCellBarcodeInfo = query1.First.BarCodeInfo
                Return (query1.Count > 0)
            Else
                Return (0)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IsPositionFree ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IsPositionFree", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return False
    End Function

    ''' <summary>
    ''' Move an item placed in a Rotor Position to a new Rotor Position
    ''' </summary>
    ''' <param name="pRingNumber">Ring Number of the new Rotor Position</param>
    ''' <param name="pCellNumber">Cell Number of the new Rotor Position</param>
    ''' <param name="pCellBarCodeStatus"></param>
    ''' <remarks>
    ''' Created by:  BK 08/12/2009 
    ''' Modified by: BK 17/12/2009
    '''              TR 21/12/2009 - Function rewrote 
    '''              VR 24/12/2009 - Tested: OK
    '''              VR 06/01/2010 - Tested: OK
    '''              TR 07/01/2010 -
    '''              AG 06/10/2011 - Added parameter pCellBarcodeStatus  
    '''              XB 27/08/2013 - BT #1266 => Added cell number update 
    ''' </remarks>
    Private Sub ChangeElementPosition(ByVal pRingNumber As Integer, ByVal pCellNumber As Integer, ByVal pCellBarCodeStatus As String, ByVal pCellBarCodeInfo As String)
        Try
            If (mySelectedElementInfo.twksWSRotorContentByPosition.Rows.Count > 0) Then 'Modified: VR 24/12/2009 - Patch Added
                'RH 13/09/2011 Do nothing if it is the same position
                If mySelectedElementInfo.twksWSRotorContentByPosition(0).RingNumber = pRingNumber AndAlso _
                mySelectedElementInfo.twksWSRotorContentByPosition(0).CellNumber = pCellNumber Then Return

                'RH 10/10/2011 For Reagents, inner ring to outer ring position changing is not allowed
                'when TubeType = "BOTTLE3" and BarCodeInfo <> String.Empty
                If String.Compare(myRotorTypeForm, "REAGENTS", False) = 0 Then
                    If (mySelectedElementInfo.twksWSRotorContentByPosition(0).RingNumber > pRingNumber) _
                    AndAlso (mySelectedElementInfo.twksWSRotorContentByPosition(0).TubeType = "BOTTLE3") _
                    AndAlso (mySelectedElementInfo.twksWSRotorContentByPosition(0).BarCodeInfo <> String.Empty) Then Return
                End If

                Dim myGlobalDataTO As GlobalDataTO
                Dim myRotorContentByPosition As New WSRotorContentByPositionDelegate

                myGlobalDataTO = myRotorContentByPosition.ChangeElementPosition(Nothing, mySelectedElementInfo, pRingNumber, pCellNumber, pCellBarCodeStatus, pCellBarCodeInfo)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myRotorContentByPositionDS As WSRotorContentByPositionDS = DirectCast(myGlobalDataTO.SetDatos, WSRotorContentByPositionDS)

                    Dim myCurrentSampleType = bsSampleTypeTextBox.Text ' XB 27/08/2013

                    UpdateRotorTreeViewArea(myRotorContentByPositionDS)
                    ClearSelection() 'TR 20/01/2010 Clean selection 

                    'TR 19/01/2010 After changing position get the information for the new position
                    mySelectedElementInfo = GetLocalPositionInfo(pRingNumber, pCellNumber, False)

                    'TR 09/03/2012 -Validate if element status is Pos then Update the status for the same element id to POS
                    If (mySelectedElementInfo.twksWSRotorContentByPosition(0).ElementStatus = "POS") Then
                        SetElementsStatusByElementID(AnalyzerIDAttribute, WorkSessionIDAttribute, _
                                                 mySelectedElementInfo.twksWSRotorContentByPosition(0).ElementID, "POS")
                    ElseIf mySelectedElementInfo.twksWSRotorContentByPosition(0).ElementStatus = "INCOMPLETE" Then
                        SetElementsStatusByElementID(AnalyzerIDAttribute, WorkSessionIDAttribute, _
                                                 mySelectedElementInfo.twksWSRotorContentByPosition(0).ElementID, "INCOMPLETE")
                    End If
                    'TR 09/03/2012 -END 

                    ' XB 27/08/2013
                    Dim myBarcodePositionsWithNoRequestsDelegate As New BarcodePositionsWithNoRequestsDelegate
                    myGlobalDataTO = myBarcodePositionsWithNoRequestsDelegate.UpdateCellNumber(Nothing, pCellNumber, _
                                                                                               mySelectedElementInfo.twksWSRotorContentByPosition(0).AnalyzerID, _
                                                                                               mySelectedElementInfo.twksWSRotorContentByPosition(0).WorkSessionID, _
                                                                                               mySelectedElementInfo.twksWSRotorContentByPosition(0).RotorType, _
                                                                                               mySelectedElementInfo.twksWSRotorContentByPosition(0).BarCodeInfo, _
                                                                                               myCurrentSampleType)
                    If (myGlobalDataTO.HasError) Then
                        ShowMessage(Me.Name & ".ChangeElementPosition", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                        Exit Try
                    End If
                    ' XB 27/08/2013

                    'Reload the info area with the new information 
                    ShowPositionInfoArea(AnalyzerIDAttribute, myRotorTypeForm, pRingNumber, pCellNumber)
                    'TR 19/01/2010 END
                Else
                    ShowMessage(Me.Name & ".ChangeElementPosition", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ChangeElementPosition ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ChangeElementPosition", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Delete the selected position in the Rotor. This function is execute only for Samples rotor    
    ''' </summary>
    ''' <remarks>
    ''' Created by:  VR 27/11/2009
    ''' Modified by: BK 23/12/2009 - Changed FixedItemDesc to MessageText
    '''              VR 31/12/2009 - (Tested: OK Partial) Reagent Flow is pending for test
    '''              VR 06/01/2010 - Tested : OK
    '''              TR 11/01/2010 - Added new funtionality to update the Treeview Element status
    '''                            - Removed duplicated functionality for the selected elements
    '''              TR 12/01/2010 - Implemented messages before deleting
    '''              TR 03/02/2010 - Validate the RotorType before complete DeletePosition function is executed
    '''              SA 26/07/2010 - Called new function PendingPositionedElements to enable/disable the Warnings Button 
    '''              AG 14/03/2011 - Changed function definition to Private
    '''              DL 23/02/2012 - Unselect all empty cells in the DS of Selected Positions to remove them from the deletion process
    '''              TR 07/11/2013 - BT#1358 => Changed call to function CompleteDeletePositionSelected to pass as parameter the flag indicating  
    '''                                         if the Analyzer is in PAUSE mode
    ''' </remarks>
    Private Sub DeleteSelectedPositions(ByVal plocalDs As WSRotorContentByPositionDS)
        Try
            Dim myMessageID As String = String.Empty
            Dim myGlobalDataTo As GlobalDataTO
            Dim myRotorContentByPosDelegate As New WSRotorContentByPositionDelegate

            'Unselect all empty cells
            If (Not mySelectedElementInfo Is Nothing AndAlso mySelectedElementInfo.twksWSRotorContentByPosition.Rows.Count > 0) Then
                For Each rcpROW As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In mySelectedElementInfo.twksWSRotorContentByPosition.Rows
                    If (rcpROW.IsElementIDNull) Then
                        'Clear the selected position on the Rotor
                        MarkSelectedPosition(rcpROW.RingNumber, rcpROW.CellNumber, False)
                    End If
                Next
            End If

            'BT#1358 - Verify if warning message "Action will be performed only on Elements no needed by the worksession" has to be shown. 
            '          Pass flag AllowScanInRunning when calling function CompleteDeletePositionSelected.
            ShowActionMessageifApply()
            myGlobalDataTo = myRotorContentByPosDelegate.CompleteDeletePositionSeleted(Nothing, mySelectedElementInfo, myMessageID, AnalyzerController.Instance.Analyzer.AllowScanInRunning) '#REFACTORING
            If (Not myGlobalDataTo.HasError AndAlso Not myGlobalDataTo.SetDatos Is Nothing) Then
                Dim myResultDialog As DialogResult
                If (myMessageID = String.Empty) Then
                    myResultDialog = DialogResult.OK
                Else
                    myResultDialog = ShowMessage(Me.Name, myMessageID, , Me)
                End If

                ClearSelection()
                If (myResultDialog = DialogResult.OK) Then
                    Dim myResultRotorContentByPos As WSRotorContentByPositionDS = DirectCast(myGlobalDataTo.SetDatos, WSRotorContentByPositionDS)
                    If (myResultRotorContentByPos.twksWSRotorContentByPosition.Rows.Count > 0) Then
                        'NOTE: the myResultRotorContentByPos is updated by reference on the method DeletePositions.
                        myGlobalDataTo = myRotorContentByPosDelegate.DeletePositions(Nothing, myResultRotorContentByPos, False)
                        If (Not myGlobalDataTo.HasError) Then
                            'Change the status in the TreeView. Filter all the received elements by ElementStatus = NOPOS
                            Dim qNoposElements = (From a In myResultRotorContentByPos.twksWSRotorContentByPosition _
                                                 Where a.ElementStatus = "NOPOS" _
                                                OrElse a.ElementStatus = "INCOMPLETE" _
                                                OrElse a.ElementStatus Is DBNull.Value _
                                                Select a).ToList()
                            If (Not bsElementsTreeView Is Nothing) Then
                                For Each rcpRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In qNoposElements
                                    ChangeTreeRotorElementsStatus(bsElementsTreeView.Nodes, False, rcpRow, myRotorTypeForm)
                                Next
                            End If
                            UpdateRotorTreeViewArea(CType(myGlobalDataTo.SetDatos, WSRotorContentByPositionDS))
                        Else
                            'Show error message
                            ShowMessage(Me.Name & ".DeleteSelectedPositions", myGlobalDataTo.ErrorCode, myGlobalDataTo.ErrorMessage, Me)
                        End If
                    End If
                End If
            Else
                'Show error message
                ShowMessage(Me.Name & ".DeleteSelectedPositions", myGlobalDataTo.ErrorCode, myGlobalDataTo.ErrorMessage, Me)
            End If

            'Select the first Element node on the TreeView
            If (bsElementsTreeView.Nodes.Count > 0) Then
                Dim myNodes As TreeNodeCollection = bsElementsTreeView.Nodes
                bsElementsTreeView.SelectedNode = myNodes(0)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DeleteSelectedPositions ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DeleteSelectedPositions", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Change the bottle size of a selected element. The new bottle size is passed as a parameter
    ''' </summary>
    ''' <param name="pRingNumber">Ring Number</param>
    ''' <param name="pCellNumber">Cell Number</param>
    ''' <param name="pBottleSize">New bottle Size</param>
    ''' <remarks>
    ''' Created by:  BK 08/12/2009 
    ''' Modified by: BK 21/12/2009 - Changes required after AG's Clarification, Use Local declaration to test the data
    '''              VR 24/12/2009 - Tested: OK
    '''              AG 05/01/2010 - Test in BCN (Reagents AND Samples) and call refresh screen (TESTED: OK)
    '''              AG 05/01/2010 - Integrated 
    '''              TR 11/01/2010 - Delete the call to functions GetAllTubeSizes and GetNextBottleSize. These calls are not needed because the new 
    '''                              bottle/tube size is received as parameter (pBottleSize)
    '''              RH 13/09/2011 - BarCode information is removed when a bottle/tube is changed
    '''              SA 13/02/2012 - Call to function UpdateRotorTreeViewArea to update Status of all positions and elements affected by the change
    '''              SA 15/11/2012 - Set BarcodeInfo to NULL instead of to String.Empty. Set also BarcodeStatus to NULL
    '''              TR 10/04/2013 - Barcode information is removed when a bottle/tube is changed only for positions that have been scanned (Barcodes manually 
    '''                              informed are not deleted)
    ''' </remarks>
    Private Sub ChangeBottleSize(ByVal pRingNumber As Integer, ByVal pCellNumber As Integer, ByVal pBottleSize As String)
        Try
            Dim query As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
            query = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                    Where a.RotorType = myRotorTypeForm _
                  AndAlso a.AnalyzerID = AnalyzerIDAttribute _
                  AndAlso a.WorkSessionID = WorkSessionIDAttribute _
                  AndAlso a.RingNumber = pRingNumber _
                  AndAlso a.CellNumber = pCellNumber _
                   Select a).ToList()

            If (query.Count = 1) Then
                query.First.TubeType = pBottleSize

                'Barcode fields are removed only for scanned Patient Sample Tubes (Barcodes manually informed are not deleted - v2.0)
                'For Reagents Positions, this function is not called when the Rotor Position has a valid Barcode (manually informed or
                'scanned), due to it is not allowed to change the Bottle Size in this case
                If (Not query.First().IsScannedPositionNull AndAlso query.First().ScannedPosition) Then
                    query.First().SetBarCodeInfoNull()
                    query.First().SetBarcodeStatusNull()
                    query.First().SetScannedPositionNull()
                End If

                'Create a new WSRotorContentsByPositionDS and add the Datarow
                Dim myRotorContentByPositionDS As New WSRotorContentByPositionDS
                myRotorContentByPositionDS.twksWSRotorContentByPosition.ImportRow(query.First)

                'Change volume by position
                Dim myGlobalDataTO As GlobalDataTO
                Dim myRotorContentByPosition As New WSRotorContentByPositionDelegate

                myGlobalDataTO = myRotorContentByPosition.ChangeVolumeByPosition(Nothing, myRotorContentByPositionDS, False)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    UpdateRotorTreeViewArea(DirectCast(myGlobalDataTO.SetDatos, WSRotorContentByPositionDS))
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ChangeBottleSize " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("ChangeBottleSize", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' To Refill the Reagent / Sample Volume
    ''' </summary>
    ''' <param name="pWSRotorContentByPositionDS">the WSRotorContentByPositionDS Dataset</param>
    ''' <remarks>
    ''' Created by:  VR 01/11/2009
    ''' Modified by: VR 31/12/2009 - (Tested : OK)
    '''              AG 05/01/2010 - Test in BCN (Reagents AND Samples) and call refresh screen (TESTED: OK)
    '''              AG 28/01/2010 - Show info area after refill single position
    '''              AG 14/03/2011 - Define private
    '''              JV 04/12/2013 - #1384 use the ChangeVolumeByPosition with the last optional parameter.
    ''' </remarks>
    Private Sub RefillReagentSampleVolume(ByRef pWSRotorContentByPositionDS As WSRotorContentByPositionDS)
        Try
            Dim myGlobalDataTo As GlobalDataTO
            Dim newRotorContentPosDS As New WSRotorContentByPositionDS
            If (pWSRotorContentByPositionDS.twksWSRotorContentByPosition.Rows.Count > 0) Then
                Dim query1 As New List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                query1 = (From a In pWSRotorContentByPositionDS.twksWSRotorContentByPosition _
                            Select a Where a.Selected = True).ToList()

                For Each myRotorContenByPosRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In query1
                    newRotorContentPosDS.twksWSRotorContentByPosition.ImportRow(myRotorContenByPosRow)
                Next

                Dim myobj As New WSRotorContentByPositionDelegate
                'myGlobalDataTo = myobj.ChangeVolumeByPosition(Nothing, newRotorContentPosDS, True)
                myGlobalDataTo = myobj.ChangeVolumeByPosition(Nothing, newRotorContentPosDS, True, myRotorTypeForm = "REAGENTS" AndAlso (WorkSessionStatusAttribute = "EMPTY" OrElse WorkSessionStatusAttribute = "OPEN"))

                If (Not myGlobalDataTo.HasError AndAlso Not myGlobalDataTo.SetDatos Is Nothing) Then
                    newRotorContentPosDS = DirectCast(myGlobalDataTo.SetDatos, WSRotorContentByPositionDS)

                    '05/01/2010 AG - Refresh screen
                    UpdateRotorTreeViewArea(newRotorContentPosDS)
                    myGlobalDataTo.SetDatos = newRotorContentPosDS  'AG 05/01/2010

                    'TR 03/05/2012 -Update local values.
                    LoadRotorAreaInfo(WorkSessionIDAttribute, AnalyzerIDAttribute, False, False)
                    'TR 03/05/2012 -END.

                    'AG 28/01/2010 - show info area
                    If (newRotorContentPosDS.twksWSRotorContentByPosition.Rows.Count = 1) Then
                        ShowPositionInfoArea(AnalyzerIDAttribute, myRotorTypeForm, newRotorContentPosDS.twksWSRotorContentByPosition(0).RingNumber, _
                                             newRotorContentPosDS.twksWSRotorContentByPosition(0).CellNumber)
                        MarkSelectedPosition(newRotorContentPosDS.twksWSRotorContentByPosition(0).RingNumber, newRotorContentPosDS.twksWSRotorContentByPosition(0).CellNumber, True)
                    End If
                    'AG 28/01/2010 END
                Else
                    ShowMessage(Me.Name & ".RefillReagentSampleVolume", myGlobalDataTo.ErrorCode, myGlobalDataTo.ErrorMessage, Me)
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "RefillReagentSampleVolume " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RefillReagentSampleVolume", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Info area is refreshed for the new current cell selected by clicking MoveFirst, MovePrevious, MoveNext, MoveLast buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by:  BK 22/12/2009
    ''' Modified by: VR
    '''              VR 23/12/2009 (Pending : Testing) - ShowPositionInfoArea clears all the values
    '''                            Integrated: AG 24/12/2009 see notes in code
    '''              VR 30/12/2009 (Tested : OK) 
    '''              AG 05/01/2010 Done completly (Tested OK for Reagents and Samples)
    '''              AG 11/01/2010 When multiple selection clear info area (Tested OK)
    '''              SA 16/07/2010 Added Try/Catch; when no cell is selected in the current Rotor all fields in Info Area
    '''                            have to be disabled (parameter passed to StatusInfoAreaControls had a wrong value)
    '''              AG 14/03/2011 - define private
    ''' </remarks>
    Private Sub ScrollButtonsInfoArea(ByVal pButtonPress As Integer)
        Try
            'Filter the local dataset where is the rotor information. Filter by column Selected (and also by RotorType (myRotorTypeForm) - new AG 24/12/2009)
            ' Only one position selected is allowed in order to show InfoArea
            'Dim query As New List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
            Dim query As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
            query = (From e In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                    Where e.Selected _
                      AndAlso e.RotorType = myRotorTypeForm _
                   Select e).ToList

            If query.Count = 1 Then
                'AG 04/01/2010 -Get current cell and unselect it
                Dim CurrentCell As Integer = query.First().CellNumber
                query.First().Selected = False
                MarkSelectedPosition(query.First.RingNumber, query.First.CellNumber, query.First.Selected)  '11/01/2010 AG - UnMark the selected position

                'Get the maximum cell number for the current analyzer and rotor type
                Dim AnalyzerConfg As New AnalyzerModelRotorsConfigDelegate
                Dim maxCellRotor As Integer = 1
                'Dim myResultData As New GlobalDataTO
                Dim myResultData As GlobalDataTO
                myResultData = AnalyzerConfg.GetRotorMaxCellNumber(Nothing, AnalyzerIDAttribute, myRotorTypeForm)
                If Not myResultData.HasError Then
                    maxCellRotor = CType(myResultData.SetDatos, Integer)
                End If

                'Update local DataSet with then new selected cell according button pressed (only 1 cell can be selected)
                Select Case pButtonPress
                    Case 1  'FIRST
                        CurrentCell = 1
                    Case 2  'PREVIOUS
                        If CurrentCell > 1 Then CurrentCell = CurrentCell - 1
                    Case 3  'NEXT
                        If CurrentCell < maxCellRotor Then CurrentCell = CurrentCell + 1
                    Case 4  'LAST
                        CurrentCell = maxCellRotor
                End Select

                query = (From e In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                        Where e.CellNumber = CurrentCell _
                          AndAlso e.RotorType = myRotorTypeForm _
                       Select e).ToList

                If query.Count > 0 Then
                    query.First.Selected = True
                End If

                'Finally show position info area from NEW SELECTED position
                ShowPositionInfoArea(AnalyzerIDAttribute, myRotorTypeForm, query.First.RingNumber, query.First.CellNumber)
                MarkSelectedPosition(query.First.RingNumber, query.First.CellNumber, query.First.Selected)  '11/01/2010 AG - Mark the new selected position
                mySelectedElementInfo = GetLocalPositionInfo(query.First.RingNumber, query.First.CellNumber, False) 'TR 04/02/2010 Get the localposition info. into the selected element.
                'AG - 11/01/2010. When multiple selection NO info area is shown
            Else
                CleanInfoArea(False)
                Me.SetInfoAreaEnabledStatus(myRotorTypeForm, False)    '11/01/2010 AG
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ScrollButtonsInfoArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ScrollButtonsInfoArea ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the Screen and set the Status of controls in each Screen Block depending on the specified WorkSession/Analyzer state
    ''' </summary>
    ''' <param name="pAppStatus">WorkSession/Analyzer State. Possible values: EMPTY / PENDING / STANDBY / RUNNING / RUNNINGPAUSE / FINISHED / ABORTED</param>
    ''' <remarks>
    ''' Created by:  BK 11/12/2009 
    ''' Modified by: BK 21/12/2009
    '''              VR 08/01/2010 - Find out the Controls for Each block in progress
    '''              AG 15/01/2010 - TO DO: call this functions (Form_Load, after manual and auto positionning and deleting)
    '''              AG 28/01/2010 - myDataLoaded change depending the current rotor showed (samples or reagents)
    '''              RH 14/02/2011 - Code optimization. Every unneeded New and Typecast get out.
    '''              AG 14/03/2011 - Define function as private
    '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must be deleted to avoid Regional Settings problems (BugTracking #1112)
    '''              SA 15/10/2013 - BT #1334 ==> Added code to set pAppStatus = RUNNINGPAUSE when, besides pAppStatus=RUNNING, property of the AnalyzerManager AllowScanInRunning is 
    '''                              TRUE (PAUSE instruction has been sent to the Analyzer in Running). Function has been re-written to reduce unneeded calls to DB functions.
    ''' </remarks>
    Private Sub LoadScreenStatus(ByVal pAppStatus As String)
        Try
            If isClosingFlag Then Exit Sub ' XB 13/03/2014 - #1496 No refresh if screen is closing

            Dim myGlobalDataTO As GlobalDataTO

            If (pAppStatus = "RUNNING") Then
                'Verify if the Analyzer is in PAUSE during RUNNING
                If ((AnalyzerController.IsAnalyzerInstantiated) AndAlso AnalyzerController.Instance.Analyzer.AllowScanInRunning) Then pAppStatus = "RUNNINGPAUSE" '#REFACTORING
            ElseIf (pAppStatus = "OPEN") Then
                pAppStatus = "EMPTY"
            End If

            'Check if there are Elements loaded in both Rotors
            Dim myDataLoaded As Integer = 0
            Dim myNotFreePosition As New List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
            If (Not myRotorContentByPositionDSForm Is Nothing) Then
                myNotFreePosition = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                                    Where a.Status <> "FREE" _
                                   OrElse (a.Status = "FREE" AndAlso a.BarcodeStatus = "ERROR") _
                                   Select a).ToList()
                If (myNotFreePosition.Count > 0) Then myDataLoaded = 1
            End If
            If (myDataLoaded = 0 AndAlso pAppStatus <> "EMPTY" AndAlso pAppStatus <> "PENDING") Then myDataLoaded = 1
            myNotFreePosition = Nothing

            'Get the numeric value for the Level of the connected User 
            Dim myUsersLevel As New UsersLevelDelegate
            Dim currentUserNumericalLevel As Integer = -1

            myGlobalDataTO = myUsersLevel.GetCurrentUserNumericalLevel(Nothing)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                currentUserNumericalLevel = CType(myGlobalDataTO.SetDatos, Integer)
            End If

            'Get the Status of all Screen Blocks for the specified WorkSession/Analyzer state, depending if there are Elements loaded in both Rotors
            Dim myScreenBlockStatusDelegate As New ScreenBlockStatusDelegate
            myGlobalDataTO = myScreenBlockStatusDelegate.ReadByScreenAndAppStatus(Nothing, "WKS001", pAppStatus, myDataLoaded)

            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myScreenBlockStatusDS As ScreenBlockStatusDS = DirectCast(myGlobalDataTO.SetDatos, ScreenBlockStatusDS)

                Dim myBlockStatus As Boolean
                For Each row As ScreenBlockStatusDS.tfmwScreenBlockStatusRow In myScreenBlockStatusDS.tfmwScreenBlockStatus
                    myBlockStatus = row.BlockEnabled

                    If (Not row.IsLowerUserLevelNull) AndAlso (Not row.IsDefaultEnabledNull) Then
                        If (currentUserNumericalLevel < row.LowerUserLevel) Then
                            myBlockStatus = row.DefaultEnabled
                        End If
                    End If

                    Select Case (row.BlockID)
                        Case "AUTOPOS"
                            Me.bsSamplesAutoPosButton.Enabled = myBlockStatus
                            Me.bsReagentAutoPosButton.Enabled = myBlockStatus

                        Case "LOAD"
                            bsLoadVRotorButton.Enabled = myBlockStatus

                        Case "SAVE"
                            bsSaveVRotorButton.Enabled = myBlockStatus

                        Case "RESET"
                            bsResetRotorButton.Enabled = myBlockStatus
                            isResetRotorAllowed = myBlockStatus

                        Case "SCAN"
                            bsScanningButton.Enabled = myBlockStatus

                        Case "WARNING"
                            bsWarningsButton.Enabled = myBlockStatus

                        Case "INFOAREA"
                            'RH 26/09/2011 - InfoArea is always disabled by default. It is enabled only when the user clicks over a positioned element,
                            'and then flag isInfoAreaAllowed should be updated (True or False) in that moment.
                            SetInfoAreaEnabledStatus("SAMPLES", False, VerifyActionsAllowedInSampleRotor(mySelectedElementInfo, False))
                            SetInfoAreaEnabledStatus("REAGENTS", False, VerifyActionsAllowedInReagentsRotor())
                            isInfoAreaAllowed = False

                        Case "CHANGESIZE"
                            bsTubeSizeComboBox.Enabled = myBlockStatus
                            bsBottleSizeComboBox.Enabled = myBlockStatus
                            isChangeTubeSizeAllowed = myBlockStatus

                        Case "REFILL"
                            bsSamplesRefillPosButton.Enabled = myBlockStatus
                            bsReagentsRefillPosButton.Enabled = myBlockStatus
                            isSingleRefillPosAllowed = myBlockStatus

                        Case "DELETE"
                            bsSamplesDeletePosButton.Enabled = myBlockStatus
                            bsReagentsDeletePosButton.Enabled = myBlockStatus
                            isSingleDeletePosAllowed = myBlockStatus

                        Case "SCROLL"
                            'Nothing to do in this case: SCROLL buttons are always available

                        Case "ROTORDRAGDROP"
                            isRotorDragDropAllowed = myBlockStatus

                        Case "TREEDRAGDROP"
                            isTreeDragDropAllowed = myBlockStatus

                        Case "CHECKPOS"
                            'bsSamplesCheckVolumePosButton.Enabled = myBlockStatus
                            'bsReagentsCheckVolumePosButton.Enabled = myBlockStatus
                            'isSingleCheckPosAllowed = myBlockStatus

                        Case "ROTORCHECK"
                            'This functionality is not implemented
                            'If (myRotorTypeForm = "SAMPLES") Then
                            '    bsCheckRotorVolumeButton.Enabled = False
                            'Else
                            '    bsCheckRotorVolumeButton.Enabled = False
                            'End If
                            'isCheckRotorVolumeAllowed = myBlockStatus
                        Case "MANUALBARCODE"
                            'TR 26/11/2013 -BT #1404
                            bsSamplesBarcodeTextBox.Enabled = myBlockStatus
                            bsReagentsBarCodeTextBox.Enabled = myBlockStatus
                            isManualBarcodeAllowed = myBlockStatus

                        Case "REPORT"
                            'bsPrintButton.Enabled = myBlockStatus 

                        Case "RERUN"
                            'Nothing to do: there is not Rerun Button in current application version

                        Case "VIEWRESULTS"
                            'Nothing to do: there is not Results Button in current application version
                    End Select
                Next
            End If

            'Finally, check availability of buttons for Rotor Scanning and Check Volume of Rotor Positions
            ValidateScanningButtonEnabled()
            ValidateCheckRotorVolumeButtonEnabled()

            'Dim myScreenBlockDS As ScreenBlockDS
            'Dim myScreenBlock As New ScreenBlockDelegate
            'Dim myGloblaDataTO As GlobalDataTO
            'Dim myGloblaDataTO1 As GlobalDataTO

            ''Get Screen Identifier and all Screen Blocks to find the Status for each one of them
            'myGloblaDataTO = myScreenBlock.GetBlocksByScreen(Nothing, "WKS001")
            'If (Not myGloblaDataTO.HasError) Then
            '    myScreenBlockDS = CType(myGloblaDataTO.SetDatos, ScreenBlockDS)

            '    If (myScreenBlockDS.tfmwScreenBlocks.Rows.Count > 0) Then
            '        Dim myScreenBlockStatus As New ScreenBlockStatusDelegate

            '        For i As Integer = 0 To myScreenBlockDS.tfmwScreenBlocks.Rows.Count - 1
            '            'Get the Block Status according the Level of the current User
            '            myGloblaDataTO1 = myScreenBlockStatus.GetUserLevelBlockStatus(Nothing, myScreenBlockDS.tfmwScreenBlocks(i).ScreenID, myScreenBlockDS.tfmwScreenBlocks(i).BlockID, _
            '                                                                          pAppStatus, myDataLoaded)
            '            If (Not myGloblaDataTO1.HasError) Then
            '                Dim BlockStatus As Boolean = CType(myGloblaDataTO1.SetDatos, Boolean) 'RH 14/02/2011

            '                Select Case (myScreenBlockDS.tfmwScreenBlocks(i).BlockID)
            '                    Case "AUTOPOS"
            '                        Me.bsSamplesAutoPosButton.Enabled = BlockStatus
            '                        Me.bsReagentAutoPosButton.Enabled = BlockStatus

            '                    Case "CHANGESIZE"
            '                        bsTubeSizeComboBox.Enabled = BlockStatus
            '                        bsBottleSizeComboBox.Enabled = BlockStatus
            '                        isChangeTubeSizeAllowed = BlockStatus

            '                    Case "CHECKPOS"
            '                        'bsSamplesCheckVolumePosButton.Enabled = BlockStatus
            '                        bsReagentsCheckVolumePosButton.Enabled = BlockStatus
            '                        isSingleCheckPosAllowed = BlockStatus

            '                    Case "DELETE"
            '                        bsSamplesDeletePosButton.Enabled = BlockStatus
            '                        bsReagentsDeletePosButton.Enabled = BlockStatus
            '                        isSingleDeletePosAllowed = BlockStatus

            '                    Case "LOAD"
            '                        bsLoadVRotorButton.Enabled = BlockStatus

            '                    Case "REFILL"
            '                        bsSamplesRefillPosButton.Enabled = BlockStatus
            '                        bsReagentsRefillPosButton.Enabled = BlockStatus
            '                        isSingleRefillPosAllowed = BlockStatus

            '                    Case "RESET"
            '                        bsResetRotorButton.Enabled = BlockStatus
            '                        isResetRotorAllowed = BlockStatus

            '                    Case "ROTORCHECK"   'AG 18/01/2010
            '                        If (myRotorTypeForm = "SAMPLES") Then
            '                            bsCheckRotorVolumeButton.Enabled = False
            '                        Else
            '                            'bsCheckRotorVolumeButton.Enabled = BlockStatus
            '                            bsCheckRotorVolumeButton.Enabled = False 'TR 28/03/2012 funct for ver 5
            '                        End If
            '                        isCheckRotorVolumeAllowed = BlockStatus

            '                    Case "SCAN"
            '                        bsScanningButton.Enabled = BlockStatus

            '                    Case "SCROLL"
            '                        'AG 28/01/2010 - Scroll buttons are enabled or disabled depending on the Rotor Type:
            '                        '                ** Samples scroll button is disabled if Samples Rotor is free
            '                        '                ** Reagents scroll button is disabled if Reagents rotor is free

            '                        'Recalculate myDataLoaded for the Samples Rotor Type and get the status only if it is EMPTY or PENDING
            '                        If (pAppStatus <> "EMPTY" OrElse pAppStatus <> "PENDING") Then
            '                            myNotFreePosition = _
            '                                (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition.AsEnumerable _
            '                                 Where a.Status <> "FREE" AndAlso a.RotorType = "SAMPLES" Select a).ToList()

            '                            Dim myRotorDataLoaded As Integer = 0
            '                            If (myNotFreePosition.Count > 0) Then myRotorDataLoaded = 1

            '                            myGloblaDataTO = myScreenBlockStatus.GetUserLevelBlockStatus(Nothing, myScreenBlockDS.tfmwScreenBlocks(i).ScreenID, _
            '                                                                                         myScreenBlockDS.tfmwScreenBlocks(i).BlockID, _
            '                                                                                         pAppStatus, myRotorDataLoaded)
            '                            If (Not myGloblaDataTO.HasError) Then myGloblaDataTO1 = myGloblaDataTO
            '                        End If

            '                        BlockStatus = CType(myGloblaDataTO1.SetDatos, Boolean) 'RH 14/02/2011

            '                        'Recalculate myDataLoaded for the reagents rotortype and get this status only is EMPTY or PENDING
            '                        If pAppStatus <> "EMPTY" OrElse pAppStatus <> "PENDING" Then
            '                            myNotFreePosition = _
            '                                (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition.AsEnumerable _
            '                                 Where a.Status <> "FREE" AndAlso a.RotorType = "REAGENTS" Select a).ToList()

            '                            Dim myRotorDataLoaded As Integer = 0

            '                            If (myNotFreePosition.Count > 0) Then myRotorDataLoaded = 1

            '                            myGloblaDataTO = myScreenBlockStatus.GetUserLevelBlockStatus(Nothing, myScreenBlockDS.tfmwScreenBlocks(i).ScreenID, myScreenBlockDS.tfmwScreenBlocks(i).BlockID, _
            '                                             pAppStatus, myRotorDataLoaded)

            '                            If Not myGloblaDataTO.HasError Then myGloblaDataTO1 = myGloblaDataTO
            '                        End If

            '                        BlockStatus = CType(myGloblaDataTO1.SetDatos, Boolean) 'RH 14/02/2011

            '                    Case "WARNING"
            '                        bsWarningsButton.Enabled = BlockStatus

            '                    Case "REPORT"
            '                        'bsPrintButton.Enabled = BlockStatus DL 11/05/2012

            '                    Case "INFOAREA"
            '                        'AG 18/01/2010 - info area status (controls and functionality)
            '                        'SetInfoAreaEnabledStatus("SAMPLES", BlockStatus)
            '                        'SetInfoAreaEnabledStatus("REAGENTS", BlockStatus)
            '                        'isInfoAreaAllowed = BlockStatus

            '                        'RH 26/09/2011 InfoArea is allways disabled by default.
            '                        'It is only enabled when the user clicks over a positioned element.
            '                        'And so, isInfoAreaAllowed should be updated (True or False) in that moment.
            '                        SetInfoAreaEnabledStatus("SAMPLES", False)
            '                        SetInfoAreaEnabledStatus("REAGENTS", False)
            '                        isInfoAreaAllowed = False

            '                    Case "ROTORDRAGDROP"
            '                        isRotorDragDropAllowed = BlockStatus

            '                    Case "TREEDRAGDROP"
            '                        isTreeDragDropAllowed = BlockStatus

            '                    Case "RERUN"
            '                        'AG 15/01/2010 TO DO : NO button in this version

            '                    Case "VIEWRESULTS"
            '                        'AG 15/01/2010 TO DO : NO button in this version
            '                End Select
            '            End If
            '        Next

            '        'RH 18/02/2011
            '        UpdateRotorPositionedButtonsStatus()

            '    End If
            'End If

            ''AG 03/10/2011 - Enable button scan bar code, check volume ... 
            ''TODO
            ''bsCheckRotorVolumeButton, bsReagentsCheckVolumePosButton, bsSamplesCheckVolumePosButton
            'ValidateScanningButtonEnabled()
            'ValidateCheckRotorVolumeButtonEnabled()
            ''AG 03/10/2011
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.StackTrace + " - " + ex.HResult.ToString + "))", Me.Name & ".LoadScreenStatus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' To get the Child Controls of the Form 
    ''' </summary>
    ''' <param name="MyControlCollection">SplitContainer1 Controls</param>
    ''' <returns></returns>
    ''' <remarks>
    '''Created by:  VR 24/12/2009 (Testing : Pending)
    '''             AG 11/01/2010 (Integrated in BCN code)
    '''             AG 05/10/2010 change picture box for BSRImage
    ''' </remarks>
    Private Function SetValuesToControls(ByVal MyControlCollection As SplitContainer.ControlCollection) As List(Of Control)
        Dim myChildControls As New List(Of Control)
        Try
            For Each myControl As Control In MyControlCollection
                If (myControl.Controls.Count > 0) Then
                    If Not (TypeOf myControl Is SplitContainer) Then
                        '' Recursively call this method to add all child controls as well.
                        myChildControls.AddRange(SetValuesToControls(myControl.Controls))
                    Else
                        myChildControls = SetValuesToControls(myControl.Controls)
                    End If
                Else
                    'Get the control type to validate if it will go to our list (only for PictureBox)
                    If Not (TypeOf myControl Is BSRImage) AndAlso Not (TypeOf myControl Is PictureBox) Then
                        myChildControls.Add(myControl)
                    End If
                End If
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SetValuesToControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SetValuesToControls", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myChildControls
    End Function

    ''' <summary>
    ''' To Generate the Selected ROTOR Type DS and Save the generate Virtual Rotor DS 
    ''' </summary>
    ''' <param name="myFormRotorContentByPositionDSObj">Typed DS WSRotorContentByPositionDS containing information of all cell in both Rotors</param>
    ''' <remarks>
    ''' Created by:   VR 23/12/2009 - DEV: Pending (Only for Code ReStructure Not for flow - Flow is already completed)
    ''' Modified by : VR 05/10/2010 - Testing: OK .... Be carefull with the date marks and when doing copy/paste!! (07/01/2010) - (AG Note)
    '''               AG 08/01/2010 - Some corrections (Tested OK)
    '''               AG 14/01/2010-  If there are elements NOT_INUSE, get the identificator (ReagentID, SolutionCode, CalibratorID,...)
    '''               AG 21/01/2010 - New treatment for NO_INUSE elements (Tested OK)
    '''               AG 21/01/2010 - For NO_INUSE positions, complete VirtualRotorPositionsDS with data from table twksWSNotInUseRotorPositions 
    '''               AG 25/01/2010 - Process and save only NOT FREE positions (Tested OK)
    '''               SA 10/03/2010 - Changes to save in the Virtual Rotor the new field SampleID when it is informed for Patient Samples
    '''               SA 14/07/2010 - Added parameter to inform the name of the Virtual Rotor loaded previously
    '''               SA 25/10/2010 - When the Rotor Position contains an existing WS Element, save also value of field OnlyForISE in the Virtual Rotor
    '''               AG 03/02/2012 - Changes for saving new field Status in table tparVirtualRotorPositions
    '''               SA 09/02/2012 - Removed last changes for saving new field Status: it is not needed to search value in DB again; information
    '''                               is already obtained when calling function GetPositionContent for NOT IN USE positions
    ''' </remarks>
    Private Sub GenerateAndSaveVRotorPositionDS(ByVal myFormRotorContentByPositionDSObj As WSRotorContentByPositionDS, ByVal pRotorType As String, _
                                                ByVal pVirtualRotorID As Integer, ByVal pVirtualRotorName As String)
        Try
            If Not (myFormRotorContentByPositionDSObj Is Nothing) Then
                Dim myGlobalDataTO As New GlobalDataTO
                Dim myVirtualRotorPosititionsDS As New VirtualRotorPosititionsDS

                'Get only not FREE positions from the Rotor
                Dim lstRotorPositions As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                lstRotorPositions = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                                    Where a.RotorType = pRotorType _
                                  AndAlso a.Status <> "FREE" _
                                   Select a).ToList()

                Dim myWSRequiredElementsDS As WSRequiredElementsDS
                Dim myWSRequiredElementsDelegate As New WSRequiredElementsDelegate

                Dim myNoInUsePos As VirtualRotorPosititionsDS
                Dim noInUseDelegate As New WSNotInUseRotorPositionsDelegate

                Dim myVirtualRotorPosititionsRow As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow
                For Each myRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In lstRotorPositions
                    'Initialize VirtualRotorPosition row for each cell in the list of not free Rotor Positions                
                    myVirtualRotorPosititionsRow = myVirtualRotorPosititionsDS.tparVirtualRotorPosititions.NewtparVirtualRotorPosititionsRow
                    Me.InitializeVirtualRotorRow(pVirtualRotorID, myRow, myVirtualRotorPosititionsRow)

                    If (Not myRow.IsElementIDNull) Then
                        'IN USE Position: get data to save for the positioned required WS Element
                        myGlobalDataTO = myWSRequiredElementsDelegate.GetRequiredElementData(Nothing, myRow.ElementID)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myWSRequiredElementsDS = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsDS)

                            If (myWSRequiredElementsDS.twksWSRequiredElements.Rows.Count > 0) Then
                                '07/01/2010 AG - All myRow (RotorContentByPositionDS) and pVirtualRotorID information has been copied in InitializeVirtualRotorRow
                                If (myWSRequiredElementsDS.twksWSRequiredElements(0).IsTubeContentNull) Then
                                    myVirtualRotorPosititionsRow.SetTubeContentNull()
                                Else
                                    myVirtualRotorPosititionsRow.TubeContent = myWSRequiredElementsDS.twksWSRequiredElements(0).TubeContent
                                End If

                                If (myWSRequiredElementsDS.twksWSRequiredElements(0).IsReagentIDNull) Then
                                    myVirtualRotorPosititionsRow.SetReagentIDNull()
                                Else
                                    myVirtualRotorPosititionsRow.ReagentID = myWSRequiredElementsDS.twksWSRequiredElements(0).ReagentID
                                End If

                                If (myWSRequiredElementsDS.twksWSRequiredElements(0).IsSolutionCodeNull) Then
                                    myVirtualRotorPosititionsRow.SetSolutionCodeNull()
                                Else
                                    myVirtualRotorPosititionsRow.SolutionCode = myWSRequiredElementsDS.twksWSRequiredElements(0).SolutionCode
                                End If

                                If (myWSRequiredElementsDS.twksWSRequiredElements(0).IsCalibratorIDNull) Then
                                    myVirtualRotorPosititionsRow.SetCalibratorIDNull()
                                Else
                                    myVirtualRotorPosititionsRow.CalibratorID = myWSRequiredElementsDS.twksWSRequiredElements(0).CalibratorID
                                End If

                                If (myWSRequiredElementsDS.twksWSRequiredElements(0).IsMultiItemNumberNull) Then
                                    myVirtualRotorPosititionsRow.SetMultiItemNumberNull()
                                Else
                                    myVirtualRotorPosititionsRow.MultiItemNumber = myWSRequiredElementsDS.twksWSRequiredElements(0).MultiItemNumber
                                End If

                                If (myWSRequiredElementsDS.twksWSRequiredElements(0).IsControlIDNull) Then
                                    myVirtualRotorPosititionsRow.SetControlIDNull()
                                Else
                                    myVirtualRotorPosititionsRow.ControlID = myWSRequiredElementsDS.twksWSRequiredElements(0).ControlID
                                End If

                                If (myWSRequiredElementsDS.twksWSRequiredElements(0).IsSampleTypeNull) Then
                                    myVirtualRotorPosititionsRow.SetSampleTypeNull()
                                Else
                                    myVirtualRotorPosititionsRow.SampleType = myWSRequiredElementsDS.twksWSRequiredElements(0).SampleType
                                End If

                                'When field PatientID or SampleID is informed, it is stored in field PatientID in table of Virtual Rotor Positions
                                If (Not myWSRequiredElementsDS.twksWSRequiredElements(0).IsPatientIDNull) Then
                                    myVirtualRotorPosititionsRow.PatientID = myWSRequiredElementsDS.twksWSRequiredElements(0).PatientID
                                    myVirtualRotorPosititionsRow.SetOrderIDNull()
                                ElseIf (Not myWSRequiredElementsDS.twksWSRequiredElements(0).IsSampleIDNull) Then
                                    myVirtualRotorPosititionsRow.PatientID = myWSRequiredElementsDS.twksWSRequiredElements(0).SampleID
                                    myVirtualRotorPosititionsRow.SetOrderIDNull()
                                ElseIf (Not myWSRequiredElementsDS.twksWSRequiredElements(0).IsOrderIDNull) Then
                                    myVirtualRotorPosititionsRow.OrderID = myWSRequiredElementsDS.twksWSRequiredElements(0).OrderID
                                    myVirtualRotorPosititionsRow.SetPatientIDNull()
                                End If

                                myVirtualRotorPosititionsRow.OnlyForISE = myWSRequiredElementsDS.twksWSRequiredElements(0).OnlyForISE

                                If (myWSRequiredElementsDS.twksWSRequiredElements(0).IsPredilutionFactorNull) Then
                                    myVirtualRotorPosititionsRow.SetPredilutionFactorNull()
                                Else
                                    myVirtualRotorPosititionsRow.PredilutionFactor = myWSRequiredElementsDS.twksWSRequiredElements(0).PredilutionFactor
                                End If

                                'Status of cells marked as DEPLETED or with FEW volume has to be preserved
                                If (myRow.IsStatusNull OrElse (myRow.Status <> "DEPLETED" AndAlso String.Compare(myRow.Status, "FEW", False) <> 0)) Then
                                    myVirtualRotorPosititionsRow.SetStatusNull()
                                Else
                                    myVirtualRotorPosititionsRow.Status = myRow.Status
                                End If
                            End If
                        Else
                            'Error getting information of the required Element
                            Exit For
                        End If
                    Else
                        If (myRow.IsElementIDNull AndAlso Not myRow.IsTubeContentNull) Then
                            'NOT IN USE Position: get data from table twksWSNotInUseRotorPositions 
                            myGlobalDataTO = noInUseDelegate.GetPositionContent(Nothing, myRow.AnalyzerID, myRow.RotorType, myRow.RingNumber, myRow.CellNumber, myRow.WorkSessionID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myNoInUsePos = DirectCast(myGlobalDataTO.SetDatos, VirtualRotorPosititionsDS)

                                myVirtualRotorPosititionsRow.TubeContent = myRow.TubeContent

                                If (myNoInUsePos.tparVirtualRotorPosititions(0).IsReagentIDNull) Then
                                    myVirtualRotorPosititionsRow.SetReagentIDNull()
                                Else
                                    myVirtualRotorPosititionsRow.ReagentID = myNoInUsePos.tparVirtualRotorPosititions(0).ReagentID
                                End If

                                If (myNoInUsePos.tparVirtualRotorPosititions(0).IsSolutionCodeNull) Then
                                    myVirtualRotorPosititionsRow.SetSolutionCodeNull()
                                Else
                                    myVirtualRotorPosititionsRow.SolutionCode = myNoInUsePos.tparVirtualRotorPosititions(0).SolutionCode
                                End If

                                If (myNoInUsePos.tparVirtualRotorPosititions(0).IsCalibratorIDNull) Then
                                    myVirtualRotorPosititionsRow.SetCalibratorIDNull()
                                Else
                                    myVirtualRotorPosititionsRow.CalibratorID = myNoInUsePos.tparVirtualRotorPosititions(0).CalibratorID
                                End If

                                If (myNoInUsePos.tparVirtualRotorPosititions(0).IsMultiItemNumberNull) Then
                                    myVirtualRotorPosititionsRow.SetMultiItemNumberNull()
                                Else
                                    myVirtualRotorPosititionsRow.MultiItemNumber = myNoInUsePos.tparVirtualRotorPosititions(0).MultiItemNumber
                                End If

                                If (myNoInUsePos.tparVirtualRotorPosititions(0).IsControlIDNull) Then
                                    myVirtualRotorPosititionsRow.SetControlIDNull()
                                Else
                                    myVirtualRotorPosititionsRow.ControlID = myNoInUsePos.tparVirtualRotorPosititions(0).ControlID
                                End If

                                If (myNoInUsePos.tparVirtualRotorPosititions(0).IsSampleTypeNull) Then
                                    myVirtualRotorPosititionsRow.SetSampleTypeNull()
                                Else
                                    myVirtualRotorPosititionsRow.SampleType = myNoInUsePos.tparVirtualRotorPosititions(0).SampleType
                                End If

                                If (myNoInUsePos.tparVirtualRotorPosititions(0).IsPatientIDNull) Then
                                    myVirtualRotorPosititionsRow.SetPatientIDNull()
                                Else
                                    myVirtualRotorPosititionsRow.PatientID = myNoInUsePos.tparVirtualRotorPosititions(0).PatientID
                                End If

                                If (myNoInUsePos.tparVirtualRotorPosititions(0).IsOrderIDNull) Then
                                    myVirtualRotorPosititionsRow.SetOrderIDNull()
                                Else
                                    myVirtualRotorPosititionsRow.OrderID = myNoInUsePos.tparVirtualRotorPosititions(0).OrderID
                                End If

                                If (myNoInUsePos.tparVirtualRotorPosititions(0).IsPredilutionFactorNull) Then
                                    myVirtualRotorPosititionsRow.SetPredilutionFactorNull()
                                Else
                                    myVirtualRotorPosititionsRow.PredilutionFactor = myNoInUsePos.tparVirtualRotorPosititions(0).PredilutionFactor
                                End If

                                If (myNoInUsePos.tparVirtualRotorPosititions(0).IsStatusNull) Then
                                    myVirtualRotorPosititionsRow.SetStatusNull()
                                Else
                                    'Status of Not In Use Positions marked as DEPLETED or FEW is preserved
                                    myVirtualRotorPosititionsRow.Status = myNoInUsePos.tparVirtualRotorPosititions(0).Status
                                End If
                            Else
                                'Error getting information of the Not In Use Position
                                Exit For
                            End If
                        End If
                    End If

                    'Add the Virtual Rotor Posititions Data Row 
                    myVirtualRotorPosititionsDS.tparVirtualRotorPosititions.Rows.Add(myVirtualRotorPosititionsRow)
                Next

                'Save Virtual Rotor
                If (Not myGlobalDataTO.HasError) Then
                    If (myVirtualRotorPosititionsDS.tparVirtualRotorPosititions.Rows.Count > 0) Then
                        'Finally, save the Virtual Rotor...
                        Dim myVirtualRotorDelegate As New VirtualRotorsDelegate
                        myGlobalDataTO = myVirtualRotorDelegate.Save(Nothing, pRotorType, myVirtualRotorPosititionsDS, pVirtualRotorName)
                    End If
                Else
                    'If an error has happened, show the message
                    ShowMessage(Me.Name & ".GenerateAndSaveVRotorPositionDS", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GenerateAndSaveVRotorPositionDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GenerateAndSaveVRotorPositionDS", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initializes a virtual rotor position row with VirtualRotorID and all fields in rotor content by position
    ''' </summary>
    ''' <param name="pVirtualRotorID"></param>
    ''' <param name="pRotorContentByPositionRow"></param>    
    ''' <remarks>
    ''' Created by: AG 07/01/2010 AG (Tested OK)
    ''' </remarks>
    Private Sub InitializeVirtualRotorRow(ByVal pVirtualRotorID As Integer, ByVal pRotorContentByPositionRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow, ByRef myVirtualRotorPosRow As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow)
        Try

            With pRotorContentByPositionRow
                If pVirtualRotorID = 0 Then myVirtualRotorPosRow.SetVirtualRotorIDNull() Else myVirtualRotorPosRow.VirtualRotorID = pVirtualRotorID
                myVirtualRotorPosRow.RingNumber = .RingNumber
                myVirtualRotorPosRow.CellNumber = .CellNumber

                If .IsTubeContentNull Then myVirtualRotorPosRow.SetTubeContentNull() Else myVirtualRotorPosRow.TubeContent = .TubeContent
                If .IsTubeTypeNull Then myVirtualRotorPosRow.SetTubeTypeNull() Else myVirtualRotorPosRow.TubeType = .TubeType
                If .IsMultiTubeNumberNull Then myVirtualRotorPosRow.SetMultiTubeNumberNull() Else myVirtualRotorPosRow.MultiTubeNumber = .MultiTubeNumber
                If .IsRealVolumeNull Then myVirtualRotorPosRow.SetRealVolumeNull() Else myVirtualRotorPosRow.RealVolume = .RealVolume
                If .IsBarCodeInfoNull Then myVirtualRotorPosRow.SetBarcodeInfoNull() Else myVirtualRotorPosRow.BarcodeInfo = .BarCodeInfo
                If .IsBarcodeStatusNull Then myVirtualRotorPosRow.SetBarcodeStatusNull() Else myVirtualRotorPosRow.BarcodeStatus = .BarcodeStatus

                'RH 14/09/2011
                If .IsScannedPositionNull Then myVirtualRotorPosRow.SetScannedPositionNull() Else myVirtualRotorPosRow.ScannedPosition = .ScannedPosition

                myVirtualRotorPosRow.SetReagentIDNull()
                myVirtualRotorPosRow.SetSolutionCodeNull()
                myVirtualRotorPosRow.SetCalibratorIDNull()
                myVirtualRotorPosRow.SetControlIDNull()
                myVirtualRotorPosRow.SetMultiItemNumberNull()
                myVirtualRotorPosRow.SetSampleTypeNull()
                myVirtualRotorPosRow.SetOrderIDNull()
                myVirtualRotorPosRow.SetPatientIDNull()
                myVirtualRotorPosRow.SetPredilutionFactorNull()
            End With

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "InitializeVirtualRotorRow " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("InitializeVirtualRotorRow", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to unselect the control position and clear the SeletedElement.
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 08/01/2010
    ''' Tested: 
    ''' </remarks>
    Private Sub ClearSelection()
        If (Not mySelectedElementInfo Is Nothing AndAlso _
            mySelectedElementInfo.twksWSRotorContentByPosition.Rows.Count > 0) Then
            For Each rcpROW As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow _
                            In mySelectedElementInfo.twksWSRotorContentByPosition.Rows
                'TR 20/01/2010 - Change the selected status to update later the Rotor with the new value
                rcpROW.Selected = False

                'Clear the selected position on the Rotor
                MarkSelectedPosition(rcpROW.RingNumber, rcpROW.CellNumber, False)

                'TR 20/01/2010 - Update Rotor local data
                UpdateRotorContentByPositionDSForm(rcpROW)
            Next

            'Clear the selected Element
            mySelectedElementInfo = Nothing

            'Disable fields in the information panel
            CleanInfoArea(False)
        Else
            CleanInfoArea(False)
        End If
    End Sub

    ''' <summary>
    ''' Fill the tubeSize and BottleSize combo boxez filtering the tubes by the ring number.
    ''' this is because each ring allow only some sizes.
    ''' this method replace the old FillTubeSizesComboBoxes.
    ''' </summary>
    ''' <param name="pRingNumber"></param>
    ''' <remarks>
    ''' Created by: TR 13/01/2010 
    ''' TESTED : OK.
    ''' </remarks>
    Private Sub FilterAllTubesbyRingNumberRotorType(ByVal pRingNumber As Integer)
        'Dim result As New List(Of TubeSizeTO)
        Dim result As List(Of TubeSizeTO)
        Try
            'filter the alltubesizelist by the ringnumber, and the rotot type.
            result = (From a In AllTubeSizeList _
                     Where a.RingNumber = pRingNumber _
                     AndAlso String.Compare(a.RotorType, myRotorTypeForm, False) = 0 _
                     Select a).ToList()

            If (result.Count > 0) Then
                'validate wich rotor is active to set the values to the corresponding combobox.
                If String.Compare(myRotorTypeForm, "SAMPLES", False) = 0 Then
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
            ShowMessage(Me.Name & ".FilterAllTubesbyRingNumberRotorType", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    ''' <summary>
    ''' Verify if there are elements in the WorkSession pending to positioning to shown the warning screen of non positioned
    ''' Work Session Elements
    ''' </summary>
    ''' <param name="pOpenMode">Optional parameter to indicate if the screen of Not Positioned Elements will be opened due to the Warnings Button
    '''                         was clicked (when its value is an empty String) or due to Close Button was clicked (when its value is TO_EXIT).
    '''                         Close Button can be clicked manually for the final User or automatically during the process of Automatic WS Creation
    '''                         with LIS</param>
    ''' <param name="pCountNoPos">By Reference parameter used to return the number of not Positioned Elements. It was used only for the process
    '''                           of Automatic WS Creation with LIS, but IT IS NOT USED ANYMORE (check BT #1481 for additional information)</param>
    ''' <returns>Boolean value indicating if Rotor Positioning screen has to be closed after generating the Executions
    '''          (when True) or if it has to remain opened to allow the User to finish the Elements positioning (when False)</returns>
    ''' <remarks>
    ''' Created by:  SG 28/07/2010
    ''' Modified by: SA 08/11/2010 - Changed returned type from Integer to Boolean.  Added optional parameter pOpenMode to inform 
    '''                              the corresponding Property in the warning screen
    '''              AG 18/07/2013 - Added optional parameter pCountNoPos (byref), needed for the process of automatic WS creation with LIS
    '''              AG 22/07/2013 - When the screen of Not Positioned Elements is shown during the process of automatic WS creation with LIS, 
    '''                              the buzzer is activated. And when this screen is closed, the buzzer is deactivated.
    ''' </remarks>
    Private Function ShowNotPositionedElements(Optional ByVal pOpenMode As String = "", Optional ByRef pCountNoPos As Integer = 0) As Boolean
        Dim closeRotorPosScreen As Boolean = True
        Dim countNotPositioned As Integer = 0

        Try
            'AG 16/07/2013 - Not Positioned Patients must be excluded from the warning of Not Positioning Elements
            '                when this function is opened during the process of automatic WS creation with LIS
            Dim excludeNoPosPatientsFlag As Boolean = False
            If (AutoWSCreationWithLISModeAttribute AndAlso OpenByAutomaticProcessAttribute) Then
                excludeNoPosPatientsFlag = True
            End If

            'Verify if there are required Work Session Elements still not positioned
            Dim resultData As GlobalDataTO
            Dim myWSReqElementsDelegate As New WSRequiredElementsDelegate

            resultData = myWSReqElementsDelegate.CountNotPositionedElements(Nothing, WorkSessionIDAttribute, , True, excludeNoPosPatientsFlag)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                countNotPositioned = DirectCast(resultData.SetDatos, Integer)

                Dim thrownIWSNotPosWarning As Boolean = False
                If (pOpenMode = "TO_EXIT") Then
                    If (countNotPositioned > 0) Then thrownIWSNotPosWarning = True
                Else
                    thrownIWSNotPosWarning = True
                End If

                If (thrownIWSNotPosWarning) Then
                    'AG 22/07/2013 - Activate the Buzzer is the process of automatic WS creation with LIS is active
                    If (AutoWSCreationWithLISModeAttribute AndAlso OpenByAutomaticProcessAttribute) Then
                        If (AnalyzerController.IsAnalyzerInstantiated) Then AnalyzerController.Instance.Analyzer.StartAnalyzerRinging() '#REFACTORING
                    End If

                    Dim StartTime As DateTime = Now 'AG 18/02/2014 - #1505

                    'Shown the Positioning Warnings Screen
                    Using NotPositionedDialog As New IWSNotPosWarning()
                        NotPositionedDialog.OpenMode = pOpenMode
                        NotPositionedDialog.ActiveWorkSession = Me.WorkSessionIDAttribute
                        NotPositionedDialog.ShowDialog()

                        closeRotorPosScreen = (NotPositionedDialog.DialogResult <> DialogResult.OK)
                    End Using

                    'AG 18/02/2014 - #1505
                    CreateLogActivity("Time with NOSPOS warning screen opened: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                "IWSROTORPositions.ShowNotPositionedElements", EventLogEntryType.Information, False)

                    'AG 22/07/2013 - Stop the Buzzer is the process of automatic WS creation with LIS is active
                    If (AutoWSCreationWithLISModeAttribute AndAlso OpenByAutomaticProcessAttribute) Then
                        If (AnalyzerController.IsAnalyzerInstantiated) Then AnalyzerController.Instance.Analyzer.StopAnalyzerRinging() '#REFACTORING
                    End If
                End If
            Else
                'Error counting the total number of not positioned Elements
                ShowMessage(Me.Name & "ShowNotPositionedElements", resultData.ErrorCode, resultData.ErrorMessage, Me)
                closeRotorPosScreen = False
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ShowNotPositionedElements ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ShowNotPositionedElements", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

            closeRotorPosScreen = False
        End Try
        pCountNoPos = countNotPositioned
        Return closeRotorPosScreen
    End Function

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID"> The current Language of Application </param>
    ''' <remarks>
    ''' Created by:  PG 14/10/10
    ''' Modified by: EF 29/08/2013 - BT #1272 => Label for Barcode TextBox changed to "Barcode" (for both cases, Samples and Reagents Rotor). Label for
    '''                              SampleID TextBox (only for Samples Rotor) changed to "Sample" 
    '''              SA 19/11/2013 - BT #1359 => In the Label used to show the multilanguage description for Selected Position it is loaded now the 
    '''                                          multilanguage description for In Process Position (Second Reagents and Washing Solution needed for second 
    '''                                          Reagent contaminations in Reactions Rotor)
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsRequiredElementsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_RotorPos_ReqElem", pLanguageID)

            '***********************'
            '** SAMPLES ROTOR Tab **' 
            '***********************'
            SamplesTab.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Samples", pLanguageID)

            'Labels for Position Information Area 
            bsSamplesPositionInfoLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_RotorPos_InfoPos", pLanguageID)
            bsSamplesRingNumLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_RingNum", pLanguageID)
            bsSamplesCellLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorCell", pLanguageID)
            bsSamplesContentLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorCellContent", pLanguageID)
            bsSamplesNumberLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Number_Short", pLanguageID)
            'EF 29/08/2013 - Bugtracking 1272 - Change label text by 'Sample' and 'Barcode' in v2.1.1  
            'bsSampleIDLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_SampleID", pLanguageID)
            bsSampleIDLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_SampleVolume", pLanguageID)
            'bsSamplesBarcodeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_Barcode", pLanguageID)
            bsSamplesBarcodeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_BARCODE", pLanguageID)
            'EF 29/08/2013
            bsSampleTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", pLanguageID)
            bsDiluteStatusLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_Diluted", pLanguageID)
            bsTubeSizeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_TubeSize", pLanguageID)
            bsSamplesStatusLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MainMDI_Status", pLanguageID)

            'Labels for Legend Area
            bsSamplesLegendLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Legend", pLanguageID)
            bsLegendCalibratorsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_CLASSES_CALIB", pLanguageID)
            bsLegendControlsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_CLASSES_CTRL", pLanguageID)
            bsLegendStatLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Stat", pLanguageID)
            bsLegendRoutineLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Routine", pLanguageID)
            bsLegendDilutedLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_DilutedSample", pLanguageID)
            bsTubeAddSolLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_AddSol", pLanguageID)

            '************************'
            '** REAGENTS ROTOR Tab **'
            '************************'
            ReagentsTab.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Reagents", pLanguageID)

            'Labels for Position Information Area 
            bsReagentsPositionInfoLabel.Text = bsSamplesPositionInfoLabel.Text
            bsReagentsRingNumLabel.Text = bsSamplesRingNumLabel.Text
            bsReagentsCellLabel.Text = bsSamplesCellLabel.Text
            bsReagentsContentLabel.Text = bsSamplesContentLabel.Text
            bsReagentsNumberLabel.Text = bsSamplesNumberLabel.Text
            bsReagentNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_ReagentName", pLanguageID)
            bsTestNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestName", pLanguageID)
            'EF 29/08/2013 - Bugtracking 1272 - Change label text by 'Barcode' (smaller=visible) in v2.1.1
            'bsReagentsBarCodeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_ReagentBarcode", pLanguageID)
            bsReagentsBarCodeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_BARCODE", pLanguageID)
            'EF 29/08/2013
            bsExpirationDateLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ExpDate_Short", pLanguageID)
            bsBottleSizeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_BottleSize", pLanguageID)
            bsCurrentVolLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_CurrentVol", pLanguageID)
            bsTestsLeftLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_TestLeft", pLanguageID)
            bsReagStatusLabel.Text = bsSamplesStatusLabel.Text

            'Labels for Legend Area
            bsReagentsLegendLabel.Text = bsSamplesLegendLabel.Text
            bsLegReagentLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_TUBE_CONTENTS_REAGENT", pLanguageID)
            bsLegReagAdditionalSol.Text = bsTubeAddSolLabel.Text
            bsLegReagDepleteLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LGD_ROTEmptyLocked", pLanguageID)
            bsLegReagLowVolLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LGD_RotBLower", pLanguageID)
            bsLegReagNoInUseLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LGD_RotNoInUse", pLanguageID)
            bsBarcodeErrorRGLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LGD_RotBarErr", pLanguageID)
            bsUnknownLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_UNKNOWN", pLanguageID)
            'LegReagentSelLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LGD_RotBSelect", pLanguageID)
            LegReagentSelLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LGD_RotBInProcess", pLanguageID)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all button ToolTips
    ''' </summary>
    ''' <param name="pLanguageID"> The current Language of Application </param>
    ''' <remarks>
    ''' Created by:  PG 14/10/10
    ''' Modified by: DL 10/05/2013 - Depending on the active LIS mode (FILES or ES), load the proper multilanguage tooltip for the button that allow
    '''                              opening the auxiliary screen of Incomplete Samples (FILES) or the auxiliary screen for Host Query (ES)
    ''' </remarks>
    Private Sub GetScreenButtonToolTips(ByVal pLanguageID As String, LisWithFilesMode As Boolean)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'Buttons for Samples and Reagents autopositioning
            bsScreenToolTips.SetToolTip(bsSamplesAutoPosButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_RotorPos_SamplesAuto", pLanguageID))
            bsScreenToolTips.SetToolTip(bsReagentAutoPosButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_RotorPos_ReagentsAuto", pLanguageID))

            'Buttons for Rotors utilities: Load/Save Virtual Rotors, Reset Rotor, Check Volume
            bsScreenToolTips.SetToolTip(bsCheckRotorVolumeButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_CheckVolRotor", pLanguageID))
            bsScreenToolTips.SetToolTip(bsLoadVRotorButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_RotorPos_OpenLoadVRotor", pLanguageID))
            bsScreenToolTips.SetToolTip(bsSaveVRotorButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_RotorPos_OpenSaveVRotor", pLanguageID))
            bsScreenToolTips.SetToolTip(bsResetRotorButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_RotorPos_ResetRotor", pLanguageID))

            'Buttons for Barcode Scanning
            bsScreenToolTips.SetToolTip(bsScanningButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_RotorPos_ReadBarcode", pLanguageID))

            'Button for open Incomplete Samples screen (LisWithFilesMode = TRUE) or Host Query Screen (LisWithFilesMode = FALSE)
            If (Not LisWithFilesMode) Then
                bsScreenToolTips.SetToolTip(BarcodeWarningButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Incomplete_Samples_List", pLanguageID))
            Else
                bsScreenToolTips.SetToolTip(BarcodeWarningButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_BARCODE_WARN", pLanguageID))
            End If

            'Button for Positioning Warnings
            bsScreenToolTips.SetToolTip(bsWarningsButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_RotorPos_OpenWarnings", pLanguageID))

            'Buttons for Position Information Area in Samples Rotor
            bsScreenToolTips.SetToolTip(bsSamplesMoveFirstPositionButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToFirst", pLanguageID))
            bsScreenToolTips.SetToolTip(bsSamplesDecreaseButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToPrevious", pLanguageID))
            bsScreenToolTips.SetToolTip(bsSamplesIncreaseButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToNext", pLanguageID))
            bsScreenToolTips.SetToolTip(bsSamplesMoveLastPositionButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToLast", pLanguageID))

            bsScreenToolTips.SetToolTip(bsSamplesRefillPosButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Refill", pLanguageID))
            'bsScreenToolTips.SetToolTip(bsSamplesCheckVolumePosButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CheckVolume_Pos", pLanguageID))
            bsScreenToolTips.SetToolTip(bsSamplesDeletePosButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Delete", pLanguageID))

            'Buttons for Position Information Area in Reagents Rotor
            bsScreenToolTips.SetToolTip(bsReagentsMoveFirstPositionButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToFirst", pLanguageID))
            bsScreenToolTips.SetToolTip(bsReagentsDecreaseButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToPrevious", pLanguageID))
            bsScreenToolTips.SetToolTip(bsReagentsIncreaseButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToNext", pLanguageID))
            bsScreenToolTips.SetToolTip(bsReagentsMoveLastPositionButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToLast", pLanguageID))

            bsScreenToolTips.SetToolTip(bsReagentsRefillPosButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Refill", pLanguageID))
            bsScreenToolTips.SetToolTip(bsReagentsCheckVolumePosButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CheckVolume_Pos", pLanguageID))
            bsScreenToolTips.SetToolTip(bsReagentsDeletePosButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Delete", pLanguageID))

            ' XB 25/03/2014 - Memory leaks - Improve Dispose (SplitContainer disallows dispose memory)
            ''Buttons for Expand/Collapse the TreeView of required Elements
            'bsScreenToolTips.SetToolTip(BsForwardButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Expand", pLanguageID))
            'bsScreenToolTips.SetToolTip(BsBackwardButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Collapse", pLanguageID))

            'Other buttons: Print and Save&Close
            bsScreenToolTips.SetToolTip(bsPrintButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Print", pLanguageID))
            bsScreenToolTips.SetToolTip(bsAcceptButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save&Close", pLanguageID))
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenButtonToolTips ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenButtonToolTips ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' For the Element selected in the TreeView, get its details and show them in the information area. Besides, if the Element Status is positioned
    ''' (fully or incomplete), select all rotor positions containing tubes/bottles for the Element
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 22/11/2010
    ''' Modified by: RH 14/02/2011 - Code optimization. Every unneeded New get out. Introduce StringBuilder
    '''              SA 21/02/2012 - Rotor Positions containing bottles for incomplete Reagents and/or Additonal Solutions also has to be selected
    ''' </remarks>
    Private Sub ShowInfoSelectedTreeNodeElement()
        Try
            SetInfoAreaEnabledStatus(myRotorTypeForm, False)

            'RH 29/07/2011 Show info for every node, not only for leaves.
            'So the next line is out.
            'If bsElementsTreeView.SelectedNode.Nodes.Count = 0 Then

            'Get the RequireElementTO info on the Tag property.
            Dim myReqElementTO As WSRequiredElementsTO = DirectCast(bsElementsTreeView.SelectedNode.Tag, WSRequiredElementsTO)

            'Validate if element is positioned (fully or incomplete) to get the detailed information
            Dim myRotorPositionDelegate As New WSRotorContentByPositionDelegate
            If (String.Compare(myReqElementTO.ElementStatus, "POS", False) = 0 OrElse myReqElementTO.ElementStatus = "INCOMPLETE") Then
                Dim myGlobalDataTO As New GlobalDataTO

                'RH 14/02/2011
                Dim myElementList As New System.Text.StringBuilder()
                myElementList.Append(myReqElementTO.ElementID)

                'Validate if it's a calibrator
                If (myReqElementTO.TubeContent = "CALIB") Then
                    Dim myWSReqElementDelegate As New WSRequiredElementsDelegate
                    myGlobalDataTO = myWSReqElementDelegate.GetMultiPointCalibratorElements(Nothing, WorkSessionIDAttribute, myReqElementTO.ElementID, myReqElementTO.ElementCode)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        'Validate if it's a Multipoint Calibrator to add all other elements on my element list.
                        Dim myReqElementDS As WSRequiredElementsDS = DirectCast(myGlobalDataTO.SetDatos, WSRequiredElementsDS)

                        If (myReqElementDS.twksWSRequiredElements.Count > 0) Then
                            For Each reqElementRow As WSRequiredElementsDS.twksWSRequiredElementsRow In myReqElementDS.twksWSRequiredElements.Rows
                                'RH 14/02/2011 Use StringBuilder for string concatenations inside loops
                                'because it is the recommended way, because StringBuilder is optimized for that task
                                myElementList.AppendFormat(",{0}", reqElementRow.ElementID)
                            Next
                        End If
                    End If
                End If

                'Get detailed information of the selected Element on TreeView
                If (Not myGlobalDataTO.HasError) Then
                    myGlobalDataTO = myRotorPositionDelegate.GetPositionedElements(Nothing, AnalyzerIDAttribute, myRotorTypeForm, myElementList.ToString())

                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        Dim myWSRotorbPosDS As WSRotorContentByPositionDS = DirectCast(myGlobalDataTO.SetDatos, WSRotorContentByPositionDS)

                        If (myWSRotorbPosDS.twksWSRotorContentByPosition.Count > 0) Then
                            ClearSelection()

                            'RH 18/02/2011
                            Dim multiSel As Boolean = (myWSRotorbPosDS.twksWSRotorContentByPosition.Count > 1)
                            Dim myRotorPosRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow

                            If (Not multiSel) Then 'Only one selected element
                                myRotorPosRow = myWSRotorbPosDS.twksWSRotorContentByPosition.Rows(0)
                                ShowPositionInfoArea(AnalyzerIDAttribute, myRotorTypeForm, myRotorPosRow.RingNumber, myRotorPosRow.CellNumber)
                            Else
                                'RH 10/10/2011
                                If (String.Compare(myRotorTypeForm, "SAMPLES", False) = 0) Then
                                    bsSamplesDeletePosButton.Enabled = True
                                Else
                                    bsReagentsDeletePosButton.Enabled = True
                                End If
                            End If

                            For Each myRotorPosRow In myWSRotorbPosDS.twksWSRotorContentByPosition.Rows
                                mySelectedElementInfo = GetLocalPositionInfo(myRotorPosRow.RingNumber, myRotorPosRow.CellNumber, multiSel)
                            Next
                            'RH 18/02/2011 END
                        Else
                            'Clear selected elements. Includes CleanInfoArea(False)
                            ClearSelection()
                        End If
                    Else
                        'Clear selected elements. Includes CleanInfoArea(False)
                        ClearSelection()
                    End If
                Else
                    'RH 29/07/2011 Clear selected elements. Includes CleanInfoArea(False).
                    ClearSelection()
                End If
            Else
                'TR 09/03/2012 -Clear selection. 
                ClearSelection()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ShowInfoSelectedTreeNodeElement", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ShowInfoSelectedTreeNodeElement", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pControl"></param>
    ''' <param name="pImagePath"></param>
    ''' <param name="pTransparentImage"></param>
    ''' <remarks>
    ''' Created by:  AG 05/12/2010
    ''' Modified By: RH 16/02/2010 Visible property gets out. Do not touch Visible all around the code.
    ''' </remarks>
    Private Sub ChangeControlPositionImage(ByRef pControl As BSRImage, ByVal pImagePath As String, _
                                           Optional ByVal pTransparentImage As Boolean = False)
        Try
            'Load when new image or reset rotor is performed
            If (Not String.Equals(pControl.ImagePath, pImagePath)) Then 'RH 15/02/2010 resetLoadRotorPerformed gets out
                pControl.ImagePath = pImagePath
                pControl.IsTransparentImage = pTransparentImage

                If (pImagePath <> Nothing) Then ' AndAlso Not setControlPosToNothing Then
                    pControl.Image = Image.FromFile(pImagePath)
                    pControl.BringToFront()
                Else
                    pControl.Image = Nothing
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ChangeControlPositionImage ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ChangeControlPositionImage", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Creates the WS Executions and sorts the pending ones
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 09/02/2011
    ''' Modified by: AG 19/09/2011 - Verify is the current Analyzer Status is running to inform the correspondent parameter in function CreateWSExecutions
    '''              SA 15/03/2012 - Changed verification of ISE Module installed; added verification of ISE Module available and ready. In both cases
    '''                              (not installed or not ready), all pending ISE Executions are marked as locked
    '''              SA 26/07/2012 - Before calling the function to create Executions, verify if the ISE Module is ready to inform the optional parameter
    '''                              that will allow blocking all ISE Executions when the module is not ready
    '''              XB 28/10/2013 - BT #1343 ==> Required Calibrations do not display any warning message because them are performed automaticaly
    '''              SA 10/04/2014 - BT #1584 ==> When the Analyzer is in PAUSE mode, flag createWSInRunning has to be set to FALSE although the Analyzer 
    '''                                           Status is Running. This is to allow block/unblock Executions when elements have been unpositioned/positioned 
    '''                                           during the Pause
    '''              XB 23/05/2014 - BT #1639 ==> Do not lock ISE preparations during Runnning (not Pause) by Pending Calibrations
    '''              XB 27/05/2014 - BT #1638 ==> ISE_NEW_TEST_LOCKED msg is anulled
    '''              IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Sub CreateWSExecutions()
        Try
            Dim resultData As GlobalDataTO

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            'AG 30/05/2014 #1644 - Redesing correction #1584 for avoid DeadLocks
            'Verify is the current Analyzer Status is RUNNING ==> BT #1584: ...and it is not in PAUSE
            Dim createWSInRunning As Boolean = False
            'If (Not mdiAnalyzerCopy Is Nothing) Then createWSInRunning = (AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso _
            '                                                              Not AnalyzerController.Instance.Analyzer.AllowScanInRunning)
            Dim pauseMode As Boolean = False
            If (AnalyzerController.IsAnalyzerInstantiated) Then
                createWSInRunning = (AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING)
                pauseMode = AnalyzerController.Instance.Analyzer.AllowScanInRunning
            End If
            'AG 30/05/2014 #1644

            'SGM 07/09/2012 - Check if there is any pending ISE Calibration
            Dim iseModuleReady As Boolean = True
            Dim AffectedISEElectrodes As List(Of String) 'SGM 07/09/2012
            Dim showISELockedMessage As Boolean = False 'SGM 07/09/2012

            'Verify there is at least an ISE Test requested in the WorkSession
            Dim myOrderTestDelegate As New OrderTestsDelegate
            resultData = myOrderTestDelegate.IsThereAnyTestByType(Nothing, WorkSessionIDAttribute, "ISE")
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                If (CType(resultData.SetDatos, Boolean)) Then
                    'Verify if the Analyzer has an ISE Module installed and if it is available and ready
                    If (AnalyzerController.IsAnalyzerInstantiated) Then
                        iseModuleReady = (Not AnalyzerController.Instance.Analyzer.ISEAnalyzer Is Nothing AndAlso AnalyzerController.Instance.Analyzer.ISEAnalyzer.IsISEModuleReady)
                        If iseModuleReady Then
                            iseModuleReady = (AnalyzerController.Instance.Analyzer.ISEAnalyzer.ISEWSCancelErrorCounter < 3)
                            If iseModuleReady Then
                                resultData = AnalyzerController.Instance.Analyzer.ISEAnalyzer.CheckAnyCalibrationIsNeeded(AffectedISEElectrodes)
                                If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                                    Dim isNeeded As Boolean = CBool(resultData.SetDatos)
                                    If isNeeded Then

                                        ' XB 23/05/2014 - BT #1639
                                        If (AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.STANDBY OrElse _
                                           (AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso AnalyzerController.Instance.Analyzer.AllowScanInRunning)) Then
                                            iseModuleReady = False
                                        End If
                                        ' XB 23/05/2014 - BT #1639

                                        ' XB 28/10/2013
                                        ' showISELockedMessage = True
                                        If AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                                            ' Check if ISE Pumps calibration is required
                                            Dim PumpsCalibrationRequired As Boolean = False
                                            resultData = AnalyzerController.Instance.Analyzer.ISEAnalyzer.CheckPumpsCalibrationIsNeeded
                                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                                PumpsCalibrationRequired = CType(resultData.SetDatos, Boolean)
                                            End If
                                            If PumpsCalibrationRequired Then
                                                showISELockedMessage = True
                                            End If
                                        End If
                                        ' XB 28/10/2013
                                    End If
                                End If

                                ' XB 28/10/2013
                            Else
                                showISELockedMessage = True
                            End If
                        Else
                            showISELockedMessage = True
                            ' XB 28/10/2013
                        End If
                    End If
                End If
            End If
            'end SGM 07/09/2012

            Dim finalTime As String = "" 'AG 18/02/2014 #1505

            'Generate the WS Executions
            'Dim resultData As GlobalDataTO
            Dim myExecutionDelegate As New ExecutionsDelegate
            'AG 30/05/2014 #1644 - Redesing correction #1584 for avoid DeadLocks (add parameter pauseMode)
            resultData = myExecutionDelegate.CreateWSExecutions(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, createWSInRunning, -1, String.Empty, iseModuleReady, AffectedISEElectrodes, pauseMode) 'SGM 07/09/2012 - inform affected electrodes for locking them

            If (resultData.HasError) Then
                ErrorOnCreateWSExecutions = String.Format("{0}|{1}", resultData.ErrorCode, resultData.ErrorMessage)
            Else
                If Not (AutoWSCreationWithLISModeAttribute AndAlso OpenByAutomaticProcessAttribute) Then    ' XB 16/07/2013 - no display warning into the Auto WS LIS process
                    If showISELockedMessage Then
                        finalTime = Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0) 'AG 18/02/2014 - #1505
                        'DL 15/05/2013
                        'Me.ShowMessage(Me.Name, "ISE_NEW_TEST_LOCKED")

                        ' XB 22/11/2013 - Task #1394
                        'Me.UIThread(Function() ShowMessage(Me.Name, "ISE_NEW_TEST_LOCKED"))

                        ' XB 27/05/2014 - BT #1638
                        'Me.UIThread(Sub() IAx00MainMDI.SetDisplayISELockedPreparationsWarning(True))

                        'DL 15/05/2013
                        'ISE preparations locked'. 
                    End If
                End If
            End If

            ' XB 25/11/2013 - Inform to MDI that this screen is closing aims to open next screen - Task #1303
            'Me.UIThread(Sub() IAx00MainMDI.SetScreenChildShownInProcess(True))
            Me.UIThread(Sub() ExitingScreen())

            'AG 18/02/2014 - #1505 (change text)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            'myLogAcciones.CreateLogActivity("IWRotorPositions - Create WS Executions: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
            '                                "IWSampleRequest.CreateWSExecutions ", EventLogEntryType.Information, False)
            If finalTime = "" Then
                finalTime = Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0)
            End If
            myLogAcciones.CreateLogActivity("IWRotorPositions - Create WS Executions: " & finalTime, _
                                            "IWRotorPositions.CreateWSExecutions ", EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            'AG 18/02/2014 - #1505 (change text)

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            AffectedISEElectrodes = Nothing 'AG 19/02/2014 - #1514
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CreateWSExecutions", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ErrorOnCreateWSExecutions = String.Format("{0}|{1}", GlobalEnumerates.Messages.SYSTEM_ERROR, ex.Message + " ((" + ex.HResult.ToString + "))")
        Finally
            ScreenWorkingProcess = False
        End Try
    End Sub



    ''' <summary>
    ''' Validate if there are Barcode position with no request and Enable or Disable Button BarcodeWarningButton.
    ''' </summary>
    ''' <remarks>CREATED BY: TR 14/09/2011
    ''' Modified AG 03/04/2013 - When Setting LIS_WITHFILES_MODE = False the barcode warning button always is enabled</remarks>
    Private Sub ValidateBCNoRequest()
        Try

            Dim myGlobalDataTO As New GlobalDataTO
            Dim myBarcodePositionsWithNoRequestsDelegate As New BarcodePositionsWithNoRequestsDelegate
            Dim myBarcodePositionsWithNoRequestsDS As New BarcodePositionsWithNoRequestsDS

            myGlobalDataTO = myBarcodePositionsWithNoRequestsDelegate.ReadByAnalyzerAndWorkSession(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute)

            If Not myGlobalDataTO.HasError Then
                myBarcodePositionsWithNoRequestsDS = DirectCast(myGlobalDataTO.SetDatos, BarcodePositionsWithNoRequestsDS)

                If myBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests.Rows.Count > 0 Then
                    BarcodeWarningButton.Enabled = True
                Else
                    'AG 03/04/2013
                    'BarcodeWarningButton.Enabled = False
                    Dim lisWithFilesMode As Boolean = False
                    Dim resultData As New GlobalDataTO
                    Dim userSettings As New UserSettingsDelegate
                    resultData = userSettings.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_WITHFILES_MODE.ToString)
                    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                        lisWithFilesMode = CType(resultData.SetDatos, Boolean)
                    End If

                    If lisWithFilesMode Then
                        BarcodeWarningButton.Enabled = False
                    Else
                        BarcodeWarningButton.Enabled = True
                    End If
                    'AG 03/04/2013
                End If
            End If

            ' XB 01/08/2013 - add functionality according to disable LIS buttons
            If IAx00MainMDI.DisableLISButtons() Then
                BarcodeWarningButton.Enabled = False
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateBCNoRequest", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateBCNoRequest", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Verify if Scanning Barcode Button has to be enabled (depending on the current Analyzer status and also on current value of 
    ''' Analyzer Setting SAMPLE_BARCODE_DISABLED)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 28/03/2012 - Moved form LoadScreenStatus
    ''' Modified by: AG 06/02/2012 - Added AnalyzerController.Instance.Analyzer.Connected to the button activation rule 
    '''              AG 03/04/2012 - Check if Barcode button should be enabled or disabled by calling function ActivateButtonWithAlarms in the MainMDI
    '''              SA 15/10/2013 - BT #1334 ==> Changes in button activation rule due to new Analyzer mode PAUSE in RUNNING: the Scanning button can 
    '''                              be available not only when the Analyzer is in STAND BY, but also when it is in RUNNING but it is currently in PAUSE mode
    '''              IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Sub ValidateScanningButtonEnabled()
        Try
            If (IsDisposed) Then Exit Sub 'IT 03/06/2014 - #1644 No refresh if screen is disposed

            Dim statusScanningButton As Boolean = False

            If (AnalyzerController.IsAnalyzerInstantiated) Then
                'Check if the Barcode Reader for the Rotor Type has been deactivated in Barcode Configuration Screen 
                Dim barcodeReaderDisabled As Boolean = False
                If (myRotorTypeForm = "SAMPLES") Then
                    barcodeReaderDisabled = sampleBarcodeReaderOFF
                Else
                    barcodeReaderDisabled = reagentBarcodeReaderOFF
                End If

                'Check if WarmUp maneuvers have finished
                Dim sensorValue As Single = AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.WARMUP_MANEUVERS_FINISHED)

                'If the Analyzer is Connected and Ready, the WarmUp maneuvers have finished, the Barcode Reader is available and not disabled....
                If (AnalyzerController.Instance.Analyzer.Connected AndAlso AnalyzerController.Instance.Analyzer.AnalyzerIsReady AndAlso sensorValue = 1 AndAlso _
                    AnalyzerController.Instance.Analyzer.BarCodeProcessBeforeRunning = AnalyzerEntity.BarcodeWorksessionActions.BARCODE_AVAILABLE AndAlso _
                    (Not barcodeReaderDisabled)) Then
                    'If the Analyzer is in STAND BY or if it is in RUNNING but has been PAUSED...
                    If (AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.STANDBY OrElse _
                       (AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso AnalyzerController.Instance.Analyzer.AllowScanInRunning)) Then
                        If (Not IAx00MainMDI Is Nothing) Then  'This condition is to be sure a new instance of the MDI is not created 
                            'Verify if the Scanning Button can be available by checking Alarms and another Analyzer states
                            statusScanningButton = IAx00MainMDI.ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.READ_BARCODE)
                        End If
                    End If
                End If
            End If

            bsScanningButton.Enabled = statusScanningButton

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ValidateScanningButtonEnabled ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ValidateScanningButtonEnabled ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Verify if Scanning Barcode Button has to be enabled (depending on the current Analyzer status and also on current value of 
    ''' Analyzer Setting SAMPLE_BARCODE_DISABLED)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 28/03/2012 - moved form LoadScreenStatus
    ''' </remarks>
    Private Sub ValidateCheckRotorVolumeButtonEnabled()
        Try
            If (IsDisposed) Then Exit Sub 'IT 03/06/2014 - #1644 No refresh if screen is disposed

            'If Not mdiAnalyzerCopy Is Nothing Then
            '    'Place the correct condition when implement this functionality
            '    'TODO
            '    If AnalyzerController.Instance.Analyzer.Connected AndAlso AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.STANDBY AndAlso AnalyzerController.Instance.Analyzer.AnalyzerIsReady Then
            '        'No cover enabled and opened (the nothing condition is to avoid create a new MDI instance)
            '        If Not IAx00MainMDI Is Nothing Then
            '            bsCheckRotorVolumeButton.Enabled = IAx00MainMDI.ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.CHECK_BOTTLE_VOLUME)  'AG 28/03/2012 bsCheckRotorVolumeButton.Enabled = True
            '            bsReagentsCheckVolumePosButton.Enabled = IAx00MainMDI.ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.CHECK_BOTTLE_VOLUME)  'AG 28/03/2012 bsReagentsCheckVolumePosButton.Enabled = True
            '        Else
            '            bsCheckRotorVolumeButton.Enabled = False
            '            bsReagentsCheckVolumePosButton.Enabled = False
            '        End If

            '        bsSamplesCheckVolumePosButton.Enabled = False
            '    Else
            '        bsCheckRotorVolumeButton.Enabled = False
            '        bsReagentsCheckVolumePosButton.Enabled = False
            '        bsSamplesCheckVolumePosButton.Enabled = False
            '    End If
            'End If

            'Temporally always disabled
            bsCheckRotorVolumeButton.Enabled = False
            bsReagentsCheckVolumePosButton.Enabled = False
            'bsSamplesCheckVolumePosButton.Enabled = False

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ValidateCheckRotorVolumeButtonEnabled ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ValidateCheckRotorVolumeButtonEnabled ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Show message "Action will be performed only on Elements no needed by the worksession"  when Analyzer is in PAUSE mode
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 12/11/2013
    ''' </remarks>
    Private Sub ShowActionMessageifApply()
        Try
            If (AnalyzerController.Instance.Analyzer.AllowScanInRunning) Then '#REFACTORING
                'Exclude all the elements that do not allow this action on pause
                Dim lstPositions As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                lstPositions = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In mySelectedElementInfo.twksWSRotorContentByPosition _
                               Where a.Status = "FREE" OrElse Not a.AllowedActionInPause _
                              Select a).ToList

                'Validate if the result count is the same without filetering
                If (lstPositions.Count > 0) AndAlso (lstPositions.Count <> mySelectedElementInfo.twksWSRotorContentByPosition.Count) Then
                    'If values are different it means there are elements that do not allow the action; then the warning message is shown
                    ShowMessage(Me.Name, GlobalEnumerates.Messages.ALLOWED_ACTION_PAUSE.ToString())
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ShowActionMessageifApply ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ShowActionMessageifApply ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' 'On-line' Refresh rotor position screen with information received from Analyzer. 
    ''' </summary>
    ''' <param name="pRefreshDS"></param>
    ''' <remarks>
    ''' AG 12/04/2011 - creation - Tested
    ''' TR - adapt for barcode reception (using ROTORPOSITION_CHANGED)
    ''' AG 22/09/2011 - Use the same method but 2 event types ROTORPOSITION_CHANGED or BARCODE_POSITION_READ due they have different updating screen dataset business
    ''' AG 07/06/2012 - change code to improve execution time
    ''' </remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        Try
            If (IsDisposed) Then Exit Sub 'IT 03/06/2014 - #1644 No refresh if screen is disposed

            Dim myGlobalDataTO As GlobalDataTO = Nothing
            If isClosingFlag Then Return 'AG 03/08/2012

            RefreshDoneField = False 'RH 28/03/2012
            Dim myLocalEventType As GlobalEnumerates.UI_RefreshEvents = GlobalEnumerates.UI_RefreshEvents.NONE

            If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED) Then
                'This event has to update only CellStatus, elementStatus and optionally TestLeft, RealVolume
                myLocalEventType = GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED
            ElseIf pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ) Then
                myLocalEventType = GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ
            End If

            If myLocalEventType <> GlobalEnumerates.UI_RefreshEvents.NONE Then

                Dim selectedPosition As Boolean = False
                Dim existSelection As Boolean = False 'True there is only one rotor position selected!!!
                Dim myLinqRes As New List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)

                If myRotorContentByPositionDSForm Is Nothing Then Exit Sub ' XB 28/02/2014 - #1523 No refresh if screen is closing

                myLinqRes = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                             Where a.Selected = True AndAlso a.RotorType = myRotorTypeForm _
                             Select a).ToList()

                If myLinqRes.Count = 1 Then
                    existSelection = True
                End If

                'Update internal RotorContentByPositionDS structure (myRotorContentByPositionDSForm and use a temporal structure for paint screen)
                'AG 07/06/2012
                'Dim tmpPositionDS As New WSRotorContentByPositionDS
                Dim tmpReagentsPositionDS As New WSRotorContentByPositionDS
                Dim tmpSamplesPositionDS As New WSRotorContentByPositionDS

                For Each updatedRow As UIRefreshDS.RotorPositionsChangedRow In pRefreshDS.RotorPositionsChanged.Rows
                    If Not updatedRow.IsAnalyzerIDNull And Not updatedRow.IsWorkSessionIDNull _
                        And Not updatedRow.IsRotorTypeNull And Not updatedRow.IsCellNumberNull Then

                        If myRotorContentByPositionDSForm Is Nothing Then Exit Sub ' XB 28/02/2014 - #1523 No refresh if screen is closing

                        'Search the row in myRotorContentByPositionDSForm and update DATA
                        myLinqRes = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In _
                                     myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                                     Where a.AnalyzerID = updatedRow.AnalyzerID _
                                     And a.WorkSessionID = updatedRow.WorkSessionID _
                                     And a.RotorType = updatedRow.RotorType _
                                     And a.CellNumber = updatedRow.CellNumber _
                                     Select a).ToList()

                        If myLinqRes.Count > 0 Then
                            ''updatedRow.Status = "NOT_INUSE" AndAlso
                            'TR 07/09/2011 -Validate if position has a positioned element to change status on treeView 
                            'AG 13/06/2014 #1661 - add condition Not myLinqRes(0).IsElementIDNull
                            If myLinqRes(0).ElementStatus = "POS" AndAlso Not myLinqRes(0).IsElementIDNull Then
                                'Change the tree status 

                                If bsElementsTreeView Is Nothing Then Exit Sub ' XB 28/02/2014 - #1523 No refresh if screen is closing

                                ChangeTreeRotorElementsStatus(bsElementsTreeView.Nodes, False, myLinqRes(0), myRotorTypeForm)
                            End If
                            'TR 07/09/2011 -END.

                            myLinqRes(0).BeginEdit()

                            If Not updatedRow.IsStatusNull Then
                                myLinqRes(0).Status = updatedRow.Status
                                'Else - AG 22/09/2011 - set to null this field only in barcode mode
                            ElseIf myLocalEventType = GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ Then
                                myLinqRes(0).SetStatusNull()
                            End If

                            If Not updatedRow.IsElementStatusNull Then
                                myLinqRes(0).ElementStatus = updatedRow.ElementStatus
                                'Else - AG 22/09/2011 - set to null this field only in barcode mode
                            ElseIf myLocalEventType = GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ Then
                                myLinqRes(0).SetElementStatusNull()
                            End If

                            If Not updatedRow.IsRealVolumeNull Then
                                myLinqRes(0).RealVolume = updatedRow.RealVolume
                                'Else - AG 22/09/2011 - set to null this field only in barcode mode
                            ElseIf myLocalEventType = GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ Then

                                myLinqRes(0).SetRealVolumeNull()
                            End If

                            If Not updatedRow.IsRemainingTestsNumberNull Then
                                myLinqRes(0).RemainingTestsNumber = updatedRow.RemainingTestsNumber
                                'Else - AG 22/09/2011 - set to null this field only in barcode mode
                            ElseIf myLocalEventType = GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ Then
                                myLinqRes(0).SetRemainingTestsNumberNull()
                            End If

                            If Not updatedRow.IsBarCodeInfoNull Then
                                myLinqRes(0).BarCodeInfo = updatedRow.BarCodeInfo
                                'Else - AG 22/09/2011 - set to null this field only in barcode mode
                            ElseIf myLocalEventType = GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ Then
                                myLinqRes(0).SetBarCodeInfoNull()
                            End If

                            If Not updatedRow.IsBarcodeStatusNull Then
                                myLinqRes(0).BarcodeStatus = updatedRow.BarcodeStatus
                                'Else - AG 22/09/2011 - set to null this field only in barcode mode
                            ElseIf myLocalEventType = GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ Then
                                myLinqRes(0).SetBarcodeStatusNull()
                            End If

                            'TR 01/09/2011 -Update new inserted columns.
                            If Not updatedRow.IsScannedPositionNull Then
                                myLinqRes(0).ScannedPosition = updatedRow.ScannedPosition
                                'Else - AG 22/09/2011 - set to null this field only in barcode mode
                            ElseIf myLocalEventType = GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ Then
                                myLinqRes(0).SetScannedPositionNull()
                            End If

                            If Not updatedRow.IsElementIDNull Then
                                myLinqRes(0).ElementID = updatedRow.ElementID
                                'Else - AG 22/09/2011 - set to null this field only in barcode mode
                            ElseIf myLocalEventType = GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ Then
                                myLinqRes(0).SetElementIDNull()
                            End If

                            If Not updatedRow.IsMultiTubeNumberNull Then
                                myLinqRes(0).MultiTubeNumber = updatedRow.MultiTubeNumber
                                'Else - AG 22/09/2011 - set to null this field only in barcode mode
                            ElseIf myLocalEventType = GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ Then
                                myLinqRes(0).SetMultiTubeNumberNull()
                            End If

                            If Not updatedRow.IsTubeTypeNull Then
                                myLinqRes(0).TubeType = updatedRow.TubeType
                                'Else - AG 22/09/2011 - set to null this field only in barcode mode
                            ElseIf myLocalEventType = GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ Then
                                myLinqRes(0).SetTubeTypeNull()
                            End If

                            If Not updatedRow.IsTubeContentNull Then
                                myLinqRes(0).TubeContent = updatedRow.TubeContent
                                'Else - AG 22/09/2011 - set to null this field only in barcode mode
                            ElseIf myLocalEventType = GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ Then
                                myLinqRes(0).SetTubeContentNull()
                            End If
                            'TR 01/09/2011 -END.

                            myLinqRes(0).EndEdit()

                            'If needed update the flag that inform us refresh position information is also needed
                            If existSelection Then
                                If myLinqRes(0).Selected Then selectedPosition = True Else selectedPosition = False
                            End If

                            'Refresh screen using UpdateRotorTreeViewArea method (use a temporal DS only with the updated position)
                            'tmpPositionDS.Clear()
                            'AG 07/06/2012
                            'Dim tmpRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow
                            'tmpRow = tmpPositionDS.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow
                            'tmpRow = myLinqRes(0)
                            'tmpPositionDS.twksWSRotorContentByPosition.ImportRow(tmpRow)
                            'tmpPositionDS.AcceptChanges()
                            'myRotorContentByPositionDSForm.twksWSRotorContentByPosition.AcceptChanges()

                            If myRotorContentByPositionDSForm Is Nothing Then Exit Sub ' XB 28/02/2014 - #1523 No refresh if screen is closing

                            Dim tmpRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow
                            If updatedRow.RotorType = "REAGENTS" Then
                                tmpRow = tmpReagentsPositionDS.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow
                                tmpRow = myLinqRes(0)
                                tmpReagentsPositionDS.twksWSRotorContentByPosition.ImportRow(tmpRow)
                                tmpReagentsPositionDS.AcceptChanges()
                                myRotorContentByPositionDSForm.twksWSRotorContentByPosition.AcceptChanges()
                            ElseIf updatedRow.RotorType = "SAMPLES" Then
                                tmpRow = tmpSamplesPositionDS.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow
                                tmpRow = myLinqRes(0)
                                tmpSamplesPositionDS.twksWSRotorContentByPosition.ImportRow(tmpRow)
                                tmpSamplesPositionDS.AcceptChanges()
                                myRotorContentByPositionDSForm.twksWSRotorContentByPosition.AcceptChanges()
                            End If
                            'AG 07/06/2012
                            If selectedPosition Then
                                'Reload the info area with the new information 

                                If mySelectedElementInfo Is Nothing Then Exit Sub ' XB 28/02/2014 - #1523 No refresh if screen is closing

                                mySelectedElementInfo = GetLocalPositionInfo(myLinqRes(0).RingNumber, myLinqRes(0).CellNumber, False)
                                ShowPositionInfoArea(AnalyzerIDAttribute, myRotorTypeForm, myLinqRes(0).RingNumber, myLinqRes(0).CellNumber)
                            End If

                        End If
                    End If
                Next

                'TR 21/11/2013 - BT #1380(2)
                Dim myWSRotorPosInProcessDS As New RotorPositionsInProcessDS
                Dim myWSRotorPosInProcessDelegate As New WSRotorPositionsInProcessDelegate
                'Get the all the elements
                myGlobalDataTO = myWSRotorPosInProcessDelegate.ReadAllReagents(Nothing, AnalyzerIDAttribute)
                If Not myGlobalDataTO.HasError Then
                    myWSRotorPosInProcessDS = DirectCast(myGlobalDataTO.SetDatos, RotorPositionsInProcessDS)

                    Dim myRCPList As New List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)

                    'Validate if the element in proces change status and is finished.
                    For Each PosInProcess As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In _
                                            myRotorContentByPositionDSForm.twksWSRotorContentByPosition.Where(Function(a) _
                                                                                                a.InProcessElement).ToList()
                        If (From a In myWSRotorPosInProcessDS.twksWSRotorPositionsInProcess _
                            Where a.AnalyzerID = PosInProcess.AnalyzerID _
                            AndAlso a.CellNumber = PosInProcess.CellNumber).Count = 0 Then
                            PosInProcess.InProcessElement = False
                            'Impor to the temporal structure use to update the positions.
                            tmpReagentsPositionDS.twksWSRotorContentByPosition.ImportRow(PosInProcess)
                        End If
                    Next

                    'Go throught each position in process and change value Posinprocess = true
                    For Each PosInProcess As RotorPositionsInProcessDS.twksWSRotorPositionsInProcessRow In _
                                                        myWSRotorPosInProcessDS.twksWSRotorPositionsInProcess.Rows
                        'Validate if position is in Process
                        myRCPList = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                                     Where a.AnalyzerID = PosInProcess.AnalyzerID _
                                     AndAlso a.CellNumber = PosInProcess.CellNumber _
                                     Select a).ToList()

                        If myRCPList.Count > 0 Then
                            'Position is in proces update value.
                            myRCPList.First.InProcessElement = True
                            'Impor to the temporal structure use to update the positions.
                            tmpReagentsPositionDS.twksWSRotorContentByPosition.ImportRow(myRCPList.First())
                        End If
                        'Clear the objec
                        myRCPList.Clear()
                    Next

                    'Set values to nothing 
                    myRCPList = Nothing
                    myWSRotorPosInProcessDS = Nothing

                Else
                    ShowMessage(Me.Name & ".RefreshScreen", GlobalEnumerates.Messages.SYSTEM_ERROR, myGlobalDataTO.ErrorMessage)
                End If
                'TR 21/11/2013 -BT #1380(2) END.

                'AG 07/06/2012
                'UpdateRotorTreeViewArea(tmpPositionDS, myLinqRes(0).RotorType)
                If tmpReagentsPositionDS.twksWSRotorContentByPosition.Rows.Count > 0 Then
                    UpdateRotorTreeViewArea(tmpReagentsPositionDS, "REAGENTS")
                End If

                If tmpSamplesPositionDS.twksWSRotorContentByPosition.Rows.Count > 0 Then
                    UpdateRotorTreeViewArea(tmpSamplesPositionDS, "SAMPLES")
                End If
                'AG 07/06/2012

                'AG-TR 10/05/2012 -validate the processingbeforerunnin.
                If myLocalEventType = GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ AndAlso IAx00MainMDI.processingBeforeRunning <> "0" Then
                    'isScanningProcess = False
                    ScreenWorkingProcess = False
                    Me.Enabled = True 'Enable the screen
                    'IAx00MainMDI.ShowStatus(GlobalEnumerates.Messages._NONE)
                End If

                RefreshDoneField = True 'RH 28/03/2012
            End If

            'AG 15/03/2012 - when FREEZE appears while UI is disabled because screen is working Sw must reactivate UI
            If AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.FREEZE) = 1 Then '#REFACTORING
                ScreenWorkingProcess = False 'Process finished
                IAx00MainMDI.EnableButtonAndMenus(True)
                IAx00MainMDI.SetActionButtonsEnableProperty(True)
                Me.Enabled = True 'Enable the screen
                Cursor = Cursors.Default
                RefreshDoneField = True 'RH 28/03/2012
            End If
            'AG 15/03/2012

            'AG 28/03/2012 -scan barcode, check rotor volume, ... buttons must be disabled if cover open while the cover detection is enabled
            If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED) Then
                ValidateScanningButtonEnabled()
                ValidateCheckRotorVolumeButtonEnabled()
                'RefreshDoneField = True 'AG 28/03/2012 The button activion or not depending the covers is is a special case
                '                                       in this case do not activate the field
            End If
            'AG 28/03/2012


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.StackTrace + " - " + ex.HResult.ToString + "))", Me.Name & ".RefreshScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RefreshScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' Refresh the screen. (rotor area and Treeview area)
    ''' </summary>
    ''' <remarks>CREATED BY: TR 14/09/2011</remarks>
    Public Sub RefreshAfterSamplesWithoutRequest(ByVal pWorkSessionStatus As String)
        Try
            If (IsDisposed) Then Exit Sub 'IT 03/06/2014 - #1644 No refresh if screen is disposed

            ScreenWorkingProcess = False
            WorkSessionStatusAttribute = pWorkSessionStatus
            LoadScreenStatus(WorkSessionStatusAttribute)
            InitializeScreen(False, "")
            RefreshDoneField = True 'RH 28/03/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".RefreshAfterSamplesWithoutRequest ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RefreshAfterSamplesWithoutRequest", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' Update the screen user events allowed depending the status
    ''' This method is called when the analyzer status changes while the RotorPosition screen is opened
    ''' </summary>
    ''' <param name="pNewAnalzyerStatus"></param>
    ''' <param name="pWorkSessionStatus"></param>
    ''' <remarks></remarks>
    Public Sub RefreshScreenStatus(ByVal pNewAnalzyerStatus As String, ByVal pWorkSessionStatus As String)
        Try
            If (IsDisposed) Then Exit Sub 'IT 03/06/2014 - #1644 No refresh if screen is disposed

            WorkSessionStatus(pNewAnalzyerStatus) = pWorkSessionStatus
            LoadScreenStatus(WorkSessionStatusAttribute)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".RefreshScreenStatus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RefreshScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' Creates the WS Executions and sorts the pending ones.    
    ''' Business: Moved from bsCreateExecutionsButton_Click to an independent method and adapted
    ''' </summary>
    ''' <param name="pFromOwnScreen">When is called from RotorPosition screen this param is TRUE (not re-activate control because the screen 
    '''                              will be closed, else FALSE (re-activate control when the thread finishes)</param>
    ''' <param name="pCreateExecutions">By reference parameter to return if the WS Executions have been created or not</param>
    ''' <remarks>
    ''' Created by:  AG 10/05/2012 - Creation (copied and adapted from bsCreateExecutionsButton_Click)
    ''' Modified by: SA 11/06/2013 - Before starting the process, get from table twksWSAnalyzers the status of the active WorkSession to update the 
    '''                              WorkSessionStatusAttribute in case the WS Status has been changed after scanning the Samples Barcode or after 
    '''                              decoding a Samples Barcode manually entered (in these cases it is possible the status changes from OPEN to PENDING)
    '''              AG 09/07/2013 - Added changes to open the screen of Not Positioned Elements also during the process of automatic WS creation 
    '''                              with LIS when there are Elements (different of Patient Samples Tubes) pending to be positioned in the Rotors
    '''              XB 19/12/2013 - Added log traces 
    '''              SA 28/01/2014 - BT #1481 ==> Removed the condition to stop the process of automatic WS creation with LIS when pCreateExecutions is 
    '''                                           TRUE but some of the needed Elements are not positioned in the Analyzer Rotors
    ''' </remarks>
    Public Function CreateExecutionsProcess(ByVal pFromOwnScreen As Boolean, ByRef pCreateExecutions As Boolean) As GlobalDataTO
        Dim returnValue As New GlobalDataTO

        Try
            'XB 19/12/2013 - Add log traces
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity("Initiate Create Executions Process Function", Me.Name & ".CreateExecutionsProcess", _
                                            EventLogEntryType.Information, False)

            'SA 11/06/2013 - Verify if the status of the active WS has changed
            Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate
            returnValue = myWSAnalyzersDelegate.ReadWSAnalyzers(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute)

            If (Not returnValue.HasError AndAlso Not returnValue.SetDatos Is Nothing) Then
                Dim activeWorkSessionDS As WSAnalyzersDS = DirectCast(returnValue.SetDatos, WSAnalyzersDS)
                If (activeWorkSessionDS.twksWSAnalyzers.Rows.Count > 0) Then WorkSessionStatusAttribute = activeWorkSessionDS.twksWSAnalyzers.First.WSStatus.Trim
            End If

            'XB 19/12/2013 - Add log traces
            myLogAcciones.CreateLogActivity("WorkSessionStatusAttribute value : [" & WorkSessionStatusAttribute & "]", Me.Name & ".CreateExecutionsProcess", _
                                            EventLogEntryType.Information, False)

            If (WorkSessionStatusAttribute <> "EMPTY" AndAlso WorkSessionStatusAttribute <> "OPEN" AndAlso _
                WorkSessionStatusAttribute <> "ABORTED") Then
                Cursor = Cursors.WaitCursor

                'XB 19/12/2013 - Add log traces
                myLogAcciones.CreateLogActivity("AutoWSCreationWithLISModeAttribute value : [" & AutoWSCreationWithLISModeAttribute & "]", Me.Name & ".CreateExecutionsProcess", _
                                                EventLogEntryType.Information, False)
                myLogAcciones.CreateLogActivity("OpenByAutomaticProcessAttribute value : [" & OpenByAutomaticProcessAttribute & "]", Me.Name & ".CreateExecutionsProcess", _
                                                EventLogEntryType.Information, False)

                'BT #1481 - Process of Automatic WS Creation with LIS should not stop when some of the required Elements are not positioned 
                '           and the User click in Continue Button to ignore them
                If (AutoWSCreationWithLISModeAttribute AndAlso OpenByAutomaticProcessAttribute) Then
                    If (pFromOwnScreen) Then
                        'Not Positioned Elements are evaluated only when this method is called from Rotor Positions Screen
                        pCreateExecutions = ShowNotPositionedElements("TO_EXIT")
                    Else
                        'When the process for Automatic WS Creation was launched from a different screen, the auxiliary screen
                        'of Not Positioned Elements is not shown
                        pCreateExecutions = True
                    End If

                    If (Not pCreateExecutions) Then
                        IAx00MainMDI.SetAutomateProcessStatusValue(GlobalEnumerates.LISautomateProcessSteps.notStarted)
                        IAx00MainMDI.InitializeAutoWSFlags()
                        OpenByAutomaticProcessAttribute = False
                        IAx00MainMDI.EnableButtonAndMenus(True, True)
                    End If
                Else
                    'The Create Executions process was launched manually by clicking in EXIT Button in Rotor Positions Screen
                    pCreateExecutions = ShowNotPositionedElements("TO_EXIT")
                End If

                ''AG 09/07/2013 - Keep the warning of Not Positioned Elements. Cancel the code to skip this warning in automatic WS process
                'If (AutoWSCreationWithLISModeAttribute AndAlso OpenByAutomaticProcessAttribute) Then
                '    'AG 10/07/2013 - If user decides to stay in Rotor Positions Screen or if there are some Not Positioned Elements 
                '    '                and user decides ignore them, the automatic WS process finishes
                '    Dim noPosCount As Integer = 0
                '    If (pFromOwnScreen) Then
                '        'Not Positioned Elements are evaluated only when this method is called from Rotor Positions Screen
                '        pCreateExecutions = ShowNotPositionedElements("TO_EXIT", noPosCount)
                '    Else
                '        'When the process for automatic WS creation was launched from a different screen, the auxiliary screen
                '        'of Not Positioned Elements is not shown
                '        pCreateExecutions = True
                '    End If

                '    If (Not pCreateExecutions OrElse noPosCount > 0) Then
                '        IAx00MainMDI.SetAutomateProcessStatusValue(GlobalEnumerates.LISautomateProcessSteps.notStarted)
                '        IAx00MainMDI.InitializeAutoWSFlags()   'XB 30/07/2013
                '        OpenByAutomaticProcessAttribute = False 'The automatic mode is finished when not positioned elements screen is shown
                '        IAx00MainMDI.EnableButtonAndMenus(True, True)
                '    End If
                'Else
                '    'The Create Executions process was launched manually by clicking in EXIT Button in Rotor Positions Screen
                '    pCreateExecutions = ShowNotPositionedElements("TO_EXIT")
                'End If
                'AG 09/07/2013

                If (pCreateExecutions) Then

                    'TR 16/04/2014 BT #1597
                    'Comment or delete this line on version 3.0.1
                    'isClosingFlag = True 'AG 18/03/2014 - #1545 Investigate avoid DeadLocks

                    'Uncomment on version 3.0.1
                    'Valiadate if pFromOwnScreen  is enable to set value to IsclosingFlag = true else false
                    If pFromOwnScreen Then
                        isClosingFlag = True
                    End If
                    'TR 16/04/2014 BT #1597 -END

                    IAx00MainMDI.EnableButtonAndMenus(False)
                    bsElementsTreeView.BackColor = SystemColors.MenuBar
                    Application.DoEvents()

                    For Each c As Control In FunctionalityArea.Controls
                        If TypeOf (c) Is BSButton Then
                            c.Enabled = False
                        End If
                    Next
                    FunctionalityArea.Enabled = False

                    'TR 06/09/2012 - Validate controls are not nothing before disabling them
                    If (Not bsElementsTreeView Is Nothing) Then bsElementsTreeView.Enabled = False
                    If (Not RotorsTabs Is Nothing) Then RotorsTabs.Enabled = False

                    'XB 19/12/2013 - Add log traces
                    myLogAcciones.CreateLogActivity("Launch CreateWSExecutions !", Me.Name & ".CreateExecutionsProcess", _
                                                    EventLogEntryType.Information, False)


                    Dim workingThread As New Threading.Thread(AddressOf CreateWSExecutions)
                    ScreenWorkingProcess = True
                    workingThread.Start()

                    While ScreenWorkingProcess
                        IAx00MainMDI.InitializeMarqueeProgreesBar()
                        Application.DoEvents()
                    End While

                    workingThread = Nothing
                    IAx00MainMDI.StopMarqueeProgressBar()

                    'AG 11/12/2013 - BT #1433 ==> Comment these code lines because they fail when START/CONTINUE WS is clicked with Rotor
                    '                             Positions Screen open and no Tubes are found or LIS does not respond anything (v211 patch1)
                    'Leave the previous code active

                    'If AutoWSCreationWithLISModeAttribute AndAlso OpenByAutomaticProcessAttribute Then
                    '    'AG 19/07/2013 v2.1.1 - when called from the automatic WS creation with LIS process do nothing, else keep previous code that enable controls
                    '    'Do nothing!!!
                    'Else
                    '    'Previous code
                    '    'AG 10/05/2012- when called from own screen do nothing (it will be closed), 
                    '    'else (mdi) reactivate controls
                    '    If Not pFromOwnScreen Then
                    '        For Each c As Control In FunctionalityArea.Controls
                    '            If TypeOf (c) Is BSButton Then
                    '                c.Enabled = True
                    '            End If
                    '        Next
                    '        FunctionalityArea.Enabled = True
                    '        bsElementsTreeView.Enabled = True
                    '        RotorsTabs.Enabled = True
                    '    End If
                    '    'AG 10/05/2012
                    'End If

                    If (Not pFromOwnScreen) Then
                        For Each c As Control In FunctionalityArea.Controls
                            If TypeOf (c) Is BSButton Then
                                c.Enabled = True
                            End If
                        Next
                        FunctionalityArea.Enabled = True
                        bsElementsTreeView.Enabled = True
                        RotorsTabs.Enabled = True
                    End If
                    'AG 11/12/2013 - BT #1433

                    Cursor = Cursors.Default
                    If (ErrorOnCreateWSExecutions = String.Empty) Then
                        'Activate vertical buttons depending the executions available
                        'IAx00MainMDI.SetActionButtonsEnableProperty(True) 'AG 07/11/2012 - Avoid blinking in START WS button when rotor position screen is closed
                    Else
                        returnValue.HasError = True 'AG 10/05/2012
                        Application.DoEvents()
                        Dim ErrorData As String() = ErrorOnCreateWSExecutions.Split("|")
                        ErrorOnCreateWSExecutions = String.Empty 'Reset the value after using it
                        ShowMessage(Me.Name, ErrorData(0), ErrorData(1), Me)
                    End If
                Else
                    Cursor = Cursors.Default

                    'TR 16/04/2014 BT #1597-Reload screen status.
                    LoadScreenStatus(WorkSessionStatusAttribute)
                End If

                'AG 10/02/2014 - If worksession is EMPTY, ABORTED or OPEN and this screen is closed menu and buttons (vertical and screen buttons) disable!
            Else
                IAx00MainMDI.EnableButtonAndMenus(False)
                If pFromOwnScreen Then
                    For Each c As Control In FunctionalityArea.Controls
                        If TypeOf (c) Is BSButton Then
                            c.Enabled = False
                        End If
                    Next
                    FunctionalityArea.Enabled = False
                    If (Not bsElementsTreeView Is Nothing) Then bsElementsTreeView.Enabled = False
                    If (Not RotorsTabs Is Nothing) Then RotorsTabs.Enabled = False
                End If
                'AG 10/02/2014

            End If

        Catch ex As Exception
            returnValue.HasError = True
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CreateExecutionsProcess", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            Me.Cursor = Cursors.Default
            Application.DoEvents()
            ShowMessage(Me.Name & "CreateExecutionsProcess()", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            If (pCreateExecutions) Then IAx00MainMDI.StopMarqueeProgressBar()
        End Try
        Return returnValue
    End Function

    Public Sub RefreshAfterCloseIncompleteSamplesScreen()

        If (IsDisposed) Then Exit Sub 'IT 03/06/2014 - #1644 No refresh if screen is disposed

        ClearSelection() 'DL 17/04/2013
        LoadScreenStatus(WorkSessionStatusAttribute)
        InitializeScreen(False, "")

    End Sub

    ''' <summary>
    ''' Function to know if all Elements placed in all selected positions allow new actions enabled in PAUSE mode.
    ''' The AllowedActionInPause (mySelectedElementInfo.twksWSRotorContentByPosition) Field always comes TRUE and the value is changed to FALSE if necessary.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' CREATED BY:JV 08/11/2013 BT - #1358
    ''' MODIFIED BY: TR 18/11/2013 -BT #1358(2) the results still the same, but the functionality change completly.
    '''              TR 27/11/2013 -BT #1404 Validate the running time else set the value 'Set the default 
    '''                             value define on ScreenBlockStatus.
    ''' </remarks>
    Public Function VerifyActionsAllowedInReagentsRotor() As Boolean
        Dim actionAllowed As Boolean = True
        Try
            If isClosingFlag Then Return actionAllowed 'AG 10/02/2014 - #1496 No refresh is screen is closing
            If Not mySelectedElementInfo Is Nothing Then
                If AnalyzerController.Instance.Analyzer.AllowScanInRunning Then '#REFACTORING
                    'Go through  the recived results related to each position.
                    For Each mySelectedElement As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In _
                                                                mySelectedElementInfo.twksWSRotorContentByPosition.Rows
                        mySelectedElement.AllowedActionInPause = True
                        If mySelectedElement.RotorType = "REAGENTS" Then
                            If mySelectedElement.InProcessElement = True Then
                                mySelectedElement.AllowedActionInPause = False
                            End If
                        End If
                    Next
                    'If there are not elements with AllowedActionInPause property = true then return false.
                    If (From a In mySelectedElementInfo.twksWSRotorContentByPosition Where a.AllowedActionInPause Select a).Count = 0 Then
                        actionAllowed = False
                    End If

                ElseIf AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then '#REFACTORING
                    'Go through  the recived results related to each position.
                    For Each mySelectedElement As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In _
                                                                mySelectedElementInfo.twksWSRotorContentByPosition.Rows
                        If mySelectedElement.RotorType = "REAGENTS" Then
                            If mySelectedElement.InProcessElement = True OrElse _
                                mySelectedElement.Status = "INUSE" OrElse _
                                mySelectedElement.Status = "FEW" OrElse _
                                mySelectedElement.Status = "DEPLETED" Then

                                actionAllowed = False
                                Exit For
                            End If
                        End If
                    Next

                Else
                    'Set the default value define on ScreenBlockStatus.
                    actionAllowed = isManualBarcodeAllowed
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".VerifyActionsAllowedInReagentsRotor()", EventLogEntryType.Error, _
                                                                            GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".VerifyActionsAllowedInReagentsRotor", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return actionAllowed
    End Function

#End Region

#Region "Events"

    Public Sub New()
        ' XB 27/11/2013 - Inform to MDI that this screen is building - Task #1303
        LoadingScreen()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        'dl 01/03/2012
        SetStyle(ControlStyles.DoubleBuffer, True)
        SetStyle(ControlStyles.UserPaint, True)
        SetStyle(ControlStyles.AllPaintingInWmPaint, True)

        'DL 18/07/2013 solve the problem when show border when screen is working
        PanelControl1.LookAndFeel.UseDefaultLookAndFeel = False
        PanelControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat
        PanelControl1.Appearance.BorderColor = Color.WhiteSmoke
        PanelControl1.Appearance.Options.UseBorderColor = True
        PanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple

        PanelControl6.LookAndFeel.UseDefaultLookAndFeel = False
        PanelControl6.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat
        PanelControl6.Appearance.BorderColor = Color.WhiteSmoke
        PanelControl6.Appearance.Options.UseBorderColor = True
        PanelControl6.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        'DL 18/07/2013

    End Sub


    ''' <summary>
    ''' Launch the Rotors scanning process
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 14/07/2011
    ''' Modified by: SA 25/07/2013 - When the scanning finishes, if the User clicked in HQ Button is Host Query Screen, 
    '''                              the LIS Orders Download process is executed 
    '''              SA 15/10/2013 - BT #1334 ==> Changes due to new Analyzer mode PAUSE in RUNNING: the Scanning process will be called not only
    '''                              when the Analyzer is in STAND BY, but also when it is in RUNNING but stopped (PAUSE)
    '''              XB 06/02/2014 - Improve WDOG BARCODE_SCAN - Task #1438
    '''              IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Sub bsScanningButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsScanningButton.Click
        Try
            CreateLogActivity("Btn Scanning", Me.Name & ".bsScanningButton_Click", EventLogEntryType.Information, False) 'JV #1360 24/10/2013
            IAx00MainMDI.SetAutomateProcessStatusValue(GlobalEnumerates.LISautomateProcessSteps.notStarted) 'AG 10/07/2013

            'AG 26/09/2011 - Use progress bar thread as in WSPrep screen
            If (AnalyzerController.IsAnalyzerInstantiated) Then
                'Call the Barcode read process only if the Analyzer is connected and the Barcode is available
                If (AnalyzerController.Instance.Analyzer.Connected AndAlso AnalyzerController.Instance.Analyzer.BarCodeProcessBeforeRunning = AnalyzerEntity.BarcodeWorksessionActions.BARCODE_AVAILABLE) Then
                    'Call the Barcode read process only if the Analyzer Status is STANDBY or if it is PAUSE in RUNNING
                    If (AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.STANDBY) OrElse _
                       (AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso AnalyzerController.Instance.Analyzer.AllowScanInRunning) Then
                        Cursor = Cursors.WaitCursor

                        'DL 14/07/2011 Disable interface application
                        IAx00MainMDI.DisabledMdiForms = Me
                        IAx00MainMDI.EnableButtonAndMenus(False)
                        IAx00MainMDI.SetActionButtonsEnableProperty(False) 'AG 22/03/2012
                        IAx00MainMDI.ShowStatus(GlobalEnumerates.Messages.BARCODE_READING) 'AG 22/03/2012
                        IAx00MainMDI.SinglecanningRequested = True 'AG 06/11/2013 - Task #1375 - Inform scanning requested from rotorposition screen started

                        Me.Enabled = False 'Disable the screen
                        'isScanningProcess = True

                        'DL 16/04/2013 Read setting WDOG_TIME_BARCODE_SCAN and assign to watchdog timer (interval property). BEGIN
                        Dim resultData As GlobalDataTO = Nothing
                        Dim myParams As New SwParametersDelegate
                        Dim myParametersDS As New ParametersDS
                        ' Read application name for LIS parameter

                        ' XB 06/02/2014 - CYCLE_MACHINE param is used instead of WDOG_TIME_BARCODE_SCAN due for accelerating the operation - Task #1438
                        'resultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.WDOG_TIME_BARCODE_SCAN.ToString, Nothing)
                        resultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.CYCLE_MACHINE.ToString, AnalyzerModelAttribute)
                        ' XB 06/02/2014

                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            myParametersDS = CType(resultData.SetDatos, ParametersDS)
                            If myParametersDS.tfmwSwParameters.Count > 0 Then
                                'sendFolder = myParametersDS.tfmwSwParameters.Item(0).ValueText
                                watchDogTimer.Interval = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric * 1000

                                ' XB 29/01/2014 - Task #1438
                                Debug.Print("******************************* WATCHDOG INTERVAL FROM DB [" & watchDogTimer.Interval.ToString & "]")
                                AnalyzerController.Instance.Analyzer.BarcodeStartInstrExpected = True
                                ' XB 29/01/2014

                                watchDogTimer.Enabled = True
                            End If
                        End If
                        'DL 16/04/2013 Read setting WDOG_TIME_BARCODE_SCAN and assign to watchdog timer (interval property). END

                        ScreenWorkingProcess = True

                        IAx00MainMDI.InitializeMarqueeProgreesBar()
                        'Dim prevMessage As String = IAx00MainMDI.bsAnalyzerStatus.Text
                        Application.DoEvents()

                        Dim workingThread As New Threading.Thread(AddressOf ScanningBarCode)
                        workingThread.Start()

                        While ScreenWorkingProcess 'isScanningProcess
                            IAx00MainMDI.InitializeMarqueeProgreesBar()
                            Application.DoEvents()
                        End While
                        workingThread = Nothing
                        watchDogTimer.Enabled = False
                        IAx00MainMDI.StopMarqueeProgressBar()

                        'SA 25/07/2013 - If the User clicked in HQ Button is Host Query Screen, the LIS Orders Download process is executed
                        If (HQButtonUserClickAttribute) Then
                            IAx00MainMDI.SetAutomateProcessStatusValue(GlobalEnumerates.LISautomateProcessSteps.subProcessDownloadOrders)
                            IAx00MainMDI.pausingAutomateProcess = True 'AG 29/01/2014 - HostQuery manual can not enter in running automatically (pause automate process just before go to Running)
                            IAx00MainMDI.CreateAutomaticWSWithLIS()
                        End If

                        IAx00MainMDI.ShowStatus(GlobalEnumerates.Messages._NONE) 'AG 22/03/2012
                        IAx00MainMDI.SetActionButtonsEnableProperty(True)        'AG 22/03/2012
                    End If
                End If
            End If
            'AG 26/09/2011

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsScanningButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsScanningButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            IAx00MainMDI.SinglecanningRequested = False 'AG 06/11/2013 - Task #1375 - Inform scanning requested from rotorposition screen finished

            'TR 03/04/2012
            Cursor = Cursors.Default
            IAx00MainMDI.StopMarqueeProgressBar()
            'TR 03/04/2012

            'TR 04/10/2011
            IAx00MainMDI.EnableButtonAndMenus(True)
        End Try
        Cursor = Cursors.Default
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Modified by: IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Sub ScanningBarCode()
        Try
            'Get the available Rotor Types for the Analyzer according its model
            'Dim AnalyzerConfig As New AnalyzerModelRotorsConfigDelegate
            Dim resultdata As New GlobalDataTO
            Dim BarCodeDS As New AnalyzerManagerDS
            Dim rowBarCode As AnalyzerManagerDS.barCodeRequestsRow

            '14/02/2012 AG - Final spec: In version 1 only read the whole rotor (reagents or samples) but not single positions

            'AG 12/09/2011 - Version 1 sw only sends read full rotor. When read single positions available
            'in Fw uncomment these code and comment the current active code under mark AG 12/09/2011

            'resultdata = AnalyzerConfig.GetAnalyzerRotorTypes(Nothing, AnalyzerIDAttribute)
            'If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
            '    Dim myAnalyzerModelRotorsConfigDS As New AnalyzerModelRotorsConfigDS
            '    myAnalyzerModelRotorsConfigDS = DirectCast(resultdata.SetDatos, AnalyzerModelRotorsConfigDS)

            '    If (Not mySelectedElementInfo Is Nothing AndAlso mySelectedElementInfo.twksWSRotorContentByPosition.Rows.Count > 0) Then
            '        'Linq for search all selected elements inside ring that allow barcode reader.
            '        Dim qSelect As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
            '        qSelect = (From a In mySelectedElementInfo.twksWSRotorContentByPosition _
            '                   Join b In myAnalyzerModelRotorsConfigDS.tfmwAnalyzerModelRotorsConfig _
            '                   On a.AnalyzerID Equals b.AnalyzerID And a.RotorType Equals b.RotorType And a.RingNumber Equals b.RingNumber _
            '                   Where b.RotorType = myRotorTypeForm AndAlso a.RotorType = myRotorTypeForm AndAlso _
            '                         b.BarcodeReaderFlag = True _
            '                   Select a).tolist

            '        For i As Integer = 0 To qSelect.Count - 1
            '            rowBarCode = BarCodeDS.barCodeRequests.NewbarCodeRequestsRow

            '            With rowBarCode
            '                .RotorType = myRotorTypeForm
            '                .Action = GlobalEnumerates.Ax00CodeBarAction.SINGLE_POS
            '                .Position = qSelect(i).CellNumber
            '                .SetCommangConfigNull()
            '            End With

            '            BarCodeDS.barCodeRequests.AddbarCodeRequestsRow(rowBarCode)
            '        Next i

            '    Else
            '        'All positions
            '        rowBarCode = BarCodeDS.barCodeRequests.NewbarCodeRequestsRow
            '        With rowBarCode
            '            .RotorType = myRotorTypeForm
            '            .Action = GlobalEnumerates.Ax00CodeBarAction.FULL_ROTOR
            '            .Position = 0
            '            .SetCommangConfigNull()
            '        End With
            '        BarCodeDS.barCodeRequests.AddbarCodeRequestsRow(rowBarCode)

            '    End If

            'End If

            'Version 1: All positions
            rowBarCode = BarCodeDS.barCodeRequests.NewbarCodeRequestsRow
            With rowBarCode
                .RotorType = myRotorTypeForm
                .Action = GlobalEnumerates.Ax00CodeBarAction.FULL_ROTOR
                .Position = 0
                ' XBC 10/02/2012
                '.SetCommangConfigNull()
                ' XBC 10/02/2012
            End With
            BarCodeDS.barCodeRequests.AddbarCodeRequestsRow(rowBarCode)
            'AG 12/09/2011

            BarCodeDS.AcceptChanges()

            'Send the bar code instruction
            If (AnalyzerController.IsAnalyzerInstantiated) Then

                ScreenWorkingProcess = True
                'AG 22/03/2012 - Do not call IAx00MainMDI from a thread ... a New MDI is created and causes system error
                'IAx00MainMDI.SetActionButtonsEnableProperty(False) 'AG 12/07/2011 - Disable all vertical action buttons bar
                'IAx00MainMDI.ShowStatus(GlobalEnumerates.Messages.BARCODE_READING)

                AnalyzerController.Instance.Analyzer.BarCodeProcessBeforeRunning = AnalyzerEntity.BarcodeWorksessionActions.NO_RUNNING_REQUEST    'Initialize barcode read with NO running involved!!
                resultdata = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.BARCODE_REQUEST, True, Nothing, BarCodeDS, "")

                If resultdata.HasError OrElse Not AnalyzerController.Instance.Analyzer.Connected Then
                    ScreenWorkingProcess = False
                    'AG 22/03/2012 - Do not call IAx00MainMDI from a thread ... a New MDI is created and causes system error
                    'IAx00MainMDI.ShowStatus(GlobalEnumerates.Messages._NONE)
                    'IAx00MainMDI.SetActionButtonsEnableProperty(True)
                End If

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ScanningBarCode", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'DL 15/05/2013
            'ShowMessage(Me.Name & ".ScanningBarCode", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Me.UIThread(Function() ShowMessage(Name & ".ScanningBarCode", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))"))
            'DL 15/05/2013
            ScreenWorkingProcess = False
        End Try
    End Sub

    ' XB 25/03/2014 - Memory leaks - Improve Dispose (SplitContainer disallows dispose memory)
    'Private Sub BsForwardButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsForwardButton.Click
    '    'SplitContainer1.Panel2Collapsed = True

    '    'RH 28/02/2011  Not needed. Introduces blinking.
    '    'SplitContainer1.Panel1MinSize = 978
    '    'SplitContainer1.Panel2MinSize = 0

    '    BsForwardButton.Visible = False
    '    BsBackwardButton.Visible = True

    'End Sub

    'Private Sub BsBackwardButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsBackwardButton.Click
    '    'SplitContainer1.Panel2Collapsed = False

    '    'RH 28/02/2011  Not needed Introduces blinking.
    '    'SplitContainer1.Panel1MinSize = 208
    '    'SplitContainer1.Panel2MinSize = 770

    '    BsForwardButton.Visible = True
    '    BsBackwardButton.Visible = False

    'End Sub
    ' XB 25/03/2014

    Private Sub watchDogTimer_Timer(ByVal source As Object, ByVal e As ElapsedEventArgs)


        'Disable timer
        watchDogTimer.Enabled = False

        'Finish loop
        ScreenWorkingProcess = False

        ' XB 15/01/2014 - Refresh screen when Fw or Sw not works fine and the timer is finally invoked - Task #1438
        Debug.Print("******************************* WATCHDOG TIMER ELAPSED !!!!")
        Me.UIThread(Sub() RefreshScreenAfterWatchDogInvoke())
        ' XB 15/01/2014

    End Sub

    ''' <summary>
    ''' Update Interval Timer value
    ''' </summary>
    ''' <param name="pNewIntervalValue"></param>
    ''' <remarks>Created by XB 15/01/2014 - Task #1438</remarks>
    Public Sub RefreshwatchDogTimer_Interval(ByVal pNewIntervalValue As Double)
        Try
            If (IsDisposed) Then Exit Sub 'IT 03/06/2014 - #1644 No refresh if screen is disposed
            MyClass.watchDogTimer.Interval = pNewIntervalValue
            'Debug.Print("******************************* WATCHDOG INTERVAL CHANGED TO [" & pNewIntervalValue.ToString & "]")

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".RefreshwatchDogTimer_Interval ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RefreshwatchDogTimer_Interval", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Update Enable Timer value
    ''' </summary>
    ''' <param name="pEnableValue"></param>
    ''' <remarks>Created by XB 29/01/2014 - Task #1438</remarks>
    Public Sub RefreshwatchDogTimer_Enable(ByVal pEnableValue As Boolean)
        Try
            If (IsDisposed) Then Exit Sub 'IT 03/06/2014 - #1644 No refresh if screen is disposed
            If pEnableValue Then AnalyzerController.Instance.Analyzer.BarcodeStartInstrExpected = True '#REFACTORING

            MyClass.watchDogTimer.Enabled = pEnableValue
            Debug.Print("******************************* WATCHDOG ENABLE CHANGED TO [" & pEnableValue.ToString & "]")

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".RefreshwatchDogTimer_Interval ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RefreshwatchDogTimer_Interval", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Refresh controls screen
    ''' </summary>
    ''' <remarks>Created by XB 15/01/2014 - Task #1438</remarks>
    Public Sub RefreshScreenAfterWatchDogInvoke()
        Try
            If (IsDisposed) Then Exit Sub 'IT 03/06/2014 - #1644 No refresh if screen is disposed

            AnalyzerController.Instance.Analyzer.BarCodeProcessBeforeRunning = AnalyzerEntity.BarcodeWorksessionActions.BARCODE_AVAILABLE '#REFACTORING
            LoadScreenStatus(WorkSessionStatusAttribute)
            Me.Enabled = True 'Enable the screen

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".RefreshScreenAfterWatchDogInvoke ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RefreshScreenAfterWatchDogInvoke", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="e"></param>
    ''' <remarks>AG 02/01/2014 - BT #1433 (v211 patch2)</remarks>
    Private Sub autoWSCreationTimer_Timer(ByVal source As Object, ByVal e As ElapsedEventArgs)

        'Disable timer
        autoWSCreationTimer.Enabled = False

        'AG 02/01/2014 - BT #1433 - Change, activate a timer and call the acceptbutton when the timer ellapses
        If AutoWSCreationWithLISModeAttribute AndAlso OpenByAutomaticProcessAttribute Then
            CreateLogActivity("AutoCreate WS with LIS: Before press Accept button automatically", "IWSRotorPositions.IWSRotorPositions_Shown", EventLogEntryType.Information, False)
            Me.UIThread(Sub() bsAcceptButton.Enabled = True)
            Me.UIThread(Sub() bsAcceptButton.PerformClick())
        End If

    End Sub


    ''' <summary>
    ''' Screen loading event 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR
    ''' Modified by: SA 21/12/2009 - Current code was replaced for a call to new method InitializeScren 
    '''              SA 14/07/2010 - Removed hardcoded values of screen properties. Added code to get the Analyzer Model
    '''                              when it has not been informed; validate that the Analyzer Model is A400
    '''              PG 14/10/2010 - Get the current Language
    '''              SA 10/02/2014 - BT #1496 ==> Call new function PreparePositionsControlsNEW instead of function PreparePositionsControls(False)
    '''                                           to avoid recursive searching through all screen Controls Collections
    '''              AG 24/02/2014 - BT #1520 ==> Use parameters MAX_APP_MEMORYUSAGE and MAX_SQL_MEMORYUSAGE into performance counters and shown a warning 
    '''                                           message if at least one of them has been exceeded
    '''              IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Sub ReagentSamplePositioning_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            'DL 16/04/2013. BEGIN
            watchDogTimer.Enabled = False
            AddHandler watchDogTimer.Elapsed, AddressOf watchDogTimer_Timer
            'DL 16/04/2013. END

            'AG 02/01/2014 - BT #1433 (v211 patch2)
            autoWSCreationTimer.Enabled = False
            AddHandler autoWSCreationTimer.Elapsed, AddressOf autoWSCreationTimer_Timer
            'AG 02/01/2014 - BT #1433

            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage

            Dim validationOK As Boolean = (AnalyzerIDAttribute <> "" AndAlso WorkSessionIDAttribute <> "" AndAlso WorkSessionStatusAttribute <> "")
            If (validationOK) Then
                'If the Analyzer Model is not informed, get it from the DB 
                If (AnalyzerModelAttribute = String.Empty) Then
                    Dim resultData As GlobalDataTO
                    Dim confAnalyzers As New AnalyzersDelegate

                    resultData = confAnalyzers.GetAnalyzerModel(Nothing, AnalyzerIDAttribute)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        Dim myAnalyzersDS As AnalyzersDS = DirectCast(resultData.SetDatos, AnalyzersDS)

                        If (myAnalyzersDS.tcfgAnalyzers.Rows.Count = 1) Then
                            AnalyzerModel = myAnalyzersDS.tcfgAnalyzers(0).AnalyzerModel
                            validationOK = (AnalyzerModelAttribute = "A400")   'This screen is only for Analyzers with Model A400 
                        Else
                            validationOK = False
                        End If
                    Else
                        'Error getting the Analyzer Model, show it
                        ShowMessage(Me.Name & ".ReagentSamplePositioning_Load", resultData.ErrorCode, resultData.ErrorMessage, Me)
                        validationOK = False
                    End If
                Else
                    validationOK = (AnalyzerModelAttribute = "A400")   'This screen is only for Analyzers with Model A400 
                End If
            End If

            If (validationOK) Then
                Application.DoEvents() 'RH 14/12/2010

                'DL 10/05/2013
                Dim LisWithFilesMode As Boolean = IsLisWithFilesMode()

                GetScreenLabels(currentLanguage) 'PG 14/10/2010
                GetScreenButtonToolTips(currentLanguage, LisWithFilesMode) 'DL 10/05/2013

                PrepareButtons(LisWithFilesMode) 'DL 10/05/2013
                Application.DoEvents() 'RH 14/12/2010

                IAx00MainMDI.bsTSInfoButton.Enabled = True 'RH 14/10/2011

                PrepareIconNames()
                PrepareReagentLegendArea()
                Application.DoEvents() 'RH 14/12/2010

                'Prepare all the position controls
                PreparePositionsControlsNEW()

                InitializeScreen(False, "") 'AG 11/11/2010 - Added 2nd parameter
                ResetBorder()               'RH 14/12/2010
                Application.DoEvents()      'RH 14/12/2010

                'AG 21/03/2012 - If WS aborted then show message in the app status bar
                If (WorkSessionStatusAttribute = "ABORTED") Then
                    bsErrorProvider1.SetError(Nothing, GetMessageText(GlobalEnumerates.Messages.WS_ABORTED.ToString))
                End If
                'AG 21/03/2012

                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                myLogAcciones.CreateLogActivity("IWSROTORPositions.LOAD (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                "IWSROTORPositions.ReagentSamplePositioning_Load", EventLogEntryType.Information, False)
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                'BT #1520 - Use parameters MAX_APP_MEMORYUSAGE and MAX_SQL_MEMORYUSAGE into performance counters and shown a warning 
                '           message if at least one of them has been exceeded
                Dim PCounters As New AXPerformanceCounters(applicationMaxMemoryUsage, SQLMaxMemoryUsage)
                PCounters.GetAllCounters()
                If PCounters.LimitExceeded Then
                    MessageBox.Show(Me, GlobalConstants.MAX_APP_MEMORY_USAGE, My.Application.Info.Title, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
                PCounters = Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ReagentSamplePositioning_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReagentSamplePositioning_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Event for click in button of Automatic Positioning of Controls, Calibrators and Patient Samples
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR
    ''' Modified by: SA - If Samples Tab is not the current one, set it as the selected Tab. Added Try/Catch
    ''' </remarks>
    Private Sub bsSamplesAutoPosButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSamplesAutoPosButton.Click
        Try
            ReagentsSamplesAutoPositioning(True)
            'If (myRotorTypeForm <> "SAMPLES") Then RotorsTabs.SelectedTab = SamplesTab
            'BsBackwardButton_Click(sender, e) 'RH 21/02/2011 Show Rotor
            'TR 09/03/2012
            If Not mySelectedElementInfo Is Nothing Then
                mySelectedElementInfo.twksWSRotorContentByPosition.Clear()
                mySelectedElementInfo.AcceptChanges()
            End If

            If bsElementsTreeView.Nodes.Count > 0 Then
                Dim myNodes As TreeNodeCollection = bsElementsTreeView.Nodes
                bsElementsTreeView.SelectedNode = myNodes(0)
            End If
            'TR 09/03/2012 -END
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsSamplesAutoPosButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsSamplesAutoPosButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try
    End Sub

    ''' <summary>
    ''' Event for click in button of Automatic Positioning of Reagents and Additional Solutions
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR
    ''' Modified by: SA - If Reagents Tab is not the current one, set it as the selected Tab 
    ''' </remarks>
    Private Sub bsReagentAutoPosButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsReagentAutoPosButton.Click
        Try
            ReagentsSamplesAutoPositioning(False)
            'If (myRotorTypeForm <> "REAGENTS") Then RotorsTabs.SelectedTab = ReagentsTab
            'BsBackwardButton_Click(sender, e) 'RH 21/02/2011 Show Rotor

            'TR 09/03/2012 -Clear the Selected element info to avoid error after auto positioning that 
            'Change  the position status to free on selected elements.
            If Not mySelectedElementInfo Is Nothing Then
                mySelectedElementInfo.twksWSRotorContentByPosition.Clear()
                mySelectedElementInfo.AcceptChanges()
            End If
            'Select the first element node on the tree view
            If bsElementsTreeView.Nodes.Count > 0 Then
                Dim myNodes As TreeNodeCollection = bsElementsTreeView.Nodes
                bsElementsTreeView.SelectedNode = myNodes(0)
            End If
            'TR 09/03/2012 -END.
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsReagentAutoPosButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsReagentAutoPosButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try
    End Sub

    ''' <summary>
    ''' Event for TreeView DragEnter 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 14/12/2009 - Tested: OK 14/12/2009
    ''' Modified by: SA 22/12/2009 - Calls to MessageBox.Show were replaced by calls to the generic function 
    '''                              ShowMessage.  Added the call to method that write in the Application Log.  
    ''' </remarks>
    Private Sub bsElementsTreeView_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles bsElementsTreeView.DragEnter
        Try
            If (e.Data.GetDataPresent(DataFormats.Bitmap)) Then
                e.Effect = DragDropEffects.Copy
            Else
                e.Effect = DragDropEffects.None
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsElementsTreeView_DragEnter ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsElementsTreeView_DragEnter", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Event for init the dragging from the TreeView  
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by:  AG 14/12/2009
    ''' Tested: OK 14/12/2009
    ''' Modified by: SA 22/12/2009 - Calls to MessageBox.Show were replaced by calls to the generic function 
    '''                              ShowMessage.  Added the call tTreeNode: CALIB 1Po method that write in the Application Log. 
    '''              AG 18/01/2010 - Allow or not this event depending on load screen status (isTreeDragDropAllowed)
    ''' </remarks>
    Private Sub bsElementsTreeView_ItemDrag(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemDragEventArgs) Handles bsElementsTreeView.ItemDrag
        Try
            If isTreeDragDropAllowed = True Then
                sourceNode = e.Item
                isDragging = True
                DoDragDrop(e.Item.ToString(), DragDropEffects.Move + DragDropEffects.Copy)
            Else
                isDragging = False
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsElemeTreeNode: ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsElementsTreeView_ItemDrag", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Finish Drag and Drop into a Position of the current Rotor
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 14/12/2009 - Tested: OK 12/12/2009
    ''' Modified by: SA 22/12/2009 - Calls to MessageBox.Show were replaced by calls to the generic function 
    '''                              ShowMessage 
    '''              TR 04/01/2010 - Add elseif condition to validate if is dragging from the rotor area.
    '''                            - Validate if the position is free before changing position (Tested: OK 05/01/2010)
    '''              SA 26/07/2010 - Call new function PendingPositionedElements to enable/disable the Warnings Button 
    '''              AG 05/10/2010 - Changed PictureBox for BSRImage
    ''' </remarks>
    Private Sub PositionControl_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) _
                                   Handles Sam19.MouseHover, Sam18.MouseHover, Sam17.MouseHover, _
                                   Sam16.MouseHover, Sam15.MouseHover, Sam145.MouseHover, Sam144.MouseHover, _
                                   Sam143.MouseHover, Sam142.MouseHover, Sam141.MouseHover, Sam140.MouseHover, _
                                   Sam14.MouseHover, Sam139.MouseHover, Sam138.MouseHover, Sam137.MouseHover, _
                                   Sam136.MouseHover, Sam135.MouseHover, Sam134.MouseHover, Sam133.MouseHover, _
                                   Sam132.MouseHover, Sam131.MouseHover, Sam130.MouseHover, Sam13.MouseHover, _
                                   Sam129.MouseHover, Sam128.MouseHover, Sam127.MouseHover, Sam126.MouseHover, _
                                   Sam125.MouseHover, Sam124.MouseHover, Sam123.MouseHover, Sam122.MouseHover, _
                                   Sam121.MouseHover, Sam120.MouseHover, Sam12.MouseHover, Sam119.MouseHover, _
                                   Sam118.MouseHover, Sam117.MouseHover, Sam116.MouseHover, Sam115.MouseHover, _
                                   Sam114.MouseHover, Sam113.MouseHover, Sam112.MouseHover, Sam111.MouseHover, _
                                   Sam110.MouseHover, Sam11.MouseHover, Sam399.MouseHover, Sam398.MouseHover, _
                                   Sam397.MouseHover, Sam396.MouseHover, Sam395.MouseHover, Sam394.MouseHover, _
                                   Sam393.MouseHover, Sam392.MouseHover, Sam391.MouseHover, Sam3135.MouseHover, _
                                   Sam3134.MouseHover, Sam3133.MouseHover, Sam3132.MouseHover, Sam3131.MouseHover, _
                                   Sam3130.MouseHover, Sam3129.MouseHover, Sam3128.MouseHover, Sam3127.MouseHover, _
                                   Sam3126.MouseHover, Sam3125.MouseHover, Sam3124.MouseHover, Sam3123.MouseHover, _
                                   Sam3122.MouseHover, Sam3121.MouseHover, Sam3120.MouseHover, Sam3119.MouseHover, _
                                   Sam3118.MouseHover, Sam3117.MouseHover, Sam3116.MouseHover, Sam3115.MouseHover, _
                                   Sam3114.MouseHover, Sam3113.MouseHover, Sam3112.MouseHover, Sam3111.MouseHover, _
                                   Sam3110.MouseHover, Sam3109.MouseHover, Sam3108.MouseHover, Sam3107.MouseHover, _
                                   Sam3106.MouseHover, Sam3105.MouseHover, Sam3104.MouseHover, Sam3103.MouseHover, _
                                   Sam3102.MouseHover, Sam3101.MouseHover, Sam3100.MouseHover, Reag288.MouseHover, _
                                   Reag287.MouseHover, Reag286.MouseHover, Reag285.MouseHover, Reag284.MouseHover, _
                                   Reag14.MouseHover, Reag13.MouseHover, Reag12.MouseHover, Reag11.MouseHover, _
                                   Sam260.MouseHover, Sam259.MouseHover, Sam258.MouseHover, Sam257.MouseHover, _
                                   Sam256.MouseHover, Sam255.MouseHover, Sam254.MouseHover, Sam253.MouseHover, _
                                   Sam252.MouseHover, Sam251.MouseHover, Sam250.MouseHover, Sam249.MouseHover, _
                                   Sam248.MouseHover, Sam247.MouseHover, Sam246.MouseHover, Sam280.MouseHover, _
                                   Sam279.MouseHover, Sam278.MouseHover, Sam277.MouseHover, Sam276.MouseHover, _
                                   Sam275.MouseHover, Sam274.MouseHover, Sam273.MouseHover, Sam272.MouseHover, _
                                   Sam271.MouseHover, Sam270.MouseHover, Sam269.MouseHover, Sam268.MouseHover, _
                                   Sam267.MouseHover, Sam266.MouseHover, Sam265.MouseHover, Sam264.MouseHover, _
                                   Sam263.MouseHover, Sam262.MouseHover, Sam261.MouseHover, Sam289.MouseHover, _
                                   Sam288.MouseHover, Sam287.MouseHover, Sam286.MouseHover, Sam285.MouseHover, _
                                   Sam284.MouseHover, Sam283.MouseHover, Sam282.MouseHover, Sam281.MouseHover, _
                                   Sam290.MouseHover, Reag19.MouseHover, Reag18.MouseHover, Reag17.MouseHover, _
                                   Reag16.MouseHover, Reag15.MouseHover, Reag119.MouseHover, Reag118.MouseHover, _
                                   Reag117.MouseHover, Reag116.MouseHover, Reag115.MouseHover, Reag114.MouseHover, _
                                   Reag113.MouseHover, Reag112.MouseHover, Reag111.MouseHover, Reag110.MouseHover, _
                                   Reag130.MouseHover, Reag129.MouseHover, Reag128.MouseHover, Reag127.MouseHover, _
                                   Reag126.MouseHover, Reag125.MouseHover, Reag124.MouseHover, Reag123.MouseHover, _
                                   Reag122.MouseHover, Reag121.MouseHover, Reag120.MouseHover, Reag140.MouseHover, _
                                   Reag139.MouseHover, Reag138.MouseHover, Reag137.MouseHover, Reag136.MouseHover, _
                                   Reag135.MouseHover, Reag134.MouseHover, Reag133.MouseHover, Reag132.MouseHover, _
                                   Reag131.MouseHover, Reag144.MouseHover, Reag143.MouseHover, Reag142.MouseHover, _
                                   Reag141.MouseHover, Reag255.MouseHover, Reag254.MouseHover, Reag253.MouseHover, _
                                   Reag252.MouseHover, Reag251.MouseHover, Reag250.MouseHover, Reag249.MouseHover, _
                                   Reag248.MouseHover, Reag247.MouseHover, Reag246.MouseHover, Reag245.MouseHover, _
                                   Reag270.MouseHover, Reag269.MouseHover, Reag268.MouseHover, Reag267.MouseHover, _
                                   Reag266.MouseHover, Reag265.MouseHover, Reag264.MouseHover, Reag263.MouseHover, _
                                   Reag262.MouseHover, Reag261.MouseHover, Reag260.MouseHover, Reag259.MouseHover, _
                                   Reag258.MouseHover, Reag257.MouseHover, Reag256.MouseHover, Reag280.MouseHover, _
                                   Reag279.MouseHover, Reag278.MouseHover, Reag277.MouseHover, Reag276.MouseHover, _
                                   Reag275.MouseHover, Reag274.MouseHover, Reag273.MouseHover, Reag272.MouseHover, _
                                   Reag271.MouseHover, Reag283.MouseHover, Reag282.MouseHover, Reag281.MouseHover

        Try
            'If sender.GetType().Name = "BSRImage" Then 'AG 05/10/2010 - Change all positions (samples and reagents) from PictureBox to BSRImage
            If TypeOf sender Is BSRImage Then 'RH 14/02/2011 Don't rely on changeable text strings, but in class type

                'Find the ring & cell number where user Drops
                ringNumberForm = 0
                cellNumberForm = 0

                'Dim myPictureBox As New BSRImage
                Dim myPictureBox As BSRImage
                myPictureBox = CType(sender, BSRImage)
                ringNumberForm = CType(myPictureBox.Tag().ToString().Split(",")(0), Integer)
                cellNumberForm = CType(myPictureBox.Tag().ToString().Split(",")(1), Integer)

                'TR 04/01/2010 - Validate if the new selected position is free 
                If IsPositionFree(ringNumberForm, cellNumberForm) Then
                    'Validate if it is dragging from the treeview area
                    If isDragging Then
                        Me.DragTreeViewElement(sourceNode, WorkSessionIDAttribute, AnalyzerIDAttribute)

                        'TR 19/01/2010 select and show the information of selected cell.
                        mySelectedElementInfo = GetLocalPositionInfo(ringNumberForm, cellNumberForm, False)
                        ShowPositionInfoArea(AnalyzerIDAttribute, myRotorTypeForm, ringNumberForm, cellNumberForm)

                        'mySelectedElementInfo = GetLocalPositionInfo(ringNumberForm, cellNumberForm, False) 'AG 09/12/2010

                        isDragging = False
                    End If
                Else
                    isDragging = False 'RH 07/10/2011
                End If

            End If 'AG 05/10/2010TreeNode: Agua Destilada (0,132)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PositionControl_MouseHover ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PositionControl_MouseHover", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Event for MouseDown in a Position of the current Rotor - When the User selects a Position on the current Rotor, 
    ''' this method calls the GetLocalPositionInformation method and load the information into mySelectElementInfoDS 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 21/12/2009
    ''' Modified by: TR 04/01/2010 - Do the show position information after the validation on the picturebox tag property.
    '''                            - Do only one convertion for the ring an cell number.
    '''                            - Activate controls in the correspondent Info Area before get and loading the information.
    '''              TR 05/01/2010 - Add the bottle size change call. (TESTED: OK 05/01/2010)
    '''              AG 18/01/2010 - Allow or not this event depending on load screen status (isRotorDragDropAllowed / isChangeTubeSizeAllowed)
    '''              TR 20/01/2010 - Reduce the validation code an improve the code to avoid blink while selection several cells.
    '''              AG 26/01/2010 - Selecting bottles in different rings the bottle size combo is disabled (Tested OK)
    '''              AG 27/01/2010
    '''              AG 05/10/2010 - Changed all positions (Samples and Reagents) from PictureBox to BSRImage
    '''              SA 04/11/2013 - BT #1371 ==> When several positions are selected in Reagents Rotor, if at least one of them have a valid Barcode (field BarcodeInfo 
    '''                              is informed), the ComboBox that allow changing the Bottle Size has to be disabled 
    '''              TR 08/11/2013 - BT #1358 Set the sample rotor functionality on pause mode.
    '''              TR 15/11/2013 - BT #1358(2) Set the reagent rotor functionality on pause mode.
    '''              IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Sub PositionControl_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Sam19.MouseDown, Sam18.MouseDown, Sam17.MouseDown, Sam16.MouseDown, Sam15.MouseDown, Sam145.MouseDown, Sam144.MouseDown, Sam143.MouseDown, Sam142.MouseDown, Sam141.MouseDown, Sam140.MouseDown, Sam14.MouseDown, Sam139.MouseDown, Sam138.MouseDown, Sam137.MouseDown, Sam136.MouseDown, Sam135.MouseDown, Sam134.MouseDown, Sam133.MouseDown, Sam132.MouseDown, Sam131.MouseDown, Sam130.MouseDown, Sam13.MouseDown, Sam129.MouseDown, Sam128.MouseDown, Sam127.MouseDown, Sam126.MouseDown, Sam125.MouseDown, Sam124.MouseDown, Sam123.MouseDown, Sam122.MouseDown, Sam121.MouseDown, Sam120.MouseDown, Sam12.MouseDown, Sam119.MouseDown, Sam118.MouseDown, Sam117.MouseDown, Sam116.MouseDown, Sam115.MouseDown, Sam114.MouseDown, Sam113.MouseDown, Sam112.MouseDown, Sam111.MouseDown, Sam110.MouseDown, Sam11.MouseDown, Sam399.MouseDown, Sam398.MouseDown, Sam397.MouseDown, Sam396.MouseDown, Sam395.MouseDown, Sam394.MouseDown, Sam393.MouseDown, Sam392.MouseDown, Sam391.MouseDown, Sam3135.MouseDown, Sam3134.MouseDown, Sam3133.MouseDown, Sam3132.MouseDown, Sam3131.MouseDown, Sam3130.MouseDown, Sam3129.MouseDown, Sam3128.MouseDown, Sam3127.MouseDown, Sam3126.MouseDown, Sam3125.MouseDown, Sam3124.MouseDown, Sam3123.MouseDown, Sam3122.MouseDown, Sam3121.MouseDown, Sam3120.MouseDown, Sam3119.MouseDown, Sam3118.MouseDown, Sam3117.MouseDown, Sam3116.MouseDown, Sam3115.MouseDown, Sam3114.MouseDown, Sam3113.MouseDown, Sam3112.MouseDown, Sam3111.MouseDown, Sam3110.MouseDown, Sam3109.MouseDown, Sam3108.MouseDown, Sam3107.MouseDown, Sam3106.MouseDown, Sam3105.MouseDown, Sam3104.MouseDown, Sam3103.MouseDown, Sam3102.MouseDown, Sam3101.MouseDown, Sam3100.MouseDown, Reag288.MouseDown, Reag287.MouseDown, Reag286.MouseDown, Reag285.MouseDown, Reag284.MouseDown, Reag14.MouseDown, Reag13.MouseDown, Reag12.MouseDown, Reag11.MouseDown, Sam260.MouseDown, Sam259.MouseDown, Sam258.MouseDown, Sam257.MouseDown, Sam256.MouseDown, Sam255.MouseDown, Sam254.MouseDown, Sam253.MouseDown, Sam252.MouseDown, Sam251.MouseDown, Sam250.MouseDown, Sam249.MouseDown, Sam248.MouseDown, Sam247.MouseDown, Sam246.MouseDown, Sam280.MouseDown, Sam279.MouseDown, Sam278.MouseDown, Sam277.MouseDown, Sam276.MouseDown, Sam275.MouseDown, Sam274.MouseDown, Sam273.MouseDown, Sam272.MouseDown, Sam271.MouseDown, Sam270.MouseDown, Sam269.MouseDown, Sam268.MouseDown, Sam267.MouseDown, Sam266.MouseDown, Sam265.MouseDown, Sam264.MouseDown, Sam263.MouseDown, Sam262.MouseDown, Sam261.MouseDown, Sam290.MouseDown, Sam289.MouseDown, Sam288.MouseDown, Sam287.MouseDown, Sam286.MouseDown, Sam285.MouseDown, Sam284.MouseDown, Sam283.MouseDown, Sam282.MouseDown, Sam281.MouseDown, Reag19.MouseDown, Reag18.MouseDown, Reag17.MouseDown, Reag16.MouseDown, Reag15.MouseDown, Reag119.MouseDown, Reag118.MouseDown, Reag117.MouseDown, Reag116.MouseDown, Reag115.MouseDown, Reag114.MouseDown, Reag113.MouseDown, Reag112.MouseDown, Reag111.MouseDown, Reag110.MouseDown, Reag130.MouseDown, Reag129.MouseDown, Reag128.MouseDown, Reag127.MouseDown, Reag126.MouseDown, Reag125.MouseDown, Reag124.MouseDown, Reag123.MouseDown, Reag122.MouseDown, Reag121.MouseDown, Reag120.MouseDown, Reag140.MouseDown, Reag139.MouseDown, Reag138.MouseDown, Reag137.MouseDown, Reag136.MouseDown, Reag135.MouseDown, Reag134.MouseDown, Reag133.MouseDown, Reag132.MouseDown, Reag131.MouseDown, Reag144.MouseDown, Reag143.MouseDown, Reag142.MouseDown, Reag141.MouseDown, Reag255.MouseDown, Reag254.MouseDown, Reag253.MouseDown, Reag252.MouseDown, Reag251.MouseDown, Reag250.MouseDown, Reag249.MouseDown, Reag248.MouseDown, Reag247.MouseDown, Reag246.MouseDown, Reag245.MouseDown, Reag270.MouseDown, Reag269.MouseDown, Reag268.MouseDown, Reag267.MouseDown, Reag266.MouseDown, Reag265.MouseDown, Reag264.MouseDown, Reag263.MouseDown, Reag262.MouseDown, Reag261.MouseDown, Reag260.MouseDown, Reag259.MouseDown, Reag258.MouseDown, Reag257.MouseDown, Reag256.MouseDown, Reag280.MouseDown, Reag279.MouseDown, Reag278.MouseDown, Reag277.MouseDown, Reag276.MouseDown, Reag275.MouseDown, Reag274.MouseDown, Reag273.MouseDown, Reag272.MouseDown, Reag271.MouseDown, Reag283.MouseDown, Reag282.MouseDown, Reag281.MouseDown
        Try
            If (TypeOf sender Is BSRImage) Then
                Dim myPictureBox As BSRImage = CType(sender, BSRImage)

                'Validate that the Tag property is not empty to get the information.
                If (Not myPictureBox.Tag Is Nothing) Then
                    'Get the selected Ring and Cell Number
                    Dim myRingNumber As Integer = CType(myPictureBox.Tag.ToString().Split(",")(0), Integer)
                    Dim myCellNumber As Integer = CType(myPictureBox.Tag.ToString().Split(",")(1), Integer)

                    'Validate if the pressed key is Shift or Control to allow multiselection
                    If (Control.ModifierKeys And Keys.Control) = Keys.Control OrElse (Control.ModifierKeys And Keys.Shift) = Keys.Shift Then
                        'Get data of the Selected Cell and load it into the global variable mySelectedElementInfo
                        mySelectedElementInfo = GetLocalPositionInfo(myRingNumber, myCellNumber, True)

                        'Validate if the clicked Mouse Button is the LEFT one: while this button is pressed dragging in the rotor area is enabled
                    ElseIf (e.Button = MouseButtons.Left) Then
                        ClearSelection() 'TR 03/04/2012 - Clear any previous selection. FOR SINGLE SELECTION.
                        'isRotorDragging = isRotorDragDropAllowed Commented by TR 08/11/2013 - BT#1358.
                        Dim ActionAllowed As Boolean = True
                        'Get data of the Selected Cell and load it into the global variable mySelectedElementInfo
                        mySelectedElementInfo = GetLocalPositionInfo(myRingNumber, myCellNumber, False)

                        'TR 08/11/2013 - BT#1358 validate if analyser is pause
                        If AnalyzerController.Instance.Analyzer.AllowScanInRunning OrElse _
                            AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then

                            If myRotorTypeForm = "SAMPLES" Then
                                ActionAllowed = VerifyActionsAllowedInSampleRotor(mySelectedElementInfo, True)
                            ElseIf myRotorTypeForm = "REAGENTS" Then
                                ActionAllowed = VerifyActionsAllowedInReagentsRotor()
                            End If

                        End If

                        isRotorDragging = (isRotorDragDropAllowed AndAlso ActionAllowed)
                        'TR 08/11/2013 - BT#1358 -END

                        isInfoAreaAllowed = isRotorDragDropAllowed 'RH 26/09/2011 Forces to enable the info area.
                        ShowPositionInfoArea(AnalyzerIDAttribute, myRotorTypeForm, myRingNumber, myCellNumber)

                        'RH 10/10/2011 Execute this line after ShowPositionInfoArea(), because Enabled Status of some controls depends on its contents
                        If (mySelectedElementInfo.twksWSRotorContentByPosition(0).Status <> "FREE") Then
                            SetInfoAreaEnabledStatus(myRotorTypeForm, True)
                        End If

                        'Validate if the clicked Mouse Button is the RIGHT one: the user want's to change the Bottle/Tube size 
                    ElseIf (e.Button = MouseButtons.Right) Then
                        'Get data of the Selected Cell and load it into the global variable mySelectedElementInfo
                        mySelectedElementInfo = GetLocalPositionInfo(myRingNumber, myCellNumber, False)

                        isInfoAreaAllowed = isRotorDragDropAllowed 'RH 26/09/2011 Forces to enable the info area.
                        ShowPositionInfoArea(AnalyzerIDAttribute, myRotorTypeForm, myRingNumber, myCellNumber)

                        'RH 10/10/2011 Execute this line after ShowPositionInfoArea(), because Enabled Status of some controls depend on its contents
                        SetInfoAreaEnabledStatus(myRotorTypeForm, True)

                        If (isChangeTubeSizeAllowed) Then
                            Dim CanChange As Boolean = True

                            'Select the current Bottle/Tube size depending on the rotor type
                            Dim currentBottleSize As String = String.Empty
                            If (myRotorTypeForm = "SAMPLES") Then
                                currentBottleSize = CType(bsTubeSizeComboBox.SelectedValue, String)
                                'TR 08/11/2013 -BT #1358 '26/11/2013 -BT #1404 Validate the running time 
                                If AnalyzerController.Instance.Analyzer.AllowScanInRunning OrElse _
                                   AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                                    'Set the function value to canchange variable.
                                    CanChange = VerifyActionsAllowedInSampleRotor(mySelectedElementInfo, False)
                                End If
                                'TR 08/11/2013 -BT #1358 END
                            ElseIf (myRotorTypeForm = "REAGENTS") Then
                                currentBottleSize = CType(bsBottleSizeComboBox.SelectedValue, String)
                                'TR 15/11/2013 - BT #1358(2) -Verify for Reagents rotorIf (analyzerInPAUSE) Then call new function 
                                '                             VerifyActionsAllowedInReagentsRotor and assign the return value to variable 
                                '                             canChange Else canChange = bsBottleSizeComboBox.Enabled.
                                If (Not String.IsNullOrEmpty(mySelectedElementInfo.twksWSRotorContentByPosition.First.BarCodeInfo) AndAlso _
                                    Not String.IsNullOrEmpty(mySelectedElementInfo.twksWSRotorContentByPosition.First.BarcodeStatus) AndAlso _
                                    mySelectedElementInfo.twksWSRotorContentByPosition.First.BarcodeStatus = "OK") Then
                                    CanChange = False
                                Else
                                    If (AnalyzerController.Instance.Analyzer.AllowScanInRunning) Then
                                        CanChange = VerifyActionsAllowedInReagentsRotor()
                                    Else
                                        CanChange = bsBottleSizeComboBox.Enabled
                                    End If
                                End If
                            End If

                            If (CanChange) Then 'RH 10/10/2011
                                Dim SelectedElement As Integer = 0
                                If (Not mySelectedElementInfo Is Nothing) Then SelectedElement = mySelectedElementInfo.twksWSRotorContentByPosition.Rows.Count

                                'TR 14/01/2010 - Get the next Bottle/Tube size and validate the result before changing the Bottle/Tube
                                Dim myNextBottleSize As String = GetNextBottleSize(myRotorTypeForm, myRingNumber, currentBottleSize)
                                If (myNextBottleSize <> "") Then
                                    ChangeBottleSize(myRingNumber, myCellNumber, myNextBottleSize)
                                End If

                                'TR 09/03/2012 - Get the new information into my SelectedElement
                                mySelectedElementInfo = GetLocalPositionInfo(myRingNumber, myCellNumber, False)

                                'Validate if element status is Pos then Update the status for the same element id to POS
                                If (mySelectedElementInfo.twksWSRotorContentByPosition(0).ElementStatus = "POS") Then
                                    SetElementsStatusByElementID(AnalyzerIDAttribute, WorkSessionIDAttribute, mySelectedElementInfo.twksWSRotorContentByPosition(0).ElementID, "POS")
                                ElseIf (mySelectedElementInfo.twksWSRotorContentByPosition(0).ElementStatus = "INCOMPLETE") Then
                                    SetElementsStatusByElementID(AnalyzerIDAttribute, WorkSessionIDAttribute, mySelectedElementInfo.twksWSRotorContentByPosition(0).ElementID, "INCOMPLETE")
                                End If
                                'TR 09/03/2012 -END 

                                'Refresh the position information area
                                ShowPositionInfoArea(AnalyzerIDAttribute, myRotorTypeForm, myRingNumber, myCellNumber)
                                'END TR 05/01/2010

                                If (SelectedElement = 1) Then
                                    MarkSelectedPosition(myRingNumber, myCellNumber, True)
                                End If
                            Else
                                'TR 11/11/2013 -BT #1358 Show message only if reagent rotor.
                                If myRotorTypeForm = "REAGENTS" Then
                                    ShowMessage("AX00", GlobalEnumerates.Messages.BOTTLE_CHANGE_NOT_ALLOWED.ToString()) 'RH 10/10/2011
                                End If
                            End If
                        End If
                    End If

                    If (mySelectedElementInfo.twksWSRotorContentByPosition.Rows.Count > 1) Then
                        Me.CleanInfoArea(False)
                        Me.SetInfoAreaEnabledStatus(myRotorTypeForm, True)

                        'AG 27/01/2010 - In Reagents Rotor when select positions in different rings the change bottle size combo is disabled                                                           
                        If (myRotorTypeForm = "REAGENTS") Then
                            If (isChangeTubeSizeAllowed) Then
                                Dim query As List(Of Integer) = (From a In mySelectedElementInfo.twksWSRotorContentByPosition.AsEnumerable _
                                                               Select a.RingNumber Distinct).ToList()

                                If (query.Count > 1) Then
                                    bsBottleSizeComboBox.Enabled = False
                                    bsBottleSizeComboBox.BackColor = Color.Gainsboro

                                Else
                                    'If all selected Positions are in the same Ring but at least one of them have a valid Barcode Informed, 
                                    'the BottleSizes ComboBox is also disabled
                                    query = (From a In mySelectedElementInfo.twksWSRotorContentByPosition.AsEnumerable _
                                        Where Not a.IsBarCodeInfoNull AndAlso a.BarCodeInfo <> String.Empty _
                                           Select a.CellNumber Distinct).ToList()
                                    If (query.Count > 0) Then
                                        bsBottleSizeComboBox.Enabled = False
                                        bsBottleSizeComboBox.BackColor = Color.Gainsboro
                                    End If
                                End If
                            End If

                            bsReagentsBarCodeTextBox.Enabled = False
                            bsReagentsBarCodeTextBox.BackColor = Color.Gainsboro

                        ElseIf (myRotorTypeForm = "SAMPLES") Then
                            'bsSamplesBarcodeTextBox.Enabled = False
                            'bsSamplesBarcodeTextBox.BackColor = Color.Gainsboro
                        End If
                    End If

                    'Validate if there is an image on the selected position control (PictureBox).
                    'RH 28/04/2011 Status <> "FREE" to avoid blinking on unneeded operation
                    If (mySelectedElementInfo.twksWSRotorContentByPosition.Rows.Count > 0) Then
                        If (Not myPictureBox.Image Is Nothing AndAlso mySelectedElementInfo.twksWSRotorContentByPosition(0).Status <> "FREE") Then
                            myPictureBox.DoDragDrop(myPictureBox.Image, DragDropEffects.Copy)
                        End If
                    End If
                End If
            End If 'AG 05/10/2010
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PositionControl_MouseDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PositionControl_MouseDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Event is activate whe the user release the pressed mouse button.
    ''' Validate the left mouse button to disable the draggin. 
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 04/01/2010
    ''' </remarks>
    Private Sub PositionControl_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
                        Handles Sam19.MouseUp, Sam18.MouseUp, Sam17.MouseUp, Sam16.MouseUp, Sam15.MouseUp, _
                        Sam145.MouseUp, Sam144.MouseUp, Sam143.MouseUp, Sam142.MouseUp, Sam141.MouseUp, Sam140.MouseUp, Sam14.MouseUp, _
                        Sam139.MouseUp, Sam138.MouseUp, Sam137.MouseUp, Sam136.MouseUp, Sam135.MouseUp, Sam134.MouseUp, Sam133.MouseUp, _
                        Sam132.MouseUp, Sam131.MouseUp, Sam130.MouseUp, Sam13.MouseUp, Sam129.MouseUp, Sam128.MouseUp, Sam127.MouseUp, _
                        Sam126.MouseUp, Sam125.MouseUp, Sam124.MouseUp, Sam123.MouseUp, Sam122.MouseUp, Sam121.MouseUp, Sam120.MouseUp, _
                        Sam12.MouseUp, Sam119.MouseUp, Sam118.MouseUp, Sam117.MouseUp, Sam116.MouseUp, Sam115.MouseUp, Sam114.MouseUp, _
                        Sam113.MouseUp, Sam112.MouseUp, Sam111.MouseUp, Sam110.MouseUp, Sam11.MouseUp, Sam399.MouseUp, Sam398.MouseUp, _
                        Sam397.MouseUp, Sam396.MouseUp, Sam395.MouseUp, Sam394.MouseUp, Sam393.MouseUp, Sam392.MouseUp, Sam391.MouseUp, _
                        Sam3135.MouseUp, Sam3134.MouseUp, Sam3133.MouseUp, Sam3132.MouseUp, Sam3131.MouseUp, Sam3130.MouseUp, Sam3129.MouseUp, _
                        Sam3128.MouseUp, Sam3127.MouseUp, Sam3126.MouseUp, Sam3125.MouseUp, Sam3124.MouseUp, Sam3123.MouseUp, Sam3122.MouseUp, _
                        Sam3121.MouseUp, Sam3120.MouseUp, Sam3119.MouseUp, Sam3118.MouseUp, Sam3117.MouseUp, Sam3116.MouseUp, Sam3115.MouseUp, _
                        Sam3114.MouseUp, Sam3113.MouseUp, Sam3112.MouseUp, Sam3111.MouseUp, Sam3110.MouseUp, Sam3109.MouseUp, Sam3108.MouseUp, _
                        Sam3107.MouseUp, Sam3106.MouseUp, Sam3105.MouseUp, Sam3104.MouseUp, Sam3103.MouseUp, Sam3102.MouseUp, Sam3101.MouseUp, _
                        Sam3100.MouseUp, Reag288.MouseUp, Reag287.MouseUp, Reag286.MouseUp, Reag285.MouseUp, Reag284.MouseUp, Reag14.MouseUp, _
                        Reag13.MouseUp, Reag12.MouseUp, Reag11.MouseUp, Sam260.MouseUp, Sam259.MouseUp, Sam258.MouseUp, Sam257.MouseUp, _
                        Sam256.MouseUp, Sam255.MouseUp, Sam254.MouseUp, Sam253.MouseUp, Sam252.MouseUp, Sam251.MouseUp, Sam250.MouseUp, _
                        Sam249.MouseUp, Sam248.MouseUp, Sam247.MouseUp, Sam246.MouseUp, Sam280.MouseUp, Sam279.MouseUp, Sam278.MouseUp, _
                        Sam277.MouseUp, Sam276.MouseUp, Sam275.MouseUp, Sam274.MouseUp, Sam273.MouseUp, Sam272.MouseUp, Sam271.MouseUp, _
                        Sam270.MouseUp, Sam269.MouseUp, Sam268.MouseUp, Sam267.MouseUp, Sam266.MouseUp, Sam265.MouseUp, Sam264.MouseUp, _
                        Sam263.MouseUp, Sam262.MouseUp, Sam261.MouseUp, SamplesTab.MouseUp, Sam289.MouseUp, Sam288.MouseUp, Sam287.MouseUp, _
                        Sam286.MouseUp, Sam285.MouseUp, Sam284.MouseUp, Sam283.MouseUp, Sam282.MouseUp, Sam281.MouseUp, Sam290.MouseUp, _
                        Reag19.MouseUp, Reag18.MouseUp, Reag17.MouseUp, Reag16.MouseUp, Reag15.MouseUp, Reag119.MouseUp, Reag118.MouseUp, _
                        Reag117.MouseUp, Reag116.MouseUp, Reag115.MouseUp, Reag114.MouseUp, Reag113.MouseUp, Reag112.MouseUp, Reag111.MouseUp, _
                        Reag110.MouseUp, Reag130.MouseUp, Reag129.MouseUp, Reag128.MouseUp, Reag127.MouseUp, Reag126.MouseUp, Reag125.MouseUp, _
                        Reag124.MouseUp, Reag123.MouseUp, Reag122.MouseUp, Reag121.MouseUp, Reag120.MouseUp, Reag140.MouseUp, Reag139.MouseUp, _
                        Reag138.MouseUp, Reag137.MouseUp, Reag136.MouseUp, Reag135.MouseUp, Reag134.MouseUp, Reag133.MouseUp, Reag132.MouseUp, _
                        Reag131.MouseUp, Reag144.MouseUp, Reag143.MouseUp, Reag142.MouseUp, Reag141.MouseUp, Reag255.MouseUp, Reag254.MouseUp, _
                        Reag253.MouseUp, Reag252.MouseUp, Reag251.MouseUp, Reag250.MouseUp, Reag249.MouseUp, Reag248.MouseUp, Reag247.MouseUp, _
                        Reag246.MouseUp, Reag245.MouseUp, Reag270.MouseUp, Reag269.MouseUp, Reag268.MouseUp, Reag267.MouseUp, Reag266.MouseUp, _
                        Reag265.MouseUp, Reag264.MouseUp, Reag263.MouseUp, Reag262.MouseUp, Reag261.MouseUp, Reag260.MouseUp, Reag259.MouseUp, _
                        Reag258.MouseUp, Reag257.MouseUp, Reag256.MouseUp, Reag280.MouseUp, Reag279.MouseUp, Reag278.MouseUp, Reag277.MouseUp, _
                        Reag276.MouseUp, Reag275.MouseUp, Reag274.MouseUp, Reag273.MouseUp, Reag272.MouseUp, Reag271.MouseUp, Reag283.MouseUp, _
                        Reag282.MouseUp, Reag281.MouseUp

        If e.Button = MouseButtons.Left Then
            isRotorDragging = False
        End If
    End Sub

    ''' <summary>
    ''' Event for Dragging from one to Position to another in the current Rotor
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by:  TR 21/12/2009
    ''' Modified by: SA 22/12/2009 - Calls to MessageBox.Show were replaced by calls to the generic function 
    '''                              ShowMessage. Added the call to method that write in the Application Log.
    ''' </remarks>
    Private Sub PositionControl_DragEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) _
                        Handles Sam19.DragEnter, Sam18.DragEnter, Sam17.DragEnter, Sam16.DragEnter, Sam15.DragEnter, Sam145.DragEnter, _
                        Sam144.DragEnter, Sam143.DragEnter, Sam142.DragEnter, Sam141.DragEnter, Sam140.DragEnter, Sam14.DragEnter, _
                        Sam139.DragEnter, Sam138.DragEnter, Sam137.DragEnter, Sam136.DragEnter, Sam135.DragEnter, Sam134.DragEnter, _
                        Sam133.DragEnter, Sam132.DragEnter, Sam131.DragEnter, Sam130.DragEnter, Sam13.DragEnter, Sam129.DragEnter, _
                        Sam128.DragEnter, Sam127.DragEnter, Sam126.DragEnter, Sam125.DragEnter, Sam124.DragEnter, Sam123.DragEnter, _
                        Sam122.DragEnter, Sam121.DragEnter, Sam120.DragEnter, Sam12.DragEnter, Sam119.DragEnter, Sam118.DragEnter, _
                        Sam117.DragEnter, Sam116.DragEnter, Sam115.DragEnter, Sam114.DragEnter, Sam113.DragEnter, Sam112.DragEnter, _
                        Sam111.DragEnter, Sam110.DragEnter, Sam11.DragEnter, Sam399.DragEnter, Sam398.DragEnter, Sam397.DragEnter, _
                        Sam396.DragEnter, Sam395.DragEnter, Sam394.DragEnter, Sam393.DragEnter, Sam392.DragEnter, Sam391.DragEnter, _
                        Sam3135.DragEnter, Sam3134.DragEnter, Sam3133.DragEnter, Sam3132.DragEnter, Sam3131.DragEnter, Sam3130.DragEnter, _
                        Sam3129.DragEnter, Sam3128.DragEnter, Sam3127.DragEnter, Sam3126.DragEnter, Sam3125.DragEnter, Sam3124.DragEnter, _
                        Sam3123.DragEnter, Sam3122.DragEnter, Sam3121.DragEnter, Sam3120.DragEnter, Sam3119.DragEnter, Sam3118.DragEnter, _
                        Sam3117.DragEnter, Sam3116.DragEnter, Sam3115.DragEnter, Sam3114.DragEnter, Sam3113.DragEnter, Sam3112.DragEnter, _
                        Sam3111.DragEnter, Sam3110.DragEnter, Sam3109.DragEnter, Sam3108.DragEnter, Sam3107.DragEnter, Sam3106.DragEnter, _
                        Sam3105.DragEnter, Sam3104.DragEnter, Sam3103.DragEnter, Sam3102.DragEnter, Sam3101.DragEnter, Sam3100.DragEnter, _
                        Reag288.DragEnter, Reag287.DragEnter, Reag286.DragEnter, Reag285.DragEnter, Reag284.DragEnter, Reag14.DragEnter, _
                        Reag13.DragEnter, Reag12.DragEnter, Reag11.DragEnter, Sam260.DragEnter, Sam259.DragEnter, Sam258.DragEnter, _
                        Sam257.DragEnter, Sam256.DragEnter, Sam255.DragEnter, Sam254.DragEnter, Sam253.DragEnter, Sam252.DragEnter, _
                        Sam251.DragEnter, Sam250.DragEnter, Sam249.DragEnter, Sam248.DragEnter, Sam247.DragEnter, Sam246.DragEnter, _
                        Sam280.DragEnter, Sam279.DragEnter, Sam278.DragEnter, Sam277.DragEnter, Sam276.DragEnter, Sam275.DragEnter, _
                        Sam274.DragEnter, Sam273.DragEnter, Sam272.DragEnter, Sam271.DragEnter, Sam270.DragEnter, Sam269.DragEnter, _
                        Sam268.DragEnter, Sam267.DragEnter, Sam266.DragEnter, Sam265.DragEnter, Sam264.DragEnter, Sam263.DragEnter, _
                        Sam262.DragEnter, Sam261.DragEnter, SamplesTab.DragEnter, Sam290.DragEnter, Sam289.DragEnter, Sam288.DragEnter, _
                        Sam287.DragEnter, Sam286.DragEnter, Sam285.DragEnter, Sam284.DragEnter, Sam283.DragEnter, Sam282.DragEnter, _
                        Sam281.DragEnter, Reag19.DragEnter, Reag18.DragEnter, Reag17.DragEnter, Reag16.DragEnter, Reag15.DragEnter, _
                        Reag119.DragEnter, Reag118.DragEnter, Reag117.DragEnter, Reag116.DragEnter, Reag115.DragEnter, Reag114.DragEnter, _
                        Reag113.DragEnter, Reag112.DragEnter, Reag111.DragEnter, Reag110.DragEnter, Reag130.DragEnter, Reag129.DragEnter, _
                        Reag128.DragEnter, Reag127.DragEnter, Reag126.DragEnter, Reag125.DragEnter, Reag124.DragEnter, Reag123.DragEnter, _
                        Reag122.DragEnter, Reag121.DragEnter, Reag120.DragEnter, Reag140.DragEnter, Reag139.DragEnter, Reag138.DragEnter, _
                        Reag137.DragEnter, Reag136.DragEnter, Reag135.DragEnter, Reag134.DragEnter, Reag133.DragEnter, Reag132.DragEnter, _
                        Reag131.DragEnter, Reag144.DragEnter, Reag143.DragEnter, Reag142.DragEnter, Reag141.DragEnter, Reag255.DragEnter, _
                        Reag254.DragEnter, Reag253.DragEnter, Reag252.DragEnter, Reag251.DragEnter, Reag250.DragEnter, Reag249.DragEnter, _
                        Reag248.DragEnter, Reag247.DragEnter, Reag246.DragEnter, Reag245.DragEnter, Reag270.DragEnter, Reag269.DragEnter, _
                        Reag268.DragEnter, Reag267.DragEnter, Reag266.DragEnter, Reag265.DragEnter, Reag264.DragEnter, Reag263.DragEnter, _
                        Reag262.DragEnter, Reag261.DragEnter, Reag260.DragEnter, Reag259.DragEnter, Reag258.DragEnter, Reag257.DragEnter, _
                        Reag256.DragEnter, Reag280.DragEnter, Reag279.DragEnter, Reag278.DragEnter, Reag277.DragEnter, Reag276.DragEnter, _
                        Reag275.DragEnter, Reag274.DragEnter, Reag273.DragEnter, Reag272.DragEnter, Reag271.DragEnter, Reag283.DragEnter, _
                        Reag282.DragEnter, Reag281.DragEnter

        Try
            'TR 01/02/2010 -Add the validation for bitmap and text, for the cursor image not to change.
            If e.Data.GetDataPresent(DataFormats.Text) OrElse e.Data.GetDataPresent(DataFormats.Bitmap) Then
                e.Effect = DragDropEffects.Copy
            Else
                e.Effect = DragDropEffects.None
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PositionControl_DragEnter ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PositionControl_DragEnter", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Event for Drag and Drop from one to Position to another in the current Rotor
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by:  TR 21/12/2009
    ''' Modified by: SA 22/12/2009 - Calls to MessageBox.Show were replaced by calls to the generic function 
    '''                              ShowMessage. Added the call to method that write in the Application Log.
    '''              TR 04/01/2010 - Validate if the position is free before changing position
    '''              AG 05/10/2010 - Changed picture box for BSRImage
    ''' </remarks>
    Private Sub PositionControl_DragDrop(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) _
        Handles Sam19.DragDrop, Sam18.DragDrop, Sam17.DragDrop, Sam16.DragDrop, Sam15.DragDrop, Sam145.DragDrop, Sam144.DragDrop, _
        Sam143.DragDrop, Sam142.DragDrop, Sam141.DragDrop, Sam140.DragDrop, Sam14.DragDrop, Sam139.DragDrop, Sam138.DragDrop, _
        Sam137.DragDrop, Sam136.DragDrop, Sam135.DragDrop, Sam134.DragDrop, Sam133.DragDrop, Sam132.DragDrop, Sam131.DragDrop, _
        Sam130.DragDrop, Sam13.DragDrop, Sam129.DragDrop, Sam128.DragDrop, Sam127.DragDrop, Sam126.DragDrop, Sam125.DragDrop, _
        Sam124.DragDrop, Sam123.DragDrop, Sam122.DragDrop, Sam121.DragDrop, Sam120.DragDrop, Sam12.DragDrop, Sam119.DragDrop, _
        Sam118.DragDrop, Sam117.DragDrop, Sam116.DragDrop, Sam115.DragDrop, Sam114.DragDrop, Sam113.DragDrop, Sam112.DragDrop, _
        Sam111.DragDrop, Sam110.DragDrop, Sam11.DragDrop, Sam399.DragDrop, Sam398.DragDrop, Sam397.DragDrop, Sam396.DragDrop, _
        Sam395.DragDrop, Sam394.DragDrop, Sam393.DragDrop, Sam392.DragDrop, Sam391.DragDrop, Sam3135.DragDrop, Sam3134.DragDrop, _
        Sam3133.DragDrop, Sam3132.DragDrop, Sam3131.DragDrop, Sam3130.DragDrop, Sam3129.DragDrop, Sam3128.DragDrop, _
        Sam3127.DragDrop, Sam3126.DragDrop, Sam3125.DragDrop, Sam3124.DragDrop, Sam3123.DragDrop, Sam3122.DragDrop, _
        Sam3121.DragDrop, Sam3120.DragDrop, Sam3119.DragDrop, Sam3118.DragDrop, Sam3117.DragDrop, Sam3116.DragDrop, _
        Sam3115.DragDrop, Sam3114.DragDrop, Sam3113.DragDrop, Sam3112.DragDrop, Sam3111.DragDrop, Sam3110.DragDrop, _
        Sam3109.DragDrop, Sam3108.DragDrop, Sam3107.DragDrop, Sam3106.DragDrop, Sam3105.DragDrop, Sam3104.DragDrop, _
        Sam3103.DragDrop, Sam3102.DragDrop, Sam3101.DragDrop, Sam3100.DragDrop, Reag288.DragDrop, Reag287.DragDrop, _
        Reag286.DragDrop, Reag285.DragDrop, Reag284.DragDrop, Reag14.DragDrop, Reag13.DragDrop, Reag12.DragDrop, _
        Reag11.DragDrop, Sam260.DragDrop, Sam259.DragDrop, Sam258.DragDrop, Sam257.DragDrop, Sam256.DragDrop, Sam255.DragDrop, _
        Sam254.DragDrop, Sam253.DragDrop, Sam252.DragDrop, Sam251.DragDrop, Sam250.DragDrop, Sam249.DragDrop, Sam248.DragDrop, _
        Sam247.DragDrop, Sam246.DragDrop, Sam280.DragDrop, Sam279.DragDrop, Sam278.DragDrop, Sam277.DragDrop, Sam276.DragDrop, _
        Sam275.DragDrop, Sam274.DragDrop, Sam273.DragDrop, Sam272.DragDrop, Sam271.DragDrop, Sam270.DragDrop, Sam269.DragDrop, _
        Sam268.DragDrop, Sam267.DragDrop, Sam266.DragDrop, Sam265.DragDrop, Sam264.DragDrop, Sam263.DragDrop, Sam262.DragDrop, _
        Sam261.DragDrop, Sam290.DragDrop, Sam289.DragDrop, Sam288.DragDrop, Sam287.DragDrop, Sam286.DragDrop, Sam285.DragDrop, _
        Sam284.DragDrop, Sam283.DragDrop, Sam282.DragDrop, Sam281.DragDrop, Reag19.DragDrop, Reag18.DragDrop, Reag17.DragDrop, _
        Reag16.DragDrop, Reag15.DragDrop, Reag119.DragDrop, Reag118.DragDrop, Reag117.DragDrop, Reag116.DragDrop, Reag115.DragDrop, _
        Reag114.DragDrop, Reag113.DragDrop, Reag112.DragDrop, Reag111.DragDrop, Reag110.DragDrop, Reag130.DragDrop, _
        Reag129.DragDrop, Reag128.DragDrop, Reag127.DragDrop, Reag126.DragDrop, Reag125.DragDrop, Reag124.DragDrop, _
        Reag123.DragDrop, Reag122.DragDrop, Reag121.DragDrop, Reag120.DragDrop, Reag140.DragDrop, Reag139.DragDrop, _
        Reag138.DragDrop, Reag137.DragDrop, Reag136.DragDrop, Reag135.DragDrop, Reag134.DragDrop, Reag133.DragDrop, _
        Reag132.DragDrop, Reag131.DragDrop, Reag144.DragDrop, Reag143.DragDrop, Reag142.DragDrop, Reag141.DragDrop, _
        Reag255.DragDrop, Reag254.DragDrop, Reag253.DragDrop, Reag252.DragDrop, Reag251.DragDrop, Reag250.DragDrop, _
        Reag249.DragDrop, Reag248.DragDrop, Reag247.DragDrop, Reag246.DragDrop, Reag245.DragDrop, Reag270.DragDrop, _
        Reag269.DragDrop, Reag268.DragDrop, Reag267.DragDrop, Reag266.DragDrop, Reag265.DragDrop, Reag264.DragDrop, _
        Reag263.DragDrop, Reag262.DragDrop, Reag261.DragDrop, Reag260.DragDrop, Reag259.DragDrop, Reag258.DragDrop, _
        Reag257.DragDrop, Reag256.DragDrop, Reag280.DragDrop, Reag279.DragDrop, Reag278.DragDrop, Reag277.DragDrop, _
        Reag276.DragDrop, Reag275.DragDrop, Reag274.DragDrop, Reag273.DragDrop, Reag272.DragDrop, Reag271.DragDrop, _
        Reag283.DragDrop, Reag282.DragDrop, Reag281.DragDrop

        Try
            'If sender.GetType().Name = "BSRImage" Then 'AG 05/10/2010 - Change all positions (samples and reagents) from PictureBox to BSRImage
            If TypeOf sender Is BSRImage Then 'RH 14/02/2011 Don't rely on changeable text strings, but in class type

                If isRotorDragging Then
                    Dim myPictureBox As BSRImage = CType(sender, BSRImage)
                    If Not myPictureBox.Tag Is Nothing Then
                        'Get the new Ring and Cell values (from the Position where the Drop was done)
                        Dim myRingNumber As Integer = CType(myPictureBox.Tag.ToString().Split(",")(0), Integer)
                        Dim myCellNumber As Integer = CType(myPictureBox.Tag.ToString().Split(",")(1), Integer)

                        Dim myNewCellBarCodeStatus As String = "" 'AG 06/10/2011 - using barcode there are new situations:
                        '                                                   - drag (from rotor) one bottle into another with BarcodeStatus ERROR (status FREE) (no changes are required)
                        ' (CANCELED AG 07/10/2011 - Has some utility??      - drag (from rotor) one bottle (INUSE) into another with BarcodeStatus UNKNOWN (status NO_INUSE) (no changes are required)
                        ' (CANCELED AG 07/10/2011 - Has some utility??      - drag (from rotor) one bottle (NO INUSE) into another with BarcodeStatus UNKNOWN (status NO_INUSE)
                        '                                                          (changes are required due new position can be added into no in use element due it already exists. Update is needed)
                        Dim myNewCellBarCodeInfo As String = ""

                        'Validate if position is free.
                        If IsPositionFree(myRingNumber, myCellNumber, myNewCellBarCodeStatus, myNewCellBarCodeInfo) Then

                            If myNewCellBarCodeStatus <> "UNKNOWN" Then 'AG 07/10/2011 - cancel drag & drop from rotor to rotor into a position with UNKNOWN bottle
                                'Call method to execute the Position change
                                ChangeElementPosition(myRingNumber, myCellNumber, myNewCellBarCodeStatus, myNewCellBarCodeInfo)
                            End If
                        End If
                    End If
                    isRotorDragging = False
                End If

            End If 'AG 05/10/2010

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PositionControl_DragDrop ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PositionControl_DragDrop", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    ''' <summary>
    ''' Keeps track of the current selected tab
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 28/04/2011
    ''' </remarks>
    Private Sub RotorsTabs_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RotorsTabs.SelectedPageChanged
        Try
            If (IsDisposed) Then Exit Sub 'IT 03/06/2014 - #1644 No refresh if screen is disposed

            If String.Equals(RotorsTabs.SelectedTabPage.Name, "SamplesTab") Then
                myRotorTypeForm = "SAMPLES"
            Else
                myRotorTypeForm = "REAGENTS"
            End If
            bsPrintButton.Visible = (myRotorTypeForm = "REAGENTS") 'JV 21/11/2013 #1382
            ValidateScanningButtonEnabled() 'AG 03/04/2012
            ValidateCheckRotorVolumeButtonEnabled() 'AG 28/03/2012 - functionality disabled in v1

            UpdateRotorPositionedButtonsStatus()

            ' XB 06/03/2014 - Add Try Catch section
        Catch ex As Exception
            'Write error in the Application Log and show it  
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".RotorsTabs_SelectedIndexChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RotorsTabs_SelectedIndexChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Updates the Enable property of the buttons that depend on the Rotor Positioned Elements existence
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 18/02/2011
    ''' Modified AG 13/07/2011 - the reset rotor button can not be enabled when isResetRotorAllowed = False. In this case do not change his Enabled property
    ''' </remarks>
    Private Sub UpdateRotorPositionedButtonsStatus()
        If isResetRotorAllowed Then
            Dim Enabled As Boolean = (RotorPositionedElements(myRotorTypeForm) <> 0)
            bsSaveVRotorButton.Enabled = Enabled


            'AG 13/07/2011
            bsResetRotorButton.Enabled = Enabled

            'If isResetRotorAllowed Then
            ' bsResetRotorButton.Enabled = Enabled
            ' End If
        End If

    End Sub

    ''' <summary>
    ''' Show the Drag cursor over the form
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by:  TR 17/12/2009
    ''' Tested: OK
    ''' Modified by: SA 22/12/2009 - Calls to MessageBox.Show were replaced by calls to the generic function 
    '''                              ShowMessage.  
    ''' </remarks>
    Private Sub ReagentSamplePositioning_DragOver(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles MyBase.DragOver
        Try
            'Check to see if the drag event contains a file data type,
            'i.e., that we are dragging a file. Ignore all other draggs
            If e.Data.GetDataPresent(DataFormats.Text) Then
                'Change the feedback (the effect) to show that this drag is allowable
                If e.AllowedEffect AndAlso DragDropEffects.Copy Then
                    e.Effect = DragDropEffects.Copy
                End If
            End If
        Catch ex As Exception
            'Write error in the Application Log and show it  
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ReagentSamplePositioning_DragOver ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReagentSamplePositioning_DragOver", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    ''' <summary>
    ''' Event for click in button of Save Virtual Rotor
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsSaveVRotorButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSaveVRotorButton.Click
        ' XB 25/03/2014 - Memory leaks - Improve Dispose (SplitContainer disallows dispose memory)
        'If SplitContainer1.Panel2Collapsed Then Return 'RH 21/02/2011 Do nothing if we have the tree with the extended view
        SaveVirtualRotor(myRotorTypeForm)
    End Sub

    ''' <summary>
    ''' Event for click in button of Load Virtual Rotor
    ''' </summary>
    ''' <remarks>
    ''' Created by:  VR
    ''' modified: AG 08/01/2010 (Testing OK)
    ''' </remarks>
    Private Sub bsLoadVRotorButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsLoadVRotorButton.Click
        ' XB 25/03/2014 - Memory leaks - Improve Dispose (SplitContainer disallows dispose memory)
        'If SplitContainer1.Panel2Collapsed Then Return 'RH 21/02/2011 Do nothing if we have the tree with the extended view
        LoadVirtualRotor(myRotorTypeForm)   '08/01/2010 AG
    End Sub

    ''' <summary>
    ''' Event for click in button to shown content of the first position (External Ring/First Cell) of the current Rotor
    ''' Handles Click Event of buttons bsSamplesMoveFirstPositionButton and bsReagentsMoveFirstPositionButton
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by:  SA 23/12/2009
    ''' </remarks>
    Private Sub bsMoveFirstPositionButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSamplesMoveFirstPositionButton.Click, _
                                                                                                                    bsReagentsMoveFirstPositionButton.Click
        ScrollButtonsInfoArea(1)
    End Sub

    ''' <summary>
    ''' Event for click in button to shown content of the previous position (regarding the one currently selected) of the current Rotor
    ''' Handles Click Event of buttons bsSamplesDecreaseButton and bsReagentsDecreaseButton
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by:  SA 23/12/2009
    ''' </remarks>
    Private Sub bsDecreaseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSamplesDecreaseButton.Click, _
                                                                                                           bsReagentsDecreaseButton.Click
        ScrollButtonsInfoArea(2)
    End Sub

    ''' <summary>
    ''' Event for click in button to shown content of the next position (regarding the one currently selected) of the current Rotor
    ''' Handles Click Event of buttons bsSamplesIncreaseButton and bsReagentsIncreaseButton
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by:  SA 23/12/2009
    ''' </remarks>
    Private Sub bsIncreaseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSamplesIncreaseButton.Click, _
                                                                                                           bsReagentsIncreaseButton.Click
        ScrollButtonsInfoArea(3)
    End Sub

    ''' <summary>
    ''' Event for click in button to shown content of the last position (More Internal Ring/Last Cell) of the current Rotor
    ''' Handles Click Event of buttons bsSamplesMoveLastPositionButton and bsReagentsMoveLastPositionButton
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by:  SA 23/12/2009
    ''' </remarks>
    Private Sub bsMoveLastPositionButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSamplesMoveLastPositionButton.Click, _
                                                                                                                   bsReagentsMoveLastPositionButton.Click
        ScrollButtonsInfoArea(4)
    End Sub

    ''' <summary>
    ''' Event for click in button to refill the Tube/Bottle placed in the selected Rotor position
    ''' Handles Click Event of buttons bsSamplesRefillPosButton and bsReagentsRefillPosButton
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by:  SA 23/12/2009
    ''' Modifed by : VR 30/12/2009 - (Tested : Pending)
    ''' Modifed by : VR 31/12/2009 - (Tested : OK) - Icons definitions are pending
    ''' Integrated AG 05/01/2010    
    ''' </remarks>
    Private Sub bsRefillPosButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSamplesRefillPosButton.Click, _
                                                                                                            bsReagentsRefillPosButton.Click
        RefillReagentSampleVolume(myRotorContentByPositionDSForm)
        myRotorContentByPositionDSForm.twksWSRotorContentByPosition.AcceptChanges()

    End Sub

    ''' <summary>
    ''' Event for click in button to check the current volume of the selected Rotor position
    ''' Handles Click Event of buttons bsSamplesCheckVolumePosButton and bsReagentsCheckVolumePosButton
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by:  SA 23/12/2009
    ''' </remarks>
    Private Sub bsCheckVolumePosButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
                                                                                                                 bsReagentsCheckVolumePosButton.Click
        'TODO: pending add the function that have to be call here
    End Sub

    ''' <summary>
    ''' Event for click in button to download the Tube/Bottle placed in the selected Rotor position
    ''' Handles Click Event of buttons bsSamplesDeletePosButton and bsReagentsDeletePosButton
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by:  SA 23/12/2009
    ''' Modificated by: AG 11/01/2010 - Add functionality to this events
    ''' </remarks>
    Private Sub bsDeletePosButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSamplesDeletePosButton.Click, _
                                                                                                            bsReagentsDeletePosButton.Click
        '11/01/2010 AG
        If Not (mySelectedElementInfo Is Nothing) Then
            If mySelectedElementInfo.twksWSRotorContentByPosition.Rows.Count > 0 Then
                If mySelectedElementInfo.twksWSRotorContentByPosition(0).Selected Then
                    DeleteSelectedPositions(mySelectedElementInfo)

                    'DL 05/10/2011
                    If myRotorTypeForm = "SAMPLES" Then
                        bsSamplesDeletePosButton.Enabled = False
                    ElseIf myRotorTypeForm = "REAGENTS" Then
                        bsReagentsDeletePosButton.Enabled = False
                    End If

                    'DL 05/10/2011
                    'TR 11/01/2010 -clear the selection
                    'ClearSelection() RH 09/09/2011 DeleteSelectedPositions() already includes ClearSelection()
                End If
            End If
        End If
        'END 11/01/2010 AG

    End Sub

    ''' <summary>
    ''' EVENT for Delete the Selected Element in the ROTOR using Delete keyboard Button Pressed
    ''' </summary>
    ''' <param name="sender">Sender</param>
    ''' <param name="e">Event</param>
    ''' <remarks>
    ''' Created by:  VR 07/01/2010 (TESTED : OK) Comments - The Rotor Area (Deleted Element) is refereshed only after reload the form again.
    ''' Modified by: TR 13/01/2010 - Validate if the barcode textbox are not focused 
    '''                            - Clear the selection mark after delete.
    '''              AG 18/01/2010 - add the LoadScreenStatus functionality (use the isSingleDeletePosAllowed and isResetRotorAllowed variables)
    '''              XB 11/03/2014 - Add additional protection exiting screen by ESC key because Obj Ref errors (This occurs pressing several ESC's when WorkSessionStatusAttribute is [EMPTY]) - #1523
    ''' </remarks>
    Private Sub ReagentSamplePositioning_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Try
            'Validate none of the barcode textboxes are focus
            If Not bsSamplesBarcodeTextBox.Focused AndAlso Not bsReagentsBarCodeTextBox.Focused Then
                Select Case e.KeyCode
                    Case Keys.Delete 'if the pressed key is the Delete then start.

                        'If SplitContainer1.Panel2Collapsed Then Return 'RH 21/02/2011 Do nothing if we have the tree with the extended view DL 05/10/2011
                        If Not mySelectedElementInfo Is Nothing AndAlso mySelectedElementInfo.twksWSRotorContentByPosition.Rows.Count > 0 Then
                            If mySelectedElementInfo.twksWSRotorContentByPosition(0).Selected Then 'AndAlso isSingleDeletePosAllowed = True Then

                                'dl 05/10/2011
                                If myRotorTypeForm = "SAMPLES" Then
                                    If bsSamplesDeletePosButton.Enabled Then DeleteSelectedPositions(mySelectedElementInfo)
                                ElseIf myRotorTypeForm = "REAGENTS" Then
                                    If bsReagentsDeletePosButton.Enabled Then DeleteSelectedPositions(mySelectedElementInfo)
                                End If
                                'dl 05/10/2011

                                'Clear the mark (selected) position 
                                'ClearSelection() RH 09/09/2011 DeleteSelectedPositions() already includes ClearSelection()
                            End If
                        ElseIf isResetRotorAllowed = True Then
                            ResetRotor(myRotorTypeForm, AnalyzerIDAttribute)
                            'VisiblePositionsControls(True) 'RH 15/02/2011 Not needed
                        End If

                        ' dl 09/11/2010
                    Case Keys.Escape

                        ' XB 11/03/2014 - #1523
                        If Not ESCKeyPressed Then
                            ESCKeyPressed = True
                            bsCreateExecutionsButton_Click(Nothing, Nothing)
                            ESCKeyPressed = False
                        End If
                        ' XB 11/03/2014 - #1523

                End Select
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ReagentSamplePositioning_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReagentSamplePositioning_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Activate when the user click on the rotor area image
    ''' clear the selecte control and the select element.
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 08/01/2010
    ''' </remarks>
    Private Sub SampleReagentRotorPic_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SamplesTab.Click, ReagentsTab.Click
        ClearSelection()
    End Sub

    ''' <summary>
    ''' Event activated only when the user select an intem on the BottleSizeComboBox
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 14/01/2010 - Save the new selected bottle/tube size
    ''' Modified by: TR 25/01/2010 - Added a For/Next loop and other changes needed to allow this functionality when several Rotor Positions have been selected 
    '''              RH 05/08/2011 - Update fields TubeType and Status for each Rotor Position in the list of selected Rotor Positions
    '''              RH 13/09/2011 - Update all Barcode fields for each Rotor Position in the list of selected Rotor Positions
    '''              TR 03/04/2012 - Mark the Position as selected in the Rotor, and update fields in Info Area frame for it
    '''              TR 01/08/2012 - Set to Nothing of used Lists before leave the function
    '''              SA 04/11/2013 - BT #1371 ==> Exclude selected FREE Rotor Positions for the process of Bottle/Tube size change. Remove update of fields in Info Area
    '''                              frame when several positions are selected, due to in this case, fields in this area should be empty. 
    '''              TR 08/11/2013 - BT#1358 And BT#1358(2)==>Exclude the Elements that not allow the action (Bottle size change) on Pause Mode.
    ''' </remarks>
    Private Sub bsBottleSizeChange_SelectionChangeCommitted(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsBottleSizeComboBox.SelectionChangeCommitted, bsTubeSizeComboBox.SelectionChangeCommitted
        Try
            Dim myTubeBottleCombo As BSComboBox
            myTubeBottleCombo = CType(sender, BSComboBox)

            If (Not myTubeBottleCombo.SelectedValue Is Nothing AndAlso myTubeBottleCombo.SelectedValue.ToString() <> String.Empty) Then
                'Validate that there is at least a Rotor Position selected in the Rotor
                If (Not mySelectedElementInfo Is Nothing AndAlso mySelectedElementInfo.twksWSRotorContentByPosition.Rows.Count > 0) Then

                    'TR 11/11/2013 BT#1358 
                    ShowActionMessageifApply()
                    'TR 11/11/2013 BT#1358-END

                    Dim selectedElements As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                    For Each selectedElementRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In mySelectedElementInfo.twksWSRotorContentByPosition.Rows
                        If (selectedElementRow.Status <> "FREE" AndAlso selectedElementRow.AllowedActionInPause) Then
                            'Change the Bottle/Tube size, indicating the new Bottle/Tube size selected on the ComboBox
                            ChangeBottleSize(selectedElementRow.RingNumber, selectedElementRow.CellNumber, myTubeBottleCombo.SelectedValue.ToString())

                            'Get the information of the Rotor Position from the DS that has been updated for the previous method 
                            selectedElements = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition.AsEnumerable _
                                               Where a.Selected = True _
                                             AndAlso a.RingNumber = selectedElementRow.RingNumber _
                                             AndAlso a.CellNumber = selectedElementRow.CellNumber _
                                             AndAlso a.RotorType = myRotorTypeForm _
                                              Select a).ToList()

                            If (selectedElements.Count > 0) Then
                                'Update fields TubeType and Status also in the DS of Selected Positions
                                selectedElementRow.TubeType = selectedElements.First().TubeType
                                selectedElementRow.Status = selectedElements.First().Status

                                'Update all Barcode fields also in the DS of Selected Positions
                                selectedElementRow.BarCodeInfo = selectedElements.First().BarCodeInfo
                                selectedElementRow.BarcodeStatus = selectedElements.First().BarcodeStatus
                                If (selectedElements.First().IsScannedPositionNull) Then
                                    selectedElementRow.SetScannedPositionNull()
                                Else
                                    selectedElementRow.ScannedPosition = selectedElements.First().ScannedPosition
                                End If

                                'Update field ElementID also in the DS of Selected Positions
                                If (selectedElements.First().IsElementIDNull) Then
                                    selectedElementRow.SetElementIDNull()
                                Else
                                    selectedElementRow.ElementID = selectedElements.First().ElementID
                                End If

                                'Only when just one Rotor Position is selected, update fields in Info Area frame
                                If (mySelectedElementInfo.twksWSRotorContentByPosition.Rows.Count = 1) Then
                                    If (myRotorTypeForm = "REAGENTS") Then
                                        bsReagentsBarCodeTextBox.Text = selectedElementRow.BarCodeInfo
                                    Else
                                        bsSamplesBarcodeTextBox.Text = selectedElementRow.BarCodeInfo
                                    End If

                                    MarkSelectedPosition(selectedElementRow.RingNumber, selectedElementRow.CellNumber, selectedElementRow.Selected)
                                    ShowPositionInfoArea(AnalyzerIDAttribute, myRotorTypeForm, selectedElementRow.RingNumber, selectedElementRow.CellNumber)
                                Else
                                    MarkSelectedPosition(selectedElementRow.RingNumber, selectedElementRow.CellNumber, selectedElementRow.Selected)
                                End If
                            End If
                        End If
                    Next
                    selectedElements = Nothing
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsBottleSizeComboBox_SelectedValueChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name, GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Event for click in button of Reset Rotor
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR
    ''' Modified by: TR 07/01/2010 - Show position controls
    ''' </remarks>
    Private Sub bsResetRotorButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsResetRotorButton.Click
        'If SplitContainer1.Panel2Collapsed Then Return 'RH 21/02/2011 Do nothing if we have the tree with the extended view DL 05/10/2011
        ResetRotor(myRotorTypeForm, AnalyzerIDAttribute)
        'VisiblePositionsControls(True) 'RH 15/02/2011 Not needed
    End Sub

    ''' <summary>
    ''' Avoid focus on read only fields in the Info Area for Samples Rotor or Reagents Rotor
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 15/07/2010
    ''' </remarks>
    Private Sub bsReadOnlyTextBox_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsSampleRingNumTextBox.GotFocus, bsSampleCellTextBox.GotFocus, _
                                                                                                        bsSampleContentTextBox.GotFocus, bsSampleNumberTextBox.GotFocus, _
                                                                                                        bsSampleIDTextBox.GotFocus, bsSampleTypeTextBox.GotFocus, _
                                                                                                        bsDiluteStatusTextBox.GotFocus, SamplesStatusTextBox.GotFocus, _
                                                                                                        bsReagentsRingNumTextBox.GotFocus, bsReagentsContentTextBox.GotFocus, _
                                                                                                        bsReagentsNumberTextBox.GotFocus, bsReagentNameTextBox.GotFocus, _
                                                                                                        bsTestNameTextBox.GotFocus, bsExpirationDateTextBox.GotFocus, _
                                                                                                        bsCurrentVolTextBox.GotFocus, bsTeststLeftTextBox.GotFocus, ReagStatusTextBox.GotFocus
        Try
            If (myRotorTypeForm = "SAMPLES") Then
                'Put Focus in the ComboBox of Tube Sizes
                bsTubeSizeComboBox.Focus()
            ElseIf (myRotorTypeForm = "REAGENTS") Then
                'Put Focus on the ComboBox of Bottle Sizes
                bsBottleSizeComboBox.Focus()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsReadOnlyTextBox_GotFocus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsReadOnlyTextBox_GotFocus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Verify if there are Elements pending to be positioned and in this case, open the warning screen
    ''' of not positioned Elements
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 28/07/2010
    ''' </remarks>
    Private Sub bsWarningsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsWarningsButton.Click
        Try
            ShowNotPositionedElements()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsWarningsButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsWarningsButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsElementsTreeView_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles bsElementsTreeView.AfterSelect
        ShowInfoSelectedTreeNodeElement() 'TR 22/11/2010
    End Sub

    ''' <summary>
    ''' Activated when the user click on the Close button (CreateExecutionsButton).
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 06/08/2010
    ''' Modified by: RH 09/02/2011
    '''              SA 12/03/2012 - If the status of the active WS is EMPTY, OPEN or ABORTED, just close the screen
    '''              AG 10/05/2012 - Move the code to an independent method defines as Public (it could be called from MDI in some cases)
    '''              RH 16/05/2012 - Return the visibility of the method to Private.
    '''              SA 09/10/2013 - BT #1323 ==>Before opening Monitor Screen, inform the MainMDI property for the WS Status with value of the WS Status 
    '''                              attribute of this screen, which was updated during Create Executions Process. 
    '''              AG 24/02/2014 - BT #1520 ==> Use parameters MAX_APP_MEMORYUSAGE and MAX_SQL_MEMORYUSAGE into performance counters (a warning 
    '''                                           message is NOT shown if at least one of them has been exceeded)
    ''' </remarks>
    Private Sub bsCreateExecutionsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAcceptButton.Click
        Dim createExecutionsFlag As Boolean = True

        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        Dim StartTime As DateTime = Now
        Dim myLogAcciones As New ApplicationLogManager()
        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        Try
            bsAcceptButton.Enabled = False 'AG 18/06/2014 #1669 - Disable button when this process starts

            'AG 28/05/2014 - New trace
            myLogAcciones.CreateLogActivity("Start Closing IWSROTORPositions", "IWSROTORPositions.bsCreateExecutionsButton_Click", EventLogEntryType.Information, False)

            'BT #1520 - Use parameters MAX_APP_MEMORYUSAGE and MAX_SQL_MEMORYUSAGE into performance counters (a warning message is NOT shown 
            '           if at least one of them has been exceeded)
            Dim pCounters As New AXPerformanceCounters(applicationMaxMemoryUsage, SQLMaxMemoryUsage)
            pCounters.GetAllCounters()
            pCounters = Nothing

            Dim myGlobal As GlobalDataTO
            myGlobal = CreateExecutionsProcess(True, createExecutionsFlag)

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            'AG 28/05/2014 - change text  'AG 18/02/2014 - #1505 - Change text!!!
            myLogAcciones.CreateLogActivity("IWSROTORPositions.CreateExecutionsProcess (Complete1): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0) & " - NOTE: High values could mean screen with element NOPOS opened (search for 'Time with NOSPOS warning screen opened')!!", _
                                            "IWSROTORPositions.bsCreateExecutionsButton_Click", EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCreateExecutionsButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "bsCreateExecutionsButton_Click()", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        Finally
            Me.Cursor = Cursors.Default
            If (createExecutionsFlag) Then
                'TR 30/08/2012 Set the is closing  to true to avoid refresh screen executions
                isClosingFlag = True
                IAx00MainMDI.StopMarqueeProgressBar()

                'ReleaseElement() 'AG 09/07/2013
                'RH 09/02/2011
                If (Not Me.Tag Is Nothing) Then
                    'A PerformClick() method was executed
                    Me.Close()
                Else
                    'Normal button click
                    'Open the WS Monitor form and close this one
                    IMonitor.WorkSessionChange = True
                    Dim autoProcessFlag As Boolean = False
                    If AutoWSCreationWithLISModeAttribute AndAlso OpenByAutomaticProcessAttribute Then
                        autoProcessFlag = True
                    End If

                    IAx00MainMDI.ActiveStatus = WorkSessionStatusAttribute
                    IAx00MainMDI.OpenMonitorForm(Me, autoProcessFlag)
                End If
            Else
                bsAcceptButton.Enabled = True 'AG 18/06/2014 #1669 - Enable button again if the screen is not closed!!
            End If

            'IAx00MainMDI.EnableButtonAndMenus(True) 'AG 07/11/2012 - Avoid blinking in START WS button when rotor position screen is closed

            'BT #1520 - Use parameters MAX_APP_MEMORYUSAGE and MAX_SQL_MEMORYUSAGE into performance counters (a warning message is NOT shown 
            '           if at least one of them has been exceeded)
            Dim pCounters As New AXPerformanceCounters(applicationMaxMemoryUsage, SQLMaxMemoryUsage)
            pCounters.GetAllCounters()
            pCounters = Nothing

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            'AG 28/05/2014 - change text 'AG 18/02/2014 - #1505 - Change text!!!
            myLogAcciones.CreateLogActivity("IWSROTORPositions CLOSED (final): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0) & " - NOTE: High values could mean screen with element NOPOS opened (search for 'Time with NOSPOS warning screen opened')!!", _
                                            "IWSROTORPositions.bsCreateExecutionsButton_Click", EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        End Try
    End Sub

    ''' <summary>
    ''' Define event but leave business empty
    ''' Protection for some users with quick clicks
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>AG + XB 19/06/2014 #1669</remarks>
    Private Sub bsAcceptButton_DoubleClick(sender As Object, e As EventArgs) Handles bsAcceptButton.DoubleClick
        'AG + XB 19/06/2014 #1669
    End Sub


    ''' <summary>
    ''' Edits Sample BarCode value
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 08/09/2011
    ''' Modified by: AG 03/10/2011 - When TubeType is not informed for the selected Position (FREE cell), inform property with value of Standard Tube code (T13)
    '''              AG 05/10/2011 - Enable BarcodeWarningButton when it was disabled and the Status of the updated Rotor Position is NO_INUSE
    '''              AG 10/10/2011 - Allow manual BarCode only in FREE Positions or in Positions with Patient Samples
    '''              TR 04/04/2012 - If an Element was positioned in the Rotor Position which Barcode was changed, validate if it is still positioned 
    '''                              and in the opposite case, change the ElementStatus in the TreeView of WS Required Elements 
    '''              SA 11/06/2013 - Passed WorkSessionStatusAttribute as parameter when calling function EntryManualBarcode in BarcodeWSDelegate
    '''              SA 04/11/2013 - BT #1371 ==> Changed code to Update Rotor Position image and Info Area fields to execute the same functions than in the 
    '''                              equivalent TextBox used for Reagents (with the previous code the Position was not selected in the Rotor although its information
    '''                              was loaded in Info Area frame)  
    ''' </remarks>
    Private Sub bsSamplesBarcodeTextBox_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles bsSamplesBarcodeTextBox.MouseUp
        Try
            If (Not mySelectedElementInfo Is Nothing AndAlso mySelectedElementInfo.twksWSRotorContentByPosition.Rows.Count > 0) Then
                Dim cellStatus As String = String.Empty
                Dim cellTubeContent As String = String.Empty
                If (Not mySelectedElementInfo.twksWSRotorContentByPosition(0).IsStatusNull) Then cellStatus = mySelectedElementInfo.twksWSRotorContentByPosition(0).Status
                If (Not mySelectedElementInfo.twksWSRotorContentByPosition(0).IsTubeContentNull) Then cellTubeContent = mySelectedElementInfo.twksWSRotorContentByPosition(0).TubeContent

                'Allow manual barcode only in free positions or in positions with patients
                If (cellStatus = "FREE" OrElse cellTubeContent = "PATIENT") Then
                    Using BarCodeForm As New IBarCodeEdit()
                        BarCodeForm.BarCode = bsSamplesBarcodeTextBox.Text
                        BarCodeForm.RotorType = GlobalEnumerates.Rotors.SAMPLES

                        Dim SelectedElement As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow
                        SelectedElement = mySelectedElementInfo.twksWSRotorContentByPosition(0)

                        'When TubeType is not informed for the selected Position (FREE cell), inform property with value of Standard Tube code (T13)
                        Dim myTubeType As String = String.Empty
                        If (SelectedElement.TubeType = "T13" OrElse SelectedElement.TubeType = String.Empty) Then
                            'Standard Tube
                            BarCodeForm.TubeType = GlobalEnumerates.TubeTypes.TUBE
                            myTubeType = GlobalEnumerates.TubeTypes.TUBE.ToString
                        Else
                            'Pediatric Tube
                            BarCodeForm.TubeType = GlobalEnumerates.TubeTypes.PEDIATRIC
                            myTubeType = GlobalEnumerates.TubeTypes.PEDIATRIC.ToString
                        End If

                        If (BarCodeForm.ShowDialog() = DialogResult.OK) Then
                            Dim resultData As GlobalDataTO
                            Dim BarCodeDelegate As BarcodeWSDelegate = New BarcodeWSDelegate()

                            resultData = BarCodeDelegate.EntryManualBarcode(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, "SAMPLES", _
                                                                            SelectedElement.CellNumber, myTubeType, BarCodeForm.BarCode, WorkSessionStatusAttribute)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                mySelectedElementInfo = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                                'If an Element was positioned in the Rotor Position which Barcode was changed, validate if it is still positioned 
                                'and in the opposite case, change the ElementStatus in the TreeView of WS Required Elements 
                                If (mySelectedElementInfo.twksWSRotorContentByPosition.Count > 0) Then
                                    If (mySelectedElementInfo.twksWSRotorContentByPosition(0).IsElementIDNull AndAlso Not SelectedElement.IsElementIDNull) Then
                                        If (Not bsElementsTreeView Is Nothing) Then
                                            ChangeTreeRotorElementsStatus(bsElementsTreeView.Nodes, False, SelectedElement, myRotorTypeForm)
                                        End If
                                    End If
                                    'AG 20/02/2014 - #1516 - Protection against "This row has been removed from a table and does not have any data.  BeginEdit() will allow creation of new data in this row" ... Move End If
                                    '                        in order to include next line
                                    SelectedElement = mySelectedElementInfo.twksWSRotorContentByPosition(0)
                                End If

                                'Enable BarcodeWarningButton when it was disabled and the Status of the updated Rotor Position is NO_INUSE
                                If (Not SelectedElement.IsStatusNull AndAlso SelectedElement.Status = "NO_INUSE" AndAlso Not BarcodeWarningButton.Enabled) Then
                                    BarcodeWarningButton.Enabled = True
                                End If

                                'Update Rotor Position image and Info Area fields 
                                'InitializeScreen(False, "")
                                'MarkSelectedPosition(SelectedElement.RingNumber, SelectedElement.CellNumber, True)
                                'ShowPositionInfoArea(SelectedElement.AnalyzerID, myRotorTypeForm, SelectedElement.RingNumber, SelectedElement.CellNumber)

                                'Update Rotor Position image and Info Area fields 
                                LoadRotorAreaInfo(WorkSessionIDAttribute, AnalyzerIDAttribute, False, String.Empty)
                                MarkSelectedPosition(SelectedElement.RingNumber, SelectedElement.CellNumber, False)
                                ShowPositionInfoArea(SelectedElement.AnalyzerID, myRotorTypeForm, SelectedElement.RingNumber, SelectedElement.CellNumber)
                                SetInfoAreaEnabledStatus(myRotorTypeForm, True)
                                MarkSelectedPosition(SelectedElement.RingNumber, SelectedElement.CellNumber, True)
                            End If
                        End If
                    End Using
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsSamplesBarcodeTextBox_MouseUp", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'ShowMessage(Me.Name & "bsSamplesBarcodeTextBox_MouseUp()", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me) 'AG 20/02/2014 - #1516 EUA (in India) reports this error sometimes triggers, but we cannot reproduce and this is not a critical error -> SOLUTION AG+EF do not show error message in this method!!
        End Try
    End Sub

    ''' <summary>
    ''' Edits Reagents BarCode value
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 08/09/2011
    ''' Modified by: TR 13/02/2012 - Added the Try/Catch and validate if mySelectedElementInfo is Nothing before execute the function code
    '''                              on the if statemen
    '''              TR 13/03/2012 - Inform property AnalyzerID before open the auxiliary screen for Barcode Edition
    '''              TR 23/03/2012 - Set focus to Reagents Tab before leave the function
    '''              TR 04/04/2012 - If an Element was positioned in the Rotor Position which Barcode was changed, validate if it is still positioned 
    '''                              and in the opposite case, change the ElementStatus in the TreeView of WS Required Elements 
    '''              SA 11/06/2013 - Passed WorkSessionStatusAttribute as parameter when calling function EntryManualBarcode in BarcodeWSDelegate (needed to update 
    '''                              the WS Status from OPEN to PENDING when the WS Required Elements are created after scanning the Samples Rotor -> new 
    '''                              functionality added for LIS with ES)
    '''              SA 04/11/2013 - BT #1371 ==> After calling function ShowPositionInfoArea, call function SetInfoAreaEnabledStatus to avoid availability of 
    '''                              BottleSize ComboBox for Rotor Positions with Barcode
    ''' </remarks>
    Private Sub bsReagentsBarCodeTextBox_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles bsReagentsBarCodeTextBox.MouseUp
        Try
            If (Not mySelectedElementInfo Is Nothing AndAlso mySelectedElementInfo.twksWSRotorContentByPosition.Rows.Count > 0) Then
                Using BarCodeForm As New IBarCodeEdit()
                    BarCodeForm.AnalyzerID = AnalyzerIDAttribute
                    BarCodeForm.BarCode = bsReagentsBarCodeTextBox.Text
                    BarCodeForm.RotorType = GlobalEnumerates.Rotors.REAGENTS

                    Dim SelectedElement As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow
                    SelectedElement = mySelectedElementInfo.twksWSRotorContentByPosition(0)

                    If (BarCodeForm.ShowDialog() = DialogResult.OK) Then
                        Dim resultData As GlobalDataTO
                        Dim BarCodeDelegate As BarcodeWSDelegate = New BarcodeWSDelegate()

                        resultData = BarCodeDelegate.EntryManualBarcode(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, "REAGENTS", SelectedElement.CellNumber, _
                                                                        SelectedElement.TubeType, BarCodeForm.BarCode, WorkSessionStatusAttribute)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            mySelectedElementInfo = DirectCast(resultData.SetDatos, WSRotorContentByPositionDS)

                            'If an Element was positioned in the Rotor Position which Barcode was changed, validate if it is still positioned and in the opposite case,
                            'change the ElementStatus in the TreeView of WS Required Elements
                            If (mySelectedElementInfo.twksWSRotorContentByPosition.Count > 0) Then
                                If (mySelectedElementInfo.twksWSRotorContentByPosition(0).IsElementIDNull AndAlso Not SelectedElement.IsElementIDNull) Then
                                    If (Not bsElementsTreeView Is Nothing) Then
                                        ChangeTreeRotorElementsStatus(bsElementsTreeView.Nodes, False, SelectedElement, myRotorTypeForm)
                                    End If
                                End If
                                'AG 20/02/2014 - #1516 - Protection against "This row has been removed from a table and does not have any data.  BeginEdit() will allow creation of new data in this row" ... Move End If
                                '                        in order to include next line
                                SelectedElement = mySelectedElementInfo.twksWSRotorContentByPosition(0)
                            End If

                            'Update Rotor Position image and Info Area fields 
                            LoadRotorAreaInfo(WorkSessionIDAttribute, AnalyzerIDAttribute, False, String.Empty)
                            MarkSelectedPosition(SelectedElement.RingNumber, SelectedElement.CellNumber, False)
                            ShowPositionInfoArea(SelectedElement.AnalyzerID, myRotorTypeForm, SelectedElement.RingNumber, SelectedElement.CellNumber)
                            SetInfoAreaEnabledStatus(myRotorTypeForm, True)
                            MarkSelectedPosition(SelectedElement.RingNumber, SelectedElement.CellNumber, True)
                        End If
                    End If
                End Using

                ReagentsTab.Focus()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsReagentsBarCodeTextBox_MouseUp", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'ShowMessage(Me.Name & "bsReagentsBarCodeTextBox_MouseUp()", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me) 'AG 20/02/2014 - #1516 EUA (in India) reports this error sometimes triggers, but we cannot reproduce and this is not a critical error -> SOLUTION AG+EF do not show error message in this method!!
        End Try
    End Sub

    ''' <summary>
    ''' Depending on value of setting LIS_WITHFILES_MODE, open the screen of Incomplete Patient Samples or the new LIS Host Query Screen
    ''' </summary>
    ''' <remarks>
    ''' Created by: 
    ''' Modified by: JC in v2.0.0 New HQBarCode screen
    '''              AG 03/04/2013 - Open the new HQBarCode screen or the SamplesIncomplete depending on value of setting LIS_WITHFILES_MODE
    '''              JC 04/04/2013 - When must open the new HQBarCode and there are SampleIncomplete selected, open HQBarCode with this samples selected
    '''              SA 24/07/2013 - Changed the linq to get the list of selected cells: get only selected Cells containing Patient Samples; exclude free
    '''                              cells and also cells containing Controls, Calibrators and/or Additional Solutions
    '''              SA 25/07/2013 - When the Host Query Screen is closed, if the User clicked in HQ Button, the process of LIS Orders Download
    '''                              is executed
    '''              AG 07/01/2014 - BT #1436 protection, if recover results INPROCESS do not open the HQ barcode neither the incomplete samples screens
    ''' </remarks>
    Private Sub BarcodeWarningButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BarcodeWarningButton.Click
        Try
            If (ShowHostQueryScreenAttribute) Then ShowHostQueryScreenAttribute = False

            'AG 07/01/2013 - BT #1436 - put all code inside this IF
            If AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess) <> "INPROCESS" Then '#REFACTORING

                Dim createAutoWS As Boolean = False
                Dim lisWithFilesMode As Boolean = IsLisWithFilesMode()
                If (lisWithFilesMode) Then
                    Using myForm As New IWSIncompleteSamplesAuxScreen()
                        myForm.AnalyzerID = AnalyzerIDAttribute
                        myForm.WorkSessionID = WorkSessionIDAttribute
                        myForm.WorkSessionStatus = WorkSessionStatusAttribute
                        myForm.SourceScreen = GlobalEnumerates.SourceScreen.ROTOR_POS

                        myForm.ShowDialog()
                        WorkSessionStatusAttribute = myForm.WorkSessionStatus
                    End Using
                Else
                    Using myForm As New HQBarcode()
                        myForm.AnalyzerID = AnalyzerIDAttribute
                        myForm.WorkSessionID = WorkSessionIDAttribute
                        myForm.WorkSessionStatus = WorkSessionStatusAttribute
                        myForm.SourceScreen = GlobalEnumerates.SourceScreen.ROTOR_POS
                        myForm.applicationMaxMemoryUsage = applicationMaxMemoryUsage  'AG 24/02/2014 - #1520 inform new property

                        If (Not IsNothing(mySelectedElementInfo)) Then
                            myForm.OpenSelectedCells = (From a In mySelectedElementInfo.Tables(0) _
                                                       Where CType(a, WSRotorContentByPositionDS.twksWSRotorContentByPositionRow).Status <> "FREE" _
                                                     AndAlso CType(a, WSRotorContentByPositionDS.twksWSRotorContentByPositionRow).TubeContent = "PATIENT" _
                                                      Select CType(a, WSRotorContentByPositionDS.twksWSRotorContentByPositionRow).CellNumber).ToList()
                        End If

                        'JC-SG 10/05/2013 Screen sometimes appear on bacground of MDIForm, and app looks like has hang out
                        '#If Not Debug Then
                        myForm.TopMost = True
                        '#End If

                        'Inform the MDI the HostQuery monitor screen is shown
                        IAx00MainMDI.AddNoMDIChildForm = myForm
                        myForm.ShowDialog()

                        IAx00MainMDI.RemoveNoMDIChildForm = myForm
                        WorkSessionStatusAttribute = myForm.WorkSessionStatus
                        createAutoWS = myForm.HQButtonUserClick
                    End Using

                    If (createAutoWS) Then
                        IAx00MainMDI.SetAutomateProcessStatusValue(GlobalEnumerates.LISautomateProcessSteps.subProcessDownloadOrders)
                        IAx00MainMDI.pausingAutomateProcess = True 'AG 29/01/2014 - HostQuery manual can not enter in running automatically (pause automate process just before go to Running)
                        IAx00MainMDI.CreateAutomaticWSWithLIS()
                    End If
                End If

                If (Not createAutoWS) Then RefreshAfterCloseIncompleteSamplesScreen()

            Else
                BarcodeWarningButton.Enabled = False
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".BarcodeWarningButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BarcodeWarningButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When user clicks the HQ button in MDI horizontal buttons bar open the rotor position screen + automatic popup!!
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 03/04/2013
    ''' </remarks>
    Private Sub IWSRotorPositions_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Try
            If ShowHostQueryScreenAttribute AndAlso BarcodeWarningButton.Visible AndAlso BarcodeWarningButton.Enabled Then
                BarcodeWarningButton.PerformClick()
            ElseIf AutoWSCreationWithLISModeAttribute AndAlso OpenByAutomaticProcessAttribute Then
                'AG 02/01/2014 - BT #1433 - Change, activate a timer and call the acceptbutton when the timer ellapses (v211 patch2)
                'CreateLogActivity("AutoCreate WS with LIS: Before press Accept button automatically", "IWSRotorPositions.IWSRotorPositions_Shown", EventLogEntryType.Information, False)
                'AcceptButton.PerformClick()
                bsAcceptButton.Enabled = False
                autoWSCreationTimer.Interval = 750
                autoWSCreationTimer.Enabled = True
                'AG 02/01/2014 - END
            Else
                ' XB 27/11/2013 - Inform to MDI that this screen is shown - Task #1303
                ShownScreen()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IWSRotorPositions_Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IWSRotorPositions_Shown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        ShowHostQueryScreenAttribute = False
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' CREATED BY:        JV 20/11/2013 #1382 - Event button to print the reagents report
    ''' <remarks></remarks>
    Private Sub bsPrintButton_Click(sender As Object, e As EventArgs) Handles bsPrintButton.Click
        Try
            GetInfoAreaForReport(AnalyzerIDAttribute, myRotorTypeForm)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsPrintButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsPrintButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

#Region "FOR TESTING-PROVISIONALLY"


    ''' <summary>
    ''' Verify if all required WorkSession Elements have been positioned in the Analyzer Rotors to set status 
    ''' of Positioning Warnings button
    ''' </summary>
    ''' <returns>True if there are required WorkSession Elements pending to positioning in the Analyzer Rotors;
    '''          otherwise it returns False</returns>
    ''' <remarks>
    ''' Created by:  SA 26/07/2010 - Not used; in this version the Warnings button is always enabled
    ''' </remarks>
    Private Function PendingPositioningElements(ByVal pReadInDB As Boolean) As Boolean
        Dim pendingElements As Boolean = True
        Try
            If (Not pReadInDB) Then
                pendingElements = ((From a In listReqElements _
                                   Where a.ElementStatus <> "POS" _
                                  Select a).ToList).Count > 0
            Else
                Dim resultData As New GlobalDataTO
                Dim myWSReqElementsDelegate As New WSRequiredElementsDelegate

                resultData = myWSReqElementsDelegate.CountNotPositionedElements(Nothing, WorkSessionIDAttribute, "", True)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    pendingElements = (DirectCast(resultData.SetDatos, Integer) > 0)
                Else
                    ShowMessage(Me.Name & ".PendingPositioningElements", resultData.ErrorCode, resultData.ErrorMessage, Me)
                End If
            End If
            bsWarningsButton.Enabled = pendingElements
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PendingPositioningElements ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PendingPositioningElements", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return pendingElements
    End Function
#End Region

#Region "TO DELETE"
    ' ''' <summary>
    ' ''' Configure and prepare all the CellControls (BSRImage)
    ' ''' </summary>
    ' ''' <remarks>
    ' ''' Created by:  TR 02/12/2009 - Tested: OK
    ' ''' Modified by: SA 21/12/2009 - Calls to MessageBox.Show were replaced by calls to the generic function 
    ' '''                              ShowMessage 
    ' '''                            - Add code (commented) to get the Icon Path and the name of the Icon used to show 
    ' '''                              FREE Rotor Positions from the AppSettings file and the Database (Preloaded Master Data)
    ' '''              SA 22/12/2009 - Added code to link Reagent Picture Boxes as children of the Reagents Rotor PictureBox   
    ' '''              AG 05/10/2010 - Change PictureBox for BSRImage control
    ' '''              RH 11/02/2011 - Make some code optimizations
    ' ''' </remarks>
    'Private Sub PreparePositionsControls(ByVal isResetRotor As Boolean)
    '    Try
    '        'Get the lis of all Positioning controls (PictureBox)
    '        myPosControlList = CreatePosControlList(SplitContainer1.Controls)

    '        'Prepare all the Position controls 
    '        For Each myControl As BSRImage In myPosControlList 'As PictureBox In myPosControlList
    '            If (myControl.Name.Contains("Sam")) Then
    '                'myControl.Parent = Me.SampleRotorPic
    '                If (Not isResetRotor) Then
    '                    'myControl.Location = New Point(CType(myControl, BSRImage).Location.X - Me.SampleRotorPic.Location.X, _
    '                    '                               CType(myControl, BSRImage).Location.Y - Me.SampleRotorPic.Location.Y)

    '                    'RH 11/02/2011 TypeOf myControl is BSRImage. There is no need to make the typecast so, CType gets out
    '                    'Also there is no need to create new points so, New Point() gets out
    '                    'myControl.Left = myControl.Left - Me.SampleRotorPic.Left
    '                    'myControl.Top = myControl.Top - Me.SampleRotorPic.Top

    '                    'myControl.Left = myControl.Left - 0
    '                    'myControl.Top = myControl.Top - 3

    '                    'TR 07/01/2010 Set the tag value by code
    '                    'myControl.Tag = myControl.Name.Replace("Sam", Nothing).Insert(1, ",").ToString()
    '                    myControl.Tag = myControl.Name.Replace("Sam", String.Empty).Insert(1, ",") 'RH 14/12/2010
    '                    'TR 07/01/2010 END
    '                End If

    '            ElseIf (myControl.Name.Contains("Reag")) Then
    '                'myControl.Parent = Me.ReagentRotorPic
    '                If (Not isResetRotor) Then
    '                    'myControl.Location = New Point(CType(myControl, BSRImage).Location.X - Me.ReagentRotorPic.Location.X, _
    '                    '                               CType(myControl, BSRImage).Location.Y - Me.ReagentRotorPic.Location.Y)

    '                    'RH 11/02/2011 TypeOf myControl is BSRImage. There is no need to make the typecast so, CType gets out
    '                    'Also there is no need to create new points so, New Point() gets out
    '                    'myControl.Left = myControl.Left - Me.ReagentRotorPic.Left
    '                    'myControl.Top = myControl.Top - Me.ReagentRotorPic.Top

    '                    'myControl.Left = myControl.Left + 1
    '                    'myControl.Top = myControl.Top - 3

    '                    'myControl.Tag = myControl.Name.Replace("Reag", Nothing).Insert(1, ",").ToString()
    '                    myControl.Tag = myControl.Name.Replace("Reag", String.Empty).Insert(1, ",") 'RH 14/12/2010
    '                End If
    '            End If

    '            'TR 05/01/2010
    '            myControl.Image = Nothing
    '            myControl.BackgroundImage = Nothing
    '            'myControl.BackgroundImageLayout = ImageLayout.Stretch 'TR 28/01/2010 add this property for the background image.

    '            'RH Remove ImageLayout.Stretch because it takes processing time
    '            'Make the image size be the actual needed size
    '            myControl.BackgroundImageLayout = ImageLayout.None

    '            myControl.InitialImage = Nothing

    '            'myControl.Visible = False
    '            'END TR 05/01/2010

    '            myControl.BackColor = Color.Transparent
    '            myControl.SizeMode = PictureBoxSizeMode.CenterImage
    '            myControl.AllowDrop = True

    '            'myControl.BringToFront()
    '            'myControl.WaitOnLoad = False
    '        Next

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PreparePositionsControls", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".PreparePositionsControls", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

    '    End Try
    'End Sub

    ''''' <summary>
    ''''' Automatic positioning of all Controls, Calibrators and Patient Samples still not placed in
    ''''' the Rotor. Update all positions assigned in the Rotor Area and the Status of the positioned
    ''''' Elements in the TreeView Area       
    ''''' </summary>
    ''''' <param name="pWorSessionID">WorkSession Identifier</param>
    ''''' <param name="pAnalizerID">Analizer identifier</param>
    ''''' <param name="pRotorType">Rotor Type</param>
    ''''' <remarks>
    ''''' Created by:  TR 02/12/2009
    ''''' Tested: OK
    ''''' Modified by: SA 21/12/2009 - Calls to MessageBox.Show were replaced by calls to the generic function 
    '''''                              ShowMessage  
    ''''' </remarks>
    ''Private Sub SamplesAutoPositioning(ByVal pWorSessionID As String, ByVal pAnalizerID As String, ByVal pRotorType As String)
    ''    Try
    ''        Dim myGlobaDataTo As New GlobalDataTO
    ''        Dim myWSRotorContentByPosition As New WSRotorContentByPositionDelegate

    ''        myGlobaDataTo = myWSRotorContentByPosition.SamplesAutoPositioning(Nothing, pWorSessionID, pAnalizerID, pRotorType)
    ''        If (Not myGlobaDataTo.HasError) Then
    ''            UpdateRotorTreeViewArea(CType(myGlobaDataTo.SetDatos, WSRotorContentByPositionDS))
    ''        Else
    ''            ShowMessage(Me.Name & ".SamplesAutoPositioning", myGlobaDataTo.ErrorCode, myGlobaDataTo.ErrorMessage)
    ''        End If
    ''    Catch ex As Exception
    ''        'Write error in the Application Log and show it  
    ''        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SamplesAutoPositioning", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    ''        ShowMessage(Me.Name & ".SamplesAutoPositioning", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
    ''    End Try
    ''End Sub
#End Region

#Region "Temporal bottle images positioning, refresh screen, ..."


    Private Sub BsRotate1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            Dim control As BSRImage = Reag144
            control.Rotation = control.Rotation + 1
            BsRotationAngle.Text = "R: " & control.Rotation.ToString
            BsRotate1.Focus()

        Catch ex As Exception
        End Try
    End Sub

    Private Sub BsRotate2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            Dim control As BSRImage = Reag144
            control.Rotation = control.Rotation - 1
            BsRotationAngle.Text = "R: " & control.Rotation.ToString
            BsRotate2.Focus()

        Catch ex As Exception
        End Try
    End Sub

    Private Sub BsTop1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            Dim control As BSRImage = Reag144
            control.Top = control.Location.Y + 1
            BsTop.Text = "Y: " & CStr(control.Location.Y + 3)
            BsTop1.Focus()

        Catch ex As Exception
        End Try
    End Sub

    Private Sub BsTop2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            Dim control As BSRImage = Reag144
            control.Top = control.Top - 1
            BsTop.Text = "Y: " & CStr(control.Location.Y + 3)
            BsTop2.Focus()

        Catch ex As Exception
        End Try
    End Sub


    Private Sub BsLeft1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            Dim control As BSRImage = Reag144
            control.Left = control.Left + 1
            BsLeft.Text = "X: " & CStr(control.Location.X - 1)
            BsLeft1.Focus()

        Catch ex As Exception
        End Try
    End Sub

    Private Sub BsLeft2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            Dim control As BSRImage = Reag144
            control.Left = control.Left - 1
            BsLeft.Text = "X: " & CStr(control.Location.X - 1)
            BsLeft2.Focus()

        Catch ex As Exception
        End Try
    End Sub

    Private Sub BsRefresh_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsRefresh.Click
        'Try

        'Dim myGlobal As New GlobalDataTO
        'Dim myDlg As New WSRotorContentByPositionDelegate

        'Dim rotorType As String = "REAGENTS"
        'Dim PosStatus As String = "DEPLETED"
        'Dim myCellNumber As Integer = 1
        'Dim myRealVolume As Single = 0
        'Dim myTestLeft As Integer = 0
        'myGlobal = myDlg.UpdateByRotorTypeAndCellNumber(Nothing, rotorType, myCellNumber, PosStatus, myRealVolume, myTestLeft, WorkSessionIDAttribute, AnalyzerIDAttribute, False, False)

        'Dim elementStatus As String = "POS"
        'If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '    Dim rcp_DS As New WSRotorContentByPositionDS
        '    rcp_DS = CType(myGlobal.SetDatos, WSRotorContentByPositionDS)

        '    If rcp_DS.twksWSRotorContentByPosition.Rows.Count > 0 Then
        '        If Not rcp_DS.twksWSRotorContentByPosition(0).IsElementStatusNull Then elementStatus = rcp_DS.twksWSRotorContentByPosition(0).ElementStatus
        '    End If
        'End If

        'Dim myUI_RefreshDS As New UIRefreshDS
        'Dim myNewRotorPositionRow As UIRefreshDS.RotorPositionsChangedRow

        'myNewRotorPositionRow = myUI_RefreshDS.RotorPositionsChanged.NewRotorPositionsChangedRow
        'With myNewRotorPositionRow
        '    .BeginEdit()
        '    .WorkSessionID = WorkSessionIDAttribute
        '    .AnalyzerID = AnalyzerIDAttribute
        '    .RotorType = rotorType
        '    .CellNumber = myCellNumber
        '    .Status = PosStatus
        '    .ElementStatus = elementStatus
        '    .RealVolume = myRealVolume
        '    .RemainingTestsNumber = myTestLeft
        '    .EndEdit()
        'End With
        'myUI_RefreshDS.RotorPositionsChanged.AddRotorPositionsChangedRow(myNewRotorPositionRow)

        'Dim myList As New List(Of GlobalEnumerates.UI_RefreshEvents)
        'myList.Add(GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED)
        'RefreshScreen(myList, myUI_RefreshDS)

        'Catch ex As Exception

        'End Try
    End Sub

#End Region

End Class

