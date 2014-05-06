Option Explicit On
Option Strict On


Imports System.Xml

Public Class AppConfigFileSetting

    ''' <summary>
    ''' UpdateAppSettings: It will update the app.Config file AppConfig key:values
    ''' </summary>
    ''' <param name="KeyName">AppConfigs KeyName</param>
    ''' <param name="KeyValue">AppConfigs KeyValue</param>
    ''' <remarks></remarks>
    Public Shared Sub UpdateAppSettings(ByVal KeyName As String, ByVal KeyValue As String)
        Try
            Dim XmlDoc As New XmlDocument() ' Xml document, to load AppConfig file.
            XmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile) 'load configuration File.
            For Each xElement As XmlElement In XmlDoc.DocumentElement
                If xElement.Name = "connectionStrings" Then 'Validate the nodes, to get the connectionString Element.
                    For Each xNode As XmlNode In xElement.ChildNodes
                        If xNode.Attributes(0).Value = KeyName Then ' change the the connection string name
                            xNode.Attributes(1).Value = KeyValue 'Change connection string value. 
                        End If
                    Next
                End If
            Next
            XmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile) 'save the new changes.
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
End Class
