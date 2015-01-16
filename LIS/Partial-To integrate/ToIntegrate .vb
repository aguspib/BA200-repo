'Class creation 22/02/2013 AG
'Based on SysteLab demo code MainForm.vb, EmbeddedSynapse.vb class,...


Imports Biosystems.Ax00.Global


Namespace Biosystems.Ax00.LISCommunications

    Partial Public Class ESxmlTranslator

        ''' <summary>
        ''' STL example of create channel xml -> for testing because all parameters are embedded in code
        ''' Instrument as Server
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pApplicationID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 11/03/2013</remarks>
        Public Function GetCreateChannelSTLServer(ByVal pDBConnection As SqlClient.SqlConnection, _
                                      ByVal pApplicationID As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                Dim channelxml As String = String.Empty

                'Create channel for HL7
                channelxml = _
                    "<channel xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='http://www.nte.es/schema/udc-interface-v1.0' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>" &
      "<registrarInformation>" &
        "<protocol>HL7LAW</protocol>" &
        "<role>provider</role>" &
        "<device>" &
          "<manufacturer>Biosystems</manufacturer>" &
          "<model>BA400</model>" &
          "<version>1.0</version>" &
        "</device>" &
      "</registrarInformation>" &
      "<parameter>" &
        "<id>udc:id</id>" &
        "<value xsi:type='EGA'>1</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:dataTransmission/@type</id>" &
        "<value xsi:type='EAABCA'>TCPIP-Server</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:highLevelProtocol/udc:maxTimeWaitingForACK</id>" &
        "<value xsi:type='EAGBAAGA'>10</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:highLevelProtocol/udc:maxTimeWaitingForResponse</id>" &
        "<value xsi:type='EAFBAAGA'>10</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:highLevelProtocol/udc:maxTimeToRespond</id>" &
        "<value xsi:type='EAEBAAGA'>10</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:highLevelProtocol/udc:translator/udc:xmlToHl7/udc:delimiters/@field</id>" &
        "<value xsi:type='EAEAAAAGBAAFA'>124</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:highLevelProtocol/udc:translator/udc:xmlToHl7/udc:delimiters/@component</id>" &
        "<value xsi:type='EADAAAAGBAAFA'>94</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:highLevelProtocol/udc:translator/udc:xmlToHl7/udc:delimiters/@subComponent</id>" &
        "<value xsi:type='EACAAAAGBAAFA'>38</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:highLevelProtocol/udc:translator/udc:xmlToHl7/udc:delimiters/@repeat</id>" &
        "<value xsi:type='EABAAAAGBAAFA'>126</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:highLevelProtocol/udc:translator/udc:xmlToHl7/udc:delimiters/@escape</id>" &
        "<value xsi:type='EAAAAAAGBAAFA'>92</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:highLevelProtocol/udc:translator/udc:sendingApplication</id>" &
        "<value xsi:type='EAFBAAFA'>BA400</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:highLevelProtocol/udc:translator/udc:sendingFacility</id>" &
        "<value xsi:type='EAEBAAFA'>BioSystems</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:highLevelProtocol/udc:translator/udc:receivingApplication</id>" &
        "<value xsi:type='EADBAAFA'>Modulab</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:highLevelProtocol/udc:translator/udc:receivingFacility</id>" &
        "<value xsi:type='EACBAAFA'>Systelab</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:highLevelProtocol/udc:translator/udc:codepage</id>" &
        "<value xsi:type='xsd:integer'>65001</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:dataTransmission[@type='TCPIP-Server']/udc:port</id>" &
        "<value xsi:type='xsd:unsignedShort'>11111</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:dataTransmission[@type='TCPIP-Server']/udc:host</id>" &
        "<value xsi:type='xsd:string'>" &
        "</value>" &
      "</parameter>" &
    "</channel>"

                'For ASTM
                'channelxml = My.Settings.ASTMServer.ToString

                resultData.SetDatos = channelxml


            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ESxmlTranslator.GetCreateChannelSTLServer", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' STL example of create channel xml -> for testing because all parameters are embedded in code
        ''' Instrument as Client
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pApplicationID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 11/03/2013</remarks>
        Public Function GetCreateChannelSTLClient(ByVal pDBConnection As SqlClient.SqlConnection, _
                                      ByVal pApplicationID As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                Dim channelxml As String = String.Empty

                channelxml = _
"<channel xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='http://www.nte.es/schema/udc-interface-v1.0' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>" &
      "<registrarInformation>" &
          "<protocol>HL7LAW</protocol>" &
          "<role>provider</role>" &
          "<device>" &
              "<manufacturer>Biosystems</manufacturer>" &
              "<model>BA400</model>" &
              "<version>1.0</version>" &
          "</device>" &
      "</registrarInformation>" &
      "<parameter>" &
          "<id>udc:id</id>" &
          "<value xsi:type='EGA'>1</value>" &
      "</parameter>" &
      "<parameter>" &
          "<id>udc:dataTransmission/@type</id>" &
          "<value xsi:type='EAABCA'>TCPIP-Client</value>" &
      "</parameter>" &
      "<parameter>" &
          "<id>udc:highLevelProtocol/udc:maxTimeWaitingForACK</id>" &
          "<value xsi:type='EAGBAAGA'>10</value>" &
      "</parameter>" &
      "<parameter>" &
          "<id>udc:highLevelProtocol/udc:maxTimeWaitingForResponse</id>" &
          "<value xsi:type='EAFBAAGA'>10</value>" &
      "</parameter>" &
      "<parameter>" &
          "<id>udc:highLevelProtocol/udc:maxTimeToRespond</id>" &
          "<value xsi:type='EAEBAAGA'>10</value>" &
      "</parameter>" &
      "<parameter>" &
          "<id>udc:highLevelProtocol/udc:translator/udc:xmlToHl7/udc:delimiters/@field</id>" &
          "<value xsi:type='EAEAAAAGBAAFA'>124</value>" &
      "</parameter>" &
      "<parameter>" &
          "<id>udc:highLevelProtocol/udc:translator/udc:xmlToHl7/udc:delimiters/@component</id>" &
          "<value xsi:type='EADAAAAGBAAFA'>94</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:highLevelProtocol/udc:translator/udc:xmlToHl7/udc:delimiters/@subComponent</id>" &
        "<value xsi:type='EACAAAAGBAAFA'>38</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:highLevelProtocol/udc:translator/udc:xmlToHl7/udc:delimiters/@repeat</id>" &
        "<value xsi:type='EABAAAAGBAAFA'>126</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:highLevelProtocol/udc:translator/udc:xmlToHl7/udc:delimiters/@escape</id>" &
        "<value xsi:type='EAAAAAAGBAAFA'>92</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:highLevelProtocol/udc:translator/udc:sendingApplication</id>" &
        "<value xsi:type='EAFBAAFA'>BA400</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:highLevelProtocol/udc:translator/udc:sendingFacility</id>" &
        "<value xsi:type='EAEBAAFA'>BioSystems</value>" &
      "</parameter>" &
      "<parameter>" &
          "<id>udc:highLevelProtocol/udc:translator/udc:receivingApplication</id>" &
          "<value xsi:type='EADBAAFA'>Modulab</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:highLevelProtocol/udc:translator/udc:receivingFacility</id>" &
        "<value xsi:type='EACBAAFA'>Systelab</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:highLevelProtocol/udc:translator/udc:codepage</id>" &
        "<value xsi:type='xsd:integer'>65001</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:dataTransmission[@type='TCPIP-Client']/udc:port</id>" &
        "<value xsi:type='xsd:unsignedShort'>11111</value>" &
      "</parameter>" &
      "<parameter>" &
        "<id>udc:dataTransmission[@type='TCPIP-Client']/udc:host</id>" &
        "<value xsi:type='EABBAACA'>172.16.2.44</value>" &
      "</parameter>" &
    "</channel>"

                resultData.SetDatos = channelxml


            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ESxmlTranslator.GetCreateChannelSTLClient", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

    End Class




End Namespace