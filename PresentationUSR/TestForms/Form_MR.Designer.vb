<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form_MR
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.QcResultsCalculationDS1 = New Biosystems.Ax00.Types.QCResultsCalculationDS()
        Me.Btn_create_refresh = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsDelete_refresh = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsBtnUpdteBLexpirationdate = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.btnLogActivity = New Biosystems.Ax00.Controls.UserControls.BSButton()
        CType(Me.QcResultsCalculationDS1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'QcResultsCalculationDS1
        '
        Me.QcResultsCalculationDS1.DataSetName = "QCResultsCalculationDS"
        Me.QcResultsCalculationDS1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'Btn_create_refresh
        '
        Me.Btn_create_refresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.Btn_create_refresh.Location = New System.Drawing.Point(23, 12)
        Me.Btn_create_refresh.Name = "Btn_create_refresh"
        Me.Btn_create_refresh.Size = New System.Drawing.Size(142, 38)
        Me.Btn_create_refresh.TabIndex = 28
        Me.Btn_create_refresh.Text = "Crear y refrescar"
        Me.Btn_create_refresh.UseVisualStyleBackColor = True
        '
        'BsDelete_refresh
        '
        Me.BsDelete_refresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsDelete_refresh.Location = New System.Drawing.Point(23, 56)
        Me.BsDelete_refresh.Name = "BsDelete_refresh"
        Me.BsDelete_refresh.Size = New System.Drawing.Size(142, 38)
        Me.BsDelete_refresh.TabIndex = 29
        Me.BsDelete_refresh.Text = "Delete_alarm"
        Me.BsDelete_refresh.UseVisualStyleBackColor = True
        '
        'BsBtnUpdteBLexpirationdate
        '
        Me.BsBtnUpdteBLexpirationdate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsBtnUpdteBLexpirationdate.Location = New System.Drawing.Point(23, 100)
        Me.BsBtnUpdteBLexpirationdate.Name = "BsBtnUpdteBLexpirationdate"
        Me.BsBtnUpdteBLexpirationdate.Size = New System.Drawing.Size(142, 38)
        Me.BsBtnUpdteBLexpirationdate.TabIndex = 30
        Me.BsBtnUpdteBLexpirationdate.Text = "Update datetime BL_Expired"
        Me.BsBtnUpdteBLexpirationdate.UseVisualStyleBackColor = True
        '
        'btnLogActivity
        '
        Me.btnLogActivity.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnLogActivity.Location = New System.Drawing.Point(573, 12)
        Me.btnLogActivity.Name = "btnLogActivity"
        Me.btnLogActivity.Size = New System.Drawing.Size(142, 38)
        Me.btnLogActivity.TabIndex = 31
        Me.btnLogActivity.Text = "Check createLogActivity"
        Me.btnLogActivity.UseVisualStyleBackColor = True
        '
        'Form_MR
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(727, 262)
        Me.Controls.Add(Me.btnLogActivity)
        Me.Controls.Add(Me.BsBtnUpdteBLexpirationdate)
        Me.Controls.Add(Me.BsDelete_refresh)
        Me.Controls.Add(Me.Btn_create_refresh)
        Me.Name = "Form_MR"
        Me.Text = "Form_MR"
        CType(Me.QcResultsCalculationDS1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents QcResultsCalculationDS1 As Biosystems.Ax00.Types.QCResultsCalculationDS
    Friend WithEvents Btn_create_refresh As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsDelete_refresh As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsBtnUpdteBLexpirationdate As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents btnLogActivity As Biosystems.Ax00.Controls.UserControls.BSButton
End Class
