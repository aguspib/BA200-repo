'RH: Standard Numeric Format Strings
'http://msdn.microsoft.com/en-us/library/dwhawy9k.aspx

Option Explicit On
Option Strict On

Imports System.Runtime.CompilerServices

Namespace Biosystems.Ax00.Global

    'RH: Module for Extension Methods declarations
    'http://msdn.microsoft.com/en-us/library/bb384936.aspx
    Public Module Extensions

        Private culture As New Globalization.CultureInfo("en-US", False)

        ''' <summary>
        ''' Converts the value of this instance to a System.String using "en-US" Culture Info.
        ''' That is, the comma in 3,14 is replaced by a dot (3.14)
        ''' </summary>
        ''' <remarks>
        ''' Created by: RH - 19/05/2010
        ''' </remarks>
        <Extension()> _
        Public Function ToSQLString(ByVal v As Single) As String
            Return v.ToString("G20", culture)
        End Function

        ''' <summary>
        ''' Converts the value of this instance to a System.String using "en-US" Culture Info.
        ''' That is, the comma in 3,14 is replaced by a dot (3.14).
        ''' </summary>
        ''' <remarks>
        ''' Created by: RH - 19/05/2010
        ''' </remarks>
        <Extension()> _
        Public Function ToSQLString(ByVal v As Double) As String
            Return v.ToString("G20", culture)
        End Function

        ''' <summary>
        ''' Converts the value of this instance to a System.String using
        ''' SQL format yyyyMMdd HH:mm:ss.
        ''' </summary>
        ''' <remarks>
        ''' Created by: RH - 21/02/2012
        ''' </remarks>
        <Extension()> _
        Public Function ToSQLString(ByVal v As DateTime) As String
            Return v.ToString("yyyyMMdd HH:mm:ss")
        End Function

        ''' <summary>
        ''' Converts the value of this instance to a System.String using the current Culture Info
        ''' Represents "decimals" decimal digits.
        ''' </summary>
        ''' <remarks>
        ''' Created by: RH - 30/07/2010
        ''' AG 03/01/2012 - add pOnlySignficativeDecimals
        ''' RH 15/02/2012 - Code optimization
        ''' </remarks>
        <Extension()> _
        Public Function ToStringWithDecimals(ByVal v As Single, ByVal decimals As Integer, Optional ByVal pOnlySignificativeDecimals As Boolean = False) As String
            Dim format As String

            If decimals <= 0 Then
                format = "0"
            Else
                If pOnlySignificativeDecimals Then
                    Dim Epsilon As Double = 1 / Math.Pow(10, decimals)
                    Dim diff As Double = Math.Abs(v - Math.Round(v))

                    If diff < Epsilon Then
                        format = "0"
                    Else
                        format = "F" & decimals
                    End If
                Else
                    format = "F" & decimals
                End If
            End If

            Return v.ToString(format)
        End Function

        ''' <summary>
        ''' Converts the value of this instance to a System.String using the current Culture Info
        ''' Represents "decimals" decimal digits.
        ''' </summary>
        ''' <remarks>
        ''' Created by: RH - 30/07/2010
        ''' AG 03/01/2012 - add pOnlySignficativeDecimals
        ''' RH 15/02/2012 - Code optimization
        ''' </remarks>
        <Extension()> _
        Public Function ToStringWithDecimals(ByVal v As Double, ByVal decimals As Integer, Optional ByVal pOnlySignificativeDecimals As Boolean = False) As String
            Dim format As String

            If decimals <= 0 Then
                format = "0"
            Else
                If pOnlySignificativeDecimals Then
                    Dim Epsilon As Double = 1 / Math.Pow(10, decimals)
                    Dim diff As Double = Math.Abs(v - Math.Round(v))

                    If diff < Epsilon Then
                        format = "0"
                    Else
                        format = "F" & decimals
                    End If
                Else
                    format = "F" & decimals
                End If
            End If

            Return v.ToString(format)
        End Function

        Private Delegate Sub SetPropertyThreadSafeDelegate(ByVal control As System.Windows.Forms.Control, ByVal propertyName As String, ByVal propertyValue As Object)

        ''' <summary>
        ''' Updates a Control property from another thread
        ''' </summary>
        ''' <remarks>
        ''' Created by: RH - 15/11/2010
        ''' http://stackoverflow.com/questions/661561/how-to-update-gui-from-another-thread-in-c
        ''' </remarks>
        <Extension()> _
        Public Sub SetPropertyThreadSafe(ByVal control As System.Windows.Forms.Control, ByVal propertyName As String, ByVal propertyValue As Object)
            If (control.InvokeRequired) Then
                control.Invoke(New SetPropertyThreadSafeDelegate(AddressOf SetPropertyThreadSafe), New Object() {control, propertyName, propertyValue})
            Else
                control.GetType().InvokeMember(propertyName, System.Reflection.BindingFlags.SetProperty, Nothing, control, New Object() {propertyValue})
            End If
        End Sub

        ''' <summary>
        ''' Executes UI method from another thread
        ''' </summary>
        ''' <remarks>
        ''' Created by: RH - 26/05/2011
        ''' Modified by: MI+XB 27/01/2015 BA-2189 
        ''' http://www.codeproject.com/KB/cs/AvoidingInvokeRequired.aspx
        ''' </remarks>
        <Extension()> _
        Public Sub UIThread(ByVal control As System.Windows.Forms.Control, ByVal code As Action)
            'This method uses async version of UIThread(control, code, async flag) (see method below)
            UIThread(control, code, False)
        End Sub
        ''' <summary>
        ''' Created by MI+XB 27/01/2015 BA-2189
        ''' </summary>
        ''' <param name="control">Control execxution context for the thread operation</param>
        ''' <param name="code">Delegate of the code being executed</param>
        ''' <param name="WaitExecution">Request sync or async execution</param>
        ''' <remarks></remarks>
        <Extension()> _
        Public Sub UIThread(ByVal control As System.Windows.Forms.Control, ByVal code As Action, WaitExecution As Boolean)
            If (control.InvokeRequired) Then
                If WaitExecution Then
                    control.Invoke(code)
                Else
                    control.BeginInvoke(code)
                End If
                Return
            End If
            code.Invoke()
        End Sub



        ''' <summary>
        ''' Executes UI method from another thread
        ''' </summary>
        ''' <remarks>
        ''' Created by: RH - 26/05/2011
        ''' http://www.codeproject.com/KB/cs/AvoidingInvokeRequired.aspx
        ''' </remarks>
        <Extension()> _
        Public Sub UIThreadInvoke(ByVal control As System.Windows.Forms.Control, ByVal code As Action)
            If (control.InvokeRequired) Then
                control.Invoke(code)
                Return
            End If
            code.Invoke()
        End Sub

        ''' <summary>
        ''' Converts the value of this instance to a System.String using the current Culture Info.
        ''' Represents "decimals" decimal digits when they are "significative" (Not equal zero).
        ''' </summary>
        ''' <param name="v" ></param>
        ''' <param name="decimals" ></param>
        ''' <remarks>
        ''' Created by: RH - 20/10/2011
        ''' Floating point numbers should not be directly compared for equallity
        ''' For a discussion, see: http://stackoverflow.com/questions/17333/most-effective-way-for-float-and-double-comparison
        ''' </remarks>
        <Extension()> _
        Public Function ToStringWithPercent(ByVal v As Single, ByVal decimals As Integer) As String
            Dim format As String

            If decimals <= 0 Then
                Return v.ToString("F0") & "%" 'No decimals
            End If

            Dim Epsilon As Double = 1 / Math.Pow(10, decimals)
            Dim diff As Double = Math.Abs(v - Math.Round(v))

            If diff < Epsilon Then
                Return v.ToString("F0") & "%" 'No decimals
            End If

            format = "F" & decimals

            Return v.ToString(format) & "%"
        End Function

        ''' <summary>
        ''' Converts the value of this instance to a System.String using the current Culture Info.
        ''' Represents "decimals" decimal digits when they are "significative" (Not equal zero).
        ''' </summary>
        ''' <remarks>
        ''' Created by: RH - 20/10/2011
        ''' Floating point numbers should not be directly compared for equallity
        ''' For a discussion, see: http://stackoverflow.com/questions/17333/most-effective-way-for-float-and-double-comparison
        ''' </remarks>
        <Extension()> _
        Public Function ToStringWithPercent(ByVal v As Double, ByVal decimals As Integer) As String
            Dim format As String

            If decimals <= 0 Then
                Return v.ToString("F0") & "%" 'No decimals
            End If

            Dim Epsilon As Double = 1 / Math.Pow(10, decimals)
            Dim diff As Double = Math.Abs(v - Math.Round(v))

            If diff < Epsilon Then
                Return v.ToString("F0") & "%" 'No decimals
            End If

            format = "F" & decimals

            Return v.ToString(format) & "%"
        End Function

        ''' <summary>
        ''' Converts the value to Uppercase using Invariant Culture Info.
        ''' </summary>
        ''' <remarks>
        ''' Created by: XB - 01/02/2013 - (Bugs tracking #1112)
        ''' </remarks>
        <Extension()>
        Public Function ToUpperBS(ByVal pExtendedString As String) As String
            Dim out As String = pExtendedString
            Try
                out = pExtendedString.ToUpperInvariant()
            Catch ex As Exception
                Throw ex
            End Try
            Return out
        End Function

        ''' <summary>
        ''' Converts the value of this instance to a System.String using
        ''' XSD format YYYY-MM-ddTHH:mm:ss.
        ''' </summary>
        ''' <remarks>
        ''' Created by:  SG - 26/02/2013
        ''' Modified by: XB - 26/06/2013 v2.1.0 - Display local time (Bugstracking #1211)
        ''' </remarks>
        <Extension()> _
        Public Function ToXSDString(ByVal v As DateTime) As String
            ' XB - 26/06/2013 v2.1.0 - Display local time (Bugstracking #1211)
            'Return v.ToUniversalTime().ToString("s")
            Return v.ToLocalTime().ToString("s")
            ' XB - 26/06/2013 v2.1.0 - Display local time (Bugstracking #1211)
        End Function

    End Module

End Namespace
