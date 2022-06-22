USE [��� ��]
GO
/****** Object:  User [fik]    Script Date: 14.06.2022 9:39:37 ******/
CREATE USER [fik] FOR LOGIN [COLLEGE\fik] WITH DEFAULT_SCHEMA=[db_owner]
GO
ALTER ROLE [db_owner] ADD MEMBER [fik]
GO
ALTER ROLE [db_datareader] ADD MEMBER [fik]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [fik]
GO
/****** Object:  UserDefinedFunction [dbo].[GetMarkCount]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create FUNCTION [dbo].[GetMarkCount](
@idAbiturient int,
@Mark int)
Returns int
AS
BEGIN
	DECLARE @MarkCount int;
	SELECT @MarkCount = 
	(SELECT SUM(����������)
                               FROM            �������������� INNER JOIN
                                                         ������� ON �������.ID�������� = ��������������.ID��������
                               WHERE        (@idAbiturient = ID�����������) AND (���� = @Mark)
                               GROUP BY ����)
	RETURN @MarkCount
END
GO
/****** Object:  UserDefinedFunction [dbo].[GetNum]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[GetNum](@st varchar(10))
RETURNS int
begin
	return
    left(
        right(@st, 1 + len(@st) - patindex('%[0-9.]%', @st)), 
        patindex('%[^0-9.]%', right(@st, 1 + len(@st) - patindex('%[0-9.]%', @st))) - 1)
end
GO
/****** Object:  Table [dbo].[�������]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[�������](
	[ID��������] [int] IDENTITY(1,1) NOT NULL,
	[ID�����������] [int] NOT NULL,
	[ID�����������] [int] NOT NULL,
	[�������������] [nvarchar](50) NULL,
	[�����������] [float] NOT NULL,
	[��������������������] [float] NOT NULL,
 CONSTRAINT [PK__�������__7B32B3EB2167DC4B] PRIMARY KEY CLUSTERED 
(
	[ID��������] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[�����]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[�����](
	[ID�����] [int] IDENTITY(1,1) NOT NULL,
	[������������] [nvarchar](25) NOT NULL,
	[����������������] [int] NOT NULL,
 CONSTRAINT [PK__�����__A0AAEA358404A57D] PRIMARY KEY CLUSTERED 
(
	[ID�����] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[�����������]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[�����������](
	[ID�����������] [int] IDENTITY(1,1) NOT NULL,
	[ID�����] [int] NOT NULL,
	[ID��������������] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID�����������] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[����������]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[����������](
	[ID�����������] [int] IDENTITY(1,1) NOT NULL,
	[�������] [nvarchar](20) NOT NULL,
	[���] [nvarchar](20) NOT NULL,
	[��������] [nvarchar](20) NULL,
	[�����] [nvarchar](50) NOT NULL,
	[�������������������] [nvarchar](10) NULL,
	[�����������������] [int] NOT NULL,
	[�����������] [bit] NULL,
	[�����������] [nvarchar](30) NULL,
	[���������] [bit] NULL,
	[��������������] [int] NOT NULL,
	[ID�����������] [int] NOT NULL,
	[�����������] [nvarchar](60) NULL,
	[���������] [nvarchar](40) NULL,
	[������] [bit] NULL,
	[��������������] [bit] NULL,
	[������������������] [bit] NULL,
	[ID���������] [int] NOT NULL,
	[ID���������] [int] NULL,
	[�������] [bit] NULL,
	[������������] [datetime] NULL,
	[������������������] [datetime] NULL,
 CONSTRAINT [PK__��������__998FBBCDBBD36977] PRIMARY KEY CLUSTERED 
(
	[ID�����������] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[GetAbiturientData]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*SELECT * FROM GetAbiturientData*/
CREATE VIEW [dbo].[GetAbiturientData]
AS
SELECT        dbo.����������.ID�����������, dbo.����������.������� + ' ' + dbo.����������.��� + ' ' + dbo.����������.�������� AS ���, dbo.����������.�����, dbo.����������.�����������������, 
                         dbo.����������.������������������, dbo.GetMarkCount(dbo.����������.ID�����������, 1) AS Mark1, dbo.GetMarkCount(dbo.����������.ID�����������, 2) AS Mark2, dbo.GetMarkCount(dbo.����������.ID�����������, 3) 
                         AS Mark3, dbo.GetMarkCount(dbo.����������.ID�����������, 4) AS Mark4, dbo.GetMarkCount(dbo.����������.ID�����������, 5) AS Mark5, dbo.GetMarkCount(dbo.����������.ID�����������, 6) AS Mark6, 
                         dbo.GetMarkCount(dbo.����������.ID�����������, 7) AS Mark7, dbo.GetMarkCount(dbo.����������.ID�����������, 8) AS Mark8, dbo.GetMarkCount(dbo.����������.ID�����������, 9) AS Mark9, 
                         dbo.GetMarkCount(dbo.����������.ID�����������, 10) AS Mark10, dbo.GetMarkCount(dbo.����������.ID�����������, 11) AS Mark11, dbo.GetMarkCount(dbo.����������.ID�����������, 12) AS Mark12, 
                         dbo.GetMarkCount(dbo.����������.ID�����������, 13) AS Mark13, dbo.GetMarkCount(dbo.����������.ID�����������, 14) AS Mark14, dbo.GetMarkCount(dbo.����������.ID�����������, 15) AS Mark15, 
                         ROUND(SUM(dbo.�������.�����������) / COUNT(*), 2) AS �����������, dbo.����������.�������������������, dbo.����������.�������, ROUND(SUM(dbo.�������.��������������������) / COUNT(*), 2) 
                         AS ��������������������������������, dbo.����������.������, dbo.����������.��������������,
                             (SELECT        ����������������
                               FROM            dbo.�����
                               WHERE        (ID����� =
                                                             (SELECT        ID�����
                                                               FROM            dbo.�����������
                                                               WHERE        (ID����������� = dbo.�������.ID�����������)))) AS [������ �����]
FROM            dbo.���������� LEFT OUTER JOIN
                         dbo.������� ON dbo.����������.ID����������� = dbo.�������.ID�����������
GROUP BY dbo.����������.ID�����������, dbo.����������.�������, dbo.����������.���, dbo.����������.��������, dbo.����������.�����, dbo.����������.�����������������, dbo.����������.������������������, 
                         dbo.����������.�������������������, dbo.����������.�������, dbo.����������.������, dbo.����������.��������������, dbo.�������.ID�����������
GO
/****** Object:  Table [dbo].[������]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[������](
	[ID������] [int] IDENTITY(1,1) NOT NULL,
	[������������] [nvarchar](20) NOT NULL,
	[���������] [int] NOT NULL,
	[������������������] [nvarchar](50) NOT NULL,
	[����������] [nvarchar](40) NULL,
 CONSTRAINT [PK__������__039CE28169CFC07B] PRIMARY KEY CLUSTERED 
(
	[ID������] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[�����������������]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[�����������������](
	[ID�����������������] [int] IDENTITY(1,1) NOT NULL,
	[ID�����������] [int] NOT NULL,
	[ID������] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID�����������������] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[AbiturientPriority]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[AbiturientPriority](@noteCount INT)
RETURNS TABLE
AS
RETURN
	SELECT TOP (@noteCount) �������, ���, ��������, ������, ��������������,
		(SELECT CASE WHEN MIN(���������) IS NULL THEN 10 ELSE MIN(���������) END 
		FROM ����������������� JOIN ������ ON (�����������������.ID������ = ������.ID������) 
		WHERE ����������.ID����������� = �����������������.ID�����������) AS ��������������������,
		(SELECT ROUND(MAX(��������������������),2) FROM ������� WHERE �������.ID����������� = ����������.ID�����������) AS [������� ����]
	FROM ����������
	ORDER BY ������ DESC, ��������������������, [������� ����] DESC
GO
/****** Object:  Table [dbo].[����������������]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[����������������](
	[ID����������������] [int] IDENTITY(1,1) NOT NULL,
	[ID�����������] [int] NOT NULL,
	[��������] [nvarchar](70) NOT NULL,
	[ID�����������] [int] NOT NULL,
 CONSTRAINT [PK__��������__985028F09B245D35] PRIMARY KEY CLUSTERED 
(
	[ID����������������] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[���������]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[���������](
	[ID���������] [int] IDENTITY(1,1) NOT NULL,
	[��������������] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID���������] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[��������������]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[��������������](
	[ID��������������] [int] IDENTITY(1,1) NOT NULL,
	[����] [int] NOT NULL,
	[����������] [int] NOT NULL,
	[ID��������] [int] NOT NULL,
 CONSTRAINT [PK__��������__DFFC47AD44A93D7B] PRIMARY KEY CLUSTERED 
(
	[ID��������������] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[����������������]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[����������������](
	[ID����������������] [int] IDENTITY(1,1) NOT NULL,
	[ID�����������] [int] NOT NULL,
	[������������] [date] NOT NULL,
	[�����] [nvarchar](15) NULL,
	[�������������] [nvarchar](15) NULL,
	[����������] [date] NULL,
	[������������������] [nvarchar](50) NULL,
	[����������������������] [nvarchar](30) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID����������������] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[�������]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[�������](
	[ID��������] [int] IDENTITY(1,1) NOT NULL,
	[ID�����] [int] NOT NULL,
	[��������] [float] NOT NULL,
	[���������������������] [float] NOT NULL,
 CONSTRAINT [PK__�������__E0371C5E79140FCB] PRIMARY KEY CLUSTERED 
(
	[ID��������] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[����������]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[����������](
	[ID����������] [int] IDENTITY(1,1) NOT NULL,
	[��������������] [varchar](4) NOT NULL,
	[ID�������������] [int] NOT NULL,
	[ID�������������] [int] NOT NULL,
	[ID��������������] [int] NOT NULL,
	[����������] [int] NOT NULL,
	[���������������������] [int] NOT NULL,
	[��] [bit] NOT NULL,
 CONSTRAINT [PK__��������__2595E66AC64EC603] PRIMARY KEY CLUSTERED 
(
	[ID����������] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[������������]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[������������](
	[ID������������] [int] IDENTITY(1,1) NOT NULL,
	[�����] [nvarchar](10) NOT NULL,
	[���] [nvarchar](50) NOT NULL,
	[ID����] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID������������] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[����]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[����](
	[ID����] [int] IDENTITY(1,1) NOT NULL,
	[������������] [nvarchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID����] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[������������]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[������������](
	[ID�������������] [int] IDENTITY(1,1) NOT NULL,
	[ID�����������] [int] NOT NULL,
	[����������] [nvarchar](20) NULL,
	[����] [int] NOT NULL,
	[���������������������] [float] NOT NULL,
	[��������������] [int] NULL,
	[����������] [nvarchar](50) NULL,
 CONSTRAINT [PK__��������__CD497088FCC4CBC1] PRIMARY KEY CLUSTERED 
(
	[ID�������������] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[�������������]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[�������������](
	[ID�������������] [int] IDENTITY(1,1) NOT NULL,
	[������������] [nvarchar](50) NOT NULL,
	[�����] [char](1) NOT NULL,
	[�������������������] [nvarchar](20) NULL,
	[���] [nvarchar](20) NULL,
 CONSTRAINT [PK__��������__67713123C471BC0F] PRIMARY KEY CLUSTERED 
(
	[ID�������������] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[��������������]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[��������������](
	[ID��������������] [int] IDENTITY(1,1) NOT NULL,
	[������������] [nvarchar](40) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID��������������] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[�����������]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[�����������](
	[ID�����������] [int] IDENTITY(1,1) NOT NULL,
	[������������] [nvarchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID�����������] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[��������������]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[��������������](
	[ID��������������] [int] IDENTITY(1,1) NOT NULL,
	[������������] [nvarchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID��������������] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[�������������]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[�������������](
	[ID�������������] [int] IDENTITY(1,1) NOT NULL,
	[������������] [nvarchar](20) NOT NULL,
	[�����������] [nvarchar](40) NOT NULL,
 CONSTRAINT [PK__��������__7BE297F5ABAB32BF] PRIMARY KEY CLUSTERED 
(
	[ID�������������] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[����������] ADD  CONSTRAINT [DF_����������_������������������]  DEFAULT ((0)) FOR [������������������]
GO
ALTER TABLE [dbo].[����������] ADD  CONSTRAINT [DF_����������_�������]  DEFAULT ((0)) FOR [�������]
GO
ALTER TABLE [dbo].[����������] ADD  CONSTRAINT [DF_����������_������������]  DEFAULT (getdate()) FOR [������������]
GO
ALTER TABLE [dbo].[����������]  WITH CHECK ADD  CONSTRAINT [FK__���������__ID���__05D8E0BE] FOREIGN KEY([ID���������])
REFERENCES [dbo].[������������] ([ID������������])
GO
ALTER TABLE [dbo].[����������] CHECK CONSTRAINT [FK__���������__ID���__05D8E0BE]
GO
ALTER TABLE [dbo].[����������]  WITH CHECK ADD  CONSTRAINT [FK__���������__ID���__04E4BC85] FOREIGN KEY([ID�����������])
REFERENCES [dbo].[����������] ([ID����������])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[����������] CHECK CONSTRAINT [FK__���������__ID���__04E4BC85]
GO
ALTER TABLE [dbo].[����������]  WITH CHECK ADD  CONSTRAINT [FK__���������__ID���__06CD04F7] FOREIGN KEY([ID���������])
REFERENCES [dbo].[������������] ([ID������������])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[����������] CHECK CONSTRAINT [FK__���������__ID���__06CD04F7]
GO
ALTER TABLE [dbo].[�������]  WITH CHECK ADD  CONSTRAINT [FK__�������__ID�����__65370702] FOREIGN KEY([ID�����������])
REFERENCES [dbo].[����������] ([ID�����������])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[�������] CHECK CONSTRAINT [FK__�������__ID�����__65370702]
GO
ALTER TABLE [dbo].[�������]  WITH CHECK ADD  CONSTRAINT [FK__�������__ID�����__662B2B3B] FOREIGN KEY([ID�����������])
REFERENCES [dbo].[�����������] ([ID�����������])
GO
ALTER TABLE [dbo].[�������] CHECK CONSTRAINT [FK__�������__ID�����__662B2B3B]
GO
ALTER TABLE [dbo].[����������������]  WITH CHECK ADD  CONSTRAINT [FK__���������__ID���__56E8E7AB] FOREIGN KEY([ID�����������])
REFERENCES [dbo].[����������] ([ID�����������])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[����������������] CHECK CONSTRAINT [FK__���������__ID���__56E8E7AB]
GO
ALTER TABLE [dbo].[����������������]  WITH CHECK ADD  CONSTRAINT [FK__���������__ID���__57DD0BE4] FOREIGN KEY([ID�����������])
REFERENCES [dbo].[�����������] ([ID�����������])
GO
ALTER TABLE [dbo].[����������������] CHECK CONSTRAINT [FK__���������__ID���__57DD0BE4]
GO
ALTER TABLE [dbo].[��������������]  WITH CHECK ADD  CONSTRAINT [FK__���������__ID���__690797E6] FOREIGN KEY([ID��������])
REFERENCES [dbo].[�������] ([ID��������])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[��������������] CHECK CONSTRAINT [FK__���������__ID���__690797E6]
GO
ALTER TABLE [dbo].[����������������]  WITH CHECK ADD  CONSTRAINT [FK__���������__ID���__5224328E] FOREIGN KEY([ID�����������])
REFERENCES [dbo].[����������] ([ID�����������])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[����������������] CHECK CONSTRAINT [FK__���������__ID���__5224328E]
GO
ALTER TABLE [dbo].[�������]  WITH CHECK ADD  CONSTRAINT [FK__�������__ID�����__5CA1C101] FOREIGN KEY([ID�����])
REFERENCES [dbo].[�����] ([ID�����])
GO
ALTER TABLE [dbo].[�������] CHECK CONSTRAINT [FK__�������__ID�����__5CA1C101]
GO
ALTER TABLE [dbo].[����������]  WITH CHECK ADD  CONSTRAINT [FK__���������__ID���__787EE5A0] FOREIGN KEY([ID�������������])
REFERENCES [dbo].[�������������] ([ID�������������])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[����������] CHECK CONSTRAINT [FK__���������__ID���__787EE5A0]
GO
ALTER TABLE [dbo].[����������]  WITH CHECK ADD  CONSTRAINT [FK__���������__ID���__7A672E12] FOREIGN KEY([ID��������������])
REFERENCES [dbo].[��������������] ([ID��������������])
GO
ALTER TABLE [dbo].[����������] CHECK CONSTRAINT [FK__���������__ID���__7A672E12]
GO
ALTER TABLE [dbo].[����������]  WITH CHECK ADD  CONSTRAINT [FK__���������__ID���__797309D9] FOREIGN KEY([ID�������������])
REFERENCES [dbo].[�������������] ([ID�������������])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[����������] CHECK CONSTRAINT [FK__���������__ID���__797309D9]
GO
ALTER TABLE [dbo].[������������]  WITH CHECK ADD  CONSTRAINT [FK__���������__ID���__02084FDA] FOREIGN KEY([ID����])
REFERENCES [dbo].[����] ([ID����])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[������������] CHECK CONSTRAINT [FK__���������__ID���__02084FDA]
GO
ALTER TABLE [dbo].[������������]  WITH CHECK ADD  CONSTRAINT [FK__���������__ID���__4F47C5E3] FOREIGN KEY([ID�����������])
REFERENCES [dbo].[����������] ([ID�����������])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[������������] CHECK CONSTRAINT [FK__���������__ID���__4F47C5E3]
GO
ALTER TABLE [dbo].[�����������������]  WITH CHECK ADD  CONSTRAINT [FK__���������__ID���__1BC821DD] FOREIGN KEY([ID�����������])
REFERENCES [dbo].[����������] ([ID�����������])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[�����������������] CHECK CONSTRAINT [FK__���������__ID���__1BC821DD]
GO
ALTER TABLE [dbo].[�����������������]  WITH CHECK ADD  CONSTRAINT [FK__���������__ID���__1CBC4616] FOREIGN KEY([ID������])
REFERENCES [dbo].[������] ([ID������])
GO
ALTER TABLE [dbo].[�����������������] CHECK CONSTRAINT [FK__���������__ID���__1CBC4616]
GO
ALTER TABLE [dbo].[�����������]  WITH CHECK ADD FOREIGN KEY([ID��������������])
REFERENCES [dbo].[��������������] ([ID��������������])
GO
ALTER TABLE [dbo].[�����������]  WITH CHECK ADD  CONSTRAINT [FK__���������__ID���__6166761E] FOREIGN KEY([ID�����])
REFERENCES [dbo].[�����] ([ID�����])
GO
ALTER TABLE [dbo].[�����������] CHECK CONSTRAINT [FK__���������__ID���__6166761E]
GO
/****** Object:  StoredProcedure [dbo].[AbiturientsPriority]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[AbiturientsPriority]
@ID INT
AS
BEGIN
	SELECT ID�����������, �������, ���, ��������, ������, ��������������,
		(SELECT CASE WHEN MIN(���������) IS NULL THEN 10 ELSE MIN(���������) END 
		FROM ����������������� JOIN ������ ON (�����������������.ID������ = ������.ID������) 
		WHERE ����������.ID����������� = �����������������.ID�����������) AS ��������������������,
		(SELECT ROUND(MAX(��������������������),2) FROM ������� WHERE �������.ID����������� = ����������.ID�����������) AS [������� ����], �������
	FROM ����������
	WHERE ID����������� = @ID
	ORDER BY �������, ������ DESC, �������������� DESC, [������� ����] DESC, ��������������������
END
GO
/****** Object:  StoredProcedure [dbo].[Add_Abiturient]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Add_Abiturient]
@surename varchar(50),
@name varchar(50),
@otchestvo varchar(50),
@shool varchar(50),
@graduationYear int,
@grajdanstvoRB bit,
@grajdanstvo varchar(50),
@obshejitie bit,
@planPriema int,
@workPlace varchar(50),
@doljnost varchar(50),
@sirota bit,
@dogovor bit,
@user int,
@ExamList varchar(10)
AS
BEGIN
	INSERT INTO ����������(�������, ���, ��������, �����, �����������������, �����������, �����������, 
						   ���������, ��������������, ID�����������, �����������, ���������, ������, 
						   ��������������, ������������������, ID���������, �������, ������������, �������������������) 
	OUTPUT inserted.ID����������� 
	VALUES(@surename, @name, @otchestvo, @shool, @graduationYear, @grajdanstvoRB, @grajdanstvo, @obshejitie, YEAR(GETDATE()), @planPriema, @workPlace, @doljnost, @sirota, @dogovor, 0, @user, 0,  GETDATE(), @ExamList)
END
GO
/****** Object:  StoredProcedure [dbo].[Add_Atestat]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Add_Atestat]
@abiturient int,
@scaleName nvarchar(50),
@attestatSeries nvarchar(50),
@avgMarks float
AS
BEGIN
IF (SELECT ��������������������� FROM ������� WHERE ID����� = (SELECT ID����� FROM ����� WHERE ������������ = @scaleName) AND �������� = ROUND(@avgMarks,1)) IS NOT NULL
	INSERT INTO ������� (ID�����������, ID�����������, �������������, �����������, ��������������������) 
		OUTPUT inserted.ID�������� 
		VALUES(@abiturient,(SELECT ID����������� FROM ����������� JOIN ����� ON (�����������.ID����� = �����.ID�����) WHERE ������������ LIKE @scaleName), @attestatSeries,
		ROUND(@avgMarks,2),(SELECT ��������������������� FROM ������� WHERE ID����� = (SELECT ID����� FROM ����� WHERE ������������ = @scaleName) AND �������� = ROUND(@avgMarks,1)))
ELSE
	INSERT INTO ������� (ID�����������, ID�����������, �������������, �����������, ��������������������) 
		OUTPUT inserted.ID�������� 
		VALUES(@abiturient,(SELECT ID����������� FROM ����������� JOIN ����� ON (�����������.ID����� = �����.ID�����) WHERE ������������ LIKE @scaleName), @attestatSeries,
		ROUND(@avgMarks,2),@avgMarks)
END
GO
/****** Object:  StoredProcedure [dbo].[Add_ContctData]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Add_ContctData]
@abiturient int,
@svedeniya nvarchar(50),
@contactType nvarchar(50)
AS
BEGIN
INSERT INTO ����������������(ID�����������, ��������, ID�����������) 
	VALUES(@abiturient,@svedeniya,
			(SELECT ID����������� FROM ����������� WHERE ������������ = @contactType))
END
GO
/****** Object:  StoredProcedure [dbo].[Add_Mark]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Add_Mark]
@attestat int,
@mark int,
@colvo int
AS
BEGIN
	INSERT INTO �������������� (����, ����������, ID��������) VALUES (@mark,@colvo,@attestat)
END
GO
/****** Object:  StoredProcedure [dbo].[Add_PassportData]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Add_PassportData]
@abiturient int,
@dateIssue date,
@dateOfBirth date,
@series varchar(2),
@PasspornNum int,
@name nvarchar(60),
@identNum nvarchar(30)
AS
BEGIN
	INSERT INTO ���������������� (ID�����������,����������,������������,�����,�������������,������������������,����������������������) 
	VALUES(@abiturient,@dateIssue,@dateOfBirth,@series,@PasspornNum,@name,@identNum)
END
GO
/****** Object:  StoredProcedure [dbo].[Add_PlanPriema]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Add_PlanPriema]
@year varchar(4),
@spec varchar(50),
@form varchar(50),
@fin varchar(50),
@obr varchar(50),
@kolva int,
@kolvaCel int,
@CT bit
AS
BEGIN
	INSERT INTO ����������(��������������, ID�������������, ID�������������, ID��������������, ����������, ���������������������, ��) VALUES(
		@year,
		(SELECT ID������������� FROM ������������� WHERE ������������ = @spec),
		(SELECT ID������������� FROM ������������� WHERE ������������ = @form AND ����������� = @obr),
		(SELECT ID�������������� FROM �������������� WHERE ������������ = @fin),
		@kolva,
		@kolvaCel,
		@CT
	)
END
GO
/****** Object:  StoredProcedure [dbo].[Add_Sertificat]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Add_Sertificat]
@sertificat int,
@disciplin nvarchar(50),
@mark int,
@decMark float,
@year int,
@serialNum nvarchar(20)
AS
BEGIN
	INSERT INTO ������������ (ID�����������, ����������, ����, ���������������������, ��������������, ����������) 
	VALUES(@sertificat,@disciplin,@mark,@decMark,@year,@serialNum)
END
GO
/****** Object:  StoredProcedure [dbo].[Add_Stati]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Add_Stati]
@abiturient int,
@statya int
AS
BEGIN
	INSERT INTO �����������������(ID�����������,ID������) VALUES(@abiturient,@statya)
END
GO
/****** Object:  StoredProcedure [dbo].[Add_User]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Add_User]
@login nvarchar(30),
@fio nvarchar(70),
@role nvarchar(50)
AS
BEGIN
INSERT INTO ������������(�����, ���, ID����) 
		OUTPUT inserted.ID������������
		VALUES (@login, @fio, (SElECT ID���� FROM ���� WHERE ������������ = @role))
END
GO
/****** Object:  StoredProcedure [dbo].[Del_AbiturientMarks]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Del_AbiturientMarks]
@abiturient int
AS
BEGIN
	DECLARE @lastID int = 0;
	select @lastID = MAX(ID��������������) FROM ��������������;
	DELETE �������������� WHERE (ID�������� = ALL(SELECT ID�������� FROM ������� WHERE ID����������� = @abiturient))
	UPDATE ���������� SET ������������������ = 0,
						  ������� = 1
					WHERE ID����������� = @abiturient;
	DBCC CHECKIDENT (��������������, RESEED, @lastID);
END
GO
/****** Object:  StoredProcedure [dbo].[Get_AbiturientaAttestat]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_AbiturientaAttestat]
@abiturient int
AS
BEGIN
	SELECT ID��������, ������������� as Num,
	(SELECT ���������� FROM �������������� WHERE ��������������.ID�������� = �������.ID�������� AND ���� = 1) as [n1],
	(SELECT ���������� FROM �������������� WHERE ��������������.ID�������� = �������.ID�������� AND ���� = 2) as [n2],
	(SELECT ���������� FROM �������������� WHERE ��������������.ID�������� = �������.ID�������� AND ���� = 3) as [n3],
	(SELECT ���������� FROM �������������� WHERE ��������������.ID�������� = �������.ID�������� AND ���� = 4) as [n4],
	(SELECT ���������� FROM �������������� WHERE ��������������.ID�������� = �������.ID�������� AND ���� = 5) as [n5],
	(SELECT ���������� FROM �������������� WHERE ��������������.ID�������� = �������.ID�������� AND ���� = 6) as [n6],
	(SELECT ���������� FROM �������������� WHERE ��������������.ID�������� = �������.ID�������� AND ���� = 7) as [n7],
	(SELECT ���������� FROM �������������� WHERE ��������������.ID�������� = �������.ID�������� AND ���� = 8) as [n8],
	(SELECT ���������� FROM �������������� WHERE ��������������.ID�������� = �������.ID�������� AND ���� = 9) as [n9],
	(SELECT ���������� FROM �������������� WHERE ��������������.ID�������� = �������.ID�������� AND ���� = 10) as [n10],
	(SELECT ���������� FROM �������������� WHERE ��������������.ID�������� = �������.ID�������� AND ���� = 11) as [n11],
	(SELECT ���������� FROM �������������� WHERE ��������������.ID�������� = �������.ID�������� AND ���� = 12) as [n12],
	(SELECT ���������� FROM �������������� WHERE ��������������.ID�������� = �������.ID�������� AND ���� = 13) as [n13],
	(SELECT ���������� FROM �������������� WHERE ��������������.ID�������� = �������.ID�������� AND ���� = 14) as [n14],
	(SELECT ���������� FROM �������������� WHERE ��������������.ID�������� = �������.ID�������� AND ���� = 15) as [n15],
	(SELECT ������������ FROM ����� WHERE �����.ID����� = (SELECT �����������.ID����� FROM ����������� WHERE �������.ID����������� = ID�����������)) as [Scale], 
	ROUND (�����������, 2, 1) as �����������,ROUND (��������������������, 2, 1) as �������������������� FROM ������� WHERE ID����������� = @abiturient
END
GO
/****** Object:  StoredProcedure [dbo].[Get_AbiturientaFullInfo]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_AbiturientaFullInfo]
@abiturient int
AS
BEGIN
	SELECT ������� + ' ' + ��� + ' ' + ��������, �����, �����������������, ������������, ����������, �����, �������������,������������������,����������������������, �����������, �����������, ���������, (SELECT ��� FROM ������������ WHERE ID������������ = ID���������), (SELECT ��� FROM ������������ WHERE ID������������ = ID���������), ������������, ������������������, ������������������, �������
	FROM ���������� LEFT JOIN ���������������� ON (����������.ID����������� = ����������������.ID�����������) 
	WHERE ����������.ID����������� = @abiturient
END
GO
/****** Object:  StoredProcedure [dbo].[Get_AbiturientaKontakti]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_AbiturientaKontakti]
@abiturient int
AS
BEGIN
	SELECT ID����������������, ROW_NUMbER() OVER(ORDER BY ID����������������) as Num, (SELECT ������������ FROM ����������� WHERE ����������������.ID����������� = �����������.ID�����������) as [�����������], �������� 
	FROM  ���������������� 
	WHERE ID����������� = @abiturient
END
GO
/****** Object:  StoredProcedure [dbo].[Get_AbiturientaSertificati]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_AbiturientaSertificati]
@abiturient int
AS
BEGIN
	SELECT ID�������������, ���������� as num, ����������, ��������������, ����, ��������������������� 
	FROM ������������ 
	WHERE ID����������� = @abiturient
END
GO
/****** Object:  StoredProcedure [dbo].[Get_AbiturientList]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_AbiturientList]
@PlanPriema int
AS
BEGIN
SELECT ����������.ID�����������,  ����������.������� + ' ' + ����������.��� +' '+����������.�������� AS ���,
����������.������, ����������.��������������, ������������.��� AS ��������, ������������, ������������������, ������� 
	FROM ���������� LEFT JOIN ������������ ON(����������.ID��������� = ������������.ID������������) 
	WHERE ����������.ID����������� = @PlanPriema
END
GO
/****** Object:  StoredProcedure [dbo].[Get_AbiturientMainInfo]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_AbiturientMainInfo]
@abiturient int
AS
BEGIN
	SELECT �������, ���, ��������, �����, �����������������, ������������, ����������, �����, �������������,������������������,����������������������, �����������, �����������, ���������, ���������, ������, ��������������, ������������������� 
	FROM ���������� LEFT JOIN ���������������� ON (����������.ID����������� = ����������������.ID�����������) 
	WHERE ����������.ID����������� = @abiturient
END
GO
/****** Object:  StoredProcedure [dbo].[Get_AbiturientPriority]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_AbiturientPriority]
AS
BEGIN
	SELECT �������, ���, ��������, ������, ��������������,
		(SELECT CASE WHEN MIN(���������) IS NULL THEN 10 ELSE MIN(���������) END 
		FROM ����������������� JOIN ������ ON (�����������������.ID������ = ������.ID������) 
		WHERE ����������.ID����������� = �����������������.ID�����������) AS ��������������������,
		(SELECT ROUND(MAX(��������������������),2) FROM ������� WHERE �������.ID����������� = ����������.ID�����������) AS [������� ����]
	FROM ����������
	ORDER BY ��������������������, [������� ����] DESC
END
GO
/****** Object:  StoredProcedure [dbo].[Get_MarkConvert]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_MarkConvert]
@scaleName varchar(50),
@mark int
AS
BEGIN
	SELECT ��������������������� FROM ������� WHERE ID����� = (SELECT ID����� FROM ����� WHERE ������������ = @scaleName) AND �������� = @mark
END
GO
/****** Object:  StoredProcedure [dbo].[Get_PlaniPriema]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_PlaniPriema]
@specialost varchar(50), 
@budjet varchar(50), @hozrash varchar(50),
@bazovoe varchar(50), @srednee varchar(50),
@dnevnaya varchar(50), @zaochnaya varchar(50)
AS
BEGIN
SELECT ��������������,  --0
		�������������.������������, --1 
		��������������.������������, --2
		�������������.������������, --3
		�����������, --4
		����������.ID����������, --5 
		(SELECT COUNT(*)FROM ���������� WHERE ID����������� = ����������.ID����������),-- 6
		�������������.���, --7
		�������������.ID�������������, -- 8
		��������������.ID��������������, --7
		����������,--10
		���������������������, --11
		��, -- 12
		�������������.ID������������� -- 13
	FROM ���������� JOIN ������������� ON(����������.ID������������� = �������������.ID�������������) 
		JOIN �������������� ON(����������.ID�������������� = ��������������.ID��������������) 
		JOIN ������������� ON(����������.ID������������� = �������������.ID�������������) 
	WHERE �������������.������������������� = @specialost
		AND (��������������.������������ like @budjet OR ��������������.������������ like @hozrash)
		AND (����������� like @bazovoe OR ����������� like @srednee) 
		AND (�������������.������������ like @dnevnaya OR �������������.������������ like @zaochnaya)
END
GO
/****** Object:  StoredProcedure [dbo].[Get_PlanPrieaByID]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[Get_PlanPrieaByID]
@id int
AS
BEGIN
SELECT ID�������������, ID�������������, ID��������������, ����������, ���������������������, ��������������,
	(SELECT ��� FROM ������������� WHERE �������������.ID������������� = ����������.ID�������������), 
	(SELECT ������������ FROM ������������� WHERE �������������.ID������������� = ����������.ID�������������), 
	(SELECT ������������ FROM ������������� WHERE �������������.ID������������� = ����������.ID�������������), 
	(SELECT ����������� FROM ������������� WHERE �������������.ID������������� = ����������.ID�������������),
	(SELECT ������������ FROM �������������� WHERE ��������������.ID�������������� = ����������.ID��������������), �� 
FROM ���������� WHERE ID���������� = @id
End
GO
/****** Object:  StoredProcedure [dbo].[Get_PlanPrieaBySpeciality]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[Get_PlanPrieaBySpeciality]
@spec nvarchar(50)
AS
BEGIN
SELECT ID����������, ID�������������, ID�������������, ID��������������, ����������, ���������������������, ��������������,
	(SELECT ��� FROM ������������� WHERE �������������.ID������������� = ����������.ID�������������), 
	(SELECT ������������ FROM ������������� WHERE �������������.ID������������� = ����������.ID�������������), 
	(SELECT ������������ FROM ������������� WHERE �������������.ID������������� = ����������.ID�������������), 
	(SELECT ����������� FROM ������������� WHERE �������������.ID������������� = ����������.ID�������������),
	(SELECT ������������ FROM �������������� WHERE ��������������.ID�������������� = ����������.ID��������������), �� 
FROM ���������� WHERE (SELECT ������������������� FROM ������������� WHERE �������������.ID������������� = ����������.ID�������������) = @spec
End
GO
/****** Object:  StoredProcedure [dbo].[Get_PlanPriemaID]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_PlanPriemaID]
@speciality varchar(50),
@formOfEducation varchar(50),
@financing varchar(50),
@education varchar(50)
AS
BEGIN
	SELECT ID���������� FROM ���������� JOIN ������������� ON(����������.ID������������� = �������������.ID�������������) 
										JOIN ������������� ON (����������.ID������������� = �������������.ID�������������) 
										JOIN �������������� ON (����������.ID�������������� = ��������������.ID��������������) 
	WHERE (�������������.������������������� LIKE @speciality OR �������������.������������ LIKE @speciality) AND 
		  �������������.������������ LIKE @formOfEducation AND 
		  ��������������.������������ LIKE @financing AND 
		  �������������.����������� LIKE @education
END
GO
/****** Object:  StoredProcedure [dbo].[Get_SpecialnostiName]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_SpecialnostiName]
@useFilter bit
AS
BEGIN
	SELECT �������������������, ������������ FROM ������������� WHERE ID������������� = ANY (SELECT ID������������� FROM ����������) OR @useFilter = 0
END
GO
/****** Object:  StoredProcedure [dbo].[Get_StatiAbiturienta]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_StatiAbiturienta]
@abiturient int
AS
BEGIN
	SELECT ������������ FROM ������ JOIN ����������������� ON (������.ID������ = �����������������.ID������) WHERE �����������������.ID����������� = @abiturient
END
GO
/****** Object:  StoredProcedure [dbo].[GetAbiturientCountForStats]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[GetAbiturientCountForStats]
@IDPlanPriema int,
@minMark float,
@maxMark float
AS
BEGIN
	SELECT ROUND(AVG(��������������������),1)
		FROM ���������� JOIN ������� ON (����������.ID����������� = �������.ID�����������)
		WHERE ����������.ID����������� = @IDPlanPriema 
			and ����������.ID����������� != all(SELECT ID����������� FROM ���������� WHERE ����������.ID����������� = @IDPlanPriema AND ����������.�������������� = 1)
			and ����������.ID����������� != all((SELECT ID����������� FROM ���������� WHERE ����������.ID����������� = @IDPlanPriema AND (����������.������ = 1 OR (SELECT MIN(���������) FROM ����������������� JOIN ������ ON (�����������������.ID������ = ������.ID������) WHERE ����������.ID����������� = ID�����������)= 0)))
		GROUP BY ����������.ID�����������
		HAVING ROUND(AVG(��������������������),1) >= @minMark AND ROUND(AVG(��������������������),1) <= @maxMark
END
GO
/****** Object:  StoredProcedure [dbo].[GetStats]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[GetStats]
@spec varchar(50)
AS
BEGIN
SELECT ����������.ID����������,
	   CASE WHEN (SELECT ������������ FROM �������������� WHERE ��������������.ID�������������� = ����������.ID��������������) = '������' THEN ���������� ELSE 0 END AS �����,
	   CASE WHEN (SELECT ������������ FROM �������������� WHERE ��������������.ID�������������� = ����������.ID��������������) = '������' THEN ��������������������� ELSE 0 END AS [� ��� ����� �� �������� ������� ����������],
	   CASE WHEN (SELECT ������������ FROM �������������� WHERE ��������������.ID�������������� = ����������.ID��������������) = '������' THEN 0 ELSE ���������� END [�� �������� ������],
	   (SELECT COUNT(*) FROM ���������� WHERE ����������.ID����������� = ����������.ID����������),
	   (SELECT COUNT(*) FROM ���������� WHERE ����������.ID����������� = ����������.ID���������� AND ����������.�������������� = 1),
	   (SELECT COUNT(*) FROM ���������� WHERE ����������.ID����������� = ����������.ID���������� AND (����������.������ = 1 OR (SELECT MIN(���������) FROM ����������������� JOIN ������ ON (�����������������.ID������ = ������.ID������) WHERE ����������.ID����������� = ID�����������)= 0))
FROM ����������
WHERE @spec = (SELECT ������������������� FROM ������������� WHERE �������������.ID������������� = ����������.ID�������������)
END
GO
/****** Object:  StoredProcedure [dbo].[HasStatya]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[HasStatya]
@abiturient int,
@statya nvarchar(50)
AS
BEGIN
SELECT * FROM ����������������� WHERE ID����������� = @abiturient AND ID������ = (SELECT ID������ FROM ������ WHERE ������������������ = @statya)
END
GO
/****** Object:  StoredProcedure [dbo].[ImportAD]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[ImportAD]
@id int
as
begin
SELECT �������, ���, ��������, 
		(SELECT TOP(1) �������� FROM ���������������� WHERE ID����������� = (SELECT ID����������� FROM ����������� WHERE ������������ = '�������� �������' AND ����������������.ID����������� = ����������.ID�����������)),
		(SELECT TOP(1) �������� FROM ���������������� WHERE ID����������� = (SELECT ID����������� FROM ����������� WHERE ������������ = '��������� �������' AND ����������������.ID����������� = ����������.ID�����������)),
		(SELECT TOP(1) �������� FROM ���������������� WHERE ID����������� = (SELECT ID����������� FROM ����������� WHERE ������������ = '�������� �����' AND ����������������.ID����������� = ����������.ID�����������)),
		(SELECT ������������ FROM ���������������� WHERE ����������������.ID����������� = ����������.ID�����������),
		(SELECT ����� FROM ���������������� WHERE ����������������.ID����������� = ����������.ID�����������),
		(SELECT ������������� FROM ���������������� WHERE ����������������.ID����������� = ����������.ID�����������),
		(SELECT ���������������������� FROM ���������������� WHERE ����������������.ID����������� = ����������.ID�����������)
	FROM ����������
	where ID����������� = @id AND ������������������ = 1
end
GO
/****** Object:  StoredProcedure [dbo].[InsertSpeciality]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[InsertSpeciality]
@Title nvarchar(50),
@ShortTitle nvarchar(20),
@Letter char(1),
@Code nvarchar(13)
AS
BEGIN
INSERT INTO ������������� (������������, �������������������, �����, ���)
VALUES (@Title, @ShortTitle, @Letter, @Code)
END
GO
/****** Object:  StoredProcedure [dbo].[NextExamList]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[NextExamList]
@id int
AS
BEGIN
SELECT MAX([dbo].[GetNum](������������������� + 'aaa')) + 1
from ����������
where ID����������� = @id
END
GO
/****** Object:  StoredProcedure [dbo].[Update_MainData]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Update_MainData]
@surename nvarchar(50),
@name nvarchar(50),
@otchestvo nvarchar(50),
@shool nvarchar(50),
@graduationYear int,
@grajdaninRB bit,
@grajdanstvo nvarchar(50),
@obshejitie bit,
@planPriema int,
@workPlase nvarchar(50),
@doljnost nvarchar(50),
@sirota bit,
@dogovor bit,
@redaktor int,
@abiturient int,
@ExamList varchar(10)
AS
BEGIN
	UPDATE ���������� SET ������� = @surename,
						  ��� = @name,
						  �������� = @otchestvo,
						  ����� = @shool,
						  ����������������� = @graduationYear,
						  ����������� = @grajdaninRB,
						  ����������� = @grajdanstvo,
						  ��������� = @obshejitie, 
						  ID����������� = @planPriema,
						  ����������� = @workPlase,
						  ��������� = @doljnost,
						  ������ = @sirota,
						  �������������� = @dogovor,
						  ID��������� = @redaktor,
						  ������������������ = GETDATE(),
						  ������������������� = @ExamList
					WHERE ID����������� = @abiturient
END
GO
/****** Object:  StoredProcedure [dbo].[Update_PasportData]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Update_PasportData]
@dateVidachi date,
@dateOfBirth date,
@seriya nvarchar(5),
@pasportNum nvarchar(15),
@vidan nvarchar(70),
@identNum nvarchar(50),
@abiturient int
AS
BEGIN
	UPDATE ���������������� SET 
				���������� = @dateVidachi,
				������������ = @dateOfBirth,
				����� = @seriya,
				������������� = @pasportNum,
				������������������ = @vidan,
				���������������������� = @identNum 
		WHERE ID����������� = @abiturient
END
GO
/****** Object:  StoredProcedure [dbo].[Update_PlanPriema]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Update_PlanPriema]
@id int,
@spec varchar(50),
@form varchar(50),
@fin varchar(50),
@obr varchar(50),
@kolva int,
@kolvaCel int,
@CT bit
AS
BEGIN
UPDATE ���������� 
	SET ID������������� = (SELECT ID������������� FROM ������������� WHERE ������������ = @spec),
		ID������������� = (SELECT ID������������� FROM ������������� WHERE ������������ = @form AND ����������� = @obr),
		ID�������������� = (SELECT ID�������������� FROM �������������� WHERE ������������ = @fin),
		���������� = @kolva,
		��������������������� = @kolvaCel,
		�� = @CT
	WHERE
		ID���������� = @id
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateSpeciality]    Script Date: 14.06.2022 9:39:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[UpdateSpeciality]
@Title nvarchar(50),
@ShortTitle nvarchar(20),
@Letter char(1),
@Code nvarchar(13),
@ID int
AS
BEGIN
UPDATE ������������� SET
������������ = @Title,
������������������� = @ShortTitle,
����� = @Letter,
��� = @Code
WHERE ID������������� = @ID
END
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "����������"
            Begin Extent = 
               Top = 138
               Left = 38
               Bottom = 268
               Right = 272
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "�������"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 273
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'GetAbiturientData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'GetAbiturientData'
GO
