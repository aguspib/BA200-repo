Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.BL.Framework
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.GlobalEnumerates
'Imports Biosystems.Ax00.CommunicationsSwFw
'Imports System.Configuration
'Imports Biosystems.Ax00.DAL
Imports System.IO

Imports System.Drawing 'SG 03/12/10
Imports System.Text.RegularExpressions
Imports System.Windows.Forms 'SG 03/12/10
Imports Biosystems.Ax00.Controls.UserControls
Imports DevExpress.Skins
Imports DevExpress.UserSkins
Imports DevExpress.XtraEditors
Imports DevExpress.XtraTab

Public Class BSBaseForm

#Region "Declarations"

    'Public SystemsInformationTO As New SystemInfoTO
    'Public GlobalAnalyzerManager As AnalyzerManager 'AG 23/09/2011 - remove this declaration. Mdi (user and service softwares)defines the analyzer manager object MDIAnalyzerManager
    'Public OSCultureInfo As New PCInfoReader.AX00PCOSCultureInfo 'SG 07/10/10

    'Protected Shared OSCultureInfo As PCInfoReader.AX00PCOSCultureInfo 'RH 21/10/2011 Code optimization
    'Protected Shared IsOSCultureInfoInitialized As Boolean = False 'RH 21/10/2011

    'Public GlobalApplicationLayer As ApplicationLayer  'AG 20/04/2010
    'Public GlobalLinkLayer As LinkLayer    'AG 20/04/2010
    Protected Shared RunningAx00MDBackGround As Boolean = False 'RH 03/11/2010

    'Private MsgBoxLookAndFeel As DevExpress.LookAndFeel.UserLookAndFeel

    Protected Structure WINDOWPOS
        Public hwnd As Int32
        Public hWndInsertAfter As Int32
        Public x As Int32
        Public y As Int32
        Public cx As Int32
        Public cy As Int32
        Public flags As Int32
    End Structure

    Protected Const WM_WINDOWPOSCHANGING As Int32 = &H46

    'SGM 22/12/2012
    Public WithEvents FormBack As UiBackground

    Protected MsgParent As Form = Nothing
    Protected isClosingFlag As Boolean = False  'AG + RH 03/04/2012 - True when the screen is closing and the refreshscreen method has to be ignored

    Public CurrentUserLevel As String = "" 'TR 20/04/2012

    Protected Shared ScreenChildShownInProcess As Boolean = False    ' XB 26/11/2013 - Task #1303

    Public WarningMsgDisplayed As Integer = 0   ' XB 24/02/2014 - #1520
#End Region

#Region "Attributes"

    Private IconsPathAttribute As String
    Private LIMSImportFilePathAttribute As String
    Private LIMSImportMemoPathAttribute As String

    Private AnalyzerModelAttr As String = "" 'SG 03/03/11
    Private screenWorkingProcessAttribute As Boolean = False 'AG 28/06/2011 - When this flag is TRUE the screen/app can not be closed due some process is still working
    Protected RefreshDoneField As Boolean = True

    Private applicationMaxMemoryUsageAttribute As Single = 900 'AG 24/02/2014 - BT #1520
    Private SQLMaxMemoryUsageAttribute As Single = 1000        'AG 24/02/2014 - BT #1520

    Private _currentUserNumericalLevel As USER_LEVEL

#End Region

#Region "Constructor" 'SG 03/12/10
    Private Shared skinsLoaded As Boolean = False
    Protected Friend Sub New()

        'Set it BEFORE adding controls, so they're created with the appropriate skin settings, instead of creating them and modifying them later
        If Not skinsLoaded Then
            'Register explicitly BonusSkins assembly and select Skin
            BonusSkins.Register()
            LookAndFeel.SkinName = "Blue"

            SkinManager.EnableFormSkins()
            SkinManager.EnableMdiFormSkins()
            skinsLoaded = True
        End If


        ' This call is required by the Windows Form Designer.
        InitializeComponent()


    End Sub

#End Region

#Region "Set Parent MDI"
    'RH 12/04/2012 Out! Not needed.
    'Protected Friend Sub SetParentMDI(ByRef myMDI As Form)
    '    MyClass.myParentMDI = myMDI
    'End Sub
#End Region

#Region "Properties"

    'SG 04/03/11
    Public Property AnalyzerModel() As String
        Get
            Return AnalyzerModelAttr
        End Get
        Set(ByVal value As String)
            AnalyzerModelAttr = value
        End Set
    End Property

    Public ReadOnly Property IconsPath() As String
        Get
            'AG 21/09/2010
            'IconsPathAttribute = Application.StartupPath & ConfigurationManager.AppSettings("IconsPath").ToString
            'IconsPathAttribute = ConfigurationManager.AppSettings("IconsPath").ToString
            IconsPathAttribute = GlobalBase.ImagesPath
            If IconsPathAttribute.StartsWith("\") AndAlso Not IconsPathAttribute.StartsWith("\\") Then
                IconsPathAttribute = Application.StartupPath & IconsPathAttribute
            End If
            'END AG 21/09/2010

            Return IconsPathAttribute
        End Get
    End Property

    Public ReadOnly Property LIMSImportFilePath() As String
        Get
            'SG 08/03/11 - Get from SWParameters table
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myParams As New SwParametersDelegate

            myGlobalDataTO = myParams.ReadTextValueByParameterName(Nothing, SwParameters.LIMS_IMPORT_PATH.ToString, Nothing)
            If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                LIMSImportFilePathAttribute = CStr(myGlobalDataTO.SetDatos)
            Else
                LIMSImportFilePathAttribute = "\Import\"
            End If

            If LIMSImportFilePathAttribute.StartsWith("\") AndAlso Not LIMSImportFilePathAttribute.StartsWith("\\") Then
                LIMSImportFilePathAttribute = Application.StartupPath & LIMSImportFilePathAttribute
            End If
            Return LIMSImportFilePathAttribute
        End Get
    End Property

    Public ReadOnly Property LIMSImportMemoPath() As String
        Get
            'SG 08/03/11 - Get from SWParameters table
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myParams As New SwParametersDelegate

            myGlobalDataTO = myParams.ReadTextValueByParameterName(Nothing, SwParameters.LIMS_IMPORT_MEMORY_PATH.ToString, Nothing)
            If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                LIMSImportMemoPathAttribute = CStr(myGlobalDataTO.SetDatos)
            Else
                LIMSImportMemoPathAttribute = "\Memo\"
            End If

            If LIMSImportMemoPathAttribute.StartsWith("\") AndAlso Not LIMSImportMemoPathAttribute.StartsWith("\\") Then
                LIMSImportMemoPathAttribute = Application.StartupPath & LIMSImportMemoPathAttribute
            End If
            Return LIMSImportMemoPathAttribute
        End Get
    End Property

    'AG 28/06/2011
    Public Property ScreenWorkingProcess() As Boolean
        Get
            Return screenWorkingProcessAttribute
        End Get
        Set(ByVal value As Boolean)
            screenWorkingProcessAttribute = value
        End Set
    End Property

    'RH 28/03/2012 - Read this property after calling Refresh() method.
    Public ReadOnly Property RefreshDone() As Boolean
        Get
            Return RefreshDoneField
        End Get
    End Property

    'AG 24/02/2014 - BT #1520
    Public Property applicationMaxMemoryUsage As Single
        Get
            Return applicationMaxMemoryUsageAttribute
        End Get
        Set(value As Single)
            applicationMaxMemoryUsageAttribute = value
        End Set
    End Property

    'AG 24/02/2014 - BT #1520
    Public Property SQLMaxMemoryUsage As Single
        Get
            Return SQLMaxMemoryUsageAttribute
        End Get
        Set(value As Single)
            SQLMaxMemoryUsageAttribute = value
        End Set
    End Property

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Updates the value of MsgParent
    ''' </summary>
    ''' <param name="NewMsgParent">New value for MsgParent</param>
    ''' <remarks>
    ''' Created by: RH 08/03/2012
    ''' </remarks>
    Public Sub SetMsgParent(ByRef NewMsgParent As Form)
        MsgParent = NewMsgParent
    End Sub

    ''' <summary>
    ''' Method that creates a Log entry in the Application or System Log
    ''' </summary>
    ''' <param name="Message">Message to save in the Log</param>
    ''' <param name="LogModule">Class and method where the incident happens</param>
    ''' <param name="LogType">Log Type</param>
    ''' <remarks>
    ''' Created by:  TR
    ''' </remarks>
    <Obsolete("Use GlobalBase.CreateLogActivity instead. there's no need to have duplicate implementations of this!")>
    Public Sub CreateLogActivity(ByVal Message As String, ByVal LogModule As String, ByVal LogType As EventLogEntryType, _
                                 ByVal InformSystem As Boolean)
        Try
            GlobalBase.CreateLogActivity(Message, LogModule, LogType, InformSystem)
        Catch ex As Exception
            ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get information from an Exception object and write the error information in the Application Log
    ''' </summary>
    ''' <param name="pException">NET Exception Object</param>
    ''' <remarks>
    ''' Created to replace current calls to CreateLogActivity in Forms
    ''' PENDING: not used yet 
    ''' </remarks>
    <Obsolete("USe GlobalBase.ShowExceptionDetails instead")>
    Public Sub ShowExceptionDetails(ByVal pException As Exception)
        Try
            'Dim myGlobalbase As New GlobalBase
            GlobalBase.ShowExceptionDetails(pException)
        Catch ex As Exception
            ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Public Overridable Sub RefreshScreen(ByVal pRefreshEventType As List(Of UI_RefreshEvents), ByVal pRefreshDS As UIRefreshDS)

    End Sub


    ''' <summary>
    '''Method that gets the Type and Multilanguage Text of the informed Message and shows it in 
    ''' a MessageBox with the Icon and Buttons required for the Message Type
    ''' </summary>
    ''' <param name="pWindowTitle">Text that will appear as Window Title in the Message Box</param>
    ''' <param name="pMessageID">Message Identifier</param>
    ''' <param name="pSystemMessageText">Optional parameter containing additional text to be added to
    '''                                  the Multilanguage Message Text (ex.Message with an exception happens)</param>
    ''' <param name="pOwnerWindow">Optional parameter for the Window that will be the owner of the MessageBox to show</param>
    ''' <remarks>
    ''' Created by:  SA 21/12/2009 - Created without functionality, just to avoid compiling errors. To be developed in Chennai.
    ''' Modified by: BK 24/12/2009 - Create ShowMessage
    '''              TR 12/01/2010 - Change the method result for a DialogResult, and change some declarations inside the method
    '''              SA 11/06/2010 - Changes to get the generic text to be used as Window Title, value of parameter pWindowTitle 
    '''                              will be ignored
    '''              SA 13/07/2010 - Added optional parameter for the Window that will be the owner of the MessageBox to show.
    '''                              If it is not informed, then the owner by default is the main MDI form
    '''              RH 29/06/2011 - Skin feature for MessageBox. Some code optimizations.
    '''              SG 22/06/2012 - Add text parameters (in texts that include $$$ tag)
    '''              SG 18/10/2012 - Add aditional text (+vbcrlf + text)
    '''              SG 26/06/2013 - set the Title from Product Name (Not to be translated!!!)
    '''              DL 12/07/2013 - Add optional parameter pMessageType
    '''              AG 10/02/2014 - Add protection, do not show message if no owner!! (isClosingFlag = True)
    '''              XB 24/02/2014 - Display an owner msg to the user - task #1520
    ''' </remarks>
    Public Overridable Function ShowMessage(ByVal pWindowTitle As String, ByVal pMessageID As String, Optional ByVal pSystemMessageText As String = "", _
                                   Optional ByVal pOwnerWindow As IWin32Window = Nothing, _
                                   Optional ByVal pTextParameters As List(Of String) = Nothing, _
                                   Optional ByVal pAditionalText As String = "", _
                                   Optional ByVal pMessageType As String = "") As DialogResult

        Dim result As DialogResult

        Try
            If Not pMessageID Is Nothing AndAlso String.Compare(pMessageID, "", False) <> 0 Then 'AG 15/03/2012

                result = DialogResult.No

                Dim Messages As New MessageDelegate()
                Dim myMessagesDS As MessagesDS
                Dim myGlobalDataTO As GlobalDataTO

                'Get multilanguage text to be used as Window Title
                Dim windowTitleText As String = String.Empty

                'SGM 26/06/2013 - set the Title from Product Name (Not to be translated!!!)
                windowTitleText = My.Application.Info.Title
                'end SGM 26/06/2013

                'Get type and multilanguage text for the informed Message
                Dim msgText As String = String.Empty

                myGlobalDataTO = Messages.GetMessageDescription(Nothing, pMessageID)

                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myMessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)

                    If (myMessagesDS.tfmwMessages.Rows.Count > 0) Then
                        msgText = myMessagesDS.tfmwMessages(0).MessageText

                        'Additional text to add to the Message Text
                        If Not String.IsNullOrEmpty(pSystemMessageText) Then msgText = msgText & " - " & pSystemMessageText

                        'Window Owner...if it has not been informed, then the owner is the Main MDI Form
                        'RH 12/04/2012 The previous version does not work properly because MyClass.myParentMDI is Nothing
                        If (pOwnerWindow Is Nothing) Then
                            If Me.MdiParent Is Nothing Then
                                pOwnerWindow = Me 'If no parent means it is the MDI
                            Else
                                pOwnerWindow = Me.MdiParent 'If exist mdi parent -> assign it (the MDI)
                            End If
                        End If

                        'Insert Text Parameters SGM 22/06/2012
                        If pTextParameters IsNot Nothing AndAlso pTextParameters.Count > 0 Then
                            If msgText.Contains("$$$") Then
                                Dim res As String = ""
                                Dim sep(pTextParameters.Count - 1) As String
                                For s As Integer = 0 To pTextParameters.Count - 1
                                    sep(s) = "$$$"
                                Next
                                Dim txt As String() = msgText.Split(sep, StringSplitOptions.RemoveEmptyEntries)
                                If txt.Length = pTextParameters.Count + 1 Then
                                    For t As Integer = 0 To txt.Length - 1
                                        If t = pTextParameters.Count - 1 Then
                                            res &= txt(t) & pTextParameters(t)
                                        Else
                                            res &= txt(t)
                                        End If
                                    Next
                                    msgText = res
                                End If
                            End If
                        End If

                        'SGM 18/1/2012
                        If pAditionalText.Length > 0 Then msgText = msgText & vbCrLf & vbCrLf & pAditionalText

                        'DL 12/07/2013
                        Dim myMessageType As String = ""

                        If pMessageType <> "" Then
                            myMessageType = pMessageType
                        Else
                            myMessageType = myMessagesDS.tfmwMessages(0).MessageType
                        End If
                        'DL 12/07/2013

                        ' XB 24/02/2014 - #1520
                        If msgText.Length > 0 Then
                            If msgText.Contains("((") And msgText.Contains("))") Then
                                Dim pos1 As Integer = msgText.IndexOf("((")
                                Dim pos2 As Integer = msgText.IndexOf("))")
                                Dim ExceptionIDHResult As String = msgText.Substring(pos1, pos2 - pos1 + 2)
                                ' Truncate HResult from the msg text to display to the user
                                msgText = msgText.Replace(ExceptionIDHResult, "")
                                ' Catch only the HResult identifier
                                ExceptionIDHResult = ExceptionIDHResult.Substring(2, ExceptionIDHResult.Length - 4)
                                ' Search the Memory problems exceptions
                                Select Case ExceptionIDHResult
                                    Case GlobalConstants.HResult_OutOfMemoryException,
                                         GlobalConstants.HResult_InsufficientMemoryException
                                        ' Display an owner msg to the user
                                        If WarningMsgDisplayed = 0 Then
                                            msgText = GlobalConstants.MAX_APP_MEMORY_USAGE ' GlobalConstants.CATCH_APP_MEMORY_ERR ' XB 20/03/2014 - Out Of Mem also displays MAX_APP_MEMORY_USAGE instead of CATCH_APP_MEMORY_ERR because with the last msg the app not works fine
                                            myMessageType = "Information"
                                            WarningMsgDisplayed += 1

                                            If ExceptionIDHResult = GlobalConstants.HResult_OutOfMemoryException Then
                                                GlobalBase.CreateLogActivity("Display a message instead of a catched 'Out Of Memory Exception'", "BSBaseForm.ShowMessage", EventLogEntryType.Error, False)
                                            Else
                                                GlobalBase.CreateLogActivity("Display a message instead of a catched 'Insufficient Memory Exception'", "BSBaseForm.ShowMessage", EventLogEntryType.Error, False)
                                            End If
                                        Else
                                            Exit Try
                                        End If
                                    Case GlobalConstants.HResult_TimeoutException
                                        ' XB 20/03/2014 - No message is displayed to the User for Timeout Exceptions - #1548
                                        Exit Try

                                        '' Display an owner msg to the user
                                        'If WarningMsgDisplayed = 0 Then
                                        '    msgText = GlobalConstants.MAX_APP_MEMORY_USAGE
                                        '    myMessageType = "Information"
                                        '    WarningMsgDisplayed += 1
                                        '    GlobalBase.CreateLogActivity("Display a message instead of a catched 'Timeout Exception'", "BSBaseForm.ShowMessage", EventLogEntryType.Error, False)
                                        'Else
                                        '    Exit Try
                                        'End If
                                        ' XB 20/03/2014 

                                    Case GlobalConstants.SQLDeadLockException
                                        ' XB 20/03/2014 - No message is displayed to the User for SQL DeadLocked Exceptions - #1548
                                        Exit Try

                                    Case GlobalConstants.ObjRefException, GlobalConstants.NullReferenceException
                                        ' XB 26/03/2014 - No message is displayed to the User for Object or Null Reference Exceptions - #1548
                                        Exit Try

                                End Select
                            End If
                        End If
                        ' XB 24/02/2014

                        'Show message with the proper icon according the Message Type
                        'If (myMessagesDS.tfmwMessages(0).MessageType = "Error") Then
                        If (myMessageType = "Error") Then
                            ' XB 29/05/2014 - Write into the Log what error messages are displayed on screen
                            GlobalBase.CreateLogActivity("This message is shown to the user: '" & msgText & "'", "BSBaseForm.ShowMessage", EventLogEntryType.Information, False)

                            'Error Message 
                            result = MessageBox.Show(pOwnerWindow, msgText, windowTitleText, MessageBoxButtons.OK, MessageBoxIcon.Error)

                            'ElseIf (myMessagesDS.tfmwMessages(0).MessageType = "Information") Then
                        ElseIf (myMessageType = "Information") Then
                            'Information Message 
                            result = MessageBox.Show(pOwnerWindow, msgText, windowTitleText, MessageBoxButtons.OK, MessageBoxIcon.Information)

                            'ElseIf (myMessagesDS.tfmwMessages(0).MessageType = "InfoCancel") Then
                        ElseIf (myMessageType = "InfoCancel") Then
                            'Information Message 
                            result = MessageBox.Show(pOwnerWindow, msgText, windowTitleText, MessageBoxButtons.OKCancel, MessageBoxIcon.Information)

                            'ElseIf (myMessagesDS.tfmwMessages(0).MessageType = "WarnCancel") Then
                        ElseIf (myMessageType = "WarnCancel") Then
                            'Information Message 
                            result = MessageBox.Show(pOwnerWindow, msgText, windowTitleText, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)

                            'ElseIf (myMessagesDS.tfmwMessages(0).MessageType = "FailureAudit") Then
                        ElseIf (myMessageType = "FailureAudit") Then
                            ' XB 29/05/2014 - Write into the Log what error messages are displayed on screen
                            GlobalBase.CreateLogActivity("This message is shown to the user: '" & msgText & "'", "BSBaseForm.ShowMessage", EventLogEntryType.Information, False)

                            'System Error Message - FailureAudit
                            result = MessageBox.Show(pOwnerWindow, msgText, windowTitleText, MessageBoxButtons.OK, MessageBoxIcon.Stop)

                            'ElseIf (myMessagesDS.tfmwMessages(0).MessageType = "Warning") Then
                        ElseIf (myMessageType = "Warning") Then
                            'System Error Message - Warning
                            result = MessageBox.Show(pOwnerWindow, msgText, windowTitleText, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                            'ElseIf (myMessagesDS.tfmwMessages(0).MessageType = "Question") Then
                        ElseIf (myMessageType = "Question") Then
                            'Question Message
                            result = MessageBox.Show(pOwnerWindow, msgText, windowTitleText, MessageBoxButtons.YesNo, MessageBoxIcon.Question)

                            'ElseIf (myMessagesDS.tfmwMessages(0).MessageType = "WarnQuestion") Then
                        ElseIf (myMessageType = "WarnQuestion") Then
                            'Question Message
                            result = MessageBox.Show(pOwnerWindow, msgText, windowTitleText, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

                            'ElseIf (myMessagesDS.tfmwMessages(0).MessageType = "QuesCancel") Then
                        ElseIf (myMessageType = "QuesCancel") Then
                            'Question Message
                            result = MessageBox.Show(pOwnerWindow, msgText, windowTitleText, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)

                            'ElseIf (myMessagesDS.tfmwMessages(0).MessageType = "WQuesCancel") Then
                        ElseIf (myMessageType = "WQuesCancel") Then
                            'Question Message
                            result = MessageBox.Show(pOwnerWindow, msgText, windowTitleText, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning)

                        ElseIf (myMessageType = "AbortRetryCancel") Then
                            'Abort Retry Cancel Message
                            result = MessageBox.Show(pOwnerWindow, msgText, windowTitleText, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Question)

                        ElseIf (myMessageType = "RetryCancel") Then
                            'Retry Cancel Message
                            result = MessageBox.Show(pOwnerWindow, msgText, windowTitleText, MessageBoxButtons.RetryCancel, MessageBoxIcon.Question)
                        End If
                        'DL 12/07/2013
                    End If
                Else
                    'TR 12/01/2010 in case there is an error on getting the message, show the error
                    result = MessageBox.Show(pOwnerWindow, myGlobalDataTO.ErrorMessage, pWindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    result = DialogResult.Abort
                End If

            End If 'AG 15/03/2012

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ShowMessage", EventLogEntryType.Error, False)

        End Try

        Return result
    End Function


    ''' <summary>
    ''' Show several MessageID in one message box
    ''' </summary>
    ''' <param name="pWindowTitle"></param>
    ''' <param name="pMessageIDList "></param>
    ''' <param name="pOwnerWindow"></param>
    ''' <param name="pTextParameters"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' AG 08/04/2011 - created (based on ShowMessage)
    ''' Modified by: RH 29/06/2011 - Skin feature for MessageBox. Some code optimizations.
    ''' Modified by SG: SG 22/06/2012 - Add text parameters (in texts that include $$$ tag)
    ''' </remarks>
    Public Function ShowMultipleMessage(ByVal pWindowTitle As String, ByVal pMessageIDList As List(Of String), _
                            Optional ByVal pOwnerWindow As IWin32Window = Nothing, _
                            Optional ByVal pTextParameters As List(Of String) = Nothing, _
                            Optional ByVal pAditionalText As String = "") As DialogResult

        Dim result As DialogResult
        Try
            result = DialogResult.No

            Dim Messages As New MessageDelegate()
            Dim myMessagesDS As MessagesDS
            Dim myGlobalDataTO As GlobalDataTO

            'Get multilanguage text to be used as Window Title
            Dim windowTitleText As String = String.Empty

            'SG 05/01/11
            Dim windowTitleMessageID As String = String.Empty

            'XBC 03/11/2011
            'Select Case My.Application.Info.ProductName.ToUpper
            '    Case "BAX00"
            '        windowTitleMessageID = GlobalEnumerates.Messages.SHOW_MESSAGE_TITLE_TEXT.ToString()
            '    Case "AX00 SERVICE"
            '        windowTitleMessageID = GlobalEnumerates.Messages.SHOW_MESSAGE_TITLE_TEXT_SRV.ToString()
            'End Select

            'SGM 01/02/2012 - Set that is Service Assembly - Bug #1112
            'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
            If GlobalBase.IsServiceAssembly Then
                windowTitleMessageID = GlobalEnumerates.Messages.SHOW_MESSAGE_TITLE_TEXT_SRV.ToString()
            Else
                windowTitleMessageID = GlobalEnumerates.Messages.SHOW_MESSAGE_TITLE_TEXT.ToString()
            End If
            'XBC 03/11/2011


            myGlobalDataTO = Messages.GetMessageDescription(Nothing, windowTitleMessageID)

            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                myMessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)
                If (myMessagesDS.tfmwMessages.Rows.Count > 0) Then windowTitleText = myMessagesDS.tfmwMessages(0).MessageText
            Else
                'If there is an error getting the title message text, use the one received as parameter
                windowTitleText = pWindowTitle
            End If

            'Get type and multilanguage text for the informed Message
            Dim msgText As String = String.Empty
            Dim msgType As String = String.Empty

            For Each item As String In pMessageIDList
                If String.Compare(item, "", False) <> 0 Then 'AG 09/03/2012
                    myGlobalDataTO = Messages.GetMessageDescription(Nothing, item)

                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myMessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)

                        If (myMessagesDS.tfmwMessages.Rows.Count > 0) Then
                            msgText = msgText & myMessagesDS.tfmwMessages(0).MessageText & vbCrLf

                            If String.IsNullOrEmpty(msgType) Then
                                msgType = myMessagesDS.tfmwMessages(0).MessageType
                            Else
                                Select Case myMessagesDS.tfmwMessages(0).MessageType
                                    Case "Error"
                                        'High Priority case
                                        If msgType <> myMessagesDS.tfmwMessages(0).MessageType Then
                                            msgType = myMessagesDS.tfmwMessages(0).MessageType
                                        End If

                                    Case "Information"
                                        'Low priority case

                                    Case "FailureAudit"
                                        'No multiple messages case

                                    Case "Warning"
                                        'Medium priority case
                                        If msgType = "Information" Then
                                            msgType = myMessagesDS.tfmwMessages(0).MessageType
                                        End If

                                    Case "Question"
                                        'No multiple messages case

                                    Case "WarnQuestion"
                                        'No multiple messages case

                                    Case Else

                                End Select
                            End If
                        End If
                    Else
                        Exit For
                    End If

                Else 'AG 09/03/2012 - If item = "" leave a blank line
                    msgText = msgText & "" & vbCrLf
                End If

            Next

            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                'Window Owner...if it has not been informed, then the owner is the Main MDI Form
                'AG 28/07/2010
                'If (pOwnerWindow Is Nothing) Then pOwnerWindow = New Ax00MainMDI
                'If (pOwnerWindow Is Nothing) Then pOwnerWindow = MyClass.myParentMDI

                'RH 12/04/2012 The previous version does not work properly because MyClass.myParentMDI is Nothing
                If (pOwnerWindow Is Nothing) Then
                    If Me.MdiParent Is Nothing Then
                        pOwnerWindow = Me
                    Else
                        pOwnerWindow = Me.MdiParent
                    End If
                End If

                'Insert Text Parameters SGM 22/06/2012
                If pTextParameters IsNot Nothing AndAlso pTextParameters.Count > 0 Then
                    If msgText.Contains("$$$") Then
                        Dim res As String = ""
                        Dim sep(pTextParameters.Count - 1) As String
                        For s As Integer = 0 To pTextParameters.Count - 1
                            sep(s) = "$$$"
                        Next
                        Dim txt As String() = msgText.Split(sep, StringSplitOptions.RemoveEmptyEntries)
                        If txt.Length = pTextParameters.Count + 1 Then
                            For t As Integer = 0 To txt.Length - 1
                                If t = pTextParameters.Count - 1 Then
                                    res &= txt(t) & pTextParameters(t)
                                Else
                                    res &= txt(t)
                                End If
                            Next
                            msgText = res
                        End If
                    End If
                End If

                'SGM 18/1/2012
                If pAditionalText.Length > 0 Then
                    msgText = msgText & vbCrLf & vbCrLf & pAditionalText
                End If
                'end SGM 18/1/2012

                'Show message with the proper icon according the Message Type
                If (msgType = "Error") Then
                    'Error Message 
                    result = MessageBox.Show(pOwnerWindow, msgText, windowTitleText, MessageBoxButtons.OK, MessageBoxIcon.Error)

                ElseIf (msgType = "Information") Then
                    'Information Message 
                    result = MessageBox.Show(pOwnerWindow, msgText, windowTitleText, MessageBoxButtons.OK, MessageBoxIcon.Information)

                ElseIf (msgType = "FailureAudit") Then
                    'System Error Message - FailureAudit
                    result = MessageBox.Show(pOwnerWindow, msgText, windowTitleText, MessageBoxButtons.OK, MessageBoxIcon.Stop)

                ElseIf (msgType = "Warning") Then
                    'System Error Message - Warning
                    result = MessageBox.Show(pOwnerWindow, msgText, windowTitleText, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                ElseIf (msgType = "Question") Then
                    'Question Message
                    result = MessageBox.Show(pOwnerWindow, msgText, windowTitleText, MessageBoxButtons.YesNo, MessageBoxIcon.Question)

                ElseIf (msgType = "WarnQuestion") Then
                    'Question Message
                    result = MessageBox.Show(pOwnerWindow, msgText, windowTitleText, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

                ElseIf (msgType = "WarnCancel") Then
                    'Information Message 
                    result = MessageBox.Show(pOwnerWindow, msgText, windowTitleText, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)

                End If

            Else
                'TR 12/01/2010 in case there is an error on getting the message, show the error
                result = MessageBox.Show(pOwnerWindow, myGlobalDataTO.ErrorMessage, pWindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                result = DialogResult.Abort
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, "BSBaseForm.ShowMultipleMessage", EventLogEntryType.Error, False)

        End Try

        Return result
    End Function

    ''' <summary>
    ''' Get text of the specified Message in the current application Language
    ''' </summary>
    ''' <param name="pMessageId">Message Identifier</param>
    ''' <param name="pLanguageID">Optional parameter. Language Identifier</param>
    ''' <returns>A String value containing the Message Text</returns>
    ''' <remarks>
    ''' Created by:  TR 19/03/2010
    ''' Modified by: SA 07/07/2010 - Moved here to be accessible to all screens 
    '''              SA 04/11/2010 - Added optional parameter for LanguageID to allow get the message text
    '''                              when the Session object has not been initialized yet (for instance, when a
    '''                              wrong UserName is informed in the Login screen)
    ''' </remarks>
    Public Function GetMessageText(ByVal pMessageId As String, Optional ByVal pLanguageID As String = "") As String
        Dim textMessage As String = String.Empty
        Try
            Dim myMessageDelegate As New MessageDelegate()
            Dim myGlobalDataTO As New GlobalDataTO()

            myGlobalDataTO = myMessageDelegate.GetMessageDescription(Nothing, pMessageId, pLanguageID)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myMessagesDS As New MessagesDS
                myMessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)
                If (myMessagesDS.tfmwMessages.Rows.Count > 0) Then
                    textMessage = myMessagesDS.tfmwMessages(0).MessageText
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, "BSBaseForm.GetMessageText ", EventLogEntryType.Error, False)
        End Try
        Return textMessage
    End Function

    ''' <summary>
    ''' Method that gets information of the Application Session from the Application Session Manager
    ''' </summary>
    ''' <returns>Returns the global transfer object ApplicationInfoSessionTO containing
    '''          the information of the Application Session</returns>
    ''' <remarks>
    ''' Created by:  TR
    ''' </remarks>
    Public Function GetApplicationInfoSession() As ApplicationInfoSessionTO
        Dim myApplicationInfoSession As ApplicationInfoSessionTO
        myApplicationInfoSession = Nothing
        Try
            Dim myApplicationSessionManager As New ApplicationSessionManager
            myApplicationInfoSession = GlobalBase.GetSessionInfo()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, "BSBaseForm.GetApplicationInfoSession", EventLogEntryType.Error, False)
        End Try
        Return myApplicationInfoSession
    End Function

    ''' <summary>
    ''' Method that initializes the information in the Application Session for the logged User
    ''' </summary>
    ''' <returns>True when the Application Session was correctly initialized; 
    '''          otherwise, it returns False</returns>
    ''' <remarks>
    ''' Created by:  TR
    ''' Modified by: SA 06/10/2010 - Added parameter for the Current Application Language
    ''' </remarks>
    Public Function InitializeApplicationInfoSession(ByVal pUserName As String, ByVal pUserLevel As String, _
                                                     ByVal pLanguageID As String) As Boolean
        Dim result As Boolean = False

        Try
            'Initialize Application Session for the logged User
            Dim myApplicationSessionManager As New ApplicationSessionManager
            result = myApplicationSessionManager.InitializeSession(pUserName, pUserLevel, IconsPath, pLanguageID)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, "BSBaseForm.InitializeApplicationInfoSession", EventLogEntryType.Error, False)
        End Try
        Return result
    End Function

    ''' <summary>
    '''  Method that resets the information in the Application Session
    ''' </summary>
    ''' <returns>True when the Application Session was correctly reseted; 
    '''          otherwise, it returns False</returns>
    ''' <remarks>
    ''' Created by:  VR
    ''' </remarks>
    Public Function ResetApplicationInfoSession() As Boolean
        Dim result As Boolean = False
        Try
            Dim myApplicationSessionManager As New ApplicationSessionManager
            result = myApplicationSessionManager.ResetSession()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, "BSBaseForm.ResetApplicationInfoSession", EventLogEntryType.Error, False)
        End Try
        Return result
    End Function

    ''' <summary>
    ''' Validate a if a value is numeric (integer or decimal)
    ''' </summary>
    ''' <param name="pValueToValidate">Value to validate</param>
    ''' <param name="pIsDecimal">Indicates if the number allows decimals</param>
    ''' <returns>True when the informed value is a number of the proper type (INT or DEC)</returns>
    ''' <remarks></remarks>
    Public Function ValidateNumericValue(ByVal pValueToValidate As String, ByVal pKeyascii As Char, _
                                         ByVal pIsDecimal As Boolean) As Boolean
        Dim result As Boolean = True
        Try
            If (String.Compare(pValueToValidate, "", False) <> 0) Then
                'Use regular expressions
                Dim isNumber As Regex
                If (pIsDecimal) Then
                    '[-+]?[0-9]*\.?[0-9]*.
                    isNumber = New Regex("[-+]?[0-9]*\.?[0-9]*.")
                Else
                    isNumber = New Regex("\b\d+(\.\d+)?\b")
                End If
                result = isNumber.Match(pValueToValidate).Success
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ValidateNumericValue ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return result
    End Function

    ''' <summary>
    ''' Get the physical name of the Icon identified for the specified key
    ''' (from table tfmwPreloadedMasterData, SubTable ICON_PATHS in the DB)
    ''' </summary>
    ''' <param name="pIconKey">Icon Identifier</param>
    ''' <returns>The physical name of the Icon identified for the specified key</returns>
    ''' <remarks>
    ''' Created by:  TR 09/04/2010
    ''' </remarks>
    Public Function GetIconName(ByVal pIconKey As String) As String
        Dim myIconName As String = ""
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim getPreloadedMasterData As New PreloadedMasterDataDelegate

            myGlobalDataTO = getPreloadedMasterData.GetSubTableItem(Nothing, PreloadedMasterDataEnum.ICON_PATHS, pIconKey)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myMasterDataDS As New PreloadedMasterDataDS
                myMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                If (myMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                    myIconName = myMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc.Trim
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " GetIconPath ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myIconName
    End Function

    ''' <summary>
    ''' Add the specified Image as a Stream to the informed ImageList
    ''' </summary>
    ''' <param name="pImageList">Image List to add the image</param>
    ''' <param name="pIconName">Physical name of the Image to load</param>
    ''' <remarks>
    ''' Created by:  SA 23/07/2010 
    ''' </remarks>
    Public Sub AddIconToImageList(ByRef pImageList As ImageList, ByVal pIconName As String)
        Try
            Dim iconPath As String = IconsPath & pIconName

            'If (IO.File.Exists(iconPath)) Then
            '    Dim myIconStream As Stream = Nothing
            '    myIconStream = System.IO.File.OpenRead(iconPath)
            '    myIconStream.Flush()

            '    pImageList.Images.Add(pIconName, Image.FromStream(myIconStream))

            '    myIconStream.Close()
            '    myIconStream.Dispose()
            'End If

            'RH 18/10/2010
            'http://visualbasic.about.com/od/usingvbnet/a/disposeobj.htm
            'A Using block guarantees the disposal of one or more such resources when your code is finished with them.
            If (File.Exists(iconPath)) Then
                Using myIconStream As Stream = File.OpenRead(iconPath)
                    myIconStream.Flush()
                    pImageList.Images.Add(pIconName, Image.FromStream(myIconStream))
                    myIconStream.Close()
                End Using
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".AddIconToImageList", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Avoid the use of a group of special characters in an Application  Screen  
    ''' </summary>
    ''' <param name="pCharacter">Entered Characters</param>
    ''' <param name="pSpecialCharacters">Forbidden Characters</param>
    ''' <returns>True if the entered Character is valid; otherwise it returns False</returns>
    ''' <remarks>
    ''' Created by : VR 23/04/2010
    ''' Modified by: AG 27/04/2010 - Place the function as Public in BSBaseForm and enter the SpecialCharacters as a new parameter
    ''' </remarks>
    Public Function ValidateSpecialCharacters(ByVal pCharacter As Char, ByVal pSpecialCharacters As String) As Boolean
        Dim myResult As Boolean = False
        Try
            Dim myValidation As Regex
            myValidation = New Regex(pSpecialCharacters)

            If (myValidation.Match(pCharacter).Success) Then
                myResult = True
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ValidateSpecialCharacters ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Prevents the form window from flickering when it is showed. Use it in the Load event.
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 15/12/2010
    ''' </remarks>
    Public Sub ResetBorder()
        'Application.DoEvents()
        Me.ControlBox = False
        Me.MinimizeBox = False
        Me.MaximizeBox = False
        Me.AutoScaleMode = AutoScaleMode.None
        Me.FormBorderStyle = FormBorderStyle.None
        Me.WindowState = FormWindowState.Normal
        Me.Left = 0
        Me.Top = 0
        Me.Width = 978
        Me.Height = 654
    End Sub

    ''' <summary>
    ''' Prevents the form window from flickering when it is showed. Use it in the Load event.
    ''' </summary>
    ''' <remarks>
    ''' Created by:  XBC 02/05/2011 - Adapted from User sw to Service sw features 
    ''' </remarks>
    Public Sub ResetBorderSRV()
        Application.DoEvents()
        Me.ControlBox = False
        Me.MinimizeBox = False
        Me.MaximizeBox = False
        Me.AutoScaleMode = AutoScaleMode.None
        Me.FormBorderStyle = FormBorderStyle.None
        Me.WindowState = FormWindowState.Normal
        Me.Left = 0
        Me.Top = 0
        Me.Width = 978
        Me.Height = 593
    End Sub

    ''' <summary>
    ''' Get the Help File Path and Name.
    ''' </summary>
    ''' <param name="pTypeID"></param>
    ''' <param name="pCurrentLanguage"></param>
    ''' <returns></returns>
    ''' <remarks>CREATE BY: TR 03/11/2011</remarks>
    Public Function GetHelpFilePath(ByVal pTypeID As HELP_FILE_TYPE, ByVal pCurrentLanguage As String) As String
        Dim myHelpPath As String = ""
        Try
            Dim myHelpFilesSettingDelegate As New HelpFilesSettingDelegate
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myHelpFilesSettingDS As New HelpFilesSettingDS
            myGlobalDataTO = myHelpFilesSettingDelegate.Read(Nothing, pTypeID, pCurrentLanguage)

            If Not myGlobalDataTO.HasError Then
                myHelpFilesSettingDS = DirectCast(myGlobalDataTO.SetDatos, HelpFilesSettingDS)
                If myHelpFilesSettingDS.tfmwHelpFilesSetting.Count = 1 Then
                    myHelpPath = AppDomain.CurrentDomain.BaseDirectory() & _
                    myHelpFilesSettingDS.tfmwHelpFilesSetting(0).HelpFileName
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".GetHelpFilePath ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myHelpPath
    End Function

    ''' <summary>
    ''' Get the Screen Chapter on the help file.
    ''' </summary>
    ''' <param name="pScreenID"></param>
    ''' <returns></returns>
    ''' <remarks>CREATE BY: TR 07/11/2011</remarks>
    Public Function GetScreenChapter(ByVal pScreenID As String) As String
        Dim mySceenHelpChapter As String = ""
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myScreenDS As New ScreenDS
            Dim myScreensDelegate As New ScreensDelegate
            myGlobalDataTO = myScreensDelegate.Read(Nothing, pScreenID)

            If Not myGlobalDataTO.HasError Then
                myScreenDS = DirectCast(myGlobalDataTO.SetDatos, ScreenDS)
                If myScreenDS.tfmwScreens.Count = 1 Then
                    mySceenHelpChapter = myScreenDS.tfmwScreens(0).ScreenHelpChapter
                End If
            Else
                ShowMessage("", myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".GetScreenChapter ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return mySceenHelpChapter
    End Function


#Region "RSAT Directory"

    ''' <summary>
    ''' Get the SAT Report directory, by default is set on the user Desktop in case user
    ''' change the directory the information is search on the User setting table.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>CREATED BY: TR 16/12/2011</remarks>
    Public Function GetSATReportDirectory() As String
        Dim myReportSatDirectory As String = ""
        Try
            Dim myUserSettingDelegate As New UserSettingsDelegate
            Dim myGlobalDataTO As New GlobalDataTO

            'Set the default directory (Current User Desktop)
            myReportSatDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            'Search on UserSetting Table in case the user has change the directory
            myGlobalDataTO = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, "REPORTSAT_DIRECTORY")

            If Not myGlobalDataTO.HasError Then
                'If found then change the value for the Report SAT Directory.
                If String.Compare(DirectCast(myGlobalDataTO.SetDatos, String), "", False) <> 0 Then
                    'Before set the directory validate if path exit, because user can set a removal device.
                    If Directory.Exists(myGlobalDataTO.SetDatos.ToString()) Then
                        myReportSatDirectory = myGlobalDataTO.SetDatos.ToString()
                    Else
                        'Removed the value as String.Empty from User Settint Table
                        SaveSATReportDirectory(String.Empty)
                    End If
                End If
            ElseIf Not myGlobalDataTO.ErrorCode = "MASTER_DATA_MISSING" Then
                ShowMessage("Error", Messages.SYSTEM_ERROR.ToString(), myGlobalDataTO.ErrorMessage)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " GetSATReportDirectory ", EventLogEntryType.Error, _
                                                    GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", Messages.SYSTEM_ERROR.ToString(), ex.Message)
        End Try
        Return myReportSatDirectory
    End Function

    Public Sub SaveSATReportDirectory(ByVal pNewPath As String)
        Try
            Dim myUserSettingDelegate As New UserSettingsDelegate
            Dim myGlobalDataTO As New GlobalDataTO
            myGlobalDataTO = myUserSettingDelegate.Update(Nothing, "REPORTSAT_DIRECTORY", pNewPath)
            If myGlobalDataTO.HasError Then
                ShowMessage("Error", Messages.SYSTEM_ERROR.ToString(), myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " GetSATReportDirectory ", EventLogEntryType.Error, _
                                                    GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", Messages.SYSTEM_ERROR.ToString(), ex.Message)
        End Try
    End Sub

#End Region


    ' XB 26/11/2013 - Task #1303
    'Public MustOverride Sub ExitingScreen() 
    ' XB REMARK : When wants implement this function to all application use Overridable to force be implemented into all screens
    Public Sub ExitingScreen()
        ScreenChildShownInProcess = True
        'Debug.Print("Exiting screen ... ScreenChildShownInProcess=TRUE")
    End Sub

    'Public MustOverride Sub LoadingScreen()
    ' XB REMARK : When wants implement this function to all application use Overridable to force be implemented into all screens
    Public Sub LoadingScreen()
        ScreenChildShownInProcess = True
        'Debug.Print("Loading screen ... ScreenChildShownInProcess=TRUE")
    End Sub

    'Public MustOverride Sub ShownScreen()
    ' XB REMARK : When wants implement this function to all application use Overridable to force be implemented into all screens
    Public Sub ShownScreen()
        ScreenChildShownInProcess = False
        'Debug.Print("Shown screen ... ScreenChildShownInProcess=FALSE")
    End Sub

#End Region

#Region "Events"


    Private Sub BSBaseForm_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles Me.FormClosing
        isClosingFlag = True
    End Sub


    ''' <summary>
    ''' Load Event of Base Form: initialize the information in the Application Session according the logged User
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR
    ''' </remarks>
    Private Sub BSBaseForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

        If Me.GetType().GetInterfaces().Contains(GetType(IPermissionLevel)) Then

            Dim formType As Type = sender.GetType()
            Dim form As IPermissionLevel = CType(sender, IPermissionLevel)
            'Dim myGlobalbase As New GlobalBase
            form.ValidatePermissionLevel(GlobalBase.GetSessionInfo.UserLevelEnum)

        End If

        'Validate if the screen is in design mode to avoid parameters error
        If (Not Me.DesignMode) Then

            'TR 05/03/2012 -Commented to avoid error on application start without database
            'Dim myApplicationInfoSession As ApplicationInfoSessionTO
            ''TR 30/09/2011 -Validate if object is initalize in memory
            'If Not AppDomain.CurrentDomain.GetData("ApplicationInfoSession") Is Nothing Then
            '    'Set the object in memory
            '    myApplicationInfoSession = CType(AppDomain.CurrentDomain.GetData("ApplicationInfoSession"), ApplicationInfoSessionTO)
            'Else
            '    'Initialize the Application Session 
            '    myApplicationInfoSession = GetApplicationInfoSession()
            '    InitializeApplicationInfoSession(myApplicationInfoSession.UserName, myApplicationInfoSession.UserLevel, _
            '                                     myApplicationInfoSession.ApplicationLanguage)
            'End If
            'TR 05/03/2012 -END.

            'SG 07/10/10 get the culture info
            'Dim myGlobalDataTO As GlobalDataTO
            'myGlobalDataTO = PCInfoReader.GetOSCultureInfo()
            'If Not myGlobalDataTO.HasError And Not myGlobalDataTO Is Nothing Then
            '    OSCultureInfo = CType(myGlobalDataTO.SetDatos, PCInfoReader.AX00PCOSCultureInfo)
            'End If
            'END SG 07/10/10

            ''RH 21/10/2011 Code optimization. Run these lines once.
            'If Not IsOSCultureInfoInitialized Then
            '    Dim myGlobalDataTO As GlobalDataTO
            '    myGlobalDataTO = PCInfoReader.GetOSCultureInfo()
            '    If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO Is Nothing Then
            '        OSCultureInfo = CType(myGlobalDataTO.SetDatos, PCInfoReader.AX00PCOSCultureInfo)
            '    End If
            '    IsOSCultureInfoInitialized = True
            'End If

            'AG 22/04/2010 - Move GlobalAnalyzerManager creation to MDI_Form_Load 

            'AG 20/04/2010
            'If AppDomain.CurrentDomain.GetData("GlobalApplicationLayer") Is Nothing Then
            '    GlobalApplicationLayer = New ApplicationLayer
            '    AppDomain.CurrentDomain.SetData("GlobalApplicationLayer", GlobalApplicationLayer)
            'End If

            'If AppDomain.CurrentDomain.GetData("GlobalLinkLayer") Is Nothing Then
            '    GlobalLinkLayer = New LinkLayer
            '    AppDomain.CurrentDomain.SetData("GlobalLinkLayer", GlobalLinkLayer)
            'End If
        End If
    End Sub

    '''' <summary>
    '''' Enter Key works as Tab Key in all fields excepting buttons. Apply for all screens excepting the Main MDI
    '''' </summary>
    '''' <remarks>
    '''' Created by:  DL 06/07/2010
    '''' </remarks>
    'Private Sub BSBaseForm__KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown

    '    If (Me.Name <> "Ax00MainMDI") Then
    '        If (e.KeyCode = Keys.Enter And Me.ActiveControl.GetType.Name.ToString <> "BSButton") Then
    '            Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
    '            e.Handled = True
    '        End If
    '    End If
    'End Sub


    ''' <summary>
    ''' This method is called during message preprocessing to handle dialog characters, such as TAB, RETURN, ESC, and arrow keys. 
    ''' This method is called only if the IsInputKey method indicates that the control is not processing the key. The ProcessDialogKey 
    ''' simply sends the character to the parent's ProcessDialogKey method, or returns false if the control has no parent. 
    ''' The Form class overrides this method to perform actual processing of dialog keys. This method is only called when the control is hosted in a 
    ''' Windows Forms application or as an ActiveX control.
    ''' When overriding the ProcessDialogKey method in a derived class, a control should return true to indicate that it has processed the key. 
    ''' For keys that are not processed by the control, the result of calling the base class's ProcessDialogChar method should be returned. 
    ''' Controls will seldom, if ever, need to override this method.
    ''' </summary>
    ''' <returns>Boolean=True if the key was processed by the control; otherwise, False</returns>
    ''' <remarks>
    ''' Created by:  DL 01/10/2010
    ''' Modified by: AG 10/12/2010 - Do not executed code is ActiveControl is Nothing
    '''              TR 07/02/2012 - New implementation of sending TAB key instead ENTER
    '''              SA 12/03/2012 - Undo last DL changes for screen IWSSampleRequest; they do not work
    '''              MI 30/01/2015 - http://confluence.ginper.local:8090/display/AREA/Don%27t+use+Name+property+of+WinForms+controls
    '''              AC 03/06/2015 - Fix Button Enter
    ''' </remarks>
    Protected Overrides Function ProcessDialogKey(ByVal keyData As Keys) As Boolean
        If (Me.ActiveControl IsNot Nothing) Then
            Dim returnIsPressed = (keyData = Keys.Return)
            If returnIsPressed AndAlso ProcessEnterAsTab() AndAlso (Me.ActiveControl.GetType IsNot GetType(BSButton)) Then
                Return MyBase.ProcessDialogKey(Keys.Tab)
            End If
        End If
        Return MyBase.ProcessDialogKey(keyData)
    End Function


    ''' <summary>
    ''' This function indicates if this form needs to process Enter key as a TAB key to navigate through application controls.
    ''' </summary>
    ''' <returns>A boolean, true means ENTER are TABS.</returns>
    ''' <remarks>By default, any form that does not provide its own implementation, will process Enter as a TAB if it is not a MDI container</remarks>
    Protected Overridable Function ProcessEnterAsTab() As Boolean
        Return Not Me.IsMdiContainer
    End Function

    'SGM 05/01/2012
    Private Sub FormBack_Exception(ByVal ex As Exception) Handles FormBack.ExceptionHappened
        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".FormBack", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        ShowMessage(Me.Name & ".FormBack", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    End Sub

#End Region

#Region "MULTILANGUAGE - TEMPORARY; COMMENT AFTER FINISH"

    '''' <summary>
    '''' For an specific application Screen, go through all Controls having a multilanguage text and add
    '''' the information in a typed DataSet with structure of the temporary table tmpScreenTextes
    '''' </summary>
    '''' <param name="pControlCollection">Collection containing all Screen Controls</param>
    '''' <param name="pScreenToolTipControl">ToolTip Control used by the Screen</param>
    '''' <param name="pScreenTextsDS">Typed DataSet in which all texts loaded for the different Screen Controls
    ''''                              will be added</param>
    '''' <remarks>
    '''' Created by: SA 01/10/2010
    '''' </remarks>
    'Private Sub GetScreenTexts(ByVal pControlCollection As System.Windows.Forms.Control.ControlCollection, _
    '                           ByVal pScreenToolTipControl As Biosystems.Ax00.Controls.UserControls.BSToolTip, _
    '                           ByRef pScreenTextsDS As DataSet)
    '    Try
    '        Dim screenControl As New Control
    '        For Each screenControl In pControlCollection
    '            Dim ctrlType() As String = Split(screenControl.GetType().ToString, ".")
    '            Dim controlType As String = ctrlType(ctrlType.Count - 1)

    '            Dim myTableRow As DataRow
    '            Select Case controlType
    '                Case "Label", "BSLabel"
    '                    If (screenControl.Text <> "") Then
    '                        Dim isTitle As Boolean = (screenControl.BackColor = Color.LightSteelBlue)
    '                        myTableRow = pScreenTextsDS.Tables(0).NewRow

    '                        myTableRow.Item("ScreenName") = Me.Name
    '                        myTableRow.Item("ControlName") = screenControl.Name
    '                        myTableRow.Item("ControlType") = controlType
    '                        myTableRow.Item("Title") = isTitle
    '                        myTableRow.Item("ColumnIndex") = -1

    '                        If (isTitle) Then
    '                            myTableRow.Item("EnglishText") = screenControl.Text
    '                        ElseIf (Mid(screenControl.Text, Len(screenControl.Text), 1) <> ":") Then
    '                            myTableRow.Item("EnglishText") = screenControl.Text
    '                        Else
    '                            myTableRow.Item("EnglishText") = Mid(screenControl.Text, 1, Len(screenControl.Text) - 1)
    '                        End If

    '                        myTableRow.Item("ResourceID") = screenControl.Name

    '                        pScreenTextsDS.Tables(0).Rows.Add(myTableRow)
    '                    End If

    '                Case "RadioButton", "BSRadioButton", "CheckBox", "BSCheckbox"
    '                    myTableRow = pScreenTextsDS.Tables(0).NewRow

    '                    myTableRow.Item("ScreenName") = Me.Name
    '                    myTableRow.Item("ControlName") = screenControl.Name
    '                    myTableRow.Item("ControlType") = controlType
    '                    myTableRow.Item("Title") = False
    '                    myTableRow.Item("ColumnIndex") = -1
    '                    myTableRow.Item("EnglishText") = screenControl.Text
    '                    myTableRow.Item("ResourceID") = screenControl.Name

    '                    pScreenTextsDS.Tables(0).Rows.Add(myTableRow)

    '                Case "Button", "BSButton"
    '                    If (screenControl.Text <> "") And (screenControl.Text <> "<") And (screenControl.Text <> "<<") And _
    '                       (screenControl.Text <> ">") And (screenControl.Text <> ">>") Then
    '                        myTableRow = pScreenTextsDS.Tables(0).NewRow

    '                        myTableRow.Item("ScreenName") = Me.Name
    '                        myTableRow.Item("ControlName") = screenControl.Name
    '                        myTableRow.Item("ControlType") = controlType
    '                        myTableRow.Item("Title") = False
    '                        myTableRow.Item("ColumnIndex") = -1
    '                        myTableRow.Item("EnglishText") = screenControl.Text
    '                        myTableRow.Item("ResourceID") = screenControl.Name

    '                        pScreenTextsDS.Tables(0).Rows.Add(myTableRow)
    '                    End If

    '                    If (Not pScreenToolTipControl.GetToolTip(screenControl) Is Nothing) AndAlso _
    '                       (pScreenToolTipControl.GetToolTip(screenControl) <> "") Then
    '                        myTableRow = pScreenTextsDS.Tables(0).NewRow

    '                        myTableRow.Item("ScreenName") = Me.Name
    '                        myTableRow.Item("ControlName") = screenControl.Name
    '                        myTableRow.Item("ControlType") = "ToolTip"
    '                        myTableRow.Item("Title") = False
    '                        myTableRow.Item("ColumnIndex") = -1
    '                        myTableRow.Item("EnglishText") = pScreenToolTipControl.GetToolTip(screenControl)
    '                        myTableRow.Item("ResourceID") = screenControl.Name & "_TT"

    '                        pScreenTextsDS.Tables(0).Rows.Add(myTableRow)
    '                    End If

    '                Case "DataGridView"
    '                    Dim myDataGrid As New DataGridView
    '                    myDataGrid = DirectCast(screenControl, DataGridView)

    '                    For i As Integer = 0 To myDataGrid.ColumnCount - 1
    '                        If (myDataGrid.Columns.Item(i).Visible) And (myDataGrid.Columns.Item(i).HeaderText <> "") Then
    '                            myTableRow = pScreenTextsDS.Tables(0).NewRow

    '                            myTableRow.Item("ScreenName") = Me.Name
    '                            myTableRow.Item("ControlName") = screenControl.Name
    '                            myTableRow.Item("ControlType") = controlType
    '                            myTableRow.Item("Title") = False
    '                            myTableRow.Item("ColumnIndex") = i
    '                            myTableRow.Item("EnglishText") = myDataGrid.Columns.Item(i).HeaderText
    '                            myTableRow.Item("ResourceID") = screenControl.Name & "_" & i

    '                            pScreenTextsDS.Tables(0).Rows.Add(myTableRow)
    '                        End If
    '                    Next

    '                Case "BSDataGridView"
    '                    Dim myDataGrid As New Biosystems.Ax00.Controls.UserControls.BSDataGridView
    '                    myDataGrid = DirectCast(screenControl, Biosystems.Ax00.Controls.UserControls.BSDataGridView)

    '                    For i As Integer = 0 To myDataGrid.ColumnCount - 1
    '                        If (myDataGrid.Columns.Item(i).Visible) And (myDataGrid.Columns.Item(i).HeaderText <> "") Then
    '                            myTableRow = pScreenTextsDS.Tables(0).NewRow

    '                            myTableRow.Item("ScreenName") = Me.Name
    '                            myTableRow.Item("ControlName") = screenControl.Name
    '                            myTableRow.Item("ControlType") = controlType
    '                            myTableRow.Item("Title") = False
    '                            myTableRow.Item("ColumnIndex") = i
    '                            myTableRow.Item("EnglishText") = myDataGrid.Columns.Item(i).HeaderText
    '                            myTableRow.Item("ResourceID") = screenControl.Name & "_" & i

    '                            pScreenTextsDS.Tables(0).Rows.Add(myTableRow)
    '                        End If
    '                    Next

    '                Case "ListView"
    '                    Dim myListView As New ListView
    '                    myListView = DirectCast(screenControl, ListView)

    '                    For i As Integer = 0 To myListView.Columns.Count - 1
    '                        If (myListView.Columns(i).Width <> 0) Then
    '                            myTableRow = pScreenTextsDS.Tables(0).NewRow

    '                            myTableRow.Item("ScreenName") = Me.Name
    '                            myTableRow.Item("ControlName") = screenControl.Name
    '                            myTableRow.Item("ControlType") = controlType
    '                            myTableRow.Item("Title") = False
    '                            myTableRow.Item("ColumnIndex") = i
    '                            myTableRow.Item("EnglishText") = myListView.Columns(i).Text
    '                            myTableRow.Item("ResourceID") = screenControl.Name & "_" & i

    '                            pScreenTextsDS.Tables(0).Rows.Add(myTableRow)
    '                        End If
    '                    Next

    '                Case "BSListView"
    '                    Dim myListView As New Biosystems.Ax00.Controls.UserControls.BSListView
    '                    myListView = DirectCast(screenControl, Biosystems.Ax00.Controls.UserControls.BSListView)

    '                    For i As Integer = 0 To myListView.Columns.Count - 1
    '                        If (myListView.Columns(i).Width <> 0) Then
    '                            myTableRow = pScreenTextsDS.Tables(0).NewRow

    '                            myTableRow.Item("ScreenName") = Me.Name
    '                            myTableRow.Item("ControlName") = screenControl.Name
    '                            myTableRow.Item("ControlType") = controlType
    '                            myTableRow.Item("Title") = False
    '                            myTableRow.Item("ColumnIndex") = i
    '                            myTableRow.Item("EnglishText") = myListView.Columns(i).Text
    '                            myTableRow.Item("ResourceID") = screenControl.Name & "_" & i

    '                            pScreenTextsDS.Tables(0).Rows.Add(myTableRow)
    '                        End If
    '                    Next

    '                Case "GroupBox", "BSGroupBox", "TabControl", "BSTabControl", "TabPage", "Panel", "BSPanel", "SplitContainer", "SplitterPanel"
    '                    If (screenControl.Text <> "") Then
    '                        myTableRow = pScreenTextsDS.Tables(0).NewRow

    '                        myTableRow.Item("ScreenName") = Me.Name
    '                        myTableRow.Item("ControlName") = screenControl.Name
    '                        myTableRow.Item("ControlType") = controlType
    '                        myTableRow.Item("Title") = False
    '                        myTableRow.Item("ColumnIndex") = -1
    '                        myTableRow.Item("EnglishText") = screenControl.Text
    '                        myTableRow.Item("ResourceID") = screenControl.Name

    '                        pScreenTextsDS.Tables(0).Rows.Add(myTableRow)
    '                    End If

    '                    'This type of Controls are containers of other Controls: get also the Texts for those child Controls
    '                    GetScreenTexts(screenControl.Controls, pScreenToolTipControl, pScreenTextsDS)

    '                Case "BSFormula"
    '                    Dim myFormula As New Biosystems.Ax00.Controls.UserControls.BSFormula
    '                    myFormula = DirectCast(screenControl, Biosystems.Ax00.Controls.UserControls.BSFormula)

    '                    myTableRow = pScreenTextsDS.Tables(0).NewRow
    '                    myTableRow.Item("ScreenName") = Me.Name
    '                    myTableRow.Item("ControlName") = screenControl.Name
    '                    myTableRow.Item("ControlType") = controlType
    '                    myTableRow.Item("Title") = False
    '                    myTableRow.Item("ColumnIndex") = -1
    '                    myTableRow.Item("EnglishText") = myFormula.FormulaTitle.ToString.Replace(":", "")
    '                    myTableRow.Item("ResourceID") = "FormulaTitle"
    '                    pScreenTextsDS.Tables(0).Rows.Add(myTableRow)

    '                    myTableRow = pScreenTextsDS.Tables(0).NewRow
    '                    myTableRow.Item("ScreenName") = Me.Name
    '                    myTableRow.Item("ControlName") = screenControl.Name
    '                    myTableRow.Item("ControlType") = controlType
    '                    myTableRow.Item("Title") = False
    '                    myTableRow.Item("ColumnIndex") = -1
    '                    myTableRow.Item("EnglishText") = myFormula.SampleTypeTitle.ToString.Replace(":", "")
    '                    myTableRow.Item("ResourceID") = "SampleTypeTitle"
    '                    pScreenTextsDS.Tables(0).Rows.Add(myTableRow)

    '                    myTableRow = pScreenTextsDS.Tables(0).NewRow
    '                    myTableRow.Item("ScreenName") = Me.Name
    '                    myTableRow.Item("ControlName") = screenControl.Name
    '                    myTableRow.Item("ControlType") = controlType
    '                    myTableRow.Item("Title") = False
    '                    myTableRow.Item("ColumnIndex") = -1
    '                    myTableRow.Item("EnglishText") = myFormula.StandardTestsTitle.ToString.Replace(":", "")
    '                    myTableRow.Item("ResourceID") = "StandardTestsTitle"
    '                    pScreenTextsDS.Tables(0).Rows.Add(myTableRow)

    '                    myTableRow = pScreenTextsDS.Tables(0).NewRow
    '                    myTableRow.Item("ScreenName") = Me.Name
    '                    myTableRow.Item("ControlName") = screenControl.Name
    '                    myTableRow.Item("ControlType") = controlType
    '                    myTableRow.Item("Title") = False
    '                    myTableRow.Item("ColumnIndex") = -1
    '                    myTableRow.Item("EnglishText") = myFormula.CalculatedTestsTitle.ToString.Replace(":", "")
    '                    myTableRow.Item("ResourceID") = "CalculatedTestsTitle"
    '                    pScreenTextsDS.Tables(0).Rows.Add(myTableRow)

    '                    myTableRow = pScreenTextsDS.Tables(0).NewRow
    '                    myTableRow.Item("ScreenName") = Me.Name
    '                    myTableRow.Item("ControlName") = screenControl.Name
    '                    myTableRow.Item("ControlType") = controlType
    '                    myTableRow.Item("Title") = False
    '                    myTableRow.Item("ColumnIndex") = -1
    '                    myTableRow.Item("EnglishText") = myFormula.DelFormulaMemberToolTip.ToString
    '                    myTableRow.Item("ResourceID") = "DelFormulaMemberToolTip"
    '                    pScreenTextsDS.Tables(0).Rows.Add(myTableRow)

    '                    myTableRow = pScreenTextsDS.Tables(0).NewRow
    '                    myTableRow.Item("ScreenName") = Me.Name
    '                    myTableRow.Item("ControlName") = screenControl.Name
    '                    myTableRow.Item("ControlType") = controlType
    '                    myTableRow.Item("Title") = False
    '                    myTableRow.Item("ColumnIndex") = -1
    '                    myTableRow.Item("EnglishText") = myFormula.ClearFormulaToolTip.ToString
    '                    myTableRow.Item("ResourceID") = "ClearFormulaToolTip"
    '                    pScreenTextsDS.Tables(0).Rows.Add(myTableRow)

    '                Case "BSDoubleList"
    '                    Dim myDList As New Biosystems.Ax00.Controls.UserControls.BSDoubleList
    '                    myDList = DirectCast(screenControl, Biosystems.Ax00.Controls.UserControls.BSDoubleList)

    '                    myTableRow = pScreenTextsDS.Tables(0).NewRow
    '                    myTableRow.Item("ScreenName") = Me.Name
    '                    myTableRow.Item("ControlName") = screenControl.Name
    '                    myTableRow.Item("ControlType") = controlType
    '                    myTableRow.Item("Title") = False
    '                    myTableRow.Item("ColumnIndex") = -1
    '                    myTableRow.Item("EnglishText") = myDList.SelectedElementsTitle.ToString.Replace(":", "")
    '                    myTableRow.Item("ResourceID") = "SelectedElementsTitle"
    '                    pScreenTextsDS.Tables(0).Rows.Add(myTableRow)

    '                    myTableRow = pScreenTextsDS.Tables(0).NewRow
    '                    myTableRow.Item("ScreenName") = Me.Name
    '                    myTableRow.Item("ControlName") = screenControl.Name
    '                    myTableRow.Item("ControlType") = controlType
    '                    myTableRow.Item("Title") = False
    '                    myTableRow.Item("ColumnIndex") = -1
    '                    myTableRow.Item("EnglishText") = myDList.SelectableElementsTitle.ToString.Replace(":", "")
    '                    myTableRow.Item("ResourceID") = "SelectableElementsTitle"
    '                    pScreenTextsDS.Tables(0).Rows.Add(myTableRow)

    '                    myTableRow = pScreenTextsDS.Tables(0).NewRow
    '                    myTableRow.Item("ScreenName") = Me.Name
    '                    myTableRow.Item("ControlName") = screenControl.Name
    '                    myTableRow.Item("ControlType") = controlType
    '                    myTableRow.Item("Title") = False
    '                    myTableRow.Item("ColumnIndex") = -1
    '                    myTableRow.Item("EnglishText") = myDList.SelectAllToolTip.ToString
    '                    myTableRow.Item("ResourceID") = "SelectAllToolTip"
    '                    pScreenTextsDS.Tables(0).Rows.Add(myTableRow)

    '                    myTableRow = pScreenTextsDS.Tables(0).NewRow
    '                    myTableRow.Item("ScreenName") = Me.Name
    '                    myTableRow.Item("ControlName") = screenControl.Name
    '                    myTableRow.Item("ControlType") = controlType
    '                    myTableRow.Item("Title") = False
    '                    myTableRow.Item("ColumnIndex") = -1
    '                    myTableRow.Item("EnglishText") = myDList.SelectSomeToolTip.ToString
    '                    myTableRow.Item("ResourceID") = "SelectSomeToolTip"
    '                    pScreenTextsDS.Tables(0).Rows.Add(myTableRow)

    '                    myTableRow = pScreenTextsDS.Tables(0).NewRow
    '                    myTableRow.Item("ScreenName") = Me.Name
    '                    myTableRow.Item("ControlName") = screenControl.Name
    '                    myTableRow.Item("ControlType") = controlType
    '                    myTableRow.Item("Title") = False
    '                    myTableRow.Item("ColumnIndex") = -1
    '                    myTableRow.Item("EnglishText") = myDList.UnselectAllToolTip.ToString
    '                    myTableRow.Item("ResourceID") = "UnselectAllToolTip"
    '                    pScreenTextsDS.Tables(0).Rows.Add(myTableRow)

    '                    myTableRow = pScreenTextsDS.Tables(0).NewRow
    '                    myTableRow.Item("ScreenName") = Me.Name
    '                    myTableRow.Item("ControlName") = screenControl.Name
    '                    myTableRow.Item("ControlType") = controlType
    '                    myTableRow.Item("Title") = False
    '                    myTableRow.Item("ColumnIndex") = -1
    '                    myTableRow.Item("EnglishText") = myDList.UnselectSomeToolTip.ToString
    '                    myTableRow.Item("ResourceID") = "UnselectSomeToolTip"
    '                    pScreenTextsDS.Tables(0).Rows.Add(myTableRow)
    '            End Select
    '        Next
    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, "BSBaseForm.GetScreenTexts ", EventLogEntryType.Error, False)
    '        ShowMessage("BSBaseForm.GetScreenTexts ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    '''' <summary>
    '''' Create a typed DataSet with structure of the temporary table tmpScreenTexts
    '''' </summary>
    '''' <returns>Typed DataSet with structure of the temporary table tmpScreenTexts</returns>
    '''' <remarks>
    '''' Created by:  SA 01/10/2010
    '''' </remarks>
    'Private Function CreateTempDataSet() As DataSet
    '    Dim myDataSet As New DataSet
    '    Try
    '        Dim myTableColumn As DataColumn
    '        Dim myTable As DataTable = New DataTable("tmpScreenTexts")

    '        myTableColumn = New DataColumn()
    '        myTableColumn.DataType = System.Type.GetType("System.String")
    '        myTableColumn.ColumnName = "ScreenName"
    '        myTable.Columns.Add(myTableColumn)

    '        myTableColumn = New DataColumn()
    '        myTableColumn.DataType = System.Type.GetType("System.String")
    '        myTableColumn.ColumnName = "ControlName"
    '        myTable.Columns.Add(myTableColumn)

    '        myTableColumn = New DataColumn()
    '        myTableColumn.DataType = System.Type.GetType("System.String")
    '        myTableColumn.ColumnName = "ControlType"
    '        myTable.Columns.Add(myTableColumn)

    '        myTableColumn = New DataColumn()
    '        myTableColumn.DataType = System.Type.GetType("System.Boolean")
    '        myTableColumn.ColumnName = "Title"
    '        myTable.Columns.Add(myTableColumn)

    '        myTableColumn = New DataColumn()
    '        myTableColumn.DataType = System.Type.GetType("System.Int32")
    '        myTableColumn.ColumnName = "ColumnIndex"
    '        myTable.Columns.Add(myTableColumn)

    '        myTableColumn = New DataColumn()
    '        myTableColumn.DataType = System.Type.GetType("System.String")
    '        myTableColumn.ColumnName = "EnglishText"
    '        myTable.Columns.Add(myTableColumn)

    '        myTableColumn = New DataColumn()
    '        myTableColumn.DataType = System.Type.GetType("System.String")
    '        myTableColumn.ColumnName = "ResourceID"
    '        myTable.Columns.Add(myTableColumn)

    '        myDataSet.Tables.Add(myTable)
    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, "BSBaseForm.CreateTempDataSet ", EventLogEntryType.Error, False)
    '        ShowMessage("BSBaseForm.CreateTempDataSet ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    '    Return myDataSet
    'End Function

    '''' <summary>
    '''' Go through the typed DataSet containing all screen controls requiring a Multilanguage Text an insert
    '''' them in the temporary table tmpScreenTexts 
    '''' </summary>
    '''' <param name="pScreenToolTipControl">ToolTip Control used by the Screen</param>
    '''' <remarks>
    '''' Created by:  SA 01/10/2010
    '''' </remarks>
    'Public Sub InsertScreenTexts(ByVal pScreenToolTipControl As Biosystems.Ax00.Controls.UserControls.BSToolTip)
    '    Try
    '        Dim tmpScreenTextsDS As New DataSet
    '        tmpScreenTextsDS = CreateTempDataSet()

    '        GetScreenTexts(Me.Controls, pScreenToolTipControl, tmpScreenTextsDS)

    '        If (tmpScreenTextsDS.Tables(0).Rows.Count > 0) Then
    '            Dim resultData As New GlobalDataTO
    '            Dim dbConnection As New SqlClient.SqlConnection

    '            resultData = DAOBase.GetOpenDBTransaction(Nothing)
    '            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
    '                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

    '                Dim insertOK As Boolean = True
    '                For i As Integer = 0 To tmpScreenTextsDS.Tables(0).Rows.Count - 1
    '                    If (tmpScreenTextsDS.Tables(0).Rows(i).Item("ControlName").ToString <> "bsGetTextsButton") Then
    '                        Dim cmdText As String = ""
    '                        cmdText = " INSERT INTO tmpScreenTexts (ScreenName, ControlName, ControlType, EnglishText, ResourceID, Title, ColumnIndex) " & _
    '                                  " VALUES ('" & tmpScreenTextsDS.Tables(0).Rows(i).Item("ScreenName").ToString & "', " & _
    '                                           "'" & tmpScreenTextsDS.Tables(0).Rows(i).Item("ControlName").ToString & "', " & _
    '                                           "'" & tmpScreenTextsDS.Tables(0).Rows(i).Item("ControlType").ToString & "', " & _
    '                                           "'" & tmpScreenTextsDS.Tables(0).Rows(i).Item("EnglishText").ToString.Replace("'", "''") & "', " & _
    '                                           "SUBSTRING('" & tmpScreenTextsDS.Tables(0).Rows(i).Item("ResourceID").ToString & "', 0, 29), " & _
    '                                                 Convert.ToInt32(IIf(Convert.ToBoolean(tmpScreenTextsDS.Tables(0).Rows(i).Item("Title")), 1, 0)) & ", " & _
    '                                                 Convert.ToInt32(tmpScreenTextsDS.Tables(0).Rows(i).Item("ColumnIndex")) & ")"

    '                        Dim dbCmd As New SqlClient.SqlCommand
    '                        dbCmd.Connection = dbConnection
    '                        dbCmd.CommandText = cmdText

    '                        If (dbCmd.ExecuteNonQuery() <> 1) Then
    '                            insertOK = False
    '                            Exit For
    '                        End If
    '                    End If
    '                Next

    '                If (insertOK) Then
    '                    DAOBase.CommitTransaction(dbConnection)
    '                Else
    '                    DAOBase.RollbackTransaction(dbConnection)
    '                End If
    '            End If
    '        End If
    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, "BSBaseForm.InsertScreenTexts ", EventLogEntryType.Error, False)
    '        ShowMessage("BSBaseForm.InsertScreenTexts ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub
#End Region

#Region "To delete??"
    ''' <summary>
    ''' Method use to open form
    ''' </summary>
    ''' <param name="FormName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFormFromList(ByVal FormName As String) As Boolean
        'TODO: Revisit this functionality. This function does 2 things:
        '   1.- Checks if a mdi child with a given name is contained in a MDI container form
        '   2.- If it is, it shows it and returns true
        '   3.- Otherwise it returns false
        '
        'Flaws: If it's called GetForm, it should return the form.
        '       If it shows the form, it should be called ShowForm or similar
        '       If it's meant to tell if the form is on the children list, it should be called IsformContained or similar
        '       If it needs to do both things (rare), it should be called something like TryToShowFormByName, and return true or false on success.
        '       It does not check if the method is called from a inherited non MDI container form, which would make no sense at all.
        Dim result As Boolean = False
        Try
            For Each MyForm As Form In Me.MdiChildren
                If MyForm.Name = FormName Then
                    result = True
                    MyForm.BringToFront() ' show form
                End If
            Next
            Return result
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
        Return result

    End Function

#End Region


    Private Sub BSBaseForm_FormClosed(ByVal sender As Object, ByVal e As FormClosedEventArgs) Handles MyBase.FormClosed
        Try
            'RH 18/04/2012 Do not release controls in common forms.
            'Release them in their own FormClosed event
            'ToDo: Get the Common project name from a value, not from a static string
            'If in the future the common project name is changed, the comparison will lost it validity.
            'If Me.ProductName <> "PresentationCOM" Then
            '    ReleaseUnManageControls(Me.Controls)
            'End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BSBaseForm_FormClosed", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BSBaseForm_FormClosed", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Release some unmanage elements used on form
    ''' Image Types.
    ''' </summary>
    ''' <param name="pControls"></param>
    ''' <remarks>CREATED BY: TR 12/04/2012 </remarks>
    <Obsolete("This method is useless")>
    Protected Sub ReleaseUnManageControls(ByVal pControls As Control.ControlCollection)
        Try
            For Each myControl As Control In pControls
                'Application.DoEvents()
                If myControl.Controls.Count > 0 Then
                    ReleaseUnManageControls(myControl.Controls)
                ElseIf Not IsNothing(myControl) Then
                    Select Case myControl.GetType().Name
                        Case "BSButton"
                            If Not IsNothing(DirectCast(myControl, BSButton).Image) Then
                                DirectCast(myControl, BSButton).Image = Nothing
                            End If
                            Exit Select

                        Case "Button"
                            If Not IsNothing(DirectCast(myControl, Button).Image) Then
                                DirectCast(myControl, Button).Image = Nothing
                            End If
                            Exit Select

                        Case "BSRImage"
                            If Not IsNothing(DirectCast(myControl, BSRImage).Image) Then
                                DirectCast(myControl, BSRImage).Image = Nothing
                            End If
                            Exit Select

                        Case "BSPictureBox"
                            If Not IsNothing(DirectCast(myControl, BSPictureBox).Image) Then
                                DirectCast(myControl, BSPictureBox).Image = Nothing
                            End If
                            Exit Select

                        Case "PictureEdit"
                            If Not IsNothing(DirectCast(myControl, PictureEdit).Image) Then
                                DirectCast(myControl, PictureEdit).Image = Nothing
                            End If
                            Exit Select

                        Case "XtraTabPage"
                            If Not IsNothing(DirectCast(myControl, XtraTabPage).Appearance.PageClient.Image) Then
                                DirectCast(myControl, XtraTabPage).Appearance.PageClient.Image = Nothing
                            End If
                            Exit Select

                    End Select
                End If
            Next

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " EnableDisableControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", Messages.SYSTEM_ERROR.ToString, ex.Message) 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message)
        End Try

    End Sub

End Class