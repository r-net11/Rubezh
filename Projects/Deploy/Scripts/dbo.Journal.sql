USE [Firesec]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Journal]') AND type in (N'U'))
DROP TABLE [dbo].[Journal]
GO
/****** Объект:  Table [dbo].[Journal]    Дата сценария: 11/11/2011 18:46:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Journal](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DeviceTime] [datetime] NULL,
	[SystemTime] [datetime] NULL,
	[ZoneName] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[DeviceName] [nvarchar](max) NULL,
	[PanelName] [nvarchar](max) NULL,
	[DeviceDatabaseId] [nvarchar](max) NULL,
	[PanelDatabaseId] [nvarchar](max) NULL,
	[UserName] [nvarchar](max) NULL,
	[StateType] [int] NULL,
	[SubSystemType] [int] NULL,
	[Detalization] [nvarchar](max) NULL,
	[OldId] [int] NULL
 CONSTRAINT [PK_Journal] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
