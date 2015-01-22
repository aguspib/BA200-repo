Option Explicit On
Option Strict On

Namespace Biosystems.Ax00.Global

    Public Class ISEDallasPage01TO


#Region "Attributes"
        Private Page01DataStringAttr As String = ""
        Private ConsumptionCalAAttr As Integer = -1
        Private InstallationDayAttr As Integer = -1
        Private InstallationMonthAttr As Integer = -1
        Private InstallationYearAttr As Integer = -1
        Private ConsumptionCalBAttr As Integer = -1
        Private NoGoodByteAttr As String = ""
        Private ConsumptionCalAbitDataAttr As String = ""
        Private ConsumptionCalBbitDataAttr As String = ""
        Private ValidationErrorAttr As Boolean = False 'SGM 06/06/2012
#End Region

#Region "Properties"

        Public Property Page01DataString() As String
            Get
                Return Page01DataStringAttr
            End Get
            Set(ByVal value As String)
                Page01DataStringAttr = value
            End Set
        End Property

        ''' <summary>
        ''' consumption value in percent
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ConsumptionCalA() As Integer
            Get
                Return ConsumptionCalAAttr
            End Get
            Set(ByVal value As Integer)
                ConsumptionCalAAttr = value
            End Set
        End Property
        Public Property InstallationDay() As Integer
            Get
                Return InstallationDayAttr
            End Get
            Set(ByVal value As Integer)
                If value > 0 And value <= 31 Then
                    InstallationDayAttr = value
                Else
                    InstallationDayAttr = -1
                    If value <> 255 Then ValidationErrorAttr = True
                End If
            End Set
        End Property
        Public Property InstallationMonth() As Integer
            Get
                Return InstallationMonthAttr
            End Get
            Set(ByVal value As Integer)
                If value > 0 And value <= 12 Then
                    InstallationMonthAttr = value
                Else
                    InstallationMonthAttr = -1
                    If value <> 255 Then ValidationErrorAttr = True
                End If
            End Set
        End Property
        Public Property InstallationYear() As Integer
            Get
                Return InstallationYearAttr
            End Get
            Set(ByVal value As Integer)
                If value > 2000 And value <= 2100 Then
                    InstallationYearAttr = value
                Else
                    InstallationYearAttr = -1
                    If value <> 255 Then ValidationErrorAttr = True
                End If
            End Set
        End Property
        Public ReadOnly Property InstallationDate() As DateTime
            Get
                If Me.InstallationYearAttr >= 0 And Me.InstallationMonthAttr >= 1 And Me.InstallationDayAttr >= 1 Then
                    Dim myDate As New DateTime(Me.InstallationYear, Me.InstallationMonthAttr, Me.InstallationDayAttr)
                    Return myDate ' Nothing for testing
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' consumption value in percent
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ConsumptionCalB() As Integer
            Get
                Return ConsumptionCalBAttr
            End Get
            Set(ByVal value As Integer)
                ConsumptionCalBAttr = value
            End Set
        End Property
        Public Property NoGoodByte() As String
            Get
                Return NoGoodByteAttr
            End Get
            Set(ByVal value As String)
                NoGoodByteAttr = value
            End Set
        End Property

        Public Property ConsumptionCalAbitData() As String
            Get
                Return Me.ConsumptionCalAbitDataAttr
            End Get
            Set(ByVal value As String)
                Me.ConsumptionCalAbitDataAttr = value
            End Set
        End Property
        Public Property ConsumptionCalBbitData() As String
            Get
                Return Me.ConsumptionCalBbitDataAttr
            End Get
            Set(ByVal value As String)
                Me.ConsumptionCalBbitDataAttr = value
            End Set
        End Property

        'error because of wrong mapping of the data - is not Biosystems pack
        Public Property ValidationError() As Boolean
            Get
                Return ValidationErrorAttr
            End Get
            Set(ByVal value As Boolean)
                ValidationErrorAttr = value
            End Set
        End Property

#End Region

#Region "Constructor"

        Public Sub New()

        End Sub

#End Region


        Private Function GetConsumptionValue(ByVal pHex As String) As Integer

            Dim myPercentCounts As Integer = 0

            Try
                'it is necessary to split into positions
                Dim myPositions As New List(Of String) 'there must be 13 positions
                For c As Integer = 0 To pHex.Length - 1 Step 2
                    myPositions.Add(pHex.Substring(c, 2))
                Next
                For Each p As String In myPositions
                    Dim myDecimalValue As Integer = Convert.ToInt32(p, 16)
                    Dim myBinaryString As String = Convert.ToString(myDecimalValue, 2)

                    For b As Integer = 0 To myBinaryString.Length - 1 Step 1
                        If myBinaryString.Substring(b, 1) = "0" Then myPercentCounts += 1
                    Next

                Next

            Catch ex As Exception
                Throw ex
            End Try

            Return myPercentCounts

        End Function

        Private Function ConvertHexToUInt32(ByVal pHex As String) As Integer
            Dim intResult As Integer = -1
            Try
                intResult = CInt("&H" & pHex)
            Catch ex As Exception
                Throw ex
            End Try
            Return intResult
        End Function

        Private Function ConvertHexToString(ByVal pHex As String) As String
            Dim strResult As String = ""
            Try
                Dim myBytes As New List(Of String)
                For c As Integer = 0 To pHex.Length - 1 Step 2
                    myBytes.Add(pHex.Substring(c, 2))
                Next
                For Each b As String In myBytes
                    strResult &= Chr(CInt("&H" & b))
                Next
            Catch ex As Exception
                Throw ex
            End Try
            Return strResult
        End Function


        Public Overrides Function ToString() As String
            Return Me.Page01DataStringAttr
        End Function

    End Class
End Namespace