Imports Biosystems.Ax00.Core.Entities
Imports NUnit.Framework
Imports Biosystems.Ax00.DAL


''' <summary>
''' Class to test the integrity of the data on the .BAK database copy
''' </summary>
''' <remarks>
''' Created on 19/06/2015 by AJG 
''' </remarks>
<TestFixture()>
Public Class DBSetupIntegrityTest

    Property Param_SAMPLE_STEPS_UL As Single = 7.95775
    Property Param_ISE_UTIL_WASHSOL_POSITION As Integer = 44

    <Test()>
    Sub IntegrityCheck()
        'TO_DO: Terminar de implementar los métodos 
        Dim ServerName = ""
        Dim DataBaseName = ""
        Dim DBLogin = ""
        Dim DBPassword = ""
        Dim BackUpFileName = ""
        DBManager.RestoreDBFileList(ServerName, DataBaseName, DBLogin, DBPassword, BackUpFileName)
    End Sub


    Function CheckIfDBExist() As Boolean
        'TO_DO: Hay que implementar los métodos
        Return True
    End Function
End Class
