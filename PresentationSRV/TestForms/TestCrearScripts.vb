
Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.CommunicationsSwFw
Imports System.IO

Public Class TestCrearScripts

    'SGM
    Private ReadOnly Property IsLoading() As Boolean
        Get
            Return IsLoadingAttr
        End Get
    End Property

    Private Property IsEditing() As Boolean
        Get
            Return IsEditingAttr
        End Get
        Set(ByVal value As Boolean)
            If Not IsEditingAttr Then
                EditingScriptsData = OriginalScriptsData.Clone
                IsAddingAttr = False
            End If
            IsEditingAttr = value
        End Set
    End Property

    Private ReadOnly Property IsAdding() As Boolean
        Get
            Return IsAddingAttr
        End Get
    End Property

    Private ReadOnly Property ChangesMade() As Boolean
        Get
            Return ChangesMadeAttr
        End Get
    End Property

    Private ReadOnly Property Encrypting() As Boolean
        Get
            Return EncryptingAttr
        End Get
    End Property

    Private OriginalScriptsData As New FwScriptsDataTO
    Private EditingScriptsData As New FwScriptsDataTO
    Private IsLoadingAttr As Boolean = False
    Private IsEditingAttr As Boolean = False
    Private IsAddingAttr As Boolean = False
    Private ChangesMadeAttr As Boolean = False
    Private ValidationError As Boolean = False
    Private EncryptingAttr As Boolean = False

    'SGM
    Private CurrentAnalyzer As String = ""
    'SGM

    Private Sub LoadDataFromXMLFile()
        Try
            Dim myXMLpath As String = My.Application.Info.DirectoryPath & "/scripts.xml"
            Dim myOpenDlg As New OpenFileDialog
            Dim res As DialogResult

            With myOpenDlg
                '.InitialDirectory = My.Application.Info.DirectoryPath
                .Filter = "AX00 Script Files|*.xml"
                res = .ShowDialog()
                myXMLpath = .FileName
            End With

            If res = Windows.Forms.DialogResult.OK Then
                IsLoadingAttr = True
                Dim myGlobal As New GlobalDataTO
                myGlobal = Importar(OriginalScriptsData.GetType, myXMLpath)
                If Not myGlobal.HasError And Not myGlobal Is Nothing Then
                    OriginalScriptsData = CType(myGlobal.SetDatos, FwScriptsDataTO)

                    CargarCombo(OriginalScriptsData) 'SGM

                    Dim myFileName As String = myOpenDlg.FileName.Substring(myOpenDlg.FileName.LastIndexOf("\") + 1)
                    Text = String.Format("Crear Scripts - {0} - v{1}", myFileName, OriginalScriptsData.Version)

                Else
                    Throw New Exception(myGlobal.ErrorCode & vbCrLf & myGlobal.ErrorMessage)
                End If
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            IsLoadingAttr = False
        End Try
    End Sub


    'SGM
    Private Sub AddDataToXMLFile()
        Try
            Dim myGlobal As New GlobalDataTO

            If Not ValidationError Then

                myGlobal = CompilarGrid()
                If Not myGlobal.HasError And Not myGlobal Is Nothing Then
                    EditingScriptsData = CType(myGlobal.SetDatos, FwScriptsDataTO)

                    If BsVersionTextBox.Text = "" Then
                        BsScreenErrorProvider.SetError(BsVersionTextBox, "Falta la versión")
                        Exit Sub
                    End If

                    'version
                    Dim myUtil As New Utilities
                    myGlobal = myUtil.GetSoftwareVersion
                    If Not myGlobal.HasError And Not myGlobal Is Nothing Then
                        Dim myVersion As String = CStr(myGlobal.SetDatos)
                        Dim myRes As DialogResult = MessageBox.Show("La versión actual del software es: " & myVersion & vbCrLf & "¿Asignar esta versión?", Me.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
                        If myRes = Windows.Forms.DialogResult.Yes Then
                            Me.BsVersionTextBox.Text = myVersion
                        ElseIf myRes = Windows.Forms.DialogResult.Cancel Then
                            Exit Sub
                        End If
                    Else
                        Me.BsVersionTextBox.Text = ""
                    End If


                    Dim myXMLpath As String
                    Dim mySaveDlg As New SaveFileDialog
                    Dim res As DialogResult
                    With mySaveDlg
                        .Filter = "AX00 Script Files|*.xml"
                        res = .ShowDialog()
                        myXMLpath = .FileName
                    End With

                    If res = Windows.Forms.DialogResult.OK Then
                        If File.Exists(myXMLpath) Then
                            File.Delete(myXMLpath)
                        End If

                        myGlobal = Exportar(EditingScriptsData, myXMLpath)

                        If Not myGlobal.HasError And Not myGlobal Is Nothing Then
                            OriginalScriptsData = EditingScriptsData
                            IsEditing = False 'SGM
                            BsVersionTextBox.Focus() 'SGM

                            Dim myFileName As String = myXMLpath.Substring(myXMLpath.LastIndexOf("\") + 1)
                            Text = "Crear Scripts - " & myFileName & " - v" & OriginalScriptsData.Version
                        End If
                    End If


                    If myGlobal.HasError Then
                        Throw New Exception(myGlobal.ErrorCode & vbCrLf & myGlobal.ErrorMessage)
                    End If

                End If

                

            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub ValidateGrid()
        Try
            ValidationError = False

            Dim hasError As Boolean
            For Each dr As DataGridViewRow In BSScriptsDGV.Rows
                ValidateRow(dr)
                hasError = hasError And ValidationError
            Next

            If Not hasError Then

                Dim result As Boolean = False
                For Each dr As DataGridViewRow In BSScriptsDGV.Rows
                    If Not dr.IsNewRow Then
                        Dim myScriptID As Integer = CInt(dr.Cells("colScript").Value)
                        Dim MyActionID As String = CStr(dr.Cells("colAction").Value)
                        Dim myScreenID As String = CStr(dr.Cells("colScreen").Value)
                        Dim myAnalyzerID As String = CStr(dr.Cells("colAnalyzer").Value)

                        'verificar que siempre se cumple la pareja Accion/Script
                        For Each ddrr As DataGridViewRow In BSScriptsDGV.Rows
                            ddrr.Cells("colAction").ErrorText = ""
                            ddrr.Cells("colscript").ErrorText = ""
                            If Not ddrr.IsNewRow Then
                                Dim mySID As String = CStr(ddrr.Cells("colScript").Value)
                                If myScriptID = CInt(mySID) Then
                                    Dim mySAID As String = CStr(ddrr.Cells("colAction").Value)
                                    If mySAID <> MyActionID Then
                                        ddrr.Cells("colAction").ErrorText = "El Script " & mySID & " no tiene Acciones idénticas"
                                        ddrr.Cells("colScript").ErrorText = String.Format("El Script {0} no tiene Acciones idénticas", mySID)
                                        ValidationError = True
                                    End If
                                End If
                            End If
                        Next

                        BsScreenErrorProvider.Clear()
                        result = True
                    End If
                Next


            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub
    

    Private Sub ValidateRow(ByVal pRow As DataGridViewRow)
        Try
            ValidationError = False

            'pRow.Cells("colScreen").ErrorText = ""
            If pRow.Cells("colScreen").Value Is Nothing Then
                pRow.Cells("colScreen").ErrorText = "Falta Screen"
                ValidationError = True
            Else
                pRow.Cells("colScreen").ErrorText = ""
            End If

            'pRow.Cells("colAction").ErrorText = ""
            If pRow.Cells("colAction").Value Is Nothing Then
                pRow.Cells("colAction").ErrorText = "Falta indicar Acción"
                ValidationError = True
            Else
                pRow.Cells("colAction").ErrorText = ""
            End If

            'pRow.Cells("colScript").ErrorText = ""
            If pRow.Cells("colScript").Value Is Nothing Then
                pRow.Cells("colScript").ErrorText = "Falta indicar Script ID"
                ValidationError = True
            Else

                If Not IsNumeric(CStr(pRow.Cells("colScript").Value)) Then
                    pRow.Cells("colScript").ErrorText = "Script ID debe ser numérico"
                    ValidationError = True
                Else
                    pRow.Cells("colScript").ErrorText = ""
                End If

            End If

            For Each dc As DataGridViewCell In pRow.Cells
                If dc.ColumnIndex < 4 Then
                    If dc.Value IsNot Nothing Then
                        Dim myValue As String = CStr(dc.Value)

                        For Each C As Char In myValue
                            Const Plantilla As String = "ZXCVBNMASDFGHJKLQWERTYUIOPÑzxcvbnmasdfghjklñqwertyuiop1234567890"
                            If Not Plantilla.Contains(C) Then
                                dc.ErrorText = "Sólo se permiten letras y números"
                                Exit For
                            End If
                        Next

                    Else
                        dc.ErrorText = "faltan datos"
                    End If

                End If
            Next

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

   
    Private Function CompilarGrid() As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try

            Dim tempScriptsData As New FwScriptsDataTO


            Dim myAllAnalyzerIDs As New List(Of String)
            Dim myAllScreenIDs As New List(Of String)
            Dim myAllScriptIDs As New List(Of Integer)


            For Each dr As DataGridViewRow In BSScriptsDGV.Rows
                If Not dr.IsNewRow Then

                    Dim myScript As FwScriptTO = Nothing
                    Dim isNewScript As Boolean = False
                    Dim myScriptID As Integer = CInt(dr.Cells("colScript").Value)

                    If Not myAllScriptIDs.Contains(myScriptID) Then
                        myAllScriptIDs.Add(myScriptID)
                        Dim myNewScript As New FwScriptTO
                        With myNewScript
                            .FwScriptID = myScriptID
                            .ActionID = CStr(dr.Cells("colAction").Value)
                            .Author = "SW"
                            .Created = DateTime.Now
                            .Modified = DateTime.Now
                            .Description = CStr(dr.Cells("colDescription").Value)
                        End With

                        'SGM 26/01/11
                        'Maintain instructions
                        For Each FS As FwScriptTO In EditingScriptsData.FwScripts
                            If FS.FwScriptID = myNewScript.FwScriptID Then
                                For Each I As InstructionTO In FS.Instructions
                                    Dim myInstruction As New InstructionTO
                                    myInstruction = I.Clone()
                                    myNewScript.Instructions.Add(myInstruction)
                                Next
                                Exit For
                            End If
                        Next

                        myScript = myNewScript
                        isNewScript = True

                    Else
                        For Each ST As FwScriptTO In tempScriptsData.FwScripts
                            If ST.FwScriptID = myScriptID Then
                                With ST
                                    .FwScriptID = myScriptID
                                    .ActionID = CStr(dr.Cells("colAction").Value)
                                    .Description = CStr(dr.Cells("colDescription").Value)
                                End With
                                myScript = ST
                                Exit For
                            End If
                        Next
                    End If

                    If isNewScript Then
                        tempScriptsData.FwScripts.Add(myScript)
                    End If

                    Dim myScreen As ScreenTO = Nothing
                    Dim isNewScreen As Boolean = False
                    Dim myScreenID As String = CStr(dr.Cells("colScreen").Value)
                    If Not myAllScreenIDs.Contains(myScreenID) Then
                        myAllScreenIDs.Add(myScreenID)
                        Dim myNewScreen As New ScreenTO() With {.ScreenID = myScreenID}
                        myScreen = myNewScreen
                        isNewScreen = True
                        myScreen.FwScriptIDs = New List(Of Integer)
                        myScreen.FwScriptIDs.Add(myScriptID)
                    Else
                        For Each SN As ScreenTO In tempScriptsData.Screens
                            If SN.ScreenID.ToUpper.Trim = myScreenID.ToUpper.Trim Then
                                If Not SN.FwScriptIDs.Contains(myScriptID) Then
                                    SN.FwScriptIDs.Add(myScriptID)
                                End If
                                With SN
                                    .ScreenID = myScreenID
                                End With
                                Exit For
                            End If
                        Next
                    End If

                    If isNewScreen Then
                        tempScriptsData.Screens.Add(myScreen)
                    End If


                    Dim myAnalyzer As AnalyzerFwScriptsTO = Nothing
                    Dim isNewAnalyzer As Boolean = False
                    Dim myAnalyzerID As String = CStr(dr.Cells("colAnalyzer").Value)
                    If Not myAllAnalyzerIDs.Contains(myAnalyzerID) Then
                        myAllAnalyzerIDs.Add(myAnalyzerID)
                        Dim myNewAnalyzer As New AnalyzerFwScriptsTO
                        With myNewAnalyzer
                            .AnalyzerID = myAnalyzerID
                        End With
                        myAnalyzer = myNewAnalyzer
                        isNewAnalyzer = True
                        myAnalyzer.ScreenIDs = New List(Of String)
                        myAnalyzer.ScreenIDs.Add(myScreenID)
                    Else
                        For Each A As AnalyzerFwScriptsTO In tempScriptsData.Analyzers
                            If A.AnalyzerID.ToUpper.Trim = myAnalyzerID.ToUpper.Trim Then
                                If Not A.ScreenIDs.Contains(myScreenID) Then
                                    A.ScreenIDs.Add(myScreenID)
                                End If
                                With A
                                    .AnalyzerID = myAnalyzerID
                                End With
                                Exit For
                            End If
                        Next
                    End If

                    If isNewAnalyzer Then
                        tempScriptsData.Analyzers.Add(myAnalyzer)
                    End If

                End If
            Next


            tempScriptsData.Version = BsVersionTextBox.Text

            myGlobal.SetDatos = tempScriptsData


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = "SYSTEM_ERROR"
            myGlobal.ErrorMessage = ex.Message
        End Try

        Return myGlobal
    End Function

    'SGM
    Private Sub CargarCombo(ByVal pScriptsData As FwScriptsDataTO)
        Try
            IsEditing = False
            BSScriptsDGV.Rows.Clear()
            BsAnalyzerComboBox.Items.Clear()

            Dim myAnalyzers As New List(Of AnalyzerFwScriptsTO)
            myAnalyzers = pScriptsData.Analyzers

            For Each A As AnalyzerFwScriptsTO In myAnalyzers
                BsAnalyzerComboBox.Items.Add(A.AnalyzerID)
                BsAnalyzerComboBox.SelectedText = A.AnalyzerID
            Next

            BsAnalyzerComboBox.Items.Add("TODOS")


            BsVersionTextBox.Text = pScriptsData.Version

            If BsAnalyzerComboBox.Items.Count > 0 Then
                CurrentAnalyzer = CStr(BsAnalyzerComboBox.Items(0))
                CargarGrid(pScriptsData, CurrentAnalyzer)
                BsAnalyzerComboBox.SelectedItem = "TODOS"
            End If


        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    'SGM
    Private Sub CargarGrid(ByVal pScriptsData As FwScriptsDataTO, ByVal pAnalyzerID As String)
        Try


            BSScriptsDGV.Rows.Clear()


            Dim myAnalyzers As New List(Of AnalyzerFwScriptsTO)
            myAnalyzers = pScriptsData.Analyzers

            Dim myScreens As New List(Of ScreenTO)
            myScreens = pScriptsData.Screens

            Dim myScripts As New List(Of FwScriptTO)
            myScripts = pScriptsData.FwScripts

            For Each A As AnalyzerFwScriptsTO In myAnalyzers
                'If A.AnalyzerID.ToUpper.Trim = pAnalyzerID.ToUpper.Trim Then
                Dim myScreenIDs As New List(Of String)
                myScreenIDs = A.ScreenIDs
                For Each SN As ScreenTO In myScreens
                    For Each SNID As String In myScreenIDs
                        If SN.ScreenID.ToUpper.Trim = SNID.ToUpper.Trim Then
                            Dim myScriptIDs As New List(Of Integer)
                            myScriptIDs = SN.FwScriptIDs
                            For Each ST As FwScriptTO In myScripts
                                For Each STID As Integer In myScriptIDs
                                    If ST.FwScriptID = STID And SN.FwScriptIDs.Contains(STID) Then
                                        Dim rowIndex As Integer = BSScriptsDGV.Rows.Add()
                                        BSScriptsDGV.Rows(rowIndex).Cells("colAnalyzer").Value = A.AnalyzerID
                                        BSScriptsDGV.Rows(rowIndex).Cells("colScreen").Value = SN.ScreenID
                                        BSScriptsDGV.Rows(rowIndex).Cells("colAction").Value = ST.ActionID
                                        BSScriptsDGV.Rows(rowIndex).Cells("colScript").Value = ST.FwScriptID.ToString
                                        BSScriptsDGV.Rows(rowIndex).Cells("colDescription").Value = ST.Description
                                        If pAnalyzerID.ToUpper.Trim <> "TODOS" And A.AnalyzerID.ToUpper.Trim <> pAnalyzerID.ToUpper.Trim Then
                                            BSScriptsDGV.Rows(rowIndex).Visible = False
                                        End If
                                    End If
                                Next
                            Next
                        End If
                    Next
                Next
                'End If
            Next

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    'SGM
    Private Function GetScriptID(ByVal pActionID As String) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            'if the Action ID is already defined
            For Each S As FwScriptTO In EditingScriptsData.FwScripts
                If S.ActionID.ToUpper.Trim = pActionID.ToUpper.Trim Then
                    myGlobal.SetDatos = S.FwScriptID
                    Exit For
                End If
            Next


            'else, assign a new one
            If myGlobal.SetDatos Is Nothing And EditingScriptsData.FwScripts.Count > 0 Then

                Dim maxID As Integer = 0
                For Each S As FwScriptTO In EditingScriptsData.FwScripts
                    If S.FwScriptID > maxID Then
                        maxID = S.FwScriptID
                    End If
                Next
                If maxID > 0 Then
                    myGlobal.SetDatos = maxID + 1
                End If
            End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = "SYSTEM_ERROR"
            myGlobal.ErrorMessage = ex.Message
        End Try
        Return myGlobal
    End Function

    'SGM
    Private Function ExistsScreen(ByVal pScreenID As String) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            For Each S As ScreenTO In EditingScriptsData.Screens
                If S.ScreenID.ToUpper.Trim = pScreenID.ToUpper.Trim Then
                    myGlobal.SetDatos = True
                    Exit Try
                End If
            Next

            myGlobal.SetDatos = False


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = "SYSTEM_ERROR"
            myGlobal.ErrorMessage = ex.Message
        End Try
        Return myGlobal
    End Function

    Private Function ExistsScript(ByVal pScriptID As Integer) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            For Each S As FwScriptTO In EditingScriptsData.FwScripts
                If S.FwScriptID = pScriptID Then
                    myGlobal.SetDatos = True
                    Exit Try
                End If
            Next

            myGlobal.SetDatos = False


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = "SYSTEM_ERROR"
            myGlobal.ErrorMessage = ex.Message
        End Try
        Return myGlobal
    End Function

    Private Function Exportar(ByVal pScriptsData As FwScriptsDataTO, ByVal pPath As String) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO
        Dim myUtil As New Utilities

        Try

            If pScriptsData IsNot Nothing Then
                Dim myScripts As New FwScripts
                myGlobal = myScripts.WriteFwScriptDataForCreating(pScriptsData, pPath, Encrypting)
            Else
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.FWSCRIPT_DATA_MISSING.ToString
            End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = "SYSTEM_ERROR"
            myGlobal.ErrorMessage = ex.Message

        End Try

        Return myGlobal

    End Function

    Private Function Importar(ByVal pScriptsDataType As Type, ByVal pPath As String) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO
        Dim myUtil As New Utilities

        Try
            Dim myScripts As New FwScripts
            myGlobal = myScripts.ReadFwScriptDataForCreating(pPath, Encrypting)
            Return myGlobal

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = "SYSTEM_ERROR"
            myGlobal.ErrorMessage = ex.Message

        End Try

        Return myGlobal


    End Function

    Private Sub CrearNuevo()
        Try
            If ChangesMade Then
                Dim res As DialogResult
                res = MessageBox.Show("Cambios sin guardar!" & vbCrLf & "¿Guardar?", Text, MessageBoxButtons.YesNoCancel)
                If res = Windows.Forms.DialogResult.Cancel Then
                    Exit Sub
                End If
                If res = Windows.Forms.DialogResult.Yes Then
                    AddDataToXMLFile()
                End If

            End If


            Dim myAnalyzerID As String = AgregarAnalizador(True)

            If myAnalyzerID.Length > 0 Then

                Me.CurrentAnalyzer = myAnalyzerID

                Me.Text = "Crear Scripts - Nuevo"

                BsAnalyzerComboBox.Items.Clear()
                BsAnalyzerComboBox.Items.Add("TODOS")
                BsAnalyzerComboBox.Items.Add(CurrentAnalyzer)
                Me.BsAnalyzerComboBox.SelectedItem = "TODOS"

                Me.BSScriptsDGV.Rows.Clear()
                Dim myNewRowIndex As Integer = BSScriptsDGV.Rows.Add
                IsAddingAttr = True

                CurrentAnalyzer = myAnalyzerID
                BSScriptsDGV.Rows(myNewRowIndex).Cells("colAnalyzer").Value = CurrentAnalyzer

                BsVersionTextBox.Text = "1.0.0.0"
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Function AgregarAnalizador(Optional ByVal isNewXML As Boolean = False) As String
        Try

            Dim myAnalyzerIDs As New List(Of String)
            For Each dr As DataGridViewRow In BSScriptsDGV.Rows
                If Not dr.IsNewRow Then
                    myAnalyzerIDs.Add(CStr(dr.Cells(0).Value).ToUpper.Trim)
                End If
            Next

            Dim isNewAnalyzer As Boolean = False
            Dim myAnalyzerID As String = InputBox("New AnalyzerID:")
            If myAnalyzerID.Length > 0 And Not myAnalyzerIDs.Contains(myAnalyzerID) Then
                isNewAnalyzer = True
            ElseIf myAnalyzerID = "" Then
                Return "" 'cancel
            End If

            If isNewXML Then

                Return myAnalyzerID

            ElseIf isNewAnalyzer Then
                If BSScriptsDGV.Rows.Count > 0 Then
                    Dim myFirstAnalyzerID As String = CStr(BSScriptsDGV.Rows(0).Cells(0).Value)
                    Dim CopiedAnalyzerRows As New List(Of DataGridViewRow)
                    For Each dr As DataGridViewRow In BSScriptsDGV.Rows
                        If Not dr.IsNewRow Then
                            If myFirstAnalyzerID.ToUpper.Trim = CStr(dr.Cells(0).Value).ToUpper.Trim Then
                                CopiedAnalyzerRows.Add(dr)
                            End If
                        End If
                    Next

                    If CopiedAnalyzerRows.Count > 0 Then
                        MessageBox.Show("se copiarán todos los elementos")

                        For Each dr As DataGridViewRow In CopiedAnalyzerRows
                            Dim myNewRow As DataGridViewRow = CType(dr.Clone, DataGridViewRow)
                            myNewRow.Cells(0).Value = myAnalyzerID
                            myNewRow.Cells(1).Value = dr.Cells(1).Value
                            myNewRow.Cells(2).Value = dr.Cells(2).Value
                            myNewRow.Cells(3).Value = dr.Cells(3).Value
                            BSScriptsDGV.Rows.Add(myNewRow)
                        Next
                    Else
                        Dim myNewRow As DataGridViewRow = BSScriptsDGV.Rows(0)
                        myNewRow.Cells(0).Value = myAnalyzerID
                    End If
                End If

                Return myAnalyzerID

            ElseIf myAnalyzerID.Length > 0 Then
                MessageBox.Show("ya existe este analizador")
                Return ""
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Return ""
        End Try
    End Function


    Private Sub BsButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsImportarButton.Click
        Try
            If ChangesMade Then
                Dim res As DialogResult
                res = MessageBox.Show("Cambios sin guardar!" & vbCrLf & "¿Guardar?", Text, MessageBoxButtons.YesNoCancel)
                If res = Windows.Forms.DialogResult.Cancel Then
                    Exit Sub
                End If
                If res = Windows.Forms.DialogResult.Yes Then
                    Me.AddDataToXMLFile()
                End If
            End If

            LoadDataFromXMLFile()

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub


    Private Sub BsButton2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsExportarButton.Click
        Try
            ValidateGrid()
            If Not ValidationError Then
                AddDataToXMLFile()

                ChangesMadeAttr = False

            Else
                MessageBox.Show("Error de Validación")
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

   

    'SGM
    Private Sub BSScriptsDGV_CellValidated(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles BSScriptsDGV.CellValidated
        Try
            Dim myGlobal As New GlobalDataTO

            If Not IsLoading And IsEditing And Not IsAdding Then
                Dim myRow As DataGridViewRow
                myRow = BSScriptsDGV.Rows(e.RowIndex)

                ValidateRow(myRow)

            End If

            If Not ValidationError Then
                IsAddingAttr = False
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

   
    'BORRAR FILA
    Private Sub BsButton3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsBorrarButton.Click
        Try
            Dim rowIndex As Integer
            If BSScriptsDGV.SelectedCells.Count > 0 Then
                rowIndex = BSScriptsDGV.SelectedCells(0).RowIndex
                BSScriptsDGV.Rows.RemoveAt(rowIndex)

            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub





    Private Sub BsButton5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsNuevoAnalizadorButton.Click
        Try
            AgregarAnalizador()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub


    Private Sub BSScriptsDGV_CellEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles BSScriptsDGV.CellEnter
        Try
            'SGM
            'If BSScriptsDGV.Rows(e.RowIndex).IsNewRow Or Me.IsLoading Then Exit Sub

            'If e.RowIndex = 0 Then
            '    If BSScriptsDGV.Rows(0).Cells(0).Value Is Nothing Then
            '        AgregarAnalizador()
            '    End If
            'ElseIf e.RowIndex > 0 Then
            '    If BSScriptsDGV.Rows(e.RowIndex).IsNewRow Then
            '        BSScriptsDGV.Rows(e.RowIndex).Cells(0).Value = BSScriptsDGV.Rows(e.RowIndex - 1).Cells(0).Value
            '    End If
            'End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    'SGM
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
           
           

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub TestCrearScripts_Shown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        Try

            Dim myGlobal As New GlobalDataTO
            Dim myUtil As New Utilities
            myGlobal = myUtil.GetSoftwareVersion
            If Not myGlobal.HasError And Not myGlobal Is Nothing Then
                BsVersionTextBox.Text = CStr(myGlobal.SetDatos)
            Else
                Me.BsVersionTextBox.Text = ""
            End If


            CrearNuevo()

            Dim id As Integer = BSScriptsDGV.Rows.Count
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    'SGM
    Private Sub BsNuevoButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsNuevoButton.Click
        Try
            If CurrentAnalyzer.Length > 0 Then
                If Not ValidationError And Not IsAdding Then
                    Dim myNewRowIndex As Integer = BSScriptsDGV.Rows.Add
                    Me.IsAddingAttr = True
                    Me.BSScriptsDGV.Rows(myNewRowIndex).Cells("colAnalyzer").Value = CurrentAnalyzer
                    If BSScriptsDGV.Rows.Count > 1 Then
                        Me.BSScriptsDGV.Rows(myNewRowIndex).Cells("colScreen").Value = Me.BSScriptsDGV.Rows(myNewRowIndex - 1).Cells("colScreen").Value
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    'SGM
    Private Sub BsAnalyzerComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsAnalyzerComboBox.SelectedIndexChanged
        Try
            If Not IsLoading And OriginalScriptsData IsNot Nothing Then
                If BsAnalyzerComboBox.Items.Count > 0 Then
                    Dim myAnalyzerID As String
                    myAnalyzerID = CStr(BsAnalyzerComboBox.SelectedItem)
                    CargarGrid(OriginalScriptsData, myAnalyzerID)
                    CurrentAnalyzer = CStr(BsAnalyzerComboBox.SelectedItem)
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    Private Sub BSScriptsDGV_CellBeginEdit(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellCancelEventArgs) Handles BSScriptsDGV.CellBeginEdit
        Try
            IsEditing = True

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub EncriptadoCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EncriptadoCheckBox.CheckedChanged
        Try
            EncryptingAttr = EncriptadoCheckBox.Checked
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub BsButton1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            ValidateGrid()
            If Not ValidationError Then
                CompilarGrid()
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub BSScriptsDGV_CellValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles BSScriptsDGV.CellValueChanged
        Try
            If Not IsLoading And IsEditing Then
                ChangesMadeAttr = True
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub BsButton1_Click_2(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton1.Click
        Try
            
            CrearNuevo()

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub


    Private Sub BSScriptsDGV_EditingControlShowing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles BSScriptsDGV.EditingControlShowing
        Try

            If Not TypeOf e.Control Is TextBox Then Return

            Dim tb As TextBox = CType(e.Control, TextBox)

            If Not tb Is Nothing Then
                AddHandler tb.KeyUp, AddressOf dgvTextBox_KeyUp
            End If


        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    'PENDING
    Private Sub dgvTextBox_KeyUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs)
        Try

            Select Case e.KeyCode
                Case Keys.Enter

                    'If Me.CurrentAnalyzer.Length > 0 Then
                    '    If Not ValidationError And Not Me.IsAdding Then
                    '        Dim myNewRowIndex As Integer = Me.BSScriptsDGV.Rows.Add
                    '        Me.IsAdding = True
                    '        Me.BSScriptsDGV.Rows(myNewRowIndex).Cells("colAnalyzer").Value = Me.CurrentAnalyzer
                    '        If Me.BSScriptsDGV.Rows.Count > 1 Then
                    '            Me.BSScriptsDGV.Rows(myNewRowIndex).Cells("colScreen").Value = Me.BSScriptsDGV.Rows(myNewRowIndex - 1).Cells("colScreen").Value
                    '        End If
                    '    End If
                    'End If

                Case Keys.Delete
                    'Dim rowIndex As Integer
                    'If Me.BSScriptsDGV.SelectedCells.Count > 0 Then
                    '    rowIndex = Me.BSScriptsDGV.SelectedCells(0).RowIndex
                    '    Me.BSScriptsDGV.Rows.RemoveAt(rowIndex)

                    'End If


            End Select


        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub


End Class