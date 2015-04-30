Imports System.Data.SqlClient
Imports System.IO
Imports System.Reflection
Imports Biosystems.Ax00.Global

Public Class DelegatesToCoreBusinesGlue
    Public Shared Function CreateWS(ByVal ppDBConnection As SqlConnection, ByVal ppAnalyzerID As String, ByVal ppWorkSessionID As String, _
                                 ByVal ppWorkInRunningMode As Boolean, Optional ByVal ppOrderTestID As Integer = -1, _
                                 Optional ByVal ppPostDilutionType As String = "", Optional ByVal ppIsISEModuleReady As Boolean = False, _
                                 Optional ByVal ppISEElectrodesList As List(Of String) = Nothing, Optional ByVal ppPauseMode As Boolean = False, _
                                 Optional ByVal ppManualRerunFlag As Boolean = True) As GlobalDataTO



        Const TypeName As String = "Biosystems.Ax00.Core.Entities.WorkSession.WSExecutionCreator"
        Dim obj = DelegatesToCoreBusinesGlue.BsCoreAssembly.GetType(TypeName)
        <ThreadStatic> Static method As MethodInfo
        <ThreadStatic> Static instance As Object = Nothing

        Try
            If method Is Nothing Then
                instance = obj.GetProperty("Instance").GetGetMethod.Invoke(Nothing, {})
                method = obj.GetMethod("CreateWS")
            End If
            Dim result = method.Invoke(
                            instance,
                                {ppDBConnection, ppAnalyzerID, ppWorkSessionID, _
                                 ppWorkInRunningMode, ppOrderTestID, _
                                 ppPostDilutionType, ppIsISEModuleReady, _
                                 ppISEElectrodesList, ppPauseMode, _
                                 ppManualRerunFlag})

            Return DirectCast(result, GlobalDataTO)

        Catch ex As Exception
            Return New GlobalDataTO With {.HasError = True, .SetDatos = Nothing, .ErrorMessage = "Reflection error. Can't createWS"}
        End Try




    End Function

    Public Shared Function BsCoreAssembly() As Reflection.Assembly
        Static block As New Object
        SyncLock block
            Static coreAssembly As Reflection.Assembly
            If coreAssembly Is Nothing Then
                coreAssembly = Assembly.LoadFile(AssemblyDirectory() & "\Biosystems.Ax00.Core.dll")
            End If
            Return coreAssembly
        End SyncLock
    End Function


    Private Shared ReadOnly Property AssemblyDirectory() As String
        Get
            <ThreadStatic> Static location As String
            If location <> String.Empty Then Return location
            Dim codeBase As String = Assembly.GetExecutingAssembly().CodeBase
            Dim uri1 As New UriBuilder(codeBase)
            Dim path2 As String = Uri.UnescapeDataString(uri1.Path)
            location = Path.GetDirectoryName(path2)
            Return location
        End Get
    End Property

End Class
