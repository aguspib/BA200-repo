Option Explicit On
Option Strict On

Imports System.Windows.Forms
Imports System.Drawing
Imports System.Windows.Forms.VisualStyles

''' <summary>
''' This class is used to enable/disable the TextBox Dataview items.
''' </summary>
''' <remarks></remarks>
''' 

Public Class DataGridViewDisableTextBoxCell
    Inherits DataGridViewTextBoxCell

    Private enabledValue As Boolean
    Public Property Enabled() As Boolean
        Get
            Return enabledValue
        End Get
        Set(ByVal value As Boolean)
            enabledValue = value
        End Set
    End Property

    ' Override the Clone method so that the Enabled property is copied.
    Public Overrides Function Clone() As Object
        Dim Cell As DataGridViewDisableTextBoxCell = _
            CType(MyBase.Clone(), DataGridViewDisableTextBoxCell)
        Cell.Enabled = Me.Enabled
        Return Cell
    End Function

    ' By default, enable the TextBox cell.
    Public Sub New()
        Me.enabledValue = True
    End Sub

    Protected Overrides Sub Paint(ByVal graphics As Graphics, _
        ByVal clipBounds As Rectangle, ByVal cellBounds As Rectangle, _
        ByVal rowIndex As Integer, _
        ByVal elementState As DataGridViewElementStates, _
        ByVal value As Object, ByVal formattedValue As Object, _
        ByVal errorText As String, _
        ByVal cellStyle As DataGridViewCellStyle, _
        ByVal advancedBorderStyle As DataGridViewAdvancedBorderStyle, _
        ByVal paintParts As DataGridViewPaintParts)
        Try
            ' The TextBox cell is disabled, so paint the border,  
            ' background, and disabled TextBox for the cell.
            If Not Me.enabledValue Then

                ' Draw the background of the cell, if specified.
                If (paintParts And DataGridViewPaintParts.Background) = _
                    DataGridViewPaintParts.Background Then

                    Dim cellBackground As New SolidBrush(cellStyle.BackColor)
                    graphics.FillRectangle(cellBackground, cellBounds)
                    cellBackground.Dispose()
                End If

                ' Draw the cell borders, if specified.
                If (paintParts And DataGridViewPaintParts.Border) = _
                    DataGridViewPaintParts.Border Then

                    PaintBorder(graphics, clipBounds, cellBounds, cellStyle, _
                        advancedBorderStyle)
                End If

                ' Calculate the area in which to draw the TextBox.
                Dim TextBoxArea As Rectangle = cellBounds
                Dim TextBoxAdjustment As Rectangle = _
                    Me.BorderWidths(advancedBorderStyle)
                TextBoxArea.X += TextBoxAdjustment.X
                TextBoxArea.Y += TextBoxAdjustment.Y
                TextBoxArea.Height -= TextBoxAdjustment.Height
                TextBoxArea.Width -= TextBoxAdjustment.Width

                ' Draw the disabled TextBox.                
                TextBoxRenderer.DrawTextBox(graphics, TextBoxArea, _
                    TextBoxState.Disabled)

                ' Draw the disabled TextBox text. 
                If TypeOf Me.FormattedValue Is String Then
                    TextRenderer.DrawText(graphics, CStr(Me.FormattedValue), _
                        Me.DataGridView.Font, TextBoxArea, SystemColors.GrayText)
                End If

            Else
                ' The TextBox cell is enabled, so let the base class 
                ' handle the painting.
                MyBase.Paint(graphics, clipBounds, cellBounds, rowIndex, _
                    elementState, value, formattedValue, errorText, _
                    cellStyle, advancedBorderStyle, paintParts)
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

End Class
