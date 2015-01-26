Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Security.Cryptography

Namespace Biosystems.Ax00.Global.Security
    ''' <summary>
    ''' This Class implements the methods needed to encrypt/decrypt Users' Passwords.
    ''' It uses the version of the Rijndael Algorithm implemented by .NET, which is an 
    ''' standard symmetric iterated block cipher.
    ''' </summary>
    ''' <remarks>
    ''' Symmetric encryption: it is a traditional way of encrypting (also called Private Key Encryption), 
    ''' where the encryption and decryption keys are the same. It is divided into two groups: 
    '''    Block ciphers: they take a number of bytes and encrypt them as a single unit. 
    '''    Stream ciphers: they encrypt the bytes of the message one at a time.
    ''' In Rijndael Algorithm, the block and key lengths can be 128, 192, or 256 bits. 
    ''' </remarks>
    Public Class Security
        Inherits GlobalBase


#Region "Declarations"

        'Variables required to generate the Symmetric Key needed to encrypt/decrypt Users' Passwords 
        Private passPhrase As String        'Password used to derive the key
        Private saltValue As String         'Key salt used to derive the key
        Private initVector As String        'Init Vector (IV) to be used for the Symmetric Algorithm
        Private keySize As Integer          'Size (in Bits) of the secret Key used for the Symmetric Algorithm 

#End Region

#Region "Constructor"

        Public Sub New()
            'Initialization of variables required to generate the Symmetric Key needed to 
            'encrypt/decrypt Users' Passwords
            passPhrase = "AX00UserSoftware"
            saltValue = "AX00UserSoftware"
            initVector = "@8A7b6C5d4E3f2G1"
            keySize = 128
        End Sub

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Public function used to encrypt a plain text 
        ''' </summary>
        ''' <param name="PlainText">Non-encrypted Text</param>
        ''' <returns>An String containing the encrypted Text</returns>
        ''' <remarks></remarks>
        Public Function Encryption(ByVal PlainText As String) As String
            Dim encryptedText As String = ""   'To return the encrypted Text

            Try
                If (Trim(PlainText) <> "") Then
                    'Encrypt the plain text to an in-memory buffer
                    encryptedText = EncryptTextToMemory(PlainText)
                End If

                'Return the string containing the encrypted or an empty string
                Return encryptedText

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "Security.Encryption", EventLogEntryType.Error, GetSessionInfo.ActivateSystemLog)
            End Try
            Return encryptedText
        End Function

        ''' <summary>
        ''' Public function used to decrypt a cipher Text
        ''' </summary>
        ''' <param name="EncryptedText">An String containing the encrypted Text</param>
        ''' <returns>An String containing the plain text</returns>
        ''' <remarks></remarks>
        Public Function Decryption(ByVal EncryptedText As String) As String
            Dim plainText As String = ""
            Try
                If (Trim(EncryptedText) <> "") Then
                    'Decrypt the cipher Text from an in-memory buffer
                    plainText = DecryptTextFromMemory(EncryptedText)
                End If

                'Return the string containing the plain text or an empty string
                Return plainText

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "Security.Decryption", EventLogEntryType.Error, GetSessionInfo.ActivateSystemLog)
            End Try
            Return plainText

        End Function

#End Region

#Region "Private Methods"

        ''' <summary>
        ''' Private function that encrypt a plain text using Rijndael Symmetric Algorithm 
        ''' </summary>
        ''' <param name="PlainText">Non-encrypted Text</param>
        ''' <returns>An String containing the encrypted Text </returns>
        ''' <remarks></remarks>
        Private Function EncryptTextToMemory(ByVal PlainText As String) As String
            Dim encryptedTest As String = ""  'To return the encrypted Password

            Try
                'Get Bytes from the plain text Password
                Dim plainTextBytes As Byte() = New ASCIIEncoding().GetBytes(PlainText)

                'Get Bytes from the global variables required to generate the Symmetric Key
                Dim initialVectorBytes As Byte() = New ASCIIEncoding().GetBytes(initVector)
                Dim saltValueBytes As Byte() = New ASCIIEncoding().GetBytes(saltValue)

                'Produce a Text-base derived key from the global base key (passPhrase) and a salt value (saltValueBytes)
                'Get Bytes from the derived key according the defined key size
                Dim TextKey As New Rfc2898DeriveBytes(passPhrase, saltValueBytes)
                Dim TextKeyBytes As Byte() = TextKey.GetBytes(CInt(keySize / 8))

                'Create the Memory Stream, the Rijndael Encryptor and the Cryptographic Stream, 
                'filling the Memory Stream with the Encrypted Text 
                Dim memStream As New MemoryStream
                Dim rijndaelEncryptor As New RijndaelManaged()
                Dim crypStream As New CryptoStream(memStream, _
                                                   rijndaelEncryptor.CreateEncryptor(TextKeyBytes, initialVectorBytes), _
                                                   CryptoStreamMode.Write)
                crypStream.Write(plainTextBytes, 0, plainTextBytes.Length)
                crypStream.FlushFinalBlock()

                'Get the encrypted Password (in Bytes) from the Memory Stream and convert it to an String
                encryptedTest = Convert.ToBase64String(memStream.ToArray())

                'Close the used Streams
                crypStream.Close()
                memStream.Close()

            Catch ex As CryptographicException
                GlobalBase.CreateLogActivity(ex.Message, "Security.EncryptTextToMemory", EventLogEntryType.Error, GetSessionInfo.ActivateSystemLog)
            End Try
            Return encryptedTest
        End Function

        ''' <summary>
        ''' Private function that decrypt a cipher using Rijndael Symmetric Algorithm
        ''' </summary>
        ''' <param name="EncryptedText">An String containing the encrypted Text</param>
        ''' <returns>An String containing the plain text</returns>
        ''' <remarks></remarks>
        Private Function DecryptTextFromMemory(ByVal EncryptedText As String) As String
            Dim plainText As String = ""  'To return the plain text Password

            Try
                'Get Bytes from the string containing the encrypted Text and create
                'the array of Bytes to store the Password plain text read from the Cryptographic Stream
                Dim encryptedTextBytes As Byte() = Convert.FromBase64String(EncryptedText)
                Dim plainTextBytes As Byte()
                ReDim plainTextBytes(encryptedTextBytes.Length)

                'Get Bytes from the global variables required to generate the Symmetric Key
                Dim initialVectorBytes As Byte() = New ASCIIEncoding().GetBytes(initVector)
                Dim saltValueBytes As Byte() = New ASCIIEncoding().GetBytes(saltValue)

                'Produce a password-based derived key from the global base key (passPhrase) and a salt value (saltValueBytes)
                'Get Bytes from the derived key according the defined key size
                Dim passwordKey As New Rfc2898DeriveBytes(passPhrase, saltValueBytes)
                Dim passwordKeyBytes As Byte() = passwordKey.GetBytes(CInt(keySize / 8))

                'Create the Memory Stream and fill it with all bytes of the encrypted Text
                Dim memStream As New MemoryStream(encryptedTextBytes)

                'Create the Rijndael Decryptor and the Cryptographic Stream, and read the plain text
                Dim rijndaelDecryptor As New RijndaelManaged()
                Dim crypStream As New CryptoStream(memStream, _
                                                   rijndaelDecryptor.CreateDecryptor(passwordKeyBytes, initialVectorBytes), _
                                                   CryptoStreamMode.Read)
                crypStream.Read(plainTextBytes, 0, plainTextBytes.Length)

                'Close the used Streams
                crypStream.Close()
                memStream.Close()

                'Get the plain text in a String from the array of Bytes read from 
                'the stream and return it
                plainText = New ASCIIEncoding().GetString(plainTextBytes)

            Catch ex As CryptographicException
                GlobalBase.CreateLogActivity(ex.Message, "Security.DecryptTextFromMemory", EventLogEntryType.Error, GetSessionInfo.ActivateSystemLog)
            End Try
            Return plainText
        End Function

#End Region

    End Class
End Namespace