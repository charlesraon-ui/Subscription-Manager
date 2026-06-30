Imports MySql.Data.MySqlClient

Public Class NotificationCenter
    ' Connection string passed from the main form
    Public connString As String

    ' Event: Form is loading
    Private Sub NotificationCenter_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupNotificationGrid()
        LoadNotifications()
    End Sub

    ' Event: Form is shown (Used to fix column sizing)
    Private Sub NotificationCenter_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        LockNotificationColumnWidth()
    End Sub

    ' Event: Window resized (Keep column width locked)
    Private Sub dgvNotifications_SizeChanged(sender As Object, e As EventArgs) Handles dgvNotifications.SizeChanged
        LockNotificationColumnWidth()
    End Sub

    '----------------------------------------SETUP GRID----------------------------------------------
    ' Configures the appearance and behavior of the notification list
    Private Sub SetupNotificationGrid()
        With dgvNotifications
            .Columns.Clear()
            .Rows.Clear()
            .AutoGenerateColumns = False
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToResizeColumns = False
            .AllowUserToResizeRows = False
            .AllowUserToOrderColumns = False
            .RowHeadersVisible = False
            .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            .GridColor = Color.LightGray
            .BackgroundColor = Color.White
            .DefaultCellStyle.BackColor = Color.White
            .DefaultCellStyle.ForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255)
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular)
            .RowTemplate.Height = 40
            .EnableHeadersVisualStyles = False
            .ColumnHeadersVisible = False ' Hide headers for a cleaner list view
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
            .ReadOnly = True
            .Cursor = Cursors.Hand
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None

            ' Define columns
            .Columns.Add("SubID", "ID")
            .Columns.Add("Message", "Notifications")

            ' Hide ID column (only used internally)
            .Columns("SubID").Visible = False
            .Columns("Message").AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            .Columns("Message").Resizable = DataGridViewTriState.False
            .Columns("Message").SortMode = DataGridViewColumnSortMode.NotSortable
        End With
    End Sub

    '----------------------------------------LOAD NOTIFICATIONS----------------------------------------------
    ' Fetches urgency-related alerts from the database
    Private Sub LoadNotifications()
        Using conn As New MySqlConnection(connString)
            conn.Open()

            ' Fetch active subscriptions, ordered by priority (Pinned > Overdue > Today > Soon)
            Dim cmd As New MySqlCommand("
            SELECT 
                s.SubscriptionID,
                s.ServiceName,
                DATEDIFF(s.PaymentDeadline, CURDATE()) AS DaysLeft,
                s.ReminderDays
            FROM Subscription s
            JOIN User u ON s.UserID = u.UserID
            WHERE s.IsActive = TRUE
            ORDER BY 
                 s.IsPinned DESC, 
                 CASE 
                     WHEN s.PaymentDeadline < CURDATE() THEN 1 
                     WHEN s.PaymentDeadline = CURDATE() THEN 2 
                     WHEN s.PaymentDeadline > CURDATE() AND DATEDIFF(s.PaymentDeadline, CURDATE()) <= s.ReminderDays THEN 3 
                     ELSE 4 
                 END ASC,
                 s.NextBillingDate ASC", conn)

            Dim dt As New DataTable()
            Dim da As New MySqlDataAdapter(cmd)
            da.Fill(dt)

            dgvNotifications.Rows.Clear()

            ' Generate human-readable messages for each alert
            For Each row As DataRow In dt.Rows
                Dim subID = Convert.ToInt32(row("SubscriptionID"))
                Dim service = row("ServiceName").ToString()
                Dim reminderDays = Convert.ToInt32(row("ReminderDays"))
                Dim days As Integer = 0

                If Not IsDBNull(row("DaysLeft")) Then
                    days = Convert.ToInt32(row("DaysLeft"))
                Else
                    Continue For
                End If

                Dim message As String = ""

                ' Categorize message based on days left
                If days < 0 Then
                    message = "❗ " & service & " is overdue by " & Math.Abs(days) & " day(s)"
                ElseIf days = 0 Then
                    message = "⏰ " & service & " is due today"
                ElseIf days <= reminderDays Then
                    message = "🔔 " & service & " is due in " & days & " day(s)"
                Else
                    ' If it's not overdue, today, or soon, don't show it in notification center
                    Continue For
                End If

                ' Add the message to the grid
                dgvNotifications.Rows.Add(subID, message)
            Next
        End Using

        ' Final UI adjustments
        LockNotificationColumnWidth()
        dgvNotifications.ClearSelection()
    End Sub

    '----------------------------------------LOCK COLUMN WIDTH----------------------------------------------
    ' Forces the "Message" column to fill the entire width of the grid
    Private Sub LockNotificationColumnWidth()
        If Not dgvNotifications.Columns.Contains("Message") Then Exit Sub

        Dim targetWidth As Integer = dgvNotifications.ClientSize.Width - 2
        If targetWidth < 100 Then targetWidth = 100

        With dgvNotifications.Columns("Message")
            .MinimumWidth = targetWidth
            .Width = targetWidth
        End With
    End Sub

    ' ---------------------------------------ROW FORMATTING----------------------------------------------
    ' Colors notification rows based on their type (Overdue, Today, Soon)
    Private Sub dgvNotifications_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvNotifications.CellFormatting
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        Dim cellValue As String = dgvNotifications.Rows(e.RowIndex).Cells("Message").Value?.ToString()
        If String.IsNullOrEmpty(cellValue) Then Exit Sub

        Dim row = dgvNotifications.Rows(e.RowIndex)

        If cellValue.StartsWith("❗") Then
            ' Overdue - Light red background, dark red text
            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 230, 230)
            row.DefaultCellStyle.ForeColor = Color.FromArgb(192, 57, 43)
        ElseIf cellValue.StartsWith("⏰") Then
            ' Due today - Light yellow background, dark gold text
            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 249, 230)
            row.DefaultCellStyle.ForeColor = Color.FromArgb(183, 149, 11)
        ElseIf cellValue.StartsWith("🔔") Then
            ' Due soon - Light green background, dark green text
            row.DefaultCellStyle.BackColor = Color.FromArgb(230, 255, 237)
            row.DefaultCellStyle.ForeColor = Color.FromArgb(39, 174, 96)
        End If
    End Sub

    '----------------------------------------------------------OPEN DETAILS ON CLICK------------------------------------------------------
    ' Clicking a notification opens the full details of that subscription
    Private Sub dgvNotifications_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvNotifications.CellClick
        If e.RowIndex < 0 Then Exit Sub

        Dim subID As Integer = Convert.ToInt32(dgvNotifications.Rows(e.RowIndex).Cells("SubID").Value)

        Dim details As New SubscriptionDetails()
        details.subID = subID
        details.connString = connString
        details.ShowDialog()

        ' Refresh notifications after closing details (in case user marked it as paid)
        LoadNotifications()
    End Sub

    '----------------------------------------------------------HOVER EFFECT------------------------------------------------------
    ' Highlight the row when mouse is over it
    Private Sub dgvNotifications_CellMouseEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgvNotifications.CellMouseEnter
        If e.RowIndex >= 0 Then
            dgvNotifications.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)
        End If
    End Sub

    ' Restore original color when mouse leaves
    Private Sub dgvNotifications_CellMouseLeave(sender As Object, e As DataGridViewCellEventArgs) Handles dgvNotifications.CellMouseLeave
        If e.RowIndex >= 0 Then
            Dim cellValue As String = dgvNotifications.Rows(e.RowIndex).Cells("Message").Value?.ToString()
            If String.IsNullOrEmpty(cellValue) Then
                dgvNotifications.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.White
                Exit Sub
            End If

            ' Restore specific urgency colors
            If cellValue.StartsWith("❗") Then
                dgvNotifications.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.FromArgb(255, 230, 230)
            ElseIf cellValue.StartsWith("⏰") Then
                dgvNotifications.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.FromArgb(255, 249, 230)
            ElseIf cellValue.StartsWith("🔔") Then
                dgvNotifications.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.FromArgb(230, 255, 237)
            Else
                dgvNotifications.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.White
            End If
        End If
    End Sub

    '----------------------------------------------------------CLOSE NOTIFICATION-------------------------------------------------------
    ' Close the Notification Center window
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub
End Class
