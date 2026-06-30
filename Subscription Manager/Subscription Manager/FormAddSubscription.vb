Imports MySql.Data.MySqlClient
Public Class FormAddSubscription
    Public connString As String
    Public subID As Integer = 0
    Public isEdit As Boolean = False

    Public isResume As Boolean = False
    Public resumeServiceName As String = ""
    Public resumeEmail As String = ""

    Private Sub FormAddSubscription_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.BackColor = Color.FromArgb(245, 247, 250)

        ' Resume, New, or Edit
        If isResume Then
            ' RESUME SUBSCRIPTION
            lblAddSubscription.Text = "Resume Subscription"
            btnSave.Text = "Resume & Save"
            btnSave.BackColor = Color.FromArgb(46, 204, 113) ' Green 
            txtService.Text = resumeServiceName
            txtEmail.Text = resumeEmail

            txtService.ReadOnly = True
            txtEmail.ReadOnly = True
            numDeadlineDays.Value = 10
            numReminderDays.Value = 3
        ElseIf subID = 0 Then
            ' NEW SUBSCRIPTION
            lblAddSubscription.Text = "New Subscription"
            btnSave.Text = "Add Subscription"
            btnSave.BackColor = Color.FromArgb(52, 152, 219) ' Blue
            numDeadlineDays.Value = 10
            numReminderDays.Value = 3
        Else
            ' EDIT SUBSCRIPTION
            isEdit = True
            lblAddSubscription.Text = "Edit Subscription"
            btnSave.Text = "Save Changes"
            btnSave.BackColor = Color.FromArgb(241, 196, 15) ' Yellow 


            Using conn As New MySqlConnection(connString)
                conn.Open()

                Dim cmd As New MySqlCommand("
                SELECT s.ServiceName, u.Email, s.NextBillingDate,
                       s.PaymentDeadlineDays, s.BillingType, s.SubscriptionFee,
                       s.ReminderDays, s.UseBankingDays
                FROM Subscription s
                JOIN User u ON s.UserID = u.UserID
                WHERE s.SubscriptionID = @id", conn)

                cmd.Parameters.AddWithValue("@id", subID)

                Dim reader = cmd.ExecuteReader()

                If reader.Read() Then
                    txtService.Text = reader("ServiceName").ToString()
                    txtEmail.Text = reader("Email").ToString()
                    dtBilling.Value = Convert.ToDateTime(reader("NextBillingDate"))
                    numDeadlineDays.Value = Convert.ToInt32(reader("PaymentDeadlineDays"))
                    cboType.Text = reader("BillingType").ToString()
                    txtFee.Text = reader("SubscriptionFee").ToString()
                    numReminderDays.Value = Convert.ToInt32(reader("ReminderDays"))
                    chkUseBankingDays.Checked = If(IsDBNull(reader("UseBankingDays")), True, Convert.ToBoolean(reader("UseBankingDays")))
                End If
            End Using
        End If

        With btnSave
            .FlatStyle = FlatStyle.Flat
            .FlatAppearance.BorderSize = 0
            .ForeColor = Color.White
            .Font = New Font("Segoe UI", 11, FontStyle.Bold)
            .Cursor = Cursors.Hand
        End With

        Dim inputs() As Control = {txtService, txtEmail, txtFee, cboType, dtBilling, numDeadlineDays, numReminderDays}
        For Each ctrl In inputs
            ctrl.Font = New Font("Segoe UI", 10)
        Next
    End Sub

    ' Save button (processes add, edit, or resume)
    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

        btnSave.Enabled = False
        btnSave.FlatStyle = FlatStyle.Flat
        btnSave.FlatAppearance.BorderSize = 0

        If isResume Then
            btnSave.Text = "Resuming..."
            btnSave.BackColor = Color.FromArgb(46, 204, 113)
        ElseIf subID = 0 Then
            btnSave.Text = "Adding..."
            btnSave.BackColor = Color.FromArgb(52, 152, 219)
        Else
            btnSave.Text = "Updating..."
            btnSave.BackColor = Color.FromArgb(241, 196, 15)
        End If

        Application.DoEvents()

        ' 1. Basic Validation
        If txtService.Text.Trim = "" Or txtEmail.Text.Trim = "" Or cboType.Text.Trim = "" Then
            MessageBox.Show("Please fill all fields!", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            ResetButton()
            Exit Sub
        End If

        ' 2. Email Validation
        Dim email As String = txtEmail.Text.Trim().ToLower()
        Dim emailPattern As String = "^[a-z0-9](\.?[a-z0-9]){2,}@gmail\.com$"
        If Not System.Text.RegularExpressions.Regex.IsMatch(email, emailPattern) Then
            MessageBox.Show("Please enter a valid Gmail address (e.g., example@gmail.com).", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            ResetButton()
            Exit Sub
        End If

        ' 3. Fee Validation
        Dim fee As Decimal = 0
        If Not Decimal.TryParse(txtFee.Text, fee) Then
            MessageBox.Show("Please enter a valid amount for the fee.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            ResetButton()
            Exit Sub
        End If

        ' 4. Deadline Calculation
        Dim deadlineDays As Integer = CInt(numDeadlineDays.Value)
        Dim deadlineDate As Date
        If chkUseBankingDays.Checked Then
            deadlineDate = BusinessLogic.AddBankingDays(dtBilling.Value, deadlineDays)
        Else
            deadlineDate = dtBilling.Value.AddDays(deadlineDays)
        End If

        ' 5. Database Persistence
        Try
            Using conn As New MySqlConnection(connString)
                conn.Open()

                ' --- ADD NEW or RESUME PAUSED ---
                If subID = 0 Or isResume Then
                    Dim userID As Integer = 0
                    Dim checkUserCmd As New MySqlCommand("SELECT UserID FROM User WHERE Email = @email", conn)
                    checkUserCmd.Parameters.AddWithValue("@email", txtEmail.Text)
                    Dim result = checkUserCmd.ExecuteScalar()

                    If result IsNot Nothing Then
                        userID = Convert.ToInt32(result)
                    Else
                        Dim userCmd As New MySqlCommand("
                        INSERT INTO User (Email)
                        VALUES (@email);
                        SELECT LAST_INSERT_ID();", conn)

                        userCmd.Parameters.AddWithValue("@email", txtEmail.Text)
                        userID = Convert.ToInt32(userCmd.ExecuteScalar())
                    End If

                    Dim subCmd As New MySqlCommand("
                    INSERT INTO Subscription
                    (UserID, ServiceName, NextBillingDate, PaymentDeadline, BillingType, SubscriptionFee, IsActive, IsPinned, ReminderDays, PaymentDeadlineDays, UseBankingDays)
                    VALUES (@uid, @service, @date, @deadline, @type, @fee, @active, FALSE, @reminderDays, @deadlineDays, @useBanking)
                    ON DUPLICATE KEY UPDATE 
                    NextBillingDate=@date, PaymentDeadline=@deadline, BillingType=@type, SubscriptionFee=@fee, IsActive=@active, ReminderDays=@reminderDays, PaymentDeadlineDays=@deadlineDays, UseBankingDays=@useBanking", conn)

                    subCmd.Parameters.AddWithValue("@uid", userID)
                    subCmd.Parameters.AddWithValue("@service", txtService.Text)
                    subCmd.Parameters.AddWithValue("@date", dtBilling.Value)
                    subCmd.Parameters.AddWithValue("@deadline", deadlineDate)
                    subCmd.Parameters.AddWithValue("@type", cboType.Text)
                    subCmd.Parameters.AddWithValue("@fee", fee)
                    subCmd.Parameters.AddWithValue("@active", True)
                    subCmd.Parameters.AddWithValue("@reminderDays", CInt(numReminderDays.Value))
                    subCmd.Parameters.AddWithValue("@deadlineDays", deadlineDays)
                    subCmd.Parameters.AddWithValue("@useBanking", chkUseBankingDays.Checked)

                    subCmd.ExecuteNonQuery()

                    Dim lastID As Integer
                    If subID = 0 Or isResume Then
                        Dim getIDCmd As New MySqlCommand("SELECT LAST_INSERT_ID()", conn)
                        lastID = Convert.ToInt32(getIDCmd.ExecuteScalar())

                        If lastID = 0 Then
                            Dim fetchIDCmd As New MySqlCommand("SELECT SubscriptionID FROM Subscription WHERE ServiceName = @service AND UserID = @uid", conn)
                            fetchIDCmd.Parameters.AddWithValue("@service", txtService.Text)
                            fetchIDCmd.Parameters.AddWithValue("@uid", userID)
                            lastID = Convert.ToInt32(fetchIDCmd.ExecuteScalar())
                        End If
                    Else
                        lastID = subID
                    End If

                    Dim checkPHCmd As New MySqlCommand("
                        SELECT COUNT(*) FROM PaymentHistory 
                        WHERE SubscriptionID = @sid AND DueDate = @deadline", conn)
                    checkPHCmd.Parameters.AddWithValue("@sid", lastID)
                    checkPHCmd.Parameters.AddWithValue("@deadline", deadlineDate)

                    If Convert.ToInt32(checkPHCmd.ExecuteScalar()) = 0 Then
                        Dim insertPHCmd As New MySqlCommand("
                            INSERT INTO PaymentHistory (SubscriptionID, BillingCycleStart, DueDate, Amount, Status)
                            VALUES (@sid, @start, @deadline, @amount, 'Not Yet Paid')", conn)
                        insertPHCmd.Parameters.AddWithValue("@sid", lastID)
                        insertPHCmd.Parameters.AddWithValue("@start", dtBilling.Value)
                        insertPHCmd.Parameters.AddWithValue("@deadline", deadlineDate)
                        insertPHCmd.Parameters.AddWithValue("@amount", fee)
                        insertPHCmd.ExecuteNonQuery()
                    Else
                        Dim updatePHCmd As New MySqlCommand("
                            UPDATE PaymentHistory 
                            SET DueDate = @deadline, Amount = @fee, BillingCycleStart = @start
                            WHERE SubscriptionID = @sid AND Status <> 'Paid'
                            ORDER BY DueDate DESC LIMIT 1", conn)
                        updatePHCmd.Parameters.AddWithValue("@sid", lastID)
                        updatePHCmd.Parameters.AddWithValue("@deadline", deadlineDate)
                        updatePHCmd.Parameters.AddWithValue("@amount", fee)
                        updatePHCmd.Parameters.AddWithValue("@start", dtBilling.Value)
                        updatePHCmd.ExecuteNonQuery()
                    End If

                Else
                    ' --- EDIT EXISTING ---
                    Dim userCmd As New MySqlCommand("
                    UPDATE User u
                    JOIN Subscription s ON u.UserID = s.UserID
                    SET u.Email = @email
                    WHERE s.SubscriptionID = @id", conn)

                    userCmd.Parameters.AddWithValue("@email", txtEmail.Text)
                    userCmd.Parameters.AddWithValue("@id", subID)
                    userCmd.ExecuteNonQuery()

                    Dim cmd As New MySqlCommand("
                    UPDATE Subscription
                    SET ServiceName=@service,
                        NextBillingDate=@date,
                        PaymentDeadline=@deadline,
                        BillingType=@type,
                        SubscriptionFee=@fee,
                        ReminderDays=@reminderDays,
                        PaymentDeadlineDays=@deadlineDays,
                        UseBankingDays=@useBanking
                    WHERE SubscriptionID=@id", conn)

                    cmd.Parameters.AddWithValue("@service", txtService.Text)
                    cmd.Parameters.AddWithValue("@date", dtBilling.Value)
                    cmd.Parameters.AddWithValue("@deadline", deadlineDate)
                    cmd.Parameters.AddWithValue("@type", cboType.Text)
                    cmd.Parameters.AddWithValue("@fee", fee)
                    cmd.Parameters.AddWithValue("@reminderDays", CInt(numReminderDays.Value))
                    cmd.Parameters.AddWithValue("@deadlineDays", deadlineDays)
                    cmd.Parameters.AddWithValue("@useBanking", chkUseBankingDays.Checked)
                    cmd.Parameters.AddWithValue("@id", subID)

                    cmd.ExecuteNonQuery()

                    ' --- SYNC PAYMENT HISTORY ON EDIT ---
                    ' When editing, we want to ensure only ONE "Not Yet Paid" record exists 
                    ' and it matches the new dates.

                    ' 1. Delete any existing "Not Yet Paid" records for this subscription
                    ' This prevents duplicates if the billing date was changed.
                    Dim cmdDeleteOld As New MySqlCommand("
                        DELETE FROM PaymentHistory 
                        WHERE SubscriptionID = @id AND Status = 'Not Yet Paid'", conn)
                    cmdDeleteOld.Parameters.AddWithValue("@id", subID)
                    cmdDeleteOld.ExecuteNonQuery()

                    ' 2. Insert the single correct "Not Yet Paid" record for the new dates
                    Dim cmdInsertPH As New MySqlCommand("
                        INSERT INTO PaymentHistory (SubscriptionID, BillingCycleStart, DueDate, Amount, Status)
                        VALUES (@id, @start, @deadline, @fee, 'Not Yet Paid')", conn)
                    cmdInsertPH.Parameters.AddWithValue("@id", subID)
                    cmdInsertPH.Parameters.AddWithValue("@start", dtBilling.Value.Date)
                    cmdInsertPH.Parameters.AddWithValue("@deadline", deadlineDate)
                    cmdInsertPH.Parameters.AddWithValue("@fee", fee)
                    cmdInsertPH.ExecuteNonQuery()
                End If
            End Using

            btnSave.BackColor = Color.FromArgb(46, 204, 113)
            btnSave.Text = "✔ Saved"
            Application.DoEvents()
            Threading.Thread.Sleep(700)

            Me.DialogResult = DialogResult.OK
            Me.Close()

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
            ResetButton()
        End Try

    End Sub

    ' Restores the button to its original state 
    Private Sub ResetButton()
        btnSave.Enabled = True
        If isResume Then
            btnSave.Text = "Resume & Save"
            btnSave.BackColor = Color.FromArgb(46, 204, 113)
        ElseIf isEdit Then
            btnSave.Text = "Save Changes"
            btnSave.BackColor = Color.FromArgb(241, 196, 15)
        Else
            btnSave.Text = "Add Subscription"
            btnSave.BackColor = Color.FromArgb(52, 152, 219)
        End If
    End Sub

    ' Darken button on mouse enter
    Private Sub btnSave_MouseEnter(sender As Object, e As EventArgs) Handles btnSave.MouseEnter
        If Not btnSave.Enabled Then Exit Sub
        If isResume Then
            btnSave.BackColor = Color.FromArgb(39, 174, 96)
        ElseIf isEdit Then
            btnSave.BackColor = Color.FromArgb(211, 171, 13)
        Else
            btnSave.BackColor = Color.FromArgb(41, 128, 185)
        End If
    End Sub

    ' Restore color on mouse leave
    Private Sub btnSave_MouseLeave(sender As Object, e As EventArgs) Handles btnSave.MouseLeave
        If Not btnSave.Enabled Then Exit Sub
        If isResume Then
            btnSave.BackColor = Color.FromArgb(46, 204, 113)
        ElseIf isEdit Then
            btnSave.BackColor = Color.FromArgb(241, 196, 15)
        Else
            btnSave.BackColor = Color.FromArgb(52, 152, 219)
        End If
    End Sub

    Private Sub chkUseBankingDays_CheckedChanged(sender As Object, e As EventArgs) Handles chkUseBankingDays.CheckedChanged

    End Sub

End Class
