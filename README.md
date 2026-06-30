
# Subscription Manager

A Windows Forms application for managing subscriptions, tracking payments, and sending email notifications.

## Features

- Subscription management
- Payment history tracking
- Calendar view
- Email notifications via Gmail SMTP
- Payment deadline alerts

## Prerequisites

- .NET 10.0 Desktop Runtime
- MySQL Server 8.0+
- Gmail account with 2-Step Verification enabled

## Quick Start

1. Clone this repository
2. Open the solution in Visual Studio
3. **IMPORTANT:** Configure your email settings:
   - Copy `EmailSettings.vb.template` to `EmailSettings.vb`
   - Update the SMTP credentials with your own Gmail account and app password
   - Or use your own MailBee license key
4. Set up MySQL database:
   - Create the database and tables using `database/schema.sql`
   - Or let the app automatically initialize the database on first run
5. Build and run!

## Database Setup

**Option 1: Manual Setup (Recommended)**

1. Open MySQL Workbench or your preferred MySQL client
2. Execute the `database/schema.sql` file to create the database and tables

**Option 2: Automatic Setup**

1. Just run the application!
2. The app will automatically create the database and tables if they don't exist

## Documentation

- `DEPLOYMENT_GUIDE.txt` - Complete deployment instructions
- `DATABASE_GUIDE.txt` - Database management guide
- `database_backup.ps1` - Database backup script
- `database/schema.sql` - Full database schema for version control

## Database Management

**IMPORTANT:** Do NOT upload actual database files or sensitive data to GitHub!

- Use `database_backup.ps1` to create local backups
- Store schema/migration scripts in the `database/` directory (version controlled)
- See `DATABASE_GUIDE.txt` for detailed instructions

## Pushing to GitHub

1. Create the repository on GitHub first: https://github.com/new
2. Name it `Subscription-Manager` (or your preferred name)
3. Choose "Public" or "Private"
4. DO NOT initialize with README, .gitignore, or license
5. Click "Create repository"

Then run these commands (already done locally):
```bash
git remote add origin https://github.com/charlesraon-ui/Subscription-Manager.git
git branch -M main
git push -u origin main
```

## License

This project uses MailBee.NET and MySql.Data NuGet packages.
