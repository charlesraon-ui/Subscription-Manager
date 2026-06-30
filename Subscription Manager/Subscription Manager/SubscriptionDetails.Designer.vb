<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SubscriptionDetails
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
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
        lblTitle = New Label()
        lblServiceName = New Label()
        lblEmail = New Label()
        lblType = New Label()
        lblFee = New Label()
        lblNextBilling = New Label()
        lblDeadline = New Label()
        btnPaid = New Button()
        btnClose = New Button()
        PanelHeader = New Panel()
        btnHistory = New Button()
        PanelHeader.SuspendLayout()
        SuspendLayout()
        ' 
        ' lblTitle
        ' 
        lblTitle.AutoSize = True
        lblTitle.Font = New Font("Segoe UI", 18F, FontStyle.Bold)
        lblTitle.ForeColor = Color.White
        lblTitle.Location = New Point(20, 15)
        lblTitle.Name = "lblTitle"
        lblTitle.Size = New Size(243, 32)
        lblTitle.TabIndex = 0
        lblTitle.Text = "Subscription Details"
        ' 
        ' lblServiceName
        ' 
        lblServiceName.AutoSize = True
        lblServiceName.Font = New Font("Segoe UI Semibold", 12F, FontStyle.Bold)
        lblServiceName.Location = New Point(30, 85)
        lblServiceName.Name = "lblServiceName"
        lblServiceName.Size = New Size(72, 21)
        lblServiceName.TabIndex = 2
        lblServiceName.Text = "Service: "
        ' 
        ' lblEmail
        ' 
        lblEmail.AutoSize = True
        lblEmail.Font = New Font("Segoe UI Semibold", 12F, FontStyle.Bold)
        lblEmail.Location = New Point(30, 125)
        lblEmail.Name = "lblEmail"
        lblEmail.Size = New Size(56, 21)
        lblEmail.TabIndex = 3
        lblEmail.Text = "Email: "
        ' 
        ' lblType
        ' 
        lblType.AutoSize = True
        lblType.Font = New Font("Segoe UI Semibold", 12F, FontStyle.Bold)
        lblType.Location = New Point(30, 165)
        lblType.Name = "lblType"
        lblType.Size = New Size(53, 21)
        lblType.TabIndex = 4
        lblType.Text = "Type: "
        ' 
        ' lblFee
        ' 
        lblFee.AutoSize = True
        lblFee.Font = New Font("Segoe UI Semibold", 12F, FontStyle.Bold)
        lblFee.Location = New Point(30, 205)
        lblFee.Name = "lblFee"
        lblFee.Size = New Size(44, 21)
        lblFee.TabIndex = 5
        lblFee.Text = "Fee: "
        ' 
        ' lblNextBilling
        ' 
        lblNextBilling.AutoSize = True
        lblNextBilling.Font = New Font("Segoe UI Semibold", 12F, FontStyle.Bold)
        lblNextBilling.Location = New Point(30, 245)
        lblNextBilling.Name = "lblNextBilling"
        lblNextBilling.Size = New Size(102, 21)
        lblNextBilling.TabIndex = 6
        lblNextBilling.Text = "Next Billing: "
        ' 
        ' lblDeadline
        ' 
        lblDeadline.AutoSize = True
        lblDeadline.Font = New Font("Segoe UI Semibold", 12F, FontStyle.Bold)
        lblDeadline.Location = New Point(30, 285)
        lblDeadline.Name = "lblDeadline"
        lblDeadline.Size = New Size(82, 21)
        lblDeadline.TabIndex = 7
        lblDeadline.Text = "Deadline: "
        ' 
        ' btnHistory
        ' 
        btnHistory.BackColor = Color.CornflowerBlue
        btnHistory.FlatStyle = FlatStyle.Flat
        btnHistory.Font = New Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        btnHistory.ForeColor = Color.White
        btnHistory.Location = New Point(109, 320)
        btnHistory.Name = "btnHistory"
        btnHistory.Size = New Size(179, 27)
        btnHistory.TabIndex = 10
        btnHistory.Text = "VIEW PAYMENT HISTORY"
        btnHistory.UseVisualStyleBackColor = False
        ' 
        ' btnPaid
        ' 
        btnPaid.BackColor = Color.FromArgb(CByte(46), CByte(204), CByte(113))
        btnPaid.FlatStyle = FlatStyle.Flat
        btnPaid.Font = New Font("Segoe UI", 11F, FontStyle.Bold)
        btnPaid.ForeColor = Color.White
        btnPaid.Location = New Point(30, 360)
        btnPaid.Name = "btnPaid"
        btnPaid.Size = New Size(140, 45)
        btnPaid.TabIndex = 8
        btnPaid.Text = "MARK AS PAID"
        btnPaid.UseVisualStyleBackColor = False
        ' 
        ' btnClose
        ' 
        btnClose.BackColor = Color.FromArgb(CByte(149), CByte(165), CByte(166))
        btnClose.FlatStyle = FlatStyle.Flat
        btnClose.Font = New Font("Segoe UI", 11F, FontStyle.Bold)
        btnClose.ForeColor = Color.White
        btnClose.Location = New Point(230, 360)
        btnClose.Name = "btnClose"
        btnClose.Size = New Size(140, 45)
        btnClose.TabIndex = 9
        btnClose.Text = "CLOSE"
        btnClose.UseVisualStyleBackColor = False
        ' 
        ' PanelHeader
        ' 
        PanelHeader.BackColor = Color.FromArgb(CByte(44), CByte(62), CByte(80))
        PanelHeader.Controls.Add(lblTitle)
        PanelHeader.Dock = DockStyle.Top
        PanelHeader.Location = New Point(0, 0)
        PanelHeader.Name = "PanelHeader"
        PanelHeader.Size = New Size(400, 65)
        PanelHeader.TabIndex = 100
        ' 
        ' SubscriptionDetails
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.White
        ClientSize = New Size(400, 440)
        Controls.Add(btnHistory)
        Controls.Add(PanelHeader)
        Controls.Add(btnClose)
        Controls.Add(btnPaid)
        Controls.Add(lblDeadline)
        Controls.Add(lblNextBilling)
        Controls.Add(lblFee)
        Controls.Add(lblType)
        Controls.Add(lblEmail)
        Controls.Add(lblServiceName)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "SubscriptionDetails"
        StartPosition = FormStartPosition.CenterParent
        Text = "Subscription Details"
        PanelHeader.ResumeLayout(False)
        PanelHeader.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents lblTitle As Label
    Friend WithEvents lblServiceName As Label
    Friend WithEvents lblEmail As Label
    Friend WithEvents lblType As Label
    Friend WithEvents lblFee As Label
    Friend WithEvents lblNextBilling As Label
    Friend WithEvents lblDeadline As Label
    Friend WithEvents btnPaid As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents PanelHeader As Panel
    Friend WithEvents btnHistory As Button
End Class
