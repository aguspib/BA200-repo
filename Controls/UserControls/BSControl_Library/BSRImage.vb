Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Math

''' <summary>
''' This control is a picturebox that rotate the image on the control.
''' </summary>
''' <remarks>
''' Create by: AG
''' Modified by: TR 
'''              RH 10/02/2011
''' </remarks>
Public Class BSRImage
    Inherits PictureBox

#Region " IDisposable Support "  ' XB 17/01/2014 - Implements Dispose Functions against memory leaks
    Implements IDisposable

    ' To detect redundant calls
    Private hasBeenDisposed As Boolean = False
    '' Implement IDisposable.
    'Public Overload Sub Dispose() Implements IDisposable.Dispose
    '    Dispose(True)
    '    MyBase.Dispose()
    '    GC.SuppressFinalize(Me)
    'End Sub

    Protected Overridable Overloads Sub Dispose(ByVal disposing As Boolean)
        If Me.hasBeenDisposed = False Then
            If disposing Then
                ' Free other state (managed objects).

                hasImageChanged = Nothing
                bm_in = Nothing
                wid = Nothing
                hgt = Nothing
                theta = Nothing
                sin_theta = Nothing
                cos_theta = Nothing
                cx = Nothing
                cy = Nothing
                'i = Nothing
                bm_out = Nothing
                corners = Nothing
                threepoints = Nothing
                threepointsbeforemodify = Nothing
                rg = Nothing
                bgr = Nothing
                gr_out = Nothing
                bkBrush = Nothing

                _degree = Nothing
                _sizemode = Nothing
                _transColor = Nothing
                _direction = Nothing
                _showThrough = Nothing
                _imagePath = Nothing
                _isTransparentImage = Nothing
            End If
            ' Free your own state (unmanaged objects).
            ' Set large fields to null.
        End If
        Me.hasBeenDisposed = True
    End Sub

    Protected Overrides Sub Finalize()
        ' Simply call Dispose(False).
        Dispose(False)
        MyBase.Finalize()
    End Sub
#End Region

#Region "Declarations"

    Public Enum DirectionEnum
        Counter_Clockwise = -1
        Clockwise = 1
    End Enum

    Private hasImageChanged As Boolean = True 'AG 06/12/2010

    'RH 10/02/2011
    Private bm_in As Bitmap
    Private wid As Single
    Private hgt As Single
    Private theta As Single
    Private sin_theta As Single
    Private cos_theta As Single
    Private cx As Integer
    Private cy As Integer
    'Private i As Integer
    Private bm_out As Bitmap
    Private corners(3) As Point
    Private threepoints(2) As Point
    Private threepointsbeforemodify(2) As Point
    Private rg As Region
    Private bgr As Graphics
    Private gr_out As Graphics
    Private bkBrush As New SolidBrush(BackColor)
    'RH 10/02/2011 END
#End Region

#Region "Fields"

    Private _degree As Integer = 0
    Private _sizemode As PictureBoxSizeMode
    Private _transColor As Color
    Private _direction As DirectionEnum = DirectionEnum.Clockwise
    Private _showThrough As Boolean
    Private _imagePath As String = String.Empty 'AG 06/12/2010
    Private _isTransparentImage As Boolean = False 'AG 09/12/2010

#End Region

#Region "Properties"
    <Description("Space not filled in by image shows the controls beneath it.")> _
    Public Property ShowThrough() As Boolean
        Get
            Return _showThrough
        End Get
        Set(ByVal Value As Boolean)
            _showThrough = Value
            Me.Invalidate()
        End Set
    End Property

    <Description("Controls the direction of the rotation.")> _
    Public Property Direction() As DirectionEnum
        Get
            Return _direction
        End Get
        Set(ByVal Value As DirectionEnum)
            _direction = Value
            If Not MyBase.Image Is Nothing Then 'RH 10/02/2011
                InitValues()
            End If
            Me.Invalidate()
        End Set
    End Property

    <Description("The angle of rotation (in degrees).")> _
    Public Property Rotation() As Integer
        Get
            Return _degree
        End Get
        Set(ByVal Value As Integer)
            _degree = ValidRotation(Value)
            If Not MyBase.Image Is Nothing Then 'RH 10/02/2011
                InitValues()
            End If
            Me.Invalidate()
        End Set
    End Property

    <Description("The color in the image to make transparent.  Web->Transparent is none.")> _
    Public Property TransparentColor() As Color
        Get
            Return _transColor
        End Get
        Set(ByVal Value As Color)
            _transColor = Value
            Me.Invalidate()
        End Set
    End Property

    Public Shadows Property SizeMode() As PictureBoxSizeMode
        Get
            Return _sizemode
        End Get
        Set(ByVal Value As PictureBoxSizeMode)
            _sizemode = Value
            If Not MyBase.Image Is Nothing Then 'RH 10/02/2011
                InitValues()
            End If
            Me.Invalidate()
        End Set
    End Property

    'AG 06/12/2010
    Public Property ImagePath() As String
        Get
            Return _imagePath
        End Get
        Set(ByVal value As String)
            If _imagePath <> value Then hasImageChanged = True
            _imagePath = value
        End Set
    End Property
    'END AG 06/12/2010

    'AG 09/12/2010
    Public Property IsTransparentImage() As Boolean
        Get
            Return _isTransparentImage
        End Get
        Set(ByVal value As Boolean)
            _isTransparentImage = value
        End Set
    End Property
    'END AG 09/12/2010

    Public Overloads Property Image() As Image 'RH 10/02/2011
        Get
            Return MyBase.Image
        End Get
        Set(ByVal value As Image)
            MyBase.Image = value
            If Not value Is Nothing Then
                InitValues()
            End If
        End Set
    End Property

    Public Overloads Property BackColor() As Color 'RH 10/02/2011
        Get
            Return MyBase.BackColor
        End Get
        Set(ByVal value As Color)
            MyBase.BackColor = value
            bkBrush = New SolidBrush(value)
        End Set
    End Property
#End Region

#Region "Constructor"

    Public Sub New()
        Try
            MyBase.SetStyle(ControlStyles.SupportsTransparentBackColor, True)

            ' XB 10/03/2014 - Add Try Catch section
        Catch ex As Exception
            ' Write into Log
            Dim myLogAcciones As New Biosystems.Ax00.Global.ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "BSRImage.New", EventLogEntryType.Error, False)
            ' Throw ex --> Do not work, no business catch this throw, just MyApplication_UnhandledException !
        End Try

    End Sub
#End Region


#Region "Functions/Subs"

    ''' <summary>
    ''' Validate the rotation value is between 0 and 360
    ''' </summary>
    ''' <param name="Value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ValidRotation(ByVal Value As Integer) As Integer
        Try
            'If Value >= 0 And Value < 360 Then
            '    Return Value
            'End If
            'If Value >= 360 Then
            '    Value -= 360
            'ElseIf Value < 0 Then
            '    Value += 360
            'End If
            'Value = ValidRotation(Value)
            'Return Value

            'RH 09/02/2011
            Value = Value Mod 360
            If Value < 0 Then
                Value = 360 + Value
            End If

            ' XB 10/03/2014 - Add Try Catch section
        Catch ex As Exception
            ' Write into Log
            Dim myLogAcciones As New Biosystems.Ax00.Global.ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "BSRImage.ValidRotation", EventLogEntryType.Error, False)
            ' Throw ex --> Do not work, no business catch this throw, just MyApplication_UnhandledException !
        End Try

        Return Value
    End Function

    'RH 09/02/2011 Old and slow version
    'Protected Overrides Sub OnPaint(ByVal pe As PaintEventArgs)
    '    If MyBase.Image Is Nothing Then
    '        If Not hasImageChanged Then Exit Sub 'AG 06/12/2010 - comment this line to avoid bad bottle images transparencies
    '        Dim b As Brush
    '        b = New SolidBrush(Me.BackColor)
    '        pe.Graphics.FillRectangle(b, 0, 0, MyBase.Width, MyBase.Height)
    '        hasImageChanged = False 'AG 06/12/2010
    '        Exit Sub
    '    End If

    '    'If Not hasImageChanged Then Exit Sub 'AG 06/12/2010 - comment this line to avoid bad bottle images transparencies

    '    Dim bm_in As New Bitmap(MyBase.Image)

    '    Dim wid As Single = bm_in.Width
    '    Dim hgt As Single = bm_in.Height

    '    Dim corners As Point() = { _
    '         New Point(0, 0), _
    '         New Point(wid, 0), _
    '         New Point(0, hgt), _
    '         New Point(wid, hgt)}

    '    Dim cx As Single = wid / 2
    '    Dim cy As Single = hgt / 2
    '    Dim i As Long
    '    For i = 0 To 3
    '        corners(i).X -= cx
    '        corners(i).Y -= cy
    '    Next

    '    Dim theta As Single = CSng((_degree) * _direction) * PI / 180

    '    Dim sin_theta As Single = Sin(theta)
    '    Dim cos_theta As Single = Cos(theta)

    '    Dim X As Single
    '    Dim Y As Single
    '    For i = 0 To 3
    '        X = corners(i).X
    '        Y = corners(i).Y
    '        corners(i).X = (X * cos_theta) - (Y * sin_theta)
    '        corners(i).Y = (Y * cos_theta) + (X * sin_theta)
    '    Next

    '    Dim xmin As Single = corners(0).X
    '    Dim ymin As Single = corners(0).Y
    '    For i = 1 To 3
    '        If xmin > corners(i).X Then xmin = corners(i).X
    '        If ymin > corners(i).Y Then ymin = corners(i).Y
    '    Next
    '    For i = 0 To 3
    '        corners(i).X -= xmin
    '        corners(i).Y -= ymin
    '    Next
    '    Dim bm_out As New Bitmap(CInt(-2 * xmin), CInt(-2 * ymin))

    '    Dim rg As Region = CreateTransRegion(corners)
    '    Dim tp As Point = corners(3)
    '    ReDim Preserve corners(2)

    '    'AG 09/12/2010 - AG 07/12/2010
    '    ''Dim bgr As Graphics = Graphics.FromImage(bm_out)
    '    ''bgr.DrawImage(bm_in, corners)
    '    'If hasImageChanged Then
    '    If hasImageChanged And Not IsTransparentImage Then
    '        Dim bgr As Graphics = Graphics.FromImage(bm_out)
    '        bgr.DrawImage(bm_in, corners)
    '    End If
    '    'END AG 09/12/2010 - END AG 07/12/2010

    '    Dim gr_out As Graphics = pe.Graphics
    '    gr_out.FillRectangle(New SolidBrush(Me.BackColor), 0, 0, Me.Width, Me.Height)
    '    bm_in.MakeTransparent(_transColor)
    '    If _sizemode = PictureBoxSizeMode.StretchImage Then
    '        Dim maxW As Integer = tp.X
    '        Dim maxH As Integer = tp.Y
    '        For t As Integer = 0 To 2
    '            If maxW < corners(t).X Then maxW = corners(t).X
    '            If maxH < corners(t).Y Then maxH = corners(t).Y
    '        Next
    '        'get hscale
    '        Dim hscale As Double = Me.Width / maxW
    '        'get vscale
    '        Dim vscale As Double = Me.Height / maxH
    '        'convert points
    '        corners(0) = New Point(corners(0).X * hscale, corners(0).Y * vscale)
    '        corners(1) = New Point(corners(1).X * hscale, corners(1).Y * vscale)
    '        corners(2) = New Point(corners(2).X * hscale, corners(2).Y * vscale)

    '        'AG 07/12/2010
    '        gr_out.DrawImage(bm_out, 0, 0, Me.Width, Me.Height)
    '        'If hasImageChanged Then gr_out.DrawImage(bm_out, 0, 0, Me.Width, Me.Height)
    '        'END AG 07/12/2010

    '        Dim np(3) As Point
    '        np(0) = corners(0)
    '        np(1) = corners(1)
    '        np(2) = corners(2)
    '        np(3) = New Point(tp.X * hscale, tp.Y * vscale)
    '        rg = CreateTransRegion(np)
    '    ElseIf _sizemode = PictureBoxSizeMode.CenterImage Then
    '        Dim wadd As Integer = CInt((Me.Width / 2) - (bm_out.Width / 2))
    '        Dim hadd As Integer = CInt((Me.Height / 2) - (bm_out.Height / 2))
    '        corners(0) = New Point(corners(0).X + wadd, corners(0).Y + hadd)
    '        corners(1) = New Point(corners(1).X + wadd, corners(1).Y + hadd)
    '        corners(2) = New Point(corners(2).X + wadd, corners(2).Y + hadd)

    '        'AG 07/12/2010
    '        gr_out.DrawImage(bm_in, corners)
    '        'If hasImageChanged Then gr_out.DrawImage(bm_in, corners)
    '        'END AG 07/12/2010

    '        Dim np(3) As Point
    '        np(0) = corners(0)
    '        np(1) = corners(1)
    '        np(2) = corners(2)
    '        np(3) = New Point(tp.X + wadd, tp.Y + hadd)
    '        rg = CreateTransRegion(np)
    '    Else
    '        'AG 07/12/2010
    '        gr_out.DrawImage(bm_in, corners)
    '        'If hasImageChanged Then gr_out.DrawImage(bm_in, corners)
    '        'END AG 07/12/2010
    '    End If
    '    If _sizemode = PictureBoxSizeMode.AutoSize Then
    '        MyBase.Width = bm_out.Width
    '        MyBase.Height = bm_out.Height
    '    End If
    '    Me.Region = Nothing
    '    If _showThrough Then
    '        Me.Region = rg
    '    End If

    '    hasImageChanged = False 'AG 06/12/2010

    'End Sub

    ''' <summary>
    ''' Event handler for OnPaint. Paints the BSRimage.
    ''' </summary>
    ''' <param name="pe">PaintEventArgs</param>
    ''' <remarks>
    ''' Created by: RH 10/02/2011
    ''' </remarks>
    Protected Overrides Sub OnPaint(ByVal pe As PaintEventArgs)
        Try
            If MyBase.Image Is Nothing Then
                'MyBase.OnPaint(pe)
                Return
            End If

            If hasImageChanged AndAlso Not IsTransparentImage Then
                bgr.DrawImage(bm_in, threepointsbeforemodify)
            End If

            gr_out = pe.Graphics
            gr_out.FillRectangle(bkBrush, 0, 0, Me.Width, Me.Height)
            bm_in.MakeTransparent(_transColor)

            If _sizemode = PictureBoxSizeMode.StretchImage Then
                gr_out.DrawImage(bm_out, 0, 0, Me.Width, Me.Height)
            Else
                gr_out.DrawImage(bm_in, threepoints)
            End If

            hasImageChanged = False

            ' XB 10/03/2014 - Add Try Catch section
        Catch ex As Exception
            ' Write into Log
            Dim myLogAcciones As New Biosystems.Ax00.Global.ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "BSRImage.OnPaint", EventLogEntryType.Error, False)
            ' Throw ex --> Do not work, no business catch this throw, just MyApplication_UnhandledException !
        End Try
    End Sub

    ''' <summary>
    ''' Initializes some values needed for painting the image.
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 10/02/2011
    ''' </remarks>
    Private Sub InitValues()
        Try
            bm_in = New Bitmap(MyBase.Image)

            wid = bm_in.Width
            hgt = bm_in.Height
            cx = CInt(wid / 2)
            cy = CInt(hgt / 2)

            corners(0).X = -cx
            corners(0).Y = -cy

            corners(1).X = CInt(wid - cx)
            corners(1).Y = -cy

            corners(2).X = -cx
            corners(2).Y = CInt(hgt - cy)

            corners(3).X = CInt(wid - cx)
            corners(3).Y = CInt(hgt - cy)

            theta = CSng(((_degree) * _direction) * PI / 180)
            sin_theta = CSng(Math.Sin(theta))
            cos_theta = CSng(Math.Cos(theta))

            Dim X As Integer
            Dim Y As Integer

            'Print(i)

            For i = 0 To 3
                X = corners(i).X
                Y = corners(i).Y
                corners(i).X = CInt((X * cos_theta) - (Y * sin_theta))
                corners(i).Y = CInt((Y * cos_theta) + (X * sin_theta))
            Next

            Dim xmin As Integer = corners(0).X
            Dim ymin As Integer = corners(0).Y

            For i = 1 To 3
                If xmin > corners(i).X Then xmin = corners(i).X
                If ymin > corners(i).Y Then ymin = corners(i).Y
            Next

            For i = 0 To 3
                corners(i).X -= xmin
                corners(i).Y -= ymin
            Next

            bm_out = New Bitmap(-2 * xmin, -2 * ymin)
            bgr = Graphics.FromImage(bm_out)

            If _sizemode = PictureBoxSizeMode.AutoSize Then
                MyBase.Width = bm_out.Width
                MyBase.Height = bm_out.Height
            End If

            Array.Copy(corners, threepointsbeforemodify, 3)

            If _sizemode = PictureBoxSizeMode.StretchImage Then
                Dim maxW As Integer = corners(3).X
                Dim maxH As Integer = corners(3).Y

                For i = 0 To 2
                    If maxW < corners(i).X Then maxW = corners(i).X
                    If maxH < corners(i).Y Then maxH = corners(i).Y
                Next

                'get hscale
                Dim hscale As Integer = CInt(Me.Width / maxW)

                'get vscale
                Dim vscale As Integer = CInt(Me.Height / maxH)

                'convert points
                For i = 0 To 3
                    corners(i).X = corners(i).X * hscale
                    corners(i).Y = corners(i).Y * vscale
                Next


            ElseIf _sizemode = PictureBoxSizeMode.CenterImage Then

                Dim wadd As Integer = CInt((Me.Width / 2) - (bm_out.Width / 2))
                Dim hadd As Integer = CInt((Me.Height / 2) - (bm_out.Height / 2))

                For i = 0 To 3
                    corners(i).X = corners(i).X + wadd
                    corners(i).Y = corners(i).Y + hadd
                Next

                Array.Copy(corners, threepoints, 3)
            End If

            rg = CreateTransRegion(corners)

            If _showThrough Then
                Me.Region = rg
            Else
                If Not Me.Region Is Nothing Then
                    Me.Region.Dispose()
                End If
            End If

            ' XB 10/03/2014 - Add Try Catch section
        Catch ex As Exception
            ' Write into Log
            Dim myLogAcciones As New Biosystems.Ax00.Global.ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "BSRImage.InitValues", EventLogEntryType.Error, False)
            ' Throw ex --> Do not work, no business catch this throw, just MyApplication_UnhandledException !
        End Try

    End Sub

    Private Function CreateTransRegion(ByRef points() As Point) As Region
        Try
            '0,1,3,2
            Dim m_p(3) As Point
            m_p(0) = points(0)
            m_p(1) = points(1)
            m_p(2) = points(3)
            m_p(3) = points(2)

            Dim p_types(3) As Byte
            'For i As Integer = 0 To 3
            '    p_types(i) = CByte(Drawing2D.PathPointType.Line)
            'Next
            'p_types(0) = CByte(Drawing2D.PathPointType.Line)
            'p_types(1) = CByte(Drawing2D.PathPointType.Line)
            'p_types(2) = CByte(Drawing2D.PathPointType.Line)
            'p_types(3) = CByte(Drawing2D.PathPointType.Line)

            p_types(0) = 1
            p_types(1) = 1
            p_types(2) = 1
            p_types(3) = 1

            Dim path As New Drawing2D.GraphicsPath(m_p, p_types)

            Dim p_region As New Region(path)

            Return p_region

            ' XB 10/03/2014 - Add Try Catch section
        Catch ex As Exception
            ' Write into Log
            Dim myLogAcciones As New Biosystems.Ax00.Global.ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "BSRImage.CreateTransRegion", EventLogEntryType.Error, False)
            ' Throw ex --> Do not work, no business catch this throw, just MyApplication_UnhandledException !
            Return Nothing
        End Try

    End Function
#End Region

End Class

