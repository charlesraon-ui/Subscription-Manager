Imports System.Threading.Tasks
Imports MailBee
Imports MailBee.SmtpMail

Public Module BusinessLogic

    Public Function AddBankingDays(startDate As Date, daysToAdd As Integer) As Date
        Dim currentDate As Date = startDate
        Dim addedDays As Integer = 0

        While addedDays < daysToAdd
            currentDate = currentDate.AddDays(1)
            If IsBankingDay(currentDate) Then
                addedDays += 1
            End If
        End While


        While Not IsBankingDay(currentDate)
            currentDate = currentDate.AddDays(1)
        End While

        Return currentDate
    End Function

    Public Function IsBankingDay(checkDate As Date) As Boolean

        Return checkDate.DayOfWeek <> DayOfWeek.Saturday AndAlso
               checkDate.DayOfWeek <> DayOfWeek.Sunday
    End Function

    Public Function GetBankingDaysBetween(startDate As Date, endDate As Date) As Integer

        Dim firstDate As Date = If(startDate < endDate, startDate, endDate).Date
        Dim lastDate As Date = If(startDate < endDate, endDate, startDate).Date

        If firstDate = lastDate Then Return 0

        Dim bankingDays As Integer = 0
        Dim current As Date = firstDate

        While current < lastDate
            current = current.AddDays(1)
            If IsBankingDay(current) Then
                bankingDays += 1
            End If
        End While

        Return bankingDays
    End Function

    ' ========================== EMAIL SYSTEM CONFIGURATION ==========================
    Public Async Function SendEmailAsync(toEmail As String, subject As String, body As String) As Task(Of Boolean)
        Return Await Task.Run(Function()
                                  Try
                                      MailBee.Global.LicenseKey = EmailSettings.MAILBEE_LICENSE_KEY

                                      Dim smtp As New Smtp()

                                      Dim server As New SmtpServer(EmailSettings.SMTP_HOST, EmailSettings.SMTP_EMAIL, EmailSettings.SMTP_PASSWORD)
                                      server.Port = EmailSettings.SMTP_PORT

                                      server.AuthMethods = AuthenticationMethods.Auto

                                      If EmailSettings.SMTP_PORT = 587 Then
                                          server.SslMode = MailBee.Security.SslStartupMode.UseStartTls
                                      ElseIf EmailSettings.SMTP_PORT = 465 Then
                                          server.SslMode = MailBee.Security.SslStartupMode.OnConnect
                                      End If

                                      smtp.SmtpServers.Add(server)

                                      smtp.From.Email = EmailSettings.SMTP_EMAIL
                                      smtp.To.Add(toEmail)
                                      smtp.Subject = subject
                                      smtp.BodyHtmlText = body

                                      smtp.Send()
                                      Return True
                                  Catch ex As MailBee.MailBeeException
                                      Debug.WriteLine("MailBee Specific Error: " & ex.Message)
                                      Return False
                                  Catch ex As Exception
                                      Debug.WriteLine("General SMTP Error: " & ex.Message)
                                      Return False
                                  End Try
                              End Function)
    End Function
End Module
