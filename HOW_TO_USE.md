# How to Use Subscription Manager

## Table of Contents
1. [First Time Setup](#first-time-setup)
2. [Adding a New Subscription](#adding-a-new-subscription)
3. [Viewing and Editing Subscriptions](#viewing-and-editing-subscriptions)
4. [Marking Subscriptions as Paid](#marking-subscriptions-as-paid)
5. [Calendar View](#calendar-view)
6. [Notifications](#notifications)
7. [Payment History](#payment-history)
8. [Tips and Tricks](#tips-and-tricks)

---

## First Time Setup

### 1. System Requirements
- Windows OS
- .NET 10.0 Desktop Runtime or later
- MySQL Server 8.0+ installed and running locally

### 2. Configure Email Settings
1. Open the project in Visual Studio
2. Copy `EmailSettings.vb.template` → `EmailSettings.vb`
3. Update these values in `EmailSettings.vb`:
   - `SMTP_EMAIL`: Your Gmail address (e.g., "you@gmail.com")
   - `SMTP_PASSWORD`: Your Gmail **app password** (not your regular password!)
   - `MAILBEE_LICENSE_KEY`: Your MailBee license key (or use the provided one if it works for you)

### 3. Get a Gmail App Password
If you have 2-Step Verification enabled on your Google account (which you should!):
1. Go to [myaccount.google.com/apppasswords](https://myaccount.google.com/apppasswords)
2. Sign in to your Google account
3. Select app: "Mail"
4. Select device: "Windows Computer"
5. Click "Generate"
6. Copy the 16-character password (spaces are okay!) and paste it into `SMTP_PASSWORD` in `EmailSettings.vb`

### 4. Run the App!
Just hit F5 or click "Start" in Visual Studio!
- The app will **automatically create the "Subscription" database and all required tables** if they don't exist yet
- The first time you run it, you'll see an empty dashboard - time to add your first subscription!

---

## Adding a New Subscription

1. Click the **"+ Add Subscription"** button at the top right
2. Fill in all the fields:
   - **Service Name**: What you're subscribing to (e.g., "Netflix", "Adobe Creative Cloud", "Spotify")
   - **Email**: Your email associated with this subscription (will be used for notifications about payments)
   - **Next Billing Date**: When your next payment is due (use the date picker)
   - **Deadline Days**: How many days after the billing date the payment is actually due (default: 10)
   - **Billing Type**: Monthly or Yearly
   - **Subscription Fee**: How much it costs (your currency will be shown as ₱, but you can treat it as any currency)
   - **Reminder Days**: How many days in advance to send you email notifications (default: 3)
   - **Use Banking Days**: Toggle this ON if you want the deadline to skip weekends and holidays
3. Click the **"Add Subscription"** or **"Save Changes"** button (depending on if you're adding or editing)

---

## Viewing and Editing Subscriptions

### Main Dashboard
The main screen shows all your active subscriptions in a table.

You can:
- **Search**: Use the search bar at the top to find subscriptions by service name
- **Filter**: Use the dropdowns to filter by status (All, Paid, Overdue, Due Today, Due Soon, Favorites, Paused) or billing type (Monthly, Yearly, All)
- **Pin**: Click the pin icon (⋮ menu → "Pin to Top") to keep important subscriptions at the top
- **Edit**: Click the "⋮" menu → "Edit" to modify subscription details
- **Pause**: Click "⋮" → "Pause Subscription" to stop notifications (un-pause the same way)
- **Delete**: Click "⋮" → "Delete Subscription" to remove it permanently (use this carefully!)

### Subscription Details
Click on any row in the table to see detailed information about that subscription!

---

## Marking Subscriptions as Paid

1. Go to the subscription details page (or use the "⋮" menu)
2. Click the **"MARK AS PAID"** button
3. Confirm when prompted
4. The app will:
   - Update the next billing date (add a month or year depending on your billing type)
   - Update the payment deadline
   - Add an entry to your payment history
   - Send you a confirmation email! (if email is configured)

---

## Calendar View

1. Click the **"📅 Calendar View"** button
2. Navigate through months using the left/right arrows
3. Click on any date to see subscriptions with payments due that day
4. This is a great way to see all your upcoming payments in one view!

---

## Notifications

### What Notifications Do You Get?
The app sends you **email reminders** automatically:
1. **Reminder**: X days before the payment is due (X = your Reminder Days setting)
2. **Due Today**: On the day the payment is due
3. **Overdue**: If you pass the deadline without marking it paid

### Notification Button
The bell icon (🔔) at the top shows a badge with how many notifications are pending!
- Red badge: Overdue payments are waiting
- Yellow badge: Payments due today
- Green badge: Payments coming up soon

---

## Payment History

1. Go to the details page for any subscription
2. Click the **"PAYMENT HISTORY"** button
3. This shows you all payments (and pending payments) for that subscription! Great for tracking your spending over time.

---

## Tips and Tricks

💡 **Organize your subscriptions**: Group similar services together in your mind, or use consistent naming conventions
💡 **Set reminder days wisely**: For important subscriptions, set 7 or 10 days instead of 3 to give yourself extra time
💡 **Use banking days**: Most companies don't process payments on weekends - turn this ON to avoid late fees!
💡 **Review often**: Check the dashboard at least once a week to stay on top of your payments
💡 **Backup your data**: Use the `database_backup.ps1` script occasionally to backup your database!

---

## Troubleshooting

### Can't connect to database?
- Make sure MySQL is running locally on your computer
- Check that your connection string in `Form1.vb` is correct
- Verify that the "Subscription" database exists (the app should create it for you!)

### Emails not sending?
- Double-check your `EmailSettings.vb`
- Make sure you're using an **app password** (not your regular Gmail password)
- Check your spam folder - sometimes notification emails end up there!
- Verify 2-Step Verification is enabled on your Google account

---

Need help? Check the `README.md` for more technical info!
