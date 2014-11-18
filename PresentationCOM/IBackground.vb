Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Types
Imports System.Windows.Forms

Public Class IBackground

    Private WithEvents myMDI As BSBaseForm
    Public Event ExceptionHappened(ByVal ex As Exception)

    ''' <summary>
    ''' overriden property in order to avoid focus when displaying
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 02/01/2012</remarks>
    Protected Overrides ReadOnly Property ShowWithoutActivation() As Boolean
        Get
            Return True
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="myMDI"></param>
    ''' <remarks>
    ''' Created by SGM 02/01/2012
    ''' Modified by RH 02/03/2012 Bug correction and code simplification.
    ''' </remarks>
    Public Sub New(ByRef myMDI As Form, ByRef MsgParent As Form)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        MyClass.myMDI = CType(myMDI, BSBaseForm)
        MyClass.myMDI.Owner = Me
        MyClass.myMDI.SetMsgParent(MsgParent)
    End Sub

    ' Created by RH 02/03/2012
    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Debug.Assert(False, "myMDI Is Nothing. Call New(ByRef myMDI As Form, ByRef MsgParent As Form).")
    End Sub

#Region "Public Methods"

    ''' <summary>
    ''' Shows the current MDI form
    ''' </summary>
    ''' <remarks>
    ''' Created by SGM 02/01/2012
    ''' Modified by RH 02/03/2012 Bug correction and code simplification.
    ''' </remarks>
    Public Sub ShowMDI(ByVal ShowBackground As Boolean)
        Try
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor 'IT 18/11/2014: BA-2025
            If ShowBackground Then
                myMDI.FormBack = Me
                AddHandler myMDI.Resize, AddressOf myMDI_Resize
            End If

            Me.Visible = ShowBackground 'Just in case the handler to myMDI.Resize has not been called yet
            myMDI.Show()
            myMDI.Focus()
            Me.Cursor = System.Windows.Forms.Cursors.Default 'IT 18/11/2014: BA-2025

        Catch ex As Exception
            RaiseEvent ExceptionHappened(ex)
        End Try
    End Sub

#End Region

#Region "MDI screen events handlers"

    ' Modified by RH 02/03/2012 Bug correction and code simplification.
    Private Sub myMDI_Resize(ByVal sender As Object, ByVal e As System.EventArgs)
        Me.Visible = (myMDI.WindowState = FormWindowState.Normal)
        If Visible Then
            myMDI.Focus()
        End If
    End Sub

#End Region

End Class

