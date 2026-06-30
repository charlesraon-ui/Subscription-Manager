Imports MySql.Data.MySqlClient

Public Class CalendarView
    Public connString As String
    Dim dtSubscriptions As DataTable
    Private txtSearchCalendar As TextBox

    Private Sub CalendarView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupSearchControl()
        SetupGrid()
        LoadAllSubscriptions()
        HighlightDueDates()
        ShowPaymentsForDate(Date.Now.Date)

        lblInfo.Top = 75
        lblInfo.Left = 460
        lblInfo.AutoSize = True

        dgvDayPayments.Left = 460
        dgvDayPayments.Top = 115
        dgvDayPayments.Width = Me.ClientSize.Width - 480
    End Sub

    Private Sub SetupSearchControl()
        Dim lblSearch As New Label()
        lblSearch.Text = "Search Subscription:"
        lblSearch.AutoSize = True
        lblSearch.Location = New Point(20, 440)
        lblSearch.Font = New Font("Segoe UI", 9, FontStyle.Bold)
        lblSearch.ForeColor = Color.FromArgb(127, 140, 141)

        txtSearchCalendar = New TextBox()
        txtSearchCalendar.Location = New Point(160, 437)
        txtSearchCalendar.Width = 260
        txtSearchCalendar.Font = New Font("Segoe UI", 10)
        txtSearchCalendar.BorderStyle = BorderStyle.FixedSingle
        AddHandler txtSearchCalendar.TextChanged, AddressOf txtSearchCalendar_TextChanged

        Me.Controls.Add(lblSearch)
        Me.Controls.Add(txtSearchCalendar)

        customCalendar1.Top = 70
    End Sub

    Private Sub txtSearchCalendar_TextChanged(sender As Object, e As EventArgs)
        Dim searchText As String = txtSearchCalendar.Text.Trim().ToLower()

        If searchText = "" Then
            ShowPaymentsForDate(Date.Now.Date)
            Exit Sub
        End If

        If searchText.Length < 2 Then Exit Sub

        Dim foundMatch As Boolean = False
        For Each row As DataRow In dtSubscriptions.Rows
            Dim serviceMatch As Boolean = row("ServiceName").ToString().ToLower().Contains(searchText)

            If serviceMatch Then
                If Not IsDBNull(row("PaymentDeadline")) Then
                    Dim deadline As DateTime
                    If DateTime.TryParse(row("PaymentDeadline").ToString(), deadline) Then
                        customCalendar1.SetDate(deadline)
                        ShowPaymentsForDate(deadline)
                        foundMatch = True
                        Exit Sub
                    End If
                End If
            End If
        Next

        If Not foundMatch Then
            customCalendar1.SetDate(Date.Now.Date)
            ShowPaymentsForDate(Date.Now.Date)
        End If
    End Sub

    Private Sub SetupGrid()
        With dgvDayPayments
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            .SelectionMode = DataGridViewSelectionMode.CellSelect
            .RowHeadersVisible = False
            .ReadOnly = True
            .AllowUserToAddRows = False
            .BackgroundColor = Color.White
            .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            .GridColor = Color.FromArgb(242, 243, 244)

            .ColumnHeadersVisible = False
            .EnableHeadersVisualStyles = False

            .DefaultCellStyle.BackColor = Color.White
            .DefaultCellStyle.ForeColor = Color.FromArgb(44, 62, 80)
            .DefaultCellStyle.SelectionBackColor = Color.White
            .DefaultCellStyle.SelectionForeColor = Color.FromArgb(44, 62, 80)
            .DefaultCellStyle.Font = New Font("Segoe UI", 10)
            
            .RowTemplate.Height = 45
            .ShowCellToolTips = False
            .MultiSelect = False
            .Enabled = True

            .AllowUserToResizeColumns = False
            .AllowUserToResizeRows = False
        End With
    End Sub

    Private Sub dgvDayPayments_SelectionChanged(sender As Object, e As EventArgs) Handles dgvDayPayments.SelectionChanged
        dgvDayPayments.ClearSelection()
        dgvDayPayments.CurrentCell = Nothing
    End Sub

    Private Sub dgvDayPayments_RowEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgvDayPayments.RowEnter
        dgvDayPayments.CurrentCell = Nothing
    End Sub

    Private Sub dgvDayPayments_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvDayPayments.CellFormatting
        If e.RowIndex < 0 Then Exit Sub

        If dgvDayPayments.Columns(e.ColumnIndex).Name = "Fee" Then
            e.CellStyle.ForeColor = Color.FromArgb(52, 152, 219) ' Blue for fees
            e.CellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        End If
    End Sub

    Private Sub LoadAllSubscriptions()
        Using conn As New MySqlConnection(connString)
            conn.Open()
            Dim cmd As New MySqlCommand("
                SELECT s.ServiceName, s.NextBillingDate, s.PaymentDeadline, s.SubscriptionFee, s.BillingType, u.Email
                FROM Subscription s
                JOIN User u ON s.UserID = u.UserID
                WHERE s.IsActive = TRUE", conn)
            Dim da As New MySqlDataAdapter(cmd)
            dtSubscriptions = New DataTable()
            da.Fill(dtSubscriptions)
        End Using
    End Sub

    Private Sub HighlightDueDates()
        Dim deadlineDates As New List(Of Date)

        For Each row As DataRow In dtSubscriptions.Rows
            If Not IsDBNull(row("PaymentDeadline")) Then
                Dim dtDeadline As DateTime
                If DateTime.TryParse(row("PaymentDeadline").ToString(), dtDeadline) Then
                    deadlineDates.Add(dtDeadline.Date)
                End If
            End If
        Next

        customCalendar1.DeadlineDates = deadlineDates
    End Sub

    Private Sub customCalendar1_DateSelected(selectedDate As Date) Handles customCalendar1.DateSelected
        ShowPaymentsForDate(selectedDate)
    End Sub

    Private Sub customCalendar1_MonthChanged(newMonth As Date) Handles customCalendar1.MonthChanged
        HighlightDueDates()
    End Sub

    Private Sub ShowPaymentsForDate(selectedDate As Date)
        Dim filteredTable As New DataTable()
        filteredTable.Columns.Add("Service")
        filteredTable.Columns.Add("Fee")

        Dim entriesAdded As New HashSet(Of String)

        For Each row As DataRow In dtSubscriptions.Rows
            Dim serviceName As String = row("ServiceName").ToString()
            Dim feeStr As String = "₱" & Convert.ToDecimal(row("SubscriptionFee")).ToString("N2")

            If Not IsDBNull(row("PaymentDeadline")) Then
                Dim dtDead As DateTime
                If DateTime.TryParse(row("PaymentDeadline").ToString(), dtDead) AndAlso dtDead.Date = selectedDate Then
                    If Not entriesAdded.Contains(serviceName) Then
                        filteredTable.Rows.Add(serviceName, feeStr)
                        entriesAdded.Add(serviceName)
                    End If
                End If
            End If
        Next

        dgvDayPayments.DataSource = filteredTable

        If dgvDayPayments.Columns.Contains("Service") Then dgvDayPayments.Columns("Service").FillWeight = 70
        If dgvDayPayments.Columns.Contains("Fee") Then 
            dgvDayPayments.Columns("Fee").FillWeight = 30
            dgvDayPayments.Columns("Fee").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        End If

        Dim dateText As String = selectedDate.ToString("MMMM dd, yyyy")
        If filteredTable.Rows.Count = 0 Then
            lblInfo.Text = "No payments scheduled for " & dateText
            lblInfo.ForeColor = Color.FromArgb(127, 140, 141)
        Else
            lblInfo.Text = "Payments for " & dateText
            lblInfo.ForeColor = Color.FromArgb(44, 62, 80)
        End If
        lblInfo.Font = New Font("Segoe UI", 12, FontStyle.Bold)
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub
End Class
