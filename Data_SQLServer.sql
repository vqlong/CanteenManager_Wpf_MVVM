USE [master]
GO
/****** Object:  Database [CanteenManager]    Script Date: 10/13/2022 3:31:27 AM ******/
CREATE DATABASE [CanteenManager]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'CanteenManager', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\CanteenManager.mdf' , SIZE = 3584KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'CanteenManager_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\CanteenManager_log.ldf' , SIZE = 3976KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [CanteenManager] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CanteenManager].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CanteenManager] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [CanteenManager] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [CanteenManager] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [CanteenManager] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [CanteenManager] SET ARITHABORT OFF 
GO
ALTER DATABASE [CanteenManager] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [CanteenManager] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [CanteenManager] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [CanteenManager] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [CanteenManager] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [CanteenManager] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [CanteenManager] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [CanteenManager] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [CanteenManager] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [CanteenManager] SET  DISABLE_BROKER 
GO
ALTER DATABASE [CanteenManager] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [CanteenManager] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [CanteenManager] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [CanteenManager] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [CanteenManager] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [CanteenManager] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [CanteenManager] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [CanteenManager] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [CanteenManager] SET  MULTI_USER 
GO
ALTER DATABASE [CanteenManager] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [CanteenManager] SET DB_CHAINING OFF 
GO
ALTER DATABASE [CanteenManager] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [CanteenManager] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [CanteenManager] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [CanteenManager] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [CanteenManager] SET QUERY_STORE = OFF
GO
USE [CanteenManager]
GO
/****** Object:  User [admin]    Script Date: 10/13/2022 3:31:27 AM ******/
CREATE USER [admin] FOR LOGIN [admin] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [admin]
GO
/****** Object:  UserDefinedFunction [dbo].[UF_ConvertToUnsigned]    Script Date: 10/13/2022 3:31:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Convert có dấu thành không dấu
CREATE FUNCTION [dbo].[UF_ConvertToUnsigned] ( @strInput NVARCHAR(4000) ) 
RETURNS NVARCHAR(4000) 
AS 
BEGIN 
	IF @strInput IS NULL RETURN @strInput 
	IF @strInput = '' RETURN @strInput 
	DECLARE @RT NVARCHAR(4000) 
	DECLARE @SIGN_CHARS NCHAR(136) 
	DECLARE @UNSIGN_CHARS NCHAR (136) 
	SET @SIGN_CHARS = N'ăâđêôơưàảãạáằẳẵặắầẩẫậấèẻẽẹéềểễệế ìỉĩịíòỏõọóồổỗộốờởỡợớùủũụúừửữựứỳỷỹỵý ĂÂĐÊÔƠƯÀẢÃẠÁẰẲẴẶẮẦẨẪẬẤÈẺẼẸÉỀỂỄỆẾÌỈĨỊÍ ÒỎÕỌÓỒỔỖỘỐỜỞỠỢỚÙỦŨỤÚỪỬỮỰỨỲỶỸỴÝ' +NCHAR(272)+ NCHAR(208) 
	SET @UNSIGN_CHARS = N'aadeoouaaaaaaaaaaaaaaaeeeeeeeeee iiiiiooooooooooooooouuuuuuuuuuyyyyy AADEOOUAAAAAAAAAAAAAAAEEEEEEEEEEIIIII OOOOOOOOOOOOOOOUUUUUUUUUUYYYYYDD' 
	DECLARE @COUNTER int 
	DECLARE @COUNTER1 int 
	SET @COUNTER = 1 
	WHILE (@COUNTER <=LEN(@strInput)) 
		BEGIN 
			SET @COUNTER1 = 1 
			WHILE (@COUNTER1 <=LEN(@SIGN_CHARS)+1) 
				BEGIN 
					IF UNICODE(SUBSTRING(@SIGN_CHARS, @COUNTER1,1)) = UNICODE(SUBSTRING(@strInput,@COUNTER ,1) ) 
						BEGIN 
							IF @COUNTER=1 
								SET @strInput = SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)-1) 
							ELSE 
								SET @strInput = SUBSTRING(@strInput, 1, @COUNTER-1) +SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)- @COUNTER) 
							BREAK 
						END 
					SET @COUNTER1 = @COUNTER1 +1 
				END 
			SET @COUNTER = @COUNTER +1 
		END 
		SET @strInput = replace(@strInput,' ','-') 
		RETURN @strInput 
END
GO
/****** Object:  Table [dbo].[Account]    Script Date: 10/13/2022 3:31:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Account](
	[UserName] [nvarchar](100) NOT NULL,
	[DisplayName] [nvarchar](100) NOT NULL,
	[PassWord] [nvarchar](1000) NOT NULL,
	[AccType] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Bill]    Script Date: 10/13/2022 3:31:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bill](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateCheckIn] [datetime] NOT NULL,
	[DateCheckOut] [datetime] NULL,
	[TableID] [int] NOT NULL,
	[BillStatus] [int] NOT NULL,
	[Discount] [int] NOT NULL,
	[TotalPrice] [float] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BillInfo]    Script Date: 10/13/2022 3:31:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BillInfo](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[BillID] [int] NOT NULL,
	[FoodID] [int] NOT NULL,
	[FoodCount] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Food]    Script Date: 10/13/2022 3:31:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Food](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[CategoryID] [int] NOT NULL,
	[Price] [float] NOT NULL,
	[FoodStatus] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FoodCategory]    Script Date: 10/13/2022 3:31:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FoodCategory](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[CategoryStatus] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TableFood]    Script Date: 10/13/2022 3:31:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TableFood](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[TableStatus] [nvarchar](100) NOT NULL,
	[UsingState] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[Account] ([UserName], [DisplayName], [PassWord], [AccType]) VALUES (N'admin', N'Quán chủ', N'1661011648932664715765126721032392207918416074316325531160126153142134247247162122227', 1)
INSERT [dbo].[Account] ([UserName], [DisplayName], [PassWord], [AccType]) VALUES (N'an', N'Em bé', N'952362351022552001115621782120108109105108121194219194572217814518010341215583925187233', 1)
INSERT [dbo].[Account] ([UserName], [DisplayName], [PassWord], [AccType]) VALUES (N'hien', N'Mẹ em bé', N'952362351022552001115621782120108109105108121194219194572217814518010341215583925187233', 1)
GO
SET IDENTITY_INSERT [dbo].[Bill] ON 

INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (1, CAST(N'2022-06-20T23:27:52.373' AS DateTime), CAST(N'2022-06-20T23:28:11.783' AS DateTime), 1, 1, 69, 18600)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (2, CAST(N'2022-06-20T23:55:06.967' AS DateTime), CAST(N'2022-06-20T23:55:12.220' AS DateTime), 2, 1, 0, 60000)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (4, CAST(N'2022-06-20T23:59:24.917' AS DateTime), CAST(N'2022-06-20T23:59:31.667' AS DateTime), 4, 1, 0, 60000)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (5, CAST(N'2022-06-21T00:03:31.570' AS DateTime), CAST(N'2022-06-21T00:03:39.813' AS DateTime), 6, 1, 69, 23250)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (6, CAST(N'2022-06-21T00:14:50.160' AS DateTime), CAST(N'2022-06-21T00:14:56.020' AS DateTime), 7, 1, 0, 25000)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (7, CAST(N'2022-06-21T00:23:10.563' AS DateTime), CAST(N'2022-06-21T00:25:39.610' AS DateTime), 10, 1, 0, 45000)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (8, CAST(N'2022-06-21T00:23:13.110' AS DateTime), CAST(N'2022-06-21T00:26:01.030' AS DateTime), 11, 1, 0, 45000)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (9, CAST(N'2022-06-21T00:23:14.910' AS DateTime), CAST(N'2022-06-21T00:26:13.973' AS DateTime), 12, 1, 0, 45000)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (10, CAST(N'2022-06-21T00:28:42.373' AS DateTime), CAST(N'2022-06-21T00:29:14.143' AS DateTime), 6, 1, 0, 32000)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (11, CAST(N'2022-06-21T00:28:54.793' AS DateTime), CAST(N'2022-06-21T00:29:47.173' AS DateTime), 7, 1, 0, 40000)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (12, CAST(N'2022-06-21T00:28:57.347' AS DateTime), CAST(N'2022-06-21T00:30:04.440' AS DateTime), 8, 1, 0, 48000)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (13, CAST(N'2022-07-21T00:34:06.650' AS DateTime), CAST(N'2022-07-23T22:05:02.690' AS DateTime), 6, 1, 0, 36000)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (14, CAST(N'2022-07-21T00:34:10.540' AS DateTime), CAST(N'2022-07-22T17:15:35.083' AS DateTime), 10, 1, 1, 47520)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (15, CAST(N'2022-07-21T00:34:17.340' AS DateTime), CAST(N'2022-07-22T17:15:27.943' AS DateTime), 12, 1, 0, 57000)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (16, CAST(N'2022-07-21T00:34:22.117' AS DateTime), CAST(N'2022-07-22T17:15:24.807' AS DateTime), 8, 1, 0, 30000)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (17, CAST(N'2022-08-21T00:49:30.460' AS DateTime), CAST(N'2022-08-27T22:16:03.150' AS DateTime), 7, 1, 99, 7049.9399999999441)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (19, CAST(N'2022-08-21T00:52:04.197' AS DateTime), CAST(N'2022-08-23T22:05:58.830' AS DateTime), 14, 1, 0, 73225)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (20, CAST(N'2022-08-21T00:52:13.713' AS DateTime), CAST(N'2022-08-22T17:15:44.900' AS DateTime), 9, 1, 2, 61519.5)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (23, CAST(N'2022-08-21T00:55:03.117' AS DateTime), CAST(N'2022-08-23T22:05:45.623' AS DateTime), 16, 1, 0, 39995)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (24, CAST(N'2022-08-24T23:12:13.103' AS DateTime), CAST(N'2022-08-24T23:12:17.247' AS DateTime), 6, 1, 0, 39995)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (27, CAST(N'2022-08-30T14:48:05.260' AS DateTime), CAST(N'2022-08-30T14:48:11.037' AS DateTime), 1, 1, 0, 98568)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (28, CAST(N'2022-08-30T14:49:07.017' AS DateTime), CAST(N'2022-08-30T14:49:11.470' AS DateTime), 1, 1, 0, 3000)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (29, CAST(N'2022-09-01T17:19:48.200' AS DateTime), CAST(N'2022-09-01T17:29:59.683' AS DateTime), 10, 1, 0, 220000)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (30, CAST(N'2022-09-01T17:31:08.623' AS DateTime), CAST(N'2022-09-01T17:48:36.407' AS DateTime), 20, 1, 0, 520000)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (31, CAST(N'2022-09-01T19:05:25.963' AS DateTime), CAST(N'2022-09-01T21:29:46.027' AS DateTime), 10, 1, 0, 616050)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (32, CAST(N'2022-09-02T01:02:03.967' AS DateTime), CAST(N'2022-09-02T01:02:08.987' AS DateTime), 6, 1, 0, 199975)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (33, CAST(N'2022-09-02T19:14:02.437' AS DateTime), CAST(N'2022-09-02T20:26:33.957' AS DateTime), 18, 1, 3, 803579.04)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (34, CAST(N'2022-09-02T23:41:14.177' AS DateTime), CAST(N'2022-09-02T23:42:52.777' AS DateTime), 6, 1, 8, 7121062.2)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (35, CAST(N'2022-09-03T12:53:19.113' AS DateTime), CAST(N'2022-09-03T12:53:57.827' AS DateTime), 9, 1, 69, 53953.020000000004)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (36, CAST(N'2022-09-03T13:07:58.490' AS DateTime), CAST(N'2022-09-03T13:08:03.193' AS DateTime), 7, 1, 0, 29007)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (37, CAST(N'2022-09-03T15:01:32.940' AS DateTime), CAST(N'2022-09-03T15:01:35.770' AS DateTime), 14, 1, 0, 29007)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (38, CAST(N'2022-09-03T15:12:13.113' AS DateTime), CAST(N'2022-09-03T15:12:16.430' AS DateTime), 12, 1, 0, 61605)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (39, CAST(N'2022-09-03T15:15:54.483' AS DateTime), CAST(N'2022-09-06T11:29:42.140' AS DateTime), 12, 1, 0, 12321)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (42, CAST(N'2022-09-18T19:26:01.993' AS DateTime), CAST(N'2022-09-18T19:27:20.447' AS DateTime), 3, 1, 0, 119188)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (44, CAST(N'2022-10-13T02:02:33.560' AS DateTime), CAST(N'2022-10-13T02:03:23.330' AS DateTime), 14, 1, 0, 799992)
INSERT [dbo].[Bill] ([ID], [DateCheckIn], [DateCheckOut], [TableID], [BillStatus], [Discount], [TotalPrice]) VALUES (45, CAST(N'2022-10-13T02:02:45.533' AS DateTime), CAST(N'2022-10-13T02:02:54.610' AS DateTime), 7, 1, 1, 97582.32)
SET IDENTITY_INSERT [dbo].[Bill] OFF
GO
SET IDENTITY_INSERT [dbo].[BillInfo] ON 

INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (1, 1, 18, 4)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (2, 2, 1, 4)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (4, 4, 16, 4)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (5, 5, 18, 5)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (6, 6, 2, 1)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (7, 7, 16, 3)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (8, 8, 16, 3)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (9, 9, 16, 3)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (10, 10, 5, 4)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (11, 11, 5, 5)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (12, 12, 5, 6)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (13, 13, 1, 3)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (14, 14, 1, 4)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (15, 15, 4, 3)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (16, 16, 12, 3)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (17, 17, 19, 6)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (18, 17, 7, 5)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (19, 17, 16, 5)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (22, 19, 25, 5)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (23, 19, 21, 5)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (24, 20, 22, 5)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (25, 20, 14, 5)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (28, 23, 27, 5)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (29, 24, 27, 5)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (33, 27, 26, 8)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (34, 28, 30, 6)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (35, 29, 13, 20)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (36, 30, 15, 40)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (37, 31, 26, 50)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (38, 32, 27, 25)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (39, 33, 19, 6)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (40, 33, 26, 18)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (41, 33, 22, 12)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (42, 34, 19, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (43, 34, 5, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (44, 34, 6, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (45, 34, 7, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (46, 34, 8, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (47, 34, 9, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (48, 34, 10, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (49, 34, 11, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (50, 34, 12, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (51, 34, 13, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (52, 34, 14, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (53, 34, 15, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (54, 34, 26, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (55, 34, 27, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (56, 34, 28, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (57, 34, 16, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (58, 34, 20, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (59, 34, 24, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (60, 34, 25, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (61, 34, 30, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (62, 34, 17, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (63, 34, 18, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (64, 34, 21, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (65, 34, 22, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (66, 34, 23, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (67, 34, 29, 21)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (68, 35, 31, 18)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (69, 36, 31, 3)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (70, 37, 31, 3)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (71, 38, 26, 5)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (72, 39, 26, 1)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (73, 42, 19, 1)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (74, 42, 26, 1)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (75, 42, 25, 1)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (77, 44, 19, 8)
INSERT [dbo].[BillInfo] ([ID], [BillID], [FoodID], [FoodCount]) VALUES (78, 45, 26, 8)
SET IDENTITY_INSERT [dbo].[BillInfo] OFF
GO
SET IDENTITY_INSERT [dbo].[Food] ON 

INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (1, N'Tiết luộc', 1, 12000, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (2, N'Cánh gà nướng', 1, 25000, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (3, N'Ốc xào xả ớt', 1, 18000, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (4, N'Nem chua rán', 1, 19000, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (5, N'Cam', 2, 8008, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (6, N'Quýt', 2, 7000, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (7, N'Mít', 2, 6000, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (8, N'Dừa', 2, 9000, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (9, N'Dưa', 2, 5000, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (10, N'Dứa', 2, 2000, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (11, N'Ổi', 2, 3000, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (12, N'Cocacola', 3, 10000, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (13, N'Pepsi', 3, 11000, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (14, N'Bia', 3, 12000, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (15, N'Sữa chua mít', 3, 13000, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (16, N'Chè bưởi', 4, 15000, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (17, N'Cháo lòng', 5, 25000, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (18, N'Cháo lưỡi', 5, 69996, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (19, N'Su su luộc chấm muối vừng', 1, 99999, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (20, N'Chè sầu', 4, 8888, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (21, N'Thuốc lá', 6, 7777, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (22, N'Thuốc lào', 6, 555, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (23, N'Thuốc phiện', 6, 7878, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (24, N'Chè đỗ đen', 4, 9898, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (25, N'Chè tươi Thái Nguyên', 4, 6868, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (26, N'Nước lã', 3, 12321, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (27, N'Yakult', 3, 7999, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (28, N'Lavie', 3, 999, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (29, N'Thuốc lắc', 6, 8899, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (30, N'Chè xanh', 4, 500, 1)
INSERT [dbo].[Food] ([ID], [Name], [CategoryID], [Price], [FoodStatus]) VALUES (31, N'Cháo bẹ măng tre', 5, 9669, 1)
SET IDENTITY_INSERT [dbo].[Food] OFF
GO
SET IDENTITY_INSERT [dbo].[FoodCategory] ON 

INSERT [dbo].[FoodCategory] ([ID], [Name], [CategoryStatus]) VALUES (1, N'Món ăn', 1)
INSERT [dbo].[FoodCategory] ([ID], [Name], [CategoryStatus]) VALUES (2, N'Hoa quả', 1)
INSERT [dbo].[FoodCategory] ([ID], [Name], [CategoryStatus]) VALUES (3, N'Nước', 1)
INSERT [dbo].[FoodCategory] ([ID], [Name], [CategoryStatus]) VALUES (4, N'Chè', 1)
INSERT [dbo].[FoodCategory] ([ID], [Name], [CategoryStatus]) VALUES (5, N'Cháo', 1)
INSERT [dbo].[FoodCategory] ([ID], [Name], [CategoryStatus]) VALUES (6, N'Thuốc', 1)
SET IDENTITY_INSERT [dbo].[FoodCategory] OFF
GO
SET IDENTITY_INSERT [dbo].[TableFood] ON 

INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (1, N'Bàn 1', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (2, N'Bàn 2', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (3, N'Bàn 3', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (4, N'Bàn 4', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (5, N'Bàn 5', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (6, N'Bàn 6', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (7, N'Bàn 7', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (8, N'Bàn 8', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (9, N'Bàn 9', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (10, N'Bàn 10', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (11, N'Bàn 11', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (12, N'Bàn 12', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (13, N'Bàn 13', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (14, N'Bàn 14', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (15, N'Bàn 15', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (16, N'Bàn 16', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (17, N'Bàn 17', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (18, N'Bàn 18', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (19, N'Bàn 19', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (20, N'Bàn 20', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (21, N'Bàn 21', N'Trống', 0)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (22, N'Bàn 22', N'Trống', 0)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (23, N'Bàn 23', N'Trống', 0)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (24, N'Bàn 24', N'Trống', 0)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (25, N'Bàn 25', N'Trống', 0)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (26, N'Bàn 26', N'Trống', 0)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (27, N'Bàn 27', N'Trống', 0)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (28, N'Bàn 28', N'Trống', 0)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (29, N'Bàn 29', N'Trống', 0)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (30, N'Bàn 30', N'Trống', 0)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (31, N'Bàn 31', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (32, N'Bàn 32', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (33, N'Bàn 33', N'Trống', 1)
INSERT [dbo].[TableFood] ([ID], [Name], [TableStatus], [UsingState]) VALUES (34, N'Bàn 34', N'Trống', 1)
SET IDENTITY_INSERT [dbo].[TableFood] OFF
GO
ALTER TABLE [dbo].[Account] ADD  DEFAULT (N'CafeNo1') FOR [DisplayName]
GO
ALTER TABLE [dbo].[Account] ADD  DEFAULT (N'952362351022552001115621782120108109105108121194219194572217814518010341215583925187233') FOR [PassWord]
GO
ALTER TABLE [dbo].[Account] ADD  DEFAULT ((0)) FOR [AccType]
GO
ALTER TABLE [dbo].[Bill] ADD  DEFAULT (getdate()) FOR [DateCheckIn]
GO
ALTER TABLE [dbo].[Bill] ADD  DEFAULT ((0)) FOR [BillStatus]
GO
ALTER TABLE [dbo].[Bill] ADD  DEFAULT ((0)) FOR [Discount]
GO
ALTER TABLE [dbo].[Bill] ADD  DEFAULT ((0)) FOR [TotalPrice]
GO
ALTER TABLE [dbo].[BillInfo] ADD  DEFAULT ((0)) FOR [FoodCount]
GO
ALTER TABLE [dbo].[Food] ADD  DEFAULT (N'Chưa đặt tên') FOR [Name]
GO
ALTER TABLE [dbo].[Food] ADD  DEFAULT ((0)) FOR [Price]
GO
ALTER TABLE [dbo].[Food] ADD  DEFAULT ((1)) FOR [FoodStatus]
GO
ALTER TABLE [dbo].[FoodCategory] ADD  DEFAULT (N'Chưa đặt tên') FOR [Name]
GO
ALTER TABLE [dbo].[FoodCategory] ADD  DEFAULT ((1)) FOR [CategoryStatus]
GO
ALTER TABLE [dbo].[TableFood] ADD  DEFAULT (N'Chưa đặt tên') FOR [Name]
GO
ALTER TABLE [dbo].[TableFood] ADD  DEFAULT (N'Trống') FOR [TableStatus]
GO
ALTER TABLE [dbo].[TableFood] ADD  DEFAULT ((1)) FOR [UsingState]
GO
ALTER TABLE [dbo].[Bill]  WITH CHECK ADD FOREIGN KEY([TableID])
REFERENCES [dbo].[TableFood] ([ID])
GO
ALTER TABLE [dbo].[BillInfo]  WITH CHECK ADD FOREIGN KEY([BillID])
REFERENCES [dbo].[Bill] ([ID])
GO
ALTER TABLE [dbo].[BillInfo]  WITH CHECK ADD FOREIGN KEY([FoodID])
REFERENCES [dbo].[Food] ([ID])
GO
ALTER TABLE [dbo].[Food]  WITH CHECK ADD FOREIGN KEY([CategoryID])
REFERENCES [dbo].[FoodCategory] ([ID])
GO
/****** Object:  StoredProcedure [dbo].[USP_CheckOut]    Script Date: 10/13/2022 3:31:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--proc thanh toán Bill
CREATE PROC [dbo].[USP_CheckOut]
@billID INT,
@discount INT,
@totalPrice FLOAT
AS
BEGIN
	--Khi Bill được thanh toán => BillStatus = 1
	UPDATE Bill SET BillStatus = 1, DateCheckOut = GETDATE(), Discount = @discount, TotalPrice = @totalPrice  WHERE ID = @billID

	--DECLARE @tableID INT
	--Theo thiết kế mỗi Bill chỉ có 1 ID duy nhất => kết quả trả về luôn chỉ có 1 hàng => Dùng hàm MAX/MIN để lấy được TableID của Bill
	--SELECT @tableID = MAX(Bill.TableID) FROM Bill WHERE Bill.ID = @billID
	--Khi 1 bàn được thanh toán Bill, nó sẽ chuyển sang trạng thái 'Trống'
	--UPDATE TableFood SET TableStatus = N'Trống' WHERE TableFood.ID = @tableID
END
GO
/****** Object:  StoredProcedure [dbo].[USP_CombineTable]    Script Date: 10/13/2022 3:31:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Proc gộp bàn thứ 1 vào bàn thứ 2
CREATE PROC [dbo].[USP_CombineTable]
@firstTableID INT,
@secondTableID INT
AS
BEGIN
    DECLARE @firstBillID INT = 0
    DECLARE @secondBillID INT = 0
    DECLARE @result INT = 0
    --Lấy ra BillID của Bill chưa thanh toán trên 2 bàn cần gộp vào nhau
    SELECT @firstBillID = Bill.ID FROM Bill WHERE TableID = @firstTableID AND BillStatus = 0
    SELECT @secondBillID = Bill.ID FROM Bill WHERE TableID = @secondTableID AND BillStatus = 0

    --Nếu 2 bàn đều có người thì mới tiến hành gộp
    IF(@firstBillID != 0 AND @secondBillID != 0)
        BEGIN
            --Gộp bàn thứ 1 vào bàn thứ 2 bằng cách thay BillID trong bảng BillInfo
            UPDATE BillInfo SET BillID = @secondBillID WHERE BillID = @firstBillID
            SET @result = 1

            --Khi gộp bàn sẽ xuất hiện các món trùng lặp với nhau
            --Tạo con trỏ và lấy ra các FoodID với số lần trùng lặp
            DECLARE BillInfoCursor CURSOR FOR SELECT FoodID, count(*) AS 'Count' FROM BillInfo WHERE BillID = @secondBillID GROUP BY FoodID
            OPEN BillInfoCursor

            DECLARE @foodID INT
            DECLARE @count INT

            FETCH NEXT FROM BillInfoCursor INTO @foodID, @count

            WHILE @@FETCH_STATUS = 0
                BEGIN
                    --Trường hợp @count > 1 tức là món này bị trùng nhau, xuất hiện hơn 1 lần
                    IF(@count > 1) 
                        BEGIN
                            DECLARE @finalFoodCount INT = 0
                            --Tính gộp tổng các FoodCount của món này
                            SELECT @finalFoodCount = SUM(FoodCount) FROM BillInfo WHERE BillID = @secondBillID AND FoodID = @foodID
            
                            DECLARE @maxID INT = 0
                            --Lấy ra max ID của món này để tí nữa giữ lại, các ID khác xoá hết cho khỏi trùng nhau
                            SELECT @maxID = MAX(ID) FROM BillInfo WHERE BillID = @secondBillID AND FoodID = @foodID

                            --update số lượng món ăn cho ID này
                            UPDATE BillInfo SET FoodCount = @finalFoodCount WHERE BillID = @secondBillID AND FoodID = @foodID AND ID = @maxID --Cài nhiều điều kiện cho chắc kèo

                            --Xoá các ID còn lại
                            DELETE BillInfo WHERE BillID = @secondBillID AND FoodID = @foodID AND ID != @maxID
            
                        END

                    FETCH NEXT FROM BillInfoCursor INTO @foodID, @count
                END

            CLOSE BillInfoCursor
            DEALLOCATE BillInfoCursor

        END

    SELECT @result
END
GO
/****** Object:  StoredProcedure [dbo].[USP_GetListBillByDate]    Script Date: 10/13/2022 3:31:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Proc lấy danh sách các hoá đơn dựa theo ngày truyền vào
CREATE PROC [dbo].[USP_GetListBillByDate]
@fromDate DATETIME,
@toDate DATETIME
AS
BEGIN
	SELECT 
	Bill.ID, 
	Bill.DateCheckIn AS [Ngày phát sinh], 
	Bill.DateCheckOut AS [Ngày thanh toán], 
	TableFood.Name AS [Tên bàn], 
	Bill.Discount AS [Giảm giá (%)], 
	Bill.TotalPrice AS [Tiền thanh toán (Vnđ)]
	FROM Bill, TableFood 
	WHERE DateCheckIn >= @fromDate AND DateCheckOut <= @toDate AND BillStatus = 1 AND TableFood.ID = Bill.TableID
END
GO
/****** Object:  StoredProcedure [dbo].[USP_GetListBillByDateAndPage]    Script Date: 10/13/2022 3:31:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Proc lấy danh sách các hoá đơn dựa theo ngày và số trang truyền vào
CREATE PROC [dbo].[USP_GetListBillByDateAndPage]
@fromDate DATETIME,
@toDate DATETIME,
@pageNumber INT = 1, --Page muốn lấy về
@pageRow INT = 10 --Số dòng 1 page
AS
BEGIN
	DECLARE @totalRow INT = (SELECT COUNT(*) FROM Bill WHERE DateCheckIn >= @fromDate AND DateCheckOut <= @toDate AND BillStatus = 1) --Lấy tổng số dòng thoả mãn ngày truyền vào
	DECLARE @selectRow INT = @pageNumber * @pageRow --Lấy số dòng chứa page cần lấy
	DECLARE @exceptRow INT = (@pageNumber - 1) * @pageRow; --Lấy số dòng sẽ loại trừ ra bằng EXCEPT
	
	--IF @exceptRow < @totalRow
		--BEGIN
			WITH temp AS
			(
				SELECT 
				Bill.ID, 
				Bill.DateCheckIn AS [Ngày phát sinh], 
				Bill.DateCheckOut AS [Ngày thanh toán], 
				TableFood.Name AS [Tên bàn], 
				Bill.Discount AS [Giảm giá (%)], 
				Bill.TotalPrice AS [Tiền thanh toán (Vnđ)]
				FROM Bill, TableFood 
				WHERE DateCheckIn >= @fromDate AND DateCheckOut <= @toDate AND BillStatus = 1 AND TableFood.ID = Bill.TableID
			)
			SELECT TOP (@selectRow) * FROM temp 
			EXCEPT 
			SELECT TOP (@exceptRow) * FROM temp
		--END
END
GO
/****** Object:  StoredProcedure [dbo].[USP_GetListTable]    Script Date: 10/13/2022 3:31:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--proc lấy danh sách bàn ăn
CREATE PROC [dbo].[USP_GetListTable]
AS
SELECT * FROM dbo.TableFood
GO
/****** Object:  StoredProcedure [dbo].[USP_GetListTableUsing]    Script Date: 10/13/2022 3:31:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--proc lấy danh sách bàn ăn trong trạng thái còn sử dụng
CREATE PROC [dbo].[USP_GetListTableUsing]
AS
SELECT * FROM TableFood WHERE UsingState = 1
GO
/****** Object:  StoredProcedure [dbo].[USP_GetMenu]    Script Date: 10/13/2022 3:31:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[USP_GetMenu]
AS
	SET NOCOUNT ON;
SELECT        Food.Name AS Món, FoodCategory.Name AS Mục, Food.Price AS Giá
FROM            Food INNER JOIN
                         FoodCategory ON Food.CategoryID = FoodCategory.ID
GO
/****** Object:  StoredProcedure [dbo].[USP_GetRevenueByFoodAndDate]    Script Date: 10/13/2022 3:31:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Proc lấy tổng doanh thu (chưa tính giảm giá) của từng món ăn dựa theo ngày truyền vào
CREATE PROCEDURE [dbo].[USP_GetRevenueByFoodAndDate]
@fromDate DATETIME,
@toDate DATETIME
AS
BEGIN
	WITH temp AS
	(
		SELECT BillInfo.ID, BillID, FoodID, Name, FoodCount, Price, FoodCount * Price AS [TotalPrice], DateCheckIn, DateCheckOut 
		FROM dbo.BillInfo 
		JOIN dbo.Food ON Food.ID = BillInfo.FoodID
		JOIN dbo.Bill ON Bill.ID = BillInfo.BillID AND BillStatus = 1 AND DateCheckIn >= @fromDate AND DateCheckOut <= @toDate
	
	)
	SELECT temp.Name, SUM(temp.FoodCount) AS [TotalFoodCount], SUM(temp.TotalPrice) AS [Revenue] FROM temp GROUP BY temp.Name
END
GO
/****** Object:  StoredProcedure [dbo].[USP_GetRevenueByMonth]    Script Date: 10/13/2022 3:31:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Proc lấy tổng doanh thu (đã tính giảm giá) của từng tháng dựa theo ngày truyền vào
CREATE PROCEDURE [dbo].[USP_GetRevenueByMonth]
@fromDate DATETIME,
@toDate DATETIME
AS
BEGIN
	WITH temp AS
	(
		SELECT *, 
			Convert(datetime, CONCAT(YEAR(DateCheckOut), '-', MONTH(DateCheckOut), '-01')) AS [FirstDayInMonth], 
			CONCAT(Month(DateCheckOut), '-', Year(DateCheckOut)) AS [Month] 
		FROM dbo.Bill 
		WHERE BillStatus = 1 AND DateCheckIn >= @fromDate AND DateCheckOut <= @toDate	
	)
	SELECT temp.FirstDayInMonth, temp.Month, SUM(temp.TotalPrice) AS [Revenue] FROM temp GROUP BY temp.FirstDayInMonth, temp.Month ORDER BY temp.FirstDayInMonth
END
GO
/****** Object:  StoredProcedure [dbo].[USP_InsertBill]    Script Date: 10/13/2022 3:31:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--proc thêm Bill mới
--  ID: tự động thêm do ràng buộc IDENTITY
--	DateCheckIn: luôn là ngày hôm nay
--	DateCheckOut: luôn là NULL do hoá đơn mới tạo, chưa thanh toán
--	TableID: ID của bàn phát sinh hoá đơn
--	BillStatus: luôn là 0 - chưa thanh toán
--  Discount: luôn là 0 khi thêm Bill mới, sau khi thanh toán mới tính vào
CREATE PROC [dbo].[USP_InsertBill]
@tableID INT
AS
BEGIN
	--Thêm 1 Bill mới
	INSERT Bill VALUES(GETDATE(), NULL, @tableID, 0, 0, 0)
	--Khi 1 bàn có Bill mới, nó sẽ chuyển sang trạng thái 'Có người'
	--UPDATE TableFood SET TableStatus = N'Có người' WHERE TableFood.ID = @tableID
END
GO
/****** Object:  StoredProcedure [dbo].[USP_InsertBillInfo]    Script Date: 10/13/2022 3:31:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--proc thêm BillInfo mới
CREATE PROC [dbo].[USP_InsertBillInfo]
@billID INT,
@foodID INT,
@foodCount INT
AS
BEGIN
	DECLARE @isExistBillInfo INT
	DECLARE @currentFoodCount INT

	--Kiểm tra xem cái BillInfo này có tồn tại không (cái Bill này đã có BillInfo nào chưa, nếu có thì đã có món ăn này chưa)
	--Nếu tồn tại thì kết quả trả về cũng chỉ có 1 hàng duy nhất => Dùng hàm MAX hoặc MIN để lấy được FoodCount của món ăn
	SELECT @isExistBillInfo = COUNT(*), @currentFoodCount = MAX(FoodCount) FROM BillInfo WHERE BillInfo.BillID = @billID AND BillInfo.FoodID = @foodID

	--Nếu đã tồn tại thì update số lượng món đã gọi
	--Nếu không thì thêm mới
	IF (@isExistBillInfo > 0)
		BEGIN
			--Theo thiết kế @foodCount truyền vào có thể âm, nếu @newFoodCount <= 0 thì xoá món đó khỏi hoá đơn
			DECLARE @newFoodCount INT = @currentFoodCount + @foodCount
			IF (@newFoodCount <= 0)
				DELETE BillInfo WHERE BillInfo.BillID = @billID AND BillInfo.FoodID = @foodID
			ELSE
				UPDATE BillInfo SET FoodCount = @newFoodCount WHERE BillInfo.BillID = @billID AND BillInfo.FoodID = @foodID
		END
	ELSE
		--Nếu @foodCount > 0 (người dùng chọn số món > 0) mới thực hiện thêm
		IF (@foodCount > 0)
			BEGIN
				INSERT BillInfo VALUES(@billID, @foodID, @foodCount)
			END
	
END
GO
/****** Object:  StoredProcedure [dbo].[USP_InsertFood]    Script Date: 10/13/2022 3:31:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Proc thêm món ăn cho Food
CREATE PROC [dbo].[USP_InsertFood]
@name NVARCHAR(100),
@categoryID INT,
@price FLOAT
AS
BEGIN
	INSERT Food(Name, CategoryID, Price) VALUES(@name, @categoryID, @price)
END
GO
/****** Object:  StoredProcedure [dbo].[USP_InsertTable]    Script Date: 10/13/2022 3:31:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--proc tạo bàn mới
CREATE PROC [dbo].[USP_InsertTable]
AS
INSERT TableFood(Name) VALUES(N'Bàn ' + CAST((SELECT COUNT(*) FROM TableFood) + 1 AS NVARCHAR(100)))
GO
/****** Object:  StoredProcedure [dbo].[USP_Login]    Script Date: 10/13/2022 3:31:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--proc kiểm tra thông tin đăng nhập
CREATE PROC [dbo].[USP_Login]
@UserName NVARCHAR(100),
@PassWord NVARCHAR(100)
AS
BEGIN
    SELECT UserName, DisplayName, AccType FROM dbo.Account WHERE UserName = @UserName AND PassWord = @PassWord
END
GO
/****** Object:  StoredProcedure [dbo].[USP_SwapTable]    Script Date: 10/13/2022 3:31:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Proc chuyển bàn
--(bằng cách tráo đổi TableID của 2 Bill)
CREATE PROC [dbo].[USP_SwapTable]
@firstTableID INT,
@secondTableID INT
AS
BEGIN
	DECLARE @firstBillID INT = 0
	DECLARE @secondBillID INT = 0
	--Lấy ra BillID của Bill chưa thanh toán trên 2 bàn cần chuyển chỗ cho nhau
	SELECT @firstBillID = Bill.ID FROM Bill WHERE TableID = @firstTableID AND BillStatus = 0
	SELECT @secondBillID = Bill.ID FROM Bill WHERE TableID = @secondTableID AND BillStatus = 0
	
	IF(@firstBillID != 0) 
	--Nếu bàn thứ nhất được chọn có người thì mới tiến hành chuyển
		IF(@secondBillID !=0) 
		--Nếu bàn thứ hai được chọn cũng có người => tráo đổi TableID của 2 Bill cho nhau
			BEGIN
				UPDATE Bill SET TableID = @firstTableID WHERE ID = @secondBillID
				UPDATE Bill SET TableID = @secondTableID WHERE ID = @firstBillID
			END
		ELSE
		--Nếu bàn thứ hai được chọn không có người => chỉ cần thay đổi TableID của Bill
			BEGIN
				UPDATE Bill SET TableID = @secondTableID WHERE ID = @firstBillID
			END
	
END
GO
/****** Object:  StoredProcedure [dbo].[USP_UpdateAccount]    Script Date: 10/13/2022 3:31:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Proc update DisplayName, PassWord và AccType cho Account
CREATE PROC [dbo].[USP_UpdateAccount]
@userName NVARCHAR(100),
@displayName NVARCHAR(100) = NULL,
@passWord NVARCHAR(100) = NULL,
@accType INT = NULL
AS
BEGIN
	UPDATE Account SET Account.DisplayName = @displayName WHERE UserName = @userName AND @displayName IS NOT NULL AND @displayName != ''
	UPDATE Account SET Account.PassWord = @passWord WHERE UserName = @userName AND @passWord IS NOT NULL AND @passWord != ''
	UPDATE Account SET Account.AccType = @accType WHERE UserName = @userName AND @accType IS NOT NULL
END
GO
/****** Object:  StoredProcedure [dbo].[USP_UpdateFood]    Script Date: 10/13/2022 3:31:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Proc update món ăn cho Food
CREATE PROC [dbo].[USP_UpdateFood]
@id INT,
@name NVARCHAR(100),
@categoryID INT,
@price FLOAT,
@foodStatus INT
AS
BEGIN
	UPDATE Food SET Food.Name = @name, Food.CategoryID = @categoryID, Food.Price = @price, Food.FoodStatus = @foodStatus WHERE Food.ID = @id
END
GO
/****** Object:  Trigger [dbo].[UTG_UpdateBill]    Script Date: 10/13/2022 3:31:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Hiện tại sử dụng lệnh UPDATE Bill cho 2 mục đích:
--  UPDATE để chuyển BillStatus = 1 khi thanh toán => Trigger chuyển trạng thái của bàn sang 'Trống' mỗi khi nó được thanh toán
--  UPDATE để thay đổi TableID khi chuyển bàn => Trigger lấy ra TableID cũ và mới, đếm số lượng Bill chưa được thanh toán trên bàn và chuyển trạng thái tương ứng
CREATE TRIGGER [dbo].[UTG_UpdateBill]
ON [dbo].[Bill]
FOR UPDATE
AS
BEGIN
	DECLARE @id INT
	--Lấy ra ID của Bill vừa được update
	SELECT @id = ID FROM inserted
	
	DECLARE @tableID INT
	--Lấy ra TableID của Bill vừa được update
	SELECT @tableID = TableID FROM Bill WHERE ID = @id

	DECLARE @countBill INT = 0
	--Đếm số lượng Bill chưa được thanh toán trên bàn
	SELECT @countBill = COUNT(*) FROM Bill WHERE TableID = @tableID AND BillStatus = 0

	IF (@countBill = 0)
		--Bàn mà không có Bill nào là chưa thanh toán => 'Trống'
		UPDATE TableFood SET TableStatus = N'Trống' WHERE TableFood.ID = @tableID
	ELSE
		UPDATE TableFood SET TableStatus = N'Có người' WHERE TableFood.ID = @tableID
	----------------------------------------------------------------------------------------
	
	DECLARE @oldTableID INT
	--Lấy ra TableID cũ sau khi UPDATE (khi chuyển bàn)
	SELECT @oldTableID = TableID FROM deleted

	--Dùng lại biến @countBill
	SET @countBill = 0
	--Đếm số lượng Bill chưa được thanh toán trên bàn
	SELECT @countBill = COUNT(*) FROM Bill WHERE TableID = @oldTableID AND BillStatus = 0

	IF (@countBill = 0)
		--Bàn mà không có Bill nào là chưa thanh toán => 'Trống'
		UPDATE TableFood SET TableStatus = N'Trống' WHERE TableFood.ID = @oldTableID
		
END
GO
ALTER TABLE [dbo].[Bill] ENABLE TRIGGER [UTG_UpdateBill]
GO
/****** Object:  Trigger [dbo].[UTG_DeleteBillInfo]    Script Date: 10/13/2022 3:31:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Trigger xoá Bill và chuyển trạng thái của bàn sang 'Trống' khi người dùng trừ hết sạch các món ăn trên bàn
CREATE TRIGGER [dbo].[UTG_DeleteBillInfo]
ON [dbo].[BillInfo]
FOR DELETE
AS
BEGIN
	--Lấy ra BillID của BillInfo vừa bị xoá
	DECLARE @billID INT
	SELECT @billID = BillID FROM deleted

	--Đếm số lượng BillInfo (số lượng món ăn) còn lại của Bill đó, nếu = 0 => xoá Bill và chuyển trạng thái của bàn sang 'Trống'
	DECLARE @countBillInfo INT = 0
	SELECT @countBillInfo = COUNT(BillID) FROM BillInfo WHERE BillID = @billID

	IF (@countBillInfo = 0)
		BEGIN
			DECLARE @tableID INT
			SELECT @tableID = TableID FROM Bill WHERE ID = @billID AND BillStatus = 0
			DELETE Bill WHERE ID = @billID
			UPDATE TableFood SET TableStatus = N'Trống' WHERE TableFood.ID = @tableID
		END
END
GO
ALTER TABLE [dbo].[BillInfo] ENABLE TRIGGER [UTG_DeleteBillInfo]
GO
/****** Object:  Trigger [dbo].[UTG_InsertBillInfo]    Script Date: 10/13/2022 3:31:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Trigger chuyển trạng thái của bàn sang 'Có người' mỗi khi nó được insert thêm BillInfo (thêm món)
CREATE TRIGGER [dbo].[UTG_InsertBillInfo]
ON [dbo].[BillInfo]
FOR INSERT
AS
BEGIN
	DECLARE @billID INT
	--Lấy ra BillID của BillInfo vừa được thêm
	SELECT @billID = BillID FROM inserted
	
	DECLARE @tableID INT
	--Lấy ra TableID của Bill có BillInfo vừa được thêm (phải là Bill chưa thanh toán)
	SELECT @tableID = TableID FROM Bill WHERE ID = @billID AND BillStatus = 0

	--Chuyển trạng thái của bàn mà Bill của nó vừa được thêm BillInfo
	UPDATE TableFood SET TableStatus = N'Có người' WHERE TableFood.ID = @tableID

END
GO
ALTER TABLE [dbo].[BillInfo] ENABLE TRIGGER [UTG_InsertBillInfo]
GO
/****** Object:  Trigger [dbo].[UTG_UpdateBillInfo]    Script Date: 10/13/2022 3:31:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Trigger xoá Bill và chuyển trạng thái của bàn thứ 1 sang 'Trống' khi gộp bàn thứ 1 vào bàn thứ 2
CREATE TRIGGER [dbo].[UTG_UpdateBillInfo]
ON [dbo].[BillInfo]
FOR UPDATE
AS
BEGIN
	--Lấy ra BillID cũ của BillInfo vừa update
	DECLARE @billID INT
	SELECT @billID = BillID FROM deleted

	--Đếm số lượng BillInfo (số lượng món ăn) còn lại của Bill đó, nếu = 0 => xoá Bill và chuyển trạng thái của bàn sang 'Trống'
	DECLARE @countBillInfo INT = 0
	SELECT @countBillInfo = COUNT(BillID) FROM BillInfo WHERE BillID = @billID

	IF (@countBillInfo = 0)
		BEGIN
			DECLARE @tableID INT
			SELECT @tableID = TableID FROM Bill WHERE ID = @billID AND BillStatus = 0
			DELETE Bill WHERE ID = @billID
			UPDATE TableFood SET TableStatus = N'Trống' WHERE TableFood.ID = @tableID
		END
END
GO
ALTER TABLE [dbo].[BillInfo] ENABLE TRIGGER [UTG_UpdateBillInfo]
GO
USE [master]
GO
ALTER DATABASE [CanteenManager] SET  READ_WRITE 
GO
