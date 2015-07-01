Imports System.IO
Imports System.Text
Imports System.Security
Imports System.Windows.Forms
Imports Biosystems.Ax00.Core.Entities.UpdateVersion

Module PreBuildBatch

    Private Const CryptKey As String = "Ax00Bios"  '// Key de encriptacion (deben ser 8 chars pq sino falla el algoritmo que usamos)
    Private _fileToEncrypt As String
    Private _fileEncrypted As String

    Sub Main()

        Try

            If Not GenerateEncryptedTaskList() Then
                Throw New Exception("There was an error generating the TaskList file.")
            End If

            If Not GenerateEncryptedFactoryFwScripts() Then
                Throw New Exception("There was an error generating the FwFactoryScriptsData file.")
            End If

            'Dim appDirect = AppDomain.CurrentDomain.BaseDirectory()
            'Dim piStart = New ProcessStartInfo(appDirect + "CommAx00.exe", " /REGSERVER")
            'piStart.CreateNoWindow = True
            'Process.Start(piStart)

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Important Message")
        End Try

    End Sub

    ''' <summary>
    ''' Encrypt file. It is inherited of iPRO 
    ''' </summary>
    ''' <param name="decryptFile"></param>
    ''' <param name="cryptfile"></param>
    ''' <remarks>
    ''' Se copia de: http://support.microsoft.com/kb/301070
    ''' </remarks>
    Private Sub EncryptFile(ByVal decryptFile As String, ByVal cryptfile As String)
        Dim fsInput As FileStream = Nothing
        Dim fsEncrypted As FileStream = Nothing
        Try
            If File.Exists(decryptFile) Then
                fsInput = New FileStream(decryptFile, FileMode.Open, FileAccess.Read)

                fsEncrypted = New FileStream(cryptfile, FileMode.Create, FileAccess.Write)

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

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GenerateEncryptedTaskList() As Boolean

        Dim commonTaskList As String
        Dim dataTaskList As String

        Dim encryptedTaskList As String
        Dim decryptedTaskList As String

        Dim appDirect = AppDomain.CurrentDomain.BaseDirectory()
        Dim appPath = Application.ExecutablePath

        commonTaskList = appDirect.Replace("Tools\", My.Settings.TaskListCommon)
        dataTaskList = appDirect.Replace("Tools\", My.Settings.TaskListData)

        decryptedTaskList = appDirect.Replace("Tools\", My.Settings.TaskListDecrypted)
        encryptedTaskList = appDirect.Replace("Tools\", My.Settings.TaskListEncrypted)

        Dim commonRepository = DatabaseUpdatesManager.Deserialize(commonTaskList)
        Dim dataRepository = DatabaseUpdatesManager.Deserialize(dataTaskList)

        commonRepository.Merge(dataRepository)

        If commonRepository.Validate() Then
            commonRepository.Serialize(decryptedTaskList)
            EncryptFile(decryptedTaskList, encryptedTaskList)
        Else
            Return False
        End If

        Return True

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GenerateEncryptedFactoryFwScripts() As Boolean

        Try

            Dim decryptedFactoryFwScriptsData As String
            Dim encryptedFactoryFwScriptsData As String

            Dim appDirect = AppDomain.CurrentDomain.BaseDirectory()
            Dim appPath = Application.ExecutablePath

            decryptedFactoryFwScriptsData = appDirect.Replace("Tools\", My.Settings.FactoryFwScriptsDataDecrypted)
            encryptedFactoryFwScriptsData = appDirect.Replace("Tools\", My.Settings.FactoryFwScriptsDataEncrypted)

            EncryptFile(decryptedFactoryFwScriptsData, encryptedFactoryFwScriptsData)

            Return True

        Catch ex As Exception
            Return False
        End Try

    End Function


End Module
