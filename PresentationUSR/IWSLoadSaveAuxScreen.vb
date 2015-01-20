Option Strict On
Option Explicit On
Option Infer On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Public Class IWSLoadSaveAuxScreen
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Attributes"
    Private IDAttribute As Integer
    Private NameAttribute As String
    Private RotorTypeAttribute As String
    Private SourceButtonAttribute As String             'AG  26/01/2010
    Private ScreenUseAttribute As String = "VROTORS"    'GDS 06/04/2010
    Private ActiveWSAttribute As String = ""            'SA  23/09/2010
    Private ActiveAnalyzerAttribute As String = ""      'SA  29/08/2012
#End Region

#Region "Properties"
    Public Property RotorType() As String
        Get
            Return RotorTypeAttribute
        End Get
        Set(ByVal value As String)
            RotorTypeAttribute = value
        End Set
    End Property

    'AG 26/01/2010
    Public Property SourceButton() As String
        Get
            Return SourceButtonAttribute
        End Get
        Set(ByVal value As String)
            SourceButtonAttribute = value
        End Set
    End Property

    'AG 26/01/2010
    Public Property NameProperty() As String
        Get
            Return NameAttribute
        End Get
        Set(ByVal value As String)
            NameAttribute = value
        End Set
    End Property

    'AG 26/01/2010
    Public Property IDProperty() As Integer
        Get
            Return IDAttribute
        End Get
        Set(ByVal value As Integer)
            IDAttribute = value
        End Set
    End Property

    'GDS 06/04/2010
    Public Property ScreenUse() As String
        Get
            Return ScreenUseAttribute
        End Get
        Set(ByVal value As String)
            ScreenUseAttribute = value
        End Set
    End Property

    'SA 23/09/2010
    Public WriteOnly Property ActiveWorkSession() As String
        Set(ByVal value As String)
            ActiveWSAttribute = value
        End Set
    End Property

    'SA 29/08/2012
    Public WriteOnly Property ActiveAnalyzer() As String
        Set(ByVal value As String)
            ActiveAnalyzerAttribute = value
        End Set
    End Property
#End Region

#Region "Methods"
    ''' <summary>
    ''' Not allow move form and mantain the center location in center parent
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 27/07/2011
    ''' </remarks>
    Protected Overrides Sub WndProc(ByRef m As Message)
        Try
            If (m.Msg = WM_WINDOWPOSCHANGING) Then
                Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, GetType(WINDOWPOS)), WINDOWPOS)

                Dim mySize As Size = IAx00MainMDI.Size
                Dim myLocation As Point = IAx00MainMDI.Location
                If (Not Me.MdiParent Is Nothing) Then
                    mySize = Me.Parent.Size
                    myLocation = Me.Parent.Location
                End If

                pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2)
                pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2)
                Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
            End If

            MyBase.WndProc(m)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WndProc " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "WndProc", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Inform out properties for the ID and Name of the selected Element to save, load or delete
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK  30/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              AG  08/01/2010 - Hide no!!, use Close (Tested OK)
    '''              AG  26/01/2010 - Changes in virtual rotor selection removing all ReagentSamplePositioning form references
    '''              GDS 07/04/2010 - Added code for SAVEDWS ScreenUseAttribute and for DELETE
    '''              SA  07/06/2010 - Code moved from the button event
    '''              SA  23/09/2010 - Changed the process for SAVE a Saved WS: when a WorkSessionID has been informed (it means
    '''                               the screen was opened from the main MDI Form), the saving is executed here due to in that
    '''                               case the screen is opened as an MDIChild. Added the Dispose() after all Close().
    '''              RH  18/10/2010 - Removed the Dispose after every Close (Let the Garbage Collector do that work. When you close a form, 
    '''                               its Dispose method is called automatically. See http://visualbasic.about.com/od/usingvbnet/a/disposeobj.htm)
    '''              SA  19/10/2010 - Changes due to function ExistsSavedWS returns now a GlobalDataTO instead an Integer
    '''              RH  20/12/2010 - Changed the way of closing the screen, opening Monitor screen when needed 
    '''              SA  08/03/2012 - Code for each different action allowed in this screen moved to individual functions
    ''' </remarks>
    Private Sub AcceptSelection()
        Try
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            'Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            Select Case SourceButtonAttribute
                Case "SAVE"
                    If (ScreenUseAttribute = "VROTORS") Then
                        SaveVirtualRotor()
                    ElseIf (ScreenUseAttribute = "SAVEDWS") Then
                        SaveWorkSession()
                    End If

                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                    GlobalBase.CreateLogActivity("IWSLoadSaveAuxScreen Save WS (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                    "IWSLoadSaveAuxScreen.AcceptSelection", EventLogEntryType.Information, False)
                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                Case "LOAD"
                    If (ScreenUseAttribute = "VROTORS") Then
                        LoadVirtualRotor()

                    ElseIf (ScreenUseAttribute = "SAVEDWS") Then
                        LoadWorkSession()
                    End If
                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                    GlobalBase.CreateLogActivity("IWSLoadSaveAuxScreen LOAD WS (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                    "IWSLoadSaveAuxScreen.AcceptSelection", EventLogEntryType.Information, False)
                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            End Select
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "AcceptSelection " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".AcceptSelection", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID"> The current Language of Application </param>
    ''' <remarks>
    ''' Created by:  PG 13/10/2010 - We have to differentiate between Rotor and Work Session
    '''                              We use the same screen by Save and Delete
    ''' Modified by: TR 28/03/2012 - Set the title label depending on source button (LOAD OR SAVE)
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For CheckBox, RadioButtons...
            If (ScreenUseAttribute = "SAVEDWS") Then
                bsNameSelectionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LoadSaveAux_SavedWS", pLanguageID) + ":"
                Select Case SourceButtonAttribute
                    Case "SAVE"
                        bsTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_WSPrep_OpenSaveWS", pLanguageID)
                    Case "LOAD"
                        bsTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_LoadSavedWS", pLanguageID)
                End Select
            Else
                bsNameSelectionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LoadSaveAux_Vrotor", pLanguageID) + ":"
                Select Case SourceButtonAttribute
                    Case "SAVE"
                        bsTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_RotorPos_OpenSaveVRotor", pLanguageID)
                    Case "LOAD"
                        bsTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_LoadVRotor", pLanguageID)
                End Select
            End If

            'For Tooltips...
            bsScreenToolTips.SetToolTip(bsAcceptButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save&Close", pLanguageID))
            bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel&Close", pLanguageID))

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Configure the Screen Labels according the functionality for which the screen is opened: Virtual Rotors or Saved Work Sessions.  
    ''' Additionally:
    ''' ** When the Screen is opened to select an existing elements and load it (SourceButtonAttribute=LOAD), show and fill the ComboBox
    ''' ** When the Screen is opened to select an existing elements and delete it (SourceButtonAttribute=DELETE), show and fill the ComboBox
    ''' ** When the Screen is opened to save an element (SourceButtonAttribute=SAVE), show the TextBox
    ''' </summary>
    ''' <remarks>
    ''' Modified by: DL  21/01/2010 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              AG  26/01/2010 - Changes in Virtual Rotor Selection: all ReagentSamplePositioning form references was removed
    '''              GDS 06/04/2010 - Added new functionality for Load/Save Work Sessions; added functionality for DELETE 
    '''              SA  07/06/2010 - Code move from the Load event; added call to method to load Icons for graphical buttons
    '''              PG  13/10/2010 - Get the current language
    '''              DL  28/07/2011 - Center the screen regarding its parent form
    ''' </remarks>
    Private Sub InitializeScreen()
        Try

            'Center the screen regarding its parent
            Dim mySize As Size = IAx00MainMDI.Size
            Dim myLocation As Point = IAx00MainMDI.Location
            If (Not Me.MdiParent Is Nothing) Then
                mySize = Me.Parent.Size
                myLocation = Me.Parent.Location
            End If
            Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2))

            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Get Icons for Screen Buttons
            PrepareButtons()

            'Set multilanguage texts for all screen labels and tooltips...
            GetScreenLabels(currentLanguage)

            'Check source button
            Select Case SourceButtonAttribute
                Case "SAVE"
                    bsElementTextBox.Select()

                    If (ScreenUseAttribute = "VROTORS") Then
                        Select Case RotorTypeAttribute
                            Case "SAMPLES", "REAGENTS"
                                'Check that any VirtualRotorName are filled
                                If (NameAttribute <> "") Then
                                    bsElementTextBox.Text = NameAttribute
                                End If

                                bsAcceptButton.Enabled = (NameAttribute <> "")
                        End Select

                    ElseIf (ScreenUseAttribute = "SAVEDWS") Then
                        If (NameAttribute <> "") Then
                            bsElementTextBox.Text = NameAttribute
                        End If

                        bsAcceptButton.Enabled = (NameAttribute <> "")
                    End If

                    bsElementsComboBox.Visible = False
                    bsElementTextBox.Visible = True
                    bsElementTextBox.ContextMenuStrip = New ContextMenuStrip

                Case "LOAD", "DELETE"
                    bsElementsComboBox.Visible = True
                    bsElementTextBox.Visible = False

                    If (ScreenUseAttribute = "VROTORS") Then
                        'Load ComboBox of existing Virtual Rotors of the specified Type
                        SelectVirtualRotor(RotorTypeAttribute)

                    ElseIf (ScreenUseAttribute = "SAVEDWS") Then
                        'Load ComboBox of existing Saved Work Sessions
                        SelectSavedWS()
                    End If
            End Select
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "InitializeScreen " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "InitializeScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Return ID and Name of the selected Virtual Rotor
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 08/03/2012 - Code moved from AcceptSelection
    ''' </remarks>
    Private Sub LoadVirtualRotor()
        Try
            Select Case RotorTypeAttribute
                Case "SAMPLES", "REAGENTS"
                    NameAttribute = bsElementsComboBox.Text
                    IDAttribute = CType(bsElementsComboBox.SelectedValue.ToString(), Integer)
            End Select

            Me.DialogResult = Windows.Forms.DialogResult.OK
            If (Not Me.MdiParent Is Nothing) Then
                'Open the WS Monitor form and close this one
                IAx00MainMDI.OpenMonitorForm(Me)
            Else
                Me.Close()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadVirtualRotor", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadVirtualRotor", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Return ID and Name of the selected saved Work Session
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 08/03/2012
    ''' </remarks>
    Private Sub LoadWorkSession()
        Try
            NameAttribute = bsElementsComboBox.Text
            IDAttribute = CType(bsElementsComboBox.SelectedValue.ToString(), Integer)

            Me.DialogResult = Windows.Forms.DialogResult.OK
            If (Not Me.MdiParent Is Nothing) Then
                'Before opening WS Preparation Screen, activate button for Reset WS
                IAx00MainMDI.bsTSResetSessionButton.Enabled = True

                'A Saved WS was loaded, open the screen of WS Preparation after inform the needed properties
                IWSSampleRequest.ActiveAnalyzer = IAx00MainMDI.ActiveAnalyzer
                IWSSampleRequest.ActiveWorkSession = IAx00MainMDI.ActiveWorkSession
                IWSSampleRequest.ActiveWSStatus = IAx00MainMDI.ActiveStatus
                IWSSampleRequest.WSLoadedID = IDProperty
                IWSSampleRequest.WSLoadedName = NameProperty

                IAx00MainMDI.OpenMDIChildForm(IWSSampleRequest)
            End If
            Me.Close()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadWorkSession", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadWorkSession", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Search Icons for screen buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 07/06/2010 
    ''' Modified by: DL 16/06/2010 - Set the proper Icons for each screen button
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'ACCEPT Button
            auxIconName = GetIconName("ACCEPT1")
            If (auxIconName <> "") Then
                bsAcceptButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'CANCEL Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsExitButton.Image = Image.FromFile(iconPath & auxIconName)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & " PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Save a Virtual Rotor
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 08/03/2012 - Code moved from AcceptSelection
    ''' Modified by: TR 11/04/2012 - Disable form while the saving is in execution to avoid use of screen buttons
    ''' </remarks>
    Private Sub SaveVirtualRotor()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myVirtualRotor As New VirtualRotorsDelegate

            Select Case RotorTypeAttribute
                Case "SAMPLES", "REAGENTS"
                    NameAttribute = bsElementTextBox.Text

                    'Verify if the informed name corresponds to an existing Virtual Rotor
                    myGlobalDataTO = myVirtualRotor.ExistVRotor(Nothing, RotorTypeAttribute, NameAttribute)
                    If (Not myGlobalDataTO.HasError) Then
                        Me.Enabled = False

                        If (myGlobalDataTO.SetDatos Is Nothing) Then
                            IDAttribute = 0
                            'The informed name does not corresponds to an existing one
                            Me.DialogResult = Windows.Forms.DialogResult.OK
                            Me.Close()
                        Else
                            'Get the VRotor Identifier
                            IDAttribute = DirectCast(myGlobalDataTO.SetDatos, Integer)

                            'The Virtual Rotor exists; inform User the Virtual Rotor will be overwritten
                            If (ShowMessage(Me.Name, GlobalEnumerates.Messages.OVERWRITE_FILE.ToString) = DialogResult.Yes) Then
                                Me.DialogResult = DialogResult.OK
                                Me.Close()
                            End If

                        End If
                    Else
                        'Error verifying if the Virtual Rotor already exists
                        ShowMessage(Me.Name & ".SaveVirtualRotor", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
                    End If
            End Select
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "SaveVirtualRotor " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveVirtualRotor", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            Me.Enabled = True
        End Try
    End Sub

    ''' <summary>
    ''' Save a Work Session
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 08/03/2012 - Code moved from AcceptSelection
    ''' Modified by: TR 11/04/2012 - Disable form while the saving is in execution to avoid use of screen buttons
    '''              SA 29/08/2012 - Inform the AnalyzerID when calling function GetOrderTestsForWS in WorkSessionsDelegate
    ''' </remarks>
    Private Sub SaveWorkSession()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim mySavedWS As New SavedWSDelegate

            'Verify if the informed name corresponds to an existing Saved WS
            IDAttribute = -1
            NameAttribute = bsElementTextBox.Text

            myGlobalDataTO = mySavedWS.ExistsSavedWS(Nothing, NameAttribute)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim mySavedWSDS As SavedWSDS
                mySavedWSDS = DirectCast(myGlobalDataTO.SetDatos, SavedWSDS)
                If (mySavedWSDS.tparSavedWS.Rows.Count = 1) Then IDAttribute = mySavedWSDS.tparSavedWS(0).SavedWSID
            Else
                'Error verifying if there is already a Saved WS with the informed name
                ShowMessage(Me.Name & ".SaveWorkSession", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If

            Dim cancelSaving As Boolean = False
            If (IDAttribute <> -1) Then
                'The Saved Work Session exists; inform User the Virtual Rotor will be overwritten
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.OVERWRITE_FILE.ToString) = DialogResult.No) Then
                    cancelSaving = True
                End If
            End If

            If (Not cancelSaving) Then
                Me.Enabled = False

                'Active WorkSession Property is informed only when the screen was opened as MDIChild
                '(from the main MDI Form). When that property is not informed, then the WS saving is
                'executed in the screen of WS Preparation using the content of the local WorkSessionResultsDS
                If (ActiveWSAttribute = "") Then
                    Me.DialogResult = DialogResult.OK

                    If (Not Me.MdiParent Is Nothing) Then
                        'Open the WS Monitor form and close this one
                        IAx00MainMDI.OpenMonitorForm(Me)
                    Else
                        Me.Close()
                    End If
                Else
                    Dim myWSDelegate As New WorkSessionsDelegate

                    myGlobalDataTO = myWSDelegate.GetOrderTestsForWS(Nothing, ActiveWSAttribute, ActiveAnalyzerAttribute)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        Dim myWorkSessionResultDS As WorkSessionResultDS = DirectCast(myGlobalDataTO.SetDatos, WorkSessionResultDS)

                        myGlobalDataTO = mySavedWS.Save(Nothing, NameAttribute, myWorkSessionResultDS, IDAttribute)
                        If (Not myGlobalDataTO.HasError) Then
                            'SavedWS was saved; the screen is closed
                            If (Not Me.MdiParent Is Nothing) Then
                                'Open the WS Monitor form and close this one
                                IAx00MainMDI.OpenMonitorForm(Me)
                            Else
                                Me.Close()
                            End If
                        Else
                            ShowMessage(Me.Name & ".SaveWorkSession", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                        End If
                    Else
                        'Error getting the list of previously requested Order Tests
                        ShowMessage(Me.Name & ".SaveWorkSession", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "SaveWorkSession " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveWorkSession", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the ComboBox of Saved WorkSessions (used when ScreenUse=SAVEDWS and SourceButton=LOAD or DELETE)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  GDS 08/04/2010
    ''' Modified by: RH  20/12/2010 - Changed the way of closing the screen, opening Monitor screen when needed 
    ''' </remarks>
    Public Sub SelectSavedWS()
        Try
            Dim result As New GlobalDataTO
            Dim myObj As New SavedWSDelegate

            result = myObj.GetAll(Nothing)
            If (Not result.HasError And Not result.SetDatos Is Nothing) Then
                Dim mySavedWSDS As New SavedWSDS
                mySavedWSDS = DirectCast(result.SetDatos, SavedWSDS)

                If (mySavedWSDS.tparSavedWS.Rows.Count > 0) Then
                    bsElementsComboBox.DataSource = mySavedWSDS.tparSavedWS

                    bsElementsComboBox.ValueMember = "SavedWSID"
                    bsElementsComboBox.DisplayMember = "SavedWSName"
                Else
                    'Show the Error Message - There are not WS to Load
                    ShowMessage(Me.Name, GlobalEnumerates.Messages.WS_NOT_FOUND.ToString(), "", Me)

                    If (Not Me.MdiParent Is Nothing) Then
                        'Open the WS Monitor form and close this one
                        IAx00MainMDI.OpenMonitorForm(Me)
                    Else
                        'Me.Opacity = 0 'Because the form could be still opening, so avoid the flickering.
                        Application.DoEvents()
                        Me.Close()
                    End If
                End If
            Else
                'Show the Error Message
                If (result.ErrorCode <> "") Then
                    ShowMessage(Me.Name & ".SelectSavedWS", result.ErrorCode, result.ErrorMessage, Me)
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "SelectSavedWS " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SelectSavedWS", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the ComboBox of Virtual Rotors of the specified type 
    ''' (used when ScreenUse=VROTORS and SourceButton=LOAD or DELETE)
    ''' </summary>
    ''' <param name="pRotorType">Type of Virtual Rotors that has to be loaded</param>
    ''' <remarks>
    ''' Modified by: BK 30/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              SA 07/06/2010 - Add message and close the screen where there are not Virtual Rotors of the specified type
    '''              RH  18/10/2010 - Removed the Dispose after every Close (Let the Garbage Collector do that work. When you close a form, 
    '''                               its Dispose method is called automatically. See http://visualbasic.about.com/od/usingvbnet/a/disposeobj.htm)
    ''' </remarks>
    Public Sub SelectVirtualRotor(ByVal pRotorType As String)
        Try
            Dim result As New GlobalDataTO
            Dim myObj As New VirtualRotorsDelegate

            result = myObj.GetVRotorsByRotorType(Nothing, RotorType)
            If (Not result.HasError And Not result.SetDatos Is Nothing) Then
                Dim myVirtualRotorsDS As New VirtualRotorsDS
                myVirtualRotorsDS = DirectCast(result.SetDatos, VirtualRotorsDS)

                If (myVirtualRotorsDS.tparVirtualRotors.Rows.Count > 0) Then
                    bsElementsComboBox.DataSource = myVirtualRotorsDS.tparVirtualRotors

                    bsElementsComboBox.ValueMember = "VirtualRotorID"
                    bsElementsComboBox.DisplayMember = "VirtualRotorName"
                Else
                    'Show the Error Message - There are not Virtual Rotors to Load
                    ShowMessage(Me.Name & ".SelectVirtualRotor", GlobalEnumerates.Messages.NO_VIRTUAL_ROTORS.ToString, "", Me)
                    Me.Close()
                End If
            Else
                'Show the Error Message
                ShowMessage(Me.Name & ".SelectVirtualRotor", result.ErrorCode, result.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", ".SelectVirtualRotor " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SelectVirtualRotor", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

#Region "Events"

    ''' <summary>
    ''' When the screen ESC key is pressed, the screen is closed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 09/11/2010
    ''' </remarks>
    Private Sub WSLoadSaveAuxScreen_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                'RH 04/07/2011 Escape key should do exactly the same operations as bsExitButton_Click()
                bsExitButton.PerformClick()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".WSLoadSaveAuxScreen_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".WSLoadSaveAuxScreen_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try


    End Sub

    Private Sub IWSLoadSaveAuxScreen_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        Dim StartTime As DateTime = Now
        'Dim myLogAcciones As New ApplicationLogManager()
        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        InitializeScreen()

        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        GlobalBase.CreateLogActivity("IWSLoadSaveAuxScreen Load\Save WS Screen (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                        "IWSLoadSaveAuxScreen.IWSLoadSaveAuxScreen_Load", EventLogEntryType.Information, False)
        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

    End Sub

    ''' <summary>
    ''' Accept selection and close the screen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsAcceptButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAcceptButton.Click
        'RH 17/12/2010 This line introduces a bug so, remove it
        'Me.Opacity = 0

        AcceptSelection()
    End Sub

    ''' <summary>
    ''' Close the screen when button Cancel is clicked
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 21/01/2010
    ''' Modified by: SA 23/09/2010 - Added the Dispose after the Close
    '''              RH 18/10/2010 - Removed the Dispose after the Close
    '''              RH 17/12/2010 - Changed the way of closing the screen, opening Monitor screen when needed 
    '''              TR 14/02/2012 - Changed the closing logic to shown the discard pending changes message
    '''              SA 08/03/2012 - Undo the previous change, discard pending changes message has not sense in this screen
    ''' </remarks>
    Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            'TR 11/04/2012 -Disable form on close to avoid any button press.
            Me.Enabled = False

            If (Not Me.MdiParent Is Nothing) Then
                If (Not Me.Tag Is Nothing) Then
                    'A PerformClick() method was executed
                    Me.Close()
                Else
                    'Normal button click
                    'Open the WS Monitor form and close this one
                    IAx00MainMDI.OpenMonitorForm(Me)
                End If
            Else
                Me.Close()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "bsExitButton_Click " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsExitButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Control availability of ACCEPT Button when the Screen was opened for saving (ScreenButtonAttribute=SAVE)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 21/01/2010
    ''' </remarks>
    Private Sub bsElementTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsElementTextBox.TextChanged
        bsAcceptButton.Enabled = (Not String.IsNullOrEmpty(bsElementTextBox.Text.Trim))
    End Sub

#End Region
End Class