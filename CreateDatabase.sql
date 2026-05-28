-- =========================================================================
-- Secure Digital KYC Portal - Database Creation Script
-- Recommended for execution in SQL Server Management Studio (SSMS)
-- =========================================================================

-- 1. Create the Database if it does not exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'KYCDB')
BEGIN
    CREATE DATABASE KYCDB;
END
GO

USE KYCDB;
GO

-- 2. Create the main KYCDetails Table if it does not exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[KYCDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[KYCDetails] (
        -- Primary Key & Auto-Increment
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        
        -- SECTION 1: Basic Account Information
        [AccountType] NVARCHAR(50) NOT NULL,
        [CustomerType] NVARCHAR(50) NOT NULL,
        [PreferredBranch] NVARCHAR(100) NOT NULL,
        [ApplicationDate] DATE NOT NULL,
        
        -- SECTION 2: Contact & Verification
        [Email] NVARCHAR(150) NOT NULL,
        [EmailOTP] NVARCHAR(10) NOT NULL,
        [MobileNumber] NVARCHAR(15) NOT NULL,
        [AlternateMobileNumber] NVARCHAR(15) NULL,
        [AadhaarMobileOTP] NVARCHAR(10) NOT NULL,
        
        -- SECTION 3: Aadhaar Details
        [AadhaarNumber] NVARCHAR(20) NOT NULL,
        [AadhaarOTP] NVARCHAR(10) NOT NULL,
        [AadhaarName] NVARCHAR(100) NOT NULL,
        [AadhaarDOB] DATE NOT NULL,
        [Gender] NVARCHAR(20) NOT NULL,
        
        -- SECTION 4: Personal Information
        [FullLegalName] NVARCHAR(100) NOT NULL,
        [FatherName] NVARCHAR(100) NOT NULL,
        [MotherName] NVARCHAR(100) NOT NULL,
        [SpouseGuardianName] NVARCHAR(100) NOT NULL,
        [MaritalStatus] NVARCHAR(50) NOT NULL,
        [Nationality] NVARCHAR(50) NOT NULL,
        [Religion] NVARCHAR(50) NULL,
        [ResidentialStatus] NVARCHAR(50) NOT NULL,
        [PlaceOfBirth] NVARCHAR(100) NULL,
        [CountryOfBirth] NVARCHAR(100) NULL,
        
        -- SECTION 5: Address Details
        [StreetHouseLandmark] NVARCHAR(250) NOT NULL,
        [AreaLocality] NVARCHAR(150) NOT NULL,
        [LocationVillageTown] NVARCHAR(150) NOT NULL,
        [PostOffice] NVARCHAR(100) NOT NULL,
        [CityDistrict] NVARCHAR(100) NOT NULL,
        [State] NVARCHAR(100) NOT NULL,
        [Country] NVARCHAR(100) NOT NULL,
        [Pincode] NVARCHAR(10) NOT NULL,
        [TypeOfAddress] NVARCHAR(50) NOT NULL,
        [IsPermanentAddressSame] NVARCHAR(5) NOT NULL, -- "Yes" or "No"
        [PermanentAddress] NVARCHAR(500) NULL,
        
        -- SECTION 6: Employment & Financials
        [OccupationType] NVARCHAR(50) NOT NULL,
        [EmployerName] NVARCHAR(150) NULL,
        [NatureOfBusiness] NVARCHAR(150) NULL,
        [Designation] NVARCHAR(100) NULL,
        [AnnualIncomeRange] NVARCHAR(100) NOT NULL,
        [SourceOfFunds] NVARCHAR(100) NOT NULL,
        
        -- SECTION 7: Banking & ID Details
        [PANNumber] NVARCHAR(20) NOT NULL,
        [PANHolderName] NVARCHAR(100) NOT NULL,
        [DrivingLicenceNumber] NVARCHAR(30) NULL,
        [DrivingLicenceDOB] DATE NULL,
        [DrivingLicenceName] NVARCHAR(100) NULL,
        
        -- SECTION 8: Document Uploads (Stores local file paths/names)
        [AadhaarCardPath] NVARCHAR(500) NOT NULL,
        [PANCardPath] NVARCHAR(500) NOT NULL,
        [PassportDLPath] NVARCHAR(500) NULL,
        [AddressProofPath] NVARCHAR(500) NULL,
        [SignatureScanPath] NVARCHAR(500) NOT NULL,
        
        -- Metadata Timestamps
        [CreatedAt] DATETIME DEFAULT GETDATE(),
        
        -- Unique Constraints (Day 3 Business Validation Rules)
        CONSTRAINT UQ_Aadhaar UNIQUE ([AadhaarNumber]),
        CONSTRAINT UQ_PAN UNIQUE ([PANNumber])
    );
    
    PRINT 'KYCDetails table created successfully.';
END
ELSE
BEGIN
    PRINT 'KYCDetails table already exists.';
END
GO
