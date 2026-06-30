
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
3. Configure email settings in `EmailSettings.vb`
4. Set up MySQL database (see `DATABASE_GUIDE.txt`)
5. Build and run!

## Documentation

- `DEPLOYMENT_GUIDE.txt` - Complete deployment instructions
- `DATABASE_GUIDE.txt` - Database management guide
- `database_backup.ps1` - Database backup script

## Database Management

**IMPORTANT:** Do NOT upload actual database files to GitHub!

- Use `database_backup.ps1` to create local backups
- Store schema/migration scripts in version control
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
