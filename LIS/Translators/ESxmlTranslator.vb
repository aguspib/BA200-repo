'Class creation 22/02/2013 AG
'Based on SysteLab demo code MainForm.vb, EmbeddedSynapse.vb class

Option Strict On
Option Explicit On

Imports System.Xml

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports System.Globalization.CultureInfo


Namespace Biosystems.Ax00.LISCommunications

    Partial Public Class ESxmlTranslator

#Region "Private Constants"
        Private Const UDCSchema As String = "http://www.nte.es/schema/udc-interface-v1.0"
        Private Const ClinicalInfoSchema As String = "http://www.nte.es/schema/clinical-information-v1.0"
        Private Const TraceSchema As String = "http://www.nte.es/schema/trace-v1.0"
        Private Const MinimumAge As Integer = 0
        Private Const MaximumAge As Integer = 120
#End Region

#Region "Enumerates"

        Public Enum Protocols
            ASTM        ' LIS2A (ASTM)
            HL7LAW      ' LawCompliant - HL7 IHE-LAW
            HL7LAWLite  ' LawQuasiCompliant - HL7 NO IHE compliant
        End Enum

        Public Enum TransmissionModes
            solicited
            unsolicited
        End Enum

        Public Enum Actions
            request
            report
        End Enum

        Public Enum ProcessModes
            production
        End Enum

        Public Enum Priorities
            Routine = 0
            Stats = 1
        End Enum

        Public Enum MessageObjects
            workOrder
            observation
        End Enum

        Public Enum TestSampleClass
            patient
            QC
        End Enum

        Public Enum TestStatusFlags
            normal
            stat
        End Enum

        Public Enum TestActions
            add
            remove
        End Enum

        Public Enum TestPriorities
            stat
            normal
        End Enum

        Public Enum PatientAssignedLocations
            obstetrics
            commercialAccount
            emergency
            inpatient
            notApplicable
            outpatient
            preadmit
            recurringPatient
            unknown
        End Enum

        Public Enum PatientGenders
            male
            female
        End Enum

#End Region

#Region "Declarations"

        Private xmlHelper As xmlHelper = Nothing 'SGM 27/02/2013

#End Region

#Region "Constructor"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pAppVersion"></param>
        ''' <param name="pAnalyzerModel"></param>
        ''' <param name="pAnalyzerSN"></param>
        ''' <remarks>Created by SGM 26/02/2013</remarks>
        Public Sub New(ByVal pChannelID As String, _
                       ByVal pAnalyzerModel As String, _
                       ByVal pAnalyzerSN As String, _
                       Optional ByVal pAppVersion As String = "")

            MyClass.ChannelIdAttr = pChannelID '"ChannelName_SimuDemo" ' pChannelID
            MyClass.AnalyzerModelAttr = pAnalyzerModel  '"Demo"
            MyClass.AnalyzerSerialNumberAttr = pAnalyzerSN
            If pAppVersion.Length > 0 Then MyClass.ApplicationVersionAttr = pAppVersion

            'SGM 27/02/2013
            Me.xmlHelper = New xmlHelper("udc", UDCSchema, "ci", ClinicalInfoSchema)

        End Sub
#End Region

#Region "Private Properties"
        ''' <remarks>Created by: SGM 26/02/2013</remarks>
        Private ReadOnly Property ApplicationVersion As String
            Get
                Return ApplicationVersionAttr
            End Get
        End Property

        ''' <remarks>Created by: SGM 26/02/2013</remarks>
        Private ReadOnly Property AnalyzerModel As String
            Get
                Return AnalyzerModelAttr
            End Get
        End Property

        ''' <remarks>Created by: SGM 26/02/2013</remarks>
        Private ReadOnly Property AnalyzerSerialNumber As String
            Get
                Return AnalyzerSerialNumberAttr
            End Get
        End Property

        ''' <remarks>Created by: SGM 26/02/2013</remarks>
        Private ReadOnly Property ChannelId As String
            Get
                Return ChannelIdAttr
            End Get
        End Property
#End Region

#Region "Private Attributes"
        Private ApplicationVersionAttr As String
        Private AnalyzerModelAttr As String
        Private AnalyzerSerialNumberAttr As String
        Private ChannelIdAttr As String
#End Region

#Region "Public methods"

#Region "PRESENTATION --> LIS"
        ''' <summary>
        ''' Returns the XML for channel creation
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pApplicationID"></param>
        ''' <returns>GlobalDataTO with setdatos as string with proper xml message</returns>
        ''' <remarks>
        ''' Created by:  AG 22/02/2013 - Empty, only creation
        ''' Modified by: SG 26/02/2013 - Built the XML document
        '''              XB 11/03/2013 - Re-built the XML document with final specification
        '''              XB 15/05/2013 - Duplication of delimiters to distinct HL7 to ASTM
        '''              XB 30/09/2013 - "BAx00" is already admitted by ES library instead of "BA400"
        ''' </remarks>
        Public Function GetCreateChannel(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pApplicationID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Read the LIS configuration settings
                        Dim settingsDelg As New UserSettingsDelegate

                        Dim pSelectedProtocol As String = Protocols.HL7LAW.ToString
                        Dim pUserSelectedProcotol As String
                        Dim pIHECompliant As Boolean
                        Dim pDataTransmissionType As String
                        Dim pTimeWaitACK As Integer
                        Dim pTimeWaitResponse As Integer
                        Dim pTimeToRespond As Integer
                        Dim pCodePage As Integer
                        Dim pPortNumber As String
                        Dim pPort2Number As String
                        Dim pHostName As String
                        Dim pHostID As String
                        Dim pHostProvider As String
                        Dim pInstrumentID As String
                        Dim pInstrumentProvider As String
                        Dim pFieldSeparator As String = Nothing
                        Dim pComponentSeparator As String = Nothing
                        Dim pRepeatSeparator As String = Nothing
                        Dim pSpecialSeparator As String = Nothing
                        Dim pSubComponentSeparator As String = Nothing

                        resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_PROTOCOL_NAME.ToString())
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            pUserSelectedProcotol = CType(resultData.SetDatos, String)
                        Else
                            'Error getting the Session Setting value, show it 
                            Exit Try
                        End If

                        resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_IHE_COMPLIANT.ToString())
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            pIHECompliant = CType(resultData.SetDatos, Boolean)
                        Else
                            'Error getting the Session Setting value, show it 
                            Exit Try
                        End If

                        Select Case pUserSelectedProcotol
                            Case "HL7"
                                If (pIHECompliant) Then
                                    pSelectedProtocol = Protocols.HL7LAW.ToString
                                Else
                                    pSelectedProtocol = Protocols.HL7LAWLite.ToString
                                End If

                                'XB 15/05/2013
                                'Delimiters
                                resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_FIELD_SEPARATOR_HL7.ToString())
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    pFieldSeparator = CType(resultData.SetDatos, String)
                                Else
                                    'Error getting the Session Setting value, show it 
                                    Exit Try
                                End If

                                resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_COMP_SEPARATOR_HL7.ToString())
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    pComponentSeparator = CType(resultData.SetDatos, String)
                                Else
                                    'Error getting the Session Setting value, show it 
                                    Exit Try
                                End If

                                resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_REP_SEPARATOR_HL7.ToString())
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    pRepeatSeparator = CType(resultData.SetDatos, String)
                                Else
                                    'Error getting the Session Setting value, show it 
                                    Exit Try
                                End If

                                resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_SPEC_SEPARATOR_HL7.ToString())
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    pSpecialSeparator = CType(resultData.SetDatos, String)
                                Else
                                    'Error getting the Session Setting value, show it 
                                    Exit Try
                                End If

                                resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_SUBC_SEPARATOR_HL7.ToString())
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    pSubComponentSeparator = CType(resultData.SetDatos, String)
                                Else
                                    'Error getting the Session Setting value, show it 
                                    Exit Try
                                End If
                            Case "ASTM"
                                pSelectedProtocol = Protocols.ASTM.ToString

                                'XB 15/05/2013
                                'Delimiters
                                resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_FIELD_SEPARATOR_ASTM.ToString())
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    pFieldSeparator = CType(resultData.SetDatos, String)
                                Else
                                    'Error getting the Session Setting value, show it 
                                    Exit Try
                                End If

                                resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_COMP_SEPARATOR_ASTM.ToString())
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    pComponentSeparator = CType(resultData.SetDatos, String)
                                Else
                                    'Error getting the Session Setting value, show it 
                                    Exit Try
                                End If

                                resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_REP_SEPARATOR_ASTM.ToString())
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    pRepeatSeparator = CType(resultData.SetDatos, String)
                                Else
                                    'Error getting the Session Setting value, show it 
                                    Exit Try
                                End If

                                resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_SPEC_SEPARATOR_ASTM.ToString())
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    pSpecialSeparator = CType(resultData.SetDatos, String)
                                Else
                                    'Error getting the Session Setting value, show it 
                                    Exit Try
                                End If
                        End Select

                        resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_DATA_TRANSMISSION_TYPE.ToString())
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            pDataTransmissionType = CType(resultData.SetDatos, String)
                        Else
                            'Error getting the Session Setting value, show it 
                            Exit Try
                        End If

                        resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_maxTimeWaitingForACK.ToString())
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            pTimeWaitACK = CType(resultData.SetDatos, Integer)
                        Else
                            'Error getting the Session Setting value, show it 
                            Exit Try
                        End If

                        resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_maxTimeWaitingForResponse.ToString())
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            pTimeWaitResponse = CType(resultData.SetDatos, Integer)
                        Else
                            'Error getting the Session Setting value, show it 
                            Exit Try
                        End If

                        resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_BAx00maxTimeToRespond.ToString())
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            pTimeToRespond = CType(resultData.SetDatos, Integer)
                        Else
                            'Error getting the Session Setting value, show it 
                            Exit Try
                        End If

                        resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_CODEPAGE.ToString())
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            pCodePage = CType(resultData.SetDatos, Integer)
                        Else
                            'Error getting the Session Setting value, show it 
                            Exit Try
                        End If

                        resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_TCP_PORT.ToString())
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            pPortNumber = CType(resultData.SetDatos, String)
                        Else
                            'Error getting the Session Setting value, show it 
                            Exit Try
                        End If

                        resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_TCP_PORT2.ToString())
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            pPort2Number = CType(resultData.SetDatos, String)
                        Else
                            'Error getting the Session Setting value, show it 
                            Exit Try
                        End If

                        resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_HOST_NAME.ToString())
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            pHostName = CType(resultData.SetDatos, String)
                        Else
                            'Error getting the Session Setting value, show it 
                            Exit Try
                        End If


                        resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_HOST_ID.ToString())
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            pHostID = CType(resultData.SetDatos, String)
                        Else
                            'Error getting the Session Setting value, show it 
                            Exit Try
                        End If

                        resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_HOST_PROVIDER.ToString())
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            pHostProvider = CType(resultData.SetDatos, String)
                        Else
                            'Error getting the Session Setting value, show it 
                            Exit Try
                        End If

                        resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_INSTRUMENT_ID.ToString())
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            pInstrumentID = CType(resultData.SetDatos, String)
                        Else
                            'Error getting the Session Setting value, show it 
                            Exit Try
                        End If

                        resultData = settingsDelg.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_INSTRUMENT_PROVIDER.ToString())
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            pInstrumentProvider = CType(resultData.SetDatos, String)
                        Else
                            'Error getting the Session Setting value, show it 
                            Exit Try
                        End If

                        Dim Doc As New XmlDocument()

                        'Root Element
                        Dim Channel As XmlElement = Doc.CreateElement("channel")
                        Dim xmlnsAttr As XmlAttribute = Doc.CreateAttribute("xmlns")
                        xmlnsAttr.Value = UDCSchema
                        Channel.Attributes.Append(xmlnsAttr)

                        'RegistrarInformation
                        Dim RegInfo As XmlElement = Doc.CreateElement("registrarInformation")

                        Dim Protocol As XmlElement = Doc.CreateElement("protocol")
                        Protocol.InnerText = pSelectedProtocol
                        RegInfo.AppendChild(Protocol)
                        Dim Role As XmlElement = Doc.CreateElement("role")
                        Role.InnerText = "provider"
                        RegInfo.AppendChild(Role)

                        Dim Device As XmlElement = Doc.CreateElement("device")
                        Dim Manufacturer As XmlElement = Doc.CreateElement("manufacturer")
                        Manufacturer.InnerText = "Biosystems"
                        Device.AppendChild(Manufacturer)
                        Dim Model As XmlElement = Doc.CreateElement("model")
                        Model.InnerText = "BAx00" ' By now is fixed !    ' XB 30/09/2013
                        Device.AppendChild(Model)
                        Dim Version As XmlElement = Doc.CreateElement("version")
                        Version.InnerText = "1.0"  ' By now is fixed !
                        Device.AppendChild(Version)
                        RegInfo.AppendChild(Device)
                        Channel.AppendChild(RegInfo)

                        'Parameters
                        Dim Parameter As XmlElement
                        Dim ParId As XmlElement
                        Dim ParValue As XmlElement

                        'Channel
                        Parameter = Doc.CreateElement("parameter")
                        ParId = Doc.CreateElement("id")
                        ParId.InnerText = "udc:id"
                        Parameter.AppendChild(ParId)
                        ParValue = Doc.CreateElement("value")
                        ParValue.InnerText = MyClass.ChannelIdAttr
                        Parameter.AppendChild(ParValue)
                        Channel.AppendChild(Parameter)

                        'Time wait for response
                        Parameter = Doc.CreateElement("parameter")
                        ParId = Doc.CreateElement("id")
                        ParId.InnerText = "udc:highLevelProtocol/udc:maxTimeWaitingForResponse"
                        Parameter.AppendChild(ParId)
                        ParValue = Doc.CreateElement("value")
                        ParValue.InnerText = pTimeWaitResponse.ToString
                        Parameter.AppendChild(ParValue)
                        Channel.AppendChild(Parameter)

                        Parameter = Doc.CreateElement("parameter")
                        ParId = Doc.CreateElement("id")
                        ParId.InnerText = "udc:highLevelProtocol/udc:maxTimeToRespond"
                        Parameter.AppendChild(ParId)
                        ParValue = Doc.CreateElement("value")
                        ParValue.InnerText = pTimeToRespond.ToString
                        Parameter.AppendChild(ParValue)
                        Channel.AppendChild(Parameter)

                        'Code page
                        Parameter = Doc.CreateElement("parameter")
                        ParId = Doc.CreateElement("id")
                        ParId.InnerText = "udc:highLevelProtocol/udc:translator/udc:codepage"
                        Parameter.AppendChild(ParId)
                        ParValue = Doc.CreateElement("value")
                        ParValue.InnerText = pCodePage.ToString
                        Parameter.AppendChild(ParValue)
                        Channel.AppendChild(Parameter)

                        Select Case (pDataTransmissionType)
                            Case "TCPIP-Client", "TCPIP-Server"
                                'Data transmission type
                                Parameter = Doc.CreateElement("parameter")
                                ParId = Doc.CreateElement("id")
                                ParId.InnerText = "udc:dataTransmission/@type"
                                Parameter.AppendChild(ParId)
                                ParValue = Doc.CreateElement("value")
                                ParValue.InnerText = pDataTransmissionType
                                Parameter.AppendChild(ParValue)

                                Channel.AppendChild(Parameter)   'port
                                Parameter = Doc.CreateElement("parameter")
                                ParId = Doc.CreateElement("id")
                                ParId.InnerText = "udc:dataTransmission[@type='" & pDataTransmissionType & "']/udc:port"
                                Parameter.AppendChild(ParId)
                                ParValue = Doc.CreateElement("value")
                                ParValue.InnerText = pPortNumber.ToString
                                Parameter.AppendChild(ParValue)
                                Channel.AppendChild(Parameter)

                                'Host
                                Parameter = Doc.CreateElement("parameter")
                                ParId = Doc.CreateElement("id")
                                ParId.InnerText = "udc:dataTransmission[@type='" & pDataTransmissionType & "']/udc:host"
                                Parameter.AppendChild(ParId)
                                ParValue = Doc.CreateElement("value")
                                ParValue.InnerText = pHostName.ToString
                                Parameter.AppendChild(ParValue)
                                Channel.AppendChild(Parameter)

                            Case "TCPIP-Trans"
                                'Data transmission type
                                Parameter = Doc.CreateElement("parameter")
                                ParId = Doc.CreateElement("id")
                                ParId.InnerText = "udc:dataTransmission/@type"
                                Parameter.AppendChild(ParId)
                                ParValue = Doc.CreateElement("value")
                                ParValue.InnerText = "TCPIP-TransitoryConnection" ' sorry but this lenght overloads preload database field ...
                                Parameter.AppendChild(ParValue)
                                Channel.AppendChild(Parameter)

                                'Port Client
                                Parameter = Doc.CreateElement("parameter")
                                ParId = Doc.CreateElement("id")
                                ParId.InnerText = "udc:dataTransmission[@type='TCPIP-TransitoryConnection']/udc:clientPort"
                                Parameter.AppendChild(ParId)
                                ParValue = Doc.CreateElement("value")
                                ParValue.InnerText = pPortNumber.ToString
                                Parameter.AppendChild(ParValue)
                                Channel.AppendChild(Parameter)

                                'Port Server
                                Parameter = Doc.CreateElement("parameter")
                                ParId = Doc.CreateElement("id")
                                ParId.InnerText = "udc:dataTransmission[@type='TCPIP-TransitoryConnection']/udc:serverPort"
                                Parameter.AppendChild(ParId)
                                ParValue = Doc.CreateElement("value")
                                ParValue.InnerText = pPort2Number.ToString
                                Parameter.AppendChild(ParValue)
                                Channel.AppendChild(Parameter)

                                'Host
                                Parameter = Doc.CreateElement("parameter")
                                ParId = Doc.CreateElement("id")
                                ParId.InnerText = "udc:dataTransmission[@type='TCPIP-TransitoryConnection']/udc:host"
                                Parameter.AppendChild(ParId)
                                ParValue = Doc.CreateElement("value")
                                ParValue.InnerText = pHostName.ToString
                                Parameter.AppendChild(ParValue)
                                Channel.AppendChild(Parameter)
                        End Select

                        If (pSelectedProtocol.Contains("HL7LAW")) Then
                            'HL7LAW or HL7LAWLite

                            'Time wait for ACK 
                            Parameter = Doc.CreateElement("parameter")
                            ParId = Doc.CreateElement("id")
                            ParId.InnerText = "udc:highLevelProtocol/udc:maxTimeWaitingForACK"
                            Parameter.AppendChild(ParId)
                            ParValue = Doc.CreateElement("value")
                            ParValue.InnerText = pTimeWaitACK.ToString
                            Parameter.AppendChild(ParValue)
                            Channel.AppendChild(Parameter)

                            'Delimiters 1/5
                            Parameter = Doc.CreateElement("parameter")
                            ParId = Doc.CreateElement("id")
                            ParId.InnerText = "udc:highLevelProtocol/udc:translator/udc:xmlToHl7/udc:delimiters/@field"
                            Parameter.AppendChild(ParId)
                            ParValue = Doc.CreateElement("value")
                            ParValue.InnerText = Asc(pFieldSeparator).ToString
                            Parameter.AppendChild(ParValue)
                            Channel.AppendChild(Parameter)

                            'Delimiters 2/5
                            Parameter = Doc.CreateElement("parameter")
                            ParId = Doc.CreateElement("id")
                            ParId.InnerText = "udc:highLevelProtocol/udc:translator/udc:xmlToHl7/udc:delimiters/@component"
                            Parameter.AppendChild(ParId)
                            ParValue = Doc.CreateElement("value")
                            ParValue.InnerText = Asc(pComponentSeparator).ToString
                            Parameter.AppendChild(ParValue)
                            Channel.AppendChild(Parameter)

                            'Delimiters 3/5
                            Parameter = Doc.CreateElement("parameter")
                            ParId = Doc.CreateElement("id")
                            ParId.InnerText = "udc:highLevelProtocol/udc:translator/udc:xmlToHl7/udc:delimiters/@subComponent"
                            Parameter.AppendChild(ParId)
                            ParValue = Doc.CreateElement("value")
                            ParValue.InnerText = Asc(pSubComponentSeparator).ToString
                            Parameter.AppendChild(ParValue)
                            Channel.AppendChild(Parameter)

                            'Delimiters 4/5
                            Parameter = Doc.CreateElement("parameter")
                            ParId = Doc.CreateElement("id")
                            ParId.InnerText = "udc:highLevelProtocol/udc:translator/udc:xmlToHl7/udc:delimiters/@repeat"
                            Parameter.AppendChild(ParId)
                            ParValue = Doc.CreateElement("value")
                            ParValue.InnerText = Asc(pRepeatSeparator).ToString
                            Parameter.AppendChild(ParValue)
                            Channel.AppendChild(Parameter)

                            'Delimiters 5/5
                            Parameter = Doc.CreateElement("parameter")
                            ParId = Doc.CreateElement("id")
                            ParId.InnerText = "udc:highLevelProtocol/udc:translator/udc:xmlToHl7/udc:delimiters/@escape"
                            Parameter.AppendChild(ParId)
                            ParValue = Doc.CreateElement("value")
                            ParValue.InnerText = Asc(pSpecialSeparator).ToString
                            Parameter.AppendChild(ParValue)
                            Channel.AppendChild(Parameter)

                            'Sending application
                            Parameter = Doc.CreateElement("parameter")
                            ParId = Doc.CreateElement("id")
                            ParId.InnerText = "udc:highLevelProtocol/udc:translator/udc:sendingApplication"
                            Parameter.AppendChild(ParId)
                            ParValue = Doc.CreateElement("value")
                            ParValue.InnerText = pInstrumentID
                            Parameter.AppendChild(ParValue)
                            Channel.AppendChild(Parameter)

                            'Sending facility
                            Parameter = Doc.CreateElement("parameter")
                            ParId = Doc.CreateElement("id")
                            ParId.InnerText = "udc:highLevelProtocol/udc:translator/udc:sendingFacility"
                            Parameter.AppendChild(ParId)
                            ParValue = Doc.CreateElement("value")
                            ParValue.InnerText = pInstrumentProvider
                            Parameter.AppendChild(ParValue)
                            Channel.AppendChild(Parameter)

                            'Receiving application
                            Parameter = Doc.CreateElement("parameter")
                            ParId = Doc.CreateElement("id")
                            ParId.InnerText = "udc:highLevelProtocol/udc:translator/udc:receivingApplication"
                            Parameter.AppendChild(ParId)
                            ParValue = Doc.CreateElement("value")
                            ParValue.InnerText = pHostID
                            Parameter.AppendChild(ParValue)
                            Channel.AppendChild(Parameter)

                            'Receiving facility
                            Parameter = Doc.CreateElement("parameter")
                            ParId = Doc.CreateElement("id")
                            ParId.InnerText = "udc:highLevelProtocol/udc:translator/udc:receivingFacility"
                            Parameter.AppendChild(ParId)
                            ParValue = Doc.CreateElement("value")
                            ParValue.InnerText = pHostProvider
                            Parameter.AppendChild(ParValue)
                            Channel.AppendChild(Parameter)
                        Else
                            'ASTM 
                            'Time keep alive
                            Parameter = Doc.CreateElement("parameter")
                            ParId = Doc.CreateElement("id")
                            ParId.InnerText = "udc:lowLevelProtocol/udc:checkChannelStatusTime"
                            Parameter.AppendChild(ParId)
                            ParValue = Doc.CreateElement("value")
                            If (pTimeWaitACK < 35) Then
                                'Minimum value for ASTM
                                pTimeWaitACK = 35
                            End If
                            ParValue.InnerText = pTimeWaitACK.ToString
                            Parameter.AppendChild(ParValue)
                            Channel.AppendChild(Parameter)

                            'Delimiters 1/4
                            Parameter = Doc.CreateElement("parameter")
                            ParId = Doc.CreateElement("id")
                            ParId.InnerText = "udc:highLevelProtocol/udc:translator/udc:fieldDelimiter"
                            Parameter.AppendChild(ParId)
                            ParValue = Doc.CreateElement("value")
                            ParValue.InnerText = Asc(pFieldSeparator).ToString
                            Parameter.AppendChild(ParValue)
                            Channel.AppendChild(Parameter)

                            'Delimiters 2/4
                            Parameter = Doc.CreateElement("parameter")
                            ParId = Doc.CreateElement("id")
                            ParId.InnerText = "udc:highLevelProtocol/udc:translator/udc:componentDelimiter"
                            Parameter.AppendChild(ParId)
                            ParValue = Doc.CreateElement("value")
                            ParValue.InnerText = Asc(pComponentSeparator).ToString
                            Parameter.AppendChild(ParValue)
                            Channel.AppendChild(Parameter)

                            'Delimiters 3/4
                            Parameter = Doc.CreateElement("parameter")
                            ParId = Doc.CreateElement("id")
                            ParId.InnerText = "udc:highLevelProtocol/udc:translator/udc:repeatDelimiter"
                            Parameter.AppendChild(ParId)
                            ParValue = Doc.CreateElement("value")
                            ParValue.InnerText = Asc(pRepeatSeparator).ToString
                            Parameter.AppendChild(ParValue)
                            Channel.AppendChild(Parameter)

                            'Delimiters 4/4
                            Parameter = Doc.CreateElement("parameter")
                            ParId = Doc.CreateElement("id")
                            ParId.InnerText = "udc:highLevelProtocol/udc:translator/udc:escapeDelimiter"
                            Parameter.AppendChild(ParId)
                            ParValue = Doc.CreateElement("value")
                            ParValue.InnerText = Asc(pSpecialSeparator).ToString
                            Parameter.AppendChild(ParValue)
                            Channel.AppendChild(Parameter)

                            'Sender
                            Parameter = Doc.CreateElement("parameter")
                            ParId = Doc.CreateElement("id")
                            ParId.InnerText = "udc:highLevelProtocol/udc:translator/udc:sender/udc:id"
                            Parameter.AppendChild(ParId)
                            ParValue = Doc.CreateElement("value")
                            ParValue.InnerText = pInstrumentID
                            Parameter.AppendChild(ParValue)
                            Channel.AppendChild(Parameter)

                            'Receiver
                            Parameter = Doc.CreateElement("parameter")
                            ParId = Doc.CreateElement("id")
                            ParId.InnerText = "udc:highLevelProtocol/udc:translator/udc:receiver/udc:id"
                            Parameter.AppendChild(ParId)
                            ParValue = Doc.CreateElement("value")
                            ParValue.InnerText = pHostID
                            Parameter.AppendChild(ParValue)
                            Channel.AppendChild(Parameter)
                        End If
                        Doc.AppendChild(Channel)

                        resultData.SetDatos = Doc.InnerXml
                        'End SG 27/02/2013
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.GetCreateChannel", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Returns the XML for get the message storage
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pMaxSendMessages"></param>
        ''' <param name="pSendFolder"></param>
        ''' <param name="pMaxReceiveMessages"></param>
        ''' <param name="pReceiveFolder"></param>
        ''' <returns>GlobalDataTO with setdatos as string with proper xml message</returns>
        ''' <remarks>
        ''' Created by:  AG 22/02/2013 - Empty, only creation
        ''' Modified by: SG 26/02/2013 - Built the XML document
        '''              AG 05/03/2013 - MaxMessages also configured for Receive tag
        ''' </remarks>
        Public Function GetMessageStorage(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pMaxSendMessages As Integer, ByVal pSendFolder As String, _
                                          ByVal pMaxReceiveMessages As Integer, ByVal pReceiveFolder As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Dim myCommand As String =
                        '    "<command type='storage' xmlns='http://www.nte.es/schema/udc-interface-v1.0'>" &
                        '        "<header>" &
                        '            "<id>setStorage</id>" &
                        '        "</header>" &
                        '        "<parameters>" &
                        '            "<storage>" &
                        '                "<send type='file'>" &
                        '                    "<maxMessages>" & pMaxMessages.ToString() & "</maxMessages>" &
                        '                    "<directory>" & pSendFolder & "</directory>" &
                        '                "</send>" &
                        '                "<receive type='file'>" &
                        '                    "<maxMessages>" & pMaxMessages.ToString() & "</maxMessages>" & 'After meeting with J Orozco (STL - 05/03/2013)
                        '                    "<directory>" & pReceiveFolder & "</directory>" &
                        '                "</receive>" &
                        '            "</storage>" &
                        '        "</parameters>" &
                        '    "</command>"

                        'SG 26/02/2012
                        Dim Doc As New XmlDocument()
                        resultData = MyClass.CreateRootElement(Doc, "storage")
                        If (Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing) Then
                            Dim Command As XmlElement = CType(resultData.SetDatos, XmlElement)

                            'Header
                            Dim Header As XmlNode = Doc.CreateElement("header")
                            Dim Id As XmlNode = Doc.CreateElement("id")
                            Id.InnerText = "setStorage"
                            Header.AppendChild(Id)
                            Command.AppendChild(Header)

                            'Parameters
                            Dim parameters As XmlNode = Doc.CreateElement("parameters")

                            'Storage
                            Dim storage As XmlNode = Doc.CreateElement("storage")

                            'Send
                            Dim send As XmlNode = Doc.CreateElement("send")
                            Dim sendTypeAttr As XmlAttribute = Doc.CreateAttribute("type") : sendTypeAttr.Value = "file"
                            send.Attributes.Append(sendTypeAttr)

                            'MaxMessages
                            Dim maxSendMessages As XmlNode = Doc.CreateElement("maxMessages")
                            send.AppendChild(maxSendMessages)
                            maxSendMessages.InnerText = pMaxSendMessages.ToString()

                            'Directory
                            Dim dir1 As XmlNode = Doc.CreateElement("directory")
                            dir1.InnerText = pSendFolder
                            send.AppendChild(dir1)
                            storage.AppendChild(send)

                            'Receive
                            Dim receive As XmlNode = Doc.CreateElement("receive")
                            Dim receiveTypeAttr As XmlAttribute = Doc.CreateAttribute("type") : receiveTypeAttr.Value = "file"
                            receive.Attributes.Append(receiveTypeAttr)

                            'AG 06/03/2013
                            Dim maxReceiveMessages As XmlNode = Doc.CreateElement("maxMessages")
                            receive.AppendChild(maxReceiveMessages)
                            maxReceiveMessages.InnerText = pMaxReceiveMessages.ToString()
                            'AG 06/03/2013

                            Dim dir2 As XmlNode = Doc.CreateElement("directory")
                            dir2.InnerText = pReceiveFolder
                            receive.AppendChild(dir2)
                            storage.AppendChild(receive)

                            parameters.AppendChild(storage)
                            Command.AppendChild(parameters)
                            Doc.AppendChild(Command)

                            resultData.SetDatos = Doc.InnerXml
                        End If
                        'End SG 26/02/2012
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.GetMessageStorage", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Returns the XML for get pending messages
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO with setdatos as string with proper xml message</returns>
        ''' <remarks>
        ''' Created by:  AG 22/02/2013 - Empty, only creation
        ''' Modified by: SG 26/02/2013 - Built the XML document
        ''' </remarks>
        Public Function GetPendingMessages(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Dim Command As String =
                        '"<command type='getPendingMessages' xmlns='http://www.nte.es/schema/udc-interface-v1.0'>" &
                        '"<header>" &
                        '"<id>command02</id>" &
                        '"<channel>" &
                        '"<id>" & Channel ID & "</id>" &
                        '"</channel>" &
                        '"</header>" &
                        '"</command>"

                        'SG 26/02/2013
                        Dim Doc As New XmlDocument()
                        resultData = MyClass.CreateRootElement(Doc, "getPendingMessages")
                        If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                            Dim Command As XmlElement = CType(resultData.SetDatos, XmlElement)

                            'Header
                            Dim Header As XmlNode = Doc.CreateElement("header")
                            Dim Id As XmlNode = Doc.CreateElement("id")
                            Id.InnerText = "command02"
                            Header.AppendChild(Id)

                            'Channel
                            Dim Channel As XmlNode = Doc.CreateElement("channel")

                            'Channel ID
                            Dim ChannelId As XmlNode = Doc.CreateElement("id")
                            ChannelId.InnerText = MyClass.ChannelId

                            Channel.AppendChild(ChannelId)
                            Header.AppendChild(Channel)
                            Command.AppendChild(Header)
                            Doc.AppendChild(Command)

                            resultData.SetDatos = Doc.InnerXml

                        End If
                        'End SG 26/02/2013
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.GetPendingMessages", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Returns the XML for delete specific messages
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pMessageID">Message Identifier</param>
        ''' <returns>GlobalDataTO with setdatos as string with proper xml message</returns>
        ''' <remarks>
        ''' Created by:  AG 22/02/2013 - Empty, only creation
        ''' Modified by: SG 26/02/2013 - Built the XML document
        ''' </remarks>
        Public Function GetDeleteMessage(ByVal pDBConnection As SqlClient.SqlConnection, _
                                         ByVal pMessageID As String) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Dim Command As String =
                        '"<command type='deleteMessage' xmlns='http://www.nte.es/schema/udc-interface-v1.0'>" &
                        '"<header>" &
                        '"<id>command03</id>" &
                        '"<channel>" &
                        '"<id>" & Channel ID & "</id>" &
                        '"</channel>" &
                        '"</header>" &
                        '"<parameters>" &
                        '"<command>" &
                        '"<id>" & Message ID & "</id>" &
                        '"</command>" &
                        '"</parameters>" &
                        '"</command>"

                        'SG 26/02/2013
                        Dim Doc As New XmlDocument()
                        resultData = MyClass.CreateRootElement(Doc, "deleteMessage")
                        If (Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing) Then
                            Dim Command As XmlElement = CType(resultData.SetDatos, XmlElement)

                            'Header
                            Dim Header As XmlNode = Doc.CreateElement("header")
                            Dim Id As XmlNode = Doc.CreateElement("id")
                            Id.InnerText = "command03"
                            Header.AppendChild(Id)

                            'Channel
                            Dim Channel As XmlNode = Doc.CreateElement("channel")

                            'Channel ID
                            Dim ChannelId As XmlNode = Doc.CreateElement("id")
                            ChannelId.InnerText = MyClass.ChannelId

                            Channel.AppendChild(ChannelId)
                            Header.AppendChild(Channel)
                            Command.AppendChild(Header)

                            Dim Parameters As XmlNode = Doc.CreateElement("parameters")
                            Dim Command2 As XmlNode = Doc.CreateElement("command")
                            Dim Command2Id As XmlNode = Doc.CreateElement("id")
                            Command2Id.InnerText = pMessageID

                            Command2.AppendChild(Command2Id)
                            Parameters.AppendChild(Command2)
                            Command.AppendChild(Parameters)
                            Doc.AppendChild(Command)

                            resultData.SetDatos = Doc.InnerXml
                        End If
                        'End SG 26/02/2013
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.GetDeleteMessage", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Returns the XML for delete all messages in queue
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO with setdatos as string with proper xml message</returns>
        ''' <remarks>
        ''' Created by:  AG 22/02/2013 - Empty, only creation
        ''' Modified by: SG 26/02/2012 - Built the XML document
        ''' </remarks>
        Public Function GetDeleteAllMessages(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Dim myCommand As String =
                        '    "<command type='deleteMessage' xmlns='http://www.nte.es/schema/udc-interface-v1.0'>" &
                        '    "<header>" &
                        '    "<id>command04</id>" &
                        '    "<channel>" &
                        '    "<id>" & pChannelID & "</id>" &
                        '    "</channel>" &
                        '    "</header>" &
                        '    "<parameters>" &
                        '    "<command set='all'/>" &
                        '    "</parameters>" &
                        '    "</command>"

                        'SG 26/02/2013
                        Dim Doc As New XmlDocument()
                        resultData = MyClass.CreateRootElement(Doc, "deleteMessage")
                        If (Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing) Then
                            Dim Command As XmlElement = CType(resultData.SetDatos, XmlElement)

                            'Header
                            Dim Header As XmlNode = Doc.CreateElement("header")
                            Dim Id As XmlNode = Doc.CreateElement("id")
                            Id.InnerText = "command04"
                            Header.AppendChild(Id)

                            'Channel
                            Dim Channel As XmlNode = Doc.CreateElement("channel")

                            'Channel ID
                            Dim ChannelId As XmlNode = Doc.CreateElement("id")
                            ChannelId.InnerText = MyClass.ChannelId

                            Channel.AppendChild(ChannelId)
                            Header.AppendChild(Channel)
                            Command.AppendChild(Header)

                            Dim Parameters As XmlNode = Doc.CreateElement("parameters")
                            Dim Command2 As XmlNode = Doc.CreateElement("command")
                            Dim Command2Attr As XmlAttribute = Doc.CreateAttribute("set")
                            Command2Attr.Value = "all"
                            Command2.Attributes.Append(Command2Attr)
                            Parameters.AppendChild(Command2)

                            Command.AppendChild(Parameters)
                            Doc.AppendChild(Command)

                            resultData.SetDatos = Doc.InnerXml
                        End If
                        'End SG 26/02/2013
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.GetDeleteAllMessages", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ' ''' <summary>
        ' ''' Returns the xml for delete all messages in incoming queue (LIS -> ES)
        ' ''' </summary>
        ' ''' <param name="pDBConnection"></param>
        ' ''' <returns>GlobalDataTO with setdatos as string with proper xml message</returns>
        ' ''' <remarks>
        ' ''' Creation AG 22/02/2013 - empty, only creation
        ' ''' </remarks>
        'Public Function GetDeleteIncomingMessages(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO

        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                'ES Library offers this functionality or not????
        '                '<Function logic>
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.GetDeleteIncomingMessages", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        ''' <summary>
        ''' Returns the XML for query all message
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pMessageID">Message Identifier</param>
        ''' <returns>GlobalDataTO with setdatos as string with proper xml message</returns>
        ''' <remarks>
        ''' Created by:  AG 22/02/2013 - empty, only creation
        ''' Modified by: SG 26/02/2013 - Built the XML document
        ''' </remarks>
        Public Function GetQueryAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pMessageID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'SG 26/02/2013
                        Dim Doc As New XmlDocument()
                        resultData = MyClass.CreateRootElement(Doc, "message")
                        If (Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing) Then
                            Dim Command As XmlElement = CType(resultData.SetDatos, XmlElement)

                            'Header
                            resultData = MyClass.CreateHeaderNode(Doc, pMessageID, ProcessModes.production, 2, _
                                                                  TransmissionModes.unsolicited, _
                                                                  Actions.request, MessageObjects.workOrder, _
                                                                  DateTime.Now)

                            If (Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing) Then
                                Dim Header As XmlNode = CType(resultData.SetDatos, XmlNode)
                                Command.AppendChild(Header)

                                'Body
                                Dim Body As XmlNode = Doc.CreateElement("body")
                                Dim BodyAttr As XmlAttribute = Doc.CreateAttribute("xmlns")
                                BodyAttr.Value = UDCSchema
                                Body.Attributes.Append(BodyAttr)

                                'Service
                                Dim Message As XmlNode = Doc.CreateElement("message")

                                '"<body xmlns="http://www.nte.es/schema/udc-interface-v1.0">" &
                                ' "<message>" &
                                ' "<ci:service xmlns:ci="http://www.nte.es/schema/clinical-information-v1.0">" &
                                ' "<ci:data>order</ci:data>" &
                                ' "<ci:type>new</ci:type>" &
                                ' "ci:specimen set="all" />" &
                                '"</ci:service>" &

                                Dim Service As XmlNode = Doc.CreateElement("ci", "service", ClinicalInfoSchema)
                                Dim Data As XmlNode = Doc.CreateElement("ci", "data", ClinicalInfoSchema)
                                Data.InnerText = "order"
                                Service.AppendChild(Data)
                                Dim Type As XmlNode = Doc.CreateElement("ci", "type", ClinicalInfoSchema)
                                Type.InnerText = "new"
                                Service.AppendChild(Type)
                                Dim Specimen As XmlNode = Doc.CreateElement("ci", "specimen", ClinicalInfoSchema)
                                Dim specimenAttr As XmlAttribute = Doc.CreateAttribute("set")
                                specimenAttr.Value = "all"
                                Specimen.Attributes.Append(specimenAttr)
                                Service.AppendChild(Specimen)

                                Message.AppendChild(Service)

                                'Source
                                resultData = MyClass.CreateSourceNode(Doc)
                                If (Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing) Then
                                    Dim Source As XmlNode = CType(resultData.SetDatos, XmlNode)
                                    Message.AppendChild(Source)
                                    Body.AppendChild(Message)
                                    Command.AppendChild(Body)
                                    Doc.AppendChild(Command)
                                    resultData.SetDatos = Doc.InnerXml
                                End If
                            End If
                        End If
                        'End SG 26/02/2013
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.GetQueryAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Returns the XML for host query message
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pMessageID">Message Identifier</param>
        ''' <param name="pSpecimenList"></param>
        ''' <returns>GlobalDataTO with setdatos as string with proper xml message</returns>
        ''' <remarks>
        ''' Created by:  AG 22/02/2013 - Empty, only creation
        ''' Modified by: SG 26/02/2013 - Built the XML document
        ''' </remarks>
        Public Function GetHostQuery(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pMessageID As String, _
                                     ByVal pSpecimenList As List(Of String)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'SG 26/02/2013
                        Dim Doc As New XmlDocument()
                        resultData = MyClass.CreateRootElement(Doc, "message")

                        If (Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing) Then
                            Dim Command As XmlElement = CType(resultData.SetDatos, XmlElement)

                            'Header
                            resultData = MyClass.CreateHeaderNode(Doc, pMessageID, ProcessModes.production, 2, TransmissionModes.unsolicited, _
                                                                  Actions.request, MessageObjects.workOrder, DateTime.Now)

                            If (Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing) Then
                                Dim Header As XmlNode = CType(resultData.SetDatos, XmlNode)
                                Command.AppendChild(Header)

                                'Body
                                Dim Body As XmlNode = Doc.CreateElement("body")
                                Dim BodyAttr As XmlAttribute = Doc.CreateAttribute("xmlns")
                                BodyAttr.Value = UDCSchema
                                Body.Attributes.Append(BodyAttr)

                                'Service
                                '"<body xmlns="http://www.nte.es/schema/udc-interface-v1.0">" &
                                ' "<message>" &
                                ' "<ci:service xmlns:ci="http://www.nte.es/schema/clinical-information-v1.0">" &
                                ' "<ci:data>order</ci:data>" &
                                ' "<ci:type>new</ci:type>" &
                                ' "ci:specimen set="all" />" &
                                '"</ci:service>" &

                                Dim Message As XmlNode = Doc.CreateElement("message")
                                Dim Service As XmlNode = Doc.CreateElement("ci", "service", ClinicalInfoSchema)
                                Dim Data As XmlNode = Doc.CreateElement("ci", "data", ClinicalInfoSchema)
                                Data.InnerText = "order"
                                Service.AppendChild(Data)
                                Dim Type As XmlNode = Doc.CreateElement("ci", "type", ClinicalInfoSchema)
                                Type.InnerText = "new"
                                Service.AppendChild(Type)

                                For Each SId As String In pSpecimenList
                                    Dim Specimen As XmlNode = Doc.CreateElement("ci", "specimen", ClinicalInfoSchema)
                                    Dim specimenAttr As XmlAttribute = Doc.CreateAttribute("set")
                                    specimenAttr.Value = "particular"
                                    Specimen.Attributes.Append(specimenAttr)

                                    Dim SpecimenId As XmlNode = Doc.CreateElement("ci", "id", ClinicalInfoSchema)
                                    SpecimenId.InnerText = SId
                                    Specimen.AppendChild(SpecimenId)
                                    Service.AppendChild(Specimen)
                                Next
                                Message.AppendChild(Service)

                                'Source
                                resultData = MyClass.CreateSourceNode(Doc)
                                If (Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing) Then
                                    Dim Source As XmlNode = CType(resultData.SetDatos, XmlNode)
                                    Message.AppendChild(Source)
                                    Body.AppendChild(Message)
                                    Command.AppendChild(Body)
                                    Doc.AppendChild(Command)

                                    resultData.SetDatos = Doc.InnerXml
                                End If
                            End If
                        End If
                        'End SG 26/02/2013
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.GetHostQuery", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Returns the XML for Awos accept message
        ''' When optional parameter pAcceptCompleteOrder is True the body and message tags are empty
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pMessageID">Message Identifier</param>
        ''' <param name="pAcceptedAwosIDList"></param>
        ''' <returns>GlobalDataTO with setdatos as string with proper xml message</returns>
        ''' <remarks>
        ''' Created by:  AG 22/02/2013 - Empty, only creation
        ''' Modified by: SG 26/02/2013 - Built the XML document
        '''              AG 08/03/2013 - Added optional parameter pAcceptCompleteOrder (default FALSE)
        ''' </remarks>
        Public Function GetAwosAccept(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pMessageID As String, _
                                      ByVal pAcceptedAwosIDList As List(Of String), Optional ByVal pAcceptCompleteOrder As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'SG 26/02/2013
                        Dim Doc As New XmlDocument()
                        resultData = MyClass.CreateRootElement(Doc, "message")

                        If (Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing) Then
                            Dim Command As XmlElement = CType(resultData.SetDatos, XmlElement)

                            'Header
                            resultData = MyClass.CreateHeaderNode(Doc, pMessageID, ProcessModes.production, 3, TransmissionModes.solicited, _
                                                                  Actions.report, MessageObjects.workOrder, DateTime.Now)

                            If (Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing) Then
                                Dim Header As XmlNode = CType(resultData.SetDatos, XmlNode)
                                Command.AppendChild(Header)

                                'Body
                                Dim Body As XmlNode = Doc.CreateElement("body")
                                Dim BodyAttr As XmlAttribute = Doc.CreateAttribute("xmlns")
                                BodyAttr.Value = UDCSchema
                                Body.Attributes.Append(BodyAttr)

                                Dim Message As XmlNode = Doc.CreateElement("message")

                                If (Not pAcceptCompleteOrder) Then 'Service tag is filled only when this parameter is false
                                    'service
                                    '"<ci:service>" & 
                                    '"<ci:id>" &  AWOSID 1  & "</ci:id>" &
                                    '"<ci:status>done</ci:status>" &
                                    '"</ci:service>" & 
                                    For Each AwosId As String In pAcceptedAwosIDList
                                        Dim Service As XmlNode = Doc.CreateElement("ci", "service", ClinicalInfoSchema)

                                        'AG 13/03/2013 - remove these tags
                                        'Dim Data As XmlNode = Doc.CreateElement("ci", "data", ClinicalInfoSchema)
                                        'Data.InnerText = "order"
                                        'Service.AppendChild(Data)
                                        'Dim Type As XmlNode = Doc.CreateElement("ci", "type", ClinicalInfoSchema)
                                        'Type.InnerText = "new"
                                        'Service.AppendChild(Type)

                                        Dim Id As XmlNode = Doc.CreateElement("ci", "id", ClinicalInfoSchema)
                                        Id.InnerText = AwosId
                                        Service.AppendChild(Id)

                                        Dim Status As XmlNode = Doc.CreateElement("ci", "status", ClinicalInfoSchema)
                                        Status.InnerText = "done"
                                        Service.AppendChild(Status)

                                        Message.AppendChild(Service)
                                    Next
                                End If

                                'Source
                                resultData = MyClass.CreateSourceNode(Doc)
                                If (Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing) Then
                                    Dim Source As XmlNode = CType(resultData.SetDatos, XmlNode)
                                    Message.AppendChild(Source)
                                    Body.AppendChild(Message)
                                    Command.AppendChild(Body)
                                    Doc.AppendChild(Command)

                                    resultData.SetDatos = Doc.InnerXml
                                End If
                            End If
                        End If
                        'End SG 26/02/2013
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.GetAwosAccept", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Returns the XML for Awos reject message
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pMessageID">Message Identifier</param>
        ''' <param name="pRejectedAwosIDList"></param>
        ''' <returns>GlobalDataTO with setdatos as string with proper xml message</returns>
        ''' <remarks>
        ''' Created by:  AG 22/02/2013 - Empty, only creation
        ''' Modified by: SG 26/02/2013 - Built the XML document
        ''' </remarks>
        Public Function GetAwosReject(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pMessageID As String, _
                                      ByVal pRejectedAwosIDList As List(Of String)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'SG 26/02/2013
                        Dim Doc As New XmlDocument()
                        resultData = MyClass.CreateRootElement(Doc, "message")

                        If (Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing) Then
                            Dim Command As XmlElement = CType(resultData.SetDatos, XmlElement)

                            'Header
                            resultData = MyClass.CreateHeaderNode(Doc, pMessageID, ProcessModes.production, 3, TransmissionModes.solicited, _
                                                                  Actions.report, MessageObjects.workOrder, DateTime.Now)

                            If (Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing) Then
                                Dim Header As XmlNode = CType(resultData.SetDatos, XmlNode)
                                Command.AppendChild(Header)

                                'Body
                                Dim Body As XmlNode = Doc.CreateElement("body")
                                Dim BodyAttr As XmlAttribute = Doc.CreateAttribute("xmlns")
                                BodyAttr.Value = UDCSchema
                                Body.Attributes.Append(BodyAttr)

                                'Service
                                '"<ci:service>" & 
                                '"<ci:id>" &  AWOSID 1  & "</ci:id>" &
                                '"<ci:status>cannotBeDone</ci:status>" &
                                '"</ci:service>" & 
                                Dim Message As XmlNode = Doc.CreateElement("message")
                                For Each AwosId As String In pRejectedAwosIDList
                                    Dim Service As XmlNode = Doc.CreateElement("ci", "service", ClinicalInfoSchema)

                                    'AG 13/03/2013 - remove these tags
                                    'Dim Data As XmlNode = Doc.CreateElement("ci", "data", ClinicalInfoSchema)
                                    'Data.InnerText = "order"
                                    'Service.AppendChild(Data)
                                    'Dim Type As XmlNode = Doc.CreateElement("ci", "type", ClinicalInfoSchema)
                                    'Type.InnerText = "new"
                                    'Service.AppendChild(Type)

                                    Dim Id As XmlNode = Doc.CreateElement("ci", "id", ClinicalInfoSchema)
                                    Id.InnerText = AwosId
                                    Service.AppendChild(Id)

                                    Dim Status As XmlNode = Doc.CreateElement("ci", "status", ClinicalInfoSchema)
                                    Status.InnerText = "cannotBeDone"
                                    Service.AppendChild(Status)

                                    Message.AppendChild(Service)
                                Next

                                'Source
                                resultData = MyClass.CreateSourceNode(Doc)
                                If (Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing) Then
                                    Dim Source As XmlNode = CType(resultData.SetDatos, XmlNode)
                                    Message.AppendChild(Source)
                                    Body.AppendChild(Message)
                                    Command.AppendChild(Body)
                                    Doc.AppendChild(Command)

                                    resultData.SetDatos = Doc.InnerXml
                                End If
                            End If
                        End If
                        'End SG 26/02/2013
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.GetAwosReject", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Returns the xml for UploadResults message adapted for reject awos delayed
        ''' (using the UploadResults xml format but simplified)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pMessageID">Message Identifier</param>
        ''' <param name="pRejectedAwosIDs"></param>
        ''' <returns>GlobalDataTO with setdatos as string with proper xml message</returns>
        ''' <remarks>
        ''' Created by:  AG 13/03/2013 - Empty, only creation
        ''' Modified by: SG 22/03/2013 - Raw business
        '''              SG 25/03/2013 - Take TestIdString instead of TestId
        '''              XB 31/05/2013 - The 'date' tag inside the 'service' tag was removed in version 1.6 of the specification ES integration XML structure
        ''' </remarks>
        Public Function GetAwosRejectDelayed(ByVal pDBConnection As SqlClient.SqlConnection, _
                                      ByVal pMessageID As String, _
                                      ByVal pRejectedAwosIDs As OrderTestsLISInfoDS) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'SGM 22/03/2013
                        Dim Doc As New XmlDocument()
                        resultData = MyClass.CreateRootElement(Doc, "message")
                        If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                            Dim Command As XmlElement = CType(resultData.SetDatos, XmlElement)

                            'header
                            resultData = MyClass.CreateHeaderNode(Doc, pMessageID, ProcessModes.production, 0, _
                                                                  TransmissionModes.unsolicited, _
                                                                  Actions.report, MessageObjects.observation, _
                                                                  DateTime.Now)

                            If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                                Dim Header As XmlNode = CType(resultData.SetDatos, XmlNode)
                                Command.AppendChild(Header)

                                'body
                                Dim Body As XmlNode = Doc.CreateElement("body")
                                Dim BodyAttr As XmlAttribute = Doc.CreateAttribute("xmlns")
                                BodyAttr.Value = UDCSchema
                                Body.Attributes.Append(BodyAttr)

                                Dim Message As XmlNode = Doc.CreateElement("message")

                                'service

                                '<ci:service xmlns:ci="http://www.nte.es/schema/clinical-information-v1.0">
                                '<ci:date> DateTime 2013-01-29T10:20:30</ci:date>
                                '<ci:id>AWOSID01</ci:id>
                                '<ci:status>cancelled</ci:status>
                                '<ci:test>
                                '<ci:id>LISMapped Test1</ci:id>
                                '<ci:role>patient or QC</ci:role>
                                '</ci:test>
                                '<ci:specimen>
                                '<ci:id>SPM01</ci:id>
                                '<ci:type>LIS Mapped Sampletype</ci:type>
                                '</ci:specimen>
                                '<ci:patient id=Guid number 1 />
                                '<ci:order id= Guid number 1 />
                                '</ci:service>


                                Dim qAwosIds As New List(Of OrderTestsLISInfoDS.twksOrderTestsLISInfoRow)
                                qAwosIds = (From a As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow _
                                            In pRejectedAwosIDs.twksOrderTestsLISInfo _
                                            Select a).ToList

                                For Each Awos As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In qAwosIds

                                    Dim Service As XmlNode = Doc.CreateElement("ci", "service", ClinicalInfoSchema)

                                    ' XB 31/05/2013
                                    'Dim NowDate As XmlNode = Doc.CreateElement("ci", "date", ClinicalInfoSchema)
                                    'NowDate.InnerText = DateTime.Now.ToXSDString
                                    'Service.AppendChild(NowDate)
                                    ' XB 31/05/2013

                                    Dim Id As XmlNode = Doc.CreateElement("ci", "id", ClinicalInfoSchema)
                                    Id.InnerText = Awos.AwosID
                                    Service.AppendChild(Id)
                                    Dim Status As XmlNode = Doc.CreateElement("ci", "status", ClinicalInfoSchema)
                                    Status.InnerText = "cancelled"
                                    Service.AppendChild(Status)

                                    Dim Test As XmlNode = Doc.CreateElement("ci", "test", ClinicalInfoSchema)
                                    Dim TestId As XmlNode = Doc.CreateElement("ci", "id", ClinicalInfoSchema)
                                    TestId.InnerText = Awos.TestIDString ' Awos.TestID.ToString  ' LISMapped Test1
                                    Test.AppendChild(Id)
                                    Dim Role As XmlNode = Doc.CreateElement("ci", "role", ClinicalInfoSchema)
                                    Role.InnerText = Awos.SampleClass ' patient Or QC
                                    Test.AppendChild(Role)
                                    Service.AppendChild(Test)

                                    'specimen
                                    Dim Specimen As XmlNode = Doc.CreateElement("ci", "specimen", ClinicalInfoSchema)
                                    Dim SpecimenId As XmlNode = Doc.CreateElement("ci", "id", ClinicalInfoSchema)
                                    SpecimenId.InnerText = Awos.SpecimenID
                                    Specimen.AppendChild(SpecimenId)
                                    Dim SpecimenType As XmlNode = Doc.CreateElement("ci", "type", ClinicalInfoSchema)
                                    SpecimenType.InnerText = Awos.SampleType ' LIS Mapped Sampletype
                                    Specimen.AppendChild(SpecimenType)
                                    Service.AppendChild(Specimen)

                                    'patient
                                    Dim Patient As XmlNode = Doc.CreateElement("ci", "patient", ClinicalInfoSchema)
                                    Dim PatientID As XmlAttribute = Doc.CreateAttribute("id")
                                    PatientID.Value = Awos.ESPatientID
                                    Patient.Attributes.Append(PatientID)
                                    Service.AppendChild(Patient)

                                    'order
                                    Dim Order As XmlNode = Doc.CreateElement("ci", "order", ClinicalInfoSchema)
                                    Dim OrderID As XmlAttribute = Doc.CreateAttribute("id")
                                    OrderID.Value = Awos.ESOrderID
                                    Order.Attributes.Append(OrderID)
                                    Service.AppendChild(Order)

                                    Message.AppendChild(Service)

                                Next

                                qAwosIds = Nothing


                                'source
                                resultData = MyClass.CreateSourceNode(Doc)
                                If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                                    Dim Source As XmlNode = CType(resultData.SetDatos, XmlNode)
                                    Message.AppendChild(Source)
                                    Body.AppendChild(Message)
                                    Command.AppendChild(Body)
                                    Doc.AppendChild(Command)
                                    resultData.SetDatos = Doc.InnerXml

                                End If

                            End If

                        End If
                        'end SGM 22/03/2013

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.GetAwosRejectDelayed", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Returns the XML for Orders Results to be uploaded message
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pMessageID">Message Identifier</param>
        ''' <param name="pToUploadDS">Results to upload</param>
        ''' <param name="pHistoricFlag">True when the export is executed from Historical Results Screen</param>
        ''' <param name="pTestMappingDS"></param>
        ''' <param name="pConfigMappingDS"></param>
        ''' <param name="pResultsDS">Informed when historicalflag FALSE</param>
        ''' <param name="pResultAlarmsDS">Informed when historicalflag FALSE</param>
        ''' <param name="pHistDataDS">Only informed when called from historical results. Contains the current data in screen with the selected filters</param>
        ''' <returns>GlobalDataTO with setdatos as string with proper xml message</returns>
        ''' <remarks>
        ''' Created by:  AG 22/02/2013 - Empty, only creation
        ''' Modified by: AG 27/02/2013 - Use new parameter 'ByVal pToUploadDS As ExecutionsDS' instead of 'ByVal pAwosToUploadResultList As List(Of String)'
        '''              AG 04/03/2013 - Develop (based on GetAwosAccept method)
        '''              SG 07/03/2013 - Get the data from LIS mapping views
        '''              SG 08/03/2013 - Mount the XML Message with the obtained XML nodes (services, patients, orders)
        '''              AG 19/03/2013 - Removed parameters pCurrentAnalyzerID and pCurrentWorkSessionID
        '''                            - Added parameters pTestMappingDS, pConfigMappingDS, pResultsDS, pResultAlarmsDS
        '''              SG 19/03/2013 - Remove manualciOrderIdTags and LISCiOrderIdTags variables
        '''              SG 26/06/2013 - Do not send Nothing in case of Service Tag missing
        '''              TR 11/07/2013 - Before creating the Source Node validate if pHistoricFlac is true to set the AnalyzerID value from the History result: if 
        '''                              value is False then the AnalyzerID is empty, using the current analyzerID.  
        '''              SA 14/01/2014 - BT #1453 ==> Changes in the way the Upload Results Message is built when for a Patient Sample there are LIS Order Tests and
        '''                                           also Manual Order Tests. Note: no changes have been made for Control Samples (current process is OK)
        '''                                           NOTE: CODE HAS BEEN COMMENTED TO GENERATE THE BETA 3 OF VERSION 3.0
        ''' </remarks>
        Public Function GetOrdersResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pMessageID As String, ByVal pToUploadDS As ExecutionsDS, _
                                         ByVal pHistoricFlag As Boolean, ByVal pTestMappingDS As AllTestsByTypeDS, ByVal pConfigMappingDS As LISMappingsDS, _
                                         Optional ByVal pResultsDS As ResultsDS = Nothing, Optional ByVal pResultAlarmsDS As ResultsDS = Nothing, _
                                         Optional ByVal pHistDataDS As HisWSResultsDS = Nothing) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Build the xml Header
                        '<command type="message">
                        '<header>
                        '<id>Message Control ID</id> ‘MessageID
                        '<channel>
                        '<id>Channel Name</id> ‘Channel ID
                        '</channel>
                        '<metadata>
                        '<container>
                        '<processMode>production</processMode>
                        '<priority>0 </priority> ‘-- 1 for STATS, 0 for ROUTINE, in v2 always 0 because the service tag also contains a priority tag
                        '<transmissionMode>unsolicited</transmissionMode>
                        '<action>report</action>
                        '<object>observation</object>
                        '<date>Message datetime</date> ‘Date time
                        '</container>
                        '</metadata>
                        '</header>
                        '<body>
                        '<message>

                        Dim myXMLDoc As New XmlDocument()
                        resultData = MyClass.CreateRootElement(myXMLDoc, "message")
                        If (Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing) Then
                            Dim Command As XmlElement = CType(resultData.SetDatos, XmlElement)

                            '1) HEADER
                            resultData = MyClass.CreateHeaderNode(myXMLDoc, pMessageID, ProcessModes.production, 0, TransmissionModes.unsolicited, _
                                                                  Actions.report, MessageObjects.observation, DateTime.Now)
                            If (Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing) Then
                                Dim Header As XmlNode = CType(resultData.SetDatos, XmlNode)
                                Command.AppendChild(Header)

                                '2) BODY
                                Dim Body As XmlNode = myXMLDoc.CreateElement("body")
                                Dim BodyAttr As XmlAttribute = myXMLDoc.CreateAttribute("xmlns")
                                BodyAttr.Value = UDCSchema
                                Body.Attributes.Append(BodyAttr)

                                Dim Message As XmlNode = myXMLDoc.CreateElement("message")

                                '** BEGIN: CODE TO CHANGE FOR BT #1453 **'
                                'Using LINQ get only the Results with LISRequest not informed (NULL) or LISRequest informed as False (MANUAL ORDERS)
                                Dim manualciServiceTags As New List(Of XmlNode)
                                Dim manualciPatientIdTags As New List(Of XmlNode)

                                Dim lnqResults As New List(Of ExecutionsDS.twksWSExecutionsRow)
                                lnqResults = (From a As ExecutionsDS.twksWSExecutionsRow In pToUploadDS.twksWSExecutions _
                                             Where a.IsLISRequestNull OrElse Not a.LISRequest Select a).ToList

                                If (Not resultData.HasError AndAlso lnqResults.Count > 0) Then
                                    resultData = GetUploadManualOrdersResults(dbConnection, lnqResults, pHistoricFlag, pTestMappingDS, pConfigMappingDS, pResultsDS, _
                                                                              pResultAlarmsDS, pHistDataDS, myXMLDoc, manualciServiceTags, manualciPatientIdTags)
                                End If

                                'Using LINQ get only the Results with LISRequest informed as True (LIS ORDERS)
                                Dim LISCiServiceTags As New List(Of XmlNode)
                                Dim LISCiPatientIdTags As New List(Of XmlNode)

                                lnqResults = (From a As ExecutionsDS.twksWSExecutionsRow In pToUploadDS.twksWSExecutions _
                                              Where Not a.IsLISRequestNull AndAlso a.LISRequest Select a).ToList

                                If (Not resultData.HasError AndAlso lnqResults.Count > 0) Then
                                    resultData = GetUploadLISOrdersResults(dbConnection, lnqResults, pHistoricFlag, pTestMappingDS, pConfigMappingDS, pResultsDS, _
                                                                           pResultAlarmsDS, pHistDataDS, myXMLDoc, LISCiServiceTags, LISCiPatientIdTags)
                                End If
                                lnqResults = Nothing
                                '** END: CODE TO CHANGE FOR BT #1453 **'

                                ''** BEGIN: NEW CODE FOR BT #1453 **'
                                ''Verify the Sample Class: PATIENT or CONTROL
                                'Dim mySampleClass As String = (From a As ExecutionsDS.twksWSExecutionsRow In pToUploadDS.twksWSExecutions _
                                '                              Select a.SampleClass Distinct).ToList.First.ToString

                                'If (mySampleClass = "PATIENT") Then
                                '    'Verify if there are Order Tests requested by LIS for this Patient
                                '    Dim lnqResults As New List(Of ExecutionsDS.twksWSExecutionsRow)
                                '    lnqResults = (From a As ExecutionsDS.twksWSExecutionsRow In pToUploadDS.twksWSExecutions _
                                '                 Where Not a.IsLISRequestNull AndAlso a.LISRequest Select a).ToList

                                '    If (Not resultData.HasError AndAlso lnqResults.Count = 0) Then
                                '        'All Tests for the Patient were MANUALLY requested in BA400
                                '        '** Copy all Order Tests to a list of ExecutionsDS.twksWSExecutionsRow
                                '        lnqResults = (From a As ExecutionsDS.twksWSExecutionsRow In pToUploadDS.twksWSExecutions Select a).ToList

                                '        '** Call the function used to create Service and Patient Tags for Manual Orders
                                '        resultData = GetUploadManualOrdersResults(dbConnection, lnqResults, pHistoricFlag, pTestMappingDS, pConfigMappingDS, pResultsDS, _
                                '                                                  pResultAlarmsDS, pHistDataDS, myXMLDoc, manualciServiceTags, manualciPatientIdTags)
                                '    Else
                                '        'Verify if there are Order Tests manually requested for this Patient
                                '        lnqResults = (From a As ExecutionsDS.twksWSExecutionsRow In pToUploadDS.twksWSExecutions _
                                '                     Where a.IsLISRequestNull OrElse Not a.LISRequest Select a).ToList

                                '        If (Not resultData.HasError AndAlso lnqResults.Count = 0) Then
                                '            'All Tests for the Patient were requested by LIS
                                '            '** Copy all Order Tests to a list of ExecutionsDS.twksWSExecutionsRow
                                '            lnqResults = (From a As ExecutionsDS.twksWSExecutionsRow In pToUploadDS.twksWSExecutions Select a).ToList

                                '            '** Call the function used to create Service and Patient Tags for LIS Orders
                                '            resultData = GetUploadLISOrdersResults(dbConnection, lnqResults, pHistoricFlag, pTestMappingDS, pConfigMappingDS, pResultsDS, _
                                '                                                   pResultAlarmsDS, pHistDataDS, myXMLDoc, LISCiServiceTags, LISCiPatientIdTags)
                                '        Else
                                '            'For the Patient there are Tests requested by LIS and also Tests manually requested from BA400
                                '            '** Copy all Order Tests to a list of ExecutionsDS.twksWSExecutionsRow
                                '            lnqResults = (From a As ExecutionsDS.twksWSExecutionsRow In pToUploadDS.twksWSExecutions Select a).ToList

                                '            If (Not pHistoricFlag) Then
                                '                '** Call the function used to create Service and Patient Tags for mixed Orders in the active WorkSession
                                '                resultData = GetMIXEDOrdersResults(dbConnection, lnqResults, pTestMappingDS, pConfigMappingDS, pResultsDS, _
                                '                                                   pResultAlarmsDS, myXMLDoc, LISCiServiceTags, LISCiPatientIdTags)
                                '            Else
                                '                '** Call the function used to create Service and Patient Tags for mixed Historic Orders 
                                '                resultData = GetMIXEDOrdersResultsHIST(dbConnection, lnqResults, pTestMappingDS, pConfigMappingDS, pHistDataDS, myXMLDoc, _
                                '                                                       LISCiServiceTags, LISCiPatientIdTags)
                                '            End If
                                '        End If
                                '    End If
                                '    lnqResults = Nothing
                                'Else
                                '    'Get results of all BA400 Controls manually requested
                                '    Dim lnqResults As New List(Of ExecutionsDS.twksWSExecutionsRow)
                                '    lnqResults = (From a As ExecutionsDS.twksWSExecutionsRow In pToUploadDS.twksWSExecutions _
                                '                 Where a.IsLISRequestNull OrElse Not a.LISRequest Select a).ToList

                                '    If (Not resultData.HasError AndAlso lnqResults.Count > 0) Then
                                '        resultData = GetUploadManualOrdersResults(dbConnection, lnqResults, pHistoricFlag, pTestMappingDS, pConfigMappingDS, pResultsDS, _
                                '                                                  pResultAlarmsDS, pHistDataDS, myXMLDoc, manualciServiceTags, manualciPatientIdTags)
                                '    End If

                                '    'Get results of all BA400 Controls requested by LIS
                                '    lnqResults = (From a As ExecutionsDS.twksWSExecutionsRow In pToUploadDS.twksWSExecutions _
                                '                 Where Not a.IsLISRequestNull AndAlso a.LISRequest Select a).ToList

                                '    If (Not resultData.HasError AndAlso lnqResults.Count > 0) Then
                                '        resultData = GetUploadLISOrdersResults(dbConnection, lnqResults, pHistoricFlag, pTestMappingDS, pConfigMappingDS, pResultsDS, _
                                '                                               pResultAlarmsDS, pHistDataDS, myXMLDoc, LISCiServiceTags, LISCiPatientIdTags)
                                '    End If
                                '    lnqResults = Nothing
                                'End If
                                ''** END: NEW CODE FOR BT #1453 **'

                                '3) Add the PatientID and OrderID Tags for LIS
                                If (Not resultData.HasError) Then
                                    'Manual Service Tags
                                    For Each ServiceNode As XmlNode In manualciServiceTags
                                        Message.AppendChild(ServiceNode)
                                    Next

                                    'LIS Service Tags
                                    For Each ServiceNode As XmlNode In LISCiServiceTags
                                        Message.AppendChild(ServiceNode)
                                    Next

                                    'Manual Patient Tags
                                    For Each PatientNode As XmlNode In manualciPatientIdTags
                                        Message.AppendChild(PatientNode)
                                    Next

                                    'LIS Patient Tags
                                    For Each PatientNode As XmlNode In LISCiPatientIdTags
                                        Message.AppendChild(PatientNode)
                                    Next
                                End If

                                '4) Build the XML foot (or source)
                                '<ci:source>
                                '<ci:companyName>Company name</ci:companyName>				
                                '<ci:model>Analyzer mode</ci:model>
                                '<ci:serialNumber>Analyzer serial number</ci:serialNumber>
                                '<ci:/source>	
                                '</message>		
                                '</body>
                                '</command>
                                If (Not resultData.HasError) Then
                                    Dim myAnalyzerID As String = String.Empty
                                    If (pHistoricFlag) Then myAnalyzerID = pHistDataDS.vhisWSResults(0).AnalyzerID

                                    resultData = MyClass.CreateSourceNode(myXMLDoc, myAnalyzerID)
                                    If (Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing) Then
                                        Dim Source As XmlNode = CType(resultData.SetDatos, XmlNode)

                                        Message.AppendChild(Source)
                                        Body.AppendChild(Message)

                                        'SG 26/06/2013 - Do not send Nothing in case of Service Tag missing
                                        If (MyClass.xmlHelper.QueryXmlNodeList(Body, "message/ci:service").Count = 0) Then
                                            resultData.SetDatos = Nothing
                                        Else
                                            Command.AppendChild(Body)
                                            myXMLDoc.AppendChild(Command)
                                            resultData.SetDatos = myXMLDoc.InnerXml
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.GetOrdersResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Generate the XML for Manual Orders (Patient and Control)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pManualResults"></param>
        ''' <param name="pHistoricalFlag"></param>
        ''' <param name="pTestMappingDS">Test mapping information</param>
        ''' <param name="pConfigMappingDS">Configuration mapping information</param>
        ''' <param name="pResultsDS">Current WS Results</param>
        ''' <param name="pResultAlarmsDS">Current WS Result Alarms</param>
        ''' <param name="pHistResultDataDS">Historical Results in Historical screen</param>
        ''' <param name="xmlDoc">XML document which the tags will become to</param>
        ''' <param name="pCiServiceIdTags">XML Service Tags to be filled</param>
        ''' <param name="pCiPatientIdTags">XML Patient Tags to be filled</param>
        ''' <returns>GlobalDataTO containing success/error information + ByRef parameters</returns>
        ''' <remarks>
        ''' Created by:  AG 04/03/2013 - Empty, only skeleton
        ''' Modified by: SG 07/03/2013 - Obtained required data for Results update
        '''              XB 13/03/2013 - Added Controls functionality
        '''              SG 19/03/2013 - Removed pCiOrderIdTags
        '''              TR 04/04/2013 - Validated if Patient or Control Result can be upload base on the user configuration parameters LIS_UPLOAD_UNSOLICITED_PAT_RES, 
        '''                              LIS_UPLOAD_UNSOLICITED_QC_RES.
        '''              SG 25/04/2013 - Assigned current Mapping names to history data in case of Manual orders
        '''              SA 09/01/2014 - BT #1407 ==> Added to code to avoid export twice results of Calculated Tests (patch copied from function GetUploadLISOrdersResults)
        '''              AG 29/09/2014 - BA-1440 part1 - Inform the LISMappingError flag calling CreateServiceNode
        ''' </remarks>
        Private Function GetUploadManualOrdersResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pManualResults As List(Of ExecutionsDS.twksWSExecutionsRow), _
                                                      ByVal pHistoricalFlag As Boolean, ByVal pTestMappingDS As AllTestsByTypeDS, ByVal pConfigMappingDS As LISMappingsDS, _
                                                      ByVal pResultsDS As ResultsDS, ByVal pResultAlarmsDS As ResultsDS, ByVal pHistResultDataDS As HisWSResultsDS, _
                                                      ByRef xmlDoc As XmlDocument, ByRef pCiServiceIdTags As List(Of XmlNode), _
                                                      ByRef pCiPatientIdTags As List(Of XmlNode)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim myLogAcciones As New ApplicationLogManager()

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        pCiServiceIdTags = New List(Of XmlNode)
                        pCiPatientIdTags = New List(Of XmlNode)

                        Dim myPatients As New List(Of String)
                        Dim myOrders As New List(Of String)

                        'Differentiate between patients and controls
                        Dim lnqExecutions As New List(Of ExecutionsDS.twksWSExecutionsRow)

                        'TR 04/04/2013 - Check if results of Manual Patient Orders can be uploaded to LIS
                        Dim myLISUploadPatResults As Boolean = False
                        Dim myLISUploadQCResults As Boolean = False

                        Dim myUserSettingDS As New UserSettingDS
                        Dim myUserSettingsDelegate As New UserSettingsDelegate
                        resultData = myUserSettingsDelegate.ReadBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.LIS_UPLOAD_UNSOLICITED_PAT_RES.ToString())
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            myUserSettingDS = DirectCast(resultData.SetDatos, UserSettingDS)
                            If (myUserSettingDS.tcfgUserSettings.Count > 0) Then
                                myLISUploadPatResults = CBool(myUserSettingDS.tcfgUserSettings(0).CurrentValue)
                            End If
                        End If

                        'Validate if patient upload is Allowed.
                        If (myLISUploadPatResults) Then
                            'SG 07/03/2013
                            '********* PATIENTS **********
                            'AG - we cannot use "patient" because the query form export returns N, U or Q (see method, twksResultsDAO.GetResultsToExport & thisWSResultsDAO.GetResultsToExportFromHIST)
                            'Where a.SampleClass = "PATIENT" Select a).ToList
                            If (Not pHistoricalFlag) Then
                                lnqExecutions = (From a As ExecutionsDS.twksWSExecutionsRow In pManualResults _
                                                Where a.SampleClass = "PATIENT" _
                                               Select a Order By a.StatFlag Descending).ToList

                            Else
                                lnqExecutions = (From a As ExecutionsDS.twksWSExecutionsRow In pManualResults _
                                                Where a.SampleClass = "N" OrElse a.SampleClass = "U" _
                                               Select a Order By a.SampleClass Descending).ToList
                            End If
                        Else
                            lnqExecutions.Clear()
                        End If
                        'TR 04/04/2013 -END.

                        Dim isCalcResultAlreadyAdded As Boolean = False
                        Dim calcs As New List(Of ExecutionsDS.twksWSExecutionsRow)
                        Dim lnqResultsAlreadyAdded As New List(Of ExecutionsDS.twksWSExecutionsRow)

                        For Each exeRow As ExecutionsDS.twksWSExecutionsRow In lnqExecutions
                            'BT #1407 - Verify if the OrderTestID is duplicated to avoid upload the same result twice 
                            '           (due to Order Tests of Calculated Tests are duplicated in the entry list pManualResults)
                            isCalcResultAlreadyAdded = False
                            If (exeRow.TestType = "CALC") Then
                                calcs = (From a As ExecutionsDS.twksWSExecutionsRow In lnqResultsAlreadyAdded _
                                        Where a.OrderTestID = exeRow.OrderTestID Select a).ToList
                                isCalcResultAlreadyAdded = (calcs.Count > 0)
                            End If

                            If (Not isCalcResultAlreadyAdded) Then
                                'Get common data information
                                Dim myResulDataTime As Date
                                Dim myReferenceRanges As TestRefRangesDS = Nothing
                                Dim myResult As ResultsDS.vwksResultsRow = Nothing
                                Dim myHisResult As HisWSResultsDS.vhisWSResultsRow = Nothing
                                Dim myAlarmResults As List(Of ResultsDS.vwksResultsAlarmsRow) = Nothing

                                resultData = GetRequiredDataForResultsUpload(pDBConnection, exeRow.OrderTestID, exeRow.RerunNumber, exeRow.TestType, pHistoricalFlag, _
                                                                             pResultsDS, pResultAlarmsDS, pHistResultDataDS, myResulDataTime, myReferenceRanges, _
                                                                             myResult, myHisResult, myAlarmResults)
                                If (resultData.HasError) Then Exit Try

                                'Get required Manual LIS data information
                                Dim mySpecimenID As String = String.Empty
                                Dim myPatientID As String = String.Empty
                                Dim myOrderID As String = String.Empty

                                If (Not pHistoricalFlag) Then
                                    'Specimen & Patient
                                    If (Not myResult.IsSampleIDNull) Then
                                        mySpecimenID = myResult.SampleID
                                        myPatientID = myResult.SampleID

                                    ElseIf (Not myResult.IsPatientIDNull) Then
                                        mySpecimenID = myResult.PatientID
                                        myPatientID = myResult.PatientID
                                    End If

                                    'Order
                                    If (Not myResult.IsOrderIDNull) Then
                                        myOrderID = myResult.OrderID.ToString
                                    End If
                                Else
                                    'SGM 25/04/2013
                                    If (myHisResult.IsTestTypeNull) Then
                                        With myHisResult
                                            .BeginEdit()
                                            .TestType = exeRow.TestType
                                            .EndEdit()
                                        End With
                                    End If

                                    If (Not myHisResult.IsPatientIDNull) Then
                                        'Specimen & Patient
                                        If (Not myHisResult.IsPatientIDNull) Then
                                            myPatientID = myHisResult.PatientID
                                            mySpecimenID = myHisResult.PatientID
                                        Else
                                            myPatientID = Guid.NewGuid.ToString
                                            mySpecimenID = Guid.NewGuid.ToString
                                        End If
                                    End If

                                    'Order
                                    myOrderID = myHisResult.HistOrderTestID.ToString
                                    'END SGM 25/04/2013
                                End If

                                'Add to Patients list
                                If (myPatientID.Length > 0 AndAlso Not myPatients.Contains(myPatientID)) Then
                                    myPatients.Add(myPatientID)
                                End If

                                'Add to Orders list
                                If (Not myOrders.Contains(myOrderID)) Then
                                    myOrders.Add(myOrderID)
                                End If

                                resultData = MyClass.CreateServiceNode(xmlDoc, pTestMappingDS, pConfigMappingDS, TestSampleClass.patient, myResulDataTime, _
                                                                       myAlarmResults, myResult, myHisResult, myReferenceRanges, mySpecimenID, myPatientID, _
                                                                       myOrderID, True, Nothing, "", exeRow.LISMappingError) 'AG 24/04/2013 - Add AwosID as an empty string

                                If (resultData.HasError) Then
                                    Exit Try
                                ElseIf (resultData.SetDatos IsNot Nothing) Then
                                    Dim myServiceNode As XmlNode = TryCast(resultData.SetDatos, XmlNode)

                                    If (myServiceNode IsNot Nothing) Then
                                        pCiServiceIdTags.Add(myServiceNode)
                                        lnqResultsAlreadyAdded.Add(exeRow)
                                    End If
                                End If
                            End If
                        Next
                        calcs = Nothing
                        lnqResultsAlreadyAdded = Nothing

                        '********* CONTROLS **********
                        'TR 04/04/2013 - Check if results of Manual QC Orders can be uploaded to LIS
                        resultData = myUserSettingsDelegate.ReadBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.LIS_UPLOAD_UNSOLICITED_QC_RES.ToString())
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            myUserSettingDS = DirectCast(resultData.SetDatos, UserSettingDS)
                            If (myUserSettingDS.tcfgUserSettings.Count > 0) Then
                                myLISUploadQCResults = CBool(myUserSettingDS.tcfgUserSettings(0).CurrentValue)
                            End If
                        End If

                        'TR 04/03/21013 - Validate if the QC Result can be Uploaded.
                        If (myLISUploadQCResults) Then
                            'NOTE: Historical controls CANNOT be exported
                            lnqExecutions = (From a As ExecutionsDS.twksWSExecutionsRow In pManualResults _
                                            Where a.SampleClass = "CTRL" Select a Order By a.StatFlag Descending).ToList
                        Else
                            lnqExecutions.Clear()
                        End If

                        For Each exeRow As ExecutionsDS.twksWSExecutionsRow In lnqExecutions
                            'Get common data information
                            Dim myResulDataTime As Date
                            Dim myReferenceRanges As TestRefRangesDS = Nothing
                            Dim myResult As ResultsDS.vwksResultsRow = Nothing
                            Dim myHisResult As HisWSResultsDS.vhisWSResultsRow = Nothing
                            Dim myAlarmResults As List(Of ResultsDS.vwksResultsAlarmsRow) = Nothing

                            resultData = GetRequiredDataForResultsUpload(pDBConnection, exeRow.OrderTestID, exeRow.RerunNumber, exeRow.TestType, pHistoricalFlag, _
                                                                         pResultsDS, pResultAlarmsDS, pHistResultDataDS, myResulDataTime, myReferenceRanges, _
                                                                         myResult, myHisResult, myAlarmResults)
                            If (resultData.HasError) Then Exit Try

                            'Get required Manual LIS data information
                            Dim mySpecimenID As String = String.Empty
                            Dim myPatientID As String = String.Empty
                            Dim myOrderID As String = String.Empty

                            'Specimen 
                            If (Not myResult.IsControlNameNull) Then
                                mySpecimenID = myResult.ControlName
                            End If

                            'Order
                            If (Not myResult.IsOrderIDNull) Then
                                myPatientID = myResult.OrderID.ToString
                                myOrderID = myResult.OrderID.ToString
                            End If

                            'Add to Orders list
                            If (Not myOrders.Contains(myOrderID)) Then
                                myOrders.Add(myOrderID)
                            End If

                            'Get Controls Info
                            Dim myControlsDS As New ControlsDS
                            Dim myControlsDelegate As New ControlsDelegate

                            resultData = myControlsDelegate.GetAll(Nothing)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                myControlsDS = DirectCast(resultData.SetDatos, ControlsDS)
                            End If
                            If (resultData.HasError) Then Exit Try

                            'Create Service Node
                            resultData = MyClass.CreateServiceNode(xmlDoc, pTestMappingDS, pConfigMappingDS, TestSampleClass.QC, myResulDataTime, _
                                                                   myAlarmResults, myResult, myHisResult, myReferenceRanges, mySpecimenID, myPatientID, _
                                                                   myOrderID, True, myControlsDS, "", exeRow.LISMappingError) 'AG 24/04/2013 - Add AwosID as an empty string

                            If (resultData.HasError) Then
                                Exit Try
                            ElseIf (resultData.SetDatos IsNot Nothing) Then
                                Dim myServiceNode As XmlNode = TryCast(resultData.SetDatos, XmlNode)
                                If (myServiceNode IsNot Nothing) Then
                                    pCiServiceIdTags.Add(myServiceNode)
                                End If
                            End If
                        Next

                        lnqExecutions = Nothing
                        resultData.SetDatos = True
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.GetUploadManualOrdersResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Generate XML for LIS Orders (Patient and Control)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pLISResults"></param>
        ''' <param name="pHistoricalFlag"></param>
        ''' <param name="pTestMappingDS">Test mapping information</param>
        ''' <param name="pConfigMappingDS">Confg mapping information</param>
        ''' <param name="pResultsDS">Current WS results</param>
        ''' <param name="pResultAlarmsDS">Current WS result alarms</param>
        ''' <param name="pHistResultDataDS">Historical results in historical screen</param>
        ''' <param name="xmlDoc"></param>
        ''' <param name="pCiServiceList">list of XMLNodes corresponding to Service items to return</param>
        ''' <param name="pCiPatientList">list of XMLNodes corresponding to Patient items to return</param>
        ''' <returns>GlobalDataTo (error or not) + byref parameters</returns>
        ''' <remarks>
        ''' Created by  AG 04/03/2013 - Empty, only skeleton
        ''' Updated by  XB 07/03/2013 - Implement it ! - REMARK : For March integration PENDING DEFINE THE HISTORICAL LIS DATA MODE, so LIS Orders from Historical are not implemented
        ''' Modified by XB 13/03/2013 - Add Controls functionality
        ''' Modified by SG 19/03/2013 - Remove pCiOrderList
        ''' Modified by SG 10/04/2013 - In patient and control results loop when the AwosID is null add 
        ''' Modified by SG 25/04/2013 - Assign saved Mapping names to history data in case of LIS orders. Obtain data from 'thisWSOrderTests'
        ''' Modified by SG 27/06/2013 - In case of Calc Tests, upload unique result for OrderTEstID
        ''' Modified by SG 25/07/2013 - In case of the executions have not AwosID, they are discarded
        ''' AG 29/09/2014 - BA-1440 part1 - Inform the LISMappingError flag calling CreateServiceNode
        ''' </remarks>
        Private Function GetUploadLISOrdersResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pLISResults As List(Of ExecutionsDS.twksWSExecutionsRow), _
                                                   ByVal pHistoricalFlag As Boolean, ByVal pTestMappingDS As AllTestsByTypeDS, ByVal pConfigMappingDS As LISMappingsDS, _
                                                   ByVal pResultsDS As ResultsDS, ByVal pResultAlarmsDS As ResultsDS, ByVal pHistResultDataDS As HisWSResultsDS, _
                                                   ByRef xmlDoc As XmlDocument, ByRef pCiServiceList As List(Of XmlNode), ByRef pCiPatientList As List(Of XmlNode)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'SGM 19/03/2013 - create local DS for storing coupple ESPatientID - ESOrderID
                        Dim myPatientsOrdersDS As New OrderTestsLISInfoDS

                        pCiServiceList = New List(Of XmlNode)
                        pCiPatientList = New List(Of XmlNode)

                        Dim myPatients As New List(Of String)
                        Dim myOrders As New List(Of String)

                        'Differentiate between patients and controls
                        Dim lnqResults As New List(Of ExecutionsDS.twksWSExecutionsRow)
                        Dim otLisInfoDlg As New OrderTestsLISInfoDelegate
                        Dim otLisInfoDS As New OrderTestsLISInfoDS

                        'SG 10/04/2013 - the items with missing AwosID are collected and updated by their ExportStatus = "NOTSENT"
                        Dim myResultsNotSentDS As New ResultsDS
                        Dim myHisResultsNotSentDS As New ResultsDS

                        'Get the LIS patients
                        If (Not pHistoricalFlag) Then
                            lnqResults = (From a As ExecutionsDS.twksWSExecutionsRow In pLISResults _
                                         Where a.SampleClass = "PATIENT" _
                                        Select a Order By a.StatFlag Descending).ToList
                        Else
                            lnqResults = (From a As ExecutionsDS.twksWSExecutionsRow In pLISResults _
                                         Where a.SampleClass = "N" OrElse a.SampleClass = "U" _
                                        Select a Order By a.SampleClass Descending).ToList
                        End If

                        Dim lnqResultsAlreadyAdded As New List(Of ExecutionsDS.twksWSExecutionsRow) 'SGM 27/06/2013
                        For Each item As ExecutionsDS.twksWSExecutionsRow In lnqResults
                            'SG 27/06/2013 - In case of Calc Tests, upload unique result for OrderTEstID
                            Dim isCalcResultAlreadyAdded As Boolean = False
                            If (item.TestType = "CALC") Then
                                Dim calcs As New List(Of ExecutionsDS.twksWSExecutionsRow)
                                calcs = (From a As ExecutionsDS.twksWSExecutionsRow In lnqResultsAlreadyAdded _
                                        Where a.OrderTestID = item.OrderTestID Select a).ToList
                                isCalcResultAlreadyAdded = (calcs.Count > 0)
                                calcs = Nothing
                            End If

                            If (Not isCalcResultAlreadyAdded) Then
                                'Get the ordertest LIS info
                                If (Not pHistoricalFlag) Then
                                    resultData = otLisInfoDlg.Read(dbConnection, item.OrderTestID, item.RerunNumber)
                                Else
                                    'SGM 24/04/2013 - Obtain data from History
                                    otLisInfoDS.Clear()
                                    If (Not item.IsSampleClassNull And (item.SampleClass = "N" Or item.SampleClass = "U")) Then
                                        With item
                                            .BeginEdit()
                                            .SampleClass = "PATIENT"
                                            .EndEdit()
                                        End With
                                    End If

                                    Dim myHisWSOTDelegate As New HisWSOrderTestsDelegate
                                    resultData = myHisWSOTDelegate.ReadAll(dbConnection, MyClass.AnalyzerSerialNumberAttr)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        Dim myHisWSOTDS As HisWSOrderTestsDS = TryCast(resultData.SetDatos, HisWSOrderTestsDS)
                                        Dim myHisData As New List(Of HisWSOrderTestsDS.thisWSOrderTestsRow)

                                        myHisData = (From a As HisWSOrderTestsDS.thisWSOrderTestsRow In myHisWSOTDS.thisWSOrderTests _
                                                     Where a.HistOrderTestID = item.OrderTestID).ToList

                                        For Each h As HisWSOrderTestsDS.thisWSOrderTestsRow In myHisData
                                            Dim myRow As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow = otLisInfoDS.twksOrderTestsLISInfo.NewtwksOrderTestsLISInfoRow
                                            With myRow
                                                .BeginEdit()
                                                .OrderTestID = h.HistOrderTestID
                                                .RerunNumber = 1
                                                .AwosID = h.AwosID
                                                .SpecimenID = h.SpecimenID
                                                .ESOrderID = h.ESOrderID
                                                .ESPatientID = h.ESPatientID
                                                .LISOrderID = h.LISOrderID
                                                .LISPatientID = h.LISPatientID
                                                .EndEdit()
                                            End With
                                            otLisInfoDS.twksOrderTestsLISInfo.AddtwksOrderTestsLISInfoRow(myRow)
                                        Next
                                        otLisInfoDS.AcceptChanges()
                                        resultData.SetDatos = otLisInfoDS

                                        myHisData = Nothing
                                    End If
                                    'SGM 24/04/2013
                                End If
                                'SGM 24/04/2013

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    otLisInfoDS = DirectCast(resultData.SetDatos, OrderTestsLISInfoDS)

                                    If (otLisInfoDS.twksOrderTestsLISInfo.Rows.Count > 0) Then
                                        If (Not otLisInfoDS.twksOrderTestsLISInfo(0).IsAwosIDNull) Then 'Generate xml
                                            'Get common data information
                                            Dim myResulDataTime As Date
                                            Dim myReferenceRanges As TestRefRangesDS = Nothing
                                            Dim myResult As ResultsDS.vwksResultsRow = Nothing
                                            Dim myHisResult As HisWSResultsDS.vhisWSResultsRow = Nothing
                                            Dim myAlarmResults As List(Of ResultsDS.vwksResultsAlarmsRow) = Nothing

                                            resultData = GetRequiredDataForResultsUpload(pDBConnection, _
                                                                                         item.OrderTestID, _
                                                                                         item.RerunNumber, _
                                                                                         item.TestType, _
                                                                                         pHistoricalFlag, _
                                                                                         pResultsDS, _
                                                                                         pResultAlarmsDS, _
                                                                                         pHistResultDataDS, _
                                                                                         myResulDataTime, _
                                                                                         myReferenceRanges, _
                                                                                         myResult, _
                                                                                         myHisResult, _
                                                                                         myAlarmResults)
                                            If (resultData.HasError) Then
                                                Exit Try
                                            End If

                                            'Get required LIS data information
                                            Dim mySpecimenID As String = ""
                                            Dim myLISPatientID As String = ""
                                            Dim myLISOrderID As String = ""
                                            Dim myESPatientID As String = ""
                                            Dim myESOrderID As String = ""

                                            mySpecimenID = otLisInfoDS.twksOrderTestsLISInfo(0).SpecimenID
                                            myLISPatientID = otLisInfoDS.twksOrderTestsLISInfo(0).LISPatientID
                                            myLISOrderID = otLisInfoDS.twksOrderTestsLISInfo(0).LISOrderID
                                            myESPatientID = otLisInfoDS.twksOrderTestsLISInfo(0).ESPatientID
                                            myESOrderID = otLisInfoDS.twksOrderTestsLISInfo(0).ESOrderID

                                            'SGM 19/03/2013 - Verify if ESPatientID-ESOrderID do not exist in the local DS
                                            Dim qOTs As List(Of OrderTestsLISInfoDS.twksOrderTestsLISInfoRow)
                                            qOTs = (From a As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In myPatientsOrdersDS.twksOrderTestsLISInfo _
                                                   Where a.ESPatientID = myESPatientID _
                                                 AndAlso a.ESOrderID = myESOrderID Select a).ToList

                                            If (qOTs.Count = 0) Then
                                                Dim myRow As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow = myPatientsOrdersDS.twksOrderTestsLISInfo.NewtwksOrderTestsLISInfoRow

                                                With myRow
                                                    .BeginEdit()
                                                    .ESPatientID = myESPatientID
                                                    .ESOrderID = myESOrderID
                                                    .LISPatientID = myLISPatientID
                                                    .LISOrderID = myLISOrderID
                                                    .EndEdit()
                                                End With
                                                myPatientsOrdersDS.twksOrderTestsLISInfo.AddtwksOrderTestsLISInfoRow(myRow)
                                                myPatientsOrdersDS.AcceptChanges()
                                            End If

                                            Dim myRole As TestSampleClass
                                            If (Not item.IsExternalQCNull AndAlso item.ExternalQC) Then
                                                myRole = TestSampleClass.QC
                                            Else
                                                myRole = TestSampleClass.patient
                                            End If

                                            'Create XML node 'SERVICE'
                                            resultData = MyClass.CreateServiceNode(xmlDoc, _
                                                                                   pTestMappingDS, _
                                                                                   pConfigMappingDS, _
                                                                                   myRole, _
                                                                                   myResulDataTime, _
                                                                                   myAlarmResults, _
                                                                                   myResult, _
                                                                                   myHisResult, _
                                                                                   myReferenceRanges, _
                                                                                   mySpecimenID, _
                                                                                   myESPatientID, _
                                                                                   myESOrderID, _
                                                                                   False, _
                                                                                   Nothing, otLisInfoDS.twksOrderTestsLISInfo(0).AwosID, item.LISMappingError) 'AG 24/04/2013 - add awosid

                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                Dim myServiceNode As XmlNode = CType(resultData.SetDatos, XmlNode)

                                                'Add to List of SERVICE nodes
                                                pCiServiceList.Add(myServiceNode)
                                                lnqResultsAlreadyAdded.Add(item) 'SGM 27/06/2013
                                            Else
                                                Exit Try
                                            End If
                                        Else
                                            'SGM 10/04/2013
                                            'The result field ExportStatus must be updated from SENDING to NOTSENT
                                            If (Not pHistoricalFlag) Then
                                                myResultsNotSentDS.twksResults.ImportRow(otLisInfoDS.twksOrderTestsLISInfo(0))
                                                myResultsNotSentDS.AcceptChanges()
                                            Else
                                                myHisResultsNotSentDS.twksResults.ImportRow(otLisInfoDS.twksOrderTestsLISInfo(0))
                                                myHisResultsNotSentDS.AcceptChanges()
                                            End If
                                            'END SGM 10/04/2013
                                        End If
                                    Else
                                        'SGM 25/07/2013
                                        'The result field ExportStatus must be updated from SENDING to NOTSENT
                                        If (Not pHistoricalFlag) Then
                                            myResultsNotSentDS.twksResults.ImportRow(item)
                                            myResultsNotSentDS.AcceptChanges()
                                        Else
                                            myHisResultsNotSentDS.twksResults.ImportRow(item)
                                            myHisResultsNotSentDS.AcceptChanges()
                                        End If
                                        'END SGM 25/07/2013
                                    End If
                                End If
                            End If
                            'END SGM 27/06/2013
                        Next

                        'SGM 19/03/2013 - Create list of patient nodes
                        resultData = MyClass.CreatePatientNodes(xmlDoc, myPatientsOrdersDS)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            pCiPatientList = TryCast(resultData.SetDatos, List(Of XmlNode))
                        Else
                            Exit Try
                        End If

                        'Get the LIS controls
                        lnqResults = (From a As ExecutionsDS.twksWSExecutionsRow In pLISResults _
                                     Where a.SampleClass = "CTRL" Select a).ToList

                        For Each item As ExecutionsDS.twksWSExecutionsRow In lnqResults
                            'Get the ordertest LIS info
                            resultData = otLisInfoDlg.Read(dbConnection, item.OrderTestID, item.RerunNumber)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                otLisInfoDS = DirectCast(resultData.SetDatos, OrderTestsLISInfoDS)

                                If (otLisInfoDS.twksOrderTestsLISInfo.Rows.Count > 0) Then
                                    If (Not otLisInfoDS.twksOrderTestsLISInfo(0).IsAwosIDNull) Then 'Generate xml
                                        'Get common data information
                                        Dim myResulDataTime As Date
                                        Dim myReferenceRanges As TestRefRangesDS = Nothing
                                        Dim myResult As ResultsDS.vwksResultsRow = Nothing
                                        Dim myHisResult As HisWSResultsDS.vhisWSResultsRow = Nothing
                                        Dim myAlarmResults As List(Of ResultsDS.vwksResultsAlarmsRow) = Nothing

                                        resultData = GetRequiredDataForResultsUpload(pDBConnection, _
                                                                                     item.OrderTestID, _
                                                                                     item.RerunNumber, _
                                                                                     item.TestType, _
                                                                                     pHistoricalFlag, _
                                                                                     pResultsDS, _
                                                                                     pResultAlarmsDS, _
                                                                                     pHistResultDataDS, _
                                                                                     myResulDataTime, _
                                                                                     myReferenceRanges, _
                                                                                     myResult, _
                                                                                     myHisResult, _
                                                                                     myAlarmResults)
                                        If (resultData.HasError) Then
                                            Exit Try
                                        End If

                                        'Get required LIS data information
                                        Dim mySpecimenID As String = ""
                                        Dim myPatientID As String = ""
                                        Dim myOrderID As String = ""

                                        If (Not myResult.IsControlNameNull) Then
                                            mySpecimenID = myResult.ControlName
                                        End If

                                        If (Not otLisInfoDS.twksOrderTestsLISInfo(0).IsESPatientIDNull) Then
                                            myPatientID = otLisInfoDS.twksOrderTestsLISInfo(0).ESPatientID

                                        ElseIf (Not otLisInfoDS.twksOrderTestsLISInfo(0).IsESOrderIDNull) Then
                                            myPatientID = otLisInfoDS.twksOrderTestsLISInfo(0).ESOrderID
                                        End If

                                        myOrderID = otLisInfoDS.twksOrderTestsLISInfo(0).ESOrderID

                                        'Get Controls Info
                                        Dim myControlsDS As New ControlsDS
                                        Dim myControlsDelegate As New ControlsDelegate

                                        resultData = myControlsDelegate.GetAll(Nothing)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            myControlsDS = DirectCast(resultData.SetDatos, ControlsDS)
                                        End If

                                        If (resultData.HasError) Then
                                            Exit Try
                                        End If

                                        'Create XML node 'SERVICE'
                                        resultData = MyClass.CreateServiceNode(xmlDoc, _
                                                                               pTestMappingDS, _
                                                                               pConfigMappingDS, _
                                                                               TestSampleClass.QC, _
                                                                               myResulDataTime, _
                                                                               myAlarmResults, _
                                                                               myResult, _
                                                                               myHisResult, _
                                                                               myReferenceRanges, _
                                                                               mySpecimenID, _
                                                                               myPatientID, _
                                                                               myOrderID, _
                                                                               False, _
                                                                               myControlsDS, otLisInfoDS.twksOrderTestsLISInfo(0).AwosID, item.LISMappingError) 'AG 24/04/2013 - add awosid)

                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            Dim myServiceNode As XmlNode = CType(resultData.SetDatos, XmlNode)
                                            'Add to List of SERVICE nodes
                                            pCiServiceList.Add(myServiceNode)
                                        Else
                                            Exit Try
                                        End If
                                    Else
                                        'SGM 10/04/2013
                                        'The result field ExportStatus must be updated from SENDING to NOTSENT
                                        If (Not pHistoricalFlag) Then
                                            myResultsNotSentDS.twksResults.ImportRow(otLisInfoDS.twksOrderTestsLISInfo(0))
                                            myResultsNotSentDS.AcceptChanges()
                                        Else
                                            myHisResultsNotSentDS.twksResults.ImportRow(otLisInfoDS.twksOrderTestsLISInfo(0))
                                            myHisResultsNotSentDS.AcceptChanges()
                                        End If
                                        'END SGM 10/04/2013
                                    End If
                                Else
                                    'SGM 25/07/2013
                                    'The result field ExportStatus must be updated from SENDING to NOTSENT
                                    If (Not pHistoricalFlag) Then
                                        myResultsNotSentDS.twksResults.ImportRow(item)
                                        myResultsNotSentDS.AcceptChanges()
                                    Else
                                        myHisResultsNotSentDS.twksResults.ImportRow(item)
                                        myHisResultsNotSentDS.AcceptChanges()
                                    End If
                                    'END SGM 25/07/2013
                                End If
                            End If
                        Next
                        lnqResults = Nothing

                        'SG 10/04/2013 - the items with missing AwosID are collected and updated by their ExportStatus = "NOTSENT"
                        If (myResultsNotSentDS.twksResults.Rows.Count > 0) Then
                            Dim myResultsDelegate As New ResultsDelegate
                            myResultsDelegate.UpdateExportStatus(Nothing, myResultsNotSentDS, "NOTSENT")
                        End If

                        If (myHisResultsNotSentDS.twksResults.Rows.Count > 0) Then
                            Dim myHisResultsDelegate As New HisWSResultsDelegate
                            myHisResultsDelegate.UpdateExportStatus(Nothing, myHisResultsNotSentDS, "NOTSENT")
                        End If
                        'END SG 10/04/2013
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.GetUploadLISOrdersResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Upload Results for a PATIENT SAMPLE that has some Order Tests requested by LIS and some Order Tests manually requested.
        ''' Created to solve issue reported as BT #1453: for Manual Order Tests, the LISPatientID has to be informed in the Patient Node, and the SpecimenID has to 
        ''' be informed with the LIS Barcode of the tube with the SampleType used to execute the Test and get the result
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pLISResults">List of rows of a typed DataSet ExecutionsDS containing all Manual and LIS Order Tests for an specific PATIENT Sample</param>
        ''' <param name="pTestMappingDS">Typed DataSet AllTestsByTypeDS containing all defined LIS Test Mappings</param>
        ''' <param name="pConfigMappingDS">Typed DataSet LISMappingsDS containing all defined LIS Configuration settings</param>
        ''' <param name="pResultsDS">Typed DataSet ResultsDS containing the group of results to upload: results for all OrderTestID/RerunNumber contained in 
        '''                          pLISResults parameter</param>
        ''' <param name="pResultAlarmsDS">Typed DataSet ResultAlarmsDS containing the group of alarms raised for the results to upload</param>
        ''' <param name="xmlDoc">ByRef parameter: XML Document in which the Service and Patient Nodes should be attached</param>
        ''' <param name="pCiServiceList">ByRef parameter: List of Service XML Nodes</param>
        ''' <param name="pCiPatientList">ByRef parameter: List of Patient XML Nodes</param>
        ''' <returns>All ByRef parameters plus a GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 14/01/2014
        ''' AG 29/09/2014 - BA-1440 part1 - Inform the LISMappingError flag calling CreateServiceNode
        ''' </remarks>
        Private Function GetMIXEDOrdersResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pLISResults As List(Of ExecutionsDS.twksWSExecutionsRow), _
                                               ByVal pTestMappingDS As AllTestsByTypeDS, ByVal pConfigMappingDS As LISMappingsDS, ByVal pResultsDS As ResultsDS, _
                                               ByVal pResultAlarmsDS As ResultsDS, ByRef xmlDoc As XmlDocument, ByRef pCiServiceList As List(Of XmlNode), _
                                               ByRef pCiPatientList As List(Of XmlNode)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Initialize the lists of Service and Patient Nodes
                        pCiServiceList = New List(Of XmlNode)
                        pCiPatientList = New List(Of XmlNode)

                        'Create local DS for storing pairs of ESPatientID - ESOrderID
                        Dim myPatientsOrdersDS As New OrderTestsLISInfoDS

                        'Get all Patient Order Tests sorted by SampleType and LISRequest (DESC, to process first the OrderTests requested by LIS)
                        Dim lnqResults As List(Of ExecutionsDS.twksWSExecutionsRow)
                        lnqResults = (From a As ExecutionsDS.twksWSExecutionsRow In pLISResults _
                                     Where a.SampleClass = "PATIENT" _
                                    Select a Order By a.SampleType, a.LISRequest Descending).ToList

                        If (lnqResults.Count > 0) Then
                            Dim myPatientID As String = lnqResults.First.PatientID

                            'Verify if results of Manual Patient Orders can be uploaded to LIS
                            Dim myLISUploadManualResults As Boolean = False
                            Dim myUserSettingsDelegate As New UserSettingsDelegate

                            resultData = myUserSettingsDelegate.ReadBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.LIS_UPLOAD_UNSOLICITED_PAT_RES.ToString())
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim myUserSettingDS As UserSettingDS = DirectCast(resultData.SetDatos, UserSettingDS)
                                If (myUserSettingDS.tcfgUserSettings.Count > 0) Then myLISUploadManualResults = CBool(myUserSettingDS.tcfgUserSettings(0).CurrentValue)
                            End If

                            'Get information of all Order Tests requested by LIS for this Patient
                            Dim otLisInfoDS As New OrderTestsLISInfoDS
                            Dim otLisInfoDlg As New OrderTestsLISInfoDelegate

                            resultData = otLisInfoDlg.GetLISInfoByLISPatient(dbConnection, myPatientID)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                otLisInfoDS = DirectCast(resultData.SetDatos, OrderTestsLISInfoDS)

                                'Declare all needed variables
                                Dim myAwosID As String = String.Empty
                                Dim mySpecimenID As String = String.Empty
                                Dim myLISPatientID As String = String.Empty
                                Dim myLISOrderID As String = String.Empty
                                Dim myESPatientID As String = String.Empty
                                Dim myESOrderID As String = String.Empty
                                Dim searchLISInformation As Boolean = False
                                Dim isCalcResultAlreadyAdded As Boolean = False
                                Dim calcs As List(Of ExecutionsDS.twksWSExecutionsRow) = Nothing
                                Dim lnqLISInformation As List(Of OrderTestsLISInfoDS.twksOrderTestsLISInfoRow)
                                Dim lnqResultsAlreadyAdded As New List(Of ExecutionsDS.twksWSExecutionsRow) 'This NEW can not be removed

                                Dim myResulDataTime As Date
                                Dim myServiceNode As XmlNode
                                Dim myRole As TestSampleClass
                                Dim myReferenceRanges As TestRefRangesDS = Nothing
                                Dim myResult As ResultsDS.vwksResultsRow = Nothing
                                Dim myHisResult As HisWSResultsDS.vhisWSResultsRow = Nothing
                                Dim myAlarmResults As List(Of ResultsDS.vwksResultsAlarmsRow) = Nothing
                                Dim qOTs As List(Of OrderTestsLISInfoDS.twksOrderTestsLISInfoRow) = Nothing

                                'Process all Order Tests for the Patient
                                For Each item As ExecutionsDS.twksWSExecutionsRow In lnqResults
                                    searchLISInformation = True

                                    'To exclude duplicated Order Tests for Calculated Tests
                                    isCalcResultAlreadyAdded = False
                                    If (item.TestType = "CALC") Then
                                        calcs = (From a As ExecutionsDS.twksWSExecutionsRow In lnqResultsAlreadyAdded _
                                                Where a.OrderTestID = item.OrderTestID Select a).ToList
                                        isCalcResultAlreadyAdded = (calcs.Count > 0)
                                    End If

                                    If (Not isCalcResultAlreadyAdded) Then
                                        If (Not item.IsLISRequestNull AndAlso item.LISRequest) Then
                                            'Search the LIS information for the OrderTestID and RerunNumber
                                            lnqLISInformation = (From b As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In otLisInfoDS.twksOrderTestsLISInfo _
                                                                Where b.OrderTestID = item.OrderTestID _
                                                              AndAlso b.RerunNumber = item.RerunNumber _
                                                               Select b).ToList

                                            If (lnqLISInformation.Count > 0) Then
                                                'Save required LIS information in local variables
                                                mySpecimenID = lnqLISInformation.First.SpecimenID
                                                myLISPatientID = lnqLISInformation.First.LISPatientID
                                                myLISOrderID = lnqLISInformation.First.LISOrderID
                                                myESPatientID = lnqLISInformation.First.ESPatientID
                                                myESOrderID = lnqLISInformation.First.ESOrderID
                                                myAwosID = String.Empty
                                                If (Not lnqLISInformation.First.IsAwosIDNull) Then myAwosID = lnqLISInformation.First.AwosID

                                                'All needed LIS information has been found
                                                searchLISInformation = False
                                            End If
                                        End If

                                        If (searchLISInformation AndAlso myLISUploadManualResults) Then
                                            'The Order Test was not requested by LIS. Search LIS information searching the a tube with the same SampleType 
                                            lnqLISInformation = (From b As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In otLisInfoDS.twksOrderTestsLISInfo _
                                                                Where b.SampleType = item.SampleType _
                                                               Select b).ToList

                                            If (lnqLISInformation.Count > 0) Then
                                                'Save required LIS information in local variables
                                                mySpecimenID = lnqLISInformation.First.SpecimenID
                                                myLISPatientID = lnqLISInformation.First.LISPatientID
                                                myLISOrderID = lnqLISInformation.First.LISOrderID
                                                myESPatientID = lnqLISInformation.First.ESPatientID
                                                myESOrderID = lnqLISInformation.First.ESOrderID
                                                myAwosID = String.Empty
                                            Else
                                                'If there is not a LIS tube with the same SampleType, get LIS Patient and use it also as SpecimenID 
                                                mySpecimenID = otLisInfoDS.twksOrderTestsLISInfo.First.LISPatientID
                                                myLISPatientID = otLisInfoDS.twksOrderTestsLISInfo.First.LISPatientID
                                                myLISOrderID = otLisInfoDS.twksOrderTestsLISInfo.First.LISOrderID
                                                myESPatientID = otLisInfoDS.twksOrderTestsLISInfo.First.ESPatientID
                                                myESOrderID = otLisInfoDS.twksOrderTestsLISInfo.First.ESOrderID
                                                myAwosID = String.Empty
                                            End If
                                        End If

                                        'Get Result information, including the Reference Range used to validate the value (if any)
                                        resultData = GetRequiredDataForResultsUpload(dbConnection, item.OrderTestID, item.RerunNumber, item.TestType, _
                                                                                     False, pResultsDS, pResultAlarmsDS, Nothing, myResulDataTime, _
                                                                                     myReferenceRanges, myResult, myHisResult, myAlarmResults)
                                        If (resultData.HasError) Then Exit Try

                                        'Verify if pair ESPatientID/ESOrderID already exists in the local DS
                                        qOTs = (From a As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In myPatientsOrdersDS.twksOrderTestsLISInfo _
                                               Where a.ESPatientID = myESPatientID _
                                             AndAlso a.ESOrderID = myESOrderID Select a).ToList

                                        If (qOTs.Count = 0) Then
                                            'New pair; add it to the local DS
                                            Dim myRow As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow = myPatientsOrdersDS.twksOrderTestsLISInfo.NewtwksOrderTestsLISInfoRow
                                            With myRow
                                                .BeginEdit()
                                                .ESPatientID = myESPatientID
                                                .ESOrderID = myESOrderID
                                                .LISPatientID = myLISPatientID
                                                .LISOrderID = myLISOrderID
                                                .EndEdit()
                                            End With
                                            myPatientsOrdersDS.twksOrderTestsLISInfo.AddtwksOrderTestsLISInfoRow(myRow)
                                            myPatientsOrdersDS.AcceptChanges()
                                        End If

                                        'Role
                                        myRole = TestSampleClass.patient
                                        If (Not item.IsExternalQCNull AndAlso item.ExternalQC) Then myRole = TestSampleClass.QC

                                        'Create XML node 'SERVICE'
                                        resultData = MyClass.CreateServiceNode(xmlDoc, pTestMappingDS, pConfigMappingDS, myRole, myResulDataTime, _
                                                                               myAlarmResults, myResult, myHisResult, myReferenceRanges, mySpecimenID, _
                                                                               myESPatientID, myESOrderID, False, Nothing, myAwosID, item.LISMappingError)

                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            myServiceNode = CType(resultData.SetDatos, XmlNode)

                                            'Add to List of SERVICE nodes
                                            pCiServiceList.Add(myServiceNode)
                                            lnqResultsAlreadyAdded.Add(item)
                                        Else
                                            Exit Try
                                        End If
                                    End If
                                Next

                                'Create the list of Patient nodes
                                resultData = MyClass.CreatePatientNodes(xmlDoc, myPatientsOrdersDS)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    pCiPatientList = TryCast(resultData.SetDatos, List(Of XmlNode))
                                Else
                                    Exit Try
                                End If

                                qOTs = Nothing
                                calcs = Nothing
                                myAlarmResults = Nothing
                                lnqLISInformation = Nothing
                                lnqResultsAlreadyAdded = Nothing
                            End If
                        End If
                        lnqResults = Nothing
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.GetMIXEDOrdersResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Upload HISTORIC Results for a PATIENT SAMPLE that has some Order Tests requested by LIS and some Order Tests manually requested.
        ''' Created to solve issue reported as BT #1453: for Manual Order Tests, the LISPatientID has to be informed in the Patient Node, and the SpecimenID has to 
        ''' be informed with the LIS Barcode of the tube with the SampleType used to execute the Test and get the result
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pLISResults">List of rows of a typed DataSet ExecutionsDS containing all Manual and LIS Order Tests for an specific PATIENT Sample</param>
        ''' <param name="pTestMappingDS">Typed DataSet AllTestsByTypeDS containing all defined LIS Test Mappings</param>
        ''' <param name="pConfigMappingDS">Typed DataSet LISMappingsDS containing all defined LIS Configuration settings</param>
        ''' <param name="pHistResultDataDS"></param>
        ''' <param name="xmlDoc">ByRef parameter: XML Document in which the Service and Patient Nodes should be attached</param>
        ''' <param name="pCiServiceList">ByRef parameter: List of Service XML Nodes</param>
        ''' <param name="pCiPatientList">ByRef parameter: List of Patient XML Nodes</param>
        ''' <returns>All ByRef parameters plus a GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 14/01/2014 
        ''' </remarks>
        Private Function GetMIXEDOrdersResultsHIST(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pLISResults As List(Of ExecutionsDS.twksWSExecutionsRow), _
                                                   ByVal pTestMappingDS As AllTestsByTypeDS, ByVal pConfigMappingDS As LISMappingsDS, ByVal pHistResultDataDS As HisWSResultsDS, _
                                                   ByRef xmlDoc As XmlDocument, ByRef pCiServiceList As List(Of XmlNode), ByRef pCiPatientList As List(Of XmlNode)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Initialize the lists of Service and Patient Nodes
                        pCiServiceList = New List(Of XmlNode)
                        pCiPatientList = New List(Of XmlNode)

                        'Create local DS for storing pairs of ESPatientID - ESOrderID
                        Dim myPatientsOrdersDS As New OrderTestsLISInfoDS

                        'Get all Patient Order Tests sorted by SampleType and LISRequest (DESC, to process first the OrderTests requested by LIS)
                        Dim lnqResults As List(Of ExecutionsDS.twksWSExecutionsRow)
                        lnqResults = (From a As ExecutionsDS.twksWSExecutionsRow In pLISResults _
                                     Where a.SampleClass = "PATIENT" _
                                    Select a Order By a.SampleType, a.LISRequest Descending).ToList

                        If (lnqResults.Count > 0) Then
                            Dim myPatientID As String = lnqResults.First.PatientID

                            'Verify if results of Manual Patient Orders can be uploaded to LIS
                            Dim myLISUploadManualResults As Boolean = False
                            Dim myUserSettingsDelegate As New UserSettingsDelegate

                            resultData = myUserSettingsDelegate.ReadBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.LIS_UPLOAD_UNSOLICITED_PAT_RES.ToString())
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim myUserSettingDS As UserSettingDS = DirectCast(resultData.SetDatos, UserSettingDS)
                                If (myUserSettingDS.tcfgUserSettings.Count > 0) Then myLISUploadManualResults = CBool(myUserSettingDS.tcfgUserSettings(0).CurrentValue)
                            End If

                            'Declare all needed variables
                            Dim myAwosID As String = String.Empty
                            Dim mySpecimenID As String = String.Empty
                            Dim myLISPatientID As String = String.Empty
                            Dim myLISOrderID As String = String.Empty
                            Dim myESPatientID As String = String.Empty
                            Dim myESOrderID As String = String.Empty
                            Dim searchLISInformation As Boolean = False
                            Dim lnqLISInformation As List(Of HisWSResultsDS.vhisWSResultsRow)

                            'Process all Order Tests for the Patient
                            For Each item As ExecutionsDS.twksWSExecutionsRow In lnqResults
                                searchLISInformation = True

                                If (Not item.IsLISRequestNull AndAlso item.LISRequest) Then
                                    'Search the LIS information for the OrderTestID
                                    lnqLISInformation = (From a As HisWSResultsDS.vhisWSResultsRow In pHistResultDataDS.vhisWSResults _
                                                        Where a.HistOrderTestID = item.OrderTestID _
                                                       Select a).ToList

                                    If (lnqLISInformation.Count > 0) Then
                                        'Save required LIS information in local variables
                                        'mySpecimenID = lnqLISInformation.First.SpecimenID
                                        myLISPatientID = lnqLISInformation.First.LISPatientID
                                        myLISOrderID = lnqLISInformation.First.LISOrderID
                                        myESPatientID = lnqLISInformation.First.ESPatientID
                                        myESOrderID = lnqLISInformation.First.ESOrderID
                                        myAwosID = String.Empty
                                        ' If (Not lnqLISInformation.First.IsAwosIDNull) Then myAwosID = lnqLISInformation.First.AwosID

                                        'All needed LIS information has been found
                                        searchLISInformation = False
                                    End If
                                End If
                            Next
                        End If
                        lnqResults = Nothing
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.GetMIXEDOrdersResultsHIST", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pOrderTestID"></param>
        ''' <param name="pRerunNumber"></param>
        ''' <param name="pTestType"></param>
        ''' <param name="pHistoricalFlag"></param>
        ''' <param name="pResultsDS"></param>
        ''' <param name="pResultAlarmsDS"></param>
        ''' <param name="pHistResultDataDS"></param>
        ''' <param name="pResulDataTime"></param>
        ''' <param name="pReferenceRanges"></param>
        ''' <param name="pResult"></param>
        ''' <param name="pHisResult"></param>
        ''' <param name="pAlarmResults"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: XB 24/05/2013 - QC's must take Ref Ranges from GetControlsNEW (TestControlsDelegate) instead of from GetReferenceRangeInterval 
        '''                              (OrderTestsDelegate) which are used for Patients
        '''              XB 26/06/2013 - Distinct Reference Ranges of different Controls assigned to the same Test (Bugstracking #1203)
        '''              SA 14/01/2014 - Use local dbConnection instead of the parameter pDBConnection when calling functions GetControlsNEW and 
        '''                              GetReferenceRangeInterval
        ''' </remarks>
        Private Function GetRequiredDataForResultsUpload(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                         ByVal pOrderTestID As Integer, _
                                                         ByVal pRerunNumber As Integer, _
                                                         ByVal pTestType As String, _
                                                         ByVal pHistoricalFlag As Boolean, _
                                                         ByVal pResultsDS As ResultsDS, _
                                                         ByVal pResultAlarmsDS As ResultsDS, _
                                                         ByVal pHistResultDataDS As HisWSResultsDS, _
                                                         ByRef pResulDataTime As Date, _
                                                         ByRef pReferenceRanges As TestRefRangesDS, _
                                                         ByRef pResult As ResultsDS.vwksResultsRow, _
                                                         ByRef pHisResult As HisWSResultsDS.vhisWSResultsRow, _
                                                         ByRef pAlarmResults As List(Of ResultsDS.vwksResultsAlarmsRow)) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOrderTestDelegate As New OrderTestsDelegate

                        If (Not pHistoricalFlag) Then
                            '*** UPLOAD OF RESULTS IN THE ACTIVE WS ***'

                            'Get Alarms for the OrderTestID/RerunNumber
                            Dim myAlarmResults As List(Of ResultsDS.vwksResultsAlarmsRow)
                            myAlarmResults = (From a As ResultsDS.vwksResultsAlarmsRow In pResultAlarmsDS.vwksResultsAlarms _
                                             Where a.OrderTestID = pOrderTestID And a.RerunNumber = pRerunNumber _
                                             Select a).ToList

                            If (myAlarmResults.Count > 0) Then pAlarmResults = myAlarmResults
                            myAlarmResults = Nothing

                            'Get Result information for the OrderTestID/RerunNumber
                            Dim myResults As List(Of ResultsDS.vwksResultsRow)
                            myResults = (From a As ResultsDS.vwksResultsRow In pResultsDS.vwksResults _
                                        Where a.OrderTestID = pOrderTestID _
                                      AndAlso a.RerunNumber = pRerunNumber _
                                      AndAlso a.TestType = pTestType _
                                       Select a).ToList

                            If (myResults.Count > 0) Then
                                pResult = myResults(0)
                                pResulDataTime = pResult.ResultDateTime

                                'XB 24/05/2013  
                                If (pResult.SampleClass = "CTRL") Then
                                    'Get Reference Ranges for CONTROLS
                                    Dim myTestCtrl As New TestControlsDelegate
                                    resultData = myTestCtrl.GetControlsNEW(dbConnection, pResult.TestType, pResult.TestID, pResult.SampleType)

                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        Dim myResultDS As TestControlsDS = DirectCast(resultData.SetDatos, TestControlsDS)

                                        'XB 26/06/2013 v2.1.0 - Bugstracking #1203
                                        Dim myControlName As String = pResult.ControlName
                                        Dim myQCResults As List(Of TestControlsDS.tparTestControlsRow)
                                        myQCResults = (From a As TestControlsDS.tparTestControlsRow In myResultDS.tparTestControls _
                                                      Where a.ControlName = myControlName _
                                                     Select a).ToList

                                        If (myQCResults.Count > 0) Then
                                            'XB 26/06/2013 v2.1.0 - Bugstracking #1203
                                            pReferenceRanges = New TestRefRangesDS

                                            Dim myRefRangeRow As TestRefRangesDS.tparTestRefRangesRow = pReferenceRanges.tparTestRefRanges.NewtparTestRefRangesRow
                                            With myRefRangeRow
                                                .BeginEdit()
                                                .NormalLowerLimit = myQCResults(0).MinConcentration ' myResultDS.tparTestControls(0).MinConcentration ' XB 26/06/2013 v2.1.0 - Bugstracking #1203
                                                .NormalUpperLimit = myQCResults(0).MaxConcentration ' myResultDS.tparTestControls(0).MaxConcentration ' XB 26/06/2013 v2.1.0 - Bugstracking #1203
                                                .EndEdit()
                                            End With
                                            pReferenceRanges.tparTestRefRanges.AddtparTestRefRangesRow(myRefRangeRow)
                                            pReferenceRanges.AcceptChanges()
                                        End If
                                    End If
                                Else
                                    'XB 24/05/2013
                                    'Get Reference Ranges for PATIENTS
                                    If (Not pResult.IsActiveRangeTypeNull) Then
                                        Dim mySampleType As String = String.Empty
                                        If (pTestType <> "CALC") Then mySampleType = pResult.SampleType

                                        resultData = myOrderTestDelegate.GetReferenceRangeInterval(dbConnection, pResult.OrderTestID, pResult.TestType, _
                                                                                                   pResult.TestID, mySampleType, pResult.ActiveRangeType)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            pReferenceRanges = DirectCast(resultData.SetDatos, TestRefRangesDS)
                                        End If
                                    End If
                                End If
                            End If
                            myResults = Nothing

                        Else
                            '*** UPLOAD OF HISTORIC PATIENT RESULTS ***'

                            'Alarms, Result information and Reference Ranges
                            Dim myHisResults As List(Of HisWSResultsDS.vhisWSResultsRow)
                            myHisResults = (From a As HisWSResultsDS.vhisWSResultsRow In pHistResultDataDS.vhisWSResults _
                                           Where a.HistOrderTestID = pOrderTestID _
                                          Select a).ToList

                            If (myHisResults.Count > 0) Then
                                pHisResult = myHisResults(0)

                                'Get all Alarms definition
                                Dim alarmDlg As New AlarmsDelegate
                                Dim alarmsDefinition As New AlarmsDS
                                Dim myAuxResultsAvg As New ResultsDS

                                resultData = alarmDlg.ReadAll(dbConnection)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    alarmsDefinition = DirectCast(resultData.SetDatos, AlarmsDS)
                                End If

                                'Decode alarms
                                Dim myAlarms() As String
                                Dim list As List(Of AlarmsDS.tfmwAlarmsRow)

                                myAlarms = Split(pHisResult.AlarmList, GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR)
                                For Each ID As String In myAlarms
                                    If (String.Compare(ID, "", False) <> 0) Then
                                        list = (From a As AlarmsDS.tfmwAlarmsRow In alarmsDefinition.tfmwAlarms _
                                               Where String.Compare(a.AlarmID, ID, False) = 0 Select a).ToList

                                        If (list.Count > 0) Then
                                            Dim viewAlarmRow As ResultsDS.vwksResultsAlarmsRow
                                            viewAlarmRow = myAuxResultsAvg.vwksResultsAlarms.NewvwksResultsAlarmsRow
                                            viewAlarmRow.OrderTestID = pOrderTestID
                                            viewAlarmRow.RerunNumber = pRerunNumber
                                            viewAlarmRow.Description = list(0).Description
                                            viewAlarmRow.AcceptedResultFlag = True
                                            viewAlarmRow.AlarmID = list(0).AlarmID
                                            If (pAlarmResults Is Nothing) Then pAlarmResults = New List(Of ResultsDS.vwksResultsAlarmsRow)
                                            pAlarmResults.Add(viewAlarmRow)
                                        End If
                                    End If
                                Next

                                'Result DateTime
                                pResulDataTime = pHisResult.ResultDateTime

                                'Reference Range
                                If (Not pHisResult.IsMinRefRangeNull AndAlso Not pHisResult.IsMaxRefRangeNull) Then
                                    pReferenceRanges = New TestRefRangesDS
                                    Dim myRefRangeRow As TestRefRangesDS.tparTestRefRangesRow = pReferenceRanges.tparTestRefRanges.NewtparTestRefRangesRow
                                    With myRefRangeRow
                                        .BeginEdit()
                                        .NormalLowerLimit = pHisResult.MinRefRange
                                        .NormalUpperLimit = pHisResult.MaxRefRange
                                        .EndEdit()
                                    End With
                                    pReferenceRanges.tparTestRefRanges.AddtparTestRefRangesRow(myRefRangeRow)
                                    pReferenceRanges.AcceptChanges()
                                End If
                            End If
                            myHisResults = Nothing
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.GetRequiredDataForResultsUpload", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "LIS --> PRESENTATION (decode xml methods)"


        ''' <summary>
        ''' Used for BAx00 application in order to get the xml information of the received message events decoded into a known language for it (Data set, Transference object (TO), …)
        ''' 
        ''' This method also implements mapping validation and inform an output field Rejected
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="xmlMessage"></param>
        ''' <returns>GlobalDataTO with setdatos as structure known by BAx00 software process</returns>
        ''' <remarks>
        ''' Creation AG 22/02/2013 - empty, only creation
        ''' </remarks>
        Public Function DecodeXMLMessage(ByVal pDBConnection As SqlClient.SqlConnection, ByVal xmlMessage As XmlDocument) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        '<Function Logic>
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.DecodeXMLMessage", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        'Public Function DecodeXMLServiceTag_OLD(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                                    ByVal pXMLNode As XmlNode, _
        '                                    ByVal pLISMappingsDS As LISMappingsDS, _
        '                                    ByVal pTestMappingsDS As AllTestsByTypeDS) As GlobalDataTO

        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)

        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then

        '                Dim mySavedWSOrderTestsDS As New SavedWSOrderTestsDS
        '                Dim myResultRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow = mySavedWSOrderTestsDS.tparSavedWSOrderTests.NewtparSavedWSOrderTestsRow()

        '                Dim myServiceNode As XmlNode = pXMLNode

        '                '1-Get and Save LIS Order Test Identifier (AWOS ID)
        '                Dim myAwosId As String = MyClass.xmlHelper.QueryStringValue(myServiceNode, "ci:id", "")
        '                If myAwosId.Length > 0 Then
        '                    myResultRow.BeginEdit()
        '                    myResultRow.AwosID = myAwosId
        '                    myResultRow.EndEdit()
        '                Else
        '                    resultData.HasError = True
        '                    resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
        '                    Exit Try
        '                End If





        '                '2-Get and Save SPECIMEN IDENTIFIER
        '                Dim mySpecimenId As String = MyClass.xmlHelper.QueryStringValue(myServiceNode, "ci:specimen/ci:id", "")
        '                If mySpecimenId.Length > 0 Then
        '                    myResultRow.BeginEdit()
        '                    myResultRow.SpecimenID = mySpecimenId
        '                    myResultRow.EndEdit()
        '                Else
        '                    mySavedWSOrderTestsDS.tparSavedWSOrderTests.AddtparSavedWSOrderTestsRow(myResultRow)
        '                    mySavedWSOrderTestsDS.AcceptChanges()
        '                    resultData.SetDatos = mySavedWSOrderTestsDS.tparSavedWSOrderTests(0)
        '                    resultData.HasError = True
        '                    resultData.ErrorCode = Messages.LIMS_INVALID_FIELD_NUM.ToString
        '                    Exit Try
        '                End If




        '                'AG 14/03/2013
        '                '2-B) Verify no info available for specimen
        '                Dim myErrorInfo As String = MyClass.xmlHelper.QueryStringValue(myServiceNode, "ci:error/ci:id", "")
        '                If myErrorInfo.Length > 0 Then
        '                    'Exist error
        '                    mySavedWSOrderTestsDS.tparSavedWSOrderTests.AddtparSavedWSOrderTestsRow(myResultRow)
        '                    mySavedWSOrderTestsDS.AcceptChanges()
        '                    resultData.SetDatos = mySavedWSOrderTestsDS.tparSavedWSOrderTests(0)
        '                    resultData.HasError = True
        '                    resultData.ErrorCode = Messages.LIS_NO_INFO_AVAILABLE.ToString
        '                    Exit Try



        '                End If
        '                'AG 14/03/2013

        '                '3-Get, Validate and Save SAMPLE CLASS
        '                Dim mySampleClass As String = MyClass.xmlHelper.QueryStringValue(myServiceNode, "ci:test/ci:role", "")
        '                If mySampleClass.Length > 0 Then
        '                    'AG 14/03/2013 - Changes for internal or external controls
        '                    If mySampleClass = TestSampleClass.QC.ToString AndAlso myResultRow.SpecimenID <> "CONTROL" Then
        '                        mySampleClass = TestSampleClass.patient.ToString 'Treat as patient!!
        '                    End If
        '                    'AG 14/03/2013

        '                    Select Case mySampleClass
        '                        Case TestSampleClass.patient.ToString
        '                            'Inform SaveWSOrderTestsDS.SampleClass = PATIENT
        '                            myResultRow.BeginEdit()
        '                            myResultRow.SampleClass = "PATIENT"
        '                            myResultRow.EndEdit()

        '                            'get StatFlag
        '                            Dim myStatFlag As String = MyClass.xmlHelper.QueryStringValue(myServiceNode, "ci:test/ci:priority", "")
        '                            If myStatFlag.Length > 0 Then
        '                                Select Case myStatFlag
        '                                    Case TestStatusFlags.normal.ToString
        '                                        myResultRow.BeginEdit()
        '                                        myResultRow.StatFlag = False
        '                                        myResultRow.EndEdit()

        '                                    Case TestStatusFlags.stat.ToString
        '                                        myResultRow.BeginEdit()
        '                                        myResultRow.StatFlag = True
        '                                        myResultRow.EndEdit()

        '                                    Case Else
        '                                        mySavedWSOrderTestsDS.tparSavedWSOrderTests.AddtparSavedWSOrderTestsRow(myResultRow)
        '                                        mySavedWSOrderTestsDS.AcceptChanges()
        '                                        resultData.SetDatos = mySavedWSOrderTestsDS.tparSavedWSOrderTests(0)
        '                                        resultData.HasError = True
        '                                        resultData.ErrorCode = Messages.LIMS_INVALID_SAMPLECLASS.ToString
        '                                        Exit Try
        '                                End Select

        '                            Else
        '                                mySavedWSOrderTestsDS.tparSavedWSOrderTests.AddtparSavedWSOrderTestsRow(myResultRow)
        '                                mySavedWSOrderTestsDS.AcceptChanges()
        '                                resultData.SetDatos = mySavedWSOrderTestsDS.tparSavedWSOrderTests(0)
        '                                resultData.HasError = True
        '                                resultData.ErrorCode = Messages.LIMS_INVALID_SAMPLECLASS.ToString
        '                                Exit Try
        '                            End If

        '                        Case TestSampleClass.QC.ToString
        '                            'Inform SaveWSOrderTestsDS.SampleClass = CTRL
        '                            myResultRow.BeginEdit()
        '                            myResultRow.SampleClass = "CTRL"
        '                            myResultRow.StatFlag = False
        '                            myResultRow.EndEdit()

        '                        Case Else
        '                            mySavedWSOrderTestsDS.tparSavedWSOrderTests.AddtparSavedWSOrderTestsRow(myResultRow)
        '                            mySavedWSOrderTestsDS.AcceptChanges()
        '                            resultData.SetDatos = mySavedWSOrderTestsDS.tparSavedWSOrderTests(0)
        '                            resultData.HasError = True
        '                            resultData.ErrorCode = Messages.LIMS_INVALID_SAMPLECLASS.ToString
        '                            Exit Try

        '                    End Select
        '                Else
        '                    mySavedWSOrderTestsDS.tparSavedWSOrderTests.AddtparSavedWSOrderTestsRow(myResultRow)
        '                    mySavedWSOrderTestsDS.AcceptChanges()
        '                    resultData.SetDatos = mySavedWSOrderTestsDS.tparSavedWSOrderTests(0)
        '                    resultData.HasError = True
        '                    resultData.ErrorCode = Messages.LIMS_INVALID_FIELD_NUM.ToString
        '                    Exit Try
        '                End If

        '                '4-Get, Validate and Save SAMPLE TYPE
        '                Dim mySampleType As String = MyClass.xmlHelper.QueryStringValue(myServiceNode, "ci:specimen/ci:type", "")
        '                If mySampleType.Length > 0 Then

        '                    Dim myLISMappings As New List(Of LISMappingsDS.vcfgLISMappingRow)
        '                    myLISMappings = (From a In pLISMappingsDS.vcfgLISMapping _
        '                                    Where a.LISValue = mySampleType Select a).ToList()

        '                    If myLISMappings.Count > 0 Then
        '                        myResultRow.BeginEdit()
        '                        myResultRow.SampleType = myLISMappings.First.ValueId
        '                        myResultRow.EndEdit()
        '                    Else
        '                        mySavedWSOrderTestsDS.tparSavedWSOrderTests.AddtparSavedWSOrderTestsRow(myResultRow)
        '                        mySavedWSOrderTestsDS.AcceptChanges()
        '                        resultData.SetDatos = mySavedWSOrderTestsDS.tparSavedWSOrderTests(0)
        '                        resultData.HasError = True
        '                        resultData.ErrorCode = Messages.LIMS_INVALID_SAMPLETYPE.ToString
        '                        Exit Try
        '                    End If
        '                    myLISMappings = Nothing

        '                Else
        '                    mySavedWSOrderTestsDS.tparSavedWSOrderTests.AddtparSavedWSOrderTestsRow(myResultRow)
        '                    mySavedWSOrderTestsDS.AcceptChanges()
        '                    resultData.SetDatos = mySavedWSOrderTestsDS.tparSavedWSOrderTests(0)
        '                    resultData.HasError = True
        '                    resultData.ErrorCode = Messages.LIMS_INVALID_FIELD_NUM.ToString
        '                    Exit Try
        '                End If

        '                '5-Get, Validate and Save TEST
        '                Dim myTestId As String = MyClass.xmlHelper.QueryStringValue(myServiceNode, "ci:test/ci:id", "")
        '                If myTestId.Length > 0 Then

        '                    Dim myLISMappings As New List(Of AllTestsByTypeDS.vparAllTestsByTypeRow)
        '                    myLISMappings = (From a In pTestMappingsDS.vparAllTestsByType _
        '                                    Where a.LISValue = myTestId Select a).ToList()

        '                    If myLISMappings.Count > 0 Then
        '                        myResultRow.BeginEdit()
        '                        myResultRow.TestType = myLISMappings.First.TestType
        '                        myResultRow.TestName = myLISMappings.First.TestName
        '                        myResultRow.TestID = CInt(myLISMappings.First.TestID)
        '                        myResultRow.EndEdit()
        '                    Else
        '                        mySavedWSOrderTestsDS.tparSavedWSOrderTests.AddtparSavedWSOrderTestsRow(myResultRow)
        '                        mySavedWSOrderTestsDS.AcceptChanges()
        '                        resultData.SetDatos = mySavedWSOrderTestsDS.tparSavedWSOrderTests(0)
        '                        resultData.HasError = True
        '                        resultData.ErrorCode = Messages.LIMS_INVALID_TEST.ToString
        '                        Exit Try
        '                    End If
        '                    myLISMappings = Nothing

        '                Else
        '                    mySavedWSOrderTestsDS.tparSavedWSOrderTests.AddtparSavedWSOrderTestsRow(myResultRow)
        '                    mySavedWSOrderTestsDS.AcceptChanges()
        '                    resultData.SetDatos = mySavedWSOrderTestsDS.tparSavedWSOrderTests(0)
        '                    resultData.HasError = True
        '                    resultData.ErrorCode = Messages.LIMS_INVALID_FIELD_NUM.ToString
        '                    Exit Try
        '                End If

        '                'PENDING TO DEFINE!!!!!!!!
        '                ''5-Get, Validate and Save TUBE TYPE
        '                'Dim myTubeType As String = MyClass.xmlHelper.QueryStringValue(myServiceNode, "?????", "")
        '                'If myTubeType.Length > 0 Then

        '                'Else
        '                '    'empty
        '                '    'TODO 
        '                'End If

        '                '6-Get and Save ES PATIENT ID
        '                Dim myESPatientId As String = MyClass.xmlHelper.QueryAttributeStringValue(myServiceNode, "id", "ci:patient", "")
        '                If myESPatientId.Length > 0 Then
        '                    myResultRow.BeginEdit()
        '                    myResultRow.ESPatientID = myESPatientId
        '                    myResultRow.EndEdit()
        '                Else
        '                    mySavedWSOrderTestsDS.tparSavedWSOrderTests.AddtparSavedWSOrderTestsRow(myResultRow)
        '                    mySavedWSOrderTestsDS.AcceptChanges()
        '                    resultData.SetDatos = mySavedWSOrderTestsDS.tparSavedWSOrderTests(0)
        '                    resultData.HasError = True
        '                    resultData.ErrorCode = Messages.LIMS_INVALID_FIELD_NUM.ToString
        '                    Exit Try
        '                End If

        '                '8-Get and Save ES ORDER ID
        '                Dim myESOrderId As String = MyClass.xmlHelper.QueryAttributeStringValue(myServiceNode, "id", "ci:order", "")
        '                If myESOrderId.Length > 0 Then
        '                    myResultRow.BeginEdit()
        '                    myResultRow.ESOrderID = myESOrderId
        '                    myResultRow.EndEdit()
        '                Else
        '                    mySavedWSOrderTestsDS.tparSavedWSOrderTests.AddtparSavedWSOrderTestsRow(myResultRow)
        '                    mySavedWSOrderTestsDS.AcceptChanges()
        '                    resultData.SetDatos = mySavedWSOrderTestsDS.tparSavedWSOrderTests(0)
        '                    resultData.HasError = True
        '                    resultData.ErrorCode = Messages.LIMS_INVALID_FIELD_NUM.ToString
        '                    Exit Try
        '                End If


        '                mySavedWSOrderTestsDS.tparSavedWSOrderTests.AddtparSavedWSOrderTestsRow(myResultRow)
        '                mySavedWSOrderTestsDS.AcceptChanges()
        '                'resultData.SetDatos = mySavedWSOrderTestsDS
        '                resultData.SetDatos = mySavedWSOrderTestsDS.tparSavedWSOrderTests(0)

        '            End If

        '        End If

        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.DecodeXMLServiceTag", EventLogEntryType.Error, False)

        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

        '    End Try

        '    Return resultData
        'End Function

        ''' <summary>
        ''' Decode a Service tag from an XML containing ORDERS sent by LIS. Validate fields are 
        ''' informed with known BAx00 values (solve mappings). 
        ''' </summary>
        ''' <param name="pXMLNode">Full content of a Node labelled as “service” in an XML containing information about an Order requested by LIS</param>
        ''' <param name="pLISMappingsDS">Typed Dataset containing the list of values that have been mapped to understand the information sent by LIS. </param>
        ''' <param name="pTestMappingsDS">Typed Dataset containing the list of Tests and Profiles that have been mapped to understand the information sent by LIS</param>
        ''' <returns>GlobalDataTO with setdatos as SavedWSOrderTestsDS.tparSavedWSOrderTestsRow</returns>
        ''' <remarks>
        ''' Created by:  SG 27/02/2013
        ''' Modified by: AG 14/03/2013 - Method returns SavedWSOrderTestsDS.tparSavedWSOrderTestsRow; changed decode order: awos, specimen, verify specimen info available, 
        '''                              sampleclass, sampletype, test, patientID, and orderID; applied change for internal or external controls
        '''              SG 25/03/2013 - Not cancel decoding process in case of missing tags or values
        '''              SA 02/04/2013 - Removed the DB Connection parameter; inform fields LISSampleClass, LISStatFlag, LISSampleType and LISTestID  with the Tag Value
        '''                              always the Tag is informed, not only when there is an error in the current tag or in the previously processed ones; when tag Error
        '''                              exists in Specimen tag, a SavedWSOrderTestsDS row with the SpecimenID have to be returned besides the error code
        '''              AG 06/05/2013 - change decode priority 1- Specimen, 2- Error, 3 - AwosID (change because the noInfoAvailable messages have not awosID)
        ''' </remarks>
        Public Function DecodeXMLServiceTag(ByVal pXMLNode As XmlNode, ByVal pLISMappingsDS As LISMappingsDS, ByVal pTestMappingsDS As AllTestsByTypeDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO 'Do not remove New declaration, is needed

            Try
                Dim myServiceNode As XmlNode = pXMLNode

                Dim mySavedWSOrderTestsDS As New SavedWSOrderTestsDS
                Dim myResultRow As SavedWSOrderTestsDS.tparSavedWSOrderTestsRow = mySavedWSOrderTestsDS.tparSavedWSOrderTests.NewtparSavedWSOrderTestsRow()

                'AG 06/05/2013 - low priority to number 3
                ''1-Get and Save LIS Order Test Identifier (AWOS ID)
                'Dim myAwosId As String = MyClass.xmlHelper.QueryStringValue(myServiceNode, "ci:id", String.Empty)

                'myResultRow.BeginEdit()
                'If (myAwosId.Length = 0) Then
                '    myResultRow.AwosID = String.Empty
                '    resultData.HasError = True
                '    resultData.ErrorCode = Messages.LIMS_INVALID_FIELD_NUM.ToString
                'Else
                '    myResultRow.AwosID = myAwosId
                'End If
                'myResultRow.EndEdit()


                '2-Get and Save SPECIMEN IDENTIFIER
                Dim checkErrorInfo As Boolean = True
                Dim mySpecimenId As String = MyClass.xmlHelper.QueryStringValue(myServiceNode, "ci:specimen/ci:id", String.Empty)

                myResultRow.BeginEdit()
                If (mySpecimenId.Length = 0) Then
                    checkErrorInfo = False
                    myResultRow.SpecimenID = String.Empty
                    resultData.HasError = True
                    resultData.ErrorCode = Messages.LIMS_INVALID_FIELD_NUM.ToString
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - INVALID FIELD : SPECIMEN ID")
                Else
                    myResultRow.SpecimenID = mySpecimenId
                End If
                myResultRow.EndEdit()

                If (checkErrorInfo) Then
                    'JC 08/05/2013 --AG 14/03/2013
                    'Verify if the Message is a HostQuery answer indicating that there is not information available for the Specimen
                    Dim myErrorInfo As String = MyClass.xmlHelper.QueryStringValue(myServiceNode, "ci:error/ci:id", String.Empty)
                    If (myErrorInfo.Length > 0 AndAlso myErrorInfo = "noInfoAvailable") Then
                        resultData.HasError = True
                        resultData.ErrorCode = Messages.LIS_NO_INFO_AVAILABLE.ToString
                        Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - LIS INFO NO AVAILABLE")

                        'Add the row with the Specimen to a SavedWSOrderTestsDS
                        mySavedWSOrderTestsDS.tparSavedWSOrderTests.AddtparSavedWSOrderTestsRow(myResultRow)
                        mySavedWSOrderTestsDS.AcceptChanges()

                        'Finally, return the SavedWSOrderTestsDS row
                        resultData.SetDatos = mySavedWSOrderTestsDS.tparSavedWSOrderTests.Rows(0)
                        Exit Try
                    End If
                    'AG 14/03/2013
                End If

                'AG 06/05/2013
                '3-Get and Save LIS Order Test Identifier (AWOS ID)
                Dim myAwosId As String = MyClass.xmlHelper.QueryStringValue(myServiceNode, "ci:id", String.Empty)

                myResultRow.BeginEdit()
                If (myAwosId.Length = 0) Then
                    myResultRow.AwosID = String.Empty
                    resultData.HasError = True
                    resultData.ErrorCode = Messages.LIMS_INVALID_FIELD_NUM.ToString
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - INVALID FIELD : AWOS ID")
                Else
                    myResultRow.AwosID = myAwosId
                End If
                myResultRow.EndEdit()


                '4-Get, Validate and Save SAMPLE CLASS
                Dim mySampleClass As String = MyClass.xmlHelper.QueryStringValue(myServiceNode, "ci:test/ci:role", String.Empty)

                myResultRow.BeginEdit()
                If (mySampleClass.Length = 0) Then
                    myResultRow.SampleClass = String.Empty
                    resultData.HasError = True
                    resultData.ErrorCode = Messages.LIMS_INVALID_FIELD_NUM.ToString
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - INVALID FIELD : SAMPLE CLASS")
                Else
                    'Tag value is always returned
                    myResultRow.LISSampleClass = mySampleClass

                    'Only when there is not an error in the previous processed tags, the SampleClass is decoded
                    If (Not resultData.HasError) Then
                        Select Case mySampleClass
                            'PATIENT
                            Case (TestSampleClass.patient.ToString)
                                myResultRow.SampleClass = "PATIENT"
                                myResultRow.ExternalQC = False

                                'QC
                            Case (TestSampleClass.QC.ToString)
                                If (myResultRow.SpecimenID = "CONTROL") Then
                                    myResultRow.SampleClass = "CTRL"
                                Else
                                    myResultRow.SampleClass = "PATIENT"
                                    myResultRow.ExternalQC = True
                                End If
                            Case Else
                                resultData.HasError = True
                                resultData.ErrorCode = Messages.LIMS_INVALID_SAMPLECLASS.ToString
                                Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - INVALID FIELD : SAMPLE CLASS")
                        End Select
                    End If
                End If
                myResultRow.EndEdit()

                '5-Validate and save StatFlag
                Dim myStatFlag As String = MyClass.xmlHelper.QueryStringValue(myServiceNode, "ci:test/ci:priority", String.Empty)

                myResultRow.BeginEdit()
                If (myStatFlag.Length = 0) Then
                    myResultRow.LISStatFlag = String.Empty
                    resultData.HasError = True
                    resultData.ErrorCode = Messages.LIMS_INVALID_FIELD_NUM.ToString
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - INVALID FIELD : STAT FLAG")
                Else
                    'Tag value is always returned
                    myResultRow.LISStatFlag = myStatFlag

                    'Only when there is not an error in the previous processed tags, the Priority is decoded
                    If (Not resultData.HasError) Then
                        Select Case myStatFlag
                            Case (TestStatusFlags.normal.ToString)
                                myResultRow.StatFlag = False

                            Case (TestStatusFlags.stat.ToString)
                                myResultRow.StatFlag = (myResultRow.SampleClass = "PATIENT")

                            Case Else
                                resultData.HasError = True
                                resultData.ErrorCode = Messages.LIMS_INVALID_SAMPLECLASS.ToString
                                Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - INVALID FIELD : STAT FLAG")
                        End Select
                    End If
                End If
                myResultRow.EndEdit()

                '6-Get, Validate and Save SAMPLE TYPE
                Dim mySampleType As String = MyClass.xmlHelper.QueryStringValue(myServiceNode, "ci:specimen/ci:type", String.Empty)

                myResultRow.BeginEdit()
                If (mySampleType.Length = 0) Then
                    myResultRow.LISSampleType = String.Empty
                    resultData.HasError = True
                    resultData.ErrorCode = Messages.LIMS_INVALID_FIELD_NUM.ToString
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - INVALID FIELD : SAMPLE TYPE")
                Else
                    'Tag value is always returned
                    myResultRow.LISSampleType = mySampleType

                    'Only when there is not an error in the previous processed tags, the SampleType is decoded
                    If (Not resultData.HasError) Then
                        Dim myLISMappings As New List(Of LISMappingsDS.vcfgLISMappingRow)
                        myLISMappings = (From a As LISMappingsDS.vcfgLISMappingRow In pLISMappingsDS.vcfgLISMapping _
                                        Where a.LISValue = mySampleType Select a).ToList()

                        If (myLISMappings.Count > 0) Then
                            myResultRow.SampleType = myLISMappings.First.ValueId
                        Else
                            resultData.HasError = True
                            resultData.ErrorCode = Messages.LIMS_INVALID_SAMPLETYPE.ToString
                            Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - INVALID FIELD : SAMPLE TYPE")
                        End If
                        myLISMappings = Nothing
                    End If
                End If
                myResultRow.EndEdit()

                '7-Get, Validate and Save TEST
                Dim myTestId As String = MyClass.xmlHelper.QueryStringValue(myServiceNode, "ci:test/ci:id", String.Empty)

                myResultRow.BeginEdit()
                If (myTestId.Length = 0) Then
                    myResultRow.LISTestID = String.Empty
                    resultData.HasError = True
                    resultData.ErrorCode = Messages.LIMS_INVALID_FIELD_NUM.ToString
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - INVALID FIELD : TEST ID")
                Else
                    'Tag value is always returned
                    myResultRow.LISTestID = myTestId

                    'Only when there is not an error in the previous processed tags, the TestName is decoded
                    If (Not resultData.HasError) Then
                        Dim myLISMappings As New List(Of AllTestsByTypeDS.vparAllTestsByTypeRow)
                        myLISMappings = (From a As AllTestsByTypeDS.vparAllTestsByTypeRow In pTestMappingsDS.vparAllTestsByType _
                                        Where a.LISValue = myTestId Select a).ToList()

                        If (myLISMappings.Count > 0) Then
                            myResultRow.TestType = myLISMappings.First.TestType
                            myResultRow.TestName = myLISMappings.First.TestName
                            myResultRow.TestID = myLISMappings.First.TestID
                        Else
                            resultData.HasError = True
                            resultData.ErrorCode = Messages.LIMS_INVALID_TEST.ToString
                            Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - INVALID FIELD : TEST ID")
                        End If
                        myLISMappings = Nothing
                    End If
                End If
                myResultRow.EndEdit()

                '8-Get and Save ES PATIENT ID
                Dim myESPatientId As String = MyClass.xmlHelper.QueryAttributeStringValue(myServiceNode, "id", "ci:patient", String.Empty)

                myResultRow.BeginEdit()
                If (myESPatientId.Length = 0) Then
                    myResultRow.ESPatientID = String.Empty
                    resultData.HasError = True
                    resultData.ErrorCode = Messages.LIMS_INVALID_FIELD_NUM.ToString
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - INVALID FIELD : ES PATIENT ID")
                Else
                    myResultRow.ESPatientID = myESPatientId
                End If
                myResultRow.EndEdit()

                '9-Get and Save ES ORDER ID
                Dim myESOrderId As String = MyClass.xmlHelper.QueryAttributeStringValue(myServiceNode, "id", "ci:order", String.Empty)

                myResultRow.BeginEdit()
                If (myESOrderId.Length = 0) Then
                    myResultRow.ESOrderID = String.Empty
                    resultData.HasError = True
                    resultData.ErrorCode = Messages.LIMS_INVALID_FIELD_NUM.ToString
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - INVALID FIELD : ES ORDER ID")
                Else
                    myResultRow.ESOrderID = myESOrderId
                End If
                myResultRow.EndEdit()

                'Add the row to a SavedWSOrderTestsDS
                mySavedWSOrderTestsDS.tparSavedWSOrderTests.AddtparSavedWSOrderTestsRow(myResultRow)
                mySavedWSOrderTestsDS.AcceptChanges()

                'Finally, return the SavedWSOrderTestsDS row
                resultData.SetDatos = mySavedWSOrderTestsDS.tparSavedWSOrderTests.Rows(0)
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.DecodeXMLServiceTag", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Decode a Patient tag from an XML containing ORDERS sent by LIS. 
        ''' </summary>
        ''' <param name="pXMLNode">Full content of a Node labelled as “patient” in an XML containing information about an Order requested by LIS</param>
        ''' <returns>GlobalDataTO with setdatos as PatientsDS.tparPatientsRow</returns>
        ''' <remarks>
        ''' Created by:  SG 27/02/2013
        ''' Modified by: AG 14/03/2013 - Method returns PatientsDS.tparPatientsRow
        '''              SA 02/04/2013 - Removed the DB Connection parameter; informed field ExternalArrivalDate with the current date and time
        '''              SA 03/04/2013 - Changed the order in which the node fields are processed: ES Identifier is read before the LIS Identifier, and
        '''                              only the first one is required (the LIS Identifier is missing for Controls); changed PatientIDType=MANUAL by 
        '''                              PatientIDType=MAN
        '''              SA 10/05/2013 - The LIS IDENTIFIER is saved in a new field in the PatientsDS row to return: LISPatientID. Field PatientID is 
        '''                              informed with the same value but in UPPERCASE; this is needed to compare and add correctly the PatientID in 
        '''                              tparPatients table
        ''' </remarks>
        Public Function DecodeXMLPatientTag(ByVal pXMLNode As XmlNode) As GlobalDataTO
            Dim resultData As New GlobalDataTO 'Do not remove New declaration, is needed

            Try
                Dim myPatientNode As XmlNode = pXMLNode
                Dim demographicsInformed As Boolean = False

                Dim myPatientsDS As New PatientsDS
                Dim myResultRow As PatientsDS.tparPatientsRow = myPatientsDS.tparPatients.NewtparPatientsRow

                '1-Get and Save ES IDENTIFIER
                Dim myESId As String = MyClass.xmlHelper.QueryAttributeStringValue(myPatientNode, "id", "", String.Empty)
                If (myESId.Length > 0) Then
                    myResultRow.BeginEdit()
                    myResultRow.ExternalPID = myESId
                    myResultRow.EndEdit()
                Else
                    resultData.HasError = True
                    resultData.ErrorCode = Messages.LIMS_INVALID_FIELD_NUM.ToString
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - INVALID FIELD : ES ID")
                    Exit Try
                End If

                '2-Get and Save LIS IDENTIFIER
                Dim myLISId As String = MyClass.xmlHelper.QueryStringValue(myPatientNode, "ci:id", String.Empty)
                If (myLISId.Length > 0) Then
                    myResultRow.BeginEdit()
                    myResultRow.LISPatientID = myLISId
                    myResultRow.PatientID = myLISId.ToUpperBS  'Save as PatientID the received LIS PatientID but in uppercase 
                    myResultRow.EndEdit()
                Else
                    myResultRow.BeginEdit()
                    myResultRow.SetLISPatientIDNull()
                    myResultRow.SetPatientIDNull()
                    myResultRow.EndEdit()
                End If

                '3-Get and Save NAME
                Dim myGivenName As String = MyClass.xmlHelper.QueryStringValue(myPatientNode, "ci:givenName", String.Empty)
                If (myGivenName.Length > 0) Then
                    myResultRow.BeginEdit()
                    myResultRow.FirstName = myGivenName
                    myResultRow.EndEdit()
                    demographicsInformed = True
                Else
                    myResultRow.BeginEdit()
                    myResultRow.FirstName = "-" 'AG 03/10/2013 DO NOT USED "NOT_INFORMED" (JMont 03/10/2013)
                    myResultRow.EndEdit()
                End If

                '4-Get and Save SURNAME
                Dim myFamilyName As String = MyClass.xmlHelper.QueryStringValue(myPatientNode, "ci:familyName", String.Empty)
                If (myFamilyName.Length > 0) Then
                    myResultRow.BeginEdit()
                    myResultRow.LastName = myFamilyName
                    myResultRow.EndEdit()
                    demographicsInformed = True
                Else
                    myResultRow.BeginEdit()
                    myResultRow.LastName = "-" 'AG 03/10/2013 DO NOT USED "NOT_INFORMED" (JMont 03/10/2013)
                    myResultRow.EndEdit()
                End If

                '5-Get, Validate and Save GENDER
                Dim myGender As String = MyClass.xmlHelper.QueryStringValue(myPatientNode, "ci:gender", String.Empty)
                If (myGender.Length > 0) Then
                    myResultRow.BeginEdit()
                    Select Case myGender
                        Case PatientGenders.female.ToString
                            myResultRow.Gender = "F"
                            demographicsInformed = True
                        Case PatientGenders.male.ToString
                            myResultRow.Gender = "M"
                            demographicsInformed = True
                    End Select
                    myResultRow.EndEdit()
                Else
                    myResultRow.BeginEdit()
                    myResultRow.SetGenderNull()
                    myResultRow.EndEdit()
                End If

                '6-Get, Validate and Save DOB
                Dim myBirthday As String = MyClass.xmlHelper.QueryStringValue(myPatientNode, "ci:birthDate", String.Empty)
                If (myBirthday.Length > 0) Then
                    Try
                        Dim myDate As DateTime = DateTime.Parse(myBirthday, System.Globalization.CultureInfo.InvariantCulture)
                        Dim myAge As Integer = CInt(DateTime.Now.Year - myDate.Year)
                        If (myAge >= MinimumAge AndAlso myAge <= MaximumAge) Then
                            myResultRow.BeginEdit()
                            myResultRow.DateOfBirth = myDate
                            myResultRow.EndEdit()
                            demographicsInformed = True
                        Else
                            myResultRow.BeginEdit()
                            myResultRow.SetDateOfBirthNull()
                            myResultRow.EndEdit()
                        End If
                    Catch ex As Exception
                        myResultRow.BeginEdit()
                        myResultRow.SetDateOfBirthNull()
                        myResultRow.EndEdit()
                    End Try
                Else
                    myResultRow.BeginEdit()
                    myResultRow.SetDateOfBirthNull()
                    myResultRow.EndEdit()
                End If

                '7-If Is demographicsInformed = TRUE, Inform PatientsDS.PatientIDType = LIS
                If (demographicsInformed) Then
                    myResultRow.BeginEdit()
                    myResultRow.PatientType = "LIS"
                    myResultRow.EndEdit()
                Else
                    myResultRow.BeginEdit()
                    myResultRow.PatientType = "MAN"
                    myResultRow.EndEdit()
                End If

                '8-Inform field ExternalArrivalDate with the current date and time
                myResultRow.BeginEdit()
                myResultRow.ExternalArrivalDate = Now
                myResultRow.EndEdit()

                'Add the row to a PatientsDS
                myPatientsDS.tparPatients.AddtparPatientsRow(myResultRow)
                myPatientsDS.AcceptChanges()

                'Finally, return the PatientsDS row
                resultData.SetDatos = myPatientsDS.tparPatients(0)

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.DecodeXMLPatientTag", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Decode an Order tag from an XML containing ORDERS sent by LIS. 
        ''' </summary>
        ''' <param name="pXMLNode">Full content of a Node labelled as “patient” in an XML containing information about an Order requested by LIS</param>
        ''' <returns>GlobalDataTO with setdatos as OrdersDS.twksOrdersRow</returns>
        ''' <remarks>
        ''' Created by:  SG 27/02/2013
        ''' Modified by: AG 14/03/2013 - When LIS identifier does not exists, return OrderID NULL instead of error; method returns OrdersDS.twksOrdersRow, not an OrdersDS
        '''              SA 02/04/2013 - Removed the DB Connection parameter;
        ''' </remarks>
        Public Function DecodeXMLOrderTag(ByVal pXMLNode As XmlNode) As GlobalDataTO
            Dim resultData As New GlobalDataTO 'Do not remove New declaration, is needed

            Try
                Dim myOrderNode As XmlNode = pXMLNode

                Dim myOrdersDS As New OrdersDS
                Dim myResultRow As OrdersDS.twksOrdersRow = myOrdersDS.twksOrders.NewtwksOrdersRow

                '1-Get and Save LIS IDENTIFIER 
                Dim myLISOrderId As String = MyClass.xmlHelper.QueryStringValue(myOrderNode, "ci:id", String.Empty)
                If (myLISOrderId.Length > 0) Then
                    myResultRow.BeginEdit()
                    myResultRow.OrderID = myLISOrderId
                    myResultRow.EndEdit()
                Else
                    myResultRow.BeginEdit()
                    myResultRow.SetOrderIDNull()
                    myResultRow.EndEdit()
                End If

                '2-Get and Save ES IDENTIFIER
                Dim myESId As String = MyClass.xmlHelper.QueryAttributeStringValue(myOrderNode, "id", "", String.Empty)
                If (myESId.Length > 0) Then
                    myResultRow.BeginEdit()
                    myResultRow.ExternalOID = myESId
                    myResultRow.EndEdit()
                Else
                    resultData.HasError = True
                    resultData.ErrorCode = Messages.LIMS_INVALID_FIELD_NUM.ToString
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - INVALID FIELD : ES ID")
                    Exit Try
                End If

                '3-Get and Save ORDER DATETIME
                Dim myOrderDate As String = MyClass.xmlHelper.QueryStringValue(myOrderNode, "ci:date", String.Empty)
                If (myOrderDate.Length > 0) Then
                    Try
                        Dim myDate As DateTime = DateTime.Parse(myOrderDate, System.Globalization.CultureInfo.InvariantCulture)
                        myResultRow.BeginEdit()
                        myResultRow.ExternalDateTime = myDate
                        myResultRow.EndEdit()
                    Catch ex As Exception
                        myResultRow.BeginEdit()
                        myResultRow.ExternalDateTime = DateTime.Now
                        myResultRow.EndEdit()
                    End Try
                Else
                    myResultRow.BeginEdit()
                    myResultRow.ExternalDateTime = DateTime.Now
                    myResultRow.EndEdit()
                End If

                'Add the row to an OrdersDS
                myOrdersDS.twksOrders.AddtwksOrdersRow(myResultRow)
                myOrdersDS.AcceptChanges()

                'Finally, return the OrdersDS row
                resultData.SetDatos = myOrdersDS.twksOrders(0)
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.DecodeXMLOrderTag", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Used for BAx00 application in order to get the xml information of the received notification events 
        ''' decoded into a known language for it (Data set, Transference object (TO), …)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="xmlMessage"></param>
        ''' <returns>GlobalDataTO with setdatos as structure known by BAx00 software process</returns>
        ''' <remarks>
        ''' Creation AG 22/02/2013 - empty, only creation
        ''' modified by: TR 28/02/2013
        ''' </remarks>
        Public Function DecodeXMLNotification(ByVal pDBConnection As SqlClient.SqlConnection, ByVal xmlMessage As XmlDocument) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim MyDictionary As New Dictionary(Of GlobalEnumerates.LISNotificationSensors, List(Of String))
                        Dim myStringList As New List(Of String)

                        Dim myXmlHelper As xmlHelper = Nothing
                        myXmlHelper = New xmlHelper("udc", UDCSchema,
                                                    "ci", ClinicalInfoSchema)

                        'Get the xml notification type and decode depending the type
                        Dim xmlNotification As XmlNode = xmlMessage.DocumentElement
                        If myXmlHelper.QueryStringValue(xmlNotification, "@type") = "controlInformation" Then
                            'CONTROL MESSAGES
                            Select Case myXmlHelper.TryQueryStringValue(xmlMessage, "udc:command/udc:information/udc:facility")

                                Case "channel" 'CHANNEL STATUS
                                    myStringList.Add(myXmlHelper.TryQueryStringValue(xmlMessage, "udc:command/udc:information/udc:item/udc:status"))
                                    MyDictionary.Add(LISNotificationSensors.STATUS, myStringList)
                                    Exit Select

                                Case "storage" 'STORAGE

                                    ' Values based on Systelab documentation: Value = 0 or 75 or 80 or 85 or 90 or 95 or 100
                                    If "full" = myXmlHelper.TryQueryStringValue(xmlMessage, "udc:command/udc:information/udc:item/udc:status") Then
                                        '75 or 80 or 85 or 90 or 95 or 100
                                        myStringList.Add(myXmlHelper.TryQueryStringValue(xmlMessage, "udc:command/udc:information/udc:item/udc:value"))
                                    ElseIf "normal" = myXmlHelper.TryQueryStringValue(xmlMessage, "udc:command/udc:information/udc:item/udc:status") Then
                                        myStringList.Add("0")
                                    ElseIf "overloaded" = myXmlHelper.TryQueryStringValue(xmlMessage, "udc:command/udc:information/udc:item/udc:status") Then
                                        myStringList.Add("100")
                                    End If
                                    MyDictionary.Add(LISNotificationSensors.STORAGE, myStringList)
                                    Exit Select

                                Case "message"
                                    For Each myXmlNode As XmlNode In myXmlHelper.QueryXmlNodeList(xmlMessage, "udc:command/udc:information/udc:item")
                                        myStringList.Clear() 'AG + TR 28/04/2013 

                                        Select Case myXmlHelper.TryQueryStringValue(myXmlNode, "udc:status")

                                            Case "delivered" 'DELIVERED
                                                myStringList.Add(myXmlHelper.TryQueryStringValue(myXmlNode, "udc:id") & GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR & _
                                                                  myXmlHelper.TryQueryStringValue(myXmlNode, "udc:status"))
                                                MyDictionary.Add(LISNotificationSensors.DELIVERED, myStringList)

                                            Case "undeliverable" 'UNDELIVERABLED
                                                ' Values based on Systelab documentation: Value = MessageID & SEPARATOR & "delivered" 
                                                myStringList.Add(myXmlHelper.TryQueryStringValue(myXmlNode, "udc:id") & GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR & _
                                                                 myXmlHelper.TryQueryStringValue(myXmlNode, "udc:status"))
                                                MyDictionary.Add(LISNotificationSensors.UNDELIVERED, myStringList)

                                            Case "unresponded" 'UNRESPONDED
                                                ' Values based on Systelab documentation: Value = MessageID & SEPARATOR & "unresponded"
                                                myStringList.Add(myXmlHelper.TryQueryStringValue(myXmlNode, "udc:id") & GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR & _
                                                                 myXmlHelper.TryQueryStringValue(myXmlNode, "udc:status"))
                                                MyDictionary.Add(LISNotificationSensors.UNRESPONDED, myStringList)

                                            Case "pendingmessages" 'PENDING MESSAGES
                                                ' Values based on Systelab documentation: Value = MessageID & SEPARATOR & "delivering" or "undelivered"
                                                myStringList.Add(myXmlHelper.TryQueryStringValue(myXmlNode, "udc:id") & GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR & _
                                                                 myXmlHelper.TryQueryStringValue(myXmlNode, "udc:status"))
                                                MyDictionary.Add(LISNotificationSensors.PENDINGMESSAGES, myStringList)

                                            Case "deleted" 'DELETED
                                                ' Values based on Systelab documentation: Value = MessageID & SEPARATOR & "deleted"
                                                myStringList.Add(myXmlHelper.TryQueryStringValue(myXmlNode, "udc:id") & GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR & _
                                                                 myXmlHelper.TryQueryStringValue(myXmlNode, "udc:status"))
                                                MyDictionary.Add(LISNotificationSensors.DELETED, myStringList)
                                        End Select
                                    Next



                                    Exit Select
                            End Select

                        ElseIf myXmlHelper.QueryStringValue(xmlNotification, "@type") = "message" AndAlso _
                               myXmlHelper.TryQueryStringValue(xmlMessage, "udc:command/udc:header/udc:metadata/udc:container/udc:action") = "program" Then

                            'QUERY RESPONSE
                            'AG 18/03/2013 - Integration with STL remove triggerMessage condition (sometimes informed and sometimes not depending the error scenario)
                            'If Not myXmlHelper.TryQueryStringValue(xmlMessage, "udc:command/udc:header/udc:metadata/udc:container/udc:triggerMessage/udc:id") = "" AndAlso _
                            'Not myXmlHelper.TryQueryStringValue(xmlMessage, "udc:command/udc:error/udc:id") = "" Then
                            If Not myXmlHelper.TryQueryStringValue(xmlMessage, "udc:command/udc:error/udc:id") = "" Then
                                'INVALID
                                ' Values Value = TriggerMessage & SEPARATOR 
                                '              & ErrorType

                                myStringList.Add(myXmlHelper.TryQueryStringValue(xmlMessage, "udc:command/udc:header/udc:metadata/udc:container/udc:triggerMessage/udc:id") & _
                                                           GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR & myXmlHelper.TryQueryStringValue(xmlMessage, "udc:command/udc:error/udc:id"))
                                MyDictionary.Add(LISNotificationSensors.INVALID, myStringList)
                            ElseIf Not xmlHelper.TryQueryStringValue(xmlMessage, "udc:command/udc:body/udc:errortype") = "" Then
                                'INVALID second case

                                myStringList.Add(myXmlHelper.TryQueryStringValue(xmlMessage, _
                                                           "udc:command/udc:header/udc:metadata/udc:container/udc:triggerMessage/udc:id") & _
                                                           GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR & _
                                                           myXmlHelper.TryQueryStringValue(xmlMessage, "udc:command/udc:body/udc:errortype"))
                                MyDictionary.Add(LISNotificationSensors.INVALID, myStringList)

                            ElseIf myXmlHelper.QueryAttributeStringValue(xmlMessage, "set", "udc:command/udc:body/udc:message/ci:service/ci:specimen", "") = "all" Then
                                'QUERY ALL 
                                'Validate the status 

                                '   Always return all fields Value = TriggerMessage & SEPARATOR &  Description & SEPARATOR & "all" & SEPARATOR & ErrorType

                                ' Value = TriggerMessage & SEPARATOR 
                                '      &  Description & SEPARATOR 
                                '      & "all" & SEPARATOR 
                                '      & ErrorType              (If description = done then ErrorType = "")

                                Dim description As String = myXmlHelper.TryQueryStringValue(xmlMessage, "udc:command/udc:body/udc:message/ci:service/ci:status")
                                Dim errorMsg As String = CStr(IIf(description = "done", "", myXmlHelper.TryQueryStringValue(xmlMessage, "udc:command/udc:body/udc:message/ci:service/ci:error")))

                                myStringList.Add(myXmlHelper.TryQueryStringValue(xmlMessage, "udc:command/udc:header/udc:metadata/udc:container/udc:triggerMessage/udc:id") & _
                                                           GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR & description & _
                                                           GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR & "all" & _
                                                           GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR & errorMsg)

                                MyDictionary.Add(LISNotificationSensors.QUERYALL, myStringList)

                            ElseIf myXmlHelper.QueryAttributeStringValue(xmlMessage, "set", "udc:command/udc:body/udc:message/ci:service/ci:specimen", "") = "particular" Then
                                'HOSTQUERY.
                                Dim myStringValues As String = ""
                                'Recorrer todos los espec. que hay y meterlo en el string list.


                                ' SpecimenID = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:body/udc:message/ci:service/ci:specimen")
                                ' ErroType (only when description is cannotBeDone) = xmlHelper.TryQueryStringValue(xmlDoc, "udc:command/udc:body/udc:message/ci:service/ci:error")

                                For Each myXmlNode As XmlNode In myXmlHelper.QueryXmlNodeList(xmlMessage, "udc:command/udc:body/udc:message/ci:service/ci:specimen")
                                    myStringValues = "" 'AG + TR 28/04/2013 

                                    ' Always return all fields
                                    ' Values based on Systelab documentation: 
                                    ' Value = TriggerMessage & SEPARATOR 
                                    '       & Description & SEPARATOR               ( Description = "" for hostquery response)
                                    '       & SpecimenID & SEPARATOR 
                                    '       & ErrorType.                            ( If status = cannotBeDone, Error ELSE ERROR = "" )


                                    Dim errorMsg As String = CStr(IIf(myXmlHelper.TryQueryStringValue(xmlMessage, "udc:command/udc:body/udc:message/ci:service/ci:status") = "cannotBeDone", _
                                                                     myXmlHelper.TryQueryStringValue(xmlMessage, "udc:command/udc:body/udc:message/ci:service/ci:error"),
                                                                     ""))

                                    myStringValues = myXmlHelper.TryQueryStringValue(xmlMessage, "udc:command/udc:header/udc:metadata/udc:container/udc:triggerMessage/udc:id") & _
                                                            GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR & "" & _
                                                            GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR & myXmlHelper.TryQueryStringValue(myXmlNode, "ci:id") & _
                                                            GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR & errorMsg

                                    myStringList.Add(myStringValues)
                                Next

                                MyDictionary.Add(LISNotificationSensors.HOSTQUERY, myStringList)

                            End If
                        End If
                        'Set the value to the global data
                        resultData.SetDatos = MyDictionary
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.DecodeXMLNotification", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Decode the errors returned by the LIS Driver Manager on an Exception. 
        ''' </summary>
        ''' <param name="pExceptionString"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 08/03/2013</remarks>
        Public Function DecodeXMLExceptions(ByVal pExceptionString As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                Dim myXmlDoc As New XmlDocument
                Try
                    myXmlDoc.LoadXml(pExceptionString)
                Catch ex As Exception
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                    resultData.ErrorMessage = ex.Message
                    Exit Try
                End Try


                If myXmlDoc.FirstChild IsNot Nothing Then
                    myXmlDoc.FirstChild.Attributes.RemoveAll()

                    Dim myXDoc As XDocument = XDocument.Parse(myXmlDoc.FirstChild.OuterXml)
                    Dim TagName As XName = XName.Get("error", "")
                    Dim myErrors As List(Of LimsExceptionTO) = (From item In myXDoc.Descendants(TagName) _
                                                                Select New LimsExceptionTO() _
                                                                With
                                                                {
                                                                    .Facility = item.Element("facility").Value,
                                                                    .Category = item.Element("category").Value,
                                                                    .Severity = item.Element("severity").Value,
                                                                    .Computer = item.Element("computer").Value,
                                                                    .hrCode = item.Element("hr").Descendants("code").Value,
                                                                    .hrDescription = item.Element("hr").Descendants("description").Value,
                                                                    .FileName = item.Element("file").Value,
                                                                    .FileLine = item.Element("line").Value,
                                                                    .ProcessName = item.Element("process").Descendants("name").Value,
                                                                    .ProcessID = item.Element("process").Descendants("id").Value,
                                                                    .ThreadID = item.Element("thread").Descendants("id").Value,
                                                                    .ErrorDateTime = item.Element("time").Value,
                                                                    .ApplicationID = item.Element("applicationId").Value,
                                                                    .ModulePath = item.Element("module").Value,
                                                                    .Component = item.Element("component").Value,
                                                                    .GUID = item.Element("guid").Value _
                                                                }).ToList()



                    Dim myXmlNode As XmlNode = TryCast(myXmlDoc.DocumentElement, XmlNode)
                    Dim myXmlHelper As New xmlHelper(myXmlNode.GetPrefixOfNamespace(myXmlNode.NamespaceURI), TraceSchema, Nothing, Nothing)
                    Dim myXmlNodeList As XmlNodeList = myXmlNode.ChildNodes
                    If myXmlNodeList IsNot Nothing Then
                        For Each E As XmlNode In myXmlNodeList
                            Dim myError As New LimsExceptionTO
                            With myError
                                .Facility = myXmlHelper.QueryStringValue(E, "facility", "")
                                .Category = myXmlHelper.QueryStringValue(E, "category", "")
                                .Severity = myXmlHelper.QueryStringValue(E, "severity", "")
                                .Computer = myXmlHelper.QueryStringValue(E, "computer", "")
                                .hrCode = myXmlHelper.QueryStringValue(E, "hr/code", "")
                                .hrDescription = myXmlHelper.QueryStringValue(E, "hr/description", "")
                                .FileName = myXmlHelper.QueryStringValue(E, "file", "")
                                .FileLine = myXmlHelper.QueryStringValue(E, "line", "")
                                .ProcessName = myXmlHelper.QueryStringValue(E, "process/name", "")
                                .ProcessID = myXmlHelper.QueryStringValue(E, "process/id", "")
                                .ThreadID = myXmlHelper.QueryStringValue(E, "thread/id", "")
                                .ErrorDateTime = myXmlHelper.QueryStringValue(E, "time", "")
                                .ApplicationID = myXmlHelper.QueryStringValue(E, "applicationId", "")
                                .ModulePath = myXmlHelper.QueryStringValue(E, "module", "")
                                .Component = myXmlHelper.QueryStringValue(E, "component", "")
                                .GUID = myXmlHelper.QueryStringValue(E, "guid", "")
                            End With
                        Next

                        '  <error>
                        '      <facility>udc</facility>
                        '      <category>application</category>
                        '      <severity>error</severity>
                        '      <computer>AUXSOFTWARE1</computer>
                        '      <hr>
                        '        <code>0x80004005</code>
                        '        <description>
                        '          Error no especificado
                        '        </description>
                        '      </hr>
                        '      <file>.\ChannelMgr.cpp</file>
                        '      <line>836</line>
                        '      <process>
                        '        <name>C:\Users\Sergio Garcia\Documents\BAx00 v1.1\AX00\PresentationUSR\bin\x86\Debug\BA400User.vshost.exe</name>
                        '        <pid>3652</pid>
                        '      </process>
                        '      <thread>
                        '        <id>6260</id>
                        '      </thread>
                        '      <time>2013-03-08T11:40:00.125</time>
                        '      <applicationId>BAx00</applicationId>
                        '      <module>C:\Program Files (x86)\Common Files\Systelab\Synapse\Core\NteCommunicationCoreModule.dll</module>
                        '      <component>NteCommunicationCoreModule.ChannelMgr.3</component>
                        '      <guid>2EB199E7-ED25-4C8D-83BD-0898416A779F</guid>
                        '</error>

                        resultData.SetDatos = myErrors 'LISXmlExceptionTO

                    End If
                    myErrors = Nothing
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.DecodeXMLExceptions", EventLogEntryType.Error, False)


            End Try

            Return resultData
        End Function
#End Region

#End Region

#Region "Private methods"



        ''' <summary>
        ''' Created the root elemet of the xml documentes (command)
        ''' </summary>
        ''' <param name="XmlDoc"></param>
        ''' <param name="pTypeAttribute"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 26/02/2013</remarks>
        Private Function CreateRootElement(ByRef XmlDoc As XmlDocument, _
                                           ByVal pTypeAttribute As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Try

                Dim Command As XmlElement = XmlDoc.CreateElement("command")
                Dim TypeAttr As XmlAttribute = XmlDoc.CreateAttribute("type")
                TypeAttr.Value = pTypeAttribute
                Dim xmlnsAttr As XmlAttribute = XmlDoc.CreateAttribute("xmlns")
                xmlnsAttr.Value = UDCSchema
                Command.Attributes.Append(TypeAttr)
                Command.Attributes.Append(xmlnsAttr)

                resultData.SetDatos = Command

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.CreateRootElement", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Creates a common Header node for all xml documents
        ''' </summary>
        ''' <param name="XmlDoc"></param>
        ''' <param name="pMessageID"></param>
        ''' <param name="pPriority"></param>
        ''' <param name="pTransMode"></param>
        ''' <param name="pAction"></param>
        ''' <param name="pDatetime"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 26/02/2013
        ''' Modified by SGM 06/02/2013 - add new input parameters
        ''' </remarks>
        Private Function CreateHeaderNode(ByRef XmlDoc As XmlDocument, _
                                  ByVal pMessageID As String, _
                                  ByVal pProcessMode As ProcessModes, _
                                  ByVal pPriority As Integer, _
                                  ByVal pTransMode As TransmissionModes, _
                                  ByVal pAction As Actions, _
                                  ByVal pObject As MessageObjects, _
                                  ByVal pDatetime As DateTime) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try

                '"<header xmlns="http://www.nte.es/schema/udc-interface-v1.0">" &
                '   "<id>" &  Message ID & "</id>" &
                '	"<channel>"
                '       "<id>" &  Channel ID & "</id>
                '   "</channel>" &
                '	"<metadata>
                '       "<container>" &
                '	        "<processMode>production</processMode>" &
                '           "<priority>2</priority>" & 
                '	        "<transmissionMode>unsolicited</transmissionMode>" &
                '	        "<action>request</action>" &
                '	        "<object>workOrder</object>" &
                '	        "<date>" &  DateTime  & "</date>" &
                '       "</container>" &
                '   "</metadata>" &
                '"</header>" &


                Dim Header As XmlNode = XmlDoc.CreateElement("header")
                Dim xmlnsAttr As XmlAttribute = XmlDoc.CreateAttribute("xmlns")
                xmlnsAttr.Value = UDCSchema
                Header.Attributes.Append(xmlnsAttr)

                Dim HeaderId As XmlNode = XmlDoc.CreateElement("id")
                HeaderId.InnerText = pMessageID
                Header.AppendChild(HeaderId)

                Dim Channel As XmlNode = XmlDoc.CreateElement("channel")
                Dim ChannelId As XmlNode = XmlDoc.CreateElement("id")
                ChannelId.InnerText = MyClass.ChannelId
                Channel.AppendChild(ChannelId)
                Header.AppendChild(Channel)

                Dim Metadata As XmlNode = XmlDoc.CreateElement("metadata")
                Dim Container As XmlNode = XmlDoc.CreateElement("container")

                Dim ProcessMode As XmlNode = XmlDoc.CreateElement("processMode")
                ProcessMode.InnerText = pProcessMode.ToString
                Container.AppendChild(ProcessMode)

                Dim Priority As XmlNode = XmlDoc.CreateElement("priority")
                Priority.InnerText = pPriority.ToString
                Container.AppendChild(Priority)

                Dim TransmissionMode As XmlNode = XmlDoc.CreateElement("transmissionMode")
                TransmissionMode.InnerText = pTransMode.ToString
                Container.AppendChild(TransmissionMode)

                Dim Action As XmlNode = XmlDoc.CreateElement("action")
                Action.InnerText = pAction.ToString
                Container.AppendChild(Action)

                Dim _Object As XmlNode = XmlDoc.CreateElement("object")
                _Object.InnerText = pObject.ToString
                Container.AppendChild(_Object)

                Dim _Date As XmlNode = XmlDoc.CreateElement("date")
                _Date.InnerText = pDatetime.ToXSDString()
                Container.AppendChild(_Date)

                Metadata.AppendChild(Container)
                Header.AppendChild(Metadata)

                resultData.SetDatos = Header

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.CreateHeaderNode", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Creates the service node with the informed data
        ''' </summary>
        ''' <param name="XmlDoc">Xml document in which the node and subnodes will be added</param>
        ''' <param name="pTestMappingDS">Test mapping for LIS</param>
        ''' <param name="pConfigMappingDS">Config mapping for LIS</param>
        ''' <param name="pRole">Sample Class: patient or QC</param>
        ''' <param name="pEndDate">Result datetime</param>
        ''' <param name="pAlarms">Remark codes</param>
        ''' <param name="pResultsRow">Results data row</param>
        ''' <param name="pHisResultsRow">History Results data row</param>
        ''' <param name="pRefRanges">Related reference ranges</param>
        ''' <param name="pSpecimenID"></param>
        ''' <param name="pPatientID"></param>
        ''' <param name="pOrderID"></param>
        ''' <param name="pIsManual">Automatic(false) or Manual(true)</param>
        ''' <param name="pControlsDS"></param>
        ''' <param name="pAwosID"></param>
        ''' <param name="pLISMappingError"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by  SG 06/03/2013
        ''' Modified by XB 13/03/2013 - Add Controls functionality
        ''' Modified by AG 24/04/2013 - Add parameter AwosID and inform it in Service id tag when different than ""
        ''' Modified by AG 03/05/2013 - Historical results could contain CONCValue = NULL
        ''' Modified by DL 16/05/2013 - Modify system decimal separator by "."
        ''' AG 27/05/2013 - RefRanges father is Result instead of QualifyingElement
        ''' AG 29/09/2014 - BA-1440 part1 - Inform the new byref parameter pLISMappingError
        ''' AG 30/09/2014 - All CreateLogActivity as EventLogEntryType.Information but the exception one
        ''' </remarks>
        Private Function CreateServiceNode(ByRef XmlDoc As XmlDocument, _
                                           ByVal pTestMappingDS As AllTestsByTypeDS, _
                                           ByVal pConfigMappingDS As LISMappingsDS, _
                                           ByVal pRole As TestSampleClass, _
                                           ByVal pEndDate As DateTime, _
                                           ByVal pAlarms As List(Of ResultsDS.vwksResultsAlarmsRow), _
                                           ByVal pResultsRow As ResultsDS.vwksResultsRow, _
                                           ByVal pHisResultsRow As HisWSResultsDS.vhisWSResultsRow, _
                                           ByVal pRefRanges As TestRefRangesDS, _
                                           ByVal pSpecimenID As String, _
                                           ByVal pPatientID As String, _
                                           ByVal pOrderID As String, _
                                           ByVal pIsManual As Boolean, _
                                           ByVal pControlsDS As ControlsDS, _
                                           ByVal pAwosID As String, ByRef pLISMappingError As Boolean) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim myResultRow As ResultsDS.vwksResultsRow = Nothing
            Dim IsHistorical As Boolean = (pHisResultsRow IsNot Nothing)
            Dim myLogAcciones As New ApplicationLogManager()

            Try
                pLISMappingError = False 'AG 29/09/2014 - BA-1440 initial value, not mapping errors
                If pResultsRow IsNot Nothing Then
                    'ws
                    myResultRow = pResultsRow
                Else
                    'historic
                    If IsHistorical Then
                        Dim myHisResultsDS As New ResultsDS
                        myResultRow = myHisResultsDS.vwksResults.NewvwksResultsRow

                        With myResultRow
                            .BeginEdit()
                            .StatFlag = pHisResultsRow.StatFlag
                            .TestID = pHisResultsRow.TestID
                            .TestName = pHisResultsRow.TestName
                            .TestType = pHisResultsRow.TestType
                            .SampleID = pHisResultsRow.PatientID
                            .SampleType = pHisResultsRow.SampleType
                            .MeasureUnit = pHisResultsRow.MeasureUnit
                            If Not pHisResultsRow.IsHistOrderTestIDNull Then
                                .OrderID = pHisResultsRow.HistOrderTestID.ToString
                            Else
                                resultData.HasError = True
                                resultData.ErrorCode = Messages.TESTS_NOT_FOUND.ToString
                                Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - TESTS NOT FOUND")
                                Exit Try
                            End If
                            'AG 03/05/2013 - Historical results DS could contain CONCValue as NULL (offsystem tests)
                            '.CONC_Value = pHisResultsRow.CONCValue
                            '.ManualResultFlag = (Not pHisResultsRow.IsManualResultNull AndAlso pHisResultsRow.IsManualResultTextNull)
                            If Not pHisResultsRow.IsCONCValueNull Then .CONC_Value = pHisResultsRow.CONCValue
                            .ManualResultFlag = pHisResultsRow.ManualResultFlag
                            'AG 03/05/2013

                            If .ManualResultFlag Then
                                If Not pHisResultsRow.IsManualResultNull Then .ManualResult = pHisResultsRow.ManualResult
                                If Not pHisResultsRow.IsManualResultTextNull Then .ManualResultText = pHisResultsRow.ManualResultText
                            End If
                            .EndEdit()
                        End With
                        myHisResultsDS.AcceptChanges()
                    End If
                End If

                If myResultRow IsNot Nothing Then

                    'status
                    Dim myTestStatus As TestStatusFlags
                    If myResultRow.StatFlag = False Then
                        myTestStatus = TestStatusFlags.normal
                    Else
                        myTestStatus = TestStatusFlags.stat
                    End If

                    Dim myMappedUnit As String = ""
                    Dim myMappedSampleType As String = ""
                    Dim myMappedTestId As String = ""
                    Dim itMustSetToNotSent As Boolean = False
                    Dim myHisResultsDlg As New HisWSResultsDelegate

                    If IsHistorical AndAlso Not pIsManual Then
                        'get mapping data from the configuration when the results were obtained
                        myMappedUnit = pHisResultsRow.LISUnits

                        If Not pHisResultsRow.IsLISSampleTypeNull OrElse pHisResultsRow.LISSampleType.Trim = String.Empty Then
                            myMappedSampleType = pHisResultsRow.LISSampleType
                        Else
                            itMustSetToNotSent = True
                        End If

                        If Not itMustSetToNotSent Then
                            If Not pHisResultsRow.IsLISTestNameNull OrElse pHisResultsRow.LISTestName.Trim = String.Empty Then
                                myMappedTestId = pHisResultsRow.LISTestName
                            Else
                                itMustSetToNotSent = True
                            End If
                        End If

                    Else

                        'get mapping data from current configuration
                        Dim myLISMappingDelegate As New LISMappingsDelegate
                        Dim myAllTestMappingDelegate As New AllTestByTypeDelegate

                        'Units
                        resultData = myLISMappingDelegate.GetLISUnits(pConfigMappingDS, myResultRow.MeasureUnit)
                        If resultData.HasError Then
                            myMappedUnit = ""
                        Else
                            myMappedUnit = CStr(resultData.SetDatos)
                        End If

                        'Sample Type 
                        resultData = myLISMappingDelegate.GetLISSampleType(pConfigMappingDS, myResultRow.SampleType)
                        If resultData.HasError Then
                            myLogAcciones.CreateLogActivity("History Result not exported: mapped LIS Sample Type is missing", "ESxmlTranslator.CreateServiceNode", EventLogEntryType.Information, False)
                            itMustSetToNotSent = True
                        Else
                            myMappedSampleType = CStr(resultData.SetDatos)
                        End If

                        If Not itMustSetToNotSent Then
                            'Test Id 
                            resultData = myAllTestMappingDelegate.GetLISTestID(pTestMappingDS, myResultRow.TestID, myResultRow.TestType)
                            If resultData.HasError Then
                                myLogAcciones.CreateLogActivity("History Result not exported: mapped LIS Test ID is missing", "ESxmlTranslator.CreateServiceNode", EventLogEntryType.Information, False)
                                itMustSetToNotSent = True
                            Else
                                myMappedTestId = CStr(resultData.SetDatos)
                            End If
                        End If

                        If myMappedSampleType = String.Empty Or myMappedTestId = String.Empty Then
                            itMustSetToNotSent = True
                        End If

                    End If

                    'in case of mapping is missing, set to 'NOTSENT'
                    If itMustSetToNotSent Then
                        pLISMappingError = True 'AG 29/09/2014 - BA-1440 inform LISMapping error!!!
                        Dim myNotSentHisResultsDS As New ResultsDS
                        Dim myRow As ResultsDS.vwksResultsRow = myNotSentHisResultsDS.vwksResults.NewvwksResultsRow()
                        With myRow
                            .BeginEdit()
                            If Not IsHistorical Then
                                .OrderTestID = myResultRow.OrderTestID
                            Else
                                .OrderTestID = pHisResultsRow.HistOrderTestID
                            End If
                            .EndEdit()
                        End With
                        myNotSentHisResultsDS.vwksResults.AddvwksResultsRow(myRow)
                        myNotSentHisResultsDS.AcceptChanges()
                        resultData = myHisResultsDlg.UpdateExportStatus(Nothing, myNotSentHisResultsDS, "NOTSENT")
                        Exit Try
                    End If

                    'START XML
                    Dim Service As XmlNode = XmlDoc.CreateElement("ci", "service", ClinicalInfoSchema)

                    Dim ServiceId As XmlNode = XmlDoc.CreateElement("ci", "id", ClinicalInfoSchema)
                    If pAwosID <> "" Then ServiceId.InnerText = pAwosID 'AG 24/04/2013 - if not informed '', else awosid
                    Service.AppendChild(ServiceId)

                    Dim ServiceStatus As XmlNode = XmlDoc.CreateElement("ci", "status", ClinicalInfoSchema)
                    ServiceStatus.InnerText = "done" 'by default 'done'
                    Service.AppendChild(ServiceStatus)

                    Dim Test As XmlNode = XmlDoc.CreateElement("ci", "test", ClinicalInfoSchema)

                    Dim TestId As XmlNode = XmlDoc.CreateElement("ci", "id", ClinicalInfoSchema)
                    TestId.InnerText = myMappedTestId
                    Test.AppendChild(TestId)

                    Dim TestRole As XmlNode = XmlDoc.CreateElement("ci", "role", ClinicalInfoSchema)
                    TestRole.InnerText = pRole.ToString
                    Test.AppendChild(TestRole)

                    Dim TestPriority As XmlNode = XmlDoc.CreateElement("ci", "priority", ClinicalInfoSchema)
                    TestPriority.InnerText = myTestStatus.ToString
                    Test.AppendChild(TestPriority)

                    Dim ResultCollection As XmlNode = XmlDoc.CreateElement("ci", "resultCollection", ClinicalInfoSchema)

                    Dim EndDate As XmlNode = XmlDoc.CreateElement("ci", "endDate", ClinicalInfoSchema)
                    EndDate.InnerText = pEndDate.ToXSDString()
                    ResultCollection.AppendChild(EndDate)

                    Dim Result As XmlNode = XmlDoc.CreateElement("ci", "result", ClinicalInfoSchema)

                    Dim ResultStatus As XmlNode = XmlDoc.CreateElement("ci", "status", ClinicalInfoSchema)
                    ResultStatus.InnerText = "final" 'For March integration fixed in ‘final’. PENDING DEFINE what does it means and what cases are “error”
                    Result.AppendChild(ResultStatus)

                    Dim IsCorrection As XmlNode = XmlDoc.CreateElement("ci", "isCorrection", ClinicalInfoSchema)
                    IsCorrection.InnerText = "false"    'Fixed to ‘false’ in v2
                    Result.AppendChild(IsCorrection)


                    'SGM 10/07/2013
                    MyClass.CreateFlagsNode(XmlDoc, Result, pAlarms, pConfigMappingDS)

                    'Dim ResultFlag As XmlNode = XmlDoc.CreateElement("ci", "flag", ClinicalInfoSchema)
                    'Dim ResultFlagId As XmlNode = XmlDoc.CreateElement("ci", "id", ClinicalInfoSchema)

                    'If Not pAlarms Is Nothing AndAlso pAlarms.Count > 0 Then
                    '    ' ES library is not capable to accept a lot of remarks, so by now this functionality is disabled
                    '    'For Each rowAlarm As ResultsDS.vwksResultsAlarmsRow In pAlarms
                    '    '    Select Case rowAlarm.AlarmID
                    '    '        Case CalculationRemarks.CONC_REMARK7.ToString : ResultFlagId.InnerText = "L" : Exit For
                    '    '        Case CalculationRemarks.CONC_REMARK8.ToString : ResultFlagId.InnerText = "H" : Exit For
                    '    '        Case CalculationRemarks.CONC_REMARK9.ToString : ResultFlagId.InnerText = "LL" : Exit For
                    '    '        Case CalculationRemarks.CONC_REMARK10.ToString : ResultFlagId.InnerText = "HH" : Exit For
                    '    '    End Select
                    '    'Next
                    '    ResultFlagId.InnerText = "See Analyzer remarks for this LIS order"
                    'End If

                    'ResultFlag.AppendChild(ResultFlagId)

                    'Result.AppendChild(ResultFlag)

                    'SGM 10/07/2013




                    'PENDING DEFINE what does it means and what cases are “error”. For March integration remove tag
                    'Dim ResultError As XmlNode = XmlDoc.CreateElement("ci", "error", ClinicalInfoSchema)
                    'Dim ResultErrorId As XmlNode = XmlDoc.CreateElement("ci", "id", ClinicalInfoSchema)
                    'ResultErrorId.InnerText = "with error"
                    'ResultError.AppendChild(ResultErrorId)
                    'Result.AppendChild(ResultError)

                    'For historical pHistDataDS do the same with equivalent fields
                    Dim ResultQElement As XmlNode = XmlDoc.CreateElement("ci", "qualifyingElement", ClinicalInfoSchema)
                    Dim QElementValue As XmlNode = XmlDoc.CreateElement("ci", "value", ClinicalInfoSchema)
                    Dim myDAO As New DAOBase 'DL 16/05/2013

                    'Use pResultsDS, if ManualResultFlag = False use CONC_Value, else 
                    'use ManualResult or ManualResultText.(For historical pHistDataDS same fields)
                    If Not myResultRow.ManualResultFlag Then
                        'DL 16/05/2013
                        'QElementValue.InnerText = myResultRow.CONC_Value.ToString(InvariantCulture)
                        QElementValue.InnerText = myDAO.ReplaceNumericString(myResultRow.CONC_Value).ToString(InvariantCulture)
                        'DL 16/05/2013
                    Else
                        If myResultRow.IsManualResultNull AndAlso Not myResultRow.IsManualResultTextNull Then
                            QElementValue.InnerText = myResultRow.ManualResultText
                        ElseIf Not myResultRow.IsManualResultNull Then
                            QElementValue.InnerText = myResultRow.ManualResult.ToString(InvariantCulture)
                            'AG 14/05/2013 - WS with a offsystem test has ManualResultText and ManualResult = NULL and informed CONC_Value (I do not know why!!)
                        ElseIf Not myResultRow.IsCONC_ValueNull Then
                            'DL 16/05/2013
                            'QElementValue.InnerText = myResultRow.CONC_Value.ToString(InvariantCulture)
                            QElementValue.InnerText = myDAO.ReplaceNumericString(myResultRow.CONC_Value).ToString(InvariantCulture)
                            'DL 16/05/2013
                        End If
                    End If
                    ResultQElement.AppendChild(QElementValue)

                    Dim QElementUnit As XmlNode = XmlDoc.CreateElement("ci", "unit", ClinicalInfoSchema)
                    QElementUnit.InnerText = myMappedUnit
                    ResultQElement.AppendChild(QElementUnit)

                    Result.AppendChild(ResultQElement)

                    'Dim QElementRefRange As XmlNode = XmlDoc.CreateElement("ci", "referenceRange", ClinicalInfoSchema)

                    ''Take a look how to use it in method MoveWSResultsToHISTModule in ResultsDelegate..(For historical use pHistDataDS.MaxRefRange
                    'Dim HiRefRange As XmlNode = XmlDoc.CreateElement("ci", "higherRange", ClinicalInfoSchema)
                    'Dim LoRefRange As XmlNode = XmlDoc.CreateElement("ci", "lowerRange", ClinicalInfoSchema)

                    If pRefRanges IsNot Nothing Then

                        'SGM 28/06/2013 - not to inform Ref Reanges tag in case of missing
                        Dim QElementRefRange As XmlNode = XmlDoc.CreateElement("ci", "referenceRange", ClinicalInfoSchema)
                        Dim HiRefRange As XmlNode = XmlDoc.CreateElement("ci", "higherRange", ClinicalInfoSchema)
                        Dim LoRefRange As XmlNode = XmlDoc.CreateElement("ci", "lowerRange", ClinicalInfoSchema)

                        Dim myRefRangesRow As TestRefRangesDS.tparTestRefRangesRow
                        If pRefRanges.tparTestRefRanges.Rows.Count > 0 Then
                            myRefRangesRow = TryCast(pRefRanges.tparTestRefRanges.Rows(0), TestRefRangesDS.tparTestRefRangesRow)

                            'higher
                            Dim HiRefValue As XmlNode = XmlDoc.CreateElement("ci", "value", ClinicalInfoSchema)
                            If IsHistorical Then
                                If Not pHisResultsRow.IsMaxRefRangeNull Then
                                    'DL 16/05/2013
                                    'HiRefValue.InnerText = pHisResultsRow.MaxRefRange.ToString(InvariantCulture)
                                    HiRefValue.InnerText = myDAO.ReplaceNumericString(pHisResultsRow.MaxRefRange).ToString(InvariantCulture)
                                    'DL 16/05/2013
                                Else
                                    HiRefValue.InnerText = String.Empty
                                End If
                            Else
                                'DL 16/05/2013
                                'HiRefValue.InnerText = myRefRangesRow.NormalUpperLimit.ToString(InvariantCulture)
                                If myRefRangesRow.NormalUpperLimit > -1 Then
                                    HiRefValue.InnerText = myDAO.ReplaceNumericString(myRefRangesRow.NormalUpperLimit).ToString(InvariantCulture)
                                End If
                                'DL 16/05/2013
                            End If

                            If Not String.IsNullOrEmpty(HiRefValue.InnerText) Then
                                HiRefRange.AppendChild(HiRefValue)
                                Dim HiRefUnit As XmlNode = XmlDoc.CreateElement("ci", "unit", ClinicalInfoSchema)
                                HiRefUnit.InnerText = myMappedUnit
                                HiRefRange.AppendChild(HiRefUnit)
                            End If


                            'lower
                            Dim LoRefValue As XmlNode = XmlDoc.CreateElement("ci", "value", ClinicalInfoSchema)
                            If IsHistorical Then
                                If Not pHisResultsRow.IsMinRefRangeNull Then
                                    'DL 16/05/2013
                                    'LoRefValue.InnerText = pHisResultsRow.MinRefRange.ToString(InvariantCulture)
                                    LoRefValue.InnerText = myDAO.ReplaceNumericString(pHisResultsRow.MinRefRange).ToString(InvariantCulture)
                                    'DL 16/05/2013
                                Else
                                    LoRefValue.InnerText = String.Empty
                                End If
                            Else
                                'DL 16/05/2013
                                'LoRefValue.InnerText = myRefRangesRow.NormalLowerLimit.ToString(InvariantCulture)
                                If myRefRangesRow.NormalLowerLimit > -1 Then
                                    LoRefValue.InnerText = myDAO.ReplaceNumericString(myRefRangesRow.NormalLowerLimit).ToString(InvariantCulture)
                                End If
                                'DL 16/05/2013
                            End If

                            If Not String.IsNullOrEmpty(LoRefValue.InnerText) Then
                                LoRefRange.AppendChild(LoRefValue)
                                Dim LoRefUnit As XmlNode = XmlDoc.CreateElement("ci", "unit", ClinicalInfoSchema)
                                LoRefUnit.InnerText = myMappedUnit
                                LoRefRange.AppendChild(LoRefUnit)
                            End If

                        End If

                        'SGM 28/06/2013 - not to inform Ref Reanges tag in case of missing
                        If HiRefRange.ChildNodes.Count = 2 AndAlso LoRefRange.ChildNodes.Count = 2 Then
                            QElementRefRange.AppendChild(HiRefRange)
                            QElementRefRange.AppendChild(LoRefRange)
                            Result.AppendChild(QElementRefRange)
                        End If

                    End If

                    'QElementRefRange.AppendChild(HiRefRange)
                    'QElementRefRange.AppendChild(LoRefRange)

                    'ResultQElement.AppendChild(QElementRefRange) 'AG 27/05/2013 - RefRanges father is Result instead of QualifyingElement
                    'Result.AppendChild(ResultQElement)
                    'Result.AppendChild(QElementRefRange) 'AG 27/05/2013 - RefRanges father is Result instead of QualifyingElement

                    ResultCollection.AppendChild(Result)
                    Test.AppendChild(ResultCollection)
                    Service.AppendChild(Test)

                    'SPECIMEN
                    Dim Specimen As XmlNode = XmlDoc.CreateElement("ci", "specimen", ClinicalInfoSchema)
                    Dim SpecimenId As XmlNode = XmlDoc.CreateElement("ci", "id", ClinicalInfoSchema)
                    SpecimenId.InnerText = pSpecimenID
                    Specimen.AppendChild(SpecimenId)
                    Dim SpecimenType As XmlNode = XmlDoc.CreateElement("ci", "type", ClinicalInfoSchema)
                    SpecimenType.InnerText = myMappedSampleType
                    Specimen.AppendChild(SpecimenType)
                    Service.AppendChild(Specimen)

                    If pRole = TestSampleClass.QC AndAlso Not pControlsDS Is Nothing Then
                        'CONTROL
                        Dim ControlNode As XmlNode = XmlDoc.CreateElement("ci", "control", ClinicalInfoSchema)
                        Dim ControlId As XmlNode = XmlDoc.CreateElement("ci", "id", ClinicalInfoSchema)
                        ControlId.InnerText = myResultRow.ControlName.ToString()
                        Dim ControlType As XmlNode = XmlDoc.CreateElement("ci", "type", ClinicalInfoSchema)
                        ControlType.InnerText = "internal"  ' Fixed
                        Dim ControlLotNumber As XmlNode = XmlDoc.CreateElement("ci", "lotNumber", ClinicalInfoSchema)
                        ControlLotNumber.InnerText = myResultRow.ControlLotNumber.ToString
                        Dim ControlExpirationDate As Date

                        ' Get Expiration Date
                        Dim lnqControls As New List(Of ControlsDS.tparControlsRow)
                        lnqControls = (From a As ControlsDS.tparControlsRow In pControlsDS.tparControls _
                                          Where a.ControlName = myResultRow.ControlName Select a).ToList

                        If lnqControls.Count > 0 Then
                            ControlExpirationDate = lnqControls(0).ExpirationDate
                        End If
                        Dim ControlExpDate As XmlNode = XmlDoc.CreateElement("ci", "expirationDate", ClinicalInfoSchema)
                        ControlExpDate.InnerText = ControlExpirationDate.ToXSDString()

                        ControlNode.AppendChild(ControlId)
                        ControlNode.AppendChild(ControlType)
                        ControlNode.AppendChild(ControlLotNumber)
                        ControlNode.AppendChild(ControlExpDate)
                        Service.AppendChild(ControlNode)
                        lnqControls = Nothing

                    End If

                    'OPERATOR
                    Dim OperatorNode As XmlNode = XmlDoc.CreateElement("ci", "operator", ClinicalInfoSchema)
                    Dim OperatorId As XmlNode = XmlDoc.CreateElement("ci", "id", ClinicalInfoSchema)
                    Dim myGlobalbase As New GlobalBase
                    OperatorId.InnerText = myGlobalbase.GetSessionInfo.UserName
                    OperatorNode.AppendChild(OperatorId)
                    Service.AppendChild(OperatorNode)

                    'PATIENT
                    Dim Patient As XmlNode = XmlDoc.CreateElement("ci", "patient", ClinicalInfoSchema)
                    Dim PatientID As XmlAttribute = XmlDoc.CreateAttribute("id")
                    PatientID.Value = pPatientID
                    Patient.Attributes.Append(PatientID)
                    Service.AppendChild(Patient)

                    'ORDER
                    Dim Order As XmlNode = XmlDoc.CreateElement("ci", "order", ClinicalInfoSchema)
                    Dim OrderID As XmlAttribute = XmlDoc.CreateAttribute("id")
                    OrderID.Value = pOrderID
                    Order.Attributes.Append(OrderID)
                    Service.AppendChild(Order)

                    resultData.SetDatos = Service

                    'myMapConfigRows = Nothing
                    'myMapUnitsRows = Nothing
                    'myMapTestIdRows = Nothing

                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.CreateServiceNode", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets from tfmwAlarms table the corresponding LIS identifier ffor each Remark and populate as many Id nodes as existing remarks
        ''' </summary>
        ''' <param name="XmlDoc"></param>
        ''' <param name="pAlarms"></param>
        ''' <param name="pRemarksMapping"></param>
        ''' <remarks>Created by SGM 09/07/2013</remarks>
        Private Sub CreateFlagsNode(ByRef XmlDoc As XmlDocument, ByRef ResultNode As XmlNode, ByVal pAlarms As List(Of ResultsDS.vwksResultsAlarmsRow), ByVal pRemarksMapping As LISMappingsDS)

            Try

                If Not pAlarms Is Nothing AndAlso pAlarms.Count > 0 Then

                    For Each rowAlarm As ResultsDS.vwksResultsAlarmsRow In pAlarms

                        Dim linqRemarks As List(Of LISMappingsDS.vcfgLISMappingRow)
                        linqRemarks = (From R As LISMappingsDS.vcfgLISMappingRow In pRemarksMapping.vcfgLISMapping _
                                       Where R.ValueType = "REMARK" And R.ValueId = rowAlarm.AlarmID Select R).ToList

                        If linqRemarks IsNot Nothing AndAlso linqRemarks.Count > 0 Then
                            Dim ResultFlag As XmlNode = XmlDoc.CreateElement("ci", "flag", ClinicalInfoSchema)
                            Dim ResultFlagId As XmlNode = XmlDoc.CreateElement("ci", "id", ClinicalInfoSchema)
                            ResultFlagId.InnerText = linqRemarks(0).LISValue
                            ResultFlag.AppendChild(ResultFlagId)
                            ResultNode.AppendChild(ResultFlag)
                        End If

                        linqRemarks = Nothing

                    Next

                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.CreateFlagsNode", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Creates a list of PatientNodes
        ''' </summary>
        ''' <param name="XmlDoc"></param>
        ''' <param name="pOrderTestsLISInfoDS"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 19/03/2013
        ''' Modified AG 26/03/2013 - the find all distinct patient code does not work properly
        '''                          the linq return the different rows not the different patients (DS can contain several rows contains the same ESPatientID but different ESOrderID)
        ''' </remarks>
        Private Function CreatePatientNodes(ByRef XmlDoc As XmlDocument, _
                                         ByVal pOrderTestsLISInfoDS As OrderTestsLISInfoDS) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                Dim myPatientNodes As New List(Of XmlNode)

                'find all distinct patients
                Dim qPatientsList As List(Of OrderTestsLISInfoDS.twksOrderTestsLISInfoRow)

                'AG 26/03/2013 - this linq returns different rows not different patients!!!
                'qPatientsList = (From o In pOrderTestsLISInfoDS.twksOrderTestsLISInfo Select o Distinct Order By o.ESPatientID).ToList()
                'For Each p As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In qPatientsList
                '    resultData = MyClass.CreatePatientNode(XmlDoc, p.ESPatientID, p.LISPatientID)
                '    If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                '        Dim myPatientNode As XmlNode = TryCast(resultData.SetDatos, XmlNode)

                '        'find all contained orders for the patient
                '        Dim qOrdersList As List(Of String)
                '        qOrdersList = (From o In pOrderTestsLISInfoDS.twksOrderTestsLISInfo Where o.ESPatientID = p.ESPatientID Select o.ESOrderID).ToList()
                '        For Each o As String In qOrdersList
                '            resultData = MyClass.CreateOrderNode(XmlDoc, o)
                '            If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                '                Dim myOrderNode As XmlNode = TryCast(resultData.SetDatos, XmlNode)
                '                myPatientNode.AppendChild(myOrderNode)
                '            End If
                '        Next

                '        myPatientNodes.Add(myPatientNode)

                '        qOrdersList = Nothing

                '    End If
                'Next

                Dim distinctPatientList As List(Of String)
                distinctPatientList = (From o As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In pOrderTestsLISInfoDS.twksOrderTestsLISInfo Select o.ESPatientID Distinct).ToList
                For Each item As String In distinctPatientList
                    qPatientsList = (From o In pOrderTestsLISInfoDS.twksOrderTestsLISInfo Where o.ESPatientID = item Select o Distinct Order By o.ESPatientID).ToList()
                    Dim counter As Integer = 0
                    Dim myPatientNode As XmlNode = Nothing
                    For Each p As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In qPatientsList
                        If counter = 0 Then
                            counter += 1
                            resultData = MyClass.CreatePatientNode(XmlDoc, p.ESPatientID, p.LISPatientID)
                            If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                                myPatientNode = TryCast(resultData.SetDatos, XmlNode)
                            End If
                        End If

                        'add orders for the patient
                        resultData = MyClass.CreateOrderNode(XmlDoc, p.ESOrderID)
                        If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                            Dim myOrderNode As XmlNode = TryCast(resultData.SetDatos, XmlNode)
                            myPatientNode.AppendChild(myOrderNode)
                        End If
                        myPatientNodes.Add(myPatientNode)
                    Next
                Next
                'AG 26/03/2013

                qPatientsList = Nothing

                resultData.SetDatos = myPatientNodes

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.CreatePatientNodes", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Creates the patient node with the informed data
        ''' </summary>
        ''' <param name="XmlDoc">Xml document in which the node and subnodes will be added</param>
        ''' <param name="pESPatientID"></param>
        ''' <param name="pLISPatientID"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 08/02/2013</remarks>
        Private Function CreatePatientNode(ByRef XmlDoc As XmlDocument, _
                                         ByVal pESPatientID As String, _
                                         ByVal pLISPatientID As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try

                Dim Order As XmlNode = XmlDoc.CreateElement("ci", "patient", ClinicalInfoSchema)
                Dim OrderIDAttr As XmlAttribute = XmlDoc.CreateAttribute("id")
                OrderIDAttr.Value = pESPatientID   'PENDING DECIDE if use a guid number. For March integration: OrderID
                Order.Attributes.Append(OrderIDAttr)
                Dim OrderId As XmlNode = XmlDoc.CreateElement("ci", "id", ClinicalInfoSchema)
                OrderId.InnerText = pLISPatientID
                Order.AppendChild(OrderId)
                resultData.SetDatos = Order

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.CreatePatientNode", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Creates the order node with the informed data
        ''' </summary>
        ''' <param name="XmlDoc">Xml document in which the node and subnodes will be added</param>
        ''' <param name="pESOrderID"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 08/02/2013
        ''' Modified by SGM 19/03/2013 - remove pLISOrderID arg
        ''' </remarks>
        Private Function CreateOrderNode(ByRef XmlDoc As XmlDocument, _
                                         ByVal pESOrderID As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try

                Dim Order As XmlNode = XmlDoc.CreateElement("ci", "order", ClinicalInfoSchema)
                Dim OrderIDAttr As XmlAttribute = XmlDoc.CreateAttribute("id")
                OrderIDAttr.Value = pESOrderID   'PENDING DECIDE if use a guid number. For March integration: OrderID
                Order.Attributes.Append(OrderIDAttr)
                Dim OrderId As XmlNode = XmlDoc.CreateElement("ci", "id", ClinicalInfoSchema)
                'OrderId.InnerText = pLISOrderID
                Order.AppendChild(OrderId)
                resultData.SetDatos = Order

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.CreateOrderNode", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Creates a common Source node for all xml documents
        ''' </summary>
        ''' <param name="XmlDoc"></param>
        ''' <param name="pSerialNumber">
        ''' If empty current analyzer serial number, 
        ''' if not empty another analyzer serial number</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 26/02/2013
        ''' MODIFIED BY: TR -Add new optional parameter Serial Number to indicate the serial number if the result is 
        '''                  for current analyzer or a previous analizer.
        ''' </remarks>
        Private Function CreateSourceNode(ByRef XmlDoc As XmlDocument, Optional ByVal pSerialNumber As String = "") As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Try

                '"<ci:source xmlns:ci="http://www.nte.es/schema/clinical-information-v1.0">" & 
                '   "<ci:companyName>Biosystems</ci:companyName>" &
                '   "<ci:OSVersion>" & Application Version & "</ci:OSVersion>" &
                '   "<ci:model>" & Analyzer Model & "</ci:model>" &
                '   "<ci:serialNumber>" & Analyzer Serial Number & "</ci:serialNumber>" &		
                '"</ci:source>" &

                Dim Source As XmlNode = XmlDoc.CreateElement("ci", "source", ClinicalInfoSchema)
                Dim xmlnsAttr As XmlAttribute = XmlDoc.CreateAttribute("xmlns:ci")
                xmlnsAttr.Value = ClinicalInfoSchema
                Source.Attributes.Append(xmlnsAttr)
                Dim CompanyName As XmlNode = XmlDoc.CreateElement("ci", "companyName", ClinicalInfoSchema)
                CompanyName.InnerText = "Biosystems"
                Source.AppendChild(CompanyName)

                'SGM 26/02/2013 - Cancelled
                'Dim OSVersion As XmlNode = XmlDoc.CreateElement("ci", "OSVersion", ClinicalInfoSchema)
                'OSVersion.InnerText = MyClass.ApplicationVersion
                'Source.AppendChild(OSVersion)

                Dim Model As XmlNode = XmlDoc.CreateElement("ci", "model", ClinicalInfoSchema)
                Model.InnerText = MyClass.AnalyzerModel
                Source.AppendChild(Model)
                Dim SerialNumber As XmlNode = XmlDoc.CreateElement("ci", "serialNumber", ClinicalInfoSchema)

                'TR 11/07/2013
                If pSerialNumber = "" Then
                    SerialNumber.InnerText = MyClass.AnalyzerSerialNumber
                Else
                    SerialNumber.InnerText = pSerialNumber
                End If
                'TR 11/07/2013 -END.


                Source.AppendChild(SerialNumber)

                resultData.SetDatos = Source

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ESxmlTranslator.CreateSourceNode", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function



#End Region


    End Class



End Namespace

