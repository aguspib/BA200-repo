Imports System.Windows
Imports System.Windows.Controls

Partial Public Class XPSViewer

#Region "Declarations"
    Public FitToHeightButtonCaption As String = "Fit To Height"
    Public FitToHeightButtonVisibility As System.Windows.Visibility = Windows.Visibility.Visible
#End Region
   

#Region "public Methods"
    Public Sub MenuBarVisible(ByVal pVisible As Boolean)
        Try
            Dim myGrid As Controls.Grid = CType(MyClass.DocumentViewer1.Template.FindName("DocumentViewerGrid", DocumentViewer1), Controls.Grid)
            If myGrid IsNot Nothing Then
                If pVisible Then
                    myGrid.RowDefinitions(0).Height = GridLength.Auto
                Else
                    myGrid.RowDefinitions(0).Height = New GridLength(0)
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Sub SearchBarVisible(ByVal pVisible As Boolean)
        Try

            Dim myGrid As Controls.Grid = CType(MyClass.DocumentViewer1.Template.FindName("DocumentViewerGrid", DocumentViewer1), Controls.Grid)
            If myGrid IsNot Nothing Then
                If pVisible Then
                    myGrid.RowDefinitions(2).Height = GridLength.Auto
                Else
                    myGrid.RowDefinitions(2).Height = New GridLength(0)
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub


    Public Sub SetButtonVisible(ByVal pButtonId As String, ByVal pVisible As Boolean)
        Try
            Dim myToolBar As Controls.ToolBar = GetToolBar()
            If myToolBar IsNot Nothing Then
                Dim myButton As Controls.Button = CType(myToolBar.FindName(pButtonId), Controls.Button)
                If myButton IsNot Nothing Then
                    If pVisible Then
                        myButton.Visibility = Windows.Visibility.Visible
                    Else
                        myButton.Visibility = Windows.Visibility.Hidden
                    End If
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Function GetButtonVisibility(ByVal pButtonId As String) As System.Windows.Visibility
        Try
            Dim myToolBar As Controls.ToolBar = GetToolBar()
            If myToolBar IsNot Nothing Then
                Dim myButton As Controls.Button = CType(myToolBar.FindName(pButtonId), Controls.Button)
                If myButton IsNot Nothing Then
                    Return myButton.Visibility
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Sub SetButtonCaption(ByVal pButtonId As String, ByVal pCaption As String)
        Try
            Dim myToolBar As Controls.ToolBar = GetToolBar()
            If myToolBar IsNot Nothing Then
                Dim myButton As Controls.Button = CType(myToolBar.FindName(pButtonId), Controls.Button)
                If myButton IsNot Nothing Then
                    myButton.Content = pCaption
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Function GetButtonCaption(ByVal pButtonId As String) As String
        Dim myCaption As String = ""
        Try
            Dim myToolBar As Controls.ToolBar = GetToolBar()
            If myToolBar IsNot Nothing Then
                Dim myButton As Controls.Button = CType(myToolBar.FindName(pButtonId), Controls.Button)
                If myButton IsNot Nothing Then
                    myCaption = myButton.Content.ToString
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
        Return myCaption
    End Function

    Public Sub EnablePopupMenu(ByVal pEnable As Boolean)
        Try
            If pEnable Then
                MyClass.UpdatePopupMenu()
            Else
                MyClass.DocumentViewer1.ContextMenu.Items.Clear()
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Sub FitToPageWidth()
        Try
            MyClass.DocumentViewer1.FitToWidth()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Sub FitToPageHeight()
        Try
            MyClass.DocumentViewer1.FitToHeight()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Sub FitToWholePage()
        Try
            MyClass.DocumentViewer1.FitToMaxPagesAcross()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub


    Public Sub SetHorizontalPageMargin(ByVal pMargin As Integer)
        Try
            If pMargin >= 0 Then
                MyClass.DocumentViewer1.HorizontalPageSpacing = pMargin
            Else
                MyClass.DocumentViewer1.HorizontalPageSpacing = 0
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Sub SetVerticalPageMargin(ByVal pMargin As Integer)
        Try
            If pMargin >= 0 Then
                MyClass.DocumentViewer1.VerticalPageSpacing = pMargin
            Else
                MyClass.DocumentViewer1.VerticalPageSpacing = 0
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Function GetHorizontalPageMargin() As Integer
        Try
            Return MyClass.DocumentViewer1.HorizontalPageSpacing
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetVerticalPageMargin() As Integer
        Try
            Return MyClass.DocumentViewer1.VerticalPageSpacing
        Catch ex As Exception
            Throw ex
        End Try
    End Function


#End Region


#Region "private Methods"

    Private Function GetToolBar() As Controls.ToolBar
        Dim myToolBar As Controls.ToolBar
        Try
            Dim myGrid As Controls.Grid = CType(MyClass.DocumentViewer1.Template.FindName("DocumentViewerGrid", DocumentViewer1), Controls.Grid)
            If myGrid IsNot Nothing Then
                myToolBar = CType(myGrid.FindName("MenuBar"), Controls.ToolBar)
            End If
        Catch ex As Exception
            Throw ex
        End Try
        Return myToolBar
    End Function

    Private Sub UpdatePopupMenu()
        Try

            MyClass.DocumentViewer1.ContextMenu.Items.Clear()

            If MyClass.DocumentViewer1.Document IsNot Nothing Then
                If GetButtonVisibility("PrintButton") = Windows.Visibility.Visible Then
                    Dim myPrintMenuItem As New Controls.MenuItem
                    myPrintMenuItem.Header = GetButtonCaption("PrintButton")
                    myPrintMenuItem.IsEnabled = True
                    AddHandler myPrintMenuItem.Click, AddressOf OnPopupPrintMenuItem_Click
                    MyClass.DocumentViewer1.ContextMenu.Items.Add(myPrintMenuItem)
                End If
                If GetButtonVisibility("IncreaseZoomButton") = Windows.Visibility.Visible Then
                    Dim myZoomInMenuItem As New Controls.MenuItem
                    myZoomInMenuItem.Header = GetButtonCaption("IncreaseZoomButton")
                    myZoomInMenuItem.IsEnabled = MyClass.DocumentViewer1.CanIncreaseZoom
                    AddHandler myZoomInMenuItem.Click, AddressOf OnPopupZoomInMenuItem_Click
                    MyClass.DocumentViewer1.ContextMenu.Items.Add(myZoomInMenuItem)
                End If
                If GetButtonVisibility("DecreaseZoomButton") = Windows.Visibility.Visible Then
                    Dim myZoomOutMenuItem As New Controls.MenuItem
                    myZoomOutMenuItem.Header = GetButtonCaption("DecreaseZoomButton")
                    myZoomOutMenuItem.IsEnabled = MyClass.DocumentViewer1.CanDecreaseZoom
                    AddHandler myZoomOutMenuItem.Click, AddressOf OnPopupZoomOutMenuItem_Click
                    MyClass.DocumentViewer1.ContextMenu.Items.Add(myZoomOutMenuItem)
                End If
                If GetButtonVisibility("FitToWidthButton") = Windows.Visibility.Visible Then
                    Dim myFitToWidthMenuItem As New Controls.MenuItem
                    myFitToWidthMenuItem.Header = GetButtonCaption("FitToWidthButton")
                    myFitToWidthMenuItem.IsEnabled = True
                    AddHandler myFitToWidthMenuItem.Click, AddressOf OnPopupFitToWidthMenuItem_Click
                    MyClass.DocumentViewer1.ContextMenu.Items.Add(myFitToWidthMenuItem)
                End If
                If MyClass.FitToHeightButtonVisibility = Windows.Visibility.Visible Then
                    Dim myFitToHeightMenuItem As New Controls.MenuItem
                    myFitToHeightMenuItem.Header = MyClass.FitToHeightButtonCaption
                    myFitToHeightMenuItem.IsEnabled = True
                    AddHandler myFitToHeightMenuItem.Click, AddressOf OnPopupFitToHeightMenuItem_Click
                    MyClass.DocumentViewer1.ContextMenu.Items.Add(myFitToHeightMenuItem)
                End If
                If GetButtonVisibility("WholePageButton") = Windows.Visibility.Visible Then
                    Dim myWholePageButtonMenuItem As New Controls.MenuItem
                    myWholePageButtonMenuItem.Header = GetButtonCaption("WholePageButton")
                    myWholePageButtonMenuItem.IsEnabled = True
                    AddHandler myWholePageButtonMenuItem.Click, AddressOf OnPopupWholePageMenuItem_Click
                    MyClass.DocumentViewer1.ContextMenu.Items.Add(myWholePageButtonMenuItem)
                End If
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub


#End Region

#Region "Event Handlers"

    

    Private Sub DocumentViewer1_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles DocumentViewer1.Loaded
        Try
            MyClass.UpdatePopupMenu()

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

#End Region

#Region "popup Menu Events"

    Private Sub OnPopupPrintMenuItem_Click()
        Try
            MyClass.DocumentViewer1.Print()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub OnPopupZoomInMenuItem_Click()
        Try
            MyClass.DocumentViewer1.IncreaseZoom()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub OnPopupZoomOutMenuItem_Click()
        Try
            MyClass.DocumentViewer1.DecreaseZoom()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub OnPopupFitToWidthMenuItem_Click()
        Try
            MyClass.DocumentViewer1.FitToWidth()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub OnPopupFitToHeightMenuItem_Click()
        Try
            MyClass.DocumentViewer1.FitToHeight()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub OnPopupWholePageMenuItem_Click()
        Try
            MyClass.DocumentViewer1.FitToMaxPagesAcross()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub DocumentViewer1_PreviewMouseRightButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles DocumentViewer1.PreviewMouseRightButtonUp
        Try
            e.Handled = (MyClass.DocumentViewer1.ContextMenu.Items.Count = 0)
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    'REMOVE IN CASE OF ENABLING POPUP MENUS
    Private Sub XPSViewer_MouseRightButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Me.MouseRightButtonUp
        Try
            MyClass.DocumentViewer1.ContextMenu.Items.Clear()
            e.Handled = True
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

#End Region

    

   
End Class
