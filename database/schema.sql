-- Subscription Manager Database Schema
-- Version: 1.0

-- Create the database if it doesn't exist
CREATE DATABASE IF NOT EXISTS Subscription CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE Subscription;

-- User table
CREATE TABLE IF NOT EXISTS User (
    UserID INT AUTO_INCREMENT PRIMARY KEY,
    Email VARCHAR(255) NOT NULL UNIQUE,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Subscription table
CREATE TABLE IF NOT EXISTS Subscription (
    SubscriptionID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT NOT NULL,
    ServiceName VARCHAR(255) NOT NULL,
    NextBillingDate DATETIME NOT NULL,
    PaymentDeadline DATETIME NOT NULL,
    BillingType VARCHAR(20) NOT NULL, -- 'Monthly' or 'Yearly'
    SubscriptionFee DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    IsPinned BOOLEAN NOT NULL DEFAULT FALSE,
    ReminderDays INT NOT NULL DEFAULT 3,
    PaymentDeadlineDays INT NOT NULL DEFAULT 10,
    UseBankingDays BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserID) REFERENCES User(UserID) ON DELETE CASCADE,
    UNIQUE KEY unique_user_service (UserID, ServiceName)
);

-- PaymentHistory table
CREATE TABLE IF NOT EXISTS PaymentHistory (
    PaymentID INT AUTO_INCREMENT PRIMARY KEY,
    SubscriptionID INT NOT NULL,
    BillingCycleStart DATETIME NOT NULL,
    DueDate DATETIME NOT NULL,
    PaymentDate DATETIME NULL,
    Amount DECIMAL(10,2) NOT NULL,
    Status VARCHAR(50) NOT NULL DEFAULT 'Not Yet Paid', -- 'Not Yet Paid', 'Paid', 'Overdue'
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (SubscriptionID) REFERENCES Subscription(SubscriptionID) ON DELETE CASCADE,
    INDEX idx_subscription_due (SubscriptionID, DueDate)
);

-- Notification table
CREATE TABLE IF NOT EXISTS Notification (
    NotificationID INT AUTO_INCREMENT PRIMARY KEY,
    SubscriptionID INT NOT NULL,
    Type VARCHAR(50) NOT NULL, -- 'Reminder', 'Due', 'Overdue'
    DateSent DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (SubscriptionID) REFERENCES Subscription(SubscriptionID) ON DELETE CASCADE
);
