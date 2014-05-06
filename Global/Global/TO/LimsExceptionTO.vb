Option Explicit On
Option Strict On

Namespace Biosystems.Ax00.Global.TO

    Public Class LimsExceptionTO
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

        Public Sub New()

        End Sub

#Region "Attributes"
        Private FacilityAttr As String
        Private CategoryAttr As String

        Private SeverityAttr As String
        Private ComputerAttr As String
        Private DataAttr As String

        Private hrCodeAttr As String
        Private hrDescriptionAttr As String

        Private FileNameAttr As String
        Private FileLineAttr As String

        Private ProcessNameAttr As String
        Private ProcessIDAttr As String

        Private ThreadIDAttr As String

        Private ErrorDatetimeAttr As String

        Private ApplicationIDAttr As String
        Private ModulePathAttr As String
        Private ComponentAttr As String
        Private GUIDAttr As String

#End Region

#Region "Properties"


        Public Property Facility() As String
            Get
                Return FacilityAttr
            End Get
            Set(ByVal value As String)
                FacilityAttr = value
            End Set
        End Property
        Public Property Category() As String
            Get
                Return CategoryAttr
            End Get
            Set(ByVal value As String)
                CategoryAttr = value
            End Set
        End Property
        Public Property Severity() As String
            Get
                Return SeverityAttr
            End Get
            Set(ByVal value As String)
                SeverityAttr = value
            End Set
        End Property
        Public Property Computer() As String
            Get
                Return ComputerAttr
            End Get
            Set(ByVal value As String)
                ComputerAttr = value
            End Set
        End Property
        Public Property Data() As String
            Get
                Return DataAttr
            End Get
            Set(ByVal value As String)
                DataAttr = value
            End Set
        End Property
        Public Property hrCode() As String
            Get
                Return hrCodeAttr
            End Get
            Set(ByVal value As String)
                hrCodeAttr = value
            End Set
        End Property
        Public Property hrDescription() As String
            Get
                Return hrDescriptionAttr
            End Get
            Set(ByVal value As String)
                hrDescriptionAttr = value
            End Set
        End Property
        Public Property FileName() As String
            Get
                Return FileNameAttr
            End Get
            Set(ByVal value As String)
                FileNameAttr = value
            End Set
        End Property
        Public Property FileLine() As String
            Get
                Return FileLineAttr
            End Get
            Set(ByVal value As String)
                FileLineAttr = value
            End Set
        End Property
        Public Property ProcessName() As String
            Get
                Return ProcessNameAttr
            End Get
            Set(ByVal value As String)
                ProcessNameAttr = value
            End Set
        End Property
        Public Property ProcessID() As String
            Get
                Return ProcessIDAttr
            End Get
            Set(ByVal value As String)
                ProcessIDAttr = value
            End Set
        End Property
        Public Property ThreadID() As String
            Get
                Return ThreadIDAttr
            End Get
            Set(ByVal value As String)
                ThreadIDAttr = value
            End Set
        End Property
        Public Property ErrorDateTime() As String
            Get
                Return ErrorDatetimeAttr
            End Get
            Set(ByVal value As String)
                ErrorDatetimeAttr = value
            End Set
        End Property
        Public Property ApplicationID() As String
            Get
                Return ApplicationIDAttr
            End Get
            Set(ByVal value As String)
                ApplicationIDAttr = value
            End Set
        End Property
        Public Property ModulePath() As String
            Get
                Return ModulePathAttr
            End Get
            Set(ByVal value As String)
                ModulePathAttr = value
            End Set
        End Property
        Public Property Component() As String
            Get
                Return ComponentAttr
            End Get
            Set(ByVal value As String)
                ComponentAttr = value
            End Set
        End Property
        Public Property GUID() As String
            Get
                Return GUIDAttr
            End Get
            Set(ByVal value As String)
                GUIDAttr = value
            End Set
        End Property
#End Region


    End Class

End Namespace