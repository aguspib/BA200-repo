Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.PresentationCOM
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()> _
Partial Class FormXavi
    Inherits BSBaseForm

    'Form overrides dispose to clean up the component list.
    <DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(FormXavi))
        Dim DataGridViewCellStyle5 As DataGridViewCellStyle = New DataGridViewCellStyle
        Dim DataGridViewCellStyle6 As DataGridViewCellStyle = New DataGridViewCellStyle
        Dim DataGridViewCellStyle7 As DataGridViewCellStyle = New DataGridViewCellStyle
        Dim DataGridViewCellStyle8 As DataGridViewCellStyle = New DataGridViewCellStyle
        Me.BsGroupBox1 = New BSGroupBox
        Me.BsLabel24 = New BSLabel
        Me.BsLabel23 = New BSLabel
        Me.BsLabel22 = New BSLabel
        Me.BsLabel21 = New BSLabel
        Me.Button5 = New Button
        Me.Button4 = New Button
        Me.BsTextWrite_Sleeping = New BSTextBox
        Me.BsReceptionButton_Sleeping = New BSButton
        Me.BsLabel20 = New BSLabel
        Me.BsButton18 = New BSButton
        Me.TextBox5 = New TextBox
        Me.BsLabel19 = New BSLabel
        Me.BsLabel18 = New BSLabel
        Me.BsLabel17 = New BSLabel
        Me.BsLabel16 = New BSLabel
        Me.BsLabel15 = New BSLabel
        Me.BsButton17 = New BSButton
        Me.BsLabel14 = New BSLabel
        Me.BsButton16 = New BSButton
        Me.BsLabel13 = New BSLabel
        Me.BsButton15 = New BSButton
        Me.BsLabel12 = New BSLabel
        Me.Button3 = New Button
        Me.BsButton13 = New BSButton
        Me.BsLabel11 = New BSLabel
        Me.BsButton14 = New BSButton
        Me.BsLabel10 = New BSLabel
        Me.Button2 = New Button
        Me.BsLabel9 = New BSLabel
        Me.Button1 = New Button
        Me.BsButton12 = New BSButton
        Me.BsButton8 = New BSButton
        Me.BsButton11 = New BSButton
        Me.BsButton10 = New BSButton
        Me.BsButton9 = New BSButton
        Me.BsLabel8 = New BSLabel
        Me.BsLabel7 = New BSLabel
        Me.BsLabel6 = New BSLabel
        Me.BsLabel5 = New BSLabel
        Me.BsLabel4 = New BSLabel
        Me.BsButton7 = New BSButton
        Me.BsButton6 = New BSButton
        Me.BsButton5 = New BSButton
        Me.BsLabel3 = New BSLabel
        Me.BsButton4 = New BSButton
        Me.AdjustmentsTextBox = New TextBox
        Me.Btn_ANSCMD_ERR = New BSButton
        Me.Btn_ANSCMD_OK = New BSButton
        Me.BsTextWrite_Error = New BSTextBox
        Me.BsReceptionButton_Error = New BSButton
        Me.BsListBox1 = New BSListBox
        Me.BsTextWrite_End = New BSTextBox
        Me.BsReceptionButton_End = New BSButton
        Me.BsGroupBox2 = New BSGroupBox
        Me.BsANSADJCheckbox = New BSCheckbox
        Me.BsANSINFOCheckbox = New BSCheckbox
        Me.BsReceivedTextBox = New RichTextBox
        Me.BsAction = New BSTextBox
        Me.BsRestore = New BSButton
        Me.BsInstruction = New BSTextBox
        Me.BsClearReception = New BSButton
        Me.BsLabel2 = New BSLabel
        Me.BsCommTestings = New BSButton
        Me.DataGridView1 = New DataGridView
        Me.BsDataGridView1 = New BSDataGridView
        Me.BsButton3 = New BSButton
        Me.BsLabel1 = New BSLabel
        Me.BsSendPrep = New BSButton
        Me.BsTextBoxToSend = New TextBox
        Me.BsRemainingTime = New BSButton
        Me.BsButton2 = New BSButton
        Me.BsExecution = New BSTextBox
        Me.BsExecuteCalc = New BSButton
        Me.BsReceptionButton = New BSButton
        Me.BsShortAction = New BSButton
        Me.BsTextWrite = New BSTextBox
        Me.BsButton1 = New BSButton
        Me.FwBlockTextBox = New TextBox
        Me.BsGroupBox1.SuspendLayout()
        Me.BsGroupBox2.SuspendLayout()
        CType(Me.DataGridView1, ISupportInitialize).BeginInit()
        CType(Me.BsDataGridView1, ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BsGroupBox1
        '
        Me.BsGroupBox1.Controls.Add(Me.FwBlockTextBox)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel24)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel23)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel22)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel21)
        Me.BsGroupBox1.Controls.Add(Me.Button5)
        Me.BsGroupBox1.Controls.Add(Me.Button4)
        Me.BsGroupBox1.Controls.Add(Me.BsTextWrite_Sleeping)
        Me.BsGroupBox1.Controls.Add(Me.BsReceptionButton_Sleeping)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel20)
        Me.BsGroupBox1.Controls.Add(Me.BsButton18)
        Me.BsGroupBox1.Controls.Add(Me.TextBox5)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel19)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel18)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel17)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel16)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel15)
        Me.BsGroupBox1.Controls.Add(Me.BsButton17)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel14)
        Me.BsGroupBox1.Controls.Add(Me.BsButton16)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel13)
        Me.BsGroupBox1.Controls.Add(Me.BsButton15)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel12)
        Me.BsGroupBox1.Controls.Add(Me.Button3)
        Me.BsGroupBox1.Controls.Add(Me.BsButton13)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel11)
        Me.BsGroupBox1.Controls.Add(Me.BsButton14)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel10)
        Me.BsGroupBox1.Controls.Add(Me.Button2)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel9)
        Me.BsGroupBox1.Controls.Add(Me.Button1)
        Me.BsGroupBox1.Controls.Add(Me.BsButton12)
        Me.BsGroupBox1.Controls.Add(Me.BsButton8)
        Me.BsGroupBox1.Controls.Add(Me.BsButton11)
        Me.BsGroupBox1.Controls.Add(Me.BsButton10)
        Me.BsGroupBox1.Controls.Add(Me.BsButton9)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel8)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel7)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel6)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel5)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel4)
        Me.BsGroupBox1.Controls.Add(Me.BsButton7)
        Me.BsGroupBox1.Controls.Add(Me.BsButton6)
        Me.BsGroupBox1.Controls.Add(Me.BsButton5)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel3)
        Me.BsGroupBox1.Controls.Add(Me.BsButton4)
        Me.BsGroupBox1.Controls.Add(Me.AdjustmentsTextBox)
        Me.BsGroupBox1.Controls.Add(Me.Btn_ANSCMD_ERR)
        Me.BsGroupBox1.Controls.Add(Me.Btn_ANSCMD_OK)
        Me.BsGroupBox1.Controls.Add(Me.BsTextWrite_Error)
        Me.BsGroupBox1.Controls.Add(Me.BsReceptionButton_Error)
        Me.BsGroupBox1.Controls.Add(Me.BsListBox1)
        Me.BsGroupBox1.Controls.Add(Me.BsTextWrite_End)
        Me.BsGroupBox1.Controls.Add(Me.BsReceptionButton_End)
        Me.BsGroupBox1.Controls.Add(Me.BsGroupBox2)
        Me.BsGroupBox1.Controls.Add(Me.DataGridView1)
        Me.BsGroupBox1.Controls.Add(Me.BsDataGridView1)
        Me.BsGroupBox1.Controls.Add(Me.BsButton3)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel1)
        Me.BsGroupBox1.Controls.Add(Me.BsSendPrep)
        Me.BsGroupBox1.Controls.Add(Me.BsTextBoxToSend)
        Me.BsGroupBox1.Controls.Add(Me.BsRemainingTime)
        Me.BsGroupBox1.Controls.Add(Me.BsButton2)
        Me.BsGroupBox1.Controls.Add(Me.BsExecution)
        Me.BsGroupBox1.Controls.Add(Me.BsExecuteCalc)
        Me.BsGroupBox1.Controls.Add(Me.BsReceptionButton)
        Me.BsGroupBox1.Controls.Add(Me.BsShortAction)
        Me.BsGroupBox1.Controls.Add(Me.BsTextWrite)
        Me.BsGroupBox1.ForeColor = Color.Black
        Me.BsGroupBox1.Location = New Point(129, 12)
        Me.BsGroupBox1.Name = "BsGroupBox1"
        Me.BsGroupBox1.Size = New Size(1105, 741)
        Me.BsGroupBox1.TabIndex = 11
        Me.BsGroupBox1.TabStop = False
        Me.BsGroupBox1.Text = "AG Testings"
        '
        'BsLabel24
        '
        Me.BsLabel24.AutoSize = True
        Me.BsLabel24.BackColor = Color.Transparent
        Me.BsLabel24.Font = New Font("Verdana", 8.25!)
        Me.BsLabel24.ForeColor = Color.Black
        Me.BsLabel24.Location = New Point(555, 693)
        Me.BsLabel24.Name = "BsLabel24"
        Me.BsLabel24.Size = New Size(47, 13)
        Me.BsLabel24.TabIndex = 96
        Me.BsLabel24.Text = "ERROR"
        Me.BsLabel24.Title = False
        '
        'BsLabel23
        '
        Me.BsLabel23.AutoSize = True
        Me.BsLabel23.BackColor = Color.Transparent
        Me.BsLabel23.Font = New Font("Verdana", 8.25!)
        Me.BsLabel23.ForeColor = Color.Black
        Me.BsLabel23.Location = New Point(555, 634)
        Me.BsLabel23.Name = "BsLabel23"
        Me.BsLabel23.Size = New Size(31, 13)
        Me.BsLabel23.TabIndex = 95
        Me.BsLabel23.Text = "END"
        Me.BsLabel23.Title = False
        '
        'BsLabel22
        '
        Me.BsLabel22.AutoSize = True
        Me.BsLabel22.BackColor = Color.Transparent
        Me.BsLabel22.Font = New Font("Verdana", 8.25!)
        Me.BsLabel22.ForeColor = Color.Black
        Me.BsLabel22.Location = New Point(555, 575)
        Me.BsLabel22.Name = "BsLabel22"
        Me.BsLabel22.Size = New Size(45, 13)
        Me.BsLabel22.TabIndex = 94
        Me.BsLabel22.Text = "START"
        Me.BsLabel22.Title = False
        '
        'BsLabel21
        '
        Me.BsLabel21.AutoSize = True
        Me.BsLabel21.BackColor = Color.Transparent
        Me.BsLabel21.Font = New Font("Verdana", 8.25!)
        Me.BsLabel21.ForeColor = Color.Black
        Me.BsLabel21.Location = New Point(555, 513)
        Me.BsLabel21.Name = "BsLabel21"
        Me.BsLabel21.Size = New Size(64, 13)
        Me.BsLabel21.TabIndex = 93
        Me.BsLabel21.Text = "SLEEPING"
        Me.BsLabel21.Title = False
        '
        'Button5
        '
        Me.Button5.Location = New Point(638, 430)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New Size(75, 23)
        Me.Button5.TabIndex = 92
        Me.Button5.Text = "ANSFWU"
        Me.Button5.UseVisualStyleBackColor = True
        '
        'Button4
        '
        Me.Button4.Location = New Point(334, 282)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New Size(75, 23)
        Me.Button4.TabIndex = 91
        Me.Button4.Text = "ANSFW"
        Me.Button4.UseVisualStyleBackColor = True
        '
        'BsTextWrite_Sleeping
        '
        Me.BsTextWrite_Sleeping.BackColor = Color.White
        Me.BsTextWrite_Sleeping.DecimalsValues = False
        Me.BsTextWrite_Sleeping.Font = New Font("Verdana", 8.25!)
        Me.BsTextWrite_Sleeping.IsNumeric = False
        Me.BsTextWrite_Sleeping.Location = New Point(555, 529)
        Me.BsTextWrite_Sleeping.Mandatory = False
        Me.BsTextWrite_Sleeping.Multiline = True
        Me.BsTextWrite_Sleeping.Name = "BsTextWrite_Sleeping"
        Me.BsTextWrite_Sleeping.Size = New Size(387, 24)
        Me.BsTextWrite_Sleeping.TabIndex = 90
        Me.BsTextWrite_Sleeping.Text = "A400;STATUS;S:1;A:1;T:0;C:6;W:0;R:0;E:0;I:1;"
        '
        'BsReceptionButton_Sleeping
        '
        Me.BsReceptionButton_Sleeping.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsReceptionButton_Sleeping.Location = New Point(220, 34)
        Me.BsReceptionButton_Sleeping.Name = "BsReceptionButton_Sleeping"
        Me.BsReceptionButton_Sleeping.Size = New Size(210, 23)
        Me.BsReceptionButton_Sleeping.TabIndex = 89
        Me.BsReceptionButton_Sleeping.Text = "SLEEPING Instruction"
        Me.BsReceptionButton_Sleeping.UseVisualStyleBackColor = True
        '
        'BsLabel20
        '
        Me.BsLabel20.AutoSize = True
        Me.BsLabel20.BackColor = Color.Transparent
        Me.BsLabel20.Font = New Font("Verdana", 8.25!)
        Me.BsLabel20.ForeColor = Color.Black
        Me.BsLabel20.Location = New Point(113, 129)
        Me.BsLabel20.Name = "BsLabel20"
        Me.BsLabel20.Size = New Size(27, 13)
        Me.BsLabel20.TabIndex = 88
        Me.BsLabel20.Text = "ISE"
        Me.BsLabel20.Title = False
        '
        'BsButton18
        '
        Me.BsButton18.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton18.Location = New Point(113, 145)
        Me.BsButton18.Name = "BsButton18"
        Me.BsButton18.Size = New Size(73, 25)
        Me.BsButton18.TabIndex = 87
        Me.BsButton18.Text = "ANSISE"
        Me.BsButton18.UseVisualStyleBackColor = True
        '
        'TextBox5
        '
        Me.TextBox5.Location = New Point(548, 65)
        Me.TextBox5.Name = "TextBox5"
        Me.TextBox5.Size = New Size(130, 21)
        Me.TextBox5.TabIndex = 86
        '
        'BsLabel19
        '
        Me.BsLabel19.AutoSize = True
        Me.BsLabel19.BackColor = Color.Transparent
        Me.BsLabel19.Font = New Font("Verdana", 8.25!)
        Me.BsLabel19.ForeColor = Color.Black
        Me.BsLabel19.Location = New Point(377, 266)
        Me.BsLabel19.Name = "BsLabel19"
        Me.BsLabel19.Size = New Size(24, 13)
        Me.BsLabel19.TabIndex = 84
        Me.BsLabel19.Text = "(1)"
        Me.BsLabel19.Title = False
        '
        'BsLabel18
        '
        Me.BsLabel18.AutoSize = True
        Me.BsLabel18.BackColor = Color.Transparent
        Me.BsLabel18.Font = New Font("Verdana", 8.25!)
        Me.BsLabel18.ForeColor = Color.Black
        Me.BsLabel18.Location = New Point(346, 318)
        Me.BsLabel18.Name = "BsLabel18"
        Me.BsLabel18.Size = New Size(31, 13)
        Me.BsLabel18.TabIndex = 83
        Me.BsLabel18.Text = "(10)"
        Me.BsLabel18.Title = False
        '
        'BsLabel17
        '
        Me.BsLabel17.AutoSize = True
        Me.BsLabel17.BackColor = Color.Transparent
        Me.BsLabel17.Font = New Font("Verdana", 8.25!)
        Me.BsLabel17.ForeColor = Color.Black
        Me.BsLabel17.Location = New Point(268, 318)
        Me.BsLabel17.Name = "BsLabel17"
        Me.BsLabel17.Size = New Size(24, 13)
        Me.BsLabel17.TabIndex = 82
        Me.BsLabel17.Text = "(7)"
        Me.BsLabel17.Title = False
        '
        'BsLabel16
        '
        Me.BsLabel16.AutoSize = True
        Me.BsLabel16.BackColor = Color.Transparent
        Me.BsLabel16.Font = New Font("Verdana", 8.25!)
        Me.BsLabel16.ForeColor = Color.Black
        Me.BsLabel16.Location = New Point(207, 318)
        Me.BsLabel16.Name = "BsLabel16"
        Me.BsLabel16.Size = New Size(24, 13)
        Me.BsLabel16.TabIndex = 81
        Me.BsLabel16.Text = "(4)"
        Me.BsLabel16.Title = False
        '
        'BsLabel15
        '
        Me.BsLabel15.AutoSize = True
        Me.BsLabel15.BackColor = Color.Transparent
        Me.BsLabel15.Font = New Font("Verdana", 8.25!)
        Me.BsLabel15.ForeColor = Color.Black
        Me.BsLabel15.Location = New Point(310, 318)
        Me.BsLabel15.Name = "BsLabel15"
        Me.BsLabel15.Size = New Size(30, 13)
        Me.BsLabel15.TabIndex = 80
        Me.BsLabel15.Text = "RR1"
        Me.BsLabel15.Title = False
        '
        'BsButton17
        '
        Me.BsButton17.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton17.Location = New Point(310, 334)
        Me.BsButton17.Name = "BsButton17"
        Me.BsButton17.Size = New Size(67, 25)
        Me.BsButton17.TabIndex = 79
        Me.BsButton17.Text = "ROTORS"
        Me.BsButton17.UseVisualStyleBackColor = True
        '
        'BsLabel14
        '
        Me.BsLabel14.AutoSize = True
        Me.BsLabel14.BackColor = Color.Transparent
        Me.BsLabel14.Font = New Font("Verdana", 8.25!)
        Me.BsLabel14.ForeColor = Color.Black
        Me.BsLabel14.Location = New Point(234, 318)
        Me.BsLabel14.Name = "BsLabel14"
        Me.BsLabel14.Size = New Size(31, 13)
        Me.BsLabel14.TabIndex = 78
        Me.BsLabel14.Text = "DR1"
        Me.BsLabel14.Title = False
        '
        'BsButton16
        '
        Me.BsButton16.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton16.Location = New Point(237, 334)
        Me.BsButton16.Name = "BsButton16"
        Me.BsButton16.Size = New Size(67, 25)
        Me.BsButton16.TabIndex = 77
        Me.BsButton16.Text = "PROBES"
        Me.BsButton16.UseVisualStyleBackColor = True
        '
        'BsLabel13
        '
        Me.BsLabel13.AutoSize = True
        Me.BsLabel13.BackColor = Color.Transparent
        Me.BsLabel13.Font = New Font("Verdana", 8.25!)
        Me.BsLabel13.ForeColor = Color.Black
        Me.BsLabel13.Location = New Point(175, 318)
        Me.BsLabel13.Name = "BsLabel13"
        Me.BsLabel13.Size = New Size(31, 13)
        Me.BsLabel13.TabIndex = 76
        Me.BsLabel13.Text = "BM1"
        Me.BsLabel13.Title = False
        '
        'BsButton15
        '
        Me.BsButton15.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton15.Location = New Point(178, 334)
        Me.BsButton15.Name = "BsButton15"
        Me.BsButton15.Size = New Size(53, 25)
        Me.BsButton15.TabIndex = 75
        Me.BsButton15.Text = "ARMS"
        Me.BsButton15.UseVisualStyleBackColor = True
        '
        'BsLabel12
        '
        Me.BsLabel12.AutoSize = True
        Me.BsLabel12.BackColor = Color.Transparent
        Me.BsLabel12.Font = New Font("Verdana", 8.25!)
        Me.BsLabel12.ForeColor = Color.Black
        Me.BsLabel12.Location = New Point(331, 266)
        Me.BsLabel12.Name = "BsLabel12"
        Me.BsLabel12.Size = New Size(31, 13)
        Me.BsLabel12.TabIndex = 74
        Me.BsLabel12.Text = "CPU"
        Me.BsLabel12.Title = False
        '
        'Button3
        '
        Me.Button3.Location = New Point(220, 282)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New Size(75, 23)
        Me.Button3.TabIndex = 73
        Me.Button3.Text = "ANSFCP"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'BsButton13
        '
        Me.BsButton13.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton13.Location = New Point(90, 334)
        Me.BsButton13.Name = "BsButton13"
        Me.BsButton13.Size = New Size(80, 25)
        Me.BsButton13.TabIndex = 72
        Me.BsButton13.Text = "MANIFOLD"
        Me.BsButton13.UseVisualStyleBackColor = True
        '
        'BsLabel11
        '
        Me.BsLabel11.AutoSize = True
        Me.BsLabel11.BackColor = Color.Transparent
        Me.BsLabel11.Font = New Font("Verdana", 8.25!)
        Me.BsLabel11.ForeColor = Color.Black
        Me.BsLabel11.Location = New Point(13, 318)
        Me.BsLabel11.Name = "BsLabel11"
        Me.BsLabel11.Size = New Size(97, 13)
        Me.BsLabel11.TabIndex = 71
        Me.BsLabel11.Text = "ANS to POLLHW"
        Me.BsLabel11.Title = False
        '
        'BsButton14
        '
        Me.BsButton14.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton14.Location = New Point(9, 334)
        Me.BsButton14.Name = "BsButton14"
        Me.BsButton14.Size = New Size(75, 25)
        Me.BsButton14.TabIndex = 70
        Me.BsButton14.Text = "FLUIDICS"
        Me.BsButton14.UseVisualStyleBackColor = True
        '
        'BsLabel10
        '
        Me.BsLabel10.AutoSize = True
        Me.BsLabel10.BackColor = Color.Transparent
        Me.BsLabel10.Font = New Font("Verdana", 8.25!)
        Me.BsLabel10.ForeColor = Color.Black
        Me.BsLabel10.Location = New Point(115, 266)
        Me.BsLabel10.Name = "BsLabel10"
        Me.BsLabel10.Size = New Size(42, 13)
        Me.BsLabel10.TabIndex = 69
        Me.BsLabel10.Text = "Errors"
        Me.BsLabel10.Title = False
        '
        'Button2
        '
        Me.Button2.Location = New Point(113, 282)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New Size(75, 23)
        Me.Button2.TabIndex = 68
        Me.Button2.Text = "ANSERR"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'BsLabel9
        '
        Me.BsLabel9.AutoSize = True
        Me.BsLabel9.BackColor = Color.Transparent
        Me.BsLabel9.Font = New Font("Verdana", 8.25!)
        Me.BsLabel9.ForeColor = Color.Black
        Me.BsLabel9.Location = New Point(6, 266)
        Me.BsLabel9.Name = "BsLabel9"
        Me.BsLabel9.Size = New Size(53, 13)
        Me.BsLabel9.TabIndex = 67
        Me.BsLabel9.Text = "Sensors"
        Me.BsLabel9.Title = False
        '
        'Button1
        '
        Me.Button1.Location = New Point(9, 282)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New Size(75, 23)
        Me.Button1.TabIndex = 12
        Me.Button1.Text = "ANSINF"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'BsButton12
        '
        Me.BsButton12.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton12.Location = New Point(327, 113)
        Me.BsButton12.Name = "BsButton12"
        Me.BsButton12.Size = New Size(130, 25)
        Me.BsButton12.TabIndex = 66
        Me.BsButton12.Text = "ANSCMD DATA"
        Me.BsButton12.UseVisualStyleBackColor = True
        '
        'BsButton8
        '
        Me.BsButton8.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton8.Location = New Point(220, 63)
        Me.BsButton8.Name = "BsButton8"
        Me.BsButton8.Size = New Size(101, 25)
        Me.BsButton8.TabIndex = 65
        Me.BsButton8.Text = "Conx DONE"
        Me.BsButton8.UseVisualStyleBackColor = True
        '
        'BsButton11
        '
        Me.BsButton11.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton11.Location = New Point(113, 231)
        Me.BsButton11.Name = "BsButton11"
        Me.BsButton11.Size = New Size(101, 25)
        Me.BsButton11.TabIndex = 64
        Me.BsButton11.Text = "STRESS CURS"
        Me.BsButton11.UseVisualStyleBackColor = True
        '
        'BsButton10
        '
        Me.BsButton10.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton10.Location = New Point(220, 231)
        Me.BsButton10.Name = "BsButton10"
        Me.BsButton10.Size = New Size(101, 25)
        Me.BsButton10.TabIndex = 63
        Me.BsButton10.Text = "STRESS FI OK"
        Me.BsButton10.UseVisualStyleBackColor = True
        '
        'BsButton9
        '
        Me.BsButton9.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton9.Location = New Point(327, 231)
        Me.BsButton9.Name = "BsButton9"
        Me.BsButton9.Size = New Size(101, 25)
        Me.BsButton9.TabIndex = 62
        Me.BsButton9.Text = "STRESS FI ERR"
        Me.BsButton9.UseVisualStyleBackColor = True
        '
        'BsLabel8
        '
        Me.BsLabel8.AutoSize = True
        Me.BsLabel8.BackColor = Color.Transparent
        Me.BsLabel8.Font = New Font("Verdana", 8.25!)
        Me.BsLabel8.ForeColor = Color.Black
        Me.BsLabel8.Location = New Point(6, 215)
        Me.BsLabel8.Name = "BsLabel8"
        Me.BsLabel8.Size = New Size(43, 13)
        Me.BsLabel8.TabIndex = 60
        Me.BsLabel8.Text = "Stress"
        Me.BsLabel8.Title = False
        '
        'BsLabel7
        '
        Me.BsLabel7.AutoSize = True
        Me.BsLabel7.BackColor = Color.Transparent
        Me.BsLabel7.Font = New Font("Verdana", 8.25!)
        Me.BsLabel7.ForeColor = Color.Black
        Me.BsLabel7.Location = New Point(217, 176)
        Me.BsLabel7.Name = "BsLabel7"
        Me.BsLabel7.Size = New Size(78, 13)
        Me.BsLabel7.TabIndex = 59
        Me.BsLabel7.Text = "Adjustments"
        Me.BsLabel7.Title = False
        '
        'BsLabel6
        '
        Me.BsLabel6.AutoSize = True
        Me.BsLabel6.BackColor = Color.Transparent
        Me.BsLabel6.Font = New Font("Verdana", 8.25!)
        Me.BsLabel6.ForeColor = Color.Black
        Me.BsLabel6.Location = New Point(6, 129)
        Me.BsLabel6.Name = "BsLabel6"
        Me.BsLabel6.Size = New Size(73, 13)
        Me.BsLabel6.TabIndex = 58
        Me.BsLabel6.Text = "Photometry"
        Me.BsLabel6.Title = False
        '
        'BsLabel5
        '
        Me.BsLabel5.AutoSize = True
        Me.BsLabel5.BackColor = Color.Transparent
        Me.BsLabel5.Font = New Font("Verdana", 8.25!)
        Me.BsLabel5.ForeColor = Color.Black
        Me.BsLabel5.Location = New Point(222, 97)
        Me.BsLabel5.Name = "BsLabel5"
        Me.BsLabel5.Size = New Size(128, 13)
        Me.BsLabel5.TabIndex = 57
        Me.BsLabel5.Text = "Command Fw Scripts"
        Me.BsLabel5.Title = False
        '
        'BsLabel4
        '
        Me.BsLabel4.AutoSize = True
        Me.BsLabel4.BackColor = Color.Transparent
        Me.BsLabel4.Font = New Font("Verdana", 8.25!)
        Me.BsLabel4.ForeColor = Color.Black
        Me.BsLabel4.Location = New Point(6, 18)
        Me.BsLabel4.Name = "BsLabel4"
        Me.BsLabel4.Size = New Size(43, 13)
        Me.BsLabel4.TabIndex = 56
        Me.BsLabel4.Text = "Status"
        Me.BsLabel4.Title = False
        '
        'BsButton7
        '
        Me.BsButton7.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton7.Location = New Point(6, 145)
        Me.BsButton7.Name = "BsButton7"
        Me.BsButton7.Size = New Size(73, 25)
        Me.BsButton7.TabIndex = 55
        Me.BsButton7.Text = "ANSBLD 1"
        Me.BsButton7.UseVisualStyleBackColor = True
        '
        'BsButton6
        '
        Me.BsButton6.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton6.Location = New Point(6, 176)
        Me.BsButton6.Name = "BsButton6"
        Me.BsButton6.Size = New Size(73, 25)
        Me.BsButton6.TabIndex = 54
        Me.BsButton6.Text = "ANSBLD 2"
        Me.BsButton6.UseVisualStyleBackColor = True
        '
        'BsButton5
        '
        Me.BsButton5.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton5.Location = New Point(9, 231)
        Me.BsButton5.Name = "BsButton5"
        Me.BsButton5.Size = New Size(101, 25)
        Me.BsButton5.TabIndex = 53
        Me.BsButton5.Text = "STRESS 1"
        Me.BsButton5.UseVisualStyleBackColor = True
        '
        'BsLabel3
        '
        Me.BsLabel3.AutoSize = True
        Me.BsLabel3.BackColor = Color.Transparent
        Me.BsLabel3.Font = New Font("Verdana", 8.25!)
        Me.BsLabel3.ForeColor = Color.Black
        Me.BsLabel3.Location = New Point(691, 16)
        Me.BsLabel3.Name = "BsLabel3"
        Me.BsLabel3.Size = New Size(78, 13)
        Me.BsLabel3.TabIndex = 42
        Me.BsLabel3.Text = "Adjustments"
        Me.BsLabel3.Title = False
        '
        'BsButton4
        '
        Me.BsButton4.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton4.Location = New Point(220, 192)
        Me.BsButton4.Name = "BsButton4"
        Me.BsButton4.Size = New Size(101, 25)
        Me.BsButton4.TabIndex = 43
        Me.BsButton4.Text = "Send ADJ"
        Me.BsButton4.UseVisualStyleBackColor = True
        '
        'AdjustmentsTextBox
        '
        Me.AdjustmentsTextBox.Location = New Point(694, 32)
        Me.AdjustmentsTextBox.Multiline = True
        Me.AdjustmentsTextBox.Name = "AdjustmentsTextBox"
        Me.AdjustmentsTextBox.Size = New Size(405, 133)
        Me.AdjustmentsTextBox.TabIndex = 38
        Me.AdjustmentsTextBox.Text = resources.GetString("AdjustmentsTextBox.Text")
        '
        'Btn_ANSCMD_ERR
        '
        Me.Btn_ANSCMD_ERR.BackgroundImageLayout = ImageLayout.Stretch
        Me.Btn_ANSCMD_ERR.Location = New Point(225, 140)
        Me.Btn_ANSCMD_ERR.Name = "Btn_ANSCMD_ERR"
        Me.Btn_ANSCMD_ERR.Size = New Size(96, 25)
        Me.Btn_ANSCMD_ERR.TabIndex = 52
        Me.Btn_ANSCMD_ERR.Text = "ANSCMD ERR ?"
        Me.Btn_ANSCMD_ERR.UseVisualStyleBackColor = True
        '
        'Btn_ANSCMD_OK
        '
        Me.Btn_ANSCMD_OK.BackgroundImageLayout = ImageLayout.Stretch
        Me.Btn_ANSCMD_OK.Location = New Point(225, 113)
        Me.Btn_ANSCMD_OK.Name = "Btn_ANSCMD_OK"
        Me.Btn_ANSCMD_OK.Size = New Size(96, 25)
        Me.Btn_ANSCMD_OK.TabIndex = 51
        Me.Btn_ANSCMD_OK.Text = "ANSCMD OK"
        Me.Btn_ANSCMD_OK.UseVisualStyleBackColor = True
        '
        'BsTextWrite_Error
        '
        Me.BsTextWrite_Error.BackColor = Color.White
        Me.BsTextWrite_Error.DecimalsValues = False
        Me.BsTextWrite_Error.Font = New Font("Verdana", 8.25!)
        Me.BsTextWrite_Error.IsNumeric = False
        Me.BsTextWrite_Error.Location = New Point(555, 709)
        Me.BsTextWrite_Error.Mandatory = False
        Me.BsTextWrite_Error.Multiline = True
        Me.BsTextWrite_Error.Name = "BsTextWrite_Error"
        Me.BsTextWrite_Error.Size = New Size(387, 24)
        Me.BsTextWrite_Error.TabIndex = 50
        Me.BsTextWrite_Error.Text = "A400;STATUS;S:2;A:51;T:0;C:6;W:0;R:0;E:44;I:1;"
        '
        'BsReceptionButton_Error
        '
        Me.BsReceptionButton_Error.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsReceptionButton_Error.Location = New Point(6, 92)
        Me.BsReceptionButton_Error.Name = "BsReceptionButton_Error"
        Me.BsReceptionButton_Error.Size = New Size(210, 23)
        Me.BsReceptionButton_Error.TabIndex = 49
        Me.BsReceptionButton_Error.Text = "ERROR Instruction"
        Me.BsReceptionButton_Error.UseVisualStyleBackColor = True
        '
        'BsListBox1
        '
        Me.BsListBox1.FormattingEnabled = True
        Me.BsListBox1.Location = New Point(438, 180)
        Me.BsListBox1.Name = "BsListBox1"
        Me.BsListBox1.Size = New Size(661, 134)
        Me.BsListBox1.TabIndex = 48
        '
        'BsTextWrite_End
        '
        Me.BsTextWrite_End.BackColor = Color.White
        Me.BsTextWrite_End.DecimalsValues = False
        Me.BsTextWrite_End.Font = New Font("Verdana", 8.25!)
        Me.BsTextWrite_End.IsNumeric = False
        Me.BsTextWrite_End.Location = New Point(555, 650)
        Me.BsTextWrite_End.Mandatory = False
        Me.BsTextWrite_End.Multiline = True
        Me.BsTextWrite_End.Name = "BsTextWrite_End"
        Me.BsTextWrite_End.Size = New Size(387, 24)
        Me.BsTextWrite_End.TabIndex = 47
        Me.BsTextWrite_End.Text = "A400;STATUS;S:2;A:55;T:0;C:6;W:0;R:0;E:0;I:1;"
        '
        'BsReceptionButton_End
        '
        Me.BsReceptionButton_End.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsReceptionButton_End.Location = New Point(4, 63)
        Me.BsReceptionButton_End.Name = "BsReceptionButton_End"
        Me.BsReceptionButton_End.Size = New Size(210, 23)
        Me.BsReceptionButton_End.TabIndex = 46
        Me.BsReceptionButton_End.Text = "END Reception Instruction"
        Me.BsReceptionButton_End.UseVisualStyleBackColor = True
        '
        'BsGroupBox2
        '
        Me.BsGroupBox2.Controls.Add(Me.BsANSADJCheckbox)
        Me.BsGroupBox2.Controls.Add(Me.BsANSINFOCheckbox)
        Me.BsGroupBox2.Controls.Add(Me.BsReceivedTextBox)
        Me.BsGroupBox2.Controls.Add(Me.BsAction)
        Me.BsGroupBox2.Controls.Add(Me.BsRestore)
        Me.BsGroupBox2.Controls.Add(Me.BsInstruction)
        Me.BsGroupBox2.Controls.Add(Me.BsClearReception)
        Me.BsGroupBox2.Controls.Add(Me.BsLabel2)
        Me.BsGroupBox2.Controls.Add(Me.BsCommTestings)
        Me.BsGroupBox2.ForeColor = Color.Black
        Me.BsGroupBox2.Location = New Point(6, 362)
        Me.BsGroupBox2.Name = "BsGroupBox2"
        Me.BsGroupBox2.Size = New Size(543, 373)
        Me.BsGroupBox2.TabIndex = 45
        Me.BsGroupBox2.TabStop = False
        Me.BsGroupBox2.Text = "Communications Testings"
        '
        'BsANSADJCheckbox
        '
        Me.BsANSADJCheckbox.AutoSize = True
        Me.BsANSADJCheckbox.Location = New Point(417, 33)
        Me.BsANSADJCheckbox.Name = "BsANSADJCheckbox"
        Me.BsANSADJCheckbox.Size = New Size(103, 17)
        Me.BsANSADJCheckbox.TabIndex = 50
        Me.BsANSADJCheckbox.Text = "View ANSADJ"
        Me.BsANSADJCheckbox.UseVisualStyleBackColor = True
        '
        'BsANSINFOCheckbox
        '
        Me.BsANSINFOCheckbox.AutoSize = True
        Me.BsANSINFOCheckbox.Location = New Point(417, 56)
        Me.BsANSINFOCheckbox.Name = "BsANSINFOCheckbox"
        Me.BsANSINFOCheckbox.Size = New Size(109, 17)
        Me.BsANSINFOCheckbox.TabIndex = 49
        Me.BsANSINFOCheckbox.Text = "View ANSINFO"
        Me.BsANSINFOCheckbox.UseVisualStyleBackColor = True
        '
        'BsReceivedTextBox
        '
        Me.BsReceivedTextBox.Location = New Point(6, 77)
        Me.BsReceivedTextBox.Name = "BsReceivedTextBox"
        Me.BsReceivedTextBox.Size = New Size(526, 284)
        Me.BsReceivedTextBox.TabIndex = 48
        Me.BsReceivedTextBox.Text = ""
        '
        'BsAction
        '
        Me.BsAction.BackColor = Color.White
        Me.BsAction.DecimalsValues = False
        Me.BsAction.Font = New Font("Verdana", 8.25!)
        Me.BsAction.IsNumeric = False
        Me.BsAction.Location = New Point(161, 47)
        Me.BsAction.Mandatory = False
        Me.BsAction.Name = "BsAction"
        Me.BsAction.Size = New Size(125, 21)
        Me.BsAction.TabIndex = 47
        Me.BsAction.Text = "action1"
        '
        'BsRestore
        '
        Me.BsRestore.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsRestore.Location = New Point(307, 23)
        Me.BsRestore.Name = "BsRestore"
        Me.BsRestore.Size = New Size(96, 21)
        Me.BsRestore.TabIndex = 46
        Me.BsRestore.Text = "Restore Comms"
        Me.BsRestore.UseVisualStyleBackColor = True
        '
        'BsInstruction
        '
        Me.BsInstruction.BackColor = Color.White
        Me.BsInstruction.DecimalsValues = False
        Me.BsInstruction.Font = New Font("Verdana", 8.25!)
        Me.BsInstruction.IsNumeric = False
        Me.BsInstruction.Location = New Point(161, 24)
        Me.BsInstruction.Mandatory = False
        Me.BsInstruction.Name = "BsInstruction"
        Me.BsInstruction.Size = New Size(125, 21)
        Me.BsInstruction.TabIndex = 14
        Me.BsInstruction.Text = "script"
        '
        'BsClearReception
        '
        Me.BsClearReception.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsClearReception.Location = New Point(307, 50)
        Me.BsClearReception.Name = "BsClearReception"
        Me.BsClearReception.Size = New Size(96, 21)
        Me.BsClearReception.TabIndex = 13
        Me.BsClearReception.Text = "Clear LOG"
        Me.BsClearReception.UseVisualStyleBackColor = True
        '
        'BsLabel2
        '
        Me.BsLabel2.AutoSize = True
        Me.BsLabel2.BackColor = Color.Transparent
        Me.BsLabel2.Font = New Font("Verdana", 8.25!)
        Me.BsLabel2.ForeColor = Color.Black
        Me.BsLabel2.Location = New Point(6, 58)
        Me.BsLabel2.Name = "BsLabel2"
        Me.BsLabel2.Size = New Size(122, 13)
        Me.BsLabel2.TabIndex = 1
        Me.BsLabel2.Text = "Received instruction"
        Me.BsLabel2.Title = False
        '
        'BsCommTestings
        '
        Me.BsCommTestings.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsCommTestings.Location = New Point(6, 24)
        Me.BsCommTestings.Name = "BsCommTestings"
        Me.BsCommTestings.Size = New Size(134, 21)
        Me.BsCommTestings.TabIndex = 0
        Me.BsCommTestings.Text = "Send instruction"
        Me.BsCommTestings.UseVisualStyleBackColor = True
        '
        'DataGridView1
        '
        Me.DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Location = New Point(800, 318)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New Size(299, 45)
        Me.DataGridView1.TabIndex = 44
        '
        'BsDataGridView1
        '
        DataGridViewCellStyle5.Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = Color.Black
        DataGridViewCellStyle5.SelectionBackColor = Color.LightSlateGray
        Me.BsDataGridView1.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle5
        Me.BsDataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle6.BackColor = Color.DarkGray
        DataGridViewCellStyle6.Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle6.ForeColor = Color.Black
        DataGridViewCellStyle6.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle6.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle6.WrapMode = DataGridViewTriState.[False]
        Me.BsDataGridView1.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle6
        Me.BsDataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle7.BackColor = SystemColors.Window
        DataGridViewCellStyle7.Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle7.ForeColor = Color.Black
        DataGridViewCellStyle7.SelectionBackColor = Color.LightSlateGray
        DataGridViewCellStyle7.SelectionForeColor = Color.White
        DataGridViewCellStyle7.WrapMode = DataGridViewTriState.[False]
        Me.BsDataGridView1.DefaultCellStyle = DataGridViewCellStyle7
        Me.BsDataGridView1.EnterToTab = False
        Me.BsDataGridView1.GridColor = Color.Silver
        Me.BsDataGridView1.Location = New Point(800, 369)
        Me.BsDataGridView1.Name = "BsDataGridView1"
        DataGridViewCellStyle8.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle8.BackColor = Color.DarkGray
        DataGridViewCellStyle8.Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle8.ForeColor = SystemColors.WindowText
        DataGridViewCellStyle8.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle8.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle8.WrapMode = DataGridViewTriState.[False]
        Me.BsDataGridView1.RowHeadersDefaultCellStyle = DataGridViewCellStyle8
        Me.BsDataGridView1.Size = New Size(318, 45)
        Me.BsDataGridView1.TabIndex = 43
        Me.BsDataGridView1.TabToEnter = False
        '
        'BsButton3
        '
        Me.BsButton3.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton3.Location = New Point(438, 63)
        Me.BsButton3.Name = "BsButton3"
        Me.BsButton3.Size = New Size(104, 32)
        Me.BsButton3.TabIndex = 42
        Me.BsButton3.Text = "Process Response"
        Me.BsButton3.UseVisualStyleBackColor = True
        '
        'BsLabel1
        '
        Me.BsLabel1.AutoSize = True
        Me.BsLabel1.BackColor = Color.Transparent
        Me.BsLabel1.Font = New Font("Verdana", 8.25!)
        Me.BsLabel1.ForeColor = Color.Black
        Me.BsLabel1.Location = New Point(545, 16)
        Me.BsLabel1.Name = "BsLabel1"
        Me.BsLabel1.Size = New Size(111, 13)
        Me.BsLabel1.TabIndex = 41
        Me.BsLabel1.Text = "Execution Number"
        Me.BsLabel1.Title = False
        '
        'BsSendPrep
        '
        Me.BsSendPrep.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsSendPrep.Location = New Point(438, 32)
        Me.BsSendPrep.Name = "BsSendPrep"
        Me.BsSendPrep.Size = New Size(101, 25)
        Me.BsSendPrep.TabIndex = 40
        Me.BsSendPrep.Text = "Preparation"
        Me.BsSendPrep.UseVisualStyleBackColor = True
        '
        'BsTextBoxToSend
        '
        Me.BsTextBoxToSend.Location = New Point(548, 32)
        Me.BsTextBoxToSend.Name = "BsTextBoxToSend"
        Me.BsTextBoxToSend.Size = New Size(122, 21)
        Me.BsTextBoxToSend.TabIndex = 37
        '
        'BsRemainingTime
        '
        Me.BsRemainingTime.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsRemainingTime.Location = New Point(800, 433)
        Me.BsRemainingTime.Name = "BsRemainingTime"
        Me.BsRemainingTime.Size = New Size(160, 24)
        Me.BsRemainingTime.TabIndex = 22
        Me.BsRemainingTime.Text = "RemainingTime"
        Me.BsRemainingTime.UseVisualStyleBackColor = True
        '
        'BsButton2
        '
        Me.BsButton2.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton2.Location = New Point(694, 320)
        Me.BsButton2.Name = "BsButton2"
        Me.BsButton2.Size = New Size(100, 23)
        Me.BsButton2.TabIndex = 16
        Me.BsButton2.Text = "SendBaseLine..."
        Me.BsButton2.UseVisualStyleBackColor = True
        '
        'BsExecution
        '
        Me.BsExecution.BackColor = Color.White
        Me.BsExecution.DecimalsValues = False
        Me.BsExecution.Font = New Font("Verdana", 8.25!)
        Me.BsExecution.IsNumeric = False
        Me.BsExecution.Location = New Point(962, 434)
        Me.BsExecution.Mandatory = False
        Me.BsExecution.Multiline = True
        Me.BsExecution.Name = "BsExecution"
        Me.BsExecution.Size = New Size(89, 23)
        Me.BsExecution.TabIndex = 15
        '
        'BsExecuteCalc
        '
        Me.BsExecuteCalc.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsExecuteCalc.Location = New Point(1057, 432)
        Me.BsExecuteCalc.Name = "BsExecuteCalc"
        Me.BsExecuteCalc.Size = New Size(130, 23)
        Me.BsExecuteCalc.TabIndex = 14
        Me.BsExecuteCalc.Text = "Execute Calculations"
        Me.BsExecuteCalc.UseVisualStyleBackColor = True
        '
        'BsReceptionButton
        '
        Me.BsReceptionButton.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsReceptionButton.Location = New Point(4, 34)
        Me.BsReceptionButton.Name = "BsReceptionButton"
        Me.BsReceptionButton.Size = New Size(210, 23)
        Me.BsReceptionButton.TabIndex = 13
        Me.BsReceptionButton.Text = "START Reception Instruction"
        Me.BsReceptionButton.UseVisualStyleBackColor = True
        '
        'BsShortAction
        '
        Me.BsShortAction.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsShortAction.Location = New Point(588, 320)
        Me.BsShortAction.Name = "BsShortAction"
        Me.BsShortAction.Size = New Size(100, 23)
        Me.BsShortAction.TabIndex = 12
        Me.BsShortAction.Text = "Short Action"
        Me.BsShortAction.UseVisualStyleBackColor = True
        '
        'BsTextWrite
        '
        Me.BsTextWrite.BackColor = Color.White
        Me.BsTextWrite.DecimalsValues = False
        Me.BsTextWrite.Font = New Font("Verdana", 8.25!)
        Me.BsTextWrite.IsNumeric = False
        Me.BsTextWrite.Location = New Point(555, 591)
        Me.BsTextWrite.Mandatory = False
        Me.BsTextWrite.Multiline = True
        Me.BsTextWrite.Name = "BsTextWrite"
        Me.BsTextWrite.Size = New Size(387, 24)
        Me.BsTextWrite.TabIndex = 11
        Me.BsTextWrite.Text = "A400;STATUS;S:1;A:54;T:60;C:0;W:0;R:1;E:0;I:1;"
        '
        'BsButton1
        '
        Me.BsButton1.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton1.Location = New Point(12, 12)
        Me.BsButton1.Name = "BsButton1"
        Me.BsButton1.Size = New Size(64, 28)
        Me.BsButton1.TabIndex = 0
        Me.BsButton1.Text = "BsButton1"
        Me.BsButton1.UseVisualStyleBackColor = True
        '
        'FwBlockTextBox
        '
        Me.FwBlockTextBox.Location = New Point(639, 459)
        Me.FwBlockTextBox.Name = "FwBlockTextBox"
        Me.FwBlockTextBox.Size = New Size(74, 21)
        Me.FwBlockTextBox.TabIndex = 97
        Me.FwBlockTextBox.Text = "0"
        '
        'FormXavi
        '
        Me.Appearance.BackColor = Color.Gainsboro
        Me.Appearance.Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleDimensions = New SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.ClientSize = New Size(1362, 765)
        Me.Controls.Add(Me.BsGroupBox1)
        Me.Controls.Add(Me.BsButton1)
        Me.LookAndFeel.SkinName = "Black"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "FormXavi"
        Me.Text = "FormXavi"
        Me.BsGroupBox1.ResumeLayout(False)
        Me.BsGroupBox1.PerformLayout()
        Me.BsGroupBox2.ResumeLayout(False)
        Me.BsGroupBox2.PerformLayout()
        CType(Me.DataGridView1, ISupportInitialize).EndInit()
        CType(Me.BsDataGridView1, ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BsButton1 As BSButton
    Friend WithEvents BsTextWrite As BSTextBox
    Friend WithEvents BsShortAction As BSButton
    Friend WithEvents BsReceptionButton As BSButton
    Friend WithEvents BsExecuteCalc As BSButton
    Friend WithEvents BsExecution As BSTextBox
    Friend WithEvents BsButton2 As BSButton
    Friend WithEvents BsRemainingTime As BSButton
    Friend WithEvents BsTextBoxToSend As TextBox
    Friend WithEvents BsSendPrep As BSButton
    Friend WithEvents BsLabel1 As BSLabel
    Friend WithEvents BsButton3 As BSButton
    Friend WithEvents BsDataGridView1 As BSDataGridView
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents BsGroupBox2 As BSGroupBox
    Friend WithEvents BsAction As BSTextBox
    Friend WithEvents BsRestore As BSButton
    Friend WithEvents BsInstruction As BSTextBox
    Friend WithEvents BsClearReception As BSButton
    Friend WithEvents BsLabel2 As BSLabel
    Friend WithEvents BsCommTestings As BSButton
    Friend WithEvents BsGroupBox1 As BSGroupBox
    Friend WithEvents BsTextWrite_End As BSTextBox
    Friend WithEvents BsReceptionButton_End As BSButton
    Friend WithEvents BsListBox1 As BSListBox
    Friend WithEvents Button1 As Button
    Friend WithEvents AdjustmentsTextBox As TextBox
    Friend WithEvents BsLabel3 As BSLabel
    Friend WithEvents BsButton4 As BSButton
    Friend WithEvents BsTextWrite_Error As BSTextBox
    Friend WithEvents BsReceptionButton_Error As BSButton
    Friend WithEvents Btn_ANSCMD_ERR As BSButton
    Friend WithEvents Btn_ANSCMD_OK As BSButton
    Friend WithEvents BsLabel4 As BSLabel
    Friend WithEvents BsButton7 As BSButton
    Friend WithEvents BsButton6 As BSButton
    Friend WithEvents BsButton5 As BSButton
    Friend WithEvents BsLabel8 As BSLabel
    Friend WithEvents BsLabel7 As BSLabel
    Friend WithEvents BsLabel6 As BSLabel
    Friend WithEvents BsLabel5 As BSLabel
    Friend WithEvents BsButton11 As BSButton
    Friend WithEvents BsButton10 As BSButton
    Friend WithEvents BsButton9 As BSButton
    Friend WithEvents BsButton8 As BSButton
    Friend WithEvents BsButton12 As BSButton
    Friend WithEvents BsLabel9 As BSLabel
    Friend WithEvents BsLabel10 As BSLabel
    Friend WithEvents Button2 As Button
    Friend WithEvents BsButton13 As BSButton
    Friend WithEvents BsLabel11 As BSLabel
    Friend WithEvents BsButton14 As BSButton
    Friend WithEvents Button3 As Button
    Friend WithEvents BsButton15 As BSButton
    Friend WithEvents BsLabel13 As BSLabel
    Friend WithEvents BsButton16 As BSButton
    Friend WithEvents BsLabel14 As BSLabel
    Friend WithEvents BsLabel15 As BSLabel
    Friend WithEvents BsButton17 As BSButton
    Friend WithEvents BsLabel19 As BSLabel
    Friend WithEvents BsLabel18 As BSLabel
    Friend WithEvents BsLabel17 As BSLabel
    Friend WithEvents BsLabel16 As BSLabel
    Friend WithEvents TextBox5 As TextBox
    Friend WithEvents BsReceivedTextBox As RichTextBox
    Friend WithEvents BsLabel20 As BSLabel
    Friend WithEvents BsButton18 As BSButton
    Friend WithEvents BsANSINFOCheckbox As BSCheckbox
    Friend WithEvents BsANSADJCheckbox As BSCheckbox
    Friend WithEvents BsTextWrite_Sleeping As BSTextBox
    Friend WithEvents BsReceptionButton_Sleeping As BSButton
    Friend WithEvents Button4 As Button
    Friend WithEvents BsLabel12 As BSLabel
    Friend WithEvents Button5 As Button
    Friend WithEvents BsLabel24 As BSLabel
    Friend WithEvents BsLabel23 As BSLabel
    Friend WithEvents BsLabel22 As BSLabel
    Friend WithEvents BsLabel21 As BSLabel
    Friend WithEvents FwBlockTextBox As TextBox
End Class
