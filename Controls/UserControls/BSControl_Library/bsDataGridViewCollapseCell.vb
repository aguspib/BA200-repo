Imports System
Imports System.Drawing
Imports System.Windows.Forms

Namespace Biosystems.Ax00.Controls.UserControls

    ''' <summary>
    ''' Defines a Collapse cell type for the System.Windows.Forms.DataGridView control
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 07/06/2010
    ''' </remarks>
    Public Class bsDataGridViewCollapseCell
        Inherits DataGridViewImageCell

#Region "Fields"

        Private SubHeader As Boolean = False
        Private Collapsed As Boolean = False
        Private PlusImage As Image
        Private MinusImage As Image
        Private NoImage As Image
        Private Enabled As Boolean

#End Region

#Region "Properties"

        Public Property IsSubHeader() As Boolean
            Get
                Return SubHeader
            End Get

            Set(ByVal value As Boolean)
                SubHeader = value

                If SubHeader Then
                    If Collapsed Then
                        MyBase.Value = PlusImage
                    Else
                        MyBase.Value = MinusImage
                    End If
                Else
                    Collapsed = False
                    MyBase.Value = NoImage
                End If
            End Set
        End Property

        Public Property IsCollapsed() As Boolean
            Get
                Return Collapsed
            End Get

            Set(ByVal value As Boolean)
                If Enabled AndAlso SubHeader Then
                    Collapsed = value

                    If Collapsed Then
                        MyBase.Value = PlusImage
                    Else
                        MyBase.Value = MinusImage
                    End If

                    Dim MaxRows As Integer

                    If RowsCount = 0 Then
                        MaxRows = DataGridView.Rows.Count
                    Else
                        MaxRows = RowsCount
                    End If

                    Dim i As Integer = Me.RowIndex + 1

                    While i < MaxRows AndAlso CType(DataGridView(Me.ColumnIndex, i), bsDataGridViewCollapseCell).IsSubHeader = False
                        DataGridView.Rows(i).Visible = Not Collapsed
                        i += 1
                    End While
                End If
            End Set
        End Property

        Public Property IsEnabled() As Boolean
            Get
                Return Enabled
            End Get
            Set(ByVal value As Boolean)
                Enabled = value

                If Enabled Then
                    If IsSubHeader Then
                        If Collapsed Then
                            MyBase.Value = PlusImage
                        Else
                            MyBase.Value = MinusImage
                        End If
                    Else
                        MyBase.Value = NoImage
                    End If
                Else
                    MyBase.Value = NoImage
                End If
            End Set
        End Property

        Private ReadOnly Property RowsCount() As Integer
            Get
                Return CType(Me.DataGridView.Columns(Me.ColumnIndex), bsDataGridViewCollapseColumn).RowsCount
            End Get
        End Property

#End Region

#Region "Public Methods"

        Public Sub New()
            MyBase.New()

            PlusImage = My.Resources.expand1 '.plus_16
            MinusImage = My.Resources.collapse1 '.minus_16
            NoImage = My.Resources.noimage

            Enabled = True
            IsSubHeader = False
        End Sub

#End Region

#Region "Protected Methods"

        Protected Overrides Sub OnMouseClick(ByVal e As DataGridViewCellMouseEventArgs)
            If Not Enabled Then Return

            If e.RowIndex >= 0 Then IsCollapsed = Not IsCollapsed
        End Sub

#End Region

    End Class

End Namespace