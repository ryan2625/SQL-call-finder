using System.Data.SqlClient;
using System.Windows;

namespace SqlCommandVisualizerTestApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var query = @"

IF NOT EXISTS(SELECT id FROM syscolumns WHERE id = OBJECT_ID('actions') AND name = 'AC_IsActionForRestriction')
	ALTER TABLE actions ADD AC_IsActionForRestriction BIT NOT NULL DEFAULT(0)
GO

UPDATE actions SET ac_isactionforrestriction = 1, ac_customscript = NULL, ac_name = 'Справочники -> ' + ac_name
WHERE ac_key IN (104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131,132,133,134,135,136,137,138,139)
GO

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'QT_IsByCheckIn' AND id = OBJECT_ID(N'[dbo].[Quotas]'))
	ALTER TABLE dbo.Quotas ADD QT_IsByCheckIn BIT NOT NULL DEFAULT(0)
GO

IF NOT EXISTS (SELECT top 1 1 FROM sys.tables t WHERE t.name='Users')
BEGIN
	CREATE TABLE [dbo].[Users](
		[US_Id] [int] IDENTITY(1,1) NOT NULL,
		[US_Login] [nvarchar](50) NOT NULL,
		[US_Password] [nvarchar](64) NOT NULL,
		[US_PRKey] [int] NOT NULL,
		[US_CreatorId] [int] NOT NULL,
		[US_Name] [nvarchar](30) NULL,
		[US_Surname] [nvarchar](30) NULL,
		[US_SecondName] [nvarchar](30) NULL,
		[US_Birthday] [datetime] NOT NULL,
		[US_LastLogDate] [datetime] NOT NULL,
		[US_PassExpireTime] [int] NOT NULL,
		[US_LastPassChange] [datetime] NOT NULL,
		[US_ChangePassword] [bit] NOT NULL,
		[US_DisableUser] [bit] NOT NULL,
		[US_DateCreate] [datetime] NOT NULL,
	 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED
	(
		[US_Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]


	ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_tbl_Partners] FOREIGN KEY([US_PRKey])
	REFERENCES [dbo].[tbl_Partners] ([PR_KEY])

	ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_tbl_Partners]

	ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Users] FOREIGN KEY([US_CreatorId])
	REFERENCES [dbo].[Users] ([US_Id])

	ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Users]
END

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'US_UsersId' AND id = OBJECT_ID(N'[dbo].[UserList]'))
BEGIN
	ALTER TABLE UserList ADD [US_UsersId] [int] NULL
	ALTER TABLE [dbo].[UserList]  WITH CHECK ADD  CONSTRAINT [FK_UserList_Users] FOREIGN KEY([US_UsersId]) REFERENCES [dbo].[Users] ([US_Id])
END
GO

IF NOT EXISTS (SELECT top 1 1 FROM sys.tables t WHERE t.name='TransfertDirection')
BEGIN
	CREATE TABLE [dbo].[TransfertDirection](
		[TFD_Id] [int] IDENTITY(1,1) NOT NULL,
		[TFD_Name] [nvarchar](128) NOT NULL,
	 CONSTRAINT [PK_TransfertDirection] PRIMARY KEY CLUSTERED
	(
		[TFD_Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT top 1 1 FROM sys.tables t WHERE t.name='Role')
BEGIN
	CREATE TABLE [dbo].[Role](
		[RL_ID] [int] IDENTITY(1,1) NOT NULL,
		[RL_NAME] [nvarchar](450) NOT NULL,
		[RL_ROLETYPE] [int] NOT NULL,
		[RL_PRKEY] [int] NULL,
		[RL_USKEY] [int] NULL,
	 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED
	(
		[RL_ID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[Role]  WITH CHECK ADD  CONSTRAINT [FK_Role_DUP_USER] FOREIGN KEY([RL_USKEY])
	REFERENCES [dbo].[DUP_USER] ([US_KEY])

	ALTER TABLE [dbo].[Role] CHECK CONSTRAINT [FK_Role_DUP_USER]

	ALTER TABLE [dbo].[Role]  WITH CHECK ADD  CONSTRAINT [FK_Role_tbl_Partners] FOREIGN KEY([RL_PRKEY])
	REFERENCES [dbo].[tbl_Partners] ([PR_KEY])

	ALTER TABLE [dbo].[Role] CHECK CONSTRAINT [FK_Role_tbl_Partners]

	ALTER TABLE [dbo].[Role]  WITH CHECK ADD  CONSTRAINT [FK_Role_Users] FOREIGN KEY([RL_USKEY])
	REFERENCES [dbo].[Users] ([US_Id])

	ALTER TABLE [dbo].[Role] CHECK CONSTRAINT [FK_Role_Users]
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'pr_sortOrder' AND id = OBJECT_ID(N'[dbo].[tbl_Partners]'))
BEGIN
	ALTER TABLE tbl_Partners ADD [pr_sortOrder] [integer] NOT NULL DEFAULT 0
	ALTER TABLE tbl_Partners ADD [PK_RLID] [integer]
	ALTER TABLE [dbo].[tbl_Partners]  WITH CHECK ADD CONSTRAINT [FK_tbl_Partners_Role] FOREIGN KEY([PK_RLID]) REFERENCES [dbo].[Role] ([RL_ID])
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'A2_ShowOrder' AND id = OBJECT_ID(N'[dbo].[AddDescript2]'))
	ALTER TABLE [dbo].[AddDescript2] ADD [A2_ShowOrder] [int] NOT NULL DEFAULT (0)
GO

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'A2_Url' AND id = OBJECT_ID(N'[dbo].[AddDescript2]'))
	ALTER TABLE [dbo].[AddDescript2] ADD [A2_Url] [varchar](192)
GO

IF NOT EXISTS (SELECT top 1 1 FROM sys.tables t WHERE t.name='TourContent')
BEGIN
	CREATE TABLE [dbo].[TourContent](
		[TC_Key] [int] IDENTITY(0,1) NOT NULL,
		[TC_Name] [nvarchar](200) NULL,
		[TC_NameLat] [nvarchar](200) NULL,
	 CONSTRAINT [PK_TourContent] PRIMARY KEY CLUSTERED
	(
		[TC_Key] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 70) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'TL_CalculationAction' AND id = OBJECT_ID(N'[dbo].[tbl_TurList]'))
	ALTER TABLE tbl_TurList ADD TL_CalculationAction INT NOT NULL DEFAULT(0)
GO

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'TL_TCKey' AND id = OBJECT_ID(N'[dbo].[tbl_TurList]'))
BEGIN
	ALTER TABLE tbl_TurList ADD [TL_TCKey] [int]
	ALTER TABLE [dbo].[tbl_TurList]  WITH CHECK ADD  CONSTRAINT [FK_tbl_TurList_TourContent] FOREIGN KEY([TL_TCKey]) REFERENCES [dbo].[TourContent] ([TC_Key])
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'AC_NADMAIN' AND id = OBJECT_ID(N'[dbo].[Accmdmentype]'))
	ALTER TABLE Accmdmentype ADD [AC_NADMAIN] [int]
GO

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'AC_NCHMAIN' AND id = OBJECT_ID(N'[dbo].[Accmdmentype]'))
	ALTER TABLE Accmdmentype ADD [AC_NCHMAIN] [int]
GO

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'AC_NADEXTRA' AND id = OBJECT_ID(N'[dbo].[Accmdmentype]'))
	ALTER TABLE Accmdmentype ADD [AC_NADEXTRA] [int]
GO

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'AC_NCHEXTRA' AND id = OBJECT_ID(N'[dbo].[Accmdmentype]'))
	ALTER TABLE Accmdmentype ADD [AC_NCHEXTRA] [int]
GO

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'AC_NCHISINFMAIN' AND id = OBJECT_ID(N'[dbo].[Accmdmentype]'))
	ALTER TABLE Accmdmentype ADD [AC_NCHISINFMAIN] [int]
GO

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'AC_NCHISINFEXTRA' AND id = OBJECT_ID(N'[dbo].[Accmdmentype]'))
	ALTER TABLE Accmdmentype ADD [AC_NCHISINFEXTRA] [int]
GO

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'SL_Url' AND id = OBJECT_ID(N'[dbo].[ServiceList]'))
	ALTER TABLE ServiceList ADD [SL_Url] [varchar](192) NULL
GO

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'SL_ShowOrder' AND id = OBJECT_ID(N'[dbo].[ServiceList]'))
	ALTER TABLE ServiceList ADD [SL_ShowOrder] [int] NOT NULL DEFAULT(0)
GO

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'SL_quKey' AND id = OBJECT_ID(N'[dbo].[ServiceList]'))
	ALTER TABLE ServiceList ADD [SL_quKey] [int] NULL
GO

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'TF_Direction' AND id = OBJECT_ID(N'[dbo].[Transfer]'))
	ALTER TABLE [Transfer] ADD [TF_Direction] [int] NOT NULL DEFAULT(0)
GO

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'TF_Url' AND id = OBJECT_ID(N'[dbo].[Transfer]'))
	ALTER TABLE [Transfer] ADD [TF_Url] [varchar](192) NULL
GO

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'TF_ShowOrder' AND id = OBJECT_ID(N'[dbo].[Transfer]'))
	ALTER TABLE [Transfer] ADD [TF_ShowOrder] [int] NOT NULL DEFAULT(0)
GO

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'ED_Url' AND id = OBJECT_ID(N'[dbo].[ExcurDictionary]'))
	ALTER TABLE ExcurDictionary ADD [ED_Url] [varchar](192) NULL
GO

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'ED_ShowOrder' AND id = OBJECT_ID(N'[dbo].[ExcurDictionary]'))
	ALTER TABLE ExcurDictionary ADD [ED_ShowOrder] [int] NOT NULL DEFAULT(0)
GO

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'CP_ExposeWeb' AND id = OBJECT_ID(N'[dbo].[CalculatingPriceLists]'))
	ALTER TABLE CalculatingPriceLists ADD [CP_ExposeWeb] [smallint] NOT NULL DEFAULT(0)
GO

IF NOT EXISTS (SELECT 1 FROM dbo.syscolumns WHERE name = 'CP_Priority' AND id = OBJECT_ID(N'[dbo].[CalculatingPriceLists]'))
	ALTER TABLE CalculatingPriceLists ADD [CP_Priority] [smallint] NOT NULL DEFAULT(0)
GO

";

            var visualizer = new SqlCommandVisualizer.SqlCommandDialogDebuggerVisualizer();
            visualizer.ShowDialog(query);

            var cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("id", 16);
        }
    }
}
