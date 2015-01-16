Option Explicit On
Option Strict On

'Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
'Imports Biosystems.Ax00.BL.Framework
Imports Biosystems.Ax00.FwScriptsManagement
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates
'Imports Biosystems.Ax00.CommunicationsSwFw
'Imports System.Configuration
'Imports System.Xml.Serialization
'Imports System.IO

Public Class FwScriptsEdition
    ' XBC 02/11/2011 - error multiples threads 
    Inherits PesentationLayer.BSAdjustmentBaseForm
    'Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm
    ' XBC 02/11/2011 - error multiples threads 

#Region "Enumerates"
    Private Enum ScreenModes
        _NONE
        INITIATING
        INITIATED
        LOADING
        LOADED
        MODIFYING
        MODIFIED
        GRIDCHANGING
        LOCALSAVING
        GLOBALSAVING
        SCRIPT_CHECKING
        SCRIPT_TESTING
        SCRIPT_IMPORTING
        SCRIPT_EXPORTING
        ALL_IMPORTING
        ALL_RESTORING
        ON_ERROR
    End Enum

    Private Enum RowMovement
        DOWN
        UP
    End Enum
#End Region

#Region "Properties"

    'already implemented in base class. Inheritance is your friend!
    'Private ReadOnly Property SimulationMode() As Boolean
    '    Get
    '        If GlobalConstants.REAL_DEVELOPMENT_MODE = 1 Then
    '            SimulationModeAttr = True ' Simulation mode
    '        ElseIf GlobalConstants.REAL_DEVELOPMENT_MODE = 2 Then
    '            SimulationModeAttr = False ' Developer mode
    '        Else
    '            SimulationModeAttr = False ' Real mode
    '        End If
    '        Return SimulationModeAttr
    '    End Get
    'End Property

    Private Property CurrentScreenMode() As ScreenModes
        Get
            Return CurrentScreenModeAttr
        End Get
        Set(ByVal value As ScreenModes)
            If value <> CurrentScreenModeAttr Then
                CurrentScreenModeAttr = value
                MyClass.PrepareUIPrepareScreen()

                'QUITAR
                Dim myMDI As Ax00ServiceMainMDI
                myMDI = CType(Me.MdiParent, Ax00ServiceMainMDI)
                If myMDI IsNot Nothing Then
                    myMDI.ErrorStatusLabel.Text = CurrentScreenModeAttr.ToString
                End If

            End If
        End Set
    End Property

    Private ReadOnly Property IsSyntaxOK() As Boolean
        Get
            If MyClass.SelectedFwScript IsNot Nothing Then
                Return MyClass.SelectedFwScript.SyntaxOK
            Else
                Return False
            End If
        End Get
    End Property

    Private ReadOnly Property IsTestedOK() As Boolean
        Get
            If MyClass.SelectedFwScript IsNot Nothing Then
                Return MyClass.SelectedFwScript.TestedOK
            Else
                Return False
            End If
        End Get
    End Property


    '
    ''' <summary>
    ''' Defines the screen mode in which a new Instruction is being added 
    ''' to the editing Script
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>SG 10/11/10</remarks>
    Private Property RowAddingMode() As Boolean
        Get
            Return RowAddingModeAttribute

        End Get
        Set(ByVal value As Boolean)
            RowAddingModeAttribute = value

        End Set
    End Property

    '
    ''' <summary>
    ''' Defines the screen mode in which the all Application Scripts Data 
    ''' has been uploaded with the modified Scripts locally
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>SG 08/11/10</remarks>
    Private Property GlobalChangesMade() As Boolean
        Get
            Return GlobalChangesMadeAttribute
        End Get
        Set(ByVal value As Boolean)
            GlobalChangesMadeAttribute = value

            Me.BsSaveAllButton.Enabled = value

            Me.BSAnalyzerCombo.Enabled = Not value

        End Set
    End Property

#End Region

#Region "Attributes"
    Private SimulationModeAttr As Boolean = False
    Private CurrentScreenModeAttr As ScreenModes = ScreenModes._NONE
    Private IsFwScriptLoadedAttr As Boolean = False
    Private RowAddingModeAttribute As Boolean = False
    Private GlobalChangesMadeAttribute As Boolean = False

#End Region

#Region "Flags"
    Private IsLocallySaved As Boolean = False

#End Region

#Region "Declarations"
    Private CurrentFwScriptsData As FwScriptsDataTO
    Private InitialFwScript As New FwScriptTO
    Private SelectedFwScript As New FwScriptTO
    Private SelectedFwScriptRelatedNodes As New List(Of TreeNode)
    Private SelectedFwScriptRelatedScreenNodes As New List(Of TreeNode)
    Private SavedFwScriptNodes As New List(Of TreeNode)
    Private SavedScreenNodes As New List(Of TreeNode)
    Private SelectedNode As TreeNode
    Private ValidationError As Boolean

    Private LanguageID As String '//15/11/2010 PG

    ' XBC 02/11/2011 - error multiples threads 
    ''Test Scripts
    'Private myAnalyzer As AnalyzerManager

    'Private WithEvents mySendFwScriptDelegate As SendFwScriptsDelegate
    'Private WithEvents myScreenDelegate As FwScriptsEditionDelegate
    Private WithEvents myScreenDelegate As FwScriptsEditionDelegate
    ' XBC 02/11/2011 - error multiples threads 

    Private PreviousScreenModeBeforeTesting As ScreenModes = ScreenModes._NONE
#End Region

#Region "constructor"
    Public Sub New()
        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Me.Text = Application.ProductName
    End Sub
#End Region

#Region "Communications Events"
    ''' <summary>
    ''' Executes ManageReceptionEvent() method in the main thread
    ''' </summary>
    ''' <param name="pResponse"></param>
    ''' <param name="pData"></param>
    ''' <remarks>Created by XBC 15/09/2011</remarks>
    Public Sub ScreenReceptionLastFwScriptEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) Handles myScreenDelegate.ReceivedLastFwScriptEvent

        ' XBC 02/11/2011 - error multiples threads 
        'Public Sub ScreenReceptionLastFwScriptEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) Handles mySendFwScriptDelegate.LastFwScriptResponseEvent
        ' XBC 02/11/2011 - error multiples threads 

        Me.UIThread(Function() ManageReceptionEvent(pResponse, pData))

    End Sub

    ''' <summary>
    ''' manages the response of the Analyzer after sending a Script List
    ''' </summary>
    ''' <param name="pResponse">response type</param>
    ''' <param name="pData">data received</param>
    ''' <remarks>Created by XBC 24/11/10</remarks>
    Private Function ManageReceptionEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) As Boolean
        Dim myGlobal As New GlobalDataTO
        Try
            If MyClass.CurrentScreenMode = ScreenModes.SCRIPT_TESTING Then

                '' XBC 02/11/2011 - error multiples threads 
                ''MyClass.myAnalyzer.ClearQueueToSend()
                'Ax00ServiceMainMDI.MDIAnalyzerManager.ClearQueueToSend()
                '' XBC 02/11/2011 - error multiples threads 

                Select Case pResponse
                    Case RESPONSE_TYPES.START
                        ' Nothing by now 

                    Case RESPONSE_TYPES.OK
                        MyClass.SelectedFwScript.TestedOK = True
                        myGlobal = MyClass.DisplayGridResults(True, True)
                        'MessageBox.Show(GetMessageText(Messages.SRV_TEST_COMPLETED.ToString))
                        ' DisplayMessage !!!

                        ' XBC 02/11/2011 - error multiples threads 
                        MyClass.CurrentScreenMode = MyClass.PreviousScreenModeBeforeTesting

                    Case RESPONSE_TYPES.NG, RESPONSE_TYPES.TIMEOUT, RESPONSE_TYPES.EXCEPTION
                        MyClass.SelectedFwScript.TestedOK = False
                        myGlobal = MyClass.DisplayGridResults(True, True)
                        MessageBox.Show(GetMessageText(Messages.ERROR_COMM.ToString))

                        ' XBC 02/11/2011 - error multiples threads 
                        MyClass.CurrentScreenMode = MyClass.PreviousScreenModeBeforeTesting

                End Select

            End If

            ' XBC 02/11/2011 - error multiples threads 
            'MyClass.CurrentScreenMode = MyClass.PreviousScreenModeBeforeTesting

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".OnManageReceptionFwScriptEvent ", _
                                     EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        End Try

        Return True
    End Function
#End Region

#Region "Methods"

#Region "Common Methods"

    ''' <summary>
    ''' Cleaning all fields
    ''' </summary>
    ''' <remarks>created by XBC 27/09/2010</remarks>
    Private Sub CleanFwScriptFields()
        BsNameTextBox.Text = ""
        BsDescriptionTextBox.Text = ""
        BsCreatedTextBox.Text = ""
        BsModifiedTextBox.Text = ""
        BsAuthorTextBox.Text = ""
        bsInstructionsListDataGridView.Rows.Clear()
    End Sub

   

    Private Function GetCurrentAuthorID() As GlobalDataTO
        Dim myResultData As New GlobalDataTO
        Try
            Dim dataSessionTO As New ApplicationInfoSessionTO
            dataSessionTO = GetApplicationInfoSession()

            myResultData.SetDatos = dataSessionTO.UserName

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".GetCurrentAuthorID", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".GetCurrentAuthorID", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myResultData
    End Function


    Private Function ConvertGridToScript() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            Dim myInstructionsList As New List(Of InstructionTO)

            'transform the grid to Instructions List
            For Each R As DataGridViewRow In Me.bsInstructionsListDataGridView.Rows
                Dim myInstructionTO As New InstructionTO
                With myInstructionTO
                    .InstructionID = R.Index + 1
                    .Sequence = R.Index + 1

                    Try
                        .Timer = CInt(R.Cells("Timer").Value)
                    Catch ex As Exception
                        myGlobal.HasError = True
                        myGlobal.ErrorCode = GlobalEnumerates.Messages.FWSCRIPT_EDIT_NUMERIC_ONLY.ToString
                        Exit Try
                    End Try


                    .Code = CStr(R.Cells("Code").Value)
                    .Params = CStr(R.Cells("Params").Value)

                End With
                myInstructionsList.Add(myInstructionTO)
            Next

            myGlobal.SetDatos = myInstructionsList

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = "SYSTEM_ERROR"
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ConvertGridToScript", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ConvertGridToScript", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

#End Region

#Region "Initialize Methods"
    Private Sub LoadScreen()
        Try
            MyClass.GlobalChangesMade = False
            MyClass.IsLocallySaved = False

            MyClass.PrepareButtons()
            MyClass.PrepareTreeViewImages()
            MyClass.GetScreenLabels()
            MyClass.InitializeInstructionsGrid()

            ' XBC 02/11/2011 - error multiples threads 
            'MyClass.myAnalyzer = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
            'MyClass.mySendFwScriptDelegate = New SendFwScriptsDelegate(myAnalyzer)

            'MyClass.CurrentFwScriptsData = MyClass.mySendFwScriptDelegate.ActiveFwScriptsDO
            'screen delegate SGM 20/01/2012
            MyClass.myScreenDelegate = New FwScriptsEditionDelegate(MyBase.myServiceMDI.ActiveAnalyzer, myFwScriptDelegate)
            MyClass.CurrentFwScriptsData = myScreenDelegate.ActiveFwScriptsDO

            'MyClass.myScreenDelegate = New FwScriptsEditionDelegate(MyClass.CurrentFwScriptsData, MyClass.mySendFwScriptDelegate)
            ' XBC 02/11/2011 - error multiples threads 

            MyClass.FillAnalyzersCombo()
            MyClass.LoadScreensTreeView()
            MyClass.SelectedFwScript = New FwScriptTO

            If Not Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then
                BsTestButton.Enabled = False
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".LoadScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LoadScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the icons used for painting the treeview
    ''' </summary>
    ''' <remarks>SG 19/09/2010</remarks>
    Private Sub PrepareTreeViewImages()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'SCREEN Image
            auxIconName = GetIconName("GRID")   ' SCREEN PDT !!! i totes les icones !!!
            If System.IO.File.Exists(iconPath & auxIconName) Then
                BStreeScreenIcons.Images.Add("SCREEN", Image.FromFile(iconPath & auxIconName))
            End If

            'ACTION Image
            auxIconName = GetIconName("ADJUSTMENT") ' ACTION
            If System.IO.File.Exists(iconPath & auxIconName) Then
                BStreeScreenIcons.Images.Add("ACTION", Image.FromFile(iconPath & auxIconName))
            End If

            Me.BsScreenActionsTreeView.ImageList = BStreeScreenIcons

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTreeViewImages", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTreeViewImages", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the icons and tooltips used for painting the buttons
    ''' </summary>
    ''' <remarks>SG 19/09/2010</remarks>
    Private Sub PrepareButtons()
        'Dim auxIconName As String = ""
        'Dim iconPath As String = MyBase.IconsPath
        'Dim myGlobal As New GlobalDataTO
        'Dim myUtil As New Utilities
        Try

            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath


            auxIconName = GetIconName("ACCEPT1")
            If (auxIconName <> "") Then BsCheckButton.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("VOLUME")
            If (auxIconName <> "") Then BsTestButton.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("REMOVE")
            If (auxIconName <> "") Then
                BsRemoveButton.Image = Image.FromFile(iconPath & auxIconName)
                BsUnTestButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("SAVE")
            If (auxIconName <> "") Then
                BsSaveButton.Image = Image.FromFile(iconPath & auxIconName)
                BsSaveAllButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("EDIT")
            If (auxIconName <> "") Then BsEditButton.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("FACTORY")
            If (auxIconName <> "") Then BsRestoreButton.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("IMPORTSCRIPT")
            If (auxIconName <> "") Then BsImportButton.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("EXPORTSCRIPT")
            If (auxIconName <> "") Then BsExportButton.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("UNDO")
            If (auxIconName <> "") Then BsCancelButton.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("OPEN")
            If (auxIconName <> "") Then BsImportAllButton.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then BsExitButton.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("ADD")
            If (auxIconName <> "") Then BsAddButton.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("CHANGEROTORB")
            If (auxIconName <> "") Then BsMoveUpButton.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("DECREASEBUT")
            If (auxIconName <> "") Then BsMoveDownButton.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("READADJ")
            If (auxIconName <> "") Then BsExportAllToFileButton.Image = Image.FromFile(iconPath & auxIconName)

            'MyClass.SetButtonImage(BsCheckButton, "ACCEPT1")
            'MyClass.SetButtonImage(BsTestButton, "VOLUME")
            'MyClass.SetButtonImage(BsUnTestButton, "CANCELF")
            'MyClass.SetButtonImage(BsUnTestButton, "DELETE")
            'MyClass.SetButtonImage(BsSaveButton, "SAVE")
            'MyClass.SetButtonImage(BsEditButton, "EDIT")
            'MyClass.SetButtonImage(BsRestoreButton, "FACTORY")
            'MyClass.SetButtonImage(BsImportButton, "IMPORTSCRIPT", 24, 24)
            'MyClass.SetButtonImage(BsExportButton, "EXPORTSCRIPT", 24, 24)
            'MyClass.SetButtonImage(BsCancelButton, "UNDO")
            'MyClass.SetButtonImage(BsImportAllButton, "OPEN")
            'MyClass.SetButtonImage(BsSaveAllButton, "SAVEALL", 24, 24)
            'MyClass.SetButtonImage(BsExitButton, "CANCEL")
            'MyClass.SetButtonImage(BsAddButton, "ADD")
            'MyClass.SetButtonImage(BsRemoveButton, "REMOVE")
            'MyClass.SetButtonImage(BsMoveUpButton, "REP_INC")
            'MyClass.SetButtonImage(BsMoveDownButton, "RED_REP")
            'MyClass.SetButtonImage(BsExportAllToFileButton, "READADJ")


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: PG 15/11/2010
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels and ToolTips of Graphical Buttons.....
            BsNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", LanguageID) + ":"
            BsDescriptionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Description", LanguageID) + ":" 'JB 01/10/2012 - Resource String unification
            BsCreatedLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SCRIPT_EDIT_Created", LanguageID) + ":"
            BsModifiedLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SCRIPT_EDIT_Modified", LanguageID) + ":"
            BsAuthorLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SCRIPT_EDIT_Author", LanguageID) + ":"
            BsAnalyzerLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SCRIPT_EDIT_Analyzer", LanguageID) + ":"

            BsScreenActionsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_SCRIPT_EDIT_ScreenActions", LanguageID)
            BsActionScriptLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_SCRIPT_EDIT_ActionScript", LanguageID)
            BsInstructionsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_SCRIPT_EDIT_Instructions", LanguageID)

            ScreenTooltips.SetToolTip(BsCheckButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SCRIPT_EDIT_Check", LanguageID))
            ScreenTooltips.SetToolTip(BsTestButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SCRIPT_EDIT_Test", LanguageID))
            ScreenTooltips.SetToolTip(BsRestoreButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SCRIPT_EDIT_Restore", LanguageID))
            ScreenTooltips.SetToolTip(BsImportButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SCRIPT_EDIT_Import", LanguageID))
            ScreenTooltips.SetToolTip(BsExportButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SCRIPT_EDIT_Export", LanguageID))
            ScreenTooltips.SetToolTip(BsEditButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Edit", LanguageID))
            ScreenTooltips.SetToolTip(BsSaveButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save", LanguageID))
            ScreenTooltips.SetToolTip(BsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel&Close", LanguageID))
            ScreenTooltips.SetToolTip(BsAddButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_AddNew", LanguageID))
            ScreenTooltips.SetToolTip(BsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save&Close", LanguageID))
            ScreenTooltips.SetToolTip(BsMoveDownButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SCRIPT_EDIT_MoveDown", LanguageID))
            ScreenTooltips.SetToolTip(BsMoveUpButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SCRIPT_EDIT_MoveUp", LanguageID))
            ScreenTooltips.SetToolTip(BsImportAllButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SCRIPT_EDIT_ImportAll", LanguageID))
            ScreenTooltips.SetToolTip(BsRemoveButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Delete", LanguageID))
            ScreenTooltips.SetToolTip(BsSaveAllButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SCRIPT_EDIT_SaveAll", LanguageID))



        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepares the Instructions Datagridview
    ''' </summary>
    ''' <remarks>
    ''' Created by SG 16/09/10          
    ''' Modified by PG 15/11/2010: use the multilanguage for the labels  
    ''' Modified by XBC 17/11/2010 - add EnableEdition hidden column
    ''' </remarks>
    Private Sub InitializeInstructionsGrid()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate '//15/11/2010 PG
            Dim gridWidth As Integer = bsInstructionsListDataGridView.Width - 23

            Dim colInstructionID As New DataGridViewTextBoxColumn

            bsInstructionsListDataGridView.Columns.Clear()

            '15/11/2010 PG 
            'colInstructionID.HeaderText = "InstructionID"
            colInstructionID.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SCRIPT_EDIT_InstructionID", LanguageID) '10/11/2010 PG
            '15/11/2010 PG
            colInstructionID.Name = "InstructionID"
            bsInstructionsListDataGridView.Columns.Add(colInstructionID)

            Dim colSequenceNumber As New DataGridViewTextBoxColumn
            '15/11/2010 PG
            'colSequenceNumber.HeaderText = "Sequence"
            colSequenceNumber.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SCRIPT_EDIT_SequenceNumber", LanguageID)
            '15/11/2010 PG
            colSequenceNumber.Name = "Sequence"
            bsInstructionsListDataGridView.Columns.Add(colSequenceNumber)

            Dim colTimer As New DataGridViewTextBoxColumn
            '15/11/2010 PG
            'colTimer.HeaderText = "Timer"
            colTimer.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SCRIPT_EDIT_Timer", LanguageID)
            '15/11/2010 PG
            colTimer.Name = "Timer"
            bsInstructionsListDataGridView.Columns.Add(colTimer)

            Dim colCode As New DataGridViewTextBoxColumn
            '15/11/2010 PG
            'colCode.HeaderText = "Code"
            colCode.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SCRIPT_EDIT_Code", LanguageID)
            '15/11/2010 PG
            colCode.Name = "Code"
            bsInstructionsListDataGridView.Columns.Add(colCode)

            Dim colPars As New DataGridViewTextBoxColumn
            '15/11/2010 PG
            'colPars.HeaderText = "Params"
            colPars.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Parameters", LanguageID) 'JB 01/10/2012 - Resource String unification
            '15/11/2010 PG
            colPars.Name = "Params"
            bsInstructionsListDataGridView.Columns.Add(colPars)

            Dim colSyntax As New DataGridViewCheckBoxColumn
            '15/11/2010 PG
            'colSyntax.HeaderText = "SyntaxOK"
            colSyntax.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SCRIPT_EDIT_SyntaxOK", LanguageID)
            '15/11/2010 PG
            colSyntax.Name = "SyntaxOK"
            bsInstructionsListDataGridView.Columns.Add(colSyntax)

            Dim colTest As New DataGridViewCheckBoxColumn
            '15/11/2010 PG
            'colTest.HeaderText = "TestedOK"
            colTest.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SCRIPT_EDIT_TestOK", LanguageID)
            '15/11/2010 PG
            colTest.Name = "TestedOK"
            bsInstructionsListDataGridView.Columns.Add(colTest)

            '17/11/2010 XBC
            Dim colEnableEdition As New DataGridViewCheckBoxColumn
            colEnableEdition.HeaderText = "EnableEdition"
            colEnableEdition.Name = "EnableEdition"
            bsInstructionsListDataGridView.Columns.Add(colEnableEdition)

            bsInstructionsListDataGridView.Columns("InstructionID").Visible = False
            bsInstructionsListDataGridView.Columns("Sequence").Visible = False
            bsInstructionsListDataGridView.Columns("Timer").Width = CInt(10 * gridWidth / 100)
            bsInstructionsListDataGridView.Columns("Code").Width = CInt(30 * gridWidth / 100)
            bsInstructionsListDataGridView.Columns("Params").Width = CInt(35 * gridWidth / 100)
            bsInstructionsListDataGridView.Columns("SyntaxOK").Width = CInt(10 * gridWidth / 100)
            bsInstructionsListDataGridView.Columns("TestedOK").Width = CInt(15 * gridWidth / 100)
            bsInstructionsListDataGridView.Columns("EnableEdition").Visible = False

            bsInstructionsListDataGridView.Columns("Timer").SortMode = DataGridViewColumnSortMode.NotSortable
            bsInstructionsListDataGridView.Columns("Code").SortMode = DataGridViewColumnSortMode.NotSortable
            bsInstructionsListDataGridView.Columns("Params").SortMode = DataGridViewColumnSortMode.NotSortable
            bsInstructionsListDataGridView.Columns("SyntaxOK").SortMode = DataGridViewColumnSortMode.NotSortable
            bsInstructionsListDataGridView.Columns("TestedOK").SortMode = DataGridViewColumnSortMode.NotSortable

            bsInstructionsListDataGridView.Columns("Timer").ReadOnly = False
            bsInstructionsListDataGridView.Columns("Code").ReadOnly = False
            bsInstructionsListDataGridView.Columns("Params").ReadOnly = False
            bsInstructionsListDataGridView.Columns("SyntaxOK").ReadOnly = True
            bsInstructionsListDataGridView.Columns("TestedOK").ReadOnly = True

            bsInstructionsListDataGridView.Columns("Timer").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsInstructionsListDataGridView.Columns("Code").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsInstructionsListDataGridView.Columns("Params").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsInstructionsListDataGridView.Columns("SyntaxOK").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsInstructionsListDataGridView.Columns("TestedOK").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            bsInstructionsListDataGridView.EditMode = DataGridViewEditMode.EditOnEnter

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeInstructionsGrid", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeInstructionsGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fills the combo with the Analyzers involved in the Application's Scripts Data
    ''' </summary>
    ''' <remarks>
    ''' created by SG 29/09/2010
    ''' modified by XBC 24/11/2010 - myScriptDelegate becomes global into the class instead local
    ''' </remarks>
    Private Function FillAnalyzersCombo() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            If MyClass.CurrentScreenMode = ScreenModes.ON_ERROR Then
                myGlobal.HasError = True
                Exit Try
            End If

            Dim myAnalyzers As New List(Of AnalyzerFwScriptsTO)
            'Dim myScriptsDelegate As New ScriptsEditionDelegate(Me.CurrentScriptsData)

            myGlobal = MyClass.myScreenDelegate.GetAnalyzers()
            If Not myGlobal.HasError And Not myGlobal Is Nothing Then
                myAnalyzers = CType(myGlobal.SetDatos, List(Of AnalyzerFwScriptsTO))

                Me.BSAnalyzerCombo.Items.Clear()
                For Each A As AnalyzerFwScriptsTO In myAnalyzers
                    Me.BSAnalyzerCombo.Items.Add(A.AnalyzerID)
                Next
                If Me.BSAnalyzerCombo.Items.Count > 0 Then
                    Me.BSAnalyzerCombo.SelectedIndex = 0
                End If

            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".FillAnalyzersCombo", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".FillAnalyzersCombo", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Loads all the Scripts Data related to the selected Analyzer
    ''' </summary>
    ''' <remarks>
    ''' created by SG 29/09/2010
    ''' modified by XBC 24/11/2010 - myScriptDelegate becomes global into the class instead local
    ''' </remarks>
    Private Function LoadScreensTreeView() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            MyClass.CurrentScreenMode = ScreenModes.INITIATING

            If MyClass.CurrentScreenMode = ScreenModes.ON_ERROR Then
                myGlobal.HasError = True
                Exit Try
            End If

            BsScreenActionsTreeView.Nodes.Clear()

            BsScreenActionsTreeView.ShowNodeToolTips = True

            Dim resultData As New GlobalDataTO
            Dim mySelectedScreens As New List(Of ScreenTO)
            'Dim myScriptsDelegate As New ScriptsEditionDelegate(Me.CurrentScriptsData)
            resultData = MyClass.myScreenDelegate.GetScreensByAnalyzerID(CStr(Me.BSAnalyzerCombo.SelectedItem))
            If Not resultData.HasError And Not resultData Is Nothing Then
                mySelectedScreens = CType(resultData.SetDatos, List(Of ScreenTO))

                If mySelectedScreens.Count > 0 Then
                    For Each S As ScreenTO In mySelectedScreens
                        Dim ScreenNode As TreeNode
                        ScreenNode = New TreeNode(S.ScreenID)
                        BsScreenActionsTreeView.Nodes.Add(ScreenNode)
                        ScreenNode.ImageKey = "SCREEN"

                        Dim ActionNode As TreeNode
                        ActionNode = New TreeNode()

                        'get all scripts actions
                        Dim myActionIDs As New List(Of String)
                        For Each sID As Integer In S.FwScriptIDs
                            resultData = MyClass.myScreenDelegate.GetFwScriptByID(sID)
                            If Not resultData.HasError And Not resultData Is Nothing Then
                                Dim myFwScript As FwScriptTO
                                myFwScript = CType(resultData.SetDatos, FwScriptTO)
                                myActionIDs.Add(myFwScript.ActionID)
                            End If
                        Next

                        'order scripts alphabetically
                        Dim myOrderedActionIDs As New List(Of String)
                        myOrderedActionIDs = (From a In myActionIDs Order By a Select a).ToList()

                        Dim myOrderedFwScriptIDs As New List(Of Integer)
                        For Each A As String In myOrderedActionIDs
                            For Each sID As Integer In S.FwScriptIDs
                                resultData = MyClass.myScreenDelegate.GetFwScriptByID(sID)
                                If Not resultData.HasError And Not resultData Is Nothing Then
                                    Dim myFwScript As FwScriptTO
                                    myFwScript = CType(resultData.SetDatos, FwScriptTO)
                                    If A = myFwScript.ActionID Then
                                        myOrderedFwScriptIDs.Add(sID)
                                    End If
                                End If
                            Next
                        Next

                        For Each sID As Integer In myOrderedFwScriptIDs
                            resultData = MyClass.myScreenDelegate.GetFwScriptByID(sID)
                            If Not resultData.HasError And Not resultData Is Nothing Then
                                Dim myFwScript As FwScriptTO
                                myFwScript = CType(resultData.SetDatos, FwScriptTO)
                                ActionNode = ScreenNode.Nodes.Add(myFwScript.FwScriptID.ToString)
                                ActionNode.Text = myFwScript.ActionID
                                ActionNode.ImageKey = "ACTION"
                                ActionNode.SelectedImageKey = "ACTION"
                            End If
                        Next

                    Next

                End If
            End If
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".LoadScreensTreeView", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LoadScreensTreeView", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        If Not myGlobal.HasError Then
            MyClass.CurrentScreenMode = ScreenModes.INITIATED
        Else
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End If

        Return myGlobal
    End Function
#End Region

#Region "Treeview related methods"


    ''' <summary>
    ''' Gets a list of nodes that represent the same script action
    ''' </summary>
    ''' <param name="pActionID"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by  SG 08/11/10
    ''' Modified by XB 04/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
    ''' </remarks>
    Private Function GetFwScriptRelatedNodes(ByVal pActionID As String) As List(Of TreeNode)
        Dim myNodes As New List(Of TreeNode)
        Try
            For Each F As TreeNode In Me.BsScreenActionsTreeView.Nodes
                If F.Nodes.Count > 0 Then
                    For Each N As TreeNode In F.Nodes
                        If N.Nodes.Count = 0 Then
                            'If N.Text.ToUpper.Trim = pActionID.ToUpper.Trim Then
                            If N.Text.ToUpperBS.Trim = pActionID.ToUpperBS.Trim Then
                                myNodes.Add(N)
                            End If
                        End If
                    Next

                End If
            Next

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".GetFwScriptRelatedNodes", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".GetFwScriptRelatedNodes", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myNodes
    End Function

    ''' <summary>
    ''' Gets a list of nodes that represent the Screens in which is defined the same script action
    ''' </summary>
    ''' <param name="pActionID"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by  SG 08/11/10
    ''' Modified by XB 04/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
    ''' </remarks>
    Private Function GetFwScriptRelatedScreenNodes(ByVal pActionID As String) As List(Of TreeNode)
        Dim myNodes As New List(Of TreeNode)
        Try

            For Each F As TreeNode In Me.BsScreenActionsTreeView.Nodes
                If F.Nodes.Count > 0 Then
                    For Each N As TreeNode In F.Nodes
                        If N.Nodes.Count = 0 Then
                            'If N.Text.ToUpper.Trim = pActionID.ToUpper.Trim Then
                            If N.Text.ToUpperBS.Trim = pActionID.ToUpperBS.Trim Then
                                myNodes.Add(F)
                                Exit For
                            End If
                        End If
                    Next

                End If
            Next

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".GetFwScriptRelatedScreenNodes", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".GetFwScriptRelatedScreenNodes", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myNodes
    End Function

    ''' <summary>
    ''' Hightlights the editing Script Action's nodes in the treeview while editing
    ''' </summary>
    ''' <remarks>created by SG 19/11/2010</remarks>
    Private Sub TreeviewWhileEditing()
        Try
            'screen nodes in which the script is defined
            Me.SelectedFwScriptRelatedScreenNodes = GetFwScriptRelatedScreenNodes(Me.SelectedFwScript.ActionID)

            'nodes that represents the same script
            Me.SelectedFwScriptRelatedNodes = GetFwScriptRelatedNodes(Me.SelectedFwScript.ActionID)

            'bold the selected script action
            For Each N As TreeNode In Me.SelectedFwScriptRelatedNodes
                N.ForeColor = Color.White
                N.BackColor = SystemColors.Highlight
            Next

            'bold the screens involved
            For Each N As TreeNode In Me.SelectedFwScriptRelatedScreenNodes
                'N.ForeColor = Color.Gray
                If Me.SelectedNode IsNot N Then
                    N.Expand()
                End If
            Next
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".TreeviewWhileEditing", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".TreeviewWhileEditing", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' The editing Script Action's nodes come back to their normal look
    ''' </summary>
    ''' <remarks>created by SG 19/11/2010</remarks>
    Private Sub TreeviewWhileReading()
        Try

            For Each N As TreeNode In Me.BsScreenActionsTreeView.Nodes 'screens
                If Not Me.SavedScreenNodes.Contains(N) Then
                    N.ForeColor = Me.BsScreenActionsTreeView.ForeColor
                End If
                For Each NN As TreeNode In N.Nodes 'actions
                    If Not Me.SavedFwScriptNodes.Contains(NN) Then
                        NN.ForeColor = Me.BsScreenActionsTreeView.ForeColor
                    End If
                    NN.BackColor = Me.BsScreenActionsTreeView.BackColor
                Next NN
                If Me.SelectedNode IsNot N Then
                    'N.Collapse()
                End If
            Next N


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".TreeviewWhileReading", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".TreeviewWhileReading", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


#End Region

#Region "UI Prepare Methods"

    Private Sub PrepareUIPrepareScreen()
        Try
            Select Case MyClass.CurrentScreenMode

                Case ScreenModes.INITIATING
                    MyClass.PrepareUIInitiating()

                Case ScreenModes.INITIATED
                    MyClass.PrepareUIInitiated()

                Case ScreenModes.LOADING
                    MyClass.PrepareUILoading()

                Case ScreenModes.LOADED
                    MyClass.PrepareUILoaded()

                Case ScreenModes.MODIFYING
                    MyClass.PrepareUIModifying()

                Case ScreenModes.MODIFIED
                    MyClass.PrepareUIModified()

                Case ScreenModes.GRIDCHANGING
                    MyClass.PrepareUIGridChanging()

                Case ScreenModes.LOCALSAVING
                    MyClass.PrepareUILocalSaving()

                Case ScreenModes.GLOBALSAVING
                    MyClass.PrepareUIGlobalSaving()

                Case ScreenModes.SCRIPT_CHECKING
                    MyClass.PrepareUIScriptChecking()

                Case ScreenModes.SCRIPT_TESTING
                    MyClass.PrepareUIScriptTesting()

                Case ScreenModes.SCRIPT_IMPORTING
                    MyClass.PrepareUIScriptImporting()

                Case ScreenModes.SCRIPT_EXPORTING
                    MyClass.PrepareUIScriptExporting()

                Case ScreenModes.ALL_IMPORTING
                    MyClass.PrepareUIAllImporting()

                Case ScreenModes.ALL_RESTORING
                    MyClass.PrepareUIAllRestoring()

                Case ScreenModes.ON_ERROR
                    MyClass.PrepareUIOnError()

            End Select
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUIPrepareScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUIPrepareScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub PrepareUIInitiating()
        Try
            Me.Cursor = Cursors.WaitCursor

            Me.BsEditButton.Enabled = False
            Me.BsSaveButton.Enabled = False
            Me.BsCancelButton.Enabled = False

            Me.BsCheckButton.Enabled = False

            Me.BsImportButton.Enabled = False
            Me.BsExportButton.Enabled = False

            Me.BsAddButton.Enabled = False
            Me.BsRemoveButton.Enabled = False

            Me.BsMoveUpButton.Enabled = False
            Me.BsMoveDownButton.Enabled = False

            Me.BsNameTextBox.Enabled = False
            Me.BsDescriptionTextBox.Enabled = False
            Me.BsModifiedTextBox.Enabled = False
            Me.BsCreatedTextBox.Enabled = False
            Me.BsAuthorTextBox.Enabled = False

            Me.bsInstructionsListDataGridView.Enabled = False
            Me.BsScreenActionsTreeView.Enabled = False

            Me.BsImportAllButton.Enabled = False
            Me.BsRestoreButton.Enabled = False
            Me.BsExportAllToFileButton.Enabled = False
            Me.BsSaveAllButton.Enabled = False

            Me.BSAnalyzerCombo.Enabled = False

            Me.BsTestButton.Enabled = False
            Me.BsUnTestButton.Enabled = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUIInitiating", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUIInitiating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    Private Sub PrepareUIInitiated()
        Try
            Me.Cursor = Cursors.Default

            Me.BsEditButton.Enabled = False
            Me.BsSaveButton.Enabled = False
            Me.BsCancelButton.Enabled = False

            Me.BsCheckButton.Enabled = False

            Me.BsImportButton.Enabled = False
            Me.BsExportButton.Enabled = False

            Me.BsAddButton.Enabled = False
            Me.BsRemoveButton.Enabled = False

            Me.BsMoveUpButton.Enabled = False
            Me.BsMoveDownButton.Enabled = False

            Me.BsNameTextBox.Enabled = False
            Me.BsDescriptionTextBox.Enabled = False
            Me.BsModifiedTextBox.Enabled = False
            Me.BsCreatedTextBox.Enabled = False
            Me.BsAuthorTextBox.Enabled = False

            Me.bsInstructionsListDataGridView.Enabled = False

            Me.BsScreenActionsTreeView.Enabled = True

            Me.BsImportAllButton.Enabled = True
            Me.BsRestoreButton.Enabled = True
            Me.BsExportAllToFileButton.Enabled = True
            Me.BsSaveAllButton.Enabled = False

            Me.BSAnalyzerCombo.Enabled = False

            Me.BsTestButton.Enabled = False
            Me.BsUnTestButton.Enabled = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUIInitiated", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUIInitiated", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    Private Sub PrepareUILoading()
        Try
            Me.Cursor = Cursors.WaitCursor

            Me.BsEditButton.Enabled = False
            Me.BsSaveButton.Enabled = False
            Me.BsCancelButton.Enabled = False

            Me.BsCheckButton.Enabled = False

            Me.BsImportButton.Enabled = False
            Me.BsExportButton.Enabled = False

            Me.BsAddButton.Enabled = False
            Me.BsRemoveButton.Enabled = False

            Me.BsMoveUpButton.Enabled = False
            Me.BsMoveDownButton.Enabled = False

            Me.BsNameTextBox.Enabled = False
            Me.BsDescriptionTextBox.Enabled = False
            Me.BsModifiedTextBox.Enabled = False
            Me.BsCreatedTextBox.Enabled = False
            Me.BsAuthorTextBox.Enabled = False

            Me.bsInstructionsListDataGridView.Enabled = False
            Me.BsScreenActionsTreeView.Enabled = False

            Me.BsImportAllButton.Enabled = False
            Me.BsRestoreButton.Enabled = False
            Me.BsExportAllToFileButton.Enabled = False
            Me.BsSaveAllButton.Enabled = False

            Me.BSAnalyzerCombo.Enabled = False

            Me.BsTestButton.Enabled = False
            Me.BsUnTestButton.Enabled = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUILoading", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUILoading", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    Private Sub PrepareUILoaded()
        Try
            Me.Cursor = Cursors.Default

            Me.BsEditButton.Enabled = True
            Me.BsSaveButton.Enabled = False
            Me.BsCancelButton.Enabled = False

            Me.BsCheckButton.Enabled = Me.bsInstructionsListDataGridView.Rows.Count > 0

            Me.BsImportButton.Enabled = False
            Me.BsExportButton.Enabled = MyClass.IsSyntaxOK And (Me.bsInstructionsListDataGridView.Rows.Count > 0)

            Me.BsAddButton.Enabled = False
            Me.BsRemoveButton.Enabled = False

            Me.BsMoveUpButton.Enabled = False
            Me.BsMoveDownButton.Enabled = False

            Me.BsNameTextBox.ReadOnly = True
            Me.BsDescriptionTextBox.ReadOnly = True
            Me.BsModifiedTextBox.ReadOnly = True
            Me.BsCreatedTextBox.ReadOnly = True
            Me.BsAuthorTextBox.ReadOnly = True

            Me.BsNameTextBox.Enabled = True
            Me.BsDescriptionTextBox.Enabled = True
            Me.BsModifiedTextBox.Enabled = True
            Me.BsCreatedTextBox.Enabled = True
            Me.BsAuthorTextBox.Enabled = True

            Me.BsScreenActionsTreeView.Enabled = True

            Me.BsImportAllButton.Enabled = True
            Me.BsRestoreButton.Enabled = True
            Me.BsExportAllToFileButton.Enabled = True

            'ONLY IF ALL ARE TESTED OK!!!
            Me.BsSaveAllButton.Enabled = Me.GlobalChangesMade And Me.CurrentFwScriptsData.AllTestedOK

            Me.BSAnalyzerCombo.Enabled = False


            With Me.bsInstructionsListDataGridView
                .ReadOnly = True
                .Enabled = True
                .DefaultCellStyle.BackColor = Color.White
                .DefaultCellStyle.ForeColor = Color.Black
                .DefaultCellStyle.SelectionBackColor = Color.SlateGray
                .DefaultCellStyle.SelectionForeColor = Color.White
            End With

            Me.bsInstructionsListDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect

            Me.BsTestButton.Enabled = Ax00ServiceMainMDI.MDIAnalyzerManager.Connected And _
                                        Not MyClass.SimulationMode And _
                                        (Me.bsInstructionsListDataGridView.Rows.Count > 0) And _
                                        MyClass.IsSyntaxOK

            If MyClass.SelectedFwScript IsNot Nothing Then
                Me.BsUnTestButton.Enabled = Me.BsTestButton.Enabled And MyClass.SelectedFwScript.TestedOK
            End If


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUILoaded", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUILoaded", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    Private Sub PrepareUIModifying()
        Try
            Me.Cursor = Cursors.Default

            MyClass.IsLocallySaved = False

            Me.BsEditButton.Enabled = False
            Me.BsSaveButton.Enabled = False
            Me.BsCancelButton.Enabled = True

            Me.BsCheckButton.Enabled = Me.bsInstructionsListDataGridView.Rows.Count > 0

            Me.BsImportButton.Enabled = True
            Me.BsExportButton.Enabled = MyClass.IsSyntaxOK And (Me.bsInstructionsListDataGridView.Rows.Count > 0)

            Me.BsAddButton.Enabled = True
            Me.BsRemoveButton.Enabled = Me.bsInstructionsListDataGridView.Rows.Count > 0


            Me.BsNameTextBox.ReadOnly = False
            Me.BsDescriptionTextBox.ReadOnly = False
            Me.BsModifiedTextBox.ReadOnly = False
            Me.BsCreatedTextBox.ReadOnly = False
            Me.BsAuthorTextBox.ReadOnly = False

            Me.BsNameTextBox.Enabled = True
            Me.BsDescriptionTextBox.Enabled = True
            Me.BsModifiedTextBox.Enabled = True
            Me.BsCreatedTextBox.Enabled = True
            Me.BsAuthorTextBox.Enabled = True

            Me.BsScreenActionsTreeView.Enabled = True

            Me.BsImportAllButton.Enabled = False
            Me.BsRestoreButton.Enabled = False
            Me.BsExportAllToFileButton.Enabled = False
            Me.BsSaveAllButton.Enabled = False

            Me.BSAnalyzerCombo.Enabled = True


            Dim myRow As DataGridViewRow = Me.bsInstructionsListDataGridView.CurrentRow
            If myRow IsNot Nothing Then
                Me.BsMoveUpButton.Enabled = (myRow.Index > 0)
                Me.BsMoveDownButton.Enabled = (myRow.Index < Me.bsInstructionsListDataGridView.Rows.Count - 1)
            Else
                Me.BsMoveUpButton.Enabled = False
                Me.BsMoveDownButton.Enabled = False
            End If

            With Me.bsInstructionsListDataGridView
                .ReadOnly = False
                .Enabled = True
                .DefaultCellStyle.BackColor = Color.White
                .DefaultCellStyle.ForeColor = Color.Black
                .DefaultCellStyle.SelectionBackColor = Color.WhiteSmoke
                .DefaultCellStyle.SelectionForeColor = Color.DimGray
            End With

            For Each R As DataGridViewRow In Me.bsInstructionsListDataGridView.Rows

                R.Cells("SyntaxOK").ReadOnly = True
                R.Cells("TestedOK").ReadOnly = True
                If CBool(R.Cells("EnableEdition").Value) Then
                    R.Cells("Sequence").ReadOnly = False
                    R.Cells("Code").ReadOnly = False
                    R.Cells("Params").ReadOnly = False
                Else
                    R.Cells("Sequence").ReadOnly = True
                    R.Cells("Code").ReadOnly = True
                    R.Cells("Params").ReadOnly = True
                    R.Cells("TestedOK").ReadOnly = True
                End If

            Next R

            Me.bsInstructionsListDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect

            Me.BsTestButton.Enabled = False
            Me.BsUnTestButton.Enabled = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUIModifying", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUIModifying", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    Private Sub PrepareUIModified()
        Try
            Me.Cursor = Cursors.Default

            Me.BsEditButton.Enabled = False
            Me.BsSaveButton.Enabled = MyClass.IsSyntaxOK
            Me.BsCancelButton.Enabled = True

            Me.BsCheckButton.Enabled = Me.bsInstructionsListDataGridView.Rows.Count > 0

            Me.BsImportButton.Enabled = True
            Me.BsExportButton.Enabled = MyClass.IsSyntaxOK And (Me.bsInstructionsListDataGridView.Rows.Count > 0)

            Me.BsAddButton.Enabled = True
            Me.BsRemoveButton.Enabled = Me.bsInstructionsListDataGridView.Rows.Count > 0

            If Me.bsInstructionsListDataGridView.SelectedCells.Count > 0 Then
                Dim myCell As DataGridViewCell = Me.bsInstructionsListDataGridView.SelectedCells(0)
                Me.BsMoveUpButton.Enabled = (myCell.RowIndex > 0)
                Me.BsMoveDownButton.Enabled = (myCell.RowIndex < Me.bsInstructionsListDataGridView.Rows.Count - 1)
            Else
                Me.BsMoveUpButton.Enabled = False
                Me.BsMoveDownButton.Enabled = False
            End If

            Me.BsNameTextBox.ReadOnly = False
            Me.BsDescriptionTextBox.ReadOnly = False
            Me.BsModifiedTextBox.ReadOnly = False
            Me.BsCreatedTextBox.ReadOnly = False
            Me.BsAuthorTextBox.ReadOnly = False

            Me.BsNameTextBox.Enabled = True
            Me.BsDescriptionTextBox.Enabled = True
            Me.BsModifiedTextBox.Enabled = True
            Me.BsCreatedTextBox.Enabled = True
            Me.BsAuthorTextBox.Enabled = True

            Me.bsInstructionsListDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect
            Me.bsInstructionsListDataGridView.ReadOnly = False
            Me.bsInstructionsListDataGridView.Enabled = True
            For Each R As DataGridViewRow In Me.bsInstructionsListDataGridView.Rows
                R.Cells("SyntaxOK").ReadOnly = True
                R.Cells("TestedOK").ReadOnly = True
                If CBool(R.Cells("EnableEdition").Value) Then
                    R.Cells("Sequence").ReadOnly = False
                    R.Cells("Code").ReadOnly = False
                    R.Cells("Params").ReadOnly = False
                Else
                    R.Cells("Sequence").ReadOnly = True
                    R.Cells("Code").ReadOnly = True
                    R.Cells("Params").ReadOnly = True
                    R.Cells("TestedOK").ReadOnly = True
                End If
            Next R

            Me.BsScreenActionsTreeView.Enabled = True

            Me.BsImportAllButton.Enabled = False
            Me.BsRestoreButton.Enabled = False
            Me.BsExportAllToFileButton.Enabled = False
            Me.BsSaveAllButton.Enabled = False

            Me.BSAnalyzerCombo.Enabled = False

            Me.BsTestButton.Enabled = False
            Me.BsUnTestButton.Enabled = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUIModified", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUIModified", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    Private Sub PrepareUIGridChanging()
        Try
            Me.Cursor = Cursors.WaitCursor

            Me.BsEditButton.Enabled = False
            Me.BsSaveButton.Enabled = False
            Me.BsCancelButton.Enabled = False

            Me.BsCheckButton.Enabled = False

            Me.BsImportButton.Enabled = False
            Me.BsExportButton.Enabled = False

            Me.BsAddButton.Enabled = False
            Me.BsRemoveButton.Enabled = False

            Me.BsMoveUpButton.Enabled = False
            Me.BsMoveDownButton.Enabled = False

            Me.BsNameTextBox.ReadOnly = False
            Me.BsDescriptionTextBox.ReadOnly = False
            Me.BsModifiedTextBox.ReadOnly = False
            Me.BsCreatedTextBox.ReadOnly = False
            Me.BsAuthorTextBox.ReadOnly = False

            Me.BsNameTextBox.Enabled = False
            Me.BsDescriptionTextBox.Enabled = False
            Me.BsModifiedTextBox.Enabled = False
            Me.BsCreatedTextBox.Enabled = False
            Me.BsAuthorTextBox.Enabled = False

            Me.bsInstructionsListDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect
            Me.bsInstructionsListDataGridView.ReadOnly = True
            Me.bsInstructionsListDataGridView.Enabled = True

            Me.BsScreenActionsTreeView.Enabled = False

            Me.BsImportAllButton.Enabled = False
            Me.BsRestoreButton.Enabled = False
            Me.BsExportAllToFileButton.Enabled = False
            Me.BsSaveAllButton.Enabled = False

            Me.BSAnalyzerCombo.Enabled = False

            Me.BsTestButton.Enabled = False
            Me.BsUnTestButton.Enabled = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUIGridChanging", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUIGridChanging", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    Private Sub PrepareUILocalSaving()
        Try
            Me.Cursor = Cursors.WaitCursor

            Me.BsEditButton.Enabled = False
            Me.BsSaveButton.Enabled = False
            Me.BsCancelButton.Enabled = False

            Me.BsCheckButton.Enabled = False

            Me.BsImportButton.Enabled = False
            Me.BsExportButton.Enabled = False

            Me.BsAddButton.Enabled = False
            Me.BsRemoveButton.Enabled = False

            Me.BsMoveUpButton.Enabled = False
            Me.BsMoveDownButton.Enabled = False

            Me.BsNameTextBox.Enabled = False
            Me.BsDescriptionTextBox.Enabled = False
            Me.BsModifiedTextBox.Enabled = False
            Me.BsCreatedTextBox.Enabled = False
            Me.BsAuthorTextBox.Enabled = False

            Me.bsInstructionsListDataGridView.Enabled = False
            Me.BsScreenActionsTreeView.Enabled = False

            Me.BsImportAllButton.Enabled = False
            Me.BsRestoreButton.Enabled = False
            Me.BsExportAllToFileButton.Enabled = False
            Me.BsSaveAllButton.Enabled = False

            Me.BSAnalyzerCombo.Enabled = False

            Me.BsTestButton.Enabled = False
            Me.BsUnTestButton.Enabled = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUILocalSaving", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUILocalSaving", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    Private Sub PrepareUIAllImporting()
        Try
            Me.Cursor = Cursors.WaitCursor

            Me.BsEditButton.Enabled = False
            Me.BsSaveButton.Enabled = False
            Me.BsCancelButton.Enabled = False

            Me.BsCheckButton.Enabled = False

            Me.BsImportButton.Enabled = False
            Me.BsExportButton.Enabled = False

            Me.BsAddButton.Enabled = False
            Me.BsRemoveButton.Enabled = False

            Me.BsMoveUpButton.Enabled = False
            Me.BsMoveDownButton.Enabled = False

            Me.BsNameTextBox.Enabled = False
            Me.BsDescriptionTextBox.Enabled = False
            Me.BsModifiedTextBox.Enabled = False
            Me.BsCreatedTextBox.Enabled = False
            Me.BsAuthorTextBox.Enabled = False

            Me.bsInstructionsListDataGridView.Enabled = False
            Me.BsScreenActionsTreeView.Enabled = False

            Me.BsImportAllButton.Enabled = False
            Me.BsRestoreButton.Enabled = False
            Me.BsExportAllToFileButton.Enabled = False
            Me.BsSaveAllButton.Enabled = False

            Me.BSAnalyzerCombo.Enabled = False

            Me.BsTestButton.Enabled = False
            Me.BsUnTestButton.Enabled = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUIAllImporting", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUIAllImporting", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    Private Sub PrepareUIScriptChecking()
        Try

            Me.Cursor = Cursors.WaitCursor

            Me.BsEditButton.Enabled = False
            Me.BsSaveButton.Enabled = False
            Me.BsCancelButton.Enabled = False

            Me.BsCheckButton.Enabled = False

            Me.BsImportButton.Enabled = False
            Me.BsExportButton.Enabled = False

            Me.BsAddButton.Enabled = False
            Me.BsRemoveButton.Enabled = False

            Me.BsMoveUpButton.Enabled = False
            Me.BsMoveDownButton.Enabled = False

            Me.BsNameTextBox.Enabled = False
            Me.BsDescriptionTextBox.Enabled = False
            Me.BsModifiedTextBox.Enabled = False
            Me.BsCreatedTextBox.Enabled = False
            Me.BsAuthorTextBox.Enabled = False

            Me.bsInstructionsListDataGridView.Enabled = False
            Me.BsScreenActionsTreeView.Enabled = False

            Me.BsImportAllButton.Enabled = False
            Me.BsRestoreButton.Enabled = False
            Me.BsExportAllToFileButton.Enabled = False
            Me.BsSaveAllButton.Enabled = False

            Me.BSAnalyzerCombo.Enabled = False

            Me.BsTestButton.Enabled = False
            Me.BsUnTestButton.Enabled = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUIScriptChecking", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUIScriptChecking", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    Private Sub PrepareUIScriptTesting()
        Try
            Me.Cursor = Cursors.WaitCursor

            Me.BsEditButton.Enabled = False
            Me.BsSaveButton.Enabled = False
            Me.BsCancelButton.Enabled = False

            Me.BsCheckButton.Enabled = False

            Me.BsImportButton.Enabled = False
            Me.BsExportButton.Enabled = False

            Me.BsAddButton.Enabled = False
            Me.BsRemoveButton.Enabled = False

            Me.BsMoveUpButton.Enabled = False
            Me.BsMoveDownButton.Enabled = False

            Me.BsNameTextBox.Enabled = False
            Me.BsDescriptionTextBox.Enabled = False
            Me.BsModifiedTextBox.Enabled = False
            Me.BsCreatedTextBox.Enabled = False
            Me.BsAuthorTextBox.Enabled = False

            Me.bsInstructionsListDataGridView.Enabled = False
            Me.BsScreenActionsTreeView.Enabled = False

            Me.BsImportAllButton.Enabled = False
            Me.BsRestoreButton.Enabled = False
            Me.BsExportAllToFileButton.Enabled = False
            Me.BsSaveAllButton.Enabled = False

            Me.BSAnalyzerCombo.Enabled = False

            Me.BsTestButton.Enabled = False
            Me.BsUnTestButton.Enabled = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUIScriptTesting", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUIScriptTesting", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    Private Sub PrepareUIScriptImporting()
        Try
            Me.Cursor = Cursors.WaitCursor

            Me.BsEditButton.Enabled = False
            Me.BsSaveButton.Enabled = False
            Me.BsCancelButton.Enabled = False

            Me.BsCheckButton.Enabled = False

            Me.BsImportButton.Enabled = False
            Me.BsExportButton.Enabled = False

            Me.BsAddButton.Enabled = False
            Me.BsRemoveButton.Enabled = False

            Me.BsMoveUpButton.Enabled = False
            Me.BsMoveDownButton.Enabled = False

            Me.BsNameTextBox.Enabled = False
            Me.BsDescriptionTextBox.Enabled = False
            Me.BsModifiedTextBox.Enabled = False
            Me.BsCreatedTextBox.Enabled = False
            Me.BsAuthorTextBox.Enabled = False

            Me.bsInstructionsListDataGridView.Enabled = False
            Me.BsScreenActionsTreeView.Enabled = False

            Me.BsImportAllButton.Enabled = False
            Me.BsRestoreButton.Enabled = False
            Me.BsExportAllToFileButton.Enabled = False
            Me.BsSaveAllButton.Enabled = False

            Me.BSAnalyzerCombo.Enabled = False

            Me.BsTestButton.Enabled = False
            Me.BsUnTestButton.Enabled = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUIScriptImporting", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUIScriptImporting", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    Private Sub PrepareUIScriptExporting()
        Try
            Me.Cursor = Cursors.WaitCursor

            Me.BsEditButton.Enabled = False
            Me.BsSaveButton.Enabled = False
            Me.BsCancelButton.Enabled = False

            Me.BsCheckButton.Enabled = False

            Me.BsImportButton.Enabled = False
            Me.BsExportButton.Enabled = False

            Me.BsAddButton.Enabled = False
            Me.BsRemoveButton.Enabled = False

            Me.BsMoveUpButton.Enabled = False
            Me.BsMoveDownButton.Enabled = False

            Me.BsNameTextBox.Enabled = False
            Me.BsDescriptionTextBox.Enabled = False
            Me.BsModifiedTextBox.Enabled = False
            Me.BsCreatedTextBox.Enabled = False
            Me.BsAuthorTextBox.Enabled = False

            Me.bsInstructionsListDataGridView.Enabled = False
            Me.BsScreenActionsTreeView.Enabled = False

            Me.BsImportAllButton.Enabled = False
            Me.BsRestoreButton.Enabled = False
            Me.BsExportAllToFileButton.Enabled = False
            Me.BsSaveAllButton.Enabled = False

            Me.BSAnalyzerCombo.Enabled = False

            Me.BsTestButton.Enabled = False
            Me.BsUnTestButton.Enabled = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUIScriptExporting", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUIScriptExporting", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    Private Sub PrepareUIGlobalSaving()
        Try
            Me.Cursor = Cursors.WaitCursor

            Me.BsEditButton.Enabled = False
            Me.BsSaveButton.Enabled = False
            Me.BsCancelButton.Enabled = False

            Me.BsCheckButton.Enabled = False

            Me.BsImportButton.Enabled = False
            Me.BsExportButton.Enabled = False

            Me.BsAddButton.Enabled = False
            Me.BsRemoveButton.Enabled = False

            Me.BsMoveUpButton.Enabled = False
            Me.BsMoveDownButton.Enabled = False

            Me.BsNameTextBox.Enabled = False
            Me.BsDescriptionTextBox.Enabled = False
            Me.BsModifiedTextBox.Enabled = False
            Me.BsCreatedTextBox.Enabled = False
            Me.BsAuthorTextBox.Enabled = False

            Me.bsInstructionsListDataGridView.Enabled = False
            Me.BsScreenActionsTreeView.Enabled = False

            Me.BsImportAllButton.Enabled = False
            Me.BsRestoreButton.Enabled = False
            Me.BsExportAllToFileButton.Enabled = False
            Me.BsSaveAllButton.Enabled = False

            Me.BSAnalyzerCombo.Enabled = False

            Me.BsTestButton.Enabled = False
            Me.BsUnTestButton.Enabled = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUIGlobalSaving", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & "UIGlobalSaving", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    Private Sub PrepareUIAllRestoring()
        Try
            Me.Cursor = Cursors.WaitCursor

            Me.BsEditButton.Enabled = False
            Me.BsSaveButton.Enabled = False
            Me.BsCancelButton.Enabled = False

            Me.BsCheckButton.Enabled = False

            Me.BsImportButton.Enabled = False
            Me.BsExportButton.Enabled = False

            Me.BsAddButton.Enabled = False
            Me.BsRemoveButton.Enabled = False

            Me.BsMoveUpButton.Enabled = False
            Me.BsMoveDownButton.Enabled = False

            Me.BsNameTextBox.Enabled = False
            Me.BsDescriptionTextBox.Enabled = False
            Me.BsModifiedTextBox.Enabled = False
            Me.BsCreatedTextBox.Enabled = False
            Me.BsAuthorTextBox.Enabled = False

            Me.bsInstructionsListDataGridView.Enabled = False
            Me.BsScreenActionsTreeView.Enabled = False

            Me.BsImportAllButton.Enabled = False
            Me.BsRestoreButton.Enabled = False
            Me.BsExportAllToFileButton.Enabled = False
            Me.BsSaveAllButton.Enabled = False

            Me.BSAnalyzerCombo.Enabled = False

            Me.BsTestButton.Enabled = False
            Me.BsUnTestButton.Enabled = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUIAllRestoring", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUIAllRestoring", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    Private Sub PrepareUIOnError()
        Try
            MyClass.PreviousScreenModeBeforeTesting = ScreenModes._NONE

            Me.BsEditButton.Enabled = False
            Me.BsSaveButton.Enabled = False
            Me.BsCancelButton.Enabled = False

            Me.BsCheckButton.Enabled = False

            Me.BsImportButton.Enabled = False
            Me.BsExportButton.Enabled = False

            Me.BsAddButton.Enabled = False
            Me.BsRemoveButton.Enabled = False


            Me.BsMoveUpButton.Enabled = False
            Me.BsMoveDownButton.Enabled = False


            Me.BsNameTextBox.Enabled = False
            Me.BsDescriptionTextBox.Enabled = False
            Me.BsModifiedTextBox.Enabled = False
            Me.BsCreatedTextBox.Enabled = False
            Me.BsAuthorTextBox.Enabled = False

            Me.bsInstructionsListDataGridView.Enabled = False
            Me.BsScreenActionsTreeView.Enabled = False

            Me.BsImportAllButton.Enabled = False
            Me.BsRestoreButton.Enabled = False
            Me.BsExportAllToFileButton.Enabled = False
            Me.BsSaveAllButton.Enabled = False

            Me.BSAnalyzerCombo.Enabled = False

            Me.BsTestButton.Enabled = False
            Me.BsUnTestButton.Enabled = False

            Me.Cursor = Cursors.Default

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareUIOnError", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareUIOnError", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

#End Region

#Region "Show Methods"

    ''' <summary>
    ''' Show Action Details
    ''' </summary>
    ''' <param name="pActionID"></param>
    ''' <remarks>
    ''' created by XBC 27/09/2010
    ''' modified by XBC 24/11/2010 - myScriptDelegate becomes global into the class instead local
    ''' </remarks>
    Private Function ShowActionFwScript(ByVal pActionID As String) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO
        Dim myPreviousScreenMode As ScreenModes = MyClass.CurrentScreenMode

        Try


            myGlobal = MyClass.myScreenDelegate.GetFwScriptByActionID(pActionID)
            If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                Dim myScript As FwScriptTO = DirectCast(myGlobal.SetDatos, FwScriptTO)
                MyClass.SelectedFwScript = myScript.Clone

                BsNameTextBox.Text = Me.SelectedFwScript.ActionID
                BsDescriptionTextBox.Text = Me.SelectedFwScript.Description

                BsCreatedTextBox.Text = Me.SelectedFwScript.Created.ToString("dd/MM/yy HH:mm")
                BsModifiedTextBox.Text = Me.SelectedFwScript.Modified.ToString("dd/MM/yy HH:mm")
                BsAuthorTextBox.Text = Me.SelectedFwScript.Author

                myGlobal = MyClass.ShowFwScriptInstructions(Me.SelectedFwScript.FwScriptID)
            Else
                MyClass.SelectedFwScript.Instructions = Nothing
            End If

            If Not myGlobal.HasError Then
                MyClass.CurrentScreenMode = myPreviousScreenMode
            Else
                MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
            End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = "SYSTEM_ERROR"
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ShowActionFwScript", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ShowActionFwScript", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return myGlobal

    End Function

    ''' <summary>
    ''' Show Instructions Details of Action (script) selected
    ''' </summary>
    ''' <param name="pFwScriptID"></param>
    ''' <remarks>
    ''' created by XBC 27/09/2010
    ''' modified by XBC 24/11/2010 - myScriptDelegate becomes global into the class instead local
    ''' </remarks>
    Private Function ShowFwScriptInstructions(ByVal pFwScriptID As Integer) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try
            myGlobal = MyClass.myScreenDelegate.GetInstructionsByFwScriptID(pFwScriptID)
            If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                Dim myInstructionsDetails As List(Of InstructionTO) = DirectCast(myGlobal.SetDatos, List(Of InstructionTO))

                myGlobal = MyClass.ShowInstructions(myInstructionsDetails)
            Else
                MyBase.ShowMessage(Me.Name & ".ShowInstructions", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = "SYSTEM_ERROR"
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ShowFwScriptInstructions", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ShowFwScriptInstructions", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return myGlobal

    End Function

    ''' <summary>
    ''' Show Instructions Details
    ''' </summary>
    ''' <param name="pInstructions"></param>
    ''' <remarks>
    ''' created by SG 28/09/2010
    ''' modified by XBC 29/09/2010 - initialize with Clearing rows
    ''' Modified by XBC 17/11/2010 - add EnableEdition 
    ''' </remarks>
    Private Function ShowInstructions(ByVal pInstructions As List(Of InstructionTO)) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Dim cont As Integer = 0
        Try
            MyClass.CurrentScreenMode = ScreenModes.LOADING

            bsInstructionsListDataGridView.Visible = False
            bsInstructionsListDataGridView.Rows.Clear()
            bsInstructionsListDataGridView.ScrollBars = ScrollBars.None

            If pInstructions IsNot Nothing Then
                If pInstructions.Count > 0 Then
                    If MyClass.CurrentScreenMode <> ScreenModes.LOADING Then
                        MyClass.CurrentScreenMode = ScreenModes.LOADING
                    End If
                    For Each Instr As InstructionTO In pInstructions
                        bsInstructionsListDataGridView.Rows.Add()
                        Dim myRow As DataGridViewRow = bsInstructionsListDataGridView.Rows(cont)
                        myRow.Cells("InstructionID").Value = Instr.InstructionID.ToString
                        myRow.Cells("Sequence").Value = Instr.Sequence.ToString
                        myRow.Cells("Timer").Value = Instr.Timer.ToString.PadLeft(6, CChar("0"))
                        myRow.Cells("Code").Value = Instr.Code.ToString
                        myRow.Cells("Params").Value = Instr.Params.ToString
                        myRow.Cells("SyntaxOK").Value = False
                        myRow.Cells("TestedOK").Value = False
                        myRow.Cells("EnableEdition").Value = Instr.EnableEdition.ToString
                        cont += 1 
                    Next
                End If

                MyClass.SelectedFwScript.Instructions = pInstructions

                myGlobal = MyClass.CheckGrid()

                bsInstructionsListDataGridView.ScrollBars = ScrollBars.Vertical
                bsInstructionsListDataGridView.Visible = True
                bsInstructionsListDataGridView.Refresh()


            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ShowInstructions", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ShowInstructions", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            If Not myGlobal.HasError Then
                MyClass.CurrentScreenMode = ScreenModes.LOADED
            Else
                MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
            End If

            bsInstructionsListDataGridView.Visible = True

        End Try

        Return myGlobal

    End Function

#End Region

#Region "Import Export Methods"

    ''' <summary>
    ''' Export to an external file the current script
    ''' </summary>
    ''' <param name="pFwScriptID">identifier of the script</param>
    ''' <remarks>created by SG 28/09/25010</remarks>
    Private Function ExportCurrentFwScript(ByVal pFwScriptID As Integer) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Dim dlg As New SaveFileDialog()

        Try
            'SG 30/09/10
            If Me.SelectedFwScript.Instructions IsNot Nothing Then
                myGlobal = MyClass.CheckGrid(True)
                If (Not myGlobal.HasError) Then
                    If SelectedFwScript.SyntaxOK Then 'END 'SG 30/09/10
                        dlg.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"
                        dlg.FilterIndex = 1
                        dlg.CheckPathExists = True
                        dlg.DefaultExt = ".txt"
                        dlg.InitialDirectory = Environment.SpecialFolder.MyDocuments.ToString
                        dlg.Title = GetMessageText(GlobalEnumerates.Messages.FWSCRIPT_TEXT_EXPORT.ToString)
                        dlg.FileName = Me.SelectedFwScript.ActionID & ".txt"

                        If dlg.ShowDialog() = DialogResult.OK Then
                            Dim myExportImportDelegate As New ExportImportDelegate(Me.CurrentFwScriptsData)
                            myGlobal = myExportImportDelegate.ExportFwScript(pFwScriptID, dlg.FileName)
                            If Not myGlobal.HasError And Not myGlobal Is Nothing Then
                                MyBase.ShowMessage(Me.Name, GlobalEnumerates.Messages.FWSCRIPT_TEXT_EXPORT_OK.ToString, , Me)
                            Else
                                MessageBox.Show(myGlobal.ErrorMessage)
                                MyBase.ShowMessage(Me.Name, GlobalEnumerates.Messages.FWSCRIPT_TEXT_EXPORT_ERROR.ToString, , Me)
                            End If
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ExportCurrentFwScript", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ExportCurrentFwScript", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Imports from an external file the current script
    ''' </summary>
    ''' <remarks>created by SG 28/09/25010</remarks>
    Private Function ImportCurrentFwScript() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Dim dlg As New OpenFileDialog()

        Try
            dlg.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"
            dlg.FilterIndex = 1
            dlg.CheckPathExists = True
            dlg.DefaultExt = ".txt"
            dlg.InitialDirectory = Environment.SpecialFolder.MyDocuments.ToString
            dlg.Title = GetMessageText(GlobalEnumerates.Messages.FWSCRIPT_TEXT_IMPORT.ToString)
            dlg.Multiselect = False

            If dlg.ShowDialog() = DialogResult.OK Then
                Dim myNewActionID As String = dlg.SafeFileName.Substring(0, dlg.SafeFileName.IndexOf("."))

                'myGlobal = CheckActionIDExists(myNewActionID)
                'If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                '    If CBool(myGlobal.SetDatos) Then
                '        myGlobal.ErrorMessage = "Action ID Already Exists" 
                '        myGlobal.HasError = True
                '    End If
                'End If

                If Not myGlobal.HasError And Not myGlobal Is Nothing Then
                    Dim myExportImportDelegate As New ExportImportDelegate(Me.CurrentFwScriptsData)
                    myGlobal = myExportImportDelegate.ImportFwScript(dlg.FileName, Me.CurrentFwScriptsData)

                    If myGlobal.HasError Then
                        If myGlobal.SetDatos IsNot Nothing Then

                            If myGlobal.ErrorCode = GlobalEnumerates.CHECK_RESULTS.SEQUENCE_ERROR.ToString Or _
                            myGlobal.ErrorCode = GlobalEnumerates.CHECK_RESULTS.SYNTAX_ERROR.ToString Then
                                myGlobal.HasError = False
                                myGlobal.ErrorMessage = ""
                                myGlobal.ErrorCode = ""
                            End If
                        End If

                    End If

                    If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                        'update the script
                        Dim myChangedFwScript As New FwScriptTO
                        With myChangedFwScript
                            .Instructions = CType(myGlobal.SetDatos, List(Of InstructionTO))
                            '.ActionID = myNewActionID
                            .Description = "Imported from Text File"
                            Dim myAuthorID As String = ""
                            myGlobal = GetCurrentAuthorID()
                            If Not myGlobal.HasError And Not myGlobal Is Nothing Then
                                myAuthorID = CStr(myGlobal.SetDatos)
                                Me.BsAuthorTextBox.Text = myAuthorID
                            End If
                            .Author = myAuthorID
                            .Created = DateTime.Now
                            .Modified = DateTime.Now
                            .SyntaxOK = True
                            .TestedOK = False
                        End With

                        myGlobal.SetDatos = myChangedFwScript
                        myGlobal.HasError = False


                    End If
                End If
            End If
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ImportCurrentFwScript", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ImportCurrentFwScript", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return myGlobal

    End Function

#End Region

#Region "Check Methods"

    Private Function CheckGrid(Optional ByVal pShowMessage As Boolean = False) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO
        Dim CopyOfSelectedScript As FwScriptTO = Nothing
        Dim SyntaxOK As Boolean = True
        Dim SequenceOK As Boolean = True
        Dim myPreviousScreenMode As ScreenModes = MyClass.CurrentScreenMode

        Try

            MyClass.CurrentScreenMode = ScreenModes.SCRIPT_CHECKING

            MyClass.ValidationError = False

            CopyOfSelectedScript = Me.SelectedFwScript

            myGlobal = MyClass.ValidateCellsEditionChanges
            If Not myGlobal.HasError And Not MyClass.ValidationError Then
                myGlobal = MyClass.CheckGridSyntax
                If Not myGlobal.HasError Then
                    SyntaxOK = CBool(myGlobal.SetDatos)
                End If

                'If SyntaxOK Then

                myGlobal = MyClass.CheckGridSequence
                If Not myGlobal.HasError Then

                    SequenceOK = CBool(myGlobal.SetDatos)

                    If SequenceOK Then
                        If Me.RowAddingMode Then
                            Dim isNewAdded As Boolean = False
                            For Each I As InstructionTO In MyClass.SelectedFwScript.Instructions
                                If Not CopyOfSelectedScript.Instructions.Contains(I) Then
                                    isNewAdded = True
                                    MyClass.SelectedFwScript.SyntaxOK = True
                                    MyClass.SelectedFwScript.TestedOK = False
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                End If

                'End If

            Else

                SyntaxOK = False

                myGlobal = MyClass.CheckGridSequence
                If Not myGlobal.HasError Then
                    SequenceOK = CBool(myGlobal.SetDatos)
                End If

            End If

            MyClass.SelectedFwScript.SyntaxOK = SyntaxOK And SequenceOK

            'display results
            myGlobal = MyClass.DisplayGridResults(SyntaxOK, SequenceOK)

            If pShowMessage Then
                If Not SyntaxOK Then
                    MyBase.ShowMessage(Me.Name, GlobalEnumerates.Messages.FWSCRIPT_SYNTAX_ERROR.ToString, , Me)
                ElseIf Not SequenceOK Then
                    MyBase.ShowMessage(Me.Name, GlobalEnumerates.Messages.FWSCRIPT_SEQUENCE_ERROR.ToString, , Me)
                Else

                End If
            End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = "SYSTEM_ERROR"
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".CheckGrid", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".CheckGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)


            If CopyOfSelectedScript IsNot Nothing Then
                Me.SelectedFwScript = CopyOfSelectedScript
            End If
        End Try

        If Not myGlobal.HasError Then
            myGlobal.SetDatos = SyntaxOK And SequenceOK
            MyClass.CurrentScreenMode = myPreviousScreenMode
        Else
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End If

        Return myGlobal

    End Function

    Private Function CheckGridSyntax() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            Dim mySyntaxOK As Boolean = True

            For Each dgv As DataGridViewRow In Me.bsInstructionsListDataGridView.Rows
                Dim myInstructionTxt As String = ""

                Dim myTimer As String = ""
                Dim myCode As String = ""
                Dim myParams As String = ""

                If Not dgv.Cells("Timer").Value Is Nothing Then
                    myTimer = CStr(dgv.Cells("Timer").Value).Trim
                End If
                If Not dgv.Cells("Code").Value Is Nothing Then
                    myCode = CStr(dgv.Cells("Code").Value).Trim
                End If
                If Not dgv.Cells("Params").Value Is Nothing Then
                    myParams = CStr(dgv.Cells("Params").Value).Trim
                End If

                ' Params maybe length = 0
                'If myTimer.Length > 0 And myCode.Length > 0 And myParams.Length > 0 Then
                If myTimer.Length > 0 And myCode.Length > 0 Then

                    myInstructionTxt &= myTimer & ":"
                    If myParams.Length = 0 Then
                        myInstructionTxt &= myCode
                    Else
                        myInstructionTxt &= myCode & ":"
                    End If
                    myInstructionTxt &= myParams & ";"

                    'Dim myCommDelegate As New ScriptsEditionDelegate(New ScriptsDataTO)
                    myGlobal = MyClass.myScreenDelegate.CheckFwScriptInstruction(myInstructionTxt)
                    If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                        mySyntaxOK = CBool(myGlobal.SetDatos)
                    Else
                        mySyntaxOK = False
                    End If

                Else
                    mySyntaxOK = False
                End If

            Next

            myGlobal.SetDatos = mySyntaxOK

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = "SYSTEM_ERROR"
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".CheckGridSyntax", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".CheckGridSyntax", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    Private Function CheckGridSequence() As GlobalDataTO

        Dim myGlobal As New GlobalDataTO
        Dim mySequenceOK As Boolean = True

        Try

            Dim myInstructions As New List(Of InstructionTO)

            'transform the grid to Instructions List
            myGlobal = MyClass.ConvertGridToScript
            If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then

                myInstructions = CType(myGlobal.SetDatos, List(Of InstructionTO))

                'check the sequence
                myGlobal = MyClass.myScreenDelegate.CheckFwScript(myInstructions)
                If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                    If CBool(myGlobal.SetDatos) Then
                        mySequenceOK = True
                        MyClass.SelectedFwScript.Instructions = myInstructions
                    Else
                        mySequenceOK = False
                    End If
                Else
                    mySequenceOK = False
                End If

            Else
                mySequenceOK = False
            End If

            myGlobal.HasError = False
            myGlobal.SetDatos = mySequenceOK

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = "SYSTEM_ERROR"
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".CheckGridSequence", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".CheckGridSequence", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    Private Function DisplayGridResults(ByVal pSyntaxOK As Boolean, ByVal pSequenceOK As Boolean) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            For Each C As DataGridViewCell In Me.bsInstructionsListDataGridView.SelectedCells
                If pSyntaxOK Then
                    C.Style.ForeColor = Color.Black
                    C.Style.SelectionForeColor = Color.White
                Else
                    If C.ErrorText.Length > 0 Then
                        C.Style.ForeColor = Color.Red
                        C.Style.SelectionForeColor = Color.Red
                    Else
                        C.Style.ForeColor = Color.Black
                        C.Style.SelectionForeColor = Color.White
                    End If
                End If
            Next

            For Each dgr As DataGridViewRow In Me.bsInstructionsListDataGridView.Rows

                ''SYNTAX
                dgr.Cells("SyntaxOK").Value = pSyntaxOK

                ''TESTED
                dgr.Cells("TestedOK").Value = MyClass.SelectedFwScript.TestedOK



                'SEQUENCE
                For Each dgv As DataGridViewRow In Me.bsInstructionsListDataGridView.Rows
                    If dgv.Cells("Timer").Value IsNot Nothing Then
                        If pSequenceOK Then
                            dgv.Cells("Timer").Value = CStr(dgv.Cells("Timer").Value).PadLeft(6, CChar("0"))
                            dgv.Cells("Timer").Style.ForeColor = Color.Black
                            dgv.Cells("Timer").Style.SelectionForeColor = Color.White
                            dgv.Cells("Timer").ErrorText = ""
                        Else
                            dgv.Cells("Timer").Style.ForeColor = Color.Red
                            dgv.Cells("Timer").Style.SelectionForeColor = Color.Red
                            dgv.Cells("Timer").ErrorText = GetMessageText(GlobalEnumerates.Messages.FWSCRIPT_SEQUENCE_ERROR.ToString)
                        End If
                    End If
                Next

                For Each R As DataGridViewRow In Me.bsInstructionsListDataGridView.Rows
                    R.Cells("SyntaxOK").Value = pSequenceOK
                    If CBool(dgr.Cells("TestedOK").Value) Then
                        R.Cells("TestedOK").Value = pSyntaxOK And pSequenceOK
                    End If
                Next


            Next

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = "SYSTEM_ERROR"
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".DisplayGridResults", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DisplayGridResults", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Validates any change in the grid's cells
    ''' </summary>
    ''' <remarks>
    ''' Created by SG
    ''' Modified by XB 25/01/2011
    '''             XB 04/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
    ''' </remarks>
    Private Function ValidateGridCell(ByVal pCell As DataGridViewCell) As GlobalDataTO
        Dim ErrorMsg As String = ""
        Dim myGlobal As New GlobalDataTO
        Dim myColName As String = ""

        Try


            'extract cel content
            Dim CellContent As String = ""
            If Not pCell Is Nothing Then

                myGlobal = MyClass.GetCellColumnName(pCell)
                If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                    myColName = CType(myGlobal.SetDatos, String)

                    If pCell.Value IsNot Nothing Then
                        CellContent = CStr(pCell.Value)
                    Else
                        CellContent = ""
                    End If
                End If

            Else
                myGlobal.HasError = True
                Exit Try
            End If

            If CellContent.Contains(":") Then
                ErrorMsg = GlobalEnumerates.Messages.FWSCRIPT_EDIT_NO_SEPARATOR.ToString  'PG 15/11/2010 "do not set separator(:)"
                Exit Try
            ElseIf CellContent.Contains(";") Then
                ErrorMsg = GlobalEnumerates.Messages.FWSCRIPT_EDIT_NO_ENDER.ToString 'PG 15/11/2010 "do not set ender(;)"
                Exit Try
            End If




            Select Case myColName
                Case "Sequence"
                    If CellContent.Contains(".") Then
                        ErrorMsg = GlobalEnumerates.Messages.FWSCRIPT_EDIT_NO_PARAMETER_SEP.ToString
                        Exit Try
                    End If

                Case "Timer"
                    Try
                        Dim myNum As Double = CDbl(pCell.Value)
                        pCell.Value = CStr(pCell.Value).PadLeft(6, CChar("0")).Trim
                    Catch ex As Exception
                        ErrorMsg = GlobalEnumerates.Messages.FWSCRIPT_EDIT_NUMERIC_ONLY.ToString
                        Exit Try
                    End Try


                    If CellContent.Contains(".") Then
                        ErrorMsg = GlobalEnumerates.Messages.FWSCRIPT_EDIT_NO_PARAMETER_SEP.ToString
                        Exit Try
                    End If

                Case "Params" 'SGM 22/10/2011
                    Try
                        Dim myPar As String = CStr(pCell.Value)

                        'sgm 05/12/2011
                        If myPar.Length > 0 Then
                            Dim myPars() As String = myPar.Split(CChar("."))
                            For p As Integer = 0 To myPars.Length - 1
                                Dim Par As String = myPars(p).Trim
                                If Par.StartsWith("#") And Par.EndsWith("@") Then

                                    Par = Par.Substring(1).Replace("@", "")
                                    If Not IsNumeric(Par) Then
                                        Throw New Exception
                                    End If

                                ElseIf Par.Length > 0 Then
                                    If Not IsNumeric(myPar) Then
                                        Throw New Exception
                                    End If
                                End If

                            Next
                        End If

                    Catch ex As Exception
                        ErrorMsg = GlobalEnumerates.Messages.FWSCRIPT_EDIT_NUMERIC_ONLY.ToString
                        Exit Try
                    End Try

                    'For Each R As DataGridViewRow In Me.bsInstructionsListDataGridView.Rows
                    '    If R.Index < myCell.RowIndex Then
                    '        If CDbl(R.Cells("Timer").Value) >= CDbl(myCell.Value) Then
                    '            ErrorMsg = GlobalEnumerates.Messages.FWSCRIPT_EDIT_FOLLOW_SEQUENCE.ToString  'PG 15/11/2010 "Follow the sequence!"
                    '            Exit Try
                    '        End If
                    '    End If
                    '    If R.Index > myCell.RowIndex Then
                    '        If CDbl(R.Cells("Timer").Value) <= CDbl(myCell.Value) Then
                    '            ErrorMsg = GlobalEnumerates.Messages.FWSCRIPT_EDIT_FOLLOW_SEQUENCE.ToString  'PG 15/11/2010 "Follow the sequence!"
                    '            Exit Try
                    '        End If
                    '    End If
                    'Next

                Case "Code"
                    Dim codeItems As String() = CellContent.Split(CChar("."))
                    For c As Integer = 0 To codeItems.Length - 1 Step 1
                        If codeItems(c).Length >= 1 Then
                            If IsNumeric(codeItems(c).First) Then
                                ErrorMsg = GlobalEnumerates.Messages.FWSCRIPT_SYNTAX_ERROR.ToString
                                Exit Try
                            End If
                        End If
                    Next

            End Select

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ValidateGridCell", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ValidateGridCell", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            Return myGlobal
        End Try

        Try
            'second format
            If ErrorMsg = "" Then
                Select Case myColName
                    Case "Timer"
                        If Not pCell.Value Is Nothing Then
                            pCell.Value = CStr(pCell.Value).PadLeft(6, CChar("0")).Trim
                        End If

                    Case "Code"
                        If Not pCell.Value Is Nothing Then
                            pCell.Value = CStr(pCell.Value).ToUpperBS.Trim    ' ToUpper.Trim
                        End If

                    Case "Params"
                        If Not pCell.Value Is Nothing Then
                            pCell.Value = CStr(pCell.Value).ToUpperBS.Trim    ' ToUpper.Trim
                        End If

                End Select

                'delete cell errors
                pCell.ErrorText = ""
                Me.ValidationError = False
                pCell.Style.ForeColor = Color.Black
            Else
                'assign cel errors
                pCell.ErrorText = GetMessageText(ErrorMsg)
                Me.ValidationError = True
                pCell.Style.ForeColor = Color.Red
            End If



            'If ErrorMsg <> GlobalEnumerates.Messages.FWSCRIPT_EDIT_NUMERIC_ONLY.ToString Then
            '    myGlobal = Me.CheckGrid()
            'End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ValidateGridCell", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ValidateGridCell", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return myGlobal

    End Function

    ''' <summary>
    ''' validates cell by cell the instructions list in the grid
    ''' </summary>
    ''' <remarks>SG 15/11/10</remarks>
    Private Function ValidateCellsEditionChanges() As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try
            If Not ValidationError Then
                For Each R As DataGridViewRow In Me.bsInstructionsListDataGridView.Rows
                    If Not R.IsNewRow Then
                        For Each C As DataGridViewCell In R.Cells
                            myGlobal = MyClass.ValidateGridCell(C)
                            If Not myGlobal.HasError Then
                                If MyClass.ValidationError Then
                                    Exit Try
                                End If
                            End If

                        Next C
                    End If
                Next R
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ValidateCellsEditionChanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ValidateCellsEditionChanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try


        Return myGlobal

    End Function

#End Region

#Region "Testing Methods"



    ''' <summary>
    ''' Test with the Analyzer the current Instructions List
    ''' </summary>
    ''' <param name="pInstructions"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by SG 28/09/2010
    ''' modified by XBC 08/11/2010
    ''' </remarks>
    Private Function TestScriptInstructions(ByVal pInstructions As List(Of InstructionTO)) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Dim params As New List(Of String)
        Dim paramsTmp As New List(Of String)
        Dim value As String = ""
        Dim values As List(Of String) = Nothing
        Try
            'If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
            If Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then

                If pInstructions IsNot Nothing Then
                    myGlobal = MyClass.CheckGrid(True)
                    If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                        If SelectedFwScript.SyntaxOK Then

                            ' XBC 02/11/2011 - error multiples threads 
                            'Dim AppLayer As New ApplicationLayer
                            ' XBC 02/11/2011 - error multiples threads 

                            ' XBC 18/11/2010
                            For Each myInstruction As InstructionTO In pInstructions
                                If myInstruction.Params.Length > 0 Then
                                    myGlobal = myInstruction.getFwScriptParams()
                                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                        paramsTmp = CType(myGlobal.SetDatos, List(Of String))
                                    Else
                                        Exit For
                                    End If

                                    For i As Integer = 0 To paramsTmp.Count - 1
                                        params.Add(paramsTmp(i))
                                    Next
                                End If
                            Next

                            Dim isCancelled As Boolean = False

                            If params.Count > 0 Then
                                For i As Integer = 0 To params.Count - 1
                                    While Not IsNumeric(value)
                                        value = InputBox("Parameter Numeric value for [#" & (i + 1).ToString & "] :")
                                        If Not IsNumeric(value) Then
                                            If value.Length = 0 Then
                                                isCancelled = True
                                                MessageBox.Show(GetMessageText(Messages.SRV_TEST_STOP_BY_USER.ToString))
                                                Exit For
                                            End If
                                            MessageBox.Show(GetMessageText(Messages.NUMERIC_ONLY.ToString))
                                        End If
                                    End While
                                    If values Is Nothing Then
                                        values = New List(Of String)
                                    End If
                                    values.Add(value)
                                    value = ""
                                Next

                                If Not isCancelled Then
                                    If values IsNot Nothing AndAlso values.Count > 0 Then
                                        For i As Integer = 0 To values.Count - 1
                                            If values(i).Length = 0 Then
                                                ' error
                                                values = Nothing
                                                Exit For
                                            End If
                                        Next
                                    End If
                                End If
                            End If

                            If Not isCancelled Then
                                MyClass.SelectedFwScript.TestedOK = False
                                myGlobal = MyClass.DisplayGridResults(True, True)
                                myGlobal = MyClass.myScreenDelegate.SendQueueForTESTING(MyClass.SelectedFwScript.ActionID, values)
                                If Not myGlobal.HasError Then
                                    ' Send FwScripts
                                    ' XBC 20/09/2011
                                    'myGlobal = mySendFwScriptDelegate.StartFwScriptQueue

                                    ' XBC 02/11/2011 - error multiples threads 
                                    'mySendFwScriptDelegate.SendFwScriptTest(pInstructions, values)
                                    MyClass.myScreenDelegate.myFwScriptDelegate.SendFwScriptTest(pInstructions, values)
                                    ' XBC 02/11/2011 - error multiples threads 

                                    ' XBC 20/09/2011
                                End If

                            Else
                                myGlobal.HasError = True
                            End If

                        End If
                    End If
                End If
            End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".TestInstructions", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".TestInstructions", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try

        Return myGlobal

    End Function

#End Region

#Region "Save Methods"

    ''' <summary>
    ''' Checks if the current script has been modified and let the user to decide to save it or not
    ''' </summary>
    ''' <returns>
    ''' Integer with Windows.Forms.DialogResult
    ''' </returns>
    ''' <remarks>
    ''' Created by SG 08/11/2010
    ''' </remarks>
    Private Function SaveLocalPendingWarningMessage() As DialogResult
        Dim dialogResultToReturn As DialogResult = Windows.Forms.DialogResult.No
        Dim myGlobal As New GlobalDataTO
        Try
            'a script has been modified
            If MyClass.CurrentScreenMode = ScreenModes.MODIFIED Then
                dialogResultToReturn = Windows.Forms.MessageBox.Show(GetMessageText(GlobalEnumerates.Messages.SAVE_PENDING.ToString), _
                                                                     GetMessageText(GlobalEnumerates.Messages.FWSCRIPT_EDITION.ToString), _
                                                                     MessageBoxButtons.YesNo)

                If dialogResultToReturn = Windows.Forms.DialogResult.Yes Then
                    MyClass.CurrentScreenMode = ScreenModes.LOCALSAVING
                    myGlobal = MyClass.SaveScriptChanges()
                    dialogResultToReturn = Windows.Forms.DialogResult.No
                End If
            End If

            If Not myGlobal.HasError Then
                MyClass.IsLocallySaved = True
                MyClass.CurrentScreenMode = ScreenModes.LOADED
            Else
                MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            MyBase.CreateLogActivity(ex.Message, Me.Name & " SavePendingWarningMessage ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message) 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message)
            dialogResultToReturn = Windows.Forms.DialogResult.No
        End Try

        Return dialogResultToReturn
    End Function

    ''' <summary>
    ''' Checks if the current script has been modified and let the user to decide to save it or not
    ''' </summary>
    ''' <returns>
    ''' Integer with Windows.Forms.DialogResult
    ''' </returns>
    ''' <remarks>
    ''' Created by SG 08/11/2010
    ''' </remarks>
    Private Function SavePendingGlobalWarningMessage() As DialogResult
        Dim dialogResultToReturn As DialogResult = Windows.Forms.DialogResult.No
        Dim myGlobal As New GlobalDataTO
        Try
            'some scripts has been modified and saved to local
            If MyClass.GlobalChangesMade Then
                dialogResultToReturn = Windows.Forms.MessageBox.Show(GetMessageText(GlobalEnumerates.Messages.SAVE_PENDING.ToString), _
                                                                     GetMessageText(GlobalEnumerates.Messages.FWSCRIPT_EDITION.ToString), _
                                                                     MessageBoxButtons.YesNo)

                If dialogResultToReturn = Windows.Forms.DialogResult.Yes Then
                    Dim myRes As DialogResult = MyBase.ShowMessage(My.Application.Info.ProductName, GlobalEnumerates.Messages.FWSCRIPT_GLOBAL_SAVE.ToString)
                    myGlobal = SaveAllChanges()
                    dialogResultToReturn = Windows.Forms.DialogResult.No
                End If
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            MyBase.CreateLogActivity(ex.Message, Me.Name & " SavePendingGlobalWarningMessage ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message) 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message)
            dialogResultToReturn = Windows.Forms.DialogResult.No
        End Try

        If myGlobal.HasError Then MyClass.CurrentScreenMode = ScreenModes.ON_ERROR

        Return dialogResultToReturn
    End Function

    '''<summary>
    ''' saves locally the changes made in selected script
    ''' </summary>
    ''' <remarks>
    ''' Created by SG 30/10/2010
    ''' modified by XBC 24/11/2010 - myScriptDelegate becomes global into the class instead local
    ''' </remarks>
    Private Function SaveScriptChanges() As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try
            'ValidateEditionChanges()

            myGlobal = MyClass.CheckGrid()
            If Not myGlobal.HasError Then
                MyClass.ValidationError = Not Me.SelectedFwScript.SyntaxOK

                If Not MyClass.ValidationError Then
                    'Dim counter As Integer
                    Dim myChangedScript As New FwScriptTO

                    'set the Instructions thar have a #Parameter to EditionEnabled = False
                    For Each I As InstructionTO In Me.SelectedFwScript.Instructions
                        If I.Params.Trim.StartsWith("#") Then
                            I.EnableEdition = False
                        End If
                    Next

                    myChangedScript = Me.SelectedFwScript.Clone
                    'myChangedScript.FwScriptID = MyClass.SelectedFwScript.FwScriptID
                    'myChangedScript.ActionID = MyClass.SelectedFwScript.ActionID


                    If Not myChangedScript Is Nothing Then

                        If myChangedScript.FwScriptID = 0 Then
                            myChangedScript.FwScriptID = MyClass.SelectedFwScript.FwScriptID
                        End If

                        If myChangedScript.SyntaxOK Then

                            myChangedScript.ActionID = BsNameTextBox.Text
                            myChangedScript.Description = BsDescriptionTextBox.Text

                            myChangedScript.Modified = DateTime.Now

                            myGlobal = MyClass.myScreenDelegate.ModifyFwScript(myChangedScript)

                            If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                                Me.CurrentFwScriptsData = MyClass.myScreenDelegate.ActiveFwScriptsDO.Clone
                                'Me.CurrentFwScriptsData = MyClass.mySendFwScriptDelegate.ActiveFwScriptsDO.Clone
                                Dim myScript As FwScriptTO = DirectCast(myGlobal.SetDatos, FwScriptTO)
                                Me.SelectedFwScript = myScript.Clone
                                If Me.SelectedFwScript.ActionID.Length > 0 Then
                                    Me.SelectedNode.Text = Me.SelectedFwScript.ActionID
                                    MyClass.CleanFwScriptFields()
                                    myGlobal = MyClass.ShowActionFwScript(Me.SelectedNode.Text)
                                    If Not myGlobal.HasError Then
                                        Me.BsScreenActionsTreeView.SelectedNode = Me.SelectedNode
                                        Me.InitialFwScript = Me.SelectedFwScript.Clone
                                        'mark the changed script action
                                        For Each N As TreeNode In Me.SelectedFwScriptRelatedNodes
                                            N.ForeColor = Color.Green
                                            N.Text = Me.SelectedFwScript.ActionID
                                            If Not Me.SavedFwScriptNodes.Contains(N) Then
                                                Me.SavedFwScriptNodes.Add(N)
                                            End If
                                        Next

                                        'mark the screens involved
                                        For Each N As TreeNode In Me.SelectedFwScriptRelatedScreenNodes
                                            N.ForeColor = Color.Green
                                            If Not Me.SavedScreenNodes.Contains(N) Then
                                                Me.SavedScreenNodes.Add(N)
                                            End If
                                        Next

                                        Me.BsScreenActionsTreeView.Select()
                                    End If

                                End If


                                Me.RowAddingMode = False

                                Me.GlobalChangesMade = True

                            Else
                                MyBase.ShowMessage(Application.ProductName, myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
                            End If
                        End If
                    End If
                Else
                    MyBase.ShowMessage(Me.Name, GlobalEnumerates.Messages.FWSCRIPT_VALIDATION_ERROR.ToString, , Me)
                End If
            End If



        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".SaveScriptChanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SaveScriptChanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return myGlobal

    End Function

    ''' <summary>
    ''' Saves to the global ScriptsData object the changes made
    ''' </summary>
    ''' <remarks>
    ''' created by SG 5/11/10
    ''' modified by XBC 24/11/2010 - myScriptDelegate becomes global into the class instead local
    ''' </remarks>
    Private Function SaveAllChanges() As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try

            'DialogResult = mybase.ShowMessage(My.Application.Info.ProductName, GlobalEnumerates.Messages.FWSCRIPT_GLOBAL_SAVE.ToString)
            MyClass.CurrentScreenMode = ScreenModes.GLOBALSAVING

            myGlobal = MyClass.myScreenDelegate.SetAppFwScriptsData(Me.CurrentFwScriptsData)

            If Not myGlobal.HasError And Not myGlobal Is Nothing Then

                ' XBC 02/11/2011 - error multiples threads 
                'myGlobal = MyClass.myAnalyzer.LoadAppFwScriptsData()
                myGlobal = Ax00ServiceMainMDI.MDIAnalyzerManager.LoadAppFwScriptsData()
                ' XBC 02/11/2011 - error multiples threads 

                If Not myGlobal.HasError And Not myGlobal Is Nothing Then
                    MyClass.CleanFwScriptFields()
                    myGlobal = MyClass.FillAnalyzersCombo()
                    myGlobal = MyClass.LoadScreensTreeView()

                    MyClass.SelectedFwScript = New FwScriptTO

                    MyClass.SelectedFwScriptRelatedNodes.Clear()
                    MyClass.SelectedFwScriptRelatedScreenNodes.Clear()
                    MyClass.SavedFwScriptNodes.Clear()
                    MyClass.SavedScreenNodes.Clear()

                    MyClass.GlobalChangesMade = False

                    DialogResult = MyBase.ShowMessage(My.Application.Info.ProductName, GlobalEnumerates.Messages.FWSCRIPT_GLOBAL_SAVE_OK.ToString)
                End If
            End If

            If Not myGlobal.HasError Then
                MyClass.IsLocallySaved = False
                MyClass.CurrentScreenMode = ScreenModes.INITIATED
            Else
                DialogResult = MyBase.ShowMessage(My.Application.Info.ProductName, GlobalEnumerates.Messages.FWSCRIPT_GLOBAL_SAVE_ERROR.ToString)
                MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".SaveAllChanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SaveAllChanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try

        Return myGlobal

    End Function

    Private Function GetCellColumnName(ByVal pCell As DataGridViewCell) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try
            Dim myColName As String = ""
            If pCell IsNot Nothing Then
                Dim colIndex As Integer = pCell.ColumnIndex
                myColName = Me.bsInstructionsListDataGridView.Columns(colIndex).Name
            End If

            myGlobal.SetDatos = myColName

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".GetCellColumnName", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".GetCellColumnName", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try

        Return myGlobal

    End Function

#End Region

#Region "Grid Methods"

    Private Function MoveRow(ByVal pTo As RowMovement) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO
        Dim CurrentRow As DataGridViewRow
        Dim CloneOfRow As DataGridViewRow

        Dim myInitialCellIndex As Integer
        Dim myInitialIndex As Integer
        Dim newIndex As Integer

        Try

            MyClass.CurrentScreenMode = ScreenModes.GRIDCHANGING

            Dim myCell As DataGridViewCell = Nothing
            If bsInstructionsListDataGridView.SelectedCells.Count > 0 Then
                myCell = bsInstructionsListDataGridView.SelectedCells(0)
            End If

            If myCell IsNot Nothing Then

                myInitialCellIndex = myCell.ColumnIndex
                myInitialIndex = myCell.RowIndex

                CurrentRow = bsInstructionsListDataGridView.Rows(myCell.RowIndex)

                Dim myNewRow As New DataGridViewRow
                If CurrentRow IsNot Nothing Then
                    myNewRow = CType(CurrentRow.Clone, DataGridViewRow)
                    If myNewRow IsNot Nothing Then
                        For Each dgc1 As DataGridViewCell In CurrentRow.Cells
                            myNewRow.Cells(dgc1.ColumnIndex).Value = dgc1.Value
                        Next
                    End If
                End If

                CloneOfRow = myNewRow

                Select Case pTo
                    Case RowMovement.DOWN
                        newIndex = myInitialIndex + 1

                        If newIndex < bsInstructionsListDataGridView.Rows.Count Then
                            'delete previous
                            bsInstructionsListDataGridView.Rows.RemoveAt(myInitialIndex)
                            bsInstructionsListDataGridView.Rows.Insert(newIndex, CloneOfRow)
                            bsInstructionsListDataGridView.Rows(myInitialIndex + 1).Cells("Timer").Selected = True
                        End If

                    Case RowMovement.UP
                        newIndex = myInitialIndex - 1

                        If newIndex >= 0 Then
                            'delete previous
                            bsInstructionsListDataGridView.Rows.RemoveAt(myInitialIndex)
                            bsInstructionsListDataGridView.Rows.Insert(newIndex, CloneOfRow)
                            bsInstructionsListDataGridView.Rows(myInitialIndex - 1).Cells("Timer").Selected = True
                        End If

                End Select

            End If

            'uncheck the Syntax OK
            Me.SelectedFwScript.SyntaxOK = False
            Me.SelectedFwScript.TestedOK = False
            For Each R As DataGridViewRow In Me.bsInstructionsListDataGridView.Rows
                R.Cells("SyntaxOK").Value = False
                R.Cells("TestedOK").Value = False
            Next

            myGlobal = MyClass.CheckGrid

            If Not myGlobal.HasError Then

                'CHANGE Me.SelectedFwScript!!!


                For Each R As DataGridViewRow In Me.bsInstructionsListDataGridView.Rows
                    If Not CBool(R.Cells("EnableEdition").Value) Then
                        R.Cells("Sequence").ReadOnly = False
                        R.Cells("Code").ReadOnly = True
                        R.Cells("Params").ReadOnly = True
                        R.Cells("SyntaxOK").ReadOnly = True
                        R.Cells("TestedOK").ReadOnly = True
                    End If
                Next R

                Me.bsInstructionsListDataGridView.Rows(newIndex).Cells(myInitialCellIndex).Selected = True

                MyClass.CurrentScreenMode = ScreenModes.MODIFIED

            Else
                MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".MoveRow", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".MoveRow", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return myGlobal

    End Function

#End Region

#Region "Others"


    '

    ''' <summary>
    ''' Restores to the Analyzer the internal adjustments factory set
    ''' </summary>
    ''' <remarks>Created by SGM 24/06/2011</remarks>
    Private Function RestoreFactoryScripts() As GlobalDataTO

        Dim myGlobal As New GlobalDataTO
        Dim myPreviousScreenMode As ScreenModes = MyClass.CurrentScreenMode

        Try

            Dim res As DialogResult = MyBase.ShowMessage(Me.Name, GlobalEnumerates.Messages.FWSCRIPT_RESTORE_WARNING.ToString, , Me)

            If res = Windows.Forms.DialogResult.Yes Then
                myGlobal = myScreenDelegate.UpdateAppFwScriptsDataToFactory()
                If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then

                    ' XBC 02/11/2011 - error multiples threads 
                    'myGlobal = MyClass.myAnalyzer.LoadAppFwScriptsData()
                    myGlobal = Ax00ServiceMainMDI.MDIAnalyzerManager.LoadAppFwScriptsData()
                    ' XBC 02/11/2011 - error multiples threads 

                End If
                If Not myGlobal.HasError Then
                    DialogResult = MyBase.ShowMessage(My.Application.Info.ProductName, GlobalEnumerates.Messages.FWSCRIPT_RESTORE_OK.ToString)
                    MyClass.CurrentScreenMode = ScreenModes.INITIATING
                    MyClass.LoadScreen()
                    MyClass.CurrentScreenMode = ScreenModes.INITIATED
                Else
                    DialogResult = MyBase.ShowMessage(My.Application.Info.ProductName, GlobalEnumerates.Messages.FWSCRIPT_RESTORE_ERROR.ToString)
                    MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
                End If
            Else
                MyClass.CurrentScreenMode = myPreviousScreenMode
            End If



        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".RestoreFactoryScripts ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RestoreFactoryScripts ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try

        Return myGlobal

    End Function


#End Region

#End Region

#Region "Event Handlers"

#Region "Screen"

    Private Sub FwScriptsEdition_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try
            If e.CloseReason = CloseReason.MdiFormClosing Then
                e.Cancel = True
            Else
                MyClass.myScreenDelegate.Dispose()
                MyClass.myScreenDelegate = Nothing
                Me.Dispose()

                Dim myMDI As Ax00ServiceMainMDI = CType(MyClass.MdiParent, Ax00ServiceMainMDI)
                If myMDI IsNot Nothing Then
                    myMDI.ActivateActionButtonBar(True)
                    myMDI.ActivateMenus(True)
                End If

            End If
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".FormScriptsManager_FormClosing", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".FormScriptsManager_FormClosing", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' created by SG 28/09/2010
    ''' modified by XBC 08/11/2010
    '''             PG  15/11/2010. Get the current language
    ''' modified by XBC 24/11/2010 - myScriptDelegate becomes global into the class instead local
    ''' </remarks>
    Private Sub FormScriptsManager_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            LanguageID = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString()

            Me.Location = New Point(0, 0)

            MyClass.LoadScreen()

            ResetBorderSRV()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".FormScriptsManager_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".FormScriptsManager_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "Buttons"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>created by SG 28/09/2010</remarks>
    Private Sub BsExportButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsExportButton.Click

        Dim myGlobal As New GlobalDataTO
        Dim myPreviousScreenMode As ScreenModes = MyClass.CurrentScreenMode

        Try
            If Me.SelectedFwScript.FwScriptID > 0 Then
                MyClass.CurrentScreenMode = ScreenModes.SCRIPT_EXPORTING
                myGlobal = MyClass.ExportCurrentFwScript(Me.SelectedFwScript.FwScriptID)

            End If

            If Not myGlobal.HasError Then
                MyClass.CurrentScreenMode = myPreviousScreenMode
            Else
                MyClass.CurrentScreenMode = myPreviousScreenMode
            End If


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsExportButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsExportButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SG 28/09/2010</remarks>
    Private Sub BsImportButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsImportButton.Click

        Dim myGlobal As New GlobalDataTO
        Dim myPreviousScreenMode As ScreenModes = MyClass.CurrentScreenMode

        Try
            If Me.SelectedFwScript.FwScriptID > 0 Then
                MyClass.CurrentScreenMode = ScreenModes.SCRIPT_IMPORTING
                myGlobal = MyClass.ImportCurrentFwScript()


                If Not myGlobal.HasError Then

                    If myGlobal.SetDatos Is Nothing Then
                        MyClass.CurrentScreenMode = myPreviousScreenMode
                        Exit Sub
                    End If

                    Dim myID As Integer = MyClass.SelectedFwScript.FwScriptID
                    MyClass.SelectedFwScript = CType(myGlobal.SetDatos, FwScriptTO)
                    MyClass.SelectedFwScript.FwScriptID = myID

                    myGlobal = MyClass.ShowInstructions(MyClass.SelectedFwScript.Instructions)
                    If Not myGlobal.HasError Then
                        MyBase.ShowMessage(Me.Name, GlobalEnumerates.Messages.FWSCRIPT_TEXT_IMPORT_OK.ToString, , Me)
                        Me.BsSaveButton.Enabled = True
                        Me.BsCancelButton.Enabled = True
                    End If

                    MyClass.CurrentScreenMode = ScreenModes.MODIFIED

                Else
                    MyBase.ShowMessage(Me.Name, GlobalEnumerates.Messages.FWSCRIPT_TEXT_IMPORT_ERROR.ToString, , Me)
                    MyClass.CurrentScreenMode = myPreviousScreenMode
                    Me.BsSaveButton.Enabled = False
                    Me.BsCancelButton.Enabled = False
                End If

            End If
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsImportButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsImportButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR

        Finally
            If Not myGlobal.HasError Then
                MyClass.CurrentScreenMode = myPreviousScreenMode
            End If
        End Try



    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SG 28/09/2010</remarks>
    Private Sub BsCheckButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsCheckButton.Click

        Dim myGlobal As New GlobalDataTO
        Dim myPreviousScreenMode As ScreenModes = MyClass.CurrentScreenMode

        Try
            If Me.SelectedFwScript.Instructions IsNot Nothing Then
                If Me.SelectedFwScript.Instructions.Count > 0 Then
                    myGlobal = MyClass.CheckGrid(False)
                    If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                        SelectedFwScript.SyntaxOK = CBool(myGlobal.SetDatos)
                    Else
                        MyClass.CurrentScreenMode = myPreviousScreenMode
                    End If
                End If
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsCheckButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsCheckButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SG 28/09/2010</remarks>
    Private Sub BsTestButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsTestButton.Click

        Dim myGlobal As New GlobalDataTO
        Dim myPreviousScreenMode As ScreenModes = MyClass.CurrentScreenMode

        MyClass.PreviousScreenModeBeforeTesting = MyClass.CurrentScreenMode

        Try
            If MyClass.SelectedFwScript.Instructions IsNot Nothing Then
                MyClass.CurrentScreenMode = ScreenModes.SCRIPT_TESTING
                myGlobal = MyClass.TestScriptInstructions(Me.SelectedFwScript.Instructions)

                If myGlobal.HasError Then
                    If myGlobal.ErrorCode IsNot Nothing Then
                        MessageBox.Show(GetMessageText(myGlobal.ErrorCode.ToString), Me.MdiParent.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                    MyClass.CurrentScreenMode = myPreviousScreenMode
                    MyClass.PreviousScreenModeBeforeTesting = ScreenModes._NONE
                End If

            End If
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsTestButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsTestButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    Private Sub BsUnTestButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsUnTestButton.Click

        Dim myGlobal As New GlobalDataTO

        Try

            If MyClass.SelectedFwScript IsNot Nothing Then
                MyClass.SelectedFwScript.TestedOK = False
                myGlobal = MyClass.CheckGrid
                MyClass.CurrentScreenMode = ScreenModes.MODIFIED
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsUnTestButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsUnTestButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SG 28/09/2010</remarks>
    Private Sub BsEditButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsEditButton.Click
        Try

            If Me.SelectedFwScript IsNot Nothing AndAlso Me.SelectedFwScript.ActionID IsNot Nothing Then
                Dim myRow As DataGridViewRow = Me.bsInstructionsListDataGridView.CurrentRow
                MyClass.CurrentScreenMode = ScreenModes.MODIFYING
                If myRow IsNot Nothing Then
                    myRow.Cells(2).Selected = True
                End If
            End If
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsEditButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsEditButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    ''' <summary>
    ''' Add new instruction row
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SG 28/09/2010</remarks>
    Private Sub BsAddButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsAddButton.Click
        Try
            If MyClass.IsSyntaxOK Then

                MyClass.CurrentScreenMode = ScreenModes.GRIDCHANGING

                Dim myRowIndex As Integer
                myRowIndex = Me.bsInstructionsListDataGridView.Rows.Add()
                Dim myNewInstruction As New InstructionTO
                With Me.bsInstructionsListDataGridView.Rows(myRowIndex)
                    .Cells(0).Value = myRowIndex
                    .Cells(1).Value = myRowIndex
                    If myRowIndex >= 1 Then
                        .Cells("Timer").Value = CStr(CInt(Me.bsInstructionsListDataGridView.Rows(myRowIndex - 1).Cells(2).Value) + 1).PadLeft(6, CChar("0"))
                    Else
                        .Cells("Timer").Value = myNewInstruction.Timer.ToString.PadLeft(6, CChar("0"))
                    End If

                    .Cells("Code").Value = myNewInstruction.Code.ToString
                    .Cells("Params").Value = myNewInstruction.Params.ToString
                    .Cells("SyntaxOK").Value = False
                    .Cells("TestedOK").Value = False
                    .Cells("EnableEdition").Value = True

                    .Cells("Sequence").ReadOnly = False
                    .Cells("Code").ReadOnly = False
                    .Cells("Params").ReadOnly = False
                    .Cells("SyntaxOK").ReadOnly = True
                    .Cells("TestedOK").ReadOnly = True
                End With

                Me.bsInstructionsListDataGridView.Rows(Me.bsInstructionsListDataGridView.Rows.Count - 1).Cells("Timer").Selected = True
                Me.RowAddingMode = True

                MyClass.CurrentScreenMode = ScreenModes.MODIFIED

            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsAddButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsAddButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try


    End Sub

    ''' <summary>
    ''' Save changes into Actions, Scripts and Instructions
    ''' </summary>
    ''' <remarks>Created by XBC 28/09/2010
    ''' Modified by SG 30/09/2010 check syntaxis</remarks>
    Private Sub BsSaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsSaveButton.Click

        Dim myGlobal As New GlobalDataTO
        Dim myPreviousScreenMode As ScreenModes = MyClass.CurrentScreenMode

        Try

            MyClass.CurrentScreenMode = ScreenModes.LOCALSAVING

            myGlobal = MyClass.SaveScriptChanges()

            If Not myGlobal.HasError Then
                MyClass.IsLocallySaved = True
                MyClass.CurrentScreenMode = ScreenModes.LOADED
            Else
                MyClass.CurrentScreenMode = myPreviousScreenMode
            End If

            If SavedFwScriptNodes.Count > 0 Then
                Me.BsSaveAllButton.Enabled = True
            End If
            'Me.SaveAllChanges()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsSaveButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsSaveButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsCancelButton.Click
        Dim myGlobal As New GlobalDataTO
        Dim myPreviousScreenMode As ScreenModes = MyClass.CurrentScreenMode
        Try
            Dim myRowIndex As Integer = -1
            If Me.bsInstructionsListDataGridView.SelectedCells.Count > 0 Then
                myRowIndex = Me.bsInstructionsListDataGridView.Rows(Me.bsInstructionsListDataGridView.SelectedCells(0).RowIndex).Index
            End If

            If MyClass.CurrentScreenMode = ScreenModes.MODIFIED Then
                Me.SelectedFwScript = Me.InitialFwScript.Clone
                MyClass.CleanFwScriptFields()
                myGlobal = MyClass.ShowActionFwScript(Me.InitialFwScript.ActionID)
                If Not myGlobal.HasError Then
                    MyClass.RowAddingMode = False
                Else
                    MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
                End If
            End If

            If Not myGlobal.HasError Then
                MyClass.CurrentScreenMode = ScreenModes.LOADED

                If myRowIndex >= 0 Then
                    Me.bsInstructionsListDataGridView.Rows(0).Selected = True
                End If
            Else
                MyClass.CurrentScreenMode = myPreviousScreenMode
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsCancelButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsCancelButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    ''' <summary>
    ''' Move to UP the selected instruction row
    ''' </summary>
    ''' <remarks>Created by XBC 29/09/2010</remarks>
    Private Sub BsMoveUpButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsMoveUpButton.Click
        Try
            Me.MoveRow(RowMovement.UP)
            'Me.Sort("UP")
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsMoveUpButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsMoveUpButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    ''' <summary>
    ''' Move to DOWN the selected instruction row
    ''' </summary>
    ''' <remarks>Created by XBC 29/09/2010</remarks>
    Private Sub BsMoveDownButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsMoveDownButton.Click
        Try
            Me.MoveRow(RowMovement.DOWN)
            'Me.Sort("DOWN")
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsMoveDownButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsMoveDownButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    ''' <summary>
    ''' Remove selected instruction row
    ''' </summary>
    ''' <remarks>Created by XBC 29/09/2010</remarks>
    Private Sub BsRemoveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsRemoveButton.Click

        Try
            MyClass.CurrentScreenMode = ScreenModes.GRIDCHANGING

            Me.bsInstructionsListDataGridView.Rows.Remove(Me.bsInstructionsListDataGridView.CurrentRow)

            MyClass.CurrentScreenMode = ScreenModes.MODIFIED

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsRemoveButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsRemoveButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try

    End Sub

    ''' <summary>
    ''' manages the exit from the screen
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SG 30/09/2010</remarks>
    Private Sub BsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsExitButton.Click
        Try

            Dim closeScreen As Boolean = True

            If SaveLocalPendingWarningMessage() <> Windows.Forms.DialogResult.No Then
                If SavePendingGlobalWarningMessage() <> Windows.Forms.DialogResult.No Then
                    closeScreen = False
                End If
            Else
                If SavePendingGlobalWarningMessage() <> Windows.Forms.DialogResult.No Then
                    closeScreen = False
                End If
            End If

            If closeScreen Then
                Me.Close()
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, "ExitButton_Click " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ExitButton_Click", "SYSTEM_ERROR", ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' saves all the scripts data
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SG 5/11/10</remarks>
    Private Sub BsSaveAllButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsSaveAllButton.Click

        Dim myGlobal As New GlobalDataTO
        Dim myPreviousScreenMode As ScreenModes = MyClass.CurrentScreenMode

        Try

            Dim myRes As DialogResult = MyBase.ShowMessage(My.Application.Info.ProductName, GlobalEnumerates.Messages.FWSCRIPT_GLOBAL_SAVE.ToString)

            If myRes = Windows.Forms.DialogResult.OK Then

                myGlobal = MyClass.SaveAllChanges()

                If myGlobal.HasError Then
                    MyClass.CurrentScreenMode = myPreviousScreenMode
                End If

            End If


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, "BsSaveAllButton_Click " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsSaveAllButton_Click", "SYSTEM_ERROR", ex.Message)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    Private Sub BsExportAllToFileButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsExportAllToFileButton.Click

        Dim myGlobal As New GlobalDataTO

        Try

            Dim dlg As New SaveFileDialog()
            dlg.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*"
            dlg.FilterIndex = 1
            dlg.CheckPathExists = True
            dlg.DefaultExt = ".xml"
            dlg.InitialDirectory = Application.StartupPath
            dlg.Title = "Export to an External FWScripts data File"

            If dlg.ShowDialog() = DialogResult.OK Then

                Dim myPath As String = dlg.FileName
                Dim isDecrypted As Boolean
                Dim myRes As DialogResult = MessageBox.Show("Do you want to export the data to a Decrypted file?", My.Application.Info.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)

                If myRes <> Windows.Forms.DialogResult.Cancel Then

                    Me.Cursor = Cursors.WaitCursor
                    isDecrypted = (myRes = Windows.Forms.DialogResult.Yes)
                    myGlobal = MyClass.myScreenDelegate.ExportFwScriptsData(myPath, isDecrypted)
                    Me.Cursor = Cursors.Default
                End If
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, "BsExportAllToFileButton_Click " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsExportAllToFileButton_Click", "SYSTEM_ERROR", ex.Message)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    Private Sub bsInstructionsListDataGridView_CellValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsInstructionsListDataGridView.CellValueChanged
        Dim myRow As DataGridViewRow = bsInstructionsListDataGridView.Rows(e.RowIndex)
        Try
            If MyClass.CurrentScreenMode = ScreenModes.MODIFYING Or MyClass.CurrentScreenMode = ScreenModes.MODIFIED Then
                If e.ColumnIndex < 5 Then
                    myRow.Cells("SyntaxOK").Value = False
                    If e.ColumnIndex < 6 Then
                        myRow.Cells("TestedOK").Value = False
                    End If
                End If

                MyClass.CurrentScreenMode = ScreenModes.MODIFIED

            End If
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, "bsInstructionsListDataGridView_CellValueChanged " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".bsInstructionsListDataGridView_CellValueChanged", "SYSTEM_ERROR", ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Restores the Factory Application's Scripts data
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by SG 09/11/10
    ''' modified by XBC 24/11/2010 - myScriptDelegate becomes global into the class instead local
    ''' </remarks>
    Private Sub BsRestoreButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsRestoreButton.Click

        Dim myglobal As New GlobalDataTO
        Dim myPreviousScreenMode As ScreenModes = MyClass.CurrentScreenMode

        Try

            MyClass.CurrentScreenMode = ScreenModes.ALL_RESTORING

            myglobal = MyClass.RestoreFactoryScripts

            If myglobal.HasError Then
                MyClass.CurrentScreenMode = myPreviousScreenMode
            End If


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, "BsRestoreButton_Click " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsRestoreButton_Click", "SYSTEM_ERROR", ex.Message)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

    ''' <summary>
    ''' Imports the Application's Scripts data
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by SG 09/11/10
    ''' modified by XBC 24/11/2010 - myScriptDelegate becomes global into the class instead local
    ''' </remarks>
    Private Sub BsImportAllButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsImportAllButton.Click

        Dim myGlobal As New GlobalDataTO
        Dim myPreviousScreenMode As ScreenModes = MyClass.CurrentScreenMode

        Try

            MyClass.CurrentScreenMode = ScreenModes.ALL_IMPORTING

            Dim res As DialogResult = MyBase.ShowMessage(Me.Name, GlobalEnumerates.Messages.FWSCRIPT_IMPORT_ALL_WARNING.ToString, , Me)

            If res = Windows.Forms.DialogResult.Yes Then

                Dim dlg As New OpenFileDialog
                dlg.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*"
                dlg.FilterIndex = 1
                dlg.CheckPathExists = True
                dlg.DefaultExt = ".xml"
                dlg.InitialDirectory = Application.StartupPath
                dlg.Title = GetMessageText(GlobalEnumerates.Messages.FWSCRIPT_EDITION.ToString)
                dlg.Multiselect = False

                res = dlg.ShowDialog()

                If res = Windows.Forms.DialogResult.OK Then
                    Dim myNewXMLFile As String = dlg.FileName

                    MyClass.CurrentScreenMode = ScreenModes.ALL_IMPORTING

                    myGlobal = MyClass.myScreenDelegate.UpdateAppFwScriptsData(myNewXMLFile)

                    If Not myGlobal.HasError And Not myGlobal Is Nothing Then

                        Me.CurrentFwScriptsData = CType(myGlobal.SetDatos, FwScriptsDataTO)

                        MyClass.IsLocallySaved = True

                        'do not save to analyzer
                        'myGlobal = MyClass.SaveAllChanges()

                        'If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                        '    myGlobal = MyClass.myAnalyzer.LoadAppFwScriptsData()
                        'End If

                    End If

                    If Not myGlobal.HasError Then
                        DialogResult = MyBase.ShowMessage(My.Application.Info.ProductName, GlobalEnumerates.Messages.FWSCRIPT_IMPORT_ALL_OK.ToString)
                        MyClass.CurrentScreenMode = ScreenModes.INITIATED
                    Else
                        DialogResult = MyBase.ShowMessage(My.Application.Info.ProductName, GlobalEnumerates.Messages.FWSCRIPT_IMPORT_ALL_ERROR.ToString)
                        MyClass.CurrentScreenMode = myPreviousScreenMode
                    End If

                Else
                    MyClass.CurrentScreenMode = myPreviousScreenMode
                End If

            Else
                MyClass.CurrentScreenMode = myPreviousScreenMode
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, "BsImportAllButton_Click " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsImportAllButton_Click", "SYSTEM_ERROR", ex.Message)
            MyClass.CurrentScreenMode = ScreenModes.ON_ERROR
        End Try
    End Sub

#End Region

#Region "Treeview"

    ''' <summary>
    ''' Mouse Click Event into Screen-Actions List
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' created by XBC 27/09/2010
    ''' Modified by SG 29/09/2010 edition mode
    ''' modified by XBC 24/11/2010 - myScriptDelegate becomes global into the class instead local
    ''' </remarks>
    Private Sub BsScreenActionsTreeView_NodeMouseClick(ByVal sender As Object, ByVal e As TreeNodeMouseClickEventArgs) Handles BsScreenActionsTreeView.NodeMouseClick

        Try
            Dim myScript As New FwScriptTO
            Dim currNode As TreeNode
            Dim ClickPoint As Point = New Point(e.X, e.Y)
            currNode = BsScreenActionsTreeView.GetNodeAt(ClickPoint)
            If Not currNode Is Nothing Then
                If myScript.ActionID <> currNode.Text Then
                    'Dim CheckScriptIsSaved As Boolean = False
                    If SaveLocalPendingWarningMessage() <> Windows.Forms.DialogResult.Yes Then
                        Dim myGlobal As New GlobalDataTO
                        'Dim myScriptsDelegate As New ScriptsEditionDelegate(Me.CurrentScriptsData)
                        myGlobal = MyClass.myScreenDelegate.GetFwScriptByActionID(currNode.Text.Trim)
                        If Not myGlobal.HasError And Not myGlobal Is Nothing Then
                            myScript = CType(myGlobal.SetDatos, FwScriptTO)
                            If myScript IsNot Nothing And currNode.Nodes.Count = 0 Then
                                Me.SelectedFwScript = myScript.Clone

                                'screen nodes in which the script is defined
                                Me.SelectedFwScriptRelatedScreenNodes = GetFwScriptRelatedScreenNodes(myScript.ActionID)

                                'nodes that represents the same script
                                Me.SelectedFwScriptRelatedNodes = GetFwScriptRelatedNodes(myScript.ActionID)

                                'get initial copy
                                Me.InitialFwScript = Me.SelectedFwScript.Clone

                                MyClass.CleanFwScriptFields()
                                myGlobal = MyClass.ShowActionFwScript(Me.SelectedFwScript.ActionID)
                                If Not myGlobal.HasError Then
                                    MyClass.SelectedNode = currNode
                                    MyClass.CurrentScreenMode = ScreenModes.LOADED
                                End If
                            Else
                                MyClass.CurrentScreenMode = ScreenModes.LOADED
                            End If
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsScreenActionsTreeView_NodeMouseClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsScreenActionsTreeView_NodeMouseClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Mouse Double Click Event into Screen-Actions List
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' created by SG 28/09/2010
    ''' modified by XBC 24/11/2010 - myScriptDelegate becomes global into the class instead local
    ''' </remarks>
    Private Sub BsScreenActionsTreeView_NodeMouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles BsScreenActionsTreeView.NodeMouseDoubleClick
        Try

            Dim myScript As New FwScriptTO
            Dim currNode As TreeNode
            Dim ClickPoint As Point = New Point(e.X, e.Y)
            currNode = BsScreenActionsTreeView.GetNodeAt(ClickPoint)
            If Not currNode Is Nothing Then
                If myScript.ActionID <> currNode.Text Then
                    If SaveLocalPendingWarningMessage() <> Windows.Forms.DialogResult.Yes Then

                        Dim myGlobal As New GlobalDataTO
                        'Dim myScriptsDelegate As New ScriptsEditionDelegate(Me.CurrentScriptsData)
                        myGlobal = MyClass.myScreenDelegate.GetFwScriptByActionID(currNode.Text.Trim)
                        If Not myGlobal.HasError And Not myGlobal Is Nothing Then
                            myScript = CType(myGlobal.SetDatos, FwScriptTO)
                            If myScript IsNot Nothing And currNode.Nodes.Count = 0 Then
                                Me.SelectedFwScript = myScript.Clone

                                'screen nodes in which the script is defined
                                Me.SelectedFwScriptRelatedScreenNodes = GetFwScriptRelatedScreenNodes(myScript.ActionID)

                                'nodes that represents the same script
                                Me.SelectedFwScriptRelatedNodes = GetFwScriptRelatedNodes(myScript.ActionID)

                                MyClass.CleanFwScriptFields()
                                myGlobal = MyClass.ShowActionFwScript(Me.SelectedFwScript.ActionID)
                                If Not myGlobal.HasError Then
                                    MyClass.SelectedNode = currNode
                                    If MyClass.CurrentScreenMode <> ScreenModes.MODIFIED Then
                                        MyClass.CurrentScreenMode = ScreenModes.MODIFYING
                                    End If
                                End If
                            Else
                                MyClass.CleanFwScriptFields()
                                MyClass.RowAddingMode = False
                                MyClass.SelectedFwScript = Nothing
                                MyClass.CurrentScreenMode = ScreenModes.LOADED
                            End If
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsScreenActionsTreeView_NodeMouseDoubleClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsScreenActionsTreeView_NodeMouseDoubleClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "Datagrid"

    Private Sub bsInstructionsListDataGridView_CellDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsInstructionsListDataGridView.CellDoubleClick
        Try
            If Me.SelectedFwScript IsNot Nothing AndAlso Me.SelectedFwScript.ActionID IsNot Nothing Then
                MyClass.CurrentScreenMode = ScreenModes.MODIFYING
            End If
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".bsInstructionsListDataGridView_CellDoubleClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".bsInstructionsListDataGridView_CellDoubleClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub bsInstructionsListDataGridView_SelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsInstructionsListDataGridView.SelectionChanged
        Dim myglobal As New GlobalDataTO
        Try
            If MyClass.CurrentScreenMode = ScreenModes.MODIFYING Or MyClass.CurrentScreenMode = ScreenModes.MODIFIED Then
                myglobal = MyClass.CheckGrid
            Else
                MyClass.PrepareUIPrepareScreen()
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, "bsInstructionsListDataGridView_SelectionChanged " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".bsInstructionsListDataGridView_SelectionChanged", "SYSTEM_ERROR", ex.Message)
        End Try
    End Sub

    Private Sub bsInstructionsListDataGridView_EditingControlShowing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles bsInstructionsListDataGridView.EditingControlShowing
        Try

            If Not TypeOf e.Control Is TextBox Then Return

            Dim tb As TextBox = CType(e.Control, TextBox)

            If Not tb Is Nothing Then
                AddHandler tb.KeyDown, AddressOf dgvTextBox_KeyDown
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & " bsInstructionsListDataGridView_EditingControlShowing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", "SYSTEM_ERROR", ex.Message)
        End Try
    End Sub

    Private Sub dgvTextBox_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs)
        Try

            Select Case e.KeyCode
                Case Keys.Delete, Keys.Back

                Case Keys.Right, Keys.Left, Keys.Up, Keys.Down, Keys.Tab, Keys.Escape, Keys.Control, Keys.ControlKey, Keys.Shift, Keys.Alt


                Case Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9

                    'Dim myText As TextBox = CType(sender, TextBox)
                    'If myText.Text.Length > 0 Then
                    '    If Not IsNumeric(myText.Text) Then
                    '        e.SuppressKeyPress = True
                    '        e.Handled = True
                    '        Exit Select
                    '    End If
                    '    If myText.SelectedText.Length = 0 And myText.Text.Length > 5 Then
                    '        e.SuppressKeyPress = True
                    '        e.Handled = True
                    '        Exit Select
                    '    End If
                    'End If

                Case Else
                    ' 20/01/2011 porqué estaba puesto esto ???
                    'e.SuppressKeyPress = True
                    'e.Handled = True

            End Select

            MyClass.CurrentScreenMode = ScreenModes.MODIFIED

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & " dgvTextBox_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", "SYSTEM_ERROR", ex.Message)
            MyClass.CurrentScreenModeAttr = ScreenModes.ON_ERROR
        End Try
    End Sub

    Private Sub bsInstructionsListDataGridView_RowEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsInstructionsListDataGridView.RowEnter

        Dim myglobal As New GlobalDataTO

        Try

            If MyClass.RowAddingMode Then
                If e.RowIndex = Me.bsInstructionsListDataGridView.Rows.Count - 1 Then
                    Dim myNewRowIndex As Integer = Me.bsInstructionsListDataGridView.Rows.Count - 1
                    myglobal = MyClass.ValidateCellsEditionChanges()
                    If Not myglobal.HasError Then
                        If Not ValidationError Then
                            MyClass.RowAddingMode = False
                        Else
                            Me.bsInstructionsListDataGridView.Rows(myNewRowIndex).Selected = True
                        End If
                    End If
                Else
                    If Not ValidationError Then
                        MyClass.RowAddingMode = False
                    End If
                End If
            End If

            If myglobal.HasError Then MyClass.CurrentScreenMode = ScreenModes.ON_ERROR

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & " bsInstructionsListDataGridView_RowEnter ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", "SYSTEM_ERROR", ex.Message)
            MyClass.CurrentScreenModeAttr = ScreenModes.ON_ERROR
        End Try
    End Sub

#End Region

#Region "Others"

    ''' <summary>
    ''' Manages all the changes during edition
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ControlValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsDescriptionTextBox.TextChanged
        If MyClass.CurrentScreenMode = ScreenModes.MODIFYING Or MyClass.CurrentScreenMode = ScreenModes.MODIFIED Then
            MyClass.CurrentScreenMode = ScreenModes.MODIFIED
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created SG 08/11/10</remarks>
    Private Sub BSAnalyzerCombo_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BSAnalyzerCombo.SelectedIndexChanged
        Try

            If MyClass.CurrentScreenMode = ScreenModes.LOADED Or MyClass.CurrentScreenMode = ScreenModes.MODIFYING Or MyClass.CurrentScreenMode = ScreenModes.MODIFIED Then
                MyClass.LoadScreensTreeView()
            End If
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, "BSAnalyzerCombo_SelectedIndexChanged " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BSAnalyzerCombo_SelectedIndexChanged", "SYSTEM_ERROR", ex.Message)
        End Try
    End Sub

#End Region

#End Region


#Region "Must Inherited"

    Public Overrides Sub PrepareErrorMode(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareErrorMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareErrorMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' It manages the Stop of the operation(s) that are currently being performed. 
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>Created by SGM 19/10/2012</remarks>
    Public Overrides Sub StopCurrentOperation(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            'TODO

            'when stop action is finished, perform final operations after alarm received
            MyBase.myServiceMDI.ManageAlarmStep2(pAlarmType)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".StopCurrentOperation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".StopCurrentOperation ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region
    '************************************************************
#Region "Not Used"

    ' ''' <summary>
    ' ''' Sorting rows instructions 
    ' ''' </summary>
    ' ''' <param name="pDirection">(up and down)</param>
    ' ''' <remarks>
    ' ''' Created by XBC 29/09/2010
    ' ''' Modified by XBC 17/11/2010 - add EnableEdition hidden column
    ' ''' Modified by XBC 24/11/2010 - Add functionality to Not Editable Instruction
    ' ''' </remarks>
    'Private Function Sort(ByVal pDirection As String) As GlobalDataTO
    '    Dim myInstruction As InstructionTO = Nothing
    '    Dim myInstructionTmp As InstructionTO = Nothing
    '    'Dim tableTemp As DataTable = Nothing
    '    Dim seleccion As String = ""
    '    Dim seleccioOk As Boolean

    '    Dim myGlobal As New GlobalDataTO

    '    Try

    '        If Me.bsInstructionsListDataGridView.SelectedRows.Count > 0 Then
    '            If Me.bsInstructionsListDataGridView.SelectedRows(0).IsNewRow Then Exit Function
    '        End If

    '        If Me.bsInstructionsListDataGridView.SelectedCells.Count > 0 Then
    '            If Me.bsInstructionsListDataGridView.SelectedCells(0).IsInEditMode Then Exit Function
    '        End If

    '        If MyClass.CurrentScreenMode = ScreenModes.MODIFYING And Me.bsInstructionsListDataGridView.Rows.Count > 0 Then

    '            For Each rdsort2 As DataGridViewRow In Me.bsInstructionsListDataGridView.Rows
    '                For Each cellsort As DataGridViewCell In rdsort2.Cells
    '                    If cellsort.Selected Then
    '                        rdsort2.Selected = True
    '                        seleccion = rdsort2.Cells("InstructionID").Value.ToString
    '                        seleccioOk = True
    '                        Exit For
    '                    End If
    '                Next
    '                If seleccioOk Then Exit For
    '            Next

    '            If Not Me.SelectedFwScript.Instructions Is Nothing Then
    '                Dim myInstructionsAux As New List(Of InstructionTO)

    '                Select Case pDirection
    '                    Case "UP"
    '                        For Each rsort As DataGridViewRow In Me.bsInstructionsListDataGridView.Rows
    '                            If Me.SelectedRow(rsort.Cells(0).Value.ToString) Then
    '                                If rsort.Index = 0 Then
    '                                    ' if wish to sort to up, first element is already sorted
    '                                    Exit Function
    '                                Else
    '                                    ' selected
    '                                    myInstruction = New InstructionTO
    '                                    myInstruction.InstructionID = CInt(rsort.Cells("InstructionID").Value)
    '                                    myInstruction.Sequence = CInt(rsort.Cells("Sequence").Value)
    '                                    myInstruction.Timer = CInt(rsort.Cells("Timer").Value)
    '                                    myInstruction.Code = CStr(rsort.Cells("Code").Value)
    '                                    myInstruction.Params = CStr(rsort.Cells("Params").Value)
    '                                    myInstruction.EnableEdition = CBool(rsort.Cells("EnableEdition").Value)
    '                                    myInstructionsAux.Add(myInstruction)
    '                                    '// adding previous item if exists
    '                                    If Not myInstructionTmp Is Nothing Then
    '                                        myInstructionsAux.Add(myInstructionTmp)
    '                                    End If
    '                                End If
    '                            Else
    '                                If rsort.Index + 1 <= Me.bsInstructionsListDataGridView.Rows.Count - 1 AndAlso _
    '                                   Me.SelectedRow(Me.bsInstructionsListDataGridView.Rows(rsort.Index + 1).Cells(0).Value.ToString) Then
    '                                    '//Save previous element to the selected item Temporaly
    '                                    myInstructionTmp = New InstructionTO
    '                                    myInstructionTmp.InstructionID = CInt(rsort.Cells("InstructionID").Value)
    '                                    myInstructionTmp.Sequence = CInt(rsort.Cells("Sequence").Value)
    '                                    myInstructionTmp.Timer = CInt(rsort.Cells("Timer").Value)
    '                                    myInstructionTmp.Code = CStr(rsort.Cells("Code").Value)
    '                                    myInstructionTmp.Params = CStr(rsort.Cells("Params").Value)
    '                                    myInstructionTmp.EnableEdition = CBool(rsort.Cells("EnableEdition").Value)
    '                                Else
    '                                    '// inserting the other items
    '                                    myInstruction = New InstructionTO
    '                                    myInstruction.InstructionID = CInt(rsort.Cells("InstructionID").Value)
    '                                    myInstruction.Sequence = CInt(rsort.Cells("Sequence").Value)
    '                                    myInstruction.Timer = CInt(rsort.Cells("Timer").Value)
    '                                    myInstruction.Code = CStr(rsort.Cells("Code").Value)
    '                                    myInstruction.Params = CStr(rsort.Cells("Params").Value)
    '                                    myInstruction.EnableEdition = CBool(rsort.Cells("EnableEdition").Value)
    '                                    myInstructionsAux.Add(myInstruction)
    '                                End If
    '                            End If
    '                        Next

    '                        ' New sorted assignation
    '                        MyClass.SelectedFwScript.Instructions = myInstructionsAux
    '                        myGlobal = MyClass.ShowInstructions(Me.SelectedFwScript.Instructions)

    '                    Case "DOWN"
    '                        For Each rsort As DataGridViewRow In Me.bsInstructionsListDataGridView.Rows
    '                            If Me.SelectedRow(rsort.Cells(0).Value.ToString) Then
    '                                If rsort.Index = Me.bsInstructionsListDataGridView.Rows.Count - 1 Then
    '                                    ' if wish to sort to down, last element is already sorted
    '                                    Exit Function
    '                                Else
    '                                    '// Selected
    '                                    '// Save selected item Temporaly
    '                                    myInstructionTmp = New InstructionTO
    '                                    myInstructionTmp.InstructionID = CInt(rsort.Cells("InstructionID").Value)
    '                                    myInstructionTmp.Sequence = CInt(rsort.Cells("Sequence").Value)
    '                                    myInstructionTmp.Timer = CInt(rsort.Cells("Timer").Value)
    '                                    myInstructionTmp.Code = CStr(rsort.Cells("Code").Value)
    '                                    myInstructionTmp.Params = CStr(rsort.Cells("Params").Value)
    '                                    myInstructionTmp.EnableEdition = CBool(rsort.Cells("EnableEdition").Value)
    '                                End If
    '                            Else
    '                                If rsort.Index > 0 AndAlso Me.SelectedRow(Me.bsInstructionsListDataGridView.Rows(rsort.Index - 1).Cells(0).Value.ToString) Then
    '                                    '// adding following selected item
    '                                    myInstruction = New InstructionTO
    '                                    myInstruction.InstructionID = CInt(rsort.Cells("InstructionID").Value)
    '                                    myInstruction.Sequence = CInt(rsort.Cells("Sequence").Value)
    '                                    myInstruction.Timer = CInt(rsort.Cells("Timer").Value)
    '                                    myInstruction.Code = CStr(rsort.Cells("Code").Value)
    '                                    myInstruction.Params = CStr(rsort.Cells("Params").Value)
    '                                    myInstruction.EnableEdition = CBool(rsort.Cells("EnableEdition").Value)
    '                                    myInstructionsAux.Add(myInstruction)
    '                                    '// adding selected item previously saved
    '                                    If Not myInstructionTmp Is Nothing Then
    '                                        myInstructionsAux.Add(myInstructionTmp)
    '                                    End If
    '                                Else
    '                                    '// inserting the other items
    '                                    myInstruction = New InstructionTO
    '                                    myInstruction.InstructionID = CInt(rsort.Cells("InstructionID").Value)
    '                                    myInstruction.Sequence = CInt(rsort.Cells("Sequence").Value)
    '                                    myInstruction.Timer = CInt(rsort.Cells("Timer").Value)
    '                                    myInstruction.Code = CStr(rsort.Cells("Code").Value)
    '                                    myInstruction.Params = CStr(rsort.Cells("Params").Value)
    '                                    myInstruction.EnableEdition = CBool(rsort.Cells("EnableEdition").Value)
    '                                    myInstructionsAux.Add(myInstruction)
    '                                End If
    '                            End If
    '                        Next

    '                        ' New sorted assignation
    '                        MyClass.SelectedFwScript.Instructions = myInstructionsAux
    '                        myGlobal = MyClass.ShowInstructions(Me.SelectedFwScript.Instructions)
    '                End Select

    '                'uncheck the Syntax OK
    '                Me.SelectedFwScript.SyntaxOK = False
    '                Me.SelectedFwScript.TestedOK = False
    '                For Each R As DataGridViewRow In Me.bsInstructionsListDataGridView.Rows
    '                    R.Cells("SyntaxOK").Value = False
    '                    R.Cells("TestedOK").Value = False
    '                Next

    '                MyClass.CurrentScreenMode = ScreenModes.MODIFIED

    '            End If


    '            '// Ending with sorted item Focused
    '            If seleccion.Length > 0 Then
    '                For Each rdsort2 As DataGridViewRow In Me.bsInstructionsListDataGridView.Rows
    '                    If rdsort2.Cells(0).Value.ToString = seleccion Then
    '                        Me.bsInstructionsListDataGridView.CurrentCell = rdsort2.Cells(3)
    '                        rdsort2.Selected = True
    '                    End If
    '                Next
    '            End If


    '            'SG 10/11/10 Validate changes
    '            'myGlobal = MyClass.ValidateEditionChanges()

    '            If Not myGlobal.HasError Then
    '                ' XBC 24/11/2010
    '                For Each R As DataGridViewRow In Me.bsInstructionsListDataGridView.Rows
    '                    If Not CBool(R.Cells("EnableEdition").Value) Then
    '                        R.Cells("Sequence").ReadOnly = False
    '                        R.Cells("Code").ReadOnly = True
    '                        R.Cells("Params").ReadOnly = True
    '                        R.Cells("SyntaxOK").ReadOnly = True
    '                        R.Cells("TestedOK").ReadOnly = True
    '                    End If
    '                Next R
    '                ' XBC 24/11/2010

    '                Me.bsInstructionsListDataGridView.Focus()
    '                'END SG 10/11/10

    '                MyClass.CurrentScreenMode = ScreenModes.MODIFIED
    '            End If

    '        End If

    '    Catch ex As Exception
    '        myGlobal.HasError = True
    '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
    '        myGlobal.ErrorMessage = ex.Message
    '        MyBase.CreateLogActivity(ex.Message, Me.Name & ".Sort", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        MyBase.ShowMessage(Me.Name & ".Sort", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try

    '    Return myGlobal

    'End Function


    ''' <summary>
    ''' Return if a row is selected
    ''' </summary>
    ''' <param name="pRowValue"></param>
    ''' <returns></returns>
    ''' <remarks>Created by XBC 29/09/2010</remarks>
    Private Function SelectedRow(ByVal pRowValue As String) As Boolean
        Dim returnValue As Boolean = False
        Try
            If Me.bsInstructionsListDataGridView.Rows.Count > 0 Then
                For Each rsort As DataGridViewRow In Me.bsInstructionsListDataGridView.Rows
                    If rsort.Cells(0).Value.ToString = pRowValue Then
                        For Each cellsort As DataGridViewCell In rsort.Cells
                            If cellsort.Selected Then
                                returnValue = True
                                Exit For
                            End If
                        Next
                    End If
                    If returnValue Then Exit For
                Next
            End If

            Return returnValue
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".SelectionRow", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SelectionRow", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Function

#End Region

   

End Class