Imports System.Windows.Forms
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.Controls.UserControls
    Public Class BSDataGridView

#Region "Attributes"
        Private TabToEnterAttribute As Boolean = False
        Private EnterToTabAttribute As Boolean = False
#End Region

#Region "Properties"
        ''' <summary>
        ''' Property used to indicate if in the DataGridView the Tab Key has to works as the 
        ''' Enter Key when moving between cells (vertical moving)
        ''' </summary>
        ''' <remarks>
        ''' Created by: TR 10/12/2010
        ''' </remarks>
        Public Property TabToEnter() As Boolean
            Get
                Return TabToEnterAttribute
            End Get
            Set(ByVal value As Boolean)
                TabToEnterAttribute = value
            End Set
        End Property

        ''' <summary>
        ''' Property used to indicate if in the DataGridView the Enter Key has to works as the 
        ''' Tab Key when moving between cells (horizontal moving)
        ''' </summary>
        ''' <remarks>
        ''' Created by: SA 21/12/2010
        ''' </remarks>
        Public Property EnterToTab() As Boolean
            Get
                Return EnterToTabAttribute
            End Get
            Set(ByVal value As Boolean)
                EnterToTabAttribute = value
            End Set
        End Property
#End Region

        Protected Overrides Sub OnPaint(ByVal pe As System.Windows.Forms.PaintEventArgs)
            Try
                MyBase.OnPaint(pe)

                ' XB 10/03/2014 - Add Try Catch section
            Catch ex As Exception
                ' Write into Log

                GlobalBase.CreateLogActivity(ex.Message, "BSDataGridView.OnPaint", EventLogEntryType.Error, False)
                ' Throw ex --> Do not work, no business catch this throw, just MyApplication_UnhandledException !
            End Try

            'Add your custom paint code here
        End Sub

        ''' <summary>
        ''' Overrides the method to make the Tab Key works the same as Enter Key (if property TabToEnter is set
        ''' to True) or the Enter Key works the same as Tab Key (if property EnterToTab is set to True)
        ''' </summary>
        ''' <remarks>
        ''' Created by:  TR 10/12/2010
        ''' Modified by: SA 21/12/2010 - Allow also the opposite case: Enter Key working as Tab Key
        ''' </remarks>
        Protected Overrides Function ProcessDialogKey(ByVal keyData As System.Windows.Forms.Keys) As Boolean

            Try
                If (TabToEnterAttribute) Then
                    If (keyData = Keys.Tab) Then
                        SendKeys.Send(Chr(Keys.Enter))
                        Return True
                    End If
                ElseIf (EnterToTabAttribute) Then
                    If (keyData = Keys.Enter) Then
                        SendKeys.Send(Chr(Keys.Tab))
                        Return True
                    End If
                Else
                    Return MyBase.ProcessDialogKey(keyData)
                End If

                'If keyData = Keys.Tab AndAlso TabToEnter Then 'If TAB 
                '    SendKeys.Send(Chr(Keys.Enter))        'Send ENTER'
                '    Return True
                'Else                                        'Other case
                '    Return MyBase.ProcessDialogKey(keyData) 'Return KeyData
                'End If

                ' XB 10/03/2014 - Add Try Catch section
            Catch ex As Exception
                ' Write into Log
                GlobalBase.CreateLogActivity(ex.Message, "BSDataGridView.ProcessDialogKey", EventLogEntryType.Error, False)
                ' Throw ex --> Do not work, no business catch this throw, just MyApplication_UnhandledException !
            End Try

        End Function
    End Class
End Namespace
