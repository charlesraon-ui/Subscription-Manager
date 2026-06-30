<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CalendarView
    Inherits System.Windows.Forms.Form

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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        lblInfo = New Label()
        btnClose = New Button()
        panelHeader = New Panel()
        lblHeader = New Label()
        dgvDayPayments = New DataGridView()
        customCalendar1 = New CustomCalendar()
        panelHeader.SuspendLayout()
        CType(dgvDayPayments, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' lblInfo
        ' 
        lblInfo.AutoSize = True
        lblInfo.Font = New Font("Segoe UI Semibold", 12F, FontStyle.Bold)
        lblInfo.ForeColor = Color.FromArgb(CByte(44), CByte(62), CByte(80))
        lblInfo.Location = New Point(480, 75)
        lblInfo.Name = "lblInfo"
        lblInfo.Size = New Size(209, 21)
        lblInfo.TabIndex = 1
        lblInfo.Text = "Payments for selected date"
        ' 
        ' btnClose
        ' 
        btnClose.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnClose.BackColor = Color.FromArgb(CByte(149), CByte(165), CByte(166))
        btnClose.FlatStyle = FlatStyle.Flat
        btnClose.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        btnClose.ForeColor = Color.White
        btnClose.Location = New Point(730, 437)
        btnClose.Name = "btnClose"
        btnClose.Size = New Size(100, 35)
        btnClose.TabIndex = 2
        btnClose.Text = "CLOSE"
        btnClose.UseVisualStyleBackColor = False
        ' 
        ' panelHeader
        ' 
        panelHeader.BackColor = Color.FromArgb(CByte(52), CByte(152), CByte(219))
        panelHeader.Controls.Add(lblHeader)
        panelHeader.Dock = DockStyle.Top
        panelHeader.Location = New Point(0, 0)
        panelHeader.Name = "panelHeader"
        panelHeader.Size = New Size(850, 60)
        panelHeader.TabIndex = 3
        ' 
        ' lblHeader
        ' 
        lblHeader.AutoSize = True
        lblHeader.Font = New Font("Segoe UI", 18F, FontStyle.Bold)
        lblHeader.ForeColor = Color.White
        lblHeader.Location = New Point(20, 15)
        lblHeader.Name = "lblHeader"
        lblHeader.Size = New Size(209, 32)
        lblHeader.TabIndex = 0
        lblHeader.Text = "CALENDAR VIEW"
        ' 
        ' dgvDayPayments
        ' 
        dgvDayPayments.AllowUserToAddRows = False
        dgvDayPayments.AllowUserToDeleteRows = False
        dgvDayPayments.BackgroundColor = Color.White
        dgvDayPayments.BorderStyle = BorderStyle.None
        dgvDayPayments.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvDayPayments.Location = New Point(480, 105)
        dgvDayPayments.Name = "dgvDayPayments"
        dgvDayPayments.ReadOnly = True
        dgvDayPayments.RowHeadersVisible = False
        dgvDayPayments.Size = New Size(350, 280)
        dgvDayPayments.TabIndex = 4
        ' 
        ' customCalendar1
        ' 
        customCalendar1.BackColor = Color.White
        customCalendar1.Location = New Point(20, 70)
        customCalendar1.Name = "customCalendar1"
        customCalendar1.Size = New Size(400, 350)
        customCalendar1.TabIndex = 5
        ' 
        ' CalendarView
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.White
        ClientSize = New Size(850, 485)
        Controls.Add(customCalendar1)
        Controls.Add(dgvDayPayments)
        Controls.Add(panelHeader)
        Controls.Add(btnClose)
        Controls.Add(lblInfo)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "CalendarView"
        StartPosition = FormStartPosition.CenterParent
        Text = "Calendar View"
        panelHeader.ResumeLayout(False)
        panelHeader.PerformLayout()
        CType(dgvDayPayments, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents lblInfo As Label
    Friend WithEvents btnClose As Button
    Friend WithEvents panelHeader As Panel
    Friend WithEvents lblHeader As Label
    Friend WithEvents dgvDayPayments As DataGridView
    Friend WithEvents customCalendar1 As CustomCalendar
End Class
