<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormAddSubscription
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
        txtService = New TextBox()
        txtEmail = New TextBox()
        cboType = New ComboBox()
        dtBilling = New DateTimePicker()
        numDeadlineDays = New NumericUpDown()
        btnSave = New Button()
        lblService = New Label()
        lblEmail = New Label()
        lblBillType = New Label()
        lblBilling = New Label()
        lblDeadlineDays = New Label()
        lblAddSubscription = New Label()
        txtFee = New TextBox()
        lblFee = New Label()
        chkPin = New CheckBox()
        numReminderDays = New NumericUpDown()
        lblReminderDays = New Label()
        lblDaysBeforeDeadline = New Label()
        lblReminderNotification = New Label()
        chkUseBankingDays = New CheckBox()
        CType(numDeadlineDays, ComponentModel.ISupportInitialize).BeginInit()
        CType(numReminderDays, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' txtService
        ' 
        txtService.Font = New Font("Segoe UI", 11F)
        txtService.Location = New Point(30, 95)
        txtService.Name = "txtService"
        txtService.Size = New Size(320, 27)
        txtService.TabIndex = 0
        ' 
        ' txtEmail
        ' 
        txtEmail.Font = New Font("Segoe UI", 11F)
        txtEmail.Location = New Point(30, 160)
        txtEmail.Name = "txtEmail"
        txtEmail.Size = New Size(320, 27)
        txtEmail.TabIndex = 1
        ' 
        ' cboType
        ' 
        cboType.DropDownStyle = ComboBoxStyle.DropDownList
        cboType.Font = New Font("Segoe UI", 11F)
        cboType.Items.AddRange(New Object() {"Monthly", "Yearly"})
        cboType.Location = New Point(380, 95)
        cboType.Name = "cboType"
        cboType.Size = New Size(145, 28)
        cboType.TabIndex = 3
        ' 
        ' dtBilling
        ' 
        dtBilling.Font = New Font("Segoe UI", 11F)
        dtBilling.Format = DateTimePickerFormat.Short
        dtBilling.Location = New Point(540, 95)
        dtBilling.Name = "dtBilling"
        dtBilling.Size = New Size(145, 27)
        dtBilling.TabIndex = 4
        ' 
        ' numDeadlineDays
        ' 
        numDeadlineDays.Font = New Font("Segoe UI", 11F)
        numDeadlineDays.Location = New Point(380, 160)
        numDeadlineDays.Maximum = New Decimal(New Integer() {60, 0, 0, 0})
        numDeadlineDays.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        numDeadlineDays.Name = "numDeadlineDays"
        numDeadlineDays.Size = New Size(80, 27)
        numDeadlineDays.TabIndex = 5
        numDeadlineDays.Value = New Decimal(New Integer() {10, 0, 0, 0})
        ' 
        ' btnSave
        ' 
        btnSave.BackColor = Color.FromArgb(CByte(52), CByte(152), CByte(219))
        btnSave.FlatAppearance.BorderSize = 0
        btnSave.FlatStyle = FlatStyle.Flat
        btnSave.Font = New Font("Segoe UI Semibold", 12F, FontStyle.Bold)
        btnSave.ForeColor = Color.White
        btnSave.Location = New Point(260, 320)
        btnSave.Name = "btnSave"
        btnSave.Size = New Size(200, 45)
        btnSave.TabIndex = 8
        btnSave.Text = "Add Subscription"
        btnSave.UseVisualStyleBackColor = False
        ' 
        ' lblService
        ' 
        lblService.AutoSize = True
        lblService.Font = New Font("Segoe UI Semibold", 9.75F, FontStyle.Bold)
        lblService.Location = New Point(30, 75)
        lblService.Name = "lblService"
        lblService.Size = New Size(130, 17)
        lblService.TabIndex = 100
        lblService.Text = "Subscription Service"
        ' 
        ' lblEmail
        ' 
        lblEmail.AutoSize = True
        lblEmail.Font = New Font("Segoe UI Semibold", 9.75F, FontStyle.Bold)
        lblEmail.Location = New Point(30, 140)
        lblEmail.Name = "lblEmail"
        lblEmail.Size = New Size(40, 17)
        lblEmail.TabIndex = 101
        lblEmail.Text = "Email"
        ' 
        ' lblBillType
        ' 
        lblBillType.AutoSize = True
        lblBillType.Font = New Font("Segoe UI Semibold", 9.75F, FontStyle.Bold)
        lblBillType.Location = New Point(380, 75)
        lblBillType.Name = "lblBillType"
        lblBillType.Size = New Size(76, 17)
        lblBillType.TabIndex = 103
        lblBillType.Text = "Billing Type"
        ' 
        ' lblBilling
        ' 
        lblBilling.AutoSize = True
        lblBilling.Font = New Font("Segoe UI Semibold", 9.75F, FontStyle.Bold)
        lblBilling.Location = New Point(540, 75)
        lblBilling.Name = "lblBilling"
        lblBilling.Size = New Size(76, 17)
        lblBilling.TabIndex = 104
        lblBilling.Text = "Billing Date"
        ' 
        ' lblDeadlineDays
        ' 
        lblDeadlineDays.AutoSize = True
        lblDeadlineDays.Font = New Font("Segoe UI Semibold", 9.75F, FontStyle.Bold)
        lblDeadlineDays.Location = New Point(380, 140)
        lblDeadlineDays.Name = "lblDeadlineDays"
        lblDeadlineDays.Size = New Size(60, 17)
        lblDeadlineDays.TabIndex = 105
        lblDeadlineDays.Text = "Deadline"
        ' 
        ' lblAddSubscription
        ' 
        lblAddSubscription.AutoSize = True
        lblAddSubscription.Font = New Font("Segoe UI Semibold", 18F, FontStyle.Bold)
        lblAddSubscription.ForeColor = Color.FromArgb(CByte(44), CByte(62), CByte(80))
        lblAddSubscription.Location = New Point(30, 20)
        lblAddSubscription.Name = "lblAddSubscription"
        lblAddSubscription.Size = New Size(204, 32)
        lblAddSubscription.TabIndex = 106
        lblAddSubscription.Text = "New Subscription"
        ' 
        ' txtFee
        ' 
        txtFee.Font = New Font("Segoe UI", 11F)
        txtFee.Location = New Point(30, 225)
        txtFee.Name = "txtFee"
        txtFee.Size = New Size(320, 27)
        txtFee.TabIndex = 6
        ' 
        ' lblFee
        ' 
        lblFee.AutoSize = True
        lblFee.Font = New Font("Segoe UI Semibold", 9.75F, FontStyle.Bold)
        lblFee.Location = New Point(30, 205)
        lblFee.Name = "lblFee"
        lblFee.Size = New Size(87, 17)
        lblFee.TabIndex = 107
        lblFee.Text = "Payment Fee"
        ' 
        ' chkPin
        ' 
        chkPin.Location = New Point(0, 0)
        chkPin.Name = "chkPin"
        chkPin.Size = New Size(104, 24)
        chkPin.TabIndex = 108
        ' 
        ' numReminderDays
        ' 
        numReminderDays.Font = New Font("Segoe UI", 11F)
        numReminderDays.Location = New Point(380, 245)
        numReminderDays.Maximum = New Decimal(New Integer() {30, 0, 0, 0})
        numReminderDays.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        numReminderDays.Name = "numReminderDays"
        numReminderDays.Size = New Size(80, 27)
        numReminderDays.TabIndex = 7
        numReminderDays.Value = New Decimal(New Integer() {3, 0, 0, 0})
        ' 
        ' lblReminderDays
        ' 
        lblReminderDays.AutoSize = True
        lblReminderDays.Font = New Font("Segoe UI", 9F)
        lblReminderDays.ForeColor = Color.Gray
        lblReminderDays.Location = New Point(465, 166)
        lblReminderDays.Name = "lblReminderDays"
        lblReminderDays.Size = New Size(94, 15)
        lblReminderDays.TabIndex = 109
        lblReminderDays.Text = "days after Billing"
        ' 
        ' lblDaysBeforeDeadline
        ' 
        lblDaysBeforeDeadline.AutoSize = True
        lblDaysBeforeDeadline.Font = New Font("Segoe UI", 9F)
        lblDaysBeforeDeadline.ForeColor = Color.Gray
        lblDaysBeforeDeadline.Location = New Point(465, 251)
        lblDaysBeforeDeadline.Name = "lblDaysBeforeDeadline"
        lblDaysBeforeDeadline.Size = New Size(117, 15)
        lblDaysBeforeDeadline.TabIndex = 110
        lblDaysBeforeDeadline.Text = "days before Deadline"
        ' 
        ' lblReminderNotification
        ' 
        lblReminderNotification.AutoSize = True
        lblReminderNotification.Font = New Font("Segoe UI Semibold", 9.75F, FontStyle.Bold)
        lblReminderNotification.Location = New Point(380, 225)
        lblReminderNotification.Name = "lblReminderNotification"
        lblReminderNotification.Size = New Size(66, 17)
        lblReminderNotification.TabIndex = 111
        lblReminderNotification.Text = "Reminder"
        ' 
        ' chkUseBankingDays
        ' 
        chkUseBankingDays.AutoSize = True
        chkUseBankingDays.Checked = True
        chkUseBankingDays.CheckState = CheckState.Checked
        chkUseBankingDays.Font = New Font("Segoe UI", 9F)
        chkUseBankingDays.Location = New Point(380, 192)
        chkUseBankingDays.Name = "chkUseBankingDays"
        chkUseBankingDays.Size = New Size(214, 19)
        chkUseBankingDays.TabIndex = 20
        chkUseBankingDays.Text = "Use Banking Days (Skips Weekends)"
        chkUseBankingDays.UseVisualStyleBackColor = True
        ' 
        ' FormAddSubscription
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.White
        ClientSize = New Size(718, 380)
        Controls.Add(lblDaysBeforeDeadline)
        Controls.Add(lblReminderDays)
        Controls.Add(numReminderDays)
        Controls.Add(lblReminderNotification)
        Controls.Add(chkUseBankingDays)
        Controls.Add(lblFee)
        Controls.Add(txtFee)
        Controls.Add(lblAddSubscription)
        Controls.Add(lblDeadlineDays)
        Controls.Add(lblBilling)
        Controls.Add(lblBillType)
        Controls.Add(lblEmail)
        Controls.Add(lblService)
        Controls.Add(btnSave)
        Controls.Add(numDeadlineDays)
        Controls.Add(dtBilling)
        Controls.Add(cboType)
        Controls.Add(txtEmail)
        Controls.Add(txtService)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "FormAddSubscription"
        StartPosition = FormStartPosition.CenterParent
        Text = "Subscription Details"
        CType(numDeadlineDays, ComponentModel.ISupportInitialize).EndInit()
        CType(numReminderDays, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents txtService As TextBox
    Friend WithEvents txtEmail As TextBox
    Friend WithEvents cboType As ComboBox
    Friend WithEvents dtBilling As DateTimePicker
    Friend WithEvents numDeadlineDays As NumericUpDown
    Friend WithEvents btnSave As Button
    Friend WithEvents lblService As Label
    Friend WithEvents lblEmail As Label
    Friend WithEvents lblBillType As Label
    Friend WithEvents lblBilling As Label
    Friend WithEvents lblDeadlineDays As Label
    Friend WithEvents lblAddSubscription As Label
    Friend WithEvents txtFee As TextBox
    Friend WithEvents lblFee As Label
    Friend WithEvents chkPin As CheckBox
    Friend WithEvents numReminderDays As NumericUpDown
    Friend WithEvents lblReminderDays As Label
    Friend WithEvents lblDaysBeforeDeadline As Label
    Friend WithEvents lblReminderNotification As Label
    Friend WithEvents chkUseBankingDays As CheckBox
End Class
