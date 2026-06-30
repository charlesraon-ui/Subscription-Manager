Imports System.Net
Imports System.Net.Mail
Imports MySql.Data.MySqlClient

Public Class Form1

    ' ========================== CONFIGURATION ==========================
    Private ReadOnly connString As String = "server=localhost;userid=root;password=;database=subscription_db;"
    Private dtSubscriptions As DataTable
    Private isSearching As Boolean = False
    Private hoveredRowIndex As Integer = -1

    Private Const OVERDUE_INTERVAL_DAYS As Integer = 3
    Private Const DEADLINE_OFFSET_DAYS As Integer = 10

    Private currentStatusFilter As String = "All"
    Private currentBillingTypeFilter As String = "All"

    Private lblNoData As Label

    ' ========================== FORM LOAD ==========================
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.BackgroundImage = Nothing
        Me.BackColor = Color.FromArgb(245, 247, 250)

        ' Apply modern styling to dashboard cards
        StyleDashboardCards()

        lblNoData = New Label()
        lblNoData.AutoSize = False
        lblNoData.Size = New Size(400, 40)
        lblNoData.TextAlign = ContentAlignment.MiddleCenter
        lblNoData.Font = New Font("Segoe UI", 10, FontStyle.Regular)
        lblNoData.ForeColor = Color.Gray
        lblNoData.BackColor = Color.Transparent
        lblNoData.Parent = dgvSubscriptions
        lblNoData.Visible = False
        lblNoData.BringToFront()

        CleanupDeadlines()
        RefreshData()

        SetupDataGridView()
        SetupFilterControls()
        StyleButtons()
        InitializeDatabase()
        RefreshData()
        ConfigureColumnWidths()
        CheckNotifications()
        UpdateNotificationBadge()
    End Sub

    ' ========================== MODERN UI STYLING ==========================
    Public Async Function ShowToast(message As String, bgColor As Color) As Task
        Dim toast As New Panel()
        toast.BackColor = bgColor
        toast.Size = New Size(240, 40)

        ' Position at the bottom right of the subscriptions list
        Dim gridRight = dgvSubscriptions.Right
        Dim gridBottom = dgvSubscriptions.Bottom
        toast.Location = New Point(gridRight - toast.Width - 10, gridBottom - toast.Height - 10)

        Dim lbl As New Label()
        lbl.Text = "✔ " & message
        lbl.ForeColor = Color.White
        lbl.Font = New Font("Segoe UI Semibold", 9)
        lbl.Dock = DockStyle.Fill
        lbl.TextAlign = ContentAlignment.MiddleCenter
        toast.Controls.Add(lbl)

        Me.Controls.Add(toast)
        toast.BringToFront()

        ' Fade in (simple)
        toast.Hide()
        toast.Show()

        Await Task.Delay(3000) ' Visible for 3 seconds

        ' Fade out / Remove
        Me.Controls.Remove(toast)
    End Function

    Private Sub StyleDashboardCards()
        ' Define modern colors and card properties
        Dim cardPanels = {PanelTotal, PanelDueToday, PanelOverdue, PanelDueSoon}
        
        ' Modern Accent Colors (Strong)
        Dim accentColors = {
            Color.FromArgb(52, 152, 219),  ' Blue
            Color.FromArgb(241, 196, 15),  ' Yellow
            Color.FromArgb(231, 76, 60),   ' Red
            Color.FromArgb(46, 204, 113)   ' Green
        }

        ' Modern Soft Pastel Backgrounds (Matching Grid)
        Dim softBackgrounds = {
            Color.FromArgb(235, 245, 251), ' Soft Blue
            Color.FromArgb(255, 253, 231), ' Soft Yellow
            Color.FromArgb(255, 235, 238), ' Soft Red
            Color.FromArgb(232, 245, 233)  ' Soft Green
        }

        For i As Integer = 0 To cardPanels.Length - 1
            Dim pnl = cardPanels(i)
            Dim accent = accentColors(i)
            Dim bg = softBackgrounds(i)
            
            pnl.BackColor = bg
            pnl.BorderStyle = BorderStyle.None
            
            ' Add a colored accent bar at the top and a subtle border
            AddHandler pnl.Paint, Sub(sender, e)
                                     Dim p = DirectCast(sender, Panel)
                                     ' Subtle bottom shadow-like border
                                     ControlPaint.DrawBorder(e.Graphics, p.ClientRectangle,
                                         Color.Transparent, 0, ButtonBorderStyle.None,
                                         Color.Transparent, 0, ButtonBorderStyle.None,
                                         Color.Transparent, 0, ButtonBorderStyle.None,
                                         Color.FromArgb(30, 0, 0, 0), 1, ButtonBorderStyle.Solid)
                                     
                                     ' Colored accent bar at the top (4px height)
                                     Using brush As New SolidBrush(accent)
                                         e.Graphics.FillRectangle(brush, 0, 0, p.Width, 4)
                                     End Using
                                 End Sub
        Next

        ' Style the labels inside the cards for clean look against soft colors
        lblTotalCount.ForeColor = Color.FromArgb(44, 62, 80)
        lblDueTodayCount.ForeColor = Color.FromArgb(44, 62, 80)
        lblOverdueCount.ForeColor = Color.FromArgb(44, 62, 80)
        lblDueSoonCount.ForeColor = Color.FromArgb(44, 62, 80)

        ' Darker versions of the accents for text readability
        lblTotalText.ForeColor = Color.FromArgb(41, 128, 185)
        lblDueTodayText.ForeColor = Color.FromArgb(184, 134, 11)
        lblOverdueText.ForeColor = Color.FromArgb(192, 57, 43)
        lblDueSoonText.ForeColor = Color.FromArgb(39, 174, 96)
    End Sub

    ' ========================== UI SETUP ==========================
    Private Sub SetupDataGridView()
        With dgvSubscriptions
            .BorderStyle = BorderStyle.None
            .BackgroundColor = Color.White
            .RowHeadersVisible = False
            .AllowUserToResizeRows = False
            .AllowUserToResizeColumns = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
            .Font = New Font("Segoe UI", 10.0F)

            .DefaultCellStyle.BackColor = Color.White
            .DefaultCellStyle.ForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 230, 250)
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.Font = New Font("Segoe UI", 10.0F)

            ' Alternating colors (Light gray row) for better readability
            .AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250)
            .AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 230, 250)
            .AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.Black
            .AlternatingRowsDefaultCellStyle.Font = New Font("Segoe UI", 10.0F)

            ' Header styling (Top row of the grid)
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(242, 242, 242)
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41)
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(242, 242, 242)
            .ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(33, 37, 41)
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI Semibold", 10.0F)
            .ColumnHeadersHeight = 45 ' Increased height
            .RowTemplate.Height = 45 ' Increased row spacing
            .EnableHeadersVisualStyles = False
        End With
    End Sub

    Private Sub SetupFilterControls()
        ' --- Status Filter ComboBox ---
        cboStatusFilter.Items.Clear()
        cboStatusFilter.Items.AddRange({"All", "Paid", "Overdue", "Due Today", "Due Soon", "Favorites", "Paused"})
        cboStatusFilter.SelectedIndex = 0
        cboStatusFilter.DropDownStyle = ComboBoxStyle.DropDownList
        cboStatusFilter.FlatStyle = FlatStyle.Flat
        cboStatusFilter.Font = New Font("Segoe UI", 10.0F)

        ' --- Billing Type Filter ComboBox ---
        cboBillingTypeFilter.Items.Clear()
        cboBillingTypeFilter.Items.AddRange({"All", "Monthly", "Yearly"})
        cboBillingTypeFilter.SelectedIndex = 0
        cboBillingTypeFilter.DropDownStyle = ComboBoxStyle.DropDownList
        cboBillingTypeFilter.FlatStyle = FlatStyle.Flat
        cboBillingTypeFilter.Font = New Font("Segoe UI", 10.0F)
    End Sub


    Private Sub StyleButtons()
        Dim allButtons() As Button = {btnRefresh, btnNotifications, btnCalendar, btnAddSubscription}

        For Each btn In allButtons
            btn.FlatStyle = FlatStyle.Flat
            btn.FlatAppearance.BorderSize = 1
            btn.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular)
            btn.Cursor = Cursors.Hand
            btn.Height = 36
            btn.Padding = New Padding(10, 4, 10, 4)
        Next

        ' Specific styling for Refresh button
        With btnRefresh
            .BackColor = Color.White
            .ForeColor = Color.FromArgb(44, 62, 80)
            .FlatAppearance.BorderColor = Color.FromArgb(189, 195, 199)
            .Text = "⟳ Refresh"
        End With

        ' Specific styling for Calendar button
        With btnCalendar
            .BackColor = Color.White
            .ForeColor = Color.FromArgb(44, 62, 80)
            .FlatAppearance.BorderColor = Color.FromArgb(189, 195, 199)
            .Text = "📅 Calendar View"
        End With

        ' Specific styling for Add Subscription button (Highlighted)
        With btnAddSubscription
            .BackColor = Color.FromArgb(44, 62, 80)
            .ForeColor = Color.White
            .FlatAppearance.BorderColor = Color.FromArgb(44, 62, 80)
            .FlatAppearance.MouseOverBackColor = Color.FromArgb(52, 73, 94)
            .FlatAppearance.MouseDownBackColor = Color.FromArgb(33, 47, 60)
            .Text = "＋ Add Subscription"
            .Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        End With

        ' Notifications button styling
        With btnNotifications
            .ForeColor = Color.Black
            .FlatAppearance.BorderColor = Color.FromArgb(189, 195, 199)
        End With
    End Sub

    Private Sub ConfigureColumnWidths()
        dgvSubscriptions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        If dgvSubscriptions.Columns.Contains("Actions") Then
            With dgvSubscriptions.Columns("Actions")
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                .Width = 30
            End With
        End If

        If dgvSubscriptions.Columns.Contains("ServiceName") Then
            dgvSubscriptions.Columns("ServiceName").FillWeight = 40
        End If

        If dgvSubscriptions.Columns.Contains("SubscriptionFee") Then
            dgvSubscriptions.Columns("SubscriptionFee").FillWeight = 15
        End If

        If dgvSubscriptions.Columns.Contains("NextBillingDate") Then
            dgvSubscriptions.Columns("NextBillingDate").FillWeight = 20
        End If

        If dgvSubscriptions.Columns.Contains("PaymentDeadline") Then
            dgvSubscriptions.Columns("PaymentDeadline").FillWeight = 20
        End If
    End Sub

    Private Sub UpdateNotificationBadge()
        Dim overdueCount As Integer = 0
        Dim dueTodayCount As Integer = 0
        Dim dueSoonCount As Integer = 0

        Using conn As New MySqlConnection(connString)
            conn.Open()

            ' Count overdue subscriptions
            Dim cmdOverdue As New MySqlCommand(
                "SELECT COUNT(*) FROM Subscription WHERE DATE(PaymentDeadline) < CURDATE() AND IsActive = TRUE", conn)
            overdueCount = Convert.ToInt32(cmdOverdue.ExecuteScalar())

            ' Count subscriptions due today
            Dim cmdToday As New MySqlCommand(
                "SELECT COUNT(*) FROM Subscription WHERE DATE(PaymentDeadline) = CURDATE() AND IsActive = TRUE", conn)
            dueTodayCount = Convert.ToInt32(cmdToday.ExecuteScalar())

            ' Count subscriptions due soon (within reminder period)
            Dim cmdSoon As New MySqlCommand(
                "SELECT COUNT(*) FROM Subscription WHERE DATE(PaymentDeadline) > CURDATE() AND DATE(PaymentDeadline) <= DATE_ADD(CURDATE(), INTERVAL ReminderDays DAY) AND IsActive = TRUE", conn)
            dueSoonCount = Convert.ToInt32(cmdSoon.ExecuteScalar())
        End Using

        Dim totalNotifications As Integer = overdueCount + dueTodayCount + dueSoonCount

        ' Update button text with count
        If totalNotifications > 0 Then
            btnNotifications.Text = "🔔 " & totalNotifications.ToString()
        Else
            btnNotifications.Text = "🔔"
        End If

        ' Color-code the button based on the most severe status
        If overdueCount > 0 Then
            btnNotifications.BackColor = Color.FromArgb(231, 76, 60) ' Red
            btnNotifications.ForeColor = Color.White
        ElseIf dueTodayCount > 0 Then
            btnNotifications.BackColor = Color.FromArgb(241, 196, 15) ' Yellow
            btnNotifications.ForeColor = Color.Black
        ElseIf dueSoonCount > 0 Then
            btnNotifications.BackColor = Color.FromArgb(46, 204, 113) ' Green
            btnNotifications.ForeColor = Color.White
        Else
            btnNotifications.BackColor = Color.White
            btnNotifications.ForeColor = Color.Black
        End If
    End Sub


    ' ========================== DATABASE INITIALIZATION ==========================
    Private Sub InitializeDatabase()
        Using conn As New MySqlConnection(connString)
            conn.Open()

            ' Create PaymentHistory table if it doesn't exist
            Dim cmdPH As New MySqlCommand("
                CREATE TABLE IF NOT EXISTS PaymentHistory (
                    PaymentID INT AUTO_INCREMENT PRIMARY KEY,
                    SubscriptionID INT,
                    BillingCycleStart DATETIME,
                    DueDate DATETIME,
                    PaymentDate DATETIME NULL,
                    Amount DECIMAL(10,2),
                    Status VARCHAR(50),
                    FOREIGN KEY (SubscriptionID) REFERENCES Subscription(SubscriptionID)
                );", conn)
            cmdPH.ExecuteNonQuery()

            ' Update PaymentHistory schema if it's old
            Try
                Dim cmdAddCycleStart As New MySqlCommand("ALTER TABLE PaymentHistory ADD COLUMN BillingCycleStart DATETIME AFTER SubscriptionID;", conn)
                cmdAddCycleStart.ExecuteNonQuery()
            Catch ex As MySqlException
                If ex.Number <> 1060 Then Throw
            End Try

            Try
                Dim cmdAddDueDate As New MySqlCommand("ALTER TABLE PaymentHistory ADD COLUMN DueDate DATETIME AFTER BillingCycleStart;", conn)
                cmdAddDueDate.ExecuteNonQuery()
            Catch ex As MySqlException
                If ex.Number <> 1060 Then Throw
            End Try

            Try
                Dim cmdModPaymentDate As New MySqlCommand("ALTER TABLE PaymentHistory MODIFY COLUMN PaymentDate DATETIME NULL;", conn)
                cmdModPaymentDate.ExecuteNonQuery()
            Catch ex As MySqlException
                ' Handle potential errors if needed
            End Try

            ' Backfill NULL DueDate and BillingCycleStart for legacy records
            Try
                ' If DueDate is null, use PaymentDate. If both are null, use current date.
                Dim cmdBackfillDue As New MySqlCommand("
                    UPDATE PaymentHistory 
                    SET DueDate = COALESCE(PaymentDate, CURDATE()) 
                    WHERE DueDate IS NULL;", conn)
                cmdBackfillDue.ExecuteNonQuery()

                ' If BillingCycleStart is null, use DueDate
                Dim cmdBackfillCycle As New MySqlCommand("
                    UPDATE PaymentHistory 
                    SET BillingCycleStart = DueDate 
                    WHERE BillingCycleStart IS NULL;", conn)
                cmdBackfillCycle.ExecuteNonQuery()

                ' --- SYNC EXISTING ACTIVE SUBSCRIPTIONS ---
                ' Ensure every active subscription has its NEXT payment record pre-generated
                ' regardless of whether the billing date has started yet.
                Dim dtActiveSubs As New DataTable()
                Dim cmdGetActive As New MySqlCommand("
                    SELECT SubscriptionID, NextBillingDate, PaymentDeadline, SubscriptionFee 
                    FROM Subscription 
                    WHERE IsActive = TRUE", conn)
                Dim daActive As New MySqlDataAdapter(cmdGetActive)
                daActive.Fill(dtActiveSubs)

                For Each row As DataRow In dtActiveSubs.Rows
                    Dim subID As Integer = Convert.ToInt32(row("SubscriptionID"))
                    Dim billingDate As Date = Convert.ToDateTime(row("NextBillingDate")).Date
                    Dim deadlineDate As Date = Convert.ToDateTime(row("PaymentDeadline")).Date
                    Dim fee As Decimal = Convert.ToDecimal(row("SubscriptionFee"))

                    ' Check if a record exists for THIS SPECIFIC billing cycle
                    Dim cmdCheckCycle As New MySqlCommand("
                        SELECT COUNT(*) FROM PaymentHistory 
                        WHERE SubscriptionID = @sid AND DATE(BillingCycleStart) = DATE(@start)", conn)
                    cmdCheckCycle.Parameters.AddWithValue("@sid", subID)
                    cmdCheckCycle.Parameters.AddWithValue("@start", billingDate)

                    Dim cycleRecordCount As Integer = Convert.ToInt32(cmdCheckCycle.ExecuteScalar())

                    ' ALWAYS create the record if it doesn't exist for the current cycle, 
                    ' even if the billing date is in the future.
                    If cycleRecordCount = 0 Then
                        Dim cmdInsert As New MySqlCommand("
                            INSERT INTO PaymentHistory (SubscriptionID, BillingCycleStart, DueDate, Amount, Status)
                            VALUES (@sid, @start, @deadline, @amount, 'Not Yet Paid')", conn)
                        cmdInsert.Parameters.AddWithValue("@sid", subID)
                        cmdInsert.Parameters.AddWithValue("@start", billingDate)
                        cmdInsert.Parameters.AddWithValue("@deadline", deadlineDate)
                        cmdInsert.Parameters.AddWithValue("@amount", fee)
                        cmdInsert.ExecuteNonQuery()
                    End If
                Next
            Catch ex As Exception
                Debug.WriteLine("Backfill/Sync error: " & ex.Message)
            End Try

            ' Create Notification table if it doesn't exist
            Dim cmdNotif As New MySqlCommand("
                CREATE TABLE IF NOT EXISTS Notification (
                    NotificationID INT AUTO_INCREMENT PRIMARY KEY,
                    SubscriptionID INT,
                    Type VARCHAR(50),
                    DateSent DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (SubscriptionID) REFERENCES Subscription(SubscriptionID)
                );", conn)
            cmdNotif.ExecuteNonQuery()

            Try
                Dim cmdFee As New MySqlCommand("ALTER TABLE Subscription ADD COLUMN SubscriptionFee DECIMAL(10,2) DEFAULT 0.00;", conn)
                cmdFee.ExecuteNonQuery()

                Dim cmdMock As New MySqlCommand("UPDATE Subscription SET SubscriptionFee = 9.99 WHERE SubscriptionFee = 0.00;", conn)
                cmdMock.ExecuteNonQuery()
            Catch ex As MySqlException
                If ex.Number <> 1060 Then Throw
            End Try

            Try
                Dim cmdDeadlineDays As New MySqlCommand("ALTER TABLE Subscription ADD COLUMN PaymentDeadlineDays INT DEFAULT 10;", conn)
                cmdDeadlineDays.ExecuteNonQuery()
            Catch ex As MySqlException
                If ex.Number <> 1060 Then Throw
            End Try

            Try
                Dim cmdUseBankingDays As New MySqlCommand("ALTER TABLE Subscription ADD COLUMN UseBankingDays BOOLEAN DEFAULT TRUE;", conn)
                cmdUseBankingDays.ExecuteNonQuery()
            Catch ex As MySqlException
                If ex.Number <> 1060 Then Throw
            End Try
        End Using
    End Sub

    ' Syncs deadlines based on the billing date and banking day rules
    Private Sub CleanupDeadlines()
        Try
            Using conn As New MySqlConnection(connString)
                conn.Open()
                Dim dtSubs As New DataTable()
                Dim cmdSelect As New MySqlCommand("SELECT SubscriptionID, NextBillingDate, PaymentDeadline, PaymentDeadlineDays, UseBankingDays FROM Subscription", conn)
                Dim da As New MySqlDataAdapter(cmdSelect)
                da.Fill(dtSubs)

                Dim updatesMade As Integer = 0
                For Each row As DataRow In dtSubs.Rows
                    If IsDBNull(row("NextBillingDate")) OrElse IsDBNull(row("PaymentDeadline")) Then Continue For

                    Dim subID As Integer = Convert.ToInt32(row("SubscriptionID"))
                    Dim billingDate As Date = Convert.ToDateTime(row("NextBillingDate")).Date
                    Dim currentDeadline As Date = Convert.ToDateTime(row("PaymentDeadline")).Date
                    Dim deadlineDays As Integer = Convert.ToInt32(If(IsDBNull(row("PaymentDeadlineDays")), 10, row("PaymentDeadlineDays")))
                    Dim useBankingDays As Boolean = If(IsDBNull(row("UseBankingDays")), True, Convert.ToBoolean(row("UseBankingDays")))

                    ' Calculate what the deadline SHOULD be
                    Dim correctDeadline As Date
                    If useBankingDays Then
                        correctDeadline = BusinessLogic.AddBankingDays(billingDate, deadlineDays)
                    Else
                        correctDeadline = billingDate.AddDays(deadlineDays)
                    End If

                    ' If the stored deadline is wrong, update it
                    If correctDeadline.Date <> currentDeadline.Date Then
                        Dim updateCmd As New MySqlCommand("UPDATE Subscription SET PaymentDeadline = @deadline WHERE SubscriptionID = @id", conn)
                        updateCmd.Parameters.AddWithValue("@deadline", correctDeadline)
                        updateCmd.Parameters.AddWithValue("@id", subID)
                        updateCmd.ExecuteNonQuery()

                        ' Improved: Also sync the PaymentHistory record for this cycle
                        Dim updatePHCmd As New MySqlCommand("
                            UPDATE PaymentHistory 
                            SET DueDate = @deadline 
                            WHERE SubscriptionID = @id AND Status <> 'Paid'
                            ORDER BY DueDate DESC LIMIT 1", conn)
                        updatePHCmd.Parameters.AddWithValue("@deadline", correctDeadline)
                        updatePHCmd.Parameters.AddWithValue("@id", subID)
                        updatePHCmd.ExecuteNonQuery()

                        updatesMade += 1
                    End If
                Next

                If updatesMade > 0 Then
                    Debug.WriteLine($"Cleaned up {updatesMade} subscription deadlines to follow banking day rules.")
                End If
            End Using
        Catch ex As Exception
            Debug.WriteLine("Deadline cleanup error: " & ex.Message)
        End Try
    End Sub

    ' ========================== DATA ACCESS ==========================
    Public Function GetSubscriptions() As DataTable
        Dim dt As New DataTable()

        Using conn As New MySqlConnection(connString)
            conn.Open()

            ' Query with priority: Pinned > Overdue > Due Today > Due Soon > Other
            ' Improved: Fetching the latest payment status for the current cycle (hidden from UI)
            ' Using DATE(s.NextBillingDate) for robust comparison with PaymentHistory
            Dim cmd As New MySqlCommand("
                SELECT s.SubscriptionID, s.ServiceName,
                       s.NextBillingDate, s.PaymentDeadline,
                       s.BillingType, s.SubscriptionFee, u.Email,
                       s.IsPinned, s.ReminderDays, s.IsActive,
                       s.PaymentDeadlineDays,
                       (SELECT Status FROM PaymentHistory 
                        WHERE SubscriptionID = s.SubscriptionID 
                        AND DATE(BillingCycleStart) = DATE(s.NextBillingDate) 
                        ORDER BY PaymentID DESC LIMIT 1) as CurrentCycleStatus
                FROM Subscription s
                JOIN User u ON s.UserID = u.UserID
                ORDER BY 
                    s.IsPinned DESC, 
                    CASE 
                        WHEN s.PaymentDeadline < CURDATE() THEN 1 
                        WHEN s.PaymentDeadline = CURDATE() THEN 2 
                        WHEN s.PaymentDeadline > CURDATE() AND DATEDIFF(s.PaymentDeadline, CURDATE()) <= s.ReminderDays THEN 3 
                        ELSE 4 
                    END ASC,
                    s.NextBillingDate ASC", conn)

            Dim da As New MySqlDataAdapter(cmd)
            da.Fill(dt)
        End Using

        dt.Columns.Add("IsDueSoon", GetType(Boolean))
        dt.Columns.Add("IsOverdue", GetType(Boolean))
        dt.Columns.Add("IsDueToday", GetType(Boolean))
        dt.Columns.Add("IsPaid", GetType(Boolean))

        ' Calculate the status of each row
        Dim today As Date = Date.Now.Date
        For Each row As DataRow In dt.Rows
            If IsDBNull(row("PaymentDeadline")) OrElse IsDBNull(row("NextBillingDate")) Then Continue For

            Dim deadlineDate As Date = Convert.ToDateTime(row("PaymentDeadline")).Date
            Dim billingDate As Date = Convert.ToDateTime(row("NextBillingDate")).Date
            Dim reminderDays As Integer = Convert.ToInt32(row("ReminderDays"))

            Dim isOverdue As Boolean = (deadlineDate < today)
            Dim isDueToday As Boolean = (deadlineDate = today)

            Dim daysUntilDeadline As Integer = (deadlineDate - today).Days
            Dim isDueSoon As Boolean = (deadlineDate > today AndAlso daysUntilDeadline <= reminderDays)

            ' A subscription is considered "Paid" for the main list if:
            ' 1. The current cycle is explicitly marked as 'Paid'
            ' 2. OR the billing date is in the future (Upcoming)
            Dim cycleStatus As String = If(IsDBNull(row("CurrentCycleStatus")), "", row("CurrentCycleStatus").ToString())
            Dim isPaid As Boolean = (cycleStatus = "Paid") OrElse (billingDate > today AndAlso Not isDueSoon)

            ' If it's already paid or upcoming, it's not overdue or due today
            If isPaid Then
                isOverdue = False
                isDueToday = False
            End If

            row("IsOverdue") = isOverdue
            row("IsDueToday") = isDueToday
            row("IsDueSoon") = isDueSoon
            row("IsPaid") = isPaid
        Next

        Return dt
    End Function

    ' Checks if a notification for a specific sub and type was already sent today
    Public Function NotificationAlreadySent(subID As Integer, type As String) As Boolean
        Using conn As New MySqlConnection(connString)
            conn.Open()

            Dim cmd As New MySqlCommand("
                SELECT COUNT(*) 
                FROM Notification 
                WHERE SubscriptionID = @sid 
                AND Type = @type 
                AND DATE(DateSent) = CURDATE()", conn)

            cmd.Parameters.AddWithValue("@sid", subID)
            cmd.Parameters.AddWithValue("@type", type)

            Return Convert.ToInt32(cmd.ExecuteScalar()) > 0
        End Using
    End Function

    Public Sub SaveNotification(subID As Integer, type As String)
        Using conn As New MySqlConnection(connString)
            conn.Open()

            Dim cmd As New MySqlCommand("
                INSERT INTO Notification (SubscriptionID, Type, DateSent)
                VALUES (@sid, @type, NOW())", conn)

            cmd.Parameters.AddWithValue("@sid", subID)
            cmd.Parameters.AddWithValue("@type", type)

            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ' ========================== BILLING OPERATIONS ==========================
    Public Sub MarkAsPaid(subID As Integer, billingDate As Date, billingType As String)
        ' Calculate the next billing date based on cycle (Monthly/Yearly)
        Dim newBillingDate As Date = If(billingType = "Monthly",
            billingDate.AddMonths(1),
            billingDate.AddYears(1))

        Dim deadlineDays As Integer = 10
        Dim useBankingDays As Boolean = True
        Dim currentDeadline As Date

        Using conn As New MySqlConnection(connString)
            conn.Open()
            ' Get the subscription's deadline settings
            Dim cmdGetSettings As New MySqlCommand("SELECT PaymentDeadline, PaymentDeadlineDays, UseBankingDays FROM Subscription WHERE SubscriptionID=@id", conn)
            cmdGetSettings.Parameters.AddWithValue("@id", subID)
            Dim reader = cmdGetSettings.ExecuteReader()
            If reader.Read() Then
                currentDeadline = Convert.ToDateTime(reader("PaymentDeadline"))
                If Not IsDBNull(reader("PaymentDeadlineDays")) Then deadlineDays = Convert.ToInt32(reader("PaymentDeadlineDays"))
                If Not IsDBNull(reader("UseBankingDays")) Then useBankingDays = Convert.ToBoolean(reader("UseBankingDays"))
            End If
            reader.Close()

            ' Calculate the new payment deadline
            Dim newDeadline As Date
            If useBankingDays Then
                newDeadline = BusinessLogic.AddBankingDays(newBillingDate, deadlineDays)
            Else
                newDeadline = newBillingDate.AddDays(deadlineDays)
            End If

            ' 1. Update the existing record for the current cycle to "Paid"
            Dim cmdUpdatePH As New MySqlCommand("
                UPDATE PaymentHistory 
                SET Status = 'Paid', PaymentDate = NOW() 
                WHERE SubscriptionID = @sid AND Status <> 'Paid'
                ORDER BY ABS(DATEDIFF(DueDate, @due)) ASC LIMIT 1", conn)
            cmdUpdatePH.Parameters.AddWithValue("@sid", subID)
            cmdUpdatePH.Parameters.AddWithValue("@due", currentDeadline)
            Dim rowsAffected = cmdUpdatePH.ExecuteNonQuery()

            ' Log the payment in history (insert if no existing record found)
            Dim cmdFee As New MySqlCommand("SELECT SubscriptionFee FROM Subscription WHERE SubscriptionID=@id", conn)
            cmdFee.Parameters.AddWithValue("@id", subID)
            Dim fee = Convert.ToDecimal(cmdFee.ExecuteScalar())

            If rowsAffected = 0 Then
                Dim cmdPH As New MySqlCommand("
                    INSERT INTO PaymentHistory (SubscriptionID, DueDate, PaymentDate, Amount, Status)
                    VALUES (@sid, @due, NOW(), @amount, 'Paid')", conn)
                cmdPH.Parameters.AddWithValue("@sid", subID)
                cmdPH.Parameters.AddWithValue("@due", currentDeadline)
                cmdPH.Parameters.AddWithValue("@amount", fee)
                cmdPH.ExecuteNonQuery()
            End If

            ' 2. Update the subscription record
            Dim cmd As New MySqlCommand("
                UPDATE Subscription 
                SET NextBillingDate=@bill, PaymentDeadline=@deadline 
                WHERE SubscriptionID=@id", conn)

            cmd.Parameters.AddWithValue("@bill", newBillingDate)
            cmd.Parameters.AddWithValue("@deadline", newDeadline)
            cmd.Parameters.AddWithValue("@id", subID)
            cmd.ExecuteNonQuery()

            ' 3. Generate the NEXT cycle's "Not Yet Paid" record
            Dim cmdNextPH As New MySqlCommand("
                INSERT INTO PaymentHistory (SubscriptionID, BillingCycleStart, DueDate, Amount, Status)
                VALUES (@sid, @start, @due, @amount, 'Not Yet Paid')", conn)
            cmdNextPH.Parameters.AddWithValue("@sid", subID)
            cmdNextPH.Parameters.AddWithValue("@start", newBillingDate)
            cmdNextPH.Parameters.AddWithValue("@due", newDeadline)
            cmdNextPH.Parameters.AddWithValue("@amount", fee)
            cmdNextPH.ExecuteNonQuery()
        End Using
    End Sub

    Public Sub UpdateBillingDate(subID As Integer, newDate As Date)
        Using conn As New MySqlConnection(connString)
            conn.Open()

            Dim cmd As New MySqlCommand("
                UPDATE Subscription
                SET NextBillingDate = @date
                WHERE SubscriptionID = @id", conn)

            cmd.Parameters.AddWithValue("@date", newDate)
            cmd.Parameters.AddWithValue("@id", subID)

            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ' ========================== NOTIFICATIONS & EMAIL ==========================
    Public Sub CheckNotifications()
        Dim subscriptions As New List(Of Tuple(Of Integer, String, String, Date, Decimal, Integer, Boolean))()

        Using conn As New MySqlConnection(connString)
            conn.Open()

            ' Fetch active subscriptions with their associated user emails
            Dim cmd As New MySqlCommand("
                SELECT s.SubscriptionID, s.ServiceName, s.PaymentDeadline,
                       s.SubscriptionFee, u.Email, s.ReminderDays, s.UseBankingDays
                FROM Subscription s
                JOIN User u ON s.UserID = u.UserID
                WHERE s.IsActive = TRUE", conn)

            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    subscriptions.Add(Tuple.Create(
                        CInt(reader("SubscriptionID")),
                        reader("ServiceName").ToString(),
                        reader("Email").ToString(),
                        Convert.ToDateTime(reader("PaymentDeadline")),
                        Convert.ToDecimal(reader("SubscriptionFee")),
                        Convert.ToInt32(reader("ReminderDays")),
                        If(IsDBNull(reader("UseBankingDays")), True, Convert.ToBoolean(reader("UseBankingDays")))
                    ))
                End While
            End Using
        End Using

        ' Process each subscription for possible notifications
        For Each sub_ In subscriptions
            Dim subID As Integer = sub_.Item1
            Dim service As String = sub_.Item2
            Dim email As String = sub_.Item3
            Dim deadlineDate As Date = sub_.Item4
            Dim fee As Decimal = sub_.Item5
            Dim reminderDays As Integer = sub_.Item6
            Dim useBankingDays As Boolean = sub_.Item7
            Dim today As Date = Date.Now.Date

            ' Calculate days remaining until the deadline
            Dim daysRemaining As Integer = 0
            If deadlineDate > today Then
                If useBankingDays Then
                    daysRemaining = BusinessLogic.GetBankingDaysBetween(today, deadlineDate)
                Else
                    daysRemaining = (deadlineDate - today).Days
                End If
            End If

            ' 1. Reminder: Due Soon (before deadline)
            If daysRemaining <= reminderDays AndAlso daysRemaining > 0 AndAlso Not NotificationAlreadySent(subID, "Reminder") Then
                SendNotificationEmail(email, service, fee, deadlineDate, "Reminder", daysRemaining, useBankingDays)
                SaveNotification(subID, "Reminder")
            End If

            ' 2. Due Today (on deadline)
            If deadlineDate = today AndAlso Not NotificationAlreadySent(subID, "Due") Then
                SendNotificationEmail(email, service, fee, deadlineDate, "Due", 0, useBankingDays)
                SaveNotification(subID, "Due")
            End If

            ' 3. Overdue (after deadline)
            Dim daysOverdue As Integer = 0
            If today > deadlineDate Then
                If useBankingDays Then
                    daysOverdue = BusinessLogic.GetBankingDaysBetween(deadlineDate, today)
                Else
                    daysOverdue = (today - deadlineDate).Days
                End If
            End If

            ' Send overdue notice if not already sent today
            If daysOverdue > 0 AndAlso Not NotificationAlreadySent(subID, "Overdue") Then
                SendNotificationEmail(email, service, fee, deadlineDate, "Overdue", daysOverdue, useBankingDays)
                SaveNotification(subID, "Overdue")
            End If
        Next
    End Sub

    ' Composes and sends an HTML notification email
    Private Sub SendNotificationEmail(toEmail As String, service As String, fee As Decimal, billingDate As Date, notifType As String, days As Integer, useBankingDays As Boolean)
        Dim subject As String
        Dim heading As String
        Dim emoji As String
        Dim headingColor As String
        Dim bgColor As String
        Dim message As String
        Dim dayLabel As String = If(useBankingDays, "banking day(s)", "day(s)")

        ' Customize email content based on notification type
        Select Case notifType
            Case "Reminder"
                subject = $"Upcoming Due Reminder – {service} - {days} {dayLabel}"
                heading = "Subscription Reminder"
                emoji = "🔔"
                headingColor = "#2c3e50"
                bgColor = "#f4f6f8"
                message = $"This is a friendly reminder that your subscription for <b>{service}</b> is due in <b style='color:#2980b9;'>{days} {dayLabel}</b>."

            Case "Due"
                subject = $"Payment Due Today – {service}"
                heading = "Payment Due Today"
                emoji = "⚠️"
                headingColor = "#e67e22"
                bgColor = "#fff4e5"
                message = $"Your subscription payment for <b>{service}</b> is <b style='color:red;'>due today</b>."

            Case "Overdue"
                subject = $"Overdue Payment Notice – {service}"
                heading = "Payment Overdue"
                emoji = "🚨"
                headingColor = "#c0392b"
                bgColor = "#ffe6e6"
                message = $"Your subscription payment for <b>{service}</b> is now <b style='color:red;'>OVERDUE</b> by <b style='color:red;'>{days} {dayLabel}</b>."

            Case Else
                Exit Sub
        End Select

        Dim closingMessage As String = If(notifType = "Overdue",
            "Please settle your payment immediately to restore or maintain your service.",
            "Please ensure your payment is completed before the due date to avoid interruption.")

        If notifType = "Due" Then
            closingMessage = "Please settle your payment today to avoid service interruption."
        End If

        Dim body As String = $"
            <div style='font-family: Arial, sans-serif; padding:20px; background-color:{bgColor};'>
                <div style='max-width:600px; margin:auto; background:white; padding:20px; border-radius:10px;'>
                    <h2 style='color:{headingColor};'>{emoji} {heading}</h2>
                    <p>Dear Subscriber,</p>
                    <p>{message}</p>
                    <hr>
                    <p><b>Service:</b> {service}</p>
                    <p><b>Amount:</b> <span style='color:#27ae60; font-size:16px;'>₱{fee.ToString("N2")}</span></p>
                    <p><b>Due Date:</b> {billingDate.ToString("MMMM dd, yyyy")}</p>
                    <hr>
                    <p style='color:#555;'>{closingMessage}</p>
                    <p style='margin-top:20px;'>Thank you,<br><b>Subscription Manager</b></p>
                </div>
            </div>"

        SendEmail(toEmail, subject, body)
    End Sub

    Private Async Sub SendEmail(toEmail As String, subject As String, body As String)
        Dim success As Boolean = Await BusinessLogic.SendEmailAsync(toEmail, subject, body)

        If Not success Then
            MessageBox.Show($"Failed to send email to {toEmail}. Check if your MailBee License Key is valid in EmailSettings.vb.", "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Debug.WriteLine("MailBee Email failed to send to: " & toEmail)
        End If
    End Sub

    Private Sub timerCheck_Tick(sender As Object, e As EventArgs) Handles timerCheck.Tick
        CheckNotifications()
    End Sub

    ' ========================== REFRESH & DATA DISPLAY ==========================

    Private Sub RefreshData()
        Dim scrollIndex As Integer = If(dgvSubscriptions.FirstDisplayedScrollingRowIndex >= 0,
            dgvSubscriptions.FirstDisplayedScrollingRowIndex, -1)

        dtSubscriptions = GetSubscriptions()
        dgvSubscriptions.AutoGenerateColumns = True
        dgvSubscriptions.DataSource = dtSubscriptions

        ApplyGridStyling()
        ApplyFilters()
        ConfigureColumns()
        AddActionColumn()
        ConfigureColumnWidths()

        dgvSubscriptions.ClearSelection()

        If scrollIndex >= 0 AndAlso scrollIndex < dgvSubscriptions.Rows.Count Then
            dgvSubscriptions.FirstDisplayedScrollingRowIndex = scrollIndex
        End If

        LoadStatsFromDB()
        UpdateNotificationBadge()
    End Sub

    Private Sub ApplyGridStyling()
        With dgvSubscriptions
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250)

            ' Row selection colors (Light blue)
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 230, 250)
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 230, 250)
            .AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.Black

            .RowHeadersVisible = False
            .BackgroundColor = Color.White
            .BorderStyle = BorderStyle.None
            .EnableHeadersVisualStyles = False

            ' Header styling (Neutral gray)
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(242, 242, 242)
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41)
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(242, 242, 242)
            .ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(33, 37, 41)
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI Semibold", 10.0F)
            .ColumnHeadersHeight = 45
            .GridColor = Color.FromArgb(222, 226, 230)
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .DefaultCellStyle.Font = New Font("Segoe UI", 10.0F)
            .RowTemplate.Height = 42
        End With
    End Sub

    ' ========================== GRID INITIALIZATION ==========================
    Private Sub dgvSubscriptions_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) Handles dgvSubscriptions.DataError
        e.ThrowException = False
        Debug.WriteLine("Grid DataError: " & e.Exception.Message)
    End Sub

    Private Sub ConfigureColumns()
        With dgvSubscriptions
            If .Columns.Contains("ServiceName") Then
                .Columns("ServiceName").HeaderText = "Service"
                .Columns("ServiceName").DisplayIndex = 0
            End If

            If .Columns.Contains("SubscriptionFee") Then
                .Columns("SubscriptionFee").HeaderText = "Fee"
                .Columns("SubscriptionFee").DefaultCellStyle.Format = "₱#,##0.00" ' Currency format
                .Columns("SubscriptionFee").DisplayIndex = 1
            End If

            If .Columns.Contains("NextBillingDate") Then
                .Columns("NextBillingDate").HeaderText = "Next Billing"
                .Columns("NextBillingDate").DisplayIndex = 2
            End If

            If .Columns.Contains("PaymentDeadline") Then
                .Columns("PaymentDeadline").HeaderText = "Deadline"
                .Columns("PaymentDeadline").DisplayIndex = 3
            End If

            If .Columns.Contains("Email") Then .Columns("Email").Visible = False
            If .Columns.Contains("BillingType") Then .Columns("BillingType").Visible = False
            If .Columns.Contains("SubscriptionID") Then .Columns("SubscriptionID").Visible = False
            If .Columns.Contains("IsPinned") Then .Columns("IsPinned").Visible = False
            If .Columns.Contains("ReminderDays") Then .Columns("ReminderDays").Visible = False
            If .Columns.Contains("IsDueSoon") Then .Columns("IsDueSoon").Visible = False
            If .Columns.Contains("IsOverdue") Then .Columns("IsOverdue").Visible = False
            If .Columns.Contains("IsDueToday") Then .Columns("IsDueToday").Visible = False
            If .Columns.Contains("IsPaid") Then .Columns("IsPaid").Visible = False
            If .Columns.Contains("IsActive") Then .Columns("IsActive").Visible = False
            If .Columns.Contains("PaymentDeadlineDays") Then .Columns("PaymentDeadlineDays").Visible = False
            If .Columns.Contains("CurrentCycleStatus") Then .Columns("CurrentCycleStatus").Visible = False
        End With
    End Sub

    Private Sub AddActionColumn()
        For Each colName In {"Actions", "Paid", "View"}
            If dgvSubscriptions.Columns.Contains(colName) Then
                dgvSubscriptions.Columns.Remove(colName)
            End If
        Next

        Dim btnMenu As New DataGridViewButtonColumn()
        btnMenu.Name = "Actions"
        btnMenu.HeaderText = ""
        btnMenu.Text = "⋮"
        btnMenu.UseColumnTextForButtonValue = True
        btnMenu.Width = 35
        dgvSubscriptions.Columns.Insert(0, btnMenu)

        dgvSubscriptions.Columns("Actions").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        dgvSubscriptions.Columns("Actions").DefaultCellStyle.BackColor = Color.White
        dgvSubscriptions.Columns("Actions").DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 230, 250)
        dgvSubscriptions.Columns("Actions").DefaultCellStyle.SelectionForeColor = Color.Black

        Dim menu As New ContextMenuStrip()

        Dim mnuEdit As New ToolStripMenuItem("Edit")
        mnuEdit.Name = "mnuEdit"
        Dim mnuCancel As New ToolStripMenuItem("Cancel Subscription")
        mnuCancel.Name = "mnuCancel"
        Dim mnuPin As New ToolStripMenuItem("Pin to Top")
        mnuPin.Name = "mnuPin"
        Dim mnuDelete As New ToolStripMenuItem("Delete Subscription")
        mnuDelete.Name = "mnuDelete"
        mnuDelete.ForeColor = Color.Red ' Visual warning for deletion

        menu.Items.AddRange(New ToolStripItem() {mnuEdit, mnuCancel, mnuPin, New ToolStripSeparator(), mnuDelete})

        AddHandler mnuEdit.Click, AddressOf EditSubscription
        AddHandler mnuCancel.Click, AddressOf CancelSubscription
        AddHandler mnuPin.Click, AddressOf TogglePinSubscription
        AddHandler mnuDelete.Click, AddressOf DeleteSubscription

        dgvSubscriptions.Tag = menu
    End Sub

    Private Sub RestoreGridState(selectedID As Integer, scrollIndex As Integer)

    End Sub

    ' ========================== ADVANCED SEARCH & FILTER ==========================
    Private Sub ApplyFilters()
        If dtSubscriptions Is Nothing Then Exit Sub

        Dim filterParts As New List(Of String)()

        ' 1. Handle text search filter (matches service name)
        Dim searchText As String = txtSearch.Text.Trim()
        If searchText.Length > 0 Then
            searchText = searchText.Replace("'", "''") ' Escape single quotes for SQL safety
            filterParts.Add($"(ServiceName LIKE '%{searchText}%')")
        End If

        ' 2. Handle Status filter selection
        Select Case currentStatusFilter
            Case "Overdue"
                filterParts.Add("IsOverdue = TRUE AND IsActive = TRUE")

            Case "Due Today"
                filterParts.Add("IsDueToday = TRUE AND IsActive = TRUE")

            Case "Due Soon"
                filterParts.Add("IsDueSoon = TRUE AND IsActive = TRUE")

            Case "Paid"
                filterParts.Add("IsPaid = TRUE AND IsActive = TRUE")

            Case "Favorites"
                filterParts.Add("IsPinned = TRUE AND IsActive = TRUE")

            Case "Paused"
                filterParts.Add("IsActive = FALSE")

            Case "All"
                filterParts.Add("IsActive = TRUE")
        End Select

        ' 3. Handle Billing Type filter (Monthly/Yearly)
        Select Case currentBillingTypeFilter
            Case "Monthly"
                filterParts.Add("BillingType = 'Monthly'")
            Case "Yearly"
                filterParts.Add("BillingType = 'Yearly'")
            Case "All"
        End Select

        Dim finalFilter As String = String.Join(" AND ", filterParts)

        Try
            Dim dv As New DataView(dtSubscriptions)
            dv.RowFilter = finalFilter
            dgvSubscriptions.DataSource = dv

            If dv.Count = 0 Then
                Dim filterMsg As String = currentStatusFilter.ToLower()
                Dim displayMessage As String = ""

                ' Customize the "No Data" message based on what the user is looking for
                Select Case filterMsg
                    Case "all"
                        displayMessage = "No subscriptions found."
                    Case "overdue"
                        displayMessage = "No overdue subscriptions."
                    Case "due today"
                        displayMessage = "No subscriptions due today."
                    Case "due soon"
                        displayMessage = "No subscriptions due soon."
                    Case "paid"
                        displayMessage = "No paid subscriptions."
                    Case "favorites"
                        displayMessage = "No favorite subscriptions."
                    Case "paused"
                        displayMessage = "No paused subscriptions."
                    Case Else
                        displayMessage = "No subscriptions found for this filter."
                End Select

                lblNoData.Text = displayMessage
                lblNoData.Location = New Point(
                    (dgvSubscriptions.Width - lblNoData.Width) / 2,
                    (dgvSubscriptions.Height - lblNoData.Height) / 2
                )
                lblNoData.Visible = True
                lblNoData.BringToFront()
            Else
                lblNoData.Visible = False
            End If

            ConfigureColumns()
            AddActionColumn()
            ConfigureColumnWidths()
        Catch ex As Exception
            dgvSubscriptions.DataSource = dtSubscriptions
            lblNoData.Visible = False
            ConfigureColumns()
            AddActionColumn()
            ConfigureColumnWidths()
        End Try
    End Sub

    ' Search text changes
    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        ApplyFilters()
        LoadStatsFromDB()
    End Sub

    ' Status dropdown changes
    Private Sub cboStatusFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboStatusFilter.SelectedIndexChanged
        currentStatusFilter = cboStatusFilter.SelectedItem.ToString()
        ApplyFilters()
    End Sub

    ' Billing Type dropdown changes
    Private Sub cboBillingTypeFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboBillingTypeFilter.SelectedIndexChanged
        currentBillingTypeFilter = cboBillingTypeFilter.SelectedItem.ToString()
        ApplyFilters()
    End Sub

    ' ========================== ROW COLORING ==========================

    Private Function GetRowColorByDeadline(deadlineDate As Date, reminderDays As Integer, isPaid As Boolean) As Color
        If isPaid Then
            Return Color.Empty
        End If

        Dim today As Date = Date.Now.Date

        ' Modern Soft Pastel Colors
        If deadlineDate < today Then Return Color.FromArgb(255, 235, 238)    ' Soft Red: Overdue
        If deadlineDate = today Then Return Color.FromArgb(255, 253, 231)    ' Soft Yellow: Due today

        Dim daysUntil As Integer = (deadlineDate - today).Days
        If deadlineDate > today AndAlso daysUntil <= reminderDays Then
            Return Color.FromArgb(232, 245, 233) ' Soft Green: Due soon
        End If

        Return Color.Empty ' Default (White)
    End Function

    Private Sub dgvSubscriptions_RowPrePaint(sender As Object, e As DataGridViewRowPrePaintEventArgs) Handles dgvSubscriptions.RowPrePaint
        Dim row = dgvSubscriptions.Rows(e.RowIndex)
        If row.IsNewRow Then Exit Sub
        If e.RowIndex = hoveredRowIndex Then Exit Sub

        Dim isActiveRow As Boolean = If(row.Cells("IsActive").Value IsNot DBNull.Value, Convert.ToBoolean(row.Cells("IsActive").Value), True)
        If Not isActiveRow Then
            row.DefaultCellStyle.BackColor = If(e.RowIndex Mod 2 = 0, Color.White, Color.FromArgb(248, 249, 250))
            Exit Sub
        End If

        Dim deadlineDate As Date
        Dim reminderDays As Integer = 3
        Dim isPaid As Boolean = False

        If row.Cells("ReminderDays").Value IsNot Nothing AndAlso Not IsDBNull(row.Cells("ReminderDays").Value) Then
            reminderDays = Convert.ToInt32(row.Cells("ReminderDays").Value)
        End If

        If row.Cells("IsPaid").Value IsNot Nothing AndAlso Not IsDBNull(row.Cells("IsPaid").Value) Then
            isPaid = Convert.ToBoolean(row.Cells("IsPaid").Value)
        End If

        If row.Cells("PaymentDeadline").Value IsNot Nothing AndAlso
           Not IsDBNull(row.Cells("PaymentDeadline").Value) AndAlso
           Date.TryParse(row.Cells("PaymentDeadline").Value.ToString(), deadlineDate) Then

            Dim color = GetRowColorByDeadline(deadlineDate, reminderDays, isPaid)
            If color <> Color.Empty Then
                row.DefaultCellStyle.BackColor = color
            Else
                row.DefaultCellStyle.BackColor = If(e.RowIndex Mod 2 = 0, Color.White, Color.FromArgb(248, 249, 250))
            End If
        Else
            row.DefaultCellStyle.BackColor = If(e.RowIndex Mod 2 = 0, Color.White, Color.FromArgb(248, 249, 250))
        End If
    End Sub

    ' Event: Mouse enters a cell (Highlight row)
    Private Sub dgvSubscriptions_CellMouseEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgvSubscriptions.CellMouseEnter
        If e.RowIndex >= 0 AndAlso e.RowIndex <> hoveredRowIndex Then
            hoveredRowIndex = e.RowIndex
            Dim row = dgvSubscriptions.Rows(e.RowIndex)
            ' Use a consistent hover color for all rows
            row.DefaultCellStyle.BackColor = Color.FromArgb(210, 230, 250)
        End If
    End Sub

    ' Event: Mouse leaves a cell (Restore original row color)
    Private Sub dgvSubscriptions_CellMouseLeave(sender As Object, e As DataGridViewCellEventArgs) Handles dgvSubscriptions.CellMouseLeave
        If e.RowIndex < 0 Then Exit Sub

        If e.RowIndex = hoveredRowIndex Then
            hoveredRowIndex = -1
        End If

        Dim row = dgvSubscriptions.Rows(e.RowIndex)

        Dim isActiveRow As Boolean = If(row.Cells("IsActive").Value IsNot DBNull.Value, Convert.ToBoolean(row.Cells("IsActive").Value), True)
        If Not isActiveRow Then
            row.DefaultCellStyle.BackColor = If(e.RowIndex Mod 2 = 0, Color.White, Color.FromArgb(248, 249, 250))
            Exit Sub
        End If

        Dim deadlineDate As Date
        Dim reminderDays As Integer = 3
        Dim isPaid As Boolean = False

        If row.Cells("ReminderDays").Value IsNot Nothing AndAlso Not IsDBNull(row.Cells("ReminderDays").Value) Then
            reminderDays = Convert.ToInt32(row.Cells("ReminderDays").Value)
        End If

        If row.Cells("IsPaid").Value IsNot Nothing AndAlso Not IsDBNull(row.Cells("IsPaid").Value) Then
            isPaid = Convert.ToBoolean(row.Cells("IsPaid").Value)
        End If

        If row.Cells("PaymentDeadline").Value IsNot Nothing AndAlso
           Not IsDBNull(row.Cells("PaymentDeadline").Value) AndAlso
           Date.TryParse(row.Cells("PaymentDeadline").Value.ToString(), deadlineDate) Then

            Dim color = GetRowColorByDeadline(deadlineDate, reminderDays, isPaid)
            If color <> Color.Empty Then
                row.DefaultCellStyle.BackColor = color
            Else
                row.DefaultCellStyle.BackColor = If(e.RowIndex Mod 2 = 0, Color.White, Color.FromArgb(248, 249, 250))
            End If
        Else
            row.DefaultCellStyle.BackColor = If(e.RowIndex Mod 2 = 0, Color.White, Color.FromArgb(248, 249, 250))
        End If
    End Sub

    ' ========================== GRID INTERACTIONS ==========================
    Private Sub dgvSubscriptions_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvSubscriptions.CellClick
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        Dim cellValue = dgvSubscriptions.Rows(e.RowIndex).Cells("SubscriptionID").Value
        If cellValue Is Nothing OrElse IsDBNull(cellValue) Then Exit Sub

        Dim subID As Integer = Convert.ToInt32(cellValue)
        Dim columnName As String = dgvSubscriptions.Columns(e.ColumnIndex).Name

        Select Case columnName
            Case "View"
                ' Open the detailed view for this subscription
                OpenSubscriptionDetails(subID)

            Case "Actions"
                ' Show the ⋮ context menu
                dgvSubscriptions.ClearSelection()
                dgvSubscriptions.Rows(e.RowIndex).Selected = True

                Dim menu As ContextMenuStrip = CType(dgvSubscriptions.Tag, ContextMenuStrip)
                Dim row = dgvSubscriptions.Rows(e.RowIndex)

                Dim isPinnedRow As Boolean = If(row.Cells("IsPinned").Value IsNot DBNull.Value, Convert.ToBoolean(row.Cells("IsPinned").Value), False)
                Dim isActiveRow As Boolean = If(row.Cells("IsActive").Value IsNot DBNull.Value, Convert.ToBoolean(row.Cells("IsActive").Value), True)

                Dim mnuEditItem = TryCast(menu.Items("mnuEdit"), ToolStripMenuItem)
                Dim mnuCancelItem = TryCast(menu.Items("mnuCancel"), ToolStripMenuItem)
                Dim mnuPinItem = TryCast(menu.Items("mnuPin"), ToolStripMenuItem)
                Dim mnuDeleteItem = TryCast(menu.Items("mnuDelete"), ToolStripMenuItem)

                If mnuEditItem IsNot Nothing Then mnuEditItem.Visible = isActiveRow
                If mnuCancelItem IsNot Nothing Then mnuCancelItem.Visible = isActiveRow
                If mnuPinItem IsNot Nothing Then mnuPinItem.Text = If(isPinnedRow, "Unpin from Top", "Pin to Top")
                If mnuDeleteItem IsNot Nothing Then mnuDeleteItem.Visible = Not isActiveRow

                menu.Show(Cursor.Position)
        End Select
    End Sub

    Private Sub dgvSubscriptions_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvSubscriptions.CellFormatting
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        Dim row = dgvSubscriptions.Rows(e.RowIndex)
        Dim isActiveRow As Boolean = If(row.Cells("IsActive").Value IsNot DBNull.Value, Convert.ToBoolean(row.Cells("IsActive").Value), True)

        ' Special formatting for paused subscriptions
        If Not isActiveRow Then
            If dgvSubscriptions.Columns(e.ColumnIndex).Name = "NextBillingDate" OrElse
               dgvSubscriptions.Columns(e.ColumnIndex).Name = "PaymentDeadline" Then
                e.Value = "Paused"
                e.FormattingApplied = True
                e.CellStyle.ForeColor = Color.Gray
                Exit Sub
            End If
        End If

        If isActiveRow Then
            If Convert.ToBoolean(row.Cells("IsOverdue").Value) Then
                row.Cells(e.ColumnIndex).ToolTipText = "This subscription is OVERDUE."
            ElseIf Convert.ToBoolean(row.Cells("IsDueToday").Value) Then
                row.Cells(e.ColumnIndex).ToolTipText = "This subscription is DUE TODAY."
            ElseIf Convert.ToBoolean(row.Cells("IsDueSoon").Value) Then
                row.Cells(e.ColumnIndex).ToolTipText = "This subscription is DUE SOON."
            End If
        End If

        Dim isBillingCycleStarted As Boolean = False
        If isActiveRow Then
            Dim billingDate As Date
            Dim deadlineDate As Date
            If row.Cells("NextBillingDate").Value IsNot Nothing AndAlso Not IsDBNull(row.Cells("NextBillingDate").Value) AndAlso
               row.Cells("PaymentDeadline").Value IsNot Nothing AndAlso Not IsDBNull(row.Cells("PaymentDeadline").Value) Then

                If Date.TryParse(row.Cells("NextBillingDate").Value.ToString(), billingDate) AndAlso
                   Date.TryParse(row.Cells("PaymentDeadline").Value.ToString(), deadlineDate) Then

                    Dim today As Date = Date.Now.Date
                    ' Bold blue if billing date is today or in the past, AND not overdue yet
                    If billingDate <= today AndAlso deadlineDate >= today Then
                        isBillingCycleStarted = True
                    End If
                End If
            End If
        End If

        ' Apply bold blue styling for active billing cycles
        If dgvSubscriptions.Columns(e.ColumnIndex).Name = "NextBillingDate" AndAlso isActiveRow Then
            If isBillingCycleStarted AndAlso Not (row.Cells("IsPaid").Value IsNot Nothing AndAlso Convert.ToBoolean(row.Cells("IsPaid").Value)) Then
                ' Only highlight if cycle started AND NOT PAID
                e.CellStyle.Font = New Font(dgvSubscriptions.Font, FontStyle.Bold)
                e.CellStyle.ForeColor = Color.FromArgb(41, 128, 185) ' Blue
                row.Cells(e.ColumnIndex).ToolTipText = "Billing cycle in progress."
            End If
        End If

        ' Add 📌 icon to service name for pinned items
        If dgvSubscriptions.Columns(e.ColumnIndex).Name = "ServiceName" Then
            Dim isPinnedRow As Boolean = False
            If row.Cells("IsPinned").Value IsNot Nothing AndAlso Not IsDBNull(row.Cells("IsPinned").Value) Then
                isPinnedRow = Convert.ToBoolean(row.Cells("IsPinned").Value)
            End If

            Dim serviceName As String = e.Value.ToString()

            If isPinnedRow Then
                e.Value = "📌 " & serviceName
            Else
                e.Value = serviceName
            End If
            e.FormattingApplied = True
        End If
    End Sub

    Private Sub dgvSubscriptions_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvSubscriptions.CellDoubleClick
        If e.RowIndex < 0 Then Exit Sub

        Dim subID As Integer = Convert.ToInt32(dgvSubscriptions.Rows(e.RowIndex).Cells("SubscriptionID").Value)
        OpenSubscriptionDetails(subID)
    End Sub

    Private Sub dgvSubscriptions_CellMouseDown(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgvSubscriptions.CellMouseDown
        If e.Button = MouseButtons.Right AndAlso e.RowIndex >= 0 Then
            dgvSubscriptions.ClearSelection()
            dgvSubscriptions.Rows(e.RowIndex).Selected = True

            For Each col As DataGridViewColumn In dgvSubscriptions.Columns
                If col.Visible Then
                    dgvSubscriptions.CurrentCell = dgvSubscriptions.Rows(e.RowIndex).Cells(col.Index)
                    Exit For
                End If
            Next
        End If
    End Sub

    ' ========================== SUBSCRIPTION CRUD ==========================

    Private Async Sub OpenSubscriptionDetails(subID As Integer)
        Dim frm As New SubscriptionDetails()
        frm.connString = connString
        frm.subID = subID
        frm.Owner = Me

        If frm.ShowDialog() = DialogResult.OK Then
            RefreshData()
            LoadStatsFromDB()
            UpdateNotificationBadge()
            ' Show toast on the main form after successful payment
            Await ShowToast("Payment recorded successfully!", Color.FromArgb(46, 204, 113))
        Else
            RefreshData()
            LoadStatsFromDB()
            UpdateNotificationBadge()
        End If
    End Sub

    Private Sub EditSubscription(sender As Object, e As EventArgs)
        If dgvSubscriptions.SelectedRows.Count = 0 Then Exit Sub

        Dim subID As Integer = Convert.ToInt32(dgvSubscriptions.SelectedRows(0).Cells("SubscriptionID").Value)

        Dim isActiveRow As Boolean = True
        If dgvSubscriptions.SelectedRows(0).Cells("IsActive").Value IsNot Nothing AndAlso Not IsDBNull(dgvSubscriptions.SelectedRows(0).Cells("IsActive").Value) Then
            isActiveRow = Convert.ToBoolean(dgvSubscriptions.SelectedRows(0).Cells("IsActive").Value)
        End If

        If Not isActiveRow Then
            MessageBox.Show("This subscription is paused and cannot be edited. Resume it first to make changes.", "Action Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim frm As New FormAddSubscription()
        frm.connString = connString
        frm.subID = subID

        If frm.ShowDialog() = DialogResult.OK Then
            RefreshData()
            LoadStatsFromDB()
            CheckNotifications()
            UpdateNotificationBadge()
        End If
    End Sub

    Private Sub CancelSubscription(sender As Object, e As EventArgs)
        If dgvSubscriptions.SelectedRows.Count = 0 Then Exit Sub

        Dim subID As Integer = Convert.ToInt32(dgvSubscriptions.SelectedRows(0).Cells("SubscriptionID").Value)
        Dim serviceName As String = dgvSubscriptions.SelectedRows(0).Cells("ServiceName").Value.ToString()

        Dim confirm = MessageBox.Show($"Are you sure you want to cancel (pause) {serviceName}?", "Confirm Cancellation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If confirm = DialogResult.Yes Then
            Using conn As New MySqlConnection(connString)
                conn.Open()
                Dim cmd As New MySqlCommand("UPDATE Subscription SET IsActive=FALSE WHERE SubscriptionID=@id", conn)
                cmd.Parameters.AddWithValue("@id", subID)
                cmd.ExecuteNonQuery()
            End Using

            MessageBox.Show("Subscription cancelled (paused). You can resume it later from the Paused filter.")
            RefreshData()
            LoadStatsFromDB()
            UpdateNotificationBadge()
        End If
    End Sub

    Private Sub DeleteSubscription(sender As Object, e As EventArgs)
        If dgvSubscriptions.SelectedRows.Count = 0 Then Exit Sub

        Dim subID As Integer = Convert.ToInt32(dgvSubscriptions.SelectedRows(0).Cells("SubscriptionID").Value)
        Dim serviceName As String = dgvSubscriptions.SelectedRows(0).Cells("ServiceName").Value.ToString()

        Dim confirm = MessageBox.Show($"Are you sure you want to PERMANENTLY delete {serviceName}? This will also delete all payment history for this subscription.",
                                    "Confirm Permanent Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Stop)

        If confirm = DialogResult.Yes Then
            Try
                Using conn As New MySqlConnection(connString)
                    conn.Open()

                    ' 1. Delete notifications
                    Dim cmdNotif As New MySqlCommand("DELETE FROM Notification WHERE SubscriptionID = @id", conn)
                    cmdNotif.Parameters.AddWithValue("@id", subID)
                    cmdNotif.ExecuteNonQuery()

                    ' 2. Delete payment history
                    Dim cmdHistory As New MySqlCommand("DELETE FROM PaymentHistory WHERE SubscriptionID = @id", conn)
                    cmdHistory.Parameters.AddWithValue("@id", subID)
                    cmdHistory.ExecuteNonQuery()

                    ' 3. Delete the subscription itself
                    Dim cmdSub As New MySqlCommand("DELETE FROM Subscription WHERE SubscriptionID = @id", conn)
                    cmdSub.Parameters.AddWithValue("@id", subID)
                    cmdSub.ExecuteNonQuery()
                End Using

                MessageBox.Show($"{serviceName} has been permanently deleted.", "Subscription Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information)
                RefreshData()
                LoadStatsFromDB()
                UpdateNotificationBadge()
            Catch ex As Exception
                MessageBox.Show("Error deleting subscription: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub TogglePinSubscription(sender As Object, e As EventArgs)
        If dgvSubscriptions.SelectedRows.Count = 0 Then Exit Sub

        Dim row = dgvSubscriptions.SelectedRows(0)
        Dim subID As Integer = Convert.ToInt32(row.Cells("SubscriptionID").Value)
        Dim currentPinned As Boolean = False
        If row.Cells("IsPinned").Value IsNot Nothing AndAlso Not IsDBNull(row.Cells("IsPinned").Value) Then
            currentPinned = Convert.ToBoolean(row.Cells("IsPinned").Value)
        End If

        Using conn As New MySqlConnection(connString)
            conn.Open()
            Dim cmd As New MySqlCommand("UPDATE Subscription SET IsPinned=@pin WHERE SubscriptionID=@id", conn)
            cmd.Parameters.AddWithValue("@pin", Not currentPinned)
            cmd.Parameters.AddWithValue("@id", subID)
            cmd.ExecuteNonQuery()
        End Using

        RefreshData()
    End Sub



    ' ========================== TOOLBAR / CONTEXT MENU HANDLERS ==========================

    Private Sub EditToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditToolStripMenuItem.Click
        EditSubscription(sender, e)
    End Sub

    Private Sub DeleteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteToolStripMenuItem.Click
        CancelSubscription(sender, e)
    End Sub

    ' ========================== BUTTON HANDLERS ==========================

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        RefreshData()
        LoadStatsFromDB()
        UpdateNotificationBadge()
    End Sub

    Private Sub btnAddSubscription_Click(sender As Object, e As EventArgs) Handles btnAddSubscription.Click
        Dim frm As New FormAddSubscription()
        frm.connString = connString

        If frm.ShowDialog() = DialogResult.OK Then
            RefreshData()
            LoadStatsFromDB()
            CheckNotifications()
            UpdateNotificationBadge()
        End If
    End Sub

    Private Sub btnNotifications_Click(sender As Object, e As EventArgs) Handles btnNotifications.Click
        Dim frm As New NotificationCenter()
        frm.connString = connString
        frm.ShowDialog()

        RefreshData()
        ConfigureColumnWidths()
        LoadStatsFromDB()
        UpdateNotificationBadge()
    End Sub

    Private Sub btnCalendar_Click(sender As Object, e As EventArgs) Handles btnCalendar.Click
        Dim frm As New CalendarView()
        frm.connString = connString
        frm.ShowDialog()
    End Sub

    ' ========================== STATISTICS ==========================

    Private Sub LoadStatsFromDB()
        If dtSubscriptions Is Nothing Then Exit Sub

        Dim totalActive As Integer = 0
        Dim dueToday As Integer = 0
        Dim overdue As Integer = 0
        Dim dueSoon As Integer = 0

        Dim totalAmountToday As Decimal = 0
        Dim totalAmountOverdue As Decimal = 0
        Dim totalAmountSoon As Decimal = 0

        For Each row As DataRow In dtSubscriptions.Rows
            If IsDBNull(row("IsActive")) OrElse Not Convert.ToBoolean(row("IsActive")) Then Continue For

            Dim fee As Decimal = If(IsDBNull(row("SubscriptionFee")), 0, Convert.ToDecimal(row("SubscriptionFee")))
            totalActive += 1

            If Convert.ToBoolean(row("IsDueToday")) Then
                dueToday += 1
                totalAmountToday += fee
            End If

            If Convert.ToBoolean(row("IsOverdue")) Then
                overdue += 1
                totalAmountOverdue += fee
            End If

            If Convert.ToBoolean(row("IsDueSoon")) Then
                dueSoon += 1
                totalAmountSoon += fee
            End If
        Next

        lblTotalCount.Text = totalActive.ToString()
        lblDueTodayCount.Text = dueToday.ToString()
        lblOverdueCount.Text = overdue.ToString()
        lblDueSoonCount.Text = dueSoon.ToString()


        lblDueTodayText.Text = "Due Today" & vbCrLf & "₱" & totalAmountToday.ToString("N0")
        lblDueTodayText.ForeColor = Color.FromArgb(184, 134, 11)

        lblOverdueText.Text = "Overdue" & vbCrLf & "₱" & totalAmountOverdue.ToString("N0")
        lblOverdueText.ForeColor = Color.FromArgb(192, 57, 43)

        lblDueSoonText.Text = "Due Soon" & vbCrLf & "₱" & totalAmountSoon.ToString("N0")
        lblDueSoonText.ForeColor = Color.FromArgb(39, 174, 96)

        lblDueTodayText.Font = New Font("Segoe UI Semibold", 9F)
        lblOverdueText.Font = New Font("Segoe UI Semibold", 9F)
        lblDueSoonText.Font = New Font("Segoe UI Semibold", 9F)
    End Sub

End Class
