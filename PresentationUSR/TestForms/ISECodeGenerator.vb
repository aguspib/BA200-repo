Option Explicit On
'Option Strict On

Imports System
' For Missing.Value and BindingFlags
' For COMException
''''''
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.CommunicationsSwFw
'Imports History.Biosystems.Ax00.BL


Public Class ISECodeGenerator
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

    Private myValidNumbers As New List(Of String)
    Private ValidNumbers() As String

    Private SelectedDSND00 As String = ""

    Private ReadOnly Property SelectedDSN As String
        Get
            Dim res As String = ""
            If SelectedDSND00.Length > 0 Then
                res = SelectedDSND00.Substring(0, 23)
            End If
            Return res
        End Get
    End Property

    Private ReadOnly Property SelectedD00 As String
        Get
            Dim res As String = ""
            If SelectedDSND00.Length > 0 Then
                res = SelectedDSND00.Substring(23)
            End If
            Return res
        End Get
    End Property

    Private Property myUtil As Object

    Private Function GetDallasSNValues(ByVal pDataStr As String) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO
        Dim myUtil As New Utilities
        Dim myDallasSNData As New ISEDallasSNTO

        Try

            Dim mySerialNumber As String = pDataStr.Trim.Substring(5, 16)
            Dim myCRCHex As String = pDataStr.Trim.Substring(21, 1)

            With myDallasSNData
                .SNDataString = pDataStr.Trim

                'SerialID 
                .SerialNumber = mySerialNumber

                'CRC Tester
                .CRC = myCRCHex

            End With

        Catch ex As Exception
            myDallasSNData.ValidationError = True

            myGlobal.HasError = True
            myGlobal.ErrorCode = "SYSTEM_ERROR"
            myGlobal.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, Me.Name & ".GetDallasSNValues", EventLogEntryType.Error, False)
        End Try

        myGlobal.SetDatos = myDallasSNData
        Return myGlobal

    End Function

    Private Function GetDallasPage00Values(ByVal pDataStr As String) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO
        Dim myUtil As New Utilities
        Dim myDallas00Data As New ISEDallasPage00TO

        Try

            Dim isError As Boolean
            Dim mySecCodeByte3Hex As String = pDataStr.Trim.Substring(8, 2)
            Dim mySecCodeByte0Hex As String = pDataStr.Trim.Substring(10, 2)
            Dim myLotNumberHex As String = pDataStr.Trim.Substring(24, 10)
            Dim myDistributorCodeHex As String = pDataStr.Trim.Substring(46, 2)
            Dim myInitialCalibAVolumeHex As String = pDataStr.Trim.Substring(56, 2)
            Dim mySecCodeByte2Hex As String = pDataStr.Trim.Substring(58, 2)
            Dim mySecCodeByte1Hex As String = pDataStr.Trim.Substring(60, 2)
            Dim myExpirationMonthHex As String = pDataStr.Trim.Substring(62, 2)
            Dim myExpirationYearHex As String = pDataStr.Trim.Substring(64, 2)
            Dim myExpirationDayHex As String = pDataStr.Trim.Substring(66, 2)
            Dim myInitialCalibBVolumeHex As String = pDataStr.Trim.Substring(68, 2)
            Dim myCRCHex As String = pDataStr.Trim.Substring(70, 2)

            With myDallas00Data
                .Page00DataString = pDataStr.Trim

                'LotNumber
                myGlobal = myUtil.ConvertHexToUInt32(myLotNumberHex)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    .LotNumber = CInt(myGlobal.SetDatos).ToString
                Else
                    isError = isError Or True
                End If


                'ExpirationDay
                myGlobal = myUtil.ConvertHexToUInt32(myExpirationDayHex)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    .ExpirationDay = CInt(myGlobal.SetDatos)
                Else
                    isError = isError Or True
                End If

                'ExpirationMonth
                myGlobal = myUtil.ConvertHexToUInt32(myExpirationMonthHex)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    .ExpirationMonth = CInt(myGlobal.SetDatos)
                Else
                    isError = isError Or True
                End If

                'ExpirationYear
                myGlobal = myUtil.ConvertHexToUInt32(myExpirationYearHex)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    .ExpirationYear = 2000 + CInt(myGlobal.SetDatos)
                Else
                    isError = isError Or True
                End If

                '.ExpirationDay = CInt(myExpirationDayHex)
                '.ExpirationMonth = CInt(myExpirationMonthHex)
                '.ExpirationYear = 2000 + CInt(myExpirationYearHex)

                'InitialCalibAVolume (mililitres)
                myGlobal = myUtil.ConvertHexToUInt32(myInitialCalibAVolumeHex)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    .InitialCalibAVolume = 10 * CInt(myGlobal.SetDatos)
                Else
                    isError = isError Or True
                End If

                'InitialCalibBVolume (mililitres)
                myGlobal = myUtil.ConvertHexToUInt32(myInitialCalibBVolumeHex)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    .InitialCalibBVolume = 10 * CInt(myGlobal.SetDatos)
                Else
                    isError = isError Or True
                End If

                'DistributorCode 
                myGlobal = myUtil.ConvertHexToUInt32(myDistributorCodeHex)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    .DistributorCode = CStr(myGlobal.SetDatos)
                Else
                    isError = isError Or True
                End If


                'SecurityCode 
                .SecurityCode = mySecCodeByte3Hex & mySecCodeByte2Hex & mySecCodeByte1Hex & mySecCodeByte0Hex

                'CRC
                .CRC = myCRCHex

                .ValidationError = isError Or .ValidationError

            End With



        Catch ex As Exception
            myDallas00Data.ValidationError = True

            myGlobal.HasError = True
            myGlobal.ErrorCode = "SYSTEM_ERROR"
            myGlobal.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, Me.Name & ".GetDallasPage00Values", EventLogEntryType.Error, False)
        End Try

        myGlobal.SetDatos = myDallas00Data
        Return myGlobal

    End Function




    Private Function GenerateBiosystemsCode(ByVal pSerialNumber As String) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO
        Dim myUtil As New Utilities
        Dim myFinalHex As String = ""

        Try



            'Se recibe un número hexadecimal de 8 cifras (32bits)
            If pSerialNumber.Length = 8 Then

                'Se convierte a formato binario
                myGlobal = myUtil.ConvertHexToBinaryString(pSerialNumber)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    Dim myBinary1 As String = CType(myGlobal.SetDatos, String)

                    'Se separa en dos partes MSB y LSB para generar el número
                    'binario del que se obtuvieron. MSB (bits pares) y LSB(bits impares)
                    Dim myMSB As String = myBinary1.Substring(0, 16)
                    Dim myLSB As String = myBinary1.Substring(16, 16)
                    Dim myBinary2 As String = ""
                    Dim bb As Integer = 0
                    For b As Integer = 0 To 31 Step 1
                        If b Mod 2 = 0 Then 'even
                            myBinary2 &= myMSB(bb)
                        Else 'odd
                            myBinary2 &= myLSB(bb)
                            bb += 1
                        End If
                    Next

                    'Se convierte el resultado binario a un hexadecimal
                    myGlobal = myUtil.ConvertBinaryStringToDecimal(myBinary2)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        Dim myDecimal As UInt32 = CType(myGlobal.SetDatos, UInt32)

                        'Se convierte el resultado an Uint32
                        myGlobal = myUtil.ConvertDecimalToHex(myDecimal)
                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                            Dim myHexadecimalLow As String = CStr(myGlobal.SetDatos)
                            'Se añaden los ceros que puedan faltar para llegar a 8 cifras
                            Dim dif8 As Integer = 8 - myHexadecimalLow.Length
                            For c As Integer = 1 To dif8 Step 1
                                myHexadecimalLow = "0" & myHexadecimalLow
                            Next

                            'se debe encontrar un número hexadecimal que añadiendolo al que ya se tiene, la raiz del total sea un entero

                            'se debe encontrar la parte alta del número hexadecimal del que ya sabemos que su raiz es entera
                            Dim myHexadecimalValue As String = ""
                            For Each V As String In myValidNumbers
                                Dim myLow As String = V.Substring(8, 8)
                                If myLow = myHexadecimalLow Then
                                    myHexadecimalValue = V
                                    Exit For
                                End If
                                'If V.EndsWith(myHexadecimalLow) Then
                                '    myHexadecimalValue = V
                                '    Exit For
                                'End If
                            Next

                            If myHexadecimalValue.Length > 0 Then
                                'Se convierte a decimal 64 bits y se realiza la raiz cuadrada
                                Dim myRoot As Double
                                myGlobal = myUtil.ConvertHexToUInt64(myHexadecimalValue)
                                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                                    Dim myDecimalTotal As UInt64 = CType(myGlobal.SetDatos, UInt64)
                                    myRoot = Math.Sqrt(myDecimalTotal)
                                End If

                                'Se convierte el resultado de la raiz a a hexadecimal 32 bits
                                myGlobal = myUtil.ConvertDecimalToHex(CInt(myRoot))
                                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                                    myFinalHex = CType(myGlobal.SetDatos, String)

                                    'Se añaden los ceros que puedan faltar para llegar a 8 cifras
                                    dif8 = 8 - myFinalHex.Length
                                    For c As Integer = 1 To dif8 Step 1
                                        myFinalHex = "0" & myFinalHex
                                    Next

                                    If myFinalHex.Length = 8 Then

                                        Dim myByte0 As String = myFinalHex.Substring(0, 2)
                                        Dim myByte1 As String = myFinalHex.Substring(2, 2)
                                        Dim myByte2 As String = myFinalHex.Substring(4, 2)
                                        Dim myByte3 As String = myFinalHex.Substring(6, 2)

                                        myFinalHex = myByte3 & myByte2 & myByte1 & myByte0

                                        myGlobal.SetDatos = myFinalHex
                                    End If

                                End If
                            End If
                        End If
                    End If
                End If
            End If

            myGlobal.SetDatos = myFinalHex

        Catch ex As Exception
            myGlobal.SetDatos = False
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, Me.Name & ".GenerateBiosystemsCode", EventLogEntryType.Error, False)
        End Try
        Return myGlobal
    End Function

    Private Function GenerateDSNDDT00(ByVal pValue As UInt32, Optional ByVal FindAll As Boolean = False) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Dim myUtil As New Utilities

        Try
            Dim myD00CodeBytesHex As String = "" 'input
            Dim myDSNCodeBytesHex As String = "" 'output

            Dim attempts As Integer = 0
            Dim maxAttempts As Integer = 100

            If FindAll Then
                maxAttempts = 1
            Else
                Me.BsProgressBar.Maximum = 100
                Me.BsProgressBar.Visible = True
            End If


            Do
                If Not FindAll Then
                    Dim myRandomValue As UInt32 = Convert.ToUInt32(Math.Ceiling(Rnd(Now.Second) * 4102))
                    pValue = myRandomValue
                End If

                myGlobal = myUtil.ConvertDecimalToHex(pValue)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    myD00CodeBytesHex = CType(myGlobal.SetDatos, String)
                    Dim dif8 As Integer = 8 - myD00CodeBytesHex.Length
                    For c As Integer = 1 To dif8 Step 1
                        myD00CodeBytesHex = "0" & myD00CodeBytesHex
                    Next

                    myGlobal = MyClass.GenerateBiosystemsCode(myD00CodeBytesHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        myDSNCodeBytesHex = CStr(myGlobal.SetDatos)
                    End If
                End If

                attempts += 1

                If attempts >= maxAttempts Then
                    Exit Do
                End If

                Me.BsProgressBar.Value = attempts

            Loop While Not myDSNCodeBytesHex.Length > 0


            If myDSNCodeBytesHex.Length > 0 Then
                'construir DSN y DDT 00

                '<DSN 0960E4F20600006De>
                '<DSN 09********00006De>
                Dim myDSN As String = "<DSN 09" & myDSNCodeBytesHex & "00006De>"


                Dim Byte0 As String = myD00CodeBytesHex.Trim.Substring(6, 2)
                Dim Byte1 As String = myD00CodeBytesHex.Trim.Substring(4, 2)
                Dim Byte2 As String = myD00CodeBytesHex.Trim.Substring(2, 2)
                Dim Byte3 As String = myD00CodeBytesHex.Trim.Substring(0, 2)


                '<DDT 00 072000640029057800009A48EA02BC019003208204E2019A344055080E1F13F1Ô>
                '<DDT 00 ****00640029057800009A48EA02BC019003208204E2019A34****080E1F13F1Ô>
                Dim myDDT00 As String = "<DDT 00 " & Byte3 & Byte0 & "00640029057800009A48EA02BC019003208204E2019A34" & Byte2 & Byte1 & "080E1F13F1Ô>"

                myGlobal.SetDatos = myDSN & myDDT00

            End If


        Catch ex As Exception
            myGlobal.SetDatos = False
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, Me.Name & ".GenerateBiosystemsCode", EventLogEntryType.Error, False)
        End Try
        Return myGlobal
    End Function


    Private Sub BsPrepareButton_Click(sender As Object, e As EventArgs) Handles BsPrepareButton.Click

        Dim myGlobal As New GlobalDataTO
        Dim myUtil As New Utilities

        Try

            Me.Enabled = False
            Me.Cursor = Cursors.WaitCursor
            Me.BsPrepareButton.Enabled = False

            Me.BsProgressBar.Maximum = 16842751UI
            Me.BsProgressBar.Visible = True

            'Generar valores de 64 bits cuya raiz cuadrada es entera
            For N As UInt32 = 0 To 16842751UI Step 1

                Me.BsProgressBar.Value = N

                Dim myPow2 As UInt64
                Try
                    myPow2 = Math.Pow(16842751UI - N, 2)
                Catch ex As Exception
                    Exit For
                End Try

                myGlobal = myUtil.ConvertUint64ToHex(myPow2)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    Dim myValidHex As String = CType(myGlobal.SetDatos, String)
                    Dim dif16 As Integer = 16 - myValidHex.Length
                    For c As Integer = 1 To dif16 Step 1
                        myValidHex = "0" & myValidHex
                    Next
                    Try
                        myValidNumbers.Add(myValidHex)
                    Catch ex As Exception
                        Exit For
                    End Try

                End If

            Next N

            Me.BsProgressBar.Visible = False

            Me.Enabled = True
            Me.Cursor = Cursors.Default

            If myValidNumbers.Count > 0 Then
                Me.BsGenerateButton.Enabled = True
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub


    Private Sub BsGenerateButton_Click(sender As Object, e As EventArgs) Handles BsGenerateButton.Click

        Dim myGlobal As New GlobalDataTO

        Try
            Me.Enabled = False
            Me.BsGenerateButton.Enabled = False
            Me.Cursor = Cursors.WaitCursor


            If Not Me.BsAllCheckbox.Checked Then


                myGlobal = MyClass.GenerateDSNDDT00(0)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    Me.BsCodeTextBox.Text = CStr(myGlobal.SetDatos)
                    MyClass.SelectedDSND00 = Me.BsCodeTextBox.Text

                    If (Not Me.BsCodesTextBox.Text.Contains(MyClass.SelectedDSN)) Then
                        Me.BsCodesTextBox.Text &= vbCrLf & CStr(myGlobal.SetDatos)
                    End If
                End If


            Else
                Me.BsProgressBar.Maximum = 4102
                Me.BsProgressBar.Visible = True
                For N As Integer = 1 To 4102 Step 1
                    Me.BsProgressBar.Value = N
                    myGlobal = MyClass.GenerateDSNDDT00(N, True)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        Me.BsCodeTextBox.Text = CStr(myGlobal.SetDatos)
                        MyClass.SelectedDSND00 = Me.BsCodeTextBox.Text

                        If (Not Me.BsCodesTextBox.Text.Contains(MyClass.SelectedDSN)) Then
                            Me.BsCodesTextBox.Text &= vbCrLf & CStr(myGlobal.SetDatos)
                        End If
                    End If
                Next

            End If


        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

        Me.BsProgressBar.Visible = False
        Me.Enabled = True
        Me.BsGenerateButton.Enabled = True
        Me.Cursor = Cursors.Default

    End Sub

    Private Sub BsCheckButton_Click(sender As Object, e As EventArgs) Handles BsCheckButton.Click
        Try
            Dim myISEDallasSN As New ISEDallasSNTO
            Dim myDallas00 As New ISEDallasPage00TO

            Dim result As Boolean = False
            Dim myGlobal As New GlobalDataTO

            If Me.BsValidateAllCheckbox.Checked Then
                Dim myCodesOK As New List(Of String)
                Dim myCodesERROR As New List(Of String)
                Dim codes() As String = Me.BsCodesTextBox.Text.Split(vbCrLf)
                For c As Integer = 0 To codes.Length - 1
                    MyClass.SelectedDSND00 = codes(c).Trim
                    If MyClass.SelectedDSND00.Length > 0 Then
                        myGlobal = MyClass.GetDallasSNValues(MyClass.SelectedDSN)
                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                            myISEDallasSN = CType(myGlobal.SetDatos, ISEDallasSNTO)
                            myGlobal = MyClass.GetDallasPage00Values(MyClass.SelectedD00)
                            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                                myDallas00 = CType(myGlobal.SetDatos, ISEDallasPage00TO)
                            End If
                        End If
                        myGlobal = ISEManager.BiosystemsValidationAlgorithm(myISEDallasSN, myDallas00)
                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                            result = CBool(myGlobal.SetDatos)
                        End If

                        If result Then
                            myCodesOK.Add(MyClass.SelectedDSND00)
                        Else
                            myCodesERROR.Add(MyClass.SelectedDSND00)
                        End If
                    End If
                Next

                Dim myMessage As String = ""
                For Each C As String In myCodesOK
                    myMessage &= C + vbCrLf
                Next
                Me.BsCodesOKTextBox.Text = myMessage

                myMessage = ""
                For Each C As String In myCodesERROR
                    myMessage &= C + vbCrLf
                Next
                Me.BsCodesErrorTextBox.Text = myMessage

            Else

                MyClass.SelectedDSND00 = Me.BsCodeTextBox.Text

                If MyClass.SelectedDSND00.Length > 0 Then
                    myGlobal = MyClass.GetDallasSNValues(MyClass.SelectedDSN)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        myISEDallasSN = CType(myGlobal.SetDatos, ISEDallasSNTO)
                        myGlobal = MyClass.GetDallasPage00Values(MyClass.SelectedD00)
                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                            myDallas00 = CType(myGlobal.SetDatos, ISEDallasPage00TO)
                        End If
                    End If
                    myGlobal = ISEManager.BiosystemsValidationAlgorithm(myISEDallasSN, myDallas00)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        result = CBool(myGlobal.SetDatos)
                    End If

                    If result Then
                        MessageBox.Show("ISE code validation OK")
                    Else
                        MessageBox.Show("ISE code validation ERROR")
                    End If
                End If

            End If

            If myGlobal.HasError Then
                MessageBox.Show("ISE code validation ERROR")
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub


















    'FOR TESTING
    Private Function ConvertDatetimeToString(ByVal pDateTime As DateTime) As String
        Dim res As String = ""
        Try
            If pDateTime <> Nothing Then
                res = pDateTime.Day.ToString("#00") & pDateTime.Month.ToString("#00") & pDateTime.Year.ToString("#00") & pDateTime.Hour.ToString("#00") & pDateTime.Minute.ToString("#00") & pDateTime.Second.ToString("#00") & pDateTime.Millisecond.ToString("#000")
            End If
        Catch ex As Exception
            res = ""
        End Try
        Return res
    End Function

    Private Function ConvertStringToDatetime(ByVal pString As String) As DateTime
        Dim res As DateTime
        Try
            If (pString.Length = 15) Then
                Dim Day As Short = pString.Substring(0, 2)
                Dim Month As Short = pString.Substring(2, 2)
                Dim Year As Short = pString.Substring(4, 2)
                Dim Hour As Short = pString.Substring(6, 2)
                Dim Minute As Short = pString.Substring(8, 2)
                Dim Second As Short = pString.Substring(10, 2)
                Dim MiliSec As Short = pString.Substring(12, 3)

                res = New DateTime(Year, Month, Day, Hour, Minute, Second, MiliSec)

            End If
        Catch ex As Exception
            res = Nothing
        End Try
        Return res
    End Function
End Class


