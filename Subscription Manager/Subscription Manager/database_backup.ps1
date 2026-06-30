
# Database Backup Script for Subscription Manager
# Usage: .\database_backup.ps1 -DatabaseName "SubscriptionManager" -Username "root" -Password "yourpassword"

param(
    [string]$DatabaseName = "SubscriptionManager",
    [string]$Username = "root",
    [string]$Password,
    [string]$BackupDir = ".\database_backups"
)

# Create backup directory if it doesn't exist
if (-not (Test-Path $BackupDir)) {
    New-Item -ItemType Directory -Path $BackupDir -Force | Out-Null
}

# Generate timestamp for backup file
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$backupFile = Join-Path $BackupDir "${DatabaseName}_${timestamp}.sql"

# Find mysqldump executable
$mysqldumpPath = $null
$possiblePaths = @(
    "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqldump.exe",
    "C:\Program Files\MySQL\MySQL Server 8.4\bin\mysqldump.exe",
    "C:\Program Files\MySQL\MySQL Workbench 8.0\bin\mysqldump.exe",
    "C:\Program Files\MySQL\MySQL Workbench 8.4\bin\mysqldump.exe"
)

foreach ($path in $possiblePaths) {
    if (Test-Path $path) {
        $mysqldumpPath = $path
        break
    }
}

# Try to find mysqldump in PATH if not found in common locations
if (-not $mysqldumpPath) {
    try {
        $mysqldumpPath = Get-Command mysqldump -ErrorAction Stop | Select-Object -ExpandProperty Source
    } catch {
        Write-Host "Error: mysqldump not found. Please install MySQL or add it to your PATH." -ForegroundColor Red
        exit 1
    }
}

Write-Host "Using mysqldump from: $mysqldumpPath" -ForegroundColor Green

# Build the mysqldump command
if ($Password) {
    $command = "& `"$mysqldumpPath`" --user=$Username --password=$Password --databases $DatabaseName > `"$backupFile`""
} else {
    $command = "& `"$mysqldumpPath`" --user=$Username --databases $DatabaseName > `"$backupFile`""
}

try {
    Write-Host "Creating database backup..." -ForegroundColor Yellow
    Invoke-Expression $command
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Backup successful! File saved to: $backupFile" -ForegroundColor Green
    } else {
        Write-Host "Backup failed with exit code: $LASTEXITCODE" -ForegroundColor Red
    }
} catch {
    Write-Host "Error during backup: $_" -ForegroundColor Red
}
