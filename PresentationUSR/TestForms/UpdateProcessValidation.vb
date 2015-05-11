Option Strict On
Option Explicit On
Option Infer On

Imports System.IO
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Framework.App
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Public Class UpdateProcessValidation
    ''' <summary>
    ''' Execute UPDATE VERSION process for ISE TESTS
    ''' </summary>
    Private Sub bsUpdateISETestsButton_Click(sender As Object, e As EventArgs) Handles bsUpdateISETestsButton.Click
        Dim myGlobal As New GlobalDataTO
        Dim dbConnection As SqlClient.SqlConnection

        XMLViewer.Clear()

        myGlobal = DAOBase.GetOpenDBTransaction(Nothing)
        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
            dbConnection = DirectCast(myGlobal.SetDatos, SqlClient.SqlConnection)
            If (Not dbConnection Is Nothing) Then
                Dim myUpdateVersionChangesList As New UpdateVersionChangesDS
                'Dim myUpdateProcessDelegate As New UpdateVersion.UpdatePreloadedFactoryTestDelegate

                myGlobal = UpdaterController.Instance.SetFactoryISETestsProgramming(dbConnection, myUpdateVersionChangesList) 'BA-2471: IT 08/05/2015
                If (Not myGlobal.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    DAOBase.CommitTransaction(dbConnection)

                    'Write the XML File containing all changes made in CUSTOMER DB
                    Dim myDirName As String = "C:\Temp\"
                    Dim myFileName As String = Now.ToString("yyyyMMdd HHmm") & " ISE UPDATE VERSION.xml"

                    myUpdateVersionChangesList.WriteXml(myDirName & myFileName)

                    Dim myTextReader As New StreamReader(myDirName & myFileName)
                    XMLViewer.Text = myTextReader.ReadToEnd()
                    myTextReader.Close()

                    MsgBox("ISE TESTS UPDATED", vbOKOnly)
                Else
                    'When the Database Connection was opened locally, then the Rollback is executed
                    DAOBase.RollbackTransaction(dbConnection)

                    MsgBox("ERROR UPDATING ISE TESTS: " & myGlobal.ErrorMessage, vbOKOnly)
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Execute UPDATE VERSION process for STD TESTS
    ''' </summary>
    Private Sub bsUpdateSTDTestsButton_Click(sender As Object, e As EventArgs) Handles bsUpdateSTDTestsButton.Click
        Dim myGlobal As New GlobalDataTO
        Dim dbConnection As SqlClient.SqlConnection

        XMLViewer.Clear()

        myGlobal = DAOBase.GetOpenDBTransaction(Nothing)
        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
            dbConnection = DirectCast(myGlobal.SetDatos, SqlClient.SqlConnection)
            If (Not dbConnection Is Nothing) Then
                Dim myUpdateVersionChangesList As New UpdateVersionChangesDS
                'Dim myUpdateProcessDelegate As New UpdateVersion.UpdatePreloadedFactoryTestDelegate

                myGlobal = UpdaterController.Instance.SetFactorySTDTestsProgramming(dbConnection, myUpdateVersionChangesList) 'BA-2471: IT 08/05/2015
                If (Not myGlobal.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    DAOBase.CommitTransaction(dbConnection)

                    'Write the XML File containing all changes made in CUSTOMER DB
                    Dim myDirName As String = "C:\Temp\"
                    Dim myFileName As String = Now.ToString("yyyyMMdd HHmm") & "STD UPDATE VERSION.xml"

                    myUpdateVersionChangesList.WriteXml(myDirName & myFileName)

                    Dim myTextReader As New StreamReader(myDirName & myFileName)
                    XMLViewer.Text = myTextReader.ReadToEnd()
                    myTextReader.Close()

                    MsgBox("STD TESTS UPDATED", vbOKOnly)
                Else
                    'When the Database Connection was opened locally, then the Rollback is executed
                    DAOBase.RollbackTransaction(dbConnection)

                    MsgBox("ERROR UPDATING STD TESTS: " & myGlobal.ErrorMessage, vbOKOnly)
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Execute UPDATE VERSION process for OFFS TESTS
    ''' </summary>
    Private Sub bsUpdateOFFSTestsButton_Click(sender As Object, e As EventArgs) Handles bsUpdateOFFSTestsButton.Click
        Dim myGlobal As New GlobalDataTO
        Dim dbConnection As SqlClient.SqlConnection

        XMLViewer.Clear()

        myGlobal = DAOBase.GetOpenDBTransaction(Nothing)
        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
            dbConnection = DirectCast(myGlobal.SetDatos, SqlClient.SqlConnection)
            If (Not dbConnection Is Nothing) Then
                'Get Application Version from CUSTOMER DB
                Dim mySwVersion As String = String.Empty
                Dim myVersionsDelegate As New VersionsDelegate
                myGlobal = myVersionsDelegate.GetVersionsData(Nothing)

                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    Dim myVersionsDS As VersionsDS = DirectCast(myGlobal.SetDatos, VersionsDS)

                    If (myVersionsDS.tfmwVersions.Count > 0) Then
                        'Inform the USER and SERVICE SW Versions to return
                        mySwVersion = myVersionsDS.tfmwVersions(0).UserSoftware
                    End If
                End If

                If (Not myGlobal.HasError) Then
                    Dim myUpdateVersionChangesList As New UpdateVersionChangesDS
                    'Dim myUpdateProcessDelegate As New UpdateVersion.UpdatePreloadedFactoryTestDelegate

                    myGlobal = UpdaterController.Instance.SetFactoryOFFSTestsProgramming(dbConnection, mySwVersion, myUpdateVersionChangesList) 'BA-2471: IT 08/05/2015
                    If (Not myGlobal.HasError) Then
                        'When the Database Connection was opened locally, then the Commit is executed
                        DAOBase.CommitTransaction(dbConnection)

                        'Write the XML File containing all changes made in CUSTOMER DB
                        Dim myDirName As String = "C:\Temp\"
                        Dim myFileName As String = Now.ToString("yyyyMMdd HHmm") & "OFFS UPDATE VERSION.xml"

                        myUpdateVersionChangesList.WriteXml(myDirName & myFileName)

                        Dim myTextReader As New StreamReader(myDirName & myFileName)
                        XMLViewer.Text = myTextReader.ReadToEnd()
                        myTextReader.Close()

                        MsgBox("OFFS TESTS UPDATED", vbOKOnly)
                    Else
                        'When the Database Connection was opened locally, then the Rollback is executed
                        DAOBase.RollbackTransaction(dbConnection)

                        MsgBox("ERROR UPDATING OFFS TESTS: " & myGlobal.ErrorMessage, vbOKOnly)
                    End If
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Execute UPDATE VERSION process for CALC TESTS
    ''' </summary>
    Private Sub bsUpdateCALCTestsButton_Click(sender As Object, e As EventArgs) Handles bsUpdateCALCTestsButton.Click
        Dim myGlobal As New GlobalDataTO
        Dim dbConnection As SqlClient.SqlConnection

        XMLViewer.Clear()

        myGlobal = DAOBase.GetOpenDBTransaction(Nothing)
        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
            dbConnection = DirectCast(myGlobal.SetDatos, SqlClient.SqlConnection)
            If (Not dbConnection Is Nothing) Then
                Dim myUpdateVersionChangesList As New UpdateVersionChangesDS
                'Dim myUpdateProcessDelegate As New UpdateVersion.UpdatePreloadedFactoryTestDelegate

                myGlobal = UpdaterController.Instance.SetFactoryCALCTestsProgramming(dbConnection, myUpdateVersionChangesList) 'BA-2471: IT 08/05/2015
                If (Not myGlobal.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    DAOBase.CommitTransaction(dbConnection)

                    'Write the XML File containing all changes made in CUSTOMER DB
                    Dim myDirName As String = "C:\Temp\"
                    Dim myFileName As String = Now.ToString("yyyyMMdd HHmm") & "CALC UPDATE VERSION.xml"

                    myUpdateVersionChangesList.WriteXml(myDirName & myFileName)

                    Dim myTextReader As New StreamReader(myDirName & myFileName)
                    XMLViewer.Text = myTextReader.ReadToEnd()
                    myTextReader.Close()

                    MsgBox("CALC TESTS UPDATED", vbOKOnly)
                Else
                    'When the Database Connection was opened locally, then the Rollback is executed
                    DAOBase.RollbackTransaction(dbConnection)

                    MsgBox("ERROR UPDATING CALC TESTS: " & myGlobal.ErrorMessage, vbOKOnly)
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Close the screen
    ''' </summary>
    Private Sub bsExitButton_Click(sender As Object, e As EventArgs) Handles bsExitButton.Click
        Me.Close()
    End Sub
End Class
