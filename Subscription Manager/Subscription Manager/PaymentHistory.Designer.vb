<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PaymentHistory
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        lblTitle = New Label()
        dgvHistory = New DataGridView()
        btnClose = New Button()
        CType(dgvHistory, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' lblTitle
        ' 
        lblTitle.AutoSize = True
        lblTitle.Font = New Font("Segoe UI", 14F, FontStyle.Bold)
        lblTitle.Location = New Point(12, 10)
        lblTitle.Name = "lblTitle"
        lblTitle.Size = New Size(161, 25)
        lblTitle.TabIndex = 0
        lblTitle.Text = "Payment History"
        ' 
        ' dgvHistory
        ' 
        dgvHistory.AllowUserToAddRows = False
        dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvHistory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvHistory.Location = New Point(12, 50)
        dgvHistory.Name = "dgvHistory"
        dgvHistory.ReadOnly = True
        dgvHistory.Size = New Size(560, 300)
        dgvHistory.TabIndex = 1
        ' 
        ' btnClose
        ' 
        btnClose.Location = New Point(497, 360)
        btnClose.Name = "btnClose"
        btnClose.Size = New Size(75, 30)
        btnClose.TabIndex = 2
        btnClose.Text = "Close"
        btnClose.UseVisualStyleBackColor = True
        ' 
        ' PaymentHistory
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(584, 400)
        Controls.Add(btnClose)
        Controls.Add(dgvHistory)
        Controls.Add(lblTitle)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "PaymentHistory"
        StartPosition = FormStartPosition.CenterParent
        Text = "Payment History"
        CType(dgvHistory, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents lblTitle As Label
    Friend WithEvents dgvHistory As DataGridView
    Friend WithEvents btnClose As Button
End Class