Imports System.Windows.Forms

Namespace Biosystems.Ax00.Controls.UserControls

    Public Class BSErrorProvider

#Region "Declarations"

        Private ErrorStatusLabel As ToolStripStatusLabel

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Clears all settings associated with this component.
        ''' </summary>
        ''' <remarks>
        ''' Created by: RH - 25/10/2010
        ''' </remarks>
        Public Overloads Sub Clear()
            MyBase.Clear()

            UpdateErrorStatusLabel()
            If (Not ErrorStatusLabel Is Nothing) Then
                ErrorStatusLabel.DisplayStyle = ToolStripItemDisplayStyle.None
                ErrorStatusLabel.Text = String.Empty
            End If
        End Sub

        ''' <summary>
        ''' Sets the error description string for the specified control.
        ''' </summary>
        ''' <remarks>
        ''' Created by:  RH 25/10/2010
        ''' Modified by: SA 10/11/2010 - When value is empty, clean also the Icon 
        ''' </remarks>
        Public Overloads Sub SetError(ByVal control As Control, ByVal value As String)
            If Not control Is Nothing Then
                MyBase.SetError(control, value)
            End If

            UpdateErrorStatusLabel()
            If (Not ErrorStatusLabel Is Nothing) Then
                If (value <> String.Empty) Then
                    ErrorStatusLabel.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
                Else
                    ErrorStatusLabel.DisplayStyle = ToolStripItemDisplayStyle.None
                End If
                ErrorStatusLabel.Text = value
            End If
        End Sub

        ''' <summary>
        ''' Shows a message error on the ErrorStatusLabel
        ''' </summary>
        ''' <remarks>
        ''' Created by:  RH 14/02/2012
        ''' </remarks>
        Public Sub ShowError(ByVal value As String)
            SetError(Nothing, value)
        End Sub

#End Region

#Region "Private Methods"

        ''' <summary>
        ''' Looks for a parent form and a BSStatusStrip inside. 
        ''' Then search for the first ToolStripStatusLabel.
        ''' Finally takes this ToolStripStatusLabel as the actual ErrorStatusLabel.
        ''' </summary>
        ''' <remarks>
        ''' Created by:  RH 25/10/2010
        ''' Modified by: SA 18/11/2010 - If the ParentForm is not an MDI Container, up one level (the Parent Form
        '''                              of the ParentForm). This is needed to shown messages that are inside an 
        '''                              User Control
        ''' </remarks>
        Private Sub UpdateErrorStatusLabel()
            If (ErrorStatusLabel Is Nothing) Then
                If (Not ContainerControl Is Nothing) AndAlso (Not ContainerControl.ParentForm Is Nothing) AndAlso _
                   (Not ContainerControl.ParentForm.Controls Is Nothing) Then
                    Dim myControls As Control.ControlCollection = Nothing

                    'The Parent Form is the Main MDIForm...
                    If (ContainerControl.ParentForm.IsMdiContainer) Then
                        myControls = ContainerControl.ParentForm.Controls
                    Else
                        'The Parent Form is an MDI Child... move one level up to reach the Main MDIForm
                        If (ContainerControl.ParentForm.ParentForm.IsMdiContainer) Then
                            myControls = ContainerControl.ParentForm.ParentForm.Controls
                        End If
                    End If

                    'Search the control in the MDI in which the message has to be shown
                    Dim StatusStrip As BSStatusStrip
                    For Each c As Control In myControls
                        If (TypeOf (c) Is BSStatusStrip) Then
                            StatusStrip = CType(c, BSStatusStrip)
                            If (StatusStrip.Items.Count > 0) Then
                                ErrorStatusLabel = CType(StatusStrip.Items(0), ToolStripStatusLabel)
                            End If
                            Exit For
                        End If
                    Next c
                End If
            End If
        End Sub
#End Region

    End Class

End Namespace