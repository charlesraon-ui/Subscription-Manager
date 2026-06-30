<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        dgvSubscriptions = New DataGridView()
        Pause = New DataGridViewButtonColumn()
        btnRefresh = New Button()
        btnAddSubscription = New Button()
        btnNotifications = New Button()
        btnCalendar = New Button()
        timerCheck = New Timer(components)
        txtSearch = New TextBox()
        lblSearch = New Label()
        PanelTotal = New Panel()
        lblTotalCount = New Label()
        lblTotalText = New Label()
        PanelDueToday = New Panel()
        lblDueTodayCount = New Label()
        lblDueTodayText = New Label()
        PanelOverdue = New Panel()
        lblOverdueCount = New Label()
        lblOverdueText = New Label()
        PanelDueSoon = New Panel()
        lblDueSoonCount = New Label()
        lblDueSoonText = New Label()
        timerRefresh = New Timer(components)
        panelTop = New Panel()
        lblPanelTitle = New Label()
        cmsSubscription = New ContextMenuStrip(components)
        EditToolStripMenuItem = New ToolStripMenuItem()
        DeleteToolStripMenuItem = New ToolStripMenuItem()
        lblStatusFilter = New Label()
        cboStatusFilter = New ComboBox()
        lblBillingTypeFilter = New Label()
        cboBillingTypeFilter = New ComboBox()
        CType(dgvSubscriptions, ComponentModel.ISupportInitialize).BeginInit()
        PanelTotal.SuspendLayout()
        PanelDueToday.SuspendLayout()
        PanelOverdue.SuspendLayout()
        PanelDueSoon.SuspendLayout()
        panelTop.SuspendLayout()
        cmsSubscription.SuspendLayout()
        SuspendLayout()
        ' 
        ' dgvSubscriptions
        ' 
        dgvSubscriptions.AllowUserToAddRows = False
        dgvSubscriptions.AllowUserToResizeColumns = False
        dgvSubscriptions.AllowUserToResizeRows = False
        dgvSubscriptions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvSubscriptions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvSubscriptions.Location = New Point(12, 140)
        dgvSubscriptions.Name = "dgvSubscriptions"
        dgvSubscriptions.ReadOnly = True
        dgvSubscriptions.Size = New Size(1048, 480)
        dgvSubscriptions.TabIndex = 0
        ' 
        ' Pause
        ' 
        Pause.HeaderText = "Status"
        Pause.Name = "Pause"
        Pause.ReadOnly = True
        Pause.UseColumnTextForButtonValue = True
        ' 
        ' btnRefresh
        ' 
        btnRefresh.FlatAppearance.BorderSize = 0
        btnRefresh.FlatStyle = FlatStyle.System
        btnRefresh.Location = New Point(432, 63)
        btnRefresh.Name = "btnRefresh"
        btnRefresh.Size = New Size(101, 35)
        btnRefresh.TabIndex = 1
        btnRefresh.Text = "Refresh"
        btnRefresh.UseVisualStyleBackColor = True
        ' 
        ' btnAddSubscription
        ' 
        btnAddSubscription.Location = New Point(882, 95)
        btnAddSubscription.Name = "btnAddSubscription"
        btnAddSubscription.Size = New Size(178, 39)
        btnAddSubscription.TabIndex = 3
        btnAddSubscription.Text = "Add Subscriptions"
        btnAddSubscription.UseVisualStyleBackColor = True
        ' 
        ' btnNotifications
        ' 
        btnNotifications.BackColor = SystemColors.Info
        btnNotifications.FlatAppearance.BorderSize = 0
        btnNotifications.Location = New Point(706, 63)
        btnNotifications.Name = "btnNotifications"
        btnNotifications.Size = New Size(90, 35)
        btnNotifications.TabIndex = 10
        btnNotifications.Text = "🔔"
        btnNotifications.UseVisualStyleBackColor = False
        ' 
        ' btnCalendar
        ' 
        btnCalendar.Location = New Point(542, 63)
        btnCalendar.Name = "btnCalendar"
        btnCalendar.Size = New Size(155, 35)
        btnCalendar.TabIndex = 11
        btnCalendar.Text = "📅 Calendar View"
        btnCalendar.UseVisualStyleBackColor = True
        ' 
        ' timerCheck
        ' 
        timerCheck.Enabled = True
        timerCheck.Interval = 60000
        ' 
        ' txtSearch
        ' 
        txtSearch.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        txtSearch.Location = New Point(70, 66)
        txtSearch.Name = "txtSearch"
        txtSearch.Size = New Size(353, 29)
        txtSearch.TabIndex = 4
        ' 
        ' lblSearch
        ' 
        lblSearch.AutoSize = True
        lblSearch.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        lblSearch.Location = New Point(12, 70)
        lblSearch.Name = "lblSearch"
        lblSearch.Size = New Size(53, 20)
        lblSearch.TabIndex = 5
        lblSearch.Text = "Search"
        ' 
        ' PanelTotal
        ' 
        PanelTotal.BackColor = Color.FromArgb(CByte(52), CByte(152), CByte(219))
        PanelTotal.Controls.Add(lblTotalCount)
        PanelTotal.Controls.Add(lblTotalText)
        PanelTotal.Location = New Point(80, 631)
        PanelTotal.Name = "PanelTotal"
        PanelTotal.Size = New Size(180, 80)
        PanelTotal.TabIndex = 6
        ' 
        ' lblTotalCount
        ' 
        lblTotalCount.Dock = DockStyle.Top
        lblTotalCount.Font = New Font("Segoe UI", 22F, FontStyle.Bold)
        lblTotalCount.ForeColor = Color.White
        lblTotalCount.Location = New Point(0, 0)
        lblTotalCount.Name = "lblTotalCount"
        lblTotalCount.Size = New Size(180, 45)
        lblTotalCount.TabIndex = 0
        lblTotalCount.Text = "0"
        lblTotalCount.TextAlign = ContentAlignment.BottomCenter
        ' 
        ' lblTotalText
        ' 
        lblTotalText.Dock = DockStyle.Bottom
        lblTotalText.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        lblTotalText.ForeColor = Color.White
        lblTotalText.Location = New Point(0, 45)
        lblTotalText.Name = "lblTotalText"
        lblTotalText.Size = New Size(180, 35)
        lblTotalText.TabIndex = 1
        lblTotalText.Text = "Total Subscriptions"
        lblTotalText.TextAlign = ContentAlignment.TopCenter
        ' 
        ' PanelDueToday
        ' 
        PanelDueToday.BackColor = Color.FromArgb(CByte(241), CByte(196), CByte(15))
        PanelDueToday.Controls.Add(lblDueTodayCount)
        PanelDueToday.Controls.Add(lblDueTodayText)
        PanelDueToday.Location = New Point(320, 631)
        PanelDueToday.Name = "PanelDueToday"
        PanelDueToday.Size = New Size(180, 80)
        PanelDueToday.TabIndex = 7
        ' 
        ' lblDueTodayCount
        ' 
        lblDueTodayCount.Dock = DockStyle.Top
        lblDueTodayCount.Font = New Font("Segoe UI", 22F, FontStyle.Bold)
        lblDueTodayCount.ForeColor = Color.White
        lblDueTodayCount.Location = New Point(0, 0)
        lblDueTodayCount.Name = "lblDueTodayCount"
        lblDueTodayCount.Size = New Size(180, 45)
        lblDueTodayCount.TabIndex = 0
        lblDueTodayCount.Text = "0"
        lblDueTodayCount.TextAlign = ContentAlignment.BottomCenter
        ' 
        ' lblDueTodayText
        ' 
        lblDueTodayText.Dock = DockStyle.Bottom
        lblDueTodayText.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        lblDueTodayText.ForeColor = Color.White
        lblDueTodayText.Location = New Point(0, 45)
        lblDueTodayText.Name = "lblDueTodayText"
        lblDueTodayText.Size = New Size(180, 35)
        lblDueTodayText.TabIndex = 1
        lblDueTodayText.Text = "Due Today"
        lblDueTodayText.TextAlign = ContentAlignment.TopCenter
        ' 
        ' PanelOverdue
        ' 
        PanelOverdue.BackColor = Color.FromArgb(CByte(231), CByte(76), CByte(60))
        PanelOverdue.Controls.Add(lblOverdueCount)
        PanelOverdue.Controls.Add(lblOverdueText)
        PanelOverdue.Location = New Point(558, 631)
        PanelOverdue.Name = "PanelOverdue"
        PanelOverdue.Size = New Size(180, 80)
        PanelOverdue.TabIndex = 8
        ' 
        ' lblOverdueCount
        ' 
        lblOverdueCount.Dock = DockStyle.Top
        lblOverdueCount.Font = New Font("Segoe UI", 22F, FontStyle.Bold)
        lblOverdueCount.ForeColor = Color.White
        lblOverdueCount.Location = New Point(0, 0)
        lblOverdueCount.Name = "lblOverdueCount"
        lblOverdueCount.Size = New Size(180, 45)
        lblOverdueCount.TabIndex = 0
        lblOverdueCount.Text = "0"
        lblOverdueCount.TextAlign = ContentAlignment.BottomCenter
        ' 
        ' lblOverdueText
        ' 
        lblOverdueText.Dock = DockStyle.Bottom
        lblOverdueText.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        lblOverdueText.ForeColor = Color.White
        lblOverdueText.Location = New Point(0, 45)
        lblOverdueText.Name = "lblOverdueText"
        lblOverdueText.Size = New Size(180, 35)
        lblOverdueText.TabIndex = 1
        lblOverdueText.Text = "Overdue"
        lblOverdueText.TextAlign = ContentAlignment.TopCenter
        ' 
        ' PanelDueSoon
        ' 
        PanelDueSoon.BackColor = Color.FromArgb(CByte(46), CByte(204), CByte(113))
        PanelDueSoon.Controls.Add(lblDueSoonCount)
        PanelDueSoon.Controls.Add(lblDueSoonText)
        PanelDueSoon.Location = New Point(800, 631)
        PanelDueSoon.Name = "PanelDueSoon"
        PanelDueSoon.Size = New Size(180, 80)
        PanelDueSoon.TabIndex = 9
        ' 
        ' lblDueSoonCount
        ' 
        lblDueSoonCount.Dock = DockStyle.Top
        lblDueSoonCount.Font = New Font("Segoe UI", 22F, FontStyle.Bold)
        lblDueSoonCount.ForeColor = Color.White
        lblDueSoonCount.Location = New Point(0, 0)
        lblDueSoonCount.Name = "lblDueSoonCount"
        lblDueSoonCount.Size = New Size(180, 45)
        lblDueSoonCount.TabIndex = 0
        lblDueSoonCount.Text = "0"
        lblDueSoonCount.TextAlign = ContentAlignment.BottomCenter
        ' 
        ' lblDueSoonText
        ' 
        lblDueSoonText.Dock = DockStyle.Bottom
        lblDueSoonText.Font = New Font("Segoe UI", 10F, FontStyle.Bold)
        lblDueSoonText.ForeColor = Color.White
        lblDueSoonText.Location = New Point(0, 45)
        lblDueSoonText.Name = "lblDueSoonText"
        lblDueSoonText.Size = New Size(180, 35)
        lblDueSoonText.TabIndex = 1
        lblDueSoonText.Text = "Due Soon"
        lblDueSoonText.TextAlign = ContentAlignment.TopCenter
        ' 
        ' timerRefresh
        ' 
        timerRefresh.Enabled = True
        timerRefresh.Interval = 10000
        ' 
        ' panelTop
        ' 
        panelTop.BackColor = Color.DarkBlue
        panelTop.Controls.Add(lblPanelTitle)
        panelTop.Dock = DockStyle.Top
        panelTop.Location = New Point(0, 0)
        panelTop.Name = "panelTop"
        panelTop.Size = New Size(1072, 55)
        panelTop.TabIndex = 12
        ' 
        ' lblPanelTitle
        ' 
        lblPanelTitle.Dock = DockStyle.Fill
        lblPanelTitle.Font = New Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        lblPanelTitle.ForeColor = SystemColors.ControlLightLight
        lblPanelTitle.Location = New Point(0, 0)
        lblPanelTitle.Name = "lblPanelTitle"
        lblPanelTitle.Size = New Size(1072, 55)
        lblPanelTitle.TabIndex = 0
        lblPanelTitle.Text = "SUBSCRIPTION MANAGER"
        lblPanelTitle.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' cmsSubscription
        ' 
        cmsSubscription.Items.AddRange(New ToolStripItem() {EditToolStripMenuItem, DeleteToolStripMenuItem})
        cmsSubscription.Name = "cmsSubscription"
        cmsSubscription.Size = New Size(108, 48)
        ' 
        ' EditToolStripMenuItem
        ' 
        EditToolStripMenuItem.Name = "EditToolStripMenuItem"
        EditToolStripMenuItem.Size = New Size(107, 22)
        EditToolStripMenuItem.Text = "Edit"
        ' 
        ' DeleteToolStripMenuItem
        ' 
        DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem"
        DeleteToolStripMenuItem.Size = New Size(107, 22)
        DeleteToolStripMenuItem.Text = "Delete"
        ' 
        ' lblStatusFilter
        ' 
        lblStatusFilter.AutoSize = True
        lblStatusFilter.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        lblStatusFilter.Location = New Point(12, 110)
        lblStatusFilter.Name = "lblStatusFilter"
        lblStatusFilter.Size = New Size(46, 17)
        lblStatusFilter.TabIndex = 13
        lblStatusFilter.Text = "Status:"
        ' 
        ' cboStatusFilter
        ' 
        cboStatusFilter.DropDownStyle = ComboBoxStyle.DropDownList
        cboStatusFilter.FlatStyle = FlatStyle.Flat
        cboStatusFilter.Font = New Font("Segoe UI", 10.0F)
        cboStatusFilter.Location = New Point(62, 107)
        cboStatusFilter.Name = "cboStatusFilter"
        cboStatusFilter.Size = New Size(130, 25)
        cboStatusFilter.TabIndex = 14
        ' 
        ' lblBillingTypeFilter
        ' 
        lblBillingTypeFilter.AutoSize = True
        lblBillingTypeFilter.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        lblBillingTypeFilter.Location = New Point(210, 110)
        lblBillingTypeFilter.Name = "lblBillingTypeFilter"
        lblBillingTypeFilter.Size = New Size(76, 17)
        lblBillingTypeFilter.TabIndex = 15
        lblBillingTypeFilter.Text = "Billing Type:"
        ' 
        ' cboBillingTypeFilter
        ' 
        cboBillingTypeFilter.DropDownStyle = ComboBoxStyle.DropDownList
        cboBillingTypeFilter.FlatStyle = FlatStyle.Flat
        cboBillingTypeFilter.Font = New Font("Segoe UI", 10.0F)
        cboBillingTypeFilter.Location = New Point(293, 107)
        cboBillingTypeFilter.Name = "cboBillingTypeFilter"
        cboBillingTypeFilter.Size = New Size(130, 25)
        cboBillingTypeFilter.TabIndex = 16
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1072, 724)
        Controls.Add(cboBillingTypeFilter)
        Controls.Add(lblBillingTypeFilter)
        Controls.Add(cboStatusFilter)
        Controls.Add(lblStatusFilter)
        Controls.Add(panelTop)
        Controls.Add(PanelDueSoon)
        Controls.Add(PanelOverdue)
        Controls.Add(PanelDueToday)
        Controls.Add(PanelTotal)
        Controls.Add(lblSearch)
        Controls.Add(txtSearch)
        Controls.Add(btnAddSubscription)
        Controls.Add(btnNotifications)
        Controls.Add(btnCalendar)
        Controls.Add(btnRefresh)
        Controls.Add(dgvSubscriptions)
        FormBorderStyle = FormBorderStyle.FixedSingle
        MaximizeBox = False
        Name = "Form1"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Subscription Manager"
        CType(dgvSubscriptions, ComponentModel.ISupportInitialize).EndInit()
        PanelTotal.ResumeLayout(False)
        PanelDueToday.ResumeLayout(False)
        PanelOverdue.ResumeLayout(False)
        PanelDueSoon.ResumeLayout(False)
        panelTop.ResumeLayout(False)
        panelTop.PerformLayout()
        cmsSubscription.ResumeLayout(False)
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents dgvSubscriptions As DataGridView
    Friend WithEvents btnRefresh As Button
    Friend WithEvents btnAddSubscription As Button
    Friend WithEvents btnNotifications As Button
    Friend WithEvents btnCalendar As Button
    Friend WithEvents Pause As DataGridViewButtonColumn
    Friend WithEvents timerCheck As Timer
    Friend WithEvents txtSearch As TextBox
    Friend WithEvents lblSearch As Label
    Friend WithEvents PanelTotal As Panel
    Friend WithEvents lblTotalCount As Label
    Friend WithEvents lblTotalText As Label
    Friend WithEvents PanelDueToday As Panel
    Friend WithEvents lblDueTodayCount As Label
    Friend WithEvents lblDueTodayText As Label
    Friend WithEvents PanelOverdue As Panel
    Friend WithEvents lblOverdueCount As Label
    Friend WithEvents lblOverdueText As Label
    Friend WithEvents PanelDueSoon As Panel
    Friend WithEvents lblDueSoonCount As Label
    Friend WithEvents lblDueSoonText As Label
    Friend WithEvents timerRefresh As Timer
    Friend WithEvents panelTop As Panel
    Friend WithEvents lblPanelTitle As Label
    Friend WithEvents cmsSubscription As ContextMenuStrip
    Friend WithEvents EditToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DeleteToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents lblStatusFilter As Label
    Friend WithEvents cboStatusFilter As ComboBox
    Friend WithEvents lblBillingTypeFilter As Label
    Friend WithEvents cboBillingTypeFilter As ComboBox

End Class
