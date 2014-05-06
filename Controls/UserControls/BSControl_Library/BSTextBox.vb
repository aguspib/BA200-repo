Imports System.Text.RegularExpressions

Namespace Biosystems.Ax00.Controls.UserControls

    Public Class BSTextBox

#Region "Attributes"
        'Attribute to indicate that is Required 
        Private MandatoryAttribute As Boolean
        Private IsNumbericAttribute As Boolean
        Private DecimalsValuesAttribute As Boolean
#End Region

#Region "Property"

        'Property to indicate Required 
        Public Property Mandatory() As Boolean
            Get
                Return MandatoryAttribute
            End Get
            Set(ByVal value As Boolean)
                MandatoryAttribute = value
                'Call the methos that Set the Back Color for Required Text 
                RequiredTextColor()
            End Set
        End Property

        Public Property IsNumeric() As Boolean
            Get
                Return IsNumbericAttribute
            End Get
            Set(ByVal value As Boolean)
                IsNumbericAttribute = value
                If IsNumbericAttribute Then
                    ShortcutsEnabled = False
                End If
            End Set
        End Property

        Public Property DecimalsValues() As Boolean
            Get
                Return DecimalsValuesAttribute
            End Get
            Set(ByVal value As Boolean)
                DecimalsValuesAttribute = value
            End Set
        End Property


#End Region

#Region "Constructor"
        Public Sub New()
            InitializeComponent()
            FontControl()
        End Sub
#End Region

#Region "Private Methods"
        ''' <summary>
        ''' Method that format the textbox if the textbox is mark as required, change color
        ''' or any other property
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub RequiredTextColor()
            Try
                If MandatoryAttribute Then
                    Me.BackColor = Drawing.Color.Khaki
                Else
                    Me.BackColor = Drawing.Color.White
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' Method that validate and change the TextBox Back Color
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ValidateTextControl()
            Try
                If MandatoryAttribute Then
                    'see if the textbox is not empty.
                    If Me.TextLength > 0 Then
                        Me.BackColor = Drawing.Color.White
                    Else
                        Me.BackColor = Drawing.Color.Khaki
                    End If
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub FontControl()
            Me.Font = New Drawing.Font("Verdana", 8.25)
        End Sub

        Private Function ValidateNumericValue(ByVal pKeyascii As Char) As Boolean
            Dim result As Boolean = False
            Try
                Dim isNumber As System.Text.RegularExpressions.Regex
                If DecimalsValuesAttribute Then
                    '^(\d|-)?(\d|,)*\.?\d*$
                    '[-+]?[0-9]*\.?[0-9]*.
                    isNumber = New System.Text.RegularExpressions.Regex("^[-+]?[0-9]\d*\.?[0]*$")
                Else
                    isNumber = New System.Text.RegularExpressions.Regex("[-+]?[0-9]")

                End If
                result = isNumber.Match(pKeyascii).Success

                If pKeyascii = "." Or pKeyascii = "," Or pKeyascii = ChrW(Windows.Forms.Keys.Back) Then
                    If (Me.Text.Contains(".") Or Me.Text.Contains(",")) AndAlso Not pKeyascii = ChrW(Windows.Forms.Keys.Back) Then
                        result = False
                    Else
                        result = True
                    End If
                End If

            Catch ex As Exception
                Throw ex
            End Try

            Return result
        End Function

#End Region

#Region "Private Events"

        Private Sub BSTextBox_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles Me.KeyPress
            If IsNumeric Then
                If ValidateNumericValue(e.KeyChar) Then
                    e.Handled = False
                Else
                    e.Handled = True
                End If
            End If
        End Sub

        Private Sub me_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Leave
            ValidateTextControl()
        End Sub

        Private Sub me_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Enter
            ValidateTextControl()
        End Sub

        Private Sub me_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.TextChanged
            ValidateTextControl()
        End Sub



#End Region
    End Class
End Namespace
