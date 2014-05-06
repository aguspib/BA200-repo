Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

''' <summary>
''' Class for Setting useLegacyV2RuntimeActivationPolicy At Runtime
''' </summary>
''' <remarks>SGM 09/01/2013</remarks>
''' 
Public NotInheritable Class RuntimePolicyHelper

    ''' http://reedcopsey.com/2011/09/15/setting-uselegacyv2runtimeactivationpolicy-at-runtime/

    Private Sub New()
    End Sub
    Public Shared Property LegacyV2RuntimeEnabledSuccessfully() As Boolean
        Get
            Return m_LegacyV2RuntimeEnabledSuccessfully
        End Get
        Private Set(value As Boolean)
            m_LegacyV2RuntimeEnabledSuccessfully = value
        End Set
    End Property
    Private Shared m_LegacyV2RuntimeEnabledSuccessfully As Boolean

    Shared Sub New()
        Dim clrRuntimeInfo As ICLRRuntimeInfo = DirectCast(RuntimeEnvironment.GetRuntimeInterfaceAsObject(Guid.Empty, GetType(ICLRRuntimeInfo).GUID), ICLRRuntimeInfo)
        Try
            clrRuntimeInfo.BindAsLegacyV2Runtime()
            LegacyV2RuntimeEnabledSuccessfully = True
        Catch generatedExceptionName As COMException
            ' This occurs with an HRESULT meaning 
            ' "A different runtime was already bound to the legacy CLR version 2 activation policy."
            LegacyV2RuntimeEnabledSuccessfully = False
        End Try
    End Sub

    <ComImport> _
    <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    <Guid("BD39D1D2-BA2F-486A-89B0-B4B0CB466891")> _
    Private Interface ICLRRuntimeInfo
        Sub xGetVersionString()
        Sub xGetRuntimeDirectory()
        Sub xIsLoaded()
        Sub xIsLoadable()
        Sub xLoadErrorString()
        Sub xLoadLibrary()
        Sub xGetProcAddress()
        Sub xGetInterface()
        Sub xSetDefaultStartupFlags()
        Sub xGetDefaultStartupFlags()

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)> _
        Sub BindAsLegacyV2Runtime()
    End Interface
End Class
