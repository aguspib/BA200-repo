Option Explicit On
Option Strict On


Namespace Biosystems.Ax00.Global

    Public Class FWUpdateRequestTO

        Public Sub New()

        End Sub

        Public Sub New(ByVal pActionType As GlobalEnumerates.FwUpdateActions)
            MyClass.ActionTypeAttr = pActionType
        End Sub


        Private ActionTypeAttr As GlobalEnumerates.FwUpdateActions
        Private DataBlockBytesAttr As Byte() = Nothing
        Private DataBlockIndexAttr As Integer = 0
        Private DataBlockSizeAttr As Integer = 0

        Public Property ActionType() As GlobalEnumerates.FwUpdateActions
            Get
                Return ActionTypeAttr
            End Get
            Set(ByVal value As GlobalEnumerates.FwUpdateActions)
                ActionTypeAttr = value
            End Set
        End Property

        Public Property DataBlockBytes() As Byte()
            Get
                Return DataBlockBytesAttr
            End Get
            Set(ByVal value As Byte())
                DataBlockBytesAttr = value
            End Set
        End Property

        Public ReadOnly Property DataBlockString() As String
            Get
                Try
                    Dim res As String = ""
                    If MyClass.DataBlockBytesAttr IsNot Nothing AndAlso MyClass.DataBlockBytesAttr.Length > 0 Then
                        res = System.Text.ASCIIEncoding.ASCII.GetString(MyClass.DataBlockBytesAttr)
                    End If
                    Return res
                Catch ex As Exception
                    Return ""
                End Try
            End Get
        End Property

        Public Property DataBlockIndex() As Integer
            Get
                Return DataBlockIndexAttr
            End Get
            Set(ByVal value As Integer)
                DataBlockIndexAttr = value
            End Set
        End Property

        Public Property DataBlockSize() As Integer
            Get
                Return DataBlockSizeAttr
            End Get
            Set(ByVal value As Integer)
                DataBlockSizeAttr = value
            End Set
        End Property

    End Class

End Namespace
