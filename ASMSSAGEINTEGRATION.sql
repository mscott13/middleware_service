CREATE DATABASE [ASMSSAGEINTEGRATION]
GO

USE [ASMSSAGEINTEGRATION]
GO
/****** Object:  User [NT AUTHORITY\IUSR]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [NT AUTHORITY\IUSR] FOR LOGIN [NT AUTHORITY\IUSR]
GO
/****** Object:  User [NT AUTHORITY\NETWORK SERVICE]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [NT AUTHORITY\NETWORK SERVICE] FOR LOGIN [NT AUTHORITY\NETWORK SERVICE] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [NT AUTHORITY\SYSTEM]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [NT AUTHORITY\SYSTEM] FOR LOGIN [NT AUTHORITY\SYSTEM] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [NT SERVICE\MSSQL$ASMSDEV]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [NT SERVICE\MSSQL$ASMSDEV] FOR LOGIN [NT Service\MSSQL$ASMSDEV]
GO
/****** Object:  User [NT Service\MSSQL$TCIASMS]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [NT Service\MSSQL$TCIASMS] FOR LOGIN [NT SERVICE\MSSQL$TCIASMS] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [NT SERVICE\SQLAgent$TCIASMS]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [NT SERVICE\SQLAgent$TCIASMS] FOR LOGIN [NT SERVICE\SQLAgent$TCIASMS] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [NT SERVICE\SQLTELEMETRY$TCIASMS]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [NT SERVICE\SQLTELEMETRY$TCIASMS] FOR LOGIN [NT SERVICE\SQLTELEMETRY$TCIASMS] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [SMA\ASMS-ACCPAC-1]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [SMA\ASMS-ACCPAC-1] FOR LOGIN [SMAGOVJM\ASMS-ACCPAC-1] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [SMA\ASMS-ACCPAC-2]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [SMA\ASMS-ACCPAC-2] FOR LOGIN [SMAGOVJM\ASMS-ACCPAC-2] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [SMA\ASMS-ACCPAC-2$]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [SMA\ASMS-ACCPAC-2$] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [SMA\ASMSUsers]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [SMA\ASMSUsers] FOR LOGIN [SMAGOVJM\ASMSUsers]
GO
/****** Object:  User [SMA\SMA-DBSRV$]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [SMA\SMA-DBSRV$] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [SMA\SMA-DT-SPARE-01$]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [SMA\SMA-DT-SPARE-01$] WITH DEFAULT_SCHEMA=[SMA\SMA-DT-SPARE-01$]
GO
/****** Object:  User [SMAGOVJM\ASMS-ACCPAC-1]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [SMAGOVJM\ASMS-ACCPAC-1] FOR LOGIN [SMAGOVJM\ASMS-ACCPAC-1] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [SMAGOVJM\ASMS-ACCPAC-1$]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [SMAGOVJM\ASMS-ACCPAC-1$] FOR LOGIN [SMAGOVJM\asms-accpac-1$] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [SMAGOVJM\ASMS-ACCPAC-2]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [SMAGOVJM\ASMS-ACCPAC-2] FOR LOGIN [SMAGOVJM\ASMS-ACCPAC-2] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [smagovjm\asms-accpac-2$]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [smagovjm\asms-accpac-2$] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [SMAGOVJM\ASMSAdministrators]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [SMAGOVJM\ASMSAdministrators] FOR LOGIN [SMAGOVJM\ASMSAdministrators]
GO
/****** Object:  User [SMAGOVJM\ASMSUsers]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [SMAGOVJM\ASMSUsers] FOR LOGIN [SMAGOVJM\ASMSUsers]
GO
/****** Object:  User [SMAGOVJM\cgriffith]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [SMAGOVJM\cgriffith] FOR LOGIN [SMAGOVJM\cgriffith] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [SMAGOVJM\mscott]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [SMAGOVJM\mscott] FOR LOGIN [SMAGOVJM\mscott] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [SMAGOVJM\Sebanks]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [SMAGOVJM\Sebanks] FOR LOGIN [SMAGOVJM\sebanks] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [SMAGOVJM\SERVER-ERP2$]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE USER [SMAGOVJM\SERVER-ERP2$] FOR LOGIN [SMAGOVJM\SERVER-ERP2$] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  DatabaseRole [aspnet_Membership_BasicAccess]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE ROLE [aspnet_Membership_BasicAccess]
GO
/****** Object:  DatabaseRole [aspnet_Membership_FullAccess]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE ROLE [aspnet_Membership_FullAccess]
GO
/****** Object:  DatabaseRole [aspnet_Membership_ReportingAccess]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE ROLE [aspnet_Membership_ReportingAccess]
GO
/****** Object:  DatabaseRole [aspnet_Personalization_BasicAccess]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE ROLE [aspnet_Personalization_BasicAccess]
GO
/****** Object:  DatabaseRole [aspnet_Personalization_FullAccess]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE ROLE [aspnet_Personalization_FullAccess]
GO
/****** Object:  DatabaseRole [aspnet_Personalization_ReportingAccess]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE ROLE [aspnet_Personalization_ReportingAccess]
GO
/****** Object:  DatabaseRole [aspnet_Profile_BasicAccess]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE ROLE [aspnet_Profile_BasicAccess]
GO
/****** Object:  DatabaseRole [aspnet_Profile_FullAccess]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE ROLE [aspnet_Profile_FullAccess]
GO
/****** Object:  DatabaseRole [aspnet_Profile_ReportingAccess]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE ROLE [aspnet_Profile_ReportingAccess]
GO
/****** Object:  DatabaseRole [aspnet_Roles_BasicAccess]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE ROLE [aspnet_Roles_BasicAccess]
GO
/****** Object:  DatabaseRole [aspnet_Roles_FullAccess]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE ROLE [aspnet_Roles_FullAccess]
GO
/****** Object:  DatabaseRole [aspnet_Roles_ReportingAccess]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE ROLE [aspnet_Roles_ReportingAccess]
GO
/****** Object:  DatabaseRole [aspnet_WebEvent_FullAccess]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE ROLE [aspnet_WebEvent_FullAccess]
GO
/****** Object:  DatabaseRole [db_executor]    Script Date: 9/18/2019 9:06:42 AM ******/
CREATE ROLE [db_executor]
GO
ALTER ROLE [aspnet_Membership_FullAccess] ADD MEMBER [NT AUTHORITY\NETWORK SERVICE]
GO
ALTER ROLE [aspnet_Membership_BasicAccess] ADD MEMBER [NT AUTHORITY\NETWORK SERVICE]
GO
ALTER ROLE [aspnet_Membership_ReportingAccess] ADD MEMBER [NT AUTHORITY\NETWORK SERVICE]
GO
ALTER ROLE [aspnet_Profile_FullAccess] ADD MEMBER [NT AUTHORITY\NETWORK SERVICE]
GO
ALTER ROLE [aspnet_Profile_BasicAccess] ADD MEMBER [NT AUTHORITY\NETWORK SERVICE]
GO
ALTER ROLE [aspnet_Profile_ReportingAccess] ADD MEMBER [NT AUTHORITY\NETWORK SERVICE]
GO
ALTER ROLE [aspnet_Roles_FullAccess] ADD MEMBER [NT AUTHORITY\NETWORK SERVICE]
GO
ALTER ROLE [aspnet_Roles_BasicAccess] ADD MEMBER [NT AUTHORITY\NETWORK SERVICE]
GO
ALTER ROLE [aspnet_Roles_ReportingAccess] ADD MEMBER [NT AUTHORITY\NETWORK SERVICE]
GO
ALTER ROLE [aspnet_Personalization_FullAccess] ADD MEMBER [NT AUTHORITY\NETWORK SERVICE]
GO
ALTER ROLE [aspnet_Personalization_BasicAccess] ADD MEMBER [NT AUTHORITY\NETWORK SERVICE]
GO
ALTER ROLE [aspnet_Personalization_ReportingAccess] ADD MEMBER [NT AUTHORITY\NETWORK SERVICE]
GO
ALTER ROLE [db_executor] ADD MEMBER [NT AUTHORITY\NETWORK SERVICE]
GO
ALTER ROLE [db_owner] ADD MEMBER [NT AUTHORITY\NETWORK SERVICE]
GO
ALTER ROLE [db_accessadmin] ADD MEMBER [NT AUTHORITY\NETWORK SERVICE]
GO
ALTER ROLE [db_datareader] ADD MEMBER [NT AUTHORITY\NETWORK SERVICE]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [NT AUTHORITY\NETWORK SERVICE]
GO
ALTER ROLE [db_executor] ADD MEMBER [NT AUTHORITY\SYSTEM]
GO
ALTER ROLE [db_owner] ADD MEMBER [NT AUTHORITY\SYSTEM]
GO
ALTER ROLE [db_datareader] ADD MEMBER [NT AUTHORITY\SYSTEM]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [NT AUTHORITY\SYSTEM]
GO
ALTER ROLE [db_owner] ADD MEMBER [NT SERVICE\MSSQL$ASMSDEV]
GO
ALTER ROLE [db_datareader] ADD MEMBER [NT SERVICE\MSSQL$ASMSDEV]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [NT SERVICE\MSSQL$ASMSDEV]
GO
ALTER ROLE [db_owner] ADD MEMBER [NT Service\MSSQL$TCIASMS]
GO
ALTER ROLE [db_owner] ADD MEMBER [NT SERVICE\SQLAgent$TCIASMS]
GO
ALTER ROLE [db_owner] ADD MEMBER [NT SERVICE\SQLTELEMETRY$TCIASMS]
GO
ALTER ROLE [db_datareader] ADD MEMBER [NT SERVICE\SQLTELEMETRY$TCIASMS]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [NT SERVICE\SQLTELEMETRY$TCIASMS]
GO
ALTER ROLE [db_owner] ADD MEMBER [SMA\ASMS-ACCPAC-1]
GO
ALTER ROLE [db_datareader] ADD MEMBER [SMA\ASMS-ACCPAC-1]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [SMA\ASMS-ACCPAC-1]
GO
ALTER ROLE [db_owner] ADD MEMBER [SMA\ASMS-ACCPAC-2]
GO
ALTER ROLE [db_datareader] ADD MEMBER [SMA\ASMS-ACCPAC-2]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [SMA\ASMS-ACCPAC-2]
GO
ALTER ROLE [db_owner] ADD MEMBER [SMA\ASMS-ACCPAC-2$]
GO
ALTER ROLE [db_datareader] ADD MEMBER [SMA\ASMS-ACCPAC-2$]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [SMA\ASMS-ACCPAC-2$]
GO
ALTER ROLE [db_owner] ADD MEMBER [SMA\ASMSUsers]
GO
ALTER ROLE [db_datareader] ADD MEMBER [SMA\ASMSUsers]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [SMA\ASMSUsers]
GO
ALTER ROLE [db_owner] ADD MEMBER [SMA\SMA-DBSRV$]
GO
ALTER ROLE [db_datareader] ADD MEMBER [SMA\SMA-DBSRV$]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [SMA\SMA-DBSRV$]
GO
ALTER ROLE [db_owner] ADD MEMBER [SMA\SMA-DT-SPARE-01$]
GO
ALTER ROLE [db_datareader] ADD MEMBER [SMA\SMA-DT-SPARE-01$]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [SMA\SMA-DT-SPARE-01$]
GO
ALTER ROLE [aspnet_Membership_FullAccess] ADD MEMBER [SMAGOVJM\ASMS-ACCPAC-1$]
GO
ALTER ROLE [aspnet_Membership_BasicAccess] ADD MEMBER [SMAGOVJM\ASMS-ACCPAC-1$]
GO
ALTER ROLE [aspnet_Membership_ReportingAccess] ADD MEMBER [SMAGOVJM\ASMS-ACCPAC-1$]
GO
ALTER ROLE [aspnet_Personalization_FullAccess] ADD MEMBER [SMAGOVJM\ASMS-ACCPAC-1$]
GO
ALTER ROLE [aspnet_Personalization_BasicAccess] ADD MEMBER [SMAGOVJM\ASMS-ACCPAC-1$]
GO
ALTER ROLE [aspnet_Personalization_ReportingAccess] ADD MEMBER [SMAGOVJM\ASMS-ACCPAC-1$]
GO
ALTER ROLE [db_executor] ADD MEMBER [SMAGOVJM\ASMS-ACCPAC-1$]
GO
ALTER ROLE [db_owner] ADD MEMBER [SMAGOVJM\ASMS-ACCPAC-1$]
GO
ALTER ROLE [db_datareader] ADD MEMBER [SMAGOVJM\ASMS-ACCPAC-1$]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [SMAGOVJM\ASMS-ACCPAC-1$]
GO
ALTER ROLE [db_owner] ADD MEMBER [SMAGOVJM\ASMS-ACCPAC-2]
GO
ALTER ROLE [db_datareader] ADD MEMBER [SMAGOVJM\ASMS-ACCPAC-2]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [SMAGOVJM\ASMS-ACCPAC-2]
GO
ALTER ROLE [db_owner] ADD MEMBER [smagovjm\asms-accpac-2$]
GO
ALTER ROLE [db_datareader] ADD MEMBER [smagovjm\asms-accpac-2$]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [smagovjm\asms-accpac-2$]
GO
ALTER ROLE [db_owner] ADD MEMBER [SMAGOVJM\ASMSAdministrators]
GO
ALTER ROLE [db_owner] ADD MEMBER [SMAGOVJM\ASMSUsers]
GO
ALTER ROLE [db_owner] ADD MEMBER [SMAGOVJM\cgriffith]
GO
ALTER ROLE [aspnet_Membership_FullAccess] ADD MEMBER [SMAGOVJM\mscott]
GO
ALTER ROLE [aspnet_Membership_BasicAccess] ADD MEMBER [SMAGOVJM\mscott]
GO
ALTER ROLE [aspnet_Membership_ReportingAccess] ADD MEMBER [SMAGOVJM\mscott]
GO
ALTER ROLE [aspnet_Profile_FullAccess] ADD MEMBER [SMAGOVJM\mscott]
GO
ALTER ROLE [aspnet_Profile_BasicAccess] ADD MEMBER [SMAGOVJM\mscott]
GO
ALTER ROLE [aspnet_Profile_ReportingAccess] ADD MEMBER [SMAGOVJM\mscott]
GO
ALTER ROLE [aspnet_Roles_FullAccess] ADD MEMBER [SMAGOVJM\mscott]
GO
ALTER ROLE [aspnet_Roles_BasicAccess] ADD MEMBER [SMAGOVJM\mscott]
GO
ALTER ROLE [aspnet_Roles_ReportingAccess] ADD MEMBER [SMAGOVJM\mscott]
GO
ALTER ROLE [aspnet_Personalization_FullAccess] ADD MEMBER [SMAGOVJM\mscott]
GO
ALTER ROLE [aspnet_Personalization_BasicAccess] ADD MEMBER [SMAGOVJM\mscott]
GO
ALTER ROLE [aspnet_Personalization_ReportingAccess] ADD MEMBER [SMAGOVJM\mscott]
GO
ALTER ROLE [aspnet_WebEvent_FullAccess] ADD MEMBER [SMAGOVJM\mscott]
GO
ALTER ROLE [db_owner] ADD MEMBER [SMAGOVJM\mscott]
GO
ALTER ROLE [db_accessadmin] ADD MEMBER [SMAGOVJM\mscott]
GO
ALTER ROLE [db_securityadmin] ADD MEMBER [SMAGOVJM\mscott]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [SMAGOVJM\mscott]
GO
ALTER ROLE [db_backupoperator] ADD MEMBER [SMAGOVJM\mscott]
GO
ALTER ROLE [db_datareader] ADD MEMBER [SMAGOVJM\mscott]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [SMAGOVJM\mscott]
GO
ALTER ROLE [db_denydatareader] ADD MEMBER [SMAGOVJM\mscott]
GO
ALTER ROLE [db_denydatawriter] ADD MEMBER [SMAGOVJM\mscott]
GO
ALTER ROLE [db_executor] ADD MEMBER [SMAGOVJM\SERVER-ERP2$]
GO
ALTER ROLE [db_owner] ADD MEMBER [SMAGOVJM\SERVER-ERP2$]
GO
ALTER ROLE [db_datareader] ADD MEMBER [SMAGOVJM\SERVER-ERP2$]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [SMAGOVJM\SERVER-ERP2$]
GO
ALTER ROLE [aspnet_Membership_BasicAccess] ADD MEMBER [aspnet_Membership_FullAccess]
GO
ALTER ROLE [aspnet_Membership_ReportingAccess] ADD MEMBER [aspnet_Membership_FullAccess]
GO
ALTER ROLE [aspnet_Personalization_BasicAccess] ADD MEMBER [aspnet_Personalization_FullAccess]
GO
ALTER ROLE [aspnet_Personalization_ReportingAccess] ADD MEMBER [aspnet_Personalization_FullAccess]
GO
ALTER ROLE [aspnet_Profile_BasicAccess] ADD MEMBER [aspnet_Profile_FullAccess]
GO
ALTER ROLE [aspnet_Profile_ReportingAccess] ADD MEMBER [aspnet_Profile_FullAccess]
GO
ALTER ROLE [aspnet_Roles_BasicAccess] ADD MEMBER [aspnet_Roles_FullAccess]
GO
ALTER ROLE [aspnet_Roles_ReportingAccess] ADD MEMBER [aspnet_Roles_FullAccess]
GO
/****** Object:  Table [dbo].[AnnualDIR_Aeronautical]    Script Date: 9/18/2019 9:06:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnnualDIR_Aeronautical](
	[report_id] [varchar](100) NOT NULL,
	[licenseNumber] [varchar](100) NULL,
	[clientCompany] [varchar](100) NULL,
	[invoiceID] [varchar](50) NULL,
	[budget] [varchar](50) NULL,
	[thisPeriodsInvoice] [varchar](5) NULL,
	[invoiceTotal] [varchar](50) NULL,
	[balanceBFoward] [varchar](50) NULL,
	[fromRevenue] [varchar](50) NULL,
	[toRevenue] [varchar](50) NULL,
	[closingBalance] [varchar](50) NULL,
	[totalMonths] [int] NULL,
	[monthsUtilized] [int] NULL,
	[monthsRemaining] [int] NULL,
	[validityStart] [varchar](100) NULL,
	[ValidityEnd] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AnnualDIR_Broadband]    Script Date: 9/18/2019 9:06:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnnualDIR_Broadband](
	[report_id] [varchar](100) NOT NULL,
	[licenseNumber] [varchar](100) NULL,
	[clientCompany] [varchar](100) NULL,
	[invoiceID] [varchar](50) NULL,
	[budget] [varchar](50) NULL,
	[thisPeriodsInvoice] [varchar](5) NULL,
	[invoiceTotal] [varchar](50) NULL,
	[balanceBFoward] [varchar](50) NULL,
	[fromRevenue] [varchar](50) NULL,
	[toRevenue] [varchar](50) NULL,
	[closingBalance] [varchar](50) NULL,
	[totalMonths] [int] NULL,
	[monthsUtilized] [int] NULL,
	[monthsRemaining] [int] NULL,
	[validityStart] [varchar](100) NULL,
	[ValidityEnd] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AnnualDIR_Cellular]    Script Date: 9/18/2019 9:06:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnnualDIR_Cellular](
	[report_id] [varchar](100) NOT NULL,
	[licenseNumber] [varchar](100) NULL,
	[clientCompany] [varchar](100) NULL,
	[invoiceID] [varchar](50) NULL,
	[budget] [varchar](50) NULL,
	[thisPeriodsInvoice] [varchar](5) NULL,
	[invoiceTotal] [varchar](50) NULL,
	[balanceBFoward] [varchar](50) NULL,
	[fromRevenue] [varchar](50) NULL,
	[toRevenue] [varchar](50) NULL,
	[closingBalance] [varchar](50) NULL,
	[totalMonths] [int] NULL,
	[monthsUtilized] [int] NULL,
	[monthsRemaining] [int] NULL,
	[validityStart] [varchar](100) NULL,
	[ValidityEnd] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AnnualDIR_Dservices]    Script Date: 9/18/2019 9:06:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnnualDIR_Dservices](
	[report_id] [varchar](100) NOT NULL,
	[licenseNumber] [varchar](100) NULL,
	[clientCompany] [varchar](100) NULL,
	[invoiceID] [varchar](50) NULL,
	[budget] [varchar](50) NULL,
	[thisPeriodsInvoice] [varchar](5) NULL,
	[invoiceTotal] [varchar](50) NULL,
	[balanceBFoward] [varchar](50) NULL,
	[fromRevenue] [varchar](50) NULL,
	[toRevenue] [varchar](50) NULL,
	[closingBalance] [varchar](50) NULL,
	[totalMonths] [int] NULL,
	[monthsUtilized] [int] NULL,
	[monthsRemaining] [int] NULL,
	[validityStart] [varchar](100) NULL,
	[ValidityEnd] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AnnualDIR_Marine]    Script Date: 9/18/2019 9:06:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnnualDIR_Marine](
	[report_id] [varchar](100) NOT NULL,
	[licenseNumber] [varchar](100) NULL,
	[clientCompany] [varchar](100) NULL,
	[invoiceID] [varchar](50) NULL,
	[budget] [varchar](50) NULL,
	[thisPeriodsInvoice] [varchar](5) NULL,
	[invoiceTotal] [varchar](50) NULL,
	[balanceBFoward] [varchar](50) NULL,
	[fromRevenue] [varchar](50) NULL,
	[toRevenue] [varchar](50) NULL,
	[closingBalance] [varchar](50) NULL,
	[totalMonths] [int] NULL,
	[monthsUtilized] [int] NULL,
	[monthsRemaining] [int] NULL,
	[validityStart] [varchar](100) NULL,
	[ValidityEnd] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AnnualDIR_Microwave]    Script Date: 9/18/2019 9:06:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnnualDIR_Microwave](
	[report_id] [varchar](100) NOT NULL,
	[licenseNumber] [varchar](100) NULL,
	[clientCompany] [varchar](100) NULL,
	[invoiceID] [varchar](50) NULL,
	[budget] [varchar](50) NULL,
	[thisPeriodsInvoice] [varchar](5) NULL,
	[invoiceTotal] [varchar](50) NULL,
	[balanceBFoward] [varchar](50) NULL,
	[fromRevenue] [varchar](50) NULL,
	[toRevenue] [varchar](50) NULL,
	[closingBalance] [varchar](50) NULL,
	[totalMonths] [int] NULL,
	[monthsUtilized] [int] NULL,
	[monthsRemaining] [int] NULL,
	[validityStart] [varchar](100) NULL,
	[ValidityEnd] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AnnualDIR_NextGenDate]    Script Date: 9/18/2019 9:06:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnnualDIR_NextGenDate](
	[date] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AnnualDIR_Other]    Script Date: 9/18/2019 9:06:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnnualDIR_Other](
	[report_id] [varchar](100) NOT NULL,
	[licenseNumber] [varchar](100) NULL,
	[clientCompany] [varchar](100) NULL,
	[invoiceID] [varchar](50) NULL,
	[budget] [varchar](50) NULL,
	[thisPeriodsInvoice] [varchar](5) NULL,
	[invoiceTotal] [varchar](50) NULL,
	[balanceBFoward] [varchar](50) NULL,
	[fromRevenue] [varchar](50) NULL,
	[toRevenue] [varchar](50) NULL,
	[closingBalance] [varchar](50) NULL,
	[totalMonths] [int] NULL,
	[monthsUtilized] [int] NULL,
	[monthsRemaining] [int] NULL,
	[validityStart] [varchar](100) NULL,
	[ValidityEnd] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AnnualDIR_ReportMain]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnnualDIR_ReportMain](
	[report_id] [varchar](100) NOT NULL,
	[report_date] [datetime] NOT NULL,
	[pdf_file] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[report_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AnnualDIR_SubTotals]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnnualDIR_SubTotals](
	[record_id] [varchar](100) NULL,
	[category] [int] NULL,
	[invoiceTotal] [varchar](50) NULL,
	[balanceBFwd] [varchar](50) NULL,
	[toRev] [varchar](50) NULL,
	[closingBal] [varchar](50) NULL,
	[fromRev] [varchar](50) NULL,
	[budget] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AnnualDIR_Totals]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnnualDIR_Totals](
	[record_id] [varchar](100) NULL,
	[invoiceTotal] [varchar](50) NULL,
	[balanceBFwd] [varchar](50) NULL,
	[toRev] [varchar](50) NULL,
	[closingBal] [varchar](50) NULL,
	[fromRev] [varchar](50) NULL,
	[budget] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AnnualDIR_Trunking]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnnualDIR_Trunking](
	[report_id] [varchar](100) NOT NULL,
	[licenseNumber] [varchar](100) NULL,
	[clientCompany] [varchar](100) NULL,
	[invoiceID] [varchar](50) NULL,
	[budget] [varchar](50) NULL,
	[thisPeriodsInvoice] [varchar](5) NULL,
	[invoiceTotal] [varchar](50) NULL,
	[balanceBFoward] [varchar](50) NULL,
	[fromRevenue] [varchar](50) NULL,
	[toRevenue] [varchar](50) NULL,
	[closingBalance] [varchar](50) NULL,
	[totalMonths] [int] NULL,
	[monthsUtilized] [int] NULL,
	[monthsRemaining] [int] NULL,
	[validityStart] [varchar](100) NULL,
	[ValidityEnd] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AnnualDIR_Vsat]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnnualDIR_Vsat](
	[report_id] [varchar](100) NOT NULL,
	[licenseNumber] [varchar](100) NULL,
	[clientCompany] [varchar](100) NULL,
	[invoiceID] [varchar](50) NULL,
	[budget] [varchar](50) NULL,
	[thisPeriodsInvoice] [varchar](5) NULL,
	[invoiceTotal] [varchar](50) NULL,
	[balanceBFoward] [varchar](50) NULL,
	[fromRevenue] [varchar](50) NULL,
	[toRevenue] [varchar](50) NULL,
	[closingBalance] [varchar](50) NULL,
	[totalMonths] [int] NULL,
	[monthsUtilized] [int] NULL,
	[monthsRemaining] [int] NULL,
	[validityStart] [varchar](100) NULL,
	[ValidityEnd] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aspnet_Applications]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnet_Applications](
	[ApplicationName] [nvarchar](256) NOT NULL,
	[LoweredApplicationName] [nvarchar](256) NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[Description] [nvarchar](256) NULL,
PRIMARY KEY NONCLUSTERED 
(
	[ApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[LoweredApplicationName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ApplicationName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aspnet_Membership]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnet_Membership](
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[Password] [nvarchar](128) NOT NULL,
	[PasswordFormat] [int] NOT NULL,
	[PasswordSalt] [nvarchar](128) NOT NULL,
	[MobilePIN] [nvarchar](16) NULL,
	[Email] [nvarchar](256) NULL,
	[LoweredEmail] [nvarchar](256) NULL,
	[PasswordQuestion] [nvarchar](256) NULL,
	[PasswordAnswer] [nvarchar](128) NULL,
	[IsApproved] [bit] NOT NULL,
	[IsLockedOut] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastLoginDate] [datetime] NOT NULL,
	[LastPasswordChangedDate] [datetime] NOT NULL,
	[LastLockoutDate] [datetime] NOT NULL,
	[FailedPasswordAttemptCount] [int] NOT NULL,
	[FailedPasswordAttemptWindowStart] [datetime] NOT NULL,
	[FailedPasswordAnswerAttemptCount] [int] NOT NULL,
	[FailedPasswordAnswerAttemptWindowStart] [datetime] NOT NULL,
	[Comment] [ntext] NULL,
PRIMARY KEY NONCLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aspnet_Paths]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnet_Paths](
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[PathId] [uniqueidentifier] NOT NULL,
	[Path] [nvarchar](256) NOT NULL,
	[LoweredPath] [nvarchar](256) NOT NULL,
PRIMARY KEY NONCLUSTERED 
(
	[PathId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aspnet_PersonalizationAllUsers]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnet_PersonalizationAllUsers](
	[PathId] [uniqueidentifier] NOT NULL,
	[PageSettings] [image] NOT NULL,
	[LastUpdatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PathId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aspnet_PersonalizationPerUser]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnet_PersonalizationPerUser](
	[Id] [uniqueidentifier] NOT NULL,
	[PathId] [uniqueidentifier] NULL,
	[UserId] [uniqueidentifier] NULL,
	[PageSettings] [image] NOT NULL,
	[LastUpdatedDate] [datetime] NOT NULL,
PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aspnet_Profile]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnet_Profile](
	[UserId] [uniqueidentifier] NOT NULL,
	[PropertyNames] [ntext] NOT NULL,
	[PropertyValuesString] [ntext] NOT NULL,
	[PropertyValuesBinary] [image] NOT NULL,
	[LastUpdatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aspnet_Roles]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnet_Roles](
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
	[RoleName] [nvarchar](256) NOT NULL,
	[LoweredRoleName] [nvarchar](256) NOT NULL,
	[Description] [nvarchar](256) NULL,
PRIMARY KEY NONCLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aspnet_SchemaVersions]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnet_SchemaVersions](
	[Feature] [nvarchar](128) NOT NULL,
	[CompatibleSchemaVersion] [nvarchar](128) NOT NULL,
	[IsCurrentVersion] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Feature] ASC,
	[CompatibleSchemaVersion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aspnet_Users]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnet_Users](
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
	[LoweredUserName] [nvarchar](256) NOT NULL,
	[MobileAlias] [nvarchar](16) NULL,
	[IsAnonymous] [bit] NOT NULL,
	[LastActivityDate] [datetime] NOT NULL,
PRIMARY KEY NONCLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aspnet_UsersInRoles]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnet_UsersInRoles](
	[UserId] [uniqueidentifier] NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aspnet_WebEvent_Events]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aspnet_WebEvent_Events](
	[EventId] [char](32) NOT NULL,
	[EventTimeUtc] [datetime] NOT NULL,
	[EventTime] [datetime] NOT NULL,
	[EventType] [nvarchar](256) NOT NULL,
	[EventSequence] [decimal](19, 0) NOT NULL,
	[EventOccurrence] [decimal](19, 0) NOT NULL,
	[EventCode] [int] NOT NULL,
	[EventDetailCode] [int] NOT NULL,
	[Message] [nvarchar](1024) NULL,
	[ApplicationPath] [nvarchar](256) NULL,
	[ApplicationVirtualPath] [nvarchar](256) NULL,
	[MachineName] [nvarchar](256) NOT NULL,
	[RequestUrl] [nvarchar](1024) NULL,
	[ExceptionType] [nvarchar](256) NULL,
	[Details] [ntext] NULL,
PRIMARY KEY CLUSTERED 
(
	[EventId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BankCode]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BankCode](
	[BankCodeId] [varchar](20) NOT NULL,
	[BankCode] [varchar](30) NOT NULL,
	[CurrentRefNumber] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BankCodeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[counters]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[counters](
	[transferredInvoices] [int] NULL,
	[transferredReceipts] [int] NULL,
	[createdCustomers] [int] NULL,
	[id] [int] NULL,
	[creditMemoSequence] [int] NULL,
	[monthlyReset] [datetime] NULL,
	[dailyReset] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CustomerCreatedDetail]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerCreatedDetail](
	[ClientId] [varchar](20) NULL,
	[Name] [varchar](100) NULL,
	[DateCreated] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CustomersTransferred]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomersTransferred](
	[TransactionID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [varchar](7) NULL,
	[DateTransferred] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeferredBudget]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeferredBudget](
	[ARInvoiceID] [int] NULL,
	[Budget] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DIRcategories]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DIRcategories](
	[Category] [int] NOT NULL,
	[Description] [varchar](100) NULL,
	[CreditGLID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Category] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IntegrationStatus]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IntegrationStatus](
	[state] [int] NULL,
	[timestamp] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[InvoiceBatch]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceBatch](
	[BatchId] [int] NOT NULL,
	[CreatedDate] [date] NOT NULL,
	[ExpiryDate] [date] NOT NULL,
	[Status] [char](10) NOT NULL,
	[Count] [int] NOT NULL,
	[BatchType] [varchar](50) NULL,
	[amount] [decimal](19, 2) NULL,
	[renstat] [varchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[BatchId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[InvoiceList]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceList](
	[invoiceId] [int] NULL,
	[status] [varchar](30) NULL,
	[TargetBatch] [int] NULL,
	[EntryNumber] [int] NULL,
	[CreditGl] [int] NULL,
	[sequence] [int] NULL,
	[ClientName] [varchar](120) NULL,
	[clientId] [varchar](20) NULL,
	[dateCreated] [datetime] NULL,
	[Author] [varchar](50) NULL,
	[Amount] [decimal](19, 2) NULL,
	[LastModified] [datetime] NULL,
	[state] [varchar](30) NULL,
	[usrate] [decimal](19, 4) NULL,
	[usamount] [decimal](19, 2) NULL,
	[isvoid] [int] NOT NULL,
	[isCreditMemo] [int] NOT NULL,
	[credMemoNum] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[invoiceTotal]    Script Date: 9/18/2019 9:06:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[invoiceTotal](
	[total] [decimal](19, 2) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Log]    Script Date: 9/18/2019 9:06:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Log](
	[Datetime] [datetime] NULL,
	[Msg] [varchar](max) NULL,
	[id] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MessageQueue]    Script Date: 9/18/2019 9:06:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MessageQueue](
	[Date] [datetime] NULL,
	[Message] [varchar](300) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MonthlyDIR_Aeronautical]    Script Date: 9/18/2019 9:06:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MonthlyDIR_Aeronautical](
	[report_id] [varchar](100) NULL,
	[licenseNumber] [varchar](100) NULL,
	[clientCompany] [varchar](100) NULL,
	[invoiceID] [varchar](50) NULL,
	[budget] [varchar](50) NULL,
	[invoiceTotal] [varchar](50) NULL,
	[thisPeriodsInvoice] [varchar](5) NULL,
	[balanceBFoward] [varchar](50) NULL,
	[fromRevenue] [varchar](50) NULL,
	[toRevenue] [varchar](50) NULL,
	[closingBalance] [varchar](50) NULL,
	[totalMonths] [int] NULL,
	[monthsUtilized] [int] NULL,
	[monthsRemaining] [int] NULL,
	[validityStart] [varchar](100) NULL,
	[validityEnd] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MonthlyDIR_Broadband]    Script Date: 9/18/2019 9:06:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MonthlyDIR_Broadband](
	[report_id] [varchar](100) NULL,
	[licenseNumber] [varchar](100) NULL,
	[clientCompany] [varchar](100) NULL,
	[invoiceID] [varchar](50) NULL,
	[budget] [varchar](50) NULL,
	[invoiceTotal] [varchar](50) NULL,
	[thisPeriodsInvoice] [varchar](5) NULL,
	[balanceBFoward] [varchar](50) NULL,
	[fromRevenue] [varchar](50) NULL,
	[toRevenue] [varchar](50) NULL,
	[closingBalance] [varchar](50) NULL,
	[totalMonths] [int] NULL,
	[monthsUtilized] [int] NULL,
	[monthsRemaining] [int] NULL,
	[validityStart] [varchar](100) NULL,
	[validityEnd] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MonthlyDIR_Cellular]    Script Date: 9/18/2019 9:06:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MonthlyDIR_Cellular](
	[report_id] [varchar](100) NULL,
	[licenseNumber] [varchar](100) NULL,
	[clientCompany] [varchar](100) NULL,
	[invoiceID] [varchar](50) NULL,
	[budget] [varchar](50) NULL,
	[invoiceTotal] [varchar](50) NULL,
	[thisPeriodsInvoice] [varchar](5) NULL,
	[balanceBFoward] [varchar](50) NULL,
	[fromRevenue] [varchar](50) NULL,
	[toRevenue] [varchar](50) NULL,
	[closingBalance] [varchar](50) NULL,
	[totalMonths] [int] NULL,
	[monthsUtilized] [int] NULL,
	[monthsRemaining] [int] NULL,
	[validityStart] [varchar](100) NULL,
	[validityEnd] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MonthlyDIR_Dservices]    Script Date: 9/18/2019 9:06:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MonthlyDIR_Dservices](
	[report_id] [varchar](100) NULL,
	[licenseNumber] [varchar](100) NULL,
	[clientCompany] [varchar](100) NULL,
	[invoiceID] [varchar](50) NULL,
	[budget] [varchar](50) NULL,
	[invoiceTotal] [varchar](50) NULL,
	[thisPeriodsInvoice] [varchar](5) NULL,
	[balanceBFoward] [varchar](50) NULL,
	[fromRevenue] [varchar](50) NULL,
	[toRevenue] [varchar](50) NULL,
	[closingBalance] [varchar](50) NULL,
	[totalMonths] [int] NULL,
	[monthsUtilized] [int] NULL,
	[monthsRemaining] [int] NULL,
	[validityStart] [varchar](100) NULL,
	[validityEnd] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MonthlyDIR_Marine]    Script Date: 9/18/2019 9:06:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MonthlyDIR_Marine](
	[report_id] [varchar](100) NULL,
	[licenseNumber] [varchar](100) NULL,
	[clientCompany] [varchar](100) NULL,
	[invoiceID] [varchar](50) NULL,
	[budget] [varchar](50) NULL,
	[invoiceTotal] [varchar](50) NULL,
	[thisPeriodsInvoice] [varchar](5) NULL,
	[balanceBFoward] [varchar](50) NULL,
	[fromRevenue] [varchar](50) NULL,
	[toRevenue] [varchar](50) NULL,
	[closingBalance] [varchar](50) NULL,
	[totalMonths] [int] NULL,
	[monthsUtilized] [int] NULL,
	[monthsRemaining] [int] NULL,
	[validityStart] [varchar](100) NULL,
	[validityEnd] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MonthlyDIR_Microwave]    Script Date: 9/18/2019 9:06:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MonthlyDIR_Microwave](
	[report_id] [varchar](100) NULL,
	[licenseNumber] [varchar](100) NULL,
	[clientCompany] [varchar](100) NULL,
	[invoiceID] [varchar](50) NULL,
	[budget] [varchar](50) NULL,
	[invoiceTotal] [varchar](50) NULL,
	[thisPeriodsInvoice] [varchar](5) NULL,
	[balanceBFoward] [varchar](50) NULL,
	[fromRevenue] [varchar](50) NULL,
	[toRevenue] [varchar](50) NULL,
	[closingBalance] [varchar](50) NULL,
	[totalMonths] [int] NULL,
	[monthsUtilized] [int] NULL,
	[monthsRemaining] [int] NULL,
	[validityStart] [varchar](100) NULL,
	[validityEnd] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MonthlyDIR_NextGenDate]    Script Date: 9/18/2019 9:06:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MonthlyDIR_NextGenDate](
	[date] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MonthlyDIR_Other]    Script Date: 9/18/2019 9:06:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MonthlyDIR_Other](
	[report_id] [varchar](100) NULL,
	[licenseNumber] [varchar](100) NULL,
	[clientCompany] [varchar](100) NULL,
	[invoiceID] [varchar](50) NULL,
	[budget] [varchar](50) NULL,
	[invoiceTotal] [varchar](50) NULL,
	[thisPeriodsInvoice] [varchar](5) NULL,
	[balanceBFoward] [varchar](50) NULL,
	[fromRevenue] [varchar](50) NULL,
	[toRevenue] [varchar](50) NULL,
	[closingBalance] [varchar](50) NULL,
	[totalMonths] [int] NULL,
	[monthsUtilized] [int] NULL,
	[monthsRemaining] [int] NULL,
	[validityStart] [varchar](100) NULL,
	[validityEnd] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MonthlyDIR_ReportMain]    Script Date: 9/18/2019 9:06:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MonthlyDIR_ReportMain](
	[report_id] [varchar](100) NOT NULL,
	[report_date] [datetime] NULL,
	[pdf_file] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[report_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MonthlyDIR_SubTotals]    Script Date: 9/18/2019 9:06:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MonthlyDIR_SubTotals](
	[record_id] [varchar](100) NULL,
	[category] [int] NULL,
	[invoiceTotal] [varchar](50) NULL,
	[balanceBFwd] [varchar](50) NULL,
	[toRev] [varchar](50) NULL,
	[closingBal] [varchar](50) NULL,
	[fromRev] [varchar](50) NULL,
	[budget] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MonthlyDIR_Totals]    Script Date: 9/18/2019 9:06:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MonthlyDIR_Totals](
	[record_id] [varchar](100) NULL,
	[invoiceTotal] [varchar](50) NULL,
	[balanceBFwd] [varchar](50) NULL,
	[toRev] [varchar](50) NULL,
	[closingBal] [varchar](50) NULL,
	[fromRev] [varchar](50) NULL,
	[budget] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MonthlyDIR_Trunking]    Script Date: 9/18/2019 9:06:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MonthlyDIR_Trunking](
	[report_id] [varchar](100) NULL,
	[licenseNumber] [varchar](100) NULL,
	[clientCompany] [varchar](100) NULL,
	[invoiceID] [varchar](50) NULL,
	[budget] [varchar](50) NULL,
	[invoiceTotal] [varchar](50) NULL,
	[thisPeriodsInvoice] [varchar](5) NULL,
	[balanceBFoward] [varchar](50) NULL,
	[fromRevenue] [varchar](50) NULL,
	[toRevenue] [varchar](50) NULL,
	[closingBalance] [varchar](50) NULL,
	[totalMonths] [int] NULL,
	[monthsUtilized] [int] NULL,
	[monthsRemaining] [int] NULL,
	[validityStart] [varchar](100) NULL,
	[validityEnd] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MonthlyDIR_Vsat]    Script Date: 9/18/2019 9:06:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MonthlyDIR_Vsat](
	[report_id] [varchar](100) NULL,
	[licenseNumber] [varchar](100) NULL,
	[clientCompany] [varchar](100) NULL,
	[invoiceID] [varchar](50) NULL,
	[budget] [varchar](50) NULL,
	[invoiceTotal] [varchar](50) NULL,
	[thisPeriodsInvoice] [varchar](5) NULL,
	[balanceBFoward] [varchar](50) NULL,
	[fromRevenue] [varchar](50) NULL,
	[toRevenue] [varchar](50) NULL,
	[closingBalance] [varchar](50) NULL,
	[totalMonths] [int] NULL,
	[monthsUtilized] [int] NULL,
	[monthsRemaining] [int] NULL,
	[validityStart] [varchar](100) NULL,
	[validityEnd] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentBatch]    Script Date: 9/18/2019 9:06:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentBatch](
	[BatchId] [int] NOT NULL,
	[CreatedDate] [date] NOT NULL,
	[ExpiryDate] [date] NOT NULL,
	[Status] [varchar](30) NOT NULL,
	[BankCodeId] [varchar](20) NOT NULL,
	[Count] [int] NOT NULL,
	[Total] [float] NULL,
PRIMARY KEY CLUSTERED 
(
	[BatchId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentList]    Script Date: 9/18/2019 9:06:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentList](
	[clientId] [varchar](20) NULL,
	[clientName] [varchar](100) NULL,
	[createdDate] [datetime] NULL,
	[invoiceId] [varchar](20) NULL,
	[amount] [decimal](19, 2) NULL,
	[Read] [varchar](5) NULL,
	[sequence] [int] NULL,
	[usamount] [decimal](19, 2) NULL,
	[prepstat] [varchar](3) NULL,
	[referenceNumber] [int] NULL,
	[prepaymentRemainder] [decimal](19, 2) NULL,
	[destinationBank] [int] NULL,
	[isPayByCredit] [varchar](20) NULL,
	[usrate] [decimal](19, 4) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCustomerCreatedCount]    Script Date: 9/18/2019 9:06:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCustomerCreatedCount](
	[Count] [int] NULL,
	[LastUpdate] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[testInsert]    Script Date: 9/18/2019 9:06:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[testInsert](
	[col1] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[testRef]    Script Date: 9/18/2019 9:06:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[testRef](
	[ref] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransferredInvoices]    Script Date: 9/18/2019 9:06:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransferredInvoices](
	[TransactionID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [varchar](7) NULL,
	[DateTransferred] [datetime] NULL,
	[inv_ID] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransferredPayments]    Script Date: 9/18/2019 9:06:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransferredPayments](
	[TransactionID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [varchar](7) NULL,
	[DateTransferred] [datetime] NULL,
	[payment_ID] [int] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[aspnet_Applications] ADD  DEFAULT (newid()) FOR [ApplicationId]
GO
ALTER TABLE [dbo].[aspnet_Membership] ADD  DEFAULT ((0)) FOR [PasswordFormat]
GO
ALTER TABLE [dbo].[aspnet_Paths] ADD  DEFAULT (newid()) FOR [PathId]
GO
ALTER TABLE [dbo].[aspnet_PersonalizationPerUser] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[aspnet_Roles] ADD  DEFAULT (newid()) FOR [RoleId]
GO
ALTER TABLE [dbo].[aspnet_Users] ADD  DEFAULT (newid()) FOR [UserId]
GO
ALTER TABLE [dbo].[aspnet_Users] ADD  DEFAULT (NULL) FOR [MobileAlias]
GO
ALTER TABLE [dbo].[aspnet_Users] ADD  DEFAULT ((0)) FOR [IsAnonymous]
GO
ALTER TABLE [dbo].[CustomersTransferred] ADD  DEFAULT (getdate()) FOR [DateTransferred]
GO
ALTER TABLE [dbo].[InvoiceList] ADD  DEFAULT ((0)) FOR [isvoid]
GO
ALTER TABLE [dbo].[InvoiceList] ADD  DEFAULT ((0)) FOR [isCreditMemo]
GO
ALTER TABLE [dbo].[InvoiceList] ADD  DEFAULT ((0)) FOR [credMemoNum]
GO
ALTER TABLE [dbo].[PaymentList] ADD  DEFAULT ((0)) FOR [prepaymentRemainder]
GO
ALTER TABLE [dbo].[TransferredInvoices] ADD  DEFAULT (getdate()) FOR [DateTransferred]
GO
ALTER TABLE [dbo].[TransferredPayments] ADD  DEFAULT (getdate()) FOR [DateTransferred]
GO
ALTER TABLE [dbo].[AnnualDIR_Aeronautical]  WITH CHECK ADD  CONSTRAINT [FK_AnnualDIR_Aeronautical_AnnualDIR_ReportMain] FOREIGN KEY([report_id])
REFERENCES [dbo].[AnnualDIR_ReportMain] ([report_id])
GO
ALTER TABLE [dbo].[AnnualDIR_Aeronautical] CHECK CONSTRAINT [FK_AnnualDIR_Aeronautical_AnnualDIR_ReportMain]
GO
ALTER TABLE [dbo].[AnnualDIR_Broadband]  WITH CHECK ADD  CONSTRAINT [FK_AnnualDIR_Broadband_AnnualDIR_ReportMain] FOREIGN KEY([report_id])
REFERENCES [dbo].[AnnualDIR_ReportMain] ([report_id])
GO
ALTER TABLE [dbo].[AnnualDIR_Broadband] CHECK CONSTRAINT [FK_AnnualDIR_Broadband_AnnualDIR_ReportMain]
GO
ALTER TABLE [dbo].[AnnualDIR_Cellular]  WITH CHECK ADD  CONSTRAINT [FK_AnnualDIR_Cellular_AnnualDIR_ReportMain] FOREIGN KEY([report_id])
REFERENCES [dbo].[AnnualDIR_ReportMain] ([report_id])
GO
ALTER TABLE [dbo].[AnnualDIR_Cellular] CHECK CONSTRAINT [FK_AnnualDIR_Cellular_AnnualDIR_ReportMain]
GO
ALTER TABLE [dbo].[AnnualDIR_Dservices]  WITH CHECK ADD  CONSTRAINT [FK_AnnualDIR_Dservices_AnnualDIR_ReportMain] FOREIGN KEY([report_id])
REFERENCES [dbo].[AnnualDIR_ReportMain] ([report_id])
GO
ALTER TABLE [dbo].[AnnualDIR_Dservices] CHECK CONSTRAINT [FK_AnnualDIR_Dservices_AnnualDIR_ReportMain]
GO
ALTER TABLE [dbo].[AnnualDIR_Marine]  WITH CHECK ADD  CONSTRAINT [FK_AnnualDIR_Marine_AnnualDIR_ReportMain] FOREIGN KEY([report_id])
REFERENCES [dbo].[AnnualDIR_ReportMain] ([report_id])
GO
ALTER TABLE [dbo].[AnnualDIR_Marine] CHECK CONSTRAINT [FK_AnnualDIR_Marine_AnnualDIR_ReportMain]
GO
ALTER TABLE [dbo].[AnnualDIR_Microwave]  WITH CHECK ADD  CONSTRAINT [FK_AnnualDIR_Microwave_AnnualDIR_ReportMain] FOREIGN KEY([report_id])
REFERENCES [dbo].[AnnualDIR_ReportMain] ([report_id])
GO
ALTER TABLE [dbo].[AnnualDIR_Microwave] CHECK CONSTRAINT [FK_AnnualDIR_Microwave_AnnualDIR_ReportMain]
GO
ALTER TABLE [dbo].[AnnualDIR_Other]  WITH CHECK ADD  CONSTRAINT [FK_AnnualDIR_Other_AnnualDIR_ReportMain] FOREIGN KEY([report_id])
REFERENCES [dbo].[AnnualDIR_ReportMain] ([report_id])
GO
ALTER TABLE [dbo].[AnnualDIR_Other] CHECK CONSTRAINT [FK_AnnualDIR_Other_AnnualDIR_ReportMain]
GO
ALTER TABLE [dbo].[AnnualDIR_SubTotals]  WITH CHECK ADD  CONSTRAINT [FK_AnnualDIR_SubTotals_AnnualDIR_ReportMain] FOREIGN KEY([record_id])
REFERENCES [dbo].[AnnualDIR_ReportMain] ([report_id])
GO
ALTER TABLE [dbo].[AnnualDIR_SubTotals] CHECK CONSTRAINT [FK_AnnualDIR_SubTotals_AnnualDIR_ReportMain]
GO
ALTER TABLE [dbo].[AnnualDIR_SubTotals]  WITH CHECK ADD  CONSTRAINT [FK_AnnualDIR_SubTotals_DIRcategories] FOREIGN KEY([category])
REFERENCES [dbo].[DIRcategories] ([Category])
GO
ALTER TABLE [dbo].[AnnualDIR_SubTotals] CHECK CONSTRAINT [FK_AnnualDIR_SubTotals_DIRcategories]
GO
ALTER TABLE [dbo].[AnnualDIR_Totals]  WITH CHECK ADD  CONSTRAINT [FK_AnnualDIR_Totals_AnnualDIR_ReportMain] FOREIGN KEY([record_id])
REFERENCES [dbo].[AnnualDIR_ReportMain] ([report_id])
GO
ALTER TABLE [dbo].[AnnualDIR_Totals] CHECK CONSTRAINT [FK_AnnualDIR_Totals_AnnualDIR_ReportMain]
GO
ALTER TABLE [dbo].[AnnualDIR_Trunking]  WITH CHECK ADD  CONSTRAINT [FK_AnnualDIR_Trunking_AnnualDIR_ReportMain] FOREIGN KEY([report_id])
REFERENCES [dbo].[AnnualDIR_ReportMain] ([report_id])
GO
ALTER TABLE [dbo].[AnnualDIR_Trunking] CHECK CONSTRAINT [FK_AnnualDIR_Trunking_AnnualDIR_ReportMain]
GO
ALTER TABLE [dbo].[AnnualDIR_Vsat]  WITH CHECK ADD  CONSTRAINT [FK_AnnualDIR_Vsat_AnnualDIR_ReportMain] FOREIGN KEY([report_id])
REFERENCES [dbo].[AnnualDIR_ReportMain] ([report_id])
GO
ALTER TABLE [dbo].[AnnualDIR_Vsat] CHECK CONSTRAINT [FK_AnnualDIR_Vsat_AnnualDIR_ReportMain]
GO
ALTER TABLE [dbo].[aspnet_Membership]  WITH CHECK ADD FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO
ALTER TABLE [dbo].[aspnet_Membership]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO
ALTER TABLE [dbo].[aspnet_Paths]  WITH CHECK ADD FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO
ALTER TABLE [dbo].[aspnet_PersonalizationAllUsers]  WITH CHECK ADD FOREIGN KEY([PathId])
REFERENCES [dbo].[aspnet_Paths] ([PathId])
GO
ALTER TABLE [dbo].[aspnet_PersonalizationPerUser]  WITH CHECK ADD FOREIGN KEY([PathId])
REFERENCES [dbo].[aspnet_Paths] ([PathId])
GO
ALTER TABLE [dbo].[aspnet_PersonalizationPerUser]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO
ALTER TABLE [dbo].[aspnet_Profile]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO
ALTER TABLE [dbo].[aspnet_Roles]  WITH CHECK ADD FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO
ALTER TABLE [dbo].[aspnet_Users]  WITH CHECK ADD FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
GO
ALTER TABLE [dbo].[aspnet_UsersInRoles]  WITH CHECK ADD FOREIGN KEY([RoleId])
REFERENCES [dbo].[aspnet_Roles] ([RoleId])
GO
ALTER TABLE [dbo].[aspnet_UsersInRoles]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[aspnet_Users] ([UserId])
GO
ALTER TABLE [dbo].[MonthlyDIR_Aeronautical]  WITH CHECK ADD FOREIGN KEY([report_id])
REFERENCES [dbo].[MonthlyDIR_ReportMain] ([report_id])
GO
ALTER TABLE [dbo].[MonthlyDIR_Broadband]  WITH CHECK ADD FOREIGN KEY([report_id])
REFERENCES [dbo].[MonthlyDIR_ReportMain] ([report_id])
GO
ALTER TABLE [dbo].[MonthlyDIR_Cellular]  WITH CHECK ADD FOREIGN KEY([report_id])
REFERENCES [dbo].[MonthlyDIR_ReportMain] ([report_id])
GO
ALTER TABLE [dbo].[MonthlyDIR_Dservices]  WITH CHECK ADD FOREIGN KEY([report_id])
REFERENCES [dbo].[MonthlyDIR_ReportMain] ([report_id])
GO
ALTER TABLE [dbo].[MonthlyDIR_Marine]  WITH CHECK ADD FOREIGN KEY([report_id])
REFERENCES [dbo].[MonthlyDIR_ReportMain] ([report_id])
GO
ALTER TABLE [dbo].[MonthlyDIR_Microwave]  WITH CHECK ADD FOREIGN KEY([report_id])
REFERENCES [dbo].[MonthlyDIR_ReportMain] ([report_id])
GO
ALTER TABLE [dbo].[MonthlyDIR_Other]  WITH CHECK ADD FOREIGN KEY([report_id])
REFERENCES [dbo].[MonthlyDIR_ReportMain] ([report_id])
GO
ALTER TABLE [dbo].[MonthlyDIR_SubTotals]  WITH CHECK ADD FOREIGN KEY([category])
REFERENCES [dbo].[DIRcategories] ([Category])
GO
ALTER TABLE [dbo].[MonthlyDIR_SubTotals]  WITH CHECK ADD FOREIGN KEY([record_id])
REFERENCES [dbo].[MonthlyDIR_ReportMain] ([report_id])
GO
ALTER TABLE [dbo].[MonthlyDIR_Totals]  WITH CHECK ADD FOREIGN KEY([record_id])
REFERENCES [dbo].[MonthlyDIR_ReportMain] ([report_id])
GO
ALTER TABLE [dbo].[MonthlyDIR_Trunking]  WITH CHECK ADD FOREIGN KEY([report_id])
REFERENCES [dbo].[MonthlyDIR_ReportMain] ([report_id])
GO
ALTER TABLE [dbo].[MonthlyDIR_Vsat]  WITH CHECK ADD FOREIGN KEY([report_id])
REFERENCES [dbo].[MonthlyDIR_ReportMain] ([report_id])
GO
ALTER TABLE [dbo].[PaymentBatch]  WITH CHECK ADD FOREIGN KEY([BankCodeId])
REFERENCES [dbo].[BankCode] ([BankCodeId])
GO
