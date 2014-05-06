'AG 22/02/2013 Creation (Based on SysteLab demo code xmlHelper.vb class)
'   - Location LIS project, folder XMLUtils
'
'AG 13/03/2013 Moved to Global project because the WorkSession project also requires this class

Imports System.Xml
Imports System.Xml.XPath
Imports System.IO
Imports System.Xml.Schema


Namespace Biosystems.Ax00.Global

    Partial Public Class xmlHelper
        Private NamespaceManager As XmlNamespaceManager
        Public ValidationError As String = Nothing

        '***************
        ' Constructor 
        '***************

        Public Sub New(prefix1 As String, uri1 As String, prefix2 As String, uri2 As String)

            Me.Initialize(prefix1, uri1, prefix2, uri2)

        End Sub

        '***************
        ' METHODS 
        '***************

        ' Public 

        Public Function QueryXmlNode(currentNode As XmlNode, xpath As String) As XmlNode

            Dim node As XmlNode = Nothing
            If (Not currentNode Is Nothing) Then
                node = currentNode.SelectSingleNode(xpath, NamespaceManager)
            End If

            Return node

        End Function

        Public Function QueryXmlNodeList(currentNode As XmlNode, xpath As String) As XmlNodeList

            Dim nodeList As XmlNodeList = Nothing
            If (Not currentNode Is Nothing) Then
                nodeList = currentNode.SelectNodes(xpath, NamespaceManager)
            End If

            Return nodeList

        End Function

        Public Function TryQueryStringValue(currentNode As XmlNode, xpath As String) As String

            Dim value As String = QueryStringValue(currentNode, xpath, Nothing)
            If (Not value Is Nothing) Then
                Return value
            Else
                'AG 18/03/2013
                'Return Nothing
                Return ""
            End If

        End Function

        Public Function QueryStringValue(currentNode As XmlNode, xpath As String) As String

            Dim value As String = QueryStringValue(currentNode, xpath, Nothing)
            If (Not value Is Nothing) Then
                Return value
            Else
                Throw New XmlException("XmlHelper query failed. No XML node matches mandatory XPath query: " & xpath)
            End If

        End Function

        Public Function QueryStringValue(currentNode As XmlNode, xpath As String, defaultValue As String) As String

            Dim resultNode As XmlNode = Nothing
            If (Not currentNode Is Nothing) Then
                resultNode = currentNode.SelectSingleNode(xpath, NamespaceManager)
            End If

            If (Not resultNode Is Nothing) Then
                Return resultNode.InnerText
            Else
                Return defaultValue
            End If

        End Function

        Public Sub UpdateSingleNode(currentNode As XmlNode, xpath As String, newValue As String)

            If (Not currentNode Is Nothing) Then

                Dim resultNode As XmlNode = currentNode.SelectSingleNode(xpath, NamespaceManager)
                If (Not resultNode Is Nothing) Then
                    resultNode.InnerText = newValue
                Else
                    Throw New XmlException("XmlHelper update failed. Search pattern doesn't fit any node. XPath query: " & xpath)
                End If

            End If

        End Sub

        Public Function ValidateXml(messageText As String, schemaTextUDC As String, schemaTextCI As String) As Boolean

            ValidationError = Nothing

            Dim strAsBytesUDC() As Byte = New System.Text.UTF8Encoding().GetBytes(schemaTextUDC)
            Dim memoryStreamSchemaUDC As New System.IO.MemoryStream(strAsBytesUDC)

            Dim strAsBytesCI() As Byte = New System.Text.UTF8Encoding().GetBytes(schemaTextCI)
            Dim memoryStreamSchemaCI As New System.IO.MemoryStream(strAsBytesCI)

            Dim schemaUDC = XmlReader.Create(memoryStreamSchemaUDC)
            Dim schemaCI = XmlReader.Create(memoryStreamSchemaCI)

            Dim document As XmlDocument = New XmlDocument()
            document.LoadXml(messageText)
            document.Schemas.Add("http://www.nte.es/schema/clinical-information-v1.0", schemaCI)
            document.Schemas.Add("http://www.nte.es/schema/udc-interface-v1.0", schemaUDC)
            document.Validate(AddressOf MyValidationEventHandler)

            If (Me.ValidationError = Nothing) Then
                Return True
            Else
                Return False
            End If

        End Function

        ' Private

        Private Sub Initialize(prefix1 As String, uri1 As String, prefix2 As String, uri2 As String)

            NamespaceManager = New XmlNamespaceManager(New NameTable())

            If (Not prefix1 Is Nothing) Then
                NamespaceManager.AddNamespace(prefix1, uri1)
            End If

            If (Not prefix2 Is Nothing) Then
                NamespaceManager.AddNamespace(prefix2, uri2)
            End If

        End Sub

        '***************
        ' EVENTS
        '***************

        Private Sub MyValidationEventHandler(ByVal sender As Object, ByVal args As ValidationEventArgs)

            Me.ValidationError = args.Message

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="currentNode"></param>
        ''' <param name="attrName"></param>
        ''' <param name="xpath"></param>
        ''' <param name="defaultValue"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 27/02/2013</remarks>
        Public Function QueryAttributeStringValue(currentNode As XmlNode, attrName As String, xpath As String, defaultValue As String) As String
            Dim resultNode As XmlNode = Nothing
            If (Not currentNode Is Nothing) Then
                If xpath.Length > 0 Then
                    resultNode = currentNode.SelectSingleNode(xpath, NamespaceManager)
                Else
                    resultNode = currentNode
                End If
            End If

            If (Not resultNode Is Nothing) Then
                Dim resultAttr As XmlAttribute = resultNode.Attributes(attrName)
                If resultAttr IsNot Nothing Then
                    Return resultAttr.Value
                Else
                    Return defaultValue
                End If
            Else
                Return defaultValue
            End If

        End Function

        ''' <summary>
        ''' Encodes a string to a valid string for XML
        ''' </summary>
        ''' <param name="pString"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 05/03/2012</remarks>
        Private Function EncodeSpecialCharsForXML(ByVal pString As String) As String
            Dim res As String = ""
            Try
                Dim myChars() As Char = pString.ToCharArray
                For c As Integer = 0 To myChars.Length - 1 Step 1
                    Select Case myChars(c)
                        Case "&" : res &= "&#x26;"
                        Case "<" : res &= "&#x60;"
                        Case ">" : res &= "&#x62;"
                        Case Else : res &= myChars(c)
                    End Select
                Next

            Catch ex As Exception
                res = ""
            End Try
            Return res
        End Function

        ''' <summary>
        ''' Decodes a string from a valid string for XML
        ''' </summary>
        ''' <param name="pString"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 05/03/2012</remarks>
        Private Function DecodeSpecialCharsForXML(ByVal pString As String) As String
            Dim res As String = pString
            Try

                If pString.Contains("&#x26;") Then
                    res = res.Replace("&#x26;", "&")
                End If
                If pString.Contains("&#x60;") Then
                    res = res.Replace("&#x60;", "<")
                End If
                If pString.Contains("&#x26;") Then
                    res = res.Replace("&#x62;", ">")
                End If


            Catch ex As Exception
                res = ""
            End Try
            Return res
        End Function

    End Class


End Namespace
