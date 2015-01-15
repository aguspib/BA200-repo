Option Explicit On
Option Strict On


Imports Biosystems.Ax00.Global
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Globalization

Namespace Biosystems.Ax00.Controls.UserControls

    ''' <summary>
    ''' User control designed for displaying current data related to the installed ISE Module
    ''' </summary>
    ''' <remarks>Created by SGM 7/02/2012</remarks>
    Public Class BSISEMonitorPanel2
        Inherits UserControl

#Region "Constructor"

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            MyClass.myCultureInfo = My.Computer.Info.InstalledUICulture
        End Sub



#End Region

#Region "Overrides"
        Protected Overrides Sub OnPaint(ByVal pe As System.Windows.Forms.PaintEventArgs)
            MyBase.OnPaint(pe)

            'Add your custom paint code here
        End Sub
#End Region

#Region "Public Enumerates"

        Public Enum LabelElements
            None

            RP_Title
            RP_InstallDate
            RP_ExpireDate
            RP_RemainingVolume
            RP_InitialVolume
            RP_Calibrator
            RP_Volume

            EL_Title
            EL_InstallDate
            EL_TestsCompleted
            EL_Reference
            EL_Sodium
            EL_Potassium
            EL_Chlorine
            EL_Lithium


            CAL_Title
            CAL_LastDate
            CAL_Results
            CAL_Electrodes
            CAL_Pumps
            CAL_Clean

            TUB_Pump
            TUB_Fluid


        End Enum

        Public Enum WarningElements
            None

            RP_NotInstalled
            RP_Expired
            RP_Depleted
            RP_WrongBiosystemsCode
            RP_CleanPackInstalled

            EL_NotInstalled
            EL_ReplaceRecommended

            CAL_Recommended
            CAL_Error
            CAL_CleanRecommended

            TUB_NotInstalled

        End Enum

        Public Enum IconImages
            None
            Ok
            Warning
            Error_
            Locked
        End Enum
#End Region

#Region "Declarations"
        Private myCultureInfo As CultureInfo
#End Region

#Region "Attributes"
        Private ImagesDataAttr As New ImageList
        Private WarningsDataAttr As New Dictionary(Of WarningElements, String)
#End Region

#Region "Public Properties"


        Public WriteOnly Property LabelsData() As Dictionary(Of LabelElements, String)
            Set(ByVal value As Dictionary(Of LabelElements, String))
                MyClass.LoadScreenLabels(value)
            End Set
        End Property



        Public WriteOnly Property WarnigsData() As Dictionary(Of WarningElements, String)
            Set(ByVal value As Dictionary(Of WarningElements, String))
                WarningsDataAttr = value
            End Set
        End Property

        Public Property ImagesData() As ImageList
            Get
                Return ImagesDataAttr
            End Get
            Set(ByVal value As ImageList)
                ImagesDataAttr = value
            End Set
        End Property

#End Region

#Region "Private Properties"

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>Created by SGM 17/02/2012</remarks>
        Public Sub RefreshFieldsData(ByVal pISEMonitorData As ISEMonitorTO)
            Try

                Me.InitializeFields()
                Me.InitializeWarnings()
                Me.InitializeIcons()

                If Not pISEMonitorData Is Nothing AndAlso pISEMonitorData.HasData Then

                    With pISEMonitorData
                        '
                        ' Reagents Pack
                        '
                        If .RP_InstallationDate <> Nothing Then

                            If .RP_HasBioCode Then

                                If Not .RP_CleanPackInstalled Then

                                    Me.RPInstallDateTextEdit.Text = .RP_InstallationDate.ToString(MyClass.myCultureInfo.DateTimeFormat.ShortDatePattern)

                                    If .RP_ExpirationDate <> Nothing Then
                                        Me.RPExpireDateTextEdit.Text = .RP_ExpirationDate.ToString(MyClass.myCultureInfo.DateTimeFormat.ShortDatePattern)
                                        If .RP_IsExpired Then
                                            Me.RPExpiredIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                                            Me.RPExpiredWarningLabel.Text = MyClass.GetWarningLabel(WarningElements.RP_Expired)
                                        End If
                                    Else 'pendiente de definir si la fecha es nula. de momento se toma por expirado
                                        Me.RPExpiredIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                                        Me.RPExpiredWarningLabel.Text = MyClass.GetWarningLabel(WarningElements.RP_Expired)
                                    End If

                                    If .RP_InitialVolA >= 0 Then
                                        Me.RPInitialCalATextEdit.Text = .RP_InitialVolA.ToString
                                        If .RP_RemainingVolA >= 0 Then
                                            Dim myValue As Integer = MyClass.GetPercent(.RP_InitialVolA, .RP_RemainingVolA)
                                            Me.RPRemainingCalATextEdit.Text = "%" & myValue.ToString
                                        End If
                                        If Not .RP_IsEnoughVolA Then
                                            Me.RPCalAIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                                            Me.RPCalAWarningLabel.Text = MyClass.GetWarningLabel(WarningElements.RP_Depleted) & " (<%5)"
                                        End If
                                    End If

                                    If .RP_InitialVolB >= 0 Then
                                        Me.RPInitialCalBTextEdit.Text = .RP_InitialVolB.ToString
                                        If .RP_RemainingVolB >= 0 Then
                                            Dim myValue As Integer = MyClass.GetPercent(.RP_InitialVolB, .RP_RemainingVolB)
                                            Me.RPRemainingCalBTextEdit.Text = "%" & myValue.ToString
                                        End If
                                        If Not .RP_IsEnoughVolB Then
                                            Me.RPCalBIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                                            Me.RPCalBWarningLabel.Text = MyClass.GetWarningLabel(WarningElements.RP_Depleted) & " (<%5)"
                                        End If
                                    End If

                                Else
                                    Me.RPInstalledIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                                    Me.RPNotInstalledWarningLabel.Text = MyClass.GetWarningLabel(WarningElements.RP_CleanPackInstalled)
                                End If
                            Else
                                Me.RPInstalledIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                                Me.RPNotInstalledWarningLabel.Text = MyClass.GetWarningLabel(WarningElements.RP_WrongBiosystemsCode)
                            End If

                        Else
                            Me.RPInstalledIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                            Me.RPNotInstalledWarningLabel.Text = MyClass.GetWarningLabel(WarningElements.RP_NotInstalled)
                        End If


                        '
                        ' Electrodes
                        '
                        If .REF_Data.Installed Then
                            Me.ELRefInstallTextEdit.Text = .REF_Data.InstallationDate.ToString(MyClass.myCultureInfo.DateTimeFormat.ShortDatePattern)
                            Me.ELRefTestTextEdit.Text = .REF_Data.TestCount.ToString
                            If .REF_Data.IsOverUsed Or .REF_Data.IsExpired Then
                                Me.ELRefIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                                Me.ELRefWarningLabel.Text = MyClass.GetWarningLabel(WarningElements.EL_ReplaceRecommended)
                            End If
                        Else
                            Me.ELRefIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                            Me.ELRefWarningLabel.Text = MyClass.GetWarningLabel(WarningElements.EL_NotInstalled)
                        End If

                        If .NA_Data.Installed Then
                            Me.ELNaInstallTextEdit.Text = .NA_Data.InstallationDate.ToString(MyClass.myCultureInfo.DateTimeFormat.ShortDatePattern)
                            Me.ELNaTestTextEdit.Text = .NA_Data.TestCount.ToString
                            If .NA_Data.IsOverUsed Or .NA_Data.IsExpired Then
                                Me.ELNaIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                                Me.ELNaWarningLabel.Text = MyClass.GetWarningLabel(WarningElements.EL_ReplaceRecommended)
                            End If
                        Else
                            Me.ELNaWarningLabel.Text = MyClass.GetWarningLabel(WarningElements.EL_NotInstalled)
                            Me.ELNaIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                        End If

                        If .K_Data.Installed Then
                            Me.ELKInstallTextEdit.Text = .K_Data.InstallationDate.ToString(MyClass.myCultureInfo.DateTimeFormat.ShortDatePattern)
                            Me.ELKTestTextEdit.Text = .K_Data.TestCount.ToString
                            If .K_Data.IsOverUsed Or .K_Data.IsExpired Then
                                Me.ELKIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                                Me.ELKWarningLabel.Text = MyClass.GetWarningLabel(WarningElements.EL_ReplaceRecommended)
                            End If
                        Else
                            Me.ELKWarningLabel.Text = MyClass.GetWarningLabel(WarningElements.EL_NotInstalled)
                            Me.ELKIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                        End If

                        If .CL_Data.Installed Then
                            Me.ELClInstallTextEdit.Text = .CL_Data.InstallationDate.ToString(MyClass.myCultureInfo.DateTimeFormat.ShortDatePattern)
                            Me.ELClTestTextEdit.Text = .CL_Data.TestCount.ToString
                            If .CL_Data.IsOverUsed Or .CL_Data.IsExpired Then
                                Me.ELClIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                                Me.ELClWarningLabel.Text = MyClass.GetWarningLabel(WarningElements.EL_ReplaceRecommended)
                            End If
                        Else
                            Me.ELClWarningLabel.Text = MyClass.GetWarningLabel(WarningElements.EL_NotInstalled)
                            Me.ELClIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                        End If

                        Me.ELLithiumLabel.Enabled = .LI_Enabled
                        If .LI_Enabled Then
                            If .LI_Data.Installed Then
                                Me.ELLiInstallTextEdit.Text = .LI_Data.InstallationDate.ToString(MyClass.myCultureInfo.DateTimeFormat.ShortDatePattern)
                                Me.ELLiTestTextEdit.Text = .LI_Data.TestCount.ToString
                                If .LI_Data.IsOverUsed Or .LI_Data.IsExpired Then
                                    Me.ELLiIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                                    Me.ELLiWarningLabel.Text = MyClass.GetWarningLabel(WarningElements.EL_ReplaceRecommended)
                                End If
                            Else
                                Me.ELLiWarningLabel.Text = MyClass.GetWarningLabel(WarningElements.EL_NotInstalled)
                                Me.ELLiIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                            End If
                        End If

                        ' 
                        ' Last Calibrations and Cleanings
                        ' 
                        Dim CheckRecommended As Boolean = False
                        If .CAL_ElectrodesCalibDate <> Nothing Then
                            Me.CALElectrodesDateTextEdit.Text = .CAL_ElectrodesCalibDate.ToString(MyClass.myCultureInfo.DateTimeFormat.ShortDatePattern)
                            Me.CALElectrodesResult1TextEdit.Text = .CAL_ElectrodesCalibResult1String
                            Me.CALElectrodesResult2TextEdit.Text = .CAL_ElectrodesCalibResult2String
                            If .CAL_ElectrodesCalibResult1OK And .CAL_ElectrodesCalibResult2OK Then
                                Me.CALElectrodesIcon1.BackgroundImage = MyClass.GetIconImage(IconImages.Ok)
                                CheckRecommended = False
                            Else
                                Me.CALElectrodesWarningLabel.Text = MyClass.GetWarningLabel(WarningElements.CAL_Error)
                                Me.CALElectrodesIcon1.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                                CheckRecommended = True
                            End If
                        Else
                            'Me.CALElectrodesIcon1.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                            CheckRecommended = True
                        End If
                        If .CAL_ElectrodesRecommended And CheckRecommended Then
                            Me.CALElectrodesWarningLabel.Text = MyClass.GetWarningLabel(WarningElements.CAL_Recommended)
                            Me.CALElectrodesIcon1.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                        End If

                        CheckRecommended = False
                        If .CAL_PumpsCalibDate <> Nothing Then
                            Me.CALPumpsDateTextEdit.Text = .CAL_PumpsCalibDate.ToString(MyClass.myCultureInfo.DateTimeFormat.ShortDatePattern)
                            Me.CALPumpsResultTextEdit.Text = .CAL_PumpsCalibResultString
                            If .CAL_PumpsCalibResultOK Then
                                Me.CALPumpsIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Ok)
                                CheckRecommended = False
                            Else
                                Me.CALPumpsWarningLabel.Text = MyClass.GetWarningLabel(WarningElements.CAL_Error)
                                Me.CALPumpsIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                                CheckRecommended = True
                            End If
                        Else
                            'Me.CALPumpsIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                            CheckRecommended = True
                        End If
                        If .CAL_PumpsRecommended And CheckRecommended Then
                            Me.CALPumpsWarningLabel.Text = MyClass.GetWarningLabel(WarningElements.CAL_Recommended)
                            Me.CALPumpsIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                        End If

                        If .CleanDate <> Nothing Then
                            Me.CALCleanDateTextEdit.Text = .CleanDate.ToString(MyClass.myCultureInfo.DateTimeFormat.ShortDatePattern)
                        End If

                        If .CleanRecommended Then
                            Me.CALCleanWarningLabel.Text = MyClass.GetWarningLabel(WarningElements.CAL_CleanRecommended)
                            Me.CALCleanIcon.BackgroundImage = MyClass.GetIconImage(IconImages.Warning)
                        End If


                        'TODO
                        If .TUB_PumpInstallDate <> Nothing Then

                        Else

                        End If

                        If .TUB_FluidInstallDate <> Nothing Then

                        Else

                        End If

                        ' XBC 22/03/2012
                        If Not .IsInitiatedOK Then
                            MyClass.ReagentsPackGroupBox.Enabled = False
                        Else
                            MyClass.ReagentsPackGroupBox.Enabled = True
                        End If
                        ' XBC 22/03/2012
                    End With

                End If

                Application.DoEvents()
                Me.Refresh()

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

#End Region

#Region "Private Methods"

#Region "Initializations"

        Private Sub InitializeFields()
            Try

                Me.RPInstallDateTextEdit.Text = ""
                Me.RPExpireDateTextEdit.Text = ""
                Me.RPRemainingCalATextEdit.Text = ""
                Me.RPRemainingCalBTextEdit.Text = ""
                Me.RPInitialCalATextEdit.Text = ""
                Me.RPInitialCalBTextEdit.Text = ""

                Me.ELRefInstallTextEdit.Text = ""
                Me.ELNaInstallTextEdit.Text = ""
                Me.ELKInstallTextEdit.Text = ""
                Me.ELClInstallTextEdit.Text = ""
                Me.ELLiInstallTextEdit.Text = ""
                Me.ELRefTestTextEdit.Text = ""
                Me.ELNaTestTextEdit.Text = ""
                Me.ELKTestTextEdit.Text = ""
                Me.ELClTestTextEdit.Text = ""
                Me.ELLiTestTextEdit.Text = ""

                Me.CALElectrodesDateTextEdit.Text = ""
                Me.CALPumpsDateTextEdit.Text = ""
                Me.CALCleanDateTextEdit.Text = ""
                Me.CALElectrodesResult1TextEdit.Text = ""
                Me.CALElectrodesResult2TextEdit.Text = ""
                Me.CALPumpsResultTextEdit.Text = ""

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub InitializeIcons()
            Try
                For Each C As Control In Me.ReagentsPackGroupBox.Controls
                    If TypeOf C Is PictureBox Then
                        Dim P As PictureBox = CType(C, PictureBox)
                        If P IsNot Nothing Then
                            P.BorderStyle = Windows.Forms.BorderStyle.None
                            P.BackgroundImage = Nothing
                            P.BackgroundImageLayout = ImageLayout.Center
                        End If

                    End If
                Next
                For Each C As Control In Me.ElectrodesGroupBox.Controls
                    If TypeOf C Is PictureBox Then
                        Dim P As PictureBox = CType(C, PictureBox)
                        If P IsNot Nothing Then
                            P.BorderStyle = Windows.Forms.BorderStyle.None
                            P.BackgroundImage = Nothing
                            P.BackgroundImageLayout = ImageLayout.Center
                        End If
                    End If
                Next
                For Each C As Control In Me.CalibrationsGroupBox.Controls
                    If TypeOf C Is PictureBox Then
                        Dim P As PictureBox = CType(C, PictureBox)
                        If P IsNot Nothing Then
                            P.BorderStyle = Windows.Forms.BorderStyle.None
                            P.BackgroundImage = Nothing
                            P.BackgroundImageLayout = ImageLayout.Center
                        End If
                    End If
                Next


            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub InitializeWarnings()
            Try

                Me.RPNotInstalledWarningLabel.Text = ""
                Me.RPExpiredWarningLabel.Text = ""
                Me.RPCalAWarningLabel.Text = ""
                Me.RPCalBWarningLabel.Text = ""

                Me.ELRefWarningLabel.Text = ""
                Me.ELNaWarningLabel.Text = ""
                Me.ELKWarningLabel.Text = ""
                Me.ELClWarningLabel.Text = ""
                Me.ELLiWarningLabel.Text = ""

                Me.CALElectrodesWarningLabel.Text = ""
                Me.CALPumpsWarningLabel.Text = ""
                Me.CALCleanWarningLabel.Text = ""

            Catch ex As Exception
                Throw ex
            End Try
        End Sub


#End Region

#Region "Load Texts"

        ''' <summary>
        ''' Get texts in the current application language for all screen controls and tooltips
        ''' </summary>
        ''' <remarks>Created by: SGM 07/02/2012</remarks>
        Private Sub LoadScreenLabels(ByVal pLabelsData As Dictionary(Of LabelElements, String))
            Try
                If pLabelsData IsNot Nothing Then

                    If pLabelsData.ContainsKey(LabelElements.RP_Title) Then
                        Me.RPTitleLabel.Text = pLabelsData(LabelElements.RP_Title)
                    End If
                    If pLabelsData.ContainsKey(LabelElements.RP_InstallDate) Then
                        Me.RPInstallDateLabel.Text = pLabelsData(LabelElements.RP_InstallDate) & ":"
                    End If
                    If pLabelsData.ContainsKey(LabelElements.RP_ExpireDate) Then
                        Me.RPExpireDateLabel.Text = pLabelsData(LabelElements.RP_ExpireDate) & ":"
                    End If
                    If pLabelsData.ContainsKey(LabelElements.RP_RemainingVolume) Then
                        Me.RPRemainingLabel.Text = pLabelsData(LabelElements.RP_RemainingVolume)
                    End If
                    If pLabelsData.ContainsKey(LabelElements.RP_InitialVolume) Then
                        Me.RPInitialVolumeLabel.Text = pLabelsData(LabelElements.RP_InitialVolume)
                    End If

                    'TR 26/03/2012 -GB for volume.
                    If pLabelsData.ContainsKey(LabelElements.RP_Volume) Then
                        Me.VolumeGB.Text = pLabelsData(LabelElements.RP_Volume)
                    End If
                    'TR 26/03/2012 -END

                    If pLabelsData.ContainsKey(LabelElements.RP_Calibrator) Then
                        Me.RPCalibratorALabel.Text = pLabelsData(LabelElements.RP_Calibrator) & " A:"
                        Me.RPCalibratorBLabel.Text = pLabelsData(LabelElements.RP_Calibrator) & " B:"
                    End If


                    If pLabelsData.ContainsKey(LabelElements.EL_Title) Then
                        Me.ELTitleLabel.Text = pLabelsData(LabelElements.EL_Title)
                    End If
                    If pLabelsData.ContainsKey(LabelElements.EL_InstallDate) Then
                        Me.ELInstallDateLabel.Text = pLabelsData(LabelElements.EL_InstallDate)
                    End If
                    If pLabelsData.ContainsKey(LabelElements.EL_TestsCompleted) Then
                        Me.ELTestCompletedLabel.Text = pLabelsData(LabelElements.EL_TestsCompleted)
                    End If
                    'If pLabelsData.ContainsKey(LabelElements.EL_Reference) Then
                    Me.ELReferenceLabel.Text = "Ref:"
                    'End If
                    'If pLabelsData.ContainsKey(LabelElements.EL_Sodium) Then
                    Me.ELSodiumLabel.Text = "Na+:"
                    'End If
                    'If pLabelsData.ContainsKey(LabelElements.EL_Potassium) Then
                    Me.ELPotassiumLabel.Text = "K+:"
                    'End If
                    'If pLabelsData.ContainsKey(LabelElements.EL_Chlorine) Then
                    Me.ELChlorineLabel.Text = "Cl-:"
                    'End If
                    'If pLabelsData.ContainsKey(LabelElements.EL_Lithium) Then
                    Me.ELLithiumLabel.Text = "Li+:"
                    'End If


                    If pLabelsData.ContainsKey(LabelElements.CAL_Title) Then
                        Me.CALTitleLabel.Text = pLabelsData(LabelElements.CAL_Title)
                    End If
                    If pLabelsData.ContainsKey(LabelElements.CAL_LastDate) Then
                        Me.CALLastDateLabel.Text = pLabelsData(LabelElements.CAL_LastDate)
                    End If
                    If pLabelsData.ContainsKey(LabelElements.CAL_Results) Then
                        Me.CALResultLabel.Text = pLabelsData(LabelElements.CAL_Results)
                    End If
                    If pLabelsData.ContainsKey(LabelElements.CAL_Electrodes) Then
                        Me.CALElectrodesLabel.Text = pLabelsData(LabelElements.CAL_Electrodes) & ":"
                    End If
                    If pLabelsData.ContainsKey(LabelElements.CAL_Pumps) Then
                        Me.CALPumpsLabel.Text = pLabelsData(LabelElements.CAL_Pumps) & ":"
                    End If
                    If pLabelsData.ContainsKey(LabelElements.CAL_Clean) Then
                        Me.CALCleanLabel.Text = pLabelsData(LabelElements.CAL_Clean) & ":"
                    End If

                End If

            Catch ex As Exception
                Throw ex
            End Try
        End Sub


#End Region

#Region "Common"

        Private Function GetPercent(ByVal pMaxValue As Single, ByVal pValue As Single) As Integer
            Dim myPercent As Integer = 0
            Try
                myPercent = CInt((100 * pValue) / pMaxValue)
            Catch ex As Exception
                Throw ex
            End Try
            Return myPercent
        End Function

        ''' <summary>
        ''' Get texts in the current application language for warnings
        ''' </summary>
        ''' <remarks>Created by: SGM 07/02/2012</remarks>
        Private Function GetWarningLabel(ByVal pWarningItem As WarningElements) As String
            Dim myWarning As String = ""
            Try
                If pWarningItem <> WarningElements.None Then
                    If MyClass.WarningsDataAttr.ContainsKey(pWarningItem) Then
                        myWarning = CStr(MyClass.WarningsDataAttr(pWarningItem))
                    End If
                End If

            Catch ex As Exception
                Throw ex
            End Try
            Return myWarning
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pIcon"></param>
        ''' <returns></returns>
        ''' <remarks>Created by: SGM 07/02/2012</remarks>
        Private Function GetIconImage(ByVal pIcon As IconImages) As Image
            Dim myImage As Image
            Try
                myImage = MyClass.ImagesData.Images(pIcon.ToString)
            Catch ex As Exception
                Throw ex
            End Try
            Return myImage
        End Function


#End Region

#End Region

#Region "Event Handlers"

        Private Sub BSISEMonitorPanel_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Try
                MyClass.InitializeFields()
                MyClass.InitializeWarnings()
                MyClass.InitializeIcons()

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' For hiding image border when no image
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>Created by SGM 13/03/2012</remarks>
        Private Sub Icons_BackgroundImageChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
                                                RPInstalledIcon.BackgroundImageChanged, _
                                                RPExpiredIcon.BackgroundImageChanged, _
 _
 _
                                                ELRefIcon.BackgroundImageChanged, _
                                                ELNaIcon.BackgroundImageChanged, _
                                                ELLiIcon.BackgroundImageChanged, _
                                                ELKIcon.BackgroundImageChanged, _
                                                ELClIcon.BackgroundImageChanged, _
                                                CALPumpsIcon.BackgroundImageChanged, _
                                                CALElectrodesIcon1.BackgroundImageChanged, _
                                                CALCleanIcon.BackgroundImageChanged, _
                                                RPInstalledIcon.BackgroundImageChanged
            Try

                Dim myPictureBox As PictureBox = CType(sender, PictureBox)
                If myPictureBox IsNot Nothing Then
                    myPictureBox.Visible = (myPictureBox.BackgroundImage IsNot Nothing)
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub
#End Region




    End Class
End Namespace