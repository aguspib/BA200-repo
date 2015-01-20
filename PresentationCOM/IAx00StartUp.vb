Public Class IAx00StartUp
    Inherits Windows.Forms.Form

#Region "Declaration"

    Private WithEvents lbTextoPreload As System.Windows.Forms.Label
    Friend WithEvents lbTitle As System.Windows.Forms.Label
    Private WithEvents picAjax As System.Windows.Forms.PictureBox
    Friend WithEvents BsTimer1 As Biosystems.Ax00.Controls.UserControls.BSTimer
    Friend WithEvents pathBackground As String

#End Region

#Region "Properties"

    'RH 22/11/2010
    Public WriteOnly Property Title() As String
        Set(ByVal value As String)
            lbTitle.Text = value
            lbTitle.Invalidate()
        End Set
    End Property

    'RH 22/11/2010
    Public WriteOnly Property WaitText() As String
        Set(ByVal value As String)
            lbTextoPreload.Text = value
            lbTextoPreload.Invalidate()
        End Set
    End Property

    'DL 17/04/2012
    Public WriteOnly Property Background() As String
        Set(ByVal value As String)
            If Not value Is String.Empty Then
                Me.BackgroundImage = System.Drawing.Image.FromFile(value)
            End If

        End Set
    End Property

#End Region

#Region "Constructor"

    ''' <summary>
    ''' Constructor that receive a form as a parameter for setting the preload form config.
    ''' </summary>
    ''' <remarks>
    ''' Modified by: RH 17/11/2010
    ''' </remarks>
    Public Sub New(ByVal FormCaller As Windows.Forms.Form)
        InitializeComponent()
        'If FormCaller IsNot Nothing Then InicializePreload(FormCaller)
    End Sub

#End Region

#Region "Eventos"

#End Region

#Region "Inicializadores de Componentes"

    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IAx00StartUp))
        Me.lbTextoPreload = New System.Windows.Forms.Label
        Me.lbTitle = New System.Windows.Forms.Label
        Me.BsTimer1 = New Biosystems.Ax00.Controls.UserControls.BSTimer
        Me.picAjax = New System.Windows.Forms.PictureBox
        CType(Me.picAjax, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lbTextoPreload
        '
        Me.lbTextoPreload.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lbTextoPreload.BackColor = System.Drawing.Color.Transparent
        Me.lbTextoPreload.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.lbTextoPreload.ForeColor = System.Drawing.Color.Black
        Me.lbTextoPreload.Location = New System.Drawing.Point(481, 359)
        Me.lbTextoPreload.Name = "lbTextoPreload"
        Me.lbTextoPreload.Size = New System.Drawing.Size(234, 19)
        Me.lbTextoPreload.TabIndex = 9
        Me.lbTextoPreload.Text = "Please wait..."
        Me.lbTextoPreload.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lbTitle
        '
        Me.lbTitle.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lbTitle.BackColor = System.Drawing.Color.Transparent
        Me.lbTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.lbTitle.ForeColor = System.Drawing.Color.Black
        Me.lbTitle.Location = New System.Drawing.Point(342, 139)
        Me.lbTitle.Name = "lbTitle"
        Me.lbTitle.Size = New System.Drawing.Size(344, 19)
        Me.lbTitle.TabIndex = 10
        Me.lbTitle.Text = "AX00 User Software"
        Me.lbTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BsTimer1
        '
        Me.BsTimer1.Enabled = True
        '
        'picAjax
        '
        Me.picAjax.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.picAjax.BackColor = System.Drawing.Color.Transparent
        Me.picAjax.Image = CType(resources.GetObject("picAjax.Image"), System.Drawing.Image)
        Me.picAjax.Location = New System.Drawing.Point(479, 173)
        Me.picAjax.Name = "picAjax"
        Me.picAjax.Size = New System.Drawing.Size(70, 70)
        Me.picAjax.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picAjax.TabIndex = 11
        Me.picAjax.TabStop = False
        '
        'IAx00StartUp
        '
        Me.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), System.Drawing.Image)
        Me.ClientSize = New System.Drawing.Size(718, 378)
        Me.ControlBox = False
        Me.Controls.Add(Me.picAjax)
        Me.Controls.Add(Me.lbTitle)
        Me.Controls.Add(Me.lbTextoPreload)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IAx00StartUp"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        CType(Me.picAjax, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "Metodos Privados"

    ''' <summary>
    ''' Metodo para obtener localizacion 
    ''' </summary>
    ''' <param name="_form"></param>
    ''' <remarks></remarks>
    Private Sub InicializePreload(ByVal _form As System.Windows.Forms.Form)
        'Supendemos el diseño
        SuspendLayout()

        'Size inicial del formpreload
        Dim mSizeInicialPreload As Drawing.Size = Size

        'Aplicamos el size y location del formulario ....
        Size = _form.Size
        Location = _form.Location

        lbTextoPreload.Width = Size.Width
        lbTitle.Width = Size.Width

        'Get the factor with this code 
        Dim factorEscalaX As Integer = CInt((_form.Size.Width - mSizeInicialPreload.Width) / 2)
        Dim factorEscalaY As Integer = CInt((_form.Size.Height - mSizeInicialPreload.Height) / 2)

        picAjax.Left = picAjax.Left + factorEscalaX
        picAjax.Top = picAjax.Top + factorEscalaY

        lbTextoPreload.Top = lbTextoPreload.Top + factorEscalaY
        lbTitle.Top = lbTitle.Top + factorEscalaY

        'Reiniciamos el diseño
        ResumeLayout()
    End Sub

    ''' <summary>
    ''' Refreshes the Loading image
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 27/05/2011
    ''' </remarks>
    Private Sub RefreshLoadingImage()
        picAjax.Refresh()
        System.Windows.Forms.Application.DoEvents()
    End Sub

    Private Sub BsTimer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsTimer1.Tick
        'Validate the form is visible.
        If Me.Visible Then
            RefreshLoadingImage()
        Else
            BsTimer1.Stop()
        End If

    End Sub

    Private Sub IAx00StartUp_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        'BsTimer1.Enabled = False
        BsTimer1.Stop()

    End Sub

#End Region

End Class
