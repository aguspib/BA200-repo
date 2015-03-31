Imports System.IO
Imports System.Text
Imports System.Security

Module BatchWorker

    Private Const CryptKey As String = "Ax00Bios"  '// Key de encriptacion (deben ser 8 chars pq sino falla el algoritmo que usamos)
    Private _fileToEncrypt As String
    Private _fileEncrypted As String

    Sub Main()
        Dim appDirect = AppDomain.CurrentDomain.BaseDirectory()
        _fileEncrypted = appDirect.Replace("SolutionTools\bin\Debug\", "PresentationCOM\Update\Task\TaskList.xml")
        _fileToEncrypt = appDirect.Replace("SolutionTools\bin\Debug\", "PresentationCOM\Update\Task\TaskListDecrypted.xml")
        EncryptFile(_fileToEncrypt, _fileEncrypted)

        Dim piStart = New ProcessStartInfo(appDirect.Replace("SolutionTools\bin\Debug\", "Analyzer\CommAx00.exe"), " /REGSERVER")
        piStart.CreateNoWindow = True
        Process.Start(piStart)
    End Sub

    ''' <summary>
    ''' Encrypt file. It is inherited of iPRO 
    ''' </summary>
    ''' <param name="decryptFile"></param>
    ''' <param name="cryptfile"></param>
    ''' <remarks>
    ''' Se copia de: http://support.microsoft.com/kb/301070
    ''' </remarks>
    Public Sub EncryptFile(ByVal decryptFile As String, ByVal cryptfile As String)
        Dim fsInput As FileStream = Nothing
        Dim fsEncrypted As FileStream = Nothing
        Try
            If File.Exists(decryptFile) Then
                fsInput = New FileStream(decryptFile, FileMode.Open, FileAccess.Read)

                fsEncrypted = New FileStream(Cryptfile, FileMode.Create, FileAccess.Write)

                Dim des As New Cryptography.DESCryptoServiceProvider()
                des.CreateDecryptor()
                des.Key = ASCIIEncoding.ASCII.GetBytes(CryptKey)
                des.IV = ASCIIEncoding.ASCII.GetBytes(CryptKey)

                Dim cryptostream As New Cryptography.CryptoStream(fsEncrypted, des.CreateEncryptor, Cryptography.CryptoStreamMode.Write)
                Dim bytearrayinput(CInt(fsInput.Length - 1)) As Byte
                fsInput.Read(bytearrayinput, 0, bytearrayinput.Length)
                cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length)

                cryptostream.Close()
                fsInput.Close()
                fsEncrypted.Close()
            Else
                Throw New FileNotFoundException
            End If
        Catch ex As Exception
            fsInput.Close()
            fsEncrypted.Close()
        End Try
    End Sub

End Module
