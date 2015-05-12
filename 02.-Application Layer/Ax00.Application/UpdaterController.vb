
Imports Biosystems.Ax00.BL.UpdateVersion
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.App

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Modified by:  IT 19/12/2014 - BA-2143
    ''' </remarks>
    Public NotInheritable Class UpdaterController
        'Implements IAnalyzerController

        Private Shared ReadOnly _instance As New Lazy(Of UpdaterController)(Function() New UpdaterController(), System.Threading.LazyThreadSafetyMode.ExecutionAndPublication)


#Region "Properties"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property Instance() As UpdaterController
            Get
                Return _instance.Value
            End Get
        End Property


        Public Property DataBaseUpdateManager As DataBaseUpdateManagerDelegate

#End Region

#Region "Public Methods"

        Public Function InstallUpdateProcess(ByVal serverName As String, ByVal dataBaseName As String, ByVal dBLogin As String, _
                                             ByVal dBPassword As String, Optional loadingRSat As Boolean = False) As GlobalDataTO
            Try
                If (DataBaseUpdateManager Is Nothing) Then
                    DataBaseUpdateManager = New DataBaseUpdateManagerDelegate()
                End If

                Return DataBaseUpdateManager.InstallUpdateProcess(serverName, dataBaseName, dBLogin, dBPassword, loadingRSat)
            Catch ex As Exception
                Throw
            End Try

        End Function

#Region "Public Methods for Testing"

        Public Function SetFactorySTDTestsProgramming(ByVal dBConnection As SqlClient.SqlConnection, ByRef updateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO

            Try
                Dim myUpdateProcessDelegate As New UpdatePreloadedFactoryTestDelegate
                Return myUpdateProcessDelegate.SetFactorySTDTestsProgramming(dBConnection, updateVersionChangesList)
            Catch ex As Exception
                Throw
            End Try

        End Function

        Public Function SetFactoryISETestsProgramming(ByVal dBConnection As SqlClient.SqlConnection, ByRef updateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO

            Try
                Dim myUpdateProcessDelegate As New UpdatePreloadedFactoryTestDelegate
                Return myUpdateProcessDelegate.SetFactoryISETestsProgramming(dBConnection, updateVersionChangesList)
            Catch ex As Exception
                Throw
            End Try

        End Function
        Public Function SetFactoryCALCTestsProgramming(ByVal dBConnection As SqlClient.SqlConnection, ByRef updateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO

            Try
                Dim myUpdateProcessDelegate As New UpdatePreloadedFactoryTestDelegate
                Return myUpdateProcessDelegate.SetFactoryCALCTestsProgramming(dBConnection, updateVersionChangesList)
            Catch ex As Exception
                Throw
            End Try

        End Function
        Public Function SetFactoryOFFSTestsProgramming(ByVal dBConnection As SqlClient.SqlConnection, ByVal customerSwVersion As String, _
                                                       ByRef updateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO

            Try
                Dim myUpdateProcessDelegate As New UpdatePreloadedFactoryTestDelegate
                Return myUpdateProcessDelegate.SetFactoryOFFSTestsProgramming(dBConnection, customerSwVersion, updateVersionChangesList)
            Catch ex As Exception
                Throw
            End Try

        End Function

        Public Function SetFactoryTestProgrammingNEW(ByVal dBConnection As SqlClient.SqlConnection, ByVal customerSwVersion As String, _
                                             ByVal factorySwVersion As String) As GlobalDataTO

            Try
                Dim myUpdateProcessDelegate As New UpdatePreloadedFactoryTestDelegate
                Return myUpdateProcessDelegate.SetFactoryTestProgrammingNEW(dBConnection, customerSwVersion, factorySwVersion)
            Catch ex As Exception
                Throw
            End Try

        End Function

#End Region

#End Region

#Region "Private Methods"


#End Region

    End Class

End Namespace

