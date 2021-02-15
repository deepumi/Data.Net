GO
/****** Object:  Table [dbo].[Users_Test]    Script Date: 09/14/2017 13:01:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users_Test](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](120) NOT NULL,
	[LastName] [nvarchar](120) NOT NULL,
	[CreateDate] [datetime] NULL,
	[Email] [nvarchar](256) NULL,
 CONSTRAINT [PK_Users_UserId] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRoles_Test]    Script Date: 09/14/2017 13:01:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoles_Test](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Role] [nvarchar](15) NULL,
 CONSTRAINT [PK_UserRoles_RoleId] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[GetUsersWithReturn_TEST]    Script Date: 09/14/2017 13:01:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[GetUsersWithReturn_TEST]
@Email varchar(100)
AS

BEGIN

DECLARE @ReturnVal INT

SELECT @ReturnVal = COUNT(*) FROM [Users_Test]

SELECT * FROM [USERS_TEST] Where email = @Email

RETURN @ReturnVal

END
GO
/****** Object:  StoredProcedure [dbo].[GetUsers_With_Output_Return_TEST]    Script Date: 09/14/2017 13:01:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[GetUsers_With_Output_Return_TEST] 
@Email varchar(100),
@RecordCount int out,
@AnotherOutParameter int out
AS

BEGIN

Declare @ReturnValue int

SELECT @RecordCount = COUNT(*) FROM [Users_Test]

SELECT * FROM [USERS_TEST] Where email = @Email

SET @AnotherOutParameter = 100

SET @ReturnValue = @RecordCount

RETURN @ReturnValue

END
GO
/****** Object:  StoredProcedure [dbo].[GetUsers_TEST]    Script Date: 09/14/2017 13:01:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[GetUsers_TEST] 
@Email varchar(100),
@RecordCount int out,
@AnotherOutParameter int out
AS

BEGIN

SELECT @RecordCount = COUNT(*) FROM [Users_Test]

SELECT * FROM [Users_Test] Where email = @Email

SET @AnotherOutParameter = 100

END
GO
/****** Object:  ForeignKey [FK_Users_UserId]    Script Date: 09/14/2017 13:01:14 ******/
ALTER TABLE [dbo].[UserRoles_Test]  WITH CHECK ADD  CONSTRAINT [FK_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users_Test] ([UserId])
GO
ALTER TABLE [dbo].[UserRoles_Test] CHECK CONSTRAINT [FK_Users_UserId]
GO


CREATE TABLE [dbo].[Address](
	[Name] [nvarchar](50) NOT NULL,
	[Address] [nvarchar](50) NULL,
	[createdate] [datetime] NULL,
	[Active] [int] NULL,
	[AddressGuid] [uniqueidentifier] NULL
) ON [PRIMARY]
GO

INSERT INTO Address VALUES('Deepu Madhusoodanan', 'Add',getdate(),1,'08CEE763-675F-4234-A5E8-2A12122BC261')

