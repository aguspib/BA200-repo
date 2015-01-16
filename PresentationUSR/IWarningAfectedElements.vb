Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types

Public Class IWarningAfectedElements
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Declarations"
    Private myScreenToolTips As New ToolTip()
#End Region

#Region "Attibutes"
    Private AffecteElementsDSAttribute As New DependenciesElementsDS
    Private AdditionalElementsAttribute As Boolean = False 'TR 29/11/2010
    Private TestConcentrationAttribute As Boolean = False 'TR 14/12/2010
    Private ElementsAffectedDetailMessAttribute As String = "LBL_ElementsDeleteMessage"
#End Region

#Region "Properties"

    ''' <summary>
    ''' DataSet containing the list of Affected Elements
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 26/11/2010
    ''' </remarks>
    Public Property AffectedElements() As DependenciesElementsDS
        Get
            Return AffecteElementsDSAttribute
        End Get
        Set(ByVal value As DependenciesElementsDS)
            AffecteElementsDSAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' Indicate to the screen that the Affected Elements to show are Additionals Elements (Blanks, Calibrators)
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 29/11/2010
    ''' </remarks>
    Public Property AdditionalElements() As Boolean
        Get
            Return AdditionalElementsAttribute
        End Get
        Set(ByVal value As Boolean)
            AdditionalElementsAttribute = value
        End Set
    End Property

    ''' <summary></summary>
    ''' <remarks>
    ''' Created by: TR 13/12/2010
    ''' </remarks>
    Public Property TestConcentration() As Boolean
        Get
            Return TestConcentrationAttribute
        End Get
        Set(ByVal value As Boolean)
            TestConcentrationAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' To load the detail message
    ''' </summary>
    ''' <remarks></remarks>
    Public Property ElementsAffectedMessageDetail() As String
        Get
            Return ElementsAffectedDetailMessAttribute
        End Get
        Set(ByVal value As String)
            ElementsAffectedDetailMessAttribute = value
        End Set
    End Property
#End Region

#Region "Methods"

    ''' <summary>
    ''' Initialize the Affected Elements GridView
    ''' </summary>
    ''' <param name="pLanguageID">Current Application Language</param>
    ''' <remarks>
    ''' Created by:  TR 24/11/2010
    ''' Modified by: TR 29/11/2010 - Validate if the Elements affected are Additionals Elements (Blanks or Calibrators)
    ''' </remarks>
    Private Sub InitializeAffectedElementsGridView(ByVal pLanguageID As String)
        Try
            Dim columnName As String
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            AffectedElementsGridView.AllowUserToAddRows = False
            AffectedElementsGridView.AllowUserToDeleteRows = False
            AffectedElementsGridView.Rows.Clear()
            AffectedElementsGridView.Columns.Clear()
            AffectedElementsGridView.AutoGenerateColumns = False

            'Element Type Icon column
            Dim TypeIconColumn As New DataGridViewImageColumn
            columnName = "TypeIcon"
            TypeIconColumn.Name = columnName
            TypeIconColumn.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", pLanguageID)
            AffectedElementsGridView.Columns.Add(TypeIconColumn)
            AffectedElementsGridView.Columns(columnName).Width = 50
            AffectedElementsGridView.Columns(columnName).DataPropertyName = "Type"
            AffectedElementsGridView.Columns(columnName).ReadOnly = True
            AffectedElementsGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic
            AffectedElementsGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            AffectedElementsGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            'Element Name column
            columnName = "TestName"
            AffectedElementsGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", pLanguageID))
            AffectedElementsGridView.Columns(columnName).Width = 180
            AffectedElementsGridView.Columns(columnName).Visible = True
            AffectedElementsGridView.Columns(columnName).DataPropertyName = "Name"
            AffectedElementsGridView.Columns(columnName).ReadOnly = True
            AffectedElementsGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic
            AffectedElementsGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
            AffectedElementsGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

            'Additional Information column
            columnName = "FormProfMember"
            AffectedElementsGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_AdditionalInformation", pLanguageID))
            AffectedElementsGridView.Columns(columnName).Width = 400
            AffectedElementsGridView.Columns(columnName).DataPropertyName = "FormProfileMember"
            AffectedElementsGridView.Columns(columnName).ReadOnly = True
            AffectedElementsGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic
            AffectedElementsGridView.Columns(columnName).DefaultCellStyle.WrapMode = DataGridViewTriState.True
            AffectedElementsGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
            AffectedElementsGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

            'For Additional Elements, change Additional Information column to show ABS Factor, and add column for ResultDateTime
            If (AdditionalElementsAttribute) Then
                'Make smaller the Name column
                AffectedElementsGridView.Columns("TestName").Width = 100

                AffectedElementsGridView.Columns("FormProfMember").HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Factor", pLanguageID)
                AffectedElementsGridView.Columns("FormProfMember").Width = 150 'Change the colunms size.
                AffectedElementsGridView.Columns("FormProfMember").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                AffectedElementsGridView.Columns("FormProfMember").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

                columnName = "ResultDateTime"
                AffectedElementsGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", pLanguageID))
                AffectedElementsGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                AffectedElementsGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                AffectedElementsGridView.Columns(columnName).Width = 150
                AffectedElementsGridView.Columns(columnName).DataPropertyName = "ResultDateTime"
                AffectedElementsGridView.Columns(columnName).ReadOnly = True
                AffectedElementsGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic
                AffectedElementsGridView.Columns(columnName).DefaultCellStyle.WrapMode = DataGridViewTriState.True
            End If
            AffectedElementsGridView.ScrollBars = ScrollBars.Both

            'Remove duplicated Elements and sort data by main columns (Type and Name)
            AffecteElementsDSAttribute = FilterDependenciesElements(AffecteElementsDSAttribute)
            AffecteElementsDSAttribute.DependenciesElements.DefaultView.Sort = "Type, Name"

            AffectedElementsGridView.DataSource = AffecteElementsDSAttribute.DependenciesElements.DefaultView
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeAffectedElementsGridView", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeAffectedElementsGridView", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Remove from the list the duplicated elements
    ''' </summary>
    ''' <param name="pDependenciesElementsDS">Typed DataSet DependenciesElementsDS with the list of affected Elements</param>
    ''' <returns>Typed DataSet DependenciesElementsDS without duplicated Elements</returns>
    ''' <remarks>
    ''' Created by: TR 12/05/2011
    ''' </remarks>
    Private Function FilterDependenciesElements(ByVal pDependenciesElementsDS As DependenciesElementsDS) As DependenciesElementsDS
        Dim myDepentdenciesResults As New DependenciesElementsDS
        Try
            'Filter the affected elemente for not showing duplicate elements.
            Dim ListDistinctElements As New List(Of String)
            'Get the distinct elements
            ListDistinctElements = (From a In pDependenciesElementsDS.DependenciesElements _
                                    Select a.Name).Distinct().ToList()

            Dim qDistinctElements As New List(Of DependenciesElementsDS.DependenciesElementsRow)
            Dim myAdditionalInformation As String = ""
            For Each dependenciesRow As String In ListDistinctElements
                qDistinctElements = (From a In pDependenciesElementsDS.DependenciesElements _
                                     Where a.Name = dependenciesRow Select a).ToList()

                If qDistinctElements.Count > 0 Then
                    myAdditionalInformation = ""
                    If Not qDistinctElements.First.IsFormProfileMemberNull Then
                        For Each elementRow As DependenciesElementsDS.DependenciesElementsRow In qDistinctElements
                            If elementRow.FormProfileMember <> "" AndAlso Not myAdditionalInformation.Contains(elementRow.FormProfileMember) Then
                                myAdditionalInformation &= elementRow.FormProfileMember & Environment.NewLine
                            End If
                        Next
                    End If

                    qDistinctElements.First().FormProfileMember = myAdditionalInformation.TrimEnd()

                    'DL 05/06/2013 if type is null is that the affected element is a calculated test
                    If qDistinctElements.First().IsTypeNull Then
                        Dim imagebytes As Byte()
                        Dim preloadedDataConfig As New PreloadedMasterDataDelegate

                        imagebytes = preloadedDataConfig.GetIconImage("TCALC")
                        qDistinctElements.First().Type = imagebytes
                    End If
                    'DL 05/06/2013 if type is null is that the affected element is a calculated test

                    myDepentdenciesResults.DependenciesElements.ImportRow(qDistinctElements.First())
                End If
            Next

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FilterDependenciesElements", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FilterDependenciesElements", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myDepentdenciesResults
    End Function

    ''' <summary>
    ''' Prepare all the Buttons and PictureBox controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 24/11/2010
    ''' </remarks>
    Private Sub PrepareButtonsAndPicturesControls()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'ACCEPT Button
            auxIconName = GetIconName("ACCEPT1")
            If (auxIconName <> "") Then
                ExitButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'EXIT Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                ButtonCancel.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'Warning PictureBox
            auxIconName = GetIconName("STUS_WITHERRS") 'WARNING") dl 23/03/2012
            If (auxIconName <> "") Then
                bsWarningPictureBox.ImageLocation = MyBase.IconsPath & auxIconName
                bsWarningPictureBox.SizeMode = ImageLayout.Stretch
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtonsAndPicturesControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtonsAndPicturesControls ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initialize all the screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 24/11/2010
    ''' </remarks>
    Private Sub InitializeScreen()
        Try
            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim().ToString()

            PrepareButtonsAndPicturesControls()
            GetScreenLabels(currentLanguage)
            InitializeAffectedElementsGridView(currentLanguage)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeScreen ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get all the screen label message depending on the Language
    ''' </summary>
    ''' <param name="pLanguageID">Current Application Language</param>
    ''' <remarks>
    ''' Created by:  TR 24/11/2010
    ''' Modified by: TR 14/12/2010 - Button Tooltips are different when the screen is used to warning about Test Concentrations
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels
            LBL_AfectedElementsWarning.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, LBL_AfectedElementsWarning.Name, pLanguageID)
            LBL_ElementsDeleteMessage.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, ElementsAffectedDetailMessAttribute, pLanguageID)
            LBL_ElementsDeleteMessage.Font = New Font("Verdana", 9, FontStyle.Regular)
            LBL_ElementsDeleteMessage.ForeColor = Color.Red

            'For Button ToolTips
            If (TestConcentrationAttribute) Then
                myScreenToolTips.SetToolTip(ButtonCancel, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_No", pLanguageID))
                myScreenToolTips.SetToolTip(ExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Yes", pLanguageID))
            Else
                'For Tooltips...
                myScreenToolTips.SetToolTip(ButtonCancel, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", pLanguageID))
                myScreenToolTips.SetToolTip(ExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save&Close", pLanguageID))
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub
#End Region

#Region "Events"
    Private Sub AfectedElementsWarning_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        InitializeScreen()
    End Sub
    Private Sub CancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonCancel.Click
        Close()
    End Sub
#End Region

End Class
