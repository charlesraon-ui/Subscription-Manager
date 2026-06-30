Imports MySql.Data.MySqlClient
Imports System.Net
Imports System.Net.Mail

Public Class SubscriptionDetails
    Public connString As String
    Public subID As Integer

    Dim serviceName As String
    Dim email As String
    Dim billingType As String
    Dim fee As Decimal
    Dim currentBillingDate As Date
    Dim currentDeadline As Date
    Dim isPinned As Boolean
    Dim reminderDays As Integer
    Dim deadlineDays As Integer
    Dim isActive As Boolean
    Dim useBankingDays As Boolean

    Private Sub SubscriptionDetails_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadSubscriptionDetails()
    End Sub

    Private Sub LoadSubscriptionDetails()
        Using conn As New MySqlConnection(connString)
            conn.Open()

            Dim cmd As New MySqlCommand("
                SELECT s.ServiceName, u.Email, s.BillingType, s.SubscriptionFee, s.NextBillingDate, s.PaymentDeadline,
                       s.IsPinned, s.ReminderDays, s.PaymentDeadlineDays, s.IsActive, s.UseBankingDays,
                       (SELECT Status FROM PaymentHistory 
                        WHERE SubscriptionID = s.SubscriptionID 
                        AND DATE(BillingCycleStart) = DATE(s.NextBillingDate) 
                        ORDER BY PaymentID DESC LIMIT 1) as CurrentCycleStatus
                FROM Subscription s
                JOIN User u ON s.UserID = u.UserID
                WHERE s.SubscriptionID = @id", conn)
            cmd.Parameters.AddWithValue("@id", subID)

            Dim reader = cmd.ExecuteReader()
            If reader.Read() Then
                serviceName = reader("ServiceName").ToString()
                email = reader("Email").ToString()
                billingType = reader("BillingType").ToString()
                fee = Convert.ToDecimal(reader("SubscriptionFee"))
                currentBillingDate = Convert.ToDateTime(reader("NextBillingDate"))
                currentDeadline = Convert.ToDateTime(reader("PaymentDeadline"))
                isPinned = Convert.ToBoolean(reader("IsPinned"))
                reminderDays = Convert.ToInt32(reader("ReminderDays"))
                deadlineDays = Convert.ToInt32(reader("PaymentDeadlineDays"))
                isActive = Convert.ToBoolean(reader("IsActive"))
                useBankingDays = If(IsDBNull(reader("UseBankingDays")), True, Convert.ToBoolean(reader("UseBankingDays")))
                Dim cycleStatus As String = If(IsDBNull(reader("CurrentCycleStatus")), "", reader("CurrentCycleStatus").ToString())

                ' Update UI labels with the fetched data
                lblServiceName.Text = "Service: " & serviceName
                lblEmail.Text = "Email: " & email
                lblType.Text = "Type: " & billingType
                lblFee.Text = "Fee: ₱" & fee.ToString("N2")

                If Not isActive Then
                    lblNextBilling.Text = "Next Billing: Paused"
                    lblNextBilling.ForeColor = Color.Gray
                    lblDeadline.Text = "Deadline: Paused"
                    lblDeadline.ForeColor = Color.Gray
                Else
                    lblNextBilling.Text = "Next Billing: " & currentBillingDate.ToString("MMMM dd, yyyy")
                    lblDeadline.Text = "Deadline: " & currentDeadline.ToString("MMMM dd, yyyy")

                    Dim today As Date = Date.Now.Date
                    If cycleStatus = "Paid" Then
                        lblNextBilling.ForeColor = Color.FromArgb(39, 174, 96) ' Green (Paid)
                        lblDeadline.ForeColor = Color.FromArgb(39, 174, 96)
                    ElseIf currentDeadline < today Then
                        lblNextBilling.ForeColor = Color.FromArgb(231, 76, 60) ' Red (Overdue)
                        lblDeadline.ForeColor = Color.FromArgb(231, 76, 60)
                    ElseIf currentDeadline = today Then
                        lblNextBilling.ForeColor = Color.FromArgb(243, 156, 18) ' Orange (Due Today)
                        lblDeadline.ForeColor = Color.FromArgb(243, 156, 18)
                    ElseIf (currentDeadline - today).Days <= reminderDays Then
                        lblNextBilling.ForeColor = Color.FromArgb(41, 128, 185) ' Blue (Due Soon)
                        lblDeadline.ForeColor = Color.FromArgb(41, 128, 185)
                    Else
                        lblNextBilling.ForeColor = Color.Black
                        lblDeadline.ForeColor = Color.Black
                    End If
                End If

                ' Mark as Paid or Resume
                If Not isActive Then
                    btnPaid.Text = "RESUME"
                    btnPaid.BackColor = Color.FromArgb(46, 204, 113)
                    btnPaid.ForeColor = Color.White
                    btnPaid.Enabled = True
                Else
                    If cycleStatus = "Paid" Then
                        btnPaid.Text = "✔ PAID"
                        btnPaid.Enabled = False
                        btnPaid.BackColor = Color.FromArgb(39, 174, 96)
                        btnPaid.ForeColor = Color.White
                    ElseIf currentBillingDate.Date > Date.Now.Date Then
                        btnPaid.Text = "Mark as Paid"
                        btnPaid.Enabled = False
                        btnPaid.BackColor = Color.LightGray
                        btnPaid.ForeColor = Color.DarkGray
                    Else
                        btnPaid.Text = "Mark as Paid"
                        btnPaid.Enabled = True
                        btnPaid.BackColor = Color.FromArgb(52, 152, 219)
                        btnPaid.ForeColor = Color.White
                    End If
                End If
            End If
        End Using
    End Sub

    ' Click Mark as Paid or Resume button
    Private Sub btnPaid_Click(sender As Object, e As EventArgs) Handles btnPaid.Click
        ' Subscription is paused, open the Add form to resume it
        If Not isActive Then
            Dim frm As New FormAddSubscription()
            frm.connString = connString
            frm.isResume = True
            frm.resumeServiceName = serviceName
            frm.resumeEmail = email

            If frm.ShowDialog() = DialogResult.OK Then
                Me.DialogResult = DialogResult.OK
                Me.Close()
            End If
            Exit Sub
        End If

        ' Subscription is active, process the payment
        Dim confirm = MessageBox.Show("Confirm payment for " & serviceName & "?", "Confirm Payment", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If confirm = DialogResult.Yes Then
            ' Calculate next billing date based on cycle
            Dim newBillingDate As Date
            If billingType = "Monthly" Then
                newBillingDate = currentBillingDate.AddMonths(1)
            Else
                newBillingDate = currentBillingDate.AddYears(1)
            End If

            ' Calculate next payment deadline
            Dim newDeadline As Date
            If useBankingDays Then
                newDeadline = BusinessLogic.AddBankingDays(newBillingDate, deadlineDays)
            Else
                newDeadline = newBillingDate.AddDays(deadlineDays)
            End If

            Using conn As New MySqlConnection(connString)
                conn.Open()

                ' 1. Update the existing record for the current cycle to "Paid"
                Dim cmdUpdatePH As New MySqlCommand("
                    UPDATE PaymentHistory 
                    SET Status = 'Paid', PaymentDate = NOW() 
                    WHERE SubscriptionID = @sid AND Status <> 'Paid'
                    ORDER BY ABS(DATEDIFF(DueDate, @due)) ASC LIMIT 1", conn)
                cmdUpdatePH.Parameters.AddWithValue("@sid", subID)
                cmdUpdatePH.Parameters.AddWithValue("@due", currentDeadline)
                Dim rowsAffected = cmdUpdatePH.ExecuteNonQuery()

                If rowsAffected = 0 Then
                    Dim cmdInsertPaid As New MySqlCommand("
                        INSERT INTO PaymentHistory (SubscriptionID, DueDate, PaymentDate, Amount, Status)
                        VALUES (@sid, @due, NOW(), @amount, 'Paid')", conn)
                    cmdInsertPaid.Parameters.AddWithValue("@sid", subID)
                    cmdInsertPaid.Parameters.AddWithValue("@due", currentDeadline)
                    cmdInsertPaid.Parameters.AddWithValue("@amount", fee)
                    cmdInsertPaid.ExecuteNonQuery()
                End If

                ' 2. Update subscription with new dates
                Dim cmdUpdate As New MySqlCommand("
                    UPDATE Subscription
                    SET NextBillingDate=@bill, PaymentDeadline=@deadline
                    WHERE SubscriptionID=@id", conn)
                cmdUpdate.Parameters.AddWithValue("@bill", newBillingDate)
                cmdUpdate.Parameters.AddWithValue("@deadline", newDeadline)
                cmdUpdate.Parameters.AddWithValue("@id", subID)
                cmdUpdate.ExecuteNonQuery()

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

            ' Send a confirmation email to the user
            SendPaymentEmail(newBillingDate)

            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If
    End Sub

    ' Composes and sends a payment confirmation email
    Private Async Sub SendPaymentEmail(newDate As Date)
        Dim body As String = "
            <div style='font-family: Arial, sans-serif; padding:20px; background-color:#eafaf1;'>
                <div style='max-width:600px; margin:auto; background:white; padding:25px; border-radius:10px;'>
                    <h2 style='color:#27ae60;'>✅ Payment Successful</h2>
                    <p>Hello,</p>
                    <p>Your payment has been <b style='color:#27ae60;'>successfully processed</b>. Thank you for staying subscribed!</p>
                    <hr>
                    <p><b>Service:</b> " & serviceName & "</p>
                    <p><b>Amount Paid:</b> <span style='color:#27ae60; font-size:16px;'>₱" & fee.ToString("N2") & "</span></p>
                    <p><b>Next Billing Date:</b> " & newDate.ToString("MMMM dd, yyyy") & "</p>
                    <hr>
                    <p style='color:#555;'>Your subscription remains active. No further action is needed at this time.</p>
                    <p style='margin-top:20px;'>Thank you,<br><b>Subscription Manager</b></p>
                </div>
            </div>
            "

        Dim success As Boolean = Await BusinessLogic.SendEmailAsync(email, "Payment Successfully Recorded", body)

        If Not success Then
            Debug.WriteLine("Email could not be sent via current provider.")
        End If
    End Sub

    ' Click Close button
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    ' Click History button to view past payments
    Private Sub btnHistory_Click(sender As Object, e As EventArgs) Handles btnHistory.Click
        Dim frm As New PaymentHistory()
        frm.connString = connString
        frm.subID = subID
        frm.serviceName = serviceName
        frm.ShowDialog()
    End Sub
End Class
