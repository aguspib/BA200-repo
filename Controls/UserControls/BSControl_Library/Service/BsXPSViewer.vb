Imports System.Windows.Forms.Integration
Imports System.Windows.Xps.Packaging
Imports System.IO

Public Class BsXPSViewer


#Region "Enumerates"
    Private Enum MenuButtonsIds
        PrintButton
        CopyButton
        IncreaseZoomButton
        DecreaseZoomButton
        ActualZoomButton
        FitToWidthButton
        FitToHeightButton
        WholePageButton
        TwoPagesButton
    End Enum
#End Region

#Region "attributes"

    Private MenuBarVisibleAttr As Boolean = False
    Private SearchBarVisibleAttr As Boolean = False

    Private PrintButtonVisibleAttr As Boolean = True
    Private CopyButtonVisibleAttr As Boolean = True
    Private IncreaseZoomButtonVisibleAttr As Boolean = True
    Private DecreaseZoomButtonVisibleAttr As Boolean = True
    Private ActualZoomButtonVisibleAttr As Boolean = True
    Private FitToWidthButtonVisibleAttr As Boolean = True
    Private FitToHeightButtonVisibleAttr As Boolean = True
    Private WholePageButtonVisibleAttr As Boolean = True
    Private TwoPagesButtonVisibleAttr As Boolean = True
    Private IsScrollableAttr As Boolean = False

    Private PrintButtonCaptionAttr As String = "Print"
    Private CopyButtonCaptionAttr As String = "Copy"
    Private IncreaseZoomButtonCaptionAttr As String = "Zoom In"
    Private DecreaseZoomButtonCaptionAttr As String = "Zoom Out"
    Private ActualZoomButtonCaptionAttr As String = "Actual Zoom"
    Private FitToWidthButtonCaptionAttr As String = "Fit To Width"
    Private WholePageButtonCaptionAttr As String = "Whole Page"
    Private TwoPagesButtonCaptionAttr As String = "Two Pages"

    Private PopupMenuEnabledAttr As Boolean = True

    Private IsLoadedAttr As Boolean = False

#End Region

#Region "public properties"

    Public Property MenuBarVisible() As Boolean
        Get
            Return MenuBarVisibleAttr
        End Get
        Set(ByVal value As Boolean)
            MenuBarVisibleAttr = value
            Me.XpsViewer1.MenuBarVisible(value)
        End Set
    End Property
    Public Property SearchBarVisible() As Boolean
        Get
            Return SearchBarVisibleAttr
        End Get
        Set(ByVal value As Boolean)
            SearchBarVisibleAttr = value
            Me.XpsViewer1.SearchBarVisible(value)
        End Set
    End Property

    'Buttons Visibility
    '*********************************************
    Public Property PrintButtonVisible() As Boolean
        Get
            Return PrintButtonVisibleAttr
        End Get
        Set(ByVal value As Boolean)
            PrintButtonVisibleAttr = value
            Me.XpsViewer1.SetButtonVisible(MenuButtonsIds.PrintButton.ToString, value)
        End Set
    End Property

    Public Property CopyButtonVisible() As Boolean
        Get
            Return CopyButtonVisibleAttr
        End Get
        Set(ByVal value As Boolean)
            CopyButtonVisibleAttr = value
            Me.XpsViewer1.SetButtonVisible(MenuButtonsIds.CopyButton.ToString, value)
        End Set
    End Property

    Public Property IncreaseZoomButtonVisible() As Boolean
        Get
            Return IncreaseZoomButtonVisibleAttr
        End Get
        Set(ByVal value As Boolean)
            IncreaseZoomButtonVisibleAttr = value
            Me.XpsViewer1.SetButtonVisible(MenuButtonsIds.IncreaseZoomButton.ToString, value)
        End Set
    End Property

    Public Property DecreaseZoomButtonVisible() As Boolean
        Get
            Return DecreaseZoomButtonVisibleAttr
        End Get
        Set(ByVal value As Boolean)
            DecreaseZoomButtonVisibleAttr = value
            Me.XpsViewer1.SetButtonVisible(MenuButtonsIds.DecreaseZoomButton.ToString, value)
        End Set
    End Property

    Public Property ActualZoomButtonVisible() As Boolean
        Get
            Return ActualZoomButtonVisibleAttr
        End Get
        Set(ByVal value As Boolean)
            ActualZoomButtonVisibleAttr = value
            Me.XpsViewer1.SetButtonVisible(MenuButtonsIds.ActualZoomButton.ToString, value)
        End Set
    End Property

    Public Property FitToWidthButtonVisible() As Boolean
        Get
            Return FitToWidthButtonVisibleAttr
        End Get
        Set(ByVal value As Boolean)
            FitToWidthButtonVisibleAttr = value
            Me.XpsViewer1.SetButtonVisible(MenuButtonsIds.FitToWidthButton.ToString, value)
        End Set
    End Property

    Public Property FitToHeightButtonVisible() As Boolean
        Get
            Return (Me.XpsViewer1.FitToHeightButtonVisibility = Windows.Visibility.Visible)
        End Get
        Set(ByVal value As Boolean)
            FitToHeightButtonVisibleAttr = value
            If value Then
                Me.XpsViewer1.FitToHeightButtonVisibility = Windows.Visibility.Visible
            Else
                Me.XpsViewer1.FitToHeightButtonVisibility = Windows.Visibility.Hidden
            End If
        End Set
    End Property

    Public Property WholePageButtonVisible() As Boolean
        Get
            Return WholePageButtonVisibleAttr
        End Get
        Set(ByVal value As Boolean)
            WholePageButtonVisibleAttr = value
            Me.XpsViewer1.SetButtonVisible(MenuButtonsIds.WholePageButton.ToString, value)
        End Set
    End Property

    Public Property TwoPagesButtonVisible() As Boolean
        Get
            Return TwoPagesButtonVisibleAttr
        End Get
        Set(ByVal value As Boolean)
            TwoPagesButtonVisibleAttr = value
            Me.XpsViewer1.SetButtonVisible(MenuButtonsIds.TwoPagesButton.ToString, value)
        End Set
    End Property

    'Buttons Caption
    '*********************************************************
    Public Property PrintButtonCaption() As String
        Get
            Return PrintButtonCaptionAttr
        End Get
        Set(ByVal value As String)
            PrintButtonCaptionAttr = value
            Me.XpsViewer1.SetButtonCaption(MenuButtonsIds.PrintButton.ToString, value)
        End Set
    End Property

    Public Property CopyButtonCaption() As String
        Get
            Return CopyButtonCaptionAttr
        End Get
        Set(ByVal value As String)
            CopyButtonCaptionAttr = value
            Me.XpsViewer1.SetButtonCaption(MenuButtonsIds.CopyButton.ToString, value)
        End Set
    End Property

    Public Property IncreaseZoomButtonCaption() As String
        Get
            Return IncreaseZoomButtonCaptionAttr
        End Get
        Set(ByVal value As String)
            IncreaseZoomButtonCaptionAttr = value
            Me.XpsViewer1.SetButtonCaption(MenuButtonsIds.IncreaseZoomButton.ToString, value)
        End Set
    End Property

    Public Property DecreaseZoomButtonCaption() As String
        Get
            Return DecreaseZoomButtonCaptionAttr
        End Get
        Set(ByVal value As String)
            DecreaseZoomButtonCaptionAttr = value
            Me.XpsViewer1.SetButtonCaption(MenuButtonsIds.DecreaseZoomButton.ToString, value)
        End Set
    End Property

    Public Property ActualZoomButtonCaption() As String
        Get
            Return ActualZoomButtonCaptionAttr
        End Get
        Set(ByVal value As String)
            ActualZoomButtonCaptionAttr = value
            Me.XpsViewer1.SetButtonCaption(MenuButtonsIds.ActualZoomButton.ToString, value)
        End Set
    End Property

    Public Property FitToWidthButtonCaption() As String
        Get
            Return FitToWidthButtonCaptionAttr
        End Get
        Set(ByVal value As String)
            FitToWidthButtonCaptionAttr = value
            Me.XpsViewer1.SetButtonCaption(MenuButtonsIds.FitToWidthButton.ToString, value)
        End Set
    End Property

    Public Property FitToHeightButtonCaption() As String
        Get
            Return Me.XpsViewer1.FitToHeightButtonCaption
        End Get
        Set(ByVal value As String)
            Me.XpsViewer1.FitToHeightButtonCaption = value
        End Set
    End Property


    Public Property WholePageButtonCaption() As String
        Get
            Return WholePageButtonCaptionAttr
        End Get
        Set(ByVal value As String)
            WholePageButtonCaptionAttr = value
            Me.XpsViewer1.SetButtonCaption(MenuButtonsIds.WholePageButton.ToString, value)
        End Set
    End Property

    Public Property TwoPagesButtonCaption() As String
        Get
            Return TwoPagesButtonCaptionAttr
        End Get
        Set(ByVal value As String)
            TwoPagesButtonCaptionAttr = value
            Me.XpsViewer1.SetButtonCaption(MenuButtonsIds.TwoPagesButton.ToString, value)
        End Set
    End Property

    Public Property PopupMenuEnabled() As Boolean
        Get
            Return PopupMenuEnabledAttr
        End Get
        Set(ByVal value As Boolean)
            PopupMenuEnabledAttr = value
            Me.XpsViewer1.EnablePopupMenu(value)
        End Set
    End Property

    Public Property HorizontalPageMargin() As Integer
        Get
            Return Me.XpsViewer1.GetHorizontalPageMargin
        End Get
        Set(ByVal value As Integer)
            Me.XpsViewer1.SetHorizontalPageMargin(value)
        End Set
    End Property

    Public Property VerticalPageMargin() As Integer
        Get
            Return Me.XpsViewer1.GetVerticalPageMargin
        End Get
        Set(ByVal value As Integer)
            Me.XpsViewer1.SetVerticalPageMargin(value)
        End Set
    End Property

    Public Property IsScrollable() As Boolean
        Get
            Return IsScrollableAttr
        End Get
        Set(ByVal value As Boolean)
            IsScrollableAttr = value
        End Set
    End Property
    Public ReadOnly Property HasNextPage()
        Get
            Return Me.XpsViewer1.DocumentViewer1.CanGoToNextPage
        End Get
    End Property

    Public ReadOnly Property HasPrevPage()
        Get
            Return Me.XpsViewer1.DocumentViewer1.CanGoToPreviousPage
        End Get
    End Property

    Public ReadOnly Property CanZoomIn()
        Get
            Return Me.XpsViewer1.DocumentViewer1.CanIncreaseZoom
        End Get
    End Property

    Public ReadOnly Property CanZoomOut()
        Get
            Return Me.XpsViewer1.DocumentViewer1.CanDecreaseZoom
        End Get
    End Property

    Public ReadOnly Property IsDocumentOpened() As Boolean
        Get
            Return (Me.XpsViewer1.DocumentViewer1.Document IsNot Nothing)
        End Get
    End Property

    Public Property IsLoaded() As Boolean
        Get

        End Get
        Set(ByVal value As Boolean)

        End Set
    End Property

#End Region

#Region "public methods"

   
    Public Sub Open(ByVal pPath As String)
        Try
            If File.Exists(pPath) Then
                Dim myXPS As XpsDocument = New XpsDocument(pPath, System.IO.FileAccess.Read, Packaging.CompressionOption.SuperFast)
                Me.XpsViewer1.DocumentViewer1.Document = myXPS.GetFixedDocumentSequence()

            Else
                Throw New FileNotFoundException
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Sub Close()
        Try
            Me.XpsViewer1.DocumentViewer1.Document = Nothing
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Sub Print()
        Try
            Me.XpsViewer1.DocumentViewer1.Print()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Sub ZoomIn()
        Try
            Me.XpsViewer1.DocumentViewer1.IncreaseZoom()
        Catch ex As Exception
            Throw ex
        End Try

    End Sub
    Public Sub ZoomOut()
        Try
            Me.XpsViewer1.DocumentViewer1.DecreaseZoom()
        Catch ex As Exception
            Throw ex
        End Try

    End Sub
    Public Sub LastPage()
        Try
            Me.XpsViewer1.DocumentViewer1.LastPage()
        Catch ex As Exception
            Throw ex
        End Try

    End Sub
    Public Sub FirstPage()
        Try
            Me.XpsViewer1.DocumentViewer1.FirstPage()
        Catch ex As Exception
            Throw ex
        End Try

    End Sub
    Public Sub NextPage()
        Try
            Me.XpsViewer1.DocumentViewer1.NextPage()
        Catch ex As Exception
            Throw ex
        End Try

    End Sub
    Public Sub PrevPage()
        Try
            Me.XpsViewer1.DocumentViewer1.PreviousPage()
        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Public Sub FitToPageWidth()
        Try
            Me.XpsViewer1.FitToPageWidth()
        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Public Sub FitToPageHeight()
        Try
            Me.XpsViewer1.FitToPageHeight()
        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Public Sub FitToWholePage()
        Try
            Me.XpsViewer1.FitToWholePage()
        Catch ex As Exception
            Throw ex
        End Try

    End Sub


    ''' <summary>
    ''' To be Called in Form_Shown and Tab_Selected
    ''' </summary>
    ''' <remarks>Created by SGM 08/05/2012</remarks>
    Public Sub RefreshPage()
        Try
            If Not MyClass.IsScrollableAttr Then
                MyClass.FitToPageHeight()
                MyClass.FitToPageWidth()
            Else
                MyClass.FitToPageWidth()
            End If

            'without margins
            MyClass.HorizontalPageMargin = 0
            MyClass.VerticalPageMargin = 0

            Me.XpsViewer1.MenuBarVisible(MyClass.MenuBarVisibleAttr)
            Me.XpsViewer1.SearchBarVisible(MyClass.SearchBarVisibleAttr)

            Me.Refresh()

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

#End Region

#Region "private Event Handlers"

    Private Sub BsXPSViewer_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            
            RefreshPage()

            MyClass.IsLoadedAttr = True

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

#End Region

   
   
End Class
