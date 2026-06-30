Imports MySql.Data.MySqlClient

Public Class PaymentHistory
    Public connString As String
    Public subID As Integer
    Public serviceName As String
    Private reminderDays As Integer = 3

    Private Sub PaymentHistory_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lblTitle.Text = "Payment History for " & serviceName
        StyleGrid()
        LoadSubscriptionSettings()
        EnsureCurrentCycleRecordExists()
        LoadHistory()
    End Sub

    Private Sub LoadSubscriptionSettings()
        Using conn As New MySqlConnection(connString)
            conn.Open()
            Dim cmd As New MySqlCommand("SELECT ReminderDays FROM Subscription WHERE SubscriptionID = @id", conn)
            cmd.Parameters.AddWithValue("@id", subID)
            Dim reader = cmd.ExecuteReader()
            If reader.Read() Then
                If Not IsDBNull(reader("ReminderDays")) Then
                    reminderDays = Convert.ToInt32(reader("ReminderDays"))
                End If
            End If
            reader.Close()
        End Using
    End Sub

    ' Ensures the "Not Yet Paid" record exists for the current active cycle before loading
    Private Sub EnsureCurrentCycleRecordExists()
        Using conn As New MySqlConnection(connString)
            conn.Open()
            
            ' Get current billing info from Subscription table
            Dim cmdGetSub As New MySqlCommand("
                SELECT NextBillingDate, PaymentDeadline, SubscriptionFee 
                FROM Subscription WHERE SubscriptionID = @id", conn)
            cmdGetSub.Parameters.AddWithValue("@id", subID)
            
            Dim reader = cmdGetSub.ExecuteReader()
            If reader.Read() Then
                Dim billingDate As Date = Convert.ToDateTime(reader("NextBillingDate")).Date
                Dim deadlineDate As Date = Convert.ToDateTime(reader("PaymentDeadline")).Date
                Dim fee As Decimal = Convert.ToDecimal(reader("SubscriptionFee"))
                reader.Close()

                ' Check if a record exists for this specific cycle
                Dim cmdCheck As New MySqlCommand("
                    SELECT COUNT(*) FROM PaymentHistory 
                    WHERE SubscriptionID = @sid AND DATE(BillingCycleStart) = DATE(@start)", conn)
                cmdCheck.Parameters.AddWithValue("@sid", subID)
                cmdCheck.Parameters.AddWithValue("@start", billingDate)

                If Convert.ToInt32(cmdCheck.ExecuteScalar()) = 0 Then
                    ' Create the missing "Not Yet Paid" record
                    Dim cmdInsert As New MySqlCommand("
                        INSERT INTO PaymentHistory (SubscriptionID, BillingCycleStart, DueDate, Amount, Status)
                        VALUES (@sid, @start, @deadline, @amount, 'Not Yet Paid')", conn)
                    cmdInsert.Parameters.AddWithValue("@sid", subID)
                    cmdInsert.Parameters.AddWithValue("@start", billingDate)
                    cmdInsert.Parameters.AddWithValue("@deadline", deadlineDate)
                    cmdInsert.Parameters.AddWithValue("@amount", fee)
                    cmdInsert.ExecuteNonQuery()
                End If
            Else
                reader.Close()
            End If
        End Using
    End Sub

    Private Sub StyleGrid()
        With dgvHistory
            .BorderStyle = BorderStyle.None
            .BackgroundColor = Color.White
            .RowHeadersVisible = False
            .AllowUserToAddRows = False
            .AllowUserToResizeRows = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
            .ReadOnly = True
            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(44, 62, 80)
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(44, 62, 80) ' Fix header blue highlight
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI Semibold", 9.75F)
            .ColumnHeadersHeight = 38
            .DefaultCellStyle.Font = New Font("Segoe UI", 10)
            .DefaultCellStyle.SelectionBackColor = .DefaultCellStyle.BackColor
            .DefaultCellStyle.SelectionForeColor = .DefaultCellStyle.ForeColor
            .AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250)
            .AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 249, 250)
            .AlternatingRowsDefaultCellStyle.SelectionForeColor = .DefaultCellStyle.ForeColor
            .GridColor = Color.FromArgb(222, 226, 230)
            .RowTemplate.Height = 35
            .Enabled = True 
            .ReadOnly = True
        End With
    End Sub

    Private Sub dgvHistory_SelectionChanged(sender As Object, e As EventArgs) Handles dgvHistory.SelectionChanged
        dgvHistory.ClearSelection()
    End Sub

    Private Sub LoadHistory()
        Using conn As New MySqlConnection(connString)
            conn.Open()

            ' Select DueDate, PaymentDate, Amount, and Status
            ' Filter: Only show if billing cycle has started OR it's already paid
            ' Use DATE() to ignore time components when comparing with CURDATE()
            Dim cmd As New MySqlCommand("
                SELECT DueDate, 
                       IF(PaymentDate IS NULL, '---', DATE_FORMAT(PaymentDate, '%M %d, %Y')) as PaymentDate, 
                       Amount, Status 
                FROM PaymentHistory 
                WHERE SubscriptionID = @sid 
                  AND (DATE(BillingCycleStart) <= CURDATE() OR Status = 'Paid')
                ORDER BY DueDate DESC", conn)
            cmd.Parameters.AddWithValue("@sid", subID)

            Dim dt As New DataTable()
            Dim da As New MySqlDataAdapter(cmd)
            da.Fill(dt)

            dgvHistory.DataSource = dt
            dgvHistory.ClearSelection()

            If dgvHistory.Columns.Contains("DueDate") Then
                dgvHistory.Columns("DueDate").HeaderText = "Due Date"
                dgvHistory.Columns("DueDate").DefaultCellStyle.Format = "MMMM dd, yyyy"
            End If

            If dgvHistory.Columns.Contains("PaymentDate") Then
                dgvHistory.Columns("PaymentDate").HeaderText = "Paid On"
                dgvHistory.Columns("PaymentDate").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            End If

            If dgvHistory.Columns.Contains("Amount") Then
                dgvHistory.Columns("Amount").DefaultCellStyle.Format = "₱#,##0.00"
            End If

            dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

            Dim today As Date = Date.Now.Date
            For Each row As DataGridViewRow In dgvHistory.Rows
                If row.IsNewRow Then Continue For

                Dim status = row.Cells("Status").Value?.ToString()
                Dim dueDateVal = row.Cells("DueDate").Value
                Dim dueDate As Date = If(IsDBNull(dueDateVal), Date.MinValue, Convert.ToDateTime(dueDateVal).Date)

                If status <> "Paid" Then
                    Dim daysUntilDue = (dueDate - today).Days
                    status = "Not Yet Paid"

                    If dueDate < today Then
                        row.Cells("Status").Style.ForeColor = Color.FromArgb(231, 76, 60) ' Red
                        row.Cells("Status").ToolTipText = "Overdue"
                    ElseIf daysUntilDue = 0 Then
                        row.Cells("Status").Style.ForeColor = Color.FromArgb(243, 156, 18) ' Yellow/Orange
                        row.Cells("Status").ToolTipText = "Due Today"
                    ElseIf daysUntilDue <= reminderDays Then
                        row.Cells("Status").Style.ForeColor = Color.FromArgb(46, 204, 113) ' Green
                        row.Cells("Status").ToolTipText = "Due Soon"
                    Else
                        row.Cells("Status").Style.ForeColor = Color.FromArgb(189, 195, 199) ' Light Gray
                        row.Cells("Status").ToolTipText = "Upcoming Payment"
                    End If

                    row.Cells("Status").Style.Font = New Font("Segoe UI", 10, FontStyle.Bold)
                    row.Cells("Status").Value = status
                Else
                    row.Cells("Status").Style.ForeColor = Color.FromArgb(39, 174, 96) ' Green
                    row.Cells("Status").Style.Font = New Font("Segoe UI", 10, FontStyle.Bold)
                    row.Cells("Status").ToolTipText = "Payment Completed"
                End If
            Next
        End Using
    End Sub



    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub
End Class
